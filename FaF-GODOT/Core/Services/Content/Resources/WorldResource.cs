using Godot;

namespace FaF.Core.Services.Content.Resources;

public partial class WorldResource : ItemLikeResource
{
    public string WorldMap;
    public PackedScene WorldMapScene => Game.AssetService.GetAsset<PackedScene>(WorldMap);
}