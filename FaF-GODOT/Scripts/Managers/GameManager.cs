namespace FaF.Managers;

public partial class GameManager : NodeSingleton<GameManager>
{
    public static readonly NetworkManager NetworkManager = new();
    public static readonly InputManager InputManager = new();
    public static readonly MouseInputManager MouseInputManager = new();

    public override void _Ready()
    {
        base._Ready();

        AddChild(NetworkManager);
        AddChild(InputManager);
        AddChild(MouseInputManager);
    }
}