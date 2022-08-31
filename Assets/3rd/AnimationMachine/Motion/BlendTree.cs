namespace Isle.AnimationMachine
{
    public class BlendTree : Motion
    {
        //TODO: 取得Motion长度，这里如果是BlendTree则需要取得各个动画混合后的长度
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