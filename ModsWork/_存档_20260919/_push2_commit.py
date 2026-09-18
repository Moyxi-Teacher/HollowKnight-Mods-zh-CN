# -*- coding: utf-8 -*-
"""第二批同步（步骤 3）：本地建 commit 并 push 到 main。

HKVocal.dll 约 104 MB，超过 REST API 的 blob 体积上限（返回 401），
因此改为直接 git push，只推送新增的对象。
"""
import io, os, subprocess

WORK = r"D:\KDQSHH\ModsWork"
PUSH = os.path.join(WORK, "_push2")
PARENT = "c6a36483cd30381b59010bed75b38b128d5a0fc2"
ROOT_TREE = io.open(os.path.join(WORK, "_push2_roottree.txt"), encoding="utf-8").read().strip()

MSG = "\n".join([
    "修复：跨模组联动因显示名汉化而失效（4 个模组）",
    "",
    "背景：模组显示名汉化后，ModHooks.GetMod(\"英文名\") 无法再匹配，",
    "导致依赖该检测的跨模组联动被整体关闭。修法为在原判断后追加对",
    "汉化显示名的识别（新旧名都接受），不改变任何原有逻辑分支。",
    "",
    "1) 敌人血条 EnemyHPBar：恢复与 CustomKnight（自定义小骑士）的联动。",
    "2) 伴侣 Konpanion：恢复与 HkmpPouch（网络库）的联动。",
    "3) 圣巢之声 HKVocal：恢复与苍白宫廷的联动（SHEO_DREAM 对话）。",
    "4) 米拉花 MylaFlower：恢复与物品替换器 ItemChangerMod 的联动（两处）。",
    "",
    "同步内容：上述 4 个模组的源码与交付 DLL/PDB。",
    "",
])


def git(args, **kw):
    p = subprocess.run(["git"] + args, cwd=PUSH, stdout=subprocess.PIPE,
                       stderr=subprocess.PIPE, **kw)
    if p.returncode != 0:
        raise RuntimeError("git %s 失败：%s" % (" ".join(args),
                                            p.stderr.decode("utf-8", "replace")))
    return p.stdout.decode("utf-8", "replace").strip()


git(["config", "user.name", "Moyxi"])
git(["config", "user.email", "245180983+Moyxi@users.noreply.github.com"])

sha = git(["commit-tree", ROOT_TREE, "-p", PARENT],
          input=MSG.encode("utf-8"))
print("新提交 = %s" % sha)

out = git(["push", "origin", "%s:refs/heads/main" % sha])
print(out)

io.open(os.path.join(WORK, "_push2_result.txt"), "w", encoding="utf-8").write(
    "commit=%s\nroot_tree=%s\nparent=%s\n" % (sha, ROOT_TREE, PARENT))
