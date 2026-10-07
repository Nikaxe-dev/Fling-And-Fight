using FaF.Services.Content;
using FaF.Services.Lifecycle;
using FaF.Services.Modding;
using FaF.Services.UserInput;
using FaF.Services.World;

namespace FaF.Services;

public partial class Game() : ServiceLoader([], [typeof(Service), typeof(ServiceLoader)], [typeof(Game)])
{
    private static Game Instance;

    #region State

    public static bool IsGameLoaded => Instance.IsLoaded;

    #endregion

    #region Service Links

    public static NetworkService NetworkService => Instance.GetNode<NetworkService>("NetworkService");
    public static InputService InputService => Instance.GetNode<InputService>("InputService");
    public static MouseInputService MouseInputService => Instance.GetNode<MouseInputService>("MouseInputService");
    public static RunService RunService => Instance.GetNode<RunService>("RunService");
    public static WorldService WorldService => Instance.GetNode<WorldService>("WorldService");
    public static PlayerService PlayerService => Instance.GetNode<PlayerService>("PlayerService");
    public static PackService PackService => Instance.GetNode<PackService>("PackService");
    public static ModService ModService => Instance.GetNode<ModService>("ModService");
    public static ContentService ContentService => Instance.GetNode<ContentService>("ContentService");
    public static AssetService AssetService => Instance.GetNode<AssetService>("AssetService");

    #endregion

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
        LoadService();
    }
}