using System;
using Godot;

namespace FaF.Game.Rig.States.Movement;

[GlobalClass]
public partial class MovementState(bool AllowJumping = true, bool AllowWalking = true, bool AutoSwitchToFreeFall = true, bool ApplyGravity = true, string walkingAnimation = "WALK_ANIMATION", string idleAnimation = "IDLE_ANIMATION") : UpState(ApplyGravity)
{
    [Export] public required FreeFallState FreeFall;
    [Export] public required JumpingState Jumping;
    [Export] public required GroundedState Grounded;
    [Export] public required LandedState Landed;

    private StringName WalkingAnimation = new(walkingAnimation);
    private StringName IdleAnimation = new(idleAnimation);

    public override void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);

        if (AllowJumping && Npc.Jump)
        {
            Machine.SwitchToState(Jumping);
        }

        if (AllowWalking && IsMultiplayerAuthority())
        {
            float walkAcceleration = Npc.WalkAcceleration;

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

            if (Npc.MoveDirection != Vector3.Zero)
            {
                Npc.PlayAnimation((NPCAnimationData)Npc.Get(WalkingAnimation));

                if (!Npc.OverrideRotation)
                {
                    double targetRotation = Math.Atan2(Npc.MoveDirection.X, Npc.MoveDirection.Z);
                    Npc.Rotation = new Vector3(Npc.Rotation.X, (float)Mathf.LerpAngle(Npc.Rotation.Y, targetRotation, Npc.TurnSpeed * delta), Npc.Rotation.Z);
                }
            } else
            {
                Npc.PlayAnimation((NPCAnimationData)Npc.Get(IdleAnimation));
            }
        }

        if (AutoSwitchToFreeFall && !Npc.IsOnFloor())
        {
            Machine.SwitchToState(FreeFall);
        }
    }

    public override void Process(double delta)
    {
        base.Process(delta);

        if (Npc.OverrideRotation)
        {
            double targetRotation = Math.Atan2(Npc.RotationOverride.X, Npc.RotationOverride.Z);
            Npc.Rotation = new Vector3(Npc.Rotation.X, (float)Mathf.LerpAngle(Npc.Rotation.Y, targetRotation, Npc.TurnSpeed * delta), Npc.Rotation.Z);
        }
    }
}