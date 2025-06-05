extends CharacterBody2D

@onready var player = get_node("/root/Main/Character")
@export var stats: MobStats
@export var audio: AudioStreamPlayer2D
@onready var death_markers: Node2D = $DeathMarkers
@onready var health = stats.max_health

const SMALL_EXP_SHARD = preload("res://Scenes/small_exp_shard.tscn")
const SMALL_WILD_SHARD = preload("res://Scenes/small_wild_shard.tscn")
const HEALING_SHARD = preload("res://Scenes/healing_shard.tscn")

@onready var animated_sprite_2d: AnimatedSprite2D = $AnimatedSprite2D

signal died

var knockback_timer = 0
var knockback_force: Vector2



func _physics_process(delta: float) -> void:
	if knockback_timer > 0:
		velocity = knockback_force
		knockback_timer -= delta
		move_and_slide()
	else:
		var direction = global_position.direction_to(player.global_position)
		velocity = direction * stats.speed
		move_and_slide()
	

	if health <= 0:
		die()
	
	play_movement_animation()

func play_movement_animation() -> void:
	if not animated_sprite_2d:
		return
	
	var direction = velocity.normalized()
	
	if abs(direction.x) > abs(direction.y):
		if direction.x < 0:
			animated_sprite_2d.play("move_left")
		else:
			animated_sprite_2d.play("move_right")
	else:
		if direction.y < 0:
			animated_sprite_2d.play("move_up")  # Note: Y is flipped in Godot
		else:
			animated_sprite_2d.play("move_down")

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
	show_damage_feedback()

func die():
	died.emit()
	shard_drop()
	spawn_ice()
	queue_free()

func show_damage_feedback() -> void:
	#Visual feedback when taking damage
	if animated_sprite_2d:
		var tween = create_tween()
		tween.tween_property(animated_sprite_2d, "modulate", Color.WHITE * 1.5, 0.1)
		tween.tween_property(animated_sprite_2d, "modulate", Color.WHITE, 0.1)
	
	audio.play()

func shard_drop():
	var exp_shard = SMALL_EXP_SHARD.instantiate()
	exp_shard.global_position = global_position
	get_tree().current_scene.add_child(exp_shard)
	
	var wild_shard = SMALL_WILD_SHARD.instantiate()
	wild_shard.global_position = global_position + Vector2.UP * 30
	get_tree().current_scene.add_child(wild_shard)
	
	var heal_shard = HEALING_SHARD.instantiate()
	var drop_heal = randf()
	if drop_heal <= 0.05:
		heal_shard.global_position = global_position + Vector2.UP * 30
		get_tree().current_scene.add_child(heal_shard)
