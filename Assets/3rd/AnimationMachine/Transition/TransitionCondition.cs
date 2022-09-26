using System;
using UnityEditor.Animations;

namespace Isle.AnimationMachine
{
    [Serializable]
    public class TransitionCondition
    {
        public string parameter;
        private AnimatorConditionMode m_ConditionMode;
        public AnimationParameterType parameterType;
        /// <summary>
        ///   <para>The mode of the condition.</para>
        /// </summary>
        public AnimatorConditionMode mode
        {
            get => this.m_ConditionMode;
            set => this.m_ConditionMode = value;
        }
        public int IntValue = 0;

        public float FloatValue = 0;

        public bool BoolValue = false;
    }
}