using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class AnimationManager
{
    private AssetBundle animationAB;
    private static AnimationManager _instance;

    public static AnimationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AnimationManager();
                _instance.InitAB();
            }
            return _instance;
        }
    }

    private AnimationManager()
    {
            
    }

    private void InitAB()
    {
        Debug.Log("InitAB");
        //animationAB = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "animations"));
        animationAB = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "animations.bundle"));
    }

    public AnimationClip PrepareAnimation(string animationName)
    {
        //animationAB.LoadAsset<AnimationClip>(animationName);
        AnimationClip[] clip = animationAB.LoadAssetWithSubAssets<AnimationClip>(animationName);
        Debug.Log(clip[0]);
        return clip[0];
    }
    
}