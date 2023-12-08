using UnityEditor;
using UnityEngine;

namespace SeinoIK.Editor
{
    public class IKSolverLookAtInspector : IKSolverInspector
    {
        private LookAtIK script;
        private bool showAxis;
        private bool showWeight;
        
        public void DrawInspector(SerializedProperty prop, LookAtIK mono)
        {
            script = mono;
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("target"), new GUIContent("注视目标"));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("IKWeight"), new GUIContent("全局权重"));
            DrawAxis(prop);
            DrawWeight(prop);
            DrawBone(prop);
        }

        public void DrawAxis(SerializedProperty prop)
        {
            showAxis = EditorGUILayout.Foldout(showAxis, "轴向配置");
            if (showAxis)
            {
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("root"), new GUIContent("根节点"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("headAxis"), new GUIContent("头轴向"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("eyesAxis"), new GUIContent("眼睛轴向"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("spinesAxis"), new GUIContent("身体轴向"));
            }
        }

        public void DrawWeight(SerializedProperty prop)
        {
            EditorGUILayout.Space(5);
            showWeight = EditorGUILayout.Foldout(showWeight, "权重配置");
            if (showWeight)
            {
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("headWeight"), new GUIContent("头部权重"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("eyesWeight"), new GUIContent("眼睛权重"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("bodyWeight"), new GUIContent("身体权重"));
            }
        }

        public void DrawBone(SerializedProperty prop)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("head.transform"), new GUIContent("头"));
            DrawElements(prop.FindPropertyRelative("eyes"), 0, new GUIContent("眼睛"), DrawLookAtBone, DrawEyeBtns);
            DrawElements(prop.FindPropertyRelative("spines"), 1, new GUIContent("身体"), DrawLookAtBone, DrawSpineBtns);
        }
        
        public void DrawLookAtBone(SerializedProperty bone, int index)
        {
            EditorGUILayout.PropertyField(bone.FindPropertyRelative("transform"), GUIContent.none, GUILayout.Width(120));
            DrawWightSlider(bone.FindPropertyRelative("weight"));
        }
        
        private void DrawWightSlider(SerializedProperty Weight)
        {
            GUILayout.Label("权重", GUILayout.Width(45));
            EditorGUILayout.PropertyField(Weight, GUIContent.none);
        }
        
        public void DrawEyeBtns(SerializedProperty prop)
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawCommonBtn(prop);
            GUILayout.EndHorizontal();
        }

        public void DrawSpineBtns(SerializedProperty prop)
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
        
        public void DrawCommonBtn(SerializedProperty prop)
        {
            if (GUILayout.Button(new GUIContent("删除全部", "删除全部列表元素"), EditorStyles.toolbarButton, GUILayout.Width(IKSolverInspector.CBtnWidth)))
            {
                if (EditorUtility.DisplayDialog("警告", "是否清空列表", "确认", "取消"))
                {
                    prop.arraySize = 0;
                }
            }
        }
        
        
        public void CreateSpines(SerializedProperty prop)
        {
            if (prop.arraySize != 0) return;

            var root = script.transform;
            var head = SeinoTools.FindTargetBone(root, "head");
            var transList = SeinoTools.CreateBoneChains(head.parent);
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
        
        public void DrawSceneGUI(IKSolverLookAt solver)
        {
            if (Application.isPlaying && !solver.initiated) return;
            
            //Draw Head
            if (solver.head.transform != null)
            {
                Handles.color = Color.green;
                Handles.SphereHandleCap(0, solver.head.Position, Quaternion.identity, GetHandleSize(solver.head.Position), EventType.Repaint);
                //Draw target
                Handles.SphereHandleCap(0, solver.IKPosition, Quaternion.identity, GetHandleSize(solver.IKPosition), EventType.Repaint);
                //Draw head to tagert line
                Handles.DrawLine(solver.head.Position, solver.IKPosition, 1);
                //Draw axis line
                Handles.color = Color.red;
                Handles.DrawLine(solver.head.Position, solver.head.Position + solver.head.Forward * 5f);
                
                Handles.color = Color.green;
                Handles.DrawLine(solver.head.Position, solver.head.Position + solver.root.transform.forward * 5f);
            }
            
            //Draw Spines
            for (int i = 0; i < solver.spines.Count; i++)
            {
                var bone = solver.spines[i];
                if (bone.transform == null) continue;
                Handles.color = SeinoTools.jointColor;
                Handles.SphereHandleCap(0, bone.Position, Quaternion.identity, GetHandleSize(bone.Position), EventType.Repaint);
                Handles.color = SeinoTools.boneColor;
                if (i < solver.spines.Count - 1) Handles.DrawLine(bone.Position, solver.spines[i+1].Position, 3);
            }
            
            Handles.color = Color.white;
        }
    }
}