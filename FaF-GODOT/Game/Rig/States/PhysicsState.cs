using Godot;

namespace FaF.Game.Rig.States;

/// <summary>
/// The base state for all physics related states.
/// </summary>
[GlobalClass]
public partial class PhysicsState : State
{
    public override void Enter()
    {
        base.Enter();

        Npc.AxisLockAngularX = false;
        Npc.AxisLockAngularY = false;
        Npc.AxisLockAngularZ = false;
    }
}