using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public abstract class IKSolver
    {
        protected abstract void OnUpadete();

        [Range(0, 1f)]
        public float IKWeight = 1f;
        
        private void Update()
        {
            OnUpadete();
        }
        

        
    
    }
}