using Godot;

namespace FaF.Game.Rig.States;

/// <summary>
/// A state that locks the physics down to no interaction with rotation.
/// </summary>
/// <param name="ApplyGravity"></param>
[GlobalClass]
public partial class UpState : PhysicsState
{
    public override void Enter()
    {
        base.Enter();
        
        Npc.AxisLockAngularX = true;
        Npc.AxisLockAngularY = true;
        Npc.AxisLockAngularZ = true;
    }
}