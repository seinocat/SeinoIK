using System;
using UnityEngine;

namespace XenoIK
{
    public abstract class XenoIK : SolverManager
    {
        protected abstract IKSolver GetIKSolver();

        protected override void UpdateSolver(float deltaTime)
        {
            if (!GetIKSolver().initiated) InitialSolver();
            if (!GetIKSolver().initiated) return;
            GetIKSolver().Update(deltaTime);
        }

        protected override void InitialSolver()
        {
            if (GetIKSolver().initiated) return;
            GetIKSolver().Init();
        }
    }
}