using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Isle.AnimationMachine
{
    public class TransitionPlayable : PlayableBehaviour
    {
        private StateTransition m_Transition;

        private Playable mixer;

        public void Initialize(StateTransition transition, Playable owner, PlayableGraph graph)

        {
            owner.SetInputCount(1);

            mixer = AnimationMixerPlayable.Create(graph, 2);

            graph.Connect(mixer, 0, owner, 0);

            owner.SetInputWeight(0, 1);

            graph.Connect(transition.from.motion.GetPlayable(graph), 0, mixer, 0);

            graph.Connect(transition.to.motion.GetPlayable(graph), 0, mixer, 1);

            //设置一个默认混合权重
            mixer.SetInputWeight(0, 1.0f);

            mixer.SetInputWeight(1, 1.0f);
        }

        public override void PrepareFrame(Playable owner, FrameData info)
        {
            if (mixer.GetInputCount() == 0)

                return;

            /*// 必要时，前进到下一剪辑

            m_TimeToNextClip -= (float) info.deltaTime;

            if (m_TimeToNextClip <= 0.0f)

            {
                m_CurrentClipIndex++;

                if (m_CurrentClipIndex >= mixer.GetInputCount())

                    m_CurrentClipIndex = 0;

                var currentClip = (AnimationClipPlayable) mixer.GetInput(m_CurrentClipIndex);

                // 重置时间，以便下一个剪辑从正确位置开始

                currentClip.SetTime(0);

                m_TimeToNextClip = currentClip.GetAnimationClip().length;
            }

            // 调整输入权重

            for (int clipIndex = 0; clipIndex < mixer.GetInputCount(); ++clipIndex)

            {
                if (Mathf.Abs(clipIndex - m_CurrentClipIndex) <= 1)

                    mixer.SetInputWeight(clipIndex, 1.0f);

                else

                    mixer.SetInputWeight(clipIndex, 0.0f);
            }*/
        }
    }
}