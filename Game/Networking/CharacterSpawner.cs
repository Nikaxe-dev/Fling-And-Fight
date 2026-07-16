using FlingAndFight.Game.Character;
using Godot;
using System;

namespace FlingAndFight.Game.Networking;

public partial class CharacterSpawner : MultiplayerSpawner
{
	[Export] public required PackedScene CharacterModel;

	public override void _Ready()
	{
		
	}

	private Node spawnFunction(Player player)
	{
		
	}
}
