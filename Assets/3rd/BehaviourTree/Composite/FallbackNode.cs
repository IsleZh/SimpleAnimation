namespace Isle.BehaviourTree
{
    /// <summary>
    /// 备选结点：按顺序执行孩子结点直到其中一个孩子结点返回成功状态或所有孩子结点返回失败状态。一般用来实现角色的备选行为。
    /// </summary>
    public class FallbackNode : CompositeNode
    {
        int current;

        protected override void OnStart()
        {
            current = 0;
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            var child = children[current];
            switch (child.Update())
            {
                case State.Running:
                    return State.Running;
                case State.Failure:
                    current++;
                    break;
                case State.Success:
                    return State.Success;
            }

            return current == children.Count ? State.Failure : State.Running;
        }
    }
}