using Godot;

namespace FaF.Data.RegistryObjects;

[GlobalClass]
public abstract partial class Registry : Resource
{
    public string FULL_ID = "FaF:unset";

    public string NAMESPACE = "FaF";
    public string ID = "unset";

    [Export] public string Name = "UNTITLED";
    [Export] public string Creator = "Unknown";

    [Export(PropertyHint.MultilineText)] public string Description;
    
    [Export] public bool ShowInGame = true;
}