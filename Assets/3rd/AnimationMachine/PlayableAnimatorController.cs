using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace Isle.AnimationMachine
{
    public class PlayableAnimatorController : ScriptableObject
    {
        private Animator animator;
        private AnimatorController animatorController;
        private List<StateLayer> m_StateLayers;

        public List<StateLayer> layers
        {
            get => m_StateLayers;
            set => m_StateLayers = value;
        }

        public AnimatorControllerParameter[] parameters;
        AnimatorState xxx;

        /*public void Test()
        {
            animator = new Animator();
            animatorController.layers;
            animatorController.parameters;
            animatorController.layers[0].stateMachine.defaultState;

            UnityEditor.Animations.BlendTree blendtree;
            blendtree
        }*/
/*
public void AddLayer(int layerIndex)
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
}
#1#

public StateLayer GetLayer(int layerIndex)
{
if (!CheckLayerIfExist(layerIndex)) return null;
return m_StateLayers[layerIndex];
}

public void SetLayerWeight(int layerIndex, float weight)
{
if (!CheckLayerIfExist(layerIndex)) return;
StateLayer layer = m_StateLayers[layerIndex];
layer.weight = Mathf.Clamp01(weight);
layer.isLayerWeightDirty = true;
}

private bool CheckLayerIfExist(int layerIndex)
{
if (layerIndex >= m_StateLayers.Count || layerIndex < 0)
{
Debug.LogErrorFormat("Layer index:{0} is out of range!", layerIndex);
return false;
}

StateLayer layer = m_StateLayers[layerIndex];
if (layer == null)
{
Debug.LogErrorFormat("Layer index:{0} has destroyed!", layerIndex);
return false;
}

return true;
}

private void UpdateLayers(float deltaTime)
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
    }
}