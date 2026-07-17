using Godot;

namespace FaF.Rig.States.Movement;

[GlobalClass]
public partial class FreeFallState() : MovementState(false)
{
    public override void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);

        if (Npc.IsOnFloor())
        {
            Machine.SwitchToState(Grounded);
        }
    }
}