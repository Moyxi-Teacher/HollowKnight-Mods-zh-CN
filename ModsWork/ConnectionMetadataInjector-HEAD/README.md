# ConnectionMetadataInjector

A helper mod that allows Hollow Knight mods that are supplemental to Randomizer to request additional metadata information from connections, 
while allowing connections to specify this metadata without declaring hard dependencies.

## Tag spec

Data is provided by connections using an [InteropTag](https://homothetyhk.github.io/HollowKnight.ItemChanger/api/ItemChanger.Tags.InteropTag.html) (or any other IInteropTag
implementation of your creation, though usually InteropTag will be the easiest) with the message `"RandoSupplementalMetadata"`. These tags can most easily be provided when
creating custom items and locations in ItemChanger. Supplemental metadata can be declared on any taggable object, and supplemental mods can specify what data they would like
to have provided.

## Built-in property support

ConnectionMetadataInjector supports a handful of default metadata properties that are anticipated to be generally useful to most consumers of this mod.

| Property | Property type | Parent object type | Description | Default handling |
| -------- | ------------- | ------------------ | ----------- | ---------------- |
| `PoolGroup` | `string` | AbstractItem, AbstractLocation, or AbstractPlacement (usually item or location) | The human-readable pool group of an item or location with spaces and punctuation, such as "Relics" or "Lifeblood Cocoons". This allows small pools to be aggregated up into larger logical groups, i.e. treating "Swim" as part of the "Skills" group rather than the "Swim" group. | For items, attempts to find the split group name in PoolDefs. For locations, attempts to infer the vanilla item name from the location name with standard naming conventions (i.e. `Item_Name-Unique_Location`), then attempts to find the split group name of the item in PoolDefs |
| `ModSource` | `string?` | TaggableObject | The name of the mod that provided the metadata | `null` |

## Property API

Supplemental mods may request additional metadata information on the tags as specified above. Information can then be retrieved using the `SupplementalMetadata` and `MetadataProperty`
classes. The workflow to do this is as follows:

1. Declare a `MetadataProperty<TObject, TValue>` somewhere in your code (probably as an instance property of your mod class so you can access it with `YourMod.Instance`).
    * `TObject` is the type of the object that is expected hold the metadata. It must be a `TaggableObject`. Usually, you will want this to be `AbstractItem` for items, and `AbstractPlacement`
    for locations (more on this later).
    * `TValue` is the expected type of the property value. Note that as this will be a tag property, you'll want it to be serializable, and you probably don't want it to be a class you define
    in most cases, since that would mean that connections would need a dependency on you to reference the class - while soft dependencies are possible, this is a bit of an inconvenience if
    it can be avoided. If you need complex structured data, I'd instead recommend requesting a `Dictionary<string, object>` which can be nested, and validating it once received.
    * A `MetadataProperty` defines the property name that should appear on the tag, and default handling if the data isn't provided
2. Read the metadata of a taggable using `SupplementalMetadata.Of<TObject>(TObject)` or `SupplementalMetadata.OfPlacementAndLocations(AbstractPlacement)`. These methods will find the
   metadata tag of the provided object if it exists and convert it to a type-enforced representation that can read metadata properties. Using `SupplementalMetadata.Of<TObject>` searches
   only the tags of the object provided. This is fine for items, but it's difficult for us to get access to location instances, and difficult for connections to place tags on locations. 
   To remediate this, use `SupplementalMetadata.OfPlacementAndLocations` instead, which defines the supplemental metadata of a placement via the underlying tags on its locations.
3. Call `SupplementalMetadata.Get(MetadataProperty)` to get the value of the specified property on the created metadata. This does the following for you:
    * At compile time:
      * Verifies you're getting the property off the correct object type, i.e. if a property is defined for an item only, you can't accidentally try to get it for a location.
      * Generically returns the correct value type, eliminating any need for casting.
    * At run time:
      * Returns the value from the tag if the tag provides the requested value.
      * Returns the default value if the tag is not provided.
      * Returns the default value if the tag is provided, but the requested property (of the requested type) is not provided.
