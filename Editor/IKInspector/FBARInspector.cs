using UnityEditor;
using UnityEngine;

namespace SeinoIK.Editor
{
    [CustomEditor(typeof(FABRIK))]
    public class FBARInspector : IKInspector
    {
        private FABRIK script => target as FABRIK;
        
        private IKSolverHeuristicInspector Inspector => this.solverInspector as IKSolverHeuristicInspector;
        
        protected override void OnInspectorEnable()
        {
            this.solverInspector = new IKSolverHeuristicInspector();
        }

        protected override void OnModifty()
        {
            this.script?.solver.Init(this.script.transform);
        }
        
        protected override void DrawInspector()
        {
            this.Inspector.DrawInspector(this.solver);
        }

        private void OnSceneGUI()
        {
            this.Inspector.DrawSceneGUI(this.script.solver);
        }
    }
}