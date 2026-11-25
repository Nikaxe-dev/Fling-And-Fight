using FaF.Client.Implementations;
using FaF.Client.Tree.World;
using FaF.Core.Tree.Game.Services.Common;
using GodotShell.Runtime;

namespace FaF.Client.Tree.Game.Services;

public partial class ClientCommandService : CommandService, IClientObject
{
    public override void ExecuteCommand(string command)
    {
        base.ExecuteCommand(command);
        RpcId(1, CommandService.MethodName.TransmitCommand, command);
    }

    public override CallerData GenerateCallerData(long peerID)
    {
        if (peerID != 0 && peerID != 1)
                if (Client.PlayerService.TryGetPlayerFromPeerID(peerID, out ClientPlayer player))
                    return new() { Player = player };
                else
                    return default;
        else
            return default;
    }
}