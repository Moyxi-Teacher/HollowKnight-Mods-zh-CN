namespace Osmi.FsmActions;

[PublicAPI]
public class BroadcastSimpleFSMEvent : FsmStateAction {
	public string? fsmName;
	public string? eventName;
	public bool includeInactive;

	public BroadcastSimpleFSMEvent() {
	}

	public BroadcastSimpleFSMEvent(string eventName, bool includeInactive = false) {
		this.eventName = eventName;
		this.includeInactive = includeInactive;
	}

	public BroadcastSimpleFSMEvent(string fsmName, string eventName, bool includeInactive = false) {
		this.fsmName = fsmName;
		this.eventName = eventName;
		this.includeInactive = includeInactive;
	}

	public override void OnEnter() {
		if (eventName != null) {
			if (fsmName == null) {
				SimpleFSM.SimpleFSM.BroadcastEvent(eventName, includeInactive);
			} else {
				SimpleFSM.SimpleFSM.BroadcastEvent(fsmName, eventName, includeInactive);
			}
		}

		Finish();
	}
}
