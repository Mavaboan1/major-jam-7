extends Area2D

var travelled_distance = 0
@export var stats: MobStats
var damage = stats.damage

func _physics_process(delta: float) -> void:
	const RANGE = 1200
	
	var direction = Vector2.RIGHT.rotated(rotation)
	position += direction * 500.0 * delta
	
	travelled_distance += 500.0 * delta
	if travelled_distance > RANGE:
		queue_free()


func _on_body_entered(body: Node2D) -> void:
	queue_free()
	if body.has_method("takeDamage"):
		body.takeDamage(damage)
