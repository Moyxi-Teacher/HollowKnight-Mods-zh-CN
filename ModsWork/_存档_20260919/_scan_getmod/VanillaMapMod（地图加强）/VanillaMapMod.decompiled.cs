using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using ConnectionMetadataInjector.Util;
using MagicUI.Core;
using MagicUI.Elements;
using MapChanger;
using MapChanger.Defs;
using MapChanger.MonoBehaviours;
using MapChanger.UI;
using Microsoft.CodeAnalysis;
using Modding;
using Newtonsoft.Json;
using UnityEngine;
using VanillaMapMod.Pins;
using VanillaMapMod.Settings;
using VanillaMapMod.UI;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: AssemblyCompany("syyePhenomenol")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyCopyright("Copyright © syyePhenomenol 2022")]
[assembly: AssemblyDescription("A Hollow Knight mod that adds extra pins for all item locations on the map.")]
[assembly: AssemblyFileVersion("2.1.6.0")]
[assembly: AssemblyInformationalVersion("2.1.6.0汉化")]
[assembly: AssemblyProduct("VanillaMapMod")]
[assembly: AssemblyTitle("VanillaMapMod")]
[assembly: AssemblyVersion("2.1.6.0")]
[module: RefSafetyRules(11)]
[CompilerGenerated]
internal sealed class <>z__ReadOnlyArray<T> : IEnumerable, ICollection, IList, IEnumerable<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection<T>, IList<T>
{
	[CompilerGenerated]
	private readonly T[] _items;

	int ICollection.Count => _items.Length;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => this;

	object IList.this[int index]
	{
		get
		{
			return _items[index];
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	bool IList.IsFixedSize => true;

	bool IList.IsReadOnly => true;

	int IReadOnlyCollection<T>.Count => _items.Length;

	T IReadOnlyList<T>.this[int index] => _items[index];

	int ICollection<T>.Count => _items.Length;

	bool ICollection<T>.IsReadOnly => true;

	T IList<T>.this[int index]
	{
		get
		{
			return _items[index];
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public <>z__ReadOnlyArray(T[] items)
	{
		_items = items;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)_items).GetEnumerator();
	}

	void ICollection.CopyTo(Array array, int index)
	{
		((ICollection)_items).CopyTo(array, index);
	}

	int IList.Add(object value)
	{
		throw new NotSupportedException();
	}

	void IList.Clear()
	{
		throw new NotSupportedException();
	}

	bool IList.Contains(object value)
	{
		return ((IList)_items).Contains(value);
	}

	int IList.IndexOf(object value)
	{
		return ((IList)_items).IndexOf(value);
	}

	void IList.Insert(int index, object value)
	{
		throw new NotSupportedException();
	}

	void IList.Remove(object value)
	{
		throw new NotSupportedException();
	}

	void IList.RemoveAt(int index)
	{
		throw new NotSupportedException();
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return ((IEnumerable<T>)_items).GetEnumerator();
	}

	void ICollection<T>.Add(T item)
	{
		throw new NotSupportedException();
	}

	void ICollection<T>.Clear()
	{
		throw new NotSupportedException();
	}

	bool ICollection<T>.Contains(T item)
	{
		return ((ICollection<T>)_items).Contains(item);
	}

	void ICollection<T>.CopyTo(T[] array, int arrayIndex)
	{
		((ICollection<T>)_items).CopyTo(array, arrayIndex);
	}

	bool ICollection<T>.Remove(T item)
	{
		throw new NotSupportedException();
	}

	int IList<T>.IndexOf(T item)
	{
		return ((IList<T>)_items).IndexOf(item);
	}

	void IList<T>.Insert(int index, T item)
	{
		throw new NotSupportedException();
	}

	void IList<T>.RemoveAt(int index)
	{
		throw new NotSupportedException();
	}
}
[CompilerGenerated]
internal sealed class <>z__ReadOnlySingleElementList<T> : IEnumerable, ICollection, IList, IEnumerable<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection<T>, IList<T>
{
	private sealed class Enumerator : IDisposable, IEnumerator, IEnumerator<T>
	{
		[CompilerGenerated]
		private readonly T _item;

