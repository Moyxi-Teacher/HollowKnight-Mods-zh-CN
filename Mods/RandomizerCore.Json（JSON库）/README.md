# RandomizerCore.Json（JSON库）

**原名**：RandomizerCore.Json
**作者**：homothetyhk
**版本**：1.1.1汉化
**许可证**：LGPL-2.1-only

## 汉化说明

RandomizerCore.Json 是 [RandomizerCore](https://github.com/homothetyhk/RandomizerCore)
的 JSON 序列化/反序列化扩展库，为随机化器提供 JSON 编码的逻辑数据读写能力。
它自身没有游戏内菜单、没有 `Language.json`，也不通过
`ModHooks.LanguageGetHook` 暴露任何玩家可见的文本——其源码仅包含
Newtonsoft.Json 转换器（Converter）、逻辑契约解析器（ContractResolver）、
逻辑对象读写器/写入器等底层序列化逻辑。

因此本次汉化**无文本改动**，仅做以下工程性调整：

## 变更记录

- 版本号固定为 `1.1.1`（源码 `VersionPrefix` + NuGet 版本 `1.1.1`）；
  `AssemblyVersion`/`FileVersion=1.1.1.0`，`InformationalVersion=1.1.1汉化`
  （菜单悬停显示）。
- 关闭强名称签名（全局 `SignAssembly=false`），与原版 DLL（PublicKeyToken=null）一致。
- 依赖 `RandomizerCore` 与 `Newtonsoft.Json` 通过 NuGet 包还原获取
  （原版 csproj 在 HK 本地引用模式下需 `$(References)` 指向本地目录，
  本次采用 NuGet 包还原方式，保证依赖可用）。
- 构建后 `PostBuild` 目标因 `PostBuildPath` 为空而**自动不执行**，
  因此**不会**复制编译产物到游戏目录，保护只读游戏目录。
- 所有源码引用保持完整，未删除任何功能代码。

## 与原版一致性

| 项目 | 原版 | 汉化版 |
|------|------|--------|
| AssemblyVersion | 1.1.1.0 | 1.1.1.0 ✓ |
| PublicKeyToken | null | null ✓ |
| 内嵌资源 | 0 | 0 ✓ |
| 程序集引用 | NuGet (RandomizerCore 2.1.0, Newtonsoft.Json) | 一致 ✓ |

## 原始信息

仓库：https://github.com/homothetyhk/RandomizerCore.Json

本模组遵循 LGPL-2.1 许可证。

## 原版 README

This is a library to supplement [RandomizerCore](https://github.com/homothetyhk/RandomizerCore).

For examples of use, see [code](https://github.com/homothetyhk/RandomizerMod/blob/master/RandomizerMod/RC/RCData.cs) and [data](https://github.com/homothetyhk/RandomizerMod/tree/master/RandomizerMod/Resources/Logic) for loading logic files into a randomizer project.

## 原版 README（翻译）

这是用于补充 [RandomizerCore](https://github.com/homothetyhk/RandomizerCore) 的库。

使用示例请参见[代码](https://github.com/homothetyhk/RandomizerMod/blob/master/RandomizerMod/RC/RCData.cs)和[数据](https://github.com/homothetyhk/RandomizerMod/tree/master/RandomizerMod/Resources/Logic)文件，这些文件用于加载随机化器项目中的逻辑文件。

## 原始说明的翻译

补充 RandomizerCore 的 JSON 序列化库。