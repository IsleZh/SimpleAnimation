using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Isle.AnimationMachine
{
    public class Animation : Motion
    {
        public AnimationClip clip;
        private Playable m_Playable;

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
                m_Playable = AnimationClipPlayable.Create(graph, clip);
            }
            //Debug.Log("Motion type is (Animation)");
            return m_Playable;
        }
    }
}