		[CompilerGenerated]
		private bool _moveNextCalled;

		object IEnumerator.Current => _item;

		T IEnumerator<T>.Current => _item;

		public Enumerator(T item)
		{
			_item = item;
		}

		bool IEnumerator.MoveNext()
		{
			if (!_moveNextCalled)
			{
				return _moveNextCalled = true;
			}
			return false;
		}

		void IEnumerator.Reset()
		{
			_moveNextCalled = false;
		}

		void IDisposable.Dispose()
		{
		}
	}

	[CompilerGenerated]
	private readonly T _item;

	int ICollection.Count => 1;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => this;

	object IList.this[int index]
	{
		get
		{
			if (index != 0)
			{
				throw new IndexOutOfRangeException();
			}
			return _item;
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	bool IList.IsFixedSize => true;

	bool IList.IsReadOnly => true;

	int IReadOnlyCollection<T>.Count => 1;

	T IReadOnlyList<T>.this[int index]
	{
		get
		{
			if (index != 0)
			{
				throw new IndexOutOfRangeException();
			}
			return _item;
		}
	}

	int ICollection<T>.Count => 1;

	bool ICollection<T>.IsReadOnly => true;

	T IList<T>.this[int index]
	{
		get
		{
			if (index != 0)
			{
				throw new IndexOutOfRangeException();
			}
			return _item;
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public <>z__ReadOnlySingleElementList(T item)
	{
		_item = item;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new Enumerator(_item);
	}

	void ICollection.CopyTo(Array array, int index)
	{
		array.SetValue(_item, index);
	}

	int IList.Add(object value)
	{
		throw new NotSupportedException();
	}

	void IList.Clear()
	{
		throw new NotSupportedException();
	}

	bool IList.Contains(object value)
	{
		return EqualityComparer<T>.Default.Equals(_item, (T)value);
	}

	int IList.IndexOf(object value)
	{
		if (!EqualityComparer<T>.Default.Equals(_item, (T)value))
		{
			return -1;
		}
		return 0;
	}

	void IList.Insert(int index, object value)
	{
		throw new NotSupportedException();
	}

	void IList.Remove(object value)
	{
		throw new NotSupportedException();
	}

	void IList.RemoveAt(int index)
	{
		throw new NotSupportedException();
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return new Enumerator(_item);
	}

	void ICollection<T>.Add(T item)
	{
		throw new NotSupportedException();
	}

	void ICollection<T>.Clear()
	{
		throw new NotSupportedException();
	}

	bool ICollection<T>.Contains(T item)
	{
		return EqualityComparer<T>.Default.Equals(_item, item);
	}

	void ICollection<T>.CopyTo(T[] array, int arrayIndex)
	{
		array[arrayIndex] = _item;
	}

	bool ICollection<T>.Remove(T item)
	{
		throw new NotSupportedException();
	}

	int IList<T>.IndexOf(T item)
	{
		if (!EqualityComparer<T>.Default.Equals(_item, item))
		{
			return -1;
		}
		return 0;
	}

	void IList<T>.Insert(int index, T item)
	{
		throw new NotSupportedException();
	}

	void IList<T>.RemoveAt(int index)
	{
		throw new NotSupportedException();
	}
}
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
namespace VanillaMapMod
{
	internal static class Localization
	{
		private static Dictionary<string, string> _localization;

		internal static void Load()
		{
			try
			{
				_localization = JsonUtil.DeserializeFromExternalFile<Dictionary<string, string>>(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "language.json"));
				Localization.AddLocalizer((Func<string, string>)Localize);
			}
			catch (Exception)
			{
				((Loggable)VanillaMapMod.Instance).LogDebug("No valid \"language.json\" file found");
			}
		}

		internal static string Localize(string t)
		{
			if (_localization != null && _localization.TryGetValue(t, out var value))
			{
				return value;
			}
			return null;
		}
	}
	internal class FullMapMode : MapMode
	{
		public override string Mod => "VanillaMapMod";

		public override string ModeName => "Full Map";

		public override bool ForceHasMap => true;

		public override bool ForceHasQuill => true;

		public override bool? VanillaPins => VanillaMapMod.LS.VanillaPinsOn;

		public override bool ImmediateMapUpdate => true;

		public override bool FullMap => true;

		public override bool? NextAreaNameActiveOverride(NextAreaName nextAreaName)
		{
			return true;
		}

		public override bool? NextAreaArrowActiveOverride(NextAreaArrow nextAreaArrow)
		{
			return true;
		}
	}
	internal class NormalMode : MapMode
	{
		public override float Priority => 10f;

		public override string Mod => "VanillaMapMod";

		public override string ModeName => "Normal";

		public override bool? VanillaPins
		{
			get
			{
				if (!VanillaMapMod.LS.VanillaPinsOn)
				{
					return false;
				}
				return null;
			}
		}

		public override bool ImmediateMapUpdate => VanillaMapMod.GS.FastMapUpdate;

		public override bool InitializeToThis()
		{
			return true;
		}
	}
	internal class VmmPinGroup : MapObject
	{
		[CompilerGenerated]
		private PoolGroup <PoolGroup>k__BackingField;

		internal PoolGroup PoolGroup
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return <PoolGroup>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				<PoolGroup>k__BackingField = value;
			}
		}

		internal void Initialize(PoolGroup poolGroup)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			((MapObject)this).Initialize();
			((MapObject)this).ActiveModifiers.Add(PoolSettingOn);
			PoolGroup = poolGroup;
			MapObjectUpdater.Add((MapObject)(object)this);
		}

