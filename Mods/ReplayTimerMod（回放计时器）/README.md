# ReplayTimerMod（回放计时器）

- 原版名称：ReplayTimerMod
- 原版版本：0.4.0.0
- 汉化版本：0.4.0汉化（InformationalVersion）；AssemblyVersion/FileVersion = 0.4.0.0（保持原版数字版本）
- 游戏内显示名：回放计时器（通过构造函数 `base("回放计时器")` 设置，原英文名 `ReplayTimerMod` 仅作为dll程序集名称保留）

## 功能简介

ReplayTimerMod 用于记录 Hollow Knight 中各房间的运行过程并支持幽灵回放、PB（BestRun 记录）追踪等功能，包含可选的计时器 HUD 与场景内计时显示。

## 汉化说明

- 所有菜单文字、状态提示、UI 标签与日志输出已翻译为中文。
- 模组显示名由 `ReplayTimerMod` 改为中文「回放计时器」，影响游戏内模组列表、日志与存档键名。
- 未删除任何源代码引用；SharpZipLib 已嵌入合并到主 DLL 中，无需额外 DLL。
- 程序集未签名（PublicKeyToken=null），与原版未签名 DLL 保持一致。

## 安装方法

1. 将 `ReplayTimerMod.HK.dll`（以及 `ReplayTimerMod.HK.pdb`）复制到游戏目录 `Hollow Knight_Data/Managed/Mods`（或通过 BepInEx/模组加载器识别的模组目录）。
2. 在游戏内启用该模组即可使用回放记录与计时功能。

## 文件清单

- `ReplayTimerMod.HK.dll` — 汉化后编译的主程序集
- `ReplayTimerMod.HK.pdb` — 调试符号
- `README.md` — 本说明

## 原版 README（翻译）

# Replay Timer Mod

记录 Hollow Knight 中各房间的运行过程并支持幽灵回放、PB（最佳记录）追踪等功能。

### 功能特点
- 幽灵回放系统
- PB 记录追踪
- 可选计时器 HUD
- 场景内计时显示

### 安装
复制 DLL 文件到 Mods 目录即可。

## 原始说明的翻译

在回放模式下显示计时器。