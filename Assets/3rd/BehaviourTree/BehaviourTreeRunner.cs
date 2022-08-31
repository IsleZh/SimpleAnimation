using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Isle.BehaviourTree
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        public BehaviourTree tree;

        // Start is called before the first frame update
        void Start()
        {
            tree = tree.Clone();
            //tree.Bind(GetComponent<AiAgent>());#TODO 绑定资源可能是用这种方法
            tree.Bind();
        }

        // Update is called once per frame
        void Update()
        {
            tree.Update();
        }
    }
}