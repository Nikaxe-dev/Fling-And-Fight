using Godot;

namespace FaF.Data.RegistryObjects;

[GlobalClass]
public partial class GearRegistry : AbstractItemLikeRegistry
{
    [Export] public PackedScene Scene;
}