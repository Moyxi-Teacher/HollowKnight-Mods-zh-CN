using Mono.Cecil.Cil;

using MonoMod.Utils;

namespace Osmi.ChildRefs;

[PublicAPI]
public static class ChildRef {
	internal static readonly LazyMap<Type, Action<Component>> map = new(Compile);

	private static readonly MethodInfo componentGetComponent = Info.OfMethod<Component>(nameof(Component.GetComponent));
	private static readonly MethodInfo componentTfGetter = Info.OfPropertyGet<Component>(nameof(Component.transform));
	private static readonly MethodInfo componentGOGetter = Info.OfPropertyGet<Component>(nameof(Component.gameObject));
	private static readonly MethodInfo tfFind = Info.OfMethod<Transform>(nameof(Transform.Find));

	private static readonly LazyMap<Type, MethodInfo> genericComponentGetComponent =
		new(t => componentGetComponent.MakeGenericMethod(t));


	public static void Precompile<T>() where T : Component => _ = map[typeof(T)];

	public static void Precompile(Type t) => _ = map[t];


	private static Action<Component> Compile(Type t) {
		float startTime = Time.realtimeSinceStartup;

		if (t.IsAbstract) {
			throw new InvalidOperationException($"Class {t.FullName} is abstract");
		}

		if (!t.IsSubclassOf(typeof(Component))) {
			throw new NotSupportedException($"Class {t.FullName} is not {nameof(Component)}");
		}

		List<Exception> exceptions = new();


		Action<Component> @delegate = ILUtil.CreateDynamicMethod<Action<Component>>("<ChildRefsInit>__" + t.FullName.Replace('.', '_'), ilp => {
			foreach (FieldInfo fi in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
				if (fi.GetCustomAttribute<ChildRefAttribute>() is not ChildRefAttribute attr) {
					continue;
				}

				string path = attr.Path;
				Type type = fi.FieldType;

				if (type != typeof(GameObject) && !type.IsSubclassOf(typeof(Component))) {
					exceptions.Add(new InvalidOperationException(
						  $"Type {type.FullName} is neither GameObject nor Component"
					));
					continue;
				}

				if (path.Length == 0) {
					if (type == typeof(GameObject)) {
						exceptions.Add(new InvalidOperationException(
							  "Do not use Child Ref to get the GameObject of its own"
						));
						continue;
					}

					if (type == typeof(Transform)) {
						exceptions.Add(new InvalidOperationException(
							  "Do not use Child Ref to get the Transform of its own"
						));
						continue;
					}
				}


				ilp.Emit(OpCodes.Ldarg_0);
				ilp.Emit(OpCodes.Ldarg_0);

				if (path.Length == 0) {
					ilp.Emit(OpCodes.Call, genericComponentGetComponent[type]);
				} else {
					ilp.Emit(OpCodes.Call, componentTfGetter);
					ilp.Emit(OpCodes.Ldstr, path);
					ilp.Emit(OpCodes.Call, tfFind);

					if (type == typeof(GameObject)) {
						ilp.Emit(OpCodes.Call, componentGOGetter);
					} else if (type == typeof(Transform)) {
						// Sets with transform itself
					} else {
						ilp.Emit(OpCodes.Call, genericComponentGetComponent[type]);
					}
				}

				ilp.Emit(OpCodes.Stfld, fi);
			}

			ilp.Emit(OpCodes.Ret);
		});

		if (exceptions.Count == 1) {
			throw exceptions[0];
		} else if (exceptions.Count > 1) {
			throw new AggregateException(exceptions);
		}

		LogDebug($"[ChildRefs] Compiling {t.FullName} took {Time.realtimeSinceStartup - startTime}s");

		return @delegate;
	}
}
