using System;
using UnityEngine;

namespace XenoIK.Runtime.Config
{
    [Serializable]
    public class IKConfig : ScriptableObject
    {
        [Range(0, 1)]
        public float defaultWeight = 1f;
    }
}