using Newtonsoft.Json;
using DiscordBot.Interfaces;

namespace DiscordBot.Models
{
    public class GuildModel : IServer
    {
        [JsonProperty("Prefix")]
        public string Prefix { get; set; } = "!";
    }
}