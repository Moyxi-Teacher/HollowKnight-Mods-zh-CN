# MagicUI（魔法界面）

**原名**：MagicUI
**作者**：BadMagic
**版本**：1.10.9277.8364汉化
**许可证**：MIT

## 汉化说明

MagicUI 是一个 Hollow Knight 用户界面样式库，为其它模组提供丰富的 UI
元素与样式。它自身不包含玩家可见的自然语言文本，也没有 `Language.json`
或语言钩子，从而不需要任何文本翻译。

## 变更记录

- 版本号固定为 `1.10.9277.8364`（原版 DLL 实为固定 1.10.9277.8364，
  源码仓库原有 `<AssemblyVersion>1.10.*</AssemblyVersion>` 自增；
  按用户决策以原版数字为准）；`InformationalVersion=1.10.9277.8364汉化`
  （菜单悬停显示）。
- 关闭强名称签名（`sgKey.snk` 保留在目录中但不再被引用），
  与原版 DLL（PublicKeyToken=null）一致。
- 重定向引用路径 `HollowKnightRefs` 至参考镜像 `$(HkRefs)`（作者原
  硬编码 `..\HKManaged\` 在本机不存在）。
- 禁用 `CopyMod` 构建后目标（`Condition="false"`），防止复制编译产物到
  游戏目录，保护只读游戏目录。
- 所有源码引用保持完整，未删除任何功能代码。

## 与原版一致性

| 项目 | 原版 | 汉化版 |
|------|------|--------|
| AssemblyVersion | 1.10.9277.8364 | 1.10.9277.8364 ✓ |
| PublicKeyToken | null | null ✓ |
| 内嵌资源 | 5 | 5 ✓ |
| 程序集引用（11） | ✓ | 逐项一致 ✓ |

## 原始信息

MagicUI 提供动态样式、元素模板与布局系统。
仓库：https://github.com/BadMagic1313/HollowKnight.MagicUI

## 原版 README

# MagicUI

A core mod for Hollow Knight that takes all the fuss of dealing with GameObjects behind the scenes, allowing mod authors to seamlessly create in-game UIs for their mod with an easy-to-use hierarchical layout system inspired by WPF.

## Features

* Layout & component system, with no GameObjects (but you can access them if you really want to)
* Several built-in elements and layouts for general use, such as: Stack layout, Images, Buttons, Text display, Text input fields
* API to create custom components and layouts to suit your needs
* Persistent and non-persistent layout types
* Ability to show UI elements only while the game is paused
* Utility to listen for specific hotkeys given modifier keys and an optional condition

## 原版 README（翻译）

# MagicUI（魔法界面）

Hollow Knight 的核心模组，在幕后处理所有与 GameObject 相关的繁琐工作，允许模组作者通过受 WPF 启发的易用层次布局系统，无缝地为其模组创建游戏内 UI。

## 功能特性

* 布局与组件系统，无需操作 GameObject（但你可以访问它们）
* 多个内置元素和布局供一般使用，如：堆栈布局、图像、按钮、文本显示、文本输入框
* API 用于创建自定义组件和布局
* 持久性和非持久性布局类型
* 仅在游戏暂停时显示 UI 元素的能力
* 实用工具，用于在给定修饰键和可选条件时监听特定热键

## 原始说明的翻译

Hollow Knight 的核心模组，为模组作者提供易用的游戏内 UI 创建系统。