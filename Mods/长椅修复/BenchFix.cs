using System;
using System.Collections.Generic;
using Modding;
using UnityEngine;

namespace BenchFix
{
    /// <summary>
    /// 【修复模组】长椅状态不一致修正。
    ///
    /// 问题（经诊断模组实测确认）：
    ///   在长椅上退出游戏 → 存档记录 PlayerData.atBench = true。
    ///   但重新加载该存档时，场景内长椅的 "Bench Control" 状态机停在
    ///   "Idle Start Pause"（空闲）——并未进入休息状态，二者状态不一致。
    ///
    ///   后果：玩家走到长椅旁按上键时，状态机进入 "Init Resting"，
    ///   检测到 atBench 已为 true（认为"已经在长椅上"）→ 转入 "Cancel Frame" 取消，
    ///   并把 atBench 置回 false，但状态机就此卡在 "Cancel Frame"，
    ///   之后无论怎么按上键都不再响应 —— 表现为"提示出现但按上键没反应"。
    ///
    /// 修复方式：
    ///   每次场景加载完成后检查一次：若 PlayerData.atBench 为 true，
    ///   但场景内长椅的状态机并未处于休息相关状态（状态名不含 "Rest"），
    ///   则说明状态不一致 —— 把 atBench 复位为 false，
    ///   使玩家能够正常按上键坐下休息。
    ///
    ///   同时兜底：若状态机卡在 "Cancel Frame"，把它复位到空闲状态。
    /// </summary>
    public class BenchFix : Mod, ITogglableMod
    {
        private const string TAG = "[长椅修复]";
        private const string BENCH_FSM_NAME = "Bench Control";
        private const string IDLE_STATE = "Idle Start Pause";
        private const string STUCK_STATE = "Cancel Frame";

        public BenchFix() : base("长椅修复") { }

        public override string GetVersion() => "1.0.0";

        public override void Initialize()
        {
            Log($"{TAG} 已加载：修正「存档在长椅上但状态机未进入休息」的状态不一致");
            On.GameManager.OnNextLevelReady += OnNextLevelReady;
        }

        public void Unload()
        {
            On.GameManager.OnNextLevelReady -= OnNextLevelReady;
            Log($"{TAG} 已卸载");
        }

        private void OnNextLevelReady(On.GameManager.orig_OnNextLevelReady orig, GameManager self)
        {
            orig(self);
            try
            {
                FixInconsistentState();
            }
            catch (Exception e)
            {
                Log($"{TAG} 检查异常: {e.Message}");
            }
        }

        private void FixInconsistentState()
        {
            PlayerData pd = PlayerData.instance;
            if (pd == null) return;

            // 只关心"存档认为玩家在长椅上"的情况
            bool atBench = false;
            try { atBench = pd.atBench; }
            catch { return; }
            if (!atBench) return;

            // 检查当前场景内所有长椅状态机
            string currentScene = GameManager.instance != null ? GameManager.instance.sceneName : null;
            var fsms = Resources.FindObjectsOfTypeAll<PlayMakerFSM>();
            bool foundResting = false;
            bool foundAnyBench = false;

            foreach (var fsm in fsms)
            {
                if (fsm == null || fsm.FsmName != BENCH_FSM_NAME) continue;
                if (fsm.gameObject == null || !fsm.gameObject.activeInHierarchy) continue;
                if (!string.IsNullOrEmpty(currentScene) && fsm.gameObject.scene.name != currentScene) continue;   // 只看当前场景

                foundAnyBench = true;
                string state = "?";
                try { state = fsm.Fsm?.ActiveStateName ?? "?"; }
                catch { }

                Log($"{TAG} 发现长椅 {fsm.gameObject.name}（场景 {fsm.gameObject.scene.name}）当前状态: {state}");

                if (state.IndexOf("Rest", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    // 状态机确实在休息（或休息流程）中，说明 atBench=true 是合理的，不处理
                    foundResting = true;
                    continue;
                }

                // 状态机不在休息状态 → 与 atBench=true 不一致
                if (state == STUCK_STATE)
                {
                    // 已卡死在取消状态，复位到空闲
                    try
                    {
                        fsm.Fsm.SetState(IDLE_STATE);
                        Log($"{TAG} 长椅状态机卡在「{STUCK_STATE}」，已复位到「{IDLE_STATE}」");
                    }
                    catch (Exception e)
                    {
                        Log($"{TAG} 复位状态机失败: {e.Message}");
                    }
                }
            }

            if (!foundAnyBench) return;
            if (foundResting) return;

            // 状态机未在休息 → 修正 atBench，避免按上键时被误判为"已在长椅上"而取消
            try
            {
                pd.atBench = false;
                Log($"{TAG} 已修正状态不一致：atBench 由 true 复位为 false（长椅状态机处于空闲，未进入休息）");
            }
            catch (Exception e)
            {
                Log($"{TAG} 复位 atBench 失败: {e.Message}");
            }
        }
    }
}
