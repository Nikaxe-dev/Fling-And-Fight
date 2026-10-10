using FaF.Services;
using FaF.Services.UserInput;
using Godot;

namespace FaF.Visuals.Camera;

public partial class FreeCamera : Camera3D
{
    [Export] public bool UserCanTurn = true;
    [Export] public bool UserCanMove = true;
    [Export] public bool UserCanZoom = true;

    [Export] public float CAMERA_TURN_SENS = 0.009f;
    [Export] public float CAMERA_MOVE_SPEED = 25;
    [Export] public float CAMERA_MOVE_ACCELERATION = 10;
    [Export] public float CAMERA_ZOOM_SPEED = 10;

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
        Game.MouseInputService.TargetMouseMode = TurnEnabled ? MouseMode.LockedCenter : MouseMode.Free;

        if (@event is InputEventMouseMotion motion && !Game.MouseInputService.IsMouseFree())
        {
            Rotation += Vector3.Down * motion.Relative.X * CAMERA_TURN_SENS + Vector3.Left * motion.Relative.Y * CAMERA_TURN_SENS;
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (UserCanTurn) ProcessCameraLookInput(@event);

        if (@event is InputEventMouseButton mouseEvent)
        {
            Vector3 direction = ProjectRayNormal(GetViewport().GetMousePosition());
            if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
            {
                Position += direction.Normalized() * CAMERA_ZOOM_SPEED;
                GetViewport().SetInputAsHandled();
            } else if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
            {
                Position -= direction.Normalized() * CAMERA_ZOOM_SPEED;
                GetViewport().SetInputAsHandled();
            }
        }
    }

    public override void _Process(double delta)
    {
        if (UserCanMove && Input.IsMouseButtonPressed(MouseButton.Right)) ProcessMovement(delta);
    }
}