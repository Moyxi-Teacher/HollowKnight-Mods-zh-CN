using Mono.Cecil.Cil;

using MonoMod.Utils;

namespace Osmi.Utils;

[PublicAPI]
public static class CloneUtil {
	private static readonly LazyMap<Type, Func<object, object>> clonerMap = new(
		(t) => ILUtil.CreateDynamicMethod<Func<object, object>>("<OsmiCloneUtil>__" + t.FullName.Replace('.', '_'), (ilp) => {
			ilp.Body.Variables.Add(new VariableDefinition(ilp.Import(t)));

			ilp.Emit(OpCodes.Newobj, t.GetConstructor(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
				null,
				Type.EmptyTypes,
				null
			) ?? throw new NotSupportedException("Only supports type with parameterless constructor"));
			ilp.Emit(OpCodes.Stloc_0);

			foreach (FieldInfo fi in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
				ilp.Emit(OpCodes.Ldloc_0);
				ilp.Emit(OpCodes.Ldarg_0);
				ilp.Emit(OpCodes.Ldfld, fi);
				ilp.Emit(OpCodes.Stfld, fi);
			}

			ilp.Emit(OpCodes.Ldloc_0);
			ilp.Emit(OpCodes.Ret);
		})
	);

	/// <summary>
	/// Generic and IL version of <see cref="object.MemberwiseClone"/>.
	/// Only supports types with parameterless constructor (regardless of
	/// visibility).
	/// </summary>
	/// <typeparam name="T">Source type and return value type</typeparam>
	/// <param name="source">Source to be cloned from</param>
	/// <returns>
	/// New object cloned from <paramref name="source"/>.
	/// Its type is same as <paramref name="source"/> regardless of <typeparamref name="T"/>.
	/// </returns>
	public static T CreateMemberwiseClone<T>(T source) where T : class =>
		(clonerMap[source.GetType()].Invoke(source) as T)!;
}
