using Godot;

namespace FaF.Data.RegistryObjects;

[GlobalClass]
public abstract partial class Registry : Resource
{
    public string FULL_ID {get => $"{NAMESPACE}:{ID}";}

    public string NAMESPACE = "FaF";
    public string ID = "unset";

    public string FILE_PATH = "err_directory_unset";

    [Export] public string Name = "UNTITLED";
    [Export] public string Creator = "Unknown";

    [Export(PropertyHint.MultilineText)] public string Description;
    
    [Export] public bool ShowInGame = true;
}