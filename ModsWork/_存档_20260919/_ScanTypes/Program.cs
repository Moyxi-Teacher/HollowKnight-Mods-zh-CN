using System;
using Mono.Cecil;

internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            using (var asm = AssemblyDefinition.ReadAssembly(args[0]))
            {
                bool menu = false, global = false, local = false;
                foreach (var t in asm.MainModule.GetTypes())
                {
                    if (!t.IsClass || t.IsInterface) continue;
                    foreach (var i in t.Interfaces)
                    {
                        string n = i.InterfaceType.Name;
                        if (n.StartsWith("IMenuMod") || n.StartsWith("ICustomMenuMod")) menu = true;
                        else if (n.StartsWith("IGlobalSettings")) global = true;
                        else if (n.StartsWith("ILocalSettings")) local = true;
                    }
                }
                Console.WriteLine((menu ? "MENU" : "-") + "|" + (global ? "GLOBAL" : "-") + "|" + (local ? "LOCAL" : "-"));
            }
        }
        catch (Exception e) { Console.WriteLine("ERR|" + e.Message + "|"); }
        return 0;
    }
}
