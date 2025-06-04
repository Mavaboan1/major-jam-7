extends Node


const MUSHROOM = preload("res://Scenes/Monsters/mushroom.tscn")
const SLIME = preload("res://Scenes/Monsters/slime.tscn")
const WEIRDOS = preload("res://Scenes/Monsters/weirdos.tscn")
const FIRE_DUDE = preload("res://Scenes/Monsters/fire_dude.tscn")
const SWARM_WAVE_1 = preload("res://Scenes/Waves/swarm_wave_1.tscn")
const SWARM_WAVE_2 = preload("res://Scenes/Waves/swarm_wave_2.tscn")

class WaveConfig:
	var start_time: float
	var end_time: float
	var enemy_types: Array[PackedScene]
	var spawn_interval: float
	var max_concurrent: int
	var spawn_weight: Array[float]
	
	func _init(s_time: float, e_time: float, enemies: Array[PackedScene], interval: float = 2.0, max_conc: int = 5, weights: Array[float] = []):
		start_time = s_time
		end_time = e_time
		enemy_types = enemies
		spawn_interval = interval
		max_concurrent = max_conc
		spawn_weight = weights if weights.size() > 0 else _create_equal_weights(enemies.size())
		
	func _create_equal_weights(count: int) -> Array[float]:
		var weights: Array[float] = []
		for i in count:
			weights.append(1.0)
		return weights

var game_time: float = 0.0
var is_active: bool = false
var current_enemies: int = 0
var last_spawn_time: float = 0.0
var current_wave_index: int = 0

var waves: Array[WaveConfig] = []

@export var spawn_radius: float = 500.0
@export var player_path: NodePath = "../Character"
@export var enemy_path: NodePath = "../Enemy"

var player_node: CharacterBody2D
var enemy_parent: Node2D

# Signals
signal wave_started(wave_index: int)
signal wave_ended(wave_index: int)
#signal _enemy_spawned(enemy_type: String)

func _ready() -> void:
	_setup_waves()
	_connect_nodes()
	start_wave_system()
	print_wave_info()

func _setup_waves():
	waves.append(WaveConfig.new(
		0.0, 20.0,
		[SLIME],
		1.0, 10,
		[1.0]
	))
	
	waves.append(WaveConfig.new(
		20.0, 30.0,
		[SLIME, SWARM_WAVE_1, SWARM_WAVE_2],
		1, 10,
		[7.0, 3.0, 3.0]
	))
	
	waves.append(WaveConfig.new(
		30.0, 50.0,
		[SLIME, FIRE_DUDE],
		1.0, 10,
		[0.7, 0.3]
	))
	
	waves.append(WaveConfig.new(
		50.0, 70.0,
		[SLIME],
		0.5, 30,
		[1.0]
	))
	
	waves.append(WaveConfig.new(
		70.0, 100.0,
		[SLIME, FIRE_DUDE, MUSHROOM],
		2.5, 40,
		[0.5, 0.3, 0.2]
	))
	
	waves.append(WaveConfig.new(
		100.0, 120.0,
		[FIRE_DUDE, SWARM_WAVE_1, SWARM_WAVE_2],
		1, 6,
		[0.5, 0.3, 0.2]
	))
	
	waves.append(WaveConfig.new(
		120.0, 160.0,
		[SLIME, FIRE_DUDE, SWARM_WAVE_1, SWARM_WAVE_2, MUSHROOM],
		2.5, 30,
		[0.1, 0.3, 0.1, 0.2, 0.3]
	))
	
	waves.append(WaveConfig.new(
		160.0, 200.0,
		[SLIME],
		0.2, 100,
		[1.0]
	))
	
	waves.append(WaveConfig.new(
		200.0, INF,
		[SLIME, FIRE_DUDE, SWARM_WAVE_1, SWARM_WAVE_2, MUSHROOM],
		0.3, 8,
		[0.1 ,0.3, 0.2, 0.2, 0.2]
	))
	
