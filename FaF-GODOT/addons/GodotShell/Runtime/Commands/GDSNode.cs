using System;
using System.Collections.Generic;
using GodotShell.Runtime.Values;

namespace GodotShell.Runtime.Commands;

public class GDSNode<ThisType>
    where ThisType : GDSNode<ThisType>
{
    public ThisType Executes(Func<GDSRuntime, Dictionary<string, GDSValue>, GDSValue> callback)
    {
        throw new NotImplementedException();
    }
}