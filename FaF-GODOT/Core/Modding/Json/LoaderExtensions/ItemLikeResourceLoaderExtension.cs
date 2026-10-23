using System.Text.Json;
using FaF.Core.Resources.Content;

namespace FaF.Core.Modding.Json.LoaderExtensions;

public class ItemLikeResourceLoaderExtension<ResourceType, ThisType> : ContentResourceLoaderExtension<ResourceType, ThisType>
    where ResourceType : ItemLikeResource, new()
    where ThisType : ItemLikeResourceLoaderExtension<ResourceType, ThisType>, new()
{
    protected override ResourceType LoadThis(ResourceType resource, JsonElement element, string filePath = null)
    {
        resource.Name = (string)LoadMetaProperty(element, "Name", JsonValueKind.String, filePath);
        resource.Creator = (string)LoadMetaProperty(element, "Creator", JsonValueKind.String, filePath);
        
        if (TryLoadMetaProperty(out object result, element, "Description", JsonValueKind.String, filePath))
            resource.Description = (string)result;

        return base.LoadThis(resource, element, filePath);
    }
}