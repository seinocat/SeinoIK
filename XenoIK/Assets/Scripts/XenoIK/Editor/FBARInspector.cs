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
            IKIKSolverIKSolverHeuristicInspector.DrawInspector(this.Solver);
        }

        private void OnSceneGUI()
        {
            IKIKSolverIKSolverHeuristicInspector.DrawSceneGUI(this.script.Solver, Color.cyan, Color.cyan);
        }
    }
}