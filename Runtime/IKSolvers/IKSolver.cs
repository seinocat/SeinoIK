using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SeinoIK
{
    [Serializable]
    public abstract class IKSolver
    {
        public bool initiated { get; private set; }
        [Title("通用设置")]
        [LabelText("目标")]
        public Vector3 IKPosition;
        [LabelText("权重"), Range(0, 1f)]
        public float IKWeight = 1f;
        [HideInInspector]
        public Transform root;
        
        public void Update(float deltaTime)
        {
            if (!this.initiated) return;

            OnPreUpdate?.Invoke();
            OnUpdate(deltaTime);
            OnPostUpdate?.Invoke();
        }

        public void Init(Transform root)
        {
            if (this.root == null) this.root = root;
            OnInitialize();
            this.initiated = true;
            StoreDefaultLocalState();
        }
        
        protected abstract void OnInitialize();
        protected abstract void OnUpdate(float deltaTime);
        public abstract void StoreDefaultLocalState();
        public abstract void FixTransform();
        
        
        public Action OnPreUpdate;
        public Action OnPostUpdate;
    }
}