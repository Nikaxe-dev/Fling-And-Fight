using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using FaF.Core.Extensions;
using FaF.Core.Implementations;
using FaF.Core.Lib;
using FaF.Core.Lib.Debug;
using FaF.Core.Lib.FileSystem;
using FaF.Core.Tree.Game.Modding.JsonExtensions;
using FaF.Core.Tree.Game.Modding.Extensions;
using FaF.Core.Tree.Game.Registries;
using FaF.Core.Resources;
using FaF.Core.Resources.Content;
using FaF.Core.Tree.Game.Nodes;
using Godot;

using Version = FaF.Core.Lib.Version;
using FaF.Core.Tree.Game.Modding.Implementations;

namespace FaF.Core.Tree.Game.Services.Common;

#nullable enable

public abstract partial class ModLoader : GameService, INodeLoader<ModExtension, ICoreObject>
{
    protected Lib.Debug.Logger LOGGER => LoggerFactory.GetLogger(Game, "FaF", "ModLoader");

    #region Configuration

    public List<string> MOD_LOAD_ORDER = [];

    public static readonly string PROJECT_MODS_DIRECTORY = "res://Mods";
    public static readonly string USER_MODS_DIRECTORY = "user://Mods";

    public static readonly string MOD_LOAD_ORDER_FILE = "user://ModLoadOrder.json";

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

    #region Dependencies

    private void VerifyDependencies(ModResource mod)
    {
        List<(string modIdentifierText, Version? givenVersion, Version recommendedVersion, string versionRequirements)> missingDependencies = [];

        foreach (var (identifierText, versionRequirements) in mod.Dependencies)
            if (ModRegistry.TryGetItem(new Identifier(identifierText), out ModResource requiredMod))
            {
                if (!requiredMod.Version.MatchesRequirements(versionRequirements))
                    missingDependencies.Add((requiredMod.Identifier.ToString(), requiredMod.Version, Version.GetRecommendedVersion(versionRequirements), versionRequirements));
            } else
                missingDependencies.Add((identifierText, null, Version.GetRecommendedVersion(versionRequirements), versionRequirements));
        
        if (missingDependencies.Count > 0)
            throw new ModDependenciesMissingException(mod, [.. missingDependencies]);
    }

    private void VerifyMods()
    {
        foreach (ModResource mod in ModRegistry)
            VerifyDependencies(mod);
    }

    private void AddModToLoadOrder(ModResource mod, bool doCheckDependencies = true)
    {
        if (MOD_LOAD_ORDER.Contains(mod.Identifier.Path))
            return;
        
        foreach (string identifierText in mod.Dependencies.Keys)
            if (ModRegistry.TryGetItem(new(identifierText), out ModResource dependencyMod))
                AddModToLoadOrder(dependencyMod);
        
        MOD_LOAD_ORDER.Add(mod.Identifier.Path);
    }
    
    private static FileInstance GetModLoadOrderSaveFile()
    {
        var (saveFile, newlyCreated) = FileInstance.OpenOrNew(MOD_LOAD_ORDER_FILE);

        if (newlyCreated)
            saveFile.WriteText("[]");

        return saveFile;
    }

    private void UpdateModLoadOrder()
    {
        FileInstance saveFile = GetModLoadOrderSaveFile();

        // load existing contents
        
        JsonArray jsonArray = saveFile.AsJsonNode<JsonArray>();

        MOD_LOAD_ORDER = [];

        foreach (JsonNode? node in jsonArray)
            MOD_LOAD_ORDER.Add(node.AsNative<string>());
        
        // update load order
        foreach (ModResource mod in ModRegistry)
            AddModToLoadOrder(mod);

        // clear out removed mods
        foreach (string identifierText in MOD_LOAD_ORDER.ToArray())
            if (!ModRegistry.TryGetItem(new(identifierText), out _))
                MOD_LOAD_ORDER.Remove(identifierText);
        
        SaveModLoadOrder();
    }

    public void SaveModLoadOrder()
    {
        FileInstance saveFile = GetModLoadOrderSaveFile();

        JsonArray jsonArray = [];

        foreach (string modIdentifierText in MOD_LOAD_ORDER)
            jsonArray.Add(modIdentifierText);
        
        saveFile.WriteJson(jsonArray);
    }

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

    #region Helpers

    private void ScanForNamespaces(List<string> resultList, FolderInstance namespaceFolder)
    {
        foreach (FSInstance item in namespaceFolder)
            if (item is FolderInstance folder)
                if (!resultList.Contains(folder.Name))
                    resultList.Add(folder.Name);
    }

    #endregion

    #region Initialization

    private void InitializeModMeta(FolderInstance modFolder)
    {
        try {
            ModResource? mod = null;
            foreach (FSInstance item in modFolder)
                if (item is FileInstance file)
                    if (file.IsJsonExtension() && file.HasExtension("mod"))
                        if (mod == null)
                        {
                            mod = ModResourceJsonExtension.LoadFile(file);
                            mod.FilePath = file.Path;
                            
                            if (!mod._DISABLED)
                                Game.RegistryHolder.AddToRegistry(new(file.Name), mod);
                        } else
                            throw new MultipleModMetaFilesFoundException(modFolder);

            if (mod == null)
                throw new ModMetaFileNotFoundException(modFolder);

        } catch (Exception ex)
        {
            LOGGER.FATAL($"Fatal problem occured whilst initializing mod in the META stage at {modFolder}:\n{ex}");
        }
    }

