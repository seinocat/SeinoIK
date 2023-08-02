using System;
using UnityEngine;
using UnityEngine.AI;

namespace XenoIK.Runtime.Ground
{
    public class GroundLeg
    {
        public bool IsGounded;
        public Vector3 IKPosition;
        public Quaternion IKRotation;
        public bool Inited;
        public float HeightFromGround;
        public Vector3 Velocity;
        public Transform FootTrans;
        public float IKOffset;

        #region Hit

        //射线
        private RaycastHit m_HeelHit;
        private RaycastHit m_PreHeelHit;

        //Navmesh
        private float NavPosOffset = float.Epsilon;
        private NavMeshHit m_HeelNavHit;
        private NavMeshHit m_PreHeelNavHit;
        private NavMeshHit m_ForwardNavHit;
        private NavMeshHit m_RightNavHit;

        #endregion
        
        private GroundSolver m_GroundSolver;
        private float m_LastTime, m_DeltaTime;
        private Vector3 m_LastPosition;
        private Quaternion m_HitNormalR, m_FinalRotation;
        private Vector3 Up;
        private Vector3 m_FootPosition; //脚本坐标
        private float m_HeelHeight; //脚跟高度

        public void InitLeg(GroundSolver solver, Transform foot)
        {
            this.Inited = false;
            this.m_GroundSolver = solver;
            this.FootTrans = foot;
            this.IKPosition = this.FootTrans.position;
            this.m_HeelHeight = (this.FootTrans.position - this.m_GroundSolver.Root.position).magnitude;
            this.Inited = true;

            this.OnEnable();
        }

        public void OnEnable()
        {
            if (!this.Inited) return;

            this.m_LastPosition = this.FootTrans.position;
            this.m_LastTime = Time.deltaTime;
        }

        public void Solve()
        {
            if (!this.Inited) return;
            if (this.m_GroundSolver.MaxStep <= 0f) return;

            this.m_FootPosition = this.FootTrans.position;
            this.m_DeltaTime = Time.time - this.m_LastTime;
            this.m_LastTime = Time.time;
            if (this.m_DeltaTime == 0) return;

            this.Up = this.m_GroundSolver.Up;
            this.HeightFromGround = float.MaxValue;

            this.Velocity = (this.m_FootPosition - this.m_LastPosition) / this.m_DeltaTime;
            this.m_LastPosition = this.m_FootPosition;

            //根据速度预测下一帧的落脚点
            Vector3 prediction = Velocity * this.m_GroundSolver.Prediction;
            this.IsGounded = false;

            //使用Navmesh作为地形检测
            if (this.m_GroundSolver.UseNavmesh)
            {
                this.m_HeelNavHit = this.GetNavmeshHit(Vector3.zero);
                this.m_PreHeelNavHit = this.GetNavmeshHit(prediction);
                if (this.m_HeelNavHit.hit || this.m_PreHeelNavHit.hit) this.IsGounded = true;
                SetFootToPlane(m_PreHeelNavHit.normal, m_PreHeelNavHit.position);
            }
            else
            {
                this.m_HeelHit = this.GetRaycastHit(Vector3.zero);
                this.m_PreHeelHit = this.GetCapsuleHit(prediction);
                if (this.m_HeelHit.collider != null || this.m_PreHeelHit.collider != null) this.IsGounded = true;
                SetFootToPlane(m_PreHeelHit.normal, m_PreHeelHit.point);
            }
           
            
            float offsetTarget =  Mathf.Clamp(this.HeightFromGround, -this.m_GroundSolver.MaxStep, this.m_GroundSolver.MaxStep);
            
            if (!this.m_GroundSolver.RootGrounded) offsetTarget = 0f;
            
            this.IKOffset = XenoTools.LerpValue(this.IKOffset, offsetTarget, this.m_GroundSolver.FootSpeed, this.m_GroundSolver.FootSpeed);
            this.IKOffset = Mathf.Lerp(this.IKOffset, offsetTarget, this.m_DeltaTime * this.m_GroundSolver.FootSpeed);

            float legHeight = this.m_GroundSolver.GetVerticalDist(this.m_FootPosition, m_GroundSolver.Root.position);
            float maxOffset = Mathf.Clamp(this.m_GroundSolver.MaxStep - legHeight, 0f, this.m_GroundSolver.MaxStep);
            
            this.IKOffset = Mathf.Clamp(this.IKOffset, -maxOffset, IKOffset);
            
            //获取脚部旋转
            Quaternion footDeltaR = Quaternion.RotateTowards(Quaternion.identity, this.m_HitNormalR, this.m_GroundSolver.MaxFootRotation);
            this.m_FinalRotation = Quaternion.Slerp(this.m_FinalRotation, footDeltaR, this.m_DeltaTime * this.m_GroundSolver.FootRotationSpeed);

            //返回IK Position和Rotation
            this.IKPosition = this.m_FootPosition- this.Up * IKOffset;
            this.IKRotation = this.m_FinalRotation;
            
#if UNITY_EDITOR
            if (this.m_GroundSolver.UseNavmesh)
            {
                Debug.DrawLine(this.m_PreHeelNavHit.position, this.m_PreHeelNavHit.position + Vector3.up * 0.5f, Color.red);
                Debug.DrawLine(this.m_HeelNavHit.position, this.m_HeelNavHit.position + Vector3.up * 0.5f, Color.green);
                Debug.DrawLine(this.IKPosition, this.IKPosition + m_PreHeelNavHit.normal * 0.5f, Color.cyan);
            }
            else
            {
                Debug.DrawLine(this.m_PreHeelHit.point, this.m_PreHeelHit.point + Vector3.up * 0.5f, Color.red);
                Debug.DrawLine(this.m_HeelHit.point, this.m_HeelHit.point + Vector3.up * 0.5f, Color.green);
                Debug.DrawLine(this.IKPosition, this.IKPosition + m_PreHeelHit.normal * 0.5f, Color.cyan);
            }
           
#endif
            
        }

