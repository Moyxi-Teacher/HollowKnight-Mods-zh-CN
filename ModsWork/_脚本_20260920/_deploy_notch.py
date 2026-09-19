# -*- coding: utf-8 -*-
"""备份并部署改造后的「额外护符槽位」到游戏模组目录。

备份：E 盘旧文件 -> D:\\KDQSHH\\备份\\（只从 E 盘读取）
部署：D 盘产物 -> E 盘模组目录（仅替换该模组自己的 3 个文件）
"""
import hashlib, io, os, shutil

GAME = r"E:\steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods\ExtraNotchesForExtraCharms（额外护符槽位）"
BACKUP = r"D:\KDQSHH\备份"
BUILT = r"D:\KDQSHH\ModsWork\HollowKnight.ExtraNotchesForExtraCharms-main\ExtraNotchesForExtraCharms\bin\Release\net472"
README_SRC = r"D:\KDQSHH\ModsWork\_deliver_readme\ExtraNotchesForExtraCharms.md"

LOG = io.open(r"D:\KDQSHH\ModsWork\_deploy_notch.log", "w", encoding="utf-8")


def md5(p):
    return hashlib.md5(open(p, "rb").read()).hexdigest()


def log(m):
    LOG.write(m + "\n")
    LOG.flush()


os.makedirs(BACKUP, exist_ok=True)
log("=== 一、备份游戏内旧文件（只读 E 盘）===")
for name in ("ExtraNotchesForExtraCharms.dll", "ExtraNotchesForExtraCharms.pdb", "README.md"):
    src = os.path.join(GAME, name)
    if not os.path.exists(src):
        log("  !! 不存在：%s" % src)
        continue
    dst = os.path.join(BACKUP, "ExtraNotchesForExtraCharms_改前_20260920_" + name)
    shutil.copy2(src, dst)
    log("  %s\n      -> %s\n      MD5 %s  (%d 字节)" % (src, dst, md5(src), os.path.getsize(src)))

log("")
log("=== 二、部署新文件到游戏目录 ===")
PAIRS = [
    (os.path.join(BUILT, "ExtraNotchesForExtraCharms.dll"),
     os.path.join(GAME, "ExtraNotchesForExtraCharms.dll")),
    (os.path.join(BUILT, "ExtraNotchesForExtraCharms.pdb"),
     os.path.join(GAME, "ExtraNotchesForExtraCharms.pdb")),
    (README_SRC, os.path.join(GAME, "README.md")),
]
for src, dst in PAIRS:
    shutil.copy2(src, dst)
    log("  %9d  %s  MD5 %s" % (os.path.getsize(dst), os.path.basename(dst), md5(dst)))

log("")
log("=== 三、复核（读回游戏目录）===")
ok = True
for src, dst in PAIRS:
    a, b = md5(src), md5(dst)
    same = a == b
    ok = ok and same
    log("  %-34s %s" % (os.path.basename(dst), "一致" if same else "不一致 %s != %s" % (a, b)))
log("")
log("全部一致：%s" % ok)
log("")
log("=== 游戏目录当前内容 ===")
for n in sorted(os.listdir(GAME)):
    p = os.path.join(GAME, n)
    log("  %9d  %s" % (os.path.getsize(p), n))

LOG.close()
print("done ok=%s" % ok)
