using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using GlobalEnums;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Modding;
using Modding.Delegates;
using MonoMod.RuntimeDetour;
using Newtonsoft.Json;
using On;
using SFCore.Generics;
using SFCore.MonoBehaviours;
using SFCore.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using WavLib;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: AssemblyCompany("SFCore")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyCopyright("Copyright ? SFGrenade 2025")]
[assembly: AssemblyDescription("A Hollow Knight Mod Library")]
[assembly: AssemblyFileVersion("1.5.14.8")]
[assembly: AssemblyInformationalVersion("1.5.14.8ºº»¯")]
[assembly: AssemblyProduct("SFCore")]
[assembly: AssemblyTitle("SFCore")]
[assembly: AssemblyVersion("1.5.14.8")]
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
namespace SFCore
{
	public struct SCustomAchievement
	{
		public string key;

		public Sprite sprite;

		public string titleConvo;

		public string textConvo;

		public bool hidden;
	}
	public static class AchievementHelper
	{
		private static List<SCustomAchievement> _customAchievements;

		static AchievementHelper()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			_customAchievements = new List<SCustomAchievement>();
			LogFine("!cctor");
			UIManager.RefreshAchievementsList += new hook_RefreshAchievementsList(OnUIManagerRefreshAchievementsList);
			DesktopPlatform.IsAchievementUnlocked += new hook_IsAchievementUnlocked(OnDesktopPlatformOnIsAchievementUnlocked);
			LogFine("~cctor");
		}

		public static void unusedInit()
		{
			LogFine("!unusedInit");
			LogFine("~unusedInit");
		}

		public static void AddAchievement(string key, Sprite sprite, string titleConvo, string textConvo, bool hidden)
		{
			LogFine("!AddAchievement");
			foreach (SCustomAchievement customAchievement in _customAchievements)
			{
				if (customAchievement.key.Length == key.Length && customAchievement.key == key)
				{
					LogError($"Achievement with key '{key}' already exists! It's defined as ('{customAchievement.key}', {customAchievement.sprite}, '{customAchievement.titleConvo}', '{customAchievement.textConvo}', {customAchievement.hidden})!");
					LogFine("~AddAchievement");
					return;
				}
			}
			LogDebug("Adding achievement '" + key + "'");
			_customAchievements.Add(new SCustomAchievement
			{
				key = key,
				sprite = sprite,
				titleConvo = titleConvo,
				textConvo = textConvo,
				hidden = hidden
			});
			LogFine("~AddAchievement");
		}

