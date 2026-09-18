using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using GlobalEnums;
using HutongGames.PlayMaker;
using IL;
using InControl;
using JetBrains.Annotations;
using Language;
using Microsoft.CodeAnalysis;
using Modding;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.ModInterop;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using Newtonsoft.Json;
using On;
using On.HutongGames.PlayMaker;
using Osmi.ChildRefs;
using Osmi.Exports;
using Osmi.FsmActions;
using Osmi.Game;
using Osmi.Imports;
using Osmi.SimpleFSM;
using Osmi.Utils;
using Osmi.Utils.Tap;
using UnityEngine;
using UnityEngine.SceneManagement;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: AssemblyCompany("Clazex")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyCopyright("Copyright © 2022-2023 Clazex")]
[assembly: AssemblyDescription("A Hollow Knight library mod")]
[assembly: AssemblyFileVersion("0.2.2.0")]
[assembly: AssemblyInformationalVersion("0.2.2.0汉化")]
[assembly: AssemblyProduct("Osmi")]
[assembly: AssemblyTitle("Osmi")]
[assembly: AssemblyMetadata("RepositoryUrl", "https://github.com/Clazex/HollowKnight.Osmi")]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[assembly: AssemblyVersion("0.2.2.0")]
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
namespace Osmi
{
	[PublicAPI]
	public class Dict
	{
		private readonly Dictionary<string, Lazy<Dictionary<string, string>>> dict;

		public Dict(Assembly asm, string path)
		{
			string prefix = asm.GetName().Name + "." + path + ".";
			dict = LangUtil.LoadLangs(asm, asm.GetManifestResourceNames().Filter(delegate(string name)
			{
				if (!name.StartsWith(prefix))
				{
					return false;
				}
				string[] array = name.StripStart(prefix).Split('.');
				return array.Length == 2 && array[1] == "json" && LangUtil.Langs.Contains(array[0]);
			}).Map((string name) => (name.StripStart(prefix).StripEnd(".json"), name: name)));
		}

		public string Localize(string key)
		{
			try
			{
				return dict[LangUtil.CurrentLang].Value[key];
			}
			catch (KeyNotFoundException)
			{
				string value;
				return dict[((LanguageCode)44).ToIdentifier()].Value.TryGetValue(key, out value) ? value : key;
			}
		}
	}
	internal static class Logger
	{
		internal static readonly SimpleLogger logger;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void Log(string message)
		{
			((Loggable)logger).Log(message);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void LogDebug(string message)
		{
			((Loggable)logger).LogDebug(message);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void LogError(string message)
		{
			((Loggable)logger).LogError(message);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void LogFine(string message)
		{
			((Loggable)logger).LogFine(message);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void LogWarn(string message)
		{
			((Loggable)logger).LogWarn(message);
		}

		static Logger()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Expected O, but got Unknown
			logger = new SimpleLogger("Osmi");
		}
	}
	[PublicAPI]
	public sealed class Osmi : Mod
	{
		private static readonly Lazy<string> version;

		public static Osmi? Instance { get; private set; }

		public static Osmi UnsafeInstance => Instance;

		public override string GetVersion()
		{
			return version.Value;
		}

		public Osmi()
			: base("Osmi（模组框架）")
		{
			Instance = this;
		}

		public override void Initialize()
		{
			ModInteropExports.Init();
		}

		static Osmi()
		{
			version = AssemblyUtil.GetMyDefaultVersion();
			GameObjectUtil.Init();
			OsmiHooks.SceneChangeHook += SceneEdit.ApplyHandles;
			ModHooks.FinishedLoadingModsHook += delegate
			{
				GlobalCoroutineExecutor.Start(OsmiHooks.CheckGameInitialized());
			};
		}
	}
	[PublicAPI]
	public static class OsmiHooks
	{
		public delegate bool HitEnemyHandler(HealthManager hm, HitInstance hit, bool invincible);

		[CompilerGenerated]
		private static class <>O
		{
			public static EnterSceneEvent <0>__OnAfterEnterSave;
		}

		private static bool gameInitializedHookCalled;

		public static event Action GameInitializedHook
		{
			add
			{
				if (gameInitializedHookCalled)
				{
					value();
				}
				else
				{
					InternalGameInitializedHook += value;
				}
			}
			remove
			{
				if (!gameInitializedHookCalled)
				{
					InternalGameInitializedHook -= value;
				}
			}
		}

		private static event Action InternalGameInitializedHook;

		public static event Action MenuBuildHook;

		public static event Action AfterEnterSaveHook;

		public static event Action<Scene, Scene> SceneChangeHook;

		public static event HitEnemyHandler HitEnemyHook;

		public static event Action GamePauseHook;

		public static event Action GameUnpauseHook;

		public static event Action<GameObject> PlayerDeathHook;

		internal static IEnumerator CheckGameInitialized()
		{
			yield return (object)new WaitUntil((Func<bool>)(() => (Object)(object)GameObject.Find("LogoTitle") != (Object)null));
			gameInitializedHookCalled = true;
			if (InternalGameInitializedHook == null)
			{
				yield break;
			}
			Delegate[] invocationList = InternalGameInitializedHook.GetInvocationList();
			for (int num = 0; num < invocationList.Length; num++)
			{
				Action action = (Action)invocationList[num];
				try
				{
					action();
				}
				catch (Exception ex)
				{
					Logger.LogError(ex.ToString());
				}
			}
			InternalGameInitializedHook = null;
		}

		private static void OnMenuBuild()
		{
			Logger.LogFine("OnMenuBuild Invoked");
			if (MenuBuildHook == null)
			{
				return;
			}
			Delegate[] invocationList = MenuBuildHook.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				Action action = (Action)invocationList[i];
				try
				{
					action();
				}
				catch (Exception ex)
				{
					Logger.LogError(ex.ToString());
				}
			}
		}

		private static void OnAfterEnterSave()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			GameManager gM = Ref.GM;
			object obj = <>O.<0>__OnAfterEnterSave;
			if (obj == null)
			{
				EnterSceneEvent val = OnAfterEnterSave;
				<>O.<0>__OnAfterEnterSave = val;
				obj = (object)val;
			}
			gM.OnFinishedEnteringScene -= (EnterSceneEvent)obj;
			Logger.LogFine("OnAfterEnterSave Invoked");
			if (AfterEnterSaveHook == null)
			{
				return;
			}
			Delegate[] invocationList = AfterEnterSaveHook.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				Action action = (Action)invocationList[i];
				try
				{
					action();
				}
				catch (Exception ex)
				{
					Logger.LogError(ex.ToString());
				}
			}
		}

		private static void OnSceneChange(Scene prev, Scene next)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			Logger.LogFine("OnSceneChange Invoked");
			if (SceneChangeHook == null)
			{
				return;
			}
			Delegate[] invocationList = SceneChangeHook.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				Action<Scene, Scene> action = (Action<Scene, Scene>)invocationList[i];
				try
				{
					action(prev, next);
				}
				catch (Exception ex)
				{
					Logger.LogError(ex.ToString());
				}
			}
		}

		private static bool OnHitEnemy(bool invincible, HealthManager hm, HitInstance hitInstance)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			Logger.LogFine("OnHitEnemy Invoked");
			if (HitEnemyHook == null)
			{
				return invincible;
			}
			Delegate[] invocationList = HitEnemyHook.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				HitEnemyHandler hitEnemyHandler = (HitEnemyHandler)invocationList[i];
				try
				{
					invincible = hitEnemyHandler(hm, hitInstance, invincible);
				}
				catch (Exception ex)
				{
					Logger.LogError(ex.ToString());
				}
			}
			return invincible;
		}

		private static void OnGamePause()
		{
			Logger.LogFine("OnGamePause Invoked");
			if (GamePauseHook == null)
			{
				return;
			}
			Delegate[] invocationList = GamePauseHook.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				Action action = (Action)invocationList[i];
				try
				{
					action();
				}
				catch (Exception ex)
				{
					Logger.LogError(ex.ToString());
				}
			}
		}

		private static void OnGameUnpause()
		{
			Logger.LogFine("OnGameUnpause Invoked");
			if (GameUnpauseHook == null)
			{
				return;
			}
			Delegate[] invocationList = GameUnpauseHook.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				Action action = (Action)invocationList[i];
				try
				{
					action();
				}
				catch (Exception ex)
				{
					Logger.LogError(ex.ToString());
				}
			}
		}

		private static void OnPlayerDeath(CustomBehaviour behaviour)
		{
			Logger.LogFine("OnPlayerDeath Invoked");
			if (PlayerDeathHook == null)
			{
				return;
			}
			Delegate[] invocationList = PlayerDeathHook.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				Action<GameObject> action = (Action<GameObject>)invocationList[i];
				try
				{
					action(((Component)behaviour).gameObject);
				}
				catch (Exception ex)
				{
					Logger.LogError(ex.ToString());
				}
			}
		}

		static OsmiHooks()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected O, but got Unknown
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Expected O, but got Unknown
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Expected O, but got Unknown
			GameInitializedHook += delegate
			{
				UIManager.EditMenus += OnMenuBuild;
			};
			HeroController.Start += (hook_Start)delegate(orig_Start orig, HeroController self)
			{
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Expected O, but got Unknown
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Expected O, but got Unknown
				orig.Invoke(self);
				Ref.GM.OnFinishedEnteringScene -= new EnterSceneEvent(OnAfterEnterSave);
				Ref.GM.OnFinishedEnteringScene += new EnterSceneEvent(OnAfterEnterSave);
			};
			SceneManager.activeSceneChanged += OnSceneChange;
			HealthManager.Hit += (Manipulator)delegate(ILContext il)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				new ILCursor(il).Goto(0, (MoveType)0, false).GotoNext(new Func<Instruction, bool>[1]
				{
					(Instruction i) => ILPatternMatchingExt.MatchCallvirt<HealthManager>(i, "IsBlockingByDirection")
				}).Emit(OpCodes.Ldarg_0)
					.Emit(OpCodes.Ldarg_1)
					.Emit(OpCodes.Call, (MethodBase)(MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/));
			};
			new ILHook((MethodBase)Extensions.GetStateMachineTarget((MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (Manipulator)delegate(ILContext il)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				new ILCursor(il).Goto(0, (MoveType)0, false).GotoNext((MoveType)0, new Func<Instruction, bool>[2]
				{
					(Instruction i) => ILPatternMatchingExt.MatchLdcR4(i, 0f),
					(Instruction i) => ILPatternMatchingExt.MatchCallvirt(i, typeof(GameManager), "SetTimeScale")
				}).Emit(OpCodes.Call, (MethodBase)(MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/))
					.GotoNext((MoveType)0, new Func<Instruction, bool>[2]
					{
						(Instruction i) => ILPatternMatchingExt.MatchLdcR4(i, 1f),
						(Instruction i) => ILPatternMatchingExt.MatchCallvirt(i, typeof(GameManager), "SetTimeScale")
					})
					.Emit(OpCodes.Call, (MethodBase)(MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/));
			});
			HeroController.Start += (hook_Start)delegate(orig_Start orig, HeroController self)
			{
				orig.Invoke(self);
				self.heroDeathPrefab.AddComponent<CustomBehaviour>().OnEnableEvent += OnPlayerDeath;
			};
		}
	}
	[PublicAPI]
	public static class Ref
	{
		public static GameManager GM => GameManager.instance;

		public static HeroController HC => HeroController.instance;

		public static PlayerData PD => PlayerData.instance;

		public static GameCameras GC => GameCameras.instance;

		public static UIManager UM => UIManager.instance;

		public static ObjectPool OP => ObjectPool.instance;

		public static InputHandler IH => InputHandler.Instance;

		public static SceneData SD => SceneData.instance;

		public static Scene DDOL
		{
			get
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				return ((Component)GM).gameObject.scene;
			}
		}
	}
}
namespace Osmi.Utils
{
	[PublicAPI]
	public static class AssemblyUtil
	{
		public static string GetVersion(this Assembly self)
		{
			return self.GetName().Version.ToString();
		}

		public static string GetInformationalVersion(this Assembly self)
		{
			return self.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
		}

		public static string GetDirectoryName(this Assembly self)
		{
			return Path.GetDirectoryName(self.Location);
		}

		public static FileStream ReadAsStream(this Assembly self)
		{
			return File.OpenRead(self.Location);
		}

		public static string GetHash(this Assembly self, int length = 7)
		{
			return HashStream.ComputeHashString(File.OpenRead(self.Location), SHA1.Create()).Substring(0, length).ToLowerInvariant();
		}

		public static string GetVersionWithHash(this Assembly self, int length = 7)
		{
			return self.GetVersion() + "+" + self.GetHash(length);
		}

		public static string GetInformationalVersionWithHash(this Assembly self, int length = 7)
		{
			return self.GetInformationalVersion() + "+" + self.GetHash(length);
		}

		public static string GetMyVersion()
		{
			return Assembly.GetCallingAssembly().GetVersion();
		}

		public static string GetMyInformationalVersion()
		{
			return Assembly.GetCallingAssembly().GetInformationalVersion();
		}

		public static string GetMyHash(int length = 7)
		{
			return Assembly.GetCallingAssembly().GetHash(length);
		}

		public static string GetMyVersionWithHash(int length = 7)
		{
			return Assembly.GetCallingAssembly().GetVersionWithHash(length);
		}

		public static string GetMyInformationalVersionWithHash(int length = 7)
		{
			return Assembly.GetCallingAssembly().GetInformationalVersionWithHash(length);
		}

		public static Lazy<string> GetMyDefaultVersion()
		{
			Assembly asm = Assembly.GetCallingAssembly();
			return new Lazy<string>(() => asm.GetInformationalVersion());
		}

		public static Lazy<string> GetMyDefaultVersionWithHash()
		{
			Assembly asm = Assembly.GetCallingAssembly();
			return new Lazy<string>(() => asm.GetInformationalVersionWithHash());
		}

		public static IEnumerable<Stream> GetManifestResourceStreams(this Assembly self)
		{
			return self.GetManifestResourceStreams(Delegates.Pass<string>());
		}

		public static IEnumerable<Stream> GetManifestResourceStreams(this Assembly self, Func<string, bool> f)
		{
			return self.GetManifestResourceNames().Filter(f).Map(self.GetManifestResourceStream);
		}
	}
	public sealed record Box<T>(T Value)
	{
		public T Value { get; set; }

		public static implicit operator Box<T>(T value)
		{
			return new Box<T>(value);
		}

