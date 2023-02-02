using System;
using UnityEngine;

namespace XenoIK.Runtime.Config
{
    [CreateAssetMenu(fileName = "New LookAt Config", menuName = "Xeno IK/Create LookAt Config")]
    public class LookAtConfig : IKConfig
    {
        public Vector3 headAxis = Vector3.forward;
        public Vector3 eyesAxis = Vector3.forward;
        public Vector3 spinesAxis = Vector3.forward;
        
        [Tooltip("When the target out of range,  hold look at within followAngle")]
        public bool holdLookAt;
        

        public float maxDistance = 10000;
        public float minDistance = 1;
        
        [Range(0, 360)]
        public float lookAtSpeed = 3f;
        
        [Range(0, 360)]
        public float lookAwaySpeed;
        
        public Vector2 detectAngleXZ = new Vector2(120, 90);
        public Vector2 followAngleXZ = new Vector2(120, 90);
        public float smoothWeightTime = 0.25f;
        public float smoothWeightSpeed = 0.5f;
        public float switchWeightTime = 0.25f;
        public float switchWeightSpeed;
        public float maxRadiansDelta = 3f;
        public float maxMagnitudeDelta = 3f;
        public Vector3 pivotOffset = Vector3.up;
        public Vector3 offset;

        #region Animation Curve

        public bool useCustomCurve = false;
        public AnimationCurve lookAtCurve = new AnimationCurve(new Keyframe(0,0,1,1), new Keyframe(1,1,1,1));
        public AnimationCurve lookAwayCurve = new AnimationCurve(new Keyframe(0,0,1,1), new Keyframe(1,1,1,1));
        public float curveWeightTime = 0.25f;
        public float curveWeightSpeed = 0.05f;

        #endregion
    }
}