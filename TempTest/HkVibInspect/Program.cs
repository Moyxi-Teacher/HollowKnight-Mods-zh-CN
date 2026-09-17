using System;
using System.IO;
using Fmod5Sharp;
using Fmod5Sharp.FmodTypes;
using System.Reflection;

class Program {
    static void Main() {
        var resResourcePath = @"E:\steam\steamapps\common\Hollow Knight\hollow_knight_Data\resources.resource";
        
        byte[] fsbBytes = File.ReadAllBytes(resResourcePath);
        
        // Dump first 128 bytes
        Console.WriteLine("First 128 bytes:");
        for (int i = 0; i < 128; i += 16) {
            Console.Write($"{i:X4}: ");
            for (int j = 0; j < 16; j++) {
                Console.Write($"{fsbBytes[i+j]:X2} ");
            }
            // ASCII
            Console.Write(" | ");
            for (int j = 0; j < 16; j++) {
                byte b = fsbBytes[i+j];
                Console.Write(b >= 32 && b < 127 ? (char)b : '.');
            }
            Console.WriteLine();
        }
        
        try {
            var fsb = FsbLoader.LoadFsbFromByteArray(fsbBytes);
            // Inspect the FmodAudioHeader via reflection
            var headerField = typeof(FmodSoundBank).GetField("Header", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Console.WriteLine($"\nHeader field: {headerField}");
            if (headerField != null) {
                var header = headerField.GetValue(fsb);
                Console.WriteLine($"Header type: {header?.GetType().Name}");
                foreach (var prop in header.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                    try { Console.WriteLine($"  {prop.Name} = {prop.GetValue(header)}"); } catch {}
                }
                foreach (var fld in header.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                    try {
                        var v = fld.GetValue(header);
                        if (fld.Name == "Samples" && v is System.Collections.IEnumerable en) {
                            int c = 0; foreach (var x in en) c++;
                            Console.WriteLine($"  Samples count = {c}");
                        } else {
                            Console.WriteLine($"  {fld.Name} = {v}");
                        }
                    } catch {}
                }
            }
        } catch (Exception ex) {
            Console.WriteLine($"FSB load error: {ex.GetType().Name}: {ex.Message}");
        }
    }
}
