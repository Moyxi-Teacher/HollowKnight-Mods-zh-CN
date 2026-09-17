using Mono.Cecil;
using Mono.Cecil.Cil;

using MonoMod.Cil;
using MonoMod.Utils;

namespace Osmi.Utils;

[PublicAPI]
public static class ILUtil {
	public static ILCursor GotoPrev(this ILCursor self, int count) =>
		self.Goto(self.Index - count);

	public static ILCursor GotoNext(this ILCursor self, int count) =>
		self.Goto(self.Index + count);

	public static ILCursor GotoEnd(this ILCursor self) =>
		self.Goto(self.Instrs.Count - 1);

	public static bool Match(this ILCursor self, params Func<Instruction, bool>[] predicates) {
		Instruction inst = self.Next;
		for (int i = 0; i < predicates.Length; i++) {
			if (inst == null || !predicates[i].Invoke(inst)) {
				return false;
			}

			inst = inst.Next;
		}

		return true;
	}

	public static ILCursor RemoveUntil(this ILCursor self, params Func<Instruction, bool>[] predicates) {
		while (!self.Match(predicates)) {
			_ = self.Remove();
		}

		return self;
	}

	public static ILCursor RemoveUntilEnd(this ILCursor self) =>
		self.RemoveRange(self.Instrs.Count - self.Index - 1);

	public static ILCursor RemoveAll(this ILCursor self) =>
		self.Goto(0).RemoveUntilEnd();

	public static MethodInfo MethodOf<T>(T method) where T : Delegate =>
		method.GetInvocationList().Length != 1
			? throw new ArgumentException(nameof(method), "Must be a single method")
			: method.Method ?? throw new ArgumentException(nameof(method), "No corresponding MethodInfo");

	public static ILCursor EmitStaticMethodCall<T>(this ILCursor self, T method) where T : Delegate =>
		self.Emit(OpCodes.Call, MethodOf(method));

	public static ILCursor MarkLabel(this ILCursor self, out ILLabel label) {
		label = self.MarkLabel();
		return self;
	}



	public static ILCursor RemoveAll(this ILContext self) =>
		new ILCursor(self).RemoveAll();

	public static ILContext ClearExceptionHandlers(this ILContext self) {
		self.IL.Body.ExceptionHandlers.Clear();
		return self;
	}


	public static DynamicMethodDefinition CreateDynamicMethodDefinition<T>(string name) where T : Delegate {
		MethodInfo invokeMethod = ReflectionHelper.GetMethodInfo(typeof(T), "Invoke");
		return new(
			name,
			invokeMethod.ReturnType,
			invokeMethod.GetParameters().Map(pi => pi.ParameterType).ToArray()
		);
	}

	public static T CreateDynamicMethod<T>(string name, Action<ILProcessor> manip) where T : Delegate {
		DynamicMethodDefinition dmd = CreateDynamicMethodDefinition<T>(name);
		manip.Invoke(dmd.GetILProcessor());
		return dmd.Generate().CreateDelegate<T>();
	}


	public static Func<T> GetFastStaticGetter<T>(this FieldInfo fi) =>
		() => ReflectionHelper.GetField<T>(fi.DeclaringType, fi.Name);

	public static Action<T> GetFastStaticSetter<T>(this FieldInfo fi) =>
		(value) => ReflectionHelper.SetField(fi.DeclaringType, fi.Name, value);

	public static (Func<T> getter, Action<T> setter) GetFastStaticAccessors<T>(this FieldInfo fi) =>
		(fi.GetFastStaticGetter<T>(), fi.GetFastStaticSetter<T>());

	public static Func<TType, TField> GetFastInstanceGetter<TType, TField>(this FieldInfo fi) =>
		(obj) => ReflectionHelper.GetField<TType, TField>(obj, fi.Name);

	public static Action<TType, TField> GetFastInstanceSetter<TType, TField>(this FieldInfo fi) =>
		(obj, value) => ReflectionHelper.SetField(obj, fi.Name, value);

	public static (Func<TType, TField> getter, Action<TType, TField> setter) GetFastInstanceAccessors<TType, TField>(this FieldInfo fi) =>
		(fi.GetFastInstanceGetter<TType, TField>(), fi.GetFastInstanceSetter<TType, TField>());
}
