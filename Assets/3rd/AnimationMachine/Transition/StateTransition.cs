using System;
using UnityEngine;

namespace Isle.AnimationMachine
{
    public class StateTransition : ScriptableObject
    {
        public string guid;
        public State from, to;
        public bool hasExitTime = true;
        #region Settings
       
        /// <summary>
        /// 当前动画退出点(单位：当前动画的百分比)
        /// </summary>
        public float exitTime;
        /// <summary>
        /// 过渡时长(单位：当前动画的百分比)
        /// </summary>
        public float transitionDuration;
        /// <summary>
        /// 目标动画起始点(单位：目标动画的百分比)
        /// </summary>
        public float transitionOffset;
        #endregion
        
        public TransitionCondition[] conditions;

        #region Todo
        public bool fixedDuration;


        #endregion

        /// <summary>
        /// TODO 尝试切换条件是否完全满足，完全满足则应该设置stateMachine的NextState;
        /// </summary>
        /// <returns></returns>
        public bool TryTransition()
        {
            return true;
        }

        
    }
}