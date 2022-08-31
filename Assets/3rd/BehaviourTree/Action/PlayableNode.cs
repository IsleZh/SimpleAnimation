using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Isle.BehaviourTree
{
    public class PlayableNode : ActionNode
    {
        public float duration = 1;
        public AnimationClip clip;
        public Animator animator;
        public bool exitOnEnd;
        PlayableGraph playableGraph;
        float startTime;

        protected override void OnStart()
        {
            Debug.Log("OnStartAction");
            startTime = Time.time;
            playableGraph = PlayableGraph.Create();
            playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            //var playableOutput = AnimationPlayableOutput.Create(playableGraph, "IsleAnimation", animator);
            //#TODO 发现找不到运行时游戏中的对象，所以只能出此下策Find(AI)。。。我记得Timeline里有一种找对象并且bind的方法。
            var playableOutput = AnimationPlayableOutput.Create(playableGraph, "IsleAnimation",
                GameObject.Find("AI").GetComponent<Animator>());
            // 将剪辑包裹在可播放项中
            var clipPlayable = AnimationClipPlayable.Create(playableGraph, clip);
            // 将可播放项连接到输出
            playableOutput.SetSourcePlayable(clipPlayable);
            // 播放该图。
            playableGraph.Play();
        }

        protected override void OnStop()
        {
            Debug.Log("OnStop");
            playableGraph.Destroy();
        }

        protected override State OnUpdate()
        {
            var Timing = Time.time - startTime;
            if (exitOnEnd && Timing > clip.length)
            {
                return State.Success;
            }

            if (Timing > duration)
            {
                return State.Success;
            }

            return State.Running;
        }
    }
}