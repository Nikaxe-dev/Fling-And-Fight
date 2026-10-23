using System;
using FaF.Core.Lib.FileSystem;
using FaF.Core.Modding.Json.LoaderExtensions;
using FaF.Core.Resources.Content;
using Godot;

namespace FaF.Core.Modding.ModExtensions;

public abstract partial class RegistryPopulatorModExtension<ResourceType, LoaderExtension>(string fileExtension, string registryFolderJoinedPath = "data") : GameInstanceAccessor(), IModNamespaceExtension
    where ResourceType : ContentResource, new()
    where LoaderExtension : ContentResourceLoaderExtension<ResourceType, LoaderExtension>, new()
{
    private void LoadFolderRecursive(FolderInstance registryFolder, string namespaceID)
    {
        foreach (FSInstance item in registryFolder)
            if (item is FileInstance file)
                if (file.IsJsonExtension() && file.GetExtension(0) == fileExtension)
                {
                    // stuck on .NET 8.0 so I cant just LoaderExtension.LoadFile()
                    // look for `public static LoadFile()` in LoaderExtension
                    var loadMethod = typeof(LoaderExtension).GetMethod("LoadFile", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                        ?? throw new LoaderExtensionLoadFileMethodNotFound(this);
                    
                    ResourceType newItem = (ResourceType)loadMethod.Invoke(null, [file]);
                    newItem.FilePath = file.Path;

                    Game.RegistryHolder.AddToRegistry(new(namespaceID, file.Name), newItem);
                } else if (item is FolderInstance folder)
                    LoadFolderRecursive(folder, namespaceID);
    }

    public void LoadNamespace(ModResource mod, string namespaceID)
    {
        FolderInstance registryFolder = FolderInstance.Open(mod.DirectoryPath.PathJoin(registryFolderJoinedPath).PathJoin(namespaceID));
        if (registryFolder == null)
            return;
        LoadFolderRecursive(registryFolder, namespaceID);
    }

    public class LoaderExtensionLoadFileMethodNotFound(RegistryPopulatorModExtension<ResourceType, LoaderExtension> causingExtension) : Exception($"{causingExtension.GetType().Name} ran into issue: public static method {typeof(LoaderExtension).Name}.LoadFile was not found");
}