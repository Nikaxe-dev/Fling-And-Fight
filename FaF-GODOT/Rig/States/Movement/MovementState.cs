using Godot;

namespace FaF.Rig.States.Movement;

[GlobalClass]
public partial class MovementState(bool AllowJumping = true, bool AllowWalking = true, bool AutoSwitchToFreeFall = true, bool ApplyGravity = true) : UpState(ApplyGravity)
{
    [Export] public required FreeFallState FreeFall;
    [Export] public required JumpingState Jumping;
    [Export] public required GroundedState Grounded;
    [Export] public required LandedState Landed;

    public override void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);

        if (AllowJumping && Npc.Jump)
        {
            Machine.SwitchToState(Jumping);
        }

        if (AllowWalking && IsMultiplayerAuthority())
        {
            float walkAcceleration = Npc.WalkAcceleration * (float)delta;

            if (Npc.Velocity.Length() < Npc.WalkSpeed)
            {
                Npc.Velocity += Npc.MoveDirection * Npc.WalkAcceleration * walkAcceleration;
            }

            Vector3 targetVelocity = Npc.MoveDirection * Npc.WalkSpeed;

            Npc.Velocity = new Vector3(
                Mathf.MoveToward(Npc.Velocity.X, targetVelocity.X, Npc.WalkAcceleration * walkAcceleration),
                Npc.Velocity.Y,
                Mathf.MoveToward(Npc.Velocity.Z, targetVelocity.Z, walkAcceleration)
            );
        }

        if (AutoSwitchToFreeFall && !Npc.IsOnFloor())
        {
            Machine.SwitchToState(FreeFall);
        }
    }
}