using Godot;

namespace FlingAndFight.Game.Visuals;

[GlobalClass, Icon("res://Assets/Textures/Decals/Symbols/Symbol Camera.png")]
public partial class FreeCamera : Camera3D
{
    [Export] public bool UserCanTurn = true;
    [Export] public bool UserCanMove = true;

    [Export] public float CAMERA_TURN_SENS = 0.009f;
    [Export] public float CAMERA_MOVE_SPEED = 15;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public void ProcessMovement(double delta)
    {
        Vector2 inputDir = Input.GetVector("movement_left","movement_right","movement_forward","movement_backward");
        Vector3 forward = GlobalTransform.Basis.Z;
        Vector3 right = GlobalTransform.Basis.X;
        Position += (right * inputDir.X + forward * inputDir.Y).Normalized() * CAMERA_MOVE_SPEED * (float)delta;
    }

    public void ProcessCameraLookInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion)
        {
            Rotation += Vector3.Down * motion.Relative.X * CAMERA_TURN_SENS + Vector3.Left * motion.Relative.Y * CAMERA_TURN_SENS;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (UserCanTurn) ProcessCameraLookInput(@event);
    }

    public override void _Process(double delta)
    {
        if (UserCanMove) ProcessMovement(delta);
    }
}