using Sirenix.OdinInspector;

namespace XenoIK.Runtime.Enum
{
    /// <summary>
    /// 射线检测类型
    /// </summary>
    public enum RayCastType
    {
        [LabelText("物理")]
        Phyics,
        [LabelText("导航网格")]
        Navmesh,
        [LabelText("混合")]
        Mix
    }

    /// <summary>
    /// 插值类型
    /// </summary>
    public enum LerpType
    {
        [LabelText("线性插值")]
        Linear,
        [LabelText("简单阻尼插值")]
        SampleDamper,
        [LabelText("阻尼插值")]
        Damper
    }
}