extends Area2D

@export var stats: MobStats
@onready var player = get_node("/root/Main/Character")
@onready var health = stats.max_health
@onready var health_bar: ProgressBar = $HealthBar

func _ready() -> void:
	health_bar.max_value = stats.max_health
	health_bar.value = stats.max_health

func _physics_process(delta: float) -> void:
	var direction = player.global_position.normalized()
	rotation = direction.angle() + 1.5708
	position += direction * stats.speed * delta
	
	
	health_bar.value = health

	if health <= 0:
		die()

func takeDamage(damage):
	health -= damage

func die():
	queue_free()
	



func _on_body_entered(body: Node2D) -> void:
	if body.has_method("takePlayerDamage"):
		body.takePlayerDamage(stats.damage)


func _on_visible_on_screen_notifier_2d_screen_exited() -> void:
	queue_free()
