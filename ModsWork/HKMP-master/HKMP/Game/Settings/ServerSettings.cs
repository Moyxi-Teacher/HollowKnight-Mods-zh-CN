using System;
using Hkmp.Api.Server;
using Hkmp.Menu;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable StringLiteralTypo

namespace Hkmp.Game.Settings;

/// <inheritdoc cref="IServerSettings" />
public class ServerSettings : IServerSettings, IEquatable<ServerSettings> {
    /// <inheritdoc />
    [SettingAlias("pvp")]
    [ModMenuSetting("玩家对战", "玩家之间是否可以互相造成伤害")]
    public bool IsPvpEnabled { get; set; }

    /// <inheritdoc />
    [SettingAlias("bodydamage")]
    [ModMenuSetting("身体碰撞伤害", "玩家对战时，玩家的碰撞体积是否会造成伤害")]
    public bool IsBodyDamageEnabled { get; set; } = true;

    /// <inheritdoc />
    [SettingAlias("globalmapicons")]
    [ModMenuSetting("全局地图图标", "始终为所有玩家显示地图图标")]
    public bool AlwaysShowMapIcons { get; set; }

    /// <inheritdoc />
    [SettingAlias("compassicon", "compassicons", "waywardicon", "waywardicons")]
    [ModMenuSetting("迷路指南针地图图标", "仅当装备迷路指南针时显示地图图标")]
    public bool OnlyBroadcastMapIconWithWaywardCompass { get; set; } = true;

    /// <inheritdoc />
    [SettingAlias("names")]
    [ModMenuSetting("显示姓名", "在玩家角色上方显示其姓名")]
    public bool DisplayNames { get; set; } = true;

    /// <inheritdoc />
    [SettingAlias("teams")]
    [ModMenuSetting("队伍", "玩家是否可以加入队伍")]
    public bool TeamsEnabled { get; set; }

    /// <inheritdoc />
    [SettingAlias("skins")]
    [ModMenuSetting("外观", "玩家是否可以使用外观")]
    public bool AllowSkins { get; set; } = true;

    /// <inheritdoc />
    [SettingAlias("parries")]
    [ModMenuSetting("格挡", "是否可以格挡某些玩家攻击")]
    public bool AllowParries { get; set; } = true;

    /// <inheritdoc />
    [SettingAlias("naildmg")]
    [ModMenuSetting("骨钉伤害", "玩家一次骨钉斩击造成的面具伤害数")]
    public byte NailDamage { get; set; } = 1;

    /// <inheritdoc />
    [SettingAlias("elegydmg")]
    [ModMenuSetting("蜕变挽歌伤害", "蜕变挽歌造成的面具伤害数")]
    public byte GrubberflyElegyDamage { get; set; } = 1;

    /// <inheritdoc />
    [SettingAlias("vsdmg", "fireballdamage", "fireballdmg")]
    [ModMenuSetting("复仇之魂伤害", "复仇之魂造成的面具伤害数")]
    public byte VengefulSpiritDamage { get; set; } = 1;

    /// <inheritdoc />
    [SettingAlias("shadesouldmg")]
    [ModMenuSetting("暗影之魂伤害", "暗影之魂造成的面具伤害数")]
    public byte ShadeSoulDamage { get; set; } = 2;

    /// <inheritdoc />
    [SettingAlias("desolatedivedmg", "ddivedmg")]
    [ModMenuSetting("荒芜俯冲伤害", "荒芜俯冲造成的面具伤害数")]
    public byte DesolateDiveDamage { get; set; } = 1;

    /// <inheritdoc />
    [SettingAlias("descendingdarkdmg", "ddarkdmg")]
    [ModMenuSetting("黑暗俯冲伤害", "黑暗俯冲造成的面具伤害数")]
    public byte DescendingDarkDamage { get; set; } = 2;

    /// <inheritdoc />
    [SettingAlias("howlingwraithsdamage", "howlingwraithsdmg", "wraithsdmg")]
    [ModMenuSetting("嚎叫幽灵伤害", "嚎叫幽灵造成的面具伤害数")]
    public byte HowlingWraithDamage { get; set; } = 1;

    /// <inheritdoc />
    [SettingAlias("abyssshriekdmg", "shriekdmg")]
    [ModMenuSetting("尖啸幽灵伤害", "尖啸幽灵造成的面具伤害数")]
    public byte AbyssShriekDamage { get; set; } = 2;

    /// <inheritdoc />
    [SettingAlias("greatslashdmg")]
    [ModMenuSetting("强力劈砍伤害", "强力劈砍造成的面具伤害数")]
    public byte GreatSlashDamage { get; set; } = 2;

    /// <inheritdoc />
    [SettingAlias("dashslashdmg")]
    [ModMenuSetting("冲刺劈砍伤害", "冲刺劈砍造成的面具伤害数")]
    public byte DashSlashDamage { get; set; } = 2;

