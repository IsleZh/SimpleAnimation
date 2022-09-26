using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using TextAsset = UnityEngine.TextCore.Text.TextAsset;

namespace Isle.AnimationMachine
{
    [CreateAssetMenu(fileName = "Blend1D Asset", menuName = "CreatePlayableAsset/Blend/AnimGraph_Blend1D", order = 1)]
    public class BlendTree1D : BlendTree
    {

        //[SerializeField] public List<float> thresholdArray;
        [SerializeField]private float[] thresholdArray;

        //TODO 还未进行初始化的controller
        public PlayableAnimatorController m_Controller;

        //public ScriptPlayable<AnimationBlendPlayable> AnimationBlendPlayable;

        /// <summary>
        /// 获得BlendTree_1D的当前长度，因为混合了多个motion所以需要通过Motion和Weights以及当前混合参数计算出来。
        /// </summary>
        /// <returns></returns>
        public override float GetLength()
        {
            var length = 0f;
            float[] weightArray = new float[children.Count];
            //Find可优化
            GetWeights(ref weightArray);
            for (int i = 0; i < children.Count; i++)
            {
                length = weightArray[i] * children[i].motion.GetLength();
            }

            return length;
        }

        //TODO 按需加载AnimationClip
        public override void LoadAsset()
        {
            m_Playable = Playable.Null;
        }

        public override Playable GetPlayable(PlayableGraph graph)
        {
            if (m_Playable.Equals(Playable.Null))
            {
                //PreInit();
                var animationBlendPlayable = ScriptPlayable<AnimationBlendPlayable>.Create(graph);
                var animationBlendPlayableBehaviour = animationBlendPlayable.GetBehaviour();
                animationBlendPlayableBehaviour.Init(this, animationBlendPlayable);
                m_Playable = animationBlendPlayable;
            }

            return m_Playable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlendTree"/> class.
        /// </summary>
        public override void PreInit(PlayableAnimatorController controller)
        {
            this.m_Controller = controller;
            //thresholdArray = new List<float>();
            thresholdArray = new float[children.Count];
            for (int i = 0; i < children.Count; i++)
            {
                thresholdArray[i] = children[i].threshold;
            }
        }

        /// <summary>
        /// 获得权值
        /// </summary>
        /// <param name="weightArray">返回的权重数组</param>
        public void GetWeights(ref float[] weightArray)
        {
            var blendValue = m_Controller.parameters.Find(x => x.Name == blendParameter).FloatValue;
            if (children.Count < 1)
                return;
            blendValue = Mathf.Clamp(blendValue, children[0].threshold,
                children[children.Count - 1].threshold);
            for (int i = 0; i < children.Count; i++)
                weightArray[i] = WeightForIndex(i, blendValue);
        }

        float WeightForIndex(int index, float blend)
        {
            if (blend >= thresholdArray[index])
            {
                if (index + 1 == thresholdArray.Length)
                {
                    return 1.0f;
                }
                else if (thresholdArray[index + 1] < blend)
                {
                    return 0.0f;
                }
                else
                {
                    if (thresholdArray[index] - thresholdArray[index + 1] != 0)
                    {
                        return (blend - thresholdArray[index + 1]) /
                               (thresholdArray[index] - thresholdArray[index + 1]);
                    }
                    else
                    {
                        return 1.0f;
                    }
                }
            }
            else
            {
                if (index == 0)
                {
                    return 1.0f;
                }
                else if (thresholdArray[index - 1] > blend)
                {
                    return 0.0f;
                }
                else
                {
                    if ((thresholdArray[index] - thresholdArray[index - 1]) != 0)
                    {
                        return (blend - thresholdArray[index - 1]) /
                               (thresholdArray[index] - thresholdArray[index - 1]);
                    }
                    else
                    {
                        return 1.0f;
                    }
                }
            }
        }
#if UNITY_EDITOR
        [ContextMenu("CreateAnimation")]
        public PlayableAnimationClip CreateAnimation()
        {
            PlayableAnimationClip playableAnimationClip = ScriptableObject.CreateInstance(typeof(PlayableAnimationClip)) as PlayableAnimationClip;
            playableAnimationClip.name = "Animation";
            playableAnimationClip.guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "CreateAnimation");
            if (this.children==null)
            {
                this.children = new List<ChildMotion>();
            }
            this.children.Add(new ChildMotion() {motion = playableAnimationClip});
            //var motionInfo = ScriptableObject.CreateInstance(typeof(Blend1DMotionInfo)) as Blend1DMotionInfo;
            //this.motionInfos.Add(motionInfo);
            //motionInfo.motion = animation;
            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(playableAnimationClip, this);
            }

            Undo.RegisterCreatedObjectUndo(playableAnimationClip, "CreateAnimation");

            AssetDatabase.SaveAssets();
            return playableAnimationClip;
        }
#endif
#if UNITY_EDITOR
        [ContextMenu("CreateBlendTree1D")]
        public BlendTree1D CreateBlendTree1D()
        {
            BlendTree1D blendTree = ScriptableObject.CreateInstance(typeof(BlendTree1D)) as BlendTree1D;
            blendTree.name = "BlendTree1D";
            blendTree.guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "CreateBlendTree1D");
            if (this.children==null)
            {
                this.children = new List<Isle.AnimationMachine.ChildMotion>();
            }
            this.children.Add(new ChildMotion() {motion = blendTree});
            //var motionInfo = ScriptableObject.CreateInstance(typeof(Blend1DMotionInfo)) as Blend1DMotionInfo;
            //motionInfo.motion = blendTree;
            //this.motionInfos.Add(motionInfo);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(blendTree, this);
            }

            Undo.RegisterCreatedObjectUndo(blendTree, "CreateBlendTree1D");

            AssetDatabase.SaveAssets();
            return blendTree;
        }
#endif
    }
}