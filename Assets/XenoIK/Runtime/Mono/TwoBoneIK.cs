namespace XenoIK
{
    public class TwoBoneIK : XenoIK
    {
        public IKSolverTwoBone solver = new IKSolverTwoBone();
        
        protected override IKSolver GetIKSolver()
        {
            return solver;
        }
    }
}