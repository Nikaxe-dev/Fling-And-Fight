using System;
using System.Collections.Generic;
using FaF.Services;
using Godot;

namespace FaF.Debug;

#nullable enable

/// <summary>
/// The FaF logger, wrapping around the GD global logging functions but applying an easy to grasp standard with many filtering options. Also outputs to the in-game console for easy debugging by the developers alongside easy ways for the players to send problems. In the future it will post to a log file somewhere.
/// </summary>
public class FaFLogger(string path)
{
    private static readonly FaFLogger LOGGER = new("Debug/Logger");

    public static readonly Dictionary<Enums.LogType, bool> EnabledLogTypes = new()
    {
        [Enums.LogType.CRASH] = true,
        [Enums.LogType.ERROR] = true,
        [Enums.LogType.WARNING] = true,
        [Enums.LogType.INFO] = true,
        [Enums.LogType.DEBUG] = true,
        [Enums.LogType.TRACE] = true,
    };

    public static readonly Dictionary<string, bool> EnabledLogPaths = [];

    private static readonly Dictionary<string, FaFLogger> loggers = new();

    public static string RichFormatLog(string environmentInfo, string message, Enums.LogType logType, bool isImportant = false) => $"{(isImportant ? "[b]" : "")}{
    
    logType switch
    {
        Enums.LogType.CRASH => $"[color=darkred]{environmentInfo}[/color] [color=red]{message}[/color]",
        Enums.LogType.ERROR => $"[color=darkred]{environmentInfo}[/color] [color=red]{message}[/color]",
        Enums.LogType.WARNING => $"[color=orange]{environmentInfo}[/color] [color=yellow]{message}[/color]",
        Enums.LogType.INFO => $"[color=darkcyan]{environmentInfo}[/color] [color=cyan]{message}[/color]",
        Enums.LogType.DEBUG => $"[color=gray]{environmentInfo}[/color] [color=white]{message}[/color]",
        Enums.LogType.TRACE => $"[color=gray]{environmentInfo}[/color] [color=white]{message}[/color]",
        _ => $"[color=gray]{environmentInfo}[/color] [color=white]{message}[/color]",
    }
    
    }";

    public static FaFLogger Get(string path)
    {
        if (loggers.TryGetValue(path, out FaFLogger? logger))
        {
            return logger;
        }
        
        var newLogger = new FaFLogger(path);
        loggers[path] = newLogger;
        
        if (!EnabledLogPaths.TryGetValue(path, out _))
        {
            EnabledLogPaths[path] = true;
        }

        LOGGER.LOG(Enums.LogType.DEBUG, $"Registered new LOGGER with path '{path}'", "Registration");

        return newLogger;
    }

    private static string GenerateNetworkLoggerInfo()
    {
        return Game.NetworkService.IsNetworkConnected ? (Game.NetworkService.IsServer ? "[Server]" : $"[Client:{Game.NetworkService.LocalPeerID}]") : "[NOT CONNECTED]";
    }

    public static string GenerateCommonLoggerInfo()
    {
        return $"[{DateTime.Now:HH:mm:ss:fff}] {GenerateNetworkLoggerInfo()}";
    }

    private static readonly List<string> queuedUIOutputs = [];

    public void LOG(Enums.LogType logType, string message, string category = "General", bool isImportant = false)
    {
        string richFormatted = RichFormatLog($"{GenerateCommonLoggerInfo()} [{logType}] [{path}:{category}]", message, logType, isImportant);
        if (EnabledLogTypes[logType] && EnabledLogPaths.TryGetValue(path, out bool enabled) && enabled) GD.PrintRich(richFormatted);
    }
}