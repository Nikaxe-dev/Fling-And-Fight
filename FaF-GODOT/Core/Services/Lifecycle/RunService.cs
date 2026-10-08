using System;
using Godot;

namespace FaF.Services.Lifecycle;

public partial class RunService() : Service(["NetworkService", "WorldService"])
{
    #region Signals

    // no use of signal due to variant limitation
    public delegate void ProgramStateChangedEventHandler(Enums.ProgramState newState, Enums.ProgramState oldState);
    public event ProgramStateChangedEventHandler ProgramStateChanged;

    [Signal] public delegate void ClientStartedEventHandler();
    [Signal] public delegate void ClientStartingEventHandler();

    [Signal] public delegate void ClientStoppedEventHandler();
    [Signal] public delegate void ClientStoppingEventHandler();

    [Signal] public delegate void ServerStartedEventHandler();
    [Signal] public delegate void ServerStartingEventHandler();

    [Signal] public delegate void ServerStoppedEventHandler();
    [Signal] public delegate void ServerStoppingEventHandler();

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
        EmitSignal(SignalName.ClientStarting);

        Game.WorldService.LoadWorldRoot();
        Game.NetworkService.CreateClient(serverIP, serverPort);

        EmitSignal(SignalName.ClientStarted);

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
        EmitSignal(SignalName.ServerStarting);

        Game.NetworkService.CreateServer(port, Game.NetworkService.DEFAULT_MAX_PLAYERS);

        EmitSignal(SignalName.ServerStarted);

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

    public void OpenTitleScreen()
    {
        throw new NotImplementedException();
    }

    #endregion
}