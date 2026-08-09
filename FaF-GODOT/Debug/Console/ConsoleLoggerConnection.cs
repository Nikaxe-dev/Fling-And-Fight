using Godot;
using Godot.Collections;

namespace FaF.Debug.Console;

public sealed partial class GameConsoleLogger : Logger
{
    public override void _LogMessage(string message, bool error)
    {
        if (message.Contains("[FaF]")) return;
        ConsoleUI.Instance.OutputRichString(FaFLogger.RichFormatLog($"{FaFLogger.GenerateCommonLoggerInfo()} (BUILTIN)", message, error ? LogType.ERROR : LogType.INFO));
    }

    public override void _LogError(string function, string file, int line, string code, string rationale, bool editorNotify, int errorType, Array<ScriptBacktrace> scriptBacktraces)
    {
        ConsoleUI.Instance.OutputRichString(FaFLogger.RichFormatLog($"{FaFLogger.GenerateCommonLoggerInfo()} (BUILTIN)", $"{file}:{function}:{line} - '{code}' - {rationale}", LogType.ERROR));
    }
}

public sealed partial class ConsoleLoggerConnection : Node
{
    public ConsoleLoggerConnection() : base() {
        OS.AddLogger(new GameConsoleLogger());
    }
}