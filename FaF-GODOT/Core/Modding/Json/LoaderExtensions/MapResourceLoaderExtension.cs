using System;
using System.Text.Json.Nodes;
using FaF.Core.Extensions;
using FaF.Core.Resources;
using FaF.Core.Resources.Content;

namespace FaF.Core.Modding.Json.LoaderExtensions;

public class MapResourceLoaderExtension : ContentResourceLoaderExtension<MapResource, MapResourceLoaderExtension>
{
    protected override MapResource LoadThis(MapResource resource, JsonNode node)
    {
        resource.Type = Enum.Parse<MapResource.MapType>(node.GetNodeAsNative<string>("Type"));

        if (resource.Type == MapResource.MapType.Godot)
            resource.GDScene = Identifier.Parse(node.GetNodeAsNative<string>("Scene"));

        return base.LoadThis(resource, node);
    }
}