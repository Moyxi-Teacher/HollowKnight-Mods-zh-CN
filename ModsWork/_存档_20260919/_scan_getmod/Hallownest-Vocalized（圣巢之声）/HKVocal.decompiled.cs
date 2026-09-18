using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Core.FsmUtil;
using Core.FsmUtil.Actions;
using Core.FsmUtil.delegates;
using FrogCore;
using FrogCore.Ext;
using GlobalEnums;
using HKMirror;
using HKMirror.Hooks.OnHooks;
using HKMirror.Reflection;
using HKMirror.Reflection.InstanceClasses;
using HKVocals.EasterEggs;
using HKVocals.MajorFeatures;
using HKVocals.UI;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using JetBrains.Annotations;
using Language;
using Microsoft.CodeAnalysis;
using Modding;
using Modding.Delegates;
using MonoMod.RuntimeDetour;
using Newtonsoft.Json;
using On;
using SFCore;
using SFCore.MonoBehaviours;
using Satchel;
using Satchel.BetterMenus;
using Satchel.BetterMenus.Config;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: AssemblyCompany("HKVocal")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyFileVersion("0.0.1.4")]
[assembly: AssemblyInformationalVersion("0.0.1.4汉化")]
[assembly: AssemblyProduct("HKVocal")]
[assembly: AssemblyTitle("HKVocals")]
[assembly: AssemblyVersion("0.0.1.4")]
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
namespace HKVocals
{
	public static class CreditsLoader
	{
		private static AssetBundle creditsBundle;

		public static void LoadAssetBundle()
		{
			creditsBundle = AssetBundle.LoadFromMemory(AssemblyUtils.GetBytesFromResources("Resources.creditsbundle"));
		}
	}
	public static class MixerLoader
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static SetMusicLevel_AfterArgs <0>__SetDampenExcludeMusicVolume;

