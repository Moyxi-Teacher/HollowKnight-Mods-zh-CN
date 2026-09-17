using System;
using System.Linq;
using System.Reflection;

var path = @"D:\KDQSHH\ModsWork\HollowKnight.ItemChanger-master\ItemChanger\bin\Release\ItemChanger.dll";
var asm = Assembly.LoadFrom(path);
var names = asm.GetManifestResourceNames();
foreach (var n in names.Where(x => x.ToLower().Contains("language")))
    Console.WriteLine(n);
