# -*- coding: utf-8 -*-
"""控制器修正 —— 把总开关从「模组列表」移到「本模组菜单的第一项」。

按用户要求：开关要在二级菜单里，与生活质量（QoL）模组的表现一致。
实现方式：ToggleButtonInsideMenu 改为 true，并把 Modding API 传入的
ModToggleDelegates 交给菜单，用 Satchel 自带的 CreateToggle 生成开关项。
（Satchel 的 HorizontalOption 会在菜单构建完成时自动刷新显示状态，
  因此不会出现 QoL 那种「永远显示关」的问题。）
"""
import os

ROOT = r"D:\KDQSHH\ModsWork\ControllerFixes-HEAD\ControllerFixes"
F_MAIN = os.path.join(ROOT, "ControllerFixes.cs")
F_MENU = os.path.join(ROOT, "Settings", "ModMenu.cs")


def load(path):
    raw = open(path, "rb").read()
    has_bom = raw[:3] == b"\xef\xbb\xbf"
    s = raw.decode("utf-8-sig" if has_bom else "utf-8")
    return s.replace("\r\n", "\n"), has_bom, (s.count("\r\n") > 0)


def save(path, text, has_bom, crlf):
    if crlf:
        text = text.replace("\n", "\r\n")
    open(path, "wb").write((b"\xef\xbb\xbf" if has_bom else b"") + text.encode("utf-8"))


def patch(path, edits):
    s, bom, crlf = load(path)
    for i, (old, new, tag) in enumerate(edits, 1):
        assert old in s, "锚点 %d 未找到: %s" % (i, tag)
        assert s.count(old) == 1, "锚点 %d 不唯一(%d): %s" % (i, s.count(old), tag)
        s = s.replace(old, new, 1)
        print("  [ok] " + tag)
    save(path, s, bom, crlf)


# 注意：原文 `=> ` 后带一个行尾空格，必须显式保留
OLD_MAIN = (
    "    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? _) => \n"
    "\t    ModMenu.CreateMenuScreen(modListMenu);\n"
    "\n"
    "    public bool ToggleButtonInsideMenu => false;"
)
NEW_MAIN = (
    "    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates) => \n"
    "\t    ModMenu.CreateMenuScreen(modListMenu, toggleDelegates);\n"
    "\n"
    "    // 总开关(2026-09-19)：改为 true，开关放进本模组菜单内部（与生活质量模组一致），\n"
    "    // 不再显示在模组列表那一层。\n"
    "    public bool ToggleButtonInsideMenu => true;"
)

OLD_MENU = (
    "    public static MenuScreen CreateMenuScreen(MenuScreen modListMenu)\n"
    "    {\n"
    "        MenuRef = new Menu(\"控制器修正\",\n"
    "        [\n"
    "            new MenuButton(\"链接至说明\","
)
NEW_MENU = (
    "    public static MenuScreen CreateMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates = null)\n"
    "    {\n"
    "        // 总开关(2026-09-19)：放在菜单第一项，与生活质量等模组的表现一致。\n"
    "        // Satchel 的 CreateToggle 会在菜单构建完成后自动刷新，显示真实启用状态。\n"
    "        List<Element> toggles = [];\n"
    "        if (toggleDelegates.HasValue)\n"
    "        {\n"
    "            toggles.Add(toggleDelegates.Value.CreateToggle(\"控制器修正\", \"启用或关闭控制器修正模组\"));\n"
    "        }\n"
    "\n"
    "        MenuRef = new Menu(\"控制器修正\",\n"
    "        [\n"
    "            .. toggles,\n"
    "            new MenuButton(\"链接至说明\","
)

print("=== ControllerFixes.cs ===")
patch(F_MAIN, [(OLD_MAIN, NEW_MAIN, "总开关移入菜单 + 传入开关委托")])

print("=== Settings/ModMenu.cs ===")
patch(F_MENU, [(OLD_MENU, NEW_MENU, "菜单第一项加入总开关")])

print("\n补丁已应用")
