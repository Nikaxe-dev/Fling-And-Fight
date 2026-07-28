using System;
using FaF.Debug;
using FaF.Networking;
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
	private static readonly FaFLogger LOGGER = FaFLogger.Get("Players/Player");

	[Export] public required NPC? Character;
	[Export] public Node3D? CameraPivot;

	public int PEER_ID;
	
	private CharacterSpawner? WorldCharacterSpawner;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Name is set to the peer id.
		PEER_ID = int.Parse(Name);
	}

    public override void _EnterTree()
    {
		PEER_ID = int.Parse(Name);

        WorldCharacterSpawner = GetTree().CurrentScene.GetNodeOrNull<CharacterSpawner>("Networking/CharacterSpawner");

		if (WorldCharacterSpawner == null) LOGGER.LOG(LogType.ERROR, "Expected CharacterSpawner Scene/Networking/CharacterSpawner to exist but got nothing", "CharacterSystems");

		SpawnNewCharacter();
    }

	public NPC LoadCharacterApparence(NPC Rig)
	{
		// Avatar customization is not implemented yet: SKIP
		return Rig;
	}

	public NPC UseNPCAsCharacter(NPC Rig)
	{
		Character = Rig;
		Character.player = this;

		LOGGER.LOG(LogType.INFO, "Player using new character", "CharacterSystems");
		
		if (IsLocalPlayer())
		{
			LOGGER.LOG(LogType.INFO, "Local player using new character", "CharacterSystems");
			Character.CreateOrbitalCamera();
		}

		return Rig;
	}

	public NPC? SpawnNewCharacter()
	{
		if (!Multiplayer.IsServer()) return null;
		Character?.QueueFree();
		return (NPC?)(WorldCharacterSpawner?.Spawn(PEER_ID));
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void RequestSpawnNewCharacter()
	{
		if (Multiplayer.GetRemoteSenderId() == PEER_ID) SpawnNewCharacter();
	}

	/// <summary>
	/// Returns whether the player is your clients.
	/// </summary>
	public bool IsLocalPlayer()
	{
		return Multiplayer.GetUniqueId() == PEER_ID;
	}

    public override void _Process(double delta)
    {
		if (Character == null || !IsLocalPlayer()) return;

        Character.OverrideRotation = Character.Camera != null && Character.Camera.IsInFirstPerson();
		Character.RotationOverride = -Character.Camera?.Camera3D?.GlobalTransform.Basis.Z ?? Vector3.Forward;

		foreach (Node3D item in Character.FirstPersonHideNodes)
		{
			item.Visible = Character.Camera == null || !Character.Camera.IsInFirstPerson();
		}

		if (Input.IsActionJustPressed("debug_reset")) RpcId(1, MethodName.RequestSpawnNewCharacter);
    }

    public override void _PhysicsProcess(double delta)
    {
		if (Character == null || !IsLocalPlayer()) return;

		Character.Jump = Input.IsActionPressed("movement_jump");

		Vector2 inputDir = Input.GetVector("movement_left","movement_right","movement_forward","movement_backward");

		Vector3 forward = Character.Camera?.Camera3D?.GlobalTransform.Basis.Z ?? Vector3.Forward;
		forward *= new Vector3(1,0,1);
		forward = forward.Normalized();

		Vector3 right = Character.Camera?.Camera3D?.GlobalTransform.Basis.X ?? Vector3.Right;
		right *= new Vector3(1,0,1);
		right = right.Normalized();

		Character.MoveDirection = (right * inputDir.X + forward * inputDir.Y).Normalized();
    }
}
