using System;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public abstract class IKSolver
    {
        protected abstract void OnUpadete();

        public Vector3 IKPosition;

        [Range(0, 1f)]
        public float IKWeight = 1f;
        
        private void Update()
        {
            OnUpadete();
        }
        

        
    
    }
}