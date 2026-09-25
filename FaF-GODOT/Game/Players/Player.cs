using FaF.Debug;
using FaF.Game.Networking;
using FaF.Game.Rig;
using FaF.UserInput;
using Godot;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

namespace FaF.Game.Players;

# nullable enable

/// <summary>
/// A player node, storing all data of a player with authority set to the server. Uses the WorldCharacterSpawner to spawn a new character when it needs to.
/// </summary>
[GlobalClass, Icon("res://Assets/Textures/Character/Icons/PlayerNode.png")]
public partial class Player : Node
{
	private static readonly FaFLogger LOGGER = FaFLogger.Get("Players/Player");

	[Export] public required NPC? Character;
	[Export] public Node3D? CameraPivot;

	public int PEER_ID;

	public string UserID = $"guest-{(int)GD.RandRange(1111, 9999)}";
	public string DisplayName = $"guest-{(int)GD.RandRange(1111, 9999)}";
	
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

	/// <summary>
	/// Loads the appearence of the given Rig.
	/// </summary>
	/// <param name="Rig"></param>
	/// <returns></returns>
	public NPC LoadCharacterApparence(NPC Rig)
	{
		// Avatar customization is not implemented yet: SKIP
		return Rig;
	}

	/// <summary>
	/// Uses the given Rig as the players character.
	/// </summary>
	/// <param name="Rig"></param>
	/// <returns></returns>
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

	/// <summary>
	/// <b>SERVER ONLY</b> - Removes the old character and spawns a new one.
	/// </summary>
	/// <returns></returns>
	public NPC? SpawnNewCharacter()
	{
		if (!Multiplayer.IsServer()) return null;
		Character?.QueueFree();
		return (NPC?)(WorldCharacterSpawner?.Spawn(PEER_ID));
	}

	/// <summary>
	/// <b>SERVER ONLY</b> - Frees the character.
	/// </summary>
	public void FreeCharacter()
	{
		if (!Multiplayer.IsServer()) return;
		Character?.QueueFree();
		Character = null;
	}

	/// <summary>
	/// <b>SERVER ONLY</b> - Handles the player disconnecting (either through leaving or a network error)
	/// </summary>
	public void HandleDisconnect()
	{
		FreeCharacter();
		QueueFree();
	}

	/// <summary>
	/// <b>CLIENT ONLY</b> - Asks the server to spawn a new character. This will likely be removed and currently is only there for debugging purposes.
	/// </summary>
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

		if (InputManager.IsActionJustPressed("debug_reset", InputContext.Gameplay)) RpcId(1, MethodName.RequestSpawnNewCharacter);
    }

    public override void _PhysicsProcess(double delta)
    {
		if (Character == null || !IsLocalPlayer()) return;

		Character.Jump = InputManager.IsActionPressed("movement_jump", InputContext.Gameplay);

		Vector2 inputDir = InputManager.GetVector("movement_left","movement_right","movement_forward","movement_backward", InputContext.Gameplay);

		Vector3 forward = Character.Camera?.Camera3D?.GlobalTransform.Basis.Z ?? Vector3.Forward;
		forward *= new Vector3(1,0,1);
		forward = forward.Normalized();

		Vector3 right = Character.Camera?.Camera3D?.GlobalTransform.Basis.X ?? Vector3.Right;
		right *= new Vector3(1,0,1);
		right = right.Normalized();

		Character.MoveDirection = (right * inputDir.X + forward * inputDir.Y).Normalized();
    }

	#region PLAYER ATTRIBUTES

	[Signal] public delegate void MoneyChangedEventHandler(int newMoney, int oldMoney);

	private int _money = 0;
	[Export] public int Money {
		get => _money;

		set
		{
			EmitSignal(SignalName.MoneyChanged, value, _money);
			_money = value;
		}
	}

	#endregion
}
