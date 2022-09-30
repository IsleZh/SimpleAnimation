/**
 *Created on 2020.3
 *Author:ZhangYuhao
 *Title: Unity AnimatorController 转换到 PlayableAnimator Asset 工具类
 */

// todo: 这里有个问题，同步层的实现只是复制了状态到需要同步的层，目前全由原始动画控制器导出，
//       所以在源层的asset中修改状态并不会自动同步到同步层，而是需要一个状态一个状态手动设置，
//       考虑后期用其他方法实现同步层，或者用编辑器实现自动同步修改状态。

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using Isle.AnimationMachine;
using NUnit.Framework;
using Animation = UnityEngine.Animation;
using AnimatorControllerParameter = Isle.AnimationMachine.AnimatorControllerParameter;
using BlendTree = UnityEditor.Animations.BlendTree;
using ChildMotion = UnityEditor.Animations.ChildMotion;
using Motion = UnityEngine.Motion;
using Object = System.Object;

namespace CostumeAnimator
{
    public class PlayableAnimatorUtil
    {
        #region Life

        private PlayableAnimatorUtil()
        {
        }

        public static PlayableAnimatorUtil GetInstance()
        {
            return Nested.instance;
        }

        class Nested
        {
            static Nested()
            {
            }

            internal static readonly PlayableAnimatorUtil instance = new PlayableAnimatorUtil();
        }

        #endregion

        #region Create Asset

        public T1 CreateAsset<T1>(string name)
            where T1 : PlayableAsset
        {
            T1 asset = ScriptableObject.CreateInstance<T1>();
            asset.name = name;
            AssetDatabase.CreateAsset(asset, GetNewAssetPath(name));
            AssetDatabase.Refresh();
            return asset;
        }

        public T1 CreateAsset<T1, T2>(string name, T2 parent)
            where T1 : PlayableAsset
            where T2 : PlayableAsset
        {
            T1 asset = ScriptableObject.CreateInstance<T1>();
            asset.guid = GUID.Generate().ToString();
            asset.name = name;
            //AssetDatabase.CreateAsset(asset, GetNewAssetPath(name));
            AssetDatabase.AddObjectToAsset(asset, parent);
            //AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return asset;
        }
        /*public T1 CreateAsset<T1,T2>(string name,T2 parent)
            where T1 : ScriptableObject
            where T2 : ScriptableObject
        {
            StateLayer layer = ScriptableObject.CreateInstance(typeof(StateLayer)) as StateLayer;
            layer.name = "StateLayer";
            layer.guid = GUID.Generate().ToString();
            Undo.RecordObject(parent, "CreateStateMachine");
            if (parent.layers==null)
            {
                parent.layers = new List<StateLayer>();
            }
            parent.layers.Add(layer);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(layer, parent);
            }

            Undo.RegisterCreatedObjectUndo(layer, "CreateStateMachine");

            AssetDatabase.SaveAssets();
            return layer;
        }
        public StateLayer CreateLayerAsset(string name,PlayableAnimatorController parent)
        {
            StateLayer layer = ScriptableObject.CreateInstance(typeof(StateLayer)) as StateLayer;
            layer.name = "StateLayer";
            layer.guid = GUID.Generate().ToString();
            Undo.RecordObject(parent, "CreateStateMachine");
            if (parent.layers==null)
            {
                parent.layers = new List<StateLayer>();
            }
            parent.layers.Add(layer);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(layer, parent);
            }

            Undo.RegisterCreatedObjectUndo(layer, "CreateStateMachine");

            AssetDatabase.SaveAssets();
            return layer;
        }*/

        /// <summary>
        /// 获取新建Asset的路径
        /// </summary>
        /// <param name="ctrlName"></param>
        /// <returns></returns>
        private string GetNewAssetPath(string name)
        {
            string dir = null;
            if (Selection.activeObject != null)
            {
                dir = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (Directory.Exists(dir) == false)
                {
                    var di = Directory.GetParent(dir);
                    dir = di.ToString();
                }
            }
            else
            {
                dir = "Assets";
            }

            return string.Format("{0}/{1}.asset", dir, name);
        }

