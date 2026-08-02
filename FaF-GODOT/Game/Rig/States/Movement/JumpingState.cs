using Godot;

namespace FaF.Game.Rig.States.Movement;

/// <summary>
/// Active for the frame after the NPC jumps. Sets the velocity on Enter().
/// </summary>
[GlobalClass]
public partial class JumpingState() : MovementState(false,true,false)
{
    public override void Enter()
    {
        base.Enter();

        if (IsMultiplayerAuthority()) Npc.Velocity = new Vector3(Npc.Velocity.X, Npc.JumpPower, Npc.Velocity.Z);
    }

    public override void Process(double delta)
    {
        base.Process(delta);
        Machine.SwitchToState(FreeFall);
    }
}