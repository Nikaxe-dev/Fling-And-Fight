using FaF.Core.Lib.Debug;
using FaF.Core.Lib.FileSystem;
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

    private void InitModDirectory(FolderInstance modFolder)
    {
        
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
}