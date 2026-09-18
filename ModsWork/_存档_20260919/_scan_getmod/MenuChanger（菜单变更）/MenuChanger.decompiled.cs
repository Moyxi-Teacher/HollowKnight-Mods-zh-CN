using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using GlobalEnums;
using Language;
using MenuChanger.Attributes;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using MenuChanger.NavigationTypes;
using Microsoft.CodeAnalysis;
using Modding;
using Modding.Patches;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using On;
using On.UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: ComVisible(false)]
[assembly: Guid("d4361348-a69e-4cdd-988e-7ac0b7eda035")]
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: AssemblyCompany("MenuChanger")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyFileVersion("1.1.0.0")]
[assembly: AssemblyInformationalVersion("1.1.0.0汉化")]
[assembly: AssemblyProduct("MenuChanger")]
[assembly: AssemblyTitle("MenuChanger")]
[assembly: AssemblyVersion("1.1.0.0")]
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
namespace MenuChanger
{
	public static class Colors
	{
		public static readonly Color DEFAULT_COLOR;

		public static readonly Color INVALID_INPUT_COLOR;

		public static readonly Color TRUE_COLOR;

		public static readonly Color FALSE_COLOR;

		public static readonly Color LOCKED_TRUE_COLOR;

		public static readonly Color LOCKED_FALSE_COLOR;

