using FaF.Editor.Attributes;
using Godot;

namespace FaF.Editor.DataModel.Physical;

public class PointInstance : Instance
{
    [EditorAccess("Transform")]
    public Transform3D Transform = Transform3D.Identity;
}