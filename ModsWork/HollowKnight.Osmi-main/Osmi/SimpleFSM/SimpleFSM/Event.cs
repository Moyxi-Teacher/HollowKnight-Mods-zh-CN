using Osmi.Game;

namespace Osmi.SimpleFSM;

public abstract partial class SimpleFSM {
	/// <summary>
	/// Sends an event to the fsm.
	/// </summary>
	/// <param name="eventName">The event to send</param>
	public void SendEvent(string eventName) {
		if (Info.GlobalTransitions.TryGetValue(eventName, out string stateName)) {
			SetState(stateName);
		} else if (CurrentTransitions.TryGetValue(eventName, out stateName)) {
			SetState(stateName);
		}
	}

	/// <summary>
	/// Sends a <see cref="FsmEvent"/> to the fsm.
	/// This method is a no-op if <see cref="AcceptPMFSMEvents"/>
	/// returns <see langword="false"/>.
	/// </summary>
	/// <param name="fsmEvent">The <see cref="FsmEvent"/> to send</param>
	public void SendPMFSMEvent(FsmEvent fsmEvent) {
		if (AcceptPMFSMEvents) {
			SendEvent(fsmEvent.Name);
		}
	}

	/// <summary>
	/// Sends an event to the fsm after given time.
	/// Note that the time is affected by <see cref="Time.timeScale"/>.
	/// </summary>
	/// <param name="eventName">The event to send</param>
	/// <param name="seconds">Time before the event is sent in seconds</param>
	public void SendEventDelayed(string eventName, float seconds) =>
		SendEventDelayed(eventName, new WaitForSeconds(seconds));

	/// <summary>
	/// Sends an event to the fsm after waiting for the given
	/// <see cref="YieldInstruction"/>.
	/// </summary>
	/// <param name="eventName">The event to send</param>
	/// <param name="inst">
	/// The <see cref="YieldInstruction"/> to wait for
	/// </param>
	public void SendEventDelayed(string eventName, YieldInstruction inst) =>
		GlobalCoroutineExecutor.Start(SendEventDelayedCoroutine(eventName, inst));

	/// <summary>
	/// Sends an event to the fsm after waiting for the
	/// given <see cref="IEnumerator"/>.
	/// </summary>
	/// <param name="eventName">The event to send</param>
	/// <param name="enumerator">The <see cref="IEnumerator"/> to wait for</param>
	public void SendEventDelayed(string eventName, IEnumerator enumerator) =>
		GlobalCoroutineExecutor.Start(SendEventDelayedCoroutine(eventName, enumerator));

	private IEnumerator SendEventDelayedCoroutine(string eventName, object obj) {
		yield return obj;
		SendEvent(eventName);
	}


	private Coroutine? waitCoro;

	private void WaitFor(object inst) {
		State = SimpleFSMState.Waiting;
		waitCoro = GlobalCoroutineExecutor.Start(WaitCoroutine(this, inst));
	}

	private IEnumerator WaitCoroutine(SimpleFSM self, object inst) {
		yield return inst;

		if (self == null) {
			yield break;
		}

		if (State == SimpleFSMState.Waiting) {
			State = SimpleFSMState.Running;
		}

		waitCoro = null;
	}
}
