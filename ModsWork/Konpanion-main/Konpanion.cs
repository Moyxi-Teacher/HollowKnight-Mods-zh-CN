using Modding;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static Satchel.EnemyUtils;
using static Satchel.GameObjectUtils;

namespace Konpanion
{
    public class Konpanion : Mod, IMenuMod, ITogglableMod
    {
        public Konpanion() : base("伴侣") { }

        internal static Konpanion Instance;

        internal static List<GameObject> knights = new List<GameObject>();
        internal static Dictionary<ushort,GameObject> remoteKnights = new Dictionary<ushort,GameObject>();

        // 游戏内菜单显示为开关按钮
        public bool ToggleButtonInsideMenu => true;

        // 仅返回一条总开关条目（说明文写进 Description），避免重复条目导致菜单渲染时 NullReferenceException。
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            List<IMenuMod.MenuEntry> entries = new List<IMenuMod.MenuEntry>();
            if (toggleButtonEntry != null)
            {
                IMenuMod.MenuEntry entry = toggleButtonEntry.Value;
                entry.Description = "跟随在小骑士身边的伙伴，在 HKMP 多人模式下会显示远程玩家的伴侣。";
                entries.Add(entry);
            }
            return entries;
        }

        public static bool HasPouch()
        {
            // 修复(2026-09-19)：HkmpPouch 汉化后显示名为「HkmpPouch（网络库）」，新旧名都识别。
            var hasPouch = ModHooks.GetMod("HkmpPouch") is Mod
                || ModHooks.GetMod("HkmpPouch（网络库）") is Mod;
            return hasPouch;
        }
        public override string GetVersion()
        {
            return "1.0";
        }

        public GameObject createKnightCompanion(GameObject ft = null){
            var heroCtrl = HeroController.instance;
            if (heroCtrl == null || heroCtrl.gameObject == null) return null;   // 主菜单等场景英雄未就绪
            var heroGo = heroCtrl.gameObject;

            heroGo.SetActive(false);
            GameObject knight;
            try
            {
                knight = heroGo.createCompanionFromPrefab();
            }
            finally
            {
                // 无论如何都要把英雄恢复 —— 克隆若中途抛异常（例如某个动作类型缺少
                // 无参构造），不恢复就会把英雄永久留在失活状态，移动/存档/长椅会一起废掉。
                heroGo.SetActive(true);
            }
            knight.RemoveComponent<HeroController>();
            knight.RemoveComponent<HeroAnimationController>();
            knight.RemoveComponent<HeroAudioController>();
            knight.RemoveComponent<ConveyorMovementHero>();

            knight.name = "kompanion";
            knight.GetAddComponent<MeshRenderer>().enabled = true;
            knight.GetAddComponent<Rigidbody2D>().gravityScale = 1f;

            //add control and adjust parameters
            var kc = knight.GetAddComponent<CompanionControl>();
            kc.moveSpeed = 10f;
            kc.followDistance = 2f;
            kc.IdleShuffleDistance = 0.01f;

            if(ft != null){
                kc.followTarget = ft;
            }

            //fix up collider size
            var collider = knight.GetAddComponent<BoxCollider2D>();
            collider.size = new Vector2(1f,2.0f);
            collider.offset = new Vector2(0f,-0.4f);
            collider.enabled = true;

            // add animations
            kc.Animations.Add(State.Idle,"Idle");
            kc.Animations.Add(State.IdleFidget1,"Acid Death");
            kc.Animations.Add(State.IdleFidget2,"Prostrate");
            kc.Animations.Add(State.Walk,"Run");
            kc.Animations.Add(State.Turn,"Map Walk");
            kc.Animations.Add(State.Teleport,"Fall");
            kc.Animations.Add(State.Jump,"Airborne");

            knight.SetActive(true);

            // 兼容修复：克隆体会干扰关卡场景对英雄的跨场景引用，这里立即纠正一次
            RepairBenchHeroRefs();

            return knight;
        }

        public override void Initialize()
        {
            Instance = this;
            ModHooks.HeroUpdateHook += update;
            On.GameManager.OnNextLevelReady += OnNextLevelReady;
            On.HutongGames.PlayMaker.Fsm.GetOwnerDefaultTarget += OnGetOwnerDefaultTarget;
            On.HutongGames.PlayMaker.FsmState.OnEnter += OnFsmStateEnter;
            if (Konpanion.HasPouch())
            {
                PouchIntegration.Initialize();
            }
        }

