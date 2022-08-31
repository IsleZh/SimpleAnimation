namespace Isle.BehaviourTree
{
    /// <summary>
    /// 并行结点：“并行执行”所有孩子结点。直到全部孩子结点返回成功状态或某个孩子结点返回失败状态。
    /// 创建这个节点的时候需要传入一个节点队列。一个接一个的运行子节点。如果子节点的状态是FAILED，那么它会将自己标识为FAILED并且直接返回；如果子节点的状态是SUCCESS或者RUNNING，那么它会运行下一个节点。只有所有的节点都标识为SUCCESS它会将自己的标识为SUCCESS并且返回，否则他会将自己标识为RUNNING。
    ///PS：部分节点（ConditionNode、NotDecorator）会在运行前被强制重启，用于判定
    /// </summary>
    public class ParallelNode : CompositeNode
    {
        int counter;

        protected override void OnStart()
        {
            counter = 0;
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            foreach (var child in children)
            {
                if (child.state != State.Success)
                {
                    var tmpState = child.Update();
                    if (tmpState == State.Failure)
                    {
                        return State.Failure;
                    }
                    else if (tmpState == State.Success)
                    {
                        ++counter;
                    }
                }
            }

            return children.Count == counter ? State.Success : State.Running;
        }
    }
}