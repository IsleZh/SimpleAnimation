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
            m_AnimationPlayableOutput = AnimationPlayableOutput.Create(m_PlayableGraph, "TestAnimation", animator);
            var animation = new Animation();
            animation.clip = this.clip;
            var state = ScriptableObject.CreateInstance<State>();
            state.motion = animation;
            var stateMachine = ScriptableObject.CreateInstance<StateMachine>();
            stateMachine.defaultState = state;
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
            var animation = new Animation();
            animation.clip = this.clip;
            var state = new State();
            state.motion = animation;
            controller = new PlayableAnimatorController();
            var layer = new StateLayer();
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
    }
}