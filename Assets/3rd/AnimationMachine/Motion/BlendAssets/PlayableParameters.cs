/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PlayableParameters", menuName = "CreatePlayableAsset/PlayableParameters", order = 5)]
public class PlayableParameters : ScriptableObject
{
    [SerializeField]
    public List<AnimParameter> animationParameters = new List<AnimParameter>();

}

[Serializable]
public class AnimParameter:IEquatable<AnimParameter>
{
    public string Name;

    public AnimationParameterType ParameterType;

    public int IntValue = 0;

    public float FloatValue = 0;

    public bool BoolValue = false;

    public bool Equals(AnimParameter other)
    {
        return Name == other.Name;
    }

    public AnimParameter(string name, AnimationParameterType animationParameterType)
    {
        Name = name;
        ParameterType = animationParameterType;
    }
}

public enum AnimationParameterType
{
    Int,
    Float,
    Bool
}
*/
