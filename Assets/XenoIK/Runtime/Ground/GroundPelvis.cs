using System;
using UnityEngine;

namespace XenoIK.Runtime.Ground
{
    public class GroundPelvis
    {
        public Vector3 IKOffset;
        
        private GroundSolver m_GroundSover;
        private Vector3 m_LastRootPos;
        private float m_Damper;
        private bool m_Inited;
        private float m_LastTime;
        private float m_HighOffset;

        public void InitPelvis(GroundSolver solver)
        {
            this.m_GroundSover = solver;
            this.m_Inited = true;
            this.OnEnable();
        }

        private void OnEnable()
        {
            if (!this.m_Inited) return;
            this.m_LastRootPos = this.m_GroundSover.Root.position;
            this.m_LastTime = Time.time;
        }

        public void Solve(float lowOffset, float heightOffset, bool isGrounded)
        {
            if (!this.m_Inited) return;

            float deltaTime = Time.time - this.m_LastTime;
            m_LastTime = Time.time;
            if (deltaTime <= 0) return;

            float offset = lowOffset + heightOffset;
            if (!m_GroundSover.RootGrounded) offset = 0f;

            this.m_HighOffset = Mathf.Lerp(this.m_HighOffset, offset, deltaTime * m_GroundSover.PelvisSpeed);
            Vector3 rootDelta = this.m_GroundSover.Root.position - this.m_LastRootPos;
            this.m_LastRootPos = this.m_GroundSover.Root.position;

            this.m_Damper = XenoTools.LerpValue(m_Damper, isGrounded ? 1f : 0f, 1f, 10f);
            this.m_HighOffset -= this.m_GroundSover.GetVerticalDist(rootDelta, Vector3.zero) *
                                 this.m_GroundSover.PelvisDamper * this.m_Damper;

            this.IKOffset = this.m_GroundSover.Up * this.m_HighOffset;
        }
        

    }
}