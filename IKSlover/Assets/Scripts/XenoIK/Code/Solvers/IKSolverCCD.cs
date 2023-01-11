using UnityEngine;

namespace XenoIK
{
    public class IKSolverCCD : IKSolver
    {
        protected override void Solve()
        {
            for (int i = this.Bones.Count - 2; i >= 0; i--)
            {
                var curBone = this.Bones[i];
                Vector3 toEffectorVec = this.Effector.Position - curBone.Position;
                Vector3 toTargetVec = this.TargetTrans.transform.position - curBone.Position;
                
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