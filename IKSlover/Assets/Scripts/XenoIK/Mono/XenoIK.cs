using System;
using UnityEngine;

namespace XenoIK
{
    public abstract class XenoIK : SolverManager
    {
        protected abstract IKSolver GetIKSolver();

        protected override void UpdateSolver()
        {
            GetIKSolver().Update();
        }
    }
}