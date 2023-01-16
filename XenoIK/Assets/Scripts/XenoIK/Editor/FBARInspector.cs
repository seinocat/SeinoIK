using System;
using UnityEditor;
using UnityEngine;

namespace XenoIK.Editor
{
    [CustomEditor(typeof(FABRIK))]
    public class FBARInspector : IKInspector
    {
        private FABRIK script => target as FABRIK;
        
        protected override MonoBehaviour GetMonoScript()
        {
            return script;
        }

        protected override void DrawInspector()
        {
            IKSolverHeuristicInspector.DrawInspector(this.solver);
        }

        private void OnSceneGUI()
        {
            IKSolverHeuristicInspector.DrawSceneGUI(this.script.solver, Color.cyan, Color.cyan);
        }
    }
}