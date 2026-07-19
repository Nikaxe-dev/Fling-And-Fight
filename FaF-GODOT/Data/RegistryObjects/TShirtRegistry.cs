using Godot;

namespace FaF.Data.RegistryObjects;

[GlobalClass]
public partial class TShirtRegistry : AbstractAvatarItemLikeRegistry
{
    [Export] public required CompressedTexture2D TorsoImage;
}