		static Colors()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			DEFAULT_COLOR = Color.white;
			INVALID_INPUT_COLOR = Color.Lerp(Color.white, Color.red, 0.5f);
			TRUE_COLOR = Color.Lerp(Color.white, Color.yellow, 0.5f);
			FALSE_COLOR = Color.grey;
			LOCKED_TRUE_COLOR = Color.Lerp(Color.grey, Color.yellow, 0.5f);
			LOCKED_FALSE_COLOR = Color.Lerp(Color.grey, Color.black, 0.5f);
		}
	}
	public class CustomInputField : InputField
	{
		private static readonly FieldInfo fi = typeof(InputField).GetField("m_ProcessingEvent", BindingFlags.Instance | BindingFlags.NonPublic);

		private readonly Event m_ProcessingEvent;

		private static readonly HashSet<KeyCode> keyCodes = new HashSet<KeyCode>
		{
			(KeyCode)273,
			(KeyCode)274,
			(KeyCode)276,
			(KeyCode)275
		};

		public CustomInputField()
		{
			object value = fi.GetValue(this);
			m_ProcessingEvent = (Event)((value is Event) ? value : null);
		}

		public bool AllSelected()
		{
			if (((InputField)this).caretPositionInternal == ((InputField)this).text.Length)
			{
				return ((InputField)this).caretSelectPositionInternal == 0;
			}
			return false;
		}

		public override void OnUpdateSelected(BaseEventData eventData)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Invalid comparison between Unknown and I4
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Invalid comparison between Unknown and I4
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Invalid comparison between Unknown and I4
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			if (!((InputField)this).isFocused)
			{
				return;
			}
			bool flag = AllSelected();
			bool flag2 = false;
			while (Event.PopEvent(m_ProcessingEvent))
			{
				if ((int)m_ProcessingEvent.rawType == 4)
				{
					flag2 = true;
					if (flag && keyCodes.Contains(m_ProcessingEvent.keyCode))
					{
						flag2 = false;
						((InputField)this).DeactivateInputField();
						break;
					}
					if ((int)((InputField)this).KeyPressed(m_ProcessingEvent) == 1)
					{
						((InputField)this).DeactivateInputField();
						break;
					}
				}
				if (m_ProcessingEvent.type - 13 <= 1)
				{
					string commandName = m_ProcessingEvent.commandName;
					if (commandName != null && commandName == "SelectAll")
					{
						((InputField)this).SelectAll();
						flag2 = true;
					}
				}
			}
			if (flag2)
			{
				((InputField)this).UpdateLabel();
			}
			if (!flag)
			{
				((AbstractEventData)eventData).Use();
			}
		}
	}
	internal static class DebugMethods
	{
		internal static Type[] badTypes = new Type[3]
		{
			typeof(Component),
			typeof(Transform),
			typeof(GameObject)
		};

		internal static Vector2 MenuButtonRect(MenuButton button)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			float num2 = 0f;
			RectTransform[] componentsInChildren = ((Component)button).gameObject.GetComponentsInChildren<RectTransform>();
			foreach (RectTransform val in componentsInChildren)
			{
				MenuChangerMod instance = MenuChangerMod.instance;
				string name = ((Object)((Component)val).gameObject).name;
				Rect rect = val.rect;
				object arg = ((Rect)(ref rect)).width * ((Transform)val).localScale.x;
				rect = val.rect;
				((Loggable)instance).Log(name + $" {arg}, {((Rect)(ref rect)).height * ((Transform)val).localScale.y}");
				rect = val.rect;
				num = Mathf.Max(((Rect)(ref rect)).width * ((Transform)val).localScale.x, num);
				rect = val.rect;
				num2 = Mathf.Max(((Rect)(ref rect)).height * ((Transform)val).localScale.y, num2);
			}
			return new Vector2(num, num2);
		}

		public static string BuildFullName(Transform t)
		{
			return (((Object)(object)t.parent == (Object)null) ? string.Empty : BuildFullName(t.parent)) + "." + ((Object)((Component)t).gameObject).name;
		}

		public static void DumpComponents(GameObject obj, bool verbose = true, int indentLevel = 0)
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Expected O, but got Unknown
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			((Loggable)MenuChangerMod.instance).Log(BuildFullName(obj.transform));
			Component[] components = ((Component)obj.transform).GetComponents<Component>();
			foreach (Component val in components)
			{
				((Loggable)MenuChangerMod.instance).Log($"{new string(' ', indentLevel)}{((object)val).GetType().Name}: {((Object)val.transform).name} at {val.transform.position} (local: {val.transform.localPosition})");
				if (!verbose)
				{
					continue;
				}
				foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(val))
				{
					string name = property.Name;
					object value = property.GetValue(val);
					((Loggable)MenuChangerMod.instance).Log($"{new string(' ', indentLevel + 4)}{property.PropertyType.Name} {name}={value}");
				}
			}
			foreach (Transform item in obj.transform)
			{
				Transform val2 = item;
				((Loggable)MenuChangerMod.instance).Log(new string(' ', indentLevel) + "----");
				((Loggable)MenuChangerMod.instance).Log($"{new string(' ', indentLevel)}{((object)((Component)val2).gameObject).GetType().Name}: {((Object)((Component)val2).gameObject).name}: {BuildFullName(((Component)val2).transform)} at {((Component)val2).transform.position} (local: {((Component)val2).transform.localPosition})");
				DumpComponents(((Component)val2).gameObject, verbose, indentLevel + 4);
			}
		}

		public static void DumpProperties(object obj)
		{
			foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj))
			{
				string name = property.Name;
				object value = property.GetValue(obj);
				((Loggable)MenuChangerMod.instance).Log($"{property.PropertyType.Name} {name}={value}");
			}
		}

		internal static void CopyRectTransform(RectTransform copyTo, RectTransform copyFrom)
		{
			PropertyInfo[] properties = typeof(RectTransform).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.CanWrite)
				{
					propertyInfo.SetValue(copyTo, propertyInfo.GetValue(copyFrom, null), null);
				}
			}
		}

		internal static void CopyText(Text copyTo, Text copyFrom)
		{
			PropertyInfo[] properties = typeof(RectTransform).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.CanWrite)
				{
					propertyInfo.SetValue(copyTo, propertyInfo.GetValue(copyFrom, null), null);
				}
			}
		}

		internal static void CopyComponent(Type T, Component from, Component to, bool verbose = true)
		{
			if (verbose)
			{
				((Loggable)MenuChangerMod.instance).Log("Copying component of type " + T.Name);
			}
			foreach (PropertyInfo p in from t in T.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				where t.CanWrite
				select t)
			{
				if (badTypes.Any((Type t) => p.PropertyType.IsAssignableFrom(t)))
				{
					if (verbose)
					{
						((Loggable)MenuChangerMod.instance).Log("    Skipping property " + p.PropertyType.Name + " " + p.Name + " of " + T.Name);
					}
				}
				else
				{
					p.SetValue(to, p.GetValue(from, null), null);
				}
			}
		}

		public static void CopyComponents(GameObject from, GameObject to)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Expected O, but got Unknown
			((Loggable)MenuChangerMod.instance).Log("Copying " + ((Object)from).name);
			Component[] components = ((Component)from.transform).GetComponents<Component>();
			foreach (Component val in components)
			{
				CopyComponent(((object)val).GetType(), val, to.AddComponent(((object)val).GetType()));
			}
			foreach (Transform item in from.transform)
			{
				((Loggable)MenuChangerMod.instance).Log((object)from.transform.childCount);
				GameObject val3 = new GameObject(((Object)((Component)item).gameObject).name);
				val3.transform.SetParent(to.transform);
				CopyComponents(((Component)item).gameObject, val3);
			}
			((Loggable)MenuChangerMod.instance).Log("Finished copying " + ((Object)from).name);
		}
	}
	public enum Neighbor
	{
		Left,
		Right,
		Up,
		Down
	}
	public interface ISelectable
	{
		void SetNeighbor(Neighbor neighbor, ISelectable selectable);

		ISelectable GetISelectable(Neighbor neighbor);

		Selectable GetSelectable(Neighbor neighbor);
	}
	public interface ISelectableGroup
	{
		void ResetNavigation();
	}
	internal static class LogHelper
	{
		public static event Action<string> OnLog;

		public static event Action<string> OnLogError;

		public static void Log(string message = "")
		{
			OnLog?.Invoke(message);
		}

		public static void Log<T>(T t) where T : struct
		{
			Log(t.ToString());
		}

		public static void Log(object message)
		{
			Log(message?.ToString() ?? "null");
		}

		public static void LogError(string message)
		{
			OnLogError?.Invoke(message);
		}
	}
	public class MenuChangerMod : Mod, ILocalSettings<Settings>, IMenuMod, IMod, ILogger, ITogglableMod
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class <>c
		{
			public static readonly <>c <>9 = new <>c();

			public static hook_UIGoToProfileMenu <>9__13_0;

			internal void <Initialize>b__13_0(orig_UIGoToProfileMenu o, UIManager s)
			{
				HideAllMenuPages();
				o.Invoke(s);
			}
		}

		internal static readonly List<MenuPage> displayedPages;

		private hook_UIGoToProfileMenu _onUIGoToProfileMenu;

		private hook_UIGoToPlayModeMenu _onUIGoToPlayModeMenu;

		public Settings Settings = new Settings();

		private static readonly string _sha1;

		private static readonly string _version;

		internal static MenuChangerMod instance { get; private set; }

		public bool ToggleButtonInsideMenu => true;

		public static event Action OnExitMainMenu;

		public MenuChangerMod()
			: base("模组菜单变更")
		{
			instance = this;
			LogHelper.OnLog += ((Loggable)this).Log;
			LogHelper.OnLogError += ((Loggable)this).LogError;
			UIManager.EditMenus += PrefabMenuObjects.Setup;
		}

		public static void HideAllMenuPages()
		{
			while (true)
			{
				MenuPage menuPage = displayedPages.FirstOrDefault();
				if (menuPage != null)
				{
					menuPage.Hide();
					continue;
				}
				break;
			}
		}

		private void OnSceneChange(Scene from, Scene to)
		{
			if (!(((Scene)(ref from)).name == "Menu_Title") || !(((Scene)(ref to)).name != "Menu_Title"))
			{
				return;
			}
			ModeMenu.OnExitMainMenu();
			PrefabMenuObjects.Dispose();
			try
			{
				OnExitMainMenu?.Invoke();
			}
			catch (Exception arg)
			{
				((Loggable)this).LogError($"Error invoking OnExitMainMenu:\n{arg}");
			}
			try
			{
				MenuPage.MenuPageComponent[] componentsInChildren = ((Component)((Component)UIManager.instance.UICanvas).transform).GetComponentsInChildren<MenuPage.MenuPageComponent>(true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					Object.Destroy((Object)(object)((Component)componentsInChildren[i]).gameObject);
				}
			}
			catch (Exception arg2)
			{
				((Loggable)this).LogError($"Error destroying MenuPages on exiting menu:{arg2}");
			}
		}

		private void EditUI()
		{
			displayedPages.Clear();
			ResumeMenu.Reset();
			ModeMenu.OnEnterMainMenu();
		}

		public override int LoadPriority()
		{
			return 100000;
		}

		public override void Initialize()
		{
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Expected O, but got Unknown
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Expected O, but got Unknown
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Expected O, but got Unknown
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			ThreadSupport.Setup();
			SceneManager.activeSceneChanged += OnSceneChange;
			UIManager.EditMenus += EditUI;
			object obj = <>c.<>9__13_0;
			if (obj == null)
			{
				hook_UIGoToProfileMenu val = delegate(orig_UIGoToProfileMenu o, UIManager s)
				{
					HideAllMenuPages();
					o.Invoke(s);
				};
				<>c.<>9__13_0 = val;
				obj = (object)val;
			}
			_onUIGoToProfileMenu = (hook_UIGoToProfileMenu)obj;
			UIManager.UIGoToProfileMenu += _onUIGoToProfileMenu;
			UIManager.ContinueGame += new hook_ContinueGame(OnContinueGame);
			UIManager.StartNewGame += new hook_StartNewGame(OnStartGame);
			_onUIGoToPlayModeMenu = (hook_UIGoToPlayModeMenu)delegate(orig_UIGoToPlayModeMenu o, UIManager s)
			{
				if (ModeMenu.Active)
				{
					((MonoBehaviour)s).StartCoroutine(GoToModeMenu(s));
				}
				else
				{
					o.Invoke(s);
				}
			};
			UIManager.UIGoToPlayModeMenu += _onUIGoToPlayModeMenu;
			ResumeMenu.Hook();
		}

		public void Unload()
		{
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Expected O, but got Unknown
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Expected O, but got Unknown
			LogHelper.OnLog -= ((Loggable)this).Log;
			LogHelper.OnLogError -= ((Loggable)this).LogError;
			UIManager.EditMenus -= PrefabMenuObjects.Setup;
			SceneManager.activeSceneChanged -= OnSceneChange;
			UIManager.EditMenus -= EditUI;
			if (_onUIGoToProfileMenu != null)
			{
				UIManager.UIGoToProfileMenu -= _onUIGoToProfileMenu;
			}
			if (_onUIGoToPlayModeMenu != null)
			{
				UIManager.UIGoToPlayModeMenu -= _onUIGoToPlayModeMenu;
			}
			UIManager.ContinueGame -= new hook_ContinueGame(OnContinueGame);
			UIManager.StartNewGame -= new hook_StartNewGame(OnStartGame);
			HideAllMenuPages();
			instance = null;
		}

		private void OnStartGame(orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
		{
			HideAllMenuPages();
			orig.Invoke(self, permaDeath, bossRush);
		}

		private void OnContinueGame(orig_ContinueGame orig, UIManager self)
		{
			HideAllMenuPages();
			orig.Invoke(self);
		}

		private IEnumerator GoToModeMenu(UIManager s)
		{
			InputHandler.Instance.StopUIInput();
			yield return s.HideSaveProfileMenu();
			ReflectionHelper.CallMethod<UIManager>(s, "SetMenuState", new object[1] { (object)(MainMenuState)16 });
			InputHandler.Instance.StartUIInput();
			ModeMenu.Show();
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
				new MenuEntry(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values, "为其他模组提供游戏内菜单构建框架。", toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader)
			};
		}

		public override string GetVersion()
		{
			return _version;
		}

		static MenuChangerMod()
		{
			displayedPages = new List<MenuPage>();
			Assembly assembly = typeof(MenuChangerMod).Assembly;
			using SHA1 sHA = SHA1.Create();
			using FileStream inputStream = File.OpenRead(assembly.Location);
			_sha1 = Convert.ToBase64String(sHA.ComputeHash(inputStream));
			int num = 5381;
			int num2 = num;
			string sha = _sha1;
			for (int i = 0; i < sha.Length && sha[i] != 0; i += 2)
			{
				num = ((num << 5) + num) ^ sha[i];
				if (i == sha.Length - 1 || sha[i + 1] == '\0')
				{
					break;
				}
				num2 = ((num2 << 5) + num2) ^ sha[i + 1];
			}
			int value = num + num2 * 1566083941;
			value = Math.Abs(value) % 997;
			Version version = assembly.GetName().Version;
			_version = $"{version.Major}.{version.Minor}.{version.Build}+{value.ToString().PadLeft(3, '0')}";
		}

		public void OnLoadLocal(Settings s)
		{
			Settings = s;
		}

		public Settings OnSaveLocal()
		{
			return Settings;
		}
	}
	public abstract class ModeMenuConstructor
	{
		public abstract void OnEnterMainMenu(MenuPage modeMenu);

		public abstract void OnExitMainMenu();

		public abstract bool TryGetModeButton(MenuPage modeMenu, out BigButton button);
	}
	public abstract class ResumeMenuConstructor
	{
		public abstract void OnEnterMainMenu();

		public abstract void OnExitMainMenu();
	}
	public class MenuPage
	{
		public class MenuPageComponent : MonoBehaviour
		{
			public MenuPage MenuPage;
		}

		public readonly GameObject self;

		public readonly RectTransform rt;

		public readonly CanvasGroup cg;

		public MenuPageNavigation nav;

		public MenuPage backTo;

		public SmallButton backButton;

		public bool isShowing;

		public bool BlockGoBack { get; set; }

		public event Action BeforeShow;

		public event Action AfterShow;

		public event Action BeforeHide;

		public event Action AfterHide;

		public event Action BeforeGoBack;

		public event Action AfterGoBack;

		public MenuPage(string name)
			: this(name, null)
		{
		}

		public MenuPage(string name, MenuPage backTo)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected O, but got Unknown
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			base..ctor();
			self = new GameObject(name);
			self.AddComponent<MenuPageComponent>().MenuPage = this;
			self.transform.position = ((Component)UIManager.instance.UICanvas).transform.position;
			self.transform.SetParent(((Component)UIManager.instance.UICanvas).transform);
			rt = self.AddComponent<RectTransform>();
			rt.anchorMin = new Vector2(0f, 0f);
			rt.anchorMax = new Vector2(1f, 1f);
			rt.sizeDelta = new Vector2(0f, 0f);
			((Transform)rt).localScale = new Vector3(1f, 1f, 1f);
			cg = self.AddComponent<CanvasGroup>();
			this.backTo = backTo;
			backButton = new SmallButton(this, "Back");
			backButton.OnClick += GoBack;
			Add(backButton.GameObject);
			backButton.MoveTo(new Vector2(0f, -450f));
			nav = new SimpleHorizontalNavigation(this);
			Hide();
		}

		public void Show()
		{
			BeforeShow?.Invoke();
			self.SetActive(true);
			cg.interactable = true;
			cg.alpha = 1f;
			isShowing = true;
			MenuChangerMod.displayedPages.Add(this);
			nav.SelectDefault();
			AfterShow?.Invoke();
		}

		public void Hide()
		{
			BeforeHide?.Invoke();
			cg.interactable = false;
			cg.alpha = 0f;
			self.SetActive(false);
			isShowing = false;
			MenuChangerMod.displayedPages.Remove(this);
			AfterHide?.Invoke();
		}

		public void GoBack()
		{
			if (!BlockGoBack)
			{
				BeforeGoBack?.Invoke();
				if (backTo == null)
				{
					UIManager.instance.UIGoToProfileMenu();
				}
				else
				{
					TransitionTo(backTo);
				}
				AfterGoBack?.Invoke();
			}
		}

		public void TransitionTo(MenuPage next)
		{
			Hide();
			next.Show();
		}

		internal void Add(GameObject obj)
		{
			obj.transform.SetParent(self.transform);
		}

		public void AddToNavigationControl(ISelectable selectable)
		{
			nav.Add(selectable);
		}

		public void ReplaceNavigation(MenuPageNavigation newNav)
		{
			foreach (ISelectable item in nav.Items)
			{
				newNav.Add(item);
			}
			nav = newNav;
			ResetNavigation();
		}

		public void ResetNavigation()
		{
			nav.ResetNavigation();
		}
	}
	public abstract class MenuPageNavigation
	{
		public MenuPage Page { get; }

		public abstract IReadOnlyCollection<ISelectable> Items { get; }

		public MenuPageNavigation(MenuPage page)
		{
			Page = page;
		}

		public abstract void Add(ISelectable selectable);

		public abstract void Remove(ISelectable selectable);

		public abstract void SelectDefault();

		public abstract void ResetNavigation();
	}
	public enum Mode
	{
		Classic,
		Steel,
		Godmaster
	}
	public static class ModeMenu
	{
		private class DefaultModeConstructor : ModeMenuConstructor
		{
			private readonly Mode mode;

			private BigButton button;

			private bool wasUnlocked;

			public bool IsUnlocked => mode switch
			{
				Mode.Classic => true, 
				Mode.Steel => GameManager.instance.GetStatusRecordInt("RecPermadeathMode") == 1, 
				Mode.Godmaster => GameManager.instance.GetStatusRecordInt("RecBossRushMode") == 1, 
				_ => true, 
			};

			public DefaultModeConstructor(Mode mode)
			{
				this.mode = mode;
			}

			public override void OnEnterMainMenu(MenuPage modeMenu)
			{
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Expected O, but got Unknown
				wasUnlocked = IsUnlocked;
				if (wasUnlocked)
				{
					button = new BigButton(modeMenu, mode);
					button.OnClick += delegate
					{
						MenuChangerMod.HideAllMenuPages();
						UIManager.instance.StartNewGame(mode == Mode.Steel, mode == Mode.Godmaster);
					};
					GameManager.instance.RefreshLanguageText += new RefreshLanguage(RefreshLanguage);
				}
			}

			public override bool TryGetModeButton(MenuPage modeMenu, out BigButton button)
			{
				if (wasUnlocked)
				{
					button = this.button;
					return true;
				}
				button = null;
				return false;
			}

			public override void OnExitMainMenu()
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				GameManager.instance.RefreshLanguageText -= new RefreshLanguage(RefreshLanguage);
				button = null;
				wasUnlocked = false;
			}

			private void RefreshLanguage()
			{
				((Component)button.GameObject.transform.Find("Text")).GetComponent<Text>().text = GetTitle();
				((Component)button.GameObject.transform.Find("DescriptionText")).GetComponent<Text>().text = GetDesc();
			}

			private string GetTitle()
			{
				if (mode == Mode.Godmaster)
				{
					return Language.Get("MODE_GODSGLORY", "CP3");
				}
				return Language.Get((mode == Mode.Classic) ? "MODE_NORMAL" : "MODE_STEEL", "MainMenu");
			}

			private string GetDesc()
			{
				if (mode == Mode.Godmaster)
				{
					return Language.Get("MODE_GODSGLORY_DESC", "CP3");
				}
				return Language.Get((mode == Mode.Classic) ? "NORMAL_MODE_TEXT" : "STEEL_MODE_TEXT", "MainMenu");
			}
		}

		private static MenuPage ModePage;

		private static MultiGridItemPanel ModeButtonPanel;

		private static List<ModeMenuConstructor> constructors = new List<ModeMenuConstructor>
		{
			new DefaultModeConstructor(Mode.Classic),
			new DefaultModeConstructor(Mode.Steel),
			new DefaultModeConstructor(Mode.Godmaster)
		};

		public static bool Active { get; private set; }

		public static void AddMode(ModeMenuConstructor constructor)
		{
			constructors.Add(constructor);
		}

		internal static void OnEnterMainMenu()
		{
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			if (constructors.Count <= 3)
			{
				((Loggable)MenuChangerMod.instance).Log("Mode menu was not requested, skipping construction...");
				return;
			}
			((Loggable)MenuChangerMod.instance).Log("Constructing mode menu...");
			Active = true;
			ModePage = new MenuPage("Mode Page");
			List<BigButton> list = new List<BigButton>();
			for (int i = 0; i < constructors.Count; i++)
			{
				try
				{
					constructors[i].OnEnterMainMenu(ModePage);
					if (constructors[i].TryGetModeButton(ModePage, out BigButton button))
					{
						list.Add(button);
					}
					((Loggable)MenuChangerMod.instance).Log("  Constructed menu from " + constructors[i].GetType().FullName);
				}
				catch (Exception arg)
				{
					((Loggable)MenuChangerMod.instance).LogError($"Error during OnEnterMainMenu for constructor of type {constructors[i].GetType().FullName}:\n{arg}");
					constructors.RemoveAt(i--);
				}
			}
			MenuPage modePage = ModePage;
			Vector2 topCenter = new Vector2(0f, 300f);
			IMenuElement[] children = list.ToArray();
			ModeButtonPanel = new MultiGridItemPanel(modePage, 5, 3, 150f, 650f, topCenter, children);
		}

		internal static void Show()
		{
			ModePage.Show();
		}

		internal static void OnExitMainMenu()
		{
			Active = false;
			ModePage = null;
			ModeButtonPanel = null;
			foreach (ModeMenuConstructor constructor in constructors)
			{
				try
				{
					constructor.OnExitMainMenu();
				}
				catch (Exception arg)
				{
					((Loggable)MenuChangerMod.instance).LogError($"Error during OnEnterMainMenu for constructor of type {constructor.GetType().Name}:\n{arg}");
				}
			}
		}
	}
	public static class PrefabMenuObjects
	{
		internal static Dictionary<string, GameObject> indexedPrefabs = new Dictionary<string, GameObject>(10);

		internal static GameObject ClassicModeButtonObject
		{
			get
			{
				return Get("ClassicModeButtonObject");
			}
			set
			{
				Set(value, "ClassicModeButtonObject");
			}
		}

		internal static GameObject SteelModeButtonObject
		{
			get
			{
				return Get("SteelModeButtonObject");
			}
			set
			{
				Set(value, "SteelModeButtonObject");
			}
		}

		internal static GameObject GGModeButtonObject
		{
			get
			{
				return Get("GGModeButtonObject");
			}
			set
			{
				Set(value, "GGModeButtonObject");
			}
		}

		internal static GameObject BackButtonObject
		{
			get
			{
				return Get("BackButtonObject");
			}
			set
			{
				Set(value, "BackButtonObject");
			}
		}

		internal static GameObject DescTextObject
		{
			get
			{
				return Get("DescTextObject");
			}
			set
			{
				Set(value, "DescTextObject");
			}
		}

		internal static GameObject Get([CallerMemberName] string name = null)
		{
			if (indexedPrefabs.TryGetValue(name, out GameObject value) && (Object)(object)value != (Object)null)
			{
				value = Object.Instantiate<GameObject>(value);
				value.SetActive(true);
				return value;
			}
			LogHelper.Log("MenuChanger prefab " + name + " did not exist at time of request!");
			return null;
		}

		internal static void Set(GameObject value, [CallerMemberName] string name = null)
		{
			Object.DontDestroyOnLoad((Object)(object)value);
			indexedPrefabs[name] = value;
			value.SetActive(false);
		}

		internal static void Setup()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			Scene activeScene = SceneManager.GetActiveScene();
			if (!(((Scene)(ref activeScene)).name != "Menu_Title"))
			{
				ClassicModeButtonObject = Object.Instantiate<GameObject>(((Component)UIManager.instance.playModeMenuScreen.defaultHighlight).gameObject);
				SteelModeButtonObject = Object.Instantiate<GameObject>(((Component)UIManager.instance.playModeMenuScreen.defaultHighlight.FindSelectableOnDown()).gameObject);
				GGModeButtonObject = Object.Instantiate<GameObject>(((Component)((Component)UIManager.instance.playModeMenuScreen.content).transform.Find("GGButton")).gameObject);
				BackButtonObject = Object.Instantiate<GameObject>(((Component)UIManager.instance.playModeMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown()).gameObject);
				DescTextObject = Object.Instantiate<GameObject>(((Component)((Component)UIManager.instance.extrasContentMenuScreen.content).gameObject.transform.Find("ScrollRect").Find("DescriptionText")).gameObject);
				Object.Destroy((Object)(object)indexedPrefabs["DescTextObject"].GetComponent<SoftMaskScript>());
			}
		}

		internal static void Dispose()
		{
			foreach (KeyValuePair<string, GameObject> indexedPrefab in indexedPrefabs)
			{
				if ((Object)(object)indexedPrefab.Value != (Object)null)
				{
					Object.Destroy((Object)(object)indexedPrefab.Value);
				}
			}
			indexedPrefabs.Clear();
		}

		public static void Normalize(MenuPage page, params GameObject[] gs)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			foreach (GameObject val in gs)
			{
				page.Add(val);
				val.transform.localPosition = Vector2.op_Implicit(Vector2.zero);
				val.transform.localScale = Vector2.op_Implicit(Vector2.one);
			}
		}

		public static (GameObject, Text, CanvasGroup) BuildDescText(MenuPage page, string text)
		{
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			GameObject descTextObject = DescTextObject;
			SoftMaskScript component = descTextObject.GetComponent<SoftMaskScript>();
			if ((Object)(object)component != (Object)null)
			{
				typeof(Graphic).GetField("m_Canvas", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(((Component)component).GetComponent<Graphic>(), UIManager.instance.UICanvas);
				Object.DestroyImmediate((Object)(object)component, true);
			}
			Text component2 = descTextObject.GetComponent<Text>();
			component2.text = text;
			((Graphic)component2).material = ((Graphic)((Component)BackButtonObject.transform.Find("Text")).GetComponent<Text>()).material;
			page.Add(descTextObject);
			descTextObject.transform.localPosition = Vector3.zero;
			descTextObject.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
			descTextObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
			CanvasGroup val = descTextObject.AddComponent<CanvasGroup>();
			val.blocksRaycasts = false;
			return (descTextObject, component2, val);
		}

		public static (MenuButton button, Text titleText, Text descText) CloneBigButton(Mode mode = Mode.Classic)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			MenuButton val = (MenuButton)(mode switch
			{
				Mode.Steel => SteelModeButtonObject.GetComponent<MenuButton>(), 
				Mode.Godmaster => GGModeButtonObject.GetComponent<MenuButton>(), 
				_ => ClassicModeButtonObject.GetComponent<MenuButton>(), 
			});
			val.buttonType = (MenuButtonType)0;
			Object.Destroy((Object)(object)((Component)val).gameObject.GetComponent<StartGameEventTrigger>());
			((MenuSelectable)val).cancelAction = (CancelAction)14;
			Navigation navigation = default(Navigation);
			((Navigation)(ref navigation)).mode = (Mode)4;
			((Selectable)val).navigation = navigation;
			Transform obj = ((Component)val).transform.Find("Text");
			Object.Destroy((Object)(object)((Component)obj).GetComponent<AutoLocalizeTextUI>());
			Text component = ((Component)obj).GetComponent<Text>();
			((Component)obj).GetComponent<RectTransform>().sizeDelta = new Vector2(784f, 63f);
			Transform obj2 = ((Component)val).transform.Find("DescriptionText");
			Object.Destroy((Object)(object)((Component)obj2).GetComponent<AutoLocalizeTextUI>());
			Text component2 = ((Component)obj2).GetComponent<Text>();
			return (button: val, titleText: component, descText: component2);
		}

		public static MenuButton BuildBigButtonOneTextNoSprite(string title)
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			var (val, val2, val3) = CloneBigButton();
			Object.Destroy((Object)(object)((Component)((Component)val).transform.Find("Image")).GetComponent<Image>());
			val2.text = title;
			((Component)((Component)val2).transform).GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -5f);
			val3.text = string.Empty;
			return val;
		}

		public static MenuButton BuildBigButtonTwoTextNoSprite(string title, string desc)
		{
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			var (val, val2, val3) = CloneBigButton();
			Object.Destroy((Object)(object)((Component)((Component)val).transform.Find("Image")).GetComponent<Image>());
			val2.text = title;
			val3.text = desc;
			((Component)val2).gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 28f);
			((Component)val3).gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -53f);
			return val;
		}

		public static MenuButton BuildBigButtonTwoTextAndSprite(Sprite sprite, string title, string desc)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			MenuButton component = ClassicModeButtonObject.GetComponent<MenuButton>();
			component.buttonType = (MenuButtonType)0;
			((MenuSelectable)component).cancelAction = (CancelAction)14;
			Object.Destroy((Object)(object)((Component)component).gameObject.GetComponent<StartGameEventTrigger>());
			Transform val = ((Component)component).transform.Find("Text");
			Object.Destroy((Object)(object)((Component)val).GetComponent<AutoLocalizeTextUI>());
			((Component)val).GetComponent<Text>().text = title ?? string.Empty;
			if (string.IsNullOrEmpty(desc))
			{
				((Component)val).GetComponent<RectTransform>().anchoredPosition = new Vector2(130.5f, -5f);
			}
			((Component)val).GetComponent<RectTransform>().sizeDelta = new Vector2(784f, 63f);
			Transform obj = ((Component)component).transform.Find("DescriptionText");
			Object.Destroy((Object)(object)((Component)obj).GetComponent<AutoLocalizeTextUI>());
			((Component)obj).GetComponent<Text>().text = desc ?? string.Empty;
			if ((Object)(object)sprite != (Object)null)
			{
				((Component)((Component)component).transform.Find("Image")).GetComponent<Image>().sprite = sprite;
			}
			return component;
		}

		public static MenuButton BuildBigButtonOneTextAndSprite(Sprite sprite, string title)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			MenuButton component = ClassicModeButtonObject.GetComponent<MenuButton>();
			component.buttonType = (MenuButtonType)0;
			Object.Destroy((Object)(object)((Component)component).gameObject.GetComponent<StartGameEventTrigger>());
			((MenuSelectable)component).cancelAction = (CancelAction)14;
			Transform obj = ((Component)component).transform.Find("Text");
			Object.Destroy((Object)(object)((Component)obj).GetComponent<AutoLocalizeTextUI>());
			((Component)obj).GetComponent<Text>().text = title ?? string.Empty;
			((Component)obj).GetComponent<RectTransform>().anchoredPosition = new Vector2(130.5f, -5f);
			((Component)obj).GetComponent<RectTransform>().sizeDelta = new Vector2(784f, 63f);
			Transform obj2 = ((Component)component).transform.Find("DescriptionText");
			Object.Destroy((Object)(object)((Component)obj2).GetComponent<AutoLocalizeTextUI>());
			((Component)obj2).GetComponent<Text>().text = string.Empty;
			if ((Object)(object)sprite != (Object)null)
			{
				((Component)((Component)component).transform.Find("Image")).GetComponent<Image>().sprite = sprite;
			}
			return component;
		}

		public static MenuButton BuildBigButtonSpriteOnly(Sprite sprite)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			MenuButton component = ClassicModeButtonObject.GetComponent<MenuButton>();
			component.buttonType = (MenuButtonType)0;
			((MenuSelectable)component).cancelAction = (CancelAction)14;
			Object.Destroy((Object)(object)((Component)component).gameObject.GetComponent<StartGameEventTrigger>());
			Object.Destroy((Object)(object)((Component)((Component)component).transform.Find("Text")).GetComponent<Text>());
			Object.Destroy((Object)(object)((Component)((Component)component).transform.Find("DescriptionText")).GetComponent<Text>());
			Image component2 = ((Component)((Component)component).transform.Find("Image")).GetComponent<Image>();
			component2.sprite = sprite;
			((Graphic)component2).rectTransform.anchoredPosition = new Vector2(0f, 0f);
			((Transform)((Graphic)component2).rectTransform).localScale = Vector2.op_Implicit(new Vector2(2.8f, 2.8f));
			return component;
		}

		public static (GameObject, Text, CanvasGroup) BuildLabel(MenuPage page, string label)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			GameObject backButtonObject = BackButtonObject;
			MenuButton component = backButtonObject.GetComponent<MenuButton>();
			((Object)component).name = label + " Label";
			component.buttonType = (MenuButtonType)1;
			Transform obj = ((Component)component).transform.Find("Text");
			Object.Destroy((Object)(object)((Component)obj).GetComponent<AutoLocalizeTextUI>());
			((Component)obj).GetComponent<Text>().text = label;
			Object.Destroy((Object)(object)backButtonObject.GetComponent<EventTrigger>());
			Object.Destroy((Object)(object)backButtonObject.GetComponent<MenuButton>());
			page.Add(backButtonObject);
			backButtonObject.transform.localPosition = Vector3.zero;
			backButtonObject.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
			CanvasGroup val = backButtonObject.AddComponent<CanvasGroup>();
			val.blocksRaycasts = false;
			return (backButtonObject, ((Component)backButtonObject.transform.Find("Text")).GetComponent<Text>(), val);
		}

		public static MenuButton BuildNewButton(string text)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			MenuButton component = BackButtonObject.GetComponent<MenuButton>();
			((Object)component).name = text + " Button";
			component.buttonType = (MenuButtonType)1;
			Transform obj = ((Component)component).transform.Find("Text");
			Object.Destroy((Object)(object)((Component)obj).GetComponent<AutoLocalizeTextUI>());
			((Component)obj).GetComponent<Text>().text = text;
			component.ClearEvents();
			((MenuSelectable)component).cancelAction = (CancelAction)14;
			Navigation navigation = default(Navigation);
			((Navigation)(ref navigation)).mode = (Mode)4;
			((Selectable)component).navigation = navigation;
			return component;
		}

		public static (GameObject, InputField) BuildEntryField()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			GameObject backButtonObject = BackButtonObject;
			MenuButton component = backButtonObject.GetComponent<MenuButton>();
			((Object)component).name = "EntryField";
			component.buttonType = (MenuButtonType)1;
			Object.DestroyImmediate((Object)(object)backButtonObject.GetComponent<MenuButton>());
			Object.DestroyImmediate((Object)(object)backButtonObject.GetComponent<EventTrigger>());
			Object.DestroyImmediate((Object)(object)((Component)backButtonObject.transform.Find("Text")).GetComponent<AutoLocalizeTextUI>());
			Object.DestroyImmediate((Object)(object)((Component)backButtonObject.transform.Find("Text")).GetComponent<FixVerticalAlign>());
			Object.DestroyImmediate((Object)(object)((Component)backButtonObject.transform.Find("Text")).GetComponent<ContentSizeFitter>());
			RectTransform component2 = ((Component)backButtonObject.transform.Find("Text")).GetComponent<RectTransform>();
			Vector2 val = default(Vector2);
			((Vector2)(ref val))..ctor(0.5f, 0.5f);
			component2.anchorMax = val;
			component2.anchorMin = val;
			component2.sizeDelta = new Vector2(337f, 63.2f);
			InputField val2 = (InputField)(object)backButtonObject.AddComponent<CustomInputField>();
			val2.textComponent = ((Component)backButtonObject.transform.Find("Text")).GetComponent<Text>();
			val2.caretColor = Color.white;
			val2.contentType = (ContentType)0;
			Navigation navigation = default(Navigation);
			((Navigation)(ref navigation)).mode = (Mode)4;
			((Selectable)val2).navigation = navigation;
			val2.caretWidth = 8;
			val2.characterLimit = 9;
			val2.text = string.Empty;
			return (backButtonObject, val2);
		}

		public static (GameObject, InputField) BuildMultiLineEntryField(MenuPage page)
		{
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			GameObject descTextObject = DescTextObject;
			SoftMaskScript component = descTextObject.GetComponent<SoftMaskScript>();
			if ((Object)(object)component != (Object)null)
			{
				typeof(Graphic).GetField("m_Canvas", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(((Component)component).GetComponent<Graphic>(), UIManager.instance.UICanvas);
				Object.DestroyImmediate((Object)(object)component, true);
			}
			Text component2 = descTextObject.GetComponent<Text>();
			((Graphic)component2).material = ((Graphic)((Component)BackButtonObject.transform.Find("Text")).GetComponent<Text>()).material;
			page.Add(descTextObject);
			descTextObject.transform.localPosition = Vector3.zero;
			descTextObject.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
			descTextObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
			try
			{
				Object.DestroyImmediate((Object)(object)descTextObject.GetComponent<AutoLocalizeTextUI>());
				Object.DestroyImmediate((Object)(object)descTextObject.GetComponent<FixVerticalAlign>());
				Object.DestroyImmediate((Object)(object)descTextObject.GetComponent<ContentSizeFitter>());
			}
			catch (Exception ex)
			{
				((Loggable)MenuChangerMod.instance).LogError((object)ex);
			}
			RectTransform component3 = descTextObject.GetComponent<RectTransform>();
			Vector2 val = default(Vector2);
			((Vector2)(ref val))..ctor(0.5f, 0.5f);
			component3.anchorMax = val;
			component3.anchorMin = val;
			component3.sizeDelta = new Vector2(450f, 800f);
			InputField val2 = (InputField)(object)descTextObject.AddComponent<CustomInputField>();
			val2.textComponent = component2;
			val2.caretColor = Color.white;
			val2.contentType = (ContentType)0;
			Navigation navigation = default(Navigation);
			((Navigation)(ref navigation)).mode = (Mode)4;
			((Selectable)val2).navigation = navigation;
			val2.caretWidth = 8;
			val2.characterLimit = 600;
			val2.text = string.Empty;
			val2.lineType = (LineType)1;
			return (descTextObject, val2);
		}
	}
	public static class ResumeMenu
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static hook_OnSubmit <0>__SaveSlotButton_OnSubmit;

			public static hook_UIGoToProfileMenu <1>__OnUIGoToProfileMenu;
		}

		private static readonly Dictionary<string, MenuPage> _resumePages = new Dictionary<string, MenuPage>();

		private static MenuPage transitionPage;

		private static MenuLabel unloadingLabel;

		private static MenuLabel loadingLabel;

		private static bool insideResumeMenu = false;

		public static void AddResumePage(string key, MenuPage page)
		{
			_resumePages[key] = page;
		}

		internal static void Reset()
		{
			_resumePages.Clear();
			transitionPage = new MenuPage("ResumeMenu Transition Page");
			transitionPage.backButton.Destroy();
			loadingLabel = new MenuLabel(transitionPage, "Please wait while save data loads...");
			loadingLabel.Hide();
			unloadingLabel = new MenuLabel(transitionPage, "Please wait while save data unloads...");
			unloadingLabel.Hide();
			insideResumeMenu = false;
		}

		internal static void Hook()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			object obj = <>O.<0>__SaveSlotButton_OnSubmit;
			if (obj == null)
			{
				hook_OnSubmit val = SaveSlotButton_OnSubmit;
				<>O.<0>__SaveSlotButton_OnSubmit = val;
				obj = (object)val;
			}
			SaveSlotButton.OnSubmit += (hook_OnSubmit)obj;
			object obj2 = <>O.<1>__OnUIGoToProfileMenu;
			if (obj2 == null)
			{
				hook_UIGoToProfileMenu val2 = OnUIGoToProfileMenu;
				<>O.<1>__OnUIGoToProfileMenu = val2;
				obj2 = (object)val2;
			}
			UIManager.UIGoToProfileMenu += (hook_UIGoToProfileMenu)obj2;
		}

		private static void OnUIGoToProfileMenu(orig_UIGoToProfileMenu orig, UIManager self)
		{
			if (insideResumeMenu)
			{
				InputHandler.Instance.StopUIInput();
				MenuChangerMod.HideAllMenuPages();
				transitionPage.Show();
				unloadingLabel.Show();
				((MonoBehaviour)self).StartCoroutine(GameManager.instance.ReturnToMainMenu((ReturnToMainMenuSaveModes)2, (Action<bool>)null));
			}
			else
			{
				orig.Invoke(self);
			}
		}

		private static SaveStats GetSaveStats(this SaveSlotButton button)
		{
			return ReflectionHelper.GetField<SaveSlotButton, SaveStats>(button, "saveStats");
		}

		private static int GetSaveSlotIndex(this SaveSlotButton button)
		{
			return (int)typeof(SaveSlotButton).GetProperty("SaveSlotIndex", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(button);
		}

		private static IEnumerator GoToResumeMenu(UIManager s, SaveSlotButton button, MenuPage resumePage)
		{
			InputHandler.Instance.StopUIInput();
			yield return s.HideSaveProfileMenu();
			ReflectionHelper.CallMethod<UIManager>(s, "SetMenuState", new object[1] { (object)(MainMenuState)16 });
			transitionPage.Show();
			loadingLabel.Show();
			yield return null;
			yield return LoadGameAndDoAction(button.GetSaveSlotIndex(), delegate
			{
				insideResumeMenu = true;
				loadingLabel.Hide();
				transitionPage.Hide();
				InputHandler.Instance.StartUIInput();
				resumePage.Show();
			});
		}

		private static void SaveSlotButton_OnSubmit(orig_OnSubmit orig, SaveSlotButton self, BaseEventData eventData)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Invalid comparison between Unknown and I4
			if ((int)self.saveFileState == 3 && self.GetSaveStats().permadeathMode != 2)
			{
				try
				{
					Settings settings = MenuChangerMod.instance.ManuallyLoadSettings<MenuChangerMod, Settings>(self.GetSaveSlotIndex());
					if (settings != null && settings.resumeKey != null && _resumePages.TryGetValue(settings.resumeKey, out MenuPage value) && value != null)
					{
						((MenuSelectable)self).ForceDeselect();
						((MonoBehaviour)UIManager.instance).StartCoroutine(GoToResumeMenu(UIManager.instance, self, value));
						return;
					}
				}
				catch (Exception arg)
				{
					((Loggable)MenuChangerMod.instance).LogError($"Unable to manually load settings from menu!\n{arg}");
				}
			}
			else if ((int)self.saveFileState == 2)
			{
				GameManager.instance.profileID = self.GetSaveSlotIndex();
				UIManager.instance.UIGoToPlayModeMenu();
				((MenuSelectable)self).ForceDeselect();
				return;
			}
			orig.Invoke(self, eventData);
		}

		private static IEnumerator LoadGameAndDoAction(int slot, Action a)
		{
			bool finishedLoading = false;
			bool successfullyLoaded = false;
			GameManager.instance.LoadGame(slot, (Action<bool>)delegate(bool b)
			{
				finishedLoading = true;
				successfullyLoaded = b;
			});
			while (!finishedLoading)
			{
				yield return null;
			}
			if (!successfullyLoaded)
			{
				UIManager.instance.UIReturnToMainMenu();
			}
			else
			{
				a?.Invoke();
			}
		}
	}
	[Serializable]
	public class Settings
	{
		public string resumeKey;
	}
	public static class SpaceParameters
	{
		public const float VSPACE_SMALL = 50f;

		public const float VSPACE_MEDIUM = 75f;

		public const float VSPACE_LARGE = 150f;

		public const float HSPACE_SMALL = 400f;

		public const float HSPACE_MEDIUM = 650f;

		public const float HSPACE_LARGE = 800f;

		public static Vector2 TOP_CENTER
		{
			get
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				return new Vector2(0f, 400f);
			}
		}

		public static Vector2 TOP_CENTER_UNDER_TITLE
		{
			get
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				return new Vector2(0f, 300f);
			}
		}
	}
	public class ThreadSupportException : Exception
	{
		public ThreadSupportException()
		{
		}

		public ThreadSupportException(string message)
			: base(message)
		{
		}

		public ThreadSupportException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
	public class ThreadSupport : MonoBehaviour
	{
		private static readonly ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();

		private static ThreadSupport instance;

		internal static void Setup()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			GameObject val = new GameObject();
			instance = val.AddComponent<ThreadSupport>();
			Object.DontDestroyOnLoad((Object)val);
		}

		public void Update()
		{
			Action result;
			while (!actions.IsEmpty && actions.TryDequeue(out result))
			{
				try
				{
					result();
				}
				catch (Exception arg)
				{
					LogHelper.LogError($"Error during ThreadSupport invocation:\n{arg}");
				}
			}
		}

		public static void BeginInvoke(Action a)
		{
			actions.Enqueue(a);
		}

		public static void BlockUntilInvoked(Action a)
		{
			EventWaitHandle h = new EventWaitHandle(initialState: false, EventResetMode.AutoReset);
			Exception error = null;
			BeginInvoke(delegate
			{
				try
				{
					a();
				}
				catch (Exception ex)
				{
					error = ex;
				}
				h.Set();
			});
			h.WaitOne();
			if (error != null)
			{
				throw new ThreadSupportException("An exception was thrown during a ThreadSupport invocation", error);
			}
		}

		public static T BlockUntilInvoked<T>(Func<T> f)
		{
			EventWaitHandle h = new EventWaitHandle(initialState: false, EventResetMode.AutoReset);
			T result = default(T);
			Exception error = null;
			BeginInvoke(delegate
			{
				try
				{
					result = f();
				}
				catch (Exception ex)
				{
					error = ex;
				}
				h.Set();
			});
			h.WaitOne();
			if (error != null)
			{
				throw new ThreadSupportException("An exception was thrown during a ThreadSupport invocation", error);
			}
			return result;
		}
	}
}
namespace MenuChanger.NavigationTypes
{
	public class SimpleHorizontalNavigation : MenuPageNavigation
	{
		public List<ISelectable> Selectables = new List<ISelectable>();