		public static explicit operator T(Box<T> box)
		{
			return box.Value;
		}
	}
	[PublicAPI]
	public static class CloneUtil
	{
		private static readonly LazyMap<Type, Func<object, object>> clonerMap = new LazyMap<Type, Func<object, object>>((Type t) => ILUtil.CreateDynamicMethod<Func<object, object>>("<OsmiCloneUtil>__" + t.FullName.Replace('.', '_'), delegate(ILProcessor ilp)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			ilp.Body.Variables.Add(new VariableDefinition(Extensions.Import(ilp, t)));
			Extensions.Emit(ilp, OpCodes.Newobj, (MethodBase)(t.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) ?? throw new NotSupportedException("Only supports type with parameterless constructor")));
			ilp.Emit(OpCodes.Stloc_0);
			FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				ilp.Emit(OpCodes.Ldloc_0);
				ilp.Emit(OpCodes.Ldarg_0);
				Extensions.Emit(ilp, OpCodes.Ldfld, fieldInfo);
				Extensions.Emit(ilp, OpCodes.Stfld, fieldInfo);
			}
			ilp.Emit(OpCodes.Ldloc_0);
			ilp.Emit(OpCodes.Ret);
		}));

		public static T CreateMemberwiseClone<T>(T source) where T : class
		{
			return clonerMap[source.GetType()](source) as T;
		}
	}
	[PublicAPI]
	public static class DeconstructUtil
	{
		public static void Deconstruct(this Vector3 self, out float x, out float y, out float z)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			(x, y, z) = self.ToTuple();
		}

		public static void Deconstruct(this Vector2 self, out float x, out float y)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			(x, y) = self.ToTuple();
		}

		public static void Deconstruct(this RaycastHit2D hit, out Collider2D? collider)
		{
			collider = ((RaycastHit2D)(ref hit)).collider;
		}

		public static void Deconstruct(this RaycastHit2D hit, out Vector2 point, out float distance)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			point = ((RaycastHit2D)(ref hit)).point;
			distance = ((RaycastHit2D)(ref hit)).distance;
		}

		public static void Deconstruct(this RaycastHit2D hit, out Collider2D? collider, out Vector2 point, out float distance)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			collider = ((RaycastHit2D)(ref hit)).collider;
			point = ((RaycastHit2D)(ref hit)).point;
			distance = ((RaycastHit2D)(ref hit)).distance;
		}

		public static void Deconstruct<T>(this T[] array, out T first, out T[] rest)
		{
			if (array.Length < 1)
			{
				throw new ArgumentException("array", "Array does not have enough items");
			}
			first = array[0];
			checked
			{
				rest = new T[array.Length - 1];
				Array.Copy(array, 1, rest, 0, array.Length - 1);
			}
		}

		public static void Deconstruct<T>(this T[] array, out T first, out T second, out T[] rest)
		{
			if (array.Length < 2)
			{
				throw new ArgumentException("array", "Array does not have enough items");
			}
			first = array[0];
			second = array[1];
			checked
			{
				rest = new T[array.Length - 2];
				Array.Copy(array, 2, rest, 0, array.Length - 2);
			}
		}

		public static void Deconstruct<T>(this T[] array, out T first, out T second, out T third, out T[] rest)
		{
			if (array.Length < 3)
			{
				throw new ArgumentException("array", "Array does not have enough items");
			}
			first = array[0];
			second = array[1];
			third = array[2];
			checked
			{
				rest = new T[array.Length - 3];
				Array.Copy(array, 3, rest, 0, array.Length - 3);
			}
		}

		public static void Deconstruct<T>(this T[] array, out T first, out T second, out T third, out T fourth, out T[] rest)
		{
			if (array.Length < 4)
			{
				throw new ArgumentException("array", "Array does not have enough items");
			}
			first = array[0];
			second = array[1];
			third = array[2];
			fourth = array[3];
			checked
			{
				rest = new T[array.Length - 4];
				Array.Copy(array, 4, rest, 0, array.Length - 4);
			}
		}

		public static void Deconstruct<T>(this T[] array, out T first, out T second, out T third, out T fourth, out T fifth, out T[] rest)
		{
			if (array.Length < 5)
			{
				throw new ArgumentException("array", "Array does not have enough items");
			}
			first = array[0];
			second = array[1];
			third = array[2];
			fourth = array[3];
			fifth = array[4];
			checked
			{
				rest = new T[array.Length - 5];
				Array.Copy(array, 5, rest, 0, array.Length - 5);
			}
		}
	}
	[PublicAPI]
	public static class Delegates
	{
		public static Func<T, T> Identity<T>()
		{
			return Funcs.Identity;
		}

		public static Action<T> Drop<T>()
		{
			return Funcs.Drop;
		}

		public static Action NoOp()
		{
			return Funcs.NoOp;
		}

		public static Action<A> NoOp<A>()
		{
			return delegate
			{
			};
		}

		public static Action<A, B> NoOp<A, B>()
		{
			return delegate
			{
			};
		}

		public static Action<A, B, C> NoOp<A, B, C>()
		{
			return delegate
			{
			};
		}

		public static Func<A, bool> Pass<A>()
		{
			return (A _) => true;
		}

		public static Func<A, B, bool> Pass<A, B>()
		{
			return (A _, B _) => true;
		}

		public static Func<A, B, C, bool> Pass<A, B, C>()
		{
			return (A _, B _, C _) => true;
		}

		public static Func<A, bool> Block<A>()
		{
			return (A _) => false;
		}

		public static Func<A, B, bool> Block<A, B>()
		{
			return (A _, B _) => false;
		}

		public static Func<A, B, C, bool> Block<A, B, C>()
		{
			return (A _, B _, C _) => false;
		}

		public static Func<T?> Default<T>()
		{
			return Funcs.Default<T>;
		}

		public static Func<T> New<T>() where T : new()
		{
			return Funcs.New<T>;
		}

		public static Func<T> Constant<T>(T val)
		{
			return Funcs.Constant(val);
		}

		public static Func<U> Constant<T, U>(T val) where T : U
		{
			return Funcs.Constant<T, U>(val);
		}
	}
	[PublicAPI]
	public static class EnumerableUtil
	{
		public static T Fold<T>(this IEnumerable<T> self, Func<T, T, T> f)
		{
			return self.Aggregate(f);
		}

		public static U Reduce<T, U>(this IEnumerable<T> self, Func<U, T, U> f, U init)
		{
			return self.Aggregate(init, f);
		}

		public static string Collect(this IEnumerable<char> self)
		{
			return new string(self.ToArray());
		}

		public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> self)
		{
			return self.ToDictionary<KeyValuePair<K, V>, K, V>((KeyValuePair<K, V> pair) => pair.Key, (KeyValuePair<K, V> pair) => pair.Value);
		}

		public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<(K key, V value)> self)
		{
			return self.ToDictionary<(K, V), K, V>(((K key, V value) tuple) => tuple.key, ((K key, V value) tuple) => tuple.value);
		}

		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> self)
		{
			return new HashSet<T>(self);
		}

		public static IEnumerator<T> Move<T>(this IEnumerator<T> self, int step = 1)
		{
			while (checked(step--) > 0)
			{
				self.MoveNext();
			}
			return self;
		}

		public static IEnumerable<T> AsEnumerable<T>(this IEnumerator<T> self)
		{
			while (self.MoveNext())
			{
				yield return self.Current;
			}
		}

		public static void ForEach<T>(this IEnumerable<T> self, Action<T> f)
		{
			foreach (T item in self)
			{
				f(item);
			}
		}

		public static IEnumerable<U> Map<T, U>(this IEnumerable<T> self, Func<T, U> f)
		{
			return self.Select(f);
		}

		public static IEnumerable<U> FlatMap<T, U>(this IEnumerable<T> self, Func<T, IEnumerable<U>> f)
		{
			return self.SelectMany(f);
		}

		public static IEnumerable<T> Tap<T>(this IEnumerable<T> self, Action<T> f)
		{
			return self.Map(TapUtil.TapFunc(f));
		}

		public static IEnumerable<T> Filter<T>(this IEnumerable<T> self, Func<T, bool> f)
		{
			return self.Where(f);
		}

		public static IEnumerable<T> Reject<T>(this IEnumerable<T> self, Func<T, bool> f)
		{
			return self.Where<T>(f.Negate());
		}

		public static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> self, Func<T, bool> f)
		{
			return self.TakeWhile<T>(f.Negate());
		}

		public static IEnumerable<T> SkipUntil<T>(this IEnumerable<T> self, Func<T, bool> f)
		{
			return self.SkipWhile<T>(f.Negate());
		}

		public static IEnumerable<T> Evaluate<T>(this IEnumerable<T> self)
		{
			return self.ToArray().AsEnumerable();
		}

		public static IEnumerable<T> Of<T>(params T[] values)
		{
			return values;
		}

		public static IEnumerable<T> Of<T>(T val, int count)
		{
			return Enumerable.Repeat(val, count);
		}

		public static void ParallelForEach<T>(this IEnumerable<T> self, Action<T> f)
		{
			Parallel.ForEach(self, f);
		}

		public static bool TryAdd<K, V>(this IDictionary<K, V> self, K key, V val)
		{
			if (self.ContainsKey(key))
			{
				return false;
			}
			self[key] = val;
			return true;
		}

		public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> self)
		{
			return self.FlatMap(Funcs.Identity);
		}

		public static IEnumerable<T> Repeat<T>(this IEnumerable<T> self, int times)
		{
			for (int i = 0; i < times; i = checked(i + 1))
			{
				foreach (T item in self)
				{
					yield return item;
				}
			}
		}

		public static IEnumerable<T> RemoveAt<T>(this IEnumerable<T> self, int index)
		{
			var (first, _, second) = self.SplitAt(index);
			return first.Concat(second);
		}

		public static IEnumerable<T> Insert<T>(this IEnumerable<T> self, int index, T value)
		{
			if (index < 0 || index >= self.Count())
			{
				throw new ArgumentOutOfRangeException("index");
			}
			var (source, second) = self.Split(index);
			return source.Append(value).Concat(second);
		}

		public static IEnumerable<T> Order<T>(this IEnumerable<T> self)
		{
			return self.OrderBy(Funcs.Identity);
		}

		public static IEnumerable<T> OrderDescending<T>(this IEnumerable<T> self)
		{
			return self.OrderByDescending(Funcs.Identity);
		}

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> self)
		{
			return self.Shuffle(new Random(Random.Range(int.MinValue, int.MaxValue)));
		}

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> self, Random random)
		{
			T[] array = self.ToArray();
			int count = array.Length;
			for (int i = 0; i < count; i = checked(i + 1))
			{
				int j = random.Next(i, count);
				yield return array[j];
				array[j] = array[i];
			}
		}

		public static bool Includes<T>(this IEnumerable<T> self, T val)
		{
			return self.Contains(val);
		}

		public static bool Empty<T>(this IEnumerable<T> self)
		{
			return !self.Any();
		}

		public static bool Some<T>(this IEnumerable<T> self, Func<T, bool> f)
		{
			return self.Any(f);
		}

		public static bool Every<T>(this IEnumerable<T> self, Func<T, bool> f)
		{
			return self.All(f);
		}

		public static int FindIndex<T>(this IEnumerable<T> self, Func<T, bool> f)
		{
			int num = 0;
			foreach (T item in self)
			{
				if (f(item))
				{
					return num;
				}
				num = checked(num + 1);
			}
			return -1;
		}

		public static bool SeqEqual<T>(this IEnumerable<T> self, IEnumerable<T> other)
		{
			if (self.Count() == other.Count())
			{
				return self.SequenceEqual(other);
			}
			return false;
		}

		public static IEnumerable<T> Slice<T>(this IEnumerable<T> self, int start)
		{
			return self.Slice(start, self.Count());
		}

		public static IEnumerable<T> Slice<T>(this IEnumerable<T> self, int start, int end)
		{
			if (start < 0)
			{
				throw new ArgumentOutOfRangeException("start");
			}
			if (start > end || end > self.Count())
			{
				throw new ArgumentOutOfRangeException("end");
			}
			int index = 0;
			checked
			{
				int length = end - start;
				using IEnumerator<T> enumerator = self.Skip(start).GetEnumerator();
				while (index++ < length)
				{
					enumerator.MoveNext();
					yield return enumerator.Current;
				}
			}
		}

		public static (IEnumerable<T> former, IEnumerable<T> latter) Split<T>(this IEnumerable<T> self, int index)
		{
			if (index == 0 && self.Empty())
			{
				return (former: Of(Array.Empty<T>()), latter: Of(Array.Empty<T>()));
			}
			var (item, element, source) = self.SplitAt(index);
			return (former: item, latter: source.Prepend(element));
		}

		public static (IEnumerable<T> former, T value, IEnumerable<T> latter) SplitAt<T>(this IEnumerable<T> self, int index)
		{
			if (index < 0 || index > self.Count())
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (self.Empty())
			{
				throw new ArgumentException("Enumerable is empty");
			}
			List<T> list = new List<T>();
			int num = 0;
			using IEnumerator<T> enumerator = self.GetEnumerator();
			while (checked(num++) < index)
			{
				enumerator.MoveNext();
				list.Add(enumerator.Current);
			}
			enumerator.MoveNext();
			T current = enumerator.Current;
			return (former: list, value: current, latter: enumerator.AsEnumerable());
		}
	}
	[PublicAPI]
	public static class Funcs
	{
		public static T Identity<T>(T self)
		{
			return self;
		}

		public static void Drop<T>(T t)
		{
			if (t is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}

		public static void NoOp()
		{
		}

		public static void NoOp(params object?[] _)
		{
		}

		public static bool Pass(params object?[] _)
		{
			return true;
		}

		public static bool Block(params object?[] _)
		{
			return false;
		}

		public static T? Default<T>()
		{
			return default(T);
		}

		public static T New<T>() where T : new()
		{
			return new T();
		}

		public static Func<T> Constant<T>(T val)
		{
			return () => val;
		}

		public static Func<U> Constant<T, U>(T val) where T : U
		{
			return () => (U)(object)val;
		}
	}
	[PublicAPI]
	public static class FuncUtil
	{
		public static Coroutine SetTimeOut(this MonoBehaviour self, float timeOut, Action action)
		{
			return self.StartCoroutine(TimeOutCoroutine(action, timeOut));
		}

		public static Coroutine SetImmediate(this MonoBehaviour self, Action action)
		{
			return self.SetTimeOut(0f, action);
		}

		private static IEnumerator TimeOutCoroutine(Action action, float timeOut)
		{
			yield return (object)new WaitForSeconds(timeOut);
			action();
		}

		public static Func<T> Bind<A, T>(this Func<A, T> self, A a)
		{
			return () => self(a);
		}

		public static Func<B, T> Bind1<A, B, T>(this Func<A, B, T> self, A a)
		{
			return (B b) => self(a, b);
		}

		public static Func<A, T> Bind2<A, B, T>(this Func<A, B, T> self, B b)
		{
			return (A a) => self(a, b);
		}

		public static Func<B, C, T> Bind1<A, B, C, T>(this Func<A, B, C, T> self, A a)
		{
			return (B b, C c) => self(a, b, c);
		}

		public static Func<A, C, T> Bind2<A, B, C, T>(this Func<A, B, C, T> self, B b)
		{
			return (A a, C c) => self(a, b, c);
		}

		public static Func<A, B, T> Bind3<A, B, C, T>(this Func<A, B, C, T> self, C c)
		{
			return (A a, B b) => self(a, b, c);
		}

		public static Func<B, C, D, T> Bind1<A, B, C, D, T>(this Func<A, B, C, D, T> self, A a)
		{
			return (B b, C c, D d) => self(a, b, c, d);
		}

		public static Func<A, C, D, T> Bind2<A, B, C, D, T>(this Func<A, B, C, D, T> self, B b)
		{
			return (A a, C c, D d) => self(a, b, c, d);
		}

		public static Func<A, B, D, T> Bind3<A, B, C, D, T>(this Func<A, B, C, D, T> self, C c)
		{
			return (A a, B b, D d) => self(a, b, c, d);
		}

		public static Func<A, B, C, T> Bind4<A, B, C, D, T>(this Func<A, B, C, D, T> self, D d)
		{
			return (A a, B b, C c) => self(a, b, c, d);
		}

		public static Func<B, C, D, E, T> Bind1<A, B, C, D, E, T>(this Func<A, B, C, D, E, T> self, A a)
		{
			return (B b, C c, D d, E e) => self(a, b, c, d, e);
		}

		public static Func<A, C, D, E, T> Bind2<A, B, C, D, E, T>(this Func<A, B, C, D, E, T> self, B b)
		{
			return (A a, C c, D d, E e) => self(a, b, c, d, e);
		}

		public static Func<A, B, D, E, T> Bind3<A, B, C, D, E, T>(this Func<A, B, C, D, E, T> self, C c)
		{
			return (A a, B b, D d, E e) => self(a, b, c, d, e);
		}

		public static Func<A, B, C, E, T> Bind4<A, B, C, D, E, T>(this Func<A, B, C, D, E, T> self, D d)
		{
			return (A a, B b, C c, E e) => self(a, b, c, d, e);
		}

		public static Func<A, B, C, D, T> Bind5<A, B, C, D, E, T>(this Func<A, B, C, D, E, T> self, E e)
		{
			return (A a, B b, C c, D d) => self(a, b, c, d, e);
		}

		public static Action Bind<A>(this Action<A> self, A a)
		{
			return delegate
			{
				self(a);
			};
		}

		public static Action<B> Bind1<A, B>(this Action<A, B> self, A a)
		{
			return delegate(B b)
			{
				self(a, b);
			};
		}

		public static Action<A> Bind2<A, B>(this Action<A, B> self, B b)
		{
			return delegate(A a)
			{
				self(a, b);
			};
		}

		public static Action<B, C> Bind1<A, B, C>(this Action<A, B, C> self, A a)
		{
			return delegate(B b, C c)
			{
				self(a, b, c);
			};
		}

		public static Action<A, C> Bind2<A, B, C>(this Action<A, B, C> self, B b)
		{
			return delegate(A a, C c)
			{
				self(a, b, c);
			};
		}

		public static Action<A, B> Bind3<A, B, C>(this Action<A, B, C> self, C c)
		{
			return delegate(A a, B b)
			{
				self(a, b, c);
			};
		}

		public static Action<B, C, D> Bind1<A, B, C, D>(this Action<A, B, C, D> self, A a)
		{
			return delegate(B b, C c, D d)
			{
				self(a, b, c, d);
			};
		}

		public static Action<A, C, D> Bind2<A, B, C, D>(this Action<A, B, C, D> self, B b)
		{
			return delegate(A a, C c, D d)
			{
				self(a, b, c, d);
			};
		}

		public static Action<A, B, D> Bind3<A, B, C, D>(this Action<A, B, C, D> self, C c)
		{
			return delegate(A a, B b, D d)
			{
				self(a, b, c, d);
			};
		}

		public static Action<A, B, C> Bind4<A, B, C, D>(this Action<A, B, C, D> self, D d)
		{
			return delegate(A a, B b, C c)
			{
				self(a, b, c, d);
			};
		}

		public static Action<B, C, D, E> Bind1<A, B, C, D, E>(this Action<A, B, C, D, E> self, A a)
		{
			return delegate(B b, C c, D d, E e)
			{
				self(a, b, c, d, e);
			};
		}

		public static Action<A, C, D, E> Bind2<A, B, C, D, E>(this Action<A, B, C, D, E> self, B b)
		{
			return delegate(A a, C c, D d, E e)
			{
				self(a, b, c, d, e);
			};
		}

		public static Action<A, B, D, E> Bind3<A, B, C, D, E>(this Action<A, B, C, D, E> self, C c)
		{
			return delegate(A a, B b, D d, E e)
			{
				self(a, b, c, d, e);
			};
		}

		public static Action<A, B, C, E> Bind4<A, B, C, D, E>(this Action<A, B, C, D, E> self, D d)
		{
			return delegate(A a, B b, C c, E e)
			{
				self(a, b, c, d, e);
			};
		}

		public static Action<A, B, C, D> Bind5<A, B, C, D, E>(this Action<A, B, C, D, E> self, E e)
		{
			return delegate(A a, B b, C c, D d)
			{
				self(a, b, c, d, e);
			};
		}

		public static T Combine<T>(T a, T b) where T : Delegate
		{
			return (T)Delegate.Combine(a, b);
		}

		public static T Combine<T>(params T[] delegates) where T : Delegate
		{
			return (T)Delegate.Combine(delegates);
		}

		public static Func<bool> Negate(this Func<bool> self)
		{
			return () => !self();
		}

		public static Func<A, bool> Negate<A>(this Func<A, bool> self)
		{
			return (A a) => !self(a);
		}

		public static Func<A, B, bool> Negate<A, B>(this Func<A, B, bool> self)
		{
			return (A a, B b) => !self(a, b);
		}

		public static Func<A, B, C, bool> Negate<A, B, C>(this Func<A, B, C, bool> self)
		{
			return (A a, B b, C c) => !self(a, b, c);
		}

		public static Func<A, B, C, D, bool> Negate<A, B, C, D>(this Func<A, B, C, D, bool> self)
		{
			return (A a, B b, C c, D d) => !self(a, b, c, d);
		}

		public static Func<A, B, C, D, E, bool> Negate<A, B, C, D, E>(this Func<A, B, C, D, E, bool> self)
		{
			return (A a, B b, C c, D d, E e) => !self(a, b, c, d, e);
		}

		public static Func<A, B, C, D, E, G, bool> Negate<A, B, C, D, E, G>(this Func<A, B, C, D, E, G, bool> self)
		{
			return (A a, B b, C c, D d, E e, G g) => !self(a, b, c, d, e, g);
		}

		public static Func<U> Thru<T, U>(this Func<T> self, Func<T, U> f)
		{
			return () => f(self());
		}

		public static Func<A, U> Thru<A, T, U>(this Func<A, T> self, Func<T, U> f)
		{
			return (A a) => f(self(a));
		}

		public static Func<A, B, U> Thru<A, B, T, U>(this Func<A, B, T> self, Func<T, U> f)
		{
			return (A a, B b) => f(self(a, b));
		}

		public static Func<A, B, C, U> Thru<A, B, C, T, U>(this Func<A, B, C, T> self, Func<T, U> f)
		{
			return (A a, B b, C c) => f(self(a, b, c));
		}

		public static Func<A, B, C, D, U> Thru<A, B, C, D, T, U>(this Func<A, B, C, D, T> self, Func<T, U> f)
		{
			return (A a, B b, C c, D d) => f(self(a, b, c, d));
		}

		public static Func<A, B, C, D, E, U> Thru<A, B, C, D, E, T, U>(this Func<A, B, C, D, E, T> self, Func<T, U> f)
		{
			return (A a, B b, C c, D d, E e) => f(self(a, b, c, d, e));
		}

		public static Func<A, B, C, D, E, G, U> Thru<A, B, C, D, E, G, T, U>(this Func<A, B, C, D, E, G, T> self, Func<T, U> f)
		{
			return (A a, B b, C c, D d, E e, G g) => f(self(a, b, c, d, e, g));
		}

		public static Action Thru<T>(this Func<T> self, Action<T> f)
		{
			return delegate
			{
				f(self());
			};
		}

		public static Action<A> Thru<A, T>(this Func<A, T> self, Action<T> f)
		{
			return delegate(A a)
			{
				f(self(a));
			};
		}

		public static Action<A, B> Thru<A, B, T>(this Func<A, B, T> self, Action<T> f)
		{
			return delegate(A a, B b)
			{
				f(self(a, b));
			};
		}

		public static Action<A, B, C> Thru<A, B, C, T>(this Func<A, B, C, T> self, Action<T> f)
		{
			return delegate(A a, B b, C c)
			{
				f(self(a, b, c));
			};
		}

		public static Action<A, B, C, D> Thru<A, B, C, D, T>(this Func<A, B, C, D, T> self, Action<T> f)
		{
			return delegate(A a, B b, C c, D d)
			{
				f(self(a, b, c, d));
			};
		}

		public static Action<A, B, C, D, E> Thru<A, B, C, D, E, T>(this Func<A, B, C, D, E, T> self, Action<T> f)
		{
			return delegate(A a, B b, C c, D d, E e)
			{
				f(self(a, b, c, d, e));
			};
		}

		public static Action<A, B, C, D, E, G> Thru<A, B, C, D, E, G, T>(this Func<A, B, C, D, E, G, T> self, Action<T> f)
		{
			return delegate(A a, B b, C c, D d, E e, G g)
			{
				f(self(a, b, c, d, e, g));
			};
		}
	}
	public sealed record GameObjectRef
	{
		public string SceneName { get; private init; }

		public string[] Path { get; private init; }

		public const string WILDCARD_SCENE_NAME = "*****************************************************************************************************************************************************************************************************************************************************";

		public const string DONT_DESTROY_ON_LOAD = "DontDestroyOnLoad";

		public GameObjectRef(string sceneName, params string[] path)
		{
			if (sceneName.Length > 244 && sceneName != "*****************************************************************************************************************************************************************************************************************************************************")
			{
				throw new ArgumentException("sceneName");
			}
			if (path.Length < 1)
			{
				throw new ArgumentException("path");
			}
			SceneName = sceneName;
			Path = path.ToArray();
		}

		public bool MatchScene(Scene scene)
		{
			if (!(SceneName == "*****************************************************************************************************************************************************************************************************************************************************"))
			{
				return SceneName == ((Scene)(ref scene)).name;
			}
			return true;
		}

		public bool MatchGameObject(GameObject go)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			if (MatchScene(go.scene))
			{
				return go.GetPath().SeqEqual(Path);
			}
			return false;
		}
	}
	[PublicAPI]
	public static class GameObjectUtil
	{
		private static readonly GameObject holder = New("Osmi MonoBehaviour Holder", dontDestroyOnLoad: true);

		internal static void Init()
		{
		}

		public static void SetParent(this GameObject self, Transform parent)
		{
			self.transform.parent = parent;
		}

		public static void SetParent(this GameObject self, GameObject parent)
		{
			self.SetParent(parent.transform);
		}

		public static void SetRoot(this GameObject self)
		{
			self.SetParent((Transform)null);
		}

		public static GameObject? Child(this GameObject self, string name)
		{
			Transform obj = self.transform.Find(name);
			if (obj == null)
			{
				return null;
			}
			return ((Component)obj).gameObject;
		}

		public static GameObject? Child(this GameObject self, params string[] path)
		{
			Transform obj = self.transform.Find(path.Join('/'));
			if (obj == null)
			{
				return null;
			}
			return ((Component)obj).gameObject;
		}

		public static GameObject? Child(this GameObject self, ArraySegment<string> path)
		{
			Transform obj = self.transform.Find(path.Join('/'));
			if (obj == null)
			{
				return null;
			}
			return ((Component)obj).gameObject;
		}

		public static IEnumerable<GameObject> GetChildren(this GameObject self)
		{
			foreach (Transform item in self.transform)
			{
				Transform val = item;
				yield return ((Component)val).gameObject;
			}
		}

		public static string[] GetPath(this GameObject self)
		{
			List<string> list = new List<string>();
			GetPathInternal(self.transform, list);
			list.Reverse();
			return list.ToArray();
		}

		private static void GetPathInternal(Transform tf, List<string> path)
		{
			path.Add(((Object)tf).name);
			Transform parent = tf.parent;
			if (parent != null)
			{
				GetPathInternal(parent, path);
			}
		}

		public static GameObject? GetRootGameObject(this Scene scene, string name)
		{
			return ((Scene)(ref scene)).GetRootGameObjects().FirstOrDefault((GameObject go) => ((Object)go).name == name);
		}

		public static GameObject? GetGameObjectByPath(this Scene scene, params string[] path)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return scene.GetGameObjectByPath(new ArraySegment<string>(path));
		}

		public static GameObject? GetGameObjectByPath(this Scene scene, ArraySegment<string> path)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if (path.Count == 0)
			{
				throw new ArgumentException("path");
			}
			GameObject rootGameObject = scene.GetRootGameObject(((IList<string>)path)[0]);
			if (path.Count <= 1)
			{
				return rootGameObject;
			}
			return Child(rootGameObject?, checked(new ArraySegment<string>(path.Array, path.Offset + 1, path.Count - 1)));
		}

		public static GameObject? FindGameObjectByRef(this Scene scene, GameObjectRef goRef)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			if (!goRef.MatchScene(scene))
			{
				return null;
			}
			return scene.GetGameObjectByPath(goRef.Path);
		}

		public static TransformDelegate GetDelegate(this Transform self)
		{
			return new TransformDelegate(self);
		}

		public static TransformDelegate GetTransformDelegate(this GameObject self)
		{
			return self.transform.GetDelegate();
		}

		public static GameObject New(bool dontDestroyOnLoad = false)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GameObject val = new GameObject();
			if (dontDestroyOnLoad)
			{
				Object.DontDestroyOnLoad((Object)(object)val);
			}
			return val;
		}

		public static GameObject New(string name, bool dontDestroyOnLoad = false)
		{
			return New(name, (Transform)null, dontDestroyOnLoad);
		}

		public static GameObject New(string name, Transform parent, bool dontDestroyOnLoad = false)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			GameObject val = new GameObject(name);
			val.SetParent(parent);
			if (dontDestroyOnLoad)
			{
				Object.DontDestroyOnLoad((Object)(object)val);
			}
			return val;
		}

		public static GameObject New(string name, GameObject parent, bool dontDestroyOnLoad = false)
		{
			return New(name, parent.transform, dontDestroyOnLoad);
		}

		public static GameObject New(string name, Transform parent, params Type[] components)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Expected O, but got Unknown
			//IL_000f: Expected O, but got Unknown
			GameObject val = new GameObject(name, components);
			SetParent(val, parent);
			return val;
		}

		public static GameObject New(string name, GameObject parent, params Type[] components)
		{
			return New(name, parent.transform, components);
		}

		public static GameObject New(string name, Transform parent, bool dontDestroyOnLoad, params Type[] components)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Expected O, but got Unknown
			GameObject val = new GameObject(name, components);
			val.SetParent(parent);
			if (dontDestroyOnLoad)
			{
				Object.DontDestroyOnLoad((Object)(object)val);
			}
			return val;
		}

		public static GameObject New(string name, GameObject parent, bool dontDestroyOnLoad, params Type[] components)
		{
			return New(name, parent.transform, dontDestroyOnLoad, components);
		}

		public static T Instantiate<T>(T original, Vector3 position) where T : Object
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			return Object.Instantiate<T>(original, position, Quaternion.identity);
		}

		public static T Instantiate<T>(T original, Vector3 position, Transform parent) where T : Object
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			return Object.Instantiate<T>(original, position, Quaternion.identity, parent);
		}

		public static T Instantiate<T>(T original, Vector3 position, GameObject parent) where T : Object
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			return Object.Instantiate<T>(original, position, Quaternion.identity, parent.transform);
		}

		public static T CreateHolder<T>(string name = "") where T : MonoBehaviour
		{
			return New(string.IsNullOrWhiteSpace(name) ? (typeof(T).Name + " Holder") : name, holder.transform).AddComponent<T>();
		}
	}
	public class HashStream : Stream
	{
		private readonly HashAlgorithm hash;

		private readonly CryptoStream crypto;

		public byte[]? Hash { get; private set; }

		public string? HashString
		{
			get
			{
				if (Hash != null)
				{
					return BitConverter.ToString(Hash).RemoveChar('-');
				}
				return null;
			}
		}

		public override bool CanRead => false;

		public override bool CanSeek => false;

		public override bool CanWrite => true;

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public HashStream(HashAlgorithm hash)
		{
			this.hash = hash;
			crypto = new CryptoStream(Stream.Null, hash, CryptoStreamMode.Write);
		}

		private static HashStream ComputeHashInternal(Stream stream, HashAlgorithm hash)
		{
			if (!stream.CanRead)
			{
				throw new ArgumentException("stream", "Stream not readable");
			}
			HashStream hashStream = new HashStream(hash);
			stream.CopyTo(hashStream);
			stream.Dispose();
			hashStream.Dispose();
			return hashStream;
		}

		public static byte[] ComputeHash(Stream stream, HashAlgorithm hash)
		{
			return ComputeHashInternal(stream, hash).Hash;
		}

		public static string ComputeHashString(Stream stream, HashAlgorithm hash)
		{
			return ComputeHashInternal(stream, hash).HashString;
		}

		public byte[] Finalize()
		{
			Dispose();
			return Hash;
		}

		public string FinalizeToString()
		{
			Dispose();
			return HashString;
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			crypto.Write(buffer, offset, count);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				crypto.Dispose();
				Hash = hash.Hash;
				hash.Dispose();
			}
			base.Dispose(disposing);
		}
	}
	[PublicAPI]
	public static class HealthShareUtil
	{
		public static SharedHealthManager? GetSharedHM(this HealthManager self)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return new DynamicData((object)self).Get<SharedHealthManager>("sharedHealthManager");
		}

		public static bool TryGetSharedHM(this HealthManager self, [NotNullWhen(true)] out SharedHealthManager? shm)
		{
			return (Object)(object)(shm = self.GetSharedHM()) != (Object)null;
		}

		public static bool IsSharing(this HealthManager self)
		{
			SharedHealthManager shm;
			return self.TryGetSharedHM(out shm);
		}

		public static SharedHealthManager? GetBackedSharedHM(this HealthManager self)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return new DynamicData((object)self).Get<SharedHealthManager>("backingSharedHealthManager");
		}

		public static bool TryGetBackedSharedHM(this HealthManager self, [NotNullWhen(true)] out SharedHealthManager? shm)
		{
			return (Object)(object)(shm = self.GetBackedSharedHM()) != (Object)null;
		}

		public static bool IsBackingHM(this HealthManager self)
		{
			SharedHealthManager shm;
			return self.TryGetBackedSharedHM(out shm);
		}

		public static void AddToShared(this HealthManager self, SharedHealthManager shm)
		{
			shm.Add(self);
		}

		public static void AddToShared(this IEnumerable<HealthManager> self, SharedHealthManager shm)
		{
			shm.Add(self);
		}

		public static void AddToShared(this GameObject self, SharedHealthManager shm)
		{
			HealthManager component = self.GetComponent<HealthManager>();
			if (component != null)
			{
				shm.Add(component);
			}
		}

		public static void AddToShared(this IEnumerable<GameObject> self, SharedHealthManager shm)
		{
			(from go in self
				select go.GetComponent<HealthManager>() into hm
				where (Object)(object)hm != (Object)null
				select hm).AddToShared(shm);
		}

		public static SharedHealthManager ShareHealth(this IEnumerable<HealthManager> self, int initialHP = 0, string name = "SharedHealthManager")
		{
			return SharedHealthManager.Create(name, initialHP, self);
		}

		public static SharedHealthManager ShareHealth(this IEnumerable<GameObject> self, int initialHP = 0, string name = "SharedHealthManager")
		{
			SharedHealthManager sharedHealthManager = SharedHealthManager.Create(name, initialHP);
			self.AddToShared(sharedHealthManager);
			return sharedHealthManager;
		}

		public static SharedHealthManager ShareWith(this HealthManager self, params HealthManager[] hms)
		{
			return SharedHealthManager.Create(((Object)self).name + " Shared Health", hms.Prepend(self));
		}

		public static void StopSharing(this HealthManager self, int healthTaking)
		{
			if (!self.TryGetSharedHM(out SharedHealthManager shm))
			{
				throw new InvalidOperationException("Specified HM is not sharing health");
			}
			shm.Remove(self, healthTaking);
		}
	}
	[PublicAPI]
	public static class ILUtil
	{
		public static ILCursor GotoPrev(this ILCursor self, int count)
		{
			return self.Goto(checked(self.Index - count), (MoveType)0, false);
		}

		public static ILCursor GotoNext(this ILCursor self, int count)
		{
			return self.Goto(checked(self.Index + count), (MoveType)0, false);
		}

		public static ILCursor GotoEnd(this ILCursor self)
		{
			return self.Goto(checked(self.Instrs.Count - 1), (MoveType)0, false);
		}

		public static bool Match(this ILCursor self, params Func<Instruction, bool>[] predicates)
		{
			Instruction next = self.Next;
			for (int i = 0; i < predicates.Length; i = checked(i + 1))
			{
				if (next == null || !predicates[i](next))
				{
					return false;
				}
				next = next.Next;
			}
			return true;
		}

		public static ILCursor RemoveUntil(this ILCursor self, params Func<Instruction, bool>[] predicates)
		{
			while (!self.Match(predicates))
			{
				self.Remove();
			}
			return self;
		}

		public static ILCursor RemoveUntilEnd(this ILCursor self)
		{
			return self.RemoveRange(checked(self.Instrs.Count - self.Index - 1));
		}

		public static ILCursor RemoveAll(this ILCursor self)
		{
			return self.Goto(0, (MoveType)0, false).RemoveUntilEnd();
		}

		public static MethodInfo MethodOf<T>(T method) where T : Delegate
		{
			if (method.GetInvocationList().Length == 1)
			{
				return method.Method ?? throw new ArgumentException("method", "No corresponding MethodInfo");
			}
			throw new ArgumentException("method", "Must be a single method");
		}

		public static ILCursor EmitStaticMethodCall<T>(this ILCursor self, T method) where T : Delegate
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return self.Emit(OpCodes.Call, (MethodBase)MethodOf(method));
		}

		public static ILCursor MarkLabel(this ILCursor self, out ILLabel label)
		{
			label = self.MarkLabel();
			return self;
		}

		public static ILCursor RemoveAll(this ILContext self)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			return ILUtil.RemoveAll(new ILCursor(self));
		}

		public static ILContext ClearExceptionHandlers(this ILContext self)
		{
			self.IL.Body.ExceptionHandlers.Clear();
			return self;
		}

		public static DynamicMethodDefinition CreateDynamicMethodDefinition<T>(string name) where T : Delegate
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Expected O, but got Unknown
			MethodInfo methodInfo = ReflectionHelper.GetMethodInfo(typeof(T), "Invoke", true);
			return new DynamicMethodDefinition(name, methodInfo.ReturnType, methodInfo.GetParameters().Map((ParameterInfo pi) => pi.ParameterType).ToArray());
		}

		public static T CreateDynamicMethod<T>(string name, Action<ILProcessor> manip) where T : Delegate
		{
			DynamicMethodDefinition val = CreateDynamicMethodDefinition<T>(name);
			manip(val.GetILProcessor());
			return Extensions.CreateDelegate<T>((MethodBase)val.Generate());
		}

		public static Func<T> GetFastStaticGetter<T>(this FieldInfo fi)
		{
			return () => ReflectionHelper.GetField<T>(fi.DeclaringType, fi.Name);
		}

		public static Action<T> GetFastStaticSetter<T>(this FieldInfo fi)
		{
			return delegate(T value)
			{
				ReflectionHelper.SetField<T>(fi.DeclaringType, fi.Name, value);
			};
		}

		public static (Func<T> getter, Action<T> setter) GetFastStaticAccessors<T>(this FieldInfo fi)
		{
			return (getter: fi.GetFastStaticGetter<T>(), setter: fi.GetFastStaticSetter<T>());
		}

		public static Func<TType, TField> GetFastInstanceGetter<TType, TField>(this FieldInfo fi)
		{
			return (TType obj) => ReflectionHelper.GetField<TType, TField>(obj, fi.Name);
		}

		public static Action<TType, TField> GetFastInstanceSetter<TType, TField>(this FieldInfo fi)
		{
			return delegate(TType obj, TField value)
			{
				ReflectionHelper.SetField<TType, TField>(obj, fi.Name, value);
			};
		}

		public static (Func<TType, TField> getter, Action<TType, TField> setter) GetFastInstanceAccessors<TType, TField>(this FieldInfo fi)
		{
			return (getter: fi.GetFastInstanceGetter<TType, TField>(), setter: fi.GetFastInstanceSetter<TType, TField>());
		}
	}
	[PublicAPI]
	public static class ImageUtil
	{
		public static Texture2D ReadToTexture2D(this Stream self)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			//IL_0015: Expected O, but got Unknown
			Texture2D val = new Texture2D(2, 2);
			ImageConversion.LoadImage(val, self.ReadToBytes());
			return val;
		}

		public static Texture2D CreateReadableCopy(this Texture2D self)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Expected O, but got Unknown
			RenderTexture temporary = RenderTexture.GetTemporary(((Texture)self).width, ((Texture)self).height, 0, ((Texture)self).graphicsFormat);
			Graphics.Blit((Texture)(object)self, temporary);
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = temporary;
			Texture2D val = new Texture2D(((Texture)self).width, ((Texture)self).height);
			val.ReadPixels(new Rect(0f, 0f, (float)((Texture)temporary).width, (float)((Texture)temporary).height), 0, 0);
			val.Apply();
			RenderTexture.active = active;
			RenderTexture.ReleaseTemporary(temporary);
			return val;
		}

		public static Sprite MakeSprite(this Texture2D self, float pixelPerUnit = 64f)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			return Sprite.Create(self, new Rect(0f, 0f, (float)((Texture)self).width, (float)((Texture)self).height), new Vector2(0.5f, 0.5f), pixelPerUnit);
		}

		public static Sprite ReadToSprite(this Stream self, float pixelPerUnit = 64f)
		{
			return self.ReadToTexture2D().MakeSprite(pixelPerUnit);
		}
	}
	[PublicAPI]
	public static class LangUtil
	{
		public static HashSet<string> Langs { get; } = Language.GetLanguages().Map((string str) => str.ToLower().Replace('_', '-')).ToHashSet();

		public static string CurrentLang
		{
			get
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				return Language.CurrentLanguage().ToIdentifier();
			}
		}

		public unsafe static string ToIdentifier(this LanguageCode code)
		{
			return ((object)(*(LanguageCode*)(&code))/*cast due to constrained. prefix*/).ToString().ToLower().Replace('_', '-');
		}

		public static Dictionary<string, Lazy<Dictionary<string, string>>> LoadLangs(Assembly asm, IEnumerable<(string lang, string path)> tuples)
		{
			return tuples.ToDictionary<(string, string), string, Lazy<Dictionary<string, string>>>(((string lang, string path) tuple) => tuple.lang, ((string lang, string path) tuple) => new Lazy<Dictionary<string, string>>(delegate
			{
				Dictionary<string, string> result = JsonConvert.DeserializeObject<Dictionary<string, string>>(asm.GetManifestResourceStream(tuple.path).ReadToString());
				Logger.LogDebug("Loaded localization for lang " + tuple.lang + " in " + asm.GetName().Name);
				return result;
			}));
		}
	}
	public class LazyMap<K, V>
	{
		private readonly Func<K, V> mapper;

		private readonly Dictionary<K, V> inner = new Dictionary<K, V>();

		public V this[K key]
		{
			get
			{
				lock (inner)
				{
					V result;
					if (!inner.TryGetValue(key, out var value))
					{
						V val = (inner[key] = mapper(key));
						result = val;
					}
					else
					{
						result = value;
					}
					return result;
				}
			}
		}

		public LazyMap(Func<K, V> mapper)
		{
			this.mapper = mapper;
		}

		public void Remove(K key)
		{
			inner.Remove(key);
		}

		public void Clear()
		{
			inner.Clear();
		}
	}
	[PublicAPI]
	public static class NumberUtil
	{
		public static bool InRange(this int self, int min, int max)
		{
			if (self >= min)
			{
				return self <= max;
			}
			return false;
		}

		public static bool InRange(this short self, short min, short max)
		{
			if (self >= min)
			{
				return self <= max;
			}
			return false;
		}

		public static bool InRange(this long self, long min, long max)
		{
			if (self >= min)
			{
				return self <= max;
			}
			return false;
		}

		public static bool InRange(this float self, float min, float max)
		{
			if (self >= min)
			{
				return self <= max;
			}
			return false;
		}

		public static bool InRange(this double self, double min, double max)
		{
			if (self >= min)
			{
				return self <= max;
			}
			return false;
		}

		public static bool InRangeExclusive(this int self, int min, int max)
		{
			if (self > min)
			{
				return self < max;
			}
			return false;
		}

		public static bool InRangeExclusive(this short self, short min, short max)
		{
			if (self > min)
			{
				return self < max;
			}
			return false;
		}

		public static bool InRangeExclusive(this long self, long min, long max)
		{
			if (self > min)
			{
				return self < max;
			}
			return false;
		}

		public static bool InRangeExclusive(this float self, float min, float max)
		{
			if (self > min)
			{
				return self < max;
			}
			return false;
		}

		public static bool InRangeExclusive(this double self, double min, double max)
		{
			if (self > min)
			{
				return self < max;
			}
			return false;
		}

		public static int Clamp(this int self, int min, int max)
		{
			if (self >= min)
			{
				if (self <= max)
				{
					return self;
				}
				return max;
			}
			return min;
		}

		public static short Clamp(this short self, short min, short max)
		{
			if (self >= min)
			{
				if (self <= max)
				{
					return self;
				}
				return max;
			}
			return min;
		}

		public static long Clamp(this long self, long min, long max)
		{
			if (self >= min)
			{
				if (self <= max)
				{
					return self;
				}
				return max;
			}
			return min;
		}

		public static float Clamp(this float self, float min, float max)
		{
			if (!(self < min))
			{
				if (!(self > max))
				{
					return self;
				}
				return max;
			}
			return min;
		}

		public static double Clamp(this double self, double min, double max)
		{
			if (!(self < min))
			{
				if (!(self > max))
				{
					return self;
				}
				return max;
			}
			return min;
		}
	}
	[PublicAPI]
	public static class SceneDataUtil
	{
		public static bool IsActivated(this List<PersistentBoolData> self, string sceneName, string id)
		{
			return self.Filter((PersistentBoolData item) => item.sceneName == sceneName && item.id == id && item.activated).Any();
		}

		public static void Set(this PersistentBoolData self, bool activated, bool semiPersistent = false)
		{
			self.activated = activated;
			self.semiPersistent = semiPersistent;
		}

		public static void Set(this List<PersistentBoolData> self, string sceneName, string id, bool activated, bool semiPersistent = false)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Expected O, but got Unknown
			PersistentBoolData val = self.Filter((PersistentBoolData item) => item.sceneName == sceneName && item.id == id).FirstOrDefault();
			if (val != null)
			{
				val.Set(activated, semiPersistent);
				return;
			}
			self.Add(new PersistentBoolData
			{
				sceneName = sceneName,
				id = id,
				activated = activated,
				semiPersistent = semiPersistent
			});
		}
	}
	public sealed class SceneEdit
	{
		private static readonly Dictionary<string, HashSet<SceneEdit>> handles = new Dictionary<string, HashSet<SceneEdit>>();

		internal readonly GameObjectRef goRef;

		internal readonly Action<GameObject> action;

		private bool enabled;

		public SceneEdit(GameObjectRef goRef, Action<GameObject> action)
		{
			this.goRef = goRef;
			this.action = action;
		}

		public void Enable()
		{
			if (enabled)
			{
				throw new InvalidOperationException("Already enabled");
			}
			string sceneName = goRef.SceneName;
			if (!handles.TryGetValue(sceneName, out HashSet<SceneEdit> value))
			{
				HashSet<SceneEdit> hashSet = (handles[sceneName] = new HashSet<SceneEdit>());
				value = hashSet;
			}
			value.Add(this);
			enabled = true;
		}

		public void Disable()
		{
			if (!enabled)
			{
				throw new InvalidOperationException("Already Disabled");
			}
			string sceneName = goRef.SceneName;
			if (handles.TryGetValue(sceneName, out HashSet<SceneEdit> value))
			{
				value.Remove(this);
				enabled = false;
				if (value.Count == 0)
				{
					handles.Remove(sceneName);
				}
			}
		}

		internal static void ApplyHandles(Scene _prev, Scene next)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			if (handles.TryGetValue("*****************************************************************************************************************************************************************************************************************************************************", out HashSet<SceneEdit> value))
			{
				value.ForEach(delegate(SceneEdit handle)
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					handle.action(next.GetGameObjectByPath(handle.goRef.Path));
				});
			}
			if (handles.TryGetValue(((Scene)(ref next)).name, out HashSet<SceneEdit> value2))
			{
				value2.ForEach(delegate(SceneEdit handle)
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					handle.action(next.GetGameObjectByPath(handle.goRef.Path));
				});
			}
		}
	}
	[PublicAPI]
	public static class StreamUtil
	{
		public static string ReadToString(this Stream self)
		{
			StreamReader streamReader = new StreamReader(self);
			string result = streamReader.ReadToEnd();
			streamReader.Close();
			self.Close();
			return result;
		}

		public static byte[] ReadToBytes(this Stream self)
		{
			MemoryStream memoryStream = self as MemoryStream;
			if (memoryStream == null)
			{
				memoryStream = new MemoryStream(checked((int)Math.Min(self.Length, 2147483647L)));
				self.CopyTo(memoryStream);
				self.Close();
			}
			byte[] result = ((memoryStream.TryGetBuffer(out var buffer) && buffer.Offset == 0 && buffer.Count == buffer.Array.Length) ? buffer.Array : memoryStream.ToArray());
			memoryStream.Close();
			return result;
		}
	}
	[PublicAPI]
	public static class StringUtil
	{
		public static bool EnclosedWith(this string self, string start, string end)
		{
			if (self.StartsWith(start))
			{
				return self.EndsWith(end);
			}
			return false;
		}

		public static string StripStart(this string self, string val)
		{
			if (!self.StartsWith(val))
			{
				return self;
			}
			return self.Substring(val.Length);
		}

		public static string StripEnd(this string self, string val)
		{
			if (!self.EndsWith(val))
			{
				return self;
			}
			return self.Substring(0, checked(self.Length - val.Length));
		}

		public static string Repeat(this string self, int count)
		{
			checked
			{
				StringBuilder stringBuilder = new StringBuilder(self.Length * count);
				for (int i = 0; i < count; i++)
				{
					stringBuilder.Append(self);
				}
				return stringBuilder.ToString();
			}
		}

		public static string Concat(this string self, string other)
		{
			return self + other;
		}

		public static string Slice(this string self, int startIndex)
		{
			return self.Slice(startIndex, self.Length);
		}

		public static string Slice(this string self, int startIndex, int endIndex)
		{
			checked
			{
				startIndex = ((startIndex >= 0) ? startIndex : (startIndex + self.Length));
				endIndex = ((endIndex >= 0) ? endIndex : (endIndex + self.Length));
				return self.Substring(startIndex, Math.Max(0, endIndex - startIndex));
			}
		}

		public static string RemoveChar(this string self, char val)
		{
			return self.Replace(val.ToString(), "");
		}

		public static string Remove(this string self, string val)
		{
			return self.Replace(val, "");
		}

		public static string Join(this IEnumerable<string> self, string sep)
		{
			using IEnumerator<string> enumerator = self.GetEnumerator();
			enumerator.MoveNext();
			StringBuilder stringBuilder = new StringBuilder(enumerator.Current);
			while (enumerator.MoveNext())
			{
				stringBuilder.Append(sep);
				stringBuilder.Append(enumerator.Current);
			}
			return stringBuilder.ToString();
		}

		public static string Join(this IEnumerable<string> self, char sep)
		{
			return self.Join(sep.ToString() ?? "");
		}

		public static string Join(this IEnumerable<string> self)
		{
			using IEnumerator<string> enumerator = self.GetEnumerator();
			enumerator.MoveNext();
			StringBuilder stringBuilder = new StringBuilder(enumerator.Current);
			while (enumerator.MoveNext())
			{
				stringBuilder.Append(enumerator.Current);
			}
			return stringBuilder.ToString();
		}

		public static bool IsNullOrEmpty(this string self)
		{
			return string.IsNullOrEmpty(self);
		}

		public static bool IsNullOrWhiteSpace(this string self)
		{
			return string.IsNullOrWhiteSpace(self);
		}

		public static string FormatForLog(object message)
		{
			if (message is string result)
			{
				return result;
			}
			if (message is IConvertible convertible)
			{
				return convertible.ToString(CultureInfo.InvariantCulture);
			}
			if (message is IFormattable formattable)
			{
				return formattable.ToString(null, CultureInfo.InvariantCulture);
			}
			string text = message.ToString();
			if (!(text == message.GetType().ToString()))
			{
				return text;
			}
			return JsonConvert.SerializeObject(message);
		}
	}
	[PublicAPI]
	public sealed class TransformDelegate
	{
		public Transform Raw { get; private init; }

		public Vector2 Position2D
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				return Raw.position.AsVector2();
			}
			set
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				Raw.position = value.ToVector3(Raw.position.z);
			}
		}

		public float X
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return Raw.position.x;
			}
			set
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				Transform raw = Raw;
				Vector3 position = Raw.position;
				position.x = value;
				raw.position = position;
			}
		}

		public float Y
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return Raw.position.y;
			}
			set
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				Transform raw = Raw;
				Vector3 position = Raw.position;
				position.y = value;
				raw.position = position;
			}
		}

		public float Z
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return Raw.position.z;
			}
			set
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				Transform raw = Raw;
				Vector3 position = Raw.position;
				position.z = value;
				raw.position = position;
			}
		}

		public Vector2 LocalPosition2D
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				return Raw.localPosition.AsVector2();
			}
			set
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				Raw.localPosition = value.ToVector3(Raw.localPosition.z);
			}
		}

		public float LocalX
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return Raw.localPosition.x;
			}
			set
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				Transform raw = Raw;
				Vector3 localPosition = Raw.localPosition;
				localPosition.x = value;
				raw.localPosition = localPosition;
			}
		}

		public float LocalY
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return Raw.localPosition.y;
			}
			set
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				Transform raw = Raw;
				Vector3 localPosition = Raw.localPosition;
				localPosition.y = value;
				raw.localPosition = localPosition;
			}
		}

		public float LocalZ
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return Raw.localPosition.z;
			}
			set
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				Transform raw = Raw;
				Vector3 localPosition = Raw.localPosition;
				localPosition.z = value;
				raw.localPosition = localPosition;
			}
		}

		public Vector2 Scale2D
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				return Raw.localScale.AsVector2();
			}
			set
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				Raw.localScale = value.ToVector3(Raw.localScale.z);
			}
		}

		public float ScaleX
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return Raw.localScale.x;
			}
			set
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				Transform raw = Raw;
				Vector3 localScale = Raw.localScale;
				localScale.x = value;
				raw.localScale = localScale;
			}
		}

		public float ScaleY
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return Raw.localScale.y;
			}
			set
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				Transform raw = Raw;
				Vector3 localScale = Raw.localScale;
				localScale.y = value;
				raw.localScale = localScale;
			}
		}

		public float ScaleZ
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return Raw.localScale.z;
			}
			set
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				Transform raw = Raw;
				Vector3 localScale = Raw.localScale;
				localScale.z = value;
				raw.localScale = localScale;
			}
		}

		public float Rotation2D
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return Raw.localEulerAngles.z;
			}
			set
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				Transform raw = Raw;
				Vector3 localEulerAngles = Raw.localEulerAngles;
				localEulerAngles.z = value;
				raw.localEulerAngles = localEulerAngles;
			}
		}

		public TransformDelegate(Transform tf)
		{
			Raw = tf;
		}
	}
	[PublicAPI]
	public static class VectorUtil
	{
		public static (float x, float y, float z) ToTuple(this Vector3 self)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return (x: self.x, y: self.y, z: self.z);
		}

		public static (float x, float y) ToTuple(this Vector2 self)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return (x: self.x, y: self.y);
		}

		public static Vector2 AsVector2(this Vector3 self)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return new Vector2(self.x, self.y);
		}

		public static Vector3 ToVector3(this Vector2 self, float z)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(self.x, self.y, z);
		}
	}
}
namespace Osmi.Utils.Tap
{
	[PublicAPI]
	public static class TapUtil
	{
		public static T Tap<T>(this T self, Action<T> f)
		{
			f(self);
			return self;
		}

