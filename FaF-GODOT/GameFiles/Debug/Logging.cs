using System;
using System.Collections.Generic;
using FaF.Services;
using Godot;

namespace FaF.Debug;

public static class CoreLoggers
{
    // CORE
    public static readonly Logger Core           = LoggerFactory.GetLogger("Core");
    public static readonly Logger ProgramLoading = LoggerFactory.GetLogger("Core", "ProgramLoading");
    public static readonly Logger ProgramEntry   = LoggerFactory.GetLogger("Core", "ProgramEntry");

    // LIB
    public static readonly Logger FileSystem     = LoggerFactory.GetLogger("Core", "FileSystem");

    // CORE SERVICES
    public static readonly Logger GameLoop       = LoggerFactory.GetLogger("Core", "GameLoop");
    public static readonly Logger Network        = LoggerFactory.GetLogger("Core", "Network");
    public static readonly Logger World          = LoggerFactory.GetLogger("Core", "World");
    public static readonly Logger Player         = LoggerFactory.GetLogger("Core", "Player");

    // ADDON SERVICES
    public static readonly Logger Content        = LoggerFactory.GetLogger("Core", "Content");
    public static readonly Logger Assets         = LoggerFactory.GetLogger("Core", "Assets");
    public static readonly Logger Modding        = LoggerFactory.GetLogger("Core", "Modding");

    // USER SERVICES
    public static readonly Logger UserInput      = LoggerFactory.GetLogger("Core", "UserInput");
}

public class Logger(string section, string module)
{
    public readonly string Section = section;
    public readonly string Module = module;

    public static string RichFormatLog(string environmentInfo, string message, Enums.LogLevel logLevel)
        => logLevel switch
        {
            Enums.LogLevel.FATAL     => $"[b][color=darkred]{environmentInfo}[/color] [color=red]{message}[/color]",
            Enums.LogLevel.ERROR     => $"[color=darkred]{environmentInfo}[/color] [color=red]{message}[/color]",
            Enums.LogLevel.WARNING   => $"[color=orange]{environmentInfo}[/color] [color=yellow]{message}[/color]",
            Enums.LogLevel.IMPORTANT => $"[b][color=darkcyan]{environmentInfo}[/color] [color=cyan]{message}[/color]",
            Enums.LogLevel.INFO      => $"[color=darkcyan]{environmentInfo}[/color] [color=cyan]{message}[/color]",
            _                        => $"[color=gray]{environmentInfo}[/color] [color=white]{message}[/color]"
        };
    
    public static string GenerateInstanceIdentityInfo()
    {
        if (Game.IsGameLoaded)
            if (Game.NetworkService.IsNetworkConnected)
                if (Game.NetworkService.IsServer)
                    return " [Server] ";
                else
                    return $" [Client:{Game.PlayerService.LocalPlayer.Username}] ";
            else
                return "";
        else
            return " [Loading] ";
    }
    
    public static string GenerateCommonEnvironmentInfo()
        => $"[{DateTime.Now:HH:mm:ss:fff}]{GenerateInstanceIdentityInfo()}";
    
    public string GenerateEnvironmentInfo(Enums.LogLevel logLevel)
        => $"{GenerateCommonEnvironmentInfo()} [{Section}{(Module == "" ? "" : $"/{Module}")}] [{logLevel}]";
    
    private void LOG(Enums.LogLevel logLevel, string message)
    {
        GD.PrintRich(RichFormatLog(GenerateEnvironmentInfo(logLevel), message, logLevel));
        if (logLevel == Enums.LogLevel.ERROR || logLevel == Enums.LogLevel.FATAL)
            GD.PushError(message);
        else if (logLevel == Enums.LogLevel.WARNING)
            GD.PushWarning(message);
    }
    
    public void FATAL(string message)
        => LOG(Enums.LogLevel.FATAL, message);
    
    public void ERROR(string message)
        => LOG(Enums.LogLevel.ERROR, message);
    
    public void WARNING(string message)
        => LOG(Enums.LogLevel.WARNING, message);
    
    public void IMPORTANT(string message)
        => LOG(Enums.LogLevel.IMPORTANT, message);
    
    public void INFO(string message)
        => LOG(Enums.LogLevel.INFO, message);
    
    public void DEBUG(string message)
        => LOG(Enums.LogLevel.DEBUG, message);
    
    public void TRACE(string message)
        => LOG(Enums.LogLevel.TRACE, message);
}

public static class LoggerFactory
{
    private static readonly Dictionary<string, Logger> RegisteredLoggers = [];

    public static Logger GetLogger(string section = "Core", string module = "")
    {
        if (RegisteredLoggers.TryGetValue($"{section}:{module}", out Logger existingLogger))
            return existingLogger;
        
        return new(section, module);
    }
}