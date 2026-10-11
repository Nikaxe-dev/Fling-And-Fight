using System.Text.Json;
using FaF.Core.Services.Content.Resources;

namespace FaF.Core.Services.Modding.ResourceLoaders;

public class PackResourceLoader : ItemLikeResourceLoader<PackResource, PackResourceLoader>
{
    protected override PackResource _Load(PackResource resource, JsonElement element)
    {
        if (TryLoadMetaProperty(out object result, element, "Links", JsonValueKind.Object) && result is JsonElement links)
            foreach (JsonProperty property in links.EnumerateObject())
                resource.Links[property.Name] = (string)LoadMetaProperty(links, property.Name, JsonValueKind.String);

        return base._Load(resource, element);
    }
}