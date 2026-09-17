# FrogCore（蛙核）

**原名**：FrogCore
**版本**：0.0.0.0汉化
**许可证**：MIT

## 汉化说明

FrogCore 是 Hollow Knight 模组核心依赖库，为作者其余模组提供
helpers/Module/Patches/UI 等基础能力。FrogCore 自身不向玩家显示
自然语言文本，无 `Language.json`，仅作为库被其他模组引用。
因此本次汉化**无文本改动**。

## 版本号说明

原版 FrogCore.dll 采用旧式（非 SDK）csproj，**无任何程序集版本
特性**，默认 `0.0.0.0`。由于定义为传统项目，不具备
`Directory.Build.props` 自动导入，版本号写入手写的
`AssemblyInfo.cs`：

- `AssemblyVersion=0.0.0.0`、`FileVersion=0.0.0.0` —— 与原版一致（兼容）。
- `InformationalVersion=0.0.0.0汉化` —— 菜单悬停显示。

## 变更记录

- 新增 `AssemblyInfo.cs`：补上程序集版本元数据（原版为空），记录
  `0.0.0.0汉化` 版本信息。
- 解除 csproj 中作者本地多路径、仅 `Exists()` 条件生效的
  `HollowKnightRefs`，使引用路径经 `Directory.Build.props` 全局变量
  `$(HkRefs)` 解析到 `D:\KDQSHH\ModsWork\HkRefs`。
- 关闭强名称签名（全局 `SignAssembly=false`），与原版 DLL
  （PublicKeyToken=null）一致。
- 禁用 `CopyMod` 构建后目标（`Condition="false"`），防止复制编译产物
  到游戏目录及任何导出路径。
- 全部 12 个程序集引用原样保留，未删任何源代码引用。

## 与原版一致性

| 项目 | 原版 | 汉化版 |
|------|------|--------|
| AssemblyVersion | 0.0.0.0 | 0.0.0.0 ✓ |
| PublicKeyToken | null | null ✓ |
| 内嵌资源 | 0 | 0 ✓ |
| 程序集引用（12） | mscorlib、UnityEngine.CoreModule、Assembly-CSharp、PlayMaker、MMHOOK_Assembly-CSharp、UnityEngine.Physics2DModule、MonoMod.RuntimeDetour、Newtonsoft.Json、UnityEngine.AssetBundleModule、UnityEngine.AudioModule、System.Core、UnityEngine.AnimationModule | 逐项一致 ✓ |

## 原始信息

仓库：https://github.com/RedFrog6002/FrogCore

## 原版 README

# FrogCore

A library of useful classes or Helpers that I make or stuff that doesn't belong anywhere but still may be of use to some people

## 原版 README（翻译）

# FrogCore（蛙核）

我制作的实用类或辅助工具的库，或者那些不属于其他地方但可能对某些人有用的东西。

## 原始说明的翻译

包含实用类和辅助工具的库，可能对某些模组开发者有用。