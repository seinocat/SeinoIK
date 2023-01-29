using System;
using System.Collections.Generic;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public class IKSolverHeuristic : IKSolver
    {
        public int maxIterations = 5;
        public int bonesCount;
        public Transform target;
        public List<Bone> bones = new List<Bone>();
        public float chainLength;
        
        protected override void OnInitialize() { }
        protected override void OnUpdate(float deltaTime) {}
        
        
        public override void StoreDefaultLocalState()
        {
            this.bones.ForEach(x=>
            {
                if(x.transform != null) x.StoreDefaultLocalState();
            });
        }

        public override void FixTransform()
        {
            if (!this.initiated) return;
            if (this.IKWeight <= 0) return;
            
            this.bones.ForEach(x=>x.FixTransform());
        }

        protected void InitBones()
        {
            this.chainLength = 0;
            this.bonesCount = bones.Count;
            for (int i = 0; i < this.bonesCount; i++)
            {
                if (i >= bonesCount - 1) break;

                var bone = bones[i];
                var lastBone = bones[i + 1];
                bone.length = (lastBone.Position - bone.Position).magnitude;
                this.chainLength += bone.length;
            }
        }
    }
}