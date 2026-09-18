using System;
using System.IO;
using Mono.Cecil;

// 从程序集中提取全部嵌入资源
// 用法: ExtractRes.exe <目标DLL> <输出目录> [资源名前缀]
internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("用法: ExtractRes.exe <DLL> <输出目录> [资源名前缀]");
            return 1;
        }

        string dllPath = args[0];
        string outDir = args[1];
        string prefix = args.Length > 2 ? args[2] : null;

        Directory.CreateDirectory(outDir);

        // 只读方式打开：不写入源 DLL 所在目录
        using (var asm = AssemblyDefinition.ReadAssembly(dllPath))
        {
            int count = 0;
            long total = 0;

            foreach (Resource res in asm.MainModule.Resources)
            {
                if (!(res is EmbeddedResource embedded)) continue;

                string name = embedded.Name;
                if (prefix != null && !name.StartsWith(prefix, StringComparison.Ordinal)) continue;

                string fileName = prefix != null ? name.Substring(prefix.Length) : name;
                string target = Path.Combine(outDir, fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(target));

                using (Stream src = embedded.GetResourceStream())
                using (var dst = File.Create(target))
                {
                    src.CopyTo(dst);
                    total += dst.Length;
                }

                count++;
                Console.WriteLine($"  {new FileInfo(target).Length,12:N0}  {fileName}");
            }

            Console.WriteLine($"\n共提取 {count} 个资源, 合计 {total / 1048576.0:F1} MB");
        }

        return 0;
    }
}
