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
using FiveKnights.BossManagement;
using FiveKnights.Dryya;
using FiveKnights.Hegemol;
using FiveKnights.Isma;
using FiveKnights.Misc;
using FiveKnights.Tiso;
using FiveKnights.Zemer;
using FrogCore;
using FrogCore.Ext;
using GlobalEnums;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using JetBrains.Annotations;
using Language;
using Microsoft.CodeAnalysis;
using Modding;
using Modding.Utils;
using MonoMod.ModInterop;
using Newtonsoft.Json;
using On;
using On.HutongGames.PlayMaker.Actions;
using On.Language;
using SFCore;
using SFCore.Generics;
using SFCore.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vasi;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: AssemblyCompany("PaleCourt")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyCopyright("Copyright © Hewlett-Packard 2020")]
[assembly: AssemblyFileVersion("1.1.1.7")]
[assembly: AssemblyInformationalVersion("1.1.1.7汉化")]
[assembly: AssemblyProduct("FiveKnights")]
[assembly: AssemblyTitle("FiveKnights")]
[assembly: AssemblyVersion("1.1.1.7")]
[module: System.Runtime.CompilerServices.RefSafetyRules(11)]
namespace Microsoft.CodeAnalysis
{
	[CompilerGenerated]
	[Microsoft.CodeAnalysis.Embedded]
	internal sealed class EmbeddedAttribute : Attribute
	{
	}
}
namespace System.Runtime.CompilerServices
{
	[CompilerGenerated]
	[Microsoft.CodeAnalysis.Embedded]
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
namespace FiveKnights
{
	internal class ArenaFinder : MonoBehaviour
	{
		private class StatueRaiseTrigger : MonoBehaviour
		{
			public GameObject DDStatue;

			private void Start()
			{
				if (PlayerData.instance.statueStateWhiteDefender.hasBeenSeen && FiveKnights.Instance.SaveSettings.HasSeenWorkshopRaised)
				{
					base.gameObject.transform.parent.gameObject.transform.position = new Vector3(56.46f, 28.11f, 2.1054f);
					DDStatue.transform.position = new Vector3(57.19f, 42.88f, 0.2f);
					SpawnPlatAndCrack();
				}
			}

			private void OnTriggerStay2D(Collider2D col)
			{
				if (PlayerData.instance.statueStateWhiteDefender.hasBeenSeen && !FiveKnights.Instance.SaveSettings.HasSeenWorkshopRaised)
				{
					FiveKnights.Instance.SaveSettings.HasSeenWorkshopRaised = true;
					StartCoroutine(RaisePlatform());
				}
			}

			private IEnumerator RaisePlatform()
			{
				PlayMakerFSM roarFSM = HeroController.instance.gameObject.LocateMyFSM("Roar Lock");
				roarFSM.GetFsmGameObjectVariable("Roar Object").Value = base.gameObject;
				roarFSM.SendEvent("ROAR ENTER");
				GameObject entrance = base.gameObject.transform.parent.gameObject;
				Transform parent = FiveKnights.preloadedGO["ObjRaise"].transform;
				GameCameras.instance.cameraShakeFSM.FsmVariables.FindFsmBool("RumblingMed").Value = true;
				GameObject raiseAud = UnityEngine.Object.Instantiate(parent.Find("Rise Audio").gameObject);
				raiseAud.SetActive(value: true);
				UnityEngine.Object.Destroy(raiseAud.GetComponent<PlayAudioAndRecycle>());
				HeroController.instance.PlayAudio(raiseAud.GetComponent<AudioSource>().clip);
				yield return new WaitForSeconds(0.5f);
				GameObject rumble = UnityEngine.Object.Instantiate(parent.Find("Rumble Dust").gameObject);
				rumble.SetActive(value: true);
				rumble.transform.position = new Vector3(56.1f, 35.1f);
				ParticleSystem rumblePartic = rumble.GetComponent<ParticleSystem>();
				rumblePartic.Play();
				GameObject rumble2 = UnityEngine.Object.Instantiate(parent.Find("Rumble Dust").gameObject);
				rumble2.SetActive(value: true);
				rumble2.transform.position = new Vector3(58.1f, 35.1f);
				ParticleSystem rumblePartic2 = rumble2.GetComponent<ParticleSystem>();
				rumblePartic2.Play();
				Vector3 ddstatEnd = new Vector3(57.19f, 42.88f, 0.2f);
				Vector3 pillarEnd = new Vector3(56.46f, 28.11f, 2.1054f);
				Vector3 ddstatStart = DDStatue.transform.position;
				Vector3 pillarStart = entrance.transform.position;
				float duration = 2.5f;
				float elapsedTime = 0f;
				while (elapsedTime < duration)
				{
					entrance.transform.position = Vector3.Lerp(pillarStart, pillarEnd, elapsedTime / duration);
					DDStatue.transform.position = Vector3.Lerp(ddstatStart, ddstatEnd, elapsedTime / duration);
					elapsedTime += Time.deltaTime;
					yield return new WaitForEndOfFrame();
				}
				entrance.transform.position = pillarEnd;
				DDStatue.transform.position = ddstatEnd;
				GameCameras.instance.cameraShakeFSM.FsmVariables.FindFsmBool("RumblingMed").Value = false;
				rumblePartic.Stop();
				rumblePartic2.Stop();
				GameObject dust = UnityEngine.Object.Instantiate(parent.Find("Rise Dust").gameObject);
				dust.SetActive(value: true);
				dust.transform.position = new Vector3(56.1f, 35.1f);
				dust.GetComponent<ParticleSystem>().Play();
				GameObject dust2 = UnityEngine.Object.Instantiate(parent.Find("Rise Dust").gameObject);
				dust2.SetActive(value: true);
				dust2.transform.position = new Vector3(59.3f, 35.1f);
				dust2.GetComponent<ParticleSystem>().Play();
				GameObject impactAud = UnityEngine.Object.Instantiate(parent.Find("Impact Audio").gameObject);
				impactAud.SetActive(value: true);
				UnityEngine.Object.Destroy(impactAud.GetComponent<PlayAudioAndRecycle>());
				HeroController.instance.PlayAudio(impactAud.GetComponent<AudioSource>().clip);
				SpawnPlatAndCrack();
				yield return new WaitForSeconds(0.3f);
				roarFSM.SendEvent("ROAR EXIT");
				dust.GetComponent<ParticleSystem>().Stop();
				dust2.GetComponent<ParticleSystem>().Stop();
				yield return new WaitForSeconds(2f);
				UnityEngine.Object.Destroy(rumble);
				UnityEngine.Object.Destroy(rumble2);
				UnityEngine.Object.Destroy(impactAud);
				UnityEngine.Object.Destroy(raiseAud);
				UnityEngine.Object.Destroy(dust);
				UnityEngine.Object.Destroy(dust2);
			}

			private void SpawnPlatAndCrack()
			{
				GameObject gameObject = base.gameObject.transform.parent.gameObject;
				foreach (Transform item in gameObject.transform.Find("Platforms"))
				{
					GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["RadPlat"]);
					obj.transform.position = item.position;
					obj.SetActive(value: true);
					PlayMakerFSM playMakerFSM = obj.LocateMyFSM("radiant_plat");
					playMakerFSM.SetState("Init");
					playMakerFSM.SendEvent("APPEAR");
				}
				gameObject.transform.Find("EntirePillar").Find("main pillar").Find("GG_pillar_top")
					.GetComponent<BoxCollider2D>()
					.enabled = true;
				GameObject obj2 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["StartDoor"]);
				UnityEngine.Object.Destroy(obj2.transform.Find("GG_secret_door").GetComponent<AudioSource>());
				obj2.SetActive(value: true);
				obj2.transform.position = new Vector3(57.45f, 38f, 4.21f);
				obj2.transform.localScale = new Vector3(1.33f, 1.02f, 0.87f);
				GameObject obj3 = obj2.transform.Find("GG_secret_door").gameObject;
				obj3.GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
				TransitionPoint component = obj3.transform.Find("door_Land_of_Storms").GetComponent<TransitionPoint>();
				component.targetScene = "White_Palace_09";
				component.entryPoint = "door_Land_of_Storms_return";
				obj3.transform.Find("door_Land_of_Storms").gameObject.LocateMyFSM("Door Control").FsmVariables.FindFsmString("New Scene").Value = "White_Palace_09";
				obj3.transform.Find("door_Land_of_Storms").gameObject.LocateMyFSM("Door Control").FsmVariables.FindFsmString("Entry Gate").Value = "door_Land_of_Storms_return";
				obj3.LocateMyFSM("Deactivate").enabled = false;
				obj3.SetActive(value: true);
			}
		}

		public static Dictionary<string, tk2dSpriteAnimation> spriteAnimations;

		public static Dictionary<string, tk2dSpriteCollection> spriteCollections;

		public static Dictionary<string, tk2dSpriteCollectionData> collectionData;

		public static int defeats;

		private GGBossManager _ggBossManager;

		private static bool hasSummonElevator;

		private string prevScene;

		private string currScene;

		public const string Isma2Scene = "GG_White_Defender";

		public const string GauntletArena = "Dream_04_White_Defender";

		public const string DryyaScene = "gg dryya";

		public const string ZemerScene = "gg zemer";

		public const string HegemolScene = "gg hegemol";

		public const string IsmaScene = "gg isma";

		private const string PrevFightScene = "White_Palace_09";

		private int lastBossLevel;

		private BossStatue lastBossStatue;

		private bool _allowInteract;

		public static Dictionary<string, Sprite> Sprites { get; private set; } = new Dictionary<string, Sprite>();

		private void Start()
		{
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneChanged;
			On.BossStatueLever.OnTriggerEnter2D += BossStatueLever_OnTriggerEnter2D2;
			On.GameManager.BeginSceneTransition += GameManager_BeginSceneTransition;
			On.BossChallengeUI.LoadBoss_int_bool += BossChallengeUI_LoadBoss_int_bool;
			On.BossSceneController.Awake += BossSceneController_Awake;
			On.GameManager.GetCurrentMapZone += GameManagerOnGetCurrentMapZone;
			On.HeroController.CanInteract += HeroCanInteract;
			spriteAnimations = new Dictionary<string, tk2dSpriteAnimation>();
			spriteCollections = new Dictionary<string, tk2dSpriteCollection>();
			collectionData = new Dictionary<string, tk2dSpriteCollectionData>();
			_allowInteract = true;
		}

		private bool HeroCanInteract(On.HeroController.orig_CanInteract orig, HeroController self)
		{
			Log("HeroCanInteract");
			if (_allowInteract)
			{
				return orig(self);
			}
			return false;
		}

		private string GameManagerOnGetCurrentMapZone(On.GameManager.orig_GetCurrentMapZone orig, GameManager self)
		{
			bool flag;
			switch (currScene)
			{
			case "gg zemer":
			case "gg dryya":
			case "gg isma":
			case "gg hegemol":
				flag = true;
				break;
			default:
				flag = false;
				break;
			}
			if (!flag)
			{
				return orig(self);
			}
			return MapZone.GODS_GLORY.ToString();
		}

		private void BossChallengeUI_LoadBoss_int_bool(On.BossChallengeUI.orig_LoadBoss_int_bool orig, BossChallengeUI self, int level, bool doHideAnim)
		{
			lastBossLevel = level;
			lastBossStatue = Mirror.GetField<BossChallengeUI, BossStatue>(self, "bossStatue");
			orig(self, level, doHideAnim);
		}

		private void BossSceneController_Awake(On.BossSceneController.orig_Awake orig, BossSceneController self)
		{
			if (BossSceneController.SetupEvent == null)
			{
				Log("BSC SetupEvent is null");
				BossSceneController.SetupEvent = delegate(BossSceneController bossSceneController)
				{
					bossSceneController.BossLevel = lastBossLevel;
					bossSceneController.DreamReturnEvent = "DREAM RETURN";
					bossSceneController.OnBossesDead += delegate
					{
						string fieldName = (lastBossStatue.UsingDreamVersion ? lastBossStatue.dreamStatueStatePD : lastBossStatue.statueStatePD);
						BossStatue.Completion playerDataVariable = GameManager.instance.GetPlayerDataVariable<BossStatue.Completion>(fieldName);
						switch (lastBossLevel)
						{
						case 0:
							playerDataVariable.completedTier1 = true;
							break;
						case 1:
							playerDataVariable.completedTier2 = true;
							break;
						case 2:
							playerDataVariable.completedTier3 = true;
							break;
						}
						GameManager.instance.SetPlayerDataVariable(fieldName, playerDataVariable);
						GameManager.instance.playerData.SetString("currentBossStatueCompletionKey", lastBossStatue.UsingDreamVersion ? lastBossStatue.dreamStatueStatePD : lastBossStatue.statueStatePD);
						GameManager.instance.playerData.SetInt("bossStatueTargetLevel", lastBossLevel);
					};
					bossSceneController.OnBossSceneComplete += delegate
					{
						bossSceneController.DoDreamReturn();
					};
				};
			}
			orig(self);
		}

		private void GameManager_BeginSceneTransition(On.GameManager.orig_BeginSceneTransition orig, GameManager self, GameManager.SceneLoadInfo info)
		{
			Log("Before: Going to " + info.SceneName + " from " + prevScene + " using gate " + info.EntryGateName);
			if (info.SceneName == "White_Palace_09")
			{
				if (CustomWP.boss == CustomWP.Boss.Isma || CustomWP.boss == CustomWP.Boss.Ogrim)
				{
					info.EntryGateName = "door_dreamReturnGGstatueStateIsma_GG_Statue_Isma";
				}
				else if (CustomWP.boss == CustomWP.Boss.Ze || CustomWP.boss == CustomWP.Boss.Mystic)
				{
					info.EntryGateName = "door_dreamReturnGGstatueStateZemer_GG_Statue_Zemer";
				}
				else if (CustomWP.boss == CustomWP.Boss.All)
				{
					info.EntryGateName = "door_dreamReturnGGTestingIt";
				}
				else if (CustomWP.boss == CustomWP.Boss.Dryya)
				{
					info.EntryGateName = "door_dreamReturnGGstatueStateDryya_GG_Statue_Dryya";
				}
				else if (CustomWP.boss == CustomWP.Boss.Hegemol)
				{
					info.EntryGateName = "door_dreamReturnGGstatueStateHegemol_GG_Statue_Hegemol";
				}
			}
			else if (prevScene == "Dream_04_White_Defender" && info.SceneName == prevScene)
			{
				Log("in here boi");
				info.SceneName = "White_Palace_09";
				info.EntryGateName = "door_dreamReturnGGTestingIt";
			}
			ModHooks.GetPlayerBoolHook -= GetPlayerBoolHook;
			if (info.SceneName == "White_Palace_09" && prevScene != "White_Palace_13")
			{
				ModHooks.GetPlayerBoolHook += GetPlayerBoolHook;
			}
			Log("After: Going to " + info.SceneName + " from " + prevScene + " using gate " + info.EntryGateName);
			prevScene = info.SceneName;
			currScene = info.SceneName;
			orig(self, info);
		}

		private void BossStatueLever_OnTriggerEnter2D2(On.BossStatueLever.orig_OnTriggerEnter2D orig, BossStatueLever self, Collider2D collision)
		{
			Vector2 vector = HeroController.instance.transform.position;
			if (vector.x > 49f && vector.x < 62.2f && vector.y > 35f && collision.tag == "Nail Attack")
			{
				self.switchSound.SpawnAndPlayOneShot(self.audioPlayerPrefab, base.transform.position);
				GameManager.instance.FreezeMoment(1);
				GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
				if ((bool)self.strikeNailPrefab && (bool)self.hitOrigin)
				{
					self.strikeNailPrefab.Spawn(self.hitOrigin.transform.position);
				}
				if ((bool)self.leverAnimator)
				{
					self.leverAnimator.Play("Hit", -1, 0f);
				}
				hasSummonElevator = true;
			}
			else
			{
				orig(self, collision);
			}
		}

		private void CreateLever()
		{
			GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["StatueMed"].FindGameObjectInChildren("alt_lever"));
			Vector3 localScale = obj.transform.localScale;
			obj.transform.localScale = new Vector3(localScale.x * -1f, localScale.y, localScale.z);
			obj.SetActive(value: true);
			obj.FindGameObjectInChildren("GG_statue_switch_bracket").SetActive(value: true);
			GameObject obj2 = obj.FindGameObjectInChildren("GG_statue_switch_lever");
			obj2.SetActive(value: true);
			obj2.GetComponent<BossStatueLever>().SetState(value: true);
			obj.transform.position = new Vector2(57.4f, 37.5f);
		}

		private IEnumerator DelayInspection()
		{
			_allowInteract = false;
			bool change = false;
			tk2dSpriteAnimator tkAnim = HeroController.instance.GetComponent<tk2dSpriteAnimator>();
			string animName = "Exit Door To Idle";
			while (!change || !(tkAnim.CurrentClip.name == "Idle"))
			{
				if (!change && tkAnim.CurrentClip.name == animName)
				{
					change = true;
				}
				yield return null;
			}
			yield return new WaitForSeconds(1f);
			_allowInteract = true;
		}

		private void SceneChanged(Scene from, Scene to)
		{
			bool flag;
			switch (from.name)
			{
			case "gg dryya":
			case "gg zemer":
			case "Menu_Title":
			case "gg hegemol":
			case "gg isma":
			case "GG_White_Defender":
			case "hidden_reward_room":
			case "Dream_04_White_Defender":
			case "GG_Workshop":
				flag = true;
				break;
			default:
				flag = false;
				break;
			}
			if (!flag && to.name == "White_Palace_09")
			{
				CustomWP.isInGodhome = false;
				return;
			}
			if ((from.name == "GG_Workshop" && to.name == "White_Palace_09") || (from.name == "White_Palace_09" && to.name == "GG_Workshop"))
			{
				StartCoroutine(DelayInspection());
			}
			switch (to.name)
			{
			case "gg dryya":
			case "gg isma":
			case "gg hegemol":
			case "gg zemer":
				flag = true;
				break;
			default:
				flag = false;
				break;
			}
			if (flag)
			{
				StartCoroutine(wow());
				Log("Done dream entry");
			}
			if (from.name == "White_Palace_09")
			{
				UnityEngine.Object.Destroy(CustomWP.Instance);
				CustomWP.Instance = null;
			}
			if (to.name == "GG_Workshop")
			{
				StartCoroutine(CameraFixer());
				Arena();
			}
			flag = CustomWP.boss != CustomWP.Boss.None;
			if (flag)
			{
				bool flag2;
				switch (to.name)
				{
				case "gg dryya":
				case "gg isma":
				case "gg zemer":
				case "gg hegemol":
				case "GG_White_Defender":
					flag2 = true;
					break;
				default:
					flag2 = false;
					break;
				}
				flag = flag2;
			}
			if (flag)
			{
				SetSceneSettings(to);
				StartCoroutine(AddComponent());
			}
			flag = to.name == "White_Palace_09";
			if (flag)
			{
				bool flag2;
				switch (from.name)
				{
				case "gg dryya":
				case "gg zemer":
				case "gg isma":
				case "gg hegemol":
				case "GG_White_Defender":
				case "Dream_04_White_Defender":
				case "hidden_reward_room":
					flag2 = true;
					break;
				default:
					flag2 = false;
					break;
				}
				flag = flag2;
			}
			if (flag)
			{
				PlayerData.instance.isInvincible = false;
				HeroController.instance.MaxHealth();
				PlayerData.instance.UpdateBlueHealth();
				HeroController.instance.ClearMPSendEvents();
			}
			if ((from.name == "White_Palace_09" && to.name == "Dream_04_White_Defender") || (from.name == "Dream_04_White_Defender" && to.name == "Dream_04_White_Defender" && CustomWP.boss == CustomWP.Boss.All))
			{
				StartCoroutine(AddComponent());
				StartCoroutine(CameraFixer());
				HeroController.instance.MaxHealth();
				PlayerData.instance.UpdateBlueHealth();
				HeroController.instance.ClearMPSendEvents();
				HeroController.instance.EnterWithoutInput(flag: true);
			}
			if (to.name == "White_Palace_09" && from.name == "Dream_04_White_Defender")
			{
				PlayerData.instance.whiteDefenderDefeats = defeats;
			}
			if (to.name == "White_Palace_09")
			{
				CustomWP.isInGodhome = true;
				LoadHubBundles();
				if (CustomWP.Instance == null)
				{
					GameManager.instance.gameObject.AddComponent<CustomWP>();
				}
				StartCoroutine(CameraFixer());
				MakeBench(to.name, "WhiteBenchNew2", new Vector3(110.6f, 94.1f, 1f));
			}
			if (_ggBossManager != null && to.name != "Dream_04_White_Defender" && to.name != "GG_White_Defender" && to.name != "gg dryya" && to.name != "gg isma" && to.name != "gg hegemol" && to.name != "gg zemer")
			{
				Log("Destroying GGBossManager");
				if (_ggBossManager != null)
				{
					UnityEngine.Object.Destroy(_ggBossManager);
					Log("Killed GGBossManager");
				}
			}
			IEnumerator wow()
			{
				Material[] array = new Material[1]
				{
					new Material(Shader.Find("UI/Blur/UIBlur"))
				};
				array[0].SetColor(Shader.PropertyToID("_TintColor"), new Color(1f, 1f, 1f, 0f));
				array[0].SetFloat(Shader.PropertyToID("_Size"), 53.7f);
				array[0].SetFloat(Shader.PropertyToID("_Vibrancy"), 0.2f);
				array[0].SetInt(Shader.PropertyToID("_StencilComp"), 8);
				array[0].SetInt(Shader.PropertyToID("_Stencil"), 0);
				array[0].SetInt(Shader.PropertyToID("_StencilOp"), 0);
				array[0].SetInt(Shader.PropertyToID("_StencilWriteMask"), 255);
				array[0].SetInt(Shader.PropertyToID("_StencilReadMask"), 255);
				foreach (GameObject item in from x in UnityEngine.Object.FindObjectsOfType<GameObject>()
					where x.name == "BlurPlane"
					select x)
				{
					Log("Found blur!");
					item.SetActive(value: false);
					item.GetComponent<MeshRenderer>().materials = array;
					item.SetActive(value: true);
				}
				GrassCut[] array2 = UnityEngine.Object.FindObjectsOfType<GrassCut>();
				foreach (GrassCut grassCut in array2)
				{
					string key = "Grass0";
					if (grassCut.gameObject.name.Contains("green_grass_1"))
					{
						key = "Grass1";
					}
					else if (grassCut.gameObject.name.Contains("green_grass_2"))
					{
						key = "Grass2";
					}
					else if (grassCut.gameObject.name.Contains("green_grass_3"))
					{
						key = "Grass3";
					}
					GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO[key]);
					obj.SetActive(value: true);
					obj.transform.position = grassCut.transform.position;
					UnityEngine.Object.Destroy(grassCut.gameObject);
				}
				Breakable[] array3 = (from x in UnityEngine.Object.FindObjectsOfType<Breakable>()
					where x.name == "cliffs_pole (2)"
					select x).ToArray();
				if (array3.Length != 0 && array3[0] != null)
				{
					UnityEngine.Object.Destroy(array3[0]);
				}
				foreach (GameObject item2 in from x in UnityEngine.Object.FindObjectsOfType<GameObject>()
					where x.name.Contains("ruin_water_bounce")
					select x)
				{
					if (item2.name.Contains("21") || item2.name.Contains("22") || item2.name.Contains("23") || item2.name.Contains("24"))
					{
						UnityEngine.Object.Destroy(item2);
					}
				}
				UnityEngine.Object.Destroy(GameObject.Find("Boss Scene Controller"));
				FiveKnights.preloadedGO["BSCW"].SetActive(value: false);
				GameObject bsc = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["BSCW"]);
				bsc.SetActive(value: false);
				BossSceneController.Instance.transitionInHoldTime = 0f;
				PlayMakerFSM fsm = bsc.FindGameObjectInChildren("Dream Entry").LocateMyFSM("Control");
				fsm.RemoveAction("Pause", 0);
				fsm.AddAction("Pause", new HutongGames.PlayMaker.Actions.NextFrameEvent
				{
					sendEvent = FsmEvent.Finished
				});
				bsc.SetActive(value: true);
				yield return null;
				EventRegister.SendEvent("GG TRANSITION IN");
				BossSceneController.Instance.GetType().GetProperty("HasTransitionedIn").SetValue(BossSceneController.Instance, true, null);
				string text = to.name;
				if ((text == "gg zemer" || text == "gg hegemol") ? true : false)
				{
					bsc.GetComponent<BossSceneController>().heroSpawn.position = new Vector3(25f, 27.4f);
					HeroController.instance.transform.position = new Vector3(25f, 27.4f);
					Log("Changed the hero's pos");
				}
				Log("Done trans in dream thing");
				while (GameObject.Find("Godseeker Crowd") == null)
				{
					yield return null;
				}
				GameObject gameObject = GameObject.Find("Godseeker Crowd");
				GameObject obj2 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Godseeker"]);
				obj2.SetActive(value: true);
				obj2.transform.position = gameObject.transform.position;
				UnityEngine.Object.Destroy(gameObject);
				GameObject.Find("GG_Arena_Prefab").GetComponent<AudioSource>().outputAudioMixerGroup = HeroController.instance.GetComponent<AudioSource>().outputAudioMixerGroup;
			}
		}

		private bool GetPlayerBoolHook(string name, bool orig)
		{
			if (name == "gotKingFragment")
			{
				return true;
			}
			return orig;
		}

		private IEnumerator CameraFixer()
		{
			yield return new WaitWhile(() => GameManager.instance.gameState != GameState.PLAYING);
			do
			{
				GameCameras.instance.cameraFadeFSM.SetState("FadeIn");
				yield return new WaitForSeconds(1f);
			}
			while (GameCameras.instance.cameraFadeFSM.ActiveStateName != "Normal");
		}

		private void MakeBench(string scene, string name, Vector3 pos)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Bench"]);
			gameObject.transform.position = pos;
			gameObject.SetActive(value: true);
			PlayMakerFSM playMakerFSM = gameObject.LocateMyFSM("Bench Control");
			playMakerFSM.FsmVariables.FindFsmString("Scene Name").Value = scene;
			playMakerFSM.FsmVariables.FindFsmString("Spawn Name").Value = name;
			playMakerFSM.FsmVariables.FindFsmVector3("Sit Vector").Value = new Vector3(0f, 0.5f, 0f);
			PlayerData.instance.respawnScene = "White_Palace_09";
			PlayerData.instance.respawnMarkerName = gameObject.name;
		}

		private IEnumerator AddComponent()
		{
			yield return null;
			_ggBossManager = GameManager.instance.gameObject.AddComponent<GGBossManager>();
		}

		private void LoadHubBundles()
		{
			FiveKnights.preloadedGO["hubfloor"] = ABManager.AssetBundles[ABManager.Bundle.GArenaHub2].LoadAsset<GameObject>("white_palace_floor_set_02 (16)");
			AssetBundle assetBundle = ABManager.AssetBundles[ABManager.Bundle.Misc];
			FiveKnights.Materials["WaveEffectMaterial"] = assetBundle.LoadAsset<Material>("WaveEffectMaterial");
			GameObject[] array = assetBundle.LoadAllAssets<GameObject>();
			foreach (GameObject gameObject in array)
			{
				if (gameObject.name == "GG_Statue_Isma")
				{
					FiveKnights.preloadedGO["GG_Statue_Isma"] = gameObject;
				}
				else if (gameObject.name == "IsmaOgrimStatue")
				{
					FiveKnights.preloadedGO["IsmaOgrimStatue"] = gameObject;
				}
				else if (gameObject.name == "IsmaOgrimStatue_Silver")
				{
					FiveKnights.preloadedGO["IsmaOgrimStatue_Silver"] = gameObject;
				}
				else if (gameObject.name == "Shockwave")
				{
					FiveKnights.preloadedGO["WaveShad"] = gameObject;
				}
				if (gameObject.GetComponent<SpriteRenderer>() == null)
				{
					SpriteRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						componentsInChildren[j].material = new Material(Shader.Find("Sprites/Default"));
					}
				}
				else
				{
					gameObject.GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
				}
			}
			Texture value = assetBundle.LoadAsset<Texture>("sonar");
			FiveKnights.Materials["TestDist"] = new Material(FiveKnights.Materials["WaveEffectMaterial"]);
			FiveKnights.Materials["TestDist"].SetTexture("_NoiseTex", value);
			FiveKnights.Materials["TestDist"].SetFloat("_Intensity", 0.2f);
			FiveKnights.preloadedGO["WaveShad"].GetComponent<SpriteRenderer>().material = FiveKnights.Materials["TestDist"];
			Log("Loading hub bundle");
		}

		private void Arena()
		{
			CustomWP.boss = CustomWP.Boss.None;
			GameObject gameObject = GameObject.Find("GG_Statue_Defender");
			gameObject.transform.position = new Vector3(57.19f, 36.34f, 0.2f);
			USceneUtil.Find(USceneUtil.Find(gameObject, "Spotlight"), "Glow Response statue_beam").GetComponent<BoxCollider2D>().size = new Vector2(11f, 100f);
			CreateCameraLock("CameraLockStat", new Vector2(57.6f, 49.7f), new Vector2(1f, 1f), new Vector2(7.5f, 16f), new Vector2(0f, 0f), new Vector2(-1f, 43f), new Vector2(-1f, 43f));
			FiveKnights.preloadedGO["Entrance"] = ABManager.AssetBundles[ABManager.Bundle.WSArena].LoadAsset<GameObject>("gg_workshop_pale_court_entrance");
			GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Entrance"]);
			obj.SetActive(value: true);
			obj.transform.position = new Vector3(56.46f, 20.3f, 2.6f);
			obj.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
			obj.transform.Find("StartTrigger").gameObject.AddComponent<StatueRaiseTrigger>().DDStatue = gameObject;
			UnityEngine.Object.Destroy(GameObject.Find("GG_pillar_bg (5)"));
			UnityEngine.Object.Destroy(GameObject.Find("GG_statues_0003_2 (5)"));
		}

		private void SetSceneSettings(Scene scene)
		{
			GameObject gameObject = null;
			SceneManager[] array = UnityEngine.Object.FindObjectsOfType<SceneManager>();
			foreach (SceneManager obj in array)
			{
				gameObject = obj.borderPrefab;
				UnityEngine.Object.Destroy(obj.gameObject);
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["SMTest"]);
			SceneManager component = gameObject2.GetComponent<SceneManager>();
			if (gameObject != null)
			{
				component.borderPrefab = gameObject;
			}
			component.noLantern = true;
			component.darknessLevel = -1;
			component.sceneType = SceneType.GAMEPLAY;
			component.saturation = 0.78f;
			component.defaultIntensity = 0.968f;
			component.defaultColor = new Color(0.934f, 0.961f, 0.961f, 1f);
			component.mapZone = MapZone.GODS_GLORY;
			component.noParticles = true;
			switch (scene.name)
			{
			case "gg zemer":
				component.environmentType = 7;
				break;
			case "gg dryya":
			case "gg isma":
				component.environmentType = 1;
				break;
			case "gg hegemol":
				component.environmentType = 0;
				break;
			}
			gameObject2.SetActive(value: true);
		}

		private void CreateCameraLock(string n, Vector2 pos, Vector2 scl, Vector2 cSize, Vector2 cOff, Vector2 min, Vector2 max, bool preventLookDown = false)
		{
			GameObject obj = new GameObject(n);
			BoxCollider2D boxCollider2D = obj.AddComponent<BoxCollider2D>();
			CameraLockArea cameraLockArea = obj.AddComponent<CameraLockArea>();
			obj.transform.position = pos;
			obj.transform.localScale = scl;
			boxCollider2D.isTrigger = true;
			boxCollider2D.size = cSize;
			boxCollider2D.offset = cOff;
			cameraLockArea.cameraXMin = min.x;
			cameraLockArea.cameraXMax = max.x;
			cameraLockArea.cameraYMin = (cameraLockArea.cameraYMax = min.y);
			cameraLockArea.preventLookDown = preventLookDown;
			cameraLockArea.maxPriority = true;
			obj.SetActive(value: true);
			bool flag = (cameraLockArea.enabled = true);
			boxCollider2D.enabled = flag;
		}

		private void OnDestroy()
		{
			Log("Destroyed ArenaFinder");
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= SceneChanged;
			ModHooks.GetPlayerBoolHook -= GetPlayerBoolHook;
			On.BossStatueLever.OnTriggerEnter2D -= BossStatueLever_OnTriggerEnter2D2;
			On.GameManager.BeginSceneTransition -= GameManager_BeginSceneTransition;
			On.BossChallengeUI.LoadBoss_int_bool -= BossChallengeUI_LoadBoss_int_bool;
			On.BossSceneController.Awake -= BossSceneController_Awake;
			On.GameManager.GetCurrentMapZone -= GameManagerOnGetCurrentMapZone;
			On.HeroController.CanInteract -= HeroCanInteract;
		}

		private void Log(object o)
		{
			Modding.Logger.Log("[Scene] " + o);
		}
	}
	public static class BossLoader
	{
		private static AssetBundle _soundBundle => ABManager.AssetBundles[ABManager.Bundle.Sound];

		private static AssetBundle _miscBundle => ABManager.AssetBundles[ABManager.Bundle.Misc];

		private static AssetBundle _ismaBundle => ABManager.AssetBundles[ABManager.Bundle.GIsma];

		private static AssetBundle _dryyaBundle => ABManager.AssetBundles[ABManager.Bundle.GDryya];

		private static AssetBundle _hegemolBundle => ABManager.AssetBundles[ABManager.Bundle.GHegemol];

		private static AssetBundle _zemerBundle => ABManager.AssetBundles[ABManager.Bundle.GZemer];

		private static AssetBundle _tisoBundle => ABManager.AssetBundles[ABManager.Bundle.TisoBund];

		public static IsmaController CreateIsma(bool onlyIsma)
		{
			Log("Creating Isma");
			GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Isma"]);
			gameObject.SetActive(value: true);
			gameObject.AddComponent<IsmaController>().onlyIsma = onlyIsma;
			Log("Done creating Isma");
			return gameObject.GetComponent<IsmaController>();
		}

		public static DryyaSetup CreateDryya()
		{
			Log("Creating Dryya");
			Vector2 vector = (OWArenaFinder.IsInOverWorld ? new Vector2(457.6f, 112.5f) : ((CustomWP.boss == CustomWP.Boss.All) ? new Vector2(91f, 25.5f) : new Vector2(90f, 25f)));
			GameObject dryya = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Dryya2"], vector, Quaternion.identity);
			GameManager.instance.StartCoroutine(DryyaIntro());
			dryya.AddComponent<DryyaSetup>();
			Log("Done creating Dryya");
			return dryya.GetComponent<DryyaSetup>();
			IEnumerator DryyaIntro()
			{
				BoxCollider2D bc = dryya.GetComponent<BoxCollider2D>();
				bc.enabled = false;
				while (dryya.transform.position.y > (OWArenaFinder.IsInOverWorld ? 103f : 20f))
				{
					yield return new WaitForFixedUpdate();
				}
				bc.enabled = true;
			}
		}

		public static HegemolController CreateHegemol()
		{
			Log("Creating Hegemol");
			GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Hegemol"], OWArenaFinder.IsInOverWorld ? new Vector2(438.4f, 28f) : new Vector2(87f, 28f), Quaternion.identity);
			gameObject.SetActive(!OWArenaFinder.IsInOverWorld);
			gameObject.AddComponent<HegemolController>();
			Log("Done creating Hegemol");
			return gameObject.GetComponent<HegemolController>();
		}

		public static ZemerController CreateZemer()
		{
			Log("Creating Zemer");
			GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Zemer"]);
			gameObject.SetActive(value: true);
			ZemerController result = gameObject.AddComponent<ZemerController>();
			Log("Done creating Zemer");
			return result;
		}

		public static TisoController CreateTiso()
		{
			Log("Creating Tiso");
			GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Tiso"]);
			gameObject.transform.position = HeroController.instance.transform.position;
			gameObject.SetActive(value: true);
			TisoController result = gameObject.AddComponent<TisoController>();
			Log("Done creating Tiso");
			return result;
		}

		public static void LoadIsmaSound()
		{
			if (!FiveKnights.Clips.ContainsKey("IsmaAudAgonyShoot"))
			{
				string[] array = new string[36]
				{
					"IsmaAudAgonyShoot", "IsmaAudAgonyIntro", "IsmaAudGroundWhip", "IsmaAudSeedBomb", "IsmaAudVineGrow", "IsmaAudVineHit", "IsmaAudWallGrow", "IsmaAudWallHit", "IsmaAudDungHit", "IsmaAudDungBreak",
					"IsmaAudAtt1", "IsmaAudAtt2", "IsmaAudAtt3", "IsmaAudAtt4", "IsmaAudAtt5", "IsmaAudAtt6", "IsmaAudAtt7", "IsmaAudAtt8", "IsmaAudAtt9", "IsmaAudAtt10",
					"IsmaAudBow", "IsmaAudDeath", "LoneIsmaIntro", "LoneIsmaLoop", "OgrimMusic", "OgrismaMusic", "IsmaAreaMusic", "IsmaAudTalk1", "IsmaAudTalk2", "IsmaAudTalk3",
					"IsmaAudTalk4", "IsmaAudTalk5", "IsmaAudTalk6", "IsmaAudTalkBye", "IsmaAudTalkHi", "IsmaAudTalkCharm"
				};
				foreach (string text in array)
				{
					FiveKnights.Clips[text] = _soundBundle.LoadAsset<AudioClip>(text);
				}
			}
		}

		public static void LoadIsmaBundle()
		{
			Log("Loading Isma bundle");
			if (FiveKnights.preloadedGO.TryGetValue("Isma", out var value) && value != null)
			{
				Log("Already loaded Isma");
				return;
			}
			LoadIsmaSound();
			AnimationClip[] array = _ismaBundle.LoadAllAssets<AnimationClip>();
			foreach (AnimationClip animationClip in array)
			{
				Log("Name of anim adding is " + animationClip.name);
				FiveKnights.AnimClips[animationClip.name] = animationClip;
			}
			string[] array2 = new string[7] { "Isma", "Gulka", "Plant", "Wall", "Fool", "ThornPlant", "Seal" };
			foreach (string text in array2)
			{
				FiveKnights.preloadedGO[text] = _ismaBundle.LoadAsset<GameObject>(text);
				SpriteRenderer[] componentsInChildren = FiveKnights.preloadedGO[text].GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].material = new Material(Shader.Find("Sprites/Default"));
				}
			}
			Collider2D[] componentsInChildren2 = FiveKnights.preloadedGO["Isma"].GetComponentsInChildren<Collider2D>(includeInactive: true);
			foreach (Collider2D obj in componentsInChildren2)
			{
				obj.gameObject.layer = 11;
				obj.gameObject.AddComponent<DamageHero>().damageDealt = 1;
			}
			FiveKnights.Materials["flash"] = _miscBundle.LoadAsset<Material>("UnlitFlashMat");
			FiveKnights.preloadedGO["Isma"].GetComponent<SpriteRenderer>().material = FiveKnights.Materials["flash"];
			foreach (Sprite item in from x in _miscBundle.LoadAllAssets<Sprite>()
				where x.name.Contains("Sil_Isma_")
				select x)
			{
				ArenaFinder.Sprites[item.name] = item;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Nosk"].LocateMyFSM("Mimic Spider").GetAction<HutongGames.PlayMaker.Actions.FlingObjectsFromGlobalPool>("Spit 1", 1).gameObject.Value);
			gameObject.SetActive(value: false);
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			ParticleSystem.MainModule main = gameObject.transform.Find("Steam").GetComponent<ParticleSystem>().main;
			ParticleSystem.MainModule main2 = gameObject.transform.Find("Air Steam").GetComponent<ParticleSystem>().main;
			main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.5019608f, 0.8862745f, 0.6627451f, 0.8509804f));
			main2.startColor = new ParticleSystem.MinMaxGradient(new Color(0.5019608f, 0.8862745f, 0.6627451f, 0.8509804f));
			AudioClip value2 = FiveKnights.preloadedGO["Nosk"].LocateMyFSM("Glob Audio").GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("SFX", 0).audioClip.Value as AudioClip;
			FiveKnights.Clips["AcidSpitSnd"] = value2;
			FiveKnights.preloadedGO["AcidSpit"] = gameObject;
			Log("Finished loading Isma bundle");
		}

		public static void LoadDryyaSound()
		{
			if (!FiveKnights.Clips.ContainsKey("DryyaMusic"))
			{
				FiveKnights.Clips["DryyaMusic"] = _soundBundle.LoadAsset<AudioClip>("DryyaMusic");
				FiveKnights.Clips["DryyaAreaMusic"] = _soundBundle.LoadAsset<AudioClip>("DryyaAreaMusic");
				string[] array = new string[21]
				{
					"1", "2", "3", "4", "5", "6", "7", "Alt1", "Alt2", "Alt3",
					"Alt4", "Alt5", "Alt6", "Beams1", "Beams2", "Beams3", "Bow", "Death", "Convo2", "Convo1",
					"Convo3"
				};
				foreach (string text in array)
				{
					FiveKnights.Clips["DryyaVoice" + text] = _soundBundle.LoadAsset<AudioClip>("DryyaVoice" + text);
				}
			}
		}

		public static void LoadDryyaBundle()
		{
			Log("Loading Dryya bundle");
			if (FiveKnights.preloadedGO.TryGetValue("Dryya2", out var value) && value != null)
			{
				Log("Already loaded Dryya");
				return;
			}
			LoadDryyaSound();
			AnimationClip[] array = _dryyaBundle.LoadAllAssets<AnimationClip>();
			foreach (AnimationClip animationClip in array)
			{
				Log("Name of anim adding is " + animationClip.name);
				FiveKnights.AnimClips[animationClip.name] = animationClip;
			}
			FiveKnights.preloadedGO["Dryya2"] = _dryyaBundle.LoadAsset<GameObject>("Dryya2");
			FiveKnights.preloadedGO["Beams"] = _dryyaBundle.LoadAsset<GameObject>("Beams");
			FiveKnights.preloadedGO["Dagger"] = _dryyaBundle.LoadAsset<GameObject>("Dagger");
			FiveKnights.preloadedGO["Beams"].GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
			FiveKnights.preloadedGO["Dagger"].GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
			foreach (Sprite item in from x in _miscBundle.LoadAllAssets<Sprite>()
				where x.name.Contains("Dryya_Silhouette_")
				select x)
			{
				ArenaFinder.Sprites[item.name] = item;
			}
			Log("Finished loading Dryya bundle");
		}

		public static void LoadHegemolSound()
		{
			if (!FiveKnights.Clips.ContainsKey("HegArrive"))
			{
				string[] array = new string[30]
				{
					"HegArrive", "HegAttackSwing", "HegAttackHit", "HegAttackCharge", "HegDamage", "HegDamageFinal", "HegDebris", "HegDungDebris", "HegJump", "HegLand",
					"HegShockwave", "HNeutral1", "HNeutral2", "HNeutral3", "HCharge", "HHeavy1", "HHeavy2", "HDeath", "HGrunt1", "HGrunt2",
					"HGrunt3", "HGrunt4", "HTired1", "HTired2", "HTired3", "HegemolMusicIntro", "HegemolMusicLoop", "HegAreaMusic", "HegAreaMusicIntro", "HegAreaMusicBG"
				};
				foreach (string text in array)
				{
					FiveKnights.Clips[text] = _soundBundle.LoadAsset<AudioClip>(text);
				}
			}
		}

		public static void LoadHegemolBundle()
		{
			Log("Loading Hegemol bundle");
			if (FiveKnights.preloadedGO.TryGetValue("Hegemol", out var value) && value != null)
			{
				Log("Already loaded Hegemol");
				return;
			}
			LoadHegemolSound();
			FiveKnights.preloadedGO["Hegemol"] = _hegemolBundle.LoadAsset<GameObject>("Hegemol");
			FiveKnights.preloadedGO["Mace"] = _hegemolBundle.LoadAsset<GameObject>("Mace");
			FiveKnights.preloadedGO["Debris"] = _hegemolBundle.LoadAsset<GameObject>("Debris");
			FiveKnights.preloadedGO["Mace"].GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
			PlayMakerFSM fsm = FiveKnights.preloadedGO["Traitor"].LocateMyFSM("Mantis");
			FiveKnights.preloadedGO["TraitorSlam"] = fsm.GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>("Waves", 0).gameObject.Value;
			FiveKnights.Clips["TraitorSlam"] = fsm.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Waves", 4).audioClip.Value as AudioClip;
			GameObject value2 = FiveKnights.preloadedGO["WhiteDef"].LocateMyFSM("Dung Defender").GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>("Throw 1", 1).gameObject.Value;
			FiveKnights.preloadedGO["DungBreakChunks"] = value2.LocateMyFSM("Ball Control").FsmVariables.FindFsmGameObject("Break Chunks").Value;
			foreach (Sprite item in from x in _miscBundle.LoadAllAssets<Sprite>()
				where x.name.Contains("hegemol_silhouette_")
				select x)
			{
				ArenaFinder.Sprites[item.name] = item;
			}
			FiveKnights.Materials["flash"] = _miscBundle.LoadAsset<Material>("UnlitFlashMat");
			Log("Finished loading Hegemol bundle");
		}

		public static void LoadZemerSound()
		{
			if (!FiveKnights.Clips.ContainsKey("ZP2Intro"))
			{
				string[] array = new string[47]
				{
					"ZP2Intro", "ZP1Loop", "ZAudAtt4", "ZAudP1DeathA", "ZAudP1DeathB", "ZAudP1DeathC", "ZAudP1DeathD", "ZAudP1Death2", "ZAudBow", "ZAudCounter",
					"ZAudAtt5", "ZP1Intro", "ZAudAtt2", "ZP2Loop", "ZAudLaser", "ZAudHoriz", "ZAudAtt3", "ZAudAtt6", "AudBasicSlash1", "AudBigSlash",
					"AudBigSlash2", "AudLand", "AudDashIntro", "AudDash", "AudBasicSlash2", "breakable_wall_hit_1", "breakable_wall_hit_2", "Zem_Area", "ZAudAtt7", "ZAudAtt8",
					"ZAudAtt9", "ZAudAtt10", "ZAudAtt11", "ZAudAtt12", "ZAudAtt13", "ZAudAtt14", "ZAudAtt15", "ZAudAtt16", "ZAudMid", "ZAudTalk1",
					"ZAudTalk1B", "ZAudTalk2", "ZAudTalk3", "ZAudTalk4", "ZAudExplode", "Zem_Backdash", "ZemBladeShine"
				};
				foreach (string text in array)
				{
					FiveKnights.Clips[text] = _soundBundle.LoadAsset<AudioClip>(text);
				}
			}
		}

		public static void LoadZemerBundle()
		{
			Log("Loading Zemer bundle");
			if (FiveKnights.preloadedGO.TryGetValue("Zemer", out var value) && value != null)
			{
				Log("Already loaded Zemer");
				return;
			}
			LoadZemerSound();
			FiveKnights.Clips["NeedleSphere"] = (AudioClip)FiveKnights.preloadedGO["HornetSphere"].LocateMyFSM("Control").GetAction<HutongGames.PlayMaker.Actions.AudioPlaySimple>("Sphere A", 0).oneShotClip.Value;
			AnimationClip[] array = _zemerBundle.LoadAllAssets<AnimationClip>();
			foreach (AnimationClip animationClip in array)
			{
				Log("Name of anim adding is " + animationClip.name);
				FiveKnights.AnimClips[animationClip.name] = animationClip;
			}
			GameObject[] array2 = _zemerBundle.LoadAllAssets<GameObject>();
			SpriteRenderer[] componentsInChildren;
			foreach (GameObject gameObject in array2)
			{
				if (gameObject.name == "Zemer")
				{
					FiveKnights.preloadedGO["Zemer"] = gameObject;
				}
				else if (gameObject.name == "TChild")
				{
					FiveKnights.preloadedGO["TChild"] = gameObject;
				}
				else if (gameObject.name == "NewSlash")
				{
					FiveKnights.preloadedGO["SlashBeam"] = gameObject;
				}
				else if (gameObject.name == "NewSlash2")
				{
					FiveKnights.preloadedGO["SlashBeam2"] = gameObject;
				}
				else if (gameObject.name == "SlashRingController")
				{
					FiveKnights.preloadedGO["SlashRingController"] = gameObject;
				}
				else if (gameObject.name == "SlashRingControllerNew")
				{
					FiveKnights.preloadedGO["SlashRingControllerNew"] = gameObject;
				}
				else if (gameObject.name == "AllFlowers")
				{
					FiveKnights.preloadedGO["AllFlowers"] = gameObject;
				}
				if (gameObject.GetComponent<SpriteRenderer>() == null)
				{
					componentsInChildren = gameObject.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						componentsInChildren[j].material = new Material(Shader.Find("Sprites/Default"));
					}
				}
				else
				{
					gameObject.GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
				}
			}
			GameObject gameObject2 = FiveKnights.preloadedGO["Zemer"];
			foreach (Transform item in FiveKnights.preloadedGO["SlashBeam"].transform)
			{
				item.gameObject.AddComponent<DamageHero>().damageDealt = 1;
				item.gameObject.layer = 22;
			}
			foreach (Transform item2 in FiveKnights.preloadedGO["TChild"].transform)
			{
				item2.gameObject.AddComponent<DamageHero>().damageDealt = 1;
				item2.gameObject.layer = 22;
			}
			foreach (Transform item3 in FiveKnights.preloadedGO["SlashBeam2"].transform)
			{
				item3.GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
				item3.Find("HB1").gameObject.AddComponent<DamageHero>().damageDealt = 1;
				item3.Find("HB2").gameObject.AddComponent<DamageHero>().damageDealt = 1;
				item3.Find("HB1").gameObject.layer = 22;
				item3.Find("HB2").gameObject.layer = 22;
			}
			PolygonCollider2D[] componentsInChildren2;
			foreach (Transform item4 in FiveKnights.preloadedGO["SlashRingControllerNew"].transform)
			{
				foreach (Transform item5 in item4)
				{
					componentsInChildren2 = item5.GetComponentsInChildren<PolygonCollider2D>(includeInactive: true);
					foreach (PolygonCollider2D obj4 in componentsInChildren2)
					{
						obj4.gameObject.AddComponent<DamageHero>().damageDealt = 1;
						obj4.gameObject.layer = 22;
						obj4.gameObject.AddComponent<ParryTink>();
					}
				}
			}
			foreach (Transform item6 in FiveKnights.preloadedGO["SlashRingController"].transform)
			{
				foreach (Transform item7 in item6)
				{
					componentsInChildren2 = item7.GetComponentsInChildren<PolygonCollider2D>(includeInactive: true);
					foreach (PolygonCollider2D obj5 in componentsInChildren2)
					{
						obj5.gameObject.AddComponent<DamageHero>().damageDealt = 1;
						obj5.gameObject.layer = 22;
						obj5.gameObject.AddComponent<ParryTink>();
					}
				}
			}
			componentsInChildren = gameObject2.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
			foreach (SpriteRenderer spriteRenderer in componentsInChildren)
			{
				spriteRenderer.material = new Material(Shader.Find("Sprites/Default"));
				BoxCollider2D component = spriteRenderer.gameObject.GetComponent<BoxCollider2D>();
				if (!(component == null))
				{
					component.isTrigger = true;
					component.gameObject.AddComponent<DamageHero>().damageDealt = 1;
					spriteRenderer.gameObject.AddComponent<Pogoable>().tar = gameObject2;
					component.gameObject.layer = 22;
					if (!spriteRenderer.name.Contains("Zemer"))
					{
						spriteRenderer.gameObject.AddComponent<ParryTink>();
					}
				}
			}
			componentsInChildren2 = gameObject2.GetComponentsInChildren<PolygonCollider2D>(includeInactive: true);
			foreach (PolygonCollider2D obj6 in componentsInChildren2)
			{
				obj6.isTrigger = true;
				obj6.gameObject.AddComponent<DamageHero>().damageDealt = 1;
				obj6.gameObject.AddComponent<ParryTink>();
				obj6.gameObject.AddComponent<Pogoable>().tar = gameObject2;
				obj6.gameObject.layer = 22;
			}
			GameObject gameObject3 = gameObject2.transform.Find("MultiDashParryHB").gameObject;
			gameObject3.AddComponent<ParryTink>();
			gameObject3.AddComponent<Pogoable>().tar = gameObject2;
			gameObject3.layer = 22;
			gameObject3.SetActive(value: false);
			FiveKnights.preloadedGO["SlashBeam"].GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
			FiveKnights.Materials["flash"] = _miscBundle.LoadAsset<Material>("UnlitFlashMat");
			gameObject2.GetComponent<SpriteRenderer>().material = FiveKnights.Materials["flash"];
			PlayMakerFSM fsm = FiveKnights.preloadedGO["Traitor"].LocateMyFSM("Mantis");
			FiveKnights.preloadedGO["TraitorSlam"] = fsm.GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>("Waves", 0).gameObject.Value;
			FiveKnights.Clips["TraitorSlam"] = fsm.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Waves", 4).audioClip.Value as AudioClip;
			ArenaFinder.Sprites["ZemParticPetal"] = _miscBundle.LoadAsset<Sprite>("petal-test");
			ArenaFinder.Sprites["ZemParticDung"] = _miscBundle.LoadAsset<Sprite>("dung-test");
			foreach (Sprite item8 in from x in _miscBundle.LoadAllAssets<Sprite>()
				where x.name.Contains("Zem_Sil_")
				select x)
			{
				ArenaFinder.Sprites[item8.name] = item8;
			}
			Log("Finished loading Zemer bundle");
		}

		public static void LoadTisoBundle()
		{
			Log("Loading Tiso bundle");
			if (FiveKnights.preloadedGO.TryGetValue("Tiso", out var value) && value != null)
			{
				Log("Already loaded Tiso");
				return;
			}
			TisoAudio.TisoAud = new Dictionary<string, AudioClip>();
			string[] array = new string[11]
			{
				"AudSpikeHitWall", "AudTisoJump", "AudTisoLand", "AudTisoShoot", "AudTisoSpin", "AudTisoThrowShield", "AudTisoWalk", "AudTisoDeath", "AudTisoRoar", "AudTisoYell",
				"AudLand"
			};
			foreach (string text in array)
			{
				TisoAudio.TisoAud[text] = _soundBundle.LoadAsset<AudioClip>(text);
			}
			for (int j = 1; j < 7; j++)
			{
				TisoAudio.TisoAud[$"AudTiso{j}"] = _soundBundle.LoadAsset<AudioClip>($"AudTiso{j}");
			}
			FiveKnights.Clips["TisoMusicStart"] = _soundBundle.LoadAsset<AudioClip>("TisoMusicStart");
			FiveKnights.Clips["TisoMusicLoop"] = _soundBundle.LoadAsset<AudioClip>("TisoMusicLoop");
			FiveKnights.preloadedGO["Tiso"] = _tisoBundle.LoadAsset<GameObject>("Tiso");
			FiveKnights.Materials["flash"] = _miscBundle.LoadAsset<Material>("UnlitFlashMat");
			Log("Finished loading Tiso bundle");
		}

		private static void Log(object o)
		{
			Modding.Logger.Log("[FiveKnights][BossLoader] " + o);
		}
	}
	public class CustomWP : MonoBehaviour
	{
		public enum Boss
		{
			None,
			Ogrim,
			Dryya,
			Isma,
			Hegemol,
			All,
			Mystic,
			Ze
		}

		public static bool isInGodhome;

		public static Boss boss;

		public static Boss prevBoss;

		public static CustomWP Instance;

		public static bool wonLastFight;

		public static int lev;

		private Dictionary<string, StatueControl> StatueControls = new Dictionary<string, StatueControl>();

		private void Start()
		{
			if (!isInGodhome)
			{
				return;
			}
			Instance = this;
			On.GameManager.EnterHero += GameManager_EnterHero;
			On.BossChallengeUI.LoadBoss_int_bool += BossChallengeUI_LoadBoss_int_bool;
			prevBoss = boss;
			boss = Boss.None;
			FiveKnights.preloadedGO["HubRoot"] = ABManager.AssetBundles[ABManager.Bundle.GArenaHub].LoadAsset<GameObject>("pale court gg throne aditions");
			GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["HubRoot"]);
			FiveKnights.preloadedGO["ThroneCovered"] = gameObject.transform.Find("throne_covered").gameObject;
			FiveKnights.preloadedGO["ThroneCovered"].SetActive(!FiveKnights.Instance.SaveSettings.UnlockedChampionsCall || !FiveKnights.Instance.SaveSettings.SeenChampionsCall);
			gameObject.SetActive(value: true);
			SpriteRenderer[] componentsInChildren = gameObject.transform.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].material = new Material(Shader.Find("Sprites/Default"));
			}
			foreach (GameObject item in from x in UnityEngine.Object.FindObjectsOfType<GameObject>()
				where x.name.Contains("wp_rib_back")
				select x)
			{
				if (item.name != "wp_rib_back(3)" && item.name != "wp_rib_back(4)")
				{
					UnityEngine.Object.Destroy(item);
				}
			}
			GameObject gameObject2 = GameObject.Find("core_extras_0024_wp(14)");
			if (gameObject2 != null)
			{
				Log("Found del, deleting");
				UnityEngine.Object.Destroy(gameObject2);
			}
			GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
			foreach (GameObject gameObject3 in rootGameObjects)
			{
				string text = gameObject3.name;
				if (!(text == "core_extras_0025_wp (6)"))
				{
					if (text == "core_extras_0025_wp")
					{
						gameObject3.transform.SetPositionZ(3.34f);
					}
				}
				else
				{
					gameObject3.transform.SetPositionZ(2.2f);
				}
			}
			foreach (GameObject item2 in from x in UnityEngine.Object.FindObjectsOfType<GameObject>()
				where x.name.Contains("new_cloud") && x.transform.position.x <= 25f
				select x)
			{
				UnityEngine.Object.Destroy(item2);
			}
			Material[] array = new Material[1]
			{
				new Material(Shader.Find("UI/Blur/UIBlur"))
			};
			array[0].SetColor(Shader.PropertyToID("_TintColor"), new Color(1f, 1f, 1f, 0f));
			array[0].SetFloat(Shader.PropertyToID("_Size"), 53.7f);
			array[0].SetFloat(Shader.PropertyToID("_Vibrancy"), 0.2f);
			array[0].SetInt(Shader.PropertyToID("_StencilComp"), 8);
			array[0].SetInt(Shader.PropertyToID("_Stencil"), 0);
			array[0].SetInt(Shader.PropertyToID("_StencilOp"), 0);
			array[0].SetInt(Shader.PropertyToID("_StencilWriteMask"), 255);
			array[0].SetInt(Shader.PropertyToID("_StencilReadMask"), 255);
			Log("Look for blur!");
			foreach (GameObject item3 in from x in UnityEngine.Object.FindObjectsOfType<GameObject>()
				where x.name == "BlurPlane"
				select x)
			{
				Log("Found blur!");
				item3.SetActive(value: false);
				item3.GetComponent<MeshRenderer>().materials = array;
				item3.SetActive(value: true);
			}
			GameObject gameObject4 = GameObject.Find("CameraLockArea (2)");
			if (gameObject4 != null)
			{
				BoxCollider2D component = gameObject4.GetComponent<BoxCollider2D>();
				component.size = new Vector2(50f, component.size.y);
				component.offset = new Vector2(-10f, component.offset.y);
				Log("Fixed WP_09 camera at edges");
			}
			On.CameraController.UpdateTargetDestinationDelta += CameraControllerUpdateTarget;
		}

		private void CameraControllerUpdateTarget(On.CameraController.orig_UpdateTargetDestinationDelta orig, CameraController self)
		{
			self.lookOffset = 0f;
			orig(self);
		}

		private void GameManager_EnterHero(On.GameManager.orig_EnterHero orig, GameManager self, bool additiveGateSearch)
		{
			if (self.sceneName == "White_Palace_09")
			{
				foreach (GameObject item in from x in UnityEngine.Object.FindObjectsOfType<GameObject>()
					where x.name == "GG_extra_walls_0000_2_2_crack"
					select x)
				{
					UnityEngine.Object.Destroy(item);
				}
				GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["hubfloor"]);
				GameObject gameObject = GameObject.Find("Chunk 2 0");
				obj.transform.Find("Chunk 2 0").GetComponent<MeshRenderer>().material = gameObject.GetComponent<MeshRenderer>().material;
				GameObject obj2 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["StartDoor"]);
				obj2.SetActive(value: true);
				obj2.transform.position = new Vector3(13.8f, 95.93f, 4.21f);
				obj2.transform.localScale = new Vector3(1.33f, 1.02f, 0.87f);
				UnityEngine.Object.Destroy(obj2.transform.Find("GG_secret_door").GetComponent<AudioSource>());
				GameObject gameObject2 = obj2.transform.Find("GG_secret_door").gameObject;
				TransitionPoint component = gameObject2.transform.Find("door_Land_of_Storms").GetComponent<TransitionPoint>();
				component.targetScene = "GG_Workshop";
				component.entryPoint = "door_Land_of_Storms_return";
				obj2.transform.Find("door_Land_of_Storms_return").gameObject.SetActive(value: true);
				gameObject2.transform.Find("door_Land_of_Storms").gameObject.LocateMyFSM("Door Control").FsmVariables.FindFsmString("New Scene").Value = "GG_Workshop";
				gameObject2.transform.Find("door_Land_of_Storms").gameObject.LocateMyFSM("Door Control").FsmVariables.FindFsmString("Entry Gate").Value = "door_Land_of_Storms_return";
				gameObject2.LocateMyFSM("Deactivate").enabled = false;
				gameObject2.SetActive(value: true);
				CreateStatues();
				HubRemove();
				AddLift();
				CreateGateway("door_dreamReturnGGTestingIt", new Vector2(60.5f, 98.4f), Vector2.zero, null, null, right: false, left: true, onlyOut: true, GameManager.SceneLoadVisualizations.Default);
				orig(self, additiveGateSearch: false);
				SetupThrone();
				Log("MADE CUSTOM WP");
			}
			else
			{
				orig(self, additiveGateSearch: false);
			}
		}

		private void SetupThrone()
		{
			GameObject throne = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["throne"]);
			throne.transform.position = new Vector3(60.5f, 97.7f, 0.2f);
			PlayMakerFSM fsm = throne.LocateMyFSM("Sit");
			GameObject gameObject = new GameObject("Throne Flash Effect");
			gameObject.transform.position = new Vector3(60.5f, 102.62f, -2f);
			gameObject.layer = 13;
			GameObject obj = UnityEngine.Object.Instantiate(USceneUtil.Find(USceneUtil.Find(FiveKnights.preloadedGO["Statue"], "Base"), "GG_Statue_First_Appear"), gameObject.transform);
			obj.name = "Throne First Appear";
			obj.transform.position = new Vector3(60.5f, 102.62f, -2f);
			UnityEngine.Object.Destroy(USceneUtil.Find(obj, "GG_statues_0027_26"));
			UnityEngine.Object.Destroy(obj.GetComponent<BossStatueFlashEffect>());
			gameObject.AddComponent<ThroneFlash>().throne = throne;
			gameObject.SetActive(value: true);
			StartCoroutine(Throne());
			GameObject.Find("throne").transform.Find("core_extras_0029_wp").gameObject.SetActive(FiveKnights.Instance.SaveSettings.UnlockedChampionsCall && FiveKnights.Instance.SaveSettings.SeenChampionsCall);
			throne.SetActive(FiveKnights.Instance.SaveSettings.UnlockedChampionsCall && FiveKnights.Instance.SaveSettings.SeenChampionsCall);
			FiveKnights.preloadedGO["ThroneCovered"].SetActive(!FiveKnights.Instance.SaveSettings.UnlockedChampionsCall || !FiveKnights.Instance.SaveSettings.SeenChampionsCall);
			IEnumerator SaidNo()
			{
				yield return null;
				PlayerData.instance.disablePause = false;
				PlayMakerFSM playMakerFSM = GameObject.Find("Text YN").LocateMyFSM("Dialogue Page Control");
				GameObject.Find("DialogueManager").LocateMyFSM("Box Open YN").SendEvent("BOX DOWN YN");
				fsm.enabled = true;
				playMakerFSM.enabled = true;
				fsm.SetState("Get Up");
				playMakerFSM.RemoveAction("No", 1);
				playMakerFSM.RemoveAction("Yes", 1);
			}
			IEnumerator SaidYes()
			{
				PlayerData.instance.disablePause = false;
				PlayMakerFSM textYN = GameObject.Find("Text YN").LocateMyFSM("Dialogue Page Control");
				GameObject.Find("DialogueManager").LocateMyFSM("Box Open YN").SendEvent("BOX DOWN YN");
				GameCameras.instance.tk2dCam.gameObject.LocateMyFSM("CameraFade").SendEvent("FADE OUT");
				yield return new WaitForSeconds(0.5f);
				boss = Boss.All;
				ArenaFinder.defeats = PlayerData.instance.whiteDefenderDefeats;
				PlayerData.instance.whiteDefenderDefeats = 0;
				PlayerData.instance.respawnMarkerName = throne.name;
				GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
				{
					SceneName = "Dream_04_White_Defender",
					EntryGateName = "door1",
					Visualization = GameManager.SceneLoadVisualizations.Dream,
					WaitForSceneTransitionCameraFade = false,
					EntryDelay = 0f
				});
				textYN.RemoveAction("Yes", 1);
				textYN.RemoveAction("No", 1);
				textYN.enabled = true;
				fsm.enabled = true;
			}
			IEnumerator Throne()
			{
				while ((bool)base.gameObject)
				{
					yield return new WaitWhile(() => fsm.ActiveStateName != "Resting");
					fsm.enabled = false;
					PlayerData.instance.disablePause = true;
					GameObject.Find("DialogueManager").LocateMyFSM("Box Open YN").SendEvent("BOX UP YN");
					GameObject.Find("DialogueManager").SetActive(value: true);
					GameObject.Find("Text YN").SetActive(value: true);
					GameObject.Find("Text YN").GetComponent<DialogueBox>().StartConversation("YN_THRONE", "Speech");
					PlayMakerFSM textYN = GameObject.Find("Text YN").LocateMyFSM("Dialogue Page Control");
					textYN.FsmVariables.FindFsmInt("Toll Cost").Value = 0;
					textYN.InsertCoroutine("Yes", 1, SaidYes);
					textYN.InsertCoroutine("No", 1, SaidNo);
					textYN.enabled = true;
					while (textYN.ActiveStateName != "Ready for Input")
					{
						yield return new WaitForEndOfFrame();
					}
					while (textYN.ActiveStateName == "Ready for Input")
					{
						yield return new WaitForEndOfFrame();
					}
					yield return new WaitForSeconds(0.1f);
				}
			}
		}

		private void HubRemove()
		{
			foreach (SpriteRenderer item in from x in UnityEngine.Object.FindObjectsOfType<SpriteRenderer>()
				where x != null && x.name.Contains("SceneBorder")
				select x)
			{
				UnityEngine.Object.Destroy(item);
			}
			string[] array = new string[17]
			{
				"Breakable Wall Waterways", "black_fader", "White_Palace_throne_room_top_0000_2", "White_Palace_throne_room_top_0001_1", "Glow Response floor_ring large2 (1)", "core_extras_0006_wp", "msk_station", "core_extras_0028_wp (12)", "wp_additions_01", "Inspect Region (1)",
				"core_extras_0021_wp (4)", "core_extras_0021_wp (5)", "core_extras_0021_wp (1)", "core_extras_0021_wp (6)", "core_extras_0021_wp (7)", "core_extras_0021_wp (2)", "Darkness Region"
			};
			foreach (GameObject item2 in from x in UnityEngine.Object.FindObjectsOfType<GameObject>()
				where x.activeSelf
				select x)
			{
				string[] array2 = array;
				foreach (string value in array2)
				{
					if (item2.name.Contains(value))
					{
						UnityEngine.Object.Destroy(item2);
					}
				}
			}
			foreach (GameObject item3 in from x in UnityEngine.Object.FindObjectsOfType<GameObject>()
				where x.name.Contains("abyss") || x.name.Contains("Abyss")
				select x)
			{
				UnityEngine.Object.Destroy(item3);
			}
		}

		private void AddLift()
		{
			StartCoroutine(FixArena());
			static IEnumerator FixArena()
			{
				yield return null;
				string[] removes = new string[4] { "white_palace_wall_set_01 (10)", "white_palace_wall_set_01 (18)", "_0028_white (4)", "_0028_white (3)" };
				foreach (GameObject item in from x in UnityEngine.Object.FindObjectsOfType<GameObject>()
					where removes.Contains(x.name)
					select x)
				{
					UnityEngine.Object.Destroy(item);
				}
				yield return null;
			}
		}

		private void BossChallengeUI_LoadBoss_int_bool(On.BossChallengeUI.orig_LoadBoss_int_bool orig, BossChallengeUI self, int level, bool doHideAnim)
		{
			UnityEngine.Object.DontDestroyOnLoad(GameManager.instance);
			object value = ReflectionHelper.GetFieldInfo(typeof(BossChallengeUI), "bossStatue").GetValue(self);
			Log(value);
			string[] array = value.ToString().Split(' ')[0].Split('_');
			Log(array[2]);
			boss = array[2] switch
			{
				"Dryya" => Boss.Dryya, 
				"Zemer" => FiveKnights.Instance.SaveSettings.AltStatueZemer ? Boss.Mystic : Boss.Ze, 
				"Isma" => FiveKnights.Instance.SaveSettings.AltStatueIsma ? Boss.Ogrim : Boss.Isma, 
				"Hegemol" => Boss.Hegemol, 
			};
			lev = level;
			orig(self, level, doHideAnim);
		}

		private void CreateGateway(string gateName, Vector2 pos, Vector2 size, string toScene, string entryGate, bool right, bool left, bool onlyOut, GameManager.SceneLoadVisualizations vis)
		{
			GameObject gameObject = new GameObject(gateName);
			gameObject.transform.SetPosition2D(pos);
			TransitionPoint transitionPoint = gameObject.AddComponent<TransitionPoint>();
			if (!onlyOut)
			{
				BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
				boxCollider2D.size = size;
				boxCollider2D.isTrigger = true;
				transitionPoint.targetScene = toScene;
				transitionPoint.entryPoint = entryGate;
			}
			transitionPoint.alwaysEnterLeft = left;
			transitionPoint.alwaysEnterRight = right;
			GameObject gameObject2 = new GameObject("Hazard Respawn Marker");
			gameObject2.transform.parent = transitionPoint.transform;
			gameObject2.transform.position = new Vector2(gameObject2.transform.position.x - 3f, gameObject2.transform.position.y);
			gameObject2.AddComponent<HazardRespawnMarker>();
			transitionPoint.respawnMarker = gameObject2.GetComponent<HazardRespawnMarker>();
			transitionPoint.sceneLoadVisualization = vis;
		}

		private void CreateStatues()
		{
			On.BossStatueLever.OnTriggerEnter2D -= BossStatueLever_OnTriggerEnter2D;
			On.BossStatueLever.OnTriggerEnter2D += BossStatueLever_OnTriggerEnter2D;
			On.BossStatueFlashEffect.FlashApex -= BossStatueFlashEffect_FlashApex;
			On.BossStatueFlashEffect.FlashApex += BossStatueFlashEffect_FlashApex;
			On.HutongGames.PlayMaker.Actions.CallMethodProper.OnEnter -= CallMethodProperOnEnter;
			On.HutongGames.PlayMaker.Actions.CallMethodProper.OnEnter += CallMethodProperOnEnter;
			GameManager.instance.OnFinishedEnteringScene -= GMOnFinishedEnteringScene;
			if (wonLastFight)
			{
				GameManager.instance.OnFinishedEnteringScene += GMOnFinishedEnteringScene;
			}
			else
			{
				lev = 0;
			}
			wonLastFight = false;
			SetStatue(new Vector2(81.75f, 94.75f), new Vector2(0.5f, 0.1f), new Vector2(0f, -0.5f), FiveKnights.preloadedGO["Statue"], "gg isma", FiveKnights.SPRITES[FiveKnights.Instance.SaveSettings.CompletionIsma.completedTier3 ? "Isma_Silver" : "Isma"], "ISMA_NAME", "ISMA_DESC", "statueStateIsma");
			SetStatue(new Vector2(39.4f, 94.75f), new Vector2(-0.25f, -0.75f), new Vector2(0f, -1f), FiveKnights.preloadedGO["StatueMed"], "gg dryya", FiveKnights.SPRITES[FiveKnights.Instance.SaveSettings.CompletionDryya.completedTier3 ? "Dryya_Silver" : "Dryya"], "DRYYA_NAME", "DRYYA_DESC", "statueStateDryya");
			SetStatue(new Vector2(73.3f, 98.75f), new Vector2(-0.13f, 1.3f), new Vector2(0f, -1.7f), FiveKnights.preloadedGO["StatueMed"], "gg zemer", FiveKnights.SPRITES[FiveKnights.Instance.SaveSettings.CompletionZemer.completedTier3 ? "Zemer_Silver" : "Zemer"], "ZEM_NAME", "ZEM_DESC", "statueStateZemer");
			SetStatue(new Vector2(48f, 98.75f), new Vector2(-0.2f, 0.1f), new Vector2(0f, -0.8f), FiveKnights.preloadedGO["StatueMed"], "gg hegemol", FiveKnights.SPRITES[FiveKnights.Instance.SaveSettings.CompletionHegemol.completedTier3 ? "Hegemol_Silver" : "Hegemol"], "HEG_NAME", "HEG_DESC", "statueStateHegemol");
		}

		private void BossStatueLever_OnTriggerEnter2D(On.BossStatueLever.orig_OnTriggerEnter2D orig, BossStatueLever self, Collider2D collision)
		{
			if (!(collision.tag != "Nail Attack"))
			{
				string statueStatePD = self.gameObject.transform.parent.parent.GetComponent<BossStatue>().statueStatePD;
				if (statueStatePD.Contains("Isma"))
				{
					StatueControls["Isma"].StartLever(self);
				}
				else if (statueStatePD.Contains("Zemer"))
				{
					StatueControls["Zemer"].StartLever(self);
				}
				else
				{
					orig(self, collision);
				}
			}
		}

		private void BossStatueFlashEffect_FlashApex(On.BossStatueFlashEffect.orig_FlashApex orig, BossStatueFlashEffect self)
		{
			BossStatue field = Mirror.GetField<BossStatueFlashEffect, BossStatue>(self, "parentStatue");
			switch (field.bossScene.sceneName)
			{
			case "gg isma":
				USceneUtil.Find(field.gameObject, "FakeStatIsma").SetActive(value: true);
				break;
			case "gg dryya":
				USceneUtil.Find(field.gameObject, "FakeStatDryya").SetActive(value: true);
				break;
			case "gg zemer":
				USceneUtil.Find(field.gameObject, "FakeStatZemer").SetActive(value: true);
				break;
			case "gg hegemol":
				USceneUtil.Find(field.gameObject, "FakeStatHegemol").SetActive(value: true);
				break;
			}
			orig(self);
		}

		private void CallMethodProperOnEnter(On.HutongGames.PlayMaker.Actions.CallMethodProper.orig_OnEnter orig, HutongGames.PlayMaker.Actions.CallMethodProper self)
		{
			if (self.Fsm.Name == "inspect_region" && self.Fsm.GameObject.name == "Inspect_Locked" && self.State.Name == "Read")
			{
				switch (self.Fsm.GameObject.transform.parent.gameObject.GetComponent<BossStatue>().bossScene.sceneName)
				{
				case "gg isma":
					self.Fsm.Variables.FindFsmString("Game Text Convo").Value = "ISMA_LOCKED_DESC";
					break;
				case "gg dryya":
					self.Fsm.Variables.FindFsmString("Game Text Convo").Value = "DRYYA_LOCKED_DESC";
					break;
				case "gg zemer":
					self.Fsm.Variables.FindFsmString("Game Text Convo").Value = "ZEM_LOCKED_DESC";
					break;
				case "gg hegemol":
					self.Fsm.Variables.FindFsmString("Game Text Convo").Value = "HEG_LOCKED_DESC";
					break;
				}
			}
			orig(self);
		}

		private void GMOnFinishedEnteringScene()
		{
			DoWinEffect();
		}

		private void DoWinEffect()
		{
			GameObject gameObject = null;
			switch (prevBoss)
			{
			case Boss.Isma:
				gameObject = ((!FiveKnights.Instance.SaveSettings.CompletionIsma2.isUnlocked) ? USceneUtil.Find(USceneUtil.Find(USceneUtil.Find(GameObject.Find("GG_Statue_Isma"), "Base"), "Plaque"), "Plaque_Trophy_Centre") : USceneUtil.Find(USceneUtil.Find(USceneUtil.Find(GameObject.Find("GG_Statue_Isma"), "Base"), "Plaque"), "Plaque_Trophy_Left"));
				break;
			case Boss.Ogrim:
				gameObject = USceneUtil.Find(USceneUtil.Find(USceneUtil.Find(GameObject.Find("GG_Statue_Isma"), "Base"), "Plaque"), "Plaque_Trophy_Right");
				break;
			case Boss.Dryya:
				gameObject = USceneUtil.Find(USceneUtil.Find(USceneUtil.Find(GameObject.Find("GG_Statue_Dryya"), "Base"), "Plaque"), "Plaque_Trophy_Centre");
				break;
			case Boss.Ze:
				gameObject = ((!FiveKnights.Instance.SaveSettings.CompletionZemer2.isUnlocked) ? USceneUtil.Find(USceneUtil.Find(USceneUtil.Find(GameObject.Find("GG_Statue_Zemer"), "Base"), "Plaque"), "Plaque_Trophy_Centre") : USceneUtil.Find(USceneUtil.Find(USceneUtil.Find(GameObject.Find("GG_Statue_Zemer"), "Base"), "Plaque"), "Plaque_Trophy_Left"));
				break;
			case Boss.Mystic:
				gameObject = USceneUtil.Find(USceneUtil.Find(USceneUtil.Find(GameObject.Find("GG_Statue_Zemer"), "Base"), "Plaque"), "Plaque_Trophy_Right");
				break;
			case Boss.Hegemol:
				gameObject = USceneUtil.Find(USceneUtil.Find(USceneUtil.Find(GameObject.Find("GG_Statue_Hegemol"), "Base"), "Plaque"), "Plaque_Trophy_Centre");
				break;
			}
			if (gameObject != null)
			{
				gameObject.GetComponent<BossStatueTrophyPlaque>().tierCompleteEffectDelay = 0.5f;
				gameObject.GetComponent<BossStatueTrophyPlaque>().DoTierCompleteEffect((BossStatueTrophyPlaque.DisplayType)lev);
			}
			lev = 0;
		}

		private GameObject SetStatue(Vector2 pos, Vector2 offset, Vector2 nameOffset, GameObject go, string sceneName, Sprite spr, string name, string desc, string state)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(go);
			gameObject.name = "GG_Statue_" + state.Substring(11);
			gameObject.transform.SetPosition3D(pos.x, pos.y, 0f);
			BossScene bossScene = ScriptableObject.CreateInstance<BossScene>();
			bossScene.sceneName = sceneName;
			BossStatue component = gameObject.GetComponent<BossStatue>();
			component.bossScene = bossScene;
			component.statueStatePD = null;
			switch (name)
			{
			case "ISMA_NAME":
				if (FiveKnights.Instance.SaveSettings.CompletionIsma.isUnlocked)
				{
					component.statueStatePD = state;
					component.StatueState = FiveKnights.Instance.SaveSettings.CompletionIsma;
				}
				if (FiveKnights.Instance.SaveSettings.CompletionIsma2.isUnlocked)
				{
					SetStatue2(gameObject, "GG_White_Defender", "statueStateIsma2", "DD_ISMA_NAME", "DD_ISMA_DESC");
					component.DreamStatueState = FiveKnights.Instance.SaveSettings.CompletionIsma2;
					component.SetDreamVersion(FiveKnights.Instance.SaveSettings.AltStatueIsma, useAltStatue: false, doAnim: false);
				}
				break;
			case "DRYYA_NAME":
				if (FiveKnights.Instance.SaveSettings.CompletionDryya.isUnlocked)
				{
					component.statueStatePD = state;
					component.StatueState = FiveKnights.Instance.SaveSettings.CompletionDryya;
				}
				break;
			case "ZEM_NAME":
				if (FiveKnights.Instance.SaveSettings.CompletionZemer.isUnlocked)
				{
					component.statueStatePD = state;
					component.StatueState = FiveKnights.Instance.SaveSettings.CompletionZemer;
				}
				if (FiveKnights.Instance.SaveSettings.CompletionZemer2.isUnlocked)
				{
					SetStatue2(gameObject, sceneName, "statueStateZemer2", "ZEM2_NAME", "ZEM2_DESC");
					component.DreamStatueState = FiveKnights.Instance.SaveSettings.CompletionZemer2;
					component.SetDreamVersion(FiveKnights.Instance.SaveSettings.AltStatueZemer, useAltStatue: false, doAnim: false);
				}
				break;
			case "HEG_NAME":
				if (FiveKnights.Instance.SaveSettings.CompletionHegemol.isUnlocked)
				{
					component.statueStatePD = state;
					component.StatueState = FiveKnights.Instance.SaveSettings.CompletionHegemol;
				}
				break;
			}
			component.SetPlaquesVisible(component.StatueState.isUnlocked && component.StatueState.hasBeenSeen);
			component.bossDetails = new BossStatue.BossUIDetails
			{
				nameKey = name,
				nameSheet = "Speech",
				descriptionKey = desc,
				descriptionSheet = "Speech"
			};
			foreach (Transform item in gameObject.transform)
			{
				if (item.name.Contains("door"))
				{
					item.name = "door_dreamReturnGG" + state;
				}
			}
			GameObject gameObject2 = gameObject.transform.Find("Base").Find("Statue").gameObject;
			gameObject2.SetActive(value: true);
			SpriteRenderer component2 = gameObject2.transform.Find("GG_statues_0006_5").GetComponent<SpriteRenderer>();
			component2.enabled = false;
			component2.sprite = spr;
			float num = (state.Contains("Zemer") ? 1.2f : 1.4f);
			component2.transform.localScale *= num;
			component2.transform.SetPosition3D(component2.transform.GetPositionX() + offset.x, component2.transform.GetPositionY() + offset.y, 2f);
			GameObject gameObject3 = new GameObject("FakeStat" + state.Substring(11));
			SpriteRenderer spriteRenderer = gameObject3.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = spr;
			gameObject3.transform.parent = gameObject.transform;
			gameObject3.transform.localScale = gameObject2.transform.Find("GG_statues_0006_5").localScale;
			gameObject3.transform.position = gameObject2.transform.Find("GG_statues_0006_5").position;
			gameObject3.SetActive(component.StatueState.isUnlocked && component.StatueState.hasBeenSeen);
			Mirror.SetField(USceneUtil.Find(USceneUtil.Find(gameObject, "Base"), "GG_Statue_First_Appear").GetComponent<BossStatueFlashEffect>(), "statueSprites", new SpriteRenderer[1] { spriteRenderer });
			if (state.Contains("Isma") || state.Contains("Zemer"))
			{
				StatueControl statueControl = gameObject.transform.Find("Base").gameObject.AddComponent<StatueControl>();
				statueControl.StatueName = state;
				statueControl._bs = component;
				statueControl._sr = spriteRenderer;
				if (state.Contains("Isma"))
				{
					GameObject gameObject4 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["IsmaOgrimStatue"], gameObject.transform);
					GameObject gameObject5 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["IsmaOgrimStatue_Silver"], gameObject.transform);
					gameObject5.transform.localScale = gameObject2.transform.Find("GG_statues_0006_5").localScale / 1.15f;
					gameObject5.transform.position = gameObject2.transform.Find("GG_statues_0006_5").position;
					gameObject5.SetActive(value: false);
					gameObject4.transform.localScale = gameObject2.transform.Find("GG_statues_0006_5").localScale / 1.15f;
					gameObject4.transform.position = gameObject2.transform.Find("GG_statues_0006_5").position;
					gameObject4.SetActive(value: false);
					(FiveKnights.Instance.SaveSettings.CompletionIsma2.completedTier3 ? gameObject5 : gameObject4).SetActive(value: true);
					statueControl._fakeStatAlt2 = (FiveKnights.Instance.SaveSettings.CompletionIsma2.completedTier3 ? gameObject5 : gameObject4);
				}
				statueControl._fakeStatAlt = gameObject3;
				if (state.Contains("Isma"))
				{
					StatueControls["Isma"] = statueControl;
				}
				else
				{
					StatueControls["Zemer"] = statueControl;
				}
			}
			Vector3 position = gameObject.transform.Find("Inspect").Find("Prompt Marker").position;
			gameObject.transform.Find("Inspect").Find("Prompt Marker").position = new Vector3(position.x + nameOffset.x, position.y + nameOffset.y, position.z);
			component.disableIfLocked = new GameObject[2]
			{
				gameObject.transform.Find("Inspect").gameObject,
				gameObject.transform.Find("Spotlight").gameObject
			};
			component.enableIfLocked = new GameObject[1] { gameObject.transform.Find("Inspect_Locked").gameObject };
			gameObject.SetActive(value: true);
			return gameObject;
		}

		private void SetStatue2(GameObject statue, string sceneN, string stateN, string key, string desc)
		{
			BossScene bossScene = ScriptableObject.CreateInstance<BossScene>();
			bossScene.sceneName = sceneN;
			BossStatue component = statue.GetComponent<BossStatue>();
			component.dreamBossScene = bossScene;
			component.dreamStatueStatePD = stateN;
			UnityEngine.Object.Destroy(statue.FindGameObjectInChildren("StatueAlt"));
			GameObject statueDisplay = component.statueDisplay;
			GameObject gameObject = UnityEngine.Object.Instantiate(statueDisplay, statueDisplay.transform.parent, worldPositionStays: true);
			gameObject.SetActive(component.UsingDreamVersion);
			gameObject.GetComponentInChildren<SpriteRenderer>(includeInactive: true).flipX = true;
			gameObject.name = "StatueAlt";
			component.statueDisplayAlt = gameObject;
			component.dreamBossDetails = new BossStatue.BossUIDetails
			{
				nameKey = key,
				nameSheet = "Speech",
				descriptionKey = desc,
				descriptionSheet = "Speech"
			};
			GameObject obj = statue.FindGameObjectInChildren("alt_lever");
			obj.SetActive(value: true);
			obj.FindGameObjectInChildren("GG_statue_switch_bracket").SetActive(value: true);
			obj.FindGameObjectInChildren("GG_statue_switch_lever").SetActive(value: true);
			BossStatueLever componentInChildren = statue.GetComponentInChildren<BossStatueLever>();
			componentInChildren.SetOwner(component);
			componentInChildren.SetState(value: true);
		}

		private void OnDestroy()
		{
			On.BossStatueLever.OnTriggerEnter2D -= BossStatueLever_OnTriggerEnter2D;
			On.BossStatueFlashEffect.FlashApex -= BossStatueFlashEffect_FlashApex;
			On.HutongGames.PlayMaker.Actions.CallMethodProper.OnEnter -= CallMethodProperOnEnter;
			GameManager.instance.OnFinishedEnteringScene -= GMOnFinishedEnteringScene;
			On.CameraController.UpdateTargetDestinationDelta -= CameraControllerUpdateTarget;
			On.GameManager.EnterHero -= GameManager_EnterHero;
			On.BossChallengeUI.LoadBoss_int_bool -= BossChallengeUI_LoadBoss_int_bool;
		}

		private static void Log(object o)
		{
			Modding.Logger.Log("[WP] " + o);
		}
	}
	public class OWBossManager : MonoBehaviour
	{
		private HealthManager _hm;

		private PlayMakerFSM _fsm;

		private GameObject _dd;

		private tk2dSpriteAnimator _tk;

		public MusicPlayer _ap;

		public MusicPlayer _ap2;

		public static OWBossManager Instance;

		private IEnumerator Start()
		{
			Instance = this;
			GameObject gameObject = GameObject.Find("White Defender");
			if (gameObject != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
			_dd = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["WhiteDef"]);
			FiveKnights.preloadedGO["WD"] = _dd;
			_dd.SetActive(value: false);
			_hm = _dd.GetComponent<HealthManager>();
			_fsm = _dd.LocateMyFSM("Dung Defender");
			_tk = _dd.GetComponent<tk2dSpriteAnimator>();
			OnDestroy();
			Log("Curr Boss " + CustomWP.boss);
			if (CustomWP.boss == CustomWP.Boss.Isma)
			{
				GameCameras.instance.cameraShakeFSM.FsmVariables.FindFsmBool("RumblingMed").Value = false;
				GameObject ogrim = GameObject.Find("Ogrim");
				yield return new WaitWhile(() => HeroController.instance == null);
				PlayMusic(FiveKnights.Clips["IsmaAreaMusic"]);
				yield return new WaitWhile(() => HeroController.instance.transform.position.x < 110.5f);
				IsmaController ic = BossLoader.CreateIsma(onlyIsma: true);
				ogrim.AddComponent<OgrimBG>().target = ic.transform;
				ic.onlyIsma = true;
				ic.gameObject.SetActive(value: true);
				yield return new WaitWhile(() => ic != null);
				PlayMusic(null);
				yield return new WaitForSeconds(3f);
				WinRoutine(OWArenaFinder.PrevIsmScene, 3);
				Log("Done with Isma boss");
				UnityEngine.Object.Destroy(this);
			}
			else if (CustomWP.boss == CustomWP.Boss.Dryya)
			{
				yield return new WaitWhile(() => HeroController.instance == null);
				DryyaSetup dc = BossLoader.CreateDryya();
				dc.gameObject.SetActive(value: false);
				PlayMusic(FiveKnights.Clips["DryyaAreaMusic"]);
				HeroController hc = HeroController.instance;
				yield return new WaitUntil(() => hc.transform.position.x > 427.5f && hc.transform.position.y < 120f);
				PlayMusic(null);
				dc.gameObject.SetActive(value: true);
				yield return new WaitWhile(() => dc != null);
				PlayMusic(null);
				yield return new WaitForSeconds(3f);
				WinRoutine(OWArenaFinder.PrevDryScene, 0);
				Log("Done with Dryya boss");
				UnityEngine.Object.Destroy(this);
			}
			else if (CustomWP.boss == CustomWP.Boss.Hegemol)
			{
				foreach (Transform item in GameObject.Find("waterfall").transform)
				{
					item.GetComponent<SpriteRenderer>().material = new Material(Shader.Find("UI/BlendModes/LinearDodge"));
				}
				AddTramAndNPCs();
				HegemolController hegemolCtrl = BossLoader.CreateHegemol();
				GameCameras.instance.cameraShakeFSM.FsmVariables.FindFsmBool("RumblingMed").Value = false;
				yield return new WaitWhile(() => HeroController.instance == null);
				PlayMusic(FiveKnights.Clips["HegAreaMusicIntro"]);
				PlayHegemolBGSound(hegemolCtrl);
				yield return new WaitForSeconds(FiveKnights.Clips["HegAreaMusicIntro"].length);
				PlayMusic(FiveKnights.Clips["HegAreaMusic"]);
				yield return new WaitWhile(() => HeroController.instance.transform.position.x < 427f);
				hegemolCtrl.gameObject.SetActive(value: true);
				yield return new WaitWhile(() => hegemolCtrl != null);
				yield return new WaitForSeconds(3f);
				Tram[] array = UnityEngine.Object.FindObjectsOfType<Tram>();
				for (int num = 0; num < array.Length; num++)
				{
					array[num].FadeAudio();
				}
				WinRoutine(OWArenaFinder.PrevHegScene, 2);
				Log("Done with Heg, transitioning out");
				UnityEngine.Object.Destroy(this);
			}
			else
			{
				if (CustomWP.boss != CustomWP.Boss.Ze)
				{
					yield break;
				}
				ZemerController.WaitForTChild = true;
				ZemerController zc = BossLoader.CreateZemer();
				PlayMusic(FiveKnights.Clips["Zem_Area"]);
				GameObject ogrim = zc.gameObject;
				ogrim.SetActive(value: true);
				ogrim.GetComponent<HealthManager>().IsInvincible = true;
				GameObject gameObject2 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["TChild"]);
				TChildCtrl tChild = gameObject2.AddComponent<TChildCtrl>();
				gameObject2.SetActive(value: true);
				tChild.zemer = ogrim;
				yield return null;
				yield return new WaitWhile(() => !tChild.helpZemer);
				ZemerController.WaitForTChild = false;
				ogrim.GetComponent<HealthManager>().IsInvincible = false;
				yield return new WaitWhile(() => zc != null);
				if (ogrim == null)
				{
					Log("Zem did not exist so destroying");
					UnityEngine.Object.Destroy(this);
					yield break;
				}
				ZemerControllerP2 zc2 = ogrim.GetComponent<ZemerControllerP2>();
				yield return new WaitWhile(() => zc2 != null);
				yield return new WaitForSeconds(3f);
				WinRoutine(OWArenaFinder.PrevZemScene, 1);
				UnityEngine.Object.Destroy(this);
			}
		}

		private void WinRoutine(string area, int index)
		{
			if ((bool)GameManager.instance.GetComponent<AwardCharms>())
			{
				GameManager.instance.GetComponent<AwardCharms>().bossWin[index] = true;
			}
			string stringValue = "placeholder key aaaaaaaa";
			switch (index)
			{
			case 0:
			{
				FiveKnights.Instance.SaveSettings.CompletionDryya.isUnlocked = true;
				FiveKnights.Instance.SaveSettings.DryyaOWWinCount++;
				int ismaOWWinCount = FiveKnights.Instance.SaveSettings.DryyaOWWinCount;
				stringValue = ((ismaOWWinCount >= 5) ? "DRYYA_OUTRO_5" : ("DRYYA_OUTRO_" + ismaOWWinCount));
				break;
			}
			case 1:
			{
				FiveKnights.Instance.SaveSettings.CompletionZemer.isUnlocked = true;
				FiveKnights.Instance.SaveSettings.ZemerOWWinCount++;
				int ismaOWWinCount = FiveKnights.Instance.SaveSettings.ZemerOWWinCount;
				stringValue = ((ismaOWWinCount >= 5) ? "ZEM_OUTRO_5" : ("ZEM_OUTRO_" + ismaOWWinCount));
				break;
			}
			case 2:
			{
				FiveKnights.Instance.SaveSettings.CompletionHegemol.isUnlocked = true;
				FiveKnights.Instance.SaveSettings.HegOWWinCount++;
				int ismaOWWinCount = FiveKnights.Instance.SaveSettings.HegOWWinCount;
				stringValue = ((ismaOWWinCount >= 5) ? "HEG_OUTRO_5" : ("HEG_OUTRO_" + ismaOWWinCount));
				break;
			}
			case 3:
			{
				FiveKnights.Instance.SaveSettings.CompletionIsma.isUnlocked = true;
				FiveKnights.Instance.SaveSettings.IsmaOWWinCount++;
				int ismaOWWinCount = FiveKnights.Instance.SaveSettings.IsmaOWWinCount;
				stringValue = ((ismaOWWinCount >= 5) ? "ISMA_OUTRO_5" : ("ISMA_OUTRO_" + ismaOWWinCount));
				break;
			}
			}
			PlayMakerFSM[] componentsInChildren = HeroController.instance.gameObject.GetComponentsInChildren<PlayMakerFSM>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SendEvent("FSM CANCEL");
			}
			HeroController.instance.AffectedByGravity(gravityApplies: true);
			HeroController.instance.StartAnimationControl();
			HeroController.instance.RelinquishControl();
			PlayerData.instance.disablePause = true;
			GameObject gameObject = GameObject.Find("Dream Exit Particle Field");
			if (gameObject != null)
			{
				gameObject.GetComponent<ParticleSystem>().Play();
			}
			GameObject obj = UnityEngine.Object.Instantiate(Mirror.GetField<EnemyDeathEffects, GameObject>(_dd.GetComponent<EnemyDeathEffectsUninfected>(), "corpsePrefab"));
			obj.SetActive(value: true);
			PlayMakerFSM playMakerFSM = obj.LocateMyFSM("Control");
			TextMeshPro component = playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.SetTextMeshProAlignment>("New Scene", 1).gameObject.GameObject.Value.GetComponent<TextMeshPro>();
			playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.Wait>("Fade Out", 4).time.Value += 2f;
			GameObject.Find("Blanker White").LocateMyFSM("Blanker Control").FsmVariables.FindFsmFloat("Fade Time").Value = 0f;
			Vasi.FsmUtil.RemoveAction(playMakerFSM.GetState("Fade Out"), 0);
			Vasi.FsmUtil.ChangeTransition(playMakerFSM, "Take Control", "FINISHED", "Outro Msg 1a");
			Vasi.FsmUtil.ChangeTransition(playMakerFSM, "Outro Msg 1a", "CONVO_FINISH", "New Scene");
			component.color = Color.black;
			component.alignment = TextAlignmentOptions.Center;
			playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.CallMethodProper>("Outro Msg 1a", 0).parameters[0].stringValue = stringValue;
			playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.CallMethodProper>("Outro Msg 1a", 0).parameters[1].stringValue = "Speech";
			playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.BeginSceneTransition>("New Scene", 6).preventCameraFadeOut = true;
			playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.BeginSceneTransition>("New Scene", 6).sceneName = area;
			playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.BeginSceneTransition>("New Scene", 6).entryGateName = "door_dreamReturn";
			playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.BeginSceneTransition>("New Scene", 6).visualization.Value = GameManager.SceneLoadVisualizations.Default;
			playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.BeginSceneTransition>("New Scene", 6).entryDelay = 0f;
			HeroController.instance.EnterWithoutInput(flag: true);
			HeroController.instance.MaxHealth();
			playMakerFSM.SetState("Fade Out");
		}

		public static void PlayMusic(AudioClip clip)
		{
			MusicCue musicCue = ScriptableObject.CreateInstance<MusicCue>();
			MusicCue.MusicChannelInfo musicChannelInfo = new MusicCue.MusicChannelInfo();
			Mirror.SetField(musicChannelInfo, "clip", clip);
			MusicCue.MusicChannelInfo[] value = new MusicCue.MusicChannelInfo[6] { musicChannelInfo, null, null, null, null, null };
			Mirror.SetField(musicCue, "channelInfos", value);
			Resources.FindObjectsOfTypeAll<AudioMixer>().First((AudioMixer x) => x.name == "Music").FindSnapshot("Main Only")
				.TransitionTo(0f);
			GameManager.instance.AudioManager.ApplyMusicCue(musicCue, 0f, 0f, applySnapshot: false);
		}

		private void AddTramAndNPCs()
		{
			foreach (Transform item in GameObject.Find("BG_Mobs").transform)
			{
				foreach (Transform item2 in item)
				{
					GameObject gameObject = item2.gameObject;
					switch (item.name)
					{
					case "Carriage":
						gameObject.AddComponent<Carriage>();
						break;
					case "Husk":
						gameObject.AddComponent<HuskCitizen>();
						break;
					case "HuskCart":
						gameObject.AddComponent<HuskCart>();
						break;
					case "Maggot":
						gameObject.AddComponent<Maggot>();
						break;
					case "MineCart":
						gameObject.AddComponent<MineBugCart>();
						break;
					}
				}
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Tram"]);
			gameObject2.AddComponent<Tram>();
			GameObject obj = GameObject.Find("Riders");
			obj.transform.position = gameObject2.transform.position;
			obj.transform.parent = gameObject2.transform;
			obj.transform.localPosition = new Vector3(0f, -2.45f, 0f);
			gameObject2.SetActive(value: true);
			GameObject obj2 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Tram"]);
			obj2.AddComponent<TramSmall>();
			obj2.SetActive(value: true);
		}

		private static void PlayHegemolBGSound(HegemolController heg)
		{
			GameObject obj = new GameObject("Audio Player", typeof(AudioSource), typeof(AutoDestroy));
			obj.transform.position = new Vector3(437f, 171.1914f, 0f);
			obj.GetComponent<AutoDestroy>().ShouldDestroy = () => heg.gameObject.activeSelf;
			AudioSource component = obj.GetComponent<AudioSource>();
			component.clip = FiveKnights.Clips["HegAreaMusicBG"];
			component.volume = 1f;
			component.pitch = 1f;
			component.loop = true;
			component.maxDistance = 150f;
			component.outputAudioMixerGroup = HeroController.instance.GetComponent<AudioSource>().outputAudioMixerGroup;
			component.Play();
		}

		private void OnDestroy()
		{
			_ap?.StopMusic();
			_ap2?.StopMusic();
		}

		private void Log(object o)
		{
			if (FiveKnights.isDebug)
			{
				Modding.Logger.Log("[OWBossManager] " + o);
			}
		}
	}
	public class AbyssalBloom : MonoBehaviour
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
			_hcAnim = _hc.GetComponent<tk2dSpriteAnimator>();
			_knightBall = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Knight Ball"], _hc.transform);
			Vector3 localScale = _knightBall.transform.localScale;
			localScale.x *= -1f;
			_knightBall.transform.localScale = localScale;
			_knightBall.transform.localPosition += new Vector3(-4.75f, -0.25f);
			_knightBallAnim = _knightBall.GetComponent<tk2dSpriteAnimator>();
			_nailSlashes = new List<NailSlash>
			{
				HeroController.instance.normalSlash,
				HeroController.instance.alternateSlash,
				HeroController.instance.downSlash,
				HeroController.instance.upSlash
			};
			_modifyProps = GetComponent<ModifyBloomProps>();
			PlayMakerFSM fsm = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Radiance"].LocateMyFSM("Control"), _hc.transform);
			FiveKnights.Clips["Shade Slash"] = (AudioClip)fsm.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Antic", 1).audioClip.Value;
			On.HealthManager.Hit += HealthManagerHit;
			On.HeroController.CancelAttack += HeroControllerCancelAttack;
			On.HeroController.CancelDownAttack += HeroControllerCancelDownAttack;
			On.HeroController.Attack += DoVoidAttack;
			On.tk2dSpriteAnimator.Play_string += Tk2dSpriteAnimatorPlay;
			On.KnightHatchling.OnEnable += KnightHatchlingOnEnable;
			On.SpriteFlash.FlashingFury += SpriteFlashFlashingFury;
		}

		private void OnDisable()
		{
			SetLevel(0);
			On.HealthManager.Hit -= HealthManagerHit;
			On.HeroController.CancelAttack -= HeroControllerCancelAttack;
			On.HeroController.CancelDownAttack -= HeroControllerCancelDownAttack;
			On.HeroController.Attack -= DoVoidAttack;
			On.tk2dSpriteAnimator.Play_string -= Tk2dSpriteAnimatorPlay;
			On.KnightHatchling.OnEnable -= KnightHatchlingOnEnable;
			On.SpriteFlash.FlashingFury -= SpriteFlashFlashingFury;
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

		private void ModifySlashColors(bool modify)
		{
			foreach (NailSlash nailSlash in _nailSlashes)
			{
				nailSlash.SetFury(modify);
			}
			Color color = (modify ? Color.black : Color.white);
			GameObject[] array = new GameObject[5] { _hc.slashPrefab, _hc.slashAltPrefab, _hc.downSlashPrefab, _hc.upSlashPrefab, _hc.wallSlashPrefab };
			for (int i = 0; i < array.Length; i++)
			{
				array[i].GetComponent<tk2dSprite>().color = color;
			}
			GameObject o = HeroController.instance.gameObject.FindGameObjectInChildren("Attacks");
			string[] array2 = new string[3] { "Cyclone Slash", "Dash Slash", "Great Slash" };
			foreach (string text in array2)
			{
				o.FindGameObjectInChildren(text).GetComponent<tk2dSprite>().color = color;
				tk2dSprite[] componentsInChildren = o.FindGameObjectInChildren(text).GetComponentsInChildren<tk2dSprite>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].color = color;
				}
			}
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			AttackTypes attackType = hitInstance.AttackType;
			if ((attackType == AttackTypes.Nail || attackType == AttackTypes.NailBeam) ? true : false)
			{
				hitInstance.Multiplier += damageBuff + (_fury ? 0.75f : 0f);
			}
			if (hitInstance.AttackType == AttackTypes.SharpShadow)
			{
				hitInstance.Multiplier += 2f * damageBuff;
			}
			orig(self, hitInstance);
		}

		private void HeroControllerCancelDownAttack(On.HeroController.orig_CancelDownAttack orig, HeroController self)
		{
			if (_vertSlashCoro != null)
			{
				CancelVerticalTendrilAttack();
			}
			orig(self);
		}

		private void HeroControllerCancelAttack(On.HeroController.orig_CancelAttack orig, HeroController self)
		{
			if (_sideSlashCoro != null)
			{
				CancelTendrilAttack();
			}
			orig(self);
		}

		private void Tk2dSpriteAnimatorPlay(On.tk2dSpriteAnimator.orig_Play_string orig, tk2dSpriteAnimator self, string name)
		{
			if (self.gameObject == _hc.gameObject && name == "Idle Hurt")
			{
				self.Play("Idle");
			}
			else
			{
				orig(self, name);
			}
		}

		private void SpriteFlashFlashingFury(On.SpriteFlash.orig_FlashingFury orig, SpriteFlash self)
		{
			self.flash(Color.black, 0.75f, 0.25f, 0.01f, 0.25f);
			Mirror.SetField(self, "repeatFlash", value: true);
		}

		private void KnightHatchlingOnEnable(On.KnightHatchling.orig_OnEnable orig, KnightHatchling self)
		{
			orig(self);
			if (_level == 2)
			{
				KnightHatchling.TypeDetails field = Mirror.GetField<KnightHatchling, KnightHatchling.TypeDetails>(self, "details");
				KnightHatchling.TypeDetails value = field;
				value.damage = field.damage * 2;
				Mirror.SetField(self, "details", value);
			}
		}

		private void DoVoidAttack(On.HeroController.orig_Attack origAttack, HeroController hc, AttackDirection dir)
		{
			if (_level != 2)
			{
				origAttack(hc, dir);
				return;
			}
			InputHandler instance = InputHandler.Instance;
			if (_pd.GetBool("equippedCharm_32"))
			{
				Mirror.SetField(_hc, "attackDuration", _hc.ATTACK_DURATION_CH);
				Mirror.SetField(_hc, "attack_cooldown", _hc.ATTACK_COOLDOWN_TIME_CH);
			}
			else
			{
				Mirror.SetField(_hc, "attackDuration", _hc.ATTACK_DURATION);
				Mirror.SetField(_hc, "attack_cooldown", _hc.ATTACK_COOLDOWN_TIME);
			}
			_hc.cState.recoiling = false;
			if (hc.cState.wallSliding)
			{
				if (_hc.cState.attacking)
				{
					CancelWallTendrilAttack();
				}
				_wallSlashCoro = StartCoroutine(WallTendrilAttack());
				return;
			}
			if ((bool)instance.ActionButtonToPlayerAction(HeroActionButton.DOWN) && !hc.CheckTouchingGround())
			{
				if (_hc.cState.attacking)
				{
					CancelVerticalTendrilAttack();
				}
				_vertSlashCoro = StartCoroutine(VerticalTendrilAttack(up: false));
				return;
			}
			if ((bool)instance.ActionButtonToPlayerAction(HeroActionButton.UP))
			{
				if (_hc.cState.attacking)
				{
					CancelVerticalTendrilAttack();
				}
				_vertSlashCoro = StartCoroutine(VerticalTendrilAttack(up: true));
				return;
			}
			if (_hc.cState.attacking)
			{
				CancelTendrilAttack();
				_shadeSlashNum = ((_shadeSlashNum != 1) ? 1 : 2);
			}
			_sideSlashCoro = StartCoroutine(TendrilAttack());
		}

		public void CancelTendrilAttack()
		{
			Log("Canceling attack");
			if (_sideSlashCoro != null)
			{
				StopCoroutine(_sideSlashCoro);
			}
			UnityEngine.Object.Destroy(_sideSlash);
			_knightBall.SetActive(value: false);
			_hc.GetComponent<MeshRenderer>().enabled = true;
			ResetTendrilAttack();
		}

		public void CancelVerticalTendrilAttack()
		{
			Log("Canceling vertical attack");
			if (_vertSlashCoro != null)
			{
				StopCoroutine(_vertSlashCoro);
			}
			UnityEngine.Object.Destroy(_shadeSlashContainer);
			_hc.StartAnimationControl();
			ResetTendrilAttack();
		}

		public void CancelWallTendrilAttack()
		{
			Log("Canceling wall attack");
			if (_wallSlashCoro != null)
			{
				StopCoroutine(_wallSlashCoro);
			}
			UnityEngine.Object.Destroy(_wallSlash);
			_hc.StartAnimationControl();
			ResetTendrilAttack();
		}

		private void ResetTendrilAttack()
		{
			_hc.cState.attacking = false;
			_hc.cState.upAttacking = false;
			_hc.cState.downAttacking = false;
			Mirror.SetField(_hc, "attack_time", 0f);
		}

		private IEnumerator TendrilAttack()
		{
			_hc.cState.attacking = true;
			MeshRenderer mr = _hc.GetComponent<MeshRenderer>();
			if (!playingAudio)
			{
				StartCoroutine(PlayAudio());
			}
			mr.enabled = false;
			_knightBall.SetActive(value: true);
			UnityEngine.Object.Destroy(_sideSlash);
			_sideSlash = new GameObject("Shade Slash");
			_sideSlash.transform.parent = _knightBall.transform;
			_sideSlash.layer = 17;
			_sideSlash.tag = "Nail Attack";
			_sideSlash.transform.localPosition = Vector3.zero;
			_sideSlash.transform.localScale = Vector3.one;
			_sideSlash.SetActive(value: false);
			AddDamageEnemiesFsm(_sideSlash, AttackDirection.normal);
			PolygonCollider2D polygonCollider2D = _sideSlash.AddComponent<PolygonCollider2D>();
			polygonCollider2D.points = new Vector2[6]
			{
				new Vector2(0f, -2f),
				new Vector2(3.5f, -2f),
				new Vector2(3.5f, 0f),
				new Vector2(3f, 1f),
				new Vector2(0f, 2f),
				new Vector2(-3f, 0f)
			};
			polygonCollider2D.offset = Vector2.zero;
			polygonCollider2D.isTrigger = true;
			GameObject obj = UnityEngine.Object.Instantiate(_sideSlash, _sideSlash.transform);
			obj.LocateMyFSM("damages_enemy").GetFsmIntVariable("damageDealt").Value = 0;
			obj.layer = 16;
			_sideSlash.AddComponent<ShadeSlash>().attackDirection = AttackDirection.normal;
			_sideSlash.SetActive(value: true);
			obj.SetActive(value: true);
			_knightBallAnim.PlayFromFrame("Slash" + _shadeSlashNum + " Antic", 2);
			yield return new WaitWhile(() => _knightBallAnim.IsPlaying("Slash" + _shadeSlashNum + " Antic"));
			yield return new WaitForSeconds(_knightBallAnim.PlayAnimGetTime("Slash" + _shadeSlashNum) - 1f / 24f);
			UnityEngine.Object.Destroy(_sideSlash);
			mr.enabled = true;
			_knightBall.SetActive(value: false);
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
				StartCoroutine(PlayAudio());
			}
			_hcAnim.Play(animName + "Slash Void");
			tk2dSpriteAnimationClip clipByName = _hcAnim.GetClipByName(animName + "Slash Void");
			_hcAnim.Play(clipByName);
			UnityEngine.Object.Destroy(_shadeSlashContainer);
			_shadeSlashContainer = UnityEngine.Object.Instantiate(new GameObject("Shade Slash Container"), _hc.transform);
			_shadeSlashContainer.layer = 17;
			_shadeSlashContainer.SetActive(value: false);
			GameObject gameObject = new GameObject("Shade Slash");
			gameObject.transform.parent = _shadeSlashContainer.transform;
			gameObject.layer = 17;
			gameObject.tag = "Nail Attack";
			gameObject.transform.localPosition = new Vector3(0f, up ? 1f : (-2f), 0f);
			gameObject.transform.localScale = new Vector3(2f, 2f, 2f);
			AddDamageEnemiesFsm(gameObject, up ? AttackDirection.upward : AttackDirection.downward);
			PolygonCollider2D polygonCollider2D = gameObject.AddComponent<PolygonCollider2D>();
			if (up)
			{
				polygonCollider2D.points = new Vector2[8]
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
				polygonCollider2D.points = new Vector2[10]
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
			polygonCollider2D.offset = new Vector2(0f, up ? (-1f) : 0.75f);
			polygonCollider2D.isTrigger = true;
			GameObject obj = UnityEngine.Object.Instantiate(gameObject, gameObject.transform);
			obj.LocateMyFSM("damages_enemy").GetFsmIntVariable("damageDealt").Value = 0;
			obj.layer = 16;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			gameObject.AddComponent<MeshRenderer>();
			gameObject.AddComponent<MeshFilter>();
			tk2dSprite obj2 = gameObject.AddComponent<tk2dSprite>();
			tk2dSpriteAnimator tk2dSpriteAnimator2 = gameObject.AddComponent<tk2dSpriteAnimator>();
			obj2.Collection = _hc.GetComponent<tk2dSprite>().Collection;
			tk2dSpriteAnimator2.Library = _hcAnim.Library;
			gameObject.AddComponent<ShadeSlash>().attackDirection = (up ? AttackDirection.upward : AttackDirection.downward);
			_shadeSlashContainer.SetActive(value: true);
			yield return new WaitForSeconds(tk2dSpriteAnimator2.PlayAnimGetTime(animName + "Slash Effect"));
			UnityEngine.Object.Destroy(_shadeSlashContainer);
			yield return new WaitWhile(() => _hcAnim.Playing && _hcAnim.IsPlaying(animName + "Slash Void"));
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
				StartCoroutine(PlayAudio());
			}
			_hcAnim.Play(_hcAnim.GetClipByName("WallSlash Void"));
			UnityEngine.Object.Destroy(_wallSlash);
			_wallSlash = new GameObject("Shade Slash");
			_wallSlash.transform.parent = _hc.transform;
			_wallSlash.layer = 17;
			_wallSlash.tag = "Nail Attack";
			_wallSlash.transform.localPosition = new Vector3(0f, 1f, 0f);
			_wallSlash.transform.localScale = Vector3.one;
			_wallSlash.SetActive(value: false);
			AddDamageEnemiesFsm(_wallSlash, AttackDirection.normal);
			PolygonCollider2D polygonCollider2D = _wallSlash.AddComponent<PolygonCollider2D>();
			polygonCollider2D.points = new Vector2[5]
			{
				new Vector2(-1.5f, 2f),
				new Vector2(1f, 1.5f),
				new Vector2(3f, -1f),
				new Vector2(1f, -2f),
				new Vector2(-1.5f, -1f)
			};
			polygonCollider2D.offset = new Vector2(1f, 0f);
			polygonCollider2D.isTrigger = true;
			GameObject obj = UnityEngine.Object.Instantiate(_wallSlash, _wallSlash.transform);
			obj.LocateMyFSM("damages_enemy").GetFsmIntVariable("damageDealt").Value = 0;
			obj.layer = 16;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			obj.SetActive(value: false);
			_wallSlash.AddComponent<MeshRenderer>();
			_wallSlash.AddComponent<MeshFilter>();
			tk2dSprite obj2 = _wallSlash.AddComponent<tk2dSprite>();
			tk2dSpriteAnimator tk2dSpriteAnimator2 = _wallSlash.AddComponent<tk2dSpriteAnimator>();
			obj2.Collection = _hc.GetComponent<tk2dSprite>().Collection;
			tk2dSpriteAnimator2.Library = _hcAnim.Library;
			_wallSlash.AddComponent<ShadeSlash>().attackDirection = AttackDirection.normal;
			_wallSlash.SetActive(value: true);
			obj.SetActive(value: true);
			yield return new WaitForSeconds(tk2dSpriteAnimator2.PlayAnimGetTime("Slash Effect"));
			UnityEngine.Object.Destroy(_wallSlash);
			_hc.StartAnimationControl();
			_hc.cState.attacking = false;
		}

		private IEnumerator PlayAudio()
		{
			playingAudio = true;
			this.PlayAudio(FiveKnights.Clips["Shade Slash"], 0.7f);
			yield return new WaitForSeconds(audioCooldown);
			playingAudio = false;
		}

		private void AddDamageEnemiesFsm(GameObject o, AttackDirection dir)
		{
			PlayMakerFSM playMakerFSM = o.AddComponent<PlayMakerFSM>();
			PlayMakerFSM obj = USceneUtil.Find(_hc.gameObject, "AltSlash").LocateMyFSM("damages_enemy");
			FieldInfo[] fields = typeof(PlayMakerFSM).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				fieldInfo.SetValue(playMakerFSM, fieldInfo.GetValue(obj));
			}
			switch (dir)
			{
			case AttackDirection.normal:
				playMakerFSM.GetFsmFloatVariable("direction").Value = (_hc.cState.facingRight ? 0f : 180f);
				break;
			case AttackDirection.upward:
				playMakerFSM.GetFsmFloatVariable("direction").Value = 90f;
				break;
			case AttackDirection.downward:
				playMakerFSM.GetFsmFloatVariable("direction").Value = 270f;
				break;
			}
		}

		private void Log(object o)
		{
			Modding.Logger.Log("[Abyssal Bloom] " + o);
		}
	}
	internal class Amulets : MonoBehaviour
	{
		private PlayMakerFSM _spellControl;

		private PlayMakerFSM _blastControl;

		private PlayMakerFSM _pvControl;

		private GameObject _audioPlayerActor;

		private bool _activated;

		private GameObject _blastKnight;

		private HeroController _hc => HeroController.instance;

		private PlayerData _pd => PlayerData.instance;

		public void Awake()
		{
			On.HeroController.Start += On_HeroController_Start;
			On.CharmIconList.GetSprite += CharmIconList_GetSprite;
			ModHooks.CharmUpdateHook += CharmUpdate;
		}

		private Sprite CharmIconList_GetSprite(On.CharmIconList.orig_GetSprite orig, CharmIconList self, int id)
		{
			if (FiveKnights.Instance.SaveSettings.upgradedCharm_10)
			{
				self.spriteList[10] = FiveKnights.SPRITES["Kings_Honour"];
			}
			else
			{
				self.spriteList[10] = FiveKnights.SPRITES["Defenders_Crest"];
			}
			return orig(self, id);
		}

		public void On_HeroController_Start(On.HeroController.orig_Start orig, HeroController self)
		{
			orig(self);
			Log("Amulets Start");
			Log("Dash Cooldown: " + _hc.DASH_COOLDOWN);
			Log("Dash Cooldown Charm: " + _hc.DASH_COOLDOWN_CH);
			Log("Dash Speed: " + _hc.DASH_SPEED);
			Log("Dash Speed Sharp: " + _hc.DASH_SPEED_SHARP);
			_pvControl = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["PV"].LocateMyFSM("Control"), _hc.transform);
			GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Blast"]);
			gameObject.SetActive(value: false);
			_blastControl = gameObject.LocateMyFSM("Control");
			Log("Waiting for Audio Player Actor...");
			_spellControl = _hc.spellControl;
			_spellControl.GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>("Fireball 2", 3).gameObject.Value.LocateMyFSM("Fireball Cast");
			_hc.gameObject.AddComponent<RoyalAura>().enabled = false;
			_hc.gameObject.AddComponent<Purity>().enabled = false;
			_hc.gameObject.AddComponent<BoonSpells>().enabled = false;
			InsertCharmSpellEffectsInFsm();
			_hc.gameObject.AddComponent<LamentControl>().enabled = false;
			_hc.gameObject.AddComponent<ModifyBloomProps>().enabled = true;
			_hc.gameObject.AddComponent<AbyssalBloom>().enabled = false;
			_hc.gameObject.AddComponent<CheckBloomStage>().enabled = true;
			AddVoidAttacks(_hc);
			ModifyFuryForBloom();
			ModifySpellsForBloom();
			Log("Got Charm 41: " + FiveKnights.Instance.SaveSettings.gotCharms[0]);
			Log("Got Charm 42: " + FiveKnights.Instance.SaveSettings.gotCharms[1]);
			Log("Got Charm 43: " + FiveKnights.Instance.SaveSettings.gotCharms[2]);
			Log("Got Charm 44: " + FiveKnights.Instance.SaveSettings.gotCharms[3]);
			Log("New Charm 41: " + FiveKnights.Instance.SaveSettings.newCharms[0]);
			Log("New Charm 42: " + FiveKnights.Instance.SaveSettings.newCharms[1]);
			Log("New Charm 43: " + FiveKnights.Instance.SaveSettings.newCharms[2]);
			Log("New Charm 44: " + FiveKnights.Instance.SaveSettings.newCharms[3]);
			Log("Equipped Charm 41: " + FiveKnights.Instance.SaveSettings.equippedCharms[0]);
			Log("Equipped Charm 42: " + FiveKnights.Instance.SaveSettings.equippedCharms[1]);
			Log("Equipped Charm 43: " + FiveKnights.Instance.SaveSettings.equippedCharms[2]);
			Log("Equipped Charm 44: " + FiveKnights.Instance.SaveSettings.equippedCharms[3]);
			Log("Upgraded Charm 10: " + FiveKnights.Instance.SaveSettings.upgradedCharm_10);
			Log("Amulets Start Finished");
			_activated = true;
		}

		private void InsertCharmSpellEffectsInFsm()
		{
			SFCore.Utils.FsmUtil.CopyState(_spellControl, "Fireball 1", "Fireball 1 SmallShots");
			SFCore.Utils.FsmUtil.CopyState(_spellControl, "Fireball 2", "Fireball 2 SmallShots");
			_spellControl.RemoveAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>("Fireball 1 SmallShots");
			_spellControl.RemoveAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>("Fireball 2 SmallShots");
			_spellControl.InsertMethod("Fireball 1 SmallShots", 3, delegate
			{
				HeroController.instance.GetComponent<BoonSpells>().CastDaggers(upgraded: false);
			});
			_spellControl.InsertMethod("Fireball 2 SmallShots", 3, delegate
			{
				HeroController.instance.GetComponent<BoonSpells>().CastDaggers(upgraded: true);
			});
			SFCore.Utils.FsmUtil.CopyState(_spellControl, "Quake1 Land", "Q1 Land Plumes");
			SFCore.Utils.FsmUtil.CopyState(_spellControl, "Q2 Land", "Q2 Land Plumes");
			SFCore.Utils.FsmUtil.ChangeTransition(_spellControl, "Q2 Land Plumes", "FINISHED", "Quake Finish");
			_spellControl.InsertMethod("Q1 Land Plumes", delegate
			{
				HeroController.instance.GetComponent<BoonSpells>().CastPlumes(upgraded: false);
			}, 0);
			_spellControl.InsertMethod("Q2 Land Plumes", delegate
			{
				HeroController.instance.GetComponent<BoonSpells>().CastPlumes(upgraded: true);
			}, 0);
			SFCore.Utils.FsmUtil.CopyState(_spellControl, "Scream Antic1", "Scream Antic1 Blasts");
			SFCore.Utils.FsmUtil.CopyState(_spellControl, "Scream Burst 1", "Scream Burst 1 Blasts");
			SFCore.Utils.FsmUtil.CopyState(_spellControl, "Scream Antic2", "Scream Antic2 Blasts");
			SFCore.Utils.FsmUtil.CopyState(_spellControl, "Scream Burst 2", "Scream Burst 2 Blasts");
			SFCore.Utils.FsmUtil.ChangeTransition(_spellControl, "Scream Antic1 Blasts", "FINISHED", "Scream Burst 1 Blasts");
			SFCore.Utils.FsmUtil.ChangeTransition(_spellControl, "Scream Antic2 Blasts", "FINISHED", "Scream Burst 2 Blasts");
			_spellControl.RemoveAction<HutongGames.PlayMaker.Actions.AudioPlay>("Scream Antic1 Blasts");
			_spellControl.RemoveAction<HutongGames.PlayMaker.Actions.CreateObject>("Scream Burst 1 Blasts");
			_spellControl.RemoveAction<HutongGames.PlayMaker.Actions.ActivateGameObject>("Scream Burst 1 Blasts");
			_spellControl.RemoveAction<HutongGames.PlayMaker.Actions.ActivateGameObject>("Scream Burst 1 Blasts");
			_spellControl.RemoveAction<HutongGames.PlayMaker.Actions.SendEventByName>("Scream Burst 1 Blasts");
			_spellControl.RemoveAction<HutongGames.PlayMaker.Actions.SendEventByName>("Scream Burst 1 Blasts");
			_spellControl.InsertMethod("Scream Burst 1 Blasts", 0, delegate
			{
				HeroController.instance.GetComponent<BoonSpells>().CastBlasts(upgraded: false);
			});
			_spellControl.RemoveAction<HutongGames.PlayMaker.Actions.AudioPlay>("Scream Antic2 Blasts");
			_spellControl.RemoveAction<HutongGames.PlayMaker.Actions.CreateObject>("Scream Burst 2 Blasts");
			_spellControl.RemoveAction<HutongGames.PlayMaker.Actions.ActivateGameObject>("Scream Burst 2 Blasts");
			_spellControl.RemoveAction<HutongGames.PlayMaker.Actions.ActivateGameObject>("Scream Burst 2 Blasts");
			_spellControl.RemoveAction<HutongGames.PlayMaker.Actions.SendEventByName>("Scream Burst 2 Blasts");
			_spellControl.RemoveAction<HutongGames.PlayMaker.Actions.SendEventByName>("Scream Burst 2 Blasts");
			_spellControl.InsertMethod("Scream Burst 2 Blasts", 0, delegate
			{
				HeroController.instance.GetComponent<BoonSpells>().CastBlasts(upgraded: true);
			});
			SFCore.Utils.FsmUtil.CopyState(_spellControl, "Focus", "Focus Blast");
			SFCore.Utils.FsmUtil.CopyState(_spellControl, "Focus Heal", "Focus Heal Blast");
			SFCore.Utils.FsmUtil.CopyState(_spellControl, "Start MP Drain", "Start MP Drain Blast");
			SFCore.Utils.FsmUtil.CopyState(_spellControl, "Focus Heal 2", "Focus Heal 2 Blast");
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
			GameObject gameObject = self.gameObject.FindGameObjectInChildren("Attacks");
			Shader shader = self.GetComponent<tk2dSprite>().Collection.spriteDefinitions[0].material.shader;
			tk2dSpriteCollection component = FiveKnights.preloadedGO["Bloom Sprite Prefab"].GetComponent<tk2dSpriteCollection>();
			tk2dSpriteAnimation component2 = FiveKnights.preloadedGO["Bloom Anim Prefab"].GetComponent<tk2dSpriteAnimation>();
			tk2dSprite component3 = self.GetComponent<tk2dSprite>();
			tk2dSpriteAnimator component4 = self.GetComponent<tk2dSpriteAnimator>();
			List<tk2dSpriteDefinition> list = component3.Collection.spriteDefinitions.ToList();
			tk2dSpriteDefinition[] spriteDefinitions = component.spriteCollection.spriteDefinitions;
			foreach (tk2dSpriteDefinition tk2dSpriteDefinition2 in spriteDefinitions)
			{
				tk2dSpriteDefinition2.material.shader = shader;
				list.Add(tk2dSpriteDefinition2);
			}
			component3.Collection.spriteDefinitions = list.ToArray();
			List<tk2dSpriteAnimationClip> list2 = component4.Library.clips.ToList();
			tk2dSpriteAnimationClip[] clips = component2.clips;
			foreach (tk2dSpriteAnimationClip item in clips)
			{
				list2.Add(item);
			}
			component4.Library.clips = list2.ToArray();
			GameObject cycloneSlashVoid = UnityEngine.Object.Instantiate(gameObject.FindGameObjectInChildren("Cyclone Slash"), gameObject.transform);
			cycloneSlashVoid.name = "Cyclone Slash Void";
			cycloneSlashVoid.GetComponent<tk2dSpriteAnimator>().DefaultClipId = component4.GetClipIdByName("Cyclone Slash Effect Void");
			GameObject dashSlashVoid = UnityEngine.Object.Instantiate(gameObject.FindGameObjectInChildren("Dash Slash"), gameObject.transform);
			dashSlashVoid.name = "Dash Slash Void";
			dashSlashVoid.GetComponent<tk2dSpriteAnimator>().DefaultClipId = component4.GetClipIdByName("Dash Slash Effect Void");
			GameObject greatSlashVoid = UnityEngine.Object.Instantiate(gameObject.FindGameObjectInChildren("Great Slash"), gameObject.transform);
			greatSlashVoid.name = "Great Slash Void";
			greatSlashVoid.GetComponent<tk2dSpriteAnimator>().DefaultClipId = component4.GetClipIdByName("Great Slash Effect Void");
			PlayMakerFSM nailArts = self.gameObject.LocateMyFSM("Nail Arts");
			if (nailArts.FsmStates[0].Fsm == null)
			{
				nailArts.Preprocess();
			}
			nailArts.AddState("Bloom Activated CSlash?");
			nailArts.AddState("Bloom Activated DSlash?");
			nailArts.AddState("Bloom Activated GSlash?");
			SFCore.Utils.FsmUtil.CopyState(nailArts, "Cyclone Start", "Cyclone Start Void");
			SFCore.Utils.FsmUtil.CopyState(nailArts, "Hover Start", "Hover Start Void");
			SFCore.Utils.FsmUtil.CopyState(nailArts, "Activate Slash", "Activate Slash Void");
			SFCore.Utils.FsmUtil.CopyState(nailArts, "Play Audio", "Play Audio Void");
			SFCore.Utils.FsmUtil.CopyState(nailArts, "Cyclone Spin", "Cyclone Spin Void");
			SFCore.Utils.FsmUtil.CopyState(nailArts, "Cyclone Extend", "Cyclone Extend Void");
			SFCore.Utils.FsmUtil.CopyState(nailArts, "Cyclone End", "Cyclone End Void");
			SFCore.Utils.FsmUtil.CopyState(nailArts, "Dash Slash", "Dash Slash Void");
			SFCore.Utils.FsmUtil.CopyState(nailArts, "DSlash Move End", "DSlash Move End Void");
			SFCore.Utils.FsmUtil.CopyState(nailArts, "D Slash End", "D Slash End Void");
			SFCore.Utils.FsmUtil.CopyState(nailArts, "G Slash", "G Slash Void");
			SFCore.Utils.FsmUtil.CopyState(nailArts, "Stop Move", "Stop Move Void");
			SFCore.Utils.FsmUtil.CopyState(nailArts, "G Slash End", "G Slash End Void");
			SFCore.Utils.FsmUtil.ChangeTransition(nailArts, "Flash", "FINISHED", "Bloom Activated CSlash?");
			SFCore.Utils.FsmUtil.ChangeTransition(nailArts, "Cyclone Start Void", "FINISHED", "Activate Slash Void");
			SFCore.Utils.FsmUtil.ChangeTransition(nailArts, "Cyclone Start Void", "BUTTON DOWN", "Hover Start Void");
			SFCore.Utils.FsmUtil.ChangeTransition(nailArts, "Hover Start Void", "FINISHED", "Cyclone Start Void");
			SFCore.Utils.FsmUtil.ChangeTransition(nailArts, "Activate Slash Void", "FINISHED", "Play Audio Void");
			SFCore.Utils.FsmUtil.ChangeTransition(nailArts, "Play Audio Void", "FINISHED", "Cyclone Spin Void");
			SFCore.Utils.FsmUtil.ChangeTransition(nailArts, "Cyclone Spin Void", "BUTTON DOWN", "Cyclone Extend Void");
			SFCore.Utils.FsmUtil.ChangeTransition(nailArts, "Cyclone Spin Void", "END", "Cyclone End Void");
			SFCore.Utils.FsmUtil.ChangeTransition(nailArts, "Cyclone Extend Void", "END", "Cyclone End Void");
			SFCore.Utils.FsmUtil.ChangeTransition(nailArts, "Cyclone Extend Void", "WAIT", "Cyclone Spin Void");
			SFCore.Utils.FsmUtil.ChangeTransition(nailArts, "Left 2", "FINISHED", "Bloom Activated DSlash?");
			SFCore.Utils.FsmUtil.ChangeTransition(nailArts, "Right 2", "FINISHED", "Bloom Activated DSlash?");
			SFCore.Utils.FsmUtil.ChangeTransition(nailArts, "Dash Slash Void", "FINISHED", "DSlash Move End Void");
			SFCore.Utils.FsmUtil.ChangeTransition(nailArts, "DSlash Move End Void", "FINISHED", "D Slash End Void");
			SFCore.Utils.FsmUtil.ChangeTransition(nailArts, "Left", "FINISHED", "Bloom Activated GSlash?");
			SFCore.Utils.FsmUtil.ChangeTransition(nailArts, "Right", "FINISHED", "Bloom Activated GSlash?");
			SFCore.Utils.FsmUtil.ChangeTransition(nailArts, "G Slash Void", "FINISHED", "Stop Move Void");
			SFCore.Utils.FsmUtil.ChangeTransition(nailArts, "Stop Move Void", "FINISHED", "G Slash End Void");
			nailArts.AddTransition("Bloom Activated CSlash?", "VOID", "Cyclone Start Void");
			nailArts.AddTransition("Bloom Activated CSlash?", "NORMAL", "Cyclone Start");
			nailArts.AddTransition("Bloom Activated DSlash?", "VOID", "Dash Slash Void");
			nailArts.AddTransition("Bloom Activated DSlash?", "NORMAL", "Dash Slash");
			nailArts.AddTransition("Bloom Activated GSlash?", "VOID", "G Slash Void");
			nailArts.AddTransition("Bloom Activated GSlash?", "NORMAL", "G Slash");
			nailArts.AddMethod("Bloom Activated CSlash?", delegate
			{
				nailArts.SetState((FiveKnights.Instance.SaveSettings.equippedCharms[3] && _pd.health <= 1) ? "Cyclone Start Void" : "Cyclone Start");
			});
			nailArts.AddMethod("Bloom Activated DSlash?", delegate
			{
				nailArts.SetState((FiveKnights.Instance.SaveSettings.equippedCharms[3] && _pd.health <= 1) ? "Dash Slash Void" : "Dash Slash");
			});
			nailArts.AddMethod("Bloom Activated GSlash?", delegate
			{
				nailArts.SetState((FiveKnights.Instance.SaveSettings.equippedCharms[3] && _pd.health <= 1) ? "G Slash Void" : "G Slash");
			});
			nailArts.GetAction<HutongGames.PlayMaker.Actions.Tk2dPlayAnimationWithEvents>("Cyclone Start Void").clipName = "NA Cyclone Start Void";
			nailArts.GetAction<HutongGames.PlayMaker.Actions.Tk2dPlayAnimation>("Cyclone Spin Void").clipName = "NA Cyclone Void";
			nailArts.GetAction<HutongGames.PlayMaker.Actions.Tk2dPlayAnimation>("Cyclone Extend Void").clipName = "NA Cyclone Void";
			nailArts.GetAction<HutongGames.PlayMaker.Actions.Tk2dPlayAnimationWithEvents>("Cyclone End Void").clipName = "NA Cyclone End Void";
			nailArts.GetAction<HutongGames.PlayMaker.Actions.Tk2dPlayAnimationWithEvents>("Dash Slash Void").clipName = "NA Dash Slash Void";
			nailArts.GetAction<HutongGames.PlayMaker.Actions.Tk2dPlayAnimationWithEvents>("G Slash Void").clipName = "NA Big Slash Void";
			nailArts.InsertMethod("Activate Slash Void", 0, delegate
			{
				cycloneSlashVoid.SetActive(value: true);
				cycloneSlashVoid.GetComponent<tk2dSpriteAnimator>().Play("Cyclone Slash Effect Void");
			});
			nailArts.InsertMethod("Cyclone End Void", 2, delegate
			{
				cycloneSlashVoid.SetActive(value: false);
			});
			nailArts.AddMethod("Cancel All", delegate
			{
				cycloneSlashVoid.SetActive(value: false);
			});
			nailArts.InsertMethod("Dash Slash Void", 0, delegate
			{
				dashSlashVoid.SetActive(value: true);
				dashSlashVoid.GetComponent<tk2dSpriteAnimator>().Play("Dash Slash Effect Void");
			});
			nailArts.InsertMethod("D Slash End Void", 0, delegate
			{
				dashSlashVoid.SetActive(value: false);
			});
			nailArts.AddMethod("Cancel All", delegate
			{
				dashSlashVoid.SetActive(value: false);
			});
			nailArts.InsertMethod("G Slash Void", 0, delegate
			{
				greatSlashVoid.SetActive(value: true);
				greatSlashVoid.GetComponent<tk2dSpriteAnimator>().Play("Great Slash Effect Void");
			});
			nailArts.InsertMethod("G Slash End Void", 0, delegate
			{
				greatSlashVoid.SetActive(value: false);
			});
			nailArts.AddMethod("Cancel All", delegate
			{
				greatSlashVoid.SetActive(value: false);
			});
			nailArts.RemoveAction<HutongGames.PlayMaker.Actions.ActivateGameObject>("Activate Slash Void");
			nailArts.RemoveAction<HutongGames.PlayMaker.Actions.ActivateGameObject>("Dash Slash Void");
			nailArts.RemoveAction<HutongGames.PlayMaker.Actions.ActivateGameObject>("G Slash Void");
			nailArts.Log();
			StartCoroutine(ResetVoidNarts(new GameObject[3] { cycloneSlashVoid, dashSlashVoid, greatSlashVoid }));
		}

		private IEnumerator ResetVoidNarts(GameObject[] narts)
		{
			GameObject[] array = narts;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
			yield return new WaitForEndOfFrame();
			array = narts;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
		}

		private void ModifyFuryForBloom()
		{
			PlayMakerFSM fury = _hc.gameObject.FindGameObjectInChildren("Charm Effects").LocateMyFSM("Fury");
			Log("Fury Color: " + fury.GetAction<HutongGames.PlayMaker.Actions.Tk2dSpriteSetColor>("Activate", 17).color.Value.ToString());
			Color furyColor = fury.GetAction<HutongGames.PlayMaker.Actions.Tk2dSpriteSetColor>("Activate", 18).color.Value;
			fury.InsertMethod("Activate", 17, delegate
			{
				Color value = (FiveKnights.Instance.SaveSettings.equippedCharms[3] ? Color.black : furyColor);
				fury.GetAction<HutongGames.PlayMaker.Actions.Tk2dSpriteSetColor>("Activate", 18).color.Value = value;
				fury.GetAction<HutongGames.PlayMaker.Actions.Tk2dSpriteSetColor>("Activate", 19).color.Value = value;
				fury.GetAction<HutongGames.PlayMaker.Actions.Tk2dSpriteSetColor>("Activate", 20).color.Value = value;
				_hc.GetComponent<AbyssalBloom>().SetFury(fury: true);
			});
			fury.InsertMethod("Stay Furied", 4, delegate
			{
				_hc.GetComponent<AbyssalBloom>().SetFury(fury: true);
			});
			fury.InsertMethod("Deactivate", 21, delegate
			{
				_hc.GetComponent<AbyssalBloom>().SetFury(fury: false);
			});
		}

		private void ModifySpellsForBloom()
		{
			_spellControl.InsertMethod("Wallside?", delegate
			{
				_hc.GetComponent<AbyssalBloom>().CancelTendrilAttack();
				_hc.GetComponent<AbyssalBloom>().CancelVerticalTendrilAttack();
				_hc.GetComponent<AbyssalBloom>().CancelWallTendrilAttack();
			}, 0);
			_spellControl.InsertMethod("Quake Antic", delegate
			{
				_hc.GetComponent<AbyssalBloom>().CancelTendrilAttack();
				_hc.GetComponent<AbyssalBloom>().CancelVerticalTendrilAttack();
				_hc.GetComponent<AbyssalBloom>().CancelWallTendrilAttack();
			}, 0);
			_spellControl.InsertMethod("Level Check 3", delegate
			{
				_hc.GetComponent<AbyssalBloom>().CancelTendrilAttack();
				_hc.GetComponent<AbyssalBloom>().CancelVerticalTendrilAttack();
				_hc.GetComponent<AbyssalBloom>().CancelWallTendrilAttack();
			}, 0);
		}

		private void CharmUpdate(PlayerData playerData, HeroController hc)
		{
			Log("Amulets Charm Update");
			if (!_activated)
			{
				Log("Amulets Start not run yet, exiting Charm Update");
				return;
			}
			_hc.GetComponent<RoyalAura>().enabled = playerData.GetBool("equippedCharm_" + 10) && FiveKnights.Instance.SaveSettings.upgradedCharm_10;
			_hc.GetComponent<Purity>().enabled = FiveKnights.Instance.SaveSettings.equippedCharms[0];
			_hc.GetComponent<LamentControl>().enabled = FiveKnights.Instance.SaveSettings.equippedCharms[1];
			if (FiveKnights.Instance.SaveSettings.equippedCharms[1])
			{
				FsmState[] fsmStates = _spellControl.FsmStates;
				foreach (FsmState fsmState in fsmStates)
				{
					FsmTransition[] transitions = fsmState.Transitions;
					foreach (FsmTransition fsmTransition in transitions)
					{
						if (fsmTransition.ToState == "Focus")
						{
							SFCore.Utils.FsmUtil.ChangeTransition(fsmState, fsmTransition.EventName, "Focus Blast");
						}
						else if (fsmTransition.ToState == "Focus Heal")
						{
							SFCore.Utils.FsmUtil.ChangeTransition(fsmState, fsmTransition.EventName, "Focus Heal Blast");
						}
						else if (fsmTransition.ToState == "Start MP Drain")
						{
							SFCore.Utils.FsmUtil.ChangeTransition(fsmState, fsmTransition.EventName, "Start MP Drain Blast");
						}
						else if (fsmTransition.ToState == "Focus Heal 2")
						{
							SFCore.Utils.FsmUtil.ChangeTransition(fsmState, fsmTransition.EventName, "Focus Heal 2 Blast");
						}
					}
				}
			}
			else
			{
				FsmState[] fsmStates = _spellControl.FsmStates;
				foreach (FsmState fsmState2 in fsmStates)
				{
					FsmTransition[] transitions = fsmState2.Transitions;
					foreach (FsmTransition fsmTransition2 in transitions)
					{
						if (fsmTransition2.ToState == "Focus Blast")
						{
							SFCore.Utils.FsmUtil.ChangeTransition(fsmState2, fsmTransition2.EventName, "Focus");
						}
						else if (fsmTransition2.ToState == "Focus Heal Blast")
						{
							SFCore.Utils.FsmUtil.ChangeTransition(fsmState2, fsmTransition2.EventName, "Focus Heal");
						}
						else if (fsmTransition2.ToState == "Start MP Drain Blast")
						{
							SFCore.Utils.FsmUtil.ChangeTransition(fsmState2, fsmTransition2.EventName, "Start MP Drain");
						}
						else if (fsmTransition2.ToState == "Focus Heal 2 Blast")
						{
							SFCore.Utils.FsmUtil.ChangeTransition(fsmState2, fsmTransition2.EventName, "Focus Heal 2");
						}
					}
				}
			}
			_hc.GetComponent<BoonSpells>().enabled = false;
			_hc.GetComponent<BoonSpells>().enabled = FiveKnights.Instance.SaveSettings.equippedCharms[2];
			_hc.GetComponent<AbyssalBloom>().enabled = FiveKnights.Instance.SaveSettings.equippedCharms[3];
		}

		private IEnumerator PureVesselBlastFadeIn()
		{
			this.PlayAudio((AudioClip)_pvControl.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Focus Charge", 2).audioClip.Value);
			_blastKnight = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Blast"], HeroController.instance.transform);
			_blastKnight.transform.localPosition += Vector3.up * 0.25f;
			_blastKnight.SetActive(value: true);
			UnityEngine.Object.Destroy(_blastKnight.FindGameObjectInChildren("hero_damager"));
			if (_pd.GetBool("equippedCharm_" + 34))
			{
				_blastKnight.transform.localScale *= 2.5f;
			}
			else
			{
				_blastKnight.transform.localScale *= 1.5f;
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
			if (_blastKnight != null)
			{
				Log("Pure Vessel Blast");
				_blastKnight.layer = 17;
				Animator component = _blastKnight.GetComponent<Animator>();
				component.speed = 1f;
				int fullPathHash = component.GetCurrentAnimatorStateInfo(0).fullPathHash;
				component.PlayInFixedTime(fullPathHash, -1, 0.8f);
				Log("Adding CircleCollider2D");
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
				blastCollider.offset = Vector3.down;
				blastCollider.isTrigger = true;
				Log("Adding DebugColliders");
				Log("Adding DamageEnemies");
				_blastKnight.AddComponent<DamageEnemies>();
				DamageEnemies component2 = _blastKnight.GetComponent<DamageEnemies>();
				if (_pd.GetBool("equippedCharm_" + 34))
				{
					component2.damageDealt = 80;
				}
				else
				{
					component2.damageDealt = 40;
				}
				component2.attackType = AttackTypes.Spell;
				component2.ignoreInvuln = false;
				component2.enabled = true;
				Log("Playing AudioClip");
				this.PlayAudio((AudioClip)_pvControl.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Focus Burst", 8).audioClip.Value, 1.5f);
				Log("Audio Clip finished");
				yield return new WaitForSeconds(0.11f);
				blastCollider.enabled = false;
				yield return new WaitForSeconds(0.69f);
				UnityEngine.Object.Destroy(_blastKnight);
			}
		}

		private void CancelBlast()
		{
			if (_blastKnight != null)
			{
				UnityEngine.Object.Destroy(_blastKnight);
			}
		}

		private void OnDestroy()
		{
			Log("Destroyed Amulets");
			On.HeroController.Start -= On_HeroController_Start;
			ModHooks.CharmUpdateHook -= CharmUpdate;
		}

		private static void Log(object message)
		{
			Modding.Logger.Log("[FiveKnights][Amulets] " + message);
		}
	}
	internal class AwardCharms : MonoBehaviour
	{
		private class FadeOut : MonoBehaviour
		{
			private float startTime;

			private float alpha;

			public float fadeTime;

			private void Start()
			{
				startTime = Time.time;
				alpha = base.gameObject.GetComponent<SpriteRenderer>().color.a;
			}

			private void Update()
			{
				if (alpha > 0f)
				{
					alpha = startTime + fadeTime - Time.time;
					base.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, alpha);
				}
			}
		}

		private GameObject _charmGet;

		private AssetBundle _charmUnlock;

		private SaveModSettings _settings = FiveKnights.Instance.SaveSettings;

		private bool pauseShroom;

		public bool[] bossWin = new bool[4];

		public void Awake()
		{
			ModHooks.BeforeSceneLoadHook += SceneCheck;
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged += BloomPlacement;
		}

		private void BloomPlacement(Scene From, Scene To)
		{
			if (To.name == "Abyssal_Temple" && !FiveKnights.Instance.SaveSettings.gotCharms[3])
			{
				_ = FiveKnights.Instance.SaveSettings;
				GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Shiny"]);
				Transform child = gameObject.transform.GetChild(0);
				UnityEngine.Object.Destroy(child.gameObject.GetComponent<PersistentBoolItem>());
				gameObject.SetActive(value: false);
				child.gameObject.SetActive(value: true);
				child.GetComponent<Rigidbody2D>().gravityScale = 0f;
				child.GetComponent<SpriteRenderer>().color = Color.black;
				child.Find("White Wave").GetComponent<WaveEffectControl>().blackWave = true;
				child.Find("White Wave").GetComponent<SpriteRenderer>().color = Color.black;
				child.Find("White Wave").GetComponent<WaveEffectControl>().scaleMultiplier = 0.5f;
				gameObject.transform.position = new Vector3(241.3f, 34.12f, 0.004f);
				PlayMakerFSM playMakerFSM = child.gameObject.LocateMyFSM("Shiny Control");
				FsmVariables fsmVariables = playMakerFSM.FsmVariables;
				playMakerFSM.ChangeFsmTransition("Init", "FINISHED", "Idle");
				playMakerFSM.GetState("Hero Down").GetAction<HutongGames.PlayMaker.Actions.Tk2dPlayAnimation>().clipName = "Collect SD 1";
				playMakerFSM.GetState("Hero Down").GetAction<HutongGames.PlayMaker.Actions.Wait>().time = 0.5f;
				playMakerFSM.GetState("Big Get Flash").RemoveAction<HutongGames.PlayMaker.Actions.Tk2dPlayAnimation>();
				playMakerFSM.GetState("Type").InsertMethod(delegate
				{
					CharmCutscene("bloom");
				}, 0);
				playMakerFSM.ChangeFsmTransition("Type", "QUEEN", "Msg");
				playMakerFSM.GetState("Hero Up").GetAction<HutongGames.PlayMaker.Actions.Tk2dPlayAnimationWithEvents>().clipName = "Collect SD 1 Back";
				playMakerFSM.GetState("Hero Up").RemoveAction<HutongGames.PlayMaker.Actions.CallMethodProper>();
				fsmVariables.FindFsmBool("Activated").Value = false;
				fsmVariables.FindFsmBool("Queen Charm").Value = true;
				gameObject.SetActive(value: true);
			}
		}

		private string SceneCheck(string sceneName)
		{
			SaveModSettings saveSettings = FiveKnights.Instance.SaveSettings;
			string sceneName2 = GameManager.instance.sceneName;
			if ((sceneName2 == "dryya overworld" && !saveSettings.gotCharms[0] && bossWin[0]) || (sceneName2 == "zemer overworld arena" && !saveSettings.gotCharms[1] && bossWin[1]) || (sceneName2 == "hegemol overworld arena" && !saveSettings.gotCharms[2] && bossWin[2]) || (sceneName2 == "isma overworld" && !saveSettings.upgradedCharm_10 && bossWin[3]))
			{
				bossWin[0] = false;
				bossWin[1] = false;
				bossWin[2] = false;
				bossWin[3] = false;
				string[] array = sceneName2.Split(' ');
				StartCoroutine(AwardCharm(array[0]));
			}
			return sceneName;
		}

		private IEnumerator AwardCharm(string boss)
		{
			yield return new WaitUntil(() => HeroController.instance != null);
			SaveModSettings saveSettings = FiveKnights.Instance.SaveSettings;
			switch (boss)
			{
			case "isma":
				PlayerData.instance.gotCharm_10 = true;
				saveSettings.upgradedCharm_10 = true;
				PlayerData.instance.newCharm_10 = true;
				break;
			case "dryya":
				saveSettings.gotCharms[0] = true;
				saveSettings.newCharms[0] = true;
				break;
			case "zemer":
				saveSettings.gotCharms[1] = true;
				saveSettings.newCharms[1] = true;
				break;
			case "hegemol":
				saveSettings.gotCharms[2] = true;
				saveSettings.newCharms[2] = true;
				break;
			}
			yield return new WaitUntil(() => HeroController.instance.GetComponent<tk2dSpriteAnimator>().CurrentClip.name == "Prostrate Rise");
			yield return new WaitUntil(() => HeroController.instance.GetComponent<tk2dSpriteAnimator>().CurrentClip.name != "Prostrate Rise");
			if (!pauseShroom)
			{
				HeroController.instance.IgnoreInput();
				CharmCutscene(boss);
			}
		}

		private void CharmCutscene(string boss)
		{
			pauseShroom = true;
			_charmUnlock = ABManager.AssetBundles[ABManager.Bundle.CharmUnlock];
			_charmGet = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["CharmGet"]);
			string text = null;
			string charmName = null;
			string audioName = null;
			float upDelay = 2.4f;
			int charmNumber = 0;
			switch (boss)
			{
			case "dryya":
				text = "PurityAppear";
				charmName = "CHARM_NAME_PURITY";
				upDelay = 2.4f;
				audioName = "purity_charm_get";
				charmNumber = 0;
				break;
			case "zemer":
				text = "LamentAppear";
				charmName = "CHARM_NAME_LAMENT";
				upDelay = 2.4f;
				audioName = "spell_information_merged";
				charmNumber = 1;
				break;
			case "isma":
				text = "CrestUpgrade";
				charmName = "CHARM_NAME_HONOUR";
				audioName = "kings_honor_get";
				upDelay = 2.8f;
				break;
			case "hegemol":
				text = "BoonAppear";
				charmName = "CHARM_NAME_BOON";
				upDelay = 2.4f;
				audioName = "spell_information_merged";
				charmNumber = 2;
				break;
			case "bloom":
				text = "BloomGrow";
				charmName = "CHARM_NAME_BLOOM";
				audioName = "abyss_bloom";
				upDelay = 2.4f;
				charmNumber = 3;
				break;
			}
			SetupUI(_charmGet, charmName, upDelay);
			HeroController.instance.RelinquishControl();
			_charmGet.SetActive(value: true);
			GameObject gameObject = UnityEngine.Object.Instantiate(_charmUnlock.LoadAsset<GameObject>(text));
			gameObject.SetActive(value: false);
			gameObject.transform.parent = _charmGet.transform;
			gameObject.transform.position = new Vector3(0.19f, 2.5f, -3.1f);
			gameObject.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
			gameObject.GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
			gameObject.GetComponent<SpriteRenderer>().sortingLayerID = 629535577;
			gameObject.AddComponent<FadeOut>().enabled = false;
			gameObject.GetComponent<FadeOut>().fadeTime = 1f;
			gameObject.layer = 5;
			StartCoroutine(AnimCoroutine(gameObject, charmNumber, boss, audioName));
		}

		private void SetupUI(GameObject UI, string charmName, float upDelay)
		{
			UnityEngine.Object.Destroy(USceneUtil.Find(UI, "Single Frag"));
			PlayMakerFSM playMakerFSM = UI.LocateMyFSM("Msg Control");
			UnityEngine.Object.Destroy(USceneUtil.Find(UI, "White Form"));
			playMakerFSM.GetState("Init").ChangeFsmTransition("BLACK", "Text");
			playMakerFSM.GetState("Init").ChangeFsmTransition("KING", "Text");
			playMakerFSM.GetState("Init").ChangeFsmTransition("QUEEN", "Text");
			playMakerFSM.InsertAction("Init", new HutongGames.PlayMaker.Actions.GetLanguageString
			{
				sheetName = "Prompts",
				convName = charmName,
				storeValue = playMakerFSM.FindFsmStringVariable("Game Text")
			}, 0);
			playMakerFSM.InsertAction("Init", new HutongGames.PlayMaker.Actions.SetTextMeshProText
			{
				textString = playMakerFSM.FindFsmStringVariable("Game Text"),
				gameObject = new FsmOwnerDefault
				{
					OwnerOption = OwnerDefaultOption.SpecifyGameObject,
					GameObject = USceneUtil.Find(playMakerFSM.gameObject, "Item Name")
				}
			}, 1);
			playMakerFSM.InsertAction("Init", new HutongGames.PlayMaker.Actions.GetLanguageString
			{
				sheetName = "Prompts",
				convName = "CUSTOM_ITEM_INTROS",
				storeValue = playMakerFSM.FindFsmStringVariable("Game Text")
			}, 2);
			playMakerFSM.InsertAction("Init", new HutongGames.PlayMaker.Actions.SetTextMeshProText
			{
				textString = playMakerFSM.FindFsmStringVariable("Game Text"),
				gameObject = new FsmOwnerDefault
				{
					OwnerOption = OwnerDefaultOption.SpecifyGameObject,
					GameObject = USceneUtil.Find(playMakerFSM.gameObject, "Item Name Prefix")
				}
			}, 3);
			TextContainer component = USceneUtil.Find(_charmGet, "Item Name").GetComponent<TextContainer>();
			component.size = new Vector2(100f, component.size.y);
			USceneUtil.Find(_charmGet, "Item Name").LocateMyFSM("color_fader").FindFsmFloatVariable("Up Delay")
				.Value = upDelay;
			USceneUtil.Find(_charmGet, "Item Name Prefix").LocateMyFSM("color_fader").FindFsmFloatVariable("Up Delay")
				.Value = upDelay;
		}

		private IEnumerator AnimCoroutine(GameObject CharmAnim, int charmNumber, string boss, string audioName)
		{
			switch (boss)
			{
			case "zemer":
			case "hegemol":
				this.PlayAudio(_charmUnlock.LoadAsset<AudioClip>(audioName));
				yield return new WaitForSeconds(1.5f);
				break;
			case "dryya":
				this.PlayAudio(_charmUnlock.LoadAsset<AudioClip>("spell_information_screen"));
				yield return new WaitForSeconds(1.5f);
				this.PlayAudio(_charmUnlock.LoadAsset<AudioClip>(audioName));
				break;
			case "isma":
				yield return new WaitForSeconds(1.5f);
				CharmAnim.SetActive(value: true);
				CharmAnim.GetComponent<SpriteRenderer>().enabled = true;
				this.PlayAudio(_charmUnlock.LoadAsset<AudioClip>("new_heartpiece_puzzle_bit"));
				yield return new WaitForSeconds(0.5f);
				this.PlayAudio(_charmUnlock.LoadAsset<AudioClip>(audioName), 1.5f);
				break;
			case "bloom":
				yield return new WaitForSeconds(1.5f);
				this.PlayAudio(_charmUnlock.LoadAsset<AudioClip>(audioName));
				break;
			}
			CharmAnim.SetActive(value: true);
			CharmAnim.GetComponent<SpriteRenderer>().enabled = true;
			yield return new WaitUntil(() => _charmGet.LocateMyFSM("Msg Control").GetState("Down").GetAction<HutongGames.PlayMaker.Actions.SendEventByName>()
				.Finished);
			CharmAnim.GetComponent<FadeOut>().enabled = true;
			yield return new WaitForSeconds(1.2f);
			if (boss == "isma")
			{
				PlayerData.instance.gotCharm_10 = true;
				FiveKnights.Instance.SaveSettings.upgradedCharm_10 = true;
				PlayerData.instance.newCharm_10 = true;
			}
			else
			{
				FiveKnights.Instance.SaveSettings.gotCharms[charmNumber] = true;
				FiveKnights.Instance.SaveSettings.newCharms[charmNumber] = true;
			}
			HeroController.instance.AcceptInput();
			HeroController.instance.RegainControl();
			pauseShroom = false;
		}

		private void OnDestroy()
		{
			Log("Destroyed AwardCharms");
			ModHooks.BeforeSceneLoadHook -= SceneCheck;
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= BloomPlacement;
		}

		private static void Log(object message)
		{
			Modding.Logger.Log("[FiveKnights][AwardCharms] " + message);
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

		private void OnEnable()
		{
			_spellControl = _hc.spellControl;
			PlayMakerFSM fsm = _spellControl.GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>("Fireball 2", 3).gameObject.Value.LocateMyFSM("Fireball Cast");
			_audioPlayer = fsm.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Cast Right", 3).audioPlayer.Value;
			PlayMakerFSM fsm2 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["PV"].LocateMyFSM("Control"), _hc.transform);
			if (!FiveKnights.preloadedGO.ContainsKey("Plume"))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(fsm2.GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>("Plume Gen", 0).gameObject.Value);
				gameObject.SetActive(value: false);
				gameObject.layer = 17;
				gameObject.tag = "Hero Spell";
				UnityEngine.Object.Destroy(gameObject.GetComponent<DamageHero>());
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				FiveKnights.preloadedGO["Plume"] = gameObject;
			}
			if (!FiveKnights.preloadedGO.ContainsKey("BoonDagger"))
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(fsm2.GetAction<HutongGames.PlayMaker.Actions.FlingObjectsFromGlobalPoolTime>("SmallShot LowHigh").gameObject.Value);
				gameObject2.SetActive(value: false);
				gameObject2.layer = 17;
				gameObject2.tag = "Hero Spell";
				UnityEngine.Object.Destroy(gameObject2.GetComponent<DamageHero>());
				UnityEngine.Object.Destroy(gameObject2.LocateMyFSM("Control"));
				gameObject2.FindGameObjectInChildren("Dribble L").layer = 9;
				gameObject2.FindGameObjectInChildren("Glow").layer = 9;
				gameObject2.FindGameObjectInChildren("Beam").layer = 9;
				UnityEngine.Object.DontDestroyOnLoad(gameObject2);
				FiveKnights.preloadedGO["BoonDagger"] = gameObject2;
			}
			FiveKnights.Clips["Burst"] = (AudioClip)fsm2.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Focus Burst", 8).audioClip.Value;
			FiveKnights.Clips["Plume Up"] = (AudioClip)fsm2.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Plume Up", 1).audioClip.Value;
			ModifySpellFSM(enabled: true);
		}

		private void OnDisable()
		{
			ModifySpellFSM(enabled: false);
		}

		private void ModifySpellFSM(bool enabled)
		{
			if (enabled)
			{
				SFCore.Utils.FsmUtil.ChangeTransition(_spellControl, "Level Check 3", "LEVEL 1", "Scream Antic1 Blasts");
				SFCore.Utils.FsmUtil.ChangeTransition(_spellControl, "Level Check 3", "LEVEL 2", "Scream Antic2 Blasts");
				SFCore.Utils.FsmUtil.ChangeTransition(_spellControl, "Quake1 Down", "HERO LANDED", "Q1 Land Plumes");
				SFCore.Utils.FsmUtil.ChangeTransition(_spellControl, "Quake2 Down", "HERO LANDED", "Q2 Land Plumes");
				if (!PlayerData.instance.GetBool("equippedCharm_11"))
				{
					SFCore.Utils.FsmUtil.ChangeTransition(_spellControl, "Level Check", "LEVEL 1", "Fireball 1 SmallShots");
					SFCore.Utils.FsmUtil.ChangeTransition(_spellControl, "Level Check", "LEVEL 2", "Fireball 2 SmallShots");
				}
			}
			else
			{
				SFCore.Utils.FsmUtil.ChangeTransition(_spellControl, "Level Check 3", "LEVEL 1", "Scream Antic1");
				SFCore.Utils.FsmUtil.ChangeTransition(_spellControl, "Level Check 3", "LEVEL 2", "Scream Antic2");
				SFCore.Utils.FsmUtil.ChangeTransition(_spellControl, "Quake1 Down", "HERO LANDED", "Quake1 Land");
				SFCore.Utils.FsmUtil.ChangeTransition(_spellControl, "Quake2 Down", "HERO LANDED", "Q2 Land");
				SFCore.Utils.FsmUtil.ChangeTransition(_spellControl, "Level Check", "LEVEL 1", "Fireball 1");
				SFCore.Utils.FsmUtil.ChangeTransition(_spellControl, "Level Check", "LEVEL 2", "Fireball 2");
			}
		}

		public void CastDaggers(bool upgraded)
		{
			bool equippedCharm_ = PlayerData.instance.equippedCharm_19;
			int num = (equippedCharm_ ? (-30) : (-25));
			int num2 = (equippedCharm_ ? 30 : 25);
			int num3 = (equippedCharm_ ? 20 : 25);
			for (int i = num; i <= num2; i += num3)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["BoonDagger"], HeroController.instance.transform.position, Quaternion.identity);
				gameObject.SetActive(value: false);
				if (i != num)
				{
					UnityEngine.Object.Destroy(gameObject.GetComponent<AudioSource>());
				}
				Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
				component.isKinematic = true;
				float x = 50f * Mathf.Cos((float)Math.PI / 180f * (float)i) * (0f - HeroController.instance.transform.localScale.x);
				float y = 50f * Mathf.Sin((float)Math.PI / 180f * (float)i);
				gameObject.SetActive(value: true);
				component.velocity = new Vector2(x, y);
				gameObject.AddComponent<Dagger>().upgraded = upgraded;
				UnityEngine.Object.Destroy(gameObject, 5f);
			}
		}

		public void CastPlumes(bool upgraded)
		{
			for (float num = 2f; num <= 10f; num += 2f)
			{
				Vector2 vector = HeroController.instance.transform.position;
				float y = vector.y - 1.8f;
				GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Plume"], new Vector2(vector.x - num, y), Quaternion.identity);
				obj.SetActive(value: true);
				obj.AddComponent<Plume>().upgraded = upgraded;
				GameObject obj2 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Plume"], new Vector2(vector.x + num, y), Quaternion.identity);
				obj2.SetActive(value: true);
				obj2.AddComponent<Plume>().upgraded = upgraded;
			}
			PlayAudio("Plume Up", 1.5f, 1.5f, 0.5f, 0.25f);
		}

		public void CastBlasts(bool upgraded)
		{
			List<GameObject> blasts = new List<GameObject>();
			StartCoroutine(CastBlastsCoro());
			StartCoroutine(DisableColliderCoro());
			StartCoroutine(DestroyBlastsCoro());
			IEnumerator CastBlastsCoro()
			{
				blasts.Add(SpawnBlast(HeroController.instance.transform.position + Vector3.up * 4f, upgraded));
				yield return new WaitForSeconds(0.2f);
				for (int i = 0; i < ((!upgraded) ? 1 : 3); i++)
				{
					blasts.Add(SpawnBlast(HeroController.instance.transform.position + Vector3.up * UnityEngine.Random.Range(4, 10) + Vector3.right * UnityEngine.Random.Range(-3, 3), upgraded));
					yield return new WaitForSeconds(0.2f);
				}
			}
			IEnumerator DestroyBlastsCoro()
			{
				yield return new WaitForSeconds(0.5f);
				for (int i = 0; i < blasts.Count; i++)
				{
					UnityEngine.Object.Destroy(blasts[i]);
					yield return new WaitForSeconds(0.2f);
				}
			}
			IEnumerator DisableColliderCoro()
			{
				yield return new WaitForSeconds(0.15f);
				for (int i = 0; i < blasts.Count; i++)
				{
					UnityEngine.Object.Destroy(blasts[i].GetComponent<CircleCollider2D>());
					yield return new WaitForSeconds(0.2f);
				}
			}
		}

		private GameObject SpawnBlast(Vector3 pos, bool upgraded)
		{
			GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Blast"], pos, Quaternion.identity);
			obj.layer = 17;
			obj.tag = "Hero Spell";
			obj.SetActive(value: true);
			UnityEngine.Object.Destroy(obj.FindGameObjectInChildren("hero_damager"));
			Animator component = obj.GetComponent<Animator>();
			int fullPathHash = component.GetCurrentAnimatorStateInfo(0).fullPathHash;
			component.PlayInFixedTime(fullPathHash, -1, 0.75f);
			CircleCollider2D circleCollider2D = obj.AddComponent<CircleCollider2D>();
			circleCollider2D.offset = Vector2.down;
			circleCollider2D.radius = 3.5f;
			circleCollider2D.isTrigger = true;
			DamageEnemies damageEnemies = obj.AddComponent<DamageEnemies>();
			damageEnemies.damageDealt = (PlayerData.instance.equippedCharm_19 ? 45 : 35);
			damageEnemies.attackType = AttackTypes.Spell;
			damageEnemies.ignoreInvuln = false;
			damageEnemies.enabled = true;
			PlayAudio("Burst", 1.2f, 1.5f, 0.5f);
			return obj;
		}

		private void PlayAudio(string clip, float minPitch = 1f, float maxPitch = 1f, float volume = 1f, float delay = 0f)
		{
			GameManager.instance.StartCoroutine(Play());
			IEnumerator Play()
			{
				AudioClip audioClip = FiveKnights.Clips[clip];
				yield return new WaitForSeconds(delay);
				GameObject audioPlayerInstance = _audioPlayer.Spawn(base.transform.position, Quaternion.identity);
				AudioSource component = audioPlayerInstance.GetComponent<AudioSource>();
				component.outputAudioMixerGroup = HeroController.instance.GetComponent<AudioSource>().outputAudioMixerGroup;
				component.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
				component.volume = volume;
				component.PlayOneShot(audioClip);
				yield return new WaitForSeconds(audioClip.length + 3f);
				UnityEngine.Object.Destroy(audioPlayerInstance);
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
		private AbyssalBloom _bloom;

		private HeroController _hc => HeroController.instance;

		private void Awake()
		{
			_bloom = _hc.GetComponent<AbyssalBloom>();
			ModHooks.CharmUpdateHook += CharmUpdateHook;
			On.HeroController.TakeDamage += HeroControllerTakeDamage;
			On.HeroController.AddHealth += HeroControllerAddHealth;
			On.HeroController.MaxHealth += HeroControllerMaxHealth;
		}

		private void CharmUpdateHook(PlayerData data, HeroController hc)
		{
			CheckStage();
		}

		private void HeroControllerTakeDamage(On.HeroController.orig_TakeDamage orig, HeroController self, GameObject go, CollisionSide damageSide, int damageAmount, int hazardType)
		{
			int health = PlayerData.instance.health;
			orig(self, go, damageSide, damageAmount, hazardType);
			if (PlayerData.instance.health != health)
			{
				CheckStage();
			}
		}

		private void HeroControllerAddHealth(On.HeroController.orig_AddHealth orig, HeroController self, int amount)
		{
			orig(self, amount);
			CheckStage();
		}

		private void HeroControllerMaxHealth(On.HeroController.orig_MaxHealth orig, HeroController self)
		{
			orig(self);
			CheckStage();
		}

		private void CheckStage()
		{
			if (!FiveKnights.Instance.SaveSettings.equippedCharms[3])
			{
				_bloom.SetLevel(0);
			}
			else if (_hc.playerData.health > (_hc.playerData.maxHealth + 1) / 2)
			{
				_bloom.SetLevel(0);
			}
			else if (_hc.playerData.health > 1 && _hc.playerData.health <= (_hc.playerData.maxHealth + 1) / 2)
			{
				Log("On level 1");
				_bloom.SetLevel(1);
			}
			else if (_hc.playerData.health <= 1)
			{
				Log("On level 2");
				_bloom.SetLevel(2);
			}
		}

		private void OnDestroy()
		{
			ModHooks.CharmUpdateHook -= CharmUpdateHook;
			On.HeroController.TakeDamage -= HeroControllerTakeDamage;
			On.HeroController.AddHealth -= HeroControllerAddHealth;
			On.HeroController.MaxHealth -= HeroControllerMaxHealth;
		}

		private void Log(object o)
		{
			Modding.Logger.Log("[FiveKnights][CheckBloomStage] " + o);
		}
	}
	public class Dagger : MonoBehaviour
	{
		private const int DaggerDamage = 13;

		private const int DaggerDamageUpgraded = 25;

		public bool upgraded;

		private void OnTriggerEnter2D(Collider2D collider)
		{
			int layer = collider.gameObject.layer;
			if (layer != 20 && layer != 9 && layer != 26 && layer != 31 && !collider.CompareTag("Geo"))
			{
				HitTaker.Hit(damageInstance: new HitInstance
				{
					DamageDealt = (upgraded ? 25 : 13),
					AttackType = AttackTypes.Spell,
					IgnoreInvulnerable = true,
					Source = base.gameObject,
					Multiplier = 1f
				}, targetGameObject: collider.gameObject);
			}
		}

		private void Log(object message)
		{
			Modding.Logger.Log("[Dagger] " + message);
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
			On.HealthManager.TakeDamage += ApplyStatus;
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ClearList;
			_pvControl = USceneUtil.Find(_hc.gameObject, "HK Prime(Clone)(Clone)").LocateMyFSM("Control");
			_spellControl = HeroController.instance.spellControl;
			if (_spellControl != null)
			{
				_spellControl.InsertMethod("Cancel All", 33, BlastControlCancel);
				_spellControl.InsertMethod("Focus Cancel", 15, BlastControlCancel);
				_spellControl.InsertMethod("Focus Cancel 2", 18, BlastControlCancel);
				_spellControl.InsertMethod("Focus Blast", 15, BlastControlFadeIn);
				_spellControl.InsertMethod("Start MP Drain Blast", 2, BlastControlFadeIn);
				_spellControl.InsertMethod("Focus Heal Blast", 16, BlastControlMain);
				_spellControl.InsertMethod("Focus Heal 2 Blast", 18, BlastControlMain);
			}
			ObjectPool.StartupPool[] startupPools = ObjectPool.instance.startupPools;
			foreach (ObjectPool.StartupPool startupPool in startupPools)
			{
				if (startupPool.prefab.name == "Knight Spore Cloud")
				{
					FiveKnights.preloadedGO["SporeCloud"] = startupPool.prefab;
				}
				if (startupPool.prefab.name == "Knight Dung Cloud")
				{
					FiveKnights.preloadedGO["DungCloud"] = startupPool.prefab;
				}
			}
		}

		private void BlastControlCancel()
		{
			PureVesselBlastCancel();
		}

		private void PureVesselBlastCancel()
		{
			foreach (GameObject markedEnemy in markedEnemies)
			{
				Log("Called CancelBlast");
				int index = markedEnemies.IndexOf(markedEnemy);
				try
				{
					if (markedEnemy.GetComponent<Afflicted>()._focusLines != null)
					{
						Log("Attempting to stop focus lines");
						markedEnemy.GetComponent<Afflicted>()._focusLines.GetComponent<tk2dSpriteAnimator>().Stop();
						Log("Animation was stopped");
						UnityEngine.Object.Destroy(markedEnemy.GetComponent<Afflicted>()._focusLines);
						Log("The object was deleted");
					}
				}
				catch (ArgumentOutOfRangeException)
				{
				}
				try
				{
					if (markedEnemies[index] != null)
					{
						if (markedEnemies[index].GetComponent<Afflicted>() == null)
						{
							Log("Afficted component not found");
						}
						else
						{
							try
							{
								markedEnemy.GetComponent<Afflicted>().visible = true;
								markedEnemy.GetComponent<Afflicted>().SoulEffect.SetActive(value: true);
								markedEnemy.GetComponent<Afflicted>().SoulEffect.GetComponent<ParticleSystem>().Play();
								Log("Reactivated Soul Effeect");
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
					markedEnemy.GetComponent<Afflicted>().StopCoroutine(markedEnemy.GetComponent<Afflicted>()._createLine);
					Log("Stopped Line Coroutine");
				}
				catch (NullReferenceException)
				{
					Log("Couldn't stop create line couroutine");
				}
				try
				{
					UnityEngine.Object.Destroy(markedEnemy.GetComponent<Afflicted>()._blast);
					Log("Removed blast");
				}
				catch (ArgumentOutOfRangeException)
				{
					Log("Exception caught in blast");
				}
				Log("Removed Blast Object");
			}
		}

		private void BlastControlFadeIn()
		{
			for (int i = 0; i < markedEnemies.Count; i++)
			{
				GameObject gameObject = markedEnemies[i];
				for (int j = 0; j < markedEnemies.Count; j++)
				{
					GameObject gameObject2 = markedEnemies[j];
					if (i != j && !(gameObject2 != gameObject))
					{
						Log("Removed Duplicate Object");
						Log($"{gameObject2} was in the list twice");
						markedEnemies.Remove(gameObject2);
					}
				}
				Log("Start coroutine: FadeIn");
				Log("Enemy index: " + markedEnemies.IndexOf(gameObject));
				if (gameObject == null || !gameObject.active)
				{
					markedEnemies.RemoveAt(i);
					Log("Removed null or inactive entity");
					i--;
				}
				else
				{
					gameObject.GetComponent<Afflicted>().StartCoroutine(gameObject.GetComponent<Afflicted>().PureVesselBlastFadeIn());
				}
			}
		}

		private void BlastControlMain()
		{
			List<int> list = new List<int>();
			foreach (GameObject markedEnemy in markedEnemies)
			{
				Log("Start Coroutine: Blast");
				int item = markedEnemies.IndexOf(markedEnemy);
				if (markedEnemy == null || !markedEnemy.active)
				{
					list.Add(item);
					Log("Item was null, continuing");
				}
				else
				{
					StartCoroutine(markedEnemy.GetComponent<Afflicted>().PureVesselBlast());
				}
			}
			foreach (int item2 in list)
			{
				markedEnemies.RemoveAt(item2);
			}
		}

		private void OnDisable()
		{
			On.HealthManager.TakeDamage -= ApplyStatus;
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= ClearList;
			if (_spellControl != null)
			{
				_spellControl.RemoveAction("Cancel All", 33);
				_spellControl.RemoveAction("Focus Cancel", 15);
				_spellControl.RemoveAction("Focus Cancel 2", 18);
				_spellControl.RemoveAction("Focus Blast", 15);
				_spellControl.RemoveAction("Start MP Drain Blast", 2);
				_spellControl.RemoveAction("Focus Heal Blast", 16);
				_spellControl.RemoveAction("Focus Heal 2 Blast", 18);
			}
		}

		private void ClearList(Scene PrevScene, Scene NextScene)
		{
			markedEnemies.Clear();
			foreach (GameObject markedEnemy in markedEnemies)
			{
				Log($"This should be empty but {markedEnemy} is still there");
			}
			Log("Cleared list");
		}

		private void ApplyStatus(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
		{
			orig(self, hitInstance);
			if (hitInstance.AttackType != AttackTypes.Nail || !(self.gameObject.GetComponent<Afflicted>() == null))
			{
				return;
			}
			self.gameObject.AddComponent<Afflicted>();
			markedEnemies.Add(self.gameObject);
			foreach (GameObject markedEnemy in markedEnemies)
			{
				Log(markedEnemy?.ToString() + " in list");
			}
		}

		private void Log(object message)
		{
			Modding.Logger.Log("[FiveKnights][LamentControl] " + message);
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
			_pvControl = USceneUtil.Find(_hc.gameObject, "HK Prime(Clone)(Clone)").LocateMyFSM("Control");
			SoulEffect = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["SoulEffect"], base.gameObject.transform);
			SoulEffect.transform.localPosition = new Vector3(0f, 0f, -0.0001f);
			Vector2 vector = base.gameObject.transform.position;
			if (base.gameObject.GetComponent<SpriteRenderer>() != null)
			{
				vector = base.gameObject.GetComponent<SpriteRenderer>().bounds.center;
			}
			if (base.gameObject.GetComponent<tk2dSprite>() != null)
			{
				vector = base.gameObject.gameObject.GetComponent<tk2dSprite>().GetBounds().center + base.gameObject.transform.position;
			}
			SoulEffect.transform.position = vector;
			SoulEffect.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
			SoulEffect.GetComponent<ParticleSystem>().startSize = 0.7f;
			SoulEffect.GetComponent<ParticleSystem>().startLifetime = 0.3f;
			SoulEffect.GetComponent<ParticleSystem>().startColor = new Color(1f, 1f, 1f, 0.5f);
			SoulEffect.SetActive(value: true);
		}

		private void Update()
		{
			if (SoulEffect != null)
			{
				Vector3 vector = base.gameObject.transform.position;
				if (base.gameObject.GetComponent<SpriteRenderer>() != null)
				{
					vector = base.gameObject.GetComponent<SpriteRenderer>().bounds.center;
				}
				if (base.gameObject.GetComponent<tk2dSprite>() != null)
				{
					vector = base.gameObject.GetComponent<tk2dSprite>().GetBounds().center + base.gameObject.transform.position;
				}
				SoulEffect.transform.position = new Vector3(vector.x, vector.y, base.gameObject.transform.position.z + -0.0001f);
				if (visible)
				{
					if (base.gameObject.GetComponent<SpriteRenderer>() != null)
					{
						SoulEffect.SetActive(base.gameObject.GetComponent<SpriteRenderer>().isVisible);
					}
					if (base.gameObject.GetComponent<MeshRenderer>() != null)
					{
						SoulEffect.SetActive(base.gameObject.GetComponent<MeshRenderer>().isVisible);
					}
				}
			}
			else
			{
				SoulEffect = new GameObject();
				Start();
			}
			if (_focusLines != null && _pd.GetBool("equippedCharm_" + 28))
			{
				_focusLines.transform.position = base.gameObject.transform.position;
			}
			if (_blast != null && _pd.GetBool("equippedCharm_" + 28))
			{
				_blast.transform.position = base.gameObject.transform.position;
			}
		}

		public IEnumerator FadeOut()
		{
			SoulEffect.GetComponent<ParticleSystem>().Stop();
			yield return new WaitUntil(() => SoulEffect.GetComponent<ParticleSystem>().isStopped);
			visible = false;
			SoulEffect.SetActive(value: false);
		}

		private void OnDisable()
		{
			UnityEngine.Object.Destroy(SoulEffect);
		}

		public IEnumerator PureVesselBlastFadeIn()
		{
			Log("Called PureVesselBlastFadeIn");
			Log("Recieved GO: " + base.gameObject.name);
			StartCoroutine(FadeOut());
			_createLine = CreateLine();
			StartCoroutine(_createLine);
			_focusLines = UnityEngine.Object.Instantiate(USceneUtil.Find(USceneUtil.Find(_hc.gameObject, "Focus Effects"), "Lines Anim"), base.gameObject.transform.position, Quaternion.identity);
			_focusLines.GetComponent<tk2dSpriteAnimator>().Play("Focus Effect");
			this.PlayAudio((AudioClip)_pvControl.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Focus Charge", 2).audioClip.Value, 0f, 1.5f);
			_blast = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Blast"]);
			_blast.transform.position += base.gameObject.transform.position;
			_blast.SetActive(value: true);
			UnityEngine.Object.Destroy(_blast.FindGameObjectInChildren("hero_damager"));
			if (_pd.GetBool("equippedCharm_" + 34))
			{
				_blast.transform.localScale *= 2.5f;
			}
			else
			{
				_blast.transform.localScale *= 1.5f;
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
			Log("Fade in finished");
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
			yield return new WaitForSeconds(num - 0.2f);
			Vector3 vector = _hc.transform.position - new Vector3(0f, 1f, 0f);
			Vector3 position = base.gameObject.transform.position;
			Vector3 position2 = Vector3.Lerp(vector, position, 0.5f);
			float y = vector.y - position.y;
			float x = vector.x - position.x;
			float num2;
			for (num2 = Mathf.Atan2(y, x) * (180f / (float)Math.PI); num2 < 0f; num2 += 360f)
			{
			}
			Log(num2);
			float x2 = Vector2.Distance(vector, position);
			_line = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["SoulTwister"].LocateMyFSM("Mage").GetAction<HutongGames.PlayMaker.Actions.CreateObject>("Tele Line").gameObject.Value, position2, new Quaternion(0f, 0f, 0f, 0f));
			_line.transform.SetRotationZ(num2);
			_line.transform.localScale = new Vector3(x2, 1f, 1f);
			_line.GetComponent<ParticleSystem>().startSize = 0.35f;
			_line.GetComponent<ParticleSystem>().emissionRate = 3000f;
			_line.GetComponent<ParticleSystem>().startLifetime = 0.75f;
			_line.GetComponent<ParticleSystem>().Emit(0);
			_line.SetActive(value: true);
			_line.GetComponent<ParticleSystem>().Play();
			_line.GetComponent<ParticleSystem>().loop = false;
		}

		public IEnumerator PureVesselBlast()
		{
			Log("Called PureVesselBlast");
			Log("Recieved GO: " + base.gameObject.name);
			_focusLines.GetComponent<tk2dSpriteAnimator>().Play("Focus Effect End");
			_blast.layer = 17;
			Animator component = _blast.GetComponent<Animator>();
			component.speed = 1f;
			int fullPathHash = component.GetCurrentAnimatorStateInfo(0).fullPathHash;
			component.PlayInFixedTime(fullPathHash, -1, 0.8f);
			Log("Adding CircleCollider2D");
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
			blastCollider.offset = Vector3.down;
			blastCollider.isTrigger = true;
			Log("Adding DebugColliders");
			Log("Adding DamageEnemies");
			_blast.AddComponent<DamageEnemies>();
			DamageEnemies component2 = _blast.GetComponent<DamageEnemies>();
			component2.damageDealt = (_pd.GetBool("equippedCharm_" + 34) ? 60 : 30);
			component2.attackType = AttackTypes.Spell;
			component2.ignoreInvuln = false;
			component2.enabled = true;
			Log("Playing AudioClip");
			this.PlayAudio((AudioClip)_pvControl.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Focus Burst", 8).audioClip.Value, 0f, 1.5f);
			Log("Audio Clip finished");
			if (_pd.GetBool("equippedCharm_" + 17))
			{
				if (_pd.GetBool("equippedCharm_" + 10))
				{
					if (FiveKnights.Instance.SaveSettings.upgradedCharm_10)
					{
						UnityEngine.Object.Instantiate(_hc.GetComponent<RoyalAura>().dungCloud, base.transform.position, Quaternion.identity).SetActive(value: true);
					}
					else
					{
						UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["DungCloud"], base.transform.position, Quaternion.identity).SetActive(value: true);
					}
				}
				else
				{
					UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["SporeCloud"], base.transform.position, Quaternion.identity).SetActive(value: true);
				}
			}
			yield return new WaitForSeconds(0.11f);
			blastCollider.enabled = false;
			yield return new WaitForSeconds(0.69f);
			UnityEngine.Object.Destroy(_blast);
			UnityEngine.Object.Destroy(_focusLines);
			Log("Blast Finished");
			try
			{
				LamentControl.markedEnemies.RemoveAt(LamentControl.markedEnemies.IndexOf(base.gameObject));
				UnityEngine.Object.Destroy(base.gameObject.GetComponent<Afflicted>());
			}
			catch (NullReferenceException)
			{
			}
		}

		private void Log(object message)
		{
			Modding.Logger.Log("[FiveKnights][LamentControl] " + message);
		}
	}
	public class ModifyAuraColor : MonoBehaviour
	{
		private readonly Color DefaultColor = new Color(0.6706f, 0.4275f, 0f, 1f);

		private readonly Color PaleColor = new Color(0.8f, 0.8f, 0.8f);

		private ParticleSystem ps;

		private void Update()
		{
			if (ps == null)
			{
				ps = base.gameObject.GetComponent<ParticleSystem>();
			}
			ParticleSystem.MainModule main = ps.main;
			main.startColor = (FiveKnights.Instance.SaveSettings.upgradedCharm_10 ? PaleColor : DefaultColor);
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
			base.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
			if (!IsGrounded())
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			_fsm = base.gameObject.LocateMyFSM("FSM");
			_fsm.GetAction<HutongGames.PlayMaker.Actions.Wait>("Antic", 2).time.Value = 0.25f;
			_fsm.GetAction<HutongGames.PlayMaker.Actions.Wait>("End", 0).time.Value = 0.5f;
			_fsm.InsertCoroutine("Plume 2", 0, AnimControl);
			_fsm.GetAction<HutongGames.PlayMaker.Actions.FloatCompare>("Outside Arena?", 2).float2.Value = float.PositiveInfinity;
			_fsm.GetAction<HutongGames.PlayMaker.Actions.FloatCompare>("Outside Arena?", 3).float2.Value = float.NegativeInfinity;
		}

		private void Start()
		{
			DamageEnemies damageEnemies = base.gameObject.AddComponent<DamageEnemies>();
			damageEnemies.ignoreInvuln = false;
			damageEnemies.attackType = AttackTypes.Spell;
			damageEnemies.direction = 2f;
			damageEnemies.damageDealt = (PlayerData.instance.equippedCharm_19 ? 11 : 7);
		}

		private IEnumerator AnimControl()
		{
			yield return new WaitForSeconds(0.1f);
			base.gameObject.GetComponent<tk2dSpriteAnimator>().enabled = false;
			yield return new WaitForSeconds(upgraded ? 0.7f : 0.4f);
			base.gameObject.GetComponent<tk2dSpriteAnimator>().enabled = true;
		}

		private bool IsGrounded()
		{
			float distance = 1.01f;
			return Physics2D.Raycast((Vector2)base.transform.position + Vector2.up, Vector2.down, distance, 256);
		}

		private void Log(object message)
		{
			Modding.Logger.Log("[FiveKnights][Plume] " + message);
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
			nailSlashes = new List<NailSlash>
			{
				HeroController.instance.normalSlash,
				HeroController.instance.alternateSlash,
				HeroController.instance.downSlash,
				HeroController.instance.upSlash,
				HeroController.instance.wallSlash
			};
			On.HealthManager.TakeDamage += IncrementSpeed;
			ModHooks.CharmUpdateHook += Duration;
			On.HeroController.CanDoubleJump += FixDoubleJump;
			On.HeroController.CanCast += FixCast;
			ModHooks.AfterTakeDamageHook += ResetSpeed;
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged += Dummy;
			On.HeroController.CanNailCharge += CancelNailArts;
			_hc.ATTACK_COOLDOWN_TIME = 0.49f;
			_hc.ATTACK_COOLDOWN_TIME_CH = 0.31f;
			_hc.ATTACK_DURATION = 0.44f;
			_hc.ATTACK_DURATION_CH = 0.25f;
		}

		private void OnDisable()
		{
			On.HealthManager.TakeDamage -= IncrementSpeed;
			ModHooks.CharmUpdateHook -= Duration;
			On.HeroController.CanDoubleJump -= FixDoubleJump;
			On.HeroController.CanCast -= FixCast;
			ModHooks.AfterTakeDamageHook -= ResetSpeed;
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= Dummy;
			On.HeroController.CanNailCharge -= CancelNailArts;
			_hc.ATTACK_COOLDOWN_TIME = 0.41f;
			_hc.ATTACK_COOLDOWN_TIME_CH = 0.25f;
			_hc.ATTACK_DURATION = 0.35f;
			_hc.ATTACK_DURATION_CH = 0.28f;
		}

		private void Update()
		{
			if (InputHandler.Instance.inputActions.attack.IsPressed && ReflectionHelper.CallMethod<HeroController, bool>(_hc, "CanAttack", Array.Empty<object>()))
			{
				ReflectionHelper.CallMethod(_hc, "DoAttack");
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
				Log("Play Reset Audio");
				this.PlayAudio(ABManager.AssetBundles[ABManager.Bundle.CharmUnlock].LoadAsset<AudioClip>("purity_reset"));
			}
			_hc.ATTACK_COOLDOWN_TIME = 0.49f;
			_hc.ATTACK_COOLDOWN_TIME_CH = 0.31f;
			_hc.ATTACK_DURATION = 0.44f;
			_hc.ATTACK_DURATION_CH = 0.25f;
			foreach (NailSlash nailSlash in nailSlashes)
			{
				if (nailSlash.GetComponent<tk2dSprite>().color != Color.black)
				{
					nailSlash.GetComponent<tk2dSprite>().color = Color.white;
					nailSlash.GetComponent<AudioSource>().pitch = 1f;
				}
			}
			audioMax = false;
			timer = 0f;
		}

		private int ResetSpeed(int hazardType, int damageAmount)
		{
			timerRunning = false;
			if ((!_pd.equippedCharm_32 && _hc.ATTACK_COOLDOWN_TIME <= 0.48f) || (_pd.equippedCharm_32 && _hc.ATTACK_COOLDOWN_TIME_CH <= 0.3f))
			{
				Log("Play Reset Audio");
				this.PlayAudio(ABManager.AssetBundles[ABManager.Bundle.CharmUnlock].LoadAsset<AudioClip>("purity_reset"));
			}
			_hc.ATTACK_COOLDOWN_TIME = 0.49f;
			_hc.ATTACK_COOLDOWN_TIME_CH = 0.31f;
			_hc.ATTACK_DURATION = 0.44f;
			_hc.ATTACK_DURATION_CH = 0.25f;
			foreach (NailSlash nailSlash in nailSlashes)
			{
				if (nailSlash.GetComponent<tk2dSprite>().color != Color.black)
				{
					nailSlash.GetComponent<tk2dSprite>().color = Color.white;
					nailSlash.GetComponent<AudioSource>().pitch = 1f;
				}
			}
			audioMax = false;
			timer = 0f;
			return damageAmount;
		}

		private void IncrementSpeed(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
		{
			if (hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				hitInstance.Multiplier *= 0.8f;
			}
			orig(self, hitInstance);
			if (hitInstance.AttackType != AttackTypes.Nail && hitInstance.AttackType != AttackTypes.NailBeam)
			{
				return;
			}
			timer = 0f;
			timerRunning = true;
			if (hitInstance.AttackType == AttackTypes.Nail)
			{
				_hc.ATTACK_COOLDOWN_TIME -= 0.035555556f;
				_hc.ATTACK_COOLDOWN_TIME_CH -= 0.016363638f;
				_hc.ATTACK_DURATION -= 0.035555556f;
				_hc.ATTACK_DURATION_CH -= 0.016363638f;
			}
			if (_hc.ATTACK_COOLDOWN_TIME_CH <= 0.17f)
			{
				ReflectionHelper.SetField(self, "evasionByHitRemaining", 0.1f);
			}
			if ((!_pd.equippedCharm_32 && _hc.ATTACK_COOLDOWN_TIME <= 0.18f && !audioMax) || (_pd.equippedCharm_32 && _hc.ATTACK_COOLDOWN_TIME_CH <= 0.14f && !audioMax))
			{
				audioMax = true;
				Log("Play Audio");
				this.PlayAudio(ABManager.AssetBundles[ABManager.Bundle.CharmUnlock].LoadAsset<AudioClip>("purity_max"), 0.5f);
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
				if (nailSlash.GetComponent<tk2dSprite>().color != Color.black)
				{
					nailSlash.GetComponent<tk2dSprite>().color = new Color(0.619f, 0.798f, 0.881f);
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
			if (To.name == "Deepnest_East_16")
			{
				UnityExtensions.FindGameObject(To, "Training Dummy").LocateMyFSM("Hit").GetState("Light Dir")
					.InsertMethod(delegate
					{
						IncrementSpeedDummy();
					}, 0);
			}
		}

		private void IncrementSpeedDummy()
		{
			timer = 0f;
			timerRunning = true;
			_hc.ATTACK_COOLDOWN_TIME -= 0.035555556f;
			_hc.ATTACK_COOLDOWN_TIME_CH -= 0.016363638f;
			_hc.ATTACK_DURATION -= 0.035555556f;
			_hc.ATTACK_DURATION_CH -= 0.016363638f;
			if ((!_pd.equippedCharm_32 && _hc.ATTACK_COOLDOWN_TIME <= 0.18f && !audioMax) || (_pd.equippedCharm_32 && _hc.ATTACK_COOLDOWN_TIME_CH <= 0.14f && !audioMax))
			{
				audioMax = true;
				Log("Play Audio");
				this.PlayAudio(ABManager.AssetBundles[ABManager.Bundle.CharmUnlock].LoadAsset<AudioClip>("purity_max"), 0.5f);
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
				if (nailSlash.GetComponent<tk2dSprite>().color != Color.black)
				{
					nailSlash.GetComponent<tk2dSprite>().color = new Color(0.619f, 0.798f, 0.881f);
				}
			}
		}

		private bool CancelNailArts(On.HeroController.orig_CanNailCharge orig, HeroController self)
		{
			return false;
		}

		private bool FixCast(On.HeroController.orig_CanCast orig, HeroController self)
		{
			if (!GameManager.instance.isPaused && !self.cState.dashing && self.hero_state != ActorStates.no_input && !self.cState.backDashing && !self.cState.recoiling && !self.cState.recoilFrozen && !self.cState.transitioning && !self.cState.hazardDeath && !self.cState.hazardRespawning && self.CanInput())
			{
				return ReflectionHelper.GetField<HeroController, float>(self, "preventCastByDialogueEndTimer") <= 0f;
			}
			return false;
		}

		private bool FixDoubleJump(On.HeroController.orig_CanDoubleJump orig, HeroController self)
		{
			if (self.playerData.GetBool("hasDoubleJump") && !self.controlReqlinquished && !ReflectionHelper.GetField<HeroController, bool>(self, "doubleJumped") && !self.inAcid && self.hero_state != ActorStates.no_input && self.hero_state != ActorStates.hard_landing && self.hero_state != ActorStates.dash_landing && !self.cState.dashing && !self.cState.wallSliding && !self.cState.backDashing && !self.cState.bouncing && !self.cState.shroomBouncing)
			{
				return !self.cState.onGround;
			}
			return false;
		}

		private void CancelMantis(On.NailSlash.orig_SetMantis orig, NailSlash self, bool set)
		{
			orig(self, set: false);
		}

		private void CancelLongnail(On.NailSlash.orig_SetLongnail orig, NailSlash self, bool set)
		{
			orig(self, set: false);
		}

		private void Log(object message)
		{
			Modding.Logger.Log("[FiveKnights][Purity Timer] " + message);
		}
	}
	public class RoyalAura : MonoBehaviour
	{
		public GameObject dungCloud;

		private GameObject _dungTrail;

		private PlayMakerFSM _dungControl;

		private PlayMakerFSM _dungTrailControl;

		private PlayMakerFSM _spellControl;

		private readonly Color PaleColor = new Color(0.65f, 0.65f, 0.65f, 0.75f);

		private float _frequency = 0.5f;

		private int _dungDamage = 2;

		private float _timer;

		private void OnEnable()
		{
			GameObject gameObject = HeroController.instance.transform.Find("Charm Effects").Find("Dung").gameObject;
			_dungControl = gameObject.LocateMyFSM("Control");
			ObjectPool.StartupPool[] startupPools = ObjectPool.instance.startupPools;
			foreach (ObjectPool.StartupPool startupPool in startupPools)
			{
				if (startupPool.prefab.name == "Knight Dung Trail")
				{
					_dungTrail = UnityEngine.Object.Instantiate(startupPool.prefab);
					_dungTrail.SetActive(value: false);
					UnityEngine.Object.DontDestroyOnLoad(_dungTrail);
				}
				if (startupPool.prefab.name == "Knight Dung Cloud")
				{
					dungCloud = UnityEngine.Object.Instantiate(startupPool.prefab);
					dungCloud.SetActive(value: false);
					UnityEngine.Object.DontDestroyOnLoad(dungCloud);
				}
			}
			_spellControl = HeroController.instance.spellControl;
			if (USceneUtil.Find(gameObject, "Particle 1").GetComponent<ModifyAuraColor>() == null)
			{
				USceneUtil.Find(gameObject, "Particle 1").AddComponent<ModifyAuraColor>();
			}
			_dungTrailControl = _dungTrail.LocateMyFSM("Control");
			ParticleSystem.MainModule main = _dungTrailControl.Fsm.GetFsmGameObject("Pt Normal").Value.GetComponent<ParticleSystem>().main;
			main.startColor = PaleColor;
			ParticleSystem[] componentsInChildren = dungCloud.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				ParticleSystem.MainModule main2 = componentsInChildren[i].main;
				main2.startColor = PaleColor;
			}
			tk2dSprite[] componentsInChildren2 = dungCloud.GetComponentsInChildren<tk2dSprite>(includeInactive: true);
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				componentsInChildren2[i].color = Color.white;
			}
			_dungControl.GetAction<HutongGames.PlayMaker.Actions.Wait>("Emit Pause", 2).time.Value = 0.1f;
			_dungControl.GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPoolOverTime>("Equipped", 0).Enabled = false;
			string[] array = new string[2] { "Dung Cloud", "Dung Cloud 2" };
			foreach (string stateName in array)
			{
				if (_spellControl == null)
				{
					break;
				}
				_spellControl.GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>(stateName, 0).Enabled = false;
				_spellControl.InsertMethod(stateName, delegate
				{
					UnityEngine.Object.Instantiate(dungCloud, base.transform.position - new Vector3(0f, 0f, -0.001f), Quaternion.identity).SetActive(value: true);
					ParticleSystem.MainModule main3 = HeroController.instance.transform.Find("Charm Effects").Find("Dung Recharge").gameObject.GetComponent<ParticleSystem>().main;
					main3.startColor = Color.white;
				}, 1);
			}
			On.ExtraDamageable.RecieveExtraDamage += ExtraDamageableRecieveExtraDamage;
			On.ExtraDamageable.GetDamageOfType += ExtraDamageableGetDamageOfType;
			On.SpriteFlash.flashDungQuick += SpriteFlashFlashDungQuick;
			On.KnightHatchling.OnEnable += KnightHatchlingOnEnable;
			On.HutongGames.PlayMaker.Actions.ActivateAllChildren.OnEnter += ActivateAllChildrenOnEnter;
			On.HutongGames.PlayMaker.Actions.Tk2dPlayAnimation.OnEnter += Tk2dPlayAnimationOnEnter;
		}

		private void ExtraDamageableRecieveExtraDamage(On.ExtraDamageable.orig_RecieveExtraDamage orig, ExtraDamageable self, ExtraDamageTypes extraDamageType)
		{
			if ((extraDamageType == ExtraDamageTypes.Dung || extraDamageType == ExtraDamageTypes.Dung2) && !self.gameObject.GetComponent<RoyalAuraSpread>())
			{
				self.gameObject.AddComponent<RoyalAuraSpread>();
			}
			orig(self, extraDamageType);
		}

		private int ExtraDamageableGetDamageOfType(On.ExtraDamageable.orig_GetDamageOfType orig, ExtraDamageTypes extraDamageTypes)
		{
			if (extraDamageTypes == ExtraDamageTypes.Dung || extraDamageTypes == ExtraDamageTypes.Dung2)
			{
				return _dungDamage;
			}
			return orig(extraDamageTypes);
		}

		private void SpriteFlashFlashDungQuick(On.SpriteFlash.orig_flashDungQuick orig, SpriteFlash self)
		{
			self.flashArmoured();
		}

		private void KnightHatchlingOnEnable(On.KnightHatchling.orig_OnEnable orig, KnightHatchling self)
		{
			orig(self);
			KnightHatchling.TypeDetails normalDetails = self.normalDetails;
			normalDetails.damage = self.dungDetails.damage;
			normalDetails.dung = true;
			normalDetails.spatterColor = Color.white;
			Mirror.SetField(self, "details", normalDetails);
			ParticleSystem.MainModule main = self.dungPt.main;
			main.startColor = PaleColor;
		}

		private void ActivateAllChildrenOnEnter(On.HutongGames.PlayMaker.Actions.ActivateAllChildren.orig_OnEnter orig, HutongGames.PlayMaker.Actions.ActivateAllChildren self)
		{
			if (self.Fsm.GameObjectName.Contains("Dung Explosion") && self.State.Name == "Explode")
			{
				UnityEngine.Object.Instantiate(dungCloud, self.Fsm.GameObject.transform.position, Quaternion.identity).SetActive(value: true);
				Component[] componentsInChildren = self.Fsm.GameObject.GetComponentsInChildren<Component>(includeInactive: true);
				foreach (Component component in componentsInChildren)
				{
					if (!component.gameObject.name.Contains("Impact"))
					{
						UnityEngine.Object.Destroy(component.gameObject);
					}
					else
					{
						component.gameObject.GetComponent<tk2dSprite>().color = Color.white;
					}
				}
			}
			orig(self);
		}

		private void Tk2dPlayAnimationOnEnter(On.HutongGames.PlayMaker.Actions.Tk2dPlayAnimation.orig_OnEnter orig, HutongGames.PlayMaker.Actions.Tk2dPlayAnimation self)
		{
			if (self.Fsm.GameObjectName.Contains("Spell Fluke Dung") && self.State.Name == "Init" && self.clipName.Value == "Dung Air")
			{
				PlayMakerFSM fsmComponent = self.Fsm.FsmComponent;
				fsmComponent.gameObject.GetComponent<tk2dSprite>().color = Color.white;
				ParticleSystem.MainModule main = fsmComponent.GetFsmGameObjectVariable("Pt Antic").Value.GetComponent<ParticleSystem>().main;
				main.startColor = PaleColor;
				GameObject value = fsmComponent.GetFsmGameObjectVariable("Dung Cloud").Value;
				ParticleSystem[] componentsInChildren = value.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					ParticleSystem.MainModule main2 = componentsInChildren[i].main;
					main2.startColor = PaleColor;
				}
				tk2dSprite[] componentsInChildren2 = value.GetComponentsInChildren<tk2dSprite>(includeInactive: true);
				for (int i = 0; i < componentsInChildren2.Length; i++)
				{
					componentsInChildren2[i].color = Color.white;
				}
			}
			orig(self);
		}

		private void Update()
		{
			_timer += Time.deltaTime;
			if (_timer > _frequency)
			{
				_timer = 0f;
				GameObject obj = UnityEngine.Object.Instantiate(_dungTrail, HeroController.instance.transform.position, Quaternion.identity);
				obj.transform.localScale *= 2f;
				obj.transform.SetPositionZ(0.01f);
				obj.SetActive(value: true);
			}
		}

		private void OnDisable()
		{
			UnityEngine.Object.Destroy(_dungTrail);
			UnityEngine.Object.Destroy(dungCloud);
			_dungControl.GetAction<HutongGames.PlayMaker.Actions.Wait>("Emit Pause", 2).time.Value = 0.5f;
			_dungControl.GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPoolOverTime>("Equipped", 0).Enabled = true;
			string[] array = new string[2] { "Dung Cloud", "Dung Cloud 2" };
			foreach (string stateName in array)
			{
				if (_spellControl == null)
				{
					break;
				}
				_spellControl.RemoveAction(stateName, 1);
				_spellControl.GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>(stateName, 0).Enabled = true;
			}
			On.ExtraDamageable.RecieveExtraDamage -= ExtraDamageableRecieveExtraDamage;
			On.ExtraDamageable.GetDamageOfType -= ExtraDamageableGetDamageOfType;
			On.SpriteFlash.flashDungQuick -= SpriteFlashFlashDungQuick;
			On.KnightHatchling.OnEnable -= KnightHatchlingOnEnable;
			On.HutongGames.PlayMaker.Actions.ActivateAllChildren.OnEnter -= ActivateAllChildrenOnEnter;
			On.HutongGames.PlayMaker.Actions.Tk2dPlayAnimation.OnEnter -= Tk2dPlayAnimationOnEnter;
		}

		private void Log(object message)
		{
			Modding.Logger.Log("[FiveKnights][Royal Aura] " + message);
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
			ObjectPool.StartupPool[] startupPools = ObjectPool.instance.startupPools;
			foreach (ObjectPool.StartupPool startupPool in startupPools)
			{
				if (startupPool.prefab.name == "Knight Dung Trail")
				{
					_dungTrail = UnityEngine.Object.Instantiate(startupPool.prefab);
					_dungTrail.SetActive(value: false);
					UnityEngine.Object.DontDestroyOnLoad(_dungTrail);
					break;
				}
			}
			_dungTrailControl = _dungTrail.LocateMyFSM("Control");
			_dungPt = _dungTrailControl.Fsm.GetFsmGameObject("Pt Normal").Value.GetComponent<ParticleSystem>();
			ParticleSystem.MainModule main = _dungPt.main;
			main.startColor = new Color(0.65f, 0.65f, 0.65f, 0.75f);
			StartCoroutine(StartCooldown());
		}

		private IEnumerator StartCooldown()
		{
			for (int i = 0; i < _repeat; i++)
			{
				HealthManager component = base.gameObject.GetComponent<HealthManager>();
				if (component.hp <= 0 || component.isDead)
				{
					break;
				}
				GameObject obj = UnityEngine.Object.Instantiate(_dungTrail, base.gameObject.transform.position, Quaternion.identity);
				obj.transform.localScale *= 2f;
				obj.SetActive(value: true);
				yield return new WaitForSeconds(_cooldown);
				yield return new WaitUntil(() => base.gameObject.activeSelf);
			}
			yield return new WaitForSeconds(1f);
			UnityEngine.Object.Destroy(this);
		}

		private void OnDestroy()
		{
			UnityEngine.Object.Destroy(_dungTrail);
			StopAllCoroutines();
		}

		private void Log(object message)
		{
			Modding.Logger.Log("[FiveKnights][Royal Aura Spread] " + message);
		}
	}
	public class ShadeSlash : MonoBehaviour
	{
		public AttackDirection attackDirection;

		private HeroController _hc => HeroController.instance;

		private void OnTriggerEnter2D(Collider2D collider)
		{
			if (!(collider != null))
			{
				return;
			}
			switch (attackDirection)
			{
			case AttackDirection.normal:
				if (!_hc.cState.facingRight)
				{
					if (collider.gameObject.layer == 11 && (collider.gameObject.GetComponent<NonBouncer>() == null || !collider.gameObject.GetComponent<NonBouncer>().active))
					{
						if (collider.gameObject.GetComponent<BounceShroom>() != null)
						{
							_hc.RecoilRightLong();
							Bounce(collider, useEffects: false);
						}
						else
						{
							_hc.RecoilRight();
						}
					}
					if (collider.gameObject.layer == 19 && collider.gameObject.GetComponent<BounceShroom>() != null)
					{
						_hc.RecoilRightLong();
						Bounce(collider, useEffects: false);
					}
					break;
				}
				if (collider.gameObject.layer == 11 && (collider.gameObject.GetComponent<NonBouncer>() == null || !collider.gameObject.GetComponent<NonBouncer>().active))
				{
					if (collider.gameObject.GetComponent<BounceShroom>() != null)
					{
						_hc.RecoilLeftLong();
						Bounce(collider, useEffects: false);
					}
					else
					{
						_hc.RecoilLeft();
					}
				}
				if (collider.gameObject.layer == 19 && collider.gameObject.GetComponent<BounceShroom>() != null)
				{
					_hc.RecoilLeftLong();
					Bounce(collider, useEffects: false);
				}
				break;
			case AttackDirection.upward:
				if (collider.gameObject.layer == 11 && (collider.gameObject.GetComponent<NonBouncer>() == null || !collider.gameObject.GetComponent<NonBouncer>().active))
				{
					if (collider.gameObject.GetComponent<BounceShroom>() != null)
					{
						_hc.RecoilDown();
						Bounce(collider, useEffects: false);
					}
					else
					{
						_hc.RecoilDown();
					}
				}
				if (collider.gameObject.layer == 19 && collider.gameObject.GetComponent<BounceShroom>() != null)
				{
					_hc.RecoilDown();
					Bounce(collider, useEffects: false);
				}
				break;
			case AttackDirection.downward:
			{
				PhysLayers layer = (PhysLayers)collider.gameObject.layer;
				if ((layer == PhysLayers.ENEMIES || layer == PhysLayers.INTERACTIVE_OBJECT || layer == PhysLayers.HERO_ATTACK) && (collider.gameObject.GetComponent<NonBouncer>() == null || !collider.gameObject.GetComponent<NonBouncer>().active))
				{
					if (collider.gameObject.GetComponent<BigBouncer>() != null)
					{
						_hc.BounceHigh();
					}
					else if (collider.gameObject.GetComponent<BounceShroom>() != null)
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
			PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(otherCollider.gameObject, "Bounce Shroom");
			if ((bool)playMakerFSM)
			{
				playMakerFSM.SendEvent("BOUNCE UPWARD");
				return;
			}
			BounceShroom component = otherCollider.GetComponent<BounceShroom>();
			if ((bool)component)
			{
				component.BounceLarge(useEffects);
			}
		}

		private void Log(object o)
		{
			Modding.Logger.Log("[Shade Slash] " + o);
		}
	}
	[UsedImplicitly]
	public class FiveKnights : FullSettingsMod<SaveModSettings, GlobalModSettings>, IMenuMod, IMod, Modding.ILogger, ITogglableMod
	{
		private int paleCourtLogoId = -1;

		public static bool isDebug = true;

		public static Dictionary<string, GameObject> preloadedGO = new Dictionary<string, GameObject>();

		public static readonly Dictionary<string, Sprite> SPRITES = new Dictionary<string, Sprite>();

		public static FiveKnights Instance;

		public List<int> charmIDs;

		public static Dictionary<string, JournalHelper> journalEntries = new Dictionary<string, JournalHelper>();

		public static readonly string[] CharmKeys = new string[5] { "PURITY", "LAMENT", "BOON", "BLOOM", "HONOUR" };

		public static readonly int[] CharmCosts = new int[4] { 3, 2, 4, 5 };

		private GameObject valueAddCustomDashEffectsHookOnce;

		private GameObject valueAddCustomHardLandEffectsHookOnce;

		private GameObject valueAddCustomJumpEffectsHookOnce;

		private GameObject valueAddCustomSoftLandEffectsHookOnce;

		private GameObject valueAddCustomRunEffectsHookOnce;

		public static Dictionary<string, AudioClip> Clips { get; } = new Dictionary<string, AudioClip>();

		public static Dictionary<string, AnimationClip> AnimClips { get; } = new Dictionary<string, AnimationClip>();

		public static Dictionary<string, Material> Materials { get; } = new Dictionary<string, Material>();

		private LanguageCtrl langStrings { get; set; }

		public static string OS
		{
			get
			{
				if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows)
				{
					return "win";
				}
				return "win";
			}
		}

		public bool ToggleButtonInsideMenu => true;

		public FiveKnights()
			: base("苍白宫廷")
		{
			langStrings = new LanguageCtrl();
			int num = 0;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string[] manifestResourceNames = executingAssembly.GetManifestResourceNames();
			foreach (string text in manifestResourceNames)
			{
				Log("looking at file " + text);
				using Stream stream = executingAssembly.GetManifestResourceStream(text);
				if (stream != null && text.EndsWith(".png"))
				{
					byte[] array = new byte[stream.Length];
					stream.Read(array, 0, array.Length);
					stream.Dispose();
					Texture2D texture2D = new Texture2D(1, 1);
					texture2D.LoadImage(array, markNonReadable: true);
					string text2 = text.Split('.')[2];
					float pixelsPerUnit = 100f;
					if (new List<string>
					{
						"DlcList", "PaleCourtDlcIcon", "LogoBlack", "journal_dryya", "journal_icon_dryya", "journal_hegemol", "journal_icon_hegemol", "journal_isma", "journal_icon_isma", "journal_zemer",
						"journal_icon_zemer", "journal_tiso", "journal_icon_tiso"
					}.Contains(text2))
					{
						pixelsPerUnit = 64f;
					}
					SPRITES.Add(text2, Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), pixelsPerUnit));
					Log("Created sprite from embedded image: " + text2 + " at ind " + ++num);
				}
			}
			LoadTitleScreen();
			On.SetVersionNumber.Start += ChangeVersionNumber;
			MenuStyleHelper.AddMenuStyleHook = (MenuStyleHelper.Hook)Delegate.Combine(MenuStyleHelper.AddMenuStyleHook, new MenuStyleHelper.Hook(AddPCMenuStyle));
			paleCourtLogoId = TitleLogoHelper.AddLogo(SPRITES["LogoBlack"]);
			DlcIconHelper.AddDlcIcon(SPRITES["PaleCourtDlcIcon"]);
			EnviromentParticleHelper.AddCustomDashEffectsHook = (EnviromentParticleHelper.CustomDashEffectsHook)Delegate.Combine(EnviromentParticleHelper.AddCustomDashEffectsHook, new EnviromentParticleHelper.CustomDashEffectsHook(AddCustomDashEffectsHook));
			EnviromentParticleHelper.AddCustomHardLandEffectsHook = (EnviromentParticleHelper.CustomHardLandEffectsHook)Delegate.Combine(EnviromentParticleHelper.AddCustomHardLandEffectsHook, new EnviromentParticleHelper.CustomHardLandEffectsHook(AddCustomHardLandEffectsHook));
			EnviromentParticleHelper.AddCustomJumpEffectsHook = (EnviromentParticleHelper.CustomJumpEffectsHook)Delegate.Combine(EnviromentParticleHelper.AddCustomJumpEffectsHook, new EnviromentParticleHelper.CustomJumpEffectsHook(AddCustomJumpEffectsHook));
			EnviromentParticleHelper.AddCustomSoftLandEffectsHook = (EnviromentParticleHelper.CustomSoftLandEffectsHook)Delegate.Combine(EnviromentParticleHelper.AddCustomSoftLandEffectsHook, new EnviromentParticleHelper.CustomSoftLandEffectsHook(AddCustomSoftLandEffectsHook));
			EnviromentParticleHelper.AddCustomRunEffectsHook = (EnviromentParticleHelper.CustomRunEffectsHook)Delegate.Combine(EnviromentParticleHelper.AddCustomRunEffectsHook, new EnviromentParticleHelper.CustomRunEffectsHook(AddCustomRunEffectsHook));
			AchievementHelper.AddAchievement("PALE_COURT_ISMA_ACH", SPRITES["ach_isma"], "ISMA_ACH_TITLE", "ISMA_ACH_DESC", hidden: false);
			AchievementHelper.AddAchievement("PALE_COURT_DRYYA_ACH", SPRITES["ach_dryya"], "DRYYA_ACH_TITLE", "DRYYA_ACH_DESC", hidden: false);
			AchievementHelper.AddAchievement("PALE_COURT_HEG_ACH", SPRITES["ach_heg"], "HEG_ACH_TITLE", "HEG_ACH_DESC", hidden: false);
			AchievementHelper.AddAchievement("PALE_COURT_ZEM_ACH", SPRITES["ach_zem"], "ZEM_ACH_TITLE", "ZEM_ACH_DESC", hidden: false);
			AchievementHelper.AddAchievement("PALE_COURT_PANTH_ACH", SPRITES["ach_panth"], "PANTH_ACH_TITLE", "PANTH_ACH_DESC", hidden: true);
			charmIDs = CharmHelper.AddSprites(SPRITES["Mark_of_Purity"], SPRITES["Vessels_Lament"], SPRITES["Boon_of_Hallownest"], SPRITES["Abyssal_Bloom"]);
			ModHooks.BeforeSavegameSaveHook += SaveEntries;
			ModHooks.SetPlayerVariableHook += SetVariableHook;
			ModHooks.GetPlayerVariableHook += GetVariableHook;
			ModHooks.AfterSavegameLoadHook += AfterSavegameLoadHookHandler;
			On.GameManager.StartNewGame += GameManager_StartNewGame;
			ModHooks.GetPlayerBoolHook += ModHooks_GetPlayerBool;
			ModHooks.SetPlayerBoolHook += ModHooks_SetPlayerBool;
			ModHooks.GetPlayerIntHook += ModHooks_GetPlayerInt;
			ModHooks.SetPlayerStringHook += ModHooks_SetPlayerString;
			ModHooks.GetPlayerStringHook += ModHooks_GetPlayerString;
			On.Language.Language.DoSwitch += SwitchLanguage;
			ModHooks.LanguageGetHook += LangGet;
			On.AudioManager.ApplyMusicCue += LoadPaleCourtMenuMusic;
			RewardRoom.Hook();
			Credits.Hook();
			AbyssalTemple.Hook();
			WhiteLadyDialogue.Hook();
			LoadDep();
			LoadBossBundles();
			LoadCharms();
		}

		private void SwitchLanguage(On.Language.Language.orig_DoSwitch orig, LanguageCode newLang)
		{
			orig(newLang);
			foreach (KeyValuePair<string, JournalHelper> journalEntry in journalEntries)
			{
				string key = journalEntry.Key;
				string text = "ENTRY_" + ((key.Length == 4) ? "ISMA" : key.Substring(0, 3).ToUpper());
				JournalHelper value = journalEntry.Value;
				if (value.playerData.killsremaining != 0)
				{
					value.playerData.killsremaining = 1;
				}
				value.playerData.Hidden = true;
				if (langStrings.ContainsKey(text + "_LONGNAME", "Journal"))
				{
					value.nameStrings.name = langStrings.Get(text + "_LONGNAME", "Journal");
				}
				if (langStrings.ContainsKey(text + "_DESC", "Journal"))
				{
					value.nameStrings.desc = langStrings.Get(text + "_DESC", "Journal");
				}
				if (langStrings.ContainsKey(text + "_NOTE", "Journal"))
				{
					value.nameStrings.note = langStrings.Get(text + "_NOTE", "Journal");
				}
				if (langStrings.ContainsKey(text + "_NAME", "Journal"))
				{
					value.nameStrings.shortname = langStrings.Get(text + "_NAME", "Journal");
				}
			}
		}

		public override string GetVersion()
		{
			return "1.1.1.7";
		}

		public override List<(string, string)> GetPreloadNames()
		{
			return new List<(string, string)>
			{
				("GG_Hive_Knight", "Battle Scene/Hive Knight/Slash 1"),
				("GG_Hollow_Knight", "Battle Scene/HK Prime"),
				("GG_Hollow_Knight", "Battle Scene/HK Prime/Counter Flash"),
				("GG_Hollow_Knight", "Battle Scene/Focus Blasts/HK Prime Blast/Blast"),
				("Abyss_05", "Dusk Knight/Dream Enter 2"),
				("Abyss_05", "Dusk Knight/Idle Pt"),
				("Abyss_05", "Dream Dialogue"),
				("GG_Failed_Champion", "False Knight Dream"),
				("GG_Failed_Champion", "Ceiling Dust"),
				("White_Palace_09", "White King Corpse/Throne Sit"),
				("Fungus1_12", "Plant Turret"),
				("Fungus1_12", "simple_grass"),
				("Fungus1_12", "green_grass_2"),
				("Fungus1_12", "green_grass_3"),
				("Fungus1_12", "green_grass_1 (1)"),
				("Fungus1_19", "Plant Trap"),
				("White_Palace_01", "WhiteBench"),
				("White_Palace_01", "White_ Spikes"),
				("GG_Workshop", "GG_Statue_ElderHu"),
				("GG_Lost_Kin", "Lost Kin"),
				("GG_Soul_Tyrant", "Dream Mage Lord"),
				("GG_Workshop", "GG_Statue_TraitorLord"),
				("Room_Mansion", "Heart Piece Folder/Heart Piece/Plink"),
				("Fungus3_23_boss", "Battle Scene/Wave 3/Mantis Traitor Lord"),
				("GG_White_Defender", "Boss Scene Controller"),
				("GG_Atrium_Roof", "Land of Storms Doors"),
				("GG_White_Defender", "GG_Arena_Prefab/Godseeker Crowd"),
				("Dream_04_White_Defender", "_SceneManager"),
				("Dream_04_White_Defender", "Battle Gate (1)"),
				("Dream_04_White_Defender", "Dream Entry"),
				("Dream_04_White_Defender", "White Defender"),
				("Dream_04_White_Defender", "Dream Fall Catcher"),
				("Dream_Final_Boss", "Boss Control/Radiance/Death/Knight Split/Knight Ball"),
				("Dream_Final_Boss", "Boss Control/Radiance"),
				("GG_Nosk", "Mimic Spider"),
				("GG_Hornet_1", "Boss Holder/Hornet Boss 1"),
				("Fungus3_13", "Thorn Collider"),
				("Abyss_05", "Dusk Knight/Shield"),
				("Mines_31", "crystal_barrel_02/mines_cryst_barrel_short/Pt Crystal (1)"),
				("Room_Queen", "UI Msg Get WhiteCharm"),
				("Room_Queen", "Queen Item"),
				("White_Palace_03_hub", "dream_nail_base"),
				("White_Palace_03_hub", "dream_beam_animation"),
				("White_Palace_03_hub", "doorWarp"),
				("Dream_Room_Believer_Shrine", "Plaque_statue_01 (1)"),
				("Room_Mansion", "Heart Piece Folder/Heart Piece"),
				("Room_Mansion", "Xun NPC/White Flash"),
				("GG_Radiance", "Boss Control/Plat Sets/Hazard Plat/Radiant Plat Small (1)"),
				("GG_Atrium", "gg_roof_door_pieces"),
				("Tutorial_01", "_Props/Tut_tablet_top/Glows"),
				("Ruins1_23", "Mage"),
				("Fungus2_03", "Mushroom Turret (2)"),
				("Crossroads_46", "Tram Main"),
				("Deepnest_East_11", "Breakable Wall top"),
				("Deepnest_38", "Collapser Small"),
				("Abyss_10", "higher_being/shadow_gate"),
				("Abyss_10", "Ruins Fossil"),
				("Abyss_10", "Tute Pole 2"),
				("Abyss_10", "hanging_cords_01"),
				("Abyss_10", "_SceneManager"),
				("Abyss_09", "abyss_black-water/abyss_water_top (3)"),
				("Abyss_10", "Surface Water Region"),
				("Abyss_10", "Darkness Region"),
				("Waterways_05", "Dream Gate Set Lock"),
				("Abyss_10", "Hollow_Shade Marker"),
				("Mines_28", "Soul Totem 5"),
				("Mines_28", "Mini_totems_0000_7")
			};
		}

		public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
		{
			Log("Storing GOs");
			preloadedGO["Tram"] = preloadedObjects["Crossroads_46"]["Tram Main"];
			preloadedGO["Heart"] = preloadedObjects["Room_Mansion"]["Heart Piece Folder/Heart Piece"];
			preloadedGO["WhiteFlashZem"] = preloadedObjects["Room_Mansion"]["Xun NPC/White Flash"];
			preloadedGO["RadPlat"] = preloadedObjects["GG_Radiance"]["Boss Control/Plat Sets/Hazard Plat/Radiant Plat Small (1)"];
			preloadedGO["ObjRaise"] = preloadedObjects["GG_Atrium"]["gg_roof_door_pieces"];
			preloadedGO["BombTurret"] = preloadedObjects["Fungus2_03"]["Mushroom Turret (2)"];
			preloadedGO["HornetSphere"] = preloadedObjects["GG_Hornet_1"]["Boss Holder/Hornet Boss 1"];
			preloadedGO["Nosk"] = preloadedObjects["GG_Nosk"]["Mimic Spider"];
			preloadedGO["Thorn Collider"] = preloadedObjects["Fungus3_13"]["Thorn Collider"];
			preloadedGO["Shield"] = preloadedObjects["Abyss_05"]["Dusk Knight/Shield"];
			preloadedGO["Statue"] = preloadedObjects["GG_Workshop"]["GG_Statue_ElderHu"];
			preloadedGO["DPortal"] = preloadedObjects["Abyss_05"]["Dusk Knight/Dream Enter 2"];
			preloadedGO["DPortal2"] = preloadedObjects["Abyss_05"]["Dusk Knight/Idle Pt"];
			preloadedGO["Dream Dialogue"] = preloadedObjects["Abyss_05"]["Dream Dialogue"];
			preloadedGO["StatueMed"] = preloadedObjects["GG_Workshop"]["GG_Statue_TraitorLord"];
			preloadedGO["Bench"] = preloadedObjects["White_Palace_01"]["WhiteBench"];
			preloadedGO["TinkEff"] = preloadedObjects["White_Palace_01"]["White_ Spikes"];
			preloadedGO["Slash"] = preloadedObjects["GG_Hive_Knight"]["Battle Scene/Hive Knight/Slash 1"];
			preloadedGO["PV"] = preloadedObjects["GG_Hollow_Knight"]["Battle Scene/HK Prime"];
			preloadedGO["CounterFX"] = preloadedObjects["GG_Hollow_Knight"]["Battle Scene/HK Prime/Counter Flash"];
			preloadedGO["Blast"] = preloadedObjects["GG_Hollow_Knight"]["Battle Scene/Focus Blasts/HK Prime Blast/Blast"];
			preloadedGO["Kin"] = preloadedObjects["GG_Lost_Kin"]["Lost Kin"];
			preloadedGO["Mage"] = preloadedObjects["GG_Soul_Tyrant"]["Dream Mage Lord"];
			preloadedGO["fk"] = preloadedObjects["GG_Failed_Champion"]["False Knight Dream"];
			preloadedGO["Ceiling Dust"] = preloadedObjects["GG_Failed_Champion"]["Ceiling Dust"];
			preloadedGO["Crystal Pt"] = preloadedObjects["Mines_31"]["crystal_barrel_02/mines_cryst_barrel_short/Pt Crystal (1)"];
			preloadedGO["throne"] = preloadedObjects["White_Palace_09"]["White King Corpse/Throne Sit"];
			preloadedGO["PTurret"] = preloadedObjects["Fungus1_12"]["Plant Turret"];
			preloadedGO["Grass0"] = preloadedObjects["Fungus1_12"]["simple_grass"];
			preloadedGO["Grass2"] = preloadedObjects["Fungus1_12"]["green_grass_2"];
			preloadedGO["Grass3"] = preloadedObjects["Fungus1_12"]["green_grass_3"];
			preloadedGO["Grass1"] = preloadedObjects["Fungus1_12"]["green_grass_1 (1)"];
			preloadedGO["PTrap"] = preloadedObjects["Fungus1_19"]["Plant Trap"];
			preloadedGO["Dream Base"] = preloadedObjects["White_Palace_03_hub"]["dream_nail_base"];
			preloadedGO["Dream Beam"] = preloadedObjects["White_Palace_03_hub"]["dream_beam_animation"];
			preloadedGO["Dream Door Warp"] = preloadedObjects["White_Palace_03_hub"]["doorWarp"];
			preloadedGO["Backer Shrine"] = preloadedObjects["Dream_Room_Believer_Shrine"]["Plaque_statue_01 (1)"];
			preloadedGO["VapeIn2"] = preloadedObjects["Room_Mansion"]["Heart Piece Folder/Heart Piece/Plink"];
			preloadedGO["Traitor"] = preloadedObjects["Fungus3_23_boss"]["Battle Scene/Wave 3/Mantis Traitor Lord"];
			preloadedGO["BSCW"] = preloadedObjects["GG_White_Defender"]["Boss Scene Controller"];
			preloadedGO["StartDoor"] = preloadedObjects["GG_Atrium_Roof"]["Land of Storms Doors"];
			preloadedGO["Godseeker"] = preloadedObjects["GG_White_Defender"]["GG_Arena_Prefab/Godseeker Crowd"];
			preloadedGO["WhiteDef"] = preloadedObjects["Dream_04_White_Defender"]["White Defender"];
			preloadedGO["DreamEntry"] = preloadedObjects["Dream_04_White_Defender"]["Dream Entry"];
			preloadedGO["SMTest"] = preloadedObjects["Dream_04_White_Defender"]["_SceneManager"];
			preloadedGO["BattleGate"] = preloadedObjects["Dream_04_White_Defender"]["Battle Gate (1)"];
			preloadedGO["DreamFall"] = preloadedObjects["Dream_04_White_Defender"]["Dream Fall Catcher"];
			preloadedGO["Knight Ball"] = preloadedObjects["Dream_Final_Boss"]["Boss Control/Radiance/Death/Knight Split/Knight Ball"];
			preloadedGO["Radiance"] = preloadedObjects["Dream_Final_Boss"]["Boss Control/Radiance"];
			preloadedGO["isma_stat"] = null;
			preloadedGO["CharmGet"] = preloadedObjects["Room_Queen"]["UI Msg Get WhiteCharm"];
			preloadedGO["Shiny"] = preloadedObjects["Room_Queen"]["Queen Item"];
			preloadedGO["SoulTwister"] = preloadedObjects["Ruins1_23"]["Mage"];
			preloadedGO["SoulEffect"] = preloadedObjects["Tutorial_01"]["_Props/Tut_tablet_top/Glows"];
			preloadedGO["BreakableCeiling"] = preloadedObjects["Deepnest_East_11"]["Breakable Wall top"];
			preloadedGO["CollapseFloor"] = preloadedObjects["Deepnest_38"]["Collapser Small"];
			preloadedGO["ShadeGate"] = preloadedObjects["Abyss_10"]["higher_being/shadow_gate"];
			preloadedGO["BreakableFossil"] = preloadedObjects["Abyss_10"]["Ruins Fossil"];
			preloadedGO["BreakableClawPole"] = preloadedObjects["Abyss_10"]["Tute Pole 2"];
			preloadedGO["BreakableVines"] = preloadedObjects["Abyss_10"]["hanging_cords_01"];
			preloadedGO["AbyssSM"] = preloadedObjects["Abyss_10"]["_SceneManager"];
			preloadedGO["AbyssWater1"] = preloadedObjects["Abyss_09"]["abyss_black-water/abyss_water_top (3)"];
			preloadedGO["AbyssWater2"] = preloadedObjects["Abyss_10"]["Surface Water Region"];
			preloadedGO["DReg"] = preloadedObjects["Abyss_10"]["Darkness Region"];
			preloadedGO["DreamgateLock"] = preloadedObjects["Waterways_05"]["Dream Gate Set Lock"];
			preloadedGO["ShadeSpawn"] = preloadedObjects["Abyss_10"]["Hollow_Shade Marker"];
			preloadedGO["SoulTotem"] = preloadedObjects["Mines_28"]["Soul Totem 5"];
			preloadedGO["TotemBase"] = preloadedObjects["Mines_28"]["Mini_totems_0000_7"];
			journalEntries.Add("Isma", new JournalHelper(SPRITES["journal_icon_isma"], SPRITES["journal_isma"], base.SaveSettings.IsmaEntryData, new JournalHelper.JournalNameStrings
			{
				name = langStrings.Get("ENTRY_ISMA_LONGNAME", "Journal"),
				desc = langStrings.Get("ENTRY_ISMA_DESC", "Journal"),
				note = langStrings.Get("ENTRY_ISMA_NOTE", "Journal"),
				shortname = langStrings.Get("ENTRY_ISMA_NAME", "Journal")
			}, "WhiteDefender", JournalHelper.EntryType.Dream));
			journalEntries.Add("Dryya", new JournalHelper(SPRITES["journal_icon_dryya"], SPRITES["journal_dryya"], base.SaveSettings.DryyaEntryData, new JournalHelper.JournalNameStrings
			{
				name = langStrings.Get("ENTRY_DRYYA_LONGNAME", "Journal"),
				desc = langStrings.Get("ENTRY_DRYYA_DESC", "Journal"),
				note = langStrings.Get("ENTRY_DRYYA_NOTE", "Journal"),
				shortname = langStrings.Get("ENTRY_DRYYA_NAME", "Journal")
			}, "WhiteDefender", JournalHelper.EntryType.Dream));
			journalEntries.Add("Hegemol", new JournalHelper(SPRITES["journal_icon_hegemol"], SPRITES["journal_hegemol"], base.SaveSettings.HegemolEntryData, new JournalHelper.JournalNameStrings
			{
				name = langStrings.Get("ENTRY_HEG_LONGNAME", "Journal"),
				desc = langStrings.Get("ENTRY_HEG_DESC", "Journal"),
				note = langStrings.Get("ENTRY_HEG_NOTE", "Journal"),
				shortname = langStrings.Get("ENTRY_HEG_NAME", "Journal")
			}, "WhiteDefender", JournalHelper.EntryType.Dream));
			journalEntries.Add("Zemer", new JournalHelper(SPRITES["journal_icon_zemer"], SPRITES["journal_zemer"], base.SaveSettings.ZemerEntryData, new JournalHelper.JournalNameStrings
			{
				name = langStrings.Get("ENTRY_ZEM_LONGNAME", "Journal"),
				desc = langStrings.Get("ENTRY_ZEM_DESC", "Journal"),
				note = langStrings.Get("ENTRY_ZEM_NOTE", "Journal"),
				shortname = langStrings.Get("ENTRY_ZEM_NAME", "Journal")
			}, "WhiteDefender", JournalHelper.EntryType.Dream));
			journalEntries.Add("Tiso", new JournalHelper(SPRITES["journal_icon_tiso"], SPRITES["journal_tiso"], base.SaveSettings.Mawlek2EntryData, new JournalHelper.JournalNameStrings
			{
				name = langStrings.Get("ENTRY_TISO_LONGNAME", "Journal"),
				desc = langStrings.Get("ENTRY_TISO_DESC", "Journal"),
				note = langStrings.Get("ENTRY_TISO_NOTE", "Journal"),
				shortname = langStrings.Get("ENTRY_TISO_NAME", "Journal")
			}, "LobsterLancer"));
			preloadedGO["Bloom Anim Prefab"] = ABManager.AssetBundles[ABManager.Bundle.Charms].LoadAsset<GameObject>("BloomAnim");
			preloadedGO["Bloom Sprite Prefab"] = ABManager.AssetBundles[ABManager.Bundle.Charms].LoadAsset<GameObject>("AbyssalBloom");
			Instance = this;
			UnityEngine.Object.Destroy(preloadedGO["DPortal"].LocateMyFSM("Check if midwarp or completed"));
			UnityEngine.Object.Destroy(preloadedGO["DPortal"].LocateMyFSM("FSM"));
			GameManager.instance.StartCoroutine(WaitForTitle());
			PlantChanger();
			GSPImport.AddFastDashPredicate((Scene prev, Scene next) => next.name == "White_Palace_09" && prev.name != "White_Palace_13");
			GSPImport.AddInfiniteChallengeReturnScenePredicate(delegate(GameManager.SceneLoadInfo info)
			{
				bool flag;
				switch (GameManager.instance.sceneName)
				{
				case "GG_White_Defender":
				case "gg dryya":
				case "gg zemer":
				case "gg hegemol":
				case "gg isma":
					flag = true;
					break;
				default:
					flag = false;
					break;
				}
				return flag && info.SceneName == "White_Palace_09";
			});
			if (!FullSettingsMod<SaveModSettings, GlobalModSettings>.GlobalSettings.ForcedMenuTheme)
			{
				FullSettingsMod<SaveModSettings, GlobalModSettings>.GlobalSettings.ForcedMenuTheme = true;
				MenuStyles.MenuStyle item = MenuStyles.Instance.styles.First((MenuStyles.MenuStyle x) => x.styleObject.name.Contains("Pale_Court"));
				MenuStyles.Instance.SetStyle(MenuStyles.Instance.styles.ToList().IndexOf(item), fade: false);
			}
			Log("Initializing");
		}

		public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
		{
			List<IMenuMod.MenuEntry> list = new List<IMenuMod.MenuEntry>();
			if (toggleButtonEntry.HasValue)
			{
				list.Add(new IMenuMod.MenuEntry(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values, "在菜单中启用或关闭苍白宫廷模组。", toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader));
			}
			list.Add(new IMenuMod.MenuEntry("强制苍白宫廷菜单主题", new string[2] { "关", "开" }, "强制使用苍白宫廷菜单主题。", delegate(int index)
			{
				FullSettingsMod<SaveModSettings, GlobalModSettings>.GlobalSettings.ForcedMenuTheme = index == 1;
			}, () => FullSettingsMod<SaveModSettings, GlobalModSettings>.GlobalSettings.ForcedMenuTheme ? 1 : 0));
			list.Add(new IMenuMod.MenuEntry("调试模式", new string[2] { "关", "开" }, "启用调试模式。", delegate(int index)
			{
				isDebug = index == 1;
			}, () => isDebug ? 1 : 0));
			list.Add(new IMenuMod.MenuEntry("伊斯玛雕像进度", new string[5] { "未记录", "已解锁", "完成第一阶", "完成第二阶", "完成第三阶" }, "设置伊斯玛雕像进度。", delegate(int index)
			{
				base.SaveSettings.CompletionIsma = GetCompletion(index);
			}, () => GetCompletionIndex(base.SaveSettings.CompletionIsma)));
			list.Add(new IMenuMod.MenuEntry("伊斯玛（梦境）雕像进度", new string[5] { "未记录", "已解锁", "完成第一阶", "完成第二阶", "完成第三阶" }, "设置伊斯玛梦境雕像进度。", delegate(int index)
			{
				base.SaveSettings.CompletionIsma2 = GetCompletion(index);
			}, () => GetCompletionIndex(base.SaveSettings.CompletionIsma2)));
			list.Add(new IMenuMod.MenuEntry("泽莫尔雕像进度", new string[5] { "未记录", "已解锁", "完成第一阶", "完成第二阶", "完成第三阶" }, "设置泽莫尔雕像进度。", delegate(int index)
			{
				base.SaveSettings.CompletionZemer = GetCompletion(index);
			}, () => GetCompletionIndex(base.SaveSettings.CompletionZemer)));
			list.Add(new IMenuMod.MenuEntry("泽莫尔（梦境）雕像进度", new string[5] { "未记录", "已解锁", "完成第一阶", "完成第二阶", "完成第三阶" }, "设置泽莫尔梦境雕像进度。", delegate(int index)
			{
				base.SaveSettings.CompletionZemer2 = GetCompletion(index);
			}, () => GetCompletionIndex(base.SaveSettings.CompletionZemer2)));
			list.Add(new IMenuMod.MenuEntry("德莱娅雕像进度", new string[5] { "未记录", "已解锁", "完成第一阶", "完成第二阶", "完成第三阶" }, "设置德莱娅雕像进度。", delegate(int index)
			{
				base.SaveSettings.CompletionDryya = GetCompletion(index);
			}, () => GetCompletionIndex(base.SaveSettings.CompletionDryya)));
			list.Add(new IMenuMod.MenuEntry("海格默雕像进度", new string[5] { "未记录", "已解锁", "完成第一阶", "完成第二阶", "完成第三阶" }, "设置海格默雕像进度。", delegate(int index)
			{
				base.SaveSettings.CompletionHegemol = GetCompletion(index);
			}, () => GetCompletionIndex(base.SaveSettings.CompletionHegemol)));
			list.Add(new IMenuMod.MenuEntry("马瓦雷克雕像进度", new string[5] { "未记录", "已解锁", "完成第一阶", "完成第二阶", "完成第三阶" }, "设置马瓦雷克雕像进度。", delegate(int index)
			{
				base.SaveSettings.CompletionMawlek2 = GetCompletion(index);
			}, () => GetCompletionIndex(base.SaveSettings.CompletionMawlek2)));
			list.Add(new IMenuMod.MenuEntry("伊斯玛雕像使用梦境版本", new string[2] { "关", "开" }, "设置伊斯玛是否使用梦境雕像版本。", delegate(int index)
			{
				base.SaveSettings.AltStatueIsma = index == 1;
			}, () => base.SaveSettings.AltStatueIsma ? 1 : 0));
			list.Add(new IMenuMod.MenuEntry("泽莫尔雕像使用梦境版本", new string[2] { "关", "开" }, "设置泽莫尔是否使用梦境雕像版本。", delegate(int index)
			{
				base.SaveSettings.AltStatueZemer = index == 1;
			}, () => base.SaveSettings.AltStatueZemer ? 1 : 0));
			list.Add(new IMenuMod.MenuEntry("马瓦雷克雕像使用梦境版本", new string[2] { "关", "开" }, "设置马瓦雷克是否使用梦境雕像版本。", delegate(int index)
			{
				base.SaveSettings.AltStatueMawlek = index == 1;
			}, () => base.SaveSettings.AltStatueMawlek ? 1 : 0));
			list.Add(new IMenuMod.MenuEntry("纯洁之印护符状态", new string[4] { "未获得", "新护符", "已获得", "已装备" }, "设置纯洁之印护符状态。", delegate(int index)
			{
				SetCharmState(0, index);
			}, () => GetCharmState(0)));
			list.Add(new IMenuMod.MenuEntry("悲叹之瓶护符状态", new string[4] { "未获得", "新护符", "已获得", "已装备" }, "设置悲叹之瓶护符状态。", delegate(int index)
			{
				SetCharmState(1, index);
			}, () => GetCharmState(1)));
			list.Add(new IMenuMod.MenuEntry("圣巢恩赐护符状态", new string[4] { "未获得", "新护符", "已获得", "已装备" }, "设置圣巢恩赐护符状态。", delegate(int index)
			{
				SetCharmState(2, index);
			}, () => GetCharmState(2)));
			list.Add(new IMenuMod.MenuEntry("深渊之花护符状态", new string[4] { "未获得", "新护符", "已获得", "已装备" }, "设置深渊之花护符状态。", delegate(int index)
			{
				SetCharmState(3, index);
			}, () => GetCharmState(3)));
			list.Add(new IMenuMod.MenuEntry("10 号护符已升级", new string[2] { "关", "开" }, "设置 10 号护符是否已升级。", delegate(int index)
			{
				base.SaveSettings.upgradedCharm_10 = index == 1;
			}, () => base.SaveSettings.upgradedCharm_10 ? 1 : 0));
			list.Add(new IMenuMod.MenuEntry("已问候钉匠", new string[2] { "关", "开" }, "设置是否已问候钉匠。", delegate(int index)
			{
				base.SaveSettings.GreetedNailsmith = index == 1;
			}, () => base.SaveSettings.GreetedNailsmith ? 1 : 0));
			list.Add(new IMenuMod.MenuEntry("已问候谢奥", new string[2] { "关", "开" }, "设置是否已问候谢奥。", delegate(int index)
			{
				base.SaveSettings.GreetedSheo = index == 1;
			}, () => base.SaveSettings.GreetedSheo ? 1 : 0));
			list.Add(new IMenuMod.MenuEntry("神堂存档", new string[2] { "关", "开" }, "设置神堂存档是否已开启。", delegate(int index)
			{
				base.SaveSettings.GodseekerSave = index == 1;
			}, () => base.SaveSettings.GodseekerSave ? 1 : 0));
			list.Add(new IMenuMod.MenuEntry("已见过神唤", new string[2] { "关", "开" }, "设置是否已见过神唤。", delegate(int index)
			{
				base.SaveSettings.SeenChampionsCall = index == 1;
			}, () => base.SaveSettings.SeenChampionsCall ? 1 : 0));
			return list;
		}

		private BossStatue.Completion GetCompletion(int index)
		{
			return index switch
			{
				1 => new BossStatue.Completion
				{
					hasBeenSeen = true,
					isUnlocked = true
				}, 
				2 => new BossStatue.Completion
				{
					hasBeenSeen = true,
					isUnlocked = true,
					completedTier1 = true
				}, 
				3 => new BossStatue.Completion
				{
					hasBeenSeen = true,
					isUnlocked = true,
					completedTier1 = true,
					completedTier2 = true
				}, 
				4 => new BossStatue.Completion
				{
					hasBeenSeen = true,
					isUnlocked = true,
					completedTier1 = true,
					completedTier2 = true,
					completedTier3 = true
				}, 
				_ => BossStatue.Completion.None, 
			};
		}

		private int GetCompletionIndex(BossStatue.Completion completion)
		{
			if (!completion.isUnlocked)
			{
				return 0;
			}
			if (!completion.completedTier1)
			{
				return 1;
			}
			if (!completion.completedTier2)
			{
				return 2;
			}
			if (!completion.completedTier3)
			{
				return 3;
			}
			return 4;
		}

		private void SetCharmState(int index, int state)
		{
			switch (index)
			{
			case 0:
				base.SaveSettings.newCharms[0] = state >= 1;
				base.SaveSettings.gotCharms[0] = state >= 2;
				base.SaveSettings.equippedCharms[0] = state >= 3;
				break;
			case 1:
				base.SaveSettings.newCharms[1] = state >= 1;
				base.SaveSettings.gotCharms[1] = state >= 2;
				base.SaveSettings.equippedCharms[1] = state >= 3;
				break;
			case 2:
				base.SaveSettings.newCharms[2] = state >= 1;
				base.SaveSettings.gotCharms[2] = state >= 2;
				base.SaveSettings.equippedCharms[2] = state >= 3;
				break;
			case 3:
				base.SaveSettings.newCharms[3] = state >= 1;
				base.SaveSettings.gotCharms[3] = state >= 2;
				base.SaveSettings.equippedCharms[3] = state >= 3;
				break;
			default:
				base.SaveSettings.newCharms[4] = state >= 1;
				base.SaveSettings.gotCharms[4] = state >= 2;
				base.SaveSettings.equippedCharms[4] = state >= 3;
				break;
			}
		}

		private int GetCharmState(int index)
		{
			switch (index)
			{
			case 0:
				if (!base.SaveSettings.newCharms[0])
				{
					return 0;
				}
				if (!base.SaveSettings.gotCharms[0])
				{
					return 1;
				}
				if (!base.SaveSettings.equippedCharms[0])
				{
					return 2;
				}
				return 3;
			case 1:
				if (!base.SaveSettings.newCharms[1])
				{
					return 0;
				}
				if (!base.SaveSettings.gotCharms[1])
				{
					return 1;
				}
				if (!base.SaveSettings.equippedCharms[1])
				{
					return 2;
				}
				return 3;
			case 2:
				if (!base.SaveSettings.newCharms[2])
				{
					return 0;
				}
				if (!base.SaveSettings.gotCharms[2])
				{
					return 1;
				}
				if (!base.SaveSettings.equippedCharms[2])
				{
					return 2;
				}
				return 3;
			case 3:
				if (!base.SaveSettings.newCharms[3])
				{
					return 0;
				}
				if (!base.SaveSettings.gotCharms[3])
				{
					return 1;
				}
				if (!base.SaveSettings.equippedCharms[3])
				{
					return 2;
				}
				return 3;
			default:
				if (!base.SaveSettings.newCharms[4])
				{
					return 0;
				}
				if (!base.SaveSettings.gotCharms[4])
				{
					return 1;
				}
				if (!base.SaveSettings.equippedCharms[4])
				{
					return 2;
				}
				return 3;
			}
		}

		public void Unload()
		{
			ModHooks.BeforeSavegameSaveHook -= SaveEntries;
			ModHooks.SetPlayerVariableHook -= SetVariableHook;
			ModHooks.GetPlayerVariableHook -= GetVariableHook;
			ModHooks.AfterSavegameLoadHook -= AfterSavegameLoadHookHandler;
			On.GameManager.StartNewGame -= GameManager_StartNewGame;
			ModHooks.GetPlayerBoolHook -= ModHooks_GetPlayerBool;
			ModHooks.SetPlayerBoolHook -= ModHooks_SetPlayerBool;
			ModHooks.GetPlayerIntHook -= ModHooks_GetPlayerInt;
			ModHooks.SetPlayerStringHook -= ModHooks_SetPlayerString;
			ModHooks.GetPlayerStringHook -= ModHooks_GetPlayerString;
			On.Language.Language.DoSwitch -= SwitchLanguage;
			ModHooks.LanguageGetHook -= LangGet;
			On.AudioManager.ApplyMusicCue -= LoadPaleCourtMenuMusic;
			UIManager.EditMenus -= UIManagerEditMenus;
			ABManager.UnloadAll();
		}

		private void AfterSavegameLoadHookHandler(SaveGameData saveGameData)
		{
			StartGame();
		}

		private IEnumerator WaitForTitle()
		{
			yield return new WaitUntil(() => GameObject.Find("LogoTitle") != null);
			UIManager.EditMenus += UIManagerEditMenus;
		}

		private void UIManagerEditMenus()
		{
			Text[] componentsInChildren = UIManager.instance.gameObject.GetComponentsInChildren<Text>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Outline outline = componentsInChildren[i].gameObject.AddComponent<Outline>();
				outline.effectColor = Color.black;
				outline.effectDistance = new Vector2(1.5f, -1.5f);
			}
		}

		private void ChangeVersionNumber(On.SetVersionNumber.orig_Start orig, SetVersionNumber self)
		{
			orig(self);
			self.GetAttr<SetVersionNumber, Text>("textUi").text = "1.6.1.3";
		}

		private void LoadPaleCourtMenuMusic(On.AudioManager.orig_ApplyMusicCue orig, AudioManager self, MusicCue musicCue, float delayTime, float transitionTime, bool applySnapshot)
		{
			MusicCue.MusicChannelInfo[] array = (MusicCue.MusicChannelInfo[])musicCue.GetType().GetField("channelInfos", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(musicCue);
			AudioClip attr = array[0].GetAttr<MusicCue.MusicChannelInfo, AudioClip>("clip");
			if (attr != null && attr.name.Equals("Title"))
			{
				if (!ABManager.AssetBundles.ContainsKey(ABManager.Bundle.Sound))
				{
					LoadMusic();
				}
				array[4] = new MusicCue.MusicChannelInfo();
				array[4].SetAttr("clip", ABManager.AssetBundles[ABManager.Bundle.Sound].LoadAsset("MM_Aud"));
				array[4].SetAttr("sync", MusicChannelSync.ExplicitOff);
			}
			musicCue.GetType().GetField("channelInfos", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(musicCue, array);
			orig(self, musicCue, delayTime, transitionTime, applySnapshot);
		}

		private (string languageString, GameObject styleGo, int titleIndex, string unlockKey, string[] achievementKeys, MenuStyles.MenuStyle.CameraCurves cameraCurves, AudioMixerSnapshot musicSnapshot) AddPCMenuStyle(MenuStyles self)
		{
			Log("Start");
			Material material = new Material(Shader.Find("Sprites/Default"));
			material.SetColor(Shader.PropertyToID("_Color"), new Color(1f, 1f, 1f, 1f));
			material.SetInt(Shader.PropertyToID("PixelSnap"), 0);
			material.SetFloat(Shader.PropertyToID("_EnableExternalAlpha"), 0f);
			material.SetInt(Shader.PropertyToID("_StencilComp"), 8);
			material.SetInt(Shader.PropertyToID("_Stencil"), 0);
			material.SetInt(Shader.PropertyToID("_StencilOp"), 0);
			material.SetInt(Shader.PropertyToID("_StencilWriteMask"), 255);
			material.SetInt(Shader.PropertyToID("_StencilReadMask"), 255);
			GameObject gameObject = UnityEngine.Object.Instantiate(ABManager.AssetBundles[ABManager.Bundle.TitleScreen].LoadAsset<GameObject>("Pale_Court_Style_1"));
			if (gameObject == null)
			{
				gameObject = new GameObject("Pale_Court");
			}
			Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.SetActive(value: true);
			}
			SpriteRenderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<SpriteRenderer>();
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				componentsInChildren2[i].materials = new Material[1] { material };
			}
			gameObject.transform.SetParent(self.gameObject.transform);
			gameObject.transform.localPosition = new Vector3(0f, -1.2f, 0f);
			MenuStyles.MenuStyle.CameraCurves cameraCurves = new MenuStyles.MenuStyle.CameraCurves();
			cameraCurves.saturation = 1f;
			cameraCurves.redChannel = new AnimationCurve();
			cameraCurves.redChannel.AddKey(new Keyframe(0f, 0f));
			cameraCurves.redChannel.AddKey(new Keyframe(1f, 1f));
			cameraCurves.greenChannel = new AnimationCurve();
			cameraCurves.greenChannel.AddKey(new Keyframe(0f, 0f));
			cameraCurves.greenChannel.AddKey(new Keyframe(1f, 1f));
			cameraCurves.blueChannel = new AnimationCurve();
			cameraCurves.blueChannel.AddKey(new Keyframe(0f, 0f));
			cameraCurves.blueChannel.AddKey(new Keyframe(1f, 1f));
			return (languageString: "UI_MENU_STYLE_PALE_COURT", styleGo: gameObject, titleIndex: paleCourtLogoId, unlockKey: "", achievementKeys: null, cameraCurves: cameraCurves, musicSnapshot: Resources.FindObjectsOfTypeAll<AudioMixer>().First((AudioMixer x) => x.name == "Music").FindSnapshot("Tension Only"));
		}

		private GameObject ChangePsrTexture(GameObject o)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(o, o.transform.parent);
			ParticleSystemRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<ParticleSystemRenderer>();
			foreach (ParticleSystemRenderer obj in componentsInChildren)
			{
				Material material = UnityEngine.Object.Instantiate(obj.material);
				material.mainTexture = SPRITES["Petal"].texture;
				obj.material = material;
			}
			return gameObject;
		}

		private (int enviromentType, GameObject dashEffects) AddCustomDashEffectsHook(DashEffect self)
		{
			if (valueAddCustomDashEffectsHookOnce == null)
			{
				valueAddCustomDashEffectsHookOnce = ChangePsrTexture(self.dashGrass);
			}
			return (enviromentType: 7, dashEffects: valueAddCustomDashEffectsHookOnce);
		}

		private (int enviromentType, GameObject hardLandEffects) AddCustomHardLandEffectsHook(HardLandEffect self)
		{
			if (valueAddCustomHardLandEffectsHookOnce == null)
			{
				valueAddCustomHardLandEffectsHookOnce = ChangePsrTexture(self.grassObj);
			}
			return (enviromentType: 7, hardLandEffects: valueAddCustomHardLandEffectsHookOnce);
		}

		private (int enviromentType, GameObject jumpEffects) AddCustomJumpEffectsHook(JumpEffects self)
		{
			if (valueAddCustomJumpEffectsHookOnce == null)
			{
				valueAddCustomJumpEffectsHookOnce = ChangePsrTexture(self.grassEffects);
			}
			return (enviromentType: 7, jumpEffects: valueAddCustomJumpEffectsHookOnce);
		}

		private (int enviromentType, GameObject softLandEffects) AddCustomSoftLandEffectsHook(SoftLandEffect self)
		{
			if (valueAddCustomSoftLandEffectsHookOnce == null)
			{
				valueAddCustomSoftLandEffectsHookOnce = ChangePsrTexture(self.grassEffects);
			}
			return (enviromentType: 7, softLandEffects: valueAddCustomSoftLandEffectsHookOnce);
		}

		private (int enviromentType, GameObject runEffects) AddCustomRunEffectsHook(GameObject self)
		{
			if (valueAddCustomRunEffectsHookOnce == null)
			{
				valueAddCustomRunEffectsHookOnce = ChangePsrTexture(self.transform.GetChild(1).gameObject);
			}
			return (enviromentType: 7, runEffects: valueAddCustomRunEffectsHookOnce);
		}

		private void LoadTitleScreen()
		{
			ABManager.Load(ABManager.Bundle.TitleScreen);
		}

		private void LoadMusic()
		{
			ABManager.Load(ABManager.Bundle.Sound);
		}

		private void LoadCharms()
		{
			ABManager.Load(ABManager.Bundle.Charms);
			ABManager.Load(ABManager.Bundle.CharmUnlock);
		}

		private void LoadBossBundles()
		{
			ABManager.Load(ABManager.Bundle.GDryya);
			ABManager.Load(ABManager.Bundle.GHegemol);
			ABManager.Load(ABManager.Bundle.GIsma);
			ABManager.Load(ABManager.Bundle.GZemer);
			ABManager.Load(ABManager.Bundle.Artist);
			ABManager.Load(ABManager.Bundle.TisoBund);
		}

		private void LoadDep()
		{
			ABManager.Load(ABManager.Bundle.GArenaDep);
			ABManager.Load(ABManager.Bundle.OWArenaDep);
			ABManager.Load(ABManager.Bundle.WSArenaDep);
			ABManager.Load(ABManager.Bundle.WSArena);
			ABManager.Load(ABManager.Bundle.GArenaHub);
			ABManager.Load(ABManager.Bundle.GArenaHub2);
			ABManager.Load(ABManager.Bundle.Misc);
			ABManager.Load(ABManager.Bundle.GArenaH);
			ABManager.Load(ABManager.Bundle.GArenaD);
			ABManager.Load(ABManager.Bundle.GArenaZ);
			ABManager.Load(ABManager.Bundle.GArenaI);
			ABManager.Load(ABManager.Bundle.OWArenaD);
			ABManager.Load(ABManager.Bundle.OWArenaZ);
			ABManager.Load(ABManager.Bundle.OWArenaH);
			ABManager.Load(ABManager.Bundle.OWArenaI);
			ABManager.Load(ABManager.Bundle.GArenaIsma);
			ABManager.Load(ABManager.Bundle.GReward);
			ABManager.Load(ABManager.Bundle.Credits);
			ABManager.Load(ABManager.Bundle.AbyssalTemple);
			Log("Finished bundling");
		}

		private void SaveEntries(SaveGameData data)
		{
			base.SaveSettings.IsmaEntryData = journalEntries["Isma"].playerData;
			base.SaveSettings.DryyaEntryData = journalEntries["Dryya"].playerData;
			base.SaveSettings.HegemolEntryData = journalEntries["Hegemol"].playerData;
			base.SaveSettings.ZemerEntryData = journalEntries["Zemer"].playerData;
			base.SaveSettings.Mawlek2EntryData = journalEntries["Tiso"].playerData;
		}

		private object SetVariableHook(Type t, string key, object orig)
		{
			switch (key)
			{
			case "statueStateIsma":
				base.SaveSettings.CompletionIsma = (BossStatue.Completion)orig;
				break;
			case "statueStateDryya":
				base.SaveSettings.CompletionDryya = (BossStatue.Completion)orig;
				break;
			case "statueStateZemer":
				base.SaveSettings.CompletionZemer = (BossStatue.Completion)orig;
				break;
			case "statueStateZemer2":
				base.SaveSettings.CompletionZemer2 = (BossStatue.Completion)orig;
				break;
			case "statueStateIsma2":
				base.SaveSettings.CompletionIsma2 = (BossStatue.Completion)orig;
				break;
			case "statueStateHegemol":
				base.SaveSettings.CompletionHegemol = (BossStatue.Completion)orig;
				break;
			case "statueStateMawlek2":
				base.SaveSettings.CompletionMawlek2 = (BossStatue.Completion)orig;
				break;
			case "statueStateBroodingMawlek":
			{
				BossStatue.Completion completion = (BossStatue.Completion)orig;
				completion.usingAltVersion = false;
				return completion;
			}
			}
			return orig;
		}

		private object GetVariableHook(Type t, string key, object orig)
		{
			switch (key)
			{
			case "statueStateIsma":
				return base.SaveSettings.CompletionIsma;
			case "statueStateDryya":
				return base.SaveSettings.CompletionDryya;
			case "statueStateZemer":
				return base.SaveSettings.CompletionZemer;
			case "statueStateZemer2":
				return base.SaveSettings.CompletionZemer2;
			case "statueStateIsma2":
				return base.SaveSettings.CompletionIsma2;
			case "statueStateHegemol":
				return base.SaveSettings.CompletionHegemol;
			case "statueStateMawlek2":
				return base.SaveSettings.CompletionMawlek2;
			case "statueStateBroodingMawlek":
			{
				BossStatue.Completion completion = (BossStatue.Completion)orig;
				completion.usingAltVersion = base.SaveSettings.AltStatueMawlek;
				return completion;
			}
			default:
				return orig;
			}
		}

		private bool ModHooks_GetPlayerBool(string target, bool orig)
		{
			if (target.StartsWith("gotCharm_"))
			{
				int item = int.Parse(target.Split('_')[1]);
				if (charmIDs.Contains(item))
				{
					return base.SaveSettings.gotCharms[charmIDs.IndexOf(item)];
				}
			}
			if (target.StartsWith("newCharm_"))
			{
				int item2 = int.Parse(target.Split('_')[1]);
				if (charmIDs.Contains(item2))
				{
					return base.SaveSettings.newCharms[charmIDs.IndexOf(item2)];
				}
			}
			if (target.StartsWith("equippedCharm_"))
			{
				int item3 = int.Parse(target.Split('_')[1]);
				if (charmIDs != null && charmIDs.Contains(item3))
				{
					return base.SaveSettings.equippedCharms[charmIDs.IndexOf(item3)];
				}
			}
			return orig;
		}

		private bool ModHooks_SetPlayerBool(string target, bool orig)
		{
			if (target.StartsWith("gotCharm_"))
			{
				int item = int.Parse(target.Split('_')[1]);
				if (charmIDs.Contains(item))
				{
					base.SaveSettings.gotCharms[charmIDs.IndexOf(item)] = orig;
					return orig;
				}
			}
			if (target.StartsWith("newCharm_"))
			{
				int item2 = int.Parse(target.Split('_')[1]);
				if (charmIDs.Contains(item2))
				{
					base.SaveSettings.newCharms[charmIDs.IndexOf(item2)] = orig;
					return orig;
				}
			}
			if (target.StartsWith("equippedCharm_"))
			{
				int item3 = int.Parse(target.Split('_')[1]);
				if (charmIDs.Contains(item3))
				{
					base.SaveSettings.equippedCharms[charmIDs.IndexOf(item3)] = orig;
					return orig;
				}
			}
			return orig;
		}

		private int ModHooks_GetPlayerInt(string target, int orig)
		{
			if (target.StartsWith("charmCost_"))
			{
				int item = int.Parse(target.Split('_')[1]);
				if (charmIDs.Contains(item))
				{
					return CharmCosts[charmIDs.IndexOf(item)];
				}
			}
			return orig;
		}

		private string ModHooks_SetPlayerString(string target, string orig)
		{
			if (target == "respawnMarkerName")
			{
				base.SaveSettings.respawnMarkerName = orig;
				if (orig == "WhiteBench(Clone)(Clone)")
				{
					return "RestBench (1)";
				}
			}
			if (target == "respawnScene")
			{
				base.SaveSettings.respawnScene = orig;
				if (orig == "White_Palace_09")
				{
					return "GG_Workshop";
				}
			}
			return orig;
		}

		private string ModHooks_GetPlayerString(string target, string orig)
		{
			if (PlayerData.instance.respawnMarkerName == "RestBench (1)" && PlayerData.instance.respawnScene == "GG_Workshop" && !string.IsNullOrEmpty(base.SaveSettings.respawnMarkerName) && base.SaveSettings.respawnMarkerName == "WhiteBench(Clone)(Clone)" && !string.IsNullOrEmpty(base.SaveSettings.respawnScene) && base.SaveSettings.respawnScene == "White_Palace_09")
			{
				if (target == "respawnMarkerName")
				{
					return "WhiteBench(Clone)(Clone)";
				}
				if (target == "respawnScene")
				{
					return "White_Palace_09";
				}
			}
			return orig;
		}

		private string LangGet(string key, string sheet, string orig)
		{
			if (key.StartsWith("CHARM_DESC_") || key.StartsWith("CHARM_NAME_"))
			{
				int num = int.Parse(key.Split('_')[2]);
				if (charmIDs.Contains(num))
				{
					key = key.Substring(0, 11) + CharmKeys[charmIDs.IndexOf(num)];
				}
				else if (num == 10 && base.SaveSettings.upgradedCharm_10)
				{
					key = key.Substring(0, 11) + CharmKeys[4];
				}
			}
			if (key == "UI_MENU_STYLE_PALE_COURT")
			{
				return "苍白宫廷";
			}
			if (!langStrings.ContainsKey(key, sheet))
			{
				return orig;
			}
			return langStrings.Get(key, sheet);
		}

		private void GameManager_StartNewGame(On.GameManager.orig_StartNewGame orig, GameManager self, bool permaDeath, bool bossRush)
		{
			orig(self, permaDeath, bossRush);
			if (bossRush)
			{
				base.SaveSettings.GodseekerSave = true;
				base.SaveSettings.gotCharms = new bool[4] { true, true, true, true };
				base.SaveSettings.upgradedCharm_10 = true;
				base.SaveSettings.CompletionIsma.isUnlocked = true;
				base.SaveSettings.CompletionIsma.hasBeenSeen = true;
				base.SaveSettings.CompletionDryya.isUnlocked = true;
				base.SaveSettings.CompletionDryya.hasBeenSeen = true;
				base.SaveSettings.CompletionZemer.isUnlocked = true;
				base.SaveSettings.CompletionZemer.hasBeenSeen = true;
				base.SaveSettings.CompletionHegemol.isUnlocked = true;
				base.SaveSettings.CompletionHegemol.hasBeenSeen = true;
				base.SaveSettings.CompletionMawlek2.isUnlocked = true;
				base.SaveSettings.CompletionMawlek2.hasBeenSeen = true;
			}
			StartGame();
		}

		private void StartGame()
		{
			if (PlayerData.instance.bossRushMode && !base.SaveSettings.HasSeenWorkshopRaised)
			{
				base.SaveSettings.GodseekerSave = true;
				base.SaveSettings.gotCharms = new bool[4] { true, true, true, true };
				base.SaveSettings.upgradedCharm_10 = true;
				base.SaveSettings.CompletionIsma.isUnlocked = true;
				base.SaveSettings.CompletionIsma.hasBeenSeen = true;
				base.SaveSettings.CompletionDryya.isUnlocked = true;
				base.SaveSettings.CompletionDryya.hasBeenSeen = true;
				base.SaveSettings.CompletionZemer.isUnlocked = true;
				base.SaveSettings.CompletionZemer.hasBeenSeen = true;
				base.SaveSettings.CompletionHegemol.isUnlocked = true;
				base.SaveSettings.CompletionHegemol.hasBeenSeen = true;
				base.SaveSettings.CompletionMawlek2.isUnlocked = true;
				base.SaveSettings.CompletionMawlek2.hasBeenSeen = true;
			}
			GameManager.instance.gameObject.AddComponent<ArenaFinder>();
			GameManager.instance.gameObject.AddComponent<TisoFinder>();
			GameManager.instance.gameObject.AddComponent<OWArenaFinder>();
			GameManager.instance.gameObject.AddComponent<Amulets>();
			GameManager.instance.gameObject.AddComponent<AwardCharms>();
			journalEntries["Isma"].playerData = base.SaveSettings.IsmaEntryData;
			journalEntries["Dryya"].playerData = base.SaveSettings.DryyaEntryData;
			journalEntries["Hegemol"].playerData = base.SaveSettings.HegemolEntryData;
			journalEntries["Zemer"].playerData = base.SaveSettings.ZemerEntryData;
			journalEntries["Tiso"].playerData = base.SaveSettings.Mawlek2EntryData;
			GameObject[] array = Resources.FindObjectsOfTypeAll<GameObject>();
			foreach (GameObject gameObject in array)
			{
				if (gameObject.name == "Gas Explosion Recycle M")
				{
					Log("Adding explosion");
					preloadedGO["Explosion"] = gameObject.gameObject;
				}
				if (gameObject.name == "Slash" && gameObject.transform.parent != null && gameObject.transform.parent.gameObject.name == "Hollow Shade")
				{
					Log("Adding parry tink");
					preloadedGO["parryFX"] = gameObject.LocateMyFSM("nail_clash_tink").GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>("No Box Down", 1).gameObject.Value;
					AudioClip audioClip = gameObject.LocateMyFSM("nail_clash_tink").GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShot>("Blocked Hit", 5).audioClips[0];
					GameObject gameObject2 = new GameObject();
					AudioSource audioSource = gameObject2.AddComponent<AudioSource>();
					audioSource.clip = audioClip;
					audioSource.pitch = UnityEngine.Random.Range(0.85f, 1.15f);
					ParryTink.TinkClip = audioClip;
					Tink.TinkClip = audioClip;
					preloadedGO["ClashTink"] = gameObject2;
					break;
				}
			}
		}

		private void PlantChanger()
		{
			string[] array = new string[2] { "PTrap", "PTurret" };
			foreach (string text in array)
			{
				GameObject gameObject = preloadedGO[text];
				UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<InfectedEnemyEffects>());
				GameObject gameObject2 = preloadedGO["WhiteDef"];
				EnemyHitEffectsUninfected component = gameObject2.GetComponent<EnemyHitEffectsUninfected>();
				EnemyHitEffectsUninfected enemyHitEffectsUninfected = gameObject.AddComponent<EnemyHitEffectsUninfected>();
				FieldInfo[] fields = typeof(EnemyHitEffectsUninfected).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (FieldInfo fieldInfo in fields)
				{
					if (fieldInfo.Name.Contains("Origin"))
					{
						enemyHitEffectsUninfected.effectOrigin = new Vector3(0f, 0.5f, 0f);
					}
					else
					{
						fieldInfo.SetValue(enemyHitEffectsUninfected, fieldInfo.GetValue(component));
					}
				}
				EnemyDeathEffectsUninfected enemyDeathEffectsUninfected = gameObject.AddComponent<EnemyDeathEffectsUninfected>();
				EnemyDeathEffectsUninfected component2 = gameObject2.GetComponent<EnemyDeathEffectsUninfected>();
				EnemyDeathEffects component3 = gameObject.GetComponent<EnemyDeathEffects>();
				fields = typeof(EnemyDeathEffects).GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (FieldInfo fieldInfo2 in fields)
				{
					fieldInfo2.SetValue(enemyDeathEffectsUninfected, fieldInfo2.GetValue(component3));
				}
				UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<EnemyDeathEffects>());
				foreach (FieldInfo item in from x in typeof(EnemyDeathEffectsUninfected).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
					where x.Name.IndexOf("corpse", StringComparison.OrdinalIgnoreCase) < 0
					select x)
				{
					item.SetValue(enemyDeathEffectsUninfected, item.GetValue(component2));
				}
				foreach (FieldInfo item2 in from x in typeof(EnemyDeathEffects).GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
					where x.Name.IndexOf("corpse", StringComparison.OrdinalIgnoreCase) < 0
					select x)
				{
					item2.SetValue(enemyDeathEffectsUninfected, item2.GetValue(component2));
				}
				HealthManager component4 = gameObject.GetComponent<HealthManager>();
				HealthManager component5 = gameObject2.GetComponent<HealthManager>();
				foreach (FieldInfo item3 in from x in typeof(HealthManager).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
					where x.Name.Contains("Prefab")
					select x)
				{
					item3.SetValue(component4, item3.GetValue(component5));
				}
				PersistentBoolItem[] componentsInChildren = gameObject.GetComponentsInChildren<PersistentBoolItem>(includeInactive: true);
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					UnityEngine.Object.Destroy(componentsInChildren[j]);
				}
				GameObject attr = enemyDeathEffectsUninfected.GetAttr<EnemyDeathEffects, GameObject>("corpsePrefab");
				if (text == "PTrap" && attr.transform.Find("Orange Puff") != null)
				{
					attr.transform.Find("Orange Puff").gameObject.AddComponent<ManipOrangePuff>();
				}
				else
				{
					ParticleSystem[] componentsInChildren2 = attr.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
					for (int j = 0; j < componentsInChildren2.Length; j++)
					{
						ParticleSystem.MainModule main = componentsInChildren2[j].main;
						main.startColor = new Color(0.16f, 0.5f, 0.003f);
					}
				}
				enemyDeathEffectsUninfected.whiteWave = attr;
				GameObject gameObject3 = new GameObject();
				UnityEngine.Object.DontDestroyOnLoad(gameObject3);
				enemyDeathEffectsUninfected.uninfectedDeathPt = gameObject3;
				enemyDeathEffectsUninfected.SetAttr<EnemyDeathEffects, GameObject>("corpsePrefab", null);
				preloadedGO[text] = gameObject;
			}
		}
	}
	public static class ABManager
	{
		public enum Bundle
		{
			Sound,
			TitleScreen,
			GIsma,
			GDryya,
			GHegemol,
			GZemer,
			GArenaDep,
			GArenaHub,
			GArenaHub2,
			GArenaIsma,
			GArenaH,
			GArenaD,
			GArenaZ,
			GArenaI,
			GReward,
			OWArenaD,
			OWArenaZ,
			OWArenaH,
			OWArenaI,
			OWArenaDep,
			WSArenaDep,
			WSArena,
			Tk2dDep,
			Credits,
			Misc,
			Charms,
			CharmUnlock,
			Artist,
			TisoBund,
			AbyssalTemple
		}

		private static readonly Assembly _asm = Assembly.GetExecutingAssembly();

		public static Dictionary<Bundle, AssetBundle> AssetBundles { get; } = new Dictionary<Bundle, AssetBundle>();

		private static string BundleToString(Bundle bd)
		{
			return bd switch
			{
				Bundle.Sound => "soundbund", 
				Bundle.TitleScreen => "titlescreen", 
				Bundle.GIsma => "isma" + FiveKnights.OS, 
				Bundle.GDryya => "dryya" + FiveKnights.OS, 
				Bundle.GHegemol => "hegemol" + FiveKnights.OS, 
				Bundle.GZemer => "zemer" + FiveKnights.OS, 
				Bundle.GArenaIsma => "ismabg", 
				Bundle.GArenaDep => "ggarenadep", 
				Bundle.GArenaHub => "ggarenahub", 
				Bundle.GArenaHub2 => "hubasset1", 
				Bundle.GArenaH => "ggarenahegemol", 
				Bundle.GArenaD => "ggarenadryya", 
				Bundle.GArenaI => "ggarenaisma", 
				Bundle.GArenaZ => "ggarenazemer", 
				Bundle.GReward => "rewardroom", 
				Bundle.OWArenaD => "owarenadryya", 
				Bundle.OWArenaH => "owarenahegemol", 
				Bundle.OWArenaZ => "owarenazemer", 
				Bundle.OWArenaI => "owarenaisma", 
				Bundle.OWArenaDep => "owarenadep", 
				Bundle.WSArenaDep => "workshopdep", 
				Bundle.WSArena => "workshopentrance", 
				Bundle.Tk2dDep => "blendvertexcolor", 
				Bundle.Credits => "creditsbundle", 
				Bundle.Misc => "miscbund", 
				Bundle.Charms => "pureamulets", 
				Bundle.CharmUnlock => "charmunlock", 
				Bundle.Artist => "artistsbund", 
				Bundle.TisoBund => "tisobundle", 
				Bundle.AbyssalTemple => "abyssaltemple", 
				_ => "", 
			};
		}

		public static AssetBundle Load(Bundle bd)
		{
			if (AssetBundles.ContainsKey(bd) && AssetBundles[bd] != null)
			{
				return AssetBundles[bd];
			}
			using Stream stream = _asm.GetManifestResourceStream("FiveKnights.StreamingAssets." + BundleToString(bd));
			AssetBundle assetBundle = AssetBundle.LoadFromStream(stream);
			AssetBundles[bd] = assetBundle;
			stream?.Dispose();
			return assetBundle;
		}

		public static IEnumerator LoadAsync(Bundle bd)
		{
			using Stream s = _asm.GetManifestResourceStream("FiveKnights.StreamingAssets." + BundleToString(bd));
			AssetBundleCreateRequest request = AssetBundle.LoadFromStreamAsync(s);
			yield return request;
			AssetBundles[bd] = request.assetBundle;
			s?.Dispose();
		}

		public static void ResetBundle(Bundle bd)
		{
			Log($"Resetting bundle {bd}");
			if (!AssetBundles.TryGetValue(bd, out var value) || value == null)
			{
				Log("Could not find AssetBundle " + BundleToString(bd) + "!");
				return;
			}
			value.Unload(unloadAllLoadedObjects: true);
			GameManager.instance.StartCoroutine(DelayedReset());
			IEnumerator DelayedReset()
			{
				yield return null;
				AssetBundles[bd] = Load(bd);
				Log($"Reset bundle {bd}");
			}
		}

		public static void UnloadAll()
		{
			foreach (Bundle key in AssetBundles.Keys)
			{
				AssetBundle assetBundle = AssetBundles[key];
				if (assetBundle != null && key != Bundle.TitleScreen)
				{
					Log("Unloaded assetbundle " + assetBundle.name);
					assetBundle.Unload(unloadAllLoadedObjects: true);
				}
			}
		}

		private static void Log(object o)
		{
			Modding.Logger.Log("[BundleManager] " + o);
		}
	}
	public static class AbyssalTemple
	{
		internal class NoShakeTink : MonoBehaviour
		{
			internal static AudioClip TinkClip = ABManager.AssetBundles[ABManager.Bundle.Sound].LoadAsset<AudioClip>("sword_hit_reject");

			private void OnTriggerEnter2D(Collider2D other)
			{
				if (!(other.gameObject.name != "Clash Tink") || other.gameObject.CompareTag("Nail Attack"))
				{
					this.PlayAudio(TinkClip, 1f, 0.15f);
				}
			}
		}

		private class IndicatorDetectorControl : MonoBehaviour
		{
			public GameObject particlerocks;

			public GameObject dust;

			private void OnTriggerEnter2D(Collider2D collision)
			{
				if (collision.gameObject.layer == 9)
				{
					Log("HERO ENTERED");
					base.gameObject.transform.parent.Find("irock").gameObject.SetActive(value: true);
					base.gameObject.transform.parent.Find("irock (1)").gameObject.SetActive(value: true);
					dust.SetActive(value: true);
					particlerocks.SetActive(value: true);
					particlerocks.GetComponent<ParticleSystem>().Emit(15);
					FiveKnights.Instance.SaveSettings.IndicatorActivated = true;
					base.gameObject.SetActive(value: false);
				}
			}
		}

		private class RockControl : MonoBehaviour
		{
			private void OnCollisionEnter2D(Collision2D collision)
			{
				if (collision.gameObject.layer == 8)
				{
					Log("Collided with " + collision.gameObject.name);
					if (base.gameObject.name == "irock (1)")
					{
						FiveKnights.Instance.SaveSettings.IndicatorPosition2 = base.gameObject.transform.localPosition;
						Vector3 indicatorPosition = FiveKnights.Instance.SaveSettings.IndicatorPosition2;
						Log("Rock 2 position = " + indicatorPosition.ToString());
					}
					else
					{
						FiveKnights.Instance.SaveSettings.IndicatorPosition1 = base.gameObject.transform.localPosition;
						Vector3 indicatorPosition = FiveKnights.Instance.SaveSettings.IndicatorPosition1;
						Log("Rock 1 position = " + indicatorPosition.ToString());
					}
				}
			}
		}

		private static string _currScene;

		public static void Hook()
		{
			On.GameManager.EnterHero += GameManagerEnterHero;
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ActiveSceneChanged;
			ModHooks.DrawBlackBordersHook += ModHooksOnDrawBlackBordersHook;
			On.TinkEffect.Start += StopScreenshake;
		}

		public static void Unhook()
		{
			On.GameManager.EnterHero -= GameManagerEnterHero;
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= ActiveSceneChanged;
			ModHooks.DrawBlackBordersHook -= ModHooksOnDrawBlackBordersHook;
		}

		private static void ActiveSceneChanged(Scene From, Scene To)
		{
			_currScene = To.name;
			if (To.name == "Abyss_09")
			{
				SetupEntranceTerrain(To);
				CreateGateway("palecourt_top1", new Vector2(209.6533f, 60f), new Vector2(4f, 1f), "Abyssal_Temple", "entry_bot1", right: true, left: false, onlyOut: false, GameManager.SceneLoadVisualizations.Default);
			}
			if (To.name == "Abyss_10")
			{
				SetupExitTerrain(To);
				CreateGateway("palecourt_top2", new Vector2(102.28f, 31.4f), new Vector2(4f, 1f), "Abyssal_Temple", "entry_bot2", right: true, left: true, onlyOut: false, GameManager.SceneLoadVisualizations.Default);
			}
			if (To.name == "Abyssal_Temple")
			{
				SetSceneSettings();
				SetupShadegates(To);
				SetupBreakables(To);
				FixHazardSpikes(To);
				SetWater();
				DreamgateLock();
				ShadeSpawn();
				SetupSoulTotems();
			}
		}

		private static void SetupSoulTotems()
		{
			Vector3[] array = new Vector3[4]
			{
				new Vector3(76.4f, 86.6f, 0.03f),
				new Vector3(198.4582f, 114.6f, 0.03f),
				new Vector3(227.6582f, 107.6f, 0.03f),
				new Vector3(106.6582f, 27.4f, 0.03f)
			};
			foreach (Vector3 vector in array)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["SoulTotem"]);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["TotemBase"]);
				gameObject.transform.position = vector;
				gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
				gameObject2.transform.position = vector + new Vector3(0.2f, -2.1f, -0.002f);
				PlayMakerFSM playMakerFSM = gameObject.LocateMyFSM("soul_totem");
				playMakerFSM.SetAttr<PlayMakerFSM, FsmTemplate>("fsmTemplate", null);
				playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.SetIntValue>("Reset").intValue = int.MaxValue;
				playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.SetIntValue>("Reset?").intValue = int.MaxValue;
				gameObject.GetComponent<SpriteRenderer>().sprite = ABManager.AssetBundles[ABManager.Bundle.Misc].LoadAsset<Sprite>("inf_totem_a");
				USceneUtil.Find(gameObject, "Dimmer").GetComponent<SpriteRenderer>().sprite = ABManager.AssetBundles[ABManager.Bundle.Misc].LoadAsset<Sprite>("inf_totem_b");
				USceneUtil.Find(gameObject, "Glower").GetComponent<SpriteRenderer>().sprite = ABManager.AssetBundles[ABManager.Bundle.Misc].LoadAsset<Sprite>("inf_totem_c");
				gameObject.SetActive(value: true);
				gameObject2.SetActive(value: true);
			}
		}

		private static void ShadeSpawn()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["ShadeSpawn"]);
			gameObject.transform.position = new Vector2(70.3f, 35.7f);
			gameObject.SetActive(value: true);
		}

		private static void SetWater()
		{
			GameObject original = FiveKnights.preloadedGO["AbyssWater1"];
			GameObject original2 = FiveKnights.preloadedGO["AbyssWater2"];
			Vector3[] array = new Vector3[2]
			{
				new Vector3(229.9012f, 31.14f, -0.396f),
				new Vector3(246.7236f, 31.14f, -0.396f)
			};
			Vector3[] array2 = new Vector3[2]
			{
				new Vector3(252f, 30.45f, 0.004f),
				new Vector3(231.6483f, 30.45f, 0.004f)
			};
			Vector3[] array3 = array;
			foreach (Vector3 position in array3)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(original);
				gameObject.transform.position = position;
				gameObject.SetActive(value: true);
			}
			array3 = array2;
			foreach (Vector3 position2 in array3)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(original2);
				gameObject2.transform.position = position2;
				gameObject2.SetActive(value: true);
			}
		}

		private static void DreamgateLock()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["DreamgateLock"]);
			gameObject.transform.localScale = new Vector3(7.3875f, 7.6309f, 0f);
			gameObject.transform.position = new Vector3(158.8776f, 117.579f, 0f);
			gameObject.SetActive(value: true);
		}

		private static void SetSceneSettings()
		{
			if (GameObject.Find("_Effects").transform.Find("Darkness Region") != null)
			{
				Transform transform = GameObject.Find("_Effects").transform.Find("Darkness Region");
				GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["DReg"]);
				BoxCollider2D component = transform.GetComponent<BoxCollider2D>();
				BoxCollider2D component2 = gameObject.GetComponent<BoxCollider2D>();
				component2.size = component.size;
				component2.offset = component.offset;
				gameObject.transform.position = transform.transform.position;
				gameObject.transform.localScale = transform.transform.localScale;
				gameObject.SetActive(value: true);
				UnityEngine.Object.Destroy(transform);
			}
			SceneManager[] array = UnityEngine.Object.FindObjectsOfType<SceneManager>();
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object.Destroy(array[i].gameObject);
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["AbyssSM"]);
			SceneManager component3 = gameObject2.GetComponent<SceneManager>();
			component3.noLantern = false;
			component3.darknessLevel = 1;
			component3.isWindy = false;
			component3.sceneType = SceneType.GAMEPLAY;
			component3.saturation = 0.5f;
			component3.defaultIntensity = 0.6f;
			component3.defaultColor = new Color(1f, 31f / 34f, 53f / 68f, 1f);
			component3.heroLightColor = new Color(1f, 0.9472616f, 0.8529412f, 0.3843137f);
			component3.mapZone = MapZone.ABYSS;
			component3.noParticles = false;
			component3.overrideParticlesWith = MapZone.NONE;
			component3.environmentType = 0;
			component3.ignorePlatformSaturationModifiers = false;
			gameObject2.SetActive(value: true);
		}

		private static void ModHooksOnDrawBlackBordersHook(List<GameObject> obj)
		{
			if (!(_currScene == "Abyssal_Temple"))
			{
				return;
			}
			foreach (GameObject item in obj)
			{
				item.SetActive(value: false);
			}
		}

		private static void SetupShadegates(Scene scene)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["ShadeGate"]);
			foreach (Transform item in scene.Find("SpikyGates").transform)
			{
				GameObject gameObject2 = item.gameObject;
				float z = gameObject2.transform.rotation.eulerAngles.z;
				gameObject2.GetComponent<AudioSource>().outputAudioMixerGroup = gameObject.GetComponent<AudioSource>().outputAudioMixerGroup;
				GameObject gameObject3 = UnityEngine.Object.Instantiate(USceneUtil.Find(gameObject, "Slash Effect"));
				gameObject3.name = "Slash Effect";
				gameObject3.transform.parent = gameObject2.transform;
				gameObject3.transform.position = ((z == 0f) ? (gameObject2.transform.position + new Vector3(3.2f, 0f, 0f)) : USceneUtil.Find(gameObject2, "shade_gate_0001_shadegate_front").transform.position);
				gameObject3.transform.SetScaleY(USceneUtil.Find(gameObject2, "solid").transform.GetScaleX() * 0.8f);
				PlayMakerFSM playMakerFSM = gameObject3.LocateMyFSM("Control");
				if (z == 0f)
				{
					gameObject3.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
					FsmState state = playMakerFSM.GetState("Effect");
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.GetPosition>(playMakerFSM.GetState("Init"), 2).y = playMakerFSM.FindFloatVariable("Self X");
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.CheckTargetDirection>(state, 7).aboveEvent = playMakerFSM.FsmEvents[2];
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.CheckTargetDirection>(state, 7).belowEvent = playMakerFSM.FsmEvents[3];
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.CheckTargetDirection>(state, 7).aboveEvent = null;
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.CheckTargetDirection>(state, 7).aboveEvent = null;
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.GetPosition>(state, 2).x = playMakerFSM.FindFloatVariable("Hero Y");
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.GetPosition>(state, 2).y = 0f;
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.SetPosition>(state, 5).x = playMakerFSM.FindFloatVariable("Hero Y");
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.SetPosition>(state, 5).y = 0f;
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.SetPosition>(state, 6).x = playMakerFSM.FindFloatVariable("Hero Y");
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.SetPosition>(state, 6).y = 0f;
					state.RemoveActionsOfType<HutongGames.PlayMaker.Actions.SendEventByName>();
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.SetPosition>(playMakerFSM.GetState("L"), 2).y = playMakerFSM.FindFloatVariable("Self X");
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.SetPosition>(playMakerFSM.GetState("L"), 2).x = playMakerFSM.FindFloatVariable("Hero Y");
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.SetPosition>(playMakerFSM.GetState("R"), 2).y = playMakerFSM.FindFloatVariable("Self X");
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.SetPosition>(playMakerFSM.GetState("R"), 2).x = playMakerFSM.FindFloatVariable("Hero Y");
					playMakerFSM.GetState("L").RemoveFsmAction(3);
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.SendMessage>(playMakerFSM.GetState("R"), 3).functionCall.FunctionName = "RecoilDown";
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.SetPosition>(playMakerFSM.GetState("Pause"), 3).y = playMakerFSM.FindFloatVariable("Self X");
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.SetPosition>(playMakerFSM.GetState("Pause"), 3).x = playMakerFSM.FindFloatVariable("Hero Y");
				}
				if (gameObject2.name == "spiky_gate_12_L")
				{
					playMakerFSM.GetState("R").RemoveActionsOfType<HutongGames.PlayMaker.Actions.SendMessage>();
				}
				if (gameObject2.name == "spiky_gate_12_R")
				{
					playMakerFSM.GetState("L").RemoveActionsOfType<HutongGames.PlayMaker.Actions.SendMessage>();
				}
				gameObject3.SetActive(value: true);
				GameObject gameObject4 = UnityEngine.Object.Instantiate(USceneUtil.Find(gameObject, "Dash Effect"));
				gameObject4.name = "Dash Effect";
				gameObject4.transform.parent = gameObject2.transform;
				gameObject4.transform.position = ((z == 0f) ? (gameObject2.transform.position + new Vector3(3.2f, 0f, 0f)) : USceneUtil.Find(gameObject2, "shade_gate_0001_shadegate_front").transform.position);
				gameObject4.transform.SetScaleY(USceneUtil.Find(gameObject2, "solid").transform.GetScaleX() * 0.8f);
				if (z == 0f)
				{
					gameObject4.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
					PlayMakerFSM fsm = gameObject4.LocateMyFSM("Control");
					FsmState state2 = fsm.GetState("Effect");
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.GetPosition>(state2, 3).x = fsm.FindFloatVariable("Hero Y");
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.GetPosition>(state2, 3).y = 0f;
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.SetPosition>(state2, 7).x = fsm.FindFloatVariable("Hero Y");
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.SetPosition>(state2, 7).y = 0f;
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.SetPosition>(state2, 8).x = fsm.FindFloatVariable("Hero Y");
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.SetPosition>(state2, 8).y = 0f;
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.SetPosition>(state2, 9).x = fsm.FindFloatVariable("Hero Y");
					SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.SetPosition>(state2, 9).y = 0f;
					state2.RemoveFsmAction(4);
				}
				gameObject4.SetActive(value: true);
				GameObject gameObject5 = UnityEngine.Object.Instantiate(USceneUtil.Find(gameObject, "Particle System"));
				gameObject5.name = "Particle System";
				gameObject5.transform.parent = gameObject2.transform;
				gameObject5.transform.position = ((z == 0f) ? (gameObject2.transform.position + new Vector3(3.2f, 0f, 0f)) : USceneUtil.Find(gameObject2, "shade_gate_0001_shadegate_front").transform.position);
				if (z != 0f)
				{
					gameObject5.transform.SetScaleY(USceneUtil.Find(gameObject2, "solid").transform.GetScaleX() * 0.8f);
				}
				else
				{
					gameObject5.transform.SetScaleX(USceneUtil.Find(gameObject2, "solid").transform.GetScaleX() * 0.8f);
				}
				if (gameObject2.transform.rotation.eulerAngles.z != 0f)
				{
					gameObject5.transform.rotation = Quaternion.Euler(0f, 0f, 270f);
				}
				else
				{
					gameObject5.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
				}
				gameObject5.SetActive(value: true);
			}
		}

		private static void SetupBreakables(Scene scene)
		{
			foreach (Transform item in scene.Find("Breakable").transform)
			{
				if (item.GetComponent<BreakableObject>() != null)
				{
					Log("Found: " + item.name);
					GameObject gameObject = item.gameObject;
					string[] array = item.name.Split('_');
					if (array[0] == "ruin")
					{
						GameObject gameObject2 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["BreakableFossil"]);
						gameObject2.transform.position = gameObject.transform.position;
						gameObject2.transform.localScale = gameObject.transform.localScale;
						gameObject2.transform.rotation = gameObject.transform.rotation;
						gameObject2.SetActive(value: true);
						UnityEngine.Object.Destroy(gameObject);
					}
					if (array[0] == "tutorial")
					{
						GameObject gameObject3 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["BreakableClawPole"]);
						gameObject3.transform.position = gameObject.transform.position;
						gameObject3.transform.localScale = gameObject.transform.localScale;
						gameObject3.transform.rotation = gameObject.transform.rotation;
						gameObject3.SetActive(value: true);
						UnityEngine.Object.Destroy(gameObject);
					}
				}
				if (item.GetComponent<GrassBehaviour>() != null)
				{
					Log("Found: " + item.name);
					GameObject gameObject4 = item.gameObject;
					GameObject gameObject5 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["BreakableVines"]);
					gameObject5.transform.position = gameObject4.transform.position;
					gameObject5.transform.localScale = gameObject4.transform.localScale;
					gameObject5.transform.rotation = gameObject4.transform.rotation;
					gameObject5.SetActive(value: true);
					UnityEngine.Object.Destroy(gameObject4);
				}
			}
		}

		public static IEnumerable<TComponent> GetObjectsOfType<TComponent>(this Scene scene) where TComponent : Component
		{
			return from x in scene.GetRootGameObjects()
				where x.GetComponent<TComponent>() != null
				select x.GetComponent<TComponent>();
		}

		private static void FixHazardSpikes(Scene scene)
		{
			foreach (Transform item in scene.Find("Spikes").transform)
			{
				item.gameObject.layer = 17;
				item.gameObject.AddComponent<DamageHero>().damageDealt = 1;
				item.gameObject.GetComponent<DamageHero>().hazardType = 2;
				item.gameObject.AddComponent<TinkEffect>().blockEffect = ABManager.AssetBundles[ABManager.Bundle.OWArenaDep].LoadAsset<GameObject>("Block Hit V2");
				item.gameObject.AddComponent<NoShakeTink>();
			}
		}

		private static void StopScreenshake(On.TinkEffect.orig_Start orig, TinkEffect self)
		{
			if (GameManager.instance.GetSceneNameString() != "Abyssal_Temple")
			{
				orig(self);
			}
		}

		private static void SetupEntranceTerrain(Scene scene)
		{
			PolygonCollider2D component = scene.Find("Roof Collider top right").GetComponent<PolygonCollider2D>();
			Vector2[] array = new Vector2[50];
			array[21] = new Vector2(35.2028f, -18.6f);
			array[22] = new Vector2(39.363f, -18.6f);
			int num = -1;
			Vector2[] points = component.points;
			foreach (Vector2 vector in points)
			{
				num = ((num != 20) ? (num + 1) : 23);
				if (num == 23)
				{
					array[num] = new Vector2(39.363f, -21.2158f);
				}
				else
				{
					array[num] = vector;
				}
			}
			component.CreatePrimitive(50);
			component.points = array;
			component.SetPath(0, array);
			UnityEngine.Object.Destroy(USceneUtil.Find(USceneUtil.Find(scene.Find("TileMap Render Data"), "Scenemap"), "Chunk 1 6").GetComponents<EdgeCollider2D>()[0]);
			GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["BreakableCeiling"]);
			gameObject.transform.position = new Vector3(209.5f, 59f, 0f);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			UnityEngine.Object.Destroy(USceneUtil.Find(gameObject, "Particle System"));
			gameObject.SetActive(value: true);
			scene.Find("frame collider (1)").layer = 8;
			GameObject gameObject2 = UnityEngine.Object.Instantiate(ABManager.AssetBundles[ABManager.Bundle.Misc].LoadAsset<GameObject>("iholder"));
			GameObject gameObject3 = USceneUtil.Find(gameObject2, "iherodetector");
			GameObject gameObject4 = UnityEngine.Object.Instantiate(USceneUtil.Find(gameObject, "Particle_rocks_small"), gameObject2.transform);
			GameObject gameObject5 = USceneUtil.Find(gameObject2, "irock");
			GameObject gameObject6 = USceneUtil.Find(gameObject2, "irock (1)");
			GameObject gameObject7 = UnityEngine.Object.Instantiate(gameObject.LocateMyFSM("breakable_wall_v2").GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>("Hit Up").gameObject.Value, gameObject2.transform);
			gameObject2.SetActive(value: false);
			gameObject2.transform.position = new Vector2(209.5f, 58.5f);
			gameObject4.transform.position = gameObject2.transform.position;
			gameObject3.transform.position = new Vector2(212f, 53f);
			gameObject7.transform.position = gameObject2.transform.position;
			gameObject7.transform.rotation = Quaternion.Euler(90f, 180f, 0f);
			gameObject7.GetComponent<ParticleSystem>().gravityModifier = 10f;
			gameObject7.SetActive(value: false);
			gameObject3.AddComponent<IndicatorDetectorControl>().particlerocks = gameObject4;
			gameObject3.GetComponent<IndicatorDetectorControl>().dust = gameObject7;
			gameObject5.AddComponent<RockControl>();
			gameObject6.AddComponent<RockControl>();
			gameObject4.SetActive(value: false);
			gameObject2.SetActive(value: true);
			if (FiveKnights.Instance.SaveSettings.IndicatorActivated)
			{
				gameObject3.SetActive(value: false);
				gameObject5.transform.localPosition = FiveKnights.Instance.SaveSettings.IndicatorPosition1;
				gameObject6.transform.localPosition = FiveKnights.Instance.SaveSettings.IndicatorPosition2;
				gameObject5.SetActive(value: true);
				gameObject6.SetActive(value: true);
			}
		}

		private static void SetupExitTerrain(Scene scene)
		{
			PolygonCollider2D component = scene.Find("Roof Collider (1)").GetComponent<PolygonCollider2D>();
			Vector2[] array = new Vector2[37];
			int num = 0;
			Vector2[] points = component.points;
			foreach (Vector2 vector in points)
			{
				if (num == 21 || num == 22 || num == 23 || num == 24)
				{
					if (num == 21)
					{
						array[21] = new Vector2(36.3528f, -5.6227f);
					}
					if (num == 22)
					{
						array[22] = new Vector2(36.3528f, -0.4f);
					}
					if (num == 23)
					{
						array[23] = new Vector2(40.3528f, -0.4f);
					}
					if (num == 24)
					{
						array[24] = new Vector2(40.3528f, -5.2621f);
					}
					num++;
				}
				else
				{
					array[num] = vector;
					num++;
				}
			}
			component.points = array;
			component.SetPath(0, array);
			GameObject gameObject = USceneUtil.Find(USceneUtil.Find(scene.Find("TileMap Render Data"), "Scenemap"), "Chunk 0 3");
			Vector2[] array2 = new Vector2[26];
			int num2 = 0;
			points = gameObject.GetComponent<EdgeCollider2D>().points;
			foreach (Vector2 vector2 in points)
			{
				if (num2 == 4 || num2 == 5)
				{
					if (num2 == 4)
					{
						array2[4] = new Vector2(8.29f, 32f);
					}
					if (num2 == 5)
					{
						array2[5] = new Vector2(8.29f, 28f);
					}
					num2++;
				}
				else
				{
					array2[num2] = vector2;
					num2++;
				}
			}
			gameObject.GetComponent<EdgeCollider2D>().points = array2;
			GameObject gameObject2 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["CollapseFloor"]);
			gameObject2.transform.position = new Vector3(99.58f, 28.5f);
			UnityEngine.Object.Destroy(USceneUtil.Find(gameObject2, "floor1"));
			UnityEngine.Object.Destroy(USceneUtil.Find(gameObject2, "floor2"));
			gameObject2.gameObject.SetActive(value: true);
		}

		private static void CreateGateway(string gateName, Vector2 pos, Vector2 size, string toScene, string entryGate, bool right, bool left, bool onlyOut, GameManager.SceneLoadVisualizations vis)
		{
			GameObject gameObject = new GameObject(gateName);
			gameObject.transform.SetPosition2D(pos);
			TransitionPoint transitionPoint = gameObject.AddComponent<TransitionPoint>();
			if (!onlyOut)
			{
				BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
				boxCollider2D.size = size;
				boxCollider2D.isTrigger = true;
				transitionPoint.targetScene = toScene;
				transitionPoint.entryPoint = entryGate;
			}
			transitionPoint.alwaysEnterLeft = left;
			transitionPoint.alwaysEnterRight = right;
			GameObject gameObject2 = new GameObject("Hazard Respawn Marker");
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.tag = "RespawnPoint";
			gameObject2.transform.SetPosition2D(pos);
			transitionPoint.respawnMarker = gameObject2.AddComponent<HazardRespawnMarker>();
			transitionPoint.sceneLoadVisualization = vis;
		}

		private static void GameManagerEnterHero(On.GameManager.orig_EnterHero orig, GameManager self, bool additiveGateSearch)
		{
			if (self.sceneName == "Abyssal_Temple")
			{
				self.sceneWidth = (self.tilemap.width = 1000);
				self.sceneHeight = (self.tilemap.height = 1000);
			}
			orig(self, additiveGateSearch: false);
		}

		private static void Log(object o)
		{
			Modding.Logger.Log("[AbyssalTemple] " + o);
		}
	}
	public static class AreaTitleCtrl
	{
		public static void ShowBossTitle(MonoBehaviour owner, GameObject areaTitleObject, float hideInSeconds, string largeMain = "", string largeSuper = "", string largeSub = "", string smallMain = "", string smallSuper = "", string smallSub = "")
		{
			areaTitleObject.LocateMyFSM("Area Title Control").enabled = false;
			areaTitleObject.SetActive(value: true);
			areaTitleObject.transform.Find("Title Large").gameObject.SetActive(value: false);
			areaTitleObject.transform.Find("Title Small").gameObject.SetActive(value: true);
			FadeGroup[] componentsInChildren = areaTitleObject.GetComponentsInChildren<FadeGroup>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].FadeUp();
			}
			areaTitleObject.FindGameObjectInChildren("Title Small Main").transform.position = new Vector3(-9.7f, -6.3f, -0.1f);
			areaTitleObject.FindGameObjectInChildren("Title Small Sub").transform.position = new Vector3(-9.7f, -7.2f, -0.1f);
			areaTitleObject.FindGameObjectInChildren("Title Small Super").transform.position = new Vector3(-9.7f, -5.2f, -0.1f);
			areaTitleObject.FindGameObjectInChildren("Title Small Main").GetComponent<TextMeshPro>().text = smallMain;
			areaTitleObject.FindGameObjectInChildren("Title Small Sub").GetComponent<TextMeshPro>().text = smallSub;
			areaTitleObject.FindGameObjectInChildren("Title Small Super").GetComponent<TextMeshPro>().text = smallSuper;
			areaTitleObject.FindGameObjectInChildren("Title Large Main").GetComponent<TextMeshPro>().text = largeMain;
			areaTitleObject.FindGameObjectInChildren("Title Large Sub").GetComponent<TextMeshPro>().text = largeSub;
			areaTitleObject.FindGameObjectInChildren("Title Large Super").GetComponent<TextMeshPro>().text = largeSuper;
			if (!((double)hideInSeconds <= 0.0))
			{
				owner.StartCoroutine(HideBossTitleAfter(areaTitleObject, hideInSeconds + 3f));
			}
		}

		private static IEnumerator HideBossTitleAfter(GameObject areaTitleObject, float time)
		{
			yield return new WaitForSeconds(time);
			HideBossTitle(areaTitleObject);
			yield return new WaitForSeconds(3f);
			areaTitleObject.SetActive(value: false);
			areaTitleObject.LocateMyFSM("Area Title Control").enabled = true;
			areaTitleObject.FindGameObjectInChildren("Title Small Main").transform.Translate(new Vector3(-4f, 0f, 0f));
			areaTitleObject.FindGameObjectInChildren("Title Small Sub").transform.Translate(new Vector3(-4f, 0f, 0f));
			areaTitleObject.FindGameObjectInChildren("Title Small Super").transform.Translate(new Vector3(-4f, 0f, 0f));
		}

		private static void HideBossTitle(GameObject areaTitleObject)
		{
			FadeGroup[] componentsInChildren = areaTitleObject.GetComponentsInChildren<FadeGroup>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].FadeDown();
			}
		}
	}
	public static class AudioSourceExtensions
	{
		private static GameObject _fc = FiveKnights.preloadedGO["fk"];

		private static GameObject _lk = FiveKnights.preloadedGO["Kin"];

		private static GameObject _pv = FiveKnights.preloadedGO["PV"];

		private static GameObject _wd = FiveKnights.preloadedGO["WD"];

		private static PlayMakerFSM _fCtrl = _fc.LocateMyFSM("FalseyControl");

		private static PlayMakerFSM _ikCtrl = _lk.LocateMyFSM("IK Control");

		private static PlayMakerFSM _pvCtrl = _pv.LocateMyFSM("Control");

		private static PlayMakerFSM _dd = _wd.LocateMyFSM("Dung Defender");

		public static void Play(this AudioSource audio, string clipName, float pitchMin = 1f, float pitchMax = 1f, float time = 0f)
		{
			audio.pitch = UnityEngine.Random.Range(pitchMin, pitchMax);
			audio.time = time;
			audio.PlayOneShot(GetAudioClip());
			AudioClip GetAudioClip()
			{
				return clipName switch
				{
					"Counter" => (AudioClip)_pvCtrl.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Counter Stance", 1).audioClip.Value, 
					"Dash" => (AudioClip)_pvCtrl.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Dash", 1).audioClip.Value, 
					"Dive" => (AudioClip)_ikCtrl.GetAction<HutongGames.PlayMaker.Actions.AudioPlaySimple>("Dstab Fall", 0).oneShotClip.Value, 
					"Dung Pillar" => (AudioClip)_dd.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Pillar", 0).audioClip.Value, 
					"Heavy Land" => (AudioClip)_ikCtrl.GetAction<HutongGames.PlayMaker.Actions.AudioPlaySimple>("Dstab Land", 0).oneShotClip.Value, 
					"Jump" => (AudioClip)_ikCtrl.GetAction<HutongGames.PlayMaker.Actions.AudioPlaySimple>("Jump").oneShotClip.Value, 
					"Light Land" => (AudioClip)_ikCtrl.GetAction<HutongGames.PlayMaker.Actions.AudioPlaySimple>("Land", 0).oneShotClip.Value, 
					"Mace Slam" => (AudioClip)_fCtrl.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Slam", 1).audioClip.Value, 
					"Mace Swing" => (AudioClip)_fCtrl.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("S Attack", 0).audioClip.Value, 
					"Slash" => (AudioClip)_pvCtrl.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Slash1", 1).audioClip.Value, 
					_ => null, 
				};
			}
		}
	}
	internal class AutoDestroy : MonoBehaviour
	{
		public Func<bool> ShouldDestroy;

		internal float Time { get; set; } = 0.7f;

		private void Start()
		{
			StartCoroutine(Wait());
		}

		private IEnumerator Wait()
		{
			if (ShouldDestroy != null)
			{
				yield return new WaitWhile(() => !ShouldDestroy());
			}
			else
			{
				yield return new WaitForSeconds(Time);
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
	public class CollisionCheck : MonoBehaviour
	{
		private Rigidbody2D _rb;

		public bool Hit { get; set; }

		public bool Freeze { get; set; }

		public event Action OnCollide;

		private void Awake()
		{
			_rb = GetComponent<Rigidbody2D>() ?? base.transform.parent.GetComponent<Rigidbody2D>();
		}

		private void Update()
		{
			if (!Hit && !(_rb.velocity != Vector2.zero))
			{
				OnCollide?.Invoke();
				Hit = true;
			}
		}

		private void OnTriggerEnter2D(Collider2D col)
		{
			if (col.gameObject.layer == 8)
			{
				Hit = true;
				OnCollide?.Invoke();
				if (Freeze)
				{
					_rb.velocity = Vector2.zero;
				}
			}
		}

		private void OnTriggerStay2D(Collider2D col)
		{
			if (col.gameObject.layer == 8)
			{
				Hit = true;
			}
		}
	}
	public static class Credits
	{
		public static void Hook()
		{
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ActiveSceneChanged;
			On.GameManager.IsNonGameplayScene += GameManagerIsNonGameplayScene;
		}

		private static void ActiveSceneChanged(Scene arg0, Scene arg1)
		{
			if (arg1.name == "Pale_Court_Credits")
			{
				GameObject.Find("Credits Controller").AddComponent<CreditsController>();
			}
		}

		private static bool GameManagerIsNonGameplayScene(On.GameManager.orig_IsNonGameplayScene orig, GameManager self)
		{
			if (!orig(self))
			{
				return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Pale_Court_Credits";
			}
			return true;
		}
	}
	public class CreditsController : MonoBehaviour
	{
		private readonly float ScrollSpeed = 330f * ((float)Screen.height / 2400f);

		private readonly float ScrollY = 17500f * ((float)Screen.height / 2400f);

		private Transform _creditsParent;

		private Transform _scrollParent;

		private Transform _thanksParent;

		private Transform _finalParent;

		private AudioSource _creditsAudio;

		private Coroutine _creditsCoro;

		private Coroutine _finalFadeCoro;

		private Coroutine _returnCoro;

		private bool _scrolling;

		private void Start()
		{
			GameManager.instance.SetState(GameState.CUTSCENE);
			UIManager.instance.SetState(UIState.CUTSCENE);
			InputHandler.Instance.PreventPause();
			_creditsParent = GameObject.Find("Credits Parent").transform;
			_scrollParent = GameObject.Find("Scroll Parent").transform;
			_thanksParent = GameObject.Find("Thanks Parent").transform;
			_finalParent = GameObject.Find("Final Parent").transform;
			_creditsAudio = GameObject.Find("Credits Audio").GetComponent<AudioSource>();
			FixFonts();
			FixTranslations();
			StopAudio();
			_creditsCoro = StartCoroutine(RollCredits());
		}

		private void Update()
		{
			if (_scrolling)
			{
				if (_scrollParent.position.y < ScrollY)
				{
					_scrollParent.Translate(Vector3.up * Time.deltaTime * ScrollSpeed);
				}
				else
				{
					_scrolling = false;
				}
			}
		}

		private void FixFonts()
		{
			Text[] componentsInChildren = _creditsParent.parent.GetComponentsInChildren<Text>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].font = CanvasUtil.GetFont("Perpetua");
			}
		}

		private void FixTranslations()
		{
			Text[] componentsInChildren = _creditsParent.parent.GetComponentsInChildren<Text>(includeInactive: true);
			foreach (Text text in componentsInChildren)
			{
				GameObject gameObject = text.gameObject;
				string text2 = "";
				text2 = ((!gameObject.name.Contains("Body")) ? (gameObject.transform.parent.gameObject.name + "-" + gameObject.name) : (gameObject.transform.parent.parent.gameObject.name + "-" + gameObject.transform.parent.gameObject.name + "-" + gameObject.name));
				text.text = global::Language.Language.Get(text2, "PaleCourtCredits");
				if (text.text == "")
				{
					text.text = "\"" + text2 + "\" - \"PaleCourtCredits\"";
				}
			}
		}

		private void StopAudio()
		{
			MusicCue musicCue = ScriptableObject.CreateInstance<MusicCue>();
			MusicCue.MusicChannelInfo[] value = new MusicCue.MusicChannelInfo[6]
			{
				new MusicCue.MusicChannelInfo(),
				null,
				null,
				null,
				null,
				null
			};
			Mirror.SetField(musicCue, "channelInfos", value);
			Resources.FindObjectsOfTypeAll<AudioMixer>().First((AudioMixer x) => x.name == "Music").FindSnapshot("Main Only")
				.TransitionTo(0f);
			GameManager.instance.AudioManager.ApplyMusicCue(musicCue, 0f, 0f, applySnapshot: false);
		}

		private IEnumerator RollCredits()
		{
			Log("Starting credits sequence");
			Log("Screen height is " + Screen.height);
			Log("Screen resolution height is " + Screen.currentResolution.height);
			yield return new WaitForSeconds(1f);
			if (FiveKnights.Instance.SaveSettings.HasSeenCredits)
			{
				_returnCoro = StartCoroutine(ReturnFromCredits());
			}
			_creditsAudio.clip = ABManager.AssetBundles[ABManager.Bundle.Sound].LoadAsset<AudioClip>("CreditsMusic");
			_creditsAudio.outputAudioMixerGroup = Mirror.GetField<AudioManager, AudioSource[]>(GameManager.instance.AudioManager, "musicSources")[0].outputAudioMixerGroup;
			_creditsAudio.Play();
			Log("Fade main credits");
			for (int i = 0; i < _creditsParent.childCount; i++)
			{
				yield return FadeInOut(_creditsParent.GetChild(i).gameObject, 1f, 5.5f, 1f);
				yield return new WaitForSeconds(0.5f);
			}
			Log("Scroll credits");
			_scrolling = true;
			yield return new WaitWhile(() => _scrolling);
			Log("Fade thank yous");
			for (int i = 0; i < _thanksParent.childCount; i++)
			{
				yield return FadeInOut(_thanksParent.GetChild(i).gameObject, 1f, 6.5f, 1f);
				yield return new WaitForSeconds(0.5f);
			}
			Log("Fade final");
			SetAlpha(_finalParent.GetChild(0).gameObject.GetComponent<Text>(), 0f);
			_finalParent.GetChild(0).gameObject.SetActive(value: true);
			yield return Fade(_finalParent.GetChild(0).gameObject, 1f, fadeIn: true);
			yield return new WaitForSeconds(3f);
			SetAlpha(_finalParent.GetChild(1).gameObject.GetComponent<Text>(), 0f);
			_finalParent.GetChild(1).gameObject.SetActive(value: true);
			_finalFadeCoro = StartCoroutine(Fade(_finalParent.GetChild(1).gameObject, 1f, fadeIn: true));
			if (_returnCoro == null)
			{
				StartCoroutine(ReturnFromCredits());
			}
		}

		private IEnumerator ReturnFromCredits()
		{
			yield return new WaitUntil(() => InputHandler.Instance.inputActions.jump.IsPressed);
			Log("Ending credits sequence, going to reward room");
			if (_creditsCoro != null)
			{
				StopCoroutine(_creditsCoro);
			}
			if (_finalFadeCoro != null)
			{
				StopCoroutine(_finalFadeCoro);
			}
			FiveKnights.Instance.SaveSettings.HasSeenCredits = true;
			StartCoroutine(Fade(_finalParent.parent.gameObject, 1f, fadeIn: false));
			StartCoroutine(FadeAudio(2f));
			yield return new WaitForSeconds(2f);
			HeroController.instance.EnterWithoutInput(flag: true);
			GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
			{
				SceneName = "hidden_reward_room",
				EntryGateName = "door1",
				Visualization = GameManager.SceneLoadVisualizations.Default,
				WaitForSceneTransitionCameraFade = false,
				PreventCameraFadeOut = true,
				EntryDelay = 0f,
				HeroLeaveDirection = GatePosition.door
			});
			InputHandler.Instance.AllowPause();
		}

		private IEnumerator FadeInOut(GameObject go, float timeUp, float timeStay, float timeDown)
		{
			Text[] componentsInChildren = go.GetComponentsInChildren<Text>(includeInactive: true);
			foreach (Text t in componentsInChildren)
			{
				SetAlpha(t, 0f);
			}
			Image[] componentsInChildren2 = go.GetComponentsInChildren<Image>(includeInactive: true);
			foreach (Image t2 in componentsInChildren2)
			{
				SetAlpha(t2, 0f);
			}
			go.SetActive(value: true);
			yield return Fade(go, timeUp, fadeIn: true);
			yield return new WaitForSeconds(timeStay);
			yield return Fade(go, timeDown, fadeIn: false);
			go.SetActive(value: false);
		}

		private IEnumerator Fade(GameObject go, float time, bool fadeIn)
		{
			float alphaChangeCounter = 0f;
			Graphic[] first = go.GetComponentsInChildren<Text>(includeInactive: true).Cast<Graphic>().ToArray();
			Graphic[] second = go.GetComponentsInChildren<Image>(includeInactive: true).Cast<Graphic>().ToArray();
			Graphic[] graphics = first.Concat(second).ToArray();
			while (alphaChangeCounter < 1f)
			{
				Graphic[] array = graphics;
				foreach (Graphic graphic in array)
				{
					if (fadeIn)
					{
						SetAlpha(graphic, graphic.color.a + Time.deltaTime / time);
					}
					else
					{
						SetAlpha(graphic, graphic.color.a - Time.deltaTime / time);
					}
				}
				alphaChangeCounter += Time.deltaTime / time;
				yield return null;
			}
		}

		private IEnumerator FadeAudio(float time)
		{
			float changeCounter = 0f;
			while (changeCounter < 1f)
			{
				_creditsAudio.volume -= Time.deltaTime / time;
				yield return null;
			}
		}

		private void SetAlpha(Graphic t, float a)
		{
			t.color = new Color(t.color.r, t.color.g, t.color.b, a);
		}

		private void Log(object o)
		{
			Modding.Logger.Log("[Credits Controller] " + o);
		}
	}
	internal static class EnemyHPBarImport
	{
		[ModImportName("EnemyHPBar")]
		internal static class EnemyHPBar
		{
			public static Action<GameObject> DisableHPBar;

			public static Action<GameObject> EnableHPBar;

			public static Action<GameObject> RefreshHPBar;

			public static Action<GameObject> MarkAsBoss;
		}

		public static void DisableHPBar(GameObject go)
		{
			EnemyHPBar.DisableHPBar?.Invoke(go);
		}

		public static void EnableHPBar(GameObject go)
		{
			EnemyHPBar.EnableHPBar?.Invoke(go);
		}

		public static void RefreshHPBar(GameObject go)
		{
			EnemyHPBar.RefreshHPBar?.Invoke(go);
		}

		public static void MarkAsBoss(GameObject go)
		{
			EnemyHPBar.MarkAsBoss?.Invoke(go);
		}

		static EnemyHPBarImport()
		{
			typeof(EnemyHPBar).ModInterop();
		}
	}
	public static class Extensions
	{
		public static bool IsPlaying(this Animator anim)
		{
			return anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f;
		}

		public static bool IsPlaying(this Animator anim, string s)
		{
			if (anim.GetCurrentAnimatorStateInfo(0).IsName(s))
			{
				return anim.IsPlaying();
			}
			return false;
		}

		public static int GetCurrentFrame(this Animator anim)
		{
			if (anim.GetCurrentAnimatorClipInfo(0).Length == 0)
			{
				Modding.Logger.Log("Warning: Could not find animator " + anim.name + " clip in GetCurrentFrame.");
				return 999999;
			}
			AnimatorClipInfo animatorClipInfo = anim.GetCurrentAnimatorClipInfo(0)[0];
			return (int)(anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f * (animatorClipInfo.clip.length * animatorClipInfo.clip.frameRate));
		}

		public static void PlayAt(this Animator anim, string name, int frame)
		{
			if (FiveKnights.AnimClips.ContainsKey(name))
			{
				AnimationClip animationClip = FiveKnights.AnimClips[name];
				float normalizedTime = (float)frame / (animationClip.length * animationClip.frameRate);
				anim.Play(name, -1, normalizedTime);
			}
			else
			{
				Modding.Logger.Log("WARNING: Could not find aninmation clip, reverting to old method.");
				AnimatorClipInfo animatorClipInfo = anim.GetCurrentAnimatorClipInfo(0)[0];
				float normalizedTime2 = (float)frame / (animatorClipInfo.clip.length * animatorClipInfo.clip.frameRate);
				anim.Play(name, 0, normalizedTime2);
			}
		}

		[Pure]
		public static IEnumerator PlayBlocking(this Animator self, string anim)
		{
			self.Play(anim, -1, 0f);
			yield return new WaitForEndOfFrame();
			while (self.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f)
			{
				yield return null;
			}
		}

		[Pure]
		public static IEnumerator PlayToFrame(this Animator self, string anim, int frame)
		{
			self.Play(anim, -1, 0f);
			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(() => self.GetCurrentFrame() < frame);
		}

		[Pure]
		public static IEnumerator PlayToFrameAt(this Animator self, string anim, int start, int frame)
		{
			self.PlayAt(anim, start);
			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(() => self.GetCurrentFrame() < frame);
		}

		[Pure]
		public static IEnumerator WaitToFrame(this Animator self, int frame)
		{
			yield return new WaitForEndOfFrame();
			while (self.GetCurrentFrame() < frame)
			{
				yield return null;
			}
		}

		[Pure]
		public static IEnumerator PlayToEnd(this Animator self, string name)
		{
			self.Play(name, -1, 0f);
			yield return new WaitForEndOfFrame();
			while (self.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f)
			{
				yield return null;
			}
		}

		[Pure]
		public static IEnumerator PlayToEnd(this Animator self)
		{
			yield return new WaitForEndOfFrame();
			while (self.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f)
			{
				yield return null;
			}
		}

		[Pure]
		public static IEnumerator WaitForFramesWithActions(this Animator self, params (int frame, Action act)[] acts)
		{
			yield return new WaitForEndOfFrame();
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
			yield return new WaitForEndOfFrame();
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
			yield return new WaitForEndOfFrame();
			while (predicate())
			{
				yield return null;
			}
		}

		public static bool Within(this float self, float rhs, float threshold)
		{
			return Math.Abs(self - rhs) <= threshold;
		}

		public static bool RecordWithoutNotes(this EnemyDeathEffects deathEffects, bool withoutnotes = true, int defaultkills = 2)
		{
			try
			{
				string intName = "kills" + ReflectionHelper.GetField<EnemyDeathEffects, string>(deathEffects, "playerDataName");
				int num = PlayerData.instance.GetInt(intName);
				if (num > 0)
				{
					if (withoutnotes)
					{
						PlayerData.instance.SetInt(intName, defaultkills);
					}
					else
					{
						PlayerData.instance.SetInt(intName, 1);
					}
				}
				deathEffects.RecordJournalEntry();
				return (num > 0) & withoutnotes;
			}
			catch (Exception message)
			{
				FiveKnights.Instance.Log(message);
				return withoutnotes;
			}
		}

		public static bool RecordWithoutNotes(this JournalHelper entry, bool withoutnotes = true, int defaultkills = 2)
		{
			try
			{
				string intName = "kills" + entry.GetEntryName();
				int num = PlayerData.instance.GetInt(intName);
				if (num > 0)
				{
					if (withoutnotes)
					{
						PlayerData.instance.SetInt(intName, defaultkills);
					}
					else
					{
						PlayerData.instance.SetInt(intName, 1);
					}
				}
				entry.RecordJournalEntry();
				return (num > 0) & withoutnotes;
			}
			catch (Exception message)
			{
				FiveKnights.Instance.Log(message);
				return withoutnotes;
			}
		}

		public static AudioSource PlayAudio(this MonoBehaviour mb, AudioClip clip, float volume = 1f, float pitchVariation = 0f, Transform posOverride = null, Func<bool> destroyWhen = null)
		{
			GameObject gameObject = new GameObject("Audio Player", typeof(AudioSource), typeof(AutoDestroy));
			gameObject.transform.position = ((posOverride == null) ? mb.transform.position : posOverride.position);
			AutoDestroy component = gameObject.GetComponent<AutoDestroy>();
			component.Time = clip.length + 1f;
			component.ShouldDestroy = destroyWhen;
			AudioSource component2 = gameObject.GetComponent<AudioSource>();
			component2.clip = clip;
			component2.volume = volume;
			component2.pitch = UnityEngine.Random.Range(1f - pitchVariation, 1f + pitchVariation);
			component2.outputAudioMixerGroup = HeroController.instance.GetComponent<AudioSource>().outputAudioMixerGroup;
			component2.Play();
			return component2;
		}
	}
	public class Flash : MonoBehaviour
	{
		private enum FlashState
		{
			Increase,
			Stay,
			Decrease,
			Stop
		}

		private SpriteRenderer _sr;

		private float amount;

		private float t;

		private float timeUp;

		private float stayTime;

		private float timeDown;

		private float flashTimer;

		private bool flashing;

		private FlashState flashingState;

		private float amountCurrent;

		private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");

		private static readonly int FlashColor = Shader.PropertyToID("_FlashColor");

		private void Start()
		{
			_sr = base.gameObject.GetComponent<SpriteRenderer>();
			_sr.material = FiveKnights.Materials["flash"];
			On.HealthManager.TakeDamage += HealthManagerTakeDamage;
			On.SpellFluke.DoDamage += SpellFlukeOnDoDamage;
			On.ExtraDamageable.RecieveExtraDamage += ExtraDamageableRecieveExtraDamage;
			On.EnemyDreamnailReaction.RecieveDreamImpact += EnemyDreamnailReactionRecieveDreamImpact;
		}

		private void ResetValues(Color color, float amount, float timeUp, float stayTime, float timeDown)
		{
			_sr.material.SetColor(FlashColor, color);
			this.amount = amount;
			this.timeUp = timeUp;
			this.stayTime = stayTime;
			this.timeDown = timeDown;
			_sr.material.SetFloat(FlashAmount, 0f);
			flashingState = FlashState.Increase;
			flashTimer = 0f;
		}

		private void Update()
		{
			if (flashingState == FlashState.Increase)
			{
				if (flashTimer < timeUp)
				{
					flashTimer += Time.deltaTime;
					t = flashTimer / timeUp;
					amountCurrent = Mathf.Lerp(0f, amount, t);
					_sr.material.SetFloat(FlashAmount, amountCurrent);
				}
				else
				{
					_sr.material.SetFloat(FlashAmount, amount);
					flashTimer = 0f;
					flashing = false;
					flashingState = FlashState.Stay;
				}
			}
			if (flashingState == FlashState.Stay)
			{
				if (flashTimer < stayTime)
				{
					flashTimer += Time.deltaTime;
				}
				else
				{
					flashTimer = 0f;
					flashingState = FlashState.Decrease;
				}
			}
			if (flashingState == FlashState.Decrease)
			{
				if (flashTimer < timeDown)
				{
					flashTimer += Time.deltaTime;
					t = flashTimer / timeDown;
					amountCurrent = Mathf.Lerp(amount, 0f, t);
					_sr.material.SetFloat(FlashAmount, amountCurrent);
				}
				else
				{
					_sr.material.SetFloat(FlashAmount, 0f);
					flashTimer = 0f;
					flashingState = FlashState.Stop;
				}
			}
		}

		public void FlashFocusHeal()
		{
			ResetValues(Color.white, 0.85f, 0.01f, 0.01f, 0.35f);
		}

		public void FlashArmoured()
		{
			ResetValues(Color.white, 0.85f, 0.01f, 0.01f, 0.25f);
		}

		public void FlashDungQuick()
		{
			ResetValues(new Color(0.45f, 0.27f, 0f), 0.75f, 0.001f, 0.05f, 0.1f);
		}

		public void FlashSporeQuick()
		{
			ResetValues(new Color(0.95f, 0.9f, 0.15f), 0.75f, 0.001f, 0.05f, 0.1f);
		}

		public void FlashDreamImpact()
		{
			ResetValues(Color.white, 0.9f, 0.01f, 0.25f, 0.75f);
		}

		private void HealthManagerTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
		{
			orig(self, hitInstance);
			if (self.gameObject == base.gameObject)
			{
				FlashFocusHeal();
			}
		}

		private void SpellFlukeOnDoDamage(On.SpellFluke.orig_DoDamage orig, SpellFluke self, GameObject tar, int upwardrecursionamount, bool burst)
		{
			orig(self, tar, upwardrecursionamount, burst);
			if (tar == base.gameObject)
			{
				FlashFocusHeal();
			}
		}

		private void ExtraDamageableRecieveExtraDamage(On.ExtraDamageable.orig_RecieveExtraDamage orig, ExtraDamageable self, ExtraDamageTypes extraDamageType)
		{
			orig(self, extraDamageType);
			if (self != null && self.gameObject == base.gameObject)
			{
				if (extraDamageType == ExtraDamageTypes.Spore)
				{
					FlashSporeQuick();
				}
				else if (FiveKnights.Instance.SaveSettings.upgradedCharm_10)
				{
					FlashArmoured();
				}
				else
				{
					FlashDungQuick();
				}
			}
		}

		private void EnemyDreamnailReactionRecieveDreamImpact(On.EnemyDreamnailReaction.orig_RecieveDreamImpact orig, EnemyDreamnailReaction self)
		{
			orig(self);
			if (self.gameObject == base.gameObject)
			{
				FlashDreamImpact();
			}
		}

		private void OnDestroy()
		{
			On.HealthManager.TakeDamage -= HealthManagerTakeDamage;
			On.SpellFluke.DoDamage -= SpellFlukeOnDoDamage;
			On.ExtraDamageable.RecieveExtraDamage -= ExtraDamageableRecieveExtraDamage;
			On.EnemyDreamnailReaction.RecieveDreamImpact -= EnemyDreamnailReactionRecieveDreamImpact;
		}

		private void Log(object o)
		{
			Modding.Logger.Log("[Flash] " + o);
		}
	}
	internal static class FsmUtil
	{
		[PublicAPI]
		public static void RemoveAction<T>(this PlayMakerFSM fsm, string stateName) where T : FsmStateAction
		{
			FsmState state = fsm.GetState(stateName);
			FsmStateAction[] actions = state.Actions;
			FsmStateAction action = fsm.GetAction<T>(stateName);
			actions = actions.Where((FsmStateAction x) => x != action).ToArray();
			Log(action.GetType().ToString());
			state.Actions = actions;
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
			return fsm.GetAction(stateName, index) as T;
		}

		[PublicAPI]
		public static T GetAction<T>(this PlayMakerFSM fsm, string stateName) where T : FsmStateAction
		{
			return fsm.GetState(stateName).Actions.FirstOrDefault((FsmStateAction x) => x is T) as T;
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
			foreach (FsmState fsmState in fsmStates)
			{
				if (source.Contains(fsmState.Name))
				{
					fsmState.Transitions = fsmState.Transitions.Where((FsmTransition trans) => !transitions.Contains(trans.ToState)).ToArray();
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
			fsm.InsertAction(stateName, new InvokeMethod(method, everyFrame), index);
		}

		[PublicAPI]
		public static void InsertCoroutine(this PlayMakerFSM fsm, string stateName, int index, Func<IEnumerator> coro, bool wait = true)
		{
			fsm.InsertAction(stateName, new InvokeCoroutine(coro, wait), index);
		}

		[PublicAPI]
		public static FsmInt GetOrCreateInt(this PlayMakerFSM fsm, string intName)
		{
			FsmInt fsmInt = new FsmInt(intName);
			List<FsmInt> list = fsm.FsmVariables.IntVariables.ToList();
			FsmInt fsmInt2 = list.FirstOrDefault((FsmInt x) => x.Name == intName);
			if (fsmInt2 != null)
			{
				return fsmInt2;
			}
			list.Add(fsmInt);
			fsm.Fsm.Variables.IntVariables = list.ToArray();
			return fsmInt;
		}

		public static void AddToSendRandomEvent(this HutongGames.PlayMaker.Actions.SendRandomEvent sre, string toState, float weight, [CanBeNull] string eventName = null)
		{
			PlayMakerFSM fsm = sre.Fsm.Owner as PlayMakerFSM;
			string name = sre.State.Name;
			eventName = eventName ?? toState.Split(' ').First();
			List<FsmEvent> list = sre.events.ToList();
			List<FsmFloat> list2 = sre.weights.ToList();
			fsm.AddTransition(name, eventName, toState);
			list.Add(fsm.GetState(name).Transitions.Single((FsmTransition x) => x.FsmEvent.Name == eventName).FsmEvent);
			list2.Add(weight);
			sre.events = list.ToArray();
			sre.weights = list2.ToArray();
		}

		public static void AddToSendRandomEventV2(this HutongGames.PlayMaker.Actions.SendRandomEventV2 sre, string toState, float weight, int eventMaxAmount, [CanBeNull] string eventName = null)
		{
			PlayMakerFSM fsm = sre.Fsm.Owner as PlayMakerFSM;
			string name = sre.State.Name;
			eventName = eventName ?? toState.Split(' ').First();
			List<FsmEvent> list = sre.events.ToList();
			List<FsmFloat> list2 = sre.weights.ToList();
			List<FsmInt> list3 = sre.trackingInts.ToList();
			List<FsmInt> list4 = sre.eventMax.ToList();
			fsm.AddTransition(name, eventName, toState);
			list.Add(fsm.GetState(name).Transitions.Single((FsmTransition x) => x.FsmEvent.Name == eventName).FsmEvent);
			list2.Add(weight);
			list3.Add(fsm.GetOrCreateInt("Ct " + eventName));
			list4.Add(eventMaxAmount);
			sre.events = list.ToArray();
			sre.weights = list2.ToArray();
			sre.trackingInts = list3.ToArray();
			sre.eventMax = list4.ToArray();
		}

		[PublicAPI]
		public static FsmState CreateState(this PlayMakerFSM fsm, string stateName)
		{
			FsmState fsmState = new FsmState(fsm.Fsm)
			{
				Name = stateName
			};
			List<FsmState> list = fsm.FsmStates.ToList();
			list.Add(fsmState);
			fsm.Fsm.States = list.ToArray();
			return fsmState;
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
			Modding.Logger.Log("[FSM UTIL]: " + str);
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
				Finish();
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
			Finish();
		}

		public override void OnEnter()
		{
			base.Fsm.Owner.StartCoroutine(_wait ? Coroutine() : _coro?.Invoke());
			if (!_wait)
			{
				Finish();
			}
		}
	}
	public class GlobalModSettings
	{
		public bool ForcedMenuTheme;
	}
	internal static class GSPImport
	{
		[ModImportName("GodSeekerPlus")]
		internal static class GodSeekerPlus
		{
			public static Action<Func<Scene, Scene, bool>> AddFastDashPredicate;

			public static Action<Func<GameManager.SceneLoadInfo, bool>> AddInfiniteChallengeReturnScenePredicate;
		}

		public static void AddFastDashPredicate(Func<Scene, Scene, bool> predicate)
		{
			GodSeekerPlus.AddFastDashPredicate?.Invoke(predicate);
		}

		public static void AddInfiniteChallengeReturnScenePredicate(Func<GameManager.SceneLoadInfo, bool> predicate)
		{
			GodSeekerPlus.AddInfiniteChallengeReturnScenePredicate?.Invoke(predicate);
		}

		static GSPImport()
		{
			typeof(GodSeekerPlus).ModInterop();
		}
	}
	public class LanguageCtrl
	{
		private Dictionary<string, Dictionary<string, Dictionary<string, string>>> jsonDict;

		public LanguageCtrl()
		{
			using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FiveKnights.assets.Language.json");
			if (stream != null)
			{
				byte[] array = new byte[stream.Length];
				stream.Read(array, 0, array.Length);
				stream.Dispose();
				string value = Encoding.Default.GetString(array);
				jsonDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(value);
				Modding.Logger.Log("[Language] Loaded Language");
				stream.Dispose();
			}
		}

		public string Get(string key, string sheet)
		{
			SupportedLanguages gameLanguage = GameManager.instance.gameSettings.gameLanguage;
			try
			{
				return jsonDict[gameLanguage.ToString()][sheet][key].Replace("<br>", "\n");
			}
			catch
			{
				return jsonDict[SupportedLanguages.EN.ToString()][sheet][key].Replace("<br>", "\n");
			}
		}

		public bool ContainsKey(string key, string sheet)
		{
			try
			{
				SupportedLanguages gameLanguage = GameManager.instance.gameSettings.gameLanguage;
				try
				{
					return jsonDict[gameLanguage.ToString()][sheet].ContainsKey(key);
				}
				catch
				{
					try
					{
						return jsonDict[SupportedLanguages.EN.ToString()][sheet].ContainsKey(key);
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
	public class MusicPlayer
	{
		public float Volume = 1f;

		public GameObject Player;

		public GameObject Spawn;

		public AudioClip Clip;

		public float MinPitch;

		public float MaxPitch;

		public bool Loop;

		private AudioSource audio;

		public void UpdateMusic()
		{
			audio.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
			audio.volume = Volume;
		}

		public void StopMusic()
		{
			audio.Stop();
		}

		private IEnumerator FixSpawn()
		{
			yield return new WaitWhile(() => HeroController.instance == null);
			Spawn = HeroController.instance.gameObject;
			DoPlayRandomClip();
		}

		public void DoPlayRandomClip()
		{
			if (Spawn == null)
			{
				GameManager.instance.StartCoroutine(FixSpawn());
				return;
			}
			GameObject gameObject = Player.Spawn(Spawn.transform.position, Quaternion.Euler(Vector3.up));
			if (Loop)
			{
				audio = gameObject.GetComponent<AudioSource>();
				audio.clip = Clip;
				audio.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
				audio.volume = Volume;
				audio.loop = Loop;
				audio.Play();
			}
			else
			{
				audio = gameObject.GetComponent<AudioSource>();
				audio.clip = null;
				audio.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
				audio.volume = Volume;
				audio.PlayOneShot(Clip);
			}
		}
	}
	internal class Parryable : MonoBehaviour
	{
		public static bool ParryFlag;

		private void OnTriggerEnter2D(Collider2D col)
		{
			if (ParryFlag || col.gameObject.layer != 16)
			{
				return;
			}
			ParryFlag = true;
			GameManager.instance.StartCoroutine(GameManager.instance.FreezeMoment(0.03f, 0.1f, 0.03f, 0f));
			HeroController.instance.NailParry();
			GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
			FiveKnights.preloadedGO["ClashTink"].transform.SetPosition2D(base.transform.position);
			FiveKnights.preloadedGO["ClashTink"].GetComponent<AudioSource>().Play();
			GameObject go = col.transform.parent.gameObject;
			float num = 0f;
			PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(go, "damages_enemy");
			if (!(playMakerFSM == null))
			{
				num = playMakerFSM.FsmVariables.FindFsmFloat("direction").Value;
				Vector3 vector = default(Vector3);
				if (num < 45f)
				{
					HeroController.instance.RecoilLeft();
					vector = new Vector2(1.5f, 0f);
				}
				else if (num < 135f)
				{
					HeroController.instance.RecoilDown();
					vector = new Vector2(0f, 1.5f);
				}
				else if (num < 225f)
				{
					HeroController.instance.RecoilRight();
					vector = new Vector2(-1.5f, 0f);
				}
				else
				{
					HeroController.instance.Bounce();
					vector = new Vector2(0f, -1.5f);
				}
				GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["parryFX"]);
				obj.transform.SetPosition2D(HeroController.instance.transform.position);
				obj.transform.position += vector;
				obj.SetActive(value: true);
				StartCoroutine(DoNextFrame());
			}
		}

		private IEnumerator DoNextFrame()
		{
			yield return null;
			HeroController.instance.NailParryRecover();
			yield return new WaitForSecondsRealtime(0.5f);
			ParryFlag = false;
		}

		private void Log(object ob)
		{
			Modding.Logger.Log("[Parryable] " + ob);
		}
	}
	internal class ParryTink : MonoBehaviour
	{
		internal static AudioClip TinkClip { get; set; }

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!(other.gameObject.name != "Clash Tink") || other.gameObject.CompareTag("Nail Attack"))
			{
				GameManager.instance.StartCoroutine(GameManager.instance.FreezeMoment(0.01f, 0.15f, 0.1f, 0f));
				HeroController.instance.NailParry();
				GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
				this.PlayAudio(TinkClip, 1f, 0.15f);
			}
		}
	}
	public class Pogoable : MonoBehaviour
	{
		public GameObject tar;

		private void OnTriggerEnter2D(Collider2D col)
		{
			if (col.gameObject.layer != 16)
			{
				return;
			}
			GameObject go = tar;
			if (col.transform.parent != null)
			{
				go = col.transform.parent.gameObject;
			}
			float num = 0f;
			PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(go, "damages_enemy");
			if (!(playMakerFSM == null))
			{
				num = playMakerFSM.FsmVariables.FindFsmFloat("direction").Value;
				if (num < 45f)
				{
					HeroController.instance.RecoilLeft();
				}
				else if (num < 135f)
				{
					HeroController.instance.RecoilDown();
				}
				else if (num < 225f)
				{
					HeroController.instance.RecoilRight();
				}
				else
				{
					HeroController.instance.Bounce();
				}
			}
		}
	}
	public static class RewardRoom
	{
		public static bool doneCCHitless;

		private static LanguageCtrl langCtrl;

		private static Animator dryyaAnim;

		private static Animator ogrimAnim;

		private static Animator ismaAnim;

		private static Animator hegemolAnim;

		private static Animator zemerAnim;

		private static Coroutine zemerAnimCoro;

		public static void Hook()
		{
			langCtrl = new LanguageCtrl();
			ModHooks.LanguageGetHook += LangGet;
			On.GameManager.GetCurrentMapZone += GameManagerGetCurrentMapZone;
			On.GameManager.EnterHero += GameManagerEnterHero;
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ActiveSceneChanged;
		}

		public static void Unhook()
		{
			ModHooks.LanguageGetHook -= LangGet;
			On.GameManager.GetCurrentMapZone -= GameManagerGetCurrentMapZone;
			On.GameManager.EnterHero -= GameManagerEnterHero;
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= ActiveSceneChanged;
		}

		private static string LangGet(string key, string sheet, string orig)
		{
			if (key.StartsWith("RR_"))
			{
				sheet = "Reward Room";
			}
			if (!langCtrl.ContainsKey(key, sheet))
			{
				return orig;
			}
			return langCtrl.Get(key, sheet);
		}

		private static string GameManagerGetCurrentMapZone(On.GameManager.orig_GetCurrentMapZone orig, GameManager self)
		{
			if (self.sceneName == "hidden_reward_room")
			{
				return MapZone.DREAM_WORLD.ToString();
			}
			return orig(self);
		}

		private static void GameManagerEnterHero(On.GameManager.orig_EnterHero orig, GameManager self, bool additiveGateSearch)
		{
			if (self.sceneName == "hidden_reward_room")
			{
				BossLoader.LoadHegemolSound();
				BossLoader.LoadIsmaBundle();
				BossLoader.LoadZemerBundle();
				BossLoader.LoadDryyaBundle();
				for (int i = 1; i < 8; i++)
				{
					string text = "DTalk" + i;
					FiveKnights.Clips[text] = ABManager.AssetBundles[ABManager.Bundle.Sound].LoadAsset<AudioClip>(text);
				}
				self.tilemap.width = 500;
				self.tilemap.height = 200;
				CreateGateway("door1", new Vector2(266f, 131f), Vector2.zero, null, null, right: true, left: false, onlyOut: true, GameManager.SceneLoadVisualizations.Dream);
				GameCameras.instance.hudCamera.gameObject.transform.Find("Blanker White").gameObject.LocateMyFSM("Blanker Control").SendEvent("FADE OUT");
				dryyaAnim = USceneUtil.Find(GameObject.Find("Dryya"), "Head").GetComponent<Animator>();
				ogrimAnim = GameObject.Find("Ogrim").GetComponent<Animator>();
				ismaAnim = GameObject.Find("Isma").GetComponent<Animator>();
				hegemolAnim = GameObject.Find("Hegemol").GetComponent<Animator>();
				zemerAnim = GameObject.Find("Zemer").GetComponent<Animator>();
				if (zemerAnim != null)
				{
					zemerAnimCoro = GameManager.instance.StartCoroutine(ZemerAnimControl());
				}
				if (doneCCHitless)
				{
					GameObject[] array = new GameObject[5]
					{
						GameObject.Find("Dryya"),
						GameObject.Find("Ogrim"),
						GameObject.Find("Isma"),
						GameObject.Find("Hegemol"),
						GameObject.Find("Zemer")
					};
					for (int j = 0; j < array.Length; j++)
					{
						SpriteRenderer[] componentsInChildren = array[j].GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
						for (int k = 0; k < componentsInChildren.Length; k++)
						{
							componentsInChildren[k].enabled = false;
						}
					}
				}
				if (!FiveKnights.Clips.ContainsKey("Pale Court") || FiveKnights.Clips["Pale Court"] == null)
				{
					AssetBundle assetBundle = ABManager.AssetBundles[ABManager.Bundle.Sound];
					FiveKnights.Clips["Pale Court"] = assetBundle.LoadAsset<AudioClip>("Pale Court");
				}
				PlayMusic(FiveKnights.Clips["Pale Court"]);
			}
			orig(self, additiveGateSearch: false);
		}

		private static void ActiveSceneChanged(Scene arg0, Scene arg1)
		{
			if (arg1.name == "White_Palace_09")
			{
				DialogueNPC dialogueNPC = DialogueNPC.CreateInstance();
				dialogueNPC.transform.position = new Vector3(65f, 98.4f, 0f);
				dialogueNPC.DialogueSelector = EntranceDialogue;
				dialogueNPC.SetTitle("RR_ENTER_TITLE");
				dialogueNPC.SetDreamKey("RR_ENTER_TITLE_SUB");
				dialogueNPC.SetUp();
				dialogueNPC.gameObject.SetActive(value: false);
			}
			if (arg1.name == "hidden_reward_room")
			{
				CameraLockArea[] array = UnityEngine.Object.FindObjectsOfType<CameraLockArea>(includeInactive: true);
				for (int i = 0; i < array.Length; i++)
				{
					UnityEngine.Object.Destroy(array[i].gameObject);
				}
				float y = 132.5f;
				CreateCameraLock("CameraLockOuter", new Vector2(301f, y), new Vector2(3f, 1f), new Vector2(37f, 23f), new Vector2(0f, 0f), new Vector2(263f, y), new Vector2(340f, y), preventLookDown: true);
				CreateCameraLock("CameraLockMid", new Vector2(302f, y), new Vector2(1f, 1f), new Vector2(24f, 23f), new Vector2(0f, 0f), new Vector2(302f, y), new Vector2(302f, y), preventLookDown: true, maxPriority: true);
				FixBlur();
				DreamEntry();
				PlayerData.instance.dreamReturnScene = "White_Palace_09";
				GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Dream Base"], new Vector3(337.71f, 127.58f, -0.5f), Quaternion.identity);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Dream Beam"], new Vector3(337.71f, 127.58f), Quaternion.identity);
				GameObject gameObject3 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Dream Door Warp"], new Vector3(337.61f, 128.18f, 0.2f), Quaternion.identity);
				PlayMakerFSM fsm = gameObject3.LocateMyFSM("Door Control");
				fsm.RemoveAction("Set PD Bool", 1);
				fsm.GetFsmStringVariable("Entry Gate").Value = "door_dreamReturn";
				fsm.GetFsmStringVariable("New Scene").Value = "White_Palace_09";
				fsm.GetAction<HutongGames.PlayMaker.Actions.BeginSceneTransition>("Change Scene", 3).preventCameraFadeOut = true;
				gameObject.SetActive(value: true);
				gameObject2.SetActive(value: true);
				gameObject3.SetActive(value: true);
				DialogueNPC dialogueNPC2 = DialogueNPC.CreateInstance();
				dialogueNPC2.transform.position = new Vector3(293.38f, 129.67f, 0f);
				dialogueNPC2.transform.Find("Prompt Marker").position = new Vector3(293.78f, 134.9f, 0.2f);
				dialogueNPC2.DialogueSelector = DryyaDialogue;
				dialogueNPC2.GetComponent<MeshRenderer>().enabled = doneCCHitless;
				dialogueNPC2.SetTitle("RR_DRYYA_TITLE");
				dialogueNPC2.SetDreamKey(GetDreamKey("DRYYA"));
				dialogueNPC2.SetUp();
				DialogueNPC dialogueNPC3 = DialogueNPC.CreateInstance();
				dialogueNPC3.transform.position = new Vector3(297.35f, 129.0865f, 0f);
				dialogueNPC3.transform.Find("Prompt Marker").position = new Vector3(297.75f, 133f, 0.2f);
				dialogueNPC3.DialogueSelector = OgrimDialogue;
				dialogueNPC3.GetComponent<MeshRenderer>().enabled = doneCCHitless;
				dialogueNPC3.SetTitle("RR_OGRIM_TITLE");
				dialogueNPC3.SetDreamKey(GetDreamKey("OGRIM"));
				dialogueNPC3.gameObject.LocateMyFSM("npc_control").GetFsmBoolVariable("Hero Always Left").Value = true;
				dialogueNPC3.gameObject.LocateMyFSM("npc_control").GetFsmBoolVariable("Hero Always Right").Value = false;
				dialogueNPC3.SetUp();
				DialogueNPC dialogueNPC4 = DialogueNPC.CreateInstance();
				dialogueNPC4.transform.position = new Vector3(300.79f, 129.0865f, 0f);
				dialogueNPC4.transform.Find("Prompt Marker").position = new Vector3(300.79f, 132.1f, 0.2f);
				dialogueNPC4.DialogueSelector = IsmaDialogue;
				dialogueNPC4.GetComponent<MeshRenderer>().enabled = doneCCHitless;
				dialogueNPC4.SetTitle("RR_ISMA_TITLE");
				dialogueNPC4.SetDreamKey(GetDreamKey("ISMA"));
				dialogueNPC4.SetUp();
				DialogueNPC dialogueNPC5 = DialogueNPC.CreateInstance();
				dialogueNPC5.transform.position = new Vector3(305.08f, 129.38f, 0f);
				dialogueNPC5.transform.Find("Prompt Marker").position = new Vector3(305.68f, 135f, 0.2f);
				dialogueNPC5.DialogueSelector = HegemolDialogue;
				dialogueNPC5.GetComponent<MeshRenderer>().enabled = doneCCHitless;
				dialogueNPC5.SetTitle("RR_HEGEMOL_TITLE");
				dialogueNPC5.SetDreamKey(GetDreamKey("HEGEMOL"));
				dialogueNPC5.gameObject.LocateMyFSM("npc_control").GetFsmBoolVariable("Hero Always Left").Value = true;
				dialogueNPC5.gameObject.LocateMyFSM("npc_control").GetFsmBoolVariable("Hero Always Right").Value = false;
				dialogueNPC5.SetUp();
				DialogueNPC dialogueNPC6 = DialogueNPC.CreateInstance();
				dialogueNPC6.transform.position = new Vector3(311.03f, 129.0576f, 0f);
				dialogueNPC6.transform.Find("Prompt Marker").position = new Vector3(310.53f, 134.97f, 0.2f);
				dialogueNPC6.DialogueSelector = ZemerDialogue;
				dialogueNPC6.GetComponent<MeshRenderer>().enabled = doneCCHitless;
				dialogueNPC6.SetTitle("RR_ZEMER_TITLE");
				dialogueNPC6.SetDreamKey(GetDreamKey("ZEMER"));
				dialogueNPC6.gameObject.LocateMyFSM("npc_control").GetFsmBoolVariable("Hero Always Left").Value = true;
				dialogueNPC6.gameObject.LocateMyFSM("npc_control").GetFsmBoolVariable("Hero Always Right").Value = false;
				dialogueNPC6.SetUp();
			}
		}

		private static void CreateGateway(string gateName, Vector2 pos, Vector2 size, string toScene, string entryGate, bool right, bool left, bool onlyOut, GameManager.SceneLoadVisualizations vis)
		{
			GameObject gameObject = new GameObject(gateName);
			gameObject.transform.SetPosition2D(pos);
			TransitionPoint transitionPoint = gameObject.AddComponent<TransitionPoint>();
			if (!onlyOut)
			{
				BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
				boxCollider2D.size = size;
				boxCollider2D.isTrigger = true;
				transitionPoint.targetScene = toScene;
				transitionPoint.entryPoint = entryGate;
			}
			transitionPoint.alwaysEnterLeft = left;
			transitionPoint.alwaysEnterRight = right;
			GameObject gameObject2 = new GameObject("Hazard Respawn Marker");
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.tag = "RespawnPoint";
			gameObject2.transform.SetPosition2D(pos);
			transitionPoint.respawnMarker = gameObject2.AddComponent<HazardRespawnMarker>();
			transitionPoint.sceneLoadVisualization = vis;
		}

		private static void CreateCameraLock(string n, Vector2 pos, Vector2 scl, Vector2 cSize, Vector2 cOff, Vector2 min, Vector2 max, bool preventLookDown = false, bool maxPriority = false)
		{
			GameObject gameObject = new GameObject(n);
			BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
			CameraLockArea cameraLockArea = gameObject.AddComponent<CameraLockArea>();
			gameObject.transform.position = pos;
			gameObject.transform.localScale = scl;
			boxCollider2D.isTrigger = true;
			boxCollider2D.size = cSize;
			boxCollider2D.offset = cOff;
			cameraLockArea.cameraXMin = min.x;
			cameraLockArea.cameraXMax = max.x;
			cameraLockArea.cameraYMin = (cameraLockArea.cameraYMax = min.y);
			cameraLockArea.preventLookDown = preventLookDown;
			cameraLockArea.maxPriority = maxPriority;
			gameObject.SetActive(value: true);
			bool enabled = (cameraLockArea.enabled = true);
			boxCollider2D.enabled = enabled;
		}

		private static void FixBlur()
		{
			GameObject gameObject = null;
			SceneManager[] array = UnityEngine.Object.FindObjectsOfType<SceneManager>();
			foreach (SceneManager obj in array)
			{
				gameObject = obj.borderPrefab;
				UnityEngine.Object.Destroy(obj.gameObject);
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["SMTest"]);
			if (gameObject != null)
			{
				gameObject2.GetComponent<SceneManager>().borderPrefab = gameObject;
			}
			gameObject2.GetComponent<SceneManager>().noLantern = true;
			gameObject2.GetComponent<SceneManager>().darknessLevel = -1;
			gameObject2.SetActive(value: true);
		}

		private static void DreamEntry()
		{
			foreach (GameObject item in from x in UnityEngine.Object.FindObjectsOfType<GameObject>()
				where x.name == "Dream Entry"
				select x)
			{
				HeroController.instance.isHeroInPosition = true;
				GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["DreamEntry"]);
				gameObject.transform.position = item.transform.position;
				UnityEngine.Object.Destroy(item);
				gameObject.SetActive(value: true);
				gameObject.name = "Dream Entry";
				HeroController.instance.FaceRight();
			}
		}

		public static void PlayMusic(AudioClip clip)
		{
			MusicCue musicCue = ScriptableObject.CreateInstance<MusicCue>();
			MusicCue.MusicChannelInfo musicChannelInfo = new MusicCue.MusicChannelInfo();
			Mirror.SetField(musicChannelInfo, "clip", clip);
			MusicCue.MusicChannelInfo[] value = new MusicCue.MusicChannelInfo[6] { musicChannelInfo, null, null, null, null, null };
			Mirror.SetField(musicCue, "channelInfos", value);
			Resources.FindObjectsOfTypeAll<AudioMixer>().First((AudioMixer x) => x.name == "Music").FindSnapshot("Main Only")
				.TransitionTo(0f);
			GameManager.instance.AudioManager.ApplyMusicCue(musicCue, 0f, 0f, applySnapshot: false);
		}

		private static IEnumerator DebugLoadRR()
		{
			yield return new WaitForSeconds(1f);
			doneCCHitless = false;
			HeroController.instance.EnterWithoutInput(flag: true);
			GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
			{
				SceneName = "Pale_Court_Credits",
				Visualization = GameManager.SceneLoadVisualizations.Default,
				WaitForSceneTransitionCameraFade = false,
				PreventCameraFadeOut = true,
				EntryDelay = 0f,
				HeroLeaveDirection = GatePosition.door
			});
		}

		private static string GetDreamKey(string name)
		{
			if (FiveKnights.Instance.SaveSettings.ChampionsCallClears <= 0)
			{
				return "RR_DRYYA_CHEATER";
			}
			string arg = "THIRD";
			if (FiveKnights.Instance.SaveSettings.ChampionsCallClears == 1)
			{
				arg = "FIRST";
			}
			else if (FiveKnights.Instance.SaveSettings.ChampionsCallClears == 2)
			{
				arg = "SECOND";
			}
			return $"RR_{name}_{arg}_DREAM";
		}

		private static DialogueOptions EntranceDialogue(DialogueCallbackOptions prev)
		{
			if (!prev.Continue)
			{
				return new DialogueOptions
				{
					Key = "RR_ENTER",
					Sheet = "Reward Room",
					Cost = 0,
					Type = DialogueType.YesNo,
					Continue = true
				};
			}
			if (prev.Response == DialogueResponse.Yes)
			{
				GameManager.instance.StartCoroutine(DebugLoadRR());
			}
			return new DialogueOptions
			{
				Continue = false
			};
		}

		private static DialogueOptions DryyaDialogue(DialogueCallbackOptions prev)
		{
			if (!prev.Continue)
			{
				string key;
				if (FiveKnights.Instance.SaveSettings.ChampionsCallClears == 1)
				{
					if (!FiveKnights.Instance.SaveSettings.DryyaFirstConvo1)
					{
						key = "RR_DRYYA_FIRST_1_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["DryyaVoiceConvo1"]);
						FiveKnights.Instance.SaveSettings.DryyaFirstConvo1 = true;
					}
					else if (!FiveKnights.Instance.SaveSettings.DryyaFirstConvo2)
					{
						key = "RR_DRYYA_FIRST_2_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["DryyaVoiceConvo2"]);
						FiveKnights.Instance.SaveSettings.DryyaFirstConvo2 = true;
					}
					else if (FiveKnights.Instance.SaveSettings.equippedCharms[0] && !FiveKnights.Instance.SaveSettings.DryyaCharmConvo)
					{
						key = "RR_DRYYA_CHARM_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["DryyaVoiceConvo2"]);
						FiveKnights.Instance.SaveSettings.DryyaCharmConvo = true;
					}
					else if (PlayerData.instance.GetInt("nailSmithUpgrades") == 0 && !FiveKnights.Instance.SaveSettings.DryyaOldNailConvo)
					{
						key = "RR_DRYYA_OLDNAIL_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["DryyaVoiceConvo2"]);
						FiveKnights.Instance.SaveSettings.DryyaOldNailConvo = true;
					}
					else
					{
						HeroController.instance.PlayAudio(FiveKnights.Clips["DryyaVoiceConvo3"]);
						key = "RR_DRYYA_FIRST_REPEAT";
					}
				}
				else if (FiveKnights.Instance.SaveSettings.ChampionsCallClears == 2)
				{
					if (!FiveKnights.Instance.SaveSettings.DryyaSecondConvo1)
					{
						key = "RR_DRYYA_SECOND_1_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["DryyaVoiceConvo1"]);
						FiveKnights.Instance.SaveSettings.DryyaSecondConvo1 = true;
					}
					else if (!FiveKnights.Instance.SaveSettings.DryyaSecondConvo2)
					{
						key = "RR_DRYYA_SECOND_2_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["DryyaVoiceConvo2"]);
						FiveKnights.Instance.SaveSettings.DryyaSecondConvo2 = true;
					}
					else if (FiveKnights.Instance.SaveSettings.equippedCharms[0] && !FiveKnights.Instance.SaveSettings.DryyaCharmConvo)
					{
						key = "RR_DRYYA_CHARM_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["DryyaVoiceConvo2"]);
						FiveKnights.Instance.SaveSettings.DryyaCharmConvo = true;
					}
					else if (PlayerData.instance.GetInt("nailSmithUpgrades") == 0 && !FiveKnights.Instance.SaveSettings.DryyaOldNailConvo)
					{
						key = "RR_DRYYA_OLDNAIL_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["DryyaVoiceConvo2"]);
						FiveKnights.Instance.SaveSettings.DryyaOldNailConvo = true;
					}
					else
					{
						HeroController.instance.PlayAudio(FiveKnights.Clips["DryyaVoiceConvo3"]);
						key = "RR_DRYYA_SECOND_REPEAT";
					}
				}
				else if (FiveKnights.Instance.SaveSettings.ChampionsCallClears >= 3)
				{
					if (!FiveKnights.Instance.SaveSettings.DryyaThirdConvo1)
					{
						key = "RR_DRYYA_THIRD_1_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["DryyaVoiceConvo1"]);
						FiveKnights.Instance.SaveSettings.DryyaThirdConvo1 = true;
					}
					else if (FiveKnights.Instance.SaveSettings.equippedCharms[0] && !FiveKnights.Instance.SaveSettings.DryyaCharmConvo)
					{
						key = "RR_DRYYA_CHARM_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["DryyaVoiceConvo2"]);
						FiveKnights.Instance.SaveSettings.DryyaCharmConvo = true;
					}
					else if (PlayerData.instance.GetInt("nailSmithUpgrades") == 0 && !FiveKnights.Instance.SaveSettings.DryyaOldNailConvo)
					{
						key = "RR_DRYYA_OLDNAIL_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["DryyaVoiceConvo2"]);
						FiveKnights.Instance.SaveSettings.DryyaOldNailConvo = true;
					}
					else
					{
						HeroController.instance.PlayAudio(FiveKnights.Clips["DryyaVoiceConvo3"]);
						key = "RR_DRYYA_THIRD_REPEAT";
					}
				}
				else
				{
					key = "RR_DRYYA_CHEATER";
				}
				return new DialogueOptions
				{
					Key = key,
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Wait = PlayAnimDryya(),
					Continue = true
				};
			}
			return prev.Key switch
			{
				"RR_DRYYA_FIRST_1_1" => new DialogueOptions
				{
					Key = "RR_DRYYA_FIRST_1_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_DRYYA_FIRST_1_2" => new DialogueOptions
				{
					Key = "RR_DRYYA_FIRST_1_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_DRYYA_FIRST_1_3" => new DialogueOptions
				{
					Key = "RR_DRYYA_FIRST_1_4",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_DRYYA_FIRST_2_1" => new DialogueOptions
				{
					Key = "RR_DRYYA_FIRST_2_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_DRYYA_FIRST_2_2" => new DialogueOptions
				{
					Key = "RR_DRYYA_FIRST_2_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_DRYYA_FIRST_2_3" => new DialogueOptions
				{
					Key = "RR_DRYYA_FIRST_2_4",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_DRYYA_SECOND_1_1" => new DialogueOptions
				{
					Key = "RR_DRYYA_SECOND_1_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_DRYYA_SECOND_1_2" => new DialogueOptions
				{
					Key = "RR_DRYYA_SECOND_1_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_DRYYA_SECOND_2_1" => new DialogueOptions
				{
					Key = ((PlayerData.instance.nailSmithUpgrades != 4) ? "RR_DRYYA_SECOND_2_2_ALT" : "RR_DRYYA_SECOND_2_2"),
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_DRYYA_SECOND_2_2" => new DialogueOptions
				{
					Key = "RR_DRYYA_SECOND_2_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_DRYYA_SECOND_2_2_ALT" => new DialogueOptions
				{
					Key = "RR_DRYYA_SECOND_2_3_ALT",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_DRYYA_THIRD_1_1" => new DialogueOptions
				{
					Key = "RR_DRYYA_THIRD_1_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_DRYYA_CHARM_1" => new DialogueOptions
				{
					Key = "RR_DRYYA_CHARM_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_DRYYA_CHARM_2" => new DialogueOptions
				{
					Key = "RR_DRYYA_CHARM_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_DRYYA_OLDNAIL_1" => new DialogueOptions
				{
					Key = "RR_DRYYA_OLDNAIL_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_DRYYA_OLDNAIL_2" => new DialogueOptions
				{
					Key = "RR_DRYYA_OLDNAIL_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				_ => new DialogueOptions
				{
					Continue = false,
					Wait = StopAnimDryya()
				}, 
			};
			static IEnumerator PlayAnimDryya()
			{
				if (HeroController.instance.transform.position.x < dryyaAnim.gameObject.transform.position.x)
				{
					dryyaAnim.Play("TalkLeft");
				}
				else
				{
					yield return dryyaAnim.PlayBlocking("TurnRight");
					dryyaAnim.Play("TalkRight");
				}
			}
			static IEnumerator StopAnimDryya()
			{
				if (HeroController.instance.transform.position.x > dryyaAnim.gameObject.transform.position.x)
				{
					yield return dryyaAnim.PlayBlocking("TurnLeft");
				}
				dryyaAnim.Play("Idle");
			}
		}

		private static DialogueOptions OgrimDialogue(DialogueCallbackOptions prev)
		{
			if (!prev.Continue)
			{
				string key;
				if (FiveKnights.Instance.SaveSettings.ChampionsCallClears == 1)
				{
					if (!FiveKnights.Instance.SaveSettings.OgrimFirstConvo1)
					{
						key = "RR_OGRIM_FIRST_1_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["DTalk1"]);
						FiveKnights.Instance.SaveSettings.OgrimFirstConvo1 = true;
					}
					else if (!FiveKnights.Instance.SaveSettings.OgrimFirstConvo2)
					{
						key = "RR_OGRIM_FIRST_2_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["DTalk2"]);
						FiveKnights.Instance.SaveSettings.OgrimFirstConvo2 = true;
					}
					else if (PlayerData.instance.equippedCharm_10 && FiveKnights.Instance.SaveSettings.upgradedCharm_10 && !FiveKnights.Instance.SaveSettings.OgrimCharmConvo)
					{
						key = "RR_OGRIM_CHARM_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["DTalk5"]);
						FiveKnights.Instance.SaveSettings.OgrimCharmConvo = true;
					}
					else
					{
						key = "RR_OGRIM_FIRST_REPEAT";
						HeroController.instance.PlayAudio(FiveKnights.Clips["DTalk7"]);
					}
				}
				else if (FiveKnights.Instance.SaveSettings.ChampionsCallClears == 2)
				{
					if (!FiveKnights.Instance.SaveSettings.OgrimSecondConvo1)
					{
						key = "RR_OGRIM_SECOND_1_1";
						FiveKnights.Instance.SaveSettings.OgrimSecondConvo1 = true;
						HeroController.instance.PlayAudio(FiveKnights.Clips["DTalk3"]);
					}
					else if (!FiveKnights.Instance.SaveSettings.OgrimSecondConvo2)
					{
						key = "RR_OGRIM_SECOND_2_1";
						FiveKnights.Instance.SaveSettings.OgrimSecondConvo2 = true;
						HeroController.instance.PlayAudio(FiveKnights.Clips["DTalk4"]);
					}
					else if (PlayerData.instance.equippedCharm_10 && FiveKnights.Instance.SaveSettings.upgradedCharm_10 && !FiveKnights.Instance.SaveSettings.OgrimCharmConvo)
					{
						key = "RR_OGRIM_CHARM_1";
						FiveKnights.Instance.SaveSettings.OgrimCharmConvo = true;
						HeroController.instance.PlayAudio(FiveKnights.Clips["DTalk5"]);
					}
					else
					{
						key = "RR_OGRIM_SECOND_REPEAT";
						HeroController.instance.PlayAudio(FiveKnights.Clips["DTalk7"]);
					}
				}
				else if (FiveKnights.Instance.SaveSettings.ChampionsCallClears >= 3)
				{
					if (!FiveKnights.Instance.SaveSettings.OgrimThirdConvo1)
					{
						key = "RR_OGRIM_THIRD_1_1";
						FiveKnights.Instance.SaveSettings.OgrimThirdConvo1 = true;
						HeroController.instance.PlayAudio(FiveKnights.Clips["DTalk6"]);
					}
					else if (PlayerData.instance.equippedCharm_10 && FiveKnights.Instance.SaveSettings.upgradedCharm_10 && !FiveKnights.Instance.SaveSettings.OgrimCharmConvo)
					{
						key = "RR_OGRIM_CHARM_1";
						FiveKnights.Instance.SaveSettings.OgrimCharmConvo = true;
						HeroController.instance.PlayAudio(FiveKnights.Clips["DTalk5"]);
					}
					else
					{
						key = "RR_OGRIM_THIRD_REPEAT";
						HeroController.instance.PlayAudio(FiveKnights.Clips["DTalk7"]);
					}
				}
				else
				{
					key = "RR_OGRIM_CHEATER";
				}
				return new DialogueOptions
				{
					Key = key,
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Wait = PlayAnimOgrim(),
					Continue = true
				};
			}
			return prev.Key switch
			{
				"RR_OGRIM_FIRST_1_1" => new DialogueOptions
				{
					Key = "RR_OGRIM_FIRST_1_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_OGRIM_FIRST_1_2" => new DialogueOptions
				{
					Key = "RR_OGRIM_FIRST_1_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_OGRIM_FIRST_2_1" => new DialogueOptions
				{
					Key = "RR_OGRIM_FIRST_2_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_OGRIM_FIRST_2_2" => new DialogueOptions
				{
					Key = "RR_OGRIM_FIRST_2_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_OGRIM_SECOND_1_1" => new DialogueOptions
				{
					Key = "RR_OGRIM_SECOND_1_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_OGRIM_SECOND_1_2" => new DialogueOptions
				{
					Key = "RR_OGRIM_SECOND_1_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_OGRIM_SECOND_2_1" => new DialogueOptions
				{
					Key = "RR_OGRIM_SECOND_2_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_OGRIM_SECOND_2_2" => new DialogueOptions
				{
					Key = "RR_OGRIM_SECOND_2_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_OGRIM_THIRD_1_1" => new DialogueOptions
				{
					Key = "RR_OGRIM_THIRD_1_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_OGRIM_CHARM_1" => new DialogueOptions
				{
					Key = "RR_OGRIM_CHARM_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_OGRIM_CHARM_2" => new DialogueOptions
				{
					Key = "RR_OGRIM_CHARM_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_OGRIM_OLDNAIL_1" => new DialogueOptions
				{
					Key = "RR_OGRIM_OLDNAIL_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_OGRIM_OLDNAIL_2" => new DialogueOptions
				{
					Key = "RR_OGRIM_OLDNAIL_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				_ => new DialogueOptions
				{
					Continue = false,
					Wait = StopAnimOgrim()
				}, 
			};
			static IEnumerator PlayAnimOgrim()
			{
				yield return ogrimAnim.PlayBlocking("TurnLeft");
				ogrimAnim.Play("TalkLeft");
			}
			static IEnumerator StopAnimOgrim()
			{
				yield return ogrimAnim.PlayBlocking("TurnRight");
				ogrimAnim.Play("Idle");
			}
		}

		private static DialogueOptions IsmaDialogue(DialogueCallbackOptions prev)
		{
			if (!prev.Continue)
			{
				string key;
				if (FiveKnights.Instance.SaveSettings.ChampionsCallClears == 1)
				{
					if (!FiveKnights.Instance.SaveSettings.IsmaFirstConvo1)
					{
						key = "RR_ISMA_FIRST_1_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["IsmaAudTalkHi"]);
						FiveKnights.Instance.SaveSettings.IsmaFirstConvo1 = true;
					}
					else if (!FiveKnights.Instance.SaveSettings.IsmaFirstConvo2)
					{
						key = "RR_ISMA_FIRST_2_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["IsmaAudTalk1"]);
						FiveKnights.Instance.SaveSettings.IsmaFirstConvo2 = true;
					}
					else if (PlayerData.instance.equippedCharm_10 && FiveKnights.Instance.SaveSettings.upgradedCharm_10 && !FiveKnights.Instance.SaveSettings.IsmaCharmConvo)
					{
						key = "RR_ISMA_CHARM_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["IsmaAudTalkCharm"]);
						FiveKnights.Instance.SaveSettings.IsmaCharmConvo = true;
					}
					else
					{
						HeroController.instance.PlayAudio(FiveKnights.Clips["IsmaAudTalkBye"]);
						key = "RR_ISMA_FIRST_REPEAT";
					}
				}
				else if (FiveKnights.Instance.SaveSettings.ChampionsCallClears == 2)
				{
					if (!FiveKnights.Instance.SaveSettings.IsmaSecondConvo1)
					{
						key = "RR_ISMA_SECOND_1_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["IsmaAudTalk3"]);
						FiveKnights.Instance.SaveSettings.IsmaSecondConvo1 = true;
					}
					else if (!FiveKnights.Instance.SaveSettings.IsmaSecondConvo2)
					{
						key = "RR_ISMA_SECOND_2_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["IsmaAudTalk5"]);
						FiveKnights.Instance.SaveSettings.IsmaSecondConvo2 = true;
					}
					else if (PlayerData.instance.equippedCharm_10 && FiveKnights.Instance.SaveSettings.upgradedCharm_10 && !FiveKnights.Instance.SaveSettings.IsmaCharmConvo)
					{
						key = "RR_ISMA_CHARM_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["IsmaAudTalkCharm"]);
						FiveKnights.Instance.SaveSettings.IsmaCharmConvo = true;
					}
					else
					{
						HeroController.instance.PlayAudio(FiveKnights.Clips["IsmaAudTalkBye"]);
						key = "RR_ISMA_SECOND_REPEAT";
					}
				}
				else if (FiveKnights.Instance.SaveSettings.ChampionsCallClears >= 3)
				{
					if (!FiveKnights.Instance.SaveSettings.IsmaThirdConvo1)
					{
						key = "RR_ISMA_THIRD_1_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["IsmaAudTalk6"]);
						FiveKnights.Instance.SaveSettings.IsmaThirdConvo1 = true;
					}
					else if (PlayerData.instance.equippedCharm_10 && FiveKnights.Instance.SaveSettings.upgradedCharm_10 && !FiveKnights.Instance.SaveSettings.IsmaCharmConvo)
					{
						key = "RR_ISMA_CHARM_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["IsmaAudTalkCharm"]);
						FiveKnights.Instance.SaveSettings.IsmaCharmConvo = true;
					}
					else
					{
						HeroController.instance.PlayAudio(FiveKnights.Clips["IsmaAudTalkBye"]);
						key = "RR_ISMA_THIRD_REPEAT";
					}
				}
				else
				{
					key = "RR_ISMA_CHEATER";
				}
				return new DialogueOptions
				{
					Key = key,
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Wait = PlayAnimIsma(),
					Continue = true
				};
			}
			return prev.Key switch
			{
				"RR_ISMA_FIRST_1_1" => new DialogueOptions
				{
					Key = "RR_ISMA_FIRST_1_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ISMA_FIRST_1_2" => new DialogueOptions
				{
					Key = "RR_ISMA_FIRST_1_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ISMA_FIRST_2_1" => new DialogueOptions
				{
					Key = "RR_ISMA_FIRST_2_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ISMA_FIRST_2_2" => new DialogueOptions
				{
					Key = "RR_ISMA_FIRST_2_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ISMA_SECOND_1_1" => new DialogueOptions
				{
					Key = "RR_ISMA_SECOND_1_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ISMA_SECOND_1_2" => new DialogueOptions
				{
					Key = "RR_ISMA_SECOND_1_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ISMA_SECOND_2_1" => new DialogueOptions
				{
					Key = "RR_ISMA_SECOND_2_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ISMA_SECOND_2_2" => new DialogueOptions
				{
					Key = "RR_ISMA_SECOND_2_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ISMA_THIRD_1_1" => new DialogueOptions
				{
					Key = "RR_ISMA_THIRD_1_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ISMA_CHARM_1" => new DialogueOptions
				{
					Key = "RR_ISMA_CHARM_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ISMA_CHARM_2" => new DialogueOptions
				{
					Key = "RR_ISMA_CHARM_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ISMA_OLDNAIL_1" => new DialogueOptions
				{
					Key = "RR_ISMA_OLDNAIL_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ISMA_OLDNAIL_2" => new DialogueOptions
				{
					Key = "RR_ISMA_OLDNAIL_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				_ => new DialogueOptions
				{
					Continue = false,
					Wait = StopAnimIsma()
				}, 
			};
			static IEnumerator PlayAnimIsma()
			{
				yield return ismaAnim.PlayBlocking("TurnRight");
				ismaAnim.Play("TalkRight");
			}
			static IEnumerator StopAnimIsma()
			{
				yield return ismaAnim.PlayBlocking("TurnLeft");
				ismaAnim.Play("Idle");
			}
		}

		private static DialogueOptions HegemolDialogue(DialogueCallbackOptions prev)
		{
			if (!prev.Continue)
			{
				string key;
				if (FiveKnights.Instance.SaveSettings.ChampionsCallClears == 1)
				{
					if (!FiveKnights.Instance.SaveSettings.HegemolFirstConvo1)
					{
						key = "RR_HEGEMOL_FIRST_1_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["HNeutral1"]);
						FiveKnights.Instance.SaveSettings.HegemolFirstConvo1 = true;
					}
					else if (!FiveKnights.Instance.SaveSettings.HegemolFirstConvo2)
					{
						key = "RR_HEGEMOL_FIRST_2_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["HNeutral2"]);
						FiveKnights.Instance.SaveSettings.HegemolFirstConvo2 = true;
					}
					else if (FiveKnights.Instance.SaveSettings.equippedCharms[2] && !FiveKnights.Instance.SaveSettings.HegemolCharmConvo)
					{
						key = "RR_HEGEMOL_CHARM_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["HNeutral2"]);
						FiveKnights.Instance.SaveSettings.HegemolCharmConvo = true;
					}
					else
					{
						HeroController.instance.PlayAudio(FiveKnights.Clips["HNeutral3"]);
						key = "RR_HEGEMOL_FIRST_REPEAT";
					}
				}
				else if (FiveKnights.Instance.SaveSettings.ChampionsCallClears == 2)
				{
					if (!FiveKnights.Instance.SaveSettings.HegemolSecondConvo1)
					{
						key = "RR_HEGEMOL_SECOND_1_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["HNeutral1"]);
						FiveKnights.Instance.SaveSettings.HegemolSecondConvo1 = true;
					}
					else if (!FiveKnights.Instance.SaveSettings.HegemolSecondConvo2)
					{
						key = "RR_HEGEMOL_SECOND_2_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["HNeutral2"]);
						FiveKnights.Instance.SaveSettings.HegemolSecondConvo2 = true;
					}
					else if (FiveKnights.Instance.SaveSettings.equippedCharms[2] && !FiveKnights.Instance.SaveSettings.HegemolCharmConvo)
					{
						key = "RR_HEGEMOL_CHARM_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["HNeutral2"]);
						FiveKnights.Instance.SaveSettings.HegemolCharmConvo = true;
					}
					else
					{
						HeroController.instance.PlayAudio(FiveKnights.Clips["HNeutral3"]);
						key = "RR_HEGEMOL_SECOND_REPEAT";
					}
				}
				else if (FiveKnights.Instance.SaveSettings.ChampionsCallClears >= 3)
				{
					if (!FiveKnights.Instance.SaveSettings.HegemolThirdConvo1)
					{
						key = "RR_HEGEMOL_THIRD_1_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["HNeutral1"]);
						FiveKnights.Instance.SaveSettings.HegemolThirdConvo1 = true;
					}
					else if (FiveKnights.Instance.SaveSettings.equippedCharms[2] && !FiveKnights.Instance.SaveSettings.HegemolCharmConvo)
					{
						HeroController.instance.PlayAudio(FiveKnights.Clips["HNeutral2"]);
						key = "RR_HEGEMOL_CHARM_1";
						FiveKnights.Instance.SaveSettings.HegemolCharmConvo = true;
					}
					else
					{
						HeroController.instance.PlayAudio(FiveKnights.Clips["HNeutral3"]);
						key = "RR_HEGEMOL_THIRD_REPEAT";
					}
				}
				else
				{
					key = "RR_HEGEMOL_CHEATER";
				}
				return new DialogueOptions
				{
					Key = key,
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Wait = PlayAnimHegemol(),
					Continue = true
				};
			}
			return prev.Key switch
			{
				"RR_HEGEMOL_FIRST_1_1" => new DialogueOptions
				{
					Key = "RR_HEGEMOL_FIRST_1_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_HEGEMOL_FIRST_1_2" => new DialogueOptions
				{
					Key = "RR_HEGEMOL_FIRST_1_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_HEGEMOL_FIRST_2_1" => new DialogueOptions
				{
					Key = "RR_HEGEMOL_FIRST_2_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_HEGEMOL_FIRST_2_2" => new DialogueOptions
				{
					Key = "RR_HEGEMOL_FIRST_2_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_HEGEMOL_FIRST_2_3" => new DialogueOptions
				{
					Key = "RR_HEGEMOL_FIRST_2_4",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_HEGEMOL_SECOND_1_1" => new DialogueOptions
				{
					Key = "RR_HEGEMOL_SECOND_1_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_HEGEMOL_SECOND_1_2" => new DialogueOptions
				{
					Key = "RR_HEGEMOL_SECOND_1_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_HEGEMOL_SECOND_1_3" => new DialogueOptions
				{
					Key = "RR_HEGEMOL_SECOND_1_4",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_HEGEMOL_SECOND_2_1" => new DialogueOptions
				{
					Key = "RR_HEGEMOL_SECOND_2_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_HEGEMOL_SECOND_2_2" => new DialogueOptions
				{
					Key = "RR_HEGEMOL_SECOND_2_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_HEGEMOL_THIRD_1_1" => new DialogueOptions
				{
					Key = "RR_HEGEMOL_THIRD_1_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_HEGEMOL_CHARM_1" => new DialogueOptions
				{
					Key = "RR_HEGEMOL_CHARM_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_HEGEMOL_CHARM_2" => new DialogueOptions
				{
					Key = "RR_HEGEMOL_CHARM_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				_ => new DialogueOptions
				{
					Continue = false,
					Wait = StopAnimHegemol()
				}, 
			};
			static IEnumerator PlayAnimHegemol()
			{
				yield return new WaitForEndOfFrame();
				yield return new WaitUntil(() => hegemolAnim.GetCurrentFrame() < 2);
				hegemolAnim.enabled = false;
				yield return new WaitForSeconds(0.1f);
				hegemolAnim.enabled = true;
				yield return hegemolAnim.PlayBlocking("TurnLeft");
				hegemolAnim.Play("Talk");
			}
			static IEnumerator StopAnimHegemol()
			{
				yield return hegemolAnim.PlayBlocking("TurnRight");
				hegemolAnim.Play("Idle");
			}
		}

		private static DialogueOptions ZemerDialogue(DialogueCallbackOptions prev)
		{
			if (!prev.Continue)
			{
				string key;
				if (FiveKnights.Instance.SaveSettings.ChampionsCallClears == 1)
				{
					if (!FiveKnights.Instance.SaveSettings.ZemerFirstConvo1)
					{
						key = "RR_ZEMER_FIRST_1_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["ZAudTalk2"]);
						FiveKnights.Instance.SaveSettings.ZemerFirstConvo1 = true;
					}
					else if (!FiveKnights.Instance.SaveSettings.ZemerFirstConvo2)
					{
						key = "RR_ZEMER_FIRST_2_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["ZAudTalk1"]);
						FiveKnights.Instance.SaveSettings.ZemerFirstConvo2 = true;
					}
					else if (FiveKnights.Instance.SaveSettings.equippedCharms[1] && !FiveKnights.Instance.SaveSettings.ZemerCharmConvo)
					{
						key = "RR_ZEMER_CHARM_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["ZAudTalk3"]);
						FiveKnights.Instance.SaveSettings.ZemerCharmConvo = true;
					}
					else
					{
						HeroController.instance.PlayAudio(FiveKnights.Clips["ZAudTalk4"]);
						key = "RR_ZEMER_FIRST_REPEAT";
					}
				}
				else if (FiveKnights.Instance.SaveSettings.ChampionsCallClears == 2)
				{
					if (!FiveKnights.Instance.SaveSettings.ZemerSecondConvo1)
					{
						key = "RR_ZEMER_SECOND_1_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["ZAudTalk1B"]);
						FiveKnights.Instance.SaveSettings.ZemerSecondConvo1 = true;
					}
					else if (!FiveKnights.Instance.SaveSettings.ZemerSecondConvo2)
					{
						key = "RR_ZEMER_SECOND_2_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["ZAudTalk2"]);
						FiveKnights.Instance.SaveSettings.ZemerSecondConvo2 = true;
					}
					else if (FiveKnights.Instance.SaveSettings.equippedCharms[1] && !FiveKnights.Instance.SaveSettings.ZemerCharmConvo)
					{
						key = "RR_ZEMER_CHARM_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["ZAudTalk3"]);
						FiveKnights.Instance.SaveSettings.ZemerCharmConvo = true;
					}
					else
					{
						HeroController.instance.PlayAudio(FiveKnights.Clips["ZAudTalk4"]);
						key = "RR_ZEMER_SECOND_REPEAT";
					}
				}
				else if (FiveKnights.Instance.SaveSettings.ChampionsCallClears >= 3)
				{
					if (!FiveKnights.Instance.SaveSettings.ZemerThirdConvo1)
					{
						key = "RR_ZEMER_THIRD_1_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["ZAudTalk1"]);
						FiveKnights.Instance.SaveSettings.ZemerThirdConvo1 = true;
					}
					else if (FiveKnights.Instance.SaveSettings.equippedCharms[1] && !FiveKnights.Instance.SaveSettings.ZemerCharmConvo)
					{
						key = "RR_ZEMER_CHARM_1";
						HeroController.instance.PlayAudio(FiveKnights.Clips["ZAudTalk3"]);
						FiveKnights.Instance.SaveSettings.ZemerCharmConvo = true;
					}
					else
					{
						HeroController.instance.PlayAudio(FiveKnights.Clips["ZAudTalk4"]);
						key = "RR_ZEMER_THIRD_REPEAT";
					}
				}
				else
				{
					key = "RR_ZEMER_CHEATER";
				}
				return new DialogueOptions
				{
					Key = key,
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Wait = PlayAnimZemer(),
					Continue = true
				};
			}
			return prev.Key switch
			{
				"RR_ZEMER_FIRST_1_1" => new DialogueOptions
				{
					Key = "RR_ZEMER_FIRST_1_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ZEMER_FIRST_1_2" => new DialogueOptions
				{
					Key = "RR_ZEMER_FIRST_1_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ZEMER_FIRST_2_1" => new DialogueOptions
				{
					Key = "RR_ZEMER_FIRST_2_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ZEMER_FIRST_2_2" => new DialogueOptions
				{
					Key = "RR_ZEMER_FIRST_2_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ZEMER_FIRST_2_3" => new DialogueOptions
				{
					Key = "RR_ZEMER_FIRST_2_4",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ZEMER_SECOND_1_1" => new DialogueOptions
				{
					Key = "RR_ZEMER_SECOND_1_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ZEMER_SECOND_1_2" => new DialogueOptions
				{
					Key = "RR_ZEMER_SECOND_1_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ZEMER_SECOND_1_3" => new DialogueOptions
				{
					Key = "RR_ZEMER_SECOND_1_4",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ZEMER_SECOND_2_1" => new DialogueOptions
				{
					Key = "RR_ZEMER_SECOND_2_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ZEMER_SECOND_2_2" => new DialogueOptions
				{
					Key = "RR_ZEMER_SECOND_2_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ZEMER_THIRD_1_1" => new DialogueOptions
				{
					Key = "RR_ZEMER_THIRD_1_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ZEMER_THIRD_1_2" => new DialogueOptions
				{
					Key = "RR_ZEMER_THIRD_1_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ZEMER_CHARM_1" => new DialogueOptions
				{
					Key = "RR_ZEMER_CHARM_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ZEMER_CHARM_2" => new DialogueOptions
				{
					Key = "RR_ZEMER_CHARM_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ZEMER_OLDNAIL_1" => new DialogueOptions
				{
					Key = "RR_ZEMER_OLDNAIL_2",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				"RR_ZEMER_OLDNAIL_2" => new DialogueOptions
				{
					Key = "RR_ZEMER_OLDNAIL_3",
					Sheet = "Reward Room",
					Type = DialogueType.Normal,
					Continue = true
				}, 
				_ => new DialogueOptions
				{
					Continue = false,
					Wait = StopAnimZemer()
				}, 
			};
			static IEnumerator PlayAnimZemer()
			{
				if (zemerAnimCoro != null)
				{
					GameManager.instance.StopCoroutine(zemerAnimCoro);
				}
				if (zemerAnim.IsPlaying("IdleAlt"))
				{
					zemerAnim.Play("IdleFromAlt");
					yield return null;
					yield return new WaitWhile(() => zemerAnim.IsPlaying("IdleFromAlt"));
				}
				else if (zemerAnim.IsPlaying("IdleFromAlt"))
				{
					yield return new WaitWhile(() => zemerAnim.IsPlaying("IdleFromAlt"));
				}
				else if (zemerAnim.IsPlaying("IdleToAlt"))
				{
					yield return new WaitWhile(() => zemerAnim.IsPlaying("IdleToAlt"));
					zemerAnim.Play("IdleFromAlt");
					yield return null;
					yield return new WaitWhile(() => zemerAnim.IsPlaying("IdleFromAlt"));
				}
				zemerAnim.Play("Talk");
			}
			static IEnumerator StopAnimZemer()
			{
				yield return new WaitWhile(() => zemerAnim.GetCurrentFrame() > 0 && zemerAnim.GetCurrentFrame() < 6);
				zemerAnim.Play("Idle");
				zemerAnimCoro = GameManager.instance.StartCoroutine(ZemerAnimControl());
			}
		}

		private static IEnumerator ZemerAnimControl()
		{
			while (zemerAnim != null)
			{
				yield return new WaitForSeconds(3f);
				zemerAnim.Play("IdleToAlt");
				yield return null;
				yield return new WaitWhile(() => zemerAnim.IsPlaying("IdleToAlt"));
				zemerAnim.Play("IdleAlt");
				yield return new WaitForSeconds(0.75f);
				zemerAnim.Play("IdleFromAlt");
				yield return null;
				yield return new WaitWhile(() => zemerAnim.IsPlaying("IdleFromAlt"));
				zemerAnim.Play("Idle");
			}
		}
	}
	public class SaveModSettings
	{
		public BossStatue.Completion CompletionIsma = BossStatue.Completion.None;

		public BossStatue.Completion CompletionIsma2 = BossStatue.Completion.None;

		public BossStatue.Completion CompletionZemer = BossStatue.Completion.None;

		public BossStatue.Completion CompletionZemer2 = BossStatue.Completion.None;

		public BossStatue.Completion CompletionDryya = BossStatue.Completion.None;

		public BossStatue.Completion CompletionHegemol = BossStatue.Completion.None;

		public BossStatue.Completion CompletionMawlek2 = BossStatue.Completion.None;

		public JournalHelper.JournalPlayerData IsmaEntryData = new JournalHelper.JournalPlayerData
		{
			haskilled = false,
			Hidden = true,
			killsremaining = 1,
			newentry = true
		};

		public JournalHelper.JournalPlayerData ZemerEntryData = new JournalHelper.JournalPlayerData
		{
			haskilled = false,
			Hidden = true,
			killsremaining = 1,
			newentry = true
		};

		public JournalHelper.JournalPlayerData DryyaEntryData = new JournalHelper.JournalPlayerData
		{
			haskilled = false,
			Hidden = true,
			killsremaining = 1,
			newentry = true
		};

		public JournalHelper.JournalPlayerData HegemolEntryData = new JournalHelper.JournalPlayerData
		{
			haskilled = false,
			Hidden = true,
			killsremaining = 1,
			newentry = true
		};

		public JournalHelper.JournalPlayerData Mawlek2EntryData = new JournalHelper.JournalPlayerData
		{
			haskilled = false,
			Hidden = true,
			killsremaining = 1,
			newentry = true
		};

		public int IsmaOWWinCount;

		public int DryyaOWWinCount;

		public int HegOWWinCount;

		public int ZemerOWWinCount;

		public bool AltStatueIsma;

		public bool AltStatueZemer;

		public bool AltStatueMawlek;

		public string respawnMarkerName;

		public string respawnScene;

		public bool[] newCharms = new bool[4] { true, true, true, true };

		public bool[] gotCharms = new bool[4];

		public bool[] equippedCharms = new bool[4];

		public bool upgradedCharm_10;

		public bool GreetedNailsmith;

		public bool GreetedSheo;

		public bool IsmaFirstConvo1;

		public bool IsmaFirstConvo2;

		public bool IsmaSecondConvo1;

		public bool IsmaSecondConvo2;

		public bool IsmaThirdConvo1;

		public bool IsmaCharmConvo;

		public bool OgrimFirstConvo1;

		public bool OgrimFirstConvo2;

		public bool OgrimSecondConvo1;

		public bool OgrimSecondConvo2;

		public bool OgrimThirdConvo1;

		public bool OgrimCharmConvo;

		public bool DryyaFirstConvo1;

		public bool DryyaFirstConvo2;

		public bool DryyaSecondConvo1;

		public bool DryyaSecondConvo2;

		public bool DryyaThirdConvo1;

		public bool DryyaCharmConvo;

		public bool DryyaOldNailConvo;

		public bool HegemolFirstConvo1;

		public bool HegemolFirstConvo2;

		public bool HegemolSecondConvo1;

		public bool HegemolSecondConvo2;

		public bool HegemolThirdConvo1;

		public bool HegemolCharmConvo;

		public bool ZemerFirstConvo1;

		public bool ZemerFirstConvo2;

		public bool ZemerSecondConvo1;

		public bool ZemerSecondConvo2;

		public bool ZemerThirdConvo1;

		public bool ZemerCharmConvo;

		public bool WLadyKHConvo1;

		public bool WLadyKHConvo2;

		public bool WLadyMPConvo1;

		public bool WLadyMPConvo2;

		public bool WLadyBHConvo1;

		public bool WLadyBHConvo2;

		public bool WLadyVLConvo1;

		public bool WLadyVLConvo2;

		public bool WLadyAllConvo1;

		public bool WLadyAllConvo2;

		public bool GodseekerSave;

		public bool HasSeenWorkshopRaised;

		public bool SeenChampionsCall;

		public int ChampionsCallClears;

		public bool ChampionsCallHitless;

		public bool HasSeenCredits;

		public bool IndicatorActivated;

		public Vector3 IndicatorPosition1;

		public Vector3 IndicatorPosition2;

		public bool UnlockedChampionsCall
		{
			get
			{
				if (!GodseekerSave)
				{
					if (CompletionIsma.isUnlocked && CompletionDryya.isUnlocked && CompletionHegemol.isUnlocked)
					{
						return CompletionZemer.isUnlocked;
					}
					return false;
				}
				if ((CompletionIsma.completedTier1 || CompletionIsma.completedTier2) && (CompletionDryya.completedTier1 || CompletionDryya.completedTier2) && (CompletionHegemol.completedTier1 || CompletionHegemol.completedTier2))
				{
					if (!CompletionZemer.completedTier1)
					{
						return CompletionZemer.completedTier2;
					}
					return true;
				}
				return false;
			}
		}
	}
	public class StatueControl : MonoBehaviour
	{
		public BossStatue _bs;

		public SpriteRenderer _sr;

		public GameObject _fakeStat;

		public GameObject _fakeStatAlt;

		public GameObject _fakeStatAlt2;

		public string StatueName;

		private bool canToggle;

		private IEnumerator Start()
		{
			yield return null;
			yield return null;
			yield return null;
			yield return null;
			canToggle = true;
			if (StatueName.Contains("Isma"))
			{
				if (FiveKnights.Instance.SaveSettings.AltStatueIsma)
				{
					_fakeStat = _fakeStatAlt;
					_fakeStat.SetActive(value: false);
					_fakeStat = _fakeStatAlt2;
					_fakeStat.transform.position = new Vector3(82.2f, 95.9f, 2f);
					_fakeStatAlt.transform.position = new Vector3(82.2f, 91.6f, 2f);
					_fakeStat.SetActive(value: true);
				}
				else
				{
					_fakeStat = _fakeStatAlt2;
					_fakeStat.SetActive(value: false);
					_fakeStat = _fakeStatAlt;
					_sr.sprite = FiveKnights.SPRITES[FiveKnights.Instance.SaveSettings.CompletionIsma.completedTier3 ? "Isma_Silver" : "Isma"];
					_fakeStat.transform.position = new Vector3(82.2f, 96.5f, 2f);
					_fakeStatAlt2.transform.position = new Vector3(82.2f, 90.9f, 2f);
					_fakeStat.SetActive(_bs.StatueState.isUnlocked && _bs.StatueState.hasBeenSeen);
				}
			}
			else if (FiveKnights.Instance.SaveSettings.AltStatueZemer)
			{
				_fakeStat = _fakeStatAlt;
				_sr.sprite = FiveKnights.SPRITES[FiveKnights.Instance.SaveSettings.CompletionZemer2.completedTier3 ? "Zemer_Silver" : "Zemer"];
				_sr.flipX = FiveKnights.Instance.SaveSettings.AltStatueZemer;
			}
			else
			{
				_fakeStat = _fakeStatAlt;
				_sr.sprite = FiveKnights.SPRITES[FiveKnights.Instance.SaveSettings.CompletionZemer.completedTier3 ? "Zemer_Silver" : "Zemer"];
			}
			StatueName = (StatueName.Contains("Isma") ? "Isma" : "Zemer");
		}

		public void StartLever(BossStatueLever self)
		{
			if (canToggle)
			{
				if (StatueName == "Isma")
				{
					StartCoroutine(SwapStatues(self));
					FiveKnights.Instance.SaveSettings.AltStatueIsma = !FiveKnights.Instance.SaveSettings.AltStatueIsma;
					_bs.SetDreamVersion(FiveKnights.Instance.SaveSettings.AltStatueIsma, useAltStatue: false, doAnim: false);
				}
				else if (StatueName == "Zemer")
				{
					StartCoroutine(SwapStatues(self));
					FiveKnights.Instance.SaveSettings.AltStatueZemer = !FiveKnights.Instance.SaveSettings.AltStatueZemer;
					_bs.SetDreamVersion(FiveKnights.Instance.SaveSettings.AltStatueZemer, useAltStatue: false, doAnim: false);
				}
				canToggle = false;
				self.switchSound.SpawnAndPlayOneShot(self.audioPlayerPrefab, base.transform.position);
				GameManager.instance.FreezeMoment(1);
				GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
				if ((bool)self.strikeNailPrefab && (bool)self.hitOrigin)
				{
					self.strikeNailPrefab.Spawn(self.hitOrigin.transform.position);
				}
				if ((bool)self.leverAnimator)
				{
					self.leverAnimator.Play("Hit");
				}
			}
		}

		private IEnumerator SwapStatues(BossStatueLever lev)
		{
			canToggle = false;
			if ((bool)_bs.bossUIControlFSM)
			{
				FSMUtility.SendEventToGameObject(_bs.bossUIControlFSM.gameObject, "NPC CONTROL OFF");
			}
			yield return new WaitForSeconds(0.25f);
			if ((bool)_bs.statueShakeParticles)
			{
				_bs.statueShakeParticles.Play();
			}
			if ((bool)_bs.statueShakeLoop)
			{
				_bs.statueShakeLoop.Play();
			}
			yield return StartCoroutine(Jitter(_bs.shakeTime, 0.1f, _fakeStat));
			if ((bool)_bs.statueShakeLoop)
			{
				_bs.statueShakeLoop.Stop();
			}
			StartCoroutine(PlayAudioEventDelayed(_bs.statueDownSound, _bs.statueDownSoundDelay));
			float time = 0.5f;
			yield return StartCoroutine(PlayAnimWait(_fakeStat, "Down", time));
			yield return new WaitForSeconds(time);
			StartCoroutine(PlayParticlesDelay(_bs.statueUpParticles, _bs.upParticleDelay));
			StartCoroutine(PlayAudioEventDelayed(_bs.statueUpSound, _bs.statueUpSoundDelay));
			if (StatueName.Contains("Isma"))
			{
				if (FiveKnights.Instance.SaveSettings.AltStatueIsma)
				{
					_fakeStat.SetActive(value: false);
					_fakeStat = _fakeStatAlt2;
					_fakeStat.SetActive(value: true);
				}
				else
				{
					_fakeStat.SetActive(value: false);
					_fakeStat = _fakeStatAlt;
					_sr.sprite = FiveKnights.SPRITES[FiveKnights.Instance.SaveSettings.CompletionIsma.completedTier3 ? "Isma_Silver" : "Isma"];
					_fakeStat.SetActive(value: true);
				}
			}
			else
			{
				if (FiveKnights.Instance.SaveSettings.AltStatueZemer)
				{
					_sr.sprite = FiveKnights.SPRITES[FiveKnights.Instance.SaveSettings.CompletionZemer2.completedTier3 ? "Zemer_Silver" : "Zemer"];
				}
				else
				{
					_sr.sprite = FiveKnights.SPRITES[FiveKnights.Instance.SaveSettings.CompletionZemer.completedTier3 ? "Zemer_Silver" : "Zemer"];
				}
				_sr.flipX = FiveKnights.Instance.SaveSettings.AltStatueZemer;
			}
			yield return StartCoroutine(PlayAnimWait(_fakeStat, "Up", time));
			if ((bool)_bs.bossUIControlFSM)
			{
				FSMUtility.SendEventToGameObject(_bs.bossUIControlFSM.gameObject, "CONVO CANCEL");
			}
			lev.leverAnimator.Play("Shine");
			canToggle = true;
		}

		private IEnumerator Jitter(float duration, float magnitude, GameObject obj)
		{
			Transform sprite = obj.transform;
			Vector3 initialPos = sprite.position;
			float elapsed = 0f;
			float half = magnitude / 2f;
			for (; elapsed < duration; elapsed += Time.deltaTime)
			{
				sprite.position = initialPos + new Vector3(UnityEngine.Random.Range(0f - half, half), UnityEngine.Random.Range(0f - half, half), 0f);
				yield return null;
			}
			sprite.position = initialPos;
		}

		private IEnumerator PlayAnimWait(GameObject go, string stateName, float normalizedTime)
		{
			float t = normalizedTime;
			float mid = t / 2f;
			if (stateName == "Down")
			{
				while (t >= 0f)
				{
					if (t <= mid)
					{
						go.SetActive(value: false);
					}
					go.transform.position -= new Vector3(0f, 0.2f, 0f);
					t -= Time.fixedDeltaTime;
					yield return new WaitForEndOfFrame();
				}
			}
			if (!(stateName == "Up"))
			{
				yield break;
			}
			while (t >= 0f)
			{
				if (t <= mid)
				{
					go.SetActive(value: true);
				}
				go.transform.position += new Vector3(0f, 0.2f, 0f);
				t -= Time.fixedDeltaTime;
				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator PlayParticlesDelay(ParticleSystem system, float delay)
		{
			if ((bool)system)
			{
				yield return new WaitForSeconds(delay);
				system.Play();
			}
		}

		private IEnumerator PlayAudioEventDelayed(AudioEvent audioEvent, float delay)
		{
			yield return new WaitForSeconds(delay);
			audioEvent.SpawnAndPlayOneShot(_bs.audioSourcePrefab, _bs.transform.position);
		}

		private void OnDestroy()
		{
		}
	}
	internal class Tink : MonoBehaviour
	{
		internal static AudioClip TinkClip { get; set; }

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!(other.gameObject.name != "Clash Tink") || other.gameObject.CompareTag("Nail Attack"))
			{
				GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
				this.PlayAudio(TinkClip, 1f, 0.15f);
			}
		}
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
}
namespace FiveKnights.Zemer
{
	public class ExtraNailBndCheck : MonoBehaviour
	{
		private Rigidbody2D _rb;

		private readonly float _nailMaxHeightStop = (OWArenaFinder.IsInOverWorld ? 118.5f : ((CustomWP.boss == CustomWP.Boss.All || CustomWP.boss == CustomWP.Boss.Ogrim) ? 18.5f : 39f));

		private readonly float _nailMaxGroundStop = (OWArenaFinder.IsInOverWorld ? 106.8f : ((CustomWP.boss == CustomWP.Boss.All || CustomWP.boss == CustomWP.Boss.Ogrim) ? 3f : 27.3f));

		private readonly float _nailMaxLeftStop = (OWArenaFinder.IsInOverWorld ? 241.9f : ((CustomWP.boss == CustomWP.Boss.All || CustomWP.boss == CustomWP.Boss.Ogrim) ? 62f : 13f));

		private readonly float _nailMaxRightStop = (OWArenaFinder.IsInOverWorld ? 271.2f : ((CustomWP.boss == CustomWP.Boss.All || CustomWP.boss == CustomWP.Boss.Ogrim) ? 89f : 43f));

		private void Awake()
		{
			_rb = GetComponent<Rigidbody2D>();
		}

		private void FixedUpdate()
		{
			Vector2 vector = base.transform.position;
			if (vector.x > _nailMaxRightStop && _rb.velocity.x > 0f)
			{
				_rb.velocity = Vector2.zero;
				base.transform.position = new Vector3(_nailMaxRightStop, vector.y);
			}
			else if (vector.x < _nailMaxLeftStop && _rb.velocity.x < 0f)
			{
				_rb.velocity = Vector2.zero;
				base.transform.position = new Vector3(_nailMaxLeftStop, vector.y);
			}
			else if (vector.y > _nailMaxHeightStop && _rb.velocity.y > 0f)
			{
				_rb.velocity = Vector2.zero;
				base.transform.position = new Vector3(vector.x, _nailMaxHeightStop);
			}
			else if (vector.y < _nailMaxGroundStop && _rb.velocity.y < 0f)
			{
				Modding.Logger.Log("Stopped at gnd zem");
				_rb.velocity = Vector2.zero;
				base.transform.position = new Vector3(vector.x, _nailMaxGroundStop);
			}
		}
	}
	public class TChildCtrl : MonoBehaviour
	{
		private BoxCollider2D _bc;

		private HealthManager _hm;

		private EnemyHitEffectsUninfected _hitEffects;

		private SpriteRenderer _sr;

		private Rigidbody2D _rb;

		private Animator _anim;

		private bool _isAtt;

		private bool _run;

		private const float GroundY = 107.32f;

		private const float StopAtX = 265.9f;

		public bool helpZemer;

		public GameObject zemer;

		private bool leaveAndRet;

		private void Awake()
		{
			On.HealthManager.TakeDamage += HealthManagerOnTakeDamage;
			_bc = GetComponent<BoxCollider2D>();
			_hm = base.gameObject.AddComponent<HealthManager>();
			_hitEffects = base.gameObject.AddComponent<EnemyHitEffectsUninfected>();
			_sr = GetComponent<SpriteRenderer>();
			_rb = GetComponent<Rigidbody2D>();
			_anim = GetComponent<Animator>();
			base.gameObject.AddComponent<DamageHero>().damageDealt = 1;
			base.gameObject.AddComponent<Flash>();
			AssignFields();
		}

		private IEnumerator Start()
		{
			base.gameObject.layer = 11;
			_hitEffects.enabled = true;
			leaveAndRet = false;
			_hm.enabled = false;
			_bc.enabled = false;
			base.transform.position = new Vector3(171.9f, 107.32f, 0f);
			base.transform.localScale *= 1.2f;
			_anim.Play("Idle");
			yield return new WaitWhile(() => HeroController.instance == null);
			GameObject go = GameObject.Find("Dream Entry");
			PlayMakerFSM fsm = go.LocateMyFSM("Control");
			yield return new WaitWhile(() => !fsm.ActiveStateName.Contains("Anim"));
			StartCoroutine(LookAndRun());
			StartCoroutine(FreezePlayer());
		}

		private void Update()
		{
			if (!_run && !_isAtt && !(HeroController.instance.transform.position.x < base.transform.position.x) && !leaveAndRet)
			{
				_run = true;
				StartCoroutine(TPAway());
			}
		}

		private IEnumerator LookAndRun()
		{
			yield return _anim.PlayBlocking("Turn");
			yield return new WaitForSeconds(0.25f);
			yield return _anim.PlayToFrame("RunStart", 2);
			_rb.velocity = new Vector2(17.5f, 0f);
			yield return _anim.PlayToEnd();
			_anim.speed *= 1.4f;
			_rb.velocity = new Vector2(15f, 0f);
			_anim.Play("Run");
			yield return new WaitWhile(() => base.transform.position.x < 265.9f && !_isAtt && !_run);
			if (_isAtt)
			{
				StartCoroutine(DoAttack());
			}
			else if (!_run)
			{
				_anim.speed = 1f;
				_rb.velocity = Vector2.zero;
				yield return _anim.PlayBlocking("Turn");
				_hm.IsInvincible = true;
				BoxCollider2D bc = _bc;
				HealthManager hm = _hm;
				bool flag = false;
				hm.enabled = false;
				bc.enabled = flag;
				leaveAndRet = true;
				StartCoroutine(LeaveAndReturn());
			}
		}

		private IEnumerator FreezePlayer()
		{
			yield return new WaitWhile(() => HeroController.instance.transform.position.x < 253f);
			OWBossManager.PlayMusic(null);
			PlayMakerFSM playMakerFSM = HeroController.instance.gameObject.LocateMyFSM("Roar Lock");
			playMakerFSM.GetFsmGameObjectVariable("Roar Object").Value = base.gameObject;
			playMakerFSM.SendEvent("ROAR ENTER");
		}

		private IEnumerator TPAway()
		{
			yield return Leave(unfreezeH: false);
			yield return Arrive(new Vector3(265.9f, 107.32f));
			StartCoroutine(LeaveAndReturn());
		}

		private IEnumerator LeaveAndReturn()
		{
			_hm.IsInvincible = true;
			yield return new WaitWhile(() => HeroController.instance.transform.position.x < 253f);
			yield return new WaitForSeconds(1.5f);
			yield return Leave(unfreezeH: true);
			yield return new WaitForSeconds(1f);
			yield return Arrive(new Vector3(259.1f, 108.4f, 7f));
			StartCoroutine(HeadTurn());
		}

		private IEnumerator HeadTurn()
		{
			Dictionary<int, (int, int)> ranges = new Dictionary<int, (int, int)>
			{
				[-2] = (0, 252),
				[-1] = (252, 258),
				[0] = (258, 264),
				[1] = (264, 500)
			};
			Dictionary<int, int> typeToFrameFromLeft = new Dictionary<int, int>
			{
				[-2] = 0,
				[-1] = 1,
				[0] = 3,
				[1] = 5
			};
			Dictionary<int, int> typeToFrameFromRight = new Dictionary<int, int>
			{
				[-2] = 5,
				[-1] = 4,
				[0] = 2,
				[1] = 0
			};
			int oldType = 0;
			while (zemer != null)
			{
				float x = zemer.transform.position.x;
				if (x > (float)ranges[oldType].Item1 && x < (float)ranges[oldType].Item2)
				{
					yield return null;
					continue;
				}
				int newType = FindCorrectRange(x);
				_anim.enabled = true;
				if (newType > oldType)
				{
					yield return _anim.PlayToFrameAt("LookRight", typeToFrameFromLeft[oldType], typeToFrameFromLeft[newType]);
				}
				else
				{
					yield return _anim.PlayToFrameAt("LookLeft", typeToFrameFromRight[oldType], typeToFrameFromRight[newType]);
				}
				_anim.enabled = false;
				oldType = newType;
			}
			int FindCorrectRange(float xPos)
			{
				return ranges.Where(delegate(KeyValuePair<int, (int, int)> kp)
				{
					float num = xPos;
					KeyValuePair<int, (int, int)> keyValuePair = kp;
					if (num > (float)keyValuePair.Value.Item1)
					{
						float num2 = xPos;
						keyValuePair = kp;
						return num2 < (float)keyValuePair.Value.Item2;
					}
					return false;
				}).Select(delegate(KeyValuePair<int, (int, int)> kp)
				{
					KeyValuePair<int, (int, int)> keyValuePair = kp;
					return keyValuePair.Key;
				}).FirstOrDefault();
			}
		}

		private IEnumerator Leave(bool unfreezeH)
		{
			_anim.speed = 1f;
			yield return _anim.PlayToFrame("Leave", 2);
			if (unfreezeH)
			{
				helpZemer = true;
				HeroController.instance.gameObject.LocateMyFSM("Roar Lock").SendEvent("ROAR EXIT");
			}
			_rb.velocity = new Vector2(10f, 0f);
			yield return null;
			yield return new WaitWhile(() => _anim.GetCurrentFrame() < 9);
			_rb.velocity = new Vector2(10f, 10f);
			yield return _anim.PlayToEnd();
			_rb.velocity = Vector2.zero;
			_sr.enabled = false;
			if (_bc != null)
			{
				UnityEngine.Object.Destroy(_bc);
			}
		}

		private IEnumerator Arrive(Vector3 pos)
		{
			_anim.Play("Back");
			_sr.enabled = true;
			base.transform.position = pos;
			yield return _anim.PlayToEnd();
			if (_bc != null)
			{
				UnityEngine.Object.Destroy(_bc);
			}
		}

		private void HealthManagerOnTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitinstance)
		{
			if (!_run && !_isAtt && self.name.Contains("TChild"))
			{
				_hitEffects.RecieveHitEffect(hitinstance.Direction);
				_isAtt = true;
			}
			orig(self, hitinstance);
		}

		private IEnumerator DoAttack()
		{
			_anim.speed = 1f;
			_rb.velocity = Vector2.zero;
			_anim.Play("StartParry");
			yield return _anim.PlayToEnd();
			_anim.Play("Parry");
			yield return _anim.PlayToEnd();
			yield return Leave(unfreezeH: false);
			yield return Arrive(new Vector3(265.9f, 107.32f));
			StartCoroutine(LeaveAndReturn());
		}

		private void AssignFields()
		{
			_hm.hp = 10000;
			GameObject gameObject = FiveKnights.preloadedGO["WD"];
			HealthManager component = gameObject.GetComponent<HealthManager>();
			foreach (FieldInfo item in from x in typeof(HealthManager).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
				where x.Name.Contains("Prefab")
				select x)
			{
				item.SetValue(_hm, item.GetValue(component));
			}
			EnemyHitEffectsUninfected component2 = gameObject.GetComponent<EnemyHitEffectsUninfected>();
			FieldInfo[] fields = typeof(EnemyHitEffectsUninfected).GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.Name.Contains("Origin"))
				{
					_hitEffects.effectOrigin = new Vector3(0f, 0.5f, 0f);
				}
				else
				{
					fieldInfo.SetValue(_hitEffects, fieldInfo.GetValue(component2));
				}
			}
		}

		private void OnDestroy()
		{
			Modding.Logger.Log("Checking to make sure bruh");
			On.HealthManager.TakeDamage -= HealthManagerOnTakeDamage;
		}
	}
	public class WaveIncrease : MonoBehaviour
	{
		private void Update()
		{
			base.transform.localScale += new Vector3(0.02f, 0.02f, 0f);
			if (base.transform.localScale.x > 5f)
			{
				Modding.Logger.Log("KILL");
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
	public class ZemerController : MonoBehaviour
	{
		private HealthManager _hm;

		private BoxCollider2D _bc;

		private SpriteRenderer _sr;

		private EnemyDreamnailReaction _dreamNailReaction;

		private GameObject _dd;

		private GameObject _dnailEff;

		private Animator _anim;

		private Rigidbody2D _rb;

		private System.Random _rand;

		private ExtraDamageable _extraDamageable;

		private EnemyHitEffectsUninfected _hitEffects;

		private GameObject _target;

		private readonly float GroundY = (OWArenaFinder.IsInOverWorld ? 108.3f : ((CustomWP.boss == CustomWP.Boss.All || CustomWP.boss == CustomWP.Boss.Ogrim) ? 9f : 28.8f));

		private readonly float LeftX = (OWArenaFinder.IsInOverWorld ? 240.1f : ((CustomWP.boss == CustomWP.Boss.All || CustomWP.boss == CustomWP.Boss.Ogrim) ? 61f : 11.2f));

		private readonly float RightX = (OWArenaFinder.IsInOverWorld ? 273.9f : ((CustomWP.boss == CustomWP.Boss.All || CustomWP.boss == CustomWP.Boss.Ogrim) ? 91f : 45.7f));

		private readonly float SlamY = (OWArenaFinder.IsInOverWorld ? 105f : ((CustomWP.boss == CustomWP.Boss.All || CustomWP.boss == CustomWP.Boss.Ogrim) ? 6f : 25.9f));

		private readonly int MaxHPV2 = 400;

		private readonly int MaxHPV1 = ((CustomWP.lev > 0) ? 1400 : 1200);

		private readonly int DoSpinSlashPhase = ((CustomWP.lev > 0) ? 800 : 700);

		private bool doingIntro;

		private PlayMakerFSM _pvFsm;

		private GameObject[] traitorSlam;

		private int traitorSlamIndex;

		private Coroutine _counterRoutine;

		private bool _blockedHit;

		private const float DashDelay = 0.18f;

		private const float TurnDelay = 0.05f;

		private float Att1BaseDelay = 0.4f;

		private const float SheoAttDelay = 0.2f;

		private const float WalkSpeed = 10f;

		private const float AerialDelay = 0.25f;

		private const float TwoFancyDelay = 0.25f;

		private bool _countering;

		public static bool WaitForTChild;

		private const float ThrowDelay = 0.2f;

		private const float NailSize = 1.15f;

		private const float NailSpeed = 80f;

		private const float DashSpeed = 60f;

		private readonly Vector3 LeaveOffset = new Vector3(1.5f, 1.5f);

		private readonly int DreamConvoAmount = 3;

		private readonly string DreamConvoKey = ((!OWArenaFinder.IsInOverWorld) ? (((uint)(CustomWP.boss - 6) <= 1u) ? "ZEM_GG_DREAM" : "ZEM_GG_DREAM") : "ZEM_DREAM");

		private static LanguageCtrl _langCtrl;

		private bool _stopForcingHB;

		private bool _hasDied;

		private void Awake()
		{
			OnDestroy();
			_langCtrl = new LanguageCtrl();
			On.HealthManager.TakeDamage += HealthManager_TakeDamage;
			On.HealthManager.Die += HealthManagerOnDie;
			On.EnemyDreamnailReaction.RecieveDreamImpact += OnReceiveDreamImpact;
			On.SpellFluke.DoDamage += SpellFlukeOnDoDamage;
			_hm = base.gameObject.AddComponent<HealthManager>();
			_anim = base.gameObject.GetComponent<Animator>();
			_bc = base.gameObject.GetComponent<BoxCollider2D>();
			_rb = base.gameObject.GetComponent<Rigidbody2D>();
			_rb.isKinematic = true;
			_rb.interpolation = RigidbodyInterpolation2D.Interpolate;
			_sr = GetComponent<SpriteRenderer>();
			_dreamNailReaction = base.gameObject.AddComponent<EnemyDreamnailReaction>();
			base.gameObject.AddComponent<AudioSource>();
			base.gameObject.AddComponent<DamageHero>().damageDealt = 1;
			_dd = FiveKnights.preloadedGO["WD"];
			_dnailEff = _dd.GetComponent<EnemyDreamnailReaction>().GetAttr<EnemyDreamnailReaction, GameObject>("dreamImpactPrefab");
			_extraDamageable = base.gameObject.AddComponent<ExtraDamageable>();
			_dreamNailReaction.enabled = true;
			Mirror.SetField(_dreamNailReaction, "convoAmount", DreamConvoAmount);
			_dreamNailReaction.SetConvoTitle(DreamConvoKey);
			_rand = new System.Random();
			_hitEffects = base.gameObject.AddComponent<EnemyHitEffectsUninfected>();
			_hitEffects.enabled = true;
			base.gameObject.AddComponent<Flash>();
			_pvFsm = FiveKnights.preloadedGO["PV"].LocateMyFSM("Control");
			traitorSlam = new GameObject[2];
			traitorSlamIndex = 0;
			string key = ((CustomWP.boss == CustomWP.Boss.All) ? "ZemParticDung" : "ZemParticPetal");
			FiveKnights.preloadedGO["TraitorSlam"].transform.Find("Grass").GetComponent<ParticleSystemRenderer>().material.mainTexture = ArenaFinder.Sprites[key].texture;
			for (int i = 0; i < traitorSlam.Length; i++)
			{
				traitorSlam[i] = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["TraitorSlam"]);
				UnityEngine.Object.Destroy(traitorSlam[i].GetComponent<AutoRecycleSelf>());
				traitorSlam[i].transform.Find("slash_core").Find("hurtbox").GetComponent<DamageHero>()
					.damageDealt = 1;
				traitorSlam[i].SetActive(value: false);
			}
			AssignFields();
			_hm.hp = ((CustomWP.boss == CustomWP.Boss.Ze) ? MaxHPV1 : MaxHPV2);
			EnemyHPBarImport.RefreshHPBar(base.gameObject);
			doingIntro = false;
			if (!OWArenaFinder.IsInOverWorld)
			{
				_stopForcingHB = false;
				_bc.enabled = false;
				StartCoroutine(ForceDisableHB());
			}
			base.gameObject.transform.localScale *= 0.8f;
			base.gameObject.layer = 11;
		}

		private void HealthManagerOnDie(On.HealthManager.orig_Die orig, HealthManager self, float? attackDirection, AttackTypes attackType, bool ignoreEvasion)
		{
			if (!self.gameObject.name.Contains("Zemer"))
			{
				orig(self, attackDirection, attackType, ignoreEvasion);
			}
		}

		private IEnumerator ForceDisableHB()
		{
			while (!_stopForcingHB)
			{
				_bc.enabled = false;
				yield return new WaitForEndOfFrame();
				_bc.enabled = false;
			}
		}

		private void DoTitle()
		{
			GameObject original = null;
			foreach (GameObject item in from x in UnityEngine.Object.FindObjectsOfType<GameObject>()
				where x.name.Contains("Area Title Holder")
				select x)
			{
				original = item.transform.Find("Area Title").gameObject;
			}
			string smallSuper = "Mysterious";
			string smallMain = "Ze'mer";
			string smallSub = "";
			string sheet = "Reward Room";
			if (_langCtrl.ContainsKey("RR_ZEMER_TITLE_SUPER", sheet))
			{
				smallSuper = _langCtrl.Get("RR_ZEMER_TITLE_SUPER", sheet);
				smallMain = _langCtrl.Get("RR_ZEMER_TITLE_MAIN", sheet);
				smallSub = _langCtrl.Get("RR_ZEMER_TITLE_SUB", sheet);
			}
			original = UnityEngine.Object.Instantiate(original);
			original.SetActive(value: true);
			AreaTitleCtrl.ShowBossTitle(this, original, 2f, "", "", "", smallMain, smallSuper, smallSub);
		}

		private IEnumerator Start()
		{
			yield return new WaitWhile(() => !(_target = HeroController.instance.gameObject));
			UnityEngine.Object.Destroy(GameObject.Find("Bounds Cage"));
			UnityEngine.Object.Destroy(GameObject.Find("World Edge v2"));
			if (!GGBossManager.alone && !OWArenaFinder.IsInOverWorld)
			{
				StartCoroutine(SilLeave());
			}
			else
			{
				yield return new WaitForSeconds(1.7f);
			}
			base.gameObject.SetActive(value: true);
			Transform obj = base.gameObject.transform;
			Vector3 position = (base.gameObject.transform.position = new Vector2(RightX - 10f, GroundY + 0.5f));
			obj.position = position;
			FaceHero();
			BoxCollider2D bc = _bc;
			SpriteRenderer sr = _sr;
			bool flag = false;
			sr.enabled = false;
			bc.enabled = flag;
			yield return new WaitForSeconds(0.2f);
			_anim.Play("ZIntro");
			_sr.enabled = true;
			yield return null;
			yield return new WaitWhile(() => _anim.GetCurrentFrame() < 1);
			base.gameObject.transform.position = new Vector2(RightX - 10f, GroundY);
			yield return new WaitWhile(() => _anim.GetCurrentFrame() < 3);
			_anim.enabled = false;
			yield return new WaitForSeconds(0.3f);
			yield return new WaitWhile(() => WaitForTChild);
			StartCoroutine(MusicControl());
			DoTitle();
			ZemerController zemerController = this;
			ZemerController zemerController2 = this;
			flag = true;
			zemerController2._stopForcingHB = true;
			zemerController.doingIntro = flag;
			_bc.enabled = true;
			_anim.enabled = true;
			yield return new WaitWhile(() => _anim.GetCurrentFrame() < 10);
			PlayAudioClip("ZAudBow");
			ZemerController zemerController3 = this;
			BoxCollider2D bc2 = _bc;
			flag = true;
			bc2.enabled = true;
			zemerController3.doingIntro = flag;
			yield return new WaitWhile(() => _anim.GetCurrentFrame() < 12);
			_anim.enabled = false;
			yield return new WaitForSeconds(0.4f);
			_anim.enabled = true;
			yield return new WaitWhile(() => _anim.IsPlaying());
			_anim.Play("ZWalkRight");
			_rb.velocity = new Vector2(10f, 0f);
			yield return new WaitWhile(() => base.transform.GetPositionX() < RightX - 15f);
			_rb.velocity = Vector2.zero;
			doingIntro = false;
			_bc.enabled = true;
			_anim.Play("ZIdle");
			StartCoroutine(Attacks());
			Log("Done Intro");
		}

		private IEnumerator MusicControl()
		{
			if (OWArenaFinder.IsInOverWorld)
			{
				OWBossManager.PlayMusic(null);
				OWBossManager.PlayMusic(FiveKnights.Clips["ZP1Intro"]);
				yield return new WaitForSecondsRealtime(7.04f);
				OWBossManager.PlayMusic(null);
				OWBossManager.PlayMusic(FiveKnights.Clips["ZP1Loop"]);
			}
			else
			{
				GGBossManager.Instance.PlayMusic(null);
				GGBossManager.Instance.PlayMusic(FiveKnights.Clips["ZP1Intro"]);
				yield return new WaitForSecondsRealtime(7.04f);
				GGBossManager.Instance.PlayMusic(null);
				GGBossManager.Instance.PlayMusic(FiveKnights.Clips["ZP1Loop"]);
			}
		}

		private void Update()
		{
			if (base.transform.GetPositionX() > RightX - 3f && _rb.velocity.x > 0f)
			{
				_rb.velocity = new Vector2(0f, _rb.velocity.y);
			}
			if (base.transform.GetPositionX() < LeftX + 3f && _rb.velocity.x < 0f)
			{
				_rb.velocity = new Vector2(0f, _rb.velocity.y);
			}
		}

		private IEnumerator Attacks()
		{
			int counterCount = 0;
			Dictionary<Func<IEnumerator>, int> rep = new Dictionary<Func<IEnumerator>, int>
			{
				[FancyAttack] = 0,
				[Dash] = 0,
				[Attack1Base] = 0,
				[AerialAttack] = 0,
				[Attack1Complete] = 0,
				[ZemerSlam] = 0,
				[NailLaunch] = 0
			};
			Dictionary<Func<IEnumerator>, int> max = new Dictionary<Func<IEnumerator>, int>
			{
				[FancyAttack] = 2,
				[Dash] = 2,
				[Attack1Base] = 2,
				[AerialAttack] = 2,
				[Attack1Complete] = 2,
				[NailLaunch] = 2,
				[ZemerSlam] = 1
			};
			if (_countering)
			{
				yield return Countered();
			}
			while (true)
			{
				Log("[Waiting to start calculation]");
				float num = ((base.transform.position.x < RightX - 22f) ? 8f : (-8f));
				yield return Walk(num + (float)UnityEngine.Random.Range(-2, 3));
				Log("[Setting Attacks]");
				Vector2 vector = base.transform.position;
				Vector2 vector2 = _target.transform.position;
				if (vector2.y > GroundY + 3f && (vector2.x <= LeftX + 1f || vector2.x >= RightX - 1f))
				{
					Log("Doing Spin Cheese Counter");
					yield return SpinAttack();
					Log("Done Spin Cheese Counter");
				}
				else if (FastApproximately(vector.x, vector2.x, 5f))
				{
					int num2 = _rand.Next(0, 5);
					if (num2 == 0 && counterCount < 2)
					{
						counterCount++;
						Log("Doing Counter");
						ZemerCounter();
						_countering = true;
						yield return new WaitWhile(() => _countering);
						Log("Done Counter");
					}
					else if (num2 < 2)
					{
						counterCount = 0;
						List<Func<IEnumerator>> attLst = new List<Func<IEnumerator>> { FancyAttack, NailLaunch, Dash, null };
						Log("Choosing Attack");
						Func<IEnumerator> att = MiscMethods.ChooseAttack(attLst, rep, max);
						Log("Done Choosing Attack");
						if (att == new Func<IEnumerator>(FancyAttack))
						{
							rep[AerialAttack] = 2;
						}
						if (att != new Func<IEnumerator>(Dash))
						{
							Log("Doing Special Dodge");
							yield return Dodge();
							rep[Dash] = 2;
							Log("Done Special Dodge's Dodge");
						}
						if (att != null)
						{
							Log("Doing " + att.Method.Name);
							yield return att();
							Log("Done " + att.Method.Name);
						}
						Log("Done Special Dodge");
					}
				}
				List<Func<IEnumerator>> attLst2 = new List<Func<IEnumerator>> { Dash, Attack1Base, Attack1Base, AerialAttack, ZemerSlam, NailLaunch };
				Log("Choosing Attack");
				Func<IEnumerator> currAtt = MiscMethods.ChooseAttack(attLst2, rep, max);
				Log("Done Choosing Attack");
				Log("Doing " + currAtt.Method.Name);
				yield return currAtt();
				Log("Done " + currAtt.Method.Name);
				if ((currAtt == new Func<IEnumerator>(ZemerSlam) || currAtt == new Func<IEnumerator>(Dash)) && rep[NailLaunch] < max[NailLaunch] && UnityEngine.Random.Range(0, 2) == 0)
				{
					rep[NailLaunch]++;
					Log("Doing NailLaunch");
					yield return NailLaunch();
					Log("Done NailLaunch");
				}
				else if (currAtt == new Func<IEnumerator>(Attack1Base))
				{
					List<Func<IEnumerator>> attLst3 = new List<Func<IEnumerator>> { FancyAttack, FancyAttack, Attack1Complete };
					if (FastApproximately(base.transform.position.x, _target.transform.position.x, 7f))
					{
						attLst3 = new List<Func<IEnumerator>> { Attack1Complete };
					}
					Log("Choosing Attack");
					currAtt = MiscMethods.ChooseAttack(attLst3, rep, max);
					Log("Done Choosing Attack");
					if (currAtt != null)
					{
						Log("Doing " + currAtt.Method.Name);
						yield return currAtt();
						Log("Done " + currAtt.Method.Name);
					}
					if (currAtt == new Func<IEnumerator>(FancyAttack))
					{
						switch (_rand.Next(0, 4))
						{
						case 0:
							Log("Doing Special Fancy Attack");
							yield return Dodge();
							yield return new WaitForSeconds(0.25f);
							yield return FancyAttack();
							rep[AerialAttack] = 2;
							Log("Done Special Fancy Attack");
							break;
						case 1:
							Log("Doing Slam");
							yield return ZemerSlam();
							Log("Done Slam");
							break;
						case 2:
							Log("Doing NailLaunch");
							yield return NailLaunch();
							Log("Done NailLaunch");
							break;
						default:
							Log("Doing Dash");
							yield return Dash();
							Log("Done Dash");
							break;
						}
					}
				}
				Log("[Done Doing Attacks]");
				_anim.Play("ZIdle");
				Log("[Restarting Calculations]");
				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator AerialAttack()
		{
			yield return Attack();
			IEnumerator Attack()
			{
				if (!IsFacingPlayer())
				{
					yield return Turn();
				}
				float xVel = FaceHero() * -1f;
				base.transform.Find("BladeAerialShadow").gameObject.SetActive(value: false);
				_anim.Play("ZAerial2");
				PlayAudioClip(ZemRandAudio.PickRandomZemAud(1, 8));
				yield return null;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 4);
				_anim.enabled = false;
				yield return new WaitForSeconds(0.25f);
				_anim.enabled = true;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 8);
				_rb.velocity = new Vector2(xVel * 38f, 15f);
				_rb.gravityScale = 1.3f;
				_rb.isKinematic = false;
				yield return new WaitForSeconds(0.1f);
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 10);
				PlayAudioClip("AudBigSlash2", 0.15f);
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 13);
				PlayAudioClip("AudBigSlash2", 0.15f);
				yield return new WaitWhile(() => base.transform.position.y > GroundY);
				_rb.velocity = Vector2.zero;
				_rb.gravityScale = 0f;
				_rb.isKinematic = true;
				base.transform.position = new Vector2(base.transform.position.x, GroundY);
				yield return new WaitWhile(() => _anim.IsPlaying());
			}
		}

		private IEnumerator Walk(float displacement)
		{
			bool isEnd = false;
			Coroutine walk = StartCoroutine(Walk());
			StartCoroutine(DodgeCheck(walk));
			yield return new WaitWhile(() => !isEnd);
			IEnumerator DodgeCheck(Coroutine routine)
			{
				int thresh = 25;
				int num = 0;
				while (!isEnd)
				{
					float x = base.transform.position.x;
					Vector3 position = _target.transform.position;
					if (x.Within(position.x, 1f) && position.y > GroundY + 3.5f)
					{
						if (num > thresh)
						{
							if (UnityEngine.Random.Range(0, 3) != 0)
							{
								Log("Stopping walk to dodge.");
								StopCoroutine(routine);
								_anim.speed = 1f;
								_rb.velocity = Vector2.zero;
								yield return UnityEngine.Random.Range(0, 2) switch
								{
									1 => Dodge(), 
									0 => Upslash(), 
									_ => Dash(), 
								};
								isEnd = true;
								break;
							}
							num = 0;
							continue;
						}
						num++;
					}
					yield return null;
				}
			}
			IEnumerator Walk()
			{
				float xPos = base.transform.position.x;
				float x = _target.transform.position.x;
				int signX = (int)Mathf.Sign(displacement);
				string text = (((int)Mathf.Sign(x - xPos) != signX) ? "ZWalkRight" : "ZWalkLeft");
				Vector3 localScale = base.gameObject.transform.localScale;
				float num = Mathf.Abs(localScale.x) * (float)signX;
				num = ((text == "ZWalkRight") ? num : (0f - num));
				base.gameObject.transform.localScale = new Vector3(num, localScale.y, 1f);
				_anim.speed = 1.38f;
				_anim.Play(text, -1, 0f);
				yield return null;
				_rb.velocity = new Vector2((float)signX * 10f, 0f);
				yield return new WaitWhile(delegate
				{
					bool num2 = signX > 0;
					float positionX = base.transform.GetPositionX();
					bool flag = (num2 ? (xPos + displacement > positionX) : (xPos + displacement < positionX));
					bool flag2 = _target.transform.position.x.Within(positionX, 3f);
					return (!_rb.velocity.x.Within(0f, 0.05f) & flag) && !flag2;
				});
				_anim.speed = 1f;
				_rb.velocity = Vector2.zero;
				isEnd = true;
				_anim.Play("ZIdle");
			}
		}

		private IEnumerator ZemerSlam()
		{
			yield return Slam();
			IEnumerator Slam()
			{
				base.transform.position += new Vector3(0f, 1.32f);
				PlayAudioClip(ZemRandAudio.PickRandomZemAud(9, 13));
				yield return _anim.PlayToFrame("ZSlamNew", 7);
				SpawnShockwaves(2f, 50f, 1, base.transform.position);
				GameCameras.instance.cameraShakeFSM.SendEvent("BigShake");
				PlayAudioClip("AudLand");
				yield return _anim.PlayToEnd();
				base.transform.position -= new Vector3(0f, 1.32f);
			}
		}

		private void SpawnShockwaves(float vertScale, float speed, int damage, Vector2 pos)
		{
			bool[] obj = new bool[2] { false, true };
			PlayMakerFSM fsm = FiveKnights.preloadedGO["Mage"].LocateMyFSM("Mage Lord");
			bool[] array = obj;
			foreach (bool flag in array)
			{
				GameObject obj2 = UnityEngine.Object.Instantiate(fsm.GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>("Quake Waves", 0).gameObject.Value);
				PlayMakerFSM playMakerFSM = obj2.LocateMyFSM("shockwave");
				playMakerFSM.FsmVariables.FindFsmBool("Facing Right").Value = flag;
				playMakerFSM.FsmVariables.FindFsmFloat("Speed").Value = speed;
				obj2.AddComponent<DamageHero>().damageDealt = damage;
				obj2.SetActive(value: true);
				obj2.transform.SetPosition2D(new Vector2(pos.x + (flag ? 0.5f : (-0.5f)), SlamY));
				obj2.transform.SetScaleX(vertScale);
			}
		}

		private IEnumerator Turn()
		{
			_anim.Play("ZTurn", -1, 0f);
			yield return new WaitForSeconds(0.05f);
		}

		private void ZemerCounter()
		{
			float dir = 0f;
			_counterRoutine = StartCoroutine(CounterAntic());
			IEnumerator CounterAntic()
			{
				if (!IsFacingPlayer())
				{
					yield return Turn();
				}
				dir = FaceHero() * -1f;
				_anim.Play("ZCInit");
				PlayAudioClip("ZAudCounter");
				yield return new WaitWhile(() => _anim.IsPlaying());
				_counterRoutine = StartCoroutine(Countering());
			}
			IEnumerator CounterEnd()
			{
				_hm.IsInvincible = false;
				On.HealthManager.Hit -= OnBlockedHit;
				_anim.Play("ZCCancel");
				yield return new WaitWhile(() => _anim.IsPlaying());
				_countering = false;
			}
			IEnumerator Countering()
			{
				_hm.IsInvincible = true;
				_anim.Play("ZCIdle");
				_blockedHit = false;
				On.HealthManager.Hit += OnBlockedHit;
				PlayAudioClip("Counter");
				Vector2 vector = base.transform.position + new Vector3(1.7f * dir, 0.8f, -0.1f);
				Quaternion rotation = Quaternion.Euler(0f, 0f, dir * 80f);
				UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["CounterFX"], vector, rotation).SetActive(value: true);
				yield return new WaitForSeconds(0.42f);
				_counterRoutine = StartCoroutine(CounterEnd());
			}
		}

		private IEnumerator FancyAttack()
		{
			yield return Attack();
			IEnumerator Attack()
			{
				if (!IsFacingPlayer())
				{
					yield return Turn();
				}
				FaceHero();
				_anim.Play("ZAtt2");
				PlayAudioClip(ZemRandAudio.PickRandomZemAud(9, 13));
				yield return null;
				PlayAudioClip("AudBasicSlash1");
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 2);
				PlayAudioClip("AudBasicSlash2");
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 6);
				PlayAudioClip("AudBigSlash2");
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 7);
				float dir = FaceHero();
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 8);
				SpawnPillar(dir);
				yield return new WaitWhile(() => _anim.IsPlaying());
				_anim.Play("ZIdle");
			}
			void SpawnPillar(float dir)
			{
				GameObject obj = traitorSlam[traitorSlamIndex++ % 2];
				Animator component = obj.transform.Find("slash_core").GetComponent<Animator>();
				obj.SetActive(value: true);
				component.enabled = true;
				component.Play("mega_mantis_slash_big", -1, 0f);
				PlayAudioClip("TraitorPillar");
				obj.GetComponent<Rigidbody2D>().velocity = new Vector2((0f - dir) * 15f, 0f);
				Vector3 position = base.transform.position;
				obj.transform.position = new Vector3((0f - dir) * 4.4f + position.x, GroundY - 3.2f, 6.4f);
				obj.transform.localScale = new Vector3(0f - dir, 1f, 1f);
			}
		}

		private IEnumerator Countered()
		{
			_hm.IsInvincible = false;
			On.HealthManager.Hit -= OnBlockedHit;
			yield return _anim.PlayToEndWithActions("ZCAtt", (3, delegate
			{
				PlayAudioClip("Slash", 0.15f);
			}));
			_anim.Play("ZIdle");
			yield return new WaitForSeconds(0.25f);
			_countering = false;
		}

		private IEnumerator Attack1Base()
		{
			yield return (_hm.hp < DoSpinSlashPhase) ? Attack1Full() : Attack1Reg();
			IEnumerator Attack1Full()
			{
				if (!IsFacingPlayer())
				{
					yield return Turn();
				}
				float xVel = FaceHero() * -1f;
				PlayAudioClip(ZemRandAudio.PickRandomZemAud(2, 8));
				yield return _anim.PlayToFrame("ZAtt1Intro", 1);
				_anim.enabled = false;
				yield return new WaitForSeconds(Att1BaseDelay);
				_anim.enabled = true;
				yield return _anim.WaitToFrame(2);
				PlayAudioClip("Slash", 0.15f);
				yield return _anim.WaitToFrame(6);
				yield return _anim.PlayToEnd();
				_rb.velocity = new Vector2(23f * xVel, 0f);
				_anim.speed = 1.5f;
				while ((xVel > 0f && base.transform.position.x < RightX - 10f) || (xVel < 0f && base.transform.position.x > LeftX + 10f))
				{
					yield return _anim.PlayToEndWithActions("ZAtt1Loop", (0, delegate
					{
						PlayAudioClip("Slash", 0.15f);
					}));
				}
				_anim.speed = 1f;
				_anim.Play("ZAtt1End");
				_rb.velocity = Vector2.zero;
				yield return _anim.PlayToEnd();
			}
			IEnumerator Attack1Reg()
			{
				if (!IsFacingPlayer())
				{
					yield return Turn();
				}
				float xVel = FaceHero() * -1f;
				_anim.Play("ZAtt1Base");
				PlayAudioClip(ZemRandAudio.PickRandomZemAud(2, 8));
				yield return null;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 1);
				_anim.enabled = false;
				yield return new WaitForSeconds(Att1BaseDelay);
				_anim.enabled = true;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 2);
				PlayAudioClip("Slash", 0.15f);
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 7);
				PlayAudioClip("Slash", 0.15f);
				_rb.velocity = new Vector2(23f * xVel, 0f);
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 11);
				_rb.velocity = Vector2.zero;
				yield return new WaitWhile(() => _anim.IsPlaying());
			}
		}

		private IEnumerator Upslash()
		{
			if (!IsFacingPlayer())
			{
				yield return Turn();
			}
			FaceHero();
			base.transform.Find("HyperCut").gameObject.SetActive(value: false);
			PlayAudioClip(ZemRandAudio.PickRandomZemAud(9, 13));
			_anim.Play("ZDashOld");
			base.transform.position = new Vector3(base.transform.position.x, GroundY - 0.3f, base.transform.position.z);
			yield return _anim.WaitToFrame(4);
			if (!IsFacingPlayer())
			{
				yield return Turn();
			}
			FaceHero();
			_anim.Play("DashCounter");
			PlayAudioClip("Slash", 0.15f);
			yield return null;
			yield return new WaitWhile(() => _anim.IsPlaying());
			_anim.Play("ZIdle");
			base.transform.position = new Vector3(base.transform.position.x, GroundY);
		}

		private IEnumerator Dash()
		{
			yield return Dash();
			IEnumerator Dash()
			{
				if (!IsFacingPlayer())
				{
					yield return Turn();
				}
				float dir = FaceHero();
				base.transform.Find("HyperCut").gameObject.SetActive(value: false);
				_anim.Play("ZDash", -1, 0f);
				base.transform.position = new Vector3(base.transform.position.x, GroundY + 0.3f, base.transform.position.z);
				int subOneFrameOnEdges = 0;
				if (base.transform.position.x < LeftX + 5f || base.transform.position.x > RightX - 5f)
				{
					subOneFrameOnEdges = 1;
					_anim.speed = 1.1f;
					_anim.Play("ZDash2", -1, 0f);
					_anim.speed = 1f;
				}
				else
				{
					yield return _anim.WaitToFrame(1);
					_anim.speed = 1.1f;
					yield return _anim.WaitToFrame(2);
					PlayAudioClip("Zem_Backdash");
					_rb.velocity = new Vector2(dir * 35f, 0f);
					yield return _anim.WaitToFrame(4);
					_anim.speed = 1f;
					_rb.velocity = Vector2.zero;
				}
				yield return _anim.WaitToFrame(7 - subOneFrameOnEdges);
				PlayAudioClip("ZemBladeShine");
				yield return _anim.WaitToFrame(11 - subOneFrameOnEdges);
				PlayAudioClip("AudDashIntro");
				yield return _anim.WaitToFrame(13 - subOneFrameOnEdges);
				PlayAudioClip("ZAudHoriz");
				PlayAudioClip("AudDash");
				_anim.speed = 1.5f;
				_rb.velocity = new Vector2((0f - dir) * 60f, 0f);
				yield return _anim.WaitToFrame(14 - subOneFrameOnEdges);
				base.transform.position = new Vector3(base.transform.position.x, GroundY - 0.5f, base.transform.position.z);
				_anim.enabled = false;
				_anim.speed = 1f;
				yield return new WaitForSeconds(0.15f);
				_anim.enabled = true;
				yield return _anim.WaitToFrame(16 - subOneFrameOnEdges);
				_rb.velocity = Vector2.zero;
				yield return new WaitWhile(() => _anim.IsPlaying());
				_anim.Play("ZIdle");
				base.transform.position = new Vector3(base.transform.position.x, GroundY, base.transform.position.z);
			}
		}

		private IEnumerator Dodge()
		{
			yield return Dodge();
			IEnumerator Dodge()
			{
				if (!IsFacingPlayer())
				{
					yield return Turn();
				}
				float num = FaceHero() * -1f;
				_anim.Play("ZDodge", -1, 0f);
				_rb.velocity = new Vector2((0f - num) * 40f, 0f);
				yield return null;
				yield return new WaitWhile(() => _anim.IsPlaying());
				_rb.velocity = new Vector2(0f, 0f);
				_anim.Play("ZIdle");
			}
		}

		private IEnumerator Attack1Complete()
		{
			yield return Attack1Complete();
			IEnumerator Attack1Complete()
			{
				_anim.Play("ZAtt1");
				float xVel = FaceHero() * -1f;
				yield return null;
				_anim.enabled = false;
				yield return new WaitForSeconds(0.2f);
				_anim.enabled = true;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 3);
				PlayAudioClip("AudBigSlash", 0.15f);
				_rb.velocity = new Vector2(40f * xVel, 0f);
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 4);
				_rb.velocity = Vector2.zero;
				yield return new WaitWhile(() => _anim.IsPlaying());
				_anim.Play("ZIdle");
			}
		}

		private IEnumerator SpinAttack()
		{
			yield return SpinAttack();
			IEnumerator SpinAttack()
			{
				if (!IsFacingPlayer())
				{
					yield return Turn();
				}
				float num = FaceHero() * -1f;
				float num2 = Mathf.Abs(_target.transform.GetPositionX() - base.transform.GetPositionX());
				float num3 = Mathf.Abs(_target.transform.GetPositionY() - base.transform.GetPositionY());
				float rot = Mathf.Atan(num3 / num2);
				rot = ((num < 0f) ? ((float)Math.PI - rot) : rot);
				_anim.Play("ZSpin");
				PlayAudioClip(ZemRandAudio.PickRandomZemAud(2, 8));
				yield return null;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 2);
				PlayAudioClip("AudDashIntro");
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 3);
				_rb.velocity = new Vector2(60f * Mathf.Cos(rot), 60f * Mathf.Sin(rot));
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 4);
				_anim.enabled = false;
				yield return new WaitWhile(() => base.transform.position.x > LeftX + 4f && base.transform.position.x < RightX - 4f);
				_anim.enabled = true;
				_rb.velocity = Vector2.zero;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 8);
				PlayAudioClip("AudBigSlash2");
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 11);
				_rb.isKinematic = false;
				_rb.gravityScale = 1.5f;
				yield return new WaitWhile(() => base.transform.position.y > GroundY);
				PlayAudioClip("AudLand");
				_rb.velocity = Vector2.zero;
				_rb.gravityScale = 0f;
				_rb.isKinematic = true;
				base.transform.position = new Vector3(base.transform.position.x, GroundY);
				yield return new WaitWhile(() => _anim.IsPlaying());
				_anim.Play("ZIdle");
			}
		}

		private IEnumerator NailLaunch()
		{
			float dir = 0f;
			float middle = (RightX + LeftX) / 2f;
			yield return Throw();
			IEnumerator GndLeave(float num)
			{
				_anim.speed = 2f;
				_anim.Play("ZThrow2");
				yield return null;
				yield return _anim.WaitToFrame(2);
				Spring(isIn: false, base.transform.position + new Vector3((0f - num) * LeaveOffset.x, 0f, 0f), 1.5f);
				yield return _anim.WaitToFrame(4);
				base.transform.position += new Vector3((0f - num) * LeaveOffset.x, 0f);
				yield return _anim.PlayToEnd();
				ToggleZemer(visible: false);
			}
			IEnumerator LaunchSide(GameObject nail)
			{
				GameObject gameObject = nail.transform.Find("ZNailC").gameObject;
				Rigidbody2D rbNail = nail.GetComponent<Rigidbody2D>();
				CollisionCheck cc = gameObject.GetComponent<CollisionCheck>();
				Log("Doing launch side");
				if (cc == null)
				{
					cc = gameObject.AddComponent<CollisionCheck>();
					cc.OnCollide += delegate
					{
						PlayAudioClip("AudLand");
						GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
						nail.GetComponent<SpriteRenderer>().enabled = false;
						nail.transform.Find("ZNailN").GetComponent<SpriteRenderer>().enabled = true;
						rbNail.velocity = Vector2.zero;
						UnityEngine.Object.Destroy(cc);
					};
				}
				cc.Hit = rbNail.velocity == Vector2.zero;
				yield return GndLeave(dir);
				yield return new WaitWhile(() => !cc.Hit && rbNail.velocity != Vector2.zero);
				yield return new WaitForSeconds(0.75f);
				Vector2 vector = base.transform.position;
				Vector2 vector2 = nail.transform.Find("Point").position;
				Vector3 localScale = base.transform.localScale;
				if (vector2.x < middle)
				{
					base.transform.localScale = new Vector3(Mathf.Abs(localScale.x), localScale.y, localScale.z);
					base.transform.position = new Vector2(vector2.x + LeaveOffset.x, vector.y);
					Spring(isIn: true, base.transform.position);
					yield return new WaitForSeconds(0.15f);
					ToggleZemer(visible: true);
					_anim.speed = 2f;
					_anim.Play("ZThrow3Gnd");
					yield return null;
					yield return _anim.WaitToFrame(1);
					base.transform.position -= new Vector3(LeaveOffset.x, 0f);
					yield return _anim.WaitToFrame(2);
					_anim.speed = 1f;
					Log(string.Format("Nail ended at {0}", nail.transform.Find("Center").position));
					UnityEngine.Object.Destroy(nail);
				}
				else
				{
					base.transform.localScale = new Vector3(0f - Mathf.Abs(localScale.x), localScale.y, localScale.z);
					base.transform.position = new Vector2(vector2.x - LeaveOffset.x, vector.y);
					Spring(isIn: true, base.transform.position);
					yield return new WaitForSeconds(0.15f);
					ToggleZemer(visible: true);
					_anim.speed = 2f;
					_anim.Play("ZThrow3Gnd");
					yield return null;
					yield return _anim.WaitToFrame(1);
					base.transform.position += new Vector3(LeaveOffset.x, 0f);
					yield return _anim.WaitToFrame(2);
					_anim.speed = 1f;
					Log(string.Format("Nail ended at {0}", nail.transform.Find("Center").position));
					UnityEngine.Object.Destroy(nail);
				}
				yield return _anim.PlayToEnd();
			}
			IEnumerator Throw()
			{
				if (((Vector2)_target.transform.position).x.Within(base.transform.position.x, 12f))
				{
					if (base.transform.position.x < LeftX + 6f || base.transform.position.x > RightX - 6f)
					{
						Log("Dash for nail launch");
						yield return Dash();
						Log("Done Dash for nail launch");
						if (_target.transform.position.x.Within(base.transform.position.x, 12f))
						{
							Log("Dodge for nail launch");
							yield return Dodge();
							Log("Done Dodge for nail launch");
						}
					}
					else
					{
						Log("Just dodge for nail launch");
						yield return Dodge();
						Log("Done Just dodge for nail launch");
					}
				}
				dir = FaceHero();
				_anim.Play("ZThrow1");
				PlayAudioClip(ZemRandAudio.PickRandomZemAud(2, 8));
				yield return _anim.WaitToFrame(2);
				_anim.enabled = false;
				yield return new WaitForSeconds(0.1f);
				Vector2 hero = _target.transform.position;
				dir = FaceHero();
				yield return FlashRepeat(hero, 0.1f);
				_anim.enabled = true;
				yield return _anim.WaitToFrame(3);
				Transform transform = base.transform.Find("ZNailB").Find("Center");
				float rot = GetAngleTo(transform.position, hero) * ((float)Math.PI / 180f);
				string text = LayerMask.LayerToName(8);
				RaycastHit2D raycastHit2D = Physics2D.BoxCast(transform.position, new Vector2(1f, 1f), 0f, new Vector2(Mathf.Cos(rot), Mathf.Sin(rot)), float.PositiveInfinity, LayerMask.GetMask(text));
				while (raycastHit2D.point.y > GroundY + 3.2f)
				{
					hero -= new Vector2(0f, 1f);
					rot = GetAngleTo(transform.position, hero) * ((float)Math.PI / 180f);
					raycastHit2D = Physics2D.BoxCast(transform.position, new Vector2(1f, 1f), 0f, new Vector2(Mathf.Cos(rot), Mathf.Sin(rot)), float.PositiveInfinity, LayerMask.GetMask(text));
				}
				float num = (0f - Mathf.Sign(Mathf.Cos(rot))) * 6f;
				for (int i = 0; i < 10; i++)
				{
					if (!transform.position.x.Within(raycastHit2D.point.x + num, 10f))
					{
						break;
					}
					hero += new Vector2(0f - dir, 0f);
					rot = GetAngleTo(transform.position, hero) * ((float)Math.PI / 180f);
					raycastHit2D = Physics2D.BoxCast(transform.position, new Vector2(1f, 1f), 0f, new Vector2(Mathf.Cos(rot), Mathf.Sin(rot)), float.PositiveInfinity, LayerMask.GetMask(text));
				}
				Log($"Putting nail to {raycastHit2D.point.x + num}");
				float num2 = rot + ((dir > 0f) ? ((float)Math.PI) : 0f);
				GameObject obj = base.transform.Find("NailHand").gameObject;
				GameObject nailPar = UnityEngine.Object.Instantiate(base.transform.Find("ZNailB").gameObject);
				Rigidbody2D parRB = nailPar.GetComponent<Rigidbody2D>();
				if (CustomWP.boss == CustomWP.Boss.Mystic || CustomWP.boss == CustomWP.Boss.Ze)
				{
					Log("double size for reg");
					nailPar.transform.Find("ZNailC").GetComponent<BoxCollider2D>().size *= 2.5f;
				}
				else if (CustomWP.boss == CustomWP.Boss.All)
				{
					Log("incr size for pantheon");
					nailPar.transform.Find("ZNailC").GetComponent<BoxCollider2D>().size *= 1.5f;
				}
				nailPar.transform.position = base.transform.Find("ZNailB").position;
				nailPar.transform.localScale = new Vector3(Mathf.Sign(base.transform.localScale.x) * 1.15f, 1.15f, 1.15f);
				obj.transform.SetRotation2D(num2 * 57.29578f);
				nailPar.transform.SetRotation2D(num2 * 57.29578f);
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 4);
				yield return new WaitForSeconds(0.07f);
				nailPar.SetActive(value: true);
				parRB.velocity = new Vector2(Mathf.Cos(rot) * 80f, Mathf.Sin(rot) * 80f);
				nailPar.AddComponent<ExtraNailBndCheck>();
				CollisionCheck cc = nailPar.transform.Find("ZNailC").gameObject.AddComponent<CollisionCheck>();
				cc.Freeze = true;
				cc.OnCollide += delegate
				{
					PlayAudioClip("AudLand");
					GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
					nailPar.GetComponent<SpriteRenderer>().enabled = false;
					nailPar.transform.Find("ZNailN").GetComponent<SpriteRenderer>().enabled = true;
					nailPar.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
					UnityEngine.Object.Destroy(cc);
				};
				yield return new WaitWhile(() => _anim.IsPlaying());
				yield return LaunchSide(nailPar);
			}
		}

		private void ToggleZemer(bool visible, bool fade = false)
		{
			_bc.enabled = false;
			_anim.enabled = false;
			_anim.speed = 1f;
			if (fade)
			{
				StartCoroutine(Fade());
			}
			else
			{
				Instant();
			}
			IEnumerator Fade()
			{
				Color col = _sr.color;
				_sr.enabled = true;
				if (visible)
				{
					_sr.color = new Color(col.r, col.g, col.b, 0f);
					for (float i = 0f; i <= 1f; i += 0.2f)
					{
						_sr.color = new Color(col.r, col.g, col.b, i);
						yield return new WaitForSeconds(0.01f);
					}
				}
				else
				{
					_sr.color = new Color(col.r, col.g, col.b, 1f);
					for (float i = col.a; i >= 0f; i -= 0.2f)
					{
						_sr.color = new Color(col.r, col.g, col.b, i);
						yield return new WaitForSeconds(0.01f);
					}
				}
				Instant();
			}
			void Instant()
			{
				_sr.enabled = visible;
				_anim.enabled = visible;
				_rb.gravityScale = 0f;
				_bc.enabled = visible;
				Color color = _sr.color;
				_sr.color = new Color(color.r, color.g, color.b, visible ? 1f : 0f);
			}
		}

		private float GetAngleTo(Vector2 from, Vector2 to)
		{
			float num;
			for (num = Mathf.Atan2(to.y - from.y, to.x - from.x) * 57.29578f; (double)num < 0.0; num += 360f)
			{
			}
			return num;
		}

		private void Spring(bool isIn, Vector2 pos, float speedSca = 1f, bool parent = false)
		{
			string key = "VapeIn2";
			GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO[key]);
			PlayMakerFSM fsm = gameObject.LocateMyFSM("FSM");
			gameObject.GetComponent<tk2dSpriteAnimator>().GetClipByName("Plink").fps = 24f * speedSca;
			gameObject.transform.localScale *= 1.7f;
			fsm.GetAction<HutongGames.PlayMaker.Actions.Wait>("State 1", 0).time = 0f;
			gameObject.transform.position = pos;
			gameObject.SetActive(value: true);
			if (parent)
			{
				gameObject.transform.parent = base.transform;
			}
		}

		private IEnumerator FlashRepeat(Vector3 targ, float timer = 0.2f)
		{
			GameObject fxOrig = FiveKnights.preloadedGO["HornetSphere"].transform.Find("Flash Effect").gameObject;
			for (int i = 0; i < 5; i++)
			{
				int[] array = new int[1] { 1 };
				foreach (int num in array)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(fxOrig);
					float num2 = (i * 90 + UnityEngine.Random.Range(30, 60)) * num;
					gameObject.transform.SetRotationZ(num2);
					gameObject.transform.position = targ;
					gameObject.SetActive(value: true);
					PlayMakerFSM playMakerFSM = gameObject.LocateMyFSM("FSM");
					playMakerFSM.enabled = true;
					playMakerFSM.FsmVariables.FindFsmFloat("Pause").Value = 1f;
					playMakerFSM.FsmVariables.FindFsmFloat("Rotation").Value = num2;
					playMakerFSM.FsmVariables.FindFsmBool("Reset Rotation").Value = false;
					playMakerFSM.FsmVariables.FindFsmVector3("Init Pos").Value = targ;
					playMakerFSM.SetState("Init");
					playMakerFSM.transform.localScale *= 0.65f;
					PlayAudioClip((i % 2 == 0) ? "AudBasicSlash1" : "AudBasicSlash2", 0.5f, 1f, gameObject.transform);
				}
				yield return new WaitForSeconds(timer);
				timer /= 1.5f;
			}
		}

		private void OnBlockedHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if (self.name.Contains("Zemer") && !_blockedHit)
			{
				GameManager.instance.StartCoroutine(GameManager.instance.FreezeMoment(0.01f, 0.35f, 0.1f, 0f));
				_blockedHit = true;
				Log("Blocked Hit");
				StopCoroutine(_counterRoutine);
				StartCoroutine(Countered());
			}
			orig(self, hitInstance);
		}

		private void OnReceiveDreamImpact(On.EnemyDreamnailReaction.orig_RecieveDreamImpact orig, EnemyDreamnailReaction self)
		{
			orig(self);
			if (self.name.Contains("Zemer"))
			{
				StartCoroutine(FlashWhite());
				UnityEngine.Object.Instantiate(_dnailEff, base.transform.position, Quaternion.identity);
			}
		}

		private void HealthManager_TakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
		{
			DoTakeDamage(self.gameObject, hitInstance.Direction);
			orig(self, hitInstance);
		}

		private void SpellFlukeOnDoDamage(On.SpellFluke.orig_DoDamage orig, SpellFluke self, GameObject tar, int upwardrecursionamount, bool burst)
		{
			DoTakeDamage(tar, 0f);
			orig(self, tar, upwardrecursionamount, burst);
		}

		private void DoTakeDamage(GameObject tar, float dir)
		{
			if (!tar.name.Contains("Zemer"))
			{
				return;
			}
			_hitEffects.RecieveHitEffect(dir);
			if (doingIntro)
			{
				StopCoroutine("Start");
				_rb.velocity = new Vector2(0f, 0f);
				doingIntro = false;
				_stopForcingHB = true;
				_bc.enabled = true;
				_anim.enabled = true;
				_anim.speed = 1f;
				_countering = true;
				FaceHero();
				StartCoroutine(Attacks());
			}
			if (_hm.hp <= 0 && !_hasDied)
			{
				Log("Going to phase 2");
				_hasDied = true;
				_bc.enabled = false;
				if (OWArenaFinder.IsInOverWorld)
				{
					GameManager.instance.AwardAchievement("PALE_COURT_ZEM_ACH");
				}
				if (OWArenaFinder.IsInOverWorld)
				{
					OWBossManager.PlayMusic(null);
				}
				else
				{
					GGBossManager.Instance.PlayMusic(null, 1f);
				}
				GameObject gameObject = GameObject.Find("ZNailB");
				if (gameObject != null && gameObject.transform.parent == null)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
				StopCoroutine("Attacks");
				OnDestroy();
				base.gameObject.AddComponent<ZemerControllerP2>().DoPhase = CustomWP.boss == CustomWP.Boss.Ze;
				UnityEngine.Object.Destroy(this);
			}
		}

		private IEnumerator SilLeave()
		{
			SpriteRenderer sil = GameObject.Find("Silhouette Zemer").GetComponent<SpriteRenderer>();
			sil.transform.localScale *= 1.2f;
			sil.sprite = ArenaFinder.Sprites["Zem_Sil_1"];
			yield return new WaitForSeconds(0.05f);
			sil.sprite = ArenaFinder.Sprites["Zem_Sil_2"];
			yield return new WaitForSeconds(0.05f);
			sil.sprite = ArenaFinder.Sprites["Zem_Sil_3"];
			yield return new WaitForSeconds(0.05f);
			UnityEngine.Object.Destroy(sil.gameObject);
		}

		private float FaceHero(bool shouldRev = false)
		{
			float num = Mathf.Sign(base.gameObject.transform.GetPositionX() - _target.transform.GetPositionX());
			num = (shouldRev ? (-1f * num) : num);
			Vector3 localScale = base.gameObject.transform.localScale;
			base.gameObject.transform.localScale = new Vector3(Mathf.Abs(localScale.x) * num, localScale.y, 1f);
			return num;
		}

		private bool IsFacingPlayer()
		{
			int num = (int)Mathf.Sign(base.transform.localScale.x);
			int num2 = (int)Mathf.Sign(base.transform.position.x - _target.transform.position.x);
			return num == num2;
		}

		private IEnumerator FlashWhite()
		{
			_sr.material.SetFloat("_FlashAmount", 1f);
			yield return null;
			for (float i = 1f; i >= 0f; i -= 0.05f)
			{
				_sr.material.SetFloat("_FlashAmount", i);
				yield return new WaitForSeconds(0.02f);
			}
			yield return null;
		}

		private void AssignFields()
		{
			HealthManager component = _dd.GetComponent<HealthManager>();
			foreach (FieldInfo item in from x in typeof(HealthManager).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
				where x.Name.Contains("Prefab")
				select x)
			{
				item.SetValue(_hm, item.GetValue(component));
			}
			EnemyHitEffectsUninfected component2 = _dd.GetComponent<EnemyHitEffectsUninfected>();
			FieldInfo[] fields = typeof(EnemyHitEffectsUninfected).GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.Name.Contains("Origin"))
				{
					_hitEffects.effectOrigin = new Vector3(0f, 0.5f, 0f);
				}
				else
				{
					fieldInfo.SetValue(_hitEffects, fieldInfo.GetValue(component2));
				}
			}
			Mirror.SetField(_extraDamageable, "impactClipTable", Mirror.GetField<ExtraDamageable, RandomAudioClipTable>(_dd.GetComponent<ExtraDamageable>(), "impactClipTable"));
			Mirror.SetField(_extraDamageable, "audioPlayerPrefab", Mirror.GetField<ExtraDamageable, AudioSource>(_dd.GetComponent<ExtraDamageable>(), "audioPlayerPrefab"));
		}

		public void PlayAudioClip(string clipName, float pitchVar = 0f, float volume = 1f, Transform posOverride = null)
		{
			this.PlayAudio(clipName switch
			{
				"Counter" => (AudioClip)_pvFsm.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Counter Stance", 1).audioClip.Value, 
				"Slash" => (AudioClip)_pvFsm.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Slash1", 1).audioClip.Value, 
				"TraitorPillar" => FiveKnights.Clips["TraitorSlam"], 
				_ => FiveKnights.Clips[clipName], 
			}, volume, pitchVar, posOverride);
		}

		private static bool FastApproximately(float a, float b, float threshold)
		{
			return ((a - b < 0f) ? ((a - b) * -1f) : (a - b)) <= threshold;
		}

		private void Log(object o)
		{
			Modding.Logger.Log("[Zemer] " + o);
		}

		private void OnDestroy()
		{
			On.HealthManager.Hit -= OnBlockedHit;
			On.HealthManager.Die -= HealthManagerOnDie;
			On.HealthManager.TakeDamage -= HealthManager_TakeDamage;
			On.EnemyDreamnailReaction.RecieveDreamImpact -= OnReceiveDreamImpact;
		}
	}
	public class ZemerControllerP2 : MonoBehaviour
	{
		private HealthManager _hm;

		private BoxCollider2D _bc;

		private SpriteRenderer _sr;

		private EnemyDreamnailReaction _dnailReac;

		private GameObject _dd;

		private GameObject[] traitorSlam;

		private int traitorSlamIndex;

		private GameObject _dnailEff;

		private Animator _anim;

		private Rigidbody2D _rb;

		private System.Random _rand;

		private DamageHero _dh;

		private EnemyHitEffectsUninfected _hitEffects;

		private EnemyDeathEffectsUninfected _deathEff;

		private GameObject _target;

		private ParticleSystem grass;

		private string[] _commonAtt;

		private List<GameObject> _destroyAtEnd;

		private readonly float deathGndOffset = (OWArenaFinder.IsInOverWorld ? 1.18f : 0.7f);

		private readonly float GroundY = (OWArenaFinder.IsInOverWorld ? 108.3f : ((CustomWP.boss == CustomWP.Boss.All || CustomWP.boss == CustomWP.Boss.Ogrim) ? 9f : 28.8f));

		private readonly float LeftX = (OWArenaFinder.IsInOverWorld ? 240.1f : ((CustomWP.boss == CustomWP.Boss.All || CustomWP.boss == CustomWP.Boss.Ogrim) ? 61f : 11.2f));

		private readonly float RightX = (OWArenaFinder.IsInOverWorld ? 273.9f : ((CustomWP.boss == CustomWP.Boss.All || CustomWP.boss == CustomWP.Boss.Ogrim) ? 91f : 45.7f));

		private readonly float SlamY = (OWArenaFinder.IsInOverWorld ? 105f : ((CustomWP.boss == CustomWP.Boss.All || CustomWP.boss == CustomWP.Boss.Ogrim) ? 6f : 25.9f));

		private readonly float NailHeightGrab = ((CustomWP.boss == CustomWP.Boss.All || CustomWP.boss == CustomWP.Boss.Ogrim) ? 10f : 19f);

		private readonly int Phase2HP = ((CustomWP.lev > 0) ? 1500 : 1350);

		private int DoneFrenzyAtt;

		private readonly int Phase3HP = ((CustomWP.lev > 0) ? 950 : 800);

		private readonly float NailMaxHeightStop = ((CustomWP.boss == CustomWP.Boss.All || CustomWP.boss == CustomWP.Boss.Ogrim) ? 18.5f : 39f);

		private readonly float NailMaxLeftStop = ((CustomWP.boss == CustomWP.Boss.All || CustomWP.boss == CustomWP.Boss.Ogrim) ? 62f : 13f);

		private readonly float NailMaxRightStop = ((CustomWP.boss == CustomWP.Boss.All || CustomWP.boss == CustomWP.Boss.Ogrim) ? 89f : 43f);

		private const float TurnDelay = 0.05f;

		private const float LandSlideTPInDelay = 0.07f;

		private const float LaserNutsEndDelay = 0.5f;

		private const float IdleDelay = 0.1f;

		private const float DashDelay = 0.18f;

		private float MIDDLE;

		private const float ThrowDelay = 0.2f;

		private const float SwingOutToInDelay = 0.75f;

		private const float GenericReturnDelay = 0.75f;

		private const float RecoveryReturnFirstDelay = 3.5f;

		private const float RecoveryReturnRestDelay = 0.75f;

		private const float NailSize = 1.15f;

		private const float LeaveAnimSpeed = 2.75f;

		private const float AfterAirNailGrabDelay = 0.4f;

		private const float StunTimeFirst = 1.5f;

		private const float StunTimeRest = 1.2f;

		private const float DashXVel = 75f;

		private const float SmallPillarSpd = 23.5f;

		private readonly Vector2 SmallPillarSize = new Vector2(1.1f, 0.45f);

		private const float Att1CompAnticTime = 0.25f;

		private float Att1BaseDelay = 0.4f;

		private float TwoFancyDelay = 0.25f;

		private readonly Vector3 LeaveOffset = new Vector3(1.5f, 1.5f);

		private PlayMakerFSM _pvFsm;

		private Coroutine _counterRoutine;

		private bool _blockedHit;

		private bool _countering;

		private Func<IEnumerator> _lastAtt;

		private bool isHit;

		public bool DoPhase;

		private bool _isKnockingOut;

		private static readonly int FLASH_AMOUNT = Shader.PropertyToID("_FlashAmount");

		private void Awake()
		{
			DoneFrenzyAtt = 0;
			MIDDLE = (RightX + LeftX) / 2f;
			OnDestroy();
			On.HealthManager.TakeDamage += HealthManager_TakeDamage;
			On.EnemyDreamnailReaction.RecieveDreamImpact += OnReceiveDreamImpact;
			On.HealthManager.Die += HealthManagerOnDie;
			_hm = GetComponent<HealthManager>();
			_anim = GetComponent<Animator>();
			_bc = GetComponent<BoxCollider2D>();
			_rb = GetComponent<Rigidbody2D>();
			_rb.isKinematic = true;
			_rb.interpolation = RigidbodyInterpolation2D.Interpolate;
			_sr = GetComponent<SpriteRenderer>();
			_dh = GetComponent<DamageHero>();
			_dh.damageDealt = 1;
			_dd = FiveKnights.preloadedGO["WD"];
			_dnailEff = _dd.GetComponent<EnemyDreamnailReaction>().GetAttr<EnemyDreamnailReaction, GameObject>("dreamImpactPrefab");
			_rand = new System.Random();
			_dnailReac = GetComponent<EnemyDreamnailReaction>();
			_dnailReac.enabled = true;
			_hitEffects = base.gameObject.GetComponent<EnemyHitEffectsUninfected>();
			_hitEffects.enabled = true;
			_pvFsm = FiveKnights.preloadedGO["PV"].LocateMyFSM("Control");
			GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["TraitorSlam"].transform.Find("Grass").gameObject, base.transform, worldPositionStays: true);
			grass = gameObject.GetComponent<ParticleSystem>();
			grass.gameObject.SetActive(value: true);
			grass.Stop();
			_destroyAtEnd = new List<GameObject>();
			AssignFields();
		}

		private void HealthManagerOnDie(On.HealthManager.orig_Die orig, HealthManager self, float? attackDirection, AttackTypes attackType, bool ignoreEvasion)
		{
			if (!self.gameObject.name.Contains("Zemer"))
			{
				orig(self, attackDirection, attackType, ignoreEvasion);
			}
		}

		private IEnumerator Start()
		{
			_hm.hp = Phase2HP;
			EnemyHPBarImport.RefreshHPBar(base.gameObject);
			_deathEff = _dd.GetComponent<EnemyDeathEffectsUninfected>();
			_target = HeroController.instance.gameObject;
			Rigidbody2D[] array = UnityEngine.Object.FindObjectsOfType<Rigidbody2D>(includeInactive: true);
			foreach (Rigidbody2D rigidbody2D in array)
			{
				if (rigidbody2D.name.Contains("Nail") && rigidbody2D.transform.parent == null)
				{
					UnityEngine.Object.Destroy(rigidbody2D.gameObject);
				}
			}
			yield return EndPhase1(firstDeath: true);
		}

		private void Update()
		{
			if (_isKnockingOut)
			{
				_rb.isKinematic = false;
				_bc.enabled = false;
			}
			if ((!(_bc == null) && _bc.enabled) || _anim.GetCurrentAnimatorStateInfo(0).IsName("ZKnocked") || _anim.GetCurrentAnimatorStateInfo(0).IsName("ZThrow2B"))
			{
				if (base.transform.GetPositionX() > RightX - 3f && _rb.velocity.x > 0f)
				{
					_rb.velocity = new Vector2(0f, _rb.velocity.y);
				}
				if (base.transform.GetPositionX() < LeftX + 3f && _rb.velocity.x < 0f)
				{
					_rb.velocity = new Vector2(0f, _rb.velocity.y);
				}
			}
		}

		private IEnumerator Attacks()
		{
			int counterCount = 0;
			Dictionary<Func<IEnumerator>, int> rep = new Dictionary<Func<IEnumerator>, int>
			{
				[FancyAttack] = 0,
				[Dash] = 0,
				[Attack1Base] = 0,
				[AerialAttack] = 0,
				[NailLaunch] = 0,
				[DoubleFancy] = 0,
				[SweepDash] = 0,
				[ZemerSlam] = 0,
				[Attack1Complete] = 0
			};
			Dictionary<Func<IEnumerator>, int> max = new Dictionary<Func<IEnumerator>, int>
			{
				[FancyAttack] = 3,
				[Dash] = 2,
				[Attack1Complete] = 2,
				[Attack1Base] = 2,
				[AerialAttack] = 2,
				[NailLaunch] = 2,
				[DoubleFancy] = 2,
				[SweepDash] = 1,
				[ZemerSlam] = 1
			};
			while (true)
			{
				Log("[Waiting to start calculation]");
				_anim.Play("ZIdle");
				isHit = false;
				yield return new WaitSecWhile(() => !isHit, 0.1f);
				Log("[End of Wait]");
				Vector2 posZem = base.transform.position;
				Vector2 posH = _target.transform.position;
				_dh.damageDealt = 1;
				if (posH.y > GroundY + 9f && (posH.x <= LeftX || posH.x >= RightX))
				{
					Log("Doing SpinAttack");
					yield return SpinAttack();
					Log("Done SpinAttack");
				}
				else if (FastApproximately(posZem.x, posH.x, 5f) && (DoneFrenzyAtt != 0 || !((float)_hm.hp < 0.65f * (float)Phase2HP)) && (DoneFrenzyAtt != 1 || !((float)_hm.hp < 0.35f * (float)Phase2HP)))
				{
					if (_rand.Next(0, 4) == 0 && counterCount < 2)
					{
						counterCount++;
						Log("Doing Counter");
						ZemerCounter();
						_countering = true;
						yield return new WaitWhile(() => _countering);
						Log("Done Counter");
					}
					else
					{
						counterCount = 0;
						List<Func<IEnumerator>> attLst = new List<Func<IEnumerator>> { Dash, FancyAttack, NailLaunch, null };
						Log("Choosing Attack");
						Func<IEnumerator> att = MiscMethods.ChooseAttack(attLst, rep, max);
						Log("Done Choosing Attack");
						if (att == new Func<IEnumerator>(FancyAttack))
						{
							rep[AerialAttack] = 2;
						}
						if (att != new Func<IEnumerator>(Dash))
						{
							Log("Doing Special Dodge");
							yield return Dodge();
							rep[Dash] = 2;
							Log("End Special Dodge's Dodge");
						}
						if (att != null)
						{
							Log("Doing " + att.Method.Name);
							yield return att();
							Log("Done " + att.Method.Name);
						}
						Log("End Special Dodge");
					}
				}
				List<Func<IEnumerator>> list = new List<Func<IEnumerator>> { NailLaunch, null, null };
				Func<IEnumerator> currAtt = null;
				if (!FastApproximately(posZem.x, posH.x, 10f))
				{
					currAtt = list[_rand.Next(0, list.Count)];
				}
				if (currAtt != null)
				{
					Log("Doing " + currAtt.Method.Name);
					yield return currAtt();
					Log("Done " + currAtt.Method.Name);
				}
				else
				{
					list = new List<Func<IEnumerator>> { Dash, Attack1Base, NailLaunch, AerialAttack, DoubleFancy, SweepDash, ZemerSlam };
					Log("Choosing Attack");
					currAtt = MiscMethods.ChooseAttack(list, rep, max);
					Log("Done Choosing Attack");
					Log("Doing " + currAtt.Method.Name);
					yield return currAtt();
					Log("Done " + currAtt.Method.Name);
					if (currAtt == new Func<IEnumerator>(DoubleFancy) || currAtt == new Func<IEnumerator>(FancyAttack))
					{
						rep[AerialAttack] = 2;
					}
					if ((currAtt == new Func<IEnumerator>(ZemerSlam) || currAtt == new Func<IEnumerator>(Dash)) && rep[NailLaunch] < max[NailLaunch] && UnityEngine.Random.Range(0, 2) == 0)
					{
						rep[NailLaunch]++;
						Log("Doing NailLaunch");
						yield return NailLaunch();
						Log("Done NailLaunch");
					}
					if (currAtt == new Func<IEnumerator>(Attack1Base))
					{
						List<Func<IEnumerator>> attLst2 = (FastApproximately(base.transform.position.x, _target.transform.position.x, 7f) ? new List<Func<IEnumerator>> { Attack1Complete } : new List<Func<IEnumerator>> { Attack1Complete, FancyAttack, FancyAttack });
						Log("Choosing Attack");
						currAtt = MiscMethods.ChooseAttack(attLst2, rep, max);
						Log("Done Choosing Attack");
						Log("Doing " + currAtt.Method.Name);
						yield return currAtt();
						Log("Done " + currAtt.Method.Name);
						if (currAtt == new Func<IEnumerator>(FancyAttack) && _rand.Next(0, 3) < 2)
						{
							Log("Doing Special Fancy Attack");
							yield return Dodge();
							yield return new WaitForSeconds(TwoFancyDelay);
							if (UnityEngine.Random.Range(0, 4) == 0)
							{
								Log("Doing Special Fancy Attack Nail Launch version");
								yield return NailLaunch();
								Log("Done NailLaunch");
							}
							else
							{
								Log("Doing FancyAttack");
								yield return FancyAttack();
								Log("Done FancyAttack");
								Log("Doing Dash");
								yield return Dash();
								Log("Done Dash");
							}
							Log("Done Special Fancy Attack");
						}
					}
				}
				Log("[Done Setting Attacks]");
				_anim.Play("ZIdle");
				Log("[Restarting Calculations]");
				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator AerialAttack()
		{
			_lastAtt = AerialAttack;
			yield return Attack();
			IEnumerator Attack()
			{
				if (!IsFacingPlayer())
				{
					yield return Turn();
				}
				base.transform.Find("BladeAerialShadow").gameObject.SetActive(value: true);
				float xVel = (float)FaceHero() * -1f;
				_anim.Play("ZAerial2");
				PlayAudioClip(ZemRandAudio.PickRandomZemAud(2, 8));
				yield return _anim.WaitToFrame(8);
				_rb.velocity = new Vector2(xVel * 35f, 18f);
				_rb.gravityScale = 1.3f;
				_rb.isKinematic = false;
				yield return new WaitForSeconds(0.1f);
				yield return _anim.WaitToFrame(10);
				PlayAudioClip("AudBigSlash2", 0.15f);
				yield return _anim.WaitToFrame(13);
				PlayAudioClip("AudBigSlash2", 0.15f);
				yield return new WaitWhile(() => base.transform.position.y > GroundY);
				_rb.velocity = Vector2.zero;
				_rb.gravityScale = 0f;
				_rb.isKinematic = true;
				base.transform.position = new Vector2(base.transform.position.x, GroundY);
				yield return _anim.PlayToEnd();
				_anim.Play("ZIdle");
			}
		}

		private float GetAngleTo(Vector2 from, Vector2 to)
		{
			float num;
			for (num = Mathf.Atan2(to.y - from.y, to.x - from.x) * 57.29578f; (double)num < 0.0; num += 360f)
			{
			}
			return num;
		}

		private IEnumerator NailLaunch()
		{
			float dir = 0f;
			bool doSlam = false;
			_lastAtt = NailLaunch;
			yield return Throw();
			IEnumerator GndLeave(float num, bool isSpedup = false)
			{
				if (isSpedup)
				{
					_anim.speed = 2f;
					_anim.PlayAt("ZThrow2", 3);
					Spring(isIn: false, base.transform.position + new Vector3((0f - num) * LeaveOffset.x, 0f, 0f), 1.6f);
					yield return null;
					yield return _anim.WaitToFrame(4);
					base.transform.position += new Vector3((0f - num) * LeaveOffset.x, 0f);
					yield return _anim.PlayToEnd();
					ToggleZemer(visible: false);
				}
				else
				{
					_anim.speed = 2f;
					_anim.Play("ZThrow2");
					yield return null;
					yield return _anim.WaitToFrame(2);
					Spring(isIn: false, base.transform.position + new Vector3((0f - num) * LeaveOffset.x, 0f, 0f), 1.5f);
					yield return _anim.WaitToFrame(4);
					base.transform.position += new Vector3((0f - num) * LeaveOffset.x, 0f);
					yield return _anim.PlayToEnd();
					ToggleZemer(visible: false);
				}
			}
			IEnumerator LaunchSide(GameObject nail, bool leave = true)
			{
				GameObject gameObject = nail.transform.Find("ZNailC").gameObject;
				Rigidbody2D rbNail = nail.GetComponent<Rigidbody2D>();
				CollisionCheck cc = gameObject.GetComponent<CollisionCheck>();
				Log("Doing launch side");
				if (cc == null)
				{
					cc = gameObject.AddComponent<CollisionCheck>();
					cc.OnCollide += delegate
					{
						PlayAudioClip("AudLand");
						GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
						nail.GetComponent<SpriteRenderer>().enabled = false;
						nail.transform.Find("ZNailN").GetComponent<SpriteRenderer>().enabled = true;
						rbNail.velocity = Vector2.zero;
						UnityEngine.Object.Destroy(cc);
					};
				}
				if (leave)
				{
					cc.Hit = rbNail.velocity == Vector2.zero;
					yield return GndLeave(dir);
					yield return new WaitWhile(() => !cc.Hit && rbNail.velocity != Vector2.zero);
					yield return new WaitForSeconds(0.75f);
				}
				Vector2 vector = base.transform.position;
				Vector2 vector2 = nail.transform.Find("Point").position;
				Vector3 localScale = base.transform.localScale;
				if (vector2.x < MIDDLE)
				{
					base.transform.localScale = new Vector3(Mathf.Abs(localScale.x), localScale.y, localScale.z);
					base.transform.position = new Vector2(vector2.x + LeaveOffset.x, vector.y);
					Spring(isIn: true, base.transform.position);
					yield return new WaitForSeconds(0.15f);
					ToggleZemer(visible: true);
					_anim.speed = 2f;
					_anim.Play("ZThrow3Gnd");
					yield return null;
					yield return _anim.WaitToFrame(1);
					base.transform.position -= new Vector3(LeaveOffset.x, 0f);
					yield return _anim.WaitToFrame(2);
					_anim.speed = 1f;
					UnityEngine.Object.Destroy(nail);
				}
				else
				{
					base.transform.localScale = new Vector3(0f - Mathf.Abs(localScale.x), localScale.y, localScale.z);
					base.transform.position = new Vector2(vector2.x - LeaveOffset.x, vector.y);
					Spring(isIn: true, base.transform.position);
					yield return new WaitForSeconds(0.15f);
					ToggleZemer(visible: true);
					_anim.speed = 2f;
					_anim.Play("ZThrow3Gnd");
					yield return null;
					yield return _anim.WaitToFrame(1);
					base.transform.position += new Vector3(LeaveOffset.x, 0f);
					yield return _anim.WaitToFrame(2);
					_anim.speed = 1f;
					UnityEngine.Object.Destroy(nail);
				}
				if (_hm.hp <= Phase3HP && _rand.Next(0, 5) < 3)
				{
					_anim.PlayAt("Z2Crawl", 1);
					yield return null;
					yield return new WaitWhile(() => _anim.IsPlaying());
					float dir2 = Mathf.Sign(base.transform.localScale.x);
					yield return RageCombo(dir2, special: false);
				}
				else
				{
					yield return _anim.PlayToEnd();
					if (doSlam)
					{
						yield return ZemerSlam();
					}
				}
			}
			IEnumerator LaunchUp(float rot, GameObject nail, bool didFakeOut, int repeatCnt)
			{
				GameObject col = nail.transform.Find("ZNailC").gameObject;
				CollisionCheck cc = col.GetComponent<CollisionCheck>() ?? col.AddComponent<CollisionCheck>();
				Rigidbody2D rb = nail.GetComponent<Rigidbody2D>();
				if (repeatCnt == 0)
				{
					yield return GndLeave(dir, isSpedup: true);
				}
				cc.Hit = rb.velocity == Vector2.zero;
				yield return new WaitWhile(() => nail.transform.position.y < NailHeightGrab && !cc.Hit && rb.velocity != Vector2.zero);
				Vector3 position = nail.transform.position + new Vector3(5f * Mathf.Cos(rot), 5f * Mathf.Sin(rot), 0f);
				if (position.y > NailMaxHeightStop)
				{
					position.y = NailMaxHeightStop;
				}
				if (position.x > NailMaxRightStop)
				{
					position.x = NailMaxRightStop;
				}
				if (position.x < NailMaxLeftStop)
				{
					position.x = NailMaxLeftStop;
				}
				base.transform.position = position;
				float oldPlayerPosX = _target.transform.position.x;
				ToggleZemer(visible: true);
				dir = FaceHero();
				_anim.speed = 2f;
				_anim.Play("ZThrow3Air", -1, 0f);
				yield return null;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 1);
				nail.SetActive(value: false);
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 2);
				PlayAudioClip(ZemRandAudio.PickRandomZemAud(2, 8));
				_anim.speed = 1f;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 4);
				_anim.enabled = false;
				yield return new WaitForSeconds(0.2f);
				Vector2 hero = _target.transform.position;
				Vector2 zem = base.gameObject.transform.position;
				if (!didFakeOut && ((oldPlayerPosX - zem.x) * (hero.x - zem.x) < 0f || zem.x.Within(hero.x, 6f)))
				{
					float tpToX = ((hero.x < MIDDLE) ? ((MIDDLE + RightX) / 2f) : ((LeftX + MIDDLE) / 2f));
					didFakeOut = true;
					repeatCnt += 10;
					Spring(isIn: false, base.transform.position, 1.6f);
					yield return new WaitForSeconds(0.1f);
					ToggleZemer(visible: false);
					base.transform.position = new Vector3(tpToX, zem.y);
					Spring(isIn: true, base.transform.position);
					yield return new WaitForSeconds(0.15f);
					dir = FaceHero();
					hero = _target.transform.position - new Vector3(0f, 3f, 0f);
					zem = base.gameObject.transform.position;
					ToggleZemer(visible: true);
				}
				if (repeatCnt > 1)
				{
					hero = new Vector2(hero.x, GroundY);
				}
				yield return FlashRepeat(hero, 2f / 15f, cringeVersion: true);
				_anim.enabled = true;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 5);
				nail.SetActive(value: true);
				rot = GetAngleTo(zem, hero) * ((float)Math.PI / 180f);
				float num = rot + ((dir > 0f) ? ((float)Math.PI) : 0f);
				Log($"Rot is {rot * 57.29578f} and rot vel is {num * 57.29578f}");
				nail.transform.SetRotation2D(num * 57.29578f);
				nail.transform.localScale = new Vector3(Mathf.Sign(base.transform.localScale.x) * 1.15f, 1.15f, 1.15f);
				Vector2 velocity = new Vector2(Mathf.Cos(rot) * 90f, Mathf.Sin(rot) * 90f);
				float num2 = ((velocity.x > 0f) ? (RightX - nail.transform.position.x) : (nail.transform.position.x - LeftX)) / Mathf.Abs(velocity.x);
				if (velocity.y * num2 + nail.transform.position.y > NailMaxHeightStop - 7f && repeatCnt <= 1)
				{
					repeatCnt++;
					Log("Doing endY!!");
					rb.velocity = velocity;
					nail.transform.position = base.transform.position;
					if (cc != null)
					{
						UnityEngine.Object.Destroy(cc);
					}
					cc = col.AddComponent<CollisionCheck>();
					nail.GetComponent<SpriteRenderer>().enabled = true;
					nail.transform.Find("ZNailN").GetComponent<SpriteRenderer>().enabled = false;
					cc.Freeze = false;
					cc.Hit = false;
					cc.OnCollide += delegate
					{
						if (!(nail.transform.position.y > GroundY + 5f))
						{
							PlayAudioClip("AudLand");
							GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
							nail.GetComponent<SpriteRenderer>().enabled = false;
							nail.transform.Find("ZNailN").GetComponent<SpriteRenderer>().enabled = true;
							rb.velocity = Vector2.zero;
							UnityEngine.Object.Destroy(cc);
						}
					};
					yield return new WaitForSeconds(0.02f);
					cc.Freeze = true;
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 8);
					_anim.speed = 2f;
					Spring(isIn: false, base.transform.position + new Vector3(LeaveOffset.x * Mathf.Cos(rot), LeaveOffset.y * Mathf.Sin(rot), 0f));
					base.transform.position += new Vector3(LeaveOffset.x * Mathf.Cos(rot), LeaveOffset.y * Mathf.Sin(rot));
					yield return _anim.PlayToEnd();
					ToggleZemer(visible: false);
					cc.Freeze = true;
					yield return LaunchUp(rot, nail, didFakeOut, repeatCnt);
				}
				else
				{
					Log("did not do endy so slamming down");
					rb.velocity = new Vector2(Mathf.Cos(rot) * 90f, Mathf.Sin(rot) * 90f);
					col.GetComponent<BoxCollider2D>().enabled = true;
					col.SetActive(value: true);
					nail.transform.position = base.transform.position;
					if (cc != null)
					{
						UnityEngine.Object.Destroy(cc);
					}
					cc = col.AddComponent<CollisionCheck>();
					cc.Hit = false;
					cc.Freeze = false;
					nail.GetComponent<SpriteRenderer>().enabled = true;
					nail.transform.Find("ZNailN").GetComponent<SpriteRenderer>().enabled = false;
					cc.OnCollide += delegate
					{
						if (!(nail.transform.position.y > GroundY + 5f))
						{
							PlayAudioClip("AudLand");
							GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
							nail.GetComponent<SpriteRenderer>().enabled = false;
							nail.transform.Find("ZNailN").GetComponent<SpriteRenderer>().enabled = true;
							rb.velocity = Vector2.zero;
							UnityEngine.Object.Destroy(cc);
						}
					};
					yield return new WaitForSeconds(0.02f);
					cc.Freeze = true;
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 8);
					_anim.speed = 2f;
					Spring(isIn: false, base.transform.position + new Vector3(LeaveOffset.x * Mathf.Cos(rot), LeaveOffset.y * Mathf.Sin(rot), 0f));
					base.transform.position += new Vector3(LeaveOffset.x * Mathf.Cos(rot), LeaveOffset.y * Mathf.Sin(rot));
					yield return _anim.PlayToEnd();
					ToggleZemer(visible: false);
					yield return new WaitForSeconds(0.1f);
					cc.Hit = rb.velocity == Vector2.zero;
					yield return new WaitWhile(() => !cc.Hit && rb.velocity != Vector2.zero);
					base.transform.position = new Vector3(80f, GroundY);
					yield return new WaitForSeconds(0.75f);
					yield return LaunchSide(nail, leave: false);
				}
			}
			IEnumerator Throw()
			{
				if (((Vector2)_target.transform.position).x.Within(base.transform.position.x, 12f))
				{
					if (base.transform.position.x < LeftX + 6f || base.transform.position.x > RightX - 6f)
					{
						yield return Dash();
						if (_target.transform.position.x.Within(base.transform.position.x, 12f))
						{
							yield return Dodge();
						}
					}
					else
					{
						yield return Dodge();
					}
				}
				dir = FaceHero();
				_anim.Play("ZThrow1");
				PlayAudioClip(ZemRandAudio.PickRandomZemAud(2, 8));
				yield return _anim.WaitToFrame(2);
				_anim.enabled = false;
				yield return new WaitForSeconds(0.1f);
				Vector2 hero = _target.transform.position;
				dir = FaceHero();
				yield return FlashRepeat(hero, 0.1f, cringeVersion: true);
				_anim.enabled = true;
				yield return _anim.WaitToFrame(3);
				Transform transform = base.transform.Find("ZNailB").Find("Center");
				float rot = GetAngleTo(base.transform.Find("ZNailB").position, hero) * ((float)Math.PI / 180f);
				string text = LayerMask.LayerToName(8);
				RaycastHit2D raycastHit2D = Physics2D.BoxCast(transform.position, new Vector2(1f, 1f), 0f, new Vector2(Mathf.Cos(rot), Mathf.Sin(rot)), float.PositiveInfinity, LayerMask.GetMask(text));
				float num = (0f - Mathf.Sign(Mathf.Cos(rot))) * 6f;
				for (int i = 0; i < 10; i++)
				{
					if (!(Vector2.Distance(transform.position, raycastHit2D.point + new Vector2(num, 0f)) < 10f))
					{
						break;
					}
					hero += new Vector2(0f, 1f);
					rot = GetAngleTo(transform.position, hero) * ((float)Math.PI / 180f);
					raycastHit2D = Physics2D.BoxCast(transform.position, new Vector2(1f, 1f), 0f, new Vector2(Mathf.Cos(rot), Mathf.Sin(rot)), float.PositiveInfinity, LayerMask.GetMask(text));
				}
				Log($"Putting nail to {raycastHit2D.point.x + num}");
				float num2 = rot + ((dir > 0f) ? ((float)Math.PI) : 0f);
				GameObject obj = base.transform.Find("NailHand").gameObject;
				GameObject nailPar = UnityEngine.Object.Instantiate(base.transform.Find("ZNailB").gameObject);
				Rigidbody2D parRB = nailPar.GetComponent<Rigidbody2D>();
				if (CustomWP.boss == CustomWP.Boss.Mystic || CustomWP.boss == CustomWP.Boss.Ze)
				{
					Log("double size for reg");
					nailPar.transform.Find("ZNailC").GetComponent<BoxCollider2D>().size *= 2.5f;
				}
				else if (CustomWP.boss == CustomWP.Boss.All)
				{
					Log("incr size for pantheon");
					nailPar.transform.Find("ZNailC").GetComponent<BoxCollider2D>().size *= 1.5f;
				}
				obj.transform.SetRotation2D(num2 * 57.29578f);
				nailPar.transform.SetRotation2D(num2 * 57.29578f);
				nailPar.transform.position = base.transform.Find("ZNailB").position;
				nailPar.transform.localScale = new Vector3(Mathf.Sign(base.transform.localScale.x) * 1.15f, 1.15f, 1.15f);
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 4);
				yield return new WaitForSeconds(0.07f);
				nailPar.SetActive(value: true);
				float num3 = 80f;
				parRB.velocity = new Vector2(Mathf.Cos(rot) * num3, Mathf.Sin(rot) * num3);
				nailPar.AddComponent<ExtraNailBndCheck>();
				yield return new WaitForSeconds(0.01f);
				CollisionCheck cc = nailPar.transform.Find("ZNailC").gameObject.AddComponent<CollisionCheck>();
				cc.Freeze = true;
				cc.OnCollide += delegate
				{
					PlayAudioClip("AudLand");
					GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
					nailPar.GetComponent<SpriteRenderer>().enabled = false;
					nailPar.transform.Find("ZNailN").GetComponent<SpriteRenderer>().enabled = true;
					nailPar.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
					UnityEngine.Object.Destroy(cc);
				};
				yield return new WaitWhile(() => _anim.IsPlaying());
				yield return (nailPar.transform.position.y > GroundY + 1f) ? LaunchUp(rot, nailPar, didFakeOut: false, 0) : LaunchSide(nailPar);
			}
		}

		private IEnumerator ZemerSlam()
		{
			_lastAtt = ZemerSlam;
			yield return Slam();
			IEnumerator Slam()
			{
				base.transform.position += new Vector3(0f, 1.32f);
				PlayAudioClip(ZemRandAudio.PickRandomZemAud(9, 13));
				yield return _anim.PlayToFrame("ZSlamNew", 7);
				SpawnShockwaves(2f, 50f, 1, base.transform.position);
				GameCameras.instance.cameraShakeFSM.SendEvent("BigShake");
				PlayAudioClip("AudLand");
				yield return _anim.PlayToEnd();
				base.transform.position -= new Vector3(0f, 1.32f);
			}
			void SpawnShockwaves(float vertScale, float speed, int damage, Vector2 pos)
			{
				bool[] obj = new bool[2] { false, true };
				PlayMakerFSM fsm = FiveKnights.preloadedGO["Mage"].LocateMyFSM("Mage Lord");
				bool[] array = obj;
				foreach (bool flag in array)
				{
					GameObject obj2 = UnityEngine.Object.Instantiate(fsm.GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>("Quake Waves", 0).gameObject.Value);
					PlayMakerFSM playMakerFSM = obj2.LocateMyFSM("shockwave");
					playMakerFSM.FsmVariables.FindFsmBool("Facing Right").Value = flag;
					playMakerFSM.FsmVariables.FindFsmFloat("Speed").Value = speed;
					obj2.AddComponent<DamageHero>().damageDealt = damage;
					obj2.SetActive(value: true);
					obj2.transform.SetPosition2D(new Vector2(pos.x + (flag ? 0.5f : (-0.5f)), SlamY));
					obj2.transform.SetScaleX(vertScale);
				}
			}
		}

		private IEnumerator Attack1Complete()
		{
			if (!(_lastAtt != new Func<IEnumerator>(Attack1Base)))
			{
				_lastAtt = this.Attack1Complete;
				yield return Attack1Complete();
			}
			IEnumerator Attack1Complete()
			{
				_anim.Play("ZAtt1");
				float xVel = FaceHero() * -1;
				_anim.enabled = false;
				yield return new WaitForSeconds(0.25f);
				_anim.enabled = true;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 3);
				PlayAudioClip("AudBigSlash", 0.15f);
				_rb.velocity = new Vector2(40f * xVel, 0f);
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 4);
				_rb.velocity = Vector2.zero;
				yield return new WaitWhile(() => _anim.IsPlaying());
				_anim.Play("ZIdle");
			}
		}

		private IEnumerator Attack1Base()
		{
			_lastAtt = this.Attack1Base;
			yield return Attack1Base();
			IEnumerator Attack1Base()
			{
				if (!IsFacingPlayer())
				{
					yield return Turn();
				}
				float xVel = (float)FaceHero() * -1f;
				PlayAudioClip(ZemRandAudio.PickRandomZemAud(2, 8));
				yield return _anim.PlayToFrame("ZAtt1Intro", 1);
				_anim.enabled = false;
				yield return new WaitForSeconds(Att1BaseDelay);
				_anim.enabled = true;
				yield return _anim.WaitToFrame(2);
				PlayAudioClip("Slash", 0.15f);
				yield return _anim.WaitToFrame(6);
				if ((int)(0f - xVel) != FaceHero(onlyCalc: true))
				{
					yield return RageCombo(0f - xVel, special: false);
					_lastAtt = null;
				}
				else
				{
					yield return _anim.PlayToEnd();
					_rb.velocity = new Vector2(23f * xVel, 0f);
					_anim.speed = 1.5f;
					while ((xVel > 0f && base.transform.position.x < RightX - 10f) || (xVel < 0f && base.transform.position.x > LeftX + 10f))
					{
						yield return _anim.PlayToEndWithActions("ZAtt1Loop", (0, delegate
						{
							PlayAudioClip("Slash", 0.15f);
						}));
					}
					_anim.speed = 1f;
					_anim.Play("ZAtt1End");
					_rb.velocity = Vector2.zero;
					yield return _anim.PlayToEnd();
				}
			}
		}

		private IEnumerator DoubleFancy()
		{
			yield return LeaveTemp(-FaceHero(), 0.75f);
			yield return BackIn();
			yield return FancyOne();
			yield return DashOtherSide();
			yield return FancyOne();
			IEnumerator BackIn()
			{
				float dir = Mathf.Sign(_target.transform.position.x - MIDDLE);
				float x = ((dir > 0f) ? (LeftX + 11f) : (RightX - 11f));
				float tarX = ((dir > 0f) ? (LeftX + 6f) : (RightX - 6f));
				base.transform.position = new Vector3(x, GroundY + 6f);
				Vector3 localScale = base.transform.localScale;
				base.transform.localScale = new Vector3(dir * Mathf.Abs(localScale.x), localScale.y, localScale.z);
				Spring(isIn: true, base.transform.position, 1.8f);
				yield return new WaitForSeconds(0.15f);
				ToggleZemer(visible: true);
				Vector2 vector = new Vector2(x - tarX, base.transform.position.y - GroundY - 0.95f);
				float num = Mathf.Atan(vector.y / vector.x);
				num = ((tarX < MIDDLE) ? (num + (float)Math.PI) : num);
				if (CustomWP.boss != CustomWP.Boss.All && CustomWP.boss != CustomWP.Boss.Ogrim)
				{
					_rb.velocity = new Vector2(55f * Mathf.Cos(num), 55f * Mathf.Sin(num));
				}
				else
				{
					_rb.velocity = new Vector2(55f * Mathf.Cos(num), 0f - Mathf.Abs(55f * Mathf.Sin(num)));
				}
				yield return _anim.PlayBlockingWhile("Z1ZipIn", () => base.transform.position.y > GroundY - 0.95f);
				base.transform.position = new Vector3(base.transform.position.x, GroundY - 0.95f);
				_rb.velocity = new Vector2((0f - dir) * 40f, 0f);
				_anim.Play("Z2Crawl");
				base.transform.position = new Vector3(base.transform.position.x, GroundY);
				yield return new WaitForSeconds(0.05f);
				_rb.velocity = new Vector2(0f, 0f);
				yield return _anim.WaitToFrame(1);
				_anim.Play("ZIdle");
			}
			IEnumerator DashOtherSide()
			{
				float dir = Mathf.Sign(base.transform.localScale.x);
				base.transform.Find("HyperCut").gameObject.SetActive(value: false);
				_anim.Play("ZDash2", -1, 0f);
				base.transform.position = new Vector3(base.transform.position.x, GroundY + 0.3f, base.transform.position.z);
				_anim.speed = 1.5f;
				yield return _anim.WaitToFrame(6);
				PlayAudioClip("ZemBladeShine");
				yield return _anim.WaitToFrame(10);
				PlayAudioClip("ZAudHoriz");
				yield return _anim.WaitToFrame(11);
				PlayAudioClip("AudDashIntro");
				yield return _anim.WaitToFrame(12);
				PlayAudioClip("AudDash");
				_rb.velocity = new Vector2((0f - dir) * 75f, 0f);
				yield return _anim.WaitToFrame(13);
				base.transform.position = new Vector3(base.transform.position.x, GroundY - 0.5f, base.transform.position.z);
				_anim.speed = 1f;
				_anim.enabled = false;
				if (0f - dir > 0f)
				{
					yield return new WaitWhile(() => base.transform.GetPositionX() < RightX - 5f);
				}
				else
				{
					yield return new WaitWhile(() => base.transform.GetPositionX() > LeftX + 5f);
				}
				_rb.velocity = Vector2.zero;
				_anim.enabled = true;
				yield return new WaitWhile(() => _anim.IsPlaying());
				_anim.Play("ZIdle");
				base.transform.position = new Vector3(base.transform.position.x, GroundY, base.transform.position.z);
			}
			IEnumerator FancyOne()
			{
				if (!IsFacingPlayer())
				{
					yield return Turn();
				}
				float dir = FaceHero();
				PlayAudioClip(ZemRandAudio.PickRandomZemAud(2, 8));
				yield return _anim.PlayToEndWithActions("ZAtt2", (0, delegate
				{
					PlayAudioClip("AudBasicSlash1");
				}), (2, delegate
				{
					PlayAudioClip("AudBasicSlash2");
				}), (6, delegate
				{
					PlayAudioClip("AudBigSlash2");
				}), (7, delegate
				{
					SpawnPillar(dir, Vector2.one, 20f);
				}));
				_anim.Play("ZIdle");
				yield return new WaitForSeconds(0.2f);
			}
		}

		private IEnumerator SweepDash()
		{
			yield return Leave();
			IEnumerator AltZSpin()
			{
				_anim.enabled = true;
				float num = GroundY + 8f;
				float num2 = (float)FaceHero() * -1f;
				float num3 = Mathf.Abs(_target.transform.GetPositionX() - base.transform.GetPositionX());
				float num4 = Mathf.Atan(Mathf.Abs(num - base.transform.GetPositionY()) / num3);
				num4 = ((num2 < 0f) ? ((float)Math.PI - num4) : num4);
				PlayAudioClip("AudDashIntro");
				grass.Stop();
				_anim.speed = 1f;
				_anim.PlayAt("ZSpin", 4);
				_rb.velocity = new Vector2(60f * Mathf.Cos(num4), 60f * Mathf.Sin(num4));
				yield return new WaitForSeconds(1f / 12f);
				_anim.enabled = false;
				yield return new WaitWhile(() => base.transform.position.x > LeftX + 5f && base.transform.position.x < RightX - 5f);
				_anim.enabled = true;
				_rb.velocity = Vector2.zero;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 8);
				PlayAudioClip("AudBigSlash2");
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 11);
				_rb.isKinematic = false;
				_rb.gravityScale = 1.5f;
				yield return new WaitWhile(() => base.transform.position.y > GroundY);
				PlayAudioClip("AudLand");
				_rb.velocity = Vector2.zero;
				_rb.gravityScale = 0f;
				_rb.isKinematic = true;
				base.transform.position = new Vector3(base.transform.position.x, GroundY);
				yield return new WaitWhile(() => _anim.IsPlaying());
				_anim.Play("ZIdle");
			}
			IEnumerator FlyStrike()
			{
				float heroX = _target.transform.position.x;
				_ = (float)FaceHero();
				float x = ((heroX < MIDDLE) ? (heroX + 8f) : (heroX - 8f));
				base.transform.position = new Vector3(x, GroundY + 9.5f);
				Spring(isIn: true, base.transform.position, 1.4f);
				yield return new WaitForSeconds(0.16f);
				ToggleZemer(visible: true);
				heroX = _target.transform.position.x;
				float num = Mathf.Sign(_target.transform.position.x - base.transform.position.x);
				float num2 = FaceHero();
				heroX = ((num > 0f) ? Mathf.Max(heroX - 3f, LeftX + 0.2f) : ((!(num < 0f)) ? ((num2 > 0f) ? (heroX - 3f) : (heroX + 3f)) : Mathf.Min(heroX + 3f, RightX - 0.2f)));
				_anim.Play("Z4AirSweep");
				PlayAudioClip(ZemRandAudio.PickRandomZemAud(2, 13));
				yield return null;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 1);
				PlayAudioClip("AudDash");
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 3);
				Vector2 vector = new Vector2(x - heroX, base.transform.position.y - GroundY - 0.95f);
				float num3 = Mathf.Atan(vector.y / vector.x);
				num3 = ((Mathf.Sin(num3) > 0f) ? (num3 + (float)Math.PI) : num3);
				_rb.velocity = new Vector2(65f * Mathf.Cos(num3), 65f * Mathf.Sin(num3));
				yield return new WaitWhile(() => base.transform.position.y > GroundY + 3.5f && _anim.GetCurrentFrame() < 4 && CheckIfStuck());
				_anim.enabled = false;
				yield return new WaitWhile(() => base.transform.position.y > GroundY + 3.5f && CheckIfStuck());
				_anim.enabled = true;
				yield return new WaitWhile(() => (double)base.transform.position.y > (double)GroundY - 0.3 && _anim.GetCurrentFrame() < 6 && CheckIfStuck());
				_anim.enabled = false;
				yield return new WaitWhile(() => base.transform.position.y > GroundY - 0.3f && CheckIfStuck());
				_anim.enabled = true;
				base.transform.position = new Vector3(base.transform.position.x, GroundY - 0.3f);
				_rb.gravityScale = 0f;
				_rb.isKinematic = true;
				_anim.speed = 1f;
				_rb.velocity = new Vector2(Mathf.Sign(_rb.velocity.x) * 30f, 0f);
				PlayAudioClip("AudLand");
				yield return new WaitWhile(() => _anim.IsPlaying());
				_rb.velocity = Vector2.zero;
				yield return LandSlide();
			}
			IEnumerator LandSlide()
			{
				float num = Mathf.Sign(base.transform.localScale.x);
				float XVel = 75f;
				_anim.PlayAt("ZThrow2", 4);
				Spring(isIn: false, base.transform.position + new Vector3((0f - num) * LeaveOffset.x, 0f, 0f), 1.5f);
				base.transform.position += new Vector3((0f - num) * LeaveOffset.x, 0f);
				yield return _anim.PlayToEnd();
				ToggleZemer(visible: false);
				int maxCount = 6;
				yield return new WaitForSeconds(0.2f);
				grass.Stop();
				grass.transform.position = base.transform.position;
				grass.transform.parent = base.transform;
				float baseDist = 11f;
				for (int i = 0; i < maxCount; i++)
				{
					float x = _target.transform.position.x;
					float y = base.transform.position.y;
					float offsetRand = _rand.Next(-1, 3);
					if (x.Within(LeftX, 7f))
					{
						base.transform.position = new Vector3(x + baseDist + offsetRand + 10f, y);
					}
					else if (x.Within(RightX, 7f))
					{
						base.transform.position = new Vector3(x - baseDist - offsetRand - 10f, y);
					}
					else
					{
						Vector3 position = ((i % 2 == 0) ? new Vector3(x + baseDist + offsetRand, y) : new Vector3(x - baseDist - offsetRand, y));
						if (position.x > RightX)
						{
							position.x = RightX;
						}
						if (position.x < LeftX)
						{
							position.x = LeftX;
						}
						base.transform.position = position;
					}
					yield return null;
					float signX = FaceHero();
					if (i == 0)
					{
						Spring(isIn: true, base.transform.position, 1.2f);
						yield return new WaitForSeconds(0.25f);
						ToggleZemer(visible: true);
					}
					else
					{
						Spring(isIn: true, base.transform.position, 1.5f);
						ToggleZemer(visible: true);
					}
					Log("Doing special dash");
					PlayAudioClip("AudDash");
					base.transform.Find("HyperCut").gameObject.SetActive(value: false);
					_anim.PlayAt("ZMultiDashAir", 1);
					_anim.enabled = true;
					_anim.speed = 0.5f;
					yield return null;
					grass.Play();
					float velX = (0f - signX) * (XVel + (float)i * 3f);
					_rb.velocity = new Vector2(velX, 0f);
					offsetRand = _rand.Next(1, 5);
					float oldDiff = _target.transform.position.x - base.transform.position.x;
					bool doSpinInstead = false;
					yield return new WaitWhile(delegate
					{
						if ((_target.transform.position.x.Within(LeftX, 2.5f) || _target.transform.position.x.Within(RightX, 2.5f)) && _target.transform.position.y > GroundY + 3f)
						{
							doSpinInstead = true;
							return false;
						}
						float num2 = _target.transform.position.x - base.transform.position.x;
						if (num2 * oldDiff <= 0f && Mathf.Abs(num2) > 7f + offsetRand)
						{
							return false;
						}
						if (_rb.velocity.x.Within(0f, 0.1f))
						{
							return false;
						}
						if (0f - signX <= 0f && base.transform.position.x.Within(LeftX, 3f))
						{
							return false;
						}
						return (!(0f - signX > 0f) || !base.transform.position.x.Within(RightX, 3f)) ? true : false;
					});
					if (doSpinInstead)
					{
						yield return AltZSpin();
						yield break;
					}
					if (i != maxCount - 1)
					{
						float x2 = velX * 0.1f;
						Vector3 newPos = base.transform.position + new Vector3(x2, 0f, 0f);
						if (newPos.x > RightX)
						{
							newPos.x = RightX - 1f;
						}
						if (newPos.x < LeftX)
						{
							newPos.x = LeftX + 1f;
						}
						Spring(isIn: false, newPos, 1.5f);
						_anim.speed = 1f;
						grass.Stop();
						yield return new WaitWhile(delegate
						{
							if (_rb.velocity.x.Within(0f, 0.1f))
							{
								return false;
							}
							if (0f - signX <= 0f)
							{
								if (base.transform.position.x.Within(LeftX, 1.2f))
								{
									return false;
								}
								if (base.transform.position.x < newPos.x)
								{
									return false;
								}
							}
							if (0f - signX > 0f)
							{
								if (0f - signX > 0f && base.transform.position.x.Within(RightX, 1.2f))
								{
									return false;
								}
								if (base.transform.position.x > newPos.x)
								{
									return false;
								}
							}
							return true;
						});
						_rb.velocity = Vector2.zero;
						ToggleZemer(visible: false);
						yield return new WaitForSeconds(0.2f);
					}
				}
				grass.Stop();
				_anim.speed = 1f;
				_anim.PlayAt("ZDash", 14);
				_anim.enabled = true;
				_rb.velocity = Vector2.zero;
				yield return new WaitWhile(() => _anim.IsPlaying());
			}
			IEnumerator Leave()
			{
				float num = FaceHero();
				yield return LeaveTemp(0f - num, 0.75f);
				yield return FlyStrike();
			}
		}

		private IEnumerator SwingTemp(float dir, float delay)
		{
			_anim.Play("Z3Swing");
			yield return null;
			yield return _anim.WaitToFrame(0);
			_anim.enabled = false;
			yield return new WaitForSeconds(delay);
			_anim.enabled = true;
			yield return _anim.WaitToFrame(1);
			PlayAudioClip("Slash", 0.15f);
			yield return _anim.WaitToFrame(2);
			SpawnPillar(0f - dir, SmallPillarSize, 23.5f);
			yield return _anim.WaitToFrame(5);
			Spring(isIn: false, base.transform.position + new Vector3(dir * LeaveOffset.x, LeaveOffset.y, 0f), 1.5f);
			yield return _anim.WaitToFrame(6);
			_anim.speed = 2.75f;
			base.transform.position += new Vector3(dir * LeaveOffset.x, LeaveOffset.y);
			yield return _anim.PlayToEnd();
			ToggleZemer(visible: false);
		}

		private IEnumerator RageCombo(float dir, bool special)
		{
			yield return Swing();
			IEnumerator FlyStrike()
			{
				float heroX = _target.transform.position.x;
				_ = (float)FaceHero();
				float x = ((heroX < MIDDLE) ? (heroX + 8f) : (heroX - 8f));
				base.transform.position = new Vector3(x, GroundY + 9.5f);
				Spring(isIn: true, base.transform.position, 1.4f);
				yield return new WaitForSeconds(0.16f);
				ToggleZemer(visible: true);
				heroX = _target.transform.position.x;
				float num = Mathf.Sign(_target.transform.position.x - base.transform.position.x);
				float num2 = FaceHero();
				heroX = ((num > 0f) ? Mathf.Max(heroX - 3f, LeftX + 0.2f) : ((!(num < 0f)) ? ((num2 > 0f) ? (heroX - 3f) : (heroX + 3f)) : Mathf.Min(heroX + 3f, RightX - 0.2f)));
				_anim.Play("Z4AirSweep");
				PlayAudioClip(ZemRandAudio.PickRandomZemAud(2, 8));
				yield return null;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 1);
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 3);
				PlayAudioClip("AudDash");
				Vector2 vector = new Vector2(x - heroX, base.transform.position.y - GroundY - 0.95f);
				float num3 = Mathf.Atan(vector.y / vector.x);
				num3 = ((Mathf.Sin(num3) > 0f) ? (num3 + (float)Math.PI) : num3);
				_rb.velocity = new Vector2(65f * Mathf.Cos(num3), 65f * Mathf.Sin(num3));
				yield return new WaitWhile(() => base.transform.position.y > GroundY + 3.5f && _anim.GetCurrentFrame() < 4 && CheckIfStuck());
				_anim.enabled = false;
				yield return new WaitWhile(() => base.transform.position.y > GroundY + 3.5f && CheckIfStuck());
				_anim.enabled = true;
				yield return new WaitWhile(() => (double)base.transform.position.y > (double)GroundY - 0.3 && _anim.GetCurrentFrame() < 6 && CheckIfStuck());
				_anim.enabled = false;
				yield return new WaitWhile(() => base.transform.position.y > GroundY - 0.3f && CheckIfStuck());
				_anim.enabled = true;
				base.transform.position = new Vector3(base.transform.position.x, GroundY - 0.3f);
				_rb.gravityScale = 0f;
				_rb.isKinematic = true;
				_anim.speed = 1f;
				_rb.velocity = new Vector2(Mathf.Sign(_rb.velocity.x) * 30f, 0f);
				PlayAudioClip("AudLand");
				yield return new WaitWhile(() => _anim.IsPlaying());
				_rb.velocity = Vector2.zero;
				yield return LandSlide();
			}
			IEnumerator LandSlide()
			{
				float dir2 = Mathf.Sign(base.transform.localScale.x);
				_anim.PlayAt("ZThrow2", 4);
				Spring(isIn: false, base.transform.position + new Vector3((0f - dir2) * LeaveOffset.x, 0f, 0f), 1.5f);
				base.transform.position += new Vector3((0f - dir2) * LeaveOffset.x, 0f);
				yield return _anim.PlayToEnd();
				ToggleZemer(visible: false);
				yield return new WaitForSeconds(0.07f);
				base.transform.position = ((dir2 > 0f) ? new Vector3(RightX - 1.5f, base.transform.position.y) : new Vector3(LeftX + 1.5f, base.transform.position.y));
				Spring(isIn: true, base.transform.position);
				yield return new WaitForSeconds(0.15f);
				ToggleZemer(visible: true);
				if (special)
				{
					float num = Mathf.Sign(base.gameObject.transform.GetPositionX() - MIDDLE);
					Vector3 localScale = base.gameObject.transform.localScale;
					base.gameObject.transform.localScale = new Vector3(Mathf.Abs(localScale.x) * num, localScale.y, 1f);
					_anim.enabled = true;
					_anim.Play("ZIdle");
					yield return DoSpiralPassage();
				}
				else
				{
					float signX = Mathf.Sign(base.gameObject.transform.GetPositionX() - MIDDLE);
					Vector3 localScale2 = base.gameObject.transform.localScale;
					base.gameObject.transform.localScale = new Vector3(Mathf.Abs(localScale2.x) * signX, localScale2.y, 1f);
					Log("Doing special dash");
					PlayAudioClip("AudDash");
					base.transform.Find("HyperCut").gameObject.SetActive(value: false);
					_anim.PlayAt("ZMultiDashAir", 1);
					_anim.enabled = true;
					_anim.speed = 0.5f;
					yield return null;
					float x = (0f - signX) * 75f;
					_rb.velocity = new Vector2(x, 0f);
					yield return new WaitWhile(() => !FastApproximately(_rb.velocity.x, 0f, 0.1f) && ((0f - signX <= 0f && !FastApproximately(base.transform.position.x, LeftX, 10f)) || (0f - signX > 0f && !FastApproximately(base.transform.position.x, RightX, 10f))));
					_anim.speed = 1f;
					_anim.PlayAt("ZDash", 14);
					_anim.enabled = true;
					_rb.velocity = Vector2.zero;
					yield return new WaitWhile(() => _anim.IsPlaying());
				}
			}
			IEnumerator Swing()
			{
				_lastAtt = null;
				yield return SwingTemp(dir, special ? 0.2f : 0.4f);
				yield return new WaitForSeconds(0.75f);
				yield return FlyStrike();
			}
		}

		private IEnumerator Dash()
		{
			_lastAtt = this.Dash;
			yield return FastApproximately(MIDDLE, base.transform.position.x, 8.5f) ? Dash() : RegularDash();
			IEnumerator Dash()
			{
				float dir = FaceHero();
				base.transform.Find("HyperCut").gameObject.SetActive(value: true);
				_anim.enabled = true;
				_anim.Play("ZDashOld", -1, 0f);
				base.transform.position = new Vector3(base.transform.position.x, GroundY - 0.3f, base.transform.position.z);
				yield return _anim.WaitToFrame(4);
				_anim.enabled = false;
				float totalDelay = 0.3f;
				StartCoroutine(FlashRepeat(base.transform.position, 0.15f));
				yield return new WaitForSeconds(totalDelay / 3f);
				StartCoroutine(FlashRepeat(base.transform.position, 0.1f));
				yield return new WaitForSeconds(totalDelay / 3f);
				StartCoroutine(FlashRepeat(base.transform.position, 0.05f));
				yield return new WaitForSeconds(totalDelay / 3f);
				PlayAudioClip("ZAudHoriz");
				_anim.enabled = true;
				yield return _anim.WaitToFrame(5);
				PlayAudioClip("AudDashIntro");
				yield return _anim.WaitToFrame(6);
				PlayAudioClip("AudDash");
				yield return _anim.WaitToFrame(7);
				Animator an = base.transform.Find("HorizSlashFX").GetComponent<Animator>();
				an.gameObject.SetActive(value: true);
				an.enabled = true;
				an.PlayAt("HorizFX", 0);
				an.speed = 2f;
				yield return _anim.WaitToFrame(9);
				Transform transform = base.transform.Find("HyperCut");
				GameObject gameObject = UnityEngine.Object.Instantiate(transform.Find("Hyper4R").gameObject);
				GameObject obj = UnityEngine.Object.Instantiate(transform.Find("Hyper4L").gameObject);
				Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
				Rigidbody2D component2 = obj.GetComponent<Rigidbody2D>();
				gameObject.SetActive(value: true);
				obj.SetActive(value: true);
				gameObject.transform.position = transform.Find("Hyper4R").position;
				obj.transform.position = transform.Find("Hyper4L").position;
				gameObject.transform.localScale = Product(transform.transform.localScale, new Vector3(dir, 1f, 1f));
				obj.transform.localScale = Product(transform.transform.localScale, new Vector3(dir, 1f, 1f));
				component2.velocity = new Vector2((0f - dir) * 50f, 0f);
				component.velocity = new Vector2(dir * 50f, 0f);
				yield return new WaitWhile(() => _anim.IsPlaying());
				an.enabled = false;
				an.gameObject.SetActive(value: false);
				_anim.Play("ZIdle");
				base.transform.position = new Vector3(base.transform.position.x, GroundY, base.transform.position.z);
			}
			IEnumerator RegularDash()
			{
				if (!IsFacingPlayer())
				{
					yield return Turn();
				}
				float dir = FaceHero();
				base.transform.Find("HyperCut").gameObject.SetActive(value: false);
				_anim.Play("ZDash", -1, 0f);
				base.transform.position = new Vector3(base.transform.position.x, GroundY + 0.3f, base.transform.position.z);
				int subOneFrameOnEdges = 0;
				if (base.transform.position.x < LeftX + 5f || base.transform.position.x > RightX - 5f)
				{
					subOneFrameOnEdges = 1;
					_anim.speed = 1.1f;
					_anim.Play("ZDash2", -1, 0f);
					_anim.speed = 1f;
				}
				else
				{
					yield return _anim.WaitToFrame(1);
					_anim.speed = 1.1f;
					yield return _anim.WaitToFrame(2);
					PlayAudioClip("Zem_Backdash");
					_rb.velocity = new Vector2(dir * 35f, 0f);
					yield return _anim.WaitToFrame(4);
					_anim.speed = 1f;
					_rb.velocity = Vector2.zero;
				}
				yield return _anim.WaitToFrame(7 - subOneFrameOnEdges);
				PlayAudioClip("ZemBladeShine");
				yield return _anim.WaitToFrame(11 - subOneFrameOnEdges);
				PlayAudioClip("AudDashIntro");
				yield return _anim.WaitToFrame(13 - subOneFrameOnEdges);
				PlayAudioClip("ZAudHoriz");
				PlayAudioClip("AudDash");
				_anim.speed = 1.5f;
				_rb.velocity = new Vector2((0f - dir) * 75f, 0f);
				yield return _anim.WaitToFrame(14 - subOneFrameOnEdges);
				base.transform.position = new Vector3(base.transform.position.x, GroundY - 0.5f, base.transform.position.z);
				_anim.enabled = false;
				_anim.speed = 1f;
				yield return new WaitForSeconds(0.15f);
				_anim.enabled = true;
				yield return _anim.WaitToFrame(16 - subOneFrameOnEdges);
				_rb.velocity = Vector2.zero;
				yield return new WaitWhile(() => _anim.IsPlaying());
				_anim.Play("ZIdle");
				base.transform.position = new Vector3(base.transform.position.x, GroundY, base.transform.position.z);
			}
		}

		private IEnumerator Dodge()
		{
			_lastAtt = this.Dodge;
			yield return Dodge();
			IEnumerator Dodge()
			{
				if (!IsFacingPlayer())
				{
					yield return Turn();
				}
				float num = (float)FaceHero() * -1f;
				_anim.Play("ZDodge", -1, 0f);
				_rb.velocity = new Vector2((0f - num) * 40f, 0f);
				yield return null;
				yield return new WaitWhile(() => _anim.IsPlaying());
				_rb.velocity = new Vector2(0f, 0f);
				_anim.Play("ZIdle");
			}
		}

		private void ZemerCounter()
		{
			float dir = 0f;
			_lastAtt = null;
			_counterRoutine = StartCoroutine(CounterAntic());
			IEnumerator CounterAntic()
			{
				if (!IsFacingPlayer())
				{
					yield return Turn();
				}
				dir = (float)FaceHero() * -1f;
				_anim.Play("ZCInit");
				PlayAudioClip("ZAudCounter");
				yield return new WaitWhile(() => _anim.IsPlaying());
				_counterRoutine = StartCoroutine(Countering());
			}
			IEnumerator CounterEnd()
			{
				_hm.IsInvincible = false;
				_anim.Play("ZCCancel");
				yield return new WaitWhile(() => _anim.IsPlaying());
				_countering = false;
			}
			IEnumerator Countering()
			{
				Parryable.ParryFlag = true;
				_hm.IsInvincible = true;
				_anim.Play("ZCIdle");
				_blockedHit = false;
				On.HealthManager.Hit -= OnBlockedHit;
				On.HealthManager.Hit += OnBlockedHit;
				PlayAudioClip("Counter");
				Vector2 vector = base.transform.position + new Vector3(1.7f * dir, 0.8f, -0.1f);
				Quaternion rotation = Quaternion.Euler(0f, 0f, dir * 80f);
				UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["CounterFX"], vector, rotation).SetActive(value: true);
				yield return new WaitForSeconds(0.42f);
				On.HealthManager.Hit -= OnBlockedHit;
				Parryable.ParryFlag = false;
				_counterRoutine = StartCoroutine(CounterEnd());
			}
		}

		private IEnumerator SpinAttack()
		{
			_lastAtt = this.SpinAttack;
			yield return SpinAttack();
			IEnumerator SpinAttack()
			{
				if (!IsFacingPlayer())
				{
					yield return Turn();
				}
				float num = (float)FaceHero() * -1f;
				float num2 = Mathf.Abs(_target.transform.GetPositionX() - base.transform.GetPositionX());
				float num3 = Mathf.Abs(_target.transform.GetPositionY() - base.transform.GetPositionY());
				float rot = Mathf.Atan(num3 / num2);
				rot = ((num < 0f) ? ((float)Math.PI - rot) : rot);
				_anim.Play("ZSpin");
				PlayAudioClip(ZemRandAudio.PickRandomZemAud(2, 8));
				yield return null;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 2);
				PlayAudioClip("AudDashIntro");
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 3);
				_rb.velocity = new Vector2(60f * Mathf.Cos(rot), 60f * Mathf.Sin(rot));
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 4);
				_anim.enabled = false;
				yield return new WaitWhile(() => base.transform.position.x > LeftX + 4f && base.transform.position.x < RightX - 4f);
				_anim.enabled = true;
				_rb.velocity = Vector2.zero;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 8);
				PlayAudioClip("AudBigSlash2");
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 11);
				_rb.isKinematic = false;
				_rb.gravityScale = 1.5f;
				yield return new WaitWhile(() => base.transform.position.y > GroundY);
				PlayAudioClip("AudLand");
				_rb.velocity = Vector2.zero;
				_rb.gravityScale = 0f;
				_rb.isKinematic = true;
				base.transform.position = new Vector3(base.transform.position.x, GroundY);
				yield return new WaitWhile(() => _anim.IsPlaying());
				_anim.Play("ZIdle");
			}
		}

		private IEnumerator FancyAttack()
		{
			_lastAtt = this.FancyAttack;
			yield return FancyAttack();
			IEnumerator FancyAttack()
			{
				if (!IsFacingPlayer())
				{
					yield return Turn();
				}
				_ = (float)FaceHero();
				_anim.Play("ZAtt2");
				PlayAudioClip(ZemRandAudio.PickRandomZemAud(9, 13));
				yield return null;
				PlayAudioClip("AudBasicSlash1");
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 2);
				PlayAudioClip("AudBasicSlash2");
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 6);
				PlayAudioClip("AudBigSlash2");
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 7);
				float dir = FaceHero();
				SpawnPillar(dir, Vector2.one, 15f);
				yield return new WaitWhile(() => _anim.IsPlaying());
				_anim.Play("ZIdle");
			}
		}

		private IEnumerator MusicControl()
		{
			if (OWArenaFinder.IsInOverWorld)
			{
				OWBossManager.PlayMusic(null);
				OWBossManager.PlayMusic(FiveKnights.Clips["ZP2Intro"]);
				yield return new WaitForSecondsRealtime(14.12f);
				OWBossManager.PlayMusic(FiveKnights.Clips["ZP2Loop"]);
			}
			else
			{
				GGBossManager.Instance.PlayMusic(null);
				GGBossManager.Instance.PlayMusic(FiveKnights.Clips["ZP2Intro"], 1f);
				yield return new WaitForSecondsRealtime(14.12f);
				GGBossManager.Instance.PlayMusic(FiveKnights.Clips["ZP2Loop"], 1f);
			}
		}

		private IEnumerator EndPhase1(bool firstDeath)
		{
			float dir;
			yield return KnockedOut();
			IEnumerator KnockedOut()
			{
				_isKnockingOut = true;
				float num = Math.Sign(base.transform.position.x - HeroController.instance.transform.position.x);
				dir = -FaceHero();
				_rb.gravityScale = 1.5f;
				_rb.velocity = new Vector2(num * 15f, 20f);
				PlayDeathFor(base.gameObject);
				_anim.enabled = true;
				_anim.Play("ZKnocked");
				if (DoPhase)
				{
					PlayAudioClip("ZAudP1DeathD");
					StartCoroutine(PlayDeathSound(FiveKnights.Clips["ZAudP1DeathD"].length));
				}
				else
				{
					string[] array = new string[3] { "ZAudP1DeathA", "ZAudP1DeathB", "ZAudP1DeathC" };
					string clipName = array[_rand.Next(array.Length)];
					PlayAudioClip(clipName);
				}
				_anim.speed = 1f;
				yield return null;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 1);
				_anim.enabled = false;
				yield return new WaitWhile(() => base.transform.position.y > GroundY - 0.99f);
				_rb.velocity = Vector2.zero;
				_anim.enabled = true;
				_rb.gravityScale = 0f;
				base.transform.position = new Vector3(base.transform.position.x, GroundY - deathGndOffset);
				yield return new WaitWhile(() => _anim.IsPlaying());
				_isKnockingOut = false;
				if (DoPhase)
				{
					_anim.enabled = false;
					FiveKnights.journalEntries["Zemer"].RecordJournalEntry();
					yield return new WaitForSeconds(1.75f);
					CustomWP.wonLastFight = true;
					UnityEngine.Object.Destroy(this);
				}
				else
				{
					_anim.enabled = false;
					_bc.enabled = true;
					_rb.isKinematic = true;
					isHit = false;
					float seconds = (firstDeath ? 1.5f : 1.2f);
					yield return new WaitSecWhile(() => !isHit, seconds);
					_anim.enabled = true;
					yield return Recover(firstDeath);
				}
			}
			IEnumerator PlayDeathSound(float waitLen)
			{
				yield return new WaitForSeconds(waitLen);
				PlayAudioClip("ZAudP1Death2");
			}
			IEnumerator Recover(bool flag = false)
			{
				PlayAudioClip("ZAudMid");
				yield return _anim.PlayBlocking("ZRecover");
				float delay = (flag ? 3.5f : 0.75f);
				yield return LeaveTemp(dir, delay, flag);
				if (flag)
				{
					StartCoroutine(MusicControl());
				}
				yield return TransitionIn();
			}
			IEnumerator TransitionIn()
			{
				float heroX = _target.transform.GetPositionX();
				float x = ((heroX < MIDDLE) ? (heroX + (float)_rand.Next(8, 11)) : (heroX - (float)_rand.Next(8, 11)));
				float xOff = ((heroX < MIDDLE) ? (-4f) : 4f);
				base.transform.position = new Vector3(x, GroundY + 6f);
				dir = FaceHero();
				Spring(isIn: true, base.transform.position, 1.8f);
				yield return new WaitForSeconds(0.15f);
				ToggleZemer(visible: true);
				Vector2 vector = new Vector2(x - heroX + xOff, base.transform.position.y - GroundY - 0.95f);
				float num = Mathf.Atan(vector.y / vector.x);
				num = ((heroX < MIDDLE) ? (num + (float)Math.PI) : num);
				_rb.velocity = new Vector2(55f * Mathf.Cos(num), 55f * Mathf.Sin(num));
				_anim.Play("Z1ZipIn");
				PlayAudioClip("ZAudAtt14");
				yield return null;
				yield return new WaitWhile(() => base.transform.position.y > GroundY - 0.95f);
				base.transform.position = new Vector3(base.transform.position.x, GroundY - 0.95f);
				_rb.velocity = new Vector2((0f - dir) * 40f, 0f);
				_anim.Play("Z2Crawl");
				yield return null;
				yield return new WaitForSeconds(0.08f);
				_rb.velocity = new Vector2(0f, 0f);
				yield return new WaitWhile(() => _anim.IsPlaying());
				float dir2 = Mathf.Sign(base.transform.localScale.x);
				yield return RageCombo(dir2, special: true);
				StartCoroutine(Attacks());
			}
		}

		private IEnumerator FlowerBloomer()
		{
			GGBossManager.Instance.PlayFlowers(2);
			yield return new WaitForSeconds(0.2f);
			GameObject whiteflashOld = FiveKnights.preloadedGO["WhiteFlashZem"];
			GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
			foreach (Animator item in GGBossManager.Instance.flowersGlow)
			{
				item.gameObject.SetActive(value: true);
				item.enabled = true;
				item.Play("Glow", -1, 0f);
			}
			yield return null;
			foreach (Animator item2 in GGBossManager.Instance.flowersGlow)
			{
				yield return item2.WaitToFrame(2);
			}
			foreach (Animator item3 in GGBossManager.Instance.flowersGlow)
			{
				item3.enabled = false;
			}
			for (int i = 0; i < 5; i++)
			{
				GameObject obj = UnityEngine.Object.Instantiate(whiteflashOld);
				obj.SetActive(value: true);
				obj.transform.position = _target.transform.position;
			}
			yield return new WaitForSeconds(0.2f);
			foreach (Animator item4 in GGBossManager.Instance.flowersGlow)
			{
				item4.enabled = true;
			}
		}

		private IEnumerator LeaveTemp(float dir, float delay, bool firstDeath = false)
		{
			_anim.PlayAt("ZThrow2B", 0);
			yield return _anim.WaitToFrame(5);
			base.transform.position += new Vector3(dir * LeaveOffset.x / 2f, LeaveOffset.y / 2f);
			Spring(isIn: false, base.transform.position + new Vector3(dir * LeaveOffset.x, LeaveOffset.y, 0f), 1.5f);
			if (firstDeath && CustomWP.boss == CustomWP.Boss.All)
			{
				StartCoroutine(FlowerBloomer());
			}
			yield return _anim.PlayToEnd();
			ToggleZemer(visible: false);
			yield return new WaitForSeconds(delay);
		}

		private void SpawnPillar(float dir, Vector2 size, float xSpd)
		{
			GameObject gameObject = traitorSlam[traitorSlamIndex++ % 2];
			Animator component = gameObject.transform.Find("slash_core").GetComponent<Animator>();
			gameObject.SetActive(value: true);
			component.enabled = true;
			component.Play("mega_mantis_slash_big", -1, 0f);
			PlayAudioClip("TraitorPillar");
			gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2((0f - dir) * xSpd, 0f);
			Vector3 position = base.transform.position;
			gameObject.transform.position = new Vector3((0f - dir) * 2.15f + position.x, GroundY - 3.2f, 6.4f);
			gameObject.transform.localScale = new Vector3((0f - dir) * size.x, size.y, 1f);
			if (gameObject.transform.Find("slash_core").Find("hurtbox") != null)
			{
				gameObject.transform.Find("slash_core").Find("hurtbox").gameObject.SetActive(value: false);
			}
			Vector2 offset = gameObject.transform.Find("slash_core").Find("Test2").GetComponent<PolygonCollider2D>()
				.offset;
			offset.y = gameObject.transform.Find("slash_core").Find("hurtbox").GetComponent<PolygonCollider2D>()
				.offset.y - 10f;
		}

		private IEnumerator DoSpiralPassage()
		{
			grass.transform.parent = base.transform;
			GameObject ciel = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Ceiling Dust"].gameObject);
			ciel.SetActive(value: false);
			bool continueWithSpiral;
			yield return PassageAcrossArena(3);
			yield return DoOneLastSpiral();
			static void ActivateSpiral(GameObject spiral, float spd)
			{
				spiral.SetActive(value: true);
				Animator component = spiral.GetComponent<Animator>();
				component.speed = spd;
				component.Rebind();
				component.Update(0f);
				Animator[] componentsInChildren = spiral.GetComponentsInChildren<Animator>(includeInactive: true);
				foreach (Animator obj in componentsInChildren)
				{
					obj.Rebind();
					obj.Update(0f);
					obj.speed = spd;
				}
			}
			IEnumerator DashWithSpiral(float dir, GameObject controller)
			{
				_anim.speed = 1f;
				for (int i = 0; i < 2; i++)
				{
					if (i == 1)
					{
						PlayAudioClip("ZAudAtt16");
					}
					float signX = ((i == 0) ? dir : Mathf.Sign(base.gameObject.transform.GetPositionX() - MIDDLE));
					float destination = ((0f - signX < 0f) ? (LeftX + 0.8f * (MIDDLE - LeftX)) : (RightX - 0.8f * (RightX - MIDDLE)));
					Vector3 localScale = base.gameObject.transform.localScale;
					base.gameObject.transform.localScale = new Vector3(Mathf.Abs(localScale.x) * signX, localScale.y, 1f);
					_anim.PlayAt("ZAerial2", 0);
					_anim.speed = 1f;
					yield return null;
					yield return _anim.WaitToFrame(4);
					_anim.enabled = false;
					yield return FlashRepeat(new Vector3((0f - signX < 0f) ? (LeftX + 1f) : (RightX - 1f), GroundY + 1f), 0.15f);
					Vector2 p2 = default(Vector2);
					Vector2 p3 = default(Vector2);
					Vector3 tarPos = new Vector3(_target.transform.position.x, GroundY);
					if (i == 1)
					{
						if (dir > 0f)
						{
							p2 = new Vector2(RightX - (RightX - LeftX) * 0.49f, GroundY + 16.2f);
							p3 = new Vector2(LeftX + (RightX - LeftX) * 0.15f, GroundY + 8.2f);
						}
						else
						{
							p2 = new Vector2(LeftX + (RightX - LeftX) * 0.49f, GroundY + 16.2f);
							p3 = new Vector2(RightX - (RightX - LeftX) * 0.15f, GroundY + 8.2f);
						}
						StartCoroutine(DelayedFlash());
					}
					_anim.enabled = true;
					_anim.PlayAt("Z5LandSlide", 1);
					PlayAudioClip("AudDash");
					float num = 60f;
					_rb.velocity = new Vector2((0f - signX) * num, 0f);
					grass.GetComponent<ParticleSystem>().Play();
					yield return null;
					_anim.enabled = false;
					yield return (0f - signX > 0f) ? new WaitWhile(() => base.transform.position.x < destination) : new WaitWhile(() => base.transform.position.x > destination);
					_anim.enabled = true;
					_anim.PlayAt("Z4AirSweep", 6);
					yield return new WaitForSeconds(0.05f);
					_anim.PlayAt("ZSpin", 4);
					_rb.velocity = new Vector2(_rb.velocity.x, 50f);
					yield return new WaitForSeconds(0.1f);
					_anim.enabled = false;
					yield return WaitByVelocity(0.3f);
					ciel.SetActive(value: true);
					ciel.GetComponent<ParticleSystem>().Play();
					ciel.transform.position = new Vector3(MIDDLE, 39.5f);
					PlayAudioClip((i % 2 == 0) ? "breakable_wall_hit_1" : "breakable_wall_hit_2");
					GameCameras.instance.cameraShakeFSM.SendEvent("BigShake");
					grass.GetComponent<ParticleSystem>().Stop();
					if (i == 1)
					{
						yield return DontStop(controller, p2, p3, tarPos);
						break;
					}
					_anim.PlayAt("ZSpin", 14);
					_anim.enabled = true;
					_rb.isKinematic = false;
					_rb.gravityScale = 2f;
					_rb.velocity = new Vector2(signX * 7f, 0f);
					yield return new WaitWhile(() => _anim.IsPlaying());
					yield return new WaitWhile(() => base.transform.position.y > GroundY - 0.3f);
					base.transform.position = new Vector3(base.transform.position.x, GroundY - 0.95f);
					_rb.gravityScale = 0f;
					_rb.isKinematic = true;
					_rb.velocity = Vector2.zero;
					_anim.PlayAt("Z5LandSlide", 3);
					yield return null;
					yield return new WaitWhile(() => _anim.IsPlaying());
					IEnumerator DelayedFlash()
					{
						yield return FlashRepeat(p2 - new Vector2(0f, 6f), 0.1f);
						tarPos = new Vector3(MIDDLE, GroundY);
						yield return FlashRepeat(p3, 0.1f);
						yield return FlashRepeat(tarPos, 0.1f);
					}
				}
			}
			IEnumerator Disperse(GameObject controller)
			{
				if (!IsFacingPlayer())
				{
					yield return Turn();
				}
				_ = (float)FaceHero();
				_anim.Play("ZAtt2");
				PlayAudioClip(ZemRandAudio.PickRandomZemAud(9, 13));
				_anim.speed = 2f;
				yield return null;
				PlayAudioClip("AudBasicSlash1");
				StartCoroutine(LerpSizeChange(controller.transform, 0.2f, 0.5f));
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 2);
				PlayAudioClip("AudBasicSlash2");
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 4);
				StartCoroutine(LerpSizeChange(controller.transform, 0.1f, 2.2f));
				GameObject original = FiveKnights.preloadedGO["HornetSphere"].transform.Find("Flash Effect").gameObject;
				float[] array = new float[4] { 0f, 90f, 180f, 270f };
				foreach (float num2 in array)
				{
					GameObject obj = UnityEngine.Object.Instantiate(original);
					obj.transform.SetRotationZ(num2 + (float)UnityEngine.Random.Range(10, 30));
					obj.transform.position = controller.transform.position;
					obj.transform.parent = controller.transform;
					obj.SetActive(value: true);
					PlayMakerFSM playMakerFSM = obj.LocateMyFSM("FSM");
					playMakerFSM.enabled = true;
					playMakerFSM.FsmVariables.FindFsmFloat("Pause").Value = 1f;
					playMakerFSM.FsmVariables.FindFsmFloat("Rotation").Value = num2 + (float)UnityEngine.Random.Range(10, 30);
					playMakerFSM.FsmVariables.FindFsmBool("Reset Rotation").Value = false;
					playMakerFSM.SetState("Init");
				}
				GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 6);
				UnityEngine.Object.Destroy(controller);
				PlayAudioClip("AudBigSlash2");
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 7);
				float dir = FaceHero();
				SpawnPillar(dir, Vector2.one, 30f);
				yield return new WaitWhile(() => _anim.IsPlaying());
				_anim.Play("ZIdle");
				_anim.speed = 1f;
			}
			IEnumerator DoOneLastSpiral()
			{
				_anim.Play("Z6LaserSpin", -1, 0f);
				_anim.speed = 1.75f;
				yield return null;
				_anim.enabled = false;
				yield return new WaitForSeconds(0.1f);
				PlayAudioClip(ZemRandAudio.PickRandomZemAud(2, 8));
				_anim.enabled = true;
				_rb.velocity = Vector2.zero;
				base.transform.position = new Vector3(base.transform.position.x, GroundY - 1.2f);
				float dir = FaceHero(onlyCalc: false, opposite: true, MIDDLE);
				yield return null;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 1);
				PlayAudioClip("AudBasicSlash1");
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 2);
				_rb.velocity = new Vector2(dir * 60f, 0f);
				_anim.speed = 1.75f;
				grass.GetComponent<ParticleSystem>().Play();
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 3);
				PlayAudioClip("AudDash");
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 8);
				_anim.enabled = false;
				_anim.speed = 1f;
				yield return WaitByVelocity(4f);
				grass.GetComponent<ParticleSystem>().Stop();
				_rb.velocity = new Vector2(dir * 50f, 0f);
				_anim.enabled = true;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 9);
				StartCoroutine(Finish());
				yield return FlashAppear(new Vector2(MIDDLE, GroundY + 2.5f));
				IEnumerator Finish()
				{
					yield return new WaitForSeconds(0.1f);
					yield return _anim.PlayToEnd();
					_anim.PlayAt("ZThrow2", 4);
					Vector3 localScale = base.gameObject.transform.localScale;
					base.gameObject.transform.localScale = new Vector3(0f - localScale.x, localScale.y, 1f);
					Spring(isIn: false, base.transform.position + new Vector3(dir * LeaveOffset.x, 0f, 0f), 1.5f);
					base.transform.position += new Vector3(dir * LeaveOffset.x, 0f);
					yield return _anim.PlayToEnd();
					ToggleZemer(visible: false);
				}
			}
			IEnumerator DontStop(GameObject controller, Vector2 p2, Vector2 p3, Vector3 tarPos)
			{
				FaceHero(onlyCalc: false, opposite: false, MIDDLE);
				_anim.PlayAt("ZSpin", 3);
				_anim.enabled = true;
				yield return null;
				Vector2 p4 = base.transform.position;
				float timePass = 0f;
				float duration = 0.5f;
				while (timePass < duration)
				{
					base.transform.position = QuadraticBezierInterp(p4, p2, p3, timePass / duration);
					timePass += Time.deltaTime;
					if (timePass / duration > 0.55f && _anim.GetCurrentFrame() >= 4)
					{
						_anim.enabled = false;
					}
					yield return null;
				}
				base.transform.position = p3;
				_anim.enabled = true;
				_rb.velocity = Vector2.zero;
				yield return LandToPlayer(controller, tarPos);
			}
			IEnumerator Falling()
			{
				_rb.isKinematic = false;
				_rb.gravityScale = 0.3f;
				yield return new WaitWhile(() => base.transform.position.y > GroundY + 1.8f);
				_anim.enabled = true;
				yield return new WaitWhile(() => base.transform.position.y > GroundY - 0.3f);
				base.transform.position = new Vector3(base.transform.position.x, GroundY - 0.95f);
				_rb.gravityScale = 0f;
				_rb.isKinematic = true;
				_rb.velocity = Vector2.zero;
				yield return _anim.PlayToEnd();
				_anim.Play("ZIdle", -1, 0f);
				yield return new WaitForSeconds(0.5f);
			}
			IEnumerator FlashAppear(Vector2 targ)
			{
				PlayAudioClip("ZAudExplode");
				GameObject original = FiveKnights.preloadedGO["Heart"].transform.Find("Appear Trail").gameObject;
				GameObject whiteflashOld = FiveKnights.preloadedGO["WhiteFlashZem"];
				GameObject original2 = FiveKnights.preloadedGO["HornetSphere"].transform.Find("Flash Effect").gameObject;
				float[] array = new float[4] { 0f, 90f, 180f, 270f };
				foreach (float num in array)
				{
					GameObject obj = UnityEngine.Object.Instantiate(original2);
					obj.transform.SetRotationZ(num + (float)UnityEngine.Random.Range(10, 30));
					obj.transform.position = base.transform.position;
					obj.transform.parent = base.transform;
					obj.SetActive(value: true);
					PlayMakerFSM playMakerFSM = obj.LocateMyFSM("FSM");
					playMakerFSM.enabled = true;
					playMakerFSM.FsmVariables.FindFsmFloat("Pause").Value = 1f;
					playMakerFSM.FsmVariables.FindFsmFloat("Rotation").Value = num + (float)UnityEngine.Random.Range(10, 30);
					playMakerFSM.FsmVariables.FindFsmBool("Reset Rotation").Value = false;
					playMakerFSM.SetState("Init");
				}
				GameObject startCircleNew = UnityEngine.Object.Instantiate(original);
				startCircleNew.SetActive(value: true);
				startCircleNew.transform.position = targ;
				startCircleNew.transform.localScale *= 3f;
				startCircleNew.GetComponent<ParticleSystem>().Play();
				yield return new WaitForSeconds(0.5f);
				yield return FlashRepeat(targ);
				yield return new WaitForSeconds(0.1f);
				UnityEngine.Object.Destroy(startCircleNew);
				GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
				PlayAudioClip("ZAudAtt15");
				for (int j = 0; j < 4; j++)
				{
					GameObject obj2 = UnityEngine.Object.Instantiate(whiteflashOld);
					obj2.SetActive(value: true);
					obj2.transform.position = targ;
				}
				yield return new WaitForSeconds(0.1f);
				StartCoroutine(SpawnSpirals(targ - new Vector2(0f, 2f), 0.9f, skipBallCircle: true));
				GameObject controller = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["SlashRingController"]);
				_destroyAtEnd.Add(controller);
				controller.transform.localScale *= 0.5f;
				controller.SetActive(value: true);
				StartCoroutine(PlayExtendedSpiral(controller, 1.3f));
				_ = (float)FaceHero();
				base.transform.position = new Vector3(targ.x, GroundY + 2f);
				ToggleZemer(visible: true);
				_anim.enabled = true;
				_anim.PlayAt("ZSpin", 15);
				yield return null;
				_anim.enabled = false;
				yield return new WaitForSeconds(0.2f);
				float dir = FaceHero();
				yield return Falling();
				yield return DashWithSpiral(dir, controller);
			}
			IEnumerator FollowZemer(Transform spiral)
			{
				while (spiral != null)
				{
					spiral.position = base.transform.position;
					yield return null;
				}
			}
			IEnumerator LandToPlayer(GameObject controller, Vector2 tarPos)
			{
				int side = 1;
				FaceHero();
				Vector2 p3 = base.transform.position;
				_anim.PlayAt("Z4AirSweep", 1);
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 3);
				PlayAudioClip("AudDash");
				_anim.speed = 2f;
				Vector2 vector = p3 - tarPos;
				float num = Mathf.Atan(vector.y / vector.x);
				if (side > 0 && p3.x > tarPos.x)
				{
					num += (float)Math.PI;
				}
				_rb.velocity = new Vector2(70f * Mathf.Cos(num), 70f * Mathf.Sin(num));
				float velDir = Mathf.Sign(_rb.velocity.x);
				yield return new WaitWhile(() => base.transform.position.y > GroundY + 3.5f && _anim.GetCurrentFrame() < 4 && CheckIfStuck());
				_anim.enabled = false;
				yield return new WaitWhile(() => base.transform.position.y > GroundY + 3.5f && CheckIfStuck());
				_anim.enabled = true;
				yield return new WaitWhile(() => (double)base.transform.position.y > (double)GroundY - 0.3 && _anim.GetCurrentFrame() < 6 && CheckIfStuck());
				_anim.enabled = false;
				yield return new WaitWhile(() => base.transform.position.y > GroundY - 0.3f && CheckIfStuck());
				_anim.enabled = true;
				base.transform.position = new Vector3(base.transform.position.x, GroundY - 0.3f);
				_rb.gravityScale = 0f;
				_rb.isKinematic = true;
				_anim.speed = 1f;
				ciel.SetActive(value: true);
				ciel.GetComponent<ParticleSystem>().Play();
				ciel.transform.position = new Vector3(MIDDLE, 39.5f);
				PlayAudioClip("breakable_wall_hit_1");
				GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
				_rb.velocity = new Vector2(velDir * 60f, 0f);
				yield return new WaitWhile(() => _anim.IsPlaying());
				_rb.velocity = Vector2.zero;
				yield return new WaitForSeconds(0.1f);
				yield return Disperse(controller);
			}
			static IEnumerator LerpScale(Transform trans, float scale)
			{
				float lerpDuration = 0.15f;
				Vector2 startValue = trans.localScale;
				Vector2 endValue = trans.localScale * scale;
				float timeElapsed = 0f;
				while (timeElapsed < lerpDuration)
				{
					trans.localScale = Vector2.Lerp(startValue, endValue, timeElapsed / lerpDuration);
					timeElapsed += Time.deltaTime;
					yield return null;
				}
				trans.localScale = endValue;
			}
			static IEnumerator LerpScale2(Transform trans)
			{
				float lerpDuration = 0.1f;
				Vector2 startValue = trans.localScale;
				Vector2 endValue = trans.localScale * 0.7f;
				float timeElapsed = 0f;
				while (timeElapsed < lerpDuration)
				{
					trans.localScale = Vector2.Lerp(startValue, endValue, timeElapsed / lerpDuration);
					timeElapsed += Time.deltaTime;
					yield return null;
				}
				trans.localScale = endValue;
			}
			static IEnumerator LerpScale3(Transform trans, float scale)
			{
				float lerpDuration = 0.4f;
				Vector2 startValue = trans.localScale;
				Vector2 endValue = trans.localScale * scale;
				float timeElapsed = 0f;
				while (timeElapsed < lerpDuration)
				{
					trans.localScale = Vector2.Lerp(startValue, endValue, timeElapsed / lerpDuration);
					timeElapsed += Time.deltaTime;
					yield return null;
				}
				trans.localScale = endValue;
			}
			static IEnumerator LerpSizeChange(Transform trans, float dur, float scale)
			{
				Vector2 startValue = trans.localScale;
				Vector2 endValue = trans.localScale * scale;
				float timeElapsed = 0f;
				while (timeElapsed < dur)
				{
					if (trans == null)
					{
						yield break;
					}
					trans.localScale = Vector2.Lerp(startValue, endValue, timeElapsed / dur);
					timeElapsed += Time.deltaTime;
					yield return null;
				}
				trans.localScale = endValue;
			}
			static Vector2 LineLerp(Vector2 p1, Vector2 p2, float t)
			{
				float x = Mathf.SmoothStep(p1.x, p2.x, t);
				float y = Mathf.SmoothStep(p1.y, p2.y, t);
				return new Vector2(x, y);
			}
			IEnumerator PassageAcrossArena(int numTimes)
			{
				while (numTimes > 0)
				{
					numTimes--;
					_anim.Play("Z6LaserSpin", -1, 0f);
					PlayAudioClip(ZemRandAudio.PickRandomZemAud(2, 8));
					_anim.speed = 1.75f;
					yield return null;
					_anim.enabled = false;
					yield return new WaitForSeconds(0.1f);
					_anim.enabled = true;
					_rb.velocity = Vector2.zero;
					base.transform.position = new Vector3(base.transform.position.x, GroundY - 1.2f);
					float dir = FaceHero(onlyCalc: false, opposite: true, MIDDLE);
					yield return null;
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 1);
					PlayAudioClip("AudBasicSlash1");
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 2);
					_rb.velocity = new Vector2(dir * 60f, 0f);
					_anim.speed = 1.75f;
					grass.GetComponent<ParticleSystem>().Play();
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 3);
					PlayAudioClip("AudDash");
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 8);
					_anim.enabled = false;
					_anim.speed = 1f;
					yield return WaitByVelocity(4f);
					grass.GetComponent<ParticleSystem>().Stop();
					_rb.velocity = new Vector2(dir * 50f, 0f);
					_anim.enabled = true;
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 9);
					continueWithSpiral = false;
					StartCoroutine(SpawnSpirals(_target.transform.position, 1.1f));
					yield return new WaitForSeconds(0.1f);
					yield return _anim.PlayToEnd();
					_anim.PlayAt("ZThrow2", 4);
					Vector3 localScale = base.gameObject.transform.localScale;
					base.gameObject.transform.localScale = new Vector3(0f - localScale.x, localScale.y, 1f);
					Spring(isIn: false, base.transform.position + new Vector3(dir * LeaveOffset.x, 0f, 0f), 1.5f);
					base.transform.position += new Vector3(dir * LeaveOffset.x, 0f);
					yield return _anim.PlayToEnd();
					ToggleZemer(visible: false);
					yield return new WaitWhile(() => !continueWithSpiral);
					base.transform.position = ((dir > 0f) ? new Vector3(RightX - 1.5f, base.transform.position.y) : new Vector3(LeftX + 1.5f, base.transform.position.y));
					Spring(isIn: true, base.transform.position);
					yield return new WaitForSeconds(0.15f);
					ToggleZemer(visible: true);
					_anim.enabled = true;
				}
			}
			IEnumerator PlayExtendedSpiral(GameObject controller, float spd)
			{
				System.Random rnd = new System.Random();
				int[] array = new int[5] { 0, 1, 2, 3, 4 }.OrderBy((int x) => rnd.Next()).ToArray();
				PlayAudioClip("NeedleSphere", 0.3f);
				int[] array2 = array;
				foreach (int num2 in array2)
				{
					USceneUtil.Find(controller, $"SlashRing{num2}").gameObject.SetActive(value: false);
				}
				GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
				controller.transform.localScale /= 2.5f;
				StartCoroutine(LerpScale3(controller.transform, 2.5f));
				GameObject original = FiveKnights.preloadedGO["HornetSphere"].transform.Find("Flash Effect").gameObject;
				float[] array3 = new float[4] { 0f, 90f, 180f, 270f };
				foreach (float num3 in array3)
				{
					GameObject obj = UnityEngine.Object.Instantiate(original);
					obj.transform.SetRotationZ(num3 + (float)UnityEngine.Random.Range(10, 30));
					obj.transform.position = base.transform.position;
					obj.transform.parent = base.transform;
					obj.SetActive(value: true);
					PlayMakerFSM playMakerFSM = obj.LocateMyFSM("FSM");
					playMakerFSM.enabled = true;
					playMakerFSM.FsmVariables.FindFsmFloat("Pause").Value = 1f;
					playMakerFSM.FsmVariables.FindFsmFloat("Rotation").Value = num3 + (float)UnityEngine.Random.Range(10, 30);
					playMakerFSM.FsmVariables.FindFsmBool("Reset Rotation").Value = false;
					playMakerFSM.SetState("Init");
				}
				int[] array4 = array;
				foreach (int num5 in array4)
				{
					GameObject gameObject = USceneUtil.Find(controller, $"SlashRing{num5}").gameObject;
					gameObject.SetActive(value: true);
					Animator component = gameObject.transform.Find("1").GetComponent<Animator>();
					Animator component2 = gameObject.transform.Find("2").GetComponent<Animator>();
					StartCoroutine(ActivateRing(gameObject, component, component2));
					StartCoroutine(FollowZemer(controller.transform));
					yield return new WaitForSeconds((float)rnd.Next(15, 30) * 0.01f);
				}
				IEnumerator ActivateRing(GameObject ring, Animator a1, Animator a2)
				{
					ring.SetActive(value: true);
					a1.enabled = true;
					a1.gameObject.SetActive(value: true);
					a1.Play("NewSlash3Antic", -1, 0f);
					a1.speed = spd;
					a2.enabled = false;
					a2.gameObject.SetActive(value: false);
					yield return null;
					yield return new WaitWhile(() => a1.GetCurrentFrame() < 4);
					StartCoroutine(LoopAnimation(a1, "NewSlash3Loop"));
					a2.enabled = true;
					a2.speed = spd;
					a2.gameObject.SetActive(value: true);
					a2.Play("NewSlash3Antic", -1, 0f);
					yield return null;
					yield return new WaitWhile(() => a2.GetCurrentFrame() < 4);
					StartCoroutine(LoopAnimation(a2, "NewSlash3Loop"));
				}
				IEnumerator LoopAnimation(Animator anim, string name)
				{
					while (anim != null)
					{
						anim.Play(name, -1, 0f);
						anim.speed = spd;
						yield return null;
						yield return new WaitWhile(() => anim != null && anim.GetCurrentFrame() < 5);
					}
				}
			}
			IEnumerator Preload(Vector2 targ, bool skipBallCircle = false)
			{
				GameObject gameObject = FiveKnights.preloadedGO["Heart"];
				GameObject original = gameObject.transform.Find("Appear Trail").gameObject;
				GameObject whiteflashOld = FiveKnights.preloadedGO["WhiteFlashZem"];
				GameObject glowOld = gameObject.transform.Find("Get Anim").Find("Get Glow").gameObject;
				GameObject startCircleNew = UnityEngine.Object.Instantiate(original);
				GameObject original2 = FiveKnights.preloadedGO["HornetSphere"].transform.Find("Flash Effect").gameObject;
				float[] array = new float[4] { 0f, 90f, 180f, 270f };
				foreach (float num in array)
				{
					GameObject obj = UnityEngine.Object.Instantiate(original2);
					obj.transform.SetRotationZ(num + (float)UnityEngine.Random.Range(10, 30));
					obj.transform.position = base.transform.position;
					obj.transform.parent = base.transform;
					obj.SetActive(value: true);
					PlayMakerFSM playMakerFSM = obj.LocateMyFSM("FSM");
					playMakerFSM.enabled = true;
					playMakerFSM.FsmVariables.FindFsmFloat("Pause").Value = 1f;
					playMakerFSM.FsmVariables.FindFsmFloat("Rotation").Value = num + (float)UnityEngine.Random.Range(10, 30);
					playMakerFSM.FsmVariables.FindFsmBool("Reset Rotation").Value = false;
					playMakerFSM.SetState("Init");
				}
				startCircleNew.transform.position = targ;
				startCircleNew.transform.localScale *= 2f;
				startCircleNew.GetComponent<ParticleSystem>().Play();
				startCircleNew.SetActive(!skipBallCircle);
				GameObject obj2 = UnityEngine.Object.Instantiate(glowOld);
				obj2.SetActive(value: true);
				obj2.transform.position = targ;
				obj2.transform.SetRotation2D(UnityEngine.Random.Range(0, 360));
				yield return new WaitForSeconds(0.3f);
				UnityEngine.Object.Destroy(startCircleNew);
				GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
				GameObject obj3 = UnityEngine.Object.Instantiate(whiteflashOld);
				obj3.SetActive(value: true);
				obj3.transform.position = targ;
				GameObject gameObject2 = new GameObject();
				gameObject2.transform.position = targ;
				PlayAudioClip("NeedleSphere", 0.3f, 1f, gameObject2.transform);
				for (int j = 0; j < 5; j++)
				{
					GameObject obj4 = UnityEngine.Object.Instantiate(glowOld);
					obj4.SetActive(value: true);
					obj4.transform.position = targ;
					obj4.transform.SetRotation2D(j * 90 + UnityEngine.Random.Range(20, 70));
				}
				yield return new WaitForSeconds(0.15f);
			}
			static Vector2 QuadraticBezierInterp(Vector2 p1, Vector2 p2, Vector2 p3, float t)
			{
				Vector2 p4 = LineLerp(p1, p2, t);
				Vector2 p5 = LineLerp(p2, p3, t);
				return LineLerp(p4, p5, t);
			}
			IEnumerator SpawnSpirals(Vector2 targ, float scale, bool skipBallCircle = false)
			{
				yield return Preload(targ, skipBallCircle);
				GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
				GameObject slash = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["SlashRingControllerNew"]);
				_destroyAtEnd.Add(slash);
				slash.SetActive(value: true);
				slash.transform.position = targ;
				slash.transform.localScale /= scale * 2.5f;
				float spd = 2.5f;
				StartCoroutine(LerpScale(slash.transform, 2.5f));
				for (int i = 0; i < 3; i++)
				{
					ActivateSpiral(slash.transform.Find($"SlashRing{i}").gameObject, 2f);
				}
				Animator oldAnim = slash.transform.Find("SlashRing0").Find("1").gameObject.GetComponent<Animator>();
				yield return new WaitWhile(() => oldAnim.GetCurrentFrame() < 8);
				StartCoroutine(MakeSureOuterSlashesDontEnd());
				for (int num = 3; num < 4; num++)
				{
					ActivateSpiral(slash.transform.Find($"SlashRing{num}").gameObject, spd * 1.2f);
				}
				int[] array = new int[4] { 4, 5, 6, 7 };
				GameObject lastSpiral = null;
				int[] array2 = array;
				foreach (int num3 in array2)
				{
					GameObject gameObject = slash.transform.Find($"SlashRing{num3}").gameObject;
					ActivateSpiral(gameObject, spd * 2f);
					lastSpiral = gameObject;
					yield return new WaitForSeconds(0.05f);
				}
				oldAnim = lastSpiral.transform.Find("2").gameObject.GetComponent<Animator>();
				yield return new WaitWhile(() => oldAnim.GetCurrentFrame() < 8);
				GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Blast"].transform.Find("Particle middle").gameObject);
				obj.transform.position = slash.transform.position;
				obj.SetActive(value: true);
				yield return new WaitWhile(() => oldAnim.GetCurrentFrame() < 10);
				yield return LerpScale2(slash.transform);
				foreach (Transform item in slash.transform)
				{
					Animator[] componentsInChildren = item.GetComponentsInChildren<Animator>();
					foreach (Animator anim in componentsInChildren)
					{
						Log("Anim we check is " + anim.name);
						yield return anim.WaitToFrame(10);
						anim.gameObject.SetActive(value: false);
					}
				}
				continueWithSpiral = true;
				IEnumerator MakeSureOuterSlashesDontEnd()
				{
					while (slash != null)
					{
						for (int j = 0; j < 3; j++)
						{
							Animator component = USceneUtil.Find(slash.transform.Find($"SlashRing{j}").gameObject, "1").gameObject.GetComponent<Animator>();
							yield return component.PlayToEnd();
							Log("It ended a cycle");
						}
						for (int k = 0; k < 3; k++)
						{
							ActivateSpiral(slash.transform.Find($"SlashRing{k}").gameObject, 2.5f);
							Log("It started a cycle");
						}
						yield return null;
					}
				}
			}
		}

		private bool IsFacingPlayer()
		{
			int num = (int)Mathf.Sign(base.transform.localScale.x);
			int num2 = (int)Mathf.Sign(base.transform.position.x - _target.transform.position.x);
			return num == num2;
		}

		private IEnumerator Turn()
		{
			_anim.Play("ZTurn", -1, 0f);
			yield return new WaitForSeconds(0.05f);
		}

		private void Spring(bool isIn, Vector2 pos, float speedSca = 1f, bool parent = false)
		{
			string key = "VapeIn2";
			GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO[key]);
			PlayMakerFSM fsm = gameObject.LocateMyFSM("FSM");
			gameObject.GetComponent<tk2dSpriteAnimator>().GetClipByName("Plink").fps = 24f * speedSca;
			gameObject.transform.localScale *= 1.7f;
			fsm.GetAction<HutongGames.PlayMaker.Actions.Wait>("State 1", 0).time = 0f;
			gameObject.transform.position = pos;
			gameObject.SetActive(value: true);
			if (parent)
			{
				gameObject.transform.parent = base.transform;
			}
		}

		private void OnBlockedHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if (self.name.Contains("Zemer") && !_blockedHit)
			{
				_blockedHit = true;
				GameManager.instance.StartCoroutine(GameManager.instance.FreezeMoment(0.01f, 0.35f, 0.1f, 0f));
				Log("Blocked Hit");
				StopCoroutine(_counterRoutine);
				StartCoroutine(Countered());
			}
			else
			{
				orig(self, hitInstance);
			}
		}

		private IEnumerator Countered()
		{
			_hm.IsInvincible = false;
			On.HealthManager.Hit -= OnBlockedHit;
			yield return _anim.PlayToEndWithActions("ZCAtt", (3, delegate
			{
				PlayAudioClip("Slash", 0.15f);
			}));
			Parryable.ParryFlag = false;
			_anim.Play("ZIdle");
			yield return new WaitForSeconds(0.25f);
			_countering = false;
		}

		private void OnReceiveDreamImpact(On.EnemyDreamnailReaction.orig_RecieveDreamImpact orig, EnemyDreamnailReaction self)
		{
			if (self.name.Contains("Zemer"))
			{
				StartCoroutine(FlashWhite());
				UnityEngine.Object.Instantiate(_dnailEff, base.transform.position, Quaternion.identity);
				_dnailReac.SetConvoTitle("ZEM_CC_DREAM");
			}
			orig(self);
		}

		private void HealthManager_TakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
		{
			if (self.name.Contains("Zemer"))
			{
				isHit = true;
				_hitEffects.RecieveHitEffect(hitInstance.Direction);
				if (DoneFrenzyAtt == 0 && (float)_hm.hp < 0.5f * (float)Phase2HP)
				{
					Log($"Doing frenzy p{DoneFrenzyAtt}");
					StopAllCoroutines();
					Rigidbody2D[] array = UnityEngine.Object.FindObjectsOfType<Rigidbody2D>(includeInactive: true);
					foreach (Rigidbody2D rigidbody2D in array)
					{
						if (rigidbody2D.name.Contains("Nail") && rigidbody2D.transform.parent == null)
						{
							UnityEngine.Object.Destroy(rigidbody2D.gameObject);
						}
					}
					foreach (GameObject item in _destroyAtEnd.Where((GameObject x) => x != null))
					{
						UnityEngine.Object.Destroy(item);
					}
					_destroyAtEnd = new List<GameObject>();
					if (grass != null)
					{
						grass.Stop();
					}
					FaceHero();
					_bc.enabled = false;
					_anim.speed = 1f;
					_anim.enabled = true;
					_rb.velocity = Vector2.zero;
					_rb.gravityScale = 0f;
					DoneFrenzyAtt++;
					StartCoroutine(EndPhase1(firstDeath: false));
				}
				if (_hm.hp <= 0)
				{
					Log("Going to die :(");
					StopAllCoroutines();
					StartCoroutine(Death());
				}
			}
			orig(self, hitInstance);
		}

		private IEnumerator Death()
		{
			Rigidbody2D[] array = UnityEngine.Object.FindObjectsOfType<Rigidbody2D>(includeInactive: true);
			foreach (Rigidbody2D rigidbody2D in array)
			{
				if (rigidbody2D.name.Contains("Nail") && rigidbody2D.transform.parent == null)
				{
					Log("Destroyed nail!!!!");
					UnityEngine.Object.Destroy(rigidbody2D.gameObject);
				}
			}
			foreach (GameObject item in _destroyAtEnd.Where((GameObject x) => x != null))
			{
				UnityEngine.Object.Destroy(item);
			}
			grass.Stop();
			if (OWArenaFinder.IsInOverWorld)
			{
				OWBossManager.PlayMusic(null);
			}
			else
			{
				GGBossManager.Instance.PlayMusic(null, 1f);
			}
			FiveKnights.journalEntries["Zemer"].RecordJournalEntry();
			_isKnockingOut = true;
			float num = Math.Sign(base.transform.position.x - HeroController.instance.transform.position.x);
			FaceHero();
			_rb.gravityScale = 1.5f;
			_rb.velocity = new Vector2(num * 15f, 20f);
			PlayDeathFor(base.gameObject);
			_anim.enabled = true;
			_anim.Play("ZKnocked");
			StartCoroutine(PlayDeathSound());
			_anim.speed = 1f;
			yield return null;
			yield return new WaitWhile(() => _anim.GetCurrentFrame() < 1);
			_anim.enabled = false;
			yield return new WaitWhile(() => base.transform.position.y > GroundY - 0.99f);
			_rb.velocity = Vector2.zero;
			_anim.enabled = true;
			_rb.gravityScale = 0f;
			base.transform.position = new Vector3(base.transform.position.x, GroundY - deathGndOffset);
			yield return new WaitWhile(() => _anim.IsPlaying());
			_isKnockingOut = false;
			yield return new WaitForSeconds(1.75f);
			CustomWP.wonLastFight = true;
			Log("here");
			UnityEngine.Object.Destroy(this);
			IEnumerator PlayDeathSound()
			{
				PlayAudioClip("ZAudP1DeathD");
				yield return new WaitForSeconds(FiveKnights.Clips["ZAudP1DeathD"].length);
				PlayAudioClip("ZAudP1Death2");
			}
		}

		private int FaceHero(bool onlyCalc = false, bool opposite = false, float? tarX = null)
		{
			float valueOrDefault = tarX.GetValueOrDefault();
			if (!tarX.HasValue)
			{
				valueOrDefault = _target.transform.position.x;
				tarX = valueOrDefault;
			}
			int num = (int)Mathf.Sign(base.gameObject.transform.GetPositionX() - tarX.Value);
			num = (opposite ? (-num) : num);
			if (onlyCalc)
			{
				return num;
			}
			Vector3 localScale = base.gameObject.transform.localScale;
			base.gameObject.transform.localScale = new Vector3(Mathf.Abs(localScale.x) * (float)num, localScale.y, 1f);
			return num;
		}

		private IEnumerator FlashRepeat(Vector3 targ, float timer = 0.2f, bool cringeVersion = false)
		{
			if (cringeVersion)
			{
				GameObject fxOrig = FiveKnights.preloadedGO["HornetSphere"].transform.Find("Flash Effect").gameObject;
				for (int i = 0; i < 5; i++)
				{
					int[] array = new int[1] { 1 };
					foreach (int num in array)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(fxOrig);
						float num2 = (i * 90 + UnityEngine.Random.Range(30, 60)) * num;
						gameObject.transform.SetRotationZ(num2);
						gameObject.transform.position = targ;
						gameObject.SetActive(value: true);
						PlayMakerFSM playMakerFSM = gameObject.LocateMyFSM("FSM");
						playMakerFSM.enabled = true;
						playMakerFSM.FsmVariables.FindFsmFloat("Pause").Value = 1f;
						playMakerFSM.FsmVariables.FindFsmFloat("Rotation").Value = num2;
						playMakerFSM.FsmVariables.FindFsmBool("Reset Rotation").Value = false;
						playMakerFSM.FsmVariables.FindFsmVector3("Init Pos").Value = targ;
						playMakerFSM.SetState("Init");
						playMakerFSM.transform.localScale *= 0.65f;
						PlayAudioClip((i % 2 == 0) ? "AudBasicSlash1" : "AudBasicSlash2", 0.5f, 1f, gameObject.transform);
					}
					yield return new WaitForSeconds(timer);
					timer /= 1.5f;
				}
			}
			else
			{
				GameObject gameObject2 = FiveKnights.preloadedGO["Heart"];
				GameObject fxOrig = gameObject2.transform.Find("Get Anim").Find("Get Glow").gameObject;
				for (int i = 0; i < 5; i++)
				{
					GameObject gameObject3 = UnityEngine.Object.Instantiate(fxOrig);
					gameObject3.SetActive(value: true);
					gameObject3.transform.position = targ;
					float num3 = i * 90 + UnityEngine.Random.Range(20, 70);
					gameObject3.transform.SetRotation2D(num3);
					gameObject3.transform.SetRotation2D(0f - num3);
					PlayAudioClip((i % 2 == 0) ? "AudBasicSlash1" : "AudBasicSlash2", 0.5f, 1f, gameObject3.transform);
					yield return new WaitForSeconds(timer);
					timer /= 1.5f;
				}
			}
		}

		private Vector3 Product(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		private IEnumerator FlashWhite()
		{
			_sr.material.SetFloat(FLASH_AMOUNT, 1f);
			yield return null;
			for (float i = 1f; i >= 0f; i -= 0.05f)
			{
				_sr.material.SetFloat(FLASH_AMOUNT, i);
				yield return new WaitForSeconds(0.02f);
			}
			yield return null;
		}

		public void PlayAudioClip(string clipName, float pitchVar = 0f, float volume = 1f, Transform posOverride = null)
		{
			this.PlayAudio(clipName switch
			{
				"Counter" => (AudioClip)_pvFsm.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Counter Stance", 1).audioClip.Value, 
				"Slash" => (AudioClip)_pvFsm.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Slash1", 1).audioClip.Value, 
				"TraitorPillar" => FiveKnights.Clips["TraitorSlam"], 
				_ => FiveKnights.Clips[clipName], 
			}, volume, pitchVar, posOverride);
		}

		private static bool FastApproximately(float a, float b, float threshold)
		{
			return Math.Abs(a - b) <= threshold;
		}

		private void PlayDeathFor(GameObject go)
		{
			GameObject obj = UnityEngine.Object.Instantiate(_deathEff.uninfectedDeathPt);
			GameObject gameObject = UnityEngine.Object.Instantiate(_deathEff.whiteWave);
			obj.SetActive(value: true);
			gameObject.SetActive(value: true);
			Transform obj2 = obj.transform;
			Vector3 position = (gameObject.transform.position = go.transform.position);
			obj2.position = position;
			_deathEff.EmitSound();
			GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
		}

		private void ToggleZemer(bool visible, bool fade = false)
		{
			_bc.enabled = false;
			_anim.enabled = false;
			_anim.speed = 1f;
			if (fade)
			{
				StartCoroutine(Fade());
			}
			else
			{
				Instant();
			}
			IEnumerator Fade()
			{
				Color col = _sr.color;
				_sr.enabled = true;
				if (visible)
				{
					_sr.color = new Color(col.r, col.g, col.b, 0f);
					for (float i = 0f; i <= 1f; i += 0.2f)
					{
						_sr.color = new Color(col.r, col.g, col.b, i);
						yield return new WaitForSeconds(0.01f);
					}
				}
				else
				{
					_sr.color = new Color(col.r, col.g, col.b, 1f);
					for (float i = col.a; i >= 0f; i -= 0.2f)
					{
						_sr.color = new Color(col.r, col.g, col.b, i);
						yield return new WaitForSeconds(0.01f);
					}
				}
				Instant();
			}
			void Instant()
			{
				_sr.enabled = visible;
				_anim.enabled = visible;
				_rb.gravityScale = 0f;
				_bc.enabled = visible;
				Color color = _sr.color;
				_sr.color = new Color(color.r, color.g, color.b, visible ? 1f : 0f);
			}
		}

		private void AssignFields()
		{
			Transform obj = base.transform.Find("NewSlash3");
			obj.localPosition = new Vector3(-16f, 3.5f, -0.5f);
			obj.localScale = new Vector3(0.6f, 0.6f, 0.5f);
			traitorSlam = new GameObject[2];
			traitorSlamIndex = 0;
			for (int i = 0; i < traitorSlam.Length; i++)
			{
				traitorSlam[i] = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["TraitorSlam"]);
				UnityEngine.Object.Destroy(traitorSlam[i].GetComponent<AutoRecycleSelf>());
				traitorSlam[i].transform.Find("slash_core").Find("hurtbox").GetComponent<DamageHero>()
					.damageDealt = 1;
				Transform transform = traitorSlam[i].transform.Find("slash_core").Find("hurtbox");
				Transform obj2 = UnityEngine.Object.Instantiate(transform);
				obj2.name = "Test2";
				obj2.parent = traitorSlam[i].transform.Find("slash_core").transform;
				obj2.transform.position = transform.transform.position;
				obj2.transform.localScale = transform.transform.localScale;
				transform.gameObject.SetActive(value: false);
				traitorSlam[i].SetActive(value: false);
			}
			DamageHero[] componentsInChildren = base.transform.Find("Ki").GetComponentsInChildren<DamageHero>(includeInactive: true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].damageDealt = 2;
			}
			componentsInChildren = base.transform.Find("HyperCut").GetComponentsInChildren<DamageHero>(includeInactive: true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].damageDealt = 2;
			}
			componentsInChildren = base.transform.Find("BladeAerialShadow").GetComponentsInChildren<DamageHero>(includeInactive: true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].damageDealt = 1;
			}
		}

		private IEnumerator WaitByVelocity(float offset)
		{
			yield return null;
			if (_rb.velocity.x < 0f)
			{
				yield return new WaitWhile(() => !_rb.velocity.x.Within(0f, 0.5f) && base.transform.position.x > LeftX + offset);
			}
			else
			{
				yield return new WaitWhile(() => !_rb.velocity.x.Within(0f, 0.5f) && base.transform.position.x < RightX - offset);
			}
		}

		private bool CheckIfStuck()
		{
			if (_rb.velocity.x.Within(0f, 0.5f))
			{
				_rb.gravityScale = 2f;
				_rb.isKinematic = false;
			}
			return true;
		}

		private void Log(object o)
		{
			Modding.Logger.Log("[Zemer2] " + o);
		}

		private void OnDestroy()
		{
			On.HealthManager.Hit -= OnBlockedHit;
			On.HealthManager.Die -= HealthManagerOnDie;
			On.HealthManager.TakeDamage -= HealthManager_TakeDamage;
			On.EnemyDreamnailReaction.RecieveDreamImpact -= OnReceiveDreamImpact;
		}
	}
	public static class ZemRandAudio
	{
		private const string BaseName = "ZAudAtt";

		private static Dictionary<string, int> _counter = new Dictionary<string, int>
		{
			["ZAudAtt2"] = 0,
			["ZAudAtt3"] = 0,
			["ZAudAtt4"] = 0,
			["ZAudAtt5"] = 0,
			["ZAudAtt6"] = 0,
			["ZAudAtt7"] = 0,
			["ZAudAtt8"] = 0,
			["ZAudAtt9"] = 0,
			["ZAudAtt10"] = 0,
			["ZAudAtt11"] = 0,
			["ZAudAtt12"] = 0,
			["ZAudAtt13"] = 0
		};

		public static string PickRandomZemAud(int min, int max)
		{
			List<string> list = (from x in new int[12]
				{
					2, 3, 4, 5, 6, 7, 8, 9, 10, 11,
					12, 13
				}
				where x >= min && x <= max
				select "ZAudAtt" + x).ToList();
			string text = list[UnityEngine.Random.Range(0, list.Count)];
			while (list.Count > 1 && _counter[text] > 0)
			{
				list.Remove(text);
				text = list[UnityEngine.Random.Range(0, list.Count)];
			}
			if (list.Count <= 1)
			{
				_counter = _counter.ToDictionary((KeyValuePair<string, int> kvp) => kvp.Key, (KeyValuePair<string, int> _) => 0);
				text = list[UnityEngine.Random.Range(0, list.Count)];
			}
			_counter[text]++;
			return text;
		}
	}
}
namespace FiveKnights.Tiso
{
	public class Shield : MonoBehaviour
	{
		private Rigidbody2D _rb;

		private float _force = 2f;

		private Vector3 _startPos;

		private bool _isready;

		private float _time;

		private const float LeftX = 51.2f;

		private const float RightX = 71.7f;

		private const float ShieldSpeed = 47f;

		public float vertDir = 1f;

		public float yOffset;

		public float horizDir;

		public bool isDoneFlag;

		private void Start()
		{
			_rb = base.transform.gameObject.AddComponent<Rigidbody2D>();
			_rb.isKinematic = false;
			_rb.gravityScale = 0f;
			_rb.angularDrag = 0f;
			_rb.velocity = new Vector2((0f - horizDir) * 47f, 0f);
			GetComponent<Animator>().speed = 2f;
			_startPos = base.transform.position;
			_time = 0f;
		}

		private void FixedUpdate()
		{
			float x = base.transform.position.x;
			if ((x < 51.2f || x > 71.7f) ? true : false)
			{
				_rb.velocity *= new Vector2(-1f, 0f);
				isDoneFlag = true;
			}
			float y = _startPos.y + vertDir * Mathf.Sin(_time * 8f) * 2f + yOffset;
			if (isDoneFlag)
			{
				_force = 2.5f;
			}
			Vector2 force = new Vector2(_force * horizDir, 0f);
			base.transform.position = new Vector3(base.transform.position.x, y);
			_rb.AddForce(force, ForceMode2D.Impulse);
			if (_rb.velocity.x * horizDir > 0f)
			{
				isDoneFlag = true;
			}
			_time += Time.fixedDeltaTime;
		}
	}
	public class TisoAttacks
	{
		private Transform transform;

		private Rigidbody2D _rb;

		private BoxCollider2D _bc;

		private Animator _anim;

		private GameObject _target;

		private GameObject _shield;

		private TisoController _tc;

		private EnemyDeathEffectsUninfected _deathEff;

		private List<GameObject> _destroyOnDeath;

		private const float RunSpeed = 12f;

		private const float DodgeSpeed = 20f;

		private const float BombHeight = 19f;

		private const float SpikeVel = 60f;

		private const int NumShots = 8;

		public TisoAttacks(Transform transform, Rigidbody2D rb, BoxCollider2D bc, Animator anim, EnemyDeathEffectsUninfected deathEff)
		{
			TisoAttacks tisoAttacks = this;
			this.transform = transform;
			_rb = rb;
			_bc = bc;
			_anim = anim;
			_shield = transform.Find("Shields").Find("ShieldHalf").gameObject;
			_target = HeroController.instance.gameObject;
			_deathEff = deathEff;
			_tc = transform.GetComponent<TisoController>();
			_destroyOnDeath = new List<GameObject>();
			foreach (Transform item in this.transform.Find("ShieldHB"))
			{
				GameObject gameObject = item.gameObject;
				TisoShieldParry tisoShieldParry = gameObject.AddComponent<TisoShieldParry>();
				if (item.name.Contains("UpShield"))
				{
					tisoShieldParry.Predicate = delegate
					{
						Vector2 vector = tisoAttacks._target.transform.position;
						Vector2 vector2 = transform.position;
						Vector3 vector3 = transform.InverseTransformPoint(vector);
						return vector.x.Within(vector2.x, 2.5f) && vector3.y > 0.5f;
					};
				}
				else
				{
					tisoShieldParry.Predicate = () => (!(Mathf.Sign(transform.GetScaleX()) > 0f)) ? (HeroController.instance.transform.position.x > transform.position.x) : (HeroController.instance.transform.position.x < transform.position.x);
				}
				gameObject.layer = 11;
				gameObject.AddComponent<DamageHero>();
			}
			TisoSpike.AllSpikes = new List<GameObject>();
		}

		public IEnumerator SpawnBombs()
		{
			Transform transform = this.transform.Find("SwapWeapon").Find("s3").Find("s8");
			GameObject bombPar = transform.Find("Bomb").gameObject;
			TisoBomb.AllBombs = new List<GameObject>();
			_anim.Play("TisoRoar");
			TisoAudio.PlayAudio(_tc, TisoAudio.Clip.Yell);
			yield return new WaitForSeconds(0.6f);
			float leftPos = this.transform.position.x - 3f;
			float rightPos = this.transform.position.x + 3f;
			Coroutine a = _tc.StartCoroutine(BlockUpSpecial());
			while (leftPos > 52.5f || rightPos < 69.2f)
			{
				if (leftPos > 52.5f)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(bombPar);
					gameObject.transform.position = new Vector3(leftPos, 19f);
					gameObject.SetActive(value: true);
					gameObject.AddComponent<TisoBomb>();
				}
				if (rightPos < 69.2f)
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate(bombPar);
					gameObject2.transform.position = new Vector3(rightPos, 19f);
					gameObject2.SetActive(value: true);
					gameObject2.AddComponent<TisoBomb>();
				}
				leftPos -= 2.5f;
				rightPos += 2.5f;
				yield return new WaitForSeconds(UnityEngine.Random.Range(0.2f, 0.25f));
			}
			yield return a;
			IEnumerator BlockUpSpecial()
			{
				this.transform.Find("ShieldHB").Find("UpShield").GetComponent<TisoShieldParry>()
					.hitFlag = false;
				yield return _anim.PlayToFrame("TisoBlockUp", 0);
				_anim.enabled = false;
				yield return new WaitWhile(() => TisoBomb.AllBombs.Count > 0);
				yield return new WaitForSeconds(0.1f);
				_anim.enabled = true;
				yield return Dodge();
			}
		}

		public IEnumerator BlockUp()
		{
			TisoShieldParry shieldParry = transform.Find("ShieldHB").Find("UpShield").GetComponent<TisoShieldParry>();
			shieldParry.hitFlag = false;
			yield return _anim.PlayToFrame("TisoBlockUp", 0);
			_anim.enabled = false;
			yield return new WaitSecWhile(() => !shieldParry.hitFlag && CheckPlayerAbove(), 1.5f);
			_anim.enabled = true;
			if (shieldParry.hitFlag)
			{
				yield return _anim.PlayToEnd();
			}
			yield return Dodge();
		}

		public IEnumerator JumpGlideSlam()
		{
			bool shouldStopWave = false;
			_tc.PlayAudio(TisoAudio.TisoAud[TisoRandAudio.PickRandomTisoAud(2, 6)]);
			yield return Jump();
			yield return Glide();
			yield return Slam();
			yield return Dodge();
			IEnumerator Glide()
			{
				float dir = Mathf.Sign(transform.GetScaleX());
				_anim.Play("TisoFlyAntic", -1, 0f);
				_rb.gravityScale = 0.5f;
				yield return _anim.PlayToEnd();
				_anim.Play("TisoFly");
				_rb.velocity = new Vector2((0f - dir) * 20f, _rb.velocity.y / 4f);
				_tc.StartCoroutine(WaveMove());
				yield return new WaitWhile(() => (!(dir > 0f)) ? (_target.transform.position.x - 2f > transform.position.x) : (_target.transform.position.x + 2f < transform.position.x));
				_rb.isKinematic = true;
				_rb.gravityScale = 0f;
				shouldStopWave = true;
			}
			IEnumerator Jump()
			{
				FaceHero();
				TisoAudio.PlayAudio(_tc, TisoAudio.Clip.Jump);
				yield return _anim.PlayToEnd("TisoJump");
				float num = FaceHero();
				_anim.Play("TisoSpin");
				float y = 40f;
				_rb.velocity = new Vector2((0f - num) * 10f, y);
				_rb.isKinematic = false;
				_rb.gravityScale = 1.4f;
				yield return new WaitWhile(() => _rb.velocity.y > 5f);
			}
			IEnumerator Slam()
			{
				_anim.speed = 1.5f;
				_tc.StartCoroutine(DampToVel(_rb.velocity, new Vector2(0f, -50f), 1f / 6f));
				yield return _anim.PlayToFrame("TisoFlySlam", 4);
				_anim.speed = 1f;
				_anim.enabled = false;
				_rb.velocity = new Vector2(0f, -50f);
				yield return new WaitWhile(() => transform.position.y > 3.5f);
				TisoAudio.PlayAudio(_tc, TisoAudio.Clip.LandHard);
				_rb.velocity = Vector2.zero;
				transform.position = new Vector2(transform.position.x, 3.5f);
				GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
				_anim.enabled = true;
				SpawnShockwaves(1.2f, 35f, 1, transform.position);
				yield return _anim.PlayToEnd();
			}
			IEnumerator WaveMove()
			{
				while (!shouldStopWave)
				{
					_rb.velocity += new Vector2(0f, 3f);
					yield return new WaitForSeconds(0.2f);
				}
			}
		}

		public IEnumerator Shoot()
		{
			float dir = FaceHero() * -1f;
			Transform transform = this.transform.Find("SwapWeapon").Find("s3").Find("s8");
			transform.position = new Vector3(transform.position.x, transform.position.y, 0.5f);
			GameObject spikePar = transform.Find("Spikes").gameObject;
			_tc.PlayAudio(TisoAudio.TisoAud[TisoRandAudio.PickRandomTisoAud(2, 6)]);
			Vector2 to = _target.transform.position;
			float angleToPlayer = GetAngleToPlayer(spikePar.transform.position, to, dir);
			angleToPlayer = ((dir > 0f) ? Mathf.Max(Mathf.Min(40f, angleToPlayer), -17f) : Mathf.Max(Mathf.Min(15f, angleToPlayer), -42f));
			float num = ((dir > 0f) ? Mathf.Max(Mathf.Min(17f, angleToPlayer), -16f) : Mathf.Max(Mathf.Min(8f, angleToPlayer), -25f));
			this.transform.Find("SwapWeapon").Find("s3").Find("s8")
				.SetRotation2D(num);
			this.transform.Find("SwapWeapon").Find("s3").SetRotation2D(angleToPlayer - num);
			yield return _anim.PlayToEnd("TisoShootAntic");
			_anim.speed = 1f;
			int numInterrupts = 0;
			for (int i = 0; i < 8; i++)
			{
				if (FaceHero(onlyCalc: true) * dir > 0f || _target.transform.position.x.Within(this.transform.position.x, 3f))
				{
					_anim.speed = 1.5f;
					yield return _anim.PlayToEnd("TisoShootEnd");
					_anim.speed = 1f;
					yield return Dodge();
					if (numInterrupts >= 2)
					{
						yield break;
					}
					_anim.speed = 1.5f;
					dir = FaceHero() * -1f;
					yield return _anim.PlayToEnd("TisoShootAntic");
					_anim.speed = 1f;
					numInterrupts++;
				}
				to = _target.transform.position - new Vector3(0f, (dir > 0f) ? 0f : 1f);
				angleToPlayer = GetAngleToPlayer(spikePar.transform.position, to, dir);
				angleToPlayer = ((dir > 0f) ? Mathf.Max(Mathf.Min(40f, angleToPlayer), -17f) : Mathf.Max(Mathf.Min(15f, angleToPlayer), -42f));
				num = ((dir > 0f) ? Mathf.Max(Mathf.Min(17f, angleToPlayer), -16f) : Mathf.Max(Mathf.Min(8f, angleToPlayer), -25f));
				this.transform.Find("SwapWeapon").Find("s3").Find("s8")
					.SetRotation2D(num);
				this.transform.Find("SwapWeapon").Find("s3").SetRotation2D(angleToPlayer - num);
				_anim.Play("TisoShoot", -1, 0f);
				TisoAudio.PlayAudio(_tc, TisoAudio.Clip.Shoot);
				GameObject gameObject = UnityEngine.Object.Instantiate(spikePar);
				gameObject.transform.SetRotation2D(num);
				gameObject.transform.position = spikePar.transform.position + new Vector3(0f, 0f, 0.5f);
				gameObject.transform.localScale *= 2.5f;
				gameObject.SetActive(value: true);
				gameObject.AddComponent<TisoSpike>();
				Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
				num += ((dir > 0f) ? 0f : 180f);
				component.velocity = new Vector2(Mathf.Cos(num * ((float)Math.PI / 180f)), Mathf.Sin(num * ((float)Math.PI / 180f))) * 60f;
				_anim.speed = 1.2f;
				yield return _anim.PlayToEnd();
				_anim.speed = 1f;
			}
			_anim.speed = 1f;
			yield return _anim.PlayToEnd("TisoShootEnd");
		}

		private float GetAngleToPlayer(Vector2 from, Vector2 to, float dir)
		{
			Vector2 to2 = ((dir < 0f) ? (from - to).normalized : (to - from).normalized);
			return Vector2.SignedAngle(from, to2);
		}

		public IEnumerator ThrowShield()
		{
			float dir = FaceHero();
			_tc.PlayAudio(TisoAudio.TisoAud[TisoRandAudio.PickRandomTisoAud(2, 6)]);
			TisoAudio.PlayAudio(_tc, TisoAudio.Clip.ThrowShield);
			yield return _anim.PlayToEnd("TisoThrow");
			GameObject[] shields = new GameObject[2]
			{
				UnityEngine.Object.Instantiate(_shield),
				UnityEngine.Object.Instantiate(_shield)
			};
			_destroyOnDeath.AddRange(shields);
			int num = 0;
			float[] array = new float[2] { 0.4f, -0.4f };
			foreach (float num2 in array)
			{
				GameObject obj = shields[num];
				obj.transform.position = _shield.transform.position;
				obj.transform.localScale *= 2f;
				obj.SetActive(value: true);
				Shield shield = obj.AddComponent<Shield>();
				shield.horizDir = dir;
				shield.vertDir = num2;
				shield.yOffset = ((num2 == 0f) ? 0.25f : 0.5f);
				num++;
			}
			yield return _anim.PlayToFrame("TisoThrowCatch", 1);
			_anim.enabled = false;
			Shield shCtrl = shields[0].GetComponent<Shield>();
			yield return new WaitWhile(() => !shCtrl.isDoneFlag || ((!(dir > 0f)) ? (shields[0].transform.position.x >= transform.position.x + 3.5f) : ((double)shields[0].transform.position.x < (double)transform.position.x - 3.5)));
			_anim.enabled = true;
			_anim.speed = 3.5f;
			yield return _anim.PlayToFrame("TisoThrowCatch", 2);
			GameObject[] array2 = shields;
			foreach (GameObject item in array2)
			{
				_destroyOnDeath.Remove(item);
			}
			array2 = shields;
			for (int i = 0; i < array2.Length; i++)
			{
				UnityEngine.Object.Destroy(array2[i]);
			}
			_anim.speed = 1f;
			yield return _anim.PlayToEnd();
		}

		public IEnumerator Walk(float towardsX)
		{
			towardsX = Mathf.Max(towardsX, 52f);
			towardsX = Mathf.Min(towardsX, 69.7f);
			float dir = FacePos(new Vector2(towardsX, 0f));
			_anim.speed = 2f;
			_anim.Play("TisoRunStart", -1, 0f);
			yield return new WaitForEndOfFrame();
			_anim.enabled = false;
			yield return new WaitForSeconds(0.15f);
			_anim.enabled = true;
			yield return _anim.PlayToEnd();
			_anim.Play("TisoRun");
			bool isDoneWalking = false;
			TisoAudio.PlayAudio(_tc, TisoAudio.Clip.Walk, 1f, 0f, () => !isDoneWalking);
			_rb.velocity = new Vector2(12f * (0f - dir), 0f);
			yield return new WaitWhile(delegate
			{
				if (!CheckPlayerAbove())
				{
					if (!(dir < 0f))
					{
						if (transform.position.x > towardsX)
						{
							return transform.position.x > 52f;
						}
						return false;
					}
					if (transform.position.x < towardsX)
					{
						return transform.position.x < 69.7f;
					}
					return false;
				}
				return false;
			});
			if (CheckPlayerAbove())
			{
				_anim.speed = 1f;
				_rb.velocity = Vector2.zero;
				yield return BlockUp();
			}
			else
			{
				isDoneWalking = true;
				yield return _anim.PlayToEnd("TisoRunEnd");
				_anim.speed = 1f;
				_rb.velocity = Vector2.zero;
			}
		}

		public IEnumerator Dodge()
		{
			float num = FaceHero();
			if ((transform.position.x < 56f && num < 0f) || (transform.position.x > 65.7f && num > 0f))
			{
				Modding.Logger.Log($"Choosing to do jump log spam {_tc.Rep[JumpGlideSlam]}");
				if (_tc.Rep[JumpGlideSlam] == 0)
				{
					Modding.Logger.Log($"Can repeat jumpglideslam {_tc.Rep[JumpGlideSlam]}");
					_tc.Rep[JumpGlideSlam]++;
					yield return JumpGlideSlam();
				}
				else
				{
					_tc.Rep[JumpGlideSlam] = 0;
					yield return BlockUp();
				}
			}
			else
			{
				_rb.velocity = new Vector2(20f * num, 0f);
				yield return _anim.PlayToEnd("TisoDodge");
				_rb.velocity = Vector2.zero;
			}
		}

		private void SpawnShockwaves(float vertScale, float speed, int damage, Vector2 pos)
		{
			bool[] obj = new bool[2] { false, true };
			PlayMakerFSM fsm = FiveKnights.preloadedGO["Mage"].LocateMyFSM("Mage Lord");
			bool[] array = obj;
			foreach (bool flag in array)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(fsm.GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>("Quake Waves", 0).gameObject.Value);
				PlayMakerFSM playMakerFSM = gameObject.LocateMyFSM("shockwave");
				playMakerFSM.FsmVariables.FindFsmBool("Facing Right").Value = flag;
				playMakerFSM.FsmVariables.FindFsmFloat("Speed").Value = speed;
				gameObject.AddComponent<DamageHero>().damageDealt = damage;
				gameObject.SetActive(value: true);
				gameObject.transform.SetPosition2D(new Vector2(pos.x + (flag ? 0.5f : (-0.5f)), 3f));
				gameObject.transform.SetScaleX(vertScale);
			}
		}

		public IEnumerator Death()
		{
			PlayerData.instance.isInvincible = true;
			foreach (GameObject item in _destroyOnDeath)
			{
				UnityEngine.Object.Destroy(item);
			}
			GameObject mawlek = GameObject.Find("Mawlek Body");
			if (mawlek == null)
			{
				Modding.Logger.Log("Mawlek not Found");
				GameObject gameObject = GameObject.Find("Battle Scene");
				mawlek = gameObject.transform.Find("Mawlek Body").gameObject;
			}
			float num = FaceHero();
			_rb.isKinematic = false;
			_bc.enabled = false;
			_rb.gravityScale = 1.5f;
			_rb.velocity = new Vector2(num * 10f, 30f);
			PlayDeathFor(transform.gameObject);
			FiveKnights.journalEntries["Tiso"].RecordJournalEntry();
			_anim.enabled = true;
			_anim.speed = 1f;
			_anim.Play("TisoSpin");
			yield return new WaitForSeconds(0.1f);
			yield return new WaitWhile(() => transform.position.y > 3.5f);
			_bc.enabled = true;
			_rb.gravityScale = 0f;
			_rb.isKinematic = true;
			_rb.velocity = Vector2.zero;
			transform.position = new Vector3(transform.position.x, 3.5f);
			FaceHero();
			yield return _anim.PlayToEnd("TisoLand");
			_anim.Play("TisoRoar");
			TisoAudio.PlayAudio(_tc, TisoAudio.Clip.Roar);
			yield return new WaitForSeconds(TisoAudio.TisoAud["AudTisoRoar"].length);
			mawlek.SetActive(value: true);
			mawlek.LocateMyFSM("Mawlek Control").FsmVariables.FindFsmBool("Skip Title").Value = true;
			mawlek.LocateMyFSM("Mawlek Control").FsmVariables.FindFsmBool("Skip Title").Value = true;
			mawlek.transform.position = new Vector2(transform.position.x, mawlek.transform.position.y);
			yield return new WaitWhile(() => mawlek.transform.position.y > 20f);
			Vector2 pos = new Vector2((61f > transform.position.x) ? 47f : 74.7f, 0f);
			float dir = FacePos(pos);
			yield return SpecialDodge(dir);
			yield return SpecialDodge(dir);
			yield return ShootBomb(mawlek);
			static IEnumerator ExplodeBomb(Animator bombAnim, float animSpd, float scale, GameObject gameObject2)
			{
				bombAnim.speed = animSpd;
				if (gameObject2 == null)
				{
					bombAnim.enabled = true;
					yield return bombAnim.PlayToEnd("BombAir");
				}
				else
				{
					Rigidbody2D rb = bombAnim.GetComponent<Rigidbody2D>();
					yield return new WaitWhile(() => (!(rb.velocity.x > 0f)) ? (bombAnim.transform.position.x > gameObject2.transform.position.x) : (bombAnim.transform.position.x < gameObject2.transform.position.x));
				}
				GameObject gameObject3 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Explosion"]);
				gameObject3.transform.position = bombAnim.transform.position;
				gameObject3.transform.localScale *= scale;
				gameObject3.SetActive(value: true);
				UnityEngine.Object.Destroy(bombAnim.gameObject);
			}
			IEnumerator ShootBomb(GameObject gameObject2)
			{
				float dir2 = FacePos(gameObject2.transform.position) * -1f;
				Transform arm = transform.Find("SwapWeapon").Find("s3").Find("s8");
				arm.position = new Vector3(arm.position.x, arm.position.y, 0.5f);
				GameObject bombPar = arm.Find("Bomb").gameObject;
				yield return _anim.PlayToEnd("TisoShootAntic");
				for (int i = 0; i < 8; i++)
				{
					Vector2 to = gameObject2.transform.position;
					float angleToPlayer = GetAngleToPlayer(transform.Find("SwapWeapon").Find("s3").Find("s8")
						.position, to, dir2);
						angleToPlayer = ((dir2 > 0f) ? Mathf.Max(Mathf.Min(40f, angleToPlayer), -17f) : Mathf.Max(Mathf.Min(15f, angleToPlayer), -42f));
						float num2 = ((dir2 > 0f) ? Mathf.Max(Mathf.Min(17f, angleToPlayer), -16f) : Mathf.Max(Mathf.Min(8f, angleToPlayer), -25f));
						transform.Find("SwapWeapon").Find("s3").Find("s8")
							.SetRotation2D(num2);
						transform.Find("SwapWeapon").Find("s3").SetRotation2D(angleToPlayer - num2);
						_anim.Play("TisoShoot", -1, 0f);
						_anim.enabled = true;
						GameObject gameObject3 = UnityEngine.Object.Instantiate(bombPar);
						gameObject3.transform.localScale *= 1.9f;
						gameObject3.transform.position = bombPar.transform.position + new Vector3(0f, 0f, 0.5f);
						gameObject3.SetActive(value: true);
						Animator component = gameObject3.GetComponent<Animator>();
						if (i == 7)
						{
							gameObject3.transform.localScale *= 1.1f;
							gameObject3.transform.localScale = new Vector3(gameObject3.transform.localScale.x, Mathf.Abs(gameObject3.transform.localScale.y) * (0f - dir2), gameObject3.transform.localScale.z);
							arm.Find("ShootFx").gameObject.SetActive(value: false);
							yield return ExplodeBomb(component, 0.6f, 2f, null);
							_anim.enabled = true;
							_anim.Play("TisoDeath");
							TisoAudio.PlayAudio(_tc, TisoAudio.Clip.Death);
							_tc.PlayMusic(null);
							transform.position -= new Vector3(0f, 0.5f, 0f);
							transform.localScale.Scale(new Vector3(-1f, 1f, 1f));
							break;
						}
						Rigidbody2D component2 = gameObject3.GetComponent<Rigidbody2D>();
						num2 += ((dir2 > 0f) ? 0f : 180f);
						component2.velocity = new Vector2(Mathf.Cos(num2 * ((float)Math.PI / 180f)), Mathf.Sin(num2 * ((float)Math.PI / 180f))) * 35f;
						_tc.StartCoroutine(ExplodeBomb(component, 1.5f, 1f, gameObject2));
						yield return _anim.PlayToEnd();
					}
				}
				IEnumerator SpecialDodge(float num2)
				{
					_rb.velocity = new Vector2(23f * dir, 0f);
					yield return _anim.PlayToEnd("TisoDodge");
					_rb.velocity = Vector2.zero;
				}
			}

			private void PlayDeathFor(GameObject go)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(_deathEff.uninfectedDeathPt);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(_deathEff.whiteWave);
				gameObject.SetActive(value: true);
				gameObject2.SetActive(value: true);
				Transform obj = gameObject.transform;
				Vector3 position = (gameObject2.transform.position = go.transform.position);
				obj.position = position;
				_deathEff.EmitSound();
				GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
			}

			private bool CheckPlayerAbove()
			{
				Vector2 vector = _target.transform.position;
				Vector2 vector2 = transform.position;
				Vector3 vector3 = transform.InverseTransformPoint(vector);
				if (vector.x.Within(vector2.x, 2.5f))
				{
					return vector3.y > 2f;
				}
				return false;
			}

			private IEnumerator DampToVel(Vector2 start, Vector2 end, float duration)
			{
				float time = 0f;
				Vector2 vel = Vector2.zero;
				while (time < duration)
				{
					_rb.velocity = Vector2.SmoothDamp(_rb.velocity, end, ref vel, duration);
					time += Time.deltaTime;
					yield return null;
				}
			}

			public float FaceHero(bool onlyCalc = false)
			{
				return FacePos(_target.transform.position, onlyCalc);
			}

			private float FacePos(Vector2 pos, bool onlyCalc = false)
			{
				float num = Mathf.Sign(transform.position.x - pos.x);
				if (!onlyCalc)
				{
					Vector2 vector = transform.localScale;
					transform.localScale = new Vector3(Mathf.Abs(vector.x) * num, vector.y);
				}
				return num;
			}
		}
		public static class TisoAudio
		{
			public enum Clip
			{
				SpikeHitWall,
				Jump,
				Land,
				LandHard,
				Shoot,
				Spin,
				ThrowShield,
				Walk,
				Death,
				Roar,
				Yell
			}

			public static Dictionary<string, AudioClip> TisoAud;

			public static AudioSource PlayAudio(MonoBehaviour from, Clip clip, float vol = 1f, float pitchVar = 0f, Func<bool> loopUntil = null)
			{
				AudioClip clipFromEnum = GetClipFromEnum(clip);
				return from.PlayAudio(clipFromEnum, vol, pitchVar, null, loopUntil);
			}

			private static AudioClip GetClipFromEnum(Clip clip)
			{
				return clip switch
				{
					Clip.SpikeHitWall => TisoAud["AudSpikeHitWall"], 
					Clip.Jump => TisoAud["AudTisoJump"], 
					Clip.Land => TisoAud["AudTisoLand"], 
					Clip.LandHard => TisoAud["AudLand"], 
					Clip.Shoot => TisoAud["AudTisoShoot"], 
					Clip.Spin => TisoAud["AudTisoSpin"], 
					Clip.ThrowShield => TisoAud["AudTisoThrowShield"], 
					Clip.Walk => TisoAud["AudTisoWalk"], 
					Clip.Death => TisoAud["AudTisoDeath"], 
					Clip.Roar => TisoAud["AudTisoRoar"], 
					Clip.Yell => TisoAud["AudTisoYell"], 
					_ => TisoAud["AudTisoYell"], 
				};
			}
		}
		public class TisoBomb : MonoBehaviour
		{
			private Animator _anim;

			private Rigidbody2D _rb;

			private bool hasExploded;

			public static List<GameObject> AllBombs;

			private void Awake()
			{
				_anim = GetComponent<Animator>();
				_rb = GetComponent<Rigidbody2D>();
				base.transform.localScale *= 2.5f;
				base.transform.position += new Vector3(0f, 0f, 0.3f);
			}

			private IEnumerator Start()
			{
				_rb.isKinematic = false;
				_rb.gravityScale = 2f;
				_anim.speed = 0.7f;
				AllBombs.Add(base.gameObject);
				yield return _anim.PlayToEnd("BombAir");
				if (!hasExploded)
				{
					StartCoroutine(Explode());
				}
			}

			private void Update()
			{
				base.transform.Rotate(0f, 0f, 25f);
			}

			private void OnTriggerEnter2D(Collider2D other)
			{
				if (!hasExploded && (other.gameObject.layer == 8 || other.gameObject.layer == 20))
				{
					StartCoroutine(Explode());
				}
			}

			private IEnumerator Explode()
			{
				hasExploded = true;
				GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Explosion"]);
				UnityEngine.Object.Destroy(obj.LocateMyFSM("damages_enemy"));
				obj.transform.localScale /= 1.65f;
				obj.transform.position = base.transform.position;
				obj.SetActive(value: true);
				yield return null;
				AllBombs.Remove(base.gameObject);
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		public class TisoController : MonoBehaviour
		{
			private HealthManager _hm;

			private BoxCollider2D _bc;

			private SpriteRenderer _sr;

			private EnemyDreamnailReaction _dnailReac;

			private GameObject _dd;

			private GameObject _dnailEff;

			private Animator _anim;

			private Rigidbody2D _rb;

			private EnemyDeathEffectsUninfected _deathEff;

			private ExtraDamageable _extraDamageable;

			private bool _hasDied;

			private EnemyHitEffectsUninfected _hitEffects;

			private GameObject _target;

			public const float GroundY = 3.5f;

			public const float LeftX = 52f;

			public const float RightX = 69.7f;

			public const float MiddleX = 61f;

			private readonly int DreamConvoAmount = 4;

			private readonly string DreamConvoKey = "TISO_DREAM";

			private readonly int MaxHP = ((BossSceneController.Instance.BossLevel > 0) ? 1400 : 1100);

			private System.Random _rand;

			private TisoAttacks _attacks;

			private bool _hit;

			private Coroutine _musicCoro;

			public Dictionary<Func<IEnumerator>, int> Rep;

			private Dictionary<Func<IEnumerator>, int> _max;

			private static LanguageCtrl _langCtrl;

			private void Awake()
			{
				_langCtrl = new LanguageCtrl();
				On.HealthManager.TakeDamage += HealthManager_TakeDamage;
				On.EnemyDreamnailReaction.RecieveDreamImpact += OnReceiveDreamImpact;
				On.SpellFluke.DoDamage += SpellFlukeOnDoDamage;
				_hm = base.gameObject.AddComponent<HealthManager>();
				_anim = base.gameObject.GetComponent<Animator>();
				_bc = base.gameObject.GetComponent<BoxCollider2D>();
				_rb = base.gameObject.AddComponent<Rigidbody2D>();
				_rb.isKinematic = true;
				_rb.interpolation = RigidbodyInterpolation2D.Interpolate;
				_sr = GetComponent<SpriteRenderer>();
				_dnailReac = base.gameObject.AddComponent<EnemyDreamnailReaction>();
				base.gameObject.AddComponent<DamageHero>().damageDealt = 1;
				_dd = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["WhiteDef"]);
				_dd.SetActive(value: false);
				_dnailEff = _dd.GetComponent<EnemyDreamnailReaction>().GetAttr<EnemyDreamnailReaction, GameObject>("dreamImpactPrefab");
				_rand = new System.Random();
				_dnailReac.enabled = true;
				Mirror.SetField(_dnailReac, "convoAmount", DreamConvoAmount);
				_extraDamageable = base.gameObject.AddComponent<ExtraDamageable>();
				_hitEffects = base.gameObject.AddComponent<EnemyHitEffectsUninfected>();
				_hitEffects.enabled = true;
				_deathEff = _dd.GetComponent<EnemyDeathEffectsUninfected>();
				base.gameObject.AddComponent<Flash>();
				_target = HeroController.instance.gameObject;
				_attacks = new TisoAttacks(base.transform, _rb, _bc, _anim, _deathEff);
				Rep = new Dictionary<Func<IEnumerator>, int>
				{
					[_attacks.Shoot] = 0,
					[_attacks.ThrowShield] = 0,
					[_attacks.JumpGlideSlam] = 0,
					[_attacks.SpawnBombs] = 0,
					[_attacks.Dodge] = 0
				};
				_max = new Dictionary<Func<IEnumerator>, int>
				{
					[_attacks.Shoot] = 1,
					[_attacks.ThrowShield] = 1,
					[_attacks.JumpGlideSlam] = 1,
					[_attacks.SpawnBombs] = 1,
					[_attacks.Dodge] = 1
				};
				AssignFields();
				_hm.hp = MaxHP;
				EnemyHPBarImport.RefreshHPBar(base.gameObject);
				base.gameObject.layer = 11;
			}

			private IEnumerator Start()
			{
				yield return DoIntro();
				StartCoroutine(Attacks());
			}

			private void Update()
			{
				Vector3 position = base.transform.position;
				if (position.x > 69.7f && _rb.velocity.x > 0f)
				{
					_rb.velocity = new Vector2(0f, _rb.velocity.y);
					base.transform.position = new Vector3(69.7f, position.y);
				}
				if (position.x < 52f && _rb.velocity.x < 0f)
				{
					_rb.velocity = new Vector2(0f, _rb.velocity.y);
					base.transform.position = new Vector3(52f, position.y);
				}
			}

			private IEnumerator DoIntro()
			{
				_sr.enabled = false;
				_bc.enabled = false;
				yield return new WaitForSeconds(3f);
				_sr.enabled = true;
				_bc.enabled = false;
				base.transform.position = new Vector3(66f, 17.5f);
				_bc.enabled = false;
				_rb.gravityScale = 1.5f;
				_rb.isKinematic = false;
				_anim.Play("TisoSpin");
				this.PlayAudio(TisoAudio.TisoAud["AudTiso1"]);
				TisoAudio.PlayAudio(this, TisoAudio.Clip.Spin);
				yield return new WaitWhile(() => base.transform.position.y > 3.5f);
				_bc.enabled = false;
				_rb.gravityScale = 0f;
				_rb.isKinematic = true;
				_rb.velocity = Vector2.zero;
				base.transform.position = new Vector3(base.transform.position.x, 3.5f);
				TisoAudio.PlayAudio(this, TisoAudio.Clip.Land);
				yield return _anim.PlayToEnd("TisoLand");
				_musicCoro = GameManager.instance.StartCoroutine(MusicControl());
				_anim.Play("TisoRoar");
				AudioSource aud = TisoAudio.PlayAudio(this, TisoAudio.Clip.Roar);
				DoTitle();
				_hit = false;
				_bc.enabled = true;
				yield return new WaitSecWhile(() => !_hit, TisoAudio.TisoAud["AudTisoRoar"].length);
				UnityEngine.Object.Destroy(aud.gameObject);
			}

			private IEnumerator MusicControl()
			{
				PlayMusic(FiveKnights.Clips["TisoMusicStart"]);
				yield return new WaitForSecondsRealtime(FiveKnights.Clips["TisoMusicStart"].length);
				PlayMusic(FiveKnights.Clips["TisoMusicLoop"]);
			}

			public void PlayMusic(AudioClip clip)
			{
				MusicCue musicCue = ScriptableObject.CreateInstance<MusicCue>();
				MusicCue.MusicChannelInfo musicChannelInfo = new MusicCue.MusicChannelInfo();
				Mirror.SetField(musicChannelInfo, "clip", clip);
				MusicCue.MusicChannelInfo[] value = new MusicCue.MusicChannelInfo[6] { musicChannelInfo, null, null, null, null, null };
				Mirror.SetField(musicCue, "channelInfos", value);
				Resources.FindObjectsOfTypeAll<AudioMixer>().First((AudioMixer x) => x.name == "Music").FindSnapshot("Main Only")
					.TransitionTo(0f);
				GameManager.instance.AudioManager.ApplyMusicCue(musicCue, 0f, 0f, applySnapshot: false);
			}

			private IEnumerator Attacks()
			{
				while (!_hasDied)
				{
					Log("[Setting Attacks]");
					Vector2 vector = _target.transform.position;
					Vector2 vector2 = base.transform.position;
					Vector3 vector3 = base.transform.InverseTransformPoint(vector);
					if (Mathf.Abs(vector.x - vector2.x) > 4f && _rand.Next(2) == 0)
					{
						Log("Doing Walk");
						float num = Mathf.Sign(vector.x - vector2.x);
						yield return _attacks.Walk((num > 0f) ? (vector2.x + 5f) : (vector2.x - 5f));
						Log("Done Walk");
					}
					else if (base.transform.position.x < 55f || base.transform.transform.position.x > 66.7f)
					{
						Log("Doing Walk");
						float num2 = Mathf.Sign(61f - vector2.x);
						yield return _attacks.Walk((num2 > 0f) ? (vector2.x + 6f) : (vector2.x - 6f));
						Log("Done Walk");
					}
					else if (vector.x.Within(vector2.x, 2.5f) && vector3.y > 2f)
					{
						Log("Doing BlockUp");
						yield return _attacks.BlockUp();
						Log("Done BlockUp");
					}
					else if (vector.x.Within(vector2.x, 2f))
					{
						Log("Doing Dodge");
						yield return _attacks.Dodge();
						Log("Done Dodge");
					}
					List<Func<IEnumerator>> attLst = ((_hm.hp <= 1000) ? new List<Func<IEnumerator>> { _attacks.Shoot, _attacks.ThrowShield, _attacks.JumpGlideSlam, _attacks.SpawnBombs } : new List<Func<IEnumerator>> { _attacks.Shoot, _attacks.ThrowShield, _attacks.JumpGlideSlam });
					Func<IEnumerator> currAtt = MiscMethods.ChooseAttack(attLst, Rep, _max);
					Log("Doing " + currAtt.Method.Name);
					yield return currAtt();
					Log("Done " + currAtt.Method.Name);
					Log("[Restarting Calculations]");
					yield return new WaitForEndOfFrame();
				}
			}

			private void DoTitle()
			{
				GameObject original = null;
				foreach (GameObject item in from x in UnityEngine.Object.FindObjectsOfType<GameObject>()
					where x.name.Contains("Area Title Holder")
					select x)
				{
					original = item.transform.Find("Area Title").gameObject;
				}
				original = UnityEngine.Object.Instantiate(original);
				original.SetActive(value: true);
				string smallMain = "Tiso";
				if (_langCtrl.ContainsKey("TISO_NAME", "Speech"))
				{
					smallMain = _langCtrl.Get("TISO_NAME", "Speech");
				}
				AreaTitleCtrl.ShowBossTitle(this, original, 2f, "", "", "", smallMain);
			}

			private void HealthManager_TakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
			{
				DoTakeDamage(self.gameObject, hitInstance.Direction);
				orig(self, hitInstance);
			}

			private void SpellFlukeOnDoDamage(On.SpellFluke.orig_DoDamage orig, SpellFluke self, GameObject tar, int upwardrecursionamount, bool burst)
			{
				DoTakeDamage(tar, 0f);
				orig(self, tar, upwardrecursionamount, burst);
			}

			private void DoTakeDamage(GameObject tar, float dir)
			{
				if (!tar.name.Contains("Tiso"))
				{
					return;
				}
				_hit = true;
				_hitEffects.RecieveHitEffect(dir);
				if (_hm.hp <= 100 && !_hasDied)
				{
					_hasDied = true;
					_bc.enabled = false;
					StopAllCoroutines();
					if (_musicCoro != null)
					{
						GameManager.instance.StopCoroutine(_musicCoro);
					}
					StartCoroutine(_attacks.Death());
				}
			}

			private void OnReceiveDreamImpact(On.EnemyDreamnailReaction.orig_RecieveDreamImpact orig, EnemyDreamnailReaction self)
			{
				if (self.name.Contains("Tiso"))
				{
					StartCoroutine(FlashWhite());
					UnityEngine.Object.Instantiate(_dnailEff, base.transform.position, Quaternion.identity);
					_dnailReac.SetConvoTitle(DreamConvoKey);
				}
				orig(self);
			}

			private IEnumerator FlashWhite()
			{
				_sr.material.SetFloat("_FlashAmount", 1f);
				yield return null;
				for (float i = 1f; i >= 0f; i -= 0.05f)
				{
					_sr.material.SetFloat("_FlashAmount", i);
					yield return new WaitForSeconds(0.02f);
				}
				yield return null;
			}

			private void OnTriggerEnter2D(Collider2D other)
			{
				if (other.gameObject.layer == 17 && other.gameObject.name.Contains("Spike"))
				{
					TisoSpike component = other.GetComponent<TisoSpike>();
					if (component.isDeflected && (!component.isDead || component.isDead2))
					{
						HitInstance hitInstance = new HitInstance
						{
							DamageDealt = 10,
							AttackType = AttackTypes.Nail,
							IgnoreInvulnerable = false,
							Source = other.gameObject,
							Multiplier = 1f,
							CircleDirection = true
						};
						_hm.Hit(hitInstance);
					}
				}
			}

			private void AssignFields()
			{
				Log("Assign Tiso Fields.");
				HealthManager component = _dd.GetComponent<HealthManager>();
				foreach (FieldInfo item in from x in typeof(HealthManager).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
					where x.Name.Contains("Prefab")
					select x)
				{
					item.SetValue(_hm, item.GetValue(component));
				}
				EnemyHitEffectsUninfected component2 = _dd.GetComponent<EnemyHitEffectsUninfected>();
				FieldInfo[] fields = typeof(EnemyHitEffectsUninfected).GetFields();
				foreach (FieldInfo fieldInfo in fields)
				{
					if (fieldInfo.Name.Contains("Origin"))
					{
						_hitEffects.effectOrigin = new Vector3(0f, 0.5f, 0f);
					}
					else
					{
						fieldInfo.SetValue(_hitEffects, fieldInfo.GetValue(component2));
					}
				}
				BoxCollider2D component3 = base.transform.Find("SwapWeapon").Find("s3").Find("s8")
					.Find("Spikes")
					.GetComponent<BoxCollider2D>();
				component3.gameObject.layer = 17;
				component3.gameObject.AddComponent<Pogoable>();
				component3.gameObject.AddComponent<Tink>();
				component3.gameObject.AddComponent<DamageHero>().damageDealt = 1;
				BoxCollider2D[] componentsInChildren = base.transform.Find("Shields").GetComponentsInChildren<BoxCollider2D>(includeInactive: true);
				foreach (BoxCollider2D obj in componentsInChildren)
				{
					obj.gameObject.layer = 22;
					obj.gameObject.AddComponent<Pogoable>();
					obj.gameObject.AddComponent<Tink>();
					obj.gameObject.AddComponent<DamageHero>().damageDealt = 1;
				}
				Transform obj2 = base.transform.Find("ShieldSpikePolygon");
				obj2.gameObject.layer = 22;
				obj2.gameObject.AddComponent<Pogoable>();
				obj2.gameObject.AddComponent<Tink>();
				obj2.gameObject.AddComponent<DamageHero>().damageDealt = 1;
				Mirror.SetField(_extraDamageable, "impactClipTable", Mirror.GetField<ExtraDamageable, RandomAudioClipTable>(_dd.GetComponent<ExtraDamageable>(), "impactClipTable"));
				Mirror.SetField(_extraDamageable, "audioPlayerPrefab", Mirror.GetField<ExtraDamageable, AudioSource>(_dd.GetComponent<ExtraDamageable>(), "audioPlayerPrefab"));
				Log("Done assigning Tiso Fields.");
			}

			private void OnDestroy()
			{
				if (_musicCoro != null)
				{
					GameManager.instance.StopCoroutine(_musicCoro);
				}
				On.HealthManager.TakeDamage -= HealthManager_TakeDamage;
				On.EnemyDreamnailReaction.RecieveDreamImpact -= OnReceiveDreamImpact;
				On.SpellFluke.DoDamage -= SpellFlukeOnDoDamage;
			}

			private void Log(object o)
			{
				Modding.Logger.Log("[Tiso] " + o);
			}
		}
		public class TisoFinder : MonoBehaviour
		{
			private const string TisoScene = "GG_Brooding_Mawlek_V";

			private const string StatueScene = "GG_Workshop";

			private void Awake()
			{
				UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChange;
				On.BossStatue.SwapStatues += BossStatueOnSwapStatues;
			}

			private IEnumerator BossStatueOnSwapStatues(On.BossStatue.orig_SwapStatues orig, BossStatue self, bool doanim)
			{
				if ((self.name == "GG_Statue_Mawlek") & doanim)
				{
					FiveKnights.Instance.SaveSettings.AltStatueMawlek = !FiveKnights.Instance.SaveSettings.AltStatueMawlek;
				}
				yield return orig(self, doanim);
			}

			private void OnSceneChange(Scene prev, Scene curr)
			{
				if (curr.name == "GG_Brooding_Mawlek_V")
				{
					Log($"We are on scene change and checking if alt is right {FiveKnights.Instance.SaveSettings.AltStatueMawlek}");
				}
				if (curr.name == "GG_Brooding_Mawlek_V" && FiveKnights.Instance.SaveSettings.AltStatueMawlek)
				{
					ClearOldContent(curr);
					BossLoader.LoadTisoBundle();
					BossLoader.CreateTiso();
				}
				if (curr.name == "GG_Workshop" && (PlayerData.instance.GetBool("tisoEncounteredColosseum") || FiveKnights.Instance.SaveSettings.CompletionMawlek2.isUnlocked))
				{
					SetStatue();
				}
			}

			private void ClearOldContent(Scene curr)
			{
				curr.GetRootGameObjects().First((GameObject go) => go.name == "Battle Scene").LocateMyFSM("Activate Boss")
					.enabled = false;
			}

			private void SetStatue()
			{
				Log("Setting Statue For Tiso.");
				GameObject gameObject = GameObject.Find("GG_Statue_Mawlek");
				Sprite sprite = ABManager.AssetBundles[ABManager.Bundle.WSArena].LoadAsset<Sprite>("Tiso_Statue");
				if (gameObject == null)
				{
					Log("Warning: Did not find Mawlek Statue");
					return;
				}
				BossStatue bs = gameObject.GetComponent<BossStatue>();
				string sceneName = "GG_Brooding_Mawlek_V";
				string dreamStatueStatePD = "statueStateMawlek2";
				string nameKey = "TISO_NAME";
				string descriptionKey = "TISO_DESC";
				BossScene bossScene = ScriptableObject.CreateInstance<BossScene>();
				bossScene.sceneName = sceneName;
				bs.dreamBossScene = bossScene;
				bs.dreamStatueStatePD = dreamStatueStatePD;
				UnityEngine.Object.Destroy(gameObject.FindGameObjectInChildren("StatueAlt"));
				GameObject statueDisplay = bs.statueDisplay;
				GameObject gameObject2 = UnityEngine.Object.Instantiate(statueDisplay, statueDisplay.transform.parent, worldPositionStays: true);
				gameObject2.SetActive(value: true);
				SpriteRenderer componentInChildren = gameObject2.GetComponentInChildren<SpriteRenderer>(includeInactive: true);
				componentInChildren.sprite = sprite;
				componentInChildren.gameObject.transform.position = new Vector3(45.54f, 8.16f, 1.94f);
				componentInChildren.gameObject.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
				gameObject2.name = "StatueAlt";
				bs.statueDisplayAlt = gameObject2;
				BossStatue.BossUIDetails dreamBossDetails = new BossStatue.BossUIDetails
				{
					nameKey = nameKey,
					nameSheet = "Speech",
					descriptionKey = descriptionKey,
					descriptionSheet = "Speech"
				};
				bs.dreamBossDetails = dreamBossDetails;
				GameObject obj = gameObject.FindGameObjectInChildren("alt_lever");
				obj.SetActive(value: true);
				obj.FindGameObjectInChildren("GG_statue_switch_bracket").SetActive(value: true);
				obj.FindGameObjectInChildren("GG_statue_switch_lever").SetActive(value: true);
				BossStatueLever componentInChildren2 = gameObject.GetComponentInChildren<BossStatueLever>();
				componentInChildren2.SetOwner(bs);
				componentInChildren2.SetState(value: true);
				FiveKnights.Instance.SaveSettings.CompletionMawlek2.isUnlocked = true;
				bs.DreamStatueState = FiveKnights.Instance.SaveSettings.CompletionMawlek2;
				StartCoroutine(Test());
				Log("Finish tiso statue.");
				IEnumerator Test()
				{
					yield return new WaitForSeconds(1f);
					bs.SetDreamVersion(FiveKnights.Instance.SaveSettings.AltStatueMawlek, useAltStatue: true, doAnim: false);
					bs.SetDreamVersion(!FiveKnights.Instance.SaveSettings.AltStatueMawlek, useAltStatue: true, doAnim: false);
					bs.SetDreamVersion(FiveKnights.Instance.SaveSettings.AltStatueMawlek, useAltStatue: true, doAnim: false);
				}
			}

			private void OnDestroy()
			{
				Log("Destroyed TisoFinder");
				UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnSceneChange;
				On.BossStatue.SwapStatues -= BossStatueOnSwapStatues;
			}

			private static void Log(object o)
			{
				Modding.Logger.Log($"[TisoFinder] {o}");
			}
		}
		public static class TisoRandAudio
		{
			private const string BaseName = "AudTiso";

			private static Dictionary<string, int> _counter = new Dictionary<string, int>
			{
				["AudTiso2"] = 0,
				["AudTiso3"] = 0,
				["AudTiso4"] = 0,
				["AudTiso5"] = 0,
				["AudTiso6"] = 0
			};

			public static string PickRandomTisoAud(int min, int max)
			{
				List<string> list = (from x in new int[5] { 2, 3, 4, 5, 6 }
					where x >= min && x <= max
					select "AudTiso" + x).ToList();
				string text = list[UnityEngine.Random.Range(0, list.Count)];
				while (list.Count > 1 && _counter[text] > 0)
				{
					list.Remove(text);
					text = list[UnityEngine.Random.Range(0, list.Count)];
				}
				if (list.Count <= 1)
				{
					_counter = _counter.ToDictionary((KeyValuePair<string, int> kvp) => kvp.Key, (KeyValuePair<string, int> _) => 0);
					text = list[UnityEngine.Random.Range(0, list.Count)];
				}
				_counter[text]++;
				return text;
			}
		}
		public class TisoShieldParry : MonoBehaviour
		{
			private BoxCollider2D _bc;

			public Action DoWhenHit = delegate
			{
			};

			public Func<bool> Predicate;

			private HealthManager _hm;

			public bool hitFlag;

			private void Awake()
			{
				_bc = GetComponent<BoxCollider2D>();
				_hm = base.transform.parent.parent.GetComponent<HealthManager>();
				On.HealthManager.Hit += OnBlockedHit;
			}

			private void Update()
			{
				if (_bc.enabled && !_hm.IsInvincible && Predicate())
				{
					_hm.InvincibleFromDirection = 0;
					_hm.IsInvincible = true;
					StartCoroutine(StopInvinc());
				}
			}

			private IEnumerator StopInvinc()
			{
				yield return new WaitForSeconds(0.1f);
				_hm.IsInvincible = false;
			}

			private void OnBlockedHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
			{
				if (self.name.Contains("Tiso") && _hm.IsInvincible)
				{
					hitFlag = true;
				}
				orig(self, hitInstance);
			}

			private void OnDestroy()
			{
				On.HealthManager.Hit -= OnBlockedHit;
			}
		}
		public class TisoSpike : MonoBehaviour
		{
			public static List<GameObject> AllSpikes;

			private const int MaxSpikes = 50;

			private const float LeftX = 51.5f;

			private const float RightX = 71.4f;

			private const float GroundY = 3.2f;

			private Rigidbody2D _rb;

			public bool isDead;

			public bool isDead2;

			public bool isDeflected;

			public const int EnemyDamage = 10;

			private const float SpikeVel = 60f;

			private void Awake()
			{
				AllSpikes.RemoveAll((GameObject spike) => spike == null);
				if (AllSpikes.Count > 50)
				{
					for (int num = 0; num < 5; num++)
					{
						int index = UnityEngine.Random.Range(0, AllSpikes.Count);
						GameObject obj = AllSpikes[index];
						AllSpikes.RemoveAt(index);
						UnityEngine.Object.Destroy(obj);
					}
				}
				_rb = GetComponent<Rigidbody2D>();
				base.transform.Find("BlurSpike").gameObject.SetActive(value: false);
				base.transform.Find("Spike").gameObject.SetActive(value: true);
				AllSpikes.Add(base.gameObject);
			}

			private void Update()
			{
				if (!isDead)
				{
					float x = base.transform.position.x;
					float y = base.transform.position.y;
					if (!(x > 51.5f) || !(x < 71.4f) || (!(y > 3.2f) && isDeflected))
					{
						base.transform.position = ((y > 3.2f) ? new Vector3((_rb.velocity.x > 0f) ? 71.4f : 51.5f, y) : new Vector3(x, 3.2f));
						_rb.velocity = Vector2.zero;
						TisoAudio.PlayAudio(this, TisoAudio.Clip.SpikeHitWall);
						base.transform.Find("BlurSpike").gameObject.SetActive(value: false);
						base.transform.Find("Spike").gameObject.SetActive(value: true);
						isDead = true;
					}
				}
			}

			private void OnTriggerEnter2D(Collider2D other)
			{
				if (other.gameObject.layer != 17 || other.gameObject.name.Contains("Spike") || other.gameObject.name.Contains("Tiso"))
				{
					return;
				}
				if (other.CompareTag("Hero Spell"))
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				else if (isDead && !isDead2)
				{
					if (ShootBasedOnHit(other))
					{
						StartCoroutine(DestroyAfterTime());
						isDeflected = true;
						isDead2 = true;
					}
				}
				else if (!isDeflected && !isDead && ShootBasedOnHit(other))
				{
					isDeflected = true;
				}
			}

			private bool ShootBasedOnHit(Collider2D other)
			{
				if (other.name.Contains("Up"))
				{
					float num = (float)UnityEngine.Random.Range(70, 110) * ((float)Math.PI / 180f);
					base.transform.SetRotation2D(num * 57.29578f);
					_rb.velocity = new Vector2(40f * Mathf.Cos(num), 40f * Mathf.Sin(num));
				}
				else if (other.name.Contains("Down"))
				{
					float num2 = (float)UnityEngine.Random.Range(250, 290) * ((float)Math.PI / 180f);
					base.transform.SetRotation2D(num2 * 57.29578f);
					_rb.velocity = new Vector2(40f * Mathf.Cos(num2), 40f * Mathf.Sin(num2));
				}
				else
				{
					if (!other.CompareTag("Nail Attack"))
					{
						return false;
					}
					float num3 = 0f - Mathf.Sign(HeroController.instance.transform.localScale.x);
					base.transform.SetRotation2D(0f);
					_rb.velocity = new Vector2(60f * num3, 0f);
				}
				return true;
			}

			private IEnumerator DestroyAfterTime()
			{
				yield return new WaitForSeconds(10f);
				if (!(base.gameObject == null))
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}
	}
	namespace FiveKnights.Misc
	{
		public class Artists : MonoBehaviour
		{
			private GameObject _sheo;

			private GameObject _smith;

			private MusicPlayer _sheoSnd;

			private MusicPlayer _smithSnd;

			private Dictionary<string, AudioClip> _clips;

			private void Awake()
			{
				UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChange;
			}

			private void OnSceneChange(Scene prev, Scene curr)
			{
				if (prev.name == "Room_nailmaster_02")
				{
					UnityEngine.Object.Destroy(_sheo);
					UnityEngine.Object.Destroy(_smith);
					UnityEngine.Object.Destroy(this);
				}
			}

			private void Start()
			{
				_clips = new Dictionary<string, AudioClip>();
				LoadArtist();
				Log("Instantiate artist");
				GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Artist"]);
				gameObject.transform.position = FiveKnights.preloadedGO["Artist"].transform.position;
				gameObject.SetActive(value: true);
				_sheo = gameObject.transform.Find("CuteScene").Find("Sheo").gameObject;
				_smith = gameObject.transform.Find("CuteScene").Find("Smith").gameObject;
				GameObject value = HeroController.instance.gameObject.LocateMyFSM("Spell Control").GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>("Fireball 2", 3).gameObject.Value.LocateMyFSM("Fireball Cast").GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Cast Right", 3).audioPlayer.Value;
				_sheoSnd = new MusicPlayer
				{
					Volume = 1f,
					Player = value,
					Loop = false,
					MaxPitch = 1f,
					MinPitch = 1f,
					Spawn = _sheo
				};
				_smithSnd = new MusicPlayer
				{
					Volume = 1f,
					Player = value,
					Loop = false,
					MaxPitch = 1f,
					MinPitch = 1f,
					Spawn = _smith
				};
				string[] array = new string[2] { "Painting Parent", "NM Parent" };
				for (int i = 0; i < array.Length; i++)
				{
					GameObject.Find(array[i]).SetActive(value: false);
				}
				_smith.transform.Find("Hands").GetComponent<Animator>().enabled = true;
				CreateDialogue(_sheo, new Vector2(42f, 11f), "NM_SHEO", "SHEO_DREAM", GetSheoDialogue, alwaysLeft: false, alwaysRight: true, 3f);
				CreateDialogue(_smith, new Vector2(30.5f, 9.4f), "NAILSMITH", "SMITH_DREAM", GetSmithDialogue, alwaysLeft: false, alwaysRight: true, 3f);
				Log("Instantiate artist 2");
			}

			private void CreateDialogue(GameObject target, Vector2 promptPos, string title, string dream, Func<DialogueCallbackOptions, DialogueOptions> dialogueSelecter, bool alwaysLeft, bool alwaysRight, float offset)
			{
				DialogueNPC dialogueNPC = DialogueNPC.CreateInstance();
				dialogueNPC.transform.position = target.transform.position;
				dialogueNPC.transform.Find("Prompt Marker").position = promptPos;
				dialogueNPC.DialogueSelector = dialogueSelecter;
				dialogueNPC.GetComponent<MeshRenderer>().enabled = false;
				dialogueNPC.SetTitle(title);
				dialogueNPC.SetDreamKey(dream);
				dialogueNPC.SetUp();
				PlayMakerFSM fsm = dialogueNPC.gameObject.LocateMyFSM("npc_control");
				fsm.GetBoolVariable("Hero Always Right").Value = alwaysRight;
				fsm.GetBoolVariable("Hero Always Left").Value = alwaysLeft;
				fsm.GetFloatVariable("Move To Offset").Value = offset;
				BoxCollider2D component = target.GetComponent<BoxCollider2D>();
				BoxCollider2D component2 = dialogueNPC.GetComponent<BoxCollider2D>();
				BoxCollider2D component3 = dialogueNPC.transform.Find("Dream Dialogue").GetComponent<BoxCollider2D>();
				Vector2 size = (component3.size = component.size);
				component2.size = size;
				size = (component3.offset = component.offset);
				component2.offset = size;
			}

			private DialogueOptions GetSheoDialogue(DialogueCallbackOptions prev)
			{
				if (!prev.Continue)
				{
					string key = (FiveKnights.Instance.SaveSettings.GreetedSheo ? "SHEO_PC_3" : "SHEO_PC_1");
					return new DialogueOptions
					{
						Key = key,
						Sheet = "Minor NPC",
						Type = DialogueType.Normal,
						Wait = PlayAnimSheo(),
						Continue = true
					};
				}
				string key2 = prev.Key;
				if (!(key2 == "SHEO_PC_1"))
				{
					if (key2 == "SHEO_PC_2")
					{
						return new DialogueOptions
						{
							Key = "SHEO_PC_3",
							Sheet = "Minor NPC",
							Type = DialogueType.Normal,
							Continue = true
						};
					}
					return new DialogueOptions
					{
						Continue = false,
						Wait = StopAnimSheo()
					};
				}
				return new DialogueOptions
				{
					Key = "SHEO_PC_2",
					Sheet = "Minor NPC",
					Type = DialogueType.Normal,
					Continue = true
				};
				IEnumerator PlayAnimSheo()
				{
					Animator anim = _sheo.GetComponent<Animator>();
					anim.enabled = true;
					_sheoSnd.Clip = _clips["sheo_1"];
					_sheoSnd.DoPlayRandomClip();
					anim.Play("SheoTalk", -1, 0f);
					yield return anim.WaitToFrame(3);
					anim.enabled = false;
				}
				IEnumerator StopAnimSheo()
				{
					FiveKnights.Instance.SaveSettings.GreetedSheo = true;
					Animator component = _sheo.GetComponent<Animator>();
					component.enabled = true;
					component.Play("SheoStopTalk", -1, 0f);
					yield return component.PlayToEnd();
				}
			}

			private DialogueOptions GetSmithDialogue(DialogueCallbackOptions prev)
			{
				if (!prev.Continue)
				{
					string key = (FiveKnights.Instance.SaveSettings.GreetedNailsmith ? "SMITH_PC_3" : "SMITH_PC_1");
					return new DialogueOptions
					{
						Key = key,
						Sheet = "Minor NPC",
						Type = DialogueType.Normal,
						Wait = PlayAnimSmith(),
						Continue = true
					};
				}
				string key2 = prev.Key;
				if (!(key2 == "SMITH_PC_1"))
				{
					if (key2 == "SMITH_PC_2")
					{
						return new DialogueOptions
						{
							Key = "SMITH_PC_3",
							Sheet = "Minor NPC",
							Type = DialogueType.Normal,
							Continue = true
						};
					}
					return new DialogueOptions
					{
						Continue = false,
						Wait = StopAnimSmith()
					};
				}
				return new DialogueOptions
				{
					Key = "SMITH_PC_2",
					Sheet = "Minor NPC",
					Type = DialogueType.Normal,
					Continue = true
				};
				IEnumerator PlayAnimSmith()
				{
					yield return null;
					_smithSnd.Clip = _clips["smith_1"];
					_smithSnd.DoPlayRandomClip();
					_smith.transform.Find("Hands").GetComponent<Animator>().enabled = false;
					_smith.GetComponent<Animator>().enabled = true;
				}
				IEnumerator StopAnimSmith()
				{
					FiveKnights.Instance.SaveSettings.GreetedNailsmith = true;
					yield return null;
					_smith.transform.Find("Hands").GetComponent<Animator>().enabled = true;
					_smith.GetComponent<Animator>().enabled = false;
				}
			}

			private void LoadArtist()
			{
				Log("Loading Artist Bundle");
				if (FiveKnights.preloadedGO.TryGetValue("Artist", out var value) && value != null)
				{
					Log("Already Loaded Artist so will destroy it");
					UnityEngine.Object.Destroy(value);
				}
				AssetBundle assetBundle = ABManager.AssetBundles[ABManager.Bundle.Artist];
				AnimationClip[] array = assetBundle.LoadAllAssets<AnimationClip>();
				foreach (AnimationClip animationClip in array)
				{
					FiveKnights.AnimClips[animationClip.name] = animationClip;
				}
				AudioClip[] array2 = assetBundle.LoadAllAssets<AudioClip>();
				foreach (AudioClip audioClip in array2)
				{
					_clips[audioClip.name] = audioClip;
				}
				FiveKnights.preloadedGO["Artist"] = assetBundle.LoadAsset<GameObject>("Artists");
				Log("Finished Loading Artist Bundle");
			}

			private void OnDestroy()
			{
				UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnSceneChange;
			}

			private static void Log(object o)
			{
				Modding.Logger.Log($"[Artist] {o}");
			}
		}
		public class DebugColliders : MonoBehaviour
		{
			public List<BoxCollider2D> boxColliders;

			public List<CircleCollider2D> circleColliders;

			public List<PolygonCollider2D> polyColliders;

			public List<EdgeCollider2D> edgeColliders;

			public GameObject lineRoot;

			public float lineWidth = 0.05f;

			public float zDepth = -2.1f;

			public Color colliderColor = Color.green;

			public Color triggerColor = Color.blue;

			public Color disabledColor = Color.red;

			public Color gameObjectDisabledColor = Color.gray;

			public bool renderDisabledColliders = true;

			public bool runDebugInput = true;

			private Dictionary<Collider2D, LineRenderer> lines;

			private Material colliderLineMat;

			private Material triggerLineMat;

			private Material disabledLineMat;

			private Material gameObjectDisabledLineMat;

			private Dictionary<Collider2D, TextMesh> labels;

			private static bool forceSingleEntry;

			private static IEnumerator debugInput;

			private List<Vector2> GetPointsFromCollider(BoxCollider2D col)
			{
				List<Vector2> list = new List<Vector2>();
				Vector2 vector = col.size * 0.5f;
				Vector2 vector2 = -vector;
				Vector2 vector3 = new Vector2(0f - vector.x, vector.y);
				Vector2 vector4 = new Vector2(vector.x, 0f - vector.y);
				Vector2 vector5 = vector;
				list.Add(vector2 + col.offset);
				list.Add(vector3 + col.offset);
				list.Add(vector5 + col.offset);
				list.Add(vector4 + col.offset);
				list.Add(vector2 + col.offset);
				for (int i = 0; i < list.Count; i++)
				{
					Vector3 vector6 = col.transform.TransformPoint(list[i]);
					list[i] = vector6;
				}
				return list;
			}

			private List<Vector2> GetPointsFromCollider(CircleCollider2D col)
			{
				List<Vector2> list = new List<Vector2>();
				Vector2 vector = col.offset + (Vector2)col.transform.position;
				float radius = col.radius;
				int num = 16;
				float num2 = (float)Math.PI * 2f / (float)num;
				for (int i = 0; i < num; i++)
				{
					float f = num2 * (float)i;
					float num3 = Mathf.Sin(f);
					float num4 = Mathf.Cos(f);
					float x = num4 - num3;
					float y = num3 + num4;
					Vector2 vector2 = new Vector2(x, y);
					vector2.x = vector2.x * radius * col.transform.localScale.x;
					vector2.y = vector2.y * radius * col.transform.localScale.y;
					list.Add(vector2 + vector);
				}
				float num5 = Mathf.Sin(0f);
				float num6 = Mathf.Cos(0f);
				float x2 = num6 - num5;
				float y2 = num5 + num6;
				Vector2 vector3 = new Vector2(x2, y2);
				vector3.x = vector3.x * radius * col.transform.localScale.x;
				vector3.y = vector3.y * radius * col.transform.localScale.y;
				list.Add(vector3 + vector);
				return list;
			}

			private List<Vector2> GetPointsFromCollider(PolygonCollider2D col)
			{
				List<Vector2> list = col.points.ToList();
				list.Add(col.points[0]);
				for (int i = 0; i < list.Count; i++)
				{
					Vector3 vector = col.transform.TransformPoint((Vector3)list[i] + (Vector3)col.offset);
					list[i] = vector;
				}
				return list;
			}

			private List<Vector2> GetPointsFromCollider(EdgeCollider2D col)
			{
				List<Vector2> list = col.points.ToList();
				for (int i = 0; i < list.Count; i++)
				{
					Vector3 vector = col.transform.TransformPoint((Vector3)list[i] + (Vector3)col.offset);
					list[i] = vector;
				}
				return list;
			}

			private List<Vector2> GetPointsHelper(Collider2D collider)
			{
				if ((bool)(collider as PolygonCollider2D))
				{
					return GetPointsFromCollider(collider as PolygonCollider2D);
				}
				if ((bool)(collider as EdgeCollider2D))
				{
					return GetPointsFromCollider(collider as EdgeCollider2D);
				}
				if ((bool)(collider as CircleCollider2D))
				{
					return GetPointsFromCollider(collider as CircleCollider2D);
				}
				if ((bool)(collider as BoxCollider2D))
				{
					return GetPointsFromCollider(collider as BoxCollider2D);
				}
				return new List<Vector2>();
			}

			private IEnumerator DebugInput()
			{
				bool suspended = false;
				while (true)
				{
					if (suspended)
					{
						Time.timeScale = 0f;
					}
					if (Input.GetKeyDown(KeyCode.Slash))
					{
						foreach (KeyValuePair<Collider2D, LineRenderer> line in lines)
						{
							line.Value.GetComponent<Renderer>().enabled = !line.Value.GetComponent<Renderer>().enabled;
						}
						foreach (KeyValuePair<Collider2D, TextMesh> label in labels)
						{
							label.Value.gameObject.SetActive(!label.Value.gameObject.activeInHierarchy);
						}
					}
					if (Input.GetKeyDown(KeyCode.Backslash))
					{
						renderDisabledColliders = !renderDisabledColliders;
					}
					if (!forceSingleEntry && Input.GetKeyDown(KeyCode.Q))
					{
						forceSingleEntry = true;
						Time.timeScale = 0f;
						suspended = true;
					}
					if (!forceSingleEntry && Input.GetKeyDown(KeyCode.W))
					{
						forceSingleEntry = true;
						Time.timeScale = 1f;
						yield return new WaitForEndOfFrame();
						yield return new WaitForEndOfFrame();
						Time.timeScale = 0f;
					}
					if (!forceSingleEntry && Input.GetKey(KeyCode.R))
					{
						forceSingleEntry = true;
						Time.timeScale = 1f;
						yield return new WaitForEndOfFrame();
						yield return new WaitForEndOfFrame();
						Time.timeScale = 0f;
					}
					if (!forceSingleEntry && Input.GetKeyDown(KeyCode.E))
					{
						forceSingleEntry = true;
						Time.timeScale = 1f;
						suspended = false;
					}
					yield return new WaitForEndOfFrame();
					forceSingleEntry = false;
				}
			}

			private void OnEnable()
			{
				if (runDebugInput && debugInput == null)
				{
					debugInput = DebugInput();
					StartCoroutine(debugInput);
				}
			}

			private void OnDisable()
			{
				if (runDebugInput && debugInput != null)
				{
					StopCoroutine(debugInput);
					debugInput = null;
				}
			}

			private IEnumerator Start()
			{
				boxColliders = GetComponentsInChildren<BoxCollider2D>(includeInactive: true).ToList();
				circleColliders = GetComponentsInChildren<CircleCollider2D>(includeInactive: true).ToList();
				polyColliders = GetComponentsInChildren<PolygonCollider2D>(includeInactive: true).ToList();
				edgeColliders = GetComponentsInChildren<EdgeCollider2D>(includeInactive: true).ToList();
				lineRoot = new GameObject("DebugCollider Lines");
				lineRoot.transform.SetParent(base.transform);
				lineRoot.transform.position = Vector3.zero;
				lines = new Dictionary<Collider2D, LineRenderer>();
				labels = new Dictionary<Collider2D, TextMesh>();
				colliderLineMat = new Material(Shader.Find("Diffuse"));
				triggerLineMat = new Material(Shader.Find("Diffuse"));
				disabledLineMat = new Material(Shader.Find("Diffuse"));
				gameObjectDisabledLineMat = new Material(Shader.Find("Diffuse"));
				CreateColliders();
				while (true)
				{
					UpdateColliders();
					yield return new WaitForEndOfFrame();
				}
			}

			private void CreateColliders()
			{
				CreateLinesFromColliders(boxColliders);
				CreateLinesFromColliders(circleColliders);
				CreateLinesFromColliders(polyColliders);
				CreateLinesFromColliders(edgeColliders);
			}

			private void CreateLinesFromColliders<T>(List<T> colliders) where T : Collider2D
			{
				foreach (T collider in colliders)
				{
					string text = "[" + collider.gameObject.name + ",  " + collider.GetType().Name + "]";
					GameObject gameObject = new GameObject(text);
					gameObject.transform.SetParent(lineRoot.transform);
					gameObject.transform.position = Vector3.zero;
					Color lineColor = GetLineColor(collider);
					LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
					Vector3[] array = (from x in GetPointsHelper(collider)
						select new Vector3(x.x, x.y, zDepth)).ToArray();
					lineRenderer.positionCount = array.Length;
					lineRenderer.SetPositions(array);
					lineRenderer.startWidth = lineWidth;
					lineRenderer.endWidth = lineWidth;
					lineRenderer.sharedMaterial = GetLineMaterial(collider);
					lineRenderer.startColor = lineColor;
					lineRenderer.endColor = lineColor;
					lines.Add(collider, lineRenderer);
					GameObject obj = new GameObject(text + " LABEL");
					obj.transform.SetParent(gameObject.transform);
					obj.transform.localScale = Vector3.one * 0.25f;
					TextMesh textMesh = gameObject.gameObject.AddComponent<TextMesh>();
					textMesh.text = text;
					textMesh.fontSize = 14;
					textMesh.color = Color.white;
					textMesh.alignment = TextAlignment.Center;
					labels.Add(collider, textMesh);
				}
			}

			private Color GetLineColor(Collider2D col)
			{
				if (!col.gameObject.activeInHierarchy)
				{
					return gameObjectDisabledColor;
				}
				if (!col.enabled)
				{
					return disabledColor;
				}
				if (col.isTrigger)
				{
					return triggerColor;
				}
				return colliderColor;
			}

			private Material GetLineMaterial(Collider2D col)
			{
				if (!col.gameObject.activeInHierarchy)
				{
					return gameObjectDisabledLineMat;
				}
				if (!col.enabled)
				{
					return disabledLineMat;
				}
				if (col.isTrigger)
				{
					return triggerLineMat;
				}
				return colliderLineMat;
			}

			private void UpdateColliders()
			{
				float num = 0f;
				foreach (KeyValuePair<Collider2D, LineRenderer> line in lines)
				{
					if (line.Key == null)
					{
						continue;
					}
					if (!renderDisabledColliders && (!line.Key.enabled || !line.Key.gameObject.activeInHierarchy))
					{
						line.Value.enabled = false;
						continue;
					}
					line.Value.sharedMaterial = GetLineMaterial(line.Key);
					Color lineColor = GetLineColor(line.Key);
					line.Value.startColor = lineColor;
					line.Value.endColor = lineColor;
					if ((bool)line.Value.GetComponent<Renderer>())
					{
						line.Value.GetComponent<Renderer>().sharedMaterial.color = lineColor;
					}
					labels[line.Key].color = lineColor;
					labels[line.Key].transform.position = line.Key.bounds.max;
					if ((bool)(line.Key as CircleCollider2D))
					{
						labels[line.Key].transform.position = Vector3.one * (line.Key as CircleCollider2D).radius;
					}
					foreach (KeyValuePair<Collider2D, TextMesh> label in labels)
					{
						if (Mathf.Abs(label.Value.transform.position.y - labels[line.Key].transform.position.y) < 0.2f)
						{
							labels[line.Key].transform.Translate(new Vector3(0f, num, 0f));
							num -= 0.2f;
							break;
						}
					}
					Vector3 position = new Vector3(labels[line.Key].transform.position.x, labels[line.Key].transform.position.y, -0.1f);
					labels[line.Key].transform.position = position;
					labels[line.Key].transform.localScale = Vector3.one * 0.2f;
					line.Value.enabled = true;
					LineRenderer value = line.Value;
					Vector3[] positions = (from x in GetPointsHelper(line.Key)
						select new Vector3(x.x, x.y, zDepth)).ToArray();
					value.SetPositions(positions);
				}
			}
		}
		public static class GameObjectExtensions
		{
			public static bool FindAndDestroyGameObjectInChildren(this GameObject gameObject, string name)
			{
				bool result = false;
				GameObject gameObject2 = gameObject.FindGameObjectInChildren(name);
				if (gameObject2 != null)
				{
					UnityEngine.Object.Destroy(gameObject2);
					result = true;
				}
				return result;
			}

			public static GameObject FindGameObjectNameContainsInChildren(this GameObject gameObject, string name)
			{
				if (gameObject == null)
				{
					return null;
				}
				Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>(includeInactive: true);
				foreach (Transform transform in componentsInChildren)
				{
					if (transform.name.Contains(name))
					{
						return transform.gameObject;
					}
				}
				return null;
			}

			public static string PrintSceneHierarchyPath(this GameObject gameObject)
			{
				if (gameObject == null)
				{
					return "WARNING: NULL GAMEOBJECT";
				}
				string result = gameObject.name;
				if (gameObject.transform.parent != null)
				{
					result = gameObject.transform.parent.gameObject.PrintSceneHierarchyPath() + "\\" + gameObject.name;
				}
				return result;
			}
		}
		public static class MiscMethods
		{
			private static System.Random _rand = new System.Random();

			public static GameObject FindGameObject(this Scene scene, string name)
			{
				if (!scene.IsValid())
				{
					return null;
				}
				GameObject[] rootGameObjects = scene.GetRootGameObjects();
				try
				{
					GameObject[] array = rootGameObjects;
					foreach (GameObject gameObject in array)
					{
						if (!(gameObject == null))
						{
							GameObject gameObject2 = gameObject.FindGameObjectInChildren(name);
							if (gameObject2 != null)
							{
								return gameObject2;
							}
							continue;
						}
						break;
					}
				}
				catch (Exception ex)
				{
					Modding.Logger.Log("Exception: " + ex.Message);
				}
				return null;
			}

			public static Func<IEnumerator> ChooseAttack(List<Func<IEnumerator>> attLst, Dictionary<Func<IEnumerator>, int> rep, Dictionary<Func<IEnumerator>, int> max)
			{
				List<Func<IEnumerator>> list = new List<Func<IEnumerator>>(attLst);
				Func<IEnumerator> func = list[_rand.Next(0, list.Count)];
				list.Remove(func);
				while (func != null && list.Count > 0 && rep[func] >= max[func])
				{
					func = list[_rand.Next(0, list.Count)];
					list.Remove(func);
				}
				if (func == null)
				{
					return null;
				}
				if (list.Count == 0 && rep[func] >= max[func])
				{
					foreach (Func<IEnumerator> item in attLst.Where((Func<IEnumerator> x) => x != null))
					{
						rep[item] = 0;
					}
					func = attLst[_rand.Next(0, attLst.Count)];
				}
				if (func != null)
				{
					rep[func]++;
				}
				return func;
			}
		}
		public class SuperDashCancel : MonoBehaviour
		{
			private void OnTriggerEnter2D(Collider2D other)
			{
				HeroController.instance.CancelSuperDash();
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		public class ThroneFlash : MonoBehaviour
		{
			public GameObject throne;

			private GameObject appearEffect;

			private Animator animator;

			private void Awake()
			{
				appearEffect = base.transform.Find("Throne First Appear").gameObject;
				animator = appearEffect.GetComponent<Animator>();
				BoxCollider2D boxCollider2D = base.gameObject.AddComponent<BoxCollider2D>();
				boxCollider2D.offset = Vector2.zero;
				boxCollider2D.size = new Vector2(9f, 15f);
				boxCollider2D.isTrigger = true;
				appearEffect.SetActive(value: false);
			}

			private void OnTriggerEnter2D(Collider2D other)
			{
				if (FiveKnights.Instance.SaveSettings.UnlockedChampionsCall && !FiveKnights.Instance.SaveSettings.SeenChampionsCall)
				{
					FiveKnights.Instance.SaveSettings.SeenChampionsCall = true;
					appearEffect.SetActive(value: true);
					throne.SetActive(value: false);
					StartCoroutine(FlashRoutine());
				}
			}

			private IEnumerator FlashRoutine()
			{
				animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
				yield return new WaitForSeconds(1.5f);
				FiveKnights.preloadedGO["ThroneCovered"].SetActive(value: false);
				throne.SetActive(value: true);
				GameObject.Find("throne").transform.Find("core_extras_0029_wp").gameObject.SetActive(FiveKnights.Instance.SaveSettings.UnlockedChampionsCall && FiveKnights.Instance.SaveSettings.SeenChampionsCall);
				yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
				animator.enabled = false;
			}
		}
		public static class WhiteLadyDialogue
		{
			private static LanguageCtrl langCtrl;

			private const string QueenScene = "Room_Queen";

			private static PlayMakerFSM queenDialogue;

			private static DialogueBox dialogue;

			private const string Sheet = "Minor NPC";

			public static void Hook()
			{
				langCtrl = new LanguageCtrl();
				ModHooks.LanguageGetHook += LangGet;
				UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ActiveSceneChanged;
			}

			public static void Unhook()
			{
				ModHooks.LanguageGetHook -= LangGet;
				UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= ActiveSceneChanged;
			}

			private static string LangGet(string key, string sheet, string orig)
			{
				if (key.StartsWith("WL_"))
				{
					sheet = "Minor NPC";
				}
				if (!langCtrl.ContainsKey(key, sheet))
				{
					return orig;
				}
				return langCtrl.Get(key, sheet);
			}

			private static void ActiveSceneChanged(Scene prev, Scene curr)
			{
				if (!(curr.name != "Room_Queen"))
				{
					queenDialogue = GameObject.Find("Queen").LocateMyFSM("Conversation Control");
					dialogue = queenDialogue.GetAction<HutongGames.PlayMaker.Actions.CallMethodProper>("Repeat", 1).gameObject.GameObject.Value.GetComponent<DialogueBox>();
					string[] obj = new string[3] { "Repeat", "Kingsoul Repeat", "Shadecharm Repeat" };
					FsmState state = queenDialogue.CreateState("PCDialogue");
					state.InsertMethod(CheckAndDoCharmDialogue, 0);
					SFCore.Utils.FsmUtil.ChangeTransition(queenDialogue, "Convo Choice", "FINISHED", "PCDialogue");
					SFCore.Utils.FsmUtil.ChangeTransition(queenDialogue, "Convo Choice", "KINGSOUL", "PCDialogue");
					SFCore.Utils.FsmUtil.ChangeTransition(queenDialogue, "Convo Choice", "SHADECHARM", "PCDialogue");
					SFCore.Utils.FsmUtil.AddTransition(state, "CONVO_FINISH", "Flower?");
					string[] array = obj;
					foreach (string toState in array)
					{
						SFCore.Utils.FsmUtil.AddTransition(state, "ELSE", toState);
					}
				}
			}

			private static void CheckAndDoCharmDialogue()
			{
				bool flag = false;
				if (FiveKnights.Instance.SaveSettings.upgradedCharm_10 && PlayerData.instance.equippedCharm_10)
				{
					if (!FiveKnights.Instance.SaveSettings.WLadyKHConvo1)
					{
						flag = (FiveKnights.Instance.SaveSettings.WLadyKHConvo1 = true);
						dialogue.StartConversation("WL_KH_1", "Minor NPC");
					}
					else if (!FiveKnights.Instance.SaveSettings.WLadyKHConvo2)
					{
						flag = (FiveKnights.Instance.SaveSettings.WLadyKHConvo2 = true);
						dialogue.StartConversation("WL_KH_2", "Minor NPC");
					}
				}
				if (!flag && FiveKnights.Instance.SaveSettings.equippedCharms[0])
				{
					if (!FiveKnights.Instance.SaveSettings.WLadyMPConvo1)
					{
						flag = (FiveKnights.Instance.SaveSettings.WLadyMPConvo1 = true);
						dialogue.StartConversation("WL_MP_1", "Minor NPC");
					}
					else if (!FiveKnights.Instance.SaveSettings.WLadyMPConvo2)
					{
						flag = (FiveKnights.Instance.SaveSettings.WLadyMPConvo2 = true);
						dialogue.StartConversation("WL_MP_2", "Minor NPC");
					}
				}
				if (!flag && FiveKnights.Instance.SaveSettings.equippedCharms[2])
				{
					if (!FiveKnights.Instance.SaveSettings.WLadyBHConvo1)
					{
						flag = (FiveKnights.Instance.SaveSettings.WLadyBHConvo1 = true);
						dialogue.StartConversation("WL_BH_1", "Minor NPC");
					}
					else if (!FiveKnights.Instance.SaveSettings.WLadyBHConvo2)
					{
						flag = (FiveKnights.Instance.SaveSettings.WLadyBHConvo2 = true);
						dialogue.StartConversation("WL_BH_2", "Minor NPC");
					}
				}
				if (!flag && FiveKnights.Instance.SaveSettings.equippedCharms[1])
				{
					if (!FiveKnights.Instance.SaveSettings.WLadyVLConvo1)
					{
						flag = (FiveKnights.Instance.SaveSettings.WLadyVLConvo1 = true);
						dialogue.StartConversation("WL_VL_1", "Minor NPC");
					}
					else if (!FiveKnights.Instance.SaveSettings.WLadyVLConvo2)
					{
						flag = (FiveKnights.Instance.SaveSettings.WLadyVLConvo2 = true);
						dialogue.StartConversation("WL_VL_2", "Minor NPC");
					}
				}
				if (!flag && FiveKnights.Instance.SaveSettings.equippedCharms[0] && FiveKnights.Instance.SaveSettings.equippedCharms[1] && FiveKnights.Instance.SaveSettings.equippedCharms[2] && FiveKnights.Instance.SaveSettings.upgradedCharm_10 && PlayerData.instance.equippedCharm_10)
				{
					if (!FiveKnights.Instance.SaveSettings.WLadyAllConvo1)
					{
						flag = (FiveKnights.Instance.SaveSettings.WLadyAllConvo1 = true);
						dialogue.StartConversation("WL_ALL_1", "Minor NPC");
					}
					else if (!FiveKnights.Instance.SaveSettings.WLadyAllConvo2)
					{
						flag = (FiveKnights.Instance.SaveSettings.WLadyAllConvo2 = true);
						dialogue.StartConversation("WL_ALL_2", "Minor NPC");
					}
				}
				if (!flag)
				{
					queenDialogue.SendEvent("ELSE");
				}
			}

			private static void Log(object o)
			{
				Modding.Logger.Log($"[White Lady]: {o}");
			}
		}
	}
	namespace FiveKnights.Isma
	{
		public class AFistFlash : MonoBehaviour
		{
			private enum FlashState
			{
				Increase,
				Stay,
				Decrease,
				Stop
			}

			private class HitCheck : MonoBehaviour
			{
				public event Action OnHit;

				private void OnTriggerEnter2D(Collider2D col)
				{
					if (col.gameObject.layer == 17)
					{
						OnHit?.Invoke();
					}
				}
			}

			private float amount;

			private float t;

			private float timeUp;

			private float stayTime;

			private float timeDown;

			private float flashTimer;

			private bool flashing;

			private FlashState flashingState;

			private float amountCurrent;

			private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");

			private SpriteRenderer _sr;

			private void Start()
			{
				_sr = GetComponent<SpriteRenderer>();
				_sr.material = FiveKnights.Materials["flash"];
				PolygonCollider2D[] componentsInChildren = base.transform.GetComponentsInChildren<PolygonCollider2D>(includeInactive: true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].gameObject.AddComponent<HitCheck>().OnHit += ResetValues;
				}
			}

			private void ResetValues()
			{
				amount = 0.85f;
				timeUp = 0.01f;
				stayTime = 0.01f;
				timeDown = 0.15f;
				_sr.material.SetFloat(FlashAmount, 0f);
				flashingState = FlashState.Increase;
				flashTimer = 0f;
			}

			private void Update()
			{
				if (flashingState == FlashState.Increase)
				{
					if (flashTimer < timeUp)
					{
						flashTimer += Time.deltaTime;
						t = flashTimer / timeUp;
						amountCurrent = Mathf.Lerp(0f, amount, t);
						_sr.material.SetFloat(FlashAmount, amountCurrent);
					}
					else
					{
						_sr.material.SetFloat(FlashAmount, amount);
						flashTimer = 0f;
						flashing = false;
						flashingState = FlashState.Stay;
					}
				}
				if (flashingState == FlashState.Stay)
				{
					if (flashTimer < stayTime)
					{
						flashTimer += Time.deltaTime;
					}
					else
					{
						flashTimer = 0f;
						flashingState = FlashState.Decrease;
					}
				}
				if (flashingState == FlashState.Decrease)
				{
					if (flashTimer < timeDown)
					{
						flashTimer += Time.deltaTime;
						t = flashTimer / timeDown;
						amountCurrent = Mathf.Lerp(amount, 0f, t);
						_sr.material.SetFloat(FlashAmount, amountCurrent);
					}
					else
					{
						_sr.material.SetFloat(FlashAmount, 0f);
						flashTimer = 0f;
						flashingState = FlashState.Stop;
					}
				}
			}
		}
		internal class DungBall : MonoBehaviour
		{
			private MusicPlayer _ap;

			private bool _hit;

			public GameObject particles;

			public bool usingThornPillars;

			public float LeftX = 67f;

			public float RightX = 85f;

			private void Awake()
			{
				base.gameObject.name = "IsmaHitBall";
				base.gameObject.transform.localScale *= 1.4f;
				base.gameObject.layer = 11;
				DamageHero damageHero = base.gameObject.AddComponent<DamageHero>();
				damageHero.damageDealt = 1;
				damageHero.hazardType = 1;
				damageHero.shadowDashHazard = false;
			}

			private void Start()
			{
				GameObject value = HeroController.instance.gameObject.LocateMyFSM("Spell Control").GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>("Fireball 2", 3).gameObject.Value.LocateMyFSM("Fireball Cast").GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Cast Right", 3).audioPlayer.Value;
				_ap = new MusicPlayer
				{
					Volume = 1f,
					Player = value,
					MaxPitch = 1f,
					MinPitch = 1f,
					Spawn = base.gameObject
				};
			}

			private void FixedUpdate()
			{
				if (!_hit && base.gameObject.transform.GetPositionY() < 7.4f)
				{
					if (EnemyPlantSpawn.PillarCount < 3 && base.transform.position.x > LeftX && base.transform.position.x < RightX && !usingThornPillars)
					{
						_ap.Clip = FiveKnights.Clips["IsmaAudDungBreak"];
						_ap.DoPlayRandomClip();
						GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Plant"]);
						obj.name = "PillarEnemy";
						obj.transform.position = new Vector3(base.transform.position.x, 6.1f, 0.1f);
						obj.AddComponent<EnemyPlantSpawn.PillarMinion>();
					}
					StartCoroutine(DelayedKill());
					_hit = true;
					particles.transform.position = base.transform.position;
					particles.GetComponent<ParticleSystem>().Play();
				}
			}

			private IEnumerator DelayedKill()
			{
				yield return new WaitForSeconds(1.5f);
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		public class EnemyPlantSpawn : MonoBehaviour
		{
			public class PillarMinion : MonoBehaviour
			{
				private bool dying;

				private void Awake()
				{
					base.gameObject.AddComponent<PlantCtrl>();
					PillarCount++;
					IsmaController.offsetTime += 0.1f;
				}

				private IEnumerator Start()
				{
					yield return new WaitWhile(() => GetComponent<HealthManager>() == null);
					GetComponent<HealthManager>().OnDeath += delegate
					{
						StartCoroutine(PillarDeath(0f));
					};
					if (!OWArenaFinder.IsInOverWorld && (base.transform.position.x < 60.3f || base.transform.position.x > 90.6f))
					{
						GetComponent<HealthManager>().SendDeathEvent();
					}
					StartCoroutine(PillarDeath(15f));
				}

				private IEnumerator PillarDeath(float delay)
				{
					if (dying)
					{
						yield break;
					}
					yield return new WaitForSeconds(delay);
					if (!dying)
					{
						dying = true;
						UnityEngine.Object.Destroy(GetComponent<BoxCollider2D>());
						if (base.transform.Find("PillarPogo") != null)
						{
							UnityEngine.Object.Destroy(base.transform.Find("PillarPogo").gameObject);
						}
						Animator anim = GetComponent<Animator>();
						anim.Play("PlantDie");
						yield return null;
						yield return new WaitWhile(() => anim.IsPlaying());
						PillarCount--;
						IsmaController.offsetTime -= 0.1f;
						UnityEngine.Object.Destroy(base.gameObject);
					}
				}

				private void Update()
				{
					if (IsmaController.eliminateMinions && !dying)
					{
						StartCoroutine(PillarDeath(0f));
					}
				}
			}

			public class FoolMinion : MonoBehaviour
			{
				private GameObject initFool;

				private GameObject finalFool;

				private HealthManager hm;

				private BoxCollider2D bc;

				private const float InitY = 6.01f;

				private const float FinalY = 8.61f;

				private readonly float OffsetY = ((CustomWP.boss == CustomWP.Boss.Isma && !OWArenaFinder.IsInOverWorld) ? 0.57f : 0f);

				private float xPos;

				private void Awake()
				{
					initFool = base.transform.Find("initFoolEnemy").gameObject;
					finalFool = base.transform.Find("finalFoolEnemy").gameObject;
					hm = finalFool.GetComponent<HealthManager>();
					RemoveGeo(hm);
					xPos = base.transform.position.x;
					FoolCount++;
					IsmaController.offsetTime += 0.1f;
					hm.OnDeath += delegate
					{
						IsmaController.offsetTime -= 0.1f;
						FoolCount--;
					};
				}

				private IEnumerator Start()
				{
					initFool.SetActive(value: true);
					hm.IsInvincible = true;
					Animator component = initFool.GetComponent<Animator>();
					initFool.transform.SetPosition2D(xPos, 6.01f + OffsetY);
					initFool.transform.localScale *= 1.4f;
					yield return component.PlayBlocking("SpawnFool");
					UnityEngine.Object.Destroy(initFool);
					finalFool.transform.SetPosition2D(xPos, 8.61f + OffsetY);
					tk2dSpriteAnimator tk = finalFool.GetComponent<tk2dSpriteAnimator>();
					PlayMakerFSM fsm = finalFool.LocateMyFSM("Plant Trap Control");
					fsm.enabled = false;
					finalFool.SetActive(value: true);
					tk.Play("Retract");
					yield return new WaitWhile(delegate
					{
						bc = finalFool.GetComponent<BoxCollider2D>();
						if (bc != null)
						{
							bc.enabled = false;
						}
						return tk.IsPlaying("Retract");
					});
					fsm.enabled = true;
					fsm.SetState("Init");
					fsm.GetAction<HutongGames.PlayMaker.Actions.Wait>("Ready", 2).time = 0.55f;
					hm.IsInvincible = false;
				}

				private void Update()
				{
					if (IsmaController.killAllMinions)
					{
						UnityEngine.Object.Destroy(base.gameObject);
					}
				}

				private void OnDestroy()
				{
					if (finalFool != null && hm != null)
					{
						hm.Die(0f, AttackTypes.Nail, ignoreEvasion: true);
					}
				}
			}

			public class GulkaMinion : MonoBehaviour
			{
				private GameObject initGulka;

				public GameObject finalGulka;

				private GameObject cover;

				private GameObject under;

				private HealthManager hm;

				private Vector2 pos;

				private readonly float LEFT = (OWArenaFinder.IsInOverWorld ? 105.55f : 60.27f);

				private readonly float RIGHT = (OWArenaFinder.IsInOverWorld ? 137.02f : 91.73f);

				private readonly float MIDDLE = (OWArenaFinder.IsInOverWorld ? 120f : 75f);

				private void Awake()
				{
					initGulka = base.transform.Find("initTurretEnemy").gameObject;
					pos = base.transform.position;
					TurretCount++;
					IsmaController.offsetTime += 0.1f;
				}

				private IEnumerator Start()
				{
					hm = finalGulka.GetComponent<HealthManager>();
					hm.GetComponent<HealthManager>().hp = 1;
					RemoveGeo(hm);
					hm.OnDeath += delegate
					{
						IsmaController.offsetTime -= 0.1f;
						TurretCount--;
						GameManager.instance.StartCoroutine(CorpseDropThroughFloor());
						UnityEngine.Object.Destroy(cover);
						UnityEngine.Object.Destroy(under);
						UnityEngine.Object.Destroy(base.gameObject);
					};
					initGulka.SetActive(value: true);
					Animator anim = initGulka.GetComponent<Animator>();
					float rotation = ((pos.x > MIDDLE) ? 90f : (-90f));
					initGulka.transform.SetPosition2D((pos.x > MIDDLE) ? (RIGHT + 0.19f) : (LEFT - 0.19f), pos.y);
					initGulka.transform.localScale *= 1.4f;
					initGulka.transform.SetRotation2D(rotation);
					MeshRenderer mesh = finalGulka.GetComponent<MeshRenderer>();
					UnityEngine.Object.Destroy(finalGulka.GetComponent<SetZ>());
					PlayMakerFSM fsm = finalGulka.LocateMyFSM("Plant Turret");
					GameObject value = fsm.GetAction<HutongGames.PlayMaker.Actions.CreateObject>("Fire", 3).gameObject.Value;
					if (!value.GetComponent<ModifiedSpit>())
					{
						value.AddComponent<ModifiedSpit>();
					}
					if (!isPhase2)
					{
						fsm.GetAction<HutongGames.PlayMaker.Actions.Wait>("Idle Anim", 1).time.Value = 1.1f;
					}
					mesh.enabled = false;
					List<MeshRenderer> lst = new List<MeshRenderer>();
					MeshRenderer[] componentsInChildren = finalGulka.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
					foreach (MeshRenderer meshRenderer in componentsInChildren)
					{
						meshRenderer.enabled = false;
						lst.Add(meshRenderer);
					}
					fsm.FsmVariables.FindFsmFloat("Distance Max").Value = 50f;
					finalGulka.transform.SetRotation2D(rotation);
					finalGulka.SetActive(value: true);
					finalGulka.transform.position = new Vector3((pos.x > MIDDLE) ? (RIGHT - 0.29f) : (LEFT + 0.29f), pos.y, -0.09f);
					cover = fsm.GetFsmGameObjectVariable("Cover").Value;
					under = fsm.GetFsmGameObjectVariable("Under").Value;
					cover.transform.position = new Vector3((pos.x > MIDDLE) ? (RIGHT - 0.16f) : (LEFT + 0.16f), pos.y, -0.1f);
					under.transform.position = new Vector3((pos.x > MIDDLE) ? (RIGHT - 0.19f) : (LEFT + 0.19f), pos.y, -0.08f);
					anim.Play("SpawnGulka");
					yield return new WaitForSeconds(0.05f);
					yield return new WaitWhile(() => anim.IsPlaying());
					UnityEngine.Object.Destroy(initGulka);
					mesh.enabled = true;
					foreach (MeshRenderer item in lst)
					{
						item.enabled = true;
					}
					fsm.SendEvent("SEE HERO");
				}

				private void Update()
				{
					if (IsmaController.killAllMinions)
					{
						hm.Die(0f, AttackTypes.Nail, ignoreEvasion: true);
						UnityEngine.Object.Destroy(base.gameObject);
					}
				}

				private IEnumerator CorpseDropThroughFloor()
				{
					yield return null;
					GameObject corpse = GameObject.Find("Corpse Plant Turret(Clone)");
					corpse.name = "Corpse Plant Turret that will fall through the floor soon";
					yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 6f));
					Collider2D[] componentsInChildren = corpse.GetComponentsInChildren<Collider2D>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].enabled = false;
					}
					if ((bool)corpse.GetComponent<Rigidbody2D>())
					{
						corpse.GetComponent<Rigidbody2D>().isKinematic = false;
					}
					yield return new WaitForSeconds(1f);
					UnityEngine.Object.Destroy(corpse);
				}
			}

			private class ModifiedSpit : MonoBehaviour
			{
				private PlayMakerFSM _fsmEnemyDmg;

				private PlayMakerFSM _fsmSpit;

				private void Awake()
				{
					base.gameObject.layer = 12;
					_fsmEnemyDmg = base.gameObject.LocateMyFSM("damages_enemy");
					_fsmEnemyDmg.enabled = false;
					_fsmSpit = base.gameObject.LocateMyFSM("spike ball control");
					_fsmSpit.GetAction<HutongGames.PlayMaker.Actions.Trigger2dEventLayer>("Idle", 1).trigger = PlayMakerUnity2d.Trigger2DType.OnTriggerStay2D;
				}

				private void Start()
				{
					StartCoroutine(WaitForPlayerHit());
				}

				private IEnumerator WaitForPlayerHit()
				{
					yield return new WaitUntil(() => _fsmSpit.ActiveStateName == "Nail Hit");
					base.gameObject.layer = 17;
					_fsmEnemyDmg.enabled = true;
				}
			}

			public static bool isPhase2;

			public static int FoolCount;

			public static int PillarCount;

			public static int TurretCount;

			public const int MAX_TURRET = 2;

			public const int MAX_FOOL = 3;

			public const int MAX_PILLAR = 3;

			private List<GameObject> foolList = new List<GameObject>();

			private const float TIME_INC = 0.1f;

			private readonly float LEFT_X = (OWArenaFinder.IsInOverWorld ? 105f : 60.3f);

			private readonly float RIGHT_X = (OWArenaFinder.IsInOverWorld ? 135f : 90.6f);

			private readonly float MIDDLE = (OWArenaFinder.IsInOverWorld ? 120f : 75f);

			private readonly float GROUND_Y = 6.05f;

			private const string FoolName = "FoolEnemy";

			private const string SpecialName = "SpecialEnemy";

			private const string GulkaName = "TurretEnemy";

			private const string PillarName = "PillarEnemy";

			private bool _hasP2GulkaInit;

			private void OnTriggerEnter2D(Collider2D other)
			{
				if (other.name != "SeedEnemy")
				{
					return;
				}
				if (base.name.Contains("Floor"))
				{
					if (isPhase2)
					{
						Log($"Working with Pillars {PillarCount}");
						if (PillarCount >= 3)
						{
							Log("Oh no too many");
							UnityEngine.Object.Destroy(other.gameObject);
							return;
						}
						SpawnPillar(other.transform.position);
					}
					else
					{
						Log($"Working with Fools {FoolCount}");
						if (FoolCount >= 3)
						{
							Log("Oh no too many");
							UnityEngine.Object.Destroy(other.gameObject);
							return;
						}
						for (int i = 0; i < foolList.Count; i++)
						{
							if (foolList[i] == null)
							{
								foolList.RemoveAt(i);
								i--;
							}
							else if (Mathf.Abs(foolList[i].transform.position.x - other.transform.position.x) < 2f)
							{
								Log("Fool is too close to another fool");
								UnityEngine.Object.Destroy(other.gameObject);
								return;
							}
						}
						SpawnFool(other.transform.position);
					}
				}
				else if (base.name.Contains("Side") && !isPhase2)
				{
					Log($"Working with Gulka {TurretCount}");
					if (TurretCount >= 2)
					{
						Log("Oh no too many");
						UnityEngine.Object.Destroy(other.gameObject);
						return;
					}
					SpawnGulka(other.transform.position);
				}
				UnityEngine.Object.Destroy(other.gameObject);
			}

			public void Phase2Spawn()
			{
				GameObject gameObject = new GameObject();
				gameObject.transform.position = new Vector2(LEFT_X + 10f, GROUND_Y + 14f);
				GameObject gameObject2 = new GameObject();
				gameObject2.transform.position = new Vector2(RIGHT_X - 10f, GROUND_Y + 14f);
				GameObject gameObject3 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["PTurret"]);
				GameObject gameObject4 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["PTurret"]);
				StartCoroutine(Phase2Spawn(gameObject, gameObject3));
				StartCoroutine(Phase2Spawn(gameObject2, gameObject4));
				StartCoroutine(WaitForInitFinish(gameObject3.LocateMyFSM("Plant Turret"), gameObject4.LocateMyFSM("Plant Turret")));
			}

			private IEnumerator WaitForInitFinish(PlayMakerFSM fsm1, PlayMakerFSM fsm2)
			{
				string stopState = "Idle Anim";
				yield return new WaitUntil(() => _hasP2GulkaInit);
				while (fsm1 != null && fsm2 != null)
				{
					PlayMakerFSM playMakerFSM = fsm1;
					PlayMakerFSM playMakerFSM2 = fsm2;
					bool flag = true;
					playMakerFSM2.enabled = true;
					playMakerFSM.enabled = flag;
					fsm1.SetState("Shoot Antic");
					fsm2.SetState("Shoot Antic");
					yield return new WaitUntil(() => fsm1.ActiveStateName == stopState || fsm2.ActiveStateName == stopState);
					if (fsm1.ActiveStateName == stopState)
					{
						fsm1.enabled = false;
						fsm1.GetComponent<tk2dSpriteAnimator>().Play("Idle");
						yield return new WaitUntil(() => fsm2.ActiveStateName == stopState);
						fsm2.enabled = false;
						fsm2.GetComponent<tk2dSpriteAnimator>().Play("Idle");
					}
					else
					{
						fsm2.enabled = false;
						fsm2.GetComponent<tk2dSpriteAnimator>().Play("Idle");
						yield return new WaitUntil(() => fsm1.ActiveStateName == stopState);
						fsm1.enabled = false;
						fsm1.GetComponent<tk2dSpriteAnimator>().Play("Idle");
					}
					yield return new WaitForSeconds(2f + 0.1f * (float)UnityEngine.Random.Range(0, 4));
				}
			}

			private IEnumerator Phase2Spawn(GameObject go, GameObject turret)
			{
				Vector2 pos = go.transform.position;
				yield return new WaitForSeconds(2f);
				GameObject gulk = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Gulka"]);
				gulk.name = "SpecialEnemy";
				Animator anim = gulk.GetComponent<Animator>();
				float num = 180f;
				gulk.transform.SetPosition2D(pos);
				gulk.transform.localScale *= 1.4f;
				gulk.transform.SetRotation2D(num);
				GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Seal"], gulk.transform.position + Vector3.down, Quaternion.identity);
				obj.transform.localScale = new Vector3(2f, 2f, 1f);
				obj.layer = 19;
				obj.AddComponent<GulkaSeal>();
				HealthManager component = turret.GetComponent<HealthManager>();
				RemoveGeo(component);
				turret.name = "SpecialEnemy";
				MeshRenderer mesh = turret.GetComponent<MeshRenderer>();
				PlayMakerFSM fsm = turret.LocateMyFSM("Plant Turret");
				GameObject value = fsm.GetAction<HutongGames.PlayMaker.Actions.CreateObject>("Fire", 3).gameObject.Value;
				if (!value.GetComponent<ModifiedSpit>())
				{
					value.AddComponent<ModifiedSpit>();
				}
				fsm.GetAction<SetInvincible>("Wake", 2).Invincible = true;
				component.IsInvincible = true;
				mesh.enabled = false;
				List<MeshRenderer> lst = new List<MeshRenderer>();
				MeshRenderer[] componentsInChildren = turret.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
				foreach (MeshRenderer meshRenderer in componentsInChildren)
				{
					meshRenderer.enabled = false;
					lst.Add(meshRenderer);
				}
				fsm.FsmVariables.FindFsmFloat("Distance Max").Value = 50f;
				fsm.GetAction<HutongGames.PlayMaker.Actions.Wait>("Idle Anim", 1).time = 1.5f;
				turret.transform.SetRotation2D(num);
				turret.SetActive(value: true);
				num *= (float)Math.PI / 180f;
				turret.transform.SetPosition2D(pos.x - 0.5f * Mathf.Sin(num), pos.y + 0.5f * Mathf.Cos(num));
				anim.Play("SpawnGulka");
				yield return new WaitForSeconds(0.05f);
				yield return new WaitWhile(() => anim.IsPlaying());
				UnityEngine.Object.Destroy(gulk);
				mesh.enabled = true;
				foreach (MeshRenderer item in lst)
				{
					item.enabled = true;
				}
				fsm.SendEvent("SEE HERO");
				fsm.enabled = false;
				_hasP2GulkaInit = true;
				StartCoroutine(FinalGulkaKiller(turret));
			}

			private IEnumerator FinalGulkaKiller(GameObject gulka)
			{
				yield return new WaitWhile(() => !IsmaController.eliminateMinions);
				Log("Killing our special gulka :(");
				gulka.GetComponent<HealthManager>().Die(0f, AttackTypes.Nail, ignoreEvasion: true);
				foreach (tk2dSprite item in new List<tk2dSprite>(UnityEngine.Object.FindObjectsOfType<tk2dSprite>()))
				{
					if (item.gameObject.name == "Cover" || item.gameObject.name == "Under")
					{
						UnityEngine.Object.Destroy(item.gameObject);
					}
				}
			}

			private void SpawnPillar(Vector2 pos)
			{
				GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Plant"]);
				obj.name = "PillarEnemy";
				obj.transform.position = new Vector3(pos.x, 6.1f, 0.1f);
				obj.AddComponent<PillarMinion>();
			}

			private void SpawnFool(Vector2 pos)
			{
				GameObject gameObject = new GameObject("FoolParent");
				gameObject.transform.position = pos;
				GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Fool"], gameObject.transform, worldPositionStays: true);
				obj.name = "initFoolEnemy";
				obj.SetActive(value: false);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["PTrap"], gameObject.transform, worldPositionStays: true);
				gameObject2.name = "finalFoolEnemy";
				gameObject2.SetActive(value: false);
				foolList.Add(gameObject2);
				gameObject.AddComponent<FoolMinion>();
			}

			private void SpawnGulka(Vector2 pos)
			{
				GameObject gameObject = new GameObject("GulkaParent");
				gameObject.transform.position = pos;
				GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Gulka"], gameObject.transform, worldPositionStays: true);
				obj.name = "initTurretEnemy";
				obj.SetActive(value: false);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["PTurret"]);
				gameObject2.name = "finalTurretEnemy";
				gameObject.AddComponent<GulkaMinion>().finalGulka = gameObject2;
			}

			private static void RemoveGeo(HealthManager hm)
			{
				hm.SetGeoLarge(0);
				hm.SetGeoMedium(0);
				hm.SetGeoSmall(0);
			}

			private static void Log(object obj)
			{
				if (FiveKnights.isDebug)
				{
					Modding.Logger.Log("[Enemy Plant] " + obj);
				}
			}
		}
		public class GulkaSeal : MonoBehaviour
		{
			private MusicPlayer _ap;

			private SpriteRenderer _sr;

			private GameObject _pt;

			private Coroutine _flashCoro;

			private void Start()
			{
				GameObject value = HeroController.instance.gameObject.LocateMyFSM("Spell Control").GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>("Fireball 2", 3).gameObject.Value.LocateMyFSM("Fireball Cast").GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Cast Right", 3).audioPlayer.Value;
				_ap = new MusicPlayer
				{
					Volume = 1f,
					Player = value,
					MaxPitch = 0.85f,
					MinPitch = 1.15f,
					Spawn = base.gameObject,
					Clip = ParryTink.TinkClip
				};
				_sr = base.transform.GetComponent<SpriteRenderer>();
				_sr.color = new Color(1f, 1f, 1f, 0f);
				_pt = FiveKnights.preloadedGO["Shield"].transform.Find("Attack Pt").gameObject;
			}

			private void OnTriggerEnter2D(Collider2D other)
			{
				if (other.gameObject.CompareTag("Nail Attack"))
				{
					this.PlayAudio(ParryTink.TinkClip, 1f, 0.15f);
					ParticleSystem.EmissionModule emission = _pt.GetComponent<ParticleSystem>().emission;
					emission.enabled = true;
					if (_flashCoro != null)
					{
						StopCoroutine(_flashCoro);
					}
					_flashCoro = StartCoroutine(Flash());
				}
			}

			private IEnumerator Flash()
			{
				for (int i = 0; i < 100; i++)
				{
					_sr.color = new Color(1f, 1f, 1f, 1f - (float)i / 100f);
					yield return new WaitForSeconds(0.01f);
				}
				_sr.color = new Color(1f, 1f, 1f, 0f);
				ParticleSystem.EmissionModule emission = _pt.GetComponent<ParticleSystem>().emission;
				emission.enabled = false;
			}
		}
		public class IsmaController : MonoBehaviour
		{
			public class AcidGnd : MonoBehaviour
			{
				private Rigidbody2D _rb;

				private PlayMakerFSM _fsm;

				private const float SlimeY = 7.5f;

				private bool _isLanded;

				public void Awake()
				{
					_rb = GetComponent<Rigidbody2D>();
					_fsm = base.gameObject.LocateMyFSM("Vomit Glob");
					_isLanded = false;
					_fsm.GetAction<HutongGames.PlayMaker.Actions.WaitRandom>("Land", 8).timeMin = 0.75f;
					_fsm.GetAction<HutongGames.PlayMaker.Actions.WaitRandom>("Land", 8).timeMax = 1f;
				}

				public void FixedUpdate()
				{
					if (!_isLanded && _fsm.ActiveStateName == "In Air" && base.transform.position.y < 7.5f)
					{
						_isLanded = true;
						this.PlayAudio(FiveKnights.Clips["AcidSpitSnd"], 1f, 0f, HeroController.instance.transform);
						Vector3 position = base.transform.position;
						base.transform.position = new Vector3(position.x, 7.5f, position.z);
						_fsm.SetState("Land");
						UnityEngine.Object.Destroy(this);
					}
				}
			}

			public class DungTracker : MonoBehaviour
			{
				private void OnDisable()
				{
					UnityEngine.Object.Destroy(this);
				}
			}

			public bool onlyIsma;

			private bool _attacking;

			private HealthManager _hm;

			private BoxCollider2D _bc;

			private HealthManager _hmDD;

			private Rigidbody2D _rb;

			private Rigidbody2D _rbDD;

			private EnemyDreamnailReaction _dreamNailReaction;

			private ExtraDamageable _extraDamageable;

			private EnemyHitEffectsUninfected _hitEffects;

			private EnemyDeathEffectsUninfected _deathFx;

			private EnemyDeathEffectsUninfected _ddDeathFx;

			private GameObject _dnailEff;

			private SpriteRenderer _sr;

			private GameObject _target;

			private GameObject dd;

			private PlayMakerFSM _ddFsm;

			private Animator _anim;

			private Texture _acidTexture;

			private System.Random _rand;

			private int _healthPool;

			private bool _waitForHitStart;

			private readonly float LeftX = (OWArenaFinder.IsInOverWorld ? 105f : 60.3f);

			private readonly float RightX = (OWArenaFinder.IsInOverWorld ? 135f : 91.7f);

			private readonly float GroundY = ((OWArenaFinder.IsInOverWorld || CustomWP.boss == CustomWP.Boss.All || CustomWP.boss == CustomWP.Boss.Ogrim) ? 5.9f : 6.67f);

			private readonly int DreamConvoAmount = (OWArenaFinder.IsInOverWorld ? 4 : ((CustomWP.boss == CustomWP.Boss.All || CustomWP.boss == CustomWP.Boss.Ogrim) ? 5 : 3));

			private readonly string DreamConvoKey = (OWArenaFinder.IsInOverWorld ? "ISMA_DREAM" : "ISMA_GG_DREAM");

			private readonly int MaxHP = ((CustomWP.lev > 0) ? 1650 : 1450);

			private readonly int WallHP = ((CustomWP.lev > 0) ? 1150 : 1050);

			private readonly int SpikeHP = ((CustomWP.lev > 0) ? 600 : 550);

			private readonly int MaxHPDuo = ((CustomWP.lev > 0) ? 1800 : 1600);

			private readonly int WallHPDuo = ((CustomWP.lev > 0) ? 1300 : 1150);

			private readonly int SpikeHPDuo = ((CustomWP.lev > 0) ? 700 : 600);

			private readonly int FrenzyHP = 250;

			private const float IDLE_TIME = 0f;

			private const int GULKA_DAMAGE = 20;

			private bool _isDead;

			private bool _isIsmaHitLast;

			private bool _usingThornPillars;

			private bool _ogrimEvaded;

			private Coroutine _musicCoro;

			private Coroutine _wallsCoro;

			public static float offsetTime;

			public static bool killAllMinions;

			public static bool eliminateMinions;

			public bool introDone;

			private static LanguageCtrl _langCtrl;

			private Action _prev;

			private bool _wallActive;

			private GameObject wallR;

			private GameObject wallL;

			private bool firstBomb = true;

			private bool spawningWalls;

			private GameObject arm;

			private GameObject spike;

			private GameObject tentArm;

			private const float WhipYOffset = 2.21f;

			private const float WhipXOffset = 1.13f;

			private GameObject whip;

			private bool _playingVoiceDS;

			private GameObject fakeIsma;

			private float agonyAnimSpd = 1.2f;

			private Vector2 agonySpread = new Vector2(35f, 40f);

			private GameObject agonyThorns;

			private bool usedAgony;

			private bool startedOgrimRage;

			private bool startedIsmaRage;

			private bool startedIsmaDeath;

			private bool preventDamage;

			private List<AudioClip> _randVoice => FiveKnights.Clips.Values.Where((AudioClip x) => x != null && x.name.Contains("IsmaAudAtt")).ToList();

			private float MiddleX => (LeftX + RightX) / 2f;

			private void Awake()
			{
				_langCtrl = new LanguageCtrl();
				_hm = base.gameObject.AddComponent<HealthManager>();
				_sr = base.gameObject.GetComponent<SpriteRenderer>();
				_anim = base.gameObject.GetComponent<Animator>();
				_bc = base.gameObject.GetComponent<BoxCollider2D>();
				_rb = base.gameObject.AddComponent<Rigidbody2D>();
				_rb.gravityScale = 0f;
				_rb.interpolation = RigidbodyInterpolation2D.Interpolate;
				dd = FiveKnights.preloadedGO["WD"];
				_hmDD = dd.GetComponent<HealthManager>();
				_rbDD = dd.GetComponent<Rigidbody2D>();
				_ddFsm = dd.LocateMyFSM("Dung Defender");
				EnemyHPBarImport.DisableHPBar(dd);
				_extraDamageable = base.gameObject.AddComponent<ExtraDamageable>();
				Mirror.SetField(_extraDamageable, "impactClipTable", Mirror.GetField<ExtraDamageable, RandomAudioClipTable>(dd.GetComponent<ExtraDamageable>(), "impactClipTable"));
				Mirror.SetField(_extraDamageable, "audioPlayerPrefab", Mirror.GetField<ExtraDamageable, AudioSource>(dd.GetComponent<ExtraDamageable>(), "audioPlayerPrefab"));
				_dnailEff = dd.GetComponent<EnemyDreamnailReaction>().GetAttr<EnemyDreamnailReaction, GameObject>("dreamImpactPrefab");
				_dreamNailReaction = base.gameObject.AddComponent<EnemyDreamnailReaction>();
				_dreamNailReaction.enabled = true;
				Mirror.SetField(_dreamNailReaction, "convoAmount", DreamConvoAmount);
				_dreamNailReaction.SetConvoTitle(DreamConvoKey);
				_hitEffects = base.gameObject.AddComponent<EnemyHitEffectsUninfected>();
				_hitEffects.enabled = true;
				_deathFx = base.gameObject.AddComponent<EnemyDeathEffectsUninfected>();
				base.gameObject.AddComponent<Flash>();
				_rand = new System.Random();
				_healthPool = 9999;
				EnemyPlantSpawn.isPhase2 = false;
				EnemyPlantSpawn.FoolCount = (EnemyPlantSpawn.PillarCount = (EnemyPlantSpawn.TurretCount = 0));
				killAllMinions = (eliminateMinions = false);
			}

			private IEnumerator Start()
			{
				Log("Begin Isma");
				yield return null;
				_healthPool = (onlyIsma ? MaxHP : MaxHPDuo);
				offsetTime = 0f;
				if (CustomWP.boss == CustomWP.Boss.All)
				{
					StartCoroutine(SilLeave());
					yield return new WaitForSeconds(0.15f);
				}
				if (onlyIsma)
				{
					GameCameras.instance.cameraShakeFSM.FsmVariables.FindFsmBool("RumblingMed").Value = false;
					yield return new WaitForSeconds(0.8f);
				}
				if (!OWArenaFinder.IsInOverWorld)
				{
					GameObject gameObject = new GameObject();
					gameObject.name = "SeedCols";
					GameObject obj = new GameObject();
					obj.name = "SeedFloor";
					obj.transform.position = new Vector3(73.3f, GroundY - 0.8f, 0f);
					BoxCollider2D boxCollider2D = obj.AddComponent<BoxCollider2D>();
					boxCollider2D.offset = new Vector2(3f, 0f);
					boxCollider2D.size = new Vector2(19f, 1f);
					obj.transform.parent = gameObject.transform;
					if (onlyIsma)
					{
						GameObject obj2 = new GameObject();
						obj2.name = "SeedSideR";
						obj2.transform.position = new Vector3(92.4f, GroundY + 6.8f, 0f);
						obj2.AddComponent<BoxCollider2D>().size = new Vector2(1f, 7f);
						obj2.transform.parent = gameObject.transform;
						GameObject obj3 = new GameObject();
						obj3.name = "SeedSideL";
						obj3.transform.position = new Vector3(59.4f, GroundY + 6.8f, 0f);
						obj3.AddComponent<BoxCollider2D>().size = new Vector2(1f, 7f);
						obj3.transform.parent = gameObject.transform;
					}
				}
				tk2dSpriteDefinition currentSpriteDef = FiveKnights.preloadedGO["AcidSpit"].GetComponentInChildren<tk2dSprite>().GetCurrentSpriteDef();
				_acidTexture = currentSpriteDef.material.mainTexture;
				currentSpriteDef.material.mainTexture = FiveKnights.SPRITES["acid_b"].texture;
				foreach (Transform item in GameObject.Find("SeedCols").transform)
				{
					item.gameObject.AddComponent<EnemyPlantSpawn>();
					item.gameObject.layer = 15;
				}
				base.gameObject.layer = 11;
				_target = HeroController.instance.gameObject;
				if (!onlyIsma)
				{
					PositionIsma();
				}
				else
				{
					base.transform.position = new Vector3(LeftX + (RightX - LeftX) / 1.6f, GroundY, 1f);
				}
				FaceHero();
				_bc.enabled = false;
				PlayMakerFSM roarFSM = HeroController.instance.gameObject.LocateMyFSM("Roar Lock");
				if (OWArenaFinder.IsInOverWorld)
				{
					roarFSM.GetFsmGameObjectVariable("Roar Object").Value = base.gameObject;
					roarFSM.SendEvent("ROAR ENTER");
				}
				On.HealthManager.TakeDamage += HealthManagerTakeDamage;
				On.HealthManager.Die += HealthManagerDie;
				On.SpellFluke.DoDamage += SpellFlukeOnDoDamage;
				On.EnemyDreamnailReaction.RecieveDreamImpact += OnReceiveDreamImpact;
				On.HutongGames.PlayMaker.Actions.ReceivedDamage.OnEnter += MarkDungBalls;
				AssignFields(base.gameObject);
				_ddFsm.FsmVariables.FindFsmInt("Rage HP").Value = 801;
				_hm.hp = (_hmDD.hp = (onlyIsma ? MaxHP : MaxHPDuo));
				EnemyHPBarImport.MarkAsBoss(base.gameObject);
				if (!OWArenaFinder.IsInOverWorld && !onlyIsma)
				{
					ToggleIsma(visible: false);
					yield return new WaitForSeconds(1f);
					ToggleIsma(visible: true);
				}
				_anim.Play("Apear");
				base.gameObject.transform.position = new Vector2(base.gameObject.transform.GetPositionX(), GroundY + 8f);
				_rb.velocity = new Vector2(0f, -40f);
				yield return new WaitWhile(() => base.gameObject.transform.GetPositionY() > GroundY);
				GameCameras.instance.cameraShakeFSM.SendEvent("SmallShake");
				_rb.velocity = new Vector2(0f, 0f);
				base.gameObject.transform.position = new Vector2(base.gameObject.transform.GetPositionX(), GroundY);
				yield return new WaitWhile(() => _anim.IsPlaying());
				if (OWArenaFinder.IsInOverWorld)
				{
					yield return new WaitForSeconds(0.75f);
					roarFSM.SendEvent("ROAR EXIT");
				}
				StartCoroutine("Start2");
				if (!OWArenaFinder.IsInOverWorld && !onlyIsma)
				{
					yield return new WaitForSeconds(1f);
					GGBossManager.Instance.PlayMusic(FiveKnights.Clips["OgrismaMusic"], 1f);
				}
				else
				{
					_musicCoro = GameManager.instance.StartCoroutine(MusicControl());
				}
			}

			private IEnumerator MusicControl()
			{
				if (OWArenaFinder.IsInOverWorld)
				{
					OWBossManager.PlayMusic(FiveKnights.Clips["LoneIsmaIntro"]);
					yield return new WaitForSecondsRealtime(FiveKnights.Clips["LoneIsmaIntro"].length);
					OWBossManager.PlayMusic(FiveKnights.Clips["LoneIsmaLoop"]);
				}
				else if (onlyIsma)
				{
					GGBossManager.Instance.PlayMusic(FiveKnights.Clips["LoneIsmaIntro"]);
					yield return new WaitForSecondsRealtime(FiveKnights.Clips["LoneIsmaIntro"].length);
					GGBossManager.Instance.PlayMusic(FiveKnights.Clips["LoneIsmaLoop"]);
				}
			}

			private IEnumerator Start2()
			{
				float dir = FaceHero();
				introDone = true;
				GameObject gameObject = null;
				foreach (GameObject item in from x in UnityEngine.Object.FindObjectsOfType<GameObject>()
					where x.name.Contains("Area Title Holder")
					select x)
				{
					gameObject = item.transform.Find("Area Title").gameObject;
				}
				string text = "Kindly";
				string text2 = "Isma";
				string text3 = "";
				string text4 = "Loyal";
				string text5 = "Ogrim";
				string text6 = "";
				string sheet = "Reward Room";
				if (_langCtrl.ContainsKey("RR_ISMA_TITLE_SUPER", sheet))
				{
					text = _langCtrl.Get("RR_ISMA_TITLE_SUPER", sheet);
					text2 = _langCtrl.Get("RR_ISMA_TITLE_MAIN", sheet);
					text3 = _langCtrl.Get("RR_ISMA_TITLE_SUB", sheet);
				}
				if (_langCtrl.ContainsKey("RR_OGRIM_TITLE_SUPER", sheet))
				{
					text4 = _langCtrl.Get("RR_OGRIM_TITLE_SUPER", sheet);
					text5 = _langCtrl.Get("RR_OGRIM_TITLE_MAIN", sheet);
					text6 = _langCtrl.Get("RR_OGRIM_TITLE_SUB", sheet);
				}
				if (!onlyIsma)
				{
					if (base.transform.position.x > dd.transform.position.x)
					{
						StartCoroutine(ChangeIntroText(gameObject, text2, text3, text, right: true));
					}
					else
					{
						StartCoroutine(ChangeIntroText(gameObject, text5, text6, text4, right: true));
					}
				}
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
				gameObject2.SetActive(value: true);
				if (!onlyIsma && base.transform.position.x > dd.transform.position.x)
				{
					AreaTitleCtrl.ShowBossTitle(this, gameObject2, 2f, "", "", "", text5, text4, text6);
				}
				else
				{
					AreaTitleCtrl.ShowBossTitle(this, gameObject2, 2f, "", "", "", text2, text, text3);
				}
				if (onlyIsma)
				{
					_bc.enabled = true;
				}
				_waitForHitStart = true;
				yield return new WaitForSeconds(0.7f);
				_anim.Play("Bow");
				this.PlayAudio(FiveKnights.Clips["IsmaAudBow"]);
				yield return new WaitForSeconds(0.05f);
				yield return new WaitWhile(() => _anim.IsPlaying());
				yield return new WaitForSeconds(1f);
				_bc.enabled = false;
				_anim.Play("EndBow");
				yield return null;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 2);
				_waitForHitStart = false;
				_rb.velocity = new Vector2(dir * 20f, 10f);
				yield return new WaitWhile(() => _anim.IsPlaying());
				_rb.velocity = Vector2.zero;
				ToggleIsma(visible: false);
				_attacking = false;
				_wallsCoro = StartCoroutine(SpawnWalls());
				if (onlyIsma)
				{
					StartCoroutine(AttackChoice());
				}
				else
				{
					StartCoroutine(DuoAttacks());
				}
			}

			private IEnumerator AttackChoice()
			{
				yield return WaitToAttack();
				yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 0.8f) + offsetTime);
				_prev = SeedBomb;
				_prev();
				while (true)
				{
					yield return new WaitWhile(() => _attacking);
					yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 0.8f) + offsetTime);
					_attacking = true;
					if (spawningWalls)
					{
						Log("Using Seed Bomb and spawning walls");
						_prev = SeedBomb;
						_prev();
						if (!usedAgony)
						{
							yield return new WaitWhile(() => _attacking);
							yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 0.8f) + offsetTime);
							_attacking = true;
							Log("Doing Agony");
							StartCoroutine(Agony());
						}
						continue;
					}
					if (startedIsmaDeath)
					{
						break;
					}
					int[] array = new int[3] { 0, 2, 2 };
					Action[] array2 = new Action[3] { SeedBomb, AirFist, WhipAttack };
					if (!_wallActive)
					{
						array[0] += Math.Max(3 - EnemyPlantSpawn.FoolCount - 1, 0);
						array[0] += 2 - EnemyPlantSpawn.TurretCount;
					}
					else
					{
						array[0] += 3 - EnemyPlantSpawn.PillarCount;
					}
					if (_prev.Method.Name == "SeedBomb")
					{
						array[0]--;
						array[1]++;
					}
					if (_prev.Method.Name == "AirFist")
					{
						array[1]--;
					}
					if (_prev.Method.Name == "WhipAttack")
					{
						array[2]--;
					}
					List<Action> list = new List<Action>();
					for (int num = 0; num < array.Length; num++)
					{
						for (int num2 = 0; num2 < array[num]; num2++)
						{
							list.Add(array2[num]);
						}
					}
					_prev = list[UnityEngine.Random.Range(0, list.Count)];
					Log("Doing: " + _prev.Method.Name);
					_prev();
				}
				StartCoroutine(IsmaLoneDeath());
			}

			private IEnumerator DuoAttacks()
			{
				EnemyPlantSpawn.isPhase2 = true;
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.SendEventByName>("After Throw?", 0).delay = 0.2f;
				_ddFsm.InsertAction("After Throw?", _ddFsm.GetAction<HutongGames.PlayMaker.Actions.Tk2dPlayAnimation>("Idle", 0), 0);
				_ddFsm.InsertMethod("Throw 1", delegate
				{
					_ddFsm.FsmVariables.FindFsmGameObject("Dung Ball").Value.name = "Ogrim Thrown Ball";
				}, 2);
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.Wait>("G Slam Recover", 0).time = 1.2f;
				_ddFsm.CopyFsmState("G Slam Recover", "G Slam Recover Acid");
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.Wait>("G Slam Recover Acid", 0).time = 0.5f;
				_ddFsm.RemoveAction("G Slam Recover Acid", 1);
				_ddFsm.InsertMethod("G Slam Recover Acid", delegate
				{
					Log("G Slam Recover Acid");
					_ddFsm.ChangeFsmTransition("G Slam Recover", "WAIT", "Move Choice");
				}, 0);
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.SetFsmBool>("Tunneling R", 0).variableName.Value = "RumblingSmall";
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.SetFsmBool>("Erupt Antic", 3).variableName.Value = "RumblingSmall";
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.SetFsmBool>("Erupt Antic R", 2).variableName.Value = "RumblingSmall";
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.SetFsmBool>("Erupt Out First", 1).variableName.Value = "RumblingSmall";
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.SetFsmBool>("Erupt Out", 2).variableName.Value = "RumblingSmall";
				_ddFsm.InsertMethod("G Slam Antic", EvadeBeforeAttack, 0);
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.RandomFloat>("Timer", 1).min.Value = 2.1f;
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.RandomFloat>("Timer", 1).max.Value = 2.1f;
				_ddFsm.InsertMethod("RJ Launch", 6, delegate
				{
					_ddFsm.FsmVariables.FindFsmFloat("Throw Speed Crt").Value = 12f;
				});
				_ddFsm.InsertMethod("Ball Hit Up", delegate
				{
					_ddFsm.FsmVariables.FindFsmInt("Bounces").Value--;
				}, 0);
				_ddFsm.InsertCoroutine("RJ In Air", 8, CheckPos);
				StartCoroutine(DungStrike());
				_ddFsm.InsertMethod("Timer", delegate
				{
					if (!startedIsmaRage)
					{
						StartCoroutine(WaitForWhipOrBomb());
					}
				}, 0);
				_ddFsm.InsertMethod("RJ Launch", delegate
				{
					StartCoroutine(WaitForBomb());
				}, 0);
				_ddFsm.InsertMethod("G Slam", delegate
				{
					StartCoroutine(WaitForAirFist());
				}, 0);
				yield return new WaitUntil(() => _wallActive);
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.Wait>("AD In", 0).time.Value = 0.4f;
				_ddFsm.RemoveAction("RJ Launch", 0);
				_ddFsm.InsertMethod("RJ Launch", delegate
				{
					_usingThornPillars = false;
					StartCoroutine(WaitForThornPillars());
				}, 0);
				_ddFsm.InsertMethod("RJ Speed Adjust", delegate
				{
					_usingThornPillars = true;
				}, 0);
				StartCoroutine(OgrimStrike());
				_ddFsm.InsertMethod("Under", delegate
				{
					StartCoroutine(WaitForAgony());
				}, 0);
				IEnumerator CheckPos()
				{
					yield return new WaitUntil(() => (!_ddFsm.FsmVariables.FindFsmBool("Still Bouncing").Value && _ddFsm.FsmVariables.FindFsmBool("Air Dive Height").Value) || (FastApproximately(dd.transform.position.y, 13.5f, 0.75f) && _rbDD.velocity.y > 0f && _ddFsm.FsmVariables.FindFsmInt("Bounces").Value < -3) || startedIsmaRage);
					_ddFsm.SendEvent("AIR DIVE");
				}
				void EvadeBeforeAttack()
				{
					if (!_ogrimEvaded && _wallActive && (FastApproximately(dd.transform.GetPositionX(), MiddleX, 4f) || (dd.transform.GetPositionX() < MiddleX - 4f && dd.transform.GetPositionX() - _target.transform.GetPositionX() > 0f) || (dd.transform.GetPositionX() > MiddleX + 4f && dd.transform.GetPositionX() - _target.transform.GetPositionX() < 0f)))
					{
						_ddFsm.SetState("Evade Dir");
						_ddFsm.GetAction<HutongGames.PlayMaker.Actions.SendRandomEvent>("After Evade", 0).weights[0].Value = 0f;
						_ogrimEvaded = true;
					}
					else
					{
						_ddFsm.GetAction<HutongGames.PlayMaker.Actions.SendRandomEvent>("After Evade", 0).weights[0].Value = 0.5f;
						_ogrimEvaded = false;
					}
				}
				IEnumerator WaitForAgony()
				{
					yield return new WaitForSeconds(0.6f);
					yield return WaitToAttack();
					yield return Agony();
				}
				IEnumerator WaitForAirFist()
				{
					yield return WaitToAttack();
					AirFist();
				}
				IEnumerator WaitForBomb()
				{
					yield return WaitToAttack();
					SeedBomb();
				}
				IEnumerator WaitForThornPillars()
				{
					yield return WaitToAttack();
					ThornPillars();
				}
				IEnumerator WaitForWhipOrBomb()
				{
					yield return WaitToAttack();
					if (spawningWalls)
					{
						SeedBomb();
					}
					else
					{
						WhipAttack();
					}
				}
			}

			private IEnumerator SpawnWalls()
			{
				GameObject[] array;
				if (!startedOgrimRage && !startedIsmaRage)
				{
					yield return new WaitWhile(() => _healthPool > (onlyIsma ? WallHP : WallHPDuo));
					if (onlyIsma)
					{
						yield return new WaitWhile(() => _attacking);
						Log("Queueing Seed Bomb");
						spawningWalls = true;
						yield return new WaitWhile(() => spawningWalls);
						yield return new WaitForSeconds(1f);
						GameObject.Find("SeedFloor").GetComponent<EnemyPlantSpawn>().Phase2Spawn();
					}
					else
					{
						yield return new WaitUntil(() => _ddFsm.ActiveStateName == "Idle");
						_ddFsm.InsertMethod("Rage?", delegate
						{
							_ddFsm.SetState("TD Set");
						}, 0);
						spawningWalls = true;
						yield return new WaitUntil(() => _ddFsm.ActiveStateName.Contains("Tunneling"));
						_ddFsm.RemoveAction("Rage?", 0);
						yield return new WaitForSeconds(0.3f);
						dd.LocateMyFSM("Constrain X").FsmVariables.FindFsmFloat("Edge R").Value = 88f;
						dd.LocateMyFSM("Constrain X").FsmVariables.FindFsmFloat("Edge L").Value = 64f;
						_ddFsm.FsmVariables.FindFsmFloat("Dolphin Max X").Value = 88f;
						_ddFsm.FsmVariables.FindFsmFloat("Dolphin Min X").Value = 64f;
						_ddFsm.FsmVariables.FindFsmFloat("Max X").Value = 85f;
						_ddFsm.FsmVariables.FindFsmFloat("Min X").Value = 67f;
						yield return new WaitWhile(() => spawningWalls);
						yield return new WaitForSeconds(1f);
					}
					EnemyPlantSpawn.isPhase2 = true;
					killAllMinions = true;
					yield return new WaitForSeconds(0.1f);
					killAllMinions = false;
					_wallActive = true;
					wallR = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Wall"]);
					wallR.transform.localScale = new Vector3(wallR.transform.localScale.x * -1f, wallR.transform.localScale.y, wallR.transform.localScale.z);
					wallL = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Wall"]);
					GameObject gameObject = wallR.transform.Find("FrontW").gameObject;
					GameObject obj = wallL.transform.Find("FrontW").gameObject;
					gameObject.layer = 8;
					obj.layer = 8;
					wallR.transform.position = new Vector3(RightX - 2.5f, GroundY, 0.1f);
					wallL.transform.position = new Vector3(LeftX + 2.5f, GroundY, 0.1f);
					Animator anim = gameObject.GetComponent<Animator>();
					this.PlayAudio(FiveKnights.Clips["IsmaAudWallGrow"]);
					yield return new WaitWhile(() => anim.GetCurrentFrame() < 3);
					wallR.transform.Find("Petal").gameObject.SetActive(value: true);
					wallL.transform.Find("Petal").gameObject.SetActive(value: true);
					Vector2 vector = _target.transform.position;
					if (vector.x > RightX - 6f)
					{
						_target.transform.position = new Vector2(RightX - 6f, vector.y);
					}
					else if (vector.x < LeftX + 6f)
					{
						_target.transform.position = new Vector2(LeftX + 6f, vector.y);
					}
					yield return new WaitWhile(() => _healthPool > (onlyIsma ? SpikeHP : SpikeHPDuo));
					if (onlyIsma)
					{
						yield return new WaitWhile(() => _attacking);
						spawningWalls = true;
						yield return new WaitWhile(() => spawningWalls);
						yield return new WaitForSeconds(1f);
					}
					else
					{
						yield return new WaitUntil(() => _ddFsm.ActiveStateName == "Idle");
						_ddFsm.InsertMethod("Rage?", delegate
						{
							_ddFsm.SetState("TD Set");
						}, 0);
						spawningWalls = true;
						yield return new WaitUntil(() => _ddFsm.ActiveStateName.Contains("Tunneling"));
						_ddFsm.RemoveAction("Rage?", 0);
						yield return new WaitForSeconds(0.3f);
						yield return new WaitWhile(() => spawningWalls);
						yield return new WaitForSeconds(1f);
					}
					array = new GameObject[2] { wallR, wallL };
					for (int num = 0; num < array.Length; num++)
					{
						GameObject obj2 = array[num].transform.Find("Spike").gameObject;
						GameObject obj3 = obj2.transform.Find("Front").gameObject;
						obj3.layer = 17;
						obj3.AddComponent<DamageHero>().damageDealt = 1;
						obj3.AddComponent<PlantHitFx>().hitSound = FiveKnights.Clips["IsmaAudWallHit"];
						obj3.AddComponent<NonBouncer>();
						obj2.SetActive(value: true);
						this.PlayAudio(FiveKnights.Clips["IsmaAudWallGrow"]);
					}
					eliminateMinions = false;
				}
				yield return new WaitWhile(() => !eliminateMinions);
				array = new GameObject[2] { wallL, wallR };
				foreach (GameObject gameObject2 in array)
				{
					if (!(gameObject2 == null))
					{
						gameObject2.transform.Find("FrontW").gameObject.GetComponent<Animator>().Play("WallFrontDestroy");
						gameObject2.transform.Find("Back").gameObject.GetComponent<Animator>().Play("WallBackDestroy");
						gameObject2.transform.Find("Petal").Find("P1").gameObject.GetComponent<Animator>().Play("PetalOneDest");
						gameObject2.transform.Find("Petal").Find("P3").gameObject.GetComponent<Animator>().Play("PetalManyDest");
						gameObject2.transform.Find("Spike").Find("Back").gameObject.GetComponent<Animator>().Play("SpikeBackDest");
						gameObject2.transform.Find("Spike").Find("Middle").gameObject.GetComponent<Animator>().Play("SpikeMiddleDest");
						gameObject2.transform.Find("Spike").Find("Front").gameObject.GetComponent<Animator>().Play("SpikeFrontDest");
						gameObject2.transform.Find("FrontW").gameObject.GetComponent<BoxCollider2D>().enabled = false;
						gameObject2.transform.Find("Spike").Find("Front").gameObject.GetComponent<BoxCollider2D>().enabled = false;
					}
				}
			}

			private void SeedBomb()
			{
				if (!onlyIsma)
				{
					firstBomb = false;
				}
				float dir = 0f;
				StartCoroutine(BombThrow());
				IEnumerator AnimEnder()
				{
					yield return _anim.WaitToFrame(8);
					yield return new WaitForSeconds(0.08f);
					base.transform.position += new Vector3(0f, 0.2f);
					yield return _anim.WaitToFrame(10);
					_bc.enabled = false;
					yield return _anim.PlayToEnd();
					ToggleIsma(visible: false);
				}
				IEnumerator BombThrow()
				{
					float positionX = _target.transform.GetPositionX();
					float x = ((positionX - MiddleX > 0f) ? (LeftX + 8f) : (RightX - 8f));
					if (_wallActive)
					{
						x = ((positionX - MiddleX > 0f) ? (LeftX + 11f) : (RightX - 11f));
					}
					base.transform.position = new Vector2(x, GroundY);
					dir = FaceHero();
					_rb.velocity = new Vector2(-20f * dir, 0f);
					ToggleIsma(visible: true);
					_anim.Play("ThrowBomb", -1, 0f);
					PlayVoice();
					yield return new WaitForEndOfFrame();
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 3);
					_anim.enabled = false;
					_rb.velocity = new Vector2(0f, 0f);
					yield return new WaitForSeconds(0.3f);
					_anim.enabled = true;
					GameObject gameObject = base.transform.Find("Bomb").gameObject;
					GameObject gameObject2 = gameObject.transform.Find("Seed").gameObject;
					GameObject bomb = UnityEngine.Object.Instantiate(gameObject, gameObject.transform.position, gameObject.transform.rotation);
					GameObject seed = UnityEngine.Object.Instantiate(gameObject2, gameObject2.transform.position, gameObject2.transform.rotation);
					Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();
					Animator anim = bomb.GetComponent<Animator>();
					bomb.transform.localScale *= 1.4f;
					bomb.SetActive(value: false);
					Vector3 localScale = seed.transform.localScale;
					localScale *= 2f;
					localScale.x *= -1f;
					seed.transform.localScale = localScale;
					seed.layer = 11;
					UnityEngine.Object.Destroy(seed.GetComponent<DamageHero>());
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 5);
					bomb.SetActive(value: true);
					rb.gravityScale = 1.3f;
					StartCoroutine(AnimEnder());
					rb.velocity = new Vector2(dir * 30f, 40f);
					CollisionCheck cc = bomb.AddComponent<CollisionCheck>();
					rb.angularVelocity = dir * 900f;
					yield return new WaitWhile(() => !cc.Hit && !FastApproximately(bomb.transform.GetPositionX(), MiddleX, 0.6f));
					anim.Play("Bomb");
					yield return new WaitWhile(() => anim.GetCurrentFrame() < 1);
					rb.angularVelocity = 0f;
					rb.gravityScale = 0f;
					rb.velocity = Vector2.zero;
					this.PlayAudio(FiveKnights.Clips["IsmaAudSeedBomb"]);
					yield return new WaitWhile(() => anim.GetCurrentFrame() <= 2);
					if (firstBomb)
					{
						for (int num = 200; num <= 340; num += 35)
						{
							GameObject obj = UnityEngine.Object.Instantiate(seed, bomb.transform.position, Quaternion.Euler(0f, 0f, num));
							obj.name = "SeedEnemy";
							obj.SetActive(value: true);
							obj.GetComponent<Rigidbody2D>().velocity = new Vector2(30f * Mathf.Cos((float)num * ((float)Math.PI / 180f)), 30f * Mathf.Sin((float)num * ((float)Math.PI / 180f)));
							UnityEngine.Object.Destroy(obj, 3f);
						}
						firstBomb = false;
					}
					else if (spawningWalls)
					{
						for (int num2 = 0; num2 < 3; num2++)
						{
							for (int num3 = 0; num3 < 8; num3++)
							{
								Vector2 vector = new Vector2((num3 < 4) ? (LeftX + 2f * (float)(num3 + 1)) : (RightX + 2f * (float)(num3 - 8)), 7.3f) - (Vector2)bomb.transform.position;
								float num4 = Mathf.Atan2(vector.y, vector.x);
								GameObject obj2 = UnityEngine.Object.Instantiate(seed, bomb.transform.position, Quaternion.Euler(0f, 0f, num4 * 57.29578f));
								obj2.name = "VineWallSeed";
								obj2.SetActive(value: true);
								obj2.GetComponent<Rigidbody2D>().velocity = new Vector2((25f + (float)num2 * 5f) * Mathf.Cos(num4), (25f + (float)num2 * 5f) * Mathf.Sin(num4));
								UnityEngine.Object.Destroy(obj2, 3f);
							}
						}
						spawningWalls = false;
					}
					else
					{
						int num5 = (_wallActive ? 50 : 40);
						for (int num6 = 0; num6 <= 360; num6 += num5)
						{
							float num7 = num6 + UnityEngine.Random.Range(0, 10) * (num5 / 10);
							GameObject obj3 = UnityEngine.Object.Instantiate(seed, bomb.transform.position, Quaternion.Euler(0f, 0f, num7));
							obj3.name = "SeedEnemy";
							obj3.SetActive(value: true);
							obj3.GetComponent<Rigidbody2D>().velocity = new Vector2(30f * Mathf.Cos(num7 * ((float)Math.PI / 180f)), 30f * Mathf.Sin(num7 * ((float)Math.PI / 180f)));
							UnityEngine.Object.Destroy(obj3, 3f);
						}
					}
					StartCoroutine(IdleTimer(0f));
					yield return new WaitWhile(() => anim.IsPlaying());
					UnityEngine.Object.Destroy(bomb);
				}
			}

			private void AirFist()
			{
				StartCoroutine(AirFist());
				IEnumerator AirFist()
				{
					float positionX = _target.transform.GetPositionX();
					float num = UnityEngine.Random.Range(10, 12);
					float x = ((positionX - MiddleX > 0f) ? (positionX - num) : (positionX + num));
					if (_wallActive)
					{
						float num2 = RightX - 8f;
						float num3 = LeftX + 10f;
						x = ((positionX - MiddleX > 0f) ? UnityEngine.Random.Range((int)num3, (int)positionX - 6) : UnityEngine.Random.Range((int)positionX + 6, (int)num2));
					}
					base.transform.position = new Vector2(x, UnityEngine.Random.Range(13, 16));
					ToggleIsma(visible: true);
					float dir = FaceHero();
					arm = base.transform.Find("Arm2").gameObject;
					_anim.Play("AFistAntic");
					PlayVoice();
					_rb.velocity = new Vector2(dir * -20f, 0f);
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 1);
					_rb.velocity = new Vector2(0f, 0f);
					spike = base.transform.Find("SpikeArm").gameObject;
					yield return new WaitWhile(() => _anim.IsPlaying());
					_anim.Play("AFist");
					yield return null;
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 0);
					spike.SetActive(value: true);
					_anim.enabled = false;
					yield return new WaitForSeconds(0.3f);
					float num4 = GetRot(arm, _target.transform.position, dir) * 57.29578f;
					if ((num4 < -55f || num4 > 55f) ? true : false)
					{
						_anim.enabled = true;
						spike.SetActive(value: false);
						yield return AcidThrow();
					}
					else
					{
						arm.transform.SetRotation2D(num4);
						GameObject gameObject = arm.transform.Find("TentArm").gameObject;
						gameObject.SetActive(value: false);
						tentArm = UnityEngine.Object.Instantiate(gameObject, gameObject.transform.position, gameObject.transform.rotation);
						tentArm.SetActive(value: false);
						Vector3 localScale = tentArm.transform.localScale;
						tentArm.transform.localScale = new Vector3(dir * localScale.x, localScale.y, localScale.z) * 1.35f;
						yield return new WaitForSeconds(0.1f);
						spike.SetActive(value: false);
						_anim.enabled = true;
						yield return new WaitUntil(() => _anim.GetCurrentFrame() >= 1);
						arm.SetActive(value: true);
						tentArm.SetActive(value: true);
						PolygonCollider2D[] componentsInChildren = tentArm.transform.GetComponentsInChildren<PolygonCollider2D>(includeInactive: true);
						for (int num5 = 0; num5 < componentsInChildren.Length; num5++)
						{
							componentsInChildren[num5].gameObject.AddComponent<PlantHitFx>().hitSound = FiveKnights.Clips["IsmaAudVineHit"];
						}
						Animator tentAnim = tentArm.GetComponent<Animator>();
						tentAnim.speed = 1.9f;
						yield return tentAnim.PlayToFrameAt("NewArmAttack", 0, 12);
						tentAnim.enabled = false;
						yield return new WaitForSeconds(0.15f);
						tentAnim.enabled = true;
						tentAnim.speed = 2f;
						yield return tentAnim.PlayToEnd();
						tentAnim.speed = 1f;
						yield return EndAirFist(spike, tentArm, dir);
						UnityEngine.Object.Destroy(tentArm);
						StartCoroutine(IdleTimer(0f));
					}
				}
				IEnumerator EndAirFist(GameObject spike, GameObject tentArm, float dir, float spd = 0.4f)
				{
					_anim.enabled = true;
					UnityEngine.Object.Destroy(tentArm);
					spike.SetActive(value: true);
					_anim.Play("AFist2");
					yield return new WaitForSeconds(0.05f);
					spike.SetActive(value: false);
					yield return _anim.PlayToFrame("AFistEnd", 1);
					_bc.enabled = false;
					yield return _anim.PlayToEnd();
					ToggleIsma(visible: false);
				}
				static float GetRot(GameObject arm, Vector3 pos, float dir)
				{
					Vector2 vector = arm.transform.position - pos;
					float num = 0f;
					if ((dir > 0f && vector.x > 0f) || (dir < 0f && vector.x < 0f))
					{
						num += 180f;
					}
					return Mathf.Atan(vector.y / vector.x) + num * ((float)Math.PI / 180f);
				}
			}

			private IEnumerator AcidThrow()
			{
				if (!onlyIsma)
				{
					_ddFsm.ChangeFsmTransition("G Slam Recover", "WAIT", "G Slam Recover Acid");
				}
				float num = ((base.transform.position.x < MiddleX) ? 1 : (-1));
				_rb.velocity = new Vector2(num * 15f, 0f);
				_anim.speed = 1.5f;
				yield return _anim.PlayToFrameAt("AcidSwipe", 0, 5);
				_anim.speed = 1f;
				_rb.velocity = Vector2.zero;
				yield return _anim.PlayToFrameAt("AcidSwipe", 5, 11);
				Vector2 vector = HeroController.instance.transform.position;
				Vector3 vector2 = base.transform.position - new Vector3(0f, 0.5f, 0f);
				float num2 = GetRot(vector2, vector);
				float num3 = 20f;
				for (int i = -1; i < 2; i++)
				{
					GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["AcidSpit"]);
					obj.AddComponent<AcidGnd>();
					obj.transform.position = vector2;
					obj.SetActive(value: true);
					float f = num2 + (float)i * (float)Math.PI / 3f;
					obj.GetComponent<Rigidbody2D>().velocity = new Vector2(num3 * Mathf.Cos(f), num3 * Mathf.Sin(f));
				}
				yield return _anim.PlayToEnd();
				ToggleIsma(visible: false);
				StartCoroutine(IdleTimer(0f));
				static float GetRot(Vector3 origPos, Vector3 tarPos)
				{
					Vector2 vector3 = origPos - tarPos;
					float num4 = Mathf.Atan(vector3.y / vector3.x) * 57.29578f;
					if (vector3.x > 0f)
					{
						num4 += 180f;
						num4 = ((num4 < 220f) ? 220f : num4);
					}
					else
					{
						num4 = ((num4 > -40f) ? (-40f) : num4);
					}
					return num4 * ((float)Math.PI / 180f);
				}
			}

			private void WhipAttack()
			{
				StartCoroutine(WhipAttack());
				IEnumerator WhipAttack()
				{
					float positionX = _target.transform.GetPositionX();
					float x = ((positionX - MiddleX > 0f) ? (LeftX + 8f) : (RightX - 8f));
					if (_wallActive)
					{
						x = ((positionX - MiddleX > 0f) ? (LeftX + 11f) : (RightX - 11f));
					}
					base.transform.position = new Vector2(x, GroundY);
					float num = FaceHero();
					ToggleIsma(visible: true);
					_rb.velocity = new Vector2(num * -20f, 0f);
					_anim.Play("GFistAntic");
					PlayVoice();
					yield return null;
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 2);
					_rb.velocity = Vector2.zero;
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 4);
					_anim.enabled = false;
					yield return new WaitForSeconds(0.7f);
					_anim.enabled = true;
					base.transform.position += new Vector3(1.13f * (float)Math.Sign(base.transform.localScale.x), 2.01f, 0f);
					GameObject gameObject = base.transform.Find("Whip").gameObject;
					whip = UnityEngine.Object.Instantiate(gameObject);
					whip.transform.position = gameObject.transform.position;
					whip.transform.localScale = gameObject.transform.lossyScale;
					whip.SetActive(value: true);
					PolygonCollider2D[] componentsInChildren = whip.transform.GetComponentsInChildren<PolygonCollider2D>(includeInactive: true);
					for (int num2 = 0; num2 < componentsInChildren.Length; num2++)
					{
						componentsInChildren[num2].gameObject.AddComponent<PlantHitFx>().hitSound = FiveKnights.Clips["IsmaAudVineHit"];
					}
					this.PlayAudio(FiveKnights.Clips["IsmaAudGroundWhip"]);
					yield return _anim.PlayToEndWithActions("GFistCopy", (0, delegate
					{
						USceneUtil.Find(whip, "W1").SetActive(value: true);
					}), (1, delegate
					{
						USceneUtil.Find(whip, "W2").SetActive(value: true);
						USceneUtil.Find(whip, "W1").SetActive(value: false);
					}), (2, delegate
					{
						USceneUtil.Find(whip, "W3").SetActive(value: true);
						USceneUtil.Find(whip, "W2").SetActive(value: false);
					}), (3, delegate
					{
						USceneUtil.Find(whip, "W4").SetActive(value: true);
						USceneUtil.Find(whip, "W3").SetActive(value: false);
					}), (4, delegate
					{
						USceneUtil.Find(whip, "W5").SetActive(value: true);
						USceneUtil.Find(whip, "W4").SetActive(value: false);
					}), (5, delegate
					{
						USceneUtil.Find(whip, "W6").SetActive(value: true);
						USceneUtil.Find(whip, "W5").SetActive(value: false);
					}), (6, delegate
					{
						USceneUtil.Find(whip, "W7").SetActive(value: true);
						USceneUtil.Find(whip, "W6").SetActive(value: false);
					}), (7, delegate
					{
						USceneUtil.Find(whip, "W8").SetActive(value: true);
						USceneUtil.Find(whip, "W7").SetActive(value: false);
					}), (8, delegate
					{
						USceneUtil.Find(whip, "W9").SetActive(value: true);
						USceneUtil.Find(whip, "W8").SetActive(value: false);
					}), (9, delegate
					{
						USceneUtil.Find(whip, "W12").SetActive(value: true);
						USceneUtil.Find(whip, "W9").SetActive(value: false);
					}), (10, delegate
					{
						USceneUtil.Find(whip, "W13").SetActive(value: true);
						USceneUtil.Find(whip, "W12").SetActive(value: false);
					}), (11, delegate
					{
						USceneUtil.Find(whip, "W14").SetActive(value: true);
						USceneUtil.Find(whip, "W13").SetActive(value: false);
					}), (12, delegate
					{
						USceneUtil.Find(whip, "W15").SetActive(value: true);
						USceneUtil.Find(whip, "W14").SetActive(value: false);
					}), (13, delegate
					{
						USceneUtil.Find(whip, "W16").SetActive(value: true);
						USceneUtil.Find(whip, "W15").SetActive(value: false);
					}), (14, delegate
					{
						USceneUtil.Find(whip, "W17").SetActive(value: true);
						USceneUtil.Find(whip, "W16").SetActive(value: false);
					}), (15, delegate
					{
						USceneUtil.Find(whip, "W17").SetActive(value: false);
					}));
					base.transform.position -= new Vector3(1.13f * (float)Math.Sign(base.transform.localScale.x), 1.9100001f, 0f);
					yield return _anim.PlayToFrame("GFistEnd", 2);
					_bc.enabled = false;
					yield return _anim.PlayToEnd();
					ToggleIsma(visible: false);
					UnityEngine.Object.Destroy(whip);
					StartCoroutine(IdleTimer(0f));
				}
			}

			private IEnumerator BowWhipAttack()
			{
				float num = -1f * FaceHero(opposite: true);
				PlayVoice();
				_anim.PlayAt("LoneDeath", 1);
				_rb.velocity = new Vector2((0f - num) * 17f, 32f);
				_rb.gravityScale = 1.5f;
				yield return null;
				_anim.enabled = false;
				yield return new WaitForSeconds(0.05f);
				_anim.enabled = true;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 2);
				_anim.enabled = false;
				yield return new WaitWhile(() => base.transform.position.y > GroundY + 2.5f);
				base.transform.position = new Vector3(base.transform.position.x, GroundY + 2.5f, base.transform.position.z);
				_anim.enabled = true;
				_rb.gravityScale = 0f;
				_rb.velocity = Vector2.zero;
				yield return new WaitForSeconds(0.1f);
				FaceHero();
				base.transform.position = new Vector3(base.transform.position.x, GroundY, base.transform.position.z);
				Log("Start play");
				base.transform.position += new Vector3(1.13f * (float)Math.Sign(base.transform.localScale.x), 2.01f, 0f);
				GameObject gameObject = base.transform.Find("Whip").gameObject;
				GameObject whip = UnityEngine.Object.Instantiate(gameObject);
				whip.transform.position = gameObject.transform.position;
				whip.transform.localScale = gameObject.transform.lossyScale;
				whip.SetActive(value: true);
				PolygonCollider2D[] componentsInChildren = whip.transform.GetComponentsInChildren<PolygonCollider2D>(includeInactive: true);
				for (int num2 = 0; num2 < componentsInChildren.Length; num2++)
				{
					componentsInChildren[num2].gameObject.AddComponent<PlantHitFx>().hitSound = FiveKnights.Clips["IsmaAudVineHit"];
				}
				this.PlayAudio(FiveKnights.Clips["IsmaAudGroundWhip"]);
				yield return _anim.PlayToEndWithActions("GFistCopy", (0, delegate
				{
					USceneUtil.Find(whip, "W1").SetActive(value: true);
				}), (1, delegate
				{
					USceneUtil.Find(whip, "W2").SetActive(value: true);
					USceneUtil.Find(whip, "W1").SetActive(value: false);
				}), (2, delegate
				{
					USceneUtil.Find(whip, "W3").SetActive(value: true);
					USceneUtil.Find(whip, "W2").SetActive(value: false);
				}), (3, delegate
				{
					USceneUtil.Find(whip, "W4").SetActive(value: true);
					USceneUtil.Find(whip, "W3").SetActive(value: false);
				}), (4, delegate
				{
					USceneUtil.Find(whip, "W5").SetActive(value: true);
					USceneUtil.Find(whip, "W4").SetActive(value: false);
				}), (5, delegate
				{
					USceneUtil.Find(whip, "W6").SetActive(value: true);
					USceneUtil.Find(whip, "W5").SetActive(value: false);
				}), (6, delegate
				{
					USceneUtil.Find(whip, "W7").SetActive(value: true);
					USceneUtil.Find(whip, "W6").SetActive(value: false);
				}), (7, delegate
				{
					USceneUtil.Find(whip, "W8").SetActive(value: true);
					USceneUtil.Find(whip, "W7").SetActive(value: false);
				}), (8, delegate
				{
					USceneUtil.Find(whip, "W9").SetActive(value: true);
					USceneUtil.Find(whip, "W8").SetActive(value: false);
				}), (9, delegate
				{
					USceneUtil.Find(whip, "W12").SetActive(value: true);
					USceneUtil.Find(whip, "W9").SetActive(value: false);
				}), (10, delegate
				{
					USceneUtil.Find(whip, "W13").SetActive(value: true);
					USceneUtil.Find(whip, "W12").SetActive(value: false);
				}), (11, delegate
				{
					USceneUtil.Find(whip, "W14").SetActive(value: true);
					USceneUtil.Find(whip, "W13").SetActive(value: false);
				}), (12, delegate
				{
					USceneUtil.Find(whip, "W15").SetActive(value: true);
					USceneUtil.Find(whip, "W14").SetActive(value: false);
				}), (13, delegate
				{
					USceneUtil.Find(whip, "W16").SetActive(value: true);
					USceneUtil.Find(whip, "W15").SetActive(value: false);
				}), (14, delegate
				{
					USceneUtil.Find(whip, "W17").SetActive(value: true);
					USceneUtil.Find(whip, "W16").SetActive(value: false);
				}), (15, delegate
				{
					USceneUtil.Find(whip, "W17").SetActive(value: false);
				}));
				base.transform.position -= new Vector3(1.13f * (float)Math.Sign(base.transform.localScale.x), 1.9100001f, 0f);
				yield return _anim.PlayToFrame("GFistEnd", 2);
				_bc.enabled = false;
				yield return _anim.PlayToEnd();
				ToggleIsma(visible: false);
				StartCoroutine(IdleTimer(0f));
			}

			private IEnumerator DungStrike()
			{
				tk2dSpriteAnimator tk = dd.GetComponent<tk2dSpriteAnimator>();
				bool prevRageBallMissed = false;
				while (true)
				{
					yield return new WaitUntil(() => tk.CurrentClip.name.Contains("Throw") || tk.CurrentClip.name.Contains("Erupt"));
					yield return WaitToAttack();
					while (tk.CurrentClip.name.Contains("Throw") || (tk.CurrentClip.name.Contains("Erupt") && ((_ddFsm.FsmVariables.FindFsmInt("Rages").Value % 2 == 1) | prevRageBallMissed)))
					{
						GameObject go = LocateBall();
						if (go == null)
						{
							yield return new WaitForEndOfFrame();
							continue;
						}
						Rigidbody2D component = go.GetComponent<Rigidbody2D>();
						Vector3 localScale = base.transform.localScale;
						if (component.velocity.x > 0f)
						{
							base.transform.localScale = new Vector3(0f - Mathf.Abs(localScale.x), localScale.y, localScale.z);
						}
						base.transform.position = new Vector3(go.transform.position.x, 16f, base.transform.position.z);
						ToggleIsma(visible: true);
						_anim.Play("BallStrike");
						Log("Starting coroutine");
						StartCoroutine(TrackBall());
						Log("Disabling animation");
						yield return new WaitWhile(() => _anim.GetCurrentFrame() < 1);
						_anim.enabled = false;
						if (!_playingVoiceDS)
						{
							StartCoroutine(DungStrikeVoice());
						}
						yield return new WaitWhile(() => _anim.GetCurrentFrame() < 2);
						_anim.enabled = true;
						Log("After coroutine");
						GameObject squish = base.gameObject.transform.Find("Squish").gameObject;
						GameObject ball = UnityEngine.Object.Instantiate(base.gameObject.transform.Find(_usingThornPillars ? "Ball" : "VineBall").gameObject);
						GameObject particles = UnityEngine.Object.Instantiate(go.LocateMyFSM("Ball Control").FsmVariables.FindFsmGameObject("Break Chunks").Value);
						DungBall dungBall = ball.AddComponent<DungBall>();
						dungBall.particles = particles;
						dungBall.usingThornPillars = _usingThornPillars;
						if (!_wallActive)
						{
							dungBall.LeftX = LeftX + 1f;
							dungBall.RightX = RightX - 1f;
						}
						Log("Trying to hit ball");
						if (go != null && go.GetComponent<Renderer>().enabled)
						{
							Log("Hitting ball");
							UnityEngine.Object.Destroy(go);
							squish.SetActive(value: true);
							this.PlayAudio(FiveKnights.Clips["IsmaAudDungHit"], 0.8f);
							yield return new WaitWhile(() => _anim.GetCurrentFrame() <= 2);
							GameObject ballFx = ball.transform.Find("BallFx").gameObject;
							squish.SetActive(value: false);
							ball.SetActive(value: true);
							ball.transform.position = base.gameObject.transform.Find("Ball").position;
							ballFx.transform.parent = null;
							Vector2 vector = ball.transform.position - _target.transform.position;
							float num = 0f;
							if (vector.x > 0f)
							{
								num += 180f;
							}
							float num2 = Mathf.Atan(vector.y / vector.x) + num * ((float)Math.PI / 180f);
							ball.transform.SetRotation2D(num2 * 57.29578f + 90f);
							Vector2 velocity = new Vector2(25f * Mathf.Cos(num2), 25f * Mathf.Sin(num2));
							ball.GetComponent<Rigidbody2D>().velocity = velocity;
							yield return new WaitForSeconds(0.1f);
							ballFx.GetComponent<Animator>().Play("FxEnd");
							yield return new WaitForSeconds(0.1f);
							UnityEngine.Object.Destroy(ballFx);
						}
						else if (_ddFsm.FsmVariables.FindFsmInt("Rages").Value % 2 != 1)
						{
						}
						prevRageBallMissed = false;
						Log("After hitting ball");
						yield return new WaitWhile(() => _anim.GetCurrentFrame() < 6);
						_rb.velocity = new Vector2(Mathf.Sign(base.transform.localScale.x) * 20f, 0f);
						yield return new WaitWhile(() => _anim.IsPlaying());
						_rb.velocity = new Vector2(0f, 0f);
						ToggleIsma(visible: false);
						yield return new WaitForEndOfFrame();
						Log("End of DungStrike");
						IEnumerator TrackBall()
						{
							Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
							while (go != null && go.GetComponent<Renderer>().enabled && go.transform.position.y < 16f && go.transform.position.y > 13f)
							{
								Vector3 localScale2 = base.transform.localScale;
								if (rb.velocity.x > 0f)
								{
									base.transform.localScale = new Vector3(0f - Mathf.Abs(localScale2.x), localScale2.y, localScale2.z);
								}
								else
								{
									base.transform.localScale = new Vector3(Mathf.Abs(localScale2.x), localScale2.y, localScale2.z);
								}
								base.transform.position = new Vector3(go.transform.position.x, 16f, base.transform.position.z);
								yield return null;
							}
							yield return new WaitUntil(() => !_anim.enabled);
							_anim.enabled = true;
							while (go != null && go.GetComponent<Renderer>().enabled)
							{
								Vector3 localScale3 = base.transform.localScale;
								if (rb.velocity.x > 0f)
								{
									base.transform.localScale = new Vector3(0f - Mathf.Abs(localScale3.x), localScale3.y, localScale3.z);
								}
								else
								{
									base.transform.localScale = new Vector3(Mathf.Abs(localScale3.x), localScale3.y, localScale3.z);
								}
								base.transform.position = new Vector3(go.transform.position.x, 16f, base.transform.position.z);
								yield return null;
							}
						}
					}
					StartCoroutine(IdleTimer(0f));
					yield return new WaitForSeconds(0.8f);
				}
			}

			private IEnumerator DungStrikeVoice()
			{
				_playingVoiceDS = true;
				PlayVoice();
				yield return new WaitForSeconds(2f);
				_playingVoiceDS = false;
			}

			private GameObject LocateBall()
			{
				tk2dSpriteAnimator tk = dd.GetComponent<tk2dSpriteAnimator>();
				GameObject[] array = (from x in UnityEngine.Object.FindObjectsOfType<GameObject>()
					where TestBall(x)
					select x).ToArray();
				if (array.Length != 0)
				{
					return array[_rand.Next(0, array.Length)];
				}
				return null;
				bool TestBall(GameObject x)
				{
					if (x.GetComponent<Rigidbody2D>() == null)
					{
						return false;
					}
					if (!x.activeSelf || !(x.transform.GetPositionY() > 13f) || !(x.GetComponent<Rigidbody2D>().velocity.y > 0f))
					{
						return false;
					}
					if (tk.CurrentClip.name.Contains("Throw") && x.name.Contains("Ogrim Thrown Ball"))
					{
						return true;
					}
					if (tk.CurrentClip.name.Contains("Erupt") && x.name.Contains("Dung Ball") && x.GetComponent<DungTracker>() == null && _ddFsm.FsmVariables.FindFsmInt("Rages").Value > 0 && FastApproximately(x.transform.GetPositionX(), _target.transform.GetPositionX(), 7.5f))
					{
						return true;
					}
					return false;
				}
			}

			private IEnumerator OgrimStrike()
			{
				tk2dSpriteAnimator tk = dd.GetComponent<tk2dSpriteAnimator>();
				_ddFsm.RemoveAction("RJ In Air", 8);
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.FloatTestToBool>("RJ In Air", 7).float2.Value = 13f;
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.BoolTestMulti>("RJ In Air", 8).Enabled = false;
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.SetVelocity2d>("Air Dive", 4).Enabled = false;
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.Tk2dPlayAnimationWithEvents>("Air Dive Antic", 1).Enabled = false;
				_ddFsm.InsertMethod("AD In", delegate
				{
					_rbDD.velocity = new Vector2(0f, _rbDD.velocity.y);
				}, 1);
				_ddFsm.InsertMethod("Under", delegate
				{
					dd.transform.SetRotation2D(0f);
				}, 0);
				while (true)
				{
					yield return new WaitUntil(() => tk.CurrentClip.name == "Roll");
					yield return new WaitWhile(() => _attacking);
					yield return new WaitForSeconds(1f);
					yield return WaitToAttack();
					yield return new WaitUntil(() => (!_ddFsm.FsmVariables.FindFsmBool("Still Bouncing").Value && _ddFsm.FsmVariables.FindFsmBool("Air Dive Height").Value) || (FastApproximately(dd.transform.position.y, 13.5f, 0.75f) && _rbDD.velocity.y > 0f && _ddFsm.FsmVariables.FindFsmInt("Bounces").Value < -3) || startedIsmaRage);
					if (startedIsmaRage)
					{
						break;
					}
					_ddFsm.SetState("Air Dive Antic");
					Vector2 vector = dd.transform.position;
					PlayVoice();
					float num = ((_rbDD.velocity.x > 0f) ? 1f : (-1f));
					float dir = FaceHero();
					base.gameObject.transform.position = new Vector2(vector.x + num * 2f, vector.y + 0.38f);
					Vector2 vector2 = dd.transform.position - new Vector3(_target.transform.GetPositionX(), GroundY);
					float num2 = 0f;
					if (vector2.x > 0f)
					{
						num2 += 180f;
					}
					float rot = Mathf.Atan(vector2.y / vector2.x) + num2 * ((float)Math.PI / 180f);
					Vector2 vel = new Vector2(50f * Mathf.Cos(rot), 45f * Mathf.Sin(rot));
					bool setVel = false;
					_ddFsm.InsertMethod("Air Dive", delegate
					{
						dd.transform.SetRotation2D(rot * 57.29578f + 90f);
						_rbDD.velocity = vel;
						setVel = true;
					}, 2);
					ToggleIsma(visible: true);
					_anim.Play("BallStrike");
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 1);
					_anim.enabled = false;
					_rb.velocity = new Vector2((0f - dir) * 5f, 0f);
					yield return new WaitForSeconds(0.1f);
					_rb.velocity = Vector2.zero;
					yield return new WaitForSeconds(0.1f);
					_anim.enabled = true;
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 3);
					_ddFsm.SendEvent("FINISHED");
					yield return new WaitUntil(() => setVel || _isDead);
					_ddFsm.RemoveAction("Air Dive", 2);
					yield return new WaitForSeconds(0.2f);
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 6);
					_rb.velocity = new Vector2(dir * 20f, 0f);
					yield return new WaitWhile(() => _anim.IsPlaying());
					_rb.velocity = Vector2.zero;
					ToggleIsma(visible: false);
					yield return new WaitForEndOfFrame();
					StartCoroutine(IdleTimer(0f));
				}
				_attacking = false;
			}

			private void ThornPillars()
			{
				StartCoroutine(DoThornPillars());
				IEnumerator DoThornPillars()
				{
					float x = ((_target.transform.GetPositionX() - MiddleX > 0f) ? (LeftX + 9f) : (RightX - 9f));
					base.transform.position = new Vector2(x, GroundY);
					float dir = FaceHero();
					ToggleIsma(visible: true);
					_rb.velocity = new Vector2(dir * -20f, 0f);
					_anim.Play("ThornPillarsAntic");
					PlayVoice();
					yield return null;
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 2);
					_rb.velocity = Vector2.zero;
					_anim.enabled = false;
					yield return new WaitForSeconds(0.3f);
					_anim.enabled = true;
					yield return new WaitWhile(() => _anim.IsPlaying());
					_anim.Play("ThornPillars");
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 3);
					_anim.enabled = false;
					yield return new WaitForSeconds(0.1f);
					_ = MiddleX;
					UnityEngine.Random.Range(-1f, 1f);
					for (int i = -2; i < 3; i++)
					{
						GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["ThornPlant"]);
						obj.AddComponent<ThornPlantCtrl>();
						obj.GetComponent<BoxCollider2D>().enabled = false;
						obj.layer = 22;
						obj.transform.position = new Vector2(MiddleX + 4f * (float)i * dir, 13.4f);
						yield return new WaitForSeconds(0.1f);
					}
					yield return new WaitForSeconds(1f);
					for (int i = -2; i < 3; i++)
					{
						GameObject obj2 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["ThornPlant"]);
						obj2.AddComponent<ThornPlantCtrl>().secondWave = true;
						obj2.GetComponent<BoxCollider2D>().enabled = false;
						obj2.layer = 22;
						obj2.transform.position = new Vector2(MiddleX + 4f * (float)i * dir + 2f * dir, 13.4f);
						yield return new WaitForSeconds(0.1f);
					}
					yield return new WaitForSeconds(0.2f);
					_anim.enabled = true;
					yield return new WaitWhile(() => _anim.IsPlaying());
					_anim.Play("ThornPillarsEnd");
					yield return null;
					yield return new WaitWhile(() => _anim.IsPlaying());
					ToggleIsma(visible: false);
					yield return new WaitForSeconds(1f);
					StartCoroutine(IdleTimer(0f));
				}
			}

			private IEnumerator Agony()
			{
				ToggleIsma(visible: true);
				Vector3 localScale = base.gameObject.transform.localScale;
				base.gameObject.transform.localScale = new Vector3(Mathf.Abs(localScale.x), localScale.y, localScale.z);
				base.gameObject.transform.SetPosition2D(MiddleX, GroundY + 11.6f);
				GameObject gameObject = new GameObject();
				gameObject.transform.position = base.gameObject.transform.position;
				gameObject.transform.localScale = base.gameObject.transform.localScale;
				GameObject gameObject2 = base.transform.Find("Thorn").gameObject;
				agonyThorns = UnityEngine.Object.Instantiate(gameObject2);
				Vector3 position = gameObject2.transform.position;
				agonyThorns.transform.position = new Vector3(position.x - 1f, position.y - 4f, position.z);
				agonyThorns.transform.parent = gameObject.transform;
				Animator tAnim = agonyThorns.transform.Find("T1").gameObject.GetComponent<Animator>();
				_anim.Play("AgonyLoopIntro");
				yield return null;
				yield return new WaitWhile(() => _anim.IsPlaying());
				PlayVoice();
				_anim.speed = 1.7f;
				yield return PerformAgony(agonyThorns, tAnim, (!onlyIsma) ? 1 : 3);
				_anim.Play("AgonyLoopEnd");
				yield return null;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 1);
				_rb.velocity = new Vector2(20f, 0f);
				yield return new WaitWhile(() => _anim.IsPlaying());
				_anim.speed = 1f;
				_rb.velocity = Vector2.zero;
				usedAgony = true;
				ToggleIsma(visible: false);
				StartCoroutine(IdleTimer(0f));
			}

			private IEnumerator PerformAgony(GameObject thorn, Animator tAnim, int loops = 0)
			{
				bool repeat = loops == 0;
				for (int j = 0; (j < loops) | repeat; j++)
				{
					_anim.PlayAt("AgonyLoop", 0);
					this.PlayAudio(FiveKnights.Clips["IsmaAudAgonyIntro"]);
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 3);
					thorn.SetActive(value: true);
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 9);
					if (j % 2 == 0 && j != 0)
					{
						PlayVoice();
					}
					Animator[] anims = thorn.GetComponentsInChildren<Animator>(includeInactive: true);
					Vector2 velocity = _target.GetComponent<Rigidbody2D>().velocity;
					float num = 0f;
					float num2 = 0f;
					float num3 = 1f;
					Vector3 vector = _target.transform.position + new Vector3(velocity.x * num3, velocity.y * num2) * num;
					Vector2 vector2 = tAnim.transform.position - vector;
					float num4 = Mathf.Atan(vector2.y / vector2.x) * 57.29578f + ((vector2.x < 0f) ? 180f : 0f);
					float smallOff = 4f;
					float num5 = num4;
					float[] arr = new float[5]
					{
						num4,
						num5 + UnityEngine.Random.Range(agonySpread.x, agonySpread.y),
						num5 + 2f * UnityEngine.Random.Range(agonySpread.x, agonySpread.y),
						num5 - UnityEngine.Random.Range(agonySpread.x, agonySpread.y),
						num5 - 2f * UnityEngine.Random.Range(agonySpread.x, agonySpread.y)
					};
					for (int num6 = 0; num6 < arr.Length; num6++)
					{
						float num7 = arr[num6];
						Animator obj = anims[num6 * 2];
						Animator animator = anims[num6 * 2 + 1];
						GameObject obj2 = obj.gameObject;
						GameObject obj3 = animator.gameObject;
						int layer = 17;
						obj3.layer = 17;
						obj2.layer = layer;
						obj.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, num7 - smallOff + 12.5f);
						animator.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, num7 + smallOff + 12.5f);
					}
					Animator[] componentsInChildren = thorn.GetComponentsInChildren<Animator>(includeInactive: true);
					foreach (Animator obj4 in componentsInChildren)
					{
						obj4.gameObject.GetComponent<SpriteRenderer>().enabled = true;
						obj4.Play("NewAThornAnim");
						obj4.speed = agonyAnimSpd;
					}
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 9);
					_anim.enabled = false;
					yield return new WaitWhile(() => tAnim.GetCurrentFrame() < 4);
					componentsInChildren = thorn.GetComponentsInChildren<Animator>(includeInactive: true);
					for (int layer = 0; layer < componentsInChildren.Length; layer++)
					{
						componentsInChildren[layer].enabled = false;
					}
					yield return new WaitForSeconds(0.2f);
					componentsInChildren = thorn.GetComponentsInChildren<Animator>(includeInactive: true);
					for (int layer = 0; layer < componentsInChildren.Length; layer++)
					{
						componentsInChildren[layer].enabled = true;
					}
					for (int num8 = 0; num8 < arr.Length; num8++)
					{
						float num9 = arr[num8];
						Animator obj5 = anims[num8 * 2];
						Animator animator2 = anims[num8 * 2 + 1];
						obj5.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, num9 - smallOff);
						animator2.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, num9 + smallOff);
					}
					yield return new WaitWhile(() => tAnim.GetCurrentFrame() < 5);
					this.PlayAudio(FiveKnights.Clips["IsmaAudAgonyShoot"]);
					yield return new WaitWhile(() => tAnim.GetCurrentFrame() < 6);
					_anim.enabled = true;
					yield return new WaitWhile(() => tAnim.IsPlaying());
					componentsInChildren = thorn.GetComponentsInChildren<Animator>(includeInactive: true);
					foreach (Animator obj6 in componentsInChildren)
					{
						obj6.gameObject.GetComponent<SpriteRenderer>().enabled = false;
						obj6.Play("IdleThorn");
					}
					thorn.SetActive(value: false);
					yield return new WaitWhile(() => _anim.IsPlaying());
				}
			}

			private IEnumerator LoopedAgony()
			{
				ToggleIsma(visible: true);
				_attacking = true;
				base.gameObject.transform.SetPosition2D(MiddleX, GroundY + 11.1f);
				fakeIsma = new GameObject();
				fakeIsma.transform.position = base.gameObject.transform.position;
				fakeIsma.transform.localScale = base.gameObject.transform.localScale;
				GameObject gameObject = base.transform.Find("Thorn").gameObject;
				GameObject thorn = UnityEngine.Object.Instantiate(gameObject);
				Vector3 position = gameObject.transform.position;
				thorn.transform.parent = fakeIsma.transform;
				thorn.transform.position = position;
				Animator[] componentsInChildren = thorn.GetComponentsInChildren<Animator>(includeInactive: true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].transform.position = gameObject.transform.Find("T1").position;
				}
				Animator tAnim = thorn.transform.Find("T1").gameObject.GetComponent<Animator>();
				_anim.Play("AgonyLoopIntro");
				yield return new WaitForSeconds(0.05f);
				yield return new WaitWhile(() => _anim.IsPlaying());
				_anim.speed = 1.6f;
				yield return PerformAgony(thorn, tAnim);
			}

			private float CalculateTrajectory(Vector2 vel0, float h, float g)
			{
				float num = g * Physics2D.gravity.y;
				float f = vel0.y * vel0.y - 2f * num * h;
				float num2 = (0f - vel0.y + Mathf.Sqrt(f)) / num;
				float num3 = (0f - vel0.y - Mathf.Sqrt(f)) / num;
				if (num2 < 0f)
				{
					num2 = num3;
				}
				return num2 * vel0.x;
			}

			private void Update()
			{
				if (_healthPool <= 0 && !_isDead)
				{
					Log("Victory");
					_isDead = true;
					preventDamage = true;
					_healthPool = FrenzyHP;
					EnemyHPBarImport.DisableHPBar(base.gameObject);
					if (onlyIsma)
					{
						_hm.hp = 800;
						_hm.isDead = false;
						startedIsmaDeath = true;
					}
					else if (_isIsmaHitLast)
					{
						startedOgrimRage = true;
						StopAllCoroutines();
						StartCoroutine(OgrimRage());
						StartCoroutine(SpawnWalls());
					}
					else
					{
						startedIsmaRage = true;
						StopCoroutine(_wallsCoro);
						StartCoroutine(IsmaRage());
						StartCoroutine(SpawnWalls());
					}
				}
			}

			private IEnumerator OgrimRage()
			{
				Log("Started Ogrim rage");
				_attacking = true;
				_healthPool = (_hm.hp = (_hmDD.hp = MaxHPDuo));
				UnityEngine.Object.Destroy(whip);
				UnityEngine.Object.Destroy(agonyThorns);
				UnityEngine.Object.Destroy(tentArm);
				UnityEngine.Object.Destroy(arm);
				UnityEngine.Object.Destroy(spike);
				float num = FaceHero(opposite: true);
				PlayDeathFor(base.gameObject);
				_bc.enabled = false;
				_rb.gravityScale = 1.5f;
				float x = num * 5f;
				_rb.velocity = new Vector2(x, 28f);
				float side = Mathf.Sign(base.gameObject.transform.localScale.x);
				_anim.Play("LoneDeath");
				_anim.speed *= 0.7f;
				_anim.enabled = true;
				yield return null;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 2);
				_anim.enabled = false;
				yield return new WaitWhile(() => base.transform.position.y > GroundY + 2.5f);
				base.transform.position = new Vector3(base.transform.position.x, GroundY + 2.25f, base.transform.position.z);
				Vector3 localScale = base.transform.localScale;
				base.transform.localScale = new Vector3(localScale.x * -1f, localScale.y, localScale.z);
				_anim.enabled = true;
				_rb.gravityScale = 0f;
				_rb.velocity = new Vector2(0f, 0f);
				yield return _anim.WaitToFrame(4);
				_anim.speed = 1f;
				_anim.PlayAt("IsmaTired", 0);
				yield return new WaitForSeconds(1f);
				base.transform.position = new Vector3(base.transform.position.x, GroundY + 2.35f, base.transform.position.z);
				localScale = base.transform.localScale;
				base.transform.localScale = new Vector3(localScale.x * -1f, localScale.y, localScale.z);
				_anim.PlayAt("LoneDeath", 5);
				_anim.speed = 1f;
				_anim.enabled = true;
				yield return _anim.WaitToFrame(7);
				_rb.velocity = new Vector2((0f - side) * 25f, 25f);
				yield return new WaitForSeconds(0.2f);
				_sr.enabled = false;
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.Wait>("AD In", 0).time.Value = 0.25f;
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.BoolTestMulti>("RJ In Air", 8).Enabled = true;
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.SetVelocity2d>("Air Dive", 4).Enabled = true;
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.Tk2dPlayAnimationWithEvents>("Air Dive Antic", 1).Enabled = true;
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.SetIntValue>("Set Rage", 1).intValue = 999;
				Coroutine waitToRage = null;
				string[] array = new string[4] { "Idle", "Move Choice", "After Throw?", "After Evade" };
				foreach (string stateName in array)
				{
					_ddFsm.InsertMethod(stateName, delegate
					{
						if (waitToRage == null)
						{
							waitToRage = StartCoroutine(WaitToRage());
						}
					}, 0);
				}
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.Tk2dPlayAnimation>("Rage Roar", 1).Enabled = true;
				_ddFsm.InsertMethod("Rage Roar", delegate
				{
					_rbDD.velocity = Vector2.zero;
					_healthPool = (_hm.hp = (_hmDD.hp = FrenzyHP));
					EnemyHPBarImport.EnableHPBar(base.gameObject);
					preventDamage = false;
				}, 0);
				_ddFsm.InsertMethod("Set Rage", delegate
				{
					UnityEngine.Object.Destroy(_ddFsm.FsmVariables.FindFsmGameObject("Roar Emitter").Value);
				}, 0);
				yield return new WaitWhile(() => _healthPool > 0);
				float xSpd = ((_target.transform.GetPositionX() > dd.transform.GetPositionX()) ? (-20f) : 20f);
				UnityEngine.Object.Destroy(_ddFsm.GetAction<HutongGames.PlayMaker.Actions.FadeAudio>("Stun Recover", 2).gameObject.GameObject.Value);
				PlayDeathFor(dd);
				eliminateMinions = true;
				killAllMinions = true;
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.SetVelocity2d>("Stun Launch", 0).y.Value = 45f;
				_ddFsm.GetAction<HutongGames.PlayMaker.Actions.SetVelocity2d>("Stun Launch", 0).x.Value = xSpd;
				if (dd.transform.GetPositionY() < 9.1f)
				{
					dd.transform.position = new Vector2(dd.transform.GetPositionX(), 9.1f);
				}
				if (_musicCoro != null)
				{
					GameManager.instance.StopCoroutine(_musicCoro);
				}
				GGBossManager.Instance.PlayMusic(null, 1f);
				GameObject obj = dd;
				GameObject obj2 = base.gameObject;
				int num2 = 26;
				obj2.layer = 26;
				obj.layer = num2;
				_ddFsm.SetState("Stun Set");
				yield return null;
				yield return new WaitWhile(() => _ddFsm.ActiveStateName == "Stun Set");
				PlayerData.instance.isInvincible = true;
				float x2 = CalculateTrajectory(new Vector2(xSpd, 45f), 5.1f - dd.transform.GetPositionY(), _rbDD.gravityScale) + dd.transform.GetPositionX();
				if (x2 < 68f)
				{
					x2 = 68f;
				}
				else if (x2 > 85f)
				{
					x2 = 85f;
				}
				yield return new WaitWhile(() => _ddFsm.ActiveStateName != "Stun In Air");
				yield return null;
				_ddFsm.enabled = false;
				yield return new WaitWhile(() => _rbDD.velocity.y > 0f);
				Log("Catching ogrim");
				ToggleIsma(visible: true);
				_anim.Play("OgrimCatchIntro");
				float num3 = Mathf.Sign(dd.transform.localScale.x);
				localScale = base.gameObject.transform.localScale;
				base.transform.localScale = new Vector3(num3 * Mathf.Abs(localScale.x), localScale.y, localScale.z);
				base.transform.localScale *= 1.2f;
				base.transform.position = new Vector2(x2 + num3 * 2f, GroundY + 2.05f);
				_rb.velocity = new Vector2(num3 * -20f, 0f);
				yield return null;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 2);
				_rb.velocity = new Vector2(0f, 0f);
				Coroutine c = StartCoroutine(OgrimCatchPos());
				Log("After start coroutine");
				FiveKnights.journalEntries["Isma"].RecordJournalEntry();
				yield return new WaitWhile(() => !FastApproximately(base.transform.GetPositionY(), dd.transform.GetPositionY(), 1.6f) && dd.transform.GetPositionY() > 13f);
				Log("After wait while");
				if (c != null)
				{
					StopCoroutine(c);
				}
				if (dd.transform.GetPositionY() < 13f)
				{
					_anim.PlayAt("OgrimCatch", 2);
					base.transform.position = new Vector3(dd.transform.GetPositionX(), GroundY + 2.05f);
				}
				else
				{
					_anim.Play("OgrimCatch");
				}
				dd.SetActive(value: false);
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 2);
				_anim.enabled = false;
				yield return new WaitForSeconds(0.65f);
				_anim.enabled = true;
				yield return null;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 3);
				_rb.velocity = new Vector2(0f, 35f);
				GameCameras.instance.cameraShakeFSM.SendEvent("BigShake");
				yield return new WaitWhile(() => base.transform.position.y < 19f);
				PlayerData.instance.isInvincible = false;
				if (CustomWP.boss != CustomWP.Boss.All)
				{
					yield return new WaitForSeconds(1f);
				}
				CustomWP.wonLastFight = true;
				_ddFsm.enabled = false;
				UnityEngine.Object.Destroy(this);
				IEnumerator OgrimCatchPos()
				{
					while (true)
					{
						base.transform.position = new Vector2(dd.transform.GetPositionX(), base.transform.GetPositionY());
						yield return null;
					}
				}
				IEnumerator WaitToRage()
				{
					yield return new WaitWhile(() => dd.GetComponent<tk2dSpriteAnimator>().Playing);
					_ddFsm.SetState("Rage Roar");
				}
			}

			private IEnumerator IsmaRage()
			{
				Log("Started Isma rage");
				_healthPool = (_hm.hp = (_hmDD.hp = MaxHPDuo));
				On.HutongGames.PlayMaker.Actions.TransitionToAudioSnapshot.OnEnter += TransitionToAudioSnapshotOnEnter;
				if (dd.transform.GetPositionY() < 9.1f)
				{
					dd.transform.position = new Vector2(dd.transform.GetPositionX(), 9.1f);
				}
				_ddFsm.SetState("Stun Set");
				PlayMakerFSM burrow = GameObject.Find("Burrow Effect").LocateMyFSM("Burrow Effect");
				burrow.enabled = true;
				burrow.SendEvent("BURROW END");
				PlayMakerFSM[] componentsInChildren = USceneUtil.Find(dd, "Slam Pillars").GetComponentsInChildren<PlayMakerFSM>();
				foreach (PlayMakerFSM playMakerFSM in componentsInChildren)
				{
					if (playMakerFSM.ActiveStateName == "Up" || playMakerFSM.ActiveStateName == "Hit")
					{
						playMakerFSM.gameObject.GetComponent<MeshRenderer>().enabled = false;
						playMakerFSM.SetState("Dormant");
						playMakerFSM.FsmVariables.FindFsmGameObject("Chunks").Value.GetComponent<ParticleSystem>().Play();
					}
					else
					{
						playMakerFSM.enabled = false;
					}
				}
				yield return new WaitUntil(() => _ddFsm.ActiveStateName == "Stun Land");
				_ddFsm.enabled = false;
				burrow.enabled = false;
				_ddFsm.RemoveAction("Wake", 9);
				yield return new WaitForSeconds(1f);
				FsmTransition[] transitions = _ddFsm.GetState("Idle").Transitions;
				foreach (FsmTransition fsmTransition in transitions)
				{
					SFCore.Utils.FsmUtil.ChangeTransition(_ddFsm, "Idle", fsmTransition.EventName, "Timer");
				}
				_ddFsm.enabled = true;
				_ddFsm.SetState("Stun Recover");
				yield return new WaitForSeconds(0.5f);
				burrow.enabled = true;
				yield return new WaitUntil(() => _ddFsm.ActiveStateName.Contains("Tunneling"));
				_ddFsm.enabled = false;
				yield return WaitToAttack();
				Coroutine c = StartCoroutine(LoopedAgony());
				base.transform.localScale = new Vector3(Mathf.Abs(base.transform.localScale.x), base.transform.localScale.y, base.transform.localScale.z);
				if (dd.transform.GetPositionX() < MiddleX)
				{
					_rbDD.velocity = new Vector2(10f, 0f);
				}
				else
				{
					_rbDD.velocity = new Vector2(-10f, 0f);
				}
				StartCoroutine(TrackIsmaPos());
				_healthPool = (_hm.hp = (_hmDD.hp = FrenzyHP));
				EnemyHPBarImport.EnableHPBar(base.gameObject);
				preventDamage = false;
				yield return new WaitWhile(() => _healthPool > 0);
				eliminateMinions = true;
				killAllMinions = true;
				if (c != null)
				{
					StopCoroutine(c);
				}
				_anim.speed = 1f;
				SpriteRenderer[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer spriteRenderer in componentsInChildren2)
				{
					if (!spriteRenderer.name.Contains("Isma"))
					{
						spriteRenderer.gameObject.SetActive(value: false);
					}
				}
				UnityEngine.Object.Destroy(fakeIsma);
				float dir = FaceHero(opposite: true);
				_anim.enabled = true;
				yield return null;
				UnityEngine.Object.Destroy(_ddFsm.GetAction<HutongGames.PlayMaker.Actions.FadeAudio>("Stun Recover", 2).gameObject.GameObject.Value);
				if (_musicCoro != null)
				{
					GameManager.instance.StopCoroutine(_musicCoro);
				}
				GGBossManager.Instance.PlayMusic(null, 1f);
				FiveKnights.journalEntries["Isma"].RecordJournalEntry();
				On.HutongGames.PlayMaker.Actions.TransitionToAudioSnapshot.OnEnter -= TransitionToAudioSnapshotOnEnter;
				PlayDeathFor(base.gameObject);
				_anim.Play("Falling");
				PlayerData.instance.isInvincible = true;
				_rb.gravityScale = 1.5f;
				float x = dir * 10f;
				_rb.velocity = new Vector2(x, 28f);
				Vector3 localScale = dd.transform.localScale;
				float num = Mathf.Sign(base.gameObject.transform.localScale.x);
				dd.transform.localScale = new Vector3(num * Mathf.Abs(localScale.x), localScale.y, localScale.z);
				while (_rb.velocity.y > -15f)
				{
					dd.transform.position = new Vector2(base.transform.GetPositionX(), dd.transform.GetPositionY());
					yield return new WaitForEndOfFrame();
				}
				_rb.velocity = new Vector2(0f, _rb.velocity.y);
				Vector3 localScale2 = dd.transform.localScale;
				float num2 = Mathf.Sign(base.gameObject.transform.localScale.x);
				dd.LocateMyFSM("Constrain X").FsmVariables.FindFsmFloat("Edge L").Value = 61.2f;
				dd.LocateMyFSM("Constrain X").FsmVariables.FindFsmFloat("Edge R").Value = 91f;
				_ddFsm.FsmVariables.FindFsmFloat("Dolphin Max X").Value = 87.18f;
				_ddFsm.FsmVariables.FindFsmFloat("Dolphin Min X").Value = 65.49f;
				_ddFsm.FsmVariables.FindFsmFloat("Max X").Value = 90.57f;
				_ddFsm.FsmVariables.FindFsmFloat("Min X").Value = 61.78f;
				_rbDD.velocity = new Vector2(5f, 0f);
				dd.transform.localScale = new Vector3(num2 * Mathf.Abs(localScale2.x), localScale2.y, localScale2.z);
				_ddFsm.enabled = true;
				_ddFsm.FsmVariables.FindFsmBool("Intro Attack").Value = false;
				_ddFsm.SetState("First?");
				GameObject.Find("Burrow Effect").LocateMyFSM("Burrow Effect").SendEvent("BURROW END");
				yield return new WaitWhile(() => !FastApproximately(base.transform.GetPositionY(), dd.transform.GetPositionY(), 0.9f));
				dd.GetComponent<MeshRenderer>().enabled = false;
				dd.SetActive(value: false);
				_sr.material = new Material(Shader.Find("Sprites/Default"));
				_anim.Play("Catch");
				base.gameObject.transform.localScale *= 1.25f;
				_rb.gravityScale = 0f;
				_rb.velocity = new Vector2(0f, 50f);
				yield return new WaitWhile(() => base.transform.position.y < 19f);
				PlayerData.instance.isInvincible = false;
				if (CustomWP.boss != CustomWP.Boss.All)
				{
					yield return new WaitForSeconds(1f);
				}
				CustomWP.wonLastFight = true;
				_ddFsm.enabled = false;
				UnityEngine.Object.Destroy(this);
				IEnumerator TrackIsmaPos()
				{
					yield return new WaitUntil(() => FastApproximately(dd.transform.GetPositionX(), MiddleX, 1f));
					_rbDD.velocity = new Vector2(0f, 0f);
				}
				static void TransitionToAudioSnapshotOnEnter(On.HutongGames.PlayMaker.Actions.TransitionToAudioSnapshot.orig_OnEnter orig, HutongGames.PlayMaker.Actions.TransitionToAudioSnapshot self)
				{
					if (!(self.State.Name == "Wake"))
					{
						orig(self);
					}
				}
			}

			private IEnumerator IsmaLoneDeath()
			{
				Log("Started Isma Lone Death");
				_healthPool = (_hm.hp = FrenzyHP);
				EnemyHPBarImport.EnableHPBar(base.gameObject);
				preventDamage = false;
				Coroutine c = StartCoroutine(LoopedAgony());
				base.transform.localScale = new Vector3(Mathf.Abs(base.transform.localScale.x), base.transform.localScale.y, base.transform.localScale.z);
				while (_healthPool > 0)
				{
					_sr.enabled = true;
					_bc.enabled = true;
					yield return new WaitForEndOfFrame();
				}
				if (OWArenaFinder.IsInOverWorld)
				{
					GameManager.instance.AwardAchievement("PALE_COURT_ISMA_ACH");
				}
				FiveKnights.journalEntries["Isma"].RecordJournalEntry();
				eliminateMinions = true;
				killAllMinions = true;
				if (c != null)
				{
					StopCoroutine(c);
				}
				_anim.speed = 1f;
				SpriteRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer spriteRenderer in componentsInChildren)
				{
					if (!spriteRenderer.name.Contains("Isma"))
					{
						spriteRenderer.gameObject.SetActive(value: false);
					}
				}
				UnityEngine.Object.Destroy(fakeIsma);
				float dir = FaceHero(opposite: true);
				_anim.enabled = true;
				yield return null;
				if (_musicCoro != null)
				{
					GameManager.instance.StopCoroutine(_musicCoro);
				}
				if (!OWArenaFinder.IsInOverWorld)
				{
					GGBossManager.Instance.PlayMusic(null, 1f);
				}
				else
				{
					OWBossManager.PlayMusic(null);
				}
				PlayDeathFor(base.gameObject);
				_bc.enabled = false;
				_rb.gravityScale = 1.5f;
				float x = dir * 10f;
				_rb.velocity = new Vector2(x, 28f);
				float side = Mathf.Sign(base.gameObject.transform.localScale.x);
				_anim.Play("LoneDeath");
				_anim.speed *= 0.7f;
				_anim.enabled = true;
				yield return null;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 2);
				_anim.enabled = false;
				yield return new WaitWhile(() => base.transform.position.y > GroundY + 2.5f);
				base.transform.position = new Vector3(base.transform.position.x, GroundY + 2.25f, base.transform.position.z);
				Vector3 localScale = base.transform.localScale;
				base.transform.localScale = new Vector3(localScale.x * -1f, localScale.y, localScale.z);
				_anim.enabled = true;
				_rb.gravityScale = 0f;
				_rb.velocity = new Vector2(0f, 0f);
				yield return _anim.WaitToFrame(4);
				_anim.speed = 1f;
				_anim.PlayAt("IsmaTired", 0);
				yield return new WaitForSeconds(1f);
				localScale = base.transform.localScale;
				base.transform.localScale = new Vector3(localScale.x * -1f, localScale.y, localScale.z);
				_anim.PlayAt("LoneDeath", 5);
				_anim.speed = 1f;
				_anim.enabled = true;
				yield return _anim.WaitToFrame(7);
				_rb.velocity = new Vector2((0f - side) * 25f, 25f);
				yield return new WaitForSeconds(0.2f);
				_sr.enabled = false;
				yield return new WaitForSeconds(0.75f);
				if (!OWArenaFinder.IsInOverWorld)
				{
					CustomWP.wonLastFight = true;
				}
				UnityEngine.Object.Destroy(this);
			}

			private IEnumerator ChangeIntroText(GameObject area, string mainTxt, string subTxt, string supTxt, bool right)
			{
				area.SetActive(value: true);
				PlayMakerFSM playMakerFSM = area.LocateMyFSM("Area Title Control");
				playMakerFSM.FsmVariables.FindFsmBool("Visited").Value = true;
				playMakerFSM.FsmVariables.FindFsmBool("Display Right").Value = right;
				yield return null;
				GameObject obj = area.transform.Find("Title Small").gameObject;
				GameObject gameObject = obj.transform.Find("Title Small Main").gameObject;
				GameObject gameObject2 = obj.transform.Find("Title Small Super").gameObject;
				GameObject obj2 = obj.transform.Find("Title Small Sub").gameObject;
				gameObject.GetComponent<TextMeshPro>().text = mainTxt;
				gameObject2.GetComponent<TextMeshPro>().text = supTxt;
				obj2.GetComponent<TextMeshPro>().text = subTxt;
				Vector3 position = obj.transform.position;
				obj.transform.position = new Vector3(position.x, position.y, -0.1f);
				yield return new WaitForSeconds(10f);
				if (area.name.Contains("Clone"))
				{
					UnityEngine.Object.Destroy(area);
				}
			}

			private void OnReceiveDreamImpact(On.EnemyDreamnailReaction.orig_RecieveDreamImpact orig, EnemyDreamnailReaction self)
			{
				if (self.name.Contains("Isma"))
				{
					UnityEngine.Object.Instantiate(_dnailEff, base.transform.position, Quaternion.identity);
				}
				if (self.name.Contains("White Defender"))
				{
					Mirror.SetField(self, "convoAmount", 5);
					self.SetConvoTitle("OGRIM_GG_DREAM");
				}
				orig(self);
			}

			private void SpellFlukeOnDoDamage(On.SpellFluke.orig_DoDamage orig, SpellFluke self, GameObject tar, int upwardrecursionamount, bool burst)
			{
				int field = Mirror.GetField<SpellFluke, int>(self, "damage");
				DoTakeDamage(tar, field, 0f);
				orig(self, tar, upwardrecursionamount, burst);
			}

			private void HealthManagerTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
			{
				if (self.gameObject.name.Contains("Isma") && hitInstance.Source.name.Contains("Spike Ball"))
				{
					hitInstance.DamageDealt = 20;
					if (_hm.hp - (int)((float)hitInstance.DamageDealt * hitInstance.Multiplier) <= 0)
					{
						_hm.hp = 2;
						hitInstance.DamageDealt = 1;
						hitInstance.Multiplier = 1f;
					}
				}
				DoTakeDamage(self.gameObject, (int)((float)hitInstance.DamageDealt * hitInstance.Multiplier), hitInstance.Direction);
				if (self.gameObject.name.Contains("White Defender") && _hmDD.hp - (int)((float)hitInstance.DamageDealt * hitInstance.Multiplier) <= 0)
				{
					_hmDD.hp = 2;
					hitInstance.DamageDealt = 1;
					hitInstance.Multiplier = 1f;
				}
				orig(self, hitInstance);
				_hmDD.hp = Math.Max(_healthPool, 1);
				_hm.hp = _healthPool;
			}

			private void HealthManagerDie(On.HealthManager.orig_Die orig, HealthManager self, float? attackDirection, AttackTypes attackType, bool ignoreEvasion)
			{
				if (!self.gameObject.name.Contains("Isma"))
				{
					orig(self, attackDirection, attackType, ignoreEvasion);
				}
			}

			private void DoTakeDamage(GameObject tar, int damage, float dir)
			{
				if (tar.name.Contains("Isma"))
				{
					if (onlyIsma && _waitForHitStart)
					{
						StopCoroutine("Start2");
						_attacking = true;
						_waitForHitStart = false;
						StartCoroutine(BowWhipAttack());
						StartCoroutine(SpawnWalls());
						StartCoroutine(AttackChoice());
					}
					if (!preventDamage)
					{
						_healthPool -= (spawningWalls ? (damage / 2) : damage);
					}
					_hitEffects.RecieveHitEffect(dir);
					_isIsmaHitLast = true;
				}
				else if (tar.name.Contains("White Defender"))
				{
					if (!preventDamage)
					{
						_healthPool -= (spawningWalls ? (damage / 2) : damage);
					}
					_isIsmaHitLast = false;
				}
			}

			private void MarkDungBalls(On.HutongGames.PlayMaker.Actions.ReceivedDamage.orig_OnEnter orig, HutongGames.PlayMaker.Actions.ReceivedDamage self)
			{
				orig(self);
				if (self.Fsm.Name.Contains("Nail Hit") && self.Fsm.GameObject.name.Contains("Dung Ball"))
				{
					Log("Player hit a ball, changing name to prevent Isma hitting it");
					self.Fsm.GameObject.AddComponent<DungTracker>();
				}
			}

			private float FaceHero(bool opposite = false)
			{
				float num = Mathf.Sign(_target.transform.GetPositionX() - base.gameObject.transform.GetPositionX());
				num = (opposite ? (-1f * num) : num);
				Vector3 localScale = base.gameObject.transform.localScale;
				base.gameObject.transform.localScale = new Vector3(Mathf.Abs(localScale.x) * num, localScale.y, 1f);
				return num;
			}

			private IEnumerator SilLeave()
			{
				SpriteRenderer sil = GameObject.Find("Silhouette Isma").GetComponent<SpriteRenderer>();
				sil.transform.localScale *= 1.2f;
				sil.sprite = ArenaFinder.Sprites["Sil_Isma_1"];
				yield return new WaitForSeconds(0.05f);
				sil.sprite = ArenaFinder.Sprites["Sil_Isma_2"];
				yield return new WaitForSeconds(0.05f);
				sil.sprite = ArenaFinder.Sprites["Sil_Isma_3"];
				yield return new WaitForSeconds(0.05f);
				UnityEngine.Object.Destroy(sil.gameObject);
			}

			private void ToggleIsma(bool visible)
			{
				_sr.enabled = visible;
				_bc.enabled = visible;
				_anim.PlayAt("Idle", 0);
			}

			private IEnumerator IdleTimer(float time)
			{
				yield return new WaitForSeconds(time);
				_attacking = false;
			}

			private void PositionIsma()
			{
				float num = 80f;
				float num2 = 0f;
				float positionX = dd.transform.GetPositionX();
				float positionX2 = _target.transform.GetPositionX();
				if (FastApproximately(num, positionX, 2f))
				{
					num2 += 4f;
				}
				if (FastApproximately(num, positionX2, 2f))
				{
					num2 -= 4f;
				}
				num += num2;
				base.gameObject.transform.position = new Vector3(num, GroundY, 1f);
			}

			private void PlayDeathFor(GameObject go)
			{
				GameObject obj = UnityEngine.Object.Instantiate(_ddDeathFx.uninfectedDeathPt);
				GameObject gameObject = UnityEngine.Object.Instantiate(_ddDeathFx.whiteWave);
				obj.SetActive(value: true);
				gameObject.SetActive(value: true);
				Transform obj2 = obj.transform;
				Vector3 position = (gameObject.transform.position = go.transform.position);
				obj2.position = position;
				_ddDeathFx.EmitSound();
				GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
				if (go.name.Contains("Isma"))
				{
					this.PlayAudio(FiveKnights.Clips["IsmaAudDeath"]);
				}
			}

			private void AssignFields(GameObject go)
			{
				HealthManager component = go.GetComponent<HealthManager>();
				HealthManager hmDD = _hmDD;
				foreach (FieldInfo item in from x in typeof(HealthManager).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
					where x.Name.Contains("Prefab")
					select x)
				{
					item.SetValue(component, item.GetValue(hmDD));
				}
				EnemyHitEffectsUninfected component2 = go.GetComponent<EnemyHitEffectsUninfected>();
				EnemyHitEffectsUninfected component3 = dd.GetComponent<EnemyHitEffectsUninfected>();
				FieldInfo[] fields = typeof(EnemyHitEffectsUninfected).GetFields(BindingFlags.Instance | BindingFlags.Public);
				foreach (FieldInfo fieldInfo in fields)
				{
					fieldInfo.SetValue(component2, fieldInfo.Name.Contains("Origin") ? ((object)new Vector3(-0.2f, 1.3f, 0f)) : fieldInfo.GetValue(component3));
				}
				_ddDeathFx = _ddFsm.gameObject.GetComponent<EnemyDeathEffectsUninfected>();
			}

			private bool FastApproximately(float a, float b, float threshold)
			{
				return ((a - b < 0f) ? ((a - b) * -1f) : (a - b)) <= threshold;
			}

			private void PlayVoice()
			{
				AudioClip clip = _randVoice[_rand.Next(0, _randVoice.Count)];
				this.PlayAudio(clip);
			}

			private IEnumerator WaitToAttack()
			{
				yield return new WaitWhile(() => _attacking);
				_attacking = true;
			}

			private void OnDestroy()
			{
				if (_musicCoro != null)
				{
					GameManager.instance.StopCoroutine(_musicCoro);
				}
				On.HealthManager.TakeDamage -= HealthManagerTakeDamage;
				On.HealthManager.Die -= HealthManagerDie;
				On.SpellFluke.DoDamage -= SpellFlukeOnDoDamage;
				On.EnemyDreamnailReaction.RecieveDreamImpact -= OnReceiveDreamImpact;
				On.HutongGames.PlayMaker.Actions.ReceivedDamage.OnEnter -= MarkDungBalls;
				FiveKnights.preloadedGO["AcidSpit"].GetComponentInChildren<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture = _acidTexture;
			}

			private void Log(object o)
			{
				if (FiveKnights.isDebug)
				{
					Modding.Logger.Log("[Isma] " + o);
				}
			}
		}
		public class ManipOrangePuff : MonoBehaviour
		{
			private void Update()
			{
				base.gameObject.SetActive(value: false);
			}
		}
		public class OgrimBG : MonoBehaviour
		{
			public Transform target;

			private Animator _anim;

			private bool lookingRight;

			private void Awake()
			{
				_anim = GetComponent<Animator>();
			}

			private void Start()
			{
				Log("Start following Isma");
				_anim.Play("IdleLeft");
				lookingRight = false;
				StartCoroutine(FollowIsma());
			}

			private IEnumerator FollowIsma()
			{
				while (target != null)
				{
					if (lookingRight && target.position.x < base.transform.position.x)
					{
						yield return _anim.PlayBlocking("LookLeft");
						_anim.Play("IdleLeft");
						lookingRight = false;
					}
					else if (!lookingRight && target.position.x > base.transform.position.x)
					{
						yield return _anim.PlayBlocking("LookRight");
						_anim.Play("IdleRight");
						lookingRight = true;
					}
					else
					{
						yield return null;
					}
				}
			}

			private void Log(object o)
			{
				Modding.Logger.Log("[FiveKnights][OgrimBG] " + o);
			}
		}
		public class PlantCtrl : MonoBehaviour
		{
			private Animator _anim;

			private HealthManager _hm;

			private BoxCollider2D _bc;

			private ShadeOnlyPass _sp;

			private const int PLANTHP = 35;

			private void Awake()
			{
				_anim = base.gameObject.GetComponent<Animator>();
				_bc = base.gameObject.GetComponent<BoxCollider2D>();
				_sp = base.gameObject.AddComponent<ShadeOnlyPass>();
				_sp.disableCollider = _bc;
				_sp.colliderEnabled = false;
				base.gameObject.layer = 13;
				base.transform.position -= new Vector3(0f, 0.2f, 0f);
			}

			private IEnumerator Start()
			{
				yield return null;
				_hm = base.gameObject.AddComponent<HealthManager>();
				SetupHM();
				base.gameObject.AddComponent<Flash>();
				base.gameObject.AddComponent<ExtraDamageable>();
				_hm.hp = 35;
				EnemyHPBarImport.RefreshHPBar(base.gameObject);
				Mirror.SetField(_hm, "enemyType", 3);
				base.gameObject.SetActive(value: true);
				_anim.enabled = true;
				_anim.Play("PlantGrow");
				yield return null;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 2);
				_anim.enabled = false;
				yield return new WaitForSeconds(0.9f);
				_anim.enabled = true;
				this.PlayAudio(FiveKnights.Clips["IsmaAudVineGrow"]);
				yield return new WaitWhile(() => _anim.IsPlaying());
				_bc.isTrigger = false;
				_sp.colliderEnabled = true;
				GameObject obj = new GameObject("PillarPogo");
				obj.layer = 11;
				obj.transform.position = base.gameObject.transform.position;
				obj.transform.rotation = base.gameObject.transform.rotation;
				obj.transform.localScale = base.gameObject.transform.localScale;
				obj.transform.parent = base.gameObject.transform;
				BoxCollider2D boxCollider2D = obj.AddComponent<BoxCollider2D>();
				boxCollider2D.isTrigger = true;
				boxCollider2D.size = _bc.size;
				boxCollider2D.offset = _bc.offset;
				obj.SetActive(value: true);
				obj.transform.parent = base.gameObject.transform;
				obj.AddComponent<PlantHitFx>().hitSound = FiveKnights.Clips["IsmaAudVineHit"];
				yield return new WaitForSeconds(0.55f);
			}

			private void SetupHM()
			{
				HealthManager component = FiveKnights.preloadedGO["WD"].GetComponent<HealthManager>();
				HealthManager component2 = base.gameObject.GetComponent<HealthManager>();
				foreach (FieldInfo item in from x in typeof(HealthManager).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
					where x.Name.Contains("Prefab")
					select x)
				{
					item.SetValue(component2, item.GetValue(component));
				}
			}
		}
		public class PlantHitFx : MonoBehaviour
		{
			private MusicPlayer _ap;

			public AudioClip hitSound;

			private GameObject hitEffectPrefab;

			private GameObject thornCutGrassPrefab;

			private GameObject spatterPrefab;

			private void Start()
			{
				GameObject value = HeroController.instance.gameObject.LocateMyFSM("Spell Control").GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>("Fireball 2", 3).gameObject.Value.LocateMyFSM("Fireball Cast").GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Cast Right", 3).audioPlayer.Value;
				_ap = new MusicPlayer
				{
					Volume = 1f,
					Player = value,
					MaxPitch = 1f,
					MinPitch = 1f,
					Spawn = base.gameObject
				};
				PlayMakerFSM fsm = FiveKnights.preloadedGO["Thorn Collider"].LocateMyFSM("Slash Reaction");
				if (hitSound == null)
				{
					hitSound = fsm.GetAction<HutongGames.PlayMaker.Actions.AudioPlay>("Get Direction", 3).oneShotClip.Value as AudioClip;
				}
				hitEffectPrefab = fsm.GetAction<HutongGames.PlayMaker.Actions.CreateObject>("Hit Right", 3).gameObject.Value;
				thornCutGrassPrefab = fsm.GetAction<HutongGames.PlayMaker.Actions.CreateObject>("Hit Right", 7).gameObject.Value;
				ObjectPool.StartupPool[] startupPools = ObjectPool.instance.startupPools;
				foreach (ObjectPool.StartupPool startupPool in startupPools)
				{
					if (startupPool.prefab.name == "Spatter White R")
					{
						spatterPrefab = startupPool.prefab;
					}
				}
			}

			private void OnTriggerEnter2D(Collider2D other)
			{
				if (other.gameObject.tag == "Nail Attack")
				{
					DoStuff(other.gameObject);
				}
			}

			public void DoStuff(GameObject other)
			{
				_ap.Clip = hitSound;
				_ap.DoPlayRandomClip();
				float value = other.LocateMyFSM("damages_enemy").FsmVariables.FindFsmFloat("direction").Value;
				Vector2 vector = other.transform.position;
				if (value < 45f)
				{
					vector += 1.62f * Vector2.right;
					FlingObjects(spatterPrefab, vector, 110f, 150f);
					GameObject obj = UnityEngine.Object.Instantiate(hitEffectPrefab, vector, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(340f, 380f)));
					obj.transform.SetScaleX(0.85f);
					obj.transform.SetScaleY(0.85f);
					obj.SetActive(value: true);
					UnityEngine.Object.Instantiate(thornCutGrassPrefab, vector, Quaternion.Euler(0f, 270f, 180f)).SetActive(value: true);
				}
				else if (value < 135f)
				{
					vector += 1.1f * Vector2.up;
					FlingObjects(spatterPrefab, vector, 225f, 315f);
					GameObject obj2 = UnityEngine.Object.Instantiate(hitEffectPrefab, vector, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(610f, 650f)));
					obj2.transform.SetScaleX(-0.85f);
					obj2.transform.SetScaleY(0.85f);
					obj2.SetActive(value: true);
					UnityEngine.Object.Instantiate(thornCutGrassPrefab, vector, Quaternion.Euler(90f, 90f, 0f)).SetActive(value: true);
				}
				else if (value < 225f)
				{
					vector += 1.62f * Vector2.left;
					FlingObjects(spatterPrefab, vector, 30f, 70f);
					GameObject obj3 = UnityEngine.Object.Instantiate(hitEffectPrefab, vector, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(340f, 380f)));
					obj3.transform.SetScaleX(-0.85f);
					obj3.transform.SetScaleY(0.85f);
					obj3.SetActive(value: true);
					UnityEngine.Object.Instantiate(thornCutGrassPrefab, vector, Quaternion.Euler(0f, 90f, 0f)).SetActive(value: true);
				}
				else
				{
					vector += 1.26f * Vector2.down;
					FlingObjects(spatterPrefab, vector, 45f, 115f);
					GameObject obj4 = UnityEngine.Object.Instantiate(hitEffectPrefab, vector, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(430f, 470f)));
					obj4.transform.SetScaleX(-0.85f);
					obj4.transform.SetScaleY(0.85f);
					obj4.SetActive(value: true);
					UnityEngine.Object.Instantiate(thornCutGrassPrefab, vector, Quaternion.Euler(270f, 90f, 0f)).SetActive(value: true);
				}
			}

			private void FlingObjects(GameObject go, Vector2 pos, float angleMin, float angleMax)
			{
				if (go != null)
				{
					int num = UnityEngine.Random.Range(2, 4);
					for (int i = 1; i <= num; i++)
					{
						GameObject obj = go.Spawn(pos, Quaternion.identity);
						obj.transform.position += new Vector3(0f, 0.5f);
						float num2 = UnityEngine.Random.Range(14f, 18f);
						float num3 = UnityEngine.Random.Range(angleMin, angleMax);
						float x = num2 * Mathf.Cos(num3 * ((float)Math.PI / 180f));
						float y = num2 * Mathf.Sin(num3 * ((float)Math.PI / 180f));
						obj.GetComponent<Rigidbody2D>().velocity = new Vector2(x, y);
					}
				}
			}
		}
		public class ShadeOnlyPass : MonoBehaviour
		{
			public Collider2D disableCollider;

			public bool colliderEnabled;

			private bool unlocked;

			private EventRegister eventRegister;

			private void Awake()
			{
				eventRegister = GetComponent<EventRegister>();
			}

			private void Start()
			{
				if ((bool)eventRegister)
				{
					eventRegister.OnReceivedEvent += Setup;
				}
				Setup();
			}

			private void OnDestroy()
			{
				if ((bool)eventRegister)
				{
					eventRegister.OnReceivedEvent -= Setup;
				}
			}

			private void Setup()
			{
				if (GameManager.instance.playerData.GetBool("hasShadowDash"))
				{
					unlocked = true;
				}
			}

			private void FixedUpdate()
			{
				if (unlocked && !(disableCollider == null))
				{
					if (HeroController.instance.cState.shadowDashing && disableCollider.enabled)
					{
						disableCollider.enabled = false;
					}
					if (!HeroController.instance.cState.shadowDashing && !disableCollider.enabled)
					{
						disableCollider.enabled = colliderEnabled;
					}
				}
			}
		}
		public class ThornPlantCtrl : MonoBehaviour
		{
			private Animator _anim;

			private MusicPlayer _ap;

			private BoxCollider2D _bc;

			public bool secondWave;

			private void Awake()
			{
				_anim = base.gameObject.GetComponent<Animator>();
				DamageHero damageHero = base.gameObject.AddComponent<DamageHero>();
				damageHero.damageDealt = 1;
				damageHero.hazardType = 1;
				damageHero.shadowDashHazard = false;
				GameObject value = HeroController.instance.gameObject.LocateMyFSM("Spell Control").GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>("Fireball 2", 3).gameObject.Value.LocateMyFSM("Fireball Cast").GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Cast Right", 3).audioPlayer.Value;
				if (value == null)
				{
					Modding.Logger.Log("ERROR: Actor not found.");
				}
				_ap = new MusicPlayer
				{
					Volume = 1f,
					Player = value,
					MaxPitch = 1f,
					MinPitch = 1f,
					Spawn = base.gameObject
				};
				_bc = GetComponent<BoxCollider2D>();
			}

			private IEnumerator Start()
			{
				_anim.enabled = true;
				_anim.Play("ThornPlantGrow");
				_ap.Clip = FiveKnights.Clips["IsmaAudAgonyIntro"];
				_ap.DoPlayRandomClip();
				yield return _anim.WaitToFrame(3);
				_anim.enabled = false;
				yield return new WaitForSeconds(secondWave ? 0.3f : 0.5f);
				_anim.enabled = true;
				_ap.Clip = FiveKnights.Clips["IsmaAudAgonyShoot"];
				_ap.DoPlayRandomClip();
				yield return _anim.WaitToFrame(5);
				_bc.enabled = true;
				yield return _anim.WaitToFrame(8);
				_anim.enabled = false;
				yield return new WaitForSeconds(0.4f);
				_anim.enabled = true;
				_anim.Play("ThornPlantDie");
				yield return _anim.WaitToFrame(3);
				_bc.enabled = false;
				yield return _anim.WaitToFrame(5);
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		public class WhipFlash : MonoBehaviour
		{
			private enum FlashState
			{
				Increase,
				Stay,
				Decrease,
				Stop
			}

			internal class HitCheck : MonoBehaviour
			{
				public event Action OnHit;

				private void OnTriggerEnter2D(Collider2D col)
				{
					if (col.gameObject.layer == 17)
					{
						OnHit?.Invoke();
					}
				}
			}

			private float amount;

			private float t;

			private float timeUp;

			private float stayTime;

			private float timeDown;

			private float flashTimer;

			private bool flashing;

			private FlashState flashingState;

			private float amountCurrent;

			private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");

			private SpriteRenderer _currSR;

			private SpriteRenderer[] _whips;

			private void Start()
			{
				_whips = base.transform.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
				foreach (SpriteRenderer item in _whips.Where((SpriteRenderer x) => x.GetComponent<PolygonCollider2D>()))
				{
					item.gameObject.AddComponent<HitCheck>().OnHit += ResetValues;
				}
			}

			private void ResetValues()
			{
				_currSR = GetWhipActive();
				amount = 0.85f;
				timeUp = 0.01f;
				stayTime = 0.01f;
				timeDown = 0.35f;
				_currSR.material.SetFloat(FlashAmount, 0f);
				flashingState = FlashState.Increase;
				flashTimer = 0f;
			}

			private SpriteRenderer GetWhipActive()
			{
				return _whips.FirstOrDefault((SpriteRenderer x) => x.gameObject.activeSelf);
			}

			private void Update()
			{
				if (_currSR == null)
				{
					return;
				}
				_currSR.material.SetFloat(FlashAmount, 0f);
				_currSR = GetWhipActive();
				if (_currSR == null)
				{
					return;
				}
				_currSR.material = FiveKnights.Materials["flash"];
				if (flashingState == FlashState.Increase)
				{
					if (flashTimer < timeUp)
					{
						flashTimer += Time.deltaTime;
						t = flashTimer / timeUp;
						amountCurrent = Mathf.Lerp(0f, amount, t);
						_currSR.material.SetFloat(FlashAmount, amountCurrent);
					}
					else
					{
						_currSR.material.SetFloat(FlashAmount, amount);
						flashTimer = 0f;
						flashing = false;
						flashingState = FlashState.Stay;
					}
				}
				if (flashingState == FlashState.Stay)
				{
					if (flashTimer < stayTime)
					{
						flashTimer += Time.deltaTime;
					}
					else
					{
						flashTimer = 0f;
						flashingState = FlashState.Decrease;
					}
				}
				if (flashingState == FlashState.Decrease)
				{
					if (flashTimer < timeDown)
					{
						flashTimer += Time.deltaTime;
						t = flashTimer / timeDown;
						amountCurrent = Mathf.Lerp(amount, 0f, t);
						_currSR.material.SetFloat(FlashAmount, amountCurrent);
					}
					else
					{
						_currSR.material.SetFloat(FlashAmount, 0f);
						flashTimer = 0f;
						flashingState = FlashState.Stop;
					}
				}
			}
		}
	}
	namespace FiveKnights.Hegemol
	{
		public class Carriage : MonoBehaviour
		{
			public float speed = 5f;

			public float distance = 90f;

			public float direction;

			private float startx;

			private float newStartx;

			private float xLimit = 407.5f;

			public float Zvalue;

			public float new_Zvalue;

			private void Start()
			{
				Zvalue = base.gameObject.transform.position.z;
				direction = base.gameObject.transform.localScale.x * -1f;
				startx = base.gameObject.transform.position.x;
				newStartx = UnityEngine.Random.Range(startx, startx + distance * direction);
				base.gameObject.transform.position = new Vector3(newStartx, base.gameObject.transform.position.y, base.gameObject.transform.position.z);
			}

			private void Update()
			{
				List<float> list = new List<float>
				{
					Zvalue,
					Zvalue + 0.5f,
					Zvalue + 1f
				};
				new_Zvalue = list[UnityEngine.Random.Range(0, list.Count)];
				if (base.gameObject.transform.position.x * direction - startx * direction < distance && base.gameObject.transform.position.x >= xLimit)
				{
					base.gameObject.transform.position = new Vector3(base.gameObject.transform.position.x + direction * speed * Time.deltaTime, base.gameObject.transform.position.y, base.gameObject.transform.position.z);
				}
				else
				{
					base.gameObject.transform.position = new Vector3(startx, base.gameObject.transform.position.y, new_Zvalue);
				}
			}
		}
		public class Debris : MonoBehaviour
		{
			public enum DebrisType
			{
				NORMAL,
				CC,
				DUNG,
				CRYSTAL
			}

			private float rotSpeed;

			private MusicPlayer _ap;

			private ParticleSystem _pt;

			private bool hitGround;

			public Rigidbody2D rb;

			public float gravityScale;

			public Vector2 vel;

			public float GroundY;

			public DebrisType type;

			private void Awake()
			{
				rb = GetComponent<Rigidbody2D>();
				GameObject value = HeroController.instance.gameObject.LocateMyFSM("Spell Control").GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>("Fireball 2", 3).gameObject.Value.LocateMyFSM("Fireball Cast").GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Cast Right", 3).audioPlayer.Value;
				_ap = new MusicPlayer
				{
					Volume = 1f,
					Player = value,
					MaxPitch = 1f,
					MinPitch = 1f,
					Spawn = base.gameObject
				};
				_pt = base.transform.Find("Particle System").gameObject.GetComponent<ParticleSystem>();
			}

			private void OnEnable()
			{
				rb.gravityScale = gravityScale;
				base.transform.rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
				rotSpeed = (float)((UnityEngine.Random.Range(0, 2) != 0) ? 1 : (-1)) * UnityEngine.Random.Range(200f, 400f);
				rb.velocity = vel;
				switch (type)
				{
				case DebrisType.NORMAL:
					base.transform.Find("Debris" + UnityEngine.Random.Range(0, 3)).gameObject.SetActive(value: true);
					_ap.Clip = FiveKnights.Clips["HegDebris"];
					break;
				case DebrisType.CC:
				{
					base.transform.Find("Debris" + UnityEngine.Random.Range(3, 6)).gameObject.SetActive(value: true);
					_ap.Clip = FiveKnights.Clips["HegDebris"];
					ParticleSystem.MainModule main = _pt.main;
					main.startColor = Color.white;
					break;
				}
				case DebrisType.DUNG:
				{
					base.transform.Find("DebrisDung").gameObject.SetActive(value: true);
					_ap.Clip = FiveKnights.Clips["HegDungDebris"];
					GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["DungBreakChunks"], base.transform);
					_pt = gameObject.GetComponent<ParticleSystem>();
					break;
				}
				case DebrisType.CRYSTAL:
				{
					base.transform.Find("Crystal" + UnityEngine.Random.Range(0, 3)).gameObject.SetActive(value: true);
					_ap.Clip = FiveKnights.Clips["HegDebris"];
					GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Crystal Pt"], base.transform);
					_pt = gameObject.GetComponent<ParticleSystem>();
					ParticleSystem.EmissionModule emission = _pt.emission;
					emission.rateOverTimeMultiplier = 100f;
					break;
				}
				}
			}

			private void FixedUpdate()
			{
				if (base.transform.position.y > GroundY - 0.4f && !hitGround)
				{
					Vector3 eulerAngles = base.transform.rotation.eulerAngles;
					eulerAngles.z += rotSpeed * Time.fixedDeltaTime;
					base.transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
				}
				else if (!hitGround)
				{
					hitGround = true;
					rb.gravityScale = 0f;
					rb.velocity = Vector2.zero;
					base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					UnityEngine.Object.Destroy(GetComponent<CircleCollider2D>());
					_ap.DoPlayRandomClip();
					if (type == DebrisType.CRYSTAL)
					{
						_pt.gameObject.SetActive(value: true);
					}
					else
					{
						_pt.Play();
					}
					SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].enabled = false;
					}
					if (type == DebrisType.DUNG || type == DebrisType.CRYSTAL)
					{
						UnityEngine.Object.Destroy(_pt.gameObject, 4f);
					}
					UnityEngine.Object.Destroy(base.gameObject, 4f);
				}
			}
		}
		public class HegemolController : MonoBehaviour
		{
			private readonly int Phase1HP = ((CustomWP.lev > 0) ? 650 : 600);

			private readonly int Phase2HP = ((CustomWP.lev > 0) ? 800 : 700);

			private readonly int Phase3HP = ((CustomWP.lev > 0) ? 950 : 800);

			private readonly float LeftX = (OWArenaFinder.IsInOverWorld ? 421.6f : ((CustomWP.boss == CustomWP.Boss.All || CustomWP.boss == CustomWP.Boss.Ogrim) ? 60.3f : 11.2f));

			private readonly float RightX = (OWArenaFinder.IsInOverWorld ? 456f : ((CustomWP.boss == CustomWP.Boss.All || CustomWP.boss == CustomWP.Boss.Ogrim) ? 91.7f : 45.7f));

			private readonly float GroundY = (OWArenaFinder.IsInOverWorld ? 155.2f : ((CustomWP.boss == CustomWP.Boss.All || CustomWP.boss == CustomWP.Boss.Ogrim) ? 7.3f : 27.5f));

			private readonly int DreamConvoAmount = (OWArenaFinder.IsInOverWorld ? 3 : 4);

			private readonly string DreamConvoKey = (OWArenaFinder.IsInOverWorld ? "HEG_DREAM" : "HEG_GG_DREAM");

			private const float IdleTime = 0f;

			private int phase = 1;

			private GameObject _ogrim;

			private DamageHero _dh;

			private BoxCollider2D _col;

			private HealthManager _hm;

			private Rigidbody2D _rb;

			private Animator _anim;

			private SpriteRenderer _sr;

			private ExtraDamageable _extraDamageable;

			private GameObject _dreamImpactPrefab;

			private EnemyDreamnailReaction _dreamNailReaction;

			private Flash _flash;

			private EnemyHitEffectsArmoured _hitFx;

			private EnemyDeathEffectsUninfected _deathFx;

			private Mace _mace;

			private GameObject _hitter;

			private GameObject _traitorSlam;

			private Coroutine _musicCoro;

			private bool _attacking;

			private bool _usingGroundPunch;

			private bool _grounded;

			private static LanguageCtrl _langCtrl;

			private Func<IEnumerator> prevAtt2;

			private Func<IEnumerator> prevAtt;

			private Func<IEnumerator> currAtt;

			private bool _staggered;

			private float MiddleX => (LeftX + RightX) / 2f;

			private float Width => RightX - LeftX;

			private void Awake()
			{
				Log("Hegemol Awake");
				_langCtrl = new LanguageCtrl();
				base.gameObject.name = "Hegemol";
				base.gameObject.layer = 26;
				base.transform.position = new Vector3(MiddleX + 5f, GroundY + 3f, 0.02f);
				base.transform.localScale = 1.5f * new Vector3(0f - Mathf.Abs(base.transform.localScale.x), base.transform.localScale.y, base.transform.localScale.z);
				_ogrim = FiveKnights.preloadedGO["WD"];
				_col = base.gameObject.GetComponent<BoxCollider2D>();
				_hm = base.gameObject.AddComponent<HealthManager>();
				_rb = base.gameObject.GetComponent<Rigidbody2D>();
				_anim = base.gameObject.GetComponent<Animator>();
				_sr = base.gameObject.GetComponent<SpriteRenderer>();
				_extraDamageable = base.gameObject.AddComponent<ExtraDamageable>();
				Mirror.SetField(_extraDamageable, "impactClipTable", Mirror.GetField<ExtraDamageable, RandomAudioClipTable>(_ogrim.GetComponent<ExtraDamageable>(), "impactClipTable"));
				Mirror.SetField(_extraDamageable, "audioPlayerPrefab", Mirror.GetField<ExtraDamageable, AudioSource>(_ogrim.GetComponent<ExtraDamageable>(), "audioPlayerPrefab"));
				_dreamImpactPrefab = _ogrim.GetComponent<EnemyDreamnailReaction>().GetAttr<EnemyDreamnailReaction, GameObject>("dreamImpactPrefab");
				_dreamNailReaction = base.gameObject.AddComponent<EnemyDreamnailReaction>();
				Mirror.SetField(_dreamNailReaction, "convoAmount", DreamConvoAmount);
				_dreamNailReaction.SetConvoTitle(DreamConvoKey);
				_flash = base.gameObject.AddComponent<Flash>();
				_flash.enabled = true;
				_hitFx = base.gameObject.AddComponent<EnemyHitEffectsArmoured>();
				_hitFx.enabled = true;
				_deathFx = base.gameObject.AddComponent<EnemyDeathEffectsUninfected>();
				_deathFx.enabled = true;
				_hm.hp = Phase1HP;
				On.EnemyDreamnailReaction.RecieveDreamImpact += OnRecieveDreamImpact;
				On.HealthManager.TakeDamage += OnTakeDamage;
				On.HealthManager.Die += HealthManagerDie;
			}

			private IEnumerator Start()
			{
				_sr.enabled = false;
				while (HeroController.instance == null)
				{
					yield return null;
				}
				yield return new WaitForSeconds(1f);
				GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Mace"], base.transform);
				gameObject.SetActive(value: false);
				_mace = gameObject.AddComponent<Mace>();
				AddDamageToGO(gameObject.transform.Find("Head").gameObject, 2, isAttack: true);
				AddDamageToGO(gameObject.transform.Find("Handle").gameObject, 2, isAttack: true);
				_hitter = base.gameObject.transform.Find("Hitter").gameObject;
				_hitter.AddComponent<NonBouncer>();
				AddDamageToGO(_hitter, 2, isAttack: true);
				AssignFields();
				_hm.hp = Phase1HP;
				EnemyHPBarImport.RefreshHPBar(base.gameObject);
				StartCoroutine(IntroGreet());
			}

			private IEnumerator IntroGreet()
			{
				_sr.enabled = true;
				_anim.Play("Arrive");
				_mace.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 50f, _mace.transform.position.z - 0.01f);
				_mace.transform.localScale = new Vector3(-1f, 1f, 1f);
				_mace.gameObject.SetActive(value: true);
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 1);
				_dh = AddDamageToGO(base.gameObject, 2, isAttack: false);
				GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
				_musicCoro = GameManager.instance.StartCoroutine(MusicControl());
				PlayVoiceClip("HNeutral", random: true, 1f);
				yield return new WaitForSeconds(1f);
				yield return new WaitWhile(() => _anim.IsPlaying());
				GameObject original = null;
				foreach (GameObject item in from x in UnityEngine.Object.FindObjectsOfType<GameObject>()
					where x.name.Contains("Area Title Holder")
					select x)
				{
					original = item.transform.Find("Area Title").gameObject;
				}
				string smallSuper = "Mighty";
				string smallMain = "Hegemol";
				string smallSub = "";
				string sheet = "Reward Room";
				if (_langCtrl.ContainsKey("RR_HEGEMOL_TITLE_SUPER", sheet))
				{
					smallSuper = _langCtrl.Get("RR_HEGEMOL_TITLE_SUPER", sheet);
					smallMain = _langCtrl.Get("RR_HEGEMOL_TITLE_MAIN", sheet);
					smallSub = _langCtrl.Get("RR_HEGEMOL_TITLE_SUB", sheet);
				}
				original = UnityEngine.Object.Instantiate(original);
				original.SetActive(value: true);
				AreaTitleCtrl.ShowBossTitle(this, original, 2f, "", "", "", smallMain, smallSuper, smallSub);
				_attacking = true;
				yield return IntroAttack();
				StartCoroutine(AttackChoice());
			}

			private IEnumerator IntroAttack()
			{
				Log("Intro Grab");
				_mace.gameObject.transform.position = base.transform.position + 50f * Vector3.up;
				_mace.LaunchSpeed = -200f;
				_mace.SpinSpeed = 560f;
				_mace.gameObject.SetActive(value: true);
				yield return new WaitWhile(() => _mace.transform.position.y > base.transform.position.y + 30f);
				_anim.Play("IntroAttack");
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 3);
				_anim.enabled = false;
				yield return new WaitWhile(() => _mace.transform.position.y > base.transform.position.y + 3f);
				_anim.enabled = true;
				Collider2D[] componentsInChildren = _mace.gameObject.GetComponentsInChildren<Collider2D>(includeInactive: true);
				for (int num = 0; num < componentsInChildren.Length; num++)
				{
					componentsInChildren[num].enabled = false;
				}
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 4);
				_mace.gameObject.SetActive(value: false);
				componentsInChildren = _mace.gameObject.GetComponentsInChildren<Collider2D>(includeInactive: true);
				for (int num = 0; num < componentsInChildren.Length; num++)
				{
					componentsInChildren[num].enabled = true;
				}
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 6);
				PlayAudioClip("HegAttackSwing", 2f);
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 9);
				PlayAudioClip("HegAttackHit", 1f);
				yield return new WaitWhile(() => _anim.IsPlaying());
				yield return Turn();
				currAtt = Charge;
				Log("[Attack] " + currAtt.Method.Name);
				StartCoroutine(currAtt());
			}

			private IEnumerator MusicControl()
			{
				Log("Start Music");
				if (OWArenaFinder.IsInOverWorld)
				{
					OWBossManager.PlayMusic(FiveKnights.Clips["HegemolMusicIntro"]);
					yield return new WaitForSecondsRealtime(FiveKnights.Clips["HegemolMusicIntro"].length);
					OWBossManager.PlayMusic(FiveKnights.Clips["HegemolMusicLoop"]);
				}
				else
				{
					GGBossManager.Instance.PlayMusic(FiveKnights.Clips["HegemolMusicIntro"]);
					yield return new WaitForSecondsRealtime(FiveKnights.Clips["HegemolMusicIntro"].length);
					GGBossManager.Instance.PlayMusic(FiveKnights.Clips["HegemolMusicLoop"]);
				}
			}

			private IEnumerator AttackChoice()
			{
				while (true)
				{
					yield return new WaitWhile(() => _attacking);
					_attacking = true;
					if (_usingGroundPunch)
					{
						Log("[Attack] GroundPunch");
						yield return GroundPunch();
						continue;
					}
					yield return Turn();
					prevAtt2 = prevAtt;
					prevAtt = currAtt;
					int[] array = new int[6];
					List<Func<IEnumerator>> list = new List<Func<IEnumerator>> { Jump, Slam, Dig, Charge, MightySlam, MightyDig };
					float num = Math.Abs(base.transform.position.x - ((base.transform.localScale.x > 0f) ? RightX : LeftX));
					array[0] += 2;
					if (num < Width / 3f + 2f)
					{
						array[3] += 4;
						if (phase > 1)
						{
							array[4] += 2;
						}
						if (phase > 2)
						{
							array[5] += 2;
						}
					}
					else if (num < 2f * Width / 2f)
					{
						array[1]++;
						array[2]++;
						array[3]++;
						if (phase > 1)
						{
							array[4]++;
						}
						if (phase > 2)
						{
							array[5]++;
						}
					}
					else
					{
						array[1] += 2;
						array[2] += 2;
						if (phase > 1)
						{
							array[4]++;
						}
						if (phase > 2)
						{
							array[5]++;
						}
					}
					if (prevAtt != null)
					{
						array[list.IndexOf(prevAtt)] = 0;
						if (prevAtt == new Func<IEnumerator>(Jump))
						{
							array[4] = 0;
							array[5] = 0;
							if (prevAtt2 != null && prevAtt2 == new Func<IEnumerator>(Dig))
							{
								array[1]++;
							}
						}
						if (prevAtt == new Func<IEnumerator>(Dig))
						{
							array[1] = 0;
						}
					}
					if (prevAtt2 != null)
					{
						array[list.IndexOf(prevAtt2)]--;
					}
					List<Func<IEnumerator>> list2 = new List<Func<IEnumerator>>();
					for (int num2 = 0; num2 < array.Length; num2++)
					{
						for (int num3 = 0; num3 < array[num2]; num3++)
						{
							list2.Add(list[num2]);
						}
					}
					currAtt = list2[UnityEngine.Random.Range(0, list2.Count)];
					Log("[Attack] " + currAtt.Method.Name);
					StartCoroutine(currAtt());
				}
			}

			private IEnumerator GroundPunch()
			{
				if (_usingGroundPunch)
				{
					_anim.Play("JumpAntic");
					PlayVoiceClip("HCharge", random: false, 1f);
					yield return null;
					yield return new WaitWhile(() => _anim.IsPlaying("JumpAntic"));
					float num = MiddleX - base.gameObject.transform.position.x;
					_rb.gravityScale = 3f;
					_rb.velocity = new Vector2(1.5f * num, 60f);
					_anim.Play("Jump");
					yield return new WaitForSeconds(0.2f);
					yield return new WaitUntil(() => _grounded);
					_anim.Play("Land");
					_rb.velocity = Vector2.zero;
					yield return null;
					yield return new WaitWhile(() => _anim.IsPlaying("Land"));
					_usingGroundPunch = false;
				}
				yield return Turn();
				_anim.Play("PunchAntic");
				_rb.velocity = Vector2.zero;
				yield return null;
				yield return new WaitWhile(() => _anim.IsPlaying("PunchAntic"));
				_mace.LaunchSpeed = 45f;
				_mace.SpinSpeed = 560f * base.transform.localScale.x;
				_mace.transform.localScale = new Vector3(1f, 1f, 1f);
				_mace.transform.position = new Vector3(base.transform.position.x - 0.75f * base.transform.localScale.x, base.transform.position.y + 6f, _mace.transform.position.z);
				_mace.gameObject.SetActive(value: true);
				bool right = base.transform.localScale.x > 0f;
				_anim.Play("Punch");
				for (int i = 0; i < 8; i++)
				{
					if (i == 7)
					{
						_mace.gameObject.SetActive(value: false);
						_mace.gameObject.transform.position = base.transform.position + 70f * Vector3.up;
						_mace.LaunchSpeed = -69f;
						_mace.SpinSpeed = 560f;
						_mace.gameObject.SetActive(value: true);
					}
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 3);
					int amount = ((phase != 3) ? 1 : (1 + i % 2));
					PlayVoiceClip("HGrunt", random: true, 1f);
					PlayAudioClip("HegAttackHit", 1f);
					SpawnShockwaves(base.transform.localScale.x > 0f, 4f, 2.5f, 35f, 1);
					StartCoroutine(SpawnDebris(amount, useLogic: false, 0f));
					GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
					yield return new WaitUntil(() => _anim.GetCurrentFrame() == 0);
					base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
					right = !right;
					if (i == 0)
					{
						_mace.gameObject.SetActive(value: false);
					}
				}
				yield return new WaitWhile(() => _mace.transform.position.y > base.transform.position.y);
				_anim.speed = 1f;
				_anim.enabled = true;
				_mace.gameObject.SetActive(value: false);
				yield return new WaitWhile(() => _anim.IsPlaying());
				_anim.Play("Idle");
				yield return IdleTimer();
			}

			private IEnumerator Jump()
			{
				float diff = ((1 == 0) ? (((base.transform.localScale.x > 0f) ? LeftX : (RightX + base.transform.position.x)) / 2f - base.transform.position.x) : (HeroController.instance.transform.position.x - base.transform.position.x));
				_anim.Play("JumpAntic");
				yield return null;
				yield return new WaitWhile(() => _anim.IsPlaying("JumpAntic"));
				_anim.Play("Jump");
				_rb.gravityScale = 3f;
				_rb.velocity = new Vector2(diff, 60f);
				PlayAudioClip("HegJump", 2f);
				yield return new WaitForSeconds(0.2f);
				yield return new WaitUntil(() => _grounded);
				_anim.Play("Land");
				GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
				_rb.velocity = Vector2.zero;
				PlayAudioClip("HegLand", 1f);
				yield return null;
				yield return new WaitWhile(() => _anim.IsPlaying("Land"));
				yield return IdleTimer();
			}

			private IEnumerator Slam()
			{
				_anim.speed = 1.25f;
				_anim.Play("AttackAntic");
				PlayVoiceClip("HNeutral", random: true, 1f);
				yield return null;
				yield return new WaitWhile(() => _anim.IsPlaying("AttackAntic"));
				_anim.speed = 1f;
				if (phase > 1 && UnityEngine.Random.Range(0, 2) == 1)
				{
					float diff = HeroController.instance.transform.position.x - base.transform.position.x;
					_anim.Play("JumpAntic");
					yield return null;
					yield return new WaitWhile(() => _anim.IsPlaying("JumpAntic"));
					_anim.Play("JumpAttackUp");
					_rb.gravityScale = 3f;
					_rb.velocity = new Vector2(diff, 60f);
					PlayAudioClip("HegJump", 2f);
					yield return new WaitForSeconds(0.2f);
					yield return new WaitWhile(() => _rb.velocity.y > 0f);
					yield return new WaitUntil(() => CheckTerrain(Vector3.down, 8f));
					_anim.Play("JumpAttackHit");
					PlayAudioClip("HegAttackSwing", 2f);
					base.transform.position -= 0.5f * Vector3.down;
					yield return null;
					_anim.enabled = false;
					yield return new WaitUntil(() => CheckTerrain(Vector3.down, 5f));
					_anim.enabled = true;
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 1);
					_anim.enabled = false;
					yield return new WaitUntil(() => _grounded);
					_rb.velocity = new Vector2(0f, _rb.velocity.y);
					_anim.enabled = true;
					yield return new WaitWhile(() => _anim.GetCurrentFrame() < 2);
					SpawnShockwaves(base.transform.localScale.x > 0f, 7f, 2.5f, 35f, 1);
					StartCoroutine(SpawnDebris(phase, useLogic: true, base.transform.position.x));
					PlayAudioClip("HegAttackHit", 1f);
					GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
					_rb.velocity = Vector2.zero;
					PlayAudioClip("HegLand", 1f);
					yield return new WaitWhile(() => _anim.IsPlaying("JumpAttackHit"));
				}
				else
				{
					_anim.Play("Attack");
					PlayAudioClip("HegAttackSwing", 2f);
					yield return null;
					yield return new WaitWhile(() => _anim.IsPlaying("Attack"));
					SpawnShockwaves(base.transform.localScale.x > 0f, 7f, 2.5f, 35f, 1);
					StartCoroutine(SpawnDebris(phase, useLogic: true, base.transform.position.x));
					PlayAudioClip("HegAttackHit", 1f);
					GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
					yield return _anim.PlayBlocking("AttackRecover");
				}
				yield return IdleTimer();
			}

			private IEnumerator Dig()
			{
				_anim.Play("DigAntic");
				yield return null;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 3);
				_anim.enabled = false;
				yield return new WaitForSeconds(0.05f);
				_anim.enabled = true;
				PlayAudioClip("HegAttackSwing", 1f);
				PlayVoiceClip("HGrunt", random: true, 1f);
				yield return new WaitWhile(() => _anim.IsPlaying("DigAntic"));
				PlayAudioClip("HegAttackHit", 1f);
				GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
				_anim.Play("Dig");
				_rb.velocity = 2f * Vector2.right * base.transform.localScale.x;
				yield return new WaitForSeconds(0.4f);
				_anim.Play("DigEnd");
				_rb.velocity = Vector2.zero;
				yield return null;
				PlayAudioClip("HegAttackSwing", 2f);
				SpawnShockwaves(base.transform.localScale.x > 0f, 5f, 1.5f, 15f, 1);
				Vector2 vector = base.transform.position + Mathf.Sign(base.transform.localScale.x) * Vector3.right * 5.5f + 2.6f * Vector3.down;
				for (int num = -2; num < 3; num++)
				{
					Vector2 vector2 = new Vector2(HeroController.instance.transform.position.x + Mathf.Sign(base.transform.localScale.x) * 10f * (float)num, GroundY) - vector;
					Vector2 zero = Vector2.zero;
					float num2 = 1f;
					zero.x = vector2.x / num2;
					zero.y = vector2.y / num2 + 45f * num2;
					GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Debris"], vector, Quaternion.identity);
					AddDamageToGO(gameObject, 1, isAttack: true);
					gameObject.SetActive(value: false);
					Debris debris = gameObject.AddComponent<Debris>();
					debris.gravityScale = 1.5f;
					debris.vel = zero;
					debris.GroundY = GroundY;
					debris.type = ((CustomWP.boss == CustomWP.Boss.All) ? Debris.DebrisType.DUNG : Debris.DebrisType.NORMAL);
					gameObject.SetActive(value: true);
				}
				yield return new WaitWhile(() => _anim.IsPlaying("DigEnd"));
				yield return IdleTimer();
			}

			private IEnumerator Charge()
			{
				PlayVoiceClip("HCharge", random: false, 1f);
				for (int i = 0; i < phase; i++)
				{
					if (i > 0)
					{
						yield return Turn();
					}
					yield return _anim.PlayBlocking("RunAntic");
					if (i == 0)
					{
						yield return new WaitForSeconds(0.3f);
					}
					_anim.Play("Run");
					_anim.speed = 0.75f;
					_rb.velocity = new Vector2(30f * Mathf.Sign(base.transform.localScale.x), 0f);
					yield return new WaitUntil(() => CheckTerrain(base.transform.localScale.x * Vector2.right, 0.1f));
					_anim.speed = 1f;
					if (i == phase - 1)
					{
						PlayVoiceClip("HHeavy", random: true, 1f);
					}
					else
					{
						PlayVoiceClip("HGrunt", random: true, 1f);
					}
					PlayAudioClip("HegShockwave", 1f);
					_anim.Play("Jump");
					_rb.gravityScale = 1.5f;
					_rb.velocity = new Vector2(-7.5f * Mathf.Sign(base.transform.localScale.x), 25f);
					yield return new WaitForSeconds(0.1f);
					yield return new WaitUntil(() => _grounded);
					_rb.velocity = Vector2.zero;
					if (i == phase - 1)
					{
						SpawnShockwaves(facingRight: true, 0f, 1.5f, 20f, 1);
						SpawnShockwaves(facingRight: false, 0f, 1.5f, 20f, 1);
					}
					StartCoroutine(SpawnDebris((i != phase - 1) ? 1 : 2, useLogic: true, (base.transform.position.x < MiddleX) ? RightX : LeftX));
					GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
					PlayAudioClip("HegLand", 1f);
				}
				_anim.Play("Land");
				yield return null;
				yield return new WaitWhile(() => _anim.IsPlaying("Land"));
				yield return IdleTimer();
			}

			private IEnumerator MightySlam()
			{
				yield return JumpBack();
				_anim.Play("AttackAntic");
				PlayVoiceClip("HCharge", random: false, 1f);
				PlayAudioClip("HegAttackCharge", 1f);
				yield return null;
				yield return new WaitWhile(() => _anim.IsPlaying("AttackAntic"));
				_anim.Play("AttackAnticLoop");
				yield return new WaitForSeconds(0.7f);
				_anim.Play("Attack");
				PlayVoiceClip("HGrunt", random: true, 1f);
				PlayAudioClip("HegAttackSwing", 2f);
				yield return null;
				yield return new WaitWhile(() => _anim.IsPlaying("Attack"));
				SpawnPillar(0f - Mathf.Sign(base.transform.localScale.x), new Vector2(2f, 1f), 15f);
				StartCoroutine(SpawnDebris(3, useLogic: true, base.transform.position.x));
				PlayAudioClip("HegAttackHit", 1f);
				GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
				yield return _anim.PlayBlocking("AttackRecover");
				yield return IdleTimer();
			}

			private IEnumerator MightyDig()
			{
				yield return JumpBack();
				_anim.Play("DigAntic");
				yield return null;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 3);
				_anim.enabled = false;
				yield return new WaitForSeconds(0.05f);
				_anim.enabled = true;
				PlayAudioClip("HegAttackSwing", 1f);
				PlayVoiceClip("HGrunt", random: true, 1f);
				yield return new WaitWhile(() => _anim.IsPlaying("DigAntic"));
				PlayAudioClip("HegAttackHit", 1f);
				GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
				_anim.Play("Dig");
				_rb.velocity = 2f * Vector2.right * base.transform.localScale.x;
				yield return new WaitForSeconds(0.2f);
				_anim.Play("DigEnd");
				_rb.velocity = Vector2.zero;
				yield return null;
				PlayAudioClip("HegAttackSwing", 2f);
				Vector2 vector = base.transform.position + Mathf.Sign(base.transform.localScale.x) * Vector3.right * 5.5f + 2.6f * Vector3.down;
				Debris[] debrisArr = new Debris[5];
				for (int num = -2; num < 3; num++)
				{
					Vector2 vector2 = new Vector2(vector.x + Mathf.Sign(base.transform.localScale.x) * 1f * (float)num, base.transform.position.y + 10f) - vector;
					Vector2 zero = Vector2.zero;
					float num2 = 0.8f;
					zero.x = vector2.x / num2;
					zero.y = vector2.y / num2 + 45f * num2;
					GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Debris"], vector, Quaternion.identity);
					AddDamageToGO(gameObject, 1, isAttack: true);
					gameObject.SetActive(value: false);
					Debris debris = gameObject.AddComponent<Debris>();
					debris.gravityScale = 1.5f;
					debris.vel = zero;
					debris.GroundY = GroundY;
					debris.type = ((CustomWP.boss == CustomWP.Boss.All) ? Debris.DebrisType.DUNG : Debris.DebrisType.NORMAL);
					debrisArr[num + 2] = debris;
					gameObject.SetActive(value: true);
				}
				yield return new WaitWhile(() => _anim.IsPlaying("DigEnd"));
				_anim.Play("JumpAntic");
				PlayVoiceClip("HCharge", random: false, 1f);
				yield return null;
				yield return new WaitWhile(() => _anim.IsPlaying("JumpAntic"));
				_anim.Play("JumpAttackUp");
				_rb.gravityScale = 3f;
				_rb.velocity = new Vector2(0f, 60f);
				PlayAudioClip("HegJump", 2f);
				yield return new WaitForSeconds(0.2f);
				yield return new WaitWhile(() => _rb.velocity.y > 0f);
				_anim.Play("JumpAttackHit");
				PlayAudioClip("HegAttackSwing", 2f);
				PlayVoiceClip("HHeavy", random: true, 1f);
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 2);
				for (int num3 = -2; num3 < 3; num3++)
				{
					Debris debris2 = debrisArr[num3 + 2];
					float num4 = ((base.transform.localScale.x > 0f) ? ((RightX + MiddleX) / 2f) : ((LeftX + MiddleX) / 2f));
					Vector2 vector3 = new Vector2(num4 + 4f * (float)num3, GroundY) - (Vector2)debris2.transform.position;
					debris2.rb.velocity = 50f * vector3.normalized;
					debris2.rb.gravityScale = 0f;
				}
				_rb.velocity = Vector2.zero;
				_rb.gravityScale = 0f;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 3);
				_rb.gravityScale = 3f;
				_anim.Play("Jump");
				yield return null;
				yield return new WaitUntil(() => _grounded);
				_anim.Play("Land");
				_rb.velocity = Vector2.zero;
				SpawnShockwaves(facingRight: true, 0f, 1.5f, 40f, 1);
				SpawnShockwaves(facingRight: false, 0f, 1.5f, 40f, 1);
				GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
				PlayAudioClip("HegLand", 1f);
				yield return null;
				yield return new WaitWhile(() => _anim.IsPlaying("Land"));
				yield return IdleTimer();
			}

			private IEnumerator JumpBack()
			{
				float diff = (((base.transform.localScale.x > 0f) ? LeftX : RightX) + base.transform.position.x) / 2f - base.transform.position.x;
				_anim.Play("JumpAntic");
				yield return null;
				yield return new WaitWhile(() => _anim.IsPlaying("JumpAntic"));
				_anim.Play("Jump");
				_rb.gravityScale = 3f;
				_rb.velocity = new Vector2(diff, 60f);
				PlayAudioClip("HegJump", 2f);
				yield return new WaitForSeconds(0.2f);
				yield return new WaitUntil(() => _grounded);
				_anim.Play("Land");
				GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
				_rb.velocity = Vector2.zero;
				PlayAudioClip("HegLand", 1f);
				yield return null;
				yield return new WaitWhile(() => _anim.IsPlaying("Land"));
			}

			private void SpawnShockwaves(bool facingRight, float offset, float scale, float speed, int damage)
			{
				Vector2 vector = base.transform.position;
				GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Mage"].LocateMyFSM("Mage Lord").GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>("Quake Waves", 0).gameObject.Value);
				PlayMakerFSM playMakerFSM = obj.LocateMyFSM("shockwave");
				playMakerFSM.FsmVariables.FindFsmBool("Facing Right").Value = facingRight;
				playMakerFSM.FsmVariables.FindFsmFloat("Speed").Value = speed;
				obj.AddComponent<DamageHero>().damageDealt = damage;
				obj.SetActive(value: true);
				obj.transform.SetPosition2D(new Vector2(vector.x + (float)(facingRight ? 1 : (-1)) * offset, GroundY - 1.4f));
				obj.transform.SetScaleX(scale);
			}

			private void SpawnPillar(float dir, Vector2 size, float xSpd)
			{
				PlayAudioClip("HegShockwave", 1f);
				GameObject gameObject = UnityEngine.Object.Instantiate(_traitorSlam);
				Animator component = gameObject.transform.Find("slash_core").GetComponent<Animator>();
				gameObject.SetActive(value: true);
				component.enabled = true;
				component.Play("mega_mantis_slash_big", -1, 0f);
				gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2((0f - dir) * xSpd, 0f);
				Vector3 position = base.transform.position;
				gameObject.transform.position = new Vector3((0f - dir) * 2.15f + position.x, GroundY - 3.2f, 6.4f);
				gameObject.transform.localScale = new Vector3((0f - dir) * size.x, size.y, 1f);
				if (gameObject.transform.Find("slash_core").Find("hurtbox") != null)
				{
					gameObject.transform.Find("slash_core").Find("hurtbox").gameObject.SetActive(value: false);
				}
				Vector2 offset = gameObject.transform.Find("slash_core").Find("Test2").GetComponent<PolygonCollider2D>()
					.offset;
				offset.y = gameObject.transform.Find("slash_core").Find("hurtbox").GetComponent<PolygonCollider2D>()
					.offset.y - 10f;
			}

			private IEnumerator SpawnDebris(int amount, bool useLogic, float targetX)
			{
				for (int i = 0; i < amount; i++)
				{
					float num = LeftX;
					float num2 = RightX;
					if (!useLogic)
					{
						num += 2f;
						num2 -= 2f;
					}
					else if (targetX > MiddleX)
					{
						num = MiddleX;
					}
					else
					{
						num2 = MiddleX;
					}
					GameObject gameObject = UnityEngine.Object.Instantiate(position: new Vector2(UnityEngine.Random.Range(num, num2), GroundY + 15f), original: FiveKnights.preloadedGO["Debris"], rotation: Quaternion.identity);
					AddDamageToGO(gameObject, 1, isAttack: true);
					gameObject.SetActive(value: false);
					Debris debris = gameObject.AddComponent<Debris>();
					debris.gravityScale = 0f;
					debris.vel = 10f * Vector2.down;
					debris.GroundY = GroundY;
					debris.type = ((CustomWP.boss == CustomWP.Boss.All) ? Debris.DebrisType.CC : Debris.DebrisType.CRYSTAL);
					gameObject.SetActive(value: true);
					yield return new WaitForSeconds(0.5f);
				}
			}

			private IEnumerator Turn()
			{
				if (Mathf.Sign(HeroController.instance.transform.position.x - base.transform.position.x) * Mathf.Sign(base.transform.localScale.x) < 0f)
				{
					base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
					yield return _anim.PlayBlocking("Turn");
				}
			}

			private IEnumerator Stagger()
			{
				Log("Staggered");
				_hm.hp = ((phase == 2) ? Phase2HP : Phase3HP);
				EnemyHPBarImport.RefreshHPBar(base.gameObject);
				_anim.enabled = true;
				_anim.speed = 1f;
				_anim.Play("Stagger");
				_sr.material.SetFloat("_FlashAmount", 0f);
				UnityEngine.Object.Destroy(_dh);
				PlayAudioClip("HegDamageFinal", 1f);
				PlayVoiceClip("HHeavy", random: true, 1f);
				float num = Mathf.Sign(base.transform.position.x - HeroController.instance.transform.position.x);
				_rb.gravityScale = 1.5f;
				_rb.velocity = new Vector2(7.5f * num, 25f);
				yield return null;
				_anim.enabled = false;
				yield return new WaitForSeconds(0.1f);
				while (!_grounded)
				{
					yield return new WaitUntil(() => _grounded || CheckTerrain(base.transform.localScale.x * Vector3.right, 0.1f));
					if (CheckTerrain(base.transform.localScale.x * Vector3.right, 0.1f))
					{
						_rb.velocity = new Vector2(0f - _rb.velocity.x, _rb.velocity.y);
					}
				}
				_anim.enabled = true;
				_rb.velocity = Vector2.zero;
				GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
				PlayAudioClip("HegLand", 1f);
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 4);
				_anim.enabled = false;
				_staggered = true;
				_hm.IsInvincible = false;
				yield return new WaitForSeconds(1f);
				yield return StaggerEnd();
			}

			private IEnumerator StaggerEnd()
			{
				PlayVoiceClip("HNeutral", random: true, 1f);
				_anim.enabled = true;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 10);
				_anim.Play("Idle");
				_rb.gravityScale = 3f;
				_dh = AddDamageToGO(base.gameObject, 2, isAttack: false);
				_usingGroundPunch = true;
				_attacking = false;
				_staggered = false;
				StartCoroutine(AttackChoice());
			}

			private void Update()
			{
				if (_hm.hp <= 0 && phase <= 3)
				{
					phase++;
					StopAllCoroutines();
					PlayDeathFor(base.transform.gameObject);
					if (phase > 3)
					{
						StartCoroutine(Die());
						return;
					}
					StartCoroutine(Stagger());
					Log("Going to phase " + phase);
				}
			}

			private void PlayDeathFor(GameObject go)
			{
				EnemyDeathEffectsUninfected component = FiveKnights.preloadedGO["WD"].GetComponent<EnemyDeathEffectsUninfected>();
				GameObject gameObject = UnityEngine.Object.Instantiate(component.uninfectedDeathPt);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(component.whiteWave);
				gameObject.SetActive(value: true);
				gameObject2.SetActive(value: true);
				Transform obj = gameObject.transform;
				Vector3 position = (gameObject2.transform.position = go.transform.position);
				obj.position = position;
				component.EmitSound();
				GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
			}

			private void OnRecieveDreamImpact(On.EnemyDreamnailReaction.orig_RecieveDreamImpact orig, EnemyDreamnailReaction self)
			{
				orig(self);
				if (self.name.Contains("Hegemol"))
				{
					_dreamImpactPrefab.Spawn(base.transform.position);
				}
			}

			private void OnTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
			{
				orig(self, hitInstance);
				if (self.gameObject.name == "Hegemol")
				{
					if (_staggered)
					{
						StopAllCoroutines();
						StartCoroutine(StaggerEnd());
					}
					_hitFx.RecieveHitEffect(hitInstance.Direction);
					PlayAudioClip("HegDamage", 1f);
				}
			}

			private void HealthManagerDie(On.HealthManager.orig_Die orig, HealthManager self, float? attackDirection, AttackTypes attackType, bool ignoreEvasion)
			{
				if (!self.gameObject.name.Contains("Hegemol") || phase >= 3)
				{
					orig(self, attackDirection, attackType, ignoreEvasion);
				}
			}

			private IEnumerator Die()
			{
				Log("Hegemol Death");
				if (_musicCoro != null)
				{
					GameManager.instance.StopCoroutine(_musicCoro);
				}
				if (OWArenaFinder.IsInOverWorld)
				{
					OWBossManager.PlayMusic(null);
				}
				else
				{
					GGBossManager.Instance.PlayMusic(null, 1f);
				}
				CustomWP.wonLastFight = true;
				if (OWArenaFinder.IsInOverWorld)
				{
					GameManager.instance.AwardAchievement("PALE_COURT_HEG_ACH");
				}
				FiveKnights.journalEntries["Hegemol"].RecordJournalEntry();
				_anim.enabled = true;
				_anim.speed = 1f;
				_anim.Play("Stagger");
				_sr.material.SetFloat("_FlashAmount", 0f);
				UnityEngine.Object.Destroy(_dh);
				base.gameObject.AddComponent<NonBouncer>();
				PlayAudioClip("HegDamageFinal", 1f);
				PlayVoiceClip("HHeavy", random: true, 1f);
				float num = Mathf.Sign(base.transform.position.x - HeroController.instance.transform.position.x);
				_rb.gravityScale = 1.5f;
				_rb.velocity = new Vector2(7.5f * num, 25f);
				yield return null;
				_anim.enabled = false;
				yield return new WaitForSeconds(0.1f);
				while (!_grounded)
				{
					yield return new WaitUntil(() => _grounded || CheckTerrain(base.transform.localScale.x * Vector3.right, 0.1f));
					if (CheckTerrain(base.transform.localScale.x * Vector3.right, 0.1f))
					{
						_rb.velocity = new Vector2(0f - _rb.velocity.x, _rb.velocity.y);
					}
				}
				_anim.enabled = true;
				_rb.velocity = Vector2.zero;
				GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
				PlayAudioClip("HegLand", 1f);
				PlayVoiceClip("HTired", random: true, 1f);
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 4);
				_anim.enabled = false;
				yield return new WaitForSeconds(1f);
				PlayVoiceClip("HNeutral", random: true, 1f);
				yield return new WaitForSeconds(1f);
				_anim.enabled = true;
				yield return null;
				yield return new WaitWhile(() => _anim.IsPlaying("Stagger"));
				_sr.enabled = false;
				yield return new WaitForSeconds(1f);
				UnityEngine.Object.Destroy(base.gameObject);
			}

			private void AssignFields()
			{
				HealthManager component = _ogrim.GetComponent<HealthManager>();
				foreach (FieldInfo item in from x in typeof(HealthManager).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
					where x.Name.Contains("Prefab")
					select x)
				{
					item.SetValue(_hm, item.GetValue(component));
				}
				_traitorSlam = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["TraitorSlam"]);
				_traitorSlam.transform.Find("slash_core").Find("hurtbox").GetComponent<DamageHero>()
					.damageDealt = 2;
				Transform transform = _traitorSlam.transform.Find("slash_core").Find("hurtbox");
				Transform obj = UnityEngine.Object.Instantiate(transform);
				obj.name = "Test2";
				obj.parent = _traitorSlam.transform.Find("slash_core").transform;
				obj.transform.position = transform.transform.position;
				obj.transform.localScale = transform.transform.localScale;
				transform.gameObject.SetActive(value: false);
				_traitorSlam.SetActive(value: false);
			}

			private DamageHero AddDamageToGO(GameObject go, int damage, bool isAttack)
			{
				go.layer = (isAttack ? 22 : 11);
				DamageHero damageHero = go.AddComponent<DamageHero>();
				damageHero.damageDealt = damage;
				damageHero.hazardType = 1;
				damageHero.shadowDashHazard = false;
				return damageHero;
			}

			private IEnumerator IdleTimer()
			{
				Log("[Idle]");
				_anim.Play("Idle");
				yield return new WaitForSeconds(0f);
				_attacking = false;
			}

			private void PlayVoiceClip(string clip, bool random, float volume)
			{
				int num = 0;
				if (random)
				{
					num = clip switch
					{
						"HNeutral" => UnityEngine.Random.Range(1, 4), 
						"HGrunt" => UnityEngine.Random.Range(1, 5), 
						"HTired" => UnityEngine.Random.Range(1, 4), 
						"HHeavy" => UnityEngine.Random.Range(1, 3), 
						_ => 0, 
					};
				}
				if (num != 0)
				{
					clip += num;
				}
				PlayAudioClip(clip, volume);
			}

			private void PlayAudioClip(string clip, float volume)
			{
				this.PlayAudio(FiveKnights.Clips[clip], volume);
			}

			private bool CheckTerrain(Vector3 dir, float distance)
			{
				return Physics2D.BoxCast(_col.bounds.center, _col.bounds.size, 0f, dir, distance, 256);
			}

			private void FixedUpdate()
			{
				_grounded = CheckTerrain(Vector3.down, 0.1f);
			}

			private void OnDestroy()
			{
				if (_musicCoro != null)
				{
					GameManager.instance.StopCoroutine(_musicCoro);
				}
				On.EnemyDreamnailReaction.RecieveDreamImpact -= OnRecieveDreamImpact;
				On.HealthManager.TakeDamage -= OnTakeDamage;
				On.HealthManager.Die -= HealthManagerDie;
			}

			private void Log(object message)
			{
				Modding.Logger.Log("[Hegemol Controller] " + message);
			}
		}
		public class HuskCart : MonoBehaviour
		{
			public float speed = 2.5f;

			public float distance = 90f;

			public float direction;

			private float startx;

			private float newStartx;

			public float Zvalue;

			public float new_Zvalue;

			private float xLimit = 407.5f;

			private void Start()
			{
				Zvalue = base.gameObject.transform.position.z;
				direction = base.gameObject.transform.localScale.x * -1f;
				startx = base.gameObject.transform.position.x;
				newStartx = UnityEngine.Random.Range(startx, startx + distance * direction);
				base.gameObject.transform.position = new Vector3(newStartx, base.gameObject.transform.position.y, base.gameObject.transform.position.z);
			}

			private void Update()
			{
				List<float> list = new List<float>
				{
					Zvalue,
					Zvalue + 0.5f,
					Zvalue + 1f
				};
				new_Zvalue = list[UnityEngine.Random.Range(0, list.Count)];
				if (base.gameObject.transform.position.x * direction - startx * direction < distance && base.gameObject.transform.position.x >= xLimit)
				{
					base.gameObject.transform.position = new Vector3(base.gameObject.transform.position.x + direction * speed * Time.deltaTime, base.gameObject.transform.position.y, base.gameObject.transform.position.z);
				}
				else
				{
					base.gameObject.transform.position = new Vector3(startx, base.gameObject.transform.position.y, new_Zvalue);
				}
			}
		}
		public class HuskCitizen : MonoBehaviour
		{
			public float speed = 2f;

			public float distance = 90f;

			public float direction;

			private float startx;

			private float newStartx;

			public float Zvalue;

			public float new_Zvalue;

			private float xLimit = 407.5f;

			private void Start()
			{
				Zvalue = base.gameObject.transform.position.z;
				direction = base.gameObject.transform.localScale.x;
				startx = base.gameObject.transform.position.x;
				newStartx = UnityEngine.Random.Range(startx, startx + distance * direction);
				base.gameObject.transform.position = new Vector3(newStartx, base.gameObject.transform.position.y, base.gameObject.transform.position.z);
			}

			private void Update()
			{
				List<float> list = new List<float>
				{
					Zvalue,
					Zvalue + 0.5f,
					Zvalue + 1f
				};
				new_Zvalue = list[UnityEngine.Random.Range(0, list.Count)];
				if (base.gameObject.transform.position.x * direction - startx * direction < distance && base.gameObject.transform.position.x >= xLimit)
				{
					base.gameObject.transform.position = new Vector3(base.gameObject.transform.position.x + direction * speed * Time.deltaTime, base.gameObject.transform.position.y, base.gameObject.transform.position.z);
				}
				else
				{
					base.gameObject.transform.position = new Vector3(startx, base.gameObject.transform.position.y, new_Zvalue);
				}
			}
		}
		public class Mace : MonoBehaviour
		{
			public float LaunchSpeed = 45f;

			public float SpinSpeed = -300f;

			private Rigidbody2D _rb;

			private void Awake()
			{
				_rb = GetComponent<Rigidbody2D>();
			}

			private void OnEnable()
			{
				_rb.velocity = new Vector3(0f, LaunchSpeed, 0f);
				base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			}

			private void FixedUpdate()
			{
				Vector3 eulerAngles = base.transform.rotation.eulerAngles;
				eulerAngles.z += SpinSpeed * Time.fixedDeltaTime;
				base.transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
			}
		}
		public class Maggot : MonoBehaviour
		{
			public float speed = 1.5f;

			public float distance = 90f;

			public float direction;

			private float startx;

			private float newStartx;

			public float Zvalue;

			public float new_Zvalue;

			private float xLimit = 407.5f;

			private void Start()
			{
				Zvalue = base.gameObject.transform.position.z;
				direction = base.gameObject.transform.localScale.x * -1f;
				startx = base.gameObject.transform.position.x;
				newStartx = UnityEngine.Random.Range(startx, startx + distance * direction);
				base.gameObject.transform.position = new Vector3(newStartx, base.gameObject.transform.position.y, base.gameObject.transform.position.z);
			}

			private void Update()
			{
				List<float> list = new List<float>
				{
					Zvalue,
					Zvalue + 0.5f,
					Zvalue + 1f
				};
				new_Zvalue = list[UnityEngine.Random.Range(0, list.Count)];
				if (base.gameObject.transform.position.x * direction - startx * direction < distance && base.gameObject.transform.position.x >= xLimit)
				{
					base.gameObject.transform.position = new Vector3(base.gameObject.transform.position.x + direction * speed * Time.deltaTime, base.gameObject.transform.position.y, base.gameObject.transform.position.z);
				}
				else
				{
					base.gameObject.transform.position = new Vector3(startx, base.gameObject.transform.position.y, new_Zvalue);
				}
			}
		}
		public class MineBugCart : MonoBehaviour
		{
			public float speed = 3f;

			public float distance = 90f;

			public float direction;

			private float startx;

			private float newStartx;

			public float Zvalue;

			public float new_Zvalue;

			private float xLimit = 407.5f;

			private void Start()
			{
				Zvalue = base.gameObject.transform.position.z;
				direction = base.gameObject.transform.localScale.x * -1f;
				startx = base.gameObject.transform.position.x;
				newStartx = UnityEngine.Random.Range(startx, startx + distance * direction);
				base.gameObject.transform.position = new Vector3(newStartx, base.gameObject.transform.position.y, base.gameObject.transform.position.z);
			}

			private void Update()
			{
				List<float> list = new List<float>
				{
					Zvalue,
					Zvalue + 0.5f,
					Zvalue + 1f
				};
				new_Zvalue = list[UnityEngine.Random.Range(0, list.Count)];
				if (base.gameObject.transform.position.x * direction - startx * direction < distance && base.gameObject.transform.position.x >= xLimit)
				{
					base.gameObject.transform.position = new Vector3(base.gameObject.transform.position.x + direction * speed * Time.deltaTime, base.gameObject.transform.position.y, base.gameObject.transform.position.z);
				}
				else
				{
					base.gameObject.transform.position = new Vector3(startx, base.gameObject.transform.position.y, new_Zvalue);
				}
			}
		}
		public class Tram : MonoBehaviour
		{
			private PlayMakerFSM _ctrl;

			private AudioSource _aud;

			private Rigidbody2D _rb;

			private bool _hasStopped;

			private bool _playerPassed;

			private static readonly Vector2 MaxVel = new Vector2(30f, 0f);

			private static readonly Vector3 Left = new Vector3(230f, 171.1914f, 41.88f);

			private static readonly Vector3 Right = new Vector3(530f, 171.1914f, 41.88f);

			private static readonly Vector3 StopAt = new Vector3(437f, 171.1914f, 41.88f);

			private void Awake()
			{
				_ctrl = base.gameObject.LocateMyFSM("Tram Control");
				_ctrl.enabled = false;
				FsmEventTarget eventTarget = _ctrl.GetAction<HutongGames.PlayMaker.Actions.SendEventByName>("Door Open", 4).eventTarget;
				_ctrl.Fsm.Event(eventTarget, "UP");
				_rb = base.gameObject.AddComponent<Rigidbody2D>();
				_rb.isKinematic = true;
				_rb.gravityScale = 0f;
				_aud = base.gameObject.GetComponent<AudioSource>();
				_aud.Play();
				_aud.maxDistance = 100f;
				Modding.Logger.Log("Did a thing with tram did light?");
			}

			private IEnumerator Start()
			{
				base.transform.position = Left;
				_hasStopped = false;
				_playerPassed = false;
				yield return new WaitWhile(() => HeroController.instance.transform.position.x < base.transform.position.x);
				_playerPassed = true;
				_rb.velocity = MaxVel;
			}

			private IEnumerator Accelerate(float from, float to)
			{
				float duration = 2f;
				float time = 0f;
				while (time < duration)
				{
					time += Time.deltaTime;
					float x = Mathf.Lerp(from, to, time / duration);
					_rb.velocity = new Vector2(x, 0f);
					yield return null;
				}
				_rb.velocity = new Vector2(to, 0f);
			}

			private void FixedUpdate()
			{
				if (_playerPassed)
				{
					if (base.transform.position.x > Right.x)
					{
						base.transform.position = Left;
						_hasStopped = false;
						_rb.velocity = MaxVel;
					}
					else if (!_hasStopped && base.transform.position.x > StopAt.x - 30f)
					{
						_hasStopped = true;
						StartCoroutine(DelayedEnd());
					}
				}
			}

			private IEnumerator DelayedStart()
			{
				yield return new WaitForSeconds(3f);
				_aud.loop = true;
				_aud.Play();
				yield return Accelerate(0f, MaxVel.x);
			}

			private IEnumerator DelayedEnd()
			{
				_aud.loop = false;
				yield return Accelerate(MaxVel.x, 0f);
				yield return DelayedStart();
			}

			public void FadeAudio()
			{
				StartCoroutine(Fade());
				IEnumerator Fade()
				{
					float changeCounter = 0f;
					while (changeCounter < 1f)
					{
						_aud.volume -= Time.deltaTime / 1.5f;
						yield return null;
					}
				}
			}
		}
		public class TramSmall : MonoBehaviour
		{
			private PlayMakerFSM _ctrl;

			private Rigidbody2D _rb;

			private static readonly Vector2 TramVel = new Vector2(-50f, 0f);

			private static readonly Vector3 StartPos = new Vector3(520f, 188f, 102.6f);

			private static readonly Vector3 EndPos = new Vector3(230f, 188f, 102.6f);

			private void Awake()
			{
				_ctrl = base.gameObject.LocateMyFSM("Tram Control");
				_ctrl.enabled = false;
				FsmEventTarget eventTarget = _ctrl.GetAction<HutongGames.PlayMaker.Actions.SendEventByName>("Door Open", 4).eventTarget;
				_ctrl.Fsm.Event(eventTarget, "UP");
				_rb = base.gameObject.AddComponent<Rigidbody2D>();
				_rb.isKinematic = true;
				_rb.gravityScale = 0f;
				base.gameObject.GetComponent<AudioSource>().Play();
				base.gameObject.GetComponent<AudioSource>().maxDistance = 100f;
			}

			private void Start()
			{
				base.transform.position = StartPos;
				_rb.velocity = TramVel;
			}

			private void FixedUpdate()
			{
				if (base.transform.position.x < EndPos.x)
				{
					base.transform.position = StartPos;
				}
			}
		}
	}
	namespace FiveKnights.Dryya
	{
		public class Dagger : MonoBehaviour
		{
			private const float Speed = 37f;

			private void Start()
			{
				StartCoroutine(WaitDestroy());
			}

			private IEnumerator WaitDestroy()
			{
				yield return new WaitForSeconds(10f);
				UnityEngine.Object.Destroy(base.gameObject);
			}

			private void FixedUpdate()
			{
				base.transform.position += base.transform.up * Time.fixedDeltaTime * 37f;
			}
		}
		public class DryyaCorpse : MonoBehaviour
		{
			private void Start()
			{
				this.PlayAudio(FiveKnights.Clips["DryyaVoiceDeath"]);
			}

			private void Log(object message)
			{
				Modding.Logger.Log("[Dryya Corpse] " + message);
			}
		}
		public class DryyaSetup : MonoBehaviour
		{
			private readonly int MaxHP = ((CustomWP.lev > 0) ? 1700 : 1500);

			private readonly int Phase2HP = ((CustomWP.lev > 0) ? 1150 : 1000);

			private readonly int Phase3HP = ((CustomWP.lev > 0) ? 300 : 250);

			private readonly float LeftX = (OWArenaFinder.IsInOverWorld ? 422f : 61f);

			private readonly float RightX = (OWArenaFinder.IsInOverWorld ? 455f : 91f);

			private readonly float GroundY = (OWArenaFinder.IsInOverWorld ? 101.0837f : 10.625f);

			private readonly float SlamY = (OWArenaFinder.IsInOverWorld ? 96.5f : ((CustomWP.boss == CustomWP.Boss.All) ? 5.7f : 5.9f));

			private readonly int DreamConvoAmount = (OWArenaFinder.IsInOverWorld ? 3 : 4);

			private readonly string DreamConvoKey = (OWArenaFinder.IsInOverWorld ? "DRYYA_DREAM" : "DRYYA_GG_DREAM");

			private readonly int MaxStaggerHits = 12;

			private int _hitsTaken;

			private bool _doneFrenzyKnockout;

			private PlayMakerFSM _mageLord;

			private PlayMakerFSM _control;

			private EnemyDeathEffectsUninfected _deathEffects;

			private EnemyDreamnailReaction _dreamNailReaction;

			private EnemyHitEffectsUninfected _hitEffects;

			private ExtraDamageable _extraDamageable;

			private HealthManager _hm;

			private SpriteFlash _spriteFlash;

			private GameObject _corpse;

			private tk2dSprite _sprite;

			private tk2dSpriteAnimator _anim;

			private Rigidbody2D _rb;

			private BoxCollider2D _bc;

			private GameObject _diveShockwave;

			private GameObject _ogrim;

			private GameObject _slash1Collider1;

			private GameObject _slash1Collider2;

			private GameObject _slash2Collider1;

			private GameObject _slash2Collider2;

			private GameObject _slash3Collider1;

			private GameObject _slash3Collider2;

			private GameObject _cheekySlashCollider1;

			private GameObject _cheekySlashCollider2;

			private GameObject _cheekySlashCollider3;

			private List<GameObject> _slashes;

			private GameObject _stabFlash;

			private List<ElegyBeam> _elegyBeams;

			private static LanguageCtrl _langCtrl;

			private Coroutine _daggersCoro;

			private GameObject _dreamImpactPrefab;

			private void Awake()
			{
				_langCtrl = new LanguageCtrl();
				base.gameObject.SetActive(value: true);
				base.gameObject.layer = 11;
				_corpse = base.gameObject.FindGameObjectInChildren("Corpse Dryya");
				_diveShockwave = base.gameObject.FindGameObjectInChildren("Dive Shockwave");
				_slash1Collider1 = base.gameObject.FindGameObjectInChildren("Slash 1 Collider 1");
				_slash1Collider2 = base.gameObject.FindGameObjectInChildren("Slash 1 Collider 2");
				_slash2Collider1 = base.gameObject.FindGameObjectInChildren("Slash 2 Collider 1");
				_slash2Collider2 = base.gameObject.FindGameObjectInChildren("Slash 2 Collider 2");
				_slash3Collider1 = base.gameObject.FindGameObjectInChildren("Slash 3 Collider 1");
				_slash3Collider2 = base.gameObject.FindGameObjectInChildren("Slash 3 Collider 2");
				_cheekySlashCollider1 = base.gameObject.FindGameObjectInChildren("Slash Collider 1");
				_cheekySlashCollider2 = base.gameObject.FindGameObjectInChildren("Slash Collider 2");
				_cheekySlashCollider3 = base.gameObject.FindGameObjectInChildren("Slash Collider 3");
				_slashes = new List<GameObject> { _slash1Collider1, _slash1Collider2, _slash2Collider1, _slash2Collider2, _slash3Collider1, _slash3Collider2, _cheekySlashCollider1, _cheekySlashCollider2, _cheekySlashCollider3 };
				_slashes.AddRange(from d in base.transform.Find("Super").gameObject.GetComponentsInChildren<DamageHero>()
					select d.gameObject);
				_stabFlash = base.gameObject.FindGameObjectInChildren("Stab Flash");
				_ogrim = FiveKnights.preloadedGO["WD"];
				_dreamImpactPrefab = _ogrim.GetComponent<EnemyDreamnailReaction>().GetAttr<EnemyDreamnailReaction, GameObject>("dreamImpactPrefab");
				_mageLord = FiveKnights.preloadedGO["Mage"].LocateMyFSM("Mage Lord");
				_control = base.gameObject.LocateMyFSM("Control");
				AddComponents();
				GetComponents();
				AssignFields();
				_control.SetState("Init");
				_control.Fsm.GetFsmGameObject("Hero").Value = HeroController.instance.gameObject;
				_control.Fsm.GetFsmBool("GG Form").Value = false;
				_control.Fsm.GetFsmFloat("Ground").Value = GroundY;
				_control.Fsm.GetFsmInt("Phase 2 HP").Value = Phase2HP;
				_control.Fsm.GetFsmInt("Phase 3 HP").Value = Phase3HP;
				_control.InsertMethod("Activate", 0, delegate
				{
					_hm.enabled = true;
				});
				_control.InsertMethod("Phase Check", 0, delegate
				{
					_control.Fsm.GetFsmInt("HP").Value = _hm.hp;
				});
				_control.InsertMethod("Knockout Transition", 0, delegate
				{
					_control.Fsm.GetFsmInt("HP").Value = _hm.hp;
				});
				_control.InsertMethod("Counter Stance", 0, delegate
				{
					_hm.IsInvincible = true;
					if (base.transform.localScale.x == 1f)
					{
						_hm.InvincibleFromDirection = 8;
					}
					else if (base.transform.localScale.x == -1f)
					{
						_hm.InvincibleFromDirection = 9;
					}
					_spriteFlash.flashFocusHeal();
					Vector2 vector = base.transform.position + Vector3.right * 1.3f * (0f - base.transform.localScale.x) + Vector3.up * 0.1f;
					Quaternion rotation = Quaternion.Euler(0f, 0f, (0f - base.transform.localScale.x) * -60f);
					UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["CounterFX"], vector, rotation).SetActive(value: true);
				});
				_control.InsertMethod("Counter End", 0, delegate
				{
					_hm.IsInvincible = false;
				});
				_control.InsertMethod("Counter Slash Antic", 0, delegate
				{
					_hm.IsInvincible = false;
				});
				_control.InsertCoroutine("Countered", 0, () => GameManager.instance.FreezeMoment(0.04f, 0.35f, 0.04f, 0f));
				_control.InsertMethod("Dive Land Heavy", 0, delegate
				{
					SpawnShockwaves(1.3f, 25f, 1);
				});
				ModifyBeams();
				ModifySuper();
				ModifyAudio();
				ModifyDaggers();
				GameCameras.instance.cameraShakeFSM.FsmVariables.FindFsmBool("RumblingMed").Value = false;
				_hm.OnDeath += DeathHandler;
				On.EnemyDreamnailReaction.RecieveDreamImpact += OnReceiveDreamImpact;
				On.HealthManager.TakeDamage += OnTakeDamage;
			}

			private IEnumerator Start()
			{
				Rigidbody2D rb = base.gameObject.GetComponent<Rigidbody2D>();
				yield return new WaitWhile(() => rb.velocity.y == 0f);
				yield return new WaitWhile(() => rb.velocity.y != 0f);
				MusicControl();
				GameObject original = null;
				foreach (GameObject item in from x in UnityEngine.Object.FindObjectsOfType<GameObject>()
					where x.name.Contains("Area Title Holder")
					select x)
				{
					original = item.transform.Find("Area Title").gameObject;
				}
				string smallSuper = "Fierce";
				string smallMain = "Dryya";
				string smallSub = "";
				string sheet = "Reward Room";
				if (_langCtrl.ContainsKey("RR_DRYYA_TITLE_SUPER", sheet))
				{
					smallSuper = _langCtrl.Get("RR_DRYYA_TITLE_SUPER", sheet);
					smallMain = _langCtrl.Get("RR_DRYYA_TITLE_MAIN", sheet);
					smallSub = _langCtrl.Get("RR_DRYYA_TITLE_SUB", sheet);
				}
				original = UnityEngine.Object.Instantiate(original);
				original.SetActive(value: true);
				AreaTitleCtrl.ShowBossTitle(this, original, 2f, "", "", "", smallMain, smallSuper, smallSub);
			}

			private void MusicControl()
			{
				if (!OWArenaFinder.IsInOverWorld)
				{
					GGBossManager.Instance.PlayMusic(FiveKnights.Clips["DryyaMusic"], 1f);
				}
				else
				{
					OWBossManager.PlayMusic(FiveKnights.Clips["DryyaMusic"]);
				}
			}

			private void DeathHandler()
			{
				if (!OWArenaFinder.IsInOverWorld)
				{
					GGBossManager.Instance.PlayMusic(null, 1f);
				}
				CustomWP.wonLastFight = true;
				if (OWArenaFinder.IsInOverWorld)
				{
					GameManager.instance.AwardAchievement("PALE_COURT_DRYYA_ACH");
				}
			}

			private void OnReceiveDreamImpact(On.EnemyDreamnailReaction.orig_RecieveDreamImpact orig, EnemyDreamnailReaction self)
			{
				orig(self);
				if (self.name.Contains("Dryya"))
				{
					_spriteFlash.flashDreamImpact();
					_dreamImpactPrefab.Spawn(base.transform.position);
				}
			}

			private void OnTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
			{
				orig(self, hitInstance);
				if (!self.gameObject.name.Contains("Dryya"))
				{
					return;
				}
				_spriteFlash.flashFocusHeal();
				_hitsTaken++;
				if (_control.ActiveStateName == "Knockout Wait")
				{
					_control.SendEvent("END");
				}
				if (_doneFrenzyKnockout || (_hm.hp > Phase3HP && (_hitsTaken < MaxStaggerHits || _hm.hp < 2 * Phase3HP)))
				{
					return;
				}
				if (_elegyBeams != null)
				{
					foreach (ElegyBeam item in _elegyBeams.Where((ElegyBeam x) => x != null))
					{
						UnityEngine.Object.Destroy(item.gameObject);
					}
				}
				if (_daggersCoro != null)
				{
					StopCoroutine(_daggersCoro);
				}
				Log("Stagger");
				_hm.IsInvincible = false;
				_bc.isTrigger = false;
				_anim.Resume();
				_control.SendEvent("STAGGER");
				_hitsTaken = 0;
				this.PlayAudio(FiveKnights.Clips["DryyaVoiceDeath"]);
				if (_hm.hp <= Phase3HP)
				{
					_doneFrenzyKnockout = true;
				}
				EnemyDeathEffectsUninfected component = FiveKnights.preloadedGO["WD"].GetComponent<EnemyDeathEffectsUninfected>();
				GameObject gameObject = UnityEngine.Object.Instantiate(component.uninfectedDeathPt);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(component.whiteWave);
				gameObject.SetActive(value: true);
				gameObject2.SetActive(value: true);
				Transform obj = gameObject.transform;
				Vector3 position = (gameObject2.transform.position = base.transform.position);
				obj.position = position;
				component.EmitSound();
				GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
			}

			private void AddComponents()
			{
				_deathEffects = base.gameObject.AddComponent<EnemyDeathEffectsUninfected>();
				_deathEffects.SetJournalEntry(FiveKnights.journalEntries["Dryya"]);
				_dreamNailReaction = base.gameObject.AddComponent<EnemyDreamnailReaction>();
				_dreamNailReaction.enabled = true;
				Mirror.SetField(_dreamNailReaction, "convoAmount", DreamConvoAmount);
				_dreamNailReaction.SetConvoTitle(DreamConvoKey);
				_hitEffects = base.gameObject.AddComponent<EnemyHitEffectsUninfected>();
				_hitEffects.enabled = true;
				_hm = base.gameObject.AddComponent<HealthManager>();
				_hm.enabled = false;
				_hm.hp = MaxHP;
				EnemyHPBarImport.RefreshHPBar(base.gameObject);
				_spriteFlash = base.gameObject.AddComponent<SpriteFlash>();
				_extraDamageable = base.gameObject.AddComponent<ExtraDamageable>();
				Mirror.SetField(_extraDamageable, "impactClipTable", Mirror.GetField<ExtraDamageable, RandomAudioClipTable>(_ogrim.GetComponent<ExtraDamageable>(), "impactClipTable"));
				Mirror.SetField(_extraDamageable, "audioPlayerPrefab", Mirror.GetField<ExtraDamageable, AudioSource>(_ogrim.GetComponent<ExtraDamageable>(), "audioPlayerPrefab"));
				PlayMakerFSM obj = FiveKnights.preloadedGO["Slash"].LocateMyFSM("nail_clash_tink");
				foreach (GameObject slash in _slashes)
				{
					PlayMakerFSM obj2 = slash.AddComponent<PlayMakerFSM>();
					FieldInfo[] fields = typeof(PlayMakerFSM).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					foreach (FieldInfo fieldInfo in fields)
					{
						fieldInfo.SetValue(obj2, fieldInfo.GetValue(obj));
					}
				}
				_stabFlash.AddComponent<DeactivateAfter2dtkAnimation>();
			}

			private void GetComponents()
			{
				_sprite = GetComponent<tk2dSprite>();
				_anim = GetComponent<tk2dSpriteAnimator>();
				_rb = GetComponent<Rigidbody2D>();
				_bc = GetComponent<BoxCollider2D>();
				EnemyDeathEffects component = GetComponent<EnemyDeathEffects>();
				component.corpseSpawnPoint = base.transform.position + Vector3.up * 2f;
				GameObject gameObject = UnityEngine.Object.Instantiate(_corpse);
				gameObject.SetActive(value: false);
				gameObject.AddComponent<DryyaCorpse>();
				component.SetAttr("corpsePrefab", gameObject);
				component.SetAttr("corpseFlingSpeed", 25f);
				Shader shader = _ogrim.GetComponent<tk2dSprite>().Collection.spriteDefinitions[0].material.shader;
				tk2dSpriteDefinition[] spriteDefinitions = _sprite.Collection.spriteDefinitions;
				for (int i = 0; i < spriteDefinitions.Length; i++)
				{
					spriteDefinitions[i].material.shader = shader;
				}
				spriteDefinitions = _diveShockwave.GetComponent<tk2dSprite>().Collection.spriteDefinitions;
				for (int i = 0; i < spriteDefinitions.Length; i++)
				{
					spriteDefinitions[i].material.shader = Shader.Find("tk2d/BlendVertexColor");
				}
				spriteDefinitions = _stabFlash.GetComponent<tk2dSprite>().Collection.spriteDefinitions;
				for (int i = 0; i < spriteDefinitions.Length; i++)
				{
					spriteDefinitions[i].material.shader = Shader.Find("tk2d/BlendVertexColor");
				}
			}

			private void AssignFields()
			{
				EnemyDeathEffectsUninfected component = _ogrim.GetComponent<EnemyDeathEffectsUninfected>();
				FieldInfo[] fields = typeof(EnemyDeathEffectsUninfected).GetFields(BindingFlags.Instance | BindingFlags.Public);
				foreach (FieldInfo fieldInfo in fields)
				{
					fieldInfo.SetValue(_deathEffects, fieldInfo.GetValue(component));
				}
				EnemyHitEffectsUninfected component2 = _ogrim.GetComponent<EnemyHitEffectsUninfected>();
				fields = typeof(EnemyHitEffectsUninfected).GetFields(BindingFlags.Instance | BindingFlags.Public);
				foreach (FieldInfo fieldInfo2 in fields)
				{
					fieldInfo2.SetValue(_hitEffects, fieldInfo2.GetValue(component2));
				}
				HealthManager component3 = _ogrim.GetComponent<HealthManager>();
				foreach (FieldInfo item in from x in typeof(HealthManager).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
					where x.Name.Contains("Prefab")
					select x)
				{
					item.SetValue(_hm, item.GetValue(component3));
				}
			}

			private void Update()
			{
				Vector2 vector = base.transform.position;
				if (vector.x > RightX && _rb.velocity.x > 0f)
				{
					base.transform.position = new Vector3(RightX, vector.y);
					_rb.velocity = new Vector2(0f, _rb.velocity.y);
				}
				else if (vector.x < LeftX && _rb.velocity.x < 0f)
				{
					base.transform.position = new Vector3(LeftX, vector.y);
					_rb.velocity = new Vector2(0f, _rb.velocity.y);
				}
			}

			private void SpawnShockwaves(float vertScale, float speed, int damage)
			{
				bool[] obj = new bool[2] { false, true };
				Vector2 vector = base.transform.position;
				bool[] array = obj;
				foreach (bool flag in array)
				{
					GameObject obj2 = UnityEngine.Object.Instantiate(_mageLord.GetAction<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>("Quake Waves", 0).gameObject.Value);
					PlayMakerFSM playMakerFSM = obj2.LocateMyFSM("shockwave");
					playMakerFSM.FsmVariables.FindFsmBool("Facing Right").Value = flag;
					playMakerFSM.FsmVariables.FindFsmFloat("Speed").Value = speed;
					obj2.AddComponent<DamageHero>().damageDealt = damage;
					obj2.SetActive(value: true);
					obj2.transform.SetPosition2D(new Vector2(vector.x + (flag ? 0.5f : (-0.5f)), SlamY));
					obj2.transform.SetScaleX(vertScale);
				}
			}

			private void ModifyDaggers()
			{
				Vasi.FsmUtil.AddMethod(_control.GetState("Dagger Throw"), delegate
				{
					if (_daggersCoro != null)
					{
						StopCoroutine(_daggersCoro);
					}
					_daggersCoro = StartCoroutine(DaggerThrow());
				});
			}

			private IEnumerator DaggerThrow()
			{
				_bc.isTrigger = true;
				Vector3 localScale = base.transform.localScale;
				Mathf.Sign(localScale.x);
				_rb.velocity = new Vector2(0f, 0f);
				_rb.gravityScale = 0f;
				_rb.isKinematic = true;
				base.transform.position -= new Vector3(0f, 1.7f, 0f);
				_anim.Play("Throw");
				yield return new WaitWhile(() => _anim.CurrentFrame < 3);
				_anim.Pause();
				yield return new WaitForSeconds(0.2f);
				_anim.Resume();
				yield return new WaitWhile(() => _anim.CurrentFrame < 5);
				_anim.ClipFps = 24f;
				PlayVoice("Alt");
				float signX = Mathf.Sign(base.transform.position.x - HeroController.instance.transform.position.x);
				localScale = new Vector3(Mathf.Abs(localScale.x) * signX, localScale.y, localScale.z);
				base.transform.localScale = localScale;
				_rb.velocity = new Vector2(signX * 55f, 25f);
				Log("Daggers - Jump");
				yield return new WaitWhile(() => _anim.CurrentFrame < 8);
				PlayAudio("Jump");
				_rb.velocity = new Vector2(signX * 30f, _rb.velocity.y);
				yield return new WaitWhile(() => _anim.CurrentFrame < 9);
				_rb.gravityScale = 3f;
				_rb.isKinematic = false;
				yield return new WaitWhile(() => _anim.CurrentFrame < 10);
				_anim.ClipFps = 12f;
				StartCoroutine(SpawnDaggers());
				Log("Daggers - Landed");
				yield return new WaitWhile(() => base.transform.position.y > GroundY - 1.7f);
				PlayAudio("Land");
				base.transform.position = new Vector3(base.transform.position.x, GroundY);
				_bc.isTrigger = false;
				_rb.velocity = new Vector2(0f, 0f);
				_rb.gravityScale = 1f;
				_control.SetState("Idle");
			}

			private IEnumerator SpawnDaggers()
			{
				float num = base.transform.position.y - HeroController.instance.transform.position.y;
				float num2 = base.transform.position.x - HeroController.instance.transform.position.x;
				float num3 = Mathf.Sqrt(num * num + num2 * num2);
				float num4 = 57.29578f * Mathf.Asin(num2 / num3);
				float startAngle = 180f - num4;
				for (int i = -2; i < 3; i++)
				{
					PlayAudio("Dagger Throw");
					GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Dagger"], base.transform.position, Quaternion.Euler(0f, 0f, startAngle + 20f * (float)i));
					obj.AddComponent<Dagger>();
					obj.AddComponent<Tink>();
					obj.AddComponent<Pogoable>();
					obj.SetActive(value: true);
					yield return new WaitForSeconds(0.01f);
				}
			}

			private void ModifyBeams()
			{
				string[] array = new string[5] { "Beams Slash 1", "Beams Slash 2", "Beams Slash 3", "Beams Slash 4", "Beams Slash 5" };
				foreach (string state in array)
				{
					_control.InsertMethod(state, delegate
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Beams"]);
						gameObject.transform.localScale = new Vector3((gameObject.transform.localScale.x * (float)UnityEngine.Random.Range(0, 2) == 0f) ? (-1f) : 1f, (gameObject.transform.localScale.y * (float)UnityEngine.Random.Range(0, 2) == 0f) ? (-1f) : 1f, gameObject.transform.localScale.z);
						ElegyBeam elegyBeam = gameObject.AddComponent<ElegyBeam>();
						elegyBeam.offset = ((state != "Beams Slash 1") ? new Vector2(UnityEngine.Random.Range(-15f, 15f), UnityEngine.Random.Range(-7.5f, 7.5f)) : Vector2.zero);
						_elegyBeams.Add(elegyBeam);
					}, 1);
				}
				_control.InsertMethod("Beams Slash 1", delegate
				{
					_elegyBeams = new List<ElegyBeam>();
				}, 1);
				_control.InsertMethod("Beams Slash End", delegate
				{
					GameManager.instance.StartCoroutine(ActivateBeams());
				}, 1);
				_control.InsertMethod("Cheeky Collider 1", delegate
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Beams"]);
					gameObject.transform.localScale = new Vector3((gameObject.transform.localScale.x * (float)UnityEngine.Random.Range(0, 2) == 0f) ? (-1f) : 1f, (gameObject.transform.localScale.y * (float)UnityEngine.Random.Range(0, 2) == 0f) ? (-1f) : 1f, gameObject.transform.localScale.z);
					ElegyBeam elegyBeam = gameObject.AddComponent<ElegyBeam>();
					elegyBeam.offset = Vector2.zero;
					GameManager.instance.StartCoroutine(ActivateSingleBeam(elegyBeam));
				}, 1);
			}

			private IEnumerator ActivateBeams()
			{
				while (_elegyBeams.Count > 0)
				{
					ElegyBeam elegyBeam = _elegyBeams[0];
					_elegyBeams.Remove(elegyBeam);
					if (elegyBeam != null)
					{
						elegyBeam.activate = true;
						PlayAudio("Beams Clip", 0.85f, 1.15f, 1f, 0.1f);
					}
					yield return new WaitForSeconds(0.05f);
				}
			}

			private IEnumerator ActivateSingleBeam(ElegyBeam elegy)
			{
				yield return new WaitForSeconds(0.5f);
				if (!(elegy == null))
				{
					elegy.activate = true;
					PlayAudio("Beams Clip", 0.85f, 1.15f, 1f, 0.1f);
				}
			}

			private void ModifySuper()
			{
				string[] array = new string[3] { "Ground Stab 1", "Ground Air 1", "Air 1" };
				foreach (string stateName in array)
				{
					_control.InsertMethod(stateName, delegate
					{
						if (HeroController.instance.transform.position.x > RightX - 6.5f)
						{
							base.transform.position = new Vector3(RightX - 4.5f, GroundY);
							base.transform.localScale = new Vector3(1f, 1f);
						}
						if (HeroController.instance.transform.position.x < LeftX + 6.5f)
						{
							base.transform.position = new Vector3(LeftX + 4.5f, GroundY);
							base.transform.localScale = new Vector3(-1f, 1f);
						}
					}, 2);
				}
			}

			private void ModifyAudio()
			{
				_control.InsertMethod("Intro Land", delegate
				{
					this.PlayAudio(FiveKnights.Clips["DryyaVoiceBow"]);
				}, 0);
				_control.InsertMethod("Cheeky Collider 1", delegate
				{
					PlayVoice();
				}, 0);
				_control.InsertMethod("Counter Collider 1", delegate
				{
					PlayVoice();
				}, 0);
				_control.InsertMethod("Dive", delegate
				{
					PlayVoice();
				}, 0);
				_control.InsertMethod("Slash 1 Collider 1", delegate
				{
					PlayVoice("Alt");
				}, 0);
				_control.InsertMethod("Stab", delegate
				{
					PlayVoice();
				}, 0);
				_control.InsertMethod("Beams Slash 1", delegate
				{
					PlayVoice("Beams");
				}, 0);
				_control.InsertMethod("Super Start 3", delegate
				{
					PlayVoice();
				}, 0);
				_control.InsertMethod("Ground Stab 4", delegate
				{
					PlayVoice();
				}, 0);
				_control.InsertMethod("Ground Air 4", delegate
				{
					PlayVoice();
				}, 0);
				_control.InsertMethod("Air 1", delegate
				{
					PlayVoice();
				}, 0);
				_control.InsertMethod("Counter Stance", delegate
				{
					PlayAudio("Counter");
				}, 0);
				_control.InsertMethod("Countered", delegate
				{
					PlayAudio("Counter");
				}, 0);
				_control.InsertMethod("Stab", delegate
				{
					PlayAudio("Dash");
				}, 0);
				_control.InsertMethod("Ground Stab 4", delegate
				{
					PlayAudio("Dash Light", 0.85f, 1.15f);
				}, 0);
				_control.InsertMethod("Ground Air 11", delegate
				{
					PlayAudio("Dash Light", 0.85f, 1.15f);
				}, 0);
				_control.InsertMethod("Super 18", delegate
				{
					PlayAudio("Dash Light", 0.85f, 1.15f);
				}, 0);
				_control.InsertMethod("Air 1", delegate
				{
					PlayAudio("Dash Light", 0.85f, 1.15f);
				}, 0);
				_control.InsertMethod("Ground Air 4", delegate
				{
					PlayAudio("Dash Light", 0.85f, 1.15f);
				}, 0);
				_control.InsertMethod("Dive", delegate
				{
					PlayAudio("Dive");
				}, 0);
				_control.InsertMethod("Dive Land Heavy", delegate
				{
					PlayAudio("Dive Land Hard");
				}, 0);
				_control.InsertMethod("Dive Land Light", delegate
				{
					PlayAudio("Dive Land Soft");
				}, 0);
				_control.InsertMethod("Dive Jump", delegate
				{
					PlayAudio("Jump");
				}, 0);
				_control.InsertMethod("Ground Stab 7", delegate
				{
					PlayAudio("Jump", 0.85f, 1.15f);
				}, 0);
				_control.InsertMethod("Ground Air 7", delegate
				{
					PlayAudio("Jump", 0.85f, 1.15f);
				}, 0);
				_control.InsertMethod("Evade Recover", delegate
				{
					PlayAudio("Land");
				}, 0);
				_control.InsertMethod("Super 15", delegate
				{
					PlayAudio("Land", 0.85f, 1.15f);
				}, 0);
				_control.InsertMethod("Counter Collider 1", delegate
				{
					PlayAudio("Slash 1 Clip", 0.85f, 1.15f);
				}, 0);
				_control.InsertMethod("Beams Slash 1", delegate
				{
					PlayAudio("Slash 1 Clip", 0.85f, 1.15f);
				}, 0);
				_control.InsertMethod("Beams Slash 2", delegate
				{
					PlayAudio("Slash 1 Clip", 0.85f, 1.15f);
				}, 0);
				_control.InsertMethod("Beams Slash 3", delegate
				{
					PlayAudio("Slash 1 Clip", 0.85f, 1.15f);
				}, 0);
				_control.InsertMethod("Beams Slash 4", delegate
				{
					PlayAudio("Slash 1 Clip", 0.85f, 1.15f);
				}, 0);
				_control.InsertMethod("Beams Slash 5", delegate
				{
					PlayAudio("Slash 1 Clip", 0.85f, 1.15f);
				}, 0);
				_control.InsertMethod("Slash 1 Collider 1", delegate
				{
					PlayAudio("Slash 1 Clip", 0.85f, 1.15f);
				}, 0);
				_control.InsertMethod("Slash 2 Collider 1", delegate
				{
					PlayAudio("Slash 1 Clip", 0.85f, 1.15f);
				}, 0);
				_control.InsertMethod("Slash 3 Collider 1", delegate
				{
					PlayAudio("Slash 1 Clip", 0.85f, 1.15f);
				}, 0);
				_control.InsertMethod("Cheeky Collider 1", delegate
				{
					PlayAudio("Slash 1 Clip", 0.85f, 1.15f);
				}, 0);
			}

			private void PlayVoice(string suffix = "")
			{
				string text = "DryyaVoice" + suffix;
				text = ((suffix == "Alt") ? (text + UnityEngine.Random.Range(1, 7)) : ((!(suffix == "Beams")) ? (text + UnityEngine.Random.Range(1, 8)) : (text + UnityEngine.Random.Range(1, 4))));
				this.PlayAudio(FiveKnights.Clips[text]);
			}

			private void PlayAudio(string clip, float minPitch = 1f, float maxPitch = 1f, float volume = 1f, float delay = 0f)
			{
				AudioClip clip2 = _control.Fsm.GetFsmObject(clip).Value as AudioClip;
				this.PlayAudio(clip2, volume, maxPitch - minPitch);
			}

			private void OnDestroy()
			{
				_hm.OnDeath -= DeathHandler;
				On.EnemyDreamnailReaction.RecieveDreamImpact -= OnReceiveDreamImpact;
				On.HealthManager.TakeDamage -= OnTakeDamage;
			}

			private void Log(object o)
			{
				if (FiveKnights.isDebug)
				{
					Modding.Logger.Log("[Dryya] " + o);
				}
			}
		}
		public class ElegyBeam : MonoBehaviour
		{
			public GameObject parent;

			public Vector2 offset;

			public bool activate;

			private Animator _anim;

			private float PosY = (OWArenaFinder.IsInOverWorld ? 100.5f : 10f);

			private void Awake()
			{
				base.gameObject.layer = 22;
				DamageHero damageHero = base.gameObject.AddComponent<DamageHero>();
				damageHero.damageDealt = 1;
				damageHero.hazardType = 1;
				damageHero.shadowDashHazard = false;
				_anim = GetComponent<Animator>();
			}

			private void OnEnable()
			{
				GameManager.instance.StartCoroutine(Beam());
			}

			private IEnumerator Beam()
			{
				Vector2 vector = new Vector2(HeroController.instance.transform.position.x, PosY);
				Quaternion rotation = Quaternion.Euler(0f, 0f, Mathf.Sign(base.transform.localScale.x) * (float)UnityEngine.Random.Range(15, 105));
				base.gameObject.transform.SetPositionAndRotation(vector + offset, rotation);
				_anim.Play("Beams");
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 3);
				_anim.enabled = false;
				yield return new WaitUntil(() => activate);
				yield return new WaitForSeconds(0.1f);
				_anim.enabled = true;
				yield return new WaitWhile(() => _anim.GetCurrentFrame() < 9);
				_anim.enabled = false;
				yield return new WaitForSeconds(0.17f);
				_anim.enabled = true;
				UnityEngine.Object.Destroy(GetComponent<PolygonCollider2D>());
				UnityEngine.Object.Destroy(GetComponent<DamageHero>());
				yield return new WaitWhile(() => _anim.IsPlaying("Beams"));
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
	namespace FiveKnights.BossManagement
	{
		public class GGBossManager : MonoBehaviour
		{
			private HealthManager _hm;

			private PlayMakerFSM _fsm;

			public GameObject dd;

			private tk2dSpriteAnimator _tk;

			public static bool alone;

			private bool HIT_FLAG;

			private bool doneCCHitless;

			public static GGBossManager Instance;

			public Dictionary<string, AnimationClip> clips;

			public List<Animator> flowersAnim;

			public List<Animator> flowersGlow;

			private void StartFlowers()
			{
				GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["AllFlowers"]);
				flowersAnim = new List<Animator>();
				flowersGlow = new List<Animator>();
				obj.SetActive(value: true);
				foreach (Transform item in obj.transform)
				{
					GameObject gameObject = item.Find("Flower").Find("Glow").gameObject;
					gameObject.SetActive(value: false);
					flowersGlow.Add(gameObject.GetComponent<Animator>());
					foreach (Transform item2 in item)
					{
						Animator component = item2.GetComponent<Animator>();
						flowersAnim.Add(component);
						component.Play("Grow", -1, 0f);
						if (!(component.name == "Flower") && !(component.GetComponent<SpriteRenderer>().color == Color.black))
						{
							GameObject obj3 = UnityEngine.Object.Instantiate(gameObject, component.transform, worldPositionStays: false);
							obj3.name = "Glow";
							obj3.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, -0.0001f);
							Animator component2 = obj3.GetComponent<Animator>();
							flowersGlow.Add(component2);
							obj3.SetActive(value: false);
						}
					}
				}
			}

			public void PlayFlowers(int step)
			{
				int[][] types = new int[3][]
				{
					new int[3] { 2, 5, 9 },
					new int[3] { 1, 3, 6 },
					new int[3] { 3, 5, 9 }
				};
				foreach (Animator item in flowersAnim)
				{
					string text = item.transform.parent.name;
					int type = ((!(text == "FlowersA")) ? ((text == "FlowersB") ? 1 : 2) : 0);
					StartCoroutine(DoFlower(item, type));
				}
				IEnumerator DoFlower(Animator anim, int num)
				{
					anim.enabled = true;
					yield return new WaitForEndOfFrame();
					yield return anim.WaitToFrame(types[num][step]);
					anim.enabled = false;
				}
			}

			private IEnumerator Start()
			{
				if (CustomWP.boss != CustomWP.Boss.All && CustomWP.boss != CustomWP.Boss.Ogrim)
				{
					BossSceneController.Instance.BossLevel = CustomWP.lev;
				}
				Instance = this;
				ReflectionHelper.SetProperty(GameManager.instance, "sm", UnityEngine.Object.FindObjectOfType<SceneManager>());
				dd = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["WhiteDef"]);
				dd.SetActive(value: false);
				CustomWP.Boss boss = CustomWP.boss;
				if ((boss == CustomWP.Boss.Ogrim || boss == CustomWP.Boss.All) ? true : false)
				{
					dd = GameObject.Find("White Defender");
					PlayerData.instance.SetBool("atBench", value: false);
					if (CustomWP.boss == CustomWP.Boss.All)
					{
						GameObject.Find("Battle Gate (1)").transform.SetPositionX(59.69f);
					}
				}
				_hm = dd.GetComponent<HealthManager>();
				_fsm = dd.LocateMyFSM("Dung Defender");
				_tk = dd.GetComponent<tk2dSpriteAnimator>();
				FiveKnights.preloadedGO["WD"] = dd;
				alone = true;
				Unload();
				On.HealthManager.TakeDamage += HealthManagerTakeDamage;
				On.HealthManager.ApplyExtraDamage += HealthManagerApplyExtraDamage;
				On.HealthManager.Die += HealthManagerDie;
				string dreamReturnScene = PlayerData.instance.dreamReturnScene;
				PlayerData.instance.dreamReturnScene = ((dreamReturnScene == "Waterways_13") ? dreamReturnScene : "White_Palace_09");
				dreamReturnScene = PlayerData.instance.dreamReturnScene;
				PlayerData.instance.dreamReturnScene = ((CustomWP.boss == CustomWP.Boss.All) ? "Dream_04_White_Defender" : dreamReturnScene);
				Log("Curr Boss " + CustomWP.boss);
				if (CustomWP.boss == CustomWP.Boss.Isma)
				{
					FiveKnights.Clips["LoneIsmaLoop"] = ABManager.AssetBundles[ABManager.Bundle.Sound].LoadAsset<AudioClip>("LoneIsmaLoop");
					FiveKnights.Clips["LoneIsmaIntro"] = ABManager.AssetBundles[ABManager.Bundle.Sound].LoadAsset<AudioClip>("LoneIsmaIntro");
					BossLoader.LoadIsmaBundle();
					IsmaController ic = BossLoader.CreateIsma(onlyIsma: true);
					yield return new WaitWhile(() => ic != null);
					if (CustomWP.wonLastFight)
					{
						int num = CustomWP.lev + 1;
						object obj = FiveKnights.Instance.SaveSettings.CompletionIsma;
						ReflectionHelper.GetFieldInfo(typeof(BossStatue.Completion), $"completedTier{num}").SetValue(obj, true);
						FiveKnights.Instance.SaveSettings.CompletionIsma = (BossStatue.Completion)obj;
						BossSceneController bsc = BossSceneController.Instance;
						UnityEngine.Object.Instantiate(bsc.transitionPrefab).LocateMyFSM("Transitions").SetState("Out Statue");
						yield return new WaitForSeconds(1f);
						bsc.gameObject.LocateMyFSM("Dream Return").SendEvent("DREAM RETURN");
					}
					UnityEngine.Object.Destroy(this);
				}
				else if (CustomWP.boss == CustomWP.Boss.Ogrim)
				{
					BossLoader.LoadIsmaBundle();
					PlayMusic(FiveKnights.Clips["OgrimMusic"], 1f);
					yield return OgrimIsmaFight();
					if (CustomWP.wonLastFight)
					{
						int num2 = CustomWP.lev + 1;
						object obj2 = FiveKnights.Instance.SaveSettings.CompletionIsma2;
						ReflectionHelper.GetFieldInfo(typeof(BossStatue.Completion), $"completedTier{num2}").SetValue(obj2, true);
						FiveKnights.Instance.SaveSettings.CompletionIsma2 = (BossStatue.Completion)obj2;
						BossSceneController bsc = BossSceneController.Instance;
						UnityEngine.Object.Instantiate(bsc.transitionPrefab).LocateMyFSM("Transitions").SetState("Out Statue");
						yield return new WaitForSeconds(1f);
						bsc.gameObject.LocateMyFSM("Dream Return").SendEvent("DREAM RETURN");
					}
					UnityEngine.Object.Destroy(this);
				}
				else if (CustomWP.boss == CustomWP.Boss.Dryya)
				{
					BossLoader.LoadDryyaBundle();
					DryyaSetup dc = BossLoader.CreateDryya();
					yield return new WaitWhile(() => dc != null);
					if (CustomWP.wonLastFight)
					{
						int num3 = CustomWP.lev + 1;
						object obj3 = FiveKnights.Instance.SaveSettings.CompletionDryya;
						ReflectionHelper.GetFieldInfo(typeof(BossStatue.Completion), $"completedTier{num3}").SetValue(obj3, true);
						FiveKnights.Instance.SaveSettings.CompletionDryya = (BossStatue.Completion)obj3;
						yield return new WaitForSeconds(5f);
						BossSceneController bsc = BossSceneController.Instance;
						UnityEngine.Object.Instantiate(bsc.transitionPrefab).LocateMyFSM("Transitions").SetState("Out Statue");
						yield return new WaitForSeconds(1f);
						bsc.gameObject.LocateMyFSM("Dream Return").SendEvent("DREAM RETURN");
						yield return null;
					}
					UnityEngine.Object.Destroy(this);
				}
				else if (CustomWP.boss == CustomWP.Boss.Hegemol)
				{
					BossLoader.LoadHegemolBundle();
					HegemolController hegemolCtrl = BossLoader.CreateHegemol();
					GameCameras.instance.cameraShakeFSM.FsmVariables.FindFsmBool("RumblingMed").Value = false;
					yield return new WaitWhile(() => hegemolCtrl != null);
					if (CustomWP.wonLastFight)
					{
						int num4 = CustomWP.lev + 1;
						object obj4 = FiveKnights.Instance.SaveSettings.CompletionHegemol;
						ReflectionHelper.GetFieldInfo(typeof(BossStatue.Completion), $"completedTier{num4}").SetValue(obj4, true);
						FiveKnights.Instance.SaveSettings.CompletionHegemol = (BossStatue.Completion)obj4;
						BossSceneController bsc = BossSceneController.Instance;
						UnityEngine.Object.Instantiate(bsc.transitionPrefab).LocateMyFSM("Transitions").SetState("Out Statue");
						yield return new WaitForSeconds(1f);
						bsc.gameObject.LocateMyFSM("Dream Return").SendEvent("DREAM RETURN");
					}
					UnityEngine.Object.Destroy(this);
				}
				else if (CustomWP.boss == CustomWP.Boss.Ze || CustomWP.boss == CustomWP.Boss.Mystic)
				{
					ZemerController.WaitForTChild = false;
					BossLoader.LoadZemerBundle();
					GameCameras.instance.cameraShakeFSM.FsmVariables.FindFsmBool("RumblingMed").Value = false;
					yield return null;
					ZemerController zc = BossLoader.CreateZemer();
					GameObject zem = zc.gameObject;
					yield return new WaitWhile(() => zc != null);
					if (zem == null)
					{
						Log("Zem did not exist so destroying");
						UnityEngine.Object.Destroy(this);
						yield break;
					}
					ZemerControllerP2 zc2 = zem.GetComponent<ZemerControllerP2>();
					yield return new WaitWhile(() => zc2 != null);
					if (CustomWP.wonLastFight)
					{
						int num5 = CustomWP.lev + 1;
						if (CustomWP.boss == CustomWP.Boss.Ze)
						{
							object obj5 = FiveKnights.Instance.SaveSettings.CompletionZemer;
							ReflectionHelper.GetFieldInfo(typeof(BossStatue.Completion), $"completedTier{num5}").SetValue(obj5, true);
							FiveKnights.Instance.SaveSettings.CompletionZemer = (BossStatue.Completion)obj5;
						}
						else
						{
							object obj6 = FiveKnights.Instance.SaveSettings.CompletionZemer2;
							ReflectionHelper.GetFieldInfo(typeof(BossStatue.Completion), $"completedTier{num5}").SetValue(obj6, true);
							FiveKnights.Instance.SaveSettings.CompletionZemer2 = (BossStatue.Completion)obj6;
						}
						BossSceneController bsc = BossSceneController.Instance;
						UnityEngine.Object.Instantiate(bsc.transitionPrefab).LocateMyFSM("Transitions").SetState("Out Statue");
						yield return new WaitForSeconds(1f);
						bsc.gameObject.LocateMyFSM("Dream Return").SendEvent("DREAM RETURN");
					}
					UnityEngine.Object.Destroy(this);
				}
				else if (CustomWP.boss == CustomWP.Boss.All)
				{
					yield return null;
					BossLoader.LoadIsmaBundle();
					BossLoader.LoadDryyaBundle();
					BossLoader.LoadHegemolBundle();
					BossLoader.LoadZemerBundle();
					GameObject obj7 = new GameObject("BSC Dummy");
					obj7.SetActive(value: false);
					BossSceneController bossSceneController = (BossSceneController.Instance = obj7.AddComponent<BossSceneController>());
					bossSceneController.bosses = new HealthManager[0];
					ReflectionHelper.SetProperty(bossSceneController, "BossHealthLookup", new Dictionary<HealthManager, BossSceneController.BossHealthDetails>());
					doneCCHitless = true;
					ModHooks.AfterTakeDamageHook += CheckCCHitless;
					yield return null;
					_fsm.RemoveAction("Music", 0);
					yield return OgrimIsmaFight();
					yield return new WaitForSeconds(1.5f);
					GameObject zem = GameObject.Find("Silhouette Dryya");
					SpriteRenderer sr = zem.GetComponent<SpriteRenderer>();
					zem.transform.localScale *= 1.2f;
					DryyaSetup dc2 = BossLoader.CreateDryya();
					sr.sprite = ArenaFinder.Sprites["Dryya_Silhouette_1"];
					yield return new WaitForSeconds(0.1f);
					sr.sprite = ArenaFinder.Sprites["Dryya_Silhouette_2"];
					yield return new WaitForSeconds(0.1f);
					sr.sprite = ArenaFinder.Sprites["Dryya_Silhouette_3"];
					yield return new WaitForSeconds(0.1f);
					UnityEngine.Object.Destroy(zem);
					yield return new WaitForSeconds(0.5f);
					yield return new WaitWhile(() => dc2 != null);
					yield return new WaitForSeconds(3.5f);
					GameObject hegSil = GameObject.Find("Silhouette Hegemol");
					SpriteRenderer sr2 = hegSil.GetComponent<SpriteRenderer>();
					hegSil.transform.localScale *= 1.55f;
					hegSil.transform.position += 0.1f * Vector3.left;
					for (int i = 1; i <= 5; i++)
					{
						sr2.sprite = ArenaFinder.Sprites["hegemol_silhouette_" + i];
						yield return new WaitForSeconds(0.1f);
					}
					sr2.sprite = ArenaFinder.Sprites["hegemol_silhouette_6"];
					hegSil.AddComponent<Rigidbody2D>().gravityScale = 0f;
					hegSil.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 50f);
					yield return new WaitForSeconds(0.1f);
					sr2.sprite = ArenaFinder.Sprites["hegemol_silhouette_7"];
					yield return new WaitForSeconds(0.5f);
					UnityEngine.Object.Destroy(hegSil);
					HegemolController hegemolCtrl2 = BossLoader.CreateHegemol();
					yield return new WaitWhile(() => hegemolCtrl2 != null);
					yield return new WaitForSeconds(1.5f);
					ZemerController zc3 = BossLoader.CreateZemer();
					GameObject zem2 = zc3.gameObject;
					yield return new WaitForSeconds(0.5f);
					PlayFlowers(1);
					yield return new WaitWhile(() => zc3 != null);
					ZemerControllerP2 zc4 = zem2.GetComponent<ZemerControllerP2>();
					FiveKnights.Instance.SaveSettings.CompletionZemer2.isUnlocked = true;
					yield return new WaitWhile(() => zc4 != null);
					Log("Won!");
					GameManager.instance.AwardAchievement("PALE_COURT_PANTH_ACH");
					FiveKnights.Instance.SaveSettings.ChampionsCallClears++;
					yield return new WaitForSeconds(0.5f);
					CCDreamExit();
				}
			}

			private int CheckCCHitless(int hazardType, int damageAmount)
			{
				Log("Got hit in CC");
				doneCCHitless = false;
				return damageAmount;
			}

			private IEnumerator OgrimIsmaFight()
			{
				On.HeroController.TakeDamage += HeroControllerTakeDamage;
				dd = GameObject.Find("White Defender");
				dd.GetComponent<DamageHero>().damageDealt = 1;
				USceneUtil.Find(dd, "Throw Swipe").gameObject.GetComponent<DamageHero>().damageDealt = 1;
				EnemyDreamnailReaction dreamNailReaction = dd.GetComponent<EnemyDreamnailReaction>();
				Mirror.SetField(dreamNailReaction, "convoAmount", 2);
				dreamNailReaction.SetConvoTitle("OGRIM_GG_DREAM");
				_hm = dd.GetComponent<HealthManager>();
				_fsm = dd.LocateMyFSM("Dung Defender");
				_tk = dd.GetComponent<tk2dSpriteAnimator>();
				alone = false;
				_hm.hp = 351;
				_fsm.GetAction<HutongGames.PlayMaker.Actions.Wait>("Rage Roar", 9).time = 1.5f;
				_fsm.FsmVariables.FindFsmBool("Raged").Value = true;
				EnemyHPBarImport.RefreshHPBar(dd);
				yield return new WaitForSeconds(1f);
				GameCameras.instance.cameraFadeFSM.Fsm.SetState("FadeIn");
				PlayMakerFSM burrow = GameObject.Find("Burrow Effect").LocateMyFSM("Burrow Effect");
				yield return new WaitWhile(() => _hm.hp > 1);
				HIT_FLAG = false;
				yield return new WaitWhile(() => !HIT_FLAG);
				FiveKnights.Instance.SaveSettings.CompletionIsma2.isUnlocked = true;
				PlayMusic(null, 1f);
				if (dd.transform.position.y < 9f)
				{
					dd.transform.position = new Vector3(dd.transform.position.x, 9f, dd.transform.position.z);
				}
				PlayerData.instance.isInvincible = true;
				dd.layer = 26;
				_fsm.SetState("Stun Set");
				Mirror.SetField(dreamNailReaction, "convoAmount", 5);
				burrow.enabled = true;
				burrow.SendEvent("BURROW END");
				PlayMakerFSM[] componentsInChildren = USceneUtil.Find(dd, "Slam Pillars").GetComponentsInChildren<PlayMakerFSM>();
				foreach (PlayMakerFSM playMakerFSM in componentsInChildren)
				{
					if (playMakerFSM.ActiveStateName == "Up" || playMakerFSM.ActiveStateName == "Hit")
					{
						playMakerFSM.gameObject.GetComponent<MeshRenderer>().enabled = false;
						playMakerFSM.SetState("Dormant");
						playMakerFSM.FsmVariables.FindFsmGameObject("Chunks").Value.GetComponent<ParticleSystem>().Play();
					}
					else
					{
						playMakerFSM.enabled = false;
					}
				}
				_fsm.GetFsmGameObjectVariable("zz_Pt Dive").Value.GetComponent<ParticleSystem>().Stop();
				yield return new WaitWhile(() => _fsm.ActiveStateName != "Stun Land");
				_fsm.enabled = false;
				burrow.enabled = false;
				yield return new WaitForSeconds(1f);
				IsmaController ic = BossLoader.CreateIsma(onlyIsma: false);
				if (CustomWP.boss == CustomWP.Boss.All)
				{
					Log("made flowers");
					StartFlowers();
					PlayFlowers(0);
				}
				yield return new WaitWhile(() => !ic.introDone);
				_fsm.enabled = true;
				_fsm.SetState("Stun Recover");
				dd.layer = 11;
				yield return null;
				_fsm.InsertMethod("Idle", 1, delegate
				{
					_fsm.SetState("Rage Roar");
				});
				yield return new WaitWhile(() => _fsm.ActiveStateName == "Stun Recover");
				yield return new WaitWhile(() => _fsm.ActiveStateName == "Rage Roar");
				_fsm.RemoveAction("Idle", 1);
				PlayerData.instance.isInvincible = false;
				burrow.enabled = true;
				componentsInChildren = USceneUtil.Find(dd, "Slam Pillars").GetComponentsInChildren<PlayMakerFSM>();
				for (int num = 0; num < componentsInChildren.Length; num++)
				{
					componentsInChildren[num].enabled = true;
				}
				yield return new WaitWhile(() => !_fsm.ActiveStateName.Contains("Tunneling"));
				yield return new WaitWhile(() => ic != null);
				On.HeroController.TakeDamage -= HeroControllerTakeDamage;
			}

			private void CCDreamExit()
			{
				PlayMakerFSM[] componentsInChildren = HeroController.instance.gameObject.GetComponentsInChildren<PlayMakerFSM>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].SendEvent("FSM CANCEL");
				}
				HeroController.instance.AffectedByGravity(gravityApplies: true);
				HeroController.instance.StartAnimationControl();
				HeroController.instance.RelinquishControl();
				PlayerData.instance.disablePause = true;
				GameObject gameObject = GameObject.Find("Dream Exit Particle Field");
				if (gameObject != null)
				{
					gameObject.GetComponent<ParticleSystem>().Play();
				}
				GameObject obj = UnityEngine.Object.Instantiate(Mirror.GetField<EnemyDeathEffects, GameObject>(dd.GetComponent<EnemyDeathEffectsUninfected>(), "corpsePrefab"));
				obj.GetComponent<Renderer>().enabled = false;
				PlayMakerFSM playMakerFSM = obj.LocateMyFSM("Control");
				TextMeshPro component = playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.SetTextMeshProAlignment>("New Scene", 1).gameObject.GameObject.Value.GetComponent<TextMeshPro>();
				component.color = Color.black;
				component.alignment = TextAlignmentOptions.Center;
				playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.Wait>("Fade Out", 4).time.Value += 2f;
				GameObject.Find("Blanker White").LocateMyFSM("Blanker Control").FsmVariables.FindFsmFloat("Fade Time").Value = 0f;
				SFCore.Utils.FsmUtil.RemoveAction(playMakerFSM.GetState("Fade Out"), 0);
				SFCore.Utils.FsmUtil.ChangeTransition(playMakerFSM, "Take Control", "FINISHED", "Outro Msg 1a");
				SFCore.Utils.FsmUtil.ChangeTransition(playMakerFSM, "Outro Msg 1b", "CONVO_FINISH", "New Scene");
				if (doneCCHitless)
				{
					playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.CallMethodProper>("Outro Msg 1a", 0).parameters[0].stringValue = "RCC_OUTRO_1a";
					playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.CallMethodProper>("Outro Msg 1b", 0).parameters[0].stringValue = "RCC_OUTRO_1b";
				}
				else
				{
					playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.CallMethodProper>("Outro Msg 1a", 0).parameters[0].stringValue = "CC_OUTRO_1a";
					playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.CallMethodProper>("Outro Msg 1b", 0).parameters[0].stringValue = "CC_OUTRO_1b";
				}
				playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.CallMethodProper>("Outro Msg 1a", 0).parameters[1].stringValue = "Speech";
				playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.CallMethodProper>("Outro Msg 1b", 0).parameters[1].stringValue = "Speech";
				RewardRoom.doneCCHitless = doneCCHitless;
				if (doneCCHitless)
				{
					FiveKnights.Instance.SaveSettings.ChampionsCallHitless = true;
				}
				playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.BeginSceneTransition>("New Scene", 6).sceneName = "Pale_Court_Credits";
				playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.BeginSceneTransition>("New Scene", 6).entryGateName = "";
				HeroController.instance.MaxHealth();
				HeroController.instance.EnterWithoutInput(flag: true);
				playMakerFSM.SetState("Fade Out");
				UnityEngine.Object.Destroy(this);
			}

			private void HeroControllerTakeDamage(On.HeroController.orig_TakeDamage orig, HeroController self, GameObject go, CollisionSide damageSide, int damageAmount, int hazardType)
			{
				orig(self, go, damageSide, (damageAmount > 1) ? 1 : damageAmount, hazardType);
			}

			private void HealthManagerTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
			{
				if (self.name.Contains("White Defender"))
				{
					HIT_FLAG = true;
				}
				orig(self, hitInstance);
			}

			private void HealthManagerApplyExtraDamage(On.HealthManager.orig_ApplyExtraDamage orig, HealthManager self, int damageAmount)
			{
				if (self.name.Contains("White Defender"))
				{
					HIT_FLAG = true;
				}
				orig(self, damageAmount);
			}

			private void HealthManagerDie(On.HealthManager.orig_Die orig, HealthManager self, float? attackDirection, AttackTypes attackType, bool ignoreEvasion)
			{
				if (self.gameObject.name.Contains("White Defender"))
				{
					self.hp = 1;
				}
				else
				{
					orig(self, attackDirection, attackType, ignoreEvasion);
				}
			}

			private void OnCollisionEnter2D(Collision2D c)
			{
				if (_tk.IsPlaying("Roll") && c.gameObject.layer == 8 && c.gameObject.name.Contains("Front"))
				{
					_fsm.SetState("RJ Wall");
				}
			}

			public void PlayMusic(AudioClip clip, float vol = 0f)
			{
				MusicCue musicCue = ScriptableObject.CreateInstance<MusicCue>();
				MusicCue.MusicChannelInfo musicChannelInfo = new MusicCue.MusicChannelInfo();
				Mirror.SetField(musicChannelInfo, "clip", clip);
				MusicCue.MusicChannelInfo[] value = new MusicCue.MusicChannelInfo[6] { musicChannelInfo, null, null, null, null, null };
				Mirror.SetField(musicCue, "channelInfos", value);
				Resources.FindObjectsOfTypeAll<AudioMixer>().First((AudioMixer x) => x.name == "Music").FindSnapshot("Main Only")
					.TransitionTo(0f);
				GameManager.instance.AudioManager.ApplyMusicCue(musicCue, 0f, 0f, applySnapshot: false);
			}

			private void Log(object o)
			{
				Modding.Logger.Log("[GGBossManager] " + o);
			}

			private void OnDestroy()
			{
				Unload();
			}

			private void Unload()
			{
				On.HealthManager.TakeDamage -= HealthManagerTakeDamage;
				On.HealthManager.ApplyExtraDamage -= HealthManagerApplyExtraDamage;
				On.HealthManager.Die -= HealthManagerDie;
				On.HeroController.TakeDamage -= HeroControllerTakeDamage;
				ModHooks.AfterTakeDamageHook -= CheckCCHitless;
			}
		}
		public class OWArenaFinder : MonoBehaviour
		{
			private static OWArenaFinder Instance;

			private const string DryyaScene = "dryya overworld";

			private const string ZemerScene = "zemer overworld arena";

			private const string HegemolScene = "hegemol overworld arena";

			private const string IsmaScene = "isma overworld";

			public static readonly string PrevDryScene = "Fungus3_48";

			public static readonly string PrevZemScene = "Room_Mansion";

			public static readonly string PrevHegScene = "Fungus2_21";

			public static readonly string PrevIsmScene = "Waterways_13";

			private const string SheoScene = "Room_nailmaster_02";

			private static Dictionary<string, Shader> ParticleMatToShader = new Dictionary<string, Shader>();

			public Dictionary<string, AnimationClip> clips;

			private string _currScene;

			private string _prevScene;

			public static bool IsInOverWorld
			{
				get
				{
					bool flag = Instance != null;
					if (flag)
					{
						bool flag2;
						switch (Instance._currScene)
						{
						case "dryya overworld":
						case "isma overworld":
						case "zemer overworld arena":
						case "hegemol overworld arena":
							flag2 = true;
							break;
						default:
							flag2 = false;
							break;
						}
						flag = flag2;
					}
					return flag;
				}
			}

			private void Start()
			{
				Instance = this;
				UnityEngine.SceneManagement.SceneManager.activeSceneChanged += USceneManagerOnActiveSceneChanged;
				On.GameManager.EnterHero += GameManagerOnEnterHero;
				On.GameManager.RefreshTilemapInfo += GameManagerOnRefreshTilemapInfo;
				On.GameManager.GetCurrentMapZone += GameManagerOnGetCurrentMapZone;
			}

			private void USceneManagerOnActiveSceneChanged(Scene fromScene, Scene toScene)
			{
				_prevScene = fromScene.name;
				if (_prevScene == "dryya overworld" || _prevScene == "zemer overworld arena" || _prevScene == "hegemol overworld arena" || _prevScene == "isma overworld")
				{
					CustomWP.boss = CustomWP.Boss.None;
				}
				ChangeSceneForArenas(toScene);
			}

			private string GameManagerOnGetCurrentMapZone(On.GameManager.orig_GetCurrentMapZone orig, GameManager self)
			{
				bool flag;
				switch (_currScene)
				{
				case "zemer overworld arena":
				case "dryya overworld":
				case "isma overworld":
				case "hegemol overworld arena":
					flag = true;
					break;
				default:
					flag = false;
					break;
				}
				if (!flag)
				{
					return orig(self);
				}
				return MapZone.DREAM_WORLD.ToString();
			}

			private void GameManagerOnRefreshTilemapInfo(On.GameManager.orig_RefreshTilemapInfo orig, GameManager self, string targetscene)
			{
				orig(self, targetscene);
				if (!(targetscene != "dryya overworld") || !(targetscene != "zemer overworld arena") || !(targetscene != "hegemol overworld arena") || !(targetscene != "isma overworld"))
				{
					self.sceneWidth = 500f;
					self.sceneHeight = 500f;
					self.tilemap.width = 500;
					self.tilemap.height = 500;
					UnityEngine.Object.FindObjectOfType<GameMap>().SetManualTilemap(0f, 0f, 500f, 500f);
				}
			}

			private void GameManagerOnEnterHero(On.GameManager.orig_EnterHero orig, GameManager self, bool additivegatesearch)
			{
				switch (self.sceneName)
				{
				case "Room_nailmaster_02":
					Log("Sheo scene");
					if (PlayerData.instance.nailsmithSheo && PlayerData.instance.sheoConvoNailsmith && PlayerData.instance.nailsmithConvoArt && FiveKnights.Instance.SaveSettings.UnlockedChampionsCall)
					{
						GameManager.instance.gameObject.AddComponent<Artists>();
					}
					break;
				case "dryya overworld":
					CreateGateway("door1", new Vector2(385.36f, 98.4f), Vector2.zero, null, null, right: true, left: false, onlyOut: true, GameManager.SceneLoadVisualizations.Dream);
					break;
				case "isma overworld":
					if (GameObject.Find("door1") != null)
					{
						UnityEngine.Object.Destroy(GameObject.Find("door1"));
					}
					CreateGateway("door1", new Vector2(385.36f, 98.4f), Vector2.zero, null, null, right: true, left: false, onlyOut: true, GameManager.SceneLoadVisualizations.Dream);
					break;
				case "zemer overworld arena":
					if (GameObject.Find("door1") != null)
					{
						UnityEngine.Object.Destroy(GameObject.Find("door1"));
					}
					CreateGateway("door1", new Vector2(165.86f, 105.92f), Vector2.zero, null, null, right: true, left: false, onlyOut: true, GameManager.SceneLoadVisualizations.Dream);
					break;
				case "hegemol overworld arena":
					if (GameObject.Find("door1") != null)
					{
						UnityEngine.Object.Destroy(GameObject.Find("door1"));
					}
					CreateGateway("door1", new Vector2(165.86f, 105.92f), Vector2.zero, null, null, right: true, left: false, onlyOut: true, GameManager.SceneLoadVisualizations.Dream);
					break;
				case "Crossroads_10":
					if (PlayerData.instance.falseKnightDefeated)
					{
						Log("Adding dreamnail reaction to armor");
						GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Dream Dialogue"], new Vector3(36.5f, 3f, 0f), Quaternion.identity);
						obj.transform.localScale = new Vector3(3f, 0.87f, 1f);
						obj.SetActive(value: true);
						PlayMakerFSM fsm = obj.LocateMyFSM("npc_dream_dialogue");
						fsm.GetFsmStringVariable("Convo Name").Value = "FK_ARMOUR_DREAM_PC";
						fsm.GetFsmStringVariable("Sheet Name").Value = "Enemy Dreams";
					}
					break;
				default:
					if (self.sceneName == PrevDryScene && PlayerData.instance.GetBool("whiteDefenderDefeated"))
					{
						CreateGateway("door_dreamReturn", new Vector2(40.5f, 94.4f), Vector2.zero, null, null, right: false, left: false, onlyOut: true, GameManager.SceneLoadVisualizations.Dream);
						if (_prevScene == "dryya overworld" && HeroController.instance.controlReqlinquished)
						{
							ClearWhiteScreen();
						}
					}
					else if (self.sceneName == PrevIsmScene && PlayerData.instance.GetBool("whiteDefenderDefeated"))
					{
						CreateGateway("door_dreamReturn", new Vector2(95.7f, 18.4f), Vector2.zero, null, null, right: false, left: false, onlyOut: true, GameManager.SceneLoadVisualizations.Dream);
						if (_prevScene == "isma overworld" && HeroController.instance.controlReqlinquished)
						{
							ClearWhiteScreen();
						}
					}
					else if (self.sceneName == PrevZemScene && PlayerData.instance.GetBool("whiteDefenderDefeated") && PlayerData.instance.GetBool("xunRewardGiven"))
					{
						CreateGateway("door_dreamReturn", new Vector2(22.1f, 6.4f), Vector2.zero, null, null, right: false, left: false, onlyOut: true, GameManager.SceneLoadVisualizations.Dream);
						if (_prevScene == "zemer overworld arena" && HeroController.instance.controlReqlinquished)
						{
							ClearWhiteScreen();
						}
					}
					else if (self.sceneName == PrevHegScene && PlayerData.instance.GetBool("whiteDefenderDefeated") && (PlayerData.instance.GetBool("openedCityGate") || PlayerData.instance.GetBool("hasCityKey")))
					{
						CreateGateway("door_dreamReturn", new Vector2(114.1f, 12.4f), Vector2.zero, null, null, right: false, left: false, onlyOut: true, GameManager.SceneLoadVisualizations.Dream);
						if (_prevScene == "hegemol overworld arena" && HeroController.instance.controlReqlinquished)
						{
							ClearWhiteScreen();
						}
					}
					break;
				}
				orig(self, additivegatesearch);
			}

			private void ClearWhiteScreen()
			{
				Log("Trying to clear white screen");
				GameObject.Find("Blanker White").LocateMyFSM("Blanker Control").SendEvent("FADE OUT");
				HeroController.instance.EnableRenderer();
				HeroController.instance.AcceptInput();
				HeroController.instance.RegainControl();
			}

			private void ArenaBundleManage()
			{
				Log("Arena bundle manage");
				if (_currScene == PrevDryScene && PlayerData.instance.GetBool("whiteDefenderDefeated"))
				{
					if (_prevScene == "dryya overworld")
					{
						BossReturn(stopMusic: false);
					}
					BossLoader.LoadDryyaBundle();
					foreach (GameObject item in from x in UnityEngine.Object.FindObjectsOfType<GameObject>()
						where x.name == "Dream Dialogue"
						select x)
					{
						UnityEngine.Object.Destroy(item);
					}
					CreateDreamGateway("Dream Enter", "door1", new Vector2(40.9f, 94.4f), new Vector2(3f, 3f), new Vector2(3f, 3f), Vector2.zero, "dryya overworld", PrevDryScene);
				}
				else if (_currScene == PrevZemScene && PlayerData.instance.GetBool("whiteDefenderDefeated") && PlayerData.instance.GetBool("xunRewardGiven"))
				{
					if (_prevScene == "zemer overworld arena")
					{
						BossReturn(stopMusic: true);
					}
					BossLoader.LoadZemerBundle();
					CreateDreamGateway("Dream Enter", "door1", new Vector2(25.1f, 6.4f), new Vector2(3f, 3f), new Vector2(3f, 3f), Vector2.zero, "zemer overworld arena", PrevZemScene);
				}
				else if (_currScene == PrevHegScene && PlayerData.instance.GetBool("whiteDefenderDefeated") && (PlayerData.instance.GetBool("openedCityGate") || PlayerData.instance.GetBool("hasCityKey")))
				{
					if (_prevScene == "hegemol overworld arena")
					{
						BossReturn(stopMusic: false);
					}
					BossLoader.LoadHegemolBundle();
					CreateDreamGateway("Dream Enter", "door1", new Vector2(118.1f, 13.5f), new Vector2(5f, 5f), new Vector2(3f, 3f), Vector2.zero, "hegemol overworld arena", PrevHegScene);
				}
				else
				{
					if (!(_currScene == PrevIsmScene) || !PlayerData.instance.GetBool("whiteDefenderDefeated"))
					{
						return;
					}
					if (_prevScene == "isma overworld")
					{
						BossReturn(stopMusic: false);
					}
					BossLoader.LoadIsmaBundle();
					foreach (GameObject item2 in from x in UnityEngine.Object.FindObjectsOfType<GameObject>()
						where x.name == "Dream Dialogue"
						select x)
					{
						UnityEngine.Object.Destroy(item2);
					}
					CreateDreamGateway("Dream Enter", "door1", new Vector2(97.6f, 19.2f), new Vector2(5f, 5f), new Vector2(3f, 3f), new Vector2(0f, 4f), "isma overworld", PrevIsmScene);
				}
			}

			private void BossReturn(bool stopMusic)
			{
				if (OWBossManager.Instance != null)
				{
					if (stopMusic)
					{
						OWBossManager.PlayMusic(null);
					}
					Log("Destroying OWBossManager");
					UnityEngine.Object.Destroy(OWBossManager.Instance);
				}
				HeroController.instance.gameObject.LocateMyFSM("Dream Return").FsmVariables.FindFsmBool("Dream Returning").Value = true;
				HeroController.instance.RelinquishControl();
				HeroController.instance.MaxHealth();
				PlayerData.instance.UpdateBlueHealth();
				HeroController.instance.ClearMPSendEvents();
			}

			private IEnumerator ShaderFixer()
			{
				yield return new WaitForSeconds(0.5f);
				ParticleSystemRenderer[] array = UnityEngine.Object.FindObjectsOfType<ParticleSystemRenderer>(includeInactive: true);
				foreach (ParticleSystemRenderer particleSystemRenderer in array)
				{
					string text = particleSystemRenderer.material.name;
					string text2 = "Hidden/InternalErrorShader";
					if (particleSystemRenderer.material.shader.name == text2)
					{
						if (particleSystemRenderer.material.name.Contains("Particle_Lift_Dust"))
						{
							particleSystemRenderer.material.shader = Shader.Find("Sprites/Lit");
						}
						else if (!ParticleMatToShader.ContainsKey(text))
						{
							Log("Did not have shader of mat " + text);
						}
						else
						{
							particleSystemRenderer.material.shader = ParticleMatToShader[text];
						}
					}
					else if (!ParticleMatToShader.ContainsKey(particleSystemRenderer.material.name) || !(ParticleMatToShader[particleSystemRenderer.material.name].name != text2))
					{
						ParticleMatToShader.Add(particleSystemRenderer.material.name, particleSystemRenderer.material.shader);
					}
				}
			}

			private void ChangeSceneForArenas(Scene sceneToChange)
			{
				_currScene = sceneToChange.name;
				StartCoroutine(ShaderFixer());
				switch (_currScene)
				{
				case "dryya overworld":
					Log("Trying to enter fight dryya");
					CustomWP.boss = CustomWP.Boss.Dryya;
					PlayerData.instance.dreamReturnScene = PrevDryScene;
					FixBlur();
					AddBattleGate(427.5f, 120f, new Vector3(421.925f, 99.5f));
					DreamEntry();
					AddSuperDashCancel();
					FixPitDeath();
					FixDryyaSpikes();
					AddContributorTablets();
					GameManager.instance.gameObject.AddComponent<OWBossManager>();
					break;
				case "isma overworld":
					Log("Trying to enter fight isma");
					CustomWP.boss = CustomWP.Boss.Isma;
					PlayerData.instance.dreamReturnScene = PrevIsmScene;
					FixBlur();
					AddBattleGate(110f, 99999f, new Vector3(104.5f, 8.5f));
					DreamEntry();
					FixIsmaSprites();
					AddSuperDashCancel();
					FixPitDeath();
					GameManager.instance.gameObject.AddComponent<OWBossManager>();
					break;
				case "zemer overworld arena":
					Log("Trying to enter fight zemer");
					CustomWP.boss = CustomWP.Boss.Ze;
					PlayerData.instance.dreamReturnScene = PrevZemScene;
					FixBlur();
					AddBattleGate(243.9f, 99999f, new Vector2(238.835f, 107f));
					AddSuperDashCancel();
					FixPitDeath();
					DreamEntry();
					GameManager.instance.gameObject.AddComponent<OWBossManager>();
					break;
				case "hegemol overworld arena":
					Log("Trying to enter fight hegemol");
					CustomWP.boss = CustomWP.Boss.Hegemol;
					PlayerData.instance.dreamReturnScene = PrevHegScene;
					FixBlur();
					FixHegemolArena();
					AddSuperDashCancel();
					FixPitDeath();
					AddBattleGate(426.5f, 99999f, new Vector2(420.925f, 156.8f));
					DreamEntry();
					GameManager.instance.gameObject.AddComponent<OWBossManager>();
					break;
				default:
					ArenaBundleManage();
					break;
				}
			}

			private void DreamEntry()
			{
				foreach (GameObject item in from x in UnityEngine.Object.FindObjectsOfType<GameObject>()
					where x.name == "Dream Entry"
					select x)
				{
					HeroController.instance.isHeroInPosition = true;
					GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["DreamEntry"]);
					obj.transform.position = item.transform.position;
					UnityEngine.Object.Destroy(item);
					obj.SetActive(value: true);
					obj.name = "Dream Entry";
					HeroController.instance.FaceRight();
				}
			}

			private void CreateGateway(string gateName, Vector2 pos, Vector2 size, string toScene, string entryGate, bool right, bool left, bool onlyOut, GameManager.SceneLoadVisualizations vis)
			{
				GameObject gameObject = new GameObject(gateName);
				gameObject.transform.SetPosition2D(pos);
				TransitionPoint transitionPoint = gameObject.AddComponent<TransitionPoint>();
				if (!onlyOut)
				{
					BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
					boxCollider2D.size = size;
					boxCollider2D.isTrigger = true;
					transitionPoint.targetScene = toScene;
					transitionPoint.entryPoint = entryGate;
				}
				transitionPoint.alwaysEnterLeft = left;
				transitionPoint.alwaysEnterRight = right;
				GameObject gameObject2 = new GameObject("Hazard Respawn Marker");
				gameObject2.transform.parent = gameObject.transform;
				gameObject2.tag = "RespawnPoint";
				gameObject2.transform.SetPosition2D(pos);
				transitionPoint.respawnMarker = gameObject2.AddComponent<HazardRespawnMarker>();
				transitionPoint.sceneLoadVisualization = vis;
			}

			private void FixBlur()
			{
				GameObject gameObject = null;
				SceneManager[] array = UnityEngine.Object.FindObjectsOfType<SceneManager>();
				foreach (SceneManager obj in array)
				{
					gameObject = obj.borderPrefab;
					UnityEngine.Object.Destroy(obj.gameObject);
				}
				GameObject gameObject2 = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["SMTest"]);
				if (gameObject != null)
				{
					gameObject2.GetComponent<SceneManager>().borderPrefab = gameObject;
				}
				gameObject2.GetComponent<SceneManager>().noLantern = true;
				gameObject2.GetComponent<SceneManager>().darknessLevel = -1;
				gameObject2.SetActive(value: true);
			}

			private void FixDryyaSpikes()
			{
				GameObject[] array = new GameObject[2]
				{
					GameObject.Find("ruind_bridge_roof_02spikes"),
					USceneUtil.Find(GameObject.Find("ruind_bridge_roof_01"), "ruind_bridge_roof_spike")
				};
				foreach (GameObject obj in array)
				{
					obj.layer = 19;
					obj.AddComponent<Pogoable>();
					obj.AddComponent<Tink>();
					DamageHero damageHero = obj.AddComponent<DamageHero>();
					damageHero.damageDealt = 1;
					damageHero.shadowDashHazard = true;
					damageHero.hazardType = 2;
				}
			}

			private void AddContributorTablets()
			{
				foreach (Transform item in GameObject.Find("Contributor Tablets").transform)
				{
					GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["Backer Shrine"], item.transform.position, Quaternion.identity);
					UnityEngine.Object.Destroy(obj.GetComponent<SpriteRenderer>());
					UnityEngine.Object.Destroy(obj.GetComponent<Breakable>());
					UnityEngine.Object.Destroy(obj.GetComponent<PersistentBoolItem>());
					UnityEngine.Object.Destroy(obj.GetComponent<BoxCollider2D>());
					UnityEngine.Object.Destroy(USceneUtil.Find(obj, "Particle_rocks_small (3)"));
					UnityEngine.Object.Destroy(USceneUtil.Find(obj, "Fungus Base Horned"));
					GameObject obj2 = USceneUtil.Find(USceneUtil.Find(obj, "Active"), "Glow Response Object");
					UnityEngine.Object.Destroy(USceneUtil.Find(obj2, "Fade Sprite").GetComponent<SpriteRenderer>());
					obj2.GetComponent<GlowResponse>().FadeSprites = new List<SpriteRenderer> { item.Find("Overlay").GetComponent<SpriteRenderer>() };
					PlayMakerFSM fsm = USceneUtil.Find(USceneUtil.Find(obj, "Active"), "Inspect Region").LocateMyFSM("inspect_region");
					fsm.GetFsmStringVariable("Game Text Convo").Value = item.name.ToUpper();
					fsm.GetFsmStringVariable("Game Text Sheet").Value = "PC Contributor Tablets";
					obj.SetActive(value: true);
				}
				GameObject obj3 = UnityEngine.Object.Instantiate(USceneUtil.Find(USceneUtil.Find(FiveKnights.preloadedGO["Backer Shrine"], "Active"), "Inspect Region"), new Vector3(417.8f, 128.2f, 0f), Quaternion.identity);
				obj3.name = "Custom Inspect Thing";
				USceneUtil.Find(obj3, "Prompt Marker").transform.localPosition = new Vector3(-0.1f, 1.51f, 0.2f);
				PlayMakerFSM fsm2 = obj3.LocateMyFSM("inspect_region");
				fsm2.GetFsmStringVariable("Game Text Convo").Value = "INTRO";
				fsm2.GetFsmStringVariable("Game Text Sheet").Value = "PC Contributor Tablets";
			}

			private void CreateCameraLock(string n, Vector2 pos, Vector2 scl, Vector2 cSize, Vector2 cOff, Vector2 min, Vector2 max, bool preventLookDown = false)
			{
				GameObject obj = new GameObject(n);
				BoxCollider2D boxCollider2D = obj.AddComponent<BoxCollider2D>();
				CameraLockArea cameraLockArea = obj.AddComponent<CameraLockArea>();
				obj.transform.position = pos;
				obj.transform.localScale = scl;
				boxCollider2D.isTrigger = true;
				boxCollider2D.size = cSize;
				boxCollider2D.offset = cOff;
				cameraLockArea.cameraXMin = min.x;
				cameraLockArea.cameraXMax = max.x;
				cameraLockArea.cameraYMin = (cameraLockArea.cameraYMax = min.y);
				cameraLockArea.preventLookDown = preventLookDown;
				cameraLockArea.maxPriority = true;
				obj.SetActive(value: true);
				bool flag = (cameraLockArea.enabled = true);
				boxCollider2D.enabled = flag;
			}

			private void FixIsmaSprites()
			{
				foreach (SpriteRenderer item in from x in UnityEngine.Object.FindObjectsOfType<SpriteRenderer>()
					where x.name.Contains("_white")
					select x)
				{
					item.material.shader = Shader.Find("Sprites/Default");
				}
			}

			private void FixPitDeath()
			{
				Log("Checking for Bottom");
				foreach (GameObject item in from x in UnityEngine.Object.FindObjectsOfType<GameObject>()
					where x.name == "Dream Fall Catcher"
					select x)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["DreamFall"]);
					gameObject.SetActive(value: false);
					BoxCollider2D componentInChildren = gameObject.GetComponentInChildren<BoxCollider2D>();
					BoxCollider2D componentInChildren2 = item.GetComponentInChildren<BoxCollider2D>();
					gameObject.transform.position = item.transform.position;
					componentInChildren.size = componentInChildren2.size;
					componentInChildren.offset = componentInChildren2.offset;
					componentInChildren.transform.position = componentInChildren2.transform.position;
					PlayMakerFSM playMakerFSM = gameObject.LocateMyFSM("Control");
					playMakerFSM.SetState("Pause");
					playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.FloatCompare>("Detect", 1).float2 = gameObject.transform.GetPositionY() + componentInChildren.size.y;
					Log(string.Format("Changing detect floor to {0}", playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.FloatCompare>("Detect", 1).float2.Value));
					playMakerFSM.MakeLog();
					UnityEngine.Object.Destroy(item);
					gameObject.SetActive(value: true);
				}
			}

			private void AddSuperDashCancel()
			{
				foreach (GameObject item in from x in UnityEngine.Object.FindObjectsOfType<GameObject>()
					where x.name.Contains("Superdash Cancel")
					select x)
				{
					item.AddComponent<SuperDashCancel>();
				}
			}

			private void FixHegemolArena()
			{
				CameraLockArea[] array = UnityEngine.Object.FindObjectsOfType<CameraLockArea>();
				for (int i = 0; i < array.Length; i++)
				{
					UnityEngine.Object.Destroy(array[i]);
				}
				CreateCameraLock("CLA1", new Vector2(325f, 156.1f), new Vector2(5f, 1.5f), new Vector2(18.9981f, 27.22f), new Vector2(-7.528451f, 2.554f), new Vector2(263f, 160f), new Vector2(323f, 160f), preventLookDown: true);
				CreateCameraLock("CLA1B", new Vector2(325f, 156.1f), new Vector2(5f, 1.5f), new Vector2(16.48628f, 27.22f), new Vector2(10.19836f, 2.554f), new Vector2(345f, 160f), new Vector2(402f, 160f), preventLookDown: true);
				CreateCameraLock("CLA2", new Vector2(437.5f, 174f), new Vector2(5f, 1f), new Vector2(10f, 45f), new Vector2(1f, 1.4f), new Vector2(434.7f, 160f), new Vector2(442.7f, 160f), preventLookDown: true);
				Log("Fixed floor");
				Renderer[] array2 = UnityEngine.Object.FindObjectsOfType<Renderer>();
				foreach (Renderer renderer in array2)
				{
					if (renderer.gameObject.name.Contains("dream particles") || renderer.gameObject.name.Contains("Dream Exit Particle Field"))
					{
						renderer.sortingOrder = 1;
					}
				}
				Log("Fixed renderer sorting orders");
				AudioSource[] array3 = UnityEngine.Object.FindObjectsOfType<AudioSource>();
				foreach (AudioSource audioSource in array3)
				{
					if (!(audioSource.outputAudioMixerGroup != null))
					{
						audioSource.outputAudioMixerGroup = HeroController.instance.GetComponent<AudioSource>().outputAudioMixerGroup;
					}
				}
			}

			private void AddBattleGate(float x, float y, Vector2 pos)
			{
				StartCoroutine(WorkBattleGate());
				IEnumerator WorkBattleGate()
				{
					foreach (GameObject item in from gameObject in UnityEngine.Object.FindObjectsOfType<GameObject>()
						where gameObject.name.Contains("Battle Gate")
						select gameObject)
					{
						UnityEngine.Object.Destroy(item);
					}
					HeroController hc = HeroController.instance;
					GameObject battleGate = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["BattleGate"]);
					battleGate.name = "opa";
					battleGate.SetActive(value: true);
					battleGate.transform.position = pos;
					battleGate.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
					PlayMakerFSM playMakerFSM = battleGate.LocateMyFSM("BG Control");
					AudioClip close = playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Close 1", 0).audioClip.Value as AudioClip;
					playMakerFSM.enabled = false;
					tk2dSpriteAnimator animGate = battleGate.GetComponent<tk2dSpriteAnimator>();
					BoxCollider2D bcGate = battleGate.GetComponent<BoxCollider2D>();
					AudioSource audGate = battleGate.GetComponent<AudioSource>();
					audGate.pitch = UnityEngine.Random.Range(0.9f, 1.2f);
					audGate.outputAudioMixerGroup = hc.GetComponent<AudioSource>().outputAudioMixerGroup;
					bcGate.enabled = false;
					yield return new WaitUntil(() => hc.transform.position.x > x && hc.transform.position.y < y);
					audGate.PlayOneShot(close);
					animGate.Play("BG Close 1");
					bcGate.enabled = true;
					yield return null;
					yield return new WaitWhile(() => animGate.IsPlaying("BG Close 1"));
					animGate.Play("BG Close 2");
					GameCameras.instance.cameraShakeFSM.SetState("EnemyKillShake");
					battleGate.transform.Find("Dust").GetComponent<ParticleSystem>().Play();
					battleGate.transform.Find("Close Effect").GetComponent<MeshRenderer>().enabled = true;
					battleGate.transform.Find("Close Effect").GetComponent<tk2dSpriteAnimator>().PlayFromFrame(0);
				}
			}

			private void CreateDreamGateway(string gateName, string toGate, Vector2 pos, Vector2 hitSize, Vector2 particSize, Vector2 particOff, string toScene, string returnScene)
			{
				Log("Creating Dream Gateway");
				GameObject gameObject = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["DPortal"]);
				gameObject.name = gateName;
				gameObject.SetActive(value: true);
				gameObject.transform.position = pos;
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.eulerAngles = Vector3.zero;
				BoxCollider2D component = gameObject.GetComponent<BoxCollider2D>();
				component.size = hitSize;
				component.offset = Vector2.zero;
				PlayMakerFSM[] components = gameObject.GetComponents<PlayMakerFSM>();
				foreach (PlayMakerFSM playMakerFSM in components)
				{
					if (!(playMakerFSM.FsmName != "Control"))
					{
						playMakerFSM.FsmVariables.GetFsmString("Return Scene").Value = returnScene;
						playMakerFSM.FsmVariables.GetFsmString("To Scene").Value = toScene;
						playMakerFSM.GetAction<HutongGames.PlayMaker.Actions.BeginSceneTransition>("Change Scene", 4).entryGateName = toGate;
					}
				}
				gameObject.transform.Find("Attack Pt").position += new Vector3(particOff.x, particOff.y);
				GameObject obj = UnityEngine.Object.Instantiate(FiveKnights.preloadedGO["DPortal2"]);
				obj.SetActive(value: true);
				obj.transform.position = new Vector3(pos.x + particOff.x, pos.y + particOff.y, -0.2f);
				obj.transform.localScale = Vector3.one;
				obj.transform.eulerAngles = Vector3.zero;
				ParticleSystem.ShapeModule shape = obj.GetComponent<ParticleSystem>().shape;
				shape.scale = new Vector3(particSize.x, particSize.y, 0.001f);
				Log("Done Creating Dream Gateway");
			}

			private void OnDestroy()
			{
				Log("Destroyed OWArenaFinder");
				UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= USceneManagerOnActiveSceneChanged;
				On.GameManager.EnterHero -= GameManagerOnEnterHero;
				On.GameManager.RefreshTilemapInfo -= GameManagerOnRefreshTilemapInfo;
				On.GameManager.GetCurrentMapZone -= GameManagerOnGetCurrentMapZone;
			}

			private static void Log(object o)
			{
				Modding.Logger.Log($"[OverWorldArena] {o}");
			}
		}
	}
