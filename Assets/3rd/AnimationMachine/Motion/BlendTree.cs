using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace Isle.AnimationMachine
{
    public class BlendTree : Motion
    {
        //private Playable m_Playable;
        //TODO: 取得Motion长度，这里如果是BlendTree则需要取得各个动画混合后的长度
        public override float GetLength()
        {
            return base.GetLength();
        }

        public override void PreInit(PlayableAnimatorController controller)
        {
            base.PreInit(controller);
        }

        public override void LoadAsset()
        {
            base.LoadAsset();
        }

        /// <summary>
        ///   <para>Parameter that is used to compute the blending weight of the childs in 1D blend trees or on the X axis of a 2D blend tree.</para>
        /// </summary>
        [SerializeField] public string blendParameter;

        /// <summary>
        ///   <para>Parameter that is used to compute the blending weight of the childs on the Y axis of a 2D blend tree.</para>
        /// </summary>
        [SerializeField] public string blendParameterY;

        /// <summary>
        ///   <para>The Blending type can be either 1D or different types of 2D.</para>
        /// </summary>
        [SerializeField] public BlendTreeType blendType;

        /// <summary>
        ///   <para>A copy of the list of the blend tree child motions.</para>
        /// </summary>
        [SerializeField] public List<Isle.AnimationMachine.ChildMotion> children;

        public override Playable GetPlayable(PlayableGraph graph)
        {
            if (m_Playable.Equals(Playable.Null))
            {
                //m_Playable = AnimationClipPlayable.Create(graph, clip);
            }

            //Debug.Log("Motion type is (Animation)");
            return m_Playable;
        }

        public void Sort()
        {
            children.Sort((a, b) => a.threshold<b.threshold?-1:1);
        }
    }

    /*/// <summary>
    /// 线性插值采样点信息
    /// </summary>
    [Serializable]
    public class ChildMotion
    {
        public Motion motion;

        public float threshold;

        public float speed;
        
        public Vector2 dirPos;
    }*/
    
    
    [Serializable]/*[StructLayout(LayoutKind.Sequential)]*/
    public /*struct*/class ChildMotion
    {
        public Motion         motion                  { get { return m_Motion; }               set { m_Motion = value; }          }
        public float          threshold               { get { return m_Threshold; }            set { m_Threshold = value; }       }
        public Vector2        position                { get { return m_Position; }             set { m_Position = value; }        }
        public float          timeScale               { get { return m_TimeScale; }            set { m_TimeScale = value; }       }
        public float          cycleOffset             { get { return m_CycleOffset; }          set { m_CycleOffset = value; }     }
        public string         directBlendParameter    { get { return m_DirectBlendParameter; }     set { m_DirectBlendParameter = value; }}
        public bool           mirror                  { get { return m_Mirror; }               set { m_Mirror = value; }          }

        [SerializeField]Motion        m_Motion;
        [SerializeField]float         m_Threshold;
        [SerializeField]Vector2       m_Position;
        [SerializeField]float         m_TimeScale;
        [SerializeField]float         m_CycleOffset;
        [SerializeField]string        m_DirectBlendParameter;
        [SerializeField]bool          m_Mirror;
    }
}