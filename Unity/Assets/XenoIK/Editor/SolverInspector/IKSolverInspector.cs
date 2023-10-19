using System;
using UnityEditor;
using UnityEngine;

namespace XenoIK.Editor
{
    public class IKSolverInspector
    {
        public delegate void DrawElement(SerializedProperty element, int index);
        public delegate void DrawButtons(SerializedProperty prop);
        
        public const float BtnWidth = 20;
        public const float CBtnWidth = 65;
        
        private SerializedProperty element;
        private bool isShowList1;
        private bool isShowList2;
        private bool isShowList3;

        private bool IsShow(int index)
        {
            return index switch
            {
                0 => isShowList1,
                1 => isShowList2,
                2 => isShowList3,
                _ => false
            };
        }

        private void SetShow(bool show, int index)
        {
            switch (index)
            {
                case 0:
                    isShowList1 = show;
                    break;
                case 1:
                    isShowList2 = show;
                    break;
                case 2:
                    isShowList3 = show;
                    break;
            }
        }
        
        public static float GetHandleSize(Vector3 position) {
            float s = HandleUtility.GetHandleSize(position) * 0.1f;
            return Mathf.Lerp(s, 0.025f, 0.5f);
        }
        
        public void DrawElements(SerializedProperty prop, int listIndex, 
            GUIContent guiContent, DrawElement drawElement, DrawButtons drawButtons = null, bool isCanEidtor = true)
        {
            int deleteIndex = -1;
            string headLabel = $"{guiContent.text}({prop.arraySize})";
            SetShow(EditorGUILayout.Foldout(IsShow(listIndex), headLabel), listIndex);

            if (!IsShow(listIndex)) return;
            EditorGUILayout.Space(5);
            for (int i = 0; i < prop.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                element = prop.GetArrayElementAtIndex(i);
                
                EditorGUILayout.BeginHorizontal();
                //Draw Bones Chians
                drawElement(element, i);

                EditorGUILayout.Space(5);
                
                if (isCanEidtor && GUILayout.Button(new GUIContent("×", "删除"), EditorStyles.miniButton, GUILayout.Width(BtnWidth)))
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

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space(5);
            // Draw Btns
            EditorGUILayout.BeginHorizontal();
            drawButtons?.Invoke(prop);
            EditorGUILayout.Space(5);
            if (isCanEidtor && GUILayout.Button(new GUIContent("添加骨骼", "添加"), EditorStyles.toolbarButton, GUILayout.Width(CBtnWidth)))
            {
                prop.arraySize++;
                OnAddBone(prop.GetArrayElementAtIndex(prop.arraySize - 1));
            }
            EditorGUILayout.EndHorizontal();
            
            if (deleteIndex != -1)
            {
                prop.DeleteArrayElementAtIndex(deleteIndex);
            }
        }
        
        
        public void OnAddBone(SerializedProperty bone)
        {
            bone.FindPropertyRelative("weight").floatValue = 1;
        }


        public void AddObjectReference(SerializedProperty prop, GUIContent guiContent, int lableWidth, int propWidth)
        {
            EditorGUILayout.LabelField(guiContent, GUILayout.Width(lableWidth));
            EditorGUILayout.PropertyField(prop, GUIContent.none, GUILayout.Width(propWidth));
        }
        

    }
}