using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using UnityEngine.Animations;
using UnityEngine.Playables;


namespace Isle.AnimationMachine
{
    [CreateAssetMenu(fileName = "PlayableAnimatorController",
        menuName = "PlayableAnimation/Playable Animator Controller")]
    public class PlayableAnimatorController : PlayableAsset
    {
        //private Animator animator;
        //private AnimatorController animatorController;
        
        [SerializeField] private List<StateLayer> stateLayers;
        [SerializeField] public List<AnimatorControllerParameter> parameters;

        public List<StateLayer> layers
        {
            get => stateLayers;
            set => stateLayers = value;
        }

        public AnimationLayerMixerPlayable LayerMixerPlayable { get; set; }

        public PlayableAnimator PlayableAnimator { get; set; }

        public void Initialize(PlayableAnimator animator,PlayableGraph graph)
        {
            LayerMixerPlayable = AnimationLayerMixerPlayable.Create(graph, layers.Count);
            //猜的
            animator.m_AnimationPlayableOutput.SetSourcePlayable(LayerMixerPlayable, 0);
            PlayableAnimator = animator;
            for (int i=0;i<stateLayers.Count;i++)
            {
                stateLayers[i].Initialize(this);
                //TODO 最后的权重应该有一个配置 暂时设为1是蒙的
                LayerMixerPlayable.ConnectInput(i,stateLayers[i].stateMachine.TransitionPlayable,0,1f);
                //TODO 实现功能
                /*LayerMixerPlayable.SetLayerAdditive();
                LayerMixerPlayable.SetLayerMaskFromAvatarMask();*/
            }
        }
        public void Update()
        {
            foreach (var sl in stateLayers)
            {
                Debug.Log("State Layers>0");
                sl.Update();
            }
        }

        /*public void Test()
        {
            animator = new Animator();
            animatorController.layers;
            animatorController.parameters;
            animatorController.layers[0].stateMachine.defaultState;

            UnityEditor.Animations.BlendTree blendtree;
            blendtree
        }*/

        /*public void AddLayer(int layerIndex)
        {
            if (-1 != m_StateLayers.FindIndex(l => l != null && l.layerIndex == layerIndex))
            {
                Debug.LogErrorFormat("layer:{0} has existed!", layerIndex);
                return;
            }

            var layer = new StateLayer(layerIndex, m_Graph, m_Params);

            int emptyIndex = m_StateLayers.FindIndex(l => l == null);
            if (emptyIndex != -1)
            {
                m_StateLayers[emptyIndex] = layer;
            }
            else
            {
                m_StateLayers.Add(layer);
                m_LayerMixer.SetInputCount(m_StateLayers.Count);
            }

            layer.SetPlayableOutput(0, layer.layerIndex, m_LayerMixer);
        }

        public void RemoveLayer(int layerIndex)
        {
            int removeIndex = m_StateLayers.FindIndex(l => l != null && l.layerIndex == layerIndex);
            m_StateLayers[removeIndex].Release();
            m_StateLayers[removeIndex] = null;
        }*/

        public StateLayer GetLayer(int layerIndex)
        {
            if (!CheckLayerIfExist(layerIndex)) return null;
            return stateLayers[layerIndex];
        }

        /*public void SetLayerWeight(int layerIndex, float weight)
        {
            if (!CheckLayerIfExist(layerIndex)) return;
            StateLayer layer = m_StateLayers[layerIndex];
            layer.weight = Mathf.Clamp01(weight);
            layer.isLayerWeightDirty = true;
        }*/

        private bool CheckLayerIfExist(int layerIndex)
        {
            if (layerIndex >= stateLayers.Count || layerIndex < 0)
            {
                Debug.LogErrorFormat("Layer index:{0} is out of range!", layerIndex);
                return false;
            }

            StateLayer layer = stateLayers[layerIndex];
            if (layer == null)
            {
                Debug.LogErrorFormat("Layer index:{0} has destroyed!", layerIndex);
                return false;
            }

            return true;
        }

        /*private void UpdateLayers(float deltaTime)
        {
            for (int i = 0; i < m_StateLayers.Count; i++)
            {
                StateLayer layer = m_StateLayers[i];
                if (layer == null) continue;

// 更新权重
                if (layer.isLayerWeightDirty)
                {
                    m_LayerMixer.SetInputWeight(layer.layerIndex, layer.weight);
                    layer.isLayerWeightDirty = false;
                }

// 更新同步层
                if (layer.isLayerSyncDirty)
                {
                    int syncLayerIndex = layer.SyncLayerIndex;
                    StateLayer syncLayer = m_StateLayers[syncLayerIndex];
                    if (layer == null)
                    {
                        Debug.LogErrorFormat("SyncLayer:{0} is not exist!", syncLayerIndex);
                    }
                    else
                    {
                        if (syncLayer.needSyncLayers == null)
                        {
                            syncLayer.needSyncLayers = new List<StateLayer>();
                        }

                        syncLayer.needSyncLayers.Add(layer);
                    }

                    layer.isLayerSyncDirty = false;
                }
            }
        }*/
        [ContextMenu("CreateLayer")]
        public void CreateLayer()
        {
            StateLayer layer = ScriptableObject.CreateInstance(typeof(StateLayer)) as StateLayer;
            layer.name = "StateLayer";
            layer.guid = GUID.Generate().ToString();
            Undo.RecordObject(this, "CreateStateMachine");

            if (this.stateLayers==null)
            {
                this.stateLayers = new List<StateLayer>();
            }
            this.stateLayers.Add(layer);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(layer, this);
            }

            Undo.RegisterCreatedObjectUndo(layer, "CreateStateMachine");

            AssetDatabase.SaveAssets();
        }
    }
}