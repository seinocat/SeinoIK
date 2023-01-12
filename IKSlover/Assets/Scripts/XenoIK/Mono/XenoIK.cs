using System;
using UnityEngine;

namespace XenoIK
{
    public abstract class XenoIK : SolverManager
    {
        protected abstract IKSolver GetIKSolver();

        protected override void UpdateSolver()
        {
            if (!GetIKSolver().Initiated) InitialSolver();
            if (!GetIKSolver().Initiated) return;
            
            GetIKSolver().Update();
        }

        protected override void InitialSolver()
        {
            if (GetIKSolver().Initiated)
                return;
            
            GetIKSolver().Init();
        }
    }
}