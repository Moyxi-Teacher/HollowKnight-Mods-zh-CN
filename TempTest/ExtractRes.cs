using System;
using System.IO;
using System.Reflection;
class ExtractRes {
    static void Main(string[] args) {
        var asm = Assembly.LoadFrom(args[0]);
        string[] names = asm.GetManifestResourceNames();
        foreach (var n in names) {
            if (n.Contains("pureamulets") || n.Contains("charmunlock")) {
                using var s = asm.GetManifestResourceStream(n);
                using var f = File.Create(Path.Combine(args[1], Path.GetFileName(n)));
                s.CopyTo(f);
                Console.WriteLine($"extracted {n} len={s.Length}");
            }
        }
    }
}
