# -*- coding: utf-8 -*-
"""更新 Release 附件中的两个 PaleCourt.dll（358 MB，超过 GitHub 单文件上限，只能走 Release）。

流程：先删旧附件 → 再上传新附件（GitHub 不允许同名附件并存）。
本地源文件保留在 D 盘，万一上传失败可重试。
"""
import json, os, subprocess, sys, urllib.parse, urllib.request

REPO = "Moyxi/KDQSHH"
RELEASE_ID = 390581448
tok = subprocess.check_output(["gh", "auth", "token"]).decode().strip()
op = urllib.request.build_opener(urllib.request.ProxyHandler({}))

TARGETS = [
    ("03_Mods_PaleCourt.dll", 570152138,
     r"D:\KDQSHH\ModsWork\PaleCourt-master\bin\Publish\PaleCourt\PaleCourt.dll"),
    ("04_HkRefs_PaleCourt.dll", 570152145,
     r"D:\KDQSHH\ModsWork\HkRefs\Mods\PaleCourt\PaleCourt.dll"),
]


def api(path, method="GET"):
    req = urllib.request.Request(
        "https://api.github.com/" + path, method=method,
        headers={"Authorization": "Bearer " + tok,
                 "Accept": "application/vnd.github+json",
                 "User-Agent": "kd"})
    with op.open(req, timeout=180) as r:
        body = r.read()
    return json.loads(body) if body else {}


def upload(name, local_path):
    size = os.path.getsize(local_path)
    with open(local_path, "rb") as f:
        req = urllib.request.Request(
            "https://uploads.github.com/repos/%s/releases/%d/assets?name=%s"
            % (REPO, RELEASE_ID, urllib.parse.quote(name)),
            data=f, method="POST",
            headers={"Authorization": "Bearer " + tok,
                     "Accept": "application/vnd.github+json",
                     "Content-Type": "application/octet-stream",
                     "Content-Length": str(size),
                     "User-Agent": "kd"})
        with op.open(req, timeout=3600) as r:
            return json.loads(r.read())


for name, asset_id, local in TARGETS:
    if not os.path.exists(local):
        print("!! 本地文件缺失:", local)
        continue
    size = os.path.getsize(local)
    print("=== %s ===" % name)
    print("  删除旧附件 id=%d ..." % asset_id, end="", flush=True)
    try:
        api("repos/%s/releases/assets/%d" % (REPO, asset_id), "DELETE")
        print(" 已删除")
    except Exception as e:
        print(" 删除时异常（继续）:", e)
    print("  上传 %.1f MB ..." % (size / 1048576.0), end="", flush=True)
    res = upload(name, local)
    print(" 完成 -> id=%s size=%s" % (res.get("id"), res.get("size")))
print("全部完成")
