// 闪烁管理器：管理所有需要闪烁的可破坏墙壁/地板
// 定时切换颜色以实现闪烁效果

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BreakableHiddenWallFlicker;

/// <summary>
/// 闪烁管理器 MonoBehaviour，挂在常驻 GameObject 上
/// </summary>
public class FlickerManager : MonoBehaviour
{
    /// <summary>
    /// 单个墙壁对象的渲染数据
    /// </summary>
    private class WallData
    {
        public Component RendererComponent;
        public PropertyInfo ColorProperty;
        public Color OriginalColor;
    }

    /// <summary>
    /// 实例引用
    /// </summary>
    public static FlickerManager Instance;

    // 存储需要闪烁的所有墙壁对象
    private readonly List<WallData> wallToFlicker = new List<WallData>();

    // 闪烁目标颜色
    private Color FlickerColor;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 解析闪烁颜色（HTML 格式字符串 → Unity Color）
        if (!ColorUtility.TryParseHtmlString(BreakableHiddenWallFlicker.Settings.FlickerColor, out FlickerColor))
        {
            FlickerColor = Color.gray;
        }

        // 启动闪烁协程
        StartCoroutine(Flicker());
    }

    /// <summary>
    /// 注册一个可破坏墙壁/地板对象
    /// </summary>
    public void RegisterObject(GameObject obj)
    {
        string name = obj.name;

        // 根据对象名称类型分别处理子网格
        if (name.StartsWith("Break Floor") || name.StartsWith("Breakable Floor") || name.StartsWith("One Way Wall"))
        {
            foreach (Transform child in obj.transform)
            {
                if (child.name.StartsWith("floor"))
                    TryRegisterRenderer(child.gameObject);
            }
            return;
        }

        if (name.StartsWith("Quake Floor"))
        {
            foreach (Transform child in obj.transform)
            {
                if (child.name.StartsWith("Active"))
                {
                    GameObject spriteObj = child.Find("Sprite")?.gameObject;
                    if (spriteObj != null)
                        TryRegisterRenderer(spriteObj);
                }
            }
            return;
        }

        // 其他类型（Break Wall / Breakable Wall）直接注册
        TryRegisterRenderer(obj);
    }

    /// <summary>
    /// 尝试为墙壁对象注册渲染器
    /// 支持 tk2dSprite 和 SpriteRenderer 两种组件
    /// </summary>
    public void TryRegisterRenderer(GameObject wallObject)
    {
        Component renderer = null;
        string rendererType = "";

        // 尝试获取 tk2dSprite（空洞骑士使用的 2D 精灵渲染器）
        renderer = wallObject.GetComponent<tk2dSprite>();
        if (renderer != null)
        {
            rendererType = "tk2dSprite";
        }
        else
        {
            // 尝试获取普通 SpriteRenderer
            renderer = wallObject.GetComponent<SpriteRenderer>();
            if (renderer != null)
                rendererType = "SpriteRenderer";
        }

        // 如果找不到渲染器则跳过
        if (renderer == null)
            return;

        // 检查是否已注册，避免重复
        foreach (WallData existing in wallToFlicker)
        {
            if (existing.RendererComponent == renderer)
                return;
        }

        // 通过反射获取 color 属性并保存原始颜色
        PropertyInfo colorProperty = renderer.GetType().GetProperty("color");
        if (colorProperty != null)
        {
            Color originalColor = (Color)colorProperty.GetValue(renderer, null);
            wallToFlicker.Add(new WallData
            {
                RendererComponent = renderer,
                ColorProperty = colorProperty,
                OriginalColor = originalColor
            });
        }
    }

    /// <summary>
    /// 闪烁协程：在主线程中持续切换墙壁颜色
    /// </summary>
    private IEnumerator Flicker()
    {
        while (true)
        {
            // 清理已销毁的渲染器引用
            wallToFlicker.RemoveAll((WallData wall) => wall.RendererComponent == null);

            if (BreakableHiddenWallFlicker.Settings.EnableFlickering)
            {
                // 切换到闪烁颜色
                SetAllColors(FlickerColor);
                yield return new WaitForSeconds(0.2f);
                // 恢复原始颜色
                SetAllOriginalColors();

                // 根据设置调整间隔
                if (BreakableHiddenWallFlicker.Settings.FasterFlicker)
                {
                    yield return new WaitForSeconds(0.4f);
                }
                else
                {
                    yield return new WaitForSeconds(0.8f);
                }
            }
            else
            {
                // 未启用闪烁时，持续显示闪烁颜色（用于视觉提示）
                SetAllColors(FlickerColor);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    /// <summary>
    /// 设置单个墙壁的颜色
    /// </summary>
    private void SetColorOnWall(WallData wall, Color color)
    {
        wall.ColorProperty.SetValue(wall.RendererComponent, color, null);
    }

    /// <summary>
    /// 将所有已注册的墙壁设置为指定颜色
    /// </summary>
    private void SetAllColors(Color color)
    {
        foreach (WallData wall in wallToFlicker)
        {
            if (wall.RendererComponent != null)
                SetColorOnWall(wall, color);
        }
    }

    /// <summary>
    /// 将所有已注册的墙壁恢复为原始颜色
    /// </summary>
    private void SetAllOriginalColors()
    {
        foreach (WallData wall in wallToFlicker)
        {
            if (wall.RendererComponent != null)
                SetColorOnWall(wall, wall.OriginalColor);
        }
    }

    /// <summary>
    /// 组件销毁时恢复所有墙壁的原始颜色
    /// </summary>
    private void OnDestroy()
    {
        SetAllOriginalColors();
    }
}