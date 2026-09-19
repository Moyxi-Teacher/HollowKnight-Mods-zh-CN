# -*- coding: utf-8 -*-
"""解析 ExtraNotchesForExtraCharms.csproj 的引用清单。"""
import io, os, re

P = r"D:\KDQSHH\ModsWork\HollowKnight.ExtraNotchesForExtraCharms-main\ExtraNotchesForExtraCharms\ExtraNotchesForExtraCharms.csproj"
OUT = r"D:\KDQSHH\ModsWork\_hintpaths.txt"
TOP = r"D:\KDQSHH\ModsWork\_csproj_head.txt"

s = io.open(P, encoding="utf-8", errors="replace").read()
lines = s.splitlines()

io.open(TOP, "w", encoding="utf-8").write("\n".join(lines[:30]))

paths = re.findall(r"<HintPath>\(([^)]*)\)</HintPath>", s)
paths = re.findall(r"<HintPath>(.*?)</HintPath>", s)
rel = []
for x in paths:
    x = x.strip()
    for pre in ("$(HollowKnightRefs)/", "$(HollowKnightRefs)\\"):
        if x.startswith(pre):
            x = x[len(pre):]
    rel.append(x.replace("\\", "/"))

io.open(OUT, "w", encoding="utf-8").write("\n".join(sorted(set(rel))))
print("引用数：%d（去重后 %d）" % (len(paths), len(set(rel))))
