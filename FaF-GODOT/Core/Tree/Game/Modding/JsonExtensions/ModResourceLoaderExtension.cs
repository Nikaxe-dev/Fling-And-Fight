using System.Text.Json.Nodes;
using FaF.Core.Extensions;
using FaF.Core.Lib;
using FaF.Core.Resources.Content;

namespace FaF.Core.Tree.Game.Modding.JsonExtensions;

public partial class ModResourceJsonExtension : ItemLikeResourceJsonExtension<ModResource, ModResourceJsonExtension>
{
    protected override ModResource LoadThis(ModResource resource, JsonNode node)
    {
        if (node.TryGetNodeAsNative("_DISABLED", out bool disabled))
            resource._DISABLED = disabled;

        if (node.TryGetNode("Links", out JsonNode linksNode))
            foreach (var (key, value) in linksNode.AsObject())
                resource.Links[key] = value.AsNative<string>();
        
        if (node.TryGetNode("Dependencies", out JsonNode dependenciesNode))
            foreach (var (key, value) in dependenciesNode.AsObject())
                resource.Dependencies[key] = value.AsNative<string>();
        
        if (node.TryGetNodeAsNative("Version", out string versionText))
            resource.Version = Version.Parse(versionText);
        
        if (node.TryGetNodeAsNative("VersionName", out string versionName))
            resource.Version.Name = versionName;

        return base.LoadThis(resource, node);
    }
}