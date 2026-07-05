extends CharacterBody3D

const SPEED := 5.0
const JUMP_VELOCITY := 4.5
const TURN_SPEED := 10.0

@onready var camera_spring_arm: SpringArm3D = $CameraPivot/SpringArm3D
@onready var character: Node3D = $Character
@onready var animationPlayer: AnimationPlayer = $Character/AnimationPlayer


func play_anim(anim: String, speed: float = 1.0, blend: float = 0.2) -> void:
	if animationPlayer.current_animation != anim:
		animationPlayer.play(anim, blend)
	animationPlayer.speed_scale = speed

func _process(delta: float) -> void:
	if camera_spring_arm.spring_length == 0:
		var forward := -camera_spring_arm.global_transform.basis.z
		var target_rotation := atan2(forward.x, forward.z)
		rotation.y = lerp_angle(rotation.y, target_rotation, TURN_SPEED * delta)

func _physics_process(delta: float) -> void:
	# Gravity
	if not is_on_floor():
		velocity += get_gravity() * delta

	# Jump
	if Input.is_action_pressed("movement_jump") and is_on_floor():
		velocity.y = JUMP_VELOCITY

	# Input
	var input_dir := Input.get_vector("movement_left","movement_right","movement_forward","movement_backward")

	# Movement
	var forward := camera_spring_arm.global_transform.basis.z
	forward.y = 0
	forward = forward.normalized()

	var right := camera_spring_arm.global_transform.basis.x
	right.y = 0
	right = right.normalized()

	var direction := (right * input_dir.x + forward * input_dir.y).normalized()

	# Movement
	if direction != Vector3.ZERO:
		velocity.x = direction.x * SPEED
		velocity.z = direction.z * SPEED
	else:
		velocity.x = move_toward(velocity.x, 0.0, SPEED)
		velocity.z = move_toward(velocity.z, 0.0, SPEED)
	
	if camera_spring_arm.spring_length != 0 and direction != Vector3.ZERO:
		var target_rotation := atan2(direction.x, direction.z)
		rotation.y = lerp_angle(rotation.y, target_rotation, TURN_SPEED * delta)
		
	move_and_slide()

	# Animations
	if not is_on_floor():
		play_anim("fall", 1.0, 0.2)
	elif direction != Vector3.ZERO:
		play_anim("walk", 1.0, 0.15)
	else:
		play_anim("idle", 0.5, 0.15)
