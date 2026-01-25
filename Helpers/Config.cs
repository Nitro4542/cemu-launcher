using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace cemu_launcher.Helpers
{
    public class Config
    {
        public bool ask_before_update { get; set; } = false;
        public string cemu_path { get; set; } = "cemu";
        public bool cemu_portable { get; set; } = true;
        public string download_path { get; set; } = "downloads";
    }

    public static class ConfigLoader
    {
        public static Config LoadConfig()
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
                return new Config();
            }
        }
    }
}
