using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace cemu_launcher
{
    public class Config
    {
        public string cemu_path { get; set; }
        public bool cemu_portable { get; set; }
        public string download_path { get; set; }
    }

    public class ConfigLoader
    {
        public static Config loadConfig()
        {
            try
            {
                string yml = File.ReadAllText("config.yml");

                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(UnderscoredNamingConvention.Instance)
                    .Build();

                return deserializer.Deserialize<Config>(yml);
            }
            catch
            {
                return defaultConfig();
            }
        }

        private static Config defaultConfig()
        {
            return new Config
            {
                cemu_path = "cemu",
                cemu_portable = true,
                download_path = "downloads"
            };
        }
    }
}