		public override IReadOnlyCollection<ISelectable> Items => Selectables.AsReadOnly();

		public SimpleHorizontalNavigation(MenuPage page)
			: base(page)
		{
		}

		public override void SelectDefault()
		{
			if (Selectables.Count > 0)
			{
				Selectable selectable = Selectables[0].GetSelectable(Neighbor.Up);
				if (Object.op_Implicit((Object)(object)selectable))
				{
					selectable.Select();
					return;
				}
			}
			if (base.Page.backButton != null && Object.op_Implicit((Object)(object)base.Page.backButton.Button))
			{
				((Selectable)base.Page.backButton.Button).Select();
			}
		}

		public override void Add(ISelectable selectable)
		{
			selectable.SetNeighbor(Neighbor.Down, base.Page.backButton);
			selectable.SetNeighbor(Neighbor.Up, base.Page.backButton);
			if (Selectables.Any())
			{
				selectable.SetNeighbor(Neighbor.Left, Selectables[Selectables.Count - 1]);
				Selectables[Selectables.Count - 1].SetNeighbor(Neighbor.Right, selectable);
				selectable.SetNeighbor(Neighbor.Right, Selectables[0]);
				Selectables[0].SetNeighbor(Neighbor.Left, selectable);
			}
			else
			{
				base.Page.backButton.SetNeighbor(Neighbor.Up, selectable);
				base.Page.backButton.SetNeighbor(Neighbor.Down, selectable);
				selectable.SetNeighbor(Neighbor.Left, selectable);
				selectable.SetNeighbor(Neighbor.Right, selectable);
			}
			Selectables.Add(selectable);
		}