		public static U Thru<T, U>(this T self, Func<T, U> f)
		{
			return f(self);
		}

		public static T CondTap<T>(this T self, Func<T, bool> predicate, Action<T> f)
		{
			if (predicate(self))
			{
				f(self);
			}
			return self;
		}

		public static void Discard<T>(this T self)
		{
		}

		public static Func<T, Action<T>, T> TapFunc<T>()
		{
			return (T t, Action<T> f) => t.Tap(f);
		}

		public static Func<Action<T>, T> TapFunc<T>(this T self)
		{
			return TapFunc<T>().Bind1(self);
		}

		public static Func<T, T> TapFunc<T>(Action<T> f)
		{
			return TapFunc<T>().Bind2<T, Action<T>, T>(f);
		}
	}
}
namespace Osmi.SimpleFSM
{
	internal static class PMFSMEventForward
	{
		internal static void ForwardPMFSMComponentBroadcastEvent(orig_BroadcastEvent_FsmEvent orig, FsmEvent fsmEvent)
		{
			orig.Invoke(fsmEvent);
			SimpleFSM.BroadcastPMFSMEvent(fsmEvent);
		}

		internal static void ForwardPMFSMUtilSendToGameObject(orig_SendEventToGameObject_GameObject_FsmEvent_bool orig, GameObject go, FsmEvent ev, bool isRecursive)
		{
			orig.Invoke(go, ev, isRecursive);
			SimpleFSM.SendPMFSMEventToGameObject(go, ev);
		}

