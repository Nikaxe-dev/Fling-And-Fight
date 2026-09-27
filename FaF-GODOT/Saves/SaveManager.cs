using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using FaF.Core;
using FaF.Game.Networking;
using FaF.Game.Players;
using Godot;

namespace FaF.Saves;

/// <summary>
/// Manages the saving and loading for a game. <br/>
/// Has methods for saving or loading player or world data. <br/>
/// 
/// Saving player data is done on the clients whilst world data is done on the server (most of the time the same computer as the player who is hosting). <br/>
/// 
/// <br/>
/// 
/// <b>Never</b> save important progression or permission data on the player save. This should either be handled by third party services (such as steam) or through saving it on the world save.
/// </summary>
/// <seealso cref="DataSaver"/>
public partial class SaveManager : Manager<SaveManager>
{
    /// <summary>
    /// CLIENT
    /// </summary>
    private void GeneratePlayerSave()
    {
        if (Multiplayer.IsServer()) return;

        JsonObject playerData = [];

        foreach (Node child in GetChildren())
        {
            if (child is DataSaver dataSaver)
                playerData = dataSaver.SavePlayerData(playerData, Player.LocalPlayer);
        }
    }

    /// <summary>
    /// SERVER
    /// </summary>
    /// <param name="document"></param>
    /// <param name="player"></param>
    private void LoadPlayerSave(JsonDocument document, Player player)
    {
        if (!Multiplayer.IsServer()) return;

        foreach (Node child in GetChildren())
        {
            if (child is DataSaver dataSaver)
                dataSaver.LoadPlayerData(document, player);
        }
    }

    /// <summary>
    /// RPC from SERVER to CLIENT
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void RequestPlayerSave()
    {
        
    }

    /// <summary>
    /// RPC from CLIENT to SERVER
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void RecievePlayerSave(string jsonText)
    {
        
    }

    /// <summary>
    /// SERVER
    /// </summary>
    private void GenerateWorldSave()
    {
        
    }

    /// <summary>
    /// SERVER
    /// </summary>
    private void LoadWorldSave()
    {
        
    }
}