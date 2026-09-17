"""
斯卡蒂深海色调重绘脚本
- 源文件：只读 E:\steam\steamapps\common\Hollow Knight\...\Skins\Default
- 输出：D:\KDQSHH\Skins\斯卡蒂_Arknights_Claude\
- 转换策略：非透明像素进行色相/饱和度/明度映射，透明像素保持原样（锁定帧布局）
- 深海配色：深蓝/冰蓝/青白主体 + 深海紫青光晕
"""
import sys
sys.stdout.reconfigure(encoding='utf-8')
from pathlib import Path
from PIL import Image, ImageEnhance
import hashlib

SRC = Path(r"E:/steam/steamapps/common/Hollow Knight/hollow_knight_Data/Managed/Mods/CustomKnight（自定义小骑士）/Skins/Default")
OUT = Path(r"D:/KDQSHH/Skins/斯卡蒂_Arknights_Claude")
OUT.mkdir(parents=True, exist_ok=True)

# 深海配色表：原色 → 目标色
# 白色 → 极地冰白（斯卡蒂主色）
# 黑色/深灰 → 深海暗蓝
# 蓝灰(78,84,105) → 冰蓝（斯卡蒂眼睛/武器）
# 红/橙 → 深海生物辉光
PALETTE_MAP = {
    (255, 255, 255, 255): (224, 242, 255, 255),   # 纯白 → 极地冰白
    (255, 254, 254, 255): (224, 242, 255, 255),
    (248, 248, 248, 255): (218, 237, 252, 255),
    (1, 1, 2, 255): (8, 30, 60, 255),              # 极淡灰 → 深海暗蓝
    (0, 0, 0, 255): (12, 28, 56, 255),             # 纯黑 → 深海蓝黑
    (0, 0, 0, 254): (12, 28, 56, 254),
    (0, 0, 0, 253): (12, 28, 56, 253),
    (0, 0, 0, 252): (12, 28, 56, 252),
    (0, 0, 0, 251): (12, 28, 56, 251),
    (0, 0, 0, 250): (12, 28, 56, 250),
    (0, 0, 0, 16): (12, 28, 56, 16),
    (0, 0, 0, 14): (12, 28, 56, 14),
    (0, 0, 0, 12): (12, 28, 56, 12),
    (0, 0, 0, 4):  (12, 28, 56, 4),
    (0, 0, 0, 2):  (12, 28, 56, 2),
    (0, 0, 0, 1):  (12, 28, 56, 1),
    (1, 1, 1, 255): (14, 32, 62, 255),
    (1, 1, 1, 0):  (14, 32, 62, 0),
    (2, 2, 4, 255): (16, 38, 72, 255),
    (3, 3, 6, 255): (18, 42, 78, 255),
    (4, 4, 9, 255): (22, 50, 90, 255),
    (6, 6, 13, 255): (26, 56, 100, 255),
    (78, 84, 105, 255): (70, 148, 210, 255),        # 蓝灰眼睛 → 冰蓝
    (79, 85, 106, 255): (70, 148, 210, 255),
    (255, 0, 0, 0): (12, 28, 56, 0),
    (0, 0, 255, 0): (12, 28, 56, 0),
    (255, 0, 0, 0): (12, 28, 56, 0),
    (253, 253, 253, 0): (200, 228, 252, 0),
    (254, 254, 254, 0): (200, 228, 252, 0),
}

# 深海生物辉光色
GLOW_MAP = {
    (255, 165, 0, 255): (0, 206, 209, 255),
    (0, 128, 255, 255): (0, 206, 209, 255),
    (255, 69, 0, 255): (0, 206, 209, 255),
    (255, 0, 128, 255): (128, 0, 210, 255),
}

def remap_pixel(r, g, b, a):
    """深海色调色板映射，保持透明度"""
    # 尝试精确匹配
    key = (r, g, b, a)
    if key in PALETTE_MAP:
        return PALETTE_MAP[key]
    # 近似匹配：对接近的颜色做偏移
    # 白色系 → 冰白
    if r > 230 and g > 230 and b > 230 and a > 10:
        return (min(240, r+5), min(250, g+8), min(255, b+12), a)
    # 黑色系 → 深海蓝
    if r < 12 and g < 12 and b < 12 and a > 10:
        return (min(25, max(0, r+10)), min(32, max(0, g+18)), min(64, max(0, b+48)), a)
    # 蓝灰系 → 冰蓝
    if 60 < r < 90 and 70 < g < 100 and 95 < b < 120 and a > 10:
        return (min(130, r+40), min(170, g+60), min(225, b+95), a)
    # 默认：增强蓝调
    return (max(0, min(255, r-5)), max(0, min(255, g+10)), max(0, min(255, b+25)), a)

def process_image(src_path, out_path):
    """处理单张图片，返回 (宽度, 高度, SHA256)"""
    img = Image.open(src_path)
    w, h = img.size
    # 确保 RGBA 模式
    if img.mode != 'RGBA':
        img = img.convert('RGBA')

    new_img = Image.new('RGBA', (w, h), (0, 0, 0, 0))
    pixels = img.load()
    new_pixels = new_img.load()

    for y in range(h):
        for x in range(w):
            r, g, b, a = pixels[x, y]
            if a > 0:
                new_r, new_g, new_b, new_a = remap_pixel(r, g, b, a)
                new_pixels[x, y] = (new_r, new_g, new_b, new_a)
            else:
                new_pixels[x, y] = (0, 0, 0, 0)

    new_img.save(out_path, 'PNG', optimize=True)
    sha = hashlib.sha256(out_path.read_bytes()).hexdigest()[:16]
    return w, h, sha

def main():
    categories = ['', 'Charms', 'HPBar', 'Inventory', 'SaveHud', 'AreaBackgrounds']
    total = 0
    errors = []

    for cat in categories:
        cat_src = SRC / cat if cat else SRC
        cat_out = OUT / cat if cat else OUT
        cat_out.mkdir(parents=True, exist_ok=True)

        ext = '*.png' if cat != 'AreaBackgrounds' else '*.png'
        files = sorted(cat_src.glob('*.png')) if cat != 'AreaBackgrounds' else sorted((SRC / 'AreaBackgrounds').glob('*.png'))

        for f in files:
            try:
                out_file = cat_out / f.name
                w, h, sha = process_image(f, out_file)
                total += 1
                if total % 50 == 0:
                    print(f"  已处理 {total} 文件: {f.name} ({w}x{h})")
            except Exception as e:
                errors.append((str(f), str(e)))
                print(f"  错误: {f.name}: {e}", file=sys.stderr)

    # 复制非 PNG 文件
    import shutil
    for f in ['replace.txt', 'skin-config.json', 'skin-settings.json']:
        src_f = SRC / f
        if src_f.exists():
            shutil.copy2(src_f, OUT / f)

    print(f"\n=== 转换完成 ===")
    print(f"总计处理: {total} 个 PNG 文件")
    print(f"错误数: {len(errors)}")
    if errors:
        for f, e in errors[:5]:
            print(f"  {f}: {e}")

if __name__ == '__main__':
    main()