		private static void InitAchievements(AchievementsList list)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Expected O, but got Unknown
			LogFine("!InitAchievements");
			foreach (SCustomAchievement ca in _customAchievements)
			{
				if (!list.achievements.Exists((Achievement a) => a.key.Length == ca.key.Length && a.key == ca.key))
				{
					Achievement item = new Achievement
					{
						key = ca.key,
						type = (AchievementType)(ca.hidden ? 1 : 0),
						earnedIcon = ca.sprite,
						unearnedIcon = ca.sprite,
						localizedText = ca.textConvo,
						localizedTitle = ca.titleConvo
					};
					list.achievements.Add(item);
				}
			}
			LogFine("~InitAchievements");
		}

		private static void OnUIManagerRefreshAchievementsList(orig_RefreshAchievementsList orig, UIManager self)
		{
			LogFine("!OnUIManagerRefreshAchievementsList");
			InitAchievements(GameManager.instance.achievementHandler.achievementsList);
			orig.Invoke(self);
			LogFine("~OnUIManagerRefreshAchievementsList");
		}

		private static bool? OnDesktopPlatformOnIsAchievementUnlocked(orig_IsAchievementUnlocked orig, DesktopPlatform self, string key)
		{
			LogFine("!OnDesktopPlatformIsAchievementUnlocked");
			bool? result = orig.Invoke(self, key);
			if (_customAchievements.Exists((SCustomAchievement x) => x.key.Length == key.Length && x.key == key))
			{
				result = ((Platform)self).EncryptedSharedData.GetBool(key, false);
			}
			LogFine("~OnDesktopPlatformIsAchievementUnlocked");
			return result;
		}

		private static void LogFine(string message)
		{
			InternalLogger.LogFine(message, "[SFCore]:[AchievementHelper]");
		}

		private static void LogFine(object message)
		{
			LogFine($"{message}");
		}

		private static void LogDebug(string message)
		{
			InternalLogger.LogDebug(message, "[SFCore]:[AchievementHelper]");
		}

		private static void LogDebug(object message)
		{
			LogDebug($"{message}");
		}

		private static void Log(string message)
		{
			InternalLogger.Log(message, "[SFCore]:[AchievementHelper]");
		}

		private static void Log(object message)
		{
			Log($"{message}");
		}

		private static void LogWarn(string message)
		{
			InternalLogger.LogWarn(message, "[SFCore]:[AchievementHelper]");
		}

		private static void LogWarn(object message)
		{
			LogWarn($"{message}");
		}

		private static void LogError(string message)
		{
			InternalLogger.LogError(message, "[SFCore]:[AchievementHelper]");
		}

		private static void LogError(object message)
		{
			LogError($"{message}");
		}
	}
	public static class CharmHelper
	{
		private static readonly List<Sprite> CustomSprites;

		private static Detour BuildEquippedCharms_Start_hook;

		private static Hook GameCameras_Start_hook;

		static CharmHelper()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Expected O, but got Unknown
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Expected O, but got Unknown
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Expected O, but got Unknown
			CustomSprites = new List<Sprite>();
			LogFine("!cctor");
			PlayerData.CalculateNotchesUsed += new hook_CalculateNotchesUsed(OnPlayerDataCalculateNotchesUsed);
			BuildEquippedCharms_Start_hook = new Detour((MethodBase)typeof(BuildEquippedCharms).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic), (MethodBase)typeof(CharmHelper).GetMethod("OnBuildEquippedCharmsStart_single", BindingFlags.Static | BindingFlags.NonPublic));
			BuildEquippedCharms_Start_hook.Apply();
			GameManager.Start += new hook_Start(OnGameManagerStart);
			ModHooks.AfterSavegameLoadHook += ModHooksOnAfterSavegameLoadHook;
			ModHooks.BeforeSavegameSaveHook += ModHooksOnBeforeSavegameSaveHook;
			ModHooks.SavegameSaveHook += ModHooksOnSavegameSaveHook;
			GameCameras_Start_hook = new Hook((MethodBase)typeof(GameCameras).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic), typeof(CharmHelper).GetMethod("GameCamerasStart_single", BindingFlags.Static | BindingFlags.NonPublic));
			GameCameras_Start_hook.Apply();
			if ((Object)(object)GameCameras.instance != (Object)null)
			{
				GameCamerasStart_single(delegate
				{
				}, GameCameras.instance);
			}
			BuildEquippedCharms.BuildCharmList += (hook_BuildCharmList)delegate(orig_BuildCharmList orig, BuildEquippedCharms self)
			{
				LogFine("!OnBuildEquippedCharmsBuildCharmList");
				if (self.gameObjectList.Count <= 40)
				{
					InitBuildEquippedCharms(self);
				}
				orig.Invoke(self);
				LogFine("~OnBuildEquippedCharmsBuildCharmList");
			};
			LogFine("~cctor");
		}

		private static void ModHooksOnSavegameSaveHook(int obj)
		{
			LogFine("!ModHooksOnSavegameSaveHook");
			AddCustomCharmsToPlayerData(PlayerData.instance);
			LogFine("~ModHooksOnSavegameSaveHook");
		}

		private static void ModHooksOnBeforeSavegameSaveHook(SaveGameData obj)
		{
			LogFine("!ModHooksOnBeforeSavegameSaveHook");
			RemoveCustomCharmsFromPlayerData(obj.playerData);
			LogFine("~ModHooksOnBeforeSavegameSaveHook");
		}

		private static void ModHooksOnAfterSavegameLoadHook(SaveGameData obj)
		{
			LogFine("!ModHooksOnAfterSavegameLoadHook");
			AddCustomCharmsToPlayerData(obj.playerData);
			LogFine("~ModHooksOnAfterSavegameLoadHook");
		}

		private static void RemoveCustomCharmsFromPlayerData(PlayerData pd)
		{
			LogFine("!RemoveCustomCharmsFromPlayerData");
			for (int num = pd.equippedCharms.Count - 1; num >= 0; num--)
			{
				if (pd.equippedCharms[num] > 40)
				{
					pd.equippedCharms.RemoveAt(num);
				}
			}
			LogFine("~RemoveCustomCharmsFromPlayerData");
		}

		private static void AddCustomCharmsToPlayerData(PlayerData pd)
		{
			LogFine("!AddCustomCharmsToPlayerData");
			for (int i = 41; i <= 40 + FullSettingsMod<SFCoreSaveSettings, SFCoreGlobalSettings>.GlobalSettings.MaxCustomCharms; i++)
			{
				if (!pd.equippedCharms.Contains(i) && PlayerData.instance.GetBool($"gotCharm_{i}") && PlayerData.instance.GetBool($"equippedCharm_{i}"))
				{
					pd.equippedCharms.Add(i);
				}
			}
			LogFine("~AddCustomCharmsToPlayerData");
		}

		public static void unusedInit()
		{
			LogFine("!unusedInit");
			LogFine("~unusedInit");
		}

		public static List<int> AddSprites(params Sprite[] charmSprites)
		{
			LogFine("!AddSprites");
			LogDebug($"Adding {charmSprites.Length} charm sprites");
			List<int> list = new List<int>();
			foreach (Sprite val in charmSprites)
			{
				SetInactive((Object)(object)val);
				CustomSprites.Add(val);
				list.Add(40 + CustomSprites.Count);
			}
			FullSettingsMod<SFCoreSaveSettings, SFCoreGlobalSettings>.GlobalSettings.MaxCustomCharms = Mathf.Max(FullSettingsMod<SFCoreSaveSettings, SFCoreGlobalSettings>.GlobalSettings.MaxCustomCharms, CustomSprites.Count);
			LogFine("~AddSprites");
			return list;
		}

		private static void OnGameManagerStart(orig_Start orig, GameManager self)
		{
			LogFine("!OnGameManagerStart");
			FullSettingsMod<SFCoreSaveSettings, SFCoreGlobalSettings>.GlobalSettings.MaxCustomCharms = Mathf.Max(FullSettingsMod<SFCoreSaveSettings, SFCoreGlobalSettings>.GlobalSettings.MaxCustomCharms, CustomSprites.Count);
			orig.Invoke(self);
			ClearModdedCharmsFromUiCharms();
			LogFine("~OnGameManagerStart");
		}

		private static void ClearModdedCharmsFromUiCharms()
		{
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			LogFine("!ClearModdedCharmsFromUiCharms");
			GameObject val = ((Component)GameCameras.instance.hudCamera).gameObject.Find("Inventory");
			if ((Object)(object)val == (Object)null)
			{
				LogFine("~ClearModdedCharmsFromUiCharms EARLY (invGo == null)");
				return;
			}
			GameObject val2 = val.Find("Charms");
			if ((Object)(object)val2 == (Object)null)
			{
				LogFine("~ClearModdedCharmsFromUiCharms EARLY (charmsGo == null)");
				return;
			}
			PlayMakerFSM val3 = FSMUtility.LocateMyFSM(val2, "UI Charms");
			if ((Object)(object)val3 == (Object)null)
			{
				LogFine("~ClearModdedCharmsFromUiCharms EARLY (charmsFsm == null)");
				return;
			}
			GameObject val4 = val2.Find("Collected Charms");
			CharmIconList instance = CharmIconList.Instance;
			if ((Object)(object)instance == (Object)null)
			{
				return;
			}
			List<Sprite> list = new List<Sprite>(instance.spriteList);
			if (list.Count > 41)
			{
				list.RemoveRange(41, list.Count - 41);
				instance.spriteList = list.ToArray();
			}
			int num = 4;
			val3.GetAction<IntCompare>("Down", 0).integer2.Value = (num - 1) * 10;
			List<FsmInt> list2 = new List<FsmInt>();
			List<FsmEvent> list3 = new List<FsmEvent>();
			for (int i = 1; i <= num * 10; i += 10)
			{
				list2.Add(FsmInt.op_Implicit(i));
				list3.Add(FsmEvent.FindEvent("TO LEFT"));
			}
			val3.GetAction<IntSwitch>("Left", 0).compareTo = list2.ToArray();
			val3.GetAction<IntSwitch>("Left", 0).sendEvent = list3.ToArray();
			List<FsmInt> list4 = new List<FsmInt>();
			List<FsmEvent> list5 = new List<FsmEvent>();
			for (int j = 10; j <= num * 10; j += 10)
			{
				list4.Add(FsmInt.op_Implicit(j));
				list5.Add(FsmEvent.FindEvent("TO RIGHT"));
			}
			val3.GetAction<IntSwitch>("Right", 0).compareTo = list4.ToArray();
			val3.GetAction<IntSwitch>("Right", 0).sendEvent = list5.ToArray();
			val3.GetAction<IntClamp>("Update Cursor", 1).maxValue = FsmInt.op_Implicit(num * 10);
			GameObject o = val2.Find("Backboards");
			for (int num2 = val4.transform.childCount; num2 >= 41; num2--)
			{
				if ((Object)(object)o.Find($"BB {num2}") != (Object)null)
				{
					Object.DestroyImmediate((Object)(object)o.Find($"BB {num2}"));
				}
			}
			GameObject o2 = val2.Find("Collected Charms");
			for (int num3 = val4.transform.childCount; num3 >= 41; num3--)
			{
				if ((Object)(object)o2.Find(num3.ToString()) != (Object)null)
				{
					Object.DestroyImmediate((Object)(object)o2.Find(num3.ToString()));
				}
			}
			float num4 = Mathf.Ceil((40f + (float)CustomSprites.Count) / 10f) / 4f;
			for (int k = 0; k <= 3; k++)
			{
				for (int l = 1; l <= 10; l++)
				{
					GameObject val5 = o.Find("BB " + (10 * k + l));
					if ((Object)(object)val5 != (Object)null)
					{
						_ = val5.transform.localPosition;
					}
					GameObject val6 = o2.Find((10 * k + l).ToString());
					if ((Object)(object)val6 != (Object)null)
					{
						_ = val6.transform.localPosition;
					}
				}
			}
			LogFine("~ClearModdedCharmsFromUiCharms");
		}

		private static void InitUiCharms()
		{
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_075a: Unknown result type (might be due to invalid IL or missing references)
			//IL_075f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0768: Unknown result type (might be due to invalid IL or missing references)
			//IL_076d: Unknown result type (might be due to invalid IL or missing references)
			//IL_076f: Unknown result type (might be due to invalid IL or missing references)
			//IL_078e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0797: Unknown result type (might be due to invalid IL or missing references)
			//IL_079e: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0849: Unknown result type (might be due to invalid IL or missing references)
			//IL_084e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0857: Unknown result type (might be due to invalid IL or missing references)
			//IL_085c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0811: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0863: Unknown result type (might be due to invalid IL or missing references)
			//IL_0882: Unknown result type (might be due to invalid IL or missing references)
			//IL_088b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0892: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0900: Unknown result type (might be due to invalid IL or missing references)
			//IL_0915: Unknown result type (might be due to invalid IL or missing references)
			//IL_091c: Unknown result type (might be due to invalid IL or missing references)
			LogFine("!InitUiCharms");
			GameObject val = ((Component)GameCameras.instance.hudCamera).gameObject.Find("Inventory").Find("Charms");
			PlayMakerFSM fsm = FSMUtility.LocateMyFSM(val, "UI Charms");
			int num = val.Find("Collected Charms").transform.childCount - 1;
			int num2 = num + CustomSprites.Count;
			CharmIconList instance = CharmIconList.Instance;
			List<Sprite> list = new List<Sprite>(instance.spriteList);
			for (int i = 0; i < CustomSprites.Count; i++)
			{
				SetInactive((Object)(object)CustomSprites[i]);
				list.Add(CustomSprites[i]);
			}
			instance.spriteList = list.ToArray();
			int num3 = Mathf.CeilToInt((float)num2 / 10f);
			fsm.GetAction<IntCompare>("Down", 0).integer2.Value = (num3 - 1) * 10;
			List<FsmInt> list2 = new List<FsmInt>();
			List<FsmEvent> list3 = new List<FsmEvent>();
			for (int j = 1; j <= num3 * 10; j += 10)
			{
				list2.Add(FsmInt.op_Implicit(j));
				list3.Add(FsmEvent.FindEvent("TO LEFT"));
			}
			fsm.GetAction<IntSwitch>("Left", 0).compareTo = list2.ToArray();
			fsm.GetAction<IntSwitch>("Left", 0).sendEvent = list3.ToArray();
			List<FsmInt> list4 = new List<FsmInt>();
			List<FsmEvent> list5 = new List<FsmEvent>();
			for (int k = 10; k <= num3 * 10; k += 10)
			{
				list4.Add(FsmInt.op_Implicit(k));
				list5.Add(FsmEvent.FindEvent("TO RIGHT"));
			}
			fsm.GetAction<IntSwitch>("Right", 0).compareTo = list4.ToArray();
			fsm.GetAction<IntSwitch>("Right", 0).sendEvent = list5.ToArray();
			fsm.GetAction<IntClamp>("Update Cursor", 1).maxValue = FsmInt.op_Implicit(num3 * 10);
			GameObject val2 = val.Find("Backboards");
			for (int l = 1; l <= num3 * 10; l++)
			{
				int num4 = (l - 1) % 20 + 21;
				if ((Object)(object)val2.Find($"BB {l}") == (Object)null)
				{
					GameObject val3 = val2.Find($"BB {num4}");
					GameObject val4 = val2.Find($"BB {num4 - 20}");
					GameObject obj = Object.Instantiate<GameObject>(val3, val2.transform, true);
					AddToCharmFadeGroup(obj, ((Component)val2.transform.parent).gameObject);
					obj.transform.localPosition = val3.transform.localPosition + (val3.transform.localPosition - val4.transform.localPosition) * (((float)l / 10f - (float)num4 / 10f) / 2f);
					((Object)obj).name = $"BB {l}";
					obj.SetActive(val3.activeSelf);
				}
				if (l > 40)
				{
					GameObject obj2 = val2.Find($"BB {l}");
					bool activeSelf = obj2.activeSelf;
					obj2.SetActive(false);
					InvCharmBackboard component = obj2.GetComponent<InvCharmBackboard>();
					component.charmNum = l;
					component.charmNumString = l.ToString();
					if (l > 40 + CustomSprites.Count)
					{
						component.gotCharmString = "";
						component.newCharmString = "";
						component.GetAttr<InvCharmBackboard, SpriteRenderer>("spriteRenderer").sprite = component.blankSprite;
						component.SetAttr<InvCharmBackboard, bool>("blanked", value: false);
					}
					else
					{
						component.SetAttr<InvCharmBackboard, bool>("blanked", value: true);
						component.gotCharmString = "gotCharm_" + l;
						component.newCharmString = "newCharm_" + l;
					}
					obj2.SetActive(activeSelf);
				}
			}
			GameObject val5 = val.Find("Collected Charms");
			for (int m = num + 1; m <= num2; m++)
			{
				int num5 = (m - 1) % 20 + 21;
				if ((Object)(object)val5.Find(num5.ToString()) != (Object)null && (Object)(object)val5.Find(m.ToString()) == (Object)null)
				{
					GameObject val6 = val5.Find(num5.ToString());
					bool activeSelf2 = val6.activeSelf;
					val6.gameObject.SetActive(true);
					GameObject val7 = Object.Instantiate<GameObject>(val6.gameObject, val5.transform, true);
					val7.SetActive(false);
					AddToCharmFadeGroup(val7.Find("Sprite"), ((Component)val5.transform.parent).gameObject);
					AddToCharmFadeGroup(val7.Find("Glow"), ((Component)val5.transform.parent).gameObject);
					val6.gameObject.SetActive(activeSelf2);
					((Object)val7).name = m.ToString();
					FsmVariables fsmVariables = FSMUtility.LocateMyFSM(val7, "charm_show_if_collected").FsmVariables;
					fsmVariables.GetFsmInt("ID").Value = m;
					fsmVariables.GetFsmString("Name").Value = $"Charm {m}";
					fsmVariables.GetFsmString("PD Bool Name").Value = "gotCharm_" + m;
					fsmVariables.GetFsmGameObject("Charm Sprite").Value = val7.Find("Sprite");
					fsmVariables.GetFsmGameObject("Glow").Value = val7.Find("Glow");
					val7.SetActive(val6.gameObject.activeSelf);
				}
				else if ((Object)(object)val5.Find(m.ToString()) != (Object)null)
				{
					GameObject val7 = val5.Find(m.ToString());
					bool activeSelf3 = val7.activeSelf;
					val7.SetActive(false);
					AddToCharmFadeGroup(val7.Find("Sprite"), ((Component)val5.transform.parent).gameObject);
					AddToCharmFadeGroup(val7.Find("Glow"), ((Component)val5.transform.parent).gameObject);
					((Object)val7).name = m.ToString();
					FsmVariables fsmVariables2 = FSMUtility.LocateMyFSM(val7, "charm_show_if_collected").FsmVariables;
					fsmVariables2.GetFsmInt("ID").Value = m;
					fsmVariables2.GetFsmString("Name").Value = $"Charm {m}";
					fsmVariables2.GetFsmString("PD Bool Name").Value = "gotCharm_" + m;
					fsmVariables2.GetFsmGameObject("Charm Sprite").Value = val7.Find("Sprite");
					fsmVariables2.GetFsmGameObject("Glow").Value = val7.Find("Glow");
					val7.SetActive(activeSelf3);
				}
			}
			Mathf.Ceil((float)num / 10f / (float)num3);
			float num6 = 4f / (float)num3;
			float num7 = Mathf.Pow(num6, 0.5f);
			for (int n = 0; n <= num3; n++)
			{
				for (int num8 = 1; num8 <= 10; num8++)
				{
					GameObject val8 = val2.Find("BB " + (10 * n + num8));
					if ((Object)(object)val8 != (Object)null)
					{
						Vector3 localPosition = val8.transform.localPosition;
						Vector3 localScale = val8.transform.localScale;
						float num9 = (localPosition.y - -8.37f) * num6 + -8.37f;
						val8.transform.localPosition = new Vector3(localPosition.x, num9, localPosition.z);
						val8.transform.localScale = new Vector3(num7, num7, localScale.z);
						if (10 * n + num8 > num2)
						{
							val8.GetComponent<SpriteRenderer>().color = new Color(0.45490196f, 0.45490196f, 0.45490196f, 0f);
						}
						else
						{
							val8.GetComponent<SpriteRenderer>().color = new Color(0.45490196f, 0.45490196f, 0.45490196f, 1f);
						}
					}
					GameObject val9 = val5.Find((10 * n + num8).ToString());
					if ((Object)(object)val9 != (Object)null)
					{
						Vector3 localPosition2 = val9.transform.localPosition;
						Vector3 localScale2 = val9.transform.localScale;
						if (n <= 3)
						{
							float num10 = (localPosition2.y - -8.34f) * num6 + -8.34f;
							val9.transform.localPosition = new Vector3(localPosition2.x, num10, localPosition2.z);
							val9.transform.localScale = new Vector3(num7, num7, localScale2.z);
						}
						else
						{
							float num11 = ((n % 2 == 0) ? (-7.92f) : (-7.01f));
							float num12 = -8.37f - (float)n * 1.42f * num6;
							val9.transform.localPosition = new Vector3(num11 + (float)(num8 - 1) * 1.5f, num12, localPosition2.z);
							val9.transform.localScale = new Vector3(num7, num7, localScale2.z);
						}
					}
				}
			}
			LogFine("~InitUiCharms");
		}

		private static void OnPlayerDataCalculateNotchesUsed(orig_CalculateNotchesUsed orig, PlayerData self)
		{
			LogFine("!OnPlayerDataCalculateNotchesUsed");
			orig.Invoke(self);
			int num = 0;
			for (int i = 0; i < CustomSprites.Count; i++)
			{
				if (self.GetBool("equippedCharm_" + (41 + i)))
				{
					num += self.GetInt("charmCost_" + (41 + i));
				}
			}
			self.SetInt("charmSlotsFilled", self.GetInt("charmSlotsFilled") + num);
			LogFine("~OnPlayerDataCalculateNotchesUsed");
		}

		private static void InitBuildEquippedCharms(BuildEquippedCharms self)
		{
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			LogFine("!InitBuildEquippedCharms");
			List<GameObject> list = new List<GameObject>();
			int num = 40 + CustomSprites.Count;
			for (int i = 0; i < num; i++)
			{
				if (i < self.gameObjectList.Count)
				{
					list.Add(self.gameObjectList[i]);
					continue;
				}
				GameObject val = Object.Instantiate<GameObject>(list[0]);
				SetInactive((Object)(object)val);
				val.transform.position = new Vector3(-20000f, -20000f, -20000f);
				val.SetActive(list[0].activeSelf);
				((Object)val).name = $"{i + 1}";
				val.GetComponent<CharmItem>();
				val.GetComponent<CharmDisplay>().id = i + 1;
				list.Add(val);
			}
			self.gameObjectList = list;
			LogFine("~InitBuildEquippedCharms");
		}

		private static void OnBuildEquippedCharmsStart_single()
		{
			LogFine("!OnBuildEquippedCharmsStart_single");
			InitUiCharms();
			LogFine("~OnBuildEquippedCharmsStart_single");
		}

		private static void GameCamerasStart_single(Action<GameCameras> orig, GameCameras self)
		{
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Expected O, but got Unknown
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Expected O, but got Unknown
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Expected O, but got Unknown
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Expected O, but got Unknown
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Expected O, but got Unknown
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Expected O, but got Unknown
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Expected O, but got Unknown
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Expected O, but got Unknown
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Expected O, but got Unknown
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Expected O, but got Unknown
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Expected O, but got Unknown
			LogFine("!GameCamerasStart_single");
			orig(self);
			GameObject val = ((Component)self).gameObject.Find("HudCamera").Find("Inventory").Find("Charms")
				.Find("Details")
				.Find("Cost");
			PlayMakerFSM val2 = FSMUtility.LocateMyFSM(val, "Charm Details Cost");
			val2.Preprocess();
			for (int i = 7; i <= 20; i++)
			{
				string text = $"Cost {i - 1}";
				string text2 = $"Cost {i}";
				string name = $"C{i}";
				string name2 = $"{i} X";
				GameObject obj = Object.Instantiate<GameObject>(val.Find(text));
				((Object)obj).name = text2;
				obj.transform.localPosition = new Vector3(11.5f + (float)i * 0.75f, -3.54f, -5.63f);
				obj.transform.SetParent(val.transform, false);
				FsmGameObject val3 = val2.AddFsmGameObjectVariable(name);
				FindChild val4 = new FindChild();
				val4.gameObject = val2.GetAction<FindChild>("Init", 0).gameObject;
				val4.childName = FsmString.op_Implicit(text2);
				val4.storeResult = val3;
				val2.InsertAction("Init", (FsmStateAction)(object)val4, i - 1);
				FsmFloat val5 = val2.AddFsmFloatVariable(name2);
				val5.Value = 1.8399999f - 1.8399999f * (((float)i - 1f) / 5f) - 1.02f;
				IntSwitch action = val2.GetAction<IntSwitch>("Check", 1);
				action.compareTo = new List<FsmInt>(action.compareTo) { FsmInt.op_Implicit(i) }.ToArray();
				action.sendEvent = new List<FsmEvent>(action.sendEvent) { FsmEvent.GetFsmEvent($"{i}") }.ToArray();
				val2.CopyState(text, text2);
				val2.AddTransition("Check", $"{i}", text2);
				val2.GetAction<SetPosition>(text2, 0).x = val5;
				FsmFloat y = val2.FindFsmFloatVariable("Absent Y");
				FsmFloat y2 = val2.FindFsmFloatVariable("Present Y");
				SetPosition action2 = new SetPosition
				{
					gameObject = new FsmOwnerDefault
					{
						OwnerOption = (OwnerDefaultOption)1,
						GameObject = val3
					},
					vector = new FsmVector3
					{
						Name = "",
						UseVariable = true
					},
					x = new FsmFloat
					{
						Name = "",
						UseVariable = true
					},
					y = y2,
					z = new FsmFloat
					{
						Name = "",
						UseVariable = true
					},
					space = (Space)1,
					everyFrame = false,
					lateUpdate = false
				};
				SetPosition action3 = new SetPosition
				{
					gameObject = new FsmOwnerDefault
					{
						OwnerOption = (OwnerDefaultOption)1,
						GameObject = val3
					},
					vector = new FsmVector3
					{
						Name = "",
						UseVariable = true
					},
					x = new FsmFloat
					{
						Name = "",
						UseVariable = true
					},
					y = y,
					z = new FsmFloat
					{
						Name = "",
						UseVariable = true
					},
					space = (Space)1,
					everyFrame = false,
					lateUpdate = false
				};
				for (int j = 0; j < i; j++)
				{
					string stateName = $"Cost {j}";
					val2.AddAction(stateName, (FsmStateAction)(object)action3);
				}
				val2.AddAction(text2, (FsmStateAction)(object)action2);
			}
			LogFine("~GameCamerasStart_single");
		}

		private static void AddToCharmFadeGroup(GameObject spriteGo, GameObject fgGo)
		{
			LogFine("!AddToCharmFadeGroup");
			SpriteRenderer component = spriteGo.GetComponent<SpriteRenderer>();
			((Renderer)component).sortingLayerID = 629535577;
			FadeGroup component2 = fgGo.GetComponent<FadeGroup>();
			component2.spriteRenderers = new List<SpriteRenderer>(component2.spriteRenderers) { component }.ToArray();
			LogFine("~AddToCharmFadeGroup");
		}

		private static void SetInactive(Object go)
		{
			LogFine("!SetInactive");
			if (go != (Object)null)
			{
				Object.DontDestroyOnLoad(go);
			}
			LogFine("~SetInactive");
		}

		private static void LogFine(string message)
		{
			InternalLogger.LogFine(message, "[SFCore]:[CharmHelper]");
		}

		private static void LogFine(object message)
		{
			LogFine($"{message}");
		}

		private static void LogDebug(string message)
		{
			InternalLogger.LogDebug(message, "[SFCore]:[CharmHelper]");
		}

		private static void LogDebug(object message)
		{
			LogDebug($"{message}");
		}

		private static void Log(string message)
		{
			InternalLogger.Log(message, "[SFCore]:[CharmHelper]");
		}

		private static void Log(object message)
		{
			Log($"{message}");
		}

		private static void LogWarn(string message)
		{
			InternalLogger.LogWarn(message, "[SFCore]:[CharmHelper]");
		}

		private static void LogWarn(object message)
		{
			LogWarn($"{message}");
		}

		private static void LogError(string message)
		{
			InternalLogger.LogError(message, "[SFCore]:[CharmHelper]");
		}

		private static void LogError(object message)
		{
			LogError($"{message}");
		}
	}
	public static class DlcIconHelper
	{
		private static readonly List<Sprite> CustomSprites;

		static DlcIconHelper()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			CustomSprites = new List<Sprite>();
			LogFine("!cctor");
			UIManager.Start += new hook_Start(AddDlcIcons);
			LogFine("~cctor");
		}

		public static void unusedInit()
		{
			LogFine("!unusedInit");
			LogFine("~unusedInit");
		}

		public static void AddDlcIcon(Sprite icon)
		{
			LogFine("!AddDlcIcon");
			CustomSprites.Add(icon);
			LogFine("~AddDlcIcon");
		}

		private static Vector3 GetPositionOffsetForIndex(int index)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			float num = 0.83f;
			float num2 = 0.83f;
			float num3 = 3.1f - 5f * num + (float)(index % 10) * num;
			float num4 = -0.16f + Mathf.Floor((float)index / 10f) * num2;
			return new Vector3(num3, num4);
		}

		private static void AddDlcIcons(orig_Start orig, UIManager self)
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			LogFine("!AddDlcIcons");
			orig.Invoke(self);
			GameObject gameObject = ((Component)((Component)self).transform.Find("UICanvas/MainMenuScreen/TeamCherryLogo/Hidden_Dreams_Logo")).gameObject;
			for (int i = 0; i < CustomSprites.Count; i++)
			{
				GameObject obj = Object.Instantiate<GameObject>(gameObject, gameObject.transform.parent);
				((Object)obj).name = "CUSTOM DLC ICON " + ((Object)CustomSprites[i]).name;
				obj.SetActive(true);
				Transform transform = obj.transform;
				transform.position += GetPositionOffsetForIndex(i + 5);
				obj.GetComponent<SpriteRenderer>().sprite = CustomSprites[i];
			}
			LogFine("~AddDlcIcons");
		}

		private static void AddToCharmFadeGroup(GameObject spriteGo, GameObject fgGo)
		{
			LogFine("!AddToCharmFadeGroup");
			SpriteRenderer component = spriteGo.GetComponent<SpriteRenderer>();
			((Renderer)component).sortingLayerID = 629535577;
			FadeGroup component2 = fgGo.GetComponent<FadeGroup>();
			component2.spriteRenderers = new List<SpriteRenderer>(component2.spriteRenderers) { component }.ToArray();
			LogFine("~AddToCharmFadeGroup");
		}

		private static void SetInactive(Object go)
		{
			LogFine("!SetInactive");
			if (go != (Object)null)
			{
				Object.DontDestroyOnLoad(go);
			}
			LogFine("~SetInactive");
		}

		private static void LogFine(string message)
		{
			InternalLogger.LogFine(message, "[SFCore]:[DlcIconHelper]");
		}

		private static void LogFine(object message)
		{
			LogFine($"{message}");
		}

		private static void LogDebug(string message)
		{
			InternalLogger.LogDebug(message, "[SFCore]:[DlcIconHelper]");
		}

		private static void LogDebug(object message)
		{
			LogDebug($"{message}");
		}

		private static void Log(string message)
		{
			InternalLogger.Log(message, "[SFCore]:[DlcIconHelper]");
		}

		private static void Log(object message)
		{
			Log($"{message}");
		}

		private static void LogWarn(string message)
		{
			InternalLogger.LogWarn(message, "[SFCore]:[DlcIconHelper]");
		}

		private static void LogWarn(object message)
		{
			LogWarn($"{message}");
		}

		private static void LogError(string message)
		{
			InternalLogger.LogError(message, "[SFCore]:[DlcIconHelper]");
		}

		private static void LogError(object message)
		{
			LogError($"{message}");
		}
	}
	public class EasyCharmState
	{
		public bool IsEquipped;

		public bool GotCharm;

		public bool IsNew;
	}
	public abstract class EasyCharm
	{
		private Sprite _sprite;

		public int Id { get; private set; }

		public bool IsEquipped { get; protected set; }

		public bool GotCharm { get; protected set; }

		public bool IsNew { get; protected set; }

		protected abstract Sprite GetSpriteInternal();

		protected abstract string GetName();

		protected abstract string GetDescription();

		protected abstract int GetCharmCost();

		public Sprite GetSprite()
		{
			if ((Object)(object)_sprite == (Object)null)
			{
				_sprite = GetSpriteInternal();
			}
			return _sprite;
		}

		public EasyCharm()
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected O, but got Unknown
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Expected O, but got Unknown
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Expected O, but got Unknown
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Expected O, but got Unknown
			base..ctor();
			Id = CharmHelper.AddSprites(GetSprite())[0];
			ModHooks.LanguageGetHook += new LanguageGetProxy(OnLanguageGetHook);
			ModHooks.GetPlayerBoolHook += new GetBoolProxy(OnGetPlayerBoolHook);
			ModHooks.SetPlayerBoolHook += new SetBoolProxy(OnSetPlayerBoolHook);
			ModHooks.GetPlayerIntHook += new GetIntProxy(OnGetPlayerIntHook);
		}

		private int OnGetPlayerIntHook(string target, int orig)
		{
			if (target == $"charmCost_{Id}")
			{
				return GetCharmCost();
			}
			return orig;
		}

		private bool OnSetPlayerBoolHook(string target, bool orig)
		{
			if (target == $"gotCharm_{Id}")
			{
				GotCharm = orig;
			}
			if (target == $"newCharm_{Id}")
			{
				IsNew = orig;
			}
			if (target == $"equippedCharm_{Id}")
			{
				IsEquipped = orig;
			}
			return orig;
		}

		private bool OnGetPlayerBoolHook(string target, bool orig)
		{
			if (target == $"gotCharm_{Id}")
			{
				return GotCharm;
			}
			if (target == $"newCharm_{Id}")
			{
				return IsNew;
			}
			if (target == $"equippedCharm_{Id}")
			{
				return IsEquipped;
			}
			return orig;
		}

		private string OnLanguageGetHook(string key, string sheetTitle, string orig)
		{
			if (key == $"CHARM_NAME_{Id}")
			{
				return GetName();
			}
			if (key == $"CHARM_DESC_{Id}")
			{
				return GetDescription();
			}
			return orig;
		}

		public void GiveCharm(bool consideredNew = false)
		{
			PlayerData.instance.SetBool($"gotCharm_{Id}", true);
			PlayerData.instance.SetBool($"newCharm_{Id}", consideredNew);
		}

		public void TakeCharm()
		{
			if (PlayerData.instance.GetBool($"gotCharm_{Id}") && PlayerData.instance.GetBool($"equippedCharm_{Id}"))
			{
				int num = PlayerData.instance.GetInt($"charmCost_{Id}");
				int num2 = PlayerData.instance.GetInt("charmSlotsFilled");
				PlayerData.instance.SetInt("charmSlotsFilled", num2 - num);
				if (PlayerData.instance.GetBool("overcharmed") && PlayerData.instance.GetInt("charmSlotsFilled") <= PlayerData.instance.GetInt("charmSlots"))
				{
					PlayerData.instance.SetBool("overcharmed", false);
				}
				PlayerData.instance.SetBool($"equippedCharm_{Id}", false);
				GameManager.instance.UnequipCharm(Id);
				ReflectionHelper.CallMethod<BuildEquippedCharms>(Object.FindObjectOfType<BuildEquippedCharms>(), "BuildCharmList", Array.Empty<object>());
			}
			PlayerData.instance.SetBool($"gotCharm_{Id}", false);
			PlayerData.instance.SetBool($"newCharm_{Id}", false);
			PlayerData.instance.SetBool($"equippedCharm_{Id}", false);
		}

		public EasyCharmState GetCharmState()
		{
			return new EasyCharmState
			{
				GotCharm = GotCharm,
				IsEquipped = IsEquipped,
				IsNew = IsNew
			};
		}

		public void RestoreCharmState(EasyCharmState state)
		{
			GotCharm = state.GotCharm;
			IsEquipped = state.IsEquipped;
			IsNew = state.IsNew;
		}
	}
	public static class EnviromentParticleHelper
	{
		public delegate(int enviromentType, AudioClip walkAudio) CustomWalkAudioHook(HeroController self);

		public delegate(int enviromentType, AudioClip runAudio) CustomRunAudioHook(HeroController self);

		public delegate(int enviromentType, GameObject dashEffects) CustomDashEffectsHook(DashEffect self);

		public delegate(int enviromentType, GameObject hardLandEffects) CustomHardLandEffectsHook(HardLandEffect self);

		public delegate(int enviromentType, GameObject jumpEffects) CustomJumpEffectsHook(JumpEffects self);

		public delegate(int enviromentType, GameObject softLandEffects) CustomSoftLandEffectsHook(SoftLandEffect self);

		public delegate(int enviromentType, GameObject runEffects) CustomRunEffectsHook(GameObject runEffectsGo);

		private static Dictionary<int, AudioClip> _customWalkAudio;

		private static Dictionary<int, AudioClip> _customRunAudio;

		private static Dictionary<int, GameObject> _customDashEffects;

		private static Dictionary<int, GameObject> _customHardLandEffects;

		private static Dictionary<int, GameObject> _customJumpEffects;

		private static Dictionary<int, GameObject> _customSoftLandEffects;

		private static Dictionary<int, GameObject> _customRunEffects;

		private static Dictionary<int, string> _customRunEffectsPrefabs;

		public static CustomWalkAudioHook AddCustomWalkAudioHook;

		public static CustomRunAudioHook AddCustomRunAudioHook;

		public static CustomDashEffectsHook AddCustomDashEffectsHook;

		public static CustomHardLandEffectsHook AddCustomHardLandEffectsHook;

		public static CustomJumpEffectsHook AddCustomJumpEffectsHook;

		public static CustomSoftLandEffectsHook AddCustomSoftLandEffectsHook;

		public static CustomRunEffectsHook AddCustomRunEffectsHook;

		static EnviromentParticleHelper()
		{
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Expected O, but got Unknown
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Expected O, but got Unknown
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Expected O, but got Unknown
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Expected O, but got Unknown
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Expected O, but got Unknown
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Expected O, but got Unknown
			_customWalkAudio = new Dictionary<int, AudioClip>();
			_customRunAudio = new Dictionary<int, AudioClip>();
			_customDashEffects = new Dictionary<int, GameObject>();
			_customHardLandEffects = new Dictionary<int, GameObject>();
			_customJumpEffects = new Dictionary<int, GameObject>();
			_customSoftLandEffects = new Dictionary<int, GameObject>();
			_customRunEffects = new Dictionary<int, GameObject>();
			_customRunEffectsPrefabs = new Dictionary<int, string>();
			HeroController.checkEnvironment += new hook_checkEnvironment(OnHeroControllercheckEnvironment);
			DashEffect.OnEnable += new hook_OnEnable(OnDashEffectOnEnable);
			HardLandEffect.OnEnable += new hook_OnEnable(OnHardLandEffectOnEnable);
			JumpEffects.OnEnable += new hook_OnEnable(OnJumpEffectsOnEnable);
			SoftLandEffect.OnEnable += new hook_OnEnable(OnSoftLandEffectOnEnable);
			EventRegister.Awake += new hook_Awake(OnEventRegisterAwake);
		}

		public static void unusedInit()
		{
		}

		private static void OnHeroControllercheckEnvironment(orig_checkEnvironment orig, HeroController self)
		{
			orig.Invoke(self);
			try
			{
				if (AddCustomWalkAudioHook != null)
				{
					Delegate[] invocationList = AddCustomWalkAudioHook.GetInvocationList();
					foreach (Delegate obj in invocationList)
					{
						if ((object)obj != null)
						{
							var (enviromentType, walkAudio) = ((int, AudioClip))obj.DynamicInvoke(self);
							AddWalkAudio(enviromentType, walkAudio);
						}
					}
				}
				if (_customWalkAudio != null)
				{
					self.footStepsWalkAudioSource.clip = _customWalkAudio[PlayerData.instance.GetInt("environmentType")];
				}
			}
			catch (Exception)
			{
			}
			try
			{
				if (AddCustomRunAudioHook != null)
				{
					Delegate[] invocationList = AddCustomRunAudioHook.GetInvocationList();
					foreach (Delegate obj2 in invocationList)
					{
						if ((object)obj2 != null)
						{
							var (enviromentType2, runAudio) = ((int, AudioClip))obj2.DynamicInvoke(self);
							AddRunAudio(enviromentType2, runAudio);
						}
					}
				}
				if (_customRunAudio != null)
				{
					self.footStepsRunAudioSource.clip = _customRunAudio[PlayerData.instance.GetInt("environmentType")];
				}
			}
			catch (Exception)
			{
			}
		}

		private static void OnDashEffectOnEnable(orig_OnEnable orig, DashEffect self)
		{
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			foreach (KeyValuePair<int, GameObject> customDashEffect in _customDashEffects)
			{
				customDashEffect.Value.SetActive(false);
			}
			try
			{
				if (AddCustomDashEffectsHook != null)
				{
					Delegate[] invocationList = AddCustomDashEffectsHook.GetInvocationList();
					foreach (Delegate obj in invocationList)
					{
						if ((object)obj != null)
						{
							var (enviromentType, dashEffects) = ((int, GameObject))obj.DynamicInvoke(self);
							AddDashEffects(enviromentType, dashEffects);
						}
					}
				}
				GameObject obj2 = _customDashEffects[PlayerData.instance.GetInt("environmentType")];
				self.heroDashPuff.SetActive(false);
				self.dashDust.SetActive(false);
				self.heroDashPuff_anim.Stop();
				obj2.transform.SetParent(((Component)self).transform);
				obj2.transform.localPosition = Vector3.zero;
				obj2.SetActive(true);
			}
			catch (Exception)
			{
			}
		}

		private static void OnHardLandEffectOnEnable(orig_OnEnable orig, HardLandEffect self)
		{
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			foreach (KeyValuePair<int, GameObject> customHardLandEffect in _customHardLandEffects)
			{
				customHardLandEffect.Value.SetActive(false);
			}
			try
			{
				if (AddCustomHardLandEffectsHook != null)
				{
					Delegate[] invocationList = AddCustomHardLandEffectsHook.GetInvocationList();
					foreach (Delegate obj in invocationList)
					{
						if ((object)obj != null)
						{
							var (enviromentType, hardLandEffects) = ((int, GameObject))obj.DynamicInvoke(self);
							AddHardLandEffects(enviromentType, hardLandEffects);
						}
					}
				}
				GameObject obj2 = _customHardLandEffects[PlayerData.instance.GetInt("environmentType")];
				obj2.transform.SetParent(((Component)self).transform);
				obj2.transform.localPosition = Vector3.zero;
				obj2.SetActive(true);
			}
			catch (Exception)
			{
			}
		}

		private static void OnJumpEffectsOnEnable(orig_OnEnable orig, JumpEffects self)
		{
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			foreach (KeyValuePair<int, GameObject> customJumpEffect in _customJumpEffects)
			{
				customJumpEffect.Value.SetActive(false);
			}
			try
			{
				if (AddCustomJumpEffectsHook != null)
				{
					Delegate[] invocationList = AddCustomJumpEffectsHook.GetInvocationList();
					foreach (Delegate obj in invocationList)
					{
						if ((object)obj != null)
						{
							var (enviromentType, jumpEffects) = ((int, GameObject))obj.DynamicInvoke(self);
							AddJumpEffects(enviromentType, jumpEffects);
						}
					}
				}
				GameObject obj2 = _customJumpEffects[PlayerData.instance.GetInt("environmentType")];
				self.dustEffects.SetActive(false);
				obj2.transform.SetParent(((Component)self).transform);
				obj2.transform.localPosition = Vector3.zero;
				obj2.SetActive(true);
			}
			catch (Exception)
			{
			}
		}

		private static void OnSoftLandEffectOnEnable(orig_OnEnable orig, SoftLandEffect self)
		{
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			foreach (KeyValuePair<int, GameObject> customSoftLandEffect in _customSoftLandEffects)
			{
				customSoftLandEffect.Value.SetActive(false);
			}
			try
			{
				if (AddCustomSoftLandEffectsHook != null)
				{
					Delegate[] invocationList = AddCustomSoftLandEffectsHook.GetInvocationList();
					foreach (Delegate obj in invocationList)
					{
						if ((object)obj != null)
						{
							var (enviromentType, softLandEffects) = ((int, GameObject))obj.DynamicInvoke(self);
							AddSoftLandEffects(enviromentType, softLandEffects);
						}
					}
				}
				GameObject obj2 = _customSoftLandEffects[PlayerData.instance.GetInt("environmentType")];
				self.dustEffects.SetActive(false);
				obj2.transform.SetParent(((Component)self).transform);
				obj2.transform.localPosition = Vector3.zero;
				obj2.SetActive(true);
			}
			catch (Exception)
			{
			}
		}

		private static void OnEventRegisterAwake(orig_Awake orig, EventRegister self)
		{
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Expected O, but got Unknown
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Expected O, but got Unknown
			orig.Invoke(self);
			if (!(self.GetAttr<EventRegister, string>("subscribedEvent") == "ENVIRO UPDATE"))
			{
				return;
			}
			PlayMakerFSM val = FSMUtility.LocateMyFSM(((Component)self).gameObject, "Run Effects");
			if ((Object)(object)val == (Object)null)
			{
				return;
			}
			val.GetState("Check Enviro");
			foreach (KeyValuePair<int, string> customRunEffectsPrefab in _customRunEffectsPrefabs)
			{
				int key = customRunEffectsPrefab.Key;
				string text = $"CUSTOM_{key}";
				FsmEvent fsmEvent = FsmEvent.GetFsmEvent(text);
				IntSwitch action = val.GetAction<IntSwitch>("Check Enviro", 5);
				FsmInt item = new FsmInt(text)
				{
					Value = key
				};
				List<FsmInt> list = new List<FsmInt>(action.compareTo) { item };
				action.compareTo = list.ToArray();
				List<FsmEvent> list2 = new List<FsmEvent>(action.sendEvent) { fsmEvent };
				action.sendEvent = list2.ToArray();
				val.AddTransition("Check Enviro", text, _customRunEffectsPrefabs[key]);
			}
			if (AddCustomRunEffectsHook != null)
			{
				Delegate[] invocationList = AddCustomRunEffectsHook.GetInvocationList();
				foreach (Delegate obj in invocationList)
				{
					if ((object)obj != null)
					{
						var (enviromentType, runEffects) = ((int, GameObject))obj.DynamicInvoke(((Component)self).gameObject);
						AddRunEffects(enviromentType, runEffects);
					}
				}
			}
			foreach (KeyValuePair<int, GameObject> customRunEffect in _customRunEffects)
			{
				GameObject obj2 = Object.Instantiate<GameObject>(customRunEffect.Value, ((Component)self).gameObject.transform);
				((Object)obj2).name = $"{customRunEffect.Key}";
				Object.DontDestroyOnLoad((Object)(object)obj2);
				obj2.transform.SetParent(((Component)self).gameObject.transform);
				int key2 = customRunEffect.Key;
				string text2 = $"CUSTOM_{key2}";
				string text3 = $"CUSTOMSTATE_{key2}";
				FsmEvent fsmEvent2 = FsmEvent.GetFsmEvent(text2);
				val.AddGameObjectVariable($"Custom_{key2}");
				val.CopyState("Grass", text3);
				val.GetAction<FindChild>(text3, 0).childName = FsmString.op_Implicit($"{key2}");
				IntSwitch action2 = val.GetAction<IntSwitch>("Check Enviro", 5);
				FsmInt item2 = new FsmInt(text2)
				{
					Value = key2
				};
				List<FsmInt> list3 = new List<FsmInt>(action2.compareTo) { item2 };
				action2.compareTo = list3.ToArray();
				List<FsmEvent> list4 = new List<FsmEvent>(action2.sendEvent) { fsmEvent2 };
				action2.sendEvent = list4.ToArray();
				val.AddTransition("Check Enviro", text2, text3);
			}
		}

		public static void AddWalkAudio(int enviromentType, AudioClip walkAudio)
		{
			if (enviromentType >= 0 && enviromentType < 8)
			{
				throw new ArgumentOutOfRangeException("enviromentType", "Only Integers smaller than 0 and larger than 7 are allowed");
			}
			if (!_customWalkAudio.ContainsKey(enviromentType))
			{
				AudioClip val = Object.Instantiate<AudioClip>(walkAudio);
				SetInactive((Object)(object)val);
				_customWalkAudio.Add(enviromentType, val);
			}
		}

		public static void AddRunAudio(int enviromentType, AudioClip runAudio)
		{
			if (enviromentType >= 0 && enviromentType < 8)
			{
				throw new ArgumentOutOfRangeException("enviromentType", "Only Integers smaller than 0 and larger than 7 are allowed");
			}
			if (!_customRunAudio.ContainsKey(enviromentType))
			{
				AudioClip val = Object.Instantiate<AudioClip>(runAudio);
				SetInactive((Object)(object)val);
				_customRunAudio.Add(enviromentType, val);
			}
		}

		public static void AddDashEffects(int enviromentType, GameObject dashEffects)
		{
			if (enviromentType >= 0 && enviromentType < 7)
			{
				throw new ArgumentOutOfRangeException("enviromentType", "Only Integers smaller than 0 and larger than 6 are allowed");
			}
			if (!_customDashEffects.ContainsKey(enviromentType))
			{
				GameObject val = Object.Instantiate<GameObject>(dashEffects);
				SetInactive(val);
				_customDashEffects.Add(enviromentType, val);
			}
		}

		public static void AddHardLandEffects(int enviromentType, GameObject hardLandEffects)
		{
			if (enviromentType >= 0 && enviromentType < 7)
			{
				throw new ArgumentOutOfRangeException("enviromentType", "Only Integers smaller than 0 and larger than 6 are allowed");
			}
			if (!_customHardLandEffects.ContainsKey(enviromentType))
			{
				GameObject val = Object.Instantiate<GameObject>(hardLandEffects);
				SetInactive(val);
				_customHardLandEffects.Add(enviromentType, val);
			}
		}

		public static void AddJumpEffects(int enviromentType, GameObject jumpEffects)
		{
			if (enviromentType >= 0 && enviromentType < 7)
			{
				throw new ArgumentOutOfRangeException("enviromentType", "Only Integers smaller than 0 and larger than 6 are allowed");
			}
			if (!_customJumpEffects.ContainsKey(enviromentType))
			{
				GameObject val = Object.Instantiate<GameObject>(jumpEffects);
				SetInactive(val);
				_customJumpEffects.Add(enviromentType, val);
			}
		}

		public static void AddSoftLandEffects(int enviromentType, GameObject softLandEffects)
		{
			if (enviromentType >= 0 && enviromentType < 7)
			{
				throw new ArgumentOutOfRangeException("enviromentType", "Only Integers smaller than 0 and larger than 6 are allowed");
			}
			if (!_customSoftLandEffects.ContainsKey(enviromentType))
			{
				GameObject val = Object.Instantiate<GameObject>(softLandEffects);
				SetInactive(val);
				_customSoftLandEffects.Add(enviromentType, val);
			}
		}

		public static void AddRunEffects(int enviromentType, GameObject runEffects)
		{
			if (enviromentType >= 0 && enviromentType < 7)
			{
				throw new ArgumentOutOfRangeException("enviromentType", "Only Integers smaller than 0 and larger than 6 are allowed");
			}
			if (!_customRunEffects.ContainsKey(enviromentType))
			{
				GameObject val = Object.Instantiate<GameObject>(runEffects);
				SetInactive(val);
				val.SetActive(true);
				val.GetComponent<ParticleSystem>().Stop(true, (ParticleSystemStopBehavior)1);
				_customRunEffects.Add(enviromentType, val);
			}
		}

		public static void AddRunEffects(int enviromentType, string runEffectsPrefab)
		{
			if (enviromentType >= 0 && enviromentType < 7)
			{
				throw new ArgumentOutOfRangeException("enviromentType", "Only Integers smaller than 0 and larger than 6 are allowed");
			}
			if (!_customRunEffectsPrefabs.ContainsKey(enviromentType))
			{
				_customRunEffectsPrefabs.Add(enviromentType, runEffectsPrefab);
			}
		}

		private static void SetInactive(Object go)
		{
			if (go != (Object)null)
			{
				Object.DontDestroyOnLoad(go);
			}
		}

		private static void SetInactive(GameObject go)
		{
			if ((Object)(object)go != (Object)null)
			{
				Object.DontDestroyOnLoad((Object)(object)go);
				go.SetActive(false);
			}
		}

		private static void LogFine(string message)
		{
			InternalLogger.LogFine(message, "[SFCore]:[EnviromentParticleHelper]");
		}

		private static void LogFine(object message)
		{
			LogFine($"{message}");
		}

		private static void LogDebug(string message)
		{
			InternalLogger.LogDebug(message, "[SFCore]:[EnviromentParticleHelper]");
		}

		private static void LogDebug(object message)
		{
			LogDebug($"{message}");
		}

		private static void Log(string message)
		{
			InternalLogger.Log(message, "[SFCore]:[EnviromentParticleHelper]");
		}

		private static void Log(object message)
		{
			Log($"{message}");
		}

		private static void LogWarn(string message)
		{
			InternalLogger.LogWarn(message, "[SFCore]:[EnviromentParticleHelper]");
		}

		private static void LogWarn(object message)
		{
			LogWarn($"{message}");
		}

		private static void LogError(string message)
		{
			InternalLogger.LogError(message, "[SFCore]:[EnviromentParticleHelper]");
		}

		private static void LogError(object message)
		{
			LogError($"{message}");
		}
	}
	internal static class InternalLogger
	{
		private static string FormatOriginator(object originator, string message)
		{
			if (originator != null)
			{
				if (originator is string)
				{
					return $"{originator} - {message}";
				}
				return "[" + originator.GetType().FullName?.Replace(".", "]:[") + "] - " + message;
			}
			return message;
		}

		internal static void LogFine(string message, object originator = null)
		{
			Logger.LogFine(FormatOriginator(originator, message));
		}

		internal static void LogDebug(string message, object originator = null)
		{
			string text = FormatOriginator(originator, message);
			Logger.LogDebug(text);
			Debug.Log((object)text);
		}

		internal static void Log(string message, object originator = null)
		{
			string text = FormatOriginator(originator, message);
			Logger.Log(text);
			Debug.Log((object)text);
		}

		internal static void LogWarn(string message, object originator = null)
		{
			string text = FormatOriginator(originator, message);
			Logger.LogWarn(text);
			Debug.LogWarning((object)text);
		}

		internal static void LogError(string message, object originator = null)
		{
			string text = FormatOriginator(originator, message);
			Logger.LogError(text);
			Debug.LogError((object)text);
		}
	}
	public enum InventoryPageType
	{
		Empty,
		Inventory,
		Charms,
		Journal,
		Map
	}
	public static class InventoryHelper
	{
		private struct CustomPaneData
		{
			public InventoryPageType Type = InventoryPageType.Empty;

			public string Name = "";

			public string ConvKey = "";

			public string EventName = "";

			public string IsAvailablePdBool = "";

			public CustomPaneData()
			{
			}

			public void Deconstruct(out InventoryPageType type, out string name, out string convKey, out string eventName, out string isAvailablePdBool)
			{
				InventoryPageType type2 = Type;
				string name2 = Name;
				string convKey2 = ConvKey;
				string eventName2 = EventName;
				string isAvailablePdBool2 = IsAvailablePdBool;
				type = type2;
				name = name2;
				convKey = convKey2;
				eventName = eventName2;
				isAvailablePdBool = isAvailablePdBool2;
			}
		}

		private static Dictionary<string, CustomPaneData> _customPaneData;

		private static Dictionary<string, Action<GameObject>> _customPaneDataCallbacks;

		static InventoryHelper()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			_customPaneData = new Dictionary<string, CustomPaneData>();
			_customPaneDataCallbacks = new Dictionary<string, Action<GameObject>>();
			GameCameras.Start += new hook_Start(GameCamerasOnStart);
		}

		public static void unusedInit()
		{
		}

		public static void AddInventoryPage(InventoryPageType type, string name, string convKey, string eventName, string isAvailablePdBool, Action<GameObject> callback)
		{
			_customPaneData[eventName] = new CustomPaneData
			{
				Type = type,
				Name = name,
				ConvKey = convKey,
				EventName = eventName,
				IsAvailablePdBool = isAvailablePdBool
			};
			_customPaneDataCallbacks[eventName] = callback;
		}

		private static void GameCamerasOnStart(orig_Start orig, GameCameras self)
		{
			orig.Invoke(self);
			foreach (CustomPaneData value in _customPaneData.Values)
			{
				CustomPaneData customPaneData = value;
				var (_, _, _, key, _) = customPaneData;
				Action<GameObject> action = _customPaneDataCallbacks[key];
				GameObject obj = DoCopyPane(self, value);
				action(obj);
			}
		}

		private static GameObject DoCopyPane(GameCameras cameras, CustomPaneData cpd)
		{
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Expected O, but got Unknown
			//IL_0b51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b99: Expected O, but got Unknown
			//IL_0ba0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bdf: Expected O, but got Unknown
			//IL_0be6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0beb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c10: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2f: Expected O, but got Unknown
			//IL_0c36: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c5b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c62: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6f: Expected O, but got Unknown
			//IL_0c76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0caf: Expected O, but got Unknown
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Expected O, but got Unknown
			CustomPaneData customPaneData = cpd;
			var (inventoryPageType2, text5, text6, text7, text8) = customPaneData;
			string name;
			switch (inventoryPageType2)
			{
			case InventoryPageType.Empty:
				name = "Inv";
				break;
			case InventoryPageType.Inventory:
				name = "Inv";
				break;
			case InventoryPageType.Charms:
				name = "Charms";
				break;
			case InventoryPageType.Journal:
				name = "Journal";
				break;
			case InventoryPageType.Map:
				name = "Map";
				break;
			default:
				return null;
			}
			GameObject val = ((Component)cameras).gameObject.Find("Inventory");
			PlayMakerFSM val2 = FSMUtility.LocateMyFSM(val, "Inventory Control");
			FsmVariables fsmVariables = val2.FsmVariables;
			if (val2.FsmStates[0].Fsm == null)
			{
				val2.Preprocess();
			}
			GameObject val3 = Object.Instantiate<GameObject>(val.FindGameObjectInChildren(name), val.transform);
			val3.SetActive(false);
			((Object)val3).name = text5;
			FsmOwnerDefault gameObject = new FsmOwnerDefault
			{
				GameObject = FsmGameObject.op_Implicit(val3),
				OwnerOption = (OwnerDefaultOption)1
			};
			switch (inventoryPageType2)
			{
			case InventoryPageType.Empty:
				name = "Inv";
				break;
			case InventoryPageType.Inventory:
				name = "Inv";
				break;
			case InventoryPageType.Charms:
				name = "Charms";
				break;
			case InventoryPageType.Journal:
				name = "Journal";
				break;
			case InventoryPageType.Map:
				name = "Map";
				break;
			default:
				return null;
			}
			switch (inventoryPageType2)
			{
			case InventoryPageType.Empty:
			{
				PlayMakerFSM val6 = FSMUtility.LocateMyFSM(val3, "UI Inventory");
				if (val6.FsmStates[0].Fsm == null)
				{
					val6.Preprocess();
				}
				for (int i = 0; i < 16; i++)
				{
					val6.RemoveAction("Init", 5);
				}
				for (int j = 0; j < 4; j++)
				{
					val6.RemoveAction("Init", 6);
				}
				for (int k = 0; k < 3; k++)
				{
					val6.RemoveAction("Init", 8);
				}
				for (int l = 0; l < 4; l++)
				{
					val6.RemoveAction("Inactive", 1);
				}
				val6.RemoveAction("Activate", 2);
				val6.GetAction<GetPosition>("Init Heart Piece", 0).gameObject = new FsmOwnerDefault
				{
					GameObject = val6.GetGameObjectVariable("Arrow L"),
					OwnerOption = (OwnerDefaultOption)1
				};
				for (int m = 0; m < 4; m++)
				{
					val6.RemoveAction("L Arrow", 3);
				}
				for (int n = 0; n < 4; n++)
				{
					val6.RemoveAction("R Arrow", 3);
				}
				val6.ChangeTransition("Inactive", "ACTIVATE", "Activate");
				val6.ChangeTransition("Init Heart Piece", "FINISHED", "L Arrow");
				val6.ChangeTransition("L Arrow", "UI RIGHT", "R Arrow");
				val6.ChangeTransition("R Arrow", "UI LEFT", "L Arrow");
				List<string> list = new List<string>
				{
					"Completion Rate", "Any Other Panes?", "Set Dream Nail Convo", "DN Normal", "DN Upgraded", "Art Backboard", "Set Heart Piece Convo", "HP None", "HP Some", "HP All",
					"Set Vessel Convo", "Vessel None", "Vessel 0", "Vessel 1", "Vessel 2", "Vessel All", "Set Nail Convo", "Nail 1", "Nail 2", "Nail 3",
					"Nail 4", "Nail 5", "Godfinder", "Godfinder?", "Godfinder? 2", "Soul Orb", "Dream Nail", "Dream Gate", "Quake", "Fireball",
					"Focus", "Nail", "Scream", "Cyclone", "Dash Slash", "Uppercut", "Geo", "To Equip", "To Equip 2", "Bottom Choice",
					"Bot to Inv", "R to bot?", "From R Bot Choice", "B Choice 1", "B Choice 2", "B Choice 3", "B Choice 4", "Trinket 1", "Trinket 2", "Trinket 3",
					"Trinket 4", "Equip Item 1", "Equip Item 2", "Equip Item 3", "Equip Item 4", "Equip Item 5", "Equip Item 6", "Equip Item 7", "Equip Item 8", "Equip Item 9",
					"Equip Item 10", "Equip Item 11", "Equip Item 12", "Equip Item 13", "Equip Item 14", "Equip Item 15", "Equip Item 16", "Choice 1", "Choice 2", "Choice 3",
					"Choice 4", "Choice 5", "Choice 6", "Choice 7", "Choice 8", "Choice 9", "Choice 10", "Choice 11", "Choice 12", "Choice 13",
					"Choice 14", "Choice 15", "Choice 16", "Choice 17", "Choice 18", "Choice 19", "Choice 20", "Choice 21", "Choice 22", "Choice 23",
					"Choice 24", "Choice 25", "Choice 26", "Choice 27", "Choice 28", "Choice 29", "Choice 30", "Choice 31", "Choice 32", "Choice 33",
					"Choice 34", "Choice 35", "Choice 36", "Choice 37", "Other Panes?", "Other Panes? R", "Heart Pieces"
				};
				for (int num = 0; num < list.Count; num++)
				{
					val6.RemoveState(list[num]);
				}
				FsmVariables fsmVariables2 = val6.FsmVariables;
				fsmVariables2.FloatVariables = Array.Empty<FsmFloat>();
				fsmVariables2.IntVariables = Array.Empty<FsmInt>();
				List<FsmBool> list2 = new List<FsmBool>();
				FsmBool[] boolVariables = fsmVariables2.BoolVariables;
				foreach (FsmBool val7 in boolVariables)
				{
					if (((NamedVariable)val7).Name == "Repeating")
					{
						list2.Add(val7);
					}
				}
				fsmVariables2.BoolVariables = list2.ToArray();
				fsmVariables2.StringVariables = Array.Empty<FsmString>();
				List<FsmGameObject> list3 = new List<FsmGameObject>();
				List<string> list4 = new List<string> { "Arrow L", "Arrow R", "Border", "Cursor", "Cursor Back", "Cursor Glow", "Parent", "Self" };
				FsmGameObject[] gameObjectVariables = fsmVariables2.GameObjectVariables;
				foreach (FsmGameObject val8 in gameObjectVariables)
				{
					if (list4.Contains(((NamedVariable)val8).Name))
					{
						list3.Add(val8);
					}
				}
				fsmVariables2.GameObjectVariables = list3.ToArray();
				List<string> list5 = new List<string>
				{
					"Inv_Items", "trinket_backboard", "Divider L", "Divider R", "Text Name", "Text Desc", "Equipment", "Text Completion", "Text Desc Low", "Item Control",
					"Godfinder Icons"
				};
				for (int num3 = 0; num3 < list5.Count; num3++)
				{
					Object.DestroyImmediate((Object)(object)val3.Find(list5[num3]));
				}
				Object.DestroyImmediate((Object)(object)FSMUtility.LocateMyFSM(val3, "Button Control"));
				Object.DestroyImmediate((Object)(object)FSMUtility.LocateMyFSM(val3, "Update Text"));
				val6.FsmName = "Empty UI";
				break;
			}
			case InventoryPageType.Inventory:
			{
				PlayMakerFSM val5 = FSMUtility.LocateMyFSM(val3, "UI Inventory");
				if (val5.FsmStates[0].Fsm == null)
				{
					val5.Preprocess();
				}
				val5.FsmName = "UI Inventory Custom";
				break;
			}
			case InventoryPageType.Charms:
			{
				PlayMakerFSM val11 = FSMUtility.LocateMyFSM(val3, "UI Charms");
				if (val11.FsmStates[0].Fsm == null)
				{
					val11.Preprocess();
				}
				val11.FsmName = "UI Charms Custom";
				break;
			}
			case InventoryPageType.Journal:
			{
				PlayMakerFSM val9 = FSMUtility.LocateMyFSM(val3, "UI Journal");
				if (val9.FsmStates[0].Fsm == null)
				{
					val9.Preprocess();
				}
				val9.FsmName = "UI Journal Custom";
				PlayMakerFSM val10 = FSMUtility.LocateMyFSM(val3.Find("Enemy List"), "Item List Control");
				if (val10.FsmStates[0].Fsm == null)
				{
					val10.Preprocess();
				}
				val10.FsmName = "Item List Control Custom";
				break;
			}
			case InventoryPageType.Map:
			{
				PlayMakerFSM val4 = FSMUtility.LocateMyFSM(val3.Find("World Map"), "UI Control");
				if (val4.FsmStates[0].Fsm == null)
				{
					val4.Preprocess();
				}
				val4.FsmName = "UI Control Custom";
				break;
			}
			}
			string text9 = text5 + " Pane";
			string text10 = "Open " + text5;
			string text11 = "Next " + text5 + " 2";
			string text12 = "Next " + text5 + " 3";
			string text13 = "Next " + text5;
			val2.AddGameObjectVariable(text9);
			int num4 = val2.GetAction<IntSwitch>("Check Current Pane", 11).compareTo.Last().Value + 1;
			PlayMakerFSM val12 = FSMUtility.LocateMyFSM(val.FindGameObjectInChildren("Inv"), "UI Inventory");
			if (val12.FsmStates[0].Fsm == null)
			{
				val12.Preprocess();
			}
			val12.InsertAction("Any Other Panes?", (FsmStateAction)new PlayerDataBoolTest
			{
				gameObject = val12.GetAction<PlayerDataBoolTest>("Any Other Panes?", 2).gameObject,
				boolName = FsmString.op_Implicit(text8),
				isTrue = val12.GetAction<PlayerDataBoolTest>("Any Other Panes?", 2).isTrue
			}, 3);
			val2.InsertAction("Init", (FsmStateAction)new FindChild
			{
				gameObject = val2.GetAction<FindChild>("Init", 3).gameObject,
				childName = FsmString.op_Implicit(text5),
				storeResult = fsmVariables.FindFsmGameObject(text9)
			}, 12);
			val2.InsertAction("Single Pane?", (FsmStateAction)new PlayerDataBoolTest
			{
				gameObject = val2.GetAction<PlayerDataBoolTest>("Single Pane?", 8).gameObject,
				boolName = FsmString.op_Implicit(text8),
				isTrue = val2.GetAction<PlayerDataBoolTest>("Single Pane?", 8).isTrue
			}, 9);
			val2.InsertAction("Regain Control", (FsmStateAction)new ActivateGameObject
			{
				gameObject = gameObject,
				activate = FsmBool.op_Implicit(false),
				recursive = FsmBool.op_Implicit(false),
				resetOnExit = false,
				everyFrame = false
			}, 5);
			val2.InsertAction("Regain Control 2", (FsmStateAction)new ActivateGameObject
			{
				gameObject = gameObject,
				activate = FsmBool.op_Implicit(false),
				recursive = FsmBool.op_Implicit(false),
				resetOnExit = false,
				everyFrame = false
			}, 4);
			List<FsmInt> list6 = new List<FsmInt>(val2.GetAction<IntSwitch>("Check Current Pane", 11).compareTo) { FsmInt.op_Implicit(num4) };
			val2.GetAction<IntSwitch>("Check Current Pane", 11).compareTo = list6.ToArray();
			List<FsmEvent> list7 = new List<FsmEvent>(val2.GetAction<IntSwitch>("Check Current Pane", 11).sendEvent) { FsmEvent.GetFsmEvent(text7) };
			val2.GetAction<IntSwitch>("Check Current Pane", 11).sendEvent = list7.ToArray();
			val2.CopyState("Open Journal", text10);
			val2.GetAction<GetLanguageString>(text10, 0).convName = FsmString.op_Implicit(text6);
			val2.GetAction<SetIntValue>(text10, 2).intValue = FsmInt.op_Implicit(num4);
			val2.GetAction<SetIntValue>(text10, 3).intValue = FsmInt.op_Implicit(num4);
			val2.GetAction<SetGameObject>(text10, 4).gameObject = fsmVariables.FindFsmGameObject(text9);
			val2.AddTransition("Check Current Pane", text7, text10);
			List<FsmInt> list8 = new List<FsmInt>(val2.GetAction<IntSwitch>("Check R Pane", 1).compareTo) { FsmInt.op_Implicit(num4) };
			list8[5] = FsmInt.op_Implicit(num4 + 1);
			val2.GetAction<IntSwitch>("Check R Pane", 1).compareTo = list8.ToArray();
			List<FsmEvent> list9 = new List<FsmEvent>(val2.GetAction<IntSwitch>("Check R Pane", 1).sendEvent) { FsmEvent.FindEvent(text7) };
			val2.GetAction<IntSwitch>("Check R Pane", 1).sendEvent = list9.ToArray();
			val2.CopyState("Next Journal 2", text11);
			val2.GetAction<PlayerDataBoolTest>(text11, 0).boolName = FsmString.op_Implicit(text8);
			val2.GetAction<GetLanguageString>(text11, 1).convName = FsmString.op_Implicit(text6);
			val2.AddTransition("Check R Pane", text7, text11);
			val2.GetAction<SetIntValue>("Under 2", 0).intValue = FsmInt.op_Implicit(num4 + 1);
			List<FsmInt> list10 = new List<FsmInt>(val2.GetAction<IntSwitch>("Check L Pane", 1).compareTo) { FsmInt.op_Implicit(num4) };
			list10[5] = FsmInt.op_Implicit(num4 + 1);
			val2.GetAction<IntSwitch>("Check L Pane", 1).compareTo = list10.ToArray();
			List<FsmEvent> list11 = new List<FsmEvent>(val2.GetAction<IntSwitch>("Check L Pane", 1).sendEvent) { FsmEvent.FindEvent(text7) };
			val2.GetAction<IntSwitch>("Check L Pane", 1).sendEvent = list11.ToArray();
			val2.CopyState("Next Journal 3", text12);
			val2.GetAction<PlayerDataBoolTest>(text12, 0).boolName = FsmString.op_Implicit(text8);
			val2.GetAction<GetLanguageString>(text12, 1).convName = FsmString.op_Implicit(text6);
			val2.AddTransition("Check L Pane", text7, text12);
			val2.GetAction<SetIntValue>("Under 3", 0).intValue = FsmInt.op_Implicit(num4 + 1);
			List<FsmInt> list12 = new List<FsmInt>(val2.GetAction<IntSwitch>("Loop Through", 3).compareTo) { FsmInt.op_Implicit(num4) };
			list12[5] = FsmInt.op_Implicit(num4 + 1);
			val2.GetAction<IntSwitch>("Loop Through", 3).compareTo = list12.ToArray();
			List<FsmEvent> list13 = new List<FsmEvent>(val2.GetAction<IntSwitch>("Loop Through", 3).sendEvent) { FsmEvent.FindEvent(text7) };
			val2.GetAction<IntSwitch>("Loop Through", 3).sendEvent = list13.ToArray();
			val2.CopyState("Next Journal", text13);
			val2.GetAction<PlayerDataBoolTest>(text13, 0).boolName = FsmString.op_Implicit(text8);
			val2.GetAction<SetGameObject>(text13, 2).gameObject = fsmVariables.FindFsmGameObject(text9);
			val2.GetAction<GetLanguageString>(text13, 3).convName = FsmString.op_Implicit(text6);
			val2.AddTransition("Loop Through", text7, text13);
			val2.GetAction<SetIntValue>("Under", 0).intValue = FsmInt.op_Implicit(num4 + 1);
			return val3;
		}

		private static void LogFine(string message)
		{
			InternalLogger.LogFine(message, "[SFCore]:[InventoryHelper]");
		}

		private static void LogFine(object message)
		{
			LogFine($"{message}");
		}

		private static void LogDebug(string message)
		{
			InternalLogger.LogDebug(message, "[SFCore]:[InventoryHelper]");
		}

		private static void LogDebug(object message)
		{
			LogDebug($"{message}");
		}

		private static void Log(string message)
		{
			InternalLogger.Log(message, "[SFCore]:[InventoryHelper]");
		}

		private static void Log(object message)
		{
			Log($"{message}");
		}

		private static void LogWarn(string message)
		{
			InternalLogger.LogWarn(message, "[SFCore]:[InventoryHelper]");
		}

		private static void LogWarn(object message)
		{
			LogWarn($"{message}");
		}

		private static void LogError(string message)
		{
			InternalLogger.LogError(message, "[SFCore]:[InventoryHelper]");
		}

		private static void LogError(object message)
		{
			LogError($"{message}");
		}
	}
	public enum ItemType
	{
		Normal,
		OneTwo,
		OneTwoBoth,
		Counted,
		Flower
	}
	public static class ItemHelper
	{
		public struct Item
		{
			public ItemType type;

			public string uniqueName;

			public Sprite sprite1;

			public Sprite sprite2;

			public Sprite spriteBoth;

			public string playerdataBool1;

			public string playerdataBool2;

			public string playerdataInt;

			public string nameConvo1;

			public string nameConvo2;

			public string nameConvoBoth;

			public string descConvo1;

			public string descConvo2;

			public string descConvoBoth;
		}

		private static Dictionary<string, Sprite> _defaultSprites;

		private static List<Item> _defaultItemList;

		private static List<Item> _customItemList;

		private static LanguageStrings _langStrings;

		private static bool _initialized;

		static ItemHelper()
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Expected O, but got Unknown
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Expected O, but got Unknown
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Expected O, but got Unknown
			_defaultSprites = new Dictionary<string, Sprite>();
			_defaultItemList = new List<Item>();
			_customItemList = new List<Item>();
			_initialized = false;
			_defaultItemList = new List<Item>();
			_customItemList = new List<Item>();
			_langStrings = new LanguageStrings(Assembly.GetExecutingAssembly(), "SFCore.Resources.Language.json", Encoding.UTF8);
			ModHooks.LanguageGetHook += new LanguageGetProxy(LanguageGetHook);
			ModHooks.GetPlayerBoolHook += new GetBoolProxy(GetPlayerBoolHook);
			ModHooks.GetPlayerIntHook += new GetIntProxy(GetPlayerIntHook);
			InventoryHelper.AddInventoryPage(InventoryPageType.Journal, "ItemList", "PANE_EQUIPMENT", "EQUIPMENT", "hasCustomInventoryItem", CreateEquipmentPane);
		}

		public static void unusedInit()
		{
		}

		private static bool GetPlayerBoolHook(string originalset, bool orig)
		{
			if (originalset.Equals("hasCustomInventoryItem"))
			{
				if ((Object)(object)CustomItemList.Instance != (Object)null)
				{
					return CustomItemList.Instance.hasAtLeastOneItem();
				}
				return false;
			}
			return orig;
		}

		private static int GetPlayerIntHook(string originalset, int orig)
		{
			if (originalset.Equals("0Return"))
			{
				return 0;
			}
			if (originalset.Equals("customItemListGotAmount"))
			{
				return CustomItemList.Instance.GotItemAmount();
			}
			if (originalset.Equals("customItemListTotalAmount"))
			{
				return CustomItemList.Instance.TotalItemAmount();
			}
			return orig;
		}

		private static string LanguageGetHook(string key, string sheet, string orig)
		{
			if (_langStrings.ContainsKey(key, sheet))
			{
				return _langStrings.Get(key, sheet);
			}
			return orig;
		}

		private static void InitDefaultItems(GameObject equipmentGo)
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Expected O, but got Unknown
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Expected O, but got Unknown
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Expected O, but got Unknown
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Expected O, but got Unknown
			PlayMakerFSM fsmPreprocessed = equipmentGo.GetFsmPreprocessed("Build Equipment List");
			if (!_initialized)
			{
				_defaultSprites["Dash"] = Object.Instantiate<Sprite>(equipmentGo.FindGameObjectInChildren("Dash Cloak").GetComponent<SpriteRenderer>().sprite);
				_defaultSprites["ShadowDash"] = (Sprite)Object.Instantiate(fsmPreprocessed.GetAction<SetSpriteRendererSprite>("Dash", 16).sprite.Value);
				_defaultSprites["Walljump"] = Object.Instantiate<Sprite>(equipmentGo.FindGameObjectInChildren("Mantis Claw").GetComponent<SpriteRenderer>().sprite);
				_defaultSprites["Super Dash"] = Object.Instantiate<Sprite>(equipmentGo.FindGameObjectInChildren("Super Dash").GetComponent<SpriteRenderer>().sprite);
				_defaultSprites["Double Jump"] = Object.Instantiate<Sprite>(equipmentGo.FindGameObjectInChildren("Double Jump").GetComponent<SpriteRenderer>().sprite);
				_defaultSprites["Acid Armour"] = Object.Instantiate<Sprite>(equipmentGo.FindGameObjectInChildren("Acid Armour").GetComponent<SpriteRenderer>().sprite);
				_defaultSprites["Lantern"] = Object.Instantiate<Sprite>(equipmentGo.FindGameObjectInChildren("Lantern").GetComponent<SpriteRenderer>().sprite);
				_defaultSprites["Map"] = (Sprite)Object.Instantiate(fsmPreprocessed.GetAction<SetSpriteRendererSprite>("Map", 1).sprite.Value);
				_defaultSprites["Quill"] = (Sprite)Object.Instantiate(fsmPreprocessed.GetAction<SetSpriteRendererSprite>("Quill", 1).sprite.Value);
				_defaultSprites["MapQuill"] = (Sprite)Object.Instantiate(fsmPreprocessed.GetAction<SetSpriteRendererSprite>("Map and Quill", 1).sprite.Value);
				_defaultSprites["Kings Brand"] = Object.Instantiate<Sprite>(equipmentGo.FindGameObjectInChildren("Kings Brand").GetComponent<SpriteRenderer>().sprite);
				_defaultSprites["Tram Pass"] = Object.Instantiate<Sprite>(equipmentGo.FindGameObjectInChildren("Tram Pass").GetComponent<SpriteRenderer>().sprite);
				_defaultSprites["City Key"] = Object.Instantiate<Sprite>(equipmentGo.FindGameObjectInChildren("City Key").GetComponent<SpriteRenderer>().sprite);
				_defaultSprites["Store Key"] = Object.Instantiate<Sprite>(equipmentGo.FindGameObjectInChildren("Store Key").GetComponent<SpriteRenderer>().sprite);
				_defaultSprites["White Key"] = Object.Instantiate<Sprite>(equipmentGo.FindGameObjectInChildren("White Key").GetComponent<SpriteRenderer>().sprite);
				_defaultSprites["Love Key"] = Object.Instantiate<Sprite>(equipmentGo.FindGameObjectInChildren("Love Key").GetComponent<SpriteRenderer>().sprite);
				_defaultSprites["Flower"] = Object.Instantiate<Sprite>(equipmentGo.FindGameObjectInChildren("Xun Flower").GetComponent<InvItemDisplay>().inactiveSprite);
				_defaultSprites["FlowerBroken"] = Object.Instantiate<Sprite>(equipmentGo.FindGameObjectInChildren("Xun Flower").GetComponent<InvItemDisplay>().activeSprite);
				_defaultSprites["Simple Key"] = Object.Instantiate<Sprite>(equipmentGo.FindGameObjectInChildren("Simple Key").GetComponent<SpriteRenderer>().sprite);
				_defaultSprites["Ore"] = Object.Instantiate<Sprite>(equipmentGo.FindGameObjectInChildren("Ore").GetComponent<SpriteRenderer>().sprite);
				_defaultSprites["Rancid Egg"] = Object.Instantiate<Sprite>(equipmentGo.FindGameObjectInChildren("Rancid Egg").GetComponent<SpriteRenderer>().sprite);
				AddDefaultOneTwoBothItem("Dash", _defaultSprites["Dash"], _defaultSprites["ShadowDash"], _defaultSprites["ShadowDash"], "hasDash", "hasShadowDash", "INV_NAME_DASH", "INV_NAME_SHADOWDASH", "INV_NAME_SHADOWDASH", "INV_DESC_DASH", "INV_DESC_SHADOWDASH", "INV_DESC_SHADOWDASH");
				AddDefaultNormalItem("Walljump", _defaultSprites["Walljump"], "hasWalljump", "INV_NAME_WALLJUMP", "INV_DESC_WALLJUMP");
				AddDefaultNormalItem("Super Dash", _defaultSprites["Super Dash"], "hasSuperDash", "INV_NAME_SUPERDASH", "INV_DESC_SUPERDASH");
				AddDefaultNormalItem("Double Jump", _defaultSprites["Double Jump"], "hasDoubleJump", "INV_NAME_DOUBLEJUMP", "INV_DESC_DOUBLEJUMP");
				AddDefaultNormalItem("Acid Armour", _defaultSprites["Acid Armour"], "hasAcidArmour", "INV_NAME_ACIDARMOUR", "INV_DESC_ACIDARMOUR");
				AddDefaultNormalItem("Lantern", _defaultSprites["Lantern"], "hasLantern", "INV_NAME_LANTERN", "INV_DESC_LANTERN");
				AddDefaultOneTwoBothItem("Map Quill", _defaultSprites["Map"], _defaultSprites["Quill"], _defaultSprites["MapQuill"], "hasMap", "hasQuill", "INV_NAME_MAP", "INV_NAME_QUILL", "INV_NAME_MAPQUILL", "INV_DESC_MAP", "INV_DESC_QUILL", "INV_DESC_MAPQUILL");
				AddDefaultNormalItem("Kings Brand", _defaultSprites["Kings Brand"], "hasKingsBrand", "INV_NAME_KINGSBRAND", "INV_DESC_KINGSBRAND");
				AddDefaultNormalItem("Tram Pass", _defaultSprites["Tram Pass"], "hasTramPass", "INV_NAME_TRAM_PASS", "INV_DESC_TRAM_PASS");
				AddDefaultNormalItem("City Key", _defaultSprites["City Key"], "hasCityKey", "INV_NAME_CITYKEY", "INV_DESC_CITYKEY");
				AddDefaultNormalItem("Store Key", _defaultSprites["Store Key"], "hasSlykey", "INV_NAME_STOREKEY", "INV_DESC_STOREKEY");
				AddDefaultNormalItem("White Key", _defaultSprites["White Key"], "hasWhiteKey", "INV_NAME_WHITEKEY", "INV_DESC_WHITEKEY");
				AddDefaultNormalItem("Love Key", _defaultSprites["Love Key"], "hasLoveKey", "INV_NAME_LOVEKEY", "INV_DESC_LOVEKEY");
				AddDefaultFlowerItem("Xun Flower", _defaultSprites["Flower"], _defaultSprites["FlowerBroken"], "hasXunFlower", "extraFlowerAppear", "xunFlowerBroken", "INV_NAME_FLOWER", "INV_NAME_FLOWER_BROKEN", "INV_DESC_FLOWER", "INV_DESC_FLOWER_BROKEN", "INV_DESC_FLOWER_QG", "INV_DESC_FLOWER_BROKEN_QG");
				AddDefaultCountedItem("Simple Key", _defaultSprites["Simple Key"], "simpleKeys", "INV_NAME_SIMPLEKEY", "INV_DESC_SIMPLEKEY");
				AddDefaultCountedItem("Ore", _defaultSprites["Ore"], "ore", "INV_NAME_ORE", "INV_DESC_ORE");
				AddDefaultCountedItem("Rancid Egg", _defaultSprites["Rancid Egg"], "rancidEggs", "INV_NAME_RANCIDEGG", "INV_DESC_RANCIDEGG");
				_initialized = true;
			}
		}

		private static void CreateEquipmentPane(GameObject newPaneGo)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Expected O, but got Unknown
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Expected O, but got Unknown
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Expected O, but got Unknown
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Expected O, but got Unknown
			//IL_022f: Expected O, but got Unknown
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Expected O, but got Unknown
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Expected O, but got Unknown
			//IL_02bc: Expected O, but got Unknown
			GameObject gameObject = ((Component)newPaneGo.transform.parent).gameObject;
			GameObject gameObject2 = ((Component)gameObject.transform.Find("Inv").Find("Equipment")).gameObject;
			gameObject2.GetFsmPreprocessed("Build Equipment List").ChangeTransition("Init", "FINISHED", "Pause");
			InitDefaultItems(gameObject2);
			PlayMakerFSM fsmPreprocessed = gameObject.GetFsmPreprocessed("Inventory Control");
			FsmVariables fsmVariables = fsmPreprocessed.FsmVariables;
			FsmOwnerDefault gameObject3 = new FsmOwnerDefault
			{
				GameObject = FsmGameObject.op_Implicit(newPaneGo),
				OwnerOption = (OwnerDefaultOption)1
			};
			GameObject val = newPaneGo.FindGameObjectInChildren("Enemy List");
			FsmOwnerDefault gameObject4 = new FsmOwnerDefault
			{
				GameObject = FsmGameObject.op_Implicit(val),
				OwnerOption = (OwnerDefaultOption)1
			};
			Object.Destroy((Object)(object)val.GetComponent<JournalList>());
			CustomItemList customItemList = val.AddComponent<CustomItemList>();
			foreach (Item defaultItem in _defaultItemList)
			{
				customItemList.List.Add(defaultItem);
			}
			foreach (Item customItem in _customItemList)
			{
				customItemList.List.Add(customItem);
			}
			newPaneGo.FindGameObjectInChildren("Enemy Sprite").transform.localScale = new Vector3(3f, 3f, 3f);
			newPaneGo.FindGameObjectInChildren("hunter_symbol").SetActive(false);
			fsmPreprocessed.AddGameObjectVariable("ItemList Pane");
			fsmPreprocessed.AddGameObjectVariable("ItemList List");
			PlayMakerFSM fsmPreprocessed2 = val.GetFsmPreprocessed("Item List Control Custom");
			FsmVariables fsmVariables2 = fsmPreprocessed2.FsmVariables;
			PlayMakerFSM fsmPreprocessed3 = newPaneGo.GetFsmPreprocessed("UI Journal Custom");
			fsmPreprocessed.AddAction("Init Enemy List", (FsmStateAction)new FindChild
			{
				gameObject = gameObject3,
				childName = FsmString.op_Implicit("Enemy List"),
				storeResult = fsmVariables.FindFsmGameObject("ItemList List")
			});
			fsmPreprocessed.AddAction("Init Enemy List", (FsmStateAction)new CallMethodProper
			{
				gameObject = gameObject4,
				behaviour = FsmString.op_Implicit("CustomItemList"),
				methodName = FsmString.op_Implicit("BuildItemList"),
				parameters = (FsmVar[])(object)new FsmVar[0],
				storeResult = new FsmVar()
			});
			fsmPreprocessed.AddAction("Init Enemy List", (FsmStateAction)new ActivateGameObject
			{
				gameObject = gameObject3,
				activate = FsmBool.op_Implicit(false),
				recursive = FsmBool.op_Implicit(false),
				resetOnExit = false,
				everyFrame = false
			});
			fsmPreprocessed.InsertAction("Refresh Enemy List", (FsmStateAction)new CallMethodProper
			{
				gameObject = gameObject4,
				behaviour = FsmString.op_Implicit("CustomItemList"),
				methodName = FsmString.op_Implicit("UpdateItemList"),
				parameters = (FsmVar[])(object)new FsmVar[0],
				storeResult = new FsmVar()
			}, 2);
			fsmPreprocessed2.GetAction<CallMethodProper>("Init", 15).behaviour = FsmString.op_Implicit("CustomItemList");
			fsmPreprocessed2.GetAction<CallMethodProper>("Init", 18).behaviour = FsmString.op_Implicit("CustomItemList");
			fsmPreprocessed2.RemoveAction("Init", 28);
			fsmPreprocessed2.RemoveAction("Init", 27);
			fsmPreprocessed2.RemoveAction("Init", 13);
			fsmPreprocessed2.RemoveAction("Init", 12);
			fsmPreprocessed2.RemoveAction("Init", 11);
			fsmPreprocessed2.GetAction<CallMethodProper>("New Item?", 0).behaviour = FsmString.op_Implicit("CustomItemList");
			fsmPreprocessed2.ChangeTransition("New Item?", "CANCEL", "New Item");
			fsmPreprocessed2.GetAction<CallMethodProper>("Get Details", 1).behaviour = FsmString.op_Implicit("CustomItemList");
			fsmPreprocessed2.GetAction<GetLanguageString>("Get Details", 2).sheetName = FsmString.op_Implicit("UI");
			fsmPreprocessed2.GetAction<CallMethodProper>("Get Details", 4).behaviour = FsmString.op_Implicit("CustomItemList");
			fsmPreprocessed2.GetAction<CallMethodProper>("Get Details", 5).behaviour = FsmString.op_Implicit("CustomItemList");
			fsmPreprocessed2.GetAction<GetLanguageString>("Get Details", 7).sheetName = FsmString.op_Implicit("UI");
			fsmPreprocessed2.RemoveAction("Get Details", 0);
			fsmPreprocessed2.GetAction<CallMethodProper>("Notes?", 0).behaviour = FsmString.op_Implicit("CustomItemList");
			fsmPreprocessed2.RemoveAction("Notes?", 1);
			fsmPreprocessed2.GetAction<SetTextMeshProText>("Get Notes", 5).textString = FsmString.op_Implicit("");
			fsmPreprocessed2.RemoveAction("Get Notes", 4);
			fsmPreprocessed2.RemoveAction("Get Notes", 3);
			fsmPreprocessed2.ChangeTransition("Get Notes", "FINISHED", "MoveTo");
			fsmPreprocessed2.GetAction<BuildString>("Display Kills", 4).stringParts = (FsmString[])(object)new FsmString[2]
			{
				FsmString.op_Implicit("Amount collected: "),
				fsmVariables2.FindFsmString("Kills String")
			};
			fsmPreprocessed2.ChangeTransition("Display Kills", "FINISHED", "MoveTo");
			fsmPreprocessed2.InsertAction("Check Down", (FsmStateAction)(object)fsmPreprocessed2.GetAction<CallMethodProper>("Init", 15), 0);
			fsmPreprocessed2.InsertAction("Check Up", (FsmStateAction)(object)fsmPreprocessed2.GetAction<CallMethodProper>("Init", 15), 0);
			Object.Destroy((Object)(object)newPaneGo.Find("Text Encountered"));
			newPaneGo.Find("Text Completion").GetComponent<SetTextMeshProGameText>().convName = "ITEMS_COLLECTED";
			fsmPreprocessed3.RemoveAction("Init", 6);
			fsmPreprocessed3.RemoveAction("Init", 5);
			fsmPreprocessed3.RemoveAction("Init", 3);
			fsmPreprocessed3.GetAction<GetPlayerDataInt>("Completion?", 2).intName = FsmString.op_Implicit("customItemListGotAmount");
			fsmPreprocessed3.GetAction<GetPlayerDataInt>("Completion?", 5).intName = FsmString.op_Implicit("customItemListGotAmount");
			fsmPreprocessed3.GetAction<GetPlayerDataInt>("Completion?", 8).intName = FsmString.op_Implicit("customItemListTotalAmount");
			fsmPreprocessed3.RemoveAction("Completion?", 12);
			fsmPreprocessed3.RemoveAction("Completion?", 7);
			fsmPreprocessed3.RemoveAction("Completion?", 6);
			fsmPreprocessed3.RemoveAction("Completion?", 5);
			fsmPreprocessed3.RemoveAction("Completion?", 0);
			fsmPreprocessed3.AddTransition("Active", "DOWN", "Inactive");
			fsmPreprocessed3.AddTransition("Active", "CHANGE DOWN", "Inactive");
			FadeGroup component = newPaneGo.GetComponent<FadeGroup>();
			List<SpriteRenderer> list = new List<SpriteRenderer>
			{
				newPaneGo.Find("Arrow D").GetComponent<SpriteRenderer>(),
				newPaneGo.Find("Arrow U").GetComponent<SpriteRenderer>(),
				newPaneGo.Find("divider").GetComponent<SpriteRenderer>(),
				newPaneGo.Find("divider (1)").GetComponent<SpriteRenderer>(),
				newPaneGo.Find("Enemy Sprite").GetComponent<SpriteRenderer>(),
				newPaneGo.Find("hunter_symbol").GetComponent<SpriteRenderer>(),
				newPaneGo.Find("selector").GetComponent<SpriteRenderer>()
			};
			SpriteRenderer[] componentsInChildren = newPaneGo.Find("Cursor").GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer item in componentsInChildren)
			{
				list.Add(item);
			}
			List<TextMeshPro> list2 = new List<TextMeshPro>
			{
				newPaneGo.Find("Text Completion").GetComponent<TextMeshPro>(),
				newPaneGo.Find("Text Completion").Find("Amount").GetComponent<TextMeshPro>(),
				newPaneGo.Find("Text Completion").Find("Text Encountered").Find("Amount")
					.GetComponent<TextMeshPro>(),
				newPaneGo.Find("Text Completion").Find("Text Encountered").Find("Total")
					.GetComponent<TextMeshPro>(),
				newPaneGo.Find("Text Completion").Find("Total").GetComponent<TextMeshPro>(),
				newPaneGo.Find("Text Desc").GetComponent<TextMeshPro>(),
				newPaneGo.Find("Text Name").GetComponent<TextMeshPro>(),
				newPaneGo.Find("Text Notes").GetComponent<TextMeshPro>()
			};
			component.spriteRenderers = list.ToArray();
			component.texts = list2.ToArray();
		}

		private static string GenerateRandomString(int maxLength = 255)
		{
			string text = new string((from s in Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", Random.Range(1, maxLength + 1))
				select s[Random.Range(0, s.Length)]).ToArray());
			while (_customItemList.Select((Item x) => x.uniqueName).Contains(text))
			{
				text = new string((from s in Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", Random.Range(1, maxLength + 1))
					select s[Random.Range(0, s.Length)]).ToArray());
			}
			return text;
		}

		public static void AddNormalItem(Sprite sprite, string playerdataBool, string nameConvo, string descConvo)
		{
			string uniqueName = GenerateRandomString();
			_customItemList.Add(new Item
			{
				type = ItemType.Normal,
				uniqueName = uniqueName,
				sprite1 = sprite,
				playerdataBool1 = playerdataBool,
				nameConvo1 = nameConvo,
				descConvo1 = descConvo
			});
		}

		public static void AddOneTwoItem(Sprite sprite1, Sprite sprite2, string playerdataBool1, string playerdataBool2, string nameConvo1, string nameConvo2, string descConvo1, string descConvo2)
		{
			string uniqueName = GenerateRandomString();
			_customItemList.Add(new Item
			{
				type = ItemType.OneTwo,
				uniqueName = uniqueName,
				sprite1 = sprite1,
				playerdataBool1 = playerdataBool1,
				nameConvo1 = nameConvo1,
				descConvo1 = descConvo1,
				sprite2 = sprite2,
				playerdataBool2 = playerdataBool2,
				nameConvo2 = nameConvo2,
				descConvo2 = descConvo2
			});
		}

		public static void AddOneTwoBothItem(Sprite sprite1, Sprite sprite2, Sprite spriteBoth, string playerdataBool1, string playerdataBool2, string nameConvo1, string nameConvo2, string nameConvoBoth, string descConvo1, string descConvo2, string descConvoBoth)
		{
			string uniqueName = GenerateRandomString();
			_customItemList.Add(new Item
			{
				type = ItemType.OneTwoBoth,
				uniqueName = uniqueName,
				sprite1 = sprite1,
				playerdataBool1 = playerdataBool1,
				nameConvo1 = nameConvo1,
				descConvo1 = descConvo1,
				sprite2 = sprite2,
				playerdataBool2 = playerdataBool2,
				nameConvo2 = nameConvo2,
				descConvo2 = descConvo2,
				spriteBoth = spriteBoth,
				nameConvoBoth = nameConvoBoth,
				descConvoBoth = descConvoBoth
			});
		}

		public static void AddCountedItem(Sprite sprite, string playerdataInt, string nameConvo, string descConvo)
		{
			string uniqueName = GenerateRandomString();
			_customItemList.Add(new Item
			{
				type = ItemType.Counted,
				uniqueName = uniqueName,
				sprite1 = sprite,
				playerdataInt = playerdataInt,
				nameConvo1 = nameConvo,
				descConvo1 = descConvo
			});
		}

		public static void AddFlowerItem(Sprite sprite, Sprite sprite2, string playerdataBool1, string playerdataBool2, string playerdataBool3, string nameConvo1, string nameConvo2, string descConvo1, string descConvo2, string descConvo3, string descConvo4)
		{
			string uniqueName = GenerateRandomString();
			_customItemList.Add(new Item
			{
				type = ItemType.Flower,
				uniqueName = uniqueName,
				sprite1 = sprite,
				sprite2 = sprite2,
				playerdataBool1 = playerdataBool1,
				playerdataBool2 = playerdataBool2,
				playerdataInt = playerdataBool3,
				nameConvo1 = nameConvo1,
				nameConvo2 = nameConvo2,
				descConvo1 = descConvo1,
				descConvo2 = descConvo2,
				nameConvoBoth = descConvo3,
				descConvoBoth = descConvo4
			});
		}

		private static void AddDefaultNormalItem(string uniqueName, Sprite sprite, string playerdataBool, string nameConvo, string descConvo)
		{
			_defaultItemList.Add(new Item
			{
				type = ItemType.Normal,
				uniqueName = uniqueName,
				sprite1 = sprite,
				playerdataBool1 = playerdataBool,
				nameConvo1 = nameConvo,
				descConvo1 = descConvo
			});
		}

		private static void AddDefaultOneTwoItem(string uniqueName, Sprite sprite1, Sprite sprite2, string playerdataBool1, string playerdataBool2, string nameConvo1, string nameConvo2, string descConvo1, string descConvo2)
		{
			_defaultItemList.Add(new Item
			{
				type = ItemType.OneTwo,
				uniqueName = uniqueName,
				sprite1 = sprite1,
				playerdataBool1 = playerdataBool1,
				nameConvo1 = nameConvo1,
				descConvo1 = descConvo1,
				sprite2 = sprite2,
				playerdataBool2 = playerdataBool2,
				nameConvo2 = nameConvo2,
				descConvo2 = descConvo2
			});
		}

		private static void AddDefaultOneTwoBothItem(string uniqueName, Sprite sprite1, Sprite sprite2, Sprite spriteBoth, string playerdataBool1, string playerdataBool2, string nameConvo1, string nameConvo2, string nameConvoBoth, string descConvo1, string descConvo2, string descConvoBoth)
		{
			_defaultItemList.Add(new Item
			{
				type = ItemType.OneTwoBoth,
				uniqueName = uniqueName,
				sprite1 = sprite1,
				playerdataBool1 = playerdataBool1,
				nameConvo1 = nameConvo1,
				descConvo1 = descConvo1,
				sprite2 = sprite2,
				playerdataBool2 = playerdataBool2,
				nameConvo2 = nameConvo2,
				descConvo2 = descConvo2,
				spriteBoth = spriteBoth,
				nameConvoBoth = nameConvoBoth,
				descConvoBoth = descConvoBoth
			});
		}

		private static void AddDefaultCountedItem(string uniqueName, Sprite sprite, string playerdataInt, string nameConvo, string descConvo)
		{
			_defaultItemList.Add(new Item
			{
				type = ItemType.Counted,
				uniqueName = uniqueName,
				sprite1 = sprite,
				playerdataInt = playerdataInt,
				nameConvo1 = nameConvo,
				descConvo1 = descConvo
			});
		}

		private static void AddDefaultFlowerItem(string uniqueName, Sprite sprite, Sprite sprite2, string playerdataBool1, string playerdataBool2, string playerdataBool3, string nameConvo1, string nameConvo2, string descConvo1, string descConvo2, string descConvo3, string descConvo4)
		{
			_defaultItemList.Add(new Item
			{
				type = ItemType.Flower,
				uniqueName = uniqueName,
				sprite1 = sprite,
				sprite2 = sprite2,
				playerdataBool1 = playerdataBool1,
				playerdataBool2 = playerdataBool2,
				playerdataInt = playerdataBool3,
				nameConvo1 = nameConvo1,
				nameConvo2 = nameConvo2,
				descConvo1 = descConvo1,
				descConvo2 = descConvo2,
				nameConvoBoth = descConvo3,
				descConvoBoth = descConvo4
			});
		}

		private static void LogFine(string message)
		{
			InternalLogger.LogFine(message, "[SFCore]:[ItemHelper]");
		}

		private static void LogFine(object message)
		{
			LogFine($"{message}");
		}

		private static void LogDebug(string message)
		{
			InternalLogger.LogDebug(message, "[SFCore]:[ItemHelper]");
		}

		private static void LogDebug(object message)
		{
			LogDebug($"{message}");
		}

		private static void Log(string message)
		{
			InternalLogger.Log(message, "[SFCore]:[ItemHelper]");
		}

		private static void Log(object message)
		{
			Log($"{message}");
		}

		private static void LogWarn(string message)
		{
			InternalLogger.LogWarn(message, "[SFCore]:[ItemHelper]");
		}

		private static void LogWarn(object message)
		{
			LogWarn($"{message}");
		}

		private static void LogError(string message)
		{
			InternalLogger.LogError(message, "[SFCore]:[ItemHelper]");
		}

		private static void LogError(object message)
		{
			LogError($"{message}");
		}
	}
	public static class MenuStyleHelper
	{
		public delegate(string languageString, GameObject styleGo, int titleIndex, string unlockKey, string[] achievementKeys, CameraCurves cameraCurves, AudioMixerSnapshot musicSnapshot) Hook(MenuStyles self);

		private static List<(string, GameObject, int, string, string[], CameraCurves, AudioMixerSnapshot)> _queue;

		private static List<Func<MenuStyles, (string, GameObject, int, string, string[], CameraCurves, AudioMixerSnapshot)>> _callbackQueue;

		private static bool _menuStylesStarted;

		public static Hook AddMenuStyleHook;

		static MenuStyleHelper()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Expected O, but got Unknown
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Expected O, but got Unknown
			_queue = new List<(string, GameObject, int, string, string[], CameraCurves, AudioMixerSnapshot)>();
			_callbackQueue = new List<Func<MenuStyles, (string, GameObject, int, string, string[], CameraCurves, AudioMixerSnapshot)>>();
			_menuStylesStarted = false;
			MenuStyles.Awake += new hook_Awake(OnMenuStylesAwake);
			MenuStyles.SetStyle += new hook_SetStyle(OnMenuStylesSetStyle);
			MenuStyles.Start += (hook_Start)delegate(orig_Start orig, MenuStyles self)
			{
				_menuStylesStarted = false;
				orig.Invoke(self);
				if (self.styles.Length > 10)
				{
					int num = 0;
					for (int i = 0; i < self.styles.Length; i++)
					{
						if (self.styles[i].displayName == FullSettingsMod<SFCoreSaveSettings, SFCoreGlobalSettings>.GlobalSettings.SelectedMenuTheme)
						{
							num = i;
						}
					}
					self.SetStyle(num, false, false);
				}
				_menuStylesStarted = true;
			};
			MenuStyles.SetStyle += (hook_SetStyle)delegate(orig_SetStyle orig, MenuStyles self, int index, bool fade, bool save)
			{
				orig.Invoke(self, index, fade, save);
				if (_menuStylesStarted)
				{
					FullSettingsMod<SFCoreSaveSettings, SFCoreGlobalSettings>.GlobalSettings.SelectedMenuTheme = self.styles[index].displayName;
				}
			};
		}

		public static void unusedInit()
		{
		}

		public static void AddMenuStyle(string languageString, GameObject styleGo, int titleIndex = -1, string unlockKey = "", string[] achievementKeys = null, CameraCurves cameraCurves = null, AudioMixerSnapshot musicSnapshot = null)
		{
			_queue.Add((languageString, styleGo, titleIndex, unlockKey, achievementKeys, cameraCurves, musicSnapshot));
			if ((Object)(object)styleGo != (Object)null)
			{
				Object.DontDestroyOnLoad((Object)(object)styleGo);
			}
			if ((Object)(object)musicSnapshot != (Object)null)
			{
				Object.DontDestroyOnLoad((Object)(object)musicSnapshot);
			}
		}

		public static void AddMenuStyle(Func<MenuStyles, (string, GameObject, int, string, string[], CameraCurves, AudioMixerSnapshot)> callback)
		{
			_callbackQueue.Add(callback);
		}

		private static void OnMenuStylesAwake(orig_Awake orig, MenuStyles self)
		{
			orig.Invoke(self);
			AddCustomStyles(self);
		}

		private static void AddCustomStyles(MenuStyles self)
		{
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Expected O, but got Unknown
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Expected O, but got Unknown
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Expected O, but got Unknown
			List<MenuStyle> list = new List<MenuStyle>(self.styles);
			MenuStyle val = list[0];
			foreach (var item22 in _queue)
			{
				string item = item22.Item1;
				GameObject item2 = item22.Item2;
				int item3 = item22.Item3;
				string item4 = item22.Item4;
				string[] item5 = item22.Item5;
				CameraCurves item6 = item22.Item6;
				AudioMixerSnapshot obj = item22.Item7;
				CameraCurves cameraColorCorrection = item6 ?? val.cameraColorCorrection;
				if (obj == null)
				{
					obj = val.musicSnapshot;
				}
				AudioMixerSnapshot musicSnapshot = obj;
				item2.transform.SetParent(((Component)self).transform);
				MenuStyle item7 = new MenuStyle
				{
					enabled = true,
					displayName = item,
					styleObject = item2,
					cameraColorCorrection = cameraColorCorrection,
					musicSnapshot = musicSnapshot,
					titleIndex = item3,
					unlockKey = item4,
					achievementKeys = item5,
					initialAudioVolumes = val.initialAudioVolumes
				};
				list.Add(item7);
			}
			foreach (Func<MenuStyles, (string, GameObject, int, string, string[], CameraCurves, AudioMixerSnapshot)> item23 in _callbackQueue)
			{
				(string, GameObject, int, string, string[], CameraCurves, AudioMixerSnapshot) tuple = item23(self);
				string item8 = tuple.Item1;
				GameObject item9 = tuple.Item2;
				int item10 = tuple.Item3;
				string item11 = tuple.Item4;
				string[] item12 = tuple.Item5;
				CameraCurves item13 = tuple.Item6;
				AudioMixerSnapshot obj2 = tuple.Item7;
				CameraCurves cameraColorCorrection2 = item13 ?? val.cameraColorCorrection;
				if (obj2 == null)
				{
					obj2 = val.musicSnapshot;
				}
				AudioMixerSnapshot musicSnapshot2 = obj2;
				item9.transform.SetParent(((Component)self).transform);
				MenuStyle item14 = new MenuStyle
				{
					enabled = true,
					displayName = item8,
					styleObject = item9,
					cameraColorCorrection = cameraColorCorrection2,
					musicSnapshot = musicSnapshot2,
					titleIndex = item10,
					unlockKey = item11,
					achievementKeys = item12,
					initialAudioVolumes = val.initialAudioVolumes
				};
				list.Add(item14);
			}
			if (AddMenuStyleHook != null)
			{
				Delegate[] invocationList = AddMenuStyleHook.GetInvocationList();
				foreach (Delegate obj3 in invocationList)
				{
					if ((object)obj3 != null)
					{
						(string, GameObject, int, string, string[], CameraCurves, AudioMixerSnapshot) obj4 = ((string, GameObject, int, string, string[], CameraCurves, AudioMixerSnapshot))obj3.DynamicInvoke(self);
						string item15 = obj4.Item1;
						GameObject item16 = obj4.Item2;
						int item17 = obj4.Item3;
						string item18 = obj4.Item4;
						string[] item19 = obj4.Item5;
						CameraCurves item20 = obj4.Item6;
						AudioMixerSnapshot obj5 = obj4.Item7;
						CameraCurves cameraColorCorrection3 = item20 ?? val.cameraColorCorrection;
						if (obj5 == null)
						{
							obj5 = val.musicSnapshot;
						}
						AudioMixerSnapshot musicSnapshot3 = obj5;
						item16.transform.SetParent(((Component)self).transform);
						MenuStyle item21 = new MenuStyle
						{
							enabled = true,
							displayName = item15,
							styleObject = item16,
							cameraColorCorrection = cameraColorCorrection3,
							musicSnapshot = musicSnapshot3,
							titleIndex = item17,
							unlockKey = item18,
							achievementKeys = item19,
							initialAudioVolumes = val.initialAudioVolumes
						};
						list.Add(item21);
					}
				}
			}
			self.styles = list.ToArray();
			if (Platform.Current.EncryptedSharedData.GetInt("menuStyle", 0) >= self.styles.Length)
			{
				Platform.Current.EncryptedSharedData.SetInt("menuStyle", 0);
			}
		}

		private static IEnumerator OnMenuStylesFade(orig_Fade orig, MenuStyles self, int styleindex, int fadetype, bool fade, AudioSource[] audiosources)
		{
			IEnumerator result = orig.Invoke(self, styleindex, fadetype, fade, audiosources);
			GameCameras.instance.colorCorrectionCurves.UpdateTextures();
			return result;
		}

		private static void OnMenuStylesSetStyle(orig_SetStyle orig, MenuStyles self, int index, bool fade, bool save)
		{
			if (index < 0 || index >= 10)
			{
				orig.Invoke(self, index, fade, false);
				Platform.Current.EncryptedSharedData.SetInt("menuStyle", 0);
			}
			else
			{
				orig.Invoke(self, index, fade, save);
			}
		}

		private static IEnumerator UpdateTexturesWhileSwitching(MenuStyles self)
		{
			while (self.GetAttr<MenuStyles, Coroutine>("fadeRoutine") != null)
			{
				GameCameras.instance.colorCorrectionCurves.UpdateTextures();
				yield return null;
			}
			for (float elapsed = 0f; elapsed < 0.251f; elapsed += Time.deltaTime)
			{
				GameCameras.instance.colorCorrectionCurves.UpdateTextures();
				yield return null;
			}
		}

		private static void LogFine(string message)
		{
			InternalLogger.LogFine(message, "[SFCore]:[MenuStyleHelper]");
		}

		private static void LogFine(object message)
		{
			LogFine($"{message}");
		}

		private static void LogDebug(string message)
		{
			InternalLogger.LogDebug(message, "[SFCore]:[MenuStyleHelper]");
		}

		private static void LogDebug(object message)
		{
			LogDebug($"{message}");
		}

		private static void Log(string message)
		{
			InternalLogger.Log(message, "[SFCore]:[MenuStyleHelper]");
		}

		private static void Log(object message)
		{
			Log($"{message}");
		}

		private static void LogWarn(string message)
		{
			InternalLogger.LogWarn(message, "[SFCore]:[MenuStyleHelper]");
		}

		private static void LogWarn(object message)
		{
			LogWarn($"{message}");
		}

		private static void LogError(string message)
		{
			InternalLogger.LogError(message, "[SFCore]:[MenuStyleHelper]");
		}

		private static void LogError(object message)
		{
			LogError($"{message}");
		}
	}
	public class SFCoreMod : FullSettingsMod<SFCoreSaveSettings, SFCoreGlobalSettings>
	{
		internal static SFCoreMod instance;

		static SFCoreMod()
		{
			MenuStyleHelper.unusedInit();
			TitleLogoHelper.unusedInit();
		}

		public SFCoreMod()
			: base("SFCore£¨ºËÐÄ£©")
		{
			instance = this;
		}

		public override string GetVersion()
		{
			return Util.GetVersion(Assembly.GetExecutingAssembly());
		}

		public override List<(string, string)> GetPreloadNames()
		{
			return new List<(string, string)> { ("Room_Mender_House", "_SceneManager") };
		}

		public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
		{
			SceneManagerPatcher.LoadPrefabs(preloadedObjects["Room_Mender_House"]["_SceneManager"].GetComponent<SceneManager>());
		}
	}
	public class SFCoreGlobalSettings
	{
		public int MaxCustomCharms;

		public string SelectedMenuTheme = "UI_MENU_STYLE_CLASSIC";
	}
	public class SFCoreSaveSettings
	{
	}
	public static class TitleLogoHelper
	{
		private static bool _instanceChanged;

		private static MenuStyleTitle _lastInstance;

		private static List<Sprite> _customLogos;

		static TitleLogoHelper()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			_instanceChanged = false;
			_lastInstance = null;
			_customLogos = new List<Sprite>();
			MenuStyleTitle.SetTitle += new hook_SetTitle(OnMenuStyleTitleSetTitle);
		}

		public static void unusedInit()
		{
		}

		public static int AddLogo(Sprite logo)
		{
			_customLogos.Add(logo);
			Object.DontDestroyOnLoad((Object)(object)logo);
			return _customLogos.Count;
		}

		private static void OnMenuStyleTitleSetTitle(orig_SetTitle orig, MenuStyleTitle self, int index)
		{
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)self == (Object)null)
			{
				orig.Invoke(self, index);
				return;
			}
			if (_lastInstance != self)
			{
				_lastInstance = self;
				_instanceChanged = false;
			}
			if (!_instanceChanged)
			{
				if (self.TitleSprites == null)
				{
					orig.Invoke(self, index);
					return;
				}
				RuntimePlatform[] array = new RuntimePlatform[13];
				RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
				RuntimePlatform[] platformWhitelist = (RuntimePlatform[])(object)array;
				List<TitleSpriteCollection> list = new List<TitleSpriteCollection>(self.TitleSprites);
				foreach (Sprite customLogo in _customLogos)
				{
					list.Add(new TitleSpriteCollection
					{
						PlatformWhitelist = platformWhitelist,
						Default = customLogo,
						Chinese = null,
						Russian = null,
						Italian = null,
						Japanese = null,
						Spanish = null,
						Korean = null,
						French = null,
						BrazilianPT = null
					});
				}
				self.TitleSprites = list.ToArray();
				_instanceChanged = true;
			}
			orig.Invoke(self, index);
		}

		private static void LogFine(string message)
		{
			InternalLogger.LogFine(message, "[SFCore]:[TitleLogoHelper]");
		}

		private static void LogFine(object message)
		{
			LogFine($"{message}");
		}

		private static void LogDebug(string message)
		{
			InternalLogger.LogDebug(message, "[SFCore]:[TitleLogoHelper]");
		}

		private static void LogDebug(object message)
		{
			LogDebug($"{message}");
		}

		private static void Log(string message)
		{
			InternalLogger.Log(message, "[SFCore]:[TitleLogoHelper]");
		}

		private static void Log(object message)
		{
			Log($"{message}");
		}

		private static void LogWarn(string message)
		{
			InternalLogger.LogWarn(message, "[SFCore]:[TitleLogoHelper]");
		}

		private static void LogWarn(object message)
		{
			LogWarn($"{message}");
		}

		private static void LogError(string message)
		{
			InternalLogger.LogError(message, "[SFCore]:[TitleLogoHelper]");
		}

		private static void LogError(object message)
		{
			LogError($"{message}");
		}
	}
}
namespace SFCore.Utils
{
	public static class FsmUtil
	{
		[PublicAPI]
		public static PlayMakerFSM GetFsm(this GameObject go, string fsmName)
		{
			PlayMakerFSM[] components = go.GetComponents<PlayMakerFSM>();
			foreach (PlayMakerFSM val in components)
			{
				if (val.FsmName == fsmName)
				{
					return val;
				}
			}
			return null;
		}

