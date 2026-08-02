using Godot;

namespace FaF.Data.RegistryObjects;

/// <summary>
/// Contains data about a prop.
/// </summary>
[GlobalClass]
public partial class PropRegistry : AbstractItemLikeRegistry
{
    [Export] public PackedScene Scene;
}