# 用「游戏内导出的精确字形表」重制中文 TMP 字体图集（霞鹜臻楷）
# 输入：D:\KDQSHH\字体资产工作\导出\字形表_<资产名>.txt  与  图集_<资产名>.png
# 输出：直接覆盖 sharedassets0.assets.resS 中对应图集的像素（同偏移同长度）
import os, sys, glob, shutil
import numpy as np
from PIL import Image, ImageDraw, ImageFont

sys.path.insert(0, r"D:\KDQSHH\字体资产工作")
import tools

TTF = r"D:\KDQSHH\ModsWork\SimplifiedChineseFont-HEAD\霞鹜臻楷.ttf"
EXPORT = r"D:\KDQSHH\字体资产工作\导出"
SS = 4                      # 超采样倍数
TARGET = {"chinese_body": tools.ATLAS_INFO["chinese_body"],
          "chinese_body_bold": tools.ATLAS_INFO["chinese_body_bold"]}


def read_glyph_table(path):
    recs = []
    meta = {}
    with open(path, "r", encoding="utf-8-sig") as f:
        for line in f:
            parts = line.rstrip("\n").split("\t")
            if not parts or not parts[0]:
                continue
            if parts[0] == "id":
                continue
            if parts[0] in ("name", "PointSize", "atlas", "material", "props", "glyphCount", "LineHeight"):
                meta[parts[0]] = parts[1:]
                continue
            try:
                recs.append([int(float(parts[0]))] + [float(v) for v in parts[1:9]])
            except Exception:
                continue
    return np.array(recs, dtype=np.float64), meta