		internal static void ForwardPMFSMBroadcastEvent(orig_BroadcastEvent_FsmEvent_bool orig, Fsm self, FsmEvent fsmEvent, bool excludeSelf)
		{
			orig.Invoke(self, fsmEvent, excludeSelf);
			SimpleFSM.BroadcastPMFSMEvent(fsmEvent);
		}

		internal static void ForwardPMFSMBroadcastEventToGameObject(orig_BroadcastEventToGameObject_GameObject_FsmEvent_FsmEventData_bool_bool orig, Fsm self, GameObject go, FsmEvent fsmEvent, FsmEventData eventData, bool sendToChildren, bool excludeSelf)
		{
			orig.Invoke(self, go, fsmEvent, eventData, sendToChildren, excludeSelf);
			SimpleFSM.SendPMFSMEventToGameObject(go, fsmEvent);
		}

		internal static void ForwardPMFSMSendEventToFsmOnGameObject(orig_SendEventToFsmOnGameObject_GameObject_string_FsmEvent orig, Fsm self, GameObject gameObject, string fsmName, FsmEvent fsmEvent)
		{
			orig.Invoke(self, gameObject, fsmName, fsmEvent);
			SimpleFSM.SendPMFSMEventToGameObject(gameObject, fsmName, fsmEvent);
		}
	}
	[AttributeUsage(AttributeTargets.Method)]
	[PublicAPI]
	[MeansImplicitUse]
	public sealed class FSMStateAttribute : Attribute
	{
	}
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	[PublicAPI]
	public sealed class FSMGlobalTransitionAttribute : Attribute
	{
		public string OnEvent { get; private init; }

		public FSMGlobalTransitionAttribute(string onEvent)
		{
			OnEvent = onEvent;
		}
	}
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	[PublicAPI]
	public sealed class FSMTransitionAttribute : Attribute
	{
		public string OnEvent { get; private init; }

		public string ToState { get; private init; }

		public FSMTransitionAttribute(string onEvent, string toState)
		{
			OnEvent = onEvent;
			ToState = toState;
		}
	}
	[PublicAPI]
	public static class SimpleFSMExtensions
	{
		public const string PROXY_PMFSM_NAME = "SimpleFSM Event Proxy";

		public static IEnumerable<SimpleFSM> LocateSimpleFSMs(this GameObject self, string name)
		{
			return self.GetComponents<SimpleFSM>().Filter((SimpleFSM fsm) => fsm.Name == name);
		}

		public static SimpleFSM? LocateSimpleFSM(this GameObject self, string name)
		{
			return self.LocateSimpleFSMs(name).FirstOrDefault();
		}

		public static void SendSimpleFSMEvent(this GameObject self, string eventName, bool includeInactive = false)
		{
			SimpleFSM.SendEventToGameObject(self, eventName, includeInactive);
		}

		public static void SendSimpleFSMEvent(this GameObject self, string fsmName, string eventName, bool includeInactive = false)
		{
			SimpleFSM.SendEventToGameObject(self, fsmName, eventName, includeInactive);
		}

		public static PlayMakerFSM CreatePlayMakerFSMEventProxy(this GameObject self, string fsmEvent)
		{
			return self.CreatePlayMakerFSMEventProxy(FsmEvent.GetFsmEvent(fsmEvent));
		}

