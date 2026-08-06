using Godot;

namespace FaF.Data.RegistryObjects;

/// <summary>
/// Abstract class for everything with AvatarItem functionality.
/// </summary>
[GlobalClass]
public abstract partial class AbstractAvatarItemLikeRegistry : Registry
{
    [Export] public bool LockedToWorld = false;
}