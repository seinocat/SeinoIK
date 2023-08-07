using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace XenoIK.Runtime.Ground
{
    public enum RayCastType
    {
        [InspectorName("物理")]
        Phyics,
        [InspectorName("导航网格")]
        Navmesh,
        [InspectorName("混合")]
        Mix
    }
    
    [Serializable]
    public class GroundSolver
    {
        [LabelText("检测类型")] 
        public RayCastType CastType = RayCastType.Navmesh;

        [LabelText("检测层级")]
        public LayerMask Layers;
        
        [LabelText("旋转解算器")] 
        public bool RotateSolver = true;
        
        [LabelText("预测速度")]
        public float Prediction = 0.05f;

        [LabelText("最小脚部速度")]
        public float MinFootSpeed = 0.01f;
        
        [LabelText("骨盆移动速度")] 
        public float PelvisSpeed = 3f;
        
        [LabelText("脚部移动速度")]
        public float FootSpeed = 3f;

        [LabelText("脚部旋转速度")]
        public float FootRotationSpeed = 7f;

        [LabelText("脚部最大偏移高度")] 
        public float MaxStep = 0.5f;
        
        [LabelText("脚部半径")] 
        public float FootRadius = 0.1f;

        [LabelText("脚部最大旋转角度"), Range(0, 90f)]
        public float MaxFootRotation = 50f;
        
        [LabelText("脚部高度偏移"), Range(-1f, 1f)]
        public float HeelOffset;

        [LabelText("自动调节高度权重")]
        public bool AutoHighWeight = true;

        [LabelText("低偏移权重"), Range(0f, 1f)]
        public float LowWeight = 1f;
        
        [LabelText("高偏移权重"), Range(0f, 1f)]
        public float HighWeight;

        [LabelText("Root检测半径"), Range(0f, 1f)]
        public float RootSphereCastRadius = 0.1f;
        
        public Transform Root { get; private set; }
        public Transform Pelvis { get; private set; }
        public float Velocity { get; private set; }
        
        public List<GroundLeg> Legs;
        public GroundPelvis PelvisSolver;
        
        private RaycastHit m_RootRayHit;
        private NavMeshHit m_RootNavHit;
        private bool m_Inited;
        
        public bool RootGrounded
        {
            get
            {
                switch (this.CastType)
                {
                    case RayCastType.Phyics:
                        return m_RootRayHit.distance < this.MaxStep * 2f;
                    case RayCastType.Navmesh:
                        return m_RootNavHit.distance < this.MaxStep * 2f;
                    case RayCastType.Mix:
                        if (m_RootRayHit.collider != null) return m_RootRayHit.distance < this.MaxStep * 2f;
                        if (m_RootNavHit.hit) return m_RootRayHit.distance < this.MaxStep * 2f;
                        return false;
                    default:
                        return false;
                }
            }
        }

        public Vector3 Up => this.UseRootRotation ? this.Root.up : Vector3.up;
        
        public Vector3 FootCenterOffset => this.Root.forward * this.FootRadius;
        
        public bool UseRootRotation
        {
            get
            {
                if (!this.RotateSolver) return false;
                return this.Root.up != Vector3.up;
            }
        }
        
        public void InitSolver(Transform root, Transform pelvis, List<Transform> feet)
        {
            this.Root = root;
            this.Pelvis = pelvis;
            this.m_Inited = false;

            this.m_RootRayHit = new RaycastHit();
            this.Legs = new List<GroundLeg>();
            this.PelvisSolver = new GroundPelvis();
            
            for (int i = 0; i < feet.Count; i++)
            {
                GroundLeg leg = new GroundLeg();
                leg.InitLeg(this, feet[i]);
                this.Legs.Add(leg);
            }
            
            this.PelvisSolver.InitPelvis(this);
            this.m_Inited = true;
        }

        public void Update()
        {
            if (!this.m_Inited) return;

            switch (this.CastType)
            {
                case RayCastType.Phyics:
                    this.m_RootNavHit = GetRootNavHit();
                    break;
                case RayCastType.Navmesh:
                    this.m_RootRayHit = GetRootRayHit();
                    break;
                case RayCastType.Mix:
                    this.m_RootNavHit = GetRootNavHit();
                    this.m_RootRayHit = GetRootRayHit();
                    break;
            }
            
            float lowOffset = float.MinValue;
            float highOffset = float.MaxValue;


            this.Velocity = 0;
            foreach (var leg in this.Legs)
            {
                leg.Solve();
                if (leg.FootOffset > lowOffset) lowOffset = leg.FootOffset;
                if (leg.FootOffset < highOffset) highOffset = leg.FootOffset;

                this.Velocity += leg.Velocity.magnitude;
            }

            this.Velocity /= this.Legs.Count;
            lowOffset = -Mathf.Max(lowOffset, 0f) * this.LowWeight;
            highOffset = -Mathf.Min(highOffset, 0f) * this.HighWeight;
            
            this.PelvisSolver.Solve(lowOffset, highOffset);
        }

        public float GetVerticalDist(Vector3 p1, Vector3 p2)
        {
            if (UseRootRotation)
            {
                var p = Quaternion.Inverse(Root.rotation) * (p1 - p2);
                return p.y;
            }
            return p1.y - p2.y;
        }


        private RaycastHit GetRootRayHit(float maxDist = 10f)
        {
            RaycastHit hit = new RaycastHit();
            Vector3 legsCenter = Vector3.zero;
            this.Legs.ForEach(x=> legsCenter += x.FootTrans.position);
            legsCenter /= this.Legs.Count;

            hit.point = legsCenter - this.Up * this.MaxStep * 10f;
            float dist = maxDist + 1f;
            hit.distance = MaxStep * dist;

            if (MaxStep <= 0) return hit;

            Physics.SphereCast(legsCenter + this.Up * MaxStep, this.RootSphereCastRadius, -this.Up, out hit, MaxStep * dist, Layers,
                QueryTriggerInteraction.Ignore);

            return hit;
        }

        private NavMeshHit GetRootNavHit(float maxDist = 10f)
        {
            NavMeshHit hit = new NavMeshHit();
            
            Vector3 legsCenter = Vector3.zero;
            this.Legs.ForEach(x=> legsCenter += x.FootTrans.position);
            legsCenter /= this.Legs.Count;

            hit.position = legsCenter - this.Up * this.MaxStep * 10f;
            hit.distance = this.MaxStep * (maxDist + 1);
            
            if (this.MaxStep <= 0) return hit;
            NavMesh.SamplePosition(legsCenter + this.Up * MaxStep, out hit, maxDist + 1, NavMesh.AllAreas);

            return hit;
        }

        
    }
}