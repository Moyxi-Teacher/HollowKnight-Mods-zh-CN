using UnityEngine.UI;

namespace CustomKnight.NewUI
{
    internal static class UIController
    {
        // GetSkinIcon 中裁剪 knight.png 所需的**最小尺寸**：
        //   裁剪区域为 (2802, 4096-3155, 86, 120) = (2802, 941, 86, 120)
        //   GetPixels(x, y, w, h) 要求 x+w <= 纹理宽度、y+h <= 纹理高度
        private const int NEED_W = 2802 + 86;             // 2888
        private const int NEED_H = (4096 - 3155) + 120;   // 1061

        private static GameObject UI, content, viewport;
        private static ScrollRect scrollRect;

        internal static Font arial;
        internal static Font perpetua;
        internal static Font trajanBold;
        internal static Font trajanNormal;
        private static float lastScrollPosition;

        private static void LoadResources()
        {
            foreach (Font font in Resources.FindObjectsOfTypeAll<Font>())
            {
                if (font != null && font.name == "TrajanPro-Bold")
                {
                    trajanBold = font;
                }

                if (font != null && font.name == "TrajanPro-Regular")
                {
                    trajanNormal = font;
                }

                //Just in case for some reason the computer doesn't have arial
                if (font != null && font.name == "Perpetua")
                {
                    perpetua = font;
                }

                foreach (string fontName in Font.GetOSInstalledFontNames())
                {
                    if (fontName.ToLower().Contains("arial"))
                    {
                        arial = Font.CreateDynamicFontFromOSFont(fontName, 13);
                        break;
                    }
                }
            }

        }
        static UIController()
        {
            LoadResources();
        }
        private static void OnSceneChange(Scene prevScene, Scene nextScene)
        {
            if (nextScene.name == "Menu_Title")
            {
                hideMenu();
            }
        }
        private static void OnUnpause(On.HeroController.orig_UnPause orig, HeroController self)
        {
            hideMenu();
            orig(self);
        }
        private static void OnPause(On.HeroController.orig_Pause orig, HeroController self)
        {
            showMenu();
            orig(self);
        }
        public static void EnableMenu()
        {
            CustomKnight.GlobalSettings.EnablePauseMenu = true;
            if (UI == null)
            {
                GenerateMenu();
            }
            On.HeroController.Pause += OnPause;
            On.HeroController.UnPause += OnUnpause;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChange;
        }

        public static void DisableMenu()
        {
            CustomKnight.GlobalSettings.EnablePauseMenu = false;
            hideMenu();
            On.HeroController.Pause -= OnPause;
            On.HeroController.UnPause -= OnUnpause;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnSceneChange;
        }
        public static void GenerateMenu()
        {
            UI = new GameObject("UI Parent");
            var cv = UI.AddComponent<UnityEngine.Canvas>();
            cv.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = UI.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            UI.AddComponent<GraphicRaycaster>();
            var rt = UI.GetAddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(300f, 1080f);
            rt.pivot = new Vector2(1f, 0f);


            scrollRect = UI.AddComponent<ScrollRect>();


            viewport = new GameObject("Viewport");
            viewport.transform.SetParent(UI.transform, false);
            var viewrt = viewport.GetAddComponent<RectTransform>();
            viewrt.sizeDelta = new Vector2(300f, 1080f);
            viewrt.anchorMin = new Vector2(1f, 0f);
            viewrt.anchorMax = new Vector2(1f, 0f);
            viewrt.pivot = new Vector2(1f, 0f);
            var mask = viewport.AddComponent<Mask>();
            var bg = viewport.GetAddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.7f);

            CreateUpdateGUI();

            scrollRect.viewport = viewrt;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.scrollSensitivity = 25f;
            scrollRect.verticalNormalizedPosition = 1f;

            GameObject.DontDestroyOnLoad(UI);
            hideMenu();
        }
        public static void hideMenu()
        {
            UI.SetActive(false);
        }
        public static void showMenu()
        {
            if (!CustomKnight.GlobalSettings.EnablePauseMenu) { return; }
            UI.SetActive(true);
        }

        public static Texture2D GetSkinIcon(ISelectableSkin skin)
        {
            var orbIcon = "orbicon.png";
            var orbFull = "OrbFull.png";
            var knight = "knight.png";
            if (skin.Exists(orbIcon))
            {
                return skin.GetTexture(orbIcon);
            }
            Texture2D defaultOrb = Texture2D.blackTexture;

            if (skin.Exists(knight))
            {
                var defaultSkin = SkinManager.GetDefaultSkin();
                if (skin.Exists(orbFull))
                {
                    defaultOrb = skin.GetTexture(orbFull);
                }
                else if (defaultSkin.Exists(orbFull))
                {
                    defaultOrb = defaultSkin.GetTexture(orbFull);
                }
                // 【修复 2026-09-18】原代码此行直接对 skin.GetTexture(knight) 做裁剪：
                //   · StaticSkin.GetTexture 读取失败时只记日志并返回 null；
                //   · 图片解码失败时纹理仍停留在 new Texture2D(2, 2, ...) 的尺寸。
                // 两种情况都会让 GetCropped / GetPixels 抛出 UnityException，
                // 进而中断整个 CustomKnight.Initialize，导致模组加载失败
                // （皮肤菜单与换肤功能全部不可用）。
                // 这里加保护：失败时退回到"不使用裁剪图标"，并把出问题的皮肤写进日志。
                Texture2D tex = null;
                try
                {
                    var knightTex = skin.GetTexture(knight);
                    if (knightTex != null && knightTex.width >= NEED_W && knightTex.height >= NEED_H)
                    {
                        tex = knightTex.GetCropped(new Rect(2802f, 4096f - 3155f, 86f, 120f));
                    }
                    else
                    {
                        CustomKnight.Instance?.Log(
                            $"[图标生成] 皮肤「{skin.GetName()}」的 knight.png 不可用（" +
                            (knightTex == null ? "读取失败" : $"{knightTex.width}x{knightTex.height}") +
                            "），已跳过裁剪");
                    }
                }
                catch (Exception e)
                {
                    CustomKnight.Instance?.Log(
                        $"[图标生成] 皮肤「{skin.GetName()}」生成图标时出错，已跳过：{e.Message}");
                }

                if (tex == null)
                {
                    // 沿用原代码在极端情况下的返回值
                    return defaultOrb;
                }

                if (defaultOrb != Texture2D.blackTexture)
                {
                    tex = SheetItem.Overlay(defaultOrb, tex, 50, 65);
                }
                DefaultSkin.Save(tex, skin.GetId(), orbIcon, false);
                return tex;
            }

            //should never happen but still
            return defaultOrb;
        }

