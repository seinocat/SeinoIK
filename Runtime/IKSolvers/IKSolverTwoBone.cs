using System;
using System.Collections.Generic;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public class IKSolverTwoBone : IKSolver
    {
        public Transform target;
        public Transform pole;
        
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
        
        const float k_SqrEpsilon = 1e-8f;
        
        /// <summary>
        /// 算法参考 https://theorangeduck.com/page/simple-two-joint
        /// </summary>
        private void Solve()
        {
            Bone jointA = this.TwoBoneList[0];
            Bone jointB = this.TwoBoneList[1];
            Bone jointC = this.TwoBoneList[2];
            Vector3 vecAB = jointB.Position - jointA.Position;
            Vector3 vecBC = jointC.Position - jointB.Position;
            Vector3 vecAC = jointC.Position - jointA.Position;
            Vector3 vecAT = this.target.position - jointA.Position;
            
            float lenAB = vecAB.magnitude;
            float lenBC = vecBC.magnitude;
            float lenAC = vecAC.magnitude;
            float lenAT = vecAT.magnitude;

            float oldABCAngle = CosineTriangle(lenAC, lenAB, lenBC) ;
            float newABCAngle = CosineTriangle(lenAT, lenAB, lenBC) ;

            Vector3 axis = Vector3.Cross(vecAB, vecBC);
            if (axis.sqrMagnitude < k_SqrEpsilon)
            {
                axis = Vector3.zero;
                
                if (axis.sqrMagnitude < k_SqrEpsilon)
                    axis = Vector3.Cross(vecAT, vecBC);

                if (axis.sqrMagnitude < k_SqrEpsilon)
                    axis = Vector3.up;
            }

            axis = axis.normalized;
            //
            // float a = 0.5f * (oldABCAngle - newABCAngle);
            // float sin = Mathf.Sin(a);
            // float cos = Mathf.Cos(a);
            // Quaternion deltaRotate = new Quaternion(axis.x * sin, axis.y * sin, axis.z * sin, cos);

            jointB.Rotation = Quaternion.AngleAxis((oldABCAngle - newABCAngle) * Mathf.Rad2Deg, axis) * jointB.Rotation;
            vecAC = jointC.Position - jointA.Position;
            jointA.Rotation = Quaternion.FromToRotation(vecAC, vecAT) * jointA.Rotation;

            // jointC.Rotation = target.rotation;
        }
        
        private float CosineTriangle(float sideA, float sideB, float sideC)
        {
            float cosA = Mathf.Clamp((sideB * sideB + sideC * sideC - sideA * sideA) / (2 * sideB * sideC), -1f, 1f);
            return Mathf.Acos(cosA);
        }
        
        
    }
}