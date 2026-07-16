using Godot;

namespace FaF.Rig.States;

# nullable enable

[GlobalClass]
public abstract partial class State : Node
{
    // SET ON _Ready()
    public required NPC Npc;
    public required StateMachine Machine;

    public State? PreviousState;

    /// <summary>
    /// Called once when the state machine transitions to this state.
    /// </summary>
    public abstract void _Enter();

    /// <summary>
    /// Called once when the state machine transitions away.
    /// </summary>
    public abstract void _Exit();

    /// <summary>
    /// Called each frame when this state is active.
    /// <param name="delta">The frames delta, passed from the state machines process function.</param>
    /// </summary>
    public abstract override void _Process(double delta);

    /// <summary>
    /// Called each physics frame when this state is active.
    /// </summary>
    /// <param name="delta">The physics frames delta, passed from the state machines process function.</param>
    public abstract override void _PhysicsProcess(double delta);
}