# -*- coding: utf-8 -*-
"""按需从私有仓库 Moyxi/KDQSHH 下载指定源码目录（不整仓 clone）。"""
import base64, json, os, subprocess, sys, urllib.parse, urllib.request

REPO = "Moyxi/KDQSHH"
DST_ROOT = r"D:\KDQSHH\ModsWork"
tok = subprocess.check_output(["gh", "auth", "token"]).decode().strip()
op = urllib.request.build_opener(urllib.request.ProxyHandler({}))


def api(path):
    req = urllib.request.Request(
        "https://api.github.com/" + path,
        headers={"Authorization": "Bearer " + tok,
                 "Accept": "application/vnd.github+json",
                 "User-Agent": "kd"})
    with op.open(req, timeout=120) as r:
        return json.load(r)


def walk(path):
    out = []
    for item in api("repos/%s/contents/%s" % (REPO, urllib.parse.quote(path))):
        if item["type"] == "dir":
            out += walk(item["path"])
        elif item["type"] == "file":
            out.append(item)
    return out


def fetch(src_path, local_dir):
    files = walk(src_path)
    print("=== %s -> %s (%d 个文件) ===" % (src_path, local_dir, len(files)))
    total = 0
    for f in files:
        rel = f["path"][len(src_path) + 1:]
        target = os.path.join(local_dir, rel.replace("/", os.sep))
        os.makedirs(os.path.dirname(target), exist_ok=True)
        if f.get("content"):
            data = base64.b64decode(f["content"])
        else:
            blob = api("repos/%s/git/blobs/%s" % (REPO, f["sha"]))
            data = base64.b64decode(blob["content"])
        open(target, "wb").write(data)
        total += len(data)
    print("  共 %d 字节" % total)


if __name__ == "__main__":
    targets = [
        ("ModsWork/Enemy-HP-Bars-master", os.path.join(DST_ROOT, "Enemy-HP-Bars-master")),
        ("ModsWork/Konpanion-main", os.path.join(DST_ROOT, "Konpanion-main")),
        ("ModsWork/MylaFlower", os.path.join(DST_ROOT, "MylaFlower")),
    ]
    for src, dst in targets:
        try:
            fetch(src, dst)
        except Exception as e:
            print("!! %s 失败: %s" % (src, e))
