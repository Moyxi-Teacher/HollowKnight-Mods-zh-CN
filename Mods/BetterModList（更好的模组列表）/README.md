# BetterModList（更好的模组列表）

## 模组信息

| 项目 | 内容 |
|---|---|
| 原始名称 | BetterModList |
| 中文名称 | 更好的模组列表 |
| 模组类型 | 功能模组 |
| 模组版本 | 1.0.0.0汉化 |
| 模组作者 | 原作者未知 |

## 模组简介

将模组列表分为两半，优化长列表显示效果。支持调整列表分布方式、显示侧边、最大长度和字体大小。

## 安装方法

将本目录中的 `BetterModList.dll` 文件复制到以下游戏模组目录：

```
Steam/steamapps/common/Hollow Knight/hollow_knight_Data/Managed/Mods/
```

## 说明

本 DLL 为**汉化版**，模组在游戏内模组列表中的显示名已翻译为中文。
汉化版本号标记为「汉化」后缀，便于识别。如遇兼容性问题，请备份原版 DLL 后替换。

## 中文翻译

本模组允许你更改游戏菜单中模组列表的布局。不过出于管理目的，Modding API 版本号会始终显示在左上角。

BetterModList 的模组菜单包含以下选项：
- 你可以更改已列出模组的分布方式
	- 「填满至上限」会根据「最大长度」滑块截断列表，并将其余部分推到屏幕另一侧
	- 「平均分配」会在列表中间截断，并在远端放置等量的模组
	- 「禁用」将所有模组列在单侧
- 你可以选择列表显示在左侧还是右侧
	- 关闭分布并不会影响你将全部模组放到右侧的能力
	- 若你选择将列表分布到两侧，列表仍会从你选择的这一侧按字母顺序开始
- 你可以更改模组列表的字体大小（默认 13）

## 原始 README

# BetterModList
This mod allows you to make changes to the layout of the mod list seen in the menu. However, for moderation purposes, the Modding API version will always appear in the top left corner.

BetterModList has a mod menu with the following options:
- You can change how the listed mods are distributed
	- "Fill to max" will cut off the list based on the "Max length" slider and push the rest of the list onto the other side of the screen
	- "Even split" will cut off the list halfway through and put an equal amount on the far side
	- "Disabled" will list all mods on one side
- You can choose whether the list appears on the left or right side
	- Disabling distribution does not affect your ability to put everything on the right side
	- If you choose to distribute the list to both sides, the list will still start alphabetically on your chosen side
- You can change the font size of the mod list (13 is default)

## 原始说明的翻译

提供更友好的模组列表界面。