        private NavMeshHit GetNavmeshHit(Vector3 predictionOffset)
        {
            NavMeshHit hit = new NavMeshHit();
            
            Vector3 origin = this.m_FootPosition + predictionOffset;
            if (this.m_GroundSolver.MaxStep <= 0f) return hit;

            Vector3 samplePos = origin + this.Up * this.m_GroundSolver.MaxStep;
            if (NavMesh.SamplePosition(samplePos, out hit, this.m_GroundSolver.MaxStep * 2f, NavMesh.AllAreas))
            {
                //因为NavHit不计算法线，所以需要手动计算法线
                hit.normal = Vector3.up;
                var fw = NavMesh.SamplePosition(samplePos + this.m_GroundSolver.Root.forward * this.NavPosOffset,
                    out this.m_ForwardNavHit, this.m_GroundSolver.MaxStep * 2f, NavMesh.AllAreas);
                var rt = NavMesh.SamplePosition(samplePos + this.m_GroundSolver.Root.right * this.NavPosOffset,
                    out this.m_RightNavHit, this.m_GroundSolver.MaxStep * 2f, NavMesh.AllAreas);

                if (fw && rt)
                {
                    var forwardVec = (this.m_ForwardNavHit.position - hit.position).normalized;
                    var rightVec = (this.m_RightNavHit.position - hit.position).normalized;
                    var normal = Vector3.Cross(forwardVec, rightVec).normalized;
                    hit.normal = normal;
                }
            }

            return hit;
        }
        
        private RaycastHit GetRaycastHit(Vector3 predictionOffset)
        {
            RaycastHit hit = new RaycastHit();
            Vector3 origin = this.m_FootPosition + predictionOffset;

            hit.point = origin - this.Up * this.m_GroundSolver.MaxStep;
            hit.normal = this.Up;

            if (this.m_GroundSolver.MaxStep <= 0f) return hit;

            Physics.Raycast(origin + this.Up * this.m_GroundSolver.MaxStep, -this.Up, out hit,
                this. m_GroundSolver.MaxStep * 2f, this.m_GroundSolver.Layers, QueryTriggerInteraction.Ignore);

            if (hit.point == Vector3.zero && hit.normal == Vector3.zero)
            {
                hit.point = origin - this.Up * this.m_GroundSolver.MaxStep;
            }
            
            return hit;
        }

        private RaycastHit GetCapsuleHit(Vector3 predictionOffset)
        {
            RaycastHit hit = new RaycastHit();
            Vector3 footCenter = this.m_GroundSolver.FootCenterOffset;
            Vector3 origin = this.m_FootPosition + footCenter;
            hit.point = origin - this.Up * this.m_GroundSolver.MaxStep;
            hit.normal = this.Up;

            Vector3 capsuleStart = origin + this.m_GroundSolver.MaxStep * Up;
            Vector3 capsuleEnd = capsuleStart + predictionOffset;
            
            Physics.CapsuleCast(capsuleStart, capsuleEnd, this.m_GroundSolver.FootRadius, -this.Up, out hit,
                this.m_GroundSolver.MaxStep * 2f, this.m_GroundSolver.Layers, QueryTriggerInteraction.Ignore);
            if (hit.point == Vector3.zero && hit.normal == Vector3.zero)
            {
                hit.point = origin - this.Up * this.m_GroundSolver.MaxStep;
            }

            return hit;
        }
        
        private void SetFootToPlane(Vector3 planeNormal, Vector3 planePoint)
        {
            this.m_HitNormalR = Quaternion.FromToRotation(this.Up, planeNormal);
            Vector3 point = XenoTools.LineToPlane(this.m_FootPosition + this.Up * this.m_GroundSolver.MaxStep, -this.Up, planeNormal, planePoint);
            this.HeightFromGround = this.GetHeightFromGround(point);
            this.HeightFromGround = Mathf.Clamp(HeightFromGround, -Mathf.Infinity, this.m_HeelHeight);
        }
        
        private float GetHeightFromGround(Vector3 hitPoint)
        {
            return this.m_GroundSolver.GetVerticalDist(this.m_FootPosition, hitPoint) - this.m_GroundSolver.GetVerticalDist(this.FootTrans.position, this.m_GroundSolver.Root.transform.position)
                + this.m_GroundSolver.HeelOffset;
        }
    }
}