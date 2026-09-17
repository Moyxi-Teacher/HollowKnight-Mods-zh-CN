using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;
using Language;
using Modding;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;

namespace SimplifiedChineseFont;

public class SimplifiedChineseFontMod : Mod, IMenuMod, IMod, ILogger, ITogglableMod
{
	[HarmonyPatch(typeof(ChangeFontByLanguage), "SetFont")]
	private static class ChangeFontByLanguagePatch
	{
		public static void Postfix(ChangeFontByLanguage __instance)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			SimplifiedChineseFontMod instance = Instance;
			if (instance == null || (Object)(object)instance._chineseFont == (Object)null || TmproField == null)
			{
				return;
			}
			try
			{
				object value = TmproField.GetValue(__instance);
				TextMeshPro val = (TextMeshPro)((value is TextMeshPro) ? value : null);
				if ((Object)(object)val != (Object)null && ((object)Language.CurrentLanguage()/*cast due to constrained. prefix*/).ToString() == "ZH")
				{
					((TMP_Text)val).font = instance._chineseFont;
				}
			}
			catch (Exception ex)
			{
				Debug.LogError((object)("[简体中文字体替换] SetFont 补丁异常：" + ex));
			}
		}
	}

	internal static SimplifiedChineseFontMod Instance;

	private TMP_FontAsset _chineseFont;

	private bool _patchInstalled;

	private Harmony _harmony;

	private static readonly FieldInfo TmproField = typeof(ChangeFontByLanguage).GetField("tmpro", BindingFlags.Instance | BindingFlags.NonPublic);

	private static readonly MethodInfo TryAddGlyphsMethod = typeof(FontEngine).GetMethod("TryAddGlyphsToTexture", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[8]
	{
		typeof(List<uint>),
		typeof(int),
		typeof(GlyphPackingMode),
		typeof(List<GlyphRect>),
		typeof(List<GlyphRect>),
		typeof(GlyphRenderMode),
		typeof(Texture2D),
		typeof(Glyph[]).MakeByRefType()
	}, null);

	public bool ToggleButtonInsideMenu => true;

	public SimplifiedChineseFontMod()
		: base("简体中文字体替换")
	{
		Instance = this;
	}

	public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
	{
		if (!_patchInstalled)
		{
			LoadFontIfNeeded();
			if ((Object)(object)_chineseFont != (Object)null)
			{
				InstallPatch();
			}
		}
	}

	public void Unload()
	{
		UninstallPatch();
	}

	public override string GetVersion()
	{
		return "1.0.0";
	}

	public List<MenuEntry> GetMenuData(MenuEntry? toggleButtonEntry)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		List<MenuEntry> list = new List<MenuEntry>();
		if (toggleButtonEntry.HasValue)
		{
			list.Add(toggleButtonEntry.Value);
		}
		list.Add(new MenuEntry
		{
			Name = "简体中文字体替换",
			Description = "使用霞鹜臻楷替换简体中文渲染字体"
		});
		return list;
	}

	private void LoadFontIfNeeded()
	{
		if ((Object)(object)_chineseFont != (Object)null)
		{
			return;
		}
		try
		{
			string text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "霞鹜臻楷.ttf");
			if (!File.Exists(text))
			{
				((Loggable)this).Log("字体文件不存在：" + text);
				return;
			}
			((Loggable)this).Log("正在构建简体中文字体资产，字体路径：" + text);
			(TMP_FontAsset, int) tuple = BuildChineseFontAsset(text);
			(_chineseFont, _) = tuple;
			if ((Object)(object)_chineseFont == (Object)null)
			{
				((Loggable)this).Log("简体中文字体资产构建失败");
			}
			else
			{
				((Loggable)this).Log($"简体中文字体资产构建成功，包含 {_chineseFont.fontInfo.CharacterCount} 个字形（渲染了 {tuple.Item2} 个候选字形）");
			}
		}
		catch (Exception ex)
		{
			((Loggable)this).Log("构建字体资产时发生异常：" + ex);
		}
	}

	private unsafe (TMP_FontAsset fontAsset, int renderedCount) BuildChineseFontAsset(string fontPath)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Invalid comparison between Unknown and I4
		FontEngineError val = FontEngine.InitializeFontEngine();
		if ((int)val > 0)
		{
			((Loggable)this).Log("FontEngine 初始化失败：" + ((object)(*(FontEngineError*)(&val))/*cast due to constrained. prefix*/).ToString());
			return (fontAsset: null, renderedCount: 0);
		}
		FontEngineError val2 = FontEngine.LoadFontFace(fontPath, 36);
		if ((int)val2 > 0)
		{
			((Loggable)this).Log("字体面加载失败：" + ((object)(*(FontEngineError*)(&val2))/*cast due to constrained. prefix*/).ToString());
			return (fontAsset: null, renderedCount: 0);
		}
		List<uint> list = new List<uint>();
		List<uint> list2 = new List<uint>();
		CollectCodepoints(19968u, 40959u, list, list2);
		CollectCodepoints(12288u, 12351u, list, list2);
		CollectCodepoints(65280u, 65519u, list, list2);
		CollectCodepoints(32u, 127u, list, list2);
		uint[] array = new uint[15]
		{
			8212u, 8211u, 8230u, 183u, 8220u, 8221u, 8216u, 8217u, 169u, 174u,
			182u, 161u, 191u, 215u, 247u
		};
		uint[] array2 = array;
		foreach (uint num in array2)
		{
			uint item = 0u;
			if (FontEngine.TryGetGlyphIndex(num, ref item))
			{
				list.Add(num);
				list2.Add(item);
			}
		}
		((Loggable)this).Log($"共找到 {list.Count} 个可用字形码位");
		if (list.Count == 0)
		{
			return (fontAsset: null, renderedCount: 0);
		}
		List<TMP_FontAsset> list3 = new List<TMP_FontAsset>();
		int num2 = 0;
		int j = 0;
		int num3 = 0;
		(TMP_FontAsset, int) tuple;
		for (; j < list2.Count; j += tuple.Item2)
		{
			if (num3 >= 200)
			{
				break;
			}
			num3++;
			int val3 = list2.Count - j;
			int count = Math.Min(val3, 5000);
			List<uint> range = list.GetRange(j, count);
			List<uint> range2 = list2.GetRange(j, count);
			tuple = BuildSingleAtlas(range, range2, 36, 4096, 1);
			if ((Object)(object)tuple.Item1 == (Object)null || tuple.Item2 == 0)
			{
				break;
			}
			list3.Add(tuple.Item1);
			num2 += tuple.Item2;
		}
		if (list3.Count == 0)
		{
			return (fontAsset: null, renderedCount: 0);
		}
		for (int k = 0; k < list3.Count - 1; k++)
		{
			if (list3[k].fallbackFontAssets == null)
			{
				list3[k].fallbackFontAssets = new List<TMP_FontAsset>();
			}
			list3[k].fallbackFontAssets.Add(list3[k + 1]);
		}
		return (fontAsset: list3[0], renderedCount: num2);
	}

	private void CollectCodepoints(uint start, uint end, List<uint> codepoints, List<uint> glyphIndexes)
	{
		for (uint num = start; num <= end; num++)
		{
			uint item = 0u;
			if (FontEngine.TryGetGlyphIndex(num, ref item))
			{
				codepoints.Add(num);
				glyphIndexes.Add(item);
			}
		}
	}

	private (TMP_FontAsset asset, int renderedCount) BuildSingleAtlas(List<uint> codepoints, List<uint> glyphIndexes, int pointSize, int atlasSize, int padding)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Expected O, but got Unknown
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Expected O, but got Unknown
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Expected O, but got Unknown
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Expected O, but got Unknown
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Expected O, but got Unknown
		MethodInfo tryAddGlyphsMethod = TryAddGlyphsMethod;
		if (tryAddGlyphsMethod == null)
		{
			((Loggable)this).Log("FontEngine.TryAddGlyphsToTexture 不可访问，无法生成图集");
			return (asset: null, renderedCount: 0);
		}
		Texture2D val = new Texture2D(atlasSize, atlasSize, (TextureFormat)4, false);
		((Texture)val).filterMode = (FilterMode)1;
		((Texture)val).wrapMode = (TextureWrapMode)1;
		List<GlyphRect> list = new List<GlyphRect>();
		List<GlyphRect> list2 = new List<GlyphRect>();
		object[] array = new object[8]
		{
			glyphIndexes,
			padding,
			(object)(GlyphPackingMode)0,
			list,
			list2,
			(object)(GlyphRenderMode)4134,
			val,
			null
		};
		bool flag = (bool)TryAddGlyphsMethod.Invoke(null, array);
		Glyph[] array2 = (Glyph[])array[7];
		if (!flag || array2 == null || array2.Length == 0)
		{
			((Loggable)this).Log("图集渲染失败或无可用字形");
			return (asset: null, renderedCount: 0);
		}
		((Loggable)this).Log($"图集渲染完成：{array2.Length} / {glyphIndexes.Count} 个字形已编码到 {atlasSize}x{atlasSize} 图集");
		TMP_Glyph[] array3 = (TMP_Glyph[])(object)new TMP_Glyph[array2.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			Glyph val2 = array2[i];
			TMP_Glyph val3 = new TMP_Glyph();
			((TMP_TextElement)val3).id = (int)codepoints[i];
			GlyphRect glyphRect = val2.glyphRect;
			((TMP_TextElement)val3).x = ((GlyphRect)(ref glyphRect)).x;
			glyphRect = val2.glyphRect;
			((TMP_TextElement)val3).y = ((GlyphRect)(ref glyphRect)).y;
			glyphRect = val2.glyphRect;
			((TMP_TextElement)val3).width = ((GlyphRect)(ref glyphRect)).width;
			glyphRect = val2.glyphRect;
			((TMP_TextElement)val3).height = ((GlyphRect)(ref glyphRect)).height;
			GlyphMetrics metrics = val2.metrics;
			((TMP_TextElement)val3).xOffset = ((GlyphMetrics)(ref metrics)).horizontalBearingX;
			metrics = val2.metrics;
			((TMP_TextElement)val3).yOffset = ((GlyphMetrics)(ref metrics)).horizontalBearingY;
			metrics = val2.metrics;
			((TMP_TextElement)val3).xAdvance = ((GlyphMetrics)(ref metrics)).horizontalAdvance;
			((TMP_TextElement)val3).scale = 1f;
			array3[i] = val3;
		}
		FaceInfo faceInfo = FontEngine.GetFaceInfo();
		FaceInfo val4 = new FaceInfo();
		val4.Name = ((FaceInfo)(ref faceInfo)).familyName;
		val4.PointSize = pointSize;
		val4.Scale = ((FaceInfo)(ref faceInfo)).scale;
		val4.CharacterCount = array3.Length;
		val4.LineHeight = ((FaceInfo)(ref faceInfo)).lineHeight;
		val4.Baseline = ((FaceInfo)(ref faceInfo)).baseline;
		val4.Ascender = ((FaceInfo)(ref faceInfo)).ascentLine;
		val4.CapHeight = ((FaceInfo)(ref faceInfo)).capLine;
		val4.Descender = ((FaceInfo)(ref faceInfo)).descentLine;
		val4.CenterLine = ((FaceInfo)(ref faceInfo)).meanLine;
		val4.SuperscriptOffset = ((FaceInfo)(ref faceInfo)).superscriptOffset;
		val4.SubscriptOffset = ((FaceInfo)(ref faceInfo)).subscriptOffset;
		val4.SubSize = ((FaceInfo)(ref faceInfo)).superscriptSize;
		val4.UnderlineThickness = 0f;
		val4.TabWidth = 10f * (float)pointSize;
		val4.Padding = padding;
		val4.AtlasWidth = atlasSize;
		val4.AtlasHeight = atlasSize;
		TMP_FontAsset val5 = ScriptableObject.CreateInstance<TMP_FontAsset>();
		val5.AddFaceInfo(val4);
		val5.AddGlyphInfo(array3);
		val5.AddKerningInfo(new KerningTable());
		val5.atlas = val;
		val5.fontAssetType = (FontAssetTypes)1;
		val5.fallbackFontAssets = new List<TMP_FontAsset>();
		TMP_FontAsset defaultFontAsset = TMP_FontAsset.defaultFontAsset;
		Material val6;
		if ((Object)(object)defaultFontAsset != (Object)null && (Object)(object)((TMP_Asset)defaultFontAsset).material != (Object)null)
		{
			val6 = new Material(((TMP_Asset)defaultFontAsset).material);
		}
		else
		{
			Shader val7 = Shader.Find("TextMeshPro/Distance Field");
			if ((Object)(object)val7 == (Object)null)
			{
				val7 = Shader.Find("Standard");
			}
			val6 = new Material(val7);
		}
		val6.mainTexture = (Texture)(object)val;
		((Object)val6).name = "简体中文字体替换-材质";
		((TMP_Asset)val5).material = val6;
		Dictionary<int, TMP_Glyph> characterDictionary = val5.characterDictionary;
		return (asset: val5, renderedCount: array2.Length);
	}

	private void InstallPatch()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		if (!_patchInstalled)
		{
			_harmony = new Harmony("com.modding.simplifiedchinese.font");
			MethodInfo method = typeof(ChangeFontByLanguage).GetMethod("SetFont", BindingFlags.Instance | BindingFlags.Public);
			if (method == null)
			{
				((Loggable)this).Log("找不到 ChangeFontByLanguage.SetFont 方法");
				return;
			}
			_harmony.Patch((MethodBase)method, (HarmonyMethod)null, new HarmonyMethod(typeof(ChangeFontByLanguagePatch).GetMethod("Postfix", BindingFlags.Static | BindingFlags.Public)), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
			_patchInstalled = true;
			((Loggable)this).Log("字体替换补丁已安装");
		}
	}

	private void UninstallPatch()
	{
		if (_patchInstalled && _harmony != null)
		{
			_harmony.UnpatchSelf();
			_patchInstalled = false;
		}
	}
}
