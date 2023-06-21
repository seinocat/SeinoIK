using System;
using System.Collections.Generic;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public class IKSolverTwoBone : IKSolver
    {
        public Transform target;
        public Transform poleTarget;
        public List<Bone> twoBoneList = new List<Bone>() { new Bone(), new Bone(), new Bone() };
        public const float sqrEpsilon = 1e-8f;

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
        /// 算法参考
        /// 1. Unity Animation Rigging
        /// 2. https://theorangeduck.com/page/simple-two-joint
        /// </summary>
        private void Solve()
        {
            Bone jointA = this.twoBoneList[0];
            Bone jointB = this.twoBoneList[1];
            Bone jointC = this.twoBoneList[2];
            
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
            if (axis.sqrMagnitude < sqrEpsilon)
            {
                if (axis.sqrMagnitude < sqrEpsilon)
                    axis = Vector3.Cross(vecAT, vecBC);
                if (axis.sqrMagnitude < sqrEpsilon)
                    axis = Vector3.up;
            }

            axis = axis.normalized;
            
            jointB.Rotation = Quaternion.AngleAxis(oldABCAngle - newABCAngle, axis) * jointB.Rotation;
            vecAC = jointC.Position - jointA.Position;
            jointA.Rotation = Quaternion.FromToRotation(vecAC, vecAT) * jointA.Rotation;

            if (poleTarget != null)
            {
                float acSqrLen = vecAC.sqrMagnitude;
                if (acSqrLen > 0)
                {
                    vecAB = jointB.Position - jointA.Position;
                    vecAC = jointC.Position - jointA.Position;
                    Vector3 acNormal = vecAC / Mathf.Sqrt(acSqrLen);
                    //极向量
                    Vector3 poleVec = this.poleTarget.position - jointA.Position;
                    //求出jointB在AC上的投影向量
                    Vector3 vecJointB = vecAB - acNormal * Vector3.Dot(vecAB, acNormal);
                    //求出极向量在AC上的投影向量
                    Vector3 vecPole = poleVec - acNormal * Vector3.Dot(poleVec, acNormal);

                    float maxReach = lenAB + lenBC;
                    if (vecJointB.sqrMagnitude > maxReach * maxReach * 0.001f)
                    {
                        //将jointB朝向旋转到极向量的朝向
                        Quaternion poleR = Quaternion.FromToRotation(vecJointB, vecPole);
                        jointA.Rotation = poleR * jointA.Rotation;
                    }
                }
            }
        }
        
        /// <summary>
        /// Acos求出为弧度，需要转换成角度
        /// </summary>
        /// <param name="sideA"></param>
        /// <param name="sideB"></param>
        /// <param name="sideC"></param>
        /// <returns></returns>
        private float CosineTriangle(float sideA, float sideB, float sideC)
        {
            float cosA = Mathf.Clamp((sideB * sideB + sideC * sideC - sideA * sideA) / (2 * sideB * sideC), -1f, 1f);
            return Mathf.Acos(cosA) * Mathf.Rad2Deg;
        }
        
        
    }
}