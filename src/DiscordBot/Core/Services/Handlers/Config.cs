using Newtonsoft.Json;
using System;
using System.IO;

namespace DiscordBot.Handlers
{
    public sealed class Config
    {
        private Config() { }

        [JsonProperty("token")]
        public string Token { get; set; }

        public static Config Load()
        {
            if (File.Exists("files/config.json"))
            {
                var json = File.ReadAllText("files/config.json");
                return JsonConvert.DeserializeObject<Config>(json);
            }
            var config = new Config();
            config.Save();
            return config;
        }
        public static Config NewToken()
        {
            var config = new Config();
            config.Save();
            return config;
        }

        public void Save()
        {
            if (!Directory.Exists("files")) System.IO.Directory.CreateDirectory("files");
            Console.WriteLine("Please insert token");
            Token = Console.ReadLine();
            var json = JsonConvert.SerializeObject(this);
            File.WriteAllText("files/config.json", json);
        }
    }
}
