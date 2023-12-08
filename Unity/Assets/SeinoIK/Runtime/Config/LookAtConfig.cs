using Sirenix.OdinInspector;
using UnityEngine;

namespace SeinoIK.Runtime.Config
{
    [CreateAssetMenu(fileName = "New LookAt Config", menuName = "Seino IK/Create LookAt Config")]
    public class LookAtConfig : IKConfig
    {
        [LabelText("位置偏移")]
        public Vector3 offset;
        
        [Title("轴向设置")]
        [LabelText("头轴向")]
        public Vector3 headAxis = Vector3.forward;
        [LabelText("眼睛轴向")]
        public Vector3 eyesAxis = Vector3.forward;
        [LabelText("身体轴向")]
        public Vector3 spinesAxis = Vector3.forward;
        [LabelText("轴偏移")]
        public Vector3 pivotOffset = Vector3.up;
        
        [Title("参数设置")]
        [LabelText("最大距离")]
        public float maxDistance = 10000f;
        [LabelText("最小距离")]
        public float minDistance = 0.1f;
        [LabelText("看向转速"), Range(0, 360)]
        public float lookAtSpeed = 3f;
        [LabelText("离开转速"), Range(0, 360)]
        public float lookAwaySpeed;
        [LabelText("发现角度")]
        public Vector2 detectAngleXZ = new(170, 90);
        [LabelText("跟随角度")]
        public Vector2 followAngleXZ = new(180, 90);
        
        [Title("插值设置")]
        [LabelText("平滑时间")]
        public float smoothWeightTime = 0.5f;
        [LabelText("切换时间")]
        public float switchWeightTime = 0.5f;
        [LabelText("最大旋转增量")]
        public float maxRadiansDelta = 3f;
        [LabelText("最大位移增量")]
        public float maxMagnitudeDelta = 3f;
    }
}