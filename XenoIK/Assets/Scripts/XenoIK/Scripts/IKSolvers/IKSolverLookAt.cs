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

        [Range(0, 360)]
        public float angle;
        [Range(0, float.MaxValue)]
        public float distance;
        [Range(0, float.MaxValue)]
        public float fadeInTime;
        [Range(0, float.MaxValue)]
        public float fadeOutTime;
        public AnimationCurve fadeInCurve = new AnimationCurve(new Keyframe(0,0,1,1), new Keyframe(1,1,1,1));
        public AnimationCurve fadeOutCurve = new AnimationCurve(new Keyframe(0,0,1,1), new Keyframe(1,1,1,1));


        protected override void OnInitialize()
        {
            this.head?.Init(this.root);
            this.spines?.ForEach(x=>x.Init(this.root));
            this.eyes?.ForEach(x=>x.Init(this.root));
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (this.IKWeight <= 0) return;
            if (this.target != null) this.IKPosition = this.target.position;

            SolveHead();
        }

        public override void StoreDefaultLocalState()
        {
            if (this.head.transform != null) this.head.StoreDefaultLocalState();
            this.spines.ForEach(x=>x.StoreDefaultLocalState());
            this.eyes.ForEach(x=>x.StoreDefaultLocalState());
        }

        public override void FixTransform()
        {
            if (!this.initiated) return;
            if (this.IKWeight <= 0) return;
            
            if (this.head.transform != null) this.head.FixTransform();
            this.spines?.ForEach(x=>x.FixTransform());
            this.eyes.ForEach(x=>x.FixTransform());
        }

        private void SolveHead()
        {
            if (this.headWeight <= 0) return;
            if (this.head.transform == null) return;
            float weight = this.headWeight * this.IKWeight;

            Vector3 baseForward = this.spines.Count > 0 ? this.spines.FindLastBone().Forward : this.head.Forward;
            Vector3 targetForward = Vector3.Lerp(baseForward, (this.IKPosition - this.head.Position).normalized, weight).normalized;
            
            this.head.LookAt(targetForward, weight);
        }
    }
}