using System;

namespace FaF.Services;

public partial class Game : NodeSingleton<Game>
{
    public static readonly NetworkService NetworkService = new() {Name = "NetworkService"};
    public static readonly InputService InputService = new() {Name = "InputService"};
    public static readonly MouseInputService MouseInputService = new() {Name = "MouseInputService"};
    public static readonly RunService RunService = new() {Name = "RunService"};

    public override void _Ready()
    {
        base._Ready();

        AddChild(NetworkService);
        AddChild(InputService);
        AddChild(MouseInputService);
        AddChild(RunService);
    }
}