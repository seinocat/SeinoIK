

using UnityEditor;
using UnityEngine;

namespace XenoIK.Editor
{
    public class Inspector
    {
        public const float Indent = 15;

        public delegate void DrawElement(SerializedProperty element);
        public delegate void DrawElementLabel(SerializedProperty element);
        public delegate void OnAddElement(SerializedProperty element);
        
        private static SerializedProperty element;
        private static SerializedProperty property;
        
        
        public static void AddList(SerializedProperty prop, GUIContent guiContent,
            DrawElement drawElement = null,
            OnAddElement onAddElement = null, 
            DrawElementLabel drawElementLabel = null)
        {

            GUILayout.Label($"{guiContent.text}({prop.arraySize})", GUILayout.Width(150));
            int deleteIndex = -1;
            
            
            
            for (int i = 0; i < prop.arraySize; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space((EditorGUI.indentLevel + 1) * Indent);
                GUILayout.BeginVertical();
                element = prop.GetArrayElementAtIndex(i);
                
                
              
                
                GUILayout.BeginHorizontal();
                
                //骨骼链面板
                drawElementLabel(element);
                
                if (GUILayout.Button(new GUIContent("-", "删除"), EditorStyles.miniButton, GUILayout.Width(20)))
                {
                    deleteIndex = i;
                }
                GUILayout.EndHorizontal();

                if (deleteIndex != -1)
                {
                    prop.DeleteArrayElementAtIndex(deleteIndex);
                }
                
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("+", "添加"), EditorStyles.miniButton, GUILayout.Width(20)))
            {
                prop.arraySize++;
                if (onAddElement != null)
                {
                    onAddElement(prop.GetArrayElementAtIndex(prop.arraySize - 1));
                }
                
            }
            GUILayout.EndHorizontal();
        }


        public static void AddObjectReference(SerializedProperty prop, GUIContent guiContent, int lableWidth,
            int propWidth)
        {
            EditorGUILayout.LabelField(guiContent, GUILayout.Width(lableWidth));
            EditorGUILayout.PropertyField(prop, GUIContent.none, GUILayout.Width(propWidth));
        }
    }
}