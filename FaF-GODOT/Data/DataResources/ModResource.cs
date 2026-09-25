using Godot;

namespace FaF.Data.DataResources;

public enum WorldType
{
    NoWorld,
    GodotScene
}

public partial class ModResource : ContentResource
{
    public bool ShowInGame = false;
    public WorldType WorldType = WorldType.NoWorld;

    public string FOLDER_PATH;

    public PackedScene Scene;
}