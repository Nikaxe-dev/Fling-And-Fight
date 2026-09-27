using System;

namespace FaF.Managers;

public partial class GameManager : NodeSingleton<GameManager>
{
    public static readonly NetworkManager NetworkManager = new();
    public static readonly InputManager InputManager = new();
    public static readonly MouseInputManager MouseInputManager = new();

    internal static void CreateClient()
    {
        throw new NotImplementedException();
    }

    internal static void CreateServer(string worldID)
    {
        throw new NotImplementedException();
    }

    internal static void SwitchToTitleScreen()
    {
        throw new NotImplementedException();
    }

    public override void _Ready()
    {
        base._Ready();

        AddChild(NetworkManager);
        AddChild(InputManager);
        AddChild(MouseInputManager);
    }
}