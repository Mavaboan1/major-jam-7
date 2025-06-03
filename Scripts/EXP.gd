extends Area2D


@export var exp_value = 10

@export var speed: float = 150
@export var magnet_distance: float = 120.0

var player: CharacterBody2D = null

func _physics_process(delta: float) -> void:
	if not player:
		player = get_node("/root/Main/Character")
		return
	
	var dist = position.distance_to(player.global_position)
	if dist < magnet_distance:
		position = position.move_toward(player.global_position, speed * delta)

func _on_body_entered(body: Node2D) -> void:
	if body.has_method("getExp"):
		body.getExp(exp_value)
	queue_free()
