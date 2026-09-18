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
using DanielSteginkUtils.Components;
using DanielSteginkUtils.Components.Dung;
using DanielSteginkUtils.Helpers.Charms.Dung;
using DanielSteginkUtils.Utilities;
using GlobalEnums;
using HKMirror.Reflection.SingletonClasses;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Items;
using ItemChanger.Tags;
using ItemChanger.UIDefs;
using Language;
using Microsoft.CodeAnalysis;
using Modding;
using Modding.Delegates;
using Modding.Utils;
using On;
using On.HutongGames.PlayMaker;
using On.HutongGames.PlayMaker.Actions;
using SFCore;
using SFCore.Generics;
using SFCore.Utils;
using UnityEngine;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: AssemblyCompany("DanielSteginkUtils")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyCopyright("Copyright ©  2025")]
[assembly: AssemblyDescription("A Hollow Knight Mod")]
[assembly: AssemblyFileVersion("1.5.1.0")]
[assembly: AssemblyInformationalVersion("1.5.1.0汉化")]
[assembly: AssemblyProduct("DanielSteginkUtils")]
[assembly: AssemblyTitle("DanielSteginkUtils")]
[assembly: AssemblyVersion("1.5.1.0")]
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
namespace DanielSteginkUtils
{
	public class DanielSteginkUtils : Mod
	{
		public static DanielSteginkUtils Instance;

		public DanielSteginkUtils()
			: base("DanielSteginkUtils（工具库）")
		{
		}

		public override string GetVersion()
		{
			return "1.5.1.0";
		}

		public override int LoadPriority()
		{
			return 0;
		}

		public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
		{
			((Loggable)this).Log("Initializing");
			Instance = this;
			((Loggable)this).Log("Initialized");
		}
	}
}
namespace DanielSteginkUtils.Utilities
{
	public static class Calculations
	{
		public enum SpellType
		{
			VengefulSpirit,
			DesolateDive,
			HowlingWraiths,
			ShadeSoul,
			DescendingDark,
			AbyssShriek
		}

		public static int GetModdedInt(int baseValue, float modifier)
		{
			float val = (float)baseValue * modifier;
			if (modifier > 1f)
			{
				return (int)Math.Max(baseValue + 1, val);
			}
			if (modifier == 1f)
			{
				return baseValue;
			}
			return (int)Math.Min(baseValue - 1, val);
		}

		public static int GetSecondMelodyShield(float bonus)
		{
			float num = 0.22459999f;
			float num2 = bonus / 100f;
			return (int)((1f - (1f - num - num2) / (1f - num)) * 100f);
		}

		public static float NailDamageToSpellDamage(float nailDamage)
		{
			float num = 51.219513f;
			float num2 = 133.33334f / num;
			return nailDamage * num2;
		}

		public static float NailDamageToDreamNailDamage(float nailDamage)
		{
			float num = 4.268293f;
			return nailDamage * num;
		}

		public static float DamagePerSoul(SpellType spellType)
		{
			return spellType switch
			{
				SpellType.VengefulSpirit => 0.45454547f, 
				SpellType.DesolateDive => 1.060606f, 
				SpellType.HowlingWraiths => 1.2121212f, 
				SpellType.ShadeSoul => 0.90909094f, 
				SpellType.DescendingDark => 1.8181819f, 
				SpellType.AbyssShriek => 1.8181819f, 
				_ => throw new ArgumentException("Spell Type not recognized."), 
			};
		}

		public static float SoulPerEssence()
		{
			return 19.8f;
		}
	}
	public static class ClassIntegrations
	{
		public static O GetField<I, O>(I input, string fieldName, BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
		{
			return (O)input.GetType().GetField(fieldName, flags).GetValue(input);
		}

		public static void SetField<I>(I input, string fieldName, object value, BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
		{
			input.GetType().GetField(fieldName, flags).SetValue(input, value);
		}

		public static O GetProperty<I, O>(I input, string fieldName, BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
		{
			return (O)input.GetType().GetProperty(fieldName, flags).GetValue(input);
		}

		public static void SetProperty<I>(I input, string fieldName, object value, BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
		{
			input.GetType().GetProperty(fieldName, flags).SetValue(input, value);
		}

		public static O CallFunction<I, O>(I input, string fieldName, object[] parameters, BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
		{
			return (O)input.GetType().GetMethod(fieldName, flags).Invoke(input, parameters);
		}
	}
	internal static class Logging
	{
		internal static void Log(string prefix, string message)
		{
			((Loggable)DanielSteginkUtils.Instance).Log(prefix + " - " + message);
		}
	}
	public static class Logic
	{
		public static List<string> nailAttackNames = new List<string> { "Slash", "AltSlash", "UpSlash", "DownSlash" };

		public static List<string> nailArtNames = new List<string> { "Cyclone Slash", "Great Slash", "Dash Slash", "Hit L", "Hit R" };

		public static bool IsNailAttack(HitInstance hitInstance, bool allowNailArts = true, bool allowGrubberfly = true)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)hitInstance.Source == (Object)null)
			{
				return false;
			}
			if (nailAttackNames.Contains(((Object)hitInstance.Source).name))
			{
				return true;
			}
			if (allowNailArts && nailArtNames.Contains(((Object)hitInstance.Source).name))
			{
				return true;
			}
			if (allowGrubberfly && ((Object)hitInstance.Source).name.Contains("Grubberfly"))
			{
				return true;
			}
			return false;
		}

		public static bool IsNailArt(string objectName)
		{
			return nailArtNames.Contains(objectName);
		}

		public static bool CanTakeDamage(int damageAmount, int hazardType)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Invalid comparison between Unknown and I4
			bool flag = ClassIntegrations.CallFunction<HeroController, bool>(HeroController.instance, "CanTakeDamage", null);
			if (flag)
			{
				if (hazardType == 1)
				{
					if ((int)HeroController.instance.damageMode == 1 || HeroController.instance.cState.shadowDashing || HeroController.instance.parryInvulnTimer > 0f)
					{
						flag = false;
					}
				}
				else if (HeroController.instance.cState.invulnerable || PlayerData.instance.GetBool("isInvincible"))
				{
					flag = false;
				}
				else if (damageAmount <= 0)
				{
					flag = false;
				}
			}
			return flag;
		}
	}
	public static class NotchCosts
	{
		public static float HealingSpeedPerNotch()
		{
			float num = 2f / 3f;
			return (1f - num) / 3f;
		}

		public static float NotchesPerHeal()
		{
			float num = 1.65f;
			float num2 = 1f - 1f / num;
			return 4f + num2 / HealingSpeedPerNotch();
		}

		public static float NotchesPerMask()
		{
			return (float)UnbreakableCharmCost(2) / 2f;
		}

		public static float NotchesPerBlueMask()
		{
			return 0.75f;
		}

		public static float PassiveHealTime()
		{
			return 10f * 4f;
		}

		public static float NailDamagePerNotch()
		{
			return 0.5f / (float)UnbreakableCharmCost(3);
		}

		public static float NailRangePerNotch()
		{
			return 1f / 12f;
		}

		public static float NailArtDamagePerNotch()
		{
			float num = 25f / 123f;
			float num2 = 0.5555555f / num;
			return NailDamagePerNotch() * num2;
		}

		public static float SpellDamagePerNotch()
		{
			return 1f / 6f;
		}

		public static float SingleSpellDamagePerNotch()
		{
			return 0.5f;
		}

		public static float DamagePerNotch()
		{
			return (NailDamagePerNotch() + SpellDamagePerNotch()) / 4f;
		}

		public static float PetDamagePerNotch()
		{
			return 1f / 6f;
		}

		public static float SoulPerNailPerNotch()
		{
			return 2f;
		}

		public static float DreamNailSoulPerNotch()
		{
			return 3f;
		}

		public static float PassiveSoulTime()
		{
			return 0.5f * 5f;
		}

		public static float DashDamagePerNotch()
		{
			return (float)PlayerData.instance.GetInt("nailDamage") / 2f;
		}

		public static float GetDashCooldownPerNotch()
		{
			return 0.33333337f / 2f;
		}

		public static int UnbreakableCharmCost(int baseCost)
		{
			return baseCost + 2;
		}

