using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Hkmp.Math;
using HkmpPouch;
using HutongGames.PlayMaker;
using Microsoft.CodeAnalysis;
using Modding;
using On;
using On.HutongGames.PlayMaker;
using Satchel;
using UnityEngine;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: ComVisible(false)]
[assembly: Guid("a658d7a5-e9f4-4be4-84ec-711fda93f18c")]
[assembly: AssemblyTitle("Konpanion")]
[assembly: AssemblyProduct("Konpanion")]
[assembly: AssemblyCopyright("Copyright ©  2024")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0.0汉化")]
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: AssemblyVersion("1.0.0.0")]
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
namespace Konpanion
{
	public enum State
	{
		Idle,
		IdleFidget1,
		IdleFidget2,
		Turn,
		Walk,
		Jump,
		Teleport
	}
	public enum Direction
	{
		Left,
		Right
	}
	public class CompanionControl : MonoBehaviour
	{
		public bool isNetworkControlled;

		public tk2dSpriteAnimator animator;

		public Rigidbody2D rb;

		public BoxCollider2D collider;

		public AudioSource audioSource;

		public float scale;

		public float moveSpeed;

		public float IdleShuffleDistance;

		public GameObject followTarget;

		public float followDistance;

		public float teleportDistance;

		public AudioClip teleport;

		public AudioClip walk;

		public AudioClip yay;

		public Dictionary<State, string> Animations;

		public float followClipChance;

		public float teleportClipChance;

		public float turnClipChance;

		public float yayClipChance;

		internal State state;

		internal Direction lookDirection;

		private bool changeDirection;

		internal bool moveToNext;

		public Vector2 networkMovementTarget;

		public Coroutine networkCoro;

		public GameObject getFollowTarget()
		{
			if ((Object)(object)followTarget == (Object)null)
			{
				HeroController instance = HeroController.instance;
				if ((Object)(object)instance == (Object)null || (Object)(object)((Component)instance).gameObject == (Object)null)
				{
					return ((Component)this).gameObject;
				}
				followTarget = ((Component)instance).gameObject;
			}
			return followTarget;
		}

		private void Start()
		{
			collider = ((Component)this).gameObject.GetComponent<BoxCollider2D>();
			rb = GameObjectUtils.GetAddComponent<Rigidbody2D>(((Component)this).gameObject);
			animator = ((Component)this).gameObject.GetComponent<tk2dSpriteAnimator>();
			audioSource = GameObjectUtils.GetAddComponent<AudioSource>(((Component)this).gameObject);
			rb.bodyType = (RigidbodyType2D)0;
			((Collider2D)collider).isTrigger = false;
			GameObjectUtils.SetScale(((Component)this).gameObject, scale, scale);
			((MonoBehaviour)this).StartCoroutine(MainLoop());
		}

		public void playAnim(string clip, bool onlyNew = false)
		{
			if ((Object)(object)animator != (Object)null && (!onlyNew || !animator.IsPlaying(clip)))
			{
				animator.PlayFromFrame(clip, 0);
			}
		}

		public void playAnimForState(bool onlyNew = false)
		{
			string value2;
			if (Animations.TryGetValue(state, out var value))
			{
				playAnim(value, onlyNew);
			}
			else if (Animations.TryGetValue(State.Idle, out value2))
			{
				playAnim(value2, onlyNew);
			}
		}

		private bool heroFartherThan(float distance)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = getFollowTarget().transform.position - ((Component)this).transform.position;
			return ((Vector3)(ref val)).magnitude > distance;
		}

