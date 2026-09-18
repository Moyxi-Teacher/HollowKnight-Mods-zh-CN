# -*- coding: utf-8 -*-
"""从本地缓存的仓库文件树中查找指定模组的交付目录与文件。"""
import io, json, sys

TREE = r"D:\KDQSHH\ModsWork\_tree.json"
OUT = r"D:\KDQSHH\ModsWork\_tree_lookup.txt"

t = json.load(open(TREE, encoding="utf-8"))
tree = t["tree"] if isinstance(t, dict) else t

KEYS = ["Enemy HP Bar", "Konpanion", "MylaFlower", "Hallownest-Vocalized",
        "Enemy-HP-Bars", "Hallownest-Vocalized-master"]

lines = []
for e in tree:
    p = e["path"]
    if not p.startswith("Mods/"):
        continue
    if p.count("/") > 2:
        continue
    if any(k in p for k in ("Enemy HP Bar", "Konpanion", "MylaFlower", "Hallownest-Vocalized")):
        lines.append("%-8s %-60s %s" % (e.get("type"), p, e.get("size", "")))

with io.open(OUT, "w", encoding="utf-8") as f:
    f.write("\n".join(lines) + "\n")
print("wrote %d lines" % len(lines))
