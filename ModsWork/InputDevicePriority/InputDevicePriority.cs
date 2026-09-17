using System;
using InControl;                  // HollowKnightInputModule 位于 InControl 命名空间
using Modding;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InputDevicePriority
{
    /// <summary>
    /// 【输入设备优先】—— 让菜单光标跟随"你正在用的那个设备"。
    ///
    /// ▍问题
    ///   Hollow Knight 的 UI 菜单里，鼠标输入开关
    ///   （<see cref="HollowKnightInputModule.allowMouseInput"/>）默认恒为 true ——
    ///   游戏自带的 <c>InputHandler.StopMouseInput()</c> / <c>EnableMouseInput()</c>
    ///   这两个方法在整个 Assembly-CSharp 里**一次都没有被调用过**。
    ///   而输入模块判断"鼠标是否在用"的条件是
    ///   <c>(thisMousePosition - lastMousePosition).sqrMagnitude &gt; 0f</c> ——
    ///   只要有哪怕 0.0001 像素的位移就算成立。
    ///   于是即使用手柄操作，鼠标的一丁点抖动就会让输入模块接管，
    ///   把菜单光标抢到鼠标位置。
    ///
    /// ▍做法
    ///   在"UI 导航已启用"期间（也就是菜单里）按最后使用的设备开关鼠标输入：
    ///     · 手柄/键盘一有活动      → 关闭鼠标输入，光标不再被抢；
    ///     · 鼠标真正移动或点击     → 打开鼠标输入，鼠标照常可用；
    ///     · 没接手柄时完全不干预   → 纯鼠标玩家不受任何影响。
    ///
    ///   游戏进行中（UI 导航被关闭）一律不插手，
    ///   避免误启用鼠标而影响战斗操作。
    ///
    /// ▍本模组只改 <c>allowMouseInput</c> 这一个开关，不碰任何游戏数据。
    /// </summary>
    public class InputDevicePriority : Mod, ITogglableMod
    {
        /// <summary>鼠标位移超过 4 像素才算"真的在用鼠标"（平方值，省去开方）。</summary>
        private const float MouseMoveThresholdSq = 16f;

        /// <summary>手柄连接状态的刷新间隔（秒）——GetJoystickNames 会分配数组，不宜逐帧调用。</summary>
        private const float PadCheckInterval = 1f;

        /// <summary>摇杆/十字键的判定阈值。</summary>
        private const float AxisThreshold = 0.3f;

        private static InputDevicePriority _instance;

        private Vector3 _lastMousePos;
        private bool _mouseInUse = true;     // 默认保持游戏原本的行为（鼠标可用）
        private bool _padConnected;
        private float _nextPadCheck;
        private int _lastPolicy = -1;
        private int _switchCount;

        public InputDevicePriority() : base("输入设备优先") { }

        public override string GetVersion() => "1.0.0";

        public override void Initialize()
        {
            _instance = this;
            _lastMousePos = Input.mousePosition;
            On.InControl.HollowKnightInputModule.ShouldActivateModule += OnShouldActivateModule;
            Modding.Logger.Log("[输入设备优先] 已加载");
        }

        public void Unload()
        {
            On.InControl.HollowKnightInputModule.ShouldActivateModule -= OnShouldActivateModule;
            _instance = null;
            Modding.Logger.Log("[输入设备优先] 已卸载");
        }

        /// <summary>
        /// 挂在输入模块的每帧激活判定上 —— 它在 <c>Process()</c>（真正派发鼠标事件）之前执行，
        /// 所以在这里改开关，同一帧就能生效。
        /// </summary>
        private static bool OnShouldActivateModule(
            On.InControl.HollowKnightInputModule.orig_ShouldActivateModule orig,
            HollowKnightInputModule self)
        {
            try { if (_instance != null) _instance.ApplyPolicy(self); }
            catch { /* 任何意外都不该影响游戏本身的输入处理 */ }
            return orig(self);
        }

        private void ApplyPolicy(HollowKnightInputModule self)
        {
            if (self == null) return;

            Vector3 cur = Input.mousePosition;
            EventSystem es = EventSystem.current;

            // 不在菜单里（或 UI 导航被游戏关闭），或压根没接手柄 → 完全不干预。
            // 但仍要把鼠标位置同步住：否则进入菜单那一刻，会拿几百帧之前的旧坐标
            // 去算位移，直接把"鼠标刚才移动过"误判成真，反而把光标抢回去。
            if (es == null || !es.sendNavigationEvents || !PadConnected())
            {
                _lastMousePos = cur;
                return;
            }

            bool mouseMoved = (cur - _lastMousePos).sqrMagnitude > MouseMoveThresholdSq;
            _lastMousePos = cur;

            bool mousePressed = Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2);

            if (mouseMoved || mousePressed)
            {
                _mouseInUse = true;      // 用户真的在用鼠标 → 让鼠标说了算
            }
            else if (PadActive())
            {
                _mouseInUse = false;     // 手柄有输入 → 抑制鼠标，光标不被抢
            }

            self.allowMouseInput = _mouseInUse;

            int policy = _mouseInUse ? 1 : 0;
            if (policy != _lastPolicy)
            {
                _lastPolicy = policy;
                _switchCount++;
                if (_switchCount <= 15)
                {
                    Modding.Logger.Log("[输入设备优先] 切换为「" + (_mouseInUse ? "鼠标" : "手柄")
                        + "」模式，鼠标输入" + (_mouseInUse ? "已启用" : "已抑制（菜单光标不会被抢走）")
                        + "；累计第 " + _switchCount + " 次");
                }
            }
        }

        /// <summary>是否接有手柄（结果缓存一秒，避免逐帧分配数组）。</summary>
        private bool PadConnected()
        {
            if (Time.unscaledTime < _nextPadCheck) return _padConnected;
            _nextPadCheck = Time.unscaledTime + PadCheckInterval;
            try
            {
                string[] names = Input.GetJoystickNames();
                _padConnected = false;
                if (names != null)
                {
                    for (int i = 0; i < names.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(names[i])) { _padConnected = true; break; }
                    }
                }
            }
            catch { _padConnected = false; }
            return _padConnected;
        }

        /// <summary>手柄或键盘方向是否有输入。</summary>
        private static bool PadActive()
        {
            try
            {
                // 手柄按键（JoystickButton0..19 覆盖常见手柄的按钮）
                for (int i = 0; i < 20; i++)
                {
                    if (Input.GetKey((KeyCode)((int)KeyCode.JoystickButton0 + i))) return true;
                }

                // 摇杆 / 十字键：Unity 默认轴同时反映手柄与键盘方向键，
                // 键盘操作同样不该被鼠标抢光标，所以一并算作"非鼠标输入"
                if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > AxisThreshold) return true;
                if (Mathf.Abs(Input.GetAxisRaw("Vertical")) > AxisThreshold) return true;
            }
            catch { }
            return false;
        }
    }
}
