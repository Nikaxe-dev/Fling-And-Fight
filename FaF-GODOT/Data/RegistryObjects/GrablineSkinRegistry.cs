using Godot;

namespace FaF.Data.RegistryObjects;

[GlobalClass]
public partial class GrablineSkinRegistry : AbstractAvatarItemLikeRegistry
{
    [Export] public Texture2D SkinTexture;
}