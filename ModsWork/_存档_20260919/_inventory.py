# -*- coding: utf-8 -*-
"""只读盘点 D:\\KDQSHH\\ModsWork 工作区内容，供清理决策使用。"""
import io, os

ROOT = r"D:\KDQSHH\ModsWork"
OUT = r"D:\KDQSHH\ModsWork\_inventory.txt"

lines = []
total = 0
nfile = 0
ndir = 0

for name in sorted(os.listdir(ROOT)):
    p = os.path.join(ROOT, name)
    if os.path.isdir(p):
        s = 0
        f = 0
        for dirpath, dirnames, filenames in os.walk(p):
            for fn in filenames:
                try:
                    s += os.path.getsize(os.path.join(dirpath, fn))
                    f += 1
                except OSError:
                    pass
        ndir += 1
        total += s
        nfile += f
        lines.append("目录  %10.1f MB  %5d 个文件  %s" % (s / 1048576.0, f, name))
    else:
        sz = os.path.getsize(p)
        total += sz
        nfile += 1
        lines.append("文件  %10.1f MB            %s" % (sz / 1048576.0, name))

lines.append("")
lines.append("合计 %d 个文件、%d 个顶层目录，共 %.1f MB" % (nfile, ndir, total / 1048576.0))

# 统计空目录
empty = 0
for dirpath, dirnames, filenames in os.walk(ROOT):
    if not dirnames and not filenames:
        empty += 1
lines.append("其中空目录：%d 个" % empty)

io.open(OUT, "w", encoding="utf-8").write("\n".join(lines) + "\n")
print("ok")
