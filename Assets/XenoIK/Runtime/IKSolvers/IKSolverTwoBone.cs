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
        
        /// <summary>
        /// 算法参考 https://theorangeduck.com/page/simple-two-joint
        /// </summary>
        private void Solve()
        {
            Bone jointA = this.TwoBoneList[0];
            Bone jointB = this.TwoBoneList[1];
            Bone jointC = this.TwoBoneList[2];
            Vector3 vecAB = (jointB.Position - jointA.Position).normalized;
            Vector3 vecAC = (jointC.Position - jointA.Position).normalized;
            Vector3 vecBC = (jointC.Position - jointB.Position).normalized;
            Vector3 vecAT = (this.IKPosition - jointA.Position).normalized;
            
            float lengthAB = (jointA.Position - jointB.Position).magnitude;
            float lengthCB = (jointB.Position - jointC.Position).magnitude;
            float lengthAT = Mathf.Clamp((jointA.Position - this.IKPosition).magnitude, Mathf.Epsilon, lengthAB + lengthCB - Mathf.Epsilon);

            float angleBAC = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(vecAB, vecAC));
            float angleABC = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(-vecAB, vecBC));
            float angleTAC = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(vecAT, vecAC));

            float targetAngleBAC = Mathf.Rad2Deg * CosineTriangle(lengthCB, lengthAT, lengthAB);
            float targetAngleABC = Mathf.Rad2Deg * CosineTriangle(lengthAT, lengthCB, lengthAB);

            // Vector3 fixVec = (jointB.Rotation * new Vector3(0, 0, 1)).normalized;
            Vector3 axis1 = Vector3.Cross(vecAC, vecAB).normalized;
            Vector3 axis2 = Vector3.Cross(vecAT, vecAC).normalized;
            
            Quaternion rotateBAC = Quaternion.AngleAxis(targetAngleBAC - angleBAC, jointA.LocalRotation * axis1);
            Quaternion rotateABC = Quaternion.AngleAxis(targetAngleABC - angleABC, jointB.LocalRotation * axis1);
            Quaternion rotateTAC = Quaternion.AngleAxis(angleTAC, jointA.LocalRotation * axis2);
            
            jointA.LocalRotation = (rotateTAC * rotateBAC) * jointA.LocalRotation;
            jointB.LocalRotation = rotateABC * jointB.LocalRotation;
        }
        
        private float CosineTriangle(float sideA, float sideB, float sideC)
        {
            float cosA = Mathf.Clamp((sideB * sideB + sideC * sideC - sideA * sideA) / (2 * sideB * sideC), -1f, 1f);
            return Mathf.Acos(cosA);
        }
        
        
    }
}