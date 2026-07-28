using System.Linq;
using FaF.Debug;
using Godot;

namespace FaF.Rig.States;

#nullable enable

[GlobalClass]
public partial class StateMachine : Node
{
    private static readonly FaFLogger LOGGER = FaFLogger.Get("States/StateMachine");

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
        CurrentState?.Enter();
    }

    public override void _Process(double delta)
    {
        CurrentState?.Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        CurrentState?.PhysicsProcess(delta);
    }

    /// <summary>
    /// Switches to the given state (target).
    /// Target MUST be a DIRECT CHILD of this StateMachine Node!
    /// </summary>
    /// <param name="Target">The state to switch to.</param>
    public void SwitchToState(State? Target)
    {
        if (CurrentState?.GetType() == Target?.GetType()) return;

        var previous = CurrentState;
        if (Target != null) Target.PreviousState = CurrentState;

        CurrentState = Target;
        Target?.Enter();

        if(IsMultiplayerAuthority()) LOGGER.LOG(LogType.INFO, $"Switched to state {CurrentState?.Name} from {previous?.Name}", "StateChanged");
    }

    public State? GetCurrentState()
    {
        return CurrentState;
    }
}