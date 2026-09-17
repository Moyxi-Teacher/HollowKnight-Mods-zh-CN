阶段93：HollowKnightHaptics Bridge --no-hid 参数修复（2026-09-15 14:55）
============================================================

1. 问题发现。
   - 阶段92修复后，游戏内实测仍报告两个症状：
     a) A 键（控制器）释放技能（聚集/施法）不工作，但键盘 A 正常
     b) 脚步震动无反馈
   - 用户确认"是模组的原因，和别的无关"，锁定根因在 HollowKnightHaptics。
   - 关键判别：键盘 A 正常 + 控制器 A 失效 = 游戏逻辑正常，控制器输入被改写。
     两症状同源 = Bridge 对 DualSense 的 HID 写入。
   - 查阅 DualSenseHid.cs 源码确认根因：
     * AssertAudioHaptics()（第106-129行）仅"尝试"写入 HID 输出报告 0x02
       （bytes [2, 0xB0, 0x80, 0, 0, 0x7F, 0xFF, 0, 0]），从不验证写入是否成功
       ——日志中 "HID: audio haptics enabled" 是假阳性。
     * 该报告把 DualSense 切入 PS4 兼容/异常 HID 模式：手柄输入（含 A 键）报告
       格式被改变，游戏无法解析 → A 键失效；而音频触觉在 USB 场景下并未真正
       启用 → 无震动。即写入既"破坏了输入"又"没换来触觉"。

2. 代码修改。
   文件: D:\KDQSHH\TempTest\decompile\HKMod\HollowKnightHaptics\HapticsPlugin.cs
   修改：TryStartBridge() 第113行，在 Bridge 启动参数中追加 --no-hid
   - 旧参数: "--no-extract --port {0} --buffer-ms {1} --latency-ms {2}"
   - 新参数: "--no-extract --no-hid --port {0} --buffer-ms {1} --latency-ms {2}"
   - 新增中文注释说明：禁止 Bridge 写模式选择报告（A 键失效根因；音频触觉假阳性；
     USB 场景禁用不损失功能）。
   - Bridge.exe 无需重编译：Engine.cs 第129行已有 hidAssert = !args.Contains("--no-hid")，
     两处 AssertAudioHaptics() 调用（第327行 Run()、第548行 WatchdogLoop()）均受此标志控制。

3. 构建与部署验证。
   - 构建命令: dotnet build HollowKnightHaptics.csproj -c Release
   - 构建结果: 0 个警告，0 个错误
   - 构建产物: D:\KDQSHH\TempTest\decompile\HKMod\bin\Release\net472\HollowKnightHaptics.dll
     （24064 字节；Python 按 UTF-16LE 校验含 --no-hid 与 --no-extract 字面量）
   - 部署路径: E:\steam\steamapps\...\BepInEx\plugins\HollowKnightHaptics\HollowKnightHaptics.dll
   - 部署后验证: 读取部署文件确认含 --no-hid，大小一致。
   - 进程清理: 停止残留 HapticsBridge 进程 PID 39576（游戏已关闭，Bridge 不锁主 DLL）。
   - 备份说明: 修改前部署 DLL（阶段74产物，不含 --no-hid）为引发 A 键失效的源头版本，
     本次未做修改前独立备份即被覆盖；该版本无需回滚，如需可从源码去掉 --no-hid 重编译恢复。
     修复后版本已独立备份至 backup\HollowKnightHaptics_20260915_deployed.dll。

## 下一步计划
- 用户重启 Hollow Knight（插件将以 --no-hid 自动拉起 Bridge，Bridge 不再写 HID 模式选择）：
  1. 测试控制器 A 键释放技能（聚集/施法）是否恢复
  2. 测试行走/受击震动是否恢复
- 核对 HapticsBridge.log：预期不再出现 "HID: audio haptics enabled" 行（确认 --no-hid 生效）
- 若 A 键恢复但仍无震动：USB 原生音频触觉在该 PC 不工作 → 转原生 rumble（SmallRumble）通路排查
- 若 A 键仍失效：进一步查 WASAPI 共享占用对 HID 枚举的间接影响，必要时排除启动音效（--no-chime）
