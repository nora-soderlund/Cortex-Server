using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;
using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Users.Furnitures;
using Server.Game.Rooms;
using Server.Game.Furnitures;
using Server.Game.Rooms.Users;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

namespace Server.Game.Users.Events {
    class Temp_DevFurniUpdate : ISocketEvent {
        public string Event => "Temp_DevFurniUpdate";

        public int Execute(SocketClient client, JToken data) {
            string id = data["id"].ToString();
            double depth = data["depth"].ToObject<double>();

            GameFurniture furniture = GameFurnitureManager.GetGameFurniture(id);

            if(furniture == null)
                return 0;

            if(Program.Discord != null)
                Program.Discord.Furniture(client.User, furniture, depth);

            furniture.Dimension.Depth = depth;

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("UPDATE furnitures SET depth = @depth WHERE id = @id", connection)) {
                    command.Parameters.AddWithValue("@id", furniture.Id);
                    command.Parameters.AddWithValue("@depth", furniture.Dimension.Depth);

                    command.ExecuteNonQuery();
                }
            }

            client.Send(new SocketMessage("Temp_DevFurniUpdate", true).Compose());

            return 1;
        }
    }
}
