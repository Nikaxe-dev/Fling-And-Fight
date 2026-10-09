using System;
using FaF.Debug;
using Godot;

namespace FaF.Services.Lifecycle;

public partial class RunService() : Service(["NetworkService"])
{
    private static readonly Debug.Logger LOGGER = CoreLoggers.GameLoop;

    #region Signals

    // no use of signal due to variant limitation
    public delegate void ProgramStateChangedEventHandler(Enums.ProgramState newState, Enums.ProgramState oldState);
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
    
    private Enums.ProgramState _programState = Enums.ProgramState.Entry;
    public Enums.ProgramState ProgramState
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

        ProgramState = Enums.ProgramState.Running;
    }

    public void StopClient()
    {
        EmitSignal(SignalName.ClientStopping);

        Game.NetworkService.StopClient();
        ProgramState = Enums.ProgramState.Between;

        EmitSignal(SignalName.ClientStopped);

        OpenTitleScreen();
    }

    public void StartServer(string worldID, int port)
    {
        EmitSignal(SignalName.ServerStarting, port);

        Game.NetworkService.CreateServer(port, Game.NetworkService.DEFAULT_MAX_PLAYERS);

        EmitSignal(SignalName.ServerStarted, port);

        Game.WorldService.LoadWorldRoot();
        Game.WorldService.LoadWorldMap(worldID);

        ProgramState = Enums.ProgramState.Running;
    }

    public void StopServer()
    {
        EmitSignal(SignalName.ServerStopping);

        Game.NetworkService.StopServer();

        EmitSignal(SignalName.ServerStopped);

        ProgramState = Enums.ProgramState.Between;
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