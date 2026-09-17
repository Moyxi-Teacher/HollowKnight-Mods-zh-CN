namespace Osmi.SimpleFSM;

public abstract partial class SimpleFSM {
	/// <summary>
	/// Whether the fsm accepts <see cref="FsmEvent"/>.
	/// If returns <see langword="false"/>, sending <see cref="FsmEvent"/>
	/// will become no-op. Defaults to <see langword="false"/>.
	/// </summary>
	protected virtual bool AcceptPMFSMEvents => false;

	/// <summary>
	/// Whether the fsm should restart (reset to <see cref="Init"/> state)
	/// on <see cref="OnEnable"/>. Defaults to <see langword="false"/>.
	/// </summary>
	protected virtual bool RestartOnEnable => false;

	/// <summary>
	/// Whether the child refs should be initialized in <see cref="Awake"/>.
	/// Otherwise, they will be initialized in <see cref="Start"/>.
	/// Defaults to <see langword="false"/>.
	/// </summary>
	protected virtual bool InitChildRefsOnAwake => false;

	/// <summary>
	/// The default state, does nothing.
	/// </summary>
	[FSMState]
	protected virtual IEnumerator Init() => NoOp();

	/// <summary>
	/// Utility method for making states that do nothing.
	/// </summary>
	/// <returns>An enumerator that instantly breaks</returns>
	protected IEnumerator NoOp() {
		yield break;
	}
}
