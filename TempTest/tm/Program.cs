using System;
using System.Linq;
using System.Reflection;
var path = @"D:\KDQSHH\Mods\Pale Court（苍白宫廷）\PaleCourt.dll";
var asm = Assembly.LoadFrom(path);
var res = asm.GetManifestResourceNames();
Console.WriteLine($"Total: {res.Length}");
Console.WriteLine("FiveKnights.StreamingAssets entries:");
foreach (var n in res.Where(x => x.StartsWith("FiveKnights.StreamingAssets", StringComparison.OrdinalIgnoreCase)).OrderBy(x => x))
    Console.WriteLine("  " + n);
Console.WriteLine("Any FiveKnights.*:");
foreach (var n in res.Where(x => x.StartsWith("FiveKnights.", StringComparison.OrdinalIgnoreCase)).OrderBy(x => x).Take(20))
    Console.WriteLine("  " + n);
