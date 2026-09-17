# WavLib（WAV库）

**原名**：WavLib
**版本**：1.1.9.0汉化
**许可证**：MIT

## 汉化说明

WavLib 是 SFGrenade 开发的音频 WAV 文件读写库，纯 .NET Standard/.NET Framework
类库。它自身不嵌入任何 Unity 游戏类型引用（仅 mscorlib/System.Data/System.Core），
不包含 `Language.json`，不调用任何语言钩子，也没有玩家可见的文本界面。

因此本次汉化**无文本改动**，仅做以下工程性调整：

## 变更记录

- `Version` 改为 `1.1.9.0`（源码仓库标示为 1.1.0.0，但游戏内原版 DLL 为 1.1.9.0，
  按用户决策以原版数字为准）；`AssemblyVersion`/`FileVersion` 随之为 1.1.9.0；
  `InformationalVersion=1.1.9.0汉化`（菜单悬停显示）。
- 关闭强名称签名（SignAssembly=false，全局继承），与原版 DLL（PublicKeyToken=null）一致。
- 仅构建 `net472` 目标框架（`-p:TargetFramework=net472 -p:GeneratePackageOnBuild=false`），
  跳过多目标 NuGet 还原与 NuGet 打包流程，保证离线构建。
- 所有源码引用保持完整，未删除任何功能代码。

## 与原版一致性

| 项目 | 原版 | 汉化版 |
|------|------|--------|
| AssemblyVersion | 1.1.9.0 | 1.1.9.0 ✓ |
| PublicKeyToken | null | null ✓ |
| 内嵌资源 | 0 | 0 ✓ |
| 程序集引用（3个） | mscorlib/System.Data/System.Core | 逐项一致 ✓ |

## 原始信息

WavLib 提供 WAV 格式的解析/写入能力，包括 PCM 与 Mu-law/A-law 转换等。
仓库：https://github.com/SFGrenade/WavLib

## 原版 README

# WavLib

Lightweight RIFF WAVE library for .NET Framework, .NET Standard and .NET Core.

# EUPL
                      Copyright (c) 2024 SFGrenade
                      Licensed under the EUPL-1.2
https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12

## 原版 README（翻译）

# WavLib（WAV 库）

适用于 .NET Framework、.NET Standard 和 .NET Core 的轻量级 RIFF WAVE 库。

# EUPL 许可
版权所有 (c) 2024 SFGrenade
基于 EUPL-1.2 许可协议授权
https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12

## 原始说明的翻译

适用于 .NET 的轻量级 RIFF WAVE 音频库。