		public override void Remove(ISelectable selectable)
		{
			int num = Selectables.IndexOf(selectable);
			if (num < 0)
			{
				return;
			}
			Selectables.RemoveAt(num);
			if (Selectables.Any())
			{
				ISelectable selectable2 = Selectables[(num > 0) ? (num - 1) : (Selectables.Count - 1)];
				ISelectable selectable3 = Selectables[(num < Selectables.Count) ? num : 0];
				selectable2.SetNeighbor(Neighbor.Right, selectable3);
				selectable3.SetNeighbor(Neighbor.Left, selectable2);
				if (num == 0)
				{
					base.Page.backButton.SetNeighbor(Neighbor.Up, selectable3);
					base.Page.backButton.SetNeighbor(Neighbor.Down, selectable3);
				}
			}
			else
			{
				base.Page.backButton.SetNeighbor(Neighbor.Up, null);
				base.Page.backButton.SetNeighbor(Neighbor.Down, null);
			}
		}

		public override void ResetNavigation()
		{
			if (Selectables.Count == 0)
			{
				base.Page.backButton.SetNeighbor(Neighbor.Up, null);
				base.Page.backButton.SetNeighbor(Neighbor.Down, null);
				return;
			}
			foreach (ISelectable selectable in Selectables)
			{
				if (selectable is ISelectableGroup selectableGroup)
				{
					selectableGroup.ResetNavigation();
				}
			}
			for (int i = 0; i < Selectables.Count; i++)
			{
				int index = ((i > 0) ? (i - 1) : (Selectables.Count - 1));
				Selectables[i].SymSetNeighbor(Neighbor.Left, Selectables[index]);
				Selectables[i].SetNeighbor(Neighbor.Up, base.Page.backButton);
				Selectables[i].SetNeighbor(Neighbor.Down, base.Page.backButton);
			}
			base.Page.backButton.SetNeighbor(Neighbor.Up, Selectables[0]);
			base.Page.backButton.SetNeighbor(Neighbor.Down, Selectables[0]);
		}
	}
}
namespace MenuChanger.MenuPanels
{
	public class GridItemPanel : IMenuPanel, IMenuElement, ISelectable, ISelectableGroup
	{
		private Vector2 localTopCenter;

		private float vspace;

		private float hspace;

		private int columns;

		public MenuPage Parent { get; }

		public List<IMenuElement> Items { get; }

		public bool Hidden { get; private set; }

		public GridItemPanel(MenuPage page, Vector2 localTopCenter, int columns, float vspace, float hspace, bool rootLevel, params IMenuElement[] children)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			base..ctor();
			Parent = page;
			this.localTopCenter = localTopCenter;
			this.columns = columns;
			this.vspace = vspace;
			this.hspace = hspace;
			Items = children.ToList();
			Reposition();
			ResetNavigation();
			if (rootLevel)
			{
				Parent.AddToNavigationControl(this);
			}
		}

