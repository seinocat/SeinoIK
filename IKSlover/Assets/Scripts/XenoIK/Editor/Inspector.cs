

using System;
using UnityEditor;
using UnityEngine;

namespace XenoIK.Editor
{
    public class Inspector
    {
        public const float Indent = 15;
        public const float BtnWidth = 20;
        public const float CBtnWidth = 65;

        public delegate void DrawElement(SerializedProperty element);
        public delegate void DrawElementLabel(SerializedProperty element, int index);
        public delegate void OnAddElement(SerializedProperty element);
        public delegate void OnMoveElement(SerializedProperty element);
        public delegate void OneKeyAdd(SerializedProperty element);
        
        private static SerializedProperty element;
        private static SerializedProperty property;
        private static bool isShowList = true;
        
        
        
        public static void AddList(SerializedProperty prop, GUIContent guiContent,
            DrawElement drawElement = null,
            OnAddElement onAddElement = null, 
            DrawElementLabel drawElementLabel = null,
            OnMoveElement onMoveElement = null,
            OneKeyAdd oneKeyAdd = null)
        {
            property = prop;
            int deleteIndex = -1;
            string headLabel = $"{guiContent.text}({prop.arraySize})";
            isShowList = EditorGUILayout.Foldout(isShowList, headLabel);
            if (!isShowList) return;
            
            for (int i = 0; i < prop.arraySize; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space((EditorGUI.indentLevel + 1) * Indent);
                GUILayout.BeginVertical();
                element = prop.GetArrayElementAtIndex(i);
                
                GUILayout.BeginHorizontal();
                //骨骼链面板
                drawElementLabel(element, i);
                GUILayout.Space(5);
                if (GUILayout.Button(new GUIContent("-", "删除"), EditorStyles.miniButton, GUILayout.Width(BtnWidth)))
                {
                    deleteIndex = i;
                }
                if (GUILayout.Button(new GUIContent("↑", "上移"), EditorStyles.miniButton, GUILayout.Width(BtnWidth)))
                {
                    prop.MoveArrayElement(i, Math.Max(0, i - 1));
                    onMoveElement?.Invoke(prop);
                }
                if (GUILayout.Button(new GUIContent("↓", "下移"), EditorStyles.miniButton, GUILayout.Width(BtnWidth)))
                {
                    prop.MoveArrayElement(i, Math.Min(prop.arraySize - 1, i + 1));
                    onMoveElement?.Invoke(prop);
                }

                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (oneKeyAdd != null)
            {
                if (GUILayout.Button(new GUIContent("自动添加", "一键添加指定元素"), EditorStyles.toolbarButton, GUILayout.Width(CBtnWidth)))
                {
                    oneKeyAdd(prop);
                }
            }
            GUILayout.Space(5);
            if (GUILayout.Button(new GUIContent("删除全部", "删除全部列表元素"), EditorStyles.toolbarButton, GUILayout.Width(CBtnWidth)))
            {
                if (EditorUtility.DisplayDialog("警告", "是否清空列表", "确认", "取消"))
                {
                    prop.arraySize = 0;
                }
            }
            GUILayout.Space(5);
            if (GUILayout.Button(new GUIContent("添加骨骼", "添加"), EditorStyles.toolbarButton, GUILayout.Width(CBtnWidth)))
            {
                prop.arraySize++;
                onAddElement?.Invoke(prop.GetArrayElementAtIndex(prop.arraySize - 1));
            }
            GUILayout.EndHorizontal();
            
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