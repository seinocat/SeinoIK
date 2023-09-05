using System;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public class IKSolverCCD : IKSolverHeuristic
    {
        protected override void OnInitialize()
        {
            this.IKPosition = this.bones[this.bones.Count - 1].Position;
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
            var lastBone = this.bones[this.bones.Count - 1];
            for (int i = this.bones.Count - 2; i >= 0; i--)
            {
                var curBone = this.bones[i];
                float weight = curBone.weight * this.IKWeight;
                if (weight <= 0) continue;

                Vector3 toEffectorVec = lastBone.Position - curBone.Position;
                Vector3 toTargetVec = this.IKPosition - curBone.Position;
                
                // 另一种写法，使用轴-角旋转
                // Vector3 axis = Vector3.Cross(toEffectorVec, toTargetVec).normalized;
                // float angle = Vector3.Angle(toEffectorVec, toTargetVec);
                // Quaternion qua = Quaternion.AngleAxis(angle, axis) * curBone.transform.rotation;
                // curBone.transform.rotation = qua;
                
                // 四元数直接算
                Quaternion finalR = Quaternion.FromToRotation(toEffectorVec, toTargetVec) * curBone.Rotation;
                curBone.Rotation = weight >= 1 ? finalR : Quaternion.Lerp(curBone.Rotation, finalR, weight);
            }
        }
        
    }
}