using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;
using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Users.Furnitures;
using Server.Game.Rooms;
using Server.Game.Shop;
using Server.Game.Furnitures;
using Server.Game.Rooms.Users;
using Server.Game.Rooms.Users.Actions;
using Server.Game.Rooms.Furnitures;
using Server.Game.Rooms.Furnitures.Logics;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

namespace Server.Game.Users.Events {
    class Temp_DevFurniUpdate : ISocketEvent {
        public string Event => "Temp_DevFurniUpdate";

        public int Execute(SocketClient client, JToken data) {
            string id = data["id"].ToString();

            GameFurniture furniture = GameFurnitureManager.GetGameFurniture(id);

            if(furniture == null)
                return 0;

            if(data.SelectToken("depth") != null) {
                double depth = data["depth"].ToObject<double>();

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
            }

            if(data.SelectToken("logic") != null) {
                string logic = data["logic"].ToString();

                furniture.Logic = logic;

                using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                    connection.Open();

                    using(MySqlCommand command = new MySqlCommand("UPDATE furnitures SET logic = @logic WHERE id = @id", connection)) {
                        command.Parameters.AddWithValue("@id", furniture.Id);
                        command.Parameters.AddWithValue("@logic", furniture.Logic);

                        command.ExecuteNonQuery();
                    }
                }

                foreach(GameRoom room in GameRoomManager.Rooms) {
                    foreach(GameRoomFurniture roomFurniture in room.Furnitures) {
                        if(roomFurniture.UserFurniture.Furniture.Id != furniture.Id)
                            continue;

                        roomFurniture.Logic = GameRoomFurnitureLogics.CreateLogic(roomFurniture);
                    }
                }
            }

            client.Send(new SocketMessage("Temp_DevFurniUpdate", true).Compose());

            return 1;
        }
    }

    class Temp_DevShopUpdate : ISocketEvent {
        public string Event => "Temp_DevShopUpdate";

        public int Execute(SocketClient client, JToken data) {
            int id = data["id"].ToObject<int>();
            int icon = data["icon"].ToObject<int>();

            GameShopPage page = GameShop.Pages.Find(x => x.Id == id);

            if(page == null)
                return 0;

            if(Program.Discord != null)
                Program.Discord.Shop(client.User, page, icon);

            page.Icon = icon;

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("UPDATE shop SET icon = @icon WHERE id = @id", connection)) {
                    command.Parameters.AddWithValue("@id", page.Id);
                    command.Parameters.AddWithValue("@icon", page.Icon);

                    command.ExecuteNonQuery();
                }
            }

            client.Send(new SocketMessage("Temp_DevShopUpdate", true).Compose());

            return 1;
        }
    }

    class Temp_Slap : ISocketEvent {
        public string Event => "Temp_DevSlap";

        public int Execute(SocketClient client, JToken data) {
            int id = data.ToObject<int>();
            

            GameRoomUser targetUser = client.User.Room.Users.Find(x => x.Id == id);

            double depth = targetUser.Position.Depth;

            client.User.Room.Actions.AddEntity(targetUser.User.Id, 0, new GameRoomUserPosition(targetUser, targetUser.Position.Row, targetUser.Position.Column, depth + 10, 0, false));

            client.User.Room.Actions.AddEntity(targetUser.User.Id, 1000, new GameRoomUserPosition(targetUser, targetUser.Position.Row, targetUser.Position.Column, depth, 500, false));

            return 1;
        }
    }
}
