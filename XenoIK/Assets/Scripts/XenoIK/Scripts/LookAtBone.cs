using System;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public class LookAtBone : Bone
    {
        public void Init(Transform root)
        {
            if (transform == null) return;
            axis = Quaternion.Inverse(transform.rotation) * root.forward;
        }

        public void LookAt(Vector3 target, float weight)
        {
            Quaternion rotate = Quaternion.FromToRotation(this.Forward, target);
            Quaternion curRotate = this.transform.rotation;
            this.transform.rotation = weight >= 1 ? rotate * curRotate : Quaternion.Lerp(curRotate, curRotate * rotate, weight);
        }


        public Vector3 Forward => transform.rotation * axis;
    }
}