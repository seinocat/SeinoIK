using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace SeinoIK.Editor
{
    [CustomEditor(typeof(LookAtIK))]
    public class LookAtInspector : IKInspector
    {
        public LookAtIK script => target as LookAtIK;
        
        private IKSolverLookAtInspector Inspector => this.solverInspector as IKSolverLookAtInspector;
        
        
        protected override void OnModifty()
        {   
            if (!Application.isPlaying) this.script?.solver.Init(this.script.transform);
            script.solver.UpdateAxis();
        }

        protected override void OnInspectorEnable()
        {
            this.solverInspector = new IKSolverLookAtInspector();
        }

        protected override void DrawInspector()
        {
            this.Inspector.DrawInspector(this.solver, script);
        }
        
        private void OnSceneGUI()
        {
            this.Inspector.DrawSceneGUI(this.script.solver);
        }
    }
}