		public static PlayMakerFSM CreatePlayMakerFSMEventProxy(this GameObject self, FsmEvent fsmEvent)
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Expected O, but got Unknown
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Expected O, but got Unknown
			string name = fsmEvent.Name;
			PlayMakerFSM val = FSMUtility.LocateMyFSM(self, "SimpleFSM Event Proxy");
			if (val == null)
			{
				val = self.AddComponent<PlayMakerFSM>();
				val.FsmName = "SimpleFSM Event Proxy";
			}
			Fsm fsm = val.Fsm;
			checked
			{
				if (fsm.GetState(name) == null)
				{
					FsmState[] array = fsm.States;
					Array.Resize(ref array, array.Length + 1);
					FsmState[] array2 = array;
					int num = array.Length - 1;
					FsmState val2 = new FsmState(fsm);
					val2.Name = name;
					val2.Actions = (FsmStateAction[])(object)new FsmStateAction[1]
					{
						new SendSimpleFSMEventToGameObject(name)
					};
					FsmState toFsmState = (array2[num] = val2);
					fsm.States = array;
					FsmTransition[] array3 = fsm.GlobalTransitions;
					Array.Resize(ref array3, array3.Length + 1);
					array3[array3.Length - 1] = new FsmTransition
					{
						FsmEvent = fsmEvent,
						ToFsmState = toFsmState,
						ToState = name
					};
					fsm.GlobalTransitions = array3;
				}
				((Behaviour)val).enabled = true;
				return val;
			}
		}
	}
	[PublicAPI]
	public sealed class SimpleFSMInfo
	{
		internal static readonly LazyMap<Type, SimpleFSMInfo> map = new LazyMap<Type, SimpleFSMInfo>(New);

		private readonly Dictionary<string, FastReflectionDelegate> states;

		private readonly Dictionary<string, string> globalTransitions = new Dictionary<string, string>();

		private readonly Dictionary<string, IReadOnlyDictionary<string, string>> transitions = new Dictionary<string, IReadOnlyDictionary<string, string>>();

		internal IReadOnlyDictionary<string, FastReflectionDelegate> States => states;

		public IReadOnlyCollection<string> StateNames => states.Keys;

		public IReadOnlyDictionary<string, string> GlobalTransitions => globalTransitions;

		public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Transitions => transitions;

		private static SimpleFSMInfo New(Type t)
		{
			return new SimpleFSMInfo(t);
		}

		private SimpleFSMInfo(Type t)
		{
			ChildRef.Precompile(t);
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (t == typeof(SimpleFSM))
			{
				throw new InvalidOperationException("Cannot compile SimpleFSM base class");
			}
			if (t.IsAbstract)
			{
				throw new InvalidOperationException("Class " + t.FullName + " is abstract");
			}
			if (!t.IsSubclassOf(typeof(SimpleFSM)))
			{
				throw new InvalidOperationException("Class " + t.FullName + " is not SimpleFSM");
			}
			List<Exception> exceptions = new List<Exception>();
			MethodInfo[] array = t.GetRuntimeMethods().Filter((MethodInfo m) => Attribute.IsDefined(m, typeof(FSMStateAttribute))).Filter(delegate(MethodInfo m)
			{
				if (!typeof(IEnumerator).IsAssignableFrom(m.ReturnType))
				{
					exceptions.Add(new InvalidOperationException("Method " + m.Name + " in SimpleFSM class " + t.FullName + " is marked as state method but does not return an IEnumerator"));
					return false;
				}
				return true;
			})
				.ToArray();
			states = array.ToDictionary((MethodInfo m) => m.Name, (MethodInfo m) => FastReflectionHelper.GetFastDelegate(m, true));
			MethodInfo[] array2 = array;
			foreach (MethodInfo methodInfo in array2)
			{
				string name = methodInfo.Name;
				foreach (FSMGlobalTransitionAttribute customAttribute in methodInfo.GetCustomAttributes<FSMGlobalTransitionAttribute>())
				{
					if (!globalTransitions.TryAdd(customAttribute.OnEvent, name))
					{
						exceptions.Add(new InvalidOperationException("Duplicate global transition on event " + customAttribute.OnEvent + ", existed on state " + globalTransitions[customAttribute.OnEvent] + ", but found another one on state " + name));
					}
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				transitions[name] = dictionary;
				foreach (FSMTransitionAttribute customAttribute2 in methodInfo.GetCustomAttributes<FSMTransitionAttribute>())
				{
					if (!states.Keys.Contains<string>(customAttribute2.ToState))
					{
						exceptions.Add(new InvalidOperationException("Invalid state name " + customAttribute2.ToState + " on event " + customAttribute2.OnEvent + " found on state " + methodInfo.Name));
					}
					else if (!dictionary.TryAdd(customAttribute2.OnEvent, customAttribute2.ToState))
					{
						exceptions.Add(new InvalidOperationException("Duplicate transition on event " + customAttribute2.OnEvent + " to " + customAttribute2.ToState + " found on state " + methodInfo.Name + ", one to " + dictionary[customAttribute2.OnEvent] + " already exists"));
					}
				}
			}
			if (exceptions.Count == 1)
			{
				throw exceptions[0];
			}
			if (exceptions.Count > 1)
			{
				throw new AggregateException(exceptions);
			}
			Logger.LogDebug($"[SimpleFSM] Compiling {t.FullName} took {Time.realtimeSinceStartup - realtimeSinceStartup}s");
		}
	}
	[PublicAPI]
	public enum SimpleFSMState
	{
		Running,
		Waiting,
		Suspended,
		Disabled
	}
	[PublicAPI]
	public abstract class SimpleFSM : MonoBehaviour
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static hook_BroadcastEvent_FsmEvent <0>__ForwardPMFSMComponentBroadcastEvent;

			public static hook_SendEventToGameObject_GameObject_FsmEvent_bool <1>__ForwardPMFSMUtilSendToGameObject;

			public static hook_BroadcastEvent_FsmEvent_bool <2>__ForwardPMFSMBroadcastEvent;

			public static hook_BroadcastEventToGameObject_GameObject_FsmEvent_FsmEventData_bool_bool <3>__ForwardPMFSMBroadcastEventToGameObject;

			public static hook_SendEventToFsmOnGameObject_GameObject_string_FsmEvent <4>__ForwardPMFSMSendEventToFsmOnGameObject;
		}

		private Lazy<SimpleFSMInfo> info;

		private IEnumerator? currentState;

		private Coroutine? waitCoro;

		public const string FINISHED = "FINISHED";

		private static readonly object lockObj = new object();

		private static readonly HashSet<SimpleFSM> existingFSMs = new HashSet<SimpleFSM>();

		private static readonly HashSet<SimpleFSM> activeFSMs = new HashSet<SimpleFSM>();

		protected virtual bool AcceptPMFSMEvents => false;

		protected virtual bool RestartOnEnable => false;

		protected virtual bool InitChildRefsOnAwake => false;

		public Type Type { get; private set; }

		public string Name { get; private set; }

		public SimpleFSMInfo Info => info.Value;

		public SimpleFSMState State { get; private set; } = SimpleFSMState.Disabled;

		public string ActiveStateName { get; private set; } = "";

		public IReadOnlyDictionary<string, string> CurrentTransitions { get; private set; } = new Dictionary<string, string>();

		[FSMState]
		protected virtual IEnumerator Init()
		{
			return NoOp();
		}

		protected IEnumerator NoOp()
		{
			yield break;
		}

		protected void Iterate()
		{
			if (State != SimpleFSMState.Running)
			{
				return;
			}
			if (currentState == null || !currentState.MoveNext())
			{
				State = SimpleFSMState.Suspended;
				currentState = null;
				SendEvent("FINISHED");
				return;
			}
			object current = currentState.Current;
			if (current is string eventName)
			{
				SendEvent(eventName);
				return;
			}
			YieldInstruction val = (YieldInstruction)((current is YieldInstruction) ? current : null);
			if (val != null)
			{
				WaitFor(val);
				return;
			}
			if (current is IEnumerator inst)
			{
				WaitFor(inst);
				return;
			}
			if (current is ITuple { Length: 2 } tuple && tuple[0] is string eventName2)
			{
				object obj = tuple[1];
				YieldInstruction val2 = (YieldInstruction)((obj is YieldInstruction) ? obj : null);
				if (val2 != null)
				{
					SendEventDelayed(eventName2, val2);
					return;
				}
			}
			if (current is ITuple { Length: 2 } tuple2 && tuple2[0] is string eventName3 && tuple2[1] is IEnumerator enumerator)
			{
				SendEventDelayed(eventName3, enumerator);
			}
		}

		protected void Iterate(int steps)
		{
			if (steps <= 0)
			{
				throw new ArgumentOutOfRangeException("steps");
			}
			for (int i = 0; i < steps; i = checked(i + 1))
			{
				Iterate();
			}
		}

		public void SetState(string stateName, params object[] args)
		{
			if (!Info.States.TryGetValue(stateName, out FastReflectionDelegate value))
			{
				throw new ArgumentException("Invalid state name " + stateName);
			}
			if (State == SimpleFSMState.Running || State == SimpleFSMState.Waiting)
			{
				State = SimpleFSMState.Suspended;
			}
			if (waitCoro != null)
			{
				GlobalCoroutineExecutor.Stop(waitCoro);
				waitCoro = null;
			}
			currentState = value.Invoke((object)this, args) as IEnumerator;
			CurrentTransitions = Info.Transitions[stateName];
			ActiveStateName = stateName;
			if (State == SimpleFSMState.Suspended)
			{
				State = SimpleFSMState.Running;
			}
		}

		public void Reset()
		{
			SetState("Init");
		}

		public void SendEvent(string eventName)
		{
			if (Info.GlobalTransitions.TryGetValue(eventName, out string value))
			{
				SetState(value);
			}
			else if (CurrentTransitions.TryGetValue(eventName, out value))
			{
				SetState(value);
			}
		}

		public void SendPMFSMEvent(FsmEvent fsmEvent)
		{
			if (AcceptPMFSMEvents)
			{
				SendEvent(fsmEvent.Name);
			}
		}

		public void SendEventDelayed(string eventName, float seconds)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			SendEventDelayed(eventName, (YieldInstruction)new WaitForSeconds(seconds));
		}

		public void SendEventDelayed(string eventName, YieldInstruction inst)
		{
			GlobalCoroutineExecutor.Start(SendEventDelayedCoroutine(eventName, inst));
		}

		public void SendEventDelayed(string eventName, IEnumerator enumerator)
		{
			GlobalCoroutineExecutor.Start(SendEventDelayedCoroutine(eventName, enumerator));
		}

		private IEnumerator SendEventDelayedCoroutine(string eventName, object obj)
		{
			yield return obj;
			SendEvent(eventName);
		}

		private void WaitFor(object inst)
		{
			State = SimpleFSMState.Waiting;
			waitCoro = GlobalCoroutineExecutor.Start(WaitCoroutine(this, inst));
		}

		private IEnumerator WaitCoroutine(SimpleFSM self, object inst)
		{
			yield return inst;
			if (!((Object)(object)self == (Object)null))
			{
				if (State == SimpleFSMState.Waiting)
				{
					State = SimpleFSMState.Running;
				}
				waitCoro = null;
			}
		}

		protected void Awake()
		{
			Type = ((object)this).GetType();
			Name = Type.Name;
			info = new Lazy<SimpleFSMInfo>(() => SimpleFSMInfo.map[Type]);
			lock (lockObj)
			{
				if (existingFSMs.Count == 0)
				{
					HookPMFSMEvents();
				}
				existingFSMs.Add(this);
			}
			if (InitChildRefsOnAwake)
			{
				((Component)(object)this).InitChildRefs();
			}
		}

		protected void Start()
		{
			if (!InitChildRefsOnAwake)
			{
				((Component)(object)this).InitChildRefs();
			}
			SetState("Init");
		}

		protected void OnEnable()
		{
			if (RestartOnEnable)
			{
				SetState("Init");
			}
			activeFSMs.Add(this);
			State = SimpleFSMState.Running;
		}

		protected void OnDisable()
		{
			State = SimpleFSMState.Disabled;
			if (waitCoro != null)
			{
				GlobalCoroutineExecutor.Stop(waitCoro);
				waitCoro = null;
			}
			activeFSMs.Remove(this);
		}

		protected void OnDestroy()
		{
			lock (lockObj)
			{
				existingFSMs.Remove(this);
				if (existingFSMs.Count == 0)
				{
					UnhookPMFSMEvents();
				}
			}
		}

		protected void Update()
		{
			Iterate();
		}

		public static void Precompile<T>() where T : SimpleFSM
		{
			_ = SimpleFSMInfo.map[typeof(T)];
		}

		public static void Precompile(Type t)
		{
			_ = SimpleFSMInfo.map[t];
		}

		public static void BroadcastEvent(string eventName, bool includeInactive = false)
		{
			foreach (SimpleFSM item in includeInactive ? existingFSMs : activeFSMs)
			{
				item.SendEvent(eventName);
			}
		}

		public static void BroadcastPMFSMEvent(FsmEvent fsmEvent, bool includeInactive = false)
		{
			foreach (SimpleFSM item in includeInactive ? existingFSMs : activeFSMs)
			{
				item.SendPMFSMEvent(fsmEvent);
			}
		}

		public static void BroadcastEvent(string fsmName, string eventName, bool includeInactive = false)
		{
			foreach (SimpleFSM item in includeInactive ? existingFSMs : activeFSMs)
			{
				if (item.Name == fsmName)
				{
					item.SendEvent(eventName);
				}
			}
		}

		public static void BroadcastPMFSMEvent(string fsmName, FsmEvent fsmEvent, bool includeInactive = false)
		{
			foreach (SimpleFSM item in includeInactive ? existingFSMs : activeFSMs)
			{
				if (item.Name == fsmName)
				{
					item.SendPMFSMEvent(fsmEvent);
				}
			}
		}

		public static void SendEventToGameObject(GameObject go, string eventName, bool includeInactive = false)
		{
			foreach (SimpleFSM item in includeInactive ? existingFSMs : activeFSMs)
			{
				if ((Object)(object)((Component)item).gameObject == (Object)(object)go)
				{
					item.SendEvent(eventName);
				}
			}
		}

		public static void SendPMFSMEventToGameObject(GameObject go, FsmEvent fsmEvent, bool includeInactive = false)
		{
			foreach (SimpleFSM item in includeInactive ? existingFSMs : activeFSMs)
			{
				if ((Object)(object)((Component)item).gameObject == (Object)(object)go)
				{
					item.SendPMFSMEvent(fsmEvent);
				}
			}
		}

		public static void SendEventToGameObject(GameObject go, string fsmName, string eventName, bool includeInactive = false)
		{
			foreach (SimpleFSM item in includeInactive ? existingFSMs : activeFSMs)
			{
				if ((Object)(object)((Component)item).gameObject == (Object)(object)go && item.Name == fsmName)
				{
					item.SendEvent(eventName);
				}
			}
		}

		public static void SendPMFSMEventToGameObject(GameObject go, string fsmName, FsmEvent fsmEvent, bool includeInactive = false)
		{
			foreach (SimpleFSM item in includeInactive ? existingFSMs : activeFSMs)
			{
				if ((Object)(object)((Component)item).gameObject == (Object)(object)go && item.Name == fsmName)
				{
					item.SendPMFSMEvent(fsmEvent);
				}
			}
		}

		private static void HookPMFSMEvents()
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
			object obj = <>O.<0>__ForwardPMFSMComponentBroadcastEvent;
			if (obj == null)
			{
				hook_BroadcastEvent_FsmEvent val = PMFSMEventForward.ForwardPMFSMComponentBroadcastEvent;
				<>O.<0>__ForwardPMFSMComponentBroadcastEvent = val;
				obj = (object)val;
			}
			PlayMakerFSM.BroadcastEvent_FsmEvent += (hook_BroadcastEvent_FsmEvent)obj;
			object obj2 = <>O.<1>__ForwardPMFSMUtilSendToGameObject;
			if (obj2 == null)
			{
				hook_SendEventToGameObject_GameObject_FsmEvent_bool val2 = PMFSMEventForward.ForwardPMFSMUtilSendToGameObject;
				<>O.<1>__ForwardPMFSMUtilSendToGameObject = val2;
				obj2 = (object)val2;
			}
			FSMUtility.SendEventToGameObject_GameObject_FsmEvent_bool += (hook_SendEventToGameObject_GameObject_FsmEvent_bool)obj2;
			object obj3 = <>O.<2>__ForwardPMFSMBroadcastEvent;
			if (obj3 == null)
			{
				hook_BroadcastEvent_FsmEvent_bool val3 = PMFSMEventForward.ForwardPMFSMBroadcastEvent;
				<>O.<2>__ForwardPMFSMBroadcastEvent = val3;
				obj3 = (object)val3;
			}
			Fsm.BroadcastEvent_FsmEvent_bool += (hook_BroadcastEvent_FsmEvent_bool)obj3;
			object obj4 = <>O.<3>__ForwardPMFSMBroadcastEventToGameObject;
			if (obj4 == null)
			{
				hook_BroadcastEventToGameObject_GameObject_FsmEvent_FsmEventData_bool_bool val4 = PMFSMEventForward.ForwardPMFSMBroadcastEventToGameObject;
				<>O.<3>__ForwardPMFSMBroadcastEventToGameObject = val4;
				obj4 = (object)val4;
			}
			Fsm.BroadcastEventToGameObject_GameObject_FsmEvent_FsmEventData_bool_bool += (hook_BroadcastEventToGameObject_GameObject_FsmEvent_FsmEventData_bool_bool)obj4;
			object obj5 = <>O.<4>__ForwardPMFSMSendEventToFsmOnGameObject;
			if (obj5 == null)
			{
				hook_SendEventToFsmOnGameObject_GameObject_string_FsmEvent val5 = PMFSMEventForward.ForwardPMFSMSendEventToFsmOnGameObject;
				<>O.<4>__ForwardPMFSMSendEventToFsmOnGameObject = val5;
				obj5 = (object)val5;
			}
			Fsm.SendEventToFsmOnGameObject_GameObject_string_FsmEvent += (hook_SendEventToFsmOnGameObject_GameObject_string_FsmEvent)obj5;
		}

		private static void UnhookPMFSMEvents()
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
			object obj = <>O.<0>__ForwardPMFSMComponentBroadcastEvent;
			if (obj == null)
			{
				hook_BroadcastEvent_FsmEvent val = PMFSMEventForward.ForwardPMFSMComponentBroadcastEvent;
				<>O.<0>__ForwardPMFSMComponentBroadcastEvent = val;
				obj = (object)val;
			}
			PlayMakerFSM.BroadcastEvent_FsmEvent -= (hook_BroadcastEvent_FsmEvent)obj;
			object obj2 = <>O.<1>__ForwardPMFSMUtilSendToGameObject;
			if (obj2 == null)
			{
				hook_SendEventToGameObject_GameObject_FsmEvent_bool val2 = PMFSMEventForward.ForwardPMFSMUtilSendToGameObject;
				<>O.<1>__ForwardPMFSMUtilSendToGameObject = val2;
				obj2 = (object)val2;
			}
			FSMUtility.SendEventToGameObject_GameObject_FsmEvent_bool -= (hook_SendEventToGameObject_GameObject_FsmEvent_bool)obj2;
			object obj3 = <>O.<2>__ForwardPMFSMBroadcastEvent;
			if (obj3 == null)
			{
				hook_BroadcastEvent_FsmEvent_bool val3 = PMFSMEventForward.ForwardPMFSMBroadcastEvent;
				<>O.<2>__ForwardPMFSMBroadcastEvent = val3;
				obj3 = (object)val3;
			}
			Fsm.BroadcastEvent_FsmEvent_bool -= (hook_BroadcastEvent_FsmEvent_bool)obj3;
			object obj4 = <>O.<3>__ForwardPMFSMBroadcastEventToGameObject;
			if (obj4 == null)
			{
				hook_BroadcastEventToGameObject_GameObject_FsmEvent_FsmEventData_bool_bool val4 = PMFSMEventForward.ForwardPMFSMBroadcastEventToGameObject;
				<>O.<3>__ForwardPMFSMBroadcastEventToGameObject = val4;
				obj4 = (object)val4;
			}
			Fsm.BroadcastEventToGameObject_GameObject_FsmEvent_FsmEventData_bool_bool -= (hook_BroadcastEventToGameObject_GameObject_FsmEvent_FsmEventData_bool_bool)obj4;
			object obj5 = <>O.<4>__ForwardPMFSMSendEventToFsmOnGameObject;
			if (obj5 == null)
			{
				hook_SendEventToFsmOnGameObject_GameObject_string_FsmEvent val5 = PMFSMEventForward.ForwardPMFSMSendEventToFsmOnGameObject;
				<>O.<4>__ForwardPMFSMSendEventToFsmOnGameObject = val5;
				obj5 = (object)val5;
			}
			Fsm.SendEventToFsmOnGameObject_GameObject_string_FsmEvent -= (hook_SendEventToFsmOnGameObject_GameObject_string_FsmEvent)obj5;
		}
	}
}
namespace Osmi.Imports
{
	internal static class EnemyHPBar
	{
		[ModImportName("EnemyHPBar")]
		private static class EnemyHPBarImport
		{
			public static Action<GameObject>? DisableHPBar;

