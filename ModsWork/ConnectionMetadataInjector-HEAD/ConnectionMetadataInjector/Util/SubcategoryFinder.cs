using Modding;

namespace ConnectionMetadataInjector.Util
{
    /// <summary>
    /// Utility class to find subcategories for named rando items and locations with a sensible default value when none could be inferred.
    /// Some of these are used as default implementations of some built-in metadata properties.
    /// </summary>
    public static class SubcategoryFinder
    {
        private static readonly Loggable log = ScopedLoggers.GetLogger();

        /// <summary>
        /// A catchall subcategory to use as a default value when none could be found
        /// </summary>
        public const string OTHER = "Other";

        /// <summary>
        /// Gets the pool group of a rando item by its name. You probably want to be using <see cref="InjectedProps.ItemPoolGroup"/> to handle custom items
        /// if you're trying to get data for an IC item with defined rando data
        /// </summary>
        /// <param name="item">The item to check</param>
        public static PoolGroup GetItemPoolGroup(string item)
        {
            if (DataLoader.ItemGroupLookup.TryGetValue(item, out PoolGroup p))
            {
                return p;
            }

            log.LogFine($"{item} not found in item pool data");
            return PoolGroup.Other;
        }

        /// <summary>
        /// Gets the pool group of a rando location. You probably want to be using <see cref="InjectedProps.LocationPoolGroup"/> to handle custom locations.
        /// if you're trying to get data for an IC placement or location with defined rando data
        /// </summary>
        /// <param name="location">The location to check</param>
        public static PoolGroup GetLocationPoolGroup(string location)
        {
            if (DataLoader.LocationGroupLookup.TryGetValue(location, out PoolGroup p))
            {
                return p;
            }

            log.LogFine($"{location} not found in location pool data");
            return PoolGroup.Other;
        }
    }
}
