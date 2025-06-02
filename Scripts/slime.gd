extends CharacterBody2D

@onready var player = get_node("/root/Main/Character")
@export var stats: MobStats
@onready var health_bar: ProgressBar = $HealthBar

@onready var health = stats.max_health

var knockback_timer = 0
var knockback_force: Vector2
const SMALL_EXP_SHARD = preload("res://Scenes/small_exp_shard.tscn")
const SMALL_WILD_SHARD = preload("res://Scenes/small_wild_shard.tscn")

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
	shard_drop()
	queue_free()
	
func shard_drop():
	var exp_shard = SMALL_EXP_SHARD.instantiate()
	exp_shard.global_position = global_position
	get_tree().current_scene.add_child(exp_shard)
	
	var wild_shard = SMALL_WILD_SHARD.instantiate()
	wild_shard.global_position = global_position + Vector2.UP * 30
	get_tree().current_scene.add_child(wild_shard)
