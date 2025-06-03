extends CharacterBody2D

enum MobState {
	IDLE,
	CHASING,
	ATTACKING,
	STUNNED,
	DYING
}

@onready var player = get_node("/root/Main/Character")
@onready var health_bar: ProgressBar = $HealthBar
@onready var animated_sprite: AnimatedSprite2D = $AnimatedSprite2D
@onready var attack_timer: Timer = $AttackTimer

@export var stats: MobStats


var current_health: float
var current_state: MobState = MobState.IDLE
var knockback_timer: float = 0.0
var knockback_force: Vector2 = Vector2.ZERO


var can_attack: bool = true
var attack_cooldown: float = 1.0  # Time between attacks
var last_direction: Vector2 = Vector2.ZERO


var chase_distance: float = 150.0
var attack_distance: float = 40.0
var lose_target_distance: float = 600.0


const KNOCKBACK_DURATION: float = 0.2
const SHARD_OFFSET: Vector2 = Vector2.UP * 30
const ANIMATION_THRESHOLD: float = 10.0  # Minimum velocity to trigger walk animation


const BIG_EXP_SHARD = preload("res://Scenes/big_exp_shard.tscn")
const BIG_WILD_SHARD = preload("res://Scenes/big_wild_shard.tscn")

# Signals
signal died
signal health_changed(new_health: float, max_health: float)
signal player_attacked(damage: float)

func _ready() -> void:
	if not stats:
		push_error("Mobstats Resource not assigned")
		return
	
	if not animated_sprite:
		push_error("AnimatedSprite2D not found!")
		
	current_health = stats.max_health
	
	setup_healthbar()
	
	change_state(MobState.IDLE)
	
	attack_timer.wait_time = stats.attack_speed

func setup_healthbar():
	if health_bar:
		health_bar.max_value = stats.max_health
		health_bar.value = current_health
		


func _physics_process(delta: float) -> void:
	handle_knockback(delta)
	update_state_machine(delta)
	execute_current_state(delta)
	handle_animations()
	update_ui()
		

func handle_knockback(delta: float) -> void:
	if knockback_timer:
		velocity = knockback_force
		knockback_timer -= delta
		move_and_slide()
		
func update_state_machine(delta: float) -> void:
	if not is_instance_valid(player):
		change_state(MobState.IDLE)
		return
	
	var distance_to_player = global_position.distance_to(player.global_position)
	
	match current_state:
		MobState.IDLE:
			if distance_to_player <= chase_distance:
				change_state(MobState.CHASING)
		
		MobState.CHASING:
			if distance_to_player <= attack_distance and can_attack:
				change_state(MobState.ATTACKING)
			elif distance_to_player > lose_target_distance:
				change_state(MobState.IDLE)
		
		MobState.ATTACKING:
			if distance_to_player > attack_distance:
				change_state(MobState.CHASING)
		
		MobState.STUNNED:
			if knockback_timer <= 0:
				if distance_to_player <= attack_distance and can_attack:
					change_state(MobState.ATTACKING)
				elif distance_to_player <= chase_distance:
					change_state(MobState.CHASING)
				else:
					change_state(MobState.IDLE)

func execute_current_state(delta: float) -> void:
	if knockback_timer > 0:
		return
	
	match current_state:
		MobState.IDLE:
			execute_idle_state(delta)
		MobState.CHASING:
			execute_chasing_state(delta)
		MobState.ATTACKING:
			execute_attacking_state(delta)
		MobState.STUNNED:
			velocity = Vector2.ZERO
		MobState.DYING:
			velocity = Vector2.ZERO

func execute_idle_state(delta: float) -> void:
	velocity = velocity.move_toward(Vector2.ZERO, stats.speed * delta * 2)

func execute_chasing_state(delta: float) -> void:
	if is_instance_valid(player):
		var direction = global_position.direction_to(player.global_position)
		velocity = direction * stats.speed
		last_direction = direction
	
func execute_attacking_state(delta: float) -> void:
	# Slight movement towards player during attack
	if is_instance_valid(player):
		var direction = global_position.direction_to(player.global_position)
		velocity = direction * stats.speed * 0.3  # Slower during attack
		last_direction = direction

func change_state(new_state: MobState) -> void:
	if current_state == new_state:
		return
	
	var old_state = current_state
	exit_state(current_state)
	current_state = new_state
	enter_state(new_state)

func enter_state(state: MobState) -> void:
	match state:
		MobState.IDLE:
			pass
		MobState.CHASING:
			pass
		MobState.ATTACKING:
			perform_attack()
		MobState.STUNNED:
			pass
		MobState.DYING:
			pass

func exit_state(state: MobState) -> void:
	match state:
		MobState.ATTACKING:
			can_attack = false
			attack_timer.start()
		_:
			pass

func perform_attack() -> void:
	if is_instance_valid(player):
		var distance_to_player = global_position.distance_to(player.global_position)
		
		if distance_to_player <= attack_distance:
			if player.has_method("takePlayerDamage"):
				player.takePlayerDamage(stats.damage)
				player_attacked.emit(stats.damage)

func handle_animations() -> void:
	if not animated_sprite:
		return
	
	match current_state:
		MobState.IDLE:
			if velocity.length() < ANIMATION_THRESHOLD:
				animated_sprite.play("wait")
			else:
				play_movement_animation()
		MobState.CHASING, MobState.ATTACKING:
			play_movement_animation()
		MobState.STUNNED:
			# Could add a stunned animation here
			animated_sprite.play("wait")
		MobState.DYING:
			# Could add a death animation here
			pass

func play_movement_animation() -> void:
	if not animated_sprite:
		return
	
	var direction = velocity.normalized()
	if direction.length() < 0.1:
		direction = last_direction
	
	if abs(direction.x) > abs(direction.y):
		if direction.x < 0:
			animated_sprite.play("walk_left")
		else:
			animated_sprite.play("walk_right")
	else:
		if direction.y < 0:
			animated_sprite.play("walk_up")  # Note: Y is flipped in Godot
		else:
			animated_sprite.play("walk_down")

func update_ui() -> void:
	if health_bar:
		health_bar.value = current_health

func apply_knockback(force: Vector2):
	knockback_timer = KNOCKBACK_DURATION
	knockback_force = force * 0.5  # Reduce knockback force
	change_state(MobState.STUNNED)

func takeDamage(damage):
	if damage <= 0 or current_state == MobState.DYING:
		return
	
	current_health = max(0, current_health - damage)
	health_changed.emit(current_health, stats.max_health)
	
	# Check for death
	if current_health <= 0:
		change_state(MobState.DYING)
		die()
		return
	
	show_damage_feedback()

func show_damage_feedback() -> void:
	#Visual feedback when taking damage
	if animated_sprite:
		var tween = create_tween()
		tween.tween_property(animated_sprite, "modulate", Color.WHITE * 1.5, 0.1)
		tween.tween_property(animated_sprite, "modulate", Color.WHITE, 0.1)

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

func _on_attack_timer_timout() -> void:
	can_attack = true

func _on_dange_zone_body_entered(body: Node2D) -> void:
	if body.has_method("takePlayerDamage"):
		pass

func _on_hurt_box_body_entered(body: Node2D) -> void:
	if body.has_method("takePlayerDamage"):
		pass
