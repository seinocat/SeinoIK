namespace SeinoIK
{
    public class LookAtIK : SeinoIK
    {
        public IKSolverLookAt solver = new IKSolverLookAt();
        
        protected override IKSolver GetIKSolver()
        {
            return solver;
        }
    }
}