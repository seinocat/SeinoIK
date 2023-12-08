using Seino.Utils;
using UnityEngine;

namespace SeinoIK.Runtime.Ground
{
    public class GroundPelvis
    {
        public Vector3 PelvisOffset;

        private GroundSolver m_GroundSover;
        private bool m_Inited;
        private float m_HighOffset;
        private float m_Veloctiy;
       

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

            if (this.m_GroundSover.LerpType == LerpType.Linear)
            {
                this.m_HighOffset = Mathf.Lerp(this.m_HighOffset, offset, Time.deltaTime * m_GroundSover.PelvisSpeed);
            }
            else
            {
                this.m_HighOffset = SeinoUtils.LerpDamper(this.m_HighOffset, offset, ref m_Veloctiy, this.m_GroundSover.OffsetDamperTime, this.m_GroundSover.LerpType);
            }
            
            this.PelvisOffset = this.m_GroundSover.Up * this.m_HighOffset;
        }
        

    }
}