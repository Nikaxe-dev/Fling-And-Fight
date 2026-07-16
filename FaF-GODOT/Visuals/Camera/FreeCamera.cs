using Godot;

namespace FaF.Visuals.Camera;

[GlobalClass, Icon("res://Assets/Textures/Decals/Symbols/Symbol Camera.png")]
public partial class FreeCamera : Camera3D
{
    [Export] public bool UserCanTurn = true;
    [Export] public bool UserCanMove = true;

    [Export] public float CAMERA_TURN_SENS = 0.009f;
    [Export] public float CAMERA_MOVE_SPEED = 15;
    [Export] public float CAMERA_MOVE_ACCELERATION = 5;

    [Export] public float MoveSpeed;

    public void ProcessMovement(double delta)
    {
        Vector2 inputDir = Input.GetVector("movement_left","movement_right","movement_forward","movement_backward");

        if (inputDir == Vector2.Zero)
        {
            MoveSpeed = CAMERA_MOVE_SPEED;
        } else
        {
            MoveSpeed += CAMERA_MOVE_ACCELERATION*(float)delta;
        }

        Vector3 forward = GlobalTransform.Basis.Z;
        Vector3 right = GlobalTransform.Basis.X;
        Position += (right * inputDir.X + forward * inputDir.Y).Normalized() * MoveSpeed * (float)delta;
    }

    public void ProcessCameraLookInput(InputEvent @event)
    {
        bool TurnEnabled = Input.IsMouseButtonPressed(MouseButton.Right);
        Input.MouseMode = TurnEnabled ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;

        if (@event is InputEventMouseMotion motion && TurnEnabled)
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