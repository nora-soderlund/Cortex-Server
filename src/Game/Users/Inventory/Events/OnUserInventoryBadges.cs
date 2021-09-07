using System;
using System.Linq;
using System.Collections.Generic;

using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Users.Furnitures;
using Cortex.Server.Game.Rooms;
using Cortex.Server.Game.Rooms.Users;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Events;
using Cortex.Server.Socket.Messages;

using Cortex.Server.Game.Users.Badges;

namespace Cortex.Server.Game.Users.Inventory.Events {
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

            if(userBadge.Equipped == false && client.User.Badges.Count(x =>x.Equipped) == 5)
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
