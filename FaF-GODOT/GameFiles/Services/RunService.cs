using System;
using Godot;

namespace FaF.Services;

public partial class RunService : Node
{
    #region Signals

    // no use of signal due to variant limitation
    public delegate void ProgramStateChangedEventHandler(Enums.ProgramState newState, Enums.ProgramState oldState);
    public event ProgramStateChangedEventHandler ProgramStateChanged;

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

    public string LoadedWorldID {get; private set;}

    #endregion

    #region State Switching

    public void StartClient(string serverIP, int serverPort)
    {
        Game.NetworkService.CreateClient(serverIP, serverPort);
        ProgramState = Enums.ProgramState.Running;
    }

    public void StopClient()
    {
        Game.NetworkService.StopClient();
        ProgramState = Enums.ProgramState.Between;

        OpenTitleScreen();
    }

    public void StartServer(string worldID, int port)
    {
        Game.NetworkService.CreateServer(port, Game.NetworkService.DEFAULT_MAX_PLAYERS);
        ProgramState = Enums.ProgramState.Running;
    }

    public void StopServer()
    {
        Game.NetworkService.StopServer();
        ProgramState = Enums.ProgramState.Between;
    }

    public void OpenTitleScreen()
    {
        throw new NotImplementedException();
    }

    #endregion
}