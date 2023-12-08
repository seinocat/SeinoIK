using Sirenix.OdinInspector;

namespace SeinoIK
{
    [HideMonoScript]
    public class TwoBoneIK : SeinoIK
    {
        [HideLabel]
        public IKSolverTwoBone solver = new IKSolverTwoBone();
        
        protected override IKSolver GetIKSolver()
        {
            return solver;
        }

#if UNITY_EDITOR
        [Title("Editor工具")]
        [Button("一键绑定骨骼")]
        public void BindBones()
        {
            var root = this.transform.parent.parent;
            var bones = SeinoTools.FindGoalBone(root, this.solver.IKGoal);
            if (bones.Count > 0)
            {
                for (int i = 0; i < bones.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            solver.Bone1 = bones[0];
                            break;
                        case 1:
                            solver.Bone2 = bones[1];
                            break;
                        case 2:
                            solver.Bone3 = bones[2];
                            break;
                    }
                }
            }
        }
        
#endif
    }
}