using FaF.Editor.Attributes;

namespace FaF.Editor.DataModel.Physical;

[CreationAccess]
public class SpawnLocation : Entity
{
    [EditorAccess("Spawn Radius"), Save]
    public float SpawnRadius {get; set;} = 16;
}