extends CharacterBody2D

@onready var player = get_node("/root/Main/character")
@export var stats: MobStats

func _physics_process(delta: float) -> void:
	var direction = global_position.direction_to(player.global_position)
	velocity = direction * stats.speed
	move_and_slide()
