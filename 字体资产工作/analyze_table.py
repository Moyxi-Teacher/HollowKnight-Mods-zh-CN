# 在字体资产的原始字节中精确定位字形表（记录为 36 字节：int id + 8 float）
import numpy as np, sys, struct
sys.path.insert(0, r"D:\KDQSHH\字体资产工作")
import tools

RAW = open(r"D:\KDQSHH\字体资产工作\原始备份\sharedassets0.assets.resS", "rb").read()


def atlas(key):
    i = tools.ATLAS_INFO[key]
    return np.frombuffer(RAW[i["offset"]:i["offset"] + i["size"]], dtype=np.uint8).reshape(i["dim"], i["dim"])


def records(data, off, cnt):
    if off + 36 * cnt > len(data):
        cnt = (len(data) - off) // 36
    return np.frombuffer(data, dtype=tools.GLYPH_DTYPE, count=cnt, offset=off)


def quality(arr, recs):
    good = tried = 0
    for r in recs:
        x, y, w, h = int(r["x"]), int(r["y"]), int(np.ceil(r["w"])), int(np.ceil(r["h"]))
        if x < 0 or y < 0 or w < 3 or h < 3 or x + w > arr.shape[1] or y + h > arr.shape[0]:
            continue
        cell = arr[y:y + h, x:x + w] >= 120
        tried += 1
        if cell.sum() == 0:
            continue
        touch = int(cell[0].any()) + int(cell[-1].any()) + int(cell[:, 0].any()) + int(cell[:, -1].any())
        if touch >= 3 and cell.mean() > 0.02:
            good += 1
    return good / max(1, tried), tried


def find(mb, key, lo, hi):
    arr = atlas(key)
    data = open(mb, "rb").read()
    best = None
    for off in range(lo, hi, 4):
        recs = records(data, off, 3000)
        q, t = quality(arr, recs[:600])
        if t < 400:
            continue
        if best is None or q > best[0]:
            best = (q, off)
    return best, arr, data


if __name__ == "__main__":
    for mb, key, lo, hi in (("mb_182.bin", "chinese_body", 3650, 3710),
                            ("mb_183.bin", "chinese_body_bold", 190, 270)):
        (q, off), arr, data = find(mb, key, lo, hi)
        # 推断记录数：从 off 起连续有效的记录
        recs = records(data, off, 5000)
        cnt = 0
        prev = -1
        for r in recs:
            x, y, w, h, adv = int(r["x"]), int(r["y"]), int(np.ceil(r["w"])), int(np.ceil(r["h"])), float(r["xa"])
            if x < 0 or y < 0 or w <= 0 or h <= 0 or x + w > arr.shape[1] or y + h > arr.shape[0] or adv <= 0:
                break
            if int(r["id"]) <= prev:
                break
            prev = int(r["id"])
            cnt += 1
        recs = recs[:cnt]
        print(f"{key}: 表偏移={off} 记录数={cnt} 合格率={q:.3f} 首=U+{int(recs[0]['id']):04X} 末=U+{int(recs[-1]['id']):04X}")
        print(f"    x范围=({recs['x'].min():.0f},{recs['x'].max():.0f}) y范围=({recs['y'].min():.0f},{recs['y'].max():.0f}) 典型advance={np.median(recs['xa']):.1f}")
        # 用「二」(上横短) 判定图集 y 方向
        for cp, nm in ((0x4E8C, "二"), (0x4E03, "七")):
            idx = np.where(recs["id"] == cp)[0]
            if len(idx) == 0:
                continue
            r = recs[idx[0]]
            x, y, w, h = int(r["x"]), int(r["y"]), int(np.ceil(r["w"])), int(np.ceil(r["h"]))
            cell = arr[y:y + h, x:x + w] >= 140
            ws = cell.sum(axis=1); k = max(1, h // 3)
            print(f"    {nm}: rect=({x},{y},{w},{h}) 上1/3墨宽={ws[:k].mean():.1f} 下1/3墨宽={ws[-k:].mean():.1f}")
