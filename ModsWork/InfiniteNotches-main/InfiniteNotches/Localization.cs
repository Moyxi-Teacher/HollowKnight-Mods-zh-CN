using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace InfiniteNotches
{
    [DataContract]
    internal sealed class Localization
    {
        private const string DefaultName = "无限护符槽";
        private const string DefaultDescription = "突破护符槽数量限制，让你自由搭配更多护符。";
        private const string DefaultEnabled = "开启";
        private const string DefaultDisabled = "关闭";

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "enabled")]
        public string Enabled { get; set; }

        [DataMember(Name = "disabled")]
        public string Disabled { get; set; }

        [IgnoreDataMember]
        public string Error { get; private set; }

        public static Localization Load()
        {
            var localization = new Localization
            {
                Name = DefaultName,
                Description = DefaultDescription,
                Enabled = DefaultEnabled,
                Disabled = DefaultDisabled
            };

            try
            {
                var assemblyPath = typeof(Localization).Assembly.Location;
                var assemblyDirectory = Path.GetDirectoryName(assemblyPath);
                var filePath = Path.Combine(assemblyDirectory ?? string.Empty, "zh.json");

                using (var stream = File.OpenRead(filePath))
                {
                    var serializer = new DataContractJsonSerializer(typeof(Localization));
                    var loaded = serializer.ReadObject(stream) as Localization;
                    if (loaded != null)
                    {
                        localization.Name = UseValue(loaded.Name, DefaultName);
                        localization.Description = UseValue(loaded.Description, DefaultDescription);
                        localization.Enabled = UseValue(loaded.Enabled, DefaultEnabled);
                        localization.Disabled = UseValue(loaded.Disabled, DefaultDisabled);
                    }
                }
            }
            catch (FileNotFoundException exception)
            {
                localization.Error = exception.Message;
            }
            catch (DirectoryNotFoundException exception)
            {
                localization.Error = exception.Message;
            }
            catch (IOException exception)
            {
                localization.Error = exception.Message;
            }
            catch (SerializationException exception)
            {
                localization.Error = exception.Message;
            }
            catch (UnauthorizedAccessException exception)
            {
                localization.Error = exception.Message;
            }

            return localization;
        }

        private static string UseValue(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }
    }
}
