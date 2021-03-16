using System;
using System.Linq;
using System.Collections.Generic;

using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

using Server.Game.Users;
using Server.Game.Users.Furnitures;
using Server.Game.Rooms;
using Server.Game.Rooms.Users;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

using Server.Game.Users.Badges;

namespace Server.Game.Users.Inventory.Events {
    class OnUserInventoryBadges : ISocketEvent {
        public string Event => "OnUserInventoryBadges";

        public int Execute(SocketClient client, JToken data) {
            JToken token = data.SelectToken("id");

            if(token == null) {
                client.Send(new SocketMessage("OnUserInventoryBadges", client.User.Badges).Compose());
                
                return 1;
            }

            string badge = data["id"].ToString();

            GameUserBadge userBadge = client.User.Badges.Find(x => x.Badge == badge);

            if(userBadge == null)
                return 0;

            userBadge.Equipped = !userBadge.Equipped;
            userBadge.EquippedTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

            client.User.Badges = client.User.Badges.OrderBy(x => x.EquippedTimestamp).ThenBy(x => x.Timestamp).ToList();

            client.Send(new SocketMessage("OnUserInventoryBadges", true).Compose());

            using MySqlConnection connection = new MySqlConnection(Program.Connection);
            connection.Open();

            using MySqlCommand command = new MySqlCommand("UPDATE user_badges SET equipped = @equipped, equipped_timestamp = @equipped_timestamp WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", userBadge.Id);
            command.Parameters.AddWithValue("@equipped", userBadge.Equipped);
            command.Parameters.AddWithValue("@equipped_timestamp", userBadge.EquippedTimestamp);

            command.ExecuteNonQuery();

            return 1;
        }
    }
}
