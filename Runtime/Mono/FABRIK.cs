using UnityEngine;

namespace XenoIK
{
    public class FABRIK : XenoIK
    {

        public IKSolverFABR solver = new IKSolverFABR();
        
        protected override IKSolver GetIKSolver()
        {
            return solver;
        }
    }
}