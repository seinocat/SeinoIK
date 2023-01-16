using System;
using UnityEditor;
using UnityEngine;

namespace XenoIK
{
    public abstract class IKInspector : UnityEditor.Editor
    {
        protected abstract MonoBehaviour GetMonoScript();
        protected abstract void DrawInspector();
        
        protected SerializedProperty solver;

        private void OnEnable()
        {
            solver = serializedObject.FindProperty("solver");
        }

        public override void OnInspectorGUI()
        {
            DrawInspector();
            serializedObject.ApplyModifiedProperties();
        }
        
    }
}