using System.Collections.Generic;
using Modding;
using UnityEngine;
using HutongGames.PlayMaker;
using Satchel;

namespace DynamicCrystalDash {
    public class DynamicCrystalDash: Mod, IMenuMod, ITogglableMod {
        public static float chargeStartTime;
        public static float totalChargeTime;
        public static bool charging;

        // ===== 速度上限配置 =====
        // 防止蓄力过久导致速度过高、冲出地图边界导致卡死
        public const float MaxChargeTime = 8f;       // 最大蓄力时间（秒）
        public const float MaxDashSpeed  = 50f;      // 最高冲刺速度（像素/帧）
        public const float MaxDashDist   = 600f;     // 冲刺最大距离（像素），超出则强制终止
        public static Vector3 dashStartPos;            // 冲刺起点位置

        public DynamicCrystalDash() : base("动态水晶冲刺") {}
        public override string GetVersion() => "1.0.0.1";
        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects) {
            On.PlayMakerFSM.OnEnable += updateFSMs;
        }

        public bool ToggleButtonInsideMenu => true;

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry) {
            return new List<IMenuMod.MenuEntry> {
                new(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values, "水晶冲刺速度根据蓄力时间动态变化，替代固定的满速冲刺。", toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader)
            };
        }

        public void Unload() {
            On.PlayMakerFSM.OnEnable -= updateFSMs;
        }

        private void updateFSMs(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(self.gameObject.name == "Knight" && self.FsmName == "Superdash") {
                self.GetValidState("Relinquish Control").AddAction(new setChargeStart());
                
                FsmState wallChargeState = self.GetValidState("Wall Charge");
                wallChargeState.ChangeTransition("BUTTON UP", "Wall Charged");
                wallChargeState.RemoveAction(15);
                FsmState groundChargeState = self.GetValidState("Ground Charge");
                groundChargeState.ChangeTransition("BUTTON UP", "Ground Charged");
                groundChargeState.RemoveAction(10);

                FsmState leftState = self.GetValidState("Left");
                leftState.RemoveAction(0);
                leftState.InsertAction(new setDynamicSpeed(self, -1), 0);
                FsmState rightState = self.GetValidState("Right");
                rightState.RemoveAction(0);
                rightState.InsertAction(new setDynamicSpeed(self, 1), 0);

                self.GetValidState("Cancelable").RemoveAction(1);
            }
            else if(self.gameObject.name.StartsWith("SD Crystal Gen") && self.FsmName == "superdash_crystal_gen") {
                int index = (self.gameObject.name.StartsWith("SD Crystal Gen G") ? 3 : 1);
                FsmState chargingState = self.GetValidState("Charging");
                chargingState.RemoveAction(index);
                chargingState.InsertAction(new fakeiTweenMoveBy(self.gameObject),index);
            }
        }
    }

    public class setChargeStart: FsmStateAction {
        public override void OnEnter() {
            DynamicCrystalDash.charging = true;
            DynamicCrystalDash.chargeStartTime = Time.time;
            // 记录冲刺起点，用于检测是否冲出地图边界
            DynamicCrystalDash.dashStartPos = this.Fsm.GameObject.transform.position;
            Finish();
        }
    }

    public class setDynamicSpeed: FsmStateAction {
        private PlayMakerFSM self;
        private int positiveDirection;
        public setDynamicSpeed(PlayMakerFSM fsm, int positiveDirection) {
            self = fsm;
            this.positiveDirection = positiveDirection;
        }
                public override void OnEnter() {
            float totalTime = Time.time - DynamicCrystalDash.chargeStartTime;
            // 限制蓄力时长，防止速度无限增长
            totalTime = Mathf.Min(totalTime, DynamicCrystalDash.MaxChargeTime);
            DynamicCrystalDash.totalChargeTime = totalTime;
            float speed = 37.5f * totalTime * positiveDirection;
            // 额外限制最高速度
            speed = Mathf.Clamp(speed, -DynamicCrystalDash.MaxDashSpeed, DynamicCrystalDash.MaxDashSpeed);
            self.FsmVariables.GetFsmFloat("Current SD Speed").SafeAssign(speed);
            self.FsmVariables.GetFsmFloat(positiveDirection > 0 ? "Superdash Speed" : "Superdash Speed neg").SafeAssign(speed);

            // 记录冲刺起点位置，用于距离检测
            DynamicCrystalDash.dashStartPos = self.transform.position;

            DynamicCrystalDash.charging = false;
            Finish();
        }

        // 每帧检测冲刺距离，超出安全范围则终止冲刺防止冲出地图
        public override void OnUpdate() {
            if(DynamicCrystalDash.dashStartPos != Vector3.zero
               && self != null && self.gameObject != null) {
                float dist = Vector3.Distance(self.gameObject.transform.position,
                    DynamicCrystalDash.dashStartPos);
                if(dist > DynamicCrystalDash.MaxDashDist) {
                    // 强制终止冲刺：发送取消事件并重置状态
                    self.SendEvent("Cancel");
                    DynamicCrystalDash.charging = true;
                }
            }
        }
    }

    public class fakeiTweenMoveBy: FsmStateAction {
        private Vector3 movement;
        private GameObject gameObject;
        public fakeiTweenMoveBy(GameObject gameObject) {
            this.gameObject = gameObject;
            switch(gameObject.name) {
                case "SD Crystal Gen G1":
                    movement = new Vector3(3.75f, 0, 0);
                    break;
                case "SD Crystal Gen G2":
                    movement = new Vector3(-3.75f, 0, 0);
                    break;
                case "SD Crystal Gen W1":
                    movement = new Vector3(0, 3.75f, 0);
                    break;
                case "SD Crystal Gen W2":
                    movement = new Vector3(0, -3.75f, 0);
                    break;
            }
        }
        public override void OnUpdate() {
            this.gameObject.transform.position += movement * Time.deltaTime;
            if(!DynamicCrystalDash.charging)
                Finish();

            // 检测是否冲出安全范围（超出冲刺起点 MaxDashDist），强制终止
            if(DynamicCrystalDash.dashStartPos != Vector3.zero
               && Vector3.Distance(this.gameObject.transform.position,
                   DynamicCrystalDash.dashStartPos) > DynamicCrystalDash.MaxDashDist) {
                DynamicCrystalDash.charging = true;
                Finish();
            }
        }
    }
}
