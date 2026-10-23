using System;
using System.Collections.Generic;
using System.Text.Json;
using FaF.Core.Lib.Debug;
using FaF.Core.Services.ProgramServices;
using FaF.Core.Services;
using Godot;
using System.Linq;

namespace FaF.Core;

public partial class Program : Node, INodeLoader<IProgramService>
{
    private static readonly Lib.Debug.Logger LOGGER = LoggerFactory.GetLogger(null);

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

    #endregion

    #region Server

    public static bool IsDedicatedServer()
        => OS.HasFeature("dedicated_server");

    public static bool IsServerAvailable()
        => TryGetServerInstanceType(out Type _);
    
    public static bool TryGetServerInstanceType(out Type instanceType)
    {
        instanceType = typeof(Program).Assembly.GetTypes().First(t => !t.IsAbstract && t.IsAssignableTo(typeof(IServerInstance)));
        return instanceType != null;
    }

    public static Type GetServerInstanceType()
    {
        if (TryGetServerInstanceType(out Type instanceType))
            return instanceType;
        else
            throw new ServerIsNotAvailableException();
    }

    public static IServerInstance NewServer(int port, string worldID)
    {
        IServerInstance server = (IServerInstance)Activator.CreateInstance(GetServerInstanceType(), [port, worldID]);

        if (server is GameInstance game)
        {
            GameInstanceContainer.AddChild(game);
            game.Name = "Server";
            game.Start();
        }

        return server;
    }

    #endregion

    #region Client

    public static bool IsClientAvailable()
        => TryGetClientInstanceType(out Type _);

    public static bool TryGetClientInstanceType(out Type instanceType)
    {
        instanceType = typeof(Program).Assembly.GetTypes().First(t => !t.IsAbstract && t.IsAssignableTo(typeof(IClientInstance)));
        return instanceType != null;
    }

    public static Type GetClientInstanceType()
    {
        if (TryGetClientInstanceType(out Type instanceType))
            return instanceType;
        else
            throw new ClientIsNotAvailableException();
    }

    public static IClientInstance NewClient()
    {
        IClientInstance client = (IClientInstance)Activator.CreateInstance(GetClientInstanceType());

        if (client is GameInstance game)
        {
            GameInstanceContainer.AddChild(game);
            game.Start();
        }

        return client;
    }

    public static IClientInstance NewAutoConnectClient(string serverIP, int serverPort)
    {
        IClientInstance client = NewClient();
        client.ConnectTo(serverIP, serverPort);
        return client;
    }

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

        ((INodeLoader<IProgramService>)this).LoadDerived();

        foreach (Node child in GetChildren())
            if (child is IProgramService && child is Service service)
                service.LoadService();

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
        /*
            This requires IClientInstance & IServerInstance to allow the client & server namespaces to not be defined in the project.
            Why is it handled like this? Because Godot doesn't easily support adding preprocessor tags for each different export OR for each added feature tag.
            If it was a feature, I would simply use a DEDICATED_SERVER preprocessor tag.
        */

        int serverPort = int.Parse(GetArgumentOrNull("port") ?? "56565");

        if (IsServerAvailable())
        {
            string worldID = GetArgument("world");

            if (IsDedicatedServer())
            {
                LOGGER.IMPORTANT("Launching as dedicated server");
                NewServer(serverPort, worldID);
                return;
            }

            if (HasArgument("server"))
            {
                LOGGER.IMPORTANT("Laucnhing server");
                NewServer(serverPort, worldID);
            }
        }

        if (IsClientAvailable())
        {
            string serverIP = GetArgumentOrNull("ip") ?? "127.0.0.1";

            if (TryGetArgument("clients", out string clientsArg))
                for (int i = 0; i < int.Parse(clientsArg); i++)
                {
                    LOGGER.IMPORTANT($"Launching client #{i+1}");
                    NewAutoConnectClient(serverIP, serverPort);
                }
            
            if (!HasArgument("clients") && !HasArgument("server"))
            {
                LOGGER.IMPORTANT("Launching as client");
                if (HasArgument("ip") || HasArgument("port"))
                    NewAutoConnectClient(serverIP, serverPort);
                else
                    NewClient();
            }
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

    public class ClientIsNotAvailableException() : Exception("Attempted to access the client. The client is not available in this build.");
    public class ServerIsNotAvailableException() : Exception("Attempted to access the server. The server is not available in this build.");

    #endregion
}