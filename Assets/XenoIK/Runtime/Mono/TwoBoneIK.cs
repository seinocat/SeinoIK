using Sirenix.OdinInspector;

namespace XenoIK
{
    public class TwoBoneIK : XenoIK
    {
        [HideLabel]
        public IKSolverTwoBone solver = new IKSolverTwoBone();
        
        protected override IKSolver GetIKSolver()
        {
            return solver;
        }
    }
}