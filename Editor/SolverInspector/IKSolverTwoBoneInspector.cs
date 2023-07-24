﻿using UnityEditor;
using UnityEngine;

namespace XenoIK.Editor
{
    public class IKSolverTwoBoneInspector : IKSolverInspector
    {
        private TwoBoneIK script;
        
        public void DrawInspector(SerializedProperty prop, TwoBoneIK mono)
        {
            script = mono;
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("target"), new GUIContent("目标"));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("poleTarget"), new GUIContent("极向量"));
            DrawTwoBoneList(prop);
        }

        public void DrawTwoBoneList(SerializedProperty prop)
        {
            DrawElements(prop.FindPropertyRelative("twoBoneList"), 0, new GUIContent("骨骼列表"), DrawTwoBoneList, null, false);
        }

        public void DrawTwoBoneList(SerializedProperty bone, int index)
        {
            EditorGUILayout.PropertyField(bone.FindPropertyRelative("transform"), GUIContent.none, GUILayout.Width(120));
        }
    }
}