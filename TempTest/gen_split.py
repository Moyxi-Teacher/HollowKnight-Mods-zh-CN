import pathlib, re

base = r"D:\KDQSHH\TempTest\decompile\HKMod\HollowKnightHaptics"

# ---------- HapticsPlugin.cs ----------
p = pathlib.Path(base, "HapticsPlugin.cs")
s = p.read_text(encoding='utf-8')
if "using System.Collections.Generic;" not in s:
    s = s.replace("using System;", "using System;\nusing System.Collections.Generic;\nusing System.Linq;\n")
fields_re = r"(?ms)^\t// 震动精细触觉倍率.*?\tinternal static ConfigEntry<float> MiscGain;.*?$"
fields_new = (
"\t// 每个震动剪辑一个独立倍率：启动时扫描 clips 目录 / 别名表 / 脚步表自动注册。\n"
"\t// key = WAV 文件名去掉 .wav 和 _vibration 后的剪辑名，value = 对应倍率配置（默认 1.0 = 基准版 0.1.2）。\n"
"\tinternal static readonly Dictionary<string, ConfigEntry<float>> ClipGains = new Dictionary<string, ConfigEntry<float>>(StringComparer.OrdinalIgnoreCase);\n"
"\tprivate static readonly Dictionary<string, string> clipDisplayNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)\n"
"\t{\n"
"\t\t{ \"hero_walk_footsteps_stone\", \"石质脚步\" },\n"
"\t\t{ \"hornet_walk_footsteps_grass\", \"草地脚步\" },\n"
"\t\t{ \"hornet_walk_footsteps_hard\", \"硬地脚步\" },\n"
"\t\t{ \"hornet_walk_footsteps_moss\", \"苔藓脚步\" },\n"
"\t\t{ \"hornet_walk_footsteps_metal\", \"金属脚步\" },\n"
"\t\t{ \"hornet_walk_footsteps_wet_metal\", \"湿金属脚步\" },\n"
"\t\t{ \"hero_shade_dash_2\", \"暗影冲刺\" },\n"
"\t\t{ \"hero_super_dash_air_brake\", \"超级冲刺空中取消\" },\n"
"\t\t{ \"hero_super_dash_burst\", \"超级冲刺迸发\" },\n"
"\t\t{ \"hero_dash\", \"普通冲刺\" },\n"
"\t\t{ \"hero_quake_spell_prepare\", \"地震术蓄力\" },\n"
"\t\t{ \"hero_quake_spell_dash\", \"地震术飞行\" },\n"
"\t\t{ \"hero_quake_spell_impact\", \"地震术落地\" },\n"
"\t\t{ \"hero_dream_nail_slash\", \"梦境钉\" },\n"
"\t\t{ \"hero_nail_art_cyclone_slash_short\", \"龙卷斩短\" },\n"
"\t\t{ \"hero_wall_jump\", \"墙跳\" },\n"
"\t\t{ \"hero_wall_slide\", \"墙滑\" },\n"
"\t};"
)
s = re.sub(fields_re, fields_new, s, flags=re.DOTALL|re.MULTILINE)
bind_re = r"(?ms)^\t\tStoneGain = Config\.Bind.*?\t\tMiscGain = Config\.Bind.*?$"
bind_new = "\t\t// 分类倍率由 RegisterClipGains() 在确定 clips 目录后自动注册。"
s = re.sub(bind_re, bind_new, s, flags=re.DOTALL|re.MULTILINE)
marker = "\t\t}\n\t\tif (AutoStartBridge.Value)"
if marker in s:
    s = s.replace(marker, "\t\t}\n\t\tRegisterClipGains(Config, text);\n\t\tif (AutoStartBridge.Value)")

