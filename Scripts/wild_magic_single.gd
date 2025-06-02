extends Node


var fire_dude_stats = preload("res://Resource/fire_dude_stats.tres")
var shroom_stats = preload("res://Resource/Shroom.tres")
var swarm_stats = preload("res://Resource/swarm.tres")
var weirdos_stats = preload("res://Resource/weirdos.tres")
const SMALL_EXP_SHARD = preload("res://Scenes/small_exp_shard.tscn")
const BIG_EXP_SHARD = preload("res://Scenes/big_exp_shard.tscn")

var player: CharacterBody2D

func _ready() -> void:
	player = get_node("/root/Main/Character")

func monster_speed():
	var fire_temp = fire_dude_stats.speed
	var shroom_temp = shroom_stats.speed
	var swarm_temp = shroom_stats.speed
	var weirdos_temp = weirdos_stats.speed
	
	fire_dude_stats.speed *= 2
	shroom_stats.speed *= 2
	swarm_stats.speed *= 2
	weirdos_stats.speed *= 2
	
	await get_tree().create_timer(15.0).timeout
	fire_dude_stats.speed = fire_temp
	shroom_stats.speed = shroom_temp
	swarm_stats.speed = swarm_temp
	weirdos_stats.speed = weirdos_temp

func monster_size():
	for i in get_node("/root/Main/Enemy").get_children():
		i.scale *= 2
	await get_tree().create_timer(15.0).timeout
	for i in get_node("/root/Main/Enemy").get_children():
		i.scale /= 2
		
func exp_explosion():
	for i in range(randi_range(5, 15)):
		var exp_shard = SMALL_EXP_SHARD.instantiate()
		exp_shard.global_position.x = player.global_position.x + randi_range(-60,60)
		exp_shard.global_position.y = player.global_position.y + randi_range(-60,60)
		get_tree().current_scene.add_child(exp_shard)
	

func hallucinations():
	pass
