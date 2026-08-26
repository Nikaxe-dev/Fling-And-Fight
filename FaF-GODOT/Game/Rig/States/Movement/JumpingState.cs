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

        Npc.SlopeFixEnabled = false;

        if (IsMultiplayerAuthority()) Npc.SetAxisVelocity(Vector3.Up*Npc.JumpPower);
    }

    public override void Process(double delta)
    {
        base.Process(delta);
        Machine.SwitchToState(FreeFall);
    }
}