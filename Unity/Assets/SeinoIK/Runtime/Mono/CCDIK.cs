namespace SeinoIK
{

    public class CCDIK : SeinoIK
    {
        public IKSolverCCD solver = new IKSolverCCD();
        
        protected override IKSolver GetIKSolver()
        {
            return solver;
        }
    }
}