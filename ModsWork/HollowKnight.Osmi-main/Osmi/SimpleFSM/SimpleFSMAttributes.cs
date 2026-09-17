namespace Osmi.SimpleFSM;

/// <summary>
/// Marks the method attached to as a state method.
/// A state method must return an <see cref="IEnumerator"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
[PublicAPI]
[MeansImplicitUse]
public sealed class FSMStateAttribute : Attribute {
}

/// <summary>
/// Marks a global transition to the state method attached to.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[PublicAPI]
public sealed class FSMGlobalTransitionAttribute : Attribute {
	public string OnEvent { get; private init; }

	/// <param name="onEvent">The name of the event</param>
	public FSMGlobalTransitionAttribute(string onEvent) =>
		OnEvent = onEvent;
}

/// <summary>
/// Marks a transition from the state method attached to.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[PublicAPI]
public sealed class FSMTransitionAttribute : Attribute {
	public string OnEvent { get; private init; }

	public string ToState { get; private init; }

	/// <param name="onEvent">The name of the event that triggers this transition</param>
	/// <param name="toState">The name of the state to transit to</param>
	public FSMTransitionAttribute(string onEvent, string toState) {
		OnEvent = onEvent;
		ToState = toState;
	}
}