        #endregion

        #region Trans

        /// <summary>
        /// 将AniamtorController转换为Playable版本的Asset
        /// </summary>
        /// <param name="path">已弃用参数</param>
        /// <param name="oldCtrl">原版AnimatorController</param>
        public void TransAnimator2Asset(AnimatorController oldCtrl)
        {
            // 生成动画控制器
            PlayableAnimatorController PACtrl = CreateAsset<PlayableAnimatorController>(oldCtrl.name);
            // 1.生成参数表
            PACtrl.parameters = new List<AnimatorControllerParameter>(oldCtrl.parameters.Length);
            for (int i = 0; i < oldCtrl.parameters.Length; i++)
            {
                var newParameter = new AnimatorControllerParameter();
                TransParameter(oldCtrl.parameters[i], newParameter);
                //PACtrl.parameters[i] = newParameter; 
                PACtrl.parameters.Add(newParameter);
            }

            // 2.生成动画层Layer
            AnimatorControllerLayer[] animCtrlLayers = oldCtrl.layers;
            PACtrl.layers = new List<StateLayer>(animCtrlLayers.Length);
            for (int i = 0; i < animCtrlLayers.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Transform Layer...", string.Format("duel with the {0} Layer...", i),
                    i / (float) animCtrlLayers.Length);

                //string layerName = string.Format("{0}_layer{1}", animCtrl.name, i);
                string layerName = animCtrlLayers[i].name;
                StateLayer newLayer = CreateAsset<StateLayer>(layerName);
                if (i == 0)
                    newLayer.defaultWeight = 1;
                else
                    newLayer.defaultWeight = animCtrlLayers[i].defaultWeight;
                newLayer.avatarMask = animCtrlLayers[i].avatarMask;
                newLayer.syncedLayerIndex = animCtrlLayers[i].syncedLayerIndex;
                newLayer.blendingMode = (LayerBlendingMode) animCtrlLayers[i].blendingMode;
                newLayer.iKPass = animCtrlLayers[i].iKPass;
                newLayer.syncedLayerAffectsTiming = animCtrlLayers[i].syncedLayerAffectsTiming;
                //PACtrl.layers[i] = newLayer;
                PACtrl.layers.Add(newLayer);


                AnimatorControllerLayer animCtrlLayer = animCtrlLayers[i];
                newLayer.avatarMask = animCtrlLayer.avatarMask;
                //#TODO 未实现以下参数对应功能
                /*assetLayer.isAdditive = animCtrlLayer.blendingMode == AnimatorLayerBlendingMode.Additive;
                assetLayer.timing = animCtrlLayer.syncedLayerAffectsTiming;*/

                // 判断是否是同步层 
                int syncLayerIndex = animCtrlLayer.syncedLayerIndex;
                bool isSync = syncLayerIndex != -1;
                AnimatorControllerLayer targetLayer = isSync ? animCtrlLayers[syncLayerIndex] : animCtrlLayer;
                // #TODO 未实现同步功能，望查阅功能作用
                //assetLayer.sourceLayerIndex = syncLayerIndex;

                // 生成动画组
                //List <StateMachine> assetGroups = new List<StateMachine>();

                // 生成默认动画机
                AnimatorStateMachine originStateMachine = targetLayer.stateMachine;
                string StateMatchineName = string.Format("{0}(StateMachine)", layerName);
                StateMachine stateMatchine = CreateAsset<StateMachine>(StateMatchineName);
                TransStateMachine(originStateMachine, stateMatchine, isSync, animCtrlLayer);
                //配置stateMachine
                newLayer.stateMachine = stateMatchine;
                //assetGroups.Add(assetDefaultGroup);

                // 生成子动画组（子控制器）
                /*ChildAnimatorStateMachine[] subMachines = animCtrlStateMachine.stateMachines;
                for (int j = 0; j < subMachines.Length; j++)
                {
                    EditorUtility.DisplayProgressBar("Transform SubMachines...", string.Format("duel with the {0} Machine...", j), j / (float)subMachines.Length);

                    AnimatorStateMachine originSubStateMachine = subMachines[j].stateMachine;
                    string subGroupName = string.Format("{0}_group_{1}_{2}", layerName, j, originSubStateMachine.name);
                    StateMachine assetSubGroup = CreateGroupAsset(subGroupName);

                    TransStateGroup(originSubStateMachine, assetSubGroup, isSync, animCtrlLayer, originSubStateMachine.name);
                    assetGroups.Add(assetSubGroup);

                    if (originSubStateMachine.stateMachines != null && originSubStateMachine.stateMachines.Length > 0)
                    {
                        Debug.LogErrorFormat("not support more then 1 layer subMachines, subMachine name:{0}", originSubStateMachine.name);
                    }
                }*/

                //assetLayer.stateGroups = assetGroups.ToArray();
            }

