using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Isle.BehaviourTree
{
    /// <summary>
    /// 顺序结点：按顺序执行孩子结点直到其中一个孩子结点返回失败状态或所有孩子结点返回成功状态。
    /// </summary>
    public class SequencerNode : CompositeNode
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
                    return State.Failure;
                case State.Success:
                    current++;
                    break;
            }

            return current == children.Count ? State.Success : State.Running;
        }
    }
}