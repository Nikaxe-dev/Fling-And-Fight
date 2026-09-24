using Godot;

namespace FaF.Core;

/// <summary>
/// A Manager node, containing the 'Instance' static property which turns it into a singleton. Requires the type param 'ThisType' for the instance property which should be the type inheriting from this.
/// </summary>
public abstract partial class Manager<ThisType> : Node where ThisType : Manager<ThisType>
{
    public static ThisType Instance {get; private set;}

    public override void _Ready()
    {
        Instance = (ThisType)this;
    }
}