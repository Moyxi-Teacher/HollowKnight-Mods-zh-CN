# -*- coding: utf-8 -*-
"""列出清空工作区前需要归档的文件（本地独有 / 本地较新，且非编译产物、体积不大）。"""
import hashlib, io, json, os

ROOT = r"D:\KDQSHH\ModsWork"
TREE = os.path.join(ROOT, "_tree_now.json")
OUT = os.path.join(ROOT, "_archive_plan.txt")
PLAN = os.path.join(ROOT, "_archive_plan.json")
PREFIX = "ModsWork/"
MAXSIZE = 8 * 1024 * 1024

SKIP_DIRS = {"bin", "obj", ".vs", ".git"}
SKIP_TOP = {"_qol_build_out"}


def blob_sha(path):
    h = hashlib.sha1()
    h.update(b"blob %d\0" % os.path.getsize(path))
    with open(path, "rb") as f:
        for chunk in iter(lambda: f.read(1 << 20), b""):
            h.update(chunk)
    return h.hexdigest()


data = json.load(io.open(TREE, encoding="utf-8"))
entries = data["tree"] if isinstance(data, dict) else data
remote = {}
for e in entries:
    if e["type"] == "blob" and e["path"].startswith(PREFIX):
        remote[e["path"][len(PREFIX):]] = e["sha"]

todo, skip_big = [], []
for dirpath, dirnames, filenames in os.walk(ROOT):
    dirnames[:] = [d for d in dirnames if d not in SKIP_DIRS]
    rel_dir = os.path.relpath(dirpath, ROOT)
    rel_dir = "" if rel_dir == "." else rel_dir
    top = rel_dir.split(os.sep)[0] if rel_dir else ""
    if top in SKIP_TOP:
        continue
    for fn in filenames:
        full = os.path.join(dirpath, fn)
        rel = ((rel_dir + "/" + fn) if rel_dir else fn).replace("\\", "/")
        if rel in ("_tree.json", "_tree_now.json"):
            continue
        size = os.path.getsize(full)
        r = remote.get(rel)
        unchanged = r is not None and blob_sha(full) == r
        if unchanged:
            continue
        if size > MAXSIZE:
            skip_big.append((rel, size))
            continue
        todo.append((rel, size))

total = sum(s for _, s in todo)
lines = ["=== 待归档（本地独有或本地较新）共 %d 个，%.2f MB ===" % (len(todo), total / 1048576.0)]
for rel, size in sorted(todo, key=lambda x: (-x[1], x[0])):
    lines.append("  %9d  %s" % (size, rel))
lines.append("")
lines.append("=== 超过 8 MB 未归档（可重新生成或从别处恢复）共 %d 个，%.2f GB ===" % (
    len(skip_big), sum(s for _, s in skip_big) / 1073741824.0))
for rel, size in sorted(skip_big, key=lambda x: -x[1]):
    lines.append("  %9d  %s" % (size, rel))

io.open(OUT, "w", encoding="utf-8").write("\n".join(lines) + "\n")
json.dump([r for r, _ in todo], io.open(PLAN, "w", encoding="utf-8"),
          ensure_ascii=False, indent=1)
print("待归档 %d 个（%.2f MB），超限跳过 %d 个" % (len(todo), total / 1048576.0, len(skip_big)))
