using UnityEngine;

namespace XenoIK
{
    public class LookAtIK : XenoIK
    {
        
        public IKSolverLookAt Solver = new IKSolverLookAt();
        
        protected override IKSolver GetIKSolver()
        {
            return Solver;
        }
    }
}