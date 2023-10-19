using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace XenoIK.Runtime.Config
{
    [Serializable]
    public class IKConfig : ScriptableObject
    {
        [Title("通用设置")]
        [LabelText("全局权重"), Range(0, 1)]
        public float defaultWeight = 1f;
    }
}