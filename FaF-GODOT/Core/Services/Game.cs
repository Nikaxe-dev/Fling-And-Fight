using System;
using System.Collections.Generic;
using FaF.Debug;
using FaF.Services.Content;
using FaF.Services.Lifecycle;
using FaF.Services.Modding;
using FaF.Services.UserInput;
using FaF.Services.World;
using Godot;

namespace FaF.Services;

public partial class Game() : ServiceLoader([], [typeof(Service), typeof(ServiceLoader)], [typeof(Game)])
{
    private static readonly Debug.Logger LOGGER = CoreLoggers.Core;
    private static Game Instance;

    #region State

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

    public static bool IsGameLoaded => Instance.IsLoaded;

    #endregion

    #region Service Links

    public static NetworkService NetworkService => Instance.GetNode<NetworkService>("NetworkService");
    public static InputService InputService => Instance.GetNode<InputService>("InputService");
    public static MouseInputService MouseInputService => Instance.GetNode<MouseInputService>("MouseInputService");
    public static RunService RunService => Instance.GetNode<RunService>("RunService");
    public static WorldService WorldService => Instance.GetNode<WorldService>("WorldService");
    public static PlayerService PlayerService => Instance.GetNode<PlayerService>("PlayerService");
    public static PackService PackService => Instance.GetNode<PackService>("PackService");
    public static ModService ModService => Instance.GetNode<ModService>("ModService");
    public static ContentService ContentService => Instance.GetNode<ContentService>("ContentService");
    public static AssetService AssetService => Instance.GetNode<AssetService>("AssetService");

    #endregion

    public override void _Ready()
    {
        base._Ready();
        Instance = this;

        ParseProgramArgs();

        LoadService();
        StartProgram();
    }

    private void StartProgram()
    {
        if (HasArgument("server"))
        {
            LOGGER.IMPORTANT("Entering program through server");

            string worldID = GetArgument("world");
            string serverPort = GetArgumentOrNull("port") ?? NetworkService.DEFAULT_PORT.ToString();

            RunService.StartServer(worldID, int.Parse(serverPort));
        } else if (ProgramArgs.ContainsKey("join-server"))
        {
            LOGGER.IMPORTANT("Entering program through server join");

            string serverIP = GetArgumentOrNull("ip") ?? "127.0.0.1";
            string serverPort = GetArgumentOrNull("port") ?? NetworkService.DEFAULT_PORT.ToString();

            RunService.StartClient(serverIP, int.Parse(serverPort));
        } else
            RunService.Start();
    }

    private static void ParseProgramArgs()
    {
        foreach (string argument in OS.GetCmdlineArgs())
            if (argument.Contains('='))
            {
                string[] keyValue = argument.Split("=");
                ProgramArgs[keyValue[0].TrimPrefix("--")] = keyValue[1];
            } else
                ProgramArgs[argument.TrimPrefix("--")] = "";
    }

    #region Exceptions

    public class ProgramArgNotGivenException(string arg) : Exception($"Argument '{arg}' not found.");

    #endregion
}