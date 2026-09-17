using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

class Program
{
    static void Main(string[] args)
    {
        string modsDir = @"D:\KDQSHH\Mods";
        var dirs = Directory.GetDirectories(modsDir).OrderBy(d => d);

        foreach (var dir in dirs)
        {
            var dlls = Directory.GetFiles(dir, "*.dll").OrderBy(f => f);
            foreach (var dll in dlls)
            {
                try
                {
                    var asm = AssemblyDefinition.ReadAssembly(dll);
                    var name = asm.Name.Name;
                    var folderName = new DirectoryInfo(dir).Name;

                    var mainClass = "";
                    var displayName = "";
                    var getNameOverride = false;
                    string infoVer = "";

                    foreach (var t in asm.MainModule.Types)
                    {
                        if (t.BaseType != null && t.BaseType.FullName == "Modding.Mod")
                        {
                            mainClass = t.Name;
                            foreach (var m in t.Methods)
                            {
                                if (m.Name == "GetName" && !m.HasParameters)
                                {
                                    if (!m.IsAbstract && m.HasBody)
                                    {
                                        getNameOverride = true;
                                        foreach (var i in m.Body.Instructions)
                                        {
                                            if (i.OpCode == OpCodes.Ldstr && i.Operand is string s && !string.IsNullOrEmpty(s) && s.Length < 100)
                                            {
                                                displayName = s;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(displayName) && !string.IsNullOrEmpty(mainClass))
                    {
                        foreach (var t in asm.MainModule.Types)
                        {
                            if (t.BaseType != null && t.BaseType.FullName == "Modding.Mod")
                            {
                                // .ctors in Mono.Cecil: the property is `.ctors`
                                foreach (var c in t.Methods)
                                {
                                    if (c.IsConstructor && c.HasBody)
                                    {
                                        foreach (var i in c.Body.Instructions)
                                        {
                                            if (i.OpCode == OpCodes.Ldstr && i.Operand is string s && !string.IsNullOrEmpty(s) && s.Length < 100 && s != t.Name)
                                            {
                                                displayName = s;
                                                break;
                                            }
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(displayName)) break;
                                }
                                break;
                            }
                        }
                    }

                    foreach (var attr in asm.MainModule.Assembly.CustomAttributes)
                    {
                        if (attr.AttributeType.FullName == "System.Reflection.AssemblyInformationalVersionAttribute" && attr.HasConstructorArguments)
                        {
                            infoVer = attr.ConstructorArguments[0].Value?.ToString() ?? "";
                        }
                    }

                    Console.WriteLine($"[{folderName}] DLL={name} Class={mainClass} 显示名=\"{displayName}\" InfoVer={infoVer} GetName覆写={getNameOverride}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{Path.GetFileName(dir)}] DLL={Path.GetFileName(dll)} 错误={ex.Message}");
                }
            }
        }
    }
}
