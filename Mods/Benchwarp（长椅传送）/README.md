# HollowKnight.BenchwarpMod（长椅传送）

## 模组信息

| 项目 | 内容 |
|---|---|
| 原始名称 | HollowKnight.BenchwarpMod |
| 中文名称 | 长椅传送 |
| 模组类型 | 功能模组 |
| 模组版本 | 3.2.6.1汉化 |
| 模组作者 | homothety |

## 模组简介

在暂停界面创建可点击菜单，用于更改长椅重生点；也修复了与区域标题弹窗交互时可能卡死的问题。

## 安装方法

将本目录中的 `HollowKnight.BenchwarpMod.dll` 文件复制到以下游戏模组目录：

```
Steam/steamapps/common/Hollow Knight/hollow_knight_Data/Managed/Mods/
```

## 说明

本 DLL 为**汉化版**，模组在游戏内模组列表中的显示名已翻译为中文。
汉化版本号标记为「汉化」后缀，便于识别。如遇兼容性问题，请备份原版 DLL 后替换。

## 中文翻译

在暂停界面创建可点击菜单，用于更改当前长椅重生点。选择后使用传送按钮（或保存并退出）即可前往新长椅。默认情况下，只有安装本模组后访问过的长椅才能作为目的地。

Benchwarp 修复了一个软锁 Bug：玩家在区域标题弹窗出现前与长椅交互时可能导致游戏卡死。作为副作用，安装 Benchwarp 后，在区域标题弹窗期间长椅也可交互。

**选项**
- 仅传送：移除长椅选择菜单
- 解锁全部：在选单中启用所有长椅
- 显示房间名：在左下角显示游戏代码中当前房间的名称
- 最大显示房间数：若启用「显示房间名」，最多额外显示这么多已加载房间
- 使用房间名：长椅菜单使用游戏代码中的房间名而非描述性名称
- 启用部署：添加用于放置自定义长椅点的菜单
- 总是切换全部：默认显示所有长椅菜单
- 门传送：将长椅菜单替换为选择场景转换的菜单（区域⇒房间⇒门）。
	- 生效时，传送按钮只能传送到所选转换的目标，不会传送到重生点。
	- 翻转按钮用于切换到当前所选转换的（原版）目标。
- 启用热键：通过输入简短按键序列而非点击菜单来传送至长椅
- 覆盖本地化：当游戏语言为中文时，长椅名称与 UI 仍保留英文

**部署**
- 部署按钮：在当前位置放置一张长椅
- 设置：将你的重生/传送点改为已部署的长椅
- 近处样式：选择英雄靠近长椅时淡入的偏好长椅精灵
- 远处样式：选择英雄远离长椅时显示的偏好长椅精灵
	- 出于安全考虑，坐在长椅上时无法更改样式。
- 选项：
	- 冷却：为部署按钮添加 300 秒冷却
	- 不可交互：部署的长椅无法交互，只能通过传送使用（例如防止在 Boss 战中使用）
	- 禁止空中部署：游戏必须判定玩家处于着地状态才能部署
	- 允许在不安全房间部署：防止在不安全区域（包括梦境房间与竞技场试炼）使用部署。
	  - 该设置可从模组菜单访问，但正常使用不应启用。在不安全房间放置长椅可能导致菜单/死亡/传送后无法加载到该长椅。下方章节说明如何恢复存档。
	  - [已废弃] 减少预加载：游戏启动时仅预加载所选样式。自 3.0 版本起，该功能不再是设置项，默认仅预加载一张长椅。
	  - 无预加载：不预加载任何长椅样式。部署按钮改为创建重生标记。要部署长椅，玩家必须更改设置并重启游戏。
	  - 应用到全部：将所选样式应用到基础游戏长椅。需要重载场景才能生效。并非所有长椅都受支持。

**长椅热键**
- 每张长椅上方显示一个两位数字。暂停时输入该数字等效于点击对应长椅按钮，然后点击传送。热键仅执行长椅传送操作。

