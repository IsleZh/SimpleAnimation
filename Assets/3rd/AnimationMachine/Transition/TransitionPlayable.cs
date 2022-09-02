using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Isle.AnimationMachine
{
    public class TransitionPlayable : PlayableBehaviour
    {
        private StateTransition m_Transition;
        private StateMachine m_StateMachine;
        Playable mixer;
        private float currentWeight;
        private float clipLength;
        private float timer;

        public void Initialize(Motion motion, Playable owner,StateMachine stateMachine,PlayableGraph graph)
        {
            m_StateMachine = stateMachine;
            
            bool flag;
            owner.SetInputCount(1);

            mixer = AnimationMixerPlayable.Create(graph, 2);

            flag = graph.Connect(mixer, 0, owner, 0);
            Debug.Log("TransitionPlayable Connect is:" + flag);
            owner.SetInputWeight(0, 1);

            //graph.Connect(transition.from.motion.GetPlayable(graph), 0, mixer, 0);
            //连接一个空的在0端口上        结果：无效 Visual Graph 看不到此节点
            //graph.Connect(Playable.Null, 0, mixer, 0);
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
            //强行防止反复switch，以后要改逻辑
            if (m_Transition!=null)
            {
                return;
            }
            timer = 0f;
            m_Transition = transition;
            
            owner.SetInputWeight(0, 1);

            //mixer.DisconnectInput(0);//.GetInput(0)
            var fromPlayable = mixer.GetInput(1);
            mixer.DisconnectInput(1);
            mixer.ConnectInput(0,fromPlayable,0);
            //graph.Connect(transition.to.motion.GetPlayable(graph), 0, mixer, 1);
            //Debug.Log("IsValid:"+transition.to.motion.GetPlayable(graph).GetOutput(0).IsValid());
            mixer.ConnectInput(1,transition.to.motion.GetPlayable(graph), 0);
           
            //mixer.CanChangeInputs();
            //graph.Connect(transition.from.motion.GetPlayable(graph), 0, mixer, 0);
            //graph.Disconnect(transition.from.motion.GetPlayable(graph), 0);
            //mixer.AddInput(transition.to.motion.GetPlayable(graph), 0);
            //graph.Connect(transition.to.motion.GetPlayable(graph), 1, mixer, 0);
            //设置一个默认混合权重
            currentWeight = 0;
            mixer.SetInputWeight(0, 1-currentWeight);

            mixer.SetInputWeight(1, currentWeight);

            clipLength = transition.from.motion.GetLength();
            //TODO 测试转换BUG
            //EditorApplication.isPaused = true;
        }

        public override void PrepareFrame(Playable owner, FrameData info)
        {
            if (mixer.GetInputCount() == 0)

                return;
            if (m_Transition!=null)
            {
                Debug.Log("TransitionPlayable::m_Transition不为空，所以正在混合两个动画motion");
                timer += info.deltaTime;
                currentWeight = (timer / clipLength) / m_Transition.transitionDuration;
                
                if (currentWeight > 1)
                {
                    currentWeight = 1;
                    var fromPlayable = mixer.GetInput(0);
                    //在Graph的Mixer中断开连接
                    mixer.DisconnectInput(0);
                    //直接Destroy不知道行不行，是不是应该先从Graph中移出，
                    //fromPlayable.Destroy();
                    m_Transition = null;
                    m_StateMachine.FinishedGoto();
                }
                    
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