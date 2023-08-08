using UnityEngine;
using UnityEngine.AI;
using XenoIK.Runtime.Enum;

namespace XenoIK.Runtime.Ground
{
    public class GroundLeg
    {
        public Vector3 IKPosition;
        public Vector3 Velocity;
        public Quaternion IKRotation;
        public Transform FootTrans;
        public bool Inited;
        public float HeightFromGround;
        public float FootOffset;

        #region Hit

        //射线
        private RaycastHit m_FootHit;

        //Navmesh
        private float m_NavPosOffset = 0.001f;
        private NavMeshHit m_FootNavHit;
        private NavMeshHit m_ForwardNavHit;
        private NavMeshHit m_RightNavHit;

        #endregion
        
        private GroundSolver m_GroundSolver;
        private Quaternion m_HitNormalR, m_FinalRotation;
        private Vector3 m_LastPosition;
        private Vector3 Up;
        private Vector3 m_FootPosition; //脚部坐标
        private Vector3 m_HitPoint = Vector3.zero;
        private Vector3 m_HitNormal = Vector3.up;
        private float m_LastTime, m_DeltaTime;

        public void InitLeg(GroundSolver solver, Transform foot)
        {
            this.Inited = false;
            this.m_GroundSolver = solver;
            this.FootTrans = foot;
            this.IKPosition = this.FootTrans.position;
            this.m_LastPosition = this.FootTrans.position;
            this.m_LastTime = Time.deltaTime;
            this.Inited = true;
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

            //根据检测类型
            switch (this.m_GroundSolver.CastType)
            {
                case RayCastType.Phyics:
                    GetPhycisCast(prediction);
                    break;
                case RayCastType.Navmesh:
                    GetNavmeshCast(prediction);
                    break;
                case RayCastType.Mix:
                    if (!GetPhycisCast(prediction))
                    {
                        GetNavmeshCast(prediction);
                    }
                    break;
            }
            
            GetHeightRotation(m_HitNormal, m_HitPoint);
            
            float offsetTarget = this.HeightFromGround;
            
            if (!this.m_GroundSolver.RootGrounded) offsetTarget = 0f;
            
            //插值
            this.FootOffset = XenoTools.LerpValue(this.FootOffset, offsetTarget, this.m_GroundSolver.FootSpeed, this.m_GroundSolver.FootSpeed);
            this.FootOffset = Mathf.Lerp(this.FootOffset, offsetTarget, this.m_DeltaTime * this.m_GroundSolver.FootSpeed);

            //当前帧源动画的抬脚高度
            float legHeight = this.m_GroundSolver.GetVerticalDist(this.m_FootPosition, m_GroundSolver.Root.position);
            float maxOffset = Mathf.Clamp(this.m_GroundSolver.MaxStep - legHeight, 0f, this.m_GroundSolver.MaxStep);
            //IK不启用时抬脚高度在不能高于源动画的高度
            this.FootOffset = Mathf.Clamp(this.FootOffset, -maxOffset, FootOffset);
            
            //获取脚部旋转
            Quaternion footDeltaR = Quaternion.RotateTowards(Quaternion.identity, this.m_HitNormalR, this.m_GroundSolver.MaxFootRotation);
            this.m_FinalRotation = Quaternion.Slerp(this.m_FinalRotation, footDeltaR, this.m_DeltaTime * this.m_GroundSolver.FootRotationSpeed);

            //返回IK Position和Rotation
            this.IKPosition = this.m_FootPosition- this.Up * FootOffset;
            this.IKRotation = this.m_FinalRotation;
            
#if UNITY_EDITOR
            switch (this.m_GroundSolver.CastType)
            {
                case RayCastType.Phyics:
                    Debug.DrawLine(this.IKPosition, this.IKPosition + m_FootHit.normal * 0.5f, Color.cyan);
                    break;
                case RayCastType.Navmesh:
                    Debug.DrawLine(this.IKPosition, this.IKPosition + m_FootHit.normal * 0.5f, Color.cyan);
                    break;
                case RayCastType.Mix:
                    break;
            }
#endif
            
        }

        /// <summary>
        /// 使用物理射线检测
        /// </summary>
        /// <param name="prediction"></param>
        /// <returns></returns>
        private bool GetPhycisCast(Vector3 prediction)
        {
            this.m_FootHit = this.GetRayHit(prediction);

            if (this.m_FootHit.collider == null)
            {
                this.m_HitPoint = this.m_FootPosition;
                this.m_HitNormal = this.Up;
                return false;
            }
            this.m_HitPoint = m_FootHit.point;
            this.m_HitNormal = m_FootHit.normal;
            
            return true;
        }

        /// <summary>
        /// 使用Navmesh采样
        /// </summary>
        /// <param name="prediction"></param>
        /// <returns></returns>
        private void GetNavmeshCast(Vector3 prediction)
        {
            this.m_FootNavHit = this.GetNavmeshHit(prediction, true);

            if (!this.m_FootNavHit.hit)
            {
                this.m_HitPoint = this.m_FootPosition;
                this.m_HitNormal = this.Up;
                return;
            }
            m_HitPoint = m_FootNavHit.position;
            m_HitNormal = m_FootNavHit.normal;
        }
        
        private NavMeshHit GetNavmeshHit(Vector3 predictionOffset, bool computeNormal = false)
        {
            NavMeshHit hit = new NavMeshHit();
            
            Vector3 origin = this.m_FootPosition + predictionOffset;
            if (this.m_GroundSolver.MaxStep <= 0f) return hit;

            Vector3 samplePos = origin + this.Up * this.m_GroundSolver.MaxStep;
            if (NavMesh.SamplePosition(samplePos, out hit, this.m_GroundSolver.MaxStep * 2f, NavMesh.AllAreas) && computeNormal)
            {
                //因为NavHit不计算法线，所以需要手动计算法线
                hit.normal = Vector3.up;
                var fw = NavMesh.SamplePosition(samplePos + this.m_GroundSolver.Root.forward * this.m_NavPosOffset,
                    out this.m_ForwardNavHit, this.m_GroundSolver.MaxStep * 2f, NavMesh.AllAreas);
                var rt = NavMesh.SamplePosition(samplePos + this.m_GroundSolver.Root.right * this.m_NavPosOffset,
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
        
        private RaycastHit GetRayHit(Vector3 predictionOffset)
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
            
#if UNITY_EDITOR
            Debug.DrawLine(capsuleStart, hit.point, Color.red);
#endif

            return hit;
        }
        
        private void GetHeightRotation(Vector3 hitNormal, Vector3 hitPoint)
        {
            this.m_HitNormalR = Quaternion.FromToRotation(this.Up, hitNormal);
            Vector3 point = XenoTools.LineToPlane(this.m_FootPosition, -this.Up, hitNormal, hitPoint);
            this.HeightFromGround = m_GroundSolver.GetVerticalDist(this.m_FootPosition, point) 
                - this.m_GroundSolver.GetVerticalDist(this.m_FootPosition, this.m_GroundSolver.Root.position) 
                + this.m_GroundSolver.HeelOffset;
            this.HeightFromGround = Mathf.Clamp(HeightFromGround, -this.m_GroundSolver.MaxStep, this.m_GroundSolver.MaxStep);
        }
        
    }
}