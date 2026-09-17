using MonoMod.Utils;

using Osmi.ChildRefs;
using Osmi.Game;

namespace Osmi.SimpleFSM;

/// <summary>
/// An implementation of finite state machine.
/// <see cref="ChildRef"/> is automatically applied.
/// </summary>
[PublicAPI]
public abstract partial class SimpleFSM {
	/// <summary>
	/// <see cref="System.Type"/> of this instance.
	/// </summary>
	public Type Type { get; private set; } = null!;

	/// <summary>
	/// Name of the <see cref="System.Type"/> of this instance.
	/// </summary>
	public string Name { get; private set; } = null!;

	/// <summary>
	/// <see cref="SimpleFSMInfo"/> of the <see cref="System.Type"/> of this instance.
	/// </summary>
	public SimpleFSMInfo Info => info.Value;

	/// <summary>
	/// Execution state of the fsm.
	/// </summary>
	public SimpleFSMState State { get; private set; } = SimpleFSMState.Disabled;

	/// <summary>
	/// Name of the active state of the fsm.
	/// </summary>
	public string ActiveStateName { get; private set; } = "";

	/// <summary>
	/// Transitions of active state of the fsm.
	/// </summary>
	public IReadOnlyDictionary<string, string> CurrentTransitions { get; private set; }
		= new Dictionary<string, string>();

	private Lazy<SimpleFSMInfo> info = null!;

	private IEnumerator? currentState;


	/// <summary>
	/// <para>
	/// Iterates the fsm for one step.
	/// </para>
	/// <para>
	/// It is safe, while not recommended to use this method, please consider
	/// refactoring your fsm if you think you need this method.
	/// </para>
	/// </summary>
	protected void Iterate() {
		if (State != SimpleFSMState.Running) {
			return;
		}

		if (currentState == null || !currentState.MoveNext()) {
			State = SimpleFSMState.Suspended;
			currentState = null;
			SendEvent(FINISHED);
			return;
		}

		object? current = currentState.Current;
		if (current is string eventName) {
			SendEvent(eventName);
		} else if (current is YieldInstruction inst) {
			WaitFor(inst);
		} else if (current is IEnumerator enmerator) {
			WaitFor(enmerator);
		} else if (current is (string delayedEventName, YieldInstruction delayInst)) {
			SendEventDelayed(delayedEventName, delayInst);
		} else if (current is (string customDelayedEventName, IEnumerator customDelayEnumerator)) {
			SendEventDelayed(customDelayedEventName, customDelayEnumerator);
		}
	}

	/// <summary>
	/// <para>
	/// Iterates the fsm for given steps
	/// </para>
	/// <para>
	/// It is safe, while highly not recommended to use this method, please
	/// consider refactoring your fsm if you think you need this method.
	/// </para>
	/// </summary>
	/// <param name="steps">Steps to iterate</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if step is less or equal than zero</exception>
	protected void Iterate(int steps) {
		if (steps <= 0) {
			throw new ArgumentOutOfRangeException(nameof(steps));
		}

		for (int i = 0; i < steps; i++) {
			Iterate();
		}
	}


	/// <summary>
	/// <para>
	/// Force set the current state of the fsm.
	/// </para>
	/// <para>
	/// It is safe, while not recommended to use this method. If you think you
	/// need this method, please consider refactoring your fsm, unless you need
	/// the additional arguments functionality.
	/// </para>
	/// </summary>
	/// <param name="stateName">The name of the state to set</param>
	/// <param name="args">Additional arguments to pass to the state method</param>
	/// <exception cref="ArgumentException">
	/// Thrown when there is not a state with given name
	/// </exception>
	/// <exception cref="Exception">
	/// Thrown when arguments passed in does not fit with the state method
	/// </exception>
	public void SetState(string stateName, params object[] args) {
		if (!Info.States.TryGetValue(stateName, out FastReflectionDelegate stateMethod)) {
			throw new ArgumentException($"Invalid state name {stateName}");
		}

		if (State == SimpleFSMState.Running || State == SimpleFSMState.Waiting) {
			State = SimpleFSMState.Suspended;
		}

		if (waitCoro != null) {
			GlobalCoroutineExecutor.Stop(waitCoro);
			waitCoro = null;
		}

		currentState = stateMethod.Invoke(this, args) as IEnumerator;
		CurrentTransitions = Info.Transitions[stateName];
		ActiveStateName = stateName;

		if (State == SimpleFSMState.Suspended) {
			State = SimpleFSMState.Running;
		}
	}

	/// <summary>
	/// Set to <see cref="Init"/> state.
	/// </summary>
	public void Reset() => SetState(nameof(Init));
}
