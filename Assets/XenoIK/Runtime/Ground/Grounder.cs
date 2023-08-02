using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace XenoIK.Runtime.Ground
{
    
    /// <summary>
    /// Gournding IK, Foot Solver部分由Twobone IK解算
    /// </summary>
    public class Grounder : MonoBehaviour
    {
        [LabelText("权重"), Range(0, 1f)]
        public float Weight = 1f;
        
        [LabelText("解算器设置")]
        public GroundSolver Solver;
        
        public List<TwoBoneIK> Legs;
        public Transform Pelvis;
        public Transform Root;

        private List<Transform> m_Feet;
        private List<Quaternion> m_FootRotation;
        private Vector3 m_AnimaPelvisLocalPos, m_SolvedPelvisLocalPos;
        private int m_SolvedCounts;
        private bool m_Solved;
        private bool m_Inited;

        private void Awake()
        {
            if (!this.m_Inited) this.Init();
        }

        public void Update()
        {
            if (this.Weight <= 0f) return;
            this.m_Solved = false;
        }

        private void Init()
        {
            this.m_Feet = new List<Transform>();

            this.m_FootRotation = new List<Quaternion>();
            this.Legs.ForEach(x =>
            {
                this.m_FootRotation.Add(Quaternion.identity);
                this.m_Feet.Add(x.solver.Bone3.transform);

                x.solver.OnPreUpdate += OnSolverUpdate;
                x.solver.OnPostUpdate += OnPostSolverUpdate;
                
            });

            this.m_AnimaPelvisLocalPos = this.Pelvis.localPosition;
            this.Solver.InitSolver(this.Root, this.m_Feet);
            
            this.m_Inited = true;
        }

        private void OnSolverUpdate()
        {
            if (this.Weight <= 0) return;
            if (this.m_Solved) return;

            if (this.Pelvis.localPosition != m_SolvedPelvisLocalPos) this.m_AnimaPelvisLocalPos = this.Pelvis.localPosition;
            else this.Pelvis.localPosition = this.m_AnimaPelvisLocalPos;
            
            this.Solver.Update();

            for (int i = 0; i < Legs.Count; i++)
            {
                var leg = Legs[i];
                m_FootRotation[i] = this.m_Feet[i].rotation;

                leg.solver.IKPosition = Solver.Legs[i].IKPosition;
                leg.solver.IKWeight = this.Weight;
            }

            this.Pelvis.position += this.Solver.Pelvis.IKOffset * Weight;
            this.m_Solved = true;
            this.m_SolvedCounts = 0;
        }

        private void OnPostSolverUpdate()
        {
            if (this.Weight <= 0f) return;
            this.m_SolvedCounts++;

            if (this.m_SolvedCounts < this.m_Feet.Count) return;

            this.m_Solved = false;
            for (int i = 0; i < this.m_Feet.Count; i++)
            {
                this.m_Feet[i].rotation = Quaternion.Slerp(Quaternion.identity, this.Solver.Legs[i].IKRotation, this.Weight) * m_FootRotation[i];
            }

            this.m_SolvedPelvisLocalPos = this.Pelvis.localPosition;
        }
    }
}