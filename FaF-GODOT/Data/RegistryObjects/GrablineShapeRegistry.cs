using Godot;

namespace FaF.Data.RegistryObjects;

[GlobalClass]
public partial class GrablineShapeRegistry : AbstractItemLikeRegistry
{
    [Export] public Mesh ShapeMesh = new SphereMesh();
}