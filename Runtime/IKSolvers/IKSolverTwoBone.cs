using System;
using System.Collections.Generic;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public class IKSolverTwoBone : IKSolver
    {
        public Transform target;
        public List<Bone> TwoBoneList = new List<Bone>(){new Bone(), new Bone(), new Bone()};

        protected override void OnInitialize()
        {
            
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (this.target != null) this.IKPosition = this.target.position;

            this.Solve();
        }

        public override void StoreDefaultLocalState()
        {
            
        }

        public override void FixTransform()
        {
            
        }
        
        private void Solve()
        {
            float eps = 0.01f;
            Bone jointA = this.TwoBoneList[0];
            Bone jointB = this.TwoBoneList[1];
            Bone jointC = this.TwoBoneList[2];
            Vector3 vecAB = (jointB.Position - jointA.Position).normalized;
            Vector3 vecAC = (jointC.Position - jointA.Position).normalized;
            Vector3 vecBC = (jointC.Position - jointB.Position).normalized;
            Vector3 vecAT = (IKPosition - jointA.Position).normalized;
            
            float lengthAB = (jointA.Position - jointB.Position).magnitude;
            float lengthCB = (jointB.Position - jointC.Position).magnitude;
            float lengthAT = Mathf.Clamp((jointA.Position - this.IKPosition).magnitude, eps, lengthAB + lengthCB - eps);

            float angleBAC = Mathf.Rad2Deg * Mathf.Acos(Mathf.Clamp(Vector3.Dot(vecAB, vecAC), -1, 1));
            float angleABC = Mathf.Rad2Deg * Mathf.Acos(Mathf.Clamp(Vector3.Dot(-vecAB, vecBC), -1, 1));
            float angleTAC = Mathf.Rad2Deg * Mathf.Acos(Mathf.Clamp(Vector3.Dot(vecAT, vecAC), -1, 1));

            float targetAngleBAC = Mathf.Rad2Deg * Mathf.Acos(Mathf.Clamp(
                    ((lengthCB * lengthCB - lengthAB * lengthAB - lengthAT * lengthAT) / (-2 * lengthAB * lengthAT)),
                    -1, 1));
            float targetAngleABC = Mathf.Rad2Deg * Mathf.Acos(Mathf.Clamp(
                    ((lengthAT * lengthAT - lengthAB * lengthAB - lengthCB * lengthCB) / (-2 * lengthAB * lengthCB)),
                    -1, 1));

            Vector3 fixVec = (jointB.Rotation * new Vector3(0, 0, 1)).normalized;
            Vector3 axis1 = Vector3.Cross(vecAC, vecAB).normalized;
            Vector3 axis2 = Vector3.Cross(vecAT, vecAC).normalized;
            
            Quaternion rotateBAC = Quaternion.AngleAxis(targetAngleBAC - angleBAC, Quaternion.Inverse(jointA.Rotation) * axis1);
            Quaternion rotateABC = Quaternion.AngleAxis(targetAngleABC - angleABC, Quaternion.Inverse(jointB.Rotation) * axis1);
            Quaternion rotateTAC = Quaternion.AngleAxis(angleTAC, Quaternion.Inverse(jointA.Rotation) * axis2);
            
            
            jointA.LocalRotation = (rotateTAC * rotateBAC) * jointA.LocalRotation;
            jointB.LocalRotation = rotateABC * jointB.LocalRotation;

        } 
    }
}