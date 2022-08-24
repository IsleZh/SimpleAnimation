using System;
using UnityEngine;


    public class WaitNode:ActionNode
    {
        public float duration = 1;
        private float startTime;
        protected override void Onstart()
        {
           startTime = Time.time;
        }

        protected override void OnStop()
        {
            //throw new System.NotImplementedException();
        }

        protected override State OnUpdate()
        {
            if (Time.time - startTime > duration)
            {
                return State.Success;
            }

            return State.Running;
        }
    }