			public static SetSoundLevel_AfterArgs <1>__SetDampenExcludeSFXVolume;
		}

		public static AssetBundle mixerBundle;

		public static AudioMixerGroup HKVAudioGroup;

		public static AudioMixerGroup DampenExcludeMusic;

		public static AudioMixerGroup DampenExcludeSFX;

		private static AudioMixer HKVMixer;

		private static readonly Dictionary<string, AudioMixerSnapshot> AudioMixerSnapshots = new Dictionary<string, AudioMixerSnapshot>();

		private static readonly Dictionary<string, Snapshots> SnapshotsByScene = new Dictionary<string, Snapshots>
		{
			{
				"Opening_Sequence",
				Snapshots.Cave
			},
			{
				"Tutorial_01",
				Snapshots.Cave
			},
			{
				"Town",
				Snapshots.Cliffs
			},
			{
				"Room_Town_Stag_Station",
				Snapshots.Cave
			},
			{
				"Room_Charm_Shop",
				Snapshots.Room
			},
			{
				"Room_Mender_House",
				Snapshots.Room
			},
			{
				"Room_mapper",
				Snapshots.Room
			},
			{
				"Room_nailmaster",
				Snapshots.Room
			},
			{
				"Room_nailmaster_02",
				Snapshots.Room
			},
			{
				"Room_nailmaster_03",
				Snapshots.Cave
			},
			{
				"Room_nailsmith",
				Snapshots.Room
			},
			{
				"Room_shop",
				Snapshots.Room
			},
			{
				"Room_Sly_Storeroom",
				Snapshots.Room
			},
			{
				"Room_temple",
				Snapshots.Arena
			},
			{
				"Room_ruinhouse",
				Snapshots.Room
			},
			{
				"Room_Mask_Maker",
				Snapshots.Cave
			},
			{
				"Room_Mansion",
				Snapshots.Room
			},
			{
				"Room_Bretta",
				Snapshots.Room
			},
			{
				"Room_Bretta_Basement",
				Snapshots.Room
			},
			{
				"Room_Ouiji",
				Snapshots.Cave
			},
			{
				"Room_Jinn",
				Snapshots.Cave
			},
			{
				"Room_Colosseum_01",
				Snapshots.Cave
			},
			{
				"Room_Colosseum_02",
				Snapshots.Cave
			},
			{
				"Room_Slug_Shrine",
				Snapshots.Room
			},
			{
				"Crossroads_10",
				Snapshots.Cave
			},
			{
				"Crossroads_11_alt",
				Snapshots.Cave
			},
			{
				"Crossroads_33",
				Snapshots.Cave
			},
			{
				"Crossroads_45",
				Snapshots.Cave
			},
			{
				"Crossroads_46",
				Snapshots.Cave
			},
			{
				"Crossroads_46b",
				Snapshots.Cave
			},
			{
				"Crossroads_ShamanTemple",
				Snapshots.Cave
			},
			{
				"Crossroads_47",
				Snapshots.Cave
			},
			{
				"Crossroads_50",
				Snapshots.Cave
			},
			{
				"Crossroads_52",
				Snapshots.Cave
			},
			{
				"Ruins_House_03",
				Snapshots.Room
			},
			{
				"Ruins_Elevator",
				Snapshots.Room
			},
			{
				"Ruins_Bathhouse",
				Snapshots.Room
			},
			{
				"Ruins1_02",
				Snapshots.Cave
			},
			{
				"Ruins1_04",
				Snapshots.Cave
			},
			{
				"Ruins1_05b",
				Snapshots.Cave
			},
			{
				"Ruins1_06",
				Snapshots.Cave
			},
			{
				"Ruins1_23",
				Snapshots.Cave
			},
			{
				"Ruins1_24",
				Snapshots.Cave
			},
			{
				"Ruins1_27",
				Snapshots.Cliffs
			},
			{
				"Ruins1_29",
				Snapshots.Cave
			},
			{
				"Ruins1_31",
				Snapshots.Cave
			},
			{
				"Ruins1_32",
				Snapshots.Cave
			},
			{
				"Ruins2_04",
				Snapshots.Cliffs
			},
			{
				"Ruins2_08",
				Snapshots.Cave
			},
			{
				"Ruins2_11",
				Snapshots.Arena
			},
			{
				"Ruins2_Watcher_Room",
				Snapshots.Arena
			},
			{
				"Fungus1_04",
				Snapshots.Cave
			},
			{
				"Fungus1_06",
				Snapshots.Cave
			},
			{
				"Fungus1_08",
				Snapshots.Cave
			},
			{
				"Fungus1_13",
				Snapshots.Cave
			},
			{
				"Fungus1_16_alt",
				Snapshots.Cave
			},
			{
				"Fungus1_17",
				Snapshots.Cave
			},
			{
				"Fungus1_19",
				Snapshots.Cave
			},
			{
				"Fungus1_20_v02",
				Snapshots.Cave
			},
			{
				"Fungus1_21",
				Snapshots.Cave
			},
			{
				"Fungus1_24",
				Snapshots.Cave
			},
			{
				"Fungus1_30",
				Snapshots.Cave
			},
			{
				"Fungus1_32",
				Snapshots.Cave
			},
			{
				"Fungus1_35",
				Snapshots.Arena
			},
			{
				"Fungus2_01",
				Snapshots.Cave
			},
			{
				"Fungus2_02",
				Snapshots.Cave
			},
			{
				"Fungus2_04",
				Snapshots.Cave
			},
			{
				"Fungus2_07",
				Snapshots.Cave
			},
			{
				"Fungus2_09",
				Snapshots.Cave
			},
			{
				"Fungus2_12",
				Snapshots.Cave
			},
			{
				"Fungus2_14",
				Snapshots.Cave
			},
			{
				"Fungus2_18",
				Snapshots.Cave
			},
			{
				"Fungus2_20",
				Snapshots.Cave
			},
			{
				"Fungus2_21",
				Snapshots.Cave
			},
			{
				"Fungus2_23",
				Snapshots.Cave
			},
			{
				"Fungus2_25",
				Snapshots.Cave
			},
			{
				"Fungus2_26",
				Snapshots.Cave
			},
			{
				"Fungus2_30",
				Snapshots.Cave
			},
			{
				"Fungus2_32",
				Snapshots.Cave
			},
			{
				"Fungus2_34",
				Snapshots.Cave
			},
			{
				"Fungus3_23",
				Snapshots.Cave
			},
			{
				"Fungus3_25",
				Snapshots.Fog_Canyon
			},
			{
				"Fungus3_34",
				Snapshots.Cave
			},
			{
				"Fungus3_35",
				Snapshots.Cave
			},
			{
				"Fungus3_39",
				Snapshots.Cave
			},
			{
				"Fungus3_40",
				Snapshots.Cave
			},
			{
				"Fungus3_44",
				Snapshots.Cave
			},
			{
				"Fungus3_47",
				Snapshots.Fog_Canyon
			},
			{
				"Fungus3_49",
				Snapshots.Cave
			},
			{
				"Fungus3_archive_02",
				Snapshots.Cave
			},
			{
				"Cliffs_01",
				Snapshots.Room
			},
			{
				"Cliffs_02",
				Snapshots.Cliffs
			},
			{
				"Cliffs_03",
				Snapshots.Cave
			},
			{
				"Cliffs_05",
				Snapshots.Cave
			},
			{
				"Cliffs_06",
				Snapshots.Cave
			},
			{
				"RestingGrounds_02",
				Snapshots.Cave
			},
			{
				"RestingGrounds_04",
				Snapshots.Arena
			},
			{
				"RestingGrounds_07",
				Snapshots.Cave
			},
			{
				"RestingGrounds_08",
				Snapshots.Cave
			},
			{
				"RestingGrounds_09",
				Snapshots.Cave
			},
			{
				"Mines_13",
				Snapshots.Cave
			},
			{
				"Mines_30",
				Snapshots.Cave
			},
			{
				"Deepnest_09",
				Snapshots.Cave
			},
			{
				"Deepnest_14",
				Snapshots.Cave
			},
			{
				"Deepnest_30",
				Snapshots.Spa
			},
			{
				"Deepnest_33",
				Snapshots.Cave
			},
			{
				"Deepnest_40",
				Snapshots.Cave
			},
			{
				"Deepnest_41",
				Snapshots.Cave
			},
			{
				"Deepnest_44",
				Snapshots.Cave
			},
			{
				"Deepnest_Spider_Town",
				Snapshots.Cave
			},
			{
				"Room_spider_small",
				Snapshots.Cave
			},
			{
				"Deepnest_01b",
				Snapshots.Cave
			},
			{
				"Deepnest_East_01",
				Snapshots.Cave
			},
			{
				"Deepnest_East_03",
				Snapshots.Arena
			},
			{
				"Deepnest_East_04",
				Snapshots.Cave
			},
			{
				"Deepnest_East_10",
				Snapshots.Cave
			},
			{
				"Deepnest_East_17",
				Snapshots.Cave
			},
			{
				"Deepnest_East_Hornet",
				Snapshots.Arena
			},
			{
				"Abyss_03",
				Snapshots.Cave
			},
			{
				"Abyss_03_b",
				Snapshots.Cave
			},
			{
				"Abyss_03_c",
				Snapshots.Cave
			},
			{
				"Abyss_04",
				Snapshots.Cave
			},
			{
				"Abyss_05",
				Snapshots.Arena
			},
			{
				"Abyss_06_Core",
				Snapshots.Cave
			},
			{
				"Abyss_09",
				Snapshots.Arena
			},
			{
				"Abyss_15",
				Snapshots.Arena
			},
			{
				"Abyss_17",
				Snapshots.Cave
			},
			{
				"Abyss_21",
				Snapshots.Cave
			},
			{
				"Abyss_22",
				Snapshots.Cave
			},
			{
				"Room_Queen",
				Snapshots.Cave
			},
			{
				"Waterways_03",
				Snapshots.Sewerpipe
			},
			{
				"Waterways_05",
				Snapshots.Cave
			},
			{
				"Waterways_07",
				Snapshots.Sewerpipe
			},
			{
				"Waterways_09",
				Snapshots.Cave
			},
			{
				"Waterways_15",
				Snapshots.Cave
			},
			{
				"White_Palace_08",
				Snapshots.Cave
			},
			{
				"White_Palace_09",
				Snapshots.Arena
			},
			{
				"White_Palace_13",
				Snapshots.Arena
			},
			{
				"White_Palace_18",
				Snapshots.Arena
			},
			{
				"Hive_05",
				Snapshots.Cave
			},
			{
				"Grimm_Divine",
				Snapshots.Room
			},
			{
				"Grimm_Main_Tent",
				Snapshots.Cave
			},
			{
				"Dream_Nailcollection",
				Snapshots.Dream
			},
			{
				"Dream_01_False_Knight",
				Snapshots.Cave
			},
			{
				"Dream_02_Mage_Lord",
				Snapshots.Cave
			},
			{
				"Dream_04_White_Defender",
				Snapshots.Cave
			},
			{
				"Dream_Mighty_Zote",
				Snapshots.Room
			},
			{
				"Dream_Backer_Shrine",
				Snapshots.Dream
			},
			{
				"Dream_Room_Believer_Shrine",
				Snapshots.Dream
			},
			{
				"Dream_Abyss",
				Snapshots.Arena
			},
			{
				"Dream_Final_Boss",
				Snapshots.Arena
			},
			{
				"Room_Final_Boss_Atrium",
				Snapshots.Arena
			},
			{
				"GG_Waterways",
				Snapshots.Arena
			},
			{
				"GG_Brooding_Mawlek",
				Snapshots.Cave
			},
			{
				"GG_Collector",
				Snapshots.Cave
			},
			{
				"GG_Crystal_Guardian",
				Snapshots.Cave
			},
			{
				"GG_Crystal_Guardian_2",
				Snapshots.Cave
			},
			{
				"GG_Dung_Defender",
				Snapshots.Cave
			},
			{
				"GG_Failed_Champion",
				Snapshots.Arena
			},
			{
				"GG_False_Knight",
				Snapshots.Cave
			},
			{
				"GG_Flukemarm",
				Snapshots.Sewerpipe
			},
			{
				"GG_God_Tamer",
				Snapshots.Cave
			},
			{
				"GG_Grey_Prince_Zote",
				Snapshots.Cave
			},
			{
				"GG_Grimm",
				Snapshots.Cave
			},
			{
				"GG_Grimm_Nightmare",
				Snapshots.Arena
			},
			{
				"GG_Gruz_Mother",
				Snapshots.Cave
			},
			{
				"GG_Hive_Knight",
				Snapshots.Cave
			},
			{
				"GG_Hornet_1",
				Snapshots.Cave
			},
			{
				"GG_Hornet_2",
				Snapshots.Cave
			},
			{
				"GG_Lurker",
				Snapshots.Cave
			},
			{
				"GG_Mantis_Lords",
				Snapshots.Cave
			},
			{
				"GG_Mega_Moss_Charger",
				Snapshots.Cave
			},
			{
				"GG_Nailmasters",
				Snapshots.Cave
			},
			{
				"GG_Oblobbles",
				Snapshots.Cave
			},
			{
				"GG_Painter",
				Snapshots.Cave
			},
			{
				"GG_Radiance",
				Snapshots.Arena
			},
			{
				"GG_Sly",
				Snapshots.Arena
			},
			{
				"GG_Soul_Master",
				Snapshots.Cave
			},
			{
				"GG_Soul_Tyrant",
				Snapshots.Cave
			},
			{
				"GG_Traitor_Lord",
				Snapshots.Cave
			},
			{
				"GG_Uumuu",
				Snapshots.Cave
			},
			{
				"GG_Vengefly",
				Snapshots.Arena
			},
			{
				"GG_Watcher_Knights",
				Snapshots.Arena
			},
			{
				"GG_White_Defender",
				Snapshots.Cave
			},
			{
				"GG_Workshop",
				Snapshots.Cave
			},
			{
				"Room_GG_Shortcut",
				Snapshots.Arena
			},
			{
				"GG_Engine",
				Snapshots.Cave
			},
			{
				"GG_Engine_Prime",
				Snapshots.Arena
			},
			{
				"GG_Engine_Root",
				Snapshots.Arena
			},
			{
				"GG_Mage_Knight",
				Snapshots.Cave
			},
			{
				"GG_Vengefly_V",
				Snapshots.Arena
			},
			{
				"GG_Entrance_Cutscene",
				Snapshots.Arena
			},
			{
				"GG_Boss_Door_Entrance",
				Snapshots.Arena
			},
			{
				"GG_Mighty_Zote",
				Snapshots.Cave
			},
			{
				"GG_Gruz_Mother_V",
				Snapshots.Cave
			},
			{
				"GG_Brooding_Mawlek_V",
				Snapshots.Cave
			},
			{
				"GG_Mantis_Lords_V",
				Snapshots.Arena
			},
			{
				"GG_Uumuu_V",
				Snapshots.Cave
			},
			{
				"GG_Mage_Knight_V",
				Snapshots.Cave
			},
			{
				"GG_Collector_V",
				Snapshots.Cave
			},
			{
				"GG_Wyrm",
				Snapshots.Arena
			},
			{
				"GG_Unn",
				Snapshots.Arena
			},
			{
				"GG_Unlock_Wastes",
				Snapshots.Cliffs
			},
			{
				"hidden_reward_room",
				Snapshots.Cave
			}
		};

		public static void LoadAssetBundle()
		{
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Expected O, but got Unknown
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Expected O, but got Unknown
			mixerBundle = AssetBundle.LoadFromMemory(AssemblyUtils.GetBytesFromResources("Resources.mixerbundle"));
			mixerBundle.LoadAllAssets(typeof(AudioMixer)).ForEach(delegate(Object x)
			{
				HKVMixer = (AudioMixer)(object)((x is AudioMixer) ? x : null);
				Object.DontDestroyOnLoad((Object)(object)HKVMixer);
			});
			mixerBundle.LoadAllAssets(typeof(AudioMixerGroup)).ForEach(delegate(Object x)
			{
				if (x.name == "HKV AudioMixerGroup")
				{
					HKVAudioGroup = (AudioMixerGroup)(object)((x is AudioMixerGroup) ? x : null);
					Object.DontDestroyOnLoad((Object)(object)HKVAudioGroup);
				}
				else if (x.name == "Dampen Exclude Music")
				{
					DampenExcludeMusic = (AudioMixerGroup)(object)((x is AudioMixerGroup) ? x : null);
					Object.DontDestroyOnLoad((Object)(object)DampenExcludeMusic);
				}
				else if (x.name == "Dampen Exclude SFX")
				{
					DampenExcludeSFX = (AudioMixerGroup)(object)((x is AudioMixerGroup) ? x : null);
					Object.DontDestroyOnLoad((Object)(object)DampenExcludeSFX);
				}
			});
			string[] names = Enum.GetNames(typeof(Snapshots));
			foreach (string text in names)
			{
				AudioMixerSnapshots[text] = HKVMixer.FindSnapshot(text);
			}
			HKVMixer.SetFloat("Dampen Exclude Music Volume", MiscUtils.GetDecibelVolume(GameManager.instance.gameSettings.musicVolume));
			HKVMixer.SetFloat("Dampen Exclude SFX Volume", MiscUtils.GetDecibelVolume(GameManager.instance.gameSettings.soundVolume));
			object obj = <>O.<0>__SetDampenExcludeMusicVolume;
			if (obj == null)
			{
				SetMusicLevel_AfterArgs val = SetDampenExcludeMusicVolume;
				<>O.<0>__SetDampenExcludeMusicVolume = val;
				obj = (object)val;
			}
			AfterOrig.SetMusicLevel += (SetMusicLevel_AfterArgs)obj;
			object obj2 = <>O.<1>__SetDampenExcludeSFXVolume;
			if (obj2 == null)
			{
				SetSoundLevel_AfterArgs val2 = SetDampenExcludeSFXVolume;
				<>O.<1>__SetDampenExcludeSFXVolume = val2;
				obj2 = (object)val2;
			}
			AfterOrig.SetSoundLevel += (SetSoundLevel_AfterArgs)obj2;
		}

		public static void SetSnapshot(Snapshots snapshot)
		{
			AudioMixerSnapshots[snapshot.ToString()].TransitionTo(0.1f);
		}

		public static void SetSnapshot(string sceneName)
		{
			if (SnapshotsByScene.TryGetValue(sceneName, out var value))
			{
				SetSnapshot(value);
			}
			else
			{
				SetSnapshot(Snapshots.No_Effect);
			}
		}

		public static void SetMixerVolume()
		{
			HKVMixer.SetFloat("VA Volume", MiscUtils.GetDecibelVolume(HKVocals._globalSettings.volume));
		}

		public static void SetMixerGroup(this AudioSource source)
		{
			if ((Object)(object)source.outputAudioMixerGroup == (Object)null || (Object)(object)source.outputAudioMixerGroup != (Object)(object)HKVAudioGroup)
			{
				source.outputAudioMixerGroup = HKVAudioGroup;
			}
		}

		private static void SetDampenExcludeMusicVolume(Params_SetMusicLevel args)
		{
			HKVMixer.SetFloat("Dampen Exclude Music Volume", MiscUtils.GetDecibelVolume(args.musicLevel));
		}

		private static void SetDampenExcludeSFXVolume(Params_SetSoundLevel args)
		{
			HKVMixer.SetFloat("Dampen Exclude SFX Volume", MiscUtils.GetDecibelVolume(args.soundLevel));
		}
	}
	public enum Snapshots
	{
		No_Effect,
		Dream,
		Cave,
		Spa,
		Cliffs,
		Room,
		Arena,
		Sewerpipe,
		Fog_Canyon
	}
	public static class StyleLoader
	{
		public static AssetBundle styleBundle;

		public static bool loaded;

		public static void LoadAssetBundle()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			styleBundle = AssetBundle.LoadFromMemory(((int)SystemInfo.operatingSystemFamily == 3) ? AssemblyUtils.GetBytesFromResources("Resources.stylebundlelinux") : AssemblyUtils.GetBytesFromResources("Resources.stylebundle"));
			loaded = true;
		}
	}
	public static class AudioAPI
	{
		public static readonly string[] AudioExtensions = new string[2] { ".mp3", ".wav" };

		private static Dictionary<int, List<IAudioProvider>> AudioProviders = new Dictionary<int, List<IAudioProvider>>();

		public static string[] CachedAudioNames = new string[0];

		internal static List<string> CustomMuteList = new List<string>();

		public static List<string> AudioNames { get; } = new List<string>();

		public static IAudioProvider[] OrderedProviers { get; private set; } = new IAudioProvider[0];

		public static bool ShouldUpdateOrderedProviders { get; private set; } = false;

		public static bool ShouldUpdateCachedAudioNames { get; private set; } = false;

		public static void AddMuteAudio(string name)
		{
			CustomMuteList.Add(name);
		}

		public static void AddAudioProvider(int optimalPriority, IAudioProvider provider)
		{
			if (!AudioProviders.TryGetValue(optimalPriority, out var value) || value == null)
			{
				value = new List<IAudioProvider>();
				AudioProviders[optimalPriority] = value;
			}
			value.Add(provider);
			ShouldUpdateOrderedProviders = true;
			ShouldUpdateCachedAudioNames = true;
		}

		public static AudioClip GetAudioFor(string convName)
		{
			UpdateOrderedPriority();
			IAudioProvider[] orderedProviers = OrderedProviers;
			for (int i = 0; i < orderedProviers.Length; i++)
			{
				AudioClip audioClip = orderedProviers[i].GetAudioClip(convName);
				if (Object.op_Implicit((Object)(object)audioClip))
				{
					return audioClip;
				}
			}
			return null;
		}

		public static bool HasAudioFor(string convName)
		{
			UpdateAudioNames();
			if (!CachedAudioNames.Contains(convName))
			{
				return CachedAudioNames.Contains(convName.ToUpperInvariant());
			}
			return true;
		}

		public static void UpdateOrderedPriority()
		{
			if (ShouldUpdateOrderedProviders)
			{
				OrderedProviers = AudioProviders.OrderBy((KeyValuePair<int, List<IAudioProvider>> pair) => -pair.Key).SelectMany((KeyValuePair<int, List<IAudioProvider>> pair) => pair.Value).ToArray();
			}
		}

		public static void UpdateAudioNames()
		{
			if (ShouldUpdateCachedAudioNames || AudioProviders.SelectMany((KeyValuePair<int, List<IAudioProvider>> pair) => pair.Value).Any((IAudioProvider provider) => provider.UpdateAudioNames()))
			{
				CachedAudioNames = AudioProviders.SelectMany((KeyValuePair<int, List<IAudioProvider>> pair) => pair.Value).SelectMany((IAudioProvider provider) => provider.GetAudioNames()).ToArray();
			}
		}
	}
	public interface IAudioProvider
	{
		AudioClip GetAudioClip(string convName);

		IEnumerable<string> GetAudioNames();

		bool UpdateAudioNames();
	}
	public class AssetBundleAudioProvider : IAudioProvider
	{
		public AssetBundle Bundle { get; protected set; }

		public string[] AudioNames { get; protected set; } = new string[0];

		public bool HasCachedAudio { get; protected set; }

		public AssetBundleAudioProvider(AssetBundle bundle, bool autoCacheAudio = true)
		{
			Bundle = bundle;
			if (autoCacheAudio)
			{
				GetAudioNames();
			}
		}

		public virtual AudioClip GetAudioClip(string convName)
		{
			AudioClip val = Bundle.LoadAsset<AudioClip>(convName);
			if ((Object)(object)val == (Object)null)
			{
				val = Bundle.LoadAsset<AudioClip>(convName.ToLowerInvariant());
			}
			if ((Object)(object)val == (Object)null)
			{
				val = Bundle.LoadAsset<AudioClip>(convName.ToUpperInvariant());
			}
			return val;
		}

		public virtual IEnumerable<string> GetAudioNames()
		{
			if (!HasCachedAudio)
			{
				AudioNames = (from s in Bundle.GetAllAssetNames()
					where AudioAPI.AudioExtensions.Any((string ext) => s.EndsWith(ext))
					select Path.GetFileNameWithoutExtension(s).ToUpper()).ToArray();
			}
			return AudioNames;
		}

		public virtual bool UpdateAudioNames()
		{
			return !HasCachedAudio;
		}
	}
	public class AssetBundleCreateRequestAudioProvider : AssetBundleAudioProvider
	{
		public AssetBundleCreateRequest Request { get; protected set; }

		public bool AutoCacheAudioOnCompletion { get; protected set; }

		public AssetBundleCreateRequestAudioProvider(AssetBundleCreateRequest request, bool autoCacheAudioOnCompletion)
			: base(null, autoCacheAudio: false)
		{
			Request = request;
			AutoCacheAudioOnCompletion = autoCacheAudioOnCompletion;
			if (((AsyncOperation)Request).isDone)
			{
				OnBundleLoadComplete((AsyncOperation)(object)Request);
			}
			else
			{
				((AsyncOperation)Request).completed += OnBundleLoadComplete;
			}
		}

		public override AudioClip GetAudioClip(string convName)
		{
			if ((Object)(object)base.Bundle == (Object)null)
			{
				return null;
			}
			return base.GetAudioClip(convName);
		}

		public override IEnumerable<string> GetAudioNames()
		{
			if ((Object)(object)base.Bundle == (Object)null)
			{
				return new string[0];
			}
			return base.GetAudioNames();
		}

		private void OnBundleLoadComplete(AsyncOperation _)
		{
			base.Bundle = Request.assetBundle;
			if (AutoCacheAudioOnCompletion)
			{
				GetAudioNames();
			}
		}
	}
	public static class AudioPlayer
	{
		private static readonly Dictionary<string, AudioClip> _trimmedCache = new Dictionary<string, AudioClip>();

		private const int TrimCacheLimit = 64;

		public static bool TryPlayAudioFor(string convName, float removeTime = 0f, AudioSource asrc = null)
		{
			((Loggable)HKVocals.instance).LogDebug("Trying to play audio for " + convName);
			if (HasAudioFor(convName))
			{
				if (removeTime != 0f)
				{
					PlayAudioWithTrim(GetAudioClip(convName), removeTime);
					return true;
				}
				PlayAudio(GetAudioClip(convName), asrc);
				return true;
			}
			((Loggable)HKVocals.instance).LogWarn("Audio doesn't exist " + convName);
			return false;
		}

		private static AudioClip GetAudioClip(string convoName)
		{
			return GetAudioFor(convoName.ToLower());
		}

		private static void PlayAudioWithTrim(AudioClip clip, float removeTime)
		{
			if ((Object)(object)clip == (Object)null)
			{
				((Loggable)HKVocals.instance).LogWarn("PlayAudioWithTrim got a null clip");
				return;
			}
			string key = ((Object)clip).name + "|" + removeTime.ToString("F2");
			if (_trimmedCache.TryGetValue(key, out var value) && (Object)(object)value != (Object)null)
			{
				PlayAudio(value);
				return;
			}
			((Loggable)HKVocals.instance).LogDebug("Trimming " + ((Object)clip).name);
			int num = (int)((float)clip.frequency * removeTime);
			if (num <= 0 || num >= clip.samples)
			{
				PlayAudio(clip);
				return;
			}
			int num2 = clip.samples - num;
			float[] array = new float[num2 * clip.channels];
			if (!clip.GetData(array, num))
			{
				((Loggable)HKVocals.instance).LogWarn("Could not read samples of " + ((Object)clip).name + ", playing untrimmed");
				PlayAudio(clip);
				return;
			}
			AudioClip val = AudioClip.Create(((Object)clip).name, num2, clip.channels, clip.frequency, false);
			val.SetData(array, 0);
			if (_trimmedCache.Count >= 64)
			{
				foreach (AudioClip value2 in _trimmedCache.Values)
				{
					if ((Object)(object)value2 != (Object)null)
					{
						Object.Destroy((Object)(object)value2);
					}
				}
				_trimmedCache.Clear();
			}
			_trimmedCache[key] = val;
			PlayAudio(val);
		}

		private static void PlayAudio(AudioClip clip, AudioSource asrc = null)
		{
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)asrc == (Object)null)
			{
				asrc = HKVocals.instance.audioSource;
			}
			if ((Object)(object)asrc == (Object)null)
			{
				HKVocals.instance.CreateAudioSource();
				asrc = HKVocals.instance.audioSource;
			}
			if ((Object)(object)asrc == (Object)null)
			{
				((Loggable)HKVocals.instance).LogWarn("PlayAudio has no audio source available");
				return;
			}
			if ((Object)(object)clip == (Object)null)
			{
				((Loggable)HKVocals.instance).LogWarn("PlayAudio got a null clip");
				return;
			}
			CheckForAchivements(((Object)clip).name);
			asrc.Stop();
			((Component)asrc).transform.localPosition = (Vector3)(((Object)(object)HeroController.instance != (Object)null) ? ((Component)HeroController.instance).transform.localPosition : new Vector3(15f, 10f, 1f));
			asrc.SetMixerGroup();
			MixerLoader.SetMixerVolume();
			asrc.PlayOneShot(clip, 1f);
		}

		private static void CheckForAchivements(string clip)
		{
			HKVocals._globalSettings.FinishedNPCDialoge.RemoveAll((string v) => v.Equals(clip, StringComparison.OrdinalIgnoreCase));
			HKVocals._globalSettings.FinishedDNailDialoge.RemoveAll((string v) => v.Equals(clip, StringComparison.OrdinalIgnoreCase));
			HKVocals._globalSettings.FinishedLoreTabletDialoge.RemoveAll((string v) => v.Equals(clip, StringComparison.OrdinalIgnoreCase));
			HKVocals._globalSettings.FinishedUIDialoge.RemoveAll((string v) => v.Equals(clip, StringComparison.OrdinalIgnoreCase));
			if (!GameManager.instance.IsAchievementAwarded("Consideration") && HKVocals._globalSettings.FinishedNPCDialoge.Count == 0)
			{
				GameManager.instance.AwardAchievement("Consideration");
			}
			if (!GameManager.instance.IsAchievementAwarded("Ambition") && HKVocals._globalSettings.FinishedDNailDialoge.Count == 0)
			{
				GameManager.instance.AwardAchievement("Ambition");
			}
			if (!GameManager.instance.IsAchievementAwarded("Chronology") && HKVocals._globalSettings.FinishedLoreTabletDialoge.Count == 0)
			{
				GameManager.instance.AwardAchievement("Chronology");
			}
			if (!GameManager.instance.IsAchievementAwarded("Acquisition") && HKVocals._globalSettings.FinishedUIDialoge.Count == 0)
			{
				GameManager.instance.AwardAchievement("Acquisition");
			}
			if (GameManager.instance.IsAchievementAwarded("Consideration") && GameManager.instance.IsAchievementAwarded("Ambition") && GameManager.instance.IsAchievementAwarded("Chronology") && GameManager.instance.IsAchievementAwarded("Acquisition") && !GameManager.instance.IsAchievementAwarded("CompendiumVocalization"))
			{
				GameManager.instance.AwardAchievement("CompendiumVocalization");
			}
		}

		public static bool IsPlaying()
		{
			return HKVocals.instance.audioSource.isPlaying;
		}

		public static void StopPlaying()
		{
			HKVocals.instance.audioSource.Stop();
		}

		public static bool HasAudioFor(string convName)
		{
			return AudioAPI.HasAudioFor(convName);
		}

		public static AudioClip GetAudioFor(string convName)
		{
			return AudioAPI.GetAudioFor(convName);
		}
	}
	public static class FSMEditData
	{
		public static Dictionary<HKVocalsFsmData, Action<PlayMakerFSM>> FsmEdits = new Dictionary<HKVocalsFsmData, Action<PlayMakerFSM>>();

		public static void Add(HKVocalsFsmData fsmData, Action<PlayMakerFSM> action)
		{
			if (!FsmEdits.ContainsKey(fsmData))
			{
				FsmEdits.Add(fsmData, action);
				return;
			}
			Dictionary<HKVocalsFsmData, Action<PlayMakerFSM>> fsmEdits = FsmEdits;
			fsmEdits[fsmData] = (Action<PlayMakerFSM>)Delegate.Combine(fsmEdits[fsmData], action);
		}

		public static void AddSceneFsmEdit(string sceneName, string gameObjectName, string fsmName, Action<PlayMakerFSM> fsmEdits)
		{
			Add(new HKVocalsFsmData(sceneName, gameObjectName, fsmName), fsmEdits);
		}

		public static void AddGameObjectFsmEdit(string gameObjectName, string fsmName, Action<PlayMakerFSM> fsmEdits)
		{
			Add(new HKVocalsFsmData(gameObjectName, fsmName), fsmEdits);
		}

		public static void AddAnyFsmEdit(string fsmName, Action<PlayMakerFSM> fsmEdits)
		{
			Add(new HKVocalsFsmData(fsmName), fsmEdits);
		}

		public static void AddRange(Dictionary<HKVocalsFsmData, Action<PlayMakerFSM>> fsmEditsList)
		{
			HKVocalsFsmData hKVocalsFsmData = default(HKVocalsFsmData);
			Action<PlayMakerFSM> action = default(Action<PlayMakerFSM>);
			foreach (KeyValuePair<HKVocalsFsmData, Action<PlayMakerFSM>> fsmEdits in fsmEditsList)
			{
				DeconstructUtil.Deconstruct<HKVocalsFsmData, Action<PlayMakerFSM>>(fsmEdits, ref hKVocalsFsmData, ref action);
				HKVocalsFsmData fsmData = hKVocalsFsmData;
				Action<PlayMakerFSM> action2 = action;
				Add(fsmData, action2);
			}
		}
	}
	public record HKVocalsFsmData(string SceneName, string GameObjectName, string FsmName)
	{
		public HKVocalsFsmData(string GameObjectName, string FsmName)
			: this(null, GameObjectName, FsmName)
		{
		}

		public HKVocalsFsmData(string FsmName)
			: this(null, null, FsmName)
		{
		}
	}
	public class DreamDialogueAction : FsmStateAction
	{
		public enum ConvoMode
		{
			Once,
			Repeat,
			Random
		}

		public string[] convNames;

		public string[] sheetNames;

		public float waitTime;

		public ConvoMode convoMode;

		public int[] convoOccurances = new int[1];

		public float chance = 1f;

		private int lastIndex = int.MaxValue;

		private int realLastIndex = -1;

		public string convName
		{
			get
			{
				return convNames[0];
			}
			set
			{
				convNames = new string[1] { value };
			}
		}

		public string sheetName
		{
			get
			{
				return sheetNames[0];
			}
			set
			{
				sheetNames = new string[1] { value };
			}
		}

		public DreamDialogueAction(string convName, string sheetName)
		{
			this.convName = convName;
			this.sheetName = sheetName;
		}

		public DreamDialogueAction(string[] convNames, string sheetName)
		{
			this.convNames = convNames;
			this.sheetName = sheetName;
		}

		public DreamDialogueAction(List<(string, string)> Conversations)
		{
			convNames = Conversations.Select(((string, string) tup) => tup.Item1).ToArray();
			sheetNames = Conversations.Select(((string, string) tup) => tup.Item2).ToArray();
		}

		public override void OnEnter()
		{
			if (chance == 1f)
			{
				StartShow();
			}
			else if (Random.value <= chance)
			{
				StartShow();
			}
			if (((FsmStateAction)this).Fsm != null)
			{
				((FsmStateAction)this).Finish();
			}
		}

		private void StartShow()
		{
			realLastIndex++;
			int num = ((convoOccurances.Length > realLastIndex) ? convoOccurances[realLastIndex] : convoOccurances.Last());
			if (num != -1)
			{
				if (waitTime == 0f)
				{
					ShowDialogue();
				}
				else
				{
					((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(WaitShowDialogue());
				}
			}
		}

		private IEnumerator WaitShowDialogue()
		{
			yield return (object)new WaitForSeconds(waitTime);
			ShowDialogue();
		}

		private void ShowDialogue()
		{
			switch (convoMode)
			{
			case ConvoMode.Once:
				if (lastIndex > convNames.Length - 2)
				{
					return;
				}
				if (lastIndex == int.MaxValue || lastIndex < -1)
				{
					lastIndex = -1;
				}
				lastIndex++;
				break;
			case ConvoMode.Repeat:
				if (lastIndex == int.MaxValue || lastIndex > convNames.Length - 2 || lastIndex < -1)
				{
					lastIndex = -1;
				}
				lastIndex++;
				break;
			case ConvoMode.Random:
				lastIndex = Random.Range(0, convNames.Length);
				break;
			}
			FSMEditUtils.CreateDreamDialogue((convNames.Length > lastIndex) ? convNames[lastIndex] : convNames.Last(), (sheetNames.Length == 0) ? "Enemy Dreams" : ((sheetNames.Length > lastIndex) ? sheetNames[lastIndex] : sheetNames.Last()));
		}
	}
	public sealed class HKVocals : Mod, IGlobalSettings<GlobalSettings>, ILocalSettings<SaveSettings>, ICustomMenuMod, IMod, ILogger, ITogglableMod
	{
		public static Sprite Icon;

		public AudioSource audioSource;

		internal static HKVocals instance;

		public static NonBouncer CoroutineHolder;

		private static string Version = "0.0.1.4";

		public static GlobalSettings _globalSettings { get; private set; } = new GlobalSettings();

		public static SaveSettings _saveSettings { get; private set; } = new SaveSettings();

		public bool ToggleButtonInsideMenu => true;

		public void OnLoadGlobal(GlobalSettings s)
		{
			_globalSettings = s;
		}

		public GlobalSettings OnSaveGlobal()
		{
			return _globalSettings;
		}

		public void OnLoadLocal(SaveSettings s)
		{
			_saveSettings = s;
		}

		public SaveSettings OnSaveLocal()
		{
			return _saveSettings;
		}

		public override List<(string, string)> GetPreloadNames()
		{
			return new List<(string, string)> { ("Room_shop", "_SceneManager") };
		}

		public HKVocals()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			((Mod)this)..ctor("圣巢之声");
			UIManager.Start += new hook_Start(AddIcon);
			Achievements.Hook();
		}

		public override string GetVersion()
		{
			return Version ?? "";
		}

		public void Unload()
		{
			if (instance == this)
			{
				instance = null;
			}
		}

		public override void Initialize()
		{
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Expected O, but got Unknown
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			instance = this;
			MixerLoader.LoadAssetBundle();
			SpecialAudio.Hook();
			NPCDialogue.Hook();
			MuteOriginalAudio.Hook();
			DampenAudio.Hook();
			DreamNailDialogue.Hook();
			AutoScroll.Hook();
			ScrollLock.Hook();
			AutomaticBossDialogue.Hook();
			UITextAudio.Hook();
			Patches.Hook();
			EternalOrdeal.Hook();
			SpecialGrub.Hook();
			PaleFlower.Hook();
			UIManager.EditMenus += AudioMenu.AddAudioSliderAndSettingsButton;
			UIManager.EditMenus += SettingsPrompt.CreatePrompt;
			SettingsPrompt.HookRemoveButton();
			Hooks.PmFsmBeforeStartHook += new FsmModificationHandler(AddFSMEdits);
			CoroutineHolder = new GameObject("HK Vocals Coroutine Holder").AddComponent<NonBouncer>();
			Object.DontDestroyOnLoad((Object)(object)CoroutineHolder);
			CreateAudioSource();
			InitAchievements();
			((Loggable)this).Log("HKVocals initialized");
		}

		public void CreateAudioSource()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			((Loggable)this).LogDebug("creating new asrc");
			GameObject val = new GameObject("HK Vocals Audio");
			audioSource = val.AddComponent<AudioSource>();
			audioSource.SetMixerGroup();
			Object.DontDestroyOnLoad((Object)(object)val);
		}

		private void AddFSMEdits(PlayMakerFSM fsm)
		{
			string currentSceneName = MiscUtils.GetCurrentSceneName();
			string name = ((Object)((Component)fsm).gameObject).name;
			string fsmName = fsm.FsmName;
			if (FSMEditData.FsmEdits.TryGetValue(new HKVocalsFsmData(currentSceneName, name, fsmName), out var value))
			{
				value.TryInvokeActions(fsm);
			}
			if (FSMEditData.FsmEdits.TryGetValue(new HKVocalsFsmData(name, fsmName), out var value2))
			{
				value2.TryInvokeActions(fsm);
			}
			if (FSMEditData.FsmEdits.TryGetValue(new HKVocalsFsmData(fsmName), out var value3))
			{
				value3.TryInvokeActions(fsm);
			}
		}

		private void InitAchievements()
		{
			if (_globalSettings.FinishedUIDialoge == null)
			{
				_globalSettings.FinishedUIDialoge = JsonConvert.DeserializeObject<List<string>>(Encoding.Default.GetString(AssemblyUtils.GetBytesFromResources("Resources.AchievementKeys.Inventory_KEYs.json")));
			}
			if (_globalSettings.FinishedNPCDialoge == null)
			{
				_globalSettings.FinishedNPCDialoge = JsonConvert.DeserializeObject<List<string>>(Encoding.Default.GetString(AssemblyUtils.GetBytesFromResources("Resources.AchievementKeys.NPCs_KEYs.json")));
			}
			if (_globalSettings.FinishedDNailDialoge == null)
			{
				_globalSettings.FinishedDNailDialoge = JsonConvert.DeserializeObject<List<string>>(Encoding.Default.GetString(AssemblyUtils.GetBytesFromResources("Resources.AchievementKeys.Dream_Nail_KEYs.json")));
			}
			if (_globalSettings.FinishedLoreTabletDialoge == null)
			{
				_globalSettings.FinishedLoreTabletDialoge = JsonConvert.DeserializeObject<List<string>>(Encoding.Default.GetString(AssemblyUtils.GetBytesFromResources("Resources.AchievementKeys.Lore_Tablet_KEYs.json")));
			}
		}

		private void AddIcon(orig_Start orig, UIManager self)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Invalid comparison between Unknown and I4
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self);
			if ((int)SystemInfo.operatingSystemFamily != 3)
			{
				_ = CultureInfo.CurrentCulture.Name;
			}
			else
			{
				Environment.GetEnvironmentVariable("LANG");
			}
			if (Object.op_Implicit((Object)(object)Icon))
			{
				GameObject gameObject = ((Component)((Component)self).transform.Find("UICanvas/MainMenuScreen/TeamCherryLogo/Hidden_Dreams_Logo")).gameObject;
				GameObject obj = Object.Instantiate<GameObject>(gameObject, gameObject.transform.parent);
				obj.SetActive(true);
				Vector3 position = obj.transform.position;
				obj.transform.position = position + new Vector3(3.2f, -0.111f, 0f);
				Extensions.SetScaleX(obj.transform, 233f);
				Extensions.SetScaleY(obj.transform, 233f);
				obj.GetComponent<SpriteRenderer>().sprite = Icon;
			}
		}

		public static void DoLogDebug(object s)
		{
			((Loggable)instance).LogDebug(s);
		}

		public static void DoLog(object s)
		{
			((Loggable)instance).Log(s);
		}

		public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
		{
			return ModMenu.CreateModMenuScreen(modListMenu);
		}
	}
	public class CancelDreamDialogueOnDeath : MonoBehaviour
	{
		private void OnDestroy()
		{
			FsmGameObject fsmGameObject = FsmVariables.GlobalVariables.GetFsmGameObject("Enemy Dream Msg");
			if (fsmGameObject == null || !((Object)(object)fsmGameObject.Value != (Object)null))
			{
				return;
			}
			PlayMakerFSM val = FSMUtility.LocateMyFSM(fsmGameObject.Value, "Display");
			if ((Object)(object)val != (Object)null)
			{
				FsmGameObject fsmGameObject2 = val.FsmVariables.GetFsmGameObject("Last Enemy");
				if (fsmGameObject2 != null && (Object)(object)fsmGameObject2.Value != (Object)null && (Object)(object)fsmGameObject2.Value == (Object)(object)((Component)this).gameObject)
				{
					val.SendEvent("CANCEL ENEMY DREAM");
				}
			}
		}
	}
	public class GlobalSettings
	{
		public int volume = 10;

		public bool ordealZoteSpeak = true;

		public bool autoScroll;

		public AutoScroll.ScrollSpeed scrollSpeed = AutoScroll.ScrollSpeed.Normal;

		public bool dnDialogue = true;

		public bool automaticBossDialogue;

		public bool scrollLock;

		public bool dampenAudio;

		public bool settingsOpened;

		public bool shopAudio = true;

		public bool invAudio = true;

		public List<string> FinishedNPCDialoge;

		public bool GotNPCAchievement;

		public List<string> FinishedDNailDialoge;

		public bool GotDNailAchievement;

		public List<string> FinishedLoreTabletDialoge;

		public bool GotLoreTabletAchievement;

		public List<string> FinishedUIDialoge;

		public bool GotUIAchievement;

		public bool GotHJAchievement;

		public bool ForceMenuTheme = true;

		public bool dampenLocked;

		public bool DevMode;
	}
	public class SaveSettings
	{
		public int[] GhostRelics = new int[4];

		public bool UsedRelicBox;

		public bool RelicBoxConvo;

		public bool LurkerFlower;

		public int LurkerConversation = -1;

		public int OrdealFails;

		public int Precepts = 1;

		public List<string> FinshedOrdealLines = new List<string>();

		public int GrubConvo = -1;

		public List<string> FinishedConvos = new List<string>();

		public Dictionary<string, int> PersistentVoiceActors = new Dictionary<string, int>();
	}
	public static class FSMEditUtils
	{
		private static Coroutine UITextRoutine;

		public static void CreateDreamDialogue(string convName, string sheetName)
		{
			PlayMakerFSM obj = FSMUtility.LocateMyFSM(FsmVariables.GlobalVariables.GetFsmGameObject("Enemy Dream Msg").Value, "Display");
			obj.Fsm.GetFsmString("Convo Title").Value = convName;
			obj.Fsm.GetFsmString("Sheet").Value = sheetName;
			obj.SendEvent("DISPLAY DREAM MSG");
		}

		public static void PlayUIText(this PlayMakerFSM fsm, string audiokey, UIAudioType audioType)
		{
			AudioPlayer.StopPlaying();
			if (UITextRoutine != null)
			{
				((MonoBehaviour)HKVocals.CoroutineHolder).StopCoroutine(UITextRoutine);
			}
			UITextRoutine = ((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(PlayAudioAfterDelay());
			IEnumerator PlayAudioAfterDelay()
			{
				if (UITextAudio.OpenShopMenu)
				{
					UITextAudio.AudioQueued = true;
					yield return (object)new WaitForSeconds(1f);
					UITextAudio.AudioQueued = false;
					UITextAudio.OpenShopMenu = false;
				}
				else if (UITextAudio.OpenInvMenu)
				{
					UITextAudio.AudioQueued = true;
					yield return (object)new WaitForSeconds(1f);
					UITextAudio.AudioQueued = false;
					UITextAudio.OpenInvMenu = false;
				}
				else
				{
					UITextAudio.AudioQueued = true;
					yield return (object)new WaitForSeconds(0.3f);
					UITextAudio.AudioQueued = false;
				}
				if (!UITextAudio.ShopMenuClosed)
				{
					if (audioType == UIAudioType.Shop)
					{
						MixerLoader.SetSnapshot(MiscUtils.GetCurrentSceneName());
					}
					else
					{
						MixerLoader.SetSnapshot(Snapshots.Room);
					}
				}
				if (audioType == UIAudioType.Shop && HKVocals._globalSettings.shopAudio)
				{
					AudioPlayer.TryPlayAudioFor(fsm.FsmVariables.GetFsmString(audiokey).Value + "_0");
				}
				else if (audioType == UIAudioType.Other && HKVocals._globalSettings.invAudio)
				{
					AudioPlayer.TryPlayAudioFor(fsm.FsmVariables.GetFsmString(audiokey).Value + "_0");
				}
			}
		}

		public static FsmState CreateEmptyState(this PlayMakerFSM fsm, string name)
		{
			FsmState obj = FsmUtil.CopyFsmState(fsm, fsm.FsmStates[0].Name, name);
			obj.Actions = Array.Empty<FsmStateAction>();
			obj.Transitions = Array.Empty<FsmTransition>();
			return obj;
		}

		public static void DisableFsmAction(this FsmState state, int index)
		{
			state.Actions[index].Enabled = false;
		}

		public static void DisableFsmActionInRange(this FsmState state, params int[] indexes)
		{
			for (int i = 0; i < state.Actions.Length; i++)
			{
				if (indexes.Contains(i))
				{
					state.Actions[i].Enabled = false;
				}
			}
		}

		public static void DisableFsmActionsThatAre(this FsmState state, Func<FsmStateAction, bool> predicate)
		{
			if (!state.Actions.Any(predicate))
			{
				return;
			}
			foreach (FsmStateAction item in state.Actions.Where(predicate))
			{
				item.Enabled = false;
			}
		}

		public static void EnableActionsThatAre(this FsmState state, Func<FsmStateAction, bool> predicate)
		{
			if (!state.Actions.Any(predicate))
			{
				return;
			}
			foreach (FsmStateAction item in state.Actions.Where(predicate))
			{
				item.Enabled = true;
			}
		}

		public static bool IsAudioAction(this FsmStateAction action)
		{
			if (action is AudioPlayerOneShot || action is AudioPlayerOneShotSingle || action is AudioPlaySimple)
			{
				return true;
			}
			return false;
		}

		public static string[] GetClipNames(this FsmStateAction action)
		{
			List<string> ret = new List<string>();
			AudioPlayerOneShot val = (AudioPlayerOneShot)(object)((action is AudioPlayerOneShot) ? action : null);
			if (val != null)
			{
				AudioClip[] audioClips = val.audioClips;
				if (audioClips != null && audioClips.Length > 0)
				{
					val.audioClips.ForEach(delegate(AudioClip c)
					{
						if ((Object)(object)c != (Object)null)
						{
							ret.Add(((Object)c).name);
						}
					});
				}
			}
			else
			{
				AudioPlayerOneShotSingle val2 = (AudioPlayerOneShotSingle)(object)((action is AudioPlayerOneShotSingle) ? action : null);
				if (val2 != null)
				{
					Object value = val2.audioClip.Value;
					if (((value is AudioClip) ? value : null) != (Object)null)
					{
						ret.Add(val2.audioClip.Value.name);
					}
				}
				else
				{
					AudioPlaySimple val3 = (AudioPlaySimple)(object)((action is AudioPlaySimple) ? action : null);
					if (val3 != null)
					{
						GameObject ownerDefaultTarget = ((FsmStateAction)val3).Fsm.GetOwnerDefaultTarget(val3.gameObject);
						if ((Object)(object)ownerDefaultTarget != (Object)null)
						{
							AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
							if ((Object)(object)component != (Object)null)
							{
								Object value2 = val3.oneShotClip.Value;
								if (((value2 is AudioClip) ? value2 : null) != (Object)null)
								{
									ret.Add(val3.oneShotClip.Value.name);
								}
								else
								{
									ret.Add(((Object)component.clip).name);
								}
							}
						}
					}
				}
			}
			return ret.ToArray();
		}
	}
	public static class MiscUtils
	{
		public static int[] GetRange(int start, int end)
		{
			int[] array = new int[end - start + 1];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = start + i;
			}
			return array;
		}

		public static int GetIndexOf<T>(this T[] arr, Func<T, bool> predicate)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				if (predicate(arr[i]))
				{
					return i;
				}
			}
			throw new ArgumentOutOfRangeException();
		}

		public static string GetCurrentSceneName()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			Scene activeScene = SceneManager.GetActiveScene();
			return ((Scene)(ref activeScene)).name;
		}

		public static void ForEach<T>(this IEnumerable<T> ienumarable, Action<T> action)
		{
			T[] array = ienumarable.ToArray();
			foreach (T obj in array)
			{
				action(obj);
			}
		}

		public static void RemoveValues<T, V>(this Dictionary<T, V> dict, Func<V, bool> condition)
		{
			List<T> list = new List<T>();
			T val = default(T);
			V val2 = default(V);
			foreach (KeyValuePair<T, V> item2 in dict)
			{
				DeconstructUtil.Deconstruct<T, V>(item2, ref val, ref val2);
				T item = val;
				V arg = val2;
				if (condition(arg))
				{
					list.Add(item);
				}
			}
			foreach (T item3 in list)
			{
				dict.Remove(item3);
			}
		}

		public static void RemoveNullValues<T>(this List<T> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] == null)
				{
					list.RemoveAt(i);
					i--;
				}
			}
		}

		public static void TryInvokeActions(this Action<PlayMakerFSM> action, PlayMakerFSM fsm)
		{
			if (action == null)
			{
				return;
			}
			Delegate[] invocationList = action.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				try
				{
					(invocationList[i] as Action<PlayMakerFSM>)(fsm);
				}
				catch (Exception ex)
				{
					((Loggable)HKVocals.instance).LogError((object)ex);
				}
			}
		}

		public static GameObject Find(this Behaviour obj, string name)
		{
			return ((Component)((Component)obj).transform.Find(name)).gameObject;
		}

		public static float GetDecibelVolume(float value)
		{
			if (!(value <= 9f))
			{
				return 0f;
			}
			return Extensions.Reflect(RefVanillaMenu.MusicVolumeSlider.GetComponent<MenuAudioSlider>()).GetVolumeLevel(value);
		}

		public static string GetFileHash(string file)
		{
			SHA1 sHA = SHA1.Create();
			FileStream fileStream = File.OpenRead(file);
			string text = BitConverter.ToString(sHA.ComputeHash(fileStream)).Replace("-", "").ToLowerInvariant();
			fileStream.Close();
			sHA.Clear();
			return text.Substring(0, 6) ?? "";
		}
	}
	public static class RefVanillaMenu
	{
		private static MenuScreen _audioMenuScreen;

		private static GameObject _audioMenuScreenContent;

		private static GameObject _musicVolumeSliderHolder;

		private static GameObject _musicVolumeSlider;

		private static AudioMixer _masterMixer;

		private static GameObject _defaultAudioSettingsButton;

		private static MenuScreen _extrasMenuScreen;

		private static GameObject _extrasMenuScreenContent;

		private static GameObject _creditButtonHolder;

		private static List<GameObject> _packDetailButtons;

		public static MenuScreen AudioMenuScreen
		{
			get
			{
				if ((Object)(object)_audioMenuScreen == (Object)null)
				{
					_audioMenuScreen = ((Behaviour)(object)UIManager.instance.UICanvas).Find("AudioMenuScreen").GetComponent<MenuScreen>();
				}
				return _audioMenuScreen;
			}
		}

		public static GameObject AudioMenuScreenContent
		{
			get
			{
				if ((Object)(object)_audioMenuScreenContent == (Object)null)
				{
					_audioMenuScreenContent = ((Behaviour)(object)AudioMenuScreen).Find("Content");
				}
				return _audioMenuScreenContent;
			}
		}

		public static GameObject MusicVolumeSliderHolder
		{
			get
			{
				if ((Object)(object)_musicVolumeSliderHolder == (Object)null)
				{
					_musicVolumeSliderHolder = GameObjectUtils.Find(AudioMenuScreenContent, "MusicVolume");
				}
				return _musicVolumeSliderHolder;
			}
		}

		public static GameObject MusicVolumeSlider
		{
			get
			{
				if ((Object)(object)_musicVolumeSlider == (Object)null)
				{
					_musicVolumeSlider = GameObjectUtils.Find(MusicVolumeSliderHolder, "MusicSlider");
				}
				return _musicVolumeSlider;
			}
		}

		public static AudioMixer MasterMixer
		{
			get
			{
				if ((Object)(object)_masterMixer == (Object)null)
				{
					_masterMixer = GameObjectUtils.Find(GameObjectUtils.Find(AudioMenuScreenContent, "MasterVolume"), "MasterSlider").GetComponent<MenuAudioSlider>().masterMixer;
				}
				return _masterMixer;
			}
		}

		public static GameObject DefaultAudioSettingsButton
		{
			get
			{
				if ((Object)(object)_defaultAudioSettingsButton == (Object)null)
				{
					_defaultAudioSettingsButton = ((Behaviour)(object)AudioMenuScreen).Find("Controls/DefaultsButton");
				}
				return _defaultAudioSettingsButton;
			}
		}

		public static MenuScreen ExtrasMenuScreen
		{
			get
			{
				if ((Object)(object)_extrasMenuScreen == (Object)null)
				{
					_extrasMenuScreen = ((Behaviour)(object)UIManager.instance.UICanvas).Find("ExtrasMenuScreen").GetComponent<MenuScreen>();
				}
				return _extrasMenuScreen;
			}
		}

		public static GameObject ExtrasMenuScreenContent
		{
			get
			{
				if ((Object)(object)_extrasMenuScreenContent == (Object)null)
				{
					_extrasMenuScreenContent = ((Behaviour)(object)ExtrasMenuScreen).Find("Content");
				}
				return _extrasMenuScreenContent;
			}
		}

		public static GameObject CreditButtonHolder
		{
			get
			{
				if ((Object)(object)_creditButtonHolder == (Object)null)
				{
					_creditButtonHolder = GameObjectUtils.Find(ExtrasMenuScreenContent, "Credits");
				}
				return _creditButtonHolder;
			}
		}

		public static List<GameObject> PackDetailButtons
		{
			get
			{
				if (_packDetailButtons == null)
				{
					_packDetailButtons = new List<GameObject>();
				}
				_packDetailButtons.RemoveNullValues();
				if (_packDetailButtons.Count < 4)
				{
					_packDetailButtons.Clear();
					_packDetailButtons = new List<GameObject>
					{
						GameObjectUtils.Find(ExtrasMenuScreenContent, "PackDetails_1"),
						GameObjectUtils.Find(ExtrasMenuScreenContent, "PackDetails_2"),
						GameObjectUtils.Find(ExtrasMenuScreenContent, "PackDetails_Lifeblood"),
						GameObjectUtils.Find(ExtrasMenuScreenContent, "PackDetails_3")
					};
				}
				return _packDetailButtons;
			}
		}
	}
}
namespace HKVocals.UI
{
	public static class AudioMenu
	{
		public static void AddAudioSliderAndSettingsButton()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Expected O, but got Unknown
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = new GameObject("HKVocals Volume");
			val.transform.SetParent(RefVanillaMenu.MusicVolumeSlider.transform.parent.parent);
			val.transform.localScale = Vector3.one;
			val.transform.localPosition = RefVanillaMenu.MusicVolumeSliderHolder.transform.localPosition + Vector3.down * 120f;
			GameObject val2 = Object.Instantiate<GameObject>(RefVanillaMenu.MusicVolumeSlider, val.transform);
			MenuAudioSlider VolumeSlider_MenuAudioSlider = val2.GetComponent<MenuAudioSlider>();
			Slider component = val2.GetComponent<Slider>();
			val2.transform.localPosition = Vector3.right * 303f;
			((Object)val2).name = "HK Vocals Slider";
			GameObject val3 = GameObject.Find("_UIManager/UICanvas/AudioMenuScreen/Content/MasterVolume/MasterSlider");
			GameObject val4 = GameObject.Find("_UIManager/UICanvas/AudioMenuScreen/Content/SoundVolume/SoundSlider");
			GameObject val5 = GameObject.Find("_UIManager/UICanvas/AudioMenuScreen/Content/MusicVolume/MusicSlider");
			int[] volumes = new int[3]
			{
				int.Parse(GameObjectUtils.Find(val3, "MasterVolValue").GetComponent<Text>().text),
				int.Parse(GameObjectUtils.Find(val4, "SoundValue").GetComponent<Text>().text),
				int.Parse(GameObjectUtils.Find(val5, "MusicValue").GetComponent<Text>().text)
			};
			if (volumes.Any((int v) => v > HKVocals._globalSettings.volume) || volumes.Any((int v) => v < 9))
			{
				HKVocals._globalSettings.dampenLocked = true;
				HKVocals._globalSettings.dampenAudio = false;
			}
			else
			{
				HKVocals._globalSettings.dampenLocked = false;
			}
			Action<float> action = delegate(float f)
			{
				VolumeSlider_MenuAudioSlider.UpdateTextValue(f);
				HKVocals._globalSettings.volume = (int)f;
				MixerLoader.SetMixerVolume();
				if (volumes.Any((int v) => v > (int)f))
				{
					HKVocals._globalSettings.dampenLocked = true;
					HKVocals._globalSettings.dampenAudio = false;
				}
				else
				{
					HKVocals._globalSettings.dampenLocked = false;
				}
			};
			GameObject[] array = (GameObject[])(object)new GameObject[3] { val3, val4, val5 };
			for (int num = 0; num < array.Length; num++)
			{
				((UnityEvent<float>)(object)array[num].GetComponent<Slider>().onValueChanged).AddListener((UnityAction<float>)delegate(float f)
				{
					if (f < 9f)
					{
						HKVocals._globalSettings.dampenLocked = true;
						HKVocals._globalSettings.dampenAudio = false;
					}
					else
					{
						HKVocals._globalSettings.dampenLocked = false;
					}
				});
			}
			SliderEvent val6 = new SliderEvent();
			((UnityEvent<float>)(object)val6).AddListener((UnityAction<float>)action.Invoke);
			component.onValueChanged = val6;
			GameObjectUtils.RemoveComponent<AutoLocalizeTextUI>(GameObjectUtils.Find(val2, "Label"));
			GameObjectUtils.Find(val2, "Label").GetComponent<Text>().text = "圣巢之声音量： ";
			val2.SetActive(true);
			VolumeSlider_MenuAudioSlider.UpdateTextValue((float)HKVocals._globalSettings.volume);
			component.value = HKVocals._globalSettings.volume;
			GameObject obj = Object.Instantiate<GameObject>(RefVanillaMenu.DefaultAudioSettingsButton, RefVanillaMenu.DefaultAudioSettingsButton.transform.parent);
			((Object)obj).name = "HK Vocals Settings";
			obj.transform.localPosition = Vector3.up * 335f;
			GameObjectUtils.RemoveComponent<EventTrigger>(obj);
			GameObjectUtils.RemoveComponent<AutoLocalizeTextUI>(GameObjectUtils.Find(obj, "Text"));
			GameObjectUtils.Find(obj, "Text").GetComponent<Text>().text = "前往圣巢之声设置";
			MenuButton component2 = obj.GetComponent<MenuButton>();
			component2.proceed = true;
			component2.buttonType = (MenuButtonType)2;
			component2.submitAction = delegate
			{
				ModMenu.GoToModMenu();
			};
			Selectable component3 = RefVanillaMenu.MusicVolumeSlider.GetComponent<Selectable>();
			Selectable component4 = val2.GetComponent<Selectable>();
			Selectable component5 = obj.GetComponent<Selectable>();
			Selectable component6 = RefVanillaMenu.DefaultAudioSettingsButton.GetComponent<Selectable>();
			Navigation navigation = component3.navigation;
			((Navigation)(ref navigation)).selectOnDown = component4;
			component3.navigation = navigation;
			navigation = default(Navigation);
			((Navigation)(ref navigation)).selectOnUp = component3;
			((Navigation)(ref navigation)).selectOnDown = component5;
			((Navigation)(ref navigation)).mode = (Mode)4;
			component4.navigation = navigation;
			navigation = default(Navigation);
			((Navigation)(ref navigation)).selectOnUp = component4;
			((Navigation)(ref navigation)).selectOnDown = component6;
			((Navigation)(ref navigation)).mode = (Mode)4;
			component5.navigation = navigation;
			navigation = component6.navigation;
			((Navigation)(ref navigation)).selectOnUp = component5;
			component6.navigation = navigation;
		}
	}
	public static class ModMenu
	{
		public static Menu MenuRef;

		public static Menu DevMenuRef;

		private static Snapshots SelectedSnapshot = Snapshots.No_Effect;

		private static string SelectedAudio = "";

		private static bool IsZH
		{
			get
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				return ((object)Language.CurrentLanguage()/*cast due to constrained. prefix*/).ToString() == "ZH";
			}
		}

		private static string Lz(string zh, string en)
		{
			if (!IsZH)
			{
				return en;
			}
			return zh;
		}

		public static MenuScreen CreateModMenuScreen(MenuScreen modListMenu)
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Expected O, but got Unknown
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c7: Expected O, but got Unknown
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Expected O, but got Unknown
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Expected O, but got Unknown
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Expected O, but got Unknown
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Expected O, but got Unknown
			CreateDevMenuScreen(modListMenu);
			if (MenuRef == null)
			{
				MenuRef = new Menu(Lz("圣巢之声", "HK Vocals"), (Element[])(object)new Element[10]
				{
					(Element)new MenuButton(Lz("更改音量", "Audio Settings"), Lz("更改语音演员的音量", "Change voice actor volume"), (Action<MenuButton>)delegate
					{
						UIManager.instance.UILeaveDynamicMenu(RefVanillaMenu.AudioMenuScreen, (MainMenuState)6);
					}, true, "__UseName"),
					(Element)Blueprints.HorizontalBoolOption(Lz("滚动锁定", "Scroll Lock"), Lz("首次对话是否应锁定滚动直到音频播放完毕？", "Should scrolling be locked until audio finishes on first dialogue?"), (Action<bool>)delegate(bool i)
					{
						HKVocals._globalSettings.scrollLock = i;
					}, (Func<bool>)(() => HKVocals._globalSettings.scrollLock), (string)null, (string)null, "__UseName"),
					(Element)Blueprints.HorizontalBoolOption(Lz("自动滚动", "Auto Scroll"), Lz("对话是否应在音频播放完毕后自动滚动？", "Should dialogue auto-scroll after audio finishes?"), (Action<bool>)delegate(bool i)
					{
						HKVocals._globalSettings.autoScroll = i;
						((BaseElement)MenuRef.Find(Lz("自动滚动速度", "Auto Scroll Speed"))).isVisible = i;
						((BaseElement)MenuRef).Update();
					}, (Func<bool>)(() => HKVocals._globalSettings.autoScroll), (string)null, (string)null, "__UseName"),
					(Element)new HorizontalOption(Lz("自动滚动速度", "Auto Scroll Speed"), Lz("音频播放完毕后自动滚动的速度应有多快？", "How fast should dialogue auto-scroll after audio?"), Enum.GetNames(typeof(AutoScroll.ScrollSpeed)).ToArray(), (Action<int>)delegate(int i)
					{
						HKVocals._globalSettings.scrollSpeed = (AutoScroll.ScrollSpeed)i;
					}, (Func<int>)(() => (int)HKVocals._globalSettings.scrollSpeed), "自动滚动速度")
					{
						isVisible = HKVocals._globalSettings.autoScroll
					},
					(Element)Blueprints.HorizontalBoolOption(Lz("梦钉对话", "Dream Nail Dialogue"), Lz("梦钉对话是否应有语音？", "Should Dream Nail dialogue have voice?"), (Action<bool>)delegate(bool i)
					{
						HKVocals._globalSettings.dnDialogue = i;
						((BaseElement)MenuRef.Find(Lz("自动Boss呐喊", "Auto Boss Shout"))).isVisible = !i;
						((BaseElement)MenuRef).Update();
					}, (Func<bool>)(() => HKVocals._globalSettings.dnDialogue), (string)null, (string)null, "__UseName"),
					(Element)new HorizontalOption(Lz("自动Boss呐喊", "Auto Boss Shout"), Lz("某些Boss是否应自动呐喊？", "Should certain bosses shout automatically?"), new string[2]
					{
						Lz("开", "On"),
						Lz("关", "Off")
					}, (Action<int>)delegate(int i)
					{
						HKVocals._globalSettings.automaticBossDialogue = i == 0;
					}, (Func<int>)(() => (!HKVocals._globalSettings.automaticBossDialogue) ? 1 : 0), "自动Boss对话")
					{
						isVisible = !HKVocals._globalSettings.dnDialogue
					},
					(Element)Blueprints.HorizontalBoolOption(Lz("衰减音频", "Dampen Audio"), Lz("播放音频时是否应衰减音频？", "Should audio dampen while playing?"), (Action<bool>)delegate(bool i)
					{
						if (!HKVocals._globalSettings.dampenLocked)
						{
							HKVocals._globalSettings.dampenAudio = i;
							if (!HKVocals._globalSettings.dampenAudio)
							{
								DampenAudio.ForceStopDampenAudio();
							}
						}
					}, (Func<bool>)(() => HKVocals._globalSettings.dampenAudio), (string)null, (string)null, "__UseName"),
					(Element)Blueprints.HorizontalBoolOption(Lz("商店", "Shop"), Lz("商店是否应有语音？", "Should shop audio have voice?"), (Action<bool>)delegate(bool i)
					{
						HKVocals._globalSettings.shopAudio = i;
					}, (Func<bool>)(() => HKVocals._globalSettings.shopAudio), (string)null, (string)null, "__UseName"),
					(Element)Blueprints.HorizontalBoolOption(Lz("物品栏", "Inventory"), Lz("物品栏是否应有语音？", "Should inventory audio have voice?"), (Action<bool>)delegate(bool i)
					{
						HKVocals._globalSettings.invAudio = i;
					}, (Func<bool>)(() => HKVocals._globalSettings.invAudio), (string)null, (string)null, "__UseName"),
					(Element)new MenuButton(Lz("加入圣巢之声 Discord 服务器！", "Join the HK Vocals Discord server!"), "", (Action<MenuButton>)delegate
					{
						Application.OpenURL("https://discord.gg/p72F2St3RF");
					}, false, "__UseName")
				});
			}
			if (HKVocals._globalSettings.DevMode)
			{
				MenuRef.AddElement((Element)new MenuButton(Lz("开发菜单", "Dev Menu"), "", (Action<MenuButton>)delegate
				{
					UIManager.instance.UIGoToDynamicMenu(DevMenuRef.menuScreen);
				}, false, "__UseName"));
			}
			return MenuRef.GetMenuScreen(modListMenu);
		}

		public static MenuScreen CreateDevMenuScreen(MenuScreen modListMenu)
		{
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Expected O, but got Unknown
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Expected O, but got Unknown
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Expected O, but got Unknown
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Expected O, but got Unknown
			if (DevMenuRef == null)
			{
				DevMenuRef = new Menu(Lz("圣巢之声开发菜单", "HNV Dev Menu"), (Element[])(object)new Element[3]
				{
					(Element)new HorizontalOption(Lz("RB-SFX（手柄音效）", "RB-SFX"), "", new string[9]
					{
						Lz("无效果", "No Effect"),
						Lz("梦境", "Dream"),
						Lz("洞穴", "Cave"),
						Lz("温泉", "Spa"),
						Lz("悬崖", "Cliffs"),
						Lz("房间", "Room"),
						Lz("竞技场", "Arena"),
						Lz("下水道", "Sewerpipe"),
						Lz("雾之峡谷", "Fog Canyon")
					}, (Action<int>)delegate(int i)
					{
						SelectedSnapshot = i switch
						{
							0 => Snapshots.No_Effect, 
							1 => Snapshots.Dream, 
							2 => Snapshots.Cave, 
							3 => Snapshots.Spa, 
							4 => Snapshots.Cliffs, 
							5 => Snapshots.Room, 
							6 => Snapshots.Arena, 
							7 => Snapshots.Sewerpipe, 
							8 => Snapshots.Fog_Canyon, 
							_ => Snapshots.No_Effect, 
						};
					}, (Func<int>)(() => 0), "RB-SFX"),
					(Element)new InputField(Lz("音频", "Audio"), (Action<string>)delegate(string i)
					{
						SelectedAudio = i;
					}, (Func<string>)(() => SelectedAudio), "", 50, (InputFieldConfig?)InputFieldConfig.DefaultText, "AudioName"),
					(Element)new MenuButton(Lz("播放音频", "Play Audio"), "", (Action<MenuButton>)delegate
					{
						MixerLoader.SetSnapshot(SelectedSnapshot);
						AudioPlayer.TryPlayAudioFor(SelectedAudio);
					}, false, "__UseName")
				});
			}
			return DevMenuRef.GetMenuScreen(modListMenu);
		}

		public static void GoToModMenu()
		{
			UIManager.instance.UIGoToDynamicMenu(((Object)(object)MenuRef.menuScreen != (Object)null) ? MenuRef.menuScreen : MenuRef.GetMenuScreen(((Behaviour)(object)UIManager.instance.UICanvas).Find("ModListMenu").GetComponent<MenuScreen>()));
		}
	}
	public static class SettingsPrompt
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static ShowMenu_BeforeArgs <0>__RemoveButton;
		}

		[CanBeNull]
		private static GameObject PromptButton;

		public static void CreatePrompt()
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			if (!HKVocals._globalSettings.settingsOpened)
			{
				GameObject val = ((Behaviour)(object)UIManager.instance.UICanvas).Find("SaveProfileScreen");
				PromptButton = Object.Instantiate<GameObject>(GameObjectUtils.Find(GameObjectUtils.Find(val, "Controls"), "BackButton"), val.transform);
				((Object)PromptButton).name = "HKV Settings Prompt";
				PromptButton.transform.localPosition = Vector3.down * 700f;
				GameObjectUtils.RemoveComponent<EventTrigger>(PromptButton);
				GameObjectUtils.RemoveComponent<AutoLocalizeTextUI>(GameObjectUtils.Find(PromptButton, "Text"));
				GameObjectUtils.Find(PromptButton, "Text").GetComponent<Text>().text = "建议游玩圣巢之声前先检查您的音频设置";
				MenuButton component = PromptButton.GetComponent<MenuButton>();
				component.proceed = true;
				component.buttonType = (MenuButtonType)2;
				component.submitAction = delegate
				{
					((MonoBehaviour)UIManager.instance).StartCoroutine(GoToModMenu());
				};
			}
		}

		private static IEnumerator GoToModMenu()
		{
			yield return UIManager.instance.HideSaveProfileMenu();
			ModMenu.GoToModMenu();
		}

		public static void HookRemoveButton()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			if (!HKVocals._globalSettings.settingsOpened)
			{
				object obj = <>O.<0>__RemoveButton;
				if (obj == null)
				{
					ShowMenu_BeforeArgs val = RemoveButton;
					<>O.<0>__RemoveButton = val;
					obj = (object)val;
				}
				BeforeOrig.ShowMenu += (ShowMenu_BeforeArgs)obj;
			}
		}

		private static void RemoveButton(Params_ShowMenu args)
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Expected O, but got Unknown
			if ((Object)(object)ModMenu.MenuRef.menuScreen != (Object)null && (Object)(object)ModMenu.MenuRef.menuScreen == (Object)(object)args.menu)
			{
				HKVocals._globalSettings.settingsOpened = true;
				if ((Object)(object)PromptButton != (Object)null)
				{
					Object.Destroy((Object)(object)PromptButton);
				}
				object obj = <>O.<0>__RemoveButton;
				if (obj == null)
				{
					ShowMenu_BeforeArgs val = RemoveButton;
					<>O.<0>__RemoveButton = val;
					obj = (object)val;
				}
				BeforeOrig.ShowMenu -= (ShowMenu_BeforeArgs)obj;
			}
		}
	}
}
namespace HKVocals.MajorFeatures
{
	public static class Achievements
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static LanguageGetProxy <0>__AchLang;

