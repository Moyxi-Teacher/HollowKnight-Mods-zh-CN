using System;
using System.IO;
using Mono.Cecil;

internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length < 2) { Console.WriteLine("用法: ExtractRes.exe <DLL> <输出目录> [前缀]"); return 1; }
        Directory.CreateDirectory(args[1]);
        using (var asm = AssemblyDefinition.ReadAssembly(args[0]))
        {
            int count = 0; long total = 0;
            string prefix = args.Length > 2 ? args[2] : null;
            foreach (Resource res in asm.MainModule.Resources)
            {
                if (!(res is EmbeddedResource er)) continue;
                string name = er.Name;
                if (prefix != null && !name.StartsWith(prefix, StringComparison.Ordinal)) continue;
                string fileName = prefix != null ? name.Substring(prefix.Length) : name;
                string target = Path.Combine(args[1], fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                using (Stream src = er.GetResourceStream())
                using (var dst = File.Create(target)) { src.CopyTo(dst); total += dst.Length; }
                count++;
                Console.WriteLine($"  {new FileInfo(target).Length,12:N0}  {fileName}");
            }
            Console.WriteLine($"\n共 {count} 个资源, {total / 1048576.0:F1} MB");
        }
        return 0;
    }
}
