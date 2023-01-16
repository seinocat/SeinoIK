using UnityEditor;
using UnityEngine;

namespace XenoIK.Editor
{
    public class IKSolverLookAtInspector : IKSolverInspector
    {
        
        
        public static void DrawInspector(SerializedProperty prop)
        {
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("target"), new GUIContent("注视目标"));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("IKWeight"), new GUIContent("全局权重"));
            
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("head.transform"), new GUIContent("头骨骼"));
            DrawElements(prop.FindPropertyRelative("eyes"), new GUIContent("眼睛"), DrawLookAtBone, XenoTools.DrawButton);
        }


        public static void DrawLookAtBone(SerializedProperty bone, int index)
        {
            EditorGUILayout.PropertyField(bone.FindPropertyRelative("transform"), GUIContent.none, GUILayout.Width(120));
        }
    }
}