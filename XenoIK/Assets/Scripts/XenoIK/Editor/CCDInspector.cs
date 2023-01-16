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

        protected override void DrawInspector()
        {
            IKSolverHeuristicInspector.DrawInspector(this.Solver);
        }
        
        private void OnSceneGUI()
        {
            IKSolverHeuristicInspector.DrawSceneGUI(this.script.Solver, Color.green, Color.cyan);
        }
    }
}