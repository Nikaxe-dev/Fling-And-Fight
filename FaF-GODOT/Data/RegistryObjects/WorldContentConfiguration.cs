using Godot;

namespace FaF.Data.RegistryObjects;

[GlobalClass]
public partial class WorldContentConfiguration : DataResource
{
    public enum WorldIncludeBehaviour
    {
        ALL,
        SELF,
        INCLUDE,
        EXCLUDE,
        INCLUDE_CREATORS,
        EXCLUDE_CREATORS
    }

    /// <summary>
    /// The behaviour for the 'Worlds' property. ALL means every world will have its content included with this one, SELF means only this world will be included, INCLUDE means only worlds on the list will be included, & EXCLUDE means everything but the worlds on the list will be loaded. The CREATOR varients of these filter for the crreator instead.
    /// </summary>
    [Export] public WorldIncludeBehaviour IncludeBehaviour = WorldIncludeBehaviour.INCLUDE_CREATORS;

    /// <summary>
    /// A list of worlds that change the behaviour of this content depending on IncludeBehaviour. It is recommended to at least have "FaF" be on here when the IncludeBehaviour is INCLUDE, as the FaF world contains all of the base props & gears (including the grabline). By default this contains "Nikaxe".
    /// </summary>
    [Export] public string[] Worlds = ["Nikaxe", "FaF"];
}