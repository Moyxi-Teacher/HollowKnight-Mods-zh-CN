
using Modding.Converters;
using Newtonsoft.Json;

namespace CustomKnight
{
    /// <summary>
    /// Per save settings
    /// </summary>
    public class SaveModSettings
    {
        /// <summary>
        /// Selected Skin
        /// </summary>
        public string DefaultSkin { get; set; } = null;
    }

    /// <summary>
    /// Overall CK settings
    /// </summary>
    public class GlobalModSettings
    {
        /// <summary>
        /// Version on which the settings file was created
        /// </summary>
        public string Version { get; set; } = "";

        /// <summary>
        /// Enables or disables Preloading gameObjects
        /// </summary>
        public bool Preloads { get; set; } = true;

        /// <summary>
        /// Currently Selected Skin
        /// </summary>
        public string DefaultSkin { get; set; } = SkinManager.DEFAULT_SKIN;

        /// <summary>
        /// Max Length of the Skin Names that is displayed in UI
        /// </summary>
        public int NameLength { get; set; } = 15;

        /// <summary>
        /// Total Number of skins to keep in memory per session
        /// </summary>
        public int MaxSkinCache { get; set; } = 10; // it can be 10 because in most cases people will use 1-2 skins in one session anyway.

        /// <summary>
        /// 平滑加载：将换肤时的贴图解码拆到逐帧进行，避免一次性全量解码导致卡顿；
        /// 关闭后恢复为原版的同步加载行为。
        /// </summary>
        public bool SmoothTextureLoading { get; set; } = true;

        /// <summary>
        /// 平滑加载时每帧最多解码的贴图张数
        /// </summary>
        public int SmoothTextureBatchSize { get; set; } = 3;

        /// <summary>
        /// 平滑加载时每帧解码所允许的时间预算（毫秒）
        /// </summary>
        public int SmoothTextureBudgetMs { get; set; } = 8;

        /// <summary>
        /// 自动补齐：当所选皮肤缺少某张贴图时，自动从 Skins 其它皮肤目录（含 Default）
        /// 随机挑选同名贴图补上，使不完整的皮肤也能正常显示；关闭后恢复原版缺图行为。
        /// </summary>
        public bool AutoFill { get; set; } = true;

        /// <summary>
        /// 自动补齐分类：小骑士 —— 补齐皮肤根目录下的指定贴图
        /// （Geo.png、Hatchling.png、Hud.png、Knight.png、OrbFull.png、
        /// Sprint.png、VoidSpells.png、VS.png、Wraiths.png）。
        /// </summary>
        public bool AutoFillKnight { get; set; } = true;

        /// <summary>
        /// 自动补齐分类：护符 —— 补齐 Charms 子目录下的护符贴图（Charm_*.png）。
        /// </summary>
        public bool AutoFillCharms { get; set; } = true;

        /// <summary>
        /// 自动补齐分类：血条 —— 补齐血条/魂条相关贴图。
        /// 注：CustomKnight 自身并无 "HPBar" 目录，实际血条/魂条贴图位于
        /// Inventory/Heart_*.png 与 Inventory/Vessel_*.png；
        /// 若今后皮肤文件夹内直接放置 HPBar/ 目录贴图也一并纳入此分类。
        /// </summary>
        public bool AutoFillHPBar { get; set; } = true;

        /// <summary>
        /// Enables Swappe
        /// </summary>
        public bool SwapperEnabled { get; set; } = true;


        /// <summary>
        /// Skins for each of the save slots
        /// </summary>
        [JsonProperty]
        private Dictionary<int, string> saveSkins = new Dictionary<int, string>() { { 0, SkinManager.DEFAULT_SKIN }, { 1, SkinManager.DEFAULT_SKIN }, { 2, SkinManager.DEFAULT_SKIN }, { 3, SkinManager.DEFAULT_SKIN } };


        /// <summary>
        /// Option to enable the new Pause Menu
        /// </summary>
        public bool EnablePauseMenu { get; set; } = true;

        /// <summary>
        /// Option to enable the Save Selection screen to be skinned
        /// </summary>
        public bool EnableSaveHuds { get; set; } = true;

        /// <summary>
        /// Option to enable swapping particles (only active when swap is enabled)
        /// </summary>
        public bool EnableParticleSwap { get; set; } = true;

        /// <summary>
        /// Option to disable loading directory swaps, makes it possible to debug skins incompatible with names.json
        /// </summary>
        public bool DisableDirectorySwaps { get; set; } = false;

        /// <summary>
        /// Used to indicate to generate default skin on next restart
        /// </summary>
        public bool GenerateDefaultSkin { get; set; } = true;

        /// <summary>
        /// Favorite skins are kept at the top of the skin list
        /// </summary>
        public List<string> FavoriteSkins { get; set; } = new();

        /// <summary>
        /// Recent skins are kept after the favorite skins
        /// </summary>
        public List<string> RecentSkins { get; set; } = new();

        /// <summary>
        /// Saves the keybinds
        /// </summary>
        [JsonConverter(typeof(PlayerActionSetConverter))]
        public KeyBinds Keybinds = new();
        internal void AddRecentSkin(string skinId)
        {
            RecentSkins.RemoveAll((v) => v == skinId);
            RecentSkins.Insert(0, skinId);
            if (RecentSkins.Count > MaxSkinCache)
            {
                RecentSkins = RecentSkins.GetRange(0, MaxSkinCache);
            }
        }

        internal static string GetSkinForProfileID(int id)
        {
            if (CustomKnight.GlobalSettings.saveSkins.TryGetValue(id, out var skinName))
            {
                return skinName;
            }
            SetSkinForProfileID(id, SkinManager.DEFAULT_SKIN);
            return SkinManager.DEFAULT_SKIN;
        }


        internal static void SetSkinForProfileID(int id, string skinName)
        {
            CustomKnight.GlobalSettings.saveSkins[GameManager.instance.profileID - 1] = skinName;
        }

        internal static void ResetProfileSkins()
        {
            CustomKnight.GlobalSettings.saveSkins = new GlobalModSettings().saveSkins;
        }
    }

}