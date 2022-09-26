using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace Isle.AnimationMachine
{
    [System.Serializable]
    public class Motion : PlayableAsset
    {
        protected Playable m_Playable;
        

        //TODO 取得Motion长度，这里如果是BlendTree则需要取得各个动画混合后的长度
        //TODO 考虑在之后持久化保存ab资源加载路径，以及自动持久化保存好动画长度
        public virtual float GetLength()
        {
            return 0;
        }

        public virtual void PreInit(PlayableAnimatorController controller)
        {
            
        }

        public virtual void LoadAsset()
        {
        }
        

        /// <summary>
        /// 根据情况传回对应Playable
        /// </summary>
        /// <returns></returns>
        public virtual Playable GetPlayable(PlayableGraph graph)
        {
            Debug.Log("Motion type is (Motion)");
            return Playable.Null;
        }
    }
}