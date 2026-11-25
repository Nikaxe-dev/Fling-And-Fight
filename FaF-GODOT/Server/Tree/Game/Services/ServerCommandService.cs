using System;
using FaF.Core.Tree.Game.Services.Common;
using FaF.Server.Implementations;
using FaF.Server.Tree.World;
using GodotShell.Runtime;

namespace FaF.Server.Tree.Game.Services;

public partial class ServerCommandService : CommandService, IServerObject
{
    public readonly GDSRuntime Runtime = new GDSRuntime().SetupLib();

    public override void ExecuteCommand(string command)
        => ExecuteCommand(command, false);

    public void ExecuteCommand(string command, bool asServer = false)
    {
        base.ExecuteCommand(command);
        long callerPeerID = asServer ? 1 : Multiplayer.GetRemoteSenderId();

        Rpc(CommandService.MethodName.OnCommand, command, callerPeerID);

        try
        {
            Runtime.Execute(command, GenerateCallerData(asServer ? 1 : callerPeerID));
        } catch (Exception exception)
        {
            Runtime.Log($"[color=red]Error occured whilst executing '{command}':\n{exception.Message}");
        }
    }

    public override void _Ready()
    {
        base._Ready();

        Runtime
            .Command("whoami", "prints info about the command caller")
                .Executes((CallerData callerData) =>
                {
                    if (callerData.Player != null)
                    {
                        Runtime.Log("CLIENT");
                        Runtime.Log($"- Logged into server at port {Server.Port}");
                        Runtime.Log("PLAYER");
                        Runtime.Log($"- Acting as {callerData.Player.Username} with peer ID {callerData.Player.PeerID}");
                    } else
                    {
                        Runtime.Log("SERVER");
                        Runtime.Log($"- Hosted at port {Server.Port}");
                    }
                });
        
        Runtime
            .Command("server", "executes the given string as a command as the server")
                .Argument<string>()
                .Executes((CallerData _, string command) => ExecuteCommand(command, true));

        Runtime.Logged += message
            => Rpc(CommandService.MethodName.OnLog, message);
    }

    public override CallerData GenerateCallerData(long peerID)
    {
        if (peerID != 0 && peerID != 1)
                if (Server.PlayerService.TryGetPlayerFromPeerID(peerID, out ServerPlayer player))
                    return new() { Player = player };
                else
                    return default;
        else
            return default;
    }
}