可通过输入特定字母组合使用便捷命令：
- **LB**：上一个长椅（Last Bench，等效于直接点击传送）
- **SB**：起始长椅（Start Bench，等效于点击设置起点，然后传送）
- **WD**：传送到已部署的长椅（Warp to Deployed，等效于点击设置，然后传送）
- **TM**：切换菜单（Toggle Menu，等效于在模组菜单中点击显示菜单）
- **DW**：门传送（Door Warp，等效于在设置菜单中点击门传送）
- **DB**：部署长椅（Deploy Bench，等效于在部署子菜单中点击部署）

在旧版 Benchwarp 中，热键通过输入与长椅名称关联的两字母序列实现。在模组菜单中启用「旧版热键」设置并切换模组或重启游戏即可重新启用该功能。

热键可通过修改模组全局设置文件（存档文件夹中的 Benchwarp.GlobalSettings.json）中的「hotkeyOverrides」字典来重映射。该字典将默认绑定映射到替代项。例如 `"hotkeyOverrides": {"SB": "WS"}` 将起始长椅命令重映射到热键「WS」。自定义热键必须恰好由两个大写字母组成。

某些房间可能不适合放置部署长椅，包括梦境区域、诸神堂竞技场和竞技场试炼。如果无法加载到已部署的长椅，可通过以下步骤恢复：
- 使用模组菜单禁用 Benchwarp（导航到 选项→模组→Benchwarp 选项→Benchwarp，设为关闭）。
- 重新加载存档文件。你现在应处于最近一次的永久重生点。
- 使用模组菜单重新启用 Benchwarp。
- 使用部署菜单销毁旧的部署长椅，或在安全位置部署新长椅。

**致谢**
homothety - 主要开发者
PimpasPimpinela - 热键支持
Serena - 开发了 UI（作为 DebugMod 的一部分）
Serena 与 5FiftySix6 - 开发了传送方法（作为本模组的一部分）
The Embraced One - 最初创建了菜单素材（作为 DebugMod 的一部分）

**许可证**
Benchwarp 基于 LGPL 许可。源代码与许可证可在 https://github.com/homothetyhk/HollowKnight.BenchwarpMod/ 找到。

**安装**
Benchwarp 可通过 Hollow Knight 模组社区制作的各种模组安装器（Scarab、Lumafly 等）安装。

## 原始 README

# HollowKnight.BenchwarpMod

Mod which creates a clickable menu on the pause screen to alter your current bench respawn point. Use warp button (or save and quit) after selection to travel to new bench. By default, only benches visited after installing the mod can be destinations.

Benchwarp fixes a bug where a the player can softlock by interacting with a bench before an area title popup occurs. As a side-effect, benches are interactable during the area title popup with Benchwarp.

Options
- Warp Only: removes bench selection menu
- Unlock All: enables all benches on the selection menu
- Show Room Name: displays the name of the current room from the game code in the bottom left
- Max Displayed Room Names: if `Show Room Name` is enabled, show up to this many additional loaded rooms
- Use Room Names: bench menu uses room names from the game code instead of descriptive names
- Enable Deploy: adds a menu to place a custom bench point
- Always Toggle All: all bench menus are displayed by default
- Doorwarp: replaces bench menu with a menu for selecting scene transitions (area=>room=>door). 
	- While active, the Warp button can only be used to warp to the selected transition, and will not warp to respawn.
	- The Flip button is used to change selection to the (vanilla) target of the currently selected transition.
- Enable Hotkeys: lets you warp to benches by typing a short key sequence rather than clicking the menu
- Override Localization: leaves bench names & UI in English when the game is in Chinese

Deploy
- Deploy button: places a bench at the current location
- Set: alters your respawn/warp point to be your currently deployed bench
- Near Style: select your preferred bench sprite which fades in when the hero is near the bench
- Far Style: select your preferred bench sprite which shows when the hero is away from the bench
	- For safety, styles cannot be changed while benched.
