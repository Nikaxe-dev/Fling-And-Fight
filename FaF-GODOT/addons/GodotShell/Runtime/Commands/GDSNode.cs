using System;
using System.Collections.Generic;

namespace GodotShell.Runtime.Commands;

public class GDSNode()
{
    public required GDSRuntime Runtime;

    public GDSNode NextArgument = null;
    public Dictionary<string, GDSCommand> NextCommands = null;

    public GDSNode Previous = null;

    public Delegate Callback = null;

    public GDSCommand Command(string name, string description)
    {
        GDSCommand command = new(name, description)
        {
            Previous = this,
            Runtime = Runtime
        };

        NextCommands ??= [];
        NextCommands[name] = command;
        
        return command;
    }

    public GDSArgument<T> Argument<T>()
    {
        GDSArgument<T> argument = new()
        {
            Previous = this,
            Runtime = Runtime
        };

        NextArgument = argument;
        return argument;
    }

    public GDSNode Executes(Delegate callback)
    {
        Callback = callback;
        return this;
    }
}