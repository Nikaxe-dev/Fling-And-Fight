using FaF.Editor.Attributes;
using Godot;

namespace FaF.Editor.DataModel.Physical;

public class PointInstance : Instance
{
    [EditorAccess("Transform"), Save]
    public Transform3D Transform {get; set;} = Transform3D.Identity;
}