		public static int NotchesPerStagger()
		{
			return 1;
		}

		public static float GeoPerNotch()
		{
			return 0.2f / (float)UnbreakableCharmCost(2);
		}

		public static float FullHealthModifier()
		{
			float num = 2f / NailRangePerNotch() / 3f - 1f;
			float num2 = 0.5f / NailDamagePerNotch();
			return (num + num2) / 3f;
		}

		public static float ShieldChancePerNotch()
		{
			return 7.486666f;
		}

		public static float IFramesPerNotch()
		{
			return 9f / 26f;
		}

		public static float UnnModifier()
		{
			return 0.2f;
		}
	}
	public static class PlayerValues
	{
		public static int MaxSoul(bool getCurrentMax)
		{
			if (PlayerData.instance.GetBool("soulLimited"))
			{
				return 66;
			}
			if (BossSequenceController.BoundSoul)
			{
				return 33;
			}
			int num = PlayerData.instance.GetInt("maxMP");
			int num2 = PlayerData.instance.GetInt("MPReserveMax");
			return num + num2;
		}

		public static int CurrentSoul()
		{
			int num = PlayerData.instance.GetInt("MPCharge");
			int num2 = PlayerData.instance.GetInt("MPReserve");
			return num + num2;
		}

		public static int SoulToSpend(int amount)
		{
			return Math.Min(amount, PlayerData.instance.GetInt("MPCharge"));
		}

		public static bool BoughtAllMaps()
		{
			if (PlayerData.instance.GetBool("mapCrossroads") && PlayerData.instance.GetBool("mapGreenpath") && PlayerData.instance.GetBool("mapFogCanyon") && PlayerData.instance.GetBool("mapRoyalGardens") && PlayerData.instance.GetBool("mapFungalWastes") && PlayerData.instance.GetBool("mapCity") && PlayerData.instance.GetBool("mapWaterways") && PlayerData.instance.GetBool("mapMines") && PlayerData.instance.GetBool("mapDeepnest") && PlayerData.instance.GetBool("mapCliffs") && PlayerData.instance.GetBool("mapOutskirts") && PlayerData.instance.GetBool("mapRestingGrounds"))
			{
				return PlayerData.instance.GetBool("mapAbyss");
			}
			return false;
		}

		public static int MaskShardsFound()
		{
			int num = 0;
			if (PlayerData.instance.slyShellFrag1)
			{
				num++;
			}
			if (PlayerData.instance.slyShellFrag2)
			{
				num++;
			}
			if (PlayerData.instance.slyShellFrag3)
			{
				num++;
			}
			if (PlayerData.instance.slyShellFrag4)
			{
				num++;
			}
			if (PlayerData.instance.dreamReward7)
			{
				num++;
			}
			return num + SceneData.instance.persistentBoolItems.Where((PersistentBoolData x) => x.id.Equals("Heart Piece")).Count();
		}

		public static int VesselFragmentsFound()
		{
			int num = 0;
			if (PlayerData.instance.slyVesselFrag1)
			{
				num++;
			}
			if (PlayerData.instance.slyVesselFrag2)
			{
				num++;
			}
			if (PlayerData.instance.dreamReward5)
			{
				num++;
			}
			if (PlayerData.instance.vesselFragStagNest)
			{
				num++;
			}
			return num + SceneData.instance.persistentBoolItems.Where((PersistentBoolData x) => x.id.Equals("Vessel Fragment")).Count();
		}
	}
}
namespace DanielSteginkUtils.Helpers
{
	public abstract class CustomBuffHelper<M, T> where M : ModBuffs<T>
	{
		public string modName { get; set; } = "";

		public string featureName { get; set; } = "";

		public float modifier { get; set; } = 1f;

		public bool performLogging { get; set; }

		private bool isActive { get; set; }

		public CustomBuffHelper(string modName, string featureName, float modifier, bool performLogging)
		{
			this.modName = modName;
			this.featureName = featureName;
			this.modifier = modifier;
			this.performLogging = performLogging;
		}

		public void Start()
		{
			if (performLogging)
			{
				Log("Starting run");
			}
			isActive = true;
			((MonoBehaviour)GameManager.instance).StartCoroutine(ApplyBuff());
		}

		public void Stop()
		{
			if (performLogging)
			{
				Log("Stopping");
			}
			isActive = false;
			RemoveBuff();
		}

		private IEnumerator ApplyBuff()
		{
			while (isActive)
			{
				AdjustObjects();
				yield return (object)new WaitForSeconds(Time.deltaTime);
			}
		}

		private void RemoveBuff()
		{
			AdjustObjects(applyBuff: false);
		}

		private void AdjustObjects(bool applyBuff = true)
		{
			foreach (GameObject @object in GetObjects())
			{
				M orAddComponent = UnityExtensions.GetOrAddComponent<M>(@object);
				if (performLogging)
				{
					Log("Mods applied: " + string.Join("|", orAddComponent.ModList.Keys) + ", " + $"Base value: {orAddComponent.BaseValue}");
				}
				T currentValue = GetCurrentValue(@object);
				if (orAddComponent.BaseValue.Equals(default(T)))
				{
					orAddComponent.BaseValue = currentValue;
				}
				if (!orAddComponent.GetModdedValue().Equals(currentValue) && !orAddComponent.BaseValue.Equals(currentValue))
				{
					orAddComponent.BaseValue = currentValue;
					orAddComponent.ModList = new Dictionary<string, Dictionary<string, float>>();
				}
				if (!orAddComponent.ModList.ContainsKey(modName))
				{
					orAddComponent.ModList.Add(modName, new Dictionary<string, float>());
				}
				if (applyBuff && !orAddComponent.ModList[modName].ContainsKey(featureName))
				{
					orAddComponent.ModList[modName].Add(featureName, modifier);
					if (performLogging)
					{
						Log($"{featureName} adjusts value by {modifier}");
					}
				}
				else if (!applyBuff && orAddComponent.ModList[modName].ContainsKey(featureName))
				{
					orAddComponent.ModList[modName].Remove(featureName);
					if (performLogging)
					{
						Log($"{featureName} no longer adjusts value by {modifier}");
					}
				}
				ApplyBuff(@object, orAddComponent);
			}
		}

		private void Log(string message)
		{
			Logging.Log(modName + " - " + featureName, message);
		}

		public virtual List<GameObject> GetObjects()
		{
			return Object.FindObjectsOfType<GameObject>().ToList();
		}

		public abstract T GetCurrentValue(GameObject gameObject);

		public abstract void ApplyBuff(GameObject gameObject, M modsApplied);
	}
	public static class GetEnemyHelper
	{
		public static GameObject GetNearestEnemy()
		{
			GameObject[] array = (from x in Object.FindObjectsOfType<GameObject>()
				where x.activeSelf
				select x).ToArray();
			if (array.Length == 0)
			{
				return null;
			}
			GameObject[] array2 = array.Where((GameObject x) => IsEnemy(x)).ToArray();
			if (array2.Length == 0)
			{
				return null;
			}
			Transform playerPosition = ((Component)HeroController.instance).gameObject.transform;
			return array2.OrderBy((GameObject x) => GetDistance(x.transform, playerPosition)).FirstOrDefault();
		}

		public static bool IsEnemy(GameObject gameObject)
		{
			bool result = false;
			if ((gameObject.layer == 11 || gameObject.tag == "Boss") && (Object)(object)gameObject.GetComponent<HealthManager>() != (Object)null)
			{
				result = true;
			}
			return result;
		}

		public static float GetDistance(Transform enemy, Transform player)
		{
			float num = Math.Abs(Extensions.GetPositionX(enemy) - Extensions.GetPositionX(player));
			float num2 = Math.Abs(Extensions.GetPositionY(enemy) - Extensions.GetPositionY(player));
			return (float)Math.Sqrt(num * num + num2 * num2);
		}
	}
	public static class SpriteHelper
	{
		public static Sprite GetLocalSprite(string filePath, string assemblyName = "", bool performLogging = false)
		{
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Expected O, but got Unknown
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			if (performLogging)
			{
				Logging.Log("SpriteHelper", "Getting sprite " + filePath + " in assembly '" + assemblyName + "'");
			}
			Assembly assembly = GetAssembly(assemblyName);
			if (assembly == null)
			{
				Logging.Log("SpriteHelper", "Assembly '" + assemblyName + "' not found");
				return null;
			}
			using Stream stream = assembly.GetManifestResourceStream(filePath);
			byte[] array = new byte[stream.Length];
			stream.Read(array, 0, array.Length);
			Texture2D val = new Texture2D(2, 2);
			ImageConversion.LoadImage(val, array, true);
			return Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), new Vector2(0.5f, 0.5f));
		}

