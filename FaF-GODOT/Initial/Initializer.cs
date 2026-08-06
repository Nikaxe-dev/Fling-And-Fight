using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using FaF.Data;
using FaF.Data.RegistryObjects;
using FaF.Debug;
using FaF.Game.Networking;
using Godot;

namespace FlingAndFight.Launcher;

/// <summary>
/// The starting point of the entire program, handling the intialization of the server & client if a startup argument says so. Otherwise loads the main menu for the client.
/// </summary>
public partial class Initializer : Node
{
	private static readonly FaFLogger LOGGER = FaFLogger.Get("Initial");

	private readonly Dictionary<string, string> arguments = [];

	public override void _Ready()
	{
		base._Ready();
		
		// SWITCH to title screen unless --server is enabled, otherwise start a new server with the provided settings (given file OR specified in args OR defaults).
		GD.Print("!!----------------------------------------!!");
		LOGGER.LOG(LogType.INFO, "Starting FaF", "Startup", true);

		foreach (var argument in OS.GetCmdlineArgs())
		{
			if (argument.Contains('='))
			{
				string[] keyValue = argument.Split("=");
				arguments[keyValue[0].TrimPrefix("--")] = keyValue[1];
			}
			else
			{
				// Options without an argument will be present in the dictionary,
				// with the value set to an empty string.
				arguments[argument.TrimPrefix("--")] = "";
			}
		}

		GD.Print("System arguments:");
		GD.Print(JsonSerializer.Serialize(arguments));

		ContentLoader.LoadWorldRegistryFolder();

		CallDeferred(nameof(QuickPlayArgs));
	}

	private void QuickPlayArgs()
	{
		if (arguments.ContainsKey("server"))
		{
			LOGGER.LOG(LogType.INFO, "Starting server", "QuickArgs", true);
			
			if (arguments.TryGetValue("world", out string ID)) {
				WorldRegistry worldRegistry = ContentLoader.GetWorld(ID);
				if (worldRegistry != null)
				{
					GetTree().ChangeSceneToPacked(worldRegistry.Scene);
					NetworkManager.Instance.StartServer();
				} else
				{
					LOGGER.LOG(LogType.ERROR, $"World specified in commandline args not found: {ID}.", "QuickArgs", true);
				}
			} else
			{
				LOGGER.LOG(LogType.ERROR, "Automatic server startup requires a world argument to be specified in the commandline args.", "QuickArgs", true);
			}
		} else if (arguments.ContainsKey("auto-join"))
		{
			LOGGER.LOG(LogType.INFO, "Auto joining server as client", "QuickArgs", true);

			GetTree().ChangeSceneToFile("res://Data/Worlds/New_Sedes/New_Sedes_Scene.tscn");
			NetworkManager.Instance.JoinServer(NetworkManager.DEFAULT_IP, NetworkManager.DEFAULT_PORT);
		} else if (arguments.ContainsKey("editor"))
		{
			LOGGER.LOG(LogType.INFO, "Launching FaF Editor", "QuickArgs", true);

			GetTree().ChangeSceneToFile("res://Editor/WorldEditorScene.tscn");
		}
	}
}
