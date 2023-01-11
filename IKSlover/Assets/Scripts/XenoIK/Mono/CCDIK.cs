using System;

namespace XenoIK
{

    public class CCDIK : XenoIK
    {
        public IKSolverCCD Solver = new IKSolverCCD();
        
        protected override IKSolver GetIKSolver()
        {
            return Solver;
        }
    }
}