		private static Assembly GetAssembly(string assemblyName = "")
		{
			if (string.IsNullOrEmpty(assemblyName))
			{
				return Assembly.GetExecutingAssembly();
			}
			return Assembly.Load(assemblyName);
		}
	}
}
namespace DanielSteginkUtils.Helpers.Shields
{
	public class CarefreeHelper : ShieldHelper
	{
		private int shieldChance { get; set; }

		private bool performLogging { get; set; }

		public CarefreeHelper(int shieldChance, bool performLogging = false)
		{
			this.shieldChance = shieldChance;
			this.performLogging = performLogging;
		}

		public override bool CustomShieldCheck()
		{
			int num = Random.Range(1, 101);
			if (performLogging)
			{
				Logging.Log("CarefreeHelper", $"Random chance {num} versus {shieldChance}");
			}
			return num <= shieldChance;
		}

		public override IEnumerator CustomEffects()
		{
			GameObject carefreeShield = HeroController.instance.carefreeShield;
			if ((Object)(object)carefreeShield != (Object)null)
			{
				carefreeShield.SetActive(true);
			}
			return base.CustomEffects();
		}
	}
	public abstract class ShieldHelper
	{
		private bool isImmune;

		public virtual bool blockFirstDamage { get; set; } = true;

		public virtual void Start()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			HeroController.TakeDamage += new hook_TakeDamage(Block);
		}

		public virtual void Stop()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			HeroController.TakeDamage -= new hook_TakeDamage(Block);
		}

		private void Block(orig_TakeDamage orig, HeroController self, GameObject go, CollisionSide damageSide, int damageAmount, int hazardType)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			if (CanTakeDamage(damageAmount, hazardType))
			{
				if (isImmune)
				{
					damageAmount = 0;
				}
				else if (CustomShieldCheck())
				{
					if (blockFirstDamage)
					{
						damageAmount = 0;
					}
					((MonoBehaviour)GameManager.instance).StartCoroutine(Invincibility());
				}
			}
			orig.Invoke(self, go, damageSide, damageAmount, hazardType);
		}

		public virtual bool CanTakeDamage(int damageAmount, int hazardType)
		{
			return Logic.CanTakeDamage(damageAmount, hazardType);
		}

		public abstract bool CustomShieldCheck();

		public IEnumerator Invincibility()
		{
			isImmune = true;
			yield return CustomEffects();
			isImmune = false;
		}

		public virtual IEnumerator CustomEffects()
		{
			yield return (object)new WaitForSeconds(GetIFrames());
		}

		public float GetIFrames()
		{
			float result = HeroController.instance.INVUL_TIME;
			if (PlayerData.instance.GetBool("equippedCharm_4"))
			{
				result = HeroController.instance.INVUL_TIME_STAL;
			}
			return result;
		}
	}
}
namespace DanielSteginkUtils.Helpers.Charms
{
	public static class GetModCharmHelper
	{
		public static int GetCharmId(string[] charmNames)
		{
			int maxCustomCharms = FullSettingsMod<SFCoreSaveSettings, SFCoreGlobalSettings>.GlobalSettings.MaxCustomCharms;
			for (int i = 41; i <= 40 + maxCustomCharms; i++)
			{
				string value = Language.Get($"CHARM_NAME_{i}", "UI");
				if (charmNames.Contains(value))
				{
					return i;
				}
			}
			return -1;
		}
	}
}
namespace DanielSteginkUtils.Helpers.Charms.Templates
{
	public abstract class ExaltedCharm : TemplateCharm
	{
		public abstract string name { get; }

		public abstract string exaltedName { get; }

		public abstract string description { get; }

		public abstract string exaltedDescription { get; }

		public abstract int cost { get; }

		public abstract int exaltedCost { get; }

		public abstract Sprite exaltedIcon { get; }

		public bool IsUpgraded { get; protected set; }

		public ExaltedCharm(string modName, bool addToMap)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			base..ctor(modName, addToMap);
			CharmIconList.GetSprite += new hook_GetSprite(GetExaltedIcon);
			ModHooks.SavegameSaveHook += Upgrade;
		}

		protected override string GetName()
		{
			if (!IsUpgraded)
			{
				return name;
			}
			return exaltedName;
		}

		protected override string GetDescription()
		{
			if (!IsUpgraded)
			{
				return description;
			}
			return exaltedDescription;
		}

		protected override int GetCharmCost()
		{
			if (!IsUpgraded)
			{
				return cost;
			}
			return exaltedCost;
		}

		public override string GetItemChangerId()
		{
			return Regex.Replace(name, "[^0-9a-zA-Z\\._]", "");
		}

		public virtual Sprite GetExaltedIcon(orig_GetSprite orig, CharmIconList self, int id)
		{
			if (id == ((EasyCharm)this).Id && IsUpgraded)
			{
				return exaltedIcon;
			}
			return orig.Invoke(self, id);
		}

		public virtual void Upgrade(int obj)
		{
			if (IsExaltationInstalled() && CanUpgrade() && ((EasyCharm)this).GotCharm)
			{
				Upgrade();
			}
		}

		internal bool IsExaltationInstalled()
		{
			return ModHooks.GetMod("Exaltation", false, false) != null;
		}

		public abstract bool CanUpgrade();

		public virtual void Upgrade()
		{
			IsUpgraded = true;
		}

		public void RestoreCharmState(ExaltedCharmState state)
		{
			((EasyCharm)this).RestoreCharmState((EasyCharmState)(object)state);
			IsUpgraded = state.IsUpgraded;
		}

		public ExaltedCharmState GetCharmState()
		{
			return new ExaltedCharmState
			{
				IsEquipped = ((EasyCharm)this).IsEquipped,
				IsNew = ((EasyCharm)this).IsNew,
				IsUpgraded = IsUpgraded,
				GotCharm = ((EasyCharm)this).GotCharm
			};
		}
	}
	public class ExaltedCharmState : EasyCharmState
	{
		public bool IsUpgraded;
	}
	public abstract class TemplateCharm : EasyCharm
	{
		private string ModName { get; set; }

		public TemplateCharm(string modName, bool addToMap)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Expected O, but got Unknown
			((EasyCharm)this)..ctor();
			ModName = modName;
			Logging.Log(ModName, $"Charm added under the numeric ID {((EasyCharm)this).Id}");
			ModHooks.SetPlayerBoolHook += new SetBoolProxy(CheckEquipped);
			HeroController.Start += new hook_Start(OnPlayerDataLoaded);
			if (addToMap)
			{
				AddCharmToItemChanger();
				UIManager.StartNewGame += new hook_StartNewGame(NewGame);
			}
		}

		internal bool CheckEquipped(string name, bool orig)
		{
			if (name.Equals($"equippedCharm_{((EasyCharm)this).Id}"))
			{
				if (orig)
				{
					Equip();
				}
				else
				{
					Unequip();
				}
			}
			return orig;
		}

		private void OnPlayerDataLoaded(orig_Start orig, HeroController self)
		{
			orig.Invoke(self);
			if (((EasyCharm)this).IsEquipped)
			{
				Equip();
			}
		}

		public abstract void Equip();

		public abstract void Unequip();

		public virtual string GetItemChangerId()
		{
			return Regex.Replace(((EasyCharm)this).GetName(), "[^0-9a-zA-Z\\._]", "");
		}

		internal void AddCharmToItemChanger()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Expected O, but got Unknown
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Expected O, but got Unknown
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Expected O, but got Unknown
			//IL_0083: Expected O, but got Unknown
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Expected O, but got Unknown
			CharmItem val = new CharmItem
			{
				charmNum = ((EasyCharm)this).Id,
				name = GetItemChangerId(),
				UIDef = (UIDef)new MsgUIDef
				{
					name = (IString)new LanguageString("UI", $"CHARM_NAME_{((EasyCharm)this).Id}"),
					shopDesc = (IString)new LanguageString("UI", $"CHARM_DESC_{((EasyCharm)this).Id}"),
					sprite = (ISprite)new BoxedSprite(((EasyCharm)this).GetSprite())
				}
			};
			InteropTag obj = ((TaggableObject)val).AddTag<InteropTag>();
			obj.Message = "RandoSupplementalMetadata";
			obj.Properties["ModSource"] = ModName;
			obj.Properties["PoolGroup"] = "Charms";
			Finder.DefineCustomItem((AbstractItem)val);
			Finder.DefineCustomLocation(ItemChangerLocation());
		}

		public abstract AbstractLocation ItemChangerLocation();

		private void NewGame(orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
		{
			PlaceCharms();
			orig.Invoke(self, permaDeath, bossRush);
		}

		public void PlaceCharms()
		{
			if (!((EasyCharm)this).GotCharm)
			{
				ItemChangerMod.CreateSettingsProfile(false, false);
				List<AbstractPlacement> list = new List<AbstractPlacement>();
				AbstractPlacement val = Finder.GetLocation(GetItemChangerId()).Wrap();
				AbstractItem item = Finder.GetItem(GetItemChangerId());
				val.Add(item);
				list.Add(val);
				ItemChangerMod.AddPlacements((IEnumerable<AbstractPlacement>)list, (PlacementConflictResolution)2);
			}
		}

		public abstract void OnLoadLocal();

		public abstract void OnSaveLocal();
	}
}
namespace DanielSteginkUtils.Helpers.Charms.Shroom
{
	public class SporeDamageHelper : CustomBuffHelper<BuffSporeDamage, float>
	{
		public SporeDamageHelper(string modName, string featureName, float modifier, bool performLogging = false)
			: base(modName, featureName, modifier, performLogging)
		{
		}

