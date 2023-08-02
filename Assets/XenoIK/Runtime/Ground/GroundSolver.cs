﻿using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace XenoIK.Runtime.Ground
{
    [Serializable]
    public class GroundSolver
    {
        [Range(-1f, 1f)]
        public float HeelOffset; 

        [LabelText("使用Navmesh")] 
        public bool UseNavmesh;

        [LabelText("层级")]
        public LayerMask Layers;
        
        [LabelText("预测速度")]
        public float Prediction = 0.05f;

        [LabelText("脚部最大高度")] 
        public float MaxStep = 0.5f;

        [LabelText("脚部速度")]
        public float FootSpeed = 2.5f;

        [LabelText("脚部半径")] 
        public float FootRadius = 0.1f;

        [LabelText("脚部最大角度"), Range(0, 90f)]
        public float MaxFootRotation = 50f;
        
        [LabelText("脚部旋转速度")]
        public float FootRotationSpeed = 7f;

        [LabelText("旋转解算器")] 
        public bool RotateSolver = true;

        [LabelText("骨盆移动速度")] 
        public float PelvisSpeed = 1f;

        [LabelText("骨盆阻尼"), Range(0f, 1f)]
        public float PelvisDamper;

        [Range(0f, 1f)]
        public float LowWeight = 1f;
        
        [Range(0f, 1f)]
        public float HighWeight = 0f;

        [Range(0f, 1f)]
        public float RootSphereCastRadius = 0.1f;
        
        public bool IsGrounded;
        
        public Transform Root { get; private set; }
        
        public List<GroundLeg> Legs;
        public GroundPelvis Pelvis;
        
        private RaycastHit m_RootRayHit;
        private NavMeshHit m_RootNavHit;
        
        public bool RootGrounded => (this.UseNavmesh ? m_RootNavHit.distance : m_RootRayHit.distance) < this.MaxStep * 2f;

        public Vector3 Up => this.UseRootRotation ? this.Root.up : Vector3.up;
        
        public Vector3 FootCenterOffset => this.Root.forward * this.FootRadius;

        private bool m_Inited;

        public bool UseRootRotation
        {
            get
            {
                if (!this.RotateSolver) return false;
                return this.Root.up != Vector3.up;
            }
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

        public void InitSolver(Transform root, List<Transform> feet)
        {
            this.Root = root;
            this.m_Inited = false;

            this.m_RootRayHit = new RaycastHit();
            this.Legs = new List<GroundLeg>();
            this.Pelvis = new GroundPelvis();


            for (int i = 0; i < feet.Count; i++)
            {
                GroundLeg leg = new GroundLeg();
                leg.InitLeg(this, feet[i]);
                this.Legs.Add(leg);
            }
            
            this.Pelvis.InitPelvis(this);
            this.m_Inited = true;
        }

        public void Update()
        {
            if (!this.m_Inited) return;

            if (this.UseNavmesh)
            {
                this.m_RootNavHit = GetRootNavHit();
            }
            else
            { 
                this.m_RootRayHit = GetRootRayHit();
            }
            
            this.IsGrounded = false;
            float lowOffset = float.MinValue;
            float highOffset = float.MaxValue;

            foreach (var leg in this.Legs)
            {
                leg.Solve();
                if (leg.IKOffset > lowOffset) lowOffset = leg.IKOffset;
                if (leg.IKOffset < highOffset) highOffset = leg.IKOffset;

                if (leg.IsGounded) this.IsGrounded = true;
            }

            lowOffset = Mathf.Max(lowOffset, 0f);
            highOffset = Mathf.Min(highOffset, 0f);
            
            this.Pelvis.Solve(-lowOffset * this.LowWeight, -highOffset * this.HighWeight, this.IsGrounded);
        }
    }
}