using FaF.Core.Implementations;
using FaF.Core.Tree.Game.Modding.JsonExtensions;
using FaF.Core.Resources.Content;
using FaF.Core.Tree.Game.Modding.Implementations;

namespace FaF.Core.Tree.Game.Modding.Extensions;

public partial class MapRegistryPopulator() : RegistryPopulatorModExtension<MapResource, MapResourceJsonExtension>(fileExtension: "map"), ICoreObject, IModLoadExtension;