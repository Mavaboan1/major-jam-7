extends CharacterBody2D

@onready var player = get_node("/root/Main/Character")
@export var stats: MobStats

@export var shot_rotation: Node2D
@onready var health_bar: ProgressBar = $HealthBar

@onready var speed = stats.speed
@onready var health = stats.max_health
@onready var attack_speed = stats.attack_speed
@onready var damage = stats.damage
@onready var fire_rate: Timer = %FireRate

var knockback_timer = 0
var knockback_force: Vector2

const BIG_WILD_SHARD = preload("res://Scenes/big_wild_shard.tscn")
const BIG_EXP_SHARD = preload("res://Scenes/big_exp_shard.tscn")

func _ready() -> void:
	fire_rate.wait_time = stats.attack_speed
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
	shot_rotation.look_at(player.global_position)
	health_bar.value = health
	if health <= 0:
		die()

func shoot():
	const SMALL_FIRE_BALL = preload("res://Scenes/small_fire_ball.tscn")
	var new_fire_ball = SMALL_FIRE_BALL.instantiate()
	new_fire_ball.global_position = %ShootingPoint.global_position
	new_fire_ball.global_rotation = %ShootingPoint.global_rotation
	%ShootingPoint.add_child(new_fire_ball)

func takeDamage(damage_taken):
	health -= damage_taken

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

func apply_knockback(force: Vector2):
	knockback_timer = 0.2
	knockback_force = force

func _on_danger_zone_body_entered(body: Node2D) -> void:
	if body.global_position == player.global_position:
		speed = 0
		fire_rate.start()
	
func _on_danger_zone_body_exited(body: Node2D) -> void:
	if body.global_position == player.global_position:
		speed = stats.speed
		fire_rate.stop()

func _on_timer_timeout() -> void:
	shoot()
