using System;
using UnityEngine;

namespace SeinoIK
{
    [Serializable]
    public class IKSolverCCD : IKSolverHeuristic
    {
        protected override void OnInitialize()
        {
            this.IKPosition = this.bones[^1].Position;
            InitBones();
        }
        
        protected override void OnUpdate(float deltaTime)
        {
            if (this.IKWeight == 0) return;
            if (this.target != null) this.IKPosition = this.target.position;
            
            for (int i = 0; i < this.maxIterations; i++)
            {
                Solve();
            }
        }
        
        /// <summary>
        /// CCD Algorithm
        /// </summary>
        private void Solve()
        {
            var lastBone = this.bones[^1];
            for (int i = this.bones.Count - 2; i >= 0; i--)
            {
                var curBone = this.bones[i];
                float weight = curBone.weight * this.IKWeight;
                if (weight <= 0) continue;

                Vector3 toEffectorVec = lastBone.Position - curBone.Position;
                Vector3 toTargetVec = this.IKPosition - curBone.Position;
                
                // 四元数直接算
                Quaternion finalR = Quaternion.FromToRotation(toEffectorVec, toTargetVec) * curBone.Rotation;
                curBone.Rotation = weight >= 1 ? finalR : Quaternion.Lerp(curBone.Rotation, finalR, weight);
            }
        }
        
    }
}