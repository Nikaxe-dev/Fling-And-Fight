namespace FaF.Core.Debug;

public static class Loggers
{
    // CORE
    public static readonly Logger Game           = LoggerFactory.GetLogger("Game");
    public static readonly Logger ProgramLoading = LoggerFactory.GetLogger("Game", "ProgramLoading");
    public static readonly Logger ProgramEntry   = LoggerFactory.GetLogger("Game", "ProgramEntry");

    // LIB
    public static readonly Logger FileSystem     = LoggerFactory.GetLogger("Game", "FileSystem");

    // CORE SERVICES
    public static readonly Logger GameLoop       = LoggerFactory.GetLogger("Game", "GameLoop");
    public static readonly Logger Network        = LoggerFactory.GetLogger("Game", "Network");
    public static readonly Logger World          = LoggerFactory.GetLogger("Game", "World");
    public static readonly Logger Player         = LoggerFactory.GetLogger("Game", "Player");

    // ADDON SERVICES
    public static readonly Logger Content        = LoggerFactory.GetLogger("Game", "Content");
    public static readonly Logger Assets         = LoggerFactory.GetLogger("Game", "Assets");
    public static readonly Logger Modding        = LoggerFactory.GetLogger("Game", "Modding");

    // USER SERVICES
    public static readonly Logger UserInput      = LoggerFactory.GetLogger("Game", "UserInput");
}