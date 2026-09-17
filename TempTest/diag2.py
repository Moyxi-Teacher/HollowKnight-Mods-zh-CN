# -*- coding: utf-8 -*-
import os, glob, struct
import UnityPy
DATA = "E:/steam/steamapps/common/Hollow Knight/hollow_knight_Data"

# 看一个 MonoBehaviour 的 raw 头部十六进制，确认布局
fp = os.path.join(DATA, "resources.assets")
env = UnityPy.load(fp)

mono_scripts = [o for o in env.objects if o.type.name == "MonoScript"]
mb = [o for o in env.objects if o.type.name == "MonoBehaviour"]
print("MonoScript:", len(mono_scripts), "MonoBehaviour:", len(mb))

# 打印前几个 MonoScript 的 typetree 和类名
print("\n--- MonoScript 样本 ---")
for o in mono_scripts[:5]:
    try:
        t = o.read_typetree()
        print(o.path_id, "name=", o.name, "|", {k: (str(v)[:40]) for k, v in list(t.items())[:6]})
    except Exception as e:
        print(o.path_id, "ERR", e)

# 看看 MonoScript 类名里有没有含 Gamepad/Vibration 的
print("\n--- 类名含 vibration/Gamepad 的 MonoScript ---")
hits = 0
for o in mono_scripts:
    try:
        t = o.read_typetree()
        for v in t.values():
            if "ibration" in str(v) or "Gamepad" in str(v):
                print(" ", t.get("m_ClassName"), t.get("m_Name")); hits += 1; break
    except Exception:
        pass
print("命中:", hits)

# 看第一个 MonoBehaviour raw 头部 hex
if mb:
    o = mb[0]
    raw = o.get_raw_data()
    print("\n--- MonoBehaviour raw 头部 hex(前 64B) ---")
    print(raw[:64].hex())
    import binascii
    print("尝试解析:")
    off = 0
    gf_id, gf_pid = struct.unpack_from("<iq", raw, off); off += 12
    enabled = raw[off]; off += 1; off += 3
    sc_id, sc_pid = struct.unpack_from("<iq", raw, off); off += 12
    nlen = struct.unpack_from("<i", raw, off)[0]; off += 4
    name = raw[off:off+nlen].decode("utf-8", "ignore")
    print(f" m_GameObject=({gf_id},{gf_pid}) enabled={enabled} m_Script=({sc_id},{sc_pid}) name={name!r}")
    print(f"  剩余字节数够: {len(raw)-off}")
