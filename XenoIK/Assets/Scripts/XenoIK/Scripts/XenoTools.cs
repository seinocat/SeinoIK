using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XenoIK.Editor;

namespace XenoIK
{
    public static class XenoTools 
    {
        
        public static Color jointColor = Color.cyan;
        public static Color boneColor = Color.cyan;

        public static Bone FindLastBone(this List<Bone> list)
        {
            return list.Count == 0 ? null : list[list.Count - 1];
        }
        
        public static LookAtBone FindLastBone(this List<LookAtBone> list)
        {
            return list.Count == 0 ? null : list[list.Count - 1];
        }

        public static List<Transform> CreateBoneChains(Transform rootBone, int boneNum = 4)
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

        public static Transform FindTargetBone(Transform root, string bone)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);
                if (child.name.ToLower().Contains(bone))
                    return child;
                var target = FindTargetBone(child, bone);
                if (target != null)
                    return target;
            }

            return null;
        }
    }
}