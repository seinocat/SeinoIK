using UnityEditor;
using UnityEngine;

namespace XenoIK.Editor
{
    [CustomEditor(typeof(LookAtCtrl))]
    public class LookAtCtrlInspector : IKInspector
    {
        
        public LookAtCtrl script => target as LookAtCtrl;
        
        protected override MonoBehaviour GetMonoScript()
        {
            return script;
        }

        protected override void DrawInspector()
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("lookAtIK"), new GUIContent("LookAtIK"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("target"), new GUIContent("目标"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("angle"), new GUIContent("角度"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("maxDistance"), new GUIContent("最大距离"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("minDistance"), new GUIContent("最小距离"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("lookAtCurve"), new GUIContent("注视速率曲线"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("lookAwayCurve"), new GUIContent("移开注视曲线"));
        }

        protected override void OnModifty()
        {
            
        }
    }
}