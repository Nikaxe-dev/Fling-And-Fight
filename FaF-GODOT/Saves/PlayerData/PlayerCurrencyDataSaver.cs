using System.Text.Json;
using System.Text.Json.Nodes;
using FaF.Game.Players;
using Godot;

namespace FaF.Saves.PlayerData;

/// <summary>
/// Saves all currencies attached to a player in the game.
/// </summary>
[GlobalClass]
public sealed partial class PlayerCurrencyDataSaver : DataSaver
{
    public override JsonObject SavePlayerData(JsonObject playerData, Player player)
    {
        base.SavePlayerData(playerData, player);

        playerData.Add("Money", JsonValue.Create(player.Money));
        return playerData;
    }

    public override void LoadPlayerData(JsonDocument playerData, Player player)
    {
        base.LoadPlayerData(playerData, player);

        if (playerData.RootElement.TryGetProperty("Money", out JsonElement moneyElement)) player.Money = moneyElement.GetInt32();
    }
}