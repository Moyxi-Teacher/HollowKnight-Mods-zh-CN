# -*- coding: utf-8 -*-
"""取回 SFCore 的 CharmHelper 与禅意护符源码，用于设计新的槽位算法。"""
import base64, json, os, subprocess, urllib.parse, urllib.request

REPO = "Moyxi/KDQSHH"
DST = r"D:\KDQSHH\ModsWork"
tok = subprocess.check_output(["gh", "auth", "token"]).decode().strip()
op = urllib.request.build_opener(urllib.request.ProxyHandler({}))


def get_file(path, local):
    req = urllib.request.Request(
        "https://api.github.com/repos/%s/contents/%s" % (REPO, urllib.parse.quote(path)),
        headers={"Authorization": "Bearer " + tok,
                 "Accept": "application/vnd.github+json",
                 "User-Agent": "kd"})
    with op.open(req, timeout=180) as r:
        info = json.load(r)
    data = base64.b64decode(info["content"])
    os.makedirs(os.path.dirname(local), exist_ok=True)
    open(local, "wb").write(data)
    print("  %8d  %s" % (len(data), path))


get_file("ModsWork/SFCore-master/src/CharmHelper.cs",
         os.path.join(DST, "SFCore-master", "src", "CharmHelper.cs"))
get_file("ModsWork/ZenCharm-master/ZenCharm.cs",
         os.path.join(DST, "ZenCharm-master", "ZenCharm.cs"))
print("完成")
