# -*- coding: utf-8 -*-
"""取回「无限护符槽」源码与本模组的交付说明，供对比诊断。"""
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
    data = base64.b64decode(info["content"]) if info.get("content") else \
        base64.b64decode(json.load(op.open(urllib.request.Request(
            info["git_url"], headers={"Authorization": "Bearer " + tok,
                                      "User-Agent": "kd"})))["content"])
    os.makedirs(os.path.dirname(local), exist_ok=True)
    open(local, "wb").write(data)
    print("  %8d  %s" % (len(data), path))


get_file("ModsWork/InfiniteNotches-main/InfiniteNotches/InfiniteNotches.cs",
         os.path.join(DST, "InfiniteNotches-main", "InfiniteNotches", "InfiniteNotches.cs"))
get_file("ModsWork/InfiniteNotches-main/InfiniteNotches/Localization.cs",
         os.path.join(DST, "InfiniteNotches-main", "InfiniteNotches", "Localization.cs"))
get_file("Mods/ExtraNotchesForExtraCharms（额外护符槽位）/README.md",
         os.path.join(DST, "_deliver_readme", "ExtraNotchesForExtraCharms.md"))
print("完成")