    private void InitializeMod(ModResource mod)
    {
        try {
        
        ulong startTime = Time.GetTicksMsec();
        EmitSignal(SignalName.InitializingMod, mod);
        
        // scan for all defined namespaces in the mod
        
        List<string> definedNamespaces = [];
        
        if (mod.Directory.TryGetChild("assets", out FolderInstance assetsFolder))
            ScanForNamespaces(definedNamespaces, assetsFolder);
        
        if (mod.Directory.TryGetChild("data", out FolderInstance dataFolder))
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

        } catch (Exception ex)
        {
            LOGGER.FATAL($"Fatal problem occured whilst initializing mod {mod.Identifier} @ {mod.DirectoryPath}:\n{ex}");
        }
    }

    private void InitializeMods()
    {
        foreach (string modIdentifierText in MOD_LOAD_ORDER)
            if (ModRegistry.TryGetItem(new(modIdentifierText), out ModResource mod))
                InitializeMod(mod);
    }

    private void InitializeModsDirectoryMeta(FolderInstance modsFolder)
    {
        foreach(FSInstance item in modsFolder)
            if (item is FolderInstance modFolder)
                InitializeModMeta(modFolder);
    }

    private void InitializeModsMeta()
    {
        InitializeModsDirectoryMeta(FolderInstance.OpenOrError(PROJECT_MODS_DIRECTORY));
        InitializeModsDirectoryMeta(FolderInstance.Open(USER_MODS_DIRECTORY, true));
    }
    
    private void Initialize()
    {
        ulong startTime = Time.GetTicksMsec();
        EmitSignal(SignalName.InitializingMods);

        // load loader start extensions
        foreach (Node child in GetChildren())
            if (child is IModInitExtension && child is IModLoaderStartExtension loaderStartExtension)
                loaderStartExtension.StartedLoadingMods();

        InitializeModsMeta();

        UpdateModLoadOrder();
        VerifyMods();
        
        InitializeMods();

        // load loader end extensions
        foreach (Node child in GetChildren())
            if (child is IModInitExtension && child is IModLoaderEndExtension loaderEndExtension)
                loaderEndExtension.StoppedLoadingMods();

        EmitSignal(SignalName.InitializedMods, Time.GetTicksMsec() - startTime);
    }

    #endregion

    #region Loading

    private void LoadMod(ModResource mod, WorldResource world)
    {
        try {

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

        } catch (Exception ex)
        {
            LOGGER.FATAL($"Fatal problem occured whilst loading mod {mod.Identifier} @ {mod.DirectoryPath}:\n{ex}");
        }
    }

    private void LoadMods(WorldResource world)
    {
        foreach (string modIdentifierText in MOD_LOAD_ORDER)
            if (ModRegistry.TryGetItem(new(modIdentifierText), out ModResource mod))
                LoadMod(mod, world);
    }

    private void Load(WorldResource world)
    {
        ulong startTime = Time.GetTicksMsec();
        EmitSignal(SignalName.LoadingMods, world);

        // load loader start extensions
        foreach (Node child in GetChildren())
            if (child is IModLoadExtension loadExtension && child is IModLoaderStartExtension loaderStartExtension)
            {
                loadExtension.LoadingWorld = world;
                loaderStartExtension.StartedLoadingMods();
                loadExtension.LoadingWorld = null;
            }

        LoadMods(world);
        
        // load loader end extensions
        foreach (Node child in GetChildren())
            if (child is IModLoadExtension loadExtension && child is IModLoaderEndExtension loaderEndExtension)
            {
                loadExtension.LoadingWorld = world;
                loaderEndExtension.StoppedLoadingMods();
                loadExtension.LoadingWorld = null;
            }
        
        EmitSignal(SignalName.LoadedMods, world, Time.GetTicksMsec() - startTime);
    }

    public override void LoadService()
    {
        base.LoadService();
        SetupLogs();

        ((INodeLoader<ModExtension, ICoreObject>)this).LoadDerived();

        Initialize();
        Load(Game.RegistryHolder.GetRegistry<WorldRegistry>().GetItem(Identifier.Parse("new_sedes:new_sedes")));
    }

    #endregion

    #region Exceptions

    public class ModMetaFileNotFoundException(FolderInstance modFolder) : Exception($"A ID.mod.json file was not found in {modFolder}.") { public readonly string ModFolderPath = modFolder.Path; }
    public class MultipleModMetaFilesFoundException(FolderInstance modFolder) : Exception($"Multiple ID.mod.json files were found in {modFolder}. Mods should only have one meta file.") { public readonly string ModFolderPath = modFolder.Path; }

    public class ModDependenciesMissingException(ModResource mod, (string modIdentifierText, Version? givenVersion, Version recommendedVersion, string versionRequirements)[] missing) : Exception()
    {
        public readonly ModResource Mod = mod;
        public readonly (string modIdentifierText, Version? givenVersion, Version recommendedVersion, string versionRequirements)[] MissingDependencies = missing;

        public override string Message
        {
            get
            {
                string message = $"Mod {Mod.Identifier} is missing dependencies:";

                foreach (var (modIdentifierText, givenVersion, recommendedVersion, versionRequirements) in MissingDependencies)
                    if (givenVersion != null)
                        message = $"{message}\n\t{modIdentifierText}, given version {givenVersion}, recommended to be {recommendedVersion} (exact requirements: {versionRequirements})";
                    else
                        message = $"{message}\n\t{modIdentifierText}, recommended to be version {recommendedVersion} (exact requirements: {versionRequirements})";
                
                return message;
            }
        }
    }

    #endregion
}