		public override List<GameObject> GetObjects()
		{
			return (from x in base.GetObjects()
				where ((Object)x).name.StartsWith("Knight Spore Cloud(Clone)")
				select x).ToList();
		}

		public override float GetCurrentValue(GameObject gameObject)
		{
			return gameObject.GetComponent<DamageEffectTicker>().damageInterval;
		}

		public override void ApplyBuff(GameObject gameObject, BuffSporeDamage modsApplied)
		{
			gameObject.GetComponent<DamageEffectTicker>().SetDamageInterval(modsApplied.GetModdedValue());
		}
	}
	public class SporeSizeHelper : CustomBuffHelper<BuffSporeSize, Vector3>
	{
		public SporeSizeHelper(string modName, string featureName, float modifier, bool performLogging = false)
			: base(modName, featureName, modifier, performLogging)
		{
		}

		public override List<GameObject> GetObjects()
		{
			return (from x in base.GetObjects()
				where ((Object)x).name.StartsWith("Knight Spore Cloud(Clone)")
				select x).ToList();
		}

		public override Vector3 GetCurrentValue(GameObject gameObject)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return gameObject.transform.localScale;
		}

		public override void ApplyBuff(GameObject gameObject, BuffSporeSize modsApplied)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			gameObject.transform.localScale = modsApplied.GetModdedValue();
		}
	}
}
namespace DanielSteginkUtils.Helpers.Charms.Pets
{
	public class AllPetsHelper
	{
		private GrimmchildHelper grimmHelper;

		private HatchlingHelper hatchlingHelper;

		private WeaverlingHelper weaverHelper;

		private FlukeHelper flukeHelper;

		private DungFlukeHelper dungFlukeHelper;

		private AspidAspectHelper aspidAspectHelper;

		public AllPetsHelper(string modName, string featureName, float damageModifier, bool performLogging = false)
		{
			grimmHelper = new GrimmchildHelper(damageModifier, 1f, performLogging);
			hatchlingHelper = new HatchlingHelper(damageModifier, performLogging);
			weaverHelper = new WeaverlingHelper(damageModifier, performLogging);
			flukeHelper = new FlukeHelper(damageModifier, performLogging);
			dungFlukeHelper = new DungFlukeHelper(modName, featureName, 1f / damageModifier, performLogging);
			aspidAspectHelper = new AspidAspectHelper(modName, featureName, damageModifier, performLogging);
		}

		public void Start()
		{
			grimmHelper.Start();
			hatchlingHelper.Start();
			weaverHelper.Start();
			flukeHelper.Start();
			dungFlukeHelper.Start();
			aspidAspectHelper.Start();
		}

		public void Stop()
		{
			grimmHelper.Stop();
			hatchlingHelper.Stop();
			weaverHelper.Stop();
			flukeHelper.Stop();
			dungFlukeHelper.Stop();
			aspidAspectHelper.Stop();
		}
	}
	public class AspidAspectHelper : CustomBuffHelper<BuffAspidAspect, int>
	{
		public AspidAspectHelper(string modName, string featureName, float modifier, bool performLogging)
			: base(modName, featureName, modifier, performLogging)
		{
		}

		public override List<GameObject> GetObjects()
		{
			List<GameObject> list = (from x in base.GetObjects()
				where ((Object)x).name.StartsWith("Aspid Shot - Damage All")
				select x).ToList();
			List<GameObject> list2 = new List<GameObject>();
			foreach (GameObject item in list)
			{
				if ((Object)(object)(from x in item.GetComponentsInChildren<Component>()
					where ((object)x).GetType().Name.Equals("AspidShotExtraDamager")
					select x).FirstOrDefault() != (Object)null)
				{
					list2.Add(item);
				}
			}
			return list2;
		}

		public override void ApplyBuff(GameObject gameObject, BuffAspidAspect modsApplied)
		{
			Component val = (from x in gameObject.GetComponentsInChildren<Component>()
				where ((object)x).GetType().Name.Equals("AspidShotExtraDamager")
				select x).FirstOrDefault();
			if ((Object)(object)val != (Object)null)
			{
				ClassIntegrations.SetProperty<Component>(val, "AdditionalDamage", modsApplied.GetModdedValue());
				if (base.performLogging)
				{
					Logging.Log("AspidAspectHelper", $"Damage buffed from {modsApplied.BaseValue} to {modsApplied.GetModdedValue()}");
				}
				return;
			}
			throw new ArgumentException("Unable to set damage value for AspidAspectHelper.");
		}

		public override int GetCurrentValue(GameObject gameObject)
		{
			Component val = (from x in gameObject.GetComponentsInChildren<Component>()
				where ((object)x).GetType().Name.Equals("AspidShotExtraDamager")
				select x).FirstOrDefault();
			if ((Object)(object)val != (Object)null)
			{
				return ClassIntegrations.GetProperty<Component, int>(val, "AdditionalDamage");
			}
			throw new ArgumentException("Unable to get damage value for AspidAspectHelper.");
		}
	}
	public class FlukeHelper
	{
		private float modifier { get; set; } = 1f;

		private bool performLogging { get; set; }

		public FlukeHelper(float modifier, bool performLogging = false)
		{
			this.modifier = modifier;
			this.performLogging = performLogging;
		}

		public void Start()
		{
			ModHooks.ObjectPoolSpawnHook += BuffDamage;
		}

		public void Stop()
		{
			ModHooks.ObjectPoolSpawnHook -= BuffDamage;
		}

		private GameObject BuffDamage(GameObject gameObject)
		{
			if (((Object)gameObject).name.StartsWith("Spell Fluke") && ((Object)gameObject).name.Contains("Clone") && !((Object)gameObject).name.Contains("Dung"))
			{
				SpellFluke component = gameObject.GetComponent<SpellFluke>();
				int field = ClassIntegrations.GetField<SpellFluke, int>(component, "damage");
				int moddedInt = Calculations.GetModdedInt(field, modifier);
				ClassIntegrations.SetField<SpellFluke>(component, "damage", moddedInt);
				if (performLogging)
				{
					Logging.Log("FlukeHelper", $"Fluke damage increased from {field} to {moddedInt}");
				}
			}
			return gameObject;
		}
	}
	public class GrimmchildHelper
	{
		private float damageModifier { get; set; } = 1f;

