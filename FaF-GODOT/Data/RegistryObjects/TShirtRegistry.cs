using Godot;

namespace FaF.Data.RegistryObjects;

/// <summary>
/// Contains data about a TShirt AvatarItem.
/// </summary>
[GlobalClass]
public partial class TShirtRegistry : AbstractAvatarItemLikeRegistry
{
    [Export] public required CompressedTexture2D TorsoImage;
}