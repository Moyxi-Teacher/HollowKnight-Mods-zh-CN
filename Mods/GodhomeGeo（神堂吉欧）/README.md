# GodhomeGeo（神堂吉欧）

## 模组信息

| 项目 | 内容 |
|---|---|
| 原始名称 | GodhomeGeo |
| 中文名称 | 神堂吉欧 |
| 模组类型 | 功能模组 |
| 模组版本 | 1.0.0.0汉化 |
| 模组作者 | 原作者未知 |

## 模组简介

本模组允许玩家在圣巢宫（Godhome）内部获得吉欧。默认情况下，圣巢宫内部的敌人不会掉落吉欧，此模组修改了 `GeoControl` 的初始化逻辑，使吉欧能够在圣巢宫内部生成。

此外，该模组提供一个菜单选项，用于禁用非 Architect（建筑师）生成的敌人掉落吉欧，以还原原版行为。

## 安装方法

将本目录中的 `GodhomeGeo.dll` 文件复制到以下游戏模组目录：

```
Steam/steamapps/common/Hollow Knight/hollow_knight_Data/Managed/Mods/
```

## 说明

本 DLL 为**汉化版**，模组在游戏内模组列表中的显示名已翻译为中文「神堂吉欧」。汉化版本号标记为「汉化」后缀，便于识别。如遇兼容性问题，请备份原版 DLL 后替换。

## 中文翻译

本模组允许玩家在圣巢宫（Godhome）内部获得吉欧。默认情况下，圣巢宫内部的敌人不会掉落吉欧，此模组修改了 `GeoControl` 的初始化逻辑，使吉欧能够在圣巢宫内部生成。

此外，该模组提供一个菜单选项，用于禁用非 Architect（建筑师）生成的敌人掉落吉欧，以还原原版行为。

## 原始 README

# GodhomeGeo
This mod allows geo to spawn when inside of Godhome.

By default, enemies inside of Godhome do not drop geo. This mod hooks into `GeoControl.OnEnable` to remove the call that zeroes out geo drops, allowing geo to spawn normally.

It also provides an option to disable geo drops from enemies not spawned using Architect, to revert to vanilla behavior.

## Special Thanks
SFGrenade for the original mod concept

## 原始说明的翻译

在神堂中添加吉欧容器。