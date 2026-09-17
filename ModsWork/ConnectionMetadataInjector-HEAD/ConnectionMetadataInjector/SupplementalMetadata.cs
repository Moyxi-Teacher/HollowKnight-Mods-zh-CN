using ItemChanger;
using ItemChanger.Tags;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectionMetadataInjector
{
    /// <summary>
    /// Represents any randomizer-supplemental metadata on a taggable object
    /// </summary>
    /// <typeparam name="TObject">The type of object that will hold the data on its tag</typeparam>
    public class SupplementalMetadata<TObject> where TObject : TaggableObject
    {
        private readonly TObject obj;
        private readonly IEnumerable<IInteropTag> tags;

        internal SupplementalMetadata(TObject obj, IEnumerable<IInteropTag> tags)
        {
            this.obj = obj;
            this.tags = tags;
        }

        /// <summary>
        /// Gets whether the given property has a non-default value in this metadata
        /// </summary>
        /// <typeparam name="TValue">The type of the property's value</typeparam>
        /// <param name="property">The property to check the value of</param>
        /// <returns>
        /// True, if this object contains a supplmental metadata tag with the requested property.
        /// Otherwise, false.
        /// </returns>
        public bool IsNonDefault<TValue>(IMetadataProperty<TObject, TValue> property)
        {
            return tags.Any(t => t.TryGetProperty(property.Name, out TValue _));
        }

        /// <summary>
        /// Gets the value of a property in this metadata
        /// </summary>
        /// <typeparam name="TValue">The type of the property's value</typeparam>
        /// <param name="property">The property to get the value of</param>
        /// <returns>
        /// The value of the property, if this object contains a supplemental metadata tag with the requested property.
        /// Otherwise, returns the default value of the property for this object.
        /// </returns>
        public TValue Get<TValue>(IMetadataProperty<TObject, TValue> property)
        {
            // keeping as a tuple lets us distinguish between our query failing to return a result vs the provided value being explicitly null
            // default(T) for a value tuple is the normal struct you'd expect to see, ie it would make hasValue false when we expect it to be true
            (bool hasValue, TValue? value) result = tags.Select(t => (property.TryGetValue(t, out TValue? value), value))
                .Where(r => r.Item1)
                .FirstOrDefault();
            if (result.hasValue)
            {
                return result.value!;
            }
            else
            {
                return property.GetDefault(obj);
            }
        }
    }

    /// <summary>
    /// Represents randomizer-supplemental metadata on a taggable object
    /// </summary>
    public static class SupplementalMetadata
    {
        /// <summary>
        /// The tag message to specify on interop tags describing supplemental metadata
        /// </summary>
        public const string InteropTagMessage = "RandoSupplementalMetadata";

        /// <summary>
        /// Gets the supplemental metadata of an object, and if it's a placement, its underlying locations as well.
        /// </summary>
        /// <typeparam name="TObject">The type of the object; usually inferred</typeparam>
        /// <param name="obj">The object</param>
        public static SupplementalMetadata<TObject> Of<TObject>(TObject obj) where TObject : TaggableObject
        {
            IEnumerable<IInteropTag> interopTags;
            if (obj is AbstractPlacement plt)
            {
                interopTags = plt.GetPlacementAndLocationTags().OfType<IInteropTag>();
            }
            else
            {
                interopTags = obj.GetTags<IInteropTag>();
            }
            return new SupplementalMetadata<TObject>(obj, interopTags.Where(t => t.Message == InteropTagMessage));
        }

        /// <summary>
        /// Gets the supplemental metadata of a placement and its underlying location(s). If you were thinking of using
        /// <see cref="Of{TObject}(TObject)"/> to get information about an <see cref="AbstractLocation"/>, you probably want this instead.
        /// </summary>
        /// <param name="plt">The placement</param>
        [Obsolete("The default behavior SupplementalMetadata.Of for placements has changed to do this, use that instead.", true)]
        public static SupplementalMetadata<AbstractPlacement> OfPlacementAndLocations(AbstractPlacement plt)
        {
            IEnumerable<IInteropTag> metaTags = plt.GetPlacementAndLocationTags().OfType<IInteropTag>().Where(t => t.Message == InteropTagMessage);
            return new SupplementalMetadata<AbstractPlacement>(plt, metaTags);
        }

        /// <summary>
        /// Gets the supplemental metadata of a placement only (and not its underlying locations).
        /// </summary>
        /// <param name="plt">The placement</param>
        public static SupplementalMetadata<AbstractPlacement> OfPlacementOnly(AbstractPlacement plt)
        {
            IEnumerable<IInteropTag> metaTags = plt.GetTags<IInteropTag>().Where(t => t.Message == InteropTagMessage);
            return new SupplementalMetadata<AbstractPlacement>(plt, metaTags);
        }
    }
}
