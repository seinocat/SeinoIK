using UnityEngine;

namespace XenoIK
{
    [RequireComponent(typeof(LookAtIK))]
    public class LookAtCtrl : MonoBehaviour
    {
        [Range(0, 360)]
        public float angle;
        [Range(0, float.MaxValue)]
        public float maxDistance;
        [Range(0, float.MaxValue)]
        public float minDistance;
        
        [Range(0, float.MaxValue)]
        public float fadeInTime;
        [Range(0, float.MaxValue)]
        public float fadeOutTime;
        public AnimationCurve fadeInCurve = new AnimationCurve(new Keyframe(0,0,1,1), new Keyframe(1,1,1,1));
        public AnimationCurve fadeOutCurve = new AnimationCurve(new Keyframe(0,0,1,1), new Keyframe(1,1,1,1));
    }
}