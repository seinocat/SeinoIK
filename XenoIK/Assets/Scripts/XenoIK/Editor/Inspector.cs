

using System;
using UnityEditor;
using UnityEngine;

namespace XenoIK.Editor
{
    public class Inspector
    {
        public delegate void DrawElement(SerializedProperty element, int index);
        public delegate void DrawButtons(SerializedProperty prop);
        
        public const float Indent = 15;
        public const float BtnWidth = 20;
        public const float CBtnWidth = 65;
        
        private static SerializedProperty element;
        private static SerializedProperty property;
        private static bool isShowList = true;
        
        
        public static void DrawElements(SerializedProperty prop, 
            GUIContent guiContent, DrawElement drawElementLabel, DrawButtons drawButtons = null)
        {
            property = prop;
            int deleteIndex = -1;
            string headLabel = $"{guiContent.text}({prop.arraySize})";
            isShowList = EditorGUILayout.Foldout(isShowList, headLabel);
            if (!isShowList) return;
            GUILayout.Space(5);
            for (int i = 0; i < prop.arraySize; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                element = prop.GetArrayElementAtIndex(i);
                
                GUILayout.BeginHorizontal();
                //Draw Bones Chians
                drawElementLabel(element, i);
                GUILayout.Space(5);
                if (GUILayout.Button(new GUIContent("×", "删除"), EditorStyles.miniButton, GUILayout.Width(BtnWidth)))
                {
                    deleteIndex = i;
                }
                if (GUILayout.Button(new GUIContent("↑", "上移"), EditorStyles.miniButton, GUILayout.Width(BtnWidth)))
                {
                    prop.MoveArrayElement(i, Math.Max(0, i - 1));
                }
                if (GUILayout.Button(new GUIContent("↓", "下移"), EditorStyles.miniButton, GUILayout.Width(BtnWidth)))
                {
                    prop.MoveArrayElement(i, Math.Min(prop.arraySize - 1, i + 1));
                }

                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(5);
            // Draw Btns
            drawButtons?.Invoke(prop);
            
            if (deleteIndex != -1)
            {
                prop.DeleteArrayElementAtIndex(deleteIndex);
            }
        }


        public static void AddObjectReference(SerializedProperty prop, GUIContent guiContent, int lableWidth, int propWidth)
        {
            EditorGUILayout.LabelField(guiContent, GUILayout.Width(lableWidth));
            EditorGUILayout.PropertyField(prop, GUIContent.none, GUILayout.Width(propWidth));
        }
    }
}