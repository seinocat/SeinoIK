using UnityEngine;

namespace SeinoIK
{
    public class FABRIK : SeinoIK
    {

        public IKSolverFABR solver = new IKSolverFABR();
        
        protected override IKSolver GetIKSolver()
        {
            return solver;
        }
    }
}