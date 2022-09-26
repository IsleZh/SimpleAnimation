using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Isle.AnimationMachine
{
    public class AnimationBlendPlayable : PlayableBehaviour
    {
        private AnimationMixerPlayable mixer;
        private BlendTree1D blendTree;

        public void Init(Motion motion, Playable owner)
        {
            blendTree = (BlendTree1D) motion;
            bool flag;
            //设置输入端口数
            owner.SetInputCount(1);
            //创建动画混合器 输入端口数=Blend.Nodes.Count
            var inputCount = (blendTree.children.Count);
            mixer = AnimationMixerPlayable.Create(owner.GetGraph(), inputCount);
            //连接mixer到端口0
            flag = owner.GetGraph().Connect(mixer, 0, owner, 0);
            Debug.Log("动画mixer 连接到父motion:" + flag);
            //设置端口的初始输入权重
            owner.SetInputWeight(0, 1);


            float[] weightArray = new float[inputCount];
            blendTree.GetWeights(ref weightArray);
            for (int i = 0; i < inputCount; i++)
            {
                //获得playable
                var motionPlayable = blendTree.children[i].motion.GetPlayable(owner.GetGraph());
                //连接Motion的Playable到mixer
                flag = owner.GetGraph().Connect(motionPlayable, 0, mixer, i);
                Debug.Log("动画motion 连接到mixer:" + flag);
                mixer.SetInputWeight(i, weightArray[i]);
            }
        }

        public override void PrepareFrame(Playable owner, FrameData info)
        {
            var inputCount = (blendTree.children.Count);
            float[] weightArray = new float[inputCount];
            blendTree.GetWeights(ref weightArray);
            for (int i = 0; i < inputCount; i++)
            {
                //获得playable
                var motionPlayable = blendTree.GetPlayable(owner.GetGraph());
                //连接Motion的Playable到mixer
                //owner.GetGraph().Connect(motionPlayable, 0, mixer, i);
                mixer.SetInputWeight(i, weightArray[i]);
            }
        }

        public override void OnPlayableCreate(Playable playable)
        {
        }

        public override void OnGraphStart(Playable playable)
        {
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
        }


        public void SetWeight()
        {
        }
    }
}