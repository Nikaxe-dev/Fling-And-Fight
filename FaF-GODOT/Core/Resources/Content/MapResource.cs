using Godot;

namespace FaF.Core.Resources.Content;

public partial class MapResource : ContentResource
{
    public enum MapType
    {
        Godot
    }

    public MapType Type;
    public Identifier GDScene;
}