using System;
using System.Text.Json;
using FaF.Core.Resources;
using FaF.Core.Resources.Content;
using Godot;

namespace FaF.Core.Modding.Json.LoaderExtensions;

public class MapResourceLoaderExtension : ContentResourceLoaderExtension<MapResource, MapResourceLoaderExtension>
{
    protected override MapResource LoadThis(MapResource resource, JsonElement element, string filePath = null)
    {
        // resource.Type = MapResource.MapType[jsonElement.Type]
        // c# can sometimes be a little annoying to use with stuff like this
        resource.Type = (MapResource.MapType)Enum.Parse(typeof(MapResource.MapType), (string)LoadMetaProperty(element, "Type", JsonValueKind.String, filePath));

        if (resource.Type == MapResource.MapType.Godot)
            resource.GDScene = Identifier.Parse((string)LoadMetaProperty(element, "Type", JsonValueKind.String, filePath));

        return base.LoadThis(resource, element, filePath);
    }
}