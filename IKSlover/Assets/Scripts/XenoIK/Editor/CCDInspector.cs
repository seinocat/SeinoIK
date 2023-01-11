using UnityEditor;
using UnityEngine;

namespace XenoIK.Editor
{
    [CustomEditor(typeof(CCDIK))]
    public class CCDInspector : IKInspector
    {
        private CCDIK script => target as CCDIK;

        protected override MonoBehaviour GetMonoScript()
        {
            return script;
        }

        protected override void AddInspector()
        {
            IKSolverHeuristicInspector.AddInspector(this.Solver);
        }
    }
}