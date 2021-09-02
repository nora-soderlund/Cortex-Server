using System;
using System.Linq;
using System.Timers;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

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
            if(message.Author.Id == 614864031945326595)
                return;

            SocketGuildUser guildUser = Client.GetGuild(713415610264191006).GetUser(message.Author.Id);

            if(message.Content.ToLower().Contains("tag")) {
                SocketRole role = guildUser.Guild.GetRole(854756677970034699);

                if(guildUser.Roles.Contains(role)) {
                    if(message.MentionedUsers.Count == 1) {
                        await guildUser.RemoveRoleAsync(role);
                        
                        foreach(var tag in message.MentionedUsers) {
                            SocketGuildUser mentionedGuildUser = Client.GetGuild(713415610264191006).GetUser(tag.Id);

                            await mentionedGuildUser.AddRoleAsync(role);

                            await message.Channel.SendMessageAsync("Tag! " + mentionedGuildUser.Mention + " is it!");
                        }
                    }
                }
            }

            if(message.Channel.Id != 816275048595718153 && message.Author.Id != 614863575126638641)
                return;
                
            if(message.Author.Id == Client.CurrentUser.Id)
                return;

            if(message.Content.Length == 0)
                return;

            if(message.Content[0] != '!')
                return;
            
            try {
                string[] parameters = message.Content.Split(' ');

                if(parameters[0] == "!say") {
                    await message.Channel.SendMessageAsync(message.Content.Substring(5));
                    
                    await message.DeleteAsync();
                }
                else if(parameters[0] == "!users") {
                    EmbedBuilder embed = new EmbedBuilder() {
                        Color = Color.DarkRed,

                        Title = "There is " + Program.Socket.clients.Count + " user" + ((Program.Socket.clients.Count != 1)?("s"):("")) + " online!"
                    };

                    foreach(SocketClient client in Program.Socket.clients) {

                        embed.Description += "\r\n**" + client.User.Name + "**";

                        if(client.User.Room != null)
                            embed.Description += "  in " + client.User.Room.Title;
                    }

                    await message.Channel.SendMessageAsync("", false, embed.Build());
                }
                else if(parameters[0] == "!remindme") {
                    if(parameters.Length < 2) {
                        await message.Channel.SendMessageAsync("", false, new EmbedBuilder() {
                            Title = "Missing command parameters!",
                            Description = "You must use !remindme [time] (message)",

                            Color = Color.DarkRed
                        }.Build());
                        
                        return;
                    }

                    int seconds = 0;

                    string time = parameters[1];

                    for(int character = 0, length = time.Length; character < length; character++) {
                        if(Char.ToUpper(time[character]) == 'S') {
                            seconds += Int32.Parse(time.Substring(0, character));

                            time = time.Substring(character, length - 1);
                            
                            character = 0;
                            length = time.Length;
                        }
                        else if(Char.ToUpper(time[character]) == 'M') {
                            seconds += Int32.Parse(time.Substring(0, character)) * 60;

                            time = time.Substring(character, length - 1);
                            
                            character = 0;
                            length = time.Length;
                        }
                        else if(Char.ToUpper(time[character]) == 'H') {
                            seconds += Int32.Parse(time.Substring(0, character)) * 60 * 60;

                            time = time.Substring(character, length - 1);
                            
                            character = 0;
                            length = time.Length;
                        }
                    }

                    List<string> reminder = parameters.ToList();

                    reminder.RemoveRange(0, 2);

                    System.Timers.Timer timer = new System.Timers.Timer(seconds);

                    timer.Elapsed += (a, b) => {
                        string content = "";

                        foreach(string msg in reminder)
                            content += msg + " ";

                        message.Channel.SendMessageAsync("Hey " + message.Author.Mention + "!\r\n> " + content);

                        timer.Stop();
                    };

                    timer.Start();
                    
                    await message.Channel.SendMessageAsync("Okay! I'll remind you in " + seconds + " seconds!");
                }
            }
            catch(Exception exception) {
                Console.Exception(exception);
            }
        }
    }
}
