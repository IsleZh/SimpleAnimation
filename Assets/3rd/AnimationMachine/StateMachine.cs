using MyPlayable;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Isle.AnimationMachine
{
    public class StateMachine
    {
        #region PlayableModel

        private PlayableGraph m_PlayableGraph;

        #endregion

        private PlayableAnimator m_PlayableAnimator;

        private State[] m_States;
        private StateMachine[] m_StateMachines;

        public State[] states => m_States;

        public StateMachine[] stateMachines => m_StateMachines;

        public State currentState, nextState;
        public ScriptPlayable<TransitionPlayable> currentTransition;

        public void Initialize(PlayableAnimator playableAnimator)
        {
            m_PlayableAnimator = playableAnimator;
            m_PlayableGraph = PlayableGraph.Create();
        }

        public void Update(float deltaTime)
        {
            //1

            //如果transition运行完成(结束)。
            {
                currentState = nextState;
                nextState = null;
                //currentTransition = null;
            }
        }

        public void Goto(StateTransition transition)
        {
            nextState = transition.to;

            currentTransition = ScriptPlayable<TransitionPlayable>.Create(m_PlayableGraph);

            var transitionBehaviour = currentTransition.GetBehaviour();

            transitionBehaviour.Initialize(transition, currentTransition, m_PlayableGraph);
            var playableOutput = AnimationPlayableOutput.Create(m_PlayableGraph, "Animation", m_PlayableAnimator.animator);

            playableOutput.SetSourcePlayable(currentTransition);
            playableOutput.SetSourceInputPort(0);
        }
    }

    public class TransitionHandler
    {
        public StateTransition transition;
    }
}