using System;
using System.Collections.Generic;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public class IKSolverLookAt : IKSolver
    {
        public Transform target;
        public LookAtBone head;
        public List<LookAtBone> eyes = new List<LookAtBone>();
        public List<LookAtBone> spines = new List<LookAtBone>();

        [Range(0, 1f)]
        public float headWeight = 1f;
        [Range(0, 1f)]
        public float eyesWeight;
        [Range(0, 1f)]
        public float bodyWeight;
        
        protected override void OnInitialize()
        {
            this.head?.Init(this.root);
            this.spines.ForEach(x=>x.Init(this.root));
            this.eyes.ForEach(x=>x.Init(this.root));
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (this.IKWeight <= 0) return;
            if (this.target != null) this.IKPosition = this.target.position;

            SolveHead();
        }

        public override void StoreDefaultLocalState()
        {
            this.head?.StoreDefaultLocalState();
            this.spines.ForEach(x=>x.StoreDefaultLocalState());
            this.eyes.ForEach(x=>x.StoreDefaultLocalState());
        }

        public override void FixTransform()
        {
            if (!this.initiated) return;
            if (this.IKWeight <= 0) return;
            
            this.head?.FixTransform();
            this.spines.ForEach(x=>x.FixTransform());
            this.eyes.ForEach(x=>x.FixTransform());
        }

        private void SolveHead()
        {
            if (this.headWeight <= 0 || this.IKWeight <= 0) return;
            if (this.head.transform == null) return;
            float weight = this.headWeight * this.IKWeight;

            Vector3 baseForward = this.spines.Count > 0 ? this.spines.FindLastBone().Forward : this.head.Forward;
            Vector3 targetForward = Vector3.Lerp(baseForward, (this.IKPosition - this.head.Position).normalized, weight).normalized;
            
            this.head.LookAt(targetForward, weight);
        }
    }
}