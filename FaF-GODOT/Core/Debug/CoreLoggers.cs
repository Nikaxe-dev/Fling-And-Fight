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