using FaF.Core.Resources.Content;

namespace FaF.Core.Tree.Game.Modding.Implementations;

public interface IModNamespaceExtension
{
    public void LoadNamespace(ModResource mod, string namespaceID);
}