using Godot;
using System;

public partial class Camera : Node3D
{
	private SpringArm3D SpringArm;
	private Camera3D Camera3D;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalRotation = Vector3.Zero;

		if(Input.IsActionJustPressed("zoom_in"))
		{
			
		}
	}
}
