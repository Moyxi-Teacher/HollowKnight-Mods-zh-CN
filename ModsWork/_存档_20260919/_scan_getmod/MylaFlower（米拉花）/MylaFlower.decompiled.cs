using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using ItemChanger.Locations;
using ItemChanger.Tags;
using Language;
using MenuChanger;
using MenuChanger.MenuElements;
using Microsoft.CodeAnalysis;
using Modding;
using Modding.Delegates;
using MonoMod.ModInterop;
using MylaFlower.RandoConnection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using On;
using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerMod;
using RandomizerMod.Logging;
using RandomizerMod.Menu;
using RandomizerMod.RC;
using RandomizerMod.RandomizerData;
using RandomizerMod.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vasi;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: ComVisible(false)]
[assembly: Guid("65d8de40-305e-439b-822c-7b99094c1f48")]
[assembly: AssemblyTitle("米拉花")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("MylaFlower")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyFileVersion("2.0.1.0")]
[assembly: AssemblyInformationalVersion("2.0.1.0汉化")]
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: AssemblyVersion("2.0.1.0")]
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
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = false)]
	internal sealed class NullableAttribute : Attribute
	{
		public readonly byte[] NullableFlags;

		public NullableAttribute(byte P_0)
		{
			NullableFlags = new byte[1] { P_0 };
		}

		public NullableAttribute(byte[] P_0)
		{
			NullableFlags = P_0;
		}
	}
	[CompilerGenerated]
	[Embedded]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
	internal sealed class NullableContextAttribute : Attribute
	{
		public readonly byte Flag;

		public NullableContextAttribute(byte P_0)
		{
			Flag = P_0;
		}
	}
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
namespace MylaFlower
{
	public static class Consts
	{
		public const string MylaScene = "Crossroads_45";

		public const string NormalMyla = "Miner";

		public const string CrazyMyla = "Myla Crazy NPC";

		public const string ZombieMyla = "Zombie Myla";

		public const string ConvoFsmName = "Conversation Control";

		public const string CustomLanguageSheet = "MYLA FLOWER";
	}
	internal static class Dialogue
	{
		internal delegate string TextEditHandler(string key, string sheetTitle, string orig);

		[CompilerGenerated]
		private static class <>O
		{
			public static LanguageGetProxy <0>__ChangeMylaDialogue;
		}

		internal static event TextEditHandler OnGetMylaText;

		internal static void Hook()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			object obj = <>O.<0>__ChangeMylaDialogue;
			if (obj == null)
			{
				LanguageGetProxy val = ChangeMylaDialogue;
				<>O.<0>__ChangeMylaDialogue = val;
				obj = (object)val;
			}
			ModHooks.LanguageGetHook += (LanguageGetProxy)obj;
		}

		private static string ChangeMylaDialogue(string key, string sheetTitle, string orig)
		{
			if (!TryChangeMylaDialogueInternal(key, sheetTitle, orig, out var newText))
			{
				return orig;
			}
			if (OnGetMylaText == null)
			{
				return newText;
			}
			Delegate[] array = OnGetMylaText?.GetInvocationList();
			for (int i = 0; i < array.Length; i++)
			{
				newText = ((TextEditHandler)array[i])(key, sheetTitle, newText);
			}
			return newText;
		}

