# -*- coding: utf-8 -*-
"""核对推送结果：远端 11 个文件的 blob SHA 是否与本地一致。"""
import hashlib, io, json, os, subprocess, urllib.parse, urllib.request

REPO = "Moyxi/KDQSHH"
WORK = r"D:\KDQSHH\ModsWork"
GAME = r"E:\steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods"
OUT = os.path.join(WORK, "_push2_verify.txt")

tok = subprocess.check_output(["gh", "auth", "token"]).decode().strip()
op = urllib.request.build_opener(urllib.request.ProxyHandler({}))


def api(path):
    req = urllib.request.Request(
        "https://api.github.com/" + path,
        headers={"Authorization": "Bearer " + tok,
                 "Accept": "application/vnd.github+json",
                 "User-Agent": "kd"})
    with op.open(req, timeout=300) as r:
        return json.load(r)


def blob_sha(data):
    h = hashlib.sha1()
    h.update(b"blob %d\0" % len(data))
    h.update(data)
    return h.hexdigest()


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

# 用本次推送的 commit 为基准列出文件树（只取需要的路径，避免大响应）
ref = api("repos/%s/git/ref/heads/main" % REPO)
head = ref["object"]["sha"]
commit = api("repos/%s/git/commits/%s" % (REPO, head))
tree_sha = commit["tree"]["sha"]

log = ["远端 main = %s" % head, "根 tree = %s" % tree_sha, ""]
log.append("远端提交信息首行：%s" % commit["message"].splitlines()[0])
log.append("")
log.append("%-6s %-12s %-12s %s" % ("结果", "远端", "本地", "路径"))

ok = 0
for local, repo_path in FILES:
    data = open(local, "rb").read()
    local_sha = blob_sha(data)
    url = "repos/%s/contents/%s?ref=%s" % (REPO, urllib.parse.quote(repo_path), head)
    info = api(url)
    same = info["sha"] == local_sha and info["size"] == len(data)
    ok += 1 if same else 0
    log.append("%-6s %-12s %-12s %s" % ("一致" if same else "不一致",
                                        info["sha"][:10], local_sha[:10], repo_path))

log.append("")
log.append("一致 %d / %d" % (ok, len(FILES)))
io.open(OUT, "w", encoding="utf-8").write("\n".join(log) + "\n")
print("done ok=%d/%d" % (ok, len(FILES)))
