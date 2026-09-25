using Godot;

namespace FaF.Data.DataResources;

public abstract partial class ContentResource : Resource
{
    public string FULL_ID {get => $"{NAMESPACE}:{ID}";}

    public string NAMESPACE;
    public string ID;

    public string FILE_PATH;

    public string Name;
    public string Description;
    
    public string Creator;
    public string[] Contributors = [];
}