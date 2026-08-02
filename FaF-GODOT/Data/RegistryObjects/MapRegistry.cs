using Godot;

namespace FaF.Data.RegistryObjects;

/// <summary>
/// Contains data about a map under a world.
/// </summary>
[GlobalClass]
public partial class MapRegistry : Resource
{
    [Export] public string Name;
    [Export] public string Creator;

    [Export(PropertyHint.MultilineText)] public string Description;

    [Export] public bool ShowInGame = true;

    [Export] public PackedScene Scene;
}