helper = (
"""
\tprivate static void RegisterClipGains(BepInEx.Config config, string clipsDir)
\t{
\t\tClipGains.Clear();
\t\tHashSet<string> clipFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
\t\tforeach (string alias in HapticMixerWrapper.GetAliasTargets())
\t\t{
\t\t\tif (!string.IsNullOrEmpty(alias))
\t\t\t\tclipFiles.Add(alias);
\t\t}
\t\tforeach (string footstep in FootstepVibrationPatch.GetKnownClipNames())
\t\t{
\t\t\tif (!string.IsNullOrEmpty(footstep))
\t\t\t\tclipFiles.Add(footstep);
\t\t}
\t\tif (!string.IsNullOrEmpty(clipsDir) && Directory.Exists(clipsDir))
\t\t{
\t\t\tforeach (string file in Directory.GetFiles(clipsDir, "*.wav"))
\t\t\t{
\t\t\t\tstring baseName = Path.GetFileNameWithoutExtension(file);
\t\t\t\tif (!string.IsNullOrEmpty(baseName))
\t\t\t\t\tclipFiles.Add(baseName);
\t\t\t}
\t\t}
\t\tforeach (string clipFile in clipFiles)
\t\t{
\t\t\tstring key = GetClipKey(clipFile);
\t\t\tif (string.IsNullOrEmpty(key) || ClipGains.ContainsKey(key))
\t\t\t\tcontinue;
\t\t\tstring display = GetClipDisplayName(key);
\t\t\tClipGains[key] = config.Bind<float>("Haptics", "Gain_" + key, 1f, display + " 精细触觉倍率，1.0=基准。范围 0~3。");
\t\t}
\t}

\tinternal static float GetClipGain(string clipFile)
\t{
\t\tstring key = GetClipKey(clipFile);
\t\tif (ClipGains.TryGetValue(key, out ConfigEntry<float> entry) && entry != null)
\t\t\treturn entry.Value;
\t\treturn 1f;
\t}

\tinternal static string GetClipKey(string clipFile)
\t{
\t\tstring key = clipFile;
\t\tif (string.IsNullOrEmpty(key))
\t\t\treturn string.Empty;
\t\tif (key.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
\t\t\tkey = key.Substring(0, key.Length - 4);
\t\tif (key.EndsWith("_vibration", StringComparison.OrdinalIgnoreCase))
\t\t\tkey = key.Substring(0, key.Length - 10);
\t\treturn key;
\t}

\tinternal static IEnumerable<KeyValuePair<string, ConfigEntry<float>>> GetSortedClipGains()
\t{
\t\treturn ClipGains.OrderBy(kv => ClipSortGroup(kv.Key)).ThenBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase);
\t}

\tprivate static int ClipSortGroup(string key)
\t{
\t\tstring lower = key.ToLowerInvariant();
\t\tif (lower.Contains("footsteps")) return 0;
\t\tif (lower.Contains("wall") || lower.Contains("dash") || lower.Contains("jump")) return 1;
\t\tif (lower.Contains("spell") || lower.Contains("dream") || lower.Contains("nail") || lower.Contains("fireball") || lower.Contains("scream")) return 2;
\t\tif (lower.Contains("hurt") || lower.Contains("damage") || lower.Contains("hit") || lower.Contains("impact") || lower.Contains("slash") || lower.Contains("attack")) return 3;
\t\treturn 4;
\t}

\tinternal static string GetClipDisplayName(string clipFile)
\t{
\t\tstring key = GetClipKey(clipFile);
\t\tif (string.IsNullOrEmpty(key))
\t\t\treturn "未知震动";
\t\tif (clipDisplayNames.TryGetValue(key, out string label))
\t\t\treturn label;
\t\tstring fallback = key.Replace("_", " ").Trim();
\t\tif (string.IsNullOrEmpty(fallback))
\t\t\treturn key;
\t\treturn char.ToUpperInvariant(fallback[0]) + fallback.Substring(1);
\t}
"""
)
idx = s.rfind('}')
if idx != -1:
    s = s[:idx] + helper + "\n" + "}"
p.write_text(s, encoding='utf-8')
print("HapticsPlugin 改写完成")

