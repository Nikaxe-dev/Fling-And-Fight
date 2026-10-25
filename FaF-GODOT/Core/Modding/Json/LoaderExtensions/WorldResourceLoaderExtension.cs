using System.Collections.Generic;
using System.Text.Json.Nodes;
using FaF.Core.Extensions;
using FaF.Core.Resources;
using FaF.Core.Resources.Content;

namespace FaF.Core.Modding.Json.LoaderExtensions;

public class WorldResourceLoaderExtension : ItemLikeResourceLoaderExtension<WorldResource, WorldResourceLoaderExtension>
{
    protected override WorldResource LoadThis(WorldResource resource, JsonNode node)
    {
        resource.MainMap = Identifier.Parse(node.GetNodeAsNative<string>("MainMap"));

        if (node.TryGetNode("Include", out JsonNode includeNode))
        {
            List<string> include = [];
            foreach (JsonNode child in includeNode.AsArray())
                include.Add(child.AsNative<string>());
            resource.IncludedNamespaces = [.. include];
        }

        if (node.TryGetNode("Exclude", out JsonNode excludeNode))
        {
            List<string> exclude = [];
            foreach (JsonNode child in excludeNode.AsArray())
                exclude.Add(child.AsNative<string>());
            resource.ExcludedNamespaces = [.. exclude];
        }

        return base.LoadThis(resource, node);
    }
}