using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text.RegularExpressions;

namespace ConnectionMetadataInjector.Util
{
    /// <summary>
    /// PoolGroup is courtesy of Phenomenol: https://github.com/syyePhenomenol/HollowKnight.MapModS/blob/08f64d3454d8cdbc62773d44d20e6fbec08a9055/MapModS/Data/PoolGroup.cs
    /// These are the same as the "Groups" in RandomizerMod's data. See pools.json
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PoolGroup
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        // groups defined by rando
        Dreamers,
        Skills,
        Charms,
        Keys,
        MaskShards,
        VesselFragments,
        CharmNotches,
        PaleOre,
        GeoChests,
        RancidEggs,
        Relics,
        WhisperingRoots,
        BossEssence,
        Grubs,
        Mimics,
        Maps,
        Stags,
        LifebloodCocoons,
        GrimmkinFlames,
        JournalEntries,
        GeoRocks,
        BossGeo,
        SoulTotems,
        LoreTablets,

        // stuff that we have custom handling for
        Shops,
        Other
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// Extension methods providing additional behavior to <see cref="PoolGroup"/>s.
    /// </summary>
    public static class PoolExtensions
    {
        private static readonly Regex nameFormatter = new(@"([^A-Z])(?=[A-Z])");

        /// <summary>
        /// Gets the friendly name of the pool group, with spaces and necessary punctuation
        /// </summary>
        public static string FriendlyName(this PoolGroup group)
        {
            return nameFormatter.Replace(group.ToString(), "$1 ");
        }
    }
}
