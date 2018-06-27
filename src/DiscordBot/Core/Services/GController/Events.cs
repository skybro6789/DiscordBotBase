using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.GController
{
    class Events
    {
        internal static async Task UserJoinedAsync(SocketGuildUser user)
        {
            await user.SendMessageAsync($"Welcome {user.Username} to {user.Guild.Name}!");
        }
    }
}
