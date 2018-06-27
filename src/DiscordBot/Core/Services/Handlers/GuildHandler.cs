using Newtonsoft.Json;
using DiscordBot.Interfaces;
using DiscordBot.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace DiscordBot.Handlers
{
    public class GuildHandler
    {
        public static Dictionary<ulong, GuildModel> GuildConfigs { get; set; } = new Dictionary<ulong, GuildModel>();

        public const string configPath = "files/GuildConfig.json";

        public static async Task SaveAsync<T>(Dictionary<ulong, T> configs) where T : IServer
            => File.WriteAllText(configPath, await Task.Run(() => JsonConvert.SerializeObject(configs, Formatting.Indented)).ConfigureAwait(false));

        public static async Task<Dictionary<ulong, T>> LoadServerConfigsAsync<T>() where T : IServer, new()
        {
            if (File.Exists(configPath))
            {
                return JsonConvert.DeserializeObject<Dictionary<ulong, T>>(File.ReadAllText(configPath));
            }
            var newConfig = new Dictionary<ulong, T>();
            await SaveAsync(newConfig);
            return newConfig;
        }

        internal static async Task DeleteGuildConfig(SocketGuild Guild)
        {
            if (GuildHandler.GuildConfigs.ContainsKey(Guild.Id))
            {
                GuildHandler.GuildConfigs.Remove(Guild.Id);
            }
            await GuildHandler.SaveAsync(GuildHandler.GuildConfigs);
        }

        internal static async Task HandleGuildConfigAsync(SocketGuild Guild)
        {
            var CreateConfig = new GuildModel();
            if (!GuildHandler.GuildConfigs.ContainsKey(Guild.Id))
            {
                GuildHandler.GuildConfigs.Add(Guild.Id, CreateConfig);
            }
            await GuildHandler.SaveAsync(GuildHandler.GuildConfigs);
        }
    }
}