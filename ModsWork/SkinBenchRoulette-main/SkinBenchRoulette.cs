using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomKnight;
using Modding;
using Satchel.BetterMenus;
using UnityEngine;

namespace SkinBenchRoulette
{
    /// <summary>
    /// 全局设置（随 <c>SkinBenchRoulette.GlobalSettings.json</c> 存档）
    /// </summary>
    public class GlobalSettings
    {
        /// <summary>
        /// 「最近五个皮肤不重复」开关：开启后，随机换肤不会抽到最近已换过的五个皮肤。
        /// </summary>
        public bool NoRepeatRecent5 { get; set; } = true;

        /// <summary>
        /// 死亡后复活时是否自动随机更换一次皮肤。
        /// </summary>
        public bool RespawnSwapOnDeath { get; set; } = true;

        /// <summary>
        /// 最近使用过的皮肤 id 列表（最新在前），用于实现「最近五个不重复」。
        /// </summary>
        public List<string> RecentSkins { get; set; } = new List<string>();
    }

    /// <summary>
    /// 皮肤长椅轮换（SkinBenchRoulette）主类
    /// <para>功能：坐在长椅上自动随机更换自定义小骑士皮肤；死亡复活后自动随机更换皮肤；</para>
    /// <para>可选的「最近五个皮肤不重复」；通过自定义二级菜单开关（模组总开关在菜单内）。</para>
    /// </summary>
    public class SkinBenchRoulette : Mod, IGlobalSettings<GlobalSettings>, ICustomMenuMod, ITogglableMod
    {
        /// <summary>
        /// 当前模组实例
        /// </summary>
        public static SkinBenchRoulette Instance { get; private set; }

        /// <summary>
        /// 全局设置实例
        /// </summary>
        public static GlobalSettings GlobalSettings { get; set; } = new GlobalSettings();

        /// <summary>
        /// 「最近五个皮肤不重复」最多记住的皮肤个数
        /// </summary>
        private const int MaxRecentSkins = 5;

        /// <summary>
        /// 是否正在更换皮肤（换肤过程中屏蔽新的触发，避免连发）
        /// </summary>
        private bool estaTrocando = false;

        /// <summary>
        /// 本次坐椅期间是否已经换过一次皮肤（离开坐姿后复位，用于防抖）
        /// </summary>
        private bool jaTrocouNoBanco = false;

        /// <summary>
        /// 死亡标记：检测到死亡（cState.dead 为真）后置位，等待复活完成时消费
        /// </summary>
        private bool jaMorreu = false;

        /// <summary>
        /// 二级菜单引用
        /// </summary>
        private static Menu MenuRef;

        /// <summary>
        /// 总开关是否位于模组二级菜单内部（由本模组自绘，ModLoader 不再在列表里加外部开关）
        /// </summary>
        public bool ToggleButtonInsideMenu { get; } = true;

        /// <summary>
        /// 构造函数（模组名称使用程序集名，存档/设置文件名保持一致）
        /// </summary>
        public SkinBenchRoulette() : base("皮肤长椅轮换")
        {
        }

        /// <summary>
        /// Mods 一级菜单按钮文字（中文模组名，不带「选项」二字）
        /// </summary>
        public override string GetMenuButtonText() => "皮肤长椅轮换";

        /// <summary>
        /// 模组版本号（一级菜单悬停时显示为 v1.2.0）
        /// </summary>
        public override string GetVersion() => "1.2.0";

        /// <summary>
        /// 读取全局设置时调用
        /// </summary>
        public void OnLoadGlobal(GlobalSettings s)
        {
            if (s != null)
            {
                GlobalSettings = s;
            }
        }

        /// <summary>
        /// 保存全局设置时调用
        /// </summary>
        public GlobalSettings OnSaveGlobal()
        {
            return GlobalSettings;
        }

        /// <summary>
        /// 模组初始化：注册每帧检测钩子
        /// </summary>
        public override void Initialize()
        {
            Instance = this;
            ModHooks.HeroUpdateHook += OnHeroUpdate;
            Log("皮肤长椅轮换已启动：坐在长椅上或死亡复活后会自动随机更换皮肤");
        }

