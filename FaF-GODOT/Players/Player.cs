using FaF.Rig;
using FaF.Visuals.Camera;
using Godot;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

namespace FaF.Players;

# nullable enable

[GlobalClass, Icon("res://Assets/Textures/Character/Icons/PlayerNode.png")]
public partial class Player : Node
{
	[Export] public required NPC Character;
	[Export] public required Node3D CameraPivot;

	[Export] public Node3D[] FirstPersonHideNodes = [];

	public int PEER_ID;

	public OrbitalCamera? Camera;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Name is set to the peer id.
		PEER_ID = int.Parse(Name);

		// HumanoidRootPart.SetNetworkOwner()
		Character.SetMultiplayerAuthority(PEER_ID);

		// call environment specific ready functions
		if (Multiplayer.IsServer()) {ReadyServer();} else {ReadyClient();}
	}

	/// <summary>
	/// Returns whether the player is your clients.
	/// </summary>
	public bool IsLocalPlayer()
	{
		GD.Print(Multiplayer.GetUniqueId(), " ", PEER_ID);
		return Multiplayer.GetUniqueId() == PEER_ID;
	}

	private void ReadyClient()
	{
		if (IsLocalPlayer()) {
			Camera = new OrbitalCamera
			{
				Name = "ClientOrbitalCamera",
			};

			Character.AddChild(Camera);
			Camera.GlobalPosition = CameraPivot.GlobalPosition;
			Camera.FOV = 90;
		}
	}

	private void ReadyServer()
	{
		
	}

    public override void _Process(double delta)
    {
        Character.OverrideRotation = Camera != null && Camera.IsInFirstPerson();
		Character.RotationOverride = -Camera?.Camera3D.GlobalTransform.Basis.Z ?? Vector3.Forward;

		// foreach (Node3D item in FirstPersonHideNodes)
		// {
		// 	item.Visible = Camera != null ? !Camera.IsInFirstPerson() : true;
		// }
    }

    public override void _PhysicsProcess(double delta)
    {
		Character.Jump = Input.IsActionPressed("movement_jump");

		Vector2 inputDir = Input.GetVector("movement_left","movement_right","movement_forward","movement_backward");

		Vector3 forward = Camera?.Camera3D.GlobalTransform.Basis.Z ?? Vector3.Forward;
		forward *= new Vector3(1,0,1);
		forward = forward.Normalized();

		Vector3 right = Camera?.Camera3D.GlobalTransform.Basis.X ?? Vector3.Right;
		right *= new Vector3(1,0,1);
		right = right.Normalized();

		Character.MoveDirection = (right * inputDir.X + forward * inputDir.Y).Normalized();
    }
}
