# Vasi（工具库）

**原名**：Vasi
**版本**：2.0.0汉化
**许可证**：MIT

## 汉化说明

Vasi 是 Hollow Knight 的依赖库模组（由 Yusuf Bham 开发），为其他模组提供
PlayMaker FSM 工具、动画工具、反射镜像（Mirror 源生成器）等实用功能。
它本身没有玩家可见的界面或文本，不包含 `Language.json`，也从不调用
`ModHooks.LanguageGetHook`——可见字符串全部来自游戏原生语言表。

因此本次汉化**无文本改动**，仅做以下工程性调整：

## 变更记录

- `InformationalVersion` 设为 `2.0.0汉化`（菜单悬停显示）；
  `AssemblyVersion`/`FileVersion` 保持 2.0.0 与原版一致。
- 关闭强名称签名（SignAssembly=false），与原版 DLL（PublicKeyToken=null）一致。
- 中断 `CopyMod` 与 `CreateReleaseZip` 两个构建后目标（`Condition="false"`）：
  它们原本会把产物复制到作者本机的游戏目录（D:/Steam/...），不符合本任务
  "编译完不得写入游戏目录"的约束。此改动仅影响构建行为，不影响 DLL 功能。
- 所有源码引用保持完整，未删除任何功能代码。

## 与原版一致性

| 项目 | 原版 | 汉化版 |
|------|------|--------|
| AssemblyVersion | 2.0.0.0 | 2.0.0.0 ✓ |
| PublicKeyToken | null | null ✓ |
| 内嵌资源 | 0 | 0 ✓ |
| 程序集引用（8个） | ✓ | 逐项一致 ✓ |

## 原始信息

Vasi 提供的核心工具：`FsmUtil`（FSM 状态/动作/过渡操作）、`AnimatorUtil`、
`GameObjectUtil`、`EnumerableUtil`、`Mirror`（反射源生成器）、`InvokeMethod`/
`InvokeCoroutine`（IL 后处理注入）等。仓库：https://github.com/yusuf-bham/Vasi

本模组遵循 MIT 许可证。

## 原版 README

# Vasi

A small library for Hollow Knight mods. It packages a few utility classes which deal with FSMs, animating, reflection, versioning, etc.

## 原版 README（翻译）

# Vasi（工具库）

Hollow Knight 模组的小型库。它打包了一些实用类，用于处理 FSM、动画、反射、版本控制等。

## 原始说明的翻译

Hollow Knight 模组的实用工具库，包含 FSM、动画、反射、版本控制等工具类。