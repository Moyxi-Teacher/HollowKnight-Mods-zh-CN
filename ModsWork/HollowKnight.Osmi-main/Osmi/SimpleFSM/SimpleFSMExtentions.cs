using Osmi.FsmActions;

namespace Osmi.SimpleFSM;

/// <summary>
/// Extension methods for easier interaction with <see cref="SimpleFSM"/>.
/// </summary>
[PublicAPI]
public static class SimpleFSMExtensions {
	/// <summary>
	/// Constant of name of the proxy <see cref="PlayMakerFSM"/> created by
	/// <see cref="CreatePlayMakerFSMEventProxy(GameObject, FsmEvent)"/>.
	/// </summary>
	public const string PROXY_PMFSM_NAME = "SimpleFSM Event Proxy";

	/// <summary>
	/// Locates <see cref="SimpleFSM"/>s with the given name attached to
	/// the given <see cref="GameObject"/>.
	/// </summary>
	/// <param name="self" />
	/// <param name="name">The name of the fsms</param>
	/// <returns><see cref="SimpleFSM"/>s with given name attached to given <see cref="GameObject"/></returns>
	public static IEnumerable<SimpleFSM> LocateSimpleFSMs(this GameObject self, string name) =>
		self.GetComponents<SimpleFSM>()
			.Filter(fsm => fsm.Name == name);

	/// <summary>
	/// Locates the first <see cref="SimpleFSM"/> with the given name attached
	/// to the given <see cref="GameObject"/>.
	/// </summary>
	/// <param name="self" />
	/// <param name="name">The name of the fsms</param>
	/// <returns>The <see cref="SimpleFSM"/> with given name attached to given <see cref="GameObject"/></returns>
	public static SimpleFSM? LocateSimpleFSM(this GameObject self, string name) =>
		self.LocateSimpleFSMs(name).FirstOrDefault();

	/// <inheritdoc cref="SimpleFSM.SendEventToGameObject(GameObject, string, bool)"/>
	public static void SendSimpleFSMEvent(this GameObject self, string eventName, bool includeInactive = false) =>
		SimpleFSM.SendEventToGameObject(self, eventName, includeInactive);

	/// <inheritdoc cref="SimpleFSM.SendEventToGameObject(GameObject, string, string, bool)"/>
	public static void SendSimpleFSMEvent(this GameObject self, string fsmName, string eventName, bool includeInactive = false) =>
		SimpleFSM.SendEventToGameObject(self, fsmName, eventName, includeInactive);


	/// <param name="self" />
	/// <param name="fsmEvent">The name of the <see cref="FsmEvent"/> to proxy</param>
	/// <inheritdoc cref="CreatePlayMakerFSMEventProxy(GameObject, FsmEvent)"/>
	public static PlayMakerFSM CreatePlayMakerFSMEventProxy(this GameObject self, string fsmEvent) =>
		self.CreatePlayMakerFSMEventProxy(FsmEvent.GetFsmEvent(fsmEvent));

	/// <summary>
	/// Creates or enables a proxy <see cref="PlayMakerFSM"/> that forwards the
	/// given <see cref="FsmEvent"/> to <see cref="SimpleFSM"/>s on the given
	/// <see cref="GameObject"/>. In most case, the default forwarding mechanic
	/// of <see cref="FsmEvent"/> is enough, only use this when you need to
	/// fill in some fields or parameters of <see cref="PlayMakerFSM"/> type.
	/// </summary>
	/// <param name="self" />
	/// <param name="fsmEvent">The <see cref="FsmEvent"/> to proxy</param>
	/// <returns>The proxy <see cref="PlayMakerFSM"/></returns>
	public static PlayMakerFSM CreatePlayMakerFSMEventProxy(this GameObject self, FsmEvent fsmEvent) {
		string name = fsmEvent.Name;

		if (self.LocateMyFSM(PROXY_PMFSM_NAME) is not PlayMakerFSM pmfsm) {
			pmfsm = self.AddComponent<PlayMakerFSM>();
			pmfsm.FsmName = PROXY_PMFSM_NAME;
		}

		Fsm fsm = pmfsm.Fsm;

		if (fsm.GetState(name) == null) {
			FsmState[] states = fsm.States;
			Array.Resize(ref states, states.Length + 1);
			FsmState state = states[states.Length - 1] = new FsmState(fsm) {
				Name = name,
				Actions = new FsmStateAction[] {
					new SendSimpleFSMEventToGameObject(name)
				}
			};
			fsm.States = states;

			FsmTransition[] globalTransitions = fsm.GlobalTransitions;
			Array.Resize(ref globalTransitions, globalTransitions.Length + 1);
			globalTransitions[globalTransitions.Length - 1] = new FsmTransition() {
				FsmEvent = fsmEvent,
				ToFsmState = state,
				ToState = name
			};
			fsm.GlobalTransitions = globalTransitions;
		}

		pmfsm.enabled = true;
		return pmfsm;
	}
}
