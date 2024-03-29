﻿using UnityEditor;
using UnityEngine;

namespace SeinoIK.Editor
{
    public class IKSolverHeuristicInspector : IKSolverInspector
    {
        public void DrawInspector(SerializedProperty solver)
        {
            DrawGUI(solver);
        }

        public void DrawSceneGUI(IKSolverHeuristic solver)
        {
            if (Application.isPlaying && !solver.initiated) return;
            if (solver.bones.Count == 0) return;
            
            for (int i = 0; i < solver.bones.Count; i++)
            {
                var bone = solver.bones[i];
                Handles.color = SeinoTools.jointColor;
                Handles.SphereHandleCap(0, bone.Position, Quaternion.identity, GetHandleSize(bone.Position), EventType.Repaint);
                Handles.color = SeinoTools.boneColor;
                if (i < solver.bones.Count - 1) Handles.DrawLine(bone.Position, solver.bones[i+1].Position, 3);
            }
        }

        public void DrawGUI(SerializedProperty solver)
        {
            EditorGUILayout.PropertyField(solver.FindPropertyRelative("target"), new GUIContent("目标物体"));
            EditorGUILayout.PropertyField(solver.FindPropertyRelative("IKWeight"), new GUIContent("全局权重"));
            EditorGUILayout.PropertyField(solver.FindPropertyRelative("maxIterations"), new GUIContent("最大迭代次数"));
            
            DrawElements(solver.FindPropertyRelative("bones"), 0, new GUIContent("骨骼"), DrawBones, DrawButton);
        }
        
        private void DrawBones(SerializedProperty bone, int index)
        {
            AddObjectReference(bone.FindPropertyRelative("transform"), GUIContent.none, 0, 120);
            DrawWightSlider(bone.FindPropertyRelative("weight"));
        }
        
        private void DrawWightSlider(SerializedProperty Weight)
        {
            GUILayout.Label("权重", GUILayout.Width(45));
            EditorGUILayout.PropertyField(Weight, GUIContent.none);
        }
        
        public void DrawButton(SerializedProperty prop)
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("自动添加", "一键添加选中骨骼元素"), EditorStyles.toolbarButton, GUILayout.Width(IKSolverInspector.CBtnWidth)))
            {
                CreateChians(prop);
            }
            GUILayout.Space(5);

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
            GUILayout.EndHorizontal();
        }
        
        public void CreateChians(SerializedProperty prop)
        {
            if (prop.arraySize != 0) return;
            var transList = SeinoTools.CreateBoneChains(Selection.activeTransform);
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