using System;
using System.Reflection;

class Program
{
    static void Main()
    {
        var asm = Assembly.LoadFrom(@"D:\KDQSHH\ModsWork\HkRefs\Modding.dll");
        foreach (var t in asm.GetTypes())
        {
            if (t.Name.Contains("MenuEntry") || t.Name.Contains("IMenuMod"))
            {
                Console.WriteLine($"=== {t.FullName} ===");
                if (t.IsValueType || t.IsClass)
                {
                    foreach (var f in t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                        Console.WriteLine($"  {f.FieldType.Name} {f.Name}");
                }
            }
        }
    }
}
