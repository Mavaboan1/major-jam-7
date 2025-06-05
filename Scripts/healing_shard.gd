extends Area2D

#Ranged signal

@export var exp_value = 5

@export var speed: float = 150
@export var magnet_distance: float = 60.0

@onready var player: CharacterBody2D = get_node("/root/Main/Character")

@export var audio: AudioStreamPlayer2D

func _ready() -> void:
	player.rangeSignal.connect(_on_range_up)

func _physics_process(delta: float) -> void:
	if not player:
		return
	
	var dist = position.distance_to(player.global_position)
	if dist < magnet_distance:
		position = position.move_toward(player.global_position, speed * delta)

func _on_body_entered(body: Node2D) -> void:
	if body.has_method("Heal"):
		body.Heal(exp_value)
	
	queue_free()


func _on_range_up(_range):
	print("Magnet up")
	magnet_distance = _range
