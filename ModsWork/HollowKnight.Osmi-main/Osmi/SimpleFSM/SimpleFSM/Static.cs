namespace Osmi.SimpleFSM;

public abstract partial class SimpleFSM {
	/// <summary>
	/// "FINISHED" event constant
	/// </summary>
	public const string FINISHED = "FINISHED";

	private static readonly object lockObj = new();
	private static readonly HashSet<SimpleFSM> existingFSMs = new();
	private static readonly HashSet<SimpleFSM> activeFSMs = new();

	/// <summary>
	/// Precompile the <see cref="SimpleFSM"/> class <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The simple fsm class to precompile</typeparam>
	public static void Precompile<T>() where T : SimpleFSM =>
		_ = SimpleFSMInfo.map[typeof(T)];

	/// <summary>
	/// Precompile the <see cref="SimpleFSM"/> class <paramref name="t"/>.
	/// </summary>
	/// <param name="t">The simple fsm class to precompile</param>
	public static void Precompile(Type t) => _ = SimpleFSMInfo.map[t];

	/// <summary>
	/// Broadcast the event to all <see cref="SimpleFSM"/>s.
	/// </summary>
	/// <param name="eventName">The event to broadcast</param>
	/// <param name="includeInactive">Whether to broadcast to inactive fsms as well</param>
	public static void BroadcastEvent(string eventName, bool includeInactive = false) {
		foreach (SimpleFSM fsm in includeInactive ? existingFSMs : activeFSMs) {
			fsm.SendEvent(eventName);
		}
	}

	/// <summary>
	/// Broadcast the <see cref="FsmEvent"/> to all <see cref="SimpleFSM"/>s.
	/// </summary>
	/// <param name="fsmEvent">The <see cref="FsmEvent"/> to broadcast</param>
	/// <param name="includeInactive">Whether to broadcast to inactive fsms as well</param>
	public static void BroadcastPMFSMEvent(FsmEvent fsmEvent, bool includeInactive = false) {
		foreach (SimpleFSM fsm in includeInactive ? existingFSMs : activeFSMs) {
			fsm.SendPMFSMEvent(fsmEvent);
		}
	}

	/// <summary>
	/// Broadcast the event to all <see cref="SimpleFSM"/>s with given name.
	/// </summary>
	/// <param name="fsmName">The name of fsms to broadcast the event</param>
	/// <param name="eventName">The event to broadcast</param>
	/// <param name="includeInactive">Whether to broadcast to inactive fsms as well</param>
	public static void BroadcastEvent(string fsmName, string eventName, bool includeInactive = false) {
		foreach (SimpleFSM fsm in includeInactive ? existingFSMs : activeFSMs) {
			if (fsm.Name == fsmName) {
				fsm.SendEvent(eventName);
			}
		}
	}

	/// <summary>
	/// Broadcast the <see cref="FsmEvent"/> to all <see cref="SimpleFSM"/>s with given name.
	/// </summary>
	/// <param name="fsmName">The name of fsms to broadcast the <see cref="FsmEvent"/></param>
	/// <param name="fsmEvent">The <see cref="FsmEvent"/> to broadcast</param>
	/// <param name="includeInactive">Whether to broadcast to inactive fsms as well</param>
	public static void BroadcastPMFSMEvent(string fsmName, FsmEvent fsmEvent, bool includeInactive = false) {
		foreach (SimpleFSM fsm in includeInactive ? existingFSMs : activeFSMs) {
			if (fsm.Name == fsmName) {
				fsm.SendPMFSMEvent(fsmEvent);
			}
		}
	}

	/// <summary>
	/// Broadcast the event to all <see cref="SimpleFSM"/>s attached
	/// to the given <see cref="GameObject"/>.
	/// </summary>
	/// <param name="go">The <see cref="GameObject"/> to broadcast the event</param>
	/// <param name="eventName">The event to broadcast</param>
	/// <param name="includeInactive">Whether to broadcast to inactive fsms as well</param>
	public static void SendEventToGameObject(GameObject go, string eventName, bool includeInactive = false) {
		foreach (SimpleFSM fsm in includeInactive ? existingFSMs : activeFSMs) {
			if (fsm.gameObject == go) {
				fsm.SendEvent(eventName);
			}
		}
	}

