using UnityEngine;

namespace XenoIK.Runtime.Ground
{
    public class GroundPelvis
    {
        public Vector3 PelvisOffset;
        
        private GroundSolver m_GroundSover;
        private bool m_Inited;
        private float m_HighOffset;

        public void InitPelvis(GroundSolver solver)
        {
            this.m_GroundSover = solver;
            this.m_Inited = true;
        }
        
        public void Solve(float lowOffset, float heightOffset)
        {
            if (!this.m_Inited) return;
            
            float offset = lowOffset + heightOffset;
            if (!m_GroundSover.RootGrounded) offset = 0f;

            this.m_HighOffset = Mathf.Lerp(this.m_HighOffset, offset, Time.deltaTime * m_GroundSover.PelvisSpeed);
            this.PelvisOffset = this.m_GroundSover.Up * this.m_HighOffset;
        }
        

    }
}