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

public class SimplifiedChineseFontMod : Mod, IMenuMod, IMod, Modding.ILogger, ITogglableMod
{
	[HarmonyPatch(typeof(ChangeFontByLanguage), "SetFont")]
	private static class ChangeFontByLanguagePatch
	{
		public static void Postfix(ChangeFontByLanguage __instance)
		{
			SimplifiedChineseFontMod instance = Instance;
			if (instance == null || instance._chineseFont == null || TmproField == null)
			{
				return;
			}
			try
			{
				object value = TmproField.GetValue(__instance);
				TextMeshPro textMeshPro = (TextMeshPro)((value is TextMeshPro) ? value : null);
				if ((UnityEngine.Object)(object)textMeshPro != null && global::Language.Language.CurrentLanguage().ToString() == "ZH")
				{
					textMeshPro.font = instance._chineseFont;
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("[简体中文字体替换] SetFont 补丁异常：" + ex);
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
			if (_chineseFont != null)
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

	public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
	{
		List<IMenuMod.MenuEntry> list = new List<IMenuMod.MenuEntry>();
		if (toggleButtonEntry.HasValue)
		{
			list.Add(toggleButtonEntry.Value);
		}
		list.Add(new IMenuMod.MenuEntry
		{
			Name = "简体中文字体替换",
			Description = "使用霞鹜臻楷替换简体中文渲染字体"
		});
		return list;
	}

	private void LoadFontIfNeeded()
	{
		if (_chineseFont != null)
		{
			return;
		}
		try
		{
			string text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "霞鹜臻楷.ttf");
			if (!File.Exists(text))
			{
				Log("字体文件不存在：" + text);
				return;
			}
			Log("正在构建简体中文字体资产，字体路径：" + text);
			(TMP_FontAsset, int) tuple = BuildChineseFontAsset(text);
			(_chineseFont, _) = tuple;
			if (_chineseFont == null)
			{
				Log("简体中文字体资产构建失败");
			}
			else
			{
				Log($"简体中文字体资产构建成功，包含 {_chineseFont.fontInfo.CharacterCount} 个字形（渲染了 {tuple.Item2} 个候选字形）");
			}
		}
		catch (Exception ex)
		{
			Log("构建字体资产时发生异常：" + ex);
		}
	}

	private (TMP_FontAsset fontAsset, int renderedCount) BuildChineseFontAsset(string fontPath)
	{
		FontEngineError fontEngineError = FontEngine.InitializeFontEngine();
		if (fontEngineError > FontEngineError.Success)
		{
			Log("FontEngine 初始化失败：" + fontEngineError);
			return (fontAsset: null, renderedCount: 0);
		}
		FontEngineError fontEngineError2 = FontEngine.LoadFontFace(fontPath, 36);
		if (fontEngineError2 > FontEngineError.Success)
		{
			Log("字体面加载失败：" + fontEngineError2);
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
		foreach (uint num in array)
		{
			uint glyphIndex = 0u;
			if (FontEngine.TryGetGlyphIndex(num, out glyphIndex))
			{
				list.Add(num);
				list2.Add(glyphIndex);
			}
		}
		Log($"共找到 {list.Count} 个可用字形码位");
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
			int count = Math.Min(list2.Count - j, 5000);
			List<uint> range = list.GetRange(j, count);
			List<uint> range2 = list2.GetRange(j, count);
			tuple = BuildSingleAtlas(range, range2, 36, 4096, 1);
			if (tuple.Item1 == null || tuple.Item2 == 0)
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
			uint glyphIndex = 0u;
			if (FontEngine.TryGetGlyphIndex(num, out glyphIndex))
			{
				codepoints.Add(num);
				glyphIndexes.Add(glyphIndex);
			}
		}
	}

	private (TMP_FontAsset asset, int renderedCount) BuildSingleAtlas(List<uint> codepoints, List<uint> glyphIndexes, int pointSize, int atlasSize, int padding)
	{
		MethodInfo tryAddGlyphsMethod = TryAddGlyphsMethod;
		if (tryAddGlyphsMethod == null)
		{
			Log("FontEngine.TryAddGlyphsToTexture 不可访问，无法生成图集");
			return (asset: null, renderedCount: 0);
		}
		Texture2D texture2D = new Texture2D(atlasSize, atlasSize, TextureFormat.Alpha8, mipChain: false);
		texture2D.filterMode = FilterMode.Bilinear;
		texture2D.wrapMode = TextureWrapMode.Clamp;
		List<GlyphRect> list = new List<GlyphRect>();
		List<GlyphRect> list2 = new List<GlyphRect>();
		object[] array = new object[8]
		{
			glyphIndexes,
			padding,
			GlyphPackingMode.BestShortSideFit,
			list,
			list2,
			GlyphRenderMode.SDF,
			texture2D,
			null
		};
		bool num = (bool)tryAddGlyphsMethod.Invoke(null, array);
		Glyph[] array2 = (Glyph[])array[7];
		if (!num || array2 == null || array2.Length == 0)
		{
			Log("图集渲染失败或无可用字形");
			return (asset: null, renderedCount: 0);
		}
		Log($"图集渲染完成：{array2.Length} / {glyphIndexes.Count} 个字形已编码到 {atlasSize}x{atlasSize} 图集");
		TMP_Glyph[] array3 = new TMP_Glyph[array2.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			Glyph obj = array2[i];
			array3[i] = new TMP_Glyph
			{
				id = (int)codepoints[i],
				x = obj.glyphRect.x,
				y = obj.glyphRect.y,
				width = obj.glyphRect.width,
				height = obj.glyphRect.height,
				xOffset = obj.metrics.horizontalBearingX,
				yOffset = obj.metrics.horizontalBearingY,
				xAdvance = obj.metrics.horizontalAdvance,
				scale = 1f
			};
		}
		UnityEngine.TextCore.FaceInfo faceInfo = FontEngine.GetFaceInfo();
		TMPro.FaceInfo faceInfo2 = new TMPro.FaceInfo();
		faceInfo2.Name = faceInfo.familyName;
		faceInfo2.PointSize = pointSize;
		faceInfo2.Scale = faceInfo.scale;
		faceInfo2.CharacterCount = array3.Length;
		faceInfo2.LineHeight = faceInfo.lineHeight;
		faceInfo2.Baseline = faceInfo.baseline;
		faceInfo2.Ascender = faceInfo.ascentLine;
		faceInfo2.CapHeight = faceInfo.capLine;
		faceInfo2.Descender = faceInfo.descentLine;
		faceInfo2.CenterLine = faceInfo.meanLine;
		faceInfo2.SuperscriptOffset = faceInfo.superscriptOffset;
		faceInfo2.SubscriptOffset = faceInfo.subscriptOffset;
		faceInfo2.SubSize = faceInfo.superscriptSize;
		faceInfo2.UnderlineThickness = 0f;
		faceInfo2.TabWidth = 10f * (float)pointSize;
		faceInfo2.Padding = padding;
		faceInfo2.AtlasWidth = atlasSize;
		faceInfo2.AtlasHeight = atlasSize;
		TMP_FontAsset tMP_FontAsset = ScriptableObject.CreateInstance<TMP_FontAsset>();
		tMP_FontAsset.AddFaceInfo(faceInfo2);
		tMP_FontAsset.AddGlyphInfo(array3);
		tMP_FontAsset.AddKerningInfo(new KerningTable());
		tMP_FontAsset.atlas = texture2D;
		tMP_FontAsset.fontAssetType = TMP_FontAsset.FontAssetTypes.SDF;
		tMP_FontAsset.fallbackFontAssets = new List<TMP_FontAsset>();
		TMP_FontAsset defaultFontAsset = TMP_FontAsset.defaultFontAsset;
		Material material;
		if (defaultFontAsset != null && defaultFontAsset.material != null)
		{
			material = new Material(defaultFontAsset.material);
		}
		else
		{
			Shader shader = Shader.Find("TextMeshPro/Distance Field");
			if (shader == null)
			{
				shader = Shader.Find("Standard");
			}
			material = new Material(shader);
		}
		material.mainTexture = texture2D;
		material.name = "简体中文字体替换-材质";
		tMP_FontAsset.material = material;
		return (asset: tMP_FontAsset, renderedCount: array2.Length);
	}

	private void InstallPatch()
	{
		if (!_patchInstalled)
		{
			_harmony = new Harmony("com.modding.simplifiedchinese.font");
			MethodInfo method = typeof(ChangeFontByLanguage).GetMethod("SetFont", BindingFlags.Instance | BindingFlags.Public);
			if (method == null)
			{
				Log("找不到 ChangeFontByLanguage.SetFont 方法");
				return;
			}
			_harmony.Patch(method, null, new HarmonyMethod(typeof(ChangeFontByLanguagePatch).GetMethod("Postfix", BindingFlags.Static | BindingFlags.Public)));
			_patchInstalled = true;
			Log("字体替换补丁已安装");
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
