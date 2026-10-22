using System.Collections.Generic;

namespace FaF.Services.Content.Resources;

public partial class ModResource : ContentResource
{
    public WorldResource World;

    public List<PropResource> Props = [];
    public List<GearResource> Gears = [];

    #region World

    public WorldResource AddWorld(WorldResource world)
    {
        if (World == null)
            World = world;
        return World;
    }

    #endregion

    #region Props

    public PropResource GetProp(string ID)
        => Props.Find(prop => prop.ID == ID)
            ?? throw new ContentService.PropNotFoundException($"{NAMESPACE}:{ID}");
    
    public bool TryGetProp(string ID, out PropResource prop)
    {
        prop = Props.Find(prop => prop.ID == ID);
        return prop != null;
    }

    public PropResource AddProp(PropResource prop)
    {
        if (TryGetProp(prop.ID, out PropResource result))
            return result;
        Props.Add(prop);
        return prop;
    }

    #endregion

    #region Gears

    public GearResource GetGear(string ID)
        => Gears.Find(gear => gear.ID == ID)
            ?? throw new ContentService.GearNotFoundException($"{NAMESPACE}:{ID}");
    
    public bool TryGetGear(string ID, out GearResource gear)
    {
        gear = Gears.Find(gear => gear.ID == ID);
        return gear != null;
    }

    public GearResource AddGear(GearResource gear)
    {
        if (TryGetGear(gear.ID, out GearResource result))
            return result;
        Gears.Add(gear);
        return gear;
    }

    #endregion
}