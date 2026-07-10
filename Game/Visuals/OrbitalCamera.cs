using Godot;
using System;

namespace FlingAndFight.Game.Visuals;

[GlobalClass, Icon("res://Assets/Textures/Decals/Symbols/Symbol Camera.png")]
public partial class OrbitalCamera : Node3D
{
	[Export] public SpringArm3D SpringArm;
	[Export] public Camera3D Camera3D;

	[Export] public bool UserCanZoom = true;
	[Export] public bool UserCanTurn = true;

	[Export] public float Zoom = 5;

	[Export] public float ZOOM_SMOOTHING = 0.15f;

	[Export] public float ZOOM_SENS = 100;
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

	public void HandleZoomInput(double delta)
	{
		if (Input.IsActionJustPressed(INPUT_ZOOM_IN, true))
		{
			Zoom -= (float)delta * ZOOM_SENS;
		}

		if (Input.IsActionJustPressed(INPUT_ZOOM_OUT, true))
		{
			Zoom += (float)delta * ZOOM_SENS;
			GD.Print(Zoom);
		}

		Zoom = Math.Clamp(Zoom, MIN_ZOOM, MAX_ZOOM);
	}

    public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (UserCanTurn) ProcessCameraLookInput(@event);
	}


	public void ProcessCameraLookInput(InputEvent @event)
	{
		bool TurnEnabled = Input.IsMouseButtonPressed(MouseButton.Right) || IsInFirstPerson();

		Input.MouseMode = TurnEnabled ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
		if(@event is InputEventMouseMotion motion && TurnEnabled)
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
		
		if (UserCanZoom) HandleZoomInput(delta);

		SpringArm.SpringLength += (Zoom - SpringArm.SpringLength) * ZOOM_SMOOTHING;
	}

	public bool IsInFirstPerson()
	{
		return Zoom <= 0;
	}
}
