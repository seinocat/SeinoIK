using System;
using System.Collections.Generic;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public class IKSolverLookAt : IKSolver
    {
        public Transform target;
        public LookAtBone head;
        public List<LookAtBone> eyes;
        public List<LookAtBone> spines;

        [Range(0, 360)]
        public float angle;
        [Range(0, float.MaxValue)]
        public float distance;
        [Range(0, float.MaxValue)]
        public float fadeInTime;
        [Range(0, float.MaxValue)]
        public float fadeOutTime;
        public AnimationCurve fadeInCurve = new AnimationCurve(new Keyframe(0,0,1,1), new Keyframe(1,1,1,1));
        public AnimationCurve fadeOutCurve = new AnimationCurve(new Keyframe(0,0,1,1), new Keyframe(1,1,1,1));


        protected override void OnInitialize()
        {
            
        }

        protected override void OnUpdate(float deltaTime)
        {
            
        }
    }
}