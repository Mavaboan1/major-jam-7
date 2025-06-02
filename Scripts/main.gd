extends Node2D

@export var slime_time: Timer
@export var time_controll: Timer


const MUSHROOM = preload("res://Scenes/Monsters/mushroom.tscn")
const SLIME = preload("res://Scenes/Monsters/slime.tscn")
const SWARM = preload("res://Scenes/Monsters/swarm.tscn")
const WEIRDOS = preload("res://Scenes/Monsters/weirdos.tscn")

func spawn_mob(mob):
	var slime = SLIME.instantiate()
	%PathFollow2D.progress_ratio = randf()
	slime.global_position = %PathFollow2D.global_position
	add_child(slime)
	
	

func _on_slime_time_timeout() -> void:
	spawn_mob(SLIME)
