using System.Collections.Generic;
using UnityEngine;

namespace XenoIK
{
    public static class Utility 
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
    }
}