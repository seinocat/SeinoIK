

using System;
using UnityEditor;
using UnityEngine;

namespace XenoIK
{
    public abstract class IKInspector : UnityEditor.Editor
    {
        protected abstract MonoBehaviour GetMonoScript();
        protected abstract void AddInspector();

        protected SerializedProperty Solver;

        private void OnEnable()
        {
            Solver = serializedObject.FindProperty("Solver");
        }

        public override void OnInspectorGUI()
        {
            AddInspector();
            serializedObject.ApplyModifiedProperties();
        }
    }
}