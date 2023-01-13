using System;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public class Bone
    {
        public Transform transform;
        
        public float length;
        [Range(0, 1f)]
        public float weight = 1f;

        public Vector3 solverPosition;

        public Vector3 Position
        {
            get => this.transform.position;
            set => this.transform.position = value;
        }

        public Quaternion Rotation
        {
            get => this.transform.rotation;
            set => this.transform.rotation = value;
        }

        public Bone() { }

        public Bone(Transform trans)
        {
            this.transform = trans;
        }
    }
}