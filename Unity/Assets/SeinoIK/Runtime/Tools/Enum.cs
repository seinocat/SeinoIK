using Sirenix.OdinInspector;

namespace SeinoIK.Runtime.Enum
{
    /// <summary>
    /// 射线检测类型
    /// </summary>
    public enum RayCastType
    {
        [LabelText("Mesh网格")]
        Mesh,
        [LabelText("导航网格")]
        Navmesh,
        [LabelText("混合")]
        Mix
    }
}