using System;
using System.Linq;
using Mono.Cecil;

public class InspectAssembly {
    public static void Main(string[] args) {
        string path = args[0];
        AssemblyDefinition asm = AssemblyDefinition.ReadAssembly(path);
        Console.WriteLine("ASSEMBLY: " + asm.MainModule.AssemblyReferences.Select(r => r.Name).OrderBy(x => x).Aggregate((a,b) => a + " | " + b));
        foreach (var t in asm.MainModule.Types.Where(t => t.Name == "FullSettingsMod" || t.Name == "IMenuMod" || t.Name == "MenuUtils" || t.Name == "MenuEntry")) {
            Console.WriteLine("\nTYPE: " + t.FullName);
            foreach (var m in t.Methods) {
                Console.WriteLine("  METHOD " + m.Name + "(" + string.Join(", ", m.Parameters.Select(p => p.ParameterType.FullName).ToArray()) + ") -> " + m.ReturnType.FullName + " public=" + m.IsPublic + " static=" + m.IsStatic);
            }
            foreach (var p in t.Properties) {
                Console.WriteLine("  PROP " + p.PropertyType.FullName + " " + p.Name + " get=" + (p.GetMethod != null) + " set=" + (p.SetMethod != null));
            }
        }
    }
}
