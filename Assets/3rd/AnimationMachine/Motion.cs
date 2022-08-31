using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace Isle.AnimationMachine
{
    [System.Serializable]
    public class Motion
    {
        public Playable playable;
        //TODO: 取得Motion长度，这里如果是BlendTree则需要取得各个动画混合后的长度
        public virtual float GetLength()
        {
            return 0;
        }

        public virtual void LoadAsset()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// 根据情况传回对应Playable
        /// </summary>
        /// <returns></returns>
        public Playable GetPlayable(PlayableGraph graph)
        {
            return playable;
        }
    }
}