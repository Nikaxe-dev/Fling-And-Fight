using System.Text.Json;
using System.Text.Json.Nodes;
using FaF.Core;
using FaF.Game.Players;
using Godot;

namespace FaF.Saves;

/// <summary>
/// A DataSaver is an abstract class providing functions for saving or loading player or world data. <br/>
/// 
/// <br/>
/// 
/// Saving works through passing the value returned by the Save[Player/World]Data method. The data returned is the data given with properties added. <br/>
/// Loading works much the same, although instead not returning anything and being provided with an immutable JsonDocument.
/// </summary>
/// <seealso cref="SaveManager"/>
[GlobalClass]
public abstract partial class DataSaver : Manager<DataSaver>
{
    public virtual JsonObject SavePlayerData(JsonObject playerData, Player player) => playerData;
    public virtual JsonObject SaveWorldData(JsonObject worldData) => worldData;

    public virtual void LoadPlayerData(JsonDocument playerData, Player player) {}
    public virtual void LoadWorldData(JsonDocument worldData) {}
}