		public void Reposition()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = localTopCenter - new Vector2(((float)columns / 2f - 0.5f) * hspace, 0f);
			for (int i = 0; i < Items.Count; i++)
			{
				Items[i].MoveTo(val + new Vector2((float)(i % columns) * hspace, (float)(i / columns) * (0f - vspace)));
			}
		}

		public void MoveTo(Vector2 pos)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			localTopCenter = pos;
			Reposition();
		}

		public void Translate(Vector2 delta)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			localTopCenter += delta;
			Reposition();
		}

		public void Respace(float vspace)
		{
			this.vspace = vspace;
			Reposition();
		}

		public void Add(IMenuElement item)
		{
			Items.Add(item);
			Reposition();
			if (Hidden)
			{
				item.Hide();
			}
		}

		public void AddRange(IEnumerable<IMenuElement> items)
		{
			Items.AddRange(items);
			Reposition();
			if (!Hidden)
			{
				return;
			}
			foreach (IMenuElement item in items)
			{
				item.Hide();
			}
		}

		public void Insert(int index, IMenuElement item)
		{
			Items.Insert(index, item);
			Reposition();
			if (Hidden)
			{
				item.Hide();
			}
		}

		public void Insert(int row, int column, IMenuElement item)
		{
			Insert(row * columns + column, item);
		}

		public bool Remove(IMenuElement item)
		{
			if (Items.Remove(item))
			{
				Reposition();
				return true;
			}
			return false;
		}

		public void RemoveAt(int index)
		{
			Items.RemoveAt(index);
			Reposition();
		}

		public void Clear()
		{
			foreach (IMenuElement item in Items)
			{
				item.Hide();
			}
			Items.Clear();
		}

		public void Hide()
		{
			Hidden = true;
			foreach (IMenuElement item in Items)
			{
				item.Hide();
			}
		}

		public void Show()
		{
			Hidden = false;
			foreach (IMenuElement item in Items)
			{
				item.Show();
			}
		}

		public void Destroy()
		{
			foreach (IMenuElement item in Items)
			{
				item.Destroy();
			}
			Items.Clear();
		}

		private IEnumerable<IMenuElement> GetColumn(int c)
		{
			return Items.Where((IMenuElement e, int i) => i % columns == c);
		}

		private IEnumerable<IMenuElement> GetRow(int r)
		{
			return Items.Skip(r * columns).Take(columns);
		}

		public void ResetNavigation()
		{
			foreach (ISelectableGroup item in Items.OfType<ISelectableGroup>())
			{
				item.ResetNavigation();
			}
			for (int i = 0; i * columns < Items.Count; i++)
			{
				ISelectable selectable = null;
				foreach (ISelectable item2 in GetRow(i).OfType<ISelectable>())
				{
					if (selectable != null)
					{
						item2.SetNeighbor(Neighbor.Left, selectable);
						selectable.SetNeighbor(Neighbor.Right, item2);
					}
					selectable = item2;
				}
			}
			for (int j = 0; j < columns; j++)
			{
				ISelectable selectable2 = null;
				foreach (ISelectable item3 in GetColumn(j).OfType<ISelectable>())
				{
					if (selectable2 != null)
					{
						item3.SetNeighbor(Neighbor.Up, selectable2);
						selectable2.SetNeighbor(Neighbor.Down, item3);
					}
					selectable2 = item3;
				}
			}
		}

		public void SetNeighbor(Neighbor neighbor, ISelectable selectable)
		{
			switch (neighbor)
			{
			case Neighbor.Up:
			{
				for (int l = 0; l < columns; l++)
				{
					GetColumn(l).OfType<ISelectable>().FirstOrDefault()?.SetNeighbor(neighbor, selectable);
				}
				break;
			}
			case Neighbor.Down:
			{
				for (int j = 0; j < columns; j++)
				{
					GetColumn(j).OfType<ISelectable>().LastOrDefault()?.SetNeighbor(neighbor, selectable);
				}
				break;
			}
			case Neighbor.Left:
			{
				for (int k = 0; k * columns < Items.Count; k++)
				{
					GetRow(k).OfType<ISelectable>().FirstOrDefault()?.SetNeighbor(neighbor, selectable);
				}
				break;
			}
			case Neighbor.Right:
			{
				for (int i = 0; i * columns < Items.Count; i++)
				{
					GetRow(i).OfType<ISelectable>().LastOrDefault()?.SetNeighbor(neighbor, selectable);
				}
				break;
			}
			}
		}

		public Selectable GetSelectable(Neighbor neighbor)
		{
			return GetISelectable(neighbor)?.GetSelectable(neighbor);
		}

		public ISelectable GetISelectable(Neighbor neighbor)
		{
			IEnumerable<ISelectable> source = Items.OfType<ISelectable>();
			if (!source.Any())
			{
				return null;
			}
			return neighbor switch
			{
				Neighbor.Up => source.First(), 
				Neighbor.Down => source.Last(), 
				Neighbor.Left => source.First(), 
				Neighbor.Right => source.Last(), 
				_ => null, 
			};
		}
	}
	public interface IMenuPanel : IMenuElement, ISelectable, ISelectableGroup
	{
		List<IMenuElement> Items { get; }

		void Add(IMenuElement obj);

		bool Remove(IMenuElement obj);
	}
	public class MultiGridItemPanel : OrderedItemViewer
	{
		private readonly int Columns;

		private readonly int GridCount;

		private readonly float Vspace;

		private readonly float Hspace;

		private Vector2 TopCenter;

		private readonly List<GridItemPanel> recycledPanels;

		public MultiGridItemPanel(MenuPage page, int rows, int columns, float vspace, float hspace, Vector2 topCenter, params IMenuElement[] children)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			recycledPanels = new List<GridItemPanel>();
			base..ctor(page);
			Columns = columns;
			Vspace = vspace;
			Hspace = hspace;
			TopCenter = topCenter;
			GridCount = rows * columns;
			for (int i = 0; i * GridCount < children.Length; i++)
			{
				int num = Math.Min(children.Length - i * GridCount, GridCount);
				if (num <= 0)
				{
					break;
				}
				IMenuElement[] array = new IMenuElement[num];
				Array.Copy(children, i * GridCount, array, 0, num);
				base.Add(NewPanel(array));
			}
			if (!base.Items.Any())
			{
				return;
			}
			foreach (IMenuElement item in base.Items)
			{
				item.Hide();
			}
			base.Items[base.Index].Show();
		}

		public MultiGridItemPanel(MenuPage page, int rows, int columns, float vspace, float hspace, Vector2 topCenter, Vector2 prevPos, Vector2 nextPos, Vector2 indexPos, params IMenuElement[] children)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			recycledPanels = new List<GridItemPanel>();
			base..ctor(page, prevPos, nextPos, indexPos);
			Columns = columns;
			Vspace = vspace;
			Hspace = hspace;
			TopCenter = topCenter;
			GridCount = rows * columns;
			AddRange(children);
		}

		private GridItemPanel NewPanel(params IMenuElement[] children)
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			if (recycledPanels.Count != 0)
			{
				GridItemPanel gridItemPanel = recycledPanels[recycledPanels.Count - 1];
				gridItemPanel.AddRange(children);
				recycledPanels.RemoveAt(recycledPanels.Count - 1);
				return gridItemPanel;
			}
			return new GridItemPanel(base.Parent, TopCenter, Columns, Vspace, Hspace, rootLevel: false, children);
		}

		private GridItemPanel NewPanel(IEnumerable<IMenuElement> children)
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			if (recycledPanels.Count != 0)
			{
				GridItemPanel gridItemPanel = recycledPanels[recycledPanels.Count - 1];
				gridItemPanel.AddRange(children);
				recycledPanels.RemoveAt(recycledPanels.Count - 1);
				return gridItemPanel;
			}
			return new GridItemPanel(base.Parent, TopCenter, Columns, Vspace, Hspace, rootLevel: false, children.ToArray());
		}

		public override void Add(IMenuElement item)
		{
			AddRange(new IMenuElement[1] { item });
		}

		public void AddRange(IEnumerable<IMenuElement> items)
		{
			if (base.Items.Count > 0)
			{
				GridItemPanel gridItemPanel = (GridItemPanel)base.Items[base.Items.Count - 1];
				int num = GridCount - gridItemPanel.Items.Count;
				if (num > 0)
				{
					gridItemPanel.AddRange(items.Take(num));
					if (gridItemPanel.Hidden)
					{
						gridItemPanel.Hide();
					}
					items = items.Skip(num);
				}
			}
			while (items.Any())
			{
				GridItemPanel obj = NewPanel(items.Take(GridCount));
				base.Add(obj);
				items = items.Skip(GridCount);
			}
			if (base.Items.Count > 0)
			{
				base.Items[base.Index].Show();
			}
		}

		public override void Clear()
		{
			foreach (GridItemPanel item in base.Items)
			{
				item.Clear();
				recycledPanels.Add(item);
			}
			base.Clear();
		}

		public override bool Remove(IMenuElement obj)
		{
			for (int i = 0; i < base.Items.Count; i++)
			{
				if (((GridItemPanel)base.Items[i]).Items.Remove(obj))
				{
					return true;
				}
			}
			return false;
		}
	}
	public class OrderedItemViewer : IMenuPanel, IMenuElement, ISelectable, ISelectableGroup
	{
		private int _index;

		private Vector2 offset;

		private readonly Dictionary<Neighbor, ISelectable> neighbors;

		public List<IMenuElement> Items { get; }

		public MenuPage Parent { get; }

		public SmallButton PrevButton { get; }

		public SmallButton NextButton { get; }

		public MenuLabel IndexLabel { get; }

		public bool Hidden { get; private set; }

		public int Index
		{
			get
			{
				return _index;
			}
			set
			{
				_index = (value % Items.Count + Items.Count) % Items.Count;
			}
		}

		public OrderedItemViewer(MenuPage page, params IMenuElement[] items)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			this..ctor(page, new Vector2(-800f, -400f), new Vector2(800f, -400f), new Vector2(0f, -400f), items);
		}

		public OrderedItemViewer(MenuPage page, Vector2 prevPos, Vector2 nextPos, Vector2 indexLabelPos, params IMenuElement[] items)
		{
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			Hidden = true;
			neighbors = new Dictionary<Neighbor, ISelectable>();
			base..ctor();
			Parent = page;
			Items = items.ToList();
			if (Items.Any())
			{
				foreach (IMenuElement item in Items)
				{
					item.Hide();
				}
				Items[Index].Show();
			}
			PrevButton = new SmallButton(Parent, "<<");
			PrevButton.Button.AddEvent(ToPrevious);
			PrevButton.MoveTo(prevPos);
			IndexLabel = new MenuLabel(Parent, ComputeCounterText());
			IndexLabel.MoveTo(indexLabelPos);
			NextButton = new SmallButton(Parent, ">>");
			NextButton.Button.AddEvent(ToNext);
			NextButton.MoveTo(nextPos);
			ResetNavigation();
			Parent.AddToNavigationControl(this);
		}

		protected string ComputeCounterText()
		{
			return $"{Index + 1}/{Items.Count}";
		}

		public void ToPrevious()
		{
			if (Items.Count != 0)
			{
				Items[Index--].Hide();
				Items[Index].Show();
				IndexLabel.Text.text = ComputeCounterText();
				ResetNavigation();
			}
		}

		public void ToNext()
		{
			if (Items.Count != 0)
			{
				Items[Index++].Hide();
				Items[Index].Show();
				IndexLabel.Text.text = ComputeCounterText();
				ResetNavigation();
			}
		}

		public void JumpTo(int index)
		{
			if (index >= 0 && index < Items.Count)
			{
				Items[Index].Hide();
				Index = index;
				Items[Index].Show();
				IndexLabel.Text.text = ComputeCounterText();
				ResetNavigation();
			}
		}

		public void JumpTo(IMenuElement element)
		{
			int num = Items.IndexOf(element);
			if (num >= 0)
			{
				JumpTo(num);
			}
		}

		public virtual void Add(IMenuElement obj)
		{
			Items.Add(obj);
			obj.Hide();
			IndexLabel.Text.text = ComputeCounterText();
			ResetNavigation();
		}

		public virtual bool Remove(IMenuElement obj)
		{
			bool result = Items.Remove(obj);
			IndexLabel.Text.text = ComputeCounterText();
			ResetNavigation();
			return result;
		}

		public virtual void Clear()
		{
			foreach (IMenuElement item in Items)
			{
				item.Hide();
			}
			Items.Clear();
			_index = 0;
			IndexLabel.Text.text = ComputeCounterText();
			ResetNavigation();
		}

		public void MoveTo(Vector2 pos)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			Translate(pos - offset);
		}

		public void Translate(Vector2 delta)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			offset += delta;
			foreach (IMenuElement item in Items)
			{
				item.Translate(delta);
			}
		}

		public void Show()
		{
			Hidden = false;
			Items[Index].Show();
			PrevButton.Show();
			IndexLabel.Show();
			NextButton.Show();
		}

		public void Hide()
		{
			Hidden = true;
			Items[Index].Hide();
			PrevButton.Hide();
			IndexLabel.Hide();
			NextButton.Hide();
		}

		public void Destroy()
		{
			foreach (IMenuElement item in Items)
			{
				item.Destroy();
			}
			Items.Clear();
			PrevButton.Destroy();
			IndexLabel.Destroy();
			NextButton.Destroy();
		}

		public void SetNeighbor(Neighbor neighbor, ISelectable selectable)
		{
			switch (neighbor)
			{
			case Neighbor.Down:
				PrevButton.SetNeighbor(neighbor, selectable);
				NextButton.SetNeighbor(neighbor, selectable);
				break;
			case Neighbor.Left:
			case Neighbor.Right:
			case Neighbor.Up:
				neighbors[neighbor] = selectable;
				if (Items.Count > 0 && Items[Index] is ISelectable selectable2)
				{
					selectable2.SetNeighbor(neighbor, selectable);
				}
				break;
			}
		}

		public Selectable GetSelectable(Neighbor neighbor)
		{
			return GetISelectable(neighbor)?.GetSelectable(neighbor);
		}

		public ISelectable GetISelectable(Neighbor neighbor)
		{
			if (Items.Count == 0 || !(Items[Index] is ISelectable result))
			{
				return neighbor switch
				{
					Neighbor.Up => PrevButton, 
					Neighbor.Left => PrevButton, 
					Neighbor.Right => NextButton, 
					Neighbor.Down => NextButton, 
					_ => null, 
				};
			}
			if (neighbor == Neighbor.Down)
			{
				return NextButton;
			}
			return result;
		}

		public void ResetNavigation()
		{
			PrevButton.SetNeighbor(Neighbor.Right, Parent.backButton);
			Parent.backButton.SetNeighbor(Neighbor.Left, PrevButton);
			NextButton.SetNeighbor(Neighbor.Left, Parent.backButton);
			Parent.backButton.SetNeighbor(Neighbor.Right, NextButton);
			PrevButton.SetNeighbor(Neighbor.Left, NextButton);
			NextButton.SetNeighbor(Neighbor.Right, PrevButton);
			if (Items.Count <= 0)
			{
				return;
			}
			if (Items[Index] is ISelectableGroup selectableGroup)
			{
				selectableGroup.ResetNavigation();
			}
			if (!(Items[Index] is ISelectable selectable))
			{
				return;
			}
			PrevButton.SetNeighbor(Neighbor.Up, selectable);
			NextButton.SetNeighbor(Neighbor.Up, selectable);
			selectable.SetNeighbor(Neighbor.Down, Parent.backButton);
			foreach (KeyValuePair<Neighbor, ISelectable> neighbor in neighbors)
			{
				selectable.SetNeighbor(neighbor.Key, neighbor.Value);
			}
		}
	}
	public class Subpage : IMenuPanel, IMenuElement, ISelectable, ISelectableGroup
	{
		private Vector2 offset;

		public MenuPage Parent { get; }

		public List<IMenuElement> Items { get; }

		public MenuLabel TitleLabel { get; }

		public bool Hidden { get; private set; }

		public Subpage(MenuPage page, string title, Vector2 titlePos)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			base..ctor();
			Parent = page;
			TitleLabel = new MenuLabel(page, title);
			TitleLabel.MoveTo(titlePos);
			Items = new List<IMenuElement>();
		}

		public Subpage(MenuPage page, string title)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			this..ctor(page, title, new Vector2(0f, 400f));
		}

		public Subpage(MenuPage page)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			this..ctor(page, string.Empty, new Vector2(0f, 400f));
		}

		public void Add(IMenuElement obj)
		{
			Items.Add(obj);
			if (Hidden)
			{
				obj.Hide();
			}
		}

		public bool Remove(IMenuElement obj)
		{
			return Items.Remove(obj);
		}

		public void MoveTo(Vector2 pos)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			Translate(pos - offset);
		}

		public void Translate(Vector2 delta)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			offset += delta;
			foreach (IMenuElement item in Items)
			{
				item.Translate(delta);
			}
			TitleLabel?.Translate(delta);
		}

		public void Show()
		{
			Hidden = false;
			foreach (IMenuElement item in Items)
			{
				item.Show();
			}
			TitleLabel?.Show();
		}

		public void Hide()
		{
			Hidden = true;
			foreach (IMenuElement item in Items)
			{
				item.Hide();
			}
			TitleLabel?.Hide();
		}

		public void Destroy()
		{
			foreach (IMenuElement item in Items)
			{
				item.Destroy();
			}
			Items.Clear();
			TitleLabel?.Destroy();
		}

		public void SetNeighbor(Neighbor neighbor, ISelectable selectable)
		{
			IEnumerable<ISelectable> enumerable = Items.OfType<ISelectable>();
			if (!enumerable.Any())
			{
				return;
			}
			switch (neighbor)
			{
			case Neighbor.Up:
			{
				foreach (ISelectable item in enumerable)
				{
					item.SetNeighbor(neighbor, selectable);
				}
				break;
			}
			case Neighbor.Down:
			{
				foreach (ISelectable item2 in enumerable)
				{
					item2.SetNeighbor(neighbor, selectable);
				}
				break;
			}
			case Neighbor.Left:
				enumerable.First().SetNeighbor(neighbor, selectable);
				break;
			case Neighbor.Right:
				enumerable.Last().SetNeighbor(neighbor, selectable);
				break;
			}
		}

		public Selectable GetSelectable(Neighbor neighbor)
		{
			return GetISelectable(neighbor)?.GetSelectable(neighbor);
		}

		public ISelectable GetISelectable(Neighbor neighbor)
		{
			IEnumerable<ISelectable> source = Items.OfType<ISelectable>();
			if (!source.Any())
			{
				return null;
			}
			return neighbor switch
			{
				Neighbor.Up => source.First(), 
				Neighbor.Down => source.Last(), 
				Neighbor.Left => source.First(), 
				Neighbor.Right => source.Last(), 
				_ => null, 
			};
		}

		public void ResetNavigation()
		{
			foreach (ISelectableGroup item in Items.OfType<ISelectableGroup>())
			{
				item.ResetNavigation();
			}
			ISelectable selectable = null;
			foreach (ISelectable item2 in Items.OfType<ISelectable>())
			{
				if (selectable != null)
				{
					item2.SetNeighbor(Neighbor.Left, selectable);
					selectable.SetNeighbor(Neighbor.Right, item2);
				}
				selectable = item2;
			}
		}
	}
	public class VerticalItemPanel : IMenuPanel, IMenuElement, ISelectable, ISelectableGroup
	{
		private Vector2 localTopCenter;

		private float vspace;

		public MenuPage Parent { get; }

		public List<IMenuElement> Items { get; }

		public bool Hidden { get; private set; }

		public VerticalItemPanel(MenuPage page, Vector2 localTopCenter, float vspace, bool rootLevel, params IMenuElement[] children)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			Hidden = true;
			base..ctor();
			Parent = page;
			this.localTopCenter = localTopCenter;
			this.vspace = vspace;
			Items = children.ToList();
			Reposition();
			ResetNavigation();
			if (rootLevel)
			{
				Parent.AddToNavigationControl(this);
			}
		}

		public void Reposition()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < Items.Count; i++)
			{
				Items[i].MoveTo(localTopCenter + new Vector2(0f, (0f - vspace) * (float)i));
			}
		}

		public void MoveTo(Vector2 pos)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			localTopCenter = pos;
			Reposition();
		}

		public void Translate(Vector2 delta)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			localTopCenter += delta;
			Reposition();
		}

		public void Respace(float vspace)
		{
			this.vspace = vspace;
			Reposition();
		}

		public void Add(IMenuElement item)
		{
			Items.Add(item);
			Reposition();
		}

		public void Insert(int index, IMenuElement item)
		{
			Items.Insert(index, item);
			Reposition();
		}

		public bool Remove(IMenuElement item)
		{
			if (Items.Remove(item))
			{
				Reposition();
				return true;
			}
			return false;
		}

		public void RemoveAt(int index)
		{
			Items.RemoveAt(index);
			Reposition();
		}

		public void Hide()
		{
			Hidden = true;
			foreach (IMenuElement item in Items)
			{
				item.Hide();
			}
		}

		public void Show()
		{
			Hidden = false;
			foreach (IMenuElement item in Items)
			{
				item.Show();
			}
		}

		public void Destroy()
		{
			foreach (IMenuElement item in Items)
			{
				item.Destroy();
			}
			Items.Clear();
		}

		public void SetNeighbor(Neighbor neighbor, ISelectable selectable)
		{
			IEnumerable<ISelectable> enumerable = Items.OfType<ISelectable>();
			if (!enumerable.Any())
			{
				return;
			}
			switch (neighbor)
			{
			case Neighbor.Up:
				enumerable.First().SetNeighbor(neighbor, selectable);
				break;
			case Neighbor.Down:
				enumerable.Last().SetNeighbor(neighbor, selectable);
				break;
			case Neighbor.Left:
			{
				foreach (ISelectable item in enumerable)
				{
					item.SetNeighbor(neighbor, selectable);
				}
				break;
			}
			case Neighbor.Right:
			{
				foreach (ISelectable item2 in enumerable)
				{
					item2.SetNeighbor(neighbor, selectable);
				}
				break;
			}
			}
		}

		public Selectable GetSelectable(Neighbor neighbor)
		{
			return GetISelectable(neighbor)?.GetSelectable(neighbor);
		}

		public ISelectable GetISelectable(Neighbor neighbor)
		{
			IEnumerable<ISelectable> source = Items.OfType<ISelectable>();
			if (!source.Any())
			{
				return null;
			}
			return neighbor switch
			{
				Neighbor.Up => source.First(), 
				Neighbor.Down => source.Last(), 
				Neighbor.Left => source.First(), 
				Neighbor.Right => source.Last(), 
				_ => null, 
			};
		}

		public void ResetNavigation()
		{
			foreach (ISelectableGroup item in Items.OfType<ISelectableGroup>())
			{
				item.ResetNavigation();
			}
			ISelectable selectable = null;
			foreach (ISelectable item2 in Items.OfType<ISelectable>())
			{
				if (selectable != null)
				{
					item2.SetNeighbor(Neighbor.Up, selectable);
					selectable.SetNeighbor(Neighbor.Down, item2);
				}
				selectable = item2;
			}
		}
	}
}
namespace MenuChanger.MenuElements
{
	public class BaseButton : IMenuElement, ISelectable
	{
		public Action OnCancel;

		public MenuPage Parent { get; }

		public MenuButton Button { get; }

		public GameObject GameObject { get; }

		public bool Hidden { get; private set; }

		public event Action OnClick;

		protected virtual void InvokeOnClick()
		{
			OnClick?.Invoke();
		}

		public void ClearOnClick()
		{
			OnClick = null;
		}

		protected virtual void InvokeOnCancel()
		{
			OnCancel?.Invoke();
		}

		public BaseButton(MenuPage page, MenuButton newButton)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			base..ctor();
			Parent = page;
			Button = newButton;
			GameObject = ((Component)Button).gameObject;
			Parent.Add(GameObject);
			GameObject.transform.localPosition = Vector3.zero;
			newButton.AddEvent(InvokeOnClick);
			((MenuSelectable)newButton).customCancelAction = delegate
			{
				InvokeOnCancel();
			};
			OnCancel = page.GoBack;
		}

		public void Show()
		{
			Hidden = false;
			GameObject.SetActive(true);
		}

		public void Hide()
		{
			Hidden = true;
			GameObject.SetActive(false);
		}

		public void Destroy()
		{
			Object.Destroy((Object)(object)GameObject);
		}

