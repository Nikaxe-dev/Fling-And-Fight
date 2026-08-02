using Godot;

namespace FaF.Game.Rig.States;

# nullable enable

/// <summary>
/// A state, used in the StateMachine node.
/// </summary>
[GlobalClass]
public abstract partial class State : Node
{
    // SET ON _Ready()
    
    /// <summary>
    /// The NPC connected to this state.
    /// </summary>
    public required NPC Npc;

    /// <summary>
    /// The StateMachine connected to this state.
    /// </summary>
    public required StateMachine Machine;

    public State? PreviousState;

    /// <summary>
    /// Called once when the state machine transitions to this state.
    /// </summary>
    public virtual void Enter() {}

    /// <summary>
    /// Called once when the state machine transitions away.
    /// </summary>
    public virtual void Exit() {}

    /// <summary>
    /// Called each frame when this state is active.
    /// <param name="delta">The frames delta, passed from the state machines process function.</param>
    /// </summary>
    public virtual void Process(double delta) {}

    /// <summary>
    /// Called each physics frame when this state is active.
    /// </summary>
    /// <param name="delta">The physics frames delta, passed from the state machines process function.</param>
    public virtual void PhysicsProcess(double delta) {}
}