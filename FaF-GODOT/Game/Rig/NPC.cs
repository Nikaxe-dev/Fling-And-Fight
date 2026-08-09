using System;
using FaF.Debug;
using FaF.Game.Players;
using FaF.Game.Rig.States;
using FaF.Visuals.Camera;
using Godot;
using Vector3 = Godot.Vector3;

namespace FaF.Game.Rig;

#nullable enable

/// <summary>
/// An NPC, containing data for movement, health, alongside other systems. Also requires to be instanced from a scene inheriting './Empty_Rig.tscn'. This scene should contain a StateMachine with all of the NPC related states in it.
/// </summary>
[GlobalClass, Icon("res://Assets/Textures/Character/Icons/NPCNode.png")]
public partial class NPC : RigidBody3D
{
    [ExportGroup("Health")]
    [Export] public float MaxHealth = 200;
    [Export] public float Health = 100;

    [ExportGroup("Movement")]
    [Export] public float WalkSpeed = 7.5f;
    [Export] public float WalkAcceleration = 9f;
    [Export] public float JumpPower = 9f;

    [Export] public float TurnSpeed = 10;

    [ExportGroup("Animations")]
    [Export] public required NPCAnimationData FALL_ANIMATION;

    [Export] public required NPCAnimationData WALK_ANIMATION;

    [Export] public required NPCAnimationData IDLE_ANIMATION;

    [ExportGroup("Connected Nodes")]
    [Export] public required AnimationPlayer Animator;
    [Export] public required CollisionShape3D Collision;
    [Export] public required StateMachine StateMachine;
    [Export] public required RayCast3D FloorCast;
    [Export] public required Node3D Model;

    [ExportGroup("Optional Model Parts")]
    [Export] public BoneAttachment3D? HeadBone;
    [Export] public BoneAttachment3D? TorsoBone;

    [Export] public Node3D? CameraPivot;

    [ExportGroup("Player Integration")]
    [Export] public Node3D[] FirstPersonHideNodes = [];

    [Export] public Player? player;

    [Export] public OrbitalCamera? Camera;

    // RUNTIME MOVEMENT
    
    [ExportGroup("Runtime Movement")]
    [Export] public bool Jump = false;
    [Export] public Vector3 MoveDirection = Vector3.Zero;

    public bool OverrideRotation = false;
    public Vector3 RotationOverride = Vector3.Zero;
    public float TurnSpeedOverride = 100;

    public bool IsOnFloor()
    {
        return FloorCast.IsColliding();
    }

    public OrbitalCamera CreateOrbitalCamera(bool doNotUsePlayer = false)
    {
        Camera = new()
        {
            Name = "ClientOrbitalCamera",
        };

        AddChild(Camera);

        if (IsInsideTree())
        {
            Camera.GlobalPosition = CameraPivot?.GlobalPosition ?? GlobalPosition;
        }

        Camera.FOV = 90;

        return Camera;
    }

    public override void _Ready()
    {
        if (Camera != null)
        {
            Camera.GlobalPosition = CameraPivot?.GlobalPosition ?? GlobalPosition;
        }

        CustomIntegrator = true;
    }

    public override void _IntegrateForces(PhysicsDirectBodyState3D state)
    {
        Vector3 floorNormal = Vector3.Up;
        bool onFloor = IsOnFloor();

        if (onFloor)
        {
            floorNormal = FloorCast.GetCollisionNormal();
        }

        Vector3 gravity = GetGravity();

        if (onFloor)
        {
            Vector3 velocity = state.LinearVelocity;

            Vector3 velocityAlongSlope = velocity.Slide(floorNormal);

            if (MoveDirection.LengthSquared() < 0.01f)
            {
                velocity -= velocityAlongSlope;
            }

            state.LinearVelocity = velocity;
            state.LinearVelocity = state.LinearVelocity.Slide(floorNormal);
        } else
        {
            state.LinearVelocity += gravity * state.Step;
        }
    }

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
}