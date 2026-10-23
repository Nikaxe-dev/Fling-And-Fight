using System.Text.Json;
using FaF.Core.Modding.Json.LoaderExtensions;
using FaF.Core.Resources.Content;

namespace FaF.Core.Modding.Json.LoaderExtensions;

public partial class ModResourceLoaderExtension : ItemLikeResourceLoaderExtension<ModResource, ModResourceLoaderExtension>
{
    protected override ModResource LoadThis(ModResource resource, JsonElement element, string filePath = null)
    {
        if (TryLoadMetaProperty(out object result, element, "Links", JsonValueKind.Object, filePath) && result is JsonElement links)
            foreach (JsonProperty property in links.EnumerateObject())
                resource.Links[property.Name] = (string)LoadMetaProperty(links, property.Name, JsonValueKind.String, filePath);

        return base.LoadThis(resource, element, filePath);
    }
}