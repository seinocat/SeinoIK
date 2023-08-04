using System;
using System.Collections.Generic;
using UnityEngine;

namespace XenoIK
{
    public static class XenoTools 
    {

        #region Math

        public static float LerpValue(float value, float target, float increaseSpeed, float decreaseSpeed) {
            if (Math.Abs(value - target) < 0.01f) return target; 
            if (value < target) return Mathf.Clamp(value + Time.deltaTime * increaseSpeed, -Mathf.Infinity, target);
            return Mathf.Clamp(value - Time.deltaTime * decreaseSpeed, target, Mathf.Infinity);
        }
        
        /// <summary>
        /// 求直线和平面的交点
        /// p = r + td
        /// </summary>
        /// <param name="linePoint">直线上的点</param>
        /// <param name="lineDir">直线方向向量(需要归一化)</param>
        /// <param name="planeNormal">平面法线</param>
        /// <param name="planePoint">平面上的点</param>
        /// <returns></returns>
        public static Vector3 LineToPlane(Vector3 linePoint, Vector3 lineDir, Vector3 planeNormal, Vector3 planePoint)
        {
            lineDir = lineDir.normalized;
            float dot1 = Vector3.Dot(planeNormal, planePoint - linePoint);
            float dot2 = Vector3.Dot(planeNormal, lineDir);

            if (dot2 == 0f) return Vector3.zero;
    
            float t = dot1 / dot2;
            return linePoint + t * lineDir;
        }
        
        /// <summary>
        /// 三角余弦定律，求角度(注意不是弧度)
        /// </summary>
        /// <param name="sideA"></param>
        /// <param name="sideB"></param>
        /// <param name="sideC"></param>
        /// <returns></returns>
        public static float CosineTriangle(float sideA, float sideB, float sideC)
        {
            float cosA = Mathf.Clamp((sideB * sideB + sideC * sideC - sideA * sideA) / (2 * sideB * sideC), -1f, 1f);
            return Mathf.Acos(cosA) * Mathf.Rad2Deg;


            
        }

        #endregion

        #region Editor

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
        
        public static LookAtCtrlMgr RequireLookAtMgr(this GameObject go)
        {
            var component = go.GetComponent<LookAtCtrlMgr>();
            if (component == null) component = go.AddComponent<LookAtCtrlMgr>();
            component.OnInit();
            return component;
        }

        #endregion
        
        
    }
}