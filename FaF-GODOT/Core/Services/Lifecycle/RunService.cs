using System;
using FaF.Core.Debug;
using Godot;

namespace FaF.Core.Services.Lifecycle;

public enum ProgramState
{
    Entry,
    Between,
    Running,
    MainMenu
}

public partial class RunService() : Service(["NetworkService"])
{
    private static readonly Debug.Logger LOGGER = CoreLoggers.GameLoop;

    #region Signals

    // no use of signal due to variant limitation
    public delegate void ProgramStateChangedEventHandler(ProgramState newState, ProgramState oldState);
    public event ProgramStateChangedEventHandler ProgramStateChanged;

    [Signal] public delegate void ClientStartedEventHandler(string serverIP, int serverPort);
    [Signal] public delegate void ClientStartingEventHandler(string serverIP, int serverPort);

    [Signal] public delegate void ClientStoppedEventHandler();
    [Signal] public delegate void ClientStoppingEventHandler();

    [Signal] public delegate void ServerStartedEventHandler(string worldID, int port);
    [Signal] public delegate void ServerStartingEventHandler(string worldID, int port);

    [Signal] public delegate void ServerStoppedEventHandler();
    [Signal] public delegate void ServerStoppingEventHandler();

    [Signal] public delegate void ProgramQuittingEventHandler(int exitCode);

    #endregion

    #region Program State
    
    private ProgramState _programState = ProgramState.Entry;
    public ProgramState ProgramState
    {
        get => _programState;
        private set
        {
            ProgramStateChanged?.Invoke(value, _programState);
            _programState = value;
        }
    }

    #endregion

    #region State Switching

    public void StartClient(string serverIP, int serverPort)
    {
        EmitSignal(SignalName.ClientStarting, serverIP, serverPort);

        Game.NetworkService.CreateClient(serverIP, serverPort);

        EmitSignal(SignalName.ClientStarted, serverIP, serverPort);

        ProgramState = ProgramState.Running;
    }

    public void StopClient()
    {
        EmitSignal(SignalName.ClientStopping);

        Game.NetworkService.StopClient();
        ProgramState = ProgramState.Between;

        EmitSignal(SignalName.ClientStopped);

        OpenTitleScreen();
    }

    public void StartServer(string worldID, int port)
    {
        Game.WorldService.LoadWorldRoot();
        Game.WorldService.LoadWorldMap(worldID);

        EmitSignal(SignalName.ServerStarting, port);

        Game.NetworkService.CreateServer(port, Game.NetworkService.DEFAULT_MAX_PLAYERS);

        EmitSignal(SignalName.ServerStarted, port);

        ProgramState = ProgramState.Running;
    }

    public void StopServer(int exitCode = 0)
    {
        EmitSignal(SignalName.ServerStopping);

        Game.NetworkService.StopServer();

        EmitSignal(SignalName.ServerStopped);

        ProgramState = ProgramState.Between;
        QuitProgram(exitCode);
    }

    public void StopSession(int exitCode = 0)
    {
        if (Game.NetworkService.IsServer)
            StopServer(exitCode);
        else if (Game.NetworkService.IsClient)
            StopClient();
        else
            QuitProgram(exitCode);
    }

    public void QuitProgram(int exitCode = 0)
    {
        EmitSignal(SignalName.ProgramQuitting, exitCode);
        GetTree().Quit(exitCode);
    }

    public void Start()
        => OpenTitleScreen();

    public void OpenTitleScreen()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region LoadService()

    private void SetupRunningLogs()
    {
        ProgramStateChanged += (newState, oldState) => LOGGER.IMPORTANT($"Program state changed from {oldState} to {newState}");
    }

    protected override void _LoadService()
    {
        base._LoadService();
        SetupRunningLogs();
    }

    #endregion
}