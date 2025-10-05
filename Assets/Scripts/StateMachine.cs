using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class StateMachine
{
    [SerializeField]
    private string initialState;

    [SerializeField]
    private StateMatrix stateMatrix;

    private string state;

    public void Initialize()
    {
        state = initialState;
    }

    public void NextState()
    {
        bool canTransition = stateMatrix.EvaluateNextState(state, out SourceState nextState);
        if (canTransition)
        {
            nextState.SetOutsideState(ref state);
        }
    }

    private readonly struct SourceState
    {
        public static SourceState Empty => empty;

        private readonly string stateName;

        private readonly UnityEvent onTransition;

        private static readonly SourceState empty = new("", null);

        public SourceState(string stateName, UnityEvent onTransition)
        {
            this.stateName = stateName;
            this.onTransition = onTransition;
        }

        public readonly bool IsStateName(string stateName)
        {
            return this.stateName == stateName;
        }

        public readonly void SetOutsideState(ref string state)
        {
            state = stateName;
            onTransition.Invoke();
        }
    }

    [Serializable]
    private class StateMatrix
    {
        [SerializeField]
        private StateRow[] stateRows;

        public bool EvaluateNextState(string currentState, out SourceState nextState)
        {
            string nextStateName = "";

            foreach (var stateRow in stateRows)
            {
                if (stateRow.IsSourceState(currentState))
                {
                    stateRow.EvaluateNextState(out nextStateName);
                    break;
                }
            }

            foreach (var stateRow in stateRows)
            {
                if (stateRow.IsSourceState(nextStateName))
                {
                    nextState = new SourceState(nextStateName, stateRow.GetOnTransition());
                    return true;
                }
            }

            nextState = SourceState.Empty;
            return false;
        }

        [Serializable]
        private class StateRow
        {
            [SerializeField]
            private string sourceState;

            [SerializeField]
            private DestinationState[] destinationStates;

            [SerializeField]
            private UnityEvent onTransition;

            public bool IsSourceState(string state)
            {
                return state == sourceState;
            }

            public bool EvaluateNextState(out string nextStateName)
            {
                foreach (var destinationState in destinationStates)
                {
                    if (destinationState.IsTheNextState())
                    {
                        nextStateName = destinationState.GetName();
                        return true;
                    }
                }

                nextStateName = "";
                return false;
            }

            public UnityEvent GetOnTransition()
            {
                return onTransition;
            }

            [Serializable]
            private class DestinationState
            {
                [SerializeField]
                private string state;

                [SerializeField]
                private UnityEvent<bool[]> condition;

                private readonly bool[] conditionResult = new bool[1];

                public string GetName()
                {
                    return state;
                }

                public bool IsTheNextState()
                {
                    condition.Invoke(conditionResult);
                    return conditionResult[0];
                }
            }
        }
    }
}
