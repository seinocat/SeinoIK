using System;
using System.Collections.Generic;
using SeinoIK.Runtime.Enum;
using UnityEngine;

namespace SeinoIK
{
    public static class SeinoTools 
    {
        public static Color jointColor = Color.cyan;
        public static Color boneColor = Color.cyan;
        private static string m_Pelvis = "pelvis";
        private static string m_Hips = "hips";
        private static string m_L_Foot1 = "left_foot";
        private static string m_L_Foot2 = "l foot";
        private static string m_R_Foot1 = "right_foot";
        private static string m_R_Foot2 = "r foot";

        // #region Math
        //
        // /// <summary>
        // /// 线性插值
        // /// </summary>
        // /// <param name="value"></param>
        // /// <param name="target"></param>
        // /// <param name="increaseSpeed"></param>
        // /// <param name="decreaseSpeed"></param>
        // /// <returns></returns>
        // public static float LerpValue(float value, float target, float increaseSpeed, float decreaseSpeed) {
        //     if (Math.Abs(value - target) < 0.01f) return target; 
        //     if (value < target) return Mathf.Clamp(value + Time.deltaTime * increaseSpeed, -Mathf.Infinity, target);
        //     return Mathf.Clamp(value - Time.deltaTime * decreaseSpeed, target, Mathf.Infinity);
        // }
        //
        // /// <summary>
        // /// 非线性插值
        // /// </summary>
        // /// <param name="value"></param>
        // /// <param name="target"></param>
        // /// <param name="velocity"></param>
        // /// <param name="time"></param>
        // /// <param name="type"></param>
        // /// <returns></returns>
        // public static float LerpDamper(float value, float target, ref float velocity, float time, LerpType type)
        // {
        //     switch (type)
        //     {
        //         case LerpType.SampleDamper:
        //             return SampleDamper(value, target, ref velocity, time);
        //         case LerpType.Damper:
        //             return Mathf.SmoothDamp(value, target, ref velocity, time);
        //     }
        //
        //     return 0;
        // }
        //
        // /// <summary>
        // /// 简化临界阻尼插值
        // /// </summary>
        // /// <param name="value"></param>
        // /// <param name="target"></param>
        // /// <param name="velocity"></param>
        // /// <param name="time"></param>
        // /// <returns></returns>
        // public static float SampleDamper(float value, float target, ref float velocity, float time)
        // {
        //     var omega = 2.0f / time;
        //     float x = omega * Time.deltaTime;
        //     float exp = 1.0f / (1.0f + x + 0.48f * x * x + 0.235f * x * x * x);
        //     float change = value - target;
        //     float temp = (velocity + omega * change) * Time.deltaTime;
        //     velocity = (velocity - omega * temp) * exp;
        //     return target + (change + temp) * exp;
        // }
        //
        // /// <summary>
        // /// 求直线和平面的交点
        // /// p = R + tV
        // /// </summary>
        // /// <param name="linePoint">直线上的点</param>
        // /// <param name="lineDir">直线方向向量(需要归一化)</param>
        // /// <param name="planeNormal">平面法线</param>
        // /// <param name="planePoint">平面上的点</param>
        // /// <returns></returns>
        // public static Vector3 LineToPlane(Vector3 linePoint, Vector3 lineDir, Vector3 planeNormal, Vector3 planePoint)
        // {
        //     lineDir = lineDir.normalized;
        //     float dot1 = Vector3.Dot(planeNormal, planePoint - linePoint);
        //     float dot2 = Vector3.Dot(planeNormal, lineDir);
        //
        //     if (dot2 == 0f) return Vector3.zero;
        //
        //     float t = dot1 / dot2;
        //     return linePoint + t * lineDir;
        // }
        //
        // /// <summary>
        // /// 三角余弦定律，求角度(注意不是弧度)
        // /// </summary>
        // /// <param name="sideA"></param>
        // /// <param name="sideB"></param>
        // /// <param name="sideC"></param>
        // /// <returns></returns>
        // public static float CosineTriangle(float sideA, float sideB, float sideC)
        // {
        //     float cosA = Mathf.Clamp((sideB * sideB + sideC * sideC - sideA * sideA) / (2 * sideB * sideC), -1f, 1f);
        //     return Mathf.Acos(cosA) * Mathf.Rad2Deg;
        // }
        //
        // #endregion

        #region runtime

        public static LookAtBone FindLastBone(this List<LookAtBone> list)
        {
            return list.Count == 0 ? null : list[^1];
        }
        
        public static LookAtCtrlMgr RequireLookAtMgr(this GameObject go)
        {
            var component = go.GetComponent<LookAtCtrlMgr>();
            if (component == null) component = go.AddComponent<LookAtCtrlMgr>();
            component.OnInit();
            return component;
        }


        #endregion
        
        #region Editor

#if UNITY_EDITOR
        
        public static float GetHandleSize(Vector3 position) {
            float s = UnityEditor.HandleUtility.GetHandleSize(position) * 0.1f;
            return Mathf.Lerp(s, 0.025f, 0.5f);
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

        /// <summary>
        /// 查找指定名称的骨骼
        /// </summary>
        /// <param name="root"></param>
        /// <param name="bone"></param>
        /// <param name="equal"></param>
        /// <returns></returns>
        public static Transform FindTargetBone(Transform root, string bone, bool equal = false)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);
                if (equal)
                {
                    if (child.name.ToLower().Equals(bone))
                        return child;
                }
                else
                {
                    if (child.name.ToLower().Contains(bone))
                        return child;
                }
                var target = FindTargetBone(child, bone);
                if (target != null)
                    return target;
            }

            return null;
        }
        
        public static Transform FindPelvis(this Transform root)
        {
            var pelvis = FindTargetBone(root, m_Pelvis);
            if (pelvis == null) pelvis = FindTargetBone(root, m_Hips);

            return pelvis;
        }

        public static List<Transform> FindGoalBone(Transform root, AvatarIKGoal goal)
        {
            List<Transform> bones = new List<Transform>();
            switch (goal)
            {
                case AvatarIKGoal.LeftFoot:
                    FindFoot(root, true, ref bones);
                    break;
                case AvatarIKGoal.RightFoot:
                    FindFoot(root, false, ref bones);
                    break;
                case AvatarIKGoal.LeftHand:
                    break;
                case AvatarIKGoal.RightHand:
                    break;
            }

            return bones;
        }

        private static void FindFoot(Transform root, bool isLeft, ref List<Transform> bones)
        {
            var foot = FindTargetBone(root, isLeft ? m_L_Foot1 : m_R_Foot1);
            if (foot == null) foot = FindTargetBone(root, isLeft ? m_L_Foot2 : m_R_Foot2);
            if (foot != null)
            {
                var calf = foot.parent;
                var thigh = calf.parent;
                bones.Add(thigh);
                bones.Add(calf);
                bones.Add(foot);
            }
        }
#endif
        #endregion
        
        
    }
}