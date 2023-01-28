using UnityEngine;

namespace XenoIK
{
    [RequireComponent(typeof(LookAtIK))]
    public class LookAtCtrl : MonoBehaviour
    {
        
        public LookAtIK lookAtIK;

        public Transform target;
        
        [Range(0, 360)]
        public float angle = 180;
        [Range(0, 100000)]
        public float maxDistance = 10000;
        [Range(0, 100000)]
        public float minDistance = 1;
        
        [Range(0, float.MaxValue)]
        public float lookAtSpeed;
        [Range(0, float.MaxValue)]
        public float lookAwaySpeed;
        
        public AnimationCurve lookAtCurve = new AnimationCurve(new Keyframe(0,0,1,1), new Keyframe(1,1,1,1));
        public AnimationCurve lookAwayCurve = new AnimationCurve(new Keyframe(0,0,1,1), new Keyframe(1,1,1,1));
    }
}