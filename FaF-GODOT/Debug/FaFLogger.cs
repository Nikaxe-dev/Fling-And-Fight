using System;
using System.Collections.Generic;
using FaF.Networking;
using Godot;

namespace FaF.Debug;

public enum LogType
{
    CRASH,
    ERROR,
    WARNING,
    INFO,
    DEBUG,
    TRACE
}

#nullable enable

public class FaFLogger(string path)
{
    private static readonly Dictionary<string, FaFLogger> loggers = new();

    public static FaFLogger Get(string path)
    {
        if (loggers.TryGetValue(path, out FaFLogger? logger))
        {
            return logger;
        }
        
        var newLogger = new FaFLogger(path);
        loggers[path] = newLogger;
        return newLogger;
    }

    private static string GenerateCommonLoggerInfo()
    {
        return $"[${DateTime.Now:HH:mm:ss:fff}] [{(NetworkManager.Instance.Multiplayer.IsServer() ? "Server" : $"Client:{NetworkManager.Instance.Multiplayer.GetUniqueId()}")}]";
    }

    public void LOG(LogType logType, string message, string category = "General")
    {
        string fullText = $"{GenerateCommonLoggerInfo()} [{logType}] [{path}:{category}] {message}";
        GD.Print(fullText);
    }
}