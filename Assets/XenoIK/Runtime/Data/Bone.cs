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


        public Vector3 defaultLocalPosition;
        public Quaternion defaultLocalRotation;

        public void StoreDefaultLocalState()
        {
            this.defaultLocalPosition = this.transform.localPosition;
            this.defaultLocalRotation = this.transform.localRotation;
        }

        public void FixTransform()
        {
            if (this.transform.localPosition != defaultLocalPosition) transform.localPosition = this.defaultLocalPosition;
            if (this.transform.localRotation != defaultLocalRotation) transform.localRotation = this.defaultLocalRotation;
        }

        public Vector3 Position
        {
            get => this.transform != null ? this.transform.position : Vector3.zero;
            set => this.transform.position = value;
        }
        
        public Vector3 LocalPosition
        {
            get => this.transform != null ? this.transform.localPosition : Vector3.zero;
            set => this.transform.localPosition = value;
        }

        public Quaternion Rotation
        {
            get => this.transform.rotation;
            set => this.transform.rotation = value;
        }

        public Quaternion LocalRotation
        {
            get => this.transform.localRotation;
            set => this.transform.localRotation = value;
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