using System.Text.Json.Nodes;
using FaF.Core.Extensions;
using FaF.Core.Resources.Content;

namespace FaF.Core.Modding.Json.LoaderExtensions;

public partial class ModResourceLoaderExtension : ItemLikeResourceLoaderExtension<ModResource, ModResourceLoaderExtension>
{
    protected override ModResource LoadThis(ModResource resource, JsonNode node)
    {
        if (node.TryGetNodeAsNative("_DISABLED", out bool disabled))
            resource._DISABLED = disabled;

        if (node.TryGetNode("Links", out JsonNode linksNode))
            foreach (var (key, value) in linksNode.AsObject())
                resource.Links[key] = value.AsNative<string>();

        return base.LoadThis(resource, node);
    }
}