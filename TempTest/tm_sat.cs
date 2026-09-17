using System;
using System.Linq;
using System.Reflection;

var path = @"D:\KDQSHH\ModsWork\HollowKnight.ItemChanger-master\ItemChanger\bin\Release\zh\ItemChanger.resources.dll";
var asm = Assembly.LoadFrom(path);
Console.WriteLine($"Assembly: {asm.FullName}");
Console.WriteLine($"Total resources: {asm.GetManifestResourceNames().Length}");
foreach (var n in asm.GetManifestResourceNames().OrderBy(x => x))
    Console.WriteLine(n);
