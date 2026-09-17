using System.Xml;
using System.Reflection;
using Modding;

namespace ItemChanger.Internal
{
    public static class LanguageStringManager
    {
        private static readonly Dictionary<string, Dictionary<string, string>> LanguageStrings = new();
        private static readonly Dictionary<string, Dictionary<string, string>> ChineseLanguageStrings = new();
        private static bool loaded;

        private static bool IsChinese => Language.Language.CurrentLanguage().ToString() == "ZH";

        internal static void Load()
        {
            if (!loaded)
            {
                Assembly a = typeof(ItemChangerMod).Assembly;
                Stream xmlStream = a.GetManifestResourceStream("ItemChanger.Resources.language.xml");

                // 读取原始英文资源，作为非中文语言的回退文本。
                XmlDocument xml = new();
                xml.Load(xmlStream);
                xmlStream.Dispose();

                XmlNodeList nodes = xml.SelectNodes("Language/entry");
                if (nodes == null)
                {
                    LogWarn("Malformatted language xml, no nodes that match Language/entry");
                    return;
                }

                foreach (XmlNode node in nodes)
                {
                    string? sheet = node.Attributes?["sheet"]?.Value;
                    string? key = node.Attributes?["key"]?.Value;

                    if (sheet == null || key == null)
                    {
                        LogWarn("Malformatted language xml, missing sheet or key on node");
                        continue;
                    }

                    SetString(LanguageStrings, sheet, key, node.InnerText.Replace("\\n", "\n"));
                }

                Stream chineseXmlStream = a.GetManifestResourceStream("ItemChanger.Resources.language.zh.xml");
                if (chineseXmlStream != null)
                {
                    XmlDocument chineseXml = new();
                    chineseXml.Load(chineseXmlStream);
                    chineseXmlStream.Dispose();
                    XmlNodeList? chineseNodes = chineseXml.SelectNodes("Language/entry");
                    if (chineseNodes != null)
                    {
                        foreach (XmlNode node in chineseNodes)
                        {
                            string? sheet = node.Attributes?["sheet"]?.Value;
                            string? key = node.Attributes?["key"]?.Value;
                            if (sheet != null && key != null)
                                SetString(ChineseLanguageStrings, sheet, key, node.InnerText.Replace("\\n", "\n"));
                        }
                    }
                }

                try
                {
                    string localPath = Path.Combine(Path.GetDirectoryName(a.Location), "language.xml");
                    if (File.Exists(localPath))
                    {
                        using (FileStream fs = File.OpenRead(localPath))
                        {
                            xml = new();
                            xml.Load(fs);
                        }
                        nodes = xml.SelectNodes("Language/entry");
                        if (nodes == null)
                        {
                            LogWarn("Malformatted language xml, no nodes that match Language/entry");
                            return;
                        }

                        foreach (XmlNode node in nodes)
                        {
                            string? sheet = node.Attributes?["sheet"]?.Value;
                            string? key = node.Attributes?["key"]?.Value;

                            if (sheet == null || key == null)
                            {
                                LogWarn("Malformatted language xml, missing sheet or key on node");
                                continue;
                            }

                            SetString(LanguageStrings, sheet, key, node.InnerText.Replace("\\n", "\n"));
                        }
                    }
                }
                catch (Exception e)
                {
                    LogError($"Error loading local language.xml:\n{e}");
                }
            }
            loaded = true;
        }

        internal static void Hook()
        {
            ModHooks.LanguageGetHook += GetLanguageString;
        }

        internal static void Unhook()
        {
            ModHooks.LanguageGetHook -= GetLanguageString;
        }

        private static void SetString(Dictionary<string, Dictionary<string, string>> strings, string sheetName, string key, string text)
        {
            if (string.IsNullOrEmpty(sheetName) || string.IsNullOrEmpty(key) || text == null)
            {
                return;
            }

            if (!strings.TryGetValue(sheetName, out Dictionary<string, string> sheet))
            {
                sheet = new Dictionary<string, string>();
                strings.Add(sheetName, sheet);
            }

            sheet[key] = text;
        }

        private static void ResetString(string sheetName, string key)
        {
            if (string.IsNullOrEmpty(sheetName) || string.IsNullOrEmpty(key))
            {
                return;
            }

            if (LanguageStrings.TryGetValue(sheetName, out Dictionary<string, string> sheet) && sheet.ContainsKey(key))
            {
                sheet.Remove(key);
            }
        }

        /// <summary>
        /// Returns the string with the given sheet and key from the languge.xml. Does not do any other search, nor does it invoke Language.Language.Get, nor does it format the result.
        /// Returns empty if the string was not found.
        /// </summary>
        public static string GetICString(string key, string sheetTitle = "IC")
        {
            Dictionary<string, Dictionary<string, string>> strings = IsChinese ? ChineseLanguageStrings : LanguageStrings;
            if (!strings.TryGetValue(sheetTitle, out Dictionary<string, string> sheet) || !sheet.TryGetValue(key, out string value))
            {
                return string.Empty;
            }

            return value;
        }

        // keep this private -- the api hook does weird stuff with GetInternal
        private static string GetLanguageString(string key, string sheetTitle, string orig)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(sheetTitle))
            {
                return string.Empty;
            }

            // 仅中文使用中文资源；其他游戏语言保留原始英文资源。
            Dictionary<string, Dictionary<string, string>> strings = IsChinese ? ChineseLanguageStrings : LanguageStrings;
            if (strings.ContainsKey(sheetTitle) && strings[sheetTitle].ContainsKey(key))
            {
                return strings[sheetTitle][key];
            }

            if (sheetTitle.StartsWith("Internal"))
            {
                return Language.Language.GetInternal(key, sheetTitle[8..]);
            }

            if (sheetTitle == "Exact")
            {
                return key;
            }

            if (key.StartsWith("ITEMCHANGER_NAME_ESSENCE_"))
            {
                return string.Format(Language.Language.Get("ESSENCE", "Fmt"), key["ITEMCHANGER_NAME_ESSENCE_".Length..]);
            }

            if (key.StartsWith("ITEMCHANGER_NAME_GEO_"))
            {
                return string.Format(Language.Language.Get("GEO", "Fmt"), key["ITEMCHANGER_NAME_GEO_".Length..]);
            }

            if (key == "ITEMCHANGER_POSTVIEW_GRUB")
            {
                return string.Format(Language.Language.Get("GRUB", "Fmt"), PlayerData.instance.GetInt(nameof(PlayerData.grubsCollected)));
            }

            if (key == "ITEMCHANGER_POSTVIEW_GRIMMKIN_FLAME")
            {
                int flames = PlayerData.instance.GetInt(nameof(Modules.GrimmkinFlameManager.cumulativeFlamesCollected));
                if (flames <= 0) flames = PlayerData.instance.GetInt(nameof(PlayerData.flamesCollected));
                return string.Format(Language.Language.Get("FLAME", "Fmt"), flames);
            }

            return orig;
        }
    }
}
