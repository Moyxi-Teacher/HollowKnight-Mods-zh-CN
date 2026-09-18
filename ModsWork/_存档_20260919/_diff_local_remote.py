# -*- coding: utf-8 -*-
"""对比本地工作区与远端仓库快照，找出「本地有、远端没有」的文件。

用途：清理工作区前，确认哪些文件删除后无法从 Moyxi/KDQSHH 恢复。
只读操作，不修改任何文件。
"""
import hashlib, io, json, os

ROOT = r"D:\KDQSHH\ModsWork"
TREE = os.path.join(ROOT, "_tree_now.json")
OUT = os.path.join(ROOT, "_diff_local_remote.txt")
PREFIX = "ModsWork/"


def blob_sha(path):
    h = hashlib.sha1()
    size = os.path.getsize(path)
    h.update(b"blob %d\0" % size)
    with open(path, "rb") as f:
        while True:
            chunk = f.read(1 << 20)
            if not chunk:
                break
            h.update(chunk)
    return h.hexdigest()


data = json.load(io.open(TREE, encoding="utf-8"))
entries = data["tree"] if isinstance(data, dict) else data
remote = {}
for e in entries:
    if e["type"] == "blob" and e["path"].startswith(PREFIX):
        remote[e["path"][len(PREFIX):]] = e["sha"]

only_local, differ, same = [], [], 0
for dirpath, dirnames, filenames in os.walk(ROOT):
    rel_dir = os.path.relpath(dirpath, ROOT)
    if rel_dir == ".":
        rel_dir = ""
    for fn in filenames:
        full = os.path.join(dirpath, fn)
        rel = (rel_dir + "\\" + fn) if rel_dir else fn
        rel_posix = rel.replace("\\", "/")
        r = remote.get(rel_posix)
        if r is None:
            only_local.append((rel_posix, os.path.getsize(full)))
        elif blob_sha(full) != r:
            differ.append((rel_posix, os.path.getsize(full)))
        else:
            same += 1

lines = []
lines.append("远端仓库快照中 ModsWork/ 下的文件数：%d" % len(remote))
lines.append("本地一致：%d 个" % same)
lines.append("")
lines.append("=== 远端没有（本地独有，删除后只能靠本地）共 %d 个 ===" % len(only_local))
for p, s in sorted(only_local, key=lambda x: -x[1]):
    lines.append("  %9d  %s" % (s, p))
lines.append("")
lines.append("=== 与远端不一致（本地较新或较旧）共 %d 个 ===" % len(differ))
for p, s in sorted(differ, key=lambda x: -x[1]):
    lines.append("  %9d  %s" % (s, p))

io.open(OUT, "w", encoding="utf-8").write("\n".join(lines) + "\n")
print("独有 %d，不一致 %d，一致 %d" % (len(only_local), len(differ), same))
