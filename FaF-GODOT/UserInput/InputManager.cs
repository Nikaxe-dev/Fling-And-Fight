using Godot;

namespace FaF.UserInput;

public enum InputContext
{
    Gameplay,
    UI,
    Global
}

/// <summary>
/// FaF input manager. Wraps around the basic Godot input manager adding a simple InputContext parameter that makes sure the user isn't in a textbox or anything when giving input.
/// </summary>
public partial class InputManager : Node
{
    public static bool GameplayEnabled = true;

    private static bool UIEnabledUSR = true;
    public static bool UIEnabled {
        get => Instance?.GetViewport().GuiGetFocusOwner() == null && UIEnabledUSR;
        set => UIEnabledUSR = value;
    }

    public static bool GlobalEnabled = true;

    public static bool ActionContextAllowed(InputContext context) => context switch {
        InputContext.Gameplay => ActionContextAllowed(InputContext.UI) && GameplayEnabled,
        InputContext.UI => ActionContextAllowed(InputContext.Global) && UIEnabled,
        _ => GlobalEnabled
    };
    
    public static bool IsActionJustPressed(StringName action, InputContext context, bool exactMatch = false)
    => ActionContextAllowed(context) && Input.IsActionJustPressed(action, exactMatch);

    public static bool IsActionJustReleased(StringName action, InputContext context, bool exactMatch = false)
    => ActionContextAllowed(context) && Input.IsActionJustReleased(action, exactMatch);

    public static bool IsActionPressed(StringName action, InputContext context, bool exactMatch = false)
    => ActionContextAllowed(context) && Input.IsActionPressed(action, exactMatch);

    public static Vector2 GetVector(StringName negativeX, StringName positiveX, StringName negativeY, StringName positiveY, InputContext context, float deadzone = -1)
    => ActionContextAllowed(context) ? Input.GetVector(negativeX, positiveX, negativeY, positiveY, deadzone) : Vector2.Zero;

    private static InputManager Instance;
    public override void _Ready()
    {
        Instance = this;
    }
}