        /// <summary>
        /// 模组被关闭/卸载时调用：解除钩子，恢复初始状态
        /// </summary>
        public void Unload()
        {
            ModHooks.HeroUpdateHook -= OnHeroUpdate;
            estaTrocando = false;
            jaTrocouNoBanco = false;
            jaMorreu = false;
            Log("皮肤长椅轮换已卸载");
        }

        // ===================== 菜单 =====================

        /// <summary>
        /// 由 ModLoader 调用，取得本模组的二级菜单画面
        /// </summary>
        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            if (MenuRef == null)
            {
                MenuRef = PrepareMenu(toggleDelegates);
            }
            MenuRef.returnScreen = modListMenu;
            return MenuRef.GetMenuScreen(modListMenu);
        }

        /// <summary>
        /// 构建二级菜单：总开关 + 功能开关 + 手动换肤按钮
        /// </summary>
        private static Menu PrepareMenu(ModToggleDelegates? toggleDelegates)
        {
            var elements = new List<Element>();

            // 模组总开关：悬停说明为该模组 GitHub 仓库描述的中文简介（不超过 25 字、单行、非说明性开头）
            if (toggleDelegates.HasValue)
            {
                elements.Add(Blueprints.CreateToggle(
                    toggleDelegates.Value,
                    "皮肤长椅轮换",
                    "每次坐在长椅上随机更换自定义小骑士皮肤",
                    "开启",
                    "关闭"));
            }

            // 「最近五个皮肤不重复」开关
            elements.Add(Blueprints.HorizontalBoolOption(
                "最近五个皮肤不重复",
                "已换过的最近五个皮肤不会被再次随机到",
                v => { GlobalSettings.NoRepeatRecent5 = v; },
                () => GlobalSettings.NoRepeatRecent5));

            // 死亡后复活时是否换肤开关
            elements.Add(Blueprints.HorizontalBoolOption(
                "死亡后复活换肤",
                "英雄死亡并复活后，自动随机更换一次皮肤",
                v => { GlobalSettings.RespawnSwapOnDeath = v; },
                () => GlobalSettings.RespawnSwapOnDeath));

            // 手动立即换肤（便于随时测试，不影响原有自动功能）
            elements.Add(new MenuButton("立即更换皮肤", "不坐长椅也立即随机更换一次皮肤", _ => OnManualSwap()));

            return new Menu("皮肤长椅轮换", elements.ToArray());
        }

        /// <summary>
        /// 手动换肤（在 Mods 二级菜单中点击触发）
        /// </summary>
        private static void OnManualSwap()
        {
            var instance = Instance;
            if (instance != null)
            {
                instance.IniciarTrocaComDelay(0.5f, "手动");
            }
        }

        // ===================== 换肤核心 =====================

        /// <summary>
        /// 每帧检测：坐长椅 / 死亡复活 两种时机触发随机换肤
        /// </summary>
        private void OnHeroUpdate()
        {
            if (HeroController.instance == null)
            {
                return;
            }

            if (estaTrocando)
            {
                return;
            }

            var anim = HeroController.instance.GetComponent<tk2dSpriteAnimator>();
            bool estaSentado = anim != null
                               && anim.CurrentClip != null
                               && anim.CurrentClip.name == "Sit";

            bool morto = HeroController.instance.cState != null && HeroController.instance.cState.dead;

            if (morto)
            {
                if (GlobalSettings.RespawnSwapOnDeath)
                {
                    // 记录死亡，等待复活完成后再换肤
                    jaMorreu = true;
                }
                else
                {
                    jaMorreu = false;
                }
                jaTrocouNoBanco = false;
                return;
            }

            // 死亡复活完成（死亡后再次存活）→ 换一次皮肤
            if (jaMorreu)
            {
                jaMorreu = false;
                // 若复活时正处于长椅坐姿，则本次复活已计入该段坐姿，避免起身动画再触发一次
                jaTrocouNoBanco = estaSentado;
                // 稍等片刻，等复活/重生演出结束后再换，避免遮挡
                IniciarTrocaComDelay(1.0f, "死亡复活");
                return;
            }

            // 坐长椅触发（沿用原版防抖逻辑：同一段坐姿只换一次）
            if (estaSentado)
            {
                if (!jaTrocouNoBanco)
                {
                    jaTrocouNoBanco = true;
                    IniciarTrocaComDelay(0.5f, "长椅");
                }
            }
            else
            {
                jaTrocouNoBanco = false;
            }
        }

