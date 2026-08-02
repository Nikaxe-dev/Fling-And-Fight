using Godot;

namespace FaF.Data.RegistryObjects;

[GlobalClass]
public partial class PropRegistry : AbstractItemLikeRegistry
{
    [Export] public PackedScene Scene;
}