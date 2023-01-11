using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;

namespace XenoIK
{
    public class Bone
    {
        [LabelText("Transform")]
        public Transform Transform;
        
        [LabelText("骨骼长度")]
        public float Length;

        public Vector3 Position
        {
            get => this.Transform.position;
            set => this.Transform.position = value;
        }

        public Bone()
        {
            
        }

        public Bone(Transform trans)
        {
            this.Transform = trans;
        }
    }
}