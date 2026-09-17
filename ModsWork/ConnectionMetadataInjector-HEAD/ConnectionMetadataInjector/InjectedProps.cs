using ConnectionMetadataInjector.Util;
using ItemChanger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectionMetadataInjector
{
    /// <summary>
    /// A class containing built-in metadata properties
    /// </summary>
    public static class InjectedProps
    {
        /// <summary>
        /// A property representing the pool group of an item
        /// </summary>
        public static readonly MetadataProperty<AbstractItem, string> ItemPoolGroup = new("PoolGroup", GetDefaultItemPoolGroup);
        /// <summary>
        /// A property representing the location of a placement
        /// </summary>
        public static readonly MetadataProperty<AbstractPlacement, string> LocationPoolGroup = new("PoolGroup", GetDefaultLocationPoolGroup);
        /// <summary>
        /// A property representing the mod that defined a given taggable
        /// </summary>
        public static readonly MetadataProperty<TaggableObject, string?> ModSource = new("ModSource", _ => null);


        private static string GetDefaultItemPoolGroup(AbstractItem item)
        {
            return SubcategoryFinder.GetItemPoolGroup(item.name).FriendlyName();
        }

        private static string GetDefaultLocationPoolGroup(AbstractPlacement placement)
        {
            return SubcategoryFinder.GetLocationPoolGroup(placement.Name).FriendlyName();
        }

        /// <summary>
        /// Gets connection-provided (i.e. non-default) values for a metadata property
        /// </summary>
        /// <typeparam name="TObject">The type of object holding the metadata</typeparam>
        /// <typeparam name="TValue">The value type of the property</typeparam>
        /// <param name="objects">The objects to check for values</param>
        /// <param name="metadataSelector">How to get metadata for a given object, usually one of the static methods of <see cref="SupplementalMetadata"/></param>
        /// <param name="prop">The property to get values for</param>
        /// <returns>A set of unique values provided by connections</returns>
        public static HashSet<TValue> GetConnectionProvidedValues<TObject, TValue>(
            IEnumerable<TObject> objects,
            Func<TObject, SupplementalMetadata<TObject>> metadataSelector,
            IMetadataProperty<TObject, TValue> prop) where TObject : TaggableObject
        {
            return new HashSet<TValue>(objects
                .Select(item => metadataSelector(item))
                .Where(md => md.IsNonDefault(prop))
                .Select(md => md.Get(prop))
            );
        }
    }
}
