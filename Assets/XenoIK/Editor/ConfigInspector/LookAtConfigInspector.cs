using System;
using UnityEditor;
using UnityEngine;
using XenoIK.Runtime.Config;

namespace XenoIK.Editor.ConfigInspector
{
    [CustomEditor(typeof(LookAtConfig))]
    public class LookAtConfigInspector : UnityEditor.Editor
    {
        public LookAtConfig config;

        private void OnEnable()
        {
            this.config = target as LookAtConfig;
        }

        public override void OnInspectorGUI()
        {
            
            
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("defaultWeight"), new GUIContent("全局权重"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("detectAngleXZ"), new GUIContent("发现角度"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("followAngleXZ"), new GUIContent("跟随角度"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("offset"), new GUIContent("偏移量"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("maxDistance"), new GUIContent("最大距离"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("minDistance"), new GUIContent("最小距离"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("lookAtSpeed"), new GUIContent("看向转速"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("lookAwaySpeed"), new GUIContent("离开转速"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("useCustomCurve"), new GUIContent("自定义动画曲线"));

            if (this.config.useCustomCurve)
            {
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("switchWeightTime"), new GUIContent("平缓时间"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("switchWeightSpeed"), new GUIContent("平缓速率"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("lookAtCurve"), new GUIContent("注视速率曲线"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("lookAwayCurve"), new GUIContent("移开注视曲线"));
            }
            
            this.serializedObject.ApplyModifiedProperties();

        }
        
        
    }
}