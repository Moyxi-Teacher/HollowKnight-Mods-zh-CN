# -*- coding: utf-8 -*-
"""控制器修正（ControllerFixes）—— 添加模组总开关。

改动：
  1) 类声明实现 ITogglableMod；
  2) 新增 Unload()，把 Initialize() 里注册的 6 个钩子全部反注册，
     使关闭模组后功能真正停止。
说明：该类 ToggleButtonInsideMenu => false，因此总开关会显示在「模组列表」里，
      不需要改动菜单本身；GetMenuScreen 的第二个参数保持忽略即可。
"""
import os

SRC = r"D:\KDQSHH\ModsWork\ControllerFixes-HEAD\ControllerFixes\ControllerFixes.cs"


def load(path):
    raw = open(path, "rb").read()
    has_bom = raw[:3] == b"\xef\xbb\xbf"
    s = raw.decode("utf-8-sig" if has_bom else "utf-8")
    return s.replace("\r\n", "\n"), has_bom, (s.count("\r\n") > 0)


def save(path, text, has_bom, crlf):
    if crlf:
        text = text.replace("\n", "\r\n")
    data = text.encode("utf-8")
    open(path, "wb").write((b"\xef\xbb\xbf" if has_bom else b"") + data)


s, has_bom, crlf = load(SRC)
print("BOM=%s CRLF=%s" % (has_bom, crlf))

edits = [
    (
        "public class ControllerFixes : Mod, IGlobalSettings<GlobalSettings>, ICustomMenuMod",
        "public class ControllerFixes : Mod, IGlobalSettings<GlobalSettings>, ICustomMenuMod, ITogglableMod",
        "类声明实现 ITogglableMod",
    ),
    (
        """    public bool ToggleButtonInsideMenu => false;
}""",
        """    public bool ToggleButtonInsideMenu => false;

    // 总开关(2026-09-19)：实现 ITogglableMod。
    // 由于 ToggleButtonInsideMenu 为 false，开关显示在「模组列表」中；
    // 关闭时反注册 Initialize 里注册的全部钩子，确保功能彻底停止。
    public void Unload()
    {
        On.ControllerDetect.LookForActiveController -= LookForActiveController;
        On.HeroController.CanCast -= CanCast;
        ILInputManager.AttachDevice -= AttachDeviceIL;
        ILUIButtonSkins.orig_GetButtonSkinFor -= GetButtonSkinForIL;
        On.ControllerButtonLabel.ShowCurrentBinding -= ShowCurrentBinding;
        On.ActionButtonIconBase.GetButtonIcon -= OverrideMenuPrompts;
    }
}""",
        "新增 Unload() 反注册全部钩子",
    ),
]

for i, (old, new, tag) in enumerate(edits, 1):
    assert old in s, "锚点 %d 未找到: %s" % (i, tag)
    assert s.count(old) == 1, "锚点 %d 不唯一(%d): %s" % (i, s.count(old), tag)
    s = s.replace(old, new, 1)
    print("  [ok] " + tag)

save(SRC, s, has_bom, crlf)
print("补丁已应用")
