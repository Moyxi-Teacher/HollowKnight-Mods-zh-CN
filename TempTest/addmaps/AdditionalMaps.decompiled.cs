using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using AdditionalMaps.Consts;
using AdditionalMaps.MonoBehaviours;
using GlobalEnums;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using IL;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Modding;
using Modding.Delegates;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using MonoMod.Cil;
using Newtonsoft.Json;
using On;
using SFCore.Generics;
using SFCore.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: AssemblyCompany("AdditionalMaps")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyCopyright("Copyright © SFGrenade 2025")]
[assembly: AssemblyDescription("A Hollow Knight Mod")]
[assembly: AssemblyFileVersion("1.5.3.4")]
[assembly: AssemblyInformationalVersion("1.5.3.4汉化")]
[assembly: AssemblyProduct("AdditionalMaps")]
[assembly: AssemblyTitle("AdditionalMaps")]
[assembly: AssemblyVersion("1.5.3.4")]
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
namespace AdditionalMaps
{
	[UsedImplicitly]
	public class AdditionalMaps : FullSettingsMod<AmSaveSettings, AmGlobalSettings>, IMenuMod, IMod, ILogger, ITogglableMod
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static hook_Start <0>__OnPlayMakerFSMStart;

			public static hook_StartNewGame <1>__OnStartNewGame;