		public void Translate(Vector2 delta)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = GameObject.transform;
			transform.localPosition += Vector2.op_Implicit(delta);
		}

		public void MoveTo(Vector2 pos)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			GameObject.transform.localPosition = Vector2.op_Implicit(pos);
		}

		public void SetNeighbor(Neighbor neighbor, ISelectable selectable)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			Navigation navigation = ((Selectable)Button).navigation;
			switch (neighbor)
			{
			case Neighbor.Up:
				((Navigation)(ref navigation)).selectOnUp = selectable?.GetSelectable(Neighbor.Down);
				break;
			case Neighbor.Down:
				((Navigation)(ref navigation)).selectOnDown = selectable?.GetSelectable(Neighbor.Up);
				break;
			case Neighbor.Right:
				((Navigation)(ref navigation)).selectOnRight = selectable?.GetSelectable(Neighbor.Left);
				break;
			case Neighbor.Left:
				((Navigation)(ref navigation)).selectOnLeft = selectable?.GetSelectable(Neighbor.Right);
				break;
			}
			((Selectable)Button).navigation = navigation;
		}

		public ISelectable GetISelectable(Neighbor neighbor)
		{
			return this;
		}

		public Selectable GetSelectable(Neighbor neighbor)
		{
			return (Selectable)(object)Button;
		}
	}
	public class BigButton : BaseButton
	{
		private BigButton(MenuPage page, MenuButton button)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			base..ctor(page, button);
			((Component)button).transform.localScale = Vector2.op_Implicit(new Vector2(0.45f, 0.45f));
			((Component)button).transform.Find("Selector").localScale = Vector2.op_Implicit(new Vector2(0.3f, 0.5f));
		}

		public BigButton(MenuPage page, string title)
			: this(page, PrefabMenuObjects.BuildBigButtonOneTextNoSprite(title))
		{
		}

		public BigButton(MenuPage page, string title, string desc)
			: this(page, PrefabMenuObjects.BuildBigButtonTwoTextNoSprite(title, desc))
		{
		}

		public BigButton(MenuPage page, Sprite sprite, string title, string desc)
			: this(page, PrefabMenuObjects.BuildBigButtonTwoTextAndSprite(sprite, title, desc))
		{
		}

		public BigButton(MenuPage page, Sprite sprite, string title)
			: this(page, PrefabMenuObjects.BuildBigButtonOneTextAndSprite(sprite, title))
		{
		}

		public BigButton(MenuPage page, Sprite sprite)
			: this(page, PrefabMenuObjects.BuildBigButtonSpriteOnly(sprite))
		{
		}

		public BigButton(MenuPage page, Mode mode, bool removeAction = false)
			: this(page, PrefabMenuObjects.CloneBigButton(mode).button)
		{
			if (removeAction)
			{
				base.Button.ClearEvents();
			}
		}
	}
	public class CounterLabel : MenuLabel
	{
		private int Counter;

		private string Prefix;

		public CounterLabel(MenuPage page, string prefix, int startValue = 0)
			: base(page, $"{prefix}: {startValue}", Style.Body)
		{
			Counter = startValue;
			Prefix = prefix + ": ";
			base.Text.alignment = (TextAnchor)1;
		}

		public string ComputeText()
		{
			return $"{Prefix}{Counter}";
		}

		public void Update()
		{
			base.Text.text = ComputeText();
		}

		public void Incr(int val = 1)
		{
			Counter += val;
			Update();
		}

		public void Set(int val)
		{
			Counter = val;
			Update();
		}
	}
	public abstract class EntryField : IMenuElement, ISelectable, IValueElement
	{
		public delegate void ModifyInputHandler<T>(ref T newValue, T orig);

		public Vector2 LabelOffset;

		public MenuPage Parent { get; }

		public GameObject GameObject { get; }

		public InputField InputField { get; }

		public bool Hidden { get; private set; }

		public MenuLabel Label { get; }

		public string ValidatedInput { get; protected set; }

		public object Value { get; protected set; }

		public Type ValueType { get; }

		public event ModifyInputHandler<string> ModifyInputString;

		public event Action<IValueElement> SelfChanged;

		public EntryField(Type valueType, MenuPage page, string label, MenuLabel.Style style = MenuLabel.Style.Title)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			LabelOffset = new Vector2(0f, 55f);
			base..ctor();
			ValueType = valueType;
			if (style != MenuLabel.Style.Title && style == MenuLabel.Style.Body)
			{
				(GameObject, InputField) = PrefabMenuObjects.BuildMultiLineEntryField(page);
			}
			else
			{
				(GameObject, InputField) = PrefabMenuObjects.BuildEntryField();
				page.Add(GameObject);
				GameObject.transform.localScale = Vector2.op_Implicit(new Vector2(0.8f, 0.8f));
			}
			Parent = page;
			Label = new MenuLabel(page, label, MenuLabel.Style.Body);
			Label.Text.alignment = (TextAnchor)1;
			MoveTo(Vector2.zero);
			((UnityEvent<string>)(object)InputField.onEndEdit).AddListener((UnityAction<string>)Validate);
		}

		protected virtual void Validate(string input)
		{
			object value;
			try
			{
				InvokeModifyInputString(ref input, ValidatedInput);
				value = Read(input, ValueType);
			}
			catch
			{
				InputField.text = ValidatedInput;
				return;
			}
			ValidatedInput = input;
			Value = value;
			try
			{
				InvokeSelfChanged();
			}
			catch
			{
			}
		}

		protected void InvokeModifyInputString(ref string newValue, string orig)
		{
			ModifyInputString?.Invoke(ref newValue, orig);
		}

		protected void InvokeSelfChanged()
		{
			try
			{
				SelfChanged?.Invoke(this);
			}
			catch
			{
			}
		}

		public virtual object Read(string input, Type type)
		{
			return Convert.ChangeType(input, type);
		}

		public virtual string Write(object t)
		{
			if (t == null)
			{
				return string.Empty;
			}
			return t.ToString();
		}

		public void SetValue(object o)
		{
			Validate(Write(o));
		}

		public virtual void Bind(object o, MemberInfo mi)
		{
			SetValue(mi.GetValue(o));
			SelfChanged += delegate(IValueElement self)
			{
				mi.SetValue(o, self.Value);
			};
		}

		public void AddValidateInputToTextColorEvent(Func<string, bool> test)
		{
			SelfChanged += delegate(IValueElement self)
			{
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				if (test(((EntryField)self).ValidatedInput))
				{
					((Graphic)InputField.textComponent).color = Colors.DEFAULT_COLOR;
				}
				else
				{
					((Graphic)InputField.textComponent).color = Colors.INVALID_INPUT_COLOR;
				}
			};
		}

		public void MoveTo(Vector2 pos)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			GameObject.transform.localPosition = Vector2.op_Implicit(pos);
			Label.MoveTo(pos + LabelOffset);
		}

		public void Translate(Vector2 delta)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = GameObject.transform;
			transform.localPosition += Vector2.op_Implicit(delta);
			Label.Translate(delta);
		}

		public void Show()
		{
			Hidden = false;
			GameObject.SetActive(true);
			Label.Show();
		}

		public void Hide()
		{
			Hidden = true;
			GameObject.SetActive(false);
			Label.Hide();
		}

		public void Destroy()
		{
			Object.Destroy((Object)(object)GameObject);
		}

		public void SetNeighbor(Neighbor neighbor, ISelectable selectable)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			Navigation navigation = ((Selectable)InputField).navigation;
			switch (neighbor)
			{
			case Neighbor.Up:
				((Navigation)(ref navigation)).selectOnUp = selectable?.GetSelectable(Neighbor.Down);
				break;
			case Neighbor.Down:
				((Navigation)(ref navigation)).selectOnDown = selectable?.GetSelectable(Neighbor.Up);
				break;
			case Neighbor.Right:
				((Navigation)(ref navigation)).selectOnRight = selectable?.GetSelectable(Neighbor.Left);
				break;
			case Neighbor.Left:
				((Navigation)(ref navigation)).selectOnLeft = selectable?.GetSelectable(Neighbor.Right);
				break;
			}
			((Selectable)InputField).navigation = navigation;
		}

		public ISelectable GetISelectable(Neighbor neighbor)
		{
			return this;
		}

		public Selectable GetSelectable(Neighbor neighbor)
		{
			return (Selectable)(object)InputField;
		}
	}
	public class EntryField<T> : EntryField, IValueElement<T>, IValueElement, IMenuElement, ISelectable
	{
		public new T Value
		{
			get
			{
				return (T)base.Value;
			}
			protected set
			{
				base.Value = value;
			}
		}

		public event ModifyInputHandler<T> Modify;

		public event Action<T> ValueChanged;

		public EntryField(MenuPage page, string label, MenuLabel.Style style = MenuLabel.Style.Title)
			: base(typeof(T), page, label, style)
		{
			((UnityEvent<string>)(object)base.InputField.onEndEdit).AddListener((UnityAction<string>)Validate);
			SetValue(default(T));
		}

		protected override void Validate(string input)
		{
			T newValue;
			try
			{
				InvokeModifyInputString(ref input, base.ValidatedInput);
				newValue = (T)Read(input, base.ValueType);
				Modify?.Invoke(ref newValue, Value);
			}
			catch
			{
				base.InputField.text = base.ValidatedInput;
				return;
			}
			Value = newValue;
			InputField inputField = base.InputField;
			string text = (base.ValidatedInput = Write(newValue));
			inputField.text = text;
			try
			{
				InvokeSelfChanged();
				ValueChanged?.Invoke(newValue);
			}
			catch
			{
			}
		}

		public void SetValue(T t)
		{
			Validate(Write(t));
		}

		public void AddValidateInputToTextColorEvent(Func<T, bool> test)
		{
			ValueChanged += delegate
			{
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				if (test(Value))
				{
					((Graphic)base.InputField.textComponent).color = Colors.DEFAULT_COLOR;
				}
				else
				{
					((Graphic)base.InputField.textComponent).color = Colors.INVALID_INPUT_COLOR;
				}
			};
		}
	}
	public class CustomEntryField<T> : EntryField<T>
	{
		public Func<string, T> read;

		public Func<T, string> write;

		public CustomEntryField(MenuPage page, string label, Func<string, T> read, Func<T, string> write)
			: base(page, label, MenuLabel.Style.Title)
		{
			this.read = read;
			this.write = write;
		}

		public override object Read(string input, Type type)
		{
			if (read == null)
			{
				return Read(input, base.ValueType);
			}
			return read(input);
		}

		public override string Write(object o)
		{
			if (write == null)
			{
				return base.Write(o);
			}
			return write((T)o);
		}
	}
	public class TextEntryField : EntryField<string>
	{
		public TextEntryField(MenuPage page, string label)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			base..ctor(page, label, MenuLabel.Style.Body);
			base.InputField.textComponent.alignment = (TextAnchor)1;
			LabelOffset = new Vector2(0f, 30f);
			MoveTo(Vector2.zero);
		}
	}
	public class NumericEntryField<T> : EntryField<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
	{
		private T clampMin;

		private T clampMax;

		private static readonly T typeMin = (T)typeof(T).GetField("MinValue", BindingFlags.Static | BindingFlags.Public).GetValue(null);

		private static readonly T typeMax = (T)typeof(T).GetField("MaxValue", BindingFlags.Static | BindingFlags.Public).GetValue(null);

		public NumericEntryField(MenuPage page, string label)
			: base(page, label, MenuLabel.Style.Title)
		{
			base.InputField.characterValidation = (CharacterValidation)2;
			base.InputField.characterLimit = Math.Max(typeMin.ToString().Length, typeMax.ToString().Length);
			SetClamp(typeMin, typeMax);
			base.Modify += InputClamp;
		}

		public override void Bind(object o, MemberInfo mi)
		{
			base.Bind(o, mi);
			MenuRangeAttribute customAttribute = mi.GetCustomAttribute<MenuRangeAttribute>();
			if (customAttribute != null)
			{
				SetClamp((T)customAttribute.min, (T)customAttribute.max);
			}
			Type type = o.GetType();
			foreach (DynamicBoundAttribute customAttribute2 in mi.GetCustomAttributes<DynamicBoundAttribute>())
			{
				MemberInfo info = type.GetMember(customAttribute2.memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault((MemberInfo m) => m.MemberType == MemberTypes.Field || (m.MemberType == MemberTypes.Property && ((PropertyInfo)m).CanRead) || (m.MemberType == MemberTypes.Method && ((MethodInfo)m).GetParameters().Length == 0));
				if (info != null && (info.MemberType == MemberTypes.Field || info.MemberType == MemberTypes.Property))
				{
					if (customAttribute2.upper)
					{
						base.Modify += delegate(ref T val, T orig)
						{
							T val2 = (T)info.GetValue(o);
							val = ((val.CompareTo(val2) <= 0) ? val : val2);
						};
					}
					else
					{
						base.Modify += delegate(ref T val, T orig)
						{
							T val2 = (T)info.GetValue(o);
							val = ((val.CompareTo(val2) >= 0) ? val : val2);
						};
					}
					continue;
				}
				MethodInfo method = info as MethodInfo;
				if ((object)method == null)
				{
					continue;
				}
				if (customAttribute2.upper)
				{
					base.Modify += delegate(ref T val, T orig)
					{
						T val2 = (T)method.Invoke(o, Array.Empty<object>());
						val = ((val.CompareTo(val2) <= 0) ? val : val2);
					};
				}
				else
				{
					base.Modify += delegate(ref T val, T orig)
					{
						T val2 = (T)method.Invoke(o, Array.Empty<object>());
						val = ((val.CompareTo(val2) >= 0) ? val : val2);
					};
				}
			}
		}

		private void InputClamp(ref T input, T orig)
		{
			input = Clamp(input, clampMin, clampMax);
		}

		public void SetClamp(T min, T max)
		{
			clampMin = min;
			clampMax = max;
		}

		public static T Clamp(T val, T min, T max)
		{
			if (val.CompareTo(min) < 0)
			{
				return min;
			}
			if (val.CompareTo(max) > 0)
			{
				return max;
			}
			return val;
		}
	}
	public interface ILockable
	{
		bool Locked { get; }

		void Lock();

		void Unlock();
	}
	public interface IMenuElement
	{
		MenuPage Parent { get; }

		bool Hidden { get; }

		void Hide();

		void Show();

		void Destroy();

		void MoveTo(Vector2 pos);

		void Translate(Vector2 delta);
	}
	public interface IValueElement : IMenuElement, ISelectable
	{
		object Value { get; }

		Type ValueType { get; }

		event Action<IValueElement> SelfChanged;

		void SetValue(object o);

		void Bind(object o, MemberInfo mi);
	}
	public interface IValueElement<T> : IValueElement, IMenuElement, ISelectable
	{
		new T Value { get; }

		event Action<T> ValueChanged;

		void SetValue(T t);
	}
	public static class MenuElementFactory
	{
		private static readonly Dictionary<(Type gen, Type par), Type> genericTypeLookup = new Dictionary<(Type, Type), Type>();

		public static bool TryGetValueElement(MenuPage page, MemberInfo mi, out IValueElement element)
		{
			Type valueType = mi.GetValueType();
			if (valueType == typeof(bool))
			{
				element = new ToggleButton(page, mi.GetMenuName());
				return true;
			}
			if (valueType.IsEnum)
			{
				Type typeFromHandle = typeof(MenuEnum<>);
				if (!genericTypeLookup.TryGetValue((typeFromHandle, valueType), out Type value))
				{
					Type type = (genericTypeLookup[(typeFromHandle, valueType)] = typeFromHandle.MakeGenericType(valueType));
					value = type;
				}
				element = (MenuItem)Activator.CreateInstance(value, page, mi.GetMenuName());
				return true;
			}
			if (valueType.IsNumeric())
			{
				Type typeFromHandle2 = typeof(NumericEntryField<>);
				if (!genericTypeLookup.TryGetValue((typeFromHandle2, valueType), out Type value2))
				{
					Type type = (genericTypeLookup[(typeFromHandle2, valueType)] = typeFromHandle2.MakeGenericType(valueType));
					value2 = type;
				}
				element = (EntryField)Activator.CreateInstance(value2, page, mi.GetMenuName());
				return true;
			}
			element = null;
			return false;
		}
	}
	public class MenuElementFactory<T>
	{
		public readonly MenuPage Parent;

		public readonly Dictionary<string, IValueElement> ElementLookup = new Dictionary<string, IValueElement>();

		public readonly IValueElement[] Elements;

		private readonly List<MemberInfo> Members;

		public MenuElementFactory(MenuPage page, T obj)
		{
			Parent = page;
			List<IValueElement> list = new List<IValueElement>();
			Members = new List<MemberInfo>();
			foreach (MemberInfo item in from f in typeof(T).GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				where f.IsValidForMenu()
				select f)
			{
				item.GetValueType();
				if (MenuElementFactory.TryGetValueElement(page, item, out IValueElement element))
				{
					list.Add(element);
					ElementLookup.Add(item.Name, element);
					element.Bind(obj, item);
					Members.Add(item);
				}
			}
			foreach (MemberInfo member2 in Members)
			{
				foreach (string member in member2.GetCustomAttributes<TriggerValidationAttribute>().SelectMany((TriggerValidationAttribute tv) => tv.memberNames))
				{
					MemberInfo memberInfo = Members.FirstOrDefault((MemberInfo m) => m.Name == member);
					if ((object)memberInfo != null && ElementLookup.TryGetValue(member2.Name, out IValueElement value) && ElementLookup.TryGetValue(memberInfo.Name, out IValueElement ve2))
					{
						value.SelfChanged += delegate
						{
							ve2.SetValue(ve2.Value);
						};
					}
				}
			}
			Elements = list.ToArray();
		}

		public void SetMenuValues(T source)
		{
			for (int i = 0; i < Elements.Length; i++)
			{
				Elements[i].SetValue(Members[i].GetValue(source));
			}
		}
	}
	public abstract class MenuItem : SmallButton, IValueElement, IMenuElement, ISelectable
	{
		public delegate void InterceptChangeHandler(MenuItem self, ref object newValue, ref bool cancelChange);

		protected readonly FixVerticalAlign _align;

		protected MenuItemFormatter _formatter;

		public object Value { get; protected set; }

		public Type ValueType { get; }

		public IList Items { get; }

		public int Index { get; protected set; }

		public string Name { get; }

		public MenuItemFormatter Formatter
		{
			get
			{
				return _formatter;
			}
			set
			{
				_formatter = value;
				RefreshText();
			}
		}

		public event InterceptChangeHandler InterceptChanged;

		public event Action<IValueElement> SelfChanged;

		public MenuItem(Type valueType, MenuPage page, string name, IList items, MenuItemFormatter formatter)
			: base(page, name)
		{
			Name = name;
			_align = base.GameObject.GetComponentInChildren<FixVerticalAlign>(true);
			_formatter = formatter;
			ValueType = valueType;
			Items = items;
			base.Button.ClearEvents();
			base.Button.AddEvent((EventTriggerType)15, OnMenuItemClick);
		}

		protected virtual void OnMenuItemClick(BaseEventData eventData)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			PointerEventData val = (PointerEventData)(object)((eventData is PointerEventData) ? eventData : null);
			if (val != null && (int)val.button != 0)
			{
				MovePrevious();
			}
			else
			{
				MoveNext();
			}
			InvokeOnClick();
		}

		public void AddItem(object o)
		{
			if (IsTypeValid(o))
			{
				Items.Add(o);
			}
		}

		public void InsertItem(int index, object o)
		{
			if (IsTypeValid(o))
			{
				if (index <= Index)
				{
					Index++;
				}
				Items.Insert(index, o);
			}
		}

		public bool RemoveItem(object o)
		{
			int num = Items.IndexOf(o);
			if (num < 0)
			{
				return false;
			}
			Items.RemoveAt(num);
			if (Index == num)
			{
				MoveNext();
			}
			return true;
		}

		public void OverwriteCurrent(object o)
		{
			if (IsTypeValid(o))
			{
				Items[Index] = o;
				RefreshText();
			}
		}

		public void RemoveCurrent()
		{
			if (Items.Count > 0)
			{
				Items.RemoveAt(Index--);
				MoveNext();
			}
		}

		public void Bind(object obj, MemberInfo mi)
		{
			SetValue(mi.GetValue(obj));
			SelfChanged += delegate(IValueElement b)
			{
				mi.SetValue(obj, b.Value);
			};
		}

		public void MoveNext()
		{
			if (!base.Locked && Items.Count != 0)
			{
				int num = Index + 1;
				if (num >= Items.Count || num < 0)
				{
					num = 0;
				}
				SetValueInternal(Items[num]);
			}
		}

		public void MovePrevious()
		{
			if (!base.Locked && Items.Count != 0)
			{
				int num = Index - 1;
				if (num >= Items.Count || num < 0)
				{
					num = Items.Count - 1;
				}
				SetValueInternal(Items[num]);
			}
		}

		public void SetValue(object o)
		{
			if (!base.Locked && IsTypeValid(o))
			{
				SetValueInternal(o);
			}
		}

		protected virtual void SetValueInternal(object o, bool invokeEvent = true)
		{
			bool cancelChange = false;
			InvokeInterceptChange(ref o, ref cancelChange);
			if (!cancelChange)
			{
				int num = Items.IndexOf(o);
				if (0 <= num && num < Items.Count)
				{
					Index = num;
					Value = Items[num];
				}
				else
				{
					Index = -1;
					Value = o;
				}
				RefreshText();
				if (invokeEvent)
				{
					InvokeChanged();
				}
			}
		}

		protected virtual void RefreshText()
		{
			base.Text.text = Formatter.GetText(Name, Value);
			_align.AlignText();
		}

		protected virtual bool IsTypeValid(object value)
		{
			if (!ValueType.IsInstanceOfType(value))
			{
				if (!ValueType.IsValueType)
				{
					return value == null;
				}
				return false;
			}
			return true;
		}

		protected virtual void InvokeInterceptChange(ref object newValue, ref bool cancelChange)
		{
			try
			{
				InterceptChanged?.Invoke(this, ref newValue, ref cancelChange);
			}
			catch
			{
			}
		}

		protected virtual void InvokeChanged()
		{
			try
			{
				SelfChanged?.Invoke(this);
			}
			catch
			{
			}
		}
	}
	public class MenuItem<T> : MenuItem
	{
		protected new readonly List<T> Items;

		public new T Value
		{
			get
			{
				return (T)base.Value;
			}
			protected set
			{
				SetValue(value);
			}
		}

		public event Action<T> ValueChanged;

		public MenuItem(MenuPage page, string name, params T[] values)
			: this(page, name, values?.ToList() ?? new List<T>())
		{
		}

		public MenuItem(MenuPage page, string name, List<T> values)
			: this(page, name, values, (MenuItemFormatter)new DefaultMenuItemFormatter())
		{
		}

		public MenuItem(MenuPage page, string name, List<T> values, MenuItemFormatter formatter)
			: base(typeof(T), page, name, values, formatter)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			Items = values;
			base.Index = 0;
			if (Items.Count > 0)
			{
				Value = Items[0];
			}
			else
			{
				Value = default(T);
			}
			RefreshText();
		}

		public void SetValue(T obj)
		{
			if (!base.Locked)
			{
				SetValueInternal(obj);
			}
		}

		public void AddItem(T t)
		{
			Items.Add(t);
		}

		public void RemoveItem(T t)
		{
			Items.Remove(t);
			if (Value.Equals(t))
			{
				MoveNext();
			}
		}

		public void OverwriteCurrent(T t)
		{
			if (Items.Count > 0)
			{
				T value = (Items[base.Index] = t);
				Value = value;
				RefreshText();
			}
		}

		protected override void InvokeChanged()
		{
			base.InvokeChanged();
			try
			{
				ValueChanged?.Invoke(Value);
			}
			catch
			{
			}
		}
	}
	public class MenuEnum<T> : MenuItem<T> where T : struct, Enum, IConvertible
	{
		public MenuEnum(MenuPage page, string name)
			: base(page, name, ((T[])Enum.GetValues(typeof(T))).ToList(), (MenuItemFormatter)new MenuItemEnumFormatter())
		{
		}
	}
	public abstract class MenuItemFormatter
	{
		public abstract string GetText(string prefix, object value);
	}
	public class DefaultMenuItemFormatter : MenuItemFormatter
	{
		public override string GetText(string prefix, object value)
		{
			return $"{prefix}: {value}";
		}
	}
	public class ToggleButtonFormatter : MenuItemFormatter
	{
		public override string GetText(string prefix, object value)
		{
			return prefix;
		}
	}
	public class MenuItemEnumFormatter : MenuItemFormatter
	{
		private static readonly Dictionary<string, string> cachedNames = new Dictionary<string, string>();

		public override string GetText(string prefix, object value)
		{
			return prefix + ": " + GetEnumName(value);
		}

		public string GetEnumName(object value)
		{
			string text = value.ToString();
			if (cachedNames.TryGetValue(text, out string value2))
			{
				return value2;
			}
			FieldInfo field = value.GetType().GetField(text);
			if ((object)field != null)
			{
				return cachedNames[text] = field.GetMenuName();
			}
			return cachedNames[text] = text.FromCamelCase();
		}
	}
	public class MenuLabel : IMenuElement
	{
		public enum Style
		{
			Title,
			Body
		}

		public MenuPage Parent { get; }

		public GameObject GameObject { get; }

		public CanvasGroup CanvasGroup { get; }

		public Text Text { get; }

		public bool Hidden { get; private set; }

		public MenuLabel(MenuPage page, string text, Style style = Style.Title)
		{
			Parent = page;
			if (style == Style.Title || style != Style.Body)
			{
				(GameObject, Text, CanvasGroup) = PrefabMenuObjects.BuildLabel(Parent, text);
			}
			else
			{
				(GameObject, Text, CanvasGroup) = PrefabMenuObjects.BuildDescText(Parent, text);
			}
		}

		public MenuLabel(MenuPage page, string text, Vector2 dimensions)
		{
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			base..ctor();
			Parent = page;
			(GameObject, Text, CanvasGroup) = PrefabMenuObjects.BuildDescText(Parent, text);
			Object.DestroyImmediate((Object)(object)GameObject.GetComponent<ContentSizeFitter>());
			Object.DestroyImmediate((Object)(object)GameObject.GetComponent<AutoLocalizeTextUI>());
			Object.DestroyImmediate((Object)(object)GameObject.GetComponent<FixVerticalAlign>());
			RectTransform rectTransform = ((Graphic)Text).rectTransform;
			RectTransform rectTransform2 = ((Graphic)Text).rectTransform;
			Vector2 val = default(Vector2);
			((Vector2)(ref val))..ctor(0.5f, 0.5f);
			rectTransform2.anchorMax = val;
			rectTransform.anchorMin = val;
			((Graphic)Text).rectTransform.sizeDelta = dimensions;
		}

		public virtual void MoveTo(Vector2 pos)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			GameObject.transform.localPosition = Vector2.op_Implicit(pos);
		}

		public virtual void Translate(Vector2 delta)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = GameObject.transform;
			transform.localPosition += Vector2.op_Implicit(delta);
		}

		public virtual void Show()
		{
			Hidden = false;
			GameObject.SetActive(true);
		}

		public virtual void Hide()
		{
			Hidden = true;
			GameObject.SetActive(false);
		}

		public virtual void Destroy()
		{
			Object.Destroy((Object)(object)GameObject);
		}

		public void MakeTransparent()
		{
			CanvasGroup.alpha = 0f;
		}

		public void MakeOpaque()
		{
			CanvasGroup.alpha = 1f;
		}

		public void SetVisibleByAlpha(bool visible)
		{
			if (visible)
			{
				MakeOpaque();
			}
			else
			{
				MakeTransparent();
			}
		}
	}
	public interface IMenuPreset
	{
		MenuLabel Label { get; }

		void UpdatePreset();
	}
	public class MenuPreset<T> : MenuItem<string>, IMenuPreset
	{
		public readonly Dictionary<string, T> Ts;

		public readonly MemberInfo[] TMembers;

		public readonly T Obj;

		public readonly Func<T, string> Caption;

		private bool isUpdating;

		private bool broadcastPreset;

		public MenuLabel Label { get; }

		public event Action<T> OnSetPreset;

		public MenuPreset(MenuPage page, string prefix, Dictionary<string, T> dict, T obj, Func<T, string> caption)
		{
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			broadcastPreset = true;
			base..ctor(page, prefix, dict.Keys.ToArray());
			Ts = dict;
			TMembers = (from mi in typeof(T).GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				where mi.IsValidForMenu()
				select mi).ToArray();
			Obj = obj;
			base.SelfChanged += SetPreset;
			Caption = caption;
			Label = new MenuLabel(page, caption(Ts[base.Value]), MenuLabel.Style.Body);
			Label.GameObject.transform.SetParent(base.GameObject.transform);
			Label.GameObject.transform.localPosition = new Vector3(0f, -25f, 0f);
			Transform transform = Label.GameObject.transform;
			transform.localScale *= 0.7f;
			Label.Text.alignment = (TextAnchor)4;
			base.SelfChanged += delegate
			{
				UpdateCaption();
			};
			UpdatePreset();
		}

		public MenuPreset(MenuPage page, string prefix, Dictionary<string, T> dict, T obj, Func<T, string> caption, MenuElementFactory<T> factory)
			: this(page, prefix, dict, obj, caption)
		{
			Pair(factory);
		}

		public void UpdateCaption()
		{
			Label.Text.text = ((Caption != null) ? Caption(Obj) : string.Empty);
		}

		public void SetValueWithoutBroadcast(string key)
		{
			broadcastPreset = false;
			SetValue(key);
			broadcastPreset = true;
		}

		protected override void SetValueInternal(object o, bool invokeEvent = true)
		{
			isUpdating = true;
			base.SetValueInternal(o, invokeEvent);
			isUpdating = false;
		}

		public void SetPreset(IValueElement self)
		{
			SetPreset((string)self.Value);
		}

		public void SetPreset(string key)
		{
			if (broadcastPreset && Ts.TryGetValue(key, out var value))
			{
				MemberInfo[] tMembers = TMembers;
				foreach (MemberInfo mi in tMembers)
				{
					mi.SetValue(Obj, mi.GetValue(value));
				}
				OnSetPreset?.Invoke(value);
			}
		}

		public void Pair(IValueElement element, MemberInfo mi)
		{
			element.SelfChanged += delegate
			{
				UpdatePreset();
			};
			OnSetPreset += delegate(T t)
			{
				element.SetValue(mi.GetValue(t));
			};
		}

		public void Pair(RadioSwitch radioSwitch, MemberInfo mi)
		{
			radioSwitch.Changed += delegate
			{
				UpdatePreset();
			};
			OnSetPreset += delegate(T t)
			{
				radioSwitch.TrySelect((string)mi.GetValue(t));
			};
		}

		public void Pair(MenuElementFactory<T> factory)
		{
			MemberInfo[] tMembers = TMembers;
			foreach (MemberInfo memberInfo in tMembers)
			{
				if (factory.ElementLookup.TryGetValue(memberInfo.Name, out IValueElement value))
				{
					Pair(value, memberInfo);
				}
			}
		}

		public void UpdatePreset(string s)
		{
			UpdatePreset();
		}

		public void UpdatePreset()
		{
			if (isUpdating)
			{
				return;
			}
			foreach (string item in Items)
			{
				if (CheckPreset(item))
				{
					if (base.Value != item)
					{
						SetValueWithoutBroadcast(item);
					}
					return;
				}
			}
			SetValueWithoutBroadcast("Custom");
		}

		public bool CheckPreset(string key)
		{
			if (Ts.TryGetValue(key, out var t))
			{
				return TMembers.All((MemberInfo mi) => object.Equals(mi.GetValue(t), mi.GetValue(Obj)));
			}
			return false;
		}
	}
	public class RadioSwitch
	{
		public enum ToggleGroupMode
		{
			RadioSwitch,
			MultiSelect
		}

		public delegate void RadioSwitchChangedEvent(string selection);

		public readonly MenuPage Parent;

		public readonly ToggleButton[] Elements;

		public ToggleButton CurrentButton { get; protected set; }

		public bool Active { get; protected set; } = true;

		public string CurrentSelection => CurrentButton?.Name;

		public event RadioSwitchChangedEvent Changed;

		public RadioSwitch(MenuPage page, params string[] ts)
		{
			if (ts == null || ts.Length == 0)
			{
				throw new ArgumentException();
			}
			Parent = page;
			Elements = ts.Select((string t) => new ToggleButton(page, t)).ToArray();
			SelectFirst();
			ToggleButton[] elements = Elements;
			for (int num = 0; num < elements.Length; num++)
			{
				elements[num].InterceptChanged += OnClick;
			}
		}

		public void OnClick(MenuItem button, ref object value, ref bool cancelChange)
		{
			if (!Active)
			{
				return;
			}
			if (CurrentButton == button)
			{
				cancelChange = true;
				return;
			}
			bool flag = (bool)button.Value;
			bool flag2 = (bool)value;
			if (!flag && !((flag == flag2) | cancelChange))
			{
				ToggleButton currentButton = CurrentButton;
				CurrentButton = button as ToggleButton;
				currentButton?.SetValue(obj: false);
				InvokeChanged();
			}
		}

		public void ChangeSelection(ToggleButton newButton)
		{
			if (CurrentButton != null)
			{
				ToggleButton currentButton = CurrentButton;
				CurrentButton = null;
				currentButton.SetValue(obj: false);
			}
			newButton.SetValue(obj: true);
			CurrentButton = newButton;
			InvokeChanged();
		}

		public void SelectFirst()
		{
			Active = true;
			ChangeSelection(Elements[0]);
		}

		public bool TrySelect(string name)
		{
			ToggleButton toggleButton = Elements.FirstOrDefault((ToggleButton b) => b.Name == name);
			if (toggleButton != null && !toggleButton.Locked)
			{
				ToggleButton currentButton = CurrentButton;
				if (currentButton == null || !currentButton.Locked)
				{
					Active = true;
					ChangeSelection(toggleButton);
					return true;
				}
			}
			return false;
		}

		public void MatchPredicateAndLock(Func<ToggleButton, bool> predicate)
		{
			Active = false;
			ToggleButton[] elements = Elements;
			foreach (ToggleButton toggleButton in elements)
			{
				toggleButton.Unlock();
				toggleButton.SetValue(predicate(toggleButton));
				toggleButton.Lock();
			}
		}

		public void DeselectAll(Func<ToggleButton, bool> lockPredicate = null)
		{
			Active = false;
			ToggleButton[] elements = Elements;
			foreach (ToggleButton toggleButton in elements)
			{
				toggleButton.Unlock();
				if (toggleButton.Value)
				{
					toggleButton.SetValue(obj: false);
				}
				if (lockPredicate != null && lockPredicate(toggleButton) && lockPredicate(toggleButton))
				{
					toggleButton.Lock();
				}
			}
		}

		public void DeselectCurrent()
		{
			CurrentButton?.SetValue(obj: false);
			CurrentButton = null;
		}

		public void SetActive(bool value)
		{
			Active = value;
		}

		protected void InvokeChanged()
		{
			Changed?.Invoke(CurrentSelection);
		}
	}
	public class SmallButton : BaseButton, ILockable
	{
		public Text Text { get; }

		public bool Locked { get; protected set; }

		public SmallButton(MenuPage page, string text)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			base..ctor(page, PrefabMenuObjects.BuildNewButton(text));
			base.GameObject.transform.localScale = Vector2.op_Implicit(new Vector2(0.7f, 0.7f));
			Text = ((Component)((Component)base.Button).transform.Find("Text")).GetComponent<Text>();
			Text.fontSize = 36;
		}

		protected override void InvokeOnClick()
		{
			if (!Locked)
			{
				base.InvokeOnClick();
			}
		}

		public virtual void Lock()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			Locked = true;
			((Graphic)Text).color = Colors.LOCKED_FALSE_COLOR;
		}

		public virtual void Unlock()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			Locked = false;
			((Graphic)Text).color = Colors.DEFAULT_COLOR;
		}

		public override string ToString()
		{
			return "Small Button { \"" + Text.text + "\" }";
		}
	}
	public class TimerLabel : MenuLabel
	{
		private readonly Stopwatch _stopwatch;

		private string _prefix;

		private string _restLabel;

		private IEnumerator maintainTime;

		private readonly StringBuilder _sb = new StringBuilder();

		public TimeSpan Elapsed => _stopwatch.Elapsed;

		public TimerLabel(MenuPage page, string prefix, string restLabel = null)
			: base(page, string.Empty, Style.Body)
		{
			SetPrefix(prefix);
			_restLabel = restLabel ?? string.Empty;
			_stopwatch = new Stopwatch();
			base.Text.alignment = (TextAnchor)1;
		}

		public void Start()
		{
			_stopwatch.Reset();
			_stopwatch.Start();
			maintainTime = MaintainTime();
			((MonoBehaviour)base.Text).StartCoroutine(maintainTime);
		}

		private void StopInternal()
		{
			if (maintainTime != null)
			{
				((MonoBehaviour)base.Text).StopCoroutine(maintainTime);
				maintainTime = null;
			}
		}

		public void Stop(string stopLabel = null)
		{
			StopInternal();
			if (stopLabel != null)
			{
				base.Text.text = stopLabel;
			}
		}

		public void Reset(string newRestLabel = null)
		{
			StopInternal();
			if (newRestLabel != null)
			{
				_restLabel = newRestLabel;
			}
			base.Text.text = _restLabel;
		}

		public void SetPrefix(string newPrefix)
		{
			_prefix = newPrefix + ": ";
		}

		public string Split(string newPrefix)
		{
			StopInternal();
			string result = ComputeTimeText();
			SetPrefix(newPrefix);
			Start();
			return result;
		}

		private IEnumerator MaintainTime()
		{
			while (true)
			{
				yield return (object)new WaitForSecondsRealtime(0.01f);
				base.Text.text = ComputeLabelText();
			}
		}

		private string ComputeTimeText()
		{
			_sb.Clear();
			int hours = _stopwatch.Elapsed.Hours;
			int minutes = _stopwatch.Elapsed.Minutes;
			int seconds = _stopwatch.Elapsed.Seconds;
			int milliseconds = _stopwatch.Elapsed.Milliseconds;
			bool flag = hours > 0;
			bool flag2 = flag || minutes > 0;
			bool flag3 = true;
			if (flag)
			{
				_sb.Append(hours);
				_sb.Append(':');
			}
			if (flag2)
			{
				if (flag)
				{
					_sb.Append(minutes.ToString().PadLeft(2, '0'));
				}
				else
				{
					_sb.Append(minutes);
				}
				_sb.Append(':');
			}
			if (flag3)
			{
				if (flag2)
				{
					_sb.Append(seconds.ToString().PadLeft(2, '0'));
				}
				else
				{
					_sb.Append(seconds);
				}
			}
			_sb.Append('.');
			_sb.Append(milliseconds.ToString().PadLeft(3, '0'));
			return _sb.ToString();
		}

		private string ComputeLabelText()
		{
			_sb.Clear();
			_sb.Append(_prefix);
			int hours = _stopwatch.Elapsed.Hours;
			int minutes = _stopwatch.Elapsed.Minutes;
			int seconds = _stopwatch.Elapsed.Seconds;
			int milliseconds = _stopwatch.Elapsed.Milliseconds;
			bool flag = hours > 0;
			bool flag2 = flag || minutes > 0;
			bool flag3 = true;
			if (flag)
			{
				_sb.Append(hours);
				_sb.Append(':');
			}
			if (flag2)
			{
				if (flag)
				{
					_sb.Append(minutes.ToString().PadLeft(2, '0'));
				}
				else
				{
					_sb.Append(minutes);
				}
				_sb.Append(':');
			}
			if (flag3)
			{
				if (flag2)
				{
					_sb.Append(seconds.ToString().PadLeft(2, '0'));
				}
				else
				{
					_sb.Append(seconds);
				}
			}
			_sb.Append('.');
			_sb.Append(milliseconds.ToString().PadLeft(3, '0'));
			return _sb.ToString();
		}
	}
	public class ToggleButton : MenuItem<bool>
	{
		private static List<bool> GetBoolList()
		{
			return new List<bool> { false, true };
		}

		public ToggleButton(MenuPage page, string text)
			: base(page, text, GetBoolList(), (MenuItemFormatter)new ToggleButtonFormatter())
		{
		}

		protected override void RefreshText()
		{
			SetColor();
			base.RefreshText();
		}

		public void SetColor()
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			if (base.Value)
			{
				if (!base.Locked)
				{
					((Graphic)base.Text).color = Colors.TRUE_COLOR;
				}
				else if (base.Locked)
				{
					((Graphic)base.Text).color = Colors.LOCKED_TRUE_COLOR;
				}
			}
			else if (base.Locked)
			{
				((Graphic)base.Text).color = Colors.LOCKED_FALSE_COLOR;
			}
			else if (!base.Locked)
			{
				((Graphic)base.Text).color = Colors.FALSE_COLOR;
			}
		}

		public void SetColor(Color c)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((Graphic)base.Text).color = c;
		}

		public override void Lock()
		{
			base.Locked = true;
			SetColor();
		}

		public override void Unlock()
		{
			base.Locked = false;
			SetColor();
		}
	}
}
namespace MenuChanger.Extensions
{
	public static class MemberInfoExtensions
	{
		public static bool IsValidForMenu(this MemberInfo mi)
		{
			if (mi.MemberType == MemberTypes.Field)
			{
				FieldInfo fieldInfo = (FieldInfo)mi;
				if (fieldInfo.IsInitOnly)
				{
					return false;
				}
				if ((fieldInfo.IsPublic && !Attribute.IsDefined(fieldInfo, typeof(MenuIgnoreAttribute))) || Attribute.IsDefined(fieldInfo, typeof(MenuIncludeAttribute)))
				{
					return true;
				}
				return false;
			}
			if (mi.MemberType == MemberTypes.Property)
			{
				PropertyInfo propertyInfo = (PropertyInfo)mi;
				if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
				{
					return false;
				}
				if ((propertyInfo.GetMethod.IsPublic && !Attribute.IsDefined(propertyInfo, typeof(MenuIgnoreAttribute))) || Attribute.IsDefined(propertyInfo, typeof(MenuIncludeAttribute)))
				{
					return true;
				}
				return false;
			}
			return false;
		}

