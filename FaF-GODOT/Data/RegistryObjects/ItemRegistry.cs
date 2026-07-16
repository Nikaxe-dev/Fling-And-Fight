using Godot;

namespace FaF.Content.RegistryObjects;

[GlobalClass]
public partial class ItemRegistry : AbstractItemLikeRegistry
{
    [Export] public PackedScene Scene;
}