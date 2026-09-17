using MonoMod.Utils;

using Osmi.ChildRefs;

namespace Osmi.SimpleFSM;

/// <summary>
/// Information about a <see cref="SimpleFSM"/> class.
/// </summary>
[PublicAPI]
public sealed class SimpleFSMInfo {
	internal static readonly LazyMap<Type, SimpleFSMInfo> map = new(New);

	private static SimpleFSMInfo New(Type t) => new(t);

	private SimpleFSMInfo(Type t) {
		ChildRef.Precompile(t);
		float startTime = Time.realtimeSinceStartup;

		if (t == typeof(SimpleFSM)) {
			throw new InvalidOperationException("Cannot compile SimpleFSM base class");
		}

		if (t.IsAbstract) {
			throw new InvalidOperationException($"Class {t.FullName} is abstract");
		}

		if (!t.IsSubclassOf(typeof(SimpleFSM))) {
			throw new InvalidOperationException($"Class {t.FullName} is not {nameof(SimpleFSM)}");
		}

		List<Exception> exceptions = new();


		MethodInfo[] stateMethods = t
			.GetRuntimeMethods()
			.Filter(m => Attribute.IsDefined(m, typeof(FSMStateAttribute)))
			.Filter(m => {
				if (!typeof(IEnumerator).IsAssignableFrom(m.ReturnType)) {
					exceptions.Add(new InvalidOperationException(
						$"Method {m.Name} in SimpleFSM class {t.FullName} is marked" +
						$" as state method but does not return an {nameof(IEnumerator)}"
					));
					return false;
				}

				return true;
			})
			.ToArray();


		states = stateMethods.ToDictionary(
			m => m.Name,
			m => m.GetFastDelegate()
		);

		foreach (MethodInfo mi in stateMethods) {
			string name = mi.Name;

			foreach (FSMGlobalTransitionAttribute attr in mi.GetCustomAttributes<FSMGlobalTransitionAttribute>()) {
				if (!globalTransitions.TryAdd(attr.OnEvent, name)) {
					exceptions.Add(new InvalidOperationException(
						  $"Duplicate global transition on event {attr.OnEvent}, existed on state "
						  + $"{globalTransitions[attr.OnEvent]}, but found another one on state {name}"
					));
				}
			}

			Dictionary<string, string> currentTransitions = new();
			transitions[name] = currentTransitions;
			foreach (FSMTransitionAttribute attr in mi.GetCustomAttributes<FSMTransitionAttribute>()) {
				if (!states.Keys.Contains(attr.ToState)) {
					exceptions.Add(new InvalidOperationException(
						$"Invalid state name {attr.ToState} on event {attr.OnEvent} found on state {mi.Name}"
					));
				} else if (!currentTransitions.TryAdd(attr.OnEvent, attr.ToState)) {
					exceptions.Add(new InvalidOperationException(
						  $"Duplicate transition on event {attr.OnEvent} to {attr.ToState} found on "
						  + $"state {mi.Name}, one to {currentTransitions[attr.OnEvent]} already exists"
					));
				}
			}
		}


		if (exceptions.Count == 1) {
			throw exceptions[0];
		} else if (exceptions.Count > 1) {
			throw new AggregateException(exceptions);
		}

		LogDebug($"[SimpleFSM] Compiling {t.FullName} took {Time.realtimeSinceStartup - startTime}s");
	}

	private readonly Dictionary<string, FastReflectionDelegate> states;

	private readonly Dictionary<string, string> globalTransitions = new();

	private readonly Dictionary<string, IReadOnlyDictionary<string, string>> transitions = new();

	internal IReadOnlyDictionary<string, FastReflectionDelegate> States => states;


	/// <summary>
	/// Names of the states of the <see cref="SimpleFSM"/> class.
	/// </summary>
	public IReadOnlyCollection<string> StateNames => states.Keys;

	/// <summary>
	/// All global trasitions defined in the <see cref="SimpleFSM"/> class.
	/// </summary>
	public IReadOnlyDictionary<string, string> GlobalTransitions => globalTransitions;

	/// <summary>
	/// Transitions defined between states of the <see cref="SimpleFSM"/> class.
	/// </summary>
	public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Transitions => transitions;
}
