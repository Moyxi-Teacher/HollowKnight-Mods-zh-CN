# -*- coding: utf-8 -*-
import os, traceback
import UnityPy

DATA = "E:/steam/steamapps/common/Hollow Knight/hollow_knight_Data"
t = os.path.join(DATA, "resources.assets")
env = UnityPy.load(t)

ok = fail = mb = 0
sample_keys = set()
first_shown = 0
for obj in env.objects:
    if obj.type.name != "MonoBehaviour":
        continue
    mb += 1
    try:
        tree = obj.read_typetree()
        ok += 1
        if isinstance(tree, dict):
            ks = frozenset(tree.keys())
            sample_keys.update(ks)
            if ("smallMotor" in ks) or ("playbackRate" in ks):
                print("HIT:", tree.get("m_Name"), "->", list(ks)[:8])
    except Exception as e:
        fail += 1
        if first_shown < 3:
            print("ERR:", type(e).__name__, str(e)[:120])
            first_shown += 1

print(f"\nMonoBehaviour={mb} typetree成功={ok} 失败={fail}")
# 展示含 Motor/Curve 的字段名线索
cand = [k for k in sample_keys if any(w in str(k).lower() for w in ("motor","curve","playback","vib","highfidelity","lowfidelity"))]
print("疑似相关字段名:", sorted(cand)[:40])
