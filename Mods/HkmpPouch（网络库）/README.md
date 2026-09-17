# HkmpPouch（网络库）

## 模组信息

| 项目 | 内容 |
|---|---|
| 原始名称 | HkmpPouch |
| 中文名称 | 网络库 |
| 模组类型 | 依赖库 |
| 模组版本 | 2.0.0.0汉化 |
| 模组作者 | Clazex |

## 模组简介

HKMP（多人联机）模组的数据存储与网络通信辅助库。仅作为依赖存在，无独立菜单。

## 必需依赖

本模组运行时**必须**同时加载 `HKMP.dll`（多人联机框架）。缺少 HKMP.dll 会导致构造阶段抛出 `TypeLoadException`，模组无法加载。

`HKMP.dll` 本身又依赖 NuGet 包 `BouncyCastle.Cryptography 2.5.1`：
其 `DtlsClient._tlsClient` 字段类型 `ClientTlsClient` 继承自 BouncyCastle 的 TLS 类。
若缺少 `BouncyCastle.Cryptography.dll`，HKMP 初始化时会抛出
`TypeLoadException: Invalid type Hkmp.Networking.Client.ClientTlsClient`，进而导致整个 HKMP 失败。

本目录已包含 `HKMP.dll` 和 `BouncyCastle.Cryptography.dll`，请一并安装。

## 安装方法

将本目录中 `HkmpPouch.dll`、`HKMP.dll` 和 `BouncyCastle.Cryptography.dll`
**三个文件一起**复制到以下游戏模组目录：

```
Steam/steamapps/common/Hollow Knight/hollow_knight_Data/Managed/Mods/HkmpPouch（网络库）/
```

> 注：`HKMP.dll` 是原版（未汉化），其显示名保持英文 "HKMP"。

## 说明

本 DLL 为**汉化版**，模组在游戏内模组列表中的显示名已翻译为中文。
汉化版本号标记为「汉化」后缀，便于识别。如遇兼容性问题，请备份原版 DLL 后替换。

## 原始 README

# HkmpPouch（网络库）

汉化版DLL

## 原版 README（翻译）

# HkmpPouch（网络库）

汉化版DLL

## 原始说明的翻译

HKMP（多人联机）模组的数据存储与网络通信辅助库。