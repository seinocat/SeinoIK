﻿using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace XenoIK.Editor
{
    [CustomEditor(typeof(LookAtIK))]
    public class LookAtInspector : IKInspector
    {
        public LookAtIK script => target as LookAtIK;
        
        protected override MonoBehaviour GetMonoScript()
        {
            return script;
        }
        
        protected override void OnModifty()
        {
            if (!Application.isPlaying) this.script?.solver.Init(this.script.transform);
            script.solver.defaultAxis.Normalize();
            script.solver.UpdateAxis();
        }

        protected override void DrawInspector()
        {
            IKSolverLookAtInspector.DrawInspector(this.solver, script);
        }
        
        private void OnSceneGUI()
        {
            IKSolverLookAtInspector.DrawSceneGUI(this.script.solver);
        }
    }
}