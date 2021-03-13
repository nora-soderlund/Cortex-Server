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

namespace Server.Discord.Sandbox {
    class DiscordSandbox {
        public readonly DiscordSocketClient Client;

        public DiscordSandbox(DiscordSocketClient client) {
            Client = client;

            Client.MessageReceived += OnMessageReceived;
        }

        public long Register(SocketUser author, string name) {
            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("INSERT INTO users (name, discord) VALUES (@name, @discord)", connection)) {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@discord", author.Id);

                    command.ExecuteNonQuery();

                    return command.LastInsertedId;
                }
            }
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

                if(parameters[0] == "!session") {
                    long user = 0;

                    string name = message.Author.Username;

                    if(parameters.Length == 2)
                        name = parameters[1];

                    using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                        connection.Open();

                        using(MySqlCommand command = new MySqlCommand("SELECT * FROM users WHERE name = @name AND (discord IS NULL OR discord != @discord)", connection)) {
                            command.Parameters.AddWithValue("@name", name);
                            command.Parameters.AddWithValue("@discord", message.Author.Id);

                            using(MySqlDataReader reader = command.ExecuteReader()) {
                                if(reader.Read()) {
                                    await message.Channel.SendMessageAsync("", false, new EmbedBuilder() {
                                        Title = ":x: Another user named " + name + " already exists!",
                                        Description = "This user is not associated with your Discord, please use !session [name] to continue with another name",

                                        Color = Color.DarkRed
                                    }.Build());

                                    return;
                                }
                            }
                        }

                        using(MySqlCommand command = new MySqlCommand("SELECT * FROM users WHERE discord = @discord", connection)) {
                            command.Parameters.AddWithValue("@discord", message.Author.Id);

                            using(MySqlDataReader reader = command.ExecuteReader()) {
                                if(!reader.Read())
                                    user = Register(message.Author, name);
                                else
                                    user = reader.GetInt64("id");
                            }
                        }
                    }

                    if(user == 0) {
                        await message.Channel.SendMessageAsync("", false, new EmbedBuilder() {
                            Title = ":x: Could not create or fetch a user?!",

                            Color = Color.DarkRed
                        }.Build());

                        await message.Channel.SendMessageAsync("Check it out, <@!614863575126638641>!");

                        return;
                    }

                    string key = Guid.NewGuid().ToString();

                    using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                        connection.Open();

                        using(MySqlCommand command = new MySqlCommand("DELETE FROM user_keys WHERE user = @user", connection)) {
                            command.Parameters.AddWithValue("@user", user);

                            command.ExecuteNonQuery();
                        }

                        using(MySqlCommand command = new MySqlCommand("INSERT INTO user_keys (user, `key`, address) VALUES (@user, @key, 'Discord')", connection)) {
                            command.Parameters.AddWithValue("@user", user);
                            command.Parameters.AddWithValue("@key", key);

                            command.ExecuteNonQuery();
                        }
                    }

                    try {
                        await message.Author.SendMessageAsync("", false, new EmbedBuilder() {
                            Title = ":white_check_mark: Single-Sign-On Key",

                            Description = "https://sandbox.project-cortex.net/" + key + "\r\nMake sure to not share this key with anyone as it will easily be compromised!",

                            Color = Color.DarkRed
                        }.Build());

                        await message.Channel.SendMessageAsync("", false, new EmbedBuilder() {
                            Title = ":white_check_mark: I've sent you your personal key in your direct-messages!",

                            Color = Color.DarkRed
                        }.Build());
                    }
                    catch(Exception) {
                        await message.Channel.SendMessageAsync("", false, new EmbedBuilder() {
                            Title = ":x: I was unable to send you a message!",
                            Description = "Make sure your direct-messages are turned on! You can try messaging me for a riddle (:",

                            Color = Color.DarkRed
                        }.Build());
                    }
                }
            }
            catch(Exception exception) {
                Program.Exception(exception);
            }
        }
    }
}
