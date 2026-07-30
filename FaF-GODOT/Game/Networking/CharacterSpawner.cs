using FaF.Debug;
using FaF.Game.Players;
using FaF.Game.Rig;
using FaF.Visuals.Camera;
using Godot;
using System;
using System.Threading.Tasks;

namespace FaF.Game.Networking;

# nullable enable

public partial class CharacterSpawner : MultiplayerSpawner
{
	private static readonly FaFLogger LOGGER = FaFLogger.Get("Networking/Spawners/CharacterSpawner");

	[Export] public required PackedScene CharacterModel;
	[Export] public required Node PlayersContainer;

    public override void _EnterTree()
    {
        SpawnFunction = new Callable(this, "CreateNewPlayerCharacter");
    }

	public Node CreateNewPlayerCharacter(int PEER_ID)
	{
		LOGGER.LOG(LogType.INFO, $"Creating character {PEER_ID}");
		var instance = CharacterModel.Instantiate();
		instance.Name = PEER_ID.ToString();

		instance.SetMultiplayerAuthority(PEER_ID);

		// Await Player to avoid situations where the Player hasnt been spawned in yet and replicated to the client
        _ = FinishCharacterInitialization(instance, PEER_ID);

		return instance;
	}

	private async Task FinishCharacterInitialization(Node instance, int PEER_ID)
	{	
		if (instance is NPC rig)
		{
			Player? player;

			while ((player = PlayersContainer.GetNodeOrNull<Player>(PEER_ID.ToString())) == null)
				await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

			// GD.Print(player, " ", Multiplayer.IsServer(), " ", rig.IsMultiplayerAuthority());
			// if (player != null && !Multiplayer.IsServer() && rig.IsMultiplayerAuthority())
			// {
			// 	player.CameraPivot = rig.CameraPivot;

			// 	player.Camera = new OrbitalCamera
			// 	{
			// 		Name = "ClientOrbitalCamera",
			// 	};

			// 	rig.AddChild(player.Camera);
			// 	player.Camera.GlobalPosition = rig.CameraPivot?.GlobalPosition ?? rig.GlobalPosition;
			// 	player.Camera.FOV = 90;
			// }

			player?.UseNPCAsCharacter(player.LoadCharacterApparence(rig));
		}
	}
}
