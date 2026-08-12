using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using FaF.Data;
using FaF.Data.RegistryObjects;
using FaF.Debug;
using FaF.Game;
using FaF.Game.Networking;
using Godot;

namespace FaF.Initial;

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

		LOGGER.LOG(LogType.INFO, "Starting new FaF instance.", "Startup", true);

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

		LOGGER.LOG(LogType.INFO, $"System arguments: {JsonSerializer.Serialize(arguments)}");

		if (arguments.TryGetValue("delay", out string rawDelay))
		{
			Thread.Sleep(int.Parse(rawDelay));
		}

		CallDeferred(nameof(StartGame));
	}

	private void StartGame()
	{
		if (arguments.ContainsKey("server"))
		{
			LOGGER.LOG(LogType.INFO, "Starting server", "QuickArgs", true);
			
			if (arguments.TryGetValue("world", out string ID)) {
				GameManager.Instance.CreateServer(ID);
			} else
			{
				LOGGER.LOG(LogType.ERROR, "Automatic server startup requires a world argument to be specified in the commandline args.", "QuickArgs", true);
			}
		} else if (arguments.ContainsKey("auto-join"))
		{
			LOGGER.LOG(LogType.INFO, "Auto joining server as client", "QuickArgs", true);
			
			GameManager.Instance.CreateClient();
		} else if (arguments.ContainsKey("faf-editor"))
		{
			LOGGER.LOG(LogType.INFO, "Launching FaF Editor", "QuickArgs", true);

			GetTree().ChangeSceneToFile("res://Editor/WorldEditorScene.tscn");
		} else
		{
			GameManager.Instance.SwitchToTitleScreen();
		}
	}
}