		private float attackModifier { get; set; } = 1f;

		private bool performLogging { get; set; }

		public GrimmchildHelper(float damageModifier, float attackModifier, bool performLogging = false)
		{
			this.damageModifier = damageModifier;
			this.attackModifier = attackModifier;
			this.performLogging = performLogging;
		}

		public void Start()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			IntOperator.OnEnter += new hook_OnEnter(BuffDamage);
			SetFloatValue.OnEnter += new hook_OnEnter(BuffSpeed);
			RandomFloat.OnEnter += new hook_OnEnter(BuffRandomSpeed);
		}

		public void Stop()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			IntOperator.OnEnter -= new hook_OnEnter(BuffDamage);
			SetFloatValue.OnEnter -= new hook_OnEnter(BuffSpeed);
			RandomFloat.OnEnter -= new hook_OnEnter(BuffRandomSpeed);
		}

		private void BuffDamage(orig_OnEnter orig, IntOperator self)
		{
			if (((FsmStateAction)self).Fsm.Name.Equals("Attack") && ((Object)((FsmStateAction)self).Fsm.GameObject).name.Equals("Enemy Damager") && ((Object)((Component)((FsmStateAction)self).Fsm.GameObject.transform.parent).gameObject).name.Contains("Grimmball") && ((FsmStateAction)self).State.Name.Equals("Hit"))
			{
				int value = ((FsmStateAction)self).Fsm.GetFsmInt("Damage").Value;
				((FsmStateAction)self).Fsm.GetFsmInt("Damage").Value = Calculations.GetModdedInt(value, damageModifier);
				if (performLogging)
				{
					Logging.Log("GrimmchildHelper", string.Format("Grimmchild damage increased from {0} to {1}", value, ((FsmStateAction)self).Fsm.GetFsmInt("Damage").Value));
				}
				orig.Invoke(self);
				((FsmStateAction)self).Fsm.GetFsmInt("Damage").Value = value;
			}
			else
			{
				orig.Invoke(self);
			}
		}

		private void BuffSpeed(orig_OnEnter orig, SetFloatValue self)
		{
			if (((Object)((FsmStateAction)self).Fsm.GameObject).name.Equals("Grimmchild(Clone)") && ((FsmStateAction)self).Fsm.Name.Equals("Control") && (((FsmStateAction)self).State.Name.Equals("Pause") || ((FsmStateAction)self).State.Name.Equals("Spawn")) && attackModifier != 1f)
			{
				float value = self.floatValue.Value;
				FsmFloat floatValue = self.floatValue;
				floatValue.Value *= attackModifier;
				if (performLogging)
				{
					Logging.Log("GrimmchildHelper", $"Attack speed reduced from {value} to {self.floatValue.Value}");
				}
				orig.Invoke(self);
				self.floatValue.Value = value;
			}
			else
			{
				orig.Invoke(self);
			}
		}

		private void BuffRandomSpeed(orig_OnEnter orig, RandomFloat self)
		{
			if (((Object)((FsmStateAction)self).Fsm.GameObject).name.Equals("Grimmchild(Clone)") && ((FsmStateAction)self).Fsm.Name.Equals("Control") && ((FsmStateAction)self).State.Name.Equals("Antic") && attackModifier != 1f)
			{
				float value = self.min.Value;
				float value2 = self.min.Value;
				FsmFloat min = self.min;
				min.Value *= attackModifier;
				FsmFloat max = self.max;
				max.Value *= attackModifier;
				if (performLogging)
				{
					Logging.Log("GrimmchildHelper", $"Random speed reduced from ({value} - {value2}) to ({self.min.Value} - {self.max.Value})");
				}
				orig.Invoke(self);
				self.min.Value = value;
				self.max.Value = value2;
			}
			else
			{
				orig.Invoke(self);
			}
		}
	}
	public class HatchlingHelper
	{
		private float damageModifier { get; set; } = 1f;

		private bool performLogging { get; set; }

		public HatchlingHelper(float damageModifier, bool performLogging = false)
		{
			this.damageModifier = damageModifier;
			this.performLogging = performLogging;
		}

		public void Start()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			HealthManager.Hit += new hook_Hit(BuffHatchlings);
		}

		public void Stop()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			HealthManager.Hit -= new hook_Hit(BuffHatchlings);
		}

		private void BuffHatchlings(orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			string text = "";
			if (((Object)hitInstance.Source.gameObject).name.Equals("Damager"))
			{
				try
				{
					text = ((Object)hitInstance.Source.gameObject.transform.parent).name;
				}
				catch
				{
				}
				if (text.Contains("Knight Hatchling"))
				{
					int damageDealt = hitInstance.DamageDealt;
					hitInstance.DamageDealt = Calculations.GetModdedInt(damageDealt, damageModifier);
					if (performLogging)
					{
						Logging.Log("HatchlingHelper", $"Hatchling damage increased from {damageDealt} to {hitInstance.DamageDealt}");
					}
				}
			}
			orig.Invoke(self, hitInstance);
		}
	}
	public class WeaverlingHelper
	{
		private float damageModifier { get; set; } = 1f;

		private bool performLogging { get; set; }

		public WeaverlingHelper(float damageModifier, bool performLogging = false)
		{
			this.damageModifier = damageModifier;
			this.performLogging = performLogging;
		}

		public void Start()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			IntOperator.OnEnter += new hook_OnEnter(BuffWeaverlings);
		}

		public void Stop()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			IntOperator.OnEnter -= new hook_OnEnter(BuffWeaverlings);
		}

		private void BuffWeaverlings(orig_OnEnter orig, IntOperator self)
		{
			if (((FsmStateAction)self).Fsm.Name.Equals("Attack") && ((Object)((FsmStateAction)self).Fsm.GameObject).name.Equals("Enemy Damager") && ((Object)((Component)((FsmStateAction)self).Fsm.GameObject.transform.parent).gameObject).name.Contains("Weaverling") && ((FsmStateAction)self).State.Name.Equals("Hit"))
			{
				int value = ((FsmStateAction)self).Fsm.GetFsmInt("Damage").Value;
				((FsmStateAction)self).Fsm.GetFsmInt("Damage").Value = Calculations.GetModdedInt(value, damageModifier);
				if (performLogging)
				{
					Logging.Log("WeaverlingHelper", string.Format("Weaverling damage increased from {0} to {1}", value, ((FsmStateAction)self).Fsm.GetFsmInt("Damage").Value));
				}
				orig.Invoke(self);
				((FsmStateAction)self).Fsm.GetFsmInt("Damage").Value = value;
			}
			else
			{
				orig.Invoke(self);
			}
		}
	}
}
namespace DanielSteginkUtils.Helpers.Charms.Elegy
{
	public class ElegyBeamAttacker
	{
		public int beamChance { get; set; }

		private bool performLogging { get; set; }

		public ElegyBeamAttacker(int beamChance, bool performLogging = false)
		{
			this.beamChance = beamChance;
			this.performLogging = performLogging;
		}

		public void Start()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			HeroController.Attack += new hook_Attack(FireBeam);
		}