        public static void ApplySkin(ISelectableSkin skin)
        {
            UIManager.instance.TogglePauseGame();
            CustomKnight.GlobalSettings.AddRecentSkin(skin.GetId());
            UIController.CreateUpdateGUI();
            HeroController.instance?.GetComponent<SpriteFlash>().flashFocusHeal();
            CoroutineHelper.WaitForFramesBeforeInvoke(10, () =>
            {
                scrollRect.verticalNormalizedPosition = 1f;
                SkinManager.SetSkinById(skin.GetId());
            });
        }

        public static void CreateUpdateGUI()
        {
            if (UI == null)
            {
                return;
            }
            if (content != null)
            {
                GameObject.DestroyImmediate(content);
            }
            // create new content object

            content = new GameObject("content");
            var bg = content.GetAddComponent<Image>();
            bg.raycastTarget = true;
            bg.color = Color.clear;
            content.transform.SetParent(viewport.transform, false);
            var contentrt = content.GetAddComponent<RectTransform>();
            contentrt.anchorMin = Vector2.zero;
            contentrt.anchorMax = new Vector2(1f, 0f);
            contentrt.pivot = new Vector2(1f, 0.5f);
            var group = content.AddComponent<VerticalLayoutGroup>();
            content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            group.childForceExpandWidth = false;
            group.childAlignment = TextAnchor.UpperCenter;
            group.padding = new RectOffset() { top = 100, bottom = 300 };
            group.childControlHeight = false;

            scrollRect.content = contentrt;

            var title = new UIText(content, "Title", "Skin List");
            title.text.fontSize = 25;
            title.textTransform.sizeDelta = new Vector2(168f, 25f);

            var keybind = CustomKnight.GlobalSettings.Keybinds.OpenSkinList.GetKeyOrMouseBinding().Key;
            var helpText = new UIText(content, "helpText", $"Press {keybind} to toggle menu");
            helpText.text.fontSize = 10;
            helpText.text.alignment = TextAnchor.UpperCenter;
            helpText.textTransform.sizeDelta = new Vector2(168f, 50f);

            var favSkins = new List<string>();
            favSkins.AddRange(CustomKnight.GlobalSettings.FavoriteSkins);
            favSkins.AddRange(CustomKnight.GlobalSettings.RecentSkins);
            for (var index = 0; favSkins.Count < Math.Min(CustomKnight.GlobalSettings.MaxSkinCache, SkinManager.SkinsList.Count); index++)
            {
                var id = SkinManager.SkinsList[index].GetId();
                if (!favSkins.Contains(id))
                {
                    favSkins.Add(id);
                }
            }
            foreach (var skinId in favSkins)
            {
                if (!SkinManager.SkinExistsById(skinId))
                {
                    continue;
                }
                var skin = SkinManager.GetSkinById(skinId);
                var tex = GetSkinIcon(skin);
                var skinName = skin.GetName();
                skinName = skinName.Length > CustomKnight.GlobalSettings.NameLength ? skinName.Substring(0, CustomKnight.GlobalSettings.NameLength) : skinName;

                if (tex != Texture2D.blackTexture)
                {
                    var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    var btn = content.AddButton(skinName, sprite, (e) => { ApplySkin(skin); });
                }
                var txtbtn = content.AddButton($"Name_{skinName}", skinName, (e) => { ApplySkin(skin); });
            }
            foreach (var skin in SkinManager.SkinsList)
            {
                if (favSkins.Contains(skin.GetId()))
                {
                    continue;
                }
                var skinName = skin.GetName();
                skinName = skinName.Length > CustomKnight.GlobalSettings.NameLength ? skinName.Substring(0, CustomKnight.GlobalSettings.NameLength) : skinName;

                var btn = content.AddButton(skinName, skinName, (e) => { ApplySkin(skin); });
            }
        }


        static UIButton AddButton(this GameObject parent, string name, Sprite sprite, Action<UIButton> callback)
        {
            return new UIButton(parent, name, sprite, callback);
        }

        static UIButton AddButton(this GameObject parent, string name, string displayName, Action<UIButton> callback)
        {
            return new UIButton(parent, name, displayName, callback);
        }

        internal static void ToggleSkinList()
        {
            if (CustomKnight.GlobalSettings.EnablePauseMenu)
            {
                DisableMenu();
                lastScrollPosition = scrollRect.verticalNormalizedPosition;
            }
            else
            {
                EnableMenu();
                showMenu();
                scrollRect.verticalNormalizedPosition = lastScrollPosition;
            }
        }
    }
}
