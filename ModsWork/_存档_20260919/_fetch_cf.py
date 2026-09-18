# -*- coding: utf-8 -*-
"""从用户私有仓库 Moyxi/KDQSHH 拉取 ControllerFixes-HEAD 源码目录。

只下载该目录，不做全仓 clone（仓库 1.35 GB，clone 不现实）。
"""
import base64, json, os, subprocess, sys, urllib.request

REPO = "Moyxi/KDQSHH"
SRC = "ModsWork/ControllerFixes-HEAD"
DST = r"D:\KDQSHH\ModsWork\ControllerFixes-HEAD"

token = subprocess.check_output(["gh", "auth", "token"]).decode().strip()
# 绕过环境里的代理设置
opener = urllib.request.build_opener(urllib.request.ProxyHandler({}))


def api(path):
    req = urllib.request.Request(
        "https://api.github.com/" + path,
        headers={
            "Authorization": "Bearer " + token,
            "Accept": "application/vnd.github+json",
            "User-Agent": "KDQSHH-agent",
        },
    )
    with opener.open(req, timeout=60) as r:
        return json.load(r)


def walk(path):
    out = []
    for item in api("repos/%s/contents/%s" % (REPO, path)):
        if item["type"] == "dir":
            out += walk(item["path"])
        elif item["type"] == "file":
            out.append(item)
    return out


files = walk(SRC)
print("files:", len(files))
total = 0
for f in files:
    rel = f["path"][len(SRC) + 1:]
    target = os.path.join(DST, rel.replace("/", os.sep))
    os.makedirs(os.path.dirname(target), exist_ok=True)
    if f.get("content"):
        data = base64.b64decode(f["content"])
    else:  # 大文件走 blob API
        blob = api("repos/%s/git/blobs/%s" % (REPO, f["sha"]))
        data = base64.b64decode(blob["content"])
    open(target, "wb").write(data)
    total += len(data)
    print("  %8d  %s" % (len(data), rel))
print("total bytes:", total)
