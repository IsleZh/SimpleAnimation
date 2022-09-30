using System.Collections.Generic;
using UnityEngine;

namespace Isle.AnimationMachine
{
    public class ChildStateMachine : Node
    {
        [SerializeField] private List<State> m_States;
        [SerializeField] private List<ChildStateMachine> m_StateMachines;

        public List<State> states
        {
            get => m_States;
            set => m_States = value;
        }
        public List<ChildStateMachine> stateMachines
        {
            get => m_StateMachines;
            set => m_StateMachines = value;
        }

        public State defaultState;
        public override State GetState()
        {
            return defaultState;
        }

        public void Initialize(StateMachine stateMachine)
        {
            foreach (var state in m_States)
            {
                state.Initialize(stateMachine);
            }
        }
    }
}