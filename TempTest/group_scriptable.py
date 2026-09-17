# -*- coding: utf-8 -*-
"""精确定位 GamepadVibration 组：含已知震动名(ShadowDash/SmallRumble)的 ScriptableObject 组。只读。"""
import os, glob, struct, json
import UnityPy
DATA = "E:/steam/steamapps/common/Hollow Knight/hollow_knight_Data"

def parse_hdr(raw):
    try:
        off = 0
        gf_id, gf_pid = struct.unpack_from("<iq", raw, off); off += 12
        off += 1; off = (off + 3) & ~3
        sc_id, sc_pid = struct.unpack_from("<iq", raw, off); off += 12
        nlen = struct.unpack_from("<i", raw, off)[0]; off += 4
        if nlen < 0 or nlen > 512: return None
        name = raw[off:off+nlen].decode("utf-8", "ignore")
        return (gf_id, gf_pid, sc_id, sc_pid, name)
    except Exception:
        return None

groups = {}
files = sorted(glob.glob(os.path.join(DATA, "*.assets")))
files += [os.path.join(DATA, "globalgamemanagers.assets"), os.path.join(DATA, "globalgamemanagers")]
for fp in files:
    if not os.path.exists(fp): continue
    try: env = UnityPy.load(fp)
    except Exception: continue
    for o in env.objects:
        if o.type.name != "MonoBehaviour": continue
        try: raw = o.get_raw_data()
        except Exception: continue
        h = parse_hdr(raw)
        if not h: continue
        gf_id, gf_pid, sc_id, sc_pid, name = h
        if (gf_id, gf_pid) != (0, 0):
            continue
        groups.setdefault((sc_id, sc_pid), []).append(name)

# 选含 "ShadowDash" 的组（唯一确定为 GamepadVibration 的组）
target = None
for key, names in groups.items():
    if "ShadowDash" in names or "SmallRumble" in names:
        target = names

assert target, "未找到 GamepadVibration 组"
names = sorted(set(target))
print("GamepadVibration 组 m_Script:", [k for k,v in groups.items() if v is target])
print("成员数:", len(names))
for n in names:
    print("  ", n)
with open("D:/KDQSHH/TempTest/vibration_asset_names.json","w",encoding="utf-8") as f:
    json.dump(names, f, ensure_ascii=False, indent=2)
print("已写 vibration_asset_names.json (", len(names), "个)")
