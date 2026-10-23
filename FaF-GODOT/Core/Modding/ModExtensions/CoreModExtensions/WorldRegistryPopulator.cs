using FaF.Core.Modding.Json.LoaderExtensions;
using FaF.Core.Resources.Content;

namespace FaF.Core.Modding.ModExtensions.CoreModExtensions;

public partial class WorldRegistryPopulator() : RegistryPopulatorModExtension<WorldResource, WorldResourceLoaderExtension>(fileExtension: "world"), ICoreModExtension, IModInitExtension, IModNamespaceExtension;