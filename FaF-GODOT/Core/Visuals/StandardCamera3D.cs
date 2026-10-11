using FaF.Core.Services;
using FaF.Core.Services.UserInput;
using Godot;

namespace FaF.Core.Visuals;

public enum CameraMode
{
    FirstPerson,
    ThirdPerson,
    Free,
    Locked,
    Unhandled
}

[GlobalClass]
public partial class StandardCamera3D : Camera3D
{
    [Export] public CameraMode Mode = CameraMode.Free;
    
    #region Free Mode

    [Export] public float FreeCameraMoveSpeed = 25;
    [Export] public float FreeCameraMoveAcceleration = 10;
    [Export] public float FreeCameraZoomSpeed = 10;

    [Export] public float CurrentFreeCameraMoveSpeed;

    public void FreeCameraProcessMovement(double delta)
    {
        Vector2 inputDir = Game.InputService.GetVector("movement_left", "movement_right", "movement_forward", "movement_backward", InputContext.Gameplay);

        if (inputDir == Vector2.Zero)
            CurrentFreeCameraMoveSpeed = FreeCameraMoveSpeed;
        else
            CurrentFreeCameraMoveSpeed += FreeCameraMoveAcceleration*(float)delta;
        
        Vector3 forward = GlobalTransform.Basis.Z;
        Vector3 right = GlobalTransform.Basis.X;
        Position += (right * inputDir.X + forward * inputDir.Y).Normalized() * CurrentFreeCameraMoveSpeed * (float)delta;
    }

    public void FreeCameraProcessLookInput(InputEvent @event)
    {
        bool TurnEnabled = Input.IsMouseButtonPressed(MouseButton.Right);
        Game.MouseInputService.TargetMouseMode = TurnEnabled ? MouseMode.LockedCenter : MouseMode.Free;

        if (@event is InputEventMouseMotion motion && !Game.MouseInputService.IsMouseFree())
            Rotation += Vector3.Down * motion.Relative.X * TurnSensitivity + Vector3.Left * motion.Relative.Y * TurnSensitivity;
    }

    public void FreeCameraProcessZoomInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            Vector3 direction = ProjectRayNormal(GetViewport().GetMousePosition());
            if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
            {
                Position += direction.Normalized() * FreeCameraZoomSpeed;
                GetViewport().SetInputAsHandled();
            } else if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
            {
                Position -= direction.Normalized() * FreeCameraZoomSpeed;
                GetViewport().SetInputAsHandled();
            }
        }
    }

    #endregion

    #region Base Mode

    [Export] public bool UserCanMove = true;
    [Export] public bool UserCanTurn = true;
    [Export] public bool UserCanZoom = true;

    [Export] public float TurnSensitivity = 0.009f;

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (UserCanTurn)
        {
            if (Mode == CameraMode.Free)
                FreeCameraProcessLookInput(@event);
        }

        if (UserCanZoom)
        {
            if (Mode == CameraMode.Free)
                FreeCameraProcessZoomInput(@event);
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (UserCanMove)
        {
            if (Mode == CameraMode.Free)
                FreeCameraProcessMovement(delta);
        }
    }

    #endregion
}