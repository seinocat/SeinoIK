using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace XenoIK
{
    public class IKSolver
    {
        protected virtual void Solve(){}
        
        [LabelText("目标物体")]
        public Transform TargetTrans;
        
        [LabelText("骨骼链")]
        public List<Transform> BoneTransList;

        public List<Bone> Bones;

        public Bone Effector;

        public int Itera = 5;

        public void Awake()
        {
            this.InitBones();
        }
        
        private void Update()
        {
            for (int i = 0; i < Itera; i++)
            {
                Solve();
            }
        }
        
        private void InitBones()
        {
            this.Bones ??= new List<Bone>();
            this.Bones.Clear();
            for (int i = 0; i < this.BoneTransList.Count; i++)
            {
                this.Bones.Add(new Bone(this.BoneTransList[i]));
            }

            this.Effector = this.Bones[this.Bones.Count - 1];
        }
        
    
    }
}