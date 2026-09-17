// 墙面粒子效果组件
// 为可破坏墙壁/地板添加发光粒子效果

using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace BreakableHiddenWallFlicker;

/// <summary>
/// 挂在可破坏墙壁/地板上的粒子效果组件
/// </summary>
public class WallParticleEffect : MonoBehaviour
{
    // 发光材质
    private static Material glowMaterial;

    // 粒子系统
    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;

    // 玩家位置引用
    private Transform playerTransform;

    // 碰撞体引用
    private BoxCollider2D wallCollider;

    // 粒子特效的 GameObject
    private GameObject glowObject;

    private void Start()
    {
        // 如果未启用粒子效果则销毁此组件
        if (!BreakableHiddenWallFlicker.Settings.EnableWallGlowParticle)
        {
            Destroy(this);
            return;
        }

        // 延迟初始化材质（首次使用时创建）
        if (glowMaterial == null)
            CreateGlowMaterial();

        // 获取玩家位置
        HeroController instance = HeroController.instance;
        playerTransform = (instance != null) ? instance.transform : null;

        // 获取碰撞体用于粒子发射形状
        wallCollider = GetComponentInChildren<BoxCollider2D>();
        if (wallCollider == null)
        {
            Destroy(this);
            return;
        }

        // 创建粒子特效 GameObject
        glowObject = new GameObject("BreakableWall_ParticleEffect");
        glowObject.transform.SetParent(transform);
        glowObject.transform.localPosition = Vector3.zero;
        glowObject.transform.localScale = Vector3.one;

        // 添加粒子系统组件
        ps = glowObject.AddComponent<ParticleSystem>();
        ParticleSystemRenderer psr = ps.GetComponent<ParticleSystemRenderer>();
        ((Renderer)psr).material = glowMaterial;
        ((Renderer)psr).sortingOrder = 10;

        // 预分配粒子数组以避免频繁内存分配
        ParticleSystem.MainModule main = ps.main;
        particles = new ParticleSystem.Particle[main.maxParticles];

        // 配置闪烁效果
        SetupShimmerEffect();
    }

    private void Update()
    {
        // 如果碰撞体被禁用（墙壁已被打破），销毁粒子系统
        if (wallCollider == null || !wallCollider.enabled)
        {
            if (glowObject != null)
                Destroy(glowObject);
            Destroy(this);
            return;
        }

        // 如果启用了粒子流向玩家功能
        if (!BreakableHiddenWallFlicker.Settings.EnableFlowToPlayer
            || playerTransform == null
            || ps == null)
            return;

        // 计算与玩家的距离
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance > BreakableHiddenWallFlicker.Settings.FlowDrawDistance)
            return;

        // 获取当前所有粒子
        int count = ps.GetParticles(particles);
        for (int i = 0; i < count; i++)
        {
            ParticleSystem.Particle p = particles[i];

            // 已到达玩家附近的粒子重新生成
            if (Vector3.Distance(playerTransform.position, p.position) < 0.3f)
            {
                p.remainingLifetime = 0f;
                continue;
            }

            // 添加向玩家方向的力
            Vector3 dir = (playerTransform.position - p.position).normalized;
            p.velocity += dir * 6f * Time.deltaTime;
            particles[i] = p;
        }

        // 更新粒子系统
        ps.SetParticles(particles, count);
    }

    /// <summary>
    /// 配置粒子系统的闪烁效果参数
    /// </summary>
    private void SetupShimmerEffect()
    {
        ParticleSystem.MainModule main = ps.main;
        main.loop = true;
        main.startSpeed = 0f;
        main.startLifetime = new MinMaxCurve(1.5f, 2f);
        main.startSize = new MinMaxCurve(0.15f, 0.25f);
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.scalingMode = ParticleSystemScalingMode.Local;

        // 发射速率：每秒 25 个粒子
        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = 25f;

        // 形状：沿碰撞体大小发射
        ParticleSystem.ShapeModule shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        if (wallCollider != null)
        {
            shape.scale = wallCollider.size;
        }

        // 应用共享设置（颜色等）
        ApplySharedSettings();
    }

    /// <summary>
    /// 应用粒子的颜色渐变设置
    /// </summary>
    private void ApplySharedSettings()
    {
        ParticleSystem.MainModule main = ps.main;
        Color particleColor;
        if (!ColorUtility.TryParseHtmlString(BreakableHiddenWallFlicker.Settings.ParticleColor, out particleColor))
        {
            particleColor = Color.white;
        }
        main.startColor = particleColor;

        // 添加生命周期渐变：淡入淡出效果
        ParticleSystem.ColorOverLifetimeModule col = ps.colorOverLifetime;
        col.enabled = true;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(0.8f, 0.2f),
                new GradientAlphaKey(0.8f, 0.8f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        col.color = gradient;
    }

    /// <summary>
    /// 动态创建发光材质（径向渐变纹理）
    /// </summary>
    private void CreateGlowMaterial()
    {
        glowMaterial = new Material(Shader.Find("Sprites/Default"));

        // 创建 64x64 的径向渐变纹理
        int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);

        float center = (float)size / 2f;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float dist = Vector2.Distance(new Vector2((float)j, (float)i), new Vector2(center, center));
                float alpha = Mathf.Clamp01(1f - dist / center);
                texture.SetPixel(j, i, new Color(1f, 1f, 1f, alpha));
            }
        }
        texture.Apply();
        glowMaterial.mainTexture = texture;
    }
}