        // 关闭时移除钩子，实现真正的总开关
        public void Unload()
        {
            ModHooks.HeroUpdateHook -= update;
            On.GameManager.OnNextLevelReady -= OnNextLevelReady;
            On.HutongGames.PlayMaker.Fsm.GetOwnerDefaultTarget -= OnGetOwnerDefaultTarget;
            On.HutongGames.PlayMaker.FsmState.OnEnter -= OnFsmStateEnter;
            Instance = null;
        }


        public GameObject GetNetworkKonpanion(ushort id){
            if(remoteKnights.TryGetValue(id,out var knight)){
                return knight;
            }
            remoteKnights[id] = createKnightCompanion();
            return remoteKnights[id];
        }
        public void update()
        {
            // 伙伴对象可能被销毁（回主菜单、读档等），列表里会留着一个空引用，
            // 只看 Count 不会重建 —— 表现就是"伴侣从此不再出现"。这里一并处理。
            if (knights.Count < 1 || knights[0] == null)
            {
                knights.Clear();
                var rebuilt = createKnightCompanion();
                if (rebuilt != null) knights.Add(rebuilt);
            }
        }


        // ═══════════════════════════════════════════════════════════════════
        //  兼容修复：克隆小骑士会干扰"场景对英雄的跨场景引用"
        //
        //  背景：本模组用 Instantiate 克隆小骑士对象作为伙伴，并把克隆体放进
        //        DontDestroyOnLoad 场景（与英雄同处一个持久场景）。这会使关卡场景里
        //        指向英雄的跨场景引用在重新解析时落到克隆体上。
        //
        //        长椅状态机 "Bench Control" 正依赖这种引用 —— 其「Can Rest?」状态要从
        //        英雄身上取组件调用方法。引用一旦指向克隆体：
        //          · 取不到 HeroController 组件；
        //          · PlayMaker 的 CallMethodProper 在取不到目标对象时**直接 return**
        //            （既不报错也不写日志）；
        //          · 表现就是按上键无法休息、骑士也不坐下。
        //
        //  处置：创建伙伴之后、以及每次场景加载之后，把长椅状态机里"目标确定为英雄"
        //        的引用重新指向真正的英雄对象。只写状态机里的引用字段，
        //        不触碰 PlayerData、不剥夺控制权、不切换状态机状态。
        // ═══════════════════════════════════════════════════════════════════

        private const string BenchFsmName = "Bench Control";
        private const string HeroControllerName = "HeroController";
        private const string HeroRefName = "Hero";

        private const BindingFlags AllInstance =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private bool _loggedBenchFix;
        private int _fixedTotal;

        private void OnNextLevelReady(On.GameManager.orig_OnNextLevelReady orig, GameManager self)
        {
            orig(self);
            try { RepairBenchHeroRefs(); }
            catch { }
        }

        // ───────────────────────────────────────────────────────────────────
        //  第二道防线：在"取值的那一刻"确保英雄引用不会被克隆体顶替
        //
        //  上面 RepairBenchHeroRefs 是"时机型"修复（创建伙伴后、场景加载后各跑一次）；
        //  但克隆体也可能在场景加载之后才重建，变量表仍会被再次污染。
        //  所以在 PlayMaker 解析目标对象的入口再兜一层：
        //      只要动作的目标写法是"变量引用 + 变量名恰为 Hero"，
        //      而解析结果不是小骑士本体，就一律改回小骑士本体。
        //
        //  只认这一种写法（游戏约定"名为 Hero 的变量 = 小骑士"），
        //  其余任何引用——包括直接引用、其它变量名、UseOwner——原样返回，不做干预。
        //  PlayMaker 的 CallMethodProper 在取不到目标时是**静默 return** 的，
        //  所以这一层必须在解析阶段生效，而不是等它失败。
        // ───────────────────────────────────────────────────────────────────

        private int _hookFixedTotal;
        private int _hookCalls;
        private int _stateFixedCount;

        // ───────────────────────────────────────────────────────────────────
        //  最可靠的一道防线：进入「Can Rest?」的那一刻，在动作执行之前摆正引用
        //
        //  为什么放在这里：取值方法 Fsm.GetOwnerDefaultTarget 体量很小，很可能被 JIT
        //  内联，一旦内联 detour 就失效（实测钩子 0 次命中就有这个嫌疑）；
        //  而状态切换的钩子是普通大方法，稳定可用。
        //  必须写在 orig(self) **之前** —— 动作的 OnEnter 一执行就会立刻去读这个引用。
        //
        //  只处理"名字恰为 Hero 或 behaviour 为 HeroController"的引用，其余一律不碰；
        //  不碰 PlayerData、不碰控制权、不改状态机走向。
        // ───────────────────────────────────────────────────────────────────

