using Godot;

namespace FaF.Core.Resources.ContentResources;

public partial class WorldResource : ItemLikeResource
{
    public string WorldMap;
    public PackedScene WorldMapScene => Program.AssetService.GetAsset<PackedScene>(WorldMap);
}