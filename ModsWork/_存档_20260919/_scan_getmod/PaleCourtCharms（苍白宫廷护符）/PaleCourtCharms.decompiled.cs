using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using DebugMod;
using GlobalEnums;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using InControl;
using ItemChanger;
using ItemChanger.Items;
using ItemChanger.Locations;
using ItemChanger.Modules;
using ItemChanger.Tags;
using ItemChanger.UIDefs;
using JetBrains.Annotations;
using MenuChanger;
using MenuChanger.Attributes;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using Microsoft.CodeAnalysis;
using Modding;
using Modding.Delegates;
using Modding.Utils;
using Newtonsoft.Json;
using On;
using On.HutongGames.PlayMaker.Actions;
using PaleCourtCharms.Interop;
using PaleCourtCharms.Rando;
using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using RandoSettingsManager.SettingsManagement.Versioning;
using RandomizerCore;
using RandomizerCore.Json;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerMod;
using RandomizerMod.IC;
using RandomizerMod.Logging;
using RandomizerMod.Menu;
using RandomizerMod.RC;
using RandomizerMod.RandomizerData;
using RandomizerMod.Settings;
using SFCore;
using SFCore.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vasi;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: AssemblyCompany("PaleCourtCharms")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyDescription("Standalone Pale Court Charms Mod")]
[assembly: AssemblyFileVersion("1.4.0.0")]
[assembly: AssemblyInformationalVersion("1.4.0汉化")]
[assembly: AssemblyProduct("PaleCourtCharms")]
[assembly: AssemblyTitle("PaleCourtCharms")]
[assembly: AssemblyVersion("1.4.0.0")]
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
namespace PaleCourtCharms
{
	public static class ABManager
	{
		private static readonly Assembly asm = Assembly.GetExecutingAssembly();

		public static AssetBundle Charms { get; private set; }

		public static AssetBundle CharmUnlock { get; private set; }

		public static void LoadAll()
		{
			if ((Object)(object)Charms == (Object)null)
			{
				Charms = LoadBundle("PaleCourtCharms.assets.pureamulets", "Charms");
			}
			if ((Object)(object)CharmUnlock == (Object)null)
			{
				CharmUnlock = LoadBundle("PaleCourtCharms.assets.charmunlock", "CharmUnlock");
			}
		}

		public static T LoadFromCharms<T>(string assetName) where T : Object
		{
			return LoadAsset<T>(Charms, assetName, "Charms");
		}

		public static T LoadFromUnlocks<T>(string assetName) where T : Object
		{
			return LoadAsset<T>(CharmUnlock, assetName, "CharmUnlock");
		}

		public static void UnloadAll(bool unloadAll = false)
		{
			if ((Object)(object)Charms != (Object)null)
			{
				Charms.Unload(unloadAll);
				Charms = null;
				Logger.Log("[PaleCourtCharms] Unloaded Charms AssetBundle.");
			}
			if ((Object)(object)CharmUnlock != (Object)null)
			{
				CharmUnlock.Unload(unloadAll);
				CharmUnlock = null;
				Logger.Log("[PaleCourtCharms] Unloaded CharmUnlock AssetBundle.");
			}
		}

		public static T LoadAsset<T>(AssetBundle bundle, string assetName, string label) where T : Object
		{
			if ((Object)(object)bundle == (Object)null)
			{
				Logger.LogError("[PaleCourtCharms] Tried to load asset '" + assetName + "' from " + label + ", but bundle is null.");
				return default(T);
			}
			T val = bundle.LoadAsset<T>(assetName);
			if ((Object)(object)val == (Object)null)
			{
				Logger.LogError("[PaleCourtCharms] Asset '" + assetName + "' not found in " + label + " bundle.");
			}
			return val;
		}

		private static AssetBundle LoadBundle(string resourceName, string label)
		{
			Stream manifestResourceStream = asm.GetManifestResourceStream(resourceName);
			if (manifestResourceStream == null)
			{
				Logger.LogError("[PaleCourtCharms] Could not find embedded resource: " + resourceName);
				return null;
			}
			using (manifestResourceStream)
			{
				using MemoryStream memoryStream = new MemoryStream();
				manifestResourceStream.CopyTo(memoryStream);
				AssetBundle obj = AssetBundle.LoadFromMemory(memoryStream.ToArray());
				if ((Object)(object)obj == (Object)null)
				{
					Logger.LogError("[PaleCourtCharms] Failed to load AssetBundle: " + label);
				}
				else
				{
					Logger.Log("[PaleCourtCharms] Loaded AssetBundle: " + label);
				}
				return obj;
			}
		}
	}
	public class AbyssalBloomBehaviour : MonoBehaviour
	{
		private tk2dSpriteAnimator _hcAnim;

		private GameObject _knightBall;

		private tk2dSpriteAnimator _knightBallAnim;

		private List<NailSlash> _nailSlashes;

		private ModifyBloomProps _modifyProps;

		private Coroutine _sideSlashCoro;

		private GameObject _sideSlash;

		private Coroutine _vertSlashCoro;

		private GameObject _shadeSlashContainer;

		private Coroutine _wallSlashCoro;

		private GameObject _wallSlash;

		private bool _crystalChecked;

		private bool _crystalAvailable;

		private Type _crystalType;

		private object _crystalInstance;

		private MethodInfo _crystalEquippedMethod;

		private const float BloomSpeedBonusL1 = 0.05f;

		private const float BloomSpeedBonusL2 = 0.15f;

		private bool _crystalEquippedCached;

		private int _level;

		private bool _fury;

		private int _shadeSlashNum = 1;

		private bool playingAudio;

		private float audioCooldown = 0.2f;

		private HeroController _hc => HeroController.instance;

		private PlayerData _pd => PlayerData.instance;

		private float damageScale
		{
			get
			{
				if (!_pd.equippedCharm_16)
				{
					return 0.0625f;
				}
				return 0.075f;
			}
		}

		private float damageBuff => damageScale * (float)(_pd.equippedCharm_27 ? Math.Max(_pd.joniHealthBlue - _pd.healthBlue, 0) : (_pd.maxHealth - _pd.health)) * ((_level == 2) ? 1.5f : 1f);

		private void OnEnable()
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Expected O, but got Unknown
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Expected O, but got Unknown
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Expected O, but got Unknown
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Expected O, but got Unknown
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Expected O, but got Unknown
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Expected O, but got Unknown
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Expected O, but got Unknown
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Expected O, but got Unknown
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Expected O, but got Unknown
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Expected O, but got Unknown
			_hcAnim = ((Component)_hc).GetComponent<tk2dSpriteAnimator>();
			_knightBall = Object.Instantiate<GameObject>(PaleCourtCharms.preloadedGO["Knight Ball"], ((Component)_hc).transform);
			Vector3 localScale = _knightBall.transform.localScale;
			localScale.x *= -1f;
			_knightBall.transform.localScale = localScale;
			Transform transform = _knightBall.transform;
			transform.localPosition += new Vector3(-4.75f, -0.25f);
			_knightBallAnim = _knightBall.GetComponent<tk2dSpriteAnimator>();
			_nailSlashes = new List<NailSlash>
			{
				HeroController.instance.normalSlash,
				HeroController.instance.alternateSlash,
				HeroController.instance.downSlash,
				HeroController.instance.upSlash
			};
			_modifyProps = ((Component)this).GetComponent<ModifyBloomProps>();
			PlayMakerFSM fsm = Object.Instantiate<PlayMakerFSM>(FSMUtility.LocateMyFSM(PaleCourtCharms.preloadedGO["Radiance"], "Control"), ((Component)_hc).transform);
			PaleCourtCharms.Clips["Shade Slash"] = (AudioClip)fsm.GetAction<AudioPlayerOneShotSingle>("Antic", 1).audioClip.Value;
			HealthManager.Hit += new hook_Hit(HealthManagerHit);
			HeroController.CancelAttack += new hook_CancelAttack(HeroControllerCancelAttack);
			HeroController.CancelDownAttack += new hook_CancelDownAttack(HeroControllerCancelDownAttack);
			HeroController.Attack += new hook_Attack(DoVoidAttack);
			tk2dSpriteAnimator.Play_string += new hook_Play_string(Tk2dSpriteAnimatorPlay);
			KnightHatchling.OnEnable += new hook_OnEnable(KnightHatchlingOnEnable);
			SpriteFlash.FlashingFury += new hook_FlashingFury(SpriteFlashFlashingFury);
			ModHooks.CharmUpdateHook += new CharmUpdateHandler(OnCharmUpdate);
			TryInitCrystalIntegration();
			UpdateCrystalEquippedCache();
			HeroController.Move += new hook_Move(HeroControllerMoveHook);
		}

