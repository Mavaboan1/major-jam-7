extends CharacterBody2D

@onready var player = get_node("/root/Main/Character")
@export var stats: MobStats
@onready var health_bar: ProgressBar = $HealthBar
@onready var death_markers: Node2D = $DeathMarkers
@onready var health = stats.max_health

func _ready() -> void:
	health_bar.max_value = stats.max_health
	health_bar.value = stats.max_health
	$AnimatedSprite2D.play("default")

func _physics_process(_delta: float) -> void:
	var direction = global_position.direction_to(player.global_position)
	velocity = direction * stats.speed
	move_and_slide()
	
	health_bar.value = health

	if health <= 0:
		die()

func spawn_ice():
	for i in death_markers.get_children():
		shoot(i)
func shoot(marker: Marker2D):
	const DYING_ICICLE = preload("res://Scenes/DyingIcicle.tscn")
	var new_dying_icicle = DYING_ICICLE.instantiate()
	new_dying_icicle.global_position = marker.global_position
	new_dying_icicle.global_rotation = marker.global_rotation
	get_tree().current_scene.add_child(new_dying_icicle)

func takeDamage(damage):
	health -= damage

func die():
	spawn_ice()
	queue_free()
