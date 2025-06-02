extends CharacterBody2D

@onready var player = get_node("/root/Main/Character")
@export var stats: MobStats
@onready var health_bar: ProgressBar = $HealthBar

@onready var health = stats.max_health
@onready var speed = 0

var knockback_timer = 0
var knockback_force: Vector2
const BIG_EXP_SHARD = preload("res://Scenes/big_exp_shard.tscn")
const BIG_WILD_SHARD = preload("res://Scenes/big_wild_shard.tscn")

func _ready() -> void:
	health_bar.max_value = stats.max_health
	health_bar.value = stats.max_health
	$AnimatedSprite2D.play("wait")

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
	
	if velocity.x < 0:
		$AnimatedSprite2D.play("walk_left")
	elif velocity.x > 0:
		$AnimatedSprite2D.play("walk_right")
	elif velocity.y < 0:
		$AnimatedSprite2D.play("walk_down")
	elif velocity.y > 0:
		$AnimatedSprite2D.play("walk_up")
		

func apply_knockback(force: Vector2):
	knockback_timer = 0.2
	knockback_force = force / 2

func takeDamage(damage):
	health -= damage

func die():
	shard_drop()
	queue_free()

func shard_drop():
	var exp_shard = BIG_EXP_SHARD.instantiate()
	exp_shard.global_position = global_position
	get_tree().current_scene.add_child(exp_shard)
	
	var wild_shard = BIG_WILD_SHARD.instantiate()
	wild_shard.global_position = global_position + Vector2.UP * 30
	get_tree().current_scene.add_child(wild_shard)
	
func _on_dange_zone_body_entered(body: Node2D) -> void:
	if body.has_method("takePlayerDamage"):
		speed = stats.speed


func _on_hurt_box_body_entered(body: Node2D) -> void:
	if body.has_method("takePlayerDamage"):
		body.takePlayerDamage(stats.damage)
