using System.Text.Json;
using FaF.Services.Content.Resources;
using Godot;

namespace FaF.Services.Modding.ResourceLoaders;

public class WorldResourceLoader : ItemLikeResourceLoader<WorldResource, WorldResourceLoader>
{
    protected override WorldResource _Load(WorldResource resource, JsonElement element)
    {
        resource.WorldMap = (string)LoadMetaProperty(element, "WorldMap", JsonValueKind.String);
        return base._Load(resource, element);
    }
}