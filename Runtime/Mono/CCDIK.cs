namespace XenoIK
{

    public class CCDIK : XenoIK
    {
        public IKSolverCCD solver = new IKSolverCCD();
        
        protected override IKSolver GetIKSolver()
        {
            return solver;
        }
    }
}