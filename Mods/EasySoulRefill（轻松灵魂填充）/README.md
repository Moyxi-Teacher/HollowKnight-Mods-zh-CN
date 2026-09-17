# EasySoulRefill（轻松灵魂填充）

## 模组信息

| 项目 | 内容 |
|---|---|
| 原始名称 | EasySoulRefill |
| 中文名称 | 轻松灵魂填充 |
| 模组类型 | 功能模组 |

## 模组简介

坐下休息和进入梦境 BOSS 房间时自动补满灵魂。

## 安装方法

将本目录中的 `EasySoulRefill.dll` 文件复制到以下游戏模组目录：

```
Steam/steamapps/common/Hollow Knight/hollow_knight_Data/Managed/Mods/
```

## 说明

本 DLL 为**汉化版**，模组在游戏内模组列表中的显示名已翻译为中文。
汉化版本号标记为「汉化」后缀，便于识别。如遇兼容性问题，请备份原版 DLL 后替换。

## 原版 README（翻译）

# EasySoulRefill

简化灵魂填充的过程，让玩家更容易获得灵魂用于释放法术。可以调整灵魂获取效率。

## 原始说明的翻译

自动填充灵魂，免除手动收集。
## 本次更新（HUD 灵魂容器显示错误修复）
原版存在已知缺陷：玩家持有灵魂容器时，重生后灵魂计量器 HUD 会显示错误
（原作者 issue #1「HUD issues」）。本次汉化版已合并社区修复方案
（`TakeReserveMP(0)`），在补满灵魂的同时刷新 HUD，实际不抽取任何灵魂。
修复后 `RefillSoul()` 逻辑为：`AddMPCharge(200)` → `TakeReserveMP(0)`。
