# -*- coding: utf-8 -*-
"""修复因模组显示名汉化而失效的跨模组联动。

背景：这些模组用 ModHooks.GetMod("英文名") 判断另一个模组是否存在，
      而汉化时把对方的显示名改成了中文，导致查找失败、联动被关闭。
修法：在原判断后追加对汉化名的识别（新旧名都认）。
"""
import os

BASE = r"D:\KDQSHH\ModsWork"

EDITS = [
    # ---------- 1) 敌人血条 ↔ CustomKnight ----------
    (os.path.join(BASE, r"Enemy-HP-Bars-master\src\EnemyHPBar.cs"),
     '\t\tInstallCK = ModHooks.GetMod("CustomKnight") is Mod;',
     '\t\t// 修复(2026-09-19)：CustomKnight 汉化后显示名为「自定义小骑士」，\n'
     '\t\t// 原先按英文名查找会失败，导致与 CK 的联动被整体关闭。此处新旧名都识别。\n'
     '\t\tInstallCK = ModHooks.GetMod("CustomKnight") is Mod\n'
     '\t\t\t|| ModHooks.GetMod("自定义小骑士") is Mod;',
     "敌人血条：CK 联动检测"),

    # ---------- 2) 圣巢之声 ↔ 苍白宫廷 ----------
    (os.path.join(BASE, r"Hallownest-Vocalized-master\HKVocal\MajorFeatures\NPCDialogue.cs"),
     '        if (args.self.currentConversation == "SHEO_DREAM" && pcConvo == "SHEO_DREAM_PC_0" && ModHooks.GetMod("Pale Court") is Mod ) ',
     '        // 修复(2026-09-19)：苍白宫廷汉化后显示名为「苍白宫廷」，新旧名都识别。\n'
     '        if (args.self.currentConversation == "SHEO_DREAM" && pcConvo == "SHEO_DREAM_PC_0" && (ModHooks.GetMod("Pale Court") is Mod || ModHooks.GetMod("苍白宫廷") is Mod)) ',
     "圣巢之声：苍白宫廷检测"),

    # ---------- 3) 伴侣 ↔ HkmpPouch ----------
    (os.path.join(BASE, r"Konpanion-main\Konpanion.cs"),
     '            var hasPouch = ModHooks.GetMod("HkmpPouch") is Mod;',
     '            // 修复(2026-09-19)：HkmpPouch 汉化后显示名为「HkmpPouch（网络库）」，新旧名都识别。\n'
     '            var hasPouch = ModHooks.GetMod("HkmpPouch") is Mod\n'
     '                || ModHooks.GetMod("HkmpPouch（网络库）") is Mod;',
     "伴侣：HkmpPouch 检测"),

    # ---------- 4) 米拉花 ↔ 物品替换器（两处） ----------
    (os.path.join(BASE, r"MylaFlower\MylaFlower.cs"),
     '            if (ModHooks.GetMod("ItemChangerMod") is not null)',
     '            // 修复(2026-09-19)：物品替换器汉化后显示名为「物品替换器」，新旧名都识别。\n'
     '            if (ModHooks.GetMod("ItemChangerMod") is not null || ModHooks.GetMod("物品替换器") is not null)',
     "米拉花：物品替换器检测（主文件）"),

    (os.path.join(BASE, r"MylaFlower\RandoConnection\RandoInterop.cs"),
     '            if (ModHooks.GetMod("ItemChangerMod") is not null)',
     '            // 修复(2026-09-19)：物品替换器汉化后显示名为「物品替换器」，新旧名都识别。\n'
     '            if (ModHooks.GetMod("ItemChangerMod") is not null || ModHooks.GetMod("物品替换器") is not null)',
     "米拉花：物品替换器检测（随机化互操作）"),
]


def load(path):
    raw = open(path, "rb").read()
    bom = raw[:3] == b"\xef\xbb\xbf"
    s = raw.decode("utf-8-sig" if bom else "utf-8")
    return s.replace("\r\n", "\n"), bom, (s.count("\r\n") > 0)


def save(path, text, bom, crlf):
    if crlf:
        text = text.replace("\n", "\r\n")
    open(path, "wb").write((b"\xef\xbb\xbf" if bom else b"") + text.encode("utf-8"))


for path, old, new, tag in EDITS:
    s, bom, crlf = load(path)
    assert old in s, "锚点未找到: %s" % tag
    assert s.count(old) == 1, "锚点不唯一(%d): %s" % (s.count(old), tag)
    s = s.replace(old, new, 1)
    save(path, s, bom, crlf)
    print("  [ok] %s  (%s)" % (tag, os.path.basename(path)))

print("\n全部补丁已应用")
