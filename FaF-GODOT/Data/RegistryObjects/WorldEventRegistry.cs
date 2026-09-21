using Godot;

namespace FaF.Data.RegistryObjects;

[GlobalClass]
public partial class WorldEventRegistry : Registry
{
    [Export] public bool IsHiddenEvent = false;
    [Export] public AwardConfiguration Award;
}