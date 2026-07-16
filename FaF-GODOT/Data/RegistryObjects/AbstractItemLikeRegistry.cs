using Godot;

namespace FaF.Content.RegistryObjects;

[GlobalClass]
public abstract partial class AbstractItemLikeRegistry : Resource
{
    [Export] public string Name;
    [Export] public string Creator;

    [Export] public bool LockedToWorld = false;
    [Export] public string LockedToWorldID = "FaF";

    [Export] public bool ShowInGame = true;
}