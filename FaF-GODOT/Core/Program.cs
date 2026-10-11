using System;
using System.Collections.Generic;
using System.Text.Json;
using FaF.Client;
using FaF.Core.Debug;
using FaF.Server;
using Godot;

namespace FaF.Core;

public partial class Program : Node
{
    private static readonly Debug.Logger LOGGER = LoggerFactory.GetLogger(null);

    private static Program Instance;

    #region Nodes

    public static SubViewportContainer GameInstanceContainer {get; private set;}

    #endregion

    #region Launch Arguments

    private static readonly Dictionary<string, string> ProgramArgs = [];

    public static bool HasArgument(string arg)
        => ProgramArgs.ContainsKey(arg);
    
    public static string GetArgument(string arg)
    {
        if (ProgramArgs.TryGetValue(arg, out string result))
            return result;
        else
            throw new ProgramArgNotGivenException(arg);
    }

    public static string GetArgumentOrNull(string arg)
    {
        TryGetArgument(arg, out string result);
        return result;
    }

    public static bool TryGetArgument(string arg, out string value)
        => ProgramArgs.TryGetValue(arg, out value);
    
    public static bool IsDedicatedServer()
        => OS.HasFeature("dedicated_server");

    #endregion

    #region Startup

    public override void _Ready()
    {
        base._Ready();
        Instance = this;

        LOGGER.IMPORTANT("Program started");

        ParseProgramArgs();
        LOGGER.INFO($"Program arguments: {JsonSerializer.Serialize(ProgramArgs)}");

        SetupGameInstanceContainer();
        StartProgram();
    }

    private void SetupGameInstanceContainer()
    {
        GameInstanceContainer = new()
        {
            Stretch = true,

            AnchorRight = 1,
            AnchorBottom = 1,

            OffsetRight = 0,
            OffsetBottom = 0,

            Name = "GameInstanceContainer"
        };

        AddChild(GameInstanceContainer, true);
    }

    private void StartProgram()
    {
        string serverPort = GetArgumentOrNull("port") ?? "56565";

        // dedicated server - no client export
        if (IsDedicatedServer())
        {
            LOGGER.IMPORTANT("Launching as dedicated server");

            string worldID = GetArgument("world");

            ServerInstance server = new(int.Parse(serverPort), worldID) { Name = "Server" };
            GameInstanceContainer.AddChild(server);

            server.Start();
            return;
        }
        
        // create server with this argument
        if (HasArgument("server"))
        {
            LOGGER.IMPORTANT("Launching server");

            string worldID = GetArgument("world");

            ServerInstance server = new(int.Parse(serverPort), worldID) { Name = "Server" };
            GameInstanceContainer.AddChild(server);

            server.Start();
        }

        // create amount of auto connecting clients
        if (TryGetArgument("clients", out string clientsArg))
        {
            string serverIP = GetArgumentOrNull("ip") ?? "127.0.0.1";

            for (int i = 0; i < int.Parse(clientsArg); i++)
            {
                LOGGER.IMPORTANT($"Launching client #{i+1}");

                ClientInstance client = new() { Name = $"Client{i+1}" };
                GameInstanceContainer.AddChild(client);

                client.Start();
                client.ConnectTo(serverIP, serverPort);
            }
        }

        // default program behaviour: start client
        if (!HasArgument("clients") && !HasArgument("server"))
        {
            LOGGER.IMPORTANT("Launching as client");

            ClientInstance client = new() { Name = "Client" };
            GameInstanceContainer.AddChild(client);

            client.Start();
        }
    }

    private void ParseProgramArgs()
    {
        foreach (string argument in OS.GetCmdlineArgs())
            if (argument.Contains('='))
            {
                string[] keyValue = argument.Split("=");
                ProgramArgs[keyValue[0].TrimPrefix("--")] = keyValue[1];
            } else
                ProgramArgs[argument.TrimPrefix("--")] = "";
    }

    #endregion

    #region Exceptions

    public class ProgramArgNotGivenException(string arg) : Exception($"Argument '{arg}' not found.");

    #endregion
}