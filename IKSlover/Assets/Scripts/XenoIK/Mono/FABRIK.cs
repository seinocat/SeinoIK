using UnityEngine;

namespace XenoIK
{
    public class FABRIK : XenoIK
    {

        public IKSolverFBAR Solver = new IKSolverFBAR();
        
        protected override IKSolver GetIKSolver()
        {
            return Solver;
        }
    }
}