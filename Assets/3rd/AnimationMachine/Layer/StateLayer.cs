﻿using System;
using FUnit;
using UnityEngine;
using UnityEngine.Playables;
#if UNITY_EDITOR
using UnityEditor;

#endif
namespace Isle.AnimationMachine
{
    [Serializable][CreateAssetMenu(fileName = "StateLayer", menuName = "PlayableAnimation/StateLayer")]
    public class StateLayer : ScriptableObject
    {
        [HideInInspector]public string guid;
        [SerializeField]private StateMachine m_StateMachine;
        [SerializeField]private PlayableAnimatorController m_PlayableAnimatorController;
        private AvatarMask m_AvatarMask;
        private LayerBlendingMode m_BlendingMode;
        private int m_SyncedLayerIndex = -1;
        private bool m_IKPass;
        private float m_DefaultWeight;
        private bool m_SyncedLayerAffectsTiming;

        public PlayableAnimatorController controller
        {
            get => m_PlayableAnimatorController;
            set => m_PlayableAnimatorController = value;
        }

        /// <summary>
        ///   <para>The state machine for the layer.</para>
        /// </summary>
        public StateMachine stateMachine
        {
            get => this.m_StateMachine;
            set => this.m_StateMachine = value;
        }

        /// <summary>
        ///   <para>The AvatarMask that is used to mask the animation on the given layer.</para>
        /// </summary>
        public AvatarMask avatarMask
        {
            get => this.m_AvatarMask;
            set => this.m_AvatarMask = value;
        }

        /// <summary>
        ///   <para>The blending mode used by the layer. It is not taken into account for the first layer.</para>
        /// </summary>
        public LayerBlendingMode blendingMode
        {
            get => this.m_BlendingMode;
            set => this.m_BlendingMode = value;
        }

        /// <summary>
        ///   <para>Specifies the index of the Synced Layer.</para>
        /// </summary>
        public int syncedLayerIndex
        {
            get => this.m_SyncedLayerIndex;
            set => this.m_SyncedLayerIndex = value;
        }

        /// <summary>
        ///   <para>When active, the layer will have an IK pass when evaluated. It will trigger an OnAnimatorIK callback.</para>
        /// </summary>
        public bool iKPass
        {
            get => this.m_IKPass;
            set => this.m_IKPass = value;
        }

        /// <summary>
        ///   <para>The default blending weight that the layers has. It is not taken into account for the first layer.</para>
        /// 图层具有的默认混合权重。第一层不考虑
        /// </summary>
        public float defaultWeight
        {
            get => this.m_DefaultWeight;
            set => this.m_DefaultWeight = value;
        }

        /// <summary>
        ///   <para>When active, the layer will take control of the duration of the Synced Layer.</para>
        /// </summary>
        public bool syncedLayerAffectsTiming
        {
            get => this.m_SyncedLayerAffectsTiming;
            set => this.m_SyncedLayerAffectsTiming = value;
        }

        public void Initialize(PlayableAnimator controller,PlayableGraph graph)
        {
            stateMachine.Initialize(controller,graph,this);
            stateMachine.Start();
        }
        
        public void Update()
        {
            if (m_StateMachine!=null)
            {
                Debug.Log("m_StateMachine != null");
                m_StateMachine.Update();
            }
        }
#if UNITY_EDITOR
        [ContextMenu("CreateMatchine")]
        public StateMachine CreateMatchine()
        {
            StateMachine stateMachine = ScriptableObject.CreateInstance(typeof(StateMachine)) as StateMachine;
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
            return stateMachine;
        }
#endif
    }
}