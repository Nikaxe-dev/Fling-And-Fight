using Godot;

namespace FaF.Game.Rig.States;

/// <summary>
/// The default UpState of an NPC, controlling the kinematic physics of it.
/// </summary>
/// <param name="ApplyGravity"></param>
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