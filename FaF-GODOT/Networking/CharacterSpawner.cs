using FaF.Debug;
using FaF.Players;
using FaF.Rig;
using Godot;
using System;
using System.Threading.Tasks;

namespace FaF.Networking;

# nullable enable

public partial class CharacterSpawner : MultiplayerSpawner
{
	[Export] public required PackedScene CharacterModel;
	[Export] public required Node PlayersContainer;

    public override void _EnterTree()
    {
        SpawnFunction = new Callable(this, "CreateNewPlayerCharacter");
    }

	public Node CreateNewPlayerCharacter(int PEER_ID)
	{
		FaFConsole.PrintINFO("CharacterSpawner", $"Creating character {PEER_ID}");
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

			player.UseNPCAsCharacter(player.LoadCharacterApparence(rig));
		}
	}
}
