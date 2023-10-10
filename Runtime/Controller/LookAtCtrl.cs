using Sirenix.OdinInspector;
using UnityEngine;
using XenoIK.Runtime.Config;

namespace XenoIK
{
    [HideMonoScript]
    public class LookAtCtrl : MonoBehaviour
    {
        [LabelText("启用")]
        public bool enableIk = true;
        [LabelText("IK解算器")]
        public LookAtIK lookAtIK;
        [LabelText("目标")]
        public Transform target;
        [LabelText("配置包")]
        public LookAtConfig config;
        
        #region Private paramters

        private Transform head;
        private Transform lastTarget;
        private Transform tempTarget;
        private Vector3 lastPosition;
        private Vector3 direction;
        private float runtimeWeight = 1f;
        private float switchWeight;
        private float smoothWeightSpeed;
        private float switchWeightSpeed;
        private bool watching;

        private Vector3 Pivot => this.config != null ? lookAtIK.transform.position + lookAtIK.transform.rotation * this.config.pivotOffset : Vector3.up;

        private IKSolverLookAt Solver => this.lookAtIK?.solver;

        #endregion
        
        /// <summary>
        /// Hold target and revert runtimeWeight value
        /// </summary>
        public void EnableIK()
        {
            this.enableIk = true;
        }

        /// <summary>
        /// Set runtimeWeight to 0
        /// </summary>
        public void DisableIK()
        {
            this.enableIk = false;
        }

        public void ResetTarget()
        {
            this.target = null;
            this.lookAtIK.solver.IKPosition = Vector3.zero;
        }

        public void SetTarget(Transform target)
        {
            if (this.tempTarget != null) Destroy(this.tempTarget);
            this.target = target;
        }

        /// <summary>
        /// Look at point via create a temp gameobject
        /// </summary>
        /// <param name="point"></param>
        public void SetLookAtPoint(Vector3 point)
        {
            if(this.tempTarget == null) this.tempTarget = new GameObject("temp_target").transform;
            this.tempTarget.position = point;
            this.target = this.tempTarget;
        }

        public void UpdateAxis()
        {
            this.Solver.headAxis = this.config.headAxis;
            this.Solver.eyesAxis = this.config.eyesAxis;
            this.Solver.spinesAxis = this.config.spinesAxis;
            this.Solver.UpdateAxis();
        }
        
        private void Awake()
        {
            if (this.lookAtIK == null) this.lookAtIK = this.GetComponent<LookAtIK>();
            if (this.lookAtIK != null) this.UpdateAxis();
        }
        

        private void Start()
        {
            if (this.config == null)
            {
                Debug.LogError("The IK Config is null, this Ik component will not work!");
                this.Solver.IKWeight = 0;
                return;
            }
            
            this.lastPosition = this.Solver.IKPosition;
            this.direction = this.Solver.IKPosition - this.Pivot;
            this.config.defaultWeight = this.runtimeWeight;
            this.head = this.Solver.head.transform;
        }
        
        /// <summary>
        /// 范围检测
        /// </summary>
        /// <returns></returns>
        private bool CheckRange()
        {
            float angleLimit = (this.watching ? this.config.followAngleXZ.x : this.config.detectAngleXZ.x);
            Vector2 aimTargetHorVec = new Vector2(this.target.position.x, this.target.position.z);
            Vector2 forwardVec = new Vector2(this.Solver.head.Forward.x, this.Solver.head.Forward.z);
            Vector2 TargetHorVector = aimTargetHorVec - new Vector2(this.head.position.x, this.head.position.z);
            
            var distance = TargetHorVector.magnitude;
            if (distance > this.config.maxDistance || distance < this.config.minDistance)
                return false;
            
            var targetAngle = Vector2.Angle(forwardVec, TargetHorVector);
            return !(targetAngle > (angleLimit / 2));
        }
        
        private void LateUpdate()
        {
            if (this.config == null || this.head == null) return;
            
            if (this.target != this.lastTarget)
            {
                if (this.target != null && this.lastTarget == null && this.Solver.IKWeight <= 0f)
                {
                    this.lastPosition = this.target.position;
                    this.direction = this.target.position - this.Pivot;
                    this.Solver.IKPosition = this.target.position + this.config.offset;
                }
                else
                {
                    this.lastPosition = this.Solver.IKPosition;
                    this.direction = this.Solver.IKPosition - this.Pivot;
                }

                this.switchWeight = 0f;
                this.lastTarget = this.target;
            }

            if (!this.enableIk || !CheckRange())
            {
                this.lastPosition = this.Solver.head.Position + this.Solver.root.forward * 0.5f;
                this.runtimeWeight = 0;
            }
            else
                this.runtimeWeight = this.config.defaultWeight;
            
            float ikWeight = this.target == null ? 0f : this.runtimeWeight;
            this.watching = ikWeight > 0 && this.target != null;

            if (ikWeight == 0)
            {
                this.Solver.IKWeight = Mathf.SmoothDamp(this.Solver.IKWeight, 0, ref this.smoothWeightSpeed, this.config.smoothWeightTime);
                return;
            }
            
            this.Solver.IKWeight = Mathf.SmoothDamp(this.Solver.IKWeight, ikWeight, ref this.smoothWeightSpeed, this.config.smoothWeightTime);
            if (this.Solver.IKWeight <= 0) return;
            if (this.Solver.IKWeight >= 0.999f && ikWeight > this.Solver.IKWeight) this.Solver.IKWeight = 1f;
            if (this.Solver.IKWeight <= 0.001f && ikWeight < this.Solver.IKWeight) this.Solver.IKWeight = 0f;

            this.switchWeight = Mathf.SmoothDamp(switchWeight, ikWeight, ref this.switchWeightSpeed, this.config.switchWeightTime);
            
            if (this.switchWeight >= 0.999f) this.switchWeight = 1f;
            if (this.switchWeight <= 0.001f) this.switchWeight = 0f;
            
            if (this.target != null)
                this.Solver.IKPosition = Vector3.Lerp(this.lastPosition, this.target.position + this.config.offset, this.switchWeight);

            Vector3 targetDir = this.Solver.IKPosition - this.Pivot;
            this.direction = Vector3.Slerp(this.direction, targetDir, Time.deltaTime * this.config.lookAtSpeed);
            this.direction = Vector3.RotateTowards(this.direction, targetDir, Time.deltaTime * this.config.maxRadiansDelta, this.config.maxMagnitudeDelta);
            this.Solver.IKPosition = this.direction + this.Pivot;
        }


        private void OnDestroy()
        {
            if (this.tempTarget != null) Destroy(this.tempTarget);
        }
    }
}