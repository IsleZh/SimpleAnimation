using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace Isle.AnimationMachine
{
    [CreateAssetMenu(fileName = "Blend2D Asset", menuName = "CreatePlayableAsset/Blend/AnimGraph_Blend2D", order = 2)]
    public class BlendTree_2D : BlendTree
    {
        struct MotionNeighborList
        {
            public int count;
            public int[] neighborArray;
        }

        /// <summary>
        /// 2D Blend结点数据类
        /// </summary>
        [Serializable]
        class Blend2dDataConstant
        {
            public int childCount;

            public Vector2[] childPositionArray;

            public int childMagnitudeCount;

            /// <summary>
            /// 采样点Direction的模长，Used by type 1
            /// </summary>
            public float[] childMagnitudeArray;

            public int childPairVectorCount;

            /// <summary>
            /// 极坐标向量，Used by type 2
            /// </summary>
            public Vector2[] childPairVectorArray;

            public int childPairAvgMagInvCount;

            /// <summary>
            /// 采样点模长均值的倒数，Used by type 1
            /// </summary>
            public float[] childPairAvgMagInvArray;

            public int childNeighborListCount;

            /// <summary>
            /// NeighborLiast，Used by type 2
            /// </summary>
            public MotionNeighborList[] childNeighborListArray;

            public Blend2DType blend2DType;
        }

        public Blend2DSampleClipInfo[] clips;

        public Blend2DType blend2DType = Blend2DType.SimpleDirectional;

        [SerializeField] Blend2dDataConstant constantData;
        [ContextMenu("PrecomputeFreeformData")]
        public void PrecomputeFreeformData()
        {
            constantData = new Blend2dDataConstant();
            var childCount = clips.Length;
            constantData.childCount = childCount;
            if (childCount > 0)
            {
                constantData.childPositionArray = new Vector2[childCount];
                for (int i = 0; i < childCount; i++)
                {
                    constantData.childPositionArray[i] = clips[i].dirPos;
                }
            }

            if (blend2DType == Blend2DType.FreedomDirectional || blend2DType == Blend2DType.FreedomCartesian)
            {
                // Populate blend 2d precomputed data for type FreeformDirectionnal2D or FreeformCartesian2D
                if (blend2DType == Blend2DType.FreedomDirectional)
                {
                    constantData.childMagnitudeCount = childCount;
                    constantData.childMagnitudeArray = new float[childCount];
                }

                constantData.childPairAvgMagInvCount = childCount * childCount;
                constantData.childPairVectorCount = childCount * childCount;
                constantData.childNeighborListCount = childCount;
                constantData.childPairAvgMagInvArray = new float[constantData.childPairAvgMagInvCount];
                constantData.childPairVectorArray = new Vector2[constantData.childPairVectorCount];
                constantData.childNeighborListArray = new MotionNeighborList[constantData.childNeighborListCount];
                PrecomputeFreeform(blend2DType);
            }
        }

        void PrecomputeFreeform(Blend2DType type)
        {
            int count = constantData.childCount;
            Vector2[] positionArray = constantData.childPositionArray;
            float[] constantMagnitudes = constantData.childMagnitudeArray;
            Vector2[] constantChildPairVectors = constantData.childPairVectorArray;
            float[] constantChildPairAvgMagInv = constantData.childPairAvgMagInvArray;
            MotionNeighborList[] constantChildNeighborLists = constantData.childNeighborListArray;

            if (type == Blend2DType.FreedomDirectional)
            {
                for (int i = 0; i < count; i++)
                    constantData.childMagnitudeArray[i] = positionArray[i].magnitude;
                for (int i = 0; i < count; i++)
                {
                    for (int j = 0; j < count; j++)
                    {
                        int pairIndex = i + j * count;
                        // 计算每一对采样点模长的均值
                        float magSum = constantMagnitudes[j] + constantMagnitudes[i];
                        if (magSum > 0)
                            constantChildPairAvgMagInv[pairIndex] = 2.0f / magSum;
                        else
                            constantChildPairAvgMagInv[pairIndex] = 2.0f / magSum;
                        // 采样点的距离处理平均模长
                        float mag = (constantMagnitudes[j] - constantMagnitudes[i]) *
                                    constantChildPairAvgMagInv[pairIndex];
                        if (constantMagnitudes[j] == 0 || constantMagnitudes[i] == 0)
                            constantChildPairVectors[pairIndex] = new Vector2(0, mag);
                        else
                        {
                            float angle = Vector2.Angle(positionArray[i], positionArray[j]);
                            if (positionArray[i].x * positionArray[j].y - positionArray[i].y * positionArray[j].x < 0)
                                angle = -angle;
                            constantChildPairVectors[pairIndex] = new Vector2(angle, mag);
                        }
                    }
                }
            }
            else if (type == Blend2DType.FreedomCartesian)
            {
                for (int i = 0; i < count; i++)
                {
                    for (int j = 0; j < count; j++)
                    {
                        int pairIndex = i + j * count;
                        constantChildPairAvgMagInv[pairIndex] = 1 / (positionArray[j] - positionArray[i]).sqrMagnitude;
                        constantChildPairVectors[pairIndex] = positionArray[j] - positionArray[i];
                    }
                }
            }

            float[] weightArray = new float[count];
            int[] cropArray = new int[count];
            bool[] neighborArray = new bool[count * count];
            for (int c = 0; c < count * count; c++)
                neighborArray[c] = false;

            float minX = 10000.0f;
            float maxX = -10000.0f;
            float minY = 10000.0f;
            float maxY = -10000.0f;
            for (int c = 0; c < count; c++)
            {
                minX = Mathf.Min(minX, positionArray[c].x);
                maxX = Mathf.Max(maxX, positionArray[c].x);
                minY = Mathf.Min(minY, positionArray[c].y);
                maxY = Mathf.Max(maxY, positionArray[c].y);
            }

            //可采样区域是样本区域的一倍
            float xRange = (maxX - minX) * 0.5f;
            float yRange = (maxY - minY) * 0.5f;
            minX -= xRange;
            maxX += xRange;
            minY -= yRange;
            maxY += yRange;

            for (int i = 0; i <= 100; i++)
            {
                for (int j = 0; j <= 100; j++)
                {
                    float x = i * 0.01f;
                    float y = j * 0.01f;
                    if (type == Blend2DType.FreedomDirectional)
                        GetWeightsFreeformDirectional(ref weightArray, ref cropArray, minX * (1 - x) + maxX * x,
                            minY * (1 - y) + maxY * y, true);
                    else if (type == Blend2DType.FreedomCartesian)
                        GetWeightsFreeformCartesian(ref weightArray, ref cropArray, minX * (1 - x) + maxX * x,
                            minY * (1 - y) + maxY * y, true);
                    for (int c = 0; c < count; c++)
                        if (cropArray[c] >= 0)
                            neighborArray[c * count + cropArray[c]] = true;
                }
            }

            for (int i = 0; i < count; i++)
            {
                List<int> nList = new List<int>();
                for (int j = 0; j < count; j++)
                    if (neighborArray[i * count + j])
                        nList.Add(j);

                constantChildNeighborLists[i].count = nList.Count;
                constantChildNeighborLists[i].neighborArray = new int[nList.Count];

                for (int d = 0; d < nList.Count; d++)
                    constantChildNeighborLists[i].neighborArray[d] = nList[d];
            }
        }
        

        int[] copyArray = new int[0];

        public void GetWeights(ref float[] weightArray, float blendValueX, float blendValueY)
        {
            switch (blend2DType)
            {
                case Blend2DType.SimpleDirectional:
                    GetWeightsSimpleDirectional(ref weightArray, blendValueX, blendValueY);
                    break;
                case Blend2DType.FreedomDirectional:
                    GetWeightsFreeformDirectional(ref weightArray, ref copyArray, blendValueX, blendValueY);
                    break;
                case Blend2DType.FreedomCartesian:
                    GetWeightsFreeformCartesian(ref weightArray, ref copyArray, blendValueX, blendValueY);
                    break;
            }
        }

        #region 计算SimpleDirectional权重

        void GetWeightsSimpleDirectional(ref float[] weightArray, float blendValueX, float blendValueY)
        {
            int count = constantData.childCount;
            if (count <= 0)
                return;
            if (weightArray == null || weightArray.Length != count)
                weightArray = new float[count];
            Vector2[] positionArray = constantData.childPositionArray;

            //只有两个Sample的时候
            if (count < 2)
            {
                if (count == 1)
                    weightArray[0] = 1;
                return;
            }

            Vector2 blendPosition = new Vector2(blendValueX, blendValueY);
            if (blendPosition == Vector2.zero)
            {
                for (int i = 0; i < count; i++)
                {
                    if (positionArray[i] == Vector2.zero)
                    {
                        weightArray[i] = 1;
                        return;
                    }
                }

                float sharedWeight = 1.0f / count;
                for (int i = 0; i < count; i++)
                    weightArray[i] = sharedWeight;
                return;
            }

            //找到BlendPos所在三个点的三角形索引
            int indexA = -1;
            int indexB = -1;
            int indexCenter = -1;
            float maxDotForNegCross = -100000.0f;
            float maxDotForPosCross = -100000.0f;
            for (int i = 0; i < count; i++)
            {
                //将原点采样设置为中心点
                if (positionArray[i] == Vector2.zero)
                {
                    if (indexCenter >= 0)
                        return;
                    indexCenter = i;
                    continue;
                }

                Vector2 posNormalized = positionArray[i].normalized;
                float dot = Vector2.Dot(posNormalized, blendPosition);
                float cross = posNormalized.x * blendPosition.y - posNormalized.y * blendPosition.x;
                //左
                if (cross > 0)
                {
                    if (dot > maxDotForPosCross)
                    {
                        maxDotForPosCross = dot;
                        indexA = i;
                    }
                }
                //右
                else
                {
                    if (dot > maxDotForNegCross)
                    {
                        maxDotForNegCross = dot;
                        indexB = i;
                    }
                }
            }

            float centerWeight = 0;

            if (indexA < 0 || indexB < 0)
            {
                // Fallback if sampling point is not inside a triangle
                centerWeight = 1;
            }
            else
            {
                Vector2 a = positionArray[indexA];
                Vector2 b = positionArray[indexB];

                // Calculate weights using barycentric coordinates
                // (formulas from http://en.wikipedia.org/wiki/Barycentric_coordinate_system_%28mathematics%29 )
                float det = b.y * a.x - b.x * a.y; // Simplified from: (b.y-0)*(a.x-0) + (0-b.x)*(a.y-0);
                float wA = (b.y * blendValueX - b.x * blendValueY) /
                           det; // Simplified from: ((b.y-0)*(l.x-0) + (0-b.x)*(l.y-0)) / det;
                float wB = (a.x * blendValueY - a.y * blendValueX) /
                           det; // Simplified from: ((0-a.y)*(l.x-0) + (a.x-0)*(l.y-0)) / det;
                centerWeight = 1 - wA - wB;

                // Clamp to be inside triangle
                if (centerWeight < 0)
                {
                    centerWeight = 0;
                    float sum = wA + wB;
                    wA /= sum;
                    wB /= sum;
                }
                else if (centerWeight > 1)
                {
                    centerWeight = 1;
                    wA = 0;
                    wB = 0;
                }

                // Give weight to the two vertices on the periphery that are closest
                weightArray[indexA] = wA;
                weightArray[indexB] = wB;
            }

            if (indexCenter >= 0)
            {
                weightArray[indexCenter] = centerWeight;
            }
            else
            {
                // Give weight to all children when input is in the center
                float sharedWeight = 1.0f / count;
                for (int i = 0; i < count; i++)
                    weightArray[i] += sharedWeight * centerWeight;
            }
        }

        #endregion

        #region 计算FreeformDirectional权重

        const float kInversePI = 1 / Mathf.PI;

        float GetWeightFreeformDirectional(Blend2dDataConstant blendConstant, Vector2[] workspaceBlendVectors, int i,
            int j, Vector2 blendPosition)
        {
            int pairIndex = i + j * blendConstant.childCount;
            Vector2 vecIJ = blendConstant.childPairVectorArray[pairIndex];
            Vector2 vecIO = workspaceBlendVectors[i];
            vecIO.y *= blendConstant.childPairAvgMagInvArray[pairIndex];

            if (blendConstant.childPositionArray[i] == Vector2.zero)
                vecIJ.x = workspaceBlendVectors[j].x;
            else if (blendConstant.childPositionArray[j] == Vector2.zero)
                vecIJ.x = workspaceBlendVectors[i].x;
            else if (vecIJ.x == 0 || blendPosition == Vector2.zero)
                vecIO.x = vecIJ.x;

            return 1 - Vector2.Dot(vecIJ, vecIO) / vecIJ.sqrMagnitude;
        }

        private void GetWeightsFreeformDirectional(ref float[] weightArray, ref int[] cropArray, float blendValueX,
            float blendValueY, bool preCompute = false)
        {
            // Get constants
            Vector2[] positionArray = constantData.childPositionArray;
            int count = constantData.childCount;
            float[] constantMagnitudes = constantData.childMagnitudeArray;
            MotionNeighborList[] constantChildNeighborLists = constantData.childNeighborListArray;
            Vector2 blendPosition = new Vector2(blendValueX, blendValueY);
            float magO = blendPosition.magnitude;
            Vector2[] workspaceBlendVectors = new Vector2[count];

            if (blendPosition == Vector2.zero)
            {
                for (int i = 0; i < count; i++)
                    workspaceBlendVectors[i] = new Vector2(0, magO - constantMagnitudes[i]);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if (positionArray[i] == Vector2.zero)
                        workspaceBlendVectors[i] = new Vector2(0, magO - constantMagnitudes[i]);
                    else
                    {
                        float angle = Vector2.Angle(positionArray[i], blendPosition);
                        if (positionArray[i].x * blendPosition.y - positionArray[i].y * blendPosition.x < 0)
                            angle = -angle;
                        workspaceBlendVectors[i] = new Vector2(angle, magO - constantMagnitudes[i]);
                    }
                }
            }

            if (preCompute)
            {
                for (int i = 0; i < count; i++)
                {
                    // Fade out over 180 degrees away from example
                    float value = 1 - Mathf.Abs(workspaceBlendVectors[i].x) * kInversePI;
                    cropArray[i] = -1;
                    for (int j = 0; j < count; j++)
                    {
                        if (i == j)
                            continue;

                        float newValue =
                            GetWeightFreeformDirectional(constantData, workspaceBlendVectors, i, j, blendPosition);

                        if (newValue <= 0)
                        {
                            value = 0;
                            cropArray[i] = -1;
                            break;
                        }

                        // Used for determining neighbors
                        if (newValue < value)
                            cropArray[i] = j;
                        value = Mathf.Min(value, newValue);
                    }
                }

                return;
            }

            for (int i = 0; i < count; i++)
            {
                // Fade out over 180 degrees away from example
                float value = 1 - Mathf.Abs(workspaceBlendVectors[i].x) * kInversePI;
                for (int jIndex = 0; jIndex < constantChildNeighborLists[i].count; jIndex++)
                {
                    int j = constantChildNeighborLists[i].neighborArray[jIndex];
                    float newValue =
                        GetWeightFreeformDirectional(constantData, workspaceBlendVectors, i, j, blendPosition);
                    if (newValue <= 0)
                    {
                        value = 0;
                        break;
                    }

                    value = Mathf.Min(value, newValue);
                }

                weightArray[i] = value;
            }

            // Normalize weights
            float summedWeight = 0;
            for (int i = 0; i < count; i++)
                summedWeight += weightArray[i];

            if (summedWeight > 0)
            {
                summedWeight = 1.0f / summedWeight; // Do division once instead of for every sample
                for (int i = 0; i < count; i++)
                    weightArray[i] *= summedWeight;
            }
            else
            {
                // Give weight to all children as fallback when no children have any weight.
                // This happens when sampling in the center if no center motion is provided.
                float evenWeight = 1.0f / count;
                for (int i = 0; i < count; i++)
                    weightArray[i] = evenWeight;
            }
        }

        #endregion

        #region 计算FreedomCartesian权重

        void GetWeightsFreeformCartesian(ref float[] weightArray, ref int[] cropArray, float blendValueX,
            float blendValueY, bool preCompute = false)
        {
            // Get constants
            Vector2[] positionArray = constantData.childPositionArray;
            int count = constantData.childCount;
            MotionNeighborList[] constantChildNeighborLists = constantData.childNeighborListArray;
            Vector2[] workspaceBlendVectors = new Vector2[count];
            Vector2 blendPosition = new Vector2(blendValueX, blendValueY);
            for (int i = 0; i < count; i++)
                workspaceBlendVectors[i] = blendPosition - positionArray[i];

            if (preCompute)
            {
                for (int i = 0; i < count; i++)
                {
                    cropArray[i] = -1;
                    Vector2 vecIO = workspaceBlendVectors[i];
                    float value = 1;
                    for (int j = 0; j < count; j++)
                    {
                        if (i == j)
                            continue;

                        int pairIndex = i + j * constantData.childCount;
                        Vector2 vecIJ = constantData.childPairVectorArray[pairIndex];
                        float newValue = 1 - Vector2.Dot(vecIJ, vecIO) *
                            constantData.childPairAvgMagInvArray[pairIndex];
                        if (newValue <= 0)
                        {
                            value = 0;
                            cropArray[i] = -1;
                            break;
                        }

                        // Used for determining neighbors
                        if (newValue < value)
                            cropArray[i] = j;
                        value = Mathf.Min(value, newValue);
                    }
                }

                return;
            }

            for (int i = 0; i < count; i++)
            {
                Vector2 vecIO = workspaceBlendVectors[i];
                float value = 1;
                for (int jIndex = 0; jIndex < constantChildNeighborLists[i].count; jIndex++)
                {
                    int j = constantChildNeighborLists[i].neighborArray[jIndex];
                    if (i == j)
                        continue;

                    int pairIndex = i + j * constantData.childCount;
                    Vector2 vecIJ = constantData.childPairVectorArray[pairIndex];
                    float newValue = 1 - Vector2.Dot(vecIJ, vecIO) * constantData.childPairAvgMagInvArray[pairIndex];
                    if (newValue < 0)
                    {
                        value = 0;
                        break;
                    }

                    value = Mathf.Min(value, newValue);
                }

                weightArray[i] = value;
            }

            // Normalize weights
            float summedWeight = 0;
            for (int i = 0; i < count; i++)
                summedWeight += weightArray[i];
            summedWeight = 1.0f / summedWeight; // Do division once instead of for every sample
            for (int i = 0; i < count; i++)
                weightArray[i] *= summedWeight;
        }

        #endregion

    }

    [Serializable]
    public class Blend2DSampleClipInfo
    {
        public AnimationClip clip;

        public Vector2 dirPos;
    }

    public enum Blend2DType
    {
        /// <summary>
        /// Sample点中要有原点、且同Direction有且只有有一个Sample点
        /// </summary>
        SimpleDirectional,

        /// <summary>
        /// 
        /// </summary>
        FreedomDirectional,

        /// <summary>
        /// 
        /// </summary>
        FreedomCartesian
    }
}