using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using FaF.Debug;
using FaF.Services;
using Godot;

namespace FaF;

public partial class ProgramEntry : Node
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("ProgramEntry");

    private readonly Dictionary<string, string> ProgramArgs = [];

    public override void _Ready()
    {
        base._Ready();

        LOGGER.LOG(Enums.LogType.INFO, "Starting new program instance", "Startup", true);

        // parse arguments into dictionary
		foreach (var argument in OS.GetCmdlineArgs())
		{
			if (argument.Contains('='))
			{
				string[] keyValue = argument.Split("=");
				ProgramArgs[keyValue[0].TrimPrefix("--")] = keyValue[1];
			}
			else
			{
				ProgramArgs[argument.TrimPrefix("--")] = "";
			}
		}

        LOGGER.LOG(Enums.LogType.INFO, $"System arguments: {JsonSerializer.Serialize(ProgramArgs)}");

        if (ProgramArgs.TryGetValue("delay", out string rawDelay))
        {
            Thread.Sleep(int.Parse(rawDelay));
        }

        StartProgram();
    }

    private void StartProgram()
    {
        if (ProgramArgs.ContainsKey("server"))
        {
            LOGGER.LOG(Enums.LogType.INFO, "Entering program through server", "QuickArgs", true);

            if (ProgramArgs.TryGetValue("world", out string worldID))
            {
                bool serverPortArgGiven = ProgramArgs.TryGetValue("port", out string serverPortString);
                Game.RunService.StartServer(worldID, serverPortArgGiven ? int.Parse(serverPortString) : Game.NetworkService.DEFAULT_PORT);
            }
            else
                LOGGER.LOG(Enums.LogType.ERROR, "Automatic server startup requires a world argument to be specified in the commandline args", "ProgramStartup", true);
        } else if (ProgramArgs.ContainsKey("auto-join"))
        {
            LOGGER.LOG(Enums.LogType.INFO, "Entering program through client", "ProgramStartup", true);

            bool serverIpArgGiven = ProgramArgs.TryGetValue("ip", out string serverIP);
            bool serverPortArgGiven = ProgramArgs.TryGetValue("port", out string serverPortString);

            Game.RunService.StartClient(serverIpArgGiven ? serverIP : "127.0.0.1", serverPortArgGiven ? int.Parse(serverPortString) : Game.NetworkService.DEFAULT_PORT);
        } else
            Game.RunService.OpenTitleScreen();
    }
}
