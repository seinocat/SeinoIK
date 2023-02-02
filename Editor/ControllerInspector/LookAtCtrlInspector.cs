using UnityEditor;
using UnityEngine;
using XenoIK.Runtime.Config;

namespace XenoIK.Editor
{
    [CustomEditor(typeof(LookAtCtrl))]
    public class LookAtCtrlInspector : IKInspector
    {
        public LookAtCtrl script => target as LookAtCtrl;
        private LookAtConfig config => script.config;

        private bool showSettings = true;

        protected override MonoBehaviour GetMonoScript()
        {
            return script;
        }

        protected override void DrawInspector()
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("lookAtIK"), new GUIContent("IK解算器"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("enableIk"), new GUIContent("启用IK"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("target"), new GUIContent("目标"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("config"), new GUIContent("IK配置"));

            if (this.script.config == null)
            {
                EditorGUILayout.HelpBox("没有对应的IK配置！！！.", MessageType.Error);
                return;
            }
            GUILayout.Space(5);
            showSettings = EditorGUILayout.Foldout(showSettings, "参数设置");
            if (!showSettings) return;
            this.config.defaultWeight = EditorGUILayout.Slider("全局权重", this.config.defaultWeight, 0, 1);
            this.config.detectAngleXZ = EditorGUILayout.Vector2Field("发现角度", this.config.detectAngleXZ);
            this.config.followAngleXZ = EditorGUILayout.Vector2Field("跟随角度", this.config.followAngleXZ);
            this.config.offset = EditorGUILayout.Vector3Field("偏移量", this.config.offset);
            this.config.maxDistance = EditorGUILayout.Slider("最大距离", this.config.maxDistance, 0, 100000);
            this.config.maxDistance = EditorGUILayout.Slider("最小距离", this.config.minDistance, 0, 100000);
            this.config.lookAtSpeed = EditorGUILayout.Slider("看向转速", this.config.lookAtSpeed, 0, 360);
            this.config.lookAwaySpeed = EditorGUILayout.Slider("离开转速", this.config.lookAwaySpeed, 0, 360);
            this.config.useCustomCurve = EditorGUILayout.Toggle("自定义动画曲线", this.config.useCustomCurve);
            if (this.config.useCustomCurve)
            {
                this.config.lookAtCurve = EditorGUILayout.CurveField("注视曲线", this.config.lookAtCurve);
                this.config.lookAwayCurve = EditorGUILayout.CurveField("移开注视曲线", this.config.lookAwayCurve);
            }


        }
        
        protected override void OnModifty()
        {
            
        }
    }
}