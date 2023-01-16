using System;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public abstract class IKSolver
    {
        protected abstract void OnInitialize();
        protected abstract void OnUpdate(float deltaTime);
        public bool initiated { get; private set; }
        
        public Vector3 IKPosition;

        [Range(0, 1f)]
        public float IKWeight = 1f;
        
        public void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        public void Init()
        {
            OnInitialize();
            this.initiated = true;
        }
        
    
    }
}