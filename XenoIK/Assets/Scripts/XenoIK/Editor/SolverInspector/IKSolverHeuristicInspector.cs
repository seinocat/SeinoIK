using System.Collections;
using UnityEditor;
using UnityEngine;

namespace XenoIK.Editor
{
    public class IKSolverHeuristicInspector : IKSolverInspector
    {

        public bool IsShowList;
        
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
            
            DrawElements(solver.FindPropertyRelative("bones"), new GUIContent("骨骼"), DrawBones, XenoTools.DrawButton);
        }
        
        private static void DrawBones(SerializedProperty bone, int index)
        {
            AddObjectReference(bone.FindPropertyRelative("transform"), GUIContent.none, 0, 120);
            DrawWightSlider(bone.FindPropertyRelative("weight"));
        }
        
        private static void DrawWightSlider(SerializedProperty Weight)
        {
            GUILayout.Label("权重", GUILayout.Width(45));
            EditorGUILayout.PropertyField(Weight, GUIContent.none);
        }
    }
}