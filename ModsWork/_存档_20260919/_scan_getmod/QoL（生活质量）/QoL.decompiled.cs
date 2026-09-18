using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using GlobalEnums;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using IL;
using InControl;
using JetBrains.Annotations;
using Language;
using Microsoft.CodeAnalysis;
using Modding;
using Modding.Delegates;
using Modding.Menu;
using Modding.Menu.Config;
using Modding.Patches;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.ModInterop;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using On;
using On.HutongGames.PlayMaker.Actions;
using On.UnityEngine.UI;
using QoL.Components;
using QoL.Modules;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vasi;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: ComVisible(false)]
[assembly: Guid("f039fa1d-a12d-4672-bf0a-60cf7900ee21")]
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: AssemblyCompany("QoL")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyCopyright("Copyright © Yusuf Bham 2021")]
[assembly: AssemblyFileVersion("4.9.0")]
[assembly: AssemblyInformationalVersion("4.9.0汉化+19f8490cff339f1b9d58924fe59f0d47da0b6eae")]
[assembly: AssemblyProduct("QoL")]
[assembly: AssemblyTitle("QoL")]
[assembly: AssemblyVersion("4.9.0.0")]
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
namespace QoL
{
	internal class I18n
	{
		private static Dictionary<string, string>? Entries;

		public static bool Available => Entries != null;

		static I18n()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			Assembly assembly = typeof(I18n).Assembly;
			string text = ((object)Language.CurrentLanguage()/*cast due to constrained. prefix*/).ToString().ToLower();
			if (TryLoadFromFile(Path.GetDirectoryName(assembly.Location), text))
			{
				return;
			}
			using Stream stream = assembly.GetManifestResourceStream("QoL.Locales." + text + ".json");
			if (stream != null)
			{
				byte[] array = new byte[stream.Length];
				if (stream.Read(array, 0, array.Length) != array.Length)
				{
					throw new InvalidOperationException();
				}
				ParseLanguage(Encoding.UTF8.GetString(array), text);
			}
		}

		private static void ParseLanguage(string data, string lang)
		{
			try
			{
				Entries = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
			}
			catch (Exception arg)
			{
				Logger.LogError($"[QoL]: Failed to parse {lang} locale! {arg}");
			}
		}

		private static bool TryLoadFromFile(string? path, string lang)
		{
			if (path == null)
			{
				return false;
			}
			string path2 = Path.Combine(path, "QoL." + lang + ".json");
			if (!File.Exists(path2))
			{
				return false;
			}
			ParseLanguage(File.ReadAllText(path2), lang);
			return true;
		}

