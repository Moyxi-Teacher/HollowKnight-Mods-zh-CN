using ItemChanger;
using ItemChanger.Tags;
using System;

namespace ConnectionMetadataInjector.Conversion
{
    /// <summary>
    /// Represents a user-requested metadata property on a supplemental metadata interop tag
    /// </summary>
    /// <typeparam name="TObject">The type of object that this property is defined for (i.e. the type of object that holds the tag)</typeparam>
    /// <typeparam name="TSerialized">The type of the value when serialized for interoperability.</typeparam>
    /// <typeparam name="TValue">The final value type of the property after conversion.</typeparam>
    /// <remarks>
    ///     Note that there may be some jank when converting from any serialized nullable type to a non-nullable value type.
    ///     You probably shouldn't be doing that anyway though.
    /// </remarks>
    public class CastingMetadataProperty<TObject, TSerialized, TValue> : IMetadataProperty<TObject, TValue>
        where TObject : TaggableObject
    {
        /// <inheritdoc/>
        public string Name { get; }

        private readonly Func<TObject, TValue> getDefault;
        Func<TObject, TValue> IMetadataProperty<TObject, TValue>.GetDefault => getDefault;

        private readonly IConverter<TSerialized, TValue> converter;

        /// <summary>
        /// Declares a metadata property with a default value
        /// </summary>
        /// <param name="name">The property name</param>
        /// <param name="valueConverter">A converter which can convert from the serialized type</param>
        /// <param name="defaultValue">The default value to use if the tag is not present or doesn't have the requested property</param>
        public CastingMetadataProperty(string name, IConverter<TSerialized, TValue> valueConverter, 
            TValue defaultValue)
        {
            Name = name;
            converter = valueConverter;
            getDefault = _ => defaultValue;
        }

        /// <summary>
        /// Declares a metadata property with a dynamically defined default value based on the parent of the tag
        /// </summary>
        /// <param name="name">The property name</param>
        /// <param name="valueConverter">A converter which can convert from the serialized type</param>
        /// <param name="handleDefaultValue">The handler to discover the default value if the tag is not present or doesn't have the requested property</param>
        public CastingMetadataProperty(string name, IConverter<TSerialized, TValue> valueConverter,
            Func<TObject, TValue> handleDefaultValue)
        {
            Name = name;
            converter = valueConverter;
            getDefault = handleDefaultValue;
        }

        bool IMetadataProperty<TObject, TValue>.TryGetValue(IInteropTag tag, out TValue? value)
        {
            bool hasValue = tag.TryGetProperty(Name, out TSerialized? serialized);
            if (!hasValue)
            {
                value = default;
                return false;
            }

            value = serialized == null ? default : converter.Convert(serialized);
            return true;
        }
    }
}