			public static Action<GameObject>? EnableHPBar;

			public static Action<GameObject>? RefreshHPBar;

			public static Action<GameObject>? MarkAsBoss;
		}

		static EnemyHPBar()
		{
			ModInteropManager.ModInterop(typeof(EnemyHPBarImport));
		}

		internal static void DisableHPBar(this GameObject self)
		{
			EnemyHPBarImport.DisableHPBar?.Invoke(self);
		}

		internal static void EnableHPBar(this GameObject self)
		{
			EnemyHPBarImport.EnableHPBar?.Invoke(self);
		}

		internal static void RefreshHPBar(this GameObject self)
		{
			EnemyHPBarImport.RefreshHPBar?.Invoke(self);
		}

		internal static void MarkAsBoss(this GameObject self)
		{
			EnemyHPBarImport.MarkAsBoss?.Invoke(self);
		}
	}
}
namespace Osmi.Game
{
	[PublicAPI]
	public static class AudioUtil
	{
		public static AudioSource PlayOneShot(GameObject playerPrefab, Vector3 position, AudioClip clip, float pitchMin, float pitchMax, float volumn)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return PlayOneShot(playerPrefab, position, clip, Random.Range(pitchMin, pitchMax), volumn);
		}

		public static AudioSource PlayOneShot(GameObject playerPrefab, Vector3 position, AudioClip clip, float pitch, float volumn)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			AudioSource component = ObjectPoolExtensions.Spawn(playerPrefab, position, Quaternion.Euler(Vector3.up)).GetComponent<AudioSource>();
			component.pitch = pitch;
			component.volume = volumn;
			component.PlayOneShot(clip);
			return component;
		}

		public static AudioSource PlayOneShot(GameObject playerPrefab, Vector3 position, AudioClip clip, float pitch)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return PlayOneShot(playerPrefab, position, clip, pitch, 1f);
		}

		public static AudioSource PlayOneShot(GameObject playerPrefab, Vector3 position, AudioClip clip)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return PlayOneShot(playerPrefab, position, clip, 1f, 1f);
		}

		public static AudioSource PlayOneShot(this AudioClip clip, Vector3 position, float pitchMin, float pitchMax, float volumn)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return PlayOneShot(Prefabs.AudioPlayer, position, clip, Random.Range(pitchMin, pitchMax), volumn);
		}

		public static AudioSource PlayOneShot(this AudioClip clip, Vector3 position, float pitch, float volumn)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return PlayOneShot(Prefabs.AudioPlayer, position, clip, pitch, volumn);
		}

		public static AudioSource PlayOneShot(this AudioClip clip, Vector3 position, float pitch)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return PlayOneShot(Prefabs.AudioPlayer, position, clip, pitch, 1f);
		}

		public static AudioSource PlayOneShot(this AudioClip clip, Vector3 position)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return PlayOneShot(Prefabs.AudioPlayer, position, clip, 1f, 1f);
		}
	}
	public static class CameraShakeUtil
	{
		public static PlayMakerFSM CameraShakeFSM => Ref.GC.cameraShakeFSM;

		public static void Small()
		{
			CameraShakeFSM.SendEvent("SmallShake");
		}

		public static void Big()
		{
			CameraShakeFSM.SendEvent("BigShake");
		}

		public static void Huge()
		{
			CameraShakeFSM.SendEvent("HugeShake");
		}

		public static void SuperDash()
		{
			CameraShakeFSM.SendEvent("SuperDashShake");
		}

		public static void EnemyKill()
		{
			CameraShakeFSM.SendEvent("EnemyKillShake");
		}

		public static void Tram()
		{
			CameraShakeFSM.SendEvent("TramShake");
		}

		public static void Average()
		{
			CameraShakeFSM.SendEvent("AverageShake");
		}

		public static void Blizzard()
		{
			CameraShakeFSM.SendEvent("BlizzardShake");
		}

		public static void Cancel()
		{
			CameraShakeFSM.SendEvent("CANCEL SHAKE");
		}
	}
	[PublicAPI]
	public enum Charm
	{
		WaywardCompass = 2,
		GatheringSwarm = 1,
		StalwartShell = 4,
		SoulCatcher = 20,
		ShamanStone = 19,
		SoulEater = 21,
		Dashmaster = 31,
		Sprintmaster = 37,
		Grubsong = 3,
		GrubberflysElegy = 35,
		UnbreakableHeart = 23,
		UnbreakableGreed = 24,
		UnbreakableStrength = 25,
		SpellTwister = 33,
		SteadyBody = 14,
		HeavyBlow = 15,
		QuickSlash = 32,
		Longnail = 18,
		MarkOfPride = 13,
		FuryOfTheFallen = 6,
		ThornsOfAgony = 12,
		BaldurShell = 5,
		Flukenest = 11,
		DefendersCrest = 10,
		GlowingWomb = 22,
		QuickFocus = 7,
		DeepFocus = 34,
		LifebloodHeart = 8,
		LifebloodCore = 9,
		JonisBlessing = 27,
		Hiveblood = 29,
		SporeShroom = 17,
		SharpShadow = 16,
		ShapeOfUnn = 28,
		NailmastersGlory = 26,
		Weaversong = 39,
		DreamWielder = 30,
		Dreamshield = 38,
		Grimmchild = 40,
		VoidHeart = 36
	}
	[PublicAPI]
	public static class CharmUtil
	{
		public static GameObject CharmsPane => ((Component)Ref.GC).gameObject.Child("HudCamera", "Inventory", "Charms");

		public static bool CharmsPaneOpen => CharmsPane.Child("Cursor", "Back").activeInHierarchy;

		public static void UpdateCharm()
		{
			Ref.HC.CharmUpdate();
			PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");
			PlayMakerFSM.BroadcastEvent("UPDATE BLUE HEALTH");
			UpdateCharmUI();
			if (!Ref.PD.atBench)
			{
				PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
			}
		}

		public static bool GotCharm(int charm)
		{
			return Ref.PD.GetBool($"gotCharm_{charm}");
		}

		public static bool EquippedCharm(int charm)
		{
			return Ref.PD.GetBool($"equippedCharm_{charm}");
		}

		public static int GetCharmCost(int charm)
		{
			return Ref.PD.GetInt($"charmCost_{charm}");
		}

		public static bool EquipCharm(int charm)
		{
			if (EquippedCharm(charm) || PlayerData.instance.GetInt("charmSlotsFilled") >= PlayerData.instance.GetInt("charmSlots"))
			{
				return false;
			}
			PlayerData instance = PlayerData.instance;
			instance.SetIntSwappedArgs(checked(instance.GetInt("charmSlotsFilled") + GetCharmCost(charm)), "charmSlotsFilled");
			if (PlayerData.instance.GetInt("charmSlots") < PlayerData.instance.GetInt("charmSlotsFilled"))
			{
				PlayerData.instance.SetBoolSwappedArgs(true, "canOvercharm");
				PlayerData.instance.SetBoolSwappedArgs(true, "overcharmed");
			}
			Ref.PD.SetBool($"equippedCharm_{charm}", true);
			Ref.PD.EquipCharm(charm);
			return true;
		}

		public static bool UnequipCharm(int charm)
		{
			if (!EquippedCharm(charm))
			{
				return false;
			}
			PlayerData instance = PlayerData.instance;
			instance.SetIntSwappedArgs(checked(instance.GetInt("charmSlotsFilled") - GetCharmCost(charm)), "charmSlotsFilled");
			if (PlayerData.instance.GetBool("overcharmed") && PlayerData.instance.GetInt("charmSlotsFilled") <= PlayerData.instance.GetInt("charmSlots"))
			{
				PlayerData.instance.SetBoolSwappedArgs(false, "overcharmed");
			}
			Ref.PD.SetBool($"equippedCharm_{charm}", false);
			Ref.PD.UnequipCharm(charm);
			return true;
		}

		public static bool EquipCharms(params int[] charms)
		{
			return charms.Every((int charm) => EquipCharm(charm));
		}

		public static bool UnequipCharms(params int[] charms)
		{
			return charms.Every((int charm) => UnequipCharm(charm));
		}

		public static void UnequipAllCharms()
		{
			UnequipCharms(Ref.PD.GetVariable<List<int>>("equippedCharms").ToArray());
		}

		public static bool GotCharm(Charm charm)
		{
			return GotCharm((int)charm);
		}

		public static bool EquippedCharm(Charm charm)
		{
			return EquippedCharm((int)charm);
		}

		public static int GetCharmCost(Charm charm)
		{
			return GetCharmCost((int)charm);
		}

		public static bool EquipCharm(Charm charm)
		{
			return EquipCharm((int)charm);
		}

		public static bool UnequipCharm(Charm charm)
		{
			return UnequipCharm((int)charm);
		}

		public static bool EquipCharms(params Charm[] charms)
		{
			return EquipCharms(charms.Map((Charm i) => (int)i).ToArray());
		}

		public static bool UnequipCharms(params Charm[] charms)
		{
			return UnequipCharms(charms.Map((Charm i) => (int)i).ToArray());
		}

		public static void UpdateCharmUI()
		{
			if (!CharmsPaneOpen)
			{
				((Component)Ref.GC).gameObject.Child("HudCamera", "Hud Canvas", "Health", "OC Backboard").SetActive(PlayerData.instance.GetBool("overcharmed"));
				return;
			}
			GameObject charmsPane = CharmsPane;
			GameObject val = charmsPane.Child("Equipped Charms");
			GameObject val2 = val.Child("Text Equipped");
			GameObject val3 = val.Child("Text Overcharmed");
			GameObject val4 = val.Child("Next Dot");
			GameObject self = charmsPane.Child("Overcharm", "OC Backboard");
			GameObject val5 = ((Component)Ref.GC).gameObject.Child("HudCamera", "Hud Canvas", "Health", "OC Backboard");
			PlayMakerFSM val6 = FSMUtility.LocateMyFSM(charmsPane, "UI Charms");
			PlayMakerFSM val7 = FSMUtility.LocateMyFSM(val.Child("Notches", "Over Indicator"), "Over Control");
			val.Child("Charms").GetChildren().ForEach((Action<GameObject>)Object.Destroy);
			ReflectionHelper.CallMethod<BuildEquippedCharms>(val.GetComponent<BuildEquippedCharms>(), "BuildCharmList", Array.Empty<object>());
			FSMUtility.SendEventToGameObject(val, "UP", false);
			int num = checked(PlayerData.instance.GetInt("charmSlotsFilled") - PlayerData.instance.GetInt("charmSlots"));
			if (PlayerData.instance.GetBool("overcharmed"))
			{
				val3.SetActive(true);
				val2.SetActive(false);
				val7.FsmVariables.GetFsmInt("Cost").Value = num;
				val7.FsmVariables.GetFsmBool("Overcharmed").Value = true;
				val7.SendEvent("DISPLAY OVER");
				self.GetTransformDelegate().LocalY = -3.91f;
				val5.SetActive(true);
			}
			else
			{
				val3.SetActive(false);
				val2.SetActive(true);
				FSMUtility.SendEventToGameObject(val2, "UP", false);
				val7.SendEvent("OVERCHARM END");
				self.GetTransformDelegate().LocalY = -50f;
				val5.SetActive(false);
			}
			FSMUtility.SendEventToGameObject(charmsPane.Child("Collected Charms"), "UPDATE", true);
			if (num < 0)
			{
				val6.FsmVariables.GetFsmBool("Open Slot").Value = true;
				FSMUtility.SendEventToGameObject(val4, "NOTCH DEF UP", false);
			}
			else
			{
				val6.FsmVariables.GetFsmBool("Open Slot").Value = false;
				FSMUtility.SendEventToGameObject(val4, "NOTCH DOWN", false);
			}
		}
	}
	[PublicAPI]
	[RequireComponent(typeof(Rigidbody2D))]
	public class ChaseObject : MoveToPosition2D
	{
		private bool isNull = true;

		private Transform? tf;

		public override Vector2? TargetPos
		{
			get
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				if (!isNull)
				{
					return Target.position.AsVector2();
				}
				return null;
			}
			set
			{
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				if (value.HasValue && !isNull)
				{
					Extensions.SetPosition2D(Target, value.Value.x, value.Value.y);
				}
			}
		}

		public Transform? Target
		{
			get
			{
				return tf;
			}
			set
			{
				isNull = (Object)(object)value == (Object)null;
				tf = value;
			}
		}
	}
	[PublicAPI]
	public static class CoroutinePresets
	{
		public static IEnumerator FadeAudio(this AudioSource audio, float from, float to, float time)
		{
			if (time <= 0f)
			{
				throw new ArgumentOutOfRangeException("time");
			}
			if (from < 0f)
			{
				throw new ArgumentOutOfRangeException("from");
			}
			if (to < 0f)
			{
				throw new ArgumentOutOfRangeException("to");
			}
			for (float elapsedTime = 0f; elapsedTime <= time; elapsedTime += Time.deltaTime)
			{
				audio.volume = Mathf.Lerp(from, to, elapsedTime / time);
				yield return null;
			}
		}

		public static IEnumerator FadeAudio(this AudioSource audio, float to, float time)
		{
			return audio.FadeAudio(audio.volume, to, time);
		}

		public static IEnumerator WaitForPlayFinished(this tk2dSpriteAnimator animator, string clip)
		{
			animator.Play(clip);
			yield return (object)new WaitWhile((Func<bool>)(() => animator.IsPlaying(clip)));
		}

		public static IEnumerator Move2DInTime(this Transform tf, Vector2 from, Vector2 to, float time)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			if (time <= 0f)
			{
				throw new ArgumentOutOfRangeException("time");
			}
			float fromX = from.x;
			float fromY = from.y;
			float toX = to.x;
			float toY = to.y;
			for (float elapsedTime = 0f; elapsedTime < time; elapsedTime += Time.deltaTime)
			{
				Extensions.SetPosition2D(tf, Mathf.Lerp(fromX, toX, elapsedTime / time), Mathf.Lerp(fromY, toY, elapsedTime / time));
				yield return null;
			}
		}
	}
	[PublicAPI]
	public class CustomBehaviour : MonoBehaviour
	{
		public event Action<CustomBehaviour> StartEvent;

		public event Action<CustomBehaviour> OnEnableEvent;

		public event Action<CustomBehaviour> OnDisableEvent;

		public event Action<CustomBehaviour> OnDestroyEvent;

		public event Action<CustomBehaviour> UpdateEvent;

		public event Action<CustomBehaviour> FixedUpdateEvent;

		protected void Start()
		{
			StartEvent?.Invoke(this);
		}

		protected void OnEnable()
		{
			OnEnableEvent?.Invoke(this);
		}

		protected void OnDisable()
		{
			OnDisableEvent?.Invoke(this);
		}

		protected void OnDestroy()
		{
			OnDestroyEvent?.Invoke(this);
		}

		protected void Update()
		{
			UpdateEvent?.Invoke(this);
		}

		protected void FixedUpdate()
		{
			FixedUpdateEvent?.Invoke(this);
		}
	}
	[PublicAPI]
	public class CustomDreamnailReaction : MonoBehaviour
	{
		private Action<Collider2D>? method;

		public void SetMethod(Action<Collider2D> action)
		{
			method = action;
		}

		public void SetMethod(Action<CustomDreamnailReaction, Collider2D> action)
		{
			method = action.Bind1(this);
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (((Component)collision).CompareTag("Dream Attack"))
			{
				method?.Invoke(collision);
			}
		}
	}
	[PublicAPI]
	public class CustomHitEffectReceiver : MonoBehaviour, IHitEffectReciever
	{
		private Action<float>? method;

		public void SetMethod(Action<float> action)
		{
			method = action;
		}

		public void SetMethod(Action<CustomHitEffectReceiver, float> action)
		{
			method = action.Bind1(this);
		}

		public void RecieveHitEffect(float attackDirection)
		{
			method?.Invoke(attackDirection);
		}
	}
	public class DestroyOnDisable : MonoBehaviour
	{
		public bool recycle;

		protected void OnDisable()
		{
			if (recycle)
			{
				ObjectPoolExtensions.Recycle(((Component)this).gameObject);
			}
			else
			{
				Object.Destroy((Object)(object)((Component)this).gameObject);
			}
		}
	}
	[RequireComponent(typeof(Collider2D))]
	public class EnemyDetector : MonoBehaviour
	{
		private readonly HashSet<Collider2D> colliders = new HashSet<Collider2D>();

		public IEnumerable<GameObject> Enemies
		{
			get
			{
				colliders.RemoveWhere((Collider2D c) => !IsValidEnemy(c));
				return colliders.Map((Collider2D c) => ((Component)c).gameObject).Distinct();
			}
		}

		public HealthManager? GetTarget()
		{
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			colliders.RemoveWhere((Collider2D c) => !IsValidEnemy(c));
			foreach (Collider2D item in colliders.OrderBy(delegate(Collider2D c)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				Vector3 val = ((Component)this).transform.position - ((Component)c).transform.position;
				return ((Vector3)(ref val)).sqrMagnitude;
			}))
			{
				HealthManager component = ((Component)item).GetComponent<HealthManager>();
				if (!component.IsInvincible && !RaycastHit2D.op_Implicit(Physics2D.Linecast(Vector2.op_Implicit(((Component)this).transform.position), Vector2.op_Implicit(((Component)component).transform.position), Layer.Terrain.GetMask())))
				{
					return component;
				}
			}
			return null;
		}

		private void OnTriggerEnter2D(Collider2D otherCollider)
		{
			if (IsValidEnemy(otherCollider))
			{
				colliders.Add(otherCollider);
			}
		}

		private void OnTriggerExit2D(Collider2D otherCollider)
		{
			colliders.Remove(otherCollider);
		}

		public static bool IsValidEnemy(GameObject go)
		{
			if (go.layer == 11)
			{
				HealthManager component = go.GetComponent<HealthManager>();
				if (component != null)
				{
					return IsValidEnemy(component);
				}
			}
			return false;
		}

		public static bool IsValidEnemy(Collider2D collider)
		{
			if (!collider.isTrigger)
			{
				return IsValidEnemy(((Component)collider).gameObject);
			}
			return false;
		}

		public static bool IsValidEnemy(HealthManager hm)
		{
			if (hm != null && ((Behaviour)hm).isActiveAndEnabled && hm.hp > 0)
			{
				return !hm.isDead;
			}
			return false;
		}
	}
	[PublicAPI]
	public sealed class GlobalCoroutineExecutor : MonoBehaviour
	{
		private static readonly GlobalCoroutineExecutor instance = GameObjectUtil.CreateHolder<GlobalCoroutineExecutor>("Global Coroutine Executor");

		public static Coroutine Start(IEnumerator enumerator)
		{
			return ((MonoBehaviour)instance).StartCoroutine(enumerator);
		}

		public static void Stop(IEnumerator enumerator)
		{
			((MonoBehaviour)instance).StopCoroutine(enumerator);
		}

		public static void Stop(Coroutine coroutine)
		{
			((MonoBehaviour)instance).StopCoroutine(coroutine);
		}

		internal static void StopAll()
		{
			((MonoBehaviour)instance).StopAllCoroutines();
		}

		public static Coroutine SetTimeOut(float timeOut, Action action)
		{
			return ((MonoBehaviour)(object)instance).SetTimeOut(timeOut, action);
		}

		public static Coroutine SetImmediate(Action action)
		{
			return ((MonoBehaviour)(object)instance).SetImmediate(action);
		}
	}
	[PublicAPI]
	public sealed class InSceneCoroutineExecutor : MonoBehaviour
	{
		private static readonly InSceneCoroutineExecutor instance;

		public static Coroutine Start(IEnumerator enumerator)
		{
			return ((MonoBehaviour)instance).StartCoroutine(enumerator);
		}

		public static void Stop(IEnumerator enumerator)
		{
			((MonoBehaviour)instance).StopCoroutine(enumerator);
		}

		public static void Stop(Coroutine coroutine)
		{
			((MonoBehaviour)instance).StopCoroutine(coroutine);
		}

		internal static void StopAll()
		{
			((MonoBehaviour)instance).StopAllCoroutines();
		}

		public static Coroutine SetTimeOut(float timeOut, Action action)
		{
			return ((MonoBehaviour)(object)instance).SetTimeOut(timeOut, action);
		}

		public static Coroutine SetImmediate(Action action)
		{
			return ((MonoBehaviour)(object)instance).SetImmediate(action);
		}

		static InSceneCoroutineExecutor()
		{
			instance = GameObjectUtil.CreateHolder<InSceneCoroutineExecutor>("In Scene Coroutine Executor");
			OsmiHooks.SceneChangeHook += delegate
			{
				StopAll();
			};
		}
	}
	public class KeyButtonActionSet : PlayerActionSet
	{
		public readonly PlayerAction key;

		public readonly PlayerAction button;

		public bool IsPressed
		{
			get
			{
				if (!((OneAxisInputControl)key).IsPressed)
				{
					return ((OneAxisInputControl)button).IsPressed;
				}
				return true;
			}
		}

		public bool WasPressed
		{
			get
			{
				if (!((OneAxisInputControl)key).WasPressed)
				{
					return ((OneAxisInputControl)button).WasPressed;
				}
				return true;
			}
		}

		public bool WasReleased
		{
			get
			{
				if (!((OneAxisInputControl)key).WasReleased)
				{
					return ((OneAxisInputControl)button).WasReleased;
				}
				return true;
			}
		}

		public bool WasRepeated
		{
			get
			{
				if (!((OneAxisInputControl)key).WasRepeated)
				{
					return ((OneAxisInputControl)button).WasRepeated;
				}
				return true;
			}
		}

		public KeyButtonActionSet()
		{
			key = ((PlayerActionSet)this).CreatePlayerAction("key");
			button = ((PlayerActionSet)this).CreatePlayerAction("button");
		}
	}
	[PublicAPI]
	public enum Layer
	{
		Default = 0,
		TransparentFX = 1,
		IgnoreRaycast = 2,
		Water = 4,
		UI = 5,
		Terrain = 8,
		Player = 9,
		TransitionGates = 10,
		Enemies = 11,
		Projectiles = 12,
		HeroDetector = 13,
		TerrainDetector = 14,
		EnemyDetector = 15,
		Tinker = 16,
		Attack = 17,
		Particle = 18,
		InteractiveObject = 19,
		HeroBox = 20,
		Grass = 21,
		EnemyAttack = 22,
		DamageAll = 23,
		Bouncer = 24,
		SoftTerrain = 25,
		Corpse = 26,
		uGUI = 27,
		HeroOnly = 28,
		ActiveRegion = 29,
		MapPin = 30,
		OrbitShield = 31
	}
	[PublicAPI]
	public static class LayerUtil
	{
		public static int GetMask(this Layer layer)
		{
			return 1 << (int)layer;
		}

		public static int GetMask(params Layer[] layers)
		{
			int num = 0;
			for (int i = 0; i < layers.Length; i = checked(i + 1))
			{
				num |= 1 << (int)layers[i];
			}
			return num;
		}
	}
	[PublicAPI]
	[RequireComponent(typeof(Rigidbody2D))]
	public class MoveToPosition2D : MonoBehaviour
	{
		[ChildRef("")]
		private readonly Rigidbody2D rb;

		public float accelerationForce;

		public float maxVelocity;

		public virtual Vector2? TargetPos { get; set; }

		private void Start()
		{
			((Component)(object)this).InitChildRefs();
		}

		private void FixedUpdate()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			if (TargetPos.HasValue && (int)rb.bodyType == 0)
			{
				Vector2 val = TargetPos.Value - ((Component)this).transform.position.AsVector2();
				if (((Vector2)(ref val)).magnitude != 0f)
				{
					rb.AddForce(((Vector2)(ref val)).normalized * accelerationForce);
					rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
				}
			}
		}
	}
	[PublicAPI]
	public static class Prefabs
	{
		private static readonly Dictionary<string, GameObject?> prefabs;

		public static GameObject AudioPlayer => Find("Audio Player Actor");

		public static GameObject RoarWaveEmitter => Find("Roar Wave Emitter");

		public static GameObject Shockwave => Find("Shockwave Wave Z");

		public static GameObject? Find(string name)
		{
			if (!prefabs.TryGetValue(name, out GameObject value))
			{
				return prefabs[name] = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(delegate(GameObject go)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					Scene scene = go.scene;
					return !((Scene)(ref scene)).IsValid() && ((Object)go).name == name;
				});
			}
			return value;
		}

		public static GameObject?[] Find(params string[] names)
		{
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			GameObject[] array = (GameObject[])(object)new GameObject[names.Length];
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			for (int i = 0; i < names.Length; i = checked(i + 1))
			{
				if (prefabs.TryGetValue(names[i], out GameObject value))
				{
					array[i] = value;
				}
				else if (!dictionary.TryAdd(names[i], i))
				{
					throw new ArgumentException("names", "Redundant values");
				}
			}
			if (dictionary.Count == 0)
			{
				return array;
			}
			GameObject[] array2 = Resources.FindObjectsOfTypeAll<GameObject>();
			foreach (GameObject val in array2)
			{
				Scene scene = val.scene;
				if (((Scene)(ref scene)).IsValid())
				{
					continue;
				}
				string name = ((Object)val).name;
				if (dictionary.ContainsKey(name))
				{
					int num = dictionary[name];
					GameObject val2 = (prefabs[name] = val);
					array[num] = val2;
					dictionary.Remove(name);
					if (dictionary.Count == 0)
					{
						return array;
					}
				}
			}
			foreach (string key in dictionary.Keys)
			{
				prefabs[key] = null;
			}
			return array;
		}

		public static void ClearCache(string name)
		{
			prefabs.Remove(name);
		}

		public static void ClearCache(params string[] names)
		{
			names.ForEach(delegate(string name)
			{
				prefabs.Remove(name);
			});
		}

		public static void ClearCahce()
		{
			prefabs.Clear();
		}

		static Prefabs()
		{
			prefabs = new Dictionary<string, GameObject>();
			Find("Audio Player Actor", "Roar Wave Emitter", "Shockwave Wave Z");
		}
	}
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	public sealed class PreloadAttribute : Attribute
	{
		public string SceneName { get; private init; }

		public string Path { get; private init; }

		public PreloadAttribute(string sceneName, string path)
		{
			SceneName = sceneName;
			Path = path;
		}
	}
	public static class PreloadUtil
	{
		private static readonly LazyMap<Type, (string sceneName, string path, Action<GameObject> setter)[]> entries;

		public static List<(string, string)> GetPreloadNames<T>()
		{
			return GetPreloadsNames(typeof(T));
		}

		public static List<(string, string)> GetPreloadsNames(Type type)
		{
			return entries[type].Map(((string sceneName, string path, Action<GameObject> setter) tuple) => (sceneName: tuple.sceneName, path: tuple.path)).ToList();
		}

		public static void SavePreloads<T>(Dictionary<string, Dictionary<string, GameObject>>? preloadedObjects)
		{
			SavePreloads(typeof(T), preloadedObjects);
		}

		public static void SavePreloads(Type type, Dictionary<string, Dictionary<string, GameObject>>? preloadedObjects)
		{
			if (preloadedObjects != null)
			{
				entries[type].ForEach(delegate((string sceneName, string path, Action<GameObject> setter) tuple)
				{
					tuple.setter(preloadedObjects[tuple.sceneName][tuple.path]);
				});
				entries.Remove(type);
			}
		}

		static PreloadUtil()
		{
			entries = new LazyMap<Type, (string, string, Action<GameObject>)[]>((Type t) => t.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Filter((FieldInfo fi) => Attribute.IsDefined(fi, typeof(PreloadAttribute))).Filter(delegate(FieldInfo fi)
			{
				if (fi.IsLiteral)
				{
					Logger.LogError("[PreloadUtil] Preload field " + fi.Name + " on type " + t.FullName + " is const");
					return false;
				}
				if (fi.FieldType != typeof(GameObject))
				{
					Logger.LogError("[PreloadUtil] Type of preload field " + fi.Name + " on type " + t.FullName + " is not GameObject");
					return false;
				}
				return true;
			})
				.Map(delegate(FieldInfo fi)
				{
					PreloadAttribute customAttribute = fi.GetCustomAttribute<PreloadAttribute>();
					return (SceneName: customAttribute.SceneName, Path: customAttribute.Path, fi.GetFastStaticSetter<GameObject>());
				})
				.ToArray());
			OsmiHooks.GameInitializedHook += entries.Clear;
		}
	}
	[PublicAPI]
	public abstract class AbstractRandomSelector<T>
	{
		public sealed record SelectionItem(T Value, float Weight, int ComboMax, int MissedMax)
		{
			internal Box<int> Combo { get; init; }

			internal Box<int> Missed { get; init; }

			public SelectionItem(T value)
				: this(value, 1f)
			{
			}

			public SelectionItem(T value, float weight)
				: this(value, weight, 0)
			{
			}

			public SelectionItem(T value, float weight, int comboMax)
				: this(value, weight, comboMax, 0)
			{
			}
		}

		public abstract void Add(SelectionItem item);

		public abstract bool Remove(SelectionItem item);

		public abstract void Clear();

		protected abstract IEnumerable<SelectionItem> GetItems();

		public T Select()
		{
			SelectionItem[] array = GetItems().ToArray();
			if (array.Length == 0)
			{
				throw new InvalidOperationException("Selection item list is empty");
			}
			float num = array.Aggregate(0f, (float w, SelectionItem i) => w + i.Weight);
			checked
			{
				for (int num2 = 0; num2 < 100; num2++)
				{
					float rng = Random.Range(0f, num);
					SelectionItem selectionItem = array.FirstOrDefault(delegate(SelectionItem i)
					{
						if (rng < i.Weight)
						{
							return true;
						}
						rng -= i.Weight;
						return false;
					});
					if (selectionItem == null)
					{
						continue;
					}
					SelectionItem selectionItem2 = array.LastOrDefault((SelectionItem item) => item.Missed.Value >= item.MissedMax && item.MissedMax > 0);
					if ((object)selectionItem2 != null)
					{
						array.ForEach(delegate(SelectionItem item)
						{
							item.Combo.Value = 0;
							item.Missed.Value++;
						});
						selectionItem2.Combo.Value = 1;
						selectionItem2.Missed.Value = 0;
						return selectionItem2.Value;
					}
					if (selectionItem.Combo.Value < selectionItem.ComboMax || selectionItem.ComboMax <= 0)
					{
						int value = selectionItem.Combo.Value + 1;
						array.ForEach(delegate(SelectionItem item)
						{
							item.Combo.Value = 0;
							item.Missed.Value++;
						});
						selectionItem.Combo.Value = value;
						selectionItem.Missed.Value = 0;
						return selectionItem.Value;
					}
				}
				return array[0].Value;
			}
		}
	}
	[PublicAPI]
	public class RandomSelector<T> : AbstractRandomSelector<T>, IEnumerable<AbstractRandomSelector<T>.SelectionItem>, IEnumerable
	{
		private readonly List<SelectionItem> items = new List<AbstractRandomSelector<T>.SelectionItem>();

		public override void Add(SelectionItem item)
		{
			items.Add(item);
		}

		public override bool Remove(SelectionItem item)
		{
			return items.Remove(item);
		}

		public override void Clear()
		{
			items.Clear();
		}

		protected override IEnumerable<SelectionItem> GetItems()
		{
			return items;
		}

		public IEnumerator<SelectionItem> GetEnumerator()
		{
			return items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return items.GetEnumerator();
		}
	}
	[PublicAPI]
	public class DynamicRandomSelector<T> : AbstractRandomSelector<T>, IEnumerable<DynamicRandomSelector<T>.DynamicSelectionItem>, IEnumerable
	{
		public sealed record DynamicSelectionItem(T Value, Func<float> Weight, Func<int> ComboMax, Func<int> MissedMax)
		{
			internal Box<int> Combo { get; init; }

			internal Box<int> Missed { get; init; }

			public DynamicSelectionItem(T value)
				: this(value, Funcs.Constant(1f))
			{
			}

			public DynamicSelectionItem(T value, Func<float> weight)
				: this(value, weight, Funcs.Constant(0))
			{
			}

			public DynamicSelectionItem(T value, Func<float> weight, Func<int> comboMax)
				: this(value, weight, comboMax, Funcs.Constant(0))
			{
			}

			public DynamicSelectionItem(SelectionItem item)
				: this(item.Value, Funcs.Constant(item.Weight), Funcs.Constant(item.ComboMax), Funcs.Constant(item.MissedMax))
			{
				Combo = item.Combo;
				Missed = item.Missed;
			}

			public SelectionItem Pin()
			{
				return new AbstractRandomSelector<T>.SelectionItem(Value, Weight(), ComboMax(), MissedMax())
				{
					Combo = Combo,
					Missed = Missed
				};
			}
		}

		private readonly List<DynamicSelectionItem> items = new List<DynamicSelectionItem>();

		public override void Add(SelectionItem item)
		{
			items.Add(new DynamicSelectionItem(item));
		}

		public override bool Remove(SelectionItem item)
		{
			return false;
		}

		public void Add(DynamicSelectionItem item)
		{
			items.Add(item);
		}

		public bool Remove(DynamicSelectionItem item)
		{
			return items.Remove(item);
		}

		public override void Clear()
		{
			items.Clear();
		}

		protected override IEnumerable<SelectionItem> GetItems()
		{
			return items.Map((DynamicSelectionItem i) => i.Pin());
		}

		public IEnumerator<DynamicSelectionItem> GetEnumerator()
		{
			return items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return items.GetEnumerator();
		}
	}
	[PublicAPI]
	public class SharedHealthManager : MonoBehaviour
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static hook_TakeDamage <0>__ReportDamage;

			public static hook_ApplyExtraDamage <1>__ReportExtraDamage;
		}

		internal const string SHM_FIELD = "sharedHealthManager";

		internal const string BACKED_SHM_FIELD = "backingSharedHealthManager";

		private readonly List<HealthManager> HMs;

		private static readonly object lockObj = new object();

		private static int instanceCount;

		public HealthManager BackingHM { get; private init; }

		public IEnumerable<HealthManager> Members => HMs;

		public int HP
		{
			get
			{
				return BackingHM.hp;
			}
			set
			{
				BackingHM.hp = value;
			}
		}

		public static int InstanceCount
		{
			get
			{
				return instanceCount;
			}
			private set
			{
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Expected O, but got Unknown
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Expected O, but got Unknown
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0084: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Expected O, but got Unknown
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Expected O, but got Unknown
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				lock (lockObj)
				{
					if (value == 0)
					{
						object obj = <>O.<0>__ReportDamage;
						if (obj == null)
						{
							hook_TakeDamage val = ReportDamage;
							<>O.<0>__ReportDamage = val;
							obj = (object)val;
						}
						HealthManager.TakeDamage -= (hook_TakeDamage)obj;
						object obj2 = <>O.<1>__ReportExtraDamage;
						if (obj2 == null)
						{
							hook_ApplyExtraDamage val2 = ReportExtraDamage;
							<>O.<1>__ReportExtraDamage = val2;
							obj2 = (object)val2;
						}
						HealthManager.ApplyExtraDamage -= (hook_ApplyExtraDamage)obj2;
					}
					else if (instanceCount == 0 && value > 0)
					{
						object obj3 = <>O.<0>__ReportDamage;
						if (obj3 == null)
						{
							hook_TakeDamage val3 = ReportDamage;
							<>O.<0>__ReportDamage = val3;
							obj3 = (object)val3;
						}
						HealthManager.TakeDamage += (hook_TakeDamage)obj3;
						object obj4 = <>O.<1>__ReportExtraDamage;
						if (obj4 == null)
						{
							hook_ApplyExtraDamage val4 = ReportExtraDamage;
							<>O.<1>__ReportExtraDamage = val4;
							obj4 = (object)val4;
						}
						HealthManager.ApplyExtraDamage += (hook_ApplyExtraDamage)obj4;
					}
					instanceCount = value;
				}
			}
		}

		public static SharedHealthManager Create(string name = "SharedHealthManager", int initialHP = 0)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			SharedHealthManager component = new GameObject(name, new Type[1] { typeof(SharedHealthManager) }).GetComponent<SharedHealthManager>();
			checked
			{
				component.HP += initialHP;
				return component;
			}
		}

		public static SharedHealthManager Create(string name, int initialHP, IEnumerable<HealthManager> hms)
		{
			SharedHealthManager sharedHealthManager = Create(name, initialHP);
			sharedHealthManager.Add(hms);
			return sharedHealthManager;
		}

		public static SharedHealthManager Create(string name, IEnumerable<HealthManager> hms)
		{
			return Create(name, 0, hms);
		}

		public static SharedHealthManager Create(string name, params HealthManager[] hms)
		{
			return Create(name, hms.AsEnumerable());
		}

		private SharedHealthManager()
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Expected O, but got Unknown
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			HMs = new List<HealthManager>();
			((MonoBehaviour)this)..ctor();
			checked
			{
				InstanceCount++;
				((Component)this).gameObject.layer = 11;
				BackingHM = ((Component)this).gameObject.AddComponent<HealthManager>();
				BackingHM.OnDeath += new DeathEvent(KillMembers);
				new DynamicData((object)BackingHM).Set("backingSharedHealthManager", (object)this);
				((Component)this).gameObject.MarkAsBoss();
				if (ModHooks.GetMod("DebugMod", true, false) != null)
				{
					((Component)this).gameObject.AddComponent<NonBouncer>();
					((Component)this).gameObject.AddComponent<BoxCollider2D>().size = new Vector2(32767f, 32767f);
				}
			}
		}

		~SharedHealthManager()
		{
			checked
			{
				try
				{
					InstanceCount--;
				}
				finally
				{
					((object)this).Finalize();
				}
			}
		}

		public void OnDestroy()
		{
			Die();
		}

		public void Add(HealthManager hm)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			if ((Object)(object)hm == (Object)(object)BackingHM)
			{
				throw new InvalidOperationException("Cannot add own backing hm to shared hm");
			}
			DynamicData val = new DynamicData((object)hm);
			if ((Object)(object)val.Get<SharedHealthManager>("sharedHealthManager") != (Object)null)
			{
				throw new InvalidOperationException("HM already add to another shared HM");
			}
			HMs.Add(hm);
			hm.OnDeath += new DeathEvent(CheckDie);
			checked
			{
				HP += hm.hp;
				hm.hp = int.MaxValue;
				val.Set("sharedHealthManager", (object)this);
				RefreshEnemyHPBar(recheckMembers: true);
			}
		}

		public void Add(IEnumerable<HealthManager> hms)
		{
			foreach (HealthManager hm in hms)
			{
				Add(hm);
			}
		}

		public void Add(params HealthManager[] hms)
		{
			Add(hms.AsEnumerable());
		}

		public void Hit(HitInstance hit)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			HealthManager backingHM = BackingHM;
			HitInstance val = hit;
			val.AttackType = (AttackTypes)5;
			val.IgnoreInvulnerable = true;
			backingHM.Hit(val);
		}

		public void Remove(HealthManager hm, int healthTaking)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			if (healthTaking < 0)
			{
				throw new ArgumentOutOfRangeException("healthTaking");
			}
			if ((Object)(object)hm.GetSharedHM() != (Object)(object)this)
			{
				throw new InvalidOperationException("Specified HM is not sharing in current Shared HM");
			}
			HMs.Remove(hm);
			hm.hp = healthTaking;
			checked
			{
				HP -= healthTaking;
				new DynamicData((object)hm).Set("sharedHealthManager", (object)null);
				((Component)hm).gameObject.EnableHPBar();
			}
		}

		public void CheckDie()
		{
			if (HMs.TrueForAll((HealthManager hm) => hm.isDead))
			{
				Die();
			}
		}

		public void Die()
		{
			BackingHM.Die((float?)null, (AttackTypes)5, true);
		}

		public void KillMembers()
		{
			foreach (HealthManager hM in HMs)
			{
				try
				{
					hM.Die((float?)null, (AttackTypes)1, true);
				}
				catch (NullReferenceException)
				{
				}
			}
		}

		public void RefreshEnemyHPBar(bool recheckMembers = false)
		{
			if (recheckMembers)
			{
				HMs.ForEach(delegate(HealthManager hm)
				{
					((Component)hm).gameObject.DisableHPBar();
				});
			}
			((Component)this).gameObject.RefreshHPBar();
		}

		private static void ReportDamage(orig_TakeDamage orig, HealthManager self, HitInstance hit)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Invalid comparison between Unknown and I4
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (self.IsBackingHM())
			{
				if ((int)hit.AttackType == 5 && hit.IgnoreInvulnerable)
				{
					orig.Invoke(self, hit);
				}
			}
			else
			{
				orig.Invoke(self, hit);
			}
			if (self.TryGetSharedHM(out SharedHealthManager shm))
			{
				HealthManager backingHM = shm.BackingHM;
				HitInstance val = hit;
				val.AttackType = (AttackTypes)5;
				val.IgnoreInvulnerable = true;
				backingHM.Hit(val);
			}
		}

		private static void ReportExtraDamage(orig_ApplyExtraDamage orig, HealthManager self, int damage)
		{
			orig.Invoke(self, damage);
			if (self.TryGetSharedHM(out SharedHealthManager shm))
			{
				shm.BackingHM.ApplyExtraDamage(damage);
			}
		}
	}
	[PublicAPI]
	[RequireComponent(typeof(DamageEnemies))]
	public sealed class UpdateHitDirection : MonoBehaviour
	{
		[ChildRef("")]
		private DamageEnemies de;

		public float offset;

		private void Start()
		{
			((Component)(object)this).InitChildRefs();
		}

		private void FixedUpdate()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			de.direction = ((Component)this).gameObject.transform.rotation.z + offset;
		}
	}
}
namespace Osmi.FsmActions
{
	[PublicAPI]
	public class BroadcastSimpleFSMEvent : FsmStateAction
	{
		public string? fsmName;

