using Godot;

namespace FaF.Rig.States.Movement;

[GlobalClass]
public partial class JumpingState() : MovementState(false,true,false)
{
    public override void Enter()
    {
        base.Enter();

        // FIXME: StateMachine gets stuck here when jumping instead of moving on to FreeFall immediately. POSSIBLE FIX: Add Machine.SwitchToState(FreeFall) to PhysicsProcess.

        if (IsMultiplayerAuthority()) Npc.Velocity = new Vector3(Npc.Velocity.X, Npc.JumpPower, Npc.Velocity.Z);
        Machine.SwitchToState(FreeFall);
    }
}