# ---------- HapticMixerWrapper.cs ----------
p = pathlib.Path(base, "HapticMixerWrapper.cs")
s = p.read_text(encoding='utf-8')
cat_re = r"(?s)\t\t\t\t\t\t// 分类倍率：按 clip 名精确匹配各路面 / 非脚步（施法·受伤·墙跳·墙滑等）.*?\t\t\t\t\t\tstrength \*= categoryGain;"
cat_new = (
"\t\t\t\t\t\t// 每个震动独立倍率：按 WAV 基名去 _vibration 后查表，未注册则 1.0。\n"
"\t\t\t\t\t\tfloat categoryGain = clipFile != null ? HapticsPlugin.GetClipGain(clipFile) : 1f;\n"
"\t\t\t\t\t\tstrength *= categoryGain;"
)
s = re.sub(cat_re, cat_new, s, flags=re.DOTALL)
if "GetAliasTargets" not in s:
    m = re.search(r"(\tprivate static readonly Dictionary<string, string> clipAliases = new Dictionary<string, string>\s*\{.*?\}\s*;)", s, re.DOTALL)
    if m:
        insert = "\n\tinternal static IEnumerable<string> GetAliasTargets()\n\t{\n\t\treturn clipAliases.Values;\n\t}"
        s = s[:m.end()] + insert + s[m.end():]
p.write_text(s, encoding='utf-8')
print("HapticMixerWrapper 改写完成")

# ---------- FootstepVibrationPatch.cs ----------
p = pathlib.Path(base, "FootstepVibrationPatch.cs")
s = p.read_text(encoding='utf-8')
if "GetKnownClipNames" not in s:
    m = re.search(r'(private const string WalkClipName = "[^"]+";)', s)
    if m:
        insert = (
"\n    internal static IEnumerable<string> GetKnownClipNames()\n"
"    {\n"
"        foreach (var name in EnvTypeRunClipNames)\n"
"        {\n"
"            if (!string.IsNullOrEmpty(name))\n"
"                yield return name;\n"
"        }\n"
"        yield return WalkClipName;\n"
"    }\n"
        )
        s = s[:m.end()] + insert + s[m.end():]
p.write_text(s, encoding='utf-8')
print("FootstepVibrationPatch 改写完成")

