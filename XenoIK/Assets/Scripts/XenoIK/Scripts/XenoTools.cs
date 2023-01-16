using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XenoIK.Editor;

namespace XenoIK
{
    public static class XenoTools 
    {
        public static List<Transform> CreateBoneChains(Transform rootBone, int boneNum)
        {
            int totalNums = boneNum;
            List<Transform> chains = new List<Transform>();
            chains.Add(rootBone);
            totalNums--;
            Transform curBone = rootBone;

            while (curBone.parent && totalNums > 0)
            {
                chains.Add(curBone.parent);
                curBone = curBone.parent;
                totalNums--;
            }

            chains.Reverse();
            return chains;
        }

        public static void DrawButton(SerializedProperty prop)
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

        public static void OnAddBone(SerializedProperty bone)
        {
            bone.FindPropertyRelative("weight").floatValue = 1;
        }

        public static void CreateChians(SerializedProperty prop)
        {
            bool NewChain = prop.arraySize == 0;
            var transList = CreateBoneChains(Selection.activeTransform, NewChain ? 5 : prop.arraySize);
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