		private void OnDisable()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Expected O, but got Unknown
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Expected O, but got Unknown
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Expected O, but got Unknown
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Expected O, but got Unknown
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Expected O, but got Unknown
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Expected O, but got Unknown
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Expected O, but got Unknown
			SetLevel(0);
			HealthManager.Hit -= new hook_Hit(HealthManagerHit);
			HeroController.CancelAttack -= new hook_CancelAttack(HeroControllerCancelAttack);
			HeroController.CancelDownAttack -= new hook_CancelDownAttack(HeroControllerCancelDownAttack);
			HeroController.Attack -= new hook_Attack(DoVoidAttack);
			tk2dSpriteAnimator.Play_string -= new hook_Play_string(Tk2dSpriteAnimatorPlay);
			KnightHatchling.OnEnable -= new hook_OnEnable(KnightHatchlingOnEnable);
			SpriteFlash.FlashingFury -= new hook_FlashingFury(SpriteFlashFlashingFury);
			HeroController.Move -= new hook_Move(HeroControllerMoveHook);
			ModHooks.CharmUpdateHook -= new CharmUpdateHandler(OnCharmUpdate);
		}

		public void SetLevel(int level)
		{
			_level = level;
			switch (_level)
			{
			case 0:
				ModifySlashColors(modify: false);
				_modifyProps.ResetProps();
				break;
			case 1:
				ModifySlashColors(modify: true);
				_modifyProps.ModifyPropsL1();
				break;
			case 2:
				ModifySlashColors(modify: true);
				_modifyProps.ModifyPropsL2();
				break;
			}
		}

		public void SetFury(bool fury)
		{
			_fury = fury;
		}

		private void OnCharmUpdate(PlayerData pd, HeroController hc)
		{
			UpdateCrystalEquippedCache();
		}

		private void TryInitCrystalIntegration()
		{
			if (_crystalChecked)
			{
				return;
			}
			_crystalChecked = true;
			try
			{
				IMod mod = ModHooks.GetMod("Transcendence", false, false);
				Assembly assembly = null;
				Mod val = (Mod)(object)((mod is Mod) ? mod : null);
				assembly = ((val == null) ? AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(delegate(Assembly a)
				{
					try
					{
						return a.GetType("Transcendence.Crystalmaster") != null;
					}
					catch
					{
						return false;
					}
				}) : ((object)val).GetType().Assembly);
				if (assembly == null)
				{
					_crystalAvailable = false;
					return;
				}
				_crystalType = assembly.GetType("Transcendence.Crystalmaster");
				if (_crystalType == null)
				{
					_crystalAvailable = false;
					return;
				}
				PropertyInfo property = _crystalType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
				if (property != null)
				{
					_crystalInstance = property.GetValue(null);
				}
				else
				{
					FieldInfo field = _crystalType.GetField("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
					if (field != null)
					{
						_crystalInstance = field.GetValue(null);
					}
				}
				_crystalEquippedMethod = _crystalType.GetMethod("Equipped", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				_crystalAvailable = _crystalEquippedMethod != null;
			}
			catch
			{
				_crystalAvailable = false;
				_crystalInstance = null;
				_crystalEquippedMethod = null;
				_crystalType = null;
			}
		}

		private void UpdateCrystalEquippedCache()
		{
			if (!_crystalChecked)
			{
				TryInitCrystalIntegration();
			}
			if (!_crystalAvailable || _crystalEquippedMethod == null || _crystalType == null)
			{
				_crystalEquippedCached = false;
				return;
			}
			if (_crystalInstance == null && _crystalType != null)
			{
				try
				{
					PropertyInfo property = _crystalType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
					if (property != null)
					{
						_crystalInstance = property.GetValue(null);
					}
					else
					{
						FieldInfo field = _crystalType.GetField("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
						if (field != null)
						{
							_crystalInstance = field.GetValue(null);
						}
					}
				}
				catch
				{
					_crystalInstance = null;
				}
			}
			if (_crystalInstance == null)
			{
				_crystalEquippedCached = false;
				return;
			}
			try
			{
				_crystalEquippedCached = (bool)_crystalEquippedMethod.Invoke(_crystalInstance, null);
			}
			catch
			{
				_crystalEquippedCached = false;
			}
		}

		private void HeroControllerMoveHook(orig_Move orig, HeroController self, float dir)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self, dir);
			if (_level <= 0 || !_crystalEquippedCached)
			{
				return;
			}
			float num = ((_level == 2) ? 0.15f : 0.05f);
			try
			{
				Rigidbody2D val = default(Rigidbody2D);
				if (((Component)self).TryGetComponent<Rigidbody2D>(ref val))
				{
					Vector2 velocity = val.velocity;
					if (Mathf.Abs(velocity.x) > 0.001f)
					{
						velocity.x *= 1f + num;
						val.velocity = velocity;
					}
				}
			}
			catch
			{
			}
		}

		private void ModifySlashColors(bool modify)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			foreach (NailSlash nailSlash in _nailSlashes)
			{
				nailSlash.SetFury(modify);
			}
			Color color = (modify ? Color.black : Color.white);
			GameObject[] array = (GameObject[])(object)new GameObject[5] { _hc.slashPrefab, _hc.slashAltPrefab, _hc.downSlashPrefab, _hc.upSlashPrefab, _hc.wallSlashPrefab };
			for (int i = 0; i < array.Length; i++)
			{
				((tk2dBaseSprite)array[i].GetComponent<tk2dSprite>()).color = color;
			}
			GameObject val = USceneUtil.FindGameObjectInChildren(((Component)HeroController.instance).gameObject, "Attacks");
			string[] array2 = new string[3] { "Cyclone Slash", "Dash Slash", "Great Slash" };
			foreach (string text in array2)
			{
				((tk2dBaseSprite)USceneUtil.FindGameObjectInChildren(val, text).GetComponent<tk2dSprite>()).color = color;
				tk2dSprite[] componentsInChildren = USceneUtil.FindGameObjectInChildren(val, text).GetComponentsInChildren<tk2dSprite>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					((tk2dBaseSprite)componentsInChildren[j]).color = color;
				}
			}
		}

		private void HealthManagerHit(orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Invalid comparison between Unknown and I4
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Invalid comparison between Unknown and I4
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Invalid comparison between Unknown and I4
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			AttackTypes attackType = hitInstance.AttackType;
			if (((int)attackType == 0 || (int)attackType == 7) ? true : false)
			{
				hitInstance.Multiplier += damageBuff + (_fury ? 0.75f : 0f);
			}
			if ((int)hitInstance.AttackType == 6)
			{
				hitInstance.Multiplier += 2f * damageBuff;
			}
			attackType = hitInstance.AttackType;
			if (((int)attackType == 0 || (int)attackType == 7) ? true : false)
			{
				hitInstance.Multiplier += damageBuff + (_fury ? 0.75f : 0f);
			}
			if ((int)hitInstance.AttackType == 6)
			{
				hitInstance.Multiplier += 2f * damageBuff;
			}
			orig.Invoke(self, hitInstance);
		}

		private void HeroControllerCancelDownAttack(orig_CancelDownAttack orig, HeroController self)
		{
			if (_vertSlashCoro != null)
			{
				CancelVerticalTendrilAttack();
			}
			orig.Invoke(self);
		}

		private void HeroControllerCancelAttack(orig_CancelAttack orig, HeroController self)
		{
			if (_sideSlashCoro != null)
			{
				CancelTendrilAttack();
			}
			orig.Invoke(self);
		}

		private void Tk2dSpriteAnimatorPlay(orig_Play_string orig, tk2dSpriteAnimator self, string name)
		{
			if ((Object)(object)((Component)self).gameObject == (Object)(object)((Component)_hc).gameObject && name == "Idle Hurt")
			{
				self.Play("Idle");
			}
			else
			{
				orig.Invoke(self, name);
			}
		}

		private void SpriteFlashFlashingFury(orig_FlashingFury orig, SpriteFlash self)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			self.flash(Color.black, 0.75f, 0.25f, 0.01f, 0.25f);
			Mirror.SetField<SpriteFlash, bool>(self, "repeatFlash", true);
		}

		private void KnightHatchlingOnEnable(orig_OnEnable orig, KnightHatchling self)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			if (_level == 2)
			{
				TypeDetails field = Mirror.GetField<KnightHatchling, TypeDetails>(self, "details");
				TypeDetails val = field;
				val.damage = field.damage * 2;
				Mirror.SetField<KnightHatchling, TypeDetails>(self, "details", val);
			}
		}

		private void DoVoidAttack(orig_Attack origAttack, HeroController hc, AttackDirection dir)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			if (_level != 2)
			{
				origAttack.Invoke(hc, dir);
				return;
			}
			InputHandler instance = InputHandler.Instance;
			if (_pd.GetBool("equippedCharm_32"))
			{
				Mirror.SetField<HeroController, float>(_hc, "attackDuration", _hc.ATTACK_DURATION_CH);
				Mirror.SetField<HeroController, float>(_hc, "attack_cooldown", _hc.ATTACK_COOLDOWN_TIME_CH);
			}
			else
			{
				Mirror.SetField<HeroController, float>(_hc, "attackDuration", _hc.ATTACK_DURATION);
				Mirror.SetField<HeroController, float>(_hc, "attack_cooldown", _hc.ATTACK_COOLDOWN_TIME);
			}
			_hc.cState.recoiling = false;
			if (hc.cState.wallSliding)
			{
				if (_hc.cState.attacking)
				{
					CancelWallTendrilAttack();
				}
				_wallSlashCoro = ((MonoBehaviour)this).StartCoroutine(WallTendrilAttack());
				return;
			}
			if (OneAxisInputControl.op_Implicit((OneAxisInputControl)(object)instance.ActionButtonToPlayerAction((HeroActionButton)11)) && !hc.CheckTouchingGround())
			{
				if (_hc.cState.attacking)
				{
					CancelVerticalTendrilAttack();
				}
				_vertSlashCoro = ((MonoBehaviour)this).StartCoroutine(VerticalTendrilAttack(up: false));
				return;
			}
			if (OneAxisInputControl.op_Implicit((OneAxisInputControl)(object)instance.ActionButtonToPlayerAction((HeroActionButton)10)))
			{
				if (_hc.cState.attacking)
				{
					CancelVerticalTendrilAttack();
				}
				_vertSlashCoro = ((MonoBehaviour)this).StartCoroutine(VerticalTendrilAttack(up: true));
				return;
			}
			if (_hc.cState.attacking)
			{
				CancelTendrilAttack();
				_shadeSlashNum = ((_shadeSlashNum != 1) ? 1 : 2);
			}
			_sideSlashCoro = ((MonoBehaviour)this).StartCoroutine(TendrilAttack());
		}

		public void CancelTendrilAttack()
		{
			if (_sideSlashCoro != null)
			{
				((MonoBehaviour)this).StopCoroutine(_sideSlashCoro);
			}
			Object.Destroy((Object)(object)_sideSlash);
			_knightBall.SetActive(false);
			((Renderer)((Component)_hc).GetComponent<MeshRenderer>()).enabled = true;
			ResetTendrilAttack();
		}

		public void CancelVerticalTendrilAttack()
		{
			if (_vertSlashCoro != null)
			{
				((MonoBehaviour)this).StopCoroutine(_vertSlashCoro);
			}
			Object.Destroy((Object)(object)_shadeSlashContainer);
			_hc.StartAnimationControl();
			ResetTendrilAttack();
		}

		public void CancelWallTendrilAttack()
		{
			if (_wallSlashCoro != null)
			{
				((MonoBehaviour)this).StopCoroutine(_wallSlashCoro);
			}
			Object.Destroy((Object)(object)_wallSlash);
			_hc.StartAnimationControl();
			ResetTendrilAttack();
		}

		private void ResetTendrilAttack()
		{
			_hc.cState.attacking = false;
			_hc.cState.upAttacking = false;
			_hc.cState.downAttacking = false;
			Mirror.SetField<HeroController, float>(_hc, "attack_time", 0f);
		}

		private IEnumerator TendrilAttack()
		{
			_hc.cState.attacking = true;
			MeshRenderer mr = ((Component)_hc).GetComponent<MeshRenderer>();
			if (!playingAudio)
			{
				((MonoBehaviour)this).StartCoroutine(PlayAudio());
			}
			((Renderer)mr).enabled = false;
			_knightBall.SetActive(true);
			Object.Destroy((Object)(object)_sideSlash);
			_sideSlash = new GameObject("Shade Slash");
			_sideSlash.transform.parent = _knightBall.transform;
			_sideSlash.layer = 17;
			_sideSlash.tag = "Nail Attack";
			_sideSlash.transform.localPosition = Vector3.zero;
			_sideSlash.transform.localScale = Vector3.one;
			_sideSlash.SetActive(false);
			AddDamageEnemiesFsm(_sideSlash, (AttackDirection)0);
			PolygonCollider2D val = _sideSlash.AddComponent<PolygonCollider2D>();
			val.points = (Vector2[])(object)new Vector2[6]
			{
				new Vector2(0f, -2f),
				new Vector2(3.5f, -2f),
				new Vector2(3.5f, 0f),
				new Vector2(3f, 1f),
				new Vector2(0f, 2f),
				new Vector2(-3f, 0f)
			};
			((Collider2D)val).offset = Vector2.zero;
			((Collider2D)val).isTrigger = true;
			_sideSlash.AddComponent<ShadeSlashKnockback>().heroCtrl = _hc;
			GameObject obj = Object.Instantiate<GameObject>(_sideSlash, _sideSlash.transform);
			FsmUtil.GetFsmIntVariable(FSMUtility.LocateMyFSM(obj, "damages_enemy"), "damageDealt").Value = 0;
			obj.layer = 16;
			_sideSlash.AddComponent<ShadeSlash>().attackDirection = (AttackDirection)0;
			_sideSlash.SetActive(true);
			obj.SetActive(true);
			_knightBallAnim.PlayFromFrame("Slash" + _shadeSlashNum + " Antic", 2);
			yield return (object)new WaitWhile((Func<bool>)(() => _knightBallAnim.IsPlaying("Slash" + _shadeSlashNum + " Antic")));
			yield return (object)new WaitForSeconds(Extensions.PlayAnimGetTime(_knightBallAnim, "Slash" + _shadeSlashNum) - 1f / 24f);
			Object.Destroy((Object)(object)_sideSlash);
			((Renderer)mr).enabled = true;
			_knightBall.SetActive(false);
			_shadeSlashNum = ((_shadeSlashNum != 1) ? 1 : 2);
			_hc.cState.attacking = false;
		}

		private IEnumerator VerticalTendrilAttack(bool up)
		{
			_hc.cState.attacking = true;
			if (up)
			{
				_hc.cState.upAttacking = true;
			}
			else
			{
				_hc.cState.downAttacking = true;
			}
			string animName = (up ? "Up" : "Down");
			_hc.StopAnimationControl();
			if (!playingAudio)
			{
				((MonoBehaviour)this).StartCoroutine(PlayAudio());
			}
			_hcAnim.Play(animName + "Slash Void");
			tk2dSpriteAnimationClip clipByName = _hcAnim.GetClipByName(animName + "Slash Void");
			_hcAnim.Play(clipByName);
			Object.Destroy((Object)(object)_shadeSlashContainer);
			_shadeSlashContainer = Object.Instantiate<GameObject>(new GameObject("Shade Slash Container"), ((Component)_hc).transform);
			_shadeSlashContainer.layer = 17;
			_shadeSlashContainer.SetActive(false);
			GameObject val = new GameObject("Shade Slash");
			val.transform.parent = _shadeSlashContainer.transform;
			val.layer = 17;
			val.tag = "Nail Attack";
			val.transform.localPosition = new Vector3(0f, up ? 1f : (-2f), 0f);
			val.transform.localScale = new Vector3(2f, 2f, 2f);
			AddDamageEnemiesFsm(val, (AttackDirection)(up ? 1 : 2));
			PolygonCollider2D val2 = val.AddComponent<PolygonCollider2D>();
			if (up)
			{
				val2.points = (Vector2[])(object)new Vector2[8]
				{
					new Vector2(-1f, 0f),
					new Vector2(-0.75f, 1.5f),
					new Vector2(-0.5f, 2f),
					new Vector2(0f, 2.25f),
					new Vector2(0.5f, 2f),
					new Vector2(0.75f, 1.5f),
					new Vector2(1f, 0f),
					new Vector2(0f, 0.5f)
				};
			}
			else
			{
				val2.points = (Vector2[])(object)new Vector2[10]
				{
					new Vector2(-1f, -0f),
					new Vector2(-1.25f, -0.5f),
					new Vector2(-0.875f, -1.5f),
					new Vector2(-0.5f, -1.9f),
					new Vector2(0f, -2.2f),
					new Vector2(0.5f, -2f),
					new Vector2(0.875f, -1.5f),
					new Vector2(1.25f, -0.5f),
					new Vector2(1f, -0f),
					new Vector2(0f, 0.5f)
				};
			}
			((Collider2D)val2).offset = new Vector2(0f, up ? (-1f) : 0.75f);
			((Collider2D)val2).isTrigger = true;
			GameObject obj = Object.Instantiate<GameObject>(val, val.transform);
			FsmUtil.GetFsmIntVariable(FSMUtility.LocateMyFSM(obj, "damages_enemy"), "damageDealt").Value = 0;
			obj.layer = 16;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			val.AddComponent<MeshRenderer>();
			val.AddComponent<MeshFilter>();
			tk2dSprite obj2 = val.AddComponent<tk2dSprite>();
			tk2dSpriteAnimator val3 = val.AddComponent<tk2dSpriteAnimator>();
			((tk2dBaseSprite)obj2).Collection = ((tk2dBaseSprite)((Component)_hc).GetComponent<tk2dSprite>()).Collection;
			val3.Library = _hcAnim.Library;
			val.AddComponent<ShadeSlash>().attackDirection = (AttackDirection)(up ? 1 : 2);
			_shadeSlashContainer.SetActive(true);
			yield return (object)new WaitForSeconds(Extensions.PlayAnimGetTime(val3, animName + "Slash Effect"));
			Object.Destroy((Object)(object)_shadeSlashContainer);
			yield return (object)new WaitWhile((Func<bool>)(() => _hcAnim.Playing && _hcAnim.IsPlaying(animName + "Slash Void")));
			_hc.StartAnimationControl();
			_hc.cState.attacking = false;
			if (up)
			{
				_hc.cState.upAttacking = false;
			}
			else
			{
				_hc.cState.downAttacking = false;
			}
		}

		private IEnumerator WallTendrilAttack()
		{
			_hc.cState.attacking = true;
			_hc.StopAnimationControl();
			if (!playingAudio)
			{
				((MonoBehaviour)this).StartCoroutine(PlayAudio());
			}
			_hcAnim.Play(_hcAnim.GetClipByName("WallSlash Void"));
			Object.Destroy((Object)(object)_wallSlash);
			_wallSlash = new GameObject("Shade Slash");
			_wallSlash.transform.parent = ((Component)_hc).transform;
			_wallSlash.layer = 17;
			_wallSlash.tag = "Nail Attack";
			_wallSlash.transform.localPosition = new Vector3(0f, 1f, 0f);
			_wallSlash.transform.localScale = Vector3.one;
			_wallSlash.SetActive(false);
			AddDamageEnemiesFsm(_wallSlash, (AttackDirection)0);
			PolygonCollider2D val = _wallSlash.AddComponent<PolygonCollider2D>();
			val.points = (Vector2[])(object)new Vector2[5]
			{
				new Vector2(-1.5f, 2f),
				new Vector2(1f, 1.5f),
				new Vector2(3f, -1f),
				new Vector2(1f, -2f),
				new Vector2(-1.5f, -1f)
			};
			((Collider2D)val).offset = new Vector2(1f, 0f);
			((Collider2D)val).isTrigger = true;
			GameObject obj = Object.Instantiate<GameObject>(_wallSlash, _wallSlash.transform);
			FsmUtil.GetFsmIntVariable(FSMUtility.LocateMyFSM(obj, "damages_enemy"), "damageDealt").Value = 0;
			obj.layer = 16;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			obj.SetActive(false);
			_wallSlash.AddComponent<MeshRenderer>();
			_wallSlash.AddComponent<MeshFilter>();
			tk2dSprite obj2 = _wallSlash.AddComponent<tk2dSprite>();
			tk2dSpriteAnimator val2 = _wallSlash.AddComponent<tk2dSpriteAnimator>();
			((tk2dBaseSprite)obj2).Collection = ((tk2dBaseSprite)((Component)_hc).GetComponent<tk2dSprite>()).Collection;
			val2.Library = _hcAnim.Library;
			_wallSlash.AddComponent<ShadeSlash>().attackDirection = (AttackDirection)0;
			_wallSlash.SetActive(true);
			obj.SetActive(true);
			yield return (object)new WaitForSeconds(Extensions.PlayAnimGetTime(val2, "Slash Effect"));
			Object.Destroy((Object)(object)_wallSlash);
			_hc.StartAnimationControl();
			_hc.cState.attacking = false;
		}

		private IEnumerator PlayAudio()
		{
			playingAudio = true;
			((MonoBehaviour)(object)this).PlayAudio(PaleCourtCharms.Clips["Shade Slash"], 0.7f);
			yield return (object)new WaitForSeconds(audioCooldown);
			playingAudio = false;
		}

		private void AddDamageEnemiesFsm(GameObject o, AttackDirection dir)
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Expected I4, but got Unknown
			PlayMakerFSM val = o.AddComponent<PlayMakerFSM>();
			PlayMakerFSM obj = FSMUtility.LocateMyFSM(USceneUtil.Find(((Component)_hc).gameObject, "AltSlash"), "damages_enemy");
			FieldInfo[] fields = typeof(PlayMakerFSM).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				fieldInfo.SetValue(val, fieldInfo.GetValue(obj));
			}
			switch ((int)dir)
			{
			case 0:
				FsmUtil.GetFsmFloatVariable(val, "direction").Value = (_hc.cState.facingRight ? 0f : 180f);
				break;
			case 1:
				FsmUtil.GetFsmFloatVariable(val, "direction").Value = 90f;
				break;
			case 2:
				FsmUtil.GetFsmFloatVariable(val, "direction").Value = 270f;
				break;
			}
		}
	}
	[RequireComponent(typeof(PolygonCollider2D))]
	public class ShadeSlashKnockback : MonoBehaviour
	{
		public HeroController heroCtrl;

		private bool _hasBounced;

		private const int WALL_LAYER = 8;

		private const float MIN_PCT = 0.6f;

		private const float TIP_RIGHT = 3.5f;

		private const float TIP_LEFT = -3f;

		private PolygonCollider2D _poly;

		private void Awake()
		{
			_poly = ((Component)this).GetComponent<PolygonCollider2D>();
			((Collider2D)_poly).isTrigger = true;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			if (_hasBounced || ((Component)other).gameObject.layer != 8)
			{
				return;
			}
			Vector2 val = Vector2.op_Implicit(((Component)this).transform.position);
			Vector2 val2 = other.ClosestPoint(val) - val;
			if (Mathf.Abs(val2.x) <= Mathf.Abs(val2.y))
			{
				return;
			}
			bool flag = val2.x > 0f;
			float num = (flag ? 3.5f : 3f);
			Vector2 val3 = default(Vector2);
			((Vector2)(ref val3))..ctor(flag ? 1f : (-1f), 0f);
			RaycastHit2D val4 = Physics2D.Raycast(val, val3, num + 0.001f, 256);
			if (!Object.op_Implicit((Object)(object)((RaycastHit2D)(ref val4)).collider))
			{
				return;
			}
			float num2 = num - ((RaycastHit2D)(ref val4)).distance;
			if (!(num2 <= 0f) && !(num2 < num * 0.6f))
			{
				if (flag)
				{
					heroCtrl.RecoilLeft();
				}
				else
				{
					heroCtrl.RecoilRight();
				}
				Rigidbody2D val5 = default(Rigidbody2D);
				if (((Component)heroCtrl).TryGetComponent<Rigidbody2D>(ref val5))
				{
					Rigidbody2D obj = val5;
					obj.velocity *= 0.5f;
				}
				_hasBounced = true;
				Object.Destroy((Object)(object)this);
			}
		}
	}
	internal class Amulets : MonoBehaviour
	{
		private PlayMakerFSM _spellControl;

		private PlayMakerFSM _blastControl;

		private PlayMakerFSM _pvControl;

		private GameObject _audioPlayerActor;

		private bool _activated;

		private bool initialized;

		private GameObject _blastKnight;

		private HeroController _hc => HeroController.instance;

		private PlayerData _pd => PlayerData.instance;

		public void Awake()
		{
			EnsureInitialized();
		}

		public void EnsureInitialized()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Expected O, but got Unknown
			if (!initialized)
			{
				initialized = true;
				HeroController.Start += new hook_Start(On_HeroController_Start);
				CharmIconList.GetSprite += new hook_GetSprite(CharmIconList_GetSprite);
				ModHooks.CharmUpdateHook += new CharmUpdateHandler(CharmUpdate);
			}
		}

		private Sprite CharmIconList_GetSprite(orig_GetSprite orig, CharmIconList self, int id)
		{
			if (PaleCourtCharms.Settings.upgradedCharm_10)
			{
				self.spriteList[10] = PaleCourtCharms.SPRITES["Kings_Honour"];
			}
			else
			{
				self.spriteList[10] = PaleCourtCharms.SPRITES["Defenders_Crest"];
			}
			return orig.Invoke(self, id);
		}

		public void On_HeroController_Start(orig_Start orig, HeroController self)
		{
			orig.Invoke(self);
			_pvControl = Object.Instantiate<PlayMakerFSM>(FSMUtility.LocateMyFSM(PaleCourtCharms.preloadedGO["PV"], "Control"), ((Component)_hc).transform);
			PaleCourtCharms.PVControl = _pvControl;
			GameObject val = Object.Instantiate<GameObject>(PaleCourtCharms.preloadedGO["Blast"]);
			val.SetActive(false);
			_blastControl = FSMUtility.LocateMyFSM(val, "Control");
			_spellControl = _hc.spellControl;
			FSMUtility.LocateMyFSM(_spellControl.GetAction<SpawnObjectFromGlobalPool>("Fireball 2", 3).gameObject.Value, "Fireball Cast");
			((Behaviour)((Component)_hc).gameObject.AddComponent<RoyalAura>()).enabled = false;
			((Behaviour)((Component)_hc).gameObject.AddComponent<Purity>()).enabled = false;
			((Behaviour)((Component)_hc).gameObject.AddComponent<BoonSpells>()).enabled = false;
			InsertCharmSpellEffectsInFsm();
			((Behaviour)((Component)_hc).gameObject.AddComponent<LamentControl>()).enabled = false;
			((Behaviour)((Component)_hc).gameObject.AddComponent<ModifyBloomProps>()).enabled = true;
			((Behaviour)((Component)_hc).gameObject.AddComponent<AbyssalBloomBehaviour>()).enabled = false;
			((Behaviour)((Component)_hc).gameObject.AddComponent<CheckBloomStage>()).enabled = true;
			AddVoidAttacks(_hc);
			ModifyFuryForBloom();
			ModifySpellsForBloom();
			FloristInterop.Init();
			_activated = true;
		}

		private void InsertCharmSpellEffectsInFsm()
		{
			FsmUtil.CopyState(_spellControl, "Fireball 1", "Fireball 1 SmallShots");
			FsmUtil.CopyState(_spellControl, "Fireball 2", "Fireball 2 SmallShots");
			_spellControl.RemoveAction<SpawnObjectFromGlobalPool>("Fireball 1 SmallShots");
			_spellControl.RemoveAction<SpawnObjectFromGlobalPool>("Fireball 2 SmallShots");
			_spellControl.InsertMethod("Fireball 1 SmallShots", 3, delegate
			{
				((Component)HeroController.instance).GetComponent<BoonSpells>().CastDaggers(upgraded: false);
			});
			_spellControl.InsertMethod("Fireball 2 SmallShots", 3, delegate
			{
				((Component)HeroController.instance).GetComponent<BoonSpells>().CastDaggers(upgraded: true);
			});
			FsmUtil.CopyState(_spellControl, "Quake1 Land", "Q1 Land Plumes");
			FsmUtil.CopyState(_spellControl, "Q2 Land", "Q2 Land Plumes");
			FsmUtil.ChangeTransition(_spellControl, "Q2 Land Plumes", "FINISHED", "Quake Finish");
			FsmUtil.InsertMethod(_spellControl, "Q1 Land Plumes", (Action)delegate
			{
				((Component)HeroController.instance).GetComponent<BoonSpells>().CastPlumes(upgraded: false);
			}, 0);
			FsmUtil.InsertMethod(_spellControl, "Q2 Land Plumes", (Action)delegate
			{
				((Component)HeroController.instance).GetComponent<BoonSpells>().CastPlumes(upgraded: true);
			}, 0);
			FsmUtil.CopyState(_spellControl, "Scream Antic1", "Scream Antic1 Blasts");
			FsmUtil.CopyState(_spellControl, "Scream Burst 1", "Scream Burst 1 Blasts");
			FsmUtil.CopyState(_spellControl, "Scream Antic2", "Scream Antic2 Blasts");
			FsmUtil.CopyState(_spellControl, "Scream Burst 2", "Scream Burst 2 Blasts");
			FsmUtil.ChangeTransition(_spellControl, "Scream Antic1 Blasts", "FINISHED", "Scream Burst 1 Blasts");
			FsmUtil.ChangeTransition(_spellControl, "Scream Antic2 Blasts", "FINISHED", "Scream Burst 2 Blasts");
			_spellControl.RemoveAction<AudioPlay>("Scream Antic1 Blasts");
			_spellControl.RemoveAction<CreateObject>("Scream Burst 1 Blasts");
			_spellControl.RemoveAction<ActivateGameObject>("Scream Burst 1 Blasts");
			_spellControl.RemoveAction<ActivateGameObject>("Scream Burst 1 Blasts");
			_spellControl.RemoveAction<SendEventByName>("Scream Burst 1 Blasts");
			_spellControl.RemoveAction<SendEventByName>("Scream Burst 1 Blasts");
			_spellControl.InsertMethod("Scream Burst 1 Blasts", 0, delegate
			{
				((Component)HeroController.instance).GetComponent<BoonSpells>().CastBlasts(upgraded: false);
			});
			_spellControl.RemoveAction<AudioPlay>("Scream Antic2 Blasts");
			_spellControl.RemoveAction<CreateObject>("Scream Burst 2 Blasts");
			_spellControl.RemoveAction<ActivateGameObject>("Scream Burst 2 Blasts");
			_spellControl.RemoveAction<ActivateGameObject>("Scream Burst 2 Blasts");
			_spellControl.RemoveAction<SendEventByName>("Scream Burst 2 Blasts");
			_spellControl.RemoveAction<SendEventByName>("Scream Burst 2 Blasts");
			_spellControl.InsertMethod("Scream Burst 2 Blasts", 0, delegate
			{
				((Component)HeroController.instance).GetComponent<BoonSpells>().CastBlasts(upgraded: true);
			});
			FsmUtil.CopyState(_spellControl, "Focus", "Focus Blast");
			FsmUtil.CopyState(_spellControl, "Focus Heal", "Focus Heal Blast");
			FsmUtil.CopyState(_spellControl, "Start MP Drain", "Start MP Drain Blast");
			FsmUtil.CopyState(_spellControl, "Focus Heal 2", "Focus Heal 2 Blast");
			_spellControl.InsertCoroutine("Focus Blast", 0, PureVesselBlastFadeIn);
			_spellControl.InsertCoroutine("Focus Heal Blast", 0, PureVesselBlast);
			_spellControl.InsertCoroutine("Start MP Drain Blast", 0, PureVesselBlastFadeIn);
			_spellControl.InsertCoroutine("Focus Heal 2 Blast", 0, PureVesselBlast);
			_spellControl.InsertMethod("Cancel All", 0, CancelBlast);
			_spellControl.InsertMethod("Focus Cancel", 0, CancelBlast);
			_spellControl.InsertMethod("Focus Cancel 2", 0, CancelBlast);
		}

		private void AddVoidAttacks(HeroController self)
		{
			GameObject val = USceneUtil.FindGameObjectInChildren(((Component)self).gameObject, "Attacks");
			Shader shader = ((tk2dBaseSprite)((Component)self).GetComponent<tk2dSprite>()).Collection.spriteDefinitions[0].material.shader;
			if (!PaleCourtCharms.preloadedGO.ContainsKey("Bloom Sprite Prefab") || !PaleCourtCharms.preloadedGO.ContainsKey("Bloom Anim Prefab"))
			{
				Log("Warning: Bloom Sprite/Anim prefabs not loaded — AddVoidAttacks skipped.");
				return;
			}
			tk2dSpriteCollection component = PaleCourtCharms.preloadedGO["Bloom Sprite Prefab"].GetComponent<tk2dSpriteCollection>();
			tk2dSpriteAnimation component2 = PaleCourtCharms.preloadedGO["Bloom Anim Prefab"].GetComponent<tk2dSpriteAnimation>();
			tk2dSprite component3 = ((Component)self).GetComponent<tk2dSprite>();
			tk2dSpriteAnimator component4 = ((Component)self).GetComponent<tk2dSpriteAnimator>();
			List<tk2dSpriteDefinition> list = ((tk2dBaseSprite)component3).Collection.spriteDefinitions.ToList();
			tk2dSpriteDefinition[] spriteDefinitions = component.spriteCollection.spriteDefinitions;
			foreach (tk2dSpriteDefinition val2 in spriteDefinitions)
			{
				val2.material.shader = shader;
				list.Add(val2);
			}
			((tk2dBaseSprite)component3).Collection.spriteDefinitions = list.ToArray();
			List<tk2dSpriteAnimationClip> list2 = component4.Library.clips.ToList();
			tk2dSpriteAnimationClip[] clips = component2.clips;
			foreach (tk2dSpriteAnimationClip item in clips)
			{
				list2.Add(item);
			}
			component4.Library.clips = list2.ToArray();
			GameObject cycloneSlashVoid = Object.Instantiate<GameObject>(USceneUtil.FindGameObjectInChildren(val, "Cyclone Slash"), val.transform);
			((Object)cycloneSlashVoid).name = "Cyclone Slash Void";
			cycloneSlashVoid.GetComponent<tk2dSpriteAnimator>().DefaultClipId = component4.GetClipIdByName("Cyclone Slash Effect Void");
			GameObject dashSlashVoid = Object.Instantiate<GameObject>(USceneUtil.FindGameObjectInChildren(val, "Dash Slash"), val.transform);
			((Object)dashSlashVoid).name = "Dash Slash Void";
			dashSlashVoid.GetComponent<tk2dSpriteAnimator>().DefaultClipId = component4.GetClipIdByName("Dash Slash Effect Void");
			GameObject greatSlashVoid = Object.Instantiate<GameObject>(USceneUtil.FindGameObjectInChildren(val, "Great Slash"), val.transform);
			((Object)greatSlashVoid).name = "Great Slash Void";
			greatSlashVoid.GetComponent<tk2dSpriteAnimator>().DefaultClipId = component4.GetClipIdByName("Great Slash Effect Void");
			PlayMakerFSM nailArts = FSMUtility.LocateMyFSM(((Component)self).gameObject, "Nail Arts");
			if (nailArts.FsmStates[0].Fsm == null)
			{
				nailArts.Preprocess();
			}
			FsmUtil.AddState(nailArts, "Bloom Activated CSlash?");
			FsmUtil.AddState(nailArts, "Bloom Activated DSlash?");
			FsmUtil.AddState(nailArts, "Bloom Activated GSlash?");
			FsmUtil.CopyState(nailArts, "Cyclone Start", "Cyclone Start Void");
			FsmUtil.CopyState(nailArts, "Hover Start", "Hover Start Void");
			FsmUtil.CopyState(nailArts, "Activate Slash", "Activate Slash Void");
			FsmUtil.CopyState(nailArts, "Play Audio", "Play Audio Void");
			FsmUtil.CopyState(nailArts, "Cyclone Spin", "Cyclone Spin Void");
			FsmUtil.CopyState(nailArts, "Cyclone Extend", "Cyclone Extend Void");
			FsmUtil.CopyState(nailArts, "Cyclone End", "Cyclone End Void");
			FsmUtil.CopyState(nailArts, "Dash Slash", "Dash Slash Void");
			FsmUtil.CopyState(nailArts, "DSlash Move End", "DSlash Move End Void");
			FsmUtil.CopyState(nailArts, "D Slash End", "D Slash End Void");
			FsmUtil.CopyState(nailArts, "G Slash", "G Slash Void");
			FsmUtil.CopyState(nailArts, "Stop Move", "Stop Move Void");
			FsmUtil.CopyState(nailArts, "G Slash End", "G Slash End Void");
			FsmUtil.ChangeTransition(nailArts, "Flash", "FINISHED", "Bloom Activated CSlash?");
			FsmUtil.ChangeTransition(nailArts, "Cyclone Start Void", "FINISHED", "Activate Slash Void");
			FsmUtil.ChangeTransition(nailArts, "Cyclone Start Void", "BUTTON DOWN", "Hover Start Void");
			FsmUtil.ChangeTransition(nailArts, "Hover Start Void", "FINISHED", "Cyclone Start Void");
			FsmUtil.ChangeTransition(nailArts, "Activate Slash Void", "FINISHED", "Play Audio Void");
			FsmUtil.ChangeTransition(nailArts, "Play Audio Void", "FINISHED", "Cyclone Spin Void");
			FsmUtil.ChangeTransition(nailArts, "Cyclone Spin Void", "BUTTON DOWN", "Cyclone Extend Void");
			FsmUtil.ChangeTransition(nailArts, "Cyclone Spin Void", "END", "Cyclone End Void");
			FsmUtil.ChangeTransition(nailArts, "Cyclone Extend Void", "END", "Cyclone End Void");
			FsmUtil.ChangeTransition(nailArts, "Cyclone Extend Void", "WAIT", "Cyclone Spin Void");
			FsmUtil.ChangeTransition(nailArts, "Left 2", "FINISHED", "Bloom Activated DSlash?");
			FsmUtil.ChangeTransition(nailArts, "Right 2", "FINISHED", "Bloom Activated DSlash?");
			FsmUtil.ChangeTransition(nailArts, "Dash Slash Void", "FINISHED", "DSlash Move End Void");
			FsmUtil.ChangeTransition(nailArts, "DSlash Move End Void", "FINISHED", "D Slash End Void");
			FsmUtil.ChangeTransition(nailArts, "Left", "FINISHED", "Bloom Activated GSlash?");
			FsmUtil.ChangeTransition(nailArts, "Right", "FINISHED", "Bloom Activated GSlash?");
			FsmUtil.ChangeTransition(nailArts, "G Slash Void", "FINISHED", "Stop Move Void");
			FsmUtil.ChangeTransition(nailArts, "Stop Move Void", "FINISHED", "G Slash End Void");
			nailArts.AddTransition("Bloom Activated CSlash?", "VOID", "Cyclone Start Void");
			nailArts.AddTransition("Bloom Activated CSlash?", "NORMAL", "Cyclone Start");
			nailArts.AddTransition("Bloom Activated DSlash?", "VOID", "Dash Slash Void");
			nailArts.AddTransition("Bloom Activated DSlash?", "NORMAL", "Dash Slash");
			nailArts.AddTransition("Bloom Activated GSlash?", "VOID", "G Slash Void");
			nailArts.AddTransition("Bloom Activated GSlash?", "NORMAL", "G Slash");
			FsmUtil.AddMethod(nailArts, "Bloom Activated CSlash?", (Action)delegate
			{
				nailArts.SetState((PaleCourtCharms.Settings.equippedCharms[3] && _pd.health <= 1) ? "Cyclone Start Void" : "Cyclone Start");
			});
			FsmUtil.AddMethod(nailArts, "Bloom Activated DSlash?", (Action)delegate
			{
				nailArts.SetState((PaleCourtCharms.Settings.equippedCharms[3] && _pd.health <= 1) ? "Dash Slash Void" : "Dash Slash");
			});
			FsmUtil.AddMethod(nailArts, "Bloom Activated GSlash?", (Action)delegate
			{
				nailArts.SetState((PaleCourtCharms.Settings.equippedCharms[3] && _pd.health <= 1) ? "G Slash Void" : "G Slash");
			});
			nailArts.GetAction<Tk2dPlayAnimationWithEvents>("Cyclone Start Void").clipName = FsmString.op_Implicit("NA Cyclone Start Void");
			nailArts.GetAction<Tk2dPlayAnimation>("Cyclone Spin Void").clipName = FsmString.op_Implicit("NA Cyclone Void");
			nailArts.GetAction<Tk2dPlayAnimation>("Cyclone Extend Void").clipName = FsmString.op_Implicit("NA Cyclone Void");
			nailArts.GetAction<Tk2dPlayAnimationWithEvents>("Cyclone End Void").clipName = FsmString.op_Implicit("NA Cyclone End Void");
			nailArts.GetAction<Tk2dPlayAnimationWithEvents>("Dash Slash Void").clipName = FsmString.op_Implicit("NA Dash Slash Void");
			nailArts.GetAction<Tk2dPlayAnimationWithEvents>("G Slash Void").clipName = FsmString.op_Implicit("NA Big Slash Void");
			nailArts.InsertMethod("Activate Slash Void", 0, delegate
			{
				cycloneSlashVoid.SetActive(true);
				cycloneSlashVoid.GetComponent<tk2dSpriteAnimator>().Play("Cyclone Slash Effect Void");
			});
			nailArts.InsertMethod("Cyclone End Void", 2, delegate
			{
				cycloneSlashVoid.SetActive(false);
			});
			FsmUtil.AddMethod(nailArts, "Cancel All", (Action)delegate
			{
				cycloneSlashVoid.SetActive(false);
			});
			nailArts.InsertMethod("Dash Slash Void", 0, delegate
			{
				dashSlashVoid.SetActive(true);
				dashSlashVoid.GetComponent<tk2dSpriteAnimator>().Play("Dash Slash Effect Void");
			});
			nailArts.InsertMethod("D Slash End Void", 0, delegate
			{
				dashSlashVoid.SetActive(false);
			});
			FsmUtil.AddMethod(nailArts, "Cancel All", (Action)delegate
			{
				dashSlashVoid.SetActive(false);
			});
			nailArts.InsertMethod("G Slash Void", 0, delegate
			{
				greatSlashVoid.SetActive(true);
				greatSlashVoid.GetComponent<tk2dSpriteAnimator>().Play("Great Slash Effect Void");
			});
			nailArts.InsertMethod("G Slash End Void", 0, delegate
			{
				greatSlashVoid.SetActive(false);
			});
			FsmUtil.AddMethod(nailArts, "Cancel All", (Action)delegate
			{
				greatSlashVoid.SetActive(false);
			});
			nailArts.RemoveAction<ActivateGameObject>("Activate Slash Void");
			nailArts.RemoveAction<ActivateGameObject>("Dash Slash Void");
			nailArts.RemoveAction<ActivateGameObject>("G Slash Void");
			FsmUtil.Log(nailArts);
			((MonoBehaviour)this).StartCoroutine(ResetVoidNarts((GameObject[])(object)new GameObject[3] { cycloneSlashVoid, dashSlashVoid, greatSlashVoid }));
		}

		private IEnumerator ResetVoidNarts(GameObject[] narts)
		{
			GameObject[] array = narts;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
			yield return (object)new WaitForEndOfFrame();
			array = narts;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
		}

		private void ModifyFuryForBloom()
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				PlayMakerFSM fury = FSMUtility.LocateMyFSM(USceneUtil.FindGameObjectInChildren(((Component)_hc).gameObject, "Charm Effects"), "Fury");
				Logger.LogFine("Fury Color: " + ((object)fury.GetAction<Tk2dSpriteSetColor>("Activate", 17).color.Value/*cast due to constrained. prefix*/).ToString());
				Color furyColor = fury.GetAction<Tk2dSpriteSetColor>("Activate", 18).color.Value;
				fury.InsertMethod("Activate", 17, delegate
				{
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					//IL_000f: Unknown result type (might be due to invalid IL or missing references)
					//IL_001b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0033: Unknown result type (might be due to invalid IL or missing references)
					//IL_0050: Unknown result type (might be due to invalid IL or missing references)
					//IL_006d: Unknown result type (might be due to invalid IL or missing references)
					Color value = (PaleCourtCharms.Settings.equippedCharms[3] ? Color.black : furyColor);
					fury.GetAction<Tk2dSpriteSetColor>("Activate", 18).color.Value = value;
					fury.GetAction<Tk2dSpriteSetColor>("Activate", 19).color.Value = value;
					fury.GetAction<Tk2dSpriteSetColor>("Activate", 20).color.Value = value;
					((Component)_hc).GetComponent<AbyssalBloomBehaviour>().SetFury(fury: true);
				});
				fury.InsertMethod("Stay Furied", 4, delegate
				{
					((Component)_hc).GetComponent<AbyssalBloomBehaviour>().SetFury(fury: true);
				});
				fury.InsertMethod("Deactivate", 21, delegate
				{
					((Component)_hc).GetComponent<AbyssalBloomBehaviour>().SetFury(fury: false);
				});
			}
			catch (NullReferenceException)
			{
			}
		}

		private void ModifySpellsForBloom()
		{
			FsmUtil.InsertMethod(_spellControl, "Wallside?", (Action)delegate
			{
				((Component)_hc).GetComponent<AbyssalBloomBehaviour>().CancelTendrilAttack();
				((Component)_hc).GetComponent<AbyssalBloomBehaviour>().CancelVerticalTendrilAttack();
				((Component)_hc).GetComponent<AbyssalBloomBehaviour>().CancelWallTendrilAttack();
			}, 0);
			FsmUtil.InsertMethod(_spellControl, "Quake Antic", (Action)delegate
			{
				((Component)_hc).GetComponent<AbyssalBloomBehaviour>().CancelTendrilAttack();
				((Component)_hc).GetComponent<AbyssalBloomBehaviour>().CancelVerticalTendrilAttack();
				((Component)_hc).GetComponent<AbyssalBloomBehaviour>().CancelWallTendrilAttack();
			}, 0);
			FsmUtil.InsertMethod(_spellControl, "Level Check 3", (Action)delegate
			{
				((Component)_hc).GetComponent<AbyssalBloomBehaviour>().CancelTendrilAttack();
				((Component)_hc).GetComponent<AbyssalBloomBehaviour>().CancelVerticalTendrilAttack();
				((Component)_hc).GetComponent<AbyssalBloomBehaviour>().CancelWallTendrilAttack();
			}, 0);
		}

		private void CharmUpdate(PlayerData playerData, HeroController hc)
		{
			if (!_activated)
			{
				return;
			}
			((Behaviour)((Component)_hc).GetComponent<RoyalAura>()).enabled = playerData.GetBool("equippedCharm_" + 10) && PaleCourtCharms.Settings.upgradedCharm_10;
			((Behaviour)((Component)_hc).GetComponent<Purity>()).enabled = PaleCourtCharms.Settings.equippedCharms[0];
			((Behaviour)((Component)_hc).GetComponent<LamentControl>()).enabled = PaleCourtCharms.Settings.equippedCharms[1];
			if (PaleCourtCharms.Settings.equippedCharms[1])
			{
				FsmState[] fsmStates = _spellControl.FsmStates;
				foreach (FsmState val in fsmStates)
				{
					FsmTransition[] transitions = val.Transitions;
					foreach (FsmTransition val2 in transitions)
					{
						if (val2.ToState == "Focus")
						{
							FsmUtil.ChangeTransition(val, val2.EventName, "Focus Blast");
						}
						else if (val2.ToState == "Focus Heal")
						{
							FsmUtil.ChangeTransition(val, val2.EventName, "Focus Heal Blast");
						}
						else if (val2.ToState == "Start MP Drain")
						{
							FsmUtil.ChangeTransition(val, val2.EventName, "Start MP Drain Blast");
						}
						else if (val2.ToState == "Focus Heal 2")
						{
							FsmUtil.ChangeTransition(val, val2.EventName, "Focus Heal 2 Blast");
						}
					}
				}
			}
			else
			{
				FsmState[] fsmStates = _spellControl.FsmStates;
				foreach (FsmState val3 in fsmStates)
				{
					FsmTransition[] transitions = val3.Transitions;
					foreach (FsmTransition val4 in transitions)
					{
						if (val4.ToState == "Focus Blast")
						{
							FsmUtil.ChangeTransition(val3, val4.EventName, "Focus");
						}
						else if (val4.ToState == "Focus Heal Blast")
						{
							FsmUtil.ChangeTransition(val3, val4.EventName, "Focus Heal");
						}
						else if (val4.ToState == "Start MP Drain Blast")
						{
							FsmUtil.ChangeTransition(val3, val4.EventName, "Start MP Drain");
						}
						else if (val4.ToState == "Focus Heal 2 Blast")
						{
							FsmUtil.ChangeTransition(val3, val4.EventName, "Focus Heal 2");
						}
					}
				}
			}
			((Behaviour)((Component)_hc).GetComponent<BoonSpells>()).enabled = false;
			((Behaviour)((Component)_hc).GetComponent<BoonSpells>()).enabled = PaleCourtCharms.Settings.equippedCharms[2];
			((Behaviour)((Component)_hc).GetComponent<AbyssalBloomBehaviour>()).enabled = PaleCourtCharms.Settings.equippedCharms[3];
		}

		private IEnumerator PureVesselBlastFadeIn()
		{
			((MonoBehaviour)(object)this).PlayAudio((AudioClip)_pvControl.GetAction<AudioPlayerOneShotSingle>("Focus Charge", 2).audioClip.Value);
			_blastKnight = Object.Instantiate<GameObject>(PaleCourtCharms.preloadedGO["Blast"], ((Component)HeroController.instance).transform);
			Transform transform = _blastKnight.transform;
			transform.localPosition += Vector3.up * 0.25f;
			_blastKnight.SetActive(true);
			Object.Destroy((Object)(object)USceneUtil.FindGameObjectInChildren(_blastKnight, "hero_damager"));
			if (_pd.GetBool("equippedCharm_" + 34))
			{
				Transform transform2 = _blastKnight.transform;
				transform2.localScale *= 2.5f;
			}
			else
			{
				Transform transform3 = _blastKnight.transform;
				transform3.localScale *= 1.5f;
			}
			Animator component = _blastKnight.GetComponent<Animator>();
			component.speed = 1f;
			if (_pd.GetBool("equippedCharm_" + 7))
			{
				component.speed *= 1.3f;
			}
			if (_pd.GetBool("equippedCharm_" + 34))
			{
				component.speed -= component.speed * 0.35f;
			}
			yield return null;
		}

		private IEnumerator PureVesselBlast()
		{
			if ((Object)(object)_blastKnight != (Object)null)
			{
				_blastKnight.layer = 17;
				Animator component = _blastKnight.GetComponent<Animator>();
				component.speed = 1f;
				AnimatorStateInfo currentAnimatorStateInfo = component.GetCurrentAnimatorStateInfo(0);
				int fullPathHash = ((AnimatorStateInfo)(ref currentAnimatorStateInfo)).fullPathHash;
				component.PlayInFixedTime(fullPathHash, -1, 0.8f);
				Logger.LogFine("Adding CircleCollider2D");
				CircleCollider2D blastCollider = _blastKnight.AddComponent<CircleCollider2D>();
				blastCollider.radius = 2.5f;
				if (_pd.GetBool("equippedCharm_" + 34))
				{
					blastCollider.radius = 3.5f;
				}
				else
				{
					blastCollider.radius *= 1.5f;
				}
				((Collider2D)blastCollider).offset = Vector2.op_Implicit(Vector3.down);
				((Collider2D)blastCollider).isTrigger = true;
				Logger.LogFine("Adding DebugColliders");
				Logger.LogFine("Adding DamageEnemies");
				_blastKnight.AddComponent<DamageEnemies>();
				DamageEnemies component2 = _blastKnight.GetComponent<DamageEnemies>();
				int num = 40;
				float num2 = 1f;
				if (_pd.GetBool("equippedCharm_" + 34))
				{
					num2 *= 2f;
				}
				if (FloristInterop.IsEquipped())
				{
					num2 *= 2f;
				}
				component2.damageDealt = Mathf.Max(1, Mathf.RoundToInt((float)num * num2));
				component2.attackType = (AttackTypes)2;
				component2.ignoreInvuln = false;
				((Behaviour)component2).enabled = true;
				Logger.LogFine("Playing AudioClip");
				((MonoBehaviour)(object)this).PlayAudio((AudioClip)_pvControl.GetAction<AudioPlayerOneShotSingle>("Focus Burst", 8).audioClip.Value, 1.5f);
				Logger.LogFine("Audio Clip finished");
				yield return (object)new WaitForSeconds(0.11f);
				((Behaviour)blastCollider).enabled = false;
				yield return (object)new WaitForSeconds(0.69f);
				Object.Destroy((Object)(object)_blastKnight);
			}
		}

		private void CancelBlast()
		{
			if ((Object)(object)_blastKnight != (Object)null)
			{
				Object.Destroy((Object)(object)_blastKnight);
			}
		}

		private void OnDestroy()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			if (initialized)
			{
				initialized = false;
				Log("Destroyed Amulets");
				HeroController.Start -= new hook_Start(On_HeroController_Start);
				ModHooks.CharmUpdateHook -= new CharmUpdateHandler(CharmUpdate);
			}
		}

		private static void Log(object message)
		{
			Logger.Log("[PC_Charms][Amulets] " + message);
		}
	}
	public class BoonSpells : MonoBehaviour
	{
		private HeroController _hc = HeroController.instance;

		private PlayMakerFSM _spellControl;

		private GameObject _audioPlayer;

		private const float DaggerSpeed = 50f;

		private const int BlastDamage = 35;

		private const int BlastDamageShaman = 45;

		private bool _transcChecked;

		private bool _transcAvailable;

		private object _shamanInstance;

		private MethodInfo _shamanEquippedMethod;

		private MethodInfo _shamanEnlargeStatic;

		private Type _shamanType;

		private bool _shamanEquippedCached;

		private bool _snailChecked;

		private bool _snailAvailable;

		private object _snailInstance;

		private MethodInfo _snailEquippedMethod;

		private Type _snailType;

		private bool _snailEquippedCached;

		private const float SnailSlowdown = 4f;

		private bool _vespaChecked;

		private bool _vespaAvailable;

		private object _vespaInstance;

		private MethodInfo _vespaEquippedMethod;

		private Type _vespaType;

		private bool _vespaEquippedCached;

		private void OnEnable()
		{
			if (!((Object)(object)HeroController.instance == (Object)null) && !((Object)(object)HeroController.instance.spellControl == (Object)null))
			{
				InitOnce();
			}
		}

		private void InitOnce()
		{
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Expected O, but got Unknown
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Expected O, but got Unknown
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Expected O, but got Unknown
			try
			{
				_hc = HeroController.instance;
				_spellControl = _hc?.spellControl;
				if ((Object)(object)_spellControl == (Object)null)
				{
					return;
				}
				PlayMakerFSM fsm = FSMUtility.LocateMyFSM(_spellControl.GetAction<SpawnObjectFromGlobalPool>("Fireball 2", 3).gameObject.Value, "Fireball Cast");
				_audioPlayer = fsm.GetAction<AudioPlayerOneShotSingle>("Cast Right", 3).audioPlayer.Value;
				PlayMakerFSM fsm2 = Object.Instantiate<PlayMakerFSM>(FSMUtility.LocateMyFSM(PaleCourtCharms.preloadedGO["PV"], "Control"), ((Component)_hc).transform);
				if (!PaleCourtCharms.preloadedGO.ContainsKey("Plume"))
				{
					GameObject val = Object.Instantiate<GameObject>(fsm2.GetAction<SpawnObjectFromGlobalPool>("Plume Gen", 0).gameObject.Value);
					val.SetActive(false);
					val.layer = 17;
					val.tag = "Hero Spell";
					Object.Destroy((Object)(object)val.GetComponent<DamageHero>());
					Object.DontDestroyOnLoad((Object)(object)val);
					PaleCourtCharms.preloadedGO["Plume"] = val;
				}
				if (!PaleCourtCharms.preloadedGO.ContainsKey("BoonDagger"))
				{
					GameObject val2 = Object.Instantiate<GameObject>(fsm2.GetAction<FlingObjectsFromGlobalPoolTime>("SmallShot LowHigh").gameObject.Value);
					val2.SetActive(false);
					val2.layer = 17;
					val2.tag = "Hero Spell";
					Object.Destroy((Object)(object)val2.GetComponent<DamageHero>());
					Object.Destroy((Object)(object)FSMUtility.LocateMyFSM(val2, "Control"));
					GameObject val3 = USceneUtil.FindGameObjectInChildren(val2, "Dribble L");
					if ((Object)(object)val3 != (Object)null)
					{
						val3.layer = 9;
					}
					GameObject val4 = USceneUtil.FindGameObjectInChildren(val2, "Glow");
					if ((Object)(object)val4 != (Object)null)
					{
						val4.layer = 9;
					}
					GameObject val5 = USceneUtil.FindGameObjectInChildren(val2, "Beam");
					if ((Object)(object)val5 != (Object)null)
					{
						val5.layer = 9;
					}
					Object.DontDestroyOnLoad((Object)(object)val2);
					PaleCourtCharms.preloadedGO["BoonDagger"] = val2;
				}
				PaleCourtCharms.Clips["Burst"] = (AudioClip)fsm2.GetAction<AudioPlayerOneShotSingle>("Focus Burst", 8).audioClip.Value;
				PaleCourtCharms.Clips["Plume Up"] = (AudioClip)fsm2.GetAction<AudioPlayerOneShotSingle>("Plume Up", 1).audioClip.Value;
				ModifySpellFSM(enabled: true);
				ModHooks.CharmUpdateHook += new CharmUpdateHandler(OnCharmUpdate);
				UpdateAllCharmCaches();
			}
			catch (Exception)
			{
			}
		}

		private void OnDisable()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			ModifySpellFSM(enabled: false);
			ModHooks.CharmUpdateHook -= new CharmUpdateHandler(OnCharmUpdate);
		}

		private void OnCharmUpdate(PlayerData pd, HeroController hc)
		{
			UpdateAllCharmCaches();
		}

		private void UpdateAllCharmCaches()
		{
			UpdateShamanAmpEquippedCache();
			UpdateSnailEquippedCache();
			UpdateVespaEquippedCache();
		}

		private void UpdateShamanAmpEquippedCache()
		{
			if (!_transcChecked)
			{
				TryInitTranscendenceIntegration();
			}
			if (!_transcAvailable || _shamanEquippedMethod == null || _shamanType == null)
			{
				_shamanEquippedCached = false;
				return;
			}
			if (_shamanInstance == null && _shamanType != null)
			{
				try
				{
					PropertyInfo property = _shamanType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
					if (property != null)
					{
						_shamanInstance = property.GetValue(null);
					}
					else
					{
						FieldInfo field = _shamanType.GetField("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
						if (field != null)
						{
							_shamanInstance = field.GetValue(null);
						}
					}
				}
				catch
				{
					_shamanInstance = null;
				}
			}
			if (_shamanInstance == null)
			{
				_shamanEquippedCached = false;
				return;
			}
			try
			{
				_shamanEquippedCached = (bool)_shamanEquippedMethod.Invoke(_shamanInstance, null);
			}
			catch
			{
				_shamanEquippedCached = false;
			}
		}

		private void UpdateSnailEquippedCache()
		{
			if (!_snailChecked)
			{
				TryInitSnailIntegration();
			}
			if (!_snailAvailable || _snailEquippedMethod == null || _snailType == null)
			{
				_snailEquippedCached = false;
				return;
			}
			if (_snailInstance == null && _snailType != null)
			{
				try
				{
					PropertyInfo property = _snailType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
					if (property != null)
					{
						_snailInstance = property.GetValue(null);
					}
					else
					{
						FieldInfo field = _snailType.GetField("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
						if (field != null)
						{
							_snailInstance = field.GetValue(null);
						}
					}
				}
				catch
				{
					_snailInstance = null;
				}
			}
			if (_snailInstance == null)
			{
				_snailEquippedCached = false;
				return;
			}
			try
			{
				_snailEquippedCached = (bool)_snailEquippedMethod.Invoke(_snailInstance, null);
			}
			catch
			{
				_snailEquippedCached = false;
			}
		}

		private void ModifySpellFSM(bool enabled)
		{
			if ((Object)(object)_spellControl == (Object)null)
			{
				return;
			}
			if (enabled)
			{
				if (!IsVespaEquipped())
				{
					FsmUtil.ChangeTransition(_spellControl, "Level Check 3", "LEVEL 1", "Scream Antic1 Blasts");
					FsmUtil.ChangeTransition(_spellControl, "Level Check 3", "LEVEL 2", "Scream Antic2 Blasts");
				}
				FsmUtil.ChangeTransition(_spellControl, "Quake1 Down", "HERO LANDED", "Q1 Land Plumes");
				FsmUtil.ChangeTransition(_spellControl, "Quake2 Down", "HERO LANDED", "Q2 Land Plumes");
				if (!PlayerData.instance.GetBool("equippedCharm_11"))
				{
					FsmUtil.ChangeTransition(_spellControl, "Level Check", "LEVEL 1", "Fireball 1 SmallShots");
					FsmUtil.ChangeTransition(_spellControl, "Level Check", "LEVEL 2", "Fireball 2 SmallShots");
				}
			}
			else
			{
				if (!IsVespaEquipped())
				{
					FsmUtil.ChangeTransition(_spellControl, "Level Check 3", "LEVEL 1", "Scream Antic1");
					FsmUtil.ChangeTransition(_spellControl, "Level Check 3", "LEVEL 2", "Scream Antic2");
				}
				FsmUtil.ChangeTransition(_spellControl, "Quake1 Down", "HERO LANDED", "Quake1 Land");
				FsmUtil.ChangeTransition(_spellControl, "Quake2 Down", "HERO LANDED", "Q2 Land");
				FsmUtil.ChangeTransition(_spellControl, "Level Check", "LEVEL 1", "Fireball 1");
				FsmUtil.ChangeTransition(_spellControl, "Level Check", "LEVEL 2", "Fireball 2");
			}
		}

		private void TryInitTranscendenceIntegration()
		{
			if (_transcChecked)
			{
				return;
			}
			_transcChecked = true;
			try
			{
				IMod mod = ModHooks.GetMod("Transcendence", false, false);
				Assembly assembly = null;
				Mod val = (Mod)(object)((mod is Mod) ? mod : null);
				assembly = ((val == null) ? AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(delegate(Assembly a)
				{
					try
					{
						return a.GetType("Transcendence.ShamanAmp") != null;
					}
					catch
					{
						return false;
					}
				}) : ((object)val).GetType().Assembly);
				if (assembly == null)
				{
					_transcAvailable = false;
					return;
				}
				_shamanType = assembly.GetType("Transcendence.ShamanAmp");
				if (_shamanType == null)
				{
					_transcAvailable = false;
					return;
				}
				PropertyInfo property = _shamanType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
				if (property != null)
				{
					_shamanInstance = property.GetValue(null);
				}
				else
				{
					FieldInfo field = _shamanType.GetField("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
					if (field != null)
					{
						_shamanInstance = field.GetValue(null);
					}
				}
				_shamanEquippedMethod = _shamanType.GetMethod("Equipped", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				_shamanEnlargeStatic = _shamanType.GetMethod("Enlarge", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy, null, new Type[1] { typeof(GameObject) }, null);
				_transcAvailable = _shamanEquippedMethod != null || _shamanEnlargeStatic != null;
			}
			catch
			{
				_transcAvailable = false;
				_shamanInstance = null;
				_shamanEquippedMethod = null;
				_shamanEnlargeStatic = null;
				_shamanType = null;
			}
		}

		private void MaybeCallTranscendenceEnlarge(GameObject obj)
		{
			if (!_transcAvailable || _shamanEnlargeStatic == null)
			{
				return;
			}
			try
			{
				_shamanEnlargeStatic.Invoke(null, new object[1] { obj });
			}
			catch
			{
			}
		}

		private void TryInitSnailIntegration()
		{
			if (_snailChecked)
			{
				return;
			}
			_snailChecked = true;
			try
			{
				IMod mod = ModHooks.GetMod("Transcendence", false, false);
				Assembly assembly = null;
				Mod val = (Mod)(object)((mod is Mod) ? mod : null);
				assembly = ((val == null) ? AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(delegate(Assembly a)
				{
					try
					{
						return a.GetType("Transcendence.SnailSoul") != null;
					}
					catch
					{
						return false;
					}
				}) : ((object)val).GetType().Assembly);
				if (assembly == null)
				{
					_snailAvailable = false;
					return;
				}
				_snailType = assembly.GetType("Transcendence.SnailSoul");
				if (_snailType == null)
				{
					_snailAvailable = false;
					return;
				}
				PropertyInfo property = _snailType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
				if (property != null)
				{
					_snailInstance = property.GetValue(null);
				}
				else
				{
					FieldInfo field = _snailType.GetField("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
					if (field != null)
					{
						_snailInstance = field.GetValue(null);
					}
				}
				_snailEquippedMethod = _snailType.GetMethod("Equipped", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				_snailAvailable = _snailEquippedMethod != null;
			}
			catch
			{
				_snailAvailable = false;
				_snailInstance = null;
				_snailEquippedMethod = null;
				_snailType = null;
			}
		}

		private void TryInitVespaIntegration()
		{
			if (_vespaChecked)
			{
				return;
			}
			_vespaChecked = true;
			try
			{
				IMod mod = ModHooks.GetMod("Transcendence", false, false);
				Assembly assembly = null;
				Mod val = (Mod)(object)((mod is Mod) ? mod : null);
				assembly = ((val == null) ? AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(delegate(Assembly a)
				{
					try
					{
						return a.GetType("Transcendence.VespasVengeance") != null;
					}
					catch
					{
						return false;
					}
				}) : ((object)val).GetType().Assembly);
				if (assembly == null)
				{
					_vespaAvailable = false;
					return;
				}
				_vespaType = assembly.GetType("Transcendence.VespasVengeance");
				if (_vespaType == null)
				{
					_vespaAvailable = false;
					return;
				}
				PropertyInfo property = _vespaType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
				if (property != null)
				{
					_vespaInstance = property.GetValue(null);
				}
				else
				{
					FieldInfo field = _vespaType.GetField("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
					if (field != null)
					{
						_vespaInstance = field.GetValue(null);
					}
				}
				_vespaEquippedMethod = _vespaType.GetMethod("Equipped", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				_vespaAvailable = _vespaEquippedMethod != null;
			}
			catch
			{
				_vespaAvailable = false;
				_vespaInstance = null;
				_vespaEquippedMethod = null;
				_vespaType = null;
			}
		}

		private void UpdateVespaEquippedCache()
		{
			if (!_vespaChecked)
			{
				TryInitVespaIntegration();
			}
			if (!_vespaAvailable || _vespaEquippedMethod == null || _vespaType == null)
			{
				_vespaEquippedCached = false;
				return;
			}
			if (_vespaInstance == null && _vespaType != null)
			{
				try
				{
					PropertyInfo property = _vespaType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
					if (property != null)
					{
						_vespaInstance = property.GetValue(null);
					}
					else
					{
						FieldInfo field = _vespaType.GetField("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
						if (field != null)
						{
							_vespaInstance = field.GetValue(null);
						}
					}
				}
				catch
				{
					_vespaInstance = null;
				}
			}
			if (_vespaInstance == null)
			{
				_vespaEquippedCached = false;
				return;
			}
			try
			{
				_vespaEquippedCached = (bool)_vespaEquippedMethod.Invoke(_vespaInstance, null);
			}
			catch
			{
				_vespaEquippedCached = false;
			}
		}

		private bool IsVespaEquipped()
		{
			if (!_vespaChecked)
			{
				TryInitVespaIntegration();
			}
			if (!_vespaAvailable || _vespaEquippedMethod == null || _vespaType == null)
			{
				return false;
			}
			if (_vespaInstance == null)
			{
				try
				{
					PropertyInfo property = _vespaType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
					if (property != null)
					{
						_vespaInstance = property.GetValue(null);
					}
					else
					{
						FieldInfo field = _vespaType.GetField("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
						if (field != null)
						{
							_vespaInstance = field.GetValue(null);
						}
					}
				}
				catch
				{
					_vespaInstance = null;
				}
			}
			if (_vespaInstance == null)
			{
				return false;
			}
			try
			{
				return (bool)_vespaEquippedMethod.Invoke(_vespaInstance, null);
			}
			catch
			{
				return false;
			}
		}

		public void CastDaggers(bool upgraded)
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			bool equippedCharm_ = PlayerData.instance.equippedCharm_19;
			int num = (equippedCharm_ ? (-30) : (-25));
			int num2 = (equippedCharm_ ? 30 : 25);
			int num3 = (equippedCharm_ ? 20 : 25);
			float num4 = 1f;
			if (_snailEquippedCached)
			{
				num4 = 0.25f;
			}
			for (int i = num; i <= num2; i += num3)
			{
				GameObject val = Object.Instantiate<GameObject>(PaleCourtCharms.preloadedGO["BoonDagger"], ((Component)HeroController.instance).transform.position, Quaternion.identity);
				val.SetActive(false);
				if (i != num)
				{
					Object.Destroy((Object)(object)val.GetComponent<AudioSource>());
				}
				if (_shamanEquippedCached)
				{
					MaybeCallTranscendenceEnlarge(val);
				}
				Rigidbody2D component = val.GetComponent<Rigidbody2D>();
				component.isKinematic = true;
				float num5 = 50f * Mathf.Cos((float)Math.PI / 180f * (float)i) * (0f - ((Component)HeroController.instance).transform.localScale.x);
				float num6 = 50f * Mathf.Sin((float)Math.PI / 180f * (float)i);
				float num7 = num5 * num4;
				float num8 = num6 * num4;
				val.SetActive(true);
				component.velocity = new Vector2(num7, num8);
				Dagger dagger = val.AddComponent<Dagger>();
				dagger.upgraded = upgraded;
				dagger.shamanEquipped = _shamanEquippedCached;
				dagger.snailEquipped = _snailEquippedCached;
				Object.Destroy((Object)(object)val, 5f);
			}
		}

		public void CastPlumes(bool upgraded)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			for (float num = 2f; num <= 10f; num += 2f)
			{
				Vector2 val = Vector2.op_Implicit(((Component)HeroController.instance).transform.position);
				float num2 = val.y - 1.8f;
				GameObject val2 = Object.Instantiate<GameObject>(PaleCourtCharms.preloadedGO["Plume"], Vector2.op_Implicit(new Vector2(val.x - num, num2)), Quaternion.identity);
				if (_shamanEquippedCached)
				{
					MaybeCallTranscendenceEnlarge(val2);
				}
				val2.SetActive(true);
				val2.AddComponent<Plume>().upgraded = upgraded;
				GameObject val3 = Object.Instantiate<GameObject>(PaleCourtCharms.preloadedGO["Plume"], Vector2.op_Implicit(new Vector2(val.x + num, num2)), Quaternion.identity);
				if (_shamanEquippedCached)
				{
					MaybeCallTranscendenceEnlarge(val3);
				}
				val3.SetActive(true);
				val3.AddComponent<Plume>().upgraded = upgraded;
			}
			PlayAudio("Plume Up", 1.5f, 1.5f, 0.5f, 0.25f);
		}

		public void CastBlasts(bool upgraded)
		{
			List<GameObject> blasts = new List<GameObject>();
			((MonoBehaviour)this).StartCoroutine(CastBlastsCoro());
			((MonoBehaviour)this).StartCoroutine(DisableColliderCoro());
			((MonoBehaviour)this).StartCoroutine(DestroyBlastsCoro());
			IEnumerator CastBlastsCoro()
			{
				GameObject item = SpawnBlast(((Component)HeroController.instance).transform.position + Vector3.up * 4f, upgraded);
				blasts.Add(item);
				yield return (object)new WaitForSeconds(0.2f);
				for (int i = 0; i < ((!upgraded) ? 1 : 3); i++)
				{
					blasts.Add(SpawnBlast(((Component)HeroController.instance).transform.position + Vector3.up * (float)Random.Range(4, 10) + Vector3.right * (float)Random.Range(-3, 3), upgraded));
					yield return (object)new WaitForSeconds(0.2f);
				}
			}
			IEnumerator DestroyBlastsCoro()
			{
				yield return (object)new WaitForSeconds(0.5f);
				for (int i = 0; i < blasts.Count; i++)
				{
					GameObject val = blasts[i];
					if ((Object)(object)val != (Object)null)
					{
						Object.Destroy((Object)(object)val);
					}
					yield return (object)new WaitForSeconds(0.2f);
				}
			}
			IEnumerator DisableColliderCoro()
			{
				yield return (object)new WaitForSeconds(0.15f);
				for (int i = 0; i < blasts.Count; i++)
				{
					GameObject val = blasts[i];
					if ((Object)(object)val != (Object)null)
					{
						Object.Destroy((Object)(object)val.GetComponent<CircleCollider2D>());
					}
					yield return (object)new WaitForSeconds(0.2f);
				}
			}
		}

		private GameObject SpawnBlast(Vector3 pos, bool upgraded)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = Object.Instantiate<GameObject>(PaleCourtCharms.preloadedGO["Blast"], pos, Quaternion.identity);
			val.layer = 17;
			val.tag = "Hero Spell";
			val.SetActive(true);
			Object.Destroy((Object)(object)USceneUtil.FindGameObjectInChildren(val, "hero_damager"));
			Animator component = val.GetComponent<Animator>();
			AnimatorStateInfo currentAnimatorStateInfo = component.GetCurrentAnimatorStateInfo(0);
			int fullPathHash = ((AnimatorStateInfo)(ref currentAnimatorStateInfo)).fullPathHash;
			component.PlayInFixedTime(fullPathHash, -1, 0.75f);
			if (_shamanEquippedCached)
			{
				MaybeCallTranscendenceEnlarge(val);
			}
			CircleCollider2D val2 = val.AddComponent<CircleCollider2D>();
			((Collider2D)val2).offset = Vector2.down;
			val2.radius = 3.5f;
			((Collider2D)val2).isTrigger = true;
			float num = Math.Max(Mathf.Abs(val.transform.localScale.x), Mathf.Abs(val.transform.localScale.y));
			if (!float.IsNaN(num) && num > 0f)
			{
				val2.radius *= num;
			}
			DamageEnemies obj = val.AddComponent<DamageEnemies>();
			obj.damageDealt = (PlayerData.instance.equippedCharm_19 ? 45 : 35);
			obj.attackType = (AttackTypes)2;
			obj.ignoreInvuln = false;
			((Behaviour)obj).enabled = true;
			PlayAudio("Burst", 1.2f, 1.5f, 0.5f);
			return val;
		}

		private void PlayAudio(string clip, float minPitch = 1f, float maxPitch = 1f, float volume = 1f, float delay = 0f)
		{
			((MonoBehaviour)GameManager.instance).StartCoroutine(Play());
			IEnumerator Play()
			{
				AudioClip audioClip = PaleCourtCharms.Clips[clip];
				yield return (object)new WaitForSeconds(delay);
				GameObject audioPlayerInstance = ObjectPoolExtensions.Spawn(_audioPlayer, ((Component)this).transform.position, Quaternion.identity);
				AudioSource component = audioPlayerInstance.GetComponent<AudioSource>();
				component.outputAudioMixerGroup = ((Component)HeroController.instance).GetComponent<AudioSource>().outputAudioMixerGroup;
				component.pitch = Random.Range(minPitch, maxPitch);
				component.volume = volume;
				component.PlayOneShot(audioClip);
				yield return (object)new WaitForSeconds(audioClip.length + 3f);
				Object.Destroy((Object)(object)audioPlayerInstance);
			}
		}
	}
	internal class Charms
	{
		public const int GatheringSwarm = 1;

		public const int WaywardCompass = 2;

		public const int Grubsong = 3;

		public const int StalwartShell = 4;

		public const int BaldurShell = 5;

		public const int FuryOfTheFallen = 6;

		public const int QuickFocus = 7;

		public const int LifebloodHeart = 8;

		public const int LifebloodCore = 9;

		public const int DefendersCrest = 10;

		public const int Flukenest = 11;

		public const int ThornsOfAgony = 12;

		public const int MarkOfPride = 13;

		public const int SteadyBody = 14;

		public const int HeavyBlow = 15;

		public const int SharpShadow = 16;

		public const int SporeShroom = 17;

		public const int Longnail = 18;

		public const int ShamanStone = 19;

		public const int SoulCatcher = 20;

		public const int SoulEater = 21;

		public const int GlowingWomb = 22;

		public const int FragileHeart = 23;

		public const int FragileGreed = 24;

		public const int FragileStrength = 25;

		public const int NailmastersGlory = 26;

		public const int JonisBlessing = 27;

		public const int ShapeOfUnn = 28;

		public const int Hiveblood = 29;

		public const int DreamCatcher = 30;

		public const int Dashmaster = 31;

		public const int QuickSlash = 32;

		public const int SpellTwister = 33;

		public const int DeepFocus = 34;

		public const int GrubberflysElegy = 35;

		public const int Voidheart = 36;

		public const int Sprintmaster = 37;

		public const int Dreamshield = 38;

		public const int Weaversong = 39;

		public const int Grimmchild = 40;

		public const int MarkOfPurity = 41;

		public const int VesselsLament = 42;

		public const int BoonOfHallownest = 43;

		public const int AbyssalBloom = 44;

		public const int MarkOfPurityBackboard = 1;

		public const int VesselsLamentBackboard = 12;

		public const int BoonOfHallownestBackboard = 23;

		public const int AbyssalBloomBackboard = 34;

		public const int charmCost_41 = 3;

		public const int charmCost_42 = 1;

		public const int charmCost_43 = 4;

		public const int charmCost_44 = 5;
	}
	public class CheckBloomStage : MonoBehaviour
	{
		private AbyssalBloomBehaviour _bloom;

		private HeroController _hc => HeroController.instance;

		private void Awake()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Expected O, but got Unknown
			_bloom = ((Component)_hc).GetComponent<AbyssalBloomBehaviour>();
			ModHooks.CharmUpdateHook += new CharmUpdateHandler(CharmUpdateHook);
			HeroController.TakeDamage += new hook_TakeDamage(HeroControllerTakeDamage);
			HeroController.AddHealth += new hook_AddHealth(HeroControllerAddHealth);
			HeroController.MaxHealth += new hook_MaxHealth(HeroControllerMaxHealth);
		}

		private void CharmUpdateHook(PlayerData data, HeroController hc)
		{
			CheckStage();
		}

		private void HeroControllerTakeDamage(orig_TakeDamage orig, HeroController self, GameObject go, CollisionSide damageSide, int damageAmount, int hazardType)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			int health = PlayerData.instance.health;
			orig.Invoke(self, go, damageSide, damageAmount, hazardType);
			if (PlayerData.instance.health != health)
			{
				CheckStage();
			}
		}

		private void HeroControllerAddHealth(orig_AddHealth orig, HeroController self, int amount)
		{
			orig.Invoke(self, amount);
			CheckStage();
		}

		private void HeroControllerMaxHealth(orig_MaxHealth orig, HeroController self)
		{
			orig.Invoke(self);
			CheckStage();
		}

		private void CheckStage()
		{
			if (!PaleCourtCharms.Settings.equippedCharms[3])
			{
				_bloom.SetLevel(0);
			}
			else if (_hc.playerData.health > (_hc.playerData.maxHealth + 1) / 2)
			{
				_bloom.SetLevel(0);
			}
			else if (_hc.playerData.health > 1 && _hc.playerData.health <= (_hc.playerData.maxHealth + 1) / 2)
			{
				Logger.LogFine("On level 1");
				_bloom.SetLevel(1);
			}
			else if (_hc.playerData.health <= 1)
			{
				Logger.LogFine("On level 2");
				_bloom.SetLevel(2);
			}
		}

		private void OnDestroy()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			ModHooks.CharmUpdateHook -= new CharmUpdateHandler(CharmUpdateHook);
			HeroController.TakeDamage -= new hook_TakeDamage(HeroControllerTakeDamage);
			HeroController.AddHealth -= new hook_AddHealth(HeroControllerAddHealth);
			HeroController.MaxHealth -= new hook_MaxHealth(HeroControllerMaxHealth);
		}

		private void Log(object o)
		{
			Logger.Log("[PC_Charms][CheckBloomStage] " + o);
		}
	}
	public class Dagger : MonoBehaviour
	{
		private const int DaggerDamage = 13;

		private const int DaggerDamageUpgraded = 25;

		public bool upgraded;

		public bool shamanEquipped;

		public bool snailEquipped;

		private readonly Dictionary<GameObject, Coroutine> _activeTargets = new Dictionary<GameObject, Coroutine>();

		private bool _toggleSource;

		private void OnDestroy()
		{
			foreach (KeyValuePair<GameObject, Coroutine> item in new List<KeyValuePair<GameObject, Coroutine>>(_activeTargets))
			{
				Coroutine value = item.Value;
				SafeStopCoroutine(value);
			}
			_activeTargets.Clear();
		}

		private void OnTriggerEnter2D(Collider2D collider)
		{
			int layer = ((Component)collider).gameObject.layer;
			if (layer == 20 || layer == 9 || layer == 26 || layer == 31 || ((Component)collider).CompareTag("Geo"))
			{
				return;
			}
			GameObject gameObject = ((Component)collider).gameObject;
			if ((Object)(object)gameObject == (Object)null)
			{
				return;
			}
			if (shamanEquipped || snailEquipped)
			{
				if (_activeTargets.ContainsKey(gameObject))
				{
					return;
				}
				IEnumerator enumerator = MultiHitTargetCoroutine(gameObject);
				if (enumerator != null)
				{
					Coroutine val = SafeStartCoroutine(enumerator);
					if (val != null)
					{
						_activeTargets[gameObject] = val;
					}
				}
			}
			else
			{
				DoHit(gameObject);
			}
		}

		private void OnTriggerExit2D(Collider2D collider)
		{
			GameObject gameObject = ((Component)collider).gameObject;
			if (!((Object)(object)gameObject == (Object)null) && _activeTargets.TryGetValue(gameObject, out var value))
			{
				SafeStopCoroutine(value);
				_activeTargets.Remove(gameObject);
			}
		}

		private Coroutine SafeStartCoroutine(IEnumerator routine)
		{
			if (routine == null)
			{
				return null;
			}
			if (!Application.isPlaying)
			{
				return null;
			}
			try
			{
				if ((Object)(object)GameManager.instance != (Object)null)
				{
					GameManager instance = GameManager.instance;
					if ((Object)(object)instance != (Object)null && (Object)(object)((Component)instance).gameObject != (Object)null)
					{
						try
						{
							return ((MonoBehaviour)instance).StartCoroutine(routine);
						}
						catch
						{
						}
					}
				}
				if ((Object)(object)this != (Object)null && (Object)(object)((Component)this).gameObject != (Object)null && ((Behaviour)this).enabled && ((Component)this).gameObject.activeInHierarchy)
				{
					try
					{
						return ((MonoBehaviour)this).StartCoroutine(routine);
					}
					catch
					{
						return null;
					}
				}
			}
			catch
			{
			}
			return null;
		}

		private void SafeStopCoroutine(Coroutine c)
		{
			if (c == null || !Application.isPlaying)
			{
				return;
			}
			try
			{
				if ((Object)(object)GameManager.instance != (Object)null)
				{
					try
					{
						((MonoBehaviour)GameManager.instance).StopCoroutine(c);
						return;
					}
					catch
					{
					}
				}
				if ((Object)(object)this != (Object)null)
				{
					try
					{
						((MonoBehaviour)this).StopCoroutine(c);
						return;
					}
					catch
					{
						return;
					}
				}
			}
			catch
			{
			}
		}

		private float GetMultiHitInterval()
		{
			if (shamanEquipped && snailEquipped)
			{
				return 0.22f;
			}
			if (shamanEquipped)
			{
				return 0.2f;
			}
			if (snailEquipped)
			{
				return 0.24f;
			}
			return 0.22f;
		}

		private IEnumerator MultiHitTargetCoroutine(GameObject target)
		{
			if ((Object)(object)target == (Object)null)
			{
				yield break;
			}
			DoHit(target);
			float multiHitInterval = GetMultiHitInterval();
			try
			{
				while ((Object)(object)target != (Object)null && !((Object)(object)this == (Object)null))
				{
					yield return (object)new WaitForSeconds(multiHitInterval);
					if ((Object)(object)target == (Object)null || (Object)(object)this == (Object)null)
					{
						break;
					}
					DoHit(target);
					multiHitInterval = GetMultiHitInterval();
				}
			}
			finally
			{
				Dagger dagger = this;
				try
				{
					foreach (GameObject item in new List<GameObject>(dagger._activeTargets.Keys))
					{
						if ((Object)(object)item == (Object)(object)target)
						{
							dagger._activeTargets.Remove(item);
							break;
						}
					}
				}
				catch
				{
				}
			}
		}

		private void DoHit(GameObject target)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)target == (Object)null)
			{
				return;
			}
			HitInstance val = new HitInstance
			{
				DamageDealt = (upgraded ? 25 : 13),
				AttackType = (AttackTypes)2,
				IgnoreInvulnerable = true,
				Source = (_toggleSource ? ((Component)this).gameObject : null)
			};
			_toggleSource = !_toggleSource;
			val.Multiplier = 1f;
			try
			{
				HitTaker.Hit(target, val, 3);
			}
			catch
			{
			}
		}
	}
	internal static class EmbeddedSprites
	{
		private static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

		public static Sprite Get(string key, float ppu = 100f)
		{
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Expected O, but got Unknown
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			if (cache.TryGetValue(key, out var value))
			{
				return value;
			}
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			using Stream stream = executingAssembly.GetManifestResourceStream(executingAssembly.GetName().Name + ".assets." + key + ".png");
			if (stream == null)
			{
				throw new FileNotFoundException("Sprite resource not found: " + key + ".png");
			}
			byte[] array = new byte[stream.Length];
			stream.Read(array, 0, array.Length);
			Texture2D val = new Texture2D(1, 1, (TextureFormat)4, false);
			ImageConversion.LoadImage(val, array, true);
			((Texture)val).filterMode = (FilterMode)1;
			value = Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), new Vector2(0.5f, 0.5f), ppu);
			cache[key] = value;
			return value;
		}
	}
	internal static class FloristInterop
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static CharmUpdateHandler <0>__OnCharmUpdate;
		}

		private static bool _initialized = false;

		private static bool _available = false;

		private static object _instance = null;

		private static int _num = -1;

		private static bool _cachedEquipped = false;

		public static void Init()
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			if (!_initialized)
			{
				_initialized = true;
				TryFindFlorist();
				UpdateCache();
				object obj = <>O.<0>__OnCharmUpdate;
				if (obj == null)
				{
					CharmUpdateHandler val = OnCharmUpdate;
					<>O.<0>__OnCharmUpdate = val;
					obj = (object)val;
				}
				ModHooks.CharmUpdateHook += (CharmUpdateHandler)obj;
			}
		}

		private static void TryFindFlorist()
		{
			try
			{
				IMod mod = ModHooks.GetMod("Transcendence", false, false);
				Assembly assembly = null;
				Mod val = (Mod)(object)((mod is Mod) ? mod : null);
				assembly = ((val == null) ? AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(delegate(Assembly a)
				{
					try
					{
						return a.GetType("Transcendence.FloristsBlessing") != null;
					}
					catch
					{
						return false;
					}
				}) : ((object)val).GetType().Assembly);
				if (assembly == null)
				{
					_available = false;
					return;
				}
				Type type = assembly.GetType("Transcendence.FloristsBlessing");
				if (type == null)
				{
					_available = false;
					return;
				}
				PropertyInfo property = type.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
				if (property != null)
				{
					_instance = property.GetValue(null);
				}
				else
				{
					FieldInfo field = type.GetField("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
					if (field != null)
					{
						_instance = field.GetValue(null);
					}
				}
				if (_instance == null)
				{
					_available = false;
					return;
				}
				PropertyInfo property2 = _instance.GetType().GetProperty("Num", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
				if (property2 != null)
				{
					_num = (int)property2.GetValue(_instance);
					_available = true;
					return;
				}
				FieldInfo field2 = _instance.GetType().GetField("Num", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
				if (field2 != null)
				{
					_num = (int)field2.GetValue(_instance);
					_available = true;
				}
			}
			catch (Exception ex)
			{
				Logger.Log("[PC_Charms][FloristInterop] init failed: " + ex.Message);
				_available = false;
			}
		}

		private static void OnCharmUpdate(PlayerData pd, HeroController hc)
		{
			UpdateCache();
		}

		private static void UpdateCache()
		{
			if (!_available || _num <= 0 || PlayerData.instance == null)
			{
				_cachedEquipped = false;
				return;
			}
			try
			{
				_cachedEquipped = PlayerData.instance.GetBool($"equippedCharm_{_num}");
			}
			catch
			{
				_cachedEquipped = false;
			}
		}

		public static bool IsEquipped()
		{
			if (!_initialized)
			{
				Init();
			}
			return _cachedEquipped;
		}
	}
	public class HonourUpgradeItem : AbstractItem
	{
		private const string HonourKey = "Kings_Honour";

		public HonourUpgradeItem()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Expected O, but got Unknown
			((AbstractItem)this)..ctor();
			base.name = "Kings_Honour";
			base.UIDef = (UIDef)new MsgUIDef
			{
				name = (IString)new LanguageString("UI", "CHARM_NAME_HONOUR"),
				shopDesc = (IString)new LanguageString("RANDO", "SHOP_DESCRIPTION_HONOUR"),
				sprite = (ISprite)(object)new ICShiny.EmbeddedSprite
				{
					key = "Kings_Honour"
				}
			};
		}

		protected override void OnLoad()
		{
			Events.OnStringGet += AddNotchCostToCharmName;
		}

		protected override void OnUnload()
		{
			Events.OnStringGet -= AddNotchCostToCharmName;
		}

		private void AddNotchCostToCharmName(StringGetArgs args)
		{
			IString source = args.Source;
			LanguageString val = (LanguageString)(object)((source is LanguageString) ? source : null);
			if (val != null && val.key == "CHARM_NAME_HONOUR")
			{
				args.Current = args.Current.Replace("-9999", $"{PlayerData.instance.charmCost_10}");
			}
		}

		public override void GiveImmediate(GiveInfo info)
		{
			PaleCourtCharms.Settings.upgradedCharm_10 = true;
			PlayerData.instance.SetBool("upgradedCharm_10", true);
			GameManager.instance.SaveGame();
		}

		public override bool Redundant()
		{
			if (PlayerData.instance.GetBool("gotCharm_10"))
			{
				return PaleCourtCharms.Settings.upgradedCharm_10;
			}
			return false;
		}
	}
	internal static class ICShiny
	{
		public class EmbeddedSprite : ISprite
		{
			public string key;

			[JsonIgnore]
			public Sprite Value => EmbeddedSprites.Get(key);

			public ISprite Clone()
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Expected O, but got Unknown
				return (ISprite)MemberwiseClone();
			}
		}

		[Serializable]
		[CompilerGenerated]
		private sealed class <>c
		{
			public static readonly <>c <>9 = new <>c();

			public static hook_ContinueGame <>9__6_0;

			public static hook_StartNewGame <>9__6_1;

			internal void <Hook>b__6_0(orig_ContinueGame orig, UIManager self)
			{
				ItemChangerMod.CreateSettingsProfile(false, false);
				orig.Invoke(self);
				TryPlaceCharms();
			}

			internal void <Hook>b__6_1(orig_StartNewGame orig, UIManager self, bool perma, bool bossRush)
			{
				ItemChangerMod.CreateSettingsProfile(false, false);
				orig.Invoke(self, perma, bossRush);
				TryPlaceCharms();
				placementsDone = false;
			}
		}

		public static bool placementsDone;

		public static readonly string[] CharmScenes;

		public static readonly Vector2[] CharmPositions;

		public const string HonourName = "Kings_Honour";

		public static readonly string HonourScene;

		public static readonly Vector2 HonourPos;

		public static void Hook()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Expected O, but got Unknown
			object obj = <>c.<>9__6_0;
			if (obj == null)
			{
				hook_ContinueGame val = delegate(orig_ContinueGame orig, UIManager self)
				{
					ItemChangerMod.CreateSettingsProfile(false, false);
					orig.Invoke(self);
					TryPlaceCharms();
				};
				<>c.<>9__6_0 = val;
				obj = (object)val;
			}
			UIManager.ContinueGame += (hook_ContinueGame)obj;
			object obj2 = <>c.<>9__6_1;
			if (obj2 == null)
			{
				hook_StartNewGame val2 = delegate(orig_StartNewGame orig, UIManager self, bool perma, bool bossRush)
				{
					ItemChangerMod.CreateSettingsProfile(false, false);
					orig.Invoke(self, perma, bossRush);
					TryPlaceCharms();
					placementsDone = false;
				};
				<>c.<>9__6_1 = val2;
				obj2 = (object)val2;
			}
			UIManager.StartNewGame += (hook_StartNewGame)obj2;
		}

		public static void ResetPlacementsFlag()
		{
			placementsDone = false;
		}

		private static bool GetIsRando()
		{
			return RandomizerMod.RS?.GenerationSettings != null;
		}

		public static void TryPlaceCharms()
		{
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Expected O, but got Unknown
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Expected O, but got Unknown
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Expected O, but got Unknown
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Expected O, but got Unknown
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Expected O, but got Unknown
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Expected O, but got Unknown
			if (placementsDone || PaleCourtCharms.CharmIDs.Count == 0)
			{
				return;
			}
			bool num = ModHooks.GetMod("Randomizer 4", false, false) is Mod;
			bool flag = false;
			if (num)
			{
				flag = GetIsRando();
			}
			bool addCharms = PaleCourtCharms.GlobalSettings.AddCharms;
			if (flag & addCharms)
			{
				return;
			}
			List<AbstractPlacement> list = new List<AbstractPlacement>();
			for (int i = 0; i < PaleCourtCharms.CharmIDs.Count && i < 4; i++)
			{
				int num2 = PaleCourtCharms.CharmIDs[i];
				string text = PaleCourtCharms.CharmKeys[i];
				if (Finder.GetItemInternal(text) == null)
				{
					CharmItem val = new CharmItem
					{
						charmNum = num2,
						name = text,
						UIDef = (UIDef)new MsgUIDef
						{
							name = (IString)new LanguageString("UI", $"CHARM_NAME_{num2}"),
							shopDesc = (IString)new LanguageString("RANDO", "SHOP_DESCRIPTION_" + PaleCourtCharms.Charms[i].InternalName),
							sprite = (ISprite)(object)new EmbeddedSprite
							{
								key = text
							}
						}
					};
					InteropTag obj = ((TaggableObject)val).AddTag<InteropTag>();
					obj.Message = "RandoSupplementalMetadata";
					obj.Properties["ModSource"] = ((Mod)PaleCourtCharms.Instance).GetName();
					obj.Properties["PoolGroup"] = "Charms";
					Finder.DefineCustomItem((AbstractItem)val);
				}
				if (Finder.GetLocationInternal(text) == null)
				{
					Finder.DefineCustomLocation((AbstractLocation)new CoordinateLocation
					{
						name = text,
						sceneName = CharmScenes[i],
						x = CharmPositions[i].x,
						y = CharmPositions[i].y,
						elevation = 0f
					});
				}
				list.Add(Finder.GetLocationInternal(text).Wrap().Add(Finder.GetItemInternal(text)));
			}
			if (Finder.GetItemInternal("Kings_Honour") == null)
			{
				Finder.DefineCustomItem((AbstractItem)(object)new HonourUpgradeItem());
			}
			if (Finder.GetLocationInternal("Kings_Honour") == null)
			{
				Finder.DefineCustomLocation((AbstractLocation)new CoordinateLocation
				{
					name = "Kings_Honour",
					sceneName = HonourScene,
					x = HonourPos.x,
					y = HonourPos.y,
					elevation = 0f
				});
			}
			list.Add(Finder.GetLocationInternal("Kings_Honour").Wrap().Add(Finder.GetItemInternal("Kings_Honour")));
			ItemChangerMod.AddPlacements((IEnumerable<AbstractPlacement>)list, (PlacementConflictResolution)3);
			placementsDone = true;
		}

		static ICShiny()
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			placementsDone = false;
			CharmScenes = new string[4] { "Fungus3_48", "Room_Mansion", "Fungus2_21", "Abyss_09" };
			CharmPositions = (Vector2[])(object)new Vector2[4]
			{
				new Vector2(49.1f, 94.4f),
				new Vector2(27f, 6.4f),
				new Vector2(114.8f, 12.4f),
				new Vector2(210.2f, 50.4f)
			};
			HonourScene = "Waterways_13";
			HonourPos = new Vector2(89.1f, 18.4f);
		}
	}
	internal class LamentControl : MonoBehaviour
	{
		public static List<GameObject> markedEnemies = new List<GameObject>();

		private PlayMakerFSM _spellControl;

		private PlayerData _pd = PlayerData.instance;

		private HeroController _hc = HeroController.instance;

		private PlayMakerFSM _pvControl;

		private void OnEnable()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			DamageEnemies.FixedUpdate += new hook_FixedUpdate(SuppressDamageEnemiesError);
			HealthManager.TakeDamage += new hook_TakeDamage(ApplyStatus);
			SceneManager.activeSceneChanged += ClearList;
			_pvControl = PaleCourtCharms.PVControl;
			_spellControl = HeroController.instance.spellControl;
			if ((Object)(object)_spellControl != (Object)null)
			{
				_spellControl.InsertMethod("Cancel All", 33, BlastControlCancel);
				_spellControl.InsertMethod("Focus Cancel", 15, BlastControlCancel);
				_spellControl.InsertMethod("Focus Cancel 2", 18, BlastControlCancel);
				_spellControl.InsertMethod("Focus Blast", 15, BlastControlFadeIn);
				_spellControl.InsertMethod("Start MP Drain Blast", 2, BlastControlFadeIn);
				_spellControl.InsertMethod("Focus Heal Blast", 16, BlastControlMain);
				_spellControl.InsertMethod("Focus Heal 2 Blast", 18, BlastControlMain);
			}
			StartupPool[] startupPools = ObjectPool.instance.startupPools;
			foreach (StartupPool val in startupPools)
			{
				if (((Object)val.prefab).name == "Knight Spore Cloud")
				{
					PaleCourtCharms.preloadedGO["SporeCloud"] = val.prefab;
				}
				if (((Object)val.prefab).name == "Knight Dung Cloud")
				{
					PaleCourtCharms.preloadedGO["DungCloud"] = val.prefab;
				}
			}
			FloristInterop.Init();
		}

		private void BlastControlCancel()
		{
			PureVesselBlastCancel();
		}

		private void PureVesselBlastCancel()
		{
			foreach (GameObject markedEnemy in markedEnemies)
			{
				int index = markedEnemies.IndexOf(markedEnemy);
				try
				{
					if ((Object)(object)markedEnemy.GetComponent<Afflicted>()._focusLines != (Object)null)
					{
						markedEnemy.GetComponent<Afflicted>()._focusLines.GetComponent<tk2dSpriteAnimator>().Stop();
						Object.Destroy((Object)(object)markedEnemy.GetComponent<Afflicted>()._focusLines);
					}
				}
				catch (ArgumentOutOfRangeException)
				{
				}
				try
				{
					if ((Object)(object)markedEnemies[index] != (Object)null)
					{
						if ((Object)(object)markedEnemies[index].GetComponent<Afflicted>() == (Object)null)
						{
							Log("Afficted component not found");
						}
						else
						{
							try
							{
								markedEnemy.GetComponent<Afflicted>().visible = true;
								markedEnemy.GetComponent<Afflicted>().SoulEffect.SetActive(true);
								markedEnemy.GetComponent<Afflicted>().SoulEffect.GetComponent<ParticleSystem>().Play();
							}
							catch (NullReferenceException)
							{
							}
						}
					}
				}
				catch (ArgumentOutOfRangeException)
				{
					Log("Exception caught in soul effect");
				}
				try
				{
					((MonoBehaviour)markedEnemy.GetComponent<Afflicted>()).StopCoroutine(markedEnemy.GetComponent<Afflicted>()._createLine);
				}
				catch (NullReferenceException)
				{
					Log("Couldn't stop create line couroutine");
				}
				try
				{
					Object.Destroy((Object)(object)markedEnemy.GetComponent<Afflicted>()._blast);
				}
				catch (ArgumentOutOfRangeException)
				{
					Log("Exception caught in blast");
				}
			}
		}

		private void BlastControlFadeIn()
		{
			for (int i = 0; i < markedEnemies.Count; i++)
			{
				GameObject val = markedEnemies[i];
				for (int j = 0; j < markedEnemies.Count; j++)
				{
					GameObject val2 = markedEnemies[j];
					if (i != j && !((Object)(object)val2 != (Object)(object)val))
					{
						Log("Removed Duplicate Object");
						Log($"{val2} was in the list twice");
						markedEnemies.Remove(val2);
					}
				}
				if ((Object)(object)val == (Object)null || !val.active)
				{
					markedEnemies.RemoveAt(i);
					i--;
				}
				else
				{
					((MonoBehaviour)val.GetComponent<Afflicted>()).StartCoroutine(val.GetComponent<Afflicted>().PureVesselBlastFadeIn());
				}
			}
		}

		private void BlastControlMain()
		{
			List<int> list = new List<int>();
			foreach (GameObject markedEnemy in markedEnemies)
			{
				int item = markedEnemies.IndexOf(markedEnemy);
				if ((Object)(object)markedEnemy == (Object)null || !markedEnemy.active)
				{
					list.Add(item);
					Logger.LogFine("Item was null, continuing");
				}
				else
				{
					((MonoBehaviour)this).StartCoroutine(markedEnemy.GetComponent<Afflicted>().PureVesselBlast());
				}
			}
			foreach (int item2 in list)
			{
				markedEnemies.RemoveAt(item2);
			}
		}

		private void SuppressDamageEnemiesError(orig_FixedUpdate orig, DamageEnemies self)
		{
			try
			{
				orig.Invoke(self);
			}
			catch (ArgumentOutOfRangeException)
			{
			}
		}

		private void OnDisable()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			DamageEnemies.FixedUpdate -= new hook_FixedUpdate(SuppressDamageEnemiesError);
			HealthManager.TakeDamage -= new hook_TakeDamage(ApplyStatus);
			SceneManager.activeSceneChanged -= ClearList;
			if ((Object)(object)_spellControl != (Object)null)
			{
				FsmUtil.RemoveAction(_spellControl, "Cancel All", 33);
				FsmUtil.RemoveAction(_spellControl, "Focus Cancel", 15);
				FsmUtil.RemoveAction(_spellControl, "Focus Cancel 2", 18);
				FsmUtil.RemoveAction(_spellControl, "Focus Blast", 15);
				FsmUtil.RemoveAction(_spellControl, "Start MP Drain Blast", 2);
				FsmUtil.RemoveAction(_spellControl, "Focus Heal Blast", 16);
				FsmUtil.RemoveAction(_spellControl, "Focus Heal 2 Blast", 18);
			}
		}

		private void ClearList(Scene PrevScene, Scene NextScene)
		{
			markedEnemies.Clear();
			foreach (GameObject markedEnemy in markedEnemies)
			{
				Log($"This should be empty but {markedEnemy} is still there");
			}
		}

		private void ApplyStatus(orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self, hitInstance);
			if ((int)hitInstance.AttackType == 0 && (Object)(object)((Component)self).gameObject.GetComponent<Afflicted>() == (Object)null)
			{
				((Component)self).gameObject.AddComponent<Afflicted>();
				markedEnemies.Add(((Component)self).gameObject);
			}
		}

		private void Log(object message)
		{
			Logger.Log("[PC_Charms][LamentControl] " + message);
		}
	}
	internal class Afflicted : MonoBehaviour
	{
		public GameObject SoulEffect;

		public GameObject _focusLines;

		public GameObject _line;

		public GameObject _blast;

		public IEnumerator _createLine;

		private PlayMakerFSM _pvControl;

		private HeroController _hc = HeroController.instance;

		private PlayerData _pd = PlayerData.instance;

		public bool visible;

		private void Start()
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			_pvControl = PaleCourtCharms.PVControl;
			SoulEffect = Object.Instantiate<GameObject>(PaleCourtCharms.preloadedGO["SoulEffect"], ((Component)this).gameObject.transform);
			SoulEffect.transform.localPosition = new Vector3(0f, 0f, -0.0001f);
			Vector2 val = Vector2.op_Implicit(((Component)this).gameObject.transform.position);
			Bounds bounds;
			if ((Object)(object)((Component)this).gameObject.GetComponent<SpriteRenderer>() != (Object)null)
			{
				bounds = ((Renderer)((Component)this).gameObject.GetComponent<SpriteRenderer>()).bounds;
				val = Vector2.op_Implicit(((Bounds)(ref bounds)).center);
			}
			if ((Object)(object)((Component)this).gameObject.GetComponent<tk2dSprite>() != (Object)null)
			{
				bounds = ((tk2dBaseSprite)((Component)this).gameObject.gameObject.GetComponent<tk2dSprite>()).GetBounds();
				val = Vector2.op_Implicit(((Bounds)(ref bounds)).center + ((Component)this).gameObject.transform.position);
			}
			SoulEffect.transform.position = Vector2.op_Implicit(val);
			SoulEffect.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
			SoulEffect.GetComponent<ParticleSystem>().startSize = 0.7f;
			SoulEffect.GetComponent<ParticleSystem>().startLifetime = 0.3f;
			SoulEffect.GetComponent<ParticleSystem>().startColor = new Color(1f, 1f, 1f, 0.5f);
			SoulEffect.SetActive(true);
		}

		private void Update()
		{
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Expected O, but got Unknown
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)SoulEffect != (Object)null)
			{
				Vector3 val = ((Component)this).gameObject.transform.position;
				Bounds bounds;
				if ((Object)(object)((Component)this).gameObject.GetComponent<SpriteRenderer>() != (Object)null)
				{
					bounds = ((Renderer)((Component)this).gameObject.GetComponent<SpriteRenderer>()).bounds;
					val = ((Bounds)(ref bounds)).center;
				}
				if ((Object)(object)((Component)this).gameObject.GetComponent<tk2dSprite>() != (Object)null)
				{
					bounds = ((tk2dBaseSprite)((Component)this).gameObject.GetComponent<tk2dSprite>()).GetBounds();
					val = ((Bounds)(ref bounds)).center + ((Component)this).gameObject.transform.position;
				}
				SoulEffect.transform.position = new Vector3(val.x, val.y, ((Component)this).gameObject.transform.position.z + -0.0001f);
				if (visible)
				{
					if ((Object)(object)((Component)this).gameObject.GetComponent<SpriteRenderer>() != (Object)null)
					{
						SoulEffect.SetActive(((Renderer)((Component)this).gameObject.GetComponent<SpriteRenderer>()).isVisible);
					}
					if ((Object)(object)((Component)this).gameObject.GetComponent<MeshRenderer>() != (Object)null)
					{
						SoulEffect.SetActive(((Renderer)((Component)this).gameObject.GetComponent<MeshRenderer>()).isVisible);
					}
				}
			}
			else
			{
				SoulEffect = new GameObject();
				Start();
			}
			if ((Object)(object)_focusLines != (Object)null && _pd.GetBool("equippedCharm_" + 28))
			{
				_focusLines.transform.position = ((Component)this).gameObject.transform.position;
			}
			if ((Object)(object)_blast != (Object)null && _pd.GetBool("equippedCharm_" + 28))
			{
				_blast.transform.position = ((Component)this).gameObject.transform.position;
			}
		}

		public IEnumerator FadeOut()
		{
			SoulEffect.GetComponent<ParticleSystem>().Stop();
			yield return (object)new WaitUntil((Func<bool>)(() => SoulEffect.GetComponent<ParticleSystem>().isStopped));
			visible = false;
			SoulEffect.SetActive(false);
		}

		private void OnDisable()
		{
			Object.Destroy((Object)(object)SoulEffect);
		}

		public IEnumerator PureVesselBlastFadeIn()
		{
			((MonoBehaviour)this).StartCoroutine(FadeOut());
			_createLine = CreateLine();
			((MonoBehaviour)this).StartCoroutine(_createLine);
			_focusLines = Object.Instantiate<GameObject>(USceneUtil.Find(USceneUtil.Find(((Component)_hc).gameObject, "Focus Effects"), "Lines Anim"), ((Component)this).gameObject.transform.position, Quaternion.identity);
			_focusLines.GetComponent<tk2dSpriteAnimator>().Play("Focus Effect");
			if ((Object)(object)_pvControl != (Object)null)
			{
				((MonoBehaviour)(object)this).PlayAudio((AudioClip)_pvControl.GetAction<AudioPlayerOneShotSingle>("Focus Charge", 2).audioClip.Value, 0f, 1.5f);
			}
			_blast = Object.Instantiate<GameObject>(PaleCourtCharms.preloadedGO["Blast"]);
			Transform transform = _blast.transform;
			transform.position += ((Component)this).gameObject.transform.position;
			_blast.SetActive(true);
			Object.Destroy((Object)(object)USceneUtil.FindGameObjectInChildren(_blast, "hero_damager"));
			if (_pd.GetBool("equippedCharm_" + 34))
			{
				Transform transform2 = _blast.transform;
				transform2.localScale *= 2.5f;
			}
			else
			{
				Transform transform3 = _blast.transform;
				transform3.localScale *= 1.5f;
			}
			Animator component = _blast.GetComponent<Animator>();
			component.speed = 1f;
			if (_pd.GetBool("equippedCharm_" + 7))
			{
				component.speed *= 1.33f;
			}
			if (_pd.GetBool("equippedCharm_" + 34))
			{
				component.speed -= component.speed * 0.35f;
			}
			yield return null;
		}

		public IEnumerator CreateLine()
		{
			float num = 1f;
			if (_pd.GetBool("equippedCharm_" + 7))
			{
				num /= 1.33f;
			}
			if (_pd.GetBool("equippedCharm_" + 34))
			{
				num -= num * 0.35f;
			}
			yield return (object)new WaitForSeconds(num - 0.2f);
			Vector3 val = ((Component)_hc).transform.position - new Vector3(0f, 1f, 0f);
			Vector3 position = ((Component)this).gameObject.transform.position;
			Vector3 val2 = Vector3.Lerp(val, position, 0.5f);
			float num2 = val.y - position.y;
			float num3 = val.x - position.x;
			float num4;
			for (num4 = Mathf.Atan2(num2, num3) * (180f / (float)Math.PI); num4 < 0f; num4 += 360f)
			{
			}
			float num5 = Vector2.Distance(Vector2.op_Implicit(val), Vector2.op_Implicit(position));
			_line = Object.Instantiate<GameObject>(FSMUtility.LocateMyFSM(PaleCourtCharms.preloadedGO["SoulTwister"], "Mage").GetAction<CreateObject>("Tele Line").gameObject.Value, val2, new Quaternion(0f, 0f, 0f, 0f));
			Extensions.SetRotationZ(_line.transform, num4);
			_line.transform.localScale = new Vector3(num5, 1f, 1f);
			_line.GetComponent<ParticleSystem>().startSize = 0.35f;
			_line.GetComponent<ParticleSystem>().emissionRate = 3000f;
			_line.GetComponent<ParticleSystem>().startLifetime = 0.75f;
			_line.GetComponent<ParticleSystem>().Emit(0);
			_line.SetActive(true);
			_line.GetComponent<ParticleSystem>().Play();
			_line.GetComponent<ParticleSystem>().loop = false;
		}

		public IEnumerator PureVesselBlast()
		{
			_focusLines.GetComponent<tk2dSpriteAnimator>().Play("Focus Effect End");
			_blast.layer = 17;
			Animator component = _blast.GetComponent<Animator>();
			component.speed = 1f;
			AnimatorStateInfo currentAnimatorStateInfo = component.GetCurrentAnimatorStateInfo(0);
			int fullPathHash = ((AnimatorStateInfo)(ref currentAnimatorStateInfo)).fullPathHash;
			component.PlayInFixedTime(fullPathHash, -1, 0.8f);
			CircleCollider2D blastCollider = _blast.AddComponent<CircleCollider2D>();
			blastCollider.radius = 2.5f;
			if (_pd.GetBool("equippedCharm_" + 34))
			{
				blastCollider.radius *= 2.5f;
			}
			else
			{
				blastCollider.radius *= 1.5f;
			}
			((Collider2D)blastCollider).offset = Vector2.op_Implicit(Vector3.down);
			((Collider2D)blastCollider).isTrigger = true;
			_blast.AddComponent<DamageEnemies>();
			int num = 30;
			float num2 = 1f;
			if (_pd.GetBool("equippedCharm_" + 34))
			{
				num2 *= 2f;
			}
			if (FloristInterop.IsEquipped())
			{
				num2 *= 2f;
			}
			int damageDealt = Mathf.Max(1, Mathf.RoundToInt((float)num * num2));
			DamageEnemies[] componentsInChildren = _blast.GetComponentsInChildren<DamageEnemies>(true);
			if (componentsInChildren == null || componentsInChildren.Length == 0)
			{
				DamageEnemies val = _blast.GetComponent<DamageEnemies>();
				if ((Object)(object)val == (Object)null)
				{
					val = _blast.AddComponent<DamageEnemies>();
				}
				val.damageDealt = damageDealt;
				val.attackType = (AttackTypes)2;
				val.ignoreInvuln = false;
				((Behaviour)val).enabled = true;
			}
			else
			{
				DamageEnemies[] array = componentsInChildren;
				foreach (DamageEnemies obj in array)
				{
					obj.damageDealt = damageDealt;
					obj.attackType = (AttackTypes)2;
					obj.ignoreInvuln = false;
					((Behaviour)obj).enabled = true;
				}
			}
			((MonoBehaviour)(object)this).PlayAudio((AudioClip)_pvControl.GetAction<AudioPlayerOneShotSingle>("Focus Burst", 8).audioClip.Value, 0f, 1.5f);
			if (_pd.GetBool("equippedCharm_" + 17))
			{
				if (_pd.GetBool("equippedCharm_" + 10))
				{
					if (PaleCourtCharms.Settings.upgradedCharm_10)
					{
						Object.Instantiate<GameObject>(((Component)_hc).GetComponent<RoyalAura>().dungCloud, ((Component)this).transform.position, Quaternion.identity).SetActive(true);
					}
					else
					{
						Object.Instantiate<GameObject>(PaleCourtCharms.preloadedGO["DungCloud"], ((Component)this).transform.position, Quaternion.identity).SetActive(true);
					}
				}
				else
				{
					Object.Instantiate<GameObject>(PaleCourtCharms.preloadedGO["SporeCloud"], ((Component)this).transform.position, Quaternion.identity).SetActive(true);
				}
			}
			yield return (object)new WaitForSeconds(0.11f);
			((Behaviour)blastCollider).enabled = false;
			yield return (object)new WaitForSeconds(0.69f);
			Object.Destroy((Object)(object)_blast);
			Object.Destroy((Object)(object)_focusLines);
			try
			{
				LamentControl.markedEnemies.RemoveAt(LamentControl.markedEnemies.IndexOf(((Component)this).gameObject));
				Object.Destroy((Object)(object)((Component)this).gameObject.GetComponent<Afflicted>());
			}
			catch (NullReferenceException)
			{
			}
		}
	}
	public class ModifyAuraColor : MonoBehaviour
	{
		private readonly Color DefaultColor;

		private readonly Color PaleColor;

		private ParticleSystem ps;

		private void Update()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)ps == (Object)null)
			{
				ps = ((Component)this).gameObject.GetComponent<ParticleSystem>();
			}
			MainModule main = ps.main;
			((MainModule)(ref main)).startColor = MinMaxGradient.op_Implicit(PaleCourtCharms.Settings.upgradedCharm_10 ? PaleColor : DefaultColor);
		}

		public ModifyAuraColor()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			DefaultColor = new Color(0.6706f, 0.4275f, 0f, 1f);
			PaleColor = new Color(0.8f, 0.8f, 0.8f);
			((MonoBehaviour)this)..ctor();
		}
	}
	public class ModifyBloomProps : MonoBehaviour
	{
		private const float ATTACK_COOLDOWN_TIME = 0.41f;

		private const float ATTACK_COOLDOWN_TIME_CH = 0.25f;

		private const float ATTACK_DURATION = 0.36f;

		private const float ATTACK_DURATION_CH = 0.25f;

		private const float DASH_COOLDOWN = 0.6f;

		private const float DASH_COOLDOWN_CH = 0.4f;

		private const float DASH_SPEED = 20f;

		private const float DASH_SPEED_SHARP = 28f;

		private const float RUN_SPEED = 8.3f;

		private const float RUN_SPEED_CH = 10f;

		private const float RUN_SPEED_CH_COMBO = 11.4f;

		private const float SHADOW_DASH_COOLDOWN = 1.5f;

		private const float MULTIPLIER_L1 = 1.1f;

		private const float MULTIPLIER_L2 = 1.2f;

		private HeroController _hc => HeroController.instance;

		public void ModifyPropsL1()
		{
			_hc.DASH_COOLDOWN = 0.54545456f;
			_hc.DASH_COOLDOWN_CH = 0.36363637f;
			_hc.RUN_SPEED = 9.13f;
			_hc.RUN_SPEED_CH = 11f;
			_hc.RUN_SPEED_CH_COMBO = 12.54f;
			_hc.SHADOW_DASH_COOLDOWN = 1.3636364f;
			_hc.shadowRechargePrefab.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmFloat("Shadow Recharge Time").Value = 1.3636364f;
		}

		public void ModifyPropsL2()
		{
			_hc.DASH_COOLDOWN = 0.5f;
			_hc.DASH_COOLDOWN_CH = 0.3333333f;
			_hc.RUN_SPEED = 9.960001f;
			_hc.RUN_SPEED_CH = 12f;
			_hc.RUN_SPEED_CH_COMBO = 13.68f;
			_hc.SHADOW_DASH_COOLDOWN = 1.25f;
			_hc.shadowRechargePrefab.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmFloat("Shadow Recharge Time").Value = 1.25f;
		}

		public void ResetProps()
		{
			_hc.DASH_COOLDOWN = 0.6f;
			_hc.DASH_COOLDOWN_CH = 0.4f;
			_hc.RUN_SPEED = 8.3f;
			_hc.RUN_SPEED_CH = 10f;
			_hc.RUN_SPEED_CH_COMBO = 11.4f;
			_hc.SHADOW_DASH_COOLDOWN = 1.5f;
			_hc.shadowRechargePrefab.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmFloat("Shadow Recharge Time").Value = 1.5f;
		}
	}
	public class Plume : MonoBehaviour
	{
		private const float Extension = 0.01f;

		private const int CollisionMask = 256;

		private PlayMakerFSM _fsm;

		public bool upgraded;

		private const int PlumeDamage = 7;

		private const int PlumeDamageShaman = 11;

		private const float PlumeTime = 0.4f;

		private const float PlumeTimeUpgraded = 0.7f;

		private void Awake()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			((Component)this).transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
			if (!IsGrounded())
			{
				Object.Destroy((Object)(object)((Component)this).gameObject);
			}
			_fsm = FSMUtility.LocateMyFSM(((Component)this).gameObject, "FSM");
			_fsm.GetAction<Wait>("Antic", 2).time.Value = 0.25f;
			_fsm.GetAction<Wait>("End", 0).time.Value = 0.5f;
			_fsm.InsertCoroutine("Plume 2", 0, AnimControl);
			_fsm.GetAction<FloatCompare>("Outside Arena?", 2).float2.Value = float.PositiveInfinity;
			_fsm.GetAction<FloatCompare>("Outside Arena?", 3).float2.Value = float.NegativeInfinity;
		}

		private void Start()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			DamageEnemies obj = ((Component)this).gameObject.AddComponent<DamageEnemies>();
			obj.ignoreInvuln = false;
			obj.attackType = (AttackTypes)2;
			obj.direction = 2f;
			obj.damageDealt = (PlayerData.instance.equippedCharm_19 ? 11 : 7);
		}

		private IEnumerator AnimControl()
		{
			yield return (object)new WaitForSeconds(0.1f);
			((Behaviour)((Component)this).gameObject.GetComponent<tk2dSpriteAnimator>()).enabled = false;
			yield return (object)new WaitForSeconds(upgraded ? 0.7f : 0.4f);
			((Behaviour)((Component)this).gameObject.GetComponent<tk2dSpriteAnimator>()).enabled = true;
		}

		private bool IsGrounded()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			float num = 1.01f;
			return RaycastHit2D.op_Implicit(Physics2D.Raycast(Vector2.op_Implicit(((Component)this).transform.position) + Vector2.up, Vector2.down, num, 256));
		}
	}
	public class Purity : MonoBehaviour
	{
		private float timer;

		private bool timerRunning;

		private bool audioMax;

		private float duration;

		private const float PURITY_DURATION_DEFAULT = 3.4f;

		private const float PURITY_DURATION_26 = 4f;

		private const float ATTACK_COOLDOWN_DEFAULT = 0.41f;

		private const float ATTACK_COOLDOWN_DEFAULT_32 = 0.25f;

		private const float ATTACK_COOLDOWN_44 = 0.49f;

		private const float ATTACK_COOLDOWN_44_32 = 0.31f;

		private const float ATTACK_DURATION_DEFAULT = 0.35f;

		private const float ATTACK_DURATION_DEFAULT_32 = 0.28f;

		private const float ATTACK_DURATION_44 = 0.44f;

		private const float ATTACK_DURATION_44_32 = 0.25f;

		private const float COOLDOWN_CAP_44 = 0.17f;

		private const float COOLDOWN_CAP_44_32 = 0.13f;

		private HeroController _hc = HeroController.instance;

		private PlayerData _pd = PlayerData.instance;

		private List<NailSlash> nailSlashes;

		private void OnEnable()
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Expected O, but got Unknown
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Expected O, but got Unknown
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Expected O, but got Unknown
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Expected O, but got Unknown
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Expected O, but got Unknown
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Expected O, but got Unknown
			nailSlashes = new List<NailSlash>
			{
				HeroController.instance.normalSlash,
				HeroController.instance.alternateSlash,
				HeroController.instance.downSlash,
				HeroController.instance.upSlash,
				HeroController.instance.wallSlash
			};
			HealthManager.TakeDamage += new hook_TakeDamage(IncrementSpeed);
			ModHooks.CharmUpdateHook += new CharmUpdateHandler(Duration);
			HeroController.CanDoubleJump += new hook_CanDoubleJump(FixDoubleJump);
			HeroController.CanCast += new hook_CanCast(FixCast);
			ModHooks.AfterTakeDamageHook += new AfterTakeDamageHandler(ResetSpeed);
			SceneManager.activeSceneChanged += Dummy;
			HeroController.CanNailCharge += new hook_CanNailCharge(CancelNailArts);
			_hc.ATTACK_COOLDOWN_TIME = 0.49f;
			_hc.ATTACK_COOLDOWN_TIME_CH = 0.31f;
			_hc.ATTACK_DURATION = 0.44f;
			_hc.ATTACK_DURATION_CH = 0.25f;
		}

		private void OnDisable()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Expected O, but got Unknown
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Expected O, but got Unknown
			HealthManager.TakeDamage -= new hook_TakeDamage(IncrementSpeed);
			ModHooks.CharmUpdateHook -= new CharmUpdateHandler(Duration);
			HeroController.CanDoubleJump -= new hook_CanDoubleJump(FixDoubleJump);
			HeroController.CanCast -= new hook_CanCast(FixCast);
			ModHooks.AfterTakeDamageHook -= new AfterTakeDamageHandler(ResetSpeed);
			SceneManager.activeSceneChanged -= Dummy;
			HeroController.CanNailCharge -= new hook_CanNailCharge(CancelNailArts);
			_hc.ATTACK_COOLDOWN_TIME = 0.41f;
			_hc.ATTACK_COOLDOWN_TIME_CH = 0.25f;
			_hc.ATTACK_DURATION = 0.35f;
			_hc.ATTACK_DURATION_CH = 0.28f;
		}

		private void Update()
		{
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			if (((OneAxisInputControl)InputHandler.Instance.inputActions.attack).IsPressed && ReflectionHelper.CallMethod<HeroController, bool>(_hc, "CanAttack", Array.Empty<object>()))
			{
				ReflectionHelper.CallMethod<HeroController>(_hc, "DoAttack", Array.Empty<object>());
			}
			if (!timerRunning)
			{
				return;
			}
			timer += Time.deltaTime;
			if (!(timer >= duration))
			{
				return;
			}
			timerRunning = false;
			if ((!_pd.equippedCharm_32 && _hc.ATTACK_COOLDOWN_TIME <= 0.48f) || (_pd.equippedCharm_32 && _hc.ATTACK_COOLDOWN_TIME_CH <= 0.3f))
			{
				((MonoBehaviour)(object)this).PlayAudio(ABManager.LoadFromUnlocks<AudioClip>("purity_reset"));
			}
			_hc.ATTACK_COOLDOWN_TIME = 0.49f;
			_hc.ATTACK_COOLDOWN_TIME_CH = 0.31f;
			_hc.ATTACK_DURATION = 0.44f;
			_hc.ATTACK_DURATION_CH = 0.25f;
			foreach (NailSlash nailSlash in nailSlashes)
			{
				if (((tk2dBaseSprite)((Component)nailSlash).GetComponent<tk2dSprite>()).color != Color.black)
				{
					((tk2dBaseSprite)((Component)nailSlash).GetComponent<tk2dSprite>()).color = Color.white;
					((Component)nailSlash).GetComponent<AudioSource>().pitch = 1f;
				}
			}
			audioMax = false;
			timer = 0f;
		}

		private int ResetSpeed(int hazardType, int damageAmount)
		{
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			timerRunning = false;
			if ((!_pd.equippedCharm_32 && _hc.ATTACK_COOLDOWN_TIME <= 0.48f) || (_pd.equippedCharm_32 && _hc.ATTACK_COOLDOWN_TIME_CH <= 0.3f))
			{
				((MonoBehaviour)(object)this).PlayAudio(ABManager.LoadFromUnlocks<AudioClip>("purity_reset"));
			}
			_hc.ATTACK_COOLDOWN_TIME = 0.49f;
			_hc.ATTACK_COOLDOWN_TIME_CH = 0.31f;
			_hc.ATTACK_DURATION = 0.44f;
			_hc.ATTACK_DURATION_CH = 0.25f;
			foreach (NailSlash nailSlash in nailSlashes)
			{
				if (((tk2dBaseSprite)((Component)nailSlash).GetComponent<tk2dSprite>()).color != Color.black)
				{
					((tk2dBaseSprite)((Component)nailSlash).GetComponent<tk2dSprite>()).color = Color.white;
					((Component)nailSlash).GetComponent<AudioSource>().pitch = 1f;
				}
			}
			audioMax = false;
			timer = 0f;
			return damageAmount;
		}

		private void IncrementSpeed(orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Invalid comparison between Unknown and I4
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Invalid comparison between Unknown and I4
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			if ((int)hitInstance.AttackType == 0 || (int)hitInstance.AttackType == 7)
			{
				hitInstance.Multiplier *= 0.8f;
			}
			orig.Invoke(self, hitInstance);
			if ((int)hitInstance.AttackType != 0 && (int)hitInstance.AttackType != 7)
			{
				return;
			}
			timer = 0f;
			timerRunning = true;
			if ((int)hitInstance.AttackType == 0)
			{
				HeroController hc = _hc;
				hc.ATTACK_COOLDOWN_TIME -= 0.035555556f;
				HeroController hc2 = _hc;
				hc2.ATTACK_COOLDOWN_TIME_CH -= 0.016363638f;
				HeroController hc3 = _hc;
				hc3.ATTACK_DURATION -= 0.035555556f;
				HeroController hc4 = _hc;
				hc4.ATTACK_DURATION_CH -= 0.016363638f;
			}
			if (_hc.ATTACK_COOLDOWN_TIME_CH <= 0.17f)
			{
				ReflectionHelper.SetField<HealthManager, float>(self, "evasionByHitRemaining", 0.1f);
			}
			if ((!_pd.equippedCharm_32 && _hc.ATTACK_COOLDOWN_TIME <= 0.18f && !audioMax) || (_pd.equippedCharm_32 && _hc.ATTACK_COOLDOWN_TIME_CH <= 0.14f && !audioMax))
			{
				audioMax = true;
				((MonoBehaviour)(object)this).PlayAudio(ABManager.LoadFromUnlocks<AudioClip>("purity_max"), 0.5f);
			}
			if (_hc.ATTACK_COOLDOWN_TIME <= 0.17f)
			{
				_hc.ATTACK_COOLDOWN_TIME = 0.17f;
			}
			if (_hc.ATTACK_COOLDOWN_TIME_CH <= 0.13f)
			{
				_hc.ATTACK_COOLDOWN_TIME_CH = 0.13f;
			}
			if (_hc.ATTACK_DURATION <= 0.17f)
			{
				_hc.ATTACK_DURATION = 0.17f;
			}
			if (_hc.ATTACK_DURATION_CH <= 0.13f)
			{
				_hc.ATTACK_DURATION_CH = 0.13f;
			}
			if (((_pd.equippedCharm_32 || !(_hc.ATTACK_COOLDOWN_TIME <= 0.17f)) && (!_pd.equippedCharm_32 || _hc.ATTACK_COOLDOWN_TIME_CH != 0.13f)) || (_pd.equippedCharm_6 && _pd.health == 1))
			{
				return;
			}
			foreach (NailSlash nailSlash in nailSlashes)
			{
				if (((tk2dBaseSprite)((Component)nailSlash).GetComponent<tk2dSprite>()).color != Color.black)
				{
					((tk2dBaseSprite)((Component)nailSlash).GetComponent<tk2dSprite>()).color = new Color(0.619f, 0.798f, 0.881f);
				}
			}
		}

		private void Duration(PlayerData data, HeroController controller)
		{
			if (data.equippedCharm_26)
			{
				duration = 4f;
			}
			else
			{
				duration = 3.4f;
			}
		}

		private void Dummy(Scene From, Scene To)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (((Scene)(ref To)).name == "Deepnest_East_16")
			{
				FsmUtil.InsertMethod(FSMUtility.LocateMyFSM(UnityExtensions.FindGameObject(To, "Training Dummy"), "Hit").GetState("Light Dir"), (Action)delegate
				{
					IncrementSpeedDummy();
				}, 0);
			}
		}

		private void IncrementSpeedDummy()
		{
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			timer = 0f;
			timerRunning = true;
			HeroController hc = _hc;
			hc.ATTACK_COOLDOWN_TIME -= 0.035555556f;
			HeroController hc2 = _hc;
			hc2.ATTACK_COOLDOWN_TIME_CH -= 0.016363638f;
			HeroController hc3 = _hc;
			hc3.ATTACK_DURATION -= 0.035555556f;
			HeroController hc4 = _hc;
			hc4.ATTACK_DURATION_CH -= 0.016363638f;
			if ((!_pd.equippedCharm_32 && _hc.ATTACK_COOLDOWN_TIME <= 0.18f && !audioMax) || (_pd.equippedCharm_32 && _hc.ATTACK_COOLDOWN_TIME_CH <= 0.14f && !audioMax))
			{
				audioMax = true;
				((MonoBehaviour)(object)this).PlayAudio(ABManager.LoadFromUnlocks<AudioClip>("purity_max"), 0.5f);
			}
			if (_hc.ATTACK_COOLDOWN_TIME <= 0.17f)
			{
				_hc.ATTACK_COOLDOWN_TIME = 0.17f;
			}
			if (_hc.ATTACK_COOLDOWN_TIME_CH <= 0.13f)
			{
				_hc.ATTACK_COOLDOWN_TIME_CH = 0.13f;
			}
			if (_hc.ATTACK_DURATION <= 0.17f)
			{
				_hc.ATTACK_DURATION = 0.17f;
			}
			if (_hc.ATTACK_DURATION_CH <= 0.13f)
			{
				_hc.ATTACK_DURATION_CH = 0.13f;
			}
			if (((_pd.equippedCharm_32 || !(_hc.ATTACK_COOLDOWN_TIME <= 0.17f)) && (!_pd.equippedCharm_32 || _hc.ATTACK_COOLDOWN_TIME_CH != 0.13f)) || (_pd.equippedCharm_6 && _pd.health == 1))
			{
				return;
			}
			foreach (NailSlash nailSlash in nailSlashes)
			{
				if (((tk2dBaseSprite)((Component)nailSlash).GetComponent<tk2dSprite>()).color != Color.black)
				{
					((tk2dBaseSprite)((Component)nailSlash).GetComponent<tk2dSprite>()).color = new Color(0.619f, 0.798f, 0.881f);
				}
			}
		}

		private bool CancelNailArts(orig_CanNailCharge orig, HeroController self)
		{
			return false;
		}

		private bool FixCast(orig_CanCast orig, HeroController self)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Invalid comparison between Unknown and I4
			if (!GameManager.instance.isPaused && !self.cState.dashing && (int)self.hero_state != 7 && !self.cState.backDashing && !self.cState.recoiling && !self.cState.recoilFrozen && !self.cState.transitioning && !self.cState.hazardDeath && !self.cState.hazardRespawning && self.CanInput())
			{
				return ReflectionHelper.GetField<HeroController, float>(self, "preventCastByDialogueEndTimer") <= 0f;
			}
			return false;
		}

		private bool FixDoubleJump(orig_CanDoubleJump orig, HeroController self)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Invalid comparison between Unknown and I4
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Invalid comparison between Unknown and I4
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Invalid comparison between Unknown and I4
			if (self.playerData.GetBool("hasDoubleJump") && !self.controlReqlinquished && !ReflectionHelper.GetField<HeroController, bool>(self, "doubleJumped") && !self.inAcid && (int)self.hero_state != 7 && (int)self.hero_state != 5 && (int)self.hero_state != 6 && !self.cState.dashing && !self.cState.wallSliding && !self.cState.backDashing && !self.cState.bouncing && !self.cState.shroomBouncing)
			{
				return !self.cState.onGround;
			}
			return false;
		}

		private void CancelMantis(orig_SetMantis orig, NailSlash self, bool set)
		{
			orig.Invoke(self, false);
		}

		private void CancelLongnail(orig_SetLongnail orig, NailSlash self, bool set)
		{
			orig.Invoke(self, false);
		}
	}
	public class RoyalAura : MonoBehaviour
	{
		public GameObject dungCloud;

		private GameObject _dungTrail;

		private PlayMakerFSM _dungControl;

		private PlayMakerFSM _dungTrailControl;

		private PlayMakerFSM _spellControl;

		private readonly Color PaleColor;

		private float _frequency;

		private int _dungDamage;

		private float _timer;

		private void OnEnable()
		{
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Expected O, but got Unknown
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Expected O, but got Unknown
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Expected O, but got Unknown
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Expected O, but got Unknown
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Expected O, but got Unknown
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Expected O, but got Unknown
			GameObject gameObject = ((Component)((Component)HeroController.instance).transform.Find("Charm Effects").Find("Dung")).gameObject;
			_dungControl = FSMUtility.LocateMyFSM(gameObject, "Control");
			StartupPool[] startupPools = ObjectPool.instance.startupPools;
			foreach (StartupPool val in startupPools)
			{
				if (((Object)val.prefab).name == "Knight Dung Trail")
				{
					_dungTrail = Object.Instantiate<GameObject>(val.prefab);
					_dungTrail.SetActive(false);
					Object.DontDestroyOnLoad((Object)(object)_dungTrail);
				}
				if (((Object)val.prefab).name == "Knight Dung Cloud")
				{
					dungCloud = Object.Instantiate<GameObject>(val.prefab);
					dungCloud.SetActive(false);
					Object.DontDestroyOnLoad((Object)(object)dungCloud);
				}
			}
			_spellControl = HeroController.instance.spellControl;
			if ((Object)(object)USceneUtil.Find(gameObject, "Particle 1").GetComponent<ModifyAuraColor>() == (Object)null)
			{
				USceneUtil.Find(gameObject, "Particle 1").AddComponent<ModifyAuraColor>();
			}
			_dungTrailControl = FSMUtility.LocateMyFSM(_dungTrail, "Control");
			MainModule main = _dungTrailControl.Fsm.GetFsmGameObject("Pt Normal").Value.GetComponent<ParticleSystem>().main;
			((MainModule)(ref main)).startColor = MinMaxGradient.op_Implicit(PaleColor);
			ParticleSystem[] componentsInChildren = dungCloud.GetComponentsInChildren<ParticleSystem>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				MainModule main2 = componentsInChildren[i].main;
				((MainModule)(ref main2)).startColor = MinMaxGradient.op_Implicit(PaleColor);
			}
			tk2dSprite[] componentsInChildren2 = dungCloud.GetComponentsInChildren<tk2dSprite>(true);
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				((tk2dBaseSprite)componentsInChildren2[i]).color = Color.white;
			}
			_dungControl.GetAction<Wait>("Emit Pause", 2).time.Value = 0.1f;
			((FsmStateAction)_dungControl.GetAction<SpawnObjectFromGlobalPoolOverTime>("Equipped", 0)).Enabled = false;
			string[] array = new string[2] { "Dung Cloud", "Dung Cloud 2" };
			foreach (string text in array)
			{
				if ((Object)(object)_spellControl == (Object)null)
				{
					break;
				}
				((FsmStateAction)_spellControl.GetAction<SpawnObjectFromGlobalPool>(text, 0)).Enabled = false;
				FsmUtil.InsertMethod(_spellControl, text, (Action)delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0020: Unknown result type (might be due to invalid IL or missing references)
					//IL_0025: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0062: Unknown result type (might be due to invalid IL or missing references)
					//IL_0067: Unknown result type (might be due to invalid IL or missing references)
					//IL_006a: Unknown result type (might be due to invalid IL or missing references)
					//IL_006f: Unknown result type (might be due to invalid IL or missing references)
					Object.Instantiate<GameObject>(dungCloud, ((Component)this).transform.position - new Vector3(0f, 0f, -0.001f), Quaternion.identity).SetActive(true);
					MainModule main3 = ((Component)((Component)HeroController.instance).transform.Find("Charm Effects").Find("Dung Recharge")).gameObject.GetComponent<ParticleSystem>().main;
					((MainModule)(ref main3)).startColor = MinMaxGradient.op_Implicit(Color.white);
				}, 1);
			}
			ExtraDamageable.RecieveExtraDamage += new hook_RecieveExtraDamage(ExtraDamageableRecieveExtraDamage);
			ExtraDamageable.GetDamageOfType += new hook_GetDamageOfType(ExtraDamageableGetDamageOfType);
			SpriteFlash.flashDungQuick += new hook_flashDungQuick(SpriteFlashFlashDungQuick);
			KnightHatchling.OnEnable += new hook_OnEnable(KnightHatchlingOnEnable);
			ActivateAllChildren.OnEnter += new hook_OnEnter(ActivateAllChildrenOnEnter);
			Tk2dPlayAnimation.OnEnter += new hook_OnEnter(Tk2dPlayAnimationOnEnter);
		}

		private void ExtraDamageableRecieveExtraDamage(orig_RecieveExtraDamage orig, ExtraDamageable self, ExtraDamageTypes extraDamageType)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			if (((int)extraDamageType == 1 || (int)extraDamageType == 2) && !Object.op_Implicit((Object)(object)((Component)self).gameObject.GetComponent<RoyalAuraSpread>()))
			{
				((Component)self).gameObject.AddComponent<RoyalAuraSpread>();
			}
			orig.Invoke(self, extraDamageType);
		}

		private int ExtraDamageableGetDamageOfType(orig_GetDamageOfType orig, ExtraDamageTypes extraDamageTypes)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			if ((int)extraDamageTypes == 1 || (int)extraDamageTypes == 2)
			{
				return _dungDamage;
			}
			return orig.Invoke(extraDamageTypes);
		}

		private void SpriteFlashFlashDungQuick(orig_flashDungQuick orig, SpriteFlash self)
		{
			self.flashArmoured();
		}

		private void KnightHatchlingOnEnable(orig_OnEnable orig, KnightHatchling self)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			TypeDetails normalDetails = self.normalDetails;
			normalDetails.damage = self.dungDetails.damage;
			normalDetails.dung = true;
			normalDetails.spatterColor = Color.white;
			Mirror.SetField<KnightHatchling, TypeDetails>(self, "details", normalDetails);
			MainModule main = self.dungPt.main;
			((MainModule)(ref main)).startColor = MinMaxGradient.op_Implicit(PaleColor);
		}

		private void ActivateAllChildrenOnEnter(orig_OnEnter orig, ActivateAllChildren self)
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			if (((FsmStateAction)self).Fsm.GameObjectName.Contains("Dung Explosion") && ((FsmStateAction)self).State.Name == "Explode")
			{
				Object.Instantiate<GameObject>(dungCloud, ((FsmStateAction)self).Fsm.GameObject.transform.position, Quaternion.identity).SetActive(true);
				Component[] componentsInChildren = ((FsmStateAction)self).Fsm.GameObject.GetComponentsInChildren<Component>(true);
				foreach (Component val in componentsInChildren)
				{
					if (!((Object)val.gameObject).name.Contains("Impact"))
					{
						Object.Destroy((Object)(object)val.gameObject);
					}
					else
					{
						((tk2dBaseSprite)val.gameObject.GetComponent<tk2dSprite>()).color = Color.white;
					}
				}
			}
			orig.Invoke(self);
		}

		private void Tk2dPlayAnimationOnEnter(orig_OnEnter orig, Tk2dPlayAnimation self)
		{
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			if (((FsmStateAction)self).Fsm.GameObjectName.Contains("Spell Fluke Dung") && ((FsmStateAction)self).State.Name == "Init" && self.clipName.Value == "Dung Air")
			{
				PlayMakerFSM fsmComponent = ((FsmStateAction)self).Fsm.FsmComponent;
				((tk2dBaseSprite)((Component)fsmComponent).gameObject.GetComponent<tk2dSprite>()).color = Color.white;
				MainModule main = FsmUtil.GetFsmGameObjectVariable(fsmComponent, "Pt Antic").Value.GetComponent<ParticleSystem>().main;
				((MainModule)(ref main)).startColor = MinMaxGradient.op_Implicit(PaleColor);
				GameObject value = FsmUtil.GetFsmGameObjectVariable(fsmComponent, "Dung Cloud").Value;
				ParticleSystem[] componentsInChildren = value.GetComponentsInChildren<ParticleSystem>(true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					MainModule main2 = componentsInChildren[i].main;
					((MainModule)(ref main2)).startColor = MinMaxGradient.op_Implicit(PaleColor);
				}
				tk2dSprite[] componentsInChildren2 = value.GetComponentsInChildren<tk2dSprite>(true);
				for (int i = 0; i < componentsInChildren2.Length; i++)
				{
					((tk2dBaseSprite)componentsInChildren2[i]).color = Color.white;
				}
			}
			orig.Invoke(self);
		}

		private void Update()
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			_timer += Time.deltaTime;
			if (_timer > _frequency)
			{
				_timer = 0f;
				GameObject obj = Object.Instantiate<GameObject>(_dungTrail, ((Component)HeroController.instance).transform.position, Quaternion.identity);
				Transform transform = obj.transform;
				transform.localScale *= 2f;
				Extensions.SetPositionZ(obj.transform, 0.01f);
				obj.SetActive(true);
			}
		}

		private void OnDisable()
		{
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Expected O, but got Unknown
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Expected O, but got Unknown
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Expected O, but got Unknown
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Expected O, but got Unknown
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Expected O, but got Unknown
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Expected O, but got Unknown
			Object.Destroy((Object)(object)_dungTrail);
			Object.Destroy((Object)(object)dungCloud);
			_dungControl.GetAction<Wait>("Emit Pause", 2).time.Value = 0.5f;
			((FsmStateAction)_dungControl.GetAction<SpawnObjectFromGlobalPoolOverTime>("Equipped", 0)).Enabled = true;
			string[] array = new string[2] { "Dung Cloud", "Dung Cloud 2" };
			foreach (string text in array)
			{
				if ((Object)(object)_spellControl == (Object)null)
				{
					break;
				}
				FsmUtil.RemoveAction(_spellControl, text, 1);
				((FsmStateAction)_spellControl.GetAction<SpawnObjectFromGlobalPool>(text, 0)).Enabled = true;
			}
			ExtraDamageable.RecieveExtraDamage -= new hook_RecieveExtraDamage(ExtraDamageableRecieveExtraDamage);
			ExtraDamageable.GetDamageOfType -= new hook_GetDamageOfType(ExtraDamageableGetDamageOfType);
			SpriteFlash.flashDungQuick -= new hook_flashDungQuick(SpriteFlashFlashDungQuick);
			KnightHatchling.OnEnable -= new hook_OnEnable(KnightHatchlingOnEnable);
			ActivateAllChildren.OnEnter -= new hook_OnEnter(ActivateAllChildrenOnEnter);
			Tk2dPlayAnimation.OnEnter -= new hook_OnEnter(Tk2dPlayAnimationOnEnter);
		}

		public RoyalAura()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			PaleColor = new Color(0.65f, 0.65f, 0.65f, 0.75f);
			_frequency = 0.5f;
			_dungDamage = 2;
			((MonoBehaviour)this)..ctor();
		}
	}
	public class RoyalAuraSpread : MonoBehaviour
	{
		private int _repeat = 5;

		private float _cooldown = 0.75f;

		private GameObject _dungTrail;

		private PlayMakerFSM _dungTrailControl;

		private ParticleSystem _dungPt;

		private void Awake()
		{
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			StartupPool[] startupPools = ObjectPool.instance.startupPools;
			foreach (StartupPool val in startupPools)
			{
				if (((Object)val.prefab).name == "Knight Dung Trail")
				{
					_dungTrail = Object.Instantiate<GameObject>(val.prefab);
					_dungTrail.SetActive(false);
					Object.DontDestroyOnLoad((Object)(object)_dungTrail);
					break;
				}
			}
			_dungTrailControl = FSMUtility.LocateMyFSM(_dungTrail, "Control");
			_dungPt = _dungTrailControl.Fsm.GetFsmGameObject("Pt Normal").Value.GetComponent<ParticleSystem>();
			MainModule main = _dungPt.main;
			((MainModule)(ref main)).startColor = MinMaxGradient.op_Implicit(new Color(0.65f, 0.65f, 0.65f, 0.75f));
			((MonoBehaviour)this).StartCoroutine(StartCooldown());
		}

		private IEnumerator StartCooldown()
		{
			for (int i = 0; i < _repeat; i++)
			{
				HealthManager component = ((Component)this).gameObject.GetComponent<HealthManager>();
				if (component.hp <= 0 || component.isDead)
				{
					break;
				}
				GameObject obj = Object.Instantiate<GameObject>(_dungTrail, ((Component)this).gameObject.transform.position, Quaternion.identity);
				Transform transform = obj.transform;
				transform.localScale *= 2f;
				obj.SetActive(true);
				yield return (object)new WaitForSeconds(_cooldown);
				yield return (object)new WaitUntil((Func<bool>)(() => ((Component)this).gameObject.activeSelf));
			}
			yield return (object)new WaitForSeconds(1f);
			Object.Destroy((Object)(object)this);
		}

		private void OnDestroy()
		{
			Object.Destroy((Object)(object)_dungTrail);
			((MonoBehaviour)this).StopAllCoroutines();
		}
	}
	public class ShadeSlash : MonoBehaviour
	{
		public AttackDirection attackDirection;

		private HeroController _hc => HeroController.instance;

		private void OnTriggerEnter2D(Collider2D collider)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected I4, but got Unknown
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Invalid comparison between Unknown and I4
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Invalid comparison between Unknown and I4
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Invalid comparison between Unknown and I4
			if (!((Object)(object)collider != (Object)null))
			{
				return;
			}
			AttackDirection val = attackDirection;
			switch ((int)val)
			{
			case 0:
				if (!_hc.cState.facingRight)
				{
					if (((Component)collider).gameObject.layer == 11 && ((Object)(object)((Component)collider).gameObject.GetComponent<NonBouncer>() == (Object)null || !((Component)collider).gameObject.GetComponent<NonBouncer>().active))
					{
						if ((Object)(object)((Component)collider).gameObject.GetComponent<BounceShroom>() != (Object)null)
						{
							_hc.RecoilRightLong();
							Bounce(collider, useEffects: false);
						}
						else
						{
							_hc.RecoilRight();
						}
					}
					if (((Component)collider).gameObject.layer == 19 && (Object)(object)((Component)collider).gameObject.GetComponent<BounceShroom>() != (Object)null)
					{
						_hc.RecoilRightLong();
						Bounce(collider, useEffects: false);
					}
					break;
				}
				if (((Component)collider).gameObject.layer == 11 && ((Object)(object)((Component)collider).gameObject.GetComponent<NonBouncer>() == (Object)null || !((Component)collider).gameObject.GetComponent<NonBouncer>().active))
				{
					if ((Object)(object)((Component)collider).gameObject.GetComponent<BounceShroom>() != (Object)null)
					{
						_hc.RecoilLeftLong();
						Bounce(collider, useEffects: false);
					}
					else
					{
						_hc.RecoilLeft();
					}
				}
				if (((Component)collider).gameObject.layer == 19 && (Object)(object)((Component)collider).gameObject.GetComponent<BounceShroom>() != (Object)null)
				{
					_hc.RecoilLeftLong();
					Bounce(collider, useEffects: false);
				}
				break;
			case 1:
				if (((Component)collider).gameObject.layer == 11 && ((Object)(object)((Component)collider).gameObject.GetComponent<NonBouncer>() == (Object)null || !((Component)collider).gameObject.GetComponent<NonBouncer>().active))
				{
					if ((Object)(object)((Component)collider).gameObject.GetComponent<BounceShroom>() != (Object)null)
					{
						_hc.RecoilDown();
						Bounce(collider, useEffects: false);
					}
					else
					{
						_hc.RecoilDown();
					}
				}
				if (((Component)collider).gameObject.layer == 19 && (Object)(object)((Component)collider).gameObject.GetComponent<BounceShroom>() != (Object)null)
				{
					_hc.RecoilDown();
					Bounce(collider, useEffects: false);
				}
				break;
			case 2:
			{
				PhysLayers val2 = (PhysLayers)((Component)collider).gameObject.layer;
				if (((int)val2 == 11 || (int)val2 == 19 || (int)val2 == 17) && ((Object)(object)((Component)collider).gameObject.GetComponent<NonBouncer>() == (Object)null || !((Component)collider).gameObject.GetComponent<NonBouncer>().active))
				{
					if ((Object)(object)((Component)collider).gameObject.GetComponent<BigBouncer>() != (Object)null)
					{
						_hc.BounceHigh();
					}
					else if ((Object)(object)((Component)collider).gameObject.GetComponent<BounceShroom>() != (Object)null)
					{
						_hc.ShroomBounce();
						Bounce(collider, useEffects: true);
					}
					else
					{
						_hc.Bounce();
					}
				}
				break;
			}
			}
		}

		private void Bounce(Collider2D otherCollider, bool useEffects)
		{
			PlayMakerFSM val = FSMUtility.LocateFSM(((Component)otherCollider).gameObject, "Bounce Shroom");
			if (Object.op_Implicit((Object)(object)val))
			{
				val.SendEvent("BOUNCE UPWARD");
				return;
			}
			BounceShroom component = ((Component)otherCollider).GetComponent<BounceShroom>();
			if (Object.op_Implicit((Object)(object)component))
			{
				component.BounceLarge(useEffects);
			}
		}

		private void Log(object o)
		{
			Logger.Log("[Shade Slash] " + o);
		}
	}
	internal class AutoDestroy : MonoBehaviour
	{
		public Func<bool> ShouldDestroy;

		internal float Time { get; set; } = 0.7f;

		private void Start()
		{
			((MonoBehaviour)this).StartCoroutine(Wait());
		}

		private IEnumerator Wait()
		{
			if (ShouldDestroy != null)
			{
				yield return (object)new WaitWhile((Func<bool>)(() => !ShouldDestroy()));
			}
			else
			{
				yield return (object)new WaitForSeconds(Time);
			}
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}
	internal static class DebugModHook
	{
		public static void GiveAllCharms(Action callback)
		{
			BindableFunctions.OnGiveAllCharms += callback;
		}

		public static void RemoveAllCharms(Action callback)
		{
			BindableFunctions.OnRemoveAllCharms += callback;
		}
	}
	public static class Extensions
	{
		public static bool IsPlaying(this Animator anim)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			AnimatorStateInfo currentAnimatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);
			return ((AnimatorStateInfo)(ref currentAnimatorStateInfo)).normalizedTime <= 1f;
		}

		public static bool IsPlaying(this Animator anim, string s)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			AnimatorStateInfo currentAnimatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);
			if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).IsName(s))
			{
				return anim.IsPlaying();
			}
			return false;
		}

		public static int GetCurrentFrame(this Animator anim)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			if (anim.GetCurrentAnimatorClipInfo(0).Length == 0)
			{
				Logger.Log("Warning: Could not find animator " + ((Object)anim).name + " clip in GetCurrentFrame.");
				return 999999;
			}
			AnimatorClipInfo val = anim.GetCurrentAnimatorClipInfo(0)[0];
			AnimatorStateInfo currentAnimatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);
			return (int)(((AnimatorStateInfo)(ref currentAnimatorStateInfo)).normalizedTime % 1f * (((AnimatorClipInfo)(ref val)).clip.length * ((AnimatorClipInfo)(ref val)).clip.frameRate));
		}

		public static void PlayAt(this Animator anim, string name, int frame)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			if (PaleCourtCharms.AnimClips.ContainsKey(name))
			{
				AnimationClip val = PaleCourtCharms.AnimClips[name];
				float num = (float)frame / (val.length * val.frameRate);
				anim.Play(name, -1, num);
			}
			else
			{
				Logger.Log("WARNING: Could not find aninmation clip, reverting to old method.");
				AnimatorClipInfo val2 = anim.GetCurrentAnimatorClipInfo(0)[0];
				float num2 = (float)frame / (((AnimatorClipInfo)(ref val2)).clip.length * ((AnimatorClipInfo)(ref val2)).clip.frameRate);
				anim.Play(name, 0, num2);
			}
		}

		[Pure]
		public static IEnumerator PlayBlocking(this Animator self, string anim)
		{
			self.Play(anim, -1, 0f);
			yield return (object)new WaitForEndOfFrame();
			while (true)
			{
				AnimatorStateInfo currentAnimatorStateInfo = self.GetCurrentAnimatorStateInfo(0);
				if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).normalizedTime <= 1f)
				{
					yield return null;
					continue;
				}
				break;
			}
		}

		[Pure]
		public static IEnumerator PlayToFrame(this Animator self, string anim, int frame)
		{
			self.Play(anim, -1, 0f);
			yield return (object)new WaitForEndOfFrame();
			yield return (object)new WaitWhile((Func<bool>)(() => self.GetCurrentFrame() < frame));
		}

		[Pure]
		public static IEnumerator PlayToFrameAt(this Animator self, string anim, int start, int frame)
		{
			self.PlayAt(anim, start);
			yield return (object)new WaitForEndOfFrame();
			yield return (object)new WaitWhile((Func<bool>)(() => self.GetCurrentFrame() < frame));
		}

		[Pure]
		public static IEnumerator WaitToFrame(this Animator self, int frame)
		{
			yield return (object)new WaitForEndOfFrame();
			while (self.GetCurrentFrame() < frame)
			{
				yield return null;
			}
		}

		[Pure]
		public static IEnumerator PlayToEnd(this Animator self, string name)
		{
			self.Play(name, -1, 0f);
			yield return (object)new WaitForEndOfFrame();
			while (true)
			{
				AnimatorStateInfo currentAnimatorStateInfo = self.GetCurrentAnimatorStateInfo(0);
				if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).normalizedTime <= 1f)
				{
					yield return null;
					continue;
				}
				break;
			}
		}

		[Pure]
		public static IEnumerator PlayToEnd(this Animator self)
		{
			yield return (object)new WaitForEndOfFrame();
			while (true)
			{
				AnimatorStateInfo currentAnimatorStateInfo = self.GetCurrentAnimatorStateInfo(0);
				if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).normalizedTime <= 1f)
				{
					yield return null;
					continue;
				}
				break;
			}
		}

		[Pure]
		public static IEnumerator WaitForFramesWithActions(this Animator self, params (int frame, Action act)[] acts)
		{
			yield return (object)new WaitForEndOfFrame();
			for (int i = 0; i < acts.Length; i++)
			{
				var (frame, act) = acts[i];
				yield return self.WaitToFrame(frame);
				act();
			}
		}

		[Pure]
		public static IEnumerator PlayWithActions(this Animator self, string anim, params (int frame, Action act)[] acts)
		{
			self.PlayAt(anim, 0);
			yield return (object)new WaitForEndOfFrame();
			yield return self.WaitForFramesWithActions(acts);
		}

		[Pure]
		public static IEnumerator PlayToEndWithActions(this Animator self, string anim, params (int frame, Action act)[] acts)
		{
			yield return self.PlayWithActions(anim, acts);
			yield return self.PlayToEnd();
		}

		public static IEnumerator PlayBlockingWhile(this Animator self, string anim, Func<bool> predicate)
		{
			self.Play(anim);
			yield return (object)new WaitForEndOfFrame();
			while (predicate())
			{
				yield return null;
			}
		}

		public static bool Within(this float self, float rhs, float threshold)
		{
			return Math.Abs(self - rhs) <= threshold;
		}

		public static AudioSource PlayAudio(this MonoBehaviour mb, AudioClip clip, float volume = 1f, float pitchVariation = 0f, Transform posOverride = null, Func<bool> destroyWhen = null)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = new GameObject("Audio Player", new Type[2]
			{
				typeof(AudioSource),
				typeof(AutoDestroy)
			});
			val.transform.position = (((Object)(object)posOverride == (Object)null) ? ((Component)mb).transform.position : posOverride.position);
			AutoDestroy component = val.GetComponent<AutoDestroy>();
			component.Time = clip.length + 1f;
			component.ShouldDestroy = destroyWhen;
			AudioSource component2 = val.GetComponent<AudioSource>();
			component2.clip = clip;
			component2.volume = volume;
			component2.pitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);
			component2.outputAudioMixerGroup = ((Component)HeroController.instance).GetComponent<AudioSource>().outputAudioMixerGroup;
			component2.Play();
			return component2;
		}
	}
	internal static class FsmUtil
	{
		[PublicAPI]
		public static void RemoveAction<T>(this PlayMakerFSM fsm, string stateName) where T : FsmStateAction
		{
			try
			{
				FsmState state = fsm.GetState(stateName);
				FsmStateAction[] actions = state.Actions;
				FsmStateAction action = (FsmStateAction)(object)fsm.GetAction<T>(stateName);
				actions = actions.Where((FsmStateAction x) => x != action).ToArray();
				Logger.LogFine(((object)action).GetType().ToString());
				state.Actions = actions;
			}
			catch (NullReferenceException)
			{
			}
		}

		[PublicAPI]
		public static FsmState GetState(this PlayMakerFSM fsm, string stateName)
		{
			return fsm.FsmStates.FirstOrDefault((FsmState t) => t.Name == stateName);
		}

		[PublicAPI]
		public static FsmStateAction GetAction(this PlayMakerFSM fsm, string stateName, int index)
		{
			return fsm.GetState(stateName).Actions[index];
		}

		[PublicAPI]
		public static T GetAction<T>(this PlayMakerFSM fsm, string stateName, int index) where T : FsmStateAction
		{
			FsmStateAction action = fsm.GetAction(stateName, index);
			return (T)(object)((action is T) ? action : null);
		}

		[PublicAPI]
		public static T GetAction<T>(this PlayMakerFSM fsm, string stateName) where T : FsmStateAction
		{
			FsmStateAction obj = fsm.GetState(stateName).Actions.FirstOrDefault((FsmStateAction x) => x is T);
			return (T)(object)((obj is T) ? obj : null);
		}

		[PublicAPI]
		public static void InsertAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action, int index)
		{
			FsmState state = fsm.GetState(stateName);
			List<FsmStateAction> list = state.Actions.ToList();
			list.Insert(index, action);
			state.Actions = list.ToArray();
			action.Init(state);
		}

		[PublicAPI]
		public static void InsertAction(this PlayMakerFSM fsm, string state, int ind, FsmStateAction action)
		{
			fsm.InsertAction(state, action, ind);
		}

		[PublicAPI]
		public static void AddTransition(this PlayMakerFSM fsm, string stateName, FsmEvent @event, string toState)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected O, but got Unknown
			FsmState state = fsm.GetState(stateName);
			List<FsmTransition> list = state.Transitions.ToList();
			list.Add(new FsmTransition
			{
				FsmEvent = @event,
				ToState = toState,
				ToFsmState = fsm.GetState(toState)
			});
			state.Transitions = list.ToArray();
		}

		[PublicAPI]
		public static void AddTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			FsmState state = fsm.GetState(stateName);
			List<FsmTransition> list = state.Transitions.ToList();
			list.Add(new FsmTransition
			{
				FsmEvent = new FsmEvent(eventName),
				ToState = toState,
				ToFsmState = fsm.GetState(toState)
			});
			state.Transitions = list.ToArray();
		}

		[PublicAPI]
		public static void RemoveTransitions(this PlayMakerFSM fsm, IEnumerable<string> states, IEnumerable<string> transitions)
		{
			IEnumerable<string> source = (states as string[]) ?? states.ToArray();
			FsmState[] fsmStates = fsm.FsmStates;
			foreach (FsmState val in fsmStates)
			{
				if (source.Contains(val.Name))
				{
					val.Transitions = val.Transitions.Where((FsmTransition trans) => !transitions.Contains(trans.ToState)).ToArray();
				}
			}
		}

		[PublicAPI]
		public static void RemoveTransition(this PlayMakerFSM fsm, string stateName, string transition)
		{
			FsmState state = fsm.GetState(stateName);
			state.Transitions = state.Transitions.Where((FsmTransition trans) => transition != trans.ToState).ToArray();
		}

		[PublicAPI]
		public static void InsertMethod(this PlayMakerFSM fsm, string stateName, int index, Action method, bool everyFrame = false)
		{
			fsm.InsertAction(stateName, (FsmStateAction)(object)new InvokeMethod(method, everyFrame), index);
		}

		[PublicAPI]
		public static void InsertCoroutine(this PlayMakerFSM fsm, string stateName, int index, Func<IEnumerator> coro, bool wait = true)
		{
			fsm.InsertAction(stateName, (FsmStateAction)(object)new InvokeCoroutine(coro, wait), index);
		}

		[PublicAPI]
		public static FsmInt GetOrCreateInt(this PlayMakerFSM fsm, string intName)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			FsmInt val = new FsmInt(intName);
			List<FsmInt> list = fsm.FsmVariables.IntVariables.ToList();
			FsmInt val2 = list.FirstOrDefault((FsmInt x) => ((NamedVariable)x).Name == intName);
			if (val2 != null)
			{
				return val2;
			}
			list.Add(val);
			fsm.Fsm.Variables.IntVariables = list.ToArray();
			return val;
		}

		public static void AddToSendRandomEvent(this SendRandomEvent sre, string toState, float weight, [CanBeNull] string eventName = null)
		{
			MonoBehaviour owner = ((FsmStateAction)sre).Fsm.Owner;
			PlayMakerFSM fsm = (PlayMakerFSM)(object)((owner is PlayMakerFSM) ? owner : null);
			string name = ((FsmStateAction)sre).State.Name;
			eventName = eventName ?? toState.Split(' ').First();
			List<FsmEvent> list = sre.events.ToList();
			List<FsmFloat> list2 = sre.weights.ToList();
			fsm.AddTransition(name, eventName, toState);
			list.Add(fsm.GetState(name).Transitions.Single((FsmTransition x) => x.FsmEvent.Name == eventName).FsmEvent);
			list2.Add(FsmFloat.op_Implicit(weight));
			sre.events = list.ToArray();
			sre.weights = list2.ToArray();
		}

		public static void AddToSendRandomEventV2(this SendRandomEventV2 sre, string toState, float weight, int eventMaxAmount, [CanBeNull] string eventName = null)
		{
			MonoBehaviour owner = ((FsmStateAction)sre).Fsm.Owner;
			PlayMakerFSM fsm = (PlayMakerFSM)(object)((owner is PlayMakerFSM) ? owner : null);
			string name = ((FsmStateAction)sre).State.Name;
			eventName = eventName ?? toState.Split(' ').First();
			List<FsmEvent> list = sre.events.ToList();
			List<FsmFloat> list2 = sre.weights.ToList();
			List<FsmInt> list3 = sre.trackingInts.ToList();
			List<FsmInt> list4 = sre.eventMax.ToList();
			fsm.AddTransition(name, eventName, toState);
			list.Add(fsm.GetState(name).Transitions.Single((FsmTransition x) => x.FsmEvent.Name == eventName).FsmEvent);
			list2.Add(FsmFloat.op_Implicit(weight));
			list3.Add(fsm.GetOrCreateInt("Ct " + eventName));
			list4.Add(FsmInt.op_Implicit(eventMaxAmount));
			sre.events = list.ToArray();
			sre.weights = list2.ToArray();
			sre.trackingInts = list3.ToArray();
			sre.eventMax = list4.ToArray();
		}

		[PublicAPI]
		public static FsmState CreateState(this PlayMakerFSM fsm, string stateName)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			FsmState val = new FsmState(fsm.Fsm)
			{
				Name = stateName
			};
			List<FsmState> list = fsm.FsmStates.ToList();
			list.Add(val);
			fsm.Fsm.States = list.ToArray();
			return val;
		}

		public static void CreateStates(this PlayMakerFSM fsm, string[] stateNames, string finalState)
		{
			for (int num = stateNames.Length - 1; num >= 0; num--)
			{
				string stateName = stateNames[num];
				fsm.CreateState(stateName);
				fsm.AddTransition(stateName, FsmEvent.Finished, (num + 1 < stateNames.Length) ? stateNames[num + 1] : finalState);
			}
		}

		private static void Log(string str)
		{
			Logger.Log("[FSM UTIL]: " + str);
		}
	}
	public class InvokeMethod : FsmStateAction
	{
		private readonly Action _action;

		private readonly bool _everyFrame;

		public InvokeMethod()
		{
			_action = null;
			_everyFrame = false;
		}

		public InvokeMethod(Action a, bool everyFrame = false)
		{
			_action = a;
			_everyFrame = everyFrame;
		}

		public override void OnEnter()
		{
			_action?.Invoke();
			if (!_everyFrame)
			{
				((FsmStateAction)this).Finish();
			}
		}

		public override void OnUpdate()
		{
			_action?.Invoke();
		}
	}
	public class InvokeCoroutine : FsmStateAction
	{
		private readonly Func<IEnumerator> _coro;

		private readonly bool _wait;

		public InvokeCoroutine()
		{
			_coro = null;
			_wait = false;
		}

		public InvokeCoroutine(Func<IEnumerator> f, bool wait)
		{
			_coro = f;
			_wait = wait;
		}

		private IEnumerator Coroutine()
		{
			yield return _coro?.Invoke();
			((FsmStateAction)this).Finish();
		}

		public override void OnEnter()
		{
			((FsmStateAction)this).Fsm.Owner.StartCoroutine(_wait ? Coroutine() : _coro?.Invoke());
			if (!_wait)
			{
				((FsmStateAction)this).Finish();
			}
		}
	}
	public static class GameObjectExtensions
	{
		public static bool FindAndDestroyGameObjectInChildren(this GameObject gameObject, string name)
		{
			bool result = false;
			GameObject val = USceneUtil.FindGameObjectInChildren(gameObject, name);
			if ((Object)(object)val != (Object)null)
			{
				Object.Destroy((Object)(object)val);
				result = true;
			}
			return result;
		}

		public static GameObject FindGameObjectNameContainsInChildren(this GameObject gameObject, string name)
		{
			if ((Object)(object)gameObject == (Object)null)
			{
				return null;
			}
			Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>(true);
			foreach (Transform val in componentsInChildren)
			{
				if (((Object)val).name.Contains(name))
				{
					return ((Component)val).gameObject;
				}
			}
			return null;
		}

		public static string PrintSceneHierarchyPath(this GameObject gameObject)
		{
			if ((Object)(object)gameObject == (Object)null)
			{
				return "WARNING: NULL GAMEOBJECT";
			}
			string result = ((Object)gameObject).name;
			if ((Object)(object)gameObject.transform.parent != (Object)null)
			{
				result = ((Component)gameObject.transform.parent).gameObject.PrintSceneHierarchyPath() + "\\" + ((Object)gameObject).name;
			}
			return result;
		}
	}
	public class SaveModSettings
	{
		public bool[] newCharms = new bool[4] { true, true, true, true };

		public bool[] gotCharms = new bool[4];

		public bool[] equippedCharms = new bool[4];

		public bool upgradedCharm_10;

		public List<int> notchCosts = new List<int>(new int[4]);

		public List<int> EnabledCharms = new List<int>();

		public Vector3 IndicatorPosition1;

		public Vector3 IndicatorPosition2;
	}
	public sealed class WaitSecWhile : CustomYieldInstruction
	{
		private float _time;

		private Func<bool> _pred;

		public override bool keepWaiting
		{
			get
			{
				_time -= Time.unscaledDeltaTime;
				if (_pred())
				{
					return _time > 0f;
				}
				return false;
			}
		}

		public WaitSecWhile(Func<bool> predicate, float seconds)
		{
			_pred = predicate;
			_time = seconds;
		}
	}
	public class PaleCourtCharms : Mod, ILocalSettings<SaveModSettings>, IGlobalSettings<GlobalSettings>, IMenuMod, IMod, ILogger, ITogglableMod
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static hook_StartNewGame <0>__HandleNewGame;
		}

		public static PaleCourtCharms Instance;

		public static LanguageStrings LangStrings;

		public static readonly List<CharmDefinition> Charms = new List<CharmDefinition>();

		public static readonly string[] CharmKeys = new string[4] { "Mark_of_Purity", "Vessels_Lament", "Boon_of_Hallownest", "Abyssal_Bloom" };

		public static readonly int[] CharmCosts = new int[4] { 3, 2, 4, 5 };

		public static List<int> CharmIDs = new List<int>();

		public static Dictionary<int, int> CharmCostsByID = new Dictionary<int, int>();

		public static readonly Dictionary<string, Sprite> SPRITES = new Dictionary<string, Sprite>();

		private SaveModSettings localSettings;

		private static bool randoInitialized = false;

		private GlobalSettings globalSettings;

		public static Dictionary<string, AudioClip> Clips { get; } = new Dictionary<string, AudioClip>();

		public static Dictionary<string, AnimationClip> AnimClips { get; } = new Dictionary<string, AnimationClip>();

		public static Dictionary<string, GameObject> preloadedGO { get; } = new Dictionary<string, GameObject>();

		public static PlayMakerFSM PVControl { get; internal set; }

		public static SaveModSettings Settings => Instance?.localSettings;

		public bool ToggleButtonInsideMenu => true;

		public static GlobalSettings GlobalSettings => Instance?.globalSettings;

		public SaveModSettings OnSaveLocal()
		{
			if (ModHooks.GetMod("Randomizer 4", false, false) is Mod)
			{
				localSettings.notchCosts = (from kvp in CharmCostsByID
					orderby Array.IndexOf(CharmKeys, CharmKeys[CharmIDs.IndexOf(kvp.Key)])
					select kvp.Value).ToList();
			}
			return localSettings;
		}

		public void OnLoadLocal(SaveModSettings s)
		{
			localSettings = s;
			if (ModHooks.GetMod("Randomizer 4", false, false) is Mod && IsRandoSave() && localSettings.notchCosts.Count == CharmKeys.Length)
			{
				for (int i = 0; i < CharmKeys.Length; i++)
				{
					int key = CharmIDs[i];
					CharmCostsByID[key] = localSettings.notchCosts[i];
				}
			}
		}

		public override string GetVersion()
		{
			return "1.4.0";
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
				new MenuEntry(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values, "添加苍白宫廷系列护符至游戏。", toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader)
			};
		}

		public bool IsToggleable()
		{
			return true;
		}

		public void Toggle()
		{
			globalSettings.AddCharms = !globalSettings.AddCharms;
		}

		public void Unload()
		{
			Instance = null;
		}

		public PaleCourtCharms()
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Expected O, but got Unknown
			localSettings = new SaveModSettings();
			globalSettings = new GlobalSettings();
			((Mod)this)..ctor("苍白宫廷护符");
			if (ModHooks.GetMod("苍白宫廷", false, false) is Mod)
			{
				Log("[Warning] Detected FiveKnights (苍白宫廷), disabling mod. Note: you'll see a few more errors, that's normal. PCC_FIX_V2");
				return;
			}
			Instance = this;
			Log("PaleCourtCharms initialized.");
			LangStrings = new LanguageStrings(Assembly.GetExecutingAssembly(), "PaleCourtCharms.assets.Language.json", (Encoding)null);
			LoadEmbeddedSprites();
			Charms.Add(new CharmDefinition
			{
				InternalName = "MarkOfPurity",
				DisplayName = () => LangStrings.Get("CHARM_NAME_PURITY", "UI"),
				Description = () => LangStrings.Get("CHARM_DESC_PURITY", "UI"),
				ShopDesc = () => LangStrings.Get("CHARM_NAME_PURITY", "UI"),
				Icon = SPRITES["Mark_of_Purity"],
				NotchCost = CharmCosts[0]
			});
			Charms.Add(new CharmDefinition
			{
				InternalName = "VesselsLament",
				DisplayName = () => LangStrings.Get("CHARM_NAME_LAMENT", "UI"),
				Description = () => LangStrings.Get("CHARM_DESC_LAMENT", "UI"),
				ShopDesc = () => LangStrings.Get("CHARM_NAME_LAMENT", "UI"),
				Icon = SPRITES["Vessels_Lament"],
				NotchCost = CharmCosts[1]
			});
			Charms.Add(new CharmDefinition
			{
				InternalName = "BoonOfHallownest",
				DisplayName = () => LangStrings.Get("CHARM_NAME_BOON", "UI"),
				Description = () => LangStrings.Get("CHARM_DESC_BOON", "UI"),
				ShopDesc = () => LangStrings.Get("CHARM_NAME_BOON", "UI"),
				Icon = SPRITES["Boon_of_Hallownest"],
				NotchCost = CharmCosts[2]
			});
			Charms.Add(new CharmDefinition
			{
				InternalName = "AbyssalBloom",
				DisplayName = () => LangStrings.Get("CHARM_NAME_BLOOM", "UI"),
				Description = () => LangStrings.Get("CHARM_DESC_BLOOM", "UI"),
				ShopDesc = () => LangStrings.Get("CHARM_NAME_BLOOM", "UI"),
				Icon = SPRITES["Abyssal_Bloom"],
				NotchCost = CharmCosts[3]
			});
			CharmIDs = CharmHelper.AddSprites((Sprite[])(object)new Sprite[4]
			{
				SPRITES["Mark_of_Purity"],
				SPRITES["Vessels_Lament"],
				SPRITES["Boon_of_Hallownest"],
				SPRITES["Abyssal_Bloom"]
			});
			InitializeCharmCosts();
		}

		private void InitializeCharmCosts()
		{
			CharmCostsByID.Clear();
			for (int i = 0; i < CharmIDs.Count; i++)
			{
				CharmCostsByID[CharmIDs[i]] = Charms[i].NotchCost;
			}
		}

		private void LoadEmbeddedSprites()
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Expected O, but got Unknown
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string[] manifestResourceNames = executingAssembly.GetManifestResourceNames();
			foreach (string text in manifestResourceNames)
			{
				if (!text.EndsWith(".png"))
				{
					continue;
				}
				using Stream stream = executingAssembly.GetManifestResourceStream(text);
				if (stream != null)
				{
					byte[] array = new byte[stream.Length];
					stream.Read(array, 0, array.Length);
					Texture2D val = new Texture2D(1, 1);
					ImageConversion.LoadImage(val, array);
					string[] array2 = text.Split('.');
					string key = ((array2.Length >= 3) ? array2[array2.Length - 2] : Path.GetFileNameWithoutExtension(text));
					SPRITES[key] = Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), new Vector2(0.5f, 0.5f), 100f);
				}
			}
		}

		public override List<(string, string)> GetPreloadNames()
		{
			return new List<(string, string)>
			{
				("GG_Hollow_Knight", "Battle Scene/HK Prime"),
				("GG_Hollow_Knight", "Battle Scene/Focus Blasts/HK Prime Blast/Blast"),
				("Dream_Final_Boss", "Boss Control/Radiance/Death/Knight Split/Knight Ball"),
				("Tutorial_01", "_Props/Tut_tablet_top/Glows"),
				("Ruins1_23", "Mage"),
				("Dream_Final_Boss", "Boss Control/Radiance")
			};
		}

		public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Expected O, but got Unknown
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Expected O, but got Unknown
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Expected O, but got Unknown
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Expected O, but got Unknown
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Expected O, but got Unknown
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Expected O, but got Unknown
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Expected O, but got Unknown
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			if (Instance == null)
			{
				return;
			}
			ModHooks.AfterSavegameLoadHook += delegate
			{
				ICShiny.ResetPlacementsFlag();
			};
			object obj = <>O.<0>__HandleNewGame;
			if (obj == null)
			{
				hook_StartNewGame val = HandleNewGame;
				<>O.<0>__HandleNewGame = val;
				obj = (object)val;
			}
			UIManager.StartNewGame += (hook_StartNewGame)obj;
			ModHooks.LanguageGetHook += new LanguageGetProxy(LangGet);
			ModHooks.GetPlayerIntHook += new GetIntProxy(OnGetPlayerIntHook);
			ModHooks.GetPlayerBoolHook += new GetBoolProxy(ModHooks_GetPlayerBool);
			ModHooks.SetPlayerBoolHook += new SetBoolProxy(OnSetPlayerBool);
			GameManager.StartNewGame += new hook_StartNewGame(GameManager_StartNewGame);
			ModHooks.AfterSavegameLoadHook += OnAfterSave;
			HeroController.Awake += new hook_Awake(HeroController_Awake);
			PlayerData.CountCharms += new hook_CountCharms(CountPCC);
			preloadedGO["PV"] = preloadedObjects["GG_Hollow_Knight"]["Battle Scene/HK Prime"];
			preloadedGO["Blast"] = preloadedObjects["GG_Hollow_Knight"]["Battle Scene/Focus Blasts/HK Prime Blast/Blast"];
			preloadedGO["Knight Ball"] = preloadedObjects["Dream_Final_Boss"]["Boss Control/Radiance/Death/Knight Split/Knight Ball"];
			preloadedGO["Radiance"] = preloadedObjects["Dream_Final_Boss"]["Boss Control/Radiance"];
			preloadedGO["SoulTwister"] = preloadedObjects["Ruins1_23"]["Mage"];
			preloadedGO["SoulEffect"] = preloadedObjects["Tutorial_01"]["_Props/Tut_tablet_top/Glows"];
			ABManager.LoadAll();
			GameObject val2 = ABManager.LoadFromCharms<GameObject>("BloomAnim");
			if ((Object)(object)val2 != (Object)null)
			{
				preloadedGO["Bloom Anim Prefab"] = val2;
			}
			else
			{
				Log("Warning: BloomAnim not found in AssetBundle.");
			}
			GameObject val3 = ABManager.LoadFromCharms<GameObject>("AbyssalBloom");
			if ((Object)(object)val3 != (Object)null)
			{
				preloadedGO["Bloom Sprite Prefab"] = val3;
			}
			else
			{
				Log("Warning: AbyssalBloom not found in AssetBundle.");
			}
			Instance = this;
			if (ModHooks.GetMod("DebugMod", false, false) is Mod)
			{
				DebugModHook.GiveAllCharms(delegate
				{
					ToggleAllCharms(give: true);
					localSettings.upgradedCharm_10 = true;
					PlayerData.instance.SetBool("upgradedCharm_10", true);
					PlayerData.instance.CountCharms();
				});
				DebugModHook.RemoveAllCharms(delegate
				{
					ToggleAllCharms(give: false);
					localSettings.upgradedCharm_10 = false;
					PlayerData.instance.SetBool("upgradedCharm_10", false);
					PlayerData.instance.CountCharms();
				});
			}
			ICShiny.Hook();
			Finder.DefineCustomItem((AbstractItem)(object)new HonourUpgradeItem());
			if (ModHooks.GetMod("Randomizer 4", false, false) is Mod)
			{
				RandoManager.Hook();
				global::PaleCourtCharms.Rando.Interop.Setup(Instance.globalSettings, Instance);
			}
			if (ModHooks.GetMod("RandoSettingsManager", false, false) is Mod)
			{
				RSM_Interop.Hook();
			}
		}

		public static bool IsRandoSave()
		{
			if (ModHooks.GetMod("Randomizer 4", false, false) is Mod)
			{
				try
				{
					return RandoManager.IsRandoSave();
				}
				catch (Exception ex)
				{
					Instance?.Log("[IsRandoSave] RandoManager.IsRandoSave threw: " + ex.Message);
					return false;
				}
			}
			return false;
		}

		private static void HandleNewGame(orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
		{
			orig.Invoke(self, permaDeath, bossRush);
			if (!randoInitialized && IsRandoSave())
			{
				randoInitialized = true;
				Instance.StartGame();
			}
		}

		private void GameManager_StartNewGame(orig_StartNewGame orig, GameManager gm, bool perma, bool bossRush)
		{
			orig.Invoke(gm, perma, bossRush);
			if (bossRush)
			{
				int count = CharmIDs.Count;
				localSettings.gotCharms = new bool[count];
				localSettings.newCharms = new bool[count];
				localSettings.equippedCharms = new bool[count];
				for (int i = 0; i < count; i++)
				{
					localSettings.gotCharms[i] = (localSettings.newCharms[i] = true);
				}
				localSettings.upgradedCharm_10 = true;
			}
			StartGame();
		}

		public void StartGame()
		{
			GameObject gameObject = ((Component)GameManager.instance).gameObject;
			if ((Object)(object)gameObject.GetComponent<Amulets>() == (Object)null)
			{
				gameObject.AddComponent<Amulets>();
			}
		}

		private int OnGetPlayerIntHook(string target, int orig)
		{
			if (target.StartsWith("charmCost_") && int.TryParse(target.Split('_')[1], out var result))
			{
				int value;
				if (!IsRandoSave())
				{
					int num = CharmIDs.IndexOf(result);
					if (num >= 0 && num < CharmCosts.Length)
					{
						return CharmCosts[num];
					}
				}
				else if (CharmCostsByID.TryGetValue(result, out value))
				{
					return value;
				}
			}
			return orig;
		}

		private bool OnSetPlayerBool(string target, bool value)
		{
			if (CharmIDs.Count == 0)
			{
				return value;
			}
			string[] array = target.Split('_');
			if (array.Length == 2 && int.TryParse(array[1], out var result))
			{
				int num = CharmIDs.IndexOf(result);
				if (num >= 0)
				{
					switch (array[0])
					{
					case "equippedCharm":
						localSettings.equippedCharms[num] = value;
						return true;
					case "gotCharm":
						localSettings.gotCharms[num] = value;
						return true;
					case "newCharm":
						localSettings.newCharms[num] = value;
						return true;
					}
				}
			}
			return value;
		}

		private bool ModHooks_GetPlayerBool(string target, bool orig)
		{
			if (CharmIDs.Count == 0)
			{
				return orig;
			}
			string[] array = target.Split('_');
			if (array.Length == 2 && int.TryParse(array[1], out var result))
			{
				int num = CharmIDs.IndexOf(result);
				if (num >= 0)
				{
					return array[0] switch
					{
						"gotCharm" => Safe(num, localSettings.gotCharms), 
						"newCharm" => Safe(num, localSettings.newCharms), 
						"equippedCharm" => Safe(num, localSettings.equippedCharms), 
						_ => orig, 
					};
				}
			}
			return orig;
			bool Safe(int i, bool[] arr)
			{
				if (i < 0 || i >= arr.Length)
				{
					return orig;
				}
				return arr[i];
			}
		}

		private string LangGet(string key, string sheet, string orig)
		{
			if (sheet == "RANDO" && key.StartsWith("SHOP_DESCRIPTION_") && LangStrings.ContainsKey(key, sheet))
			{
				return LangStrings.Get(key, sheet);
			}
			if (sheet == "UI")
			{
				if (key == "CHARM_NAME_10")
				{
					if (!localSettings.upgradedCharm_10)
					{
						return orig;
					}
					return LangStrings.Get("CHARM_NAME_HONOUR", "UI");
				}
				if (key == "CHARM_DESC_10")
				{
					if (!localSettings.upgradedCharm_10)
					{
						return orig;
					}
					return LangStrings.Get("CHARM_DESC_HONOUR", "UI");
				}
			}
			if (LangStrings.ContainsKey(key, sheet))
			{
				return LangStrings.Get(key, sheet);
			}
			if (sheet == "UI" && (key.StartsWith("CHARM_NAME_") || key.StartsWith("CHARM_DESC_")))
			{
				string[] array = key.Split('_');
				if (array.Length == 3 && int.TryParse(array[2], out var result))
				{
					int num = CharmIDs.IndexOf(result);
					if (num >= 0)
					{
						if (!key.StartsWith("CHARM_NAME_"))
						{
							return Charms[num].Description();
						}
						return Charms[num].DisplayName();
					}
				}
			}
			return orig;
		}

		private static void ToggleAllCharms(bool give)
		{
			for (int i = 0; i < CharmIDs.Count; i++)
			{
				Instance.localSettings.gotCharms[i] = give;
				Instance.localSettings.newCharms[i] = give;
				Instance.localSettings.equippedCharms[i] = false;
			}
		}

		private void OnAfterSave(SaveGameData data)
		{
			if (CharmIDs.Count != 0 && localSettings.gotCharms != null && localSettings.gotCharms.Length == CharmIDs.Count)
			{
				for (int i = 0; i < CharmIDs.Count; i++)
				{
					int num = CharmIDs[i];
					PlayerData.instance.SetBool($"gotCharm_{num}", localSettings.gotCharms[i]);
					PlayerData.instance.SetBool($"newCharm_{num}", localSettings.newCharms[i]);
					PlayerData.instance.SetBool($"equippedCharm_{num}", localSettings.equippedCharms[i]);
				}
				if (localSettings.upgradedCharm_10)
				{
					PlayerData.instance.SetBool("upgradedCharm_10", true);
				}
			}
		}

		public void OnLoadGlobal(GlobalSettings s)
		{
			globalSettings = s;
		}

		public GlobalSettings OnSaveGlobal()
		{
			return globalSettings;
		}

		private void HeroController_Awake(orig_Awake orig, HeroController self)
		{
			ItemHandler.ModulesRegistered = false;
			orig.Invoke(self);
			if ((Object)(object)GameManager.instance != (Object)null && (Object)(object)((Component)GameManager.instance).gameObject.GetComponent<Amulets>() == (Object)null)
			{
				((Component)GameManager.instance).gameObject.AddComponent<Amulets>();
			}
		}

		private void CountPCC(orig_CountCharms orig, PlayerData self)
		{
			orig.Invoke(self);
			SaveModSettings saveModSettings = Instance?.localSettings;
			if (saveModSettings == null || saveModSettings.gotCharms == null)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < Charms.Count; i++)
			{
				if (saveModSettings.gotCharms[i])
				{
					num++;
				}
			}
			self.SetInt("charmsOwned", self.GetInt("charmsOwned") + num);
		}

		private void Log(object msg)
		{
			Logger.Log("[PaleCourtCharms] " + msg);
		}
	}
	public class CharmDefinition
	{
		public string InternalName;

		public int NotchCost;

		public Func<string> DisplayName;

		public Func<string> Description;

		public Func<string> ShopDesc;

		public Sprite Icon;
	}
	public class GlobalSettings
	{
		public bool AddCharms { get; set; }

		public bool RandomizeCosts { get; set; }
	}
}
namespace PaleCourtCharms.Interop
{
	internal static class RSM_Interop
	{
		public static void Hook()
		{
			RandoSettingsManagerMod.Instance.RegisterConnection((object)new PaleCourtSettingsProxy());
		}
	}
	internal class PaleCourtSettingsProxy : RandoSettingsProxy<RandoSettings, string>
	{
		public override string ModKey => ((Mod)PaleCourtCharms.Instance).GetName();

