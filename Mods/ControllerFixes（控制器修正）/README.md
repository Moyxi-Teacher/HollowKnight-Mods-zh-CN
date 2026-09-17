# ControllerFixes（控制器修正）

## 模组信息

| 项目 | 内容 |
|---|---|
| 原始名称 | ControllerFixes |
| 中文名称 | 控制器修正 |
| 模组类型 | 功能模组 |
| 模组版本 | 1.0.2.0汉化 |
| 模组作者 | TheMulhima |

## 模组简介

修复手柄支持问题：控制器断开/重连检测、Mac 上 Xbox 手柄识别、以及按键提示切换等。

## 安装方法

将本目录中的 `ControllerFixes.dll` 文件复制到以下游戏模组目录：

```
Steam/steamapps/common/Hollow Knight/hollow_knight_Data/Managed/Mods/
```

## 说明

本 DLL 为**汉化版**，模组在游戏内模组列表中的显示名已翻译为中文。
汉化版本号标记为「汉化」后缀，便于识别。如遇兼容性问题，请备份原版 DLL 后替换。

## 中文翻译

这是 [Button Prompt Switcher](https://github.com/TheMulhima/Button-Prompt-Switcher) 的一个更新版本。

本模组包含：
- 从 Button Prompt Switcher 继承的功能。
    - 在模组菜单中：
        - 切换是否从专注（Focus）按钮/按键移除施放（cast）动作
        - 切换「设置→控制器」中的手柄皮肤
        - 切换强制使用 Xbox 手柄输入
        - 切换使用 Steam 任天堂手柄布局
    - 修复某些手柄断开/重连的检测
    - 修复某些特殊手柄的检测
    - 更多信息，请参见 Steam 上 [tintingaroo 的帖子](https://steamcommunity.com/sharedfiles/filedetails/?id=2525497719)。
- 修复某些手柄（例如 Xbox）在 Mac 上未被识别为输入设备的问题。
    - 安装本模组时，你必须将「设置→控制器→启用原生输入」设为开启。
    - Steam 上的相关讨论：
        - [冲刺 RT 无法使用](https://steamcommunity.com/app/367520/discussions/0/1621724915787605440/)
        - [M1 Mac 上的手柄支持](https://steamcommunity.com/app/367520/discussions/0/3108025660984901686/)

如果某些功能对你无效，请在 [Hollow Knight Discord 服务器](https://discord.com/invite/hollowknight) 的 #modding-help 板块发帖。

## 原始 README

# ControllerFixes

This is a newer version of [Button Prompt Switcher](https://github.com/TheMulhima/Button-Prompt-Switcher).

This mod includes:
- Features carried over from Button Prompt Switcher.
    - In the Mod menu:
        - Toggle removing the cast action from the Focus button/key
        - Toggle the controller skin in Settings -> Controller
        - Toggle forcing an Xbox controller input
        - Toggle using the Steam Nintendo controller layout
    - Fixes detection of certain controllers disconnecting/reconnecting
    - Fixes detection of certain exotic controllers
    - For more information, see [tintingaroo's post](https://steamcommunity.com/sharedfiles/filedetails/?id=2525497719) on Steam.
- Fixing certain controllers (such as Xbox) on Mac not being detected as input.
    - You must set Settings -> Controller -> Enable Native Input to ON while having this mod installed.
    - Related discussions on Steam:
        - [Dash RT isn't working](https://steamcommunity.com/app/367520/discussions/0/1621724915787605440/)
        - [Controller support on M1 Mac](https://steamcommunity.com/app/367520/discussions/0/3108025660984901686/)
 
If something is not working for you, please make a thread in #modding-help on the [Hollow Knight Discord server](https://discord.com/invite/hollowknight).

## 原始说明的翻译

修复控制器输入相关的问题。