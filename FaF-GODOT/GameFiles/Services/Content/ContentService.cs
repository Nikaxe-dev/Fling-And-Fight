using System;
using System.Collections.Generic;
using FaF.Debug;
using FaF.Services.Content.Resources;

namespace FaF.Services.Content;

public partial class ContentService() : Service([])
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("Services/ContentService");

    #region State

    public readonly List<PackResource> Packs = [];
    public readonly List<ModResource> Mods = [];

    #endregion

    #region Data Handling

    public void ClearRunningData()
        => Mods.Clear();

    public void ClearData()
        => Packs.Clear();

    #endregion

    #region Packs
    
    public PackResource GetPack(string packID)
    {
        PackResource pack = Packs.Find(pack => pack.ID == packID)
            ?? throw new PackNotFoundException(packID);

        return pack;
    }

    public bool TryGetPack(string packID, out PackResource pack)
    {
        pack = Packs.Find(pack => pack.ID == packID);
        return pack != null;
    }

    #endregion

    #region Mods

    public ModResource GetMod(string modID)
        => Mods.Find(mod => mod.ID == modID) 
            ?? throw new ModNotFoundException(modID);

    public bool TryGetMod(string modID, out ModResource mod)
    {
        mod = Mods.Find(mod => mod.ID == modID);
        return mod != null;
    }

    public ModResource GetOrCreateMod(string modID)
    {
        if (TryGetMod(modID, out ModResource mod))
            return mod;
        else
        {
            mod = new() {ID = modID};
            Mods.Add(mod);
            return mod;
        }
    }

    #endregion

    #region Worlds

    public WorldResource GetWorld(string modID)
        => GetMod(modID).World 
            ?? throw new WorldNotFoundException(modID);

    public bool TryGetWorld(string modID, out WorldResource world)
    {
        if (TryGetMod(modID, out ModResource mod))
        {
            world = mod.World;
            return mod.World != null;
        }

        world = null;
        return false;
    }

    public WorldResource AddWorld(WorldResource world)
        => GetOrCreateMod(world.ID).AddWorld(world);

    #endregion

    #region Props

    public PropResource GetProp(string modID, string ID)
        => GetMod(modID).GetProp(ID);
    
    public bool TryGetProp(string modID, string ID, out PropResource prop)
        => GetMod(modID).TryGetProp(ID, out prop);
    
    public PropResource AddProp(string modID, PropResource prop)
        => GetOrCreateMod(modID).AddProp(prop);

    #endregion

    #region Gears

    public GearResource GetGear(string modID, string ID)
        => GetMod(modID).GetGear(ID);
    
    public bool TryGetGear(string modID, string ID, out GearResource gear)
        => GetMod(modID).TryGetGear(ID, out gear);
    
    public GearResource AddGear(string modID, GearResource gear)
        => GetOrCreateMod(modID).AddGear(gear);

    #endregion

    #region Exceptions

    public class PackNotFoundException(string packID) : Exception($"Pack '{packID}' does not exist.");
    public class ModNotFoundException(string modID) : Exception($"Mod '{modID}' does not exist.");
    public class WorldNotFoundException(string modID) : Exception($"Mod '{modID}' does not have a world definition.");
    public class PropNotFoundException(string fullID) : Exception($"Prop '{fullID}' does not exist.");
    public class GearNotFoundException(string fullID) : Exception($"Gear '{fullID}' does not exist.");

    #endregion
}