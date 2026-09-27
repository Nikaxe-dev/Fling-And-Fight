using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using FaF.Debug;
using FaF.Enums;
using FaF.Managers;
using Godot;

namespace FaF;

public partial class ProgramEntry : Node
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("ProgramEntry");

    private readonly Dictionary<string, string> ProgramArgs = [];

    public override void _Ready()
    {
        base._Ready();

        LOGGER.LOG(LogType.INFO, "Starting new program instance", "Startup", true);

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

        LOGGER.LOG(LogType.INFO, $"System arguments: {JsonSerializer.Serialize(ProgramArgs)}");

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
            LOGGER.LOG(LogType.INFO, "Starting server", "QuickArgs", true);

            if (ProgramArgs.TryGetValue("world", out string ID))
                GameManager.CreateServer(ID);
            else
                LOGGER.LOG(LogType.ERROR, "Automatic server startup requires a world argument to be specified in the commandline args", "ProgramStartup", true);
        } else if (ProgramArgs.ContainsKey("auto-join"))
        {
            LOGGER.LOG(LogType.INFO, "Auto joining server as client", "ProgramStartup", true);
            GameManager.CreateClient();
        } else
            GameManager.SwitchToTitleScreen();
    }
}
