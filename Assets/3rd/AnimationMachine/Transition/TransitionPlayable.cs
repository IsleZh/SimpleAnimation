using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Isle.AnimationMachine
{
    public class TransitionPlayable : PlayableBehaviour
    {
        private StateTransition m_Transition;
        private Playable mixer;
        private float currentWeight;
        private float clipLength;
        private float timer;

        public void Initialize(Motion motion, Playable owner, PlayableGraph graph)
        {
            bool flag;
            owner.SetInputCount(1);

            mixer = AnimationMixerPlayable.Create(graph, 2);

            flag = graph.Connect(mixer, 0, owner, 0);
            Debug.Log("TransitionPlayable Connect is:" + flag);
            owner.SetInputWeight(0, 1);

            //graph.Connect(transition.from.motion.GetPlayable(graph), 0, mixer, 0);
            //连接一个空的在0端口上        结果：无效 Visual Graph 看不到此节点
            graph.Connect(Playable.Null, 0, mixer, 0);
            Debug.Log("motion.GetPlayable(graph) is" +motion.GetPlayable(graph));
            flag = graph.Connect(motion.GetPlayable(graph), 0, mixer, 1);
            Debug.Log("AnimationclipPlayable Connect is:" + flag);
            //设置一个默认混合权重
            currentWeight = 1;
            mixer.SetInputWeight(0, 1-currentWeight);

            mixer.SetInputWeight(1, currentWeight);

            clipLength = motion.GetLength();
        }
        
        public void Switch(StateTransition transition, Playable owner, PlayableGraph graph)
        {
            m_Transition = transition;
            
            owner.SetInputWeight(0, 1);

            //graph.Connect(transition.from.motion.GetPlayable(graph), 0, mixer, 0);
            graph.Disconnect(transition.from.motion.GetPlayable(graph), 0);
            mixer.AddInput(transition.to.motion.GetPlayable(graph), 1);
            //graph.Connect(transition.to.motion.GetPlayable(graph), 0, mixer, 1);
            //设置一个默认混合权重
            currentWeight = 0;
            mixer.SetInputWeight(0, 1-currentWeight);

            mixer.SetInputWeight(1, currentWeight);

            clipLength = transition.from.motion.GetLength();
        }

        public override void PrepareFrame(Playable owner, FrameData info)
        {
            if (mixer.GetInputCount() == 0)

                return;
            if (m_Transition!=null)
            {
                timer += info.deltaTime;
                currentWeight = (timer / clipLength) / m_Transition.transitionDuration;
                if(currentWeight>1)
                    currentWeight = 1;
                mixer.SetInputWeight(0, 1-currentWeight);

                mixer.SetInputWeight(1, currentWeight);
            }
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