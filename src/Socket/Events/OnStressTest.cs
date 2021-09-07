using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

using Cortex.Server.Game.Badges;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Events;
using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Socket.Events {
    class OnStressTest : ISocketEvent {
        public string Event => "OnStressTest";

        public int Execute(SocketClient client, JToken data) {
            WriteUsers(client, data);
            WriteFigures(client, data);
            WriteFurnitures(client, data);

            client.Send(new SocketMessage("OnStressTest", true).Compose());

            return 1;
        }

        public async void WriteUsers(SocketClient client, JToken data) {
            string cache = client.Connection.ConnectionInfo.Headers["Cache-Control"];
            string agent = client.Connection.ConnectionInfo.Headers["User-Agent"];

            using MySqlConnection connection = new MySqlConnection(Program.ConnectionTest);
            await connection.OpenAsync();

            using MySqlCommand command = new MySqlCommand("INSERT INTO users (user, agent, cache, count, assets, downloads, sprites, timestamp) VALUES (@user, @agent, @cache, @count, @assets, @downloads, @sprites, @timestamp)", connection);
            command.Parameters.AddWithValue("@user", client.User.Id);
            command.Parameters.AddWithValue("@agent", agent);
            command.Parameters.AddWithValue("@cache", cache);
            command.Parameters.AddWithValue("@count", data["count"]);
            command.Parameters.AddWithValue("@assets", data["assets"]["count"]);
            command.Parameters.AddWithValue("@downloads", data["assets"]["downloads"]);
            command.Parameters.AddWithValue("@sprites", data["assets"]["sprites"]);
            command.Parameters.AddWithValue("@timestamp", DateTimeOffset.Now.ToUnixTimeSeconds());

            await command.ExecuteNonQueryAsync();
        }

        public async void WriteFurnitures(SocketClient client, JToken data) {
            using MySqlConnection connection = new MySqlConnection(Program.ConnectionTest);
            await connection.OpenAsync();

            JObject rates = (JObject)data["furnitures"]["rates"];

            foreach(var rate in rates) {
                using MySqlCommand command = new MySqlCommand("INSERT INTO furniture_rates (user, furniture, count, timestamp) VALUES (@user, @furniture, @count, @timestamp)", connection);
                command.Parameters.AddWithValue("@user", client.User.Id);
                command.Parameters.AddWithValue("@furniture", rate.Key);
                command.Parameters.AddWithValue("@count", rate.Value);
                command.Parameters.AddWithValue("@timestamp", DateTimeOffset.Now.ToUnixTimeSeconds());

                await command.ExecuteNonQueryAsync();
            }
        }

        public async void WriteFigures(SocketClient client, JToken data) {
            using MySqlConnection connection = new MySqlConnection(Program.ConnectionTest);
            await connection.OpenAsync();

            JObject rates = (JObject)data["figures"]["rates"];

            foreach(var rate in rates) {
                using MySqlCommand command = new MySqlCommand("INSERT INTO figure_rates (user, figure, count, timestamp) VALUES (@user, @figure, @count, @timestamp)", connection);
                command.Parameters.AddWithValue("@user", client.User.Id);
                command.Parameters.AddWithValue("@figure", rate.Key);
                command.Parameters.AddWithValue("@count", rate.Value);
                command.Parameters.AddWithValue("@timestamp", DateTimeOffset.Now.ToUnixTimeSeconds());

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
