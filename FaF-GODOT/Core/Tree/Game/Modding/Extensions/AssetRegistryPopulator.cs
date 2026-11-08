using System.IO;
using FaF.Core.Implementations;
using FaF.Core.Lib.FileSystem;
using FaF.Core.Tree.Game.Registries;
using FaF.Core.Resources.Content;
using FaF.Core.Tree.Game.Modding.Implementations;

namespace FaF.Core.Tree.Game.Modding.Extensions;

public partial class AssetRegistryPopulator : ModExtension, ICoreObject, IModInitExtension, IModNamespaceExtension
{
    private AssetRegistry AssetRegistry => Game.RegistryHolder.GetRegistry<AssetRegistry>();

    private void LoadAssetsFolder(string namespaceID, FolderInstance assetsFolder, FolderInstance baseFolder)
    {
        foreach (FSInstance item in assetsFolder)
            if (item is FileInstance file && !file.IsGodotImportExtension())
                AssetRegistry.Register(new(namespaceID, Path.GetRelativePath(baseFolder.Path, file.BaseName)), file.Path);
            else if (item is FolderInstance folder)
                LoadAssetsFolder(namespaceID, folder, baseFolder);
    }

    public void LoadNamespace(ModResource mod, string namespaceID)
    {
        if (mod.Directory.TryGetChild($"assets/{namespaceID}", out FSInstance child))
            if (child is FolderInstance folder)
                LoadAssetsFolder(namespaceID, folder, folder);
    }
}