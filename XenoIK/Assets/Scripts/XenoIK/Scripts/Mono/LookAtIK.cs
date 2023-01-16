using UnityEngine;

namespace XenoIK
{
    public class LookAtIK : XenoIK
    {
        public IKSolverLookAt solver = new IKSolverLookAt();
        
        protected override IKSolver GetIKSolver()
        {
            return solver;
        }
    }
}