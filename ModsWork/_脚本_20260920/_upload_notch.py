# -*- coding: utf-8 -*-
"""把「额外护符槽位」的改造同步到私有仓库（源码 + 交付件）。"""
import base64, hashlib, io, json, os, subprocess

REPO = "Moyxi/KDQSHH"
WORK = r"D:\KDQSHH\ModsWork"
SRC = os.path.join(WORK, "HollowKnight.ExtraNotchesForExtraCharms-main")
GAME = r"E:\steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods\ExtraNotchesForExtraCharms（额外护符槽位）"
LOG = io.open(os.path.join(WORK, "_upload_notch.log"), "w", encoding="utf-8")
TMP = os.path.join(WORK, "_tmp_resp.json")
PF = os.path.join(WORK, "_tmp_payload.json")


def log(m):
    LOG.write(m + "\n")
    LOG.flush()


def gh(args, input_path=None, stdout_path=None):
    cmd = ["gh", "api"] + args
    if input_path:
        cmd += ["--input", input_path]
    if stdout_path:
        with open(stdout_path, "wb") as f:
            p = subprocess.run(cmd, stdout=f, stderr=subprocess.PIPE)
    else:
        p = subprocess.run(cmd, capture_output=True)
    if p.returncode != 0:
        raise RuntimeError("gh 失败：%s\n%s" % (" ".join(cmd),
                                               p.stderr.decode("utf-8", "replace")))
    if not stdout_path:
        return p.stdout


def post(path, obj):
    with open(PF, "w", encoding="utf-8") as f:
        json.dump(obj, f, ensure_ascii=False)
    gh(["--method", "POST", path], input_path=PF, stdout_path=TMP)
    return json.load(open(TMP, encoding="utf-8"))


def blob_sha(data):
    h = hashlib.sha1()
    h.update(b"blob %d\0" % len(data))
    h.update(data)
    return h.hexdigest()


FILES = [
    (os.path.join(SRC, "ExtraNotchesForExtraCharms", "ExtraNotchesForExtraCharms.cs"),
     "ModsWork/HollowKnight.ExtraNotchesForExtraCharms-main/ExtraNotchesForExtraCharms/ExtraNotchesForExtraCharms.cs"),
    (os.path.join(SRC, "ExtraNotchesForExtraCharms", "GlobalSettings.cs"),
     "ModsWork/HollowKnight.ExtraNotchesForExtraCharms-main/ExtraNotchesForExtraCharms/GlobalSettings.cs"),
    (os.path.join(SRC, "ExtraNotchesForExtraCharms", "ModMenu.cs"),
     "ModsWork/HollowKnight.ExtraNotchesForExtraCharms-main/ExtraNotchesForExtraCharms/ModMenu.cs"),
    (os.path.join(SRC, "ExtraNotchesForExtraCharms", "ExtraNotchesForExtraCharms.csproj"),
     "ModsWork/HollowKnight.ExtraNotchesForExtraCharms-main/ExtraNotchesForExtraCharms/ExtraNotchesForExtraCharms.csproj"),
    (os.path.join(WORK, "_fix_notch.py"),
     "ModsWork/HollowKnight.ExtraNotchesForExtraCharms-main/_patch_20260920.py"),
    (os.path.join(GAME, "ExtraNotchesForExtraCharms.dll"),
     "Mods/ExtraNotchesForExtraCharms（额外护符槽位）/ExtraNotchesForExtraCharms.dll"),
    (os.path.join(GAME, "ExtraNotchesForExtraCharms.pdb"),
     "Mods/ExtraNotchesForExtraCharms（额外护符槽位）/ExtraNotchesForExtraCharms.pdb"),
    (os.path.join(GAME, "README.md"),
     "Mods/ExtraNotchesForExtraCharms（额外护符槽位）/README.md"),
]

gh(["repos/%s/git/ref/heads/main" % REPO], stdout_path=TMP)
head = json.load(open(TMP, encoding="utf-8"))["object"]["sha"]
gh(["repos/%s/git/commits/%s" % (REPO, head)], stdout_path=TMP)
base_tree = json.load(open(TMP, encoding="utf-8"))["tree"]["sha"]
log("远端 main=%s" % head)
log("base_tree=%s" % base_tree)

gh(["repos/%s/git/trees/%s?recursive=1" % (REPO, head)], stdout_path=TMP)
remote = {e["path"]: e["sha"] for e in json.load(open(TMP, encoding="utf-8"))["tree"]
          if e["type"] == "blob"}

log("")
log("=== 创建 blobs ===")
entries = []
for local, repo_path in FILES:
    data = open(local, "rb").read()
    sha = blob_sha(data)
    if remote.get(repo_path) == sha:
        log("  跳过(未变) %8d  %s" % (len(data), repo_path))
        continue
    r = post("repos/%s/git/blobs" % REPO,
             {"encoding": "base64", "content": base64.b64encode(data).decode("ascii")})
    assert r["sha"] == sha
    entries.append({"path": repo_path, "mode": "100644", "type": "blob", "sha": sha})
    log("  %8d  %s" % (len(data), repo_path))

if not entries:
    log("没有变化，无需提交")
    LOG.close()
    raise SystemExit(0)

MSG = "\n".join([
    "额外护符槽位：改造加成算法（每个已解锁护符 +N 槽位）",
    "",
    "问题：原版算法是「每 N 个额外护符换 1 个槽位」（除法），默认 N=5，",
    "装了 5 个额外护符也只多 1 个槽位，实际效果几乎不可见。",
    "",
    "改为：",
    "· 只统计玩家**已解锁**的额外护符（读 PlayerData 的 gotCharm_41、42 ……）",
    "· 每解锁 1 个就增加 N 个槽位（乘法），默认 N=5",
    "· 一个都没解锁时不增加（模组不激活）",
    "· 菜单项由「每护符槽位数」改为「每个护符增加的槽位数」（1–10）",
    "· 版本号 1.0.0.0 → 1.1.0.0",
    "",
    "例：只解锁禅意护符 → +5 槽位；再解锁 1 个苍白宫廷护符 → +10 槽位。",
])

tree = post("repos/%s/git/trees" % REPO, {"base_tree": base_tree, "tree": entries})
commit = post("repos/%s/git/commits" % REPO,
              {"message": MSG, "tree": tree["sha"], "parents": [head]})
with open(PF, "w", encoding="utf-8") as f:
    json.dump({"sha": commit["sha"], "force": False}, f)
p = subprocess.run(["gh", "api", "--method", "PATCH",
                    "repos/%s/git/refs/heads/main" % REPO, "--input", PF],
                   capture_output=True)
if p.returncode != 0:
    raise RuntimeError(p.stderr.decode("utf-8", "replace"))
log("")
log("tree=%s" % tree["sha"])
log("commit=%s" % commit["sha"])
io.open(os.path.join(WORK, "_upload_notch_commit.txt"), "w", encoding="utf-8").write(
    commit["sha"] + "\n")
log("完成")
LOG.close()
print("commit=%s files=%d" % (commit["sha"], len(entries)))
