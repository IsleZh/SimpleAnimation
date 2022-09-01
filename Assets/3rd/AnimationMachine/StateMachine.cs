using System;
using MyPlayable;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Isle.AnimationMachine
{
    [Serializable][CreateAssetMenu(fileName = "StateMachine", menuName = "PlayableAnimation/StateMachine")]
    public class StateMachine:ScriptableObject
    {
        [HideInInspector]public string guid;
        #region PlayableModel
        private PlayableGraph m_PlayableGraph;
        private PlayableAnimator m_PlayableAnimator;
        private StateLayer m_StateLayer;
        #endregion
        

        [SerializeField]private State[] m_States;
        [SerializeField]private StateMachine[] m_StateMachines;

        public State[] states => m_States;

        public StateMachine[] stateMachines => m_StateMachines;

        public State defaultState,currentState, nextState;
        public ScriptPlayable<TransitionPlayable> transitionPlayableScript;
        public StateTransition currentTransition;

        private bool isStarted = false;
        public void Initialize(PlayableAnimator playableAnimator,PlayableGraph graph,StateLayer stateLayer)
        {
            m_PlayableAnimator = playableAnimator;
            m_StateLayer = stateLayer;
            m_PlayableGraph = graph;
            transitionPlayableScript = ScriptPlayable<TransitionPlayable>.Create(m_PlayableGraph);
            m_PlayableAnimator.m_AnimationPlayableOutput.SetSourcePlayable(transitionPlayableScript,0);
        }

        public void Start()
        {
            var transitionBehaviour = transitionPlayableScript.GetBehaviour();
            
            //m_PlayableAnimator.m_AnimationPlayableOutput.SetSourcePlayable(transitionBehaviour,0);
            
            currentState = defaultState;
            Debug.Log("playable is :"+currentState.motion.playable);
            transitionBehaviour.Initialize(currentState.motion, transitionPlayableScript, m_PlayableGraph);
            isStarted = true;
            m_PlayableGraph.Play();
        }

        public void Update()
        {
            /*思路有些问题
            if (isStarted== false)
            {
                Start();
            }*/
            
            //有可能要同时处理两个Update
            currentState.DoUpdate(Time.deltaTime);
            //nextState.DoUpdate(Time.deltaTime);
            
            //如果transition运行完成(结束)。
            {
                /*currentState = nextState;
                nextState = null;*/
                //currentTransition = null;
            }
        }

        public void Goto(StateTransition transition)
        {
            
            nextState = transition.to;

            var transitionBehaviour = transitionPlayableScript.GetBehaviour();

            transitionBehaviour.Switch(transition, transitionPlayableScript, m_PlayableGraph);
            m_PlayableAnimator.m_AnimationPlayableOutput.SetSourcePlayable(transitionPlayableScript,0);
            
        }
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