# -*- coding: utf-8 -*-
from PIL import Image, ImageDraw, ImageFont
import sys

text = "苍白宫廷"
w, h = 310, 53
font_size = 30

font = ImageFont.truetype("/c/Windows/Fonts/msyh.ttc", font_size)
print("Font loaded OK")

img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
draw = ImageDraw.Draw(img)
bbox = draw.textbbox((0, 0), text, font=font)
tw, th = bbox[2] - bbox[0], bbox[3] - bbox[1]
tx = (w - tw) // 2
ty = (h - th) // 2 + 2
draw.text((tx, ty), text, font=font, fill=(240, 238, 228, 255))
img.save("zh_logo_final.png")
print("Saved zh_logo_final.png:", w, "x", h)

px = img.load()
center = px[w//2, h//2]
print("Center pixel (should be near white):", center)

# Also check a few pixels around the center for non-white content
nonwhite = []
for x in range(w//2 - 20, w//2 + 20):
    for y in range(h//2 - 5, h//2 + 5):
        if px[x, y][3] > 100:
            nonwhite.append((x, y, px[x,y]))
print("Non-transparent pixels near center:", len(nonwhite))
if nonwhite:
    print("Sample:", nonwhite[:5])
