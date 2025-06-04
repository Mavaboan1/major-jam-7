extends Area2D


@export var spawn_radius: float = 50.0 # How close to player to spawn
@export var explosion_delay: float = 3.0
@export var bounce_force: float = 300.0
@export var fragment_count: int = 4
@export var fragment_scene: PackedScene # Assign smaller ball scene in the editor
@export var shrink_factor: float = 0.7 # How much smaller the fragments areawn_radius
@export var mobs_stat: MobStats

var player_reference: Node2D
var velocity: Vector2
var has_exploded: bool = false
var animated_sprite: AnimatedSprite2D
var is_casting: bool = true
var explosion_timer: Timer

func _ready() -> void:
	animated_sprite = get_node("AnimatedSprite2D")
	if not animated_sprite:
		push_error("RandomSpawnBall needs an AnimatedSprite2D child node")
		return
	
	animated_sprite.animation_finished.connect(_on_animation_finished)
	
	body_entered.connect(_on_collision)
	
	animated_sprite.play("cast")
	#is_casting = false
	#_start_movement()
	
	
	velocity = Vector2(
		randf_range(-bounce_force, bounce_force),
		randf_range(-bounce_force, bounce_force)
	)
	
	explosion_timer = Timer.new()
	add_child(explosion_timer)
	explosion_timer.wait_time = explosion_delay
	explosion_timer.one_shot = true
	explosion_timer.timeout.connect(_explode)
	explosion_timer.start()
	
func _physics_process(delta: float) -> void:
	if not has_exploded and not is_casting:
		velocity.y += 300 * delta # Gravity
		global_position += velocity * delta
		
		# Simple bounce off screen edges (optional)
		var screen_size = get_viewport().get_visible_rect().size
		if global_position.x < 0 or global_position.x > screen_size.x:
			velocity.x *= -0.8
		if global_position.y < 0:
			velocity.y *= -0.8
		elif global_position.y > screen_size.y:
			velocity.y = abs(velocity.y) * -0.8

func _start_movement():
	print("Starting movement, velocity: ", velocity)
	if animated_sprite:
		animated_sprite.play("movement")
	
	

func _on_animation_finished():
	match animated_sprite.animation:
		"cast":
			is_casting = false
			_start_movement()
			print("Cast finished, starting movement")
		"hit":
			# Animation finished, now actually remove the projectile
			queue_free()

func spawn_near_player(player: CharacterBody2D):
	player_reference = player
	
	# Calculate random position around player
	var angle = randf() * TAU
	var distance = randf_range(spawn_radius * 0.5, spawn_radius)
	var offset = Vector2(cos(angle), sin(angle)) * distance
	
	global_position = player.global_position + offset
	
func _explode():
	if has_exploded:
		return
	
	has_exploded = true
	
	velocity = Vector2.ZERO
	
	# Spawn smaller versions
	_spawn_smaller_balls()
	
	if animated_sprite:
		animated_sprite.play("hit")
		# Wait for animation to finish before spawning fragments and removing
		await animated_sprite.animation_finished
	
	
	
	if not (animated_sprite):
		queue_free()
	
func _spawn_smaller_balls():
	if not fragment_scene:
		return
	
	var parent = get_parent()
	if not parent:
		return
	
	for i in fragment_count:
		var smaller_ball = fragment_scene.instantiate()
		parent.add_child(smaller_ball)
		
		# Position at explosion point
		smaller_ball.global_position = global_position
		
		# Scale down the smaller ball
		smaller_ball.scale = scale * shrink_factor
		
		# Calculate spread direction
		var base_angle = (TAU / fragment_count) * i
		var random_offset = randf_range(-0.8, 0.8)
		var angle = base_angle + random_offset
		var direction = Vector2(cos(angle), sin(angle))
		
		
		# Set initial velocity for fragments
		if smaller_ball.has_method("set_velocity"):
			var fragment_velocity = direction * bounce_force * randf_range(0.6, 1.2)
			smaller_ball.set_velocity(fragment_velocity)
			print("Set fragment velocity: ", fragment_velocity)
		elif smaller_ball.has_method("_set_initial_velocity"):
			smaller_ball._set_initial_velocity(direction * bounce_force * 0.8)
		
		# Reduce explosion delay for smaller balls to prevent infinite recursion
		if smaller_ball.has_method("set_explosion_delay"):
			var new_delay = explosion_delay * 0.6
			smaller_ball.set_explosion_delay(new_delay)
			print("Set fragment explosion delay: ", new_delay)
			
		# Make sure the fragment isn't in casting mode
		if smaller_ball.has_method("start_immediately"):
			smaller_ball.start_immediately()

func set_velocity(vel: Vector2):
	velocity = vel

func set_explosion_delay(delay: float):
	explosion_delay = delay
	# Update existing timer if it exists
	if explosion_timer and is_instance_valid(explosion_timer):
		explosion_timer.wait_time = delay

func start_immediately():
	is_casting = false
	if animated_sprite:
		animated_sprite.play("movement")

func _on_collision(body):
	if body != self:
		if body.has_method("takePlayerDamage"):
			body.takePlayerDamage(mobs_stat.damage)
		# Simple bounce logic - reverse velocity with some damping
		velocity *= -0.6
