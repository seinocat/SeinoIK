using System;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public class LookAtBone : Bone
    {
        public void Init(Transform root)
        {
            if (this.transform == null) return;
            this.axis = Quaternion.Inverse(this.transform.rotation) * root.forward;
        }

        public void LookAt(Vector3 target, float weight)
        {
            Quaternion rotate = Quaternion.FromToRotation(this.Forward, target);
            Quaternion curRotate = this.transform.rotation;
            this.transform.rotation = Quaternion.Lerp(curRotate, rotate * curRotate, weight);
        }
        
        public Vector3 Forward => this.transform.rotation * this.axis;
    }
}