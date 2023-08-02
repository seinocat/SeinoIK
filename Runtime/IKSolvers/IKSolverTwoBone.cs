using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace XenoIK
{
    public enum SolverType
    {
        Animation,
        Goal
    }
    
    [Serializable]
    public class TwoBone : Bone
    {
        private Quaternion m_TargetToLocalSpace;
        private Vector3 m_DefaultLocalBendNormal;

        public void Init(Vector3 childPos, Vector3 bendNormal)
        {
            Quaternion r = Quaternion.LookRotation(childPos - Position, bendNormal);
            this.m_TargetToLocalSpace = Quaternion.Inverse(r) * this.Rotation;
            this.m_DefaultLocalBendNormal = this.ToLocalRotation * bendNormal;
        }

        public Quaternion GetRotation(Vector3 dir, Vector3 bendNormal)
        {
            return Quaternion.LookRotation(dir, bendNormal) * this.m_TargetToLocalSpace;
        }

        /// <summary>
        /// 获取世界坐标下BendNormal
        /// </summary>
        /// <returns></returns>
        public Vector3 GetBendNormal()
        {
            return this.Rotation * this.m_DefaultLocalBendNormal;
        }
    }
    
    [Serializable]
    public class IKSolverTwoBone : IKSolver
    {
        public Transform target;
        public TwoBone Bone1;
        public TwoBone Bone2;
        public TwoBone Bone3;
        public SolverType SolverType = SolverType.Animation;
        
        protected override void OnInitialize()
        {
            if (this.BendNormal == Vector3.zero) this.BendNormal = Vector3.right;

            OnInit();
            this.IKPosition = Bone3.Position;
            InitBones();
        }

        
        protected override void OnUpdate(float deltaTime)
        {
            if (this.target != null) this.IKPosition = this.target.position;

            if (this.SolverType == SolverType.Animation)
            {
                this.OnUpdate();
                this.Solve();
                this.OnPostSolve();
            }
            else
            {
                this.Solve2();
            }
           
        }
        
        public List<TwoBone> GetBones()
        {
            return new List<TwoBone>() { this.Bone1, this.Bone2, this.Bone3 };
        }
        
        public override void StoreDefaultLocalState()
        {
            Bone1.StoreDefaultLocalState();
            Bone2.StoreDefaultLocalState();
            Bone3.StoreDefaultLocalState();
        }

        public override void FixTransform()
        {
            Bone1.FixTransform();
            Bone2.FixTransform();
            Bone3.FixTransform();
        }

        #region 解算器1
        
        public Vector3 BendNormal;
        [Range(0f, 1f)]
        public float BendModifierWieght = 1f;

        private Vector3 m_WeightIKPosition;
        private bool m_HasMaintainBend;
        private Vector3 m_BendNormal, m_AnimationNormal;

        protected void OnInit()
        {
            Vector3 normal = Vector3.Cross(Bone2.Position - Bone1.Position, Bone3.Position - Bone2.Position);
            if (normal != Vector3.zero) BendNormal = normal;

            this.m_AnimationNormal = BendNormal;
        }

        protected void OnUpdate()
        {
            if (IKWeight > 0)
            {
                this.m_BendNormal = BendNormal;
                BendNormal = GetBendNormal();
            }
            
        }

        protected void OnPostSolve()
        {
            BendNormal = this.m_BendNormal;
        }



        private void InitBones()
        {
            Bone1.Init(Bone2.Position, BendNormal);
            Bone2.Init(Bone3.Position, BendNormal);
            SetBendPlaneToCurrent();
        }
        
        public void SetBendPlaneToCurrent()
        {
            if (!this.initiated) return;

            Vector3 normal = Vector3.Cross(Bone2.Position - Bone1.Position, Bone3.Position - Bone2.Position);
            if (normal != Vector3.zero) BendNormal = normal;
        }
        
        /// <summary>
        /// 解算器1：参考Final IK的算法实现 
        /// </summary>
        private void Solve()
        {
            if (this.IKWeight == 0) return;
        
            Bone1.sqrMag = (Bone2.Position - Bone1.Position).sqrMagnitude;
            Bone2.sqrMag = (Bone3.Position - Bone2.Position).sqrMagnitude;
        
            this.m_WeightIKPosition = Vector3.Lerp(Bone3.Position, IKPosition, IKWeight);
            
            Vector3 curBendNormal = Vector3.Lerp(Bone1.GetBendNormal(), BendNormal, this.IKWeight);
            Vector3 bendDir = Vector3.Lerp(Bone2.Position - Bone1.Position,
                GetBendDir(m_WeightIKPosition, curBendNormal), IKWeight);
        
            if (bendDir == Vector3.zero) bendDir = Bone2.Position - Bone1.Position;
        
            Bone1.Rotation = Bone1.GetRotation(bendDir, curBendNormal);
            Bone2.Rotation = Bone2.GetRotation(m_WeightIKPosition - Bone2.Position, Bone2.GetBendNormal());
        }
        
        private Vector3 GetBendDir(Vector3 pos, Vector3 normal)
        {
            Vector3 dir = pos - Bone1.Position;
            if (dir == Vector3.zero) return Vector3.zero;

            float dirSqlMag = dir.sqrMagnitude;
            float dirMag = dir.magnitude;

            float x = (dirSqlMag + Bone1.sqrMag - Bone2.sqrMag) / (2f * dirMag);
            float y = Mathf.Sqrt(Mathf.Clamp(Bone1.sqrMag - x * x, 0, Mathf.Infinity));
            
            Vector3 yDir = Vector3.Cross(dir.normalized, normal);
            return Quaternion.LookRotation(dir, yDir) * new Vector3(0f, y, x);
        }
        
        private void MaintainBend()
        {
            if (!this.initiated) return;

            this.m_AnimationNormal = Bone1.GetBendNormal();
            this.m_HasMaintainBend = true;
        }

        private Vector3 GetBendNormal()
        {
            if (!this.m_HasMaintainBend) MaintainBend();
            this.m_HasMaintainBend = false;

            return Vector3.Lerp(BendNormal, m_AnimationNormal, BendModifierWieght);
        }

        #endregion

        #region 解算器2
        
        [ShowIf("SolverType", SolverType.Goal)]
        public Transform poleTarget;
        
        /// <summary>
        /// 解算器2: 算法参考
        /// 1. Unity Animation Rigging
        /// 2. https://theorangeduck.com/page/simple-two-joint
        /// </summary>
        private void Solve2()
        {
            Bone jointA = this.Bone1;
            Bone jointB = this.Bone2;
            Bone jointC = this.Bone3;
            
            Vector3 vecAB = jointB.Position - jointA.Position;
            Vector3 vecBC = jointC.Position - jointB.Position;
            Vector3 vecAC = jointC.Position - jointA.Position;
            Vector3 vecAT = this.IKPosition - jointA.Position;
            
            float lenAB = vecAB.magnitude;
            float lenBC = vecBC.magnitude;
            float lenAC = vecAC.magnitude;
            float lenAT = vecAT.magnitude;
        
            float oldABCAngle = CosineTriangle(lenAC, lenAB, lenBC) ;
            float newABCAngle = CosineTriangle(lenAT, lenAB, lenBC) ;
        
            Vector3 axis = Vector3.Cross(vecAB, vecBC);
            if (axis.sqrMagnitude < Mathf.Epsilon)
            {
                if (axis.sqrMagnitude <  Mathf.Epsilon)
                    axis = Vector3.Cross(vecAT, vecBC);
                if (axis.sqrMagnitude <  Mathf.Epsilon)
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

        #endregion
    }
}