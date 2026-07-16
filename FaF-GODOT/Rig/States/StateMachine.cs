using System.Linq;
using Godot;

namespace FaF.Rig.States;

#nullable enable

[GlobalClass]
public partial class StateMachine : Node
{
    [Export] public State? INITIAL_STATE;

    public required NPC Npc;

    private State? CurrentState;

    public override void _Ready()
    {
        Npc = GetParent<NPC>();

        foreach (State state in GetChildren().Cast<State>())
        {
            state.Machine = this;
            state.Npc = Npc;
        }

        CurrentState = INITIAL_STATE;
        CurrentState?._Enter();
    }

    public override void _Process(double delta)
    {
        CurrentState?._Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        CurrentState?._PhysicsProcess(delta);
    }

    /// <summary>
    /// Switches to the given state (target).
    /// Target MUST be a DIRECT CHILD of this StateMachine Node!
    /// </summary>
    /// <param name="Target">The state to switch to.</param>
    public void SwitchToState(State? Target)
    {
        if (CurrentState == Target) return;
        
        CurrentState?._Exit();
        CurrentState = Target;
        if (Target != null) Target.PreviousState = CurrentState;

    }

    public State? GetCurrentState()
    {
        return CurrentState;
    }
}