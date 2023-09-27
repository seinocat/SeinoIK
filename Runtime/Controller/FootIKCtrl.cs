using UnityEngine;
using XenoIK.Runtime.Enum;
using XenoIK.Runtime.Ground;

namespace XenoIK
{
    public class FootIKCtrl : MonoBehaviour
    {
        public FootIK FootIK;

        private float m_Speed;
        private float m_Weight;
        private float m_Time;
        private bool m_SetWeight;

        private void Awake()
        {
            if (FootIK == null)
            {
                this.FootIK = this.GetComponentInChildren<FootIK>();
            }
        }

        private void Update()
        {
            if (FootIK == null) return;
            if (m_SetWeight)
            {
                this.FootIK.Weight = Mathf.SmoothDamp(this.FootIK.Weight, this.m_Weight, ref m_Speed, this.m_Time);
                if (Mathf.Abs(this.FootIK.Weight - this.m_Weight) <= 0.01f)
                {
                    m_Speed = 0;
                    m_SetWeight = false;
                } 
            }
        }

        public void EnableIK()
        {
            if (FootIK == null) return;
            SetWeight(1);
        }

        public void DisableIK()
        {
            if (FootIK == null) return;
            SetWeight(0);
        }

        public void SetWeight(float weight, float duratime = 0.25f)
        {
            if (FootIK == null) return;
            this.m_SetWeight = true;
            this.m_Weight = Mathf.Clamp01(weight);
            this.m_Time = duratime;
        }

        public void SetPrediction(float speed)
        {
            if (FootIK == null) return;
            speed = Mathf.Max(0, speed);
            this.FootIK.GroundSolver.Prediction = speed;
        }

        public void SetMaxStep(float high)
        {
            if (FootIK == null) return;
            high = Mathf.Max(0, high);
            this.FootIK.GroundSolver.MaxStep = high;
        }

        public void SetRayCastType(RayCastType type)
        {
            if (FootIK == null) return;
            this.FootIK.GroundSolver.CastType = type;
        }
        
        public void SetLerpType(LerpType type)
        {
            if (FootIK == null) return;
            this.FootIK.GroundSolver.LerpType = type;
        }
    }
}