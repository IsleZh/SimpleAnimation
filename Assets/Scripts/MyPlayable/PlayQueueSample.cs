﻿using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace MyPlayable
{
    public class PlayQueuePlayable : PlayableBehaviour

    {

        private int m_CurrentClipIndex = -1;

        private float m_TimeToNextClip;

        private Playable mixer;

        public void Initialize(AnimationClip[] clipsToPlay, Playable owner, PlayableGraph graph)

        {

            owner.SetInputCount(1);

            mixer = AnimationMixerPlayable.Create(graph, clipsToPlay.Length);

            graph.Connect(mixer, 0, owner, 0);

            owner.SetInputWeight(0, 1);

            for (int clipIndex = 0 ; clipIndex < mixer.GetInputCount() ; ++clipIndex)

            {

                graph.Connect(AnimationClipPlayable.Create(graph, clipsToPlay[clipIndex]), 0, mixer, clipIndex);

                mixer.SetInputWeight(clipIndex, 1.0f);

            }

        }

        override public void PrepareFrame(Playable owner, FrameData info)

        {

            if (mixer.GetInputCount() == 0)

                return;

            // 必要时，前进到下一剪辑

            m_TimeToNextClip -= (float)info.deltaTime;

            if (m_TimeToNextClip <= 0.0f)

            {

                m_CurrentClipIndex++;

                if (m_CurrentClipIndex >= mixer.GetInputCount())

                    m_CurrentClipIndex = 0;

                var currentClip = (AnimationClipPlayable)mixer.GetInput(m_CurrentClipIndex);

                // 重置时间，以便下一个剪辑从正确位置开始

                currentClip.SetTime(0);

                m_TimeToNextClip = currentClip.GetAnimationClip().length;
                

            }

            // 调整输入权重

            for (int clipIndex = 0 ; clipIndex < mixer.GetInputCount(); ++clipIndex)

            {

                if (Mathf.Abs(clipIndex -m_CurrentClipIndex)<=1)

                    mixer.SetInputWeight(clipIndex, 1.0f);

                else

                    mixer.SetInputWeight(clipIndex, 0.0f);

            }

        }

    }

    [RequireComponent(typeof (Animator))]

    public class PlayQueueSample : MonoBehaviour

    {

        public AnimationClip[] clipsToPlay;

        PlayableGraph playableGraph;

        void Start()

        {

            playableGraph = PlayableGraph.Create("Animation");

            var playQueuePlayable = ScriptPlayable<PlayQueuePlayable>.Create(playableGraph);

            var playQueue = playQueuePlayable.GetBehaviour();

            playQueue.Initialize(clipsToPlay, playQueuePlayable, playableGraph);

            var playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", GetComponent<Animator>());

            playableOutput.SetSourcePlayable(playQueuePlayable);
            playableOutput.SetSourceInputPort(0);

            playableGraph.Play();

        }

        void OnDisable()

        {

            // 销毁该图创建的所有可播放项和输出。

            playableGraph.Destroy();

        }

    }
}