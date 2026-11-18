using System;
using FaF.Core.Tree.Game.Services.Common;
using FaF.Server.Implementations;
using GodotShell.Runtime;

namespace FaF.Server.Tree.Game.Services;

public partial class ServerCommandService : CommandService, IServerObject
{
    public readonly GDSRuntime Runtime = new GDSRuntime().SetupLib();

    public override void ExecuteCommand(string command)
    {
        base.ExecuteCommand(command);

        try
        {
            Runtime.Execute(command);
        } catch (Exception exception)
        {
            Runtime.Log($"[color=red]Error occured whilst executing '{command}':\n{exception.Message}");
        }
    }

    public override void _Ready()
    {
        base._Ready();

        Runtime.Logged += message
            => Rpc(CommandService.MethodName.OnLog, message);
    }
}