using System.Text.Json;
using FaF.Core.Services.Content.Resources;
using Godot;

namespace FaF.Core.Services.Modding.ResourceLoaders;

public class WorldResourceLoader : ItemLikeResourceLoader<WorldResource, WorldResourceLoader>
{
    protected override WorldResource _Load(WorldResource resource, JsonElement element)
    {
        resource.WorldMap = (string)LoadMetaProperty(element, "WorldMap", JsonValueKind.String);
        return base._Load(resource, element);
    }
}