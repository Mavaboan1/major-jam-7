extends CharacterBody2D

@onready var player = get_node("/root/Main/Character")
@export var stats: MobStats

func _ready() -> void:
	var current_health: int = stats.current_health

func _physics_process(delta: float) -> void:
	var direction = global_position.direction_to(player.global_position)
	velocity = direction * stats.speed
	move_and_slide()
