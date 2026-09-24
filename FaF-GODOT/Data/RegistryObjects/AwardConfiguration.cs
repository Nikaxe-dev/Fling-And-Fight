using Godot;

namespace FaF.Data;

/// <summary>
/// Configures the award given to the player upon activation of the parent registry. (EXAMPLES FOR USE: Events, Achievements)
/// </summary>
[GlobalClass]
public partial class AwardConfiguration : DataResource
{
    [Export] public int MoneyAwarded = 0;
    
    [ExportGroup("Content")]

    [Export] public string[] PropsAwarded = [];
    [Export] public string[] GearsAwarded = [];

    [Export] public string[] GrablineShapesAwarded = [];
    [Export] public string[] GrablineSkinsAwarded = [];

    [Export] public string[] AccessoriesAwarded = [];
    [Export] public string[] PantsAwarded = [];
    [Export] public string[] TShirtsAwarded = [];
    [Export] public string[] ShirtsAwarded = [];

    [Export] public string[] AchievementsAwarded = [];
}