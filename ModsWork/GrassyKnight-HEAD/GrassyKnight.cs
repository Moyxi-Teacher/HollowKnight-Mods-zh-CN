using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GrassyKnight
{
    public class GrassyKnight : Modding.Mod, Modding.IGlobalSettings<MyGlobalSettings>, Modding.ILocalSettings<MySaveData>, Modding.ITogglableMod, Modding.IMenuMod {

        public bool ToggleButtonInsideMenu => true;

        public MySaveData OnSaveLocal() => new MySaveData {
            serializedGrassDB = GrassStates.Serialize()
        };

        public void OnLoadLocal(MySaveData value) {
            GrassStates.Clear();
            GrassStates.AddSerializedData(value.serializedGrassDB);
        }

        private MyGlobalSettings Settings = new MyGlobalSettings();

        public MyGlobalSettings OnSaveGlobal() => Settings;
        public void OnLoadGlobal(MyGlobalSettings s) => Settings = s;

        // 保存所有需要在 Unload 时移除的订阅委托，确保总开关可正确关闭
        private UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode> sceneLoadedFindGrass;
        private UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode> sceneLoadedUpdateCount;
        private Action<int> savegameLoadHook;
        private Action newGameHook;
        private Modding.Delegates.SlashHitHandler slashHitHook;
        private Action heroUpdateCompass;
        private Action heroUpdateAutoMower;
        private bool hookedCompass;
        private bool hookedAutoMower;
        private EventHandler onStatsChanged;

        public void Unload() {
            try {
                On.GrassCut.ShouldCut -= HandleShouldCut;
                On.GrassBehaviour.OnTriggerEnter2D -= HandleGrassCollisionEnter;
                On.GrassCut.OnTriggerEnter2D -= HandleGrassCollisionEnter;
                On.TownGrass.OnTriggerEnter2D -= HandleGrassCollisionEnter;
                On.GrassSpriteBehaviour.OnTriggerEnter2D -= HandleGrassCollisionEnter;

                if (savegameLoadHook != null) Modding.ModHooks.SavegameLoadHook -= savegameLoadHook;
                if (newGameHook != null) Modding.ModHooks.NewGameHook -= newGameHook;
                if (slashHitHook != null) Modding.ModHooks.SlashHitHook -= slashHitHook;
                if (sceneLoadedFindGrass != null)
                    UnityEngine.SceneManagement.SceneManager.sceneLoaded -= sceneLoadedFindGrass;
                if (sceneLoadedUpdateCount != null)
                    UnityEngine.SceneManagement.SceneManager.sceneLoaded -= sceneLoadedUpdateCount;

                if (hookedCompass) Modding.ModHooks.HeroUpdateHook -= heroUpdateCompass;
                if (hookedAutoMower) Modding.ModHooks.HeroUpdateHook -= heroUpdateAutoMower;

                if (UtilityBehaviour != null) {
                    UtilityBehaviour.OnUpdate -= HandleAttachGrassCount;
                    UnityEngine.Object.Destroy(UtilityBehaviour.gameObject);
                }
                if (onStatsChanged != null) GrassStates.OnStatsChanged -= onStatsChanged;
            } catch (System.Exception e) {
                LogException("Error in Unload", e);
            }
        }

        private static readonly string[] KEY_NAMES = new[] {
            "Backspace","Delete","Tab","Clear","Return","Pause","Escape","Space",
            "Keypad0","Keypad1","Keypad2","Keypad3","Keypad4","Keypad5","Keypad6","Keypad7","Keypad8","Keypad9",
            "KeypadPeriod","KeypadDivide","KeypadMultiply","KeypadMinus","KeypadPlus","KeypadEnter","KeypadEquals",
            "UpArrow","DownArrow","RightArrow","LeftArrow","Insert","Home","End","PageUp","PageDown",
            "F1","F2","F3","F4","F5","F6","F7","F8","F9","F10","F11","F12","F13","F14","F15",
            "Alpha0","Alpha1","Alpha2","Alpha3","Alpha4","Alpha5","Alpha6","Alpha7","Alpha8","Alpha9",
            "Exclaim","DoubleQuote","Hash","Dollar","Percent","Ampersand","Quote","LeftParen","RightParen",
            "Asterisk","Plus","Comma","Minus","Period","Slash","Colon","Semicolon","Less","Equals",
            "Greater","Question","At","LeftBracket","Backslash","RightBracket","Caret","Underscore","BackQuote",
            "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
            "LeftCurlyBracket","Pipe","RightCurlyBracket","Tilde","Numlock","CapsLock","ScrollLock",
            "RightShift","LeftShift","RightControl","LeftControl","RightAlt","LeftAlt",
            "LeftCommand","LeftApple","LeftWindows","RightCommand","RightApple","RightWindows","AltGr",
            "Help","Print","SysReq","Break","Menu",
            "Mouse0","Mouse1","Mouse2","Mouse3","Mouse4","Mouse5","Mouse6",
            "JoystickButton0","JoystickButton1","JoystickButton2","JoystickButton3","JoystickButton4",
            "JoystickButton5","JoystickButton6","JoystickButton7","JoystickButton8","JoystickButton9",
            "JoystickButton10","JoystickButton11","JoystickButton12","JoystickButton13","JoystickButton14",
            "JoystickButton15","JoystickButton16","JoystickButton17","JoystickButton18","JoystickButton19",
            "Joystick1Button0","Joystick1Button1","Joystick1Button2","Joystick1Button3","Joystick1Button4",
            "Joystick1Button5","Joystick1Button6","Joystick1Button7","Joystick1Button8","Joystick1Button9",
            "Joystick1Button10","Joystick1Button11","Joystick1Button12","Joystick1Button13","Joystick1Button14",
            "Joystick1Button15","Joystick1Button16","Joystick1Button17","Joystick1Button18","Joystick1Button19",
            "Joystick2Button0","Joystick2Button1","Joystick2Button2","Joystick2Button3","Joystick2Button4",
            "Joystick2Button5","Joystick2Button6","Joystick2Button7","Joystick2Button8","Joystick2Button9",
            "Joystick2Button10","Joystick2Button11","Joystick2Button12","Joystick2Button13","Joystick2Button14",
            "Joystick2Button15","Joystick2Button16","Joystick2Button17","Joystick2Button18","Joystick2Button19",
            "Joystick3Button0","Joystick3Button1","Joystick3Button2","Joystick3Button3","Joystick3Button4",
            "Joystick3Button5","Joystick3Button6","Joystick3Button7","Joystick3Button8","Joystick3Button9",
            "Joystick3Button10","Joystick3Button11","Joystick3Button12","Joystick3Button13","Joystick3Button14",
            "Joystick3Button15","Joystick3Button16","Joystick3Button17","Joystick3Button18","Joystick3Button19",
            "Joystick4Button0","Joystick4Button1","Joystick4Button2","Joystick4Button3","Joystick4Button4",
            "Joystick4Button5","Joystick4Button6","Joystick4Button7","Joystick4Button8","Joystick4Button9",
            "Joystick4Button10","Joystick4Button11","Joystick4Button12","Joystick4Button13","Joystick4Button14",
            "Joystick4Button15","Joystick4Button16","Joystick4Button17","Joystick4Button18","Joystick4Button19",
            "Joystick5Button0","Joystick5Button1","Joystick5Button2","Joystick5Button3","Joystick5Button4",
            "Joystick5Button5","Joystick5Button6","Joystick5Button7","Joystick5Button8","Joystick5Button9",
            "Joystick5Button10","Joystick5Button11","Joystick5Button12","Joystick5Button13","Joystick5Button14",
            "Joystick5Button15","Joystick5Button16","Joystick5Button17","Joystick5Button18","Joystick5Button19",
            "Joystick6Button0","Joystick6Button1","Joystick6Button2","Joystick6Button3","Joystick6Button4",
            "Joystick6Button5","Joystick6Button6","Joystick6Button7","Joystick6Button8","Joystick6Button9",
            "Joystick6Button10","Joystick6Button11","Joystick6Button12","Joystick6Button13","Joystick6Button14",
            "Joystick6Button15","Joystick6Button16","Joystick6Button17","Joystick6Button18","Joystick6Button19",
            "Joystick7Button0","Joystick7Button1","Joystick7Button2","Joystick7Button3","Joystick7Button4",
            "Joystick7Button5","Joystick7Button6","Joystick7Button7","Joystick7Button8","Joystick7Button9",
            "Joystick7Button10","Joystick7Button11","Joystick7Button12","Joystick7Button13","Joystick7Button14",
            "Joystick7Button15","Joystick7Button16","Joystick7Button17","Joystick7Button18","Joystick7Button19",
            "Joystick8Button0","Joystick8Button1","Joystick8Button2","Joystick8Button3","Joystick8Button4",
            "Joystick8Button5","Joystick8Button6","Joystick8Button7","Joystick8Button8","Joystick8Button9",
            "Joystick8Button10","Joystick8Button11","Joystick8Button12","Joystick8Button13","Joystick8Button14",
            "Joystick8Button15","Joystick8Button16","Joystick8Button17","Joystick8Button18","Joystick8Button19"
        };

        public List<Modding.IMenuMod.MenuEntry> GetMenuData(Modding.IMenuMod.MenuEntry? toggleButtonEntry) {
            return new List<Modding.IMenuMod.MenuEntry>
            {
                new(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values, "启用或禁用割草骑士效果。", toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader),
                new("自动割草", new[]{"开","关"}, "是否自动挥剑割草。", i => Settings.AutomaticallyCutGrass = i == 0, () => Settings.AutomaticallyCutGrass ? 0 : 1),
                new("指南针", new[]{"开","关"}, "是否启用草之指南针，指向最近的未割草。", i => Settings.DisableCompass = i == 1, () => Settings.DisableCompass ? 1 : 0),
                new("指南针热键修改", KEY_NAMES, "修改用于在游戏中显示/隐藏草之指南针的按键。", i => Settings.ToggleCompassHotkey = KEY_NAMES[i], () => Array.IndexOf(KEY_NAMES, Settings.ToggleCompassHotkey))
            };
        }

        // Will be set to the exactly one ModMain in existance... Trusting
        // Modding.Mod to ensure that ModMain is only ever instantiated once...
        public static GrassyKnight Instance = null;

        // Stores which grass is cut and allows queries (like "where's the
        // nearest uncut grass?")
        GrassDB GrassStates = new GrassDB();

        // Knows if an object is grass. Very wise. Uwu. Which knower we use
        // depends on configuration
        GrassKnower SetOfAllGrass = null;

        // Usually Unity code is contained in MonoBehaviour classes, so Unity
        // has lots of very useful functionality in them (ex: access to the
        // coroutine scheduler). This is forever-living MonoBehaviour object we
        // use to give us that funcionality despite our non-MonoBheaviour
        // status.
        Behaviour UtilityBehaviour = null;

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public GrassyKnight() : base("割草骑士") {
            GrassyKnight.Instance = this;
        }

        public override void Initialize() {
            base.Initialize();

            // I tried creating this in the field initializer but it failed...
            // I think construction is too early to make game objects, though I
            // don't now why.
            UtilityBehaviour = Behaviour.CreateBehaviour();

            if (Settings.UseHeuristicGrassKnower) {
                SetOfAllGrass = new HeuristicGrassKnower();
                Log("Using HeuristicGrassKnower");

                // Because the heuristic grass knower doesn't know about grass
                // until it sees it for the first time, we need to constantly
                // look for grass each time we enter a scene.
                sceneLoadedFindGrass = (_, _1) => UtilityBehaviour.StartCoroutine(
                    WaitThenFindGrass());
                UnityEngine.SceneManagement.SceneManager.sceneLoaded += sceneLoadedFindGrass;


            } else {
                CuratedGrassKnower curatedKnower = new CuratedGrassKnower();
                SetOfAllGrass = curatedKnower;
                Log($"Using CuratedGrassKnower");

                savegameLoadHook = _ => HandleFileEntered();
                Modding.ModHooks.SavegameLoadHook += savegameLoadHook;
                newGameHook = () => HandleFileEntered();
                Modding.ModHooks.NewGameHook += newGameHook;

                foreach ((GrassKey, GrassKey) alias in curatedKnower.GetAliases()) {
                    GrassStates.AddAlias(alias.Item1, alias.Item2);
                }
            }

            // Triggered when real grass is being cut for real
            On.GrassCut.ShouldCut += HandleShouldCut;

            // Lots of various callbacks all doing the same thing: making sure
            // our grassy box is full when HandleShouldCut is called.
            On.GrassBehaviour.OnTriggerEnter2D += HandleGrassCollisionEnter;
            On.GrassCut.OnTriggerEnter2D += HandleGrassCollisionEnter;
            On.TownGrass.OnTriggerEnter2D += HandleGrassCollisionEnter;
            On.GrassSpriteBehaviour.OnTriggerEnter2D += HandleGrassCollisionEnter;

            // Backup we use to make sure we notice uncuttable grass getting
            // swung at. This is the detector of shameful grass.
            slashHitHook = HandleSlashHit;
            Modding.ModHooks.SlashHitHook += slashHitHook;

            // Update the grass count whenever we change scenes or if they
            // change.
            onStatsChanged = (_, _1) => UpdateGrassCount();
            GrassStates.OnStatsChanged += onStatsChanged;
            sceneLoadedUpdateCount = (_, _1) => UtilityBehaviour.StartCoroutine(
                WaitThenUpdateGrassCount());
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += sceneLoadedUpdateCount;

            // Makes sure our grassy counter is always in-place
            UtilityBehaviour.OnUpdate += HandleAttachGrassCount;

            // Make sure the hero always has the grassy compass component
            // attached. We could probably hook the hero object's creation to
            // be more efficient, but it's a cheap operation so imma not worry
            // about it.
            if (!Settings.DisableCompass) {
                hookedCompass = true;
                heroUpdateCompass = HandleCheckGrassyCompass;
                Modding.ModHooks.HeroUpdateHook += heroUpdateCompass;
            }

            // It's dangerous out there, make sure to bring your lawnmower!
            // This'll make sure the hero has their lawnmower handy at all
            // times.
            if (Settings.AutomaticallyCutGrass) {
                hookedAutoMower = true;
                heroUpdateAutoMower = HandleCheckAutoMower;
                Modding.ModHooks.HeroUpdateHook += heroUpdateAutoMower;
            }
        }

        // Triggered anytime the user loads a save file or starts a new game
        private void HandleFileEntered() {
            try {
                CuratedGrassKnower knower = (CuratedGrassKnower)SetOfAllGrass;
                foreach (GrassKey k in knower.GetAllGrassKeys()) {
                    GrassStates.TrySet(k, GrassState.Uncut);
                }
            } catch (System.Exception e) {
                LogException("Error in HandleFileEntered", e);
            }
        }

        // We'll hook this into a bunch of Grass components' OnTriggerEnter2D
        // methods. It's only responsibility is to store the game object that
        // the component is attached to for a moment in case ShouldCut is
        // called in the original function.
        private void HandleGrassCollisionEnter<OrigFunc, Component>(
            OrigFunc orig,
            Component self,
            Collider2D collision)
        where Component : MonoBehaviour
        where OrigFunc : MulticastDelegate
        {
            var context = new GrassyBox(self.gameObject);
            try {
                orig.DynamicInvoke(new object[] { self, collision });
            } finally {
                context.Dispose();
            }
        }

        private void HandleCheckGrassyCompass() {
            try {
                // Ensure the hero has their grassy compass friend
                GameObject hero = GameManager.instance?.hero_ctrl?.gameObject;
                if (hero != null &&
                        hero.GetComponent<GrassyCompass>() == null) {
                    GrassyCompass compassComponent =
                        hero.AddComponent<GrassyCompass>();
                    compassComponent.AllGrass = GrassStates;

                    if (Settings.ToggleCompassHotkey != null) {
                        try {
                            KeyCode hotkey = (KeyCode)Enum.Parse(
                                typeof(KeyCode),
                                Settings.ToggleCompassHotkey);
                            compassComponent.ToggleHotkey = hotkey;
                            Log($"Hotkey for toggling the Grassy Compass " +
                                $"set to {hotkey}");
                        } catch (ArgumentException) {
                            LogError(
                                $"Unrecognized key name for " +
                                $"ToggleCompassHotkey " +
                                $"{Settings.ToggleCompassHotkey}. See the " +
                                $"README.md file for a list of all valid " +
                                $"key names.");
                        }
                    }
                }
            } catch (System.Exception e) {
                LogException("Error in HandleCheckGrassyCompass", e);
            }
        }

        private void HandleCheckAutoMower() {
            try {
                // Ensure the hero has their lawnmower
                GameObject hero = GameManager.instance?.hero_ctrl?.gameObject;
                if (hero != null && hero.GetComponent<AutoMower>() == null) {
                    AutoMower autoMower = hero.AddComponent<AutoMower>();
                    autoMower.SetOfAllGrass = SetOfAllGrass;
                    autoMower.GrassStates = GrassStates;
                    LogDebug("Attached autoMower to hero");
                }
            } catch (System.Exception e) {
                LogException("Error in HandleCheckAutoMower", e);
            }
        }

        private void HandleAttachGrassCount(object _, EventArgs _1) {
            try {
                GameObject geoCounter =
                    GameManager.instance?.hero_ctrl?.geoCounter?.gameObject;
                if (geoCounter != null &&
                        geoCounter.GetComponent<GrassCount>() == null) {
                    geoCounter.AddComponent<GrassCount>();
                    LogDebug("Attached Grass Count to Geo Counter");
                    UpdateGrassCount();
                }
            } catch (System.Exception e) {
                LogException("Error in HandleCheckAutoMower", e);
            }
        }

        // Sets state of maybeGrass if it is grass
        private void MaybeSetGrassState(GameObject maybeGrass, GrassState state) {
            GrassKey k = GrassKey.FromGameObject(maybeGrass);
            if (GrassStates.Contains(k) || SetOfAllGrass.IsGrass(maybeGrass)) {
                GrassStates.TrySet(k, state);
            }
        }

        // Meant to be called when a new scene is entered
        private IEnumerator WaitThenFindGrass() {
            // The docs suggest waiting a frame after scene loads before we
            // consider the scene fully instantiated. We've got time, so wait
            // even longer.
            yield return new WaitForSeconds(0.5f);

            try {
                foreach (GameObject maybeGrass in
                         UnityEngine.Object.FindObjectsOfType<GameObject>()) {
                    MaybeSetGrassState(maybeGrass, GrassState.Uncut);
                }
            } catch (System.Exception e) {
                LogException("Error in WaitThenFindGrass", e);
            }
        }

        // Meant to be called when a new scene is entered
        private IEnumerator WaitThenUpdateGrassCount() {
            // The docs suggest wait a moment to make sure everything's set
            yield return new WaitForSeconds(0.1f);

            UpdateGrassCount();
        }

        private void UpdateGrassCount() {
            try {
                string sceneName = GameManager.instance?.sceneName;
                if (sceneName != null) {
                    GrassCount grassCount =
                        GameManager.instance
                            ?.hero_ctrl
                            ?.geoCounter
                            ?.gameObject
                            ?.GetComponent<GrassCount>();
                    grassCount?.UpdateStats(
                        GrassStates.GetStatsForScene(sceneName),
                        GrassStates.GetGlobalStats());
                }
            } catch (System.Exception e) {
                LogException("Error in UpdateGrassCount", e);
            }
        }

        private static string IndentString(string str, string indent = "... ") {
            return indent + str.Replace("\n", "\n" + indent);
        }

        public void LogException(string heading, System.Exception error) {
            LogError($"{heading}\n{IndentString(error.ToString())}");
        }

        private bool HandleShouldCut(On.GrassCut.orig_ShouldCut orig, Collider2D collision) {
            // Find out whether the original game code thinks this should be
            // cut. We'll pass this value through no matter what.
            bool shouldCut = orig(collision);

            try {
                if (shouldCut) {
                    // ShouldCut is a static function so we've hooked every
                    // function that calls ShouldCut. Our hooks will store the
                    // GameObject whose component's method is calling ShouldCut
                    // in this box so that we can grab it out. This could also
                    // be done by walking the stack upwards IF C# let us
                    // examine the argument values of stack frames, but C# does
                    // not give us a good way to do that so here we are.
                    GameObject grass = GrassyBox.GetValue();
                    MaybeSetGrassState(grass, GrassState.Cut);
                }
            } catch (System.Exception e) {
                LogException("Error in HandleShouldCut", e);

                // Exception stack traces seem to terminate once we're out
                // of this assembly... It doesn't show who called ShouldCut
                // anyways. And that's exactly the information we want if we're
                // looking for more functions to hook HandleGrassCollisionEnter
                // into.
                LogDebug("More complete stack trace:");
                LogDebug(IndentString(System.Environment.StackTrace));
            }

            return shouldCut;
        }

        private void HandleSlashHit(Collider2D otherCollider, GameObject _) {
            try {
                MaybeSetGrassState(otherCollider.gameObject,
                                   GrassState.ShouldBeCut);
            } catch(System.Exception e) {
                LogException("Error in HandleSlashHit", e);
            }
        }
    }
}
