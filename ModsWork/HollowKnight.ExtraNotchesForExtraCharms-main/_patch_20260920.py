# -*- coding: utf-8 -*-
"""改造「额外护符槽位」的加成算法。

用户要求（2026-09-20）：
  · 默认「一个护符增加 5 个槽位」（原来是「每 5 个护符换 1 个槽位」的反比例算法）
  · 菜单里也要改成「一个护符增加几个槽位」
  · 只统计玩家**已解锁**的额外护符；一个都没解锁就不加成
    例：只解锁禅意护符 → +5；再解锁一个苍白宫廷护符 → +10
"""
import io, os

BASE = r"D:\KDQSHH\ModsWork\HollowKnight.ExtraNotchesForExtraCharms-main\ExtraNotchesForExtraCharms"


def load(path):
    raw = open(path, "rb").read()
    bom = raw[:3] == b"\xef\xbb\xbf"
    s = raw.decode("utf-8-sig" if bom else "utf-8")
    crlf = "\r\n" in s
    return s.replace("\r\n", "\n"), bom, crlf


def save(path, text, bom, crlf):
    if crlf:
        text = text.replace("\n", "\r\n")
    open(path, "wb").write((b"\xef\xbb\xbf" if bom else b"") + text.encode("utf-8"))


def patch(rel, old, new, tag):
    path = os.path.join(BASE, rel)
    s, bom, crlf = load(path)
    assert old in s, "锚点未找到：%s" % tag
    assert s.count(old) == 1, "锚点不唯一(%d)：%s" % (s.count(old), tag)
    save(path, s.replace(old, new, 1), bom, crlf)
    print("  [ok] %s  (%s)" % (tag, rel))


# ---------- 1) 设置字段：语义反转 ----------
patch(
    "GlobalSettings.cs",
    """        /// <summary>
        /// Number of extra charms required for an extra charm notch
        /// </summary>
        public int charmsPerNotch { get; set; } = 5;""",
    """        /// <summary>
        /// 每解锁一个「额外护符」，额外增加的护符槽位数量。
        /// 修改(2026-09-20)：原为「每 N 个额外护符换 1 个护符槽位」（除法），
        /// 现改为「每个已解锁的额外护符增加 N 个槽位」（乘法），默认 5。
        /// </summary>
        public int notchesPerCharm { get; set; } = 5;""",
    "设置字段：charmsPerNotch → notchesPerCharm")

# ---------- 2) 核心算法 ----------
patch(
    "ExtraNotchesForExtraCharms.cs",
    """        /// <summary>
        /// Calculates the number of extra notches to add
        /// </summary>
        /// <returns></returns>
        private int GetExtraNotches()
        {
            FieldInfo fieldInfo = typeof(CharmHelper).GetField("CustomSprites",
                                                                BindingFlags.NonPublic | BindingFlags.Static);
            List<Sprite> customCharmList = (List<Sprite>)fieldInfo.GetValue(null);
            int maxCharms = customCharmList.Count;
            int extraNotches = maxCharms / globalSettings.charmsPerNotch;
            Log($"{maxCharms} modded charms -> {extraNotches} extra charm notches");

            return extraNotches;
        }""",
    """        /// <summary>
        /// 计算本次应额外增加的护符槽位数量。
        /// 修改(2026-09-20)：只统计玩家**已经解锁**的额外护符，
        /// 每解锁一个就增加 notchesPerCharm 个槽位；一个都没解锁则不增加。
        /// </summary>
        /// <returns></returns>
        private int GetExtraNotches()
        {
            FieldInfo fieldInfo = typeof(CharmHelper).GetField("CustomSprites",
                                                                BindingFlags.NonPublic | BindingFlags.Static);
            List<Sprite> customCharmList = (List<Sprite>)fieldInfo.GetValue(null);
            int maxCharms = customCharmList.Count;

            int unlocked = CountUnlockedModdedCharms(maxCharms);
            int extraNotches = unlocked * globalSettings.notchesPerCharm;
            Log($"已解锁 {unlocked}/{maxCharms} 个额外护符，每个 +{globalSettings.notchesPerCharm} 槽位 -> 共 {extraNotches} 个额外护符槽位");

            return extraNotches;
        }

        /// <summary>
        /// 统计玩家已经解锁（获得）的额外护符数量。
        /// 模组新增的护符编号从 41 开始（SFCore 的约定：第 i 个护符的编号是 41 + i），
        /// 解锁状态记录在 PlayerData 的 gotCharm_41、gotCharm_42 …… 字段中。
        /// 苍白宫廷、禅意护符等模组都按这个约定读写（部分模组用钩子映射到自己的存档字段）。
        /// </summary>
        /// <param name="maxCharms">模组新增的护符总数</param>
        /// <returns></returns>
        private int CountUnlockedModdedCharms(int maxCharms)
        {
            int unlocked = 0;
            for (int i = 0; i < maxCharms; i++)
            {
                int charmId = 41 + i;
                try
                {
                    if (PlayerData.instance.GetBool("gotCharm_" + charmId))
                    {
                        unlocked++;
                    }
                }
                catch (Exception e)
                {
                    // 极少数护符没有对应的存档字段：按未解锁处理，不影响其余护符
                    Log($"读取 gotCharm_{charmId} 失败，按未解锁处理：{e.Message}");
                }
            }
            return unlocked;
        }""",
    "核心算法：改为「每个已解锁护符 × N」")

