// Breakable Hidden Wall Flicker 全局设置类
// 用于保存模组的所有配置选项

namespace BreakableHiddenWallFlicker;

/// <summary>
/// 模组的全局设置，保存到 JSON 文件中
/// </summary>
public class GlobalSettings
{
    // 是否启用墙壁闪烁效果
    public bool EnableFlickering = true;

    // 闪烁颜色（HTML 格式字符串）
    public string FlickerColor = "#808080FF";

    // 是否加速闪烁动画
    public bool FasterFlicker = false;

    // 是否禁用墙面遮罩
    public bool DisableMasks = false;

    // 是否启用墙面发光粒子效果
    public bool EnableWallGlowParticle = false;

    // 粒子颜色（HTML 格式字符串）
    public string ParticleColor = "#FFFFFFFF";

    // 粒子是否流向玩家
    public bool EnableFlowToPlayer = false;

    // 粒子流向玩家的距离阈值
    public float FlowDrawDistance = 10f;
}