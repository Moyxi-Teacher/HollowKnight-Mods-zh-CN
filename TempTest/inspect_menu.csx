using System;
using System.Reflection;
var asm = Assembly.LoadFrom(@"D:\KDQSHH\ModsWork\HkRefs\Modding.dll");
var type = asm.GetType("Modding.IMenuMod+MenuEntry") ?? asm.GetTypes().FirstOrDefault(t => t.Name.Contains("MenuEntry"));
if (type == null) { Console.WriteLine("类型未找到"); return; }
Console.WriteLine($"类型：{type.FullName}");
Console.WriteLine("--- 字段 ---");
foreach (var f in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
    Console.WriteLine($"{f.FieldType.Name} {f.Name}");
Console.WriteLine("--- 属性 ---");
foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
    Console.WriteLine($"{p.PropertyType.Name} {p.Name}");
