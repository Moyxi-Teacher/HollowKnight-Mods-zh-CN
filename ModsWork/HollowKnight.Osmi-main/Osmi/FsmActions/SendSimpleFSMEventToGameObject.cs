using Osmi.SimpleFSM;

namespace Osmi.FsmActions;

[PublicAPI]
public class SendSimpleFSMEventToGameObject : FsmStateAction {
	public GameObject? gameObject;
	public string? fsmName;
	public string? eventName;
	public bool includeInactive;

	public SendSimpleFSMEventToGameObject() {
	}

	public SendSimpleFSMEventToGameObject(string eventName, bool includeInactive = false) {
		this.eventName = eventName;
		this.includeInactive = includeInactive;
	}

	public SendSimpleFSMEventToGameObject(string fsmName, string eventName, bool includeInactive = false) {
		this.fsmName = fsmName;
		this.eventName = eventName;
	}

	public SendSimpleFSMEventToGameObject(GameObject gameObject, string eventName, bool includeInactive = false) {
		this.gameObject = gameObject;
		this.eventName = eventName;
		this.includeInactive = includeInactive;
	}

	public SendSimpleFSMEventToGameObject(GameObject gameObject, string fsmName, string eventName, bool includeInactive = false) {
		this.gameObject = gameObject;
		this.fsmName = fsmName;
		this.eventName = eventName;
		this.includeInactive = includeInactive;
	}

	public override void OnEnter() {
		if (eventName != null) {
			GameObject target = gameObject ?? Owner;

			if (fsmName == null) {
				target.SendSimpleFSMEvent(eventName, includeInactive);
			} else {
				target.SendSimpleFSMEvent(fsmName, eventName, includeInactive);
			}
		}

		Finish();
	}
}
