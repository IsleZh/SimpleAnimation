using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Isle.AnimationMachine
{
    public class State :Node
    {
        /// <summary>
        ///   <para>The hashed name of the state.</para>
        /// </summary>
        public int nameHash { get; }

        /// <summary>
        ///   <para>The motion assigned to this state.</para>
        /// </summary>
        public Motion motion;

        /// <summary>
        ///   <para>The transitions that are going out of the state.</para>
        /// </summary>
        [SerializeField] public List<StateTransition> transitions;
        private float timer;

        public StateMachine stateMachine;

        /// <summary>
        /// 应在状态机某一时刻初始化
        /// </summary>
        /// <param name="stateMachine"></param>
        public void Initialize(StateMachine stateMachine)
        {
            //TODO 不知道这样初始化正确不正确
            //transitions = new List<StateTransition>();
            
            this.stateMachine = stateMachine;
            //#TODO 在这里准备按需加载motion 
            motion.LoadAsset();
            
            //较小的exitTime在前是为了方便判断
            transitions?.Sort((x1, x2) => x1.exitTime < x2.exitTime ? 1 : -1);
        }

        public void DoUpdate(float deltaTime)
        {
            OnUpdate(deltaTime);
        }
        public void OnEnter()
        {
            timer = 0;
        }

        public bool OnUpdate(float deltaTime)
        {
            var preTime = timer;
            timer+=deltaTime;
            if (transitions==null||transitions?.Count==0)
            {
                return false;
            }
            //Debug.Log("State::transitions不为空 开始判断切换条件"+"timer ="+timer);
            if (stateMachine.currentTransition==null)
            {
                foreach (var transition in transitions)
                {
                    if (timer>transition.exitTime*transition.from.motion.GetLength())
                    {
                        Debug.Log("切换动画判断通过，Timer:"+timer +" Duration:"+transition.exitTime*transition.from.motion.GetLength());
                        if (transition.TryTransition())
                        {
                            stateMachine.Goto(transition);
                            return true; 
                        }
                    }
                }
            }

            return false;
        }

        public void OnExit()
        {
            
        }

        public void Reset()
        {
            
        }
#if UNITY_EDITOR
        [ContextMenu("CreateTransition")]
        public StateTransition CreateTransition()
        {
            StateTransition stateTransition = ScriptableObject.CreateInstance(typeof(StateTransition)) as StateTransition;
            stateTransition.name = "StateTransition";
            stateTransition.guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "CreateStateMachine");
            if (this.transitions==null)
            {
                this.transitions = new List<StateTransition>();
            }
            this.transitions.Add(stateTransition);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(stateTransition, this);
            }

            Undo.RegisterCreatedObjectUndo(stateTransition, "CreateTransition");

            AssetDatabase.SaveAssets();
            return stateTransition;
        }
#endif
#if UNITY_EDITOR
        [ContextMenu("CreateAnimation")]
        public Animation CreateAnimation()
        {
            Animation animation = ScriptableObject.CreateInstance(typeof(Animation)) as Animation;
            animation.name = "Animation";
            animation.guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "CreateAnimation");
            this.motion = animation;

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(animation, this);
            }

            Undo.RegisterCreatedObjectUndo(animation, "CreateAnimation");

            AssetDatabase.SaveAssets();
            return animation;
        }
#endif
#if UNITY_EDITOR
        [ContextMenu("CreateBlendTree")]
        public StateMachine CreateBlendTree()
        {
            //TODO CreateBlendTree
            /*StateMachine stateMachine = ScriptableObject.CreateInstance(typeof(StateMachine)) as StateMachine;
            stateMachine.name = "StateMachine";
            stateMachine.guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "CreateStateMachine");
            this.stateMachine = stateMachine;

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(stateMachine, this);
            }

            Undo.RegisterCreatedObjectUndo(stateMachine, "CreateStateMachine");

            AssetDatabase.SaveAssets();
            return stateMachine;*/
            return null;
        }