		[PublicAPI]
		public static PlayMakerFSM GetFsmPreprocessed(this GameObject go, string fsmName)
		{
			PlayMakerFSM[] components = go.GetComponents<PlayMakerFSM>();
			foreach (PlayMakerFSM val in components)
			{
				if (val.FsmName == fsmName)
				{
					val.Preprocess();
					return val;
				}
			}
			return null;
		}

		private static TVal GetItemFromArray<TVal>(TVal[] origArray, Func<TVal, bool> isItemCheck) where TVal : class
		{
			foreach (TVal val in origArray)
			{
				if (isItemCheck(val))
				{
					return val;
				}
			}
			return null;
		}

		private static TVal[] GetItemsFromArray<TVal>(TVal[] origArray, Func<TVal, bool> isItemCheck) where TVal : class
		{
			int num = 0;
			TVal[] array = origArray;
			foreach (TVal arg in array)
			{
				if (isItemCheck(arg))
				{
					num++;
				}
			}
			if (num == origArray.Length)
			{
				return origArray;
			}
			if (num == 0)
			{
				return Array.Empty<TVal>();
			}
			TVal[] array2 = new TVal[num];
			int num2 = 0;
			array = origArray;
			foreach (TVal val in array)
			{
				if (isItemCheck(val))
				{
					array2[num2] = val;
					num2++;
				}
			}
			return array2;
		}

