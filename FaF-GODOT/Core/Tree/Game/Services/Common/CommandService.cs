using FaF.Core.Lib.Debug;
using FaF.Core.Tree.Game.Nodes;
using Godot;
using GodotShell.Runtime;

namespace FaF.Core.Tree.Game.Services.Common;

// TODO: clean up this horrible area of the codebase

public abstract partial class CommandService : GameService
{
    protected Lib.Debug.Logger LOGGER => LoggerFactory.GetLogger(Game, "FaF", "Commands");

    private static readonly PackedScene TERMINAL_SCENE = ResourceLoader.Load<PackedScene>("uid://dry76m6j82j3a");

    #region Signals

    public delegate void CommandExecutedEventHandler(CallerData callerData, string command);
    public event CommandExecutedEventHandler CommandExecuted;

    [Signal] public delegate void LoggedEventHandler(string message);

    #endregion

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void OnLog(string msg)
        => EmitSignal(SignalName.Logged, msg);

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void OnCommand(string command, long peerID)
        => CommandExecuted?.Invoke(GenerateCallerData(peerID), command);
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void TransmitCommand(string command)
        => ExecuteCommand(command);
    
    public virtual void ExecuteCommand(string command) {}
    
    public virtual CallerData GenerateCallerData(long peerID)
        => default;

    public override void _Ready()
    {
        base._Ready();
        
        CommandExecuted += (callerData, command) => LOGGER.INFO($"[{callerData.Player?.Username ?? "server"}@FaF ~]$ {command}");
        Logged += message => LOGGER.INFO(message, "[GodotShell]");

        AddChild(TERMINAL_SCENE.Instantiate());
    }
}