		private static bool TryChangeMylaDialogueInternal(string key, string sheetTitle, string orig, out string newText)
		{
			if (sheetTitle == "MYLA FLOWER")
			{
				switch (key)
				{
				case "FLOWER_OFFER":
				case "FLOWER_OFFER_YN":
				case "GIVEN_FLOWER":
				case "NOT_GIVEN_FLOWER":
					newText = Localization.GetText(key);
					return true;
				}
				((Loggable)MylaFlower.instance).LogWarn("Unrecognized key for myla flower sheet: " + key);
			}
			if (sheetTitle == "Minor NPC" && key == "MINER_DREAM_2" && MylaFlower.GetMylaState() == MylaState.Crazy)
			{
				newText = Localization.GetText("CRAZY_DREAM");
				return true;
			}
			if (sheetTitle == "Enemy Dreams" && key.StartsWith("MYLA"))
			{
				newText = orig;
				return true;
			}
			if (sheetTitle == "Minor NPC" && key == "MINER_EARLY_3" && MylaFlower.GetMylaState() == MylaState.Normal)
			{
				newText = Localization.GetText("GIVEN_DIALOGUE");
				return true;
			}
			newText = orig;
			return false;
		}
	}
	public enum MylaState
	{
		Normal,
		Crazy,
		Zombie
	}
	[JsonConverter(typeof(LocaleConverter))]
	public class Locale
	{
		private static readonly Random _rng = new Random();

		public Dictionary<string, List<string>> Data;

		public bool TryGetString(string key, out string value)
		{
			if (!Data.TryGetValue(key, out var value2) || value2.Count == 0)
			{
				value = "%!%" + key + "%!%";
				return false;
			}
			value = value2[_rng.Next(value2.Count)];
			return true;
		}
	}
	public class LocaleConverter : JsonConverter<Locale>
	{
		public override bool CanRead => true;

		public override bool CanWrite => false;

		public override Locale ReadJson(JsonReader reader, Type objectType, Locale existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Invalid comparison between Unknown and I4
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Invalid comparison between Unknown and I4
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			string text = default(string);
			JToken val = default(JToken);
			foreach (KeyValuePair<string, JToken> item in JObject.Load(reader))
			{
				DeconstructUtil.Deconstruct<string, JToken>(item, ref text, ref val);
				string key = text;
				JToken val2 = val;
				if ((int)val2.Type == 8)
				{
					dictionary[key] = new List<string> { val2.ToObject<string>() };
				}
				else if ((int)val2.Type == 2)
				{
					dictionary[key] = val2.ToObject<List<string>>();
				}
			}
			return new Locale
			{
				Data = dictionary
			};
		}

		public override void WriteJson(JsonWriter writer, Locale value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
	internal static class Localization
	{
		private static readonly ILogger _logger;

		private static Dictionary<string, Locale> _localizationData;

		private static Dictionary<string, Locale> _normalizedLocalizationData;

		public static void LoadResources()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			_localizationData = new Dictionary<string, Locale>();
			LoadEmbeddedResources();
			LoadLocalResources();
			NormalizeLocalization();
			_logger.LogDebug($"Current language: {Language.CurrentLanguage()}");
		}

		private static void NormalizeLocalization()
		{
			_normalizedLocalizationData = new Dictionary<string, Locale>();
			string text = default(string);
			Locale locale = default(Locale);
			foreach (KeyValuePair<string, Locale> item in _localizationData.OrderByDescending((KeyValuePair<string, Locale> kvp) => kvp.Key))
			{
				DeconstructUtil.Deconstruct<string, Locale>(item, ref text, ref locale);
				string text2 = text;
				Locale value = locale;
				_normalizedLocalizationData[text2.Substring(0, 2)] = value;
			}
		}

		private static Locale LoadFromStream(Stream s)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Expected O, but got Unknown
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			using StreamReader streamReader = new StreamReader(s);
			JsonTextReader val = new JsonTextReader((TextReader)streamReader);
			try
			{
				return new JsonSerializer().Deserialize<Locale>((JsonReader)(object)val);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}

		private static void LoadLocalResources()
		{
			foreach (FileInfo item in new DirectoryInfo(Path.GetDirectoryName(typeof(Localization).Assembly.Location)).EnumerateFiles("*.json"))
			{
				string text = item.Name.Substring(0, item.Name.Length - ".json".Length).ToUpperInvariant();
				_logger.LogDebug("Loading local language data for " + text);
				using Stream s = item.OpenRead();
				_localizationData[text] = LoadFromStream(s);
			}
		}

		private static void LoadEmbeddedResources()
		{
			Assembly assembly = typeof(Localization).Assembly;
			string[] manifestResourceNames = assembly.GetManifestResourceNames();
			foreach (string text in manifestResourceNames)
			{
				if (text.StartsWith("MylaFlower.Resources.lang.") && text.EndsWith(".json"))
				{
					string text2 = text.Substring("MylaFlower.Resources.lang.".Length, text.Length - "MylaFlower.Resources.lang.".Length - ".json".Length).ToUpperInvariant();
					_logger.LogDebug("Loading embedded language data for " + text2);
					using Stream s = assembly.GetManifestResourceStream(text);
					_localizationData[text2] = LoadFromStream(s);
				}
			}
		}

		public static string GetText(string key)
		{
			string value = string.Empty;
			foreach (Locale availableSheet in GetAvailableSheets())
			{
				if (availableSheet.TryGetString(key, out value))
				{
					return value;
				}
			}
			return value;
		}

		private static IEnumerable<Locale> GetAvailableSheets()
		{
			string lang = ((object)Language.CurrentLanguage()/*cast due to constrained. prefix*/).ToString().ToUpperInvariant();
			if (_localizationData.TryGetValue(lang, out var value))
			{
				yield return value;
			}
			string key = lang.Substring(0, 2);
			if (_normalizedLocalizationData.TryGetValue(key, out var value2))
			{
				yield return value2;
			}
			yield return _localizationData["EN"];
		}

		static Localization()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Expected O, but got Unknown
			_logger = (ILogger)new SimpleLogger("MylaFlower.Localization");
		}
	}
	public class MylaFlower : Mod, ILocalSettings<SaveSettings>, IGlobalSettings<GlobalSettings>
	{
		internal static MylaFlower instance;

		public static SaveSettings LS { get; internal set; } = new SaveSettings();

		public static GlobalSettings GS { get; internal set; } = new GlobalSettings();

		public static event Action OnGiveFlower;

		public void OnLoadLocal(SaveSettings s)
		{
			LS = s;
		}

		public SaveSettings OnSaveLocal()
		{
			return LS;
		}

		public void OnLoadGlobal(GlobalSettings s)
		{
			GS = s;
		}

		public GlobalSettings OnSaveGlobal()
		{
			return GS;
		}

		public MylaFlower()
			: base("米拉花")
		{
			instance = this;
			Localization.LoadResources();
		}

		public override string GetVersion()
		{
			return VersionUtil.GetVersion<MylaFlower>();
		}

		public override void Initialize()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Expected O, but got Unknown
			((Loggable)this).Log("Initializing Mod...");
			SpriteLoader.LoadTextures();
			DeactivateIfPlayerdataTrue.OnEnable += new hook_OnEnable(OverrideMylaState);
			DeactivateIfPlayerdataFalse.OnEnable += new hook_OnEnable(OverrideMylaState);
			PlayMakerFSM.OnEnable += new hook_OnEnable(ModifyMylaFsms);
			Dialogue.Hook();
			SceneManager.activeSceneChanged += ReplaceMylaSprite;
			if (ModHooks.GetMod("ItemChangerMod", false, false) != null)
			{
				RandoInteropManager.Hook();
			}
		}

		private void ReplaceMylaSprite(Scene _, Scene scene)
		{
			if (((Scene)(ref scene)).name == "Crossroads_45" && LS.DeliveredFlower)
			{
				Texture2D texture = SpriteLoader.GetTexture("MylaFlower");
				((tk2dBaseSprite)((Scene)(ref scene)).GetRootGameObjects().First((GameObject x) => ((Object)x).name == "Miner").GetComponent<tk2dSprite>()).GetCurrentSpriteDef().material.mainTexture = (Texture)(object)texture;
			}
		}

		private void ModifyMylaFsms(orig_OnEnable orig, PlayMakerFSM self)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			Scene scene;
			if (((Object)((Component)self).gameObject).name == "Myla Crazy NPC" && self.FsmName == "Conversation Control")
			{
				scene = ((Component)self).gameObject.scene;
				if (((Scene)(ref scene)).name == "Crossroads_45" && GetMylaState() == MylaState.Crazy)
				{
					EditCrazyMylaFsm(self);
				}
			}
			if (((Object)((Component)self).gameObject).name == "Miner" && self.FsmName == "Conversation Control")
			{
				scene = ((Component)self).gameObject.scene;
				if (((Scene)(ref scene)).name == "Crossroads_45" && GetMylaState() == MylaState.Normal)
				{
					EditNormalMylaFsm(self);
				}
			}
		}

		private void EditNormalMylaFsm(PlayMakerFSM self)
		{
			FsmState state = FsmUtil.GetState(self, "Convo Choice");
			FsmState state2 = FsmUtil.GetState(self, "Early Choice");
			FsmState state3 = FsmUtil.GetState(self, "Early 3");
			state.Transitions = Array.Empty<FsmTransition>();
			FsmUtil.AddTransition(state, FsmEvent.Finished, state2.Name);
			state2.Transitions = Array.Empty<FsmTransition>();
			FsmUtil.AddTransition(state2, FsmEvent.Finished, state3.Name);
		}

		private void EditCrazyMylaFsm(PlayMakerFSM fsm)
		{
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Expected O, but got Unknown
			FsmState state = FsmUtil.GetState(fsm, "Convo Choice");
			FsmState val = FsmUtil.CopyState(fsm, "Box Down", "Flower Box Down");
			FsmState val2 = FsmUtil.CopyState(fsm, "Box Down", "Flower Box Up YN");
			FsmState val3 = FsmUtil.CopyState(fsm, "Box Down", "Flower Send Text YN");
			FsmState val4 = FsmUtil.CopyState(fsm, "Box Down", "Accept Flower");
			FsmState val5 = FsmUtil.CopyState(fsm, "Box Down", "Decline Flower");
			AddMylaConvoState("Flower Convo", "Flower Box Down", "FLOWER_OFFER", "Repeat");
			FsmState val6 = AddMylaConvoState("Accept Convo", "Talk Finish", "GIVEN_FLOWER");
			AddMylaConvoState("Decline Convo", "Talk Finish", "NOT_GIVEN_FLOWER");
			AddBoxUpState("Accept Convo Box Up", "Accept Convo");
			AddBoxUpState("Decline Convo Box Up", "Decline Convo");
			FsmUtil.AddTransition(state, "FLOWER OFFER", "Flower Convo");
			FsmUtil.AddTransition(state, "ALREADY GIVEN", "Accept Convo");
			FsmUtil.InsertMethod(state, 0, (Action)delegate
			{
				if (LS.DeliveredFlower)
				{
					fsm.SendEvent("ALREADY GIVEN");
				}
				else
				{
					fsm.SendEvent("FLOWER OFFER");
				}
			});
			FsmUtil.ChangeTransition(val, "FINISHED", "Flower Box Up YN");
			FsmUtil.RemoveAction(val2, 1);
			FsmUtil.GetAction<SendEventByName>(val2).sendEvent = FsmString.op_Implicit("BOX UP YN");
			FsmUtil.GetAction<Wait>(val2).time = FsmFloat.op_Implicit(0.25f);
			FsmUtil.ChangeTransition(val2, "FINISHED", val3.Name);
			FsmStateAction[] actions = (FsmStateAction[])(object)new InvokeMethod[1]
			{
				new InvokeMethod((Action)delegate
				{
					GameObject obj = GameObject.Find("Text YN");
					PlayMakerFSM obj2 = FSMUtility.LocateMyFSM(obj, "Dialogue Page Control");
					obj2.FsmVariables.GetFsmInt("Toll Cost").Value = 0;
					obj2.FsmVariables.GetFsmGameObject("Requester").Value = ((Component)fsm).gameObject;
					obj.GetComponent<DialogueBox>().StartConversation("FLOWER_OFFER_YN", "MYLA FLOWER");
				})
			};
			val3.Actions = actions;
			val3.Transitions = Array.Empty<FsmTransition>();
			FsmUtil.AddTransition(val3, "YES", val4.Name);
			FsmUtil.AddTransition(val3, "NO", val5.Name);
			val4.Actions = (FsmStateAction[])(object)new FsmStateAction[2]
			{
				val4.Actions[2],
				val4.Actions[0]
			};
			FsmUtil.GetAction<SendEventByName>(val4).sendEvent = FsmString.op_Implicit("BOX DOWN YN");
			val5.Actions = (FsmStateAction[])(object)new FsmStateAction[2]
			{
				val5.Actions[2],
				val5.Actions[0]
			};
			FsmUtil.GetAction<SendEventByName>(val5).sendEvent = FsmString.op_Implicit("BOX DOWN YN");
			val5.Transitions = Array.Empty<FsmTransition>();
			FsmUtil.AddTransition(val5, "FINISHED", "Decline Convo Box Up");
			val4.Transitions = Array.Empty<FsmTransition>();
			FsmUtil.AddTransition(val4, "FINISHED", "Accept Convo Box Up");
			FsmUtil.InsertMethod(val6, 0, (Action)delegate
			{
				LS.DeliveredFlower = true;
				PlayerData.instance.SetBool("hasXunFlower", false);
				OnGiveFlower?.Invoke();
			});
			FsmState AddBoxUpState(string stateName, string targetState)
			{
				FsmState val7 = FsmUtil.CopyState(fsm, "Box Up", stateName);
				val7.Actions = (FsmStateAction[])(object)new FsmStateAction[2]
				{
					val7.Actions[0],
					val7.Actions[1]
				};
				FsmUtil.ChangeTransition(val7, "FINISHED", targetState);
				return val7;
			}
			FsmState AddMylaConvoState(string stateName, string targetState, string convoName, string origTarget = "Greet")
			{
				FsmState obj = FsmUtil.CopyState(fsm, origTarget, stateName);
				CallMethodProper action = FsmUtil.GetAction<CallMethodProper>(obj);
				action.parameters[0].stringValue = convoName;
				action.parameters[1].stringValue = "MYLA FLOWER";
				obj.Transitions[0].ToFsmState = FsmUtil.GetState(fsm, targetState);
				obj.Transitions[0].ToState = targetState;
				return obj;
			}
		}

		private void OverrideMylaState(orig_OnEnable orig, DeactivateIfPlayerdataTrue self)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			Scene scene = ((Component)self).gameObject.scene;
			if (((Scene)(ref scene)).name == "Crossroads_45")
			{
				bool? shouldKillMyla = GetShouldKillMyla(((Object)((Component)self).gameObject).name);
				if (shouldKillMyla == true)
				{
					((Component)self).gameObject.SetActive(false);
				}
				else if (shouldKillMyla == false)
				{
					return;
				}
			}
			orig.Invoke(self);
		}

		private void OverrideMylaState(orig_OnEnable orig, DeactivateIfPlayerdataFalse self)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			Scene scene = ((Component)self).gameObject.scene;
			if (((Scene)(ref scene)).name == "Crossroads_45")
			{
				bool? shouldKillMyla = GetShouldKillMyla(((Object)((Component)self).gameObject).name);
				if (shouldKillMyla == true)
				{
					((Component)self).gameObject.SetActive(false);
				}
				else if (shouldKillMyla == false)
				{
					return;
				}
			}
			orig.Invoke(self);
		}

		public static MylaState? GetMylaState()
		{
			if (LS.DeliveredFlower)
			{
				return MylaState.Normal;
			}
			if (!PlayerData.instance.GetBool("hasSuperDash"))
			{
				return null;
			}
			if (MylaKilled())
			{
				return MylaState.Zombie;
			}
			if (PlayerData.instance.GetBool("hasXunFlower") && !PlayerData.instance.GetBool("xunFlowerBroken"))
			{
				return MylaState.Crazy;
			}
			return MylaState.Zombie;
		}

		public static bool? GetShouldKillMyla(string objectName)
		{
			MylaState? mylaState = GetMylaState();
			if (!mylaState.HasValue)
			{
				return null;
			}
			return objectName switch
			{
				"Miner" => mylaState != MylaState.Normal, 
				"Myla Crazy NPC" => mylaState != MylaState.Crazy, 
				"Zombie Myla" => mylaState != MylaState.Zombie, 
				_ => null, 
			};
		}

		internal static bool MylaKilled()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			PersistentBoolData val = new PersistentBoolData
			{
				id = "Zombie Myla",
				sceneName = "Crossroads_45"
			};
			SceneData obj = SceneData.instance;
			return ((obj != null) ? obj.FindMyState(val) : null)?.activated ?? false;
		}
	}
	public class SaveSettings
	{
		public bool DeliveredFlower { get; set; }
	}
	public class GlobalSettings
	{
		public RandoSettings RandoSettings { get; set; } = new RandoSettings();
	}
	public static class SpriteLoader
	{
		public static Dictionary<string, Texture2D> Textures;

		public static Texture2D GetTexture(string imgName)
		{
			return Textures[imgName];
		}

		public static void LoadTextures()
		{
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Expected O, but got Unknown
			Textures = new Dictionary<string, Texture2D>();
			foreach (string item in from name in typeof(SpriteLoader).Assembly.GetManifestResourceNames()
				where name.ToLower().EndsWith(".png")
				select name)
			{
				try
				{
					using Stream stream = typeof(SpriteLoader).Assembly.GetManifestResourceStream(item);
					if (stream != null)
					{
						byte[] array = new byte[stream.Length];
						stream.Read(array, 0, array.Length);
						Texture2D val = new Texture2D(1, 1, (TextureFormat)4, false);
						ImageConversion.LoadImage(val, array, true);
						((Texture)val).filterMode = (FilterMode)0;
						string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item);
						string[] array2 = fileNameWithoutExtension.Split('.');
						fileNameWithoutExtension = array2[array2.Length - 1];
						Textures.Add(fileNameWithoutExtension, val);
					}
				}
				catch (Exception ex)
				{
					((Loggable)MylaFlower.instance).LogError((object)ex);
				}
			}
		}
	}
}
namespace MylaFlower.RandoConnection
{
	public class MylaFlowerLocation : AutoLocation, ILocalHintLocation
	{
		public bool HintActive { get; set; }

