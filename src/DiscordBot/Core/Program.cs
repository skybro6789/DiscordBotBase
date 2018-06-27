using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using System.Net;
using Discord.Net;
using DiscordBot.Handlers;
using DiscordBot.Models;
using DiscordBot.GController;
using System.Runtime.InteropServices;
using System.Threading;

namespace DiscordBot
{
    class Program
    {
        #region Application Termination
        static bool exitSystem = false;
        
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            Console.WriteLine($"Exiting system due to external {sig}");
            
            new Program().Outro().GetAwaiter().GetResult();

            Console.WriteLine("Cleanup complete");
            
            exitSystem = true;
            
            Environment.Exit(-1);

            return true;
        }
        public async Task Outro()
        {
            Console.Clear();
            const string introText = "Shutting down..";
            int left = Console.WindowWidth / 2 - introText.Length / 2;
            int top = Console.WindowHeight / 2 - 1;
            const ConsoleColor outroColor = ConsoleColor.Red;

            Console.SetCursorPosition(left, top);
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = outroColor;

            foreach (char ch in introText)
            {
                Console.Write(ch);
                Thread.Sleep(50);
            }

            Console.ForegroundColor = originalColor;

            Thread.Sleep(300);
        }

        static void Main(string[] args)
        {
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            bool success = false;
            int retry = 0;
            while (!success)
            {
                try
                {
                    new Program().MainAsync().GetAwaiter().GetResult();
                    success = true;
                }
                catch (Exception e)
                {
                    Console.Clear();
                    Console.WriteLine($"{e.Message}\nRetrying!\nTimes Tried: {retry+1}");
                    Thread.Sleep(500);
                    retry++;
                }
            }

            while (!exitSystem)
            {
                Thread.Sleep(500);
            }
        }
        #endregion


        #region Discord
        private WebClient WClient = new WebClient();
        private DiscordSocketClient _client;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            var services = ConfigureServices();
            services.GetRequiredService<LogService>();
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync(services);
            
            try
            {
                Config.Load();
                GuildHandler.GuildConfigs = await GuildHandler.LoadServerConfigsAsync<GuildModel>();
                await _client.LoginAsync(TokenType.Bot, Config.Load().Token);
                await _client.StartAsync();
            }
            catch (HttpException httpException)
            {
                if (httpException.HttpCode == HttpStatusCode.Unauthorized)
                {
                    Config.NewToken();
                    await _client.LoginAsync(TokenType.Bot, Config.Load().Token);
                    await _client.StartAsync();
                }
            }

            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddLogging()
                .AddSingleton<LogService>();

            var Provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);
            Provider.GetService<GuildHandler>();
            Provider.GetService<Events>();
            return Provider;
        }
        #endregion
    }
}