using System;
using System.Collections.Generic;
using FaF.Core.Lib.Debug;
using FaF.Core.Lib.FileSystem;
using FaF.Core.Modding.Json.LoaderExtensions;
using FaF.Core.Modding.ModExtensions;
using FaF.Core.Registries;
using FaF.Core.Resources.Content;
using Godot;

namespace FaF.Core.Services.AbstractServices;

public abstract partial class ModLoader : Service, INodeLoader<ICoreModExtension>
{
    protected Lib.Debug.Logger LOGGER => LoggerFactory.GetLogger(Game, "FaF", "ModLoader");

    #region Configuration

    public readonly string PROJECT_MODS_DIRECTORY = "res://Mods";
    public readonly string USER_MODS_DIRECTORY = "user://Mods";

    #endregion

    #region Connections

    protected ModRegistry ModRegistry => Game.RegistryHolder.GetRegistry<ModRegistry>();

    #endregion

    #region Signals

    [Signal] public delegate void LoadedModOnProgramEventHandler(ModResource mod);

    [Signal] public delegate void LoadingModOnWorldEventHandler(ModResource mod);
    [Signal] public delegate void LoadedModOnWorldEventHandler(ModResource mod);

    #endregion

    private void ScanForNamespaces(List<string> resultList, FolderInstance namespaceFolder)
    {
        foreach (FSInstance item in namespaceFolder)
            if (item is FolderInstance folder)
                if (!resultList.Contains(folder.Name))
                    resultList.Add(folder.Name);
    }

    private void InitModDirectory(FolderInstance modFolder)
    {
        ModResource mod = null;
        foreach (FSInstance item in modFolder)
            if (item is FileInstance file)
                if (file.IsJsonExtension() && file.GetExtension(0) == "mod")
                    if (mod == null)
                    {
                        mod = ModResourceLoaderExtension.LoadFile(file);
                        mod.FilePath = file.Path;
                        Game.RegistryHolder.AddToRegistry(new(file.Name), mod);
                    } else
                        throw new MultipleModMetaFilesFoundException(modFolder);
        
        if (mod == null)
            throw new ModMetaFileNotFoundException(modFolder);
        
        foreach (Node child in GetChildren())
            if (child is IModInitExtension && child is IModRootExtension rootExtension)
                rootExtension.LoadMod(mod);
        
        List<string> namespaces = [];
        
        if (modFolder.TryGetChild("assets", out FolderInstance assetsFolder))
            ScanForNamespaces(namespaces, assetsFolder);
        
        if (modFolder.TryGetChild("data", out FolderInstance dataFolder))
            ScanForNamespaces(namespaces, dataFolder);
        
        foreach (string namespaceID in namespaces)
            foreach (Node child in GetChildren())
                if (child is IModInitExtension && child is IModNamespaceExtension namespaceExtension)
                    namespaceExtension.LoadNamespace(mod, namespaceID);
    }

    private void InitModsDirectory(FolderInstance modsFolder)
    {
        foreach (FSInstance item in modsFolder)
            if (item is FolderInstance modFolder)
                InitModDirectory(modFolder);
    }
    
    private void InitMods()
    {
        InitModsDirectory(FolderInstance.OpenOrError(PROJECT_MODS_DIRECTORY));
        InitModsDirectory(FolderInstance.Open(USER_MODS_DIRECTORY, true));
    }

    private void LoadMod(ModResource mod, WorldResource world)
    {
        
    }

    private void LoadMods(WorldResource world)
    {
        foreach (ModResource mod in ModRegistry.GetItems())
            LoadMod(mod, world);
    }

    public override void LoadService()
    {
        base.LoadService();
        ((INodeLoader<ICoreModExtension>)this).LoadDerived();

        InitMods();
    }

    public class ModMetaFileNotFoundException(FolderInstance modFolder) : Exception($"A ID.mod.json file was not found in {modFolder}.");
    public class MultipleModMetaFilesFoundException(FolderInstance modFolder) : Exception($"Multiple ID.mod.json files were found in {modFolder}. Mods should only have one meta file.");
}