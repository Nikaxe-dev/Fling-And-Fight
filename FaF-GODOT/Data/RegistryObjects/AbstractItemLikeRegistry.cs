using Godot;

namespace FaF.Data.RegistryObjects;

/// <summary>
/// Abstract class for everything with functionality similar to an 'Item'. (Gears, Props)
/// </summary>
[GlobalClass]
public abstract partial class AbstractItemLikeRegistry : Registry
{
    [Export] public bool LockedToWorld = false;
}