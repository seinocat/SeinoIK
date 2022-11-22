using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace IKSlover
{
    public class CCDIK : MonoBehaviour
    {
        [LabelText("目标物体")]
        public Transform TargetTrans;
        
        [LabelText("骨骼链")]
        public List<Transform> BoneTransList;

        public List<Bone> Bones;

        public void Awake()
        {
            this.InitBones();
        }
        
        private void InitBones()
        {
            this.Bones ??= new List<Bone>();
            this.Bones.Clear();
            for (int i = 0; i < this.BoneTransList.Count; i++)
            {
                this.Bones.Add(new Bone(this.BoneTransList[i]));
            }
        }

        private void Update()
        {
            
        }
    }
}