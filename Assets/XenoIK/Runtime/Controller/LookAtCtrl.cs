using UnityEngine;

namespace XenoIK
{
    [RequireComponent(typeof(LookAtIK))]
    public class LookAtCtrl : MonoBehaviour
    {
        public LookAtIK lookAtIK;

        public Transform target;

        [Range(0, 1)]
        public float weight = 1f;

        public float smoothWeightTime = 0.25f;
        public float swithWeghtTime = 0.25f;
        public float maxRadiansDelta = 3f;
        public float maxMagnitudeDelta = 3f;

        [Tooltip("When the target out of range,  hold look at within followAngle")]
        public bool holdLookAt;
        
        [Range(0, 360)]
        public float detectAngle = 120;
        [Range(0, 360)]
        public float followAngle = 120;
        
        [Range(0, 100000)]
        public float maxDistance = 10000;
        [Range(0, 100000)]
        public float minDistance = 1;
        
        [Range(0, float.MaxValue)]
        public float lookAtSpeed = 3f;
        [Range(0, float.MaxValue)]
        public float lookAwaySpeed;
        public Vector3 pivotOffset = Vector3.up;
        public Vector3 offset;
        public AnimationCurve lookAtCurve = new AnimationCurve(new Keyframe(0,0,1,1), new Keyframe(1,1,1,1));
        public AnimationCurve lookAwayCurve = new AnimationCurve(new Keyframe(0,0,1,1), new Keyframe(1,1,1,1));

        
        private Transform lastTarget;
        private Transform tempTarget;
        private Vector3 lastPosition;
        private Vector3 direction;
        private float lastWeight;
        private float smoothWeightSpeed;
        private float switchWeight, switchWeightSpeed;
        private bool enableIk = true;
        private bool watching;


        private Vector3 Pivot => lookAtIK.transform.position + lookAtIK.transform.rotation * pivotOffset;

        private IKSolverLookAt Solver => this.lookAtIK?.solver;


        /// <summary>
        /// Hold target and revert weight value
        /// </summary>
        public void OpenIK()
        {
            this.weight = this.lastWeight;
            this.enableIk = true;
        }

        /// <summary>
        /// Store weight and set weight to 0
        /// </summary>
        public void CloseIK()
        {
            this.lastWeight = this.weight;
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
        /// Detect the target whether in range
        /// </summary>
        /// <returns></returns>
        private bool DetectRange()
        {
            if (this.target == null) return false;
            
            float angle = (this.watching ? this.followAngle : this.detectAngle) / 2;

            var forwardDir = this.Solver.root.forward;
            var targetDir = this.target.position - this.Solver.root.position;

            var distance = targetDir.magnitude;
            if (distance > this.maxDistance || distance < minDistance) return false;

            var targeAngle = Vector3.Angle(forwardDir, targetDir);
            return !(targeAngle > angle);
        }

        private void Start()
        {
            this.lastPosition = this.Solver.IKPosition;
            this.direction = this.Solver.IKPosition - this.Pivot;
            this.lastWeight = this.weight;
        }
        

        private void LateUpdate()
        {
            if (this.target != this.lastTarget)
            {
                if (this.target != null && this.lastTarget == null && this.Solver.IKWeight <= 0f)
                {
                    this.lastPosition = this.target.position;
                    this.direction = this.target.position - this.Pivot;
                    this.Solver.IKPosition = this.target.position + this.offset;
                }
                else
                {
                    this.lastPosition = this.Solver.IKPosition;
                    this.direction = this.Solver.IKPosition - this.Pivot;
                }

                this.lastTarget = this.target;
            }
            
            if (!this.enableIk || !this.DetectRange())
                this.weight = 0;
            else
                this.weight = this.lastWeight;
            
            
            float ikWeight = this.target == null ? 0f : this.weight;
            this.watching = ikWeight > 0;
            
            this.Solver.IKWeight = Mathf.SmoothDamp(this.Solver.IKWeight, ikWeight, ref smoothWeightSpeed, this.smoothWeightTime);
            
            if (this.Solver.IKWeight <= 0) return;
            if (this.Solver.IKWeight >= 0.999f && ikWeight > this.Solver.IKWeight) this.Solver.IKWeight = 1f;
            if (this.Solver.IKWeight <= 0.001f && ikWeight < this.Solver.IKWeight) this.Solver.IKWeight = 0f;

            this.switchWeight = Mathf.SmoothDamp(switchWeight, 1f, ref switchWeightSpeed, this.swithWeghtTime);
            if (this.switchWeight >= 0.999f) this.switchWeight = 1f;

            if (this.target != null) 
                this.Solver.IKPosition = Vector3.Lerp(this.lastPosition, this.target.position + this.offset, switchWeight);

            var targetDir = this.Solver.IKPosition - this.Pivot;
            this.direction = Vector3.Slerp(this.direction, targetDir, Time.deltaTime * lookAtSpeed);
            this.direction = Vector3.RotateTowards(this.direction, targetDir, Time.deltaTime * this.maxRadiansDelta, this.maxMagnitudeDelta);
            this.Solver.IKPosition = this.direction + this.Pivot;
        }
    }
}