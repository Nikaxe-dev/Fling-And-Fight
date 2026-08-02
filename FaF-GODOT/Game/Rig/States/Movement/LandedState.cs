using Godot;

namespace FaF.Game.Rig.States.Movement;

/// <summary>
/// Activated for a quick frame when the NPC lands after being in FreeFall. Immediately switches to Grounded.
/// </summary>
[GlobalClass]
public partial class LandedState : MovementState
{
    public override void Process(double delta)
    {
        base.Process(delta);

        // this state is only meant to be a notifier to other scripts for when the player lands
        // SO: it only runs for less than a frame
        Machine.SwitchToState(Grounded);
    }
}