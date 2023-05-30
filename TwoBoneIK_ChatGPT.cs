using UnityEngine;

namespace XenoIK
{
    
public class TwoBoneIK_ChatGPT : MonoBehaviour
{
    public Transform startBone;
    public Transform midBone;
    public Transform endBone;
    public Transform target;

    public float lengthA;
    public float lengthB;

    public float epsilon = 0.01f;
    public int maxIterations = 10;

    private void LateUpdate()
    {
        SolveIK();
    }

    private void SolveIK()
    {
        Vector3 targetPos = target.position;
        Vector3 startBonePos = startBone.position;
        Vector3 midBonePos = midBone.position;
        Vector3 endBonePos = endBone.position;

        // Calculate the direction vectors between the bones
        Vector3 dirA = midBonePos - startBonePos;
        Vector3 dirB = endBonePos - midBonePos;
        Vector3 dirTarget = targetPos - startBonePos;

        // Calculate the lengths of the bones
        float lenA = dirA.magnitude;
        float lenB = dirB.magnitude;

        // If the target is too far away, set it to be at the maximum possible distance
        if (dirTarget.magnitude > lenA + lenB)
        {
            dirTarget = dirTarget.normalized * (lenA + lenB);
        }

        // Calculate the angles of the two triangles in the IK system
        float angleA = Vector3.Angle(dirTarget, dirA);
        float angleB = Vector3.Angle(dirA, dirB);

        // If the target is too close to the start bone, the angles may be NaN, so we need to handle that
        if (float.IsNaN(angleA) || float.IsNaN(angleB))
        {
            return;
        }

        // If the target is unreachable, set the midpoint to be at the maximum possible distance
        if (angleA + angleB > 180f - epsilon)
        {
            dirA = dirA.normalized * lenA;
            dirB = dirB.normalized * lenB;
            midBone.position = startBonePos + dirA + dirB;
            return;
        }

        // Calculate the rotation axis and angle for the first bone
        Vector3 axisA = Vector3.Cross(dirTarget, dirA).normalized;
        float angleAxisA = Mathf.Min(angleA, 180f - epsilon);

        // Calculate the rotation axis and angle for the second bone
        Vector3 axisB = Vector3.Cross(dirA, dirB).normalized;
        float angleAxisB = Mathf.Min(angleB, 180f - epsilon);

        // Apply the rotations to the bones
        Quaternion rotA = Quaternion.AngleAxis(angleAxisA, axisA);
        Quaternion rotB = Quaternion.AngleAxis(-angleAxisB, axisB);
        midBone.rotation = startBone.rotation * rotA;
        endBone.rotation = midBone.rotation * rotB;

        // Keep the start bone and end bone in place
        startBone.position = startBonePos;
        endBone.position = endBonePos;
    }
}

}