# Core.FsmUtil（状态机工具）

**原名**：Core.FsmUtil
**版本**：1.1.2.0汉化
**许可证**：MIT

## 汉化说明

Core.FsmUtil 是 Hollow Knight 模组核心依赖库，封装
FSM（PlayMaker 有限状态机）查询/编辑的便捷 API。其暴露的仅为
开发者接口，源码中的英文文本多为 XML 文档注释（`/// <param>`）
与 API 形参描述，**不向玩家显示**。

- 无 `Language.json`；
- 无 `ModHooks.LanguageGetHook` / `LanguageStringKey` 语言钩子；
- 无硬编码玩家可见字符串。

因此本次汉化**无文本改动**，仅同步版本信息：

- `AssemblyVersion=1.1.2.0`、`FileVersion=1.1.2.0` —— 与原版一致。
- `InformationalVersion=1.1.2.0汉化` —— 菜单悬停显示。

## 版本号说明

- 原版 `Core_FsmUtil.dll`：AssemblyVersion=1.1.2.0，PublicKeyToken=null，
  InformationalVersion=1.0.0（原版作者未同步，属疏漏），无内嵌资源，
  7 个程序集引用。
- 按用户决策（2026-09-11）统一处理：AssemblyVersion/FileVersion=1.1.2.0，
  InformationalVersion=1.1.2.0汉化。

## 变更记录

- 添加 `InformationalVersion=1.1.2.0汉化`（csproj 原无该属性）。
- 关闭强名称签名（全局 `SignAssembly=false`），与原版 DLL
  （PublicKeyToken=null）一致。
- 引用路径 `$(HollowKnightRefs)=../HkRefs` 经 `Directory.Build.props`
  全局变量 `$(HkRefs)` 解析到 `D:\KDQSHH\ModsWork\HkRefs`；
  `<Reference Include="$(HollowKnightRefs)/*.dll" />` 保留原样。
- 禁用 `build_steps.targets` 中两个部署目标
  `Copy mod to export dir` / `Copy mod to game dir`
  （均设 `Condition="false"`），防止复制编译产物到游戏目录。
- 全部 7 个程序集引用原样保留，未删任何源代码引用。

## 与原版一致性

| 项目 | 原版 | 汉化版 |
|------|------|--------|
| AssemblyVersion | 1.1.2.0 | 1.1.2.0 ✓ |
| FileVersion | 1.1.2.0 | 1.1.2.0 ✓ |
| PublicKeyToken | null | null ✓ |
| InformationalVersion | 1.0.0 | 1.1.2.0汉化 ✓ |
| 内嵌资源 | 0 | 0 ✓ |
| 程序集引用（7） | mscorlib、Assembly-CSharp、UnityEngine.CoreModule、PlayMaker、MMHOOK_PlayMaker、System、MonoMod.RuntimeDetour | 逐项一致 ✓ |

## 原始信息

仓库：https://github.com/hk-modding/HK.Core.FsmUtil

## 原版 README

# Core_FsmUtil

A library that makes it easy to edit PlayMakerFSMs.

## 原版 README（翻译）

# Core_FsmUtil（状态机工具）

一个使 PlayMakerFSM 编辑变得简单的库。

## 原始说明的翻译

一个简化 PlayMakerFSM 编辑操作的库。