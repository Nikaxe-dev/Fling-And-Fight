using FaF.Rig;
using Godot;
using System;

namespace FaF.Networking;

public partial class CharacterSpawner : MultiplayerSpawner
{
	[Export] public required PackedScene CharacterModel;

	public override void _Ready()
	{
		
	}

	// private Node spawnFunction(Player player)
	// {
		
	// }
}
