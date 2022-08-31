using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace MyPlayable
{
    [RequireComponent(typeof(Animator))]

    public class PlayAnimationSample : MonoBehaviour

    {

        public AnimationClip clip;

        PlayableGraph playableGraph;

        void Start()

        {

            playableGraph = PlayableGraph.Create();

            playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            var playableOutput = AnimationPlayableOutput.Create(playableGraph, "IsleAnimation", GetComponent<Animator>());

            // 将剪辑包裹在可播放项中

            var clipPlayable = AnimationClipPlayable.Create(playableGraph, clip);

            // 将可播放项连接到输出

            playableOutput.SetSourcePlayable(clipPlayable);

            // 播放该图。

            playableGraph.Play();

        }

        void OnDisable()

        {

            //销毁该图创建的所有可播放项和 PlayableOutput。

            playableGraph.Destroy();

        }

    }
}