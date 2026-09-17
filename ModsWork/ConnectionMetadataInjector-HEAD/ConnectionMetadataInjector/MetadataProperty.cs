using ItemChanger;
using ItemChanger.Tags;
using System;

namespace ConnectionMetadataInjector
{
    /// <summary>
    /// Represents a user-requested metadata property on a supplemental metadata interop tag
    /// </summary>
    /// <typeparam name="TObject">The type of object that this property is defined for (i.e. the type of object that holds the tag)</typeparam>
    /// <typeparam name="TValue">The type of the value of the property</typeparam>
    public class MetadataProperty<TObject, TValue> : IMetadataProperty<TObject, TValue>
        where TObject : TaggableObject
    {
        /// <inheritdoc/>
        public string Name { get; private init; }

        private readonly Func<TObject, TValue> getDefault;
        Func<TObject, TValue> IMetadataProperty<TObject, TValue>.GetDefault => getDefault;

        /// <summary>
        /// Declares a metadata property with a default value
        /// </summary>
        /// <param name="name">The property name</param>
        /// <param name="defaultValue">The default value to use if the tag is not present or doesn't have the requested property</param>
        public MetadataProperty(string name, TValue defaultValue)
        {
            Name = name;
            getDefault = _ => defaultValue;
        }

        /// <summary>
        /// Declares a metadata property with a dynamically defined default value based on the parent of the tag
        /// </summary>
        /// <param name="name">The property name</param>
        /// <param name="handleDefaultValue">The handler to discover the default value if the tag is not present or doesn't have the requested property</param>
        public MetadataProperty(string name, Func<TObject, TValue> handleDefaultValue)
        {
            Name = name;
            getDefault = handleDefaultValue;
        }

        bool IMetadataProperty<TObject, TValue>.TryGetValue(IInteropTag tag, out TValue? value)
        {
            return tag.TryGetProperty(Name, out value);
        }
    }
}
