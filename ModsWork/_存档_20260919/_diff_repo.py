# -*- coding: utf-8 -*-
"""本地目录 vs 仓库目录 的逐文件 Git 对象哈希对比（用已下载的 tree.json，不再访问网络）。"""
import hashlib, json, os

TREE = r"D:\KDQSHH\ModsWork\_tree.json"


def git_sha(data):
    h = hashlib.sha1()
    h.update(b"blob %d\0" % len(data))
    h.update(data)
    return h.hexdigest()


def remote_files(prefix):
    d = json.load(open(TREE, encoding="utf-8"))
    out = {}
    p = prefix.strip("/") + "/"
    for item in d["tree"]:
        if item["type"] == "blob" and item["path"].startswith(p):
            out[item["path"][len(p):]] = item["sha"]
    return out


def local_files(root):
    out = {}
    for dp, dns, fns in os.walk(root):
        dns[:] = [x for x in dns if x not in ("bin", "obj", ".git", ".vs")]
        for f in fns:
            fp = os.path.join(dp, f)
            rel = os.path.relpath(fp, root).replace("\\", "/")
            out[rel] = git_sha(open(fp, "rb").read())
    return out


def compare(prefix, root, label):
    r = remote_files(prefix)
    l = local_files(root)
    only_r = sorted(set(r) - set(l))
    only_l = sorted(set(l) - set(r))
    diff = sorted(k for k in set(r) & set(l) if r[k] != l[k])
    print("=== %s ===" % label)
    print("  仓库 %d / 本地 %d" % (len(r), len(l)))
    print("  仅仓库有 %d: %s" % (len(only_r), only_r[:8]))
    print("  仅本地有 %d: %s" % (len(only_l), only_l[:8]))
    print("  内容不同 %d: %s" % (len(diff), diff[:14]))
    return l


compare("ModsWork/PaleCourt-master", r"D:\KDQSHH\ModsWork\PaleCourt-master", "PaleCourt")
compare("ModsWork/ControllerFixes-HEAD", r"D:\KDQSHH\ModsWork\ControllerFixes-HEAD", "ControllerFixes")
compare("ModsWork/QoL-main/QoL", r"D:\KDQSHH\ModsWork\_qol_49\QoL", "QoL (v4.9 本地 vs QoL-main)")