func _connect_nodes():
	player_node = get_node_or_null(player_path)
	enemy_parent = get_node_or_null(enemy_path)
	if not player_node:
		print("Warning: Player node not found at path: ", player_path)
	if not enemy_parent:
		print("Warning: Enemy parent node not found at path: ", enemy_path)

func _physics_process(delta: float) -> void:
	if not is_active:
		return
	
	game_time += delta
	_update_current_wave()
	_handle_spawning()
	#print_wave_info()
	
func start_wave_system():
	is_active = true
	game_time = 0.0
	current_wave_index = 0

func stop_wave_system():
	is_active = false
	
func _update_current_wave():
	var new_wave_index = _get_current_wave_index()
	
	if new_wave_index != current_wave_index and new_wave_index < waves.size():
		if current_wave_index >= 0:
			wave_ended.emit(current_wave_index)
		
		current_wave_index = new_wave_index
		wave_started.emit(current_wave_index)
		
func _get_current_wave_index() -> int:
	for i in range(waves.size()):
		var wave = waves[i]
		if game_time >= wave.start_time and game_time < wave.end_time:
			return i
	return waves.size() - 1

func _handle_spawning():
	if current_wave_index >= waves.size():
		return
	
	var current_wave = waves[current_wave_index]
	
	if current_wave_index == waves.size() - 1 and game_time > 200.0:
		var time_multiplier = 1.0 + (game_time - 200) / 60.0 # Harder every minute
		current_wave.spawn_interval = max(0.5, 1.2 / time_multiplier) # Faster Spawning
		current_wave.max_concurrent = min(20, int(8 * time_multiplier)) # More Enemies
	
	var time_since_last_spawn = game_time - last_spawn_time
	
	if time_since_last_spawn >= current_wave.spawn_interval and current_enemies < current_wave.max_concurrent:
		_spawn_enemy(current_wave)
		last_spawn_time = game_time

func _spawn_enemy(wave: WaveConfig):
	if not player_node or not enemy_parent:
		return
	
	var selected_enemy = _select_weighted_enemy(wave.enemy_types, wave.spawn_weight)
	if not selected_enemy:
		return
	
	var spawn_pos = _get_spawn_position()
	
	var enemy_instance = selected_enemy.instantiate()
	enemy_parent.add_child(enemy_instance)
	
	enemy_instance.global_position = spawn_pos
	
	if enemy_instance.has_signal("died"):
		var signal_name = "died" if enemy_instance.has_signal("died") else "enemy_died"
		enemy_instance.connect(signal_name, _on_enemy_died)
	
	current_enemies += 1

func _select_weighted_enemy(enemy_types: Array[PackedScene], weights: Array[float]) -> PackedScene:
	if enemy_types.is_empty():
		return null
	
	var total_weight = 0.0
	for weight in weights:
		total_weight += weight
	
	var random_value = randf() * total_weight
	var current_weight = 0.0
	
	for i in range(enemy_types.size()):
		current_weight += weights[i] if i < weights.size() else 1.0
		if random_value <= current_weight:
			return enemy_types[i]
	
	return enemy_types[0]
		
func _get_spawn_position() -> Vector2:
	if not player_node:
		return Vector2.ZERO
	
	var player_pos = player_node.global_position
	var angle = randf() * 2 * PI
	var  distance = spawn_radius + randf() * 100
	
	return player_pos + Vector2(cos(angle), sin(angle)) * distance

func _on_enemy_died():
	current_enemies = max(0, current_enemies - 1)


func print_wave_info():
	print("=== Wave System Status ===")
	print("Game Time: ", game_time)
	print("Current Wave: ", current_wave_index + 1, "/", waves.size())
	print("Enemies Alive: ", current_enemies)
	if current_wave_index < waves.size():
		var wave = waves[current_wave_index]
		print("Wave ends at: ", wave.end_time)
		print("Max concurrent: ", wave.max_concurrent)
	
	
	
	
	
	
