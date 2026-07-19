using System.Reflection;
using System.Runtime.CompilerServices;
using FaF.Networking;
using Godot;

namespace FaF.Debug;

public enum ConsoleCategory
{
    // NETWORKING
    Networking,
    PlayerJoinLog,

    // NPC
    NPC,

    NPCAnimation,

    StateMachine,
    StateMachineMovement,
    StateMachinePhysics,
    StateMachineRagdoll,

    ClientStateChanged,

    // PlayerCharacter
    PlayerCharacter,

    // UI
    UI,
    MainMenu,

    // Visuals
    Visuals,

    // Initialization
    Initialization,

    // Data
    Data,
    DataLoader,
    
    DataWorlds,
    DataMaps,
    
    DataItemLike,
    DataItems,
    DataGears,

    DataAvatarItem,
    DataAccessory,
    DataPants,
    DataShirt,
    DataTShirt
}

public static class FaFConsole
{
    private static string genPrintEnvInfo()
    {
        return $"[{(NetworkManager.Instance.Multiplayer.IsServer() ? "Server" : $"Client:{NetworkManager.Instance.Multiplayer.GetUniqueId()}")}]";
    }

    public static void PrintINFO(string category, string msg)
    {
        string fullText = $"{genPrintEnvInfo()} [{category}] [INFO] {msg}";
        GD.Print(fullText);
    }

    public static void PrintWARNING(string category, string msg)
    {
        string fullText = $"{genPrintEnvInfo()} [{category}] [WARNING] {msg}";
        GD.Print(fullText);
    }

    public static void PrintERROR(string category, string msg)
    {
        string fullText = $"{genPrintEnvInfo()} [{category}] [WARNING] {msg}";
        GD.PrintErr(fullText);
    }
    
    public static void PushERROR(string category, string msg)
    {
        string fullText = $"{genPrintEnvInfo()} [{category}] [WARNING] {msg}";
        GD.PrintErr(fullText);
        GD.PushError(fullText);
    }
}