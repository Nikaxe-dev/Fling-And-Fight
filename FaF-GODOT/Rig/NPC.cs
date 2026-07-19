using System;
using FaF.Players;
using FaF.Rig.States;
using FaF.Visuals.Camera;
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

    [Export] public Node3D? CameraPivot;

    [ExportGroup("Player Integration")]
    [Export] public Node3D[] FirstPersonHideNodes = [];

    [Export] public Player? player;

    // RUNTIME MOVEMENT
    
    [ExportGroup("Runtime Movement")]
    [Export] public bool Jump = false;
    [Export] public Vector3 MoveDirection = Vector3.Zero;

    public bool OverrideRotation = false;
    public Vector3 RotationOverride = Vector3.Zero;

    public void CreateCameraForPlayer()
    {
        // FIXME: PLAYER IS NULL FOR NO REASON
        if (player != null && !Multiplayer.IsServer() && player.IsLocalPlayer())
		{
			player.CameraPivot = CameraPivot;

			player.Camera = new OrbitalCamera
			{
				Name = "ClientOrbitalCamera",
			};

			AddChild(player.Camera);
			player.Camera.GlobalPosition = CameraPivot?.GlobalPosition ?? GlobalPosition;
			player.Camera.FOV = 90;
		}
    }

    public override void _EnterTree()
    {
        CreateCameraForPlayer();
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