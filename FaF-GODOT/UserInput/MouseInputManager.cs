using FaF.Debug;
using Godot;

namespace FaF.UserInput;

public enum MouseMode
{
    Free,
    LockedCenter,
}

/// <summary>
/// Handles the global mouse mode using a UIModal group similar to Roblox's GuiButton.Modal property.
/// Also uses my own enums and methods for changing the mouse mode, which is understood much better to me.
/// </summary>
public partial class MouseInputManager : Node
{
    public static MouseInputManager Instance;

    private static readonly FaFLogger LOGGER = FaFLogger.Get("UI/MouseInputManager");

    public MouseMode TargetMouseMode = MouseMode.Free;
    public MouseMode CurrentMouseMode = MouseMode.Free;

    public bool IsInModalMode()
    {
        foreach (Node item in GetTree().GetNodesInGroup("UIModal")) {
            if (item is Control control)
            {
                if (control.Visible) return true;
            } else
            {
                LOGGER.LOG(LogType.WARNING, $"Non control node '{item.GetPath()}' has UIModal enabled", "ModalCheck");
            }
        }
        
        return false;
    }

    private MouseMode GetMouseMode()
    {
        if (!Input.IsMouseButtonPressed(MouseButton.Right) && IsInModalMode()) return MouseMode.Free;
        return TargetMouseMode;
    }

    private static Input.MouseModeEnum MouseModeToGD(MouseMode mode) => mode switch {
        MouseMode.Free => Input.MouseModeEnum.Visible,
        MouseMode.LockedCenter => Input.MouseModeEnum.Captured,
        _ => Input.MouseModeEnum.Visible
    };

    public bool IsMouseFree() => CurrentMouseMode switch
    {
        MouseMode.Free => true,
        MouseMode.LockedCenter => false,
        _ => true
    };

    public override void _Process(double delta)
    {
        CurrentMouseMode = GetMouseMode();
        Input.MouseMode = MouseModeToGD(CurrentMouseMode);
    }

    public override void _Ready()
    {
        Instance = this;
    }
}