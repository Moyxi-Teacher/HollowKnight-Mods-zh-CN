using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using CustomKnight;
using Microsoft.CodeAnalysis;
using Modding;
using Modding.Delegates;
using MonoMod.ModInterop;
using Newtonsoft.Json;
using On;
using Satchel;
using Satchel.BetterMenus;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: AssemblyCompany("EnemyHPBar")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyCopyright("Copyright © 2018-2022")]
[assembly: AssemblyDescription("A Hollow Knight mod adding a customizable HP bar for all enemies and bosses")]
[assembly: AssemblyFileVersion("3.1.5.0")]
[assembly: AssemblyInformationalVersion("3.1.5.0汉化")]
[assembly: AssemblyProduct("EnemyHPBar")]
[assembly: AssemblyTitle("EnemyHPBar")]
[assembly: AssemblyVersion("3.1.5.0")]
[module: RefSafetyRules(11)]
namespace Microsoft.CodeAnalysis
{
	[CompilerGenerated]
	[Embedded]
	internal sealed class EmbeddedAttribute : Attribute
	{
	}
}
namespace System.Runtime.CompilerServices
{
	[CompilerGenerated]
	[Embedded]
	[AttributeUsage(AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
	internal sealed class RefSafetyRulesAttribute : Attribute
	{
		public readonly int Version;

		public RefSafetyRulesAttribute(int P_0)
		{
			Version = P_0;
		}
	}
}
namespace EnemyHPBar
{
	public class AnimJson
	{
		public static Dictionary<string, HPBarAnimation> animDict = new Dictionary<string, HPBarAnimation>();

		public static string FilterExtension(string name)
		{
			return name.Replace(".png", "");
		}

		public static void SaveAnimConfig(HPBarAnimation anim, string name)
		{
			FileStream fileStream = new FileStream(Path.Combine(EnemyHPBar.SkinPath, name + ".json"), FileMode.Create, FileAccess.Write);
			StreamWriter streamWriter = new StreamWriter(fileStream);
			streamWriter.Write(JsonConvert.SerializeObject((object)anim));
			streamWriter.Close();
			fileStream.Close();
		}

		public static HPBarAnimation LoadAnimConfig(string path)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
			StreamReader streamReader = new StreamReader(fileStream);
			HPBarAnimation result = JsonConvert.DeserializeObject<HPBarAnimation>(streamReader.ReadToEnd(), new JsonSerializerSettings
			{
				ObjectCreationHandling = (ObjectCreationHandling)2
			});
			streamReader.Close();
			fileStream.Close();
			return result;
		}

		public static bool ConfigExist(HPBarAnimation anim, string name)
		{
			return File.Exists(Path.Combine(EnemyHPBar.SkinPath, name + ".json"));
		}

		public static void Initdic()
		{
			animDict["ol"] = CustomHPBarAnimation.LoadAnimation(ResourceLoader.GetAllImages("ol.png"));
			animDict["fg"] = CustomHPBarAnimation.LoadAnimation(ResourceLoader.GetAllImages("fg.png"));
			animDict["bg"] = CustomHPBarAnimation.LoadAnimation(ResourceLoader.GetAllImages("bg.png"));
			animDict["mg"] = CustomHPBarAnimation.LoadAnimation(ResourceLoader.GetAllImages("mg.png"));
			animDict["bossfg"] = CustomHPBarAnimation.LoadAnimation(ResourceLoader.GetAllImages("bossfg.png"));
			animDict["bossbg"] = CustomHPBarAnimation.LoadAnimation(ResourceLoader.GetAllImages("bossbg.png"));
			animDict["bossol"] = CustomHPBarAnimation.LoadAnimation(ResourceLoader.GetAllImages("bossol.png"));
		}

		public static void LoadAllConfig()
		{
			foreach (string key in animDict.Keys)
			{
				if (ConfigExist(animDict[key], key))
				{
					HPBarAnimation hPBarAnimation = LoadAnimConfig(Path.Combine(EnemyHPBar.SkinPath, key + ".json"));
					animDict[key].loop = hPBarAnimation.loop;
					animDict[key].fps = hPBarAnimation.fps;
				}
			}
		}

		public static bool AnimExists(string name)
		{
			return CustomHPBarAnimation.loadedSprites[animDict[name]].Length != 0;
		}

		public static void SaveAllConfig()
		{
			foreach (string key in animDict.Keys)
			{
				if (!ConfigExist(animDict[key], key) && CustomHPBarAnimation.loadedSprites[animDict[key]].Length != 0)
				{
					SaveAnimConfig(animDict[key], key);
				}
			}
		}
	}
	internal static class BetterMenu
	{
		internal static int selectedSkin;

		internal static Menu MenuRef;

		internal static MenuScreen GetMenu(MenuScreen lastMenu, ModToggleDelegates? toggleDelegates)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			if (MenuRef == null)
			{
				MenuRef = PrepareMenu(toggleDelegates.Value);
			}
			MenuRef.OnBuilt += delegate
			{
				if (EnemyHPBar.CurrentSkin != null)
				{
					SelectedSkin(EnemyHPBar.CurrentSkin.GetId());
				}
			};
			return MenuRef.GetMenuScreen(lastMenu);
		}

		internal static void ApplySkin()
		{
			SetSkinById(EnemyHPBar.SkinList[selectedSkin].GetId());
			EnemyHPBar.CompleteImage(Path.Combine(EnemyHPBar.DATA_DIR, EnemyHPBar.CurrentSkin.GetId()));
			EnemyHPBar.bossol = EnemyHPBar.HPBarCreateSprite(ResourceLoader.GetBossOutlineImage());
			EnemyHPBar.bossbg = EnemyHPBar.HPBarCreateSprite(ResourceLoader.GetBossBackgroundImage());
			EnemyHPBar.bossfg = EnemyHPBar.HPBarCreateSprite(ResourceLoader.GetBossForegroundImage());
			EnemyHPBar.ol = EnemyHPBar.HPBarCreateSprite(ResourceLoader.GetOutlineImage());
			EnemyHPBar.fg = EnemyHPBar.HPBarCreateSprite(ResourceLoader.GetForegroundImage());
			EnemyHPBar.mg = EnemyHPBar.HPBarCreateSprite(ResourceLoader.GetMiddlegroundImage());
			EnemyHPBar.bg = EnemyHPBar.HPBarCreateSprite(ResourceLoader.GetBackgroundImage());
			AnimJson.animDict.Clear();
			AnimJson.Initdic();
			AnimJson.LoadAllConfig();
			AnimJson.SaveAllConfig();
		}

		internal static string[] GetSkinNameArray()
		{
			return EnemyHPBar.SkinList.Select((ISelectableSkin s) => HPBarList.MaxLength(s.GetName(), EnemyHPBar.globalSettings.NameLength)).ToArray();
		}

		internal static Menu PrepareMenu(ModToggleDelegates toggleDelegates)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Expected O, but got Unknown
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Expected O, but got Unknown
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Expected O, but got Unknown
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Expected O, but got Unknown
			return new Menu("敌人血条", (Element[])(object)new Element[4]
			{
				(Element)Blueprints.CreateToggle(toggleDelegates, "血条开关", "", "已启用", "已禁用"),
				(Element)new HorizontalOption("选择皮肤", "当前将使用的皮肤", GetSkinNameArray(), (Action<int>)delegate(int setting)
				{
					selectedSkin = setting;
				}, (Func<int>)(() => selectedSkin), "SelectSkinOption"),
				(Element)Blueprints.HorizontalBoolOption("与CK联动", "是否与 CustomKnight 集成？（使用此项请确保已安装 CK，并在切换为开启时重置皮肤；请在你的皮肤文件夹中创建名为 \"HPBar\" 的文件夹）", (Action<bool>)delegate(bool choose)
				{
					EnemyHPBar.globalSettings.Intergration = choose;
				}, (Func<bool>)(() => EnemyHPBar.globalSettings.Intergration), (string)null, (string)null, "CKIntergration"),
				(Element)new MenuRow(new List<Element>
				{
					(Element)(object)Blueprints.NavigateToMenu("皮肤列表", "打开皮肤列表", (Func<MenuScreen>)(() => HPBarList.GetMenu(MenuRef.menuScreen))),
					(Element)new MenuButton("应用皮肤", "应用当前选中的皮肤。", (Action<MenuButton>)delegate
					{
						ApplySkin();
					}, false, "__UseName")
				}, "ApplyButtonGroup")
				{
					XDelta = 400f
				}
			});
		}