		private bool PoolSettingOn()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return VanillaMapMod.LS.GetPoolGroupSetting(PoolGroup);
		}
	}
	internal static class VmmPinManager
	{
		private const float OFFSETZ_BASE = -1.4f;

		private const float OFFSETZ_RANGE = 0.4f;

		internal static Dictionary<string, VmmPin> Pins { get; private set; } = new Dictionary<string, VmmPin>();

		internal static Dictionary<PoolGroup, VmmPinGroup> PinGroups { get; private set; } = new Dictionary<PoolGroup, VmmPinGroup>();

		internal static void MakePins(GameObject goMap)
		{
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			((Loggable)VanillaMapMod.Instance).LogDebug("Make Pins");
			Pins = new Dictionary<string, VmmPin>();
			PinGroups = new Dictionary<PoolGroup, VmmPinGroup>();
			foreach (PoolGroup item in Enum.GetValues(typeof(PoolGroup)).Cast<PoolGroup>().Where(delegate(PoolGroup poolGroup)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0003: Invalid comparison between Unknown and I4
				return (int)poolGroup != 25;
			}))
			{
				VmmPinGroup vmmPinGroup = Utils.MakeMonoBehaviour<VmmPinGroup>(goMap, PoolExtensions.FriendlyName(item));
				vmmPinGroup.Initialize(item);
				PinGroups[item] = vmmPinGroup;
			}
			foreach (MapLocationDef value in Finder.GetAllVanillaLocations().Values)
			{
				VmmPin vmmPin = Utils.MakeMonoBehaviour<VmmPin>(goMap, value.Name);
				vmmPin.Initialize(value, PinGroups[SubcategoryFinder.GetLocationPoolGroup(value.Name)]);
				Pins[value.Name] = vmmPin;
			}
			IEnumerable<MapObject> source = (IEnumerable<MapObject>)Pins.Values.OrderBy(delegate(VmmPin mapObj)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return ((Component)mapObj).transform.position.x;
			}).ThenBy(delegate(VmmPin mapObj)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return ((Component)mapObj).transform.position.y;
			});
			for (int num = 0; num < source.Count(); num++)
			{
				Transform transform = ((Component)source.ElementAt(num)).transform;
				transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -1.4f + (float)num / (float)Pins.Count() * 0.4f);
			}
		}

		internal static string GetPoolGroupCounter(PoolGroup poolGroup)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			IReadOnlyCollection<MapObject> children = ((MapObject)PinGroups[poolGroup]).Children;
			string text = ((!IsPersistent(poolGroup)) ? (children.Where((MapObject pin) => Tracker.HasClearedLocation(((Object)pin).name)).Count() + " / ") : "");
			return text + children.Count();
		}

		private static bool IsPersistent(PoolGroup poolGroup)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Invalid comparison between Unknown and I4
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Invalid comparison between Unknown and I4
			if ((int)poolGroup == 17 || poolGroup - 22 <= 1)
			{
				return true;
			}
			return false;
		}
	}
	public sealed class VanillaMapMod : Mod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>, IMenuMod, IMod, ILogger, ITogglableMod
	{
		private static readonly string[] _dependencies = new string[2] { "地图变更", "CMICore" };

		private static readonly MapMode[] _modes = (MapMode[])(object)new MapMode[2]
		{
			new NormalMode(),
			new FullMapMode()
		};

		public static VanillaMapMod Instance { get; private set; }

		internal static LocalSettings LS { get; private set; } = new LocalSettings();

		internal static GlobalSettings GS { get; private set; } = new GlobalSettings();

		internal static VmmPauseMenu PauseMenu { get; private set; }

		public bool ToggleButtonInsideMenu => true;

		public VanillaMapMod()
			: base("地图加强")
		{
			Instance = this;
		}

		public override string GetVersion()
		{
			return "2.1.6";
		}

		public override int LoadPriority()
		{
			return 10;
		}

		public void OnLoadLocal(LocalSettings ls)
		{
			LS = ls;
		}

		public LocalSettings OnSaveLocal()
		{
			return LS;
		}

		public void OnLoadGlobal(GlobalSettings gs)
		{
			GS = gs;
		}

		public GlobalSettings OnSaveGlobal()
		{
			return GS;
		}

		public List<MenuEntry> GetMenuData(MenuEntry? toggleButtonEntry)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			return new List<MenuEntry>(1)
			{
				new MenuEntry(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values, "启用地图加强模组，添加地图标记功能。", toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader)
			};
		}

		public bool IsToggleable()
		{
			return true;
		}

		public void Toggle()
		{
			ModeManager.ToggleModEnabled();
		}

		public void Unload()
		{
			VmmPauseMenu pauseMenu = PauseMenu;
			if (pauseMenu != null)
			{
				((UILayout)pauseMenu).Destroy();
			}
			PauseMenu = null;
			Instance = null;
		}

		public override void Initialize()
		{
			((Loggable)this).LogDebug("Initializing");
			string[] dependencies = _dependencies;
			foreach (string text in dependencies)
			{
				if (!(ModHooks.GetMod(text, false, false) is Mod))
				{
					((Loggable)Instance).LogWarn("Dependency not found for " + ((object)this).GetType().Name + ": " + text);
					return;
				}
			}
			Localization.Load();
			Events.OnEnterGame += OnEnterGame;
			Events.OnQuitToMenu += OnQuitToMenu;
			((Loggable)this).LogDebug("初始化完成。");
		}

		private static void OnEnterGame()
		{
			ModeManager.AddModes((IEnumerable<MapMode>)_modes);
			Events.OnSetGameMap += OnSetGameMap;
		}

		private static void OnQuitToMenu()
		{
			Events.OnSetGameMap -= OnSetGameMap;
			VmmPauseMenu pauseMenu = PauseMenu;
			if (pauseMenu != null)
			{
				((UILayout)pauseMenu).Destroy();
			}
			PauseMenu = null;
		}

		private static void OnSetGameMap(GameObject goMap)
		{
			try
			{
				VmmPinManager.MakePins(goMap);
				PauseMenu = new VmmPauseMenu();
			}
			catch (Exception ex)
			{
				((Loggable)Instance).LogError((object)ex);
			}
		}
	}
}
namespace VanillaMapMod.UI
{
	internal class FastMapUpdateButton : MainButton
	{
		public override void Update()
		{
			((MainButton)this).Update();
			if (ModeManager.CurrentMode() is FullMapMode)
			{
				((ArrangableElement)((PauseMenuButton)this).Button).Visibility = (Visibility)2;
			}
			else
			{
				((ArrangableElement)((PauseMenuButton)this).Button).Visibility = (Visibility)0;
			}
		}

