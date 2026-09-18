using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace DoodleBosses
{
    public class DoodleBosses : Mod, IGlobalSettings<Settings>, IMenuMod, ITogglableMod
    {
        internal static DoodleBosses Instance;
        public Settings settings = new();
        public bool ToggleButtonInsideMenu => true;

        // ── 总开关 ────────────────────────────────────────────────────────
        // 本模组的做法是把 boss 材质上的贴图换成涂鸦贴图（material 是共享的，
        // 所以一改就是全局生效）。要能真正关掉，就必须把这些材质原本的贴图
        // 记下来，Unload 时原样还原；只摘掉钩子是不够的。
        private readonly Dictionary<Material, Texture> _originalTextures = new();

        /// <summary>替换贴图前先记住原贴图；重复初始化时不覆盖已记录的值。</summary>
        private void RememberTexture(Material mat)
        {
            if (mat == null) return;
            if (!_originalTextures.ContainsKey(mat)) _originalTextures[mat] = mat.mainTexture;
        }

        /// <summary>总开关：关闭模组时还原贴图并摘掉语言钩子。</summary>
        public void Unload()
        {
            ModHooks.LanguageGetHook -= LanguageGet;

            foreach (var pair in _originalTextures)
            {
                if (pair.Key != null) pair.Key.mainTexture = pair.Value;
            }
            _originalTextures.Clear();

            Log("Unloaded");
        }

        public static readonly Dictionary<string, GameObject> _gameObjects = new();

        private Dictionary<string, ValueTuple<string, string>> _preloads = new()
        {

            [BossStrings[Bosses.GRUZ_MOTHER]] = ("GG_Gruz_Mother", "_Enemies/Giant Fly"),
            [BossStrings[Bosses.VENGEFLY_KING]] = ("GG_Vengefly", "Giant Buzzer Col"),
            [BossStrings[Bosses.BROODING_MAWLEK]] = ("GG_Brooding_Mawlek", "Battle Scene/Mawlek Body"),
            [BossStrings[Bosses.FALSE_KNIGHT]] = ("GG_False_Knight", "Battle Scene/False Knight New"),
            //[BossStrings[Bosses.HORNET]] = ("GG_Hornet_1", "Boss Holder/Hornet Boss 1"),
            [BossStrings[Bosses.MASSIVE_MOSS_CHARGER]] = ("GG_Mega_Moss_Charger", "Mega Moss Charger"),
            [BossStrings[Bosses.FLUKEMARM]] = ("GG_Flukemarm", "Fluke Mother"),
            [BossStrings[Bosses.MANTIS_LORDS]] = ("Fungus2_15_boss", "Mantis Battle/Battle Main/Mantis Lord"),
            [BossStrings[Bosses.OBLOBBLES]] = ("GG_Oblobbles", "Mega Fat Bee"),
            [BossStrings[Bosses.HIVE_KNIGHT]] = ("GG_Hive_Knight", "Battle Scene/Hive Knight"),
            [BossStrings[Bosses.BROKEN_VESSEL]] = ("GG_Broken_Vessel", "Infected Knight"),
            [BossStrings[Bosses.NOSK]] = ("GG_Nosk", "Mimic Spider"),
            [BossStrings[Bosses.WINGED_NOSK]] = ("GG_Nosk_Hornet", "Battle Scene/Hornet Nosk"),
            [BossStrings[Bosses.COLLECTOR]] = ("GG_Collector", "Battle Scene/Jar Collector"),
            [BossStrings[Bosses.GOD_TAMER]] = ("GG_God_Tamer", "Entry Object/Lancer"),
            [BossStrings[Bosses.CRYSTAL_GUARDIAN]] = ("GG_Crystal_Guardian", "Mega Zombie Beam Miner (1)"),
            [BossStrings[Bosses.UUMUU]] = ("GG_Uumuu", "Mega Jellyfish GG"),
            [BossStrings[Bosses.TRAITOR_LORD]] = ("GG_Traitor_Lord", "Battle Scene/Wave 3/Mantis Traitor Lord"),
            [BossStrings[Bosses.GREY_PRINCE_ZOTE]] = ("GG_Grey_Prince_Zote", "Grey Prince"),
            [BossStrings[Bosses.SOUL_WARRIOR]] = ("GG_Mage_Knight", "Mage Knight"),
            [BossStrings[Bosses.SOUL_MASTER]] = ("GG_Soul_Master", "Mage Lord"),
            [BossStrings[Bosses.DUNG_DEFENDER]] = ("GG_Dung_Defender", "Dung Defender"),
            [BossStrings[Bosses.WHITE_DEFENDER]] = ("GG_White_Defender", "White Defender"),
            [BossStrings[Bosses.WATCHER_KNIGHT]] = ("GG_Watcher_Knights", "Battle Control/Black Knight 1"),
            [BossStrings[Bosses.NO_EYES]] = ("GG_Ghost_No_Eyes", "Warrior/Ghost Warrior No Eyes"),
            [BossStrings[Bosses.MARMU]] = ("GG_Ghost_Marmu", "Warrior/Ghost Warrior Marmu"),
            [BossStrings[Bosses.GALIEN]] = ("GG_Ghost_Galien", "Warrior/Ghost Warrior Galien"),
            [BossStrings[Bosses.MARKOTH]] = ("GG_Ghost_Markoth", "Warrior/Ghost Warrior Markoth"),
            [BossStrings[Bosses.XERO]] = ("GG_Ghost_Xero", "Warrior/Ghost Warrior Xero"),
            [BossStrings[Bosses.GORB]] = ("GG_Ghost_Gorb", "Warrior/Ghost Warrior Slug"),
            [BossStrings[Bosses.ELDER_HU]] = ("GG_Ghost_Hu", "Warrior/Ghost Warrior Hu"),
            [BossStrings[Bosses.ORO_MATO]] = ("GG_Nailmasters", "Brothers/Oro"),
            [BossStrings[Bosses.PAINTMASTER_SHEO]] = ("GG_Painter", "Battle Scene/Sheo Boss"),
            [BossStrings[Bosses.GREAT_NAILSAGE_SLY]] = ("GG_Sly", "Battle Scene/Sly Boss"),
            [BossStrings[Bosses.PURE_VESSEL]] = ("GG_Hollow_Knight", "Battle Scene/HK Prime"),
            [SpecialStrings[Specials.GRIMM1].Item1] = ("GG_Grimm", "Grimm Scene/Grimm Boss"),
            [BossStrings[Bosses.NIGHTMARE_KING]] = ("GG_Grimm_Nightmare", "Grimm Control/Nightmare Grimm Boss"),
            [BossStrings[Bosses.HOLLOW_KNIGHT]] = ("Room_Final_Boss_Core", "Boss Control/Hollow Knight Boss"),
            [SpecialStrings[Specials.RADIANCE1].Item1] = ("GG_Radiance", "Boss Control/Radiance Roar"),//"Boss Control/Absolute Radiance"),
            [BossStrings[Bosses.ZOTE]] = ("GG_Mighty_Zote", "Battle Control/First Zote/Zote Boss"),

        };

        public Textures SpriteDict { get; private set; }

        public static Sprite GetSprite(int key, bool special = false) => Instance.SpriteDict.Get(key, special);

        public override string GetVersion() => "0.9.6-0";

        public override List<ValueTuple<string, string>> GetPreloadNames()
        {
            return _preloads.Values.ToList();
        }

        public DoodleBosses() : base("涂鸦BOSS")
        {
            Instance = this;

            SpriteDict = new Textures();
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            foreach (var (name, (scene, path)) in _preloads)
            {
                _gameObjects[name] = preloadedObjects[scene][path];
            }

            Instance = this;

            foreach (Bosses boss in BossStrings.Keys)
            {
                var spriteDef = _gameObjects[BossStrings[boss]].GetComponent<tk2dSprite>().GetCurrentSpriteDef();
                RememberTexture(spriteDef.material);          // 记下原贴图，供总开关还原
                spriteDef.material.mainTexture = GetSprite((int)boss).texture;
            }

            Log("Finished loading regular bosses");

            foreach (Specials sheet in SpecialStrings.Keys)
            {
                Log("Doing special boss " + SpecialStrings[sheet].Item1);
                var mat = _gameObjects[SpecialStrings[sheet].Item1].GetComponent<tk2dSprite>()
                              .Collection.materials[SpecialStrings[sheet].Item2];
                RememberTexture(mat);                          // 同上
                mat.mainTexture = GetSprite((int)sheet, true).texture;
            }


            //_gameObjects[BossStrings[Bosses.GRUZ_MOTHER]].GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture = GetSprite(Bosses.GRUZ_MOTHER).texture;

            ModHooks.LanguageGetHook += LanguageGet;

            Log("Initialized");
        }

        public void OnLoadGlobal(Settings _settings) => settings = _settings;
        public Settings OnSaveGlobal() => settings;

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? menu)
        {
            List<IMenuMod.MenuEntry> menus = new();

            // 模组总开关：API 会把它作为 menu 参数传进来，必须放进返回列表里才会显示。
            // 不接收它的话，设置菜单里就没有开关（原先正是如此）。
            if (menu != null) menus.Add(menu.Value);

            menus.Add(new()
                {
                    Name = "显示艺术家署名",
                    Description = "若已启用，将覆盖赞助者鸣谢",
                    Values = new string[]
                    {
                        Language.Language.Get("MOH_ON", "MainMenu"),
                        Language.Language.Get("MOH_OFF", "MainMenu"),
                    },
                    Saver = i => settings.creditsOn = i == 0,
                    Loader = () => settings.creditsOn ? 0 : 1
                });
            menus.Add(new()
                {
                    Name = "涂鸦名字",
                    Description = "为所有BOSS添加滑稽的涂鸦名字！",
                    Values = new string[]
                    {
                        Language.Language.Get("MOH_ON", "MainMenu"),
                        Language.Language.Get("MOH_OFF", "MainMenu"),
                    },
                    Saver = i => settings.doodleNamesOn = i == 0,
                    Loader = () => settings.doodleNamesOn ? 0 : 1
                });

            return menus;
        }

        private static string LanguageGet(string key, string sheetTitle, string orig)
        {
            Settings settings = Instance.settings;

            if (!settings.creditsOn && !settings.doodleNamesOn)
                return orig;

            if (Language.Language.CurrentLanguage().ToString() != "ZH")
                return orig;

            switch (key)
            {
                case "BIGFLY_SUPER":
                    if (settings.doodleNamesOn) return "Mother of";
                    return "Gruz";
                case "BIGFLY_MAIN":
                    if (settings.doodleNamesOn) return "Sketches";
                    return "Mother";
                case "BIGFLY_SUB":
                    if (settings.creditsOn) return "by Godfriend";
                    return "";
                case "VENGEFLY_SUPER":
                    if (settings.doodleNamesOn) return "Doodle";
                    return "Vengefly";
                case "VENGEFLY_MAIN":
                    return "King";
                case "VENGEFLY_SUB":
                    if (settings.creditsOn) return "by Godfriend";
                    return "";
                case "MAWLEK_SUPER":
                    if (settings.doodleNamesOn) return "Doodling";
                    return "Brooding";
                case "MAWLEK_MAIN":
                    return "Mawlek";
                case "MAWLEK_SUB":
                    if (settings.creditsOn) return "by Godfriend";
                    return "";
                case "FALSE_KNIGHT_SUPER":
                    return "";
                case "FALSE_KNIGHT_MAIN":
                    if (settings.doodleNamesOn) return "False Doodle";
                    return "False Knight";
                case "FALSE_KNIGHT_SUB":
                    if (settings.creditsOn) return "by MinishLink";
                    return "";
                case "FALSE_KNIGHT_DREAM_SUPER":
                    return "Failed";
                case "FALSE_KNIGHT_DREAM_MAIN":
                    if (settings.doodleNamesOn) return "Masterpiece";
                    return "Champion";
                case "FALSE_KNIGHT_DREAM_SUB":
                    if (settings.creditsOn) return "by MinishLink";
                    return "";
                /*case "HORNET_SUPER":
                    return "";
                case "HORNET_MAIN":
                    return "";
                case "HORNET_SUB":
                    return "";*/
                case "MEGA_MOSS_SUPER":
                    return "Massive";
                case "MEGA_MOSS_MAIN":
                    return "Moss Charger";
                case "MEGA_MOSS_SUB":
                    if (settings.creditsOn) return "by flame-shadow";
                    return "";
                case "FLUKEMARM_SUPER":
                    return "";
                case "FLUKEMARM_MAIN":
                    if (settings.doodleNamesOn) return "Mommy Anime";
                    return "Flukemarm";
                case "FLUKEMARM_SUB":
                    if (settings.creditsOn) return "by Failed Vessel";
                    return "";
                case "MANTIS_LORDS_SUPER":
                    return "Mantis";
                case "MANTIS_LORDS_MAIN":
                    return "Lords";
                case "MANTIS_LORDS_SUB":
                    if (settings.creditsOn) return "by flame-shadow";
                    return "";
                case "SISTERS_SUPER":
                    if (settings.creditsOn) return "Sisters";
                    return "";
                case "SISTERS_MAIN":
                    if (settings.creditsOn) return "Of Battle";
                    return "Sisters";
                case "SISTERS_SUB":
                    if (settings.creditsOn) return "by flame-shadow";
                    return "Of Battle";
                case "OBLOBBLES_SUPER":
                    return "";
                case "OBLOBBLES_MAIN":
                    if (settings.doodleNamesOn) return "Doodlobles";
                    return "Oblobbles";
                case "OBLOBBLES_SUB":
                    if (settings.creditsOn) return "by Jex111";
                    return "";
                case "HIVE_KNIGHT_SUPER":
                    if (settings.creditsOn) return "Hive";
                    return "";
                case "HIVE_KNIGHT_MAIN":
                    if (settings.creditsOn) return "Knight";
                    return "Hive";
                case "HIVE_KNIGHT_SUB":
                    if (settings.creditsOn) return "by MTmerm";
                    return "Knight";
                case "INFECTED_KNIGHT_SUPER":
                    if (settings.doodleNamesOn) return "Watercolor";
                    return "Broken";
                case "INFECTED_KNIGHT_MAIN":
                    return "Vessel";
                case "INFECTED_KNIGHT_SUB":
                    if (settings.creditsOn) return "by Akivaq";
                    return "";
                case "INFECTED_KNIGHT_DREAM_SUPER":
                    if (settings.doodleNamesOn) return "Watercolor";
                    return "Lost";
                case "INFECTED_KNIGHT_DREAM_MAIN":
                    return "Kin";
                case "INFECTED_KNIGHT_DREAM_SUB":
                    if (settings.creditsOn) return "by Akivaq";
                    return "";
                case "MIMIC_SPIDER_SUPER":
                    if (settings.doodleNamesOn) return "Sketch";
                    return "";
                case "MIMIC_SPIDER_MAIN":
                    return "Nosk";
                case "MIMIC_SPIDER_SUB":
                    if (settings.creditsOn) return "by MinishLink";
                    return "";
                case "COLLECTOR_SUPER":
                    if (settings.doodleNamesOn) return "Art";
                    return "The";
                case "COLLECTOR_MAIN":
                    return "Collector";
                case "COLLECTOR_SUB":
                    if (settings.creditsOn) return "by KaziVuri";
                    return "";
                case "LOBSTER_LANCER_NC_SUPER":
                    if (!settings.doodleNamesOn) return "God";
                    if (!settings.creditsOn) return "";
                    return "Sketch";
                case "LOBSTER_LANCER_C_SUPER":
                    if (settings.doodleNamesOn) return "Sketch";
                    return "God";
                case "LOBSTER_LANCER_NC_MAIN":
                    if (settings.creditsOn) return "Tamer";
                    return "Sketch";
                case "LOBSTER_LANCER_C_MAIN":
                    return "Tamer";
                case "LOBSTER_LANCER_NC_SUB":
                    if (settings.creditsOn) return "by Godfriend";
                    return "Tamer";
                case "LOBSTER_LANCER_C_SUB":
                    if (settings.creditsOn) return "by Godfriend";
                    return orig;
                case "CRYSTAL_GUARDIAN_SUPER":
                    if (!settings.creditsOn) return "";
                    if (!settings.doodleNamesOn) return "Crystal";
                    return "Sketchy";
                case "CRYSTAL_GUARDIAN_MAIN":
                    if (!settings.creditsOn) return "Sketchy";
                    if (!settings.doodleNamesOn) return "Guardian";
                    return "CG";
                case "CRYSTAL_GUARDIAN_SUB":
                    if (settings.creditsOn) return "by DaveyTheDuck";
                    return "CG";
                case "ENRAGED_GUARDIAN_SUPER":
                    if (settings.doodleNamesOn) return "Sketchy";
                    return "Enraged";
                case "ENRAGED_GUARDIAN_MAIN":
                    if (settings.doodleNamesOn) return "EG";
                    return "Guardian";
                case "ENRAGED_GUARDIAN_SUB":
                    if (settings.creditsOn) return "by DaveyTheDuck";
                    return "";
                case "MEGA_JELLY_SUPER":
                    if (settings.doodleNamesOn) return "Sketch";
                    return "";
                case "MEGA_JELLY_MAIN":
                    if (settings.doodleNamesOn) return "Uuwuu";
                    return "Uumuu";
                case "MEGA_JELLY_SUB":
                    if (settings.creditsOn) return "by AceiestArtist";
                    return "";
                case "TRAITOR_LORD_SUPER":
                    if (settings.doodleNamesOn) return "Doodle";
                    return "Traitor";
                case "TRAITOR_LORD_MAIN":
                    if (settings.doodleNamesOn) return "Traitor";
                    return "Lord";
                case "TRAITOR_LORD_SUB":
                    if (settings.creditsOn) return "by MTmerm";
                    return "";
                // GPZ
                case "MAGE_KNIGHT_SUPER":
                    if (settings.doodleNamesOn) return "Soul Warrior";
                    return "Soul";
                case "MAGE_KNIGHT_MAIN":
                    if (settings.doodleNamesOn) return "Lite";
                    return "Warrior";
                case "MAGE_KNIGHT_SUB":
                    if (settings.creditsOn) return "by Akivaq";
                    return "";
                case "MAGE_LORD_SUPER":
                    if (settings.doodleNamesOn) return "Doodle";
                    return "Soul";
                case "MAGE_LORD_MAIN":
                    return "Master";
                case "MAGE_LORD_SUB":
                    if (settings.creditsOn) return "by Chaktis";
                    return "";
                case "MAGE_LORD_DREAM_SUPER":
                    if (settings.doodleNamesOn) return "Doodle";
                    return "Soul";
                case "MAGE_LORD_DREAM_MAIN":
                    return "Tyrant";
                case "MAGE_LORD_DREAM_SUB":
                    if (settings.creditsOn) return "by Chaktis";
                    return "";
                case "DUNG_DEFENDER_SUPER":
                    if (!settings.creditsOn) return "";
                    if (!settings.doodleNamesOn) return "Dung";
                    return "Doodle";
                case "DUNG_DEFENDER_MAIN":
                    if (settings.creditsOn) return "Defender";
                    return "Doodle";
                case "DUNG_DEFENDER_SUB":
                    if (settings.creditsOn) return "by flame-shadow";
                    return "Defender";
                case "WHITE_DEFENDER_SUPER":
                    if (!settings.creditsOn) return "";
                    if (!settings.doodleNamesOn) return "White";
                    return "Wite-Out";
                case "WHITE_DEFENDER_MAIN":
                    if (settings.creditsOn) return "Defender";
                    return "Wite-Out";
                case "WHITE_DEFENDER_SUB":
                    if (settings.creditsOn) return "by RocketFire20";
                    return "Defender";
                case "BLACK_KNIGHT_SUPER":
                    if (settings.doodleNamesOn) return "Sharpie";
                    return "Watcher";
                case "BLACK_KNIGHT_MAIN":
                    return "Knight";
                case "BLACK_KNIGHT_SUB":
                    if (settings.creditsOn) return "by Dandy";
                    return "";
                case "GH_NOEYES_NC_SUPER":
                case "GH_NOEYES_C_SUPER":
                //    return "";
                case "GH_NOEYES_NC_MAIN":
                case "GH_NOEYES_C_MAIN":
                    return orig;//"No Eyes";
                case "GH_NOEYES_NC_SUB":
                case "GH_NOEYES_C_SUB":
                    if (settings.creditsOn) return "by Torny";
                    return orig;//"";
                case "GH_MUMCAT_NC_SUPER":
                case "GH_MUMCAT_C_SUPER":
                    //return "";
                case "GH_MUMCAT_NC_MAIN":
                case "GH_MUMCAT_C_MAIN":
                    return orig;//"";
                case "GH_MUMCAT_NC_SUB":
                case "GH_MUMCAT_C_SUB":
                    if (settings.creditsOn) return "by flame-shadow";
                    return orig;//"";
                case "GH_GALIEN_NC_SUPER":
                case "GH_GALIEN_C_SUPER":
                    return "";
                case "GH_GALIEN_NC_MAIN":
                case "GH_GALIEN_C_MAIN":
                    if (settings.doodleNamesOn) return "Scrabblien";
                    return "Galien";
                case "GH_GALIEN_NC_SUB":
                case "GH_GALIEN_C_SUB":
                    if (settings.creditsOn) return "by Jex111";
                    return orig;//"";
                case "GH_MARKOTH_NC_SUPER":
                case "GH_MARKOTH_C_SUPER":
                    return "";
                case "GH_MARKOTH_NC_MAIN":
                case "GH_MARKOTH_C_MAIN":
                    if (settings.doodleNamesOn) return "Doodlekoth";
                    return "Markoth";
                case "GH_MARKOTH_NC_SUB":
                case "GH_MARKOTH_C_SUB":
                    if (settings.creditsOn) return "by Chaktis";
                    return orig;//"";
                case "GH_XERO_NC_SUPER":
                case "GH_XERO_C_SUPER":
                    if (settings.doodleNamesOn) return "Watery";
                    return "";
                case "GH_XERO_NC_MAIN":
                case "GH_XERO_C_MAIN":
                    return "Xero";
                case "GH_XERO_NC_SUB":
                case "GH_XERO_C_SUB":
                    if (settings.creditsOn) return "by flame-shadow";
                    return orig;//"";
                case "GH_ALADAR_NC_SUPER":
                case "GH_ALADAR_C_SUPER":
                    if (settings.doodleNamesOn) return "Anime";
                    return "";
                case "GH_ALADAR_NC_MAIN":
                case "GH_ALADAR_C_MAIN":
                    if (settings.doodleNamesOn) return "Protagonist";
                    return "Gorb";
                case "GH_ALADAR_NC_SUB":
                case "GH_ALADAR_C_SUB":
                    if (settings.creditsOn) return "by FailedVessel";
                    return orig;//"";
                case "GH_HU_NC_SUPER":
                case "GH_HU_C_SUPER":
                    return "";
                case "GH_HU_NC_MAIN":
                case "GH_HU_C_MAIN":
                    if (settings.doodleNamesOn) return "Pancake Man";
                    return "Elder Hu";
                case "GH_HU_NC_SUB":
                case "GH_HU_C_SUB":
                    if (settings.creditsOn) return "by RocketFire20";
                    return orig;//"";
                // Oro/Mato
                case "PAINTMASTER_SUPER":
                    if (settings.doodleNamesOn) return "Doodlemaster";
                    return "Paintmaster";
                case "PAINTMASTER_MAIN":
                    return "Sheo";
                case "PAINTMASTER_SUB":
                    if (settings.creditsOn) return "by RocketFire20";
                    return "";
                case "SLY_BOSS_SUPER":
                    if (settings.doodleNamesOn) return "Pencilsage";
                    return "Great Nailsage";
                case "SLY_BOSS_MAIN":
                    return "Sly";
                case "SLY_BOSS_SUB":
                    if (settings.creditsOn) return "by RiverRobot";
                    return "";
                // PV
                case "GRIMM_SUPER":
                    if (settings.doodleNamesOn) return "Crayon Master";
                    return "Troupe Master";
                case "GRIMM_MAIN":
                    return "Grimm";
                case "GRIMM_SUB":
                    if (settings.creditsOn) return "by MinishLink";
                    return "";
                case "NIGHTMARE_GRIMM_SUPER":
                    if (settings.doodleNamesOn) return "Nightmare Crayon";
                    return "Troupe Master";
                case "NIGHTMARE_GRIMM_MAIN":
                    return "Grimm";
                case "NIGHTMARE_GRIMM_SUB":
                    if (settings.creditsOn) return "by MinishLink";
                    return "";
                // AbsRad
                case "ZOTE_SUPER":
                    if (settings.doodleNamesOn) return "Off-hand";
                    return "Zote";
                case "ZOTE_MAIN":
                    if (settings.doodleNamesOn) return "Zote";
                    return "The Mighty";
                case "ZOTE_SUB":
                    if (settings.creditsOn) return "by flame-shadow";
                    return "";
                // Rad
                case "HOLLOW_KNIGHT_SUPER":
                    if (settings.creditsOn) return "Wisp0468 presents";
                    return "The";
                case "HOLLOW_KNIGHT_MAIN":
                    if (!settings.creditsOn) return "Doodle Knight";
                    if (!settings.doodleNamesOn) return "The Hollow Knight";
                    return "The Doodle Knight";
            }


            return orig;//key;//
        }

        public enum Bosses
        {
            GRUZ_MOTHER,
            VENGEFLY_KING,
            BROODING_MAWLEK,
            FALSE_KNIGHT,
            //HORNET, // No sheet
            MASSIVE_MOSS_CHARGER,
            FLUKEMARM,
            MANTIS_LORDS,
            OBLOBBLES,
            HIVE_KNIGHT,
            BROKEN_VESSEL,
            NOSK,
            WINGED_NOSK,
            COLLECTOR,
            GOD_TAMER,
            CRYSTAL_GUARDIAN,
            UUMUU,
            TRAITOR_LORD,
            GREY_PRINCE_ZOTE,
            SOUL_WARRIOR,
            SOUL_MASTER,
            DUNG_DEFENDER,
            WHITE_DEFENDER,
            WATCHER_KNIGHT,
            NO_EYES,
            MARMU,
            GALIEN,
            MARKOTH,
            XERO,
            GORB,
            ELDER_HU,
            ORO_MATO,
            PAINTMASTER_SHEO,
            GREAT_NAILSAGE_SLY,
            PURE_VESSEL,
            //GRIMM, // Exceptional
            NIGHTMARE_KING,
            HOLLOW_KNIGHT, // No Sheet
            //RADIANCE, // Exceptional
            ZOTE
        }
        public enum Specials
        {
            GRIMM1,
            GRIMM2,
            RADIANCE1,
            RADIANCE2,
        }

        internal static readonly Dictionary<Bosses, string> BossStrings = new Dictionary<Bosses, string>()
        {
            { Bosses.GRUZ_MOTHER, "GruzMother" },
            { Bosses.VENGEFLY_KING, "VengeflyKing" },
            { Bosses.BROODING_MAWLEK, "BroodingMawlek" },
            { Bosses.FALSE_KNIGHT, "FalseKnight" },
            //{ Bosses.HORNET, "Hornet" },
            { Bosses.MASSIVE_MOSS_CHARGER, "MassiveMossCharger" },
            { Bosses.FLUKEMARM, "Flukemarm" },
            { Bosses.MANTIS_LORDS, "MantisLord" },
            { Bosses.OBLOBBLES, "Oblobble" },
            { Bosses.HIVE_KNIGHT, "HiveKnight" },
            { Bosses.BROKEN_VESSEL, "BrokenVessel" },
            { Bosses.NOSK, "Nosk" },
            { Bosses.WINGED_NOSK, "WingedNosk" },
            { Bosses.COLLECTOR, "Collector" },
            { Bosses.GOD_TAMER, "GodTamer" },
            { Bosses.CRYSTAL_GUARDIAN, "CrystalGuardian" },
            { Bosses.UUMUU, "Uumuu" },
            { Bosses.TRAITOR_LORD, "TraitorLord" },
            { Bosses.GREY_PRINCE_ZOTE, "GreyPrinceZote" },
            { Bosses.SOUL_WARRIOR, "SoulWarrior" },
            { Bosses.SOUL_MASTER, "SoulMaster" },
            { Bosses.DUNG_DEFENDER, "DungDefender" },
            { Bosses.WHITE_DEFENDER, "WhiteDefender" },
            { Bosses.WATCHER_KNIGHT, "WatcherKnight" },
            { Bosses.NO_EYES, "NoEyes" },
            { Bosses.MARMU, "Marmu" },
            { Bosses.GALIEN, "Galien" },
            { Bosses.MARKOTH, "Markoth" },
            { Bosses.XERO, "Xero" },
            { Bosses.GORB, "Gorb" },
            { Bosses.ELDER_HU, "ElderHu" },
            { Bosses.ORO_MATO, "OroMato" },
            { Bosses.PAINTMASTER_SHEO, "PaintmasterSheo" },
            { Bosses.GREAT_NAILSAGE_SLY, "GreatNailsageSly" },
            { Bosses.PURE_VESSEL, "PureVessel" },
            //{ Bosses.GRIMM, "Grimm" },
            { Bosses.NIGHTMARE_KING, "NightmareKing" },
            { Bosses.HOLLOW_KNIGHT, "TheHollowKnight" },
            //{ Bosses.RADIANCE, "Radiance" },
            { Bosses.ZOTE, "Zote" },
        };

        internal static readonly Dictionary<Specials, ValueTuple<string, int>> SpecialStrings = new Dictionary<Specials, ValueTuple<string, int>>()
        {
            { Specials.GRIMM1, ("Grimm", 0) },
            { Specials.GRIMM2, ("Grimm", 1) },
            { Specials.RADIANCE1, ("Radiance", 0) },
            { Specials.RADIANCE2, ("Radiance", 1) },
        };
    }
}