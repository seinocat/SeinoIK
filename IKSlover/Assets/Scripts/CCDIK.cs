using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Mathematics;
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

        public Bone Effector;

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

            this.Effector = this.Bones[this.Bones.Count - 1];
        }


        private void CCDSolver()
        {
            for (int i = this.Bones.Count - 2; i >= 0; i--)
            {
                var curBone = this.Bones[i];
                Vector3 toEffectorVec = this.Effector.Position - curBone.Position;
                Vector3 toTargetVec = this.TargetTrans.transform.position - curBone.Position;
                
                // 此段代码会发生抖动
                // Vector3 axis = Vector3.Cross(toEffectorVec, toTargetVec);
                // float angle = Mathf.Acos(Vector3.Dot(toEffectorVec, toTargetVec));
                // Quaternion qua = Quaternion.AngleAxis(angle, axis);
                // curBone.Transform.rotation *= qua;
                
                Quaternion qua = Quaternion.FromToRotation(toEffectorVec, toTargetVec) * curBone.Transform.rotation;
                curBone.Transform.rotation = qua;
            }
        }

        private void LateUpdate()
        {
            CCDSolver();
        }
    }
}