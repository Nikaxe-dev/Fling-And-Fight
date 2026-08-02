using System;
using Godot;

namespace FaF.Game.Rig.States.Movement;

/// <summary>
/// A configurable state where the NPC can move around.
/// </summary>
/// <param name="AllowJumping">Whether the NPC can jump in this state.</param>
/// <param name="AllowWalking">Whether the NPC can walk in this state.</param>
/// <param name="AutoSwitchToFreeFall">Whether it switches to FreeFall automatically when detected to be off the ground.</param>
/// <param name="ApplyGravity">Whether to apply gravity in this state.</param>
/// <param name="walkingAnimation">The name of the property that stores the NPCAnimationData that is played when walking.</param>
/// <param name="idleAnimation">The name of the property that stores the NPCAnimationData that is played when idle.</param>
[GlobalClass]
public partial class MovementState(bool AllowJumping = true, bool AllowWalking = true, bool AutoSwitchToFreeFall = true, string walkingAnimation = "WALK_ANIMATION", string idleAnimation = "IDLE_ANIMATION") : UpState
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
            if (Npc.LinearVelocity.Length() < Npc.WalkSpeed)
            {
                Npc.ApplyCentralForce(Npc.MoveDirection * Npc.WalkAcceleration);
            }

            Vector3 targetVelocity = Npc.MoveDirection * Npc.WalkSpeed;

            Npc.LinearVelocity = new Vector3(
                Mathf.MoveToward(Npc.LinearVelocity.X, targetVelocity.X, Npc.WalkAcceleration),
                Npc.LinearVelocity.Y,
                Mathf.MoveToward(Npc.LinearVelocity.Z, targetVelocity.Z, Npc.WalkAcceleration)
            );

            if (Npc.MoveDirection != Vector3.Zero)
            {
                Npc.PlayAnimation((NPCAnimationData)Npc.Get(WalkingAnimation));

                if (!Npc.OverrideRotation)
                {
                    double targetRotation = Math.Atan2(Npc.MoveDirection.X, Npc.MoveDirection.Z);
                    Npc.Rotation = new Vector3(Npc.Rotation.X, (float)Mathf.LerpAngle(Npc.Rotation.Y, targetRotation, Npc.TurnSpeed * delta), Npc.Rotation.Z);
                    Npc.Model.Rotation = Vector3.Zero;
                }
            } else
            {
                Npc.PlayAnimation((NPCAnimationData)(GodotObject)Npc.Get(IdleAnimation));
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
            Npc.Model.GlobalRotation = new Vector3(Npc.Model.GlobalRotation.X, (float)Mathf.LerpAngle(Npc.Model.GlobalRotation.Y, targetRotation, Npc.TurnSpeed * delta), Npc.Model.GlobalRotation.Z);
        }
    }
}