        /// <summary>
        /// 启动带延迟的换肤协程（先加锁，防止并发触发）
        /// </summary>
        private void IniciarTrocaComDelay(float delay, string motivo)
        {
            if (estaTrocando)
            {
                return;
            }
            estaTrocando = true;
            HeroController.instance.StartCoroutine(TrocarComDelay(delay, motivo));
        }

        /// <summary>
        /// 延迟后执行一次随机换肤，结束后解锁
        /// </summary>
        private IEnumerator TrocarComDelay(float delay, string motivo)
        {
            yield return new WaitForSeconds(delay);

            try
            {
                var escolhida = EscolherSkinAleatoria();
                if (escolhida == null)
                {
                    LogWarn($"[{motivo}] 当前没有可用于更换的皮肤，跳过本次随机");
                }
                else
                {
                    string id = escolhida.GetId();
                    // 调用新版 CustomKnight 的公开静态接口直接换肤（修复旧版按 "Custom Knight" 硬编码目录不生效的问题）
                    SkinManager.SetSkinById(id);

                    // 记录最近使用，用于「最近五个皮肤不重复」
                    GlobalSettings.RecentSkins.RemoveAll(x => x == id);
                    GlobalSettings.RecentSkins.Insert(0, id);
                    while (GlobalSettings.RecentSkins.Count > MaxRecentSkins)
                    {
                        GlobalSettings.RecentSkins.RemoveAt(GlobalSettings.RecentSkins.Count - 1);
                    }

                    Log($"[{motivo}] 已随机更换皮肤为：{escolhida.GetName()}（{id}）");
                }
            }
            catch (Exception e)
            {
                LogError($"[{motivo}] 更换皮肤出错：" + e);
            }
            finally
            {
                estaTrocando = false;
            }
        }

        /// <summary>
        /// 从已安装皮肤中随机挑选一个：
        /// <para>总是剔除当前正在使用的皮肤；若开启「最近五个皮肤不重复」，再剔除最近记录；</para>
        /// <para>当剔除后无可选皮肤时，自动退化为仅剔除当前皮肤，避免永远无法换肤。</para>
        /// </summary>
        private ISelectableSkin EscolherSkinAleatoria()
        {
            // 自定义小骑士未启用时无法换肤（由类型加载保证 dll 存在，此处仅做实例检查并给出提示）
            if (CustomKnight.CustomKnight.Instance == null)
            {
                LogError("未检测到「自定义小骑士」正在运行，无法随机换肤");
                return null;
            }

            var todas = SkinManager.GetInstalledSkins();
            if (todas == null || todas.Length <= 1)
            {
                return null;
            }

            var atual = SkinManager.GetCurrentSkin();
            string idAtual = atual?.GetId();

            var pool = todas.Where(s => s.GetId() != idAtual).ToList();

            if (GlobalSettings.NoRepeatRecent5 && pool.Count > 1)
            {
                var antigos = GlobalSettings.RecentSkins;
                pool.RemoveAll(s => antigos.Contains(s.GetId()));
                // 候选不足时退化为只排除当前皮肤
                if (pool.Count == 0)
                {
                    pool = todas.Where(s => s.GetId() != idAtual).ToList();
                }
            }

            if (pool.Count == 0)
            {
                return null;
            }

            return pool[UnityEngine.Random.Range(0, pool.Count)];
        }
    }
}
