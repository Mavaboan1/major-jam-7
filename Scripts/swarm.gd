extends CharacterBody2D

@export var stats: MobStats
@onready var player = get_node("/root/Main/Character")
@onready var health = stats.max_health
@onready var health_bar: ProgressBar = $HealthBar
const SMALL_EXP_SHARD = preload("res://Scenes/small_exp_shard.tscn")
const SMALL_WILD_SHARD = preload("res://Scenes/small_wild_shard.tscn")
signal died

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
	died.emit()
	shard_drop()
	queue_free()
	
func shard_drop():
	var exp_shard = SMALL_EXP_SHARD.instantiate()
	exp_shard.global_position = global_position
	get_tree().current_scene.add_child(exp_shard)
	
	var wild_shard = SMALL_WILD_SHARD.instantiate()
	wild_shard.global_position = global_position + Vector2.UP * 30
	get_tree().current_scene.add_child(wild_shard)
	
func _on_visible_on_screen_notifier_2d_screen_exited() -> void:
	queue_free()


func _on_hurt_box_body_entered(body: Node2D) -> void:
	if body.has_method("takePlayerDamage"):
		body.takePlayerDamage(stats.damage)
