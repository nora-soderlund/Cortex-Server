using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

using Server.Game.Badges;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

namespace Server.Socket.Events {
    class OnUnhandledException : ISocketEvent {
        public string Event => "OnUnhandledException";

        public int Execute(SocketClient client, JToken data) {
            string cache = client.Connection.ConnectionInfo.Headers["Cache-Control"];
            string agent = client.Connection.ConnectionInfo.Headers["User-Agent"];

            using MySqlConnection connection = new MySqlConnection(Program.ConnectionTest);
            connection.Open();

            using MySqlCommand command = new MySqlCommand("INSERT INTO user_exceptions (user, agent, cache, file, exception, stack, timestamp) VALUES (@user, @agent, @cache, @file, @exception, @stack, @timestamp)", connection);
            command.Parameters.AddWithValue("@user", client.User.Id);
            command.Parameters.AddWithValue("@agent", agent);
            command.Parameters.AddWithValue("@cache", cache);
            command.Parameters.AddWithValue("@file", data["file"]);
            command.Parameters.AddWithValue("@exception", data["exception"]);
            command.Parameters.AddWithValue("@stack", data["stack"]);
            command.Parameters.AddWithValue("@timestamp", DateTimeOffset.Now.ToUnixTimeSeconds());

            command.ExecuteNonQuery();

            client.Send(new SocketMessage("OnUnhandledException", true).Compose());

            if(Program.Discord != null)
                Program.Discord.Exception(client, data);

            return 1;
        }
    }
}
