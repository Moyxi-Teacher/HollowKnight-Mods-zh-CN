# -*- coding: utf-8 -*-
"""第二批同步：把 4 个跨模组联动修复的「源码 + DLL/PDB」提交到私有仓库。

采用 Git Data API（blobs -> tree -> commit -> 更新 ref），
不整仓 clone；先比对本地 blob SHA 与远端，只上传真正变化的文件。
"""
import base64, hashlib, io, json, os, subprocess, sys

REPO = "Moyxi/KDQSHH"
WORK = r"D:\KDQSHH\ModsWork"
GAME = r"E:\steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods"
LOG = os.path.join(WORK, "_upload_batch2.log")

LOG_F = io.open(LOG, "w", encoding="utf-8")


def log(msg):
    LOG_F.write(msg + "\n")
    LOG_F.flush()
    print(msg.encode("utf-8", "replace").decode("utf-8"))


def run(args, input_path=None):
    cmd = ["gh", "api"] + args
    if input_path:
        cmd += ["--input", input_path]
    p = subprocess.run(cmd, capture_output=True)
    if p.returncode != 0:
        raise RuntimeError("gh 调用失败: %s\n%s" % (" ".join(cmd),
                                                   p.stderr.decode("utf-8", "replace")))
    return p.stdout


def api_json(path):
    return json.loads(run([path]).decode("utf-8"))


def blob_sha(data):
    h = hashlib.sha1()
    h.update(b"blob %d\0" % len(data))
    h.update(data)
    return h.hexdigest()


# ---------- 1) 远端现状 ----------
ref = api_json("repos/%s/git/ref/heads/main" % REPO)
head = ref["object"]["sha"]
commit = api_json("repos/%s/git/commits/%s" % (REPO, head))
base_tree = commit["tree"]["sha"]
log("远端 main = %s" % head)
log("base_tree = %s" % base_tree)

remote = {}
for line in io.open(os.path.join(WORK, "_tree.json"), encoding="utf-8"):
    pass
tree_json = json.load(io.open(os.path.join(WORK, "_tree.json"), encoding="utf-8"))
for e in (tree_json["tree"] if isinstance(tree_json, dict) else tree_json):
    remote[e["path"]] = e

# ---------- 2) 待上传清单 ----------
FILES = [
    # 源码
    (os.path.join(WORK, r"Enemy-HP-Bars-master\src\EnemyHPBar.cs"),
     "ModsWork/Enemy-HP-Bars-master/src/EnemyHPBar.cs"),
    (os.path.join(WORK, r"Konpanion-main\Konpanion.cs"),
     "ModsWork/Konpanion-main/Konpanion.cs"),
    (os.path.join(WORK, r"MylaFlower\MylaFlower.cs"),
     "ModsWork/MylaFlower/MylaFlower.cs"),
    (os.path.join(WORK, r"MylaFlower\RandoConnection\RandoInterop.cs"),
     "ModsWork/MylaFlower/RandoConnection/RandoInterop.cs"),
    (os.path.join(WORK, r"Hallownest-Vocalized-master\HKVocal\MajorFeatures\NPCDialogue.cs"),
     "ModsWork/Hallownest-Vocalized-master/HKVocal/MajorFeatures/NPCDialogue.cs"),
    # 交付件
    (os.path.join(GAME, "Enemy HP Bar（敌人血条）", "EnemyHPBar.dll"),
     "Mods/Enemy HP Bar（敌人血条）/EnemyHPBar.dll"),
    (os.path.join(GAME, "Enemy HP Bar（敌人血条）", "EnemyHPBar.pdb"),
     "Mods/Enemy HP Bar（敌人血条）/EnemyHPBar.pdb"),
    (os.path.join(GAME, "Konpanion（伴侣）", "Konpanion.dll"),
     "Mods/Konpanion（伴侣）/Konpanion.dll"),
    (os.path.join(WORK, r"Konpanion-main\bin\Release\Konpanion.pdb"),
     "Mods/Konpanion（伴侣）/Konpanion.pdb"),
    (os.path.join(GAME, "MylaFlower（米拉花）", "MylaFlower.dll"),
     "Mods/MylaFlower（米拉花）/MylaFlower.dll"),
    (os.path.join(GAME, "MylaFlower（米拉花）", "MylaFlower.pdb"),
     "Mods/MylaFlower（米拉花）/MylaFlower.pdb"),
    (os.path.join(GAME, "Hallownest-Vocalized（圣巢之声）", "HKVocal.dll"),
     "Mods/Hallownest-Vocalized（圣巢之声）/HKVocal.dll"),
    (os.path.join(GAME, "Hallownest-Vocalized（圣巢之声）", "HKVocal.pdb"),
     "Mods/Hallownest-Vocalized（圣巢之声）/HKVocal.pdb"),
]

todo, skipped = [], []
for local, repo_path in FILES:
    if not os.path.exists(local):
        log("!! 本地缺失: %s" % local)
        continue
    data = open(local, "rb").read()
    sha = blob_sha(data)
    r = remote.get(repo_path)
    if r and r.get("sha") == sha:
        skipped.append(repo_path)
        log("  跳过(未变) %10d  %s" % (len(data), repo_path))
    else:
        todo.append((local, repo_path, data, sha))

log("")
log("需上传 %d 个文件，跳过 %d 个" % (len(todo), len(skipped)))
for local, repo_path, data, sha in todo:
    log("  %-12d %s" % (len(data), repo_path))
log("")

# 汇总信息写盘，供下一步使用
json.dump({"head": head, "base_tree": base_tree,
           "todo": [[l, r, s] for l, r, d, s in todo],
           "skipped": skipped},
          io.open(os.path.join(WORK, "_upload_batch2_plan.json"), "w", encoding="utf-8"),
          ensure_ascii=False, indent=1)
LOG_F.close()
