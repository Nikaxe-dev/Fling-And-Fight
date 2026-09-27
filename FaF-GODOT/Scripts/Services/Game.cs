using System;

namespace FaF.Services;

public partial class Game : NodeSingleton<Game>
{
    public static readonly NetworkService NetworkService = new() {Name = "NetworkService"};
    public static readonly InputService InputService = new() {Name = "InputService"};
    public static readonly MouseInputService MouseInputService = new() {Name = "MouseInputService"};

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

        AddChild(NetworkService);
        AddChild(InputService);
        AddChild(MouseInputService);
    }
}