using Godot;

namespace FaF.Game.Rig.States;

[GlobalClass]
public partial class UpState(bool ApplyGravity) : State
{
    public override void PhysicsProcess(double delta)
    {
        Npc.MoveAndSlide();

        base.PhysicsProcess(delta);

        if (ApplyGravity && IsMultiplayerAuthority())
        {
            if (!Npc.IsOnFloor())
            {
                Npc.Velocity += Npc.GetGravity() * (float)delta;
            }
        }
    }
}