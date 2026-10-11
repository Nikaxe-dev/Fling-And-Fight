using System.Collections;
using System.Collections.Generic;
using Godot;

namespace FaF.Core.FileSystem;

public class FolderInstance : FSInstance, IEnumerable
{
    public override string Name => Path.GetFile();

    public static FolderInstance Open(string path, bool MakeDir = false)
    {
        if (DirAccess.DirExistsAbsolute(path))
            return new FolderInstance() {Path = path};
        else if (MakeDir)
        {
            DirAccess.MakeDirRecursiveAbsolute(path);
            return new FolderInstance() {Path = path};
        }

        return null;
    }

    public IEnumerator GetEnumerator()
    {
        List<FSInstance> result = [];

        foreach (string dirName in DirAccess.GetDirectoriesAt(Path))
            result.Add(Open(Path.PathJoin(dirName)));
        foreach (string fileName in DirAccess.GetFilesAt(Path))
            result.Add(FileInstance.Open(Path.PathJoin(fileName)));
        
        return result.GetEnumerator();
    }

    public FSInstance GetChild(string joinedPath) => FSInstance.Open(Path.PathJoin(joinedPath));

    public bool TryGetChild(string joinedPath, out FSInstance child)
    {
        child = GetChild(joinedPath);
        return child != null;
    }
}