# 依据字形表，把游戏中文 TMP 字体图集里的字形重制为「霞鹜臻楷」的 SDF 字形。
# 原理：图集是 Alpha8 的 SDF 数据（边缘值 128，梯度 = 128/_GradientScale=6 → 约 21.3 级/像素），
#      材质与字形表都不改动，只替换像素 → 游戏原生显示霞鹜臻楷，无需任何运行时模组。
import sys, os
import numpy as np
from PIL import Image, ImageDraw, ImageFont

sys.path.insert(0, r"D:\KDQSHH\字体资产工作")
import tools

TTF = r"D:\KDQSHH\ModsWork\SimplifiedChineseFont-HEAD\霞鹜臻楷.ttf"
EDGE = 128.0
SLOPE = 128.0 / 6.0     # 由原版材质 _GradientScale=6 推出
SS = 4                  # 渲染超采样倍数

# 各资产的字号（由字形表 xAdvance 反推：全角字advance即em尺寸）
EM = {"chinese_body": 73, "chinese_body_bold": 34}


def render_ink(ch, em_px, ss=SS):
    """把字符渲染成超采样灰度图，返回 (数组, 墨迹包围盒)"""
    size = max(8, int(round(em_px * ss)))
    font = ImageFont.truetype(TTF, size)
    pad = size
    img = Image.new("L", (size * 3, size * 3), 0)
    d = ImageDraw.Draw(img)
    try:
        d.text((size, size), ch, fill=255, font=font, anchor="ls")
    except Exception:
        d.text((size, size), ch, fill=255, font=font)
    a = np.asarray(img, dtype=np.uint8)
    ys, xs = np.nonzero(a)
    if len(ys) == 0:
        return None, None
    return a, (xs.min(), ys.min(), xs.max() + 1, ys.max() + 1)


def signed_field(inside):
    """二值掩膜 → 有符号距离场（像素单位，内部为正）"""
    h, w = inside.shape
    er = inside.copy()
    er[1:, :] &= inside[:-1, :]; er[:-1, :] &= inside[1:, :]
    er[:, 1:] &= inside[:, :-1]; er[:, :-1] &= inside[:, 1:]
    bedge = inside & ~er
    nb = np.argwhere(bedge)
    ys, xs = np.mgrid[0:h, 0:w]
    if len(nb) == 0:
        return np.where(inside, 8.0, -8.0)
    dy = ys[..., None] - nb[:, 0]
    dx = xs[..., None] - nb[:, 1]
    dist = np.sqrt((dy * dy + dx * dx).astype(np.float32)).min(axis=2)
    return np.where(inside, dist - 0.5, -(dist - 0.5))


def glyph_sdf(rec, em_px, target_w, target_h):
    """生成单个字形的 SDF 图（target_h × target_w），无法渲染时返回 None"""
    ch = chr(int(rec["id"]))
    a, bbox = render_ink(ch, em_px)
    if a is None:
        return None
    x0, y0, x1, y1 = bbox
    ink = a[y0:y1, x0:x1]
    ih, iw = ink.shape
    # 目标：墨迹放进 rect 内（留 1 像素余量，保持比例）
    tw = max(1, target_w - 2)
    th = max(1, target_h - 2)
    scale = min(tw * SS / iw, th * SS / ih)
    nw, nh = max(1, int(round(iw * scale / SS))), max(1, int(round(ih * scale / SS)))
    small = np.asarray(Image.fromarray(ink).resize((nw, nh), Image.BOX), dtype=np.float32) / 255.0
    inside = small >= 0.5
    if inside.sum() == 0:
        return None
    field = signed_field(inside)
    val = np.clip(EDGE + SLOPE * field, 0, 255).astype(np.uint8)
    out = np.zeros((target_h, target_w), dtype=np.uint8)
    oy = (target_h - nh) // 2
    ox = (target_w - nw) // 2
    out[oy:oy + nh, ox:ox + nw] = val
    return out


def build(key, dry_run=False, sample=None):
    arr = tools.load_atlas(key).copy()
    recs, _ = tools.load_glyphs(key)
    em = EM[key]
    total = len(recs)
    ok = miss = 0
    samples = []
    for i, rec in enumerate(recs):
        cp = int(rec["id"])
        x, y = int(rec["x"]), int(rec["y"])
        w, h = max(1, int(np.ceil(rec["w"]))), max(1, int(np.ceil(rec["h"])))
        if x + w > arr.shape[1] or y + h > arr.shape[0]:
            miss += 1
            continue
        try:
            sdf = glyph_sdf(rec, em, w, h)
        except Exception:
            sdf = None
        if sdf is None:
            miss += 1
            continue
        if sample and cp in sample:
            samples.append((cp, arr[y:y + h, x:x + w].copy(), sdf.copy()))
        if not dry_run:
            arr[y:y + h, x:x + w] = sdf
        ok += 1
        if (i + 1) % 500 == 0:
            print(f"   {key}: {i+1}/{total} …")
    return arr, recs, ok, miss, samples


if __name__ == "__main__":
    mode = sys.argv[1] if len(sys.argv) > 1 else "dry"
    key = sys.argv[2] if len(sys.argv) > 2 else "chinese_body"
    if mode == "dry":
        sample = {0x6C38, 0x4E2D, 0x6211, 0x5B57, 0x4E00, 0x5341}
        arr, recs, ok, miss, samples = build(key, dry_run=True, sample=sample)
        print(f"{key}: 可重制 {ok} 个，跳过 {miss} 个")
        tiles = []
        for cp, old, new in samples:
            tiles.append((f"U+{cp:04X} 原版", old))
            tiles.append((f"U+{cp:04X} 新版", new))
        H = max(t[1].shape[0] for t in tiles) + 6
        W = sum(t[1].shape[1] + 6 for t in tiles)
        canvas = np.zeros((H, W), dtype=np.uint8)
        xo = 0
        for name, p in tiles:
            canvas[3:3 + p.shape[0], xo + 3:xo + 3 + p.shape[1]] = p
            xo += p.shape[1] + 6
        Image.fromarray(np.flipud(canvas), mode="L").resize((W * 4, H * 4), Image.NEAREST).save(f"对比_{key}.png")
        print("已输出对比图 对比_%s.png" % key)
    else:
        arr, recs, ok, miss, _ = build(key, dry_run=False)
        print(f"{key}: 完成重制 {ok} 个字形，跳过 {miss} 个")
        if mode == "write":
            tools.save_atlas(key, arr)
            print(f"已写入 {key} 图集（长度与偏移保持不变）")
        else:
            Image.fromarray(np.flipud(arr), mode="L").save(f"新图集预览_{key}.png")
