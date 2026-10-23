using FaF.Core.Modding.Json.LoaderExtensions;
using FaF.Core.Resources.Content;

namespace FaF.Core.Modding.ModExtensions.CoreModExtensions;

public partial class MapRegistryPopulator() : RegistryPopulatorModExtension<MapResource, MapResourceLoaderExtension>(fileExtension: "map"), ICoreModExtension, IModLoadExtension, IModNamespaceExtension;