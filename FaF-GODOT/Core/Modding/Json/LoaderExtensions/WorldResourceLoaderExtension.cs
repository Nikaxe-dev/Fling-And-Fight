using System.Text.Json;
using FaF.Core.Resources;
using FaF.Core.Resources.Content;

namespace FaF.Core.Modding.Json.LoaderExtensions;

public class WorldResourceLoaderExtension : ItemLikeResourceLoaderExtension<WorldResource, WorldResourceLoaderExtension>
{
    protected override WorldResource LoadThis(WorldResource resource, JsonElement element, string filePath = null)
    {
        resource.MainMap = Identifier.Parse((string)LoadMetaProperty(element, "MainMap", JsonValueKind.String, filePath));

        return base.LoadThis(resource, element, filePath);
    }
}