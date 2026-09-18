# -*- coding: utf-8 -*-
"""把本次三个模组的改动上传到 Moyxi/KDQSHH（Git Data API，单次提交）。

上传内容：
  · 源码：PaleCourt 2 个、ControllerFixes 2 个、QoL 28 个
  · 交付 DLL/PDB：Mods/ 下 QoL、ControllerFixes 的 dll+pdb，以及 PaleCourt.pdb
（PaleCourt.dll 体积 358 MB 超过 GitHub 单文件上限，改由 Release 附件承载，另行处理。）
"""
import base64, json, os, subprocess, sys, urllib.request

REPO = "Moyxi/KDQSHH"
BRANCH = "main"
tok = subprocess.check_output(["gh", "auth", "token"]).decode().strip()
op = urllib.request.build_opener(urllib.request.ProxyHandler({}))


def api(path, method="GET", payload=None):
    data = json.dumps(payload).encode() if payload is not None else None
    req = urllib.request.Request(
        "https://api.github.com/" + path, data=data, method=method,
        headers={"Authorization": "Bearer " + tok,
                 "Accept": "application/vnd.github+json",
                 "User-Agent": "kd",
                 "Content-Type": "application/json"})
    with op.open(req, timeout=180) as r:
        body = r.read()
    return json.loads(body) if body else {}


D = r"D:\KDQSHH\ModsWork"
UPLOADS = [
    # (仓库路径, 本地路径)
    ("ModsWork/PaleCourt-master/FiveKnights.cs", D + r"\PaleCourt-master\FiveKnights.cs"),
    ("ModsWork/PaleCourt-master/Misc/SaveModSettings.cs", D + r"\PaleCourt-master\Misc\SaveModSettings.cs"),
    ("ModsWork/ControllerFixes-HEAD/ControllerFixes/ControllerFixes.cs", D + r"\ControllerFixes-HEAD\ControllerFixes\ControllerFixes.cs"),
    ("ModsWork/ControllerFixes-HEAD/ControllerFixes/Settings/ModMenu.cs", D + r"\ControllerFixes-HEAD\ControllerFixes\Settings\ModMenu.cs"),
    ("Mods/Pale Court（苍白宫廷）/PaleCourt.pdb", r"E:\steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods\Pale Court（苍白宫廷）\PaleCourt.pdb"),
    ("Mods/QoL（生活质量）/QoL.dll", r"E:\steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods\QoL（生活质量）\QoL.dll"),
    ("Mods/QoL（生活质量）/QoL.pdb", r"E:\steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods\QoL（生活质量）\QoL.pdb"),
    ("Mods/ControllerFixes（控制器修正）/ControllerFixes.dll", r"E:\steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods\ControllerFixes（控制器修正）\ControllerFixes.dll"),
    ("Mods/ControllerFixes（控制器修正）/ControllerFixes.pdb", r"E:\steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods\ControllerFixes（控制器修正）\ControllerFixes.pdb"),
]
# QoL 源码整体覆盖（v4.9 版本）
QOL_SRC = D + r"\_qol_49\QoL"
QOL_ROOT = "ModsWork/QoL-main/QoL"
for dp, dns, fns in os.walk(QOL_SRC):
    dns[:] = [x for x in dns if x not in ("bin", "obj", ".git")]
    for f in fns:
        p = os.path.join(dp, f)
        rel = os.path.relpath(p, QOL_SRC).replace("\\", "/")
        UPLOADS.append((QOL_ROOT + "/" + rel, p))

print("待上传文件数:", len(UPLOADS))

# 1) 取当前 main 的 commit 与 tree
ref = api("repos/%s/git/ref/heads/%s" % (REPO, BRANCH))
base_commit = ref["object"]["sha"]
commit = api("repos/%s/git/commits/%s" % (REPO, base_commit))
base_tree = commit["tree"]["sha"]
print("base commit:", base_commit[:8], "tree:", base_tree[:8])

# 2) 逐个创建 blob
tree_entries = []
for repo_path, local_path in UPLOADS:
    raw = open(local_path, "rb").read()
    blob = api("repos/%s/git/blobs" % REPO, "POST", {
        "content": base64.b64encode(raw).decode(),
        "encoding": "base64",
    })
    tree_entries.append({
        "path": repo_path,
        "mode": "100755" if repo_path.endswith((".dll", ".pdb")) else "100644",
        "type": "blob",
        "sha": blob["sha"],
    })
    print("  blob %8d  %s" % (len(raw), repo_path))

# 3) 建树（以现有树为基础）
new_tree = api("repos/%s/git/trees" % REPO, "POST",
               {"base_tree": base_tree, "tree": tree_entries})
print("new tree:", new_tree["sha"][:8])

# 4) 提交
new_commit = api("repos/%s/git/commits" % REPO, "POST", {
    "message": "修复苍白宫廷菜单持久化、生活质量总开关显示、控制器修正新增总开关",
    "tree": new_tree["sha"],
    "parents": [base_commit],
})
print("new commit:", new_commit["sha"][:8])

# 5) 更新 main
api("repos/%s/git/refs/heads/%s" % (REPO, BRANCH), "PATCH",
    {"sha": new_commit["sha"], "force": False})
print("main 已更新 ->", new_commit["sha"])