		protected override void OnClick()
		{
			VanillaMapMod.GS.ToggleFastMapUpdate();
		}

		protected override TextFormat GetTextFormat()
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			string text = Localization.L("Fast Map\nUpdate") + ": ";
			if (!VanillaMapMod.GS.FastMapUpdate)
			{
				return new TextFormat(text + Localization.L("Off"), Color.op_Implicit(Colors.GetColor((ColorSetting)2)));
			}
			return new TextFormat(text + Localization.L("On"), Color.op_Implicit(Colors.GetColor((ColorSetting)1)));
		}
	}
	internal class ModeButton : MainButton
	{
		protected override void OnClick()
		{
			ModeManager.ToggleMode();
		}

		protected override TextFormat GetTextFormat()
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			string text = Localization.L("Mode") + "\n";
			MapMode val = ModeManager.CurrentMode();
			if (!(val is NormalMode normalMode))
			{
				if (val is FullMapMode fullMapMode)
				{
					return new TextFormat(text + Localization.L(((MapMode)fullMapMode).ModeName), Color.op_Implicit(Colors.GetColor((ColorSetting)1)));
				}
				return default(TextFormat);
			}
			return new TextFormat(text + Localization.L(((MapMode)normalMode).ModeName), Color.op_Implicit(Colors.GetColor((ColorSetting)2)));
		}
	}
	internal class ModEnabledButton : MainButton
	{
		protected override void OnClick()
		{
			ModeManager.ToggleModEnabled();
		}

		protected override TextFormat GetTextFormat()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			return new TextFormat(Localization.L("Mod\nEnabled"), Color.op_Implicit(Colors.GetColor((ColorSetting)1)));
		}
	}
	internal class ModPinsButton : MainButton
	{
		protected override void OnClick()
		{
			VanillaMapMod.LS.ToggleAllPools();
		}

		protected override TextFormat GetTextFormat()
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			string text = Localization.L("Mod Pins:\n");
			if (VanillaMapMod.LS.PoolSettings.Values.All((bool value) => value))
			{
				return new TextFormat(text + Localization.L("On"), Color.op_Implicit(Colors.GetColor((ColorSetting)1)));
			}
			if (VanillaMapMod.LS.PoolSettings.Values.All((bool value) => !value))
			{
				return new TextFormat(text + Localization.L("Off"), Color.op_Implicit(Colors.GetColor((ColorSetting)2)));
			}
			return new TextFormat(text + Localization.L("Custom"), Color.op_Implicit(Colors.GetColor((ColorSetting)3)));
		}
	}
	internal class PinShapeButton : MainButton
	{
		protected override void OnClick()
		{
			VanillaMapMod.GS.TogglePinShape();
		}

		protected override TextFormat GetTextFormat()
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			return new TextFormat(Localization.L("Pin Shape") + ":\n" + Localization.L(Utils.ToWhitespaced(VanillaMapMod.GS.PinShape.ToString())), Color.op_Implicit(Colors.GetColor((ColorSetting)2)));
		}
	}
	internal class PinSizeButton : MainButton
	{
		protected override void OnClick()
		{
			VanillaMapMod.GS.TogglePinSize();
		}

		protected override TextFormat GetTextFormat()
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			return new TextFormat(Localization.L("Pin Size") + "\n" + Localization.L(Utils.ToWhitespaced(VanillaMapMod.GS.PinSize.ToString())), Color.op_Implicit(Colors.GetColor((ColorSetting)2)));
		}
	}
	internal class PoolButton : ExtraButton
	{
		[CompilerGenerated]
		private readonly PoolGroup <PoolGroup>k__BackingField;

		public override string Name
		{
			get
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				return $"{((UILayout)((UIElementWrapper<PauseMenuLayout, Button>)this).Layout).Mod} : {PoolGroup}";
			}
		}

		internal PoolGroup PoolGroup
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return <PoolGroup>k__BackingField;
			}
		}

		public PoolButton(PoolGroup poolGroup)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			PoolGroup = poolGroup;
			((ExtraButton)this)..ctor();
		}

		protected override void OnClick()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			VanillaMapMod.LS.TogglePoolGroupSetting(PoolGroup);
		}

		protected override TextFormat GetTextFormat()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			return new TextFormat(Localization.L(PoolExtensions.FriendlyName(PoolGroup)).Replace(" ", "\n") + "\n" + VmmPinManager.GetPoolGroupCounter(PoolGroup), Color.op_Implicit(Colors.GetColor((ColorSetting)(VanillaMapMod.LS.GetPoolGroupSetting(PoolGroup) ? 1 : 2))));
		}
	}
	internal class PoolsGrid : ExtraButtonGrid
	{
		internal static PoolsGrid Instance { get; private set; }

		public override int RowSize => 8;

		protected override DynamicUniformGrid Build(PauseMenuLayout layout)
		{
			Instance = this;
			return ((ExtraButtonGrid)this).Build(layout);
		}

		protected override IEnumerable<ExtraButton> GetButtons()
		{
			return (IEnumerable<ExtraButton>)Enum.GetValues(typeof(PoolGroup)).Cast<PoolGroup>().Except(new <>z__ReadOnlySingleElementList<PoolGroup>((PoolGroup)25))
				.Select(delegate(PoolGroup p)
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					return new PoolButton(p);
				});
		}
	}
	internal class PoolsGridControlButton : GridControlButton<PoolsGrid>
	{
		protected override TextFormat GetTextFormat()
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			string text = Localization.L("Customize\nPins");
			PoolsGrid instance = PoolsGrid.Instance;
			bool? obj;
			if (instance == null)
			{
				obj = null;
			}
			else
			{
				DynamicUniformGrid grid = ((ExtraButtonGrid)instance).Grid;
				obj = ((grid != null) ? new bool?(((ArrangableElement)grid).IsEffectivelyVisible) : ((bool?)null));
			}
			bool? flag = obj;
			return new TextFormat(text, Color.op_Implicit((flag == true) ? Colors.GetColor((ColorSetting)3) : Colors.GetColor((ColorSetting)2)));
		}

		protected override Color GetBorderColor()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return Color.op_Implicit(Colors.GetColor((ColorSetting)2));
		}
	}
	internal class UIScaleButton : MainButton
	{
		protected override void OnClick()
		{
			MapChangerMod.GS.ToggleUIScale();
		}

		protected override TextFormat GetTextFormat()
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			return new TextFormat(string.Format("{0}\n{1:n1} ×", Localization.L("UI Scale"), MapChangerMod.GS.UIScale), Color.op_Implicit(Colors.GetColor((ColorSetting)2)));
		}
	}
	internal class VanillaPinsButton : MainButton
	{
		protected override void OnClick()
		{
			VanillaMapMod.LS.ToggleVanillaPins();
		}

		protected override TextFormat GetTextFormat()
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			string text = Localization.L("Vanilla Pins") + ":\n";
			if (!VanillaMapMod.LS.VanillaPinsOn)
			{
				return new TextFormat(text + Localization.L("Off"), Color.op_Implicit(Colors.GetColor((ColorSetting)2)));
			}
			return new TextFormat(text + Localization.L("On"), Color.op_Implicit(Colors.GetColor((ColorSetting)1)));
		}
	}
	internal class VmmPauseMenu : PauseMenuLayout
	{
		private sealed class VmmPauseMenuTitle : PauseMenuTitle
		{
			protected override TextFormat GetDefaultTextFormat()
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				return new TextFormat("地图加强", Color.op_Implicit(Colors.GetColor((ColorSetting)2)));
			}
		}

		internal VmmPauseMenu()
			: base("VanillaMapMod", "地图加强")
		{
		}

		protected override bool ActiveCondition()
		{
			if (((PauseMenuLayout)this).ActiveCondition() && MapChangerMod.IsEnabled())
			{
				return ModeManager.CurrentMode().Mod == "VanillaMapMod";
			}
			return false;
		}

		protected override PauseMenuTitle GetTitle()
		{
			return (PauseMenuTitle)(object)new VmmPauseMenuTitle();
		}

		protected override IEnumerable<MainButton> GetMainButtons()
		{
			return new <>z__ReadOnlyArray<MainButton>((MainButton[])(object)new MainButton[9]
			{
				new ModEnabledButton(),
				new ModPinsButton(),
				new VanillaPinsButton(),
				(MainButton)new PoolsGridControlButton(),
				new ModeButton(),
				new PinShapeButton(),
				new PinSizeButton(),
				new FastMapUpdateButton(),
				new UIScaleButton()
			});
		}

		protected override IEnumerable<ExtraButtonGrid> GetExtraButtonGrids()
		{
			return new <>z__ReadOnlySingleElementList<ExtraButtonGrid>((ExtraButtonGrid)(object)new PoolsGrid());
		}
	}
}
namespace VanillaMapMod.Settings
{
	public enum PinSize
	{
		Tiny,
		Small,
		Medium,
		Large,
		Huge
	}
	public enum PinShape
	{
		Circle,
		Diamond,
		Square,
		Pentagon,
		Hexagon,
		NoBorder
	}
	public class GlobalSettings
	{
		[JsonProperty]
		public PinSize PinSize { get; private set; } = PinSize.Medium;

