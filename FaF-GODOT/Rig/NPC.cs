using System;
using FaF.Rig.States;
using Godot;
using Vector3 = Godot.Vector3;

namespace FaF.Rig;

#nullable enable

[GlobalClass, Icon("res://Assets/Textures/Character/Icons/NPCNode.png")]
public partial class NPC : CharacterBody3D
{
    [ExportGroup("Health")]
    [Export] public float MaxHealth = 200;
    [Export] public float Health = 100;

    [ExportGroup("Movement")]
    [Export] public float WalkSpeed = 7.5f;
    [Export] public float WalkAcceleration = 3f;
    [Export] public float JumpPower = 9f;

    [Export] public float TurnSpeed = 10;

    // [ExportGroup("State Machine")]
    // private NPCState _state = NPCState.Movement;

    // [Export] public NPCState State
    // {
    //     get => _state;
    //     set => _state = IsStateEnabled(value) ? value : _state;
    // }

    // [Export] public Godot.Collections.Dictionary<NPCState, bool> EnabledStates = new()
    // {
    //     [NPCState.Dead] = true,
    //     [NPCState.Movement] = true,
    //     [NPCState.Physics] = true,
    //     [NPCState.Ragdolled] = true,
    //     [NPCState.Seated] = true,
    //     [NPCState.Up] = true,
    // };

    [ExportGroup("Animations")]
    [Export] public required NPCAnimationData FALL_ANIMATION;

    [Export] public required NPCAnimationData WALK_ANIMATION;

    [Export] public required NPCAnimationData IDLE_ANIMATION;

    [ExportGroup("Connected Nodes")]
    [Export] public required AnimationPlayer Animator;
    [Export] public required CollisionShape3D Collision;
    [Export] public required StateMachine StateMachine;

    [ExportGroup("Optional Model Parts")]
    [Export] public BoneAttachment3D? HeadBone;
    [Export] public BoneAttachment3D? TorsoBone;

    // public enum NPCState
    // {
    //     /// <summary>
    //     /// The NPC is ragdolled in physics mode.
    //     /// </summary>
    //     Ragdolled,

    //     /// <summary>
    //     /// The NPC is dead. Switching to this state will kill the NPC.
    //     /// </summary>
    //     Dead,

    //     /// <summary>
    //     /// The NPC doesn't apply any of it's own forces and acts in physics mode.
    //     /// </summary>
    //     Physics,

    //     /// <summary>
    //     /// The NPC is seated.
    //     /// </summary>
    //     Seated,

    //     /// <summary>
    //     /// The NPC is up and movable through normal NPC means.
    //     /// </summary>
    //     Movement,
        
    //     /// <summary>
    //     /// The NPC is up.
    //     /// </summary>
    //     Up,
    // }

    // RUNTIME MOVEMENT
    
    [ExportGroup("Runtime Movement")]
    [Export] public bool Jump = false;
    [Export] public Vector3 MoveDirection = Vector3.Zero;

    public bool OverrideRotation = false;
    public Vector3 RotationOverride = Vector3.Zero;

    // STATE MACHINE

    // public bool IsStateEnabled(NPCState state)
    // {
    //     return EnabledStates[state];
    // }

    // // PHYSICS

    // public void PhysicsGravity(double delta)
    // {
    //     if (!IsOnFloor())
    //     {
    //         Velocity += GetGravity() * (float)delta;
    //     }
    // }

    // MOVEMENT/PHYSICS

    // public bool StateAllowsMovement()
    // {
    //     return State == NPCState.Movement;
    // }

    // public void MovementJump()
    // {
    //     Velocity = new Vector3(Velocity.X, JumpPower, Velocity.Z);
    // }

    // public void PhysicsMovement(double delta)
    // {
    //     if (Jump && IsOnFloor()) MovementJump();

    //     if (MoveDirection != Vector3.Zero)
    //     {
    //         if (Velocity.Length() < WalkSpeed)
    //         {
    //             Velocity += MoveDirection * WalkAcceleration;
    //         }

    //         Vector3 targetVelocity = MoveDirection * WalkSpeed;

    //         Velocity = new Vector3(
    //             Mathf.MoveToward(Velocity.X, targetVelocity.X, WalkAcceleration),
    //             Velocity.Y,
    //             Mathf.MoveToward(Velocity.Z, targetVelocity.Z, WalkAcceleration)
    //         );
    //     } else
    //     {
    //         Velocity = new Vector3(Mathf.MoveToward(Velocity.X, 0, WalkAcceleration), Velocity.Y, Mathf.MoveToward(Velocity.Z, 0, WalkAcceleration));
    //     }
    // }

    // // OVERRIDES

    // public override void _Process(double delta)
    // {
    //     if (IsMultiplayerAuthority())
    //     {
    //         ProcessAnimations(delta);
    //     }
    // }

    // public override void _PhysicsProcess(double delta)
    // {
    //     if (IsMultiplayerAuthority())
    //     {
    //         PhysicsGravity(delta);
    //         if (StateAllowsMovement()) PhysicsMovement(delta);
            
    //         PhysicsAnimations(delta);

    //         MoveAndSlide();
    //     }
    // }

    // ANIMATION

    public void PlayAnimation(NPCAnimationData animation, float speed, float blend)
    {
        if (Animator.CurrentAnimation != animation.ANIMATION_ID || speed != Animator.SpeedScale)
        {
            RPCPlayAnimation(animation.ANIMATION_ID, speed, blend);
            Rpc(MethodName.RPCPlayAnimation, animation.ANIMATION_ID, speed, blend);
        }
    }

    public void PlayAnimation(NPCAnimationData animation)
    {
        if (Animator.CurrentAnimation != animation.ANIMATION_ID || animation.PLAYBACK_SPEED != Animator.SpeedScale)
        {
            RPCPlayAnimation(animation.ANIMATION_ID, animation.PLAYBACK_SPEED, animation.PLAYBACK_BLEND);
            Rpc(MethodName.RPCPlayAnimation, animation.ANIMATION_ID, animation.PLAYBACK_SPEED, animation.PLAYBACK_BLEND);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.Authority)]
    public void RPCPlayAnimation(string id, float speed = 1, float blend = 0.2f)
    {
        if (Animator.CurrentAnimation != id)
        {
            Animator.Play(id, blend);
        }
        Animator.SpeedScale = speed;
    }

    // public void PhysicsAnimations(double delta)
    // {
    //     if (MoveDirection != Vector3.Zero && !OverrideRotation)
    //     {
    //         double targetRotation = Math.Atan2(MoveDirection.X, MoveDirection.Z);
    //         Rotation = new Vector3(Rotation.X, (float)Mathf.LerpAngle(Rotation.Y, targetRotation, TurnSpeed * delta), Rotation.Z);
    //     }

    //     if (!IsOnFloor())
    //     {
    //         PlayAnimation(FALL_ANIMATION, 1, 0.1f);
    //     } else if (MoveDirection != Vector3.Zero)
    //     {
    //         PlayAnimation(WALK_ANIMATION, WalkSpeed/5, 0.1f);
    //     } else
    //     {
    //         PlayAnimation(IDLE_ANIMATION, 1, 0.1f);
    //     }
    // }

    // public void ProcessAnimations(double delta)
    // {
    //     if (OverrideRotation)
    //     {
    //         double targetRotation = Math.Atan2(RotationOverride.X, RotationOverride.Z);
    //         Rotation = new Vector3(Rotation.X, (float)Mathf.LerpAngle(Rotation.Y, targetRotation, TurnSpeed * delta), Rotation.Z);
    //     }
    // }
}