def calibrate(arr, recs):
    """标定原版 SDF：edge（边缘值）与 slope（每像素级数）"""
    best = None
    for thresh in (80, 88, 95, 105, 115, 128):
        vals, dists = [], []
        for r in recs[:: max(1, len(recs) // 200)]:
            cp, x, y, w, h = int(r[0]), int(r[1]), int(r[2]), int(np.ceil(r[3])), int(np.ceil(r[4]))
            if w < 4 or h < 4 or x + w > arr.shape[1] or y + h > arr.shape[0]:
                continue
            p = arr[y:y + h, x:x + w].astype(np.float64)
            inside = p >= thresh
            if inside.sum() in (0, p.size):
                continue
            din = _edt(inside)
            dout = _edt(~inside)
            vals.append(p.ravel()); dists.append((din - dout).ravel())
        if not vals:
            continue
        v = np.concatenate(vals); d = np.concatenate(dists)
        m = np.abs(d) <= 4
        A = np.vstack([np.ones(m.sum()), d[m]]).T
        coef, *_ = np.linalg.lstsq(A, v[m], rcond=None)
        rmse = float(np.sqrt(np.mean((A @ coef - v[m]) ** 2)))
        print(f"   阈值={thresh}: edge={coef[0]:.1f} slope={coef[1]:.1f}/px RMSE={rmse:.2f}")
        # 自洽性：拟合出的 edge 应与所用的二值化阈值接近
        err = abs(coef[0] - thresh) + rmse
        if best is None or err < best[0]:
            best = (err, coef[0], coef[1])
    return (best[1], best[2]) if best else (110.0, 18.0)


try:
    from scipy import ndimage as _ndi
    _HAS_SCIPY = True
except Exception:
    _HAS_SCIPY = False


def _edt(mask):
    """返回每个像素到最近「异类」像素的距离（有 scipy 时用精确欧氏距离变换）"""
    if _HAS_SCIPY:
        d_in = _ndi.distance_transform_edt(mask)        # 内部像素 → 最近外部像素
        d_out = _ndi.distance_transform_edt(~mask)      # 外部像素 → 最近内部像素
        return np.where(mask, d_in, d_out).astype(np.float32)
    h, w = mask.shape
    ys, xs = np.mgrid[0:h, 0:w]
    edge = mask & ~_erode(mask)
    pts = np.argwhere(edge)
    if len(pts) == 0:
        return np.full((h, w), 4.0, np.float32)
    dy = ys[..., None] - pts[:, 0]; dx = xs[..., None] - pts[:, 1]
    return np.sqrt((dy * dy + dx * dx).astype(np.float32)).min(axis=2)


def _erode(m):
    e = m.copy()
    e[1:, :] &= m[:-1, :]; e[:-1, :] &= m[1:, :]
    e[:, 1:] &= m[:, :-1]; e[:, :-1] &= m[:, 1:]
    return e


def render_glyph_alpha(ch, em, tw, th, flip):
    font = ImageFont.truetype(TTF, max(8, int(round(em * SS))))
    side = max(8, int(round(em * SS))) * 3
    img = Image.new("L", (side, side), 0)
    ImageDraw.Draw(img).text((side // 3, side // 3), ch, fill=255, font=font, anchor="ls")
    a = np.asarray(img)
    ys, xs = np.nonzero(a)
    if len(ys) == 0:
        return None
    ink = a[ys.min():ys.max() + 1, xs.min():xs.max() + 1]
    ih, iw = ink.shape
    sc = min((tw - 2) * SS / iw, (th - 2) * SS / ih)
    nw, nh = max(1, int(round(iw * sc / SS))), max(1, int(round(ih * sc / SS)))
    small = np.asarray(Image.fromarray(ink).resize((nw, nh), Image.BOX), np.float32) / 255.0
    if flip:
        small = np.flipud(small)
    out = np.zeros((th, tw), np.float32)
    oy = (th - nh) // 2; ox = (tw - nw) // 2
    out[oy:oy + nh, ox:ox + nw] = small
    return out



SDF_SS = 4          # 计算 SDF 的超采样倍数（解决边缘锯齿）


def render_glyph_sdf(ch, em, tw, th, flip, edge=128.0, slope=128.0 / 6.0, bbox=None):
    """把字符渲染成 (th, tw) 的 SDF 值图：以 SDF_SS 倍超采样求距离后再降采样，边缘平滑"""
    ss = SDF_SS
    font = ImageFont.truetype(TTF, max(8, int(round(em * ss))))
    side = max(8, int(round(em * ss))) * 3
    img = Image.new("L", (side, side), 0)
    ImageDraw.Draw(img).text((side // 3, side // 3), ch, fill=255, font=font, anchor="ls")
    a = np.asarray(img)
    ys, xs = np.nonzero(a)
    if len(ys) == 0:
        return None
    ink = a[ys.min():ys.max() + 1, xs.min():xs.max() + 1]
    ih, iw = ink.shape
    canvas = np.zeros((th * ss, tw * ss), np.float32)
    if bbox is not None:
        # 按原版该字的墨迹框定位与缩放（保持原版大小与位置）
        bx0, by0, bx1, by1 = bbox
        nw = max(1, int(round((bx1 - bx0) * ss)))
        nh = max(1, int(round((by1 - by0) * ss)))
        big = np.asarray(Image.fromarray(ink).resize((nw, nh), Image.LANCZOS), np.float32) / 255.0
        ox, oy = int(round(bx0 * ss)), int(round(by0 * ss))
    else:
        sc = min((tw - 2) * ss / iw, (th - 2) * ss / ih)
        nw, nh = max(1, int(round(iw * sc))), max(1, int(round(ih * sc)))
        big = np.asarray(Image.fromarray(ink).resize((nw, nh), Image.LANCZOS), np.float32) / 255.0
        oy = (th * ss - nh) // 2
        ox = (tw * ss - nw) // 2
    if flip:
        big = np.flipud(big)
    # 越界保护
    if ox < 0: ox = 0
    if oy < 0: oy = 0
    if ox + nw > tw * ss: nw = tw * ss - ox
    if oy + nh > th * ss: nh = th * ss - oy
    big = big[:nh, :nw]
    canvas[oy:oy + nh, ox:ox + nw] = big
    inside = canvas >= 0.5
    if inside.sum() == 0:
        return None
    d = _edt(inside) / ss          # 距离换算回目标像素
    signed = np.where(inside, d - 0.5 / ss, -(d - 0.5 / ss))
    v = np.clip(edge + slope * signed, 0, 255).astype(np.uint8)
    # 降采样回目标分辨率
    return np.asarray(Image.fromarray(v).resize((tw, th), Image.BOX), np.uint8)



def orig_bbox(arr_orig, x, y, w, h):
    """从原版图集测量该字的墨迹框（相对于格子左上角），用于让新字形保持原版大小与位置"""
    H = arr_orig.shape[0]
    yy = H - y - h
    if yy < 0 or yy + h > H:
        return None
    cell = arr_orig[yy:yy + h, x:x + w]
    mask = cell >= 128
    if mask.sum() == 0:
        return None
    rows = np.nonzero(mask.any(axis=1))[0]
    cols = np.nonzero(mask.any(axis=0))[0]
    return (int(cols.min()), int(rows.min()), int(cols.max()) + 1, int(rows.max()) + 1)


def process(key, edge, slope, flip, arr_src=None, arr_orig=None):
    info = TARGET[key]
    tbl = os.path.join(EXPORT, f"字形表_{key}.txt")
    recs, meta = read_glyph_table(tbl)
    em = float(meta.get("PointSize", ["73"])[0]) if meta.get("PointSize") else 73.0
    if em < 5:
        em = 73.0
    arr = (arr_src if arr_src is not None else tools.load_atlas(key)).copy()
    # 关键：先把整张图集清空，再逐字写入 —— 否则原版邻字残留在格子外的墨迹会被采样到，
    # 表现为「每个字旁边有别的字的一部分」。
    arr[:] = 0
    ok = miss = 0
    for r in recs:
        cp, x, y, w, h = int(r[0]), int(r[1]), int(r[2]), int(np.ceil(r[3])), int(np.ceil(r[4]))
        if w < 1 or h < 1 or x < 0 or y < 0 or x + w > arr.shape[1] or y + h > arr.shape[0]:
            miss += 1; continue
        bbox = orig_bbox(arr_orig, x, y, w, h) if arr_orig is not None else None
        sdf = render_glyph_sdf(chr(cp), em, w, h, flip, edge, slope, bbox)
        if sdf is None:
            miss += 1; continue
        # 关键：字形表的 y 是「自图集底部」计的，图集字节按图像顺序存放 → 真实行号 = 高度 - y - 字形高
        yy = arr.shape[0] - y - h
        if yy < 0 or yy + h > arr.shape[0]:
            miss += 1; continue
        arr[yy:yy + h, x:x + w] = sdf
        ok += 1
    return arr, ok, miss, em


def choose_flip(arr, recs, em):
    """用多个常用字的墨迹重合度判定存储方向（IoU 高者为准）"""
    samples = [0x4E00, 0x4E8C, 0x4E09, 0x4E2D, 0x56DB, 0x4E94, 0x516D, 0x4E03, 0x5341, 0x738B]
    score = {False: 0.0, True: 0.0}
    cnt = 0
    for cp in samples:
        idx = np.where(recs[:, 0] == cp)[0]
        if len(idx) == 0:
            continue
        r = recs[idx[0]]
        x, y, w, h = int(r[1]), int(r[2]), int(np.ceil(r[3])), int(np.ceil(r[4]))
        if w < 6 or h < 6 or x + w > arr.shape[1] or y + h > arr.shape[0]:
            continue
        orig = arr[y:y + h, x:x + w] >= 128
        if orig.sum() == 0:
            continue
        cnt += 1
        for flip in (False, True):
            a = render_glyph_alpha(chr(cp), em, w, h, flip)
            if a is None:
                continue
            mine = a >= 0.5
            inter = (mine & orig).sum(); union = (mine | orig).sum()
            score[flip] += inter / max(1, union)
    if cnt == 0:
        return False
    return score[True] > score[False]


if __name__ == "__main__":
    EDGE, SLOPE = 128.0, 128.0 / 6.0
    write = "--write" in sys.argv
    for key in ("chinese_body", "chinese_body_bold"):
        tbl = os.path.join(EXPORT, f"字形表_{key}.txt")
        if not os.path.exists(tbl):
            print(f"缺少导出文件：{tbl}")
            continue
        recs, meta = read_glyph_table(tbl)
        em = float(meta.get("PointSize", ["73"])[0])
        arr_src = tools.load_atlas(key)
        flip = True   # 游戏内实测：必须上下翻转
        print(f"--- {key}: 字形 {len(recs)} 个，PointSize={em}，方向翻转={flip}")
        arr, ok, miss, em2 = process(key, EDGE, SLOPE, flip, arr_src)
        print(f"    重制 {ok} 个，跳过 {miss} 个")
        if write:
            tools.save_atlas(key, arr)
            print(f"    已写入 {key} 图集（同偏移同长度）")
        else:
            Image.fromarray(np.flipud(arr), "L").save(f"预览新图集_{key}.png")
            print("    已输出预览图（未写入）")