		internal static void SelectedSkin(string skinId)
		{
			selectedSkin = EnemyHPBar.SkinList.FindIndex((ISelectableSkin skin) => skin.GetId() == skinId);
		}

		public static ISelectableSkin GetSkinById(string id)
		{
			return EnemyHPBar.SkinList.Find((ISelectableSkin skin) => skin.GetId() == id) ?? GetDefaultSkin();
		}

		public static ISelectableSkin GetDefaultSkin()
		{
			if (EnemyHPBar.DefaultSkin == null)
			{
				EnemyHPBar.DefaultSkin = GetSkinById("Default");
			}
			return EnemyHPBar.DefaultSkin;
		}

		public static void SetSkinById(string id)
		{
			ISelectableSkin skinById = GetSkinById(id);
			if (!(EnemyHPBar.CurrentSkin.GetId() == skinById.GetId()))
			{
				EnemyHPBar.CurrentSkin = skinById;
			}
		}
	}
	public class BossHPBar : MonoBehaviour
	{
		private GameObject bg_go;

		private GameObject fg_go;

		private GameObject ol_go;

		private CanvasRenderer bg_cr;

		private CanvasRenderer fg_cr;

		private CanvasRenderer ol_cr;

		private readonly float bossbgScale = EnemyHPBar.globalSettings.bossbgScale;

		private readonly float bossfgScale = EnemyHPBar.globalSettings.bossfgScale;

		private readonly float bossolScale = EnemyHPBar.globalSettings.bossolScale;

		private const float scaleFactor = 2f / 3f;

		public Image health_bar;

		public float maxHP;

		public int position;

		public Vector2 screenScale;

		public HealthManager hm;

		public Vector2 objectPos;

		public void Awake()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Expected O, but got Unknown
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Expected O, but got Unknown
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Expected O, but got Unknown
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			Logger.LogDebug("Creating Boss HP Bar for " + ((Object)this).name);
			GameObject bossCanvas = EnemyHPBar.bossCanvas;
			Sprite bossbg = EnemyHPBar.bossbg;
			Rect rect = EnemyHPBar.bossbg.rect;
			bg_go = CanvasUtil.CreateImagePanel(bossCanvas, bossbg, new RectData(((Rect)(ref rect)).size * bossbgScale * (2f / 3f), new Vector2(0f, 24f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f)));
			GameObject bossCanvas2 = EnemyHPBar.bossCanvas;
			Sprite bossfg = EnemyHPBar.bossfg;
			rect = EnemyHPBar.bossfg.rect;
			fg_go = CanvasUtil.CreateImagePanel(bossCanvas2, bossfg, new RectData(((Rect)(ref rect)).size * bossfgScale * (2f / 3f), new Vector2(0f, 24f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f)));
			GameObject bossCanvas3 = EnemyHPBar.bossCanvas;
			Sprite bossol = EnemyHPBar.bossol;
			rect = EnemyHPBar.bossol.rect;
			ol_go = CanvasUtil.CreateImagePanel(bossCanvas3, bossol, new RectData(((Rect)(ref rect)).size * bossolScale * (2f / 3f), new Vector2(0f, 24f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f)));
			bg_cr = bg_go.GetComponent<CanvasRenderer>();
			fg_cr = fg_go.GetComponent<CanvasRenderer>();
			ol_cr = ol_go.GetComponent<CanvasRenderer>();
			objectPos = Vector2.op_Implicit(fg_go.transform.position);
			health_bar = fg_go.GetComponent<Image>();
			health_bar.type = (Type)3;
			health_bar.fillMethod = (FillMethod)0;
			health_bar.preserveAspect = false;
			bg_go.GetComponent<Image>().preserveAspect = false;
			ol_go.GetComponent<Image>().preserveAspect = false;
			hm = ((Component)this).gameObject.GetComponent<HealthManager>();
			SetHPBarAlpha(0f);
			maxHP = hm.hp;
			if (AnimJson.AnimExists("bossbg"))
			{
				GameObjectUtils.GetAddComponent<HPBarAnimationController>(bg_go).Init(AnimJson.animDict["bossbg"]);
			}
			if (AnimJson.AnimExists("bossfg"))
			{
				GameObjectUtils.GetAddComponent<HPBarAnimationController>(fg_go).Init(AnimJson.animDict["bossfg"]);
			}
			if (AnimJson.AnimExists("bossol"))
			{
				GameObjectUtils.GetAddComponent<HPBarAnimationController>(ol_go).Init(AnimJson.animDict["bossol"]);
			}
		}

		private void SetHPBarAlpha(float alpha)
		{
			if (alpha <= 0f)
			{
				EnemyHPBar.ActiveBosses.Remove(((Component)this).gameObject);
			}
			else if (!EnemyHPBar.ActiveBosses.Contains(((Component)this).gameObject))
			{
				EnemyHPBar.ActiveBosses.Add(((Component)this).gameObject);
			}
			bg_cr.SetAlpha(alpha);
			fg_cr.SetAlpha(alpha);
			ol_cr.SetAlpha(alpha);
		}

		private void DestroyHPBar()
		{
			Object.Destroy((Object)(object)fg_go);
			Object.Destroy((Object)(object)bg_go);
			Object.Destroy((Object)(object)ol_go);
			Object.Destroy((Object)(object)health_bar);
		}

		private void MoveHPBar(Vector2 position)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			fg_go.transform.position = Vector2.op_Implicit(position);
			bg_go.transform.position = Vector2.op_Implicit(position);
			ol_go.transform.position = Vector2.op_Implicit(position);
		}

		private void OnDestroy()
		{
			SetHPBarAlpha(0f);
			DestroyHPBar();
			Logger.LogDebug("Destroyed enemy " + ((Object)((Component)this).gameObject).name);
		}

		private void OnDisable()
		{
			SetHPBarAlpha(0f);
			Logger.LogDebug("Disabled enemy " + ((Object)((Component)this).gameObject).name);
		}

		private void FixedUpdate()
		{
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Invalid comparison between Unknown and I4
			position = checked(EnemyHPBar.ActiveBosses.IndexOf(((Component)this).gameObject) + 1);
			maxHP = Math.Max(maxHP, hm.hp);
			Logger.LogFine($"Enemy {((Object)this).name}: currHP {hm.hp}, maxHP {maxHP}");
			health_bar.fillAmount = (float)hm.hp / maxHP;
			if (health_bar.fillAmount < 1f && health_bar.fillAmount > 0f)
			{
				float hPBarAlpha = (((int)GameManager.instance.gameState == 5) ? 0.5f : 1f);
				SetHPBarAlpha(hPBarAlpha);
			}
			if (((Object)((Component)this).gameObject).name == "New Game Object" && hm.hp <= 0)
			{
				Object.Destroy((Object)(object)((Component)this).gameObject);
				Logger.LogDebug("Placeholder killed");
			}
			if ((float)hm.hp <= 0f && !hm.hasSpecialDeath)
			{
				SetHPBarAlpha(0f);
			}
		}

		private void LateUpdate()
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			position = EnemyHPBar.ActiveBosses.IndexOf(((Component)this).gameObject);
			MoveHPBar(new Vector2(objectPos.x, objectPos.y + (float)position * (((Graphic)health_bar).rectTransform.sizeDelta.y + 10f * (float)Screen.height / 720f)));
		}
	}
	public class HPBar : MonoBehaviour
	{
		private GameObject bg_go;

		private GameObject mg_go;

		private GameObject fg_go;

		private GameObject ol_go;

		private CanvasRenderer bg_cr;

		private CanvasRenderer fg_cr;

		private CanvasRenderer mg_cr;

		private CanvasRenderer ol_cr;

		private readonly float bgScale = EnemyHPBar.globalSettings.bgScale;

		private readonly float fgScale = EnemyHPBar.globalSettings.fgScale;

		private readonly float olScale = EnemyHPBar.globalSettings.olScale;

		private readonly float mgScale = EnemyHPBar.globalSettings.mgScale;

		private const float scaleFactor = 0.015015015f;

		public Image health_bar;

		public Image hpbg;

		public float currHP;

		public float maxHP;

		public int oldHP;

		public HealthManager hm;

		public Vector2 objectPos;

		public Vector2 screenScale;

		public void Awake()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected O, but got Unknown
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Expected O, but got Unknown
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Expected O, but got Unknown
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Expected O, but got Unknown
			Logger.LogDebug("Creating HP Bar for " + ((Object)this).name);
			GameObject canvas = EnemyHPBar.canvas;
			Sprite bg = EnemyHPBar.bg;
			Rect rect = EnemyHPBar.bg.rect;
			bg_go = CanvasUtil.CreateImagePanel(canvas, bg, new RectData(((Rect)(ref rect)).size * bgScale * 0.015015015f, new Vector2(0f, 32f)));
			GameObject canvas2 = EnemyHPBar.canvas;
			Sprite mg = EnemyHPBar.mg;
			rect = EnemyHPBar.mg.rect;
			mg_go = CanvasUtil.CreateImagePanel(canvas2, mg, new RectData(((Rect)(ref rect)).size * mgScale * 0.015015015f, new Vector2(0f, 32f)));
			GameObject canvas3 = EnemyHPBar.canvas;
			Sprite fg = EnemyHPBar.fg;
			rect = EnemyHPBar.fg.rect;
			fg_go = CanvasUtil.CreateImagePanel(canvas3, fg, new RectData(((Rect)(ref rect)).size * fgScale * 0.015015015f, new Vector2(0f, 32f)));
			GameObject canvas4 = EnemyHPBar.canvas;
			Sprite ol = EnemyHPBar.ol;
			rect = EnemyHPBar.ol.rect;
			ol_go = CanvasUtil.CreateImagePanel(canvas4, ol, new RectData(((Rect)(ref rect)).size * olScale * 0.015015015f, new Vector2(0f, 32f)));
			bg_cr = bg_go.GetComponent<CanvasRenderer>();
			fg_cr = fg_go.GetComponent<CanvasRenderer>();
			mg_cr = mg_go.GetComponent<CanvasRenderer>();
			ol_cr = ol_go.GetComponent<CanvasRenderer>();
			hpbg = mg_go.GetComponent<Image>();
			hpbg.type = (Type)3;
			hpbg.fillMethod = (FillMethod)0;
			hpbg.preserveAspect = false;
			health_bar = fg_go.GetComponent<Image>();
			health_bar.type = (Type)3;
			health_bar.fillMethod = (FillMethod)0;
			health_bar.preserveAspect = false;
			bg_go.GetComponent<Image>().preserveAspect = false;
			ol_go.GetComponent<Image>().preserveAspect = false;
			hm = ((Component)this).gameObject.GetComponent<HealthManager>();
			SetHPBarAlpha(0f);
			maxHP = hm.hp;
			currHP = hm.hp;
			if (AnimJson.AnimExists("bg"))
			{
				GameObjectUtils.GetAddComponent<HPBarAnimationController>(bg_go).Init(AnimJson.animDict["bg"]);
			}
			if (AnimJson.AnimExists("fg"))
			{
				GameObjectUtils.GetAddComponent<HPBarAnimationController>(fg_go).Init(AnimJson.animDict["fg"]);
			}
			if (AnimJson.AnimExists("mg"))
			{
				GameObjectUtils.GetAddComponent<HPBarAnimationController>(mg_go).Init(AnimJson.animDict["mg"]);
			}
			if (AnimJson.AnimExists("ol"))
			{
				GameObjectUtils.GetAddComponent<HPBarAnimationController>(ol_go).Init(AnimJson.animDict["ol"]);
			}
		}

		private void SetHPBarAlpha(float alpha)
		{
			bg_cr.SetAlpha(alpha);
			fg_cr.SetAlpha(alpha);
			mg_cr.SetAlpha(alpha);
			ol_cr.SetAlpha(alpha);
		}

		private void DestroyHPBar()
		{
			Object.Destroy((Object)(object)fg_go);
			Object.Destroy((Object)(object)bg_go);
			Object.Destroy((Object)(object)mg_go);
			Object.Destroy((Object)(object)ol_go);
			Object.Destroy((Object)(object)hpbg);
			Object.Destroy((Object)(object)health_bar);
		}

		private void MoveHPBar(Vector2 position)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			fg_go.transform.position = Vector2.op_Implicit(position);
			mg_go.transform.position = Vector2.op_Implicit(position);
			bg_go.transform.position = Vector2.op_Implicit(position);
			ol_go.transform.position = Vector2.op_Implicit(position);
		}

		private void OnDestroy()
		{
			SetHPBarAlpha(0f);
			DestroyHPBar();
			Logger.LogDebug("Destroyed enemy " + ((Object)this).name);
		}

		private void OnDisable()
		{
			SetHPBarAlpha(0f);
			Logger.LogDebug("Disabled enemy " + ((Object)this).name);
		}

		private void FixedUpdate()
		{
			if (currHP > (float)hm.hp)
			{
				currHP--;
			}
			else
			{
				currHP = hm.hp;
			}
			Logger.LogFine($"Enemy {((Object)this).name}: currHP {hm.hp}, maxHP {maxHP}");
			health_bar.fillAmount = (float)hm.hp / maxHP;
			hpbg.fillAmount = currHP / maxHP;
			if (health_bar.fillAmount < 1f)
			{
				SetHPBarAlpha(1f);
			}
			if (((Object)((Component)this).gameObject).name == "New Game Object" && currHP <= 0f)
			{
				Logger.LogDebug("Placeholder killed");
				Object.Destroy((Object)(object)((Component)this).gameObject);
			}
			if (currHP <= 0f)
			{
				SetHPBarAlpha(0f);
			}
			oldHP = hm.hp;
		}

		private void LateUpdate()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			objectPos = Vector2.op_Implicit(((Component)this).transform.position + Vector3.up * 1.5f * (float)Screen.height / 720f);
			MoveHPBar(objectPos);
		}
	}
	public class EnemyHPBar : Mod, IGlobalSettings<Settings>, ICustomMenuMod, IMod, ILogger, ITogglableMod
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static hook_Start <0>__ModifyFSM;
		}

		private static readonly Lazy<string> Version = new Lazy<string>(() => Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);

		public static GameObject canvas;

		public static GameObject bossCanvas;

		public static bool InstallCK = false;

		public const string HPBAR_BG = "bg.png";

		public const string HPBAR_FG = "fg.png";

		public const string HPBAR_MG = "mg.png";

		public const string HPBAR_OL = "ol.png";

		public const string HPBAR_BOSSOL = "bossol.png";

		public const string HPBAR_BOSSFG = "bossfg.png";

		public const string HPBAR_BOSSBG = "bossbg.png";

		public const string SPRITE_FOLDER = "CustomHPBar";

		public static readonly string DATA_DIR = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "CustomHPBar");

		public static Sprite bg;

		public static Sprite mg;

		public static Sprite fg;

		public static Sprite ol;

		public static Sprite bossbg;

		public static Sprite bossfg;

		public static Sprite bossol;

		public static Settings globalSettings = new Settings();

		public static List<GameObject> ActiveBosses;

		public static List<ISelectableSkin> SkinList;

		public static ISelectableSkin CurrentSkin;

		public static ISelectableSkin DefaultSkin;

		public static string SkinPath
		{
			get
			{
				if (!InstallCK || !globalSettings.Intergration)
				{
					return Path.Combine(DATA_DIR, CurrentSkin.GetId());
				}
				return GetCKPath();
			}
		}

		public bool ToggleButtonInsideMenu { get; } = true;

		public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggle)
		{
			return BetterMenu.GetMenu(modListMenu, toggle);
		}

		public override string GetVersion()
		{
			return Version.Value;
		}

		public EnemyHPBar()
			: base("敌人血条")
		{
			ModInteropManager.ModInterop(typeof(EnemyHPBarExport));
		}

		public override void Initialize()
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Expected O, but got Unknown
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Expected O, but got Unknown
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Expected O, but got Unknown
			if (!Directory.Exists(DATA_DIR))
			{
				Directory.CreateDirectory(DATA_DIR);
			}
			if (!Directory.Exists(Path.Combine(DATA_DIR, "Default")))
			{
				Directory.CreateDirectory(Path.Combine(DATA_DIR, "Default"));
			}
			CompleteImage(Path.Combine(DATA_DIR, "Default"));
			GetSkinList();
			ModHooks.OnEnableEnemyHook += new OnEnableEnemyHandler(Instance_OnEnableEnemyHook);
			ModHooks.OnReceiveDeathEventHook += new OnReceiveDeathEventHandler(Instance_OnReceiveDeathEventHook);
			SceneManager.sceneLoaded += SceneManager_sceneLoaded;
			object obj = <>O.<0>__ModifyFSM;
			if (obj == null)
			{
				hook_Start val = ModifyFSM;
				<>O.<0>__ModifyFSM = val;
				obj = (object)val;
			}
			PlayMakerFSM.Start += (hook_Start)obj;
			canvas = CanvasUtil.CreateCanvas((RenderMode)2, new Vector2(1280f, 720f));
			bossCanvas = CanvasUtil.CreateCanvas((RenderMode)0, new Vector2(1280f, 720f));
			canvas.GetComponent<Canvas>().sortingOrder = 1;
			bossCanvas.GetComponent<Canvas>().sortingOrder = 1;
			bossol = HPBarCreateSprite(ResourceLoader.GetBossOutlineImage());
			bossbg = HPBarCreateSprite(ResourceLoader.GetBossBackgroundImage());
			bossfg = HPBarCreateSprite(ResourceLoader.GetBossForegroundImage());
			ol = HPBarCreateSprite(ResourceLoader.GetOutlineImage());
			fg = HPBarCreateSprite(ResourceLoader.GetForegroundImage());
			mg = HPBarCreateSprite(ResourceLoader.GetMiddlegroundImage());
			bg = HPBarCreateSprite(ResourceLoader.GetBackgroundImage());
			Object.DontDestroyOnLoad((Object)(object)canvas);
			Object.DontDestroyOnLoad((Object)(object)bossCanvas);
			InstallCK = ModHooks.GetMod("CustomKnight", false, false) is Mod;
			if (InstallCK)
			{
				AddCKHandle();
			}
			AnimJson.animDict.Clear();
			AnimJson.Initdic();
			AnimJson.LoadAllConfig();
			AnimJson.SaveAllConfig();
		}

		private void AddCKHandle()
		{
			SkinManager.OnSetSkin += ChangeSkin;
		}

		private void ChangeSkin(object sender, EventArgs e)
		{
			if (Directory.Exists(Path.Combine(SkinManager.GetCurrentSkin().getSwapperPath(), "HPBar")) && globalSettings.Intergration)
			{
				CompleteImage(Path.Combine(SkinManager.GetCurrentSkin().getSwapperPath(), "HPBar"));
				bossol = HPBarCreateSprite(ResourceLoader.GetCKImage("bossol.png"));
				bossbg = HPBarCreateSprite(ResourceLoader.GetCKImage("bossbg.png"));
				bossfg = HPBarCreateSprite(ResourceLoader.GetCKImage("bossfg.png"));
				ol = HPBarCreateSprite(ResourceLoader.GetCKImage("ol.png"));
				fg = HPBarCreateSprite(ResourceLoader.GetCKImage("fg.png"));
				mg = HPBarCreateSprite(ResourceLoader.GetCKImage("mg.png"));
				bg = HPBarCreateSprite(ResourceLoader.GetCKImage("bg.png"));
				AnimJson.animDict.Clear();
				AnimJson.Initdic();
				AnimJson.LoadAllConfig();
				AnimJson.SaveAllConfig();
			}
		}

		public static string GetCKPath()
		{
			if (!Directory.Exists(Path.Combine(SkinManager.GetCurrentSkin().getSwapperPath(), "HPBar")))
			{
				return Path.Combine(DATA_DIR, CurrentSkin.GetId());
			}
			return Path.Combine(SkinManager.GetCurrentSkin().getSwapperPath(), "HPBar");
		}

		public void OnLoadGlobal(Settings s)
		{
			globalSettings = s;
		}

		public Settings OnSaveGlobal()
		{
			globalSettings.CurrentSkin = CurrentSkin.GetId();
			return globalSettings;
		}

		public static Sprite HPBarCreateSprite(byte[] data)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			Texture2D val = new Texture2D(1, 1);
			ImageConversion.LoadImage(val, data);
			((Texture)val).anisoLevel = 0;
			return Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), new Vector2(0.5f, 0.5f));
		}

		private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
		{
			ActiveBosses = new List<GameObject>();
		}

		private void Instance_OnReceiveDeathEventHook(EnemyDeathEffects enemyDeathEffects, bool eventAlreadyRecieved, ref float? attackDirection, ref bool resetDeathEvent, ref bool spellBurn, ref bool isWatery)
		{
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			Logger.LogDebug("Enemy " + ((Object)((Component)enemyDeathEffects).gameObject).name + " dead");
			if ((Object)(object)((Component)enemyDeathEffects).gameObject.GetComponent<HPBar>() != (Object)null)
			{
				if (((Component)enemyDeathEffects).gameObject.GetComponent<HPBar>().oldHP == 0)
				{
					GameObject val = new GameObject();
					val.transform.position = ((Component)enemyDeathEffects).gameObject.transform.position;
					HealthManager val2 = val.AddComponent<HealthManager>();
					HPBar hPBar = val.AddComponent<HPBar>();
					hPBar.currHP = 18f;
					hPBar.maxHP = 18f;
					val2.hp = 0;
				}
				else
				{
					GameObject val3 = new GameObject();
					val3.transform.position = ((Component)enemyDeathEffects).gameObject.transform.position;
					HealthManager val4 = val3.AddComponent<HealthManager>();
					HPBar hPBar2 = val3.AddComponent<HPBar>();
					val3.AddComponent<EnemyDeathEffects>();
					hPBar2.currHP = ((Component)enemyDeathEffects).gameObject.GetComponent<HPBar>().oldHP;
					hPBar2.maxHP = ((Component)enemyDeathEffects).gameObject.GetComponent<HPBar>().maxHP;
					val4.hp = 0;
				}
			}
		}

		private bool Instance_OnEnableEnemyHook(GameObject enemy, bool isAlreadyDead)
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Invalid comparison between Unknown and I4
			if ((Object)(object)enemy.GetComponent<DisableHPBar>() != (Object)null)
			{
				return isAlreadyDead;
			}
			if (((Object)enemy).name.Contains("White Palace Fly"))
			{
				return isAlreadyDead;
			}
			HealthManager component = enemy.GetComponent<HealthManager>();
			if ((Object)(object)component == (Object)null)
			{
				return isAlreadyDead;
			}
			EnemyDeathEffects component2 = enemy.GetComponent<EnemyDeathEffects>();
			EnemyDeathTypes? val = (((Object)(object)component2 == (Object)null) ? ((EnemyDeathTypes?)null) : new EnemyDeathTypes?(ReflectionHelper.GetField<EnemyDeathEffects, EnemyDeathTypes>(component2, "enemyDeathType")));
			bool flag = component.hp >= 200 || (int)val.GetValueOrDefault() == 5;
			BossMarker component3 = enemy.GetComponent<BossMarker>();
			if (component3 != null)
			{
				flag = component3.isBoss;
			}
			if (!flag)
			{
				if (((Object)(object)((Component)component).gameObject.GetComponent<HPBar>() != (Object)null || component.hp >= 7000) | isAlreadyDead)
				{
					return isAlreadyDead;
				}
				((Component)component).gameObject.AddComponent<HPBar>();
				((Loggable)this).LogDebug("Added HP bar to " + ((Object)enemy).name);
			}
			else
			{
				if (((Object)(object)((Component)component).gameObject.GetComponent<BossHPBar>() != (Object)null || component.hp >= 7000) | isAlreadyDead)
				{
					return isAlreadyDead;
				}
				((Component)component).gameObject.AddComponent<BossHPBar>();
				((Loggable)this).LogDebug("Added Boss HP bar to " + ((Object)enemy).name);
			}
			return isAlreadyDead;
		}

		private static void ModifyFSM(orig_Start orig, PlayMakerFSM self)
		{
			orig.Invoke(self);
			try
			{
				GameObject go = ((Component)self).gameObject;
				if (self != null && ((Object)self).name == "False Knight New" && self.FsmName == "FalseyControl")
				{
					EnemyHPBarExport.MarkAsBoss(go);
				}
				else if (((Object)self).name.StartsWith("Zote Balloon") && self.FsmName == "Control")
				{
					FsmUtil.AddCustomAction(self, "Reset", (Action)delegate
					{
						EnemyHPBarExport.RefreshHPBar(go);
					});
				}
				else if (((Object)self).name.StartsWith("Zote Crew "))
				{
					FsmUtil.AddCustomAction(self, "Death", (Action)delegate
					{
						EnemyHPBarExport.DisableHPBar(go);
					});
					FsmUtil.AddCustomAction(self, "Activate", (Action)delegate
					{
						EnemyHPBarExport.EnableHPBar(go);
					});
				}
				else if (self != null && ((Object)self).name == "Zote Fluke" && self.FsmName == "Control")
				{
					FsmUtil.AddCustomAction(self, "Sleeping", (Action)delegate
					{
						EnemyHPBarExport.RefreshHPBar(go);
					});
				}
				else if (self != null && ((Object)self).name == "Zote Salubra" && self.FsmName == "Control")
				{
					FsmUtil.AddCustomAction(self, "Death", (Action)delegate
					{
						EnemyHPBarExport.RefreshHPBar(go);
					});
				}
				else if (self != null && ((Object)self).name == "Zote Thwomp" && self.FsmName == "Control")
				{
					EnemyHPBarExport.MarkAsNonBoss(((Component)self).gameObject);
					FsmUtil.AddCustomAction(self, "Wait", (Action)delegate
					{
						EnemyHPBarExport.DisableHPBar(go);
					});
					FsmUtil.AddCustomAction(self, "Set Pos", (Action)delegate
					{
						EnemyHPBarExport.EnableHPBar(go);
					});
				}
				else if (self != null && ((Object)self).name == "Zote Turrep" && self.FsmName == "Control")
				{
					EnemyHPBarExport.MarkAsNonBoss(((Component)self).gameObject);
					FsmUtil.AddCustomAction(self, "Death Pause", (Action)delegate
					{
						EnemyHPBarExport.DisableHPBar(go);
					});
					FsmUtil.AddCustomAction(self, "Appear", (Action)delegate
					{
						EnemyHPBarExport.EnableHPBar(go);
					});
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.ToString());
			}
		}

		internal static void GetSkinList()
		{
			string[] directories = Directory.GetDirectories(DATA_DIR);
			SkinList = new List<ISelectableSkin>();
			for (int i = 0; i < directories.Length; i = checked(i + 1))
			{
				string name = new DirectoryInfo(directories[i]).Name;
				SkinList.Add(new HPBarList(name));
			}
			CurrentSkin = BetterMenu.GetSkinById(globalSettings.CurrentSkin);
			Logger.Log("Loaded Skins list");
		}

		public static void CompleteImage(string skinpath)
		{
			foreach (string item in from t in Assembly.GetExecutingAssembly().GetManifestResourceNames()
				where t.EndsWith("png")
				select t)
			{
				string path = item.Replace("EnemyHPBar.Resources.", "");
				string path2 = Path.Combine(skinpath, path);
				if (File.Exists(path2))
				{
					continue;
				}
				using FileStream destination = File.Create(path2);
				using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(item);
				stream.CopyTo(destination);
			}
		}

		public void Unload()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Expected O, but got Unknown
			ModHooks.OnEnableEnemyHook -= new OnEnableEnemyHandler(Instance_OnEnableEnemyHook);
			ModHooks.OnReceiveDeathEventHook -= new OnReceiveDeathEventHandler(Instance_OnReceiveDeathEventHook);
			SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
			object obj = <>O.<0>__ModifyFSM;
			if (obj == null)
			{
				hook_Start val = ModifyFSM;
				<>O.<0>__ModifyFSM = val;
				obj = (object)val;
			}
			PlayMakerFSM.Start -= (hook_Start)obj;
			globalSettings.CurrentSkin = CurrentSkin.GetId();
			GameObject[] array = Object.FindObjectsOfType<GameObject>();
			foreach (GameObject obj2 in array)
			{
				HPBar component = obj2.GetComponent<HPBar>();
				if (component != null)
				{
					Object.Destroy((Object)(object)component);
				}
				BossHPBar component2 = obj2.GetComponent<BossHPBar>();
				if (component2 != null)
				{
					Object.Destroy((Object)(object)component2);
				}
			}
		}
	}
	public class HPBarAnimation
	{
		public float fps;

		public bool loop;
	}
	public class HPBarAnimationController : MonoBehaviour
	{
		public HPBarAnimation anim;

		public Sprite[] sprites;

		public int currentFrame;

		public bool animating;

		private Image img;

		private DateTime lastFrameChange;

		public void Start()
		{
			img = GameObjectUtils.GetAddComponent<Image>(((Component)this).gameObject);
		}

		public void Init(HPBarAnimation animation)
		{
			anim = animation;
			lastFrameChange = DateTime.MinValue;
			currentFrame = -1;
			animating = true;
			sprites = CustomHPBarAnimation.loadedSprites[anim];
		}

		public void Update()
		{
			if (!animating)
			{
				return;
			}
			_ = lastFrameChange;
			checked
			{
				if ((DateTime.Now - lastFrameChange).TotalMilliseconds > (double)(1000f / anim.fps))
				{
					currentFrame++;
					if (currentFrame >= sprites.Length)
					{
						currentFrame = ((!anim.loop) ? (sprites.Length - 1) : 0);
					}
					lastFrameChange = DateTime.Now;
				}
				if ((Object)(object)img == (Object)null)
				{
					img = GameObjectUtils.GetAddComponent<Image>(((Component)this).gameObject);
				}
				img.sprite = sprites[currentFrame];
			}
		}
	}
	public static class CustomHPBarAnimation
	{
		public static Dictionary<HPBarAnimation, Sprite[]> loadedSprites = new Dictionary<HPBarAnimation, Sprite[]>();

		public static HPBarAnimation LoadAnimation(Sprite[] sprites)
		{
			HPBarAnimation hPBarAnimation = new HPBarAnimation
			{
				fps = 1f,
				loop = true
			};
			loadedSprites[hPBarAnimation] = sprites;
			return hPBarAnimation;
		}
	}
	public interface ISelectableSkin
	{
		string GetId();

		string GetName();
	}
	internal class HPBarList : ISelectableSkin
	{
		internal static Menu MenuRef;

		internal static MenuScreen lastMenu;

		private static bool applying;

		public string SkinDirectory = "";

		public HPBarList(string DirectoryName)
		{
			SkinDirectory = DirectoryName;
		}

		public string GetId()
		{
			return SkinDirectory;
		}

		public string GetName()
		{
			return SkinDirectory;
		}

		internal static Menu PrepareMenu()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected O, but got Unknown
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Expected O, but got Unknown
			Menu val = new Menu("选择皮肤", (Element[])(object)new Element[2]
			{
				(Element)new TextPanel("选择要应用的皮肤", 1000f, 35, "helptext"),
				(Element)new TextPanel("应用皮肤中...", 1000f, 35, "applying")
				{
					isVisible = false
				}
			});
			for (int i = 0; i < EnemyHPBar.SkinList.Count; i = checked(i + 1))
			{
				val.AddElement((Element)(object)ApplySkinButton(i));
			}
			return val;
		}

		internal static string MaxLength(string skinName, int length)
		{
			if (skinName.Length > length)
			{
				return skinName.Substring(0, checked(length - 3)) + "...";
			}
			return skinName;
		}

		internal static MenuButton ApplySkinButton(int index)
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Expected O, but got Unknown
			return new MenuButton(MaxLength(EnemyHPBar.SkinList[index].GetName(), EnemyHPBar.globalSettings.NameLength), "", (Action<MenuButton>)delegate
			{
				if (!applying)
				{
					applying = true;
					BetterMenu.selectedSkin = index;
					((MonoBehaviour)GameManager.instance).StartCoroutine(applyAndGoBack());
				}
			}, false, "skinbutton" + EnemyHPBar.SkinList[index].GetId());
		}

		private static void setSkinButtonVisibility(bool isVisible)
		{
			for (int i = 0; i < EnemyHPBar.SkinList.Count; i = checked(i + 1))
			{
				Menu menuRef = MenuRef;
				Element val = ((menuRef != null) ? menuRef.Find("skinbutton" + EnemyHPBar.SkinList[i].GetId()) : null);
				if (val != null)
				{
					((BaseElement)val).isVisible = isVisible;
				}
			}
			((BaseElement)MenuRef).Update();
		}

		private static IEnumerator applyAndGoBack()
		{
			((BaseElement)MenuRef.Find("helptext")).isVisible = false;
			((BaseElement)MenuRef.Find("applying")).isVisible = true;
			setSkinButtonVisibility(isVisible: false);
			yield return (object)new WaitForSecondsRealtime(0.2f);
			Menu menuRef = BetterMenu.MenuRef;
			if (menuRef != null)
			{
				Element obj = menuRef.Find("SelectSkinOption");
				if (obj != null)
				{
					((BaseElement)obj).updateAfter((Action<BaseElement>)delegate
					{
						BetterMenu.ApplySkin();
					});
				}
			}
			yield return (object)new WaitForSecondsRealtime(0.2f);
			UIManager.instance.UIGoToDynamicMenu(lastMenu);
			yield return (object)new WaitForSecondsRealtime(0.2f);
			((BaseElement)MenuRef.Find("helptext")).isVisible = true;
			((BaseElement)MenuRef.Find("applying")).isVisible = false;
			setSkinButtonVisibility(isVisible: true);
		}

		internal static MenuScreen GetMenu(MenuScreen lastMenu)
		{
			if (MenuRef == null)
			{
				MenuRef = PrepareMenu();
			}
			applying = false;
			HPBarList.lastMenu = lastMenu;
			return MenuRef.GetMenuScreen(lastMenu);
		}
	}
	internal static class Logger
	{
		private const string prefix = "[EnemyHPBar] - ";

		internal static void LogFine(string message)
		{
			Logger.LogFine("[EnemyHPBar] - " + message);
		}

		internal static void LogDebug(string message)
		{
			Logger.LogDebug("[EnemyHPBar] - " + message);
		}

		internal static void Log(string message)
		{
			Logger.Log("[EnemyHPBar] - " + message);
		}

		internal static void LogWarn(string message)
		{
			Logger.LogWarn("[EnemyHPBar] - " + message);
		}

		internal static void LogError(string message)
		{
			Logger.LogError("[EnemyHPBar] - " + message);
		}
	}
	internal sealed class DisableHPBar : MonoBehaviour
	{
	}
	internal sealed class BossMarker : MonoBehaviour
	{
		public bool isBoss = true;
	}
	[ModExportName("EnemyHPBar")]
	public static class EnemyHPBarExport
	{
		public static void DisableHPBar(GameObject go)
		{
			HPBar component = go.GetComponent<HPBar>();
			if (component != null)
			{
				Object.Destroy((Object)(object)component);
			}
			BossHPBar component2 = go.GetComponent<BossHPBar>();
			if (component2 != null)
			{
				Object.Destroy((Object)(object)component2);
			}
			GameObjectUtils.GetAddComponent<DisableHPBar>(go);
		}

		public static void EnableHPBar(GameObject go)
		{
			DisableHPBar component = go.GetComponent<DisableHPBar>();
			if (component != null)
			{
				Object.Destroy((Object)(object)component);
			}
			HealthManager component2 = go.GetComponent<HealthManager>();
			((Behaviour)component2).enabled = false;
			((Behaviour)component2).enabled = true;
		}

		public static void RefreshHPBar(GameObject go)
		{
			DisableHPBar(go);
			EnableHPBar(go);
		}

		public static void MarkAsBoss(GameObject go)
		{
			GameObjectUtils.GetAddComponent<BossMarker>(go).isBoss = true;
			RefreshHPBar(go);
		}

		public static void MarkAsNonBoss(GameObject go)
		{
			GameObjectUtils.GetAddComponent<BossMarker>(go).isBoss = false;
			RefreshHPBar(go);
		}
	}
	internal static class ResourceLoader
	{
		private static byte[] GetImage(string name)
		{
			return File.ReadAllBytes(Path.Combine(EnemyHPBar.DATA_DIR, EnemyHPBar.CurrentSkin.GetId(), name));
		}

		public static byte[] GetCKImage(string name)
		{
			if (!Directory.Exists(Path.Combine(SkinManager.GetCurrentSkin().getSwapperPath(), "HPBar")))
			{
				return GetImage(name);
			}
			return File.ReadAllBytes(Path.Combine(SkinManager.GetCurrentSkin().getSwapperPath(), "HPBar", name));
		}

		public static Sprite[] GetAllImages(string name)
		{
			string[] files = Directory.GetFiles(EnemyHPBar.SkinPath, AnimJson.FilterExtension(name) + "_*.png");
			Sprite[] array = (Sprite[])(object)new Sprite[files.Length];
			string[] array2 = files;
			foreach (string text in array2)
			{
				int.TryParse(Regex.Match(text, ".*_([0-9]*).png").Groups[1].Value, out var result);
				Logger.LogDebug($"num:{result}");
				array[result] = EnemyHPBar.HPBarCreateSprite(File.ReadAllBytes(text));
			}
			return array;
		}

		public static byte[] GetBackgroundImage()
		{
			return GetImage("bg.png");
		}

		public static byte[] GetForegroundImage()
		{
			return GetImage("fg.png");
		}

		public static byte[] GetMiddlegroundImage()
		{
			return GetImage("mg.png");
		}

		public static byte[] GetOutlineImage()
		{
			return GetImage("ol.png");
		}

		public static byte[] GetBossBackgroundImage()
		{
			return GetImage("bossbg.png");
		}

		public static byte[] GetBossForegroundImage()
		{
			return GetImage("bossfg.png");
		}

		public static byte[] GetBossOutlineImage()
		{
			return GetImage("bossol.png");
		}
	}
	public sealed class Settings
	{
		public float fgScale = 1f;

		public float bgScale = 1f;

		public float mgScale = 1f;

		public float olScale = 1f;

		public float bossfgScale = 1f;

		public float bossbgScale = 1f;

		public float bossolScale = 1f;

		public int NameLength = 10;

		public bool Intergration;

		public string DefaultSkin { get; set; } = "Default";

		public string CurrentSkin { get; set; } = "Default";
	}
}
