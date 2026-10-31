using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

namespace FaF.Core.Lib.FileSystem;

public class FolderInstance : FSInstance, IEnumerable
{
    public override string Name => Path.GetFile();

    public static FolderInstance Open(string path, bool makeDir = false)
    {
        if (DirAccess.DirExistsAbsolute(path))
            return new FolderInstance() {Path = path};
        else if (makeDir)
        {
            DirAccess.MakeDirRecursiveAbsolute(path);
            return new FolderInstance() {Path = path};
        }

        return null;
    }

    public static new FolderInstance OpenOrError(string path, Exception customException = null)
    {
        FolderInstance result = Open(path);
        if (result != null)
            return result;
        else
            throw customException ?? new PathDoesNotExistException(path);
    }

    public static bool TryOpen(string path, out FolderInstance folder)
    {
        folder = Open(path);
        return folder != null;
    }

    public static bool TryOpenOrError(string path, out FolderInstance folder, Exception customException = null)
    {
        folder = OpenOrError(path, customException);
        return true;
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
    public T GetChild<T>(string joinedPath) where T : FSInstance => (T)FSInstance.Open(Path.PathJoin(joinedPath));

    public bool TryGetChild(string joinedPath, out FSInstance child)
    {
        child = GetChild(joinedPath);
        return child != null;
    }

    public bool TryGetChild<T>(string joinedPath, out T child) where T : FSInstance
    {
        child = (T)GetChild(joinedPath);
        return child != null;
    }
}