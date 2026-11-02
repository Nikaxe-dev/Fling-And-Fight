using FaF.Core.Lib.FileSystem;
using Godot;

namespace FaF.Core.Resources.Content;

public partial class ContentResource : Resource
{
    public Identifier Identifier;

    public string FilePath;
    public string DirectoryPath => FilePath.GetBaseDir();

    /// <summary>
    /// The json file the resource was loaded from. When using overrides, this points to the declaration file.
    /// </summary>
    public FileInstance File => FileInstance.Open(FilePath);
    
    /// <summary>
    /// The directory <c>File</c> is inside. When using overrides, this uses the declaration file.
    /// </summary>
    public FolderInstance Directory => FolderInstance.Open(DirectoryPath);
}