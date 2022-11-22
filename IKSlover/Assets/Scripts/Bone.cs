using Sirenix.OdinInspector;
using UnityEngine;

namespace IKSlover
{
    public class Bone
    {
        [LabelText("Transform")]
        public Transform Transform;
        
        [LabelText("骨骼长度")]
        public float Length;

        public Bone()
        {
            
        }

        public Bone(Transform trans)
        {
            this.Transform = trans;
        }
    }
}