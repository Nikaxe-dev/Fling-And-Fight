using Godot;

namespace FlingAndFight.Content.Registry;

[GlobalClass]
public partial class MapRegistry : Resource
{
    [Export] public string Name;
    [Export] public string Creator;

    [Export] public bool ShowInGame = true;

    [Export] public PackedScene Scene;
}