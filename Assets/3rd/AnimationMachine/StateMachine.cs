using System;
using System.Collections.Generic;
using MyPlayable;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Isle.AnimationMachine
{
    [Serializable]
    [CreateAssetMenu(fileName = "StateMachine", menuName = "PlayableAnimation/StateMachine")]
    public class StateMachine : ScriptableObject
    {
        [HideInInspector] public string guid;

        #region PlayableModel

        private PlayableGraph m_PlayableGraph;
        private PlayableAnimator m_PlayableAnimator;
        private StateLayer m_StateLayer;

        public PlayableGraph playableGraph => m_PlayableGraph;

        public PlayableAnimator playableAnimator => m_PlayableAnimator;

        public StateLayer stateLayer => m_StateLayer;

        #endregion


        [SerializeField] private List<State> m_States;
        [SerializeField] private List<StateMachine> m_StateMachines;

        public List<State> states => m_States;
        public List<StateMachine> stateMachines => m_StateMachines;
        public State defaultState, currentState, nextState;
        public ScriptPlayable<TransitionPlayable> TransitionPlayable;
        public StateTransition currentTransition;

        private bool isStarted = false;

        public void Initialize(PlayableAnimator playableAnimator, PlayableGraph graph, StateLayer stateLayer)
        {
            m_PlayableAnimator = playableAnimator;
            m_StateLayer = stateLayer;
            m_PlayableGraph = graph;
            TransitionPlayable = ScriptPlayable<TransitionPlayable>.Create(m_PlayableGraph);
            m_PlayableAnimator.m_AnimationPlayableOutput.SetSourcePlayable(TransitionPlayable, 0);

            foreach (var state in m_States)
            {
                state.Initialize(this);
            }
        }

        public void Start()
        {
            var transitionPlayableBehaviour = TransitionPlayable.GetBehaviour();

            currentState = defaultState;
            transitionPlayableBehaviour.Init(currentState.motion, TransitionPlayable, this);
            isStarted = true;
            m_PlayableGraph.Play();
        }

        public void Update()
        {
            //有可能要同时处理两个Update
            currentState.DoUpdate(Time.deltaTime);
            //nextState.DoUpdate(Time.deltaTime);
        }

        /// <summary>
        /// 如果transition运行完成(结束)。
        /// 则由TransitionBehaviour直接调用此方法
        /// </summary>
        public void FinishedGoto()
        {
            currentState.OnExit();
            nextState.OnEnter();
            currentState = nextState;
            nextState = null;
            currentTransition = null;
        }

        public void Goto(StateTransition transition)
        {
            currentTransition = transition;
            nextState = transition.to;
            var transitionBehaviour = TransitionPlayable.GetBehaviour();
            transitionBehaviour.DoSwitch(transition, TransitionPlayable);
            m_PlayableAnimator.m_AnimationPlayableOutput.SetSourcePlayable(TransitionPlayable, 0);
        }
#if UNITY_EDITOR
        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <returns></returns>
        [ContextMenu("CreateStateMatchine")]
        public StateMachine CreateMatchine()
        {
            StateMachine stateMachine = ScriptableObject.CreateInstance(typeof(StateMachine)) as StateMachine;
            stateMachine.name = "ChildStateMachine";
            stateMachine.guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "CreateStateMachine");
            if (this.m_StateMachines==null)
            {
                this.m_StateMachines = new List<StateMachine>();
            }
            this.m_StateMachines.Add(stateMachine);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(stateMachine, this);
            }

            Undo.RegisterCreatedObjectUndo(stateMachine, "CreateStateMachine");

            AssetDatabase.SaveAssets();
            return stateMachine;
        }
        /// <summary>
        /// 创建状态
        /// </summary>
        /// <returns></returns>
        [ContextMenu("CreateState")]
        public State CreateState()
        {
            State state = ScriptableObject.CreateInstance(typeof(State)) as State;
            state.name = "State";
            state.guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "CreateState");
            if (this.m_States==null)
            {
                this.m_States = new List<State>();
            }
            this.m_States.Add(state);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(state, this);
            }

            Undo.RegisterCreatedObjectUndo(state, "CreateState");

            AssetDatabase.SaveAssets();
            return state;
        }
#endif
    }
#if UNITY_EDITOR
    /*public Node CreateNode(System.Type type)
    {
    Node node = ScriptableObject.CreateInstance(type) as Node;
    node.name = type.Name;
    node.guid = GUID.Generate().ToString();

    Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
    nodes.Add(node);

    if (!Application.isPlaying)
    {
        AssetDatabase.AddObjectToAsset(node, this);
    }

    Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");

    AssetDatabase.SaveAssets();
    return node;
    }

    public void DeleteNode(Node node)
    {
    Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");
    nodes.Remove(node);

    //AssetDatabase.RemoveObjectFromAsset(node);
    Undo.DestroyObjectImmediate(node);

    AssetDatabase.SaveAssets();
    }*/
#endif
}