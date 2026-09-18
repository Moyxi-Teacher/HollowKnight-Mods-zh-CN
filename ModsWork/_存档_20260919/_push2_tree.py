# -*- coding: utf-8 -*-
"""用仓库文件树快照重建 tree 对象（HKVocal.dll 超过 REST API 上限，改走 git push）。

步骤：读取 _tree.json 的全量快照 -> 替换 11 个文件的 blob SHA ->
自底向上 git mktree --missing -> 得到根 tree。
"""
import io, json, os, subprocess, sys

WORK = r"D:\KDQSHH\ModsWork"
PUSH = os.path.join(WORK, "_push2")
GAME = r"E:\steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods"
TREE = os.path.join(WORK, "_tree.json")
OUT = os.path.join(WORK, "_push2_tree.txt")

FILES = [
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

log = []
data = json.load(io.open(TREE, encoding="utf-8"))
entries = data["tree"] if isinstance(data, dict) else data
log.append("快照条目数：%d" % len(entries))

# 路径 -> (模式, blob sha)；模式必须保留，否则 tree SHA 与远端不一致
blobs = {}
for e in entries:
    if e["type"] != "blob":
        continue
    blobs[e["path"]] = (e.get("mode", "100644"), e["sha"])
log.append("快照中的文件数：%d" % len(blobs))


def run(args, stdin_bytes=None):
    p = subprocess.run(["git"] + args, cwd=PUSH, input=stdin_bytes,
                       stdout=subprocess.PIPE, stderr=subprocess.PIPE)
    if p.returncode != 0:
        raise RuntimeError("git %s 失败：%s" % (" ".join(args),
                                            p.stderr.decode("utf-8", "replace")))
    return p.stdout.decode("utf-8").strip()


# 写入新 blob，并替换 SHA
log.append("")
log.append("=== 写入本地 blob ===")
for local, repo_path in FILES:
    if not os.path.exists(local):
        log.append("!! 本地缺失：%s" % local)
        continue
    sha = run(["hash-object", "-w", "--", local])
    mode = blobs.get(repo_path, ("100644",))[0]
    blobs[repo_path] = (mode, sha)
    log.append("  %s  %s" % (sha[:12], repo_path))

# 建立目录树结构
root = {}
for path, val in blobs.items():
    parts = path.split("/")
    node = root
    for p in parts[:-1]:
        node = node.setdefault(p, {})
    node[parts[-1]] = val


def build(node, prefix):
    """自底向上生成 tree，返回 tree sha。"""
    lines = []
    for name, val in node.items():
        if isinstance(val, dict):
            sub = build(val, prefix + name + "/")
            lines.append("040000 tree %s\t%s\n" % (sub, name))
        else:
            lines.append("%s blob %s\t%s\n" % (val[0], val[1], name))
    payload = "".join(lines).encode("utf-8")
    return run(["mktree", "--missing"], payload)


log.append("")
log.append("=== 生成 tree ===")
root_sha = build(root, "")
log.append("根 tree = %s" % root_sha)

io.open(OUT, "w", encoding="utf-8").write("\n".join(log) + "\n")
io.open(os.path.join(WORK, "_push2_roottree.txt"), "w", encoding="utf-8").write(root_sha)
print("root tree = %s" % root_sha)
