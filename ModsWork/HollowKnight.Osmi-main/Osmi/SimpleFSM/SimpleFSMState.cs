namespace Osmi.SimpleFSM;

/// <summary>
/// Execution state of a <see cref="SimpleFSM"/>.
/// </summary>
[PublicAPI]
public enum SimpleFSMState {
	/// <summary>
	/// The fsm is running.
	/// </summary>
	Running,
	/// <summary>
	/// The fsm is waiting for a coroutine to finish.
	/// </summary>
	Waiting,
	/// <summary>
	/// The fsm is suspended as its current state has finished execution.
	/// </summary>
	Suspended,
	/// <summary>
	/// The fsm has been disabled.
	/// </summary>
	Disabled
}
