using Godot;

namespace FaF.Data.RegistryObjects;

# nullable enable

/// <summary>
/// Contains data about a world.
/// </summary>
[GlobalClass]
public partial class WorldRegistry : Registry
{
    [Export] public PackedScene? Scene;
}