		[JsonProperty]
		public PinShape PinShape { get; private set; }

		[JsonProperty]
		public bool FastMapUpdate { get; private set; }

		internal void TogglePinSize()
		{
			PinSize = (PinSize)((int)(PinSize + 1) % Enum.GetNames(typeof(PinSize)).Length);
		}

		internal void TogglePinShape()
		{
			PinShape = (PinShape)((int)(PinShape + 1) % Enum.GetNames(typeof(PinShape)).Length);
		}

		internal void ToggleFastMapUpdate()
		{
			FastMapUpdate = !FastMapUpdate;
		}
	}
	public class LocalSettings
	{
		[JsonProperty]
		public Dictionary<PoolGroup, bool> PoolSettings { get; private set; } = Enum.GetValues(typeof(PoolGroup)).Cast<PoolGroup>().Where(delegate(PoolGroup poolGroup)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Invalid comparison between Unknown and I4
			return (int)poolGroup != 25;
		})
			.ToDictionary(delegate(PoolGroup t)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				return t;
			}, (PoolGroup t) => true);

		[JsonProperty]
		public bool VanillaPinsOn { get; private set; }

		internal bool GetPoolGroupSetting(PoolGroup poolGroup)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if (PoolSettings.ContainsKey(poolGroup))
			{
				return PoolSettings[poolGroup];
			}
			return false;
		}

		internal void SetPoolGroupSetting(PoolGroup poolGroup, bool value)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if (PoolSettings.ContainsKey(poolGroup))
			{
				PoolSettings[poolGroup] = value;
			}
		}

		internal void TogglePoolGroupSetting(PoolGroup poolGroup)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			if (PoolSettings.ContainsKey(poolGroup))
			{
				PoolSettings[poolGroup] = !PoolSettings[poolGroup];
			}
		}

		internal void ToggleAllPools()
		{
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			bool value = false;
			if (PoolSettings.Values.Any((bool flag) => !flag))
			{
				value = true;
			}
			foreach (PoolGroup item in Enum.GetValues(typeof(PoolGroup)).Cast<PoolGroup>().Where(delegate(PoolGroup poolGroup)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0003: Invalid comparison between Unknown and I4
				return (int)poolGroup != 25;
			}))
			{
				PoolSettings[item] = value;
			}
		}

		internal void ToggleVanillaPins()
		{
			VanillaPinsOn = !VanillaPinsOn;
		}
	}
}
namespace VanillaMapMod.Pins
{
	internal class VmmPin : BorderedBackgroundPin
	{
		private const float TINY_SCALE = 0.47f;

