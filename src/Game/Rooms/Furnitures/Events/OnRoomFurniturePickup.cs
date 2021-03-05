using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Server.Game;
using Server.Game.Users;
using Server.Game.Users.Furnitures;

using Server.Game.Rooms;
using Server.Game.Rooms.Users;
using Server.Game.Rooms.Actions;

using Server.Game.Furnitures;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

namespace Server.Game.Rooms.Furnitures.Events {
    class OnRoomFurniturePickup : ISocketEvent {
        public string Event => "OnRoomFurniturePickup";

        public int Execute(SocketClient client, JToken data) {
            if(client.User.Room == null)
                return 0;

            GameRoomUser roomUser = client.User.Room.GetUser(client.User.Id);

            if(!roomUser.HasRights())
                return 0;
            
            int id = data.ToObject<int>();

            GameRoomFurniture roomFurniture = client.User.Room.Furnitures.Find(x => x.Id == id);

            if(roomFurniture == null)
                return 0;

            client.User.Room.Furnitures.Remove(roomFurniture);

            roomFurniture.UserFurniture.Room = 0;

            client.User.Room.Send(new SocketMessage("OnRoomEntityRemove", new { furnitures = roomFurniture.Id }).Compose());

            client.Send(new SocketMessage("OnRoomFurniturePickup", roomFurniture.Id).Compose());

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("DELETE FROM room_furnitures WHERE id = @id", connection)) {
                    command.Parameters.AddWithValue("@id", roomFurniture.Id);

                    command.ExecuteNonQuery();
                }

                using(MySqlCommand command = new MySqlCommand("UPDATE user_furnitures SET room = 0 WHERE id = @id", connection)) {
                    command.Parameters.AddWithValue("@id", roomFurniture.UserFurniture.Id);

                    command.ExecuteNonQuery();
                }
            }

            GameUser owner = Game.GetUser(roomFurniture.UserFurniture.User);

            if(owner != null)
                owner.Client.Send(new SocketMessage("OnUserFurnitureUpdate", owner.GetFurnitureMessages(roomFurniture.UserFurniture.Furniture.Id)).Compose());

            return 1;
        }
    }
}
