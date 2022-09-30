using System;
using UnityEditor.Animations;
using UnityEngine;

namespace Isle.AnimationMachine
{
    public class NodeTransition : PlayableAsset
    {
        public Node from, to;
        /// <summary>
        ///   <para>When active the transition will have an exit time condition.</para>
        /// </summary>
        public bool hasExitTime { get; set; } = true;
        #region Settings
       
        /// <summary>
        /// 当前动画退出点(单位：当前动画的百分比)
        /// </summary>
        public float exitTime;
        /// <summary>
        /// 过渡时长(单位：当前动画的百分比)
        /// </summary>
        public float duration;
        /// <summary>
        /// 目标动画起始点(单位：目标动画的百分比)
        /// </summary>
        public float offset;
        #endregion
        
        [SerializeField]public TransitionCondition[] conditions;
        [SerializeField]public PlayableAnimatorController controller;
        #region Todo
        public bool hasFixedDuration;
        
        #endregion

        /// <summary>
        /// TODO 尝试切换条件是否完全满足，完全满足则应该设置stateMachine的NextState;
        /// </summary>
        /// <returns></returns>
        public bool TryTransition(PlayableAnimatorController controller)
        {
            foreach (var cond in conditions)
            {
                Debug.Log("Transition.Cond= " + cond.parameter);
                var param = controller.parameters.Find(x => x.Name == cond.parameter);
                Debug.Log("Transition.Param= " + param);
                //不启用我的Condition 直接用原版condition匹配
                if (cond.mode==AnimatorConditionMode.If&&param.BoolValue == false||cond.mode==AnimatorConditionMode.IfNot&&param.BoolValue == true)
                {
                    return false;
                }
                /*
                if (param.BoolValue !=cond.BoolValue)
                {
                    return false;
                }
                */
                
            }
            return true;
        }

        
    }
}