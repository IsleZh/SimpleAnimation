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
        private PlayableGraph m_PlayableGraph;
        public AnimationPlayableOutput m_AnimationPlayableOutput;

        #region Test

        public AnimationClip clip;
        public AnimationClip transitionClip;


        #endregion

        public PlayableGraph playableGraph
        {
            get => m_PlayableGraph;
            set => m_PlayableGraph = value;
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Start()
        {
            m_PlayableGraph = PlayableGraph.Create(controller.name+"_"+this.gameObject.name);
            m_AnimationPlayableOutput = AnimationPlayableOutput.Create(m_PlayableGraph, "TestAnimation", animator);

            //controller = Instantiate(controller);
            controller.Initialize(this,m_PlayableGraph);
        }

        [ContextMenu("TestInitAnimation")]
        public void Test()
        {
            m_PlayableGraph = PlayableGraph.Create();
            m_AnimationPlayableOutput = AnimationPlayableOutput.Create(m_PlayableGraph, "TestAnimation", animator);
            var animation = new PlayableAnimationClip();
            animation.clip = this.clip;
            var state = ScriptableObject.CreateInstance<State>();
            state.motion = animation;
            var stateMachine = ScriptableObject.CreateInstance<StateMachine>();
            stateMachine.defaultState = state;
            state.Initialize(stateMachine);
            var layer = ScriptableObject.CreateInstance<StateLayer>();
            layer.stateMachine = stateMachine;
            controller = ScriptableObject.CreateInstance<PlayableAnimatorController>();
            stateMachine.Initialize(layer,controller);
            stateMachine.Start();
            controller.layers = new List<StateLayer>();
            controller.layers.Add(layer);

        }

        [ContextMenu("TestSwitchAnimation")]
        public void Test2()
        {
            m_PlayableGraph = PlayableGraph.Create();
            m_AnimationPlayableOutput = AnimationPlayableOutput.Create(m_PlayableGraph, "TestAnimation", animator);
            var animation = new PlayableAnimationClip();
            animation.clip = this.clip;
            
            var animation2 = new PlayableAnimationClip();
            animation2.clip = this.transitionClip;
            
            var state1 = ScriptableObject.CreateInstance<State>();
            state1.motion = animation;
            var state2 = ScriptableObject.CreateInstance<State>();
            state2.motion = animation2;

            var transition = new StateTransition {duration = 2f, exitTime = 2f, from = state1, to = state2};
            
            state1.transitions = new List<StateTransition> {transition};
            
            
            //返回 的Transition
            var transition2 = new StateTransition {duration = 2f, exitTime = 2f, from = state2, to = state1};
            state2.transitions = new List<StateTransition> {transition2};
            

            var stateMachine = ScriptableObject.CreateInstance<StateMachine>();
            stateMachine.defaultState = state1;
            
            state1.Initialize(stateMachine);
            state2.Initialize(stateMachine);
            
            var layer = ScriptableObject.CreateInstance<StateLayer>();
            layer.stateMachine = stateMachine;
            controller = ScriptableObject.CreateInstance<PlayableAnimatorController>();
            stateMachine.Initialize(layer,controller);
            stateMachine.Start();
            
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