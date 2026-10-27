using System;
using System.Collections.Generic;
using Godot;

namespace FaF.Core.Lib.Debug;

public enum LogLevel
{
    FATAL,
    ERROR,
    WARNING,
    IMPORTANT,
    INFO,
    DEBUG,
    TRACE
}

public class Logger(GameInstance GameInstance, string section, string module)
{
    /// <summary>
    /// Controls whether the log messages are sent so they all appear at the same position in the output
    /// </summary>
    public static bool LogPaddingEnabled = false;
    
    private static int MaxEnvLength = 0;

    public readonly GameInstance GameInstance = GameInstance;
    public readonly string Section = section;
    public readonly string Module = module;

    public static string RichFormatLog(string environmentInfo, string message, LogLevel logLevel)
        => logLevel switch
        {
            LogLevel.FATAL     => $"[b][color=darkred]{environmentInfo}[/color] [color=red]{message}[/color]",
            LogLevel.ERROR     => $"[color=darkred]{environmentInfo}[/color] [color=red]{message}[/color]",
            LogLevel.WARNING   => $"[color=orange]{environmentInfo}[/color] [color=yellow]{message}[/color]",
            LogLevel.IMPORTANT => $"[b][color=darkcyan]{environmentInfo}[/color] [color=cyan]{message}[/color]",
            LogLevel.INFO      => $"[color=darkcyan]{environmentInfo}[/color] [color=cyan]{message}[/color]",
            _                  => $"[color=gray]{environmentInfo}[/color] [color=white]{message}[/color]"
        };
    
    // TODO: add instance identity info to logger
    public string GenerateInstanceIdentityInfo()
        => GameInstance == null ? "[Core]" : (GameInstance is IClientInstance ? "[Client]" : "[Server]");
    
    public string GenerateCommonEnvironmentInfo()
        => $"[{DateTime.Now:HH:mm:ss:fff}] {GenerateInstanceIdentityInfo()}";
    
    public string GenerateEnvironmentInfo(LogLevel logLevel)
        => $"{GenerateCommonEnvironmentInfo()} [{Section}] {(Module == "" ? "" : $"[{Module}] ")}[{logLevel}]";
    
    private void LOG(LogLevel logLevel, string message)
    {
        string envInfo = GenerateEnvironmentInfo(logLevel);

        if (envInfo.Length > MaxEnvLength)
            MaxEnvLength = envInfo.Length;
        
        if (LogPaddingEnabled) {
            int messagePadding = MaxEnvLength - envInfo.Length;

            for (int i=0; i<messagePadding; i++)
                message = $" {message}";
        }

        GD.PrintRich(RichFormatLog(envInfo, message, logLevel));
        if (logLevel == LogLevel.ERROR || logLevel == LogLevel.FATAL)
            GD.PushError(message);
        else if (logLevel == LogLevel.WARNING)
            GD.PushWarning(message);
    }
    
    public void FATAL(string message)
        => LOG(LogLevel.FATAL, message);
    
    public void ERROR(string message)
        => LOG(LogLevel.ERROR, message);
    
    public void WARNING(string message)
        => LOG(LogLevel.WARNING, message);
    
    public void IMPORTANT(string message)
        => LOG(LogLevel.IMPORTANT, message);
    
    public void INFO(string message)
        => LOG(LogLevel.INFO, message);
    
    public void DEBUG(string message)
        => LOG(LogLevel.DEBUG, message);
    
    public void TRACE(string message)
        => LOG(LogLevel.TRACE, message);
}

public static class LoggerFactory
{
    private static readonly Dictionary<string, Logger> RegisteredLoggers = [];

    public static Logger GetLogger(GameInstance GameInstance, string section = "FaF", string module = "")
    {
        if (RegisteredLoggers.TryGetValue($"{GameInstance}{section}{module}", out Logger existingLogger))
            return existingLogger;
        
        Logger logger = new(GameInstance, section, module);
        RegisteredLoggers[$"{GameInstance}:{section}:{module}"] = logger;
        return logger;
    }
}