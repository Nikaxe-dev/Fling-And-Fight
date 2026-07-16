using System.Reflection;
using System.Runtime.CompilerServices;
using FlingAndFight.Game.Networking;
using Godot;

namespace FlingAndFight.Game.Debug;

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