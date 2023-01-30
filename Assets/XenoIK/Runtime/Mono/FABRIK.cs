using UnityEngine;

namespace XenoIK
{
    public class FABRIK : XenoIK
    {

        public IKSolverFBAR solver = new IKSolverFBAR();
        
        protected override IKSolver GetIKSolver()
        {
            return solver;
        }
    }
}