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
        
        [Range(0, 360)]
        public float angle = 180;
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

        public bool pauseIK;
        
        
        private Transform lastTarget;
        private Vector3 lastPosition;
        private Vector3 direction;
        private float lastWeight;
        private float smoothWeightSpeed;
        private float switchWeight, switchWeightSpeed;


        private Vector3 Pivot => lookAtIK.transform.position + lookAtIK.transform.rotation * pivotOffset;

        private IKSolverLookAt Solver => this.lookAtIK?.solver;


        /// <summary>
        /// 保持target但是权重归零, 用于播放技能或者特殊动画时暂时关闭IK
        /// </summary>
        public void PauseIK()
        {
            this.pauseIK = true;
            this.lastWeight = this.weight;
        }

        public void ReopenIK()
        {
            this.pauseIK = false;
            this.weight = this.lastWeight;
        }


        private void Start()
        {
            this.lastPosition = this.Solver.IKPosition;
            this.direction = this.Solver.IKPosition - this.Pivot;
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

            
            if (this.pauseIK) this.weight = 0f;
            float ikWeight = this.target == null ? 0f : this.weight;

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