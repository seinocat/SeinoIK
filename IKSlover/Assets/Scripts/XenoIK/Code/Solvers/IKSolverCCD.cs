using System;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public class IKSolverCCD : IKSolverHeuristic
    {
        protected override void OnUpadete()
        {
            if (this.IKWeight == 0)
                return;
            
            if (this.Target != null)
                this.IKPosition = this.Target.position;
            
            for (int i = 0; i < this.MaxIterations; i++)
            {
                Solve();
            }
        }

        private void Solve()
        {
            var lastBone = this.Bones[this.Bones.Count - 1];
            for (int i = this.Bones.Count - 2; i >= 0; i--)
            {
                var curBone = this.Bones[i];
                float weight = curBone.Weight * this.IKWeight;
                if (weight <= 0) continue;

                Vector3 toEffectorVec = lastBone.Position - curBone.Position;
                Vector3 toTargetVec = this.IKPosition - curBone.Position;
                
                // 另一种实现，使用轴旋转
                // Vector3 axis = Vector3.Cross(toEffectorVec, toTargetVec).normalized;
                // float angle = Vector3.Angle(toEffectorVec, toTargetVec);
                // Quaternion qua = Quaternion.AngleAxis(angle, axis) * curBone.Transform.rotation;
                // curBone.Transform.rotation = qua;
                
                // 四元数
                Quaternion finalQuater = Quaternion.FromToRotation(toEffectorVec, toTargetVec) * curBone.Transform.rotation;
                
                curBone.Transform.rotation = weight >= 1 ? finalQuater : Quaternion.Lerp(curBone.Transform.rotation, finalQuater, weight);
            }
        }
        
    }
}