extends Node3D

@export_range(0.001, 0.05)
var mouse_sensitivity := 0.01

var pitch := 0.0
var yaw := 0.0

var cam_offset = Vector3(0, 0.75, 0)

@export
var zoom := 5.0

@onready var camera := $Camera
@onready var player: Node3D = get_parent()

func _ready() -> void:
	Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
	set_as_top_level(true)

func _process(_delta: float) -> void:
	global_position = player.global_position + cam_offset
	
	if Input.is_mouse_button_pressed(MOUSE_BUTTON_RIGHT):
		Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
	else:
		Input.mouse_mode = Input.MOUSE_MODE_VISIBLE

func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventMouseMotion and Input.is_mouse_button_pressed(MOUSE_BUTTON_RIGHT):
		yaw -= event.relative.x * mouse_sensitivity
		pitch -= event.relative.y * mouse_sensitivity
		pitch = clamp(pitch, deg_to_rad(-80), deg_to_rad(80))

		rotation.y = yaw
		rotation.x = pitch
