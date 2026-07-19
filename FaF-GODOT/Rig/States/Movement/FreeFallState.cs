using Godot;

namespace FaF.Rig.States.Movement;

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