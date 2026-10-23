using System;
using Godot;

namespace FaF.Core.Lib.FileSystem;

public abstract class FSInstance
{
    public string Path {get; protected set;}

    public abstract string Name {get;}
    public FolderInstance Parent => FolderInstance.Open(Path.GetBaseName());

    public override string ToString() => Path;
    public override bool Equals(object obj) => obj != null && obj is FSInstance instance && Path == instance.Path;
    public override int GetHashCode() => Path.GetHashCode();

    public static FSInstance Open(string path) => FileAccess.FileExists(path) ? FileInstance.Open(path) : FolderInstance.Open(path);
    public static FSInstance OpenOrError(string path, Exception customException = null) => FileAccess.FileExists(path) ? FileInstance.OpenOrError(path, customException) : FolderInstance.OpenOrError(path, customException);

    public class PathDoesNotExistException(string path) : Exception($"The given file/directory at '{path}' was not found.");
}