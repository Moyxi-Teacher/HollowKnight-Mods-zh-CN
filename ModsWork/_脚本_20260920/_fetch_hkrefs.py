# -*- coding: utf-8 -*-
"""从私有仓库取回编译所需的参考程序集（不触碰游戏目录）。"""
import io, json, os, subprocess, urllib.parse, urllib.request

REPO = "Moyxi/KDQSHH"
REF = "main"
DST = r"D:\KDQSHH\ModsWork\HkRefs"
LIST = r"D:\KDQSHH\ModsWork\_hintpaths.txt"

tok = subprocess.check_output(["gh", "auth", "token"]).decode().strip()
op = urllib.request.build_opener(urllib.request.ProxyHandler({}))

# 先取仓库树，建立路径 -> (sha, size)
req = urllib.request.Request(
    "https://api.github.com/repos/%s/git/trees/%s?recursive=1" % (REPO, REF),
    headers={"Authorization": "Bearer " + tok,
             "Accept": "application/vnd.github+json",
             "User-Agent": "kd"})
tree = json.load(op.open(req, timeout=300))["tree"]
remote = {e["path"]: e for e in tree if e["type"] == "blob"}

rels = [l.strip() for l in io.open(LIST, encoding="utf-8") if l.strip()]
print("需要 %d 个引用文件" % len(rels))

ok, miss, total = 0, [], 0
for rel in rels:
    rp = "ModsWork/HkRefs/" + rel
    e = remote.get(rp)
    local = os.path.join(DST, rel.replace("/", os.sep))
    if e is None:
        miss.append(rel)
        continue
    if os.path.exists(local) and os.path.getsize(local) == e.get("size"):
        ok += 1
        total += e["size"]
        continue
    os.makedirs(os.path.dirname(local), exist_ok=True)
    url = "https://api.github.com/repos/%s/git/blobs/%s" % (REPO, e["sha"])
    rq = urllib.request.Request(url, headers={
        "Authorization": "Bearer " + tok,
        "Accept": "application/vnd.github+json",
        "User-Agent": "kd"})
    import base64
    blob = json.load(op.open(rq, timeout=600))
    data = base64.b64decode(blob["content"])
    open(local, "wb").write(data)
    ok += 1
    total += len(data)
    print("  %9d  %s" % (len(data), rel))

print()
print("已就绪 %d / %d，共 %.1f MB" % (ok, len(rels), total / 1048576.0))
if miss:
    print("仓库中缺失 %d 个：" % len(miss))
    for m in miss:
        print("   ", m)