		protected override void OnLoad()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Expected O, but got Unknown
			MylaFlower.OnGiveFlower += GiveItemOnGiveFlower;
			Events.AddFsmEdit(((AbstractLocation)this).sceneName, new FsmID("Miner", "Conversation Control"), (Action<PlayMakerFSM>)GiveRespawnedItems);
			Dialogue.OnGetMylaText += ModifyMylaText;
			HealthManager.TakeDamage += new hook_TakeDamage(ProtectMyla);
		}

		protected override void OnUnload()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Expected O, but got Unknown
			MylaFlower.OnGiveFlower -= GiveItemOnGiveFlower;
			Events.RemoveFsmEdit(((AbstractLocation)this).sceneName, new FsmID("Miner", "Conversation Control"), (Action<PlayMakerFSM>)GiveRespawnedItems);
			Dialogue.OnGetMylaText -= ModifyMylaText;
			HealthManager.TakeDamage -= new hook_TakeDamage(ProtectMyla);
		}

		private void ProtectMyla(orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			if (!(((Object)((Component)self).gameObject).name != "Zombie Myla"))
			{
				Scene scene = ((Component)self).gameObject.scene;
				if (!(((Scene)(ref scene)).name != ((AbstractLocation)this).sceneName))
				{
					int num = Mathf.RoundToInt((float)hitInstance.DamageDealt * hitInstance.Multiplier);
					if (num >= self.hp)
					{
						self.hp = 50 + num;
						ShowPreviewConvo(self);
					}
					orig.Invoke(self, hitInstance);
					return;
				}
			}
			orig.Invoke(self, hitInstance);
		}

		private void ShowPreviewConvo(HealthManager hm)
		{
			ReflectionHelper.CallMethod<EnemyDreamnailReaction>(((Component)hm).gameObject.GetComponent<EnemyDreamnailReaction>(), "ShowConvo", Array.Empty<object>());
		}

		private string ModifyMylaText(string key, string sheetTitle, string orig)
		{
			if (!LocalHintLocationExtensions.GetItemHintActive((ILocalHintLocation)(object)this))
			{
				return orig;
			}
			if (key == "FLOWER_OFFER_YN" && sheetTitle == "MYLA FLOWER")
			{
				string uIName = ((AbstractLocation)this).Placement.GetUIName();
				((AbstractLocation)this).Placement.OnPreview(uIName);
				return string.Format(Localization.GetText("FLOWER_OFFER_YN_RANDO"), uIName);
			}
			if (key.StartsWith("MYLA") && sheetTitle == "Enemy Dreams")
			{
				string uIName2 = ((AbstractLocation)this).Placement.GetUIName();
				((AbstractLocation)this).Placement.OnPreview(uIName2);
				return string.Format(Localization.GetText("ZOMBIE_DREAM_RANDO"), uIName2);
			}
			return orig;
		}

		private void GiveRespawnedItems(PlayMakerFSM fsm)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			PlayMakerExtensions.AddFirstAction(PlayMakerExtensions.GetState(fsm, "Talk Finish"), (FsmStateAction)new Lambda((Action)delegate
			{
				if (((AbstractLocation)this).Placement.CheckVisitedAll((VisitState)16) && !((AbstractLocation)this).Placement.AllObtained())
				{
					((AutoLocation)this).GiveAll();
				}
			}));
		}

		private void GiveItemOnGiveFlower()
		{
			((AbstractLocation)this).Placement.AddVisitFlag((VisitState)16);
			((AutoLocation)this).GiveAll();
		}
	}
	public static class RandoInteropManager
	{
		public static void Hook()
		{
			if (ModHooks.GetMod("ItemChangerMod", false, false) != null)
			{
				ICInterop.HookItemChanger();
			}
			if (ModHooks.GetMod("Randomizer 4", false, false) != null)
			{
				RandoInterop.HookRandomizer();
			}
		}
	}
	public static class ICInterop
	{
		internal static void HookItemChanger()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected O, but got Unknown
			AbstractLocation val = (AbstractLocation)(object)new MylaFlowerLocation
			{
				sceneName = "Crossroads_45",
				name = "Myla_Flower",
				HintActive = true
			};
			InteropTag val2 = new InteropTag
			{
				Message = "RandoSupplementalMetadata"
			};
			val2.Properties["PoolGroup"] = "MaskShards";
			val2.Properties["ModSource"] = ((Mod)MylaFlower.instance).GetName();
			val2.Properties["WorldMapLocations"] = new(string, float, float)[1] { ("Crossroads_45", 52.9f, 3.4f) };
			val2.Properties["PinSpriteKey"] = "Extra.Flower";
			((TaggableObject)val).AddTag((Tag)(object)val2);
			Finder.DefineCustomLocation(val);
		}
	}
	public static class RandoInterop
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static RequestBuilderUpdateHandler <0>__SetupRefs;

			public static RequestBuilderUpdateHandler <1>__DefineRandoLocation;

			public static Action<GenerationSettings, LogicManagerBuilder> <2>__DefineLogic;

			public static TryGetMenuButtonHandler <3>__BuildConnectionMenuButton;

			public static GroupResolver <4>__MatchFlowerGroup;
		}

		[Serializable]
		[CompilerGenerated]
		private sealed class <>c
		{
			public static readonly <>c <>9 = new <>c();

			public static OnRandomizerMenuConstructionHandler <>9__4_0;

			public static Action<LogArguments, TextWriter> <>9__4_1;

			public static Func<LocationDef> <>9__6_2;

			public static Action<ICFactory, RandoPlacement, AbstractPlacement> <>9__6_3;

			internal void <HookRandomizer>b__4_0(MenuPage _)
			{
			}

			internal void <HookRandomizer>b__4_1(LogArguments args, TextWriter tw)
			{
				if (RS.Enabled)
				{
					tw.WriteLine("MylaFlower: Enabled");
				}
				else
				{
					tw.WriteLine("MylaFlower: Disabled");
				}
			}

			internal LocationDef <SetupRefs>b__6_2()
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Expected O, but got Unknown
				LocationDef val = new LocationDef();
				val.set_Name("Myla_Flower");
				val.set_SceneName("Crossroads_45");
				val.set_FlexibleCount(false);
				val.set_AdditionalProgressionPenalty(false);
				return val;
			}

			internal void <SetupRefs>b__6_3(ICFactory _, RandoPlacement _, AbstractPlacement pmt)
			{
				((TaggableObject)pmt).GetOrAddTag<DisableItemPreviewTag>();
			}
		}

		public const string MylaFlowerLocation = "Myla_Flower";

		public static RandoSettings RS
		{
			get
			{
				return MylaFlower.GS.RandoSettings;
			}
			set
			{
				MylaFlower.GS.RandoSettings = value;
			}
		}

		internal static void HookRandomizer()
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Expected O, but got Unknown
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Expected O, but got Unknown
			PriorityEvent<RequestBuilderUpdateHandler> onUpdate = RequestBuilder.OnUpdate;
			object obj = <>O.<0>__SetupRefs;
			if (obj == null)
			{
				RequestBuilderUpdateHandler val = SetupRefs;
				<>O.<0>__SetupRefs = val;
				obj = (object)val;
			}
			onUpdate.Subscribe(-500f, (RequestBuilderUpdateHandler)obj);
			PriorityEvent<RequestBuilderUpdateHandler> onUpdate2 = RequestBuilder.OnUpdate;
			object obj2 = <>O.<1>__DefineRandoLocation;
			if (obj2 == null)
			{
				RequestBuilderUpdateHandler val2 = DefineRandoLocation;
				<>O.<1>__DefineRandoLocation = val2;
				obj2 = (object)val2;
			}
			onUpdate2.Subscribe(0.1f, (RequestBuilderUpdateHandler)obj2);
			RCData.RuntimeLogicOverride.Subscribe(10f, (Action<GenerationSettings, LogicManagerBuilder>)DefineLogic);
			object obj3 = <>c.<>9__4_0;
			if (obj3 == null)
			{
				OnRandomizerMenuConstructionHandler val3 = delegate
				{
				};
				<>c.<>9__4_0 = val3;
				obj3 = (object)val3;
			}
			object obj4 = <>O.<3>__BuildConnectionMenuButton;
			if (obj4 == null)
			{
				TryGetMenuButtonHandler val4 = BuildConnectionMenuButton;
				<>O.<3>__BuildConnectionMenuButton = val4;
				obj4 = (object)val4;
			}
			RandomizerMenuAPI.AddMenuPage((OnRandomizerMenuConstructionHandler)obj3, (TryGetMenuButtonHandler)obj4);
			RandoSettingsManagerInterop.Hook();
			SettingsLog.AfterLogSettings += delegate(LogArguments args, TextWriter tw)
			{
				if (RS.Enabled)
				{
					tw.WriteLine("MylaFlower: Enabled");
				}
				else
				{
					tw.WriteLine("MylaFlower: Disabled");
				}
			};
		}

		private static bool BuildConnectionMenuButton(MenuPage landingPage, out SmallButton button)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			SmallButton settingsButton = new SmallButton(landingPage, "Myla Flower");
			UpdateButtonColor();
			((BaseButton)settingsButton).OnClick += delegate
			{
				RS.Enabled = !RS.Enabled;
				UpdateButtonColor();
			};
			button = settingsButton;
			return true;
			void UpdateButtonColor()
			{
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				((Graphic)settingsButton.Text).color = (RS.Enabled ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR);
			}
		}

		private static void SetupRefs(RequestBuilder rb)
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Expected O, but got Unknown
			if (!RS.Enabled)
			{
				return;
			}
			rb.EditLocationRequest("Myla_Flower", (Action<LocationRequestInfo>)delegate(LocationRequestInfo info)
			{
				info.getLocationDef = delegate
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0005: Unknown result type (might be due to invalid IL or missing references)
					//IL_0010: Unknown result type (might be due to invalid IL or missing references)
					//IL_001b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Expected O, but got Unknown
					LocationDef val2 = new LocationDef();
					val2.set_Name("Myla_Flower");
					val2.set_SceneName("Crossroads_45");
					val2.set_FlexibleCount(false);
					val2.set_AdditionalProgressionPenalty(false);
					return val2;
				};
				if (!rb.gs.LongLocationSettings.FlowerQuestPreview)
				{
					info.onPlacementFetch = (Action<ICFactory, RandoPlacement, AbstractPlacement>)Delegate.Combine(info.onPlacementFetch, (Action<ICFactory, RandoPlacement, AbstractPlacement>)delegate(ICFactory _, RandoPlacement _, AbstractPlacement pmt)
					{
						((TaggableObject)pmt).GetOrAddTag<DisableItemPreviewTag>();
					});
				}
			});
			PriorityEvent<GroupResolver> onGetGroupFor = rb.OnGetGroupFor;
			object obj = <>O.<4>__MatchFlowerGroup;
			if (obj == null)
			{
				GroupResolver val = MatchFlowerGroup;
				<>O.<4>__MatchFlowerGroup = val;
				obj = (object)val;
			}
			onGetGroupFor.Subscribe(0f, (GroupResolver)obj);
			static bool MatchFlowerGroup(RequestBuilder val2, string item, ElementType type, out GroupBuilder gb)
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Invalid comparison between Unknown and I4
				if (item == "Myla_Flower" && ((int)type == 0 || (int)type == 2))
				{
					gb = val2.GetGroupFor("Mask_Shard-Grey_Mourner", (ElementType)0);
					return true;
				}
				gb = null;
				return false;
			}
		}

		private static void DefineLogic(GenerationSettings gs, LogicManagerBuilder lmb)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			if (RS.Enabled)
			{
				lmb.AddLogicDef(new RawLogicDef("Myla_Flower", "(Crossroads_45[left1] | Crossroads_45[right1]) + SUPERDASH + NOFLOWER=FALSE"));
			}
		}

		private static void DefineRandoLocation(RequestBuilder rb)
		{
			if (RS.Enabled)
			{
				rb.AddLocationByName("Myla_Flower");
			}
		}
	}
	public class RandoSettings
	{
		public bool Enabled { get; set; }
	}
	[ModImportName("RandoSettingsManager")]
	internal static class RandoSettingsManagerInterop
	{
		public static Action<Mod, Type, Delegate, Delegate> RegisterConnectionSimple;

		public static void Hook()
		{
			RegisterConnectionSimple?.Invoke((Mod)(object)MylaFlower.instance, typeof(RandoSettings), new Action<RandoSettings>(ReceiveSettings), new Func<RandoSettings>(ProvideSettings));
		}

		private static void ReceiveSettings(RandoSettings s)
		{
			RandoInterop.RS = s ?? new RandoSettings();
		}

		private static RandoSettings ProvideSettings()
		{
			if (!RandoInterop.RS.Enabled)
			{
				return null;
			}
			return RandoInterop.RS;
		}

		static RandoSettingsManagerInterop()
		{
			ModInteropManager.ModInterop(typeof(RandoSettingsManagerInterop));
		}
	}
}
