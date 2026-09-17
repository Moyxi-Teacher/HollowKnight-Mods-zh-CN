using Osmi.ChildRefs;
using Osmi.Game;

namespace Osmi.SimpleFSM;

public abstract partial class SimpleFSM : MonoBehaviour {
	/// <summary>
	/// To use the unity message methods that are already declared in the
	/// <see cref="SimpleFSM"/> base class, do the following:
	/// <code>
	/// protected new void Awake() { // Same for other messages
	///		// Something to do before base class method
	///		base.Awake();
	///		// Something to do after base class method
	/// }
	/// </code>
	/// Do NOT omit the call to base class method unless you know what you
	/// are doing.
	/// </summary>
	protected void Awake() {
		Type = GetType();
		Name = Type.Name;
		info = new(() => SimpleFSMInfo.map[Type]);

		lock (lockObj) {
			if (existingFSMs.Count == 0) {
				HookPMFSMEvents();
			}

			_ = existingFSMs.Add(this);
		}

		if (InitChildRefsOnAwake) {
			this.InitChildRefs();
		}
	}

	/// <inheritdoc cref="Awake"/>
	protected void Start() {
		if (!InitChildRefsOnAwake) {
			this.InitChildRefs();
		}

		SetState(nameof(Init));
	}

	/// <inheritdoc cref="Awake"/>
	protected void OnEnable() {
		if (RestartOnEnable) {
			SetState(nameof(Init));
		}

		_ = activeFSMs.Add(this);
		State = SimpleFSMState.Running;
	}

	/// <inheritdoc cref="Awake"/>
	protected void OnDisable() {
		State = SimpleFSMState.Disabled;

		if (waitCoro != null) {
			GlobalCoroutineExecutor.Stop(waitCoro);
			waitCoro = null;
		}

		_ = activeFSMs.Remove(this);
	}

	/// <inheritdoc cref="Awake"/>
	protected void OnDestroy() {
		lock (lockObj) {
			_ = existingFSMs.Remove(this);

			if (existingFSMs.Count == 0) {
				UnhookPMFSMEvents();
			}
		}
	}

	/// <inheritdoc cref="Awake"/>
	/// <remarks>
	/// You CAN omit the call to this and call <see cref="Iterate()"/> in
	/// FixedUpdate to make the fsm to run regardless of frame rate
	/// and time scale. There is no virtual property to accomplish this
	/// for performance reason.
	/// </remarks>
	protected void Update() => Iterate();
}
