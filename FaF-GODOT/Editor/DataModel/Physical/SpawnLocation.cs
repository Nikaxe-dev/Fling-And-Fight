using FaF.Editor.Attributes;

namespace FaF.Editor.DataModel.Physical;

public class SpawnLocation : Entity
{
    [EditorAccess("Spawn Radius")]
    public float SpawnRadius;
}