# 圣巢之声音频加载器（Hallownest-Vocalized-AudioLoader）——汉化版

- **模组原名**：Hallownest-Vocalized-AudioLoader
- **版本**：1.0.0.0汉化
- **适用游戏**：Hollow Knight（空洞骑士）

## 汉化说明

- **目标**：汉化模组在游戏内玩家可见的文本，仅限文本/字符串层面。
- **内容**：修订模组显示名、菜单文本及内嵌语言资源中的中文文本。
- **不改动项目**：本次汉化不删除或修改任何源代码引用，未改变 mod 功能逻辑。
- **与原版一致性**：
  - `AssemblyVersion` / `FileVersion` 保留原版数字版本。
  - `InformationalVersion` 为 `1.0.0.0汉化`（菜单悬停显示）。
  - 强名称签名与原版保持一致（均为 `PublicKeyToken=null`，未签名）。
- **语言回退**：非中文语言严格返回原英文，仅中文语言显示中文。

## 功能

为圣巢之声模组（Hallownest-Vocalized）异步加载音频资源包，支持：

- 背景菜单主题音频/视频加载
- 滚动字幕（Credits）界面音频加载
- 主音频资源包异步加载（含加载进度提示）

## 安装

1. 将本文件夹内的 `Hallownest-Vocalized-AudioLoader.dll` 复制到游戏的 `hollow_knight_Data/Managed/Mods/` 目录
2. 同时复制 `Hallownest-Vocalized-AudioLoader.pdb`（调试符号，可选）
3. 启动游戏后，在模组菜单中启用「圣巢之声音频加载器」

## 注意

- 该模组为圣巢之声（HKVocal）的音频加载依赖，必须与圣巢之声主模组配合使用
- 若单独启用，圣巢之声将因缺少音频资源而无法正常加载音频
- 旧存档中的模组设置可能会因显示名变更而重置，这是预期行为

## 原版 README（翻译）

# Hallownest-Vocalized Audio Loader

圣巢之声的音频加载依赖模组。负责异步加载背景菜单主题音频、滚动字幕界面音频以及主音频资源包，并提供加载进度提示。

**注意**：此模组必须与 Hallownest-Vocalized 主模组配合使用，不能独立运行。

## 原始说明的翻译

圣巢之声模组的音频加载辅助库。