# -*- coding: utf-8 -*-
"""按需从私有仓库下载「额外护符槽位」及其相关模组的源码（不整仓 clone）。"""
import base64, json, os, subprocess, urllib.parse, urllib.request

REPO = "Moyxi/KDQSHH"
DST = r"D:\KDQSHH\ModsWork"
tok = subprocess.check_output(["gh", "auth", "token"]).decode().strip()
op = urllib.request.build_opener(urllib.request.ProxyHandler({}))


def api(path):
    req = urllib.request.Request(
        "https://api.github.com/" + path,
        headers={"Authorization": "Bearer " + tok,
                 "Accept": "application/vnd.github+json",
                 "User-Agent": "kd"})
    with op.open(req, timeout=180) as r:
        return json.load(r)


def walk(path):
    out = []
    for item in api("repos/%s/contents/%s" % (REPO, urllib.parse.quote(path))):
        if item["type"] == "dir":
            out += walk(item["path"])
        elif item["type"] == "file":
            out.append(item)
    return out


def fetch(src, dst):
    files = walk(src)
    print("=== %s (%d 个文件) ===" % (src, len(files)))
    for f in files:
        rel = f["path"][len(src) + 1:]
        target = os.path.join(dst, rel.replace("/", os.sep))
        os.makedirs(os.path.dirname(target), exist_ok=True)
        if f.get("content"):
            data = base64.b64decode(f["content"])
        else:
            blob = api("repos/%s/git/blobs/%s" % (REPO, f["sha"]))
            data = base64.b64decode(blob["content"])
        open(target, "wb").write(data)
        print("  %8d  %s" % (len(data), rel))


fetch("ModsWork/HollowKnight.ExtraNotchesForExtraCharms-main",
      os.path.join(DST, "HollowKnight.ExtraNotchesForExtraCharms-main"))
print("完成")
