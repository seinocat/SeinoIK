using Sirenix.OdinInspector;

namespace XenoIK
{
    [HideReferenceObjectPicker]
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