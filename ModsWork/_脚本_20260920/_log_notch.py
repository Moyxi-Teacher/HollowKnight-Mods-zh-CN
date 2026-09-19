# -*- coding: utf-8 -*-
"""把本次「额外护符槽位」修复追加到访问日志。"""
import io

TEXT = r"""
──────────────────────────────────────────────────────────────
【记录时间】2026-09-20 06:28  操作人：AI 汉化编译工程师
【操作类型】诊断并修复「额外护符槽位」（ExtraNotchesForExtraCharms）加成算法
【用户报告】额外护符槽位无效果
【诊断依据】只读检查了以下内容：
  · 游戏日志 C:\Users\...\LocalLow\Team Cherry\Hollow Knight\ModLog.txt
      772 行 Starting slots: 3、773 行 5 modded charms -> 1 extra charm notches
      → 模组本身一直在正常工作，只是默认比例下只加了 1 个槽位
  · 模组设置 ExtraNotchesForExtraCharms.GlobalSettings.json（charmsPerNotch=5，默认值，未改过）
  · 模组启用状态 ModdingApi.GlobalSettings.json（「额外护符槽位」= 开）
  · 侧证：日志中 [PaleCourtCharms] Detected FiveKnights (苍白宫廷), disabling mod
      ——「苍白宫廷护符」被其自身逻辑禁用（本体已含护符），属正常现象，与本问题无关
【E盘访问 - 仅读取】
  路径：E:\steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods\ExtraNotchesForExtraCharms（额外护符槽位）\
  目的：核对部署的 DLL 与仓库版本是否一致（结果：MD5 完全相同，9728 字节）
        以及备份改造前的 3 个文件
【E盘访问 - 写入（部署）】
  路径：同上
  内容：ExtraNotchesForExtraCharms.dll / .pdb / README.md —— 仅替换该模组自身的 3 个文件
  说明：未触碰游戏原版文件、其他模组目录、Managed 根目录下的任何文件
  备份：D:\KDQSHH\备份\ExtraNotchesForExtraCharms_改前_20260920_{dll,pdb,md}
        （旧 DLL MD5 e7618b7a57fb91e8d5f787110362f77a，9728 字节）
  复核：部署后读回比对，3 个文件全部一致
【网络操作 - 只读下载】Moyxi/KDQSHH
  · HkRefs 编译参考程序集 93 个（21.2 MB，供重新编译使用，未从游戏目录复制）
  · 源码 ModsWork/HollowKnight.ExtraNotchesForExtraCharms-main
  · 诊断参考：InfiniteNotches.cs、SFCore 的 CharmHelper.cs、ZenCharm.cs、
    PaleCourt 的 Amulets.cs / AwardCharms.cs / FiveKnights.cs
【修复内容】加成算法由「每 N 个护符换 1 个槽位」（除法）改为
  「每个已解锁的额外护符增加 N 个槽位」（乘法），默认 N=5；
  只统计玩家已解锁的额外护符（读 PlayerData 的 gotCharm_41、gotCharm_42 …… 字段），
  一个都没解锁时不加成；
  菜单项「每护符槽位数」改为「每个护符增加的槽位数」（1–10）；
  版本号 1.0.0.0 → 1.1.0.0（csproj 与 GetVersion 同步）
【编译】dotnet build -c Release 通过（0 错误 0 警告）
  产物 10240 字节，MD5 fadf881a01abe5f12e08b88d29ec070c
【网络操作 - 写入】Moyxi/KDQSHH 提交 c2252f6884fbe8d580500c4c35b26d14897b44e6
  源码 5 个（含补丁脚本 _patch_20260920.py）+ 交付件 3 个（dll / pdb / README.md）
  复核：远端 8 个文件与本地逐一对齐（8/8 一致）
【未触碰】游戏原版文件、其他模组目录、mods2、Mods1；未删除任何游戏文件
──────────────────────────────────────────────────────────────
"""

with io.open(r"D:\KDQSHH\访问日志.txt", "a", encoding="utf-8") as f:
    f.write(TEXT)
s = io.open(r"D:\KDQSHH\访问日志.txt", encoding="utf-8").read()
print("日志总行数", len(s.splitlines()))