		public override VersioningPolicy<string> VersioningPolicy { get; } = (VersioningPolicy<string>)(object)new EqualityVersioningPolicy<string>(((Mod)PaleCourtCharms.Instance).GetVersion());

		public override bool TryProvideSettings(out RandoSettings settings)
		{
			settings = RandoSettings.FromGlobal(PaleCourtCharms.GlobalSettings);
			return settings.Enabled;
		}

		public override void ReceiveSettings(RandoSettings settings)
		{
			if (settings != null)
			{
				PaleCourtCharms.GlobalSettings.AddCharms = settings.Enabled;
				PaleCourtCharms.GlobalSettings.RandomizeCosts = settings.RandomizeCosts;
				ConnectionMenu.Instance.Apply(settings);
			}
			else
			{
				ConnectionMenu.Instance.Disable();
			}
		}
	}
}
namespace PaleCourtCharms.Rando
{
	public class ConnectionMenu
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static OnRandomizerMenuConstructionHandler <0>__ConstructMenu;

			public static TryGetMenuButtonHandler <1>__HandleButton;
		}

		private readonly SmallButton pageRootButton;

		private readonly MenuPage accessPage;

		private readonly MenuElementFactory<RandoSettings> topLevelFactory;

		internal static ConnectionMenu Instance { get; private set; }

		public static void Hook()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			object obj = <>O.<0>__ConstructMenu;
			if (obj == null)
			{
				OnRandomizerMenuConstructionHandler val = ConstructMenu;
				<>O.<0>__ConstructMenu = val;
				obj = (object)val;
			}
			object obj2 = <>O.<1>__HandleButton;
			if (obj2 == null)
			{
				TryGetMenuButtonHandler val2 = HandleButton;
				<>O.<1>__HandleButton = val2;
				obj2 = (object)val2;
			}
			RandomizerMenuAPI.AddMenuPage((OnRandomizerMenuConstructionHandler)obj, (TryGetMenuButtonHandler)obj2);
			MenuChangerMod.OnExitMainMenu += delegate
			{
				Instance = null;
			};
		}

		private static bool HandleButton(MenuPage landingPage, out SmallButton button)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			button = Instance.pageRootButton;
			bool addCharms = PaleCourtCharms.GlobalSettings.AddCharms;
			((Graphic)button.Text).color = (addCharms ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR);
			return true;
		}

		private static void ConstructMenu(MenuPage landingPage)
		{
			Instance = new ConnectionMenu(landingPage);
		}

		private ConnectionMenu(MenuPage landingPage)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Expected O, but got Unknown
			base..ctor();
			accessPage = new MenuPage("苍白宫廷护符", landingPage);
			topLevelFactory = new MenuElementFactory<RandoSettings>(accessPage, RandoMenuProxy.RS);
			MenuPage obj = accessPage;
			Vector2 val = new Vector2(0f, 200f);
			IMenuElement[] elements = (IMenuElement[])(object)topLevelFactory.Elements;
			new VerticalItemPanel(obj, val, 48f, true, elements);
			pageRootButton = new SmallButton(landingPage, "苍白宫廷护符");
			MenuElementExtensions.AddHideAndShowEvent((BaseButton)(object)pageRootButton, landingPage, accessPage);
			landingPage.BeforeShow += delegate
			{
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				bool flag = RandoMenuProxy.RS.Enabled && RandoMenuProxy.RS.RandomizeCosts;
				((Graphic)pageRootButton.Text).color = (flag ? Colors.TRUE_COLOR : Colors.FALSE_COLOR);
			};
			accessPage.BeforeHide += delegate
			{
				RandoSettings randoSettings = (RandoMenuProxy.RS = new RandoSettings
				{
					Enabled = (bool)topLevelFactory.ElementLookup["Enabled"].Value,
					RandomizeCosts = (bool)topLevelFactory.ElementLookup["RandomizeCosts"].Value
				});
				topLevelFactory.SetMenuValues(randoSettings);
				PaleCourtCharms.GlobalSettings.AddCharms = randoSettings.Enabled;
				PaleCourtCharms.GlobalSettings.RandomizeCosts = randoSettings.RandomizeCosts;
			};
		}

		public void Apply(RandoSettings settings)
		{
			topLevelFactory.SetMenuValues(settings);
		}

		public void Disable()
		{
			topLevelFactory.ElementLookup["Enabled"].SetValue((object)false);
		}
	}
	internal static class Interop
	{
		public static void Setup(GlobalSettings gs, PaleCourtCharms main)
		{
			LogicHandler.Hook();
			ItemHandler.Hook();
			ConnectionMenu.Hook();
			Events.AfterStartNewGame += delegate
			{
				PaleCourtCharms.Instance.StartGame();
			};
		}
	}
	internal static class ItemHandler
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class <>c
		{
			public static readonly <>c <>9 = new <>c();

			public static RequestBuilderUpdateHandler <>9__7_0;

			public static RequestBuilderUpdateHandler <>9__7_1;

			public static RequestBuilderUpdateHandler <>9__7_2;

			public static Action<GenerationSettings, LogicManagerBuilder> <>9__7_3;

			public static RequestBuilderUpdateHandler <>9__7_4;

			public static RequestBuilderUpdateHandler <>9__7_5;

			public static Predicate<Tag> <>9__7_7;

			public static Action<RandoController> <>9__7_6;

			public static Func<ICFactory, RandoPlacement, AbstractItem> <>9__9_4;

			internal void <Hook>b__7_0(RequestBuilder rb)
			{
				int tickCount = Environment.TickCount;
				try
				{
					if (PaleCourtCharms.GlobalSettings.AddCharms)
					{
						AddToPool(rb);
					}
				}
				catch (Exception arg)
				{
					Logger.LogError($"[PaleCourtCharms] AddToPool exception (op {tickCount}): {arg}");
					throw;
				}
			}

			internal void <Hook>b__7_1(RequestBuilder rb)
			{
				int tickCount = Environment.TickCount;
				try
				{
					if (RandomizerMod.RS != null && PaleCourtCharms.GlobalSettings.RandomizeCosts)
					{
						ScaleNotchBudget(rb);
					}
				}
				catch (Exception arg)
				{
					Logger.LogError($"[PaleCourtCharms] ScaleNotchBudget exception (op {tickCount}): {arg}");
					throw;
				}
			}

			internal void <Hook>b__7_2(RequestBuilder rb)
			{
				int tickCount = Environment.TickCount;
				try
				{
					if (RandomizerMod.RS != null && PaleCourtCharms.GlobalSettings.RandomizeCosts)
					{
						RandomizeNotchCosts(rb);
					}
				}
				catch (Exception arg)
				{
					Logger.LogError($"[PaleCourtCharms] RandomizeNotchCosts exception (op {tickCount}): {arg}");
					throw;
				}
			}

			internal void <Hook>b__7_3(GenerationSettings gs, LogicManagerBuilder lmb)
			{
				int tickCount = Environment.TickCount;
				try
				{
					if (PaleCourtCharms.GlobalSettings.AddCharms)
					{
						InjectLogic(gs, lmb);
					}
				}
				catch (Exception arg)
				{
					Logger.LogError($"[PaleCourtCharms] InjectLogic exception (op {tickCount}): {arg}");
					throw;
				}
			}

			internal void <Hook>b__7_4(RequestBuilder rb)
			{
				try
				{
					if (_savedVanillaNotchCosts == null && rb?.ctx?.notchCosts != null)
					{
						_savedVanillaNotchCosts = rb.ctx.notchCosts.ToArray();
						Logger.Log("[PaleCourtCharms] Saved vanilla notch costs.");
					}
				}
				catch (Exception arg)
				{
					Logger.LogError($"[PaleCourtCharms] Error saving vanilla notch costs: {arg}");
				}
			}

			internal void <Hook>b__7_5(RequestBuilder rb)
			{
				try
				{
					if (RandomizerMod.RS?.GenerationSettings?.MiscSettings?.RandomizeNotchCosts != true || PaleCourtCharms.GlobalSettings.RandomizeCosts)
					{
						return;
					}
					List<int> list = new List<int>();
					int num = PaleCourtCharms.CharmKeys.Length;
					for (int i = 0; i < num; i++)
					{
						list.Add((i < PaleCourtCharms.CharmCosts.Length) ? PaleCourtCharms.CharmCosts[i] : 0);
					}
					try
					{
						List<int> notchCosts = rb.ctx.notchCosts;
						if (notchCosts != null)
						{
							notchCosts.Clear();
							notchCosts.AddRange(list);
						}
						else
						{
							rb.ctx.notchCosts = new List<int>(list);
						}
					}
					catch (Exception arg)
					{
						Logger.LogError($"[PaleCourtCharms] Could not write rb.ctx.notchCosts: {arg}");
					}
					PaleCourtCharms.Settings.notchCosts = new List<int>(list);
					int num2 = Math.Min(PaleCourtCharms.CharmIDs.Count, list.Count);
					for (int j = 0; j < num2; j++)
					{
						int key = PaleCourtCharms.CharmIDs[j];
						PaleCourtCharms.CharmCostsByID[key] = list[j];
					}
					Logger.Log("[PaleCourtCharms] Forced default notch costs because Randomizer did costs but user opted out.");
				}
				catch (Exception arg2)
				{
					Logger.LogError($"[PaleCourtCharms] Error forcing default notch costs: {arg2}");
				}
			}

			internal void <Hook>b__7_6(RandoController _)
			{
				_savedVanillaNotchCosts = null;
				lock (_defineLock)
				{
					try
					{
						DefineObjects();
						Interlocked.Exchange(ref _objectsDefined, 1);
					}
					catch (Exception arg)
					{
						Logger.LogError($"[PaleCourtCharms] Exception in DefineObjects during OnExportCompleted: {arg}");
					}
				}
				bool valueOrDefault = RandomizerMod.RS?.GenerationSettings?.MiscSettings?.RandomizeNotchCosts == true;
				if (PaleCourtCharms.GlobalSettings.RandomizeCosts && !valueOrDefault && !ModulesRegistered)
				{
					try
					{
						ItemChangerMod.Modules.Add<NotchCostUI>();
						ItemChangerMod.Modules.Add<ZeroCostCharmEquip>();
						ModulesRegistered = true;
					}
					catch (Exception arg2)
					{
						Logger.LogError($"[PaleCourtCharms] Failed to register modules: {arg2}");
					}
				}
				try
				{
					AbstractItem itemInternal = Finder.GetItemInternal("Defender's_Crest");
					if (((TaggableObject)(itemInternal?)).tags != null)
					{
						int num = ((TaggableObject)itemInternal).tags.RemoveAll(delegate(Tag t)
						{
							ItemChainTag val = (ItemChainTag)(object)((t is ItemChainTag) ? t : null);
							return val != null && val.successor == "Kings_Honour";
						});
						if (num > 0)
						{
							Logger.Log($"[PaleCourtCharms] Cleaned up {num} leftover Crest chain tag(s).");
						}
					}
				}
				catch (Exception arg3)
				{
					Logger.LogError($"[PaleCourtCharms] Exception during crest cleanup: {arg3}");
				}
			}

			internal bool <Hook>b__7_7(Tag t)
			{
				ItemChainTag val = (ItemChainTag)(object)((t is ItemChainTag) ? t : null);
				if (val != null)
				{
					return val.successor == "Kings_Honour";
				}
				return false;
			}

			internal AbstractItem <AddToPool>b__9_4(ICFactory factory, RandoPlacement placement)
			{
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Expected O, but got Unknown
				AbstractItem val;
				AbstractItem obj = (val = factory.MakeItem("Kings_Honour"));
				if (((TaggableObject)val).tags == null)
				{
					((TaggableObject)val).tags = new List<Tag>();
				}
				((TaggableObject)obj).tags.Add((Tag)new ItemChainTag
				{
					predecessor = "Defender's_Crest",
					successor = "Kings_Honour"
				});
				return obj;
			}
		}

		private const string HonourKey = "Kings_Honour";

		public static bool ModulesRegistered = false;

		private static int _objectsDefined = 0;

		private static int _addToPoolRunning = 0;

		private static int _subscribed = 0;

		private static readonly object _defineLock = new object();

		private static int[] _savedVanillaNotchCosts = null;

		public static void Hook()
		{
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Expected O, but got Unknown
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Expected O, but got Unknown
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Expected O, but got Unknown
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Expected O, but got Unknown
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Expected O, but got Unknown
			if (Interlocked.CompareExchange(ref _objectsDefined, 1, 0) == 0)
			{
				DefineObjects();
			}
			if (Interlocked.CompareExchange(ref _subscribed, 1, 0) == 0)
			{
				PriorityEvent<RequestBuilderUpdateHandler> onUpdate = RequestBuilder.OnUpdate;
				object obj = <>c.<>9__7_0;
				if (obj == null)
				{
					RequestBuilderUpdateHandler val = delegate(RequestBuilder rb)
					{
						int tickCount = Environment.TickCount;
						try
						{
							if (PaleCourtCharms.GlobalSettings.AddCharms)
							{
								AddToPool(rb);
							}
						}
						catch (Exception arg)
						{
							Logger.LogError($"[PaleCourtCharms] AddToPool exception (op {tickCount}): {arg}");
							throw;
						}
					};
					<>c.<>9__7_0 = val;
					obj = (object)val;
				}
				onUpdate.Subscribe(0f, (RequestBuilderUpdateHandler)obj);
				PriorityEvent<RequestBuilderUpdateHandler> onUpdate2 = RequestBuilder.OnUpdate;
				object obj2 = <>c.<>9__7_1;
				if (obj2 == null)
				{
					RequestBuilderUpdateHandler val2 = delegate(RequestBuilder rb)
					{
						int tickCount = Environment.TickCount;
						try
						{
							if (RandomizerMod.RS != null && PaleCourtCharms.GlobalSettings.RandomizeCosts)
							{
								ScaleNotchBudget(rb);
							}
						}
						catch (Exception arg)
						{
							Logger.LogError($"[PaleCourtCharms] ScaleNotchBudget exception (op {tickCount}): {arg}");
							throw;
						}
					};
					<>c.<>9__7_1 = val2;
					obj2 = (object)val2;
				}
				onUpdate2.Subscribe(1f, (RequestBuilderUpdateHandler)obj2);
				PriorityEvent<RequestBuilderUpdateHandler> onUpdate3 = RequestBuilder.OnUpdate;
				object obj3 = <>c.<>9__7_2;
				if (obj3 == null)
				{
					RequestBuilderUpdateHandler val3 = delegate(RequestBuilder rb)
					{
						int tickCount = Environment.TickCount;
						try
						{
							if (RandomizerMod.RS != null && PaleCourtCharms.GlobalSettings.RandomizeCosts)
							{
								RandomizeNotchCosts(rb);
							}
						}
						catch (Exception arg)
						{
							Logger.LogError($"[PaleCourtCharms] RandomizeNotchCosts exception (op {tickCount}): {arg}");
							throw;
						}
					};
					<>c.<>9__7_2 = val3;
					obj3 = (object)val3;
				}
				onUpdate3.Subscribe(50f, (RequestBuilderUpdateHandler)obj3);
				RCData.RuntimeLogicOverride.Subscribe(50f, (Action<GenerationSettings, LogicManagerBuilder>)delegate(GenerationSettings gs, LogicManagerBuilder lmb)
				{
					int tickCount = Environment.TickCount;
					try
					{
						if (PaleCourtCharms.GlobalSettings.AddCharms)
						{
							InjectLogic(gs, lmb);
						}
					}
					catch (Exception arg)
					{
						Logger.LogError($"[PaleCourtCharms] InjectLogic exception (op {tickCount}): {arg}");
						throw;
					}
				});
			}
			PriorityEvent<RequestBuilderUpdateHandler> onUpdate4 = RequestBuilder.OnUpdate;
			object obj4 = <>c.<>9__7_4;
			if (obj4 == null)
			{
				RequestBuilderUpdateHandler val4 = delegate(RequestBuilder rb)
				{
					try
					{
						if (_savedVanillaNotchCosts == null && rb?.ctx?.notchCosts != null)
						{
							_savedVanillaNotchCosts = rb.ctx.notchCosts.ToArray();
							Logger.Log("[PaleCourtCharms] Saved vanilla notch costs.");
						}
					}
					catch (Exception arg)
					{
						Logger.LogError($"[PaleCourtCharms] Error saving vanilla notch costs: {arg}");
					}
				};
				<>c.<>9__7_4 = val4;
				obj4 = (object)val4;
			}
			onUpdate4.Subscribe(-100f, (RequestBuilderUpdateHandler)obj4);
			PriorityEvent<RequestBuilderUpdateHandler> onUpdate5 = RequestBuilder.OnUpdate;
			object obj5 = <>c.<>9__7_5;
			if (obj5 == null)
			{
				RequestBuilderUpdateHandler val5 = delegate(RequestBuilder rb)
				{
					try
					{
						if (RandomizerMod.RS?.GenerationSettings?.MiscSettings?.RandomizeNotchCosts == true && !PaleCourtCharms.GlobalSettings.RandomizeCosts)
						{
							List<int> list = new List<int>();
							int num = PaleCourtCharms.CharmKeys.Length;
							for (int i = 0; i < num; i++)
							{
								list.Add((i < PaleCourtCharms.CharmCosts.Length) ? PaleCourtCharms.CharmCosts[i] : 0);
							}
							try
							{
								List<int> notchCosts = rb.ctx.notchCosts;
								if (notchCosts != null)
								{
									notchCosts.Clear();
									notchCosts.AddRange(list);
								}
								else
								{
									rb.ctx.notchCosts = new List<int>(list);
								}
							}
							catch (Exception arg)
							{
								Logger.LogError($"[PaleCourtCharms] Could not write rb.ctx.notchCosts: {arg}");
							}
							PaleCourtCharms.Settings.notchCosts = new List<int>(list);
							int num2 = Math.Min(PaleCourtCharms.CharmIDs.Count, list.Count);
							for (int j = 0; j < num2; j++)
							{
								int key = PaleCourtCharms.CharmIDs[j];
								PaleCourtCharms.CharmCostsByID[key] = list[j];
							}
							Logger.Log("[PaleCourtCharms] Forced default notch costs because Randomizer did costs but user opted out.");
						}
					}
					catch (Exception arg2)
					{
						Logger.LogError($"[PaleCourtCharms] Error forcing default notch costs: {arg2}");
					}
				};
				<>c.<>9__7_5 = val5;
				obj5 = (object)val5;
			}
			onUpdate5.Subscribe(100f, (RequestBuilderUpdateHandler)obj5);
			RandoController.OnExportCompleted += delegate
			{
				_savedVanillaNotchCosts = null;
				lock (_defineLock)
				{
					try
					{
						DefineObjects();
						Interlocked.Exchange(ref _objectsDefined, 1);
					}
					catch (Exception arg)
					{
						Logger.LogError($"[PaleCourtCharms] Exception in DefineObjects during OnExportCompleted: {arg}");
					}
				}
				bool valueOrDefault = RandomizerMod.RS?.GenerationSettings?.MiscSettings?.RandomizeNotchCosts == true;
				if (PaleCourtCharms.GlobalSettings.RandomizeCosts && !valueOrDefault && !ModulesRegistered)
				{
					try
					{
						ItemChangerMod.Modules.Add<NotchCostUI>();
						ItemChangerMod.Modules.Add<ZeroCostCharmEquip>();
						ModulesRegistered = true;
					}
					catch (Exception arg2)
					{
						Logger.LogError($"[PaleCourtCharms] Failed to register modules: {arg2}");
					}
				}
				try
				{
					AbstractItem itemInternal = Finder.GetItemInternal("Defender's_Crest");
					if (((TaggableObject)(itemInternal?)).tags != null)
					{
						int num = ((TaggableObject)itemInternal).tags.RemoveAll(delegate(Tag t)
						{
							ItemChainTag val6 = (ItemChainTag)(object)((t is ItemChainTag) ? t : null);
							return val6 != null && val6.successor == "Kings_Honour";
						});
						if (num > 0)
						{
							Logger.Log($"[PaleCourtCharms] Cleaned up {num} leftover Crest chain tag(s).");
						}
					}
				}
				catch (Exception arg3)
				{
					Logger.LogError($"[PaleCourtCharms] Exception during crest cleanup: {arg3}");
				}
			};
		}

		private static void DefineObjects()
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Expected O, but got Unknown
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Expected O, but got Unknown
			lock (_defineLock)
			{
				try
				{
					for (int i = 0; i < PaleCourtCharms.CharmKeys.Length; i++)
					{
						string text = PaleCourtCharms.CharmKeys[i];
						if (Finder.GetItemInternal(text) == null)
						{
							Finder.DefineCustomItem((AbstractItem)(object)new PC_CharmItem(i));
						}
						if (Finder.GetLocationInternal(text) == null)
						{
							CoordinateLocation val = new CoordinateLocation
							{
								name = text,
								sceneName = ICShiny.CharmScenes[i],
								x = ICShiny.CharmPositions[i].x,
								y = ICShiny.CharmPositions[i].y,
								elevation = 0f
							};
							InteropTag val2 = ((TaggableObject)val).AddTag<InteropTag>();
							val2.Message = "RandoSupplementalMetadata";
							val2.Properties["ModSource"] = ((Mod)PaleCourtCharms.Instance).GetName();
							val2.Properties["PoolGroup"] = "Charms";
							val2.Properties["VanillaItem"] = text;
							if (text == "Boon_of_Hallownest")
							{
								val2.Properties["MapLocations"] = new(string, float, float)[1] { ("Fungus2_21", 122.77f, 12.15f) };
								val2.Properties["HighlightScenes"] = new string[1] { "Fungus2_21" };
								val2.Properties["WorldMapLocation"] = ("Fungus2_21", 122.77f, 12.15f);
							}
							Finder.DefineCustomLocation((AbstractLocation)(object)val);
						}
					}
					if (Finder.GetItemInternal("Kings_Honour") == null)
					{
						Finder.DefineCustomItem((AbstractItem)(object)new HonourUpgradeItem());
					}
					if (Finder.GetLocationInternal("Kings_Honour") == null)
					{
						CoordinateLocation val3 = new CoordinateLocation
						{
							name = "Kings_Honour",
							sceneName = ICShiny.HonourScene,
							x = ICShiny.HonourPos.x,
							y = ICShiny.HonourPos.y,
							elevation = 0f
						};
						InteropTag obj = ((TaggableObject)val3).AddTag<InteropTag>();
						obj.Message = "RandoSupplementalMetadata";
						obj.Properties["ModSource"] = ((Mod)PaleCourtCharms.Instance).GetName();
						obj.Properties["PoolGroup"] = "Charms";
						obj.Properties["VanillaItem"] = "Kings_Honour";
						Finder.DefineCustomLocation((AbstractLocation)val3);
					}
				}
				catch (Exception arg)
				{
					Logger.LogError($"[PaleCourtCharms] DefineObjects threw: {arg}");
					throw;
				}
			}
		}

		private static void AddToPool(RequestBuilder rb)
		{
			if (Interlocked.CompareExchange(ref _addToPoolRunning, 1, 0) == 1)
			{
				return;
			}
			try
			{
				int tickCount = Environment.TickCount;
				Logger.Log($"[PaleCourtCharms] AddToPool start (op {tickCount})");
				foreach (string key in PaleCourtCharms.CharmKeys.Concat(new string[1] { "Kings_Honour" }))
				{
					try
					{
						rb.RemoveLocationByName(key);
						rb.AddLocationByName(key);
						rb.EditLocationRequest(key, (Action<LocationRequestInfo>)delegate(LocationRequestInfo info)
						{
							info.getLocationDef = delegate
							{
								//IL_0000: Unknown result type (might be due to invalid IL or missing references)
								//IL_0005: Unknown result type (might be due to invalid IL or missing references)
								//IL_0011: Unknown result type (might be due to invalid IL or missing references)
								//IL_0022: Unknown result type (might be due to invalid IL or missing references)
								//IL_0029: Unknown result type (might be due to invalid IL or missing references)
								//IL_0031: Expected O, but got Unknown
								LocationDef val = new LocationDef();
								val.set_Name(key);
								val.set_SceneName(GetSceneName(key));
								val.set_FlexibleCount(false);
								val.set_AdditionalProgressionPenalty(false);
								return val;
							};
						});
						rb.AddItemByName(key);
						rb.EditItemRequest(key, (Action<ItemRequestInfo>)delegate(ItemRequestInfo info)
						{
							info.getItemDef = delegate
							{
								//IL_0000: Unknown result type (might be due to invalid IL or missing references)
								//IL_0005: Unknown result type (might be due to invalid IL or missing references)
								//IL_0011: Unknown result type (might be due to invalid IL or missing references)
								//IL_001c: Unknown result type (might be due to invalid IL or missing references)
								//IL_0023: Unknown result type (might be due to invalid IL or missing references)
								//IL_002f: Expected O, but got Unknown
								ItemDef val = new ItemDef();
								val.set_Name(key);
								val.set_Pool("Charm");
								val.set_MajorItem(false);
								val.set_PriceCap(2000);
								return val;
							};
							if (key == "Kings_Honour" && rb.gs.PoolSettings.Charms)
							{
								info.realItemCreator = delegate(ICFactory factory, RandoPlacement placement)
								{
									//IL_0026: Unknown result type (might be due to invalid IL or missing references)
									//IL_002b: Unknown result type (might be due to invalid IL or missing references)
									//IL_0036: Unknown result type (might be due to invalid IL or missing references)
									//IL_0046: Expected O, but got Unknown
									AbstractItem val;
									AbstractItem obj = (val = factory.MakeItem("Kings_Honour"));
									if (((TaggableObject)val).tags == null)
									{
										((TaggableObject)val).tags = new List<Tag>();
									}
									((TaggableObject)obj).tags.Add((Tag)new ItemChainTag
									{
										predecessor = "Defender's_Crest",
										successor = "Kings_Honour"
									});
									return obj;
								};
							}
						});
					}
					catch (Exception arg)
					{
						Logger.LogError($"[PaleCourtCharms] AddToPool: failed handling key {key}: {arg}");
						throw;
					}
				}
				Logger.Log($"[PaleCourtCharms] AddToPool completed (op {tickCount})");
			}
			catch (ThreadAbortException)
			{
				Logger.LogWarn("[PaleCourtCharms] AddToPool aborted by thread (ThreadAbortException).");
				throw;
			}
			catch (Exception arg2)
			{
				Logger.LogError($"[PaleCourtCharms] AddToPool threw: {arg2}");
				throw;
			}
			finally
			{
				Interlocked.Exchange(ref _addToPoolRunning, 0);
				Logger.Log("[PaleCourtCharms] AddToPool cleaned up running flag.");
			}
		}

		private static void ScaleNotchBudget(RequestBuilder rb)
		{
			MiscSettings miscSettings = rb.gs.MiscSettings;
			miscSettings.MinRandomNotchTotal += 14;
			MiscSettings miscSettings2 = rb.gs.MiscSettings;
			miscSettings2.MaxRandomNotchTotal += 14;
		}

		private static void RandomizeNotchCosts(RequestBuilder rb)
		{
			Random rng = rb.rng;
			int num = rb.ctx.notchCosts.Sum();
			int minRandomNotchTotal = rb.gs.MiscSettings.MinRandomNotchTotal;
			int num2 = rb.gs.MiscSettings.MaxRandomNotchTotal - minRandomNotchTotal;
			int num3 = PaleCourtCharms.CharmKeys.Length;
			int minValue = Math.Max(0, (num - num2) / 10);
			int num4 = Math.Min(num3 * 6, (num + num2) / 10);
			int num5 = rng.Next(minValue, num4 + 1);
			int[] array = new int[num3];
			for (int i = 0; i < num5; i++)
			{
				int num6;
				do
				{
					num6 = rng.Next(num3);
				}
				while (array[num6] >= 6);
				array[num6]++;
			}
			PaleCourtCharms.Settings.notchCosts = array.ToList();
			for (int j = 0; j < num3; j++)
			{
				PaleCourtCharms.CharmCostsByID[PaleCourtCharms.CharmIDs[j]] = array[j];
			}
		}

		private static string GetSceneName(string key)
		{
			if (key == "Kings_Honour")
			{
				return ICShiny.HonourScene;
			}
			int num = Array.IndexOf(PaleCourtCharms.CharmKeys, key);
			if (num < 0)
			{
				return null;
			}
			return ICShiny.CharmScenes[num];
		}

		private static void InjectLogic(GenerationSettings gs, LogicManagerBuilder lmb)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Expected O, but got Unknown
			try
			{
				string[] charmKeys = PaleCourtCharms.CharmKeys;
				foreach (string text in charmKeys)
				{
					lmb.AddItem((LogicItem)new SingleItem(text, new TermValue(lmb.GetOrAddTerm(text), 1)));
				}
				lmb.AddItem((LogicItem)new SingleItem("Kings_Honour", new TermValue(lmb.GetOrAddTerm("Kings_Honour"), 1)));
			}
			catch (Exception arg)
			{
				Logger.LogError($"[PaleCourtCharms] InjectLogic threw: {arg}");
				throw;
			}
		}
	}
	internal static class LogicHandler
	{
		internal static void Hook()
		{
			RCData.RuntimeLogicOverride.Subscribe(0f, (Action<GenerationSettings, LogicManagerBuilder>)ApplyLogic);
			SettingsLog.AfterLogSettings += AddConnectionSettings;
		}

		private static void ApplyLogic(GenerationSettings gs, LogicManagerBuilder lmb)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			if (!PaleCourtCharms.GlobalSettings.AddCharms)
			{
				return;
			}
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			JsonLogicFormat val = new JsonLogicFormat();
			using (Stream stream = executingAssembly.GetManifestResourceStream("PaleCourtCharms.Rando.Terms.json"))
			{
				lmb.DeserializeFile((LogicFileType)0, (ILogicFormat)(object)val, stream);
			}
			using (Stream stream2 = executingAssembly.GetManifestResourceStream("PaleCourtCharms.Rando.Locations.json"))
			{
				lmb.DeserializeFile((LogicFileType)5, (ILogicFormat)(object)val, stream2);
			}
			using Stream stream3 = executingAssembly.GetManifestResourceStream("PaleCourtCharms.Rando.Items.json");
			lmb.DeserializeFile((LogicFileType)11, (ILogicFormat)(object)val, stream3);
		}

		private static void AddConnectionSettings(LogArguments args, TextWriter tw)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			if (!PaleCourtCharms.GlobalSettings.AddCharms && !PaleCourtCharms.GlobalSettings.RandomizeCosts)
			{
				return;
			}
			tw.WriteLine("Logging Pale Court Charms Settings:");
			JsonTextWriter val = new JsonTextWriter(tw)
			{
				CloseOutput = false
			};
			try
			{
				JsonUtil._js.Serialize((JsonWriter)(object)val, (object)PaleCourtCharms.GlobalSettings);
				tw.WriteLine();
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}
	public class LogicSettings
	{
		public bool Purity;

		public bool Lament;

		public bool Boon;

		public bool Bloom;

		[MenuLabel("King's Honour Logic")]
		public bool KingsHonour;

		public bool AnyEnabled()
		{
			if (!Purity && !Lament && !Boon && !Bloom)
			{
				return KingsHonour;
			}
			return true;
		}
	}
	public class PC_CharmItem : AbstractItem
	{
		public int CharmIndex;

		public override void GiveImmediate(GiveInfo info)
		{
			if (CharmIndex < 0 || CharmIndex >= PaleCourtCharms.CharmIDs.Count)
			{
				((Loggable)PaleCourtCharms.Instance).Log($"[PC_CharmItem] Invalid CharmIndex {CharmIndex}.");
				return;
			}
			int num = PaleCourtCharms.CharmIDs[CharmIndex];
			PlayerData.instance.SetBool($"gotCharm_{num}", true);
			PlayerData.instance.SetBool($"newCharm_{num}", true);
			PlayerData.instance.SetBool("equippedCharm_" + num, false);
		}

		public PC_CharmItem(int idx)
		{
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Expected O, but got Unknown
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Expected O, but got Unknown
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Expected O, but got Unknown
			((AbstractItem)this)..ctor();
			CharmIndex = idx;
			if (idx < 0 || idx >= PaleCourtCharms.CharmKeys.Length)
			{
				throw new ArgumentOutOfRangeException("idx", $"Charm index {idx} is out of range.");
			}
			string key = (base.name = PaleCourtCharms.CharmKeys[idx]);
			if (idx >= PaleCourtCharms.CharmIDs.Count)
			{
				throw new InvalidOperationException($"CharmIDs not yet initialized or missing index {idx}");
			}
			int num = PaleCourtCharms.CharmIDs[idx];
			base.UIDef = (UIDef)new MsgUIDef
			{
				name = (IString)new LanguageString("UI", $"CHARM_NAME_{num}"),
				shopDesc = (IString)new LanguageString("RANDO", "SHOP_DESCRIPTION_" + PaleCourtCharms.Charms[CharmIndex].InternalName),
				sprite = (ISprite)(object)new ICShiny.EmbeddedSprite
				{
					key = key
				}
			};
			InteropTag obj = ((TaggableObject)this).AddTag<InteropTag>();
			obj.Message = "RandoSupplementalMetadata";
			obj.Properties["ModSource"] = ((Mod)PaleCourtCharms.Instance).GetName();
			obj.Properties["PoolGroup"] = "Charms";
		}
	}
	public class PC_CharmLocation : CoordinateLocation
	{
		public PC_CharmLocation(string internalName, string scene, float x, float y)
		{
			((AbstractLocation)this).name = internalName;
			((AbstractLocation)this).sceneName = scene;
			base.x = x;
			base.y = y;
			base.elevation = 0f;
			InteropTag obj = ((TaggableObject)this).AddTag<InteropTag>();
			obj.Message = "RandoSupplementalMetadata";
			obj.Properties["ModSource"] = ((Mod)PaleCourtCharms.Instance).GetName();
			obj.Properties["PoolGroup"] = "Charms";
			obj.Properties["VanillaItem"] = internalName;
		}
	}
	internal static class RandoManager
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static hook_StartNewGame <0>__HandleNewGame;
		}

		public static void Hook()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			ItemHandler.Hook();
			object obj = <>O.<0>__HandleNewGame;
			if (obj == null)
			{
				hook_StartNewGame val = HandleNewGame;
				<>O.<0>__HandleNewGame = val;
				obj = (object)val;
			}
			UIManager.StartNewGame += (hook_StartNewGame)obj;
		}

		public static bool IsRandoSave()
		{
			try
			{
				return ItemChangerMod.Modules.Get<RandomizerModule>() != null;
			}
			catch (NullReferenceException)
			{
				return false;
			}
		}

		private static void HandleNewGame(orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
		{
			orig.Invoke(self, permaDeath, bossRush);
			if (ModHooks.GetMod("Randomizer 4", false, false) is Mod && RandomizerMod.IsRandoSave)
			{
				PaleCourtCharms.Instance.StartGame();
			}
		}
	}
	internal static class RandoMenuProxy
	{
		public static RandoSettings RS
		{
			get
			{
				return new RandoSettings
				{
					Enabled = PaleCourtCharms.GlobalSettings.AddCharms,
					RandomizeCosts = PaleCourtCharms.GlobalSettings.RandomizeCosts
				};
			}
			set
			{
				PaleCourtCharms.GlobalSettings.AddCharms = value.Enabled;
				PaleCourtCharms.GlobalSettings.RandomizeCosts = value.RandomizeCosts;
			}
		}
	}
	public class RandoSettings
	{
		public bool Enabled { get; set; }

		public bool RandomizeCosts { get; set; }

		public static RandoSettings FromGlobal(GlobalSettings g)
		{
			return new RandoSettings
			{
				Enabled = g.AddCharms,
				RandomizeCosts = g.RandomizeCosts
			};
		}

		public void ApplyTo(SaveModSettings s)
		{
			if (!Enabled)
			{
				s.EnabledCharms.Clear();
			}
			PaleCourtCharms.GlobalSettings.RandomizeCosts = RandomizeCosts;
		}

		public bool IsEnabled()
		{
			return Enabled;
		}
	}
}
