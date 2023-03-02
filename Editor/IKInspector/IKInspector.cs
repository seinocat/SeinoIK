using System;
using UnityEditor;
using UnityEngine;
using XenoIK.Editor;

namespace XenoIK
{
    public abstract class IKInspector : UnityEditor.Editor
    {
        protected abstract void DrawInspector();
        protected abstract void OnModifty();
        protected abstract void OnInspectorEnable();
        
        protected SerializedProperty solver;
        protected IKSolverInspector solverInspector;
        
        private void OnEnable()
        {
            this.solver = this.serializedObject.FindProperty("solver");
            this.OnInspectorEnable();
        }

        public override void OnInspectorGUI()
        {
            DrawInspector();
            if (this.serializedObject.ApplyModifiedProperties())
            {
                OnModifty();
            }
        }
        
    }
}