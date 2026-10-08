using Godot;

namespace FaF.Services.Content.Resources;

public partial class WorldResource : ItemLikeResource
{
    public string WorldMap;
    public PackedScene WorldMapScene => Game.AssetService.GetAsset<PackedScene>(WorldMap);
}