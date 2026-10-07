using FaF.Debug;
using Godot;

namespace FaF.Services.UserInput;

public partial class MouseInputService() : Service([])
{
    private static readonly Debug.Logger LOGGER = CoreLoggers.UserInput;

    public Enums.MouseMode TargetMouseMode = Enums.MouseMode.Free;
    public Enums.MouseMode CurrentMouseMode = Enums.MouseMode.Free;

    public bool IsInModalMode()
    {
        foreach (Node item in GetTree().GetNodesInGroup("UIModal")) {
            if (item is Control control)
                if (control.Visible)
                    return true;
            else
                LOGGER.WARNING($"Non control node '{item.GetPath()}' has UIModal enabled");
        }
        
        return false;
    }

    private Enums.MouseMode GetMouseMode()
    {
        if (!Input.IsMouseButtonPressed(MouseButton.Right) && IsInModalMode()) return Enums.MouseMode.Free;
        return TargetMouseMode;
    }

    private Input.MouseModeEnum MouseModeToGD(Enums.MouseMode mode) => mode switch {
        Enums.MouseMode.Free => Input.MouseModeEnum.Visible,
        Enums.MouseMode.LockedCenter => Input.MouseModeEnum.Captured,
        _ => Input.MouseModeEnum.Visible
    };

    public bool IsMouseFree() => CurrentMouseMode switch
    {
        Enums.MouseMode.Free => true,
        Enums.MouseMode.LockedCenter => false,
        _ => true
    };

    public override void _Process(double delta)
    {
        CurrentMouseMode = GetMouseMode();
        Input.MouseMode = MouseModeToGD(CurrentMouseMode);
    }
}