			public static RecieveDreamImpact_BeforeArgs <1>__Aluba;
		}

		public static void Hook()
		{
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Expected O, but got Unknown
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Expected O, but got Unknown
			AchievementHelper.AddAchievement("ImpatientLemm", AssemblyUtils.GetSpriteFromResources("HKVocals.Resources.Impateint.png", 66f), "Impatient", "Leave a relic in Lemm’s deposit box.", true);
			AchievementHelper.AddAchievement("DisdainGrub", AssemblyUtils.GetSpriteFromResources("HKVocals.Resources.Distain.png", 66f), "Disdain", "Read the dreams of a particularly ungrateful Grub.", true);
			AchievementHelper.AddAchievement("KindnessPaleLurker", AssemblyUtils.GetSpriteFromResources("HKVocals.Resources.Kindness.png", 66f), "Kindness", "Show the Pale Lurker a new perspective on life.", true);
			AchievementHelper.AddAchievement("LastLaughOrdeal", AssemblyUtils.GetSpriteFromResources("HKVocals.Resources.Last_Laugh.png", 66f), "Last Laugh", "Hit the lever below The Eternal Ordeal’s Zote statue.", true);
			AchievementHelper.AddAchievement("AlubafarDreamnail", AssemblyUtils.GetSpriteFromResources("HKVocals.Resources.Alubafar.png", 66f), "Alubafar", "Listen to what an Aluba has to say", true);
			AchievementHelper.AddAchievement("CompendiumVocalization", AssemblyUtils.GetSpriteFromResources("HKVocals.Resources.Full_compendium.png", 66f), "Compendium Vocalization", "Listen to every line of vocalized dialogue in Hallownest.", false);
			AchievementHelper.AddAchievement("Consideration", AssemblyUtils.GetSpriteFromResources("HKVocals.Resources.All_Dialogue.png", 66f), "Consideration", "Listen to every word of Hallownest’s living inhabitants.", false);
			AchievementHelper.AddAchievement("Ambition", AssemblyUtils.GetSpriteFromResources("HKVocals.Resources.All_Dreams.png", 66f), "Ambition", "Uncover every dream, of bug and spirit alike.", false);
			AchievementHelper.AddAchievement("Chronology", AssemblyUtils.GetSpriteFromResources("HKVocals.Resources.All_Lore_Tablets.png", 66f), "Chronology", "Find every lore tablet buried under this dead Kingdom.", false);
			AchievementHelper.AddAchievement("Acquisition", AssemblyUtils.GetSpriteFromResources("HKVocals.Resources.All_Items_and_Journal.png", 66f), "Acquisition", "Review every item and journal entry there is to acquire.", false);
			object obj = <>O.<0>__AchLang;
			if (obj == null)
			{
				LanguageGetProxy val = AchLang;
				<>O.<0>__AchLang = val;
				obj = (object)val;
			}
			ModHooks.LanguageGetHook += (LanguageGetProxy)obj;
			object obj2 = <>O.<1>__Aluba;
			if (obj2 == null)
			{
				RecieveDreamImpact_BeforeArgs val2 = Aluba;
				<>O.<1>__Aluba = val2;
				obj2 = (object)val2;
			}
			BeforeOrig.RecieveDreamImpact += (RecieveDreamImpact_BeforeArgs)obj2;
		}

		private static string AchLang(string key, string sheettitle, string orig)
		{
			return key switch
			{
				"Impatient" => "Impatient", 
				"Leave a relic in Lemm’s deposit box." => "Leave a relic in Lemm’s deposit box.", 
				"Disdain" => "Disdain", 
				"Read the dreams of a particularly ungrateful Grub." => "Read the dreams of a particularly ungrateful Grub.", 
				"Kindness" => "Kindness", 
				"Show the Pale Lurker a new perspective on life." => "Show the Pale Lurker a new perspective on life.", 
				"Last Laugh" => "Last Laugh", 
				"Hit the lever below The Eternal Ordeal’s Zote statue." => "Hit the lever below The Eternal Ordeal’s Zote statue.", 
				"Alubafar" => "Alubafar", 
				"Listen to what an Aluba has to say" => "Listen to what an Aluba has to say", 
				"Compendium Vocalization" => "Compendium Vocalization", 
				"Listen to every line of vocalized dialogue in Hallownest." => "Listen to every line of vocalized dialogue in Hallownest.", 
				"Consideration" => "Consideration", 
				"Listen to every word of Hallownest’s living inhabitants." => "Listen to every word of Hallownest’s living inhabitants.", 
				"Ambition" => "Ambition", 
				"Uncover every dream, of bug and spirit alike." => "Uncover every dream, of bug and spirit alike.", 
				"Chronology" => "Chronology", 
				"Find every lore tablet buried under this dead Kingdom." => "Find every lore tablet buried under this dead Kingdom.", 
				"Acquisition" => "Acquisition", 
				"Review every item and journal entry there is to acquire." => "Review every item and journal entry there is to acquire.", 
				_ => orig, 
			};
		}

		private static void Aluba(Params_RecieveDreamImpact args)
		{
			if (((Object)((Component)args.self).gameObject).name == "Lazy Flyer Enemy")
			{
				GameManager.instance.AwardAchievement("AlubafarDreamnail");
			}
		}
	}
	public static class AutomaticBossDialogue
	{
		private struct FsmLocation(string go, string fsm, string scene = null)
		{
			public string scene = scene;

			public string go = go;

			public string fsm = fsm;
		}

		private struct GenericBoss(string go, string key, bool requiresDreamNail = false)
		{
			public string go = go;

			public string key = key;

			public bool requiresDreamNail = requiresDreamNail;
		}

		private struct ABDLine(string[] lines, float chance = 1f, float wait = 0f)
		{
			public string[] lines = lines;

			public float chance = chance;

			public float wait = wait;
		}

		private struct ABDStates(Dictionary<string, ABDLine> dialogueStates, Dictionary<string, Func<GameObject, IEnumerator>> coroutineStates = null, Action<GameObject> init = null)
		{
			public Dictionary<string, ABDLine> dialogueStates = dialogueStates;

			public Dictionary<string, Func<GameObject, IEnumerator>> coroutineStates = coroutineStates;

			public Action<GameObject> init = init;
		}

		public enum BossClassification
		{
			Normal,
			RequiresDreamNail
		}

		[CompilerGenerated]
		private static class <>O
		{
			public static Start_AfterArgs <0>__InitHpListeners;

			public static TakeDamage_AfterArgs <1>__CheckHpListeners;
		}

		public const string ABDKeyPrefix = "HKVocals_ABD_";

		private const string ANY_GO = "*";

		private static readonly Dictionary<FsmLocation, ABDStates> BossDialogueGoFsm = new Dictionary<FsmLocation, ABDStates>
		{
			{
				new FsmLocation("False Knight New", "FalseyControl"),
				new ABDStates(new Dictionary<string, ABDLine>
				{
					{
						"Start Fall",
						new ABDLine(new string[1] { "FALSE_KNIGHT_1" }, 1f, 5f)
					},
					{
						"Recover",
						new ABDLine(new string[2] { "FALSE_KNIGHT_2", "FALSE_KNIGHT_3" }, 1f, 1f)
					}
				})
			},
			{
				new FsmLocation("False Knight Dream", "FalseyControl"),
				new ABDStates(new Dictionary<string, ABDLine>
				{
					{
						"Start Fall",
						new ABDLine(new string[1] { "FALSE_KNIGHT_D_1" }, 1f, 5f)
					},
					{
						"Recover",
						new ABDLine(new string[2] { "FALSE_KNIGHT_D_2", "FALSE_KNIGHT_D_3" }, 1f, 1f)
					}
				})
			},
			{
				new FsmLocation("Oro", "nailmaster"),
				new ABDStates(new Dictionary<string, ABDLine>(), new Dictionary<string, Func<GameObject, IEnumerator>> { { "Death Start", OroDialogue } })
			},
			{
				new FsmLocation("Jar Collector", "Control"),
				new ABDStates(new Dictionary<string, ABDLine> { 
				{
					"Slam",
					new ABDLine(new string[3] { "JAR_COLLECTOR_1", "JAR_COLLECTOR_2", "JAR_COLLECTOR_3" }, 0.4f)
				} })
			},
			{
				new FsmLocation("Dream Mage Lord Phase2", "Mage Lord 2"),
				new ABDStates(new Dictionary<string, ABDLine> { 
				{
					"Music",
					new ABDLine(new string[1] { "MAGELORD_D_1" })
				} })
			},
			{
				new FsmLocation("Grey Prince", "Control"),
				new ABDStates(new Dictionary<string, ABDLine>
				{
					{
						"Jump",
						new ABDLine(new string[5] { "GREY_PRINCE_1", "GREY_PRINCE_2", "GREY_PRINCE_3", "GREY_PRINCE_4", "GREY_PRINCE_5" }, 0.2f)
					},
					{
						"Spit Dir",
						new ABDLine(new string[5] { "GREY_PRINCE_1", "GREY_PRINCE_2", "GREY_PRINCE_3", "GREY_PRINCE_4", "GREY_PRINCE_5" }, 0.2f)
					}
				})
			},
			{
				new FsmLocation("Dung Defender", "Dung Defender"),
				new ABDStates(new Dictionary<string, ABDLine>(), new Dictionary<string, Func<GameObject, IEnumerator>> { { "Wake", DungDefenderDialogue } })
			},
			{
				new FsmLocation("Absolute Radiance", "Control"),
				new ABDStates(new Dictionary<string, ABDLine>
				{
					{
						"Set Arena 1",
						new ABDLine(new string[1] { "RADIANCE_1" }, 1f, 5f)
					},
					{
						"Rage1 Tele",
						new ABDLine(new string[1] { "RADIANCE_3" })
					},
					{
						"Tendrils1",
						new ABDLine(new string[1] { "RADIANCE_4" })
					},
					{
						"Arena 2 Start",
						new ABDLine(new string[1] { "RADIANCE_5" }, 1f, 2f)
					},
					{
						"Scream",
						new ABDLine(new string[1] { "RADIANCE_6" }, 1f, 5f)
					}
				})
			},
			{
				new FsmLocation("Absolute Radiance", "Phase Control"),
				new ABDStates(new Dictionary<string, ABDLine> { 
				{
					"Set Phase 2",
					new ABDLine(new string[1] { "RADIANCE_2" })
				} })
			},
			{
				new FsmLocation("Radiance", "Control"),
				new ABDStates(new Dictionary<string, ABDLine>
				{
					{
						"Set Arena 1",
						new ABDLine(new string[1] { "RADIANCE_1" }, 1f, 5f)
					},
					{
						"Rage1 Tele",
						new ABDLine(new string[1] { "RADIANCE_3" })
					},
					{
						"Tendrils1",
						new ABDLine(new string[1] { "RADIANCE_4" })
					},
					{
						"Arena 2 Start",
						new ABDLine(new string[1] { "RADIANCE_5" }, 1f, 2f)
					},
					{
						"Ascend Tele",
						new ABDLine(new string[1] { "RADIANCE_6" }, 1f, 5f)
					}
				})
			},
			{
				new FsmLocation("Radiance", "Phase Control"),
				new ABDStates(new Dictionary<string, ABDLine> { 
				{
					"Set Phase 2",
					new ABDLine(new string[1] { "RADIANCE_2" })
				} })
			}
		};

		private static readonly Dictionary<FsmLocation, Dictionary<float, ABDLine>> HealthTriggers = new Dictionary<FsmLocation, Dictionary<float, ABDLine>>
		{
			{
				new FsmLocation("Dream Mage Lord", "Dream Mage Lord"),
				new Dictionary<float, ABDLine>
				{
					{
						2f / 3f,
						new ABDLine(new string[1] { "MAGELORD_D_2" })
					},
					{
						1f / 3f,
						new ABDLine(new string[1] { "MAGELORD_D_3" })
					}
				}
			},
			{
				new FsmLocation("Hornet Boss 1", "Hornet Boss 1", "Fungus1_04_boss"),
				new Dictionary<float, ABDLine>
				{
					{
						0.75f,
						new ABDLine(new string[1] { "HORNET_GREENPATH_1" })
					},
					{
						0.5f,
						new ABDLine(new string[1] { "HORNET_GREENPATH_2" })
					},
					{
						0.25f,
						new ABDLine(new string[1] { "HORNET_GREENPATH_3" })
					}
				}
			},
			{
				new FsmLocation("Hornet Boss 2", "Hornet Boss 2", "GG_Hornet_2"),
				new Dictionary<float, ABDLine>
				{
					{
						0.75f,
						new ABDLine(new string[1] { "HORNET_GG_1" })
					},
					{
						0.5f,
						new ABDLine(new string[1] { "HORNET_GG_2" })
					},
					{
						0.25f,
						new ABDLine(new string[1] { "HORNET_GG_3" })
					}
				}
			},
			{
				new FsmLocation("Oro", "Oro"),
				new Dictionary<float, ABDLine> { 
				{
					0.3f,
					new ABDLine(new string[1] { "ORO_1" })
				} }
			}
		};

		private static readonly List<GenericBoss> GenericBosses = new List<GenericBoss>(16)
		{
			new GenericBoss("Hive Knight", "HIVE_KNIGHT", requiresDreamNail: true),
			new GenericBoss("Mawlek Body", "MAWLEK", requiresDreamNail: true),
			new GenericBoss("Fluke Mother", "FLUKEMOTHER", requiresDreamNail: true),
			new GenericBoss("Lobster", "GENERIC", requiresDreamNail: true),
			new GenericBoss("Giant Fly", "GRUZMOTHER", requiresDreamNail: true),
			new GenericBoss("Mega Moss Charger", "GENERIC", requiresDreamNail: true),
			new GenericBoss("Mage Knight", "MAGE_KNIGHT", requiresDreamNail: true),
			new GenericBoss("Mega Jellyfish", "MEGAJELLYFISH", requiresDreamNail: true),
			new GenericBoss("Mega Zombie Beam Miner", "ZOMBIE_MEGA_MINER"),
			new GenericBoss("Lancer", "ZOMBIE"),
			new GenericBoss("Sly Boss", "SLY"),
			new GenericBoss("Grimm Boss", "GRIMM"),
			new GenericBoss("Nightmare Grimm Boss", "NIGHTMARE_GRIMM"),
			new GenericBoss("Sheo Boss", "SHEO"),
			new GenericBoss("Mantis Traitor Lord", "MANTIS_TRAITOR"),
			new GenericBoss("Zote Boss", "ZOTE")
		};

		private static readonly Dictionary<FsmLocation, float> LastHealthValues = new Dictionary<FsmLocation, float>();

		private static readonly Dictionary<FsmLocation, float> MaxHealthValues = new Dictionary<FsmLocation, float>();

		public static void Hook()
		{
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Expected O, but got Unknown
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Expected O, but got Unknown
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Expected O, but got Unknown
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Expected O, but got Unknown
			foreach (GenericBoss boss in GenericBosses)
			{
				List<string> list = AudioAPI.AudioNames.FindAll((string s) => s.StartsWith(boss.key.ToUpper()));
				int num = Math.Min(list.Count, 3);
				Dictionary<float, ABDLine> dictionary = new Dictionary<float, ABDLine>();
				for (int num2 = 0; num2 < num; num2++)
				{
					int index = Random.Range(0, list.Count);
					float key = ((float)num2 + 1f) / ((float)num + 1f);
					dictionary[key] = new ABDLine(new string[1] { list[index] });
					list.RemoveAt(index);
				}
				HealthTriggers.Add(new FsmLocation(boss.go, ""), dictionary);
			}
			object obj = <>O.<0>__InitHpListeners;
			if (obj == null)
			{
				Start_AfterArgs val = InitHpListeners;
				<>O.<0>__InitHpListeners = val;
				obj = (object)val;
			}
			AfterOrig.Start += (Start_AfterArgs)obj;
			object obj2 = <>O.<1>__CheckHpListeners;
			if (obj2 == null)
			{
				TakeDamage_AfterArgs val2 = CheckHpListeners;
				<>O.<1>__CheckHpListeners = val2;
				obj2 = (object)val2;
			}
			AfterOrig.TakeDamage += (TakeDamage_AfterArgs)obj2;
			foreach (KeyValuePair<FsmLocation, ABDStates> item in BossDialogueGoFsm)
			{
				if (item.Value.coroutineStates != null)
				{
					foreach (KeyValuePair<string, Func<GameObject, IEnumerator>> coroutine in item.Value.coroutineStates)
					{
						Hooks.HookStateEnteredFromTransition(new FSMData(item.Key.go, item.Key.fsm, coroutine.Key), (Action<PlayMakerFSM, string>)delegate(PlayMakerFSM FSM, string type)
						{
							((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(coroutine.Value(((Component)FSM).gameObject));
						});
					}
				}
				foreach (KeyValuePair<string, ABDLine> dialogue in item.Value.dialogueStates)
				{
					Hooks.HookStateEnteredFromTransition((item.Key.go == "*") ? new FSMData(item.Key.fsm, dialogue.Key) : new FSMData(item.Key.go, item.Key.fsm, dialogue.Key), (Action<PlayMakerFSM, string>)delegate(PlayMakerFSM fsm, string type)
					{
						PlayABDLine(dialogue.Value, ((Component)fsm).gameObject);
					});
				}
				item.Value.init?.Invoke(GameObject.Find(item.Key.go));
			}
		}

		private static void PlayABDLine(ABDLine line, GameObject go)
		{
			string key = line.lines[Random.Range(0, line.lines.Length)];
			if (Random.value <= line.chance)
			{
				((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(PlayLineAfter(go, key, line.wait));
			}
		}

		private static IEnumerator PlayLineAfter(GameObject go, string key, float time)
		{
			yield return (object)new WaitForSeconds(time);
			DreamNailDialogue.InvokeAutomaticBossDialogue(go, key);
		}

		public static void OnFsmInit()
		{
		}

		private static FsmLocation? GetHealthTrigger(HealthManager hm)
		{
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			GameObject gameObject = ((Component)hm).gameObject;
			PlayMakerFSM component = gameObject.GetComponent<PlayMakerFSM>();
			if ((Object)(object)component == (Object)null)
			{
				return null;
			}
			Scene scene2;
			foreach (KeyValuePair<FsmLocation, Dictionary<float, ABDLine>> healthTrigger in HealthTriggers)
			{
				if (healthTrigger.Key.fsm == "" && ((Object)gameObject).name == healthTrigger.Key.go)
				{
					return healthTrigger.Key;
				}
				if (!(healthTrigger.Key.fsm == ((Object)component).name) || !(healthTrigger.Key.go == ((Object)gameObject).name))
				{
					continue;
				}
				if (healthTrigger.Key.scene != null)
				{
					string scene = healthTrigger.Key.scene;
					scene2 = gameObject.scene;
					if (!(scene == ((Scene)(ref scene2)).name))
					{
						continue;
					}
				}
				return healthTrigger.Key;
			}
			HKVocals instance = HKVocals.instance;
			string[] obj = new string[6]
			{
				"Found an HM/FSM with no matching trigger. GameObject: ",
				((Object)gameObject).name,
				" FSM: ",
				((Object)component).name,
				" Scene: $",
				null
			};
			scene2 = gameObject.scene;
			obj[5] = ((Scene)(ref scene2)).name;
			((Loggable)instance).LogDebug(string.Concat(obj));
			return null;
		}

		private static void InitHpListeners(Params_Start args)
		{
			FsmLocation? healthTrigger = GetHealthTrigger(args.self);
			if (healthTrigger.HasValue)
			{
				FsmLocation value = healthTrigger.Value;
				LastHealthValues[value] = args.self.hp;
				MaxHealthValues[value] = args.self.hp;
			}
		}

		private static void CheckHpListeners(Params_TakeDamage args)
		{
			FsmLocation? healthTrigger = GetHealthTrigger(args.self);
			if (!healthTrigger.HasValue)
			{
				return;
			}
			FsmLocation trigger = healthTrigger.Value;
			float num = LastHealthValues[trigger] / MaxHealthValues[trigger];
			float num2 = (float)args.self.hp / MaxHealthValues[trigger];
			foreach (KeyValuePair<float, ABDLine> item in HealthTriggers[trigger])
			{
				if (!(num > item.Key) || !(num2 <= item.Key))
				{
					continue;
				}
				IEnumerable<GenericBoss> source = GenericBosses.Where((GenericBoss boss) => boss.go == trigger.go);
				if (source.Count() == 0)
				{
					PlayABDLine(item.Value, ((Component)args.self).gameObject);
					continue;
				}
				GenericBoss genericBoss = source.First();
				if ((genericBoss.requiresDreamNail && PlayerData.instance.hasDreamNail) || !genericBoss.requiresDreamNail)
				{
					PlayABDLine(item.Value, ((Component)args.self).gameObject);
				}
			}
			LastHealthValues[trigger] = args.self.hp;
		}

		private static IEnumerator OroDialogue(GameObject oro)
		{
			GameObject mato = GameObject.Find("Mato");
			yield return (object)new WaitForSeconds(10f);
			DreamNailDialogue.InvokeAutomaticBossDialogue(mato, "MATO_1");
			yield return (object)new WaitForSeconds(AudioPlayer.GetAudioFor("$Mato$_MATO_1_0_1").length + 0.5f);
			DreamNailDialogue.InvokeAutomaticBossDialogue(oro, "ORO_ALT_2");
			yield return (object)new WaitForSeconds(AudioPlayer.GetAudioFor("$Oro$_ORO_ALT_2_0_1").length + 0.5f);
			DreamNailDialogue.InvokeAutomaticBossDialogue(mato, "MATO_2");
		}

		private static IEnumerator DungDefenderDialogue(GameObject boss)
		{
			yield return (object)new WaitForSeconds(5f);
			DreamNailDialogue.InvokeAutomaticBossDialogue(boss, "DUNG_DEF_1");
			yield return (object)new WaitForSeconds(AudioPlayer.GetAudioFor("$Dung Defender$_DUNG_DEF_1_0_1").length + 4f);
			DreamNailDialogue.InvokeAutomaticBossDialogue(boss, "DUNG_DEF_2");
			yield return (object)new WaitForSeconds(AudioPlayer.GetAudioFor("$Dung Defender$_DUNG_DEF_2_0_1").length + 4f);
			DreamNailDialogue.InvokeAutomaticBossDialogue(boss, "DUNG_DEF_3");
		}
	}
	public static class AutoScroll
	{
		public enum ScrollSpeed
		{
			Instant,
			Fast,
			Normal,
			Slow
		}

		public static Dictionary<ScrollSpeed, float> ScrollSpeeds = new Dictionary<ScrollSpeed, float>
		{
			{
				ScrollSpeed.Instant,
				0f
			},
			{
				ScrollSpeed.Fast,
				0.25f
			},
			{
				ScrollSpeed.Normal,
				0.6f
			},
			{
				ScrollSpeed.Slow,
				1f
			}
		};

		public const string CustomNextEvent = "NEXT_NOAUDIO";

		public static void Hook()
		{
			FSMEditData.AddGameObjectFsmEdit("Text", "Dialogue Page Control", ImplementAutoScroll);
		}

		public static void ImplementAutoScroll(PlayMakerFSM fsm)
		{
			FsmBool val = FsmUtil.AddFsmBoolVariable(fsm, "Is Convo Ending");
			val.Value = false;
			ImplementAutoScroll_OnPageEnd(fsm, val);
			ImplementAutoScroll_OnHalfConvoEnd(fsm, val);
		}

		private static void ImplementAutoScroll_OnPageEnd(PlayMakerFSM fsm, FsmBool isConvoEnding)
		{
			FsmUtil.AddFsmAction(FsmUtil.GetFsmState(fsm, "Page End"), (FsmStateAction)(object)new AutoScrollOnFinishPlaying(isConvoEnding));
			RemoveNextPagePrompt(fsm, "Page End", 1);
			RemoveAudioOnAutoScroll(fsm, "Show Next Page", "Page End");
		}

		private static void ImplementAutoScroll_OnHalfConvoEnd(PlayMakerFSM fsm, FsmBool isConvoEnding)
		{
			FsmUtil.AddFsmMethod(fsm, "Arrow", (Action)delegate
			{
				isConvoEnding.Value = false;
			});
			FsmUtil.AddFsmMethod(fsm, "Stop", (Action)delegate
			{
				isConvoEnding.Value = true;
			});
			FsmUtil.AddFsmAction(fsm, "Conversation End", (FsmStateAction)(object)new AutoScrollOnFinishPlaying(isConvoEnding, shouldConsiderConvoEnding: true));
			RemoveNextPagePrompt(fsm, "Arrow", 2);
			RemoveAudioOnAutoScroll(fsm, "SFX", "Conversation End");
		}

		private static void RemoveAudioOnAutoScroll(PlayMakerFSM fsm, string audioState, string previousState)
		{
			FsmState fsmState = FsmUtil.GetFsmState(fsm, audioState);
			FsmState val = FsmUtil.CopyFsmState(fsm, fsmState.Name, audioState + " No Audio");
			FsmUtil.ReplaceAllActions(val, fsmState.Actions);
			FsmUtil.RemoveFsmAction(val, val.Actions.GetIndexOf((FsmStateAction a) => a is AudioPlayerOneShotSingle));
			FsmUtil.AddFsmTransition(fsm, previousState, "NEXT_NOAUDIO", val.Name);
		}

		private static void RemoveNextPagePrompt(PlayMakerFSM fsm, string state, int arrowUpActionIndex)
		{
			FsmUtil.GetFsmState(fsm, state).DisableFsmAction(arrowUpActionIndex);
			FsmUtil.InsertFsmMethod(FsmUtil.GetFsmState(fsm, state), (Action)delegate
			{
				if (!NPCDialogue.DidPlayAudioOnDialogueBox || !HKVocals._globalSettings.autoScroll || !HKVocals._globalSettings.scrollLock)
				{
					FSMUtility.LocateMyFSM(((Component)fsm.Fsm.GameObject.gameObject.transform.parent.Find("Arrow")).gameObject, "Arrow Anim").SendEvent("UP");
				}
			}, arrowUpActionIndex);
		}
	}
	public class AutoScrollOnFinishPlaying : FsmStateAction
	{
		private FsmBool IsConvoEnding;

		private bool ShouldConsiderConvoEnding;

		private float timer;

		private float WaitTime;

		public AutoScrollOnFinishPlaying(FsmBool isConvoEnding, bool shouldConsiderConvoEnding = false)
		{
			IsConvoEnding = isConvoEnding;
			ShouldConsiderConvoEnding = shouldConsiderConvoEnding;
		}

		public override void OnEnter()
		{
			timer = 0f;
			WaitTime = AutoScroll.ScrollSpeeds[HKVocals._globalSettings.scrollSpeed];
			CheckForFinish();
		}

		public override void Reset()
		{
			timer = 0f;
		}

		public override void OnUpdate()
		{
			CheckForFinish();
		}

		public void CheckForFinish()
		{
			if (!AudioPlayer.IsPlaying() && NPCDialogue.DidPlayAudioOnDialogueBox && HKVocals._globalSettings.autoScroll && (!ShouldConsiderConvoEnding || (ShouldConsiderConvoEnding && !IsConvoEnding.Value)))
			{
				timer += Time.deltaTime;
				if (timer >= WaitTime)
				{
					timer = 0f;
					((FsmStateAction)this).Fsm.Event("NEXT_NOAUDIO");
				}
			}
		}
	}
	public static class DampenAudio
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class <>c
		{
			public static readonly <>c <>9 = new <>c();

			public static HideText_BeforeArgs <>9__9_0;

			public static Action<PlayMakerFSM> <>9__9_1;

			public static Action<PlayMakerFSM> <>9__9_2;

			public static Action<PlayMakerFSM> <>9__9_3;

			public static Action<PlayMakerFSM> <>9__9_4;

			internal void <Hook>b__9_0(Params_HideText _)
			{
				StopDampenAudio();
			}

			internal void <Hook>b__9_1(PlayMakerFSM _)
			{
				StopDampenAudio();
			}

			internal void <Hook>b__9_2(PlayMakerFSM _)
			{
				StopDampenAudio();
			}

			internal void <Hook>b__9_3(PlayMakerFSM _)
			{
				StopDampenAudio();
			}

			internal void <Hook>b__9_4(PlayMakerFSM _)
			{
				StopDampenAudio();
			}
		}

		private static float DampenValue;

		private static bool AudioDampened;

		private const float DampenValueNormal = 45f;

		private const float DampenValueDream = 45f;

		private const float DampenTime = 1f;

		private static AudioSource _dreamDialogueMusic;

		private static AudioSource _dreamNailSlash;

		private static AudioMixerGroup DreamDialogueMusicGroup;

		private static AudioMixerGroup DreamNailSlashGroup;

		private static AudioSource DreamDialogueMusic
		{
			get
			{
				if ((Object)(object)_dreamDialogueMusic == (Object)null && (Object)(object)GameCameras.instance != (Object)null)
				{
					_dreamDialogueMusic = ((Component)GameCameras.instance.cameraParent.Find("tk2dCamera").Find("Audio").Find("Dream Dialogue")).GetComponent<AudioSource>();
				}
				return _dreamDialogueMusic;
			}
		}

		private static AudioSource DreamNailSlash
		{
			get
			{
				if ((Object)(object)_dreamNailSlash == (Object)null && (Object)(object)HeroController.instance != (Object)null)
				{
					_dreamNailSlash = ((Component)((Component)HeroController.instance).transform.Find("Sounds").Find("Dream Nail")).GetComponent<AudioSource>();
				}
				return _dreamNailSlash;
			}
		}

		public static void Hook()
		{
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Expected O, but got Unknown
			//IL_009c: Expected O, but got Unknown
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Expected O, but got Unknown
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Expected O, but got Unknown
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Expected O, but got Unknown
			NPCDialogue.OnPlayNPCDialogue += StartDampingAudioNormal;
			DreamNailDialogue.OnPlayDreamDialogue += StartDampingAudioDream;
			object obj = <>c.<>9__9_0;
			if (obj == null)
			{
				HideText_BeforeArgs val = delegate
				{
					StopDampenAudio();
				};
				<>c.<>9__9_0 = val;
				obj = (object)val;
			}
			BeforeOrig.HideText += (HideText_BeforeArgs)obj;
			Hooks.HookStateEntered(new FSMData("DialogueManager", "Box Open Dream", "Stop Audio"), (Action<PlayMakerFSM>)delegate
			{
				StopDampenAudio();
			});
			Hooks.HookStateEntered(new FSMData("Dream Msg", "Display", "Full Cancel"), (Action<PlayMakerFSM>)delegate
			{
				StopDampenAudio();
			});
			Hooks.HookStateEntered(new FSMData("Dream Msg", "Display", "Text Down"), (Action<PlayMakerFSM>)delegate
			{
				StopDampenAudio();
			});
			Hooks.HookStateEntered(new FSMData("DialogueManager", "Box Open Dream", "Stop Audio"), (Action<PlayMakerFSM>)delegate
			{
				StopDampenAudio();
			});
			FSMEditData.AddGameObjectFsmEdit("Text", "Dialogue Page Control", DontCallHideTextOnHalfConvoFinish);
		}

		private static void DontCallHideTextOnHalfConvoFinish(PlayMakerFSM fsm)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			FsmState fsmState = FsmUtil.GetFsmState(fsm, "End Conversation");
			FsmUtil.ReplaceFsmAction(fsmState, (FsmStateAction)new MethodAction
			{
				Method = delegate
				{
					FsmBool fsmBoolVariable = FsmUtil.GetFsmBoolVariable(fsm, "Use Stop");
					DialogueBoxR val = Extensions.Reflect(fsm.Fsm.GameObject.gameObject.GetComponent<DialogueBox>());
					if (!fsmBoolVariable.Value)
					{
						orig_DialogueBox_HideText(val);
					}
					else
					{
						val.HideText();
					}
					fsmBoolVariable.Value = true;
				}
			}, 3);
			FsmUtil.RemoveFsmAction(fsmState, 0);
		}

		private static void orig_DialogueBox_HideText(DialogueBoxR box)
		{
			if (box.typing)
			{
				box.StopTypewriter();
			}
			((TMP_Text)box.textMesh).maxVisibleCharacters = 0;
			box.hidden = true;
		}

		public static void StartDampingAudioNormal()
		{
			DampenValue = 45f;
			if (HKVocals._globalSettings.dampenAudio)
			{
				((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(DoDampenAudio(dampen: true));
			}
		}

		private static void StartDampingAudioDream()
		{
			DampenValue = 45f;
			if (HKVocals._globalSettings.dampenAudio)
			{
				SetDampenExceptions();
				((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(DoDampenAudio(dampen: true));
			}
		}

		private static void StopDampenAudio()
		{
			if (HKVocals._globalSettings.dampenAudio)
			{
				ForceStopDampenAudio();
			}
		}

		public static void ForceStopDampenAudio()
		{
			UnsetDampenExceptions();
			((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(DoDampenAudio(dampen: false));
		}

		private static IEnumerator DoDampenAudio(bool dampen)
		{
			if ((!dampen || !AudioDampened) && (dampen || AudioDampened))
			{
				HKVocals.DoLogDebug((dampen ? "Dampening" : "UnDampening") + " audio");
				AudioDampened = dampen;
				float currentTime = 0f;
				float multiplier = (100f - DampenValue) / 100f;
				float masterVolume = GameManager.instance.gameSettings.masterVolume;
				while (currentTime <= 1f)
				{
					currentTime += Time.deltaTime;
					RefVanillaMenu.MasterMixer.SetFloat("MasterVolume", Mathf.Lerp(MiscUtils.GetDecibelVolume(masterVolume * (dampen ? 1f : multiplier)), MiscUtils.GetDecibelVolume(masterVolume * (dampen ? multiplier : 1f)), currentTime / 1f));
					yield return null;
				}
			}
		}

		private static void SetDampenExceptions()
		{
			if ((Object)(object)DreamDialogueMusic.outputAudioMixerGroup != (Object)null && (Object)(object)DreamDialogueMusic.outputAudioMixerGroup != (Object)(object)MixerLoader.DampenExcludeMusic)
			{
				if ((Object)(object)DreamDialogueMusic.outputAudioMixerGroup != (Object)null)
				{
					DreamDialogueMusicGroup = DreamDialogueMusic.outputAudioMixerGroup;
				}
				DreamDialogueMusic.outputAudioMixerGroup = MixerLoader.DampenExcludeMusic;
			}
			if ((Object)(object)DreamNailSlash.outputAudioMixerGroup != (Object)null && (Object)(object)DreamNailSlash.outputAudioMixerGroup != (Object)(object)MixerLoader.DampenExcludeSFX)
			{
				if ((Object)(object)DreamNailSlash.outputAudioMixerGroup != (Object)null)
				{
					DreamNailSlashGroup = DreamNailSlash.outputAudioMixerGroup;
				}
				DreamNailSlash.outputAudioMixerGroup = MixerLoader.DampenExcludeSFX;
			}
		}

		private static void UnsetDampenExceptions()
		{
			if ((Object)(object)DreamDialogueMusic != (Object)null && (Object)(object)DreamDialogueMusic.outputAudioMixerGroup != (Object)null && (Object)(object)DreamDialogueMusic.outputAudioMixerGroup == (Object)(object)MixerLoader.DampenExcludeMusic && (Object)(object)DreamDialogueMusicGroup != (Object)null)
			{
				DreamDialogueMusic.outputAudioMixerGroup = DreamDialogueMusicGroup;
			}
			if ((Object)(object)DreamNailSlash != (Object)null && (Object)(object)DreamNailSlash.outputAudioMixerGroup != (Object)null && (Object)(object)DreamNailSlash.outputAudioMixerGroup == (Object)(object)MixerLoader.DampenExcludeSFX && (Object)(object)DreamNailSlashGroup != (Object)null)
			{
				DreamNailSlash.outputAudioMixerGroup = DreamNailSlashGroup;
			}
		}
	}
	public static class DreamNailDialogue
	{
		public delegate void OnPlayDreamDialogueHandler();

		[CompilerGenerated]
		private static class <>O
		{
			public static LanguageGetProxy <0>__PlayDreamNailDialogue;

			public static Start_AfterArgs <1>__AddCancelDreamDialogueOnDeath;

			public static ShowConvo_BeforeArgs <2>__SetLastDreamNailedEnemy;
		}

		private static GameObject lastDreamnailedEnemy;

		private static Regex enemyTrimRegex = new Regex("([^0-9\\(\\)]+)", RegexOptions.Compiled);

		private static string[] AutomaticKeys = new string[2] { "KING_ABYSS_", "GRIMM_REMINDER" };

		public static Dictionary<string, string> DNGroups = new Dictionary<string, string>
		{
			{ "Crawler", "GB1" },
			{ "Climber", "GB1" },
			{ "Buzzer", "GB1" },
			{ "Spitter", "GB1" },
			{ "Hatcher", "GB1" },
			{ "Roller", "GB1" },
			{ "Mosquito", "GB1" },
			{ "Fat Fly", "GB1" },
			{ "Acid Walker", "GB1" },
			{ "Acid Flyer", "GB1" },
			{ "Ceiling Dropper", "GB1" },
			{ "Flip Hopper", "GB1" },
			{ "Inflater", "GB1" },
			{ "Mines Crawler", "GB1" },
			{ "Crystal Flyer", "GB1" },
			{ "Baby Centipede", "GB1" },
			{ "Centipede Hatcher", "GB1" },
			{ "Tiny Spider", "GB1" },
			{ "Spider Mini", "GB1" },
			{ "Super Spitter", "GB1" },
			{ "Hopper", "GB1" },
			{ "Grass Hopper", "GB1" },
			{ "Colosseum_Armoured_Roller", "GB1" },
			{ "Blobble", "GB1" },
			{ "Colosseum Grass Hopper", "GB1" },
			{ "Colosseum_Armoured_Mosquito", "GB1" },
			{ "Spawn Roller v", "GB1" },
			{ "Spitter R", "GB1" },
			{ "Buzzer R", "GB1" },
			{ "Roller R", "GB1" },
			{ "Super Spitter Col", "GB1" },
			{ "Buzzer Col", "GB1" },
			{ "Ceiling Dropper Col", "GB1" },
			{ "Baby Centipede Spawner", "GB1" },
			{ "Super Spitter R", "GB1" },
			{ "Colosseum_Armoured_Roller R", "GB1" },
			{ "Colosseum_Armoured_Mosquito R", "GB1" },
			{ "Worm", "GB2" },
			{ "Big Centipede", "GB2" },
			{ "Crystal Crawler", "GB2" },
			{ "Blow Fly", "GB2" },
			{ "Giant Hopper", "GB2" },
			{ "Big Centipede Col", "GB2" },
			{ "Zombie Runner", "GH" },
			{ "Zombie Hornhead", "GH" },
			{ "Zombie Leaper", "GH" },
			{ "Zombie Barger", "GH" },
			{ "Zombie Shield", "GH" },
			{ "Zombie Guard", "GH" },
			{ "Garden Zombie", "GH" },
			{ "Zombie Fungus A", "GH" },
			{ "Zombie Fungus B", "GH" },
			{ "Zombie Runner Sp", "GH" },
			{ "Zombie Hornhead Sp", "GH" }
		};

		public static event OnPlayDreamDialogueHandler OnPlayDreamDialogue;

		public static void Hook()
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
			object obj = <>O.<0>__PlayDreamNailDialogue;
			if (obj == null)
			{
				LanguageGetProxy val = PlayDreamNailDialogue;
				<>O.<0>__PlayDreamNailDialogue = val;
				obj = (object)val;
			}
			ModHooks.LanguageGetHook += (LanguageGetProxy)obj;
			object obj2 = <>O.<1>__AddCancelDreamDialogueOnDeath;
			if (obj2 == null)
			{
				Start_AfterArgs val2 = AddCancelDreamDialogueOnDeath;
				<>O.<1>__AddCancelDreamDialogueOnDeath = val2;
				obj2 = (object)val2;
			}
			AfterOrig.Start += (Start_AfterArgs)obj2;
			object obj3 = <>O.<2>__SetLastDreamNailedEnemy;
			if (obj3 == null)
			{
				ShowConvo_BeforeArgs val3 = SetLastDreamNailedEnemy;
				<>O.<2>__SetLastDreamNailedEnemy = val3;
				obj3 = (object)val3;
			}
			BeforeOrig.ShowConvo += (ShowConvo_BeforeArgs)obj3;
		}

		public static string GetUniqueId(Transform transform, string path = "")
		{
			if ((Object)(object)transform.parent == (Object)null)
			{
				return MiscUtils.GetCurrentSceneName() + ":" + path + ((Object)transform).name;
			}
			return GetUniqueId(transform.parent, path + ((Object)transform).name + "/");
		}

		public static void InvokeAutomaticBossDialogue(GameObject boss, string key)
		{
			lastDreamnailedEnemy = boss;
			if (HKVocals._globalSettings.automaticBossDialogue || HKVocals._globalSettings.dnDialogue)
			{
				FSMEditUtils.CreateDreamDialogue("HKVocals_ABD_" + key, "Enemy Dreams");
			}
		}

		private static string PlayDreamNailDialogue(string key, string sheetTitle, string orig)
		{
			string[] automaticKeys = AutomaticKeys;
			foreach (string value in automaticKeys)
			{
				if (key.StartsWith(value))
				{
					MixerLoader.SetSnapshot((!key.Contains("ABYSS")) ? Snapshots.Dream : Snapshots.Cave);
					AudioPlayer.TryPlayAudioFor(key + "_0");
					return orig;
				}
			}
			bool flag = false;
			((Loggable)HKVocals.instance).Log("Language get attempted with key " + key);
			if (key.StartsWith("HKVocals_ABD_"))
			{
				key = key.Remove(0, "HKVocals_ABD_".Length);
				orig = Language.GetInternal(key, sheetTitle);
				flag = true;
			}
			if (key == "FK_MACE_1")
			{
				AudioPlayer.TryPlayAudioFor($"$Mace Head Bug$_FK_MACE_{Random.Range(1, 5)}_0_1".ToUpper());
			}
			if ((Object)(object)lastDreamnailedEnemy == (Object)null && !flag)
			{
				return orig;
			}
			if (!HKVocals._globalSettings.dnDialogue && !(HKVocals._globalSettings.automaticBossDialogue & flag))
			{
				return orig;
			}
			string uniqueId = GetUniqueId(lastDreamnailedEnemy.transform);
			string name = enemyTrimRegex.Match(((Object)lastDreamnailedEnemy).name).Value.Trim();
			lastDreamnailedEnemy = null;
			if (DNGroups.ContainsKey(name))
			{
				name = DNGroups[name];
			}
			AudioAPI.UpdateAudioNames();
			List<string> list = AudioAPI.CachedAudioNames.Where((string s) => s.Contains(("$" + name + "$_" + key).ToUpper())).ToList();
			if (list == null || list.Count == 0)
			{
				((Loggable)HKVocals.instance).LogError("No clips for $" + name + "$_" + key);
				return orig;
			}
			int num;
			if (HKVocals._saveSettings.PersistentVoiceActors.ContainsKey(uniqueId))
			{
				num = HKVocals._saveSettings.PersistentVoiceActors[uniqueId];
			}
			else
			{
				num = Random.Range(1, list.Count + 1);
				HKVocals._saveSettings.PersistentVoiceActors[uniqueId] = num;
			}
			MixerLoader.SetSnapshot(Snapshots.Dream);
			if (AudioPlayer.TryPlayAudioFor($"${name}$_{key}_0_{num}".ToUpper()))
			{
				OnPlayDreamDialogue?.Invoke();
			}
			if (key == "ORO_ALT_2")
			{
				return Language.Get("ORO_2", "Enemy Dreams");
			}
			return orig;
		}

		private static void SetLastDreamNailedEnemy(Params_ShowConvo args)
		{
			lastDreamnailedEnemy = ((Component)args.self).gameObject;
		}

		private static void AddCancelDreamDialogueOnDeath(Params_Start args)
		{
			((Component)args.self).gameObject.AddComponent<CancelDreamDialogueOnDeath>();
		}
	}
	public static class MuteOriginalAudio
	{
		private class MuteAudioSourceData
		{
			public AudioSource originalAudioSource;

			public bool muted;

			public MuteAudioSourceData(AudioSource _originalAudioSource)
			{
				originalAudioSource = _originalAudioSource;
				muted = false;
			}

			public void Mute()
			{
				((Loggable)HKVocals.instance).LogError("Muting " + ((Object)originalAudioSource.clip).name);
				originalAudioSource.mute = true;
				muted = true;
			}

			public void UnMute()
			{
				((Loggable)HKVocals.instance).LogError("UnMuting " + ((Object)originalAudioSource.clip).name);
				originalAudioSource.mute = false;
				muted = false;
			}

			public bool IsNull()
			{
				if (!((Object)(object)originalAudioSource == (Object)null))
				{
					return (Object)(object)originalAudioSource.clip == (Object)null;
				}
				return true;
			}

			public void ReplayAudio()
			{
				if (!IsNull() && !originalAudioSource.isPlaying && originalAudioSource.loop)
				{
					((Loggable)HKVocals.instance).LogDebug($"Re-playing {originalAudioSource.clip}");
					originalAudioSource.Play();
				}
			}
		}

		[CompilerGenerated]
		private static class <>O
		{
			public static Action<PlayMakerFSM> <0>__RemoveOriginalNPCSounds;

			public static NPCDialogue.OnPlayNPCDialogueHandler <1>__MuteCollectedAudios;

			public static HideText_BeforeArgs <2>__UnMuteCollectedAudios;

			public static Action<PlayMakerFSM> <3>__DontSetMetIseldaPDBool;

			public static Action<PlayMakerFSM> <4>__ChangeIseldaShopAudio;

			public static Action<PlayMakerFSM> <5>__RemoveMaskMakerAudioOnSpeak;

			public static Action<PlayMakerFSM> <6>__MakeSureMaskMakerAudioIsReplayed;

			public static Action<PlayMakerFSM> <7>__ReplayMaskMakerAudioOnFinish;

			public static Action<PlayMakerFSM> <8>__RemoveLemmAndSlyShopAudio;

			public static Action<PlayMakerFSM> <9>__RemoveRelicDiscussionsAudio;

			public static Action<PlayMakerFSM> <10>__RemoveFlukeHermitAudio;

			public static Action<Action<AudioSource>, AudioSource> <11>__ExcludeSomeAudios_Play;

			public static Action<Action<AudioSource, ulong>, AudioSource, ulong> <12>__ExcludeSomeAudios_Play_delay;

			public static Action<Action<AudioSource, float>, AudioSource, float> <13>__ExcludeSomeAudios_PlayDelayed;

			public static Action<Action<AudioSource, double>, AudioSource, double> <14>__ExcludeSomeAudios_PlayScheduled;

			public static Action<Action<AudioSource, AudioClip, float>, AudioSource, AudioClip, float> <15>__ExcludeSomeAudios_PlayOneShot;
		}

		private static readonly Dictionary<string, MuteAudioSourceData> ToMuteAudioSources = new Dictionary<string, MuteAudioSourceData>();

		private static readonly List<string> ClipsToInclude = new List<string>
		{
			"switch_gate_gate", "dream_enter", "dream_ghost_appear", "Grey_Mourner_wake", "stalactite_break", "dream_ghost_appear", "dream_plant_hit", "dream_orb_pickup", "dream_enter_pt_2", "shaman_charging_spell",
			"shaman_spell", "shiny_item_pickup", "misc_rumble_loop", "misc_rumble_impact", "mage_lord_summon_projectiles", "dream_plant_hit", "mawlek_spit", "HK_Cloth_Battle_Cry", "Nailmaster_roar_01", "Nailmaster_roar_02",
			"Nailmaster_Bama_01", "Nailmaster_Bama_02", "new_heartpiece_puzzle_bit", "manhole_key_turn", "coccoon_burst_open", "manhole_key_turn", "misc_rumble_loop", "misc_rumble_impact", "coccoon_god_body_fall_out", "godseeker_groan_01",
			"godseeker_ground_hit", "bathhouse_door_open", "manhole_key_turn", "jiji_door_open_pt_1", "jiji_door_open_pt_2", "focus_health_charging", "GS_engine_hum_loop", "Sly_dazed_loop", "Bretta_scared_loop", "HK_Cloth_Burst01",
			"hornet_wall_land", "hornet_small_jump", "hornet_jump", "Mr_Mush_Talk_Loop", "Nailmaster_Bama_01", "Nailmaster_Bama_02", "Jiji_Summon", "dark_spell_absorb", "dark_spell_get", "Jiji_retreat_01",
			"hornet_ground_land", "boss_explode_clean", "ghost_dialogue_death_initial_white", "ghost_dialogue_death_explode", "ghost_absorb", "ghost_absorb_final_impact", "hollow_knight_rumble_pre_chain_break", "final_boss_atrium_white_4", "S83-14 Accordion", "shiny_item_loop",
			"manhole_open", "grimm_teleport_in", "accordion_sting_03", "Grimm_click", "nightmare_lantern_powering_up", "nightmare_lantern_flame_burst", "grimm_teleport_out", "Brumm_flame_give_short", "flamebearer_fire_spiral_prepare", "flamebearer_fire_spiral_cast",
			"Brumm_grunt_01", "Brumm_grunt_double", "nightmare_bg_eye_break_1", "nightmare_bg_eye_break_2", "spell_information_screen", "mawlek_scream", "ruin_sentry_sword_1", "Divine_eat_01", "hatcher_give_birth", "Divine_post_eat_sigh",
			"Divine_poop_long", "spider_zombie_shake", "Divine_poop_end", "Leg_Eater_angry_01", "GS_engine_room_02", "Iselda_Shop_Open"
		};

		public static void Hook()
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Expected O, but got Unknown
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Expected O, but got Unknown
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Expected O, but got Unknown
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Expected O, but got Unknown
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Expected O, but got Unknown
			FSMEditData.AddAnyFsmEdit("Conversation Control", RemoveOriginalNPCSounds);
			HookAudioSourcePlays();
			NPCDialogue.OnPlayNPCDialogue += MuteCollectedAudios;
			object obj = <>O.<2>__UnMuteCollectedAudios;
			if (obj == null)
			{
				HideText_BeforeArgs val = UnMuteCollectedAudios;
				<>O.<2>__UnMuteCollectedAudios = val;
				obj = (object)val;
			}
			BeforeOrig.HideText += (HideText_BeforeArgs)obj;
			FSMEditData.AddGameObjectFsmEdit("Iselda", "Conversation Control", DontSetMetIseldaPDBool);
			Hooks.HookStateEntered(new FSMData("Iselda", "Shop Anim", "Shop Start"), (Action<PlayMakerFSM>)ChangeIseldaShopAudio);
			Hooks.HookStateEntered(new FSMData("Maskmaker NPC", "Conversation Control", "Mask Choice"), (Action<PlayMakerFSM>)RemoveMaskMakerAudioOnSpeak);
			Hooks.HookStateEntered(new FSMData("Maskmaker NPC", "Conversation Control", "Play Audio"), (Action<PlayMakerFSM>)MakeSureMaskMakerAudioIsReplayed);
			FSMEditData.AddGameObjectFsmEdit("Maskmaker NPC", "Conversation Control", ReplayMaskMakerAudioOnFinish);
			FSMEditData.AddGameObjectFsmEdit("Shop Region", "Shop Region", RemoveLemmAndSlyShopAudio);
			FSMEditData.AddGameObjectFsmEdit("Relic Dealer", "Relic Discussions", RemoveRelicDiscussionsAudio);
			Hooks.HookStateEntered(new FSMData("Fluke Hermit", "Conversation Control", "Box Up"), (Action<PlayMakerFSM>)RemoveFlukeHermitAudio);
		}

		private static void RemoveOriginalNPCSounds(PlayMakerFSM fsm)
		{
			FsmState[] fsmStates = fsm.FsmStates;
			foreach (FsmState obj in fsmStates)
			{
				obj.DisableFsmActionsThatAre((FsmStateAction action2) => action2.IsAudioAction() && !action2.GetClipNames().Any((string clip) => ClipsToInclude.Contains(clip)));
				FsmStateAction[] actions = obj.Actions;
				foreach (FsmStateAction action in actions)
				{
					if (action.IsAudioAction())
					{
						(from x in action.GetClipNames()
							where ClipsToInclude.Contains(x)
							select x).ForEach((Action<string>)((Loggable)HKVocals.instance).Log);
					}
				}
			}
		}

		private static void MuteCollectedAudios()
		{
			ToMuteAudioSources.RemoveValues((MuteAudioSourceData data) => data.IsNull());
			ToMuteAudioSources.Values.ForEach(delegate(MuteAudioSourceData data)
			{
				if (AudioAPI.CustomMuteList.Contains(((Object)data.originalAudioSource.clip).name))
				{
					data.Mute();
				}
			});
		}

		private static void UnMuteCollectedAudios(Params_HideText args)
		{
			ToMuteAudioSources.RemoveValues((MuteAudioSourceData data) => data.IsNull());
			ToMuteAudioSources.Values.ForEach(delegate(MuteAudioSourceData data)
			{
				data.UnMute();
				CoroutineHelper.WaitForFramesBeforeInvoke(20, (Action)data.ReplayAudio);
			});
		}

		private static void DontSetMetIseldaPDBool(PlayMakerFSM fsm)
		{
			FsmUtil.DisableFsmAction(fsm, "Meet", 2);
		}

		private static void ChangeIseldaShopAudio(PlayMakerFSM fsm)
		{
			AudioPlayerOneShotSingle fsmAction = FsmUtil.GetFsmAction<AudioPlayerOneShotSingle>(fsm, "Shop Start", 1);
			fsmAction.volume = FsmFloat.op_Implicit(0.4f);
			if (!PlayerDataAccess.metIselda)
			{
				((FsmStateAction)fsmAction).Enabled = false;
				CoroutineHelper.WaitForSecondsBeforeInvoke(2f, (Action)delegate
				{
					PlayerDataAccess.metIselda = true;
				});
			}
			else
			{
				((FsmStateAction)fsmAction).Enabled = true;
			}
		}

		private static void ReplayMaskMakerAudioOnFinish(PlayMakerFSM fsm)
		{
			string[] array = new string[4] { "Mask1", "Mask 2", "Mask 3", "Mask 4" };
			foreach (string text in array)
			{
				FsmUtil.ChangeFsmTransition(fsm, text, "CONVO_FINISH", "Play Audio");
			}
		}

		private static void RemoveMaskMakerAudioOnSpeak(PlayMakerFSM fsm)
		{
			GameObjectUtils.Find(((Component)fsm).gameObject, "Voice Loop").GetComponent<AudioSource>().Stop();
		}

		private static void MakeSureMaskMakerAudioIsReplayed(PlayMakerFSM fsm)
		{
			AudioSource component = GameObjectUtils.Find(((Component)fsm).gameObject, "Voice Loop").GetComponent<AudioSource>();
			component.volume = 1f;
			component.pitch = 1f;
			component.Play();
		}

		private static void RemoveLemmAndSlyShopAudio(PlayMakerFSM fsm)
		{
			FsmUtil.ChangeFsmTransition(fsm, "Voice", "LEMM", "Convo");
			FsmUtil.ChangeFsmTransition(fsm, "Voice", "SLY", "Convo");
			FsmUtil.GetFsmState(fsm, "Convo Relic").DisableFsmActionsThatAre((FsmStateAction a) => a.IsAudioAction());
		}

		private static void RemoveRelicDiscussionsAudio(PlayMakerFSM fsm)
		{
			FsmUtil.GetFsmState(fsm, "Convo").DisableFsmActionsThatAre((FsmStateAction a) => a.IsAudioAction());
		}

		private static void RemoveFlukeHermitAudio(PlayMakerFSM fsm)
		{
			StoreAudioSourceToMute(((Component)fsm).gameObject.GetComponent<AudioSource>());
		}

		private static void HookAudioSourcePlays()
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			new Hook((MethodBase)typeof(AudioSource).GetMethod("Play", Array.Empty<Type>()), (Delegate)new Action<Action<AudioSource>, AudioSource>(ExcludeSomeAudios_Play));
			new Hook((MethodBase)typeof(AudioSource).GetMethod("Play", new Type[1] { typeof(ulong) }), (Delegate)new Action<Action<AudioSource, ulong>, AudioSource, ulong>(ExcludeSomeAudios_Play_delay));
			new Hook((MethodBase)typeof(AudioSource).GetMethod("PlayDelayed"), (Delegate)new Action<Action<AudioSource, float>, AudioSource, float>(ExcludeSomeAudios_PlayDelayed));
			new Hook((MethodBase)typeof(AudioSource).GetMethod("PlayScheduled"), (Delegate)new Action<Action<AudioSource, double>, AudioSource, double>(ExcludeSomeAudios_PlayScheduled));
			new Hook((MethodBase)typeof(AudioSource).GetMethod("PlayOneShot", new Type[2]
			{
				typeof(AudioClip),
				typeof(float)
			}), (Delegate)new Action<Action<AudioSource, AudioClip, float>, AudioSource, AudioClip, float>(ExcludeSomeAudios_PlayOneShot));
		}

		private static bool StoreAudioSourceToMute(AudioSource source)
		{
			return StoreAudioSourceToMute(source, source.clip);
		}

		private static bool StoreAudioSourceToMute(AudioSource source, AudioClip clip)
		{
			if ((Object)(object)clip != (Object)null && AudioAPI.CustomMuteList.Contains(((Object)clip).name))
			{
				ToMuteAudioSources[((Object)clip).name] = new MuteAudioSourceData(source);
				((Loggable)HKVocals.instance).LogDebug("Storing audio for mute " + ((Object)clip).name);
				return true;
			}
			return false;
		}

		private static void ExcludeSomeAudios_Play(Action<AudioSource> orig, AudioSource self)
		{
			if (!StoreAudioSourceToMute(self))
			{
				orig(self);
			}
		}

		private static void ExcludeSomeAudios_Play_delay(Action<AudioSource, ulong> orig, AudioSource self, ulong delay)
		{
			if (!StoreAudioSourceToMute(self))
			{
				orig(self, delay);
			}
		}

		private static void ExcludeSomeAudios_PlayDelayed(Action<AudioSource, float> orig, AudioSource self, float delay)
		{
			if (!StoreAudioSourceToMute(self))
			{
				orig(self, delay);
			}
		}

		private static void ExcludeSomeAudios_PlayScheduled(Action<AudioSource, double> orig, AudioSource self, double time)
		{
			if (!StoreAudioSourceToMute(self))
			{
				orig(self, time);
			}
		}

		private static void ExcludeSomeAudios_PlayOneShot(Action<AudioSource, AudioClip, float> orig, AudioSource self, AudioClip clip, float volumeScale)
		{
			if (!StoreAudioSourceToMute(self, clip))
			{
				orig(self, clip, volumeScale);
			}
		}
	}
	public static class NPCDialogue
	{
		public delegate void OnPlayNPCDialogueHandler();

		[CompilerGenerated]
		private static class <>O
		{
			public static LanguageGetProxy <0>__LanguageGet;

			public static ShowPage_AfterArgs <1>__PlayAudioForNPCDialogue;
		}

		[Serializable]
		[CompilerGenerated]
		private sealed class <>c
		{
			public static readonly <>c <>9 = new <>c();

			public static HideText_BeforeArgs <>9__9_0;

			internal void <Hook>b__9_0(Params_HideText _)
			{
				AudioPlayer.StopPlaying();
			}
		}

		public static bool DidPlayAudioOnDialogueBox;

		public static List<int> CollectorVAs = new List<int> { 28, 30, 31 };

		public static string pcConvo = "";

		public static Dictionary<string, int> GrubVAs = new Dictionary<string, int>
		{
			{ "Crossroads_05", 1 },
			{ "Crossroads_48", 2 },
			{ "Crossroads_03", 3 },
			{ "Crossroads_31", 4 },
			{ "Crossroads_35", 5 },
			{ "Fungus1_06", 6 },
			{ "Fungus1_07", 7 },
			{ "Fungus1_13", 8 },
			{ "Fungus1_21", 9 },
			{ "Mines_03", 10 },
			{ "Mines_35", 11 },
			{ "Mines_19", 12 },
			{ "Mines_04", 13 },
			{ "Mines_31", 14 },
			{ "Mines_16", 15 },
			{ "Mines_24", 16 },
			{ "Fungus2_18", 17 },
			{ "Fungus2_20", 18 },
			{ "Fungus3_47", 19 },
			{ "Waterways_04", 20 },
			{ "Waterways_13", 21 },
			{ "Waterways_14", 22 },
			{ "Ruins1_05", 23 },
			{ "Ruins1_32", 24 },
			{ "Ruins2_03", 25 },
			{ "Ruins_House_01", 26 },
			{ "Ruins2_07", 27 },
			{ "Deepnest_East_11", 29 },
			{ "Deepnest_East_14", 32 },
			{ "Hive_04", 33 },
			{ "Hive_03", 34 },
			{ "Fungus3_10", 35 },
			{ "Fungus3_22", 36 },
			{ "Fungus3_48", 37 },
			{ "Deepnest_Spider_Town", 38 },
			{ "Deepnest_31", 39 },
			{ "Deepnest_03", 40 },
			{ "Deepnest_36", 41 },
			{ "Deepnest_39", 42 },
			{ "Abyss_17", 43 },
			{ "RestingGrounds_10", 44 },
			{ "Fungus1_28", 45 }
		};

		public static List<string> DWInspects = new List<string> { "ALADAR_INSPECT", "GALIEN_INSPECT", "HU_INSPECT", "MARKOTH_INSPECT", "MUMCAT_INSPECT", "NOEYES_INSPECT", "XERO_INSPECT" };

		public static event OnPlayNPCDialogueHandler OnPlayNPCDialogue;

		public static void Hook()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Expected O, but got Unknown
			object obj = <>O.<0>__LanguageGet;
			if (obj == null)
			{
				LanguageGetProxy val = LanguageGet;
				<>O.<0>__LanguageGet = val;
				obj = (object)val;
			}
			ModHooks.LanguageGetHook += (LanguageGetProxy)obj;
			object obj2 = <>O.<1>__PlayAudioForNPCDialogue;
			if (obj2 == null)
			{
				ShowPage_AfterArgs val2 = PlayAudioForNPCDialogue;
				<>O.<1>__PlayAudioForNPCDialogue = val2;
				obj2 = (object)val2;
			}
			AfterOrig.ShowPage += (ShowPage_AfterArgs)obj2;
			object obj3 = <>c.<>9__9_0;
			if (obj3 == null)
			{
				HideText_BeforeArgs val3 = delegate
				{
					AudioPlayer.StopPlaying();
				};
				<>c.<>9__9_0 = val3;
				obj3 = (object)val3;
			}
			BeforeOrig.HideText += (HideText_BeforeArgs)obj3;
		}

		private static string LanguageGet(string key, string sheetTitle, string orig)
		{
			if (key == "JINN_OFFER" && sheetTitle != "Prompts")
			{
				AudioPlayer.TryPlayAudioFor("JINN_OFFER_TALK_0", 0.6f);
			}
			if (key == "SHEO_DREAM" && sheetTitle == "Zote")
			{
				pcConvo = "SHEO_DREAM_PC_0";
			}
			return orig;
		}

		private static void PlayAudioForNPCDialogue(Params_ShowPage args)
		{
			GameObject gameObject = ((Component)((Component)args.self).gameObject.transform.parent).gameObject;
			bool enabled = ((Renderer)GameObjectUtils.Find(gameObject, "Box Dream").GetComponent<MeshRenderer>()).enabled;
			if (((Renderer)GameObjectUtils.Find(GameObjectUtils.Find(gameObject, "DialogueBox"), "backboard").GetComponent<SpriteRenderer>()).enabled)
			{
				MixerLoader.SetSnapshot(MiscUtils.GetCurrentSceneName());
			}
			else if (enabled)
			{
				if (!HKVocals._globalSettings.dnDialogue)
				{
					return;
				}
				MixerLoader.SetSnapshot(Snapshots.Dream);
			}
			bool flag;
			switch (args.self.currentConversation)
			{
			case "TUT_TAB_01":
			case "TUT_TAB_02":
			case "TUT_TAB_02_1":
			case "TUT_TAB_03":
				flag = true;
				break;
			default:
				flag = false;
				break;
			}
			if (flag)
			{
				MixerLoader.SetSnapshot(Snapshots.Cave);
			}
			string currentConversation = args.self.currentConversation;
			if ((currentConversation == "CLIFF_TAB_02" || currentConversation == "CLIFF_TAB_02_1") ? true : false)
			{
				MixerLoader.SetSnapshot(Snapshots.Room);
			}
			currentConversation = args.self.currentConversation;
			if ((currentConversation == "ABYSS_TUT_TAB_01" || currentConversation == "ABYSS_TUT_TAB_01_1") ? true : false)
			{
				MixerLoader.SetSnapshot(Snapshots.Cave);
			}
			if (args.self.currentConversation == "SENSE_TAB_01")
			{
				MixerLoader.SetSnapshot(Snapshots.Arena);
			}
			int num = args.self.currentPage - 1;
			string text = args.self.currentConversation + "_" + num;
			float removeTime = ((num == 0) ? 0.6f : 0.75f);
			if (DWInspects.Contains(args.self.currentConversation))
			{
				text = ((!PlayerData.instance.hasDreamNail) ? (args.self.currentConversation + "_ALT_0") : (args.self.currentConversation + "_0"));
			}
			if (args.self.currentConversation == "DREAM" && MiscUtils.GetCurrentSceneName() == "Room_Ouiji")
			{
				text = "DREAM_JIJI" + $"_{args.self.currentPage - 1}";
			}
			((Loggable)HKVocals.instance).Log(args.self.currentConversation);
			if (args.self.currentConversation == "WHITE_DEFENDER_OUTRO_1a" || args.self.currentConversation == "WHITE_DEFENDER_OUTRO_1b")
			{
				text = ((args.self.currentConversation == "WHITE_DEFENDER_OUTRO_1a") ? "WHITE_DEFENDER_OUTRO_1A_0" : "WHITE_DEFENDER_OUTRO_1B_0");
			}
			if (args.self.currentConversation == "JINN_OFFER")
			{
				return;
			}
			if (args.self.currentConversation == "GRUB_BOTTLE_DREAM")
			{
				foreach (KeyValuePair<string, int> grubVA in GrubVAs)
				{
					if (MiscUtils.GetCurrentSceneName() == "Ruins2_11")
					{
						int num2 = CollectorVAs[Random.Range(0, 3)];
						AudioPlayer.TryPlayAudioFor(text + $"_{num2}", removeTime);
						break;
					}
					if (grubVA.Key == MiscUtils.GetCurrentSceneName())
					{
						int num2 = grubVA.Value;
						AudioPlayer.TryPlayAudioFor(text + $"_{num2}", removeTime);
					}
				}
			}
			if (args.self.currentConversation == "SHEO_DREAM" && pcConvo == "SHEO_DREAM_PC_0" && ModHooks.GetMod("Pale Court", false, false) is Mod)
			{
				text = pcConvo;
			}
			switch (args.self.currentConversation)
			{
			case "CC_OUTRO_1a":
				text = "CC_OUTRO_1A_0";
				break;
			case "CC_OUTRO_1b":
				text = "CC_OUTRO_1B_0";
				break;
			case "RCC_OUTRO_1a":
				text = "RCC_OUTRO_1A_0";
				break;
			case "RCC_OUTRO_1b":
				text = "RCC_OUTRO_1B_0";
				break;
			}
			DidPlayAudioOnDialogueBox = AudioPlayer.TryPlayAudioFor(text, removeTime);
			if (DidPlayAudioOnDialogueBox)
			{
				OnPlayNPCDialogue?.Invoke();
			}
		}
	}
	public class Patches
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static LanguageGetProxy <0>__LanguageGetPatches;
		}

		private static bool IsChinese
		{
			get
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				return ((object)Language.CurrentLanguage()/*cast due to constrained. prefix*/).ToString() == "ZH";
			}
		}

		internal static void Hook()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			object obj = <>O.<0>__LanguageGetPatches;
			if (obj == null)
			{
				LanguageGetProxy val = LanguageGetPatches;
				<>O.<0>__LanguageGetPatches = val;
				obj = (object)val;
			}
			ModHooks.LanguageGetHook += (LanguageGetProxy)obj;
		}

		private static string LanguageGetPatches(string key, string sheetTitle, string orig)
		{
			switch (key)
			{
			case "SHOP_DESC_NOTCH_4":
				return orig.Replace("more charms", "more of your charms");
			case "HORNET_FOUNTAIN_1":
				if (!IsChinese)
				{
					return "Again we meet little ghost.<page>I'm normally quite perceptive. You I underestimated, though I've since guessed the truth.<page>You've seen beyond this kingdom's bounds. Yours is resilience born of two voids.<page>It's no surprise then you've managed to reach the heart of this world. In doing so, you shall know the sacrifice that keeps it standing.";
				}
				return "又见面了，小幽灵。<page>我向来洞察敏锐，却曾低估了你；不过如今我已猜到真相。<page>你已见识过这个王国之外的世界。你的坚韧源于两重虚空。<page>难怪你能抵达这个世界的心脏。如此一来，你也将知晓维系它所需的牺牲。";
			case "WITCH_REWARD_2A":
				if (!IsChinese)
				{
					return "Ahhh... Your Dream Nail holds over 200 Essence. You're proving your talent in its collection.<page>Have you seen that great door just outside? My tribe closed it long ago and forbade its opening.<page>Ah, but as far as I can tell I'm the only member of my tribe still breathing. That means I mustn't feel bad about breaking a taboo. As proof of my belief in you, I'll open the door.";
				}
				return "啊……你的梦之钉已积蓄超过 200 点精华，证明了你收集精华的天赋。<page>你看见外面那扇巨门了吗？我的族人很久以前封上了它，并禁止任何人开启。<page>不过据我所知，我似乎是族中唯一还活着的人。那我便不必为打破禁忌而内疚。为了证明我对你的信任，我会打开那扇门。";
			case "WATERWAYS_GREET":
				if (!IsChinese)
				{
					return "Ho ho! Aren't these waterways thrilling? A labyrinth of pipes and tunnels.<page>I couldn't have asked for a better place to employ my talents. It's all so orderly, so considered, nothing like the crude irregularity of those caverns.<page>Ahh but so sad, my trunk is telling me those Fungal Wastes are close and I sense my damp adventure may have reached its end. Guess I'll be calling this one done.";
				}
				return "呵呵！这些水道多么令人兴奋啊，简直是由管道与隧道构成的迷宫。<page>我再也找不到比这里更适合施展本领的地方了。一切都如此规整周详，与那些洞窟粗陋无序的样子完全不同。<page>啊，不过真可惜，我的鼻子告诉我真菌荒地近在咫尺；我想这趟潮湿的冒险快要结束了。看来该为这次旅程画上句号啦。";
			case "WATERWAYS_BOUGHT":
				if (!IsChinese)
				{
					return "I'd wager these pipes and chambers were once used to carry the city's waste. Would've been a horrid stench down here. Thankfully that constant rain has flushed them clean. ";
				}
				return "我敢说这些管道与房间过去是用来运送城市废物的。那时这里肯定臭气熏天。幸好连绵不断的雨水已经将它们冲洗干净。";
			default:
				return orig;
			}
		}
	}
	public static class ScrollLock
	{
		public static void Hook()
		{
			FSMEditData.AddGameObjectFsmEdit("Text", "Dialogue Page Control", ImplementLockScroll);
		}

		public static void ImplementLockScroll(PlayMakerFSM fsm)
		{
			ImplementLock(fsm, "Page End", "PAGE_END");
			ImplementLock(fsm, "Stop Pause", "CONVERSATION_END");
		}

		private static void ImplementLock(PlayMakerFSM fsm, string landingStateName, string eventName)
		{
			FsmState val = fsm.CreateEmptyState("New " + landingStateName);
			FsmUtil.AddFsmAction(val, (FsmStateAction)(object)new LockScrollUntilFinishPlaying());
			FsmUtil.AddFsmTransition(val, "FINISHED", landingStateName);
			FsmTransition obj = fsm.FsmGlobalTransitions.First((FsmTransition s) => s.EventName == eventName);
			obj.ToState = val.Name;
			obj.ToFsmState = val;
		}
	}
	public class LockScrollUntilFinishPlaying : FsmStateAction
	{
		public override void OnEnter()
		{
			DialogueBoxR val = Extensions.Reflect(((FsmStateAction)this).Fsm.GameObject.gameObject.GetComponent<DialogueBox>());
			string item = $"{val.currentConversation}_{val.currentPage - 1}";
			if (!HKVocals._saveSettings.FinishedConvos.Contains(item))
			{
				HKVocals._saveSettings.FinishedConvos.Add(item);
				if (HKVocals._globalSettings.scrollLock)
				{
					CheckForFinishState();
					return;
				}
			}
			((FsmStateAction)this).Finish();
		}

		public override void OnUpdate()
		{
			CheckForFinishState();
		}

		public void CheckForFinishState()
		{
			if (!NPCDialogue.DidPlayAudioOnDialogueBox)
			{
				((FsmStateAction)this).Finish();
			}
			else if (!AudioPlayer.IsPlaying())
			{
				((FsmStateAction)this).Finish();
			}
		}
	}
	public static class SpecialAudio
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static Begin_AfterArgs <0>__PlayMonomonIntroPoem;

			public static hook_Skip <1>__LockSkippingMonomonIntro;

			public static hook_Update <2>__WaitForAudioBeforeNextCutscene;

			public static Action<PlayMakerFSM> <3>__MakeElderbugPlaySpecialAudio;

			public static Action<PlayMakerFSM> <4>__ChangeNameOfClashingKey;

			public static Action<PlayMakerFSM> <5>__PlayDreamerPlaqueDialogue;

			public static Action<PlayMakerFSM> <6>__PlayTHKPlaqueDialogue;

			public static LanguageGetProxy <7>__AddSpecialAudioKeys;
		}

		public static void Hook()
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
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Expected O, but got Unknown
			object obj = <>O.<0>__PlayMonomonIntroPoem;
			if (obj == null)
			{
				Begin_AfterArgs val = PlayMonomonIntroPoem;
				<>O.<0>__PlayMonomonIntroPoem = val;
				obj = (object)val;
			}
			AfterOrig.Begin += (Begin_AfterArgs)obj;
			object obj2 = <>O.<1>__LockSkippingMonomonIntro;
			if (obj2 == null)
			{
				hook_Skip val2 = LockSkippingMonomonIntro;
				<>O.<1>__LockSkippingMonomonIntro = val2;
				obj2 = (object)val2;
			}
			WithOrig.Skip += (hook_Skip)obj2;
			object obj3 = <>O.<2>__WaitForAudioBeforeNextCutscene;
			if (obj3 == null)
			{
				hook_Update val3 = WaitForAudioBeforeNextCutscene;
				<>O.<2>__WaitForAudioBeforeNextCutscene = val3;
				obj3 = (object)val3;
			}
			WithOrig.Update += (hook_Update)obj3;
			FSMEditData.AddGameObjectFsmEdit("Elderbug", "Conversation Control", MakeElderbugPlaySpecialAudio);
			FSMEditData.AddGameObjectFsmEdit("Shiny Item RoyalCharm", "Shiny Control", ChangeNameOfClashingKey);
			FSMEditData.AddGameObjectFsmEdit("Dreamer Plaque Inspect", "Conversation Control", PlayDreamerPlaqueDialogue);
			FSMEditData.AddGameObjectFsmEdit("Fountain Inspect", "Conversation Control", PlayTHKPlaqueDialogue);
			object obj4 = <>O.<7>__AddSpecialAudioKeys;
			if (obj4 == null)
			{
				LanguageGetProxy val4 = AddSpecialAudioKeys;
				<>O.<7>__AddSpecialAudioKeys = val4;
				obj4 = (object)val4;
			}
			ModHooks.LanguageGetHook += (LanguageGetProxy)obj4;
		}

		private static void LockSkippingMonomonIntro(orig_Skip orig, AnimatorSequence self)
		{
			if (!HKVocals._globalSettings.scrollLock)
			{
				orig.Invoke(self);
				HKVocals.instance.audioSource.Stop();
			}
		}

		private static void WaitForAudioBeforeNextCutscene(orig_Update orig, ChainSequence self)
		{
			ChainSequenceR val = Extensions.Reflect(self);
			if (!((Object)(object)val.CurrentSequence == (Object)null) && !val.CurrentSequence.IsPlaying && !val.isSkipped && !AudioPlayer.IsPlaying())
			{
				val.Next();
			}
		}

		private static void PlayMonomonIntroPoem(Params_Begin args)
		{
			CoroutineHelper.WaitForSecondsBeforeInvoke(1.164f, (Action)delegate
			{
				MixerLoader.SetSnapshot(MiscUtils.GetCurrentSceneName());
				AudioPlayer.TryPlayAudioFor("RANDOM_POEM_STUFF_0");
			});
		}

		private static string AddSpecialAudioKeys(string key, string sheettitle, string orig)
		{
			if (key == "ELDERBUG_INTRO_MAIN_ALT" && sheettitle == "Elderbug")
			{
				orig = Language.Get("ELDERBUG_INTRO_MAIN", sheettitle);
			}
			else if (key == "KING_SOUL_PICKUP_KING_FINAL_WORDS" && sheettitle == "Minor NPC")
			{
				orig = Language.Get("KING_FINAL_WORDS", sheettitle);
			}
			return orig;
		}

		public static void MakeElderbugPlaySpecialAudio(PlayMakerFSM fsm)
		{
			FsmState introMain = FsmUtil.GetFsmState(fsm, "Intro Main");
			FsmState val = FsmUtil.CopyFsmState(fsm, introMain.Name, introMain.Name + " Alt");
			val.Actions = Array.Empty<FsmStateAction>();
			FsmUtil.AddFsmMethod(val, (Action)delegate
			{
				PlayerDataAccess.metElderbug = true;
				FsmUtil.GetFsmAction<CallMethodProper>(introMain, 1).gameObject.GameObject.Value.GetComponent<DialogueBox>().StartConversation("ELDERBUG_INTRO_MAIN_ALT", "Elderbug");
			});
			FsmUtil.ChangeFsmTransition(FsmUtil.GetFsmState(fsm, "Intro 2"), "CONVO_FINISH", val.Name);
			FsmUtil.ChangeFsmTransition(FsmUtil.GetFsmState(fsm, "Intro 3"), "CONVO_FINISH", val.Name);
		}

		public static void ChangeNameOfClashingKey(PlayMakerFSM fsm)
		{
			FsmUtil.GetFirstActionOfType<CallMethodProper>(FsmUtil.GetFsmState(fsm, "King Msg")).parameters[0].stringValue = "KING_SOUL_PICKUP_KING_FINAL_WORDS";
		}

		public static void PlayDreamerPlaqueDialogue(PlayMakerFSM fsm)
		{
			Coroutine dreamerCo = null;
			FsmUtil.InsertCustomAction(fsm, "Fade Up", (Action)delegate
			{
				dreamerCo = ((MonoBehaviour)fsm).StartCoroutine(DreamerAudio());
			}, 0);
			FsmUtil.InsertCustomAction(fsm, "Msg Down", (Action)Stop, 0);
			FsmUtil.AddCustomAction(fsm, "Update Map?", (Action)delegate
			{
				MixerLoader.SetSnapshot(Snapshots.Arena);
				AudioPlayer.TryPlayAudioFor("DREAMERS_INSPECT_RG6");
			});
			static IEnumerator DreamerAudio()
			{
				for (int i = 1; i < 6; i++)
				{
					MixerLoader.SetSnapshot(Snapshots.Arena);
					AudioPlayer.TryPlayAudioFor("DREAMERS_INSPECT_RG" + i);
					yield return (object)new WaitWhile((Func<bool>)(() => AudioPlayer.IsPlaying()));
				}
			}
			void Stop()
			{
				AudioPlayer.StopPlaying();
				((MonoBehaviour)fsm).StopCoroutine(dreamerCo);
			}
		}

		public static void PlayTHKPlaqueDialogue(PlayMakerFSM fsm)
		{
			FsmUtil.InsertCustomAction(fsm, "Stop", (Action)delegate
			{
				MixerLoader.SetSnapshot(Snapshots.Cliffs);
				AudioPlayer.TryPlayAudioFor("RUINS_FOUNTAIN");
			}, 0);
			FsmUtil.InsertCustomAction(fsm, "Fade Down", (Action)delegate
			{
				AudioPlayer.StopPlaying();
			}, 0);
			FsmUtil.AddCustomAction(fsm, "Update Map?", (Action)delegate
			{
				MixerLoader.SetSnapshot(Snapshots.Cliffs);
				AudioPlayer.TryPlayAudioFor("PROMPT_MAP_BLACKEGG");
			});
		}
	}
	public class UITextAudio
	{
		private static bool HunterNotesUnlocked = false;

		public static bool OpenShopMenu = false;

		public static bool OpenInvMenu = false;

		public static bool ShopMenuClosed = true;

		public static bool InvMenuClosed = true;

		public static bool AudioQueued = false;

		public static void Hook()
		{
			FSMEditData.AddGameObjectFsmEdit("Enemy List", "Item List Control", PlayJournalText);
			FSMEditData.AddGameObjectFsmEdit("Inv", "Update Text", PlayInventoryText);
			FSMEditData.AddGameObjectFsmEdit("Charms", "Update Text", PlayCharmText);
			FSMEditData.AddGameObjectFsmEdit("Item List", "Item List Control", PlayShopText);
			FSMEditData.AddGameObjectFsmEdit("Shop Menu", "shop_control", ShopMenuOpenClose);
			FSMEditData.AddGameObjectFsmEdit("Inventory", "Inventory Control", InventoryOpenClose);
			FSMEditData.AddGameObjectFsmEdit("Enemy List", "Item List Control Custom", PlayEquipmentText);
		}

		private static IEnumerator AudioStop()
		{
			yield return (object)new WaitUntil((Func<bool>)AudioPlayer.IsPlaying);
			AudioPlayer.StopPlaying();
		}

		public static void PlayCharmText(PlayMakerFSM fsm)
		{
			FsmUtil.AddFsmMethod(fsm, "Change Text", (Action)delegate
			{
				MixerLoader.SetSnapshot(Snapshots.Cliffs);
				fsm.PlayUIText("Convo Desc", UIAudioType.Other);
			});
		}

		public static void PlayInventoryText(PlayMakerFSM fsm)
		{
			FsmUtil.AddFsmMethod(fsm, "Change Text", (Action)delegate
			{
				MixerLoader.SetSnapshot(Snapshots.Cliffs);
				fsm.PlayUIText("Convo Desc", UIAudioType.Other);
			});
		}

		public static void PlayJournalText(PlayMakerFSM fsm)
		{
			FsmUtil.AddFsmMethod(fsm, "Display Kills", (Action)delegate
			{
				HunterNotesUnlocked = false;
			});
			FsmUtil.AddFsmMethod(fsm, "Get Notes", (Action)delegate
			{
				HunterNotesUnlocked = true;
			});
			FsmUtil.AddFsmMethod(fsm, "Get Details", (Action)delegate
			{
				((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(HunterWait(fsm));
			});
		}

		private static IEnumerator HunterWait(PlayMakerFSM fsm)
		{
			yield return (object)new WaitForSeconds(0.3f);
			((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(HunterCode(fsm));
		}

		private static IEnumerator HunterCode(PlayMakerFSM fsm)
		{
			string text = "";
			if (HunterNotesUnlocked)
			{
				text = "Item Notes Convo";
			}
			else if (!HunterNotesUnlocked)
			{
				text = "Item Desc Convo";
			}
			MixerLoader.SetSnapshot(Snapshots.Cliffs);
			fsm.PlayUIText(text, UIAudioType.Other);
			yield return text;
		}

		public static void PlayEquipmentText(PlayMakerFSM fsm)
		{
			FsmUtil.AddFsmMethod(fsm, "Get Details", (Action)delegate
			{
				MixerLoader.SetSnapshot(Snapshots.Cliffs);
				fsm.PlayUIText("Item Notes Convo", UIAudioType.Other);
			});
		}

		public static void ShopMenuOpenClose(PlayMakerFSM fsm)
		{
			FsmUtil.AddFsmMethod(fsm, "Open Window", (Action)delegate
			{
				if (HKVocals._globalSettings.dampenAudio)
				{
					DampenAudio.StartDampingAudioNormal();
				}
				OpenShopMenu = true;
				ShopMenuClosed = false;
			});
			FsmUtil.AddFsmMethod(fsm, "Down", (Action)delegate
			{
				if (HKVocals._globalSettings.dampenAudio)
				{
					DampenAudio.ForceStopDampenAudio();
				}
				if (AudioQueued)
				{
					((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(AudioStop());
				}
				AudioPlayer.StopPlaying();
				ShopMenuClosed = true;
				OpenShopMenu = false;
			});
		}

		public static void InventoryOpenClose(PlayMakerFSM fsm)
		{
			FsmUtil.AddFsmMethod(fsm, "Open", (Action)delegate
			{
				if (HKVocals._globalSettings.dampenAudio)
				{
					DampenAudio.StartDampingAudioNormal();
				}
				OpenInvMenu = true;
				InvMenuClosed = false;
			});
			FsmUtil.AddFsmMethod(fsm, "Close", (Action)delegate
			{
				if (HKVocals._globalSettings.dampenAudio)
				{
					DampenAudio.ForceStopDampenAudio();
				}
				if (AudioQueued)
				{
					((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(AudioStop());
				}
				AudioPlayer.StopPlaying();
				InvMenuClosed = true;
				OpenInvMenu = false;
			});
		}

		public static void PlayShopText(PlayMakerFSM fsm)
		{
			FsmUtil.AddFsmMethod(fsm, "Get Details Init", (Action)delegate
			{
				fsm.PlayUIText("Item Desc Convo", UIAudioType.Shop);
			});
			FsmUtil.AddFsmMethod(fsm, "Get Details", (Action)delegate
			{
				fsm.PlayUIText("Item Desc Convo", UIAudioType.Shop);
			});
		}
	}
	public enum UIAudioType
	{
		Shop,
		Other
	}
}
namespace HKVocals.EasterEggs
{
	public static class EternalOrdeal
	{
		private static GameObject ZoteLeverGo;

		private static string[] ZoteDialogues = new string[3] { "ZOTE_1", "ZOTE_2", "ZOTE_3" };

		private static string[] ZoteHit = new string[3] { "ZOTE_EO_HIT_A1_0", "ZOTE_EO_HIT_A2_0", "ZOTE_EO_HIT_A3_0" };

		private static string[] ZoteOw = new string[8] { "ZOTE_EO_OW_1_0", "ZOTE_EO_OW_2_0", "ZOTE_EO_OW_3_0", "ZOTE_EO_OW_4_0", "ZOTE_EO_OW_5_0", "ZOTE_EO_OW_6_0", "ZOTE_EO_OW_7_0", "ZOTE_EO_OW_8_0" };

		private static bool FirstFail = false;

		private static bool InStatueRoom = false;

		private static string ZoteFsmName = "Control";

		public static List<AudioSource> ZoteAudioPlayers = new List<AudioSource>();

		private static readonly List<string> Zote_Normal = new List<string> { "Zote Boss", "Zote Crew Normal (1)", "Zote Crew Normal (2)", "Zote Crew Normal (3)", "Zote Crew Normal (4)", "Zote Crew Fat (1)", "Zote Crew Fat (2)", "Zote Crew Fat (3)", "Zote Crew Tall (1)", "Zote Crew Tall" };

		private static readonly List<string> Zote_Balloon = new List<string> { "Zote Balloon (1)", "Zote Balloon Ordeal" };

		private static readonly List<string> Zote_Zoteling = new List<string> { "Ordeal Zoteling", "Ordeal Zoteling (1)" };

		private static readonly List<string> Zote_Other = new List<string> { "Zote Fluke", "Zote Salubra", "Zote Turret" };

		private static readonly List<string> Zote_Thowmp = new List<string> { "Zote Thwomp" };

		public static void Hook()
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Expected O, but got Unknown
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Expected O, but got Unknown
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Expected O, but got Unknown
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Expected O, but got Unknown
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Expected O, but got Unknown
			ModHooks.HeroUpdateHook += HeroUpdate;
			foreach (string item in Zote_Normal)
			{
				Hooks.HookStateEntered(new FSMData(item, ZoteFsmName, "Tumble Out"), (Action<PlayMakerFSM>)PlayRandomDialogue);
			}
			foreach (string item2 in Zote_Balloon)
			{
				Hooks.HookStateEntered(new FSMData(item2, ZoteFsmName, "Appear"), (Action<PlayMakerFSM>)PlayRandomDialogue);
			}
			foreach (string item3 in Zote_Zoteling)
			{
				Hooks.HookStateEntered(new FSMData(item3, ZoteFsmName, "Ball"), (Action<PlayMakerFSM>)PlayRandomDialogue);
			}
			foreach (string item4 in Zote_Other)
			{
				Hooks.HookStateEntered(new FSMData(item4, ZoteFsmName, "Antic"), (Action<PlayMakerFSM>)PlayRandomDialogue);
			}
			foreach (string item5 in Zote_Thowmp)
			{
				Hooks.HookStateEntered(new FSMData(item5, ZoteFsmName, "Antic Shake"), (Action<PlayMakerFSM>)PlayRandomDialogue);
			}
		}

		private static void ZoteFail(Scene From, Scene To)
		{
			if (!(((Scene)(ref From)).name == "GG_Mighty_Zote") || !(((Scene)(ref To)).name == "GG_Workshop"))
			{
				return;
			}
			if (FsmUtil.FindIntVariable(FSMUtility.LocateMyFSM(GameObject.Find("Battle Control"), "Kill Counter"), "Kills").Value < 57)
			{
				if (HKVocals._saveSettings.OrdealFails == 0)
				{
					ZoteLeverGo.SetActive(true);
					HKVocals._saveSettings.OrdealFails = 1;
				}
				else if (HKVocals._saveSettings.OrdealFails > 1)
				{
					HKVocals._saveSettings.OrdealFails++;
				}
				if (HKVocals._saveSettings.OrdealFails == 1)
				{
					if (!HKVocals._saveSettings.FinshedOrdealLines.Contains("ZOTE_EO_F_FIRST_0"))
					{
						FirstFail = true;
						HKVocals._globalSettings.ordealZoteSpeak = true;
						AudioPlayer.TryPlayAudioFor("ZOTE_EO_F_FIRST_0");
						((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(FirstFailZoteAudioChecks());
					}
				}
				else if (HKVocals._saveSettings.OrdealFails > 1)
				{
					if (!HKVocals._saveSettings.FinshedOrdealLines.Contains("ZOTE_EO_F_FIRST_0") || !HKVocals._saveSettings.FinshedOrdealLines.Contains("ZOTE_EO_HIT_FIRST_0"))
					{
						AudioPlayer.TryPlayAudioFor("ZOTE_EO_HIT_FIRST_0");
						HKVocals._saveSettings.FinshedOrdealLines.Add("ZOTE_EO_HIT_FIRST_0");
					}
					else
					{
						AudioPlayer.TryPlayAudioFor("ZOTE_EO_F_GENERIC_0");
					}
				}
			}
			else
			{
				_ = FsmUtil.FindIntVariable(FSMUtility.LocateMyFSM(GameObject.Find("Battle Control"), "Kill Counter"), "Kills").Value;
				_ = 57;
			}
		}

		private static IEnumerator FirstFailZoteAudioChecks()
		{
			yield return (object)new WaitUntil((Func<bool>)AudioPlayer.IsPlaying);
			((Loggable)HKVocals.instance).Log("firstfailcheck");
			if (AudioPlayer.IsPlaying() && !HKVocals._globalSettings.ordealZoteSpeak)
			{
				AudioPlayer.StopPlaying();
				ZoteRandomOw();
				FirstFail = false;
				((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(HitLeverAfter());
			}
			else if (!AudioPlayer.IsPlaying() && HKVocals._globalSettings.ordealZoteSpeak)
			{
				HKVocals._saveSettings.FinshedOrdealLines.Add("ZOTE_EO_F_FIRST_0");
				((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(ZoteListen());
				FirstFail = false;
			}
			else
			{
				((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(FirstFailZoteAudioChecks());
			}
		}

		private static IEnumerator HitLeverAfter()
		{
			if (InStatueRoom && HKVocals._globalSettings.ordealZoteSpeak)
			{
				AudioPlayer.TryPlayAudioFor("ZOTE_EO_HIT_FIRST_0");
				((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(HitLeverAfterChecks());
			}
			else if (InStatueRoom)
			{
				((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(HitLeverAfter());
			}
			yield break;
		}

		private static IEnumerator HitLeverAfterChecks()
		{
			yield return (object)new WaitUntil((Func<bool>)AudioPlayer.IsPlaying);
			if (AudioPlayer.IsPlaying() && !HKVocals._globalSettings.ordealZoteSpeak)
			{
				AudioPlayer.StopPlaying();
			}
			else if (!AudioPlayer.IsPlaying() && HKVocals._globalSettings.ordealZoteSpeak)
			{
				HKVocals._saveSettings.FinshedOrdealLines.Add("ZOTE_EO_HIT_FIRST_0");
			}
			else
			{
				((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(HitLeverAfterChecks());
			}
		}

		private static void HeroUpdate()
		{
			if (Input.GetKey((KeyCode)53) && Input.GetKey((KeyCode)55))
			{
				HKVocals._saveSettings.Precepts = 1;
				((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(ZotePrecepts());
			}
		}

		public static void PlaceZoteLever(Scene From, Scene To)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Expected O, but got Unknown
			if (((Scene)(ref To)).name == "GG_Workshop")
			{
				ZoteLeverGo = Object.Instantiate<GameObject>(GameObject.Find("GG_Statue_MantisLords/alt_lever/GG_statue_switch_lever"), new Vector3(196.8f, 63.5f, 1f), Quaternion.identity);
				((Object)ZoteLeverGo).name = "ZoteLever";
				ZoteLeverGo.GetComponent<BossStatueLever>().SetOwner(new BossStatue());
				if (HKVocals._saveSettings.OrdealFails >= 1)
				{
					ZoteLeverGo.SetActive(true);
				}
				else
				{
					ZoteLeverGo.SetActive(false);
				}
			}
		}

		private static IEnumerator HitLever()
		{
			if (FirstFail)
			{
				GameManager.instance.AwardAchievement("LastLaughOrdeal");
				AudioPlayer.TryPlayAudioFor(ZoteHit[Random.Range(0, 3)]);
				yield return (object)new WaitWhile((Func<bool>)AudioPlayer.IsPlaying);
				if (!HKVocals._saveSettings.FinshedOrdealLines.Contains("ZOTE_EO_PATIENCE_0"))
				{
					AudioPlayer.TryPlayAudioFor("ZOTE_EO_HIT_B2_0");
				}
				else if (HKVocals._saveSettings.Precepts < 57)
				{
					AudioPlayer.TryPlayAudioFor("ZOTE_EO_HIT_B3_0");
					HKVocals._saveSettings.Precepts = 1;
					((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(ZotePrecepts());
				}
				else
				{
					AudioPlayer.TryPlayAudioFor("ZOTE_EO_HIT_B1_0");
				}
			}
		}

		public static void UseZoteLever(orig_OnTriggerEnter2D orig, BossStatueLever self, Collider2D collision)
		{
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			if (((Object)((Component)self).gameObject).name != "ZoteLever")
			{
				orig.Invoke(self, collision);
			}
			else
			{
				if ((Object)(object)ZoteLeverGo == (Object)null || !ZoteLeverGo.activeInHierarchy || !((Component)collision).CompareTag("Nail Attack"))
				{
					return;
				}
				BossStatueLeverR ZoteLeverComponent = Extensions.Reflect(ZoteLeverGo.GetComponent<BossStatueLever>());
				if (!ZoteLeverComponent.canToggle)
				{
					return;
				}
				ZoteLeverComponent.canToggle = false;
				AudioEvent switchSound = ZoteLeverComponent.switchSound;
				((AudioEvent)(ref switchSound)).SpawnAndPlayOneShot(GetZoteAudioPlayer(), ((Component)HeroController.instance).transform.position);
				GameManager.instance.FreezeMoment(1);
				GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
				if (Object.op_Implicit((Object)(object)ZoteLeverComponent.strikeNailPrefab) && Object.op_Implicit((Object)(object)ZoteLeverComponent.hitOrigin))
				{
					ObjectPoolExtensions.Spawn(ZoteLeverComponent.strikeNailPrefab, ((Component)ZoteLeverComponent.hitOrigin).transform.position);
				}
				if (!Object.op_Implicit((Object)(object)ZoteLeverComponent.leverAnimator))
				{
					return;
				}
				ZoteLeverComponent.leverAnimator.Play("Hit");
				HKVocals._globalSettings.ordealZoteSpeak = !HKVocals._globalSettings.ordealZoteSpeak;
				CoroutineHelper.WaitForSecondsBeforeInvoke(1f, (Action)delegate
				{
					ZoteLeverComponent.canToggle = true;
					ZoteLeverComponent.leverAnimator.Play("Shine");
				});
			}
			if (!HKVocals._globalSettings.ordealZoteSpeak)
			{
				AudioPlayer.StopPlaying();
				ZoteRandomOw();
			}
			else if (HKVocals._globalSettings.ordealZoteSpeak)
			{
				((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(HitLever());
			}
		}

		public static void DeleteZoteAudioPlayersOnSceneChange(Scene from, Scene to)
		{
			if (((Scene)(ref from)).name == "GG_Mighty_Zote")
			{
				DeleteZoteAudioPlayers();
			}
		}

		public static AudioSource GetZoteAudioPlayer()
		{
			AudioSource val = ZoteAudioPlayers.FirstOrDefault((AudioSource p) => !p.isPlaying);
			if ((Object)(object)val == (Object)null)
			{
				AddZoteAudioPlayer();
				val = ZoteAudioPlayers.FirstOrDefault((AudioSource p) => !p.isPlaying);
			}
			return val;
		}

		public static void DeleteZoteAudioPlayers()
		{
			if (ZoteAudioPlayers.Count == 0)
			{
				return;
			}
			foreach (AudioSource zoteAudioPlayer in ZoteAudioPlayers)
			{
				Object.Destroy((Object)(object)zoteAudioPlayer);
			}
			ZoteAudioPlayers.Clear();
		}

		public static void AddZoteAudioPlayer()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			AudioSource val = new GameObject("HKVocal ZoteAudioPlayer").AddComponent<AudioSource>();
			val.outputAudioMixerGroup = MixerLoader.HKVAudioGroup;
			ZoteAudioPlayers.Add(val);
			Object.DontDestroyOnLoad((Object)(object)((Component)val).gameObject);
		}

		public static void PlayRandomDialogue(PlayMakerFSM fsm)
		{
			if (MiscUtils.GetCurrentSceneName() == "GG_Mighty_Zote" && HKVocals._globalSettings.ordealZoteSpeak && Random.value <= 0.4f)
			{
				AudioPlayer.TryPlayAudioFor(ZoteDialogues[Random.Range(0, 3)]);
			}
		}

		public static void ZoteRandomOw()
		{
			AudioPlayer.TryPlayAudioFor(ZoteOw[Random.Range(0, 8)]);
		}

		private static IEnumerator ZoteListen()
		{
			yield return (object)new WaitWhile((Func<bool>)AudioPlayer.IsPlaying);
			if (HKVocals._globalSettings.ordealZoteSpeak && (HKVocals._saveSettings.FinshedOrdealLines.Contains("ZOTE_EO_F_FIRST_0") || HKVocals._saveSettings.FinshedOrdealLines.Contains("ZOTE_EO_F_GENERIC_0")))
			{
				AudioPlayer.TryPlayAudioFor("");
				yield return (object)new WaitWhile((Func<bool>)AudioPlayer.IsPlaying);
				AudioPlayer.TryPlayAudioFor("ZOTE_EO_PATIENCE_0");
				yield return (object)new WaitWhile((Func<bool>)AudioPlayer.IsPlaying);
				((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(ZotePrecepts());
			}
		}

		private static IEnumerator ZotePrecepts()
		{
			yield return (object)new WaitWhile((Func<bool>)AudioPlayer.IsPlaying);
			if (HKVocals._saveSettings.Precepts == 1)
			{
				AudioPlayer.TryPlayAudioFor("PRECEPT_1_R_0");
			}
			else if (HKVocals._saveSettings.Precepts == 2)
			{
				AudioPlayer.TryPlayAudioFor("PRECEPT_2_0");
				yield return (object)new WaitWhile((Func<bool>)AudioPlayer.IsPlaying);
				AudioPlayer.TryPlayAudioFor("PRECEPT_2_1");
			}
			else if (HKVocals._saveSettings.Precepts == 55)
			{
				AudioPlayer.TryPlayAudioFor("PRECEPT_55_0");
				yield return (object)new WaitWhile((Func<bool>)AudioPlayer.IsPlaying);
				AudioPlayer.TryPlayAudioFor("PRECEPT_55_1");
			}
			else
			{
				if (HKVocals._saveSettings.Precepts == 57)
				{
					AudioPlayer.TryPlayAudioFor("PRECEPT_57_0");
					yield return (object)new WaitWhile((Func<bool>)AudioPlayer.IsPlaying);
					AudioPlayer.TryPlayAudioFor("PRECEPT_57_1");
					((Loggable)HKVocals.instance).Log("Stopped");
					yield break;
				}
				AudioPlayer.TryPlayAudioFor("PRECEPT_" + HKVocals._saveSettings.Precepts + "_0");
			}
			((Loggable)HKVocals.instance).Log("Audio Played");
			HKVocals._saveSettings.Precepts++;
			((Loggable)HKVocals.instance).Log((object)HKVocals._saveSettings.Precepts);
			((MonoBehaviour)HKVocals.CoroutineHolder).StartCoroutine(ZotePrecepts());
		}
	}
	public static class GhostRelics
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static hook_Start <0>__ShopMenuStock_Start;

			public static GetBoolProxy <1>__ModHooks_GetPlayerBoolHook;

			public static GetIntProxy <2>__ModHooks_GetPlayerIntHook;

			public static LanguageGetProxy <3>__ModHooks_LanguageGetHook;

			public static UnityAction<Scene, Scene> <4>__PlaceBox;

			public static Action<PlayMakerFSM> <5>__AddSpecialType;

			public static Action<PlayMakerFSM> <6>__EditConvo;

			public static Action<PlayMakerFSM> <7>__DontCloseShopGetAudio;

			public static Action<PlayMakerFSM> <8>__DontCloseShop;

			public static Func<DialogueCallbackOptions, DialogueOptions> <9>__BoxDialogue;
		}

		private static int[] relicAmts = new int[4] { 8, 7, 5, 4 };

		private static string[] relicNames = new string[4] { "JOURNAL", "SEAL", "IDOL", "EGG" };

		private static AudioClip denyClip;

		private static AudioClip acceptClip;

		private static GameObject audioPlayer;

		public static void Hook()
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
			object obj = <>O.<0>__ShopMenuStock_Start;
			if (obj == null)
			{
				hook_Start val = ShopMenuStock_Start;
				<>O.<0>__ShopMenuStock_Start = val;
				obj = (object)val;
			}
			ShopMenuStock.Start += (hook_Start)obj;
			object obj2 = <>O.<1>__ModHooks_GetPlayerBoolHook;
			if (obj2 == null)
			{
				GetBoolProxy val2 = ModHooks_GetPlayerBoolHook;
				<>O.<1>__ModHooks_GetPlayerBoolHook = val2;
				obj2 = (object)val2;
			}
			ModHooks.GetPlayerBoolHook += (GetBoolProxy)obj2;
			object obj3 = <>O.<2>__ModHooks_GetPlayerIntHook;
			if (obj3 == null)
			{
				GetIntProxy val3 = ModHooks_GetPlayerIntHook;
				<>O.<2>__ModHooks_GetPlayerIntHook = val3;
				obj3 = (object)val3;
			}
			ModHooks.GetPlayerIntHook += (GetIntProxy)obj3;
			object obj4 = <>O.<3>__ModHooks_LanguageGetHook;
			if (obj4 == null)
			{
				LanguageGetProxy val4 = ModHooks_LanguageGetHook;
				<>O.<3>__ModHooks_LanguageGetHook = val4;
				obj4 = (object)val4;
			}
			ModHooks.LanguageGetHook += (LanguageGetProxy)obj4;
			SceneManager.activeSceneChanged += PlaceBox;
			FSMEditData.AddGameObjectFsmEdit("UI List", "Confirm Control", AddSpecialType);
			FSMEditData.AddGameObjectFsmEdit("Relic Dealer", "Conversation Control", EditConvo);
			FSMEditData.AddGameObjectFsmEdit("Shop Menu", "shop_control", DontCloseShopGetAudio);
			FSMEditData.AddGameObjectFsmEdit("Shop Region", "Shop Region", DontCloseShop);
		}

		private static void ShopMenuStock_Start(orig_Start orig, ShopMenuStock self)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			((Loggable)HKVocals.instance).Log("Start: " + (object)self);
			Scene activeScene = SceneManager.GetActiveScene();
			if (((Scene)(ref activeScene)).name == "Ruins1_05b" && self.stock.Length == 4)
			{
				List<GameObject> list = new List<GameObject>(self.stock);
				for (int i = 0; i < 4; i++)
				{
					GameObject val = Object.Instantiate<GameObject>(self.stock[i]);
					ShopItemStats component = val.GetComponent<ShopItemStats>();
					((Component)val.transform.Find("Item Sprite")).GetComponent<SpriteRenderer>().color = new Color(0.85f, 0.85f, 0.85f, 0.6f);
					component.activeColour = new Color(0.85f, 0.85f, 0.85f, 0.6f);
					component.relic = true;
					component.relicNumber *= -1;
					component.cost = 0;
					component.priceConvo = "";
					component.descConvo = "SHOP_DESC_PHANTOM";
					component.relicPDInt = $"ghostRelic_{i + 1}";
					component.requiredPlayerDataBool = $"hasGhostRelic_{i + 1}";
					component.removalPlayerDataBool = "";
					component.playerDataBoolName = "false";
					list.Add(val);
				}
				self.stock = list.ToArray();
			}
			orig.Invoke(self);
		}

		private static bool ModHooks_GetPlayerBoolHook(string name, bool orig)
		{
			if (name == "false")
			{
				return false;
			}
			try
			{
				if (name.StartsWith("hasGhostRelic"))
				{
					int num = int.Parse(name.Split('_')[1]);
					return HKVocals._saveSettings.GhostRelics[num - 1] > 0;
				}
			}
			catch
			{
			}
			return orig;
		}

		private static int ModHooks_GetPlayerIntHook(string name, int orig)
		{
			try
			{
				if (name.StartsWith("ghostRelic"))
				{
					int num = int.Parse(name.Split('_')[1]);
					return HKVocals._saveSettings.GhostRelics[num - 1];
				}
			}
			catch
			{
			}
			return orig;
		}

		private static string ModHooks_LanguageGetHook(string key, string sheetTitle, string orig)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			switch (key)
			{
			case "RELICDEALER_SHOP_INTRO":
				if (!(((object)Language.CurrentLanguage()/*cast due to constrained. prefix*/).ToString() == "ZH"))
				{
					return orig + "<page>Though, you adventurous types are prone to rushing things, aren’t you? That chest over there, at the end of the room, it’s my deposit box for relics.<page>If you really have no inclination to learn about the history of the relics that you bring me, and you wish to go swinging that blasted nail around everywhere, simply put everything you find in that box.<page>Don’t worry, if you bring me anything worthwhile, I’ll pay you in full.<page>I’m busy enough organizing my collection behind the counter, so whenever you return with some spare time, hand me the relics you deposited and I’ll put them on display where they belong.";
				}
				return orig + "<page>你们这些冒险者总是急着赶路，不是吗？房间尽头的那个箱子，是我用来寄存遗物的箱子。<page>如果你无心了解带来遗物的历史，只想挥着那把该死的骨钉四处闯荡，就把找到的一切都放进那个箱子吧。<page>别担心，只要你带来有价值的东西，我都会全额付给你报酬。<page>我在柜台后整理藏品已经够忙了；等你有空回来时，把寄存的遗物交给我，我会将它们陈列在该在的位置。";
			case "SHOP_DESC_PHANTOM":
				if (!(((object)Language.CurrentLanguage()/*cast due to constrained. prefix*/).ToString() == "ZH"))
				{
					return "I see you left some relics in my deposit box. If you’d be so kind and bring them here, I’ll organize them on my shelves for display.";
				}
				return "我看见你在寄存箱里留了一些遗物。若你愿意把它们带来，我会在货架上整理并陈列它们。";
			case "SHOP_CHEST_DEPOSIT":
				if (!(((object)Language.CurrentLanguage()/*cast due to constrained. prefix*/).ToString() == "ZH"))
				{
					return "DEPOSIT";
				}
				return "寄存";
			case "SHOP_CHEST_NPC_SUPER":
			case "SHOP_CHEST_NPC_MAIN":
			case "SHOP_CHEST_NPC_SUB":
				return "";
			default:
				return orig;
			}
		}

		private static void PlaceBox(Scene arg0, Scene arg1)
		{
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			if (((Scene)(ref arg1)).name == "Ruins1_05b")
			{
				DialogueNPC val = DialogueNPC.CreateInstance();
				PlayMakerFSM obj = FSMUtility.LocateMyFSM(((Component)val).gameObject, "npc_control");
				FsmUtil.GetBoolVariable(obj, "Hero Always Right").Value = false;
				FsmUtil.GetFloatVariable(obj, "Move To Offset").Value = 0f;
				((Component)val).transform.position = new Vector3(58f, 23.4f, ((Component)val).transform.position.z);
				val.DialogueSelector = BoxDialogue;
				Object.Destroy((Object)(object)((Component)val).transform.Find("Dream Dialogue"));
				val.SetTitle("SHOP_CHEST_NPC");
				val.SetUp();
			}
		}

		private static void PlayAudioOneShot(AudioClip clip)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			((Loggable)HKVocals.instance).Log("Audio player: " + (object)audioPlayer);
			AudioSource component = ObjectPoolExtensions.Spawn(audioPlayer, ((Component)HeroController.instance).transform.position).GetComponent<AudioSource>();
			component.volume = 0.7f;
			component.pitch = 1.1f;
			component.PlayOneShot(clip);
		}

		private static DialogueOptions BoxDialogue(DialogueCallbackOptions options)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Invalid comparison between Unknown and I4
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			if (!options.Continue)
			{
				return new DialogueOptions
				{
					Key = "SHOP_CHEST_DEPOSIT",
					Sheet = "",
					Cost = 0,
					Type = (DialogueType)1,
					Continue = true
				};
			}
			if ((int)options.Response == 1)
			{
				if (PlayerDataAccess.trinket1 > 0 || PlayerDataAccess.trinket2 > 0 || PlayerDataAccess.trinket3 > 0 || PlayerDataAccess.trinket4 > 0)
				{
					GameManager.instance.AwardAchievement("ImpatientLemm");
					PlayAudioOneShot(acceptClip);
					HKVocals._saveSettings.UsedRelicBox = true;
					HeroController.instance.AddGeo(PlayerDataAccess.trinket1 * 200 + PlayerDataAccess.trinket2 * 800 + PlayerDataAccess.trinket3 * 450 + PlayerDataAccess.trinket4 * 1200);
					HKVocals._saveSettings.GhostRelics[0] += PlayerDataAccess.trinket1;
					HKVocals._saveSettings.GhostRelics[1] += PlayerDataAccess.trinket2;
					HKVocals._saveSettings.GhostRelics[2] += PlayerDataAccess.trinket3;
					HKVocals._saveSettings.GhostRelics[3] += PlayerDataAccess.trinket4;
					PlayerDataAccess.trinket1 = 0;
					PlayerDataAccess.trinket2 = 0;
					PlayerDataAccess.trinket3 = 0;
					PlayerDataAccess.trinket4 = 0;
				}
				else
				{
					PlayAudioOneShot(denyClip);
				}
			}
			return new DialogueOptions
			{
				Continue = false
			};
		}

		public static void AddSpecialType(PlayMakerFSM fsm)
		{
			FsmInt relicNum = FsmUtil.GetIntVariable(fsm, "Relic Number");
			FsmUtil.InsertMethod(fsm, "Sell Item", (Action)delegate
			{
				if (relicNum.Value < 0)
				{
					FsmInt obj = relicNum;
					obj.Value *= -1;
					HKVocals._saveSettings.GhostRelics[relicNum.Value - 1]--;
					fsm.SendEvent("FINISHED");
				}
			}, 0);
		}

		public static void EditConvo(PlayMakerFSM fsm)
		{
			FsmState val = FsmUtil.CopyState(fsm, "Shop Intro", "Box Discussion");
			FsmUtil.RemoveAction(val, 1);
			FsmUtil.AddMethod(val, (Action)delegate
			{
				HKVocals._saveSettings.RelicBoxConvo = true;
			});
			FsmUtil.GetAction<CallMethodProper>(val, 0).parameters[0].stringValue = "SHOP_DESC_PHANTOM";
			FsmState state = FsmUtil.GetState(fsm, "Convo Choice");
			FsmUtil.AddTransition(state, "BOX", val.Name);
			FsmUtil.InsertMethod(state, (Action)delegate
			{
				if (HKVocals._saveSettings.UsedRelicBox && !HKVocals._saveSettings.RelicBoxConvo)
				{
					fsm.SendEvent("BOX");
				}
			}, 4);
		}

		public static void DontCloseShopGetAudio(PlayMakerFSM fsm)
		{
			FsmUtil.InsertMethod(fsm, "Check Relics", (Action)delegate
			{
				((Loggable)HKVocals.instance).Log((object)HKVocals._saveSettings.GhostRelics.Sum());
				if (HKVocals._saveSettings.GhostRelics.Sum() > 0)
				{
					fsm.SendEvent("HAS RELIC");
				}
			}, 15);
			fsm = FSMUtility.LocateMyFSM(((Component)((Component)fsm).transform.Find("Item List")).gameObject, "Item List Control");
			Object value = FsmUtil.GetAction<AudioPlayerOneShotSingle>(fsm, "Can't Buy", 0).audioClip.Value;
			denyClip = (AudioClip)(object)((value is AudioClip) ? value : null);
			Object value2 = FsmUtil.GetAction<AudioPlayerOneShotSingle>(fsm, "Menu Down", 4).audioClip.Value;
			acceptClip = (AudioClip)(object)((value2 is AudioClip) ? value2 : null);
			audioPlayer = FsmUtil.GetAction<AudioPlayerOneShotSingle>(fsm, "Menu Down", 4).audioPlayer.Value;
		}

		public static void DontCloseShop(PlayMakerFSM fsm)
		{
			FsmUtil.InsertMethod(fsm, "Check Relics", (Action)delegate
			{
				((Loggable)HKVocals.instance).Log((object)HKVocals._saveSettings.GhostRelics.Sum());
				if (HKVocals._saveSettings.GhostRelics.Sum() > 0)
				{
					fsm.SendEvent("HAS RELIC");
				}
			}, 10);
		}
	}
	public static class PaleFlower
	{
		private class LurkerNPCFirst : MonoBehaviour
		{
			private DialogueNPC npc;

			private tk2dSpriteAnimator _anim;

			private Rigidbody2D _rb2d;

			private Collider2D _col;

			private HealthManager _hm;

			private Coroutine _turnCo;

			private bool waitForLeave = true;

			private GameObject audioPlayer;

			private void PlayAudioOneShot(AudioClip clip)
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				AudioSource component = ObjectPoolExtensions.Spawn(audioPlayer, ((Component)HeroController.instance).transform.position).GetComponent<AudioSource>();
				component.volume = 0.7f;
				component.pitch = 1.1f;
				component.PlayOneShot(clip);
			}

			private void Awake()
			{
				Object.Destroy((Object)(object)((Component)this).gameObject.GetComponent<DamageHero>());
				_anim = ((Component)this).GetComponent<tk2dSpriteAnimator>();
				_rb2d = ((Component)this).GetComponent<Rigidbody2D>();
				_col = ((Component)this).GetComponent<Collider2D>();
				_hm = ((Component)this).GetComponent<HealthManager>();
				((Behaviour)_hm).enabled = false;
				audioPlayer = FsmUtil.GetAction<AudioPlayerOneShotSingle>(FSMUtility.LocateMyFSM(((Component)this).gameObject, "Lurker Control"), "Slash 1", 1).audioPlayer.Value;
			}

			private void Invincible(bool invincible)
			{
				((Component)this).gameObject.layer = (invincible ? 13 : 11);
				((Behaviour)_hm).enabled = !invincible;
			}

			private void SetUpNPC()
			{
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				Invincible(invincible: true);
				npc = DialogueNPC.CreateInstance();
				((Component)npc).transform.position = ((Component)this).transform.position;
				npc.DialogueSelector = GetDialogue;
				((Renderer)((Component)npc).GetComponent<MeshRenderer>()).enabled = false;
				npc.SetDreamKey("LURKER_B4_0");
				npc.SetTitle("LURKER_NPC");
				npc.SetUp();
				PlayMakerFSM npc_control = FSMUtility.LocateMyFSM(((Component)npc).gameObject, "npc_control");
				FsmUtil.GetBoolVariable(npc_control, "Hero Always Right").Value = false;
				FsmUtil.GetFloatVariable(npc_control, "Move To Offset").Value = 2f;
				FsmUtil.AddMethod(npc_control, "Convo End", (Action)delegate
				{
					if (!HKVocals._saveSettings.LurkerFlower)
					{
						((MonoBehaviour)this).StartCoroutine(RunAwayAnim());
					}
				});
				FsmUtil.AddState(npc_control, "Wait For Shiny Collect");
				FsmUtil.ChangeTransition(npc_control, "Pause", "FINISHED", "Wait For Shiny Collect");
				FsmUtil.AddTransition(npc_control, "Wait For Shiny Collect", "SHINY PICKED UP", "Idle");
				FsmUtil.AddMethod(npc_control, "Wait For Shiny Collect", (Action)delegate
				{
					FsmUtil.ChangeTransition(npc_control, "Pause", "FINISHED", "Idle");
				});
			}

			private DialogueOptions GetDialogue(DialogueCallbackOptions prev)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Invalid comparison between Unknown and I4
				//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_020b: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_01da: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_016d: Unknown result type (might be due to invalid IL or missing references)
				//IL_019b: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_012e: Unknown result type (might be due to invalid IL or missing references)
				//IL_015c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0169: Unknown result type (might be due to invalid IL or missing references)
				if (!prev.Continue)
				{
					if (HKVocals._saveSettings.LurkerFlower)
					{
						return new DialogueOptions
						{
							Key = "LURKER_FLOWER_REPEAT",
							Sheet = "",
							Type = (DialogueType)0,
							Continue = true
						};
					}
					return new DialogueOptions
					{
						Key = "LURKER_DEFEAT",
						Sheet = "",
						Type = (DialogueType)0,
						Continue = true
					};
				}
				string key = prev.Key;
				if (!(key == "LURKER_DEFEAT"))
				{
					if (key == "LURKER_REQUEST")
					{
						if ((int)prev.Response == 1)
						{
							waitForLeave = false;
							if (PlayerData.instance.hasXunFlower)
							{
								if (!PlayerData.instance.xunFlowerBroken)
								{
									HKVocals._saveSettings.LurkerFlower = true;
									npc.SetDreamKey("LURKER_FLOWER_DREAM");
									PlayerData.instance.hasXunFlower = false;
									return new DialogueOptions
									{
										Key = "LURKER_FLOWER",
										Sheet = "",
										Wait = TakeFlowerAnim(),
										Type = (DialogueType)0,
										Continue = true
									};
								}
								return new DialogueOptions
								{
									Key = "LURKER_BROKENFLOWER",
									Sheet = "",
									Wait = TakePause(),
									Type = (DialogueType)0,
									Continue = true
								};
							}
							return new DialogueOptions
							{
								Key = "LURKER_UNINTERESTED",
								Sheet = "",
								Wait = TakePause(),
								Type = (DialogueType)0,
								Continue = true
							};
						}
						return new DialogueOptions
						{
							Continue = false
						};
					}
					return new DialogueOptions
					{
						Continue = false
					};
				}
				return new DialogueOptions
				{
					Key = "LURKER_REQUEST",
					Sheet = "",
					Cost = 0,
					Type = (DialogueType)1,
					Continue = true
				};
			}

			private IEnumerator TakePause()
			{
				yield return (object)new WaitForSeconds(2f);
			}

			private IEnumerator TakeFlowerAnim()
			{
				_anim.Play("Idle");
				yield return (object)new WaitForSeconds(1f);
				PlayAudioOneShot(Extensions.Reflect(HeroController.instance.normalSlash).audio.clip);
				yield return Extensions.PlayAnimWait(_anim, "SnatchFlower");
				_anim.Play("IdleFlower");
				yield return (object)new WaitForSeconds(1f);
				EnemyDeathEffects component = ((Component)this).GetComponent<EnemyDeathEffects>();
				Object.Instantiate<GameObject>(((Component)Extensions.Reflect(component).corpsePrefab.transform.GetChild(0)).gameObject, ((Component)this).transform);
				Extensions.RecordJournalEntry(component);
				((MonoBehaviour)this).StopCoroutine(_turnCo);
				GameManager.instance.AwardAchievement("KindnessPaleLurker");
			}

			private IEnumerator RunAwayAnim()
			{
				yield return (object)new WaitWhile((Func<bool>)(() => HeroController.instance.controlReqlinquished));
				Object.Destroy((Object)(object)((Component)npc).gameObject);
				Invincible(invincible: false);
				_col.isTrigger = false;
				if (waitForLeave)
				{
					yield return (object)new WaitForSeconds(5f);
				}
				yield return null;
				((MonoBehaviour)this).StopCoroutine(_turnCo);
				PlayMakerFSM obj = FSMUtility.LocateMyFSM(((Component)this).gameObject, "Lurker Control");
				((Behaviour)obj).enabled = true;
				obj.SetState("Dig 1");
				FsmUtil.ChangeTransition(obj, "Dig 2", "FINISHED", "Dormant");
				FsmUtil.AddMethod(obj, "Dormant", (Action)delegate
				{
					PlayMakerFSM.BroadcastEvent("SHINY PICKED UP");
					((Component)this).gameObject.SetActive(false);
				});
			}

			internal void StartWaitForRecoil()
			{
				((MonoBehaviour)this).StartCoroutine(WaitForRecoil());
			}

			private IEnumerator WaitForRecoil()
			{
				((Behaviour)FSMUtility.LocateMyFSM(((Component)this).gameObject, "Lurker Control")).enabled = false;
				(from t in ((Component)((Component)this).transform).GetComponentsInChildren<Transform>()
					where (Object)(object)t != (Object)(object)((Component)this).transform
					select t).ForEach(delegate(Transform t)
				{
					((Component)t).gameObject.SetActive(false);
				});
				_rb2d.velocity = new Vector2(7f * ((Component)this).transform.localScale.x, 10f);
				_anim.Play("Death Air");
				if (!PlayerData.instance.xunFlowerBroken)
				{
					((MonoBehaviour)this).StartCoroutine(SpawnEffects());
				}
				((Component)this).GetComponent<SpriteFlash>().flashWhiteLong();
				GameManager.instance.FreezeMoment(2);
				while (_rb2d.velocity != Vector2.zero)
				{
					_rb2d.velocity = new Vector2(_rb2d.velocity.y, _rb2d.velocity.x / (1f + Time.fixedDeltaTime / 10f));
					yield return (object)new WaitForFixedUpdate();
				}
				_rb2d.velocity = Vector2.zero;
				yield return Extensions.PlayAnimWait(_anim, "Land");
				((Component)this).gameObject.layer = 13;
				_rb2d.isKinematic = false;
				_rb2d.gravityScale = 0f;
				_col.isTrigger = true;
				_anim.Play("Idle");
				_turnCo = ((MonoBehaviour)this).StartCoroutine(AutoTurn());
				try
				{
					Extensions.Reflect(((Component)this).GetComponent<EnemyDeathEffects>()).audioSnapshotOnDeath.TransitionTo(2f);
				}
				catch
				{
				}
				SetUpNPC();
			}

			private IEnumerator SpawnEffects()
			{
				EnemyHitEffectsUninfected hitEffects = ((Component)this).GetComponent<EnemyHitEffectsUninfected>();
				Config config = new Config
				{
					AmountMin = 20,
					AmountMax = 30,
					SpeedMin = 20f,
					SpeedMax = 35f,
					AngleMin = 140f,
					AngleMax = 220f,
					OriginVariationX = 0f,
					OriginVariationY = 0f
				};
				for (int i = 0; i < 4; i++)
				{
					config.Prefab = hitEffects.slashEffectGhost1;
					FlingUtils.SpawnAndFling(config, ((Component)this).transform, hitEffects.effectOrigin);
					config.Prefab = hitEffects.slashEffectGhost2;
					FlingUtils.SpawnAndFling(config, ((Component)this).transform, hitEffects.effectOrigin);
					yield return null;
				}
			}

			private IEnumerator AutoTurn()
			{
				while (true)
				{
					if (((Component)this).transform.position.x - ((Component)HeroController.instance).transform.position.x > 0f)
					{
						if (((Component)this).transform.localScale.x < 0f)
						{
							yield return Extensions.PlayAnimWait(_anim, "Turn");
							((Component)this).transform.localScale = new Vector3(1f, 1f, 1f);
							_anim.Play(HKVocals._saveSettings.LurkerFlower ? "IdleFlower" : "Idle");
						}
					}
					else if (((Component)this).transform.localScale.x > 0f)
					{
						yield return Extensions.PlayAnimWait(_anim, "Turn");
						((Component)this).transform.localScale = new Vector3(-1f, 1f, 1f);
						_anim.Play(HKVocals._saveSettings.LurkerFlower ? "IdleFlower" : "Idle");
					}
					yield return null;
				}
			}
		}

		private class LurkerNPCSecond : MonoBehaviour
		{
			private DialogueNPC npc;

			private tk2dSpriteAnimator _anim;

			private int repeat = -1;

			private void Awake()
			{
				Extensions.SetPosition2D(((Component)this).transform, 224.6f, 52.5f);
				((Component)this).transform.parent = null;
				Object.Destroy((Object)(object)((Component)this).gameObject.GetComponent<DamageHero>());
				PlayMakerFSM[] components = ((Component)this).GetComponents<PlayMakerFSM>();
				for (int i = 0; i < components.Length; i++)
				{
					Object.Destroy((Object)(object)components[i]);
				}
				_anim = ((Component)this).GetComponent<tk2dSpriteAnimator>();
				((Component)this).gameObject.layer = 13;
				Rigidbody2D component = ((Component)this).GetComponent<Rigidbody2D>();
				component.isKinematic = false;
				component.gravityScale = 0f;
				((Component)this).GetComponent<Collider2D>().isTrigger = true;
				((Behaviour)((Component)this).GetComponent<HealthManager>()).enabled = false;
				SetUpNPC();
				_anim.Play("IdleFlower");
			}

			private void SetUpNPC()
			{
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				npc = DialogueNPC.CreateInstance();
				((Component)npc).transform.position = ((Component)this).transform.position;
				npc.DialogueSelector = GetDialogue;
				((Renderer)((Component)npc).GetComponent<MeshRenderer>()).enabled = false;
				npc.SetDreamKey("LURKER_IDLE_DREAM");
				npc.SetTitle("LURKER_NPC");
				npc.SetUp();
				PlayMakerFSM obj = FSMUtility.LocateMyFSM(((Component)npc).gameObject, "npc_control");
				FsmUtil.GetBoolVariable(obj, "Hero Always Right").Value = false;
				FsmUtil.GetBoolVariable(obj, "Hero Always Left").Value = true;
				FsmUtil.GetFloatVariable(obj, "Move To Offset").Value = 2f;
			}

			private DialogueOptions GetDialogue(DialogueCallbackOptions prev)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
				if (prev.Continue)
				{
					return new DialogueOptions
					{
						Continue = false
					};
				}
				if (HKVocals._saveSettings.LurkerConversation <= 4)
				{
					HKVocals._saveSettings.LurkerConversation++;
					return new DialogueOptions
					{
						Key = "LURKER_IDLE_" + HKVocals._saveSettings.LurkerConversation,
						Sheet = "",
						Cost = 0,
						Type = (DialogueType)0,
						Continue = true
					};
				}
				repeat++;
				if (repeat > 2)
				{
					repeat = 0;
				}
				return new DialogueOptions
				{
					Key = "LURKER_IDLE_REPEAT_" + repeat,
					Sheet = "",
					Cost = 0,
					Type = (DialogueType)0,
					Continue = true
				};
			}
		}

		[CompilerGenerated]
		private static class <>O
		{
			public static Func<Func<CustomItemList, Item, bool>, CustomItemList, Item, bool> <0>__ModifySFCoreFlower1;

			public static Func<Func<CustomItemList, int, string>, CustomItemList, int, string> <1>__ModifySFCoreFlower2;

			public static hook_Awake <2>__PaleLurkerAwake;

			public static UnityAction<Scene, Scene> <3>__SceneChanged;

			public static hook_Die <4>__PaleLurkerDie;

			public static LanguageGetProxy <5>__LurkerText;
		}

		private static tk2dSpriteCollectionData LurkerFlowerIdleSC;

		private static tk2dSpriteCollectionData LurkerFlowerSnatchSC;

		private static bool addedAnimations;

		public static void Hook()
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Expected O, but got Unknown
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Expected O, but got Unknown
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Expected O, but got Unknown
			new Hook((MethodBase)ReflectionHelper.GetMethodInfo(typeof(CustomItemList), "CheckBool", true), (Delegate)new Func<Func<CustomItemList, Item, bool>, CustomItemList, Item, bool>(ModifySFCoreFlower1));
			new Hook((MethodBase)ReflectionHelper.GetMethodInfo(typeof(CustomItemList), "GetDescConvo", true), (Delegate)new Func<Func<CustomItemList, int, string>, CustomItemList, int, string>(ModifySFCoreFlower2));
			object obj = <>O.<2>__PaleLurkerAwake;
			if (obj == null)
			{
				hook_Awake val = PaleLurkerAwake;
				<>O.<2>__PaleLurkerAwake = val;
				obj = (object)val;
			}
			HealthManager.Awake += (hook_Awake)obj;
			SceneManager.activeSceneChanged += SceneChanged;
			object obj2 = <>O.<4>__PaleLurkerDie;
			if (obj2 == null)
			{
				hook_Die val2 = PaleLurkerDie;
				<>O.<4>__PaleLurkerDie = val2;
				obj2 = (object)val2;
			}
			HealthManager.Die += (hook_Die)obj2;
			object obj3 = <>O.<5>__LurkerText;
			if (obj3 == null)
			{
				LanguageGetProxy val3 = LurkerText;
				<>O.<5>__LurkerText = val3;
				obj3 = (object)val3;
			}
			ModHooks.LanguageGetHook += (LanguageGetProxy)obj3;
			CreateCollections();
		}

		private static bool ModifySFCoreFlower1(Func<CustomItemList, Item, bool> orig, CustomItemList self, Item item)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if ((int)item.type == 4)
			{
				return PlayerData.instance.GetBool(item.playerdataBool1);
			}
			return orig(self, item);
		}

		private static string ModifySFCoreFlower2(Func<CustomItemList, int, string> orig, CustomItemList self, int itemNum)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Invalid comparison between Unknown and I4
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			Item val = self.List.First(delegate(Item x)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				return x.uniqueName.Equals(((Object)ReflectionHelper.GetField<CustomItemList, GameObject[]>(self, "_currentList")[itemNum]).name);
			});
			if ((int)val.type == 4)
			{
				bool num = PlayerData.instance.GetBool(val.playerdataBool2);
				bool flag = PlayerData.instance.GetBool(val.playerdataInt);
				if (!num)
				{
					if (!flag)
					{
						return val.descConvo1;
					}
					return val.descConvo2;
				}
				if (!flag)
				{
					return val.nameConvoBoth;
				}
				return val.descConvoBoth;
			}
			return orig(self, itemNum);
		}

		private static void PaleLurkerAwake(orig_Awake orig, HealthManager self)
		{
			orig.Invoke(self);
			if (((Object)self).name == "苍白潜伏者" && !addedAnimations)
			{
				AddAnimations(((Component)self).GetComponent<tk2dSpriteAnimator>().Library);
				addedAnimations = true;
			}
		}

		private static void SceneChanged(Scene from, Scene to)
		{
			if (HKVocals._saveSettings.LurkerFlower)
			{
				GameObject gameObject = ((Component)((Scene)(ref to)).GetRootGameObjects().First((GameObject g) => ((Object)g).name == "Lurker Control").transform.Find("苍白潜伏者")).gameObject;
				gameObject.AddComponent<LurkerNPCSecond>();
				gameObject.transform.parent = null;
				gameObject.SetActive(true);
				((Scene)(ref to)).GetRootGameObjects().First((GameObject g) => ((Object)g).name == "Corpse Pale Lurker").SetActive(false);
			}
		}

		private static void CreateCollections()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			Texture2D textureFromResources = AssemblyUtils.GetTextureFromResources("HKVocals.Resources.Pale_Lurker_Flower_Idle.png");
			Texture2D textureFromResources2 = AssemblyUtils.GetTextureFromResources("HKVocals.Resources.Pale_Lurker_Flower_Snatch.png");
			GameObject val = new GameObject("Custom Pale Lurker Idle Col");
			GameObject val2 = new GameObject("Custom Pale Lurker Snatch Col");
			Extensions.DontDestroyOnLoad((Object)(object)val);
			Extensions.DontDestroyOnLoad((Object)(object)val2);
			float num = (float)((Texture)textureFromResources).width / 6f;
			float num2 = ((Texture)textureFromResources).height;
			string[] array = new string[6];
			Rect[] array2 = (Rect[])(object)new Rect[6];
			Vector2[] array3 = (Vector2[])(object)new Vector2[6];
			for (int i = 0; i < 6; i++)
			{
				array[i] = i.ToString();
				array2[i] = new Rect(num * (float)i, 0f, num, num2);
				array3[i] = new Vector2(139f, 128f);
			}
			LurkerFlowerIdleSC = Utils.CreateFromTexture(val, (Texture)(object)textureFromResources, tk2dSpriteCollectionSize.PixelsPerMeter(66f), new Vector2(num * 6f, num2), array, array2, (Rect[])null, array3, new bool[6]);
			LurkerFlowerIdleSC.hasPlatformData = false;
			num = (float)((Texture)textureFromResources2).width / 5f;
			num2 = ((Texture)textureFromResources2).height;
			array = new string[5];
			array2 = (Rect[])(object)new Rect[5];
			array3 = (Vector2[])(object)new Vector2[5];
			for (int j = 0; j < 5; j++)
			{
				array[j] = j.ToString();
				array2[j] = new Rect(num * (float)j, 0f, num, num2);
				array3[j] = new Vector2(139f, 128f);
			}
			LurkerFlowerSnatchSC = Utils.CreateFromTexture(val2, (Texture)(object)textureFromResources2, tk2dSpriteCollectionSize.PixelsPerMeter(66f), new Vector2(num * 5f, num2), array, array2, (Rect[])null, array3, new bool[6]);
			LurkerFlowerSnatchSC.hasPlatformData = false;
		}

		public static void AddAnimations(tk2dSpriteAnimation animation)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Expected O, but got Unknown
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Expected O, but got Unknown
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Expected O, but got Unknown
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Expected O, but got Unknown
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Expected O, but got Unknown
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Expected O, but got Unknown
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Expected O, but got Unknown
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Expected O, but got Unknown
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Expected O, but got Unknown
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Expected O, but got Unknown
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Expected O, but got Unknown
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Expected O, but got Unknown
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Expected O, but got Unknown
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Expected O, but got Unknown
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Expected O, but got Unknown
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Expected O, but got Unknown
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Expected O, but got Unknown
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Expected O, but got Unknown
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Expected O, but got Unknown
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Expected O, but got Unknown
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Expected O, but got Unknown
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Expected O, but got Unknown
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Expected O, but got Unknown
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Expected O, but got Unknown
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Expected O, but got Unknown
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Expected O, but got Unknown
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Expected O, but got Unknown
			tk2dSpriteAnimationClip val = new tk2dSpriteAnimationClip(animation.GetClipByName("Idle"));
			val.name = "IdleFlower";
			val.frames = (tk2dSpriteAnimationFrame[])(object)new tk2dSpriteAnimationFrame[6]
			{
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerIdleSC,
					spriteId = 0
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerIdleSC,
					spriteId = 1
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerIdleSC,
					spriteId = 2
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerIdleSC,
					spriteId = 3
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerIdleSC,
					spriteId = 4
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerIdleSC,
					spriteId = 5
				}
			};
			tk2dSpriteAnimationClip element = val;
			val = new tk2dSpriteAnimationClip();
			val.name = "SnatchFlower";
			val.fps = 60f;
			val.wrapMode = (WrapMode)2;
			val.frames = (tk2dSpriteAnimationFrame[])(object)new tk2dSpriteAnimationFrame[21]
			{
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerSnatchSC,
					spriteId = 0
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerSnatchSC,
					spriteId = 0
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerSnatchSC,
					spriteId = 0
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerSnatchSC,
					spriteId = 0
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerSnatchSC,
					spriteId = 1
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerSnatchSC,
					spriteId = 1
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerSnatchSC,
					spriteId = 1
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerSnatchSC,
					spriteId = 1
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerSnatchSC,
					spriteId = 1
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerSnatchSC,
					spriteId = 2
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerSnatchSC,
					spriteId = 2
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerSnatchSC,
					spriteId = 2
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerSnatchSC,
					spriteId = 2
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerSnatchSC,
					spriteId = 2
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerSnatchSC,
					spriteId = 3
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerSnatchSC,
					spriteId = 3
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerSnatchSC,
					spriteId = 3
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerSnatchSC,
					spriteId = 4
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerSnatchSC,
					spriteId = 4
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerSnatchSC,
					spriteId = 4
				},
				new tk2dSpriteAnimationFrame
				{
					spriteCollection = LurkerFlowerSnatchSC,
					spriteId = 4
				}
			};
			tk2dSpriteAnimationClip element2 = val;
			animation.clips = animation.clips.Append(element).Append(element2).ToArray();
		}

		private static string LurkerText(string key, string sheetTitle, string orig)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			if (((object)Language.CurrentLanguage()/*cast due to constrained. prefix*/).ToString() != "ZH")
			{
				return orig;
			}
			return key switch
			{
				"LURKER_DEFEAT" => "……国王！……宝物！……我的！我的！……<page>累了……宝物！闪亮的宝物……", 
				"LURKER_REQUEST" => "要交出物品吗？", 
				"LURKER_UNINTERESTED" => "小玩意！……呜啊，黯淡的宝物……<page>钥匙！我的！走开，走开！", 
				"LURKER_BROKENFLOWER" => "……呜啊？……花茎！被剥光了。悲伤的花……<page>呜啊！悲伤的宝物！归于尘土！<page>国王！我的！走开，走开！", 
				"LURKER_FLOWER" => "呜哦哦！宝物！漂亮的宝物！闪亮的花……<page>苍白之物！脆弱？小心！小心，小心！<page>精巧之物。柔软的花瓣。呜哦哦……", 
				"LURKER_FLOWER_REPEAT" => "苍白之物！闪亮的宝物……", 
				"LURKER_FLOWER_DREAM" => "……呜哦哦，啊啊啊……", 
				"LURKER_IDLE_0" => "花！宝物！……柔软的花……", 
				"LURKER_IDLE_1" => "永不，永不凋零。苍白之物……", 
				"LURKER_IDLE_2" => "生长？不！我的，我的！", 
				"LURKER_IDLE_3" => "为了我！为了你？为了……国王？", 
				"LURKER_IDLE_4" => "不。为了我。为了我！", 
				"LURKER_IDLE_5" => "脆弱……柔软……纯净……", 
				"LURKER_IDLE_REPEAT_0" => "呜哦哦……苍白之物……", 
				"LURKER_IDLE_REPEAT_1" => "花！宝物！……柔软的花……", 
				"LURKER_IDLE_REPEAT_2" => "永不，永不凋零。苍白之物……", 
				"LURKER_IDLE_DREAM" => "嘿嘿嘿嘿。小东西，微小的东西！<page>但很温暖……", 
				"LURKER_B4_0" => "钥匙！宝物！走开，走开！", 
				"LURKER_NPC_SUPER" => "", 
				"LURKER_NPC_MAIN" => "苍白潜伏者", 
				"LURKER_NPC_SUB" => "", 
				_ => orig, 
			};
		}

		private static void PaleLurkerDie(orig_Die orig, HealthManager self, float? attackDirection, AttackTypes attackType, bool ignoreEvasion)
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			if (((Object)((Component)self).gameObject).name == "Pale Lurker")
			{
				if (!Object.op_Implicit((Object)(object)((Component)self).gameObject.GetComponent<LurkerNPCFirst>()))
				{
					((Component)self).gameObject.AddComponent<LurkerNPCFirst>().StartWaitForRecoil();
				}
				else if (((Behaviour)((Component)self).GetComponent<HealthManager>()).enabled)
				{
					orig.Invoke(self, attackDirection, attackType, ignoreEvasion);
				}
			}
			else
			{
				orig.Invoke(self, attackDirection, attackType, ignoreEvasion);
			}
		}
	}
	public static class SpecialGrub
	{
		private class OnDreamNail : MonoBehaviour
		{
			private void OnTriggerEnter2D(Collider2D col)
			{
				if (((Component)col).tag == "Dream Attack")
				{
					MixerLoader.SetSnapshot(Snapshots.Dream);
					if (HKVocals._saveSettings.GrubConvo < 8)
					{
						HKVocals._saveSettings.GrubConvo++;
						FsmUtil.GetFsmStringVariable(((Component)GameObject.Find("Grub Bottle").transform.GetChild(0).GetChild(0)).GetComponent<PlayMakerFSM>(), "Sheet Name").Value = "Elderbug";
						FsmUtil.GetFsmStringVariable(((Component)GameObject.Find("Grub Bottle").transform.GetChild(0).GetChild(0)).GetComponent<PlayMakerFSM>(), "Convo Name").Value = $"GRUB_BOTTLE_DREAM_S_{HKVocals._saveSettings.GrubConvo}";
						AudioPlayer.TryPlayAudioFor($"GRUB_BOTTLE_DREAM_S_{HKVocals._saveSettings.GrubConvo}");
					}
					else
					{
						GameManager.instance.AwardAchievement("DisdainGrub");
						FsmUtil.GetFsmStringVariable(((Component)GameObject.Find("Grub Bottle").transform.GetChild(0).GetChild(0)).GetComponent<PlayMakerFSM>(), "Convo Name").Value = "GRUB_BOTTLE_DREAM_S_REPEAT_0";
						FsmUtil.GetFsmStringVariable(((Component)GameObject.Find("Grub Bottle").transform.GetChild(0).GetChild(0)).GetComponent<PlayMakerFSM>(), "Sheet Name").Value = "Elderbug";
					}
				}
			}
		}

		[CompilerGenerated]
		private static class <>O
		{
			public static UnityAction<Scene, Scene> <0>__EditSpecialGrub;

			public static LanguageGetProxy <1>__GetSpecialGrubDialogue;
		}

		public static string SpeicalGrubSceneName = "Abyss_19";

		public static void Hook()
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			SceneManager.activeSceneChanged += EditSpecialGrub;
			object obj = <>O.<1>__GetSpecialGrubDialogue;
			if (obj == null)
			{
				LanguageGetProxy val = GetSpecialGrubDialogue;
				<>O.<1>__GetSpecialGrubDialogue = val;
				obj = (object)val;
			}
			ModHooks.LanguageGetHook += (LanguageGetProxy)obj;
		}

		public static void EditSpecialGrub(Scene From, Scene To)
		{
			if (((Scene)(ref To)).name == "Abyss_19")
			{
				((Component)GameObject.Find("Grub Bottle").transform.GetChild(0).GetChild(0)).gameObject.AddComponent<OnDreamNail>();
			}
		}

		public static string GetSpecialGrubDialogue(string key, string sheettitle, string orig)
		{
			return key switch
			{
				"GRUB_BOTTLE_DREAM_S_0" => " ...Home...", 
				"GRUB_BOTTLE_DREAM_S_1" => "Why does it stare at me so?", 
				"GRUB_BOTTLE_DREAM_S_2" => "Has it not come to release me? To save me from this cruel fate?", 
				"GRUB_BOTTLE_DREAM_S_3" => "Repetitively now its fist draws back, as if readying to shatter this invisible prison. But it only swipes the air.", 
				"GRUB_BOTTLE_DREAM_S_4" => "It wishes not to destroy my confines, but my pride.", 
				"GRUB_BOTTLE_DREAM_S_5" => "Does it really intend to mock and shame a helpless grub as I? What evil bug it must be, to knowingly prolong this torture, torn from my kin.", 
				"GRUB_BOTTLE_DREAM_S_6" => "From my... Grubfather.", 
				"GRUB_BOTTLE_DREAM_S_7" => "When the time is right and this bug least expects it...", 
				"GRUB_BOTTLE_DREAM_S_8" => "I will gladly return the favor.", 
				"GRUB_BOTTLE_DREAM_S_REPEAT_0" => "...", 
				_ => orig, 
			};
		}
	}
}
