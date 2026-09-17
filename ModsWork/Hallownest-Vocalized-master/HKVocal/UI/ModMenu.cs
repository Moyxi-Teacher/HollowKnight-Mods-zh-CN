using GlobalEnums;
using Language;
using Satchel.BetterMenus.Config;
using InputField = Satchel.BetterMenus.InputField;
using UMenuButton = UnityEngine.UI.MenuButton;
using MenuButton = Satchel.BetterMenus.MenuButton;

namespace HKVocals.UI;
public static class ModMenu
{
    // 仅中文游戏语言返回中文，其余语言返回原始英文。
    private static bool IsZH => Language.Language.CurrentLanguage().ToString() == "ZH";
    private static string Lz(string zh, string en) => IsZH ? zh : en;

    public static Menu MenuRef;
    public static Menu DevMenuRef;
    
    public static MenuScreen CreateModMenuScreen(MenuScreen modListMenu)
    {
        MenuScreen DevMenuScreen = CreateDevMenuScreen(modListMenu);
        MenuRef ??= new Menu(Lz("圣巢之声", "HK Vocals"), new Element[]
        {
            //new TextPanel("To change volume, please use Audio Menu"),
            new MenuButton(Lz("更改音量", "Audio Settings"), Lz("更改语音演员的音量", "Change voice actor volume"), _ =>
            {
                UIManager.instance.UILeaveDynamicMenu(RefVanillaMenu.AudioMenuScreen, MainMenuState.AUDIO_MENU);
            }, proceed: true),

            Blueprints.HorizontalBoolOption(Lz("滚动锁定", "Scroll Lock"),
                Lz("首次对话是否应锁定滚动直到音频播放完毕？", "Should scrolling be locked until audio finishes on first dialogue?"),
                (i) => HKVocals._globalSettings.scrollLock = i,
                () => HKVocals._globalSettings.scrollLock),

            Blueprints.HorizontalBoolOption(Lz("自动滚动", "Auto Scroll"),
                Lz("对话是否应在音频播放完毕后自动滚动？", "Should dialogue auto-scroll after audio finishes?"),
                (i) =>
                {
                    HKVocals._globalSettings.autoScroll = i;
                    MenuRef.Find(Lz("自动滚动速度", "Auto Scroll Speed")).isVisible = i;
                    MenuRef.Update();
                }
                ,
                () => HKVocals._globalSettings.autoScroll),

            new HorizontalOption(Lz("自动滚动速度", "Auto Scroll Speed"),
                Lz("音频播放完毕后自动滚动的速度应有多快？", "How fast should dialogue auto-scroll after audio?"),
                Enum.GetNames(typeof(MajorFeatures.AutoScroll.ScrollSpeed)).ToArray(),
                (i) => HKVocals._globalSettings.scrollSpeed = (MajorFeatures.AutoScroll.ScrollSpeed)i,
                () => (int)HKVocals._globalSettings.scrollSpeed,
                Id: "自动滚动速度")
            {
                isVisible = HKVocals._globalSettings.autoScroll
            },

            Blueprints.HorizontalBoolOption(Lz("梦钉对话", "Dream Nail Dialogue"),
                Lz("梦钉对话是否应有语音？", "Should Dream Nail dialogue have voice?"),
                (i) =>
                {
                    HKVocals._globalSettings.dnDialogue = i;
                    MenuRef.Find(Lz("自动Boss呐喊", "Auto Boss Shout")).isVisible = !i;
                    MenuRef.Update();
                },
                () => HKVocals._globalSettings.dnDialogue),

            new HorizontalOption(Lz("自动Boss呐喊", "Auto Boss Shout"),
                Lz("某些Boss是否应自动呐喊？", "Should certain bosses shout automatically?"),
                new[] { Lz("开", "On"), Lz("关", "Off") },
                i => HKVocals._globalSettings.automaticBossDialogue = i == 0,
                () => HKVocals._globalSettings.automaticBossDialogue ? 0 : 1,
                Id: "自动Boss对话")
            {
                isVisible = !HKVocals._globalSettings.dnDialogue
            },

            Blueprints.HorizontalBoolOption(Lz("衰减音频", "Dampen Audio"),
                Lz("播放音频时是否应衰减音频？", "Should audio dampen while playing?"),
                i =>
                {
                    if (HKVocals._globalSettings.dampenLocked) return;
                    HKVocals._globalSettings.dampenAudio = i;
                    if (!HKVocals._globalSettings.dampenAudio)
                    {
                        MajorFeatures.DampenAudio.ForceStopDampenAudio();
                    }
                },
                () => HKVocals._globalSettings.dampenAudio),

            Blueprints.HorizontalBoolOption(Lz("商店", "Shop"),
                Lz("商店是否应有语音？", "Should shop audio have voice?"),
                i =>
                {
                    HKVocals._globalSettings.shopAudio = i;
                },
                () => HKVocals._globalSettings.shopAudio),

            Blueprints.HorizontalBoolOption(Lz("物品栏", "Inventory"),
                Lz("物品栏是否应有语音？", "Should inventory audio have voice?"),
                i =>
                {
                    HKVocals._globalSettings.invAudio = i;
                },
                () => HKVocals._globalSettings.invAudio),

            new MenuButton(Lz("加入圣巢之声 Discord 服务器！", "Join the HK Vocals Discord server!"), "", (_) =>
            {
                Application.OpenURL("https://discord.gg/p72F2St3RF");
            }),
        });
        if (HKVocals._globalSettings.DevMode)
        {
            MenuRef.AddElement(new MenuButton(Lz("开发菜单", "Dev Menu"), "", (_) => { UIManager.instance.UIGoToDynamicMenu(DevMenuRef.menuScreen); }));
        }
        return MenuRef.GetMenuScreen(modListMenu);
    }