        private const string CanRestStateName = "Can Rest?";

        private void OnFsmStateEnter(On.HutongGames.PlayMaker.FsmState.orig_OnEnter orig,
                                     HutongGames.PlayMaker.FsmState self)
        {
            try
            {
                if (self != null && self.Fsm != null
                    && self.Fsm.Name == BenchFsmName
                    && self.Name == CanRestStateName)
                {
                    FixHeroRefsInState(self);
                }
            }
            catch { }
            orig(self);
        }

        private void FixHeroRefsInState(HutongGames.PlayMaker.FsmState st)
        {
            var hc = HeroController.instance;
            if (hc == null) return;
            var hero = hc.gameObject;
            if (hero == null) return;

            var actions = st.Actions;
            if (actions == null) return;

            for (int i = 0; i < actions.Length; i++)
            {
                var act = actions[i];
                if (act == null) continue;
                var type = act.GetType();
                string behaviour = GetBehaviourName(act, type);

                foreach (var field in type.GetFields(AllInstance))
                {
                    if (field.FieldType != typeof(HutongGames.PlayMaker.FsmOwnerDefault)) continue;
                    var od = field.GetValue(act) as HutongGames.PlayMaker.FsmOwnerDefault;
                    if (od == null || od.GameObject == null) continue;

                    string refName;
                    try { refName = od.GameObject.Name ?? string.Empty; }
                    catch { refName = string.Empty; }
                    if (refName != HeroRefName && behaviour != HeroControllerName) continue;

                    if (od.GameObject.Value == hero) continue;

                    try
                    {
                        // 切断与（可能已被污染的）变量表的联系，再直接指向小骑士本体
                        od.GameObject.CastVariable = null;
                        od.GameObject.Value = hero;
                    }
                    catch { }

                    _stateFixedCount++;
                    if (_stateFixedCount <= 10)
                    {
                        Modding.Logger.Log("[伴侣] 进入「Can Rest?」时已摆正英雄引用（累计第 " + _stateFixedCount + " 次）");
                    }
                }
            }
        }

        private static GameObject OnGetOwnerDefaultTarget(
            On.HutongGames.PlayMaker.Fsm.orig_GetOwnerDefaultTarget orig,
            HutongGames.PlayMaker.Fsm self,
            HutongGames.PlayMaker.FsmOwnerDefault ownerDefault)
        {
            GameObject result = orig(self, ownerDefault);
            try
            {
                if (self == null || ownerDefault == null) return result;
                if (ownerDefault.OwnerOption != HutongGames.PlayMaker.OwnerDefaultOption.SpecifyGameObject) return result;

                // 只干预长椅状态机。本模组的克隆动作只会污染长椅的引用；
                // 把打击面收窄到这一个状态机，可确保不波及其它模组（例如伴侣自身）的逻辑。
                if (self.Name != BenchFsmName) return result;

                var fgo = ownerDefault.GameObject;
                if (fgo == null) return result;

                // 只认同一个特征：变量名恰为 "Hero"（游戏里"名为 Hero 的变量 = 小骑士"）。
                // 注意：**不能**再把 UseVariable 当作条件 ——
                // 状态机修复会把该引用的 UseVariable 关掉，若这里也要求它为 true，
                // 钩子就会被自己的修复绕过，等于两道防线互相抵消（这正是上一版失效的原因）。
                if (!string.Equals(fgo.Name, HeroRefName, StringComparison.Ordinal)) return result;

                var inst = Instance;
                if (inst != null)
                {
                    inst._hookCalls++;
                    if (inst._hookCalls == 1)
                    {
                        Modding.Logger.Log("[伴侣] 英雄引用解析钩子已生效（Fsm.GetOwnerDefaultTarget 已挂接）");
                    }
                }

                var hc = HeroController.instance;
                if (hc == null) return result;
                var hero = hc.gameObject;
                if (hero == null || result == hero) return result;

                if (inst != null)
                {
                    inst._hookFixedTotal++;
                    if (inst._hookFixedTotal <= 5 || inst._hookFixedTotal % 500 == 0)
                    {
                        Modding.Logger.Log($"[伴侣] 已拦截被顶替的英雄引用（原本解析到 \"{(result != null ? result.name : "null")}\"），" +
                                           $"已改回小骑士本体；累计第 {inst._hookFixedTotal} 次");
                    }
                }
                return hero;
            }
            catch { return result; }
        }

