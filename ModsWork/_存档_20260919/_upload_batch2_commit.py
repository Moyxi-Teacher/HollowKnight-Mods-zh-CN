# -*- coding: utf-8 -*-
"""第二批同步（步骤 2）：创建 blobs -> 新 tree -> commit -> 更新 main。"""
import base64, io, json, os, subprocess

REPO = "Moyxi/KDQSHH"
WORK = r"D:\KDQSHH\ModsWork"
LOG_F = io.open(os.path.join(WORK, "_upload_batch2.log"), "a", encoding="utf-8")


def log(msg):
    LOG_F.write(msg + "\n")
    LOG_F.flush()


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
        raise RuntimeError("gh 失败: %s\n%s" % (" ".join(cmd),
                                                p.stderr.decode("utf-8", "replace")))
    if not stdout_path:
        return p.stdout


def post(path, payload_obj, out_file):
    """以 JSON 文件为请求体做 POST，并把响应写入 out_file 后解析。"""
    pf = os.path.join(WORK, "_tmp_payload.json")
    with open(pf, "w", encoding="utf-8") as f:
        json.dump(payload_obj, f, ensure_ascii=False)
    gh(["--method", "POST", path], input_path=pf, stdout_path=out_file)
    return json.load(open(out_file, encoding="utf-8"))


TMPOUT = os.path.join(WORK, "_tmp_resp.json")
plan = json.load(open(os.path.join(WORK, "_upload_batch2_plan.json"), encoding="utf-8"))
head, base_tree = plan["head"], plan["base_tree"]
log("")
log("=== 创建 blobs（共 %d 个）===" % len(plan["todo"]))

entries = []
for local, repo_path, sha in plan["todo"]:
    data = open(local, "rb").read()
    b64 = base64.b64encode(data).decode("ascii")
    del data
    r = post("repos/%s/git/blobs" % REPO,
             {"encoding": "base64", "content": b64}, TMPOUT)
    del b64
    assert r["sha"] == sha, "blob SHA 不一致: %s" % repo_path
    entries.append({"path": repo_path, "mode": "100644", "type": "blob", "sha": sha})
    log("  [ok] %s  (%s)" % (repo_path, sha[:10]))

log("")
log("=== 创建 tree ===")
tree = post("repos/%s/git/trees" % REPO,
            {"base_tree": base_tree, "tree": entries}, TMPOUT)
log("  tree = %s" % tree["sha"])

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
])

log("")
log("=== 创建 commit ===")
commit = post("repos/%s/git/commits" % REPO,
              {"message": MSG, "tree": tree["sha"], "parents": [head]}, TMPOUT)
log("  commit = %s" % commit["sha"])

log("")
log("=== 更新 main ===")
pf = os.path.join(WORK, "_tmp_payload.json")
with open(pf, "w", encoding="utf-8") as f:
    json.dump({"sha": commit["sha"], "force": False}, f)
import subprocess as sp
p = sp.run(["gh", "api", "--method", "PATCH", "repos/%s/git/refs/heads/main" % REPO,
            "--input", pf], capture_output=True)
if p.returncode != 0:
    raise RuntimeError(p.stderr.decode("utf-8", "replace"))
log("  main -> %s" % json.loads(p.stdout.decode("utf-8"))["object"]["sha"])

log("")
log("完成。提交：%s" % commit["sha"])
LOG_F.close()
print("OK commit=%s" % commit["sha"])
