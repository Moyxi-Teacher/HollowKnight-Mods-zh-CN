# -*- coding: utf-8 -*-
"""生活质量（QoL）修复补丁。

问题：模组菜单里那个「总开关」永远显示为「关」，与实际启用状态不符。
原因：QoL 实现的是 ICustomMenuMod（自建菜单），而 Modding API 只在 IMenuMod 分支里
      主动调用 menuSetting.RefreshValueFromGameSettings()，ICustomMenuMod 分支没有，
      于是该选项的显示值一直停留在 optionList 的初始索引 0（= 关）。
修复：在创建该选项后立即手动刷新一次。

同时保留此前的汉化改动（模组显示名 → 生活质量）。
"""
import os

PROJ = r"D:\KDQSHH\ModsWork\_qol_49\QoL"


def load(path):
    raw = open(path, "rb").read()
    has_bom = raw[:3] == b"\xef\xbb\xbf"
    s = raw.decode("utf-8-sig" if has_bom else "utf-8")
    return s.replace("\r\n", "\n"), has_bom, (s.count("\r\n") > 0)


def save(path, text, has_bom, use_crlf):
    if use_crlf:
        text = text.replace("\n", "\r\n")
    data = text.encode("utf-8")
    if has_bom:
        data = b"\xef\xbb\xbf" + data
    open(path, "wb").write(data)


def patch(path, edits):
    s, has_bom, crlf = load(path)
    for i, (old, new, tag) in enumerate(edits, 1):
        assert old in s, "锚点 %d 未找到: %s" % (i, tag)
        assert s.count(old) == 1, "锚点 %d 不唯一: %s" % (i, tag)
        s = s.replace(old, new, 1)
        print("  [ok] " + tag)
    save(path, s, has_bom, crlf)


print("=== QoL.cs：保留汉化显示名 ===")
patch(os.path.join(PROJ, "QoL.cs"), [
    (
        """        public QoL() : base()
        {
            typeof(SettingsOverride).ModInterop();
        }

        public override string GetVersion() => VersionUtil.GetVersion<QoL>();""",
        """        // 汉化：模组显示名改为中文。注意存档键也随之变为「生活质量」。
        public QoL() : base("生活质量")
        {
            typeof(SettingsOverride).ModInterop();
        }

        public override string GetVersion() => VersionUtil.GetVersion<QoL>();

        // 汉化：模组列表里进入本模组菜单的按钮文本
        public override string GetMenuButtonText() => "生活质量";""",
        "显示名「生活质量」+ 菜单按钮文本",
    ),
])

print("=== ModMenu.cs：修复总开关显示 ===")
patch(os.path.join(PROJ, "ModMenu.cs"), [
    (
        """                    c.AddHorizontalOption("QoL", new HorizontalOptionConfig
                    {
                        Label = "QoL",
                        Options = new[]
                        {
                            Lang.Get("MOH_OFF", "MainMenu"),
                            Lang.Get("MOH_ON", "MainMenu")
                        },
                        ApplySetting   = (_, i) => dels.SetModEnabled(Convert.ToBoolean(i)),
                        RefreshSetting = (s, _) => s.optionList.SetOptionTo(Convert.ToInt32(dels.GetModEnabled())),
                        CancelAction = Return,
                        Style = HorizontalOptionStyle.VanillaStyle
                    });""",
        """                    // 修复(2026-09-19)：本模组走 ICustomMenuMod 自建菜单这条路，
                    // Modding API 只在 IMenuMod 分支里主动调用 RefreshValueFromGameSettings()，
                    // 因此这个「总开关」的显示值一直停留在初始索引 0（显示为「关」），
                    // 与实际启用状态不符。这里在创建之后立即手动刷新一次。
                    c.AddHorizontalOption("QoL", new HorizontalOptionConfig
                    {
                        Label = "QoL",
                        Options = new[]
                        {
                            Lang.Get("MOH_OFF", "MainMenu"),
                            Lang.Get("MOH_ON", "MainMenu")
                        },
                        ApplySetting   = (_, i) => dels.SetModEnabled(Convert.ToBoolean(i)),
                        RefreshSetting = (s, _) => s.optionList.SetOptionTo(Convert.ToInt32(dels.GetModEnabled())),
                        CancelAction = Return,
                        Style = HorizontalOptionStyle.VanillaStyle
                    }, out MenuOptionHorizontal qolToggle);
                    qolToggle.menuSetting.RefreshValueFromGameSettings();""",
        "总开关创建后立即刷新显示",
    ),
])

print("\n补丁已应用")
