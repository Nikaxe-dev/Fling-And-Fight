using FaF.FileSystem;
using Godot;

namespace FaF.Services.Content.Resources;

public partial class ContentResource : Resource
{
    public string FULL_ID => $"{NAMESPACE}:{ID}";

    public string NAMESPACE;
    public string ID;

    public string FILE_PATH;
    public string DIRECTORY_PATH => FILE_PATH.GetBaseDir();

    public FileInstance File => FileInstance.Open(FILE_PATH);
    public FolderInstance Parent => FolderInstance.Open(DIRECTORY_PATH);
}