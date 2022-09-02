using System;

namespace Isle.AnimationMachine
{
    [Serializable]
    public class AnimatorControllerParameter:IEquatable<AnimatorControllerParameter>
    {
        public string Name;

        public AnimationParameterType ParameterType;

        public int IntValue = 0;

        public float FloatValue = 0;

        public bool BoolValue = false;

        public bool Equals(AnimatorControllerParameter other)
        {
            return Name == other.Name;
        }

        public AnimatorControllerParameter(string name, AnimationParameterType animationParameterType)
        {
            Name = name;
            ParameterType = animationParameterType;
        }
    }
}

public enum AnimationParameterType
{
    Int,
    Float,
    Bool
}