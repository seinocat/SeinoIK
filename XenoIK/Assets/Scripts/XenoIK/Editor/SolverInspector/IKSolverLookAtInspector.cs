using UnityEditor;
using UnityEngine;

namespace XenoIK.Editor
{
    public class IKSolverLookAtInspector : IKSolverInspector
    {
        private static MonoBehaviour script;
        
        public static void DrawInspector(SerializedProperty prop, MonoBehaviour mono)
        {
            script = mono;
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("target"), new GUIContent("注视目标"));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("IKWeight"), new GUIContent("全局权重"));
            
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("head.transform"), new GUIContent("头骨骼"));
            DrawElements(prop.FindPropertyRelative("eyes"), 0, new GUIContent("眼睛"), DrawLookAtBone, DrawEyeBtns);
            DrawElements(prop.FindPropertyRelative("spines"), 1, new GUIContent("脊椎"), DrawLookAtBone, DrawSpineBtns);
        }
        
        public static void DrawLookAtBone(SerializedProperty bone, int index)
        {
            EditorGUILayout.PropertyField(bone.FindPropertyRelative("transform"), GUIContent.none, GUILayout.Width(120));
            DrawWightSlider(bone.FindPropertyRelative("weight"));
        }
        
        private static void DrawWightSlider(SerializedProperty Weight)
        {
            GUILayout.Label("权重", GUILayout.Width(45));
            EditorGUILayout.PropertyField(Weight, GUIContent.none);
        }
        
        public static void DrawEyeBtns(SerializedProperty prop)
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawCommonBtn(prop);
            GUILayout.EndHorizontal();
        }

        public static void DrawSpineBtns(SerializedProperty prop)
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("自动添加", "一键添加选中骨骼元素"), EditorStyles.toolbarButton, GUILayout.Width(IKSolverInspector.CBtnWidth)))
            {
                CreateSpines(prop);
            }
            GUILayout.Space(5);

            DrawCommonBtn(prop);
            GUILayout.EndHorizontal();
        }


        public static void DrawCommonBtn(SerializedProperty prop)
        {
            if (GUILayout.Button(new GUIContent("删除全部", "删除全部列表元素"), EditorStyles.toolbarButton, GUILayout.Width(IKSolverInspector.CBtnWidth)))
            {
                if (EditorUtility.DisplayDialog("警告", "是否清空列表", "确认", "取消"))
                {
                    prop.arraySize = 0;
                }
            }
            GUILayout.Space(5);

            if (GUILayout.Button(new GUIContent("添加骨骼", "添加"), EditorStyles.toolbarButton, GUILayout.Width(IKSolverInspector.CBtnWidth)))
            {
                prop.arraySize++;
                OnAddBone(prop.GetArrayElementAtIndex(prop.arraySize - 1));
            }
        }
        
        public static void OnAddBone(SerializedProperty bone)
        {
            bone.FindPropertyRelative("weight").floatValue = 1;
        }
        
        public static void CreateSpines(SerializedProperty prop)
        {
            if (prop.arraySize != 0) return;

            var root = script.transform;
            var head = XenoTools.FindTargetBone(root, "head");
            var transList = XenoTools.CreateBoneChains(head.parent);
            prop.arraySize = transList.Count;
            
            for (int i = 0; i < prop.arraySize; i++)
            {
                if (i > transList.Count) break;
                var element = prop.GetArrayElementAtIndex(i);
                if (element != null)
                {
                    OnAddBone(element);
                    var bone = element.FindPropertyRelative("transform");
                    bone.objectReferenceValue = transList[i];
                }
            }
        }
    }
}