using UnityEditor;
using UnityEngine;

namespace XenoIK.Editor
{
    public class IKSolverLookAtInspector : IKSolverInspector
    {
        private static MonoBehaviour script;
        private static bool FadeFoldout = true;
        
        public static void DrawInspector(SerializedProperty prop, MonoBehaviour mono)
        {
            script = mono;
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("target"), new GUIContent("注视目标"));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("IKWeight"), new GUIContent("全局权重"));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("headWeight"), new GUIContent("头部权重"));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("eyesWeight"), new GUIContent("眼睛权重"));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("bodyWeight"), new GUIContent("身体权重"));

            FadeFoldout = EditorGUILayout.Foldout(FadeFoldout, "配置相关");
            if (FadeFoldout)
            {
                for (int i = 0; i < 6; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space((EditorGUI.indentLevel + 1) * 15);
                    if(i == 0)EditorGUILayout.PropertyField(prop.FindPropertyRelative("angle"), new GUIContent("有效角度"));
                    if(i == 1)EditorGUILayout.PropertyField(prop.FindPropertyRelative("distance"), new GUIContent("有效距离"));
                    if(i == 2)EditorGUILayout.PropertyField(prop.FindPropertyRelative("fadeInTime"), new GUIContent("渐入时间"));
                    if(i == 3)EditorGUILayout.PropertyField(prop.FindPropertyRelative("fadeOutTime"), new GUIContent("渐出时间"));
                    if(i == 4)EditorGUILayout.PropertyField(prop.FindPropertyRelative("fadeInCurve"), new GUIContent("渐入曲线"));
                    if(i == 5)EditorGUILayout.PropertyField(prop.FindPropertyRelative("fadeOutCurve"), new GUIContent("渐出曲线"));
                    GUILayout.EndHorizontal();
                }
            }
            
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("head.transform"), new GUIContent("头骨骼"));
            DrawElements(prop.FindPropertyRelative("eyes"), 0, new GUIContent("眼睛"), DrawLookAtBone, DrawEyeBtns);
            DrawElements(prop.FindPropertyRelative("spines"), 1, new GUIContent("身体"), DrawLookAtBone, DrawSpineBtns);
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
        
        public static void DrawSceneGUI(IKSolverLookAt solver)
        {
            if (Application.isPlaying && !solver.initiated) return;

            //Draw Head
            if (solver.head.transform != null)
            {
                Handles.color = Color.green;
                Handles.SphereHandleCap(0, solver.head.Position, Quaternion.identity, GetHandleSize(solver.head.Position), EventType.Repaint);
                //Draw Target
                Handles.SphereHandleCap(0, solver.IKPosition, Quaternion.identity, GetHandleSize(solver.IKPosition), EventType.Repaint);
                //Draw Head to Tagert line
                Handles.DrawLine(solver.head.Position, solver.IKPosition, 1);
            }
            
            //Draw Spines
            for (int i = 0; i < solver.spines.Count; i++)
            {
                var bone = solver.spines[i];
                if (bone.transform == null) continue;
                Handles.color = XenoTools.jointColor;
                Handles.SphereHandleCap(0, bone.Position, Quaternion.identity, GetHandleSize(bone.Position), EventType.Repaint);
                Handles.color = XenoTools.boneColor;
                if (i < solver.spines.Count - 1) Handles.DrawLine(bone.Position, solver.spines[i+1].Position, 3);
            }
            
            Handles.color = Color.white;
        }
    }
}