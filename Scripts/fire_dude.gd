extends CharacterBody2D

@onready var player = get_node("/root/Main/Character")
@onready var health_bar: ProgressBar = $HealthBar
@onready var fire_rate_timer: Timer = %FireRate
@onready var shooting_point: Marker2D = %ShootingPoint

@export var shot_rotation: Node2D
@export var stats: MobStats
@export var audio: AudioStreamPlayer2D

const KNOCKBACK_DURATION: float = 0.2
const SHARD_OFFSET: Vector2 = Vector2.UP * 30
 

var current_health: float
var knockback_timer = 0
var knockback_force: Vector2
var is_in_danger_zone: bool = false

signal died
signal health_changed(new_health: float, max_health: float)

const BIG_WILD_SHARD = preload("res://Scenes/big_wild_shard.tscn")
const BIG_EXP_SHARD = preload("res://Scenes/big_exp_shard.tscn")
const SMALL_FIRE_BALL = preload("res://Scenes/small_fire_ball.tscn")

@onready var sprite_2d: AnimatedSprite2D = $Sprite2D



func _ready() -> void:
	if not stats:
		push_error("MobStats resource not Assigned")
		return
	
	current_health = stats.max_health
	
	setup_health_bar()
	
	fire_rate_timer.wait_time = stats.attack_speed
	fire_rate_timer.timeout.connect(_on_timer_timeout)
	
func setup_health_bar() -> void:
	if health_bar:
		health_bar.max_value = stats.max_health
		health_bar.value = current_health

func _physics_process(delta: float) -> void:
	handle_movement(delta)
	handle_rotation()
	update_ui()
	check_death()
	play_movement_animation()

func handle_movement(delta: float) -> void:
	if knockback_timer > 0:
		velocity = knockback_force
		knockback_timer -= delta
	else:
		if not is_in_danger_zone and is_instance_valid(player):
			var direction = global_position.direction_to(player.global_position)
			velocity = direction * stats.speed
		else:
			velocity = Vector2.ZERO
	
	move_and_slide()
	
func handle_rotation() -> void:
	if shot_rotation and is_instance_valid(player):
		shot_rotation.look_at(player.global_position)

func update_ui():
	if health_bar:
		health_bar.value = current_health

func check_death() -> void:
	if current_health <= 0:
		die()

func shoot():
	if not shooting_point:
		push_warning("ShootingPoint Not found")
		return
	var new_fire_ball = SMALL_FIRE_BALL.instantiate()
	new_fire_ball.global_position = shooting_point.global_position
	new_fire_ball.rotation = shooting_point.global_rotation
	
	get_tree().current_scene.add_child(new_fire_ball)

func takeDamage(damage_taken):
	if damage_taken <= 0:
		return
	
	current_health = max(0, current_health - damage_taken)
	health_changed.emit(current_health, stats.max_health)
	show_damage_feedback()

func show_damage_feedback() -> void:
	#Visual feedback when taking damage
	if sprite_2d:
		var tween = create_tween()
		tween.tween_property(sprite_2d, "modulate", Color.WHITE * 1.5, 0.1)
		tween.tween_property(sprite_2d, "modulate", Color.WHITE, 0.1)
	
	audio.play()

func die():
	died.emit()
	shard_drop()
	queue_free()

func shard_drop():
	var current_scene = get_tree().current_scene
	if not current_scene:
		return
	
	var exp_shard = BIG_EXP_SHARD.instantiate()
	exp_shard.global_position = global_position
	current_scene.add_child(exp_shard)
	
	var wild_shard = BIG_WILD_SHARD.instantiate()
	wild_shard.global_position = global_position + Vector2.UP * 30
	current_scene.add_child(wild_shard)

func apply_knockback(force: Vector2):
	knockback_timer = KNOCKBACK_DURATION
	knockback_force = force

func play_movement_animation() -> void:
	if not sprite_2d:
		return
	
	var direction = velocity.normalized()
	if velocity == Vector2.ZERO:
		sprite_2d.play("idle")
	
	if abs(direction.x) > abs(direction.y):
		if direction.x < 0:
			sprite_2d.play("move_left")
		else:
			sprite_2d.play("move_right")
	else:
		if direction.y < 0:
			sprite_2d.play("move_up")  # Note: Y is flipped in Godot
		else:
			sprite_2d.play("move_down")

func _on_danger_zone_body_entered(body: Node2D) -> void:
	if body.has_method("takePlayerDamage"):
		is_in_danger_zone = true
		fire_rate_timer.start()
		
	
func _on_danger_zone_body_exited(body: Node2D) -> void:
	if body.has_method("takePlayerDamage"):
		is_in_danger_zone = false
		fire_rate_timer.stop()

func _on_timer_timeout() -> void:
	if is_in_danger_zone and is_instance_valid(player):
		shoot()