        private void RepairBenchHeroRefs()
        {
            var hc = HeroController.instance;
            if (hc == null) return;
            var hero = hc.gameObject;
            if (hero == null) return;

            PlayMakerFSM[] all;
            try { all = Resources.FindObjectsOfTypeAll<PlayMakerFSM>(); }
            catch { return; }

            foreach (var fsm in all)
            {
                if (fsm == null || fsm.FsmName != BenchFsmName) continue;
                var f = fsm.Fsm;
                if (f == null || f.States == null) continue;

                foreach (var st in f.States)
                {
                    if (st == null || st.Actions == null) continue;
                    foreach (var act in st.Actions)
                    {
                        if (act == null) continue;
                        var type = act.GetType();
                        string behaviour = GetBehaviourName(act, type);

                        foreach (var field in type.GetFields(AllInstance))
                        {
                            if (field.FieldType != typeof(HutongGames.PlayMaker.FsmOwnerDefault)) continue;
                            var od = field.GetValue(act) as HutongGames.PlayMaker.FsmOwnerDefault;
                            if (od == null || od.GameObject == null) continue;
                            if (od.OwnerOption != HutongGames.PlayMaker.OwnerDefaultOption.SpecifyGameObject) continue;

                            string refName;
                            try { refName = od.GameObject.Name ?? string.Empty; }
                            catch { refName = string.Empty; }

                            // 只在能百分之百确定目标时动手
                            if (refName != HeroRefName && behaviour != HeroControllerName) continue;
                            if (od.GameObject.Value == hero) continue;

                            var wrong = od.GameObject.Value;

                            // 关键：FsmGameObject.Value 的 getter 并不检查 UseVariable，只看 CastVariable ——
                            //     get { if (CastVariable == null) return value;
                            //           return CastVariable.RawValue as GameObject; }
                            // 也就是说，只要 CastVariable 非空（该引用是"变量引用"写法），
                            // 无论我们怎么写它自己的 value 字段都会被忽略，读到的是变量表里的值；
                            // 而变量表里的 Hero 同样会被克隆动作波及。
                            // 因此这里把它彻底改成"直接引用"：清掉 CastVariable 并关闭 UseVariable，
                            // 让它与同一状态里另外 6 个 GetState 动作（本来就是直接引用）行为一致。
                            try
                            {
                                od.GameObject.CastVariable = null;
                                od.GameObject.UseVariable = false;
                            }
                            catch { }

                            od.GameObject.Value = hero;

                            // 变量表里的同名变量也一并纠正（有则改之）
                            if (f.Variables != null)
                            {
                                var v = f.Variables.GetFsmGameObject(od.GameObject.Name);
                                if (v != null && v != od.GameObject) v.Value = hero;
                            }
                            ClearMethodCache(act, type);

                            _fixedTotal++;
                            if (!_loggedBenchFix || _fixedTotal <= 5)
                            {
                                _loggedBenchFix = true;
                                Log($"[伴侣] 已纠正长椅状态机中指向伙伴克隆体的英雄引用" +
                                    $"（「{st.Name}」{type.Name}，原为 {(wrong == null ? "null" : wrong.name)}；累计第 {_fixedTotal} 次）");
                            }
                        }
                    }
                }
            }
        }

        private static string GetBehaviourName(object action, Type type)
        {
            try
            {
                var field = type.GetField("behaviour", AllInstance);
                if (field == null) return string.Empty;
                var value = field.GetValue(action);
                if (value == null) return string.Empty;
                var prop = value.GetType().GetProperty("Value");
                return prop != null ? (prop.GetValue(value) as string ?? string.Empty) : string.Empty;
            }
            catch { return string.Empty; }
        }

        private static void ClearMethodCache(object action, Type type)
        {
            foreach (var name in new[] { "cachedBehaviour", "cachedType", "cachedMethodInfo", "cachedParameterInfo", "errorString" })
            {
                try
                {
                    var field = type.GetField(name, AllInstance);
                    if (field != null) field.SetValue(action, null);
                }
                catch { }
            }
        }

    }

}
