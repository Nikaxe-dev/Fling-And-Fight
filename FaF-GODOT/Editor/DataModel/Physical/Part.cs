using FaF.Editor.Attributes;
using Godot;

namespace FaF.Editor.DataModel.Physical.World;

public class Part : PointInstance
{
    [EditorAccess("Size")]
    public Vector3 Size = new(1,0.5f,1);
}