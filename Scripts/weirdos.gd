extends CharacterBody2D

@onready var player = get_node("/root/Main/Character")
@export var stats: MobStats
@onready var health_bar: ProgressBar = $HealthBar
@onready var death_markers: Node2D = $DeathMarkers
@onready var health = stats.max_health

const SMALL_EXP_SHARD = preload("res://Scenes/small_exp_shard.tscn")
const SMALL_WILD_SHARD = preload("res://Scenes/small_wild_shard.tscn")

signal died

var knockback_timer = 0
var knockback_force: Vector2

func _ready() -> void:
	health_bar.max_value = stats.max_health
	health_bar.value = stats.max_health
	$AnimatedSprite2D.play("default")

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

func spawn_ice():
	for i in death_markers.get_children():
		shoot(i)
		
func shoot(marker: Marker2D):
	const DYING_ICICLE = preload("res://Scenes/DyingIcicle.tscn")
	var new_dying_icicle = DYING_ICICLE.instantiate()
	new_dying_icicle.global_position = marker.global_position
	new_dying_icicle.global_rotation = marker.global_rotation
	get_tree().current_scene.add_child(new_dying_icicle)

func apply_knockback(force: Vector2):
	knockback_timer = 0.2
	knockback_force = force

func takeDamage(damage):
	health -= damage

func die():
	died.emit()
	shard_drop()
	spawn_ice()
	queue_free()
	
func shard_drop():
	var exp_shard = SMALL_EXP_SHARD.instantiate()
	exp_shard.global_position = global_position
	get_tree().current_scene.add_child(exp_shard)
	
	var wild_shard = SMALL_WILD_SHARD.instantiate()
	wild_shard.global_position = global_position + Vector2.UP * 30
	get_tree().current_scene.add_child(wild_shard)
