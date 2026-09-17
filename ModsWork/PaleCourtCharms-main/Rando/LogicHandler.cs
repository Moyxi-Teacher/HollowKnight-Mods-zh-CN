using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using RandomizerCore.Json;
using RandomizerCore.Logic;
using RandomizerMod.Logging;
using RandomizerMod.RC;
using RandomizerMod.Settings;

namespace PaleCourtCharms.Rando
{
    internal static class LogicHandler
    {
        internal static void Hook()
        {
            RCData.RuntimeLogicOverride.Subscribe(0f, ApplyLogic);
            SettingsLog.AfterLogSettings += AddConnectionSettings;
        }

        private static void ApplyLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!PaleCourtCharms.GlobalSettings.AddCharms)
                return;

            Assembly asm = Assembly.GetExecutingAssembly();
            JsonLogicFormat fmt = new();

            using (var s = asm.GetManifestResourceStream("PaleCourtCharms.Rando.Terms.json"))
                lmb.DeserializeFile(LogicFileType.Terms, fmt, s);

            using (var s = asm.GetManifestResourceStream("PaleCourtCharms.Rando.Locations.json"))
                lmb.DeserializeFile(LogicFileType.Locations, fmt, s);

            using (var s = asm.GetManifestResourceStream("PaleCourtCharms.Rando.Items.json"))
                lmb.DeserializeFile(LogicFileType.ItemStrings, fmt, s);

            // using (var s = asm.GetManifestResourceStream("PaleCourtCharms.Rando.LogicMacros.json"))
            //     lmb.DeserializeFile(LogicFileType.MacroEdit, fmt, s);

            // using (var s = asm.GetManifestResourceStream("PaleCourtCharms.Rando.LogicWaypoints.json"))
            //     lmb.DeserializeFile(LogicFileType.Waypoints, fmt, s);

            // using (var s = asm.GetManifestResourceStream("PaleCourtCharms.Rando.ConnectionLogicPatches.json"))
            //     lmb.DeserializeFile(LogicFileType.LogicEdit, fmt, s);
        }
        //Log to settings.txt
        private static void AddConnectionSettings(LogArguments args, TextWriter tw)
        {
            if (!PaleCourtCharms.GlobalSettings.AddCharms && !PaleCourtCharms.GlobalSettings.RandomizeCosts)
                return;

            tw.WriteLine("Logging Pale Court Charms Settings:");
            using JsonTextWriter jtw = new(tw) { CloseOutput = false };
            RandomizerMod.RandomizerData.JsonUtil._js.Serialize(jtw, PaleCourtCharms.GlobalSettings);
            tw.WriteLine();
        }
    }
}
 