    /// <inheritdoc />
    [SettingAlias("cycloneslashdmg", "cyclonedmg")]
    [ModMenuSetting("旋风劈砍伤害", "旋风劈砍造成的面具伤害数")]
    public byte CycloneSlashDamage { get; set; } = 1;

    /// <inheritdoc />
    [SettingAlias("sporeshroomdmg")]
    [ModMenuSetting("孢子菌菇伤害", "孢子菌菇毒云造成的面具伤害数")]
    public byte SporeShroomDamage { get; set; } = 1;

    /// <inheritdoc />
    [SettingAlias("sporedungshroomdmg", "dungshroomdmg")]
    [ModMenuSetting("孢子菌菇+防御者纹章伤害", "装备防御者纹章时孢子菌菇毒云造成的面具伤害数")]
    public byte SporeDungShroomDamage { get; set; } = 1;

    /// <inheritdoc />
    [SettingAlias("thornsofagonydamage", "thornsofagonydmg", "thornsdamage", "thornsdmg")]
    [ModMenuSetting("痛苦荆棘伤害", "痛苦荆棘反击造成的面具伤害数")]
    public byte ThornOfAgonyDamage { get; set; } = 1;

    /// <inheritdoc />
    [SettingAlias("sharpshadowdmg")]
    [ModMenuSetting("锋利之影伤害", "锋利之影暗影冲刺造成的面具伤害数")]
    public byte SharpShadowDamage { get; set; } = 1;

    /// <summary>
    /// Set all properties in this <see cref="ServerSettings"/> instance to the values from the given
    /// <see cref="ServerSettings"/> instance.
    /// </summary>
    /// <param name="serverSettings">The instance to copy from.</param>
    public void SetAllProperties(ServerSettings serverSettings) {
        // Use reflection to copy over all properties into this object
        foreach (var prop in GetType().GetProperties()) {
            if (!prop.CanRead || !prop.CanWrite) {
                continue;
            }

            prop.SetValue(this, prop.GetValue(serverSettings, null), null);
        }
    }

    /// <summary>
    /// Get a copy of this instance of the server settings.
    /// </summary>
    /// <returns>A new instance of the server settings with the same values as this instance.</returns>
    public ServerSettings GetCopy() {
        var serverSettings = new ServerSettings();
        serverSettings.SetAllProperties(this);

        return serverSettings;
    }

    /// <inheritdoc />
    public bool Equals(ServerSettings other) {
        if (ReferenceEquals(null, other)) {
            return false;
        }
    
        if (ReferenceEquals(this, other)) {
            return true;
        }
    
        foreach (var prop in GetType().GetProperties()) {
            if (!prop.CanRead) {
                continue;
            }
    
            if (prop.GetValue(this) != prop.GetValue(other)) {
                return false;
            }
        }
    
        return true;
    }
    
    /// <inheritdoc />
    public override bool Equals(object obj) {
        if (ReferenceEquals(null, obj)) {
            return false;
        }
    
        if (ReferenceEquals(this, obj)) {
            return true;
        }
    
        if (obj.GetType() != GetType()) {
            return false;
        }
    
        return Equals((ServerSettings) obj);
    }
    
    /// <inheritdoc />
    public override int GetHashCode() {
        unchecked {
            var hashCode = 0;
            var first = true;
            foreach (var prop in GetType().GetProperties()) {
                if (!prop.CanRead) {
                    continue;
                }
                
                var propHashCode = prop.GetValue(this).GetHashCode();
    
                if (first) {
                    hashCode = propHashCode;
                    first = false;
                    continue;
                }
    
                hashCode = (hashCode * 397) ^ propHashCode;
            }
    
            return hashCode;
        }
    }
    
    /// <summary>
    /// Indicates whether one <see cref="ServerSettings"/> is equal to another <see cref="ServerSettings"/>.
    /// </summary>
    /// <param name="left">The first <see cref="ServerSettings"/> to compare.</param>
    /// <param name="right">The second <see cref="ServerSettings"/> to compare.</param>
    /// <returns>true if <paramref name="left"/> is equal to <paramref name="right"/>; false otherwise.</returns>
    public static bool operator ==(ServerSettings left, ServerSettings right) {
        return Equals(left, right);
    }
    
    /// <summary>
    /// Indicates whether one <see cref="ServerSettings"/> is not equal to another <see cref="ServerSettings"/>.
    /// </summary>
    /// <param name="left">The first <see cref="ServerSettings"/> to compare.</param>
    /// <param name="right">The second <see cref="ServerSettings"/> to compare.</param>
    /// <returns>true if <paramref name="left"/> is not equal to <paramref name="right"/>; false otherwise.</returns>
    public static bool operator !=(ServerSettings left, ServerSettings right) {
        return !Equals(left, right);
    }
}
