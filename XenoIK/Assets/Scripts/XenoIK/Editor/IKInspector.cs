using System;
using UnityEditor;
using UnityEngine;

namespace XenoIK
{
    public abstract class IKInspector : UnityEditor.Editor
    {
        protected abstract MonoBehaviour GetMonoScript();
        protected abstract void DrawInspector();
        protected abstract void OnModifty();
        
        protected SerializedProperty solver;
        

        private void OnEnable()
        {
            this.solver = this.serializedObject.FindProperty("solver");
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