		public void Stop()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			HeroController.Attack -= new hook_Attack(FireBeam);
		}

		private void FireBeam(orig_Attack orig, HeroController self, AttackDirection attackDir)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Expected I4, but got Unknown
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self, attackDir);
			int num = Random.Range(1, 101);
			if (performLogging)
			{
				Logging.Log("ElegyBeamAttacker", $"Random chance {num} versus {beamChance}");
			}
			if (num <= beamChance)
			{
				GameObject val = null;
				float field = ClassIntegrations.GetField<HeroController, float>(HeroController.instance, "MANTIS_CHARM_SCALE");
				switch ((int)attackDir)
				{
				case 0:
					val = ((!(((Component)self).transform.localScale.x < 0f)) ? ObjectPoolExtensions.Spawn(self.grubberFlyBeamPrefabL, ((Component)self).transform.position) : ObjectPoolExtensions.Spawn(self.grubberFlyBeamPrefabR, ((Component)self).transform.position));
					Extensions.SetScaleY(val.transform, 1f);
					break;
				case 1:
					val = ObjectPoolExtensions.Spawn(self.grubberFlyBeamPrefabU, ((Component)self).transform.position);
					Extensions.SetScaleY(val.transform, ((Component)self).transform.localScale.x);
					val.transform.localEulerAngles = new Vector3(0f, 0f, 270f);
					break;
				case 2:
					val = ObjectPoolExtensions.Spawn(self.grubberFlyBeamPrefabD, ((Component)self).transform.position);
					Extensions.SetScaleY(val.transform, ((Component)self).transform.localScale.x);
					val.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
					break;
				}
				if (PlayerData.instance.GetBool("equippedCharm_13"))
				{
					Extensions.SetScaleY(val.transform, val.transform.localScale.y * field);
				}
			}
		}
	}
	public abstract class ElegyBeamRangeHelper
	{
		public string modName { get; set; } = "";

		public string featureName { get; set; } = "";

		public bool performLogging { get; set; }

		public ElegyBeamRangeHelper(string modName, string featureName, bool performLogging)
		{
			this.modName = modName;
			this.featureName = featureName;
			this.performLogging = performLogging;
		}

		public void Start()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			SetVelocity2d.OnEnter += new hook_OnEnter(BuffBeamSpeed);
		}

		public void Stop()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			SetVelocity2d.OnEnter -= new hook_OnEnter(BuffBeamSpeed);
		}

		private GameObject BuffBeam(orig_Spawn_GameObject_Transform_Vector3_Quaternion orig, GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			GameObject newObject = orig.Invoke(prefab, parent, position, rotation);
			if (((Object)newObject).name.Contains("Grubberfly Beam"))
			{
				FsmUtil.AddMethod(FsmUtil.GetState(FSMUtility.LocateMyFSM(newObject, "Control"), "Init"), (Action)delegate
				{
					//IL_0032: Unknown result type (might be due to invalid IL or missing references)
					//IL_0038: Unknown result type (might be due to invalid IL or missing references)
					//IL_006c: Unknown result type (might be due to invalid IL or missing references)
					string direction = GetDirection(((Object)newObject).name);
					float modifier = GetModifier(direction);
					Rigidbody2D component = newObject.GetComponent<Rigidbody2D>();
					component.velocity *= modifier;
					if (performLogging)
					{
						Log($"Beam {((Object)newObject).name}'s speed adjusted by {modifier} to {component.velocity}");
					}
				});
			}
			return newObject;
		}

		private void BuffBeamSpeed(orig_OnEnter orig, SetVelocity2d self)
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			try
			{
				GameObject gameObject = ((FsmStateAction)self).Fsm.GameObject;
				if (((FsmStateAction)self).Fsm.Name.Equals("Control") && ((Object)gameObject).name.Contains("Grubberfly"))
				{
					string direction = GetDirection(((Object)gameObject).name);
					float modifier = GetModifier(direction);
					Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
					component.velocity *= modifier;
					if (performLogging)
					{
						Log($"Beam {((Object)gameObject).name}'s speed adjusted by {modifier} to {component.velocity}");
					}
				}
			}
			catch
			{
			}
		}

		private void Log(string message)
		{
			Logging.Log(modName + " - " + featureName, message);
		}

		public abstract float GetModifier(string direction);

		public string GetDirection(string objectName)
		{
			if (objectName.StartsWith("Grubberfly BeamR"))
			{
				return "RIGHT";
			}
			if (objectName.StartsWith("Grubberfly BeamL"))
			{
				return "LEFT";
			}
			if (objectName.StartsWith("Grubberfly BeamU"))
			{
				return "UP";
			}
			if (objectName.StartsWith("Grubberfly BeamD"))
			{
				return "DOWN";
			}
			return "";
		}
	}
}
namespace DanielSteginkUtils.Helpers.Charms.Dung
{
	public class DungDamageHelper : CustomBuffHelper<BuffDungDamage, float>
	{
		public DungDamageHelper(string modName, string featureName, float modifier, bool performLogging = false)
			: base(modName, featureName, modifier, performLogging)
		{
		}

		public override List<GameObject> GetObjects()
		{
			return (from x in base.GetObjects()
				where ((Object)x).name.StartsWith("Knight Dung Trail(Clone)")
				select x).ToList();
		}

		public override float GetCurrentValue(GameObject gameObject)
		{
			return gameObject.GetComponent<DamageEffectTicker>().damageInterval;
		}

		public override void ApplyBuff(GameObject gameObject, BuffDungDamage modsApplied)
		{
			gameObject.GetComponent<DamageEffectTicker>().SetDamageInterval(modsApplied.GetModdedValue());
		}
	}
	public class DungFlukeHelper : CustomBuffHelper<BuffDungFluke, float>
	{
		public DungFlukeHelper(string modName, string featureName, float modifier, bool performLogging = false)
			: base(modName, featureName, modifier, performLogging)
		{
		}

		public override List<GameObject> GetObjects()
		{
			return (from x in base.GetObjects()
				where ((Object)x).name.StartsWith("Knight Dung Cloud")
				select x).ToList();
		}

		public override float GetCurrentValue(GameObject gameObject)
		{
			return gameObject.GetComponent<DamageEffectTicker>().damageInterval;
		}

		public override void ApplyBuff(GameObject gameObject, BuffDungFluke modsApplied)
		{
			gameObject.GetComponent<DamageEffectTicker>().SetDamageInterval(modsApplied.GetModdedValue());
		}
	}
	public class DungSizeHelper : CustomBuffHelper<BuffDungSize, Vector3>
	{
		public DungSizeHelper(string modName, string featureName, float modifier, bool performLogging = false)
			: base(modName, featureName, modifier, performLogging)
		{
		}

		public override List<GameObject> GetObjects()
		{
			return (from x in base.GetObjects()
				where ((Object)x).name.StartsWith("Knight Dung Trail(Clone)")
				select x).ToList();
		}

		public override Vector3 GetCurrentValue(GameObject gameObject)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return gameObject.transform.localScale;
		}

		public override void ApplyBuff(GameObject gameObject, BuffDungSize modsApplied)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			gameObject.transform.localScale = modsApplied.GetModdedValue();
		}
	}
}
namespace DanielSteginkUtils.Helpers.Libraries
{
	public static class SoulHelper
	{
		public static void GainSoul(int amount, bool performLogging = false)
		{
			int num = PlayerData.instance.GetInt("MPReserve");
			PlayerData.instance.AddMPCharge(amount);
			GameCameras.instance.soulOrbFSM.SendEvent("MP GAIN");
			if (PlayerData.instance.GetInt("MPReserve") != num)
			{
				GameManager.instance.soulVessel_fsm.SendEvent("MP RESERVE UP");
			}
			if (performLogging)
			{
				Logging.Log("SoulHelper", $"{amount} SOUL sent to player.");
			}
		}
	}
}
namespace DanielSteginkUtils.Helpers.Attributes
{
	public static class DamageHelper
	{
		public static void DealDamage(HealthManager enemy, int amount, AttackTypes attackType, string attackName)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = new GameObject(attackName);
			DealDamage(enemy, amount, attackType, val);
			Object.Destroy((Object)(object)val);
		}

