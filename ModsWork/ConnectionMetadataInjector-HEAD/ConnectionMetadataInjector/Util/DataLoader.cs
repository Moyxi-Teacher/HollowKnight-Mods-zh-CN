using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ConnectionMetadataInjector.Util
{
    internal static class DataLoader
    {
        public static readonly IReadOnlyDictionary<string, PoolGroup> ItemGroupLookup;
        public static readonly IReadOnlyDictionary<string, PoolGroup> LocationGroupLookup;

        static DataLoader()
        {
            using Stream items = typeof(DataLoader).Assembly.GetManifestResourceStream("ConnectionMetadataInjector.Resources.itemGroups.json");
            using StreamReader itemReader = new(items);
            ItemGroupLookup = JsonConvert.DeserializeObject<Dictionary<string, PoolGroup>>(itemReader.ReadToEnd()) ?? new();

            using Stream locations = typeof(DataLoader).Assembly.GetManifestResourceStream("ConnectionMetadataInjector.Resources.locationGroups.json");
            using StreamReader locationReader = new(locations);
            LocationGroupLookup = JsonConvert.DeserializeObject<Dictionary<string, PoolGroup>>(locationReader.ReadToEnd()) ?? new();
        }
    }
}
