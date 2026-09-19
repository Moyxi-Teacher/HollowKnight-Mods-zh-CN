# -*- coding: utf-8 -*-
"""按附录F规范，把阶段 105 追加到任务进度文件（只追加，不删改历史）。"""
import io

P = r"D:\KDQSHH\任务进度.txt"

TEXT = r"""
==============================================================
阶段 105  「额外护符槽位」加成算法改造（用户报告无效果）
【开始时间】2026-09-20 06:20
【完成时间】2026-09-20 06:30
【一、问题诊断】
  · 用户报告：额外护符槽位无效果
  · 排查结论：**模组没有损坏，部署也是正确的**，问题出在算法本身
      - 游戏日志证明模组一直在工作：
          772 行 Starting slots: 3
          773 行 5 modded charms -> 1 extra charm notches   （即 3 变成 4）
      - 游戏内 DLL 与仓库版本 MD5 完全一致（e7618b7a…，9728 字节）
      - 原算法是「每 N 个护符换 1 个槽位」（除法），默认 N=5，
        装了 5 个额外护符也只多 1 个槽位，实际效果几乎不可见
  · 侧证（与本问题无关）：「苍白宫廷护符」在日志中显示
      [PaleCourtCharms] Detected FiveKnights (苍白宫廷), disabling mod
    这是它自己的设计（本体已含护符，独立版自动让位），不是故障

【二、用户确认的新规则】
  · 默认「一个护符增加 5 个槽位」
  · 菜单同样改为「一个护符增加几个槽位」
  · 只在**解锁了额外护符**时才加成
      例：只解锁禅意护符 → +5 槽位；再解锁 1 个苍白宫廷护符 → +10 槽位

【三、改造内容】源码 ModsWork/HollowKnight.ExtraNotchesForExtraCharms-main
  · GetExtraNotches()：除法 → 乘法，且只统计已解锁的额外护符
  · 新增 CountUnlockedModdedCharms()：读取 PlayerData 的 gotCharm_41、gotCharm_42 ……
      （SFCore 约定：第 i 个额外护符编号为 41+i；
        苍白宫廷用钩子映射到自身存档、禅意护符直接用该字段，均已核实）
  · GlobalSettings：charmsPerNotch → notchesPerCharm（默认 5）
  · 菜单项：「每护符槽位数」→「每个护符增加的槽位数」（范围 1–10）
  · 版本号 1.0.0.0 → 1.1.0.0（csproj 与 GetVersion 同步）

【四、编译与部署】
  · 编译参考库 HkRefs 从仓库取回（93 个程序集，21.2 MB），未从游戏目录复制
  · dotnet build -c Release 通过（0 错误 0 警告）
  · 产物 10240 字节，MD5 fadf881a01abe5f12e08b88d29ec070c
  · 备份：D:\KDQSHH\备份\ExtraNotchesForExtraCharms_改前_20260920_{dll,pdb,md}
  · 部署至 E:\...\Managed\Mods\ExtraNotchesForExtraCharms（额外护符槽位）\

【五、同步至仓库】commit c2252f6884fbe8d580500c4c35b26d14897b44e6
  源码 5 个（含补丁脚本 _patch_20260920.py）+ 交付件 3 个，复核 8/8 一致

【待用户验证】请在游戏内读档后查看护符槽数量：
  若「禅意护符」已解锁，应比改动前多出 5 个槽位
【下一步】等待用户指示。
==============================================================
"""

raw = open(P, "rb").read()
bom = raw[:3] == b"\xef\xbb\xbf"
print("进度文件：%d 字节，BOM=%s" % (len(raw), bom))

old = raw.decode("utf-8-sig" if bom else "utf-8")
crlf = "\r\n" in old
new = old.rstrip("\r\n") + ("\r\n" if crlf else "\n") + TEXT.replace("\n", "\r\n" if crlf else "\n")
open(P, "wb").write((b"\xef\xbb\xbf" if bom else b"") + new.encode("utf-8"))

s = open(P, "rb").read().decode("utf-8-sig" if bom else "utf-8")
print("写入后：%d 行" % len(s.splitlines()))
print("阶段 105 已存在：%s" % ("阶段 105" in s))
