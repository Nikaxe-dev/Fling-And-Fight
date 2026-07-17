using Godot;

namespace FaF.Rig.States;

[GlobalClass]
public partial class UpState(bool ApplyGravity) : State
{
    public override void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);

        if (ApplyGravity && IsMultiplayerAuthority())
        {
            if (!Npc.IsOnFloor())
            {
                Npc.Velocity += Npc.GetGravity() * (float)delta;
            }
        }

        Npc.MoveAndSlide();
    }
}