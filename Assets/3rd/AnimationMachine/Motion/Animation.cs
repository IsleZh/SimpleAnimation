using UnityEngine;

namespace Isle.AnimationMachine
{
    public class Animation : Motion
    {
        public AnimationClip clip;
        public override float GetLength()
        {
            return base.GetLength();
        }

        public override void LoadAsset()
        {
            base.LoadAsset();
        }
    }
}