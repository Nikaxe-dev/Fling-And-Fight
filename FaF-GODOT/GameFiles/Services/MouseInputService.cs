using FaF.Debug;
using FaF.Enums;
using Godot;

namespace FaF.Services;

public partial class MouseInputService : Node
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("UI/MouseInputService");

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

    private Input.MouseModeEnum MouseModeToGD(MouseMode mode) => mode switch {
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
}