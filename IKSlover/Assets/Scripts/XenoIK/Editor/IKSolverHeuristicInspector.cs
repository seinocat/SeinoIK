using System.Collections;
using UnityEditor;
using UnityEngine;

namespace XenoIK.Editor
{
    public class IKSolverHeuristicInspector : IKSolverInspector
    {
        public static void DrawInspector(SerializedProperty solver)
        {
            DrawGUI(solver);
        }

        public static void DrawSceneGUI(IKSolverHeuristic solver, Color jointColor, Color boneColor)
        {
            if (Application.isPlaying && !solver.initiated) return;
            if (solver.bones.Count == 0) return;
            
            for (int i = 0; i < solver.bones.Count; i++)
            {
                var bone = solver.bones[i];
                Handles.color = jointColor;
                Handles.SphereHandleCap(0, bone.Position, Quaternion.identity, GetHandleSize(bone.Position), EventType.Repaint);
                Handles.color = boneColor;
                if (i < solver.bones.Count - 1) Handles.DrawLine(bone.Position, solver.bones[i+1].Position, 3);
            }
        }

        public static void DrawGUI(SerializedProperty solver)
        {
            EditorGUILayout.PropertyField(solver.FindPropertyRelative("target"), new GUIContent("目标物体"));
            EditorGUILayout.PropertyField(solver.FindPropertyRelative("IKWeight"), new GUIContent("全局权重"));
            EditorGUILayout.PropertyField(solver.FindPropertyRelative("maxIterations"), new GUIContent("最大迭代次数"));
            
            DrawElements(solver.FindPropertyRelative("bones"), new GUIContent("骨骼"), OnDrawElementBones, OnDrawButtons);
        }
        
        private static void OnDrawElementBones(SerializedProperty bone, int index)
        {
            var boneTrans = bone.FindPropertyRelative("transform");
            AddObjectReference(boneTrans, GUIContent.none, 0, 120);
            OnDrawWightSlider(bone.FindPropertyRelative("weight"));
        }
        
        private static void OnDrawWightSlider(SerializedProperty Weight)
        {
            GUILayout.Label("权重", GUILayout.Width(45));
            EditorGUILayout.PropertyField(Weight, GUIContent.none);
        }
        
        private static void OnAddBone(SerializedProperty bone)
        {
            bone.FindPropertyRelative("weight").floatValue = 1;
        }

        private static void OnDrawButtons(SerializedProperty prop)
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button(new GUIContent("自动添加", "一键添加指定元素"), EditorStyles.toolbarButton, GUILayout.Width(CBtnWidth)))
            {
                CreateChians(prop);
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
                OnAddBone(prop.GetArrayElementAtIndex(prop.arraySize - 1));
            }
            GUILayout.EndHorizontal();
        }

        
        private static void CreateChians(SerializedProperty prop)
        {
            bool NewChain = prop.arraySize == 0;
            var transList = XenoTools.CreateBoneChains(Selection.activeTransform, NewChain ? 5 : prop.arraySize);
            if (NewChain)
            {
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
}