- Options:
	- Cooldown: adds a 300s cooldown to the deploy button
	- Noninteractive: deployed bench cannot be interacted with, except via warping (e.g. preventing use during boss fights)
	- No Mid-Air Deploy: the game must recognize the player as grounded in order to deploy
	- Allow Deploy in Unsafe Rooms: prevents use of deploy in unsafe areas, including Dream World rooms and Colosseum trials.
	  - This setting can be accessed from the mod menu, but should not be enabled in normal use. Placing a bench in an unsafe room can make it impossible to load into the bench after a menu/death/warp. See the section below for instructions on how to recover a save file in such circumstances.
	- [Obsolete] Reduce Preloads: when the game starts up, only the selected style is preloaded. To use other styles, the player must restart the game.
		- As of version 3.0, Reduce Preloads is no longer a setting, and only one bench is preloaded by default.
	- No preloads: no bench styles are preloaded. The deploy button instead will create a respawn marker. To deploy benches, the player must change the setting and restart the game.
	- Apply Style To All: applies the selected styles to base game benches. Requires a scene reload to take effect. Not all benches are supported.

Bench Hotkeys
- A small 2-digit number is displayed above each bench. Typing that number while paused is equivalent to clicking the corresponding bench button, and then the Warp button in Bench Warp mode. In other words, a code for an unselectable bench will result in warping to the current respawn. Hotkeys only perform bench warps; this holds even if the current setting is set to Door Warp mode.

There are also several convenient commands available by entering certain letter combinations:
- **LB**: **L**ast **B**ench (equivalent to just clicking Warp)
- **SB**: **S**tart **B**ench (equivalent to clicking Set Start, then Warp)
- **WD**: **W**arp to **D**eployed Bench (equivalent to clicking Set in the Deploy submenu, then Warp)
- **TM**: **T**oggle **M**enu (equivalent to clicking Show Menu in the Benchwarp submenu in the Mods menu)
- **DW**: **D**oor **W**arp (equivalent to clicking Door Warp in the Settings menu)
- **DB**: **D**eploy **B**ench (equivalent to clicking Deploy in the Deploy submenu)

In older versions of Benchwarp, hotkeys were implemented through entering a 2 letter sequence associated with the name of the bench. That feature can be reactivated by enabling the Legacy Hotkeys setting in the mod menu, and toggling the mod or restarting the game.

Hotkeys can be remapped by modifying the "hotkeyOverrides" dictionary in the
mod's global settings file (Benchwarp.GlobalSettings.json, in your saves folder). This dictionary
should map the default bindings to their replacements. For example, `"hotkeyOverrides": {"SB": "WS"}` remaps the Start Bench command to use the hotkey "WS".
Custom hotkeys must be composed of exactly two uppercase letters. 

Some rooms may not be safe to place a deployed bench, including dream areas, Godhome arenas, and Colosseum trials. If you are unable to load into a deployed bench, you can recover through the following steps:
- Disable Benchwarp using the mod menu. That is, navigate to Options->Mods->Benchwarp Options->Benchwarp and set to Off.
- Reload the save file. You should now be at your most recent permanent respawn point.
- Reenable Benchwarp using the mod menu.
- Use the deploy menu to destroy the old deployed bench, or to deploy a new bench in a safe location.

Credits
homothety - main developer
PimpasPimpinela - hotkey support!
Serena - developed the UI, as part of DebugMod
Serena and 5FiftySix6 - developed the Warp method, as part of this mod
The Embraced One - originally created the menu assets, as part of DebugMod

##  License

Benchwarp is licensed under LGPL. Source code and license can be found at https://github.com/homothetyhk/HollowKnight.BenchwarpMod/

## Installation

Benchwarp can be installed by any of the various modding installers (Scarab, Lumafly, etc) made by the Hollow Knight modding community.

## 原始说明的翻译

在长椅处添加传送选项，跳过游戏进程限制。