using FaF.Core.Lib.FileSystem;
using Godot;

namespace FaF.Core.Data.ContentResources;

public partial class ContentResource : Resource
{
    public Identifier Identifier;

    public string FilePath;
    public string DirectoryPath => FilePath.GetBaseDir();

    public FileInstance File => FileInstance.Open(FilePath);
    public FolderInstance Directory => FolderInstance.Open(DirectoryPath);
}