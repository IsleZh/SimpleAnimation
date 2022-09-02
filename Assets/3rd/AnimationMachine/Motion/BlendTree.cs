using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Isle.AnimationMachine
{
    public class BlendTree : Motion
    {
        private Playable m_Playable;
        //TODO: 取得Motion长度，这里如果是BlendTree则需要取得各个动画混合后的长度
        public override float GetLength()
        {
            return base.GetLength();
        }

        public override void LoadAsset()
        {
            base.LoadAsset();
        }
        
        public override Playable GetPlayable(PlayableGraph graph)
        {
            if (m_Playable.Equals(Playable.Null))
            {
                //m_Playable = AnimationClipPlayable.Create(graph, clip);
            }
            //Debug.Log("Motion type is (Animation)");
            return m_Playable;
        }
    }
}