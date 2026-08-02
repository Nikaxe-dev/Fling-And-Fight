using Godot;

namespace FaF.Data.RegistryObjects;

/// <summary>
/// Contains data about a world.
/// </summary>
[GlobalClass]
public partial class WorldRegistry : Resource
{
    [Export] public string Name = "UNTITLED";
    [Export] public string Creator = "Unknown";

    [Export(PropertyHint.MultilineText)] public string Description;

    [Export] public bool ShowInGame = true;
}