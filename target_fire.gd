extends Area2D

@export var speed: float = 300.0
@export var lifetime: float = 5.0
@export var explosion_force: float = 200.0
@export var fragment_count: int = 6
@export var fragment_scene: PackedScene

var target_position: Vector2
var velocity: Vector2
var has_exploded: bool = false
var animated_sprite: AnimatedSprite2D
var is_casting: bool = true
var lifetime_timer: Timer

func _ready() -> void:
	animated_sprite = get_node("AnimatedSprite2D")
	if not animated_sprite:
		push_error("target_fire needs an AnimatedSprite2D child node")
		return
	
	# Connect animation finished signal
	animated_sprite.animation_finished.connect(_on_animation_finished)
	
	body_entered.connect(_on_collision)
	
	animated_sprite.play("cast")
	#is_casting = false
	#_start_movement()
	
	lifetime_timer = Timer.new()
	add_child(lifetime_timer)
	lifetime_timer.wait_time = lifetime
	lifetime_timer.one_shot = true
	lifetime_timer.timeout.connect(_explode)
	lifetime_timer.start()

func _physics_process(delta: float) -> void:
	if not has_exploded and not is_casting:
		global_position += velocity * delta

func launch_to_target(target_pos: Vector2):
	target_position = target_pos
	var direction = (target_position - global_position).normalized()
	velocity = direction * speed
	
	rotation = direction.angle()
	
	if not is_casting:
		_start_movement()

func _start_movement():
	print("Starting movement, velocity: ", velocity) # Debug line
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

func _on_collision(body):
	if body != self and not has_exploded:
		_explode()

func _explode():
	if has_exploded:
		return
	
	has_exploded = true
	velocity = Vector2.ZERO
	
	call_deferred("_spawn_fragments")

	if animated_sprite:
		animated_sprite.play("hit")
		await animated_sprite.animation_finished
	
	queue_free()

func _spawn_fragments():
	if not fragment_scene:
		return
	
	var parent = get_parent()
	if not parent:
		return
	
	for i in fragment_count:
		var fragment = fragment_scene.instantiate()
		if not fragment:
			print("Warning: Failed to instantiate fragment_scene!")
			continue
		
		# Add to parent first
		parent.add_child(fragment)
		
		# Set position at explosion point
		fragment.global_position = global_position
		
		# Calculate direction with some randomness
		var base_angle = (TAU / fragment_count) * i
		var random_offset = randf_range(-0.5, 0.5)
		var angle = base_angle + random_offset
		var direction = Vector2(cos(angle), sin(angle))
		
		# Set fragment velocity
		if fragment.has_method("set_velocity"):
			var fragment_velocity = direction * explosion_force * randf_range(0.8, 1.2)
			fragment.set_velocity(fragment_velocity)
			print("Set fragment velocity: ", fragment_velocity)
		
		
		
		
		