		public string? eventName;

		public bool includeInactive;

		public BroadcastSimpleFSMEvent()
		{
		}

		public BroadcastSimpleFSMEvent(string eventName, bool includeInactive = false)
		{
			this.eventName = eventName;
			this.includeInactive = includeInactive;
		}

		public BroadcastSimpleFSMEvent(string fsmName, string eventName, bool includeInactive = false)
		{
			this.fsmName = fsmName;
			this.eventName = eventName;
			this.includeInactive = includeInactive;
		}

		public override void OnEnter()
		{
			if (eventName != null)
			{
				if (fsmName == null)
				{
					global::Osmi.SimpleFSM.SimpleFSM.BroadcastEvent(eventName, includeInactive);
				}
				else
				{
					global::Osmi.SimpleFSM.SimpleFSM.BroadcastEvent(fsmName, eventName, includeInactive);
				}
			}
			((FsmStateAction)this).Finish();
		}
	}
	[PublicAPI]
	public class CheckIfInMapZone : FsmStateAction
	{
		public MapZone zone;

		public FsmEvent? trueEvent;

		public FsmEvent? falseEvent;

		public override void OnEnter()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			Fsm fsm = ((FsmStateAction)this).Fsm;
			GameManager instance = GameManager.instance;
			fsm.Event((((instance != null) ? new MapZone?(instance.sm.mapZone) : ((MapZone?)null)) == (MapZone?)zone) ? trueEvent : falseEvent);
			((FsmStateAction)this).Finish();
		}
	}
	[PublicAPI]
	public class CheckSceneName : FsmStateAction
	{
		public string sceneName = "";

		public FsmEvent? trueEvent;

		public FsmEvent? falseEvent;

		public CheckSceneName()
		{
		}

		public CheckSceneName(string sceneName)
		{
			this.sceneName = sceneName;
		}

		public override void OnEnter()
		{
			((FsmStateAction)this).Fsm.Event((GameManager.instance?.sceneName == sceneName) ? trueEvent : falseEvent);
			((FsmStateAction)this).Finish();
		}
	}
	[PublicAPI]
	public class EmitEvent : FsmStateAction
	{
		public FsmEvent? @event;

		public EmitEvent()
		{
		}

		public EmitEvent(FsmEvent @event)
		{
			this.@event = @event;
		}

		public EmitEvent(string eventName)
			: this(FsmEvent.GetFsmEvent(eventName))
		{
		}

		public override void OnEnter()
		{
			((FsmStateAction)this).Fsm.Event(@event);
			((FsmStateAction)this).Finish();
		}
	}
	[PublicAPI]
	public class InvokeCoroutine : FsmStateAction
	{
		public Func<IEnumerator>? coroutine;

		public bool sync;

		public InvokeCoroutine()
		{
		}

		public InvokeCoroutine(Func<IEnumerator> coroutine, bool sync = false)
		{
			this.coroutine = coroutine;
			this.sync = sync;
		}

		public InvokeCoroutine(IEnumerator coroutine, bool sync = false)
		{
			this.coroutine = Funcs.Constant(coroutine);
			this.sync = sync;
		}

		public override void OnEnter()
		{
			if (coroutine != null)
			{
				if (sync)
				{
					((FsmStateAction)this).Fsm.Owner.StartCoroutine(SyncCoroutine());
					return;
				}
				((FsmStateAction)this).Fsm.Owner.StartCoroutine(coroutine());
				((FsmStateAction)this).Finish();
			}
		}

		private IEnumerator SyncCoroutine()
		{
			yield return coroutine?.Invoke();
			((FsmStateAction)this).Finish();
		}
	}
	[PublicAPI]
	public class InvokeMethod : FsmStateAction
	{
		public Action? method;

		public InvokeMethod()
		{
		}

		public InvokeMethod(Action method)
		{
			this.method = method;
		}

		public InvokeMethod(Action<FsmStateAction> method)
		{
			this.method = method.Bind((FsmStateAction)(object)this);
		}

		public override void OnEnter()
		{
			method?.Invoke();
			((FsmStateAction)this).Finish();
		}
	}
	[PublicAPI]
	public class InvokePredicate : FsmStateAction
	{
		public Func<bool>? predicate;

		public FsmEvent? trueEvent;

		public FsmEvent? falseEvent;

		public InvokePredicate()
		{
		}

		public InvokePredicate(Func<bool> predicate)
		{
			this.predicate = predicate;
		}

		public InvokePredicate(Func<FsmStateAction, bool> predicate)
		{
			this.predicate = predicate.Bind((FsmStateAction)(object)this);
		}

		public override void OnEnter()
		{
			if (predicate != null)
			{
				Fsm fsm = ((FsmStateAction)this).Fsm;
				Func<bool>? func = predicate;
				fsm.Event((func != null && func()) ? trueEvent : falseEvent);
			}
			((FsmStateAction)this).Finish();
		}
	}
	[PublicAPI]
	public sealed class NoOp : FsmStateAction
	{
		public override void OnEnter()
		{
			((FsmStateAction)this).Finish();
		}
	}
	[PublicAPI]
	public class SendSimpleFSMEvent : FsmStateAction
	{
		public global::Osmi.SimpleFSM.SimpleFSM? fsm;

		public string? eventName;

		public SendSimpleFSMEvent()
		{
		}

		public SendSimpleFSMEvent(global::Osmi.SimpleFSM.SimpleFSM fsm, string eventName)
		{
			this.fsm = fsm;
			this.eventName = eventName;
		}

		public override void OnEnter()
		{
			if (eventName != null)
			{
				fsm?.SendEvent(eventName);
			}
			((FsmStateAction)this).Finish();
		}
	}
	[PublicAPI]
	public class SendSimpleFSMEventToGameObject : FsmStateAction
	{
		public GameObject? gameObject;

		public string? fsmName;

		public string? eventName;

		public bool includeInactive;

		public SendSimpleFSMEventToGameObject()
		{
		}

		public SendSimpleFSMEventToGameObject(string eventName, bool includeInactive = false)
		{
			this.eventName = eventName;
			this.includeInactive = includeInactive;
		}

		public SendSimpleFSMEventToGameObject(string fsmName, string eventName, bool includeInactive = false)
		{
			this.fsmName = fsmName;
			this.eventName = eventName;
		}

		public SendSimpleFSMEventToGameObject(GameObject gameObject, string eventName, bool includeInactive = false)
		{
			this.gameObject = gameObject;
			this.eventName = eventName;
			this.includeInactive = includeInactive;
		}

		public SendSimpleFSMEventToGameObject(GameObject gameObject, string fsmName, string eventName, bool includeInactive = false)
		{
			this.gameObject = gameObject;
			this.fsmName = fsmName;
			this.eventName = eventName;
			this.includeInactive = includeInactive;
		}

		public override void OnEnter()
		{
			if (eventName != null)
			{
				GameObject self = gameObject ?? ((FsmStateAction)this).Owner;
				if (fsmName == null)
				{
					self.SendSimpleFSMEvent(eventName, includeInactive);
				}
				else
				{
					self.SendSimpleFSMEvent(fsmName, eventName, includeInactive);
				}
			}
			((FsmStateAction)this).Finish();
		}
	}
	[PublicAPI]
	public class WaitUntil : FsmStateAction
	{
		public Func<bool>? predicate;

		public FsmEvent? @event;

		public WaitUntil()
		{
		}

		public WaitUntil(Func<bool> predicate, FsmEvent @event)
		{
			this.predicate = predicate;
			this.@event = @event;
		}

		public override void OnEnter()
		{
			if (predicate == null)
			{
				((FsmStateAction)this).Finish();
			}
		}

		public override void OnUpdate()
		{
			if (predicate != null && predicate())
			{
				((FsmStateAction)this).Fsm.Event(@event);
				((FsmStateAction)this).Finish();
			}
		}
	}
}
namespace Osmi.Exports
{
	[ModExportName("HealthShare")]
	[PublicAPI]
	public static class HealthShareExport
	{
		public static bool IsSharing(HealthManager self)
		{
			return self.IsSharing();
		}

