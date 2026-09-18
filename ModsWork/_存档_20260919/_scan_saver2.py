# -*- coding: utf-8 -*-
"""精细排查：对「无持久化接口」的模组，看它们到底有没有需要保存的菜单项。

判定：
  · 有 Saver 且 Saver 写的是内存字段 → 需要持久化，缺失则「不保存」
  · 无 Saver → 菜单不涉及存储，无需修复
  · 有 IGlobalSettings / ILocalSettings → 正常
"""
import os, re, subprocess, shutil

ROOT = r"E:\steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods"
TMP  = r"D:\重要文件\Downloads\temp\scan_fine"
shutil.rmtree(TMP, ignore_errors=True)
os.makedirs(TMP, exist_ok=True)

# 用户明确点名的 + 扫描出的无接口模组
NAMES = ["Pale Court（苍白宫廷）", "QoL（生活质量）",
         "AdditionalMaps（额外地图）", "AlwaysHazardRespawn（就近复活）",
         "BenchWarpFix（长椅传送修正）", "BenchwarpSpoilerCloak（长椅传送保护）",
         "CollisionGlitchRemover（碰撞故障修正）", "DarkerTransitions（更暗的转场）",
         "DoubleJumpFix（双跳修正）", "DynamicCrystalDash（动态水晶冲刺）",
         "EasySoulRefill（轻松灵魂填充）", "GeoPots（吉欧存钱罐）",
         "InfectedHallownest（受感染的圣巢）", "InfiniteNotches（无限护符槽）",
         "InvulnerabilityIndicator（无敌指示器）", "KeepDiveIFrames（保持下冲无敌帧）",
         "Konpanion（伴侣）", "PersistentDivine（常驻迪万）", "Pogonest（打击跳跃）",
         "QuirrelBoat（奎若的船）", "ReplayTimerMod（回放计时器）",
         "ScreenResolution（屏幕分辨率）", "UneatedGrubs（保护幼虫）"]

def analyze(name):
    d = os.path.join(ROOT, name)
    if not os.path.isdir(d): return None
    dlls = [f for f in os.listdir(d) if f.lower().endswith(".dll")]
    if not dlls: return None
    out = os.path.join(TMP, re.sub(r"[^\w]", "_", name))
    try:
        subprocess.run(["ilspycmd", os.path.join(d, dlls[0]), "-o", out],
                       capture_output=True, text=True, encoding="utf-8",
                       errors="replace", timeout=240)
    except Exception:
        return ("超时", [], [], [])
    blob = ""
    if os.path.isdir(out):
        for dp, _, fns in os.walk(out):
            for f in fns:
                if f.endswith(".cs"):
                    blob += open(os.path.join(dp, f), encoding="utf-8", errors="replace").read() + "\n"
    has_g = "IGlobalSettings" in blob
    has_l = "ILocalSettings" in blob
    savers = re.findall(r"Saver\s*=\s*([^\n]{0,120})", blob)
    writes = sorted(set(re.findall(r"(?:File\.Write\w*|StreamWriter|JsonConvert\.Serialize\w*)", blob)))[:4]
    return ("接口:" + ("G" if has_g else "-") + ("L" if has_l else "-"), savers, writes, [])

print("%-40s %-10s %-8s %s" % ("模组", "持久化接口", "Saver数", "自行落盘"))
print("-" * 90)
need_fix = []
for n in NAMES:
    r = analyze(n)
    if r is None:
        print("%-40s %s" % (n[:38], "目录/ DLL 缺失")); continue
    iface, savers, writes, _ = r
    has_persist = ("G" in iface) or ("L" in iface)
    flag = ""
    if savers and not has_persist and not writes:
        flag = "  <<< 需要修复"
        need_fix.append(n)
    print("%-40s %-10s %-8d %s%s" % (n[:38], iface, len(savers), ",".join(writes)[:26], flag))

print("\n=== 确实需要修复（有 Saver 但无任何持久化）===")
for n in need_fix: print("  " + n)
