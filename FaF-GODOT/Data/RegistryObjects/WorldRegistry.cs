using Godot;

namespace FaF.Data.RegistryObjects;

[GlobalClass]
public partial class WorldRegistry : Resource
{
    [Export] public string Name = "UNTITLED";
    [Export] public string Creator = "Unknown";

    [Export(PropertyHint.MultilineText)] public string Description;

    [Export] public bool ShowInGame = true;
}