    private static Snapshots SelectedSnapshot = Snapshots.No_Effect;
    private static String SelectedAudio = "";

    public static MenuScreen CreateDevMenuScreen(MenuScreen modListMenu)
    {
        DevMenuRef ??= new Menu(Lz("圣巢之声开发菜单", "HNV Dev Menu"), new Element[]
        {
            new HorizontalOption(
                Lz("RB-SFX（手柄音效）", "RB-SFX"),
                "",
                new []{ Lz("无效果", "No Effect"), Lz("梦境", "Dream"), Lz("洞穴", "Cave"), Lz("温泉", "Spa"), Lz("悬崖", "Cliffs"), Lz("房间", "Room"), Lz("竞技场", "Arena"), Lz("下水道", "Sewerpipe"), Lz("雾之峡谷", "Fog Canyon") },
                i =>
                {
                    SelectedSnapshot = i switch
                    {
                        0 => Snapshots.No_Effect,
                        1 => Snapshots.Dream,
                        2 => Snapshots.Cave,
                        3 => Snapshots.Spa,
                        4 => Snapshots.Cliffs,
                        5 => Snapshots.Room,
                        6 => Snapshots.Arena,
                        7 => Snapshots.Sewerpipe,
                        8 => Snapshots.Fog_Canyon,
                        _ => Snapshots.No_Effect,
                    };
                },
                () =>  0,
                "RB-SFX"
            ),
            new InputField(Lz("音频", "Audio"), (i) =>
                {
                    SelectedAudio = i;
                },
                () => SelectedAudio,
                "",
                50,
                InputFieldConfig.DefaultText,
                "AudioName"
                ),
            new MenuButton(Lz("播放音频", "Play Audio"), "", (_) =>
            {
                MixerLoader.SetSnapshot(SelectedSnapshot);
                AudioPlayer.TryPlayAudioFor(SelectedAudio);
            }),
        });
        return DevMenuRef.GetMenuScreen(modListMenu);
    }

    public static void GoToModMenu()
    {
        // we cant garuntee our modmenu has been created yet so our MenuRef.menuScreen might be null. hence we manually find modist menu and use that
        UIManager.instance.UIGoToDynamicMenu(ModMenu.MenuRef.menuScreen != null 
            ? ModMenu.MenuRef.menuScreen 
            : ModMenu.MenuRef.GetMenuScreen(UIManager.instance.UICanvas.Find("ModListMenu").GetComponent<MenuScreen>()));
    }
}
