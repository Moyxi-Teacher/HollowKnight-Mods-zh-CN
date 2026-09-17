# 空洞骑士中文 TMP 字体资产：读取/解析/重建工具
# 说明：字体图集是 Alpha8 的流数据，存放在 sharedassets0.assets.resS 里，偏移与长度固定，
#      因此可以用「同长度覆盖」的方式替换像素，完全不改变资产结构。
import struct, os
import numpy as np

GAME = r"E:\steam\steamapps\common\Hollow Knight\hollow_knight_Data"
WORK = r"D:\KDQSHH\字体资产工作"
RESS = os.path.join(GAME, "sharedassets0.assets.resS")

# 图集在流文件中的位置（来自 m_StreamData）
ATLAS_INFO = {
    "chinese_body":      {"offset": 22650616, "size": 16777216, "dim": 4096, "mb": "mb_182.bin", "table_off": 3672, "count": 2540},
    "chinese_body_bold": {"offset":  8756984, "size":  4194304, "dim": 2048, "mb": "mb_183.bin", "table_off": 220,  "count": 2068},
}

GLYPH_DTYPE = np.dtype([("id", "<i4"), ("x", "<f4"), ("y", "<f4"), ("w", "<f4"), ("h", "<f4"),
                        ("xo", "<f4"), ("yo", "<f4"), ("xa", "<f4"), ("scale", "<f4")])


def load_glyphs(key):
    """读取字形表：返回 (records, byte_offset_of_table)"""
    info = ATLAS_INFO[key]
    data = open(os.path.join(WORK, info["mb"]), "rb").read()
    off, cnt = info["table_off"], info["count"]
    recs = np.frombuffer(data, dtype=GLYPH_DTYPE, count=cnt, offset=off)
    return recs, off


def load_atlas(key):
    """读取图集像素：(dim, dim) 的 uint8 数组。第 0 行对应 Unity 的 y=0（图像底部）。"""
    info = ATLAS_INFO[key]
    with open(RESS, "rb") as f:
        f.seek(info["offset"])
        raw = f.read(info["size"])
    dim = info["dim"]
    assert len(raw) == dim * dim, f"图集大小不符: {len(raw)} != {dim*dim}"
    return np.frombuffer(raw, dtype=np.uint8).reshape(dim, dim)


def save_atlas(key, arr):
    """把像素写回流文件（长度必须完全一致，只做同长度覆盖）"""
    info = ATLAS_INFO[key]
    data = np.ascontiguousarray(arr, dtype=np.uint8).tobytes()
    assert len(data) == info["size"], "字节数不一致，拒绝写入"
    with open(RESS, "r+b") as f:
        f.seek(info["offset"])
        f.write(data)


def glyph_of(recs, codepoint):
    idx = np.where(recs["id"] == codepoint)[0]
    return recs[idx[0]] if len(idx) else None


def to_image(arr):
    """转为 PIL 图像（垂直翻转，便于人工查看）"""
    from PIL import Image
    return Image.fromarray(np.flipud(arr), mode="L")
