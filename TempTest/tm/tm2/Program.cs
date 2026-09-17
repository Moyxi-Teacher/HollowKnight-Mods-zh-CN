using System;
using System.Linq;
using System.Reflection;
var path = @"E:\steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods\Pale Court（苍白宫廷）\PaleCourt.dll";
var asm = Assembly.LoadFrom(path);
var res = asm.GetManifestResourceNames();
Console.WriteLine($"Total: {res.Length}");
Console.WriteLine(string.Join("\n", res));