		[PublicAPI]
		public static FsmState GetState(this PlayMakerFSM fsm, string stateName)
		{
			return GetItemFromArray(fsm.FsmStates, (FsmState x) => x.Name == stateName);
		}

		[PublicAPI]
		public static FsmState GetState(this Fsm fsm, string stateName)
		{
			return GetItemFromArray(fsm.States, (FsmState x) => x.Name == stateName);
		}

		[PublicAPI]
		public static FsmTransition GetTransition(this PlayMakerFSM fsm, string stateName, string eventName)
		{
			return fsm.GetState(stateName).GetTransition(eventName);
		}

		[PublicAPI]
		public static FsmTransition GetTransition(this Fsm fsm, string stateName, string eventName)
		{
			return fsm.GetState(stateName).GetTransition(eventName);
		}

		[PublicAPI]
		public static FsmTransition GetTransition(this FsmState state, string eventName)
		{
			return GetItemFromArray(state.Transitions, (FsmTransition x) => x.EventName == eventName);
		}

		[PublicAPI]
		public static FsmTransition GetGlobalTransition(this PlayMakerFSM fsm, string globalEventName)
		{
			return fsm.Fsm.GetGlobalTransition(globalEventName);
		}

		[PublicAPI]
		public static FsmTransition GetGlobalTransition(this Fsm fsm, string globalEventName)
		{
			return GetItemFromArray(fsm.GlobalTransitions, (FsmTransition x) => x.EventName == globalEventName);
		}

		[PublicAPI]
		public static TAction GetAction<TAction>(this PlayMakerFSM fsm, string stateName, int index) where TAction : FsmStateAction
		{
			return fsm.GetState(stateName).GetAction<TAction>(index);
		}

		[PublicAPI]
		public static TAction GetAction<TAction>(this Fsm fsm, string stateName, int index) where TAction : FsmStateAction
		{
			return fsm.GetState(stateName).GetAction<TAction>(index);
		}

		[PublicAPI]
		public static TAction GetAction<TAction>(this FsmState state, int index) where TAction : FsmStateAction
		{
			FsmStateAction obj = state.Actions[index];
			return (TAction)(object)((obj is TAction) ? obj : null);
		}

		[PublicAPI]
		public static FsmStateAction GetStateAction(this PlayMakerFSM fsm, string stateName, int index)
		{
			return fsm.GetState(stateName).GetStateAction(index);
		}

		[PublicAPI]
		public static FsmStateAction GetStateAction(this Fsm fsm, string stateName, int index)
		{
			return fsm.GetState(stateName).GetStateAction(index);
		}

		[PublicAPI]
		public static FsmStateAction GetStateAction(this FsmState state, int index)
		{
			return state.Actions[index];
		}

		[PublicAPI]
		public static TAction[] GetActionsOfType<TAction>(this PlayMakerFSM fsm, string stateName) where TAction : FsmStateAction
		{
			return fsm.GetState(stateName).GetActionsOfType<TAction>();
		}

		[PublicAPI]
		public static TAction[] GetActionsOfType<TAction>(this Fsm fsm, string stateName) where TAction : FsmStateAction
		{
			return fsm.GetState(stateName).GetActionsOfType<TAction>();
		}

		[PublicAPI]
		public static TAction[] GetActionsOfType<TAction>(this FsmState state) where TAction : FsmStateAction
		{
			return Array.ConvertAll(GetItemsFromArray(state.Actions, (FsmStateAction x) => x is TAction), (FsmStateAction x) => (TAction)(object)x);
		}

		[PublicAPI]
		public static TAction GetFirstActionOfType<TAction>(this PlayMakerFSM fsm, string stateName) where TAction : FsmStateAction
		{
			return fsm.GetState(stateName).GetFirstActionOfType<TAction>();
		}

		[PublicAPI]
		public static TAction GetFirstActionOfType<TAction>(this Fsm fsm, string stateName) where TAction : FsmStateAction
		{
			return fsm.GetState(stateName).GetFirstActionOfType<TAction>();
		}

		[PublicAPI]
		public static TAction GetFirstActionOfType<TAction>(this FsmState state) where TAction : FsmStateAction
		{
			int num = -1;
			for (int i = 0; i < state.Actions.Length; i++)
			{
				if (state.Actions[i] is TAction)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				return default(TAction);
			}
			return state.GetAction<TAction>(num);
		}

		[PublicAPI]
		public static TAction GetLastActionOfType<TAction>(this PlayMakerFSM fsm, string stateName) where TAction : FsmStateAction
		{
			return fsm.GetState(stateName).GetLastActionOfType<TAction>();
		}

		[PublicAPI]
		public static TAction GetLastActionOfType<TAction>(this Fsm fsm, string stateName) where TAction : FsmStateAction
		{
			return fsm.GetState(stateName).GetLastActionOfType<TAction>();
		}

		[PublicAPI]
		public static TAction GetLastActionOfType<TAction>(this FsmState state) where TAction : FsmStateAction
		{
			int num = -1;
			for (int num2 = state.Actions.Length - 1; num2 >= 0; num2--)
			{
				if (state.Actions[num2] is TAction)
				{
					num = num2;
					break;
				}
			}
			if (num == -1)
			{
				return default(TAction);
			}
			return state.GetAction<TAction>(num);
		}

		private static TVal[] AddItemToArray<TVal>(TVal[] origArray, TVal value)
		{
			TVal[] array = new TVal[origArray.Length + 1];
			origArray.CopyTo(array, 0);
			array[origArray.Length] = value;
			return array;
		}

		[PublicAPI]
		public static FsmState AddState(this PlayMakerFSM fsm, string stateName)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			return fsm.Fsm.AddState(new FsmState(fsm.Fsm)
			{
				Name = stateName
			});
		}

