using System;
using SeinoIK.Editor;
using UnityEditor;
using UnityEngine;

namespace SeinoIK
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
            OnInspectorEnable();
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