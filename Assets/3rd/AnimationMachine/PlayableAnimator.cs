using System;
using UnityEngine;

namespace Isle.AnimationMachine
{
    [RequireComponent(typeof(Animator))]
    public class PlayableAnimator :MonoBehaviour
    {
        public Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }
    }
}