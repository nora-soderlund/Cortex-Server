using System;
using System.Threading;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using Server.Events;

using Server.Discord.Sandbox;

namespace Server.Discord {
    class DiscordClient {
        public readonly DiscordSocketClient Client;

        public DiscordSandbox Sandbox;

        public DiscordClient() {
            Client = new DiscordSocketClient();

            Client.Ready += ReadyAsync;

            Sandbox = new DiscordSandbox(Client);
        }

        public void Start() {
            MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            // Tokens should be considered secret data, and never hard-coded.
            await Client.LoginAsync(TokenType.Bot, "NjE0ODY0MDMxOTQ1MzI2NTk1.XWFqwA.qx40zdPvjsovWu3j5l_RWd__Zmo");
            await Client.StartAsync();

            // Block the program until it is closed.
            await Task.Delay(Timeout.Infinite);
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"{Client.CurrentUser} is connected!");

            Client.SetGameAsync("with myself...");

            return Task.CompletedTask;
        }
    }
}
