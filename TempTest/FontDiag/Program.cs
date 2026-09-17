using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;

class Program
{
    static void Main()
    {
        var asm = Assembly.LoadFrom(@"D:\KDQSHH\ModsWork\HkRefs\Assembly-CSharp.dll");
        var t = asm.GetType("UnityEngine.TextCore.LowLevel.FontEngine");
        if (t == null) { Console.WriteLine("FontEngine type not found"); return; }
        Console.WriteLine("FontEngine type: " + t.FullName);
        foreach (var m in t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
        {
            Console.WriteLine($"  {m.ReturnType.Name} {m.Name}({string.Join(", ", m.GetParameters().Length > 0 ? m.GetParameters()[0].ParameterType.Name : "")})");
        }
    }
}
