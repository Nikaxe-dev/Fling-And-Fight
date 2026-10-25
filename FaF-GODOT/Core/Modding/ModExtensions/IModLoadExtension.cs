using FaF.Core.Resources.Content;

namespace FaF.Core.Modding.ModExtensions;

/// <summary>
/// NOTE: When implementing IModLoadExtension.LoadingWorld, keep it as { get; set; } as outside classes
/// set this before and after calling the load method.
/// </summary>
public interface IModLoadExtension
{
    // uses a property to allow either Root or Namespace mod extensions to work with it.
    // classes implementing this should be able to access it
    public WorldResource LoadingWorld { get; set; }
};