#endif
        #region Todo
        /// <summary>
        ///   <para>The default speed of the motion.</para>
        /// </summary>
        public float speed { get; set; }

        /// <summary>
        ///         <para>Offset at which the animation loop starts. Useful for synchronizing looped animations.
        /// Units is normalized time.</para>
        ///       </summary>
        public float cycleOffset { get; set; }

        /// <summary>
        ///   <para>Should the state be mirrored.</para>
        /// </summary>
        public bool mirror { get; set; }

        /// <summary>
        ///   <para>Should Foot IK be respected for this state.</para>
        /// </summary>
        public bool iKOnFeet { get; set; }

        /// <summary>
        ///   <para>Whether or not the States writes back the default values for properties that are not animated by its Motion.</para>
        /// </summary>
        public bool writeDefaultValues { get; set; }

        /// <summary>
        ///   <para>A tag can be used to identify a state.</para>
        /// </summary>
        public string tag { get; set; }

        /// <summary>
        ///   <para>The animator controller parameter that drives the speed value.</para>
        /// </summary>
        public string speedParameter { get; set; }

        /// <summary>
        ///   <para>The animator controller parameter that drives the cycle offset value.</para>
        /// </summary>
        public string cycleOffsetParameter { get; set; }

        /// <summary>
        ///   <para>The animator controller parameter that drives the mirror value.</para>
        /// </summary>
        public string mirrorParameter { get; set; }

        /// <summary>
        ///   <para>If timeParameterActive is true, the value of this Parameter will be used instead of normalized time.</para>
        /// </summary>
        public string timeParameter { get; set; }

        /// <summary>
        ///   <para>Define if the speed value is driven by an Animator controller parameter or by the value set in the editor.</para>
        /// </summary>
        public bool speedParameterActive { get; set; }

        /// <summary>
        ///   <para>Define if the cycle offset value is driven by an Animator controller parameter or by the value set in the editor.</para>
        /// </summary>
        public bool cycleOffsetParameterActive { get; set; }

        /// <summary>
        ///   <para>Define if the mirror value is driven by an Animator controller parameter or by the value set in the editor.</para>
        /// </summary>
        public bool mirrorParameterActive { get; set; }

        /// <summary>
        ///   <para>If true, use value of given Parameter as normalized time.</para>
        /// </summary>
        public bool timeParameterActive { get; set; }

        #endregion



        /*/// <summary>
        ///   <para>Utility function to add an outgoing transition.</para>
        /// </summary>
        /// <param name="transition">The transition to add.</param>
        public void AddTransition(StateTransition transition)
        {
            this.undoHandler.DoUndo((UnityEngine.Object) this, "Transition added");
            StateTransition[] transitions = this.transitions;
            ArrayUtility.Add<StateTransition>(ref transitions, transition);
            this.transitions = transitions;
        }

        /// <summary>
        ///   <para>Utility function to remove a transition from the state.</para>
        /// </summary>
        /// <param name="transition">Transition to remove.</param>
        public void RemoveTransition(StateTransition transition)
        {
            this.undoHandler.DoUndo((UnityEngine.Object) this, "Transition removed");
            StateTransition[] transitions = this.transitions;
            ArrayUtility.Remove<StateTransition>(ref transitions, transition);
            this.transitions = transitions;
            if (!MecanimUtilities.AreSameAsset((UnityEngine.Object) this, (UnityEngine.Object) transition))
                return;
            Undo.DestroyObjectImmediate((UnityEngine.Object) transition);
        }

        private StateTransition CreateTransition(bool setDefaultExitTime)
        {
            StateTransition newTransition = new StateTransition();
            newTransition.hasExitTime = false;
            newTransition.hasFixedDuration = true;
            if (AssetDatabase.GetAssetPath((UnityEngine.Object) this) != "")
                AssetDatabase.AddObjectToAsset((UnityEngine.Object) newTransition,
                    AssetDatabase.GetAssetPath((UnityEngine.Object) this));
            newTransition.hideFlags = HideFlags.HideInHierarchy;
            this.undoHandler.DoUndoCreated((UnityEngine.Object) newTransition, "Transition Created");
            if (setDefaultExitTime)
                this.SetDefaultTransitionExitTime(ref newTransition);
            return newTransition;
        }

        private void SetDefaultTransitionExitTime(ref StateTransition newTransition)
        {
            newTransition.hasExitTime = true;
            if ((UnityEngine.Object) this.motion != (UnityEngine.Object) null &&
                (double) this.motion.averageDuration > 0.0)
            {
                float num = 0.25f / this.motion.averageDuration;
                newTransition.duration = 0.25f;
                newTransition.exitTime = 1f - num;
            }
            else
            {
                newTransition.duration = 0.25f;
                newTransition.exitTime = 0.75f;
            }
        }

        /// <summary>
        ///   <para>Utility function to add an outgoing transition to the destination state.</para>
        /// </summary>
        /// <param name="defaultExitTime">If true, the exit time will be the equivalent of 0.25 second.</param>
        /// <param name="destinationState">The destination state.</param>
        public StateTransition AddTransition(
            State destinationState)
        {
            StateTransition transition = this.CreateTransition(false);
            transition.destinationState = destinationState;
            this.AddTransition(transition);
            return transition;
        }

        /// <summary>
        ///   <para>Utility function to add an outgoing transition to the destination state machine.</para>
        /// </summary>
        /// <param name="defaultExitTime">If true, the exit time will be the equivalent of 0.25 second.</param>
        /// <param name="destinationStateMachine">The destination state machine.</param>
        public StateTransition AddTransition(
            StateMachine destinationStateMachine)
        {
            StateTransition transition = this.CreateTransition(false);
            transition.destinationStateMachine = destinationStateMachine;
            this.AddTransition(transition);
            return transition;
        }

        /// <summary>
        ///   <para>Utility function to add an outgoing transition to the destination state.</para>
        /// </summary>
        /// <param name="defaultExitTime">If true, the exit time will be the equivalent of 0.25 second.</param>
        /// <param name="destinationState">The destination state.</param>
        public StateTransition AddTransition(
            State destinationState,
            bool defaultExitTime)
        {
            StateTransition transition = this.CreateTransition(defaultExitTime);
            transition.destinationState = destinationState;
            this.AddTransition(transition);
            return transition;
        }

        /// <summary>
        ///   <para>Utility function to add an outgoing transition to the destination state machine.</para>
        /// </summary>
        /// <param name="defaultExitTime">If true, the exit time will be the equivalent of 0.25 second.</param>
        /// <param name="destinationStateMachine">The destination state machine.</param>
        public StateTransition AddTransition(
            StateMachine destinationStateMachine,
            bool defaultExitTime)
        {
            StateTransition transition = this.CreateTransition(defaultExitTime);
            transition.destinationStateMachine = destinationStateMachine;
            this.AddTransition(transition);
            return transition;
        }

        /// <summary>
        ///   <para>Utility function to add an outgoing transition to the exit of the state's parent state machine.</para>
        /// </summary>
        /// <param name="defaultExitTime">If true, the exit time will be the equivalent of 0.25 second.</param>
        /// <returns>
        ///   <para>The Animations.StateTransition that was added.</para>
        /// </returns>
        public StateTransition AddExitTransition() => this.AddExitTransition(false);

        /// <summary>
        ///   <para>Utility function to add an outgoing transition to the exit of the state's parent state machine.</para>
        /// </summary>
        /// <param name="defaultExitTime">If true, the exit time will be the equivalent of 0.25 second.</param>
        /// <returns>
        ///   <para>The Animations.StateTransition that was added.</para>
        /// </returns>
        public StateTransition AddExitTransition(bool defaultExitTime)
        {
            StateTransition transition = this.CreateTransition(defaultExitTime);
            transition.isExit = true;
            this.AddTransition(transition);
            return transition;
        }

        internal StateMachine FindParent(StateMachine root) => root.HasState(this, false)
            ? root
            : root.stateMachinesRecursive
                .Find((Predicate<ChildStateMachine>) (sm => sm.stateMachine.HasState(this, false))).stateMachine;

        internal StateTransition FindTransition(
            State destinationState)
        {
            return new List<StateTransition>((IEnumerable<StateTransition>) this.transitions).Find(
                (Predicate<StateTransition>) (t =>
                    (UnityEngine.Object) t.destinationState == (UnityEngine.Object) destinationState));
        }*/
    }
}
