using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using XenoIK.Runtime.Enum;

namespace XenoIK.Runtime.Ground
{
    /// <summary>
    /// Gournd Solver负责解算骨盆偏移值和落脚点坐标旋转
    /// IK Solver部分由Twobone解算
    /// </summary>
    public class FootIK : MonoBehaviour
    {
        [Title("设置")]
        [LabelText("权重"), Range(0, 1f)]
        public float Weight = 1f;

        public Transform Pelvis;
        public Transform Root;
        
        [HideLabel]
        public GroundSolver GroundSolver;
        [Title("IK Solvers")]
        public List<TwoBoneIK> IKSolvers; //目前只支持TwoBone Solver
       

        public bool Debug;

        private List<Transform> m_Feet;
        private List<Quaternion> m_FootRotation;
        private Vector3 m_AnimaPelvisLocalPos, m_SolvedPelvisLocalPos;
        private float m_LastRootY;
        private int m_IKSolvedCounts;
        private bool m_Solved;
        private bool m_Inited;
        private float m_WeightVelocity;
        

        private void Awake()
        { 
            this.Init();
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
            
            //初始化IK Solver
            this.IKSolvers.ForEach(x =>
            {
                this.m_FootRotation.Add(Quaternion.identity);
                this.m_Feet.Add(x.solver.Bone3);

                x.solver.OnPreUpdate += OnSolverUpdate;
                x.solver.OnPostUpdate += OnPostSolverUpdate;
            });

            //初始化Ground Solver
            this.m_AnimaPelvisLocalPos = this.Pelvis.localPosition;
            this.GroundSolver.InitSolver(this.Root, this.Pelvis, this.m_Feet);
            this.m_LastRootY = this.Root.position.y;
            this.m_Inited = true;
        }

        private void OnSolverUpdate()
        {
            if (!this.m_Inited) return;
            if (this.Weight <= 0) return;
            if (this.m_Solved) return;

            if (this.Pelvis.localPosition != m_SolvedPelvisLocalPos) this.m_AnimaPelvisLocalPos = this.Pelvis.localPosition;
            else this.Pelvis.localPosition = this.m_AnimaPelvisLocalPos;
            
            //Ground Solver先解算出落脚点和骨盆偏移值
            this.GroundSolver.Update();

            //IK Solver获取Ground Solver的落脚点坐标再解算腿部姿态
            for (int i = 0; i < IKSolvers.Count; i++)
            {
                var leg = IKSolvers[i];
                m_FootRotation[i] = this.m_Feet[i].rotation;

                leg.solver.IKPosition = GroundSolver.Legs[i].IKPosition;
                leg.solver.IKWeight = this.Weight;
            }
            
#if UNITY_EDITOR
            if (Debug)
            {
                UnityEngine.Debug.Log("");
            }
#endif
            //偏移骨盆
            this.Pelvis.position += this.GroundSolver.PelvisSolver.PelvisOffset * Weight;
            
            //设置权重
            if (this.GroundSolver.AutoHighWeight)
            {
                bool isUp = this.Root.position.y - this.m_LastRootY > 0;
                float target = this.GroundSolver.Velocity > this.GroundSolver.MinFootSpeed && isUp ? 1 : 0;
                if (this.GroundSolver.LerpType == LerpType.Linear)
                {
                    this.GroundSolver.HighWeight = XenoTools.LerpValue(this.GroundSolver.HighWeight, target, this.GroundSolver.FootSpeed, this.GroundSolver.FootSpeed);
                }
                else
                {
                    this.GroundSolver.HighWeight = XenoTools.LerpDamper(this.GroundSolver.HighWeight, target, ref this.m_WeightVelocity, this.GroundSolver.WeightDamperTime, this.GroundSolver.LerpType);
                }
                
            }
            
            this.m_IKSolvedCounts = 0;
            this.m_Solved = true;
        }

        private void OnPostSolverUpdate()
        {
            if (!this.m_Inited) return;
            if (this.Weight <= 0f) return;
            this.m_IKSolvedCounts++;

            if (this.m_IKSolvedCounts < this.m_Feet.Count) return;
            //等待这一帧所有Solver解算完成后再旋转脚部
            this.m_Solved = false;
            for (int i = 0; i < this.m_Feet.Count; i++)
            {
                this.m_Feet[i].rotation = Quaternion.Slerp(Quaternion.identity, this.GroundSolver.Legs[i].IKRotation, this.Weight) * m_FootRotation[i];
            }

            this.m_LastRootY = this.Root.position.y;
            this.m_SolvedPelvisLocalPos = this.Pelvis.localPosition;
        }

#if UNITY_EDITOR
        
        private void GetIKSolver()
        {
            if (this.IKSolvers != null && this.IKSolvers.Count > 0) return;

            var iks = this.transform.GetComponentsInChildren<TwoBoneIK>().ToList();
            this.IKSolvers = iks;
        }

        [Button("一键绑定骨骼")]
        public void AutoBindRootAndPelvis()
        {
            this.GetIKSolver();
            this.Pelvis = this.transform.parent.FindPelvis();
            if (this.Root == null) this.Root = XenoTools.FindTargetBone(this.transform.parent, "root", true);
            if (this.Root == null && this.Pelvis != null) this.Root = this.Pelvis.parent;
            for (int i = 0; i < this.IKSolvers.Count; i++)
            {
                this.IKSolvers[i].BindBones();
            }
        }
        
#endif
    }
}