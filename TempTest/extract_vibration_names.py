# -*- coding: utf-8 -*-
"""精确提取所有 GamepadVibration ScriptableObject 的资产名。
方法：手工解析 MonoBehaviour 原始头部（m_GameObject/m_Enabled/m_Script/m_Name），
绕开 .resS 流式数据导致的 typetree 失败。只读，绝不写 E 盘。"""
import os, glob, json, struct, sys
import UnityPy

DATA = "E:/steam/steamapps/common/Hollow Knight/hollow_knight_Data"

def parse_mono_header(raw):
    """返回 (script_fileID, script_pathID, name_str, name_end_off) 或 None。
    布局: m_GameObject PPtr(fileID i32 + pathID i64) | m_Enabled u8 + pad3 |
          m_Script PPtr(fileID i32 + pathID i64) | m_Name (i32 len + bytes, align4)"""
    try:
        off = 0
        gf_id, gf_pid = struct.unpack_from("<iq", raw, off); off += 12
        enabled = raw[off]; off += 1
        off = (off + 3) & ~3  # align4
        sc_id, sc_pid = struct.unpack_from("<iq", raw, off); off += 12
        nlen = struct.unpack_from("<i", raw, off)[0]; off += 4
        if nlen < 0 or nlen > 512:
            return None
        name = raw[off:off+nlen].decode("utf-8", "ignore")
        off += nlen
        off = (off + 3) & ~3
        return sc_id, sc_pid, name, off
    except Exception:
        return None

results = {}   # asset_name -> file
script_names = {}  # (file, pathID) -> class_name
files = sorted(glob.glob(os.path.join(DATA, "*.assets")))
files += [os.path.join(DATA, "globalgamemanagers.assets")]
files += [os.path.join(DATA, "globalgamemanagers")]
files += sorted(glob.glob(os.path.join(DATA, "level*", "*.assets")))

for fp in files:
    if not os.path.exists(fp):
        continue
    try:
        env = UnityPy.load(fp)
    except Exception as e:
        print("载入失败:", os.path.basename(fp), e)
        continue
    objs = list(env.objects)
    if not objs:
        continue

    # 1) 本文件内 MonoScript 类名表（fileID=0 即本文件）
    for o in objs:
        if o.type.name == "MonoScript":
            try:
                t = o.read_typetree()
                cn = t.get("m_ClassName") or t.get("m_Name")
                if cn:
                    script_names[(fp, o.path_id)] = cn
            except Exception:
                pass

    # 2) 遍历 MonoBehaviour，解析头，查 m_Script 类名
    for o in objs:
        if o.type.name != "MonoBehaviour":
            continue
        try:
            raw = o.get_raw_data()
        except Exception:
            continue
        h = parse_mono_header(raw)
        if not h:
            continue
        sc_id, sc_pid, name, _ = h
        if sc_id != 0:
            continue  # 跨文件引用，GamepadVibration 与其 MonoScript 同文件，跳过
        cn = script_names.get((fp, sc_pid))
        if cn == "GamepadVibration":
            results.setdefault(name, os.path.basename(fp))

print("==== 全部 GamepadVibration 资产 (%d) ====" % len(results))
for n in sorted(results):
    print("  ", n)

with open("D:/KDQSHH/TempTest/vibration_asset_names.json", "w", encoding="utf-8") as f:
    json.dump(sorted(results.keys()), f, ensure_ascii=False, indent=2)
print("已写 D:/KDQSHH/TempTest/vibration_asset_names.json")
