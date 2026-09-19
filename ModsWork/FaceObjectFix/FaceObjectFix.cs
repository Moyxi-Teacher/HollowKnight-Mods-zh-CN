using Modding;
using System;
using HutongGames.PlayMaker.Actions;

namespace FaceObjectFix
{
    /// <summary>
    /// 「朝向动作修正」—— 修复原版 FaceObject 动作的空引用缺陷。
    ///
    /// 原版实现（Assembly-CSharp.dll 的 HutongGames.PlayMaker.Actions.FaceObject）有两处隐患：
    ///   ① DoFace() 第一行就读取 objectA.Value.transform.localScale，
    ///      但整个方法只检查 objectB，从不检查 objectA；
    ///   ② OnEnter() 在 _sprite == null 时调用了 Finish() 却没有 return，
    ///      之后仍然访问 objectA.Value 并继续执行 DoFace()。
    /// 当动作的宿主对象被销毁（例如场景重载、对象提前消失）而 FSM 仍在执行该状态时，
    /// 就会每帧抛出 NullReferenceException；异常会中断该动作的执行，
    /// 使 NPC/敌人卡在朝向状态上不响应（可能表现为无法交互）。
    ///
    /// 修复方式：在调用原方法之前先校验 objectA / objectB 是否有效，
    /// 无效则结束该动作（与原版「objectB 无效时 Finish()」的设计保持一致），
    /// 并对内部引用失效的极少数情况做兜底，避免异常刷屏拖慢游戏。
    /// 本模组不改变任何正常情况下的行为。
    /// </summary>
    public class FaceObjectFix : Mod
    {
        public FaceObjectFix() : base("朝向动作修正") { }

        public override string GetVersion() => "1.0.0.0";

        /// <summary>已拦截的无效调用次数（只统计，避免刷屏）。</summary>
        private static int suppressed;

        public override void Initialize()
        {
            On.HutongGames.PlayMaker.Actions.FaceObject.OnEnter += OnEnterHook;
            On.HutongGames.PlayMaker.Actions.FaceObject.DoFace += DoFaceHook;
        }

        public void Unload()
        {
            On.HutongGames.PlayMaker.Actions.FaceObject.OnEnter -= OnEnterHook;
            On.HutongGames.PlayMaker.Actions.FaceObject.DoFace -= DoFaceHook;
        }

        private static void OnEnterHook(
            On.HutongGames.PlayMaker.Actions.FaceObject.orig_OnEnter orig,
            HutongGames.PlayMaker.Actions.FaceObject self)
        {
            if (!EndpointsValid(self)) return;
            try
            {
                orig(self);
            }
            catch (NullReferenceException)
            {
                OnInternalNull(self);
            }
        }

        private static void DoFaceHook(
            On.HutongGames.PlayMaker.Actions.FaceObject.orig_DoFace orig,
            HutongGames.PlayMaker.Actions.FaceObject self)
        {
            if (!EndpointsValid(self)) return;
            try
            {
                orig(self);
            }
            catch (NullReferenceException)
            {
                OnInternalNull(self);
            }
        }

        /// <summary>
        /// 校验动作两端的对象是否都存在且未被销毁。
        /// 任一无效时结束该动作并返回 false，调用方据此跳过原方法。
        /// </summary>
        private static bool EndpointsValid(HutongGames.PlayMaker.Actions.FaceObject self)
        {
            if (self == null) return false;

            // objectA 是要被翻转的对象；原版从不检查它，这正是空引用的来源。
            // （Unity 的 == 重载会把「已销毁」的引用也判为 null）
            if (self.objectA == null || self.objectA.Value == null)
            {
                self.Finish();
                return false;
            }

            // objectB 是朝向目标；原版会检查，这里保持同样的处理。
            if (self.objectB == null || self.objectB.Value == null || self.objectB.IsNone)
            {
                self.Finish();
                return false;
            }

            return true;
        }

        /// <summary>
        /// 原方法内部还有动画组件等引用（如 _sprite），失效时同样会抛空引用。
        /// 这里结束该动作而不是让异常继续扩散，避免拖慢游戏或让 FSM 卡住。
        /// </summary>
        private static void OnInternalNull(HutongGames.PlayMaker.Actions.FaceObject self)
        {
            suppressed++;
            if (suppressed <= 5 || suppressed % 100 == 0)
            {
                Logger.Log($"[朝向动作修正] 已拦截 FaceObject 内部空引用 {suppressed} 次（宿主：{OwnerName(self)}）");
            }
            if (self != null)
            {
                self.Finish();
            }
        }

        /// <summary>取动作宿主的对象名，仅用于日志。</summary>
        private static string OwnerName(HutongGames.PlayMaker.Actions.FaceObject self)
        {
            try
            {
                if (self != null && self.Fsm != null && self.Fsm.GameObject != null)
                {
                    return self.Fsm.GameObject.name;
                }
            }
            catch
            {
                // 宿主已销毁，忽略
            }
            return "未知";
        }
    }
}
