using System;
using System.Collections.Generic;
using System.Text.Json;
using FaF.Debug.Console;
using FaF.Game.Networking;
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

    public static string RichFormatLog(string environmentInfo, string message, LogType logType) => logType switch
    {
        LogType.CRASH => $"[color=darkred]{environmentInfo}[/color] [color=red]{message}[/color]",
        LogType.ERROR => $"[color=darkred]{environmentInfo}[/color] [color=red]{message}[/color]",
        LogType.WARNING => $"[color=orange]{environmentInfo}[/color] [color=yellow]{message}[/color]",
        LogType.INFO => $"[color=darkcyan]{environmentInfo}[/color] [color=cyan]{message}[/color]",
        LogType.DEBUG => $"[color=gray]{environmentInfo}[/color] [color=white]{message}[/color]",
        LogType.TRACE => $"[color=gray]{environmentInfo}[/color] [color=white]{message}[/color]",
        _ => $"[color=gray]{environmentInfo}[/color] [color=white]{message}[/color]",
    };

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

        return newLogger;
    }

    private static string GenerateCommonLoggerInfo()
    {
        return $"[{DateTime.Now:HH:mm:ss:fff}] [{(NetworkManager.Instance.Multiplayer.IsServer() ? "Server" : $"Client:{NetworkManager.Instance.Multiplayer.GetUniqueId()}")}]";
    }

    public void LOG(LogType logType, string message, string category = "General")
    {
        string richFormatted = RichFormatLog($"{GenerateCommonLoggerInfo()} [{logType}] [{path}:{category}]", message, logType);
        if (EnabledLogTypes[logType] && EnabledLogPaths.TryGetValue(path, out bool enabled) && enabled) GD.PrintRich(richFormatted);
        ConsoleUI.Instance.OutputRichString(richFormatted);
    }
}