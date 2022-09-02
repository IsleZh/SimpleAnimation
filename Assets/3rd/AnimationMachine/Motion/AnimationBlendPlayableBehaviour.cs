using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Isle.AnimationMachine
{
public class AnimationBlendPlayableBehaviour : PlayableBehaviour
{
    public float firstClipWeight;
    AnimationMixerPlayable m_mixerPlayable;
    Playable m_fatherMixerPlayable;//控制所有AnimationBlendPlayableBehaviour的AnimationMixerPlayable
    PlayableGraph m_playableGraph;
    float m_firstClipLength, m_secondClipLength;

    public void Init(AnimationClip clip1, AnimationClip clip2, float weight) {
        var clip1Playable = AnimationClipPlayable.Create(m_playableGraph, clip1);
        var clip2Playable = AnimationClipPlayable.Create(m_playableGraph, clip2);
        m_mixerPlayable.ConnectInput(0, clip1Playable, 0);
        m_mixerPlayable.ConnectInput(1, clip2Playable, 0);
        firstClipWeight = Mathf.Clamp01(weight);
        m_firstClipLength = clip1.length;
        m_secondClipLength = clip2.length;
        clip1Playable.SetSpeed(m_firstClipLength);
        clip2Playable.SetSpeed(m_secondClipLength);
    }

    public override void OnPlayableCreate(Playable playable) {
        base.OnPlayableCreate(playable);

        m_playableGraph = playable.GetGraph();
        m_mixerPlayable = AnimationMixerPlayable.Create(m_playableGraph, 2);
        playable.ConnectInput(0, m_mixerPlayable, 0);
    }

    public override void OnGraphStart(Playable playable) {
        base.OnGraphStart(playable);

        m_fatherMixerPlayable = playable.GetOutput(0);
        if(!m_fatherMixerPlayable.IsPlayableOfType<AnimationMixerPlayable>())
            Debug.LogError("Get AnimationMixerPlayable Error");

        //如果是第一个Clip，直接设置权重
        if(playable.Equals(m_fatherMixerPlayable.GetInput(0)))
            SetWeight();
    }

    public override void OnBehaviourPause(Playable playable, FrameData info) {
        base.OnBehaviourPause(playable, info);

        //播放结束
        if(playable.GetTime() >= playable.GetDuration()) {
            m_mixerPlayable.SetInputWeight(0, 0);
            m_mixerPlayable.SetInputWeight(1, 0);

            //设置下一个Clip权重
            for(int i = 0, count = m_fatherMixerPlayable.GetInputCount(); i < count - 1; i++) {
                if(playable.Equals(m_fatherMixerPlayable.GetInput(i))) {
                    ScriptPlayable<AnimationBlendPlayableBehaviour> sp = (ScriptPlayable<AnimationBlendPlayableBehaviour>)m_fatherMixerPlayable.GetInput(i + 1);
                    sp.GetBehaviour().SetWeight();
                    break;
                }
            }
        }
    }

    public void SetWeight() {
        float secondClipWeight = 1.0f - firstClipWeight;
        m_mixerPlayable.SetInputWeight(0, firstClipWeight);
        m_mixerPlayable.SetInputWeight(1, secondClipWeight);
        float mixerPlayableSpeed = 1.0f / (firstClipWeight * m_firstClipLength + secondClipWeight * m_secondClipLength);
        m_mixerPlayable.SetSpeed(mixerPlayableSpeed);
    }
}

}