using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Fmod5Sharp.Util;

[JsonSerializable(typeof(Dictionary<uint, FmodVorbisData>))]
[GeneratedCode("System.Text.Json.SourceGeneration", "6.0.6.26707")]
internal class Fmod5SharpJsonContext : JsonSerializerContext
{
	private JsonTypeInfo<uint>? _UInt32;

	private JsonTypeInfo<byte[]>? _ByteArray;

	private JsonTypeInfo<int>? _Int32;

	private JsonTypeInfo<bool>? _Boolean;

	private JsonTypeInfo<FmodVorbisData>? _FmodVorbisData;

	private JsonTypeInfo<Dictionary<uint, FmodVorbisData>>? _DictionaryUInt32FmodVorbisData;

	private static Fmod5SharpJsonContext? s_defaultContext;

	private static readonly JsonEncodedText PropName_headerBytes = JsonEncodedText.Encode("headerBytes");

	private static readonly JsonEncodedText PropName_seekBit = JsonEncodedText.Encode("seekBit");

	public JsonTypeInfo<uint> UInt32
	{
		get
		{
			if (_UInt32 == null)
			{
				JsonConverter runtimeProvidedCustomConverter;
				if (base.Options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(typeof(uint))) != null)
				{
					_UInt32 = JsonMetadataServices.CreateValueInfo<uint>(base.Options, runtimeProvidedCustomConverter);
				}
				else
				{
					_UInt32 = JsonMetadataServices.CreateValueInfo<uint>(base.Options, JsonMetadataServices.UInt32Converter);
				}
			}
			return _UInt32;
		}
	}

	public JsonTypeInfo<byte[]> ByteArray
	{
		get
		{
			if (_ByteArray == null)
			{
				JsonConverter runtimeProvidedCustomConverter;
				if (base.Options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(typeof(byte[]))) != null)
				{
					_ByteArray = JsonMetadataServices.CreateValueInfo<byte[]>(base.Options, runtimeProvidedCustomConverter);
				}
				else
				{
					_ByteArray = JsonMetadataServices.CreateValueInfo<byte[]>(base.Options, JsonMetadataServices.ByteArrayConverter);
				}
			}
			return _ByteArray;
		}
	}

	public JsonTypeInfo<int> Int32
	{
		get
		{
			if (_Int32 == null)
			{
				JsonConverter runtimeProvidedCustomConverter;
				if (base.Options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(typeof(int))) != null)
				{
					_Int32 = JsonMetadataServices.CreateValueInfo<int>(base.Options, runtimeProvidedCustomConverter);
				}
				else
				{
					_Int32 = JsonMetadataServices.CreateValueInfo<int>(base.Options, JsonMetadataServices.Int32Converter);
				}
			}
			return _Int32;
		}
	}

	public JsonTypeInfo<bool> Boolean
	{
		get
		{
			if (_Boolean == null)
			{
				JsonConverter runtimeProvidedCustomConverter;
				if (base.Options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(typeof(bool))) != null)
				{
					_Boolean = JsonMetadataServices.CreateValueInfo<bool>(base.Options, runtimeProvidedCustomConverter);
				}
				else
				{
					_Boolean = JsonMetadataServices.CreateValueInfo<bool>(base.Options, JsonMetadataServices.BooleanConverter);
				}
			}
			return _Boolean;
		}
	}

	public JsonTypeInfo<FmodVorbisData> FmodVorbisData
	{
		get
		{
			if (_FmodVorbisData == null)
			{
				JsonConverter runtimeProvidedCustomConverter;
				if (base.Options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(typeof(FmodVorbisData))) != null)
				{
					_FmodVorbisData = JsonMetadataServices.CreateValueInfo<FmodVorbisData>(base.Options, runtimeProvidedCustomConverter);
				}
				else
				{
					JsonObjectInfoValues<FmodVorbisData> objectInfo = new JsonObjectInfoValues<FmodVorbisData>
					{
						ObjectCreator = null,
						ObjectWithParameterizedConstructorCreator = (object[] args) => new FmodVorbisData((byte[])args[0], (int)args[1]),
						PropertyMetadataInitializer = FmodVorbisDataPropInit,
						ConstructorParameterMetadataInitializer = FmodVorbisDataCtorParamInit,
						NumberHandling = JsonNumberHandling.Strict,
						SerializeHandler = FmodVorbisDataSerializeHandler
					};
					_FmodVorbisData = JsonMetadataServices.CreateObjectInfo(base.Options, objectInfo);
				}
			}
			return _FmodVorbisData;
		}
	}

	public JsonTypeInfo<Dictionary<uint, FmodVorbisData>> DictionaryUInt32FmodVorbisData
	{
		get
		{
			if (_DictionaryUInt32FmodVorbisData == null)
			{
				JsonConverter runtimeProvidedCustomConverter;
				if (base.Options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(typeof(Dictionary<uint, FmodVorbisData>))) != null)
				{
					_DictionaryUInt32FmodVorbisData = JsonMetadataServices.CreateValueInfo<Dictionary<uint, FmodVorbisData>>(base.Options, runtimeProvidedCustomConverter);
				}
				else
				{
					JsonCollectionInfoValues<Dictionary<uint, FmodVorbisData>> collectionInfo = new JsonCollectionInfoValues<Dictionary<uint, FmodVorbisData>>
					{
						ObjectCreator = () => new Dictionary<uint, FmodVorbisData>(),
						KeyInfo = UInt32,
						ElementInfo = FmodVorbisData,
						NumberHandling = JsonNumberHandling.Strict,
						SerializeHandler = null
					};
					_DictionaryUInt32FmodVorbisData = JsonMetadataServices.CreateDictionaryInfo<Dictionary<uint, FmodVorbisData>, uint, FmodVorbisData>(base.Options, collectionInfo);
				}
			}
			return _DictionaryUInt32FmodVorbisData;
		}
	}

	private static JsonSerializerOptions s_defaultOptions { get; } = new JsonSerializerOptions
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.Never,
		IgnoreReadOnlyFields = false,
		IgnoreReadOnlyProperties = false,
		IncludeFields = false,
		WriteIndented = false
	};

	public static Fmod5SharpJsonContext Default => s_defaultContext ?? (s_defaultContext = new Fmod5SharpJsonContext(new JsonSerializerOptions(s_defaultOptions)));

	protected override JsonSerializerOptions? GeneratedSerializerOptions { get; } = s_defaultOptions;

	private static JsonPropertyInfo[] FmodVorbisDataPropInit(JsonSerializerContext context)
	{
		Fmod5SharpJsonContext fmod5SharpJsonContext = (Fmod5SharpJsonContext)context;
		JsonSerializerOptions options = context.Options;
		JsonPropertyInfo[] array = new JsonPropertyInfo[4];
		JsonPropertyInfoValues<byte[]> propertyInfo = new JsonPropertyInfoValues<byte[]>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(FmodVorbisData),
			PropertyTypeInfo = fmod5SharpJsonContext.ByteArray,
			Converter = null,
			Getter = (object obj) => ((FmodVorbisData)obj).HeaderBytes,
			Setter = delegate(object obj, byte[]? value)
			{
				((FmodVorbisData)obj).HeaderBytes = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "HeaderBytes",
			JsonPropertyName = "headerBytes"
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<int> propertyInfo2 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(FmodVorbisData),
			PropertyTypeInfo = fmod5SharpJsonContext.Int32,
			Converter = null,
			Getter = (object obj) => ((FmodVorbisData)obj).SeekBit,
			Setter = delegate(object obj, int value)
			{
				((FmodVorbisData)obj).SeekBit = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "SeekBit",
			JsonPropertyName = "seekBit"
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<byte[]> propertyInfo3 = new JsonPropertyInfoValues<byte[]>
		{
			IsProperty = true,
			IsPublic = false,
			IsVirtual = false,
			DeclaringType = typeof(FmodVorbisData),
			PropertyTypeInfo = fmod5SharpJsonContext.ByteArray,
			Converter = null,
			Getter = null,
			Setter = null,
			IgnoreCondition = JsonIgnoreCondition.Always,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "BlockFlags",
			JsonPropertyName = null
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<bool> propertyInfo4 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = false,
			IsPublic = false,
			IsVirtual = false,
			DeclaringType = typeof(FmodVorbisData),
			PropertyTypeInfo = fmod5SharpJsonContext.Boolean,
			Converter = null,
			Getter = null,
			Setter = null,
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "_initialized",
			JsonPropertyName = null
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		return array;
	}

	private static void FmodVorbisDataSerializeHandler(Utf8JsonWriter writer, FmodVorbisData? value)
	{
		if (value == null)
		{
			writer.WriteNullValue();
			return;
		}
		writer.WriteStartObject();
		writer.WriteBase64String(PropName_headerBytes, value.HeaderBytes);
		writer.WriteNumber(PropName_seekBit, value.SeekBit);
		writer.WriteEndObject();
	}

	private static JsonParameterInfoValues[] FmodVorbisDataCtorParamInit()
	{
		return new JsonParameterInfoValues[2]
		{
			new JsonParameterInfoValues
			{
				Name = "headerBytes",
				ParameterType = typeof(byte[]),
				Position = 0,
				HasDefaultValue = false,
				DefaultValue = null
			},
			new JsonParameterInfoValues
			{
				Name = "seekBit",
				ParameterType = typeof(int),
				Position = 1,
				HasDefaultValue = false,
				DefaultValue = 0
			}
		};
	}

	public Fmod5SharpJsonContext()
		: base(null)
	{
	}

	public Fmod5SharpJsonContext(JsonSerializerOptions options)
		: base(options)
	{
	}

	private JsonConverter? GetRuntimeProvidedCustomConverter(Type type)
	{
		IList<JsonConverter> converters = base.Options.Converters;
		for (int i = 0; i < converters.Count; i++)
		{
			JsonConverter jsonConverter = converters[i];
			if (!jsonConverter.CanConvert(type))
			{
				continue;
			}
			if (jsonConverter is JsonConverterFactory jsonConverterFactory)
			{
				jsonConverter = jsonConverterFactory.CreateConverter(type, base.Options);
				if (jsonConverter == null || jsonConverter is JsonConverterFactory)
				{
					throw new InvalidOperationException($"The converter '{jsonConverterFactory.GetType()}' cannot return null or a JsonConverterFactory instance.");
				}
			}
			return jsonConverter;
		}
		return null;
	}

	public override JsonTypeInfo GetTypeInfo(Type type)
	{
		if (type == typeof(Dictionary<uint, FmodVorbisData>))
		{
			return DictionaryUInt32FmodVorbisData;
		}
		if (type == typeof(byte[]))
		{
			return ByteArray;
		}
		if (type == typeof(int))
		{
			return Int32;
		}
		return null;
	}
}
