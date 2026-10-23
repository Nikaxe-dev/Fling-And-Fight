using FaF.Core.Modding.Json.LoaderExtensions;
using FaF.Core.Resources.Content;

namespace FaF.Core.Modding.ModExtensions.CoreModExtensions;

public partial class PropRegistryPopulator() : RegistryPopulatorModExtension<PropResource, PropResourceLoaderExtension>(fileExtension: "prop"), ICoreModExtension, IModLoadExtension, IModNamespaceExtension;