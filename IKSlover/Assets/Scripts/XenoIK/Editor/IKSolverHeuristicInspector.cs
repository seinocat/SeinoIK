using UnityEditor;
using UnityEngine;

namespace XenoIK.Editor
{
    public class IKSolverHeuristicInspector : IKSolverInspector
    {
        private static SerializedProperty Prop;

        public static void AddInspector(SerializedProperty prop)
        {
            Prop = prop;
            AddTarget();
            AddIKWeight();
            AddProps();
        }
        
        public static void AddTarget()
        {
            EditorGUILayout.PropertyField(Prop.FindPropertyRelative("Target"), new GUIContent("目标物体"));
        }

        public static void AddIKWeight()
        {
            EditorGUILayout.PropertyField(Prop.FindPropertyRelative("IKWeight"), new GUIContent("权重"));
        }

        public static void AddProps()
        {
            EditorGUILayout.PropertyField(Prop.FindPropertyRelative("MaxIterations"), new GUIContent("最大迭代次数"));
        }

        public static void AddBones()
        {
            
        }
    }
}