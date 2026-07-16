using Godot;

[GlobalClass]
public partial class WorldRegistry : Resource
{
    [Export] public string Name = "UNTITLED";
    [Export] public string Creator = "Unknown";

    [Export] public bool ShowInGame = true;
}