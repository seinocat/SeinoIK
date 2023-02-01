using System;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public class LookAtBone : Bone
    {
        private Quaternion defaultRotation;
        private Quaternion defaultRootRotation;
        
        public void Init(Transform root, Vector3 defaultAxis)
        {
            if (this.transform == null) return;
            this.defaultRotation = this.transform.rotation;
            this.defaultRootRotation = root.rotation;
            this.axis = Quaternion.Inverse(this.transform.rotation) * (root.rotation * defaultAxis);
        }

        public void UpdateAxis(Vector3 newAxis)
        {
            this.axis = Quaternion.Inverse(this.defaultRotation) * (this.defaultRootRotation * newAxis);
        }

        public void LookAt(Vector3 target, float targetWeight)
        {
            Quaternion rotate = Quaternion.FromToRotation(this.Forward, target);
            Quaternion curRotate = this.transform.rotation;
            this.transform.rotation = Quaternion.Lerp(curRotate, rotate * curRotate, targetWeight * this.weight);
        }
        
        public Vector3 Forward => this.transform.rotation * this.axis;
    }
}