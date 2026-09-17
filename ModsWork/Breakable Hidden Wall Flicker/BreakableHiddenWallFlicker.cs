// 隐藏墙壁可视化模组
// 用于让可破坏的墙壁/地板闪烁，使其更容易被发现
// 需要汉化：所有菜单文本、日志输出

using System;
using System.Collections.Generic;
using Modding;
using On;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
// 注意：Assembly-CSharp 中存在全局命名空间的 SceneManager 类型（游戏自身场景管理类），
// 会遮蔽 UnityEngine.SceneManagement.SceneManager，因此用别名显式引用后者
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using UnityScene = UnityEngine.SceneManagement.Scene;
using UnityLoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;
using static UnityEngine.Object;

namespace BreakableHiddenWallFlicker;

public class BreakableHiddenWallFlicker : Mod, IMenuMod, IGlobalSettings<GlobalSettings>
{
    /// <summary>
    /// 模组单例
    /// </summary>
    internal static BreakableHiddenWallFlicker Instance;

    /// <summary>
    /// 模组全局设置
    /// </summary>
    internal static GlobalSettings Settings { get; private set; } = new GlobalSettings();

    // IMenuMod 接口实现：开关按钮不在菜单内（由模组列表统一控制）
    public bool ToggleButtonInsideMenu => false;

    /// <summary>
    /// 获取模组版本号
    /// </summary>
    public override string GetVersion()
    {
        return "1.2.1";
    }

    // IGlobalSettings 接口实现：加载全局设置
    public void OnLoadGlobal(GlobalSettings s)
    {
        Settings = s;
    }

    // IGlobalSettings 接口实现：保存全局设置
    public GlobalSettings OnSaveGlobal()
    {
        return Settings;
    }

    /// <summary>
    /// 模组初始化方法，在模组加载时调用
    /// </summary>
    public override void Initialize()
    {
        Instance = this;
        Log("正在初始化");

        // 创建闪烁管理器游戏对象，跨场景持久化
        GameObject val = new GameObject("FlickerManager");
        val.AddComponent<FlickerManager>();
        DontDestroyOnLoad(val);

        // 注册 PlayMakerFSM 启动钩子，监听可破坏墙壁的 FSM 状态变化
        // 使用 On. 命名空间的 MonoMod 钩子方式
        On.PlayMakerFSM.Start += OnFsmStart;

        // 注册场景加载钩子，用于禁用墙面遮罩
        UnitySceneManager.sceneLoaded += OnSceneLoaded;

        Log("已初始化");
    }

    /// <summary>
    /// 获取模组菜单数据
    /// 返回所有可配置的菜单项，包括"模组总开关"（由框架自动添加）
    /// </summary>
    public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
    {
        List<IMenuMod.MenuEntry> list = new List<IMenuMod.MenuEntry>();

        // 菜单项：启用闪烁
        list.Add(new IMenuMod.MenuEntry(
            name: "启用闪烁:",
            values: new string[] { "关闭", "开启" },
            description: "让可破坏的墙壁/地板闪烁，使其更容易被发现。关闭仅改变颜色。",
            saver: delegate(int i) { Settings.EnableFlickering = i == 1; },
            loader: () => Settings.EnableFlickering ? 1 : 0
        ));

        // 菜单项：加速闪烁
        list.Add(new IMenuMod.MenuEntry(
            name: "加速闪烁:",
            values: new string[] { "关闭", "开启" },
            description: "让闪烁动画速度更快。",
            saver: delegate(int i) { Settings.FasterFlicker = i == 1; },
            loader: () => Settings.FasterFlicker ? 1 : 0
        ));

        // 菜单项：禁用墙面遮罩
        list.Add(new IMenuMod.MenuEntry(
            name: "禁用墙面遮罩:",
            values: new string[] { "关闭", "开启" },
            description: "移除隐藏机密区域的黑色遮罩。",
            saver: delegate(int i) { Settings.DisableMasks = i == 1; },
            loader: () => Settings.DisableMasks ? 1 : 0
        ));

        // 菜单项：启用墙面粒子效果
        list.Add(new IMenuMod.MenuEntry(
            name: "启用墙面粒子:",
            values: new string[] { "关闭", "开启" },
            description: "为可破坏的墙壁/地板添加发光粒子效果。",
            saver: delegate(int i) { Settings.EnableWallGlowParticle = i == 1; },
            loader: () => Settings.EnableWallGlowParticle ? 1 : 0
        ));

        // 菜单项：粒子流向玩家
        list.Add(new IMenuMod.MenuEntry(
            name: "粒子流向玩家:",
            values: new string[] { "关闭", "开启" },
            description: "玩家靠近时，粒子会被吸引向玩家方向移动。",
            saver: delegate(int i) { Settings.EnableFlowToPlayer = i == 1; },
            loader: () => Settings.EnableFlowToPlayer ? 1 : 0
        ));

        return list;
    }

    /// <summary>
    /// PlayMakerFSM 启动时的钩子方法
    /// 当 FSM 关联到可破坏墙壁/地板时，注册到闪烁管理器
    /// </summary>
    private void OnFsmStart(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig.Invoke(self);

        GameObject go = self.gameObject;
        string name = go.name;

        // 检测是否是各类可破坏墙壁/地板对象
        if (name.StartsWith("Break Wall")
            || name.StartsWith("Breakable Wall")
            || name.StartsWith("Break Floor")
            || name.StartsWith("Breakable Floor")
            || name.StartsWith("One Way Wall")
            || name.StartsWith("Quake Floor"))
        {
            // 注册到闪烁管理器
            FlickerManager.Instance?.RegisterObject(go);

            // 如果还没有粒子效果组件，则添加
            if (go.GetComponent<WallParticleEffect>() == null)
            {
                go.AddComponent<WallParticleEffect>();
            }
        }
    }

    // 构造函数：设置模组中文显示名（方案 B）
    public BreakableHiddenWallFlicker()
        : base("隐藏墙壁可视化")
    {
    }

    /// <summary>
    /// 场景加载完成时的钩子方法
    /// 当开启"禁用墙面遮罩"时，隐藏当前场景中名称包含 "_mask" 的黑色遮罩物体
    /// </summary>
    private void OnSceneLoaded(UnityScene scene, UnityLoadSceneMode mode)
    {
        // 未开启禁用遮罩时不做处理
        if (!Settings.DisableMasks)
        {
            return;
        }

        // 遍历所有已加载的游戏对象，仅处理属于当前场景且名称含 "_mask" 的遮罩
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        for (int i = 0; i < allObjects.Length; i++)
        {
            GameObject go = allObjects[i];
            if (go.scene.name == scene.name && go.name.Contains("_mask"))
            {
                go.SetActive(false);
            }
        }
    }
}