			public static hook_ContinueGame <2>__OnContinueGame;
		}

		private static AdditionalMaps _instance;

		private GameObject _shinyPrefab;

		private PlayMakerFSM _setCompassPointPrefab;

		private PlayMakerFSM _setCompassPointRoomPrefab;

		private static GameObject areaWhitePalace = null;

		private static GameObject areaGodhome = null;

		private static readonly string[] _allScenes = new string[18]
		{
			"White_Palace_01", "White_Palace_02", "White_Palace_04", "White_Palace_05", "White_Palace_06", "White_Palace_07", "White_Palace_08", "White_Palace_09", "White_Palace_12", "White_Palace_13",
			"White_Palace_14", "White_Palace_15", "White_Palace_16", "White_Palace_17", "White_Palace_18", "White_Palace_19", "White_Palace_20", "GG_Atrium_Roof"
		};

		private readonly List<string> _sceneList = new List<string> { "White_Palace_03_hub", "White_Palace_08", "GG_Atrium", "GG_Workshop", "GG_Blue_Room" };

		private static string customAreaNameWp = "White_Palace";

		private static string boolNameWp = "AdditionalMapsGotWpMap";

		private static string customGlobalEventWp = customAreaNameWp.ToUpper() + "_GLOBAL";

		private static string customAreaNameGh = "Godhome";

		private static string boolNameGh = "AdditionalMapsGotGhMap";

		private static string customGlobalEventGh = customAreaNameGh.ToUpper() + "_GLOBAL";

		private LanguageStrings LangStrings { get; }

		private TextureStrings SpriteDict { get; }

		private static Material DefaultSpriteMaterial { get; set; }

		public bool ToggleButtonInsideMenu => true;

		private static Sprite GetSprite(string name)
		{
			return _instance.SpriteDict.Get(name);
		}

		public override string GetVersion()
		{
			return Util.GetVersion(Assembly.GetExecutingAssembly());
		}

		public override List<(string, string)> GetPreloadNames()
		{
			return new List<(string, string)>
			{
				("Crossroads_33", "scatter_map 1"),
				("Crossroads_33", "scatter_map 2"),
				("Crossroads_33", "scatter_map 3"),
				("Grimm_Divine", "Charm Holder"),
				("Town", "_Props/Stag_station/open/door_station"),
				("Room_mapper", "_SceneManager")
			};
		}

		public AdditionalMaps()
			: base("AdditionalMaps（额外地图）")
		{
			_instance = this;
			LangStrings = new LanguageStrings();
			SpriteDict = new TextureStrings();
			InitCallbacks();
			GameMapHooks.Init(GameMapCallback);
		}

		public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
		{
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Expected O, but got Unknown
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			((Loggable)this).Log("Initializing");
			_shinyPrefab = preloadedObjects["Grimm_Divine"]["Charm Holder"];
			Object.Destroy((Object)(object)_shinyPrefab.transform.GetChild(2));
			Object.Destroy((Object)(object)_shinyPrefab.transform.GetChild(1));
			Object.Destroy((Object)(object)((Component)_shinyPrefab.transform.GetChild(0)).gameObject.GetComponent<PersistentBoolItem>());
			SetInactive(_shinyPrefab);
			_setCompassPointPrefab = FSMUtility.LocateMyFSM(preloadedObjects["Town"]["_Props/Stag_station/open/door_station"], "Set Compass Point");
			_setCompassPointRoomPrefab = FSMUtility.LocateMyFSM(preloadedObjects["Room_mapper"]["_SceneManager"], "map_isroom");
			DefaultSpriteMaterial = new Material(Shader.Find("Sprites/Default"));
			DefaultSpriteMaterial.SetColor(Shader.PropertyToID("_Color"), new Color(1f, 1f, 1f, 1f));
			DefaultSpriteMaterial.SetInt(Shader.PropertyToID("PixelSnap"), 0);
			DefaultSpriteMaterial.SetFloat(Shader.PropertyToID("_EnableExternalAlpha"), 0f);
			DefaultSpriteMaterial.SetInt(Shader.PropertyToID("_StencilComp"), 8);
			DefaultSpriteMaterial.SetInt(Shader.PropertyToID("_Stencil"), 0);
			DefaultSpriteMaterial.SetInt(Shader.PropertyToID("_StencilOp"), 0);
			DefaultSpriteMaterial.SetInt(Shader.PropertyToID("_StencilWriteMask"), 255);
			DefaultSpriteMaterial.SetInt(Shader.PropertyToID("_StencilReadMask"), 255);
			((Loggable)this).Log("Initialized");
		}

		public List<MenuEntry> GetMenuData(MenuEntry? toggleButtonEntry)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			return new List<MenuEntry>
			{
				new MenuEntry(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values, "将白色宫殿和神居的地图添加到世界地图。", toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader)
			};
		}

		public void Unload()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Expected O, but got Unknown
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Expected O, but got Unknown
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Expected O, but got Unknown
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Expected O, but got Unknown
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			object obj = <>O.<0>__OnPlayMakerFSMStart;
			if (obj == null)
			{
				hook_Start val = OnPlayMakerFSMStart;
				<>O.<0>__OnPlayMakerFSMStart = val;
				obj = (object)val;
			}
			PlayMakerFSM.Start -= (hook_Start)obj;
			object obj2 = <>O.<1>__OnStartNewGame;
			if (obj2 == null)
			{
				hook_StartNewGame val2 = OnStartNewGame;
				<>O.<1>__OnStartNewGame = val2;
				obj2 = (object)val2;
			}
			GameManager.StartNewGame -= (hook_StartNewGame)obj2;
			object obj3 = <>O.<2>__OnContinueGame;
			if (obj3 == null)
			{
				hook_ContinueGame val3 = OnContinueGame;
				<>O.<2>__OnContinueGame = val3;
				obj3 = (object)val3;
			}
			GameManager.ContinueGame -= (hook_ContinueGame)obj3;
			ModHooks.LanguageGetHook -= new LanguageGetProxy(OnLanguageGetHook);
			ModHooks.GetPlayerBoolHook -= new GetBoolProxy(OnGetPlayerBoolHook);
			ModHooks.SetPlayerBoolHook -= new SetBoolProxy(OnSetPlayerBoolHook);
			SceneManager.activeSceneChanged -= OnSceneManagerActiveSceneChanged;
			SceneManager.AddSceneMapped -= new Manipulator(OnSceneManagerAddSceneMapped);
			GameMapHooks.Unhook();
		}

		private Dictionary<string, SCustomArea> GameMapCallback(GameMap gameMapBetter)
		{
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Expected O, but got Unknown
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Expected O, but got Unknown
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Expected O, but got Unknown
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Expected O, but got Unknown
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Expected O, but got Unknown
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Expected O, but got Unknown
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Expected O, but got Unknown
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Expected O, but got Unknown
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Expected O, but got Unknown
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Expected O, but got Unknown
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Expected O, but got Unknown
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Expected O, but got Unknown
			//IL_0483: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Expected O, but got Unknown
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Expected O, but got Unknown
			//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0509: Expected O, but got Unknown
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0547: Expected O, but got Unknown
			//IL_057b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0585: Expected O, but got Unknown
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Expected O, but got Unknown
			//IL_060b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_070b: Unknown result type (might be due to invalid IL or missing references)
			//IL_072e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0751: Unknown result type (might be due to invalid IL or missing references)
			//IL_0774: Unknown result type (might be due to invalid IL or missing references)
			//IL_0797: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0800: Unknown result type (might be due to invalid IL or missing references)
			//IL_0824: Unknown result type (might be due to invalid IL or missing references)
			//IL_0848: Unknown result type (might be due to invalid IL or missing references)
			//IL_086c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0890: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0920: Unknown result type (might be due to invalid IL or missing references)
			//IL_0944: Unknown result type (might be due to invalid IL or missing references)
			//IL_096b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0996: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a0: Expected O, but got Unknown
			//IL_09ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c4: Expected O, but got Unknown
			//IL_09de: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e8: Expected O, but got Unknown
			//IL_0a02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0c: Expected O, but got Unknown
			//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a30: Expected O, but got Unknown
			//IL_0a4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a54: Expected O, but got Unknown
			//IL_0a6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a78: Expected O, but got Unknown
			//IL_0a92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9c: Expected O, but got Unknown
			//IL_0ab6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac0: Expected O, but got Unknown
			//IL_0ada: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae4: Expected O, but got Unknown
			//IL_0afe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b08: Expected O, but got Unknown
			//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2c: Expected O, but got Unknown
			//IL_0b46: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b50: Expected O, but got Unknown
			//IL_0b6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b74: Expected O, but got Unknown
			//IL_0b8e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b98: Expected O, but got Unknown
			//IL_0bb2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbc: Expected O, but got Unknown
			//IL_0bd6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be0: Expected O, but got Unknown
			//IL_0bfa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c04: Expected O, but got Unknown
			//IL_0c3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cbe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cfe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dbe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dfe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ec4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f07: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fd0: Unknown result type (might be due to invalid IL or missing references)
			//IL_1013: Unknown result type (might be due to invalid IL or missing references)
			//IL_1056: Unknown result type (might be due to invalid IL or missing references)
			//IL_1099: Unknown result type (might be due to invalid IL or missing references)
			//IL_10ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_114d: Unknown result type (might be due to invalid IL or missing references)
			//IL_118d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1197: Unknown result type (might be due to invalid IL or missing references)
			//IL_11ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_11f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_1251: Unknown result type (might be due to invalid IL or missing references)
			//IL_125b: Unknown result type (might be due to invalid IL or missing references)
			//IL_129a: Unknown result type (might be due to invalid IL or missing references)
			//IL_12ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_12b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_12f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_1336: Unknown result type (might be due to invalid IL or missing references)
			//IL_1375: Unknown result type (might be due to invalid IL or missing references)
			//IL_13b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_13c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_1426: Unknown result type (might be due to invalid IL or missing references)
			//IL_142b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1542: Unknown result type (might be due to invalid IL or missing references)
			//IL_156b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1575: Unknown result type (might be due to invalid IL or missing references)
			//IL_15ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_15c4: Expected O, but got Unknown
			//IL_15f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_1602: Expected O, but got Unknown
			//IL_164a: Unknown result type (might be due to invalid IL or missing references)
			//IL_170a: Unknown result type (might be due to invalid IL or missing references)
			//IL_172d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1758: Unknown result type (might be due to invalid IL or missing references)
			//IL_1762: Expected O, but got Unknown
			//IL_177c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1786: Expected O, but got Unknown
			//IL_17c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_1800: Unknown result type (might be due to invalid IL or missing references)
			//IL_1835: Unknown result type (might be due to invalid IL or missing references)
			//IL_18cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_18d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_1915: Unknown result type (might be due to invalid IL or missing references)
			//IL_1927: Unknown result type (might be due to invalid IL or missing references)
			//IL_1931: Unknown result type (might be due to invalid IL or missing references)
			//IL_1972: Unknown result type (might be due to invalid IL or missing references)
			//IL_19b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_19f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_19fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a62: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a67: Unknown result type (might be due to invalid IL or missing references)
			((Loggable)this).Log("!gameMapCallback");
			GameObject val = Object.Instantiate<GameObject>(USceneUtil.Find(gameMapBetter.areaCliffs, "Area Name (1)"));
			val.SetActive(false);
			GameObject val2 = Object.Instantiate<GameObject>(USceneUtil.Find(USceneUtil.Find(gameMapBetter.areaCliffs, "Cliffs_05"), "Sub Area Name (4)"));
			val2.SetActive(false);
			Material material = (DefaultSpriteMaterial = Object.Instantiate<Material>(((Renderer)USceneUtil.Find(gameMapBetter.areaCliffs, "Cliffs_01").GetComponent<SpriteRenderer>()).material));
			GameObject val4 = Object.Instantiate<GameObject>(USceneUtil.Find(USceneUtil.Find(gameMapBetter.areaCliffs, "Cliffs_02"), "pin_bench"));
			val4.SetActive(false);
			Dictionary<string, SCustomArea> dictionary = new Dictionary<string, SCustomArea>();
			areaWhitePalace = Object.Instantiate<GameObject>(gameMapBetter.areaCliffs, ((Component)gameMapBetter).transform);
			areaWhitePalace.SetActive(false);
			for (int num = areaWhitePalace.transform.childCount - 1; num >= 0; num--)
			{
				Object.Destroy((Object)(object)((Component)areaWhitePalace.transform.GetChild(num)).gameObject);
			}
			((Object)areaWhitePalace).name = "WHITE_PALACE";
			areaWhitePalace.layer = 5;
			areaWhitePalace.transform.localScale = Vector3.one;
			areaWhitePalace.transform.localPosition = new Vector3(-2f, 15f, gameMapBetter.areaCliffs.transform.localPosition.z);
			List<GameObject> list = new List<GameObject>();
			list.Add(new GameObject("White_Palace_01", new Type[3]
			{
				typeof(SpriteRenderer),
				typeof(RoughMapRoom),
				typeof(MappedCustomRoom)
			}));
			list.Add(new GameObject("White_Palace_02", new Type[3]
			{
				typeof(SpriteRenderer),
				typeof(RoughMapRoom),
				typeof(MappedCustomRoom)
			}));
			list.Add(new GameObject("White_Palace_03_hub", new Type[3]
			{
				typeof(SpriteRenderer),
				typeof(RoughMapRoom),
				typeof(MappedCustomRoom)
			}));
			list.Add(new GameObject("White_Palace_04", new Type[3]
			{
				typeof(SpriteRenderer),
				typeof(RoughMapRoom),
				typeof(MappedCustomRoom)
			}));
			list.Add(new GameObject("White_Palace_05", new Type[3]
			{
				typeof(SpriteRenderer),
				typeof(RoughMapRoom),
				typeof(MappedCustomRoom)
			}));
			list.Add(new GameObject("White_Palace_06", new Type[3]
			{
				typeof(SpriteRenderer),
				typeof(RoughMapRoom),
				typeof(MappedCustomRoom)
			}));
			list.Add(new GameObject("White_Palace_07", new Type[3]
			{
				typeof(SpriteRenderer),
				typeof(RoughMapRoom),
				typeof(MappedCustomRoom)
			}));
			list.Add(new GameObject("White_Palace_08", new Type[3]
			{
				typeof(SpriteRenderer),
				typeof(RoughMapRoom),
				typeof(MappedCustomRoom)
			}));
			list.Add(new GameObject("White_Palace_09", new Type[3]
			{
				typeof(SpriteRenderer),
				typeof(RoughMapRoom),
				typeof(MappedCustomRoom)
			}));
			list.Add(new GameObject("White_Palace_12", new Type[3]
			{
				typeof(SpriteRenderer),
				typeof(RoughMapRoom),
				typeof(MappedCustomRoom)
			}));
			list.Add(new GameObject("White_Palace_13", new Type[3]
			{
				typeof(SpriteRenderer),
				typeof(RoughMapRoom),
				typeof(MappedCustomRoom)
			}));
			list.Add(new GameObject("White_Palace_14", new Type[3]
			{
				typeof(SpriteRenderer),
				typeof(RoughMapRoom),
				typeof(MappedCustomRoom)
			}));
			list.Add(new GameObject("White_Palace_15", new Type[3]
			{
				typeof(SpriteRenderer),
				typeof(RoughMapRoom),
				typeof(MappedCustomRoom)
			}));
			list.Add(new GameObject("White_Palace_16", new Type[3]
			{
				typeof(SpriteRenderer),
				typeof(RoughMapRoom),
				typeof(MappedCustomRoom)
			}));
			list.Add(new GameObject("White_Palace_17", new Type[3]
			{
				typeof(SpriteRenderer),
				typeof(RoughMapRoom),
				typeof(MappedCustomRoom)
			}));
			list.Add(new GameObject("White_Palace_18", new Type[3]
			{
				typeof(SpriteRenderer),
				typeof(RoughMapRoom),
				typeof(MappedCustomRoom)
			}));
			list.Add(new GameObject("White_Palace_19", new Type[3]
			{
				typeof(SpriteRenderer),
				typeof(RoughMapRoom),
				typeof(MappedCustomRoom)
			}));
			list.Add(new GameObject("White_Palace_20", new Type[3]
			{
				typeof(SpriteRenderer),
				typeof(RoughMapRoom),
				typeof(MappedCustomRoom)
			}));
			List<GameObject> list2 = list;
			foreach (GameObject item in list2)
			{
				item.SetActive(false);
				item.transform.SetParent(areaWhitePalace.transform);
				item.layer = 5;
				item.transform.localScale = Vector3.one;
				SpriteRenderer component = item.GetComponent<SpriteRenderer>();
				((Renderer)component).material = material;
				component.sprite = SpriteDict.Get(((Object)item).name);
				((Loggable)this).Log($"Sprite for GO {((Object)item).name} is {(Object)(object)component.sprite != (Object)null}");
				((Renderer)component).sortingLayerID = 629535577;
				((Renderer)component).sortingOrder = 0;
				RoughMapRoom component2 = item.GetComponent<RoughMapRoom>();
				component2.fullSprite = component.sprite;
				component2.fullSpriteDisplayed = false;
			}
			float z = gameMapBetter.areaCliffs.transform.GetChild(6).localPosition.z;
			list2[0].transform.localPosition = new Vector3(-45f / 64f, -4.70625f, z);
			list2[1].transform.localPosition = new Vector3(1.605f, -5.038125f, z);
			list2[2].transform.localPosition = new Vector3(-0.056250002f, -2.805f, z);
			list2[3].transform.localPosition = new Vector3(-2.0812502f, -3.0881252f, z);
			list2[4].transform.localPosition = new Vector3(2.085f, -2.049375f, z);
			list2[5].transform.localPosition = new Vector3(0.097500004f, -1.344375f, z);
			list2[6].transform.localPosition = new Vector3(0.0675f, 0.125625f, z);
			list2[7].transform.localPosition = new Vector3(3.695625f, 1.5937501f, z);
			list2[8].transform.localPosition = new Vector3(-0.73312503f, 3.3825002f, z);
			list2[9].transform.localPosition = new Vector3(-0.21937501f, 0.855f, z);
			list2[10].transform.localPosition = new Vector3(1.449375f, 1.0725f, z);
			list2[11].transform.localPosition = new Vector3(-2.05125f, -2.44125f, z);
			list2[12].transform.localPosition = new Vector3(1.11f, -2.2518752f, z);
			list2[13].transform.localPosition = new Vector3(2.6381252f, -2.150625f, z);
			list2[14].transform.localPosition = new Vector3(-4.100625f, -0.53062505f, z);
			list2[15].transform.localPosition = new Vector3(-2.8256252f, -1.2f, z);
			list2[16].transform.localPosition = new Vector3(-3.2193751f, -0.17437501f, z);
			list2[17].transform.localPosition = new Vector3(-2.4225001f, 0.075f, z);
			list2[17].transform.localScale = new Vector3(0.93f, 1.04f, 1f);
			list = new List<GameObject>();
			list.Add(new GameObject("RWP01", new Type[1] { typeof(SpriteRenderer) }));
			list.Add(new GameObject("RWP02", new Type[1] { typeof(SpriteRenderer) }));
			list.Add(new GameObject("RWP03", new Type[1] { typeof(SpriteRenderer) }));
			list.Add(new GameObject("RWP04", new Type[1] { typeof(SpriteRenderer) }));
			list.Add(new GameObject("RWP05", new Type[1] { typeof(SpriteRenderer) }));
			list.Add(new GameObject("RWP06", new Type[1] { typeof(SpriteRenderer) }));
			list.Add(new GameObject("RWP07", new Type[1] { typeof(SpriteRenderer) }));
			list.Add(new GameObject("RWP08", new Type[1] { typeof(SpriteRenderer) }));
			list.Add(new GameObject("RWP09", new Type[1] { typeof(SpriteRenderer) }));
			list.Add(new GameObject("RWP12", new Type[1] { typeof(SpriteRenderer) }));
			list.Add(new GameObject("RWP13", new Type[1] { typeof(SpriteRenderer) }));
			list.Add(new GameObject("RWP14", new Type[1] { typeof(SpriteRenderer) }));
			list.Add(new GameObject("RWP15", new Type[1] { typeof(SpriteRenderer) }));
			list.Add(new GameObject("RWP16", new Type[1] { typeof(SpriteRenderer) }));
			list.Add(new GameObject("RWP17", new Type[1] { typeof(SpriteRenderer) }));
			list.Add(new GameObject("RWP18", new Type[1] { typeof(SpriteRenderer) }));
			list.Add(new GameObject("RWP19", new Type[1] { typeof(SpriteRenderer) }));
			list.Add(new GameObject("RWP20", new Type[1] { typeof(SpriteRenderer) }));
			List<GameObject> list3 = list;
			list3[0].transform.SetParent(list2[0].transform);
			list3[0].transform.localPosition = new Vector3(0.043125004f, 0.0062500015f);
			list3[1].transform.SetParent(list2[1].transform);
			list3[1].transform.localPosition = new Vector3(0.120000005f, -0.027f);
			list3[2].transform.SetParent(list2[2].transform);
			list3[2].transform.localPosition = new Vector3(0.0065625003f, 0f);
			list3[3].transform.SetParent(list2[3].transform);
			list3[3].transform.localPosition = new Vector3(0.031125f, -0.0069375006f);
			list3[4].transform.SetParent(list2[4].transform);
			list3[4].transform.localPosition = new Vector3(0.33f, 0.039375f);
			list3[5].transform.SetParent(list2[5].transform);
			list3[5].transform.localPosition = new Vector3(0.24249996f, -0.005625f);
			list3[6].transform.SetParent(list2[6].transform);
			list3[6].transform.localPosition = new Vector3(0.16250001f, 0.009375f);
			list3[7].transform.SetParent(list2[7].transform);
			list3[7].transform.localPosition = new Vector3(0.059375007f, 0.0062500015f);
			list3[8].transform.SetParent(list2[8].transform);
			list3[8].transform.localPosition = new Vector3(0.032812502f, -0.25256252f);
			list3[9].transform.SetParent(list2[9].transform);
			list3[9].transform.localPosition = new Vector3(0.0045000003f, 0.82500005f);
			list3[10].transform.SetParent(list2[10].transform);
			list3[10].transform.localPosition = new Vector3(0.015625004f, 0.74250007f);
			list3[11].transform.SetParent(list2[11].transform);
			list3[11].transform.localPosition = new Vector3(0.0315f, 0.4362501f);
			list3[12].transform.SetParent(list2[12].transform);
			list3[12].transform.localPosition = new Vector3(0.060000002f, 0.10187502f);
			list3[13].transform.SetParent(list2[13].transform);
			list3[13].transform.localPosition = new Vector3(0.84637505f, -0.019374995f);
			list3[14].transform.SetParent(list2[14].transform);
			list3[14].transform.localPosition = new Vector3(0.010833334f, -0.009375f);
			list3[15].transform.SetParent(list2[15].transform);
			list3[15].transform.localPosition = new Vector3(0.825625f, -0.035000008f);
			list3[16].transform.SetParent(list2[16].transform);
			list3[16].transform.localPosition = new Vector3(-0.1305f, 0.17943752f);
			list3[17].transform.SetParent(list2[17].transform);
			list3[17].transform.localPosition = new Vector3(0.008125f, 0.79312503f);
			foreach (GameObject item2 in list3)
			{
				item2.SetActive(false);
				item2.layer = 5;
				item2.transform.localScale = Vector3.one;
				SpriteRenderer component3 = item2.GetComponent<SpriteRenderer>();
				((Renderer)component3).material = material;
				component3.sprite = SpriteDict.Get(((Object)item2).name);
				((Renderer)component3).sortingLayerID = 629535577;
				((Renderer)component3).sortingOrder = 0;
				item2.SetActive(true);
			}
			list3[17].transform.localScale = new Vector3(1.0752689f, 0.9615385f, 1f);
			GameObject val5 = Object.Instantiate<GameObject>(val2, list2[15].transform);
			val5.SetActive(false);
			val5.transform.localPosition = new Vector3(5.875f, -0.8f, val5.transform.localPosition.z);
			val5.GetComponent<SetTextMeshProGameText>().convName = "PathOfPainArea";
			val5.SetActive(true);
			GameObject val6 = Object.Instantiate<GameObject>(val2, list2[7].transform);
			val6.SetActive(false);
			val6.transform.localPosition = new Vector3(5f, -1.25f, val6.transform.localPosition.z);
			val6.GetComponent<SetTextMeshProGameText>().convName = "WorkshopArea";
			val6.SetActive(true);
			GameObject val7 = Object.Instantiate<GameObject>(val2, list2[6].transform);
			val7.SetActive(false);
			val7.transform.localPosition = new Vector3(7f, -1.5f, val7.transform.localPosition.z);
			val7.transform.SetParent(areaWhitePalace.transform, true);
			val7.GetComponent<SetTextMeshProGameText>().convName = "CreditsArea";
			RectTransform component4 = val7.GetComponent<RectTransform>();
			component4.sizeDelta = new Vector2(component4.sizeDelta.x + 1f, component4.sizeDelta.y);
			val7.SetActive(true);
			GameObject obj = Object.Instantiate<GameObject>(val4, list3[0].transform);
			obj.SetActive(false);
			obj.transform.localPosition = new Vector3(-0.4f, -0.5f, -0.013f);
			obj.SetActive(true);
			GameObject obj2 = Object.Instantiate<GameObject>(val4, list3[2].transform);
			obj2.SetActive(false);
			obj2.transform.localPosition = new Vector3(0.05f, -0.15f, -0.013f);
			obj2.SetActive(true);
			GameObject obj3 = Object.Instantiate<GameObject>(val4, list3[5].transform);
			obj3.SetActive(false);
			obj3.transform.localPosition = new Vector3(-0.1f, 0.45f, -0.013f);
			obj3.SetActive(true);
			GameObject val8 = Object.Instantiate<GameObject>(val, areaWhitePalace.transform);
			val8.SetActive(false);
			val8.transform.localPosition = new Vector3(6.433125f, 1.6825f, val8.transform.localPosition.z);
			val8.GetComponent<SetTextMeshProGameText>().convName = "WHITE_PALACE";
			val8.SetActive(true);
			areaWhitePalace.SetActive(true);
			areaWhitePalace.SetActive(false);
			dictionary.Add("WHITE_PALACE", new SCustomArea
			{
				AreaGameObject = areaWhitePalace,
				CameraPosition = new Vector3(3.07f, -23f, 18f),
				MapZoneStrings = new List<string> { "WHITE_PALACE" },
				PlayerDataBoolGotAreaMap = "AdditionalMapsGotWpMap",
				panMinX = null,
				panMaxX = null,
				panMinY = -20f,
				panMaxY = null
			});
			GameObject obj4 = list2.Find((GameObject s) => ((Object)s).name == "White_Palace_03_hub");
			if (obj4 != null)
			{
				obj4.SetActive(true);
			}
			areaGodhome = Object.Instantiate<GameObject>(gameMapBetter.areaCliffs, ((Component)gameMapBetter).transform);
			areaGodhome.SetActive(false);
			for (int num2 = areaGodhome.transform.childCount - 1; num2 >= 0; num2--)
			{
				Object.Destroy((Object)(object)((Component)areaGodhome.transform.GetChild(num2)).gameObject);
			}
			((Object)areaGodhome).name = "GODS_GLORY";
			areaGodhome.layer = 5;
			areaGodhome.transform.localScale = Vector3.one;
			areaGodhome.transform.localPosition = new Vector3(5.5f, 14.5f, gameMapBetter.areaCliffs.transform.localPosition.z);
			list = new List<GameObject>();
			list.Add(new GameObject("GG_Atrium", new Type[3]
			{
				typeof(SpriteRenderer),
				typeof(RoughMapRoom),
				typeof(MappedCustomRoom)
			}));
			list.Add(new GameObject("GG_Atrium_Roof", new Type[3]
			{
				typeof(SpriteRenderer),
				typeof(RoughMapRoom),
				typeof(MappedCustomRoom)
			}));
			List<GameObject> list4 = list;
			foreach (GameObject item3 in list4)
			{
				item3.transform.SetParent(areaGodhome.transform);
				item3.SetActive(false);
				item3.layer = 5;
				item3.transform.localScale = Vector3.one;
				SpriteRenderer component5 = item3.GetComponent<SpriteRenderer>();
				((Renderer)component5).material = material;
				component5.sprite = SpriteDict.Get(((Object)item3).name);
				((Loggable)this).Log($"Sprite for GO {((Object)item3).name} is {(Object)(object)component5.sprite != (Object)null}");
				((Renderer)component5).sortingLayerID = 629535577;
				((Renderer)component5).sortingOrder = 0;
				RoughMapRoom component6 = item3.GetComponent<RoughMapRoom>();
				component6.fullSprite = component5.sprite;
				component6.fullSpriteDisplayed = false;
			}
			list4[0].transform.localPosition = new Vector3(0.3687f, -2.678f, z);
			list4[1].transform.localPosition = new Vector3(-0.708f, 0.65f, z);
			list = new List<GameObject>();
			list.Add(new GameObject("RGH0", new Type[1] { typeof(SpriteRenderer) }));
			list.Add(new GameObject("RGH1", new Type[1] { typeof(SpriteRenderer) }));
			List<GameObject> list5 = list;
			list5[0].transform.SetParent(list4[0].transform);
			list5[0].transform.localPosition = new Vector3(-0.408678f, 0.138f);
			list5[1].transform.SetParent(list4[1].transform);
			list5[1].transform.localPosition = new Vector3(0.6480024f, -0.187f);
			foreach (GameObject item4 in list5)
			{
				item4.SetActive(false);
				item4.layer = 5;
				item4.transform.localScale = Vector3.one;
				SpriteRenderer component7 = item4.GetComponent<SpriteRenderer>();
				((Renderer)component7).material = material;
				component7.sprite = SpriteDict.Get(((Object)item4).name);
				((Renderer)component7).sortingLayerID = 629535577;
				((Renderer)component7).sortingOrder = 0;
				item4.SetActive(true);
			}
			GameObject val9 = Object.Instantiate<GameObject>(val2, list4[0].transform);
			val9.SetActive(false);
			val9.transform.localPosition = new Vector3(8f, 1.5f, val9.transform.localPosition.z);
			val9.transform.SetParent(areaGodhome.transform, true);
			val9.GetComponent<SetTextMeshProGameText>().convName = "CreditsArea";
			RectTransform component8 = val9.GetComponent<RectTransform>();
			component8.sizeDelta = new Vector2(component8.sizeDelta.x + 1f, component8.sizeDelta.y);
			val9.SetActive(true);
			GameObject obj5 = Object.Instantiate<GameObject>(val4, list5[0].transform);
			obj5.SetActive(false);
			obj5.transform.localPosition = new Vector3(0.925f, 0.5f, -0.013f);
			obj5.SetActive(true);
			GameObject obj6 = Object.Instantiate<GameObject>(val4, list5[1].transform);
			obj6.SetActive(false);
			obj6.transform.localPosition = new Vector3(0.8f, -0.1f, -0.013f);
			obj6.SetActive(true);
			GameObject val10 = Object.Instantiate<GameObject>(val, areaGodhome.transform);
			val10.SetActive(false);
			val10.transform.localPosition = new Vector3(5.208f, 0.65f, val10.transform.localPosition.z);
			val10.GetComponent<SetTextMeshProGameText>().convName = "GODS_GLORY";
			val10.SetActive(true);
			areaGodhome.SetActive(true);
			areaGodhome.SetActive(false);
			dictionary.Add("GODS_GLORY", new SCustomArea
			{
				AreaGameObject = areaGodhome,
				CameraPosition = new Vector3(-8.5f, -22f, 18f),
				MapZoneStrings = new List<string> { "GODS_GLORY" },
				PlayerDataBoolGotAreaMap = "AdditionalMapsGotGhMap",
				panMinX = -5f,
				panMaxX = null,
				panMinY = -19f,
				panMaxY = null
			});
			GameObject obj7 = list4.Find((GameObject s) => ((Object)s).name == "GG_Atrium");
			if (obj7 != null)
			{
				obj7.SetActive(true);
			}
			((Loggable)this).Log("~gameMapCallback");
			return dictionary;
		}

		private void InitCallbacks()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Expected O, but got Unknown
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Expected O, but got Unknown
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Expected O, but got Unknown
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Expected O, but got Unknown
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			object obj = <>O.<0>__OnPlayMakerFSMStart;
			if (obj == null)
			{
				hook_Start val = OnPlayMakerFSMStart;
				<>O.<0>__OnPlayMakerFSMStart = val;
				obj = (object)val;
			}
			PlayMakerFSM.Start += (hook_Start)obj;
			object obj2 = <>O.<1>__OnStartNewGame;
			if (obj2 == null)
			{
				hook_StartNewGame val2 = OnStartNewGame;
				<>O.<1>__OnStartNewGame = val2;
				obj2 = (object)val2;
			}
			GameManager.StartNewGame += (hook_StartNewGame)obj2;
			object obj3 = <>O.<2>__OnContinueGame;
			if (obj3 == null)
			{
				hook_ContinueGame val3 = OnContinueGame;
				<>O.<2>__OnContinueGame = val3;
				obj3 = (object)val3;
			}
			GameManager.ContinueGame += (hook_ContinueGame)obj3;
			ModHooks.LanguageGetHook += new LanguageGetProxy(OnLanguageGetHook);
			ModHooks.GetPlayerBoolHook += new GetBoolProxy(OnGetPlayerBoolHook);
			ModHooks.SetPlayerBoolHook += new SetBoolProxy(OnSetPlayerBoolHook);
			SceneManager.activeSceneChanged += OnSceneManagerActiveSceneChanged;
			SceneManager.AddSceneMapped += new Manipulator(OnSceneManagerAddSceneMapped);
		}

		private static void OnStartNewGame(orig_StartNewGame orig, GameManager self, bool permadeathMode, bool bossRushMode)
		{
			orig.Invoke(self, permadeathMode, bossRushMode);
			TruncateScenesMapped();
		}

		private static void OnContinueGame(orig_ContinueGame orig, GameManager self)
		{
			orig.Invoke(self);
			TruncateScenesMapped();
		}

		private static void TruncateScenesMapped()
		{
			string[] allScenes = _allScenes;
			foreach (string item in allScenes)
			{
				if (!PlayerData.instance.scenesVisited.Contains(item))
				{
					PlayerData.instance.scenesMapped.Remove(item);
				}
			}
		}

		private static void RestoreTruncatedScenesMapped()
		{
			if (PlayerData.instance.scenesMapped.Contains("Cinematic_Stag_travel"))
			{
				PlayerData.instance.scenesMapped.Add("Cinematic_Stag_travel");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_Town_Stag_Station"))
			{
				PlayerData.instance.scenesMapped.Add("Room_Town_Stag_Station");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_Charm_Shop"))
			{
				PlayerData.instance.scenesMapped.Add("Room_Charm_Shop");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_Mender_House"))
			{
				PlayerData.instance.scenesMapped.Add("Room_Mender_House");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_mapper"))
			{
				PlayerData.instance.scenesMapped.Add("Room_mapper");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_nailmaster"))
			{
				PlayerData.instance.scenesMapped.Add("Room_nailmaster");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_nailmaster_02"))
			{
				PlayerData.instance.scenesMapped.Add("Room_nailmaster_02");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_nailmaster_03"))
			{
				PlayerData.instance.scenesMapped.Add("Room_nailmaster_03");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_shop"))
			{
				PlayerData.instance.scenesMapped.Add("Room_shop");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_nailsmith"))
			{
				PlayerData.instance.scenesMapped.Add("Room_nailsmith");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_temple"))
			{
				PlayerData.instance.scenesMapped.Add("Room_temple");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_ruinhouse"))
			{
				PlayerData.instance.scenesMapped.Add("Room_ruinhouse");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_Mansion"))
			{
				PlayerData.instance.scenesMapped.Add("Room_Mansion");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_Tram"))
			{
				PlayerData.instance.scenesMapped.Add("Room_Tram");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_Tram_RG"))
			{
				PlayerData.instance.scenesMapped.Add("Room_Tram_RG");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_Bretta"))
			{
				PlayerData.instance.scenesMapped.Add("Room_Bretta");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_Fungus_Shaman"))
			{
				PlayerData.instance.scenesMapped.Add("Room_Fungus_Shaman");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_Ouiji"))
			{
				PlayerData.instance.scenesMapped.Add("Room_Ouiji");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_Jinn"))
			{
				PlayerData.instance.scenesMapped.Add("Room_Jinn");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_Colosseum_01"))
			{
				PlayerData.instance.scenesMapped.Add("Room_Colosseum_01");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_Colosseum_02"))
			{
				PlayerData.instance.scenesMapped.Add("Room_Colosseum_02");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_Colosseum_03"))
			{
				PlayerData.instance.scenesMapped.Add("Room_Colosseum_03");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_Colosseum_Bronze"))
			{
				PlayerData.instance.scenesMapped.Add("Room_Colosseum_Bronze");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_Colosseum_Silver"))
			{
				PlayerData.instance.scenesMapped.Add("Room_Colosseum_Silver");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_Colosseum_Gold"))
			{
				PlayerData.instance.scenesMapped.Add("Room_Colosseum_Gold");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_Slug_Shrine"))
			{
				PlayerData.instance.scenesMapped.Add("Room_Slug_Shrine");
			}
			if (PlayerData.instance.scenesMapped.Contains("Crossroads_ShamanTemple"))
			{
				PlayerData.instance.scenesMapped.Add("Crossroads_ShamanTemple");
			}
			if (PlayerData.instance.scenesMapped.Contains("Ruins_House_01"))
			{
				PlayerData.instance.scenesMapped.Add("Ruins_House_01");
			}
			if (PlayerData.instance.scenesMapped.Contains("Ruins_House_02"))
			{
				PlayerData.instance.scenesMapped.Add("Ruins_House_02");
			}
			if (PlayerData.instance.scenesMapped.Contains("Ruins_House_03"))
			{
				PlayerData.instance.scenesMapped.Add("Ruins_House_03");
			}
			if (PlayerData.instance.scenesMapped.Contains("Fungus1_35"))
			{
				PlayerData.instance.scenesMapped.Add("Fungus1_35");
			}
			if (PlayerData.instance.scenesMapped.Contains("Fungus1_36"))
			{
				PlayerData.instance.scenesMapped.Add("Fungus1_36");
			}
			if (PlayerData.instance.scenesMapped.Contains("Fungus3_archive"))
			{
				PlayerData.instance.scenesMapped.Add("Fungus3_archive");
			}
			if (PlayerData.instance.scenesMapped.Contains("Fungus3_archive_02"))
			{
				PlayerData.instance.scenesMapped.Add("Fungus3_archive_02");
			}
			if (PlayerData.instance.scenesMapped.Contains("Cliffs_03"))
			{
				PlayerData.instance.scenesMapped.Add("Cliffs_03");
			}
			if (PlayerData.instance.scenesMapped.Contains("RestingGrounds_07"))
			{
				PlayerData.instance.scenesMapped.Add("RestingGrounds_07");
			}
			if (PlayerData.instance.scenesMapped.Contains("Deepnest_45_v02"))
			{
				PlayerData.instance.scenesMapped.Add("Deepnest_45_v02");
			}
			if (PlayerData.instance.scenesMapped.Contains("Deepnest_Spider_Town"))
			{
				PlayerData.instance.scenesMapped.Add("Deepnest_Spider_Town");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_spider_small"))
			{
				PlayerData.instance.scenesMapped.Add("Room_spider_small");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_Wyrm"))
			{
				PlayerData.instance.scenesMapped.Add("Room_Wyrm");
			}
			if (PlayerData.instance.scenesMapped.Contains("Abyss_Lighthouse_room"))
			{
				PlayerData.instance.scenesMapped.Add("Abyss_Lighthouse_room");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_Queen"))
			{
				PlayerData.instance.scenesMapped.Add("Room_Queen");
			}
			if (PlayerData.instance.scenesMapped.Contains("White_Palace_01"))
			{
				PlayerData.instance.scenesMapped.Add("White_Palace_01");
			}
			if (PlayerData.instance.scenesMapped.Contains("White_Palace_02"))
			{
				PlayerData.instance.scenesMapped.Add("White_Palace_02");
			}
			if (PlayerData.instance.scenesMapped.Contains("White_Palace_03_hub"))
			{
				PlayerData.instance.scenesMapped.Add("White_Palace_03_hub");
			}
			if (PlayerData.instance.scenesMapped.Contains("White_Palace_04"))
			{
				PlayerData.instance.scenesMapped.Add("White_Palace_04");
			}
			if (PlayerData.instance.scenesMapped.Contains("White_Palace_05"))
			{
				PlayerData.instance.scenesMapped.Add("White_Palace_05");
			}
			if (PlayerData.instance.scenesMapped.Contains("White_Palace_06"))
			{
				PlayerData.instance.scenesMapped.Add("White_Palace_06");
			}
			if (PlayerData.instance.scenesMapped.Contains("White_Palace_07"))
			{
				PlayerData.instance.scenesMapped.Add("White_Palace_07");
			}
			if (PlayerData.instance.scenesMapped.Contains("White_Palace_08"))
			{
				PlayerData.instance.scenesMapped.Add("White_Palace_08");
			}
			if (PlayerData.instance.scenesMapped.Contains("White_Palace_09"))
			{
				PlayerData.instance.scenesMapped.Add("White_Palace_09");
			}
			if (PlayerData.instance.scenesMapped.Contains("White_Palace_11"))
			{
				PlayerData.instance.scenesMapped.Add("White_Palace_11");
			}
			if (PlayerData.instance.scenesMapped.Contains("White_Palace_12"))
			{
				PlayerData.instance.scenesMapped.Add("White_Palace_12");
			}
			if (PlayerData.instance.scenesMapped.Contains("White_Palace_13"))
			{
				PlayerData.instance.scenesMapped.Add("White_Palace_13");
			}
			if (PlayerData.instance.scenesMapped.Contains("White_Palace_14"))
			{
				PlayerData.instance.scenesMapped.Add("White_Palace_14");
			}
			if (PlayerData.instance.scenesMapped.Contains("White_Palace_15"))
			{
				PlayerData.instance.scenesMapped.Add("White_Palace_15");
			}
			if (PlayerData.instance.scenesMapped.Contains("White_Palace_16"))
			{
				PlayerData.instance.scenesMapped.Add("White_Palace_16");
			}
			if (PlayerData.instance.scenesMapped.Contains("Dream_Nailcollection"))
			{
				PlayerData.instance.scenesMapped.Add("Dream_Nailcollection");
			}
			if (PlayerData.instance.scenesMapped.Contains("Dream_01_False_Knight"))
			{
				PlayerData.instance.scenesMapped.Add("Dream_01_False_Knight");
			}
			if (PlayerData.instance.scenesMapped.Contains("Dream_03_Infected_Knight"))
			{
				PlayerData.instance.scenesMapped.Add("Dream_03_Infected_Knight");
			}
			if (PlayerData.instance.scenesMapped.Contains("Dream_02_Mage_Lord"))
			{
				PlayerData.instance.scenesMapped.Add("Dream_02_Mage_Lord");
			}
			if (PlayerData.instance.scenesMapped.Contains("Dream_Guardian"))
			{
				PlayerData.instance.scenesMapped.Add("Dream_Guardian");
			}
			if (PlayerData.instance.scenesMapped.Contains("Dream_Guardian_Hegemol"))
			{
				PlayerData.instance.scenesMapped.Add("Dream_Guardian_Hegemol");
			}
			if (PlayerData.instance.scenesMapped.Contains("Dream_Guardian_Lurien"))
			{
				PlayerData.instance.scenesMapped.Add("Dream_Guardian_Lurien");
			}
			if (PlayerData.instance.scenesMapped.Contains("Dream_Guardian_Monomon"))
			{
				PlayerData.instance.scenesMapped.Add("Dream_Guardian_Monomon");
			}
			if (PlayerData.instance.scenesMapped.Contains("Cutscene_Boss_Door"))
			{
				PlayerData.instance.scenesMapped.Add("Cutscene_Boss_Door");
			}
			if (PlayerData.instance.scenesMapped.Contains("Dream_Backer_Shrine"))
			{
				PlayerData.instance.scenesMapped.Add("Dream_Backer_Shrine");
			}
			if (PlayerData.instance.scenesMapped.Contains("Dream_Room_Believer_Shrine"))
			{
				PlayerData.instance.scenesMapped.Add("Dream_Room_Believer_Shrine");
			}
			if (PlayerData.instance.scenesMapped.Contains("Dream_Abyss"))
			{
				PlayerData.instance.scenesMapped.Add("Dream_Abyss");
			}
			if (PlayerData.instance.scenesMapped.Contains("Dream_Final_Boss"))
			{
				PlayerData.instance.scenesMapped.Add("Dream_Final_Boss");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_Final_Boss_Atrium"))
			{
				PlayerData.instance.scenesMapped.Add("Room_Final_Boss_Atrium");
			}
			if (PlayerData.instance.scenesMapped.Contains("Room_Final_Boss_Core"))
			{
				PlayerData.instance.scenesMapped.Add("Room_Final_Boss_Core");
			}
			if (PlayerData.instance.scenesMapped.Contains("Cinematic_Ending_A"))
			{
				PlayerData.instance.scenesMapped.Add("Cinematic_Ending_A");
			}
			if (PlayerData.instance.scenesMapped.Contains("Cinematic_Ending_B"))
			{
				PlayerData.instance.scenesMapped.Add("Cinematic_Ending_B");
			}
			if (PlayerData.instance.scenesMapped.Contains("Cinematic_Ending_C"))
			{
				PlayerData.instance.scenesMapped.Add("Cinematic_Ending_C");
			}
			if (PlayerData.instance.scenesMapped.Contains("Cinematic_Ending_D"))
			{
				PlayerData.instance.scenesMapped.Add("Cinematic_Ending_D");
			}
			if (PlayerData.instance.scenesMapped.Contains("Cinematic_Ending_E"))
			{
				PlayerData.instance.scenesMapped.Add("Cinematic_Ending_E");
			}
			if (PlayerData.instance.scenesMapped.Contains("End_Credits"))
			{
				PlayerData.instance.scenesMapped.Add("End_Credits");
			}
			if (PlayerData.instance.scenesMapped.Contains("Cinematic_MrMushroom"))
			{
				PlayerData.instance.scenesMapped.Add("Cinematic_MrMushroom");
			}
			if (PlayerData.instance.scenesMapped.Contains("End_Game_Completion"))
			{
				PlayerData.instance.scenesMapped.Add("End_Game_Completion");
			}
			if (PlayerData.instance.scenesMapped.Contains("PermaDeath"))
			{
				PlayerData.instance.scenesMapped.Add("PermaDeath");
			}
			if (PlayerData.instance.scenesMapped.Contains("PermaDeath_Unlock"))
			{
				PlayerData.instance.scenesMapped.Add("PermaDeath_Unlock");
			}
			if (PlayerData.instance.scenesMapped.Contains("Deepnest_East_17"))
			{
				PlayerData.instance.scenesMapped.Add("Deepnest_East_17");
			}
		}

		private void OnSceneManagerAddSceneMapped(ILContext il)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			ILCursor val = new ILCursor(il);
			val.Goto(0, (MoveType)0, false);
			val.GotoNext((MoveType)0, new Func<Instruction, bool>[2]
			{
				(Instruction x) => ILPatternMatchingExt.MatchLdarg(x, 0),
				(Instruction x) => ILPatternMatchingExt.MatchLdfld<SceneManager>(x, "mapZone")
			});
			val.RemoveRange(8);
		}

		private void OnSceneManagerActiveSceneChanged(Scene from, Scene to)
		{
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0437: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_0550: Unknown result type (might be due to invalid IL or missing references)
			//IL_0555: Unknown result type (might be due to invalid IL or missing references)
			//IL_057d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0591: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_085c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0888: Unknown result type (might be due to invalid IL or missing references)
			if (_sceneList.Contains(((Scene)(ref to)).name))
			{
				switch (((Scene)(ref to)).name)
				{
				case "White_Palace_03_hub":
				{
					MakeSpriteGo("sm1", GetSprite("SM1"), new Vector3(55.2f, 45.1f, 0.1f), Vector3.zero);
					MakeSpriteGo("sm2", GetSprite("SM2"), new Vector3(56.3f, 45.1f, 0.1f), new Vector3(0f, 0f, -1.374f));
					MakeSpriteGo("sm2", GetSprite("SM2"), new Vector3(57.5f, 45.1f, 0.17f), new Vector3(0f, 0f, 14.291f));
					MakeSpriteGo("sm3", GetSprite("SM3"), new Vector3(58.6f, 45.2f, 0.1f), Vector3.zero);
					MakeSpriteGo("sm3", GetSprite("SM3"), new Vector3(59.7f, 45.2f, 0.1f), new Vector3(0f, 0f, -5.748f));
					GameObject obj3 = Object.Instantiate<GameObject>(_shinyPrefab);
					((Object)obj3).name = "Map";
					obj3.SetActive(false);
					((Component)obj3.transform.GetChild(0)).gameObject.SetActive(true);
					obj3.transform.position = new Vector3(57.5f, 45f, 0.05f);
					PlayMakerFSM obj4 = FSMUtility.LocateMyFSM(((Component)obj3.transform.GetChild(0)).gameObject, "Shiny Control");
					FsmVariables fsmVariables2 = obj4.FsmVariables;
					fsmVariables2.FindFsmInt("Charm ID").Value = 0;
					fsmVariables2.FindFsmInt("Type").Value = 0;
					fsmVariables2.FindFsmBool("Activated").Value = false;
					fsmVariables2.FindFsmBool("Charm").Value = false;
					fsmVariables2.FindFsmBool("Dash Cloak").Value = false;
					fsmVariables2.FindFsmBool("Exit Dream").Value = false;
					fsmVariables2.FindFsmBool("Fling L").Value = false;
					fsmVariables2.FindFsmBool("Fling On Start").Value = false;
					fsmVariables2.FindFsmBool("Journal").Value = false;
					fsmVariables2.FindFsmBool("King's Brand").Value = false;
					fsmVariables2.FindFsmBool("Mantis Claw").Value = false;
					fsmVariables2.FindFsmBool("Pure Seed").Value = false;
					fsmVariables2.FindFsmBool("Quake").Value = false;
					fsmVariables2.FindFsmBool("Show Charm Tute").Value = false;
					fsmVariables2.FindFsmBool("Slug Fling").Value = false;
					fsmVariables2.FindFsmBool("Super Dash").Value = false;
					fsmVariables2.FindFsmString("Item Name").Value = "SFGrenadeAdditionalMaps_WpMapName";
					fsmVariables2.FindFsmString("PD Bool Name").Value = "AdditionalMapsGotWpMap";
					IntSwitch action2 = FsmUtil.GetAction<IntSwitch>(obj4, "Trinket Type", 0);
					List<FsmInt> list3 = new List<FsmInt>(action2.compareTo);
					list3.Add(FsmInt.op_Implicit(list3.Count + 1));
					action2.compareTo = list3.ToArray();
					fsmVariables2.FindFsmInt("Trinket Num").Value = list3.Count;
					List<FsmEvent> list4 = new List<FsmEvent>(action2.sendEvent) { FsmEvent.FindEvent("PURE SEED") };
					action2.sendEvent = list4.ToArray();
					FsmUtil.CopyState(obj4, "Love Key", "Necklace");
					FsmUtil.GetAction<SetPlayerDataBool>(obj4, "Necklace", 0).boolName = FsmString.op_Implicit("AdditionalMapsGotWpMap");
					FsmUtil.GetAction<SetSpriteRendererSprite>(obj4, "Necklace", 1).sprite = FsmObject.op_Implicit((Object)(object)GetSprite("MapIcon"));
					FsmUtil.GetAction<GetLanguageString>(obj4, "Necklace", 2).convName = FsmString.op_Implicit("SFGrenadeAdditionalMaps_WpMapName");
					FsmUtil.AddTransition(obj4, "Trinket Type", "PURE SEED", "Necklace");
					obj3.SetActive(true);
					break;
				}
				case "White_Palace_08":
					USceneUtil.Find(to, "_SceneManager").GetComponent<SceneManager>().mapZone = (MapZone)21;
					break;
				case "GG_Atrium":
				{
					GameObject val3 = USceneUtil.Find(to, "door1_blueRoom");
					_setCompassPointPrefab.CopyOnto<PlayMakerFSM>(val3);
					FSMUtility.LocateMyFSM(val3, "Set Compass Point").Preprocess();
					GameObject val4 = USceneUtil.Find(to, "Door_Workshop");
					_setCompassPointPrefab.CopyOnto<PlayMakerFSM>(val4);
					FSMUtility.LocateMyFSM(val4, "Set Compass Point").Preprocess();
					MakeSpriteGo("sm1", GetSprite("SM1"), new Vector3(115.7f, 60.1f, 0.1f), Vector3.zero);
					MakeSpriteGo("sm2", GetSprite("SM2"), new Vector3(116.7f, 60.1f, 0.1f), new Vector3(0f, 0f, -1.374f));
					MakeSpriteGo("sm2", GetSprite("SM2"), new Vector3(118f, 60.1f, 0.17f), new Vector3(0f, 0f, 14.291f));
					MakeSpriteGo("sm3", GetSprite("SM3"), new Vector3(119.1f, 60.2f, 0.1f), Vector3.zero);
					MakeSpriteGo("sm3", GetSprite("SM3"), new Vector3(120.2f, 60.2f, 0.1f), new Vector3(0f, 0f, -5.748f));
					GameObject obj = Object.Instantiate<GameObject>(_shinyPrefab);
					((Object)obj).name = "Map";
					obj.SetActive(false);
					((Component)obj.transform.GetChild(0)).gameObject.SetActive(true);
					obj.transform.position = new Vector3(118f, 60.5f, 0.05f);
					PlayMakerFSM obj2 = FSMUtility.LocateMyFSM(((Component)obj.transform.GetChild(0)).gameObject, "Shiny Control");
					FsmVariables fsmVariables = obj2.FsmVariables;
					fsmVariables.FindFsmInt("Charm ID").Value = 0;
					fsmVariables.FindFsmInt("Type").Value = 0;
					fsmVariables.FindFsmBool("Activated").Value = false;
					fsmVariables.FindFsmBool("Charm").Value = false;
					fsmVariables.FindFsmBool("Dash Cloak").Value = false;
					fsmVariables.FindFsmBool("Exit Dream").Value = false;
					fsmVariables.FindFsmBool("Fling L").Value = false;
					fsmVariables.FindFsmBool("Fling On Start").Value = false;
					fsmVariables.FindFsmBool("Journal").Value = false;
					fsmVariables.FindFsmBool("King's Brand").Value = false;
					fsmVariables.FindFsmBool("Mantis Claw").Value = false;
					fsmVariables.FindFsmBool("Pure Seed").Value = false;
					fsmVariables.FindFsmBool("Quake").Value = false;
					fsmVariables.FindFsmBool("Show Charm Tute").Value = false;
					fsmVariables.FindFsmBool("Slug Fling").Value = false;
					fsmVariables.FindFsmBool("Super Dash").Value = false;
					fsmVariables.FindFsmString("Item Name").Value = "SFGrenadeAdditionalMaps_GhMapName";
					fsmVariables.FindFsmString("PD Bool Name").Value = "AdditionalMapsGotGhMap";
					IntSwitch action = FsmUtil.GetAction<IntSwitch>(obj2, "Trinket Type", 0);
					List<FsmInt> list = new List<FsmInt>(action.compareTo);
					list.Add(FsmInt.op_Implicit(list.Count + 1));
					action.compareTo = list.ToArray();
					fsmVariables.FindFsmInt("Trinket Num").Value = list.Count;
					List<FsmEvent> list2 = new List<FsmEvent>(action.sendEvent) { FsmEvent.FindEvent("PURE SEED") };
					action.sendEvent = list2.ToArray();
					FsmUtil.CopyState(obj2, "Love Key", "Necklace");
					FsmUtil.GetAction<SetPlayerDataBool>(obj2, "Necklace", 0).boolName = FsmString.op_Implicit("AdditionalMapsGotGhMap");
					FsmUtil.GetAction<SetSpriteRendererSprite>(obj2, "Necklace", 1).sprite = FsmObject.op_Implicit((Object)(object)GetSprite("MapIcon"));
					FsmUtil.GetAction<GetLanguageString>(obj2, "Necklace", 2).convName = FsmString.op_Implicit("SFGrenadeAdditionalMaps_GhMapName");
					FsmUtil.AddTransition(obj2, "Trinket Type", "PURE SEED", "Necklace");
					obj.SetActive(true);
					break;
				}
				case "GG_Workshop":
				{
					GameObject val2 = USceneUtil.Find(to, "_SceneManager");
					_setCompassPointRoomPrefab.CopyOnto<PlayMakerFSM>(val2);
					FSMUtility.LocateMyFSM(val2, "map_isroom").Preprocess();
					break;
				}
				case "GG_Blue_Room":
				{
					GameObject val = USceneUtil.Find(to, "_SceneManager");
					_setCompassPointRoomPrefab.CopyOnto<PlayMakerFSM>(val);
					FSMUtility.LocateMyFSM(val, "map_isroom").Preprocess();
					break;
				}
				}
			}
		}

		private static void MakeSpriteGo(string name, Sprite sprite, Vector3 pos, Vector3 angles)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			GameObject val = new GameObject(name);
			val.transform.position = pos;
			val.transform.eulerAngles = angles;
			SpriteRenderer obj = val.AddComponent<SpriteRenderer>();
			((Renderer)obj).material = new Material(Shader.Find("Sprites/Default"));
			obj.sprite = sprite;
		}

		private static void OnPlayMakerFSMStart(orig_Start orig, PlayMakerFSM self)
		{
			orig.Invoke(self);
			if (!self.FsmName.Equals("UI Control") || !((Object)((Component)self).gameObject).name.Equals("World Map"))
			{
				return;
			}
			ChangeWpMap(((Component)self).gameObject, ((Component)((Component)self).transform.GetChild(4)).gameObject);
			ChangeGhMap(((Component)self).gameObject, ((Component)((Component)self).transform.GetChild(4)).gameObject);
			if (PlayerData.instance.GetBool("whitePalaceMidWarp"))
			{
				if (!PlayerData.instance.GetVariable<List<string>>("scenesVisited").Contains("White_Palace_11"))
				{
					PlayerData.instance.GetVariable<List<string>>("scenesVisited").Add("White_Palace_11");
				}
				if (!PlayerData.instance.GetVariable<List<string>>("scenesVisited").Contains("White_Palace_01"))
				{
					PlayerData.instance.GetVariable<List<string>>("scenesVisited").Add("White_Palace_01");
				}
				if (!PlayerData.instance.GetVariable<List<string>>("scenesVisited").Contains("White_Palace_02"))
				{
					PlayerData.instance.GetVariable<List<string>>("scenesVisited").Add("White_Palace_02");
				}
				if (!PlayerData.instance.GetVariable<List<string>>("scenesVisited").Contains("White_Palace_03_hub"))
				{
					PlayerData.instance.GetVariable<List<string>>("scenesVisited").Add("White_Palace_03_hub");
				}
			}
			if (PlayerData.instance.GetBool("whitePalaceOrb_2"))
			{
				if (!PlayerData.instance.GetVariable<List<string>>("scenesVisited").Contains("White_Palace_15"))
				{
					PlayerData.instance.GetVariable<List<string>>("scenesVisited").Add("White_Palace_15");
				}
				if (!PlayerData.instance.GetVariable<List<string>>("scenesVisited").Contains("White_Palace_05"))
				{
					PlayerData.instance.GetVariable<List<string>>("scenesVisited").Add("White_Palace_05");
				}
				if (!PlayerData.instance.GetVariable<List<string>>("scenesVisited").Contains("White_Palace_16"))
				{
					PlayerData.instance.GetVariable<List<string>>("scenesVisited").Add("White_Palace_16");
				}
			}
			if (PlayerData.instance.GetBool("whitePalaceOrb_3"))
			{
				if (!PlayerData.instance.GetVariable<List<string>>("scenesVisited").Contains("White_Palace_04"))
				{
					PlayerData.instance.GetVariable<List<string>>("scenesVisited").Add("White_Palace_04");
				}
				if (!PlayerData.instance.GetVariable<List<string>>("scenesVisited").Contains("White_Palace_14"))
				{
					PlayerData.instance.GetVariable<List<string>>("scenesVisited").Add("White_Palace_14");
				}
			}
			if (PlayerData.instance.GetBool("gotKingFragment"))
			{
				if (!PlayerData.instance.GetVariable<List<string>>("scenesVisited").Contains("White_Palace_06"))
				{
					PlayerData.instance.GetVariable<List<string>>("scenesVisited").Add("White_Palace_06");
				}
				if (!PlayerData.instance.GetVariable<List<string>>("scenesVisited").Contains("White_Palace_07"))
				{
					PlayerData.instance.GetVariable<List<string>>("scenesVisited").Add("White_Palace_07");
				}
				if (!PlayerData.instance.GetVariable<List<string>>("scenesVisited").Contains("White_Palace_12"))
				{
					PlayerData.instance.GetVariable<List<string>>("scenesVisited").Add("White_Palace_12");
				}
				if (!PlayerData.instance.GetVariable<List<string>>("scenesVisited").Contains("White_Palace_13"))
				{
					PlayerData.instance.GetVariable<List<string>>("scenesVisited").Add("White_Palace_13");
				}
				if (!PlayerData.instance.GetVariable<List<string>>("scenesVisited").Contains("White_Palace_09"))
				{
					PlayerData.instance.GetVariable<List<string>>("scenesVisited").Add("White_Palace_09");
				}
			}
			if (PlayerData.instance.GetBool("killedBindingSeal"))
			{
				if (!PlayerData.instance.GetVariable<List<string>>("scenesVisited").Contains("White_Palace_18"))
				{
					PlayerData.instance.GetVariable<List<string>>("scenesVisited").Add("White_Palace_18");
				}
				if (!PlayerData.instance.GetVariable<List<string>>("scenesVisited").Contains("White_Palace_17"))
				{
					PlayerData.instance.GetVariable<List<string>>("scenesVisited").Add("White_Palace_17");
				}
				if (!PlayerData.instance.GetVariable<List<string>>("scenesVisited").Contains("White_Palace_19"))
				{
					PlayerData.instance.GetVariable<List<string>>("scenesVisited").Add("White_Palace_19");
				}
				if (!PlayerData.instance.GetVariable<List<string>>("scenesVisited").Contains("White_Palace_20"))
				{
					PlayerData.instance.GetVariable<List<string>>("scenesVisited").Add("White_Palace_20");
				}
			}
		}

		private string OnLanguageGetHook(string key, string sheet, string orig)
		{
			if (!LangStrings.ContainsKey(key, sheet))
			{
				return orig;
			}
			return LangStrings.Get(key, sheet);
		}

		private static bool HasGetSettingsValue<T>(string target)
		{
			FieldInfo fieldInfo = ReflectionHelper.GetFieldInfo(typeof(AmSaveSettings), target, true);
			if (fieldInfo != null)
			{
				return fieldInfo.FieldType == typeof(T);
			}
			return false;
		}

		private T GetSettingsValue<T>(string target)
		{
			return ReflectionHelper.GetField<AmSaveSettings, T>(base.SaveSettings, target);
		}

		private void SetSettingsValue<T>(string target, T val)
		{
			ReflectionHelper.SetField<AmSaveSettings, T>(base.SaveSettings, target, val);
		}

		private bool OnGetPlayerBoolHook(string target, bool orig)
		{
			if (!HasGetSettingsValue<bool>(target))
			{
				return orig;
			}
			return GetSettingsValue<bool>(target);
		}

		private bool OnSetPlayerBoolHook(string target, bool orig)
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			if (!HasGetSettingsValue<bool>(target))
			{
				return orig;
			}
			SetSettingsValue(target, orig);
			GameManager.instance.UpdateGameMap();
			Object.FindObjectOfType<GameMap>().SetupMap(false);
			ObjectPoolExtensions.Spawn(((Component)Resources.FindObjectsOfTypeAll<Transform>().First((Transform t) => ((Object)((Component)t).gameObject).name.Equals("Map Update Msg"))).gameObject, Vector3.zero);
			return orig;
		}

		private static void ChangeWpMap(GameObject worldMap, GameObject wideMap)
		{
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Expected O, but got Unknown
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Expected O, but got Unknown
			//IL_02d9: Expected O, but got Unknown
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Expected O, but got Unknown
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Expected O, but got Unknown
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Expected O, but got Unknown
			DebugLog($"!ChangeWpMap: \"{wideMap}\"");
			Vector3 val = default(Vector3);
			((Vector3)(ref val))..ctor(2.07f, -20f, -22f);
			Vector3 localPosition = default(Vector3);
			((Vector3)(ref localPosition))..ctor(2.25f, -2.25f, -2.3f);
			bool activeSelf = wideMap.activeSelf;
			wideMap.SetActive(false);
			string text = customAreaNameWp + " State";
			string text2 = customAreaNameWp + " State Left";
			string text3 = customAreaNameWp + " State Right";
			string text4 = customAreaNameWp + " State Down";
			string text5 = customAreaNameWp + " State Zoom";
			string text6 = "T Up";
			string text7 = "Cl Up";
			GameObject val2 = Object.Instantiate<GameObject>(((Component)wideMap.transform.GetChild(0)).gameObject, wideMap.transform, true);
			val2.SetActive(false);
			((Object)val2).name = customAreaNameWp;
			val2.transform.localPosition = localPosition;
			val2.GetComponent<SpriteRenderer>().sprite = GetSprite("CustomArea.WP");
			val2.GetComponentInChildren<SetTextMeshProGameText>().convName = customAreaNameWp.ToUpper();
			Transform obj = val2.transform.Find("Area Name");
			obj.localPosition += new Vector3(-2.75f, 0.5f, 0f);
			FSMUtility.LocateMyFSM(val2, "deactivate").FsmVariables.GetFsmString("playerData bool").Value = boolNameWp;
			PlayMakerFSM val3 = FSMUtility.LocateMyFSM(worldMap, "UI Control");
			if (FsmUtil.GetState(val3, "Mines").Fsm == null)
			{
				val3.Preprocess();
			}
			FsmVariables fsmVariables = val3.FsmVariables;
			FsmUtil.CopyState(val3, "Mines", text);
			FsmUtil.CopyState(val3, "T Left", text2);
			FsmUtil.CopyState(val3, "T Right", text3);
			FsmUtil.CopyState(val3, "Mi Left", text4);
			FsmUtil.CopyState(val3, "To Zoom 10", text5);
			FsmUtil.CopyState(val3, "CR Up", text6);
			FsmUtil.CopyState(val3, "CR Up", text7);
			FsmUtil.AddGameObjectVariable(val3, customAreaNameWp);
			FindChild val4 = new FindChild
			{
				gameObject = FsmUtil.GetAction<FindChild>(val3, "Init", 10).gameObject,
				childName = FsmString.op_Implicit(customAreaNameWp),
				storeResult = fsmVariables.GetFsmGameObject(customAreaNameWp)
			};
			FsmUtil.InsertAction(val3, "Init", (FsmStateAction)(object)val4, 11);
			FsmEvent val5 = FsmUtil.AddGlobalTransition(val3, customGlobalEventWp, text);
			DebugLog("... Added Custom Global Transition...");
			FsmOwnerDefault gameObject = FsmUtil.GetAction<PlayerDataBoolTest>(val3, "D Up", 2).gameObject;
			DebugLog("... Create Custom States...");
			FsmUtil.GetAction<SendEventByName>(val3, text, 2).eventTarget = new FsmEventTarget
			{
				target = (EventTarget)1,
				gameObject = new FsmOwnerDefault
				{
					OwnerOption = (OwnerDefaultOption)1,
					GameObject = FsmGameObject.op_Implicit(val2)
				}
			};
			FsmUtil.GetAction<GetLanguageString>(val3, text, 3).convName = FsmString.op_Implicit("MAP_NAME_" + customAreaNameWp.ToUpper());
			FsmUtil.GetAction<SetStringValue>(val3, text, 5).stringValue = FsmString.op_Implicit(val5.Name);
			FsmUtil.GetAction<SetVector3Value>(val3, text, 6).vector3Value = FsmVector3.op_Implicit(val);
			FsmUtil.InsertAction(val3, text6, (FsmStateAction)new PlayerDataBoolTest
			{
				gameObject = gameObject,
				boolName = FsmString.op_Implicit(boolNameWp),
				isTrue = val5
			}, 0);
			FsmUtil.InsertAction(val3, text7, (FsmStateAction)new PlayerDataBoolTest
			{
				gameObject = gameObject,
				boolName = FsmString.op_Implicit(boolNameWp),
				isTrue = val5
			}, 0);
			FsmUtil.RemoveAction(val3, text7, 1);
			FsmEvent fsmEvent = FsmEvent.GetFsmEvent(customGlobalEventGh);
			FsmUtil.InsertAction(val3, text3, (FsmStateAction)new PlayerDataBoolTest
			{
				gameObject = gameObject,
				boolName = FsmString.op_Implicit(boolNameGh),
				isTrue = fsmEvent
			}, 0);
			FsmUtil.ChangeTransition(val3, text, "UI LEFT", text2);
			FsmUtil.ChangeTransition(val3, text, "UI RIGHT", text3);
			FsmUtil.ChangeTransition(val3, text, "UI DOWN", text4);
			FsmUtil.ChangeTransition(val3, text, "UI CONFIRM", text5);
			FsmUtil.ChangeTransition(val3, text2, "FINISHED", text);
			FsmUtil.ChangeTransition(val3, text3, "FINISHED", text);
			FsmUtil.ChangeTransition(val3, text4, "FINISHED", text);
			FsmUtil.AddTransition(val3, "Town", "UI UP", text6);
			FsmUtil.ChangeTransition(val3, "Town", "UI UP", text6);
			FsmUtil.AddTransition(val3, text6, "FINISHED", "Town");
			FsmUtil.ChangeTransition(val3, text6, "FINISHED", "Town");
			FsmUtil.AddTransition(val3, "Cliffs", "UI UP", text7);
			FsmUtil.ChangeTransition(val3, "Cliffs", "UI UP", text7);
			FsmUtil.AddTransition(val3, text7, "FINISHED", "Cliffs");
			FsmUtil.ChangeTransition(val3, text7, "FINISHED", "Cliffs");
			wideMap.SetActive(activeSelf);
			DebugLog("~ChangeWpMap");
		}

		private static void ChangeGhMap(GameObject worldMap, GameObject wideMap)
		{
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Expected O, but got Unknown
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Expected O, but got Unknown
			//IL_02c3: Expected O, but got Unknown
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Expected O, but got Unknown
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Expected O, but got Unknown
			DebugLog($"!ChangeGhMap: \"{wideMap}\"");
			Vector3 val = default(Vector3);
			((Vector3)(ref val))..ctor(-8.07f, -16f, -22f);
			Vector3 localPosition = default(Vector3);
			((Vector3)(ref localPosition))..ctor(4.75f, -2.25f, -2.3f);
			bool activeSelf = wideMap.activeSelf;
			wideMap.SetActive(false);
			string text = customAreaNameGh + " State";
			string text2 = customAreaNameGh + " State Left";
			string text3 = customAreaNameGh + " State Right";
			string text4 = customAreaNameGh + " State Down";
			string text5 = customAreaNameGh + " State Zoom";
			string text6 = "Mi Up";
			GameObject val2 = Object.Instantiate<GameObject>(((Component)wideMap.transform.GetChild(0)).gameObject, wideMap.transform, true);
			val2.SetActive(false);
			((Object)val2).name = customAreaNameGh;
			val2.transform.localPosition = localPosition;
			val2.GetComponent<SpriteRenderer>().sprite = GetSprite("CustomArea.GH");
			val2.GetComponentInChildren<SetTextMeshProGameText>().convName = customAreaNameGh.ToUpper();
			Transform obj = val2.transform.Find("Area Name");
			obj.localPosition += new Vector3(1.75f, 0.5f, 0f);
			FSMUtility.LocateMyFSM(val2, "deactivate").FsmVariables.GetFsmString("playerData bool").Value = boolNameGh;
			PlayMakerFSM val3 = FSMUtility.LocateMyFSM(worldMap, "UI Control");
			if (FsmUtil.GetState(val3, "Mines").Fsm == null)
			{
				val3.Preprocess();
			}
			FsmVariables fsmVariables = val3.FsmVariables;
			FsmUtil.CopyState(val3, "Mines", text);
			FsmUtil.CopyState(val3, "Mi Left", text2);
			FsmUtil.CopyState(val3, "Mi Right", text3);
			FsmUtil.CopyState(val3, "T Right", text4);
			FsmUtil.CopyState(val3, "To Zoom 11", text5);
			FsmUtil.CopyState(val3, "RG Up", text6);
			FsmUtil.AddGameObjectVariable(val3, customAreaNameGh);
			FindChild val4 = new FindChild
			{
				gameObject = FsmUtil.GetAction<FindChild>(val3, "Init", 10).gameObject,
				childName = FsmString.op_Implicit(customAreaNameGh),
				storeResult = fsmVariables.GetFsmGameObject(customAreaNameGh)
			};
			FsmUtil.InsertAction(val3, "Init", (FsmStateAction)(object)val4, 11);
			FsmEvent val5 = FsmUtil.AddGlobalTransition(val3, customGlobalEventGh, text);
			DebugLog("... Added Custom Global Transition...");
			FsmOwnerDefault gameObject = FsmUtil.GetAction<PlayerDataBoolTest>(val3, "D Up", 2).gameObject;
			DebugLog("... Create Custom States...");
			FsmUtil.GetAction<SendEventByName>(val3, text, 2).eventTarget = new FsmEventTarget
			{
				target = (EventTarget)1,
				gameObject = new FsmOwnerDefault
				{
					OwnerOption = (OwnerDefaultOption)1,
					GameObject = FsmGameObject.op_Implicit(val2)
				}
			};
			FsmUtil.GetAction<GetLanguageString>(val3, text, 3).convName = FsmString.op_Implicit("MAP_NAME_" + customAreaNameGh.ToUpper());
			FsmUtil.GetAction<SetStringValue>(val3, text, 5).stringValue = FsmString.op_Implicit(val5.Name);
			FsmUtil.GetAction<SetVector3Value>(val3, text, 6).vector3Value = FsmVector3.op_Implicit(val);
			FsmUtil.InsertAction(val3, text6, (FsmStateAction)new PlayerDataBoolTest
			{
				gameObject = gameObject,
				boolName = FsmString.op_Implicit(boolNameGh),
				isTrue = val5
			}, 0);
			FsmEvent fsmEvent = FsmEvent.GetFsmEvent(customGlobalEventWp);
			FsmUtil.InsertAction(val3, text2, (FsmStateAction)new PlayerDataBoolTest
			{
				gameObject = gameObject,
				boolName = FsmString.op_Implicit(boolNameWp),
				isTrue = fsmEvent
			}, 0);
			FsmUtil.ChangeTransition(val3, text, "UI LEFT", text2);
			FsmUtil.ChangeTransition(val3, text, "UI RIGHT", text3);
			FsmUtil.ChangeTransition(val3, text, "UI DOWN", text4);
			FsmUtil.ChangeTransition(val3, text, "UI CONFIRM", text5);
			FsmUtil.ChangeTransition(val3, text2, "FINISHED", text);
			FsmUtil.ChangeTransition(val3, text3, "FINISHED", text);
			FsmUtil.ChangeTransition(val3, text4, "FINISHED", text);
			FsmUtil.AddTransition(val3, "Mines", "UI UP", text6);
			FsmUtil.ChangeTransition(val3, "Mines", "UI UP", text6);
			FsmUtil.AddTransition(val3, text6, "FINISHED", "Mines");
			FsmUtil.ChangeTransition(val3, text6, "FINISHED", "Mines");
			wideMap.SetActive(activeSelf);
			DebugLog("~ChangeGhMap");
		}

		private static void DebugLog(string msg)
		{
			string text = "[AdditionalMaps] - " + msg;
			Logger.LogDebug(text);
			Debug.Log((object)text);
		}

		private static void SetInactive(GameObject go)
		{
			if (!((Object)(object)go == (Object)null))
			{
				Object.DontDestroyOnLoad((Object)(object)go);
				go.SetActive(false);
			}
		}
	}
	public class AmSaveSettings
	{
		public bool AdditionalMapsGotWpMap;

		public bool AdditionalMapsGotGhMap;
	}
	public class AmGlobalSettings
	{
	}
	public static class ComponentUtil
	{
		public static void CopyOnto<T>(this T comp, GameObject o) where T : Component
		{
			Type typeFromHandle = typeof(T);
			T val = o.AddComponent<T>();
			PropertyInfo[] properties = typeFromHandle.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.CanWrite)
				{
					try
					{
						propertyInfo.SetValue(val, propertyInfo.GetValue(comp, null), null);
					}
					catch
					{
					}
				}
			}
			FieldInfo[] fields = typeFromHandle.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				fieldInfo.SetValue(val, fieldInfo.GetValue(comp));
			}
		}
	}
}
namespace AdditionalMaps.MonoBehaviours
{
	public struct SCustomArea
	{
		public GameObject AreaGameObject;

		public Vector3 CameraPosition;

		public string PlayerDataBoolGotAreaMap;

		public List<string> MapZoneStrings;

		public float? panMinX;

		public float? panMaxX;

		public float? panMinY;

		public float? panMaxY;
	}
	public class ActionArg
	{
		public GameObject Go;

		public FsmString FsmString;
	}
	public static class GameMapHooks
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static hook_SetGameMap <0>__OnGameManagerSetGameMap;

			public static Manipulator <1>__IlWorldMap;

			public static hook_QuickMapAncientBasin <2>__NewQuickMapAncientBasin;

			public static hook_QuickMapCity <3>__NewQuickMapCity;

			public static hook_QuickMapCliffs <4>__NewQuickMapCliffs;

			public static hook_QuickMapCrossroads <5>__NewQuickMapCrossroads;

			public static hook_QuickMapCrystalPeak <6>__NewQuickMapCrystalPeak;

			public static hook_QuickMapDeepnest <7>__NewQuickMapDeepnest;

			public static hook_QuickMapFogCanyon <8>__NewQuickMapFogCanyon;

			public static hook_QuickMapFungalWastes <9>__NewQuickMapFungalWastes;

			public static hook_QuickMapGreenpath <10>__NewQuickMapGreenpath;

			public static hook_QuickMapKingdomsEdge <11>__NewQuickMapKingdomsEdge;

			public static hook_QuickMapQueensGardens <12>__NewQuickMapQueensGardens;

			public static hook_QuickMapRestingGrounds <13>__NewQuickMapRestingGrounds;

			public static hook_QuickMapDirtmouth <14>__NewQuickMapDirtmouth;

			public static hook_QuickMapWaterways <15>__NewQuickMapWaterways;

			public static hook_CloseQuickMap <16>__NewCloseQuickMap;

			public static Manipulator <17>__IlPositionCompass;

			public static Action<ActionArg> <18>__QuickMapNew;
		}

		private static readonly Dictionary<string, SCustomArea> CustomAreas = new Dictionary<string, SCustomArea>();

		private static readonly List<Func<GameMap, Dictionary<string, SCustomArea>>> Callbacks = new List<Func<GameMap, Dictionary<string, SCustomArea>>>();

		public static void Init(Func<GameMap, Dictionary<string, SCustomArea>> callback = null)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Expected O, but got Unknown
			try
			{
				object obj = <>O.<0>__OnGameManagerSetGameMap;
				if (obj == null)
				{
					hook_SetGameMap val = OnGameManagerSetGameMap;
					<>O.<0>__OnGameManagerSetGameMap = val;
					obj = (object)val;
				}
				GameManager.SetGameMap -= (hook_SetGameMap)obj;
			}
			catch
			{
			}
			finally
			{
				object obj3 = <>O.<0>__OnGameManagerSetGameMap;
				if (obj3 == null)
				{
					hook_SetGameMap val2 = OnGameManagerSetGameMap;
					<>O.<0>__OnGameManagerSetGameMap = val2;
					obj3 = (object)val2;
				}
				GameManager.SetGameMap += (hook_SetGameMap)obj3;
			}
			if (callback != null)
			{
				Callbacks.Add(callback);
			}
		}

		public static void Unhook()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Expected O, but got Unknown
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Expected O, but got Unknown
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Expected O, but got Unknown
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Expected O, but got Unknown
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Expected O, but got Unknown
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Expected O, but got Unknown
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Expected O, but got Unknown
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Expected O, but got Unknown
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Expected O, but got Unknown
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Expected O, but got Unknown
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Expected O, but got Unknown
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Expected O, but got Unknown
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Expected O, but got Unknown
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Expected O, but got Unknown
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Expected O, but got Unknown
			object obj = <>O.<0>__OnGameManagerSetGameMap;
			if (obj == null)
			{
				hook_SetGameMap val = OnGameManagerSetGameMap;
				<>O.<0>__OnGameManagerSetGameMap = val;
				obj = (object)val;
			}
			GameManager.SetGameMap -= (hook_SetGameMap)obj;
			object obj2 = <>O.<1>__IlWorldMap;
			if (obj2 == null)
			{
				Manipulator val2 = IlWorldMap;
				<>O.<1>__IlWorldMap = val2;
				obj2 = (object)val2;
			}
			GameMap.WorldMap -= (Manipulator)obj2;
			object obj3 = <>O.<2>__NewQuickMapAncientBasin;
			if (obj3 == null)
			{
				hook_QuickMapAncientBasin val3 = NewQuickMapAncientBasin;
				<>O.<2>__NewQuickMapAncientBasin = val3;
				obj3 = (object)val3;
			}
			GameMap.QuickMapAncientBasin -= (hook_QuickMapAncientBasin)obj3;
			object obj4 = <>O.<3>__NewQuickMapCity;
			if (obj4 == null)
			{
				hook_QuickMapCity val4 = NewQuickMapCity;
				<>O.<3>__NewQuickMapCity = val4;
				obj4 = (object)val4;
			}
			GameMap.QuickMapCity -= (hook_QuickMapCity)obj4;
			object obj5 = <>O.<4>__NewQuickMapCliffs;
			if (obj5 == null)
			{
				hook_QuickMapCliffs val5 = NewQuickMapCliffs;
				<>O.<4>__NewQuickMapCliffs = val5;
				obj5 = (object)val5;
			}
			GameMap.QuickMapCliffs -= (hook_QuickMapCliffs)obj5;
			object obj6 = <>O.<5>__NewQuickMapCrossroads;
			if (obj6 == null)
			{
				hook_QuickMapCrossroads val6 = NewQuickMapCrossroads;
				<>O.<5>__NewQuickMapCrossroads = val6;
				obj6 = (object)val6;
			}
			GameMap.QuickMapCrossroads -= (hook_QuickMapCrossroads)obj6;
			object obj7 = <>O.<6>__NewQuickMapCrystalPeak;
			if (obj7 == null)
			{
				hook_QuickMapCrystalPeak val7 = NewQuickMapCrystalPeak;
				<>O.<6>__NewQuickMapCrystalPeak = val7;
				obj7 = (object)val7;
			}
			GameMap.QuickMapCrystalPeak -= (hook_QuickMapCrystalPeak)obj7;
			object obj8 = <>O.<7>__NewQuickMapDeepnest;
			if (obj8 == null)
			{
				hook_QuickMapDeepnest val8 = NewQuickMapDeepnest;
				<>O.<7>__NewQuickMapDeepnest = val8;
				obj8 = (object)val8;
			}
			GameMap.QuickMapDeepnest -= (hook_QuickMapDeepnest)obj8;
			object obj9 = <>O.<8>__NewQuickMapFogCanyon;
			if (obj9 == null)
			{
				hook_QuickMapFogCanyon val9 = NewQuickMapFogCanyon;
				<>O.<8>__NewQuickMapFogCanyon = val9;
				obj9 = (object)val9;
			}
			GameMap.QuickMapFogCanyon -= (hook_QuickMapFogCanyon)obj9;
			object obj10 = <>O.<9>__NewQuickMapFungalWastes;
			if (obj10 == null)
			{
				hook_QuickMapFungalWastes val10 = NewQuickMapFungalWastes;
				<>O.<9>__NewQuickMapFungalWastes = val10;
				obj10 = (object)val10;
			}
			GameMap.QuickMapFungalWastes -= (hook_QuickMapFungalWastes)obj10;
			object obj11 = <>O.<10>__NewQuickMapGreenpath;
			if (obj11 == null)
			{
				hook_QuickMapGreenpath val11 = NewQuickMapGreenpath;
				<>O.<10>__NewQuickMapGreenpath = val11;
				obj11 = (object)val11;
			}
			GameMap.QuickMapGreenpath -= (hook_QuickMapGreenpath)obj11;
			object obj12 = <>O.<11>__NewQuickMapKingdomsEdge;
			if (obj12 == null)
			{
				hook_QuickMapKingdomsEdge val12 = NewQuickMapKingdomsEdge;
				<>O.<11>__NewQuickMapKingdomsEdge = val12;
				obj12 = (object)val12;
			}
			GameMap.QuickMapKingdomsEdge -= (hook_QuickMapKingdomsEdge)obj12;
			object obj13 = <>O.<12>__NewQuickMapQueensGardens;
			if (obj13 == null)
			{
				hook_QuickMapQueensGardens val13 = NewQuickMapQueensGardens;
				<>O.<12>__NewQuickMapQueensGardens = val13;
				obj13 = (object)val13;
			}
			GameMap.QuickMapQueensGardens -= (hook_QuickMapQueensGardens)obj13;
			object obj14 = <>O.<13>__NewQuickMapRestingGrounds;
			if (obj14 == null)
			{
				hook_QuickMapRestingGrounds val14 = NewQuickMapRestingGrounds;
				<>O.<13>__NewQuickMapRestingGrounds = val14;
				obj14 = (object)val14;
			}
			GameMap.QuickMapRestingGrounds -= (hook_QuickMapRestingGrounds)obj14;
			object obj15 = <>O.<14>__NewQuickMapDirtmouth;
			if (obj15 == null)
			{
				hook_QuickMapDirtmouth val15 = NewQuickMapDirtmouth;
				<>O.<14>__NewQuickMapDirtmouth = val15;
				obj15 = (object)val15;
			}
			GameMap.QuickMapDirtmouth -= (hook_QuickMapDirtmouth)obj15;
			object obj16 = <>O.<15>__NewQuickMapWaterways;
			if (obj16 == null)
			{
				hook_QuickMapWaterways val16 = NewQuickMapWaterways;
				<>O.<15>__NewQuickMapWaterways = val16;
				obj16 = (object)val16;
			}
			GameMap.QuickMapWaterways -= (hook_QuickMapWaterways)obj16;
			object obj17 = <>O.<16>__NewCloseQuickMap;
			if (obj17 == null)
			{
				hook_CloseQuickMap val17 = NewCloseQuickMap;
				<>O.<16>__NewCloseQuickMap = val17;
				obj17 = (object)val17;
			}
			GameMap.CloseQuickMap -= (hook_CloseQuickMap)obj17;
			object obj18 = <>O.<17>__IlPositionCompass;
			if (obj18 == null)
			{
				Manipulator val18 = IlPositionCompass;
				<>O.<17>__IlPositionCompass = val18;
				obj18 = (object)val18;
			}
			GameMap.PositionCompass -= (Manipulator)obj18;
		}

		private static void MakeHooks()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Expected O, but got Unknown
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Expected O, but got Unknown
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Expected O, but got Unknown
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Expected O, but got Unknown
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Expected O, but got Unknown
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Expected O, but got Unknown
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Expected O, but got Unknown
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Expected O, but got Unknown
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Expected O, but got Unknown
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Expected O, but got Unknown
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Expected O, but got Unknown
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Expected O, but got Unknown
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Expected O, but got Unknown
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Expected O, but got Unknown
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Expected O, but got Unknown
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Expected O, but got Unknown
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b0: Expected O, but got Unknown
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Expected O, but got Unknown
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Expected O, but got Unknown
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Expected O, but got Unknown
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Expected O, but got Unknown
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Expected O, but got Unknown
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Expected O, but got Unknown
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Expected O, but got Unknown
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Expected O, but got Unknown
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Expected O, but got Unknown
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Expected O, but got Unknown
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Expected O, but got Unknown
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Expected O, but got Unknown
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Expected O, but got Unknown
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Expected O, but got Unknown
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Expected O, but got Unknown
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Expected O, but got Unknown
			try
			{
				object obj = <>O.<1>__IlWorldMap;
				if (obj == null)
				{
					Manipulator val = IlWorldMap;
					<>O.<1>__IlWorldMap = val;
					obj = (object)val;
				}
				GameMap.WorldMap -= (Manipulator)obj;
			}
			catch
			{
			}
			finally
			{
				object obj3 = <>O.<1>__IlWorldMap;
				if (obj3 == null)
				{
					Manipulator val2 = IlWorldMap;
					<>O.<1>__IlWorldMap = val2;
					obj3 = (object)val2;
				}
				GameMap.WorldMap += (Manipulator)obj3;
			}
			try
			{
				object obj4 = <>O.<2>__NewQuickMapAncientBasin;
				if (obj4 == null)
				{
					hook_QuickMapAncientBasin val3 = NewQuickMapAncientBasin;
					<>O.<2>__NewQuickMapAncientBasin = val3;
					obj4 = (object)val3;
				}
				GameMap.QuickMapAncientBasin -= (hook_QuickMapAncientBasin)obj4;
			}
			catch
			{
			}
			finally
			{
				object obj6 = <>O.<2>__NewQuickMapAncientBasin;
				if (obj6 == null)
				{
					hook_QuickMapAncientBasin val4 = NewQuickMapAncientBasin;
					<>O.<2>__NewQuickMapAncientBasin = val4;
					obj6 = (object)val4;
				}
				GameMap.QuickMapAncientBasin += (hook_QuickMapAncientBasin)obj6;
			}
			try
			{
				object obj7 = <>O.<3>__NewQuickMapCity;
				if (obj7 == null)
				{
					hook_QuickMapCity val5 = NewQuickMapCity;
					<>O.<3>__NewQuickMapCity = val5;
					obj7 = (object)val5;
				}
				GameMap.QuickMapCity -= (hook_QuickMapCity)obj7;
			}
			catch
			{
			}
			finally
			{
				object obj9 = <>O.<3>__NewQuickMapCity;
				if (obj9 == null)
				{
					hook_QuickMapCity val6 = NewQuickMapCity;
					<>O.<3>__NewQuickMapCity = val6;
					obj9 = (object)val6;
				}
				GameMap.QuickMapCity += (hook_QuickMapCity)obj9;
			}
			try
			{
				object obj10 = <>O.<4>__NewQuickMapCliffs;
				if (obj10 == null)
				{
					hook_QuickMapCliffs val7 = NewQuickMapCliffs;
					<>O.<4>__NewQuickMapCliffs = val7;
					obj10 = (object)val7;
				}
				GameMap.QuickMapCliffs -= (hook_QuickMapCliffs)obj10;
			}
			catch
			{
			}
			finally
			{
				object obj12 = <>O.<4>__NewQuickMapCliffs;
				if (obj12 == null)
				{
					hook_QuickMapCliffs val8 = NewQuickMapCliffs;
					<>O.<4>__NewQuickMapCliffs = val8;
					obj12 = (object)val8;
				}
				GameMap.QuickMapCliffs += (hook_QuickMapCliffs)obj12;
			}
			try
			{
				object obj13 = <>O.<5>__NewQuickMapCrossroads;
				if (obj13 == null)
				{
					hook_QuickMapCrossroads val9 = NewQuickMapCrossroads;
					<>O.<5>__NewQuickMapCrossroads = val9;
					obj13 = (object)val9;
				}
				GameMap.QuickMapCrossroads -= (hook_QuickMapCrossroads)obj13;
			}
			catch
			{
			}
			finally
			{
				object obj15 = <>O.<5>__NewQuickMapCrossroads;
				if (obj15 == null)
				{
					hook_QuickMapCrossroads val10 = NewQuickMapCrossroads;
					<>O.<5>__NewQuickMapCrossroads = val10;
					obj15 = (object)val10;
				}
				GameMap.QuickMapCrossroads += (hook_QuickMapCrossroads)obj15;
			}
			try
			{
				object obj16 = <>O.<6>__NewQuickMapCrystalPeak;
				if (obj16 == null)
				{
					hook_QuickMapCrystalPeak val11 = NewQuickMapCrystalPeak;
					<>O.<6>__NewQuickMapCrystalPeak = val11;
					obj16 = (object)val11;
				}
				GameMap.QuickMapCrystalPeak -= (hook_QuickMapCrystalPeak)obj16;
			}
			catch
			{
			}
			finally
			{
				object obj18 = <>O.<6>__NewQuickMapCrystalPeak;
				if (obj18 == null)
				{
					hook_QuickMapCrystalPeak val12 = NewQuickMapCrystalPeak;
					<>O.<6>__NewQuickMapCrystalPeak = val12;
					obj18 = (object)val12;
				}
				GameMap.QuickMapCrystalPeak += (hook_QuickMapCrystalPeak)obj18;
			}
			try
			{
				object obj19 = <>O.<7>__NewQuickMapDeepnest;
				if (obj19 == null)
				{
					hook_QuickMapDeepnest val13 = NewQuickMapDeepnest;
					<>O.<7>__NewQuickMapDeepnest = val13;
					obj19 = (object)val13;
				}
				GameMap.QuickMapDeepnest -= (hook_QuickMapDeepnest)obj19;
			}
			catch
			{
			}
			finally
			{
				object obj21 = <>O.<7>__NewQuickMapDeepnest;
				if (obj21 == null)
				{
					hook_QuickMapDeepnest val14 = NewQuickMapDeepnest;
					<>O.<7>__NewQuickMapDeepnest = val14;
					obj21 = (object)val14;
				}
				GameMap.QuickMapDeepnest += (hook_QuickMapDeepnest)obj21;
			}
			try
			{
				object obj22 = <>O.<8>__NewQuickMapFogCanyon;
				if (obj22 == null)
				{
					hook_QuickMapFogCanyon val15 = NewQuickMapFogCanyon;
					<>O.<8>__NewQuickMapFogCanyon = val15;
					obj22 = (object)val15;
				}
				GameMap.QuickMapFogCanyon -= (hook_QuickMapFogCanyon)obj22;
			}
			catch
			{
			}
			finally
			{
				object obj24 = <>O.<8>__NewQuickMapFogCanyon;
				if (obj24 == null)
				{
					hook_QuickMapFogCanyon val16 = NewQuickMapFogCanyon;
					<>O.<8>__NewQuickMapFogCanyon = val16;
					obj24 = (object)val16;
				}
				GameMap.QuickMapFogCanyon += (hook_QuickMapFogCanyon)obj24;
			}
			try
			{
				object obj25 = <>O.<9>__NewQuickMapFungalWastes;
				if (obj25 == null)
				{
					hook_QuickMapFungalWastes val17 = NewQuickMapFungalWastes;
					<>O.<9>__NewQuickMapFungalWastes = val17;
					obj25 = (object)val17;
				}
				GameMap.QuickMapFungalWastes -= (hook_QuickMapFungalWastes)obj25;
			}
			catch
			{
			}
			finally
			{
				object obj27 = <>O.<9>__NewQuickMapFungalWastes;
				if (obj27 == null)
				{
					hook_QuickMapFungalWastes val18 = NewQuickMapFungalWastes;
					<>O.<9>__NewQuickMapFungalWastes = val18;
					obj27 = (object)val18;
				}
				GameMap.QuickMapFungalWastes += (hook_QuickMapFungalWastes)obj27;
			}
			try
			{
				object obj28 = <>O.<10>__NewQuickMapGreenpath;
				if (obj28 == null)
				{
					hook_QuickMapGreenpath val19 = NewQuickMapGreenpath;
					<>O.<10>__NewQuickMapGreenpath = val19;
					obj28 = (object)val19;
				}
				GameMap.QuickMapGreenpath -= (hook_QuickMapGreenpath)obj28;
			}
			catch
			{
			}
			finally
			{
				object obj30 = <>O.<10>__NewQuickMapGreenpath;
				if (obj30 == null)
				{
					hook_QuickMapGreenpath val20 = NewQuickMapGreenpath;
					<>O.<10>__NewQuickMapGreenpath = val20;
					obj30 = (object)val20;
				}
				GameMap.QuickMapGreenpath += (hook_QuickMapGreenpath)obj30;
			}
			try
			{
				object obj31 = <>O.<11>__NewQuickMapKingdomsEdge;
				if (obj31 == null)
				{
					hook_QuickMapKingdomsEdge val21 = NewQuickMapKingdomsEdge;
					<>O.<11>__NewQuickMapKingdomsEdge = val21;
					obj31 = (object)val21;
				}
				GameMap.QuickMapKingdomsEdge -= (hook_QuickMapKingdomsEdge)obj31;
			}
			catch
			{
			}
			finally
			{
				object obj33 = <>O.<11>__NewQuickMapKingdomsEdge;
				if (obj33 == null)
				{
					hook_QuickMapKingdomsEdge val22 = NewQuickMapKingdomsEdge;
					<>O.<11>__NewQuickMapKingdomsEdge = val22;
					obj33 = (object)val22;
				}
				GameMap.QuickMapKingdomsEdge += (hook_QuickMapKingdomsEdge)obj33;
			}
			try
			{
				object obj34 = <>O.<12>__NewQuickMapQueensGardens;
				if (obj34 == null)
				{
					hook_QuickMapQueensGardens val23 = NewQuickMapQueensGardens;
					<>O.<12>__NewQuickMapQueensGardens = val23;
					obj34 = (object)val23;
				}
				GameMap.QuickMapQueensGardens -= (hook_QuickMapQueensGardens)obj34;
			}
			catch
			{
			}
			finally
			{
				object obj36 = <>O.<12>__NewQuickMapQueensGardens;
				if (obj36 == null)
				{
					hook_QuickMapQueensGardens val24 = NewQuickMapQueensGardens;
					<>O.<12>__NewQuickMapQueensGardens = val24;
					obj36 = (object)val24;
				}
				GameMap.QuickMapQueensGardens += (hook_QuickMapQueensGardens)obj36;
			}
			try
			{
				object obj37 = <>O.<13>__NewQuickMapRestingGrounds;
				if (obj37 == null)
				{
					hook_QuickMapRestingGrounds val25 = NewQuickMapRestingGrounds;
					<>O.<13>__NewQuickMapRestingGrounds = val25;
					obj37 = (object)val25;
				}
				GameMap.QuickMapRestingGrounds -= (hook_QuickMapRestingGrounds)obj37;
			}
			catch
			{
			}
			finally
			{
				object obj39 = <>O.<13>__NewQuickMapRestingGrounds;
				if (obj39 == null)
				{
					hook_QuickMapRestingGrounds val26 = NewQuickMapRestingGrounds;
					<>O.<13>__NewQuickMapRestingGrounds = val26;
					obj39 = (object)val26;
				}
				GameMap.QuickMapRestingGrounds += (hook_QuickMapRestingGrounds)obj39;
			}
			try
			{
				object obj40 = <>O.<14>__NewQuickMapDirtmouth;
				if (obj40 == null)
				{
					hook_QuickMapDirtmouth val27 = NewQuickMapDirtmouth;
					<>O.<14>__NewQuickMapDirtmouth = val27;
					obj40 = (object)val27;
				}
				GameMap.QuickMapDirtmouth -= (hook_QuickMapDirtmouth)obj40;
			}
			catch
			{
			}
			finally
			{
				object obj42 = <>O.<14>__NewQuickMapDirtmouth;
				if (obj42 == null)
				{
					hook_QuickMapDirtmouth val28 = NewQuickMapDirtmouth;
					<>O.<14>__NewQuickMapDirtmouth = val28;
					obj42 = (object)val28;
				}
				GameMap.QuickMapDirtmouth += (hook_QuickMapDirtmouth)obj42;
			}
			try
			{
				object obj43 = <>O.<15>__NewQuickMapWaterways;
				if (obj43 == null)
				{
					hook_QuickMapWaterways val29 = NewQuickMapWaterways;
					<>O.<15>__NewQuickMapWaterways = val29;
					obj43 = (object)val29;
				}
				GameMap.QuickMapWaterways -= (hook_QuickMapWaterways)obj43;
			}
			catch
			{
			}
			finally
			{
				object obj45 = <>O.<15>__NewQuickMapWaterways;
				if (obj45 == null)
				{
					hook_QuickMapWaterways val30 = NewQuickMapWaterways;
					<>O.<15>__NewQuickMapWaterways = val30;
					obj45 = (object)val30;
				}
				GameMap.QuickMapWaterways += (hook_QuickMapWaterways)obj45;
			}
			try
			{
				object obj46 = <>O.<16>__NewCloseQuickMap;
				if (obj46 == null)
				{
					hook_CloseQuickMap val31 = NewCloseQuickMap;
					<>O.<16>__NewCloseQuickMap = val31;
					obj46 = (object)val31;
				}
				GameMap.CloseQuickMap -= (hook_CloseQuickMap)obj46;
			}
			catch
			{
			}
			finally
			{
				object obj48 = <>O.<16>__NewCloseQuickMap;
				if (obj48 == null)
				{
					hook_CloseQuickMap val32 = NewCloseQuickMap;
					<>O.<16>__NewCloseQuickMap = val32;
					obj48 = (object)val32;
				}
				GameMap.CloseQuickMap += (hook_CloseQuickMap)obj48;
			}
			try
			{
				object obj49 = <>O.<17>__IlPositionCompass;
				if (obj49 == null)
				{
					Manipulator val33 = IlPositionCompass;
					<>O.<17>__IlPositionCompass = val33;
					obj49 = (object)val33;
				}
				GameMap.PositionCompass -= (Manipulator)obj49;
			}
			catch
			{
			}
			finally
			{
				object obj51 = <>O.<17>__IlPositionCompass;
				if (obj51 == null)
				{
					Manipulator val34 = IlPositionCompass;
					<>O.<17>__IlPositionCompass = val34;
					obj51 = (object)val34;
				}
				GameMap.PositionCompass += (Manipulator)obj51;
			}
		}

		private static void OnGameManagerSetGameMap(orig_SetGameMap orig, GameManager self, GameObject goGameMap)
		{
			ReplaceComponent(goGameMap);
			orig.Invoke(self, goGameMap);
		}

		public static void ReplaceComponent(GameObject gameMapGameObject)
		{
			Log("!ReplaceComponent");
			if ((Object)(object)gameMapGameObject == (Object)null)
			{
				Log("~ReplaceComponent");
				return;
			}
			Log("Got " + Callbacks.Count + " callbacks!");
			string text = default(string);
			SCustomArea sCustomArea = default(SCustomArea);
			foreach (Func<GameMap, Dictionary<string, SCustomArea>> callback in Callbacks)
			{
				Object.DontDestroyOnLoad((Object)(object)gameMapGameObject);
				Dictionary<string, SCustomArea> dictionary = callback(gameMapGameObject.GetComponent<GameMap>());
				Log("Got " + dictionary.Count + " custom Areas!");
				foreach (KeyValuePair<string, SCustomArea> item in dictionary)
				{
					item.Deconstruct(ref text, ref sCustomArea);
					string text2 = text;
					SCustomArea value = sCustomArea;
					Log("Name: " + text2);
					CustomAreas[text2] = value;
					Object.DontDestroyOnLoad((Object)(object)value.AreaGameObject);
					value.AreaGameObject.transform.SetParent(gameMapGameObject.transform);
				}
			}
			ReplaceFsmQuickMap(gameMapGameObject);
			MakeHooks();
			Log("~ReplaceComponent");
		}

		public static void ReplaceFsmQuickMap(GameObject gameMapGameObject)
		{
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			Log("!ReplaceFsmQuickMap");
			foreach (KeyValuePair<string, SCustomArea> customArea in CustomAreas)
			{
				customArea.Value.AreaGameObject.transform.SetParent(gameMapGameObject.transform);
			}
			PlayMakerFSM val = FSMUtility.LocateMyFSM(GameObject.Find("Quick Map"), "Quick Map");
			FsmVariables fsmVariables = val.FsmVariables;
			foreach (KeyValuePair<string, SCustomArea> customArea2 in CustomAreas)
			{
				Log("Adding state " + customArea2.Key);
				FsmUtil.CopyState(val, "Cliffs", customArea2.Key);
				FsmUtil.GetAction<PlayerDataBoolTest>(val, customArea2.Key, 0).boolName = FsmString.op_Implicit(customArea2.Value.PlayerDataBoolGotAreaMap);
				FsmUtil.GetAction<GetLanguageString>(val, customArea2.Key, 1).convName = FsmString.op_Implicit(customArea2.Key);
				FsmUtil.GetAction<SetPosition>(val, customArea2.Key, 5).x = FsmFloat.op_Implicit(customArea2.Value.CameraPosition.x);
				FsmUtil.GetAction<SetPosition>(val, customArea2.Key, 5).y = FsmFloat.op_Implicit(customArea2.Value.CameraPosition.y);
				FsmUtil.GetAction<SetPosition>(val, customArea2.Key, 5).z = FsmFloat.op_Implicit(customArea2.Value.CameraPosition.z);
				FunctionAction<ActionArg> val2 = new FunctionAction<ActionArg>
				{
					arg = new ActionArg
					{
						Go = gameMapGameObject,
						FsmString = fsmVariables.FindFsmString("Map Zone")
					},
					action = QuickMapNew
				};
				FsmUtil.InsertAction(val, customArea2.Key, (FsmStateAction)(object)val2, 5);
				FsmUtil.RemoveAction(val, customArea2.Key, 4);
				foreach (string mapZoneString in customArea2.Value.MapZoneStrings)
				{
					FsmUtil.AddTransition(val, "Check Area", mapZoneString, customArea2.Key);
				}
				FsmUtil.AddTransition(val, customArea2.Key, "CLOSE QUICK MAP", "Check State");
				FsmUtil.ChangeTransition(val, customArea2.Key, "CLOSE QUICK MAP", "Check State");
			}
			Log("~ReplaceFsmQuickMap");
		}

		public static void QuickMapNew(ActionArg arg)
		{
			QuickMapNew(arg.Go.GetComponent<GameMap>(), arg.FsmString.Value);
		}

		public static void QuickMapNew(GameObject gameMapGo, string area)
		{
			QuickMapNew(gameMapGo.GetComponent<GameMap>(), area);
		}

		public static void QuickMapNew(GameMap self, string area)
		{
			Log("!QuickMapNew: \"" + area + "\"");
			if (area == "")
			{
				area = GameManager.instance.GetCurrentMapZone();
				Log("Area: \"" + area + "\"");
			}
			string text = default(string);
			SCustomArea sCustomArea = default(SCustomArea);
			foreach (KeyValuePair<string, SCustomArea> item in CustomAreas.Where((KeyValuePair<string, SCustomArea> pair) => (Object)(object)pair.Value.AreaGameObject != (Object)null))
			{
				item.Deconstruct(ref text, ref sCustomArea);
				SCustomArea sCustomArea2 = sCustomArea;
				sCustomArea2.AreaGameObject.SetActive(sCustomArea2.MapZoneStrings.Contains(area));
			}
			self.PositionCompass(false);
			Log("~QuickMapNew");
		}

		private static void Log(string message)
		{
			string text = "[AdditionalMaps][MonoBehaviours][GameMapHooks] - " + message;
			Logger.LogDebug(text);
			Debug.Log((object)text);
		}

		private static void IlWorldMap(ILContext il)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_051b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0533: Unknown result type (might be due to invalid IL or missing references)
			//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0574: Unknown result type (might be due to invalid IL or missing references)
			//IL_0672: Unknown result type (might be due to invalid IL or missing references)
			//IL_067e: Unknown result type (might be due to invalid IL or missing references)
			//IL_068a: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_063c: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_073a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0746: Unknown result type (might be due to invalid IL or missing references)
			//IL_0752: Unknown result type (might be due to invalid IL or missing references)
			//IL_0773: Unknown result type (might be due to invalid IL or missing references)
			//IL_078b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0704: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cc: Unknown result type (might be due to invalid IL or missing references)
			ILCursor val = new ILCursor(il);
			val.Goto(0, (MoveType)0, false);
			val.GotoNext((MoveType)0, new Func<Instruction, bool>[2]
			{
				(Instruction x) => ILPatternMatchingExt.MatchLdarg(x, 0),
				(Instruction x) => ILPatternMatchingExt.MatchLdfld<GameMap>(x, "flamePins")
			});
			List<ILLabel> list = new List<ILLabel>();
			Enumerator<Instruction> enumerator = il.Instrs.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Instruction current = enumerator.Current;
					object operand = current.Operand;
					ILLabel val2 = (ILLabel)((operand is ILLabel) ? operand : null);
					if (val2 != null && current.Offset > 0 && current.Offset <= val.Prev.Offset && val2.Target.Offset > 0 && val2.Target.Offset >= val.Next.Offset && !list.Contains(val2))
					{
						Log($"Label from {current.Offset} to {val2.Target.Offset} while being before {val.Next.Offset} has to be adjusted");
						list.Add(val2);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
			ILLabel val3 = null;
			string text = default(string);
			SCustomArea sCustomArea = default(SCustomArea);
			foreach (KeyValuePair<string, SCustomArea> customArea in CustomAreas)
			{
				customArea.Deconstruct(ref text, ref sCustomArea);
				string text2 = text;
				SCustomArea sCustomArea2 = sCustomArea;
				Log("Adding area '" + text2 + "' to method!");
				ILLabel val4 = il.DefineLabel();
				ILLabel val5 = il.DefineLabel();
				val.Emit(OpCodes.Ldarg_0);
				if (val3 != null)
				{
					val3.Target = val.Prev;
				}
				foreach (ILLabel item in list)
				{
					item.Target = val.Prev;
				}
				list.Clear();
				val.Emit(OpCodes.Ldfld, ReflectionHelper.GetFieldInfo(typeof(GameMap), "pd", true));
				val.Emit(OpCodes.Ldstr, sCustomArea2.PlayerDataBoolGotAreaMap);
				val.Emit(OpCodes.Callvirt, (MethodBase)ReflectionHelper.GetMethodInfo(typeof(PlayerData), "GetBool", true));
				val.Emit(OpCodes.Brtrue_S, (object)val5);
				ILLabel val6 = il.DefineLabel();
				for (int num = 0; num < sCustomArea2.MapZoneStrings.Count - 1; num++)
				{
					val.Emit(OpCodes.Ldloc_0);
					val.Emit(OpCodes.Ldstr, sCustomArea2.MapZoneStrings[num]);
					val.Emit(OpCodes.Callvirt, (MethodBase)ReflectionHelper.GetMethodInfo(typeof(string), "op_Equality", false));
					val.Emit(OpCodes.Brtrue_S, (object)val6);
				}
				if (sCustomArea2.MapZoneStrings.Count > 0)
				{
					val.Emit(OpCodes.Ldloc_0);
					val.Emit(OpCodes.Ldstr, sCustomArea2.MapZoneStrings[sCustomArea2.MapZoneStrings.Count - 1]);
					val.Emit(OpCodes.Callvirt, (MethodBase)ReflectionHelper.GetMethodInfo(typeof(string), "op_Equality", false));
					val.Emit(OpCodes.Brfalse_S, (object)val4);
				}
				val.Emit(OpCodes.Ldarg_0);
				val6.Target = val.Prev;
				val.Emit(OpCodes.Ldfld, ReflectionHelper.GetFieldInfo(typeof(GameMap), "pd", true));
				val.Emit(OpCodes.Ldstr, "equippedCharm_2");
				val.Emit(OpCodes.Callvirt, (MethodBase)ReflectionHelper.GetMethodInfo(typeof(PlayerData), "GetBool", true));
				val.Emit(OpCodes.Brtrue_S, (object)val5);
				val.Emit(OpCodes.Ldarg_0);
				val.Emit(OpCodes.Ldfld, ReflectionHelper.GetFieldInfo(typeof(GameMap), "pd", true));
				val.Emit(OpCodes.Ldstr, "CompassAlwaysOn.Enabled");
				val.Emit(OpCodes.Callvirt, (MethodBase)ReflectionHelper.GetMethodInfo(typeof(PlayerData), "GetBool", true));
				val.Emit(OpCodes.Brfalse_S, (object)val4);
				val.EmitReference<GameObject>(sCustomArea2.AreaGameObject);
				val5.Target = val.Prev.Previous;
				val.Emit(OpCodes.Ldarg_0);
				val.Emit(OpCodes.Ldfld, ReflectionHelper.GetFieldInfo(typeof(GameMap), "pd", true));
				val.Emit(OpCodes.Ldstr, sCustomArea2.PlayerDataBoolGotAreaMap);
				val.Emit(OpCodes.Callvirt, (MethodBase)ReflectionHelper.GetMethodInfo(typeof(PlayerData), "GetBool", true));
				val.Emit(OpCodes.Callvirt, (MethodBase)ReflectionHelper.GetMethodInfo(typeof(GameObject), "SetActive", true));
				float? panMinX = sCustomArea2.panMinX;
				if (panMinX.HasValue)
				{
					val.Emit(OpCodes.Ldarg_0);
					val.Emit(OpCodes.Ldarg_0);
					val.Emit(OpCodes.Ldfld, ReflectionHelper.GetFieldInfo(typeof(GameMap), "panMinX", true));
					val.Emit(OpCodes.Ldc_R4, (object)sCustomArea2.panMinX);
					val.Emit(OpCodes.Callvirt, (MethodBase)typeof(Mathf).GetMethods(BindingFlags.Static | BindingFlags.Public).First((MethodInfo x) => x.Name == "Min" && x.GetParameters()[0].ParameterType == typeof(float)));
					val.Emit(OpCodes.Stfld, ReflectionHelper.GetFieldInfo(typeof(GameMap), "panMinX", true));
				}
				panMinX = sCustomArea2.panMaxX;
				if (panMinX.HasValue)
				{
					val.Emit(OpCodes.Ldarg_0);
					val.Emit(OpCodes.Ldarg_0);
					val.Emit(OpCodes.Ldfld, ReflectionHelper.GetFieldInfo(typeof(GameMap), "panMaxX", true));
					val.Emit(OpCodes.Ldc_R4, (object)sCustomArea2.panMaxX);
					val.Emit(OpCodes.Callvirt, (MethodBase)typeof(Mathf).GetMethods(BindingFlags.Static | BindingFlags.Public).First((MethodInfo x) => x.Name == "Max" && x.GetParameters()[0].ParameterType == typeof(float)));
					val.Emit(OpCodes.Stfld, ReflectionHelper.GetFieldInfo(typeof(GameMap), "panMaxX", true));
				}
				panMinX = sCustomArea2.panMinY;
				if (panMinX.HasValue)
				{
					val.Emit(OpCodes.Ldarg_0);
					val.Emit(OpCodes.Ldarg_0);
					val.Emit(OpCodes.Ldfld, ReflectionHelper.GetFieldInfo(typeof(GameMap), "panMinY", true));
					val.Emit(OpCodes.Ldc_R4, (object)sCustomArea2.panMinY);
					val.Emit(OpCodes.Callvirt, (MethodBase)typeof(Mathf).GetMethods(BindingFlags.Static | BindingFlags.Public).First((MethodInfo x) => x.Name == "Min" && x.GetParameters()[0].ParameterType == typeof(float)));
					val.Emit(OpCodes.Stfld, ReflectionHelper.GetFieldInfo(typeof(GameMap), "panMinY", true));
				}
				panMinX = sCustomArea2.panMaxY;
				if (panMinX.HasValue)
				{
					val.Emit(OpCodes.Ldarg_0);
					val.Emit(OpCodes.Ldarg_0);
					val.Emit(OpCodes.Ldfld, ReflectionHelper.GetFieldInfo(typeof(GameMap), "panMaxY", true));
					val.Emit(OpCodes.Ldc_R4, (object)sCustomArea2.panMaxY);
					val.Emit(OpCodes.Callvirt, (MethodBase)typeof(Mathf).GetMethods(BindingFlags.Static | BindingFlags.Public).First((MethodInfo x) => x.Name == "Max" && x.GetParameters()[0].ParameterType == typeof(float)));
					val.Emit(OpCodes.Stfld, ReflectionHelper.GetFieldInfo(typeof(GameMap), "panMaxY", true));
				}
				val.Emit(OpCodes.Nop);
				val3 = val4;
			}
			if (val3 != null)
			{
				val3.Target = val.Prev;
			}
		}

		public static void NewQuickMapAncientBasin(orig_QuickMapAncientBasin orig, GameMap self)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			QuickMapNew(self, ((object)(MapZone)19/*cast due to constrained. prefix*/).ToString());
		}

		public static void NewQuickMapCity(orig_QuickMapCity orig, GameMap self)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			QuickMapNew(self, ((object)(MapZone)16/*cast due to constrained. prefix*/).ToString());
		}

		public static void NewQuickMapCliffs(orig_QuickMapCliffs orig, GameMap self)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			QuickMapNew(self, ((object)(MapZone)3/*cast due to constrained. prefix*/).ToString());
		}

		public static void NewQuickMapCrossroads(orig_QuickMapCrossroads orig, GameMap self)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			QuickMapNew(self, ((object)(MapZone)5/*cast due to constrained. prefix*/).ToString());
		}

		public static void NewQuickMapCrystalPeak(orig_QuickMapCrystalPeak orig, GameMap self)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			QuickMapNew(self, ((object)(MapZone)14/*cast due to constrained. prefix*/).ToString());
		}

		public static void NewQuickMapDeepnest(orig_QuickMapDeepnest orig, GameMap self)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			QuickMapNew(self, ((object)(MapZone)10/*cast due to constrained. prefix*/).ToString());
		}

		public static void NewQuickMapFogCanyon(orig_QuickMapFogCanyon orig, GameMap self)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			QuickMapNew(self, ((object)(MapZone)8/*cast due to constrained. prefix*/).ToString());
		}

		public static void NewQuickMapFungalWastes(orig_QuickMapFungalWastes orig, GameMap self)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			QuickMapNew(self, ((object)(MapZone)9/*cast due to constrained. prefix*/).ToString());
		}

		public static void NewQuickMapGreenpath(orig_QuickMapGreenpath orig, GameMap self)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			QuickMapNew(self, ((object)(MapZone)6/*cast due to constrained. prefix*/).ToString());
		}

		public static void NewQuickMapKingdomsEdge(orig_QuickMapKingdomsEdge orig, GameMap self)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			QuickMapNew(self, ((object)(MapZone)25/*cast due to constrained. prefix*/).ToString());
		}

		public static void NewQuickMapQueensGardens(orig_QuickMapQueensGardens orig, GameMap self)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			QuickMapNew(self, ((object)(MapZone)7/*cast due to constrained. prefix*/).ToString());
		}

		public static void NewQuickMapRestingGrounds(orig_QuickMapRestingGrounds orig, GameMap self)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			QuickMapNew(self, ((object)(MapZone)15/*cast due to constrained. prefix*/).ToString());
		}

		public static void NewQuickMapDirtmouth(orig_QuickMapDirtmouth orig, GameMap self)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			QuickMapNew(self, ((object)(MapZone)4/*cast due to constrained. prefix*/).ToString());
		}

		public static void NewQuickMapWaterways(orig_QuickMapWaterways orig, GameMap self)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			QuickMapNew(self, ((object)(MapZone)23/*cast due to constrained. prefix*/).ToString());
		}

		private static void NewCloseQuickMap(orig_CloseQuickMap orig, GameMap self)
		{
			orig.Invoke(self);
			foreach (KeyValuePair<string, SCustomArea> item in CustomAreas.Where((KeyValuePair<string, SCustomArea> pair) => (Object)(object)pair.Value.AreaGameObject != (Object)null))
			{
				item.Value.AreaGameObject.SetActive(false);
			}
		}

		private static void IlPositionCompass(ILContext il)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_051b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_0543: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_0572: Unknown result type (might be due to invalid IL or missing references)
			//IL_058d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0599: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_05da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0600: Unknown result type (might be due to invalid IL or missing references)
			//IL_0626: Unknown result type (might be due to invalid IL or missing references)
			//IL_0634: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			ILCursor val = new ILCursor(il);
			val.Goto(0, (MoveType)0, false);
			if (val.TryGotoNext((MoveType)0, new Func<Instruction, bool>[2]
			{
				(Instruction x) => ILPatternMatchingExt.MatchLdloc(x, 1),
				(Instruction x) => ILPatternMatchingExt.MatchLdstr(x, "DREAM_WORLD")
			}))
			{
				val.RemoveRange(20);
			}
			val.GotoNext((MoveType)0, new Func<Instruction, bool>[3]
			{
				(Instruction x) => ILPatternMatchingExt.MatchLdarg(x, 0),
				(Instruction x) => ILPatternMatchingExt.MatchLdfld<GameMap>(x, "currentScene"),
				(Instruction x) => ILPatternMatchingExt.MatchLdnull(x)
			});
			List<ILLabel> list = new List<ILLabel>();
			Enumerator<Instruction> enumerator = il.Instrs.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Instruction current = enumerator.Current;
					object operand = current.Operand;
					ILLabel val2 = (ILLabel)((operand is ILLabel) ? operand : null);
					if (val2 != null && current.Offset > 0 && current.Offset <= val.Prev.Offset && val2.Target.Offset > 0 && val2.Target.Offset >= val.Next.Offset && !list.Contains(val2))
					{
						Log($"Label from {current.Offset} to {val2.Target.Offset} while being before {val.Next.Offset} has to be adjusted");
						list.Add(val2);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
			ILLabel val3 = null;
			string text = default(string);
			SCustomArea sCustomArea = default(SCustomArea);
			foreach (KeyValuePair<string, SCustomArea> customArea in CustomAreas)
			{
				customArea.Deconstruct(ref text, ref sCustomArea);
				string text2 = text;
				SCustomArea sCustomArea2 = sCustomArea;
				Log("Adding area '" + text2 + "' to method!");
				ILLabel val4 = il.DefineLabel();
				bool flag = false;
				ILLabel val5 = il.DefineLabel();
				for (int num = 0; num < sCustomArea2.MapZoneStrings.Count - 1; num++)
				{
					val.Emit(OpCodes.Ldloc_1);
					if (val3 != null && !flag)
					{
						val3.Target = val.Prev;
						flag = true;
					}
					foreach (ILLabel item in list)
					{
						item.Target = val.Prev;
					}
					list.Clear();
					val.Emit(OpCodes.Ldstr, sCustomArea2.MapZoneStrings[num]);
					val.Emit(OpCodes.Callvirt, (MethodBase)ReflectionHelper.GetMethodInfo(typeof(string), "op_Equality", false));
					val.Emit(OpCodes.Brtrue_S, (object)val5);
				}
				if (sCustomArea2.MapZoneStrings.Count > 0)
				{
					val.Emit(OpCodes.Ldloc_1);
					if (val3 != null && !flag)
					{
						val3.Target = val.Prev;
					}
					foreach (ILLabel item2 in list)
					{
						item2.Target = val.Prev;
					}
					list.Clear();
					val.Emit(OpCodes.Ldstr, sCustomArea2.MapZoneStrings[sCustomArea2.MapZoneStrings.Count - 1]);
					val.Emit(OpCodes.Callvirt, (MethodBase)ReflectionHelper.GetMethodInfo(typeof(string), "op_Equality", false));
					val.Emit(OpCodes.Brfalse_S, (object)val4);
				}
				val.EmitReference<GameObject>(sCustomArea2.AreaGameObject);
				val5.Target = val.Prev.Previous;
				val.Emit(OpCodes.Stloc_0);
				ILLabel val6 = il.DefineLabel();
				val.Emit(OpCodes.Ldc_I4_0);
				val.Emit(OpCodes.Stloc_S, (byte)29);
				val.Emit(OpCodes.Br_S, (object)val6);
				ILLabel val7 = il.DefineLabel();
				val.Emit(OpCodes.Ldloc_0);
				val7.Target = val.Prev;
				val.Emit(OpCodes.Callvirt, (MethodBase)ReflectionHelper.GetPropertyInfo(typeof(GameObject), "transform", true).GetGetMethod());
				val.Emit(OpCodes.Ldloc_S, (byte)29);
				val.Emit(OpCodes.Callvirt, (MethodBase)ReflectionHelper.GetMethodInfo(typeof(Transform), "GetChild", true));
				val.Emit(OpCodes.Callvirt, (MethodBase)ReflectionHelper.GetPropertyInfo(typeof(Component), "gameObject", true).GetGetMethod());
				val.Emit(OpCodes.Stloc_S, (byte)30);
				ILLabel val8 = il.DefineLabel();
				val.Emit(OpCodes.Ldloc_S, (byte)30);
				val.Emit(OpCodes.Callvirt, (MethodBase)ReflectionHelper.GetPropertyInfo(typeof(Object), "name", true).GetGetMethod());
				val.Emit(OpCodes.Ldloc_2);
				val.Emit(OpCodes.Callvirt, (MethodBase)ReflectionHelper.GetMethodInfo(typeof(string), "op_Equality", false));
				val.Emit(OpCodes.Brfalse_S, (object)val8);
				val.Emit(OpCodes.Ldarg_0);
				val.Emit(OpCodes.Ldloc_S, (byte)30);
				val.Emit(OpCodes.Stfld, ReflectionHelper.GetFieldInfo(typeof(GameMap), "currentScene", true));
				val.Emit(OpCodes.Br_S, (object)val4);
				val.Emit(OpCodes.Ldloc_S, (byte)29);
				val8.Target = val.Prev;
				val.Emit(OpCodes.Ldc_I4_1);
				val.Emit(OpCodes.Add);
				val.Emit(OpCodes.Stloc_S, (byte)29);
				val.Emit(OpCodes.Ldloc_S, (byte)29);
				val6.Target = val.Prev;
				val.Emit(OpCodes.Ldloc_0);
				val.Emit(OpCodes.Callvirt, (MethodBase)ReflectionHelper.GetPropertyInfo(typeof(GameObject), "transform", true).GetGetMethod());
				val.Emit(OpCodes.Callvirt, (MethodBase)ReflectionHelper.GetPropertyInfo(typeof(Transform), "childCount", true).GetGetMethod());
				val.Emit(OpCodes.Blt_S, (object)val7);
				val.Emit(OpCodes.Nop);
				val3 = val4;
			}
			if (val3 != null)
			{
				val3.Target = val.Prev;
			}
		}
	}
	internal class MappedCustomRoom : MonoBehaviour
	{
		[UsedImplicitly]
		private void Awake()
		{
			for (int i = 0; i < ((Component)this).transform.childCount; i++)
			{
				((Component)((Component)this).transform.GetChild(i)).gameObject.SetActive(true);
			}
		}
	}
}
namespace AdditionalMaps.Consts
{
	public class LanguageStrings
	{
		public const string PathOfPainKey = "PathOfPainArea";

		public const string WorkshopKey = "WorkshopArea";

		public const string CreditsKey = "CreditsArea";

		public const string WpMapKey = "SFGrenadeAdditionalMaps_WpMapName";

		public const string PantheonKey = "PantheonArea";

		public const string PoHKey = "PohArea";

		public const string GhMapKey = "SFGrenadeAdditionalMaps_GhMapName";

		private readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> _jsonDict;

		public LanguageStrings()
		{
			using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AdditionalMaps.Resources.Language.json");
			if (stream != null)
			{
				byte[] array = new byte[stream.Length];
				stream.Read(array, 0, array.Length);
				stream.Dispose();
				string text = Encoding.Default.GetString(array);
				_jsonDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(text);
				Logger.Log("[AdditionalMaps.Consts.LanguageStrings] - Loaded Language");
			}
		}

		public unsafe string Get(string key, string sheet)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			SupportedLanguages gameLanguage = GameManager.instance.gameSettings.gameLanguage;
			try
			{
				return _jsonDict[((object)(*(SupportedLanguages*)(&gameLanguage))/*cast due to constrained. prefix*/).ToString()][sheet][key].Replace("<br>", "\n");
			}
			catch
			{
				return _jsonDict[((object)(SupportedLanguages)44/*cast due to constrained. prefix*/).ToString()][sheet][key].Replace("<br>", "\n");
			}
		}

		public unsafe bool ContainsKey(string key, string sheet)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				SupportedLanguages gameLanguage = GameManager.instance.gameSettings.gameLanguage;
				try
				{
					return _jsonDict[((object)(*(SupportedLanguages*)(&gameLanguage))/*cast due to constrained. prefix*/).ToString()][sheet].ContainsKey(key);
				}
				catch
				{
					try
					{
						return _jsonDict[((object)(SupportedLanguages)44/*cast due to constrained. prefix*/).ToString()][sheet].ContainsKey(key);
					}
					catch
					{
						return false;
					}
				}
			}
			catch
			{
				return false;
			}
		}
	}
	public class TextureStrings
	{
		public const string CustomAreaKey = "CustomArea";

		private const string CustomAreaFile = "AdditionalMaps.Resources.CustomArea.png";

		public const string CustomGhAreaKey = "CustomArea.GH";

		private const string CustomGhAreaFile = "AdditionalMaps.Resources.CustomArea.GH.png";

		public const string CustomWpAreaKey = "CustomArea.WP";

		private const string CustomWpAreaFile = "AdditionalMaps.Resources.CustomArea.WP.png";

		public const string Wp01Key = "White_Palace_01";

		private const string Wp01File = "AdditionalMaps.Resources.White_Palace.WP01.png";

		public const string Wp02Key = "White_Palace_02";

		private const string Wp02File = "AdditionalMaps.Resources.White_Palace.WP02.png";

		public const string Wp03Key = "White_Palace_03_hub";

		private const string Wp03File = "AdditionalMaps.Resources.White_Palace.WP03.png";

		public const string Wp04Key = "White_Palace_04";

		private const string Wp04File = "AdditionalMaps.Resources.White_Palace.WP04.png";

		public const string Wp05Key = "White_Palace_05";

		private const string Wp05File = "AdditionalMaps.Resources.White_Palace.WP05.png";

		public const string Wp06Key = "White_Palace_06";

		private const string Wp06File = "AdditionalMaps.Resources.White_Palace.WP06.png";

		public const string Wp07Key = "White_Palace_07";

		private const string Wp07File = "AdditionalMaps.Resources.White_Palace.WP07.png";

		public const string Wp08Key = "White_Palace_08";

		private const string Wp08File = "AdditionalMaps.Resources.White_Palace.WP08.png";

		public const string Wp09Key = "White_Palace_09";

		private const string Wp09File = "AdditionalMaps.Resources.White_Palace.WP09.png";

		public const string Wp12Key = "White_Palace_12";

		private const string Wp12File = "AdditionalMaps.Resources.White_Palace.WP12.png";

		public const string Wp13Key = "White_Palace_13";

		private const string Wp13File = "AdditionalMaps.Resources.White_Palace.WP13.png";

		public const string Wp14Key = "White_Palace_14";

		private const string Wp14File = "AdditionalMaps.Resources.White_Palace.WP14.png";

		public const string Wp15Key = "White_Palace_15";

		private const string Wp15File = "AdditionalMaps.Resources.White_Palace.WP15.png";

		public const string Wp16Key = "White_Palace_16";

		private const string Wp16File = "AdditionalMaps.Resources.White_Palace.WP16.png";

		public const string Wp17Key = "White_Palace_17";

		private const string Wp17File = "AdditionalMaps.Resources.White_Palace.WP17.png";

		public const string Wp18Key = "White_Palace_18";

		private const string Wp18File = "AdditionalMaps.Resources.White_Palace.WP18.png";

		public const string Wp19Key = "White_Palace_19";

		private const string Wp19File = "AdditionalMaps.Resources.White_Palace.WP19.png";

		public const string Wp20Key = "White_Palace_20";

		private const string Wp20File = "AdditionalMaps.Resources.White_Palace.WP20.png";

		public const string WpMapKey = "WPMap";

		private const string WpMapFile = "AdditionalMaps.Resources.White_Palace.White_Palace_Transparent.png";

		public const string RealWp01Key = "RWP01";

		private const string RealWp01File = "AdditionalMaps.Resources.White_Palace.RWP01.png";

		public const string RealWp02Key = "RWP02";

		private const string RealWp02File = "AdditionalMaps.Resources.White_Palace.RWP02.png";

		public const string RealWp03Key = "RWP03";

		private const string RealWp03File = "AdditionalMaps.Resources.White_Palace.RWP03.png";

		public const string RealWp04Key = "RWP04";

		private const string RealWp04File = "AdditionalMaps.Resources.White_Palace.RWP04.png";

		public const string RealWp05Key = "RWP05";

		private const string RealWp05File = "AdditionalMaps.Resources.White_Palace.RWP05.png";

		public const string RealWp06Key = "RWP06";

		private const string RealWp06File = "AdditionalMaps.Resources.White_Palace.RWP06.png";

		public const string RealWp07Key = "RWP07";

		private const string RealWp07File = "AdditionalMaps.Resources.White_Palace.RWP07.png";

		public const string RealWp08Key = "RWP08";

		private const string RealWp08File = "AdditionalMaps.Resources.White_Palace.RWP08.png";

		public const string RealWp09Key = "RWP09";

		private const string RealWp09File = "AdditionalMaps.Resources.White_Palace.RWP09.png";

		public const string RealWp12Key = "RWP12";

		private const string RealWp12File = "AdditionalMaps.Resources.White_Palace.RWP12.png";

		public const string RealWp13Key = "RWP13";

		private const string RealWp13File = "AdditionalMaps.Resources.White_Palace.RWP13.png";

		public const string RealWp14Key = "RWP14";

		private const string RealWp14File = "AdditionalMaps.Resources.White_Palace.RWP14.png";

		public const string RealWp15Key = "RWP15";

		private const string RealWp15File = "AdditionalMaps.Resources.White_Palace.RWP15.png";

		public const string RealWp16Key = "RWP16";

		private const string RealWp16File = "AdditionalMaps.Resources.White_Palace.RWP16.png";

		public const string RealWp17Key = "RWP17";

		private const string RealWp17File = "AdditionalMaps.Resources.White_Palace.RWP17.png";

		public const string RealWp18Key = "RWP18";

		private const string RealWp18File = "AdditionalMaps.Resources.White_Palace.RWP18.png";

		public const string RealWp19Key = "RWP19";

		private const string RealWp19File = "AdditionalMaps.Resources.White_Palace.RWP19.png";

		public const string RealWp20Key = "RWP20";

		private const string RealWp20File = "AdditionalMaps.Resources.White_Palace.RWP20.png";

		public const string Sm1Key = "SM1";

		private const string Sm1File = "AdditionalMaps.Resources.scattermaps1.png";

		public const string Sm2Key = "SM2";

		private const string Sm2File = "AdditionalMaps.Resources.scattermaps2.png";

		public const string Sm3Key = "SM3";

		private const string Sm3File = "AdditionalMaps.Resources.scattermaps3.png";

		public const string MapKey = "MapIcon";

		private const string MapFile = "AdditionalMaps.Resources.MapIcon.png";

		public const string GhAKey = "GG_Atrium";

		private const string GhAFile = "AdditionalMaps.Resources.Godhome.GH0.png";

		public const string GhArKey = "GG_Atrium_Roof";

		private const string GhArFile = "AdditionalMaps.Resources.Godhome.GH1.png";

		public const string RealGhAKey = "RGH0";

		private const string RealGhAFile = "AdditionalMaps.Resources.Godhome.RGH0.png";

		public const string RealGhArKey = "RGH1";

		private const string RealGhArFile = "AdditionalMaps.Resources.Godhome.RGH1.png";

		private readonly Dictionary<string, Sprite> _dict;

		public TextureStrings()
		{
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Expected O, but got Unknown
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			base..ctor();
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			_dict = new Dictionary<string, Sprite>();
			foreach (KeyValuePair<string, string> item in new Dictionary<string, string>
			{
				{ "CustomArea", "AdditionalMaps.Resources.CustomArea.png" },
				{ "CustomArea.GH", "AdditionalMaps.Resources.CustomArea.GH.png" },
				{ "CustomArea.WP", "AdditionalMaps.Resources.CustomArea.WP.png" },
				{ "White_Palace_01", "AdditionalMaps.Resources.White_Palace.WP01.png" },
				{ "White_Palace_02", "AdditionalMaps.Resources.White_Palace.WP02.png" },
				{ "White_Palace_03_hub", "AdditionalMaps.Resources.White_Palace.WP03.png" },
				{ "White_Palace_04", "AdditionalMaps.Resources.White_Palace.WP04.png" },
				{ "White_Palace_05", "AdditionalMaps.Resources.White_Palace.WP05.png" },
				{ "White_Palace_06", "AdditionalMaps.Resources.White_Palace.WP06.png" },
				{ "White_Palace_07", "AdditionalMaps.Resources.White_Palace.WP07.png" },
				{ "White_Palace_08", "AdditionalMaps.Resources.White_Palace.WP08.png" },
				{ "White_Palace_09", "AdditionalMaps.Resources.White_Palace.WP09.png" },
				{ "White_Palace_12", "AdditionalMaps.Resources.White_Palace.WP12.png" },
				{ "White_Palace_13", "AdditionalMaps.Resources.White_Palace.WP13.png" },
				{ "White_Palace_14", "AdditionalMaps.Resources.White_Palace.WP14.png" },
				{ "White_Palace_15", "AdditionalMaps.Resources.White_Palace.WP15.png" },
				{ "White_Palace_16", "AdditionalMaps.Resources.White_Palace.WP16.png" },
				{ "White_Palace_17", "AdditionalMaps.Resources.White_Palace.WP17.png" },
				{ "White_Palace_18", "AdditionalMaps.Resources.White_Palace.WP18.png" },
				{ "White_Palace_19", "AdditionalMaps.Resources.White_Palace.WP19.png" },
				{ "White_Palace_20", "AdditionalMaps.Resources.White_Palace.WP20.png" },
				{ "WPMap", "AdditionalMaps.Resources.White_Palace.White_Palace_Transparent.png" },
				{ "RWP01", "AdditionalMaps.Resources.White_Palace.RWP01.png" },
				{ "RWP02", "AdditionalMaps.Resources.White_Palace.RWP02.png" },
				{ "RWP03", "AdditionalMaps.Resources.White_Palace.RWP03.png" },
				{ "RWP04", "AdditionalMaps.Resources.White_Palace.RWP04.png" },
				{ "RWP05", "AdditionalMaps.Resources.White_Palace.RWP05.png" },
				{ "RWP06", "AdditionalMaps.Resources.White_Palace.RWP06.png" },
				{ "RWP07", "AdditionalMaps.Resources.White_Palace.RWP07.png" },
				{ "RWP08", "AdditionalMaps.Resources.White_Palace.RWP08.png" },
				{ "RWP09", "AdditionalMaps.Resources.White_Palace.RWP09.png" },
				{ "RWP12", "AdditionalMaps.Resources.White_Palace.RWP12.png" },
				{ "RWP13", "AdditionalMaps.Resources.White_Palace.RWP13.png" },
				{ "RWP14", "AdditionalMaps.Resources.White_Palace.RWP14.png" },
				{ "RWP15", "AdditionalMaps.Resources.White_Palace.RWP15.png" },
				{ "RWP16", "AdditionalMaps.Resources.White_Palace.RWP16.png" },
				{ "RWP17", "AdditionalMaps.Resources.White_Palace.RWP17.png" },
				{ "RWP18", "AdditionalMaps.Resources.White_Palace.RWP18.png" },
				{ "RWP19", "AdditionalMaps.Resources.White_Palace.RWP19.png" },
				{ "RWP20", "AdditionalMaps.Resources.White_Palace.RWP20.png" },
				{ "SM1", "AdditionalMaps.Resources.scattermaps1.png" },
				{ "SM2", "AdditionalMaps.Resources.scattermaps2.png" },
				{ "SM3", "AdditionalMaps.Resources.scattermaps3.png" },
				{ "MapIcon", "AdditionalMaps.Resources.MapIcon.png" },
				{ "GG_Atrium", "AdditionalMaps.Resources.Godhome.GH0.png" },
				{ "GG_Atrium_Roof", "AdditionalMaps.Resources.Godhome.GH1.png" },
				{ "RGH0", "AdditionalMaps.Resources.Godhome.RGH0.png" },
				{ "RGH1", "AdditionalMaps.Resources.Godhome.RGH1.png" }
			})
			{
				using Stream stream = executingAssembly.GetManifestResourceStream(item.Value);
				if (stream != null)
				{
					byte[] array = new byte[stream.Length];
					stream.Read(array, 0, array.Length);
					stream.Dispose();
					Texture2D val = new Texture2D(2, 2);
					ImageConversion.LoadImage(val, array, true);
					float num = (item.Value.Contains("scattermaps") ? 64 : 100);
					_dict.Add(item.Key, Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), new Vector2(0.5f, 0.5f), num, 0u, (SpriteMeshType)0));
				}
			}
		}

		public Sprite Get(string key)
		{
			return _dict[key];
		}
	}
}
