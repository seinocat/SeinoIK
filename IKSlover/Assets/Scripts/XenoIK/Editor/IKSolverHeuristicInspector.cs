using System.Collections;
using UnityEditor;
using UnityEngine;

namespace XenoIK.Editor
{
    public class IKSolverHeuristicInspector : IKSolverInspector
    {
        private static SerializedProperty Solver;

        public static void AddInspector(SerializedProperty prop)
        {
            Solver = prop;
            AddTarget();
            AddIKWeight();
            AddProps();
            AddBones();
        }
        
        public static void AddTarget()
        {
            EditorGUILayout.PropertyField(Solver.FindPropertyRelative("Target"), new GUIContent("目标物体"));
        }

        public static void AddIKWeight()
        {
            EditorGUILayout.PropertyField(Solver.FindPropertyRelative("IKWeight"), new GUIContent("全局权重"));
        }

        public static void AddProps()
        {
            EditorGUILayout.PropertyField(Solver.FindPropertyRelative("MaxIterations"), new GUIContent("最大迭代次数"));
        }

        public static void AddBones()
        {
            AddList(Solver.FindPropertyRelative("Bones"), new GUIContent("骨骼"), null, 
                OnAddBone, DrawElementBones, OnMoveBone, CreateChians);
        }
        
        private static void DrawElementBones(SerializedProperty bone, int index)
        {
            var boneTrans = bone.FindPropertyRelative("Transform");
            AddObjectReference(boneTrans, new GUIContent(""), 0, 120);
            AddWightSlider(bone.FindPropertyRelative("Weight"));
        }

        private static void OnAddBone(SerializedProperty bone)
        {
            bone.FindPropertyRelative("Weight").floatValue = 1;
        }

        private static void AddWightSlider(SerializedProperty Weight)
        {
            GUILayout.Label("权重", GUILayout.Width(45));
            EditorGUILayout.PropertyField(Weight, GUIContent.none);
        }

        private static void OnMoveBone(SerializedProperty prop)
        {
            
        }

        private static void CreateChians(SerializedProperty prop)
        {
            bool NewChain = prop.arraySize == 0;
            var transList = Utility.CreateBoneChains(Selection.activeTransform, NewChain ? 5 : prop.arraySize);
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
                        var bone = element.FindPropertyRelative("Transform");
                        bone.objectReferenceValue = transList[i];
                    }
                }
            }
        }
        
    }
}