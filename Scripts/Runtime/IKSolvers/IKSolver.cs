using System;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public abstract class IKSolver
    {
        protected abstract void OnInitialize();
        protected abstract void OnUpdate(float deltaTime);
        public abstract void StoreDefaultLocalState();
        public abstract void FixTransform();
        
        public bool initiated { get; private set; }
        public bool firstInitiated = true;
        
        
        public Vector3 IKPosition;
        
        [Range(0, 1f)]
        public float IKWeight = 1f;
        
        [SerializeField][HideInInspector]
        protected Transform root;
        
        public void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        public void Init(Transform root)
        {
            this.root = root;
            this.OnInitialize();
            this.initiated = true;
            this.firstInitiated = false;
            this.StoreDefaultLocalState();
        }
        
    
    }
}