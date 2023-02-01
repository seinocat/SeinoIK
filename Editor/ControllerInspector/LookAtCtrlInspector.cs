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
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("lookAtIK"), new GUIContent("IK解算器"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("enableIk"), new GUIContent("启用IK"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("target"), new GUIContent("目标"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("defaultWeight"), new GUIContent("全局权重"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("detectAngleXZ"), new GUIContent("发现角度"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("followAngleXZ"), new GUIContent("跟随角度"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("offset"), new GUIContent("偏移量"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("maxDistance"), new GUIContent("最大距离"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("minDistance"), new GUIContent("最小距离"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("lookAtSpeed"), new GUIContent("看向转速"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("lookAwaySpeed"), new GUIContent("离开转速"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("useCustomCurve"), new GUIContent("自定义动画曲线"));
            
            if (!script.useCustomCurve) return;
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("switchWeightTime"), new GUIContent("平缓时间"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("switchWeightSpeed"), new GUIContent("平缓速率"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("lookAtCurve"), new GUIContent("注视速率曲线"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("lookAwayCurve"), new GUIContent("移开注视曲线"));
        }

        protected override void OnModifty()
        {
            
        }
    }
}