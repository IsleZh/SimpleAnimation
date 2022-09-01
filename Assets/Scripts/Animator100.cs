using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class Animator100 : MonoBehaviour
{
    public Animator a;

    public int AnimationCount;
    private int[] statesHash;
    float Timer;

    private int animationIndex;

    // Start is called before the first frame update
    void Start()
    {
        a = GetComponent<Animator>();
        

        AnimatorController animatorController = a.runtimeAnimatorController as AnimatorController;
        AnimatorStateMachine stateMachine = animatorController.layers[0].stateMachine;
        statesHash = new int[stateMachine.states.Length];
        for (int i = 0; i < stateMachine.states.Length; i++)
        {
            statesHash[i] = stateMachine.states[i].state.nameHash;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Time.realtimeSinceStartup - Timer > 1)
        {
            SwitchAnimator();
            Timer = Time.realtimeSinceStartup;
        }
    }

    public void SwitchAnimator()
    {
        a.Play(statesHash[animationIndex++]);
        if (animationIndex == statesHash.Length)
        {
            animationIndex = 0;
        }
    }
}