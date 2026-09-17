# 标定原版 SDF 的数值约定：value = clamp(edge + slope * 有符号距离, lo, hi)
import numpy as np, sys
sys.path.insert(0, r"D:\KDQSHH\字体资产工作")
import tools
from scipy import ndimage


def patch_of(arr, recs, cp, pad=4):
    g = tools.glyph_of(recs, cp)
    if g is None:
        return None
    x, y, w, h = int(g["x"]), int(g["y"]), int(np.ceil(g["w"])), int(np.ceil(g["h"]))
    x0, y0 = max(0, x - pad), max(0, y - pad)
    x1, y1 = min(arr.shape[1], x + w + pad), min(arr.shape[0], y + h + pad)
    return arr[y0:y1, x0:x1].astype(np.float64)


def fit(arr, recs, codepoints, thresh):
    vals, dists = [], []
    for cp in codepoints:
        p = patch_of(arr, recs, cp)
        if p is None or p.size < 20:
            continue
        inside = p >= thresh
        if inside.sum() == 0 or inside.sum() == p.size:
            continue
        d_in = ndimage.distance_transform_edt(inside)
        d_out = ndimage.distance_transform_edt(~inside)
        signed = d_in - d_out          # >0 内部, <0 外部
        vals.append(p.ravel())
        dists.append(signed.ravel())
    if not vals:
        return None
    v = np.concatenate(vals)
    d = np.concatenate(dists)
    # 只在过渡带附近拟合（|d| <= 4），更贴近边缘
    m = np.abs(d) <= 4
    A = np.vstack([np.ones(m.sum()), d[m]]).T
    coef, res, *_ = np.linalg.lstsq(A, v[m], rcond=None)
    pred = A @ coef
    rmse = float(np.sqrt(np.mean((pred - v[m]) ** 2)))
    return coef[0], coef[1], rmse, float(np.percentile(v, 1)), float(np.percentile(v, 99))


if __name__ == "__main__":
    cps = [0x4E00, 0x5341, 0x6C38, 0x4E2D, 0x6211, 0x5B57, 0x53E3, 0x65E5, 0x76EE, 0x7530, 0x5730, 0x4EBA]
    for key in ("chinese_body", "chinese_body_bold"):
        arr = tools.load_atlas(key)
        recs, _ = tools.load_glyphs(key)
        print(f"--- {key}")
        for thresh in (88, 100, 110, 128):
            r = fit(arr, recs, cps, thresh)
            if r:
                edge, slope, rmse, p1, p99 = r
                print(f"   阈值={thresh:3d} → 拟合 edge={edge:7.2f} slope={slope:6.2f} 每像素  RMSE={rmse:6.2f}  值域1%..99%={p1:.0f}..{p99:.0f}")
