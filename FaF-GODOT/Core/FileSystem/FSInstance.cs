using FaF.Core.Debug;
using Godot;

namespace FaF.Core.FileSystem;

public abstract class FSInstance
{
    protected static Debug.Logger LOGGER = CoreLoggers.FileSystem;

    public string Path {get; protected set;}

    public abstract string Name {get;}
    public FolderInstance Parent => FolderInstance.Open(Path.GetBaseName());

    public override string ToString() => Path;
    public override bool Equals(object obj) => obj != null && obj is FSInstance instance && Path == instance.Path;
    public override int GetHashCode() => Path.GetHashCode();

    public static FSInstance Open(string path) => FileAccess.FileExists(path) ? FileInstance.Open(path) : FolderInstance.Open(path);
}