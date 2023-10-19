using UnityEditor;
using UnityEngine;

namespace XenoIK.Editor
{
    [CustomEditor(typeof(CCDIK))]
    public class CCDInspector : IKInspector
    {
        private CCDIK script => target as CCDIK;
        private IKSolverHeuristicInspector Inspector => this.solverInspector as IKSolverHeuristicInspector;
        
        protected override void OnInspectorEnable()
        {
            this.solverInspector = new IKSolverHeuristicInspector();
        }

        protected override void DrawInspector()
        {
            this.Inspector.DrawInspector(this.solver);
        }

        protected override void OnModifty()
        {
            this.script?.solver.Init(this.script.transform);
        }
        
        private void OnSceneGUI()
        {
            this.Inspector.DrawSceneGUI(this.script.solver);
        }
    }
}