using SeinoIK.Runtime.Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SeinoIK
{
    [HideMonoScript]
    public class LookAtCtrl : MonoBehaviour
    {
        [LabelText("启用")]
        public bool EnableIK = true;
        [LabelText("IK解算器"), Required]
        public LookAtIK lookAtIK;
        [LabelText("目标")]
        public Transform Target;
        [LabelText("配置包"), Required]
        public LookAtConfig IKConfig;
        
        #region Private paramters
        
        private Transform m_Head;
        private Transform m_LastTarget;
        private Vector3 m_IkPosition;
        private Vector3 m_LastPosition;
        private Vector3 m_Direction;
        private float m_RuntimeWeight = 1f;
        private float m_SwitchWeight;
        private float m_SmoothSpeed;
        private float m_SwitchSpeed;
        private bool m_Watching;

        private Vector3 Pivot => this.IKConfig != null ? lookAtIK.transform.position + lookAtIK.transform.rotation * this.IKConfig.pivotOffset : Vector3.up;

        private IKSolverLookAt Solver => this.lookAtIK?.solver;

        #endregion
        
        /// <summary>
        /// Hold target and revert runtimeWeight value
        /// </summary>
        public void Enable()
        {
            this.EnableIK = true;
        }

        /// <summary>
        /// Set runtimeWeight to 0
        /// </summary>
        public void Disable()
        {
            this.EnableIK = false;
        }

        public void ResetTarget()
        {
            this.Target = null;
            this.lookAtIK.solver.IKPosition = Vector3.zero;
        }

        public void SetTarget(Transform target)
        {
            this.Target = target;
        }

        /// <summary>
        /// Look at point via create a temp gameobject
        /// </summary>
        /// <param name="point"></param>
        public void SetLookAtPoint(Vector3 point)
        {
            m_IkPosition = point;
        }

        public void UpdateAxis()
        {
            if (IKConfig == null) return;

            this.Solver.headAxis = this.IKConfig.headAxis;
            this.Solver.eyesAxis = this.IKConfig.eyesAxis;
            this.Solver.spinesAxis = this.IKConfig.spinesAxis;
            this.Solver.UpdateAxis();
        }
        
        private void Awake()
        {
            if (this.lookAtIK == null) this.lookAtIK = this.GetComponent<LookAtIK>();
            if (this.lookAtIK != null) this.UpdateAxis();
        }
        

        private void Start()
        {
            if (this.IKConfig == null)
            {
                Debug.LogError("The IK Config is null, this Ik component will not work!");
                this.Solver.IKWeight = 0;
                return;
            }
            
            this.m_LastPosition = this.Solver.IKPosition;
            this.m_Direction = this.Solver.IKPosition - this.Pivot;
            this.IKConfig.defaultWeight = this.m_RuntimeWeight;
            this.m_Head = this.Solver.head.transform;
        }
        
        /// <summary>
        /// 范围检测
        /// </summary>
        /// <returns></returns>
        private bool CheckRange()
        {
            float angleLimit = (this.m_Watching ? this.IKConfig.followAngleXZ.x : this.IKConfig.detectAngleXZ.x);
            Vector2 aimTargetHorVec = new Vector2(m_IkPosition.x, m_IkPosition.z);
            Vector2 forwardVec = new Vector2(this.Solver.head.Forward.x, this.Solver.head.Forward.z);
            Vector2 TargetHorVector = aimTargetHorVec - new Vector2(this.m_Head.position.x, this.m_Head.position.z);
            
            var distance = TargetHorVector.magnitude;
            if (distance > this.IKConfig.maxDistance || distance < this.IKConfig.minDistance)
                return false;
            
            var targetAngle = Vector2.Angle(forwardVec, TargetHorVector);
            return !(targetAngle > (angleLimit / 2));
        }
        
        private void LateUpdate()
        {
            if (this.IKConfig == null || this.m_Head == null) return;

            if (this.Target != null) m_IkPosition = this.Target.position;

            if (this.Target != this.m_LastTarget)
            {
                if (this.Target != null && this.m_LastTarget == null && this.Solver.IKWeight <= 0f)
                {
                    this.m_LastPosition = m_IkPosition;
                    this.m_Direction = m_IkPosition - this.Pivot;
                    this.Solver.IKPosition = m_IkPosition + this.IKConfig.offset;
                }
                else
                {
                    this.m_LastPosition = this.Solver.IKPosition;
                    this.m_Direction = this.Solver.IKPosition - this.Pivot;
                }

                this.m_SwitchWeight = 0f;
                this.m_LastTarget = this.Target;
            }

            if (!this.EnableIK || !CheckRange())
            {
                this.m_LastPosition = this.Solver.head.Position + this.Solver.root.forward * 0.5f;
                this.m_RuntimeWeight = 0;
            }
            else
                this.m_RuntimeWeight = this.IKConfig.defaultWeight;
            
            float ikWeight = this.Target == null ? 0f : this.m_RuntimeWeight;
            this.m_Watching = ikWeight > 0 && this.Target != null;
            this.Solver.IKWeight = Mathf.SmoothDamp(this.Solver.IKWeight, ikWeight, ref this.m_SmoothSpeed, this.IKConfig.smoothWeightTime);
            if (!this.m_Watching)
            {
                this.Solver.IKPosition = this.Solver.head.Position + this.Solver.head.Forward * 0.5f;
            }
            
            if (this.Solver.IKWeight <= 0) return;
            if (this.Solver.IKWeight >= 0.999f && ikWeight > this.Solver.IKWeight) this.Solver.IKWeight = 1f;
            if (this.Solver.IKWeight <= 0.001f && ikWeight < this.Solver.IKWeight) this.Solver.IKWeight = 0f;

            this.m_SwitchWeight = Mathf.SmoothDamp(m_SwitchWeight, ikWeight, ref this.m_SwitchSpeed, this.IKConfig.switchWeightTime);
            
            if (this.m_SwitchWeight >= 0.999f) this.m_SwitchWeight = 1f;
            if (this.m_SwitchWeight <= 0.001f) this.m_SwitchWeight = 0f;
            
            if (this.Target != null)
                this.Solver.IKPosition = Vector3.Lerp(this.m_LastPosition, m_IkPosition + this.IKConfig.offset, this.m_SwitchWeight);

            Vector3 targetDir = this.Solver.IKPosition - this.Pivot;
            this.m_Direction = Vector3.Slerp(this.m_Direction, targetDir, Time.deltaTime * this.IKConfig.lookAtSpeed);
            this.m_Direction = Vector3.RotateTowards(this.m_Direction, targetDir, Time.deltaTime * this.IKConfig.maxRadiansDelta, this.IKConfig.maxMagnitudeDelta);
            this.Solver.IKPosition = this.m_Direction + this.Pivot;
        }
    }
}