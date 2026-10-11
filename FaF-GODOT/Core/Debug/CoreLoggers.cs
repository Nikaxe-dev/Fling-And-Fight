namespace FaF.Core.Debug;

public static class CoreLoggers
{
    public static readonly Logger Core = LoggerFactory.GetLogger("Core");
    public static readonly Logger Networking = LoggerFactory.GetLogger("Core", "Networking");

    public static readonly Logger FileSystem = LoggerFactory.GetLogger("Core", "FileSystem");
}