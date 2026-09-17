# ConnectionMetadataInjector（连接元数据注入器）

**原名**：ConnectionMetadataInjector
**版本**：2.2.9750.1271汉化
**许可证**：MIT

## 汉化说明

ConnectionMetadataInjector 是 Hollow Knight 模组依赖库，负责在模组
间传递连接状态和元数据。源码仅有英文 API 注释，无 `Language.json`，
无 `ModHooks.LanguageGetHook` / `LanguageStringKey` 语言钩子，无玩家
可见自然语言文本。

原版的两个内嵌资源 `itemGroups.json`、`locationGroups.json` 是
ItemChanger 游戏对象数据文件（池组/位置组键），非文本本地化内容，
原样保留。

因此本次汉化**无文本改动**，仅同步版本信息：
- `AssemblyVersion=2.2.9750.1271`、`FileVersion=2.2.9750.1271`
  —— 原版使用通配符 `<AssemblyVersion>2.2.*</AssemblyVersion>`，
  固定为编译时实际生成的版本号。
- `InformationalVersion=2.2.9750.1271汉化` —— 菜单悬停显示。

## 版本号说明

原版 DLL 由作者本地通过 `<AssemblyVersion>2.2.*</AssemblyVersion>`
随机时间戳生成，版本号为 `2.2.9750.1271`。按用户决策（2026-09-11）：
`AssemblyVersion/FileVersion` 固定为原版数字（保持兼容），
`InformationalVersion` 附加 `"汉化"` 后缀。

## 变更记录

- 固定版本：`AssemblyVersion` 改为 `2.2.9750.1271`，
  新增 `FileVersion=2.2.9750.1271`、
  `InformationalVersion=2.2.9750.1271汉化`。
- 引用路径：`$(HollowKnightRefs)` 从 `../API` 改为 `$(HkRefs)`，
  经 `Directory.Build.props` 全局变量统一指向
  `D:\KDQSHH\ModsWork\HkRefs`（原版路径不存在）。
- 禁用 `CopyMod` 构建后目标（`Condition="false"`），防止复制产物
  到游戏目录及导出目录。
- 关闭强名称签名（全局 `SignAssembly=false`），与原版
  （PublicKeyToken=null）一致。
- 全部 6 个程序集引用原样保留，未删任何源代码引用。

## 与原版一致性

| 项目 | 原版 | 汉化版 |
|------|------|--------|
| AssemblyVersion | 2.2.9750.1271 | 2.2.9750.1271 ✓ |
| PublicKeyToken | null | null ✓ |
| InformationalVersion | 1.0.0 | 2.2.9750.1271汉化 ✓ |
| 内嵌资源（2） | itemGroups.json、locationGroups.json | 2 ✓（完全一致） |
| 程序集引用（6） | mscorlib、Assembly-CSharp、ItemChanger 2.1.6.0、System.Core、Newtonsoft.Json 11.0.0.0、System | 逐项一致 ✓ |

## 原始信息

仓库：https://github.com/BadMagic100/ConnectionMetadataInjector

## 原版 README

# ConnectionMetadataInjector

A helper mod that allows Hollow Knight mods that are supplemental to Randomizer to request additional metadata information from connections, while allowing connections to specify this metadata without declaring hard dependencies.

## 原版 README（翻译）

# ConnectionMetadataInjector（连接元数据注入器）

一个辅助模组，允许作为随机化器补充的 Hollow Knight 模组从连接中请求额外的元数据信息，同时允许连接在不声明硬性依赖的情况下指定这些元数据。

## 原始说明的翻译

一个辅助模组，允许随机化器补充模组在不声明硬性依赖的情况下从连接中获取元数据。