using System;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public abstract class IKSolver
    {
        protected abstract void OnInitialize();
        protected abstract void OnUpadete();
        
        public bool Initiated { get; private set; }
        
        public Vector3 IKPosition;

        [Range(0, 1f)]
        public float IKWeight = 1f;
        
        public void Update()
        {
            OnUpadete();
        }

        public void Init()
        {
            OnInitialize();
            Initiated = true;
        }
        
    
    }
}