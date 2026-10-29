using System;
using System.Collections.Generic;
using FaF.Core.Lib.Debug;
using FaF.Core.Lib.FileSystem;
using FaF.Core.Modding.Json.LoaderExtensions;
using FaF.Core.Modding.ModExtensions;
using FaF.Core.Registries;
using FaF.Core.Resources;
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

    // Initialization

    [Signal] public delegate void InitializingModsEventHandler();

    [Signal] public delegate void InitializingModEventHandler(ModResource mod);

    [Signal] public delegate void InitializingModRootEventHandler(ModResource mod);
    [Signal] public delegate void InitializedModRootEventHandler(ModResource mod, ulong timeMs);

    [Signal] public delegate void InitializingNamespaceEventHandler(ModResource mod, string namespaceID);
    [Signal] public delegate void InitializedNamespaceEventHandler(ModResource mod, string namespaceID, ulong timeMs);

    [Signal] public delegate void InitializedModEventHandler(ModResource mod, ulong timeMs);

    [Signal] public delegate void InitializedModsEventHandler(ulong timeMs);

    // Loading

    [Signal] public delegate void LoadingModsEventHandler(WorldResource world);

    [Signal] public delegate void LoadingModEventHandler(ModResource mod, WorldResource world);

    [Signal] public delegate void LoadingModRootEventHandler(ModResource mod, WorldResource world);
    [Signal] public delegate void LoadedModRootEventHandler(ModResource mod, WorldResource world, ulong timeMs);

    [Signal] public delegate void LoadingNamespaceEventHandler(ModResource mod, WorldResource world, string namespaceID);
    [Signal] public delegate void LoadedNamespaceEventHandler(ModResource mod, WorldResource world, string namespaceID, ulong timeMs);

    [Signal] public delegate void LoadedModEventHandler(ModResource mod, WorldResource world, ulong timeMs);

    [Signal] public delegate void LoadedModsEventHandler(WorldResource world, ulong timeMs);

    #endregion

    private void SetupLogs()
    {
        InitializingMods += () => LOGGER.IMPORTANT("Initializing mods:");
        InitializedMods += timeMs => LOGGER.IMPORTANT($"Initialized mods in {timeMs} ms");

        InitializingMod += mod => LOGGER.INFO($"\tInitializing {mod.Identifier}");
        InitializedMod += (mod, timeMs) => LOGGER.INFO($"\tInitialized {mod.Identifier} in {timeMs} ms");

        InitializedNamespace += (mod, namespaceID, timeMs) => LOGGER.INFO($"\t\tInitialized {mod.Identifier}/{namespaceID} in {timeMs} ms");

        LoadingMods += world => LOGGER.IMPORTANT("Loading mods:");
        LoadedMods += (world, timeMs) => LOGGER.IMPORTANT($"Loaded mods in {timeMs} ms");

        LoadingMod += (mod, world) => LOGGER.INFO($"\tLoading {mod.Identifier}");
        LoadedMod += (mod, world, timeMs) => LOGGER.INFO($"\tLoaded {mod.Identifier} in {timeMs} ms");

        LoadedNamespace += (mod, world, namespaceId, timeMs) => LOGGER.INFO($"\t\tLoaded {mod.Identifier}/{namespaceId} in {timeMs} ms");
    }

    private void ScanForNamespaces(List<string> resultList, FolderInstance namespaceFolder)
    {
        foreach (FSInstance item in namespaceFolder)
            if (item is FolderInstance folder)
                if (!resultList.Contains(folder.Name))
                    resultList.Add(folder.Name);
    }

    private void InitModDirectory(FolderInstance modFolder)
    {
        // scan for and then load meta file

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
        
        // mod is disabled: skip loading anything more
        if (mod._DISABLED)
            return;
        
        ulong startTime = Time.GetTicksMsec();
        EmitSignal(SignalName.InitializingMod, mod);
        
        // scan for all defined namespaces in the mod
        
        List<string> definedNamespaces = [];
        
        if (modFolder.TryGetChild("assets", out FolderInstance assetsFolder))
            ScanForNamespaces(definedNamespaces, assetsFolder);
        
        if (modFolder.TryGetChild("data", out FolderInstance dataFolder))
            ScanForNamespaces(definedNamespaces, dataFolder);
        
        mod.DefinedNamespaces = [.. definedNamespaces];

        // load root init mod extensions

        ulong rootStartTime = Time.GetTicksMsec();
        EmitSignal(SignalName.InitializingModRoot, mod);
        
        foreach (Node child in GetChildren())
            if (child is IModInitExtension && child is IModRootExtension rootExtension)
                rootExtension.LoadMod(mod);
        
        EmitSignal(SignalName.InitializedModRoot, mod, Time.GetTicksMsec() - rootStartTime);
        
        // load namespace init mod extensions
        
        foreach (string namespaceID in mod.DefinedNamespaces)
        {
            ulong namespaceStartTime = Time.GetTicksMsec();
            EmitSignal(SignalName.InitializingNamespace, mod, namespaceID);

            foreach (Node child in GetChildren())
                if (child is IModInitExtension && child is IModNamespaceExtension namespaceExtension)
                    namespaceExtension.LoadNamespace(mod, namespaceID);

            EmitSignal(SignalName.InitializedNamespace, mod, namespaceID, Time.GetTicksMsec() - namespaceStartTime);
        }

        EmitSignal(SignalName.InitializedMod, mod, Time.GetTicksMsec() - startTime);
    }

    private void InitModsDirectory(FolderInstance modsFolder)
    {
        foreach (FSInstance item in modsFolder)
            if (item is FolderInstance modFolder)
                InitModDirectory(modFolder);
    }
    
    private void InitMods()
    {
        ulong startTime = Time.GetTicksMsec();
        EmitSignal(SignalName.InitializingMods);

        InitModsDirectory(FolderInstance.OpenOrError(PROJECT_MODS_DIRECTORY));
        InitModsDirectory(FolderInstance.Open(USER_MODS_DIRECTORY, true));

        EmitSignal(SignalName.InitializedMods, Time.GetTicksMsec() - startTime);
    }

    private void LoadMod(ModResource mod, WorldResource world)
    {
        // skip loading if the mod is configured to be disabled
        if (mod._DISABLED)
            return;

        ulong startTime = Time.GetTicksMsec();
        EmitSignal(SignalName.LoadingMod, mod, world);

        // load root load mod extensions

        ulong rootStartTime = Time.GetTicksMsec();
        EmitSignal(SignalName.LoadingModRoot, mod, world);

        foreach (Node child in GetChildren())
            if (child is IModLoadExtension loadExtension && child is IModRootExtension rootExtension)
            {
                loadExtension.LoadingWorld = world;
                rootExtension.LoadMod(mod);

                // set back to null when not in use to avoid any memory leaks
                loadExtension.LoadingWorld = null;
            }
        
        EmitSignal(SignalName.LoadedModRoot, mod, world, Time.GetTicksMsec() - rootStartTime);
        
        // load namespace load mod extensions

        foreach (string namespaceID in mod.DefinedNamespaces)
        {
            ulong namespaceStartTime = Time.GetTicksMsec();
            EmitSignal(SignalName.LoadingNamespace, mod, world, namespaceID);

            foreach (Node child in GetChildren())
                if (child is IModLoadExtension loadExtension && child is IModNamespaceExtension namespaceExtension)
                {
                    loadExtension.LoadingWorld = world;
                    namespaceExtension.LoadNamespace(mod, namespaceID);
                    loadExtension.LoadingWorld = null;
                }
            
            EmitSignal(SignalName.LoadedNamespace, mod, world, namespaceID, Time.GetTicksMsec() - namespaceStartTime);
        }

        EmitSignal(SignalName.LoadedMod, mod, world, Time.GetTicksMsec() - startTime);
    }

    private void LoadMods(WorldResource world)
    {
        ulong startTime = Time.GetTicksMsec();
        EmitSignal(SignalName.LoadingMods, world);

        foreach (ModResource mod in ModRegistry.GetItems())
            LoadMod(mod, world);
        
        EmitSignal(SignalName.LoadedMods, world, Time.GetTicksMsec() - startTime);
    }

    public override void LoadService()
    {
        base.LoadService();
        SetupLogs();

        ((INodeLoader<ICoreModExtension>)this).LoadDerived();

        InitMods();
        LoadMods(Game.RegistryHolder.GetRegistry<WorldRegistry>().GetItem(Identifier.Parse("new_sedes:new_sedes")));
    }

    public class ModMetaFileNotFoundException(FolderInstance modFolder) : Exception($"A ID.mod.json file was not found in {modFolder}.");
    public class MultipleModMetaFilesFoundException(FolderInstance modFolder) : Exception($"Multiple ID.mod.json files were found in {modFolder}. Mods should only have one meta file.");
}