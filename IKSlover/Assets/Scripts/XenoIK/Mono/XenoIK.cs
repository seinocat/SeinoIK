using UnityEngine;

namespace XenoIK
{
    public abstract class XenoIK : SolverManager
    {
        protected abstract IKSolver GetIKSolver();
    }
}