		public static bool IsBackingHM(HealthManager self)
		{
			return self.IsBackingHM();
		}

		public static void AddToShared(HealthManager self, SharedHealthManager shm)
		{
			self.AddToShared(shm);
		}

		public static void AddToShared(IEnumerable<HealthManager> self, SharedHealthManager shm)
		{
			self.AddToShared(shm);
		}

		public static void AddToShared(GameObject self, SharedHealthManager shm)
		{
			self.AddToShared(shm);
		}

		public static void AddToShared(IEnumerable<GameObject> self, SharedHealthManager shm)
		{
			self.AddToShared(shm);
		}

		public static MonoBehaviour ShareHealth(IEnumerable<HealthManager> self, int initialHP, string name)
		{
			return (MonoBehaviour)(object)self.ShareHealth(initialHP, name);
		}

		public static MonoBehaviour ShareHealth(IEnumerable<GameObject> self, int initialHP, string name)
		{
			return (MonoBehaviour)(object)self.ShareHealth(initialHP, name);
		}

		public static MonoBehaviour ShareWith(HealthManager self, params HealthManager[] hms)
		{
			return (MonoBehaviour)(object)self.ShareWith(hms);
		}

		public static void StopSharing(HealthManager self, int healthTaking)
		{
			self.StopSharing(healthTaking);
		}
	}
	internal static class ModInteropExports
	{
		internal static void Init()
		{
			ModInteropManager.ModInterop(typeof(HealthShareExport));
		}
	}
}
namespace Osmi.ChildRefs
{
	[PublicAPI]
	public static class ChildRef
	{
		internal static readonly LazyMap<Type, Action<Component>> map = new LazyMap<Type, Action<Component>>(Compile);

		private static readonly MethodInfo componentGetComponent = (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/);

		private static readonly MethodInfo componentTfGetter = (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/);

		private static readonly MethodInfo componentGOGetter = (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/);

		private static readonly MethodInfo tfFind = (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/);

		private static readonly LazyMap<Type, MethodInfo> genericComponentGetComponent = new LazyMap<Type, MethodInfo>((Type t) => componentGetComponent.MakeGenericMethod(t));

		public static void Precompile<T>() where T : Component
		{
			_ = map[typeof(T)];
		}

		public static void Precompile(Type t)
		{
			_ = map[t];
		}

		private static Action<Component> Compile(Type t)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (t.IsAbstract)
			{
				throw new InvalidOperationException("Class " + t.FullName + " is abstract");
			}
			if (!t.IsSubclassOf(typeof(Component)))
			{
				throw new NotSupportedException("Class " + t.FullName + " is not Component");
			}
			List<Exception> exceptions = new List<Exception>();
			Action<Component> result = ILUtil.CreateDynamicMethod<Action<Component>>("<ChildRefsInit>__" + t.FullName.Replace('.', '_'), delegate(ILProcessor ilp)
			{
				//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0126: Unknown result type (might be due to invalid IL or missing references)
				//IL_0136: Unknown result type (might be due to invalid IL or missing references)
				//IL_0143: Unknown result type (might be due to invalid IL or missing references)
				//IL_010a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0166: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_018b: Unknown result type (might be due to invalid IL or missing references)
				FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (FieldInfo fieldInfo in fields)
				{
					ChildRefAttribute customAttribute = fieldInfo.GetCustomAttribute<ChildRefAttribute>();
					if (customAttribute != null)
					{
						string path = customAttribute.Path;
						Type fieldType = fieldInfo.FieldType;
						if (fieldType != typeof(GameObject) && !fieldType.IsSubclassOf(typeof(Component)))
						{
							exceptions.Add(new InvalidOperationException("Type " + fieldType.FullName + " is neither GameObject nor Component"));
						}
						else
						{
							if (path.Length == 0)
							{
								if (fieldType == typeof(GameObject))
								{
									exceptions.Add(new InvalidOperationException("Do not use Child Ref to get the GameObject of its own"));
									continue;
								}
								if (fieldType == typeof(Transform))
								{
									exceptions.Add(new InvalidOperationException("Do not use Child Ref to get the Transform of its own"));
									continue;
								}
							}
							ilp.Emit(OpCodes.Ldarg_0);
							ilp.Emit(OpCodes.Ldarg_0);
							if (path.Length == 0)
							{
								Extensions.Emit(ilp, OpCodes.Call, (MethodBase)genericComponentGetComponent[fieldType]);
							}
							else
							{
								Extensions.Emit(ilp, OpCodes.Call, (MethodBase)componentTfGetter);
								ilp.Emit(OpCodes.Ldstr, path);
								Extensions.Emit(ilp, OpCodes.Call, (MethodBase)tfFind);
								if (fieldType == typeof(GameObject))
								{
									Extensions.Emit(ilp, OpCodes.Call, (MethodBase)componentGOGetter);
								}
								else if (!(fieldType == typeof(Transform)))
								{
									Extensions.Emit(ilp, OpCodes.Call, (MethodBase)genericComponentGetComponent[fieldType]);
								}
							}
							Extensions.Emit(ilp, OpCodes.Stfld, fieldInfo);
						}
					}
				}
				ilp.Emit(OpCodes.Ret);
			});
			if (exceptions.Count == 1)
			{
				throw exceptions[0];
			}
			if (exceptions.Count > 1)
			{
				throw new AggregateException(exceptions);
			}
			Logger.LogDebug($"[ChildRefs] Compiling {t.FullName} took {Time.realtimeSinceStartup - realtimeSinceStartup}s");
			return result;
		}
	}
	[AttributeUsage(AttributeTargets.Field)]
	[PublicAPI]
	[MeansImplicitUse]
	public sealed class ChildRefAttribute : Attribute
	{
		public string Path { get; private init; }

		public ChildRefAttribute(string path = "")
		{
			Path = path;
		}
	}
	[PublicAPI]
	public static class ChildRefsExtension
	{
		public static void InitChildRefs(this Component self)
		{
			ChildRef.map[((object)self).GetType()](self);
		}
	}
}
namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class AllowNullAttribute : Attribute
	{
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class DisallowNullAttribute : Attribute
	{
	}
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class DoesNotReturnAttribute : Attribute
	{
	}
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class DoesNotReturnIfAttribute : Attribute
	{
		public bool ParameterValue { get; }

		public DoesNotReturnIfAttribute(bool parameterValue)
		{
			ParameterValue = parameterValue;
		}
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class MaybeNullAttribute : Attribute
	{
	}
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class MaybeNullWhenAttribute : Attribute
	{
		public bool ReturnValue { get; }

		public MaybeNullWhenAttribute(bool returnValue)
		{
			ReturnValue = returnValue;
		}
	}
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class MemberNotNullAttribute : Attribute
	{
		public string[] Members { get; }

		public MemberNotNullAttribute(string member)
		{
			Members = new string[1] { member };
		}

		public MemberNotNullAttribute(params string[] members)
		{
			Members = members;
		}
	}
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class MemberNotNullWhenAttribute : Attribute
	{
		public bool ReturnValue { get; }

		public string[] Members { get; }

		public MemberNotNullWhenAttribute(bool returnValue, string member)
		{
			ReturnValue = returnValue;
			Members = new string[1] { member };
		}

		public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
		{
			ReturnValue = returnValue;
			Members = members;
		}
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class NotNullAttribute : Attribute
	{
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class NotNullIfNotNullAttribute : Attribute
	{
		public string ParameterName { get; }

		public NotNullIfNotNullAttribute(string parameterName)
		{
			ParameterName = parameterName;
		}
	}
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class NotNullWhenAttribute : Attribute
	{
		public bool ReturnValue { get; }

		public NotNullWhenAttribute(bool returnValue)
		{
			ReturnValue = returnValue;
		}
	}
}
internal class Osmi_ProcessedByFody
{
	internal const string FodyVersion = "6.6.4.0";

	internal const string InfoOf = "2.1.1.0";

	internal const string HKReflect = "0.2.0.0";
}
