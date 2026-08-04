using Godot;

namespace FaF.Data.RegistryObjects;

# nullable enable

/// <summary>
/// Contains data about a world.
/// </summary>
[GlobalClass]
public partial class WorldRegistry : Resource
{
    [Export] public string Name = "UNTITLED";
    [Export] public string Creator = "Unknown";

    [Export(PropertyHint.MultilineText)] public required string Description;

    [Export] public PackedScene? Scene;
    [Export] public bool ShowInGame = true;
}