using Godot;

namespace FaF.Data.RegistryObjects;

[GlobalClass]
public partial class ItemRegistry : AbstractItemLikeRegistry
{
    [Export] public PackedScene Scene;
}