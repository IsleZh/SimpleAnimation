using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace Isle.AnimationMachine
{
    public class Animation : Motion
    {
        public AnimationClip clip;
        private Playable m_Playable;

        public override float GetLength()
        {
            LoadAsset();
            length = clip.length;
            return length;
        }
        //TODO 按需加载AnimationClip
        public override void LoadAsset()
        {
            base.LoadAsset();
        }

        public override Playable GetPlayable(PlayableGraph graph)
        {
            if (m_Playable.Equals(Playable.Null))
            {
                m_Playable = AnimationClipPlayable.Create(graph, clip);
            }
            //Debug.Log("Motion type is (Animation)");
            return m_Playable;
        }
    }
}