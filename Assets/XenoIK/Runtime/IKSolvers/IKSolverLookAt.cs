using System;
using System.Collections.Generic;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public class IKSolverLookAt : IKSolver
    {
        public Transform target;
        public Vector3 headAxis = Vector3.forward;
        public Vector3 eyesAxis = Vector3.forward;
        public Vector3 spinesAxis = Vector3.forward;
        public LookAtBone head;
        public List<LookAtBone> eyes = new List<LookAtBone>();
        public List<LookAtBone> spines = new List<LookAtBone>();

        [Range(0, 1f)]
        public float headWeight = 0.6f;
        [Range(0, 1f)]
        public float eyesWeight = 1.0f;
        [Range(0, 1f)]
        public float bodyWeight;
        
        public Vector3 RootForward => this.root.rotation * this.headAxis;
        
        
        protected override void OnInitialize()
        {
            if (this.firstInitiated)
            {
                if (this.spines.Count > 0) this.IKPosition = this.spines.FindLastBone().Position + this.root.forward * 3f;
                else if (this.head != null) this.IKPosition = this.head.Position + this.root.forward * 3f;
                else if (this.eyes.Count > 0) this.IKPosition = this.eyes[0].Position + this.root.forward * 3f;
            }
            
            this.head?.Init(this.root, this.headAxis);
            this.spines.ForEach(x=>x.Init(this.root, this.eyesAxis));
            this.eyes.ForEach(x=>x.Init(this.root, this.spinesAxis));
        }
        
        protected override void OnUpdate(float deltaTime)
        {
            if (this.IKWeight <= 0) return;
            if (this.target != null) this.IKPosition = this.target.position;

            this.SolveHead();
            this.SolveEyes();
            this.SolveSpines();
        }

        public override void StoreDefaultLocalState()
        {
            if(this.head.transform != null) this.head.StoreDefaultLocalState();
            this.spines.ForEach(x=>
            {
                if (x.transform != null) x.StoreDefaultLocalState();
            });
            this.eyes.ForEach(x=>
            {
                if (x.transform != null) x.StoreDefaultLocalState();
            });
        }

        public override void FixTransform()
        {
            if (!this.initiated) return;
            if (this.IKWeight <= 0) return;
            
            this.head?.FixTransform();
            this.spines.ForEach(x=>x.FixTransform());
            this.eyes.ForEach(x=>x.FixTransform());
        }
        
        public void UpdateAxis()
        {
            this.head?.UpdateAxis(this.headAxis);
            this.spines.ForEach(x=>x.UpdateAxis(this.spinesAxis));
            this.eyes.ForEach(x=>x.UpdateAxis(this.eyesAxis));
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

        private void SolveEyes()
        {
            if (this.eyesWeight <= 0 || this.IKWeight <= 0) return;
            if (this.eyes.Count == 0) return;

            float weight = this.eyesWeight * this.IKWeight;

            Vector3 baseForward = this.head.Forward;
            Vector3 targetForward = Vector3.Lerp(baseForward, (this.IKPosition - baseForward).normalized, weight).normalized;
            
            this.eyes.ForEach(eye =>
            {
                if (eye.transform != null) eye.LookAt(targetForward, weight);
            });
        }

        private void SolveSpines()
        {
            if (this.bodyWeight <= 0 || this.IKWeight <= 0) return;
            if (this.spines.Count == 0) return;

            float weight = this.bodyWeight * this.IKWeight;
            
            this.spines.ForEach(spine =>
            {
                if (spine.transform == null) return;

                Vector3 targetForward = (this.IKPosition - spine.Position).normalized;
                spine.LookAt(targetForward, weight);
            });
            

        }
    }
}