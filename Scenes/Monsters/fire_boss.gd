extends CharacterBody2D

enum EnemyState {
	IDLE,
	CHASING,
	ATTACKING,
	STUNNED,
	DYING
}

@onready var player: CharacterBody2D = get_node("/root/Main/Character")
@onready var animated_sprite: AnimatedSprite2D = $AnimatedSprite2D
@onready var fire_rate_timer: Timer = $FireRateTimer
@onready var shooting_point: Marker2D = %ShootingPoint
@onready var shot_rotation: Node2D = $ShotRotation

@export var stats: MobStats
@export var projectile_scene: PackedScene
@export var lava_scene: PackedScene

var current_health: float
var current_state: EnemyState = EnemyState.IDLE
var knockback_timer: float = 0.0
var knockback_force: Vector2 = Vector2.ZERO
var last_direction: Vector2 = Vector2.ZERO

const KNOCKBACK_DURATION: float = 0.2
const SHARD_OFFSET: Vector2 = Vector2.UP * 30
const ANIMATION_TRESHOLD: float = 10.0

const BIG_EXP_SHARD = preload("res://Scenes/big_exp_shard.tscn")
const BIG_WILD_SHARD = preload("res://Scenes/big_wild_shard.tscn")

signal died
signal health_changed(new_health: float, max_health: float)
@warning_ignore("unused_signal")
signal state_changed(old_state: EnemyState, new_state: EnemyState)

func _ready() -> void:
	validate_setup()
	initialize_stats()
	setup_shooting()
	change_state(EnemyState.IDLE)
	

func validate_setup() -> void:
	if not stats:
		push_error("Mobstats resource not assigned to " + str(name))
	
	if not animated_sprite:
		push_error("AnimatedSprite2D not found " + str(name))
		
func initialize_stats() -> void:
	current_health = stats.max_health
	
func _physics_process(delta: float) -> void:
	handle_knockback(delta)
	update_stats_machine(delta)
	execute_current_state(delta)
	handle_animation()
	handle_rotation()
	

func update_stats_machine(_delta: float) -> void:
	if not is_instance_valid(player):
		change_state(EnemyState.IDLE)
		return
	
	var distance = get_distance_to_player()
	var chase_dist = 1200
	var attack_dist = 150.0
	
	match current_state:
		EnemyState.IDLE:
			if distance <= chase_dist:
				change_state(EnemyState.CHASING)
		EnemyState.CHASING:
			if distance <= attack_dist:
				change_state(EnemyState.ATTACKING)
		EnemyState.ATTACKING:
			if distance > attack_dist * 1.2:
				change_state(EnemyState.CHASING)
		EnemyState.STUNNED:
			if knockback_timer <= 0:
				if distance <= attack_dist:
					change_state(EnemyState.ATTACKING)
				elif distance <= chase_dist:
					change_state(EnemyState.CHASING)
				else:
					change_state(EnemyState.IDLE)

func execute_current_state(delta: float) -> void:
	if knockback_timer > 0:
		return
	
	match current_state:
		EnemyState.IDLE:
			velocity = velocity.move_toward(Vector2.ZERO, stats.speed * delta * 2)
		
		EnemyState.CHASING:
			var direction = get_direction_to_player()
			velocity = direction * stats.speed
			last_direction = direction
		
		EnemyState.ATTACKING:
			velocity = Vector2.ZERO  # Stop to shoot
		
		EnemyState.STUNNED, EnemyState.DYING:
			velocity = Vector2.ZERO
	
	move_and_slide()
	
	# Handle rotation for shooting
	if shot_rotation and is_instance_valid(player):
		shot_rotation.look_at(player.global_position)

func handle_animation() -> void: #TODO
	pass

func handle_knockback(delta: float) -> void:
	if knockback_timer > 0:
		velocity = knockback_force
		knockback_timer -= delta
		move_and_slide()

func change_state(new_state: EnemyState) -> void:
	if current_state == new_state:
		return
	
	var _old_state = current_state
	exit_state(current_state)
	current_state = new_state
	enter_state(new_state)

func enter_state(state: EnemyState) -> void:
	match state:
		EnemyState.ATTACKING:
			fire_rate_timer.start()
		_:
			fire_rate_timer.stop()

func exit_state(state: EnemyState) -> void:
	match state:
		EnemyState.ATTACKING:
			fire_rate_timer.stop()
		_:
			pass

func takeDamage(damage_amount: float) -> void:
	if damage_amount <= 0 or current_state == EnemyState.DYING:
		return
	
	
	current_health = max(0, current_health - damage_amount)
	health_changed.emit(current_health, stats.max_health)
	
	show_damage_feedback()

func show_damage_feedback() -> void:
	if animated_sprite:
		var tween = create_tween()
		tween.tween_property(animated_sprite, "modulate", Color.WHITE * 1.5, 0.1)
		tween.tween_property(animated_sprite, "modulate", Color.WHITE, 0.1)

func die() -> void:
	died.emit()
	drop_shards()
	queue_free()

func drop_shards() -> void:
	var current_scene = get_tree().current_scene
	if not current_scene:
		return
	
	var exp_shard = BIG_EXP_SHARD.instantiate()
	exp_shard.global_position = global_position
	current_scene.add_child(exp_shard)
	
	var wild_shard = BIG_WILD_SHARD.instantiate()
	wild_shard.global_position = global_position + SHARD_OFFSET
	current_scene.add_child(wild_shard)
	
func apply_knockback(force: Vector2) -> void:
	var resistance = stats.knockback_resistance if stats.has_method("knockback_resistance") else 1.0
	knockback_timer = KNOCKBACK_DURATION
	knockback_force = force * resistance
	change_state(EnemyState.STUNNED)

# Shooting functions
func setup_shooting() -> void:
	if fire_rate_timer:
		fire_rate_timer.wait_time = stats.attack_speed
		fire_rate_timer.timeout.connect(_on_fire_rate_timer_timeout)

func shoot():
	if not shooting_point:
		push_warning("ShootingPoint Not found")
		return
	var new_projectile
	var random_projectile = randi_range(0,1)
	if random_projectile == 0:
		new_projectile = projectile_scene.instantiate()
		add_child(new_projectile)
		new_projectile.global_position = shooting_point.global_position
		new_projectile.launch_to_target(player.global_position)
	elif random_projectile == 1:
		new_projectile = lava_scene.instantiate()
		add_child(new_projectile)
		new_projectile.spawn_near_player(player)
	else:
		push_error("Couldn't shoot")



func handle_rotation() -> void:
	if shot_rotation and is_instance_valid(player):
		shot_rotation.look_at(player.global_position)

# Utility functions
func get_distance_to_player() -> float:
	if is_instance_valid(player):
		return global_position.distance_to(player.global_position)
	return -1.0

func get_direction_to_player() -> Vector2:
	if is_instance_valid(player):
		return global_position.direction_to(player.global_position)
	return Vector2.ZERO


# SIGNALS

func _on_fire_rate_timer_timeout():
	shoot()
