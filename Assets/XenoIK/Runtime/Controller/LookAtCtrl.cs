using System;
using UnityEngine;
using XenoIK.Runtime.Config;

namespace XenoIK
{
    [RequireComponent(typeof(LookAtIK))]
    public class LookAtCtrl : MonoBehaviour
    {
        public LookAtIK lookAtIK;
        public bool enableIk = true;
        public Transform target;
        
        public LookAtConfig config;
        
        #region Private paramters

        private Transform head;
        private Transform lastTarget;
        private Transform tempTarget;
        private Vector3 lastPosition;
        private Vector3 direction;
        private float runtimeWeight = 1f;
        private float switchWeight;
        private bool watching;
        
        private Vector3 Pivot => lookAtIK.transform.position + lookAtIK.transform.rotation * this.config.pivotOffset;

        private IKSolverLookAt Solver => this.lookAtIK?.solver;

        #endregion
        
        /// <summary>
        /// Hold target and revert runtimeWeight value
        /// </summary>
        public void OpenIK()
        {
            this.enableIk = true;
        }

        /// <summary>
        /// Set runtimeWeight to 0
        /// </summary>
        public void CloseIK()
        {
            this.enableIk = false;
        }

        public void ResetTarget()
        {
            this.target = null;
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


        /// <summary>
        /// Check the target whether in range
        /// </summary>
        /// <returns></returns>
        private bool CheckRange()
        {
            if (this.target == null) return false;
            
            Vector2 angleLimit = (this.watching ? this.config.followAngleXZ : this.config.detectAngleXZ)/2;
            Vector3 targetDir = this.target.position - this.Solver.head.Position;
            
            float distance = targetDir.magnitude;
            if (distance > this.config.maxDistance || distance < this.config.minDistance) return false;

            Vector3 baseForward = this.head.position + this.Solver.RootForward;
            Debug.DrawLine(this.head.position, baseForward * 2 , Color.black);
            Vector3 dirYZ = new Vector3(0, this.target.position.y, this.target.position.z);
            Vector3 dirXZ = new Vector3(this.target.position.x, 0, this.target.position.z);
            if (Vector3.Angle(baseForward, dirXZ) > angleLimit.x || Vector3.Angle(baseForward, dirYZ) > angleLimit.y)
                return false;
            
            return true;
        }

        private void Awake()
        {
            if (this.lookAtIK == null) this.lookAtIK = this.GetComponent<LookAtIK>();
        }

        private void Start()
        {
            this.lastPosition = this.Solver.IKPosition;
            this.direction = this.Solver.IKPosition - this.Pivot;
            this.config.defaultWeight = this.runtimeWeight;
            this.head = this.Solver.head.transform;
        }

        private float durationTime = 0f;
        
        private void LateUpdate()
        {
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
                this.durationTime = 0f;
                this.lastTarget = this.target;
            }
            
            if (!this.enableIk || !this.CheckRange())
                this.runtimeWeight = 0;
            else
                this.runtimeWeight = this.config.defaultWeight;
            
            float ikWeight = this.target == null ? 0f : this.runtimeWeight;
            this.watching = ikWeight > 0 && this.target != null;
            
            this.Solver.IKWeight = Mathf.SmoothDamp(this.Solver.IKWeight, ikWeight, ref this.config.smoothWeightSpeed, this.config.smoothWeightTime);
            
            if (this.Solver.IKWeight <= 0) return;
            if (this.Solver.IKWeight >= 0.999f && ikWeight > this.Solver.IKWeight) this.Solver.IKWeight = 1f;
            if (this.Solver.IKWeight <= 0.001f && ikWeight < this.Solver.IKWeight) this.Solver.IKWeight = 0f;

            if (this.config.useCustomCurve)
            {
                AnimationCurve curve = this.watching ? this.config.lookAtCurve : this.config.lookAwayCurve;
                if (this.durationTime >= this.config.switchWeightTime) 
                    this.durationTime = this.config.switchWeightTime;
                else 
                    this.durationTime += Time.deltaTime * 0.05f;
                float curveTime = this.durationTime / this.config.switchWeightTime;
                this.switchWeight = curve.Evaluate(curveTime);
            }
            else
            {
                this.switchWeight = Mathf.SmoothDamp(switchWeight, 1f, ref this.config.switchWeightSpeed, this.config.switchWeightTime);
            }
            
            if (this.switchWeight >= 0.999f) this.switchWeight = 1f;

            if (this.target != null) 
                this.Solver.IKPosition = Vector3.Lerp(this.lastPosition, this.target.position + this.config.offset, switchWeight);

            Vector3 targetDir = this.Solver.IKPosition - this.Pivot;
            this.direction = Vector3.Slerp(this.direction, targetDir, Time.deltaTime * this.config.lookAtSpeed);
            this.direction = Vector3.RotateTowards(this.direction, targetDir, Time.deltaTime * this.config.maxRadiansDelta, this.config.maxMagnitudeDelta);
            this.Solver.IKPosition = this.direction + this.Pivot;
        }
    }
}