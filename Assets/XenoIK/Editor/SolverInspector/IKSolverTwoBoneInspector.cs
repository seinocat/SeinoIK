using UnityEditor;
using UnityEngine;

namespace XenoIK.Editor
{
    public class IKSolverTwoBoneInspector : IKSolverInspector
    {
        private static TwoBoneIK script;
        
        public void DrawInspector(SerializedProperty prop, TwoBoneIK mono)
        {
            script = mono;

            DrawTwoBoneList(prop);
        }

        public void DrawTwoBoneList(SerializedProperty prop)
        {
            DrawElements(prop.FindPropertyRelative("TwoBoneList"), 0, new GUIContent("骨骼列表"), DrawTwoBoneList, null, false);
        }

        public void DrawTwoBoneList(SerializedProperty bone, int index)
        {
            EditorGUILayout.PropertyField(bone.FindPropertyRelative("transform"), GUIContent.none, GUILayout.Width(120));
        }
    }
}