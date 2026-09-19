# ExtraNotchesForExtraCharms（额外护符槽位）

## 模组信息

| 项目 | 内容 |
|---|---|
| 原始名称 | ExtraNotchesForExtraCharms |
| 中文名称 | 额外护符槽位 |
| 模组类型 | 功能模组 |
| 模组版本 | 1.1.0.0汉化 |
| 模组作者 | 原作者未知 |

## 模组简介

本模组会按照玩家**已经解锁的额外护符数量**，为玩家增加护符槽位，使玩家能够更充分地利用其他模组新增的护符。

**加成规则：**

- 每解锁 **1 个**额外护符，增加 **5 个**护符槽位（默认值，可在模组菜单中调整为 1–10）
- **一个额外护符都没有解锁时，不增加任何槽位**
- 举例：
  - 只解锁「禅意护符」→ 增加 5 个槽位
  - 再解锁 1 个「苍白宫廷」的额外护符 → 增加 10 个槽位

槽位在以下 3 个时机会重新计算：

- 加载存档时，应用额外的槽位
- 保存存档（例如在长椅休息）时，重新检查应增加的槽位数
- 退出存档时，移除额外槽位

## 更新记录

### 1.1.0.0（2026-09-20）

原版算法是「每 N 个额外护符换 1 个护符槽位」（除法），默认 N=5。
按此算法，即使装了 5 个额外护符也只多出 1 个槽位，实际效果几乎不可见。

现改为「每个**已解锁**的额外护符增加 N 个槽位」（乘法），默认 N=5，
并且只统计玩家真正获得的护符，未解锁时完全不生效。

### 1.0.0.0 汉化版

显示名汉化为「额外护符槽位」，菜单中文化。

## 安装方法

将本目录中的 `ExtraNotchesForExtraCharms.dll` 与 `ExtraNotchesForExtraCharms.pdb` 复制到以下游戏模组目录：

```
Steam/steamapps/common/Hollow Knight/hollow_knight_Data/Managed/Mods/ExtraNotchesForExtraCharms（额外护符槽位）/
```

## 说明

本 DLL 为**汉化版**，模组在游戏内模组列表中的显示名已翻译为中文「额外护符槽位」。
汉化版本号标记为「汉化」后缀，便于识别。如遇兼容性问题，请备份原版 DLL 后替换。

## 游戏内菜单

本模组已加入游戏内模组选项菜单，可在其中进行以下设置：

- **模组总开关**：启用或禁用额外护符槽位功能。
- **每个护符增加的槽位数**：每解锁一个额外护符，增加的护符槽位数量（1–10，默认 5）。

## 依赖

- SFCore（通过其 `CharmHelper` 统计其他模组新增的护符）
- Satchel（仅编译期引用）

## 原始 README

# Extra Notches for Extra Charms
This mod adds charm notches based on the number of
charms added by other mods, allowing players to make
more extensive use of their charm mods.

By default, this mod adds 1 notch per 5 charms added
by other mods. This setting can be customized from the
mod menu (max 1 notch per charm, min 1 notch per 10
charms).

The mod resets your notch count in 3 cases:
- When you load a save, it applies extra notches
- When you save (ie. rest at a bench), it rechecks
how many notches to add
- When you exit a save, it removes the extra notches

## Special Thanks
MsGreen419 for the idea
