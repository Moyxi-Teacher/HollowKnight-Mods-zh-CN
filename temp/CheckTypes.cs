using System;
using System.Reflection;

public class CheckTypes
{
    public static void Main(string[] args)
    {
        Assembly asm = Assembly.LoadFrom(args[0]);
        Console.WriteLine("Assembly: " + asm.FullName);
        foreach (Type t in asm.GetTypes())
        {
            if (t.Name.Contains("DontDestroyOnLoad") || t.Name.Contains("MinMaxCurve") || t.Name == "Object")
            {
                Console.WriteLine("Found: " + t.FullName);
            }
        }
    }
}