		[PublicAPI]
		public static FsmState AddState(this Fsm fsm, string stateName)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			return fsm.AddState(new FsmState(fsm)
			{
				Name = stateName
			});
		}

		[PublicAPI]
		public static FsmState AddState(this PlayMakerFSM fsm, FsmState state)
		{
			return fsm.Fsm.AddState(state);
		}

		[PublicAPI]
		public static FsmState AddState(this Fsm fsm, FsmState state)
		{
			FsmState[] states = fsm.States;
			FsmState[] array = (fsm.States = AddItemToArray(states, state));
			fsm.SaveActions();
			return array[states.Length];
		}

		[PublicAPI]
		public static FsmState CopyState(this PlayMakerFSM fsm, string fromState, string toState)
		{
			return fsm.Fsm.CopyState(fromState, toState);
		}

		[PublicAPI]
		public static FsmState CopyState(this Fsm fsm, string fromState, string toState)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			FsmState state = fsm.GetState(fromState);
			state.SaveActions();
			FsmState val = new FsmState(state)
			{
				Name = toState
			};
			FsmTransition[] transitions = val.Transitions;
			foreach (FsmTransition val2 in transitions)
			{
				val2.ToFsmState = fsm.GetState(val2.ToState);
			}
			fsm.AddState(val);
			return val;
		}

		[PublicAPI]
		public static FsmEvent AddTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState)
		{
			return fsm.GetState(stateName).AddTransition(eventName, toState);
		}

		[PublicAPI]
		public static FsmEvent AddTransition(this Fsm fsm, string stateName, string eventName, string toState)
		{
			return fsm.GetState(stateName).AddTransition(eventName, toState);
		}

		[PublicAPI]
		public static FsmEvent AddTransition(this FsmState state, string eventName, string toState)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected O, but got Unknown
			FsmEvent fsmEvent = FsmEvent.GetFsmEvent(eventName);
			FsmTransition[] transitions = AddItemToArray(state.Transitions, new FsmTransition
			{
				ToState = toState,
				ToFsmState = state.Fsm.GetState(toState),
				FsmEvent = fsmEvent
			});
			state.Transitions = transitions;
			return fsmEvent;
		}

		[PublicAPI]
		public static FsmEvent AddGlobalTransition(this PlayMakerFSM fsm, string globalEventName, string toState)
		{
			return fsm.Fsm.AddGlobalTransition(globalEventName, toState);
		}

		[PublicAPI]
		public static FsmEvent AddGlobalTransition(this Fsm fsm, string globalEventName, string toState)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Expected O, but got Unknown
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected O, but got Unknown
			FsmEvent val = new FsmEvent(globalEventName)
			{
				IsGlobal = true
			};
			FsmTransition[] globalTransitions = AddItemToArray(fsm.GlobalTransitions, new FsmTransition
			{
				ToState = toState,
				ToFsmState = fsm.GetState(toState),
				FsmEvent = val
			});
			fsm.GlobalTransitions = globalTransitions;
			return val;
		}

		[PublicAPI]
		public static void AddAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action)
		{
			fsm.GetState(stateName).AddAction(action);
		}

		[PublicAPI]
		public static void AddAction(this Fsm fsm, string stateName, FsmStateAction action)
		{
			fsm.GetState(stateName).AddAction(action);
		}

		[PublicAPI]
		public static void AddAction(this FsmState state, FsmStateAction action)
		{
			FsmStateAction[] actions = AddItemToArray(state.Actions, action);
			state.Actions = actions;
			action.Init(state);
		}

		[PublicAPI]
		public static void AddActions(this PlayMakerFSM fsm, string stateName, params FsmStateAction[] actions)
		{
			fsm.GetState(stateName).AddActions(actions);
		}

		[PublicAPI]
		public static void AddActions(this Fsm fsm, string stateName, params FsmStateAction[] actions)
		{
			fsm.GetState(stateName).AddActions(actions);
		}

		[PublicAPI]
		public static void AddActions(this FsmState state, params FsmStateAction[] actions)
		{
			foreach (FsmStateAction action in actions)
			{
				state.AddAction(action);
			}
		}

		[PublicAPI]
		public static void AddMethod(this PlayMakerFSM fsm, string stateName, Action method)
		{
			fsm.GetState(stateName).AddMethod(method);
		}

		[PublicAPI]
		public static void AddMethod(this Fsm fsm, string stateName, Action method)
		{
			fsm.GetState(stateName).AddMethod(method);
		}

		[PublicAPI]
		public static void AddMethod(this FsmState state, Action method)
		{
			MethodAction action = new MethodAction
			{
				method = method
			};
			state.AddAction((FsmStateAction)(object)action);
		}

		[PublicAPI]
		public static void AddLambdaMethod(this PlayMakerFSM fsm, string stateName, Action<Action> method)
		{
			fsm.GetState(stateName).AddLambdaMethod(method);
		}

		[PublicAPI]
		public static void AddLambdaMethod(this Fsm fsm, string stateName, Action<Action> method)
		{
			fsm.GetState(stateName).AddLambdaMethod(method);
		}

		[PublicAPI]
		public static void AddLambdaMethod(this FsmState state, Action<Action> method)
		{
			FunctionAction<Action> functionAction = new FunctionAction<Action>
			{
				action = method
			};
			functionAction.arg = ((FsmStateAction)functionAction).Finish;
			state.AddAction((FsmStateAction)(object)functionAction);
		}

		private static TVal[] InsertItemIntoArray<TVal>(TVal[] origArray, TVal value, int index)
		{
			int num = origArray.Length;
			if (index < 0 || index > num + 1)
			{
				throw new ArgumentOutOfRangeException($"Index {index} was out of range for array with length {num}!");
			}
			TVal[] array = new TVal[num + 1];
			for (int i = 0; i < index; i++)
			{
				array[i] = origArray[i];
			}
			array[index] = value;
			for (int i = index; i < num; i++)
			{
				array[i + 1] = origArray[i];
			}
			return array;
		}

		[PublicAPI]
		public static void InsertAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action, int index)
		{
			fsm.GetState(stateName).InsertAction(index, action);
		}

		[PublicAPI]
		public static void InsertAction(this PlayMakerFSM fsm, string stateName, int index, FsmStateAction action)
		{
			fsm.GetState(stateName).InsertAction(index, action);
		}

		[PublicAPI]
		public static void InsertAction(this Fsm fsm, string stateName, FsmStateAction action, int index)
		{
			fsm.GetState(stateName).InsertAction(index, action);
		}

		[PublicAPI]
		public static void InsertAction(this Fsm fsm, string stateName, int index, FsmStateAction action)
		{
			fsm.GetState(stateName).InsertAction(index, action);
		}

		[PublicAPI]
		public static void InsertAction(this FsmState state, FsmStateAction action, int index)
		{
			state.InsertAction(index, action);
		}

		[PublicAPI]
		public static void InsertAction(this FsmState state, int index, FsmStateAction action)
		{
			FsmStateAction[] actions = InsertItemIntoArray(state.Actions, action, index);
			state.Actions = actions;
			action.Init(state);
		}

		[PublicAPI]
		public static void InsertActions(this PlayMakerFSM fsm, string stateName, int index, params FsmStateAction[] actions)
		{
			fsm.GetState(stateName).InsertActions(index, actions);
		}

		[PublicAPI]
		public static void InsertActions(this Fsm fsm, string stateName, int index, params FsmStateAction[] actions)
		{
			fsm.GetState(stateName).InsertActions(index, actions);
		}

		[PublicAPI]
		public static void InsertActions(this FsmState state, int index, params FsmStateAction[] actions)
		{
			foreach (FsmStateAction action in actions)
			{
				state.InsertAction(action, index);
				index++;
			}
		}

		[PublicAPI]
		public static void InsertMethod(this PlayMakerFSM fsm, string stateName, Action method, int index)
		{
			fsm.GetState(stateName).InsertMethod(index, method);
		}

		[PublicAPI]
		public static void InsertMethod(this PlayMakerFSM fsm, string stateName, int index, Action method)
		{
			fsm.GetState(stateName).InsertMethod(index, method);
		}

		[PublicAPI]
		public static void InsertMethod(this Fsm fsm, string stateName, Action method, int index)
		{
			fsm.GetState(stateName).InsertMethod(index, method);
		}

		[PublicAPI]
		public static void InsertMethod(this Fsm fsm, string stateName, int index, Action method)
		{
			fsm.GetState(stateName).InsertMethod(index, method);
		}

		[PublicAPI]
		public static void InsertMethod(this FsmState state, Action method, int index)
		{
			state.InsertMethod(index, method);
		}

		[PublicAPI]
		public static void InsertMethod(this FsmState state, int index, Action method)
		{
			MethodAction action = new MethodAction
			{
				method = method
			};
			state.InsertAction((FsmStateAction)(object)action, index);
		}

		[PublicAPI]
		public static void InsertLambdaMethod(this PlayMakerFSM fsm, string stateName, Action<Action> method, int index)
		{
			fsm.GetState(stateName).InsertLambdaMethod(index, method);
		}

		[PublicAPI]
		public static void InsertLambdaMethod(this PlayMakerFSM fsm, string stateName, int index, Action<Action> method)
		{
			fsm.GetState(stateName).InsertLambdaMethod(index, method);
		}

		[PublicAPI]
		public static void InsertLambdaMethod(this Fsm fsm, string stateName, Action<Action> method, int index)
		{
			fsm.GetState(stateName).InsertLambdaMethod(index, method);
		}

		[PublicAPI]
		public static void InsertLambdaMethod(this Fsm fsm, string stateName, int index, Action<Action> method)
		{
			fsm.GetState(stateName).InsertLambdaMethod(index, method);
		}

		[PublicAPI]
		public static void InsertLambdaMethod(this FsmState state, Action<Action> method, int index)
		{
			state.InsertLambdaMethod(index, method);
		}

		[PublicAPI]
		public static void InsertLambdaMethod(this FsmState state, int index, Action<Action> method)
		{
			FunctionAction<Action> functionAction = new FunctionAction<Action>
			{
				action = method
			};
			functionAction.arg = ((FsmStateAction)functionAction).Finish;
			state.InsertAction((FsmStateAction)(object)functionAction, index);
		}

		[PublicAPI]
		public static void InsertMethodBefore(this FsmStateAction action, Action method)
		{
			FsmState state = action.State;
			int index = Array.IndexOf(state.Actions, action);
			state.InsertMethod(index, method);
		}

		[PublicAPI]
		public static void InsertMethodAfter(this FsmStateAction action, Action method)
		{
			FsmState state = action.State;
			int num = Array.IndexOf(state.Actions, action);
			state.InsertMethod(num + 1, method);
		}

		[PublicAPI]
		public static void InsertActionBefore(this FsmStateAction action, FsmStateAction newAction)
		{
			FsmState state = action.State;
			int index = Array.IndexOf(state.Actions, action);
			state.InsertAction(index, newAction);
		}

		[PublicAPI]
		public static void InsertActionAfter(this FsmStateAction action, FsmStateAction newAction)
		{
			FsmState state = action.State;
			int num = Array.IndexOf(state.Actions, action);
			state.InsertAction(num + 1, newAction);
		}

		[PublicAPI]
		public static void ReplaceAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action, int index)
		{
			fsm.GetState(stateName).ReplaceAction(index, action);
		}

		[PublicAPI]
		public static void ReplaceAction(this PlayMakerFSM fsm, string stateName, int index, FsmStateAction action)
		{
			fsm.GetState(stateName).ReplaceAction(index, action);
		}

		[PublicAPI]
		public static void ReplaceAction(this Fsm fsm, string stateName, FsmStateAction action, int index)
		{
			fsm.GetState(stateName).ReplaceAction(index, action);
		}

		[PublicAPI]
		public static void ReplaceAction(this Fsm fsm, string stateName, int index, FsmStateAction action)
		{
			fsm.GetState(stateName).ReplaceAction(index, action);
		}

		[PublicAPI]
		public static void ReplaceAction(this FsmState state, FsmStateAction action, int index)
		{
			state.ReplaceAction(index, action);
		}

		[PublicAPI]
		public static void ReplaceAction(this FsmState state, int index, FsmStateAction action)
		{
			state.Actions[index] = action;
			action.Init(state);
		}

		[PublicAPI]
		public static void ReplaceAllActions(this PlayMakerFSM fsm, string stateName, params FsmStateAction[] actions)
		{
			fsm.GetState(stateName).ReplaceAllActions(actions);
		}

		[PublicAPI]
		public static void ReplaceAllActions(this Fsm fsm, string stateName, params FsmStateAction[] actions)
		{
			fsm.GetState(stateName).ReplaceAllActions(actions);
		}

		[PublicAPI]
		public static void ReplaceAllActions(this FsmState state, params FsmStateAction[] actions)
		{
			state.Actions = actions;
			for (int i = 0; i < actions.Length; i++)
			{
				actions[i].Init(state);
			}
		}

		[PublicAPI]
		public static void ChangeTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState)
		{
			fsm.GetState(stateName).ChangeTransition(eventName, toState);
		}

		[PublicAPI]
		public static void ChangeTransition(this Fsm fsm, string stateName, string eventName, string toState)
		{
			fsm.GetState(stateName).ChangeTransition(eventName, toState);
		}

		[PublicAPI]
		public static void ChangeTransition(this FsmState state, string eventName, string toState)
		{
			FsmTransition transition = state.GetTransition(eventName);
			if (transition != null)
			{
				transition.ToState = toState;
				transition.ToFsmState = state.Fsm.GetState(toState);
			}
		}

		[PublicAPI]
		public static bool ChangeGlobalTransition(this PlayMakerFSM fsm, string globalEventName, string toState)
		{
			return fsm.Fsm.ChangeGlobalTransition(globalEventName, toState);
		}

		[PublicAPI]
		public static bool ChangeGlobalTransition(this Fsm fsm, string globalEventName, string toState)
		{
			FsmTransition globalTransition = fsm.GetGlobalTransition(globalEventName);
			if (globalTransition == null)
			{
				return false;
			}
			globalTransition.ToState = toState;
			globalTransition.ToFsmState = fsm.GetState(toState);
			return true;
		}

		private static TVal[] RemoveItemsFromArray<TVal>(TVal[] origArray, Func<TVal, bool> shouldBeRemovedCallback)
		{
			int num = 0;
			foreach (TVal arg in origArray)
			{
				if (shouldBeRemovedCallback(arg))
				{
					num++;
				}
			}
			if (num == 0)
			{
				return origArray;
			}
			TVal[] array = new TVal[origArray.Length - num];
			for (int num2 = origArray.Length - 1; num2 >= 0; num2--)
			{
				TVal val = origArray[num2];
				if (shouldBeRemovedCallback(val))
				{
					num--;
				}
				else
				{
					array[num2 - num] = val;
				}
			}
			return array;
		}

		[PublicAPI]
		public static void RemoveState(this PlayMakerFSM fsm, string stateName)
		{
			fsm.Fsm.RemoveState(stateName);
		}

		[PublicAPI]
		public static void RemoveState(this Fsm fsm, string stateName)
		{
			fsm.States = RemoveItemsFromArray(fsm.States, (FsmState x) => x.Name == stateName);
		}

		[PublicAPI]
		public static void RemoveTransition(this PlayMakerFSM fsm, string stateName, string eventName)
		{
			fsm.GetState(stateName).RemoveTransition(eventName);
		}

		[PublicAPI]
		public static void RemoveTransition(this Fsm fsm, string stateName, string eventName)
		{
			fsm.GetState(stateName).RemoveTransition(eventName);
		}

		[PublicAPI]
		public static void RemoveTransition(this FsmState state, string eventName)
		{
			state.Transitions = RemoveItemsFromArray(state.Transitions, (FsmTransition x) => x.EventName == eventName);
		}

		[PublicAPI]
		public static void RemoveGlobalTransition(this PlayMakerFSM fsm, string globalEventName)
		{
			fsm.Fsm.RemoveGlobalTransition(globalEventName);
		}

		[PublicAPI]
		public static void RemoveGlobalTransition(this Fsm fsm, string globalEventName)
		{
			fsm.GlobalTransitions = RemoveItemsFromArray(fsm.GlobalTransitions, (FsmTransition x) => x.EventName == globalEventName);
		}

		[PublicAPI]
		public static void RemoveTransitionsTo(this PlayMakerFSM fsm, string toState)
		{
			fsm.Fsm.RemoveTransitionsTo(toState);
		}

		[PublicAPI]
		public static void RemoveTransitionsTo(this Fsm fsm, string toState)
		{
			FsmState[] states = fsm.States;
			for (int i = 0; i < states.Length; i++)
			{
				states[i].RemoveTransitionsTo(toState);
			}
		}

		[PublicAPI]
		public static void RemoveTransitionsTo(this PlayMakerFSM fsm, string stateName, string toState)
		{
			fsm.GetState(stateName).RemoveTransitionsTo(toState);
		}

		[PublicAPI]
		public static void RemoveTransitionsTo(this Fsm fsm, string stateName, string toState)
		{
			fsm.GetState(stateName).RemoveTransitionsTo(toState);
		}

		[PublicAPI]
		public static void RemoveTransitionsTo(this FsmState state, string toState)
		{
			state.Transitions = RemoveItemsFromArray(state.Transitions, (FsmTransition x) => x.ToState == toState);
		}

		[PublicAPI]
		public static void RemoveTransitions(this PlayMakerFSM fsm, string stateName)
		{
			fsm.GetState(stateName).RemoveTransitions();
		}

		[PublicAPI]
		public static void RemoveTransitions(this Fsm fsm, string stateName)
		{
			fsm.GetState(stateName).RemoveTransitions();
		}

		[PublicAPI]
		public static void RemoveTransitions(this FsmState state)
		{
			state.Transitions = Array.Empty<FsmTransition>();
		}

		[PublicAPI]
		public static void RemoveAction(this PlayMakerFSM fsm, string stateName, int index)
		{
			fsm.GetState(stateName).RemoveAction(index);
		}

		[PublicAPI]
		public static void RemoveAction(this Fsm fsm, string stateName, int index)
		{
			fsm.GetState(stateName).RemoveAction(index);
		}

		[PublicAPI]
		public static void RemoveAction(this FsmState state, int index)
		{
			FsmStateAction[] actions = state.Actions;
			if (index >= 0 && index < actions.Length)
			{
				FsmStateAction[] array = (FsmStateAction[])(object)new FsmStateAction[actions.Length - 1];
				int num = array.Length;
				for (int i = 0; i < index; i++)
				{
					array[i] = actions[i];
				}
				for (int i = index; i < num; i++)
				{
					array[i] = actions[i + 1];
				}
				state.Actions = array;
			}
		}

		[PublicAPI]
		public static void RemoveActionsOfType<TAction>(this PlayMakerFSM fsm)
		{
			fsm.Fsm.RemoveActionsOfType<TAction>();
		}

		[PublicAPI]
		public static void RemoveActionsOfType<TAction>(this Fsm fsm)
		{
			FsmState[] states = fsm.States;
			for (int i = 0; i < states.Length; i++)
			{
				states[i].RemoveActionsOfType<TAction>();
			}
		}

		[PublicAPI]
		public static void RemoveActionsOfType<TAction>(this PlayMakerFSM fsm, string stateName)
		{
			fsm.GetState(stateName).RemoveActionsOfType<TAction>();
		}

		[PublicAPI]
		public static void RemoveActionsOfType<TAction>(this Fsm fsm, string stateName)
		{
			fsm.GetState(stateName).RemoveActionsOfType<TAction>();
		}

		[PublicAPI]
		public static void RemoveActionsOfType<TAction>(this FsmState state)
		{
			state.Actions = RemoveItemsFromArray(state.Actions, (FsmStateAction x) => x is TAction);
		}

		[PublicAPI]
		public static void RemoveFirstActionOfType<TAction>(this PlayMakerFSM fsm, string stateName)
		{
			fsm.GetState(stateName).RemoveFirstActionOfType<TAction>();
		}

		[PublicAPI]
		public static void RemoveFirstActionOfType<TAction>(this Fsm fsm, string stateName)
		{
			fsm.GetState(stateName).RemoveFirstActionOfType<TAction>();
		}

		[PublicAPI]
		public static void RemoveFirstActionOfType<TAction>(this FsmState state)
		{
			int num = -1;
			for (int i = 0; i < state.Actions.Length; i++)
			{
				if (state.Actions[i] is TAction)
				{
					num = i;
					break;
				}
			}
			if (num != -1)
			{
				state.RemoveAction(num);
			}
		}

		[PublicAPI]
		public static void RemoveLastActionOfType<TAction>(this PlayMakerFSM fsm, string stateName)
		{
			fsm.GetState(stateName).RemoveLastActionOfType<TAction>();
		}

		[PublicAPI]
		public static void RemoveLastActionOfType<TAction>(this Fsm fsm, string stateName)
		{
			fsm.GetState(stateName).RemoveLastActionOfType<TAction>();
		}

		[PublicAPI]
		public static void RemoveLastActionOfType<TAction>(this FsmState state)
		{
			int num = -1;
			for (int num2 = state.Actions.Length - 1; num2 >= 0; num2--)
			{
				if (state.Actions[num2] is TAction)
				{
					num = num2;
					break;
				}
			}
			if (num != -1)
			{
				state.RemoveAction(num);
			}
		}

		[PublicAPI]
		public static bool DisableAction(this PlayMakerFSM fsm, string stateName, int index)
		{
			return fsm.GetState(stateName).DisableAction(index);
		}

		[PublicAPI]
		public static bool DisableAction(this Fsm fsm, string stateName, int index)
		{
			return fsm.GetState(stateName).DisableAction(index);
		}

		[PublicAPI]
		public static bool DisableAction(this FsmState state, int index)
		{
			if (index < 0 || index >= state.Actions.Length)
			{
				return false;
			}
			state.Actions[index].Enabled = false;
			return true;
		}

		[PublicAPI]
		public static bool DisableActions(this PlayMakerFSM fsm, string stateName, params int[] indices)
		{
			return fsm.GetState(stateName).DisableActions(indices);
		}

		[PublicAPI]
		public static bool DisableActions(this Fsm fsm, string stateName, params int[] indices)
		{
			return fsm.GetState(stateName).DisableActions(indices);
		}

		[PublicAPI]
		public static bool DisableActions(this FsmState state, params int[] indices)
		{
			bool flag = true;
			foreach (int index in indices)
			{
				flag = flag && state.DisableAction(index);
			}
			return flag;
		}

		[PublicAPI]
		public static void DisableActionsOfType<TAction>(this PlayMakerFSM fsm)
		{
			fsm.Fsm.DisableActionsOfType<TAction>();
		}

		[PublicAPI]
		public static void DisableActionsOfType<TAction>(this Fsm fsm)
		{
			FsmState[] states = fsm.States;
			for (int i = 0; i < states.Length; i++)
			{
				states[i].DisableActionsOfType<TAction>();
			}
		}

		[PublicAPI]
		public static void DisableActionsOfType<TAction>(this PlayMakerFSM fsm, string stateName)
		{
			fsm.GetState(stateName).DisableActionsOfType<TAction>();
		}

		[PublicAPI]
		public static void DisableActionsOfType<TAction>(this Fsm fsm, string stateName)
		{
			fsm.GetState(stateName).DisableActionsOfType<TAction>();
		}

		[PublicAPI]
		public static void DisableActionsOfType<TAction>(this FsmState state)
		{
			FsmStateAction[] actions = state.Actions;
			foreach (FsmStateAction val in actions)
			{
				if (val is TAction)
				{
					val.Enabled = false;
				}
			}
		}

		private static TVar[] MakeNewVariableArray<TVar>(TVar[] orig, string name) where TVar : NamedVariable, new()
		{
			TVar[] array = new TVar[orig.Length + 1];
			orig.CopyTo(array, 0);
			int num = orig.Length;
			TVar val = new TVar();
			((NamedVariable)val).Name = name;
			array[num] = val;
			return array;
		}

		[PublicAPI]
		public static FsmFloat AddFloatVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.AddFloatVariable(name);
		}

		[PublicAPI]
		public static FsmFloat AddFloatVariable(this Fsm fsm, string name)
		{
			FsmFloat[] array = MakeNewVariableArray<FsmFloat>(fsm.Variables.FloatVariables, name);
			fsm.Variables.FloatVariables = array;
			return array[array.Length - 1];
		}

		[PublicAPI]
		public static FsmInt AddIntVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.AddIntVariable(name);
		}

		[PublicAPI]
		public static FsmInt AddIntVariable(this Fsm fsm, string name)
		{
			FsmInt[] array = MakeNewVariableArray<FsmInt>(fsm.Variables.IntVariables, name);
			fsm.Variables.IntVariables = array;
			return array[array.Length - 1];
		}

		[PublicAPI]
		public static FsmBool AddBoolVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.AddBoolVariable(name);
		}

		[PublicAPI]
		public static FsmBool AddBoolVariable(this Fsm fsm, string name)
		{
			FsmBool[] array = MakeNewVariableArray<FsmBool>(fsm.Variables.BoolVariables, name);
			fsm.Variables.BoolVariables = array;
			return array[array.Length - 1];
		}

		[PublicAPI]
		public static FsmString AddStringVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.AddStringVariable(name);
		}

		[PublicAPI]
		public static FsmString AddStringVariable(this Fsm fsm, string name)
		{
			FsmString[] array = MakeNewVariableArray<FsmString>(fsm.Variables.StringVariables, name);
			fsm.Variables.StringVariables = array;
			return array[array.Length - 1];
		}

		[PublicAPI]
		public static FsmVector2 AddVector2Variable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.AddVector2Variable(name);
		}

		[PublicAPI]
		public static FsmVector2 AddVector2Variable(this Fsm fsm, string name)
		{
			FsmVector2[] array = MakeNewVariableArray<FsmVector2>(fsm.Variables.Vector2Variables, name);
			fsm.Variables.Vector2Variables = array;
			return array[array.Length - 1];
		}

		[PublicAPI]
		public static FsmVector3 AddVector3Variable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.AddVector3Variable(name);
		}

		[PublicAPI]
		public static FsmVector3 AddVector3Variable(this Fsm fsm, string name)
		{
			FsmVector3[] array = MakeNewVariableArray<FsmVector3>(fsm.Variables.Vector3Variables, name);
			fsm.Variables.Vector3Variables = array;
			return array[array.Length - 1];
		}

		[PublicAPI]
		public static FsmRect AddRectVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.AddRectVariable(name);
		}

		[PublicAPI]
		public static FsmRect AddRectVariable(this Fsm fsm, string name)
		{
			FsmRect[] array = MakeNewVariableArray<FsmRect>(fsm.Variables.RectVariables, name);
			fsm.Variables.RectVariables = array;
			return array[array.Length - 1];
		}

		[PublicAPI]
		public static FsmQuaternion AddQuaternionVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.AddQuaternionVariable(name);
		}

		[PublicAPI]
		public static FsmQuaternion AddQuaternionVariable(this Fsm fsm, string name)
		{
			FsmQuaternion[] array = MakeNewVariableArray<FsmQuaternion>(fsm.Variables.QuaternionVariables, name);
			fsm.Variables.QuaternionVariables = array;
			return array[array.Length - 1];
		}

		[PublicAPI]
		public static FsmColor AddColorVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.AddColorVariable(name);
		}

		[PublicAPI]
		public static FsmColor AddColorVariable(this Fsm fsm, string name)
		{
			FsmColor[] array = MakeNewVariableArray<FsmColor>(fsm.Variables.ColorVariables, name);
			fsm.Variables.ColorVariables = array;
			return array[array.Length - 1];
		}

		[PublicAPI]
		public static FsmGameObject AddGameObjectVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.AddGameObjectVariable(name);
		}

		[PublicAPI]
		public static FsmGameObject AddGameObjectVariable(this Fsm fsm, string name)
		{
			FsmGameObject[] array = MakeNewVariableArray<FsmGameObject>(fsm.Variables.GameObjectVariables, name);
			fsm.Variables.GameObjectVariables = array;
			return array[array.Length - 1];
		}

		[PublicAPI]
		public static FsmArray AddArrayVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.AddArrayVariable(name);
		}

		[PublicAPI]
		public static FsmArray AddArrayVariable(this Fsm fsm, string name)
		{
			FsmArray[] array = MakeNewVariableArray<FsmArray>(fsm.Variables.ArrayVariables, name);
			fsm.Variables.ArrayVariables = array;
			return array[array.Length - 1];
		}

		[PublicAPI]
		public static FsmEnum AddEnumVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.AddEnumVariable(name);
		}

		[PublicAPI]
		public static FsmEnum AddEnumVariable(this Fsm fsm, string name)
		{
			FsmEnum[] array = MakeNewVariableArray<FsmEnum>(fsm.Variables.EnumVariables, name);
			fsm.Variables.EnumVariables = array;
			return array[array.Length - 1];
		}

		[PublicAPI]
		public static FsmObject AddObjectVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.AddObjectVariable(name);
		}

		[PublicAPI]
		public static FsmObject AddObjectVariable(this Fsm fsm, string name)
		{
			FsmObject[] array = MakeNewVariableArray<FsmObject>(fsm.Variables.ObjectVariables, name);
			fsm.Variables.ObjectVariables = array;
			return array[array.Length - 1];
		}

		[PublicAPI]
		public static FsmMaterial AddMaterialVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.AddMaterialVariable(name);
		}

		[PublicAPI]
		public static FsmMaterial AddMaterialVariable(this Fsm fsm, string name)
		{
			FsmMaterial[] array = MakeNewVariableArray<FsmMaterial>(fsm.Variables.MaterialVariables, name);
			fsm.Variables.MaterialVariables = array;
			return array[array.Length - 1];
		}

		[PublicAPI]
		public static FsmTexture AddTextureVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.AddTextureVariable(name);
		}

		[PublicAPI]
		public static FsmTexture AddTextureVariable(this Fsm fsm, string name)
		{
			FsmTexture[] array = MakeNewVariableArray<FsmTexture>(fsm.Variables.TextureVariables, name);
			fsm.Variables.TextureVariables = array;
			return array[array.Length - 1];
		}

		private static TVar FindInVariableArray<TVar>(TVar[] orig, string name) where TVar : NamedVariable, new()
		{
			foreach (TVar val in orig)
			{
				if (((NamedVariable)val).Name == name)
				{
					return val;
				}
			}
			return default(TVar);
		}

		[PublicAPI]
		public static FsmFloat FindFloatVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.FindFloatVariable(name);
		}

		[PublicAPI]
		public static FsmFloat FindFloatVariable(this Fsm fsm, string name)
		{
			return FindInVariableArray<FsmFloat>(fsm.Variables.FloatVariables, name);
		}

		[PublicAPI]
		public static FsmInt FindIntVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.FindIntVariable(name);
		}

		[PublicAPI]
		public static FsmInt FindIntVariable(this Fsm fsm, string name)
		{
			return FindInVariableArray<FsmInt>(fsm.Variables.IntVariables, name);
		}

		[PublicAPI]
		public static FsmBool FindBoolVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.FindBoolVariable(name);
		}

		[PublicAPI]
		public static FsmBool FindBoolVariable(this Fsm fsm, string name)
		{
			return FindInVariableArray<FsmBool>(fsm.Variables.BoolVariables, name);
		}

		[PublicAPI]
		public static FsmString FindStringVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.FindStringVariable(name);
		}

		[PublicAPI]
		public static FsmString FindStringVariable(this Fsm fsm, string name)
		{
			return FindInVariableArray<FsmString>(fsm.Variables.StringVariables, name);
		}

		[PublicAPI]
		public static FsmVector2 FindVector2Variable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.FindVector2Variable(name);
		}

		[PublicAPI]
		public static FsmVector2 FindVector2Variable(this Fsm fsm, string name)
		{
			return FindInVariableArray<FsmVector2>(fsm.Variables.Vector2Variables, name);
		}

		[PublicAPI]
		public static FsmVector3 FindVector3Variable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.FindVector3Variable(name);
		}

		[PublicAPI]
		public static FsmVector3 FindVector3Variable(this Fsm fsm, string name)
		{
			return FindInVariableArray<FsmVector3>(fsm.Variables.Vector3Variables, name);
		}

		[PublicAPI]
		public static FsmRect FindRectVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.FindRectVariable(name);
		}

		[PublicAPI]
		public static FsmRect FindRectVariable(this Fsm fsm, string name)
		{
			return FindInVariableArray<FsmRect>(fsm.Variables.RectVariables, name);
		}

		[PublicAPI]
		public static FsmQuaternion FindQuaternionVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.FindQuaternionVariable(name);
		}

		[PublicAPI]
		public static FsmQuaternion FindQuaternionVariable(this Fsm fsm, string name)
		{
			return FindInVariableArray<FsmQuaternion>(fsm.Variables.QuaternionVariables, name);
		}

		[PublicAPI]
		public static FsmColor FindColorVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.FindColorVariable(name);
		}

		[PublicAPI]
		public static FsmColor FindColorVariable(this Fsm fsm, string name)
		{
			return FindInVariableArray<FsmColor>(fsm.Variables.ColorVariables, name);
		}

		[PublicAPI]
		public static FsmGameObject FindGameObjectVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.FindGameObjectVariable(name);
		}

		[PublicAPI]
		public static FsmGameObject FindGameObjectVariable(this Fsm fsm, string name)
		{
			return FindInVariableArray<FsmGameObject>(fsm.Variables.GameObjectVariables, name);
		}

		[PublicAPI]
		public static FsmArray FindArrayVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.FindArrayVariable(name);
		}

		[PublicAPI]
		public static FsmArray FindArrayVariable(this Fsm fsm, string name)
		{
			return FindInVariableArray<FsmArray>(fsm.Variables.ArrayVariables, name);
		}

		[PublicAPI]
		public static FsmEnum FindEnumVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.FindEnumVariable(name);
		}

		[PublicAPI]
		public static FsmEnum FindEnumVariable(this Fsm fsm, string name)
		{
			return FindInVariableArray<FsmEnum>(fsm.Variables.EnumVariables, name);
		}

		[PublicAPI]
		public static FsmObject FindObjectVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.FindObjectVariable(name);
		}

		[PublicAPI]
		public static FsmObject FindObjectVariable(this Fsm fsm, string name)
		{
			return FindInVariableArray<FsmObject>(fsm.Variables.ObjectVariables, name);
		}

		[PublicAPI]
		public static FsmMaterial FindMaterialVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.FindMaterialVariable(name);
		}

		[PublicAPI]
		public static FsmMaterial FindMaterialVariable(this Fsm fsm, string name)
		{
			return FindInVariableArray<FsmMaterial>(fsm.Variables.MaterialVariables, name);
		}

		[PublicAPI]
		public static FsmTexture FindTextureVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.FindTextureVariable(name);
		}

		[PublicAPI]
		public static FsmTexture FindTextureVariable(this Fsm fsm, string name)
		{
			return FindInVariableArray<FsmTexture>(fsm.Variables.TextureVariables, name);
		}

		[PublicAPI]
		public static FsmFloat GetFloatVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.GetFloatVariable(name);
		}

		[PublicAPI]
		public static FsmFloat GetFloatVariable(this Fsm fsm, string name)
		{
			FsmFloat val = fsm.FindFloatVariable(name);
			if (val != null)
			{
				return val;
			}
			return fsm.AddFloatVariable(name);
		}

		[PublicAPI]
		public static FsmInt GetIntVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.GetIntVariable(name);
		}

		[PublicAPI]
		public static FsmInt GetIntVariable(this Fsm fsm, string name)
		{
			FsmInt val = fsm.FindIntVariable(name);
			if (val != null)
			{
				return val;
			}
			return fsm.AddIntVariable(name);
		}

		[PublicAPI]
		public static FsmBool GetBoolVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.GetBoolVariable(name);
		}

		[PublicAPI]
		public static FsmBool GetBoolVariable(this Fsm fsm, string name)
		{
			FsmBool val = fsm.FindBoolVariable(name);
			if (val != null)
			{
				return val;
			}
			return fsm.AddBoolVariable(name);
		}

		[PublicAPI]
		public static FsmString GetStringVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.GetStringVariable(name);
		}

		[PublicAPI]
		public static FsmString GetStringVariable(this Fsm fsm, string name)
		{
			FsmString val = fsm.FindStringVariable(name);
			if (val != null)
			{
				return val;
			}
			return fsm.AddStringVariable(name);
		}

		[PublicAPI]
		public static FsmVector2 GetVector2Variable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.GetVector2Variable(name);
		}

		[PublicAPI]
		public static FsmVector2 GetVector2Variable(this Fsm fsm, string name)
		{
			FsmVector2 val = fsm.FindVector2Variable(name);
			if (val != null)
			{
				return val;
			}
			return fsm.AddVector2Variable(name);
		}

		[PublicAPI]
		public static FsmVector3 GetVector3Variable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.GetVector3Variable(name);
		}

		[PublicAPI]
		public static FsmVector3 GetVector3Variable(this Fsm fsm, string name)
		{
			FsmVector3 val = fsm.FindVector3Variable(name);
			if (val != null)
			{
				return val;
			}
			return fsm.AddVector3Variable(name);
		}

		[PublicAPI]
		public static FsmRect GetRectVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.GetRectVariable(name);
		}

		[PublicAPI]
		public static FsmRect GetRectVariable(this Fsm fsm, string name)
		{
			FsmRect val = fsm.FindRectVariable(name);
			if (val != null)
			{
				return val;
			}
			return fsm.AddRectVariable(name);
		}

		[PublicAPI]
		public static FsmQuaternion GetQuaternionVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.GetQuaternionVariable(name);
		}

		[PublicAPI]
		public static FsmQuaternion GetQuaternionVariable(this Fsm fsm, string name)
		{
			FsmQuaternion val = fsm.FindQuaternionVariable(name);
			if (val != null)
			{
				return val;
			}
			return fsm.AddQuaternionVariable(name);
		}

		[PublicAPI]
		public static FsmColor GetColorVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.GetColorVariable(name);
		}

		[PublicAPI]
		public static FsmColor GetColorVariable(this Fsm fsm, string name)
		{
			FsmColor val = fsm.FindColorVariable(name);
			if (val != null)
			{
				return val;
			}
			return fsm.AddColorVariable(name);
		}

		[PublicAPI]
		public static FsmGameObject GetGameObjectVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.GetGameObjectVariable(name);
		}

		[PublicAPI]
		public static FsmGameObject GetGameObjectVariable(this Fsm fsm, string name)
		{
			FsmGameObject val = fsm.FindGameObjectVariable(name);
			if (val != null)
			{
				return val;
			}
			return fsm.AddGameObjectVariable(name);
		}

		[PublicAPI]
		public static FsmArray GetArrayVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.GetArrayVariable(name);
		}

		[PublicAPI]
		public static FsmArray GetArrayVariable(this Fsm fsm, string name)
		{
			FsmArray val = fsm.FindArrayVariable(name);
			if (val != null)
			{
				return val;
			}
			return fsm.AddArrayVariable(name);
		}

		[PublicAPI]
		public static FsmEnum GetEnumVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.GetEnumVariable(name);
		}

		[PublicAPI]
		public static FsmEnum GetEnumVariable(this Fsm fsm, string name)
		{
			FsmEnum val = fsm.FindEnumVariable(name);
			if (val != null)
			{
				return val;
			}
			return fsm.AddEnumVariable(name);
		}

		[PublicAPI]
		public static FsmObject GetObjectVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.GetObjectVariable(name);
		}

		[PublicAPI]
		public static FsmObject GetObjectVariable(this Fsm fsm, string name)
		{
			FsmObject val = fsm.FindObjectVariable(name);
			if (val != null)
			{
				return val;
			}
			return fsm.AddObjectVariable(name);
		}

		[PublicAPI]
		public static FsmMaterial GetMaterialVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.GetMaterialVariable(name);
		}

		[PublicAPI]
		public static FsmMaterial GetMaterialVariable(this Fsm fsm, string name)
		{
			FsmMaterial val = fsm.FindMaterialVariable(name);
			if (val != null)
			{
				return val;
			}
			return fsm.AddMaterialVariable(name);
		}

		[PublicAPI]
		public static FsmTexture GetTextureVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.Fsm.GetTextureVariable(name);
		}

		[PublicAPI]
		public static FsmTexture GetTextureVariable(this Fsm fsm, string name)
		{
			FsmTexture val = fsm.FindTextureVariable(name);
			if (val != null)
			{
				return val;
			}
			return fsm.AddTextureVariable(name);
		}

		[PublicAPI]
		public static void MakeLog(this PlayMakerFSM fsm, bool additionalLogging = false)
		{
			fsm.Fsm.MakeLog(additionalLogging);
		}

		[PublicAPI]
		public static void MakeLog(this Fsm fsm, bool additionalLogging = false)
		{
			FsmState[] states = fsm.States;
			foreach (FsmState val in states)
			{
				for (int num = val.Actions.Length - 1; num >= 0; num--)
				{
					fsm.InsertAction(val.Name, (FsmStateAction)(object)new StatusLog
					{
						text = FsmString.op_Implicit($"{num} - {((object)val.Actions[num]).GetType().FullName}")
					}, num);
					if (additionalLogging)
					{
						fsm.InsertMethod(val.Name, delegate
						{
							//IL_0190: Unknown result type (might be due to invalid IL or missing references)
							//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
							//IL_0203: Unknown result type (might be due to invalid IL or missing references)
							//IL_0217: Unknown result type (might be due to invalid IL or missing references)
							//IL_022b: Unknown result type (might be due to invalid IL or missing references)
							//IL_027c: Unknown result type (might be due to invalid IL or missing references)
							//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
							//IL_0312: Unknown result type (might be due to invalid IL or missing references)
							FsmVariables variables = fsm.Variables;
							FsmFloat[] floatVariables = variables.FloatVariables;
							foreach (FsmFloat val2 in floatVariables)
							{
								InternalLogger.Log($"[{fsm.Name}]:[FloatVariables] - '{((NamedVariable)val2).Name}': '{val2.Value}'");
							}
							FsmInt[] intVariables = variables.IntVariables;
							foreach (FsmInt val3 in intVariables)
							{
								InternalLogger.Log($"[{fsm.Name}]:[IntVariables] - '{((NamedVariable)val3).Name}': '{val3.Value}'");
							}
							FsmBool[] boolVariables = variables.BoolVariables;
							foreach (FsmBool val4 in boolVariables)
							{
								InternalLogger.Log($"[{fsm.Name}]:[BoolVariables] - '{((NamedVariable)val4).Name}': '{val4.Value}'");
							}
							FsmString[] stringVariables = variables.StringVariables;
							foreach (FsmString val5 in stringVariables)
							{
								InternalLogger.Log("[" + fsm.Name + "]:[StringVariables] - '" + ((NamedVariable)val5).Name + "': '" + val5.Value + "'");
							}
							FsmVector2[] vector2Variables = variables.Vector2Variables;
							foreach (FsmVector2 val6 in vector2Variables)
							{
								InternalLogger.Log($"[{fsm.Name}]:[Vector2Variables] - '{((NamedVariable)val6).Name}': '({val6.Value.x}, {val6.Value.y})'");
							}
							FsmVector3[] vector3Variables = variables.Vector3Variables;
							foreach (FsmVector3 val7 in vector3Variables)
							{
								InternalLogger.Log($"[{fsm.Name}]:[Vector3Variables] - '{((NamedVariable)val7).Name}': '({val7.Value.x}, {val7.Value.y}, {val7.Value.z})'");
							}
							FsmRect[] rectVariables = variables.RectVariables;
							foreach (FsmRect val8 in rectVariables)
							{
								InternalLogger.Log($"[{fsm.Name}]:[RectVariables] - '{((NamedVariable)val8).Name}': '{val8.Value}'");
							}
							FsmQuaternion[] quaternionVariables = variables.QuaternionVariables;
							foreach (FsmQuaternion val9 in quaternionVariables)
							{
								InternalLogger.Log($"[{fsm.Name}]:[QuaternionVariables] - '{((NamedVariable)val9).Name}': '{val9.Value}'");
							}
							FsmColor[] colorVariables = variables.ColorVariables;
							foreach (FsmColor val10 in colorVariables)
							{
								InternalLogger.Log($"[{fsm.Name}]:[ColorVariables] - '{((NamedVariable)val10).Name}': '{val10.Value}'");
							}
							FsmGameObject[] gameObjectVariables = variables.GameObjectVariables;
							foreach (FsmGameObject val11 in gameObjectVariables)
							{
								InternalLogger.Log($"[{fsm.Name}]:[GameObjectVariables] - '{((NamedVariable)val11).Name}': '{val11.Value}'");
							}
							FsmArray[] arrayVariables = variables.ArrayVariables;
							foreach (FsmArray val12 in arrayVariables)
							{
								InternalLogger.Log($"[{fsm.Name}]:[ArrayVariables] - '{((NamedVariable)val12).Name}': '{val12}'");
							}
							FsmEnum[] enumVariables = variables.EnumVariables;
							foreach (FsmEnum val13 in enumVariables)
							{
								InternalLogger.Log($"[{fsm.Name}]:[EnumVariables] - '{((NamedVariable)val13).Name}': '{val13.Value}'");
							}
							FsmObject[] objectVariables = variables.ObjectVariables;
							foreach (FsmObject val14 in objectVariables)
							{
								InternalLogger.Log($"[{fsm.Name}]:[ObjectVariables] - '{((NamedVariable)val14).Name}': '{val14.Value}'");
							}
							FsmMaterial[] materialVariables = variables.MaterialVariables;
							foreach (FsmMaterial val15 in materialVariables)
							{
								InternalLogger.Log($"[{fsm.Name}]:[MaterialVariables] - '{((NamedVariable)val15).Name}': '{val15.Value}'");
							}
							FsmTexture[] textureVariables = variables.TextureVariables;
							foreach (FsmTexture val16 in textureVariables)
							{
								InternalLogger.Log($"[{fsm.Name}]:[TextureVariables] - '{((NamedVariable)val16).Name}': '{val16.Value}'");
							}
						}, num + 1);
					}
				}
			}
		}

		[PublicAPI]
		public static void Log(this PlayMakerFSM fsm)
		{
			fsm.Fsm.Log(((Object)((Component)fsm).gameObject).name);
		}

		[PublicAPI]
		public static void Log(this Fsm fsm, string gameObjectName = "")
		{
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			Log("FSM \"" + fsm.Name + "\" on \"" + gameObjectName + "\"");
			Log($"\t{fsm.States.Length} States");
			FsmState[] states = fsm.States;
			FsmTransition[] transitions;
			foreach (FsmState val in states)
			{
				Log("\t\tState \"" + val.Name + "\"");
				transitions = val.Transitions;
				foreach (FsmTransition val2 in transitions)
				{
					Log("\t\t\t-> \"" + val2.ToState + "\" via \"" + val2.EventName + "\"");
				}
			}
			Log($"\t{fsm.GlobalTransitions.Length} Global Transitions");
			transitions = fsm.GlobalTransitions;
			foreach (FsmTransition val3 in transitions)
			{
				Log("\t\tGlobal Transition \"" + val3.EventName + "\" to \"" + val3.ToState + "\"");
			}
			Log("\tVariables");
			FsmVariables variables = fsm.Variables;
			FsmFloat[] floatVariables = variables.FloatVariables;
			foreach (FsmFloat val4 in floatVariables)
			{
				Log($"\t\tFloat \"{((NamedVariable)val4).Name}\": {val4.Value}");
			}
			FsmInt[] intVariables = variables.IntVariables;
			foreach (FsmInt val5 in intVariables)
			{
				Log($"\t\tInt \"{((NamedVariable)val5).Name}\": {val5.Value}");
			}
			FsmBool[] boolVariables = variables.BoolVariables;
			foreach (FsmBool val6 in boolVariables)
			{
				Log($"\t\tBool \"{((NamedVariable)val6).Name}\": {val6.Value}");
			}
			FsmString[] stringVariables = variables.StringVariables;
			foreach (FsmString val7 in stringVariables)
			{
				Log("\t\tString \"" + ((NamedVariable)val7).Name + "\": " + val7.Value);
			}
			FsmVector2[] vector2Variables = variables.Vector2Variables;
			foreach (FsmVector2 val8 in vector2Variables)
			{
				Log($"\t\tVector2 \"{((NamedVariable)val8).Name}\": {val8.Value}");
			}
			FsmVector3[] vector3Variables = variables.Vector3Variables;
			foreach (FsmVector3 val9 in vector3Variables)
			{
				Log($"\t\tVector3 \"{((NamedVariable)val9).Name}\": {val9.Value}");
			}
			FsmRect[] rectVariables = variables.RectVariables;
			foreach (FsmRect val10 in rectVariables)
			{
				Log($"\t\tRect \"{((NamedVariable)val10).Name}\": {val10.Value}");
			}
			FsmQuaternion[] quaternionVariables = variables.QuaternionVariables;
			foreach (FsmQuaternion val11 in quaternionVariables)
			{
				Log($"\t\tQuaternion \"{((NamedVariable)val11).Name}\": {val11.Value}");
			}
			FsmColor[] colorVariables = variables.ColorVariables;
			foreach (FsmColor val12 in colorVariables)
			{
				Log($"\t\tColor \"{((NamedVariable)val12).Name}\": {val12.Value}");
			}
			FsmGameObject[] gameObjectVariables = variables.GameObjectVariables;
			foreach (FsmGameObject val13 in gameObjectVariables)
			{
				Log($"\t\tGameObject \"{((NamedVariable)val13).Name}\": {val13.Value}");
			}
			FsmArray[] arrayVariables = variables.ArrayVariables;
			foreach (FsmArray val14 in arrayVariables)
			{
				string[] obj = new string[5]
				{
					"\t\tArray \"",
					((NamedVariable)val14).Name,
					"\": \"",
					null,
					null
				};
				object[] objectReferences = val14.objectReferences;
				obj[3] = string.Join(", ", objectReferences);
				obj[4] = "\"";
				Log(string.Concat(obj));
			}
			FsmEnum[] enumVariables = variables.EnumVariables;
			foreach (FsmEnum val15 in enumVariables)
			{
				Log($"\t\tEnum \"{((NamedVariable)val15).Name}\": {val15.Value}");
			}
			FsmObject[] objectVariables = variables.ObjectVariables;
			foreach (FsmObject val16 in objectVariables)
			{
				Log($"\t\tObject \"{((NamedVariable)val16).Name}\": {val16.Value}");
			}
			FsmMaterial[] materialVariables = variables.MaterialVariables;
			foreach (FsmMaterial val17 in materialVariables)
			{
				Log($"\t\tMaterial \"{((NamedVariable)val17).Name}\": {val17.Value}");
			}
			FsmTexture[] textureVariables = variables.TextureVariables;
			foreach (FsmTexture val18 in textureVariables)
			{
				Log($"\t\tTexture \"{((NamedVariable)val18).Name}\": {val18.Value}");
			}
		}

		private static void Log(string goName, string fsmName, string part, string msg)
		{
			Logger.LogDebug("[" + goName + "]:[" + fsmName + "]:[" + part + "] - " + msg);
			Debug.Log((object)("[" + goName + "]:[" + fsmName + "]:[" + part + "] - " + msg));
		}

		private static void Log(string msg)
		{
			Logger.LogDebug("[SFCore]:[Util]:[FsmUtil]:" + msg);
			Debug.Log((object)("[SFCore]:[Util]:[FsmUtil]:" + msg));
		}

		private static void Log(object msg)
		{
			Log($"{msg}");
		}

		[PublicAPI]
		[Obsolete("Use method `GetState(PlayMakerFSM, string)` instead!")]
		public static FsmState GetFsmState(this PlayMakerFSM fsm, string stateName)
		{
			return fsm.GetState(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `GetState(Fsm, string)` instead!")]
		public static FsmState GetFsmState(this Fsm fsm, string stateName)
		{
			return fsm.GetState(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `GetTransition(PlayMakerFSM, string, string)` instead!")]
		public static FsmTransition GetFsmTransition(this PlayMakerFSM fsm, string stateName, string eventName)
		{
			return fsm.GetTransition(stateName, eventName);
		}

		[PublicAPI]
		[Obsolete("Use method `GetTransition(Fsm, string, string)` instead!")]
		public static FsmTransition GetFsmTransition(this Fsm fsm, string stateName, string eventName)
		{
			return fsm.GetTransition(stateName, eventName);
		}

		[PublicAPI]
		[Obsolete("Use method `GetTransition(FsmState, string)` instead!")]
		public static FsmTransition GetFsmTransition(this FsmState state, string eventName)
		{
			return state.GetTransition(eventName);
		}

		[PublicAPI]
		[Obsolete("Use method `GetGlobalTransition(PlayMakerFSM, string)` instead!")]
		public static FsmTransition GetFsmGlobalTransition(this PlayMakerFSM fsm, string globalEventName)
		{
			return fsm.GetGlobalTransition(globalEventName);
		}

		[PublicAPI]
		[Obsolete("Use method `GetGlobalTransition(Fsm, string)` instead!")]
		public static FsmTransition GetFsmGlobalTransition(this Fsm fsm, string globalEventName)
		{
			return fsm.GetGlobalTransition(globalEventName);
		}

		[PublicAPI]
		[Obsolete("Use method `GetAction<TAction>(PlayMakerFSM, string, int)` instead!")]
		public static TAction GetFsmAction<TAction>(this PlayMakerFSM fsm, string stateName, int index) where TAction : FsmStateAction
		{
			return fsm.GetAction<TAction>(stateName, index);
		}

		[PublicAPI]
		[Obsolete("Use method `GetAction<TAction>(Fsm, string, int)` instead!")]
		public static TAction GetFsmAction<TAction>(this Fsm fsm, string stateName, int index) where TAction : FsmStateAction
		{
			return fsm.GetAction<TAction>(stateName, index);
		}

		[PublicAPI]
		[Obsolete("Use method `GetAction<TAction>(FsmState, int)` instead!")]
		public static TAction GetFsmAction<TAction>(this FsmState state, int index) where TAction : FsmStateAction
		{
			return state.GetAction<TAction>(index);
		}

		[PublicAPI]
		[Obsolete("Use method `GetStateAction(PlayMakerFSM, string, int)` instead!")]
		public static FsmStateAction GetFsmStateAction(this PlayMakerFSM fsm, string stateName, int index)
		{
			return fsm.GetStateAction(stateName, index);
		}

		[PublicAPI]
		[Obsolete("Use method `GetStateAction(Fsm, string, int)` instead!")]
		public static FsmStateAction GetFsmStateAction(this Fsm fsm, string stateName, int index)
		{
			return fsm.GetStateAction(stateName, index);
		}

		[PublicAPI]
		[Obsolete("Use method `GetStateAction(FsmState, int)` instead!")]
		public static FsmStateAction GetFsmStateAction(this FsmState state, int index)
		{
			return state.GetStateAction(index);
		}

		[PublicAPI]
		[Obsolete("Use method `GetActionsOfType<TAction>(PlayMakerFSM, string)` instead!")]
		public static TAction[] GetFsmActionsOfType<TAction>(this PlayMakerFSM fsm, string stateName) where TAction : FsmStateAction
		{
			return fsm.GetActionsOfType<TAction>(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `GetActionsOfType<TAction>(Fsm, string)` instead!")]
		public static TAction[] GetFsmActionsOfType<TAction>(this Fsm fsm, string stateName) where TAction : FsmStateAction
		{
			return fsm.GetActionsOfType<TAction>(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `GetActionsOfType<TAction>(FsmState)` instead!")]
		public static TAction[] GetFsmActionsOfType<TAction>(this FsmState state) where TAction : FsmStateAction
		{
			return state.GetActionsOfType<TAction>();
		}

		[PublicAPI]
		[Obsolete("Use method `GetFirstActionOfType<TAction>(PlayMakerFSM, string)` instead!")]
		public static TAction GetFsmFirstActionOfType<TAction>(this PlayMakerFSM fsm, string stateName) where TAction : FsmStateAction
		{
			return fsm.GetFirstActionOfType<TAction>(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `GetFirstActionOfType<TAction>(Fsm, string)` instead!")]
		public static TAction GetFsmFirstActionOfType<TAction>(this Fsm fsm, string stateName) where TAction : FsmStateAction
		{
			return fsm.GetFirstActionOfType<TAction>(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `GetFirstActionOfType<TAction>(FsmState)` instead!")]
		public static TAction GetFsmFirstActionOfType<TAction>(this FsmState state) where TAction : FsmStateAction
		{
			return state.GetFirstActionOfType<TAction>();
		}

		[PublicAPI]
		[Obsolete("Use method `GetLastActionOfType<TAction>(PlayMakerFSM, string)` instead!")]
		public static TAction GetFsmLastActionOfType<TAction>(this PlayMakerFSM fsm, string stateName) where TAction : FsmStateAction
		{
			return fsm.GetLastActionOfType<TAction>(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `GetLastActionOfType<TAction>(Fsm, string)` instead!")]
		public static TAction GetFsmLastActionOfType<TAction>(this Fsm fsm, string stateName) where TAction : FsmStateAction
		{
			return fsm.GetLastActionOfType<TAction>(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `GetLastActionOfType<TAction>(FsmState)` instead!")]
		public static TAction GetFsmLastActionOfType<TAction>(this FsmState state) where TAction : FsmStateAction
		{
			return state.GetLastActionOfType<TAction>();
		}

		[PublicAPI]
		[Obsolete("Use method `AddState(PlayMakerFSM, string)` instead!")]
		public static FsmState AddFsmState(this PlayMakerFSM fsm, string stateName)
		{
			return fsm.AddState(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `AddState(Fsm, string)` instead!")]
		public static FsmState AddFsmState(this Fsm fsm, string stateName)
		{
			return fsm.AddState(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `AddState(PlayMakerFSM, FsmState)` instead!")]
		public static FsmState AddFsmState(this PlayMakerFSM fsm, FsmState state)
		{
			return fsm.AddState(state);
		}

		[PublicAPI]
		[Obsolete("Use method `AddState(Fsm, FsmState)` instead!")]
		public static FsmState AddFsmState(this Fsm fsm, FsmState state)
		{
			return fsm.AddState(state);
		}

		[PublicAPI]
		[Obsolete("Use method `CopyState(PlayMakerFSM, string, string)` instead!")]
		public static FsmState CopyFsmState(this PlayMakerFSM fsm, string fromState, string toState)
		{
			return fsm.CopyState(fromState, toState);
		}

		[PublicAPI]
		[Obsolete("Use method `CopyState(Fsm, string, string)` instead!")]
		public static FsmState CopyFsmState(this Fsm fsm, string fromState, string toState)
		{
			return fsm.CopyState(fromState, toState);
		}

		[PublicAPI]
		[Obsolete("Use method `AddTransition(PlayMakerFSM, string, string, string)` instead!")]
		public static FsmEvent AddFsmTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState)
		{
			return fsm.AddTransition(stateName, eventName, toState);
		}

		[PublicAPI]
		[Obsolete("Use method `AddTransition(Fsm, string, string, string)` instead!")]
		public static FsmEvent AddFsmTransition(this Fsm fsm, string stateName, string eventName, string toState)
		{
			return fsm.AddTransition(stateName, eventName, toState);
		}

		[PublicAPI]
		[Obsolete("Use method `AddTransition(FsmState, string, string)` instead!")]
		public static FsmEvent AddFsmTransition(this FsmState state, string eventName, string toState)
		{
			return state.AddTransition(eventName, toState);
		}

		[PublicAPI]
		[Obsolete("Use method `AddGlobalTransition(PlayMakerFSM, string, string)` instead!")]
		public static FsmEvent AddFsmGlobalTransition(this PlayMakerFSM fsm, string globalEventName, string toState)
		{
			return fsm.AddGlobalTransition(globalEventName, toState);
		}

		[PublicAPI]
		[Obsolete("Use method `AddGlobalTransition(Fsm, string, string)` instead!")]
		public static FsmEvent AddFsmGlobalTransition(this Fsm fsm, string globalEventName, string toState)
		{
			return fsm.AddGlobalTransition(globalEventName, toState);
		}

		[PublicAPI]
		[Obsolete("Use method `AddAction(PlayMakerFSM, string, FsmStateAction)` instead!")]
		public static void AddFsmAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action)
		{
			fsm.AddAction(stateName, action);
		}

		[PublicAPI]
		[Obsolete("Use method `AddAction(Fsm, string, FsmStateAction)` instead!")]
		public static void AddFsmAction(this Fsm fsm, string stateName, FsmStateAction action)
		{
			fsm.AddAction(stateName, action);
		}

		[PublicAPI]
		[Obsolete("Use method `AddAction(FsmState, FsmStateAction)` instead!")]
		public static void AddFsmAction(this FsmState state, FsmStateAction action)
		{
			state.AddAction(action);
		}

		[PublicAPI]
		[Obsolete("Use method `AddActions(PlayMakerFSM, string, params FsmStateAction[])` instead!")]
		public static void AddFsmActions(this PlayMakerFSM fsm, string stateName, params FsmStateAction[] actions)
		{
			fsm.AddActions(stateName, actions);
		}

		[PublicAPI]
		[Obsolete("Use method `AddActions(Fsm, string, params FsmStateAction[])` instead!")]
		public static void AddFsmActions(this Fsm fsm, string stateName, params FsmStateAction[] actions)
		{
			fsm.AddActions(stateName, actions);
		}

		[PublicAPI]
		[Obsolete("Use method `AddActions(FsmState, params FsmStateAction[])` instead!")]
		public static void AddFsmActions(this FsmState state, params FsmStateAction[] actions)
		{
			state.AddActions(actions);
		}

		[PublicAPI]
		[Obsolete("Use method `AddMethod(PlayMakerFSM, string, Action)` instead!")]
		public static void AddFsmMethod(this PlayMakerFSM fsm, string stateName, Action method)
		{
			fsm.AddMethod(stateName, method);
		}

		[PublicAPI]
		[Obsolete("Use method `AddMethod(Fsm, string, Action)` instead!")]
		public static void AddFsmMethod(this Fsm fsm, string stateName, Action method)
		{
			fsm.AddMethod(stateName, method);
		}

		[PublicAPI]
		[Obsolete("Use method `AddMethod(FsmState, Action)` instead!")]
		public static void AddFsmMethod(this FsmState state, Action method)
		{
			state.AddMethod(method);
		}

		[PublicAPI]
		[Obsolete("Use method `AddLambdaMethod(PlayMakerFSM, string, Action<Action>)` instead!")]
		public static void AddFsmLambdaMethod(this PlayMakerFSM fsm, string stateName, Action<Action> method)
		{
			fsm.AddLambdaMethod(stateName, method);
		}

		[PublicAPI]
		[Obsolete("Use method `AddLambdaMethod(Fsm, string, Action<Action>)` instead!")]
		public static void AddFsmLambdaMethod(this Fsm fsm, string stateName, Action<Action> method)
		{
			fsm.AddLambdaMethod(stateName, method);
		}

		[PublicAPI]
		[Obsolete("Use method `AddLambdaMethod(FsmState, Action<Action>)` instead!")]
		public static void AddFsmLambdaMethod(this FsmState state, Action<Action> method)
		{
			state.AddLambdaMethod(method);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertAction(PlayMakerFSM, string, FsmStateAction, int)` instead!")]
		public static void InsertFsmAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action, int index)
		{
			fsm.InsertAction(stateName, action, index);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertAction(PlayMakerFSM, string, int, FsmStateAction)` instead!")]
		public static void InsertFsmAction(this PlayMakerFSM fsm, string stateName, int index, FsmStateAction action)
		{
			fsm.InsertAction(stateName, index, action);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertAction(Fsm, string, FsmStateAction, int)` instead!")]
		public static void InsertFsmAction(this Fsm fsm, string stateName, FsmStateAction action, int index)
		{
			fsm.InsertAction(stateName, action, index);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertAction(Fsm, string, int, FsmStateAction)` instead!")]
		public static void InsertFsmAction(this Fsm fsm, string stateName, int index, FsmStateAction action)
		{
			fsm.InsertAction(stateName, index, action);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertAction(FsmState, FsmStateAction, int)` instead!")]
		public static void InsertFsmAction(this FsmState state, FsmStateAction action, int index)
		{
			state.InsertAction(action, index);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertAction(FsmState, int, FsmStateAction)` instead!")]
		public static void InsertFsmAction(this FsmState state, int index, FsmStateAction action)
		{
			state.InsertAction(index, action);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertActions(PlayMakerFSM, string, int, params FsmStateAction[])` instead!")]
		public static void InsertFsmActions(this PlayMakerFSM fsm, string stateName, int index, params FsmStateAction[] actions)
		{
			fsm.InsertActions(stateName, index, actions);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertActions(Fsm, string, int, params FsmStateAction[])` instead!")]
		public static void InsertFsmActions(this Fsm fsm, string stateName, int index, params FsmStateAction[] actions)
		{
			fsm.InsertActions(stateName, index, actions);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertActions(FsmState, int, params FsmStateAction[])` instead!")]
		public static void InsertFsmActions(this FsmState state, int index, params FsmStateAction[] actions)
		{
			state.InsertActions(index, actions);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertMethod(PlayMakerFSM, string, Action, int)` instead!")]
		public static void InsertFsmMethod(this PlayMakerFSM fsm, string stateName, Action method, int index)
		{
			fsm.InsertMethod(stateName, method, index);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertMethod(PlayMakerFSM, string, int, Action)` instead!")]
		public static void InsertFsmMethod(this PlayMakerFSM fsm, string stateName, int index, Action method)
		{
			fsm.InsertMethod(stateName, index, method);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertMethod(Fsm, string, Action, int)` instead!")]
		public static void InsertFsmMethod(this Fsm fsm, string stateName, Action method, int index)
		{
			fsm.InsertMethod(stateName, method, index);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertMethod(Fsm, string, int, Action)` instead!")]
		public static void InsertFsmMethod(this Fsm fsm, string stateName, int index, Action method)
		{
			fsm.InsertMethod(stateName, index, method);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertMethod(FsmState, Action, int)` instead!")]
		public static void InsertFsmMethod(this FsmState state, Action method, int index)
		{
			state.InsertMethod(method, index);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertMethod(FsmState, int, Action)` instead!")]
		public static void InsertFsmMethod(this FsmState state, int index, Action method)
		{
			state.InsertMethod(index, method);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertLambdaMethod(PlayMakerFSM, string, Action<Action>, int)` instead!")]
		public static void InsertFsmLambdaMethod(this PlayMakerFSM fsm, string stateName, Action<Action> method, int index)
		{
			fsm.InsertLambdaMethod(stateName, method, index);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertLambdaMethod(PlayMakerFSM, string, int, Action<Action>)` instead!")]
		public static void InsertFsmLambdaMethod(this PlayMakerFSM fsm, string stateName, int index, Action<Action> method)
		{
			fsm.InsertLambdaMethod(stateName, index, method);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertLambdaMethod(Fsm, string, Action<Action>, int)` instead!")]
		public static void InsertFsmLambdaMethod(this Fsm fsm, string stateName, Action<Action> method, int index)
		{
			fsm.InsertLambdaMethod(stateName, method, index);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertLambdaMethod(Fsm, string, int, Action<Action>)` instead!")]
		public static void InsertFsmLambdaMethod(this Fsm fsm, string stateName, int index, Action<Action> method)
		{
			fsm.InsertLambdaMethod(stateName, index, method);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertLambdaMethod(FsmState, Action<Action>, int)` instead!")]
		public static void InsertFsmLambdaMethod(this FsmState state, Action<Action> method, int index)
		{
			state.InsertLambdaMethod(method, index);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertLambdaMethod(FsmState, int, Action<Action>)` instead!")]
		public static void InsertFsmLambdaMethod(this FsmState state, int index, Action<Action> method)
		{
			state.InsertLambdaMethod(index, method);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertMethodBefore(FsmStateAction, Action)` instead!")]
		public static void InsertFsmMethodBefore(this FsmStateAction action, Action method)
		{
			action.InsertMethodBefore(method);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertMethodAfter(FsmStateAction, Action)` instead!")]
		public static void InsertFsmMethodAfter(this FsmStateAction action, Action method)
		{
			action.InsertMethodAfter(method);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertActionBefore(FsmStateAction, FsmStateAction)` instead!")]
		public static void InsertFsmActionBefore(this FsmStateAction action, FsmStateAction newAction)
		{
			action.InsertActionBefore(newAction);
		}

		[PublicAPI]
		[Obsolete("Use method `InsertActionAfter(FsmStateAction, FsmStateAction)` instead!")]
		public static void InsertFsmActionAfter(this FsmStateAction action, FsmStateAction newAction)
		{
			action.InsertActionAfter(newAction);
		}

		[PublicAPI]
		[Obsolete("Use method `ReplaceAction(PlayMakerFSM, string, FsmStateAction, int)` instead!")]
		public static void ReplaceFsmAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action, int index)
		{
			fsm.ReplaceAction(stateName, action, index);
		}

		[PublicAPI]
		[Obsolete("Use method `ReplaceAction(PlayMakerFSM, string, int, FsmStateAction)` instead!")]
		public static void ReplaceFsmAction(this PlayMakerFSM fsm, string stateName, int index, FsmStateAction action)
		{
			fsm.ReplaceAction(stateName, index, action);
		}

		[PublicAPI]
		[Obsolete("Use method `ReplaceAction(Fsm, string, FsmStateAction, int)` instead!")]
		public static void ReplaceFsmAction(this Fsm fsm, string stateName, FsmStateAction action, int index)
		{
			fsm.ReplaceAction(stateName, action, index);
		}

		[PublicAPI]
		[Obsolete("Use method `ReplaceAction(Fsm, string, int, FsmStateAction)` instead!")]
		public static void ReplaceFsmAction(this Fsm fsm, string stateName, int index, FsmStateAction action)
		{
			fsm.ReplaceAction(stateName, index, action);
		}

		[PublicAPI]
		[Obsolete("Use method `ReplaceAction(FsmState, FsmStateAction, int)` instead!")]
		public static void ReplaceFsmAction(this FsmState state, FsmStateAction action, int index)
		{
			state.ReplaceAction(action, index);
		}

		[PublicAPI]
		[Obsolete("Use method `ReplaceAction(FsmState, int, FsmStateAction)` instead!")]
		public static void ReplaceFsmAction(this FsmState state, int index, FsmStateAction action)
		{
			state.ReplaceAction(index, action);
		}

		[PublicAPI]
		[Obsolete("Use method `ReplaceAllActions(PlayMakerFSM, string, params FsmStateAction[])` instead!")]
		public static void ReplaceFsmAllActions(this PlayMakerFSM fsm, string stateName, params FsmStateAction[] actions)
		{
			fsm.ReplaceAllActions(stateName, actions);
		}

		[PublicAPI]
		[Obsolete("Use method `ReplaceAllActions(Fsm, string, params FsmStateAction[])` instead!")]
		public static void ReplaceFsmAllActions(this Fsm fsm, string stateName, params FsmStateAction[] actions)
		{
			fsm.ReplaceAllActions(stateName, actions);
		}

		[PublicAPI]
		[Obsolete("Use method `ReplaceAllActions(FsmState, params FsmStateAction[])` instead!")]
		public static void ReplaceFsmAllActions(this FsmState state, params FsmStateAction[] actions)
		{
			state.ReplaceAllActions(actions);
		}

		[PublicAPI]
		[Obsolete("Use method `ChangeTransition(PlayMakerFSM, string, string, string)` instead!")]
		public static void ChangeFsmTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState)
		{
			fsm.ChangeTransition(stateName, eventName, toState);
		}

		[PublicAPI]
		[Obsolete("Use method `ChangeTransition(Fsm, string, string, string)` instead!")]
		public static void ChangeFsmTransition(this Fsm fsm, string stateName, string eventName, string toState)
		{
			fsm.ChangeTransition(stateName, eventName, toState);
		}

		[PublicAPI]
		[Obsolete("Use method `ChangeTransition(FsmState, string, string)` instead!")]
		public static void ChangeFsmTransition(this FsmState state, string eventName, string toState)
		{
			state.ChangeTransition(eventName, toState);
		}

		[PublicAPI]
		[Obsolete("Use method `ChangeGlobalTransition(PlayMakerFSM, string, string)` instead!")]
		public static bool ChangeFsmGlobalTransition(this PlayMakerFSM fsm, string globalEventName, string toState)
		{
			return fsm.ChangeGlobalTransition(globalEventName, toState);
		}

		[PublicAPI]
		[Obsolete("Use method `ChangeGlobalTransition(Fsm, string, string)` instead!")]
		public static bool ChangeFsmGlobalTransition(this Fsm fsm, string globalEventName, string toState)
		{
			return fsm.ChangeGlobalTransition(globalEventName, toState);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveState(PlayMakerFSM, string)` instead!")]
		public static void RemoveFsmState(this PlayMakerFSM fsm, string stateName)
		{
			fsm.RemoveState(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveState(Fsm, string)` instead!")]
		public static void RemoveFsmState(this Fsm fsm, string stateName)
		{
			fsm.RemoveState(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveTransition(PlayMakerFSM, string, string)` instead!")]
		public static void RemoveFsmTransition(this PlayMakerFSM fsm, string stateName, string eventName)
		{
			fsm.RemoveTransition(stateName, eventName);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveTransition(Fsm, string, string)` instead!")]
		public static void RemoveFsmTransition(this Fsm fsm, string stateName, string eventName)
		{
			fsm.RemoveTransition(stateName, eventName);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveTransition(FsmState, string)` instead!")]
		public static void RemoveFsmTransition(this FsmState state, string eventName)
		{
			state.RemoveTransition(eventName);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveGlobalTransition(PlayMakerFSM, string)` instead!")]
		public static void RemoveFsmGlobalTransition(this PlayMakerFSM fsm, string globalEventName)
		{
			fsm.RemoveGlobalTransition(globalEventName);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveGlobalTransition(Fsm, string)` instead!")]
		public static void RemoveFsmGlobalTransition(this Fsm fsm, string globalEventName)
		{
			fsm.RemoveGlobalTransition(globalEventName);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveTransitionsTo(PlayMakerFSM, string)` instead!")]
		public static void RemoveFsmTransitionsTo(this PlayMakerFSM fsm, string toState)
		{
			fsm.RemoveTransitionsTo(toState);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveTransitionsTo(Fsm, string)` instead!")]
		public static void RemoveFsmTransitionsTo(this Fsm fsm, string toState)
		{
			fsm.RemoveTransitionsTo(toState);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveTransitionsTo(PlayMakerFSM, string, string)` instead!")]
		public static void RemoveFsmTransitionsTo(this PlayMakerFSM fsm, string stateName, string toState)
		{
			fsm.RemoveTransitionsTo(stateName, toState);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveTransitionsTo(Fsm, string, string)` instead!")]
		public static void RemoveFsmTransitionsTo(this Fsm fsm, string stateName, string toState)
		{
			fsm.RemoveTransitionsTo(stateName, toState);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveTransitionsTo(FsmState, string)` instead!")]
		public static void RemoveFsmTransitionsTo(this FsmState state, string toState)
		{
			state.RemoveTransitionsTo(toState);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveTransitions(PlayMakerFSM, string)` instead!")]
		public static void RemoveFsmTransitions(this PlayMakerFSM fsm, string stateName)
		{
			fsm.RemoveTransitions(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveTransitions(Fsm, string)` instead!")]
		public static void RemoveFsmTransitions(this Fsm fsm, string stateName)
		{
			fsm.RemoveTransitions(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveTransitions(FsmState)` instead!")]
		public static void RemoveFsmTransitions(this FsmState state)
		{
			state.RemoveTransitions();
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveAction(PlayMakerFSM, string, int)` instead!")]
		public static void RemoveFsmAction(this PlayMakerFSM fsm, string stateName, int index)
		{
			fsm.RemoveAction(stateName, index);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveAction(Fsm, string, int)` instead!")]
		public static void RemoveFsmAction(this Fsm fsm, string stateName, int index)
		{
			fsm.RemoveAction(stateName, index);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveAction(FsmState, int)` instead!")]
		public static void RemoveFsmAction(this FsmState state, int index)
		{
			state.RemoveAction(index);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveActionsOfType<TAction>(PlayMakerFSM)` instead!")]
		public static void RemoveFsmActionsOfType<TAction>(this PlayMakerFSM fsm)
		{
			fsm.RemoveActionsOfType<TAction>();
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveActionsOfType<TAction>(Fsm)` instead!")]
		public static void RemoveFsmActionsOfType<TAction>(this Fsm fsm)
		{
			fsm.RemoveActionsOfType<TAction>();
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveActionsOfType<TAction>(PlayMakerFSM, string)` instead!")]
		public static void RemoveFsmActionsOfType<TAction>(this PlayMakerFSM fsm, string stateName)
		{
			fsm.RemoveActionsOfType<TAction>(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveActionsOfType<TAction>(Fsm, string)` instead!")]
		public static void RemoveFsmActionsOfType<TAction>(this Fsm fsm, string stateName)
		{
			fsm.RemoveActionsOfType<TAction>(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveActionsOfType<TAction>(FsmState)` instead!")]
		public static void RemoveFsmActionsOfType<TAction>(this FsmState state)
		{
			state.RemoveActionsOfType<TAction>();
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveFirstActionOfType<TAction>(PlayMakerFSM, string)` instead!")]
		public static void RemoveFsmFirstActionOfType<TAction>(this PlayMakerFSM fsm, string stateName)
		{
			fsm.RemoveFirstActionOfType<TAction>(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveFirstActionOfType<TAction>(Fsm, string)` instead!")]
		public static void RemoveFsmFirstActionOfType<TAction>(this Fsm fsm, string stateName)
		{
			fsm.RemoveFirstActionOfType<TAction>(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveFirstActionOfType<TAction>(FsmState)` instead!")]
		public static void RemoveFsmFirstActionOfType<TAction>(this FsmState state)
		{
			state.RemoveFirstActionOfType<TAction>();
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveLastActionOfType<TAction>(PlayMakerFSM, string)` instead!")]
		public static void RemoveFsmLastActionOfType<TAction>(this PlayMakerFSM fsm, string stateName)
		{
			fsm.RemoveLastActionOfType<TAction>(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveLastActionOfType<TAction>(Fsm, string)` instead!")]
		public static void RemoveFsmLastActionOfType<TAction>(this Fsm fsm, string stateName)
		{
			fsm.RemoveLastActionOfType<TAction>(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `RemoveLastActionOfType<TAction>(FsmState)` instead!")]
		public static void RemoveFsmLastActionOfType<TAction>(this FsmState state)
		{
			state.RemoveLastActionOfType<TAction>();
		}

		[PublicAPI]
		[Obsolete("Use method `DisableAction(PlayMakerFSM, string, int)` instead!")]
		public static bool DisableFsmAction(this PlayMakerFSM fsm, string stateName, int index)
		{
			return fsm.DisableAction(stateName, index);
		}

		[PublicAPI]
		[Obsolete("Use method `DisableAction(Fsm, string, int)` instead!")]
		public static bool DisableFsmAction(this Fsm fsm, string stateName, int index)
		{
			return fsm.DisableAction(stateName, index);
		}

		[PublicAPI]
		[Obsolete("Use method `DisableAction(FsmState, int)` instead!")]
		public static bool DisableFsmAction(this FsmState state, int index)
		{
			return state.DisableAction(index);
		}

		[PublicAPI]
		[Obsolete("Use method `DisableActions(PlayMakerFSM, string, params int[])` instead!")]
		public static bool DisableFsmActions(this PlayMakerFSM fsm, string stateName, params int[] indices)
		{
			return fsm.DisableActions(stateName, indices);
		}

		[PublicAPI]
		[Obsolete("Use method `DisableActions(Fsm, string, params int[])` instead!")]
		public static bool DisableFsmActions(this Fsm fsm, string stateName, params int[] indices)
		{
			return fsm.DisableActions(stateName, indices);
		}

		[PublicAPI]
		[Obsolete("Use method `DisableActions(FsmState, params int[])` instead!")]
		public static bool DisableFsmActions(this FsmState state, params int[] indices)
		{
			return state.DisableActions(indices);
		}

		[PublicAPI]
		[Obsolete("Use method `DisableActionsOfType<TAction>(PlayMakerFSM)` instead!")]
		public static void DisableFsmActionsOfType<TAction>(this PlayMakerFSM fsm)
		{
			fsm.DisableActionsOfType<TAction>();
		}

		[PublicAPI]
		[Obsolete("Use method `DisableActionsOfType<TAction>(Fsm)` instead!")]
		public static void DisableFsmActionsOfType<TAction>(this Fsm fsm)
		{
			fsm.DisableActionsOfType<TAction>();
		}

		[PublicAPI]
		[Obsolete("Use method `DisableActionsOfType<TAction>(PlayMakerFSM, string)` instead!")]
		public static void DisableFsmActionsOfType<TAction>(this PlayMakerFSM fsm, string stateName)
		{
			fsm.DisableActionsOfType<TAction>(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `DisableActionsOfType<TAction>(Fsm, string)` instead!")]
		public static void DisableFsmActionsOfType<TAction>(this Fsm fsm, string stateName)
		{
			fsm.DisableActionsOfType<TAction>(stateName);
		}

		[PublicAPI]
		[Obsolete("Use method `DisableActionsOfType<TAction>(FsmState)` instead!")]
		public static void DisableFsmActionsOfType<TAction>(this FsmState state)
		{
			state.DisableActionsOfType<TAction>();
		}

		[PublicAPI]
		[Obsolete("Use method `AddFloatVariable(PlayMakerFSM, string)` instead!")]
		public static FsmFloat AddFsmFloatVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.AddFloatVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddFloatVariable(Fsm, string)` instead!")]
		public static FsmFloat AddFsmFloatVariable(this Fsm fsm, string name)
		{
			return fsm.AddFloatVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddIntVariable(PlayMakerFSM, string)` instead!")]
		public static FsmInt AddFsmIntVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.AddIntVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddIntVariable(Fsm, string)` instead!")]
		public static FsmInt AddFsmIntVariable(this Fsm fsm, string name)
		{
			return fsm.AddIntVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddBoolVariable(PlayMakerFSM, string)` instead!")]
		public static FsmBool AddFsmBoolVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.AddBoolVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddBoolVariable(Fsm, string)` instead!")]
		public static FsmBool AddFsmBoolVariable(this Fsm fsm, string name)
		{
			return fsm.AddBoolVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddStringVariable(PlayMakerFSM, string)` instead!")]
		public static FsmString AddFsmStringVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.AddStringVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddStringVariable(Fsm, string)` instead!")]
		public static FsmString AddFsmStringVariable(this Fsm fsm, string name)
		{
			return fsm.AddStringVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddVector2Variable(PlayMakerFSM, string)` instead!")]
		public static FsmVector2 AddFsmVector2Variable(this PlayMakerFSM fsm, string name)
		{
			return fsm.AddVector2Variable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddVector2Variable(Fsm, string)` instead!")]
		public static FsmVector2 AddFsmVector2Variable(this Fsm fsm, string name)
		{
			return fsm.AddVector2Variable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddVector3Variable(PlayMakerFSM, string)` instead!")]
		public static FsmVector3 AddFsmVector3Variable(this PlayMakerFSM fsm, string name)
		{
			return fsm.AddVector3Variable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddVector3Variable(Fsm, string)` instead!")]
		public static FsmVector3 AddFsmVector3Variable(this Fsm fsm, string name)
		{
			return fsm.AddVector3Variable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddRectVariable(PlayMakerFSM, string)` instead!")]
		public static FsmRect AddFsmRectVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.AddRectVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddRectVariable(Fsm, string)` instead!")]
		public static FsmRect AddFsmRectVariable(this Fsm fsm, string name)
		{
			return fsm.AddRectVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddQuaternionVariable(PlayMakerFSM, string)` instead!")]
		public static FsmQuaternion AddFsmQuaternionVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.AddQuaternionVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddQuaternionVariable(Fsm, string)` instead!")]
		public static FsmQuaternion AddFsmQuaternionVariable(this Fsm fsm, string name)
		{
			return fsm.AddQuaternionVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddColorVariable(PlayMakerFSM, string)` instead!")]
		public static FsmColor AddFsmColorVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.AddColorVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddColorVariable(Fsm, string)` instead!")]
		public static FsmColor AddFsmColorVariable(this Fsm fsm, string name)
		{
			return fsm.AddColorVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddGameObjectVariable(PlayMakerFSM, string)` instead!")]
		public static FsmGameObject AddFsmGameObjectVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.AddGameObjectVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddGameObjectVariable(Fsm, string)` instead!")]
		public static FsmGameObject AddFsmGameObjectVariable(this Fsm fsm, string name)
		{
			return fsm.AddGameObjectVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddArrayVariable(PlayMakerFSM, string)` instead!")]
		public static FsmArray AddFsmArrayVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.AddArrayVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddArrayVariable(Fsm, string)` instead!")]
		public static FsmArray AddFsmArrayVariable(this Fsm fsm, string name)
		{
			return fsm.AddArrayVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddEnumVariable(PlayMakerFSM, string)` instead!")]
		public static FsmEnum AddFsmEnumVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.AddEnumVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddEnumVariable(Fsm, string)` instead!")]
		public static FsmEnum AddFsmEnumVariable(this Fsm fsm, string name)
		{
			return fsm.AddEnumVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddObjectVariable(PlayMakerFSM, string)` instead!")]
		public static FsmObject AddFsmObjectVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.AddObjectVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddObjectVariable(Fsm, string)` instead!")]
		public static FsmObject AddFsmObjectVariable(this Fsm fsm, string name)
		{
			return fsm.AddObjectVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddMaterialVariable(PlayMakerFSM, string)` instead!")]
		public static FsmMaterial AddFsmMaterialVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.AddMaterialVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddMaterialVariable(Fsm, string)` instead!")]
		public static FsmMaterial AddFsmMaterialVariable(this Fsm fsm, string name)
		{
			return fsm.AddMaterialVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddTextureVariable(PlayMakerFSM, string)` instead!")]
		public static FsmTexture AddFsmTextureVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.AddTextureVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `AddTextureVariable(Fsm, string)` instead!")]
		public static FsmTexture AddFsmTextureVariable(this Fsm fsm, string name)
		{
			return fsm.AddTextureVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindFloatVariable(PlayMakerFSM, string)` instead!")]
		public static FsmFloat FindFsmFloatVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.FindFloatVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindFloatVariable(Fsm, string)` instead!")]
		public static FsmFloat FindFsmFloatVariable(this Fsm fsm, string name)
		{
			return fsm.FindFloatVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindIntVariable(PlayMakerFSM, string)` instead!")]
		public static FsmInt FindFsmIntVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.FindIntVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindIntVariable(Fsm, string)` instead!")]
		public static FsmInt FindFsmIntVariable(this Fsm fsm, string name)
		{
			return fsm.FindIntVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindBoolVariable(PlayMakerFSM, string)` instead!")]
		public static FsmBool FindFsmBoolVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.FindBoolVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindBoolVariable(Fsm, string)` instead!")]
		public static FsmBool FindFsmBoolVariable(this Fsm fsm, string name)
		{
			return fsm.FindBoolVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindStringVariable(PlayMakerFSM, string)` instead!")]
		public static FsmString FindFsmStringVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.FindStringVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindStringVariable(Fsm, string)` instead!")]
		public static FsmString FindFsmStringVariable(this Fsm fsm, string name)
		{
			return fsm.FindStringVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindVector2Variable(PlayMakerFSM, string)` instead!")]
		public static FsmVector2 FindFsmVector2Variable(this PlayMakerFSM fsm, string name)
		{
			return fsm.FindVector2Variable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindVector2Variable(Fsm, string)` instead!")]
		public static FsmVector2 FindFsmVector2Variable(this Fsm fsm, string name)
		{
			return fsm.FindVector2Variable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindVector3Variable(PlayMakerFSM, string)` instead!")]
		public static FsmVector3 FindFsmVector3Variable(this PlayMakerFSM fsm, string name)
		{
			return fsm.FindVector3Variable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindVector3Variable(Fsm, string)` instead!")]
		public static FsmVector3 FindFsmVector3Variable(this Fsm fsm, string name)
		{
			return fsm.FindVector3Variable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindRectVariable(PlayMakerFSM, string)` instead!")]
		public static FsmRect FindFsmRectVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.FindRectVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindRectVariable(Fsm, string)` instead!")]
		public static FsmRect FindFsmRectVariable(this Fsm fsm, string name)
		{
			return fsm.FindRectVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindQuaternionVariable(PlayMakerFSM, string)` instead!")]
		public static FsmQuaternion FindFsmQuaternionVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.FindQuaternionVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindQuaternionVariable(Fsm, string)` instead!")]
		public static FsmQuaternion FindFsmQuaternionVariable(this Fsm fsm, string name)
		{
			return fsm.FindQuaternionVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindColorVariable(PlayMakerFSM, string)` instead!")]
		public static FsmColor FindFsmColorVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.FindColorVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindColorVariable(Fsm, string)` instead!")]
		public static FsmColor FindFsmColorVariable(this Fsm fsm, string name)
		{
			return fsm.FindColorVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindGameObjectVariable(PlayMakerFSM, string)` instead!")]
		public static FsmGameObject FindFsmGameObjectVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.FindGameObjectVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindGameObjectVariable(Fsm, string)` instead!")]
		public static FsmGameObject FindFsmGameObjectVariable(this Fsm fsm, string name)
		{
			return fsm.FindGameObjectVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindArrayVariable(PlayMakerFSM, string)` instead!")]
		public static FsmArray FindFsmArrayVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.FindArrayVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindArrayVariable(Fsm, string)` instead!")]
		public static FsmArray FindFsmArrayVariable(this Fsm fsm, string name)
		{
			return fsm.FindArrayVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindEnumVariable(PlayMakerFSM, string)` instead!")]
		public static FsmEnum FindFsmEnumVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.FindEnumVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindEnumVariable(Fsm, string)` instead!")]
		public static FsmEnum FindFsmEnumVariable(this Fsm fsm, string name)
		{
			return fsm.FindEnumVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindObjectVariable(PlayMakerFSM, string)` instead!")]
		public static FsmObject FindFsmObjectVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.FindObjectVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindObjectVariable(Fsm, string)` instead!")]
		public static FsmObject FindFsmObjectVariable(this Fsm fsm, string name)
		{
			return fsm.FindObjectVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindMaterialVariable(PlayMakerFSM, string)` instead!")]
		public static FsmMaterial FindFsmMaterialVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.FindMaterialVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindMaterialVariable(Fsm, string)` instead!")]
		public static FsmMaterial FindFsmMaterialVariable(this Fsm fsm, string name)
		{
			return fsm.FindMaterialVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindTextureVariable(PlayMakerFSM, string)` instead!")]
		public static FsmTexture FindFsmTextureVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.FindTextureVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `FindTextureVariable(Fsm, string)` instead!")]
		public static FsmTexture FindFsmTextureVariable(this Fsm fsm, string name)
		{
			return fsm.FindTextureVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetFloatVariable(PlayMakerFSM, string)` instead!")]
		public static FsmFloat GetFsmFloatVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.GetFloatVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetFloatVariable(Fsm, string)` instead!")]
		public static FsmFloat GetFsmFloatVariable(this Fsm fsm, string name)
		{
			return fsm.GetFloatVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetIntVariable(PlayMakerFSM, string)` instead!")]
		public static FsmInt GetFsmIntVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.GetIntVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetIntVariable(Fsm, string)` instead!")]
		public static FsmInt GetFsmIntVariable(this Fsm fsm, string name)
		{
			return fsm.GetIntVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetBoolVariable(PlayMakerFSM, string)` instead!")]
		public static FsmBool GetFsmBoolVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.GetBoolVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetBoolVariable(Fsm, string)` instead!")]
		public static FsmBool GetFsmBoolVariable(this Fsm fsm, string name)
		{
			return fsm.GetBoolVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetStringVariable(PlayMakerFSM, string)` instead!")]
		public static FsmString GetFsmStringVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.GetStringVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetStringVariable(Fsm, string)` instead!")]
		public static FsmString GetFsmStringVariable(this Fsm fsm, string name)
		{
			return fsm.GetStringVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetVector2Variable(PlayMakerFSM, string)` instead!")]
		public static FsmVector2 GetFsmVector2Variable(this PlayMakerFSM fsm, string name)
		{
			return fsm.GetVector2Variable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetVector2Variable(Fsm, string)` instead!")]
		public static FsmVector2 GetFsmVector2Variable(this Fsm fsm, string name)
		{
			return fsm.GetVector2Variable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetVector3Variable(PlayMakerFSM, string)` instead!")]
		public static FsmVector3 GetFsmVector3Variable(this PlayMakerFSM fsm, string name)
		{
			return fsm.GetVector3Variable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetVector3Variable(Fsm, string)` instead!")]
		public static FsmVector3 GetFsmVector3Variable(this Fsm fsm, string name)
		{
			return fsm.GetVector3Variable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetRectVariable(PlayMakerFSM, string)` instead!")]
		public static FsmRect GetFsmRectVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.GetRectVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetRectVariable(Fsm, string)` instead!")]
		public static FsmRect GetFsmRectVariable(this Fsm fsm, string name)
		{
			return fsm.GetRectVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetQuaternionVariable(PlayMakerFSM, string)` instead!")]
		public static FsmQuaternion GetFsmQuaternionVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.GetQuaternionVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetQuaternionVariable(Fsm, string)` instead!")]
		public static FsmQuaternion GetFsmQuaternionVariable(this Fsm fsm, string name)
		{
			return fsm.GetQuaternionVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetColorVariable(PlayMakerFSM, string)` instead!")]
		public static FsmColor GetFsmColorVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.GetColorVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetColorVariable(Fsm, string)` instead!")]
		public static FsmColor GetFsmColorVariable(this Fsm fsm, string name)
		{
			return fsm.GetColorVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetGameObjectVariable(PlayMakerFSM, string)` instead!")]
		public static FsmGameObject GetFsmGameObjectVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.GetGameObjectVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetGameObjectVariable(Fsm, string)` instead!")]
		public static FsmGameObject GetFsmGameObjectVariable(this Fsm fsm, string name)
		{
			return fsm.GetGameObjectVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetArrayVariable(PlayMakerFSM, string)` instead!")]
		public static FsmArray GetFsmArrayVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.GetArrayVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetArrayVariable(Fsm, string)` instead!")]
		public static FsmArray GetFsmArrayVariable(this Fsm fsm, string name)
		{
			return fsm.GetArrayVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetEnumVariable(PlayMakerFSM, string)` instead!")]
		public static FsmEnum GetFsmEnumVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.GetEnumVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetEnumVariable(Fsm, string)` instead!")]
		public static FsmEnum GetFsmEnumVariable(this Fsm fsm, string name)
		{
			return fsm.GetEnumVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetObjectVariable(PlayMakerFSM, string)` instead!")]
		public static FsmObject GetFsmObjectVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.GetObjectVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetObjectVariable(Fsm, string)` instead!")]
		public static FsmObject GetFsmObjectVariable(this Fsm fsm, string name)
		{
			return fsm.GetObjectVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetMaterialVariable(PlayMakerFSM, string)` instead!")]
		public static FsmMaterial GetFsmMaterialVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.GetMaterialVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetMaterialVariable(Fsm, string)` instead!")]
		public static FsmMaterial GetFsmMaterialVariable(this Fsm fsm, string name)
		{
			return fsm.GetMaterialVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetTextureVariable(PlayMakerFSM, string)` instead!")]
		public static FsmTexture GetFsmTextureVariable(this PlayMakerFSM fsm, string name)
		{
			return fsm.GetTextureVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `GetTextureVariable(Fsm, string)` instead!")]
		public static FsmTexture GetFsmTextureVariable(this Fsm fsm, string name)
		{
			return fsm.GetTextureVariable(name);
		}

		[PublicAPI]
		[Obsolete("Use method `MakeLog(PlayMakerFSM, bool)` instead!")]
		public static void MakeFsmLog(this PlayMakerFSM fsm, bool additionalLogging = false)
		{
			fsm.MakeLog();
		}

		[PublicAPI]
		[Obsolete("Use method `MakeLog(Fsm, bool)` instead!")]
		public static void MakeFsmLog(this Fsm fsm, bool additionalLogging = false)
		{
			fsm.MakeLog(additionalLogging);
		}

		[PublicAPI]
		[Obsolete("Use method `Log(PlayMakerFSM)` instead!")]
		public static void LogFsm(this PlayMakerFSM fsm)
		{
			fsm.Log();
		}

		[PublicAPI]
		[Obsolete("Use method `Log(Fsm, string)` instead!")]
		public static void LogFsm(this Fsm fsm, string gameObjectName = "")
		{
			fsm.Log(gameObjectName);
		}

		[PublicAPI]
		[Obsolete("Use method `AddGlobalTransition(PlayMakerFSM, string, string)` instead!")]
		public static FsmEvent AddFsmGlobalTransitions(this PlayMakerFSM fsm, string globalEventName, string toState)
		{
			return fsm.AddGlobalTransition(globalEventName, toState);
		}
	}
	public class FunctionAction<TArg> : FsmStateAction
	{
		public Action<TArg> action;

		public TArg arg;

		public override void Reset()
		{
			action = null;
			((FsmStateAction)this).Reset();
		}

		public override void OnEnter()
		{
			if (this.action != null && arg != null)
			{
				this.action(arg);
			}
			if (!(arg is Action action) || action != new Action(base.Finish))
			{
				((FsmStateAction)this).Finish();
			}
		}
	}
	public class LanguageStrings
	{
		private readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> _jsonDict;

		public LanguageStrings(Assembly asm, string resourceName, Encoding encoding = null)
		{
			using Stream stream = asm.GetManifestResourceStream(resourceName);
			if (stream != null)
			{
				byte[] array = new byte[stream.Length];
				stream.Read(array, 0, array.Length);
				stream.Dispose();
				string text = ((encoding == null) ? Encoding.Default : encoding).GetString(array);
				_jsonDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(text);
			}
		}

		public LanguageStrings(string json)
		{
			_jsonDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(json);
		}

		public unsafe string Get(string key, string sheet)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			SupportedLanguages gameLanguage = GameManager.instance.gameSettings.gameLanguage;
			string text = ((!_jsonDict.ContainsKey(((object)(*(SupportedLanguages*)(&gameLanguage))/*cast due to constrained. prefix*/).ToString())) ? _jsonDict[((object)(SupportedLanguages)44/*cast due to constrained. prefix*/).ToString()][sheet][key] : (_jsonDict[((object)(*(SupportedLanguages*)(&gameLanguage))/*cast due to constrained. prefix*/).ToString()].ContainsKey(sheet) ? _jsonDict[((object)(*(SupportedLanguages*)(&gameLanguage))/*cast due to constrained. prefix*/).ToString()][sheet][key] : _jsonDict[((object)(SupportedLanguages)44/*cast due to constrained. prefix*/).ToString()][sheet][key]));
			return text.Replace("<br>", "\n");
		}

		public unsafe bool ContainsKey(string key, string sheet)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)GameManager.instance == (Object)null)
			{
				return false;
			}
			SupportedLanguages gameLanguage = GameManager.instance.gameSettings.gameLanguage;
			if (!_jsonDict.ContainsKey(((object)(*(SupportedLanguages*)(&gameLanguage))/*cast due to constrained. prefix*/).ToString()))
			{
				if (!_jsonDict[((object)(SupportedLanguages)44/*cast due to constrained. prefix*/).ToString()].ContainsKey(sheet))
				{
					return false;
				}
				return _jsonDict[((object)(SupportedLanguages)44/*cast due to constrained. prefix*/).ToString()][sheet].ContainsKey(key);
			}
			if (!_jsonDict[((object)(*(SupportedLanguages*)(&gameLanguage))/*cast due to constrained. prefix*/).ToString()].ContainsKey(sheet))
			{
				if (!_jsonDict[((object)(SupportedLanguages)44/*cast due to constrained. prefix*/).ToString()].ContainsKey(sheet))
				{
					return false;
				}
				return _jsonDict[((object)(SupportedLanguages)44/*cast due to constrained. prefix*/).ToString()][sheet].ContainsKey(key);
			}
			return _jsonDict[((object)(*(SupportedLanguages*)(&gameLanguage))/*cast due to constrained. prefix*/).ToString()][sheet].ContainsKey(key);
		}
	}
	public class LogAction : FsmStateAction
	{
		public FsmString text;

		public override void Reset()
		{
			text = FsmString.op_Implicit(string.Empty);
			((FsmStateAction)this).Reset();
		}

		public override void OnEnter()
		{
			if (text.Value != null)
			{
				Log("FSM Log: \"" + text.Value + "\"");
			}
			((FsmStateAction)this).Finish();
		}

		private void Log(string message)
		{
			Debug.Log((object)("[SFCore]:[Util]:[LogAction] - " + message));
			Logger.LogDebug("[SFCore]:[Util]:[LogAction] - " + message);
		}
	}
	public class StatusLog : FsmStateAction
	{
		public FsmString text;

		public override void Reset()
		{
			text = FsmString.op_Implicit(string.Empty);
			((FsmStateAction)this).Reset();
		}

		public override void OnEnter()
		{
			if (text.Value != null)
			{
				Log(text.Value ?? "");
			}
			((FsmStateAction)this).Finish();
		}

		private void Log(string message)
		{
			string text = ((FsmStateAction)this).Fsm.GameObjectName;
			Transform parent = ((FsmStateAction)this).Fsm.GameObject.transform.parent;
			while ((Object)(object)parent != (Object)null)
			{
				text = ((Object)((Component)parent).gameObject).name + "/" + text;
				parent = parent.parent;
			}
			Debug.Log((object)("[" + text + "]:[" + ((FsmStateAction)this).Fsm.Name + "]:[" + ((FsmStateAction)this).State.Name + "] - " + message));
			Logger.LogDebug("[" + text + "]:[" + ((FsmStateAction)this).Fsm.Name + "]:[" + ((FsmStateAction)this).State.Name + "] - " + message);
		}
	}
	public class MethodAction : FsmStateAction
	{
		public Action method;

		public override void Reset()
		{
			method = null;
			((FsmStateAction)this).Reset();
		}

		public override void OnEnter()
		{
			if (method != null)
			{
				method();
			}
			((FsmStateAction)this).Finish();
		}
	}
	public static class MiscCreator
	{
		private static AudioMixer _musicAm;

		private static AudioMixer _atmosAm;

		private static AudioMixer _enviroAm;

		private static AudioMixer _actorAm;

		private static AudioMixer _shadeAm;

		private static void InitAudioMixers()
		{
			if ((Object)(object)_musicAm == (Object)null)
			{
				_musicAm = Resources.FindObjectsOfTypeAll<AudioMixer>().First((AudioMixer x) => ((Object)x).name == "Music");
			}
			if ((Object)(object)_atmosAm == (Object)null)
			{
				_atmosAm = Resources.FindObjectsOfTypeAll<AudioMixer>().First((AudioMixer x) => ((Object)x).name == "Atmos");
			}
			if ((Object)(object)_enviroAm == (Object)null)
			{
				_enviroAm = Resources.FindObjectsOfTypeAll<AudioMixer>().First((AudioMixer x) => ((Object)x).name == "EnviroEffects");
			}
			if ((Object)(object)_actorAm == (Object)null)
			{
				_actorAm = Resources.FindObjectsOfTypeAll<AudioMixer>().First((AudioMixer x) => ((Object)x).name == "Actors");
			}
			if ((Object)(object)_shadeAm == (Object)null)
			{
				_shadeAm = Resources.FindObjectsOfTypeAll<AudioMixer>().First((AudioMixer x) => ((Object)x).name == "ShadeMixer");
			}
		}

		public static void ResetSceneManagerAudio(SceneManager sm)
		{
			InitAudioMixers();
			sm.SetAttr<SceneManager, AudioMixerSnapshot>("musicSnapshot", _musicAm.FindSnapshot("Silent"));
			sm.atmosSnapshot = _atmosAm.FindSnapshot("at None");
			sm.enviroSnapshot = _enviroAm.FindSnapshot("en Silent");
			sm.actorSnapshot = _actorAm.FindSnapshot("On");
			sm.shadeSnapshot = _shadeAm.FindSnapshot("Away");
			sm.SetAttr<SceneManager, AtmosCue>("atmosCue", Resources.FindObjectsOfTypeAll<AtmosCue>().First((AtmosCue x) => ((Object)x).name == "None"));
			sm.SetAttr<SceneManager, MusicCue>("musicCue", Resources.FindObjectsOfTypeAll<MusicCue>().First((MusicCue x) => ((Object)x).name == "None"));
			sm.SetAttr<SceneManager, MusicCue>("infectedMusicCue", Resources.FindObjectsOfTypeAll<MusicCue>().First((MusicCue x) => ((Object)x).name == "None"));
		}

		public static void Set(this Vector3 self, double x, double y, double z)
		{
			self.x = (float)x;
			self.y = (float)y;
			self.y = (float)z;
		}

		public static void setOriginalMusicEventName(this MusicCue cue, string name)
		{
			cue.SetAttr<MusicCue, string>("originalMusicEventName", name);
		}

		public static void setOriginalMusicTrackNumber(this MusicCue cue, int number)
		{
			cue.SetAttr<MusicCue, int>("originalMusicTrackNumber", number);
		}

		public static void setSnapshot(this MusicCue cue, AudioMixerSnapshot snapshot)
		{
			cue.SetAttr<MusicCue, AudioMixerSnapshot>("snapshot", snapshot);
		}

		public static MusicChannelInfo[] getChannelInfos(this MusicCue cue)
		{
			return cue.GetAttr<MusicCue, MusicChannelInfo[]>("channelInfos");
		}

		public static void setChannelInfos(this MusicCue cue, MusicChannelInfo[] infos)
		{
			cue.SetAttr<MusicCue, MusicChannelInfo[]>("channelInfos", infos);
		}
	}
	public static class SpriteUtil
	{
		private delegate Color Texture2D_GetPixelImpl(int image, int x, int y);

		private static MethodInfo tex_GetPixelImpl = typeof(Texture2D).GetMethod("GetPixelImpl", BindingFlags.Instance | BindingFlags.NonPublic);

		private static Texture2D_GetPixelImpl getGetPixel(Texture2D tex)
		{
			return (Texture2D_GetPixelImpl)tex_GetPixelImpl.CreateDelegate(typeof(Texture2D_GetPixelImpl), tex);
		}

		public static float CalcTriangleArea(Vector2Int a, Vector2Int b, Vector2Int c)
		{
			return Mathf.Abs((float)(((Vector2Int)(ref a)).x * (((Vector2Int)(ref b)).y - ((Vector2Int)(ref c)).y) + ((Vector2Int)(ref b)).x * (((Vector2Int)(ref c)).y - ((Vector2Int)(ref a)).y) + ((Vector2Int)(ref c)).x * (((Vector2Int)(ref a)).y - ((Vector2Int)(ref b)).y)) / 2f);
		}

		public static Texture2D GetReadableCroppedTexture(Texture2D tex, int x, int y, int width, int height)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected O, but got Unknown
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			float num = (float)width / (float)((Texture)tex).width;
			float num2 = (float)height / (float)((Texture)tex).height;
			float num3 = (float)x / (float)((Texture)tex).width;
			float num4 = (float)y / (float)((Texture)tex).height;
			Texture2D val = new Texture2D(width, height);
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, (RenderTextureFormat)7, (RenderTextureReadWrite)1);
			Graphics.Blit((Texture)(object)tex, temporary, new Vector2(num, num2), new Vector2(num3, num4));
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = temporary;
			val.ReadPixels(new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), 0, 0);
			val.Apply();
			RenderTexture.active = active;
			RenderTexture.ReleaseTemporary(temporary);
			return val;
		}

		public static Texture2D ExtractTextureFromSprite(Sprite sprite)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			(int, int) tuple = (((Texture)sprite.texture).width, ((Texture)sprite.texture).height);
			List<Vector2Int> list = new List<Vector2Int>();
			List<(Vector2Int, Vector2Int, Vector2Int)> list2 = new List<(Vector2Int, Vector2Int, Vector2Int)>();
			Vector2[] uv = sprite.uv;
			foreach (Vector2 val in uv)
			{
				list.Add(new Vector2Int(Mathf.RoundToInt(val.x * (float)(tuple.Item1 - 1)), Mathf.RoundToInt(val.y * (float)(tuple.Item2 - 1))));
			}
			for (int j = 0; j < sprite.triangles.Length; j += 3)
			{
				list2.Add((list[sprite.triangles[j]], list[sprite.triangles[j + 1]], list[sprite.triangles[j + 2]]));
			}
			int num = list.Min((Vector2Int val3) => ((Vector2Int)(ref val3)).x);
			int num2 = list.Max((Vector2Int val3) => ((Vector2Int)(ref val3)).x);
			int num3 = list.Min((Vector2Int val3) => ((Vector2Int)(ref val3)).y);
			int num4 = list.Max((Vector2Int val3) => ((Vector2Int)(ref val3)).y);
			int num5 = num2 - num + 1;
			int num6 = num4 - num3 + 1;
			bool[][] array = new bool[num6][];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = new bool[num5];
			}
			foreach (var item in list2)
			{
				float num7 = CalcTriangleArea(item.Item1, item.Item2, item.Item3);
				for (int num8 = 0; num8 < num5; num8++)
				{
					for (int num9 = 0; num9 < num6; num9++)
					{
						Vector2Int val2 = new Vector2Int(num + num8, num3 + num9);
						float num10 = CalcTriangleArea(item.Item1, item.Item2, val2);
						float num11 = CalcTriangleArea(val2, item.Item2, item.Item3);
						float num12 = CalcTriangleArea(item.Item1, val2, item.Item3);
						if (num10 + num11 + num12 == num7)
						{
							array[num9][num8] = true;
						}
					}
				}
			}
			Texture2D readableCroppedTexture = GetReadableCroppedTexture(sprite.texture, num, num3, num5, num6);
			for (int num8 = 0; num8 < num5; num8++)
			{
				for (int num9 = 0; num9 < num6; num9++)
				{
					if (!array[num9][num8])
					{
						readableCroppedTexture.SetPixel(num8, num9, new Color(0f, 0f, 0f, 0f));
					}
				}
			}
			readableCroppedTexture.Apply();
			return readableCroppedTexture;
		}
	}
	public static class USceneUtil
	{
		public static GameObject FindRoot(this Scene scene, string name)
		{
			if (((Scene)(ref scene)).IsValid())
			{
				GameObject[] rootGameObjects = ((Scene)(ref scene)).GetRootGameObjects();
				int num = rootGameObjects.Length;
				for (int i = 0; i < num; i++)
				{
					if (((Object)rootGameObjects[i]).name == name)
					{
						return rootGameObjects[i];
					}
				}
			}
			return null;
		}

		public static GameObject Find(this Scene scene, string name)
		{
			if (((Scene)(ref scene)).IsValid())
			{
				GameObject[] rootGameObjects = ((Scene)(ref scene)).GetRootGameObjects();
				int num = rootGameObjects.Length;
				for (int i = 0; i < num; i++)
				{
					if (((Object)rootGameObjects[i]).name == name)
					{
						return rootGameObjects[i];
					}
					GameObject val = rootGameObjects[i].Find(name);
					if ((Object)(object)val != (Object)null)
					{
						return val;
					}
				}
			}
			return null;
		}

		public static GameObject FindGameObjectInChildren(this GameObject o, string name)
		{
			return o.Find(name);
		}

		public static GameObject Find(this GameObject o, string name)
		{
			if ((Object)(object)o == (Object)null)
			{
				return null;
			}
			int childCount = o.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				GameObject gameObject = ((Component)o.transform.GetChild(i)).gameObject;
				if (name == ((Object)gameObject).name)
				{
					return gameObject;
				}
			}
			for (int j = 0; j < o.transform.childCount; j++)
			{
				GameObject val = ((Component)o.transform.GetChild(j)).gameObject.Find(name);
				if ((Object)(object)val != (Object)null)
				{
					return val;
				}
			}
			return null;
		}

		public static void Log(this Scene scene)
		{
			Logger.LogDebug("[SceneLog] - Scene \"" + ((Scene)(ref scene)).name + "\"");
			GameObject[] rootGameObjects = ((Scene)(ref scene)).GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				rootGameObjects[i].transform.Log();
			}
		}

		public static void Log(this Transform go, string n = "\t")
		{
			Logger.LogDebug("[SceneLog] - " + n + "\"" + ((Object)go).name + "\"");
			Component[] components = ((Component)go).GetComponents<Component>();
			foreach (Component val in components)
			{
				Logger.LogDebug($"[SceneLog] - {n} => \"{((object)val).GetType()}\": {val}");
			}
			for (int j = 0; j < go.childCount; j++)
			{
				go.GetChild(j).Log(n + "\t");
			}
		}
	}
	public static class Util
	{
		public static void SetAttr<TSelf, TVal>(this TSelf o, string fieldname, TVal value)
		{
			ReflectionHelper.SetField<TSelf, TVal>(o, fieldname, value);
		}

		public static TVal GetAttr<TSelf, TVal>(this TSelf o, string fieldname)
		{
			return ReflectionHelper.GetField<TSelf, TVal>(o, fieldname);
		}

		public static string GetVersion(Assembly asm)
		{
			string text = asm.GetName().Version.ToString();
			SHA1 sHA = SHA1.Create();
			FileStream fileStream = File.OpenRead(asm.Location);
			string text2 = BitConverter.ToString(sHA.ComputeHash(fileStream)).Replace("-", "").ToLowerInvariant();
			fileStream.Close();
			sHA.Clear();
			return text + "-" + text2.Substring(0, 6);
		}
	}
	public class WavUtils
	{
		public static AudioClip ToAudioClip(Stream dataStream, string origName = "")
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			WavData.Inspect(dataStream, (Action<string>)delegate(string msg)
			{
				InternalLogger.LogDebug(msg);
			});
			WavData val = new WavData();
			val.Parse(dataStream, (Action<string>)delegate(string msg)
			{
				InternalLogger.LogDebug(msg);
			});
			dataStream.Close();
			float[] samples = val.GetSamples();
			AudioClip obj = AudioClip.Create(origName, samples.Length / val.FormatChunk.NumChannels, (int)val.FormatChunk.NumChannels, (int)val.FormatChunk.SampleRate, false);
			obj.SetData(samples, 0);
			return obj;
		}

		public static AudioClip ToAudioClip(string filePath)
		{
			return ToAudioClip(File.Open(filePath, FileMode.Open), Path.GetFileNameWithoutExtension(filePath));
		}

		public static AudioClip ToAudioClip(byte[] wavData, string origName = "")
		{
			return ToAudioClip(new MemoryStream(wavData, writable: false), origName);
		}
	}
}
namespace SFCore.MonoBehaviours
{
	[RequireComponent(typeof(MeshRenderer))]
	public class BlurPlanePatcher : MonoBehaviour
	{
		private static Material[] _blurPlaneMaterials;

		private static bool _initialized;

		public void Start()
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			if (!_initialized)
			{
				_initialized = true;
				_blurPlaneMaterials = (Material[])(object)new Material[1];
				_blurPlaneMaterials[0] = new Material(Shader.Find("UI/Blur/UIBlur"));
				_blurPlaneMaterials[0].SetColor(Shader.PropertyToID("_TintColor"), new Color(1f, 1f, 1f, 0f));
				_blurPlaneMaterials[0].SetFloat(Shader.PropertyToID("_Size"), 53.7f);
				_blurPlaneMaterials[0].SetFloat(Shader.PropertyToID("_Vibrancy"), 0.2f);
				_blurPlaneMaterials[0].SetInt(Shader.PropertyToID("_StencilComp"), 8);
				_blurPlaneMaterials[0].SetInt(Shader.PropertyToID("_Stencil"), 0);
				_blurPlaneMaterials[0].SetInt(Shader.PropertyToID("_StencilOp"), 0);
				_blurPlaneMaterials[0].SetInt(Shader.PropertyToID("_StencilWriteMask"), 255);
				_blurPlaneMaterials[0].SetInt(Shader.PropertyToID("_StencilReadMask"), 255);
			}
			BlurPlane val = ((Component)this).gameObject.GetComponent<BlurPlane>();
			if ((Object)(object)val == (Object)null)
			{
				val = ((Component)this).gameObject.AddComponent<BlurPlane>();
			}
			MeshRenderer component = ((Component)this).gameObject.GetComponent<MeshRenderer>();
			((Renderer)component).materials = _blurPlaneMaterials;
			((Renderer)component).material = _blurPlaneMaterials[0];
			val.SetPlaneMaterial(((Renderer)component).materials[0]);
			val.SetPlaneVisibility(true);
		}
	}
	public static class CueHolder
	{
		public static Dictionary<string, MusicCue> MusicCues = new Dictionary<string, MusicCue>();

		public static Dictionary<string, AtmosCue> AtmosCues = new Dictionary<string, AtmosCue>();

		public static MusicCue GetMusicCue(string set, AudioMixerSnapshot snapshot, AudioClip[] clips, SceneManagerPatcher.MusicChannelSync[] syncs)
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected O, but got Unknown
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Expected O, but got Unknown
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Expected O, but got Unknown
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Expected O, but got Unknown
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Expected O, but got Unknown
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Expected O, but got Unknown
			if (set == "")
			{
				return null;
			}
			if (!MusicCues.ContainsKey(set))
			{
				MusicCue val = ScriptableObject.CreateInstance<MusicCue>();
				val.SetAttr<MusicCue, string>("originalMusicEventName", "CROSSROADS");
				val.SetAttr<MusicCue, int>("originalMusicTrackNumber", 2);
				val.SetAttr<MusicCue, AudioMixerSnapshot>("snapshot", snapshot);
				val.SetAttr<MusicCue, Alternative[]>("alternatives", null);
				MusicChannelInfo[] array = (MusicChannelInfo[])(object)new MusicChannelInfo[6]
				{
					new MusicChannelInfo(),
					new MusicChannelInfo(),
					new MusicChannelInfo(),
					new MusicChannelInfo(),
					new MusicChannelInfo(),
					new MusicChannelInfo()
				};
				array[0].SetAttr<MusicChannelInfo, AudioClip>("clip", clips[0]);
				array[2].SetAttr<MusicChannelInfo, AudioClip>("clip", clips[2]);
				array[3].SetAttr<MusicChannelInfo, AudioClip>("clip", clips[3]);
				array[4].SetAttr<MusicChannelInfo, AudioClip>("clip", clips[4]);
				array[1].SetAttr<MusicChannelInfo, AudioClip>("clip", clips[1]);
				array[5].SetAttr<MusicChannelInfo, AudioClip>("clip", clips[5]);
				array[0].SetAttr<MusicChannelInfo, MusicChannelSync>("sync", (MusicChannelSync)syncs[0]);
				array[2].SetAttr<MusicChannelInfo, MusicChannelSync>("sync", (MusicChannelSync)syncs[2]);
				array[3].SetAttr<MusicChannelInfo, MusicChannelSync>("sync", (MusicChannelSync)syncs[3]);
				array[4].SetAttr<MusicChannelInfo, MusicChannelSync>("sync", (MusicChannelSync)syncs[4]);
				array[1].SetAttr<MusicChannelInfo, MusicChannelSync>("sync", (MusicChannelSync)syncs[1]);
				array[5].SetAttr<MusicChannelInfo, MusicChannelSync>("sync", (MusicChannelSync)syncs[5]);
				val.SetAttr<MusicCue, MusicChannelInfo[]>("channelInfos", array);
				MusicCues.Add(set, val);
			}
			return MusicCues[set];
		}

		public static AtmosCue GetAtmosCue(string set, AudioMixerSnapshot snapshot, bool[] isChannelEnabled)
		{
			if (set == "")
			{
				return null;
			}
			if (!AtmosCues.ContainsKey(set))
			{
				AtmosCue val = ScriptableObject.CreateInstance<AtmosCue>();
				val.SetAttr<AtmosCue, AudioMixerSnapshot>("snapshot", snapshot);
				val.SetAttr<AtmosCue, bool[]>("isChannelEnabled", isChannelEnabled);
				AtmosCues.Add(set, val);
			}
			return AtmosCues[set];
		}
	}
	public class CustomItemList : MonoBehaviour
	{
		private GameObject[] _currentList = (GameObject[])(object)new GameObject[0];

		private PlayerData _pd = PlayerData.instance;

		private bool _built;

		public static CustomItemList Instance { get; private set; }

		public List<ItemHelper.Item> List { get; private set; } = new List<ItemHelper.Item>();

		public GameObject[] ListInv { get; private set; } = (GameObject[])(object)new GameObject[0];

		public float YDistance { get; private set; } = -2f;

		public int ItemCount { get; private set; } = -1;

		public int FirstNewItem { get; private set; }

		public CustomItemList()
		{
			Instance = this;
		}

		public bool hasAtLeastOneItem()
		{
			if (_currentList.Length != 0)
			{
				return (Object)(object)_currentList[0] != (Object)null;
			}
			return false;
		}

		public int TotalItemAmount()
		{
			return List.Count;
		}

		public int GotItemAmount()
		{
			return ItemCount + 1;
		}

		public void BuildItemList()
		{
			_pd = PlayerData.instance;
			FirstNewItem = -1;
			ItemCount = -1;
			ListInv = (GameObject[])(object)new GameObject[List.Count];
			GameObject val = Object.Instantiate<GameObject>(((Component)Resources.FindObjectsOfTypeAll<JournalEntryStats>().First((JournalEntryStats x) => ((Object)((Component)x).gameObject).name.Equals("Journal Crawler"))).gameObject);
			Object.Destroy((Object)(object)val.GetComponent<JournalEntryStats>());
			val.FindGameObjectInChildren("Name").GetComponent<SetTextMeshProGameText>().sheetName = "UI";
			for (int num = 0; num < List.Count; num++)
			{
				ItemCount++;
				GameObject val2 = MakeGameObject(List[num], val);
				val2.transform.SetParent(((Component)this).transform, false);
				val2.SetActive(false);
				ListInv[ItemCount] = val2;
			}
			AddToFadeGroup();
			_built = true;
		}

		public void UpdateItemList()
		{
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			if (!_built)
			{
				BuildItemList();
			}
			FirstNewItem = 0;
			ItemCount = -1;
			float num = 0f;
			_currentList = (GameObject[])(object)new GameObject[ListInv.Length];
			for (int i = 0; i < ListInv.Length; i++)
			{
				GameObject val = ListInv[i];
				MakeSpritesText(val, List[i]);
				bool flag = CheckBool(List[i]);
				val.SetActive(flag);
				if (flag)
				{
					ItemCount++;
					_currentList[ItemCount] = val;
					val.transform.localPosition = new Vector3(0f, num, 0f);
					num += YDistance;
				}
				else
				{
					val.transform.localPosition = new Vector3(-500f, -500f, -500f);
				}
			}
		}

		private GameObject MakeGameObject(ItemHelper.Item item, GameObject prefab)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = Object.Instantiate<GameObject>(prefab);
			((Object)val).name = item.uniqueName;
			val.transform.localPosition = Vector3.zero;
			MakeSpritesText(val, item);
			val.SetActive(true);
			val.Find("Portrait").SetActive(true);
			val.Find("Name").SetActive(true);
			return val;
		}

		private void MakeSpritesText(GameObject go, ItemHelper.Item item)
		{
			SpriteRenderer component = go.FindGameObjectInChildren("Portrait").GetComponent<SpriteRenderer>();
			SetTextMeshProGameText component2 = go.FindGameObjectInChildren("Name").GetComponent<SetTextMeshProGameText>();
			switch (item.type)
			{
			case ItemType.Normal:
				component.sprite = item.sprite1;
				component2.convName = item.nameConvo1;
				break;
			case ItemType.OneTwo:
				if (_pd.GetBool(item.playerdataBool1))
				{
					component.sprite = item.sprite1;
					component2.convName = item.nameConvo1;
				}
				else
				{
					component.sprite = item.sprite2;
					component2.convName = item.nameConvo2;
				}
				break;
			case ItemType.OneTwoBoth:
				if (_pd.GetBool(item.playerdataBool1) && !_pd.GetBool(item.playerdataBool2))
				{
					component.sprite = item.sprite1;
					component2.convName = item.nameConvo1;
				}
				else if (!_pd.GetBool(item.playerdataBool1) && _pd.GetBool(item.playerdataBool2))
				{
					component.sprite = item.sprite2;
					component2.convName = item.nameConvo2;
				}
				else
				{
					component.sprite = item.spriteBoth;
					component2.convName = item.nameConvoBoth;
				}
				break;
			case ItemType.Counted:
				component.sprite = item.sprite1;
				component2.convName = item.nameConvo1;
				break;
			case ItemType.Flower:
				if (_pd.GetBool(item.playerdataInt))
				{
					component.sprite = item.sprite2;
					component2.convName = item.nameConvo2;
				}
				else
				{
					component.sprite = item.sprite1;
					component2.convName = item.nameConvo1;
				}
				break;
			}
		}

		private bool CheckBool(ItemHelper.Item item)
		{
			switch (item.type)
			{
			case ItemType.Normal:
				return _pd.GetBool(item.playerdataBool1);
			case ItemType.OneTwo:
				return _pd.GetBool(item.playerdataBool1) ^ _pd.GetBool(item.playerdataBool2);
			case ItemType.OneTwoBoth:
				if (!_pd.GetBool(item.playerdataBool1))
				{
					return _pd.GetBool(item.playerdataBool2);
				}
				return true;
			case ItemType.Counted:
				return _pd.GetInt(item.playerdataInt) > 0;
			case ItemType.Flower:
				if (_pd.GetBool(item.playerdataBool1))
				{
					if (_pd.GetBool(item.playerdataBool2))
					{
						return !_pd.GetBool(item.playerdataInt);
					}
					return true;
				}
				return false;
			default:
				return false;
			}
		}

		public int GetItemCount()
		{
			return ItemCount;
		}

		public string GetDescConvo(int itemNum)
		{
			ItemHelper.Item item = List.First((ItemHelper.Item x) => x.uniqueName.Equals(((Object)_currentList[itemNum]).name));
			switch (item.type)
			{
			case ItemType.Normal:
				return item.descConvo1;
			case ItemType.OneTwo:
				if (_pd.GetBool(item.playerdataBool1))
				{
					return item.descConvo1;
				}
				return item.descConvo2;
			case ItemType.OneTwoBoth:
				if (_pd.GetBool(item.playerdataBool1) && !_pd.GetBool(item.playerdataBool2))
				{
					return item.descConvo1;
				}
				if (!_pd.GetBool(item.playerdataBool1) && _pd.GetBool(item.playerdataBool2))
				{
					return item.descConvo2;
				}
				return item.descConvoBoth;
			case ItemType.Counted:
				return item.descConvo1;
			case ItemType.Flower:
				if (_pd.GetBool(item.playerdataInt))
				{
					if (_pd.GetBool(item.playerdataBool2))
					{
						return item.descConvoBoth;
					}
					return item.descConvo2;
				}
				if (_pd.GetBool(item.playerdataBool2))
				{
					return item.nameConvoBoth;
				}
				return item.descConvo1;
			default:
				return "";
			}
		}

		public string GetNameConvo(int itemNum)
		{
			ItemHelper.Item item = List.First((ItemHelper.Item x) => x.uniqueName.Equals(((Object)_currentList[itemNum]).name));
			switch (item.type)
			{
			case ItemType.Normal:
				return item.nameConvo1;
			case ItemType.OneTwo:
				if (_pd.GetBool(item.playerdataBool1))
				{
					return item.nameConvo1;
				}
				return item.nameConvo2;
			case ItemType.OneTwoBoth:
				if (_pd.GetBool(item.playerdataBool1) && !_pd.GetBool(item.playerdataBool2))
				{
					return item.nameConvo1;
				}
				if (!_pd.GetBool(item.playerdataBool1) && _pd.GetBool(item.playerdataBool2))
				{
					return item.nameConvo2;
				}
				return item.nameConvoBoth;
			case ItemType.Counted:
				return item.nameConvo1;
			case ItemType.Flower:
				if (_pd.GetBool(item.playerdataInt))
				{
					return item.nameConvo2;
				}
				return item.nameConvo1;
			default:
				return "";
			}
		}

		public Sprite GetSprite(int itemNum)
		{
			return _currentList[itemNum].GetComponentInChildren<SpriteRenderer>().sprite;
		}

		public float GetYDistance()
		{
			return YDistance;
		}

		public int GetFirstNewItem()
		{
			return FirstNewItem;
		}

		public string GetPlayerDataKillsName(int itemNum)
		{
			ItemHelper.Item item = List.First((ItemHelper.Item x) => x.uniqueName.Equals(((Object)_currentList[itemNum]).name));
			if (item.type != ItemType.Counted)
			{
				return "0Return";
			}
			return item.playerdataInt;
		}

		private void AddToFadeGroup()
		{
			FadeGroup component = ((Component)((Component)this).transform.parent).GetComponent<FadeGroup>();
			List<SpriteRenderer> list = new List<SpriteRenderer>(component.spriteRenderers);
			List<TextMeshPro> list2 = new List<TextMeshPro>(component.texts);
			GameObject[] listInv = ListInv;
			foreach (GameObject val in listInv)
			{
				SpriteRenderer[] componentsInChildren = val.GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer item in componentsInChildren)
				{
					list.Add(item);
				}
				TextMeshPro[] componentsInChildren2 = val.GetComponentsInChildren<TextMeshPro>();
				foreach (TextMeshPro item2 in componentsInChildren2)
				{
					list2.Add(item2);
				}
			}
			component.spriteRenderers = list.ToArray();
			component.texts = list2.ToArray();
		}

		private static void Log(string message)
		{
			Logger.LogDebug("[SFCore]:[CustomItemList] - " + message);
			Debug.Log((object)("[SFCore]:[CustomItemList] - " + message));
		}

		private static void Log(object message)
		{
			Log($"{message}");
		}
	}
	[RequireComponent(typeof(BoxCollider2D))]
	public class PatchMusicRegions : MonoBehaviour
	{
		private static AudioMixer _am;

		public bool useAlts;

		public static bool altMusic;

		public string SnapshotName = "Silent";

		public int SnapshotIndex = 9;

		public string MusicRegionSet = "";

		public AudioClip Main;

		public AudioClip Action;

		public AudioClip Sub;

		public AudioClip Tension;

		public AudioClip MainAlt;

		public AudioClip Extra;

		public AudioClip Main2;

		public AudioClip Action2;

		public AudioClip Sub2;

		public AudioClip Tension2;

		public AudioClip MainAlt2;

		public AudioClip Extra2;

		public bool Dirtmouth;

		public bool MinesDelay;

		public string EnterTrackEvent = "CROSSROADS";

		public float EnterTransitionTime = 3f;

		public void Start()
		{
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Expected O, but got Unknown
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Expected O, but got Unknown
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Expected O, but got Unknown
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Expected O, but got Unknown
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Expected O, but got Unknown
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Expected O, but got Unknown
			((Component)this).gameObject.SetActive(false);
			if ((Object)(object)_am == (Object)null)
			{
				_am = Resources.FindObjectsOfTypeAll<AudioMixer>().First((AudioMixer x) => ((Object)x).name == "Music");
			}
			AudioMixerSnapshot val = _am.FindSnapshot(SnapshotName);
			MusicRegion val2 = ((Component)this).gameObject.AddComponent<MusicRegion>();
			val2.enterMusicSnapshot = val;
			val2.dirtmouth = Dirtmouth;
			val2.minesDelay = MinesDelay;
			val2.enterTrackEvent = EnterTrackEvent;
			val2.enterTransitionTime = EnterTransitionTime;
			val2.exitMusicCue = null;
			val2.exitMusicSnapshot = null;
			val2.exitTrackEvent = "";
			val2.exitTransitionTime = 0f;
			if (!CueHolder.MusicCues.ContainsKey(MusicRegionSet))
			{
				MusicCue val3 = ScriptableObject.CreateInstance<MusicCue>();
				val3.SetAttr<MusicCue, string>("originalMusicEventName", EnterTrackEvent);
				val3.SetAttr<MusicCue, int>("originalMusicTrackNumber", 2);
				val3.SetAttr<MusicCue, AudioMixerSnapshot>("snapshot", val);
				val3.SetAttr<MusicCue, Alternative[]>("alternatives", null);
				CueHolder.MusicCues.Add(MusicRegionSet, val3);
			}
			val2.enterMusicCue = CueHolder.MusicCues[MusicRegionSet];
			MusicChannelInfo[] array = (MusicChannelInfo[])(object)new MusicChannelInfo[6]
			{
				new MusicChannelInfo(),
				new MusicChannelInfo(),
				new MusicChannelInfo(),
				new MusicChannelInfo(),
				new MusicChannelInfo(),
				new MusicChannelInfo()
			};
			array[0].SetAttr<MusicChannelInfo, MusicChannelSync>("sync", (MusicChannelSync)0);
			array[2].SetAttr<MusicChannelInfo, MusicChannelSync>("sync", (MusicChannelSync)1);
			array[3].SetAttr<MusicChannelInfo, MusicChannelSync>("sync", (MusicChannelSync)1);
			array[4].SetAttr<MusicChannelInfo, MusicChannelSync>("sync", (MusicChannelSync)1);
			array[1].SetAttr<MusicChannelInfo, MusicChannelSync>("sync", (MusicChannelSync)1);
			array[5].SetAttr<MusicChannelInfo, MusicChannelSync>("sync", (MusicChannelSync)1);
			if (!altMusic || !useAlts)
			{
				array[0].SetAttr<MusicChannelInfo, AudioClip>("clip", Main);
				array[2].SetAttr<MusicChannelInfo, AudioClip>("clip", Action);
				array[3].SetAttr<MusicChannelInfo, AudioClip>("clip", Sub);
				array[4].SetAttr<MusicChannelInfo, AudioClip>("clip", Tension);
				array[1].SetAttr<MusicChannelInfo, AudioClip>("clip", MainAlt);
				array[5].SetAttr<MusicChannelInfo, AudioClip>("clip", Extra);
			}
			else if (useAlts)
			{
				array[0].SetAttr<MusicChannelInfo, AudioClip>("clip", Main2);
				array[2].SetAttr<MusicChannelInfo, AudioClip>("clip", Action2);
				array[3].SetAttr<MusicChannelInfo, AudioClip>("clip", Sub2);
				array[4].SetAttr<MusicChannelInfo, AudioClip>("clip", Tension2);
				array[1].SetAttr<MusicChannelInfo, AudioClip>("clip", MainAlt2);
				array[5].SetAttr<MusicChannelInfo, AudioClip>("clip", Extra2);
			}
			val2.enterMusicCue.SetAttr<MusicCue, MusicChannelInfo[]>("channelInfos", array);
			((Component)this).gameObject.SetActive(true);
		}
	}
	public class SceneManagerPatcher : MonoBehaviour
	{
		public enum EnviromentType
		{
			Dust,
			Grass,
			Bone,
			Spa,
			Metal,
			NoEffect,
			Wet
		}

		public enum DarknessLevel
		{
			Undark1 = -1,
			Normal,
			Dark1,
			Dark2
		}

		public enum MusicChoices
		{
			Normal,
			NormalAlt,
			NormalSoft,
			NormalSofter,
			NormalFlange,
			NormalFlangier,
			Action,
			ActionAndSub,
			SubArea,
			Silent,
			SilentFlange,
			Off,
			TensionOnly,
			NormalGramaphone,
			ActionOnly,
			MainOnly,
			HK_DECLINE2,
			HK_DECLINE3,
			HK_DECLINE4,
			HK_DECLINE5,
			HK_DECLINE6
		}

		public enum AtmosChoices
		{
			None,
			Cave,
			Surface,
			SurfaceInterior,
			SurfaceBasement,
			SurfaceNook,
			RainyIndoors,
			RainyOutdoors,
			DistantRain,
			DistantRainRoom,
			Greenpath,
			QueensGardens,
			Fungus,
			FogCanyon,
			WaterwaysFlowing,
			Waterways,
			GreenpathInterior,
			FogCanyonMinor,
			MinesCrystal,
			MinesMachinery,
			Deepnest,
			DeepnestQuiet,
			WindTunnel,
			MiscWind
		}

		public enum EnviroChoices
		{
			Cave,
			Spa,
			Cliffs,
			Room,
			Arena,
			Sewerpipe,
			FogCanyon,
			Dream,
			Silent
		}

		public enum ActorChoices
		{
			On,
			Off
		}

		public enum ShadeChoices
		{
			Away,
			Close
		}

		public enum AtmosChannels
		{
			CaveWind,
			SurfaceWind1,
			GrassyWind,
			SurfaceWind2,
			CaveNoises,
			RainIndoor,
			RainOutdoor,
			Greenpath,
			Fungus,
			FogCanyon,
			Waterways,
			WaterfallMed,
			MinesCrystal,
			MinesMachinery,
			Deepnest,
			MiscWind
		}

		public enum MusicChannelSync
		{
			Implicit,
			ExplicitOn,
			ExplicitOff
		}

		private static AudioMixer _musicAm;

		private static AudioMixer _atmosAm;

		private static AudioMixer _enviroAm;

		private static AudioMixer _actorAm;

		private static AudioMixer _shadeAm;

		private static GameObject _borderPrefab;

		private static GameObject _hollowShadePrefab;

		private static GameObject _dreamgatePrefab;

		[Header("Gameplay Scene Settings")]
		[Tooltip("The area of the map this scene belongs to.")]
		[Space(6f)]
		public MapZone mapZone;

		[Tooltip("Determines if this area is currently windy.")]
		public bool isWindy;

		[Tooltip("Determines if this level experiences tremors.")]
		public bool isTremorZone;

		[Tooltip("Set environment type on scene entry.")]
		public EnviromentType environmentType;

		[Tooltip("Set darkness level on scene entry.")]
		public DarknessLevel darknessLevel;

		[Tooltip("Determines if the lantern is deactivated.")]
		public bool noLantern;

		[Header("Camera Color Correction Curves")]
		[Range(0f, 5f)]
		public float saturation;

		public bool ignorePlatformSaturationModifiers;

		public AnimationCurve redChannel;

		public AnimationCurve greenChannel;

		public AnimationCurve blueChannel;

		[Header("Ambient Light")]
		[Tooltip("The default ambient light colour for this scene.")]
		[Space(6f)]
		public Color defaultColor;

		[Tooltip("The intensity of the ambient light in this scene.")]
		[Range(0f, 1f)]
		public float defaultIntensity;

		[Header("Hero Light")]
		[Tooltip("Color of the hero's light gradient (not point lights)")]
		[Space(6f)]
		public Color heroLightColor;

		[Header("Scene Particles")]
		public bool noParticles;

		public MapZone overrideParticlesWith;

		[Header("Audio Snapshots")]
		[Space(6f)]
		public string AtmosCueSet;

		public string AtmosCueSnapshotName;

		public AtmosChoices AtmosCueSnapshotIndex;

		[ArrayForEnum(typeof(AtmosChannels))]
		public bool[] AtmosCueIsChannelEnabled;

		public string MusicCueSet;

		public string MusicCueSnapshotName;

		public MusicChoices MusicCueSnapshotIndex;

		[ArrayForEnum(typeof(MusicChannels))]
		public AudioClip[] MusicCueChannelInfoClips;

		[ArrayForEnum(typeof(MusicChannels))]
		public MusicChannelSync[] MusicCueChannelInfoSyncs;

		public string InfectedMusicCueSet;

		public string InfectedMusicCueSnapshotName;

		public MusicChoices InfectedMusicCueSnapshotIndex;

		[ArrayForEnum(typeof(MusicChannels))]
		public AudioClip[] InfectedMusicCueChannelInfoClips;

		[ArrayForEnum(typeof(MusicChannels))]
		public MusicChannelSync[] InfectedMusicCueChannelInfoSyncs;

		public string MsSnapshotName;

		public MusicChoices MsSnapshotIndex;

		public float musicDelayTime;

		public float musicTransitionTime;

		public string AtsSnapshotName;

		public AtmosChoices AtsSnapshotIndex;

		public string EsSnapshotName;

		public EnviroChoices EsSnapshotIndex;

		public string AcsSnapshotName;

		public ActorChoices AcsSnapshotIndex;

		public string SsSnapshotName;

		public ShadeChoices SsSnapshotIndex;

		public float transitionTime;

		[Header("Mapping")]
		[Space(6f)]
		public bool manualMapTrigger;

		internal static void LoadPrefabs(SceneManager template)
		{
			_borderPrefab = template.borderPrefab;
			_hollowShadePrefab = template.hollowShadeObject;
			_dreamgatePrefab = template.dreamgateObject;
		}

		public void Awake()
		{
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)_musicAm == (Object)null)
			{
				AudioMixer[] source = Resources.FindObjectsOfTypeAll<AudioMixer>();
				_musicAm = source.First((AudioMixer x) => ((Object)x).name == "Music");
				_atmosAm = source.First((AudioMixer x) => ((Object)x).name == "Atmos");
				_enviroAm = source.First((AudioMixer x) => ((Object)x).name == "EnviroEffects");
				_actorAm = source.First((AudioMixer x) => ((Object)x).name == "Actors");
				_shadeAm = source.First((AudioMixer x) => ((Object)x).name == "ShadeMixer");
			}
			GameObject val = new GameObject("_SceneManager");
			val.SetActive(false);
			SceneManager obj = val.AddComponent<SceneManager>();
			obj.sceneType = (SceneType)0;
			obj.mapZone = mapZone;
			obj.isWindy = isWindy;
			obj.isTremorZone = isTremorZone;
			obj.environmentType = (int)environmentType;
			obj.darknessLevel = (int)darknessLevel;
			obj.noLantern = noLantern;
			obj.saturation = saturation;
			obj.ignorePlatformSaturationModifiers = ignorePlatformSaturationModifiers;
			obj.redChannel = redChannel;
			obj.greenChannel = greenChannel;
			obj.blueChannel = blueChannel;
			obj.defaultColor = defaultColor;
			obj.defaultIntensity = defaultIntensity;
			obj.heroLightColor = heroLightColor;
			obj.noParticles = noParticles;
			obj.overrideParticlesWith = overrideParticlesWith;
			obj.SetAttr<SceneManager, AtmosCue>("atmosCue", CueHolder.GetAtmosCue(AtmosCueSet, _atmosAm.FindSnapshot(AtmosCueSnapshotName), AtmosCueIsChannelEnabled));
			obj.SetAttr<SceneManager, MusicCue>("musicCue", CueHolder.GetMusicCue(MusicCueSet, _musicAm.FindSnapshot(MusicCueSnapshotName), MusicCueChannelInfoClips, MusicCueChannelInfoSyncs));
			obj.SetAttr<SceneManager, MusicCue>("infectedMusicCue", CueHolder.GetMusicCue(InfectedMusicCueSet, _musicAm.FindSnapshot(InfectedMusicCueSnapshotName), InfectedMusicCueChannelInfoClips, InfectedMusicCueChannelInfoSyncs));
			obj.SetAttr<SceneManager, AudioMixerSnapshot>("musicSnapshot", _musicAm.FindSnapshot(MsSnapshotName));
			obj.SetAttr<SceneManager, float>("musicDelayTime", musicDelayTime);
			obj.SetAttr<SceneManager, float>("musicTransitionTime", musicTransitionTime);
			obj.atmosSnapshot = _atmosAm.FindSnapshot(AtsSnapshotName);
			obj.enviroSnapshot = _enviroAm.FindSnapshot(EsSnapshotName);
			obj.actorSnapshot = _actorAm.FindSnapshot(AcsSnapshotName);
			obj.shadeSnapshot = _shadeAm.FindSnapshot(SsSnapshotName);
			obj.transitionTime = transitionTime;
			obj.borderPrefab = _borderPrefab;
			obj.manualMapTrigger = manualMapTrigger;
			obj.hollowShadeObject = _hollowShadePrefab;
			obj.dreamgateObject = _dreamgatePrefab;
			val.SetActive(true);
			val.tag = "SceneManager";
			GameManager instance = GameManager.instance;
			if (instance != null)
			{
				instance.SetupSceneRefs(false);
			}
		}

		public SceneManagerPatcher()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			mapZone = (MapZone)4;
			saturation = 0.7f;
			redChannel = AnimationCurve.Linear(0f, 0f, 1f, 1f);
			greenChannel = AnimationCurve.Linear(0f, 0f, 1f, 1f);
			blueChannel = AnimationCurve.Linear(0f, 0f, 1f, 1f);
			defaultColor = new Color(1f, 1f, 1f, 1f);
			defaultIntensity = 0.8f;
			heroLightColor = new Color(1f, 1f, 1f, 0.48f);
			AtmosCueSet = "";
			AtmosCueSnapshotName = "at Surface";
			AtmosCueSnapshotIndex = AtmosChoices.Surface;
			AtmosCueIsChannelEnabled = new bool[16]
			{
				false, true, false, true, false, true, true, false, false, false,
				false, false, false, false, false, false
			};
			MusicCueSet = "";
			MusicCueSnapshotName = "Silent";
			MusicCueSnapshotIndex = MusicChoices.Silent;
			MusicCueChannelInfoClips = (AudioClip[])(object)new AudioClip[6];
			MusicCueChannelInfoSyncs = new MusicChannelSync[6];
			InfectedMusicCueSet = "";
			InfectedMusicCueSnapshotName = "Silent";
			InfectedMusicCueSnapshotIndex = MusicChoices.Silent;
			InfectedMusicCueChannelInfoClips = (AudioClip[])(object)new AudioClip[6];
			InfectedMusicCueChannelInfoSyncs = new MusicChannelSync[6];
			MsSnapshotName = "Silent";
			MsSnapshotIndex = MusicChoices.Silent;
			musicTransitionTime = 3f;
			AtsSnapshotName = "at Surface";
			AtsSnapshotIndex = AtmosChoices.Surface;
			EsSnapshotName = "en Cliffs";
			EsSnapshotIndex = EnviroChoices.Cliffs;
			AcsSnapshotName = "On";
			SsSnapshotName = "Away";
			transitionTime = 0.5f;
			((MonoBehaviour)this)..ctor();
		}
	}
	public class SceneMapPatcher : MonoBehaviour
	{
		public Texture tex;

		public void Start()
		{
			MeshRenderer[] componentsInChildren = ((Component)this).gameObject.GetComponentsInChildren<MeshRenderer>(false);
			foreach (MeshRenderer obj in componentsInChildren)
			{
				((Renderer)obj).material.shader = Shader.Find("tk2d/BlendVertexColor");
				((Renderer)obj).material.SetTexture(Shader.PropertyToID("_MainTex"), tex);
			}
		}
	}
	public class SpritePatcher : MonoBehaviour
	{
		public string shader = "Sprites/Default";

		public float Scale = 1f;

		public void Start()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			SpriteRenderer[] componentsInChildren = ((Component)this).gameObject.GetComponentsInChildren<SpriteRenderer>(true);
			foreach (SpriteRenderer obj in componentsInChildren)
			{
				Transform transform = ((Component)obj).gameObject.transform;
				transform.localScale *= Scale;
				((Renderer)obj).material.shader = Shader.Find(shader);
			}
		}
	}
}
namespace SFCore.ModInterOp
{
	internal class InterOpItem
	{
		private Delegate m_method;

		private object m_obj;

		private bool m_isStatic;

		public Delegate method => m_method;

		public object obj => m_obj;

		public bool isStatic => m_isStatic;

		public InterOpItem(Delegate m, object o = null, bool isStatic = true)
		{
			m_method = m;
			m_obj = o;
			m_isStatic = isStatic;
		}
	}
	public static class MIOController
	{
		private static Dictionary<string, InterOpItem> m_methodMap = new Dictionary<string, InterOpItem>();

		[PublicAPI]
		public static bool AddDelegate(Delegate d, string n)
		{
			if (m_methodMap.ContainsKey(n))
			{
				return false;
			}
			InterOpItem value = (d.Method.IsStatic ? new InterOpItem(d) : new InterOpItem(d, d.Target, isStatic: false));
			m_methodMap.Add(n, value);
			return true;
		}

		[PublicAPI]
		public static bool AddAction(Action a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddAction<T1>(Action<T1> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddAction<T1, T2>(Action<T1, T2> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddAction<T1, T2, T3>(Action<T1, T2, T3> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddAction<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddAction<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddAction<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddFunc<TRes>(Func<TRes> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddFunc<T1, TRes>(Func<T1, TRes> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddFunc<T1, T2, TRes>(Func<T1, T2, TRes> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddFunc<T1, T2, T3, TRes>(Func<T1, T2, T3, TRes> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddFunc<T1, T2, T3, T4, TRes>(Func<T1, T2, T3, T4, TRes> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddFunc<T1, T2, T3, T4, T5, TRes>(Func<T1, T2, T3, T4, T5, TRes> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddFunc<T1, T2, T3, T4, T5, T6, TRes>(Func<T1, T2, T3, T4, T5, T6, TRes> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, TRes> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TRes> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TRes> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRes> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TRes> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRes> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TRes> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TRes> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TRes> a, string n)
		{
			return AddDelegate(a, n);
		}

		[PublicAPI]
		public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TRes> a, string n)
		{
			return AddDelegate(a, n);
		}

		private static bool CheckForMethod(InterOpItem ioi, object[] @params)
		{
			ParameterInfo[] parameters = ioi.method.Method.GetParameters();
			if (parameters.Length != @params.Length)
			{
				return false;
			}
			bool flag = true;
			for (int i = 0; i < parameters.Length; i++)
			{
				flag &= parameters[i].ParameterType.FullName == @params.GetType().FullName;
			}
			return flag;
		}

		[PublicAPI]
		public static bool CallAction(string name, object[] @params = null)
		{
			if (!m_methodMap.ContainsKey(name))
			{
				return false;
			}
			InterOpItem interOpItem = m_methodMap[name];
			if (!CheckForMethod(interOpItem, @params))
			{
				return false;
			}
			if (interOpItem.isStatic)
			{
				interOpItem.method.DynamicInvoke(@params);
			}
			else
			{
				interOpItem.method.Method.Invoke(interOpItem.obj, @params);
			}
			return true;
		}

		[PublicAPI]
		public static bool CallFunc<TRes>(string name, out TRes res, object[] @params = null)
		{
			if (!m_methodMap.ContainsKey(name))
			{
				res = default(TRes);
				return false;
			}
			InterOpItem interOpItem = m_methodMap[name];
			if (!CheckForMethod(interOpItem, @params))
			{
				res = default(TRes);
				return false;
			}
			if (interOpItem.isStatic)
			{
				res = (TRes)interOpItem.method.DynamicInvoke(@params);
			}
			else
			{
				res = (TRes)interOpItem.method.Method.Invoke(interOpItem.obj, @params);
			}
			return true;
		}
	}
}
namespace SFCore.Generics
{
	public abstract class FullSettingsMod<TSave, TGlobal> : Mod, ILocalSettings<TSave>, IGlobalSettings<TGlobal> where TSave : new() where TGlobal : new()
	{
		public static TGlobal GlobalSettings { get; protected set; } = new TGlobal();

		public TSave SaveSettings { get; protected set; } = new TSave();

		public void OnLoadGlobal(TGlobal s)
		{
			GlobalSettings = s;
		}

		public TGlobal OnSaveGlobal()
		{
			return GlobalSettings;
		}

		public void OnLoadLocal(TSave s)
		{
			SaveSettings = s;
		}

		public TSave OnSaveLocal()
		{
			return SaveSettings;
		}

		public FullSettingsMod()
			: base((string)null)
		{
		}

		public FullSettingsMod(string name)
			: base(name)
		{
		}
	}
	public abstract class GlobalSettingsMod<TGlobal> : Mod, IGlobalSettings<TGlobal> where TGlobal : new()
	{
		public static TGlobal GlobalSettings { get; protected set; } = new TGlobal();

		public void OnLoadGlobal(TGlobal s)
		{
			GlobalSettings = s;
		}

		public TGlobal OnSaveGlobal()
		{
			return GlobalSettings;
		}

		public GlobalSettingsMod()
			: base((string)null)
		{
		}

		public GlobalSettingsMod(string name)
			: base(name)
		{
		}
	}
	public abstract class SaveSettingsMod<TSave> : Mod, ILocalSettings<TSave> where TSave : new()
	{
		public TSave SaveSettings { get; protected set; } = new TSave();

		public void OnLoadLocal(TSave s)
		{
			SaveSettings = s;
		}

		public TSave OnSaveLocal()
		{
			return SaveSettings;
		}

		public SaveSettingsMod()
			: base((string)null)
		{
		}

		public SaveSettingsMod(string name)
			: base(name)
		{
		}
	}
}