            EditorUtility.ClearProgressBar();
        }


        /// <summary>
        /// 把 AnimatorStateMachine 转换为 StateMachine
        /// </summary>
        /// <param name="originStateMachine"></param>
        /// <param name="stateMachine"></param>
        /// <param name="isSync"></param>
        /// <param name="groupName"></param>
        private void TransStateMachine(AnimatorStateMachine originStateMachine, StateMachine stateMachine,
            bool isSync, AnimatorControllerLayer overrideLayer)
        {
            //暂时记录一个原状态与新状态的绑定字典，用于状态转换Transitions的设置。
            Dictionary<AnimatorState, State> stateMap = new Dictionary<AnimatorState, State>();
            //暂时记录一个原子状态机与新子状态机的绑定字典，用于状态转换Transitions的设置。
            Dictionary<AnimatorStateMachine, ChildStateMachine> stateMachineMap =
                new Dictionary<AnimatorStateMachine, ChildStateMachine>();

            //收集原状态机的状态
            ChildAnimatorState[] animCtrlStates = originStateMachine.states;
            //收集原状态机的子状态机
            ChildAnimatorStateMachine[] animCtrlStateMachines = originStateMachine.stateMachines;
            //收集子状态机包含的所有状态
            UnityEditor.Animations.ChildAnimatorStateMachine[] stateMachines = originStateMachine.stateMachines;
            //1.处理所有的state基本数据
            stateMachine.states = new List<State>(animCtrlStates.Length);
            for (int i = 0; i < animCtrlStates.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Transform StateMachine...",
                    string.Format("duel with the {0} state...", i), i / (float) animCtrlStates.Length);

                AnimatorState state = animCtrlStates[i].state;
                string stateName = string.Format("{0}_{1}(state)", stateMachine.name, state.name);
                State newState = CreateAsset<State>(stateName);
                Motion motion =
                    isSync ? overrideLayer.GetOverrideMotion(state) : state.motion; // 如果是同步层，取OverrideMotion
                TransState(state, newState, motion);
                //stateMachine.states[i] = newState;
                stateMachine.states.Add(newState);


                //绑定原状态与新状态
                stateMap.Add(animCtrlStates[i].state, newState);
            }

            //2.设置状态机的默认状态State
            stateMachine.defaultState = stateMap[originStateMachine.defaultState];

            //3.转换所有子状态机,其中递归执行了1、2、3步骤
            stateMachine.stateMachines = new List<ChildStateMachine>(animCtrlStateMachines.Length);
            for (int i = 0; i < animCtrlStateMachines.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Transform State machine...",
                    string.Format("duel with the {0} child state machine...", i),
                    i / (float) animCtrlStateMachines.Length);

                ChildAnimatorStateMachine childStateMachine = animCtrlStateMachines[i];
                string stateMachineName = string.Format("{0}_{1}(state)", stateMachine.name,
                    childStateMachine.stateMachine.name);
                ChildStateMachine newChildStateMachine = CreateAsset<ChildStateMachine>(stateMachineName);
                //递归执行了1、2、3步骤
                TransChildStateMachine(childStateMachine.stateMachine, newChildStateMachine, stateMap, stateMachineMap,
                    isSync,
                    overrideLayer);
                //stateMachine.stateMachines[i] = newChildStateMachine;
                stateMachine.stateMachines.Add(newChildStateMachine);