# ---------- 3) 菜单文案与取值 ----------
patch(
    "ExtraNotchesForExtraCharms.cs",
    """                new("每护符槽位数", new[]{"1","2","3","4","5","6","7","8","9","10"}, "每个护符槽位可容纳的额外护符数量。", i => globalSettings.charmsPerNotch = i + 1, () => globalSettings.charmsPerNotch - 1)""",
    """                new("每个护符增加的槽位数", new[]{"1","2","3","4","5","6","7","8","9","10"}, "每解锁一个额外护符，增加的护符槽位数量。", i => globalSettings.notchesPerCharm = i + 1, () => globalSettings.notchesPerCharm - 1)""",
    "菜单项：文案与语义")

# ---------- 4) 版本号（功能已变更，升版本以便区分） ----------
patch(
    "ExtraNotchesForExtraCharms.cs",
    """        public override string GetVersion() => "1.0.0.0";""",
    """        public override string GetVersion() => "1.1.0.0";""",
    "版本号 1.0.0.0 → 1.1.0.0")

patch(
    "ExtraNotchesForExtraCharms.csproj",
    """    <AssemblyVersion>1.0.0.0</AssemblyVersion>
    <FileVersion>1.0.0.0</FileVersion>
    <InformationalVersion>1.0.0.0汉化</InformationalVersion>""",
    """    <AssemblyVersion>1.1.0.0</AssemblyVersion>
    <FileVersion>1.1.0.0</FileVersion>
    <InformationalVersion>1.1.0.0汉化</InformationalVersion>""",
    "csproj 版本号")

# ---------- 5) 未被调用的 Satchel 菜单也同步（保持一致，避免以后误用） ----------
patch(
    "ModMenu.cs",
    """            menu.AddElement(new HorizontalOption("每护符槽位数",
                                                    "每个护符槽位可容纳的额外护符数量",
                                                    CostValues(),
                                                    value => ExtraNotchesForExtraCharms.globalSettings.charmsPerNotch = value + 1,
                                                    () => ExtraNotchesForExtraCharms.globalSettings.charmsPerNotch - 1));""",
    """            menu.AddElement(new HorizontalOption("每个护符增加的槽位数",
                                                    "每解锁一个额外护符，增加的护符槽位数量",
                                                    CostValues(),
                                                    value => ExtraNotchesForExtraCharms.globalSettings.notchesPerCharm = value + 1,
                                                    () => ExtraNotchesForExtraCharms.globalSettings.notchesPerCharm - 1));""",
    "Satchel 菜单同步")

print("\n全部补丁已应用")
