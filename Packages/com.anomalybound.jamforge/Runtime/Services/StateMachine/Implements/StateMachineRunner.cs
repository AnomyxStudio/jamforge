using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

namespace JamForge.StateMachine
{
    public class Transition<TIndex, TState> where TState : IState
    {
        public TIndex To { get; }

        public Func<TState, bool> Condition { get; }

        public Transition(TIndex to, Func<TState, bool> condition)
        {
            To = to;
            Condition = condition;
        }
    }

    [Preserve]
    public class StateMachineRunner<TIndex, TState> : IDisposable, IStateMachineRunner<TIndex, TState>
        where TState : IState
    {
        public TIndex ActiveStateIndex { get; private set; }

        public TIndex PreviousStateIndex { get; private set; }

        public TState ActiveState => States[ActiveStateIndex];

        public TState PreviousState => PreviousStateIndex == null ? default : States[PreviousStateIndex];

        public Dictionary<TIndex, TState> States { get; } = new();

        public Dictionary<TIndex, List<Transition<TIndex, TState>>> Transitions { get; } = new();

        public event Action<TIndex, TState> StateEnterEvent;

        public event Action<TIndex, TState> StateExitEvent;

        public event Action<TIndex, TState, float> StateUpdateEvent;

        public event Action<TIndex, TState, TIndex, TState> StateTransitionEvent;

        public void Start(TIndex index)
        {
            if (States.TryGetValue(index, out var state))
            {
                ActiveStateIndex = index;

                state.Enter();
            }

            if (ActiveStateIndex == null) { throw new Exception("State machine has no initial state"); }
        }

        public void Run(float deltaTime)
        {
            if (ActiveStateIndex == null) { return; }

            if (Transitions.TryGetValue(ActiveStateIndex, out var transitions))
            {
                foreach (var transition in transitions.Where(transition => transition.Condition(ActiveState)))
                {
                    ChangeToState(transition.To);
                    return;
                }
            }

            ActiveState.Update(deltaTime);
            StateUpdateEvent?.Invoke(ActiveStateIndex, ActiveState, deltaTime);
        }

        public void AddState(TIndex index, TState state)
        {
            if (States.TryAdd(index, state)) { return; }

            Debug.LogError($"State machine already contains state with index {index}");
        }

        public void AddTransition(TIndex from, TIndex to, Func<TState, bool> condition)
        {
            if (!States.TryGetValue(from, out _))
            {
                Debug.LogError($"State machine does not contain state with index {from}");
                return;
            }

            if (!States.TryGetValue(to, out _))
            {
                Debug.LogError($"State machine does not contain state with index {to}");
                return;
            }

            if (!Transitions.TryGetValue(from, out var transitions))
            {
                transitions = new List<Transition<TIndex, TState>>();
                Transitions.Add(from, transitions);
            }

            transitions.Add(new Transition<TIndex, TState>(to, condition));
        }

        public void SwitchState(TIndex index)
        {
            if (ActiveStateIndex != null && ActiveStateIndex.Equals(index)) { return; }

            ChangeToState(index);
        }

        // State machine's state transition should controlled by the state machine runner itself
        private void ChangeToState(TIndex index)
        {
            PreviousStateIndex = ActiveStateIndex;

            PreviousState?.Exit();
            StateExitEvent?.Invoke(PreviousStateIndex, PreviousState);

            ActiveStateIndex = index;

            ActiveState.Enter();
            StateEnterEvent?.Invoke(ActiveStateIndex, ActiveState);

            StateTransitionEvent?.Invoke(PreviousStateIndex, PreviousState, ActiveStateIndex, ActiveState);
        }

        public virtual void Dispose() { }
    }
}
