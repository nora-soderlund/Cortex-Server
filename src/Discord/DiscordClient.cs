using System;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Discord;
using Discord.WebSocket;

using Server.Events;
using Server.Game.Users;
using Server.Game.Shop;
using Server.Game.Furnitures;

using Server.Discord.Sandbox;

using Server.Socket.Clients;

namespace Server.Discord {
    class DiscordClient {
        public readonly DiscordSocketClient Client;

        public DiscordSandbox Sandbox;

        public DiscordClient() {
            Client = new DiscordSocketClient();

            Client.Ready += ReadyAsync;

            Sandbox = new DiscordSandbox(Client);
            new DiscordRCON(Client);
        }

        public void Start() {
            MainAsync().GetAwaiter().GetResult();
        }

        public Task Exception(Exception exception) {
            SocketTextChannel channel = Client.GetGuild(Program.Config["discord"]["guild"].ToObject<ulong>()).GetTextChannel(Program.Config["discord"]["channels"]["logs-server"].ToObject<ulong>());

            channel.SendMessageAsync("", false, new EmbedBuilder() {
                Title = ":x: " + exception.Message,

                Description = exception.StackTrace,

                Color = Color.DarkRed
            }.Build());

            return Task.CompletedTask;
        }

        public Task Exception(SocketClient client, JToken data) {
            SocketTextChannel channel = Client.GetGuild(Program.Config["discord"]["guild"].ToObject<ulong>()).GetTextChannel(Program.Config["discord"]["channels"]["logs-client"].ToObject<ulong>());

            channel.SendMessageAsync("", false, new EmbedBuilder() {
                Title = data["exception"].ToString(),

                Description = data["stack"].ToString(),

                Footer = new EmbedFooterBuilder() {
                    Text = client.Connection.ConnectionInfo.Headers["Cache-Control"] + " " + client.Connection.ConnectionInfo.Headers["User-Agent"]
                },

                Color = Color.DarkRed
            }.Build());

            return Task.CompletedTask;
        }

        public Task Furniture(GameUser user, GameFurniture furniture, double depth) {
            SocketTextChannel channel = Client.GetGuild(Program.Config["discord"]["guild"].ToObject<ulong>()).GetTextChannel(Program.Config["discord"]["channels"]["logs-server"].ToObject<ulong>());

            channel.SendMessageAsync("", false, new EmbedBuilder() {
                Title = user.Name + " is making a furniture depth change!",

                Description = user.Name + " is changing HabboFurnitures/" + furniture.Line + "/" + furniture.Id + "'s depth from " + furniture.Dimension.Depth + " to " + depth,

                Color = Color.DarkRed
            }.Build());

            return Task.CompletedTask;
        }

        public Task Shop(GameUser user, GameShopPage shop, int icon) {
            SocketTextChannel channel = Client.GetGuild(Program.Config["discord"]["guild"].ToObject<ulong>()).GetTextChannel(Program.Config["discord"]["channels"]["logs-server"].ToObject<ulong>());

            channel.SendMessageAsync("", false, new EmbedBuilder() {
                Title = "HabboShop/" + shop.Title + " change!",

                Description = user.Name + " is changing HabboShop/" + shop.Title + "'s icon from " + shop.Icon + " to " + icon,

                Color = Color.DarkRed
            }.Build());

            return Task.CompletedTask;
        }

        public async Task MainAsync()
        {
            // Tokens should be considered secret data, and never hard-coded.
            await Client.LoginAsync(TokenType.Bot, Program.Config["discord"]["token"].ToString());
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