		private const float SMALL_SCALE = 0.56f;

		private const float MEDIUM_SCALE = 0.67f;

		private const float LARGE_SCALE = 0.8f;

		private const float HUGE_SCALE = 0.96f;

		private protected const float NO_BORDER_MULTIPLIER = 1.3f;

		private static readonly Dictionary<PinSize, float> _pinSizes = new Dictionary<PinSize, float>
		{
			{
				PinSize.Tiny,
				0.47f
			},
			{
				PinSize.Small,
				0.56f
			},
			{
				PinSize.Medium,
				0.67f
			},
			{
				PinSize.Large,
				0.8f
			},
			{
				PinSize.Huge,
				0.96f
			}
		};

		internal MapRoomPosition Mlp { get; private set; }

		internal void Initialize(MapLocationDef mld, VmmPinGroup parent)
		{
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Expected O, but got Unknown
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			((BorderedBackgroundPin)this).Initialize();
			((MapObject)this).ActiveModifiers.AddRange(new <>z__ReadOnlyArray<Func<bool>>(new Func<bool>[3] { CorrectMapOpen, ActiveByCurrentMode, LocationNotCleared }));
			((MapObject)parent).AddChild((MapObject)(object)this);
			Mlp = new MapRoomPosition((IEnumerable<MapLocation>)mld.MapLocations);
			((Pin)this).MapPosition = (IMapPosition)(object)Mlp;
			((Pin)this).Sprite = GetSpriteFromPoolGroup(parent.PoolGroup);
		}

