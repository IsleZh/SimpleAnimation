using UnityEngine;

namespace Isle.AnimationMachine
{
    public class Node : PlayableAsset
    {
        //[HideInInspector] public string guid;
        //[HideInInspector] public Vector2 position;
        public virtual State GetState()
        {
            return null;
        }
    }
}