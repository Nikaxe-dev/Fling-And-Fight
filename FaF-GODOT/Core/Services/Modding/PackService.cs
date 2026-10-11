using System;
using FaF.Core.Debug;
using FaF.Core.FileSystem;
using FaF.Core.Services.Content.Resources;
using FaF.Core.Services.Modding.ResourceLoaders;
using Godot;

namespace FaF.Core.Services.Modding;

public partial class PackService() : Service(["ContentService", "AssetService"])
{
    private static readonly Debug.Logger LOGGER = CoreLoggers.Modding;

    #region Constants

    public static readonly string BUILTIN_PACKS_PATH = "res://Packs";
    public static readonly string USER_PACKS_PATH = "user://Packs";

    public static readonly string PACK_META_EXTENSION = "pack";

    #endregion

    #region Mods

    private void LoadPackMods(FolderInstance packFolder, PackResource pack)
    {
        if (packFolder.TryGetChild("mods", out FSInstance modsChild))
            if (modsChild is FolderInstance modsFolder)
            {
                LOGGER.INFO($"\t\t\\_ mods:");
                foreach (FSInstance instance in modsFolder)
                    if (instance is FolderInstance modFolder)
                        Game.ModService.LoadMod(pack, modFolder);
            }
    }

    #endregion

    #region Packs

    public void LoadPack(FolderInstance packFolder)
    {
        try {
            PackResource pack = LoadPackMeta(packFolder);
            Game.ContentService.Packs.Add(pack);

            LOGGER.INFO($"\t\\_ {pack.Creator}/{pack.ID} @ {pack.DIRECTORY_PATH}:");
            LOGGER.INFO($"\t\t\\_ {pack.FILE_PATH.GetFile()}");
            
            LoadPackMods(packFolder, pack);
        } catch(Exception exception)
        {
            LOGGER.ERROR($"Failed to load pack '{packFolder}' with message:\n{exception.Message}");
        }
    }

    public void LoadPacksAt(FolderInstance packsFolder)
    {
        foreach (FSInstance instance in packsFolder)
            if (instance is FolderInstance packFolder)
                LoadPack(packFolder);
    }

    private PackResource LoadPackMeta(FolderInstance packFolder)
    {
        PackResource pack = null;
        foreach (FSInstance instance in packFolder)
            if (instance is FileInstance file)
                if (file.IsJsonExtension())
                    if (file.GetExtension(0) == PACK_META_EXTENSION)
                    {
                        if (pack != null)
                            throw new MultiplePackMetaFilesException(packFolder);
                        pack = PackResourceLoader.Load(file);
                    }
        
        if (pack == null)
            throw new PackMetaFileNotFoundException(packFolder);

        return pack;
    }

    #endregion

    protected override void _LoadService()
    {
        base._LoadService();
        LOGGER.INFO("Loading packs:");
        LoadPacksAt(FolderInstance.Open(BUILTIN_PACKS_PATH));
        LoadPacksAt(FolderInstance.Open(USER_PACKS_PATH, true));
    }

    #region Exceptions

    public class PackMetaFileNotFoundException(FolderInstance packFolder) : Exception($"A meta file was not found in pack '{packFolder}'.");
    public class MultiplePackMetaFilesException(FolderInstance packFolder) : Exception($"Multiple meta files were found in pack '{packFolder}'. Packs only accept one meta file.");

    #endregion
}