		public static string Get(string key)
		{
			if (Entries == null)
			{
				return key;
			}
			if (!Entries.TryGetValue(key, out string value))
			{
				return key;
			}
			return value;
		}
	}
	public static class ModMenu
	{
		private static readonly string[] Bools;

		private static readonly List<string> _TogglableModuleNames;

		private static MenuScreen? _ModMenuScreen;

		private static MenuScreen? _ModuleToggleScreen;

		private static MenuScreen? _ModuleFieldToggleScreen;

		public static MenuScreen GetMenuScreen(MenuScreen returnScreen, ModToggleDelegates dels)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			MenuButton val = default(MenuButton);
			MenuBuilder obj = MenuUtils.CreateMenuBuilderWithBackButton("QoL", returnScreen, ref val);
			ApplySetting val3 = default(ApplySetting);
			RefreshSetting val6 = default(RefreshSetting);
			obj.AddContent((IContentLayout)(object)RegularGridLayout.CreateVerticalLayout(105f, default(Vector2)), (Action<ContentArea>)delegate(ContentArea c)
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Expected O, but got Unknown
				//IL_006b: Expected O, but got Unknown
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Expected O, but got Unknown
				//IL_0091: Expected O, but got Unknown
				//IL_012c: Unknown result type (might be due to invalid IL or missing references)
				//IL_013c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0190: Unknown result type (might be due to invalid IL or missing references)
				HorizontalOptionConfig val2 = new HorizontalOptionConfig
				{
					Label = "QoL",
					Options = new string[2]
					{
						Language.Get("MOH_OFF", "MainMenu"),
						Language.Get("MOH_ON", "MainMenu")
					}
				};
				ApplySetting obj2 = val3;
				if (obj2 == null)
				{
					ApplySetting val4 = delegate(MenuSetting _, int i)
					{
						dels.SetModEnabled(Convert.ToBoolean(i));
					};
					ApplySetting val5 = val4;
					val3 = val4;
					obj2 = val5;
				}
				val2.ApplySetting = obj2;
				RefreshSetting obj3 = val6;
				if (obj3 == null)
				{
					RefreshSetting val7 = delegate(MenuSetting s, bool _)
					{
						s.optionList.SetOptionTo(Convert.ToInt32(dels.GetModEnabled()));
					};
					RefreshSetting val8 = val7;
					val6 = val7;
					obj3 = val8;
				}
				val2.RefreshSetting = obj3;
				val2.CancelAction = Return;
				val2.Style = HorizontalOptionStyle.VanillaStyle;
				MenuOptionHorizontal val9 = default(MenuOptionHorizontal);
				HorizontalOptionContent.AddHorizontalOption(c, "QoL", val2, ref val9);
				val9.menuSetting.RefreshValueFromGameSettings(false);
				MenuButtonContent.AddMenuButton(c, "Module Toggles", new MenuButtonConfig
				{
					Label = I18n.Get("Module Toggles"),
					Proceed = true,
					SubmitAction = Modules,
					CancelAction = Return
				});
				MenuButtonContent.AddMenuButton(c, "Module Field Toggles", new MenuButtonConfig
				{
					Label = I18n.Get("Module Field Toggles"),
					Proceed = true,
					SubmitAction = Fields,
					CancelAction = Return
				});
			});
			_ModMenuScreen = obj.Build();
			_ModuleToggleScreen = MenuUtils.CreateMenuScreen(I18n.Get("Module Toggles"), GetModuleToggleMenuData(), _ModMenuScreen);
			_ModuleFieldToggleScreen = MenuUtils.CreateMenuScreen(I18n.Get("Module Field Toggles"), GetModuleFieldMenuData(), _ModMenuScreen);
			return _ModMenuScreen;
			static void Fields(MenuSelectable _)
			{
				UIManager.instance.UIGoToDynamicMenu(_ModuleFieldToggleScreen);
			}
			static void Modules(MenuSelectable _)
			{
				UIManager.instance.UIGoToDynamicMenu(_ModuleToggleScreen);
			}
			void Return(MenuSelectable _)
			{
				UIManager.instance.UIGoToDynamicMenu(returnScreen);
			}
		}

		private static List<MenuEntry> GetModuleToggleMenuData()
		{
			return _TogglableModuleNames.Where((string x) => !SettingsOverride.TryGetModuleOverride(x, out var _)).Select(CreateEntry).ToList();
			static MenuEntry CreateEntry(string name)
			{
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				return new MenuEntry(I18n.Get(name), Bools, string.Empty, (Action<int>)delegate(int i)
				{
					QoL.ToggleModule(name, Convert.ToBoolean(i));
				}, (Func<int>)(() => Convert.ToInt32(QoL.GlobalSettings.EnabledModules[name])));
			}
		}

		private static List<MenuEntry> GetModuleFieldMenuData()
		{
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			List<MenuEntry> list = new List<MenuEntry>();
			FieldInfo fieldInfo = default(FieldInfo);
			Type type = default(Type);
			foreach (KeyValuePair<FieldInfo, Type> field in QoL.GlobalSettings.Fields)
			{
				DeconstructUtil.Deconstruct<FieldInfo, Type>(field, ref fieldInfo, ref type);
				FieldInfo fi = fieldInfo;
				Type type2 = type;
				if (!(fi.FieldType != typeof(bool)) && !SettingsOverride.TryGetSettingOverride(type2.Name + ":" + fi.Name, out var _))
				{
					list.Add(new MenuEntry(I18n.Available ? I18n.Get(fi.Name) : PascalToSpaces(fi.Name), Bools, string.Format(I18n.Get("Comes from {0}"), I18n.Get(type2.Name)), (Action<int>)delegate(int i)
					{
						fi.SetValue(null, Convert.ToBoolean(i));
					}, (Func<int>)(() => Convert.ToInt32(fi.GetValue(null)))));
				}
			}
			return list;
			static string PascalToSpaces(string s)
			{
				return Regex.Replace(s, "([A-Z])", " $1").Trim();
			}
		}

		static ModMenu()
		{
			Bools = new string[2]
			{
				I18n.Get("false"),
				I18n.Get("true")
			};
			_TogglableModuleNames = (from t in typeof(ModMenu).Assembly.GetTypes().Where(FauxMod.IsToggleableFauxMod)
				select t.Name).ToList();
		}
	}
	[UsedImplicitly]
	public class QoL : Mod, ITogglableMod, IMod, ILogger, IGlobalSettings<Settings>, ICustomMenuMod
	{
		private static readonly List<FauxMod> _FauxMods = new List<FauxMod>();

		internal static Settings GlobalSettings { get; private set; } = new Settings();

		bool ICustomMenuMod.ToggleButtonInsideMenu => true;

		public QoL()
			: base("生活质量")
		{
			ModInteropManager.ModInterop(typeof(SettingsOverride));
		}

		public override string GetVersion()
		{
			return VersionUtil.GetVersion<QoL>();
		}

		public override string GetMenuButtonText()
		{
			return "生活质量";
		}

		public override int LoadPriority()
		{
			return int.MaxValue;
		}

		public MenuScreen GetMenuScreen(MenuScreen returnScreen, ModToggleDelegates? dels)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			return ModMenu.GetMenuScreen(returnScreen, dels.Value);
		}

		public void OnLoadGlobal(Settings? s)
		{
			GlobalSettings = s ?? GlobalSettings;
		}

		public Settings OnSaveGlobal()
		{
			return GlobalSettings;
		}

		public override void Initialize()
		{
			foreach (Type item in from x in Assembly.GetAssembly(typeof(QoL)).GetTypes()
				where x.IsSubclassOf(typeof(FauxMod))
				select x)
			{
				FauxMod fauxMod = (FauxMod)Activator.CreateInstance(item);
				if (!SettingsOverride.TryGetModuleOverride(item.Name, out var enabled) && !GlobalSettings.EnabledModules.TryGetValue(item.Name, out enabled))
				{
					enabled = fauxMod.DefaultState;
					GlobalSettings.EnabledModules.Add(item.Name, enabled);
				}
				if (!FauxMod.IsToggleable(item))
				{
					enabled = true;
				}
				if (enabled)
				{
					fauxMod.Initialize();
					fauxMod.IsLoaded = true;
				}
				_FauxMods.Add(fauxMod);
			}
		}

		internal static void ToggleModule(string name, bool enable)
		{
			FauxMod fauxMod = _FauxMods.FirstOrDefault((FauxMod f) => f.GetType().Name == name);
			if (fauxMod != null && fauxMod.IsLoaded != enable)
			{
				if (enable)
				{
					fauxMod.Initialize();
					fauxMod.IsLoaded = true;
					GlobalSettings.EnabledModules[name] = true;
				}
				else
				{
					fauxMod.Unload();
					fauxMod.IsLoaded = false;
					GlobalSettings.EnabledModules[name] = false;
				}
			}
		}

		public void Unload()
		{
			foreach (FauxMod item in _FauxMods.Where((FauxMod x) => x.IsLoaded))
			{
				item.Unload();
			}
		}
	}
	[MeansImplicitUse]
	public class SerializeToSetting : Attribute
	{
		public static bool ShouldSerialize(FieldInfo fi)
		{
			return Attribute.IsDefined(fi, typeof(SerializeToSetting));
		}
	}
	[Serializable]
	public class Settings
	{
		private readonly Assembly _asm = Assembly.GetAssembly(typeof(Settings));

		internal readonly Dictionary<FieldInfo, Type> Fields = new Dictionary<FieldInfo, Type>();

		public Dictionary<string, bool> EnabledModules { get; set; } = new Dictionary<string, bool>();

		public Dictionary<string, bool> Booleans { get; set; } = new Dictionary<string, bool>();

		public Dictionary<string, float> Floats { get; set; } = new Dictionary<string, float>();

		public Dictionary<string, int> Integers { get; set; } = new Dictionary<string, int>();

		public Settings()
		{
			Type[] types = _asm.GetTypes();
			foreach (Type type in types)
			{
				foreach (FieldInfo item in type.GetFields().Where(SerializeToSetting.ShouldSerialize))
				{
					Fields.Add(item, type);
				}
			}
		}

		[OnSerializing]
		public void OnBeforeSerialize(StreamingContext _)
		{
			FieldInfo fieldInfo = default(FieldInfo);
			Type type = default(Type);
			foreach (KeyValuePair<FieldInfo, Type> field in Fields)
			{
				DeconstructUtil.Deconstruct<FieldInfo, Type>(field, ref fieldInfo, ref type);
				FieldInfo fieldInfo2 = fieldInfo;
				Type type2 = type;
				if (fieldInfo2.FieldType == typeof(bool))
				{
					Booleans[type2.Name + ":" + fieldInfo2.Name] = (SettingsOverride.TryGetOrigSetting(type2.Name + ":" + fieldInfo2.Name, out var value) ? value : ((bool)fieldInfo2.GetValue(null)));
				}
				else if (fieldInfo2.FieldType == typeof(float))
				{
					Floats[type2.Name + ":" + fieldInfo2.Name] = (float)fieldInfo2.GetValue(null);
				}
				else if (fieldInfo2.FieldType == typeof(int))
				{
					Integers[type2.Name + ":" + fieldInfo2.Name] = (int)fieldInfo2.GetValue(null);
				}
			}
		}

		[OnDeserialized]
		public void OnAfterDeserialize(StreamingContext _)
		{
			FieldInfo fieldInfo = default(FieldInfo);
			Type type = default(Type);
			foreach (KeyValuePair<FieldInfo, Type> field in Fields)
			{
				DeconstructUtil.Deconstruct<FieldInfo, Type>(field, ref fieldInfo, ref type);
				FieldInfo fieldInfo2 = fieldInfo;
				Type type2 = type;
				int value2;
				if (fieldInfo2.FieldType == typeof(bool))
				{
					if (SettingsOverride.TryGetSettingOverride(type2.Name + ":" + fieldInfo2.Name, out var enabled) || Booleans.TryGetValue(type2.Name + ":" + fieldInfo2.Name, out enabled))
					{
						fieldInfo2.SetValue(null, enabled);
					}
				}
				else if (fieldInfo2.FieldType == typeof(float))
				{
					if (Floats.TryGetValue(type2.Name + ":" + fieldInfo2.Name, out var value))
					{
						fieldInfo2.SetValue(null, value);
					}
				}
				else if (fieldInfo2.FieldType == typeof(int) && Integers.TryGetValue(type2.Name + ":" + fieldInfo2.Name, out value2))
				{
					fieldInfo2.SetValue(null, value2);
				}
			}
		}
	}
	[PublicAPI]
	[ModExportName("QoL")]
	public static class SettingsOverride
	{
		private static readonly HashSet<string> _Modules;

		private static readonly Dictionary<string, bool> _ModuleOverrides;

		private static readonly Dictionary<string, bool> _OrigEnabledModules;

		private static readonly Dictionary<string, FieldInfo> _Fields;

		private static readonly Dictionary<string, bool> _SettingOverrides;

		private static readonly Dictionary<string, bool> _OrigSettings;

		public static void RemoveOverride(string key, Dictionary<string, bool> overrides, Dictionary<string, bool> origSettings, Action<string, bool> toggle)
		{
			bool num = overrides.TryGetValue(key, out var value);
			bool flag = origSettings.TryGetValue(key, out var value2);
			if (num)
			{
				overrides.Remove(key);
			}
			if (flag)
			{
				origSettings.Remove(key);
				if (value != value2)
				{
					toggle(key, value2);
				}
			}
		}

		public static void OverrideModuleToggle(string name, bool enable)
		{
			if (name == null || !_Modules.Contains(name))
			{
				throw new ArgumentException("Name does not correspond to any togglable QoL module.", "name");
			}
			if (QoL.GlobalSettings.EnabledModules.TryGetValue(name, out var value) && !_OrigEnabledModules.ContainsKey(name))
			{
				_OrigEnabledModules[name] = value;
			}
			_ModuleOverrides[name] = enable;
			QoL.ToggleModule(name, enable);
		}

		public static void OverrideSettingToggle(string type, string field, bool enable)
		{
			string text = type + ":" + field;
			if (!_Fields.TryGetValue(text, out FieldInfo value))
			{
				throw new ArgumentException("QoL setting " + text + " not found.", "field");
			}
			if (!_OrigSettings.ContainsKey(text))
			{
				_OrigSettings[text] = (bool)value.GetValue(null);
			}
			_SettingOverrides[text] = enable;
			value.SetValue(null, enable);
		}

		public static void RemoveModuleOverride(string name)
		{
			RemoveOverride(name, _ModuleOverrides, _OrigEnabledModules, QoL.ToggleModule);
		}

		public static void RemoveSettingOverride(string type, string field)
		{
			RemoveOverride(type + ":" + field, _SettingOverrides, _OrigSettings, delegate(string k, bool v)
			{
				_Fields[k].SetValue(null, v);
			});
		}

		private static bool TryGetOverride(string name, out bool enabled, Dictionary<string, bool> overrides)
		{
			bool result = overrides.TryGetValue(name, out var value);
			enabled = value;
			return result;
		}

		public static bool TryGetModuleOverride(string name, out bool enabled)
		{
			return TryGetOverride(name, out enabled, _ModuleOverrides);
		}

		public static bool TryGetSettingOverride(string name, out bool enabled)
		{
			return TryGetOverride(name, out enabled, _SettingOverrides);
		}

		public static bool TryGetOrigSetting(string name, out bool value)
		{
			return _OrigSettings.TryGetValue(name, out value);
		}

		public static bool TryGetOrigModuleEnabled(string name, out bool value)
		{
			return _OrigEnabledModules.TryGetValue(name, out value);
		}

		static SettingsOverride()
		{
			_ModuleOverrides = new Dictionary<string, bool>();
			_OrigEnabledModules = new Dictionary<string, bool>();
			_SettingOverrides = new Dictionary<string, bool>();
			_OrigSettings = new Dictionary<string, bool>();
			Type[] types = typeof(SettingsOverride).Assembly.GetTypes();
			_Modules = new HashSet<string>(from t in types.Where(FauxMod.IsToggleableFauxMod)
				select t.Name);
			_Fields = (from f in types.SelectMany((Type t) => t.GetFields())
				where f.FieldType == typeof(bool) && SerializeToSetting.ShouldSerialize(f)
				select f).ToDictionary((FieldInfo f) => f.DeclaringType.Name + ":" + f.Name);
		}
	}
}
namespace QoL.Modules
{
	[UsedImplicitly]
	public class Dev : FauxMod
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static LogCallback <0>__HandleLog;
		}

		[SerializeToSetting]
		public static bool LogUnityErrors;

		[SerializeToSetting]
		public static bool EnableStacktrace;

		private static readonly ILogger UnityLogger;

		public override void Initialize()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			if (EnableStacktrace)
			{
				Application.SetStackTraceLogType((LogType)4, (StackTraceLogType)2);
			}
			if (LogUnityErrors)
			{
				object obj = <>O.<0>__HandleLog;
				if (obj == null)
				{
					LogCallback val = HandleLog;
					<>O.<0>__HandleLog = val;
					obj = (object)val;
				}
				Application.logMessageReceived += (LogCallback)obj;
			}
		}

		private static void HandleLog(string condition, string stacktrace, LogType type)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			if ((int)type == 4)
			{
				UnityLogger.LogError(condition);
				UnityLogger.LogError(stacktrace);
			}
		}

		static Dev()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			LogUnityErrors = true;
			EnableStacktrace = true;
			UnityLogger = (ILogger)new SimpleLogger("UNITY");
		}
	}
	[UsedImplicitly]
	public class FasterLoads : FauxMod
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static Manipulator <0>__EnterScene;
		}

		private static readonly float[] SKIP = new float[2] { 0.4f, 0.165f };

		private ILHook? _hook;

		public override void Initialize()
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			Unload();
			MethodInfo stateMachineTarget = Extensions.GetStateMachineTarget(typeof(HeroController).GetMethod("EnterScene"));
			object obj = <>O.<0>__EnterScene;
			if (obj == null)
			{
				Manipulator val = EnterScene;
				<>O.<0>__EnterScene = val;
				obj = (object)val;
			}
			_hook = new ILHook((MethodBase)stateMachineTarget, (Manipulator)obj);
		}

		private static void EnterScene(ILContext il)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			ILCursor val = new ILCursor(il).Goto(0, (MoveType)0, false);
			while (val.TryGotoNext(new Func<Instruction, bool>[2]
			{
				delegate(Instruction i)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					return i.OpCode == OpCodes.Ldc_R4;
				},
				delegate(Instruction i)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					return i.OpCode == OpCodes.Newobj && ILPatternMatchingExt.MatchNewobj<WaitForSeconds>(i);
				}
			}))
			{
				if (val.Instrs[val.Index].Operand is float value && SKIP.Contains(value))
				{
					val.Remove();
					val.Remove();
					val.Emit(OpCodes.Ldnull);
				}
			}
		}

		public override void Unload()
		{
			ILHook? hook = _hook;
			if (hook != null)
			{
				hook.Dispose();
			}
		}
	}
	[UsedImplicitly]
	public class FastMenus : FauxMod
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static hook_FadeInCanvasGroupAlpha <0>__FadeInCanvasGroupAlpha;

			public static hook_FadeOutCanvasGroup <1>__FadeOutCanvasGroup;

			public static hook_FadeInSprite <2>__FadeInSprite;

			public static hook_FadeOutSprite <3>__FadeOutSprite;

			public static hook_FadeInCanvasGroupAfterDelay <4>__FadeInAfterDelay;

			public static Manipulator <5>__DecreaseWait;

			public static Manipulator <6>__PauseGameToggle;

			public static Manipulator <7>__RunGame;
		}

		private readonly (Type, string, Manipulator)[] ILHooks;

		private readonly List<ILHook> _hooked;

		public override void Initialize()
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Expected O, but got Unknown
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Expected O, but got Unknown
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Expected O, but got Unknown
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Expected O, but got Unknown
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Expected O, but got Unknown
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Expected O, but got Unknown
			(Type, string, Manipulator)[] iLHooks = ILHooks;
			for (int i = 0; i < iLHooks.Length; i++)
			{
				var (type, name, val) = iLHooks[i];
				_hooked.Add(new ILHook((MethodBase)Extensions.GetStateMachineTarget(type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), val));
			}
			object obj = <>O.<0>__FadeInCanvasGroupAlpha;
			if (obj == null)
			{
				hook_FadeInCanvasGroupAlpha val2 = FadeInCanvasGroupAlpha;
				<>O.<0>__FadeInCanvasGroupAlpha = val2;
				obj = (object)val2;
			}
			UIManager.FadeInCanvasGroupAlpha += (hook_FadeInCanvasGroupAlpha)obj;
			object obj2 = <>O.<1>__FadeOutCanvasGroup;
			if (obj2 == null)
			{
				hook_FadeOutCanvasGroup val3 = FadeOutCanvasGroup;
				<>O.<1>__FadeOutCanvasGroup = val3;
				obj2 = (object)val3;
			}
			UIManager.FadeOutCanvasGroup += (hook_FadeOutCanvasGroup)obj2;
			object obj3 = <>O.<2>__FadeInSprite;
			if (obj3 == null)
			{
				hook_FadeInSprite val4 = FadeInSprite;
				<>O.<2>__FadeInSprite = val4;
				obj3 = (object)val4;
			}
			UIManager.FadeInSprite += (hook_FadeInSprite)obj3;
			object obj4 = <>O.<3>__FadeOutSprite;
			if (obj4 == null)
			{
				hook_FadeOutSprite val5 = FadeOutSprite;
				<>O.<3>__FadeOutSprite = val5;
				obj4 = (object)val5;
			}
			UIManager.FadeOutSprite += (hook_FadeOutSprite)obj4;
			object obj5 = <>O.<4>__FadeInAfterDelay;
			if (obj5 == null)
			{
				hook_FadeInCanvasGroupAfterDelay val6 = FadeInAfterDelay;
				<>O.<4>__FadeInAfterDelay = val6;
				obj5 = (object)val6;
			}
			SaveSlotButton.FadeInCanvasGroupAfterDelay += (hook_FadeInCanvasGroupAfterDelay)obj5;
			EaseColor.OnEnter += new hook_OnEnter(EaseStartBlanker);
		}

		public override void Unload()
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Expected O, but got Unknown
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Expected O, but got Unknown
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Expected O, but got Unknown
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Expected O, but got Unknown
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Expected O, but got Unknown
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Expected O, but got Unknown
			foreach (ILHook item in _hooked)
			{
				if (item != null)
				{
					item.Dispose();
				}
			}
			_hooked.Clear();
			object obj = <>O.<0>__FadeInCanvasGroupAlpha;
			if (obj == null)
			{
				hook_FadeInCanvasGroupAlpha val = FadeInCanvasGroupAlpha;
				<>O.<0>__FadeInCanvasGroupAlpha = val;
				obj = (object)val;
			}
			UIManager.FadeInCanvasGroupAlpha -= (hook_FadeInCanvasGroupAlpha)obj;
			object obj2 = <>O.<1>__FadeOutCanvasGroup;
			if (obj2 == null)
			{
				hook_FadeOutCanvasGroup val2 = FadeOutCanvasGroup;
				<>O.<1>__FadeOutCanvasGroup = val2;
				obj2 = (object)val2;
			}
			UIManager.FadeOutCanvasGroup -= (hook_FadeOutCanvasGroup)obj2;
			object obj3 = <>O.<2>__FadeInSprite;
			if (obj3 == null)
			{
				hook_FadeInSprite val3 = FadeInSprite;
				<>O.<2>__FadeInSprite = val3;
				obj3 = (object)val3;
			}
			UIManager.FadeInSprite -= (hook_FadeInSprite)obj3;
			object obj4 = <>O.<3>__FadeOutSprite;
			if (obj4 == null)
			{
				hook_FadeOutSprite val4 = FadeOutSprite;
				<>O.<3>__FadeOutSprite = val4;
				obj4 = (object)val4;
			}
			UIManager.FadeOutSprite -= (hook_FadeOutSprite)obj4;
			object obj5 = <>O.<4>__FadeInAfterDelay;
			if (obj5 == null)
			{
				hook_FadeInCanvasGroupAfterDelay val5 = FadeInAfterDelay;
				<>O.<4>__FadeInAfterDelay = val5;
				obj5 = (object)val5;
			}
			SaveSlotButton.FadeInCanvasGroupAfterDelay -= (hook_FadeInCanvasGroupAfterDelay)obj5;
			EaseColor.OnEnter -= new hook_OnEnter(EaseStartBlanker);
		}

		private static void RunGame(ILContext il)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			ILCursor val = new ILCursor(il).Goto(0, (MoveType)0, false);
			while (val.TryGotoNext(new Func<Instruction, bool>[1]
			{
				(Instruction x) => ILPatternMatchingExt.MatchLdcR4(x, 2.6f)
			}))
			{
				val.Next.Operand = 0.05f;
			}
		}

		private static void PauseGameToggle(ILContext il)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			ILCursor val = new ILCursor(il).Goto(0, (MoveType)0, false);
			while (val.TryGotoNext(new Func<Instruction, bool>[1]
			{
				(Instruction x) =>
				{
					float num = default(float);
					return ILPatternMatchingExt.MatchLdcR4(x, ref num);
				}
			}))
			{
				if (!(Mathf.Abs((float)val.Next.Operand - 1f) < Mathf.Epsilon))
				{
					val.Next.Operand = 0f;
				}
			}
		}

		private static void DecreaseWait(ILContext il)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			ILCursor val = new ILCursor(il).Goto(0, (MoveType)0, false);
			while (val.TryGotoNext(new Func<Instruction, bool>[1]
			{
				(Instruction x) =>
				{
					float num = default(float);
					return ILPatternMatchingExt.MatchLdcR4(x, ref num);
				}
			}))
			{
				val.Next.Operand = 0f;
			}
		}

		private static IEnumerator FadeOutSprite(orig_FadeOutSprite orig, UIManager self, SpriteRenderer sprite)
		{
			if (!((Object)(object)sprite == (Object)null))
			{
				sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f);
			}
			yield break;
		}

		private static IEnumerator FadeInSprite(orig_FadeInSprite orig, UIManager self, SpriteRenderer sprite)
		{
			if (!((Object)(object)sprite == (Object)null))
			{
				sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
			}
			yield break;
		}

		private static IEnumerator FadeInAfterDelay(orig_FadeInCanvasGroupAfterDelay orig, SaveSlotButton self, float delay, CanvasGroup cg)
		{
			if (!((Object)(object)cg == (Object)null))
			{
				((Component)cg).gameObject.SetActive(true);
				cg.alpha = 1f;
				cg.interactable = true;
			}
			yield break;
		}

		private static IEnumerator FadeOutCanvasGroup(orig_FadeOutCanvasGroup orig, UIManager self, CanvasGroup cg)
		{
			if (!((Object)(object)cg == (Object)null))
			{
				cg.interactable = false;
				cg.alpha = 0f;
				((Component)cg).gameObject.SetActive(false);
			}
			yield break;
		}

		private static IEnumerator FadeInCanvasGroupAlpha(orig_FadeInCanvasGroupAlpha orig, UIManager self, CanvasGroup cg, float end)
		{
			if (!((Object)(object)cg == (Object)null))
			{
				((Component)cg).gameObject.SetActive(true);
				cg.alpha = end;
				cg.interactable = true;
			}
			yield break;
		}

		private void EaseStartBlanker(orig_OnEnter orig, EaseColor self)
		{
			if (((FsmStateAction)self).Fsm.Name == "Blanker Control" && ((Object)((FsmStateAction)self).Fsm.FsmComponent).name == "Start Blanker" && ((FsmStateAction)self).State.Name == "Fade In")
			{
				((EaseFsmAction)self).time = FsmFloat.op_Implicit(0.05f);
			}
			orig.Invoke(self);
		}

		public FastMenus()
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected O, but got Unknown
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Expected O, but got Unknown
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Expected O, but got Unknown
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Expected O, but got Unknown
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Expected O, but got Unknown
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Expected O, but got Unknown
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Expected O, but got Unknown
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Expected O, but got Unknown
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Expected O, but got Unknown
			(Type, string, Manipulator)[] array = new(Type, string, Manipulator)[9];
			Type typeFromHandle = typeof(UIManager);
			object obj = <>O.<5>__DecreaseWait;
			if (obj == null)
			{
				Manipulator val = DecreaseWait;
				<>O.<5>__DecreaseWait = val;
				obj = (object)val;
			}
			array[0] = (typeFromHandle, "HideSaveProfileMenu", (Manipulator)obj);
			Type typeFromHandle2 = typeof(UIManager);
			object obj2 = <>O.<5>__DecreaseWait;
			if (obj2 == null)
			{
				Manipulator val2 = DecreaseWait;
				<>O.<5>__DecreaseWait = val2;
				obj2 = (object)val2;
			}
			array[1] = (typeFromHandle2, "HideCurrentMenu", (Manipulator)obj2);
			Type typeFromHandle3 = typeof(UIManager);
			object obj3 = <>O.<5>__DecreaseWait;
			if (obj3 == null)
			{
				Manipulator val3 = DecreaseWait;
				<>O.<5>__DecreaseWait = val3;
				obj3 = (object)val3;
			}
			array[2] = (typeFromHandle3, "HideMenu", (Manipulator)obj3);
			Type typeFromHandle4 = typeof(UIManager);
			object obj4 = <>O.<5>__DecreaseWait;
			if (obj4 == null)
			{
				Manipulator val4 = DecreaseWait;
				<>O.<5>__DecreaseWait = val4;
				obj4 = (object)val4;
			}
			array[3] = (typeFromHandle4, "ShowMenu", (Manipulator)obj4);
			Type typeFromHandle5 = typeof(UIManager);
			object obj5 = <>O.<5>__DecreaseWait;
			if (obj5 == null)
			{
				Manipulator val5 = DecreaseWait;
				<>O.<5>__DecreaseWait = val5;
				obj5 = (object)val5;
			}
			array[4] = (typeFromHandle5, "GoToProfileMenu", (Manipulator)obj5);
			Type typeFromHandle6 = typeof(GameManager);
			object obj6 = <>O.<6>__PauseGameToggle;
			if (obj6 == null)
			{
				Manipulator val6 = PauseGameToggle;
				<>O.<6>__PauseGameToggle = val6;
				obj6 = (object)val6;
			}
			array[5] = (typeFromHandle6, "PauseGameToggle", (Manipulator)obj6);
			Type typeFromHandle7 = typeof(GameManager);
			object obj7 = <>O.<7>__RunGame;
			if (obj7 == null)
			{
				Manipulator val7 = RunGame;
				<>O.<7>__RunGame = val7;
				obj7 = (object)val7;
			}
			array[6] = (typeFromHandle7, "RunStartNewGame", (Manipulator)obj7);
			Type typeFromHandle8 = typeof(GameManager);
			object obj8 = <>O.<7>__RunGame;
			if (obj8 == null)
			{
				Manipulator val8 = RunGame;
				<>O.<7>__RunGame = val8;
				obj8 = (object)val8;
			}
			array[7] = (typeFromHandle8, "RunContinueGame", (Manipulator)obj8);
			Type typeFromHandle9 = typeof(SaveSlotButton);
			object obj9 = <>O.<5>__DecreaseWait;
			if (obj9 == null)
			{
				Manipulator val9 = DecreaseWait;
				<>O.<5>__DecreaseWait = val9;
				obj9 = (object)val9;
			}
			array[8] = (typeFromHandle9, "AnimateToSlotState", (Manipulator)obj9);
			ILHooks = array;
			_hooked = new List<ILHook>();
			base..ctor();
		}
	}
	[UsedImplicitly]
	public class FastText : FauxMod
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static hook_ShowNextChar <0>__OnNextChar;
		}

		public override void Initialize()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			object obj = <>O.<0>__OnNextChar;
			if (obj == null)
			{
				hook_ShowNextChar val = OnNextChar;
				<>O.<0>__OnNextChar = val;
				obj = (object)val;
			}
			DialogueBox.ShowNextChar += (hook_ShowNextChar)obj;
		}

		public override void Unload()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			object obj = <>O.<0>__OnNextChar;
			if (obj == null)
			{
				hook_ShowNextChar val = OnNextChar;
				<>O.<0>__OnNextChar = val;
				obj = (object)val;
			}
			DialogueBox.ShowNextChar -= (hook_ShowNextChar)obj;
		}

		private static void OnNextChar(orig_ShowNextChar orig, DialogueBox self)
		{
			TextMeshPro field = Mirror.GetField<DialogueBox, TextMeshPro>(self, "textMesh");
			((TMP_Text)field).maxVisibleCharacters = ((TMP_Text)field).textInfo.pageInfo[self.currentPage - 1].lastCharacterIndex + 1;
		}
	}
	[UsedImplicitly]
	public abstract class FauxMod
	{
		public bool DefaultState { get; } = true;

		internal bool IsLoaded { get; set; }

		protected FauxMod()
		{
		}

		protected FauxMod(bool enabled)
		{
			DefaultState = enabled;
		}

		[PublicAPI]
		internal void Log(object obj)
		{
			Logger.Log($"[QoL : {GetType().Name}] - {obj}");
		}

		public virtual void Initialize()
		{
		}

		public virtual void Unload()
		{
		}

		public static bool IsToggleableFauxMod(Type t)
		{
			if (t.IsSubclassOf(typeof(FauxMod)))
			{
				return IsToggleableUnsafe(t);
			}
			return false;
		}

		public static bool IsToggleable(Type t)
		{
			if (!t.IsSubclassOf(typeof(FauxMod)))
			{
				throw new ArgumentException($"Type {t} is not a FauxMod!", "t");
			}
			return IsToggleableUnsafe(t);
		}

		private static bool IsToggleableUnsafe(Type t)
		{
			return t.GetMethod("Unload").DeclaringType != typeof(FauxMod);
		}
	}
	[UsedImplicitly]
	public class FixFireballs : FauxMod
	{
		private class NextPostPhysicsFrameEvent : FsmStateAction
		{
			[RequiredField]
			public FsmEvent? sendEvent;

			private bool _fixedUpdate;

			public override void Reset()
			{
				sendEvent = null;
			}

			public override void OnEnter()
			{
				_fixedUpdate = false;
			}

			public override void OnFixedUpdate()
			{
				_fixedUpdate = true;
			}

			public override void OnUpdate()
			{
				if (_fixedUpdate)
				{
					((FsmStateAction)this).Finish();
					((FsmStateAction)this).Fsm.Event(sendEvent);
				}
			}
		}

		[CompilerGenerated]
		private static class <>O
		{
			public static hook_OnEnable <0>__CheckFireball;
		}

		public override void Initialize()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			object obj = <>O.<0>__CheckFireball;
			if (obj == null)
			{
				hook_OnEnable val = CheckFireball;
				<>O.<0>__CheckFireball = val;
				obj = (object)val;
			}
			PlayMakerFSM.OnEnable += (hook_OnEnable)obj;
		}

		public override void Unload()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			object obj = <>O.<0>__CheckFireball;
			if (obj == null)
			{
				hook_OnEnable val = CheckFireball;
				<>O.<0>__CheckFireball = val;
				obj = (object)val;
			}
			PlayMakerFSM.OnEnable -= (hook_OnEnable)obj;
		}

		private static void CheckFireball(orig_OnEnable orig, PlayMakerFSM self)
		{
			FsmState val = default(FsmState);
			if (self.FsmName != "Fireball Control" || !FsmUtil.TryGetState(self, "Wall Impact", ref val) || FsmUtil.GetAction<FsmStateAction>(self, "Pause", 0) is NextPostPhysicsFrameEvent)
			{
				orig.Invoke(self);
				return;
			}
			FsmUtil.RemoveAction<NextFrameEvent>(FsmUtil.GetState(self, "Pause"));
			FsmUtil.InsertAction(FsmUtil.GetState(self, "Pause"), 0, (FsmStateAction)(object)new NextPostPhysicsFrameEvent
			{
				sendEvent = FsmEvent.FindEvent("FINISHED")
			});
			orig.Invoke(self);
		}
	}
	[UsedImplicitly]
	public class FixVolume : FauxMod
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static hook_StartRecoil <0>__StartRecoil;

			public static UnityAction<Scene, Scene> <1>__SceneChanged;
		}

		[SerializeToSetting]
		public static float DoubleDamageVolumeModifier = 0.8f;

		public static float Volume => GameManager.instance.GetImplicitCinematicVolume();

		public override void Initialize()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			object obj = <>O.<0>__StartRecoil;
			if (obj == null)
			{
				hook_StartRecoil val = StartRecoil;
				<>O.<0>__StartRecoil = val;
				obj = (object)val;
			}
			HeroController.StartRecoil += (hook_StartRecoil)obj;
			SceneManager.activeSceneChanged += SceneChanged;
		}

		private static void SceneChanged(Scene arg0, Scene arg1)
		{
			if (!((Scene)(ref arg1)).name.ToLower().Contains("dream"))
			{
				return;
			}
			GameObject[] array = Object.FindObjectsOfType<GameObject>();
			foreach (GameObject val in array)
			{
				if (((Object)val).name.Contains("grass"))
				{
					AudioSource component = val.GetComponent<AudioSource>();
					if ((Object)(object)component != (Object)null)
					{
						component.volume = Volume;
					}
				}
			}
		}

		private static IEnumerator StartRecoil(orig_StartRecoil orig, HeroController self, CollisionSide impactside, bool spawndamageeffect, int damageamount)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			self.takeHitDoublePrefab.GetComponent<AudioSource>().volume = Mathf.Clamp(Volume * DoubleDamageVolumeModifier, 0f, 1f);
			yield return orig.Invoke(self, impactside, spawndamageeffect, damageamount);
		}

		public override void Unload()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			object obj = <>O.<0>__StartRecoil;
			if (obj == null)
			{
				hook_StartRecoil val = StartRecoil;
				<>O.<0>__StartRecoil = val;
				obj = (object)val;
			}
			HeroController.StartRecoil -= (hook_StartRecoil)obj;
			SceneManager.activeSceneChanged -= SceneChanged;
		}
	}
	[UsedImplicitly]
	public class InfinitePoP : FauxMod
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static GetBoolProxy <0>__GetBool;

			public static SetBoolProxy <1>__SetBool;

			public static UnityAction<Scene, Scene> <2>__OnSceneChanged;
		}

		public override void Initialize()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			object obj = <>O.<0>__GetBool;
			if (obj == null)
			{
				GetBoolProxy val = GetBool;
				<>O.<0>__GetBool = val;
				obj = (object)val;
			}
			ModHooks.GetPlayerBoolHook += (GetBoolProxy)obj;
			object obj2 = <>O.<1>__SetBool;
			if (obj2 == null)
			{
				SetBoolProxy val2 = SetBool;
				<>O.<1>__SetBool = val2;
				obj2 = (object)val2;
			}
			ModHooks.SetPlayerBoolHook += (SetBoolProxy)obj2;
			SceneManager.activeSceneChanged += OnSceneChanged;
		}

		private static void OnSceneChanged(Scene arg0, Scene arg1)
		{
			if (!(((Scene)(ref arg1)).name != "White_Palace_06"))
			{
				GameObject val = GameObject.Find("Path of Pain Blocker");
				if ((Object)(object)val != (Object)null)
				{
					Object.Destroy((Object)(object)val);
				}
			}
		}

		private static bool SetBool(string field, bool value)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Expected O, but got Unknown
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Expected O, but got Unknown
			if (field != "newDataBindingSeal" || !value)
			{
				return value;
			}
			SceneData sceneData = GameManager.instance.sceneData;
			sceneData.SaveMyState(new PersistentBoolData
			{
				sceneName = "White_Palace_17",
				id = "WP Lever",
				activated = false
			});
			sceneData.SaveMyState(new PersistentBoolData
			{
				sceneName = "White_Palace_17",
				id = "Collapser Small",
				activated = false
			});
			sceneData.SaveMyState(new PersistentBoolData
			{
				sceneName = "White_Palace_06",
				id = "Breakable Wall Ruin Lift",
				activated = false
			});
			return PlayerData.instance.newDataBindingSeal;
		}

		private static bool GetBool(string originalset, bool orig)
		{
			return (originalset == "newDataBindingSeal") | orig;
		}

		public override void Unload()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			object obj = <>O.<0>__GetBool;
			if (obj == null)
			{
				GetBoolProxy val = GetBool;
				<>O.<0>__GetBool = val;
				obj = (object)val;
			}
			ModHooks.GetPlayerBoolHook -= (GetBoolProxy)obj;
			object obj2 = <>O.<1>__SetBool;
			if (obj2 == null)
			{
				SetBoolProxy val2 = SetBool;
				<>O.<1>__SetBool = val2;
				obj2 = (object)val2;
			}
			ModHooks.SetPlayerBoolHook -= (SetBoolProxy)obj2;
			SceneManager.activeSceneChanged -= OnSceneChanged;
		}
	}
	[UsedImplicitly]
	public class LockMenuCursor : FauxMod
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static Manipulator <0>__ReplaceLockState;
		}

		private ILHook? _hook;

		public LockMenuCursor()
			: base(enabled: false)
		{
		}

		public override void Initialize()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Expected O, but got Unknown
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Expected O, but got Unknown
			object obj = <>O.<0>__ReplaceLockState;
			if (obj == null)
			{
				Manipulator val = ReplaceLockState;
				<>O.<0>__ReplaceLockState = val;
				obj = (object)val;
			}
			InputHandler.OnGUI += (Manipulator)obj;
			MethodInfo method = typeof(ModHooks).GetMethod("OnCursor", BindingFlags.Static | BindingFlags.NonPublic);
			object obj2 = <>O.<0>__ReplaceLockState;
			if (obj2 == null)
			{
				Manipulator val2 = ReplaceLockState;
				<>O.<0>__ReplaceLockState = val2;
				obj2 = (object)val2;
			}
			_hook = new ILHook((MethodBase)method, (Manipulator)obj2);
		}

		public override void Unload()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			object obj = <>O.<0>__ReplaceLockState;
			if (obj == null)
			{
				Manipulator val = ReplaceLockState;
				<>O.<0>__ReplaceLockState = val;
				obj = (object)val;
			}
			InputHandler.OnGUI -= (Manipulator)obj;
			ILHook? hook = _hook;
			if (hook != null)
			{
				hook.Dispose();
			}
		}

		private static void ReplaceLockState(ILContext il)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			ILCursor val = new ILCursor(il).Goto(0, (MoveType)0, false);
			ILCursor[] array = default(ILCursor[]);
			while (val.TryFindNext(ref array, new Func<Instruction, bool>[2]
			{
				(Instruction instr) => ILPatternMatchingExt.MatchLdcI4(instr, 0),
				(Instruction instr) => ILPatternMatchingExt.MatchCall(instr, typeof(Cursor), "set_lockState")
			}))
			{
				array[0].Remove();
				array[0].EmitDelegate<Func<int>>((Func<int>)(() => 2));
			}
		}
	}
	[UsedImplicitly]
	public class NPCSellAll : FauxMod
	{
		[SerializeToSetting]
		public static bool LemmSellAll = true;

		[SerializeToSetting]
		public static bool JinnSellAll = true;

		[SerializeToSetting]
		public static bool NailsmithBuyAll = true;

		private const int EGG_COST = 450;

		private static readonly int[] RELIC_COST = new int[4] { 200, 450, 800, 1200 };

		private static readonly (int ore, int geo)[] NAIL_UPGRADE_COSTS = new(int, int)[4]
		{
			(0, 250),
			(1, 800),
			(2, 2000),
			(3, 4000)
		};

		public override void Initialize()
		{
			SceneManager.sceneLoaded += PatchVendorNPCs;
		}

		public override void Unload()
		{
			SceneManager.sceneLoaded -= PatchVendorNPCs;
		}

		private static void PatchVendorNPCs(Scene scene, LoadSceneMode lsm)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			switch (((Scene)(ref scene)).name)
			{
			case "Ruins1_05b":
				if (LemmSellAll)
				{
					LemmSell(scene);
				}
				break;
			case "Room_Jinn":
				if (JinnSellAll)
				{
					JinnSell(scene);
				}
				break;
			case "Room_nailsmith":
				if (NailsmithBuyAll)
				{
					NailsmithBuy(scene);
				}
				break;
			}
		}

		private static void LemmSell(Scene scene)
		{
			GameObject val = ((Scene)(ref scene)).GetRootGameObjects().FirstOrDefault((GameObject obj) => ((Object)obj).name == "Relic Dealer");
			if (!((Object)(object)val == (Object)null))
			{
				FsmUtil.AddMethod(FsmUtil.GetState(FSMUtility.LocateMyFSM(val, "npc_control"), "Convo End"), (Action)SellRelics);
			}
		}

		private static void SellRelics()
		{
			PlayerData instance = PlayerData.instance;
			if (instance.GetBool("equippedCharm_10"))
			{
				return;
			}
			for (int i = 1; i <= 4; i++)
			{
				int num = instance.GetInt($"trinket{i}");
				if (num != 0)
				{
					int num2 = num * RELIC_COST[i - 1];
					instance.SetInt($"soldTrinket{i}", instance.GetInt($"soldTrinket{i}") + num);
					instance.SetInt($"trinket{i}", 0);
					HeroController.instance.AddGeo(num2);
				}
			}
		}

		private static void JinnSell(Scene scene)
		{
			GameObject val = ((Scene)(ref scene)).GetRootGameObjects().FirstOrDefault((GameObject obj) => ((Object)obj).name == "Jinn NPC");
			if (!((Object)(object)val == (Object)null))
			{
				FsmUtil.RemoveAction<PlayerDataBoolTest>(FsmUtil.GetState(FSMUtility.LocateMyFSM(val, "Wake and Animate"), "Talk NPC?"));
				FsmUtil.InsertMethod(FsmUtil.GetState(FSMUtility.LocateMyFSM(((Component)val.transform.Find("Talk NPC")).gameObject, "Conversation Control"), "Talk Finish"), 0, (Action)SellEggs);
			}
		}

		private static void SellEggs()
		{
			PlayerData instance = PlayerData.instance;
			int num = instance.GetInt("rancidEggs");
			if (num != 0)
			{
				int num2 = num * 450;
				instance.SetInt("jinnEggsSold", instance.GetInt("jinnEggsSold") + num);
				instance.SetInt("rancidEggs", 0);
				HeroController.instance.AddGeo(num2);
			}
		}

		private static void NailsmithBuy(Scene scene)
		{
			bool right = false;
			GameObject val = ((Scene)(ref scene)).GetRootGameObjects().FirstOrDefault((GameObject obj) => ((Object)obj).name == "Nailsmith");
			if ((Object)(object)val == (Object)null)
			{
				return;
			}
			PlayMakerFSM convo = FSMUtility.LocateMyFSM(val, "Conversation Control");
			FsmState state = FsmUtil.GetState(convo, "Box Up");
			FsmUtil.AddTransition(state, "BOUGHT ALL", "Box Up 4");
			FsmUtil.InsertMethod(state, 0, (Action)delegate
			{
				if (right && BuyNailUpgrades())
				{
					convo.SendEvent("BOUGHT ALL");
				}
			});
			FsmUtil.InsertMethod(FsmUtil.GetState(FSMUtility.LocateMyFSM(val, "npc_control"), "Move Hero Left"), 0, (Action)delegate
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				Bounds bounds = ((Component)HeroController.instance).GetComponent<Collider2D>().bounds;
				float x = ((Bounds)(ref bounds)).min.x;
				right = 18.5f < x;
			});
		}

		private static bool BuyNailUpgrades()
		{
			PlayerData pd = PlayerData.instance;
			int num = pd.GetInt("nailSmithUpgrades");
			if (num > 3)
			{
				return false;
			}
			int num2 = 0;
			foreach (var (num3, num4) in NAIL_UPGRADE_COSTS.Skip(num).TakeWhile(((int ore, int geo) x) => x.ore <= pd.GetInt("ore") && x.geo <= pd.GetInt("geo")))
			{
				pd.IntAdd("ore", -num3);
				HeroController.instance.TakeGeo(num4);
				num2++;
			}
			if (num2 <= 0)
			{
				return false;
			}
			GameManager instance = GameManager.instance;
			pd.SetBool("honedNail", true);
			pd.IntAdd("nailDamage", num2 * 4);
			pd.IntAdd("nailSmithUpgrades", num2);
			PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
			instance.ResetSemiPersistentItems();
			instance.TimePasses();
			instance.StoryRecord_upgradeNail();
			return true;
		}
	}
	[UsedImplicitly]
	public class OldDashmaster : FauxMod
	{
		public OldDashmaster()
			: base(enabled: false)
		{
		}

		public override void Initialize()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			Unload();
			HeroController.HeroDash += new Manipulator(HeroDash);
		}

		private void HeroDash(ILContext il)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			ILCursor val = new ILCursor(il);
			while (val.TryGotoNext(new Func<Instruction, bool>[4]
			{
				(Instruction i) => ILPatternMatchingExt.MatchLdloc(i, 0),
				(Instruction i) => ILPatternMatchingExt.MatchLdfld<HeroActions>(i, "left") || ILPatternMatchingExt.MatchLdfld<HeroActions>(i, "right"),
				(Instruction i) => ILPatternMatchingExt.MatchCallvirt<OneAxisInputControl>(i, "get_IsPressed"),
				(Instruction i) =>
				{
					ILLabel val2 = default(ILLabel);
					return ILPatternMatchingExt.MatchBrtrue(i, ref val2);
				}
			}))
			{
				val.RemoveRange(4);
			}
		}

		public override void Unload()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			HeroController.HeroDash -= new Manipulator(HeroDash);
		}
	}
	public class PatchedBosses : FauxMod
	{
		[SerializeToSetting]
		public static bool SporeShroomHollowKnight = true;

		[SerializeToSetting]
		public static bool NoBackrolls;

		[SerializeToSetting]
		public static bool SleepyCrystalGuardian;

		public override void Initialize()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			ExtraDamageable.RecieveExtraDamage += new Manipulator(AllowRecieveExtraDamage);
			DamageEffectTicker.OnTriggerExit2D += new hook_OnTriggerExit2D(KeepDamagingInactives);
			SceneManager.sceneLoaded += BossFsmChanges;
		}

		public override void Unload()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			ExtraDamageable.RecieveExtraDamage -= new Manipulator(AllowRecieveExtraDamage);
			DamageEffectTicker.OnTriggerExit2D -= new hook_OnTriggerExit2D(KeepDamagingInactives);
			SceneManager.sceneLoaded -= BossFsmChanges;
		}

		private void BossFsmChanges(Scene scene, LoadSceneMode lsm)
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			switch (((Scene)(ref scene)).name)
			{
			case "Ruins2_03_boss":
				if (!NoBackrolls)
				{
					break;
				}
				goto IL_003f;
			case "GG_Watcher_Knights":
				if (!NoBackrolls)
				{
					break;
				}
				goto IL_003f;
			case "Mines_18_boss":
				{
					if (SleepyCrystalGuardian)
					{
						((MonoBehaviour)GameManager.instance).StartCoroutine(FixCrystalGuardian());
					}
					break;
				}
				IL_003f:
				((MonoBehaviour)GameManager.instance).StartCoroutine(FixWatchers(scene));
				break;
			}
		}

		private IEnumerator FixWatchers(Scene scene)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			yield return null;
			PlayMakerFSM[] componentsInChildren = ((Scene)(ref scene)).GetRootGameObjects().First((GameObject obj) => ((Object)obj).name == "Battle Control").GetComponentsInChildren<PlayMakerFSM>();
			foreach (PlayMakerFSM val in componentsInChildren)
			{
				if (val.FsmName == "Black Knight")
				{
					FsmUtil.RemoveTransition(FsmUtil.GetState(val, "In Range Choice"), "In Range Double");
				}
			}
		}

		private IEnumerator FixCrystalGuardian()
		{
			yield return null;
			GameObject val = GameObject.Find("Mega Zombie Beam Miner (1)");
			FsmState val2 = default(FsmState);
			if (!((Object)(object)val == (Object)null) && FsmUtil.TryGetState(FSMUtility.LocateMyFSM(val, "Beam Miner"), "Sleep", ref val2))
			{
				val2.Transitions = val2.Transitions.Where((FsmTransition x) => x.FsmEvent.Name != "EXTRA DAMAGED" && x.FsmEvent.Name != "TOOK DAMAGE").ToArray();
			}
		}

		private void AllowRecieveExtraDamage(ILContext il)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			ILCursor val = new ILCursor(il);
			if (val.TryGotoNext((MoveType)2, new Func<Instruction, bool>[2]
			{
				(Instruction i) => ILPatternMatchingExt.MatchLdarg(i, 0),
				(Instruction i) => ILPatternMatchingExt.MatchLdfld<ExtraDamageable>(i, "damagedThisFrame")
			}))
			{
				val.EmitDelegate<Func<bool, bool>>((Func<bool, bool>)((bool x) => x && !SporeShroomHollowKnight));
			}
		}

		private void KeepDamagingInactives(orig_OnTriggerExit2D orig, DamageEffectTicker self, Collider2D otherCollider)
		{
			if (((Component)otherCollider).gameObject.activeSelf || !SporeShroomHollowKnight)
			{
				orig.Invoke(self, otherCollider);
			}
		}
	}
	[UsedImplicitly]
	public class SalubraKiller : FauxMod
	{
		public override void Initialize()
		{
			SceneManager.activeSceneChanged += SceneChanged;
		}

		private static void SceneChanged(Scene arg0, Scene arg1)
		{
			if (!((Object)(object)HeroController.instance == (Object)null))
			{
				((MonoBehaviour)HeroController.instance).StartCoroutine(KillSalubra());
			}
			static IEnumerator KillSalubra()
			{
				yield return null;
				GameObject val = GameObject.Find("Blessing Ghost");
				if (!((Object)(object)val == (Object)null))
				{
					FsmUtil.GetAction<ActivateGameObject>(FSMUtility.LocateMyFSM(val, "Blessing Control"), "Start Blessing").activate.Value = false;
				}
			}
		}

		public override void Unload()
		{
			SceneManager.activeSceneChanged -= SceneChanged;
		}
	}
	[UsedImplicitly]
	public class SaneUumuu : FauxMod
	{
		private NonBouncer _coro;

		public override void Initialize()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GameObject val = new GameObject();
			_coro = val.AddComponent<NonBouncer>();
			Object.DontDestroyOnLoad((Object)(object)_coro);
			SceneManager.sceneLoaded += StartRoutine;
		}

		private void StartRoutine(Scene scene, LoadSceneMode lsm)
		{
			if (((Scene)(ref scene)).name == "Fungus3_archive_02_boss")
			{
				((MonoBehaviour)_coro).StartCoroutine(FixUumuu());
			}
		}

		private static IEnumerator FixUumuu()
		{
			yield return null;
			GameObject val = GameObject.Find("Mega Jellyfish");
			if (!((Object)(object)val == (Object)null))
			{
				PlayMakerFSM fsm = FSMUtility.LocateMyFSM(val, "Mega Jellyfish");
				FsmUtil.GetAction<WaitRandom>(FsmUtil.GetState(fsm, "Idle")).timeMax = FsmFloat.op_Implicit(1.5f);
				FsmUtil.GetAction<RandomFloat>(FsmUtil.GetState(fsm, "Set Timer")).max = FsmFloat.op_Implicit(2f);
				fsm.FsmVariables.GetFsmFloat("Quirrel Time").Value = 4f;
				FsmState state = FsmUtil.GetState(fsm, "Choice");
				FsmUtil.RemoveAction<SendRandomEventV2>(state);
				FsmUtil.AddMethod(state, (Action)delegate
				{
					SetUumuuPattern(fsm);
				});
				FsmUtil.AddMethod(FsmUtil.GetState(fsm, "Recover"), (Action)delegate
				{
					fsm.FsmVariables.GetFsmInt("Ct Multizap").Value = 0;
				});
				val.GetComponent<Recoil>().SetRecoilSpeed(0f);
				val.GetComponent<HealthManager>().hp = 250;
			}
		}

		private static void SetUumuuPattern(PlayMakerFSM fsm)
		{
			if (fsm.FsmVariables.GetFsmInt("Ct Multizap").Value < 2)
			{
				fsm.Fsm.Event(fsm.FsmEvents.First((FsmEvent e) => e.Name == "MULTIZAP"));
				FsmInt fsmInt = fsm.FsmVariables.GetFsmInt("Ct Multizap");
				int value = fsmInt.Value;
				fsmInt.Value = value + 1;
			}
			else
			{
				fsm.Fsm.Event(fsm.FsmEvents.First((FsmEvent e) => e.Name == "CHASE"));
			}
		}

		public override void Unload()
		{
			Object.Destroy((Object)(object)_coro);
			SceneManager.sceneLoaded -= StartRoutine;
		}
	}
	[UsedImplicitly]
	public class SkipCutscenes : FauxMod
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static hook_Begin <0>__CinematicBegin;

			public static hook_Begin <1>__FadeBegin;

			public static hook_Begin <2>__AnimatorBegin;

			public static hook_SetSkipMode <3>__OnSetSkip;

			public static hook_BeginSceneTransitionRoutine <4>__OnBeginSceneTransition;

			public static hook_OnEnter <5>__FastEaseColor;

			public static hook_FadeSceneInWithDelay <6>__NoFade;

			public static hook_OnEnter <7>__MageLordPhaseTransitionSkip;

			public static Action <8>__OnNewGame;

			public static UnityAction<Scene, Scene> <9>__FsmSkips;
		}

		[SerializeToSetting]
		public static bool DreamersGet = true;

		[SerializeToSetting]
		public static bool AbsoluteRadiance = true;

		[SerializeToSetting]
		public static bool AbyssShriekGet = true;

		[SerializeToSetting]
		public static bool AfterKingsBrandGet = true;

		[SerializeToSetting]
		public static bool BlackEggOpen = true;

		[SerializeToSetting]
		public static bool StagArrive = true;

		[SerializeToSetting]
		public static bool HallOfGodsStatues = true;

		[SerializeToSetting]
		public static bool GodhomeEntry = true;

		[SerializeToSetting]
		public static bool PureVesselRoar = true;

		[SerializeToSetting]
		public static bool FirstTimeBosses = true;

		[SerializeToSetting]
		public static bool FirstCharm = true;

		[SerializeToSetting]
		public static bool AutoSkipCinematics = true;

		[SerializeToSetting]
		public static bool AllowSkippingNonskippable = true;

		[SerializeToSetting]
		public static bool SkipCutscenesWithoutPrompt = true;

		[SerializeToSetting]
		public static bool InstantSceneFadeIns = true;

		[SerializeToSetting]
		public static bool SoulMasterPhaseTransitionSkip = true;

		private const string GUARDIAN = "Dream_Guardian_";

		private static readonly string[] DREAMERS = new string[3] { "Deepnest_Spider_Town", "Fungus3_archive_02", "Ruins2_Watcher_Room" };

		private static readonly string[] ALL_DREAMER_BOOLS = new string[16]
		{
			"corniferAtHome", "iseldaConvo1", "dungDefenderSleeping", "corn_crossroadsLeft", "corn_greenpathLeft", "corn_fogCanyonLeft", "corn_fungalWastesLeft", "corn_cityLeft", "corn_waterwaysLeft", "corn_minesLeft",
			"corn_cliffsLeft", "corn_deepnestLeft", "corn_outskirtsLeft", "corn_royalGardensLeft", "corn_abyssLeft", "metIselda"
		};

		private static readonly (Func<bool>, Func<Scene, IEnumerator>)[] FSM_SKIPS = new(Func<bool>, Func<Scene, IEnumerator>)[8]
		{
			(() => DreamersGet, DreamerFsm),
			(() => AbsoluteRadiance, AbsRadSkip),
			(() => PureVesselRoar, HKPrimeSkip),
			(() => HallOfGodsStatues, StatueWait),
			(() => StagArrive, StagCutscene),
			(() => AbyssShriekGet, AbyssShriekPickup),
			(() => AfterKingsBrandGet, KingsBrand),
			(() => BlackEggOpen, BlackEgg)
		};

		private static readonly string[] PD_BOOLS = new string[5] { "unchainedHollowKnight", "encounteredMimicSpider", "infectedKnightEncountered", "mageLordEncountered", "mageLordEncountered_2" };

		public override void Initialize()
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
			object obj = <>O.<0>__CinematicBegin;
			if (obj == null)
			{
				hook_Begin val = CinematicBegin;
				<>O.<0>__CinematicBegin = val;
				obj = (object)val;
			}
			CinematicSequence.Begin += (hook_Begin)obj;
			object obj2 = <>O.<1>__FadeBegin;
			if (obj2 == null)
			{
				hook_Begin val2 = FadeBegin;
				<>O.<1>__FadeBegin = val2;
				obj2 = (object)val2;
			}
			FadeSequence.Begin += (hook_Begin)obj2;
			object obj3 = <>O.<2>__AnimatorBegin;
			if (obj3 == null)
			{
				hook_Begin val3 = AnimatorBegin;
				<>O.<2>__AnimatorBegin = val3;
				obj3 = (object)val3;
			}
			AnimatorSequence.Begin += (hook_Begin)obj3;
			object obj4 = <>O.<3>__OnSetSkip;
			if (obj4 == null)
			{
				hook_SetSkipMode val4 = OnSetSkip;
				<>O.<3>__OnSetSkip = val4;
				obj4 = (object)val4;
			}
			InputHandler.SetSkipMode += (hook_SetSkipMode)obj4;
			object obj5 = <>O.<4>__OnBeginSceneTransition;
			if (obj5 == null)
			{
				hook_BeginSceneTransitionRoutine val5 = OnBeginSceneTransition;
				<>O.<4>__OnBeginSceneTransition = val5;
				obj5 = (object)val5;
			}
			GameManager.BeginSceneTransitionRoutine += (hook_BeginSceneTransitionRoutine)obj5;
			object obj6 = <>O.<5>__FastEaseColor;
			if (obj6 == null)
			{
				hook_OnEnter val6 = FastEaseColor;
				<>O.<5>__FastEaseColor = val6;
				obj6 = (object)val6;
			}
			EaseColor.OnEnter += (hook_OnEnter)obj6;
			object obj7 = <>O.<6>__NoFade;
			if (obj7 == null)
			{
				hook_FadeSceneInWithDelay val7 = NoFade;
				<>O.<6>__NoFade = val7;
				obj7 = (object)val7;
			}
			GameManager.FadeSceneInWithDelay += (hook_FadeSceneInWithDelay)obj7;
			object obj8 = <>O.<7>__MageLordPhaseTransitionSkip;
			if (obj8 == null)
			{
				hook_OnEnter val8 = MageLordPhaseTransitionSkip;
				<>O.<7>__MageLordPhaseTransitionSkip = val8;
				obj8 = (object)val8;
			}
			GGCheckIfBossScene.OnEnter += (hook_OnEnter)obj8;
			ModHooks.NewGameHook += OnNewGame;
			SceneManager.activeSceneChanged += FsmSkips;
		}

		public override void Unload()
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
			object obj = <>O.<0>__CinematicBegin;
			if (obj == null)
			{
				hook_Begin val = CinematicBegin;
				<>O.<0>__CinematicBegin = val;
				obj = (object)val;
			}
			CinematicSequence.Begin -= (hook_Begin)obj;
			object obj2 = <>O.<1>__FadeBegin;
			if (obj2 == null)
			{
				hook_Begin val2 = FadeBegin;
				<>O.<1>__FadeBegin = val2;
				obj2 = (object)val2;
			}
			FadeSequence.Begin -= (hook_Begin)obj2;
			object obj3 = <>O.<2>__AnimatorBegin;
			if (obj3 == null)
			{
				hook_Begin val3 = AnimatorBegin;
				<>O.<2>__AnimatorBegin = val3;
				obj3 = (object)val3;
			}
			AnimatorSequence.Begin -= (hook_Begin)obj3;
			object obj4 = <>O.<3>__OnSetSkip;
			if (obj4 == null)
			{
				hook_SetSkipMode val4 = OnSetSkip;
				<>O.<3>__OnSetSkip = val4;
				obj4 = (object)val4;
			}
			InputHandler.SetSkipMode -= (hook_SetSkipMode)obj4;
			object obj5 = <>O.<4>__OnBeginSceneTransition;
			if (obj5 == null)
			{
				hook_BeginSceneTransitionRoutine val5 = OnBeginSceneTransition;
				<>O.<4>__OnBeginSceneTransition = val5;
				obj5 = (object)val5;
			}
			GameManager.BeginSceneTransitionRoutine -= (hook_BeginSceneTransitionRoutine)obj5;
			object obj6 = <>O.<5>__FastEaseColor;
			if (obj6 == null)
			{
				hook_OnEnter val6 = FastEaseColor;
				<>O.<5>__FastEaseColor = val6;
				obj6 = (object)val6;
			}
			EaseColor.OnEnter -= (hook_OnEnter)obj6;
			object obj7 = <>O.<6>__NoFade;
			if (obj7 == null)
			{
				hook_FadeSceneInWithDelay val7 = NoFade;
				<>O.<6>__NoFade = val7;
				obj7 = (object)val7;
			}
			GameManager.FadeSceneInWithDelay -= (hook_FadeSceneInWithDelay)obj7;
			object obj8 = <>O.<7>__MageLordPhaseTransitionSkip;
			if (obj8 == null)
			{
				hook_OnEnter val8 = MageLordPhaseTransitionSkip;
				<>O.<7>__MageLordPhaseTransitionSkip = val8;
				obj8 = (object)val8;
			}
			GGCheckIfBossScene.OnEnter -= (hook_OnEnter)obj8;
			SceneManager.activeSceneChanged -= FsmSkips;
			ModHooks.NewGameHook -= OnNewGame;
		}

		private static void OnNewGame()
		{
			if (FirstCharm)
			{
				PlayerData.instance.SetBool("hasCharm", true);
			}
			if (GodhomeEntry)
			{
				PlayerData.instance.SetBool("enteredGGAtrium", true);
			}
			if (FirstTimeBosses)
			{
				string[] pD_BOOLS = PD_BOOLS;
				foreach (string text in pD_BOOLS)
				{
					PlayerData.instance.SetBool(text, true);
				}
			}
		}

		private static IEnumerator NoFade(orig_FadeSceneInWithDelay orig, GameManager self, float delay)
		{
			yield return orig.Invoke(self, InstantSceneFadeIns ? 0f : delay);
		}

		private static void FastEaseColor(orig_OnEnter orig, EaseColor self)
		{
			if (InstantSceneFadeIns && ((Object)((FsmStateAction)self).Owner).name == "Blanker White" && Math.Abs((double)((EaseFsmAction)self).time.Value - 0.3) < 0.05)
			{
				((EaseFsmAction)self).time.Value = 0.066f;
			}
			orig.Invoke(self);
		}

		private static void MageLordPhaseTransitionSkip(orig_OnEnter orig, GGCheckIfBossScene self)
		{
			if (SoulMasterPhaseTransitionSkip && ((Object)((FsmStateAction)self).Owner.transform).name.Contains("Corpse Mage") && ((FsmStateAction)self).Fsm.ActiveStateName.Contains("Quick Death?"))
			{
				FsmStateAction obj = ((FsmStateAction)self).Fsm.ActiveState.Actions[1];
				PlayerDataBoolTest val = (PlayerDataBoolTest)(object)((obj is PlayerDataBoolTest) ? obj : null);
				if (val != null)
				{
					((FsmStateAction)self).Fsm.Event(val.isTrue);
					return;
				}
			}
			orig.Invoke(self);
		}

		private static void FsmSkips(Scene arg0, Scene arg1)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			HeroController instance = HeroController.instance;
			if ((Object)(object)instance == (Object)null)
			{
				return;
			}
			(Func<bool>, Func<Scene, IEnumerator>)[] fSM_SKIPS = FSM_SKIPS;
			for (int i = 0; i < fSM_SKIPS.Length; i++)
			{
				var (func, func2) = fSM_SKIPS[i];
				if (func())
				{
					((MonoBehaviour)instance).StartCoroutine(func2(arg1));
				}
			}
		}

		private static IEnumerator StagCutscene(Scene _)
		{
			yield return null;
			GameObject val = GameObject.Find("Stag");
			if (!((Object)(object)val == (Object)null))
			{
				PlayMakerFSM obj = FSMUtility.LocateMyFSM(val, "Stag Control");
				FsmUtil.RemoveAction<Wait>(FsmUtil.GetState(obj, "Arrive Pause"));
				FsmUtil.RemoveAction<Wait>(FsmUtil.GetState(obj, "Activate"));
				val.GetComponent<tk2dSpriteAnimator>().GetClipByName("Arrive").fps = 600f;
				GameObject value = FsmUtil.GetAction<Tk2dPlayAnimationWithEvents>(FsmUtil.GetState(obj, "Open Grate")).gameObject.GameObject.Value;
				if (!((Object)(object)value == (Object)null))
				{
					value.GetComponent<tk2dSpriteAnimator>().GetClipByName("Grate_disappear").fps = 600f;
				}
			}
		}

		private static IEnumerator StatueWait(Scene arg1)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			if (((Scene)(ref arg1)).name != "GG_Workshop")
			{
				yield break;
			}
			foreach (PlayMakerFSM item in from x in Object.FindObjectsOfType<PlayMakerFSM>()
				where x.FsmName == "GG Boss UI"
				select x)
			{
				FsmUtil.ChangeTransition(FsmUtil.GetState(item, "On Left"), "FINISHED", "Dream Box Down");
				FsmUtil.ChangeTransition(FsmUtil.GetState(item, "On Right"), "FINISHED", "Dream Box Down");
				FsmUtil.InsertAction(FsmUtil.GetState(item, "Dream Box Down"), 0, (FsmStateAction)(object)FsmUtil.GetAction<SetPlayerDataString>(item, "Impact"));
			}
		}

		private static IEnumerator HKPrimeSkip(Scene arg1)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			if (!(((Scene)(ref arg1)).name != "GG_Hollow_Knight"))
			{
				yield return null;
				PlayMakerFSM obj = FSMUtility.LocateMyFSM(GameObject.Find("HK Prime"), "Control");
				FsmUtil.ChangeTransition(FsmUtil.GetState(obj, "Init"), "FINISHED", "Intro Roar");
				FsmUtil.GetAction<Wait>(obj, "Intro 2").time = FsmFloat.op_Implicit(0.01f);
				FsmUtil.GetAction<Wait>(obj, "Intro 1").time = FsmFloat.op_Implicit(0.01f);
				FsmUtil.GetAction<Wait>(obj, "Intro Roar").time = FsmFloat.op_Implicit(1f);
			}
		}

		private static IEnumerator AbsRadSkip(Scene arg1)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			if (!(((Scene)(ref arg1)).name != "GG_Radiance"))
			{
				yield return null;
				PlayMakerFSM obj = FSMUtility.LocateMyFSM(GameObject.Find("Boss Control"), "Control");
				Object.Destroy((Object)(object)GameObject.Find("Sun"));
				Object.Destroy((Object)(object)GameObject.Find("feather_particles"));
				FsmState state = FsmUtil.GetState(obj, "Setup");
				FsmUtil.GetAction<Wait>(state).time = FsmFloat.op_Implicit(1.5f);
				FsmUtil.RemoveAction<SetPlayerDataBool>(state);
				FsmUtil.RemoveAction(state, 4);
				FsmUtil.ChangeTransition(state, "FINISHED", "Eye Flash");
				FsmUtil.GetAction<Wait>(obj, "Title Up").time = FsmFloat.op_Implicit(1f);
			}
		}

		private static IEnumerator DreamerFsm(Scene arg1)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			if (DREAMERS.Contains<string>(((Scene)(ref arg1)).name))
			{
				yield return null;
				GameObject val = GameObject.Find("Dream Enter");
				if (!((Object)(object)val == (Object)null))
				{
					FsmUtil.ChangeTransition(FsmUtil.GetState(FSMUtility.LocateMyFSM(val, "Control"), "Idle"), "DREAM HIT", "Change Scene");
				}
			}
		}

		private static IEnumerator AbyssShriekPickup(Scene arg1)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			if (!(((Scene)(ref arg1)).name != "Abyss_12"))
			{
				yield return null;
				PlayMakerFSM obj = FSMUtility.LocateMyFSM(GameObject.Find("Scream 2 Get"), "Scream Get");
				FsmUtil.ChangeTransition(FsmUtil.GetState(obj, "Move To"), "FINISHED", "Get");
				FsmUtil.RemoveAllOfType<Wait>(FsmUtil.GetState(obj, "Get Pause"));
				FsmUtil.RemoveAllOfType<Wait>(FsmUtil.GetState(obj, "Land"));
			}
		}

		private static IEnumerator KingsBrand(Scene arg1)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			yield return KingsBrandHornet(arg1);
			yield return KingsBrandAvalanche(arg1);
		}

		private static IEnumerator KingsBrandHornet(Scene arg1)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			if (!(((Scene)(ref arg1)).name != "Deepnest_East_12"))
			{
				yield return null;
				PlayMakerFSM obj = FSMUtility.LocateMyFSM(GameObject.Find("Hornet Blizzard Return Scene"), "Control");
				FsmUtil.RemoveAllOfType<Wait>(FsmUtil.GetState(obj, "Fade Pause"));
				FsmUtil.RemoveAllOfType<Wait>(FsmUtil.GetState(obj, "Fade In"));
				FsmUtil.RemoveAllOfType<Wait>(FsmUtil.GetState(obj, "Land"));
			}
		}

		private static IEnumerator KingsBrandAvalanche(Scene arg1)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			if (!(((Scene)(ref arg1)).name != "Room_Wyrm"))
			{
				yield return null;
				FsmUtil.GetAction<Wait>(FsmUtil.GetState(FSMUtility.LocateMyFSM(GameObject.Find("Avalanche End"), "Control"), "Fade")).time = FsmFloat.op_Implicit(1f);
			}
		}

		private static IEnumerator BlackEgg(Scene arg1)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			if (!(((Scene)(ref arg1)).name != "Room_temple"))
			{
				yield return null;
				PlayMakerFSM obj = FSMUtility.LocateMyFSM(GameObject.Find("Final Boss Door"), "Control");
				FsmUtil.RemoveAllOfType<Wait>(FsmUtil.GetState(obj, "Take Control"));
				FsmUtil.GetAction<Wait>(FsmUtil.GetState(obj, "Shake")).time = FsmFloat.op_Implicit(1f);
				FsmUtil.RemoveAllOfType<Wait>(FsmUtil.GetState(obj, "Barrier Flash"));
				FsmUtil.RemoveAllOfType<Wait>(FsmUtil.GetState(obj, "Blow"));
				FsmUtil.RemoveAllOfType<Wait>(FsmUtil.GetState(obj, "Door Off"));
				FsmUtil.RemoveAllOfType<Wait>(FsmUtil.GetState(obj, "Roar"));
				FsmUtil.GetAction<Wait>(FsmUtil.GetState(obj, "Roar End")).time = FsmFloat.op_Implicit(1f);
			}
		}

		private static IEnumerator OnBeginSceneTransition(orig_BeginSceneTransitionRoutine orig, GameManager self, SceneLoadInfo info)
		{
			if (InstantSceneFadeIns)
			{
				info.EntryDelay = 0f;
			}
			if (!DreamersGet || info.SceneName.Length <= 15 || info.SceneName.Substring(0, 15) != "Dream_Guardian_")
			{
				yield return orig.Invoke(self, info);
				yield break;
			}
			string text = info.SceneName.Substring(15);
			PlayerData instance = PlayerData.instance;
			instance.SetBool(text.ToLower() + "Defeated", true);
			instance.SetBool("maskBroken" + text, true);
			instance.guardiansDefeated++;
			instance.crossroadsInfected = true;
			if (instance.guardiansDefeated == 3)
			{
				instance.mrMushroomState = 1;
				instance.brettaState++;
				string[] aLL_DREAMER_BOOLS = ALL_DREAMER_BOOLS;
				foreach (string text2 in aLL_DREAMER_BOOLS)
				{
					instance.SetBool(text2, true);
				}
			}
			info.SceneName = GameManager.instance.sceneName;
			info.EntryGateName = "door_dreamReturn";
			GameCameras.instance.cameraFadeFSM.Fsm.Event("FADE INSTANT");
			yield return orig.Invoke(self, info);
			GameCameras.instance.cameraFadeFSM.Fsm.SetState("FadeIn");
			yield return null;
			HeroController.instance.MaxHealth();
			while ((int)GameManager.instance.gameState != 4)
			{
				yield return null;
			}
			yield return null;
			PlayMakerFSM.BroadcastEvent("BOX DOWN");
			PlayMakerFSM.BroadcastEvent("BOX DOWN DREAM");
			string sceneName = GameManager.instance.sceneName;
			string text3 = ((sceneName == "Deepnest_Spider_Town") ? "Death Respawn Marker Hegemol" : "Death Respawn Marker");
			HeroController.instance.SetBenchRespawn(text3, sceneName, 0, true);
			if (sceneName == "Ruins2_Watcher_Room")
			{
				GameManager.instance.SetCurrentMapZoneAsRespawn();
			}
			if (sceneName == "Deepnest_Spider_Town")
			{
				PlayerData.instance.SetBool("spiderCapture", true);
			}
			GameManager.instance.SaveGame();
			HeroController.instance.AcceptInput();
		}

		private static void OnSetSkip(orig_SetSkipMode orig, InputHandler self, SkipPromptMode newmode)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Invalid comparison between Unknown and I4
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			bool flag = AllowSkippingNonskippable;
			if (flag)
			{
				bool flag2 = (int)newmode <= 1;
				flag = !flag2;
			}
			if (flag)
			{
				newmode = (SkipPromptMode)(SkipCutscenesWithoutPrompt ? 1 : 0);
			}
			else if (SkipCutscenesWithoutPrompt && (int)newmode == 0)
			{
				newmode = (SkipPromptMode)1;
			}
			orig.Invoke(self, newmode);
		}

		private static void AnimatorBegin(orig_Begin orig, AnimatorSequence self)
		{
			if (AutoSkipCinematics)
			{
				((SkippableSequence)self).Skip();
			}
			else
			{
				orig.Invoke(self);
			}
		}

		private static void FadeBegin(orig_Begin orig, FadeSequence self)
		{
			if (AutoSkipCinematics)
			{
				((SkippableSequence)self).Skip();
			}
			else
			{
				orig.Invoke(self);
			}
		}

		private static void CinematicBegin(orig_Begin orig, CinematicSequence self)
		{
			if (AutoSkipCinematics)
			{
				((SkippableSequence)self).Skip();
			}
			else
			{
				orig.Invoke(self);
			}
		}
	}
	[UsedImplicitly]
	public class SpeedBroke : FauxMod
	{
		private class PrePhysicsRelinquishControl : FsmStateAction
		{
			public override void OnEnter()
			{
				if (!MenuDrop)
				{
					((FsmStateAction)this).OnFixedUpdate();
				}
			}

			public override void OnFixedUpdate()
			{
				HeroController.instance.RelinquishControl();
				((FsmStateAction)this).Finish();
			}
		}

		[CompilerGenerated]
		private static class <>O
		{
			public static hook_CanOpenInventory <0>__CanOpenInventory;

			public static hook_CanQuickMap <1>__CanQuickMap;

			public static hook_Start <2>__AllowPause;

			public static hook_ShouldHardLand <3>__CanHardLand;

			public static hook_OnEnable <4>__ModifyFSM;

			public static hook_Update <5>__EnableSuperslides;

			public static hook_SetStartingMotionState_bool <6>__SetStartingMotionState_bool;

			public static hook_OnCollisionExit2D <7>__OnCollisionExit2D;

			public static hook_CancelWallsliding <8>__CancelWallsliding;

			public static Manipulator <9>__FinishedDashing;

			public static Func<GameObject, GameObject> <10>__OnObjectPoolSpawn;

			public static UnityAction<Scene, Scene> <11>__SceneChanged;

			public static Manipulator <12>__RestoreElevatorStorage;
		}

		[SerializeToSetting]
		public static bool MenuDrop = true;

		[SerializeToSetting]
		public static bool Storage = true;

		[SerializeToSetting]
		public static bool Superslides = true;

		[SerializeToSetting]
		public static bool Televator = true;

		[SerializeToSetting]
		public static bool WallClingStorage;

		[SerializeToSetting]
		public static bool ExplosionPogo = true;

		[SerializeToSetting]
		public static bool GrubsThroughWalls = true;

		[SerializeToSetting]
		public static bool LeverSkips = true;

		[SerializeToSetting]
		public static bool NoHardFalls;

		[SerializeToSetting]
		public static bool ShadeSoulLeverSkip;

		[SerializeToSetting]
		public static bool CrystalisedMoundSpikes = true;

		private static readonly MethodInfo _DieFromHazardIteratorMethod = Extensions.GetStateMachineTarget(typeof(HeroController).GetMethod("DieFromHazard", BindingFlags.Instance | BindingFlags.NonPublic));

		private ILHook? _elevatorStorage;

		public override void Initialize()
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
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Expected O, but got Unknown
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Expected O, but got Unknown
			object obj = <>O.<0>__CanOpenInventory;
			if (obj == null)
			{
				hook_CanOpenInventory val = CanOpenInventory;
				<>O.<0>__CanOpenInventory = val;
				obj = (object)val;
			}
			HeroController.CanOpenInventory += (hook_CanOpenInventory)obj;
			object obj2 = <>O.<1>__CanQuickMap;
			if (obj2 == null)
			{
				hook_CanQuickMap val2 = CanQuickMap;
				<>O.<1>__CanQuickMap = val2;
				obj2 = (object)val2;
			}
			HeroController.CanQuickMap += (hook_CanQuickMap)obj2;
			object obj3 = <>O.<2>__AllowPause;
			if (obj3 == null)
			{
				hook_Start val3 = AllowPause;
				<>O.<2>__AllowPause = val3;
				obj3 = (object)val3;
			}
			TutorialEntryPauser.Start += (hook_Start)obj3;
			object obj4 = <>O.<3>__CanHardLand;
			if (obj4 == null)
			{
				hook_ShouldHardLand val4 = CanHardLand;
				<>O.<3>__CanHardLand = val4;
				obj4 = (object)val4;
			}
			HeroController.ShouldHardLand += (hook_ShouldHardLand)obj4;
			object obj5 = <>O.<4>__ModifyFSM;
			if (obj5 == null)
			{
				hook_OnEnable val5 = ModifyFSM;
				<>O.<4>__ModifyFSM = val5;
				obj5 = (object)val5;
			}
			PlayMakerFSM.OnEnable += (hook_OnEnable)obj5;
			object obj6 = <>O.<5>__EnableSuperslides;
			if (obj6 == null)
			{
				hook_Update val6 = EnableSuperslides;
				<>O.<5>__EnableSuperslides = val6;
				obj6 = (object)val6;
			}
			InputHandler.Update += (hook_Update)obj6;
			object obj7 = <>O.<6>__SetStartingMotionState_bool;
			if (obj7 == null)
			{
				hook_SetStartingMotionState_bool val7 = SetStartingMotionState_bool;
				<>O.<6>__SetStartingMotionState_bool = val7;
				obj7 = (object)val7;
			}
			HeroController.SetStartingMotionState_bool += (hook_SetStartingMotionState_bool)obj7;
			object obj8 = <>O.<7>__OnCollisionExit2D;
			if (obj8 == null)
			{
				hook_OnCollisionExit2D val8 = OnCollisionExit2D;
				<>O.<7>__OnCollisionExit2D = val8;
				obj8 = (object)val8;
			}
			HeroController.OnCollisionExit2D += (hook_OnCollisionExit2D)obj8;
			object obj9 = <>O.<8>__CancelWallsliding;
			if (obj9 == null)
			{
				hook_CancelWallsliding val9 = CancelWallsliding;
				<>O.<8>__CancelWallsliding = val9;
				obj9 = (object)val9;
			}
			HeroController.CancelWallsliding += (hook_CancelWallsliding)obj9;
			object obj10 = <>O.<9>__FinishedDashing;
			if (obj10 == null)
			{
				Manipulator val10 = FinishedDashing;
				<>O.<9>__FinishedDashing = val10;
				obj10 = (object)val10;
			}
			HeroController.FinishedDashing += (Manipulator)obj10;
			ModHooks.ObjectPoolSpawnHook += OnObjectPoolSpawn;
			SceneManager.activeSceneChanged += SceneChanged;
			MethodInfo dieFromHazardIteratorMethod = _DieFromHazardIteratorMethod;
			object obj11 = <>O.<12>__RestoreElevatorStorage;
			if (obj11 == null)
			{
				Manipulator val11 = RestoreElevatorStorage;
				<>O.<12>__RestoreElevatorStorage = val11;
				obj11 = (object)val11;
			}
			_elevatorStorage = new ILHook((MethodBase)dieFromHazardIteratorMethod, (Manipulator)obj11);
		}

		public override void Unload()
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
			object obj = <>O.<0>__CanOpenInventory;
			if (obj == null)
			{
				hook_CanOpenInventory val = CanOpenInventory;
				<>O.<0>__CanOpenInventory = val;
				obj = (object)val;
			}
			HeroController.CanOpenInventory -= (hook_CanOpenInventory)obj;
			object obj2 = <>O.<1>__CanQuickMap;
			if (obj2 == null)
			{
				hook_CanQuickMap val2 = CanQuickMap;
				<>O.<1>__CanQuickMap = val2;
				obj2 = (object)val2;
			}
			HeroController.CanQuickMap -= (hook_CanQuickMap)obj2;
			object obj3 = <>O.<2>__AllowPause;
			if (obj3 == null)
			{
				hook_Start val3 = AllowPause;
				<>O.<2>__AllowPause = val3;
				obj3 = (object)val3;
			}
			TutorialEntryPauser.Start -= (hook_Start)obj3;
			object obj4 = <>O.<3>__CanHardLand;
			if (obj4 == null)
			{
				hook_ShouldHardLand val4 = CanHardLand;
				<>O.<3>__CanHardLand = val4;
				obj4 = (object)val4;
			}
			HeroController.ShouldHardLand -= (hook_ShouldHardLand)obj4;
			object obj5 = <>O.<4>__ModifyFSM;
			if (obj5 == null)
			{
				hook_OnEnable val5 = ModifyFSM;
				<>O.<4>__ModifyFSM = val5;
				obj5 = (object)val5;
			}
			PlayMakerFSM.OnEnable -= (hook_OnEnable)obj5;
			object obj6 = <>O.<5>__EnableSuperslides;
			if (obj6 == null)
			{
				hook_Update val6 = EnableSuperslides;
				<>O.<5>__EnableSuperslides = val6;
				obj6 = (object)val6;
			}
			InputHandler.Update -= (hook_Update)obj6;
			object obj7 = <>O.<6>__SetStartingMotionState_bool;
			if (obj7 == null)
			{
				hook_SetStartingMotionState_bool val7 = SetStartingMotionState_bool;
				<>O.<6>__SetStartingMotionState_bool = val7;
				obj7 = (object)val7;
			}
			HeroController.SetStartingMotionState_bool -= (hook_SetStartingMotionState_bool)obj7;
			object obj8 = <>O.<7>__OnCollisionExit2D;
			if (obj8 == null)
			{
				hook_OnCollisionExit2D val8 = OnCollisionExit2D;
				<>O.<7>__OnCollisionExit2D = val8;
				obj8 = (object)val8;
			}
			HeroController.OnCollisionExit2D -= (hook_OnCollisionExit2D)obj8;
			object obj9 = <>O.<8>__CancelWallsliding;
			if (obj9 == null)
			{
				hook_CancelWallsliding val9 = CancelWallsliding;
				<>O.<8>__CancelWallsliding = val9;
				obj9 = (object)val9;
			}
			HeroController.CancelWallsliding -= (hook_CancelWallsliding)obj9;
			object obj10 = <>O.<9>__FinishedDashing;
			if (obj10 == null)
			{
				Manipulator val10 = FinishedDashing;
				<>O.<9>__FinishedDashing = val10;
				obj10 = (object)val10;
			}
			HeroController.FinishedDashing -= (Manipulator)obj10;
			ModHooks.ObjectPoolSpawnHook -= OnObjectPoolSpawn;
			SceneManager.activeSceneChanged -= SceneChanged;
			ILHook? elevatorStorage = _elevatorStorage;
			if (elevatorStorage != null)
			{
				elevatorStorage.Dispose();
			}
		}

		private static void RestoreElevatorStorage(ILContext il)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			ILCursor val = new ILCursor(il);
			val.GotoNext(new Func<Instruction, bool>[3]
			{
				delegate(Instruction i)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					return ILPatternMatchingExt.Match(i, OpCodes.Ldloc_1);
				},
				(Instruction i) => ILPatternMatchingExt.MatchLdnull(i),
				(Instruction i) => ILPatternMatchingExt.MatchCallvirt<HeroController>(i, "SetHeroParent")
			});
			val.GotoNext(Array.Empty<Func<Instruction, bool>>());
			val.RemoveRange(2);
			val.EmitDelegate<Action<HeroController>>((Action<HeroController>)delegate(HeroController hc)
			{
				if (!Storage)
				{
					hc.SetHeroParent((Transform)null);
				}
			});
		}

		private static void AllowPause(orig_Start orig, TutorialEntryPauser self)
		{
			HeroController.instance.isEnteringFirstLevel = false;
		}

		private static bool CanQuickMap(orig_CanQuickMap orig, HeroController self)
		{
			HeroControllerStates cState = self.cState;
			if (!Storage)
			{
				return orig.Invoke(self);
			}
			if (!GameManager.instance.isPaused && !cState.onConveyor && !cState.dashing && !cState.backDashing && (!cState.attacking || Mirror.GetField<HeroController, float>(self, "attack_time") >= self.ATTACK_RECOVERY_TIME) && !cState.recoiling && !cState.hazardDeath)
			{
				return !cState.hazardRespawning;
			}
			return false;
		}

		private static bool CanOpenInventory(orig_CanOpenInventory orig, HeroController self)
		{
			HeroControllerStates cState = self.cState;
			if (!MenuDrop)
			{
				return orig.Invoke(self);
			}
			if (GameManager.instance.isPaused || self.controlReqlinquished || cState.recoiling || cState.transitioning || cState.hazardDeath || cState.hazardRespawning || self.playerData.disablePause || !self.CanInput())
			{
				return self.playerData.atBench;
			}
			return true;
		}

		private static void EnableSuperslides(orig_Update orig, InputHandler self)
		{
			if (Superslides && GameManager.instance.TimeSlowed)
			{
				Mirror.SetField<HeroController, int>(HeroController.instance, "recoilSteps", 0);
				ref int fieldRef = ref Mirror.GetFieldRef<GameManager, int>(GameManager.instance, "timeSlowedCount");
				int num = fieldRef;
				fieldRef = 0;
				orig.Invoke(self);
				fieldRef = num;
			}
			else
			{
				orig.Invoke(self);
			}
		}

		private static bool CanHardLand(orig_ShouldHardLand orig, HeroController self, Collision2D collision)
		{
			if (!NoHardFalls)
			{
				return orig.Invoke(self, collision);
			}
			return false;
		}

		private static void ModifyFSM(orig_OnEnable orig, PlayMakerFSM self)
		{
			switch (self.FsmName)
			{
			case "Control":
				if (((Object)self).name == "Initial Fall Impact" && NoHardFalls)
				{
					FsmUtil.ChangeTransition(self, "Idle", "LAND", "Return Control");
				}
				break;
			case "Call Lever":
			{
				FsmState val = default(FsmState);
				if (((Object)self).name.StartsWith("Lift Call Lever") && Televator && FsmUtil.TryGetState(self, "Check Already Called", ref val))
				{
					FsmUtil.ChangeTransition(self, "Left", "FINISHED", "Send Msg");
					FsmUtil.ChangeTransition(self, "Right", "FINISHED", "Send Msg");
				}
				break;
			}
			case "Bottle Control":
			{
				FsmState state = FsmUtil.GetState(self, "Shatter");
				if (state != null && GrubsThroughWalls)
				{
					FsmUtil.RemoveAllOfType<BoolTest>(state);
				}
				break;
			}
			case "Switch Control":
				if (((Object)self).name.Contains("Ruins Lever") && LeverSkips)
				{
					FsmUtil.RemoveAllOfType<BoolTest>(FsmUtil.GetState(self, "Range"));
					FsmUtil.RemoveAllOfType<BoolTest>(FsmUtil.GetState(self, "Check If Nail"));
				}
				break;
			case "Dream Nail":
				if (((Object)self).name == "Knight" && Storage)
				{
					FsmUtil.GetAction<ListenForDreamNail>(FsmUtil.GetState(self, "Cancelable")).activeBool = FsmBool.op_Implicit(true);
					FsmUtil.GetAction<ListenForDreamNail>(FsmUtil.GetState(self, "Cancelable Dash")).activeBool = FsmBool.op_Implicit(true);
					FsmUtil.GetAction<ListenForDreamNail>(FsmUtil.GetState(self, "Queuing")).activeBool = FsmBool.op_Implicit(true);
					FsmUtil.RemoveAllOfType<BoolTest>(FsmUtil.GetState(self, "Queuing"));
				}
				break;
			case "Inventory Control":
				if (((Object)self).name == "Inventory" && !(FsmUtil.GetAction<FsmStateAction>(self, "Open", 5) is PrePhysicsRelinquishControl))
				{
					self.Fsm.HandleFixedUpdate = true;
					self.AddEventHandlerComponents();
					FsmUtil.RemoveAction<CallMethodProper>(FsmUtil.GetState(self, "Open"));
					FsmUtil.InsertAction(FsmUtil.GetState(self, "Open"), 5, (FsmStateAction)(object)new PrePhysicsRelinquishControl());
				}
				break;
			}
			orig.Invoke(self);
		}

		private static GameObject OnObjectPoolSpawn(GameObject go)
		{
			if (!ExplosionPogo)
			{
				return go;
			}
			if (!((Object)go).name.StartsWith("Gas Explosion Recycle M"))
			{
				return go;
			}
			go.layer = 11;
			NonBouncer component = go.GetComponent<NonBouncer>();
			if (Object.op_Implicit((Object)(object)component))
			{
				component.active = false;
			}
			return go;
		}

		private static void SceneChanged(Scene from, Scene to)
		{
			string name = ((Scene)(ref to)).name;
			if (!(name == "Ruins1_31"))
			{
				if (name == "Mines_35" && CrystalisedMoundSpikes)
				{
					((MonoBehaviour)HeroController.instance).StartCoroutine(AddCrystalisedMoundSpikes());
				}
			}
			else if (ShadeSoulLeverSkip)
			{
				((MonoBehaviour)HeroController.instance).StartCoroutine(EnableShadeSoulLeverSkip());
			}
		}

		private static IEnumerator EnableShadeSoulLeverSkip()
		{
			yield return null;
			GameObject val = GameObject.Find("Chunk 1 1");
			GameObject val2 = new GameObject("Ruins Lever", new Type[2]
			{
				typeof(Lever),
				typeof(BoxCollider2D)
			});
			val2.transform.position = new Vector3(38f, 56.7f);
			val2.layer = 8;
			val2.GetComponent<Lever>().OnHit = delegate
			{
				FSMUtility.LocateMyFSM(GameObject.Find("Ruins Gate"), "Toll Gate").SendEvent("OPEN");
			};
			BoxCollider2D component = val2.GetComponent<BoxCollider2D>();
			component.size = new Vector2(0.4f, 0.6f);
			((Collider2D)component).isTrigger = true;
			Vector2[] collection = (Vector2[])(object)new Vector2[3]
			{
				new Vector2(21.4f, 19f),
				new Vector2(21.5f, 16f),
				new Vector2(21.5f, 19f)
			};
			EdgeCollider2D obj = val.GetComponents<EdgeCollider2D>().First(delegate(EdgeCollider2D x)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				return x.points[0] == new Vector2(0f, 12f);
			});
			List<Vector2> list = obj.points.ToList();
			list.RemoveRange(6, 2);
			list.InsertRange(6, collection);
			obj.points = list.ToArray();
		}

		private static IEnumerator AddCrystalisedMoundSpikes()
		{
			yield return null;
			NonBouncer[] array = Object.FindObjectsOfType<NonBouncer>();
			foreach (NonBouncer val in array)
			{
				if (((Object)((Component)val).gameObject).name.StartsWith("Spike Collider"))
				{
					val.active = false;
					AudioSource val2 = new GameObject("tinkAudio").AddComponent<AudioSource>();
					val2.clip = GameObject.Find("Mines Platform").GetComponent<FlipPlatform>().hitSound;
					val2.volume = FixVolume.Volume;
					((Component)val).gameObject.AddComponent<TinkEffect>().blockEffect = ((Component)val2).gameObject;
				}
			}
		}

		private static void SetStartingMotionState_bool(orig_SetStartingMotionState_bool orig, HeroController self, bool preventRunDip)
		{
			if (WallClingStorage)
			{
				bool touchingWall = self.cState.touchingWall;
				orig.Invoke(self, preventRunDip);
				self.cState.touchingWall = touchingWall;
			}
			else
			{
				orig.Invoke(self, preventRunDip);
			}
		}

		private static void OnCollisionExit2D(orig_OnCollisionExit2D orig, HeroController self, Collision2D collision)
		{
			if (WallClingStorage && !self.cState.recoilingLeft && !self.cState.recoilingRight)
			{
				bool touchingWall = self.cState.touchingWall;
				bool touchingWallL = self.touchingWallL;
				bool touchingWallR = self.touchingWallR;
				orig.Invoke(self, collision);
				self.cState.touchingWall = touchingWall;
				self.touchingWallL = touchingWallL;
				self.touchingWallR = touchingWallR;
			}
			else
			{
				orig.Invoke(self, collision);
			}
		}

		private static void CancelWallsliding(orig_CancelWallsliding orig, HeroController self)
		{
			if (WallClingStorage)
			{
				bool touchingWallL = self.touchingWallL;
				bool touchingWallR = self.touchingWallR;
				orig.Invoke(self);
				self.touchingWallL = touchingWallL;
				self.touchingWallR = touchingWallR;
			}
			else
			{
				orig.Invoke(self);
			}
		}

		private static void FinishedDashing(ILContext il)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			ILCursor val = new ILCursor(il);
			val.GotoNext(new Func<Instruction, bool>[8]
			{
				(Instruction i) => ILPatternMatchingExt.MatchLdarg(i, 0),
				(Instruction i) => ILPatternMatchingExt.MatchLdfld<HeroController>(i, "touchingWallL"),
				(Instruction i) =>
				{
					ILLabel val2 = default(ILLabel);
					return ILPatternMatchingExt.MatchBrtrue(i, ref val2);
				},
				(Instruction i) => ILPatternMatchingExt.MatchLdarg(i, 0),
				(Instruction i) => ILPatternMatchingExt.MatchLdfld<HeroController>(i, "touchingWallR"),
				(Instruction i) =>
				{
					ILLabel val2 = default(ILLabel);
					return ILPatternMatchingExt.MatchBr(i, ref val2);
				},
				(Instruction i) => ILPatternMatchingExt.MatchLdcI4(i, 1),
				(Instruction i) => ILPatternMatchingExt.MatchAnd(i)
			});
			val.GotoNext(Array.Empty<Func<Instruction, bool>>());
			val.RemoveRange(7);
			val.EmitDelegate<Func<bool, HeroController, bool>>((Func<bool, HeroController, bool>)((bool flag, HeroController hc) => flag && (WallClingStorage || hc.touchingWallL || hc.touchingWallR)));
			val.GotoNext(new Func<Instruction, bool>[5]
			{
				(Instruction i) => ILPatternMatchingExt.MatchLdarg(i, 0),
				(Instruction i) => ILPatternMatchingExt.MatchLdfld<HeroController>(i, "dashingDown"),
				(Instruction i) =>
				{
					ILLabel val2 = default(ILLabel);
					return ILPatternMatchingExt.MatchBrfalse(i, ref val2);
				},
				(Instruction i) => ILPatternMatchingExt.MatchLdarg(i, 0),
				(Instruction i) => ILPatternMatchingExt.MatchCallvirt<HeroController>(i, "FlipSprite")
			});
			val.GotoNext(Array.Empty<Func<Instruction, bool>>());
			val.RemoveRange(4);
			val.EmitDelegate<Action<HeroController>>((Action<HeroController>)delegate(HeroController hc)
			{
				if (!WallClingStorage && hc.dashingDown)
				{
					hc.FlipSprite();
				}
			});
		}
	}
	[UsedImplicitly]
	public class UnencryptedSaves : FauxMod
	{
		private static readonly FastReflectionDelegate GetSaveFileName = FastReflectionHelper.GetFastDelegate(typeof(ModHooks).GetMethod("GetSaveFileName", BindingFlags.Static | BindingFlags.NonPublic), true);

		public override void Initialize()
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected O, but got Unknown
			ModHooks.SavegameLoadHook += OnSaveLoad;
			ModHooks.BeforeSavegameSaveHook += OnSaveSave;
			DesktopPlatform.WriteSaveSlot += new Manipulator(RemoveStupidSave);
		}

		private void RemoveStupidSave(ILContext il)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			ILCursor val = new ILCursor(il).Goto(0, (MoveType)0, false);
			ILCursor[] array = default(ILCursor[]);
			while (val.TryFindNext(ref array, new Func<Instruction, bool>[7]
			{
				(Instruction x) => ILPatternMatchingExt.MatchLdstr(x, "_1.4.3.2.dat"),
				(Instruction x) => ILPatternMatchingExt.MatchCall(x, typeof(string), "Concat"),
				(Instruction x) => ILPatternMatchingExt.MatchStloc(x, 5),
				(Instruction x) => ILPatternMatchingExt.MatchLdloc(x, 5),
				(Instruction x) => ILPatternMatchingExt.MatchLdarg(x, 2),
				(Instruction x) => ILPatternMatchingExt.MatchCall(x, typeof(File), "WriteAllBytes"),
				(Instruction x) =>
				{
					ILLabel val2 = default(ILLabel);
					return ILPatternMatchingExt.MatchLeaveS(x, ref val2);
				}
			}))
			{
				for (int num = array.Length - 1; num >= 0; num--)
				{
					array[num].Remove();
				}
			}
		}

		private static void OnSaveSave(SaveGameData data)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			string savePath = GetSavePath(GetRealID(data.playerData.profileID), "json");
			string contents = JsonConvert.SerializeObject((object)data, (Formatting)1, new JsonSerializerSettings
			{
				ContractResolver = (IContractResolver)(object)ShouldSerializeContractResolver.Instance,
				TypeNameHandling = (TypeNameHandling)4
			});
			File.WriteAllText(savePath, contents);
			File.SetLastWriteTime(savePath, new DateTime(1999, 6, 11));
		}

		private void OnSaveLoad(int saveSlot)
		{
			saveSlot = GetRealID(saveSlot);
			GameManager gm = GameManager.instance;
			string savePath = GetSavePath(saveSlot, "json");
			if (File.Exists(savePath) && File.GetLastWriteTimeUtc(savePath).Year != 1999)
			{
				LoadJson(savePath, DoLoad);
			}
			void DoLoad(string text)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Expected O, but got Unknown
				try
				{
					SaveGameData val = JsonConvert.DeserializeObject<SaveGameData>(text, new JsonSerializerSettings
					{
						ContractResolver = (IContractResolver)(object)ShouldSerializeContractResolver.Instance,
						TypeNameHandling = (TypeNameHandling)4
					});
					if (val != null)
					{
						gm.playerData = (PlayerData.instance = val.playerData);
						gm.sceneData = (SceneData.instance = val.sceneData);
						gm.profileID = saveSlot;
						gm.inputHandler.RefreshPlayerData();
					}
				}
				catch (ArgumentException arg)
				{
					Log($"Caught ArgumentException when deserializing SaveGameData! {arg}");
				}
			}
		}

		private void LoadJson(string jsonPath, Action<string> callback)
		{
			string res;
			try
			{
				res = Encoding.UTF8.GetString(File.ReadAllBytes(jsonPath));
			}
			catch (Exception ex)
			{
				Log("Failed to read json!: " + ex.Message);
				return;
			}
			CoreLoop.InvokeNext((Action)delegate
			{
				callback(res);
			});
		}

		private static string GetSavePath(int saveSlot, string ending)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Expected O, but got Unknown
			return Path.Combine(Mirror.GetField<DesktopPlatform, string>((DesktopPlatform)Platform.Current, "saveDirPath"), $"user{saveSlot}.{ending}");
		}

		private static int GetRealID(int id)
		{
			string text = (string)GetSaveFileName.Invoke((object)null, new object[1] { id });
			if (text != null)
			{
				return int.Parse(new string(text.SkipWhile((char c) => !char.IsDigit(c)).TakeWhile(char.IsDigit).ToArray()));
			}
			return id;
		}
	}
}
namespace QoL.Components
{
	public class Lever : MonoBehaviour
	{
		public PersistentBoolData BoolData { get; set; }

		public Action? OnHit { get; set; }

		public Lever()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			((MonoBehaviour)this)..ctor();
			PersistentBoolData val = new PersistentBoolData();
			Scene activeScene = SceneManager.GetActiveScene();
			val.sceneName = ((Scene)(ref activeScene)).name;
			val.id = ((Object)this).name;
			BoolData = val;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (((Component)other).gameObject.layer != 17 || !((Object)((Component)other).gameObject).name.Contains("Slash"))
			{
				return;
			}
			SceneData sceneData = GameManager.instance.sceneData;
			PersistentBoolData val = sceneData.FindMyState(BoolData) ?? BoolData;
			if (!val.activated)
			{
				if (OnHit == null)
				{
					throw new NullReferenceException("OnHit");
				}
				val.activated = true;
				OnHit();
				sceneData.SaveMyState(val);
			}
		}
	}
}
