using System;
using System.Collections.Generic;
using FaF.Enums;
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

    public static readonly Dictionary<LogType, bool> EnabledLogTypes = new()
    {
        [LogType.CRASH] = true,
        [LogType.ERROR] = true,
        [LogType.WARNING] = true,
        [LogType.INFO] = true,
        [LogType.DEBUG] = true,
        [LogType.TRACE] = true,
    };

    public static readonly Dictionary<string, bool> EnabledLogPaths = [];

    private static readonly Dictionary<string, FaFLogger> loggers = new();

    public static string RichFormatLog(string environmentInfo, string message, LogType logType, bool isImportant = false) => $"{(isImportant ? "[b]" : "")}{
    
    logType switch
    {
        LogType.CRASH => $"[color=darkred]{environmentInfo}[/color] [color=red]{message}[/color]",
        LogType.ERROR => $"[color=darkred]{environmentInfo}[/color] [color=red]{message}[/color]",
        LogType.WARNING => $"[color=orange]{environmentInfo}[/color] [color=yellow]{message}[/color]",
        LogType.INFO => $"[color=darkcyan]{environmentInfo}[/color] [color=cyan]{message}[/color]",
        LogType.DEBUG => $"[color=gray]{environmentInfo}[/color] [color=white]{message}[/color]",
        LogType.TRACE => $"[color=gray]{environmentInfo}[/color] [color=white]{message}[/color]",
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

        LOGGER.LOG(LogType.DEBUG, $"Registered new LOGGER with path '{path}'", "Registration");

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

    public void LOG(LogType logType, string message, string category = "General", bool isImportant = false)
    {
        string richFormatted = RichFormatLog($"{GenerateCommonLoggerInfo()} [{logType}] [{path}:{category}]", message, logType, isImportant);
        if (EnabledLogTypes[logType] && EnabledLogPaths.TryGetValue(path, out bool enabled) && enabled) GD.PrintRich(richFormatted);
    }
}