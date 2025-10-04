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
        bool canChange = stateMatrix.EvaluateNextState(state, out string nextState);
        if (canChange)
        {
            state = nextState;
        }
    }

    [Serializable]
    private class StateMatrix
    {
        [SerializeField]
        private StateRow[] stateRows;

        public bool EvaluateNextState(string currentState, out string nextState)
        {

            foreach (var stateRow in stateRows)
            {
                if (stateRow.IsSourceState(currentState))
                {
                    return stateRow.EvaluateNextState(out nextState);
                }
            }

            nextState = "";
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
            private UnityEvent onChangeToThis;

            public bool IsSourceState(string state)
            {
                return state == sourceState;
            }

            public bool EvaluateNextState(out string nextState)
            {
                foreach (var destinationState in destinationStates)
                {
                    if (destinationState.IsTheNextState())
                    {
                        nextState = destinationState.GetState();
                        return true;
                    }
                }

                nextState = "";
                return false;
            }

            [Serializable]
            private class DestinationState
            {
                [SerializeField]
                private string state;

                [SerializeField]
                private UnityEvent<bool[]> condition;

                private readonly bool[] conditionResult = new bool[1];

                public string GetState()
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
