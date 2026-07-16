using Godot;

namespace FaF.Content.RegistryObjects;

[GlobalClass]
public partial class GearRegistry : AbstractItemLikeRegistry
{
    [Export] public PackedScene Scene;
}