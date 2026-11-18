using FaF.Core.Lib.Debug;
using FaF.Core.Tree.Game.Nodes;
using Godot;

namespace FaF.Core.Tree.Game.Services.Common;

public abstract partial class CommandService : GameService
{
    protected Lib.Debug.Logger LOGGER => LoggerFactory.GetLogger(Game, "FaF", "Commands");

    private static readonly PackedScene TERMINAL_SCENE = ResourceLoader.Load<PackedScene>("uid://dry76m6j82j3a");

    #region Signals

    [Signal] public delegate void CommandExecutedEventHandler(string command);
    [Signal] public delegate void LoggedEventHandler(string message);

    #endregion

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void OnLog(string msg)
        => EmitSignal(SignalName.Logged, msg);
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void TransmitCommand(string command)
        => ExecuteCommand(command);
    
    public virtual void ExecuteCommand(string command)
        => EmitSignal(SignalName.CommandExecuted, command);

    public override void _Ready()
    {
        base._Ready();
        
        CommandExecuted += command => LOGGER.INFO($"[server@FaF ~]$ {command}");
        Logged += message => LOGGER.INFO(message, "[GodotShell]");

        AddChild(TERMINAL_SCENE.Instantiate());
    }
}