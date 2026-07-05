extends Node3D

const CAMERA_SENS = 0.009
const CAMERA_LIMIT_DEG = 90

const ZOOM_SENS = 100
const MAX_ZOOM = 50
const MIN_ZOOM = 0

@onready var camera_3d: Camera3D = $SpringArm3D/Camera3D
@onready var spring_arm_3d: SpringArm3D = $SpringArm3D

@onready var torso: BoneAttachment3D = $"../Character/ObbyAvatar/Skeleton3D/torso"
@onready var head: BoneAttachment3D = $"../Character/ObbyAvatar/Skeleton3D/head"
@onready var leftleg: BoneAttachment3D = $"../Character/ObbyAvatar/Skeleton3D/leftleg"
@onready var rightleg: BoneAttachment3D = $"../Character/ObbyAvatar/Skeleton3D/rightleg"

func _process(delta: float) -> void:
	global_rotation = Vector3.ZERO
	
	if Input.is_action_just_released("zoom_in"):
		spring_arm_3d.spring_length -= delta * ZOOM_SENS
	if Input.is_action_just_released("zoom_out"):
		spring_arm_3d.spring_length += delta * ZOOM_SENS
	spring_arm_3d.spring_length = clampf(spring_arm_3d.spring_length, MIN_ZOOM, MAX_ZOOM)
	
	var body_parts_visible = spring_arm_3d.spring_length != 0
	torso.visible = body_parts_visible
	head.visible = body_parts_visible
	leftleg.visible = body_parts_visible
	rightleg.visible = body_parts_visible

func _ready() -> void:
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED

func _input(event: InputEvent) -> void:
	if event.is_action_pressed("ui_cancel"): get_tree().quit()
	_camera_control(event)

func _camera_control(event: InputEvent):
	if event is InputEventMouseMotion:
		spring_arm_3d.rotation.y -= event.relative.x * CAMERA_SENS
		spring_arm_3d.rotation.x -= event.relative.y * CAMERA_SENS
		var limit_radians = deg_to_rad(CAMERA_LIMIT_DEG)
		spring_arm_3d.rotation.x = clampf(spring_arm_3d.rotation.x, -limit_radians, limit_radians)