		public static string GetMenuName(this MemberInfo mi)
		{
			if (Attribute.GetCustomAttribute(mi, typeof(MenuLabelAttribute)) is MenuLabelAttribute menuLabelAttribute)
			{
				return menuLabelAttribute.text;
			}
			return mi.Name.FromCamelCase();
		}

		public static object GetValue(this MemberInfo mi, object o)
		{
			return mi.MemberType switch
			{
				MemberTypes.Field => ((FieldInfo)mi).GetValue(o), 
				MemberTypes.Property => ((PropertyInfo)mi).GetValue(o), 
				_ => throw new NotImplementedException(), 
			};
		}

		public static void SetValue(this MemberInfo mi, object o, object value)
		{
			switch (mi.MemberType)
			{
			case MemberTypes.Field:
				((FieldInfo)mi).SetValue(o, value);
				break;
			case MemberTypes.Property:
				((PropertyInfo)mi).SetValue(o, value);
				break;
			default:
				throw new NotImplementedException();
			}
		}

		public static Type GetValueType(this MemberInfo mi)
		{
			return mi.MemberType switch
			{
				MemberTypes.Field => ((FieldInfo)mi).FieldType, 
				MemberTypes.Property => ((PropertyInfo)mi).PropertyType, 
				_ => throw new NotImplementedException(), 
			};
		}

