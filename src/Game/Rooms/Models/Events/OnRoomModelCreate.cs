using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Rooms;
using Cortex.Server.Game.Rooms.Users;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Events;
using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Rooms.Models.Events {
    class OnRoomModelCreate : ISocketEvent {
        public string Event => "OnRoomModelCreate";

        public int Execute(SocketClient client, JToken data) {
            JToken properties = data["properties"];

            JToken title = properties["title"];
            JToken description = properties["description"];
            JToken category = properties["category"];
            JToken access = properties["access"];
            JToken password = properties["password"];

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("INSERT INTO rooms (user, title, description, category, access, password, map, door_row, door_column, door_direction) VALUES (@user, @title, @description, @category, @access, @password, @map, @door_row, @door_column, @door_direction)", connection)) {
                    command.Parameters.AddWithValue("@user", client.User.Id);
                    command.Parameters.AddWithValue("@title", (title == null)?("No room title"):(title));
                    command.Parameters.AddWithValue("@description", (description == null)?("No room description..."):(description));
                    command.Parameters.AddWithValue("@category", (category == null)?(1):(category));
                    command.Parameters.AddWithValue("@access", (access == null)?(1):(access));
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@map", data["map"]["map"]);
                    command.Parameters.AddWithValue("@door_row", data["map"]["door"]["row"]);
                    command.Parameters.AddWithValue("@door_column", data["map"]["door"]["column"]);
                    command.Parameters.AddWithValue("@door_direction", data["map"]["door"]["direction"]);

                    command.ExecuteNonQuery();

                    client.Send(new SocketMessage("OnRoomModelCreate", command.LastInsertedId).Compose());

                    GameRoomManager.AddUser(command.LastInsertedId, client.User);
                }
            }

            return 1;
        }
    }
}
