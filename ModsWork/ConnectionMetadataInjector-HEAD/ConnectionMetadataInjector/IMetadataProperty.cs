using ItemChanger;
using ItemChanger.Tags;
using System;

namespace ConnectionMetadataInjector
{
    /// <summary>
    /// Contravariant interface definition for metadata properties
    /// </summary>
    /// <typeparam name="TObject">The type of taggable that this property is defined for</typeparam>
    /// <typeparam name="TValue">The value type of the property</typeparam>
    public interface IMetadataProperty<in TObject, TValue> where TObject : TaggableObject
    {
        /// <summary>
        /// The metadata property name, as expected in the metadata tag
        /// </summary>
        public string Name { get; }

        internal Func<TObject, TValue> GetDefault { get; }

        internal bool TryGetValue(IInteropTag tag, out TValue? value);
    }
}
