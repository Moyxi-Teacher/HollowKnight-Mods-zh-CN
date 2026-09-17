# -*- coding: utf-8 -*-
"""字符串扫描所有 .assets（不含 .resS）提取震动资产名候选。只读。"""
import os, re, json, glob

DATA = "E:/steam/steamapps/common/Hollow Knight/hollow_knight_Data"
CLIPS = "E:/steam/steamapps/common/Hollow Knight/BepInEx/plugins/HollowKnightHaptics/clips"

# 由 clips 文件名派生的词根（拆 '_'、去短词），用于判断一个字符串是否"像震动资产名"
clip_words = set()
for f in os.listdir(CLIPS):
    if f.lower().endswith(".wav"):
        base = f[:-4].lower()
        for w in re.split(r"[_\-]+", base):
            if len(w) >= 4:
                clip_words.add(w)
# 额外语义词根（覆盖无 vibration 后缀的事件名，如 ShadowDash/Scream）
extra_roots = {"shadow","dash","scream","scre","rumble","small","large","cast","dive",
               "dream","focus","nail","quake","spike","leak","wind","charm","geo","soul",
               "void","knight","shade","wall","jump","land","hit","bump","hurt","impact",
               "break","blast","roar","explode","crystal","poison","acid","lava","ice","fire",
               "parry","block","strike","swipe","throw","double","hero","world","grimm","troupe",
               "white","lady","king","beast","spore","shaman","mage","hunter","super"}

allwords = clip_words | extra_roots

# 提取 >=4 连续 ASCII 的可读字符串
ascii_re = re.compile(rb"[\x20-\x7e]{4,60}")
# 时间戳/哈希/无意义（全数字、含路径分隔等）过滤
bad_re = re.compile(r"[\\/<>]")  # 路径
pure_num = re.compile(r"^\d+$")
pure_hex = re.compile(r"^[0-9a-fA-F]+$")

candidates = {}
files = sorted(glob.glob(os.path.join(DATA, "*.assets"))) + \
         [os.path.join(DATA, "globalgamemanagers")] + \
         [os.path.join(DATA, "globalgamemanagers.assets")]
# 加 level* 与 Resources 目录内 .assets
files += sorted(glob.glob(os.path.join(DATA, "level*", "*.assets")))
files += sorted(glob.glob(os.path.join(DATA, "Resources", "*.assets")))

def looks_vib(s):
    sl = s.lower()
    if bad_re.search(s) or pure_num.match(s) or pure_hex.match(s):
        return False
    # 直接等于某 clips 词根附近，或含词根
    for w in allwords:
        if w in sl:
            return True
    return False

for fp in files:
    if not os.path.exists(fp):
        continue
    try:
        data = open(fp, "rb").read()
    except Exception as e:
        print("读失败:", fp, e)
        continue
    for m in ascii_re.finditer(data):
        s = m.group().decode("ascii", "ignore")
        if looks_vib(s):
            candidates.setdefault(s, 0)
            candidates[s] += 1

# 过滤：长度合理，且不是纯英文单词噪声（保留含震动语境的）
result = sorted([s for s, c in candidates.items() if 4 <= len(s) <= 48])
print("候选字符串数:", len(result))
for s in result:
    print("  ", repr(s), candidates[s])

with open("D:/KDQSHH/TempTest/asset_name_candidates.json", "w", encoding="utf-8") as f:
    json.dump(result, f, ensure_ascii=False, indent=2)
print("\n写出 asset_name_candidates.json")
