using System;

namespace Isle.AnimationMachine
{
    [Serializable]
    public class TransitionCondition
    {
        public string Parameter;

        public AnimationParameterType ParameterType;

        public int IntValue = 0;

        public float FloatValue = 0;

        public bool BoolValue = false;
    }
}