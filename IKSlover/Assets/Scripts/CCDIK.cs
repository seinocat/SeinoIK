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

        public int Itera = 5;

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
                
                // 轴旋转
                Vector3 axis = Vector3.Cross(toEffectorVec, toTargetVec).normalized;
                float angle = Vector3.Angle(toEffectorVec, toTargetVec);
                Quaternion qua = Quaternion.AngleAxis(angle, axis) * curBone.Transform.rotation;
                curBone.Transform.rotation = qua;

                // 四元数
                // Quaternion qua = Quaternion.FromToRotation(toEffectorVec, toTargetVec) * curBone.Transform.rotation;
                // curBone.Transform.rotation = qua;
            }
        }

        private void Update()
        {
            for (int i = 0; i < Itera; i++)
            {
                CCDSolver();
            }
        }
    }
}