		public static void DealDamage(HealthManager enemy, int amount, AttackTypes attackType, GameObject attacker)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			HitInstance val = new HitInstance
			{
				DamageDealt = amount,
				AttackType = attackType,
				IgnoreInvulnerable = true,
				Source = attacker,
				Multiplier = 1f
			};
			enemy.Hit(val);
		}
	}
	public class GeoHelper
	{
		public float modifier { get; set; } = 1f;

		private bool performLogging { get; set; }

		public GeoHelper(float modifier, bool performLogging = false)
		{
			this.modifier = modifier;
			this.performLogging = performLogging;
		}

		public void Start()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			HeroController.AddGeo += new hook_AddGeo(AddGeo);
		}

		public void Stop()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			HeroController.AddGeo -= new hook_AddGeo(AddGeo);
		}

		private void AddGeo(orig_AddGeo orig, HeroController self, int amount)
		{
			int moddedInt = Calculations.GetModdedInt(amount, modifier);
			orig.Invoke(self, moddedInt);
			if (performLogging)
			{
				Logging.Log("GeoHelper", $"Geo increased from {amount} to {moddedInt}");
			}
		}
	}
	public class HealHelper
	{
		public int healChance { get; set; }

		private bool performLogging { get; set; }

		public HealHelper(int healChance, bool performLogging = false)
		{
			this.healChance = healChance;
			this.performLogging = performLogging;
		}

		public void Start()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			HeroController.AddHealth += new hook_AddHealth(IncreaseHealing);
		}

		public void Stop()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			HeroController.AddHealth -= new hook_AddHealth(IncreaseHealing);
		}

		private void IncreaseHealing(orig_AddHealth orig, HeroController self, int amount)
		{
			int num = Random.Range(1, 101);
			if (performLogging)
			{
				Logging.Log("HealHelper", $"Random chance {num} versus {healChance}");
			}
			if (num <= healChance && CustomCheck())
			{
				amount++;
			}
			orig.Invoke(self, amount);
		}

		public virtual bool CustomCheck()
		{
			return true;
		}
	}
	public class HealingSpeedHelper
	{
		public float modifier { get; set; } = 1f;

		public bool performLogging { get; set; }

		public HealingSpeedHelper(float modifier, bool performLogging = false)
		{
			this.modifier = modifier;
			this.performLogging = performLogging;
		}

		public void Start()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			HeroController.StartMPDrain += new hook_StartMPDrain(SpeedHealing);
		}

		public void Stop()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			HeroController.StartMPDrain -= new hook_StartMPDrain(SpeedHealing);
		}

		private void SpeedHealing(orig_StartMPDrain orig, HeroController self, float time)
		{
			time *= modifier;
			if (performLogging)
			{
				Logging.Log("HealingSpeedHelper", $"Healing speed set to {time}");
			}
			orig.Invoke(self, time);
		}
	}
	public abstract class NailArtRangeHelper
	{
		public string modName { get; set; } = "";

		public string featureName { get; set; } = "";

		public float modifier { get; set; } = 1f;

		public bool performLogging { get; set; }

		private bool buffApplied { get; set; }

		public NailArtRangeHelper(string modName, string featureName, float modifier, bool performLogging)
		{
			this.modName = modName;
			this.featureName = featureName;
			this.modifier = modifier;
			this.performLogging = performLogging;
		}

		public void Start()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			FsmState.OnEnter += new hook_OnEnter(BuffNailArt);
		}

		public void Stop()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			FsmState.OnEnter -= new hook_OnEnter(BuffNailArt);
		}

		private void Log(string message)
		{
			Logging.Log(modName + " - " + featureName, message);
		}

		private void BuffNailArt(orig_OnEnter orig, FsmState self)
		{
			orig.Invoke(self);
			if (!self.Fsm.Name.Equals("Nail Arts"))
			{
				return;
			}
			string[] array = new string[3] { "Cyclone Slash", "Great Slash Effect", "Dash Slash" };
			if (new string[3] { "Activate Slash", "G Slash", "Dash Slash" }.Contains(self.Name) && !buffApplied)
			{
				string[] array2 = array;
				foreach (string nailArtName in array2)
				{
					ApplyBuff(self, nailArtName, applyBuff: true);
				}
				buffApplied = true;
			}
			if (buffApplied && (self.Name.Equals("Regain Control") || self.Name.Equals("Cancel All")))
			{
				string[] array2 = array;
				foreach (string nailArtName2 in array2)
				{
					ApplyBuff(self, nailArtName2, applyBuff: false);
				}
				buffApplied = false;
			}
		}

		private void ApplyBuff(FsmState self, string nailArtName, bool applyBuff)
		{
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			GameObject value = self.Fsm.Variables.GetFsmGameObject(nailArtName).Value;
			if (applyBuff)
			{
				Transform transform = self.Fsm.Variables.GetFsmGameObject(nailArtName).Value.transform;
				transform.localScale *= modifier;
				if (performLogging)
				{
					Log($"Game object {((Object)value).name} buffed by {modifier} to {value.transform.localScale}");
				}
			}
			else
			{
				Transform transform2 = self.Fsm.Variables.GetFsmGameObject(nailArtName).Value.transform;
				transform2.localScale /= modifier;
				if (performLogging)
				{
					Log($"Game object {((Object)value).name} reset by {modifier} to {value.transform.localScale}");
				}
			}
		}
	}
	public abstract class NailRangeHelper
	{
		public bool forceMantisAnim { get; set; }

		public void Start()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			NailSlash.StartSlash += new hook_StartSlash(IncreaseNailRange);
		}

		public void Stop()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			NailSlash.StartSlash -= new hook_StartSlash(IncreaseNailRange);
		}

		private void IncreaseNailRange(orig_StartSlash orig, NailSlash self)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			Vector3 scale = self.scale;
			if (forceMantisAnim)
			{
				self.SetMantis(true);
				if (!PlayerData.instance.GetBool("equippedCharm_13"))
				{
					float num = 1f;
					num = ((!PlayerData.instance.GetBool("equippedCharm_18")) ? 0.8f : (23f / 28f));
					self.scale = new Vector3(scale.x * num, scale.y * num);
				}
			}
			if (CustomCheck())
			{
				float modifier = GetModifier();
				Vector3 scale2 = default(Vector3);
				((Vector3)(ref scale2))..ctor(self.scale.x * modifier, self.scale.y * modifier);
				self.scale = scale2;
			}
			orig.Invoke(self);
			self.scale = scale;
		}

		public abstract bool CustomCheck();

		public abstract float GetModifier();
	}
	public class SpeedHelper
	{
		private bool isUnnActive;

		private bool applyUnn = true;

		public float speedModifier { get; set; } = 1f;

		public float unnModifier { get; set; } = 1f;

		public bool performLogging { get; set; }

		public SpeedHelper(float speedModifier, float unnModifier = 1f, bool performLogging = false)
		{
			this.speedModifier = speedModifier;
			this.unnModifier = unnModifier;
			this.performLogging = performLogging;
		}

		public void Start()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			HeroController.Move += new hook_Move(SpeedBoost);
			isUnnActive = true;
			((MonoBehaviour)GameManager.instance).StartCoroutine(UnnBoost());
		}

		public void Stop()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			HeroController.Move -= new hook_Move(SpeedBoost);
			isUnnActive = false;
		}

		private void SpeedBoost(orig_Move orig, HeroController self, float move_direction)
		{
			move_direction *= speedModifier;
			if (performLogging)
			{
				Logging.Log("SpeedHelper", $"Movement speed increased to {move_direction}");
			}
			orig.Invoke(self, move_direction);
		}

		private IEnumerator UnnBoost()
		{
			while (isUnnActive)
			{
				if (PlayerData.instance.GetBool("equippedCharm_28") && HeroController.instance.cState.focusing)
				{
					Rigidbody2D field = ClassIntegrations.GetField<HeroController, Rigidbody2D>(HeroController.instance, "rb2d");
					if (field.velocity.x == 0f)
					{
						applyUnn = true;
					}
					else if (applyUnn)
					{
						field.velocity = new Vector2(field.velocity.x * unnModifier, field.velocity.y);
						if (performLogging)
						{
							Logging.Log("SpeedHelper", $"Unn speed set to {field.velocity}");
						}
						applyUnn = false;
					}
				}
				else
				{
					applyUnn = true;
				}
				yield return (object)new WaitForSeconds(Time.deltaTime);
			}
			applyUnn = true;
		}
	}
	public class StaggerHelper
	{
		public int maxModifier { get; set; }

		public int comboModifier { get; set; }

		public bool performLogging { get; set; }

		public StaggerHelper(int maxModifier, int comboModifier = 0, bool performLogging = false)
		{
			this.maxModifier = maxModifier;
			this.comboModifier = comboModifier;
			this.performLogging = performLogging;
		}

		public void Start()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			IntCompare.OnEnter += new hook_OnEnter(ExtraStagger);
		}

		public void Stop()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			IntCompare.OnEnter -= new hook_OnEnter(ExtraStagger);
		}

		private void ExtraStagger(orig_OnEnter orig, IntCompare self)
		{
			if (((FsmStateAction)self).Fsm.Name.Equals("Stun") || ((FsmStateAction)self).Fsm.Name.Equals("Stun Control"))
			{
				int value = self.integer2.Value;
				if (((FsmStateAction)self).State.Name.Equals("Max Check") || ((FsmStateAction)self).State.Name.Equals("Continue Combo"))
				{
					FsmInt integer = self.integer2;
					integer.Value -= maxModifier;
					if (self.integer2.Value < 1)
					{
						self.integer2.Value = 1;
					}
					if (performLogging)
					{
						Logging.Log("StaggerHelper", $"Max stun reduced by {maxModifier} to {self.integer2.Value}");
					}
				}
				else if (((FsmStateAction)self).State.Name.Equals("In Combo"))
				{
					FsmInt integer2 = self.integer2;
					integer2.Value -= comboModifier;
					if (self.integer2.Value < 1)
					{
						self.integer2.Value = 1;
					}
					if (performLogging)
					{
						Logging.Log("StaggerHelper", $"Combo stun reduced by {comboModifier} to {self.integer2.Value}");
					}
				}
				orig.Invoke(self);
				self.integer2.Value = value;
				if (performLogging)
				{
					Logging.Log("StaggerHelper", $"Stun reset to {self.integer2.Value}");
				}
			}
			else
			{
				orig.Invoke(self);
			}
		}
	}
}
namespace DanielSteginkUtils.Helpers.Abilities
{
	public class DashHelper
	{
		public float dashModifier { get; set; } = 1f;

