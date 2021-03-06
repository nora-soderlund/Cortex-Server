using System;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;
using Discord.Net;
using Discord.Commands;

using MySql.Data.MySqlClient;

using Server.Events;
using Server.Socket;
using Server.Socket.Clients;

namespace Server.Discord.Sandbox {
    class DiscordRCON {
        public readonly DiscordSocketClient Client;

        public DiscordRCON(DiscordSocketClient client) {
            Client = client;

            Client.MessageReceived += OnMessageReceived;
        }

        public async Task OnMessageReceived(SocketMessage message) {
            if(message.Channel.Id != 816275048595718153)
                return;
                
            if(message.Author.Id == Client.CurrentUser.Id)
                return;

            if(message.Content.Length == 0)
                return;

            if(message.Content[0] != '!')
                return;
            
            try {
                string[] parameters = message.Content.Split(' ');

                if(parameters[0] == "!users") {
                    EmbedBuilder embed = new EmbedBuilder() {
                        Color = Color.DarkRed,

                        Title = Program.Socket.clients.Count + " users online!"
                    };

                    foreach(SocketClient client in Program.Socket.clients) {
                        EmbedFieldBuilder field = new EmbedFieldBuilder();

                        field.WithName(client.User.Name + " (" + client.User.Id + ")");

                        if(client.User.Room != null)
                            field.WithValue(client.User.Room.Title);

                        embed.AddField(field);
                        //embed.Description += "\r\n" + client.GetPort() + "\t" + client.User.Name + "\t" + ((client.User.Room != null)?());
                    }

                    await message.Channel.SendMessageAsync("", false, embed.Build());
                }
            }
            catch(Exception exception) {
                await message.Channel.SendMessageAsync("", false, new EmbedBuilder() {
                    Title = ":x: " + exception.Message,

                    Description = exception.StackTrace,

                    Color = Color.DarkRed
                }.Build());

                await message.Channel.SendMessageAsync("Check it out, <@!614863575126638641>!");
            }
        }
    }
}