		public static bool IsNumeric(this Type T)
		{
			if (T.IsEnum)
			{
				return false;
			}
			TypeCode typeCode = Type.GetTypeCode(T);
			if ((uint)(typeCode - 5) <= 10u)
			{
				return true;
			}
			return false;
		}
	}
	internal static class MenuButtonExtensions
	{
		public static void ClearEvents(this MenuButton self)
		{
			((Component)self).gameObject.GetComponent<EventTrigger>().triggers.Clear();
		}

		public static void AddEvent(this MenuButton self, Action a)
		{
			self.AddEvent((EventTriggerType)15, delegate
			{
				a();
			});
		}

		public static void AddEvent(this MenuButton self, EventTriggerType type, UnityAction<BaseEventData> func)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Invalid comparison between Unknown and I4
			Entry val = new Entry
			{
				eventID = type
			};
			((UnityEvent<BaseEventData>)(object)val.callback).AddListener(func);
			EventTrigger val2 = ((Component)self).gameObject.GetComponent<EventTrigger>();
			if ((Object)(object)val2 == (Object)null)
			{
				val2 = ((Component)self).gameObject.AddComponent<EventTrigger>();
			}
			val2.triggers.Add(val);
			if ((int)type == 15)
			{
				self.AddEvent((EventTriggerType)4, func);
			}
		}
	}
	public static class MenuElementExtensions
	{
		public static void AddHideMenuPageEvent(this BaseButton self)
		{
			self.OnClick += self.Parent.Hide;
		}

		public static void AddHideMenuPageEvent(this BaseButton self, MenuPage prev)
		{
			self.OnClick += prev.Hide;
		}

		public static void AddHideAllMenuPagesEvent(this BaseButton self)
		{
			self.OnClick += MenuChangerMod.HideAllMenuPages;
		}

		public static void AddShowMenuPageEvent(this BaseButton self, MenuPage next)
		{
			self.OnClick += next.Show;
		}

		public static void AddHideAndShowEvent(this BaseButton self, MenuPage next)
		{
			self.OnClick += self.Parent.Hide;
			self.OnClick += next.Show;
		}

		public static void AddHideAndShowEvent(this BaseButton self, MenuPage prev, MenuPage next)
		{
			self.OnClick += prev.Hide;
			self.OnClick += next.Show;
		}

		public static void AddSetResumeKeyEvent(this BaseButton self, string key)
		{
			self.OnClick += delegate
			{
				MenuChangerMod.instance.Settings.resumeKey = key;
			};
		}

		public static void SymSetNeighbor(this ISelectable self, Neighbor neighbor, ISelectable selectable)
		{
			self.SetNeighbor(neighbor, selectable);
			neighbor = neighbor switch
			{
				Neighbor.Down => Neighbor.Up, 
				Neighbor.Up => Neighbor.Down, 
				Neighbor.Left => Neighbor.Right, 
				Neighbor.Right => Neighbor.Left, 
				_ => neighbor, 
			};
			selectable.SetNeighbor(neighbor, self);
		}
	}
	public static class SaveGameDataExtensions
	{
		public class FakeModSavegameData
		{
			public Dictionary<string, string> loadedMods;

			public Dictionary<string, JToken> modData = new Dictionary<string, JToken>();
		}

		public static T ManuallyLoadSettings<U, T>(this U u, int saveSlot) where U : Mod, ILocalSettings<T>
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Expected O, but got Unknown
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Expected O, but got Unknown
			FakeModSavegameData fakeModSavegameData;
			try
			{
				string path = typeof(GameManager).GetMethod("ModdedSavePath", BindingFlags.Static | BindingFlags.NonPublic).Invoke(GameManager.instance, new object[1] { saveSlot }) as string;
				if (File.Exists(path))
				{
					using FileStream stream = File.OpenRead(path);
					using StreamReader streamReader = new StreamReader(stream);
					string text = streamReader.ReadToEnd();
					fakeModSavegameData = JsonConvert.DeserializeObject<FakeModSavegameData>(text, new JsonSerializerSettings
					{
						ContractResolver = (IContractResolver)(object)ShouldSerializeContractResolver.Instance,
						TypeNameHandling = (TypeNameHandling)4,
						ObjectCreationHandling = (ObjectCreationHandling)2,
						Converters = JsonConverterTypes.ConverterTypes
					});
					if (fakeModSavegameData == null)
					{
						LogHelper.LogError("Loaded mod savegame data deserialized to null: " + text);
						fakeModSavegameData = new FakeModSavegameData();
					}
				}
				else
				{
					fakeModSavegameData = new FakeModSavegameData();
				}
			}
			catch (Exception ex)
			{
				LogHelper.LogError(ex.ToString());
				fakeModSavegameData = new FakeModSavegameData();
			}
			try
			{
				if (fakeModSavegameData.modData.TryGetValue(((Mod)u).GetName(), out JToken value))
				{
					return value.ToObject<T>(JsonSerializer.Create(new JsonSerializerSettings
					{
						ContractResolver = (IContractResolver)(object)ShouldSerializeContractResolver.Instance,
						TypeNameHandling = (TypeNameHandling)4,
						ObjectCreationHandling = (ObjectCreationHandling)2,
						Converters = JsonConverterTypes.ConverterTypes
					}));
				}
			}
			catch (Exception ex2)
			{
				LogHelper.LogError(ex2.ToString());
			}
			return default(T);
		}
	}
	internal static class StringExtensions
	{
		public static string FromCamelCase(this string str)
		{
			StringBuilder stringBuilder = new StringBuilder(str);
			if (str.Length > 0)
			{
				stringBuilder[0] = char.ToUpper(stringBuilder[0]);
			}
			for (int i = 1; i < stringBuilder.Length; i++)
			{
				if (char.IsUpper(stringBuilder[i]))
				{
					if (char.IsLower(stringBuilder[i - 1]))
					{
						stringBuilder.Insert(i++, " ");
					}
					else if (i + 1 < stringBuilder.Length && char.IsLower(stringBuilder[i + 1]))
					{
						stringBuilder.Insert(i++, " ");
					}
				}
				if (char.IsDigit(stringBuilder[i]) && !char.IsDigit(stringBuilder[i - 1]) && !char.IsWhiteSpace(stringBuilder[i - 1]))
				{
					stringBuilder.Insert(i, " ");
				}
			}
			return stringBuilder.ToString();
		}
	}
}
namespace MenuChanger.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public class DynamicBoundAttribute : Attribute
	{
		public readonly string memberName;

		public readonly bool upper;

		public DynamicBoundAttribute(string memberName, bool upper)
		{
			this.memberName = memberName;
			this.upper = upper;
		}
	}
	public class MenuIgnoreAttribute : Attribute
	{
	}
	public class MenuIncludeAttribute : Attribute
	{
	}
	public class MenuLabelAttribute : Attribute
	{
		public readonly string text;

		public MenuLabelAttribute(string text)
		{
			this.text = text;
		}
	}
	public class MenuRangeAttribute : Attribute
	{
		public readonly object min;

		public readonly object max;

		public MenuRangeAttribute(object min, object max)
		{
			this.min = min;
			this.max = max;
		}
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public class TriggerValidationAttribute : Attribute
	{
		public string[] memberNames;

		public TriggerValidationAttribute(string memberName)
		{
			memberNames = new string[1] { memberName };
		}

		public TriggerValidationAttribute(params string[] memberNames)
		{
			this.memberNames = memberNames;
		}
	}
}
