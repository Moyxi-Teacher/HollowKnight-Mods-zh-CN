using System;
using System.Collections.Generic;
using System.Reflection;
using Modding;
using UnityEngine;

namespace BenchWarpFix;

/// <summary>
/// 长椅传送修正（BenchWarpFix）
///
/// 除原有的"掉出地图边界时自动传回德特茅斯"功能外，
/// 追加了一项针对长椅状态机的兼容修复（见下方说明）。
/// </summary>
public class BenchWarpFix : Mod, IMenuMod, ITogglableMod
{
    // ─────────── 追加的兼容修复所涉及的常量 ───────────

    private const string BenchFsmName = "Bench Control";
    private const string CanRestState = "Can Rest?";
    private const string HeroControllerName = "HeroController";
    private const string HeroRefName = "Hero";

    private const BindingFlags AllInstance =
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private bool _loggedRepair;

    public BenchWarpFix() : base("长椅传送修正") { }

    internal static BenchWarpFix Instance;

    public override string GetVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }

    public override void Initialize()
    {
        Instance = this;

        On.HeroController.FixedUpdate += this.OnHeroControllerFixedUpdate;
        On.HutongGames.PlayMaker.FsmState.OnEnter += this.OnBenchFsmStateEnter;
    }

    public bool ToggleButtonInsideMenu => true;

    public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
    {
        return new List<IMenuMod.MenuEntry>
        {
            new(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values, "当英雄掉出地图边界时，自动传送回德特茅斯以防止卡在外层空间。", toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader)
        };
    }

    public void Unload()
    {
        On.HeroController.FixedUpdate -= this.OnHeroControllerFixedUpdate;
        On.HutongGames.PlayMaker.FsmState.OnEnter -= this.OnBenchFsmStateEnter;
    }

    private void OnHeroControllerFixedUpdate(On.HeroController.orig_FixedUpdate orig, HeroController self)
    {
        orig(self);

        var heroPos = self.transform.position;

        if (heroPos.x >= -10000.0f && heroPos.y >= -10000.0f)
        {
            return;
        }

        var failSafe = new GameObject("FAILSAFE");
        failSafe.transform.SetPosition2D(heroPos);
        var tp = failSafe.AddComponent<TransitionPoint>();

        var bc = failSafe.AddComponent<BoxCollider2D>();
        bc.size = new Vector2(10f, 10f);
        bc.isTrigger = true;
        tp.SetTargetScene("Town");
        tp.entryPoint = "top1";

        var rm = new GameObject("Hazard Respawn Marker");
        rm.transform.parent = failSafe.transform;
        rm.transform.SetPosition2D(new Vector2(0f, 0f));
        var tmp = rm.AddComponent<HazardRespawnMarker>();
        tmp.respawnFacingRight = false;
        tp.respawnMarker = rm.GetComponent<HazardRespawnMarker>();
        tp.sceneLoadVisualization = GameManager.SceneLoadVisualizations.Default;
    }

    // ═══════════════════════════════════════════════════════════════════
    //  长椅状态机兼容修复
    //
    //  背景：长椅状态机 "Bench Control" 的「Can Rest?」状态里，
    //        判断玩家此刻能否休息需要重新采样英雄状态；而其中的
    //        CallMethodProper 动作在取不到英雄对象时是**静默返回**的
    //        （既不改变量也不报错），于是按键会被取消、坐下动作也不会播。
    //
    //  做法：在状态动作**执行之前**（PlayMaker 的执行顺序是先 FsmState.OnEnter、
    //        随后才逐个执行动作），把该状态里目标确定为英雄的引用重新指向当前英雄，
    //        并把「Can Rest?」依赖的判定变量同步为玩家实时状态。
    //
    //  安全性：只写状态机里的引用字段与判定变量，
    //          不修改 PlayerData、不剥夺玩家控制权、不切换状态机状态。
    // ═══════════════════════════════════════════════════════════════════

    private void OnBenchFsmStateEnter(On.HutongGames.PlayMaker.FsmState.orig_OnEnter orig,
                                      HutongGames.PlayMaker.FsmState self)
    {
        try
        {
            if (self != null && self.Fsm != null && self.Fsm.Name == BenchFsmName)
            {
                var hc = HeroController.instance;
                if (hc != null)
                {
                    var hero = hc.gameObject;
                    if (hero != null)
                    {
                        RepairStateRefs(self, hero);
                        if (self.Name == CanRestState)
                        {
                            SyncCanRestVariables(self.Fsm, hc);
                        }
                    }
                }
            }
        }
        catch
        {
            // 任何异常都不应影响游戏本身的流程
        }

        orig(self);
    }

    /// <summary>把该状态内"目标确定为英雄"的引用重新指向当前英雄对象。</summary>
    private void RepairStateRefs(HutongGames.PlayMaker.FsmState state, GameObject hero)
    {
        var acts = state.Actions;
        if (acts == null)
        {
            return;
        }

        foreach (var act in acts)
        {
            if (act == null)
            {
                continue;
            }

            var type = act.GetType();
            var behaviour = GetBehaviourName(act, type);

            foreach (var field in type.GetFields(AllInstance))
            {
                if (field.FieldType != typeof(HutongGames.PlayMaker.FsmOwnerDefault))
                {
                    continue;
                }

                var od = field.GetValue(act) as HutongGames.PlayMaker.FsmOwnerDefault;
                if (od == null || od.GameObject == null)
                {
                    continue;
                }

                if (od.OwnerOption != HutongGames.PlayMaker.OwnerDefaultOption.SpecifyGameObject)
                {
                    continue;
                }

                string refName;
                try
                {
                    refName = od.GameObject.Name ?? string.Empty;
                }
                catch
                {
                    refName = string.Empty;
                }

                // 只在能百分之百确定目标时动手：引用名为 "Hero"，或动作明确要调用 HeroController
                if (refName != HeroRefName && behaviour != HeroControllerName)
                {
                    continue;
                }

                if (od.GameObject.Value == hero)
                {
                    continue;
                }

                AssignTarget(od, state.Fsm, hero);
                ClearMethodCache(act, type);

                if (!_loggedRepair)
                {
                    _loggedRepair = true;
                    Log($"[长椅传送修正] 已修正「{state.Name}」中指向英雄的对象引用" +
                        $"（{type.Name}，引用名 \"{refName}\"）");
                }
            }
        }
    }

    /// <summary>
    /// 把引用指向指定对象。
    /// 动作运行时是通过 Fsm.GetOwnerDefaultTarget() 取目标的，而它直接读
    /// FsmOwnerDefault.GameObject.Value 字段，因此必须写这个字段本身；
    /// 若该引用同时是"引用 FSM 变量"的写法，也把它指向的变量一并写上。
    /// </summary>
    private static void AssignTarget(HutongGames.PlayMaker.FsmOwnerDefault od,
                                     HutongGames.PlayMaker.Fsm fsm, GameObject target)
    {
        try
        {
            var fgo = od.GameObject;

            fgo.Value = target;

            if (fgo.UseVariable && fsm != null && fsm.Variables != null)
            {
                var v = fsm.Variables.GetFsmGameObject(fgo.Name);
                if (v != null && v != fgo)
                {
                    v.Value = target;
                }
            }
        }
        catch
        {
        }
    }

    /// <summary>取 CallMethodProper 的 behaviour（要取组件的名字）。</summary>
    private static string GetBehaviourName(object action, Type type)
    {
        try
        {
            var field = type.GetField("behaviour", AllInstance);
            if (field == null)
            {
                return string.Empty;
            }

            var value = field.GetValue(action);
            if (value == null)
            {
                return string.Empty;
            }

            var prop = value.GetType().GetProperty("Value");
            return prop != null ? (prop.GetValue(value) as string ?? string.Empty) : string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>清掉动作内部缓存的方法信息，避免引用修正后仍沿用旧的失败缓存。</summary>
    private static void ClearMethodCache(object action, Type type)
    {
        foreach (var name in new[] { "cachedBehaviour", "cachedType", "cachedMethodInfo", "cachedParameterInfo", "errorString" })
        {
            try
            {
                var field = type.GetField(name, AllInstance);
                if (field != null)
                {
                    field.SetValue(action, null);
                }
            }
            catch
            {
            }
        }
    }

    /// <summary>把「Can Rest?」判断所依赖的 7 个变量同步为玩家实时状态。</summary>
    private static void SyncCanRestVariables(HutongGames.PlayMaker.Fsm fsm, HeroController hc)
    {
        try
        {
            var vars = fsm.Variables;
            if (vars == null || hc == null || hc.cState == null)
            {
                return;
            }

            bool canInput;
            try
            {
                canInput = hc.CanInput();
            }
            catch
            {
                return;
            }

            SetBool(vars, "Can Input", canInput);
            SetBool(vars, "On Ground", hc.cState.onGround);
            SetBool(vars, "Attacking", hc.cState.attacking);
            SetBool(vars, "Attacking Up", hc.cState.upAttacking);
            SetBool(vars, "Attacking Down", hc.cState.downAttacking);
            SetBool(vars, "Dashing", hc.cState.dashing);
            SetBool(vars, "Backdashing", hc.cState.backDashing);
        }
        catch
        {
        }
    }

    private static void SetBool(HutongGames.PlayMaker.FsmVariables vars, string name, bool value)
    {
        try
        {
            var b = vars.GetFsmBool(name);
            if (b != null && b.Value != value)
            {
                b.Value = value;
            }
        }
        catch
        {
        }
    }
}
