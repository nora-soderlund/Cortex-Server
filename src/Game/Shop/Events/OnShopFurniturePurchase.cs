using System;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Rooms;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

using Server.Game.Furnitures;
using Server.Game.Shop.Furnitures;

namespace Server.Game.Shop.Events {
    class OnShopFurniturePurchase : ISocketEvent {
        public string Event => "OnShopFurniturePurchase";

        public int Execute(SocketClient client, JToken data) {
            int id = data.ToObject<int>();

            GameShopFurniture shopFurniture = GameShop.Furnitures.Find(x => x.Id == id);

            if(shopFurniture == null)
                return 0;

            using MySqlConnection connection = new MySqlConnection(Program.Connection);
            connection.Open();

            using MySqlCommand command = new MySqlCommand("INSERT INTO user_furnitures (user, furniture) VALUES (@user, @furniture)", connection);

            command.Parameters.AddWithValue("@user", client.User.Id);
            command.Parameters.AddWithValue("@furniture", shopFurniture.Furniture.Id);

            int result = command.ExecuteNonQuery();

            if(result == 0)
                return 0;

            client.User.Furnitures.Add(GameFurnitureManager.GetGameUserFurniture((int)command.LastInsertedId));

            client.Send(new SocketMessage("OnShopFurniturePurchase", true).Compose());

            return 1;
        }
    }
}
