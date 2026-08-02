using Godot;

namespace FaF.Game.Rig.States.Movement;

/// <summary>
/// FreeFall state with walking & jumping disabled. Active when the NPC is in air.
/// </summary>
[GlobalClass]
public partial class FreeFallState() : MovementState(false, true, false, true, "FALL_ANIMATION", "FALL_ANIMATION")
{
    public override void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);

        if (Npc.IsOnFloor())
        {
            Machine.SwitchToState(Landed);
        }
    }
}