extends CharacterBody2D

@onready var player = get_node("/root/Main/Character")
@export var stats: MobStats
@onready var health_bar: ProgressBar = $HealthBar

@onready var health = stats.max_health

var knockback_timer = 0
var knockback_force: Vector2

func _ready() -> void:
	health_bar.max_value = stats.max_health
	health_bar.value = stats.max_health

func _physics_process(delta: float) -> void:
	if knockback_timer > 0:
		velocity = knockback_force
		knockback_timer -= delta
		move_and_slide()
	else:
		var direction = global_position.direction_to(player.global_position)
		velocity = direction * stats.speed
		move_and_slide()
	
	health_bar.value = health

	if health <= 0:
		die()

func apply_knockback(force: Vector2):
	knockback_timer = 0.2
	knockback_force = force
	

func takeDamage(damage):
	health -= damage

func die():
	queue_free()