		public override void OnMainUpdate(bool active)
		{
			if (active)
			{
				UpdatePinSize();
				UpdatePinShape();
			}
		}

		internal void UpdatePinSize()
		{
			((Pin)this).Size = _pinSizes[VanillaMapMod.GS.PinSize];
			if (VanillaMapMod.GS.PinShape == PinShape.NoBorder)
			{
				((Pin)this).Size = ((Pin)this).Size * 1.3f;
			}
		}

		internal void UpdatePinShape()
		{
			if (VanillaMapMod.GS.PinShape == PinShape.NoBorder)
			{
				((BorderedBackgroundPin)this).BackgroundSprite = null;
				((BorderedPin)this).BorderSprite = null;
			}
			else
			{
				((BorderedBackgroundPin)this).BackgroundSprite = SpriteManager.Instance.GetSprite($"Pins.Background{VanillaMapMod.GS.PinShape}");
				((BorderedPin)this).BorderSprite = SpriteManager.Instance.GetSprite($"Pins.Border{VanillaMapMod.GS.PinShape}");
			}
		}

		private bool CorrectMapOpen()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if (!States.WorldMapOpen)
			{
				if (States.QuickMapOpen)
				{
					return States.CurrentMapZone == Mlp.MapZone;
				}
				return false;
			}
			return true;
		}

