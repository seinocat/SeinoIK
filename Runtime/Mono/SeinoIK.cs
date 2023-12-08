using System;
using UnityEngine;

namespace SeinoIK
{
    public abstract class SeinoIK : SolverManager
    {
        protected abstract IKSolver GetIKSolver();
        
        protected override void InitialSolver()
        {
            if (GetIKSolver().initiated) return;
            GetIKSolver().Init(this.transform);
        }

        protected override void FixTransform()
        {
            GetIKSolver().FixTransform();
        }

        protected override void UpdateSolver(float deltaTime)
        {
            if (!GetIKSolver().initiated) InitialSolver();
            if (!GetIKSolver().initiated) return;
            GetIKSolver().Update(deltaTime);
        }
    }
}