                //绑定原状态与新状态
                stateMachineMap.Add(animCtrlStateMachines[i].stateMachine, newChildStateMachine);
            }

            //4.处理所有state的Transition
            /*for (int i = 0; i < animCtrlStates.Length; i++)
            {
                var oldState = animCtrlStates[i].state;
                var newState = stateMap[animCtrlStates[i].state];
                newState.transitions =
                    new List<NodeTransition>(animCtrlStates[i].state.transitions.Length);
                for (int j = 0; j < oldState.transitions.Length; j++)
                {
                    NodeTransition newTransition =
                        CreateAsset<NodeTransition, State>("transitions" + j, stateMap[oldState]);

                    TransTransition(oldState.transitions[j], newTransition, stateMap[oldState], stateMap,stateMachineMap);
                    newState.transitions.Add(newTransition);
                    //不知道为啥上面的CreateAsset改名和Save没有效果，只能再写一次了。
                    newTransition.name = "transitions" + j;
                    AssetDatabase.SaveAssets();
                    //newState.transitions[j] = newTransition;
                }
            }*/
            foreach (var statePair in stateMap)
            {
                statePair.Value.transitions =
                    new List<NodeTransition>();
                for (int j = 0; j < statePair.Key.transitions.Length; j++)
                {
                    NodeTransition newTransition =
                        CreateAsset<NodeTransition, State>("transitions" + j, stateMap[statePair.Key]);

                    TransTransition(statePair.Key.transitions[j], newTransition, stateMap[statePair.Key], stateMap,stateMachineMap);
                    statePair.Value.transitions.Add(newTransition);
                    //不知道为啥上面的CreateAsset改名和Save没有效果，只能再写一次了。
                    newTransition.name = "transitions" + j;
                    AssetDatabase.SaveAssets();
                    //newState.transitions[j] = newTransition;
                }
            }

            AssetDatabase.SaveAssets();
            /*assetStateGroup.groupName = groupName;
            assetStateGroup.motions = tmpMotions.ToArray();
            assetStateGroup.blendTrees = blendTrees.ToArray();*/
        }

        private void TransChildStateMachine(AnimatorStateMachine originStateMachine,
            ChildStateMachine stateMachine, Dictionary<AnimatorState, State> stateMap,
            Dictionary<AnimatorStateMachine, ChildStateMachine> stateMachineMap,
            bool isSync, AnimatorControllerLayer overrideLayer)
        {
            //收集原状态机的状态
            ChildAnimatorState[] animCtrlStates = originStateMachine.states;
            //收集原状态机的子状态机
            ChildAnimatorStateMachine[] animCtrlStateMachines = originStateMachine.stateMachines;
            //收集子状态机包含的所有状态
            UnityEditor.Animations.ChildAnimatorStateMachine[] stateMachines = originStateMachine.stateMachines;
            //1.处理所有的state基本数据
            stateMachine.states = new List<State>(animCtrlStates.Length);
            for (int i = 0; i < animCtrlStates.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Transform StateMachine...",
                    string.Format("duel with the {0} state...", i), i / (float) animCtrlStates.Length);

                AnimatorState state = animCtrlStates[i].state;
                string stateName = string.Format("{0}_{1}(state)", stateMachine.name, state.name);
                State newState = CreateAsset<State>(stateName);
                Motion motion =
                    isSync ? overrideLayer.GetOverrideMotion(state) : state.motion; // 如果是同步层，取OverrideMotion
                TransState(state, newState, motion);
                //stateMachine.states[i] = newState;
                stateMachine.states.Add(newState);


                //绑定原状态与新状态
                stateMap.Add(animCtrlStates[i].state, newState);
            }

            //2.设置状态机的默认状态State
            stateMachine.defaultState = stateMap[originStateMachine.defaultState];

            //3.转换所有子状态机,其中递归执行了1、2、3步骤
            stateMachine.stateMachines = new List<ChildStateMachine>(animCtrlStateMachines.Length);
            for (int i = 0; i < animCtrlStateMachines.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Transform State machine...",
                    string.Format("duel with the {0} child state machine...", i),
                    i / (float) animCtrlStateMachines.Length);

                ChildAnimatorStateMachine childStateMachine = animCtrlStateMachines[i];
                string stateMachineName = string.Format("{0}_{1}(state)", stateMachine.name,
                    childStateMachine.stateMachine.name);
                ChildStateMachine newChildStateMachine = CreateAsset<ChildStateMachine>(stateMachineName);
                TransChildStateMachine(childStateMachine.stateMachine, newChildStateMachine, stateMap, stateMachineMap,
                    isSync,
                    overrideLayer);
                //stateMachine.stateMachines[i] = newChildStateMachine;
                stateMachine.stateMachines.Add(newChildStateMachine);


                //绑定原状态与新状态
                stateMachineMap.Add(animCtrlStateMachines[i].stateMachine, newChildStateMachine);
            }
        }

        /// <summary>
        /// 转换State
        /// </summary>
        /// <param name="originState"></param>
        /// <param name="state"></param>
        private void TransState(AnimatorState originState, State state, Motion motion)
        {
            Isle.AnimationMachine.Motion newMotion = null;
            if (motion is BlendTree)
            {
                BlendTree originBlendTree = motion as BlendTree;
                //string blendTreeName = string.Format("{0}_blendTree_{1}", stateMachine.name, state.name);

                TransMotion(originBlendTree, out newMotion, state.name);
                //tmpMotions.Add(blendTree);
            }
            else if (motion is AnimationClip)
            {
                AnimationClip originClip = motion as AnimationClip;
                TransMotion(originClip, out newMotion, state.name);
                /*newMotion.clip = originClip;
                newMotion.speed = state.speed;
                newMotion.stateName = state.name;*/
                //tmpMotions.Add(animation);
            }

            state.motion = newMotion;
        }

        private void TransTransition(AnimatorStateTransition transition, NodeTransition newTransition, State state,
            Dictionary<AnimatorState, State> stateMap,Dictionary<AnimatorStateMachine, ChildStateMachine> stateMachineMap)
        {
            newTransition.name = transition.name;
            newTransition.from = state;
            if (transition.destinationState != null)
            {
                newTransition.to = stateMap[transition.destinationState];
            }else if (transition.destinationStateMachine != null)
            {
                newTransition.to = stateMachineMap[transition.destinationStateMachine];
            }
           
            newTransition.offset = transition.offset;
            newTransition.duration = transition.duration;
            newTransition.exitTime = transition.exitTime;
            newTransition.hasExitTime = transition.hasExitTime;
            newTransition.hasFixedDuration = transition.hasFixedDuration;
            newTransition.conditions = new TransitionCondition[transition.conditions.Length];
            for (int i = 0; i < transition.conditions.Length; i++)
            {
                newTransition.conditions[i] = new TransitionCondition();
                ;
                TransCondition(transition.conditions[i], newTransition.conditions[i]);
            }
        }

        /// <summary>
        /// 转换Condition
        /// </summary>
        /// <param name="transitionCondition"></param>
        /// <param name="newTransitionCondition"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void TransCondition(AnimatorCondition transitionCondition, TransitionCondition newTransitionCondition)
        {
            if (newTransitionCondition == null)
            {
                Debug.Log("newTransitionCondition == null");
            }

            newTransitionCondition.parameter = transitionCondition.parameter;
            //暂时试用的转换参数
            newTransitionCondition.FloatValue = transitionCondition.threshold;

            newTransitionCondition.mode = transitionCondition.mode;
            /*switch (transitionCondition.mode)
            {
                case TransitionMode.
            }*/
            //newTransitionCondition.parameterType = transitionCondition.mode;
            //TODO 添加这些转换方式
            /*newTransitionCondition.threshold = transitionCondition.threshold;
            newTransitionCondition.mode = transitionCondition.mode;*/
        }

        /// <summary>
        /// 转换Parameter
        /// </summary>
        /// <param name="oldCtrlParameter"></param>
        /// <param name="paCtrlParameter"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void TransParameter(UnityEngine.AnimatorControllerParameter oldCtrlParameter,
            AnimatorControllerParameter paCtrlParameter)
        {
            paCtrlParameter.ParameterType = (AnimationParameterType) oldCtrlParameter.type;
            paCtrlParameter.Name = oldCtrlParameter.name;
            paCtrlParameter.BoolValue = oldCtrlParameter.defaultBool;
            paCtrlParameter.FloatValue = oldCtrlParameter.defaultFloat;
            paCtrlParameter.IntValue = oldCtrlParameter.defaultInt;
        }

        /// <summary>
        /// 把 UnityMotion 转换为 Motion
        /// </summary>
        /// <param name="originBlendTree"></param>
        /// <param name="blendTree"></param>
        private void TransMotion(UnityEngine.Motion originMotion, out Isle.AnimationMachine.Motion newMotion,
            string stateName)
        {
            if (originMotion is AnimationClip clip)
            {
                newMotion = CreateAsset<Isle.AnimationMachine.PlayableAnimationClip>(originMotion.name);
                TransAnimationClip(clip, (Isle.AnimationMachine.PlayableAnimationClip) newMotion, originMotion.name);
            }
            else if (originMotion is BlendTree tree)
            {
                TransBlendTree(tree, out var newTree, originMotion.name);
                newMotion = newTree;
            }
            else
            {
                newMotion = null;
                Debug.LogError("转换失败，尝试转换不被支持的Motion类型");
            }
        }

        /// <summary>
        /// 把 Unity.AnimationClip 转换为 AnimationClip  
        /// </summary>
        /// <param name="oldAnimation"></param>
        /// <param name="animation"></param>
        /// <param name="name"></param>
        private void TransAnimationClip(AnimationClip oldAnimation,
            Isle.AnimationMachine.PlayableAnimationClip animation,
            string name)
        {
            //animation.name = oldAnimation.name;
            animation.clip = oldAnimation;
            //TODO animation.abPath = ........
        }

        /// <summary>
        /// 把 BlendTree 转换为 BlendTree
        /// </summary>
        /// <param name="originBlendTree"></param>
        /// <param name="blendTree"></param>
        private void TransBlendTree(BlendTree originBlendTree, out Isle.AnimationMachine.BlendTree blendTree,
            string blendTreeName)
        {
            //blendTree.name = blendTreeName;

            if (originBlendTree.blendType == BlendTreeType.Simple1D)
            {
                blendTree = CreateAsset<Isle.AnimationMachine.BlendTree1D>(originBlendTree.name);
                //blendTree.name = originBlendTree.name;
                blendTree.blendParameter = originBlendTree.blendParameter;
                blendTree.blendType = BlendTreeType.Simple1D; // 目前只支持1D混合树
            }
            else
            {
                Debug.LogError("not support blendTree type except Simple1D");
                blendTree = null;
                return;
            }

            ChildMotion[] originChilds = originBlendTree.children;
            blendTree.children = new List<Isle.AnimationMachine.ChildMotion>(originChilds.Length);
            for (int i = 0; i < originChilds.Length; i++)
            {
                Isle.AnimationMachine.ChildMotion childMotion = new Isle.AnimationMachine.ChildMotion();
                //newMotion.motion = CreateAsset<Isle.AnimationMachine.BlendTree>(originBlendTree.name);
                TransMotion(originChilds[i].motion, out var newMotion,
                    "motionName???????");
                childMotion.motion = newMotion;
                childMotion.timeScale = originChilds[i].timeScale;
                childMotion.position = originChilds[i].position;
                childMotion.threshold = originChilds[i].threshold;
                childMotion.directBlendParameter = originChilds[i].directBlendParameter;
                //blendTree.children[i] = newMotion;
                blendTree.children.Add(childMotion);
            }

            blendTree.Sort(); // 按照threshold排序
        }

        #endregion
    }
}