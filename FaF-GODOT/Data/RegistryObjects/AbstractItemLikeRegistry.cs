using Godot;

namespace FaF.Data.RegistryObjects;

[GlobalClass]
public abstract partial class AbstractItemLikeRegistry : Resource
{
    [Export] public string Name;
    [Export] public string Creator;

    [Export(PropertyHint.MultilineText)] public string Description;

    [Export] public bool LockedToWorld = false;
    [Export] public string LockedToWorldID = "FaF";

    [Export] public bool ShowInGame = true;
}