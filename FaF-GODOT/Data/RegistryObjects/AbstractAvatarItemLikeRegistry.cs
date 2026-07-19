using Godot;

namespace FaF.Data.RegistryObjects;

[GlobalClass]
public abstract partial class AbstractAvatarItemLikeRegistry : Resource
{
    [Export] public string Name;
    [Export] public string Creator;

    [Export(PropertyHint.MultilineText)] public string Description;
}