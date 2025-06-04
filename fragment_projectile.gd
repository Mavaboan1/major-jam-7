extends Area2D

@export var lifetime: float = 3.0
@export var pro_gravity: float = 400.0
@export var stats: MobStats
@export var damage: int

var velocity: Vector2
var has_hit: bool = false
var animated_sprite: AnimatedSprite2D

func _ready():
	damage = stats.damage
	
	# Get AnimatedSprite2D reference
	animated_sprite = get_node("AnimatedSprite2D")
	if not animated_sprite:
		push_error("FragmentProjectile needs an AnimatedSprite2D child node")
		return
	
	# Connect animation finished signal
	animated_sprite.animation_finished.connect(_on_animation_finished)
	
	# Set up collision detection
	body_entered.connect(_on_collision)
	
	# Start with movement animation (fragments skip cast animation)
	animated_sprite.play("movement")
	
	# Set up lifetime timer
	var timer = Timer.new()
	add_child(timer)
	timer.wait_time = lifetime
	timer.one_shot = true
	timer.timeout.connect(_on_lifetime_expired)
	timer.start()
	
func _physics_process(delta):
	if not has_hit:
		# Apply gravity
		velocity.y += gravity * delta
		
		# Move fragment
		global_position += velocity * delta
		
		# Rotate sprite based on velocity direction (optional visual effect)
		if animated_sprite and velocity.length() > 0:
			animated_sprite.rotation = velocity.angle()
		
func set_velocity(vel: Vector2):
	velocity = vel

func _on_collision(body):
	if body != self and not has_hit:
		# Check if it's an enemy or damageable object
		if body.has_method("takePlayerDamage"):
			body.takePlayerDamage(damage)
		
		# Hit anything - no bouncing
		_hit()
		
func _hit():
	if has_hit:
		return
	
	has_hit = true
	velocity = Vector2.ZERO
	
	# Play hit animation if available
	if animated_sprite:
		animated_sprite.play("hit")
		# Wait for animation before removing
	else:
		# No hit animation, remove immediately
		queue_free()

func _on_lifetime_expired():
	_hit()

func _on_animation_finished():
	if animated_sprite.animation == "hit":
		queue_free()
