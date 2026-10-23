using FaF.Core.Modding.Json.LoaderExtensions;
using FaF.Core.Resources.Content;

namespace FaF.Core.Modding.ModExtensions.CoreModExtensions;

public partial class GearRegistryPopulator() : RegistryPopulatorModExtension<GearResource, GearResourceLoaderExtension>(fileExtension: "gear"), ICoreModExtension, IModLoadExtension, IModNamespaceExtension;