		private bool ActiveByCurrentMode()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			if (!(ModeManager.CurrentMode() is NormalMode) || !Utils.HasMapSetting(Mlp.MapZone) || !PlayerData.instance.GetVariable<List<string>>("scenesMapped").Contains(Mlp.MappedScene))
			{
				return ModeManager.CurrentMode() is FullMapMode;
			}
			return true;
		}

		private bool LocationNotCleared()
		{
			return !Tracker.HasClearedLocation(((Object)this).name);
		}

		private static Sprite GetSpriteFromPoolGroup(PoolGroup poolGroup)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Expected I4, but got Unknown
			return SpriteManager.Instance.GetSprite("Pins." + (int)poolGroup switch
			{
				0 => "Dreamer", 
				1 => "Skill", 
				2 => "Charm", 
				3 => "Key", 
				4 => "Mask", 
				5 => "Vessel", 
				6 => "Notch", 
				7 => "Ore", 
				8 => "Geo", 
				9 => "Egg", 
				10 => "Relic", 
				11 => "Root", 
				12 => "EssenceBoss", 
				13 => "Grub", 
				14 => "Grub", 
				15 => "Map", 
				16 => "Stag", 
				17 => "Cocoon", 
				18 => "Flame", 
				19 => "Journal", 
				20 => "Rock", 
				21 => "Geo", 
				22 => "Totem", 
				23 => "Lore", 
				24 => "Shop", 
				_ => "Unknown", 
			});
		}
	}
}
