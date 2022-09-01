using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Isle.AnimationMachine
{
    [RequireComponent(typeof(Animator))]
    public class PlayableAnimator : MonoBehaviour
    {
        public Animator animator;
        public PlayableAnimatorController controller;
        public PlayableGraph m_PlayableGraph;
        public AnimationPlayableOutput m_AnimationPlayableOutput;

        #region Test

        public AnimationClip clip;
        public AnimationClip transitionClip;


        #endregion

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Start()
        {
            m_PlayableGraph = PlayableGraph.Create();
        }

        [ContextMenu("TestInitAnimation")]
        public void Test()
        {
            m_PlayableGraph = PlayableGraph.Create();
            m_AnimationPlayableOutput = AnimationPlayableOutput.Create(m_PlayableGraph, "TestAnimation", animator);
            var animation = new Animation();
            animation.clip = this.clip;
            var state = ScriptableObject.CreateInstance<State>();
            state.motion = animation;
            var stateMachine = ScriptableObject.CreateInstance<StateMachine>();
            stateMachine.defaultState = state;
            state.Initialize(stateMachine);
            var layer = ScriptableObject.CreateInstance<StateLayer>();
            layer.stateMachine = stateMachine;
            stateMachine.Initialize(this,m_PlayableGraph, layer);
            stateMachine.Start();
            controller = ScriptableObject.CreateInstance<PlayableAnimatorController>();
            controller.layers = new List<StateLayer>();
            controller.layers.Add(layer);

        }

        [ContextMenu("TestSwitchAnimation")]
        public void Test2()
        {
            m_PlayableGraph = PlayableGraph.Create();
            m_AnimationPlayableOutput = AnimationPlayableOutput.Create(m_PlayableGraph, "TestAnimation", animator);
            var animation = new Animation();
            animation.clip = this.clip;
            
            var animation2 = new Animation();
            animation2.clip = this.transitionClip;
            
            var state = ScriptableObject.CreateInstance<State>();
            state.motion = animation;
            var state2 = ScriptableObject.CreateInstance<State>();
            state2.motion = animation2;

            var transition = new StateTransition {transitionDuration = 5f, exitTime = 2f, from = state, to = state2};

            state.transitions = new List<StateTransition> {transition};

            var stateMachine = ScriptableObject.CreateInstance<StateMachine>();
            stateMachine.defaultState = state;
            
            state.Initialize(stateMachine);
            state2.Initialize(stateMachine);
            
            var layer = ScriptableObject.CreateInstance<StateLayer>();
            layer.stateMachine = stateMachine;
            stateMachine.Initialize(this,m_PlayableGraph, layer);
            stateMachine.Start();
            controller = ScriptableObject.CreateInstance<PlayableAnimatorController>();
            controller.layers = new List<StateLayer>();
            controller.layers.Add(layer);
        }

        private void Update()
        {
            if (controller != null)
            {
                Debug.Log("controller != null");
                controller.Update();
            }
        }
        //被编辑器警告了，加上这个；
        private void OnDestroy()
        {
            m_PlayableGraph.Destroy();
        }
    }
}