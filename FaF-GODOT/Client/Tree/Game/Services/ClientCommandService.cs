using FaF.Client.Implementations;
using FaF.Core.Tree.Game.Services.Common;

namespace FaF.Client.Tree.Game.Services;

public partial class ClientCommandService : CommandService, IClientObject
{
    public override void ExecuteCommand(string command)
    {
        base.ExecuteCommand(command);
        RpcId(1, CommandService.MethodName.TransmitCommand, command);
    }
}