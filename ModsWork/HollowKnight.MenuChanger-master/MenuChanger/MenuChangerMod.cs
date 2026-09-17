using GlobalEnums;
using Modding;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MenuChanger
{
    public class MenuChangerMod : Mod, ILocalSettings<Settings>, IMenuMod, ITogglableMod
    {
        public MenuChangerMod() : base("模组菜单变更")
        {
            instance = this;
            LogHelper.OnLog += Log;
            LogHelper.OnLogError += LogError;
            UIManager.EditMenus += PrefabMenuObjects.Setup;
        }
        internal static MenuChangerMod instance { get; private set; }
        internal static readonly List<MenuPage> displayedPages = new();

        public static event Action OnExitMainMenu;

        public static void HideAllMenuPages()
        {
            while (displayedPages.FirstOrDefault() is MenuPage page) page.Hide();
        }

        private void OnSceneChange(Scene from, Scene to)
        {
            if (from.name == "Menu_Title" && to.name != "Menu_Title")
            {
                ModeMenu.OnExitMainMenu();
                PrefabMenuObjects.Dispose();
                try
                {
                    OnExitMainMenu?.Invoke();
                }
                catch (Exception e)
                {
                    LogError($"Error invoking OnExitMainMenu:\n{e}");
                }
                try
                {
                    foreach (var comp in UIManager.instance.UICanvas.transform.GetComponentsInChildren<MenuPage.MenuPageComponent>(true))
                    {
                        UObject.Destroy(comp.gameObject);
                    }
                }
                catch (Exception e)
                {
                    LogError($"Error destroying MenuPages on exiting menu:{e}");
                }
            }
        }

        private void EditUI()
        {
            displayedPages.Clear();
            ResumeMenu.Reset();
            ModeMenu.OnEnterMainMenu();
        }

        public override int LoadPriority() => 100000;

        public override void Initialize()
        {
            ThreadSupport.Setup();
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChange;
            UIManager.EditMenus += EditUI;
            _onUIGoToProfileMenu = (o, s) =>
            {
                HideAllMenuPages();
                o(s);
            };
            On.UIManager.UIGoToProfileMenu += _onUIGoToProfileMenu;
            On.UIManager.ContinueGame += OnContinueGame;
            On.UIManager.StartNewGame += OnStartGame;
            _onUIGoToPlayModeMenu = (o, s) =>
            {
                if (ModeMenu.Active) s.StartCoroutine(GoToModeMenu(s));
                else o(s);
            };
            On.UIManager.UIGoToPlayModeMenu += _onUIGoToPlayModeMenu;
            ResumeMenu.Hook();
        }

        private On.UIManager.hook_UIGoToProfileMenu _onUIGoToProfileMenu;
        private On.UIManager.hook_UIGoToPlayModeMenu _onUIGoToPlayModeMenu;

        public void Unload()
        {
            LogHelper.OnLog -= Log;
            LogHelper.OnLogError -= LogError;
            UIManager.EditMenus -= PrefabMenuObjects.Setup;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnSceneChange;
            UIManager.EditMenus -= EditUI;
            if (_onUIGoToProfileMenu != null) On.UIManager.UIGoToProfileMenu -= _onUIGoToProfileMenu;
            if (_onUIGoToPlayModeMenu != null) On.UIManager.UIGoToPlayModeMenu -= _onUIGoToPlayModeMenu;
            On.UIManager.ContinueGame -= OnContinueGame;
            On.UIManager.StartNewGame -= OnStartGame;
            HideAllMenuPages();
            instance = null;
        }

        private void OnStartGame(On.UIManager.orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
        {
            HideAllMenuPages();
            orig(self, permaDeath, bossRush);
        }

        private void OnContinueGame(On.UIManager.orig_ContinueGame orig, UIManager self)
        {
            HideAllMenuPages();
            orig(self);
        }



        private IEnumerator GoToModeMenu(UIManager s)
        {
            InputHandler.Instance.StopUIInput();
            yield return s.HideSaveProfileMenu();
            ReflectionHelper.CallMethod(s, "SetMenuState", MainMenuState.PLAY_MODE_MENU);
            InputHandler.Instance.StartUIInput();
            ModeMenu.Show();
        }


        public Settings Settings = new();

        public bool ToggleButtonInsideMenu => true;

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return new List<IMenuMod.MenuEntry>
            {
                new(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values, "为其他模组提供游戏内菜单构建框架。", toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader)
            };
        }

        public override string GetVersion()
        {
            return _version;
        }

        private static readonly string _sha1;
        private static readonly string _version;
        static MenuChangerMod()
        {
            System.Reflection.Assembly a = typeof(MenuChangerMod).Assembly;

            using var sha1 = System.Security.Cryptography.SHA1.Create();
            using var sr = File.OpenRead(a.Location);
            _sha1 = Convert.ToBase64String(sha1.ComputeHash(sr));

            int buildHash;
            unchecked // stable string hash code
            {
                int hash1 = 5381;
                int hash2 = hash1;
                string str = _sha1;

                for (int i = 0; i < str.Length && str[i] != '\0'; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1 || str[i + 1] == '\0')
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                buildHash = hash1 + (hash2 * 1566083941);
                buildHash = Math.Abs(buildHash) % 997;
            }

            Version v = a.GetName().Version;
            _version = $"{v.Major}.{v.Minor}.{v.Build}+{buildHash.ToString().PadLeft(3, '0')}";
        }


        public void OnLoadLocal(Settings s)
        {
            Settings = s;
        }

        public Settings OnSaveLocal()
        {
            return Settings;
        }
    }
}
