using System;
using System.Collections.Generic;
using System.Linq;
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

    public class TransitionDecision
    {
        private bool decision = false;

        public void Decide(bool decision)
        {
            this.decision = decision;
        }

        public bool GetDecision()
        {
            return decision;
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
                List<(int, string)> candidateStates = new();
                int probabilitySum = 0;
                int randomResult = 0;

                foreach (var destinationState in destinationStates)
                {
                    if (destinationState.IsPossibleNextState())
                    {
                        candidateStates.Add((destinationState.GetProbabilityCoefficient(), destinationState.GetName()));
                    }
                }

                probabilitySum = candidateStates.Select(i => i.Item1).Sum();
                randomResult = UnityEngine.Random.Range(0, probabilitySum);

                candidateStates.Sort((i1, i2) => i2.Item1.CompareTo(i1.Item1));
                foreach ((var probabilityCoefficient, var stateName) in candidateStates)
                {
                    if (randomResult < probabilityCoefficient)
                    {
                        nextStateName = stateName;
                        return true;
                    }
                    else
                    {
                        randomResult -= probabilityCoefficient;
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

                [SerializeField, Min(0)]
                private int probabilityCoefficient;

                [SerializeField]
                private UnityEvent<TransitionDecision> condition;

                private readonly TransitionDecision transitionDecision = new();

                public string GetName()
                {
                    return state;
                }

                public bool IsPossibleNextState()
                {
                    condition.Invoke(transitionDecision);
                    return transitionDecision.GetDecision();
                }

                public int GetProbabilityCoefficient()
                {
                    return probabilityCoefficient;
                }
            }
        }
    }
}
