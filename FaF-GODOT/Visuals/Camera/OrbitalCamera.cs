using FaF.UserInput;
using Godot;
using System;

namespace FaF.Visuals.Camera;

/// <summary>
/// A camera that allows you to pivot around an object and zoom in or out. Also has a first person mode which the player controller hides certain body parts in.
/// </summary>
[GlobalClass, Icon("res://Assets/Textures/Decals/Symbols/Symbol Camera.png")]
public partial class OrbitalCamera : Node3D
{
	[Export] public SpringArm3D SpringArm;
	[Export] public Camera3D Camera3D;

	[Export] public bool UserCanZoom = true;
	[Export] public bool UserCanTurn = true;

	[Export] public float Zoom = 5;

	[Export] public float ZOOM_SMOOTHING = 0.15f;

	[Export] public float ZOOM_SENS = 1;
	[Export] public float MAX_ZOOM = 50;
	[Export] public float MIN_ZOOM = 0;

	[Export] public float CAMERA_LIMIT_DEG = 80;

	[Export] public float CAMERA_TURN_SENS = 0.009f;

	[Export] public float FOV
	{
		set => Camera3D.Fov = value;
		get => Camera3D.Fov;
	}

	public static readonly StringName INPUT_ZOOM_IN = new("zoom_in");
	public static readonly StringName INPUT_ZOOM_OUT = new("zoom_out");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SpringArm = new SpringArm3D();
		Camera3D = new Camera3D();

		SpringArm.SpringLength = Zoom;

		SpringArm.AddChild(Camera3D);
		AddChild(SpringArm);
	}

    public override void _UnhandledInput(InputEvent @event)
	{
		base._Input(@event);

		if (UserCanTurn) ProcessCameraLookInput(@event);

		if (UserCanZoom)
		{
			if (@event.IsActionPressed("zoom_in"))
			{
				Zoom -= ZOOM_SENS;
			}

			if (@event.IsActionPressed("zoom_out"))
			{
				Zoom += ZOOM_SENS;
			}

			Zoom = Math.Clamp(Zoom, MIN_ZOOM, MAX_ZOOM);
		}
	}


	public void ProcessCameraLookInput(InputEvent @event)
	{
		bool TurnEnabled = Input.IsMouseButtonPressed(MouseButton.Right) || IsInFirstPerson();
		MouseInputManager.Instance.TargetMouseMode = TurnEnabled ? MouseMode.LockedCenter : MouseMode.Free;

		if(@event is InputEventMouseMotion motion && !MouseInputManager.Instance.IsMouseFree())
		{
			SpringArm.Rotation -= Vector3.Up * motion.Relative.X * CAMERA_TURN_SENS;
			SpringArm.Rotation -= Vector3.Right * motion.Relative.Y * CAMERA_TURN_SENS;
			
			float limitRadians = float.DegreesToRadians(CAMERA_LIMIT_DEG);
			SpringArm.Rotation = new Vector3(Math.Clamp(SpringArm.Rotation.X, -limitRadians, limitRadians), SpringArm.Rotation.Y, SpringArm.Rotation.Z);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalRotation = Vector3.Zero;

		SpringArm.SpringLength += (Zoom - SpringArm.SpringLength) * ZOOM_SMOOTHING;
	}

	public bool IsInFirstPerson()
	{
		return Zoom <= 0;
	}
}
