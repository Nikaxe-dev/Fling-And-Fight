using System.Text.Json;
using FaF.Services.Content.Resources;
using Godot;

namespace FaF.Services.Modding.ResourceLoaders;

public class ItemLikeResourceLoader<ResourceType, ThisType> : ContentResourceLoader<ResourceType, ThisType>
    where ThisType : ItemLikeResourceLoader<ResourceType, ThisType>, new()
    where ResourceType : ItemLikeResource, new()
{
    protected override ResourceType _Load(ResourceType resource, JsonElement element)
    {
        resource.Name = (string)LoadMetaProperty(element, "Name", JsonValueKind.String);
        resource.Description = (string)LoadMetaProperty(element, "Description", JsonValueKind.String);
        resource.Creator = (string)LoadMetaProperty(element, "Creator", JsonValueKind.String);

        return base._Load(resource, element);
    }
}