	/// <summary>
	/// Broadcast the <see cref="FsmEvent"/> to all <see cref="SimpleFSM"/>s
	/// attached to the given <see cref="GameObject"/>.
	/// </summary>
	/// <param name="go">The <see cref="GameObject"/> to broadcast the <see cref="FsmEvent"/></param>
	/// <param name="fsmEvent">The <see cref="FsmEvent"/> to broadcast</param>
	/// <param name="includeInactive">Whether to broadcast to inactive fsms as well</param>
	public static void SendPMFSMEventToGameObject(GameObject go, FsmEvent fsmEvent, bool includeInactive = false) {
		foreach (SimpleFSM fsm in includeInactive ? existingFSMs : activeFSMs) {
			if (fsm.gameObject == go) {
				fsm.SendPMFSMEvent(fsmEvent);
			}
		}
	}

	/// <summary>
	/// Broadcast the event to all <see cref="SimpleFSM"/>s with given name
	/// attached to the given <see cref="GameObject"/>.
	/// </summary>
	/// <param name="go">The <see cref="GameObject"/> to broadcast the event</param>
	/// <param name="fsmName">The name of fsms to broadcast the event</param>
	/// <param name="eventName">The event to broadcast</param>
	/// <param name="includeInactive">Whether to broadcast to inactive fsms as well</param>
	public static void SendEventToGameObject(GameObject go, string fsmName, string eventName, bool includeInactive = false) {
		foreach (SimpleFSM fsm in includeInactive ? existingFSMs : activeFSMs) {
			if (fsm.gameObject == go && fsm.Name == fsmName) {
				fsm.SendEvent(eventName);
			}
		}
	}

	/// <summary>
	/// Broadcast the <see cref="FsmEvent"/> to all <see cref="SimpleFSM"/>s
	/// with given name attached to the given <see cref="GameObject"/>.
	/// </summary>
	/// <param name="go">The <see cref="GameObject"/> to broadcast the <see cref="FsmEvent"/></param>
	/// <param name="fsmName">The name of fsms to broadcast the <see cref="FsmEvent"/></param>
	/// <param name="fsmEvent">The <see cref="FsmEvent"/> to broadcast</param>
	/// <param name="includeInactive">Whether to broadcast to inactive fsms as well</param>
	public static void SendPMFSMEventToGameObject(GameObject go, string fsmName, FsmEvent fsmEvent, bool includeInactive = false) {
		foreach (SimpleFSM fsm in includeInactive ? existingFSMs : activeFSMs) {
			if (fsm.gameObject == go && fsm.Name == fsmName) {
				fsm.SendPMFSMEvent(fsmEvent);
			}
		}
	}


	private static void HookPMFSMEvents() {
		On.PlayMakerFSM.BroadcastEvent_FsmEvent += PMFSMEventForward.ForwardPMFSMComponentBroadcastEvent;
		On.FSMUtility.SendEventToGameObject_GameObject_FsmEvent_bool += PMFSMEventForward.ForwardPMFSMUtilSendToGameObject;
		On.HutongGames.PlayMaker.Fsm.BroadcastEvent_FsmEvent_bool += PMFSMEventForward.ForwardPMFSMBroadcastEvent;
		On.HutongGames.PlayMaker.Fsm.BroadcastEventToGameObject_GameObject_FsmEvent_FsmEventData_bool_bool += PMFSMEventForward.ForwardPMFSMBroadcastEventToGameObject;
		On.HutongGames.PlayMaker.Fsm.SendEventToFsmOnGameObject_GameObject_string_FsmEvent += PMFSMEventForward.ForwardPMFSMSendEventToFsmOnGameObject;
	}

	private static void UnhookPMFSMEvents() {
		On.PlayMakerFSM.BroadcastEvent_FsmEvent -= PMFSMEventForward.ForwardPMFSMComponentBroadcastEvent;
		On.FSMUtility.SendEventToGameObject_GameObject_FsmEvent_bool -= PMFSMEventForward.ForwardPMFSMUtilSendToGameObject;
		On.HutongGames.PlayMaker.Fsm.BroadcastEvent_FsmEvent_bool -= PMFSMEventForward.ForwardPMFSMBroadcastEvent;
		On.HutongGames.PlayMaker.Fsm.BroadcastEventToGameObject_GameObject_FsmEvent_FsmEventData_bool_bool -= PMFSMEventForward.ForwardPMFSMBroadcastEventToGameObject;
		On.HutongGames.PlayMaker.Fsm.SendEventToFsmOnGameObject_GameObject_string_FsmEvent -= PMFSMEventForward.ForwardPMFSMSendEventToFsmOnGameObject;
	}
}
