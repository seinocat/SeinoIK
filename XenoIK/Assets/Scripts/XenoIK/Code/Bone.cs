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

        public Vector3 axis;
        public Vector3 solverPosition;
        public Quaternion solverRotation;

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

        public void Swing(Vector3 targetPos, float weight = 1f)
        {
            if (weight <= 0f) return;
            Quaternion rotation = Quaternion.FromToRotation(this.Rotation * this.axis, targetPos - this.Position);
            this.Rotation = weight >= 1f
                ? rotation * this.Rotation
                : Quaternion.Lerp(Quaternion.identity, rotation * this.Rotation, weight);
        }
    }
}