# -*- coding: utf-8 -*-
"""全量排查：模组的菜单设置能否持久化。

判定依据（Modding API 机制）：
  · 菜单项 Saver 只负责把值写进某处；
  · 真正的落盘靠 IGlobalSettings<T>（Profile 外）/ ILocalSettings<T>（存档内），
    API 会在合适时机调用 OnSaveGlobal()/OnSaveLocal()。
  · 模组若两者都没有，又不自己写文件，则退出游戏后设置必然丢失。

因此对每个模组：
  1) 是否有 IGlobalSettings / ILocalSettings
  2) 若无，是否自行落盘（出现 File.Write / StreamWriter / JsonConvert.Serialize 等）
  3) 若有菜单但以上都没有 → 判定为「设置不会保存」
"""
import os, re, subprocess, shutil

ROOT = r"E:\steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods"
TMP  = r"D:\重要文件\Downloads\temp\scan_all"
shutil.rmtree(TMP, ignore_errors=True)
os.makedirs(TMP, exist_ok=True)

WRITE_PAT = re.compile(r"File\.(Write|Append)|StreamWriter|JsonConvert\.Serialize|JsonSerializer|SerializeObject|File\.Create")
RESULT = []

for name in sorted(os.listdir(ROOT)):
    d = os.path.join(ROOT, name)
    if not os.path.isdir(d): continue
    dlls = [f for f in os.listdir(d) if f.lower().endswith(".dll")]
    if not dlls: continue
    p = os.path.join(d, dlls[0])
    out = os.path.join(TMP, re.sub(r"[^\w]", "_", name))
    try:
        subprocess.run(["ilspycmd", p, "-o", out], capture_output=True, text=True,
                       encoding="utf-8", errors="replace", timeout=240)
    except Exception:
        RESULT.append((name, "?", "?", "反编译超时")); continue

    texts = []
    if os.path.isdir(out):
        for dp, _, fns in os.walk(out):
            for f in fns:
                if f.endswith(".cs"):
                    try: texts.append(open(os.path.join(dp, f), encoding="utf-8", errors="replace").read())
                    except OSError: pass
    blob = "\n".join(texts)

    has_menu   = ("IMenuMod" in blob) or ("ICustomMenuMod" in blob)
    has_global = "IGlobalSettings" in blob
    has_local  = "ILocalSettings" in blob
    self_save  = bool(WRITE_PAT.search(blob))
    saver_cnt  = len(re.findall(r"Saver\s*=", blob))

    if not has_menu:
        verdict = "无菜单"
    elif has_global or has_local:
        verdict = "OK"
    elif self_save:
        verdict = "自行落盘(需人工确认)"
    elif saver_cnt == 0:
        verdict = "菜单无存储项"
    else:
        verdict = "*** 设置不会保存 ***"
    RESULT.append((name, ("G" if has_global else "-") + ("L" if has_local else "-"),
                   ("self" if self_save else "-"), verdict))

bad  = [r for r in RESULT if r[3].startswith("***")]
chk  = [r for r in RESULT if "人工确认" in r[3]]
print("扫描 %d 个模组" % len(RESULT))
print("\n=== 确认「设置不会保存」（有菜单+有Saver，但无任何持久化）===")
for n, f, s, v in bad: print("  %-40s %s" % (n[:38], f))
print("\n=== 需人工确认（自己写文件）===")
for n, f, s, v in chk: print("  %-40s %s" % (n[:38], f))
print("\n=== 汇总 ===")
print("  确认不保存 : %d 个" % len(bad))
print("  需人工确认 : %d 个" % len(chk))
