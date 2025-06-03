extends Node2D

@export var slime_time: Timer
@export var time_controll: Timer

var time_count := 1
@onready var path_1 = %PathFollow2D
@onready var path_2 = %PathFollow2D2

var path_array = [path_1, path_2]

const MUSHROOM = preload("res://Scenes/Monsters/mushroom.tscn")
const SLIME = preload("res://Scenes/Monsters/slime.tscn")
const WEIRDOS = preload("res://Scenes/Monsters/weirdos.tscn")
const FIRE_DUDE = preload("res://Scenes/Monsters/fire_dude.tscn")
const SWARM_WAVE_1 = preload("res://Scenes/Waves/swarm_wave_1.tscn")
const SWARM_WAVE_2 = preload("res://Scenes/Waves/swarm_wave_2.tscn")

var swarms = [SWARM_WAVE_1, SWARM_WAVE_2]

func spawn_mob(mob):
	var temp_mob = mob.instantiate()
	%PathFollow2D.progress_ratio = randf()
	temp_mob.global_position = %PathFollow2D.global_position
	add_child(temp_mob)

func _physics_process(delta: float) -> void:
	if time_count < 10 and time_count > 7:
		spawn_mob(FIRE_DUDE)
		spawn_mob(FIRE_DUDE)
	if time_count > 20 and time_count < 25:
		spawn_mob(swarms[randi_range(0, 1)])
		await get_tree().create_timer(3.0).timeout
		spawn_mob(swarms[randi_range(0, 1)])
	if time_count > 35 and time_count < 40:
		slime_time.wait_time = 0.7
		spawn_mob(MUSHROOM)
		spawn_mob(MUSHROOM)
		spawn_mob(MUSHROOM)
 
	

func _on_slime_time_timeout() -> void:
	spawn_mob(SLIME)


func _on_timer_timeout() -> void:
	time_count += 1
