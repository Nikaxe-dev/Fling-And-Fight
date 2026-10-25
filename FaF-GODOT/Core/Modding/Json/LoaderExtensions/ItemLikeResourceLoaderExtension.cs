using System.Text.Json.Nodes;
using FaF.Core.Extensions;
using FaF.Core.Resources.Content;

namespace FaF.Core.Modding.Json.LoaderExtensions;

public class ItemLikeResourceLoaderExtension<ResourceType, ThisType> : ContentResourceLoaderExtension<ResourceType, ThisType>
    where ResourceType : ItemLikeResource, new()
    where ThisType : ItemLikeResourceLoaderExtension<ResourceType, ThisType>, new()
{
    protected override ResourceType LoadThis(ResourceType resource, JsonNode node)
    {
        resource.Name = node.GetNodeAsNative<string>("Name");

        if (node.TryGetNodeAsNative("Creator", out string creator))
            resource.Creator = creator;
        
        if (node.TryGetNodeAsNative("Description", out string description))
            resource.Description = description;

        return base.LoadThis(resource, node);
    }
}