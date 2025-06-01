extends CharacterBody2D

@onready var player = get_node("/root/Main/Character")
@export var stats: MobStats

@export var shot_rotation: Node2D

@onready var speed = stats.speed
@onready var max_health = stats.max_health
@onready var current_health = stats.current_health
@onready var attack_speed = stats.attack_speed
@onready var damage = stats.damage
@onready var fire_rate: Timer = %FireRate

func _physics_process(delta: float) -> void:
	var direction = global_position.direction_to(player.global_position)
	velocity = direction * speed
	move_and_slide()
	shot_rotation.look_at(player.global_position)
	#if speed == 0:
		#fire_rate.start()
	#else:
		#fire_rate.stop()
	
	

func shoot():
	const SMALL_FIRE_BALL = preload("res://Scenes/small_fire_ball.tscn")
	var new_fire_ball = SMALL_FIRE_BALL.instantiate()
	new_fire_ball.global_position = %ShootingPoint.global_position
	new_fire_ball.global_rotation = %ShootingPoint.global_rotation
	%ShootingPoint.add_child(new_fire_ball)
	

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
