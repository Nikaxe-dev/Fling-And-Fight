using FaF.Core.Resources.Content;

namespace FaF.Core.Modding.ModExtensions;

public interface IModNamespaceExtension
{
    public void LoadNamespace(ModResource mod, string namespaceID);
}