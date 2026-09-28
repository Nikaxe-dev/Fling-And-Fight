using Godot;

namespace FaF.Services.UserInput;

public partial class InputService : Node
{
    public bool GameplayEnabled = true;

    private bool UIEnabledUSR = true;
    public bool UIEnabled {
        get => GetViewport().GuiGetFocusOwner() == null && UIEnabledUSR;
        set => UIEnabledUSR = value;
    }

    public bool GlobalEnabled = true;

    public bool ActionContextAllowed(Enums.InputContext context) => context switch {
        Enums.InputContext.Gameplay => ActionContextAllowed(Enums.InputContext.UI) && GameplayEnabled,
        Enums.InputContext.UI => ActionContextAllowed(Enums.InputContext.Global) && UIEnabled,
        _ => GlobalEnabled
    };
    
    public bool IsActionJustPressed(StringName action, Enums.InputContext context, bool exactMatch = false)
    => ActionContextAllowed(context) && Input.IsActionJustPressed(action, exactMatch);

    public bool IsActionJustReleased(StringName action, Enums.InputContext context, bool exactMatch = false)
    => ActionContextAllowed(context) && Input.IsActionJustReleased(action, exactMatch);

    public bool IsActionPressed(StringName action, Enums.InputContext context, bool exactMatch = false)
    => ActionContextAllowed(context) && Input.IsActionPressed(action, exactMatch);

    public Vector2 GetVector(StringName negativeX, StringName positiveX, StringName negativeY, StringName positiveY, Enums.InputContext context, float deadzone = -1)
    => ActionContextAllowed(context) ? Input.GetVector(negativeX, positiveX, negativeY, positiveY, deadzone) : Vector2.Zero;
}