		public float darkDashModifier { get; set; } = 1f;

		public bool performLogging { get; set; } = true;

		public DashHelper(float dashModifier, float darkDashModifier = 1f, bool performLogging = false)
		{
			this.dashModifier = dashModifier;
			this.darkDashModifier = darkDashModifier;
			this.performLogging = performLogging;
		}

		public virtual void Start()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			HeroController.HeroDash += new hook_HeroDash(BuffDashCooldowns);
		}

		public virtual void Stop()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			HeroController.HeroDash -= new hook_HeroDash(BuffDashCooldowns);
		}

		private void BuffDashCooldowns(orig_HeroDash orig, HeroController self)
		{
			orig.Invoke(self);
			float dashCooldownTimer = HeroControllerR.dashCooldownTimer;
			float shadowDashTimer = HeroControllerR.shadowDashTimer;
			HeroControllerR.dashCooldownTimer *= dashModifier;
			if (performLogging)
			{
				Logging.Log("DashHelper", $"Dash cooldown: {dashCooldownTimer} -> {HeroControllerR.dashCooldownTimer}");
			}
			if (self.cState.shadowDashing)
			{
				HeroControllerR.shadowDashTimer *= darkDashModifier;
				if (performLogging)
				{
					Logging.Log("DashHelper", $"Shade Dash cooldown: {shadowDashTimer} -> {HeroControllerR.shadowDashTimer}");
				}
			}
		}
	}
	public class JumpHelper
	{
		public string modName { get; set; } = "";

		public string featureName { get; set; } = "";

		public int extraJumps { get; set; }

		public JumpHelper(string modName, string featureName, int extraJumps)
		{
			this.modName = modName;
			this.featureName = featureName;
			this.extraJumps = extraJumps;
		}

		public void Start()
		{
			ExtraJumps orAddComponent = UnityExtensions.GetOrAddComponent<ExtraJumps>(((Component)HeroController.instance).gameObject);
			if (!orAddComponent.ModList.ContainsKey(modName))
			{
				orAddComponent.ModList.Add(modName, new Dictionary<string, int>());
			}
			if (!orAddComponent.ModList[modName].ContainsKey(featureName))
			{
				orAddComponent.ModList[modName].Add(featureName, extraJumps);
			}
			orAddComponent.ModList[modName][featureName] = extraJumps;
		}

		public void Stop()
		{
			ExtraJumps orAddComponent = UnityExtensions.GetOrAddComponent<ExtraJumps>(((Component)HeroController.instance).gameObject);
			if (!orAddComponent.ModList.ContainsKey(modName))
			{
				orAddComponent.ModList.Add(modName, new Dictionary<string, int>());
			}
			if (orAddComponent.ModList[modName].ContainsKey(featureName))
			{
				orAddComponent.ModList[modName].Remove(featureName);
			}
		}
	}
}
namespace DanielSteginkUtils.Components
{
	public class BuffAspidAspect : ModBuffs<int>
	{
		public override int GetModdedValue()
		{
			return Calculations.GetModdedInt(BaseValue, GetModifier());
		}
	}
	public class ExtraJumps : MonoBehaviour
	{
		public Dictionary<string, Dictionary<string, int>> ModList = new Dictionary<string, Dictionary<string, int>>();

		private int jumpCounter { get; set; }

		public void OnEnable()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			HeroController.CanDoubleJump += new hook_CanDoubleJump(AllowExtraJump);
			HeroController.DoDoubleJump += new hook_DoDoubleJump(DoExtraJump);
			HeroController.BackOnGround += new hook_BackOnGround(ResetJump);
		}

		public void OnDisable()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			HeroController.CanDoubleJump -= new hook_CanDoubleJump(AllowExtraJump);
			HeroController.DoDoubleJump -= new hook_DoDoubleJump(DoExtraJump);
			HeroController.BackOnGround -= new hook_BackOnGround(ResetJump);
		}

		private bool AllowExtraJump(orig_CanDoubleJump orig, HeroController self)
		{
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			if (orig.Invoke(self))
			{
				return true;
			}
			ActorStates[] source = (ActorStates[])(object)new ActorStates[2]
			{
				(ActorStates)7,
				(ActorStates)5
			};
			bool flag = !self.inAcid && !self.cState.dashing && !self.cState.wallSliding && !self.cState.backDashing && !self.cState.attacking && !self.cState.bouncing && !self.cState.shroomBouncing && !self.cState.onGround;
			if ((PlayerData.instance.GetBool("hasDoubleJump") && !self.controlReqlinquished && !source.Contains(self.hero_state)) & flag)
			{
				return jumpCounter < GetTotalJumps();
			}
			return false;
		}

		private void DoExtraJump(orig_DoDoubleJump orig, HeroController self)
		{
			if (ClassIntegrations.GetField<HeroController, bool>(self, "doubleJumped"))
			{
				self.dJumpWingsPrefab.SetActive(false);
				self.dJumpFlashPrefab.SetActive(false);
				jumpCounter++;
			}
			orig.Invoke(self);
		}

		private void ResetJump(orig_BackOnGround orig, HeroController self)
		{
			jumpCounter = 0;
			orig.Invoke(self);
		}

		public int GetTotalJumps()
		{
			int num = 0;
			foreach (string key in ModList.Keys)
			{
				foreach (string key2 in ModList[key].Keys)
				{
					num += ModList[key][key2];
				}
			}
			return num;
		}
	}
	public abstract class ModBuffs<T> : MonoBehaviour
	{
		public Dictionary<string, Dictionary<string, float>> ModList = new Dictionary<string, Dictionary<string, float>>();

		public T BaseValue;

		public float GetModifier()
		{
			float num = 1f;
			foreach (string key in ModList.Keys)
			{
				foreach (string key2 in ModList[key].Keys)
				{
					num *= ModList[key][key2];
				}
			}
			return num;
		}

		public abstract T GetModdedValue();
	}
	public class BuffSporeDamage : ModBuffs<float>
	{
		public override float GetModdedValue()
		{
			return BaseValue * GetModifier();
		}
	}
}
namespace DanielSteginkUtils.Components.Dung
{
	public class BuffDungDamage : ModBuffs<float>
	{
		public override float GetModdedValue()
		{
			return BaseValue * GetModifier();
		}
	}
	public class BuffDungFluke : ModBuffs<float>
	{
		public override float GetModdedValue()
		{
			return BaseValue * GetModifier();
		}
	}
	public class BuffDungSize : ModBuffs<Vector3>
	{
		public override Vector3 GetModdedValue()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return BaseValue * GetModifier();
		}
	}
	public class BuffSporeSize : ModBuffs<Vector3>
	{
		public override Vector3 GetModdedValue()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return BaseValue * GetModifier();
		}
	}
}
