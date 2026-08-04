using Godot;

namespace FaF.Data.RegistryObjects;

/// <summary>
/// Abstract class for everything with AvatarItem functionality.
/// </summary>
[GlobalClass]
public abstract partial class AbstractAvatarItemLikeRegistry : Resource
{
    [Export] public string Name;
    [Export] public string Creator;

    [Export(PropertyHint.MultilineText)] public string Description;

    [Export] public bool LockedToWorld = false;
    [Export] public bool ShowInGame = true;
}