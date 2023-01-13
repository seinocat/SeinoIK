using UnityEditor;
using UnityEngine;

namespace XenoIK.Editor
{
    public class IKSolverInspector : Inspector
    {
        public static float GetHandleSize(Vector3 position) {
            float s = HandleUtility.GetHandleSize(position) * 0.1f;
            return Mathf.Lerp(s, 0.025f, 0.5f);
        }
    }
}