# GrassyKnight（割草骑士）

## 模组信息

| 项目 | 内容 |
|---|---|
| 原始名称 | GrassyKnight |
| 中文名称 | 割草骑士 |
| 模组类型 | 功能模组 |
| 模组版本 | 2.1.2.0汉化 |
| 模组作者 | 未知 |

## 模组简介

本模组追踪玩家割草进度，在游戏中显示草丛计数：当前房间的草丛数和整个圣巢的草丛总数。支持草丛罗盘（指向最近未割草丛）、自动割草、割草热键等功能。

## 游戏内菜单

本模组已加入游戏内模组选项菜单，可在其中进行以下设置：

- **模组总开关**：启用或禁用割草骑士效果。
- **自动割草**：是否自动挥剑割草。
- **指南针**：是否启用草之指南针，指向最近的未割草。
- **指南针热键修改**：修改用于在游戏中显示/隐藏草之指南针的按键。

## 安装方法

将本目录中的 `GrassyKnight.dll` 文件复制到以下游戏模组目录：

```
Steam/steamapps/common/Hollow Knight/hollow_knight_Data/Managed/Mods/
```

## 说明

本 DLL 为**汉化版**，模组在游戏内模组列表中的显示名已翻译为中文「割草骑士」。汉化版本号标记为「汉化」后缀，便于识别。如遇兼容性问题，请备份原版 DLL 后替换。

## 中文翻译

- 启用罗盘：你可以启用一个罗盘，始终指向当前房间内最近的未割草丛。注意：如果你想提交 Grass% 速通，应在练习时启用罗盘，正式游玩时关闭。
- 要启用罗盘，需要找到 `GrassyKnight.GlobalSettings.json` 文件（位于存档文件夹）。用文本编辑器打开，将 `DisableCompass` 改为 `false`（默认为 `true`）。
- 可通过空格键隐藏/显示罗盘。修改 `ToggleCompassHotkey` 可更改热键。

## 原始 README

# Grassy Knight

A Hollow Knight mod that tracks the grass you've cut. Your progress will appear in-game next to your geo count: one counter for grass within your current room and another for grass within all of Hollow Nest.

![](screenshot.png)

Grass will regrow but you only get credit the first time you cut it.

## Enabling the Compass

You can enable a compass that will always point you to the nearest uncut grass within the room you're in.

Note that if you're interested in submitting your speedrun of Grass % (where you cut all the grass in the game) you should only enable the compass during practice.

To enable the compass you'll need to find `GrassyKnight.GlobalSettings.json`. It is located in the same folder that your saved games are stored. Edit the file with a text editor (such as Notepad) and change the `DisableCompass` option to `false` (it is `true` by default).

For example, here's what your settings file might look like by default:

```json
{
  "UseHeuristicGrassKnower": false,
  "AutomaticallyCutGrass": false,
  "ToggleCompassHotkey": "Space",
  "DisableCompass": true
}
```

And here's what you would change it to in order to enable the compass:

```json
{
  "UseHeuristicGrassKnower": false,
  "AutomaticallyCutGrass": false,
  "ToggleCompassHotkey": "Space",
  "DisableCompass": false
}
```

With the compass enabled, you can hide/show it in-game by pressing the space bar.

If you'd like to change which hotkey hides/shows the compass, change the `ToggleCompassHotkey` option to something else. Check out [ALL_KEYBOARD_KEY_NAMES.txt](https://raw.githubusercontent.com/itsjohncs/GrassyKnight/main/ALL_KEYBOARD_KEY_NAMES.txt) for a list of all the possible values.

## 原始说明的翻译

割草骑士相关的自定义内容。