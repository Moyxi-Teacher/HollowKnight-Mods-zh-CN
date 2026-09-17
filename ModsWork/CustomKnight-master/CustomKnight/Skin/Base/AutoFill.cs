using System;
using System.Collections.Generic;
using System.IO;

namespace CustomKnight
{
    /// <summary>
    /// 自动补齐辅助类：当所选皮肤缺少某张贴图时，从其它已安装皮肤中随机挑选同名贴图补上，
    /// 使不完整的皮肤也能正常显示外观；与菜单"高级选项 → 自动补齐缺失贴图"开关联动。
    /// 自动补齐仅作用于所启用的分类（小骑士 / 护符 / 血条）内的文件，其余文件不自动补充。
    /// </summary>
    internal static class AutoFill
    {
        /// <summary>自动补齐的分类</summary>
        internal enum AutoFillCategory
        {
            None,
            Knight,
            Charms,
            HPBar
        }

        /// <summary>小骑士分类：皮肤根目录下必须存在的 9 张贴图文件名</summary>
        private static readonly HashSet<string> KnightFiles = new(StringComparer.OrdinalIgnoreCase)
        {
            "Geo.png", "Hatchling.png", "Hud.png", "Knight.png", "OrbFull.png",
            "Sprint.png", "VoidSpells.png", "VS.png", "Wraiths.png"
        };

        /// <summary>
        /// 根据贴图的相对路径判断其所属自动补齐分类。
        /// 小骑士 = 皮肤根目录的指定 9 张文件；护符 = Charms/ 子目录；血条 = HPBar/ 或 Inventory/Heart_* / Inventory/Vessel_*。
        /// </summary>
        internal static AutoFillCategory GetCategory(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return AutoFillCategory.None;
            string f = fileName.Replace('\\', '/');
            // 护符 = Charms/ 子目录
            if (f.StartsWith("Charms/", StringComparison.OrdinalIgnoreCase))
                return AutoFillCategory.Charms;
            // 血条：HPBar/ 前缀，或实际的血条/魂条 Inventory/Heart_* 与 Inventory/Vessel_*
            if (f.StartsWith("HPBar/", StringComparison.OrdinalIgnoreCase)
                || f.StartsWith("Inventory/Heart", StringComparison.OrdinalIgnoreCase)
                || f.StartsWith("Inventory/Vessel", StringComparison.OrdinalIgnoreCase))
                return AutoFillCategory.HPBar;
            // 小骑士 = 皮肤根目录的指定 9 张贴图
            string name = Path.GetFileName(f);
            if (KnightFiles.Contains(name))
                return AutoFillCategory.Knight;
            return AutoFillCategory.None;
        }

        /// <summary>
        /// 判断某张贴图是否属于当前允许自动补齐的范围（受三个分类开关与总开关共同约束）。
        /// </summary>
        internal static bool IsAllowed(string fileName)
        {
            if (!CustomKnight.GlobalSettings.AutoFill)
                return false;
            var cat = GetCategory(fileName);
            return cat switch
            {
                AutoFillCategory.Knight => CustomKnight.GlobalSettings.AutoFillKnight,
                AutoFillCategory.Charms => CustomKnight.GlobalSettings.AutoFillCharms,
                AutoFillCategory.HPBar  => CustomKnight.GlobalSettings.AutoFillHPBar,
                _ => false,
            };
        }

        /// <summary>
        /// 尝试为"当前皮肤缺失"的贴图自动补齐：遍历所有已安装皮肤（含 Default），
        /// 随机选一个拥有同名文件的皮肤并取出其贴图，成功后写入贴图缓存并把 missing 置为 false。
        /// 找不到任何候选时返回 false。
        /// 仅当贴图属于当前启用的分类时才会补齐；其他文件不自动补充。
        /// </summary>
        internal static bool TryFill(CustomKnightTexture texture, ISelectableSkin currentSkin)
        {
            if (texture == null || string.IsNullOrEmpty(texture.fileName))
            {
                return false;
            }
            // 自动补齐仅作用于所启用分类内的文件
            if (!IsAllowed(texture.fileName))
            {
                return false;
            }
            string currentId = currentSkin?.GetId();
            List<ISelectableSkin> candidates = new List<ISelectableSkin>();
            ISelectableSkin[] skins = SkinManager.GetInstalledSkins();
            if (skins == null)
            {
                return false;
            }
            for (int i = 0; i < skins.Length; i++)
            {
                ISelectableSkin skin = skins[i];
                if (skin == null || skin == currentSkin)
                {
                    continue;
                }
                if (currentId != null && skin.GetId() == currentId)
                {
                    continue;
                }
                try
                {
                    if (skin.Exists(texture.fileName))
                    {
                        candidates.Add(skin);
                    }
                }
                catch (Exception)
                {
                    // 个别皮肤实现异常时忽略该候选皮肤，继续寻找其它候选
                }
            }
            if (candidates.Count == 0)
            {
                return false;
            }
            ISelectableSkin picked = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            Texture2D filledTex = picked.GetTexture(texture.fileName);
            if (filledTex == null)
            {
                return false;
            }
            texture.tex = filledTex;
            texture.missing = false;
            // 将补齐结果写入贴图缓存，避免同一次换肤中对同一张缺失贴图反复随机补齐
            if (currentSkin != null && currentSkin.shouldCache())
            {
                TextureCache.setSkinTextureCache(currentSkin.GetId(), texture.fileName,
                    new CustomKnightTexture(texture.fileName, false, texture.defaultTex, filledTex));
            }
            return true;
        }
    }
}
