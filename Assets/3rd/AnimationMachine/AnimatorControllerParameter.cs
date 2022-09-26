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

        public AnimatorControllerParameter()
        {
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
    /// <summary>
    ///   <para>Float type parameter.</para>
    /// </summary>
    Float = 1,
    /// <summary>
    ///   <para>Int type parameter.</para>
    /// </summary>
    Int = 3,
    /// <summary>
    ///   <para>Boolean type parameter.</para>
    /// </summary>
    Bool = 4,
    /// <summary>
    ///   <para>Trigger type parameter.</para>
    /// </summary>
    Trigger = 9,
}