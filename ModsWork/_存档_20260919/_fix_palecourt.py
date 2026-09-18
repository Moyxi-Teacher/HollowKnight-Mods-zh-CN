# -*- coding: utf-8 -*-
"""苍白宫廷修复补丁：让「强制菜单主题」和「调试模式」可以正常关闭并保存。

改动三处：
  1) FiveKnights.cs —— Initialize() 里删掉「发现是关就强制改回开」的逻辑，
     改为「只有玩家设为开时才切换主题」。
  2) FiveKnights.cs —— 「调试模式」菜单项接入 SaveModSettings.IsDebug。
  3) FiveKnights.cs —— 存档加载后把 IsDebug 同步到静态字段 isDebug。
  4) Misc/SaveModSettings.cs —— 新增字段 IsDebug（默认 true，保留原行为）。
"""
import io, os, sys

PROJ = r"D:\KDQSHH\ModsWork\PaleCourt-master"
f5   = os.path.join(PROJ, "FiveKnights.cs")
fsv  = os.path.join(PROJ, "Misc", "SaveModSettings.cs")


def load(path, bom):
    """读取源码：归一化为 LF，记录原换行风格"""
    raw = open(path, "rb").read()
    has_bom = raw[:3] == b"\xef\xbb\xbf"
    s = raw.decode("utf-8-sig" if has_bom else "utf-8")
    crlf = s.count("\r\n")
    return s.replace("\r\n", "\n"), has_bom, (crlf > 0)


def save(path, text, has_bom, use_crlf):
    if use_crlf:
        text = text.replace("\n", "\r\n")
    data = text.encode("utf-8")
    if has_bom:
        data = b"\xef\xbb\xbf" + data
    open(path, "wb").write(data)


def patch(path, edits, bom_ok=True):
    s, has_bom, use_crlf = load(path, bom_ok)
    for i, (old, new, tag) in enumerate(edits, 1):
        assert old in s, "锚点 %d 未找到: %s" % (i, tag)
        assert s.count(old) == 1, "锚点 %d 不唯一(%d 处): %s" % (i, s.count(old), tag)
        s = s.replace(old, new, 1)
        print("  [ok] " + tag)
    save(path, s, has_bom, use_crlf)


print("=== 1) FiveKnights.cs ===")
patch(f5, [
    # --- 修复①：强制菜单主题 —— 改成「仅当设置为开时才切换」 ---
    (
        """            if (!GlobalSettings.ForcedMenuTheme)
            {
                GlobalSettings.ForcedMenuTheme = true;
                var tmpStyle = MenuStyles.Instance.styles.First(x => x.styleObject.name.Contains("Pale_Court"));
                MenuStyles.Instance.SetStyle(MenuStyles.Instance.styles.ToList().IndexOf(tmpStyle), false);
            }""",
        """            // 修复(2026-09-19)：原版在此处只要发现玩家把「强制使用模组菜单主题」设为关，
            // 就立即改回开并强制切换主题，导致该选项永远无法关闭。
            // 现改为：只有玩家把它设为「开」时才切换主题。
            if (GlobalSettings.ForcedMenuTheme)
            {
                var tmpStyle = MenuStyles.Instance.styles.First(x => x.styleObject.name.Contains("Pale_Court"));
                MenuStyles.Instance.SetStyle(MenuStyles.Instance.styles.ToList().IndexOf(tmpStyle), false);
            }""",
        "强制菜单主题：去掉自动改回「开」的逻辑",
    ),
    # --- 修复②：调试模式接入存档 ---
    (
        """                "调试模式",
                new[] { "关", "开" },
                "启用调试模式。",
                index => isDebug = index == 1,
                () => isDebug ? 1 : 0));""",
        """                "调试模式",
                new[] { "关", "开" },
                "启用调试模式。",
                index =>
                {
                    SaveSettings.IsDebug = index == 1;
                    isDebug = index == 1;
                },
                () => SaveSettings.IsDebug ? 1 : 0));""",
        "调试模式：菜单读写改为存档字段",
    ),
    # --- 修复③：存档加载后同步 isDebug ---
    (
        """        private void AfterSavegameLoadHookHandler(SaveGameData saveGameData)
        {
            StartGame();
        }""",
        """        private void AfterSavegameLoadHookHandler(SaveGameData saveGameData)
        {
            // 修复(2026-09-19)：把存档里保存的调试模式开关同步到静态字段
            isDebug = SaveSettings.IsDebug;
            StartGame();
        }""",
        "调试模式：存档加载后同步静态字段",
    ),
])

print("=== 2) Misc/SaveModSettings.cs ===")
patch(fsv, [
    (
        """        public bool GreetedNailsmith = false;
        public bool GreetedSheo = false;""",
        """        public bool GreetedNailsmith = false;
        public bool GreetedSheo = false;

        // 修复(2026-09-19)：新增「调试模式」存档字段。
        // 原版该项只写内存静态字段 isDebug（默认 true），从不落盘，重启必然回到默认值。
        // 默认值保持 true，以保证老存档（无此字段）的初始行为与原版一致。
        public bool IsDebug = true;""",
        "新增 IsDebug 存档字段",
    ),
])

print("\n全部补丁已应用")
