namespace Osmi.ChildRefs;

[AttributeUsage(AttributeTargets.Field)]
[PublicAPI]
[MeansImplicitUse]
public sealed class ChildRefAttribute : Attribute {
	public string Path { get; private init; }

	public ChildRefAttribute(string path = "") => Path = path;
}
