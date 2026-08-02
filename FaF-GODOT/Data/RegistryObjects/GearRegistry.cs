using Godot;

namespace FaF.Data.RegistryObjects;

/// <summary>
/// Contains data about a Gear.
/// </summary>
[GlobalClass]
public partial class GearRegistry : AbstractItemLikeRegistry
{
    [Export] public PackedScene Scene;
}