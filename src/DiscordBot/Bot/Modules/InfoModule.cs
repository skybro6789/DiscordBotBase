using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Handlers;

namespace DiscordBot.Modules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {//commands galore (maybe make more cs files tho?)
        [Command("Prefix", RunMode = RunMode.Async), Summary("Sets guild prefix"), Remarks("Prefix .")]
        [RequireUserPermission(GuildPermission.ManageChannels)]
        public async Task SetPrefixAsync(string prefix)
        {
            var Guild = Context.Guild as SocketGuild;
            var gldConfig = GuildHandler.GuildConfigs[Guild.Id];
            gldConfig.Prefix = prefix;
            GuildHandler.GuildConfigs[Context.Guild.Id] = gldConfig;
            await GuildHandler.SaveAsync(GuildHandler.GuildConfigs);
            await ReplyAsync($"Guild Prefix has been set to: **{prefix}**");
        }

        [Command("Prefix", RunMode = RunMode.Async), Summary("show's guild prefix"), Remarks("Prefix")]
        public async Task PrefixAsync()
        {
            var Guild = Context.Guild as SocketGuild;
            var gldConfig = GuildHandler.GuildConfigs[Guild.Id];

            await ReplyAsync($"Guild Prefix is: **{gldConfig.Prefix}**");
        }

        [Command("info", RunMode = RunMode.Async), Summary("show's some info"), Remarks("Info")]
        public async Task Info()
        {
            Random rand = new Random();
            int R = rand.Next(256);
            int G = rand.Next(256);
            int B = rand.Next(256);
            var em = new EmbedBuilder();
            em.WithAuthor(new EmbedAuthorBuilder().WithName($"{Context.Client.CurrentUser.Username} Info:").WithIconUrl(Context.Client.CurrentUser.GetAvatarUrl()));
            em.WithColor(new Discord.Color(R, G, B));
            em.WithImageUrl(Context.Client.CurrentUser.GetAvatarUrl());
            em.WithCurrentTimestamp();
            em.AddField(new EmbedFieldBuilder().WithName("ID")
                .WithValue(Context.Client.CurrentUser.Id).WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("Discriminator")
                .WithValue(Context.Client.CurrentUser.Discriminator).WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("Status")
                .WithValue(Context.Client.CurrentUser.Status).WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("Current User")
                .WithValue(Context.Client.CurrentUser).WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("Mention")
                .WithValue(Context.Client.CurrentUser.Mention).WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("Connection State")
                .WithValue(Context.Client.ConnectionState).WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("Memory")
            .WithValue($"{Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2)} MB").WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("Uptime")
                .WithValue((DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss")).WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("API Library")
              .WithValue($"Discord.Net {DiscordConfig.Version}").WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("Created At")
                .WithValue(Context.Client.CurrentUser.CreatedAt).WithIsInline(true));
            await Context.Channel.TriggerTypingAsync();
            await ReplyAsync("", embed: em.Build());
        }
    }
}