		private bool heroFartherThanY(float distance)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			return (getFollowTarget().transform.position - ((Component)this).transform.position).y > distance;
		}

		private bool UpdateLookDirection()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			changeDirection = false;
			Direction direction = ((getFollowTarget().transform.position.x > ((Component)this).transform.position.x) ? Direction.Right : Direction.Left);
			if (direction != lookDirection)
			{
				changeDirection = true;
				lookDirection = direction;
			}
			return changeDirection;
		}

		private void fixRotation()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			((Component)this).gameObject.transform.rotation = Quaternion.identity;
		}

		private IEnumerator TurnToHero()
		{
			fixRotation();
			playAnimForState();
			if ((Object)(object)teleport != (Object)null && !audioSource.isPlaying && Random.Range(0f, 1f) < turnClipChance)
			{
				audioSource.PlayOneShot(teleport);
			}
			yield return (object)new WaitForSeconds(0.5f);
			Vector3 localScale = ((Component)this).gameObject.transform.localScale;
			localScale.x = ((lookDirection == Direction.Left) ? 1f : (-1f)) * Mathf.Abs(localScale.x);
			((Component)this).gameObject.transform.localScale = localScale;
			state = State.Idle;
			moveToNext = true;
		}

		private void decideNextState()
		{
			bool flag = true;
			if ((Object)(object)getFollowTarget() == (Object)(object)HeroController.instance)
			{
				flag = HeroController.instance.cState.onGround || Random.Range(0f, 1f) < 0.3f;
			}
			if (UpdateLookDirection())
			{
				state = State.Turn;
			}
			else if (heroFartherThan(teleportDistance) & flag)
			{
				state = State.Teleport;
			}
			else if (heroFartherThan(followDistance) & flag)
			{
				if (heroFartherThanY(0.5f) && Random.Range(0f, 1f) < 0.3f)
				{
					state = State.Jump;
				}
				else
				{
					state = State.Walk;
				}
			}
			else
			{
				state = State.Idle;
			}
		}

		private IEnumerator Idle()
		{
			fixRotation();
			playAnimForState(onlyNew: true);
			if ((Object)(object)yay != (Object)null && !audioSource.isPlaying && Random.Range(0f, 1f) < yayClipChance)
			{
				audioSource.PlayOneShot(yay);
			}
			rb.velocity = new Vector2(0f, 0f);
			State state = this.state;
			decideNextState();
			if (this.state == State.Idle && (state == State.IdleFidget1 || state == State.IdleFidget2))
			{
				this.state = state;
				yield return (object)new WaitForSeconds(0.1f);
			}
			else if (state == State.Idle)
			{
				float num = Random.Range(0f, 1f);
				if (num < 0.001f)
				{
					this.state = State.IdleFidget1;
				}
				else if (num < 0.011f)
				{
					this.state = State.IdleFidget2;
				}
				yield return (object)new WaitForSeconds(0.1f);
			}
			else if (state != this.state || this.state != State.Walk)
			{
				yield return (object)new WaitForSeconds(1f);
			}
			moveToNext = true;
		}

		private IEnumerator Teleport()
		{
			fixRotation();
			if ((Object)(object)teleport != (Object)null && !audioSource.isPlaying && Random.Range(0f, 1f) < teleportClipChance)
			{
				audioSource.PlayOneShot(teleport);
			}
			playAnimForState();
			float num = Random.Range(-0.5f, 0.5f);
			if (Random.Range(0f, 1f) < 0.5f)
			{
				((Component)this).gameObject.transform.position = getFollowTarget().transform.position + new Vector3(0.5f + num, 0f, 0f);
			}
			else
			{
				((Component)this).gameObject.transform.position = getFollowTarget().transform.position + new Vector3(-0.5f + num, 0f, 0f);
			}
			yield return (object)new WaitForSeconds(0.1f);
			state = State.Idle;
			moveToNext = true;
		}

		private IEnumerator Follow()
		{
			playAnimForState();
			if ((Object)(object)walk != (Object)null && !audioSource.isPlaying && Random.Range(0f, 1f) < followClipChance)
			{
				audioSource.PlayOneShot(walk);
			}
			Vector2 displacement = Vector2.op_Implicit(getFollowTarget().transform.position - ((Component)this).transform.position);
			displacement += new Vector2(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f));
			if (state == State.Jump)
			{
				displacement += new Vector2(0f, 3f);
			}
			float num = followDistance * (1f + Random.Range(-0.25f, 0.5f));
			float distance = Mathf.Min(teleportDistance, ((Vector2)(ref displacement)).magnitude - num);
			float moveSpeedR = moveSpeed * (1f + Random.Range(-0.25f, 0.5f));
			yield return EnemyUtils.moveTowards(rb, displacement, distance * 0.5f, distance * 0.5f / moveSpeedR);
			playAnimForState();
			yield return EnemyUtils.moveTowards(rb, displacement, distance * 0.5f, distance * 0.5f / moveSpeedR);
			state = State.Idle;
			moveToNext = true;
		}

		private IEnumerator ApplyNetworkState()
		{
			yield return null;
			playAnimForState();
			Vector3 localScale = ((Component)this).gameObject.transform.localScale;
			localScale.x = ((lookDirection == Direction.Left) ? 1f : (-1f)) * Mathf.Abs(localScale.x);
			((Component)this).gameObject.transform.localScale = localScale;
			float value = 0.1f;
			if (state == State.Teleport)
			{
				((Component)this).transform.position = Vector2.op_Implicit(networkMovementTarget);
				yield break;
			}
			while (Vector2.op_Implicit(((Component)this).transform.position) != networkMovementTarget)
			{
				((Component)this).transform.position = Vector2.op_Implicit(Vector2.Lerp(Vector2.op_Implicit(((Component)this).transform.position), networkMovementTarget, value));
				value += 0.1f;
				yield return (object)new WaitForSeconds(0.02f);
			}
		}

		internal void UpdateNetworkCoro()
		{
			if (networkCoro != null)
			{
				((MonoBehaviour)this).StopCoroutine(networkCoro);
			}
			networkCoro = ((MonoBehaviour)this).StartCoroutine(ApplyNetworkState());
		}

		private IEnumerator MainLoop()
		{
			while (true)
			{
				yield return (object)new WaitWhile((Func<bool>)(() => !moveToNext));
				moveToNext = false;
				if (!isNetworkControlled)
				{
					if (Konpanion.HasPouch())
					{
						PouchIntegration.SendUpdate(this);
					}
					if (state == State.Idle || state == State.IdleFidget1 || state == State.IdleFidget2)
					{
						((MonoBehaviour)this).StartCoroutine(Idle());
					}
					else if (state == State.Walk)
					{
						((MonoBehaviour)this).StartCoroutine(Follow());
					}
					else if (state == State.Jump)
					{
						((MonoBehaviour)this).StartCoroutine(Follow());
					}
					else if (state == State.Teleport)
					{
						((MonoBehaviour)this).StartCoroutine(Teleport());
					}
					else if (state == State.Turn)
					{
						((MonoBehaviour)this).StartCoroutine(TurnToHero());
					}
				}
			}
		}

		public CompanionControl()
		{
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			scale = 0.6f;
			moveSpeed = 15f;
			IdleShuffleDistance = 0.1f;
			followDistance = 2f;
			teleportDistance = 15f;
			Animations = new Dictionary<State, string>();
			followClipChance = 0.03f;
			teleportClipChance = 0.6f;
			turnClipChance = 30f;
			yayClipChance = 0.01f;
			moveToNext = true;
			networkMovementTarget = new Vector2(0f, 0f);
			((MonoBehaviour)this)..ctor();
		}
	}
	internal class Constants
	{
		public const char Separator = '|';

		public static char[] SplitSep = new char[1] { '|' };
	}
	public class Konpanion : Mod, IMenuMod, IMod, ILogger, ITogglableMod
	{
		[CompilerGenerated]
		private static class <>O
		{
			public static hook_GetOwnerDefaultTarget <0>__OnGetOwnerDefaultTarget;
		}

		internal static Konpanion Instance;

		internal static List<GameObject> knights = new List<GameObject>();

		internal static Dictionary<ushort, GameObject> remoteKnights = new Dictionary<ushort, GameObject>();

		private const string BenchFsmName = "Bench Control";

		private const string HeroControllerName = "HeroController";

		private const string HeroRefName = "Hero";

		private const BindingFlags AllInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private bool _loggedBenchFix;

		private int _fixedTotal;

		private int _hookFixedTotal;

		private int _hookCalls;

		private int _stateFixedCount;

		private const string CanRestStateName = "Can Rest?";

		public bool ToggleButtonInsideMenu => true;

		public Konpanion()
			: base("伴侣")
		{
		}

		public List<MenuEntry> GetMenuData(MenuEntry? toggleButtonEntry)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			List<MenuEntry> list = new List<MenuEntry>();
			if (toggleButtonEntry.HasValue)
			{
				MenuEntry value = toggleButtonEntry.Value;
				value.Description = "跟随在小骑士身边的伙伴，在 HKMP 多人模式下会显示远程玩家的伴侣。";
				list.Add(value);
			}
			return list;
		}

		public static bool HasPouch()
		{
			return ModHooks.GetMod("HkmpPouch", false, false) is Mod;
		}

		public override string GetVersion()
		{
			return "1.0";
		}

		public GameObject createKnightCompanion(GameObject ft = null)
		{
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			HeroController instance = HeroController.instance;
			if ((Object)(object)instance == (Object)null || (Object)(object)((Component)instance).gameObject == (Object)null)
			{
				return null;
			}
			GameObject gameObject = ((Component)instance).gameObject;
			gameObject.SetActive(false);
			GameObject val;
			try
			{
				val = EnemyUtils.createCompanionFromPrefab(gameObject);
			}
			finally
			{
				gameObject.SetActive(true);
			}
			GameObjectUtils.RemoveComponent<HeroController>(val);
			GameObjectUtils.RemoveComponent<HeroAnimationController>(val);
			GameObjectUtils.RemoveComponent<HeroAudioController>(val);
			GameObjectUtils.RemoveComponent<ConveyorMovementHero>(val);
			((Object)val).name = "kompanion";
			((Renderer)GameObjectUtils.GetAddComponent<MeshRenderer>(val)).enabled = true;
			GameObjectUtils.GetAddComponent<Rigidbody2D>(val).gravityScale = 1f;
			CompanionControl addComponent = GameObjectUtils.GetAddComponent<CompanionControl>(val);
			addComponent.moveSpeed = 10f;
			addComponent.followDistance = 2f;
			addComponent.IdleShuffleDistance = 0.01f;
			if ((Object)(object)ft != (Object)null)
			{
				addComponent.followTarget = ft;
			}
			BoxCollider2D addComponent2 = GameObjectUtils.GetAddComponent<BoxCollider2D>(val);
			addComponent2.size = new Vector2(1f, 2f);
			((Collider2D)addComponent2).offset = new Vector2(0f, -0.4f);
			((Behaviour)addComponent2).enabled = true;
			addComponent.Animations.Add(State.Idle, "Idle");
			addComponent.Animations.Add(State.IdleFidget1, "Acid Death");
			addComponent.Animations.Add(State.IdleFidget2, "Prostrate");
			addComponent.Animations.Add(State.Walk, "Run");
			addComponent.Animations.Add(State.Turn, "Map Walk");
			addComponent.Animations.Add(State.Teleport, "Fall");
			addComponent.Animations.Add(State.Jump, "Airborne");
			val.SetActive(true);
			RepairBenchHeroRefs();
			return val;
		}

		public override void Initialize()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Expected O, but got Unknown
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Expected O, but got Unknown
			Instance = this;
			ModHooks.HeroUpdateHook += update;
			GameManager.OnNextLevelReady += new hook_OnNextLevelReady(OnNextLevelReady);
			object obj = <>O.<0>__OnGetOwnerDefaultTarget;
			if (obj == null)
			{
				hook_GetOwnerDefaultTarget val = OnGetOwnerDefaultTarget;
				<>O.<0>__OnGetOwnerDefaultTarget = val;
				obj = (object)val;
			}
			Fsm.GetOwnerDefaultTarget += (hook_GetOwnerDefaultTarget)obj;
			FsmState.OnEnter += new hook_OnEnter(OnFsmStateEnter);
			if (HasPouch())
			{
				PouchIntegration.Initialize();
			}
		}

		public void Unload()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			ModHooks.HeroUpdateHook -= update;
			GameManager.OnNextLevelReady -= new hook_OnNextLevelReady(OnNextLevelReady);
			object obj = <>O.<0>__OnGetOwnerDefaultTarget;
			if (obj == null)
			{
				hook_GetOwnerDefaultTarget val = OnGetOwnerDefaultTarget;
				<>O.<0>__OnGetOwnerDefaultTarget = val;
				obj = (object)val;
			}
			Fsm.GetOwnerDefaultTarget -= (hook_GetOwnerDefaultTarget)obj;
			FsmState.OnEnter -= new hook_OnEnter(OnFsmStateEnter);
			Instance = null;
		}

		public GameObject GetNetworkKonpanion(ushort id)
		{
			if (remoteKnights.TryGetValue(id, out var value))
			{
				return value;
			}
			remoteKnights[id] = createKnightCompanion();
			return remoteKnights[id];
		}

		public void update()
		{
			if (knights.Count < 1 || (Object)(object)knights[0] == (Object)null)
			{
				knights.Clear();
				GameObject val = createKnightCompanion();
				if ((Object)(object)val != (Object)null)
				{
					knights.Add(val);
				}
			}
		}

		private void OnNextLevelReady(orig_OnNextLevelReady orig, GameManager self)
		{
			orig.Invoke(self);
			try
			{
				RepairBenchHeroRefs();
			}
			catch
			{
			}
		}

		private void OnFsmStateEnter(orig_OnEnter orig, FsmState self)
		{
			try
			{
				if (self != null && self.Fsm != null && self.Fsm.Name == "Bench Control" && self.Name == "Can Rest?")
				{
					FixHeroRefsInState(self);
				}
			}
			catch
			{
			}
			orig.Invoke(self);
		}

		private void FixHeroRefsInState(FsmState st)
		{
			HeroController instance = HeroController.instance;
			if ((Object)(object)instance == (Object)null)
			{
				return;
			}
			GameObject gameObject = ((Component)instance).gameObject;
			if ((Object)(object)gameObject == (Object)null)
			{
				return;
			}
			FsmStateAction[] actions = st.Actions;
			if (actions == null)
			{
				return;
			}
			foreach (FsmStateAction val in actions)
			{
				if (val == null)
				{
					continue;
				}
				Type type = ((object)val).GetType();
				string behaviourName = GetBehaviourName(val, type);
				FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (FieldInfo fieldInfo in fields)
				{
					if (fieldInfo.FieldType != typeof(FsmOwnerDefault))
					{
						continue;
					}
					object value = fieldInfo.GetValue(val);
					FsmOwnerDefault val2 = (FsmOwnerDefault)((value is FsmOwnerDefault) ? value : null);
					if (val2 == null || val2.GameObject == null)
					{
						continue;
					}
					string text;
					try
					{
						text = ((NamedVariable)val2.GameObject).Name ?? string.Empty;
					}
					catch
					{
						text = string.Empty;
					}
					if ((!(text != "Hero") || !(behaviourName != "HeroController")) && !((Object)(object)val2.GameObject.Value == (Object)(object)gameObject))
					{
						try
						{
							((NamedVariable)val2.GameObject).CastVariable = null;
							val2.GameObject.Value = gameObject;
						}
						catch
						{
						}
						_stateFixedCount++;
						if (_stateFixedCount <= 10)
						{
							Logger.Log("[伴侣] 进入「Can Rest?」时已摆正英雄引用（累计第 " + _stateFixedCount + " 次）");
						}
					}
				}
			}
		}

		private static GameObject OnGetOwnerDefaultTarget(orig_GetOwnerDefaultTarget orig, Fsm self, FsmOwnerDefault ownerDefault)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Invalid comparison between Unknown and I4
			GameObject val = orig.Invoke(self, ownerDefault);
			try
			{
				if (self == null || ownerDefault == null)
				{
					return val;
				}
				if ((int)ownerDefault.OwnerOption != 1)
				{
					return val;
				}
				if (self.Name != "Bench Control")
				{
					return val;
				}
				FsmGameObject gameObject = ownerDefault.GameObject;
				if (gameObject == null)
				{
					return val;
				}
				if (!string.Equals(((NamedVariable)gameObject).Name, "Hero", StringComparison.Ordinal))
				{
					return val;
				}
				Konpanion instance = Instance;
				if (instance != null)
				{
					instance._hookCalls++;
					if (instance._hookCalls == 1)
					{
						Logger.Log("[伴侣] 英雄引用解析钩子已生效（Fsm.GetOwnerDefaultTarget 已挂接）");
					}
				}
				HeroController instance2 = HeroController.instance;
				if ((Object)(object)instance2 == (Object)null)
				{
					return val;
				}
				GameObject gameObject2 = ((Component)instance2).gameObject;
				if ((Object)(object)gameObject2 == (Object)null || (Object)(object)val == (Object)(object)gameObject2)
				{
					return val;
				}
				if (instance != null)
				{
					instance._hookFixedTotal++;
					if (instance._hookFixedTotal <= 5 || instance._hookFixedTotal % 500 == 0)
					{
						Logger.Log("[伴侣] 已拦截被顶替的英雄引用（原本解析到 \"" + (((Object)(object)val != (Object)null) ? ((Object)val).name : "null") + "\"），" + $"已改回小骑士本体；累计第 {instance._hookFixedTotal} 次");
					}
				}
				return gameObject2;
			}
			catch
			{
				return val;
			}
		}

		private void RepairBenchHeroRefs()
		{
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Invalid comparison between Unknown and I4
			HeroController instance = HeroController.instance;
			if ((Object)(object)instance == (Object)null)
			{
				return;
			}
			GameObject gameObject = ((Component)instance).gameObject;
			if ((Object)(object)gameObject == (Object)null)
			{
				return;
			}
			PlayMakerFSM[] array;
			try
			{
				array = Resources.FindObjectsOfTypeAll<PlayMakerFSM>();
			}
			catch
			{
				return;
			}
			PlayMakerFSM[] array2 = array;
			foreach (PlayMakerFSM val in array2)
			{
				if ((Object)(object)val == (Object)null || val.FsmName != "Bench Control")
				{
					continue;
				}
				Fsm fsm = val.Fsm;
				if (fsm == null || fsm.States == null)
				{
					continue;
				}
				FsmState[] states = fsm.States;
				foreach (FsmState val2 in states)
				{
					if (val2 == null || val2.Actions == null)
					{
						continue;
					}
					FsmStateAction[] actions = val2.Actions;
					foreach (FsmStateAction val3 in actions)
					{
						if (val3 == null)
						{
							continue;
						}
						Type type = ((object)val3).GetType();
						string behaviourName = GetBehaviourName(val3, type);
						FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						foreach (FieldInfo fieldInfo in fields)
						{
							if (fieldInfo.FieldType != typeof(FsmOwnerDefault))
							{
								continue;
							}
							object value = fieldInfo.GetValue(val3);
							FsmOwnerDefault val4 = (FsmOwnerDefault)((value is FsmOwnerDefault) ? value : null);
							if (val4 == null || val4.GameObject == null || (int)val4.OwnerOption != 1)
							{
								continue;
							}
							string text;
							try
							{
								text = ((NamedVariable)val4.GameObject).Name ?? string.Empty;
							}
							catch
							{
								text = string.Empty;
							}
							if ((text != "Hero" && behaviourName != "HeroController") || (Object)(object)val4.GameObject.Value == (Object)(object)gameObject)
							{
								continue;
							}
							GameObject value2 = val4.GameObject.Value;
							try
							{
								((NamedVariable)val4.GameObject).CastVariable = null;
								((NamedVariable)val4.GameObject).UseVariable = false;
							}
							catch
							{
							}
							val4.GameObject.Value = gameObject;
							if (fsm.Variables != null)
							{
								FsmGameObject fsmGameObject = fsm.Variables.GetFsmGameObject(((NamedVariable)val4.GameObject).Name);
								if (fsmGameObject != null && fsmGameObject != val4.GameObject)
								{
									fsmGameObject.Value = gameObject;
								}
							}
							ClearMethodCache(val3, type);
							_fixedTotal++;
							if (!_loggedBenchFix || _fixedTotal <= 5)
							{
								_loggedBenchFix = true;
								((Loggable)this).Log("[伴侣] 已纠正长椅状态机中指向伙伴克隆体的英雄引用" + string.Format("（「{0}」{1}，原为 {2}；累计第 {3} 次）", val2.Name, type.Name, ((Object)(object)value2 == (Object)null) ? "null" : ((Object)value2).name, _fixedTotal));
							}
						}
					}
				}
			}
		}

		private static string GetBehaviourName(object action, Type type)
		{
			try
			{
				FieldInfo field = type.GetField("behaviour", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (field == null)
				{
					return string.Empty;
				}
				object value = field.GetValue(action);
				if (value == null)
				{
					return string.Empty;
				}
				PropertyInfo property = value.GetType().GetProperty("Value");
				return (property != null) ? ((property.GetValue(value) as string) ?? string.Empty) : string.Empty;
			}
			catch
			{
				return string.Empty;
			}
		}

		private static void ClearMethodCache(object action, Type type)
		{
			string[] array = new string[5] { "cachedBehaviour", "cachedType", "cachedMethodInfo", "cachedParameterInfo", "errorString" };
			foreach (string name in array)
			{
				try
				{
					FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (field != null)
					{
						field.SetValue(action, null);
					}
				}
				catch
				{
				}
			}
		}
	}
	internal class KonpanionUpdate : PipeEvent
	{
		public static string Name = "KonpanionUpdate";

		public Vector2 pos { get; set; }

		public State anim { get; set; }

		public Direction dir { get; set; }

		public override string GetName()
		{
			return Name;
		}

		public override string ToString()
		{
			return $"{Utilities.i2s((int)anim)}{'|'}{Utilities.i2s((int)dir)}{'|'}{Utilities.f2s(pos.X)}{'|'}{Utilities.f2s(pos.Y)}";
		}
	}
	internal class KonpanionUpdateFactory : IEventFactory
	{
		public static KonpanionUpdateFactory Instance { get; internal set; } = new KonpanionUpdateFactory();

		public PipeEvent FromSerializedString(string serializedData)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Expected O, but got Unknown
			KonpanionUpdate konpanionUpdate = new KonpanionUpdate();
			string[] array = serializedData.Split(Constants.SplitSep);
			konpanionUpdate.anim = (State)Utilities.s2i(array[0]);
			konpanionUpdate.dir = (Direction)Utilities.s2i(array[1]);
			konpanionUpdate.pos = new Vector2(Utilities.s2f(array[2]), Utilities.s2f(array[3]));
			return (PipeEvent)(object)konpanionUpdate;
		}

		public string GetName()
		{
			return KonpanionUpdate.Name;
		}
	}
	internal static class Utilities
	{
		public static string f2s(float x)
		{
			return x.ToString("0.00", CultureInfo.InvariantCulture);
		}

		public static float s2f(string v)
		{
			return float.Parse(v, CultureInfo.InvariantCulture);
		}

		public static int s2i(string s)
		{
			return int.Parse(s, CultureInfo.InvariantCulture);
		}

		public static string i2s(int i)
		{
			return i.ToString(CultureInfo.InvariantCulture);
		}
	}
	internal static class SafePouchIntegration
	{
		internal static PipeClient pipe;

		static SafePouchIntegration()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Expected O, but got Unknown
			pipe = new PipeClient("Konpanion");
		}
	}
	internal static class PouchIntegration
	{
		internal static string oldUpdate;

		internal static void Initialize()
		{
			((OnAble)SafePouchIntegration.pipe).On((IEventFactory)(object)KonpanionUpdateFactory.Instance).Do<KonpanionUpdate>((Action<KonpanionUpdate>)KonpanionUpdateHandler);
		}

		internal static void SendUpdate(CompanionControl companionControl)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			if (SafePouchIntegration.pipe != null && SafePouchIntegration.pipe.ClientApi != null && SafePouchIntegration.pipe.ClientApi.NetClient.IsConnected)
			{
				KonpanionUpdate konpanionUpdate = new KonpanionUpdate
				{
					pos = (Vector2)Vector2.op_Implicit(((Component)companionControl).transform.position),
					dir = companionControl.lookDirection,
					anim = companionControl.state
				};
				if (oldUpdate != ((object)konpanionUpdate).ToString())
				{
					SafePouchIntegration.pipe.Broadcast((PipeEvent)(object)konpanionUpdate, true, true);
					oldUpdate = ((object)konpanionUpdate).ToString();
				}
			}
		}

		internal static void KonpanionUpdateHandler(KonpanionUpdate update)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			CompanionControl component = Konpanion.Instance.GetNetworkKonpanion(((baseEventData)update).FromPlayer).GetComponent<CompanionControl>();
			component.state = update.anim;
			component.networkMovementTarget = new Vector2(update.pos.X, update.pos.Y);
			component.lookDirection = update.dir;
			component.isNetworkControlled = true;
			component.UpdateNetworkCoro();
		}
	}
}