# ---------- HapticsMenu.cs ----------
new_menu = (
"using UnityEngine;\n"
"using UnityEngine.UI;\n"
"using System.Collections.Generic;\n"
"using BepInEx.Configuration;\n"
"\n"
"namespace HollowKnightHaptics;\n"
"\n"
"public class HapticsMenu : MonoBehaviour\n"
"{\n"
"    private Canvas _canvas;\n"
"    private GameObject _panel;\n"
"    private RectTransform _content;\n"
"    private bool _visible;\n"
"    private readonly KeyCode _toggleKey = KeyCode.F6;\n"
"\n"
"    void Awake() { CreateUI(); }\n"
"\n"
"    void Update()\n"
"    {\n"
"        if (Input.GetKeyDown(_toggleKey))\n"
"        {\n"
"            if (UnityEngine.EventSystems.EventSystem.current == null)\n"
"            {\n"
"                var es = new GameObject(\"HapticsEventSystem\");\n"
"                DontDestroyOnLoad(es);\n"
"                es.AddComponent<UnityEngine.EventSystems.EventSystem>();\n"
"                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();\n"
"            }\n"
"            _visible = !_visible;\n"
"            _panel.SetActive(_visible);\n"
"        }\n"
"    }\n"
"\n"
"    private void CreateUI()\n"
"    {\n"
"        var go = new GameObject(\"HapticsMenuCanvas\");\n"
"        DontDestroyOnLoad(go);\n"
"        _canvas = go.AddComponent<Canvas>();\n"
"        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;\n"
"        _canvas.sortingOrder = 9999;\n"
"        var scaler = go.AddComponent<CanvasScaler>();\n"
"        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;\n"
"        scaler.referenceResolution = new Vector2(1920,1080);\n"
"        scaler.matchWidthOrHeight = 0.5f;\n"
"        go.AddComponent<GraphicRaycaster>();\n"
"\n"
"        _panel = new GameObject(\"Panel\");\n"
"        _panel.transform.SetParent(go.transform,false);\n"
"        var rt = _panel.AddComponent<RectTransform>();\n"
"        rt.anchorMin = new Vector2(0.5f,0.5f);\n"
"        rt.anchorMax = new Vector2(0.5f,0.5f);\n"
"        rt.pivot = new Vector2(0.5f,0.5f);\n"
"        rt.sizeDelta = new Vector2(460f, 880f);\n"
"\n"
"        var bg = _panel.AddComponent<Image>();\n"
"        bg.color = new Color(0.07f,0.07f,0.09f,0.96f);\n"
"\n"
"        var scroll = _panel.AddComponent<ScrollRect>();\n"
"        scroll.horizontal = false;\n"
"        scroll.vertical = true;\n"
"        scroll.movementType = ScrollRect.MovementType.Clamped;\n"
"        scroll.scrollSensitivity = 50f;\n"
"\n"
"        var viewport = new GameObject(\"Viewport\");\n"
"        viewport.transform.SetParent(_panel.transform,false);\n"
"        var vrt = viewport.AddComponent<RectTransform>();\n"
"        vrt.anchorMin = Vector2.zero;\n"
"        vrt.anchorMax = Vector2.one;\n"
"        vrt.pivot = new Vector2(0.5f,0.5f);\n"
"        vrt.offsetMin = new Vector2(10f,10f);\n"
"        vrt.offsetMax = new Vector2(-10f,-10f);\n"
"        viewport.AddComponent<RectMask2D>();\n"
"        scroll.viewport = vrt;\n"
"\n"
"        var contentGo = new GameObject(\"Content\");\n"
"        contentGo.transform.SetParent(viewport.transform,false);\n"
"        _content = contentGo.AddComponent<RectTransform>();\n"
"        _content.anchorMin = new Vector2(0f,1f);\n"
"        _content.anchorMax = new Vector2(1f,1f);\n"
"        _content.pivot = new Vector2(0.5f,1f);\n"
"        _content.sizeDelta = Vector2.zero;\n"
"\n"
"        var layout = contentGo.AddComponent<VerticalLayoutGroup>();\n"
"        layout.padding = new RectOffset(14,14,14,14);\n"
"        layout.spacing = 8;\n"
"        layout.childAlignment = TextAnchor.UpperCenter;\n"
"        layout.childControlWidth = true;\n"
"        layout.childForceExpandWidth = true;\n"
"        layout.childControlHeight = false;\n"
"\n"
"        var fitter = contentGo.AddComponent<ContentSizeFitter>();\n"
"        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;\n"
"        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;\n"
"\n"
"        scroll.content = _content;\n"
"\n"
"        AddTitle(\"PS5 触觉 参数\");\n"
"        AddSlider(\"主增益\", HapticsPlugin.MasterGain, 0f, 2f);\n"
"\n"
"        foreach (var kv in HapticsPlugin.GetSortedClipGains())\n"
"        {\n"
"            string label = HapticsPlugin.GetClipDisplayName(kv.Key);\n"
"            AddSlider(label, kv.Value, 0f, 3f);\n"
"        }\n"
"\n"
"        _panel.SetActive(false);\n"
"    }\n"
"\n"
"    private void AddTitle(string title)\n"
"    {\n"
"        var go = new GameObject(\"Title\");\n"
"        go.transform.SetParent(_content,false);\n"
"        var t = go.AddComponent<Text>();\n"
"        t.text = title;\n"
"        t.font = Resources.GetBuiltinResource<Font>(\"Arial.ttf\");\n"
"        t.fontSize = 20;\n"
"        t.alignment = TextAnchor.MiddleCenter;\n"
"        t.color = Color.white;\n"
"        var trt = go.GetComponent<RectTransform>();\n"
"        trt.sizeDelta = new Vector2(0f,34f);\n"
"    }\n"
"\n"
"    private void AddSlider(string label, ConfigEntry<float> cfg, float min, float max)\n"
"    {\n"
"        var row = new GameObject(\"Row_\" + label);\n"
"        row.transform.SetParent(_content,false);\n"
"        var rowRT = row.AddComponent<RectTransform>();\n"
"        rowRT.sizeDelta = new Vector2(0f,52f);\n"
"\n"
"        var vgroup = row.AddComponent<VerticalLayoutGroup>();\n"
"        vgroup.spacing = 3;\n"
"        vgroup.childControlWidth = true;\n"
"        vgroup.childForceExpandWidth = true;\n"
"\n"
"        var lbl = new GameObject(\"Label\");\n"
"        lbl.transform.SetParent(row.transform,false);\n"
"        var lblT = lbl.AddComponent<Text>();\n"
"        lblT.text = label + \": \" + cfg.Value.ToString(\"F2\");\n"
"        lblT.font = Resources.GetBuiltinResource<Font>(\"Arial.ttf\");\n"
"        lblT.fontSize = 14;\n"
"        lblT.color = Color.white;\n"
"        var lblRT = lbl.GetComponent<RectTransform>();\n"
"        lblRT.sizeDelta = new Vector2(0f,18f);\n"
"\n"
"        var sliderObj = new GameObject(\"Slider\");\n"
"        sliderObj.transform.SetParent(row.transform,false);\n"
"        var slider = sliderObj.AddComponent<Slider>();\n"
"        slider.minValue = min;\n"
"        slider.maxValue = max;\n"
"        slider.value = cfg.Value;\n"
"        slider.wholeNumbers = false;\n"
"\n"
"        var sliderImg = sliderObj.AddComponent<Image>();\n"
"        sliderImg.color = new Color(0.2f,0.2f,0.2f);\n"
"\n"
"        var fillObj = new GameObject(\"Fill\");\n"
"        fillObj.transform.SetParent(slider.transform,false);\n"
"        var fillImg = fillObj.AddComponent<Image>();\n"
"        fillImg.color = new Color(0.4f,0.7f,1f);\n"
"        var fillRT = fillObj.GetComponent<RectTransform>();\n"
"        fillRT.anchorMin = Vector2.zero;\n"
"        fillRT.anchorMax = Vector2.one;\n"
"        fillRT.offsetMin = Vector2.zero;\n"
"        fillRT.offsetMax = Vector2.zero;\n"
"        slider.fillRect = fillRT;\n"
"\n"
"        var handleObj = new GameObject(\"Handle\");\n"
"        handleObj.transform.SetParent(slider.transform,false);\n"
"        var handleImg = handleObj.AddComponent<Image>();\n"
"        handleImg.color = Color.white;\n"
"        var handleRT = handleObj.GetComponent<RectTransform>();\n"
"        handleRT.sizeDelta = new Vector2(20f,20f);\n"
"        slider.handleRect = handleRT;\n"
"\n"
"        var valueObj = new GameObject(\"Value\");\n"
"        valueObj.transform.SetParent(row.transform,false);\n"
"        var valT = valueObj.AddComponent<Text>();\n"
"        valT.text = cfg.Value.ToString(\"F2\");\n"
"        valT.font = Resources.GetBuiltinResource<Font>(\"Arial.ttf\");\n"
"        valT.fontSize = 12;\n"
"        valT.color = Color.gray;\n"
"        valT.alignment = TextAnchor.MiddleLeft;\n"
"        var valRT = valT.GetComponent<RectTransform>();\n"
"        valRT.sizeDelta = new Vector2(0f,16f);\n"
"\n"
"        slider.onValueChanged.AddListener(v =>\n"
"        {\n"
"            cfg.Value = v;\n"
"            lblT.text = label + \": \" + v.ToString(\"F2\");\n"
"            valT.text = v.ToString(\"F2\");\n"
"        });\n"
"    }\n"
"}\n"
)
pathlib.Path(base, "HapticsMenu.cs").write_text(new_menu, encoding='utf-8')
print("HapticsMenu 改写完成")
