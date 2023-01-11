using System;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public class IKSolverCCD : IKSolverHeuristic
    {
        protected override void OnUpadete()
        {
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
                Vector3 toEffectorVec = lastBone.Position - curBone.Position;
                Vector3 toTargetVec = this.Target.transform.position - curBone.Position;
                
                // 轴旋转
                // Vector3 axis = Vector3.Cross(toEffectorVec, toTargetVec).normalized;
                // float angle = Vector3.Angle(toEffectorVec, toTargetVec);
                // Quaternion qua = Quaternion.AngleAxis(angle, axis) * curBone.Transform.rotation;
                // curBone.Transform.rotation = qua;
                
                // 四元数
                Quaternion qua1 = Quaternion.FromToRotation(toEffectorVec, toTargetVec) * curBone.Transform.rotation;
                curBone.Transform.rotation = qua1;
            }
        }
        
    }
}