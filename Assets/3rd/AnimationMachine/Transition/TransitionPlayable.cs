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
        private Playable mixer;
        private float currentWeight;
        private float clipLength;
        private float timer;

        public void Init(Motion motion, Playable owner,StateMachine stateMachine)
        {
            m_StateMachine = stateMachine;
            bool flag;
            var graph = owner.GetGraph();
            //设置输出端口数
            owner.SetInputCount(1);
            //创建动画混合器 输入端口数=2
            mixer = AnimationMixerPlayable.Create(graph, 2);
            //连接mixer到端口0
            flag = graph.Connect(mixer, 0, owner, 0);
            Debug.Log("TransitionPlayable Connect is:" + flag);
            //设置端口的输入权重
            owner.SetInputWeight(0, 1);

            //graph.Connect(transition.from.motion.GetPlayable(graph), 0, mixer, 0);
            //连接一个空的在0端口上        结果：无效 Visual Graph 看不到此节点
            //graph.Connect(Playable.Null, 0, mixer, 0);
            Debug.Log("motion.GetPlayable(graph) is" +motion.GetPlayable(graph));
            //获得playable
            var motionPlayable = motion.GetPlayable(graph);
            //连接Motion的Playable到mixer
            flag = graph.Connect(motionPlayable, 0, mixer, 1);
            Debug.Log("AnimationclipPlayable Connect is:" + flag);
            //设置一个默认混合权重
            currentWeight = 1;
            mixer.SetInputWeight(0, 1-currentWeight);
            mixer.SetInputWeight(1, currentWeight);
            //获取当前长度
            clipLength = motion.GetLength();
        }
        /// <summary>
        /// 开始切换状态
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="owner"></param>
        /// <param name="graph"></param>
        public void DoSwitch(StateTransition transition, Playable owner)
        {
            //强行防止反复switch，以后要改逻辑
            if (m_Transition!=null)
            {
                return;
            }
            timer = 0f;
            m_Transition = transition;
            owner.SetInputWeight(0, 1);
            //获得端口1的Playable
            var fromPlayable = mixer.GetInput(1);
            //断开端口1的输入
            mixer.DisconnectInput(1);
            //然后连接到端口0上
            mixer.ConnectInput(0,fromPlayable,0);
            //graph.Connect(transition.to.motion.GetPlayable(graph), 0, mixer, 1);
            //Debug.Log("IsValid:"+transition.to.motion.GetPlayable(graph).GetOutput(0).IsValid());
            //最后把要转换的动画连接到端口1上
            mixer.ConnectInput(1,transition.to.motion.GetPlayable(owner.GetGraph()), 0);
           
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
                currentWeight